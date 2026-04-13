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
COPY nginx.conf /etc/nginx/nginx.conf

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost/index.html || exit 1

# Labels
LABEL org.opencontainers.image.title="E-Commerce Frontend"
LABEL org.opencontainers.image.description="React 18 + Vite frontend"
LABEL org.opencontainers.image.version="1.0.0"

EXPOSE 80
STOPSIGNAL SIGQUIT
