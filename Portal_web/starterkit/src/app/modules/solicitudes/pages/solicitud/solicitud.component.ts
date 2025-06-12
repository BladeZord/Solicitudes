import { Component, OnDestroy, OnInit } from "@angular/core";
import { SolicitudService } from "../../services/solicitud.service";
import { LogicaComunService } from "../../../mantenimiento/services/logica-comun.service";
import { SolicitudType } from "../../interfaces/solicitud.interface";
import { HttpErrorResponse } from "@angular/common/http";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CatalogoService } from "../../../mantenimiento/services/catalogo.service";
import { CatalogoType } from "../../../mantenimiento/interfaces/catalogo.interface";
import { Observable, throwError } from "rxjs";
import { tap, catchError } from "rxjs/operators";

@Component({
  selector: "app-solicitud",
  templateUrl: "./solicitud.component.html",
  styleUrls: ["./solicitud.component.scss"],
})
export class SolicitudComponent implements OnInit, OnDestroy {
  masterSeleccionado: boolean = false;
  idsSeleccionados: number[] = [];
  arrayListSolicitudes: SolicitudType[] = [];
  arrayListEstados: CatalogoType[] = [];
  formulario: SolicitudType;
  numberPages: number[] = [5, 10, 15, 20, 25, 50, 100];
  page = 1;
  pageSize = 5;

  constructor(
    private _solicitudService: SolicitudService,
    private _catalogoService: CatalogoService,
    public _utilService: LogicaComunService,
    private _modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.obtenerDataSolicitudes();
    this.obtenerEstados().subscribe();
    this.reiniciarFormulario();
  }

  ngOnDestroy(): void {
    this._modalService.dismissAll();
  }

  reiniciarFormulario(): void {
    const userData = localStorage.getItem("usuario");
    let usuarioId = 0;
    if (userData) {
      const user = JSON.parse(userData);
      usuarioId = user.id;
    }

    this.formulario = {
      id: 0,
      codigo: "",
      monto: 0,
      plazoMeses: 0,
      ingresosMensual: 0,
      antiguedadLaboral: 0,
      estado_Id: 0,
      fechaRegistro: new Date(),
      usuario_Id: usuarioId,
    };
  }

  obtenerEstados(): Observable<CatalogoType[]> {
    return this._catalogoService
      .obtenerCatalogosPorTipo("ESTADO_SOLICITUD")
      .pipe(
        tap((response) => {
          this.arrayListEstados = response || [];
        }),
        catchError((err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error.message);
          return throwError(() => err);
        })
      );
  }

  abrirModal(content: any, solicitud?: SolicitudType): void {
    if (solicitud) {
      this.editarRegistro(content, solicitud);
    } else {
      this.reiniciarFormulario();
      this._modalService.open(content, {
        ariaLabelledBy: "modal-basic-title",
        size: "lg",
        backdrop: "static",
        keyboard: false,
      });
    }
  }

  editarRegistro(content: any, solicitud: SolicitudType): void {
    this.reiniciarFormulario();
    this.formulario = {
      id: solicitud.id,
      codigo: solicitud.codigo,
      monto: solicitud.monto,
      plazoMeses: solicitud.plazoMeses,
      ingresosMensual: solicitud.ingresosMensual,
      antiguedadLaboral: solicitud.antiguedadLaboral,
      estado_Id: solicitud.estado_Id,
      fechaRegistro: new Date(solicitud.fechaRegistro),
      usuario_Id: solicitud.usuario_Id,
    };

    this._modalService.open(content, {
      ariaLabelledBy: "modal-basic-title",
      size: "lg",
      backdrop: "static",
      keyboard: false,
    });
  }

  imprimirDetalleSolicitud(solicitud: SolicitudType): void {
    const titulos = [
      "Código",
      "Monto",
      "Plazo (Meses)",
      "Ingresos Mensuales",
      "Antigüedad Laboral",
      "Estado",
      "Fecha de Registro"
    ];

    const data = [{
      "Código": solicitud.codigo,
      "Monto": `S/ ${solicitud.monto.toFixed(2)}`,
      "Plazo (Meses)": solicitud.plazoMeses,
      "Ingresos Mensuales": `S/ ${solicitud.ingresosMensual.toFixed(2)}`,
      "Antigüedad Laboral": `${solicitud.antiguedadLaboral} años`,
      "Estado": this.obtenerDescripcionEstado(solicitud.estado_Id),
      "Fecha de Registro": new Date(solicitud.fechaRegistro).toLocaleDateString()
    }];

    this._utilService.exportarPDF("Detalle de Solicitud", titulos, data);
  }

  cerrarModal(): void {
    this._modalService.dismissAll();
  }

  cancelar(): void {
    this.reiniciarFormulario();
    this.cerrarModal();
  }

  toggleSeleccion(id: number): void {
    if (this.idsSeleccionados.includes(id)) {
      this.idsSeleccionados = this.idsSeleccionados.filter((x) => x !== id);
    } else {
      this.idsSeleccionados.push(id);
    }
    this.actualizarMasterSeleccionado();
  }

  estaSeleccionado(id: number): boolean {
    return this.idsSeleccionados.includes(id);
  }

  toggleSeleccionarTodos(): void {
    const idsVisibles = this.arrayListSolicitudes
      .slice(
        (this.page - 1) * this.pageSize,
        (this.page - 1) * this.pageSize + this.pageSize
      )
      .map((x) => x.id);

    if (this.masterSeleccionado) {
      this.idsSeleccionados = Array.from(
        new Set([...this.idsSeleccionados, ...idsVisibles])
      );
    } else {
      this.idsSeleccionados = this.idsSeleccionados.filter(
        (id) => !idsVisibles.includes(id)
      );
    }
  }

  actualizarMasterSeleccionado(): void {
    const idsVisibles = this.arrayListSolicitudes
      .slice(
        (this.page - 1) * this.pageSize,
        (this.page - 1) * this.pageSize + this.pageSize
      )
      .map((x) => x.id);

    this.masterSeleccionado = idsVisibles.every((id) =>
      this.idsSeleccionados.includes(id)
    );
  }

  exportarAExcel(): void {
    if (this.arrayListSolicitudes.length === 0) {
      this._utilService.mostrarMensaje(
        "warning",
        "No hay registros por exportar."
      );
      return;
    }
    const seleccionados =
      this.idsSeleccionados.length > 0
        ? this.arrayListSolicitudes.filter((s) =>
            this.idsSeleccionados.includes(s.id)
          )
        : this.arrayListSolicitudes;

    const encabezados = [
      "N°",
      "Monto",
      "Plazo (Meses)",
      "Ingresos (Mensual)",
      "Antigüedad (Años)",
      "Estado",
      "Fecha de registro",
      "Usuario",
    ];

    const datos = seleccionados.map((item, index) => [
      index + 1,
      item.monto,
      item.plazoMeses,
      item.ingresosMensual,
      item.antiguedadLaboral,
      item.estado_Descripcion || "",
      new Date(item.fechaRegistro).toLocaleDateString(),
      item.nombre_Usuario || "",
    ]);

    this._utilService.imprimirTabla(
      "Reporte de Solicitudes",
      encabezados,
      datos
    );
  }

  nuevoRegistro(content: any): void {
    this.reiniciarFormulario();
    this.abrirModal(content);
  }

  guardarRegistro(): void {
    if (
      this.formulario.monto <= 0 ||
      this.formulario.plazoMeses <= 0 ||
      this.formulario.ingresosMensual <= 0 ||
      this.formulario.antiguedadLaboral <= 0
    ) {
      this._utilService.mostrarMensaje(
        "warning",
        "Complete todos los campos con valores válidos."
      );
      return;
    }

    const userData = localStorage.getItem("usuario");
    let usuarioId = 0;
    if (userData) {
      const user = JSON.parse(userData);
      usuarioId = user.id;
    }

    if (this.formulario.id === 0) {
      const { estado_Id, ...formularioSinEstado } = this.formulario;

      this._solicitudService.crearSolicitud(formularioSinEstado as SolicitudType).subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            "Solicitud creada correctamente"
          );
          this.arrayListSolicitudes.push(response);
          this.cerrarModal();
          this.obtenerDataSolicitudes();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error.message);
        },
      });
    } else {
      this._solicitudService.actualizarSolicitud(this.formulario).subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            "Solicitud actualizada correctamente"
          );
          const index = this.arrayListSolicitudes.findIndex(
            (x) => x.id === response.id
          );
          if (index !== -1) {
            this.arrayListSolicitudes[index] = response;
          }
          this.cerrarModal();
          this.obtenerDataSolicitudes();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error.message);
        },
      });
    }
  }

  obtenerDataSolicitudes(): void {
    const userData = localStorage.getItem("usuario");
    let usuarioId = 0;
    if (userData) {
      const user = JSON.parse(userData);
      usuarioId = user.id;
    }

    this._solicitudService.obtenerSolicitudesPorUsuario(usuarioId).subscribe({
      next: (response) => {
        this.arrayListSolicitudes = response || [];
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  eliminarRegistro(id: number): void {
    if (!id) {
      this._utilService.mostrarMensaje("warning", "El id no es válido.");
      return;
    }
    this._solicitudService.eliminarSolicitud(id).subscribe({
      next: () => {
        this._utilService.mostrarMensaje(
          "success",
          "Solicitud eliminada correctamente"
        );
        this.obtenerDataSolicitudes();
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  obtenerDescripcionEstado(estadoId: number): string {
    const estado = this.arrayListEstados.find((e) => e.id === estadoId);
    return estado ? estado.descripcion : "";
  }
}
