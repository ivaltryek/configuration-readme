# Send notification to Slack in Octopus Deploy using AWS Lambda

## Requirements
- AWS Account
- Slack Account
- Slack Incoming Webhook
- Octopus Deploy Account
  

## Lambda Setup

Create Lambda with the choice of your runtime, I've used the Python 3.9 runtime. Once Lambda generation is finished, Write the following code inside it.

```python
import json
import urllib3
http = urllib3.PoolManager()

def lambda_handler(event, context):
    print(event)
    print(context)
    
    data  = json.loads(event['body'])
    
    # Webhook URL of Slack
    url = "https://hooks.slack.com/services/XXXXXX/XXXXXX/XXXXXXXXX"
    
    msg = {
        # Slack channel name where you want to send your notifications.
        "channel": "octopus-notifications",
        "username": "Octopus Notifications",
        "text": data['Payload']['Event']['Message'],
        "icon_emoji": ""
    }
    
    encoded_msg = json.dumps(msg).encode('utf-8')
    resp = http.request('POST',url, body=encoded_msg)
    print({
        "message": data['Payload']['Event']['Message'],
        "status_code": resp.status, 
        "response": resp.data
    })

```

Save the code and deploy the function. To `expose` the function simply go to `Configuration` menu and select `function url` and set it to public access.

This will generate https function url.

## Octopus deploy configuration

- Go to `Configuration` in nav bar and select `Subscriptions` from Left Vertical menu.
- Create a Subscription and copy the AWS Lambda's Function URL in webhook settings.


That's it for the configuration. You can test this setup by setting up events in octopus and triggering it manually.