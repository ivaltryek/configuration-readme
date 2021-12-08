# Istio Setup 
 
## Download Istio
```
curl -L https://istio.io/downloadIstio | sh -
```
 
## Install istio
```bash
# Default is recommended for Production ready clusters
istioctl install --set profile=default -y
```
 
> It'll install necessary components to run the istio.
 
## Troubleshooting
 
**Q**: It gives an error related to sandbox changed and restating the pod and it never assigns the IP to Pod. How to resolve this error?
 
**A**: In this case, adding ```hostNetwork:true``` under ```spec.template.spec``` to the istiod deployment may help.This seems to be workaround when using Calico CNI for Pod Networking.
 
> For more: https://stackoverflow.com/questions/67159786/istio-installation-successful-but-not-able-to-deploy-pod
 
**Q**: External Load Balancer is pending for Istio Ingress Gateway. How to resolve this error?
 
**A**: Make sure that *VPC* has the tag ```kubernetes.io/cluster/<cluster-name>``` and it's value should be ```owned``` where your cluster is located. Also make sure that your *public subnets* inside the cluster VPC have the tag ```kubernetes.io/role/elb``` and it's value should be ```1``` and ```kubernetes.io/cluster/<cluster-name>``` and it's value should be ```owned```. For *Private subnets* tag ```kubernetes.io/role/internal-elb``` and it's value should be ```1``` and ```kubernetes.io/cluster/<cluster-name>``` and it's value should be ```owned```. This is in case if there is need to assign internal Load Balancer.