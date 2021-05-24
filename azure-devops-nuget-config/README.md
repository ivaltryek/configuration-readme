# Nuget Artifact Configuration

### nuget.config setup

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<packageSources>
		<clear />
		<add key="online-ordering-artifacts" value="https://pkgs.dev.azure.com/ftxinfotech/_packaging/online-ordering-artifacts/nuget/v3/index.json" />
		<add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
	</packageSources>

	<packageSourceCredentials>
		<online-ordering-artifacts>
			<add key="Username" value="<Usually Azure Devops Account Email Address>" />
			<add key="ClearTextPassword" value="<Personal Access Token>" />
		</online-ordering-artifacts>
	</packageSourceCredentials>

</configuration>


```

In,  `<add key="ClearTextPassword" value="<Personal Access Token>" />`the value needs to be changes every 90 days. Basically, value is PAT created from Azure devops account.

## Packaging the nupkg

### Command

    dontnet pack <project-name>.csproj --output <output-folder>

## Pushing the package to Nuget feed of Azure DevOps

### Prerequisites

 - Install Nuget.exe from [Nuget Downloads](https://www.nuget.org/downloads). Download the recommended one
 - Copy .exe file to C drive and make the folder called nuget and paste it in there.
 - Folder path should be look like this C:\nuget\
 - Add this path under the System variables. For the demo 
[Click here](https://drive.google.com/file/d/1gwwErHuYP-rigcJOYHcjMT1umSpk7Tot/view?usp=sharing)

- This steps required because now, you can access nuget anywhere from the system.

### Command

    nuget push -Source "<Feedname>" -ApiKey az <Package-name>.nupkg
    
   
   #### Example
   ``` nuget push -Source "online-ordering-artifacts" -ApiKey az FTXCore.Services.Identity.Integration.1.1.1.```
   

You should open the cmd where is your current package is or the other way, provide the absolute path of the package.

Once the command is fired, you'll be prompted to enter the Microsoft account credentials for the authentication.

For the confirmation that the package is successfully uploaded to the azure devops feed,
You'll see something like this at the end of the command process

> PUT https://pkgs.dev.azure.com/ftxinfotech/_packaging/4400b28f-d2bd-432d-b34d-62ae3d519c6b/nuget/v2/
  Accepted https://pkgs.dev.azure.com/ftxinfotech/_packaging/4400b28f-d2bd-432d-b34d-62ae3d519c6b/nuget/v2/ 5347ms
Your package was pushed.

## Restore Packages from Azure DevOps Artifacts

#### Command
> Run this command at the root of the project where the .csproj file or package.config exists.

``` nuget restore ```

## Common Problems

 Q. While pushing the package got the ,

> 	 The specified source 'MySource' is invalid. Please provide a valid
> source.

A. ```nuget sources Add -Name "MySource" -Source <url>```
	Here ```"MySource"``` is the feed name and  ```<url>``` is the artifact feed url.
	
Example

```nuget sources Add -Name "online-ordering-artifacts" -Source https://pkgs.dev.azure.com/ftxinfotech/_packaging/online-ordering-artifacts/nuget/v3/index.json```


 Q. Getting 401 Unauthorized while restoring artifacts.
 
 A. This may occure if you're restoring packages with ```dotnet restore```. Please try restoring using the ```nuget restore```.