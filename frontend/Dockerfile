FROM node:18-alpine AS build
WORKDIR /app

COPY frontend/package*.json ./
RUN npm install --frozen-lockfile

COPY frontend/ ./
RUN npm run build


FROM nginx:alpine-slim

RUN addgroup -g 1000 www-user && \
    adduser -u 1000 -G www-user -s /sbin/nologin -D www-user

COPY --from=build /app/dist /usr/share/nginx/html
COPY frontend/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
