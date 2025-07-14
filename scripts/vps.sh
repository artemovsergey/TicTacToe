#!/bin/bash
echo "Go to project"
cd /home/user1/tictactoe/

echo "Stop all containers"
docker-compose down

echo "Update new version images"
docker-compose pull

echo "Up"
docker-compose up -d --build

echo "Status"
docker ps


