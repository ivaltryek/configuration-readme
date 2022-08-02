# Python with Distroless

Use case for Distroless images is that they're extremely small due to no extra tools and shells inside of it. That also makes it secure compare to other images.

Here's example of how you can use distroless image with python

```dockerfile
FROM python:3.9.2-slim AS build-env
WORKDIR /app
COPY requirements.txt /app
RUN pip install -r requirements.txt  
COPY . /app

FROM gcr.io/distroless/python3
RUN python --version
COPY --from=build-env /app /app
COPY --from=build-env /usr/local/lib/python3.9/site-packages /usr/local/lib/python3.9/site-packages

WORKDIR /app
ENV PYTHONPATH=/usr/local/lib/python3.9/site-packages

CMD ["app.py"]
```

This dockerfile could be used with this repo: [Python-Colors](https://github.com/meet86/python-colors/)

## BreakDown of Dockerfile

```dockerfile
FROM python:3.9.2-slim AS build-env
WORKDIR /app
COPY requirements.txt /app
RUN pip install -r requirements.txt  
COPY . /app
```

This can be considered as build stage. In this stage, It'll build the app with necessary package required such as pip packages.

```dockerfile
FROM gcr.io/distroless/python3
RUN python --version
COPY --from=build-env /app /app
COPY --from=build-env /usr/local/lib/python3.9/site-packages /usr/local/lib/python3.9/site-packages

WORKDIR /app
ENV PYTHONPATH=/usr/local/lib/python3.9/site-packages

CMD ["app.py"]
```

This can be considered as run stage. In this stage we're using python3 distroless image and we're copying the dependency required by the app and running the app server.

## Building the image

```bash
docker build -t pycolors:distroless -f Dockerfile-distroless .
```

## Running an image inside Kubernetes cluster

**Note**: I'm running my Kubernetes cluster locally with the help of `minikube`.

```bash
kubectl run pycolor-distoless --image=pycolors:distroless --image-pull-policy=Never --port=5000
```

## Debugging the container

You can do this by either 2 ways:

**1st** is to create separate build with `:debug` tag 

For example: 
```dockerfile 
FROM gcr.io/distroless/python3:debug
```

**2nd** is to using Ephemeral Containers, for more info you can take a look [here](https://kubernetes.io/docs/concepts/workloads/pods/ephemeral-containers/).

**Note**: This works only with Kubernetes 1.23 or latter versions.

```bash
kubectl debug -it pycolor-distoless --image=ubuntu --copy-to=edistro --share-processes
```
Here:

`pycolor-distoless`: is a pod which is running a distroless image.<br/>
`--image=ubuntu`: is a name of image you want to use to start debug pod. <br/>
`--copy-to=edistro`: will create replica of distroless pod with name of edistro, you can name it with any name.<br/>
`--share-processes`: will share the process namespaces between containers.<br/>