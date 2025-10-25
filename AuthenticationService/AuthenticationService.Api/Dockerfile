FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY OTPService.sln .
COPY shared/ shared/
COPY AuthenticationService/AuthenticationService.Api/AuthenticationService.Api.csproj AuthenticationService/AuthenticationService.Api/
COPY AuthenticationService/AuthenticationService.Application/AuthenticationService.Application.csproj AuthenticationService/AuthenticationService.Application/
COPY AuthenticationService/AuthenticationService.Domain/AuthenticationService.Domain.csproj AuthenticationService/AuthenticationService.Domain/
COPY AuthenticationService/AuthenticationService.Infrastructure/AuthenticationService.Infrastructure.csproj AuthenticationService/AuthenticationService.Infrastructure/

RUN dotnet restore AuthenticationService/AuthenticationService.Api/AuthenticationService.Api.csproj

COPY . .
RUN dotnet publish AuthenticationService/AuthenticationService.Api/AuthenticationService.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5270
ENV ASPNETCORE_URLS=http://+:5270
ENTRYPOINT ["dotnet", "AuthenticationService.Api.dll"]

