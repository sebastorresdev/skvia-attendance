FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and csproj files
COPY ["src/Skvia.Attendance.Api/Skvia.Attendance.Api.csproj", "src/Skvia.Attendance.Api/"]
COPY ["src/Skvia.Attendance.Application/Skvia.Attendance.Application.csproj", "src/Skvia.Attendance.Application/"]
COPY ["src/Skvia.Attendance.Domain/Skvia.Attendance.Domain.csproj", "src/Skvia.Attendance.Domain/"]
COPY ["src/Skvia.Attendance.Infrastructure/Skvia.Attendance.Infrastructure.csproj", "src/Skvia.Attendance.Infrastructure/"]
COPY ["aspire/Skvia.Attendance.ServiceDefaults/Skvia.Attendance.ServiceDefaults.csproj", "aspire/Skvia.Attendance.ServiceDefaults/"]

RUN dotnet restore "src/Skvia.Attendance.Api/Skvia.Attendance.Api.csproj"

# Copy source code and build
COPY . .
WORKDIR "/src/src/Skvia.Attendance.Api"
RUN dotnet build "Skvia.Attendance.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Skvia.Attendance.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Skvia.Attendance.Api.dll"]
