# Secret Store CSI Driver for Kubernetes

Secrets Store CSI Driver for Kubernetes secrets - Integrates secrets stores with Kubernetes via a Container Storage Interface (CSI) volume.

Visit [Docs](https://secrets-store-csi-driver.sigs.k8s.io/) for more.

I'm using the following setup:
- Hashicorp Vault (To store the Secrets)
- Minikube (For Local kubernetes cluster)

### Assuming you have configured Hashicorp vault, You'll still have to install csi driver from Hashicorp vault.

To Install: 

```bash
helm repo add hashicorp https://helm.releases.hashicorp.com
helm repo update
helm upgrade --install vault hashicorp/vault --namespace vault --create-namespace \
    --values helm-vault-values.yaml \
    --wait
```

Here's the `helm-vault-values.yaml`
```yaml
server:
  affinity: ""
  ha:
    enabled: true
csi:
  enabled: true
```

### Install the Secret Store CSI Driver

To install:

```bash
helm repo add secrets-store-csi-driver https://kubernetes-sigs.github.io/secrets-store-csi-driver/charts
helm repo update

helm upgrade --install csi-secrets-store secrets-store-csi-driver/secrets-store-csi-driver \
    --namespace kube-system \
    --set enableSecretRotation=true \
    --set rotationPollInterval=10s \
    --wait
```

Here:  <br>
    `enableSecretRotation=true`: for the auto update for secrets, meaning when you make change in secrets it'll automatically upadted.<br>
     `rotationPollInterval=10s`: for the checking new secrets.


### Creating Secret in vault

I have enabled Key-value engine with path `kv`, it might be different to what you have set as path.

Create the secret:

```bash
vault kv put kv/auth password="SoSimpleRight"
```

### You'll have to enable the kubernetes engine as we're going to use that. 

To do that:
```bash
vault auth enable kubernetes
vault write auth/kubernetes/config \
    issuer="https://kubernetes.default.svc.cluster.local" \
    token_reviewer_jwt="$(cat /var/run/secrets/kubernetes.io/serviceaccount/token)" \
    kubernetes_host="https://$KUBERNETES_PORT_443_TCP_ADDR:443" \
    kubernetes_ca_cert=@/var/run/secrets/kubernetes.io/serviceaccount/ca.crt
```

### Create policy to read the secret

```bash
vault policy write csi - <<EOF
path "kv/data/auth" {
  capabilities = ["read"]
}
EOF
```

### Bind policy to the role
```bash
vault write auth/kubernetes/role/csi \
   bound_service_account_names=sa-demo \
   bound_service_account_namespaces=default \
   policies=csi \
   ttl=20m
```

### Apply manifests file
```bash
kubectl apply -f manifests/
```

### Check the secrets if it's mounted properly.

```bash
kubectl exec -ti <pod-name> -- cat /mnt/secrets-store/password
```
