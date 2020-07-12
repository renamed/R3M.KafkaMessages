#!/bin/bash

killall -I -q -w -e WritRead

docker-compose stop
docker-compose down
