# Base image for final runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Node.js 16 (safe for Webpack/OpenSSL)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_16.x | bash - && \
    apt-get install -y nodejs

COPY . .

# Build client app
WORKDIR /src/src/WebUI/ClientApp
RUN npm install --legacy-peer-deps
RUN NODE_OPTIONS=--openssl-legacy-provider npm run build -- --prod

# Publish .NET backend
WORKDIR /src
RUN dotnet publish src/WebUI/WebUI.csproj -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitecture.WebUI.dll"]
