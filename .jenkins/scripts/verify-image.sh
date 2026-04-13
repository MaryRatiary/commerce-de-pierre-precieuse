#!/bin/bash
set -e

echo "✅ Starting Image Verification with Cosign..."

IMAGE=$1
COSIGN_PUB=$2

if [ -z "$IMAGE" ] || [ -z "$COSIGN_PUB" ]; then
    echo "Usage: $0 <image> <cosign-pub-key-path>"
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

# Verify image signature
echo "🔍 Verifying signature for: $IMAGE"
$COSIGN_BIN verify --key $COSIGN_PUB $IMAGE

if [ $? -eq 0 ]; then
    echo "✅ Image signature verification PASSED!"
    exit 0
else
    echo "❌ Image signature verification FAILED!"
    exit 1
fi
