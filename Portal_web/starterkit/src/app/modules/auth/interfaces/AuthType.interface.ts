export interface AuthResponseType {
  id:number;
  token: string;
  correo: string;
  nombre: string;
  roles: string[];
}

export interface AuthRequestType {
  correo: string;
  contrasenia: string;
  // perfil?: string;
}
