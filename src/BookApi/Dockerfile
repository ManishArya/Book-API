# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src/BookApi
COPY *.csproj .
# Restore as distinct layers
RUN dotnet restore

COPY . .

# Build and publish a release
RUN dotnet publish -c Release -o out /publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as runtime
WORKDIR /publish
COPY --from=build-env /publish .
ENTRYPOINT ["dotnet", "BookApi.dll"]