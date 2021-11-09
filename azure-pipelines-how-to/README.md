# Azure Pipelines How To Guide

#### ✨ How to Set Variables in Release Pipeline

**Steps**
- Open Azure DevOps Project
- Go to the existing release Pipeline > Edit
- Select Stage 
- Select Variables
  
  ![select-vars-tab](https://user-images.githubusercontent.com/31511537/140730140-71b0f99a-c8c6-4aab-9c84-c5841229367f.png)

- Select Add

  ![vars-add](https://user-images.githubusercontent.com/31511537/140730545-57d8bcfe-419f-4a8a-9d50-fed3fc1a5af7.png)

- Make sure to select the correct stage before saving it, if not selected the desired one then later on the variable value won't be available for desired stage.
  
 ![create-var](https://user-images.githubusercontent.com/31511537/140731232-e1706d40-df12-46c3-9246-3e913d1a7b47.png)



 #### ✨ How to consume Pipeline variables.

 > To consume variables, simply put the variable name in between **$(here_goes_the_variable_name)**

#### ✨ How to add plugin in the Azure DevOps Project.

**Steps**

- To add the plugin visit [Azure Devops Marketplace](https://marketplace.visualstudio.com/azuredevops)

- Select the required extension and click on get it free.
  
  ![get-extension](https://user-images.githubusercontent.com/31511537/140732644-886f561a-62c8-43d9-bfe0-9685858243f1.png)

- It'll redirect to Azure DevOps Organization page, select one and it'll be added.

- To approve the extension request go to ```https://dev.azure.com/<your-org-name>/_settings/extensions?tab=requested&status=pending```
- Approve the pending extension for to use in Pipelines.