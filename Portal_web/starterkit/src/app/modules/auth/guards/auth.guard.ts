import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../Services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    console.log('AuthGuard - Verificando acceso a:', state.url);
    
    // Verificar si el usuario está autenticado
    if (!this.authService.isAuthenticated()) {
      console.log('AuthGuard - Usuario no autenticado, redirigiendo a login');
      this.router.navigate(['/auth/login']);
      return false;
    }

    // Verificar si el usuario puede acceder a la ruta
    if (!this.authService.canAccessRoute(state.url)) {
      console.log('AuthGuard - Usuario no tiene permisos para:', state.url);
      // Redirigir a la página principal según el rol
      this.authService.redirectToHome();
      return false;
    }

    console.log('AuthGuard - Acceso permitido a:', state.url);
    return true;
  }
} 