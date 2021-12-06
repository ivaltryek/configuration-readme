# How to configure Hashicorp Vault with Postgresql as backend

> 💡 I've configured postgres on kubernetes in this tutorial but you can create on  any cloud provider too. Just for demonstration's sake, it was easier to setup.

### Postgresql Setup on Kubernetes
  ```bash
  # Create separate namespace for the Postgresql installation.
  kubectl create ns ns-pgsql

  # I'm using kubegres operator for installation of Postgresql. Run the following commands to install kubegres operator.
  kubectl apply -f https://raw.githubusercontent.com/reactive-tech/kubegres/v1.13/kubegres.yaml
  kubectl apply -f ./pgsqlsecret.yml
  kubectl apply -f ./pgsql.yml

 # Now to create DB, you'll have to connect master pod, which is usually named  `1` in the pod name.
 # Exec into the pod and run the following commands. 

# Create the DB to store the secrets, this is totally optional, by default you can use postgres DB which is created by server.

# Run the following query to create the DB.
CREATE DATABASE vault;

# Switch to the newly created DB.
\c vault;

# Create tables as per hashicorp docs. Here's schema.

CREATE TABLE vault_kv_store (
  parent_path TEXT COLLATE "C" NOT NULL,
  path        TEXT COLLATE "C",
  key         TEXT COLLATE "C",
  value       BYTEA,
  CONSTRAINT pkey PRIMARY KEY (path, key)
);

CREATE INDEX parent_path_idx ON vault_kv_store (parent_path);


CREATE TABLE vault_ha_locks (
  ha_key                                      TEXT COLLATE "C" NOT NULL,
  ha_identity                                 TEXT COLLATE "C" NOT NULL,
  ha_value                                    TEXT COLLATE "C",
  valid_until                                 TIMESTAMP WITH TIME ZONE NOT NULL,
  CONSTRAINT ha_key PRIMARY KEY (ha_key)
);
  ```

### Vault Setup

```bash
# Download the Vault helm chart.
helm repo add hashicorp https://helm.releases.hashicorp.com
helm repo update
helm pull hashicorp/vault

# Create kubernetes namespace to install vault.
kubectl create ns ns-vault
```
#### Edit the *Vaules.yaml*

```yaml
# Note that, this is only portion of the values.yaml, for better visibility I've only highlighted the area that requires change.

# Run Vault in "HA" mode. There are no storage requirements unless audit log
  # persistence is required.  In HA mode Vault will configure itself to use Consul
  # for its storage backend.  The default configuration provided will work the Consul
  # Helm project by default.  It is possible to manually configure Vault to use a
  # different HA backend.
  ha:
    enabled: true
    replicas: 2

    # Set the api_addr configuration for Vault HA
    # See https://www.vaultproject.io/docs/configuration#api_addr
    # If set to null, this will be set to the Pod IP Address
    apiAddr: null

    # Enables Vault's integrated Raft storage.  Unlike the typical HA modes where
    # Vault's persistence is external (such as Consul), enabling Raft mode will create
    # persistent volumes for Vault to store data according to the configuration under server.dataStorage.
    # The Vault cluster will coordinate leader elections and failovers internally.
    raft:

      # Enables Raft integrated storage
      enabled: false
      # Set the Node Raft ID to the name of the pod
      setNodeId: false

      # Note: Configuration files are stored in ConfigMaps so sensitive data
      # such as passwords should be either mounted through extraSecretEnvironmentVars
      # or through a Kube secret.  For more information see:
      # https://www.vaultproject.io/docs/platform/k8s/helm/run#protecting-sensitive-vault-configurations
      config: |
        ui = true

        listener "tcp" {
          tls_disable = 1
          address = "[::]:8200"
          cluster_address = "[::]:8201"
        }

        storage "raft" {
          path = "/vault/data"
        }

        service_registration "kubernetes" {}
    # config is a raw string of default configuration when using a Stateful
    # deployment. Default is to use a Consul for its HA storage backend.
    # This should be HCL.

    # Note: Configuration files are stored in ConfigMaps so sensitive data
    # such as passwords should be either mounted through extraSecretEnvironmentVars
    # or through a Kube secret.  For more information see:
    # https://www.vaultproject.io/docs/platform/k8s/helm/run#protecting-sensitive-vault-configurations
    config: |
      ui = true

      listener "tcp" {
        tls_disable = 1
        address = "[::]:8200"
        cluster_address = "[::]:8201"
      }
      storage "postgresql" {
        connection_url="postgres://postgres:74744gfgd@postgresql.ns-pgsql.svc.cluster.local:5432/vault?sslmode=disable",
        table="vault_kv_store",
        ha_enabled=true,
        ha_table="vault_ha_locks"
      }
      service_registration "kubernetes" {}
```

#### Install the helm chart with following command:
```bash
helm install vault ./vault -n ns-vault -f ./vault/values.yaml

# Wait for the Pods to get running, you may see that pods are running but it's not in ready state. for that you have to initialize and unseal the vault.

# To initialize the vault, run the following command:
kubectl exec -n ns-vault vault-0 -- vault operator init -key-shares=1 -key-threshold=1 -format=json > ccluster-keys.json

# This will initialize the vault and will create one file with contating keys to unseal the vault.
# To unseal the vault, you'll have to get the unseal key value
VAULT_UNSEAL_KEY=$(cat cluster-keys.json  | jq -r ".unseal_keys_b64[]")

# Now that it is stored at the shell variable, we can use them in command
kubectl exec -n ns-vault vault-1 -- vault operator unseal $VAULT_UNSEAL_KEY

# 💡 Depending on how many replicas that you have created, you may have to initialize and unseal as many pods in the cluster. Just change the vault-1 to other vault pod name and repeat the process till all the vault pods are in the ready state.

```
