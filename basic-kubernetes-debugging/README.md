# Basic Kubernetes Debugging

## Prerequisites
- Basic Knowledge about Kubernetes
- kubectl utility installed
- Connected Kubernetes Cluster

<hr />

## Problems
**Q: How to resolve Pod CrashLoopBackoff?**

**A:** Run the following command to identify the issue.
   ```bash
   # This will get the logs of the pod
   kubectl logs -f <pod-name> -n <namespace>
   ```

   #### Other Possible reasons:
   - Check the environment configurations (ENV Variables).
   - Check the ENTRYPOINT or CMD in Dockerfile.
   - Check if there are other commands listed in Kubernetes Deployement file.
  
**Q: How to resolve the image pull issue?**

**A:** Run the following command to get the reason for image pull failure.

```bash
# This will get the details related to pod.
kubectl describe po <pod-name> -n <namespace>
```
> At the end, you'll see reasons sections for success and failure stages.

#### Sample Output:
```bash
Type     Reason     Age                From               Message
  ----     ------     ----               ----               -------
  Normal   Scheduled  32s                default-scheduler  Successfully assigned rk/nginx-deployment-6c879b5f64-2xrmt to aks-agentpool-x
  Normal   Pulling    17s (x2 over 30s)  kubelet            Pulling image "unreachableserver/nginx:1.14.22222"
  Warning  Failed     16s (x2 over 29s)  kubelet            Failed to pull image "unreachableserver/nginx:1.14.22222": rpc error: code = Unknown desc = Error response from daemon: pull access denied for unreachableserver/nginx, repository does not exist or may require 'docker login': denied: requested access to the resource is denied
  Warning  Failed     16s (x2 over 29s)  kubelet            Error: ErrImagePull
  Normal   BackOff    5s (x2 over 28s)   kubelet            Back-off pulling image "unreachableserver/nginx:1.14.22222"
  Warning  Failed     5s (x2 over 28s)   kubelet            Error: ImagePullBackOff

```

#### Other Possible reasons:
- If you're using different registry other than where the cluster is located, you may have to provide secret in Deployment file and also create one in Kubernetes cluster.

**Q: How to restart an application pod?**

**A:** ```kubectl delete <pod-name> -n <namespace> 
        ```

**Q: How to check pod configuration?**

**A:** ```kubectl describe <pod-name> -n <namespace>```

**Q: How to resolve Pod Pending issue?**

**A:** Possible list of commands to check for errors.
```bash
# Describe the Pod Configuration and get the stages events.
kubectl describe <pod-name> -n <namespace>

# Get the overall events
kubectl get events

# Get the pod logs
kubectl logs -f <pod-name> -n <namespace>
```