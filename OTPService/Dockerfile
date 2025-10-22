FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5254
ENV ASPNETCORE_URLS=http://+:5254

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

WORKDIR /src/OTPService/src/OTPService.Api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
USER app
WORKDIR /app
COPY --from=build /app/publish .
COPY OTPService/data/.env .
COPY OTPService/data/appsettings*.json .
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]

FROM base AS final
USER app
WORKDIR /app
COPY --from=build /app/publish .
COPY OTPService/data/.env .
COPY OTPService/data/appsettings*.json .
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]

FROM base AS final
USER app
WORKDIR /app
COPY --from=build /app/publish .
COPY OTPService/data/.env .
COPY OTPService/data/appsettings*.json .
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]
WORKDIR "/src/src/OTPService.Api"
RUN dotnet build "OTPService.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "OTPService.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OTPService.Api.dll"]
