# Send Jenkins Build Status to Bitbucket

## Steps

### Configuration

- Install Jenkins Plugin Bitbucket build status notifier, Find out more from [here](https://plugins.jenkins.io/bitbucket-build-status-notifier/).
- Create OAuth Credentials Inside Bitbucket. Visit [this](https://support.atlassian.com/bitbucket-cloud/docs/use-oauth-on-bitbucket-cloud/) docs for step by step walkthrough.
- Once you reach the Oauth Page, Fill in the details. Here's an example:
  - Name: Jenkins Build status (This could be anything)
  - Description: Optional
  - Callback URL: http://my-jenkins-ci.doodle.com (Jenkins dashboard URL, please note no forward slash (/) at the end of the URL)
  - Permissions: Go to *Repository* Section, - [x] Read 
                                             - [x] Write
- Please make sure that, you have **ticked** *private consumer checkmark* while creating Oauth Token.

    <img src="https://user-images.githubusercontent.com/31511537/154633597-33a0f287-cf20-4132-a519-5effcc49343b.png" width="550">

- Copy Oauth Key and Secret, save them as Username and Password Jenkins credentials.

### Usage 

```groovy

pipeline {
    agent any
    stages {
        stage("dev") {
            when {
            expression {
              return env.GIT_BRANCH == "origin/ci_cd_dev"
            }
            }
            steps {
                sh''' 
                echo "dev test ..........,"
                '''
            }
        }
        stage("test") {
         when {
            expression {
              return env.GIT_BRANCH == "origin/ci_cd_test"
            }
         }
            steps {
                echo "test test ....c....,"
            }   
        }
    }
    
    post {
        success {
           // Pass Jenkins Credential ID, the one you stored Oauth details with type of Username and Password
           bitbucketStatusNotify(buildState: 'SUCCESSFUL', credentialsId: 'oauth-test')
        }
        failure {
           // Pass Jenkins Credential ID, the one you stored Oauth details with type of Username and Password
           bitbucketStatusNotify(buildState: 'FAILED', credentialsId: 'oauth-test')
        }
    }
}


```