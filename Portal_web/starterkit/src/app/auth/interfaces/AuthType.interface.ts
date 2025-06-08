export interface AuthResponseType {
  token: string;
  correo: string;
  nombre: string;
  rol: string;
}

export interface AuthRequestType {
  correo: string;
  contrasenia: string;
}
