# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5068

# Set app to listen on port 5068
ENV ASPNETCORE_URLS=http://+:5068

# Switch to non-root user
USER app

# Build image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src

# Copy project file and restore
COPY ["ExpenseTracker.csproj", "./"]
RUN dotnet restore "ExpenseTracker.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "ExpenseTracker.csproj" -c $configuration -o /app/build

# Publish image
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "ExpenseTracker.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExpenseTracker.dll"]
