import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CatalogoComponent } from './pages/catalogo/catalogo.component';
import { UsuarioComponent } from './pages/usuario/usuario.component';



@NgModule({
  declarations: [
    CatalogoComponent,
    UsuarioComponent
  ],
  imports: [
    CommonModule
  ]
})
export class MantenimientoModule { }
