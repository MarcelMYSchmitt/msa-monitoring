#FROM microsoft/dotnet:2.0-runtime AS base
#WORKDIR /app

#FROM microsoft/dotnet:2.0-sdk AS build
#WORKDIR /src
#COPY MicroserviceArchitecture.Monitoring.Watchdog/MicroserviceArchitecture.Monitoring.Watchdog.csproj MicroserviceArchitecture.Monitoring.Watchdog/
#RUN dotnet restore MicroserviceArchitecture.Monitoring.Watchdog/MicroserviceArchitecture.Monitoring.Watchdog.csproj
#COPY . .
#WORKDIR /src/MicroserviceArchitecture.Monitoring.Watchdog
#RUN dotnet build MicroserviceArchitecture.Monitoring.Watchdog.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish MicroserviceArchitecture.Monitoring.Watchdog.csproj -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Watchdog.dll"]

FROM microsoft/dotnet:2.0.3-runtime
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Watchdog.dll"]
