FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files to leverage restore caching
COPY OTPService.sln .
COPY shared/ shared/
COPY NotificationService/NotificationService.Api/NotificationService.Api.csproj NotificationService/NotificationService.Api/
COPY NotificationService/NotificationService.Application/NotificationService.Application.csproj NotificationService/NotificationService.Application/
COPY NotificationService/NotificationService.Domain/NotificationService.Domain.csproj NotificationService/NotificationService.Domain/
COPY NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj NotificationService/NotificationService.Infrastructure/

RUN dotnet restore NotificationService/NotificationService.Api/NotificationService.Api.csproj

# Copy the rest of the source
COPY . .
RUN dotnet publish NotificationService/NotificationService.Api/NotificationService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5260
ENV ASPNETCORE_URLS=http://+:5260
ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]
