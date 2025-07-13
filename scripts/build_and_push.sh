#!/bin/bash
echo "Build images from compose"
docker-compose build --no-cache --pull

echo "Login to dockerhub: "
docker login -u artik3314 -p 123 docker.io

echo "Push services"

echo "Push service Nginx: "
docker tag react_nginx artik3314/react_nginx:latest
docker push artik3314/react_nginx:latest

echo "Push service Angular: "
docker tag react_angular artik3314/react_angular:latest
docker push artik3314/react_angular:latest

echo "Push service API: "
docker tag react_api artik3314/react_api:latest
docker push artik3314/react_api:latest