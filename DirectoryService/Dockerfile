FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/DirectoryService.Domain/*.csproj src/DirectoryService.Domain/
COPY src/DirectoryService.Application/*.csproj src/DirectoryService.Application/
COPY src/DirectoryService.Infrastructure/*.csproj src/DirectoryService.Infrastructure/
COPY src/DirectoryService.Presentation/*.csproj src/DirectoryService.Presentation/

RUN dotnet restore src/DirectoryService.Presentation/DirectoryService.Presentation.csproj

COPY . .

RUN dotnet publish src/DirectoryService.Presentation/DirectoryService.Presentation.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "DirectoryService.Presentation.dll"]