FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ARG CONFIGURATION=Release

COPY ["src/WebVella.Tefter.Site/WebVella.Tefter.Site.csproj", "src/WebVella.Tefter.Site/"]
RUN dotnet restore "src/WebVella.Tefter.Site/WebVella.Tefter.Site.csproj"

COPY . .
RUN dotnet publish "src/WebVella.Tefter.Site/WebVella.Tefter.Site.csproj" \
	-c $CONFIGURATION \
	-o /app/publish \
	--no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .

# create blob storage folder, add non-root user and set ownership (run as root)
RUN useradd --create-home appuser \
	&& mkdir -p /app/blob-storage \
	&& chown -R appuser:appuser /app /app/blob-storage

# run as non-root
USER appuser


ENV Tefter__ConnectionString="" 
ENV Tefter__CompanyName=""
ENV Tefter__CompanyLogoUrl=""
ENV Tefter__BaseUrl=""

ENTRYPOINT ["dotnet", "WebVella.Tefter.Site.dll"]