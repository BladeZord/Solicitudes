import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginComponent } from './login/login.component';
import { RegistroComponent } from './registro/registro.component';
import { RouterModule, Routes } from '@angular/router';
import { ToastrModule } from 'ngx-toastr';
import { FormsModule } from '@angular/forms';

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
    ToastrModule.forRoot(),
    FormsModule
  ]
})
export class AuthModule { }
