FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files for restore caching
COPY OTPService.sln .
COPY shared/ shared/
COPY OTPService/OTPService.Api/OTPService.Api.csproj OTPService/OTPService.Api/
COPY OTPService/OTPService.Application/OTPService.Application.csproj OTPService/OTPService.Application/
COPY OTPService/OTPService.Domain/OTPService.Domain.csproj OTPService/OTPService.Domain/
COPY OTPService/OTPService.Infrastructure/OTPService.Infrastructure.csproj OTPService/OTPService.Infrastructure/

RUN dotnet restore OTPService/OTPService.Api/OTPService.Api.csproj

# Copy remaining sources and publish
COPY . .
RUN dotnet publish OTPService/OTPService.Api/OTPService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5254
ENV ASPNETCORE_URLS=http://+:5254
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]

