# Global Load balancer

This demo allows you to create a Global Standard Load Balancer with two regional Standard Load balancer as backend pool.


## Create the Demo
To create the demo, run the following command:

```
az deployment sub create --location <deployment location> --template-file main.bicep --parameters primaryLBLocation=<primary LB location> secondaryLBLocation=<secondary LB location> globalLBLocation=<global LB location> adminUsername=<admin username> adminPassword=<admin password>
```

where
- `<deployment location>` : The location to store the deployment metadata
- `<primary LB location>` : The location of one of the standard LB
- `<secondary LB location>` : The location of the other standard LB
- `<global LB location>` : The location of the global LB