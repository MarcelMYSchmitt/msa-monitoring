# Introduction 
This project currently contains the watchdog and its message generator.
It's a semantic check, which is going to send messages to eventhub and tries to get the message from the backendforfrontend. 

If there are any problems, the watchdog is going to bark and we will see it in our grafana. 

# Getting Started

Create an .env file in the root of the project, and provide the following parameters:

```
EnvironmentTag=<<your environment tag which you gave into Create-Infrastructure.ps1>>
AzureRegionTag=<<your azure tag which you gave into Create-Infrastructure.ps1>>
EventHubSendConnectionString=please_replace
```

Run the application from visual studio or use the docker-compose up command. 
