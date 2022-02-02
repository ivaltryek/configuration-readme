# Setup


⚠️ Please make sure that you have installed the **coverlet.msbuild** in test projects, you may install using: ```dotnet add package coverlet.msbuild```

- Start the SonarQube Server Container
  ```bash
  docker-compose up -d
  ```

- Create SonarQube Project.
- Start .NET tests
  ```bash
  dotnet test  /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
  ```
  ```/p:CollectCoverage=true``` - Collects Code Coverage

  ```/p:CoverletOutputFormat=opencover``` - Creates an XML opencover file

- Install .NET SonarScanner Plugin
  ```bash
  dotnet tool install --global dotnet-sonarscanner
  ```
- Start SonarScanner
  ```bash
  dotnet sonarscanner begin /k:"<project-name>" /d:sonar.host.url="<sonarqube-server-url>"  /d:sonar.login="<project-key>" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" /d:sonar.coverage.exclusions="**/**Test*.cs"
  ```
  ```project-name``` - Project Name

  ```sonarqube-server-url``` - SonarQube Server URL

  ```project-key``` - Onetime Available while creating Project in SonarQube Dashboard.

  ```/d:sonar.cs.opencover.reportsPaths``` - Path to an XML reports generated in ```dotnet test``` step.
  
  ```/d:sonar.coverage.exclusions``` - Exclude Test cases in code coverage.
  

  > ℹ️ If you get the error that says tool not found, please add the path. Run the following command:
  ```bash
  export PATH=$PATH:$HOME/.dotnet/tools
  ```
  ```

- Build the Project
  ```bash
  dotnet build
  ```

- End SonarScanner
  ```bash
  dotnet sonarscanner end /d:sonar.login="<project-key>"
  ```
  > ℹ️ If you get the error that says, java path not found, please install Java Runtime(JRE)

- Check your SonarQube Dashboard for updated data.