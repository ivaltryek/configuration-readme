# Dependabot Configuration
Dependabot Configuration

Steps:

1.  Create dependabot folder into a branch.
    

2.  Inside create the following files:
    

1.  azure-pipeline-config.yml
    

2.  Gemfile
    

3.  update.rb
    

###  File Contents:

#### azure-pipeline-config.yml

```yaml 
trigger:
  branches:
    include:
      - security


pool:
  vmImage: 'Ubuntu-16.04'

steps:
- task: UseRubyVersion@0
  inputs:
    versionSpec: '=2.6.7'

- script: |
    cd $(System.DefaultWorkingDirectory)/dependabot
    gem install bundler
    bundle install --retry=3 --jobs=4
  displayName: 'bundle install'

- script: |
    cd $(System.DefaultWorkingDirectory)/dependabot
    echo "-----------------"
    export DEPENDABOT_NATIVE_HELPERS_PATH="$(pwd)/native-helpers"
    mkdir -p $DEPENDABOT_NATIVE_HELPERS_PATH/{terraform,python,dep,go_modules,hex,composer,npm_and_yarn}
    export PATH="$PATH:$DEPENDABOT_NATIVE_HELPERS_PATH/terraform/bin:$DEPENDABOT_NATIVE_HELPERS_PATH/python/bin:$DEPENDABOT_NATIVE_HELPERS_PATH/go_modules/bin:$DEPENDABOT_NATIVE_HELPERS_PATH/dep/bin"
    export MIX_HOME="$DEPENDABOT_NATIVE_HELPERS_PATH/hex/mix"

    cp -r $(bundle show dependabot-npm_and_yarn)/helpers $DEPENDABOT_NATIVE_HELPERS_PATH/npm_and_yarn/helpers
    $DEPENDABOT_NATIVE_HELPERS_PATH/npm_and_yarn/helpers/build $DEPENDABOT_NATIVE_HELPERS_PATH/npm_and_yarn

    bundle exec ruby ./update.rb
    echo "-----------------"
  env:
    SYSTEM_ACCESSTOKEN: $(System.AccessToken)
  displayName: 'Update dependencies'



```

#### Gemfile
```gemfile 
# frozen_string_literal: true
ruby "2.6.7"
source "https://rubygems.org"

gem "irb"
gem "dependabot-omnibus", "~> 0.138.1"
```

#### update.rb

```ruby
require "dependabot/omnibus"

package_manager = "nuget"
repo = "<organization>/<project-name>/_git/<repository-name>" # For google/doodle/_git/doodle_repo
branch = "<branchname>" # Target Branch of PR 

credentials = [{
  "type" => "git_source",
  "host" => "dev.azure.com",
  "username" => "x-access-token",
  "password" => "<PAT TOKEN>"
},

{
  "type" => "git_source",
  "host" => "github.com",
  "username" => "x-access-token",
  "password" => "<GITHUB TOKEN>" # A GitHub access token with read access to public repos
},

{
  "type" => "nuget_feed",
  "url" => "https://myabilities.pkgs.visualstudio.com/Pillar/_packaging/Pillar-modules/nuget/v3/index.json",
  "token" => ":#{ENV["SYSTEM_ACCESSTOKEN"]}"
}

]

source = Dependabot::Source.new(
  provider: "azure",
  repo: repo,
  hostname: "dev.azure.com",
  api_endpoint: "https://dev.azure.com/",
  branch: branch
)

fetcher = Dependabot::FileFetchers.for_package_manager(package_manager).new(
  source: source,
  credentials: credentials,
)

files = fetcher.files
commit = fetcher.commit 

parser = Dependabot::FileParsers.for_package_manager(package_manager).new(
  dependency_files: files,
  source: source,
  credentials: credentials,
)

dependencies = parser.parse

dependencies.select(&:top_level?).each do |dep|
  puts "Found #{dep.name} @ #{dep.version}..."

  checker = Dependabot::UpdateCheckers.for_package_manager(package_manager).new(
    dependency: dep,
    dependency_files: files,
    credentials: credentials,
  )

  if checker.up_to_date?
    puts "  already using latest version"
    next
  end

  requirements_to_unlock =
    if !checker.requirements_unlocked_or_can_be?
      if checker.can_update?(requirements_to_unlock: :none) then :none
      else :update_not_possible
      end
    elsif checker.can_update?(requirements_to_unlock: :own) then :own
    elsif checker.can_update?(requirements_to_unlock: :all) then :all
    else :update_not_possible
    end

  next if requirements_to_unlock == :update_not_possible

  updated_deps = checker.updated_dependencies(
    requirements_to_unlock: requirements_to_unlock
  )

  puts "  considering upgrade to #{checker.latest_version}"
  updater = Dependabot::FileUpdaters.for_package_manager(package_manager).new(
    dependencies: updated_deps,
    dependency_files: files,
    credentials: credentials,
  )

  updated_files = updater.updated_dependency_files

  pr_creator = Dependabot::PullRequestCreator.new(
    source: source,
    base_commit: commit,
    dependencies: updated_deps,
    files: updated_files,
    credentials: credentials,
    label_language: true,
    author_details: {
      email: "dependabot@myabilities",
      name: "dependabot"
    },
  )

  pull_request = pr_creator.create

  if pull_request&.status == 201
    content = JSON[pull_request.body]

    puts "  PR ##{content["pullRequestId"]} submitted"
  else
    puts "  PR already exists or an error has occurred"
  end

  next unless pull_request
end

```

### Changes to make to work with different package managers

#### azure-pipeline-config.yml

Need to make changes, for the these lines:
```
cp -r $(bundle show dependabot-npm_and_yarn)/helpers $DEPENDABOT_NATIVE_HELPERS_PATH/npm_and_yarn/helpers
$DEPENDABOT_NATIVE_HELPERS_PATH/npm_and_yarn/helpers/build $DEPENDABOT_NATIVE_HELPERS_PATH/npm_and_yarn

```
#### Note
> For nuget package manager, there is no need to perform the above helper step. Dependabot provides nuget helpers internally.

To see, different commands for the different package managers: 
Visit: [https://github.com/dependabot/dependabot-script](https://github.com/dependabot/dependabot-script) , and check the readme of the repo

#### update.rb

Need to make changes for these lines:

```ruby
package_manager = "npm_and_yarn" 
repo = "myabilities/PitchAI/_git/PitchAI-Portal"
```

for package_manager, these many options are available:
Valid values: `bundler`, `cargo`, `composer`, `dep`, `docker`, `elm`, `go_modules`, `gradle`, `hex`, `maven`, `npm_and_yarn`, `nuget`, `pip` (includes pipenv), `submodules`, `terraform`

The github token: is generated through the github.com personal account with the only access with of the reading the public repo. This was required because it was throwing the Limited requests error for fetching the repo anonymously.

Need to make changes for these lines:

    {
      "type" => "npm_and_yarn_feed",
      "url" => "<artifact-url>",
      "token" => ":#{ENV["SYSTEM_ACCESSTOKEN"]}"
    }

type should be in this format: 
<package_manager>_feed

URL should be the artifact URL. Which can be accessed from the Azure Artifact menu.
