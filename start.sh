#!/bin/bash

# chmod 0755 start.sh

if [ "$#" -ne 1 ]; then
    echo "Preciso do número de pares de programas a serem executados"
    exit 2
fi

docker-compose rm -svf
docker-compose up --build -d

cd WritRead/

dotnet build
rc=$?; if [[ $rc != 0 ]]; then exit $rc; fi

X=1
for i in $(seq "$1")
do
    Y=$((X+1))
    SERV1_NAME="SERV_${X}"
    SERV2_NAME="SERV_${Y}"
    TOPIC_1="X_${X}"
    TOPIC_2="X_${Y}"    
    x-terminal-emulator -e dotnet run $SERV1_NAME $TOPIC_1 $TOPIC_2
    x-terminal-emulator -e dotnet run $SERV2_NAME $TOPIC_2 $TOPIC_1
    X=$((X+2))
done