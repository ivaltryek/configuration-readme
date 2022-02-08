FROM jenkins/jenkins

USER root

RUN apt-get update && \
    apt-get install -y wget apt-transport-https unzip

RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-3.1

RUN curl -L "https://github.com/docker/compose/releases/download/1.29.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose && chmod +x /usr/local/bin/docker-compose

# Download Chrome Binary for Chrome Driver
RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && \
    apt install -y ./google-chrome-stable_current_amd64.deb && \
    rm ./google-chrome-stable_current_amd64.deb


# Download ChromeDriver
# VERSION=$(google-chrome --version | sed 's/[A-Za-z]*//g') => Removes Alphabets from output. 
# i.e Google Chrome 98.0.4758.80 -> 98.0.4758.80
# VERSION=$(echo $VERSION | xargs) => Removes whitespaces in version variable.
RUN VERSION=$(google-chrome --version | sed 's/[A-Za-z]*//g') && \
    VERSION=$(echo $VERSION | xargs) && \
    curl -L "https://chromedriver.storage.googleapis.com/$(echo $VERSION)/chromedriver_linux64.zip" -o /usr/local/bin/chromedriver.zip && \
    unzip /usr/local/bin/chromedriver.zip -d /usr/local/bin/ && \
    chmod +x /usr/local/bin/chromedriver && \
    rm /usr/local/bin/chromedriver.zip

# Check the installed versions
RUN google-chrome --version && chromedriver --version