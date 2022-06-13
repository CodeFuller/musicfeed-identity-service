FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

COPY ["src/MusicFeed.IdentityService/MusicFeed.IdentityService.csproj", "src/MusicFeed.IdentityService/"]
COPY ["src/MusicFeed.IdentityService.Abstractions/MusicFeed.IdentityService.Abstractions.csproj", "src/MusicFeed.IdentityService.Abstractions/"]
COPY ["src/MusicFeed.IdentityService.Infrastructure.PostgreSql/MusicFeed.IdentityService.Infrastructure.PostgreSql.csproj", "src/MusicFeed.IdentityService.Infrastructure.PostgreSql/"]
COPY ["tests/MusicFeed.IdentityService.IntegrationTests/MusicFeed.IdentityService.IntegrationTests.csproj", "tests/MusicFeed.IdentityService.IntegrationTests/"]
COPY ["MusicFeed.IdentityService.Build.sln", "./"]
COPY ["MusicFeed.IdentityService.src.ruleset", "./"]
COPY ["MusicFeed.IdentityService.tests.ruleset", "./"]

RUN dotnet restore "MusicFeed.IdentityService.Build.sln"

COPY . .

RUN dotnet restore "MusicFeed.IdentityService.Build.sln"

RUN dotnet build "MusicFeed.IdentityService.Build.sln" --no-restore --configuration Release

FROM build AS test

# Applying EF Core migrations necessary for IT.
RUN dotnet tool install -g dotnet-ef
ENV PATH $PATH:/root/.dotnet/tools
RUN dotnet ef database update --no-build --configuration Release --project "src/MusicFeed.IdentityService.Infrastructure.PostgreSql" --context CustomPersistedGrantDbContext -- "Server=localhost;Port=5432;Database=IdentityDB;User Id=postgres;Password=Qwerty123;"
RUN dotnet ef database update --no-build --configuration Release --project "src/MusicFeed.IdentityService.Infrastructure.PostgreSql" --context CustomIdentityDbContext -- "Server=localhost;Port=5432;Database=IdentityDB;User Id=postgres;Password=Qwerty123;"

# We don't let dotnet test command to fail build if some tests are failed.
# This will prevent publish of tests results (trx file).
# The build will be failed by the task that publishes test results.
# Instead the build is marked as unstable due to tests failures and artifacts publishing stage is skipped due to skipStagesAfterUnstable option.
RUN dotnet test "MusicFeed.IdentityService.Build.sln" --no-build --configuration Release --logger "trx" --collect:"XPlat Code Coverage"; exit 0

RUN mkdir TestResults
RUN cp -r --target-directory=TestResults --parents tests/*/TestResults

FROM test AS publish
RUN dotnet publish "src/MusicFeed.IdentityService/MusicFeed.IdentityService.csproj" --no-build --configuration Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicFeed.IdentityService.dll"]
