import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthRequestType, AuthResponseType, AuthType } from '../interfaces/AuthType.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7271/v1/es/usuario/login'
  constructor(private _http: HttpClient) { }

  login(request: AuthRequestType): Observable<AuthResponseType> {
    return this._http.post<AuthResponseType>(this.apiUrl, request);
  }
}
