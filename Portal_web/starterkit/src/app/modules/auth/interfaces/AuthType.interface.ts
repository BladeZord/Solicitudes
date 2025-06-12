export interface AuthResponseType {
  id:number;
  token: string;
  correo: string;
  nombre: string;
  rol: RolType[];
}

export interface RolType {
  id: number;
  descripcion: string;
}

export interface AuthRequestType {
  correo: string;
  contrasenia: string;
  perfil?: string;
}
