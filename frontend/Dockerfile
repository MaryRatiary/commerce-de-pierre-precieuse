# Multi-stage build - Frontend
FROM node:18-alpine AS build
WORKDIR /app

# Install dependencies
COPY package*.json ./
RUN npm install --frozen-lockfile

# Build application
COPY . ./
RUN npm run build

# Serve stage - minimal nginx
FROM nginx:alpine-slim

# Create non-root user
RUN addgroup -g 1000 www-user && \
    adduser -u 1000 -G www-user -s /sbin/nologin -D www-user

# Copy built app
COPY --from=build /app/dist /usr/share/nginx/html

# Copy nginx config
COPY << 'NGINX_EOF' /etc/nginx/nginx.conf
user www-user;
worker_processes auto;
error_log /var/log/nginx/error.log warn;
pid /var/run/nginx.pid;

events {
    worker_connections 1024;
}

http {
    include /etc/nginx/mime.types;
    default_type application/octet-stream;

    log_format main '$remote_addr - $remote_user [$time_local] "$request" '
                    '$status $body_bytes_sent "$http_referer" '
                    '"$http_user_agent" "$http_x_forwarded_for"';

    access_log /var/log/nginx/access.log main;

    sendfile on;
    tcp_nopush on;
    keepalive_timeout 65;
    gzip on;
    gzip_types text/plain text/css text/javascript application/json;

    server {
        listen 80;
        server_name _;

        # Security headers
        add_header X-Frame-Options "SAMEORIGIN" always;
        add_header X-Content-Type-Options "nosniff" always;
        add_header X-XSS-Protection "1; mode=block" always;
        add_header Referrer-Policy "no-referrer-when-downgrade" always;
        add_header Content-Security-Policy "default-src 'self' https: data: 'unsafe-inline' 'unsafe-eval';" always;

        location / {
            root /usr/share/nginx/html;
            try_files $uri $uri/ /index.html;
        }

        location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ {
            expires 1y;
            add_header Cache-Control "public, immutable";
        }
    }
}
NGINX_EOF

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost/index.html || exit 1

# Labels
LABEL org.opencontainers.image.title="E-Commerce Frontend"
LABEL org.opencontainers.image.description="React 18 + Vite frontend"
LABEL org.opencontainers.image.version="1.0.0"

EXPOSE 80
STOPSIGNAL SIGQUIT
