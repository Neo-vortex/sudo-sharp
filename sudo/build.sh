#!/bin/bash
set -e

DOCKER_IMAGE="aot-builder"
CONTAINER_NAME="aot-container"
OUTPUT_DIR="./out"

echo "🔧 Building Docker image..."
docker build -t $DOCKER_IMAGE .

echo "📦 Creating container..."
docker create --name $CONTAINER_NAME $DOCKER_IMAGE

echo "📤 Copying built binary from container to host..."
mkdir -p $OUTPUT_DIR
docker cp $CONTAINER_NAME:/out/. $OUTPUT_DIR/

echo "🧹 Cleaning up container..."
docker rm $CONTAINER_NAME


APP_BIN=$(find $OUTPUT_DIR -maxdepth 1 -type f -executable | head -n 1)

if [ "$EUID" -eq 0 ]; then
  echo "🔐 Applying root ownership and setuid on host binary..."
  chown root:root "$APP_BIN"
  chmod u+s "$APP_BIN"
else
  echo "⚠️  Run with sudo if you want to apply root ownership and setuid on host."
fi

echo "✅ Done. Final binary: $APP_BIN"
