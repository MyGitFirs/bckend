# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

COPY . .

# OPTIONAL: Run npm install manually with flag (fixes peer deps)
WORKDIR /src/src/WebUI/ClientApp
RUN npm install --legacy-peer-deps

WORKDIR /src

# Now build the .NET project
RUN dotnet publish src/WebUI/WebUI.csproj -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanArchitecture.WebUI.dll"]
