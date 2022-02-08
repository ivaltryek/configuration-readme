# .NET Core and Selenium with Jenkins (Docker in Docker)

- To run the setup
  ```bash
  docker-compose up -d
  ```

### Running Manually in local machine

- Download Chrome Browser.
- Download Chrome Driver.
  > ℹ️ Version of Chrome Driver and Chrome Browser must be identical.
- ```cd``` into directory where ```.sln``` file is and run
  ```bash
  dotnet build
  dotnet test
  ```