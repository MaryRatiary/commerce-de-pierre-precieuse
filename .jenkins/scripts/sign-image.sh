#!/bin/bash
set -e

echo "🔐 Starting Image Signing with Cosign..."

IMAGE=$1
COSIGN_KEY=$2

if [ -z "$IMAGE" ] || [ -z "$COSIGN_KEY" ]; then
    echo "Usage: $0 <image> <cosign-key-path>"
    exit 1
fi

# Check if cosign is installed
if ! command -v cosign &> /dev/null; then
    echo "📦 Installing Cosign..."
    curl -sSL https://github.com/sigstore/cosign/releases/latest/download/cosign-linux-amd64 -o cosign
    chmod +x cosign
    COSIGN_BIN="./cosign"
else
    COSIGN_BIN="cosign"
fi

# Sign image
echo "🔑 Signing image: $IMAGE"
$COSIGN_BIN sign --key $COSIGN_KEY $IMAGE

echo "✅ Image signed successfully!"
