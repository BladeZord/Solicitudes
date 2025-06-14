import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthRequestType, AuthResponseType } from '../interfaces/AuthType.interface';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // private apiUrl = 'https://localhost:7271/v1/es/usuario/login'
  private apiUrl = `${environment.usuarioUrl}/login`  
  

  // Definición de rutas permitidas por rol
  private rutasPermitidas = {
    SOLICITANTE: [
      '/solicitudes/solicitud',
      '/starter'
    ],
    ANALISTA: [
      '/solicitudes/solicitud',
      '/solicitudes/consulta',
      '/solicitudes/auditoria',
      '/starter',
      '/mantenimiento/usuario',
      '/mantenimiento/catalogo',
    ]
  };

  constructor(private _http: HttpClient, private _router: Router) { }

  login(request: AuthRequestType): Observable<AuthResponseType> {
    return this._http.post<AuthResponseType>(this.apiUrl, request);
  }

  logout() {
    const usuario = localStorage.getItem('usuario');
    if (!usuario || usuario.trim() === '') {
      console.warn('No hay usuario para cerrar sesión')
      return;
    }
    localStorage.removeItem('usuario');
    this._router.navigate(['/auth/login'])
  }

  /**
   * Obtiene el usuario actual desde localStorage
   */
  getCurrentUser(): AuthResponseType | null {
    const userData = localStorage.getItem('usuario');
    if (!userData) return null;
    
    try {
      return JSON.parse(userData) as AuthResponseType;
    } catch {
      return null;
    }
  }

  /**
   * Verifica si el usuario tiene un rol específico
   */
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    if (!user || !user.roles) return false;
    
    return user.roles.includes(role);
  }

  /**
   * Verifica si el usuario tiene al menos uno de los roles especificados
   */
  hasAnyRole(roles: string[]): boolean {
    const user = this.getCurrentUser();
    if (!user || !user.roles) return false;
    
    return roles.some(role => user.roles.includes(role));
  }

  /**
   * Verifica si el usuario puede acceder a una ruta específica
   */
  canAccessRoute(route: string): boolean {
    console.log('canAccessRoute - Verificando ruta:', route);
    
    const user = this.getCurrentUser();
    if (!user || !user.roles) {
      console.log('canAccessRoute - No hay usuario o roles');
      return false;
    }

    console.log('canAccessRoute - Roles del usuario:', user.roles);

    // Verificar si alguna de las rutas permitidas para los roles del usuario coincide
    for (const role of user.roles) {
      const rutasDelRol = this.rutasPermitidas[role as keyof typeof this.rutasPermitidas];
      console.log(`canAccessRoute - Rutas para rol ${role}:`, rutasDelRol);
      
      if (rutasDelRol && rutasDelRol.includes(route)) {
        console.log(`canAccessRoute - Ruta ${route} permitida para rol ${role}`);
        return true;
      }
    }

    console.log(`canAccessRoute - Ruta ${route} NO permitida`);
    return false;
  }

  /**
   * Obtiene todas las rutas permitidas para el usuario actual
   */
  getPermittedRoutes(): string[] {
    const user = this.getCurrentUser();
    if (!user || !user.roles) return [];

    const rutasPermitidas = new Set<string>();
    
    for (const role of user.roles) {
      const rutasDelRol = this.rutasPermitidas[role as keyof typeof this.rutasPermitidas];
      if (rutasDelRol) {
        rutasDelRol.forEach(ruta => rutasPermitidas.add(ruta));
      }
    }

    return Array.from(rutasPermitidas);
  }

  /**
   * Verifica si el usuario está autenticado
   */
  isAuthenticated(): boolean {
    return this.getCurrentUser() !== null;
  }

  /**
   * Redirige al usuario a la página principal según su rol
   */
  redirectToHome(): void {
    console.log('redirectToHome llamado');
    
    const user = this.getCurrentUser();
    console.log('Usuario actual:', user);
    
    if (!user) {
      console.log('No hay usuario, redirigiendo a login');
      this._router.navigate(['/auth/login']);
      return;
    }

    console.log('Roles del usuario:', user.roles);
    console.log('¿Es analista?', this.hasRole('ANALISTA'));

    // Si es analista, puede ir a consulta, si no a solicitud
    if (this.hasRole('ANALISTA')) {
      console.log('Redirigiendo a /starter');
      this._router.navigate(['/starter']);
    } else {
      console.log('Redirigiendo a /solicitudes/solicitud');
      this._router.navigate(['/solicitudes/solicitud']);
    }
  }
}
