#FROM microsoft/dotnet:2.0-runtime AS base
#WORKDIR /app

#FROM microsoft/dotnet:2.0-sdk AS build
#WORKDIR /src
#COPY MicroserviceArchitecture.Monitoring.MessageGenerator/MicroserviceArchitecture.Monitoring.MessageGenerator.csproj MicroserviceArchitecture.Monitoring.MessageGenerator/
#RUN dotnet restore MicroserviceArchitecture.Monitoring.MessageGenerator/MicroserviceArchitecture.Monitoring.MessageGenerator.csproj
#COPY . .
#WORKDIR /src/MicroserviceArchitecture.Monitoring.MessageGenerator
#RUN dotnet build MicroserviceArchitecture.Monitoring.MessageGenerator.csproj -c Release -o /app

#FROM build AS publish
#RUN dotnet publish MicroserviceArchitecture.Monitoring.MessageGenerator.csproj -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "MicroserviceArchitecture.Monitoring.MessageGenerator.dll"]

FROM microsoft/dotnet:2.0.3-runtime
ARG source
WORKDIR /app
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "MicroserviceArchitecture.MessageGenerator.dll"]