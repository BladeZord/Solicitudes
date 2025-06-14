import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AuthResponseType } from "../../auth/interfaces/AuthType.interface";
import { catchError, Observable, throwError } from "rxjs";
import { RolType, UsuarioType } from "../interfaces/usuario.inteface";
import { environment } from "src/environments/environment";

@Injectable({
  providedIn: "root",
})
export class UsuarioService {
  // private apiUrl = "https://localhost:7271/v1/es/usuario";
  private apiUrl = `${environment.usuarioUrl}`  

  constructor(private _http: HttpClient) {}

  private getToken(): string | null {
    const userData = localStorage.getItem("usuario");
    if (!userData) return null;
    try {
      const user = JSON.parse(userData) as AuthResponseType;
      return user.token;
    } catch {
      return null;
    }
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.getToken();
    if (!token) throw new Error("No token found");
    return new HttpHeaders({
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    });
  }

  private handleError(error: any) {
    if (error.status === 401) {
      localStorage.removeItem("usuario");
      window.location.href = "/login";
    }
    console.error("API error", error);
    return throwError(() => error);
  }

  crearUsuario(usuario: UsuarioType): Observable<UsuarioType> {
    // No requiere token para registro público
    return this._http
      .post<UsuarioType>(this.apiUrl, usuario)
      .pipe(catchError(this.handleError));
  }

  actualizarUsuario(usuario: UsuarioType): Observable<UsuarioType> {
    return this._http
      .put<UsuarioType>(this.apiUrl, usuario, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerUsuarios(): Observable<UsuarioType[]> {
    return this._http
      .get<UsuarioType[]>(this.apiUrl, { headers: this.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  obtenerUsuarioPorId(id: number): Observable<UsuarioType> {
    return this._http
      .get<UsuarioType>(`${this.apiUrl}/${id}`, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  eliminarUsuario(id: number): Observable<void> {
    return this._http
      .delete<void>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  cambiarContrasenia(id: number, nuevaContrasenia: string): Observable<string> {
    return this._http
      .put<string>(`${this.apiUrl}/contrasenia`, { id, nuevaContrasenia }, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  obtenerRolesPorUsuario(usuarioId: number): Observable<RolType[]> {
    return this._http
      .get<RolType[]>(`${this.apiUrl}/roles/${usuarioId}`, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  asignarRol(usuario_Id: number, rol_Id: number): Observable<any> {
    return this._http
      .post(`${this.apiUrl}/roles`, { usuario_Id, rol_Id }, {
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }

  desasignarRol(usuario_Id: number, rol_Id: number): Observable<any> {
    return this._http
      .delete(`${this.apiUrl}/roles`, {
        body: { usuario_Id, rol_Id },
        headers: this.getAuthHeaders(),
      })
      .pipe(catchError(this.handleError));
  }
}
