export interface HistorialAuditoriaType {
  logId: number;
  fecha_registro: string;
  accion: string;
  estadoAnterior: string;
  estadoActual: string;
  usuarioId: number;
  nombreUsuario: string;
  solicitudId: number;
  codigoSolicitud: string;
  monto: number;
  plazoMeses: number;
  fechaSolicitud: string;
}

export interface FiltrosHistorialAuditoriaType {
  solicitudId: number;
  usuarioId: number;
  estadoAnteriorId: number;
  estadoActualId: number;
  fechaInicio: string | null;
  fechaFin: string | null;
} 