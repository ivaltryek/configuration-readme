# Karpenter AutoScaler

This guide is for how to setup Provisioner, if you want to install the Karpenter Autoscaler, visit [Docs](https://karpenter.sh/v0.8.2/getting-started/getting-started-with-eksctl/)

```yaml
apiVersion: karpenter.sh/v1alpha5
kind: Provisioner
metadata:
  name: default
spec:
  # If omitted, the feature is disabled and nodes will never expire.  If set to less time than it requires for a node
  # to become ready, the node may expire before any pods successfully start.
  # I'd prefer to not to set this value.
  ttlSecondsUntilExpired: 2592000 # 30 Days = 60 * 60 * 24 * 30 Seconds;

  # If omitted, the feature is disabled, nodes will never scale down due to low utilization
  ttlSecondsAfterEmpty: 30

  # Provisioned nodes will have these taints
  # Taints may prevent pods from scheduling if they are not tolerated
  taints:
    - key: example.com/special-taint
      effect: NoSchedule

  # Labels are arbitrary key-values that are applied to all nodes
  labels:
    billing-team: my-team

  # Requirements that constrain the parameters of provisioned nodes.
  # These requirements are combined with pod.spec.affinity.nodeAffinity rules.
  # Operators { In, NotIn } are supported to enable including or excluding values
  requirements:
    - key: "node.kubernetes.io/instance-type"
      operator: In
      values: ["m5.large", "m5.2xlarge"]
    - key: "topology.kubernetes.io/zone"
      operator: In
      values: ["us-west-2a", "us-west-2b"]
    - key: "kubernetes.io/arch"
      operator: In
      values: ["arm64", "amd64"]
    - key: "karpenter.sh/capacity-type" # If not included, the webhook for the AWS cloud provider will default to on-demand
      operator: In
      values: ["spot", "on-demand"]

  # Karpenter provides the ability to specify a few additional Kubelet args.
  # These are all optional and provide support for additional customization and use cases.
  kubeletConfiguration:
    clusterDNS: ["10.0.1.100"]

  # Resource limits constrain the total size of the cluster.
  # Limits prevent Karpenter from creating new instances once the limit is exceeded.
  limits:
    resources:
      cpu: "1000"
      memory: 1000Gi

  # These fields vary per cloud provider, see your cloud provider specific documentation
  provider:
    # Create new node in defined subnet.
    # Subnets may be specified by any AWS tag, including Name. Selecting tag values using wildcards ("*") is supported.
    # When launching nodes, Karpenter automatically chooses a subnet that matches the desired zone. 
    # If multiple subnets exist for a zone, the one with the most available IP addresses will be used.
    subnetSelector:
      Name: my-subnet-name
    # The security group of an instance is comparable to a set of firewall rules.
    # Security groups may be specified by any AWS tag, including “Name”. Selecting tags using wildcards ("*") is supported.
    securityGroupSelector:
      Name: eks-cluster-sg-2232
```