import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { CatalogoType } from '../interfaces/catalogo.interface';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CatalogoService {
  // private apiUrl = 'https://localhost:7270/v1/es/catalogo';
  private apiUrl = `${environment.catalogoUrl}`  

  constructor(private _http: HttpClient) { }

  private getToken(): string | null {
    const userData = localStorage.getItem("usuario");
    if (!userData) return null;
    try {
      const user = JSON.parse(userData);
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

  obtenerCatalogos(): Observable<CatalogoType[]> {
    return this._http.get<CatalogoType[]>(this.apiUrl, { headers: this.getAuthHeaders() })
      .pipe(catchError(this.handleError));
  }

  obtenerCatalogosPorTipo(tipo?: string): Observable<CatalogoType[]> {
    const params = tipo ? { Tipo: tipo } : {};
    return this._http.get<CatalogoType[]>(`${this.apiUrl}/tipo`, { 
      params,
      headers: this.getAuthHeaders() 
    }).pipe(catchError(this.handleError));
  }
  
  obtenerCatalogosPorId(id: number): Observable<CatalogoType> {
    return this._http.get<CatalogoType>(`${this.apiUrl}/${id}`, { 
      headers: this.getAuthHeaders() 
    }).pipe(catchError(this.handleError));
  }

  EliminarCatalogo(id: number): Observable<string> {
    return this._http.delete<string>(`${this.apiUrl}/${id}`, { 
      headers: this.getAuthHeaders() 
    }).pipe(catchError(this.handleError));
  }

  AgregarCatalogo(catalogo: CatalogoType): Observable<CatalogoType> {
    return this._http.post<CatalogoType>(this.apiUrl, catalogo, { 
      headers: this.getAuthHeaders() 
    }).pipe(catchError(this.handleError));
  }

  ActualizarCatalogo(catalogo: CatalogoType): Observable<CatalogoType> {
    return this._http.put<CatalogoType>(this.apiUrl, catalogo, { 
      headers: this.getAuthHeaders() 
    }).pipe(catchError(this.handleError));
  }
}
