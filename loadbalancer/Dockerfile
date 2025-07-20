FROM nginx:alpine
RUN mkdir -p /etc/nginx/ssl
COPY ./loadbalancer/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80 443
CMD ["nginx", "-g", "daemon off;"]
