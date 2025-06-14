# Sistema de Control de Roles

## Descripción
Este sistema implementa un control de acceso basado en roles (RBAC) para la aplicación de solicitudes. Permite definir qué rutas y funcionalidades puede acceder cada tipo de usuario.

## Roles Definidos

### 1. SOLICITANTE
- **Descripción**: Usuario que puede crear y gestionar sus propias solicitudes
- **Rutas permitidas**:
  - `/solicitud` - Gestión de solicitudes propias
  - `/starter` - Página principal

- **Funcionalidades**:
  - Crear nuevas solicitudes
  - Editar sus propias solicitudes
  - Eliminar sus propias solicitudes
  - Ver sus propias solicitudes
  - Imprimir detalles de sus solicitudes

### 2. ANALISTA
- **Descripción**: Usuario con permisos administrativos que puede gestionar todas las solicitudes
- **Rutas permitidas**:
  - `/solicitud` - Gestión de todas las solicitudes
  - `/consulta` - Consultas de solicitudes
  - `/auditoria` - Auditoría de solicitudes
  - `/starter` - Página principal

- **Funcionalidades**:
  - Crear nuevas solicitudes
  - Editar cualquier solicitud
  - Eliminar cualquier solicitud
  - Ver todas las solicitudes
  - Exportar datos a Excel
  - Imprimir detalles de cualquier solicitud
  - Acceso a consultas y auditoría

## Implementación Técnica

### 1. Servicio de Autenticación (`AuthService`)
Ubicación: `src/app/modules/auth/Services/auth.service.ts`

**Métodos principales**:
- `hasRole(role: string)`: Verifica si el usuario tiene un rol específico
- `hasAnyRole(roles: string[])`: Verifica si el usuario tiene al menos uno de los roles especificados
- `canAccessRoute(route: string)`: Verifica si el usuario puede acceder a una ruta específica
- `getPermittedRoutes()`: Obtiene todas las rutas permitidas para el usuario actual
- `redirectToHome()`: Redirige al usuario a su página principal según su rol

### 2. Guard de Autenticación (`AuthGuard`)
Ubicación: `src/app/modules/auth/guards/auth.guard.ts`

**Funcionalidad**:
- Protege las rutas verificando autenticación y permisos
- Redirige a login si no está autenticado
- Redirige a página principal si no tiene permisos para la ruta

### 3. Directiva de Roles (`RoleDirective`)
Ubicación: `src/app/shared/directives/role.directive.ts`

**Uso en templates**:
```html
<div *appRole="'ANALISTA'">
  <!-- Contenido solo visible para analistas -->
</div>

<div *appRole="['SOLICITANTE', 'ANALISTA']">
  <!-- Contenido visible para ambos roles -->
</div>
```

### 4. Control de Roles en Componentes
Ejemplo en `SolicitudComponent`:

**Métodos de verificación**:
- `puedeEditarSolicitud(solicitud)`: Verifica si puede editar una solicitud específica
- `puedeEliminarSolicitud(solicitud)`: Verifica si puede eliminar una solicitud específica
- `puedeCrearSolicitud()`: Verifica si puede crear nuevas solicitudes
- `puedeExportar()`: Verifica si puede exportar datos

**Uso en template**:
```html
<button *ngIf="puedeCrearSolicitud()" class="btn btn-primary">
  Nuevo
</button>

<button *ngIf="puedeEditarSolicitud(solicitud)" class="btn btn-primary">
  Editar
</button>
```

## Configuración de Rutas

### Rutas por Rol
```typescript
private rutasPermitidas = {
  SOLICITANTE: [
    '/solicitud',
    '/starter'
  ],
  ANALISTA: [
    '/solicitud',
    '/consulta',
    '/auditoria',
    '/starter'
  ]
};
```

### Aplicación del Guard
En `app-routing.module.ts`:
```typescript
{
  path: "",
  component: FullComponent,
  canActivate: [AuthGuard], // Protege todas las rutas hijas
  children: [
    // Rutas protegidas
  ]
}
```

## Flujo de Autenticación

1. **Login**: Usuario ingresa credenciales
2. **Verificación**: Se valida contra el backend
3. **Almacenamiento**: Se guarda información del usuario en localStorage
4. **Redirección**: Se redirige según el rol:
   - ANALISTA → `/consulta`
   - SOLICITANTE → `/solicitud`
5. **Protección**: El guard verifica permisos en cada navegación

## Extensibilidad

Para agregar nuevos roles:

1. **Definir el rol** en `AuthResponseType.roles`
2. **Agregar rutas permitidas** en `AuthService.rutasPermitidas`
3. **Crear métodos de verificación** en los componentes necesarios
4. **Aplicar directivas** en los templates correspondientes

## Seguridad

- **Frontend**: Control de UI y navegación
- **Backend**: Validación real de permisos (debe implementarse)
- **Tokens**: Manejo de autenticación con JWT
- **Logout**: Limpieza de datos de sesión

## Notas Importantes

- El control de roles en el frontend es solo para UX
- La seguridad real debe implementarse en el backend
- Los permisos se verifican en cada navegación
- El sistema es extensible para nuevos roles y permisos 