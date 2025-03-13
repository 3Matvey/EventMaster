#!/bin/sh

echo "Replacing environment variables in config.template.json..."
envsubst < /usr/share/nginx/html/config.template.json > /usr/share/nginx/html/config.json

echo "Starting Nginx..."
exec nginx -g 'daemon off;'
