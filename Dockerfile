FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy backend project file and restore dependencies
COPY backend/RealTimeChat/*.csproj ./backend/RealTimeChat/
WORKDIR /app/backend/RealTimeChat
RUN dotnet restore

# Copy remaining backend files and publish
WORKDIR /app/
COPY backend/RealTimeChat/ ./
WORKDIR /app/backend/RealTimeChat
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/backend/RealTimeChat/out .
ENTRYPOINT ["dotnet", "RealTimeChat.dll"] 