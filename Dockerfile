# Dotnet 8 project build image
FROM mcr.microsoft.com/dotnet/sdk:8.0

ENV U_GID=1000
ENV U_UID=1000
ENV U_NAME=u1000
ENV U_GROUP_NAME=u1000

USER root

WORKDIR /app

COPY cs-source/*.csproj .

# Restore project dependencies
RUN dotnet restore

EXPOSE 8080

RUN groupadd -g $U_GID $U_GROUP_NAME
RUN useradd $U_NAME -u $U_UID -g $U_GID -m -s /sbin/nologin

USER $U_NAME
