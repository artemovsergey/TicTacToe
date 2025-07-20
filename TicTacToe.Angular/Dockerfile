FROM node:latest
WORKDIR /app
COPY TicTacToe.Angular/package*.json ./
RUN npm install
COPY ./TicTacToe.Angular .
RUN npm run build --prod
RUN npm install -g @angular/cli
EXPOSE 4200
CMD ["ng", "serve","-c", "production", "--host", "0.0.0.0"]
