export const HOST: string = "http://localhost";
export const environment = {
  production: true,
  solicitudUrl: `${HOST}/solicitudes/v1/es/solicitudes`, // Servicio de solicitudes
  catalogoUrl: `${HOST}/catalogo/v1/es/catalogo`, // Servicio de catálogos
  usuarioUrl: `${HOST}/api/v1/es/usuario` // Servicio de usuarios
};
