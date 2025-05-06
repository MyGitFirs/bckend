# Base image for final runtime (must come first if reused later)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Node.js (for npm commands)
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

COPY . .

# Run npm install with legacy-peer-deps (to resolve Angular issue)
WORKDIR /src/src/WebUI/ClientApp
RUN npm install --legacy-peer-deps

WORKDIR /src
RUN dotnet publish src/WebUI/WebUI.csproj -c Release -o /app/publish

# Final image using previously defined "base"
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitecture.WebUI.dll"]
