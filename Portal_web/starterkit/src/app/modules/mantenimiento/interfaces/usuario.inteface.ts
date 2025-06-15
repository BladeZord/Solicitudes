export interface RolType{
  rol_Id: number;
  usuario_Id: string;
}

export interface UsuarioType {
  id: number;
  nombre: string;
  apellidos: string;
  correo: string;
  domicilio?: string;
  telefono?: string;
  contrasenia: string;
  roles?: string[] | null;
}
