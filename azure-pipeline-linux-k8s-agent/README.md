# How to run self hosted Azure Pipeline in Kubernetes

Azure DevOps provides only one pipeline for CI and CD. Issue occurs when we have multiple repositories in project and building them at the same time.

By having only one CI pipeline, Build time for projects get increased as they have to wait in the queue.

Solution for that is either get CI pipeline from Azure DevOps or Make a self hosted pipeline in Kubernetes Cluster.

>💡This guide shows how to host pipeline in kubernetes but you can also host pipeline in the docker. For that please check reference at the end of this guide.

## Steps
- Clone the following repository https://github.com/meet86/azure-pipeline-linux-agent

- Now that the repo is cloned, cd into that directory and, run:
  ```shell
  docker build -t azure-k8s-agent:latest .
  ```
- Push the image to docker hub or any other container registry that you can pull from.

- Please read the ```deployment.yaml``` and fill the environments variables.

- Once done, Run the following command,
  ```shell
  kubectl create -f deployment.yaml
  ``` 

- It'll create a Agent pod. You could check if it is configured properly or not by 
  ```shell
  kubectl logs -f <pod-name> -n <namespace>
  ```
- Also, You could check by navigating Azure DevOps > Organization Settings > Agent Pools > Default
  
- Now the configuration is over but at this moment I've faced issues where I was not able to use curl or can not fetch nuget packages while building .NET core project images

- **Solution**: In docker build task (Docker@2), make sure to pass the following configuration:
  
```yaml
- task: Docker@2
    inputs:
      repository: '$(repositoryName)'
      command: 'build'
      Dockerfile: '**/Dockerfile'
      tags: '$(tagName)'
      arguments: '--no-cache --network=host'
```
- Here ```arguments:``` ```--network=host``` did the job.

- - -

## Reference
https://docs.microsoft.com/en-us/azure/devops/pipelines/agents/docker?view=azure-devops