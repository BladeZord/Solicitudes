import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

import { FullComponent } from "./layouts/full/full.component";
import { BlankComponent } from "./layouts/blank/blank.component";
import { AuthGuard } from "./modules/auth/guards/auth.guard";

export const Approutes: Routes = [
  {
    path: "",
    redirectTo: "/auth/login",
    pathMatch: "full",
  },
  {
    path: "",
    component: BlankComponent,
    children: [
      {
        path: "auth",
        loadChildren: () =>
          import("./modules/auth/auth.module").then((m) => m.AuthModule),
      },
    ],
  },
  {
    path: "",
    component: FullComponent,
    // canActivate: [AuthGuard], // Comentado temporalmente para debug
    children: [
      {
        path: "starter",
        loadChildren: () =>
          import("./starter/starter.module").then((m) => m.StarterModule),
      },
      {
        path: "mantenimiento",
        loadChildren: () =>
          import("./modules/mantenimiento/mantenimiento.module").then(
            (m) => m.MantenimientoModule
          ),
      },
      {
        path: "solicitudes",
        loadChildren: () =>
          import("./modules/solicitudes/solicitudes.module").then(
            (m) => m.SolicitudesModule
          ),
      },
      {
        path: "component",
        loadChildren: () =>
          import("./component/component.module").then(
            (m) => m.ComponentsModule
          ),
      },
    ],
  },
  {
    path: "**",
    redirectTo: "/auth/login",
  },
];
