import {
  Component,
  OnDestroy,
  OnInit,
} from "@angular/core";
import { CatalogoService } from "../../services/catalogo.service";
import { LogicaComunService } from "../../services/logica-comun.service";
import { CatalogoType } from "../../interfaces/catalogo.interface";
import { HttpErrorResponse } from "@angular/common/http";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: "app-catalogo",
  templateUrl: "./catalogo.component.html",
  styleUrls: ["./catalogo.component.scss"],
})
export class CatalogoComponent implements OnInit, OnDestroy {
  masterSeleccionado: boolean = false;
  idsSeleccionados: number[] = [];
  arrayListCatalogos: CatalogoType[] = [];
  arrayListCatalogosPadres: CatalogoType[] = [];
  formulario: CatalogoType;
  numberPages: number[] = [5, 10, 15, 20, 25, 50, 100];
  page = 1;
  pageSize = 5;

  constructor(
    private _catalogoService: CatalogoService,
    public _utilService: LogicaComunService,
    private _modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.obtenerDataCatalogos();
    this.reiniciarFormulario();
  }

  ngOnDestroy(): void {
    this._modalService.dismissAll();
  }

  reiniciarFormulario(): void {
    this.formulario = {
      id: 0,
      codigo: "",
      descripcion: "",
      padre_Id: null,
      tipo: "",
    };
  }

  abrirModal(content: any, catalogo?: CatalogoType): void {
    if (catalogo) {
      this.formulario = { ...catalogo };
    } else {
      this.reiniciarFormulario();
    }
    this.consultarCatalogsPorTipo("");
    this._modalService.open(content, {
      ariaLabelledBy: "modal-basic-title",
      size: "lg",
      backdrop: 'static',
      keyboard: false
    });
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
    const idsVisibles = this.arrayListCatalogos
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
    const idsVisibles = this.arrayListCatalogos
      .slice(
        (this.page - 1) * this.pageSize,
        (this.page - 1) * this.pageSize + this.pageSize
      )
      .map((x) => x.id);

    this.masterSeleccionado = idsVisibles.every((id) =>
      this.idsSeleccionados.includes(id)
    );
  }

  consultarCatalogsPorTipo(tipo?: string): void {
    this._catalogoService.obtenerCatalogosPorTipo(tipo).subscribe({
      next: (response) => {
        this.arrayListCatalogosPadres = response || [];
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error);
      },
    });
  }

  exportarAExcel(): void {
    if (this.arrayListCatalogos.length === 0) {
      this._utilService.mostrarMensaje(
        "warning",
        "No hay registros por exportar."
      );
      return;
    }
    const seleccionados =
      this.idsSeleccionados.length > 0
        ? this.arrayListCatalogos.filter((c) =>
            this.idsSeleccionados.includes(c.id)
          )
        : this.arrayListCatalogos;

    const encabezados = ["Id", "Código", "Descripción", "Tipo"];

    const datos = seleccionados.map((item) => ({
      Id: item.id,
      Código: item.codigo,
      Descripción: item.descripcion,
      Tipo: item.tipo,
    }));

    this._utilService.exportarExcel("Catalogos", encabezados, datos);
  }

  nuevoRegistro(content: any): void {
    this.reiniciarFormulario();
    this.abrirModal(content);
  }

  editarRegistro(content: any, catalogo: CatalogoType): void {
    console.log('Catálogo recibido para edición:', catalogo);
    this.formulario.id = catalogo.id;
    this.formulario.codigo = catalogo.codigo;
    this.formulario.descripcion = catalogo.descripcion;
    this.formulario.padre_Id = catalogo.padre_Id || null;
    this.formulario.tipo = catalogo.tipo || '';
    console.log('Formulario después de asignación explícita:', this.formulario);

    setTimeout(() => {
      this.abrirModal(content);
    }, 0);
  }

  guardarRegistro(): void {
    if (
      !this._utilService.isInputValido(this.formulario.codigo) ||
      !this._utilService.isInputValido(this.formulario.descripcion)
    ) {
      this._utilService.mostrarMensaje("warning", "Complete los campos.");
      return;
    }

    if (this.formulario.padre_Id && this.formulario.padre_Id > 0) {
      const registroPadre = this.arrayListCatalogosPadres.find(
        (x) => x.id === this.formulario.padre_Id
      );
      this.formulario.tipo = registroPadre ? registroPadre.codigo : "";
    } else {
      this.formulario.tipo = "";
    }

    if (this.formulario.id === 0) {
      this._catalogoService.AgregarCatalogo(this.formulario).subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            "Registro guardado correctamente"
          );
          this.arrayListCatalogos.push(response);
          this.arrayListCatalogosPadres.push(response);
          this.cerrarModal();
          this.obtenerDataCatalogos();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error);
        },
      });
    } else {
      this._catalogoService.ActualizarCatalogo(this.formulario).subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            "Registro actualizado correctamente"
          );
          const index = this.arrayListCatalogos.findIndex(
            (x) => x.id === response.id
          );
          if (index !== -1) {
            this.arrayListCatalogos[index] = response;
          }
          this.cerrarModal();
          this.obtenerDataCatalogos();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error);
        },
      });
    }
  }

  obtenerDataCatalogos(): void {
    this._catalogoService.obtenerCatalogos().subscribe({
      next: (response) => {
        this.arrayListCatalogos = response || [];
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error);
      },
    });
  }

  eliminarRegistro(id: number): void {
    if (!id) {
      this._utilService.mostrarMensaje("warning", "El id no es válido.");
      return;
    }
    this._catalogoService.EliminarCatalogo(id).subscribe({
      next: (response) => {
        this._utilService.mostrarMensaje("success", response);
        this.obtenerDataCatalogos();
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error);
      },
    });
  }
}
