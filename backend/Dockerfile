# Use the official .NET 9 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy the solution file and restore dependencies
COPY backend.sln ./
COPY EasyApply.Api/EasyApply.Api.csproj ./EasyApply.Api/
COPY EasyApply.BusinessLayer/EasyApply.BusinessLayer.csproj ./EasyApply.BusinessLayer/
COPY EasyApply.DataAccess/EasyApply.DataAccess.csproj ./EasyApply.DataAccess/
COPY EasyApply.Domain/EasyApply.Domain.csproj ./EasyApply.Domain/

RUN dotnet restore

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish EasyApply.Api/EasyApply.Api.csproj -c Release -o out

# Use the official .NET 9 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port the app runs on (Render/Fly.io/Docker default)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "EasyApply.Api.dll"]
