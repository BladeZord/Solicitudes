import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SolicitudComponent } from './pages/solicitud/solicitud.component';
import { Routes } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbPaginationModule, NgbTooltipModule, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { ToastrModule } from 'ngx-toastr';

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
  
];

@NgModule({
  declarations: [SolicitudComponent],
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
