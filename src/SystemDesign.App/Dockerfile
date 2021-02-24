FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
EXPOSE 5000 5001
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY SystemDesign.App.fsproj SystemDesign.App/
RUN dotnet restore SystemDesign.App/SystemDesign.App.fsproj
COPY . SystemDesign.App/
ARG SYSTEMDESIGN_CONN_STR
RUN dotnet build SystemDesign.App/SystemDesign.App.fsproj -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT dotnet /app/SystemDesign.App.dll