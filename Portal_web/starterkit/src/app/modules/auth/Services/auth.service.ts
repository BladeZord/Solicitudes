import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthRequestType, AuthResponseType } from '../interfaces/AuthType.interface';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7271/v1/es/usuario/login'

  constructor(private _http: HttpClient, private _router:Router) { }

  login(request: AuthRequestType): Observable<AuthResponseType> {
    return this._http.post<AuthResponseType>(this.apiUrl, request);
  }

  logout(){
    const usuario = localStorage.getItem('usuario')
    if (!usuario){
      
    }
    localStorage.removeItem('usuario');
    this._router.navigate(['/auth/login'])
  }
}
