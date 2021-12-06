# Azure DevOps CI-CD Guide

## Prerequisite
- Existing Azure DevOps Project

## ➡️ Walkthrough for the CI

### **Navigate to existing project and click on the pipeline button to create the CI Pipeline.**
<br />

![ci-navigation](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/ci-navigation.png)
### **Click on New Pipeline**
![ci-new-pipeline](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/ci-new-pipeline.png)

#### It'll open up stage process, follow the snapshots to create pipeline

- Select the Azure repos git option which will lead to selection of the project repository.

    ![ci-connect](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/ci-connect.png)

- Select the repository

    ![ci-select](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/ci-select.png)

- You'll have to choose from the following two options:
  - Starter Pipeline: Which will create **azure-pipelines.yml** file in your repository with few startup steps
  - Choose existing: Select this one when you have **azure-pipelines.yml** already configured.

  - ✅ I'm using starter pipeline in upcoming configuration for more insights.
    ![ci-configure](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/ci-configure.png)


- If starter is selected, you'll be given starter yml template for azure pipeline which will look like the following:

    - Also note the highlighted area **Show assistant**, From here you can select various task for your needs. (i.e docker, ECR Pull or Push etc.)
    ![ci-review](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/ci-review.png)

### 🏁 At this point CI Pipeline configuration is done. For how to create pipelines as per your needs you may have to look up the examples.

<hr />

## ➡️ Walkthrough for the CD

- To create CD, Click on the Release as shown in snapshot.
    
    ![cd-releases](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-releases.png)

- Click on create release pipeline
    
    ![cd-create-new](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-create-new.png)

- You'll be prompted to select a stage task, select empty job to start from the scratch.
  
    ![cd-create-job](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-create-job.png)

- You'll have to choose artifacts to deploy the app through CD. Artifacts are basically are the dependencies those are needed at deployment time, also it can be a repository or drop or Build artifact generated from CI Pipeline.
  
    ![cd-artificate](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-artifact.png)

- Here you can set your stage name which will be displayed at the release page. (i.e dev, prod, stage etc.)
  
    ![cd-stage-configuration](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-stage-configuration.png)

- Once the naming is done, you'll have to setup set of tasks in the stage.
    
    ![cd-set-tasks](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-set-tasks.png)

- You'll be redirected to the particular stage dashboard. Click on ➕ to add job tasks.
  
    ![cd-task-plus](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-plus.png)

- You'll be prompted to select a tasks from the sidebar menu. (i.e helm task, deploy to app service etc.)
  
    ![cd-task-sidebar](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-sidebar.png)

- Select the task as per your need and configure it.

⚠️Note: You can configure variables in from the variable menu for different stage. You can use them in any task by using **$(var_name)** syntax.

  
### Below is the example that deploys the app to the Kubernetes cluster using Helm.
  
- Overall Configuration:
  
    ![cd-stage-example](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-stage-example.png)

- Stage Configuration - Helm install:
  - It'll install Helm tool in the Agent System.

    ![cd-task-one](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-one.png)

- Stage Configuration - Download Helm chart:
  - It'll download the helm chart which is stored at S3 Storage.
  
    ![cd-task-two](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-two.png)

- Stage Configuration - Helm Upgrade:
  - It'll start deploying or upgrading the specified helm chart to the Kubernetes cluster
  
    ![cd-task-three-one](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-three-one.png)
    
  - More task configurations continues:

    ![cd-task-three-two](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-three-two.png)    

    ![cd-task-three-three](https://raw.githubusercontent.com/meet86/configuration-readme/main/azure-devops-ci-cd-guide/assets/cd-task-three-three.png)


#### 🏁 At this point CD Pipeline configuration is done. For how to create pipelines as per your needs you may have to look up the examples.

## Example of the CI Pipeline YAML configuration

- Docker build and push to registry
  
  ```yaml
  trigger:
  batch: true
  branches:
    include:
    - ci_cd_bs4goftx # branches.include: mention branch names to add pipeline trigger.
  paths:
    exclude:
    - k8s/* # paths.exclude: mention path name to exclude pipeline trigger. It won't trigger if any changes occurred in mentioned path.

  pool: cc-prod-k8s-pool # Pool: Which pool to use to run the builds.

  variables: # Mention variables to use them in further in the pipeline. To use, just insert variable name in $() syntax.
    solution: '**/*.sln'
    buildPlatform: 'Any CPU'
    buildConfiguration: 'Release'
    repositoryName: 'controlcenter'
    dockerFileName: './Dockerfile'
    buildContext: '$(System.DefaultWorkingDirectory)' # This is pre-defined variable. to see more, visit https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables?view=azure-devops&tabs=yaml
    imageName: $(repositoryName)
    tagName: $(Build.BuildNumber)
    
  steps:

  - task: ECRPullImage@1 # Pulls the image from ECR Repository.
    inputs:
      awsCredentials: 'aws-sc' # Service connection name that connects to an AWS Account.
      regionName: 'us-east-2'
      repository: '$(repositoryName)' # ECR Repository name.
      imageSource: 'imagetag'
      imageTag: 'baseV2' # Image tag filter.

  - task: Docker@2 # Builds the docker image from Dockerfile.
    displayName: 'Docker build - bs4'
    inputs:
      repository: '$(repositoryName)-bs4'
      command: 'build'
      Dockerfile: '$(dockerFileName)-bs4' # Dockerfile name.
      buildContext: '$(buildContext)' # Docker context. Generally where the Dockerfile is located.
      tags: '$(tagName)'
      arguments: '--network=host' # has to be given, if you're building an image inside docker in docker configuration.
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/ci_cd_bs4goftx') # Activate this task only if it matches the given condition.

  - task: ECRPushImage@1 # Push the built image to ECR.
    displayName: 'Push to ECR - bs4'
    inputs:
      awsCredentials: 'AWS service connection'
      regionName: 'us-east-2'
      imageSource: 'imagename'
      sourceImageName: '$(imageName)-bs4'
      sourceImageTag: '$(tagName)'
      repositoryName: '$(imageName)-bs4'
      pushTag: '$(tagName)'
      autoCreateRepository: true
    condition: eq(variables['Build.SourceBranch'], 'refs/heads/ci_cd_bs4goftx')


  ```