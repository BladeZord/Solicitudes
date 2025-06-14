import { Component, OnDestroy, OnInit } from "@angular/core";
import { SolicitudService } from "../../services/solicitud.service";
import { LogicaComunService } from "../../../mantenimiento/services/logica-comun.service";
import { HttpErrorResponse } from "@angular/common/http";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CatalogoService } from "../../../mantenimiento/services/catalogo.service";
import { CatalogoType } from "../../../mantenimiento/interfaces/catalogo.interface";
import { Observable, throwError } from "rxjs";
import { tap, catchError } from "rxjs/operators";
import { FiltrosHistorialAuditoriaType, HistorialAuditoriaType } from "../../interfaces/historial-auditoria.interface";
import { UsuarioService } from "../../../mantenimiento/services/usuario.service";
import { UsuarioType } from "../../../mantenimiento/interfaces/usuario.inteface";
import { SolicitudType } from "../../interfaces/solicitud.interface";

@Component({
  selector: "app-auditoria",
  templateUrl: "./auditoria.component.html",
  styleUrls: ["./auditoria.component.scss"],
})
export class AuditoriaComponent implements OnInit, OnDestroy {
  arrayListHistorial: HistorialAuditoriaType[] = [];
  arrayListEstados: CatalogoType[] = [];
  arrayListSolicitudes: SolicitudType[] = [];
  arrayListUsuarios: UsuarioType[] = [];
  numberPages: number[] = [5, 10, 15, 20, 25, 50, 100];
  page = 1;
  pageSize = 5;
  historial: HistorialAuditoriaType | null = null;
  filtros: FiltrosHistorialAuditoriaType = {
    solicitudId: 0,
    usuarioId: 0,
    estadoAnteriorId: 0,
    estadoActualId: 0,
    fechaInicio: null,
    fechaFin: null
  };

  constructor(
    private _solicitudService: SolicitudService,
    private _catalogoService: CatalogoService,
    private _usuarioService: UsuarioService,
    public _utilService: LogicaComunService,
    private _modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.limpiarFiltros();
    this.cargarDatosIniciales();
  }

  ngOnDestroy(): void {
    this._modalService.dismissAll();
  }

  cargarDatosIniciales(): void {
    // Cargar estados
    this.obtenerEstados().subscribe();
    // Cargar solicitudes
    this.obtenerSolicitudes().subscribe();
    // Cargar usuarios
    this.obtenerUsuarios().subscribe();
    // Aplicar filtros iniciales
    this.aplicarFiltros();
  }

  obtenerSolicitudes(): Observable<SolicitudType[]> {
    return this._solicitudService.obtenerSolicitudes().pipe(
      tap((response) => {
        this.arrayListSolicitudes = response || [];
      }),
      catchError((err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
        return throwError(() => err);
      })
    );
  }

  obtenerUsuarios(): Observable<UsuarioType[]> {
    return this._usuarioService.obtenerUsuarios().pipe(
      tap((response) => {
        this.arrayListUsuarios = response || [];
      }),
      catchError((err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
        return throwError(() => err);
      })
    );
  }

  aplicarFiltros(): void {
    this._solicitudService.obtenerHistorialAuditoria(this.filtros).subscribe({
      next: (response) => {
        this.arrayListHistorial = response || [];
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  limpiarFiltros(): void {
    this.filtros = {
      solicitudId: 0,
      usuarioId: 0,
      estadoAnteriorId: 0,
      estadoActualId: 0,
      fechaInicio: null,
      fechaFin: null
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

  abrirModalDetalle(content: any, historial: HistorialAuditoriaType): void {
    this.historial = historial;
    this._modalService.open(content, {
      ariaLabelledBy: "modal-basic-title",
      size: "lg",
      backdrop: "static",
      keyboard: false,
    });
  }

  imprimirDetalleHistorial(historial: HistorialAuditoriaType): void {
    const titulos = [
      "Fecha Registro",
      "Acción",
      "Estado Anterior",
      "Estado Actual",
      "Usuario",
      "Código Solicitud",
      "Monto",
      "Plazo (Meses)",
      "Fecha Solicitud"
    ];

    const data = [{
      "Fecha Registro": new Date(historial.fecha_registro).toLocaleString(),
      "Acción": historial.accion,
      "Estado Anterior": historial.estadoAnterior,
      "Estado Actual": historial.estadoActual,
      "Usuario": historial.nombreUsuario,
      "Código Solicitud": historial.codigoSolicitud,
      "Monto": `S/ ${historial.monto.toFixed(2)}`,
      "Plazo (Meses)": historial.plazoMeses,
      "Fecha Solicitud": new Date(historial.fechaSolicitud).toLocaleString()
    }];

    this._utilService.exportarPDF("Detalle de Historial", titulos, data);
  }
} 