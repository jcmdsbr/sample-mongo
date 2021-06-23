FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build 
WORKDIR /src
COPY *.sln ./
COPY Sample.Api/Sample.Api.csproj/ Sample.Api/
RUN dotnet restore
COPY . ./

WORKDIR /src/Sample.Api
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
EXPOSE 80
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Sample.Api.dll"]
