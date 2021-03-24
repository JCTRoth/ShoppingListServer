FROM mcr.microsoft.com/dotnet/sdk AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

EXPOSE 5678

FROM mcr.microsoft.com/dotnet/aspnet:3.1
COPY --from=build-env /app/out .
CMD ["dotnet", "ShoppingListServer.dll", "--server.urls", "http://0.0.0.0:5678"]