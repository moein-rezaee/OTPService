FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5254
ENV ASPNETCORE_URLS=http://+:5254
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all files
COPY ["OTPService.sln", "."]
COPY ["OTPService/OTPService.Api/OTPService.Api.csproj", "OTPService/OTPService.Api/"]
COPY ["OTPService/OTPService.Application/OTPService.Application.csproj", "OTPService/OTPService.Application/"]
COPY ["OTPService/OTPService.Domain/OTPService.Domain.csproj", "OTPService/OTPService.Domain/"]
COPY ["OTPService/OTPService.Infrastructure/OTPService.Infrastructure.csproj", "OTPService/OTPService.Infrastructure/"]
COPY ["Shared/CacheService/CacheService.csproj", "Shared/CacheService/"]
COPY ["Shared/SmsProvider/SmsProvider.csproj", "Shared/SmsProvider/"]

# Restore packages
RUN dotnet restore "./OTPService/OTPService.Api/OTPService.Api.csproj"

# Copy remaining source code
COPY . .

# Build and publish
WORKDIR /src/OTPService/OTPService.Api
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM base AS final
USER app
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]
