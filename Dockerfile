# build-go image
FROM golang:latest as build-go
WORKDIR /build

RUN apt-get update && apt-get install --assume-yes upx
COPY . /build
RUN make go-build
RUN upx --best --lzma coreapi

# Run image
FROM python:3
RUN apt update && \
    apt install mono-complete -y

RUN mkdir /app
RUN mkdir /data
RUN mkdir /app/python
COPY --from=build-go /build/coreapi /app
COPY --from=build-go /build/python /app/python
COPY --from=build-go /build/start.sh /app
COPY --from=build-go /build/.env.example /data/.env

# runtime params
WORKDIR /app
ENV PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin

EXPOSE 8080

CMD ["/bin/sh", "start.sh"]