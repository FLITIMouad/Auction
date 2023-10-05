FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
EXPOSE 80

# COPY all .csproj files and restor as distinct layers. use of the same copy command 
# for every dockerfile in the project to take advantage of docker caching
COPY carsties.sln carsties.sln
Copy src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
Copy src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
Copy src/GetwayService/GetwayService.csproj src/GetwayService/GetwayService.csproj
Copy src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
Copy src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj

#Restore package deps

RUN dotnet restore carsties.sln

#copy the app folders over
Copy src/GetwayService src/GetwayService 
workdir /app/src/GetwayService
RUN dotnet publish -c Release -o /app/src/out

# BUild runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
Copy --from=build /app/src/out .
ENTRYPOINT [ "dotnet" ,"GetwayService.dll" ]