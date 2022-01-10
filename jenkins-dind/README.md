# Steps

```bash
docker build -t jenkins-dind:latest .

docker run -itd -e JENKINS_USER=$(id -u) -v /var/run/docker.sock:/var/run/docker.sock -v $(which docker):/usr/bin/docker -p 8082:8080 -p 50000:50000 -u root jenkins-dind:latest
```