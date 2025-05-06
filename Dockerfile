# Base image for final runtime
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

# Copy all files into the container
COPY . .  # This copies everything from your repo into /src

# List files to check if they are copied correctly
RUN ls -R /src

# Navigate to the ClientApp directory
WORKDIR /src/src/WebUI/ClientApp

# Run npm install
RUN npm install --legacy-peer-deps --force

# Build the frontend
RUN npm run build -- --prod

# Return to src and publish the .NET app
WORKDIR /src
ENV NODE_OPTIONS=--openssl-legacy-provider
RUN dotnet publish src/WebUI/WebUI.csproj -c Release -o /app/publish

# Final image using previously defined "base"
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish . 
ENTRYPOINT ["dotnet", "CleanArchitecture.WebUI.dll"]
