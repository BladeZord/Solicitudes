#!/bin/bash
set -e

echo "Esperando a que SQL Server esté listo..."
sleep 45s

echo "Descomprimiendo el backup..."
unzip -o /DB/SolicitudesDB.zip -d /tmp

echo "Restaurando el backup..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -Q "RESTORE DATABASE SolicitudesDB FROM DISK = '/tmp/SolicitudesDB.bak' WITH MOVE 'SolicitudesDB' TO '/var/opt/mssql/data/SolicitudesDB.mdf', MOVE 'SolicitudesDB_log' TO '/var/opt/mssql/data/SolicitudesDB_log.ldf'"

echo "Verificando la restauración..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -d SolicitudesDB -Q "SELECT name FROM sys.tables"

echo "Base de datos inicializada correctamente" 