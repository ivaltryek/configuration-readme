# Configure Hashicorp vault in Kubernetes

## 💡 This guide shows you how to install Vault in Kubernetes with any cloud providers.

### Steps:

- Download Hashicorp Vault Helm chart
  
  ```bash
  helm repo add hashicorp https://helm.releases.hashicorp.com
  helm repo update
  helm pull hashicorp/vault
  ```
    > This will add the helm repo and download the vault chart 

- Edit the ```values.yaml``` to add ingress configuration, by enabling ingress you can configure or access Vault using it's UI mode
  
  ```yaml
  ingress:
    enabled: true
    labels: {}
      # traffic: external
    annotations:
      kubernetes.io/ingress.class: nginx
      # |
      # kubernetes.io/ingress.class: nginx
      # kubernetes.io/tls-acme: "true"
      #   or
      # kubernetes.io/ingress.class: nginx
      # kubernetes.io/tls-acme: "true"

    # When HA mode is enabled and K8s service registration is being used,
    # configure the ingress to point to the Vault active service.
    activeService: true
    hosts:
      - host: # URL to access. for ex vault.myorg.com
        paths: []
    ## Extra paths to prepend to the host configuration. This is useful when working with annotation based services.
    extraPaths: []
    # - path: /*
    #   backend:
    #     serviceName: ssl-redirect
    #     servicePort: use-annotation
    tls: []
    #  - secretName: chart-example-tls
    #    hosts:
    #      - chart-example.local
  ```

- Now that it is configured, install the helm chart
  
  ```bash
    helm install vault hashicorp/vault \
    --set='server.ha.enabled=true' \
    --set='server.ha.raft.enabled=true'\
    --set='server.ha.replicas=1'
  ```

    > Here replica is intentionally kept to 1 for testing purposes.

- Once the pods are in running state, open the URL that is configured in ingress to Explore Vault UI.

<img src="https://i.imgur.com/7GQq68L.png"></img>