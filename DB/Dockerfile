FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

# Instalar unzip
RUN apt-get update && apt-get install -y unzip

# Copiar el script de inicialización
COPY init-db.sh /docker-entrypoint-initdb.d/
RUN chmod +x /docker-entrypoint-initdb.d/init-db.sh

# Crear directorio temporal y establecer permisos
RUN mkdir -p /tmp && chmod 777 /tmp

USER mssql 