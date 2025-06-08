import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegistroComponent } from './registro/registro.component';
import { RouterModule, Routes } from '@angular/router';

const AUTH_ROUTES: Routes = [
  {
    path: 'login',
    data: {
      // title: 'Inicio de sesión',
      // urls: [
      //   { title: 'Inicio de sesión', url: '/login' },
      // ]
    },
    component: LoginComponent
  },
  {
    path: '',
    data: {
      title: 'Registrarse',
      urls: [
        { title: 'Registrarse', url: '/registro' },
      ]
    },
    component: LoginComponent
  },
]


@NgModule({
  declarations: [
    LoginComponent,
    RegistroComponent,
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(AUTH_ROUTES),
    
  ]
})
export class AuthModule { }
