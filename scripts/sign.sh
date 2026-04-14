#!/bin/bash

# Script de signature d'images Docker avec Cosign
# Utilisation : ./scripts/sign.sh <image_uri> <keyfile>

set -e

IMAGE_URI="${1}"
KEYFILE="${2:-cosign.key}"
COSIGN_PASSWORD="${COSIGN_PASSWORD:-}"

if [ -z "$IMAGE_URI" ]; then
    echo "❌ Usage: ./scripts/sign.sh <image_uri> [keyfile]"
    echo "Example: ./scripts/sign.sh harbor.example.com/ecom/backend:abc12345 cosign.key"
    exit 1
fi

echo "✍️ Signing image: $IMAGE_URI"

if [ ! -f "$KEYFILE" ]; then
    echo "❌ Key file not found: $KEYFILE"
    echo "Generate with: cosign generate-key-pair"
    exit 1
fi

# Sign avec Cosign
cosign sign --key "$KEYFILE" "$IMAGE_URI" --yes

echo "✅ Image signed successfully!"
echo "To verify: cosign verify --key cosign.pub $IMAGE_URI"
