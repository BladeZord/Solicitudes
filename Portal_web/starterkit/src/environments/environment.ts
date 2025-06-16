// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const HOST: string ="http://localhost:";
export const environment = {
  production: false,
  // solicitudUrl: 'https://localhost:7272/v1/es/solicitudes', // Servicio de solicitudes
  // catalogoUrl: 'https://localhost:7270/v1/es/catalogo', // Servicio de catálogos
  // usuarioUrl: 'https://localhost:7271/v1/es/usuario' // Servicio de usuarios
  solicitudUrl: `${HOST}5002/v1/es/solicitudes`, // Servicio de solicitudes
  catalogoUrl: `${HOST}5001/v1/es/catalogo`, // Servicio de catálogos
  usuarioUrl: `${HOST}5003/v1/es/usuario` // Servicio de usuarios
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
