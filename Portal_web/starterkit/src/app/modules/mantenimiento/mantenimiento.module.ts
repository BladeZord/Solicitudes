import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { CatalogoComponent } from "./pages/catalogo/catalogo.component";
import { UsuarioComponent } from "./pages/usuario/usuario.component";
import { RouterModule, Routes } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { NgbModalModule, NgbPaginationModule, NgbTooltipModule } from "@ng-bootstrap/ng-bootstrap";
import { ToastrModule } from "ngx-toastr";

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
      title: "Usuarios",
      urls: [
        { title: "Mantenimiento", url: "/usuario" },

        { title: "Usuarios", url: "/usuario" },
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
    NgbPaginationModule,
    ToastrModule.forRoot({ timeOut: 3000 }),
    NgbTooltipModule,
    NgbModalModule
  ],
})
export class MantenimientoModule {}
