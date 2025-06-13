# Sistema de Gestión de Solicitudes

Aplicación full-stack para la gestión de solicitudes desarrollada con Angular 16, .NET 8 y SQL Server.

## 📋 Tabla de Contenidos

- [Requisitos del Sistema](#requisitos-del-sistema)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Configuración de la Base de Datos](#configuración-de-la-base-de-datos)
- [Instalación y Configuración](#instalación-y-configuración)
- [Ejecución de los Servicios](#ejecución-de-los-servicios)
- [Ejecución del Portal Web](#ejecución-del-portal-web)
- [Solución de Problemas](#solución-de-problemas)

## 🖥️ Requisitos del Sistema

### Para el Portal Web (Angular)
- **Node.js**: Versión 18.0.0 o superior
- **npm**: Versión 9.0.0 o superior (se instala con Node.js)
- **Navegador web**: Chrome, Firefox, Edge o Safari (versiones recientes)

### Para los Servicios (.NET)
- **.NET 8 SDK**: Descargar desde [Microsoft .NET](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (recomendado) o **Visual Studio Code**
- **SQL Server**: SQL Server 2019 o superior, o SQL Server Express

### Para la Base de Datos
- **SQL Server**: SQL Server 2019 o superior
- **SQL Server Management Studio (SSMS)** (opcional, para administración)

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
3. Durante la instalación, configura una contraseña para el usuario 'sa'

### Paso 2: Ejecutar Scripts de Base de Datos
1. Abre **SQL Server Management Studio (SSMS)** o **Azure Data Studio**
2. Conéctate a tu instancia de SQL Server
3. Ejecuta los scripts en el siguiente orden:

```sql
-- 1. Ejecutar el script principal de inicialización
-- Archivo: DB/init/01-init-db.sql

-- 2. Ejecutar el script de catálogos
-- Archivo: DB/init/02-init-catalogos.sql
```

**Nota**: Los scripts crearán automáticamente:
- Base de datos `SolicitudesDB`
- Tablas: Usuarios, Solicitudes, Catalogos, Log_auditoria, Usuario_Roles
- Datos iniciales de catálogos

## 🚀 Instalación y Configuración

### Paso 1: Clonar el Repositorio
```bash
git clone [URL_DEL_REPOSITORIO]
cd Solicitudes
git checkout dev
```

**⚠️ Importante**: El contenido del proyecto se encuentra en la rama `dev` del repositorio de GitHub. Asegúrate de cambiar a esta rama después de clonar.

### Paso 2: Configurar Variables de Entorno
Antes de ejecutar los servicios, necesitas configurar las cadenas de conexión:

1. **Para es-solicitudes**: Edita `Servicios/es-solicitudes/es-solicitudes/appsettings.json`
2. **Para es-catalogo**: Edita `Servicios/es-catalogo/es-catalogo/appsettings.json`
3. **Para es-usuario**: Edita `Servicios/es-usuario/es-usuario/appsettings.json`

Cambia la cadena de conexión por la de tu servidor:
```json
{
  "ConnectionStrings": {
    "SQLConnection": "Server=TU_SERVIDOR;Database=SolicitudesDB;User Id=TU_USUARIO;Password=TU_PASSWORD;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;Connection Timeout=30;Application Name=es-solicitudes"
  }
}
```

**Ejemplo para SQL Server Express local**:
```json
"SQLConnection": "Server=localhost\\SQLEXPRESS;Database=SolicitudesDB;User Id=sa;Password=TuPassword;TrustServerCertificate=True;Encrypt=False;MultipleActiveResultSets=true;Connection Timeout=30;Application Name=es-solicitudes"
```

## ⚙️ Ejecución de los Servicios

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

### Verificar que los Servicios Funcionen
1. Abre tu navegador
2. Visita las URLs de Swagger para cada servicio:
   - `https://localhost:7001/swagger`
   - `https://localhost:7002/swagger`
   - `https://localhost:7003/swagger`
3. Deberías ver la documentación de la API

## 🌐 Ejecución del Portal Web

### Paso 1: Instalar Dependencias
1. Abre una terminal en la carpeta del portal:
```bash
cd Portal_web/starterkit
```

2. Instala las dependencias de Node.js:
```bash
npm install
```

**Nota**: Este proceso puede tomar varios minutos la primera vez.

### Paso 2: Configurar URLs de los Servicios
1. Abre el archivo `src/environments/environment.ts`
2. Asegúrate de que las URLs de los servicios coincidan con las que están ejecutándose:
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001', // Servicio de solicitudes
  catalogoUrl: 'https://localhost:7002', // Servicio de catálogos
  usuarioUrl: 'https://localhost:7003' // Servicio de usuarios
};
```

### Paso 3: Ejecutar el Portal
```bash
npm start
```

El portal se abrirá automáticamente en `http://localhost:4200`

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
