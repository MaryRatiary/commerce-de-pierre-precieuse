#!/bin/bash

# Script de vérification de signature d'images Docker avec Cosign
# Utilisation : ./scripts/verify.sh <image_uri> [pubkey]

set -e

IMAGE_URI="${1}"
PUBKEY="${2:-cosign.pub}"

if [ -z "$IMAGE_URI" ]; then
    echo "❌ Usage: ./scripts/verify.sh <image_uri> [pubkey]"
    echo "Example: ./scripts/verify.sh harbor.example.com/ecom/backend:abc12345 cosign.pub"
    exit 1
fi

echo "🔍 Verifying signature for: $IMAGE_URI"

if [ ! -f "$PUBKEY" ]; then
    echo "❌ Public key file not found: $PUBKEY"
    exit 1
fi

# Verify signature
if cosign verify --key "$PUBKEY" "$IMAGE_URI"; then
    echo "✅ Signature verified successfully!"
    exit 0
else
    echo "❌ Signature verification failed!"
    exit 1
fi
