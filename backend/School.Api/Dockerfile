# Multi-stage build for Fidel Hub Core API (.NET 10 LTS)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project definitions for optimized caching
COPY FidelHub.sln ./
COPY backend/School.Api/School.Api.csproj backend/School.Api/
COPY backend/School.Shared/School.Shared.csproj backend/School.Shared/
COPY backend/School.Worker/School.Worker.csproj backend/School.Worker/
COPY backend/Modules/Academics/Academics.csproj backend/Modules/Academics/
COPY backend/Modules/Admissions/Admissions.csproj backend/Modules/Admissions/
COPY backend/Modules/Attendance/Attendance.csproj backend/Modules/Attendance/
COPY backend/Modules/Communication/Communication.csproj backend/Modules/Communication/
COPY backend/Modules/Examinations/Examinations.csproj backend/Modules/Examinations/
COPY backend/Modules/Finance/Finance.csproj backend/Modules/Finance/
COPY backend/Modules/Identity/Identity.csproj backend/Modules/Identity/
COPY backend/Modules/Reporting/Reporting.csproj backend/Modules/Reporting/
COPY backend/Modules/Students/Students.csproj backend/Modules/Students/
COPY backend/Modules/Tenants/Tenants.csproj backend/Modules/Tenants/
COPY backend/tests/School.ArchitectureTests/School.ArchitectureTests.csproj backend/tests/School.ArchitectureTests/
COPY backend/tests/School.UnitTests/School.UnitTests.csproj backend/tests/School.UnitTests/

RUN dotnet restore FidelHub.sln

# Copy source and publish
COPY backend/ backend/
WORKDIR /src/backend/School.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime Stage (.NET 10 ASP.NET Core LTS)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish ./
USER app
ENTRYPOINT ["dotnet", "School.Api.dll"]
