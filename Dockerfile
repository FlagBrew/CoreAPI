# build-go image
FROM golang:latest as build-go
WORKDIR /build

RUN apt-get update && apt-get install --assume-yes upx
COPY . /build
RUN make go-build
RUN upx --best --lzma coreapi

# build-cs-release image
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-cs-release
WORKDIR /build

COPY . /build
RUN apt-get update && apt-get install --assume-yes make
RUN make cs-build-release


# Run image
FROM python:3
RUN apt update && \
    apt install mono-complete -y

RUN mkdir /app
RUN mkdir /data
RUN mkdir /app/css
COPY --from=build-go /build/coreapi /app
COPY --from=build-go /build/start.sh /app
COPY --from=build-go /build/.env.example /data/.env
COPY --from=build-cs-release /build/cc /app/cc

# runtime params
WORKDIR /app
ENV PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin

EXPOSE 8080

CMD ["/bin/sh", "start.sh"]