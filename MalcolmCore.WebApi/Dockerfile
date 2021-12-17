#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MalcolmCore.WebApi/MalcolmCore.WebApi.csproj", "MalcolmCore.WebApi/"]
COPY ["MalcolmCore.Data/MalcolmCore.Data.csproj", "MalcolmCore.Data/"]
COPY ["MalcolmCore.IService/MalcolmCore.IService.csproj", "MalcolmCore.IService/"]
COPY ["MalcolmCore.Utils/MalcolmCore.Utils.csproj", "MalcolmCore.Utils/"]
COPY ["MalcolmCore.DTO/MalcolmCore.DTO.csproj", "MalcolmCore.DTO/"]
COPY ["MalcolmCore.Service/MalcolmCore.Service.csproj", "MalcolmCore.Service/"]
RUN dotnet restore "MalcolmCore.WebApi/MalcolmCore.WebApi.csproj"
COPY . .
WORKDIR "/src/MalcolmCore.WebApi"
RUN dotnet build "MalcolmCore.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MalcolmCore.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MalcolmCore.WebApi.dll"]