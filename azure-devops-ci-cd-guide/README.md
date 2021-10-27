# Azure DevOps CI-CD Guide

## Prerequisite
- Existing Azure DevOps Project

## ➡️ Walkthrough for the CI

### **Navigate to existing project and click on the pipeline button to create the CI Pipeline.**
<br />
<img src="./assets/ci-navigation.png">

### **Click on New Pipeline**
<img src="./assets/ci-new-pipeline.png">

#### It'll open up stage process, follow the snapshots to create pipeline

- Select the Azure repos git option which will lead to selection of the project repository.

    <img src="./assets/ci-connect.png">

- Select the repository

    <img src="./assets/ci-select.png"> 

- You'll have to choose from the following two options:
  - Starter Pipeline: Which will create **azure-pipelines.yml** file in your repository with few startup steps
  - Choose existing: Select this one when you have **azure-pipelines.yml** already configured.

  - ✅ I'm using starter pipeline in upcoming configuration for more insights.
    <img src="./assets/ci-configure.png">


- If starter is selected, you'll be given starter yml template for azure pipeline which will look like the following:

    - Also note the highlighted area **Show assistant**, From here you can select various task for your needs. (i.e docker, ECR Pull or Push etc.)
    <img src="./assets/ci-review.png">

### 🏁 At this point CI Pipeline configuration is done. For how to create pipelines as per your needs you may have to look up the examples.

<hr />

## ➡️ Walkthrough for the CD

- To create CD, Click on the Release as shown in snapshot.
    
    <img src="./assets/cd-releases.png">

- Click on create release pipeline
    
    <img src="./assets/cd-create-new.png">

- You'll be prompted to select a stage task, select empty job to start from the scratch.
  
    <img src="./assets/cd-create-job.png">

- You'll have to choose artifacts to deploy the app through CD. Artifacts are basically are the dependencies those are needed at deployment time, also it can be a repository or drop or Build artifact generated from CI Pipeline.
  
    <img src="./assets/cd-artifact.png">

- Here you can set your stage name which will be displayed at the release page. (i.e dev, prod, stage etc.)
  
    <img src="./assets/cd-stage-configuration.png">

- Once the naming is done, you'll have to setup set of tasks in the stage.
    
    <img src="./assets/cd-set-tasks.png">

- You'll be redirected to the particular stage dashboard. Click on ➕ to add job tasks.
  
    <img src="./assets/cd-task-plus.png">

- You'll be prompted to select a tasks from the sidebar menu. (i.e helm task, deploy to app service etc.)
  
    <img src="./assets/cd-task-sidebar.png">

- Select the task as per your need and configure it.

⚠️Note: You can configure variables in from the variable menu for different stage. You can use them in any task by using **$(var_name)** syntax.

  
### Below is the example that deploys the app to the Kubernetes cluster using Helm.
  
- Overall Configuration:
  
    <img src="./assets/cd-stage-example.png">

- Stage Configuration - Helm install:
  - It'll install Helm tool in the Agent System.

    <img src="./assets/cd-task-one.png">

- Stage Configuration - Download Helm chart:
  - It'll download the helm chart which is stored at S3 Storage.
  
    <img src="./assets/cd-task-two.png">

- Stage Configuration - Helm Upgrade:
  - It'll start deploying or upgrading the specified helm chart to the Kubernetes cluster
  
    <img src="./assets/cd-task-three-one.png">
    
  - More task configurations continues:

    <img src="./assets/cd-task-three-two.png">    

    <img src="./assets/cd-task-three-three.png"> 


### 🏁 At this point CD Pipeline configuration is done. For how to create pipelines as per your needs you may have to look up the examples.