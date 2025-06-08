import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CatalogoType } from '../interfaces/catalogo.interface';

@Injectable({
  providedIn: 'root'
})
export class CatalogoService {
  private apiUrl = 'https://localhost:7270/v1/es/catalogo';
  constructor(private _http: HttpClient) { }

  obtenerCatalogos(): Observable<CatalogoType[]> {
    return this._http.get<CatalogoType[]>(this.apiUrl);
  }

  obtenerCatalogosPorTipo(tipo: string): Observable<CatalogoType[]> {
    return this._http.get<CatalogoType[]>(this.apiUrl + "/tipo/" + tipo);
  }

  obtenerCatalogosPorId(id: number): Observable<CatalogoType> {
    return this._http.get<CatalogoType>(this.apiUrl + "/" + id);
  }

  EliminarCatalogo(id: number): Observable<string> {
    return this._http.delete<string>(this.apiUrl + "/" + id);
  }

  AgregarCatalogo(catalogo: CatalogoType): Observable<CatalogoType> {
    return this._http.post<CatalogoType>(this.apiUrl, catalogo);
  }

  ActualizarCatalogo(catalogo: CatalogoType): Observable<CatalogoType> {
    return this._http.put<CatalogoType>(this.apiUrl, catalogo);
  }
}
