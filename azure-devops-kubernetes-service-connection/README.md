# How to Add kubernetes service connection in Azure DevOps

#### Prerequisites
- ```kubectl``` utility
- Connected to Kubernetes cluster

**Steps**

- Go to your Azure DevOps Portal, the URL should be something like this: ```https://dev.azure.com/<organization>/<project-name>/_settings/adminservices```

- Click on New Service Connection.
  
  ![new-service-connection](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-kubernetes-service-connection/assets/new-service-connection.png)

- Select Kubernetes from the list.
  
  ![select-kubernetes](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-kubernetes-service-connection/assets/select-kubernetes.png)

- Select service account.
  
  ![select-service-account](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-kubernetes-service-connection/assets/select-service-account.png)

- Run the following command to get the server url.
  
  ``` 
  kubectl config view --minify -o jsonpath={.clusters[0].cluster.server}
  ```
  > 💡 If you're on zsh terminal, use the double quotes around jsonpath query
  ```
  kubectl config view --minify -o jsonpath="{.clusters[0].cluster.server}"
  ```

- Copy the output to the Server url field. which will look something like this,
  Mine's hosted on AWS, so it'll be different for every cloud providers.
  ```
  https://44444444444444444444444.gr8.us-east-1.eks.amazonaws.com
  ```

- Run the following command to get the secret name
  
  ```
    kubectl get serviceAccounts <service-account-name> -n <namespace> -o=jsonpath={.secrets[*].name}
  ```

- Run the following command to get the secret's value.
  
  ```
    kubectl get secret <service-account-secret-name> -n <namespace> -o json
  ```

- Copy the whole output to the Secret field in azure devops.

- Set the service connection name.
- Click on save.