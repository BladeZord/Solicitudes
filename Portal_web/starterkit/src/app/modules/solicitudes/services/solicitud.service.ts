import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import { SolicitudType } from "../interfaces/solicitud.interface";
import { FiltrosSolicitudType } from "../interfaces/filtros-solicitud.interface";
import { CambiarEstadoSolicitudType } from "../interfaces/cambiar-estado-solicitud.interface";

@Injectable({
  providedIn: "root",
})
export class SolicitudService {
  private apiUrl = "https://localhost:7272/v1/es/solicitudes";

  constructor(private http: HttpClient) {}

  private getToken(): string {
    return localStorage.getItem("token") || "";
  }

  private getAuthHeaders(): HttpHeaders {
    return new HttpHeaders({
      "Content-Type": "application/json",
      Authorization: `Bearer ${this.getToken()}`,
    });
  }

  private handleError(error: any) {
    if (error.status === 401) {
      localStorage.removeItem("token");
      localStorage.removeItem("usuario");
      window.location.href = "/login";
    }
    return throwError(() => error);
  }

  crearSolicitud(solicitud: SolicitudType): Observable<SolicitudType> {
    return this.http
      .post<SolicitudType>(this.apiUrl, solicitud, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  actualizarSolicitud(solicitud: SolicitudType): Observable<SolicitudType> {
    return this.http
      .put<SolicitudType>(this.apiUrl, solicitud, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerSolicitudes(): Observable<SolicitudType[]> {
    return this.http
      .get<SolicitudType[]>(this.apiUrl, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerSolicitudPorId(id: number): Observable<SolicitudType> {
    return this.http
      .get<SolicitudType>(`${this.apiUrl}/${id}`, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  eliminarSolicitud(id: number): Observable<any> {
    return this.http
      .delete(`${this.apiUrl}/${id}`, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  cambiarEstadoSolicitud(cambioEstado: CambiarEstadoSolicitudType): Observable<any> {
    return this.http
      .post(`${this.apiUrl}/cambiar-estado`, cambioEstado, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerSolicitudesPorUsuario(usuarioId: number): Observable<SolicitudType[]> {
    return this.http
      .get<SolicitudType[]>(`${this.apiUrl}/usuario/${usuarioId}`, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerSolicitudesPorEstado(estadoId: number): Observable<SolicitudType[]> {
    return this.http
      .get<SolicitudType[]>(`${this.apiUrl}/estado/${estadoId}`, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerSolicitudesPorFiltros(filtros: FiltrosSolicitudType): Observable<SolicitudType[]> {
    return this.http
      .post<SolicitudType[]>(`${this.apiUrl}/filtros`, filtros, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }
} 