FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic as build
COPY . /build
WORKDIR /build

RUN dotnet build --configuration Release

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
RUN mkdir /app
COPY --from=build /build/bin/Release/netcoreapp3.1/* /app/
COPY --from=build /build/Moves.csv /app/
RUN mkdir /app/data
COPY --from=build /build/data/pokemon.json /app/data/pokemon.json
RUN dotnet add package runtime.linux-x64.CoreCompat.System.Drawing

EXPOSE 5555
WORKDIR /app
ENV PATH=/app:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
CMD ["CoreAPI"]
