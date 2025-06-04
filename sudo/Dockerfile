FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt-get update && apt-get install -y clang build-essential && rm -rf /var/lib/apt/lists/*

WORKDIR /src

COPY *.csproj ./

RUN dotnet restore

COPY . .

RUN PROJECT_FILE=$(find . -maxdepth 1 -name '*.csproj' | head -n 1) && \
    dotnet publish "$PROJECT_FILE" -c Release -r linux-x64 \
      /p:PublishAot=true \
      /p:PublishSingleFile=true \
      /p:IncludeNativeLibrariesForSelfExtract=false \
      /p:SelfContained=true \
      -o /publish

FROM debian:bookworm-slim AS finalizer

WORKDIR /out
COPY --from=build /publish/ ./

RUN chmod +x ./* && \
    BIN=$(find . -maxdepth 1 -type f -executable | head -n 1) && \
    chown root:root "$BIN" && chmod u+s "$BIN"
