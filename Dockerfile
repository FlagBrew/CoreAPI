FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
COPY . /build
WORKDIR /build

RUN dotnet build --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN mkdir /app
COPY --from=build /build/bin/Release/net6.0/* /app/
COPY --from=build /build/Moves.csv /app/
RUN mkdir /app/data
COPY --from=build /build/data/pokemon.json /app/data/pokemon.json

EXPOSE 5555
WORKDIR /app
ENV PATH=/app:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
CMD ["CoreAPI"]
