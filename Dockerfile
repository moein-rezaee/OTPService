FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5254
ENV ASPNETCORE_URLS=http://+:5254
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all files
COPY ["OTPService.sln", "."]
COPY ["src/OTPService.Api/OTPService.Api.csproj", "src/OTPService.Api/"]
COPY ["src/OTPService.Application/OTPService.Application.csproj", "src/OTPService.Application/"]
COPY ["src/OTPService.Domain/OTPService.Domain.csproj", "src/OTPService.Domain/"]
COPY ["src/OTPService.Infrastructure/OTPService.Infrastructure.csproj", "src/OTPService.Infrastructure/"]
COPY ["shared/CacheService/CacheService.csproj", "shared/CacheService/"]
COPY ["shared/SmsProvider/SmsProvider.csproj", "shared/SmsProvider/"]

# Restore packages
RUN dotnet restore "./src/OTPService.Api/OTPService.Api.csproj"

# Copy remaining source code
COPY . .

# Build and publish
WORKDIR /src/src/OTPService.Api
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM base AS final
USER app
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]
