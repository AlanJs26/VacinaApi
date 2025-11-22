#See https://aka.ms/customizecontainer to learn how to customize your#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Vacina.slnx", "."]
COPY ["VacinaApi/VacinaApi.csproj", "VacinaApi/"]
COPY ["VacinaApi.Tests/VacinaApi.Tests.csproj", "VacinaApi.Tests/"]
RUN dotnet restore "Vacina.slnx"
COPY . .
WORKDIR "/src/VacinaApi"
RUN dotnet build "VacinaApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS test
WORKDIR "/src/VacinaApi.Tests"
RUN dotnet test "VacinaApi.Tests.csproj"

FROM test AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/VacinaApi"
RUN dotnet publish "VacinaApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VacinaApi.dll"]
