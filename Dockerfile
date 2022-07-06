ARG listen_port=6667

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
RUN \
    git clone https://github.com/IRC7/IRC7.git && \
    dotnet publish IRC7/Irc7d \
      --runtime alpine-x64 \
      --self-contained true \
      /p:PublishTrimmed=true \
      /p:PublishSingleFile=true \
      -c Release \
      -o ./output && \
    mv ./output/Irc7d ./output/irc7

FROM alpine:latest
ARG listen_port
ENV listen_port ${listen_port}
RUN \
    apk add libstdc++ krb5-libs
    #ln -s /etc/irc7/* ./
CMD ./irc7 --ip 0.0.0.0 --port $listen_port -fqdn localhost
EXPOSE ${listen_port}
