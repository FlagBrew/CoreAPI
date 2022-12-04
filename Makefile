.DEFAULT_GOAL := build-all

export PROJECT := "coreapi"
export PACKAGE := "github.com/FlagBrew/CoreAPI"

VERSION=$(shell git describe --tags --always --abbrev=0 --match=v* 2> /dev/null | sed -r "s:^v::g" || echo 0)
VERSION_FULL=$(shell git describe --tags --always --dirty --match=v* 2> /dev/null | sed -r "s:^v::g" || echo 0)

# General
clean:
	/bin/rm -rfv ${PROJECT}


# Docker
docker:
	docker compose \
		--project-name ${COMPOSE_PROJECT} \
		--file docker-compose.yaml \
		up \
		--remove-orphans \
		--build \
		--timeout 0 ${COMPOSE_ARGS}

docker-clean:
	docker compose \
		--project-name ${COMPOSE_PROJECT} \
		--file docker-compose.yaml \
		down \
		--volumes \
		--remove-orphans \
		--rmi local --timeout 1

docker-build:
	docker build \
		--tag ${PROJECT} \
		--force-rm .


# Python
python-fetch:
	python3 -m pip install -r python/requirements.txt

# Go
go-fetch:
	go mod download
	go mod tidy

go-upgrade-deps:
	go get -u ./...
	go mod tidy

go-upgrade-deps-patch:
	go get -u=patch ./...
	go mod tidy

go-dlv: go-fetch
	dlv debug \
		--headless --listen=:2345 \
		--api-version=2 --log \
		--allow-non-terminal-interactive \
		${PACKAGE} -- --debug

go-debug: go-fetch
	go run ${PACKAGE} --debug

go-debug-fast:
	go run ${PACKAGE} --debug

go-build: go-fetch
	CGO_ENABLED=0 \
	go build \
		-ldflags '-d -s -w -extldflags=-static' \
		-tags=netgo,osusergo,static_build \
		-installsuffix netgo \
		-buildvcs=false \
		-trimpath \
		-o ${PROJECT} \
		${PACKAGE}