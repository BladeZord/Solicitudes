import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SolicitudComponent } from './pages/solicitud/solicitud.component';
import { ConsultasSolicitudesComponent } from './pages/consultas-solicitudes/consultas-solicitudes.component';
import { AuditoriaComponent } from './pages/auditoria/auditoria.component';
import { Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule, NgbTooltipModule, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';
registerLocaleData(localeEs);
const SOLICITUDES_ROUTES: Routes = [
  {
    path: "solicitud",
    data: {
      title: "Mis solicitudes",
      urls: [
        { title: "Solicitud", url: "/solicitud" },
        { title: "Mis solicitudes", url: "/solicitud" },
      ],
    },
    component: SolicitudComponent,
  },
  {
    path: "consulta",
    data: {
      title: "Consultas de Solicitudes",
      urls: [
        { title: "Consultas", url: "/consulta" },
        { title: "Consultas de Solicitudes", url: "/consulta" },
      ],
    },
    component: ConsultasSolicitudesComponent,
  },
  {
    path: "auditoria",
    data: {
      title: "Auditoría de Solicitudes",
      urls: [
        { title: "Auditoría", url: "/auditoria" },
        { title: "Auditoría de Solicitudes", url: "/auditoria" },
      ],
    },
    component: AuditoriaComponent,
  },
];

@NgModule({
  declarations: [
    SolicitudComponent,
    ConsultasSolicitudesComponent,
    AuditoriaComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(SOLICITUDES_ROUTES),
    FormsModule,
    NgbPaginationModule,
    ToastrModule.forRoot({ timeOut: 3000 }),
    NgbTooltipModule,
    NgbModalModule
  ]
})
export class SolicitudesModule { }
