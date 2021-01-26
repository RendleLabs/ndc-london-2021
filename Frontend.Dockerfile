FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /code

# copy csproj and restore as distinct layers
COPY ./Pizza.sln .
COPY ./src/Frontend/Frontend.csproj ./src/Frontend/
COPY ./src/Ingredients/Ingredients.csproj ./src/Ingredients/
COPY ./src/Orders/Orders.csproj ./src/Orders/
COPY ./src/Orders.PubSub/Orders.PubSub.csproj ./src/Orders.PubSub/
COPY ./src/Pizza.Data/Pizza.Data.csproj ./src/Pizza.Data/
COPY ./src/ShopConsole/ShopConsole.csproj ./src/ShopConsole/
COPY ./test/TestHelpers/TestHelpers.csproj ./test/TestHelpers/
COPY ./test/Ingredients.Tests/Ingredients.Tests.csproj ./test/Ingredients.Tests/
COPY ./tools/CreateData/CreateData.csproj ./tools/CreateData/

RUN dotnet restore

COPY . .

RUN dotnet build -c Release --no-restore
RUN dotnet publish src/Frontend -c Release -o /app --no-build

# Create deployment image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["./Frontend"]
