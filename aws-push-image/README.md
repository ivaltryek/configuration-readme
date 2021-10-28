# Push Image to AWS ECR

## Prerequisites
- AWS Account
- AWS CLI
- Docker

<hr />

## ✨ Building an Image

**Go to the Folder where Dockerfile is located**

Run the following command to build an image
```shell
docker build -t <tag-name> -f <docker-file-name> .
```

## ✨ Login to AWS ECR in order to push the image

```shell
aws ecr get-login-password --region <region> 
| docker login --username AWS --password-stdin <account-number>.dkr.ecr.<region>.amazonaws.com
```

## ✨ Tagging an Image

```shell
docker tag <tag-name> <account-number>.dkr.ecr.<region>.amazonaws.com/<ecr-repo>:<tag>
```
- ### Example
    ```shell
    docker build -t example:v1 -f ./Dockerfile .
    docker tag example:v1 123456778.dkr.ecr.us-east-2.amazonaws.com/example:v1
    ```

## ✨ Pushing an Image to ECR

```shell
docker push <account-number>.dkr.ecr.us-east-2.amazonaws.com/<ecr-repo>:<tag>
```

- ### Example
    ```shell
    docker push 123456778.dkr.ecr.us-east-2.amazonaws.com/example:v1
    ```