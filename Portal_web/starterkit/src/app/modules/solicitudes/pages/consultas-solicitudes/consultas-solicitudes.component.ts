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
import { FiltrosSolicitudType } from "../../interfaces/filtros-solicitud.interface";

@Component({
  selector: "app-consultas-solicitudes",
  templateUrl: "./consultas-solicitudes.component.html",
  styleUrls: ["./consultas-solicitudes.component.scss"],
})
export class ConsultasSolicitudesComponent implements OnInit, OnDestroy {
  arrayListSolicitudes: SolicitudType[] = [];
  arrayListEstados: CatalogoType[] = [];
  numberPages: number[] = [5, 10, 15, 20, 25, 50, 100];
  page = 1;
  pageSize = 5;
  filtros: FiltrosSolicitudType = {
    usuarioId: 0,
    estadoId: 0,
    fechaInicio: '',
    fechaFin: '',
  };
  masterSeleccionado: boolean = false;
  idsSeleccionados: number[] = [];
  formulario: SolicitudType; // Para el detalle y cambio de estado en el modal

  constructor(
    private _solicitudService: SolicitudService,
    private _catalogoService: CatalogoService,
    public _utilService: LogicaComunService,
    private _modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.limpiarFiltros();
    this.obtenerEstados().subscribe();
    this.aplicarFiltros(); // Cargar datos al inicio
    this.reiniciarFormulario(); // Inicializar formulario para la modal
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

  aplicarFiltros(): void {
    this._solicitudService.obtenerSolicitudesPorFiltros(this.filtros).subscribe({
      next: (response) => {
        this.arrayListSolicitudes = response || [];
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  limpiarFiltros(): void {
    this.filtros = {
      usuarioId: 0,
      estadoId: 0,
      fechaInicio: null,
      fechaFin: null,
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

  obtenerDescripcionEstado(estadoId: number): string {
    const estado = this.arrayListEstados.find((e) => e.id === estadoId);
    return estado ? estado.descripcion : "";
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

  abrirModalDetalleEstado(content: any, solicitud: SolicitudType): void {
    this.formulario = { ...solicitud }; // Copiar la solicitud para no modificar la original
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

  cambiarEstadoSolicitud(): void {
    const userData = localStorage.getItem("usuario");
    let usuarioAccionId = 0;
    if (userData) {
      const user = JSON.parse(userData);
      usuarioAccionId = user.id;
    }

    if (!this.formulario.id || this.formulario.estado_Id === 0) {
      this._utilService.mostrarMensaje("warning", "Seleccione un estado válido.");
      return;
    }

    const cambioEstadoPayload = {
      solicitudId: this.formulario.id,
      nuevoEstadoId: this.formulario.estado_Id,
      usuarioAccionId: usuarioAccionId,
    };

    this._solicitudService.cambiarEstadoSolicitud(cambioEstadoPayload).subscribe({
      next: () => {
        this._utilService.mostrarMensaje("success", "Estado de solicitud actualizado correctamente");
        this._modalService.dismissAll();
        this.aplicarFiltros(); // Refrescar la tabla
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  // Métodos de SolicitudComponent que no son relevantes para consultas:
  // nuevoRegistro, guardarRegistro, editarRegistro, eliminarRegistro, 
  // cerrarModal, cancelar. Se eliminarán del HTML también.
} 