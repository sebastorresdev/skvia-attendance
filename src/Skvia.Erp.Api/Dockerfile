FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and csproj files
COPY ["src/Skvia.Erp.Api/Skvia.Erp.Api.csproj", "src/Skvia.Erp.Api/"]
COPY ["src/Skvia.Erp.Application/Skvia.Erp.Application.csproj", "src/Skvia.Erp.Application/"]
COPY ["src/Skvia.Erp.Domain/Skvia.Erp.Domain.csproj", "src/Skvia.Erp.Domain/"]
COPY ["src/Skvia.Erp.Infrastructure/Skvia.Erp.Infrastructure.csproj", "src/Skvia.Erp.Infrastructure/"]
COPY ["aspire/Skvia.Erp.ServiceDefaults/Skvia.Erp.ServiceDefaults.csproj", "aspire/Skvia.Erp.ServiceDefaults/"]

RUN dotnet restore "src/Skvia.Erp.Api/Skvia.Erp.Api.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/Skvia.Erp.Api"
RUN dotnet build "Skvia.Erp.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skvia.Erp.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skvia.Erp.Api.dll"]

