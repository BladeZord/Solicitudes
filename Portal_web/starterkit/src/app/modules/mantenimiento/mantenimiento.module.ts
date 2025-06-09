import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { CatalogoComponent } from "./pages/catalogo/catalogo.component";
import { UsuarioComponent } from "./pages/usuario/usuario.component";
import { RouterModule, Routes } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { NgbPaginationModule } from "@ng-bootstrap/ng-bootstrap";

const MANTENIMIENTO_ROUTES: Routes = [
  {
    path: "catalogo",
    data: {
      title: "Catalogo",
      urls: [
        { title: "Mantenimiento", url: "/catalogo" },
        { title: "Catalogo", url: "/catalogo" },
      ],
    },
    component: CatalogoComponent,
  },
  {
    path: "usuario",
    data: {
      title: "Usuario",
      urls: [
        { title: "Mantenimiento", url: "/catalogo" },

        { title: "Usuario", url: "/usuario" },
      ],
    },
    component: UsuarioComponent,
  },
];

@NgModule({
  declarations: [CatalogoComponent, UsuarioComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(MANTENIMIENTO_ROUTES),
    FormsModule,
    NgbPaginationModule
  ],
})
export class MantenimientoModule {}
