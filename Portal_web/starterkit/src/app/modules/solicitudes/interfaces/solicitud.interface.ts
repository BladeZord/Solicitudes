export interface SolicitudType {
  id: number;
  codigo?: string;
  monto: number;
  plazoMeses: number;
  ingresosMensual: number;
  antiguedadLaboral: number;
  estado_Id: number;
  estado_Descripcion?: string;
  fechaRegistro: Date;
  usuario_Id: number;
  nombre_Usuario?: string;
}
