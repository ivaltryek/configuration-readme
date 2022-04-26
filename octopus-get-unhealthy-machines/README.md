# Octopus: Get Unhealthy/Unavailable Machines for Environment

*octopus-get-unhealthy-machines.ps1*
```PowerShell
# Provide Octopus API Key to connect with REST APIs.
$Headers = @{'X-Octopus-ApiKey' = '<octopus-api-key>'}

$ErrorMsg = "There seems to be Unavailable machines"

# Octopus Server URl -- Ex: myserver.octopus.com
$ServerURL = "<octopus-server-url>"

# If you have not created any space. it is set to `Default`.
$SpaceName = 'Default'

# Get Space Id
$UsersURI = $ServerURL+"/api/users/me"
$UserData = Invoke-RestMethod -Method GET -Uri $UsersURI -Headers $Headers
$UserId = $UserData.Id
$SpaceIdURI = $ServerURL+"/api/users/"+$UserId+"/spaces"
$SpacesData = Invoke-RestMethod -Method GET -Uri $SpaceIdURI -Headers $Headers

For ($i=0; $i -lt $SpacesData.Length; $i++) {
    if ($SpacesData[$i].Name -eq $SpaceName) {
        $SpaceId = $SpacesData[$i].Id
    }
}

# Octopus Environment Name -- Ex: Doodle-Dev
$EnvironmentName = "<Environment-Name>"


# Get the Environment Id from the Environment Name
$EnvionmentIdURI = $ServerURL+"/api/"+$SpaceId+"/environments/summary"
$EnvironmentIds = Invoke-RestMethod -Method GET -Uri $EnvionmentIdURI -Headers $Headers
For ($i=0; $i -lt $EnvironmentIds.EnvironmentSummaries.Length; $i++) {
    if ($EnvironmentIds.EnvironmentSummaries[$i].Environment.Name -eq $EnvironmentName) {
        $EnvironmentId = $EnvironmentIds.EnvironmentSummaries[$i].Environment.Id
    }
}
# Check if the Environment Id is set or not.
if (([string]::IsNullOrEmpty($EnvironmentId)))
{
    Write-host "EnvironemntId could not be set, please re-enter the correct Environment name."
}

# Get Machines listed for Selected environment.
$MachineURI = $ServerURL+"/api/"+$SpaceId+"/machines?environmentids="+$EnvironmentId
$Machines = Invoke-RestMethod -Method GET -Uri $MachineURI -Headers $Headers

# Store them in an array.
[System.Collections.ArrayList]$MachineIds = @()
ForEach($i in $Machines.Items){
  [void]$MachineIds.Add(@($i.Id))
}

# Initialise empty array for Unavialble machines.
[System.Collections.ArrayList]$UnavailableMachines = @()

# Loop through all machine's status to check their health.
ForEach ($MachineId in $MachineIds) {

  $StatusURI = $ServerURL+"/api/"+$SpaceId+"/machines/$MachineId"
  $data = Invoke-RestMethod -Method GET -Uri $StatusURI -Headers $Headers
  # If they're not healthy, push them in `UnavailableMachines` array.
  if ($data.HealthStatus -eq "Unavailable") {
    [void]$UnavailableMachines.Add(@($data.Name))
  } 
}

# If `UnavailableMachines` has values in it...
if ($UnavailableMachines.Count -gt 0) {
    echo $ErrorMsg --------- "Here's the list: "
    # Print Unhealthy machines in the environment.
    ForEach($UnavailableMachine in $UnavailableMachines) {
      echo $UnavailableMachine
    }
}else{
    echo "All machines are Healthy and Available."
  }

```