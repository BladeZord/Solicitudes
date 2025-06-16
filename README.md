# Sistema de Gestión de Solicitudes

Aplicación full-stack para la gestión de solicitudes desarrollada con Angular 16, .NET 8 y SQL Server.

## 📋 Tabla de Contenidos

- [Requisitos del Sistema](#-requisitos-del-sistema)
- [Tecnologías Utilizadas](#-tecnologías-utilizadas)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Configuración de la Base de Datos](#-configuración-de-la-base-de-datos)
- [Instalación y Configuración](#-instalación-y-configuración)
- [Ejecución con Docker](#-ejecución-con-docker)
- [Ejecución Manual](#-ejecución-manual)
- [Solución de Problemas](#-solución-de-problemas)

## 🖥️ Requisitos del Sistema

### Para el Portal Web (Angular)
- **Node.js**: Versión 18.0.0 o superior
- **npm**: Versión 9.0.0 o superior (se instala con Node.js)
- **Navegador web**: Chrome, Firefox, Edge o Safari (versiones recientes)

### Para los Servicios (.NET)
- **.NET 8 SDK**: Descargar desde [Microsoft .NET](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (recomendado) o **Visual Studio Code**
- **SQL Server**: SQL Server 2019 o superior, o SQL Server Express

### Para Docker
- **Docker Desktop**: Versión 4.0.0 o superior
- **Docker Compose**: Versión 2.0.0 o superior

### Para la Base de Datos
- **SQL Server**: SQL Server 2019 o superior
- **SQL Server Management Studio (SSMS)** (opcional, para administración)
- **DBearver** (opcional, para administración)

## 🛠️ Tecnologías Utilizadas

### Frontend (Portal Web)
- **Angular 16**: Framework de desarrollo web
- **Bootstrap 5**: Framework CSS para el diseño
- **TypeScript**: Lenguaje de programación
- **RxJS**: Biblioteca para programación reactiva
- **NgBootstrap**: Componentes de Bootstrap para Angular

### Backend (Servicios)
- **.NET 8**: Framework de desarrollo
- **ASP.NET Core**: Framework web
- **Dapper**: Micro ORM para acceso a datos
- **Swagger/OpenAPI**: Documentación de APIs
- **Log4Net**: Sistema de logging

### Base de Datos
- **SQL Server**: Motor de base de datos
- **T-SQL**: Lenguaje de consultas

### Infraestructura
- **Docker**: Contenedorización
- **Docker Compose**: Orquestación de contenedores
- **Nginx**: Servidor web y proxy inverso

## 📁 Estructura del Proyecto

```
Solicitudes/
├── Portal_web/                 # Aplicación Angular
│   └── starterkit/            # Código fuente del portal
├── Servicios/                 # Microservicios .NET
│   ├── es-solicitudes/        # Servicio de solicitudes
│   ├── es-catalogo/           # Servicio de catálogos
│   └── es-usuario/            # Servicio de usuarios
├── DB/                        # Scripts de base de datos
│   └── init/                  # Scripts de inicialización
└── Documentos/                # Documentación adicional
```

## 🗄️ Configuración de la Base de Datos

### Paso 1: Instalar SQL Server
1. Descarga SQL Server desde [Microsoft](https://www.microsoft.com/es-es/sql-server/sql-server-downloads)
2. Instala SQL Server Express (gratuito) o Developer Edition
3. Durante la instalación, configura una contraseña para el usuario 'sa' o el que detallas el Script

### Paso 2: Ejecutar Scripts de Base de Datos
1. Abre **SQL Server Management Studio (SSMS)** o **Azure Data Studio**
2. Conéctate a tu instancia de SQL Server
3. Ejecuta los scripts en el siguiente orden:

```sql
-- 1. Ejecutar el script principal de inicialización
-- Archivo: DB/init/01-init-db.sql

-- 2. Ejecutar el script de catálogos
-- Archivo: DB/init/02-init-catalogos.sql

-- 3. Ejecutar el script de usuario inicial este se usara para el acceso al portal como ANALISTA
-- Archivo: DB/init/03-init-usuario.sql
```

**Nota**: Los scripts crearán automáticamente:
- Base de datos `SolicitudesDB`
- Tablas: Usuarios, Solicitudes, Catalogos, Log_auditoria, Usuario_Roles
- Datos iniciales de catálogos y usuario administrador

## 🚀 Instalación y Configuración

### Paso 1: Clonar el Repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd Solicitudes
git checkout dev
```

**⚠️ Importante**: El contenido del proyecto se encuentra en la rama `dev` del repositorio de GitHub. Asegúrate de cambiar a esta rama después de clonar, ya que esta es la mas actualizada. 

### Paso 2: Configurar Variables de Entorno
Antes de ejecutar los servicios, necesitas configurar las cadenas de conexión:

1. **Para es-solicitudes**: Edita `Servicios/es-solicitudes/es-solicitudes/appsettings.json`
2. **Para es-catalogo**: Edita `Servicios/es-catalogo/es-catalogo/appsettings.json`
3. **Para es-usuario**: Edita `Servicios/es-usuario/es-usuario/appsettings.json`

Cambia la cadena de conexión por la de tu servidor:
```json
{
  "ConnectionStrings": {
    "SQLConnection": "Server=host.docker.internal,1433;Database=SolicitudesDB;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;Connection Timeout=30;Application Name=es-solicitudes"
  }
}
```

## 🐳 Ejecución con Docker

### Paso 1: Verificar Docker
Asegúrate de que Docker Desktop esté instalado y en ejecución:
```bash
docker --version
docker-compose --version
```

### Paso 2: Construir y Ejecutar los Contenedores
1. Desde la raíz del proyecto, ejecuta:
```bash
docker-compose up --build
```

2. Los servicios estarán disponibles en:
   - Portal Web: `http://localhost`
   - Servicio de Usuarios: `http://localhost:8080`
   - Servicio de Catálogos: `http://localhost:8081`
   - Servicio de Solicitudes: `http://localhost:8082`

### Paso 3: Verificar los Servicios
1. Abre tu navegador y visita:
   - Portal Web: `http://localhost`
   - Swagger de Usuarios: `http://localhost:8080/swagger`
   - Swagger de Catálogos: `http://localhost:8081/swagger`
   - Swagger de Solicitudes: `http://localhost:8082/swagger`

### Paso 4: Detener los Contenedores
Para detener los contenedores:
```bash
docker-compose down
```

## ⚙️ Ejecución Manual

### Opción 1: Desde Visual Studio (Recomendado para principiantes)
1. Abre Visual Studio 2022
2. Abre cada solución:
   - `Servicios/es-solicitudes/es-solicitudes.sln`
   - `Servicios/es-catalogo/es-catalogo.sln`
   - `Servicios/es-usuario/es-usuario.sln`
3. Presiona **F5** o haz clic en **Iniciar** para cada proyecto
4. Los servicios se ejecutarán en:
   - es-solicitudes: `https://localhost:7001`
   - es-catalogo: `https://localhost:7002`
   - es-usuario: `https://localhost:7003`

### Opción 2: Desde Línea de Comandos
1. Abre una terminal en cada carpeta de servicio:
```bash
# Terminal 1 - Servicio de Solicitudes
cd Servicios/es-solicitudes/es-solicitudes
dotnet run

# Terminal 2 - Servicio de Catálogos
cd Servicios/es-catalogo/es-catalogo
dotnet run

# Terminal 3 - Servicio de Usuarios
cd Servicios/es-usuario/es-usuario
dotnet run
```

### Ejecución del Portal Web
1. Abre una terminal en la carpeta del portal:
```bash
cd Portal_web/starterkit
```

2. Instala las dependencias:
```bash
npm install
```

3. Ejecuta el portal:
```bash
npm start
```

El portal se abrirá en `http://localhost:4200`

## 🔧 Solución de Problemas

### Error: "No se puede conectar a la base de datos"
- Verifica que SQL Server esté ejecutándose
- Confirma que la cadena de conexión sea correcta
- Asegúrate de que el usuario tenga permisos en la base de datos

### Error: "Puerto ya está en uso"
- Cambia el puerto en `launchSettings.json` de cada servicio
- O termina el proceso que está usando el puerto

### Error: "npm install falló"
- Verifica que tengas Node.js instalado correctamente
- Ejecuta `npm cache clean --force`
- Intenta con `npm install --legacy-peer-deps`

### Error: "Certificado SSL no válido"
- En desarrollo, puedes usar `http://` en lugar de `https://`
- O configura certificados de desarrollo con `dotnet dev-certs`

### Los servicios no se comunican entre sí
- Verifica que todos los servicios estén ejecutándose
- Confirma que las URLs en el portal coincidan con los puertos de los servicios
- Revisa la consola del navegador para errores de CORS

### Problemas con Docker
- Verifica que Docker Desktop esté en ejecución
- Asegúrate de que los puertos no estén en uso
- Revisa los logs de los contenedores con `docker-compose logs`
- Limpia los contenedores y volúmenes con `docker-compose down -v`

## 📞 Soporte

Si encuentras problemas:
1. Revisa los logs en la consola de cada servicio
2. Verifica que todos los requisitos estén instalados
3. Confirma que las configuraciones sean correctas
4. Consulta la documentación de cada tecnología

## 🎯 Próximos Pasos

Una vez que tengas todo funcionando:
1. Explora la documentación de Swagger de cada servicio
2. Prueba las funcionalidades del portal web
3. Revisa los logs para entender el flujo de datos
4. Familiarízate con la estructura de la base de datos
