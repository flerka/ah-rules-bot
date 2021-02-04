FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build

WORKDIR /src
COPY src .

WORKDIR /src/AhRulesBot
RUN  dotnet publish -v q -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "AhRulesBot.dll"]