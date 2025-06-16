import { Component, OnDestroy, OnInit } from "@angular/core";
import { UsuarioService } from "../../services/usuario.service";
import { LogicaComunService } from "../../services/logica-comun.service";
import { UsuarioType, RolType } from "../../interfaces/usuario.inteface";
import { HttpErrorResponse } from "@angular/common/http";
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { CatalogoService } from "../../services/catalogo.service";
import { CatalogoType } from "../../interfaces/catalogo.interface";
import { firstValueFrom } from "rxjs";

@Component({
  selector: "app-usuario",
  templateUrl: "./usuario.component.html",
  styleUrls: ["./usuario.component.scss"],
})
export class UsuarioComponent implements OnInit, OnDestroy {
  masterSeleccionado: boolean = false;
  idsSeleccionados: number[] = [];
  arrayListUsuarios: UsuarioType[] = [];
  arrayListRoles: CatalogoType[] = [];
  formulario: UsuarioType;
  formularioPassword: {
    id: number;
    contrasenia: string;
    confirmarContrasenia: string;
  };
  numberPages: number[] = [5, 10, 15, 20, 25, 50, 100];
  page = 1;
  pageSize = 5;
  rolesDisponibles: CatalogoType[] = [];
  rolesAsignados: CatalogoType[] = [];

  constructor(
    private _usuarioService: UsuarioService,
    private _catalogoService: CatalogoService,
    public _utilService: LogicaComunService,
    private _modalService: NgbModal
  ) {}

  ngOnInit(): void {
    this.obtenerDataUsuarios();
    this.reiniciarFormulario();
    this.cargarRoles();
  }

  ngOnDestroy(): void {
    this._modalService.dismissAll();
  }

  reiniciarFormulario(): void {
    this.formulario = {
      id: 0,
      nombre: "",
      apellidos: "",
      correo: "",
      contrasenia: "",
      domicilio: "",
      telefono: "",
      roles: [],
    };
  }

  reiniciarFormularioPassword(): void {
    this.formularioPassword = {
      id: 0,
      contrasenia: "",
      confirmarContrasenia: "",
    };
  }

  abrirModal(content: any, usuario?: UsuarioType): void {
    if (usuario) {
      this.editarRegistro(content, usuario);
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

  abrirModalPassword(content: any, id: number): void {
    this.reiniciarFormularioPassword();
    this.formularioPassword.id = id;
    this._modalService.open(content, {
      ariaLabelledBy: "modal-basic-title",
      size: "md",
      backdrop: "static",
      keyboard: false,
    });
  }

  abrirModalRoles(content: any, usuario: UsuarioType): void {
    this.formulario = { ...usuario };
    this.cargarRolesUsuario(usuario.id);
    this._modalService.open(content, {
      ariaLabelledBy: "modal-basic-title",
      size: "lg",
      backdrop: "static",
      keyboard: false,
    });
  }

  cargarRolesUsuario(usuarioId: number): void {
    this._usuarioService.obtenerRolesPorUsuario(usuarioId).subscribe({
      next: async (roles) => {
        // Obtener la información del catálogo para cada rol
        const rolesPromises = roles.map(async rol => {
          const catalogos = await firstValueFrom(this._catalogoService.obtenerCatalogosPorTipo("TIPO_PERSONA"));
          return catalogos.find(cat => cat.id === rol.rol_Id);
        });

        const catalogos = await Promise.all(rolesPromises);
        this.rolesAsignados = catalogos.filter(cat => cat !== undefined) as CatalogoType[];
        this.actualizarRolesDisponibles();
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  cargarRoles(): void {
    this._catalogoService.obtenerCatalogosPorTipo("TIPO_PERSONA").subscribe({
      next: (roles) => {
        this.arrayListRoles = roles;
      },
      error: (err: HttpErrorResponse) => {
        console.error("Error al cargar roles:", err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  actualizarRolesDisponibles(): void {
    this.rolesDisponibles = this.arrayListRoles.filter(
      (rol) => !this.rolesAsignados.some((r) => r.id === rol.id)
    );
  }

  asignarRol(rolId: number): void {
    this._usuarioService.asignarRol(this.formulario.id, rolId).subscribe({
      next: () => {
        this._utilService.mostrarMensaje(
          "success",
          "Rol asignado correctamente"
        );
        this.cargarRolesUsuario(this.formulario.id);
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }

  desasignarRol(rolId: number): void {
    this._usuarioService.desasignarRol(this.formulario.id, rolId).subscribe({
      next: () => {
        this._utilService.mostrarMensaje(
          "success",
          "Rol desasignado correctamente"
        );
        this.cargarRolesUsuario(this.formulario.id);
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
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
    const idsVisibles = this.arrayListUsuarios
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
    const idsVisibles = this.arrayListUsuarios
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
    if (this.arrayListUsuarios.length === 0) {
      this._utilService.mostrarMensaje(
        "warning",
        "No hay registros por exportar."
      );
      return;
    }
    const seleccionados =
      this.idsSeleccionados.length > 0
        ? this.arrayListUsuarios.filter((u) =>
            this.idsSeleccionados.includes(u.id)
          )
        : this.arrayListUsuarios;

    const encabezados = [
      "Id",
      "Nombre",
      "Apellidos",
      "Correo",
      "Domicilio",
      "Teléfono",
    ];

    const datos = seleccionados.map((item) => ({
      Id: item.id,
      Nombre: item.nombre,
      Apellidos: item.apellidos,
      Correo: item.correo,
      Domicilio: item.domicilio || "",
      Teléfono: item.telefono || "",
    }));

    this._utilService.exportarExcel("Usuarios", encabezados, datos);
  }

  nuevoRegistro(content: any): void {
    this.reiniciarFormulario();
    this.abrirModal(content);
  }

  editarRegistro(content: any, usuario: UsuarioType): void {
    this.reiniciarFormulario();
    this.formulario = {
      id: usuario.id,
      nombre: usuario.nombre || "",
      apellidos: usuario.apellidos || "",
      correo: usuario.correo || "",
      contrasenia: "", // No copiamos la contraseña por seguridad
      domicilio: usuario.domicilio || "",
      telefono: usuario.telefono || "",
      roles: usuario.roles ? [...usuario.roles] : [],
    };
    this._modalService.open(content, {
      ariaLabelledBy: "modal-basic-title",
      size: "lg",
      backdrop: "static",
      keyboard: false,
    });
  }

  guardarRegistro(): void {
    if (
      !this._utilService.isInputValido(this.formulario.nombre) ||
      !this._utilService.isInputValido(this.formulario.apellidos) ||
      !this._utilService.isInputValido(this.formulario.correo)
    ) {
      this._utilService.mostrarMensaje(
        "warning",
        "Complete los campos obligatorios."
      );
      return;
    }

    if (
      this.formulario.id === 0 &&
      !this._utilService.isInputValido(this.formulario.contrasenia)
    ) {
      this._utilService.mostrarMensaje(
        "warning",
        "La contraseña es obligatoria para nuevos usuarios."
      );
      return;
    }

    if (this.formulario.id === 0) {
      this._usuarioService.crearUsuario(this.formulario).subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            "Usuario creado correctamente"
          );
          this.arrayListUsuarios.push(response);
          this.cerrarModal();
          this.obtenerDataUsuarios();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error.message);
        },
      });
    } else {
      this._usuarioService.actualizarUsuario(this.formulario).subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            "Usuario actualizado correctamente"
          );
          const index = this.arrayListUsuarios.findIndex(
            (x) => x.id === response.id
          );
          if (index !== -1) {
            this.arrayListUsuarios[index] = response;
          }
          this.cerrarModal();
          this.obtenerDataUsuarios();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error.message);
        },
      });
    }
  }

  cambiarPassword(): void {
    if (!this._utilService.isInputValido(this.formularioPassword.contrasenia)) {
      this._utilService.mostrarMensaje(
        "warning",
        "La contraseña es obligatoria."
      );
      return;
    }

    if (
      this.formularioPassword.contrasenia !==
      this.formularioPassword.confirmarContrasenia
    ) {
      this._utilService.mostrarMensaje(
        "warning",
        "Las contraseñas no coinciden."
      );
      return;
    }

    this._usuarioService
      .cambiarContrasenia(
        this.formularioPassword.id,
        this.formularioPassword.contrasenia
      )
      .subscribe({
        next: (response) => {
          this._utilService.mostrarMensaje(
            "success",
            response || "Contraseña actualizada correctamente"
          );
          this.cerrarModal();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          this._utilService.mostrarMensaje("error", err.error.message);
        },
      });
  }

  obtenerDataUsuarios(): void {
    this._usuarioService.obtenerUsuarios().subscribe({
      next: (response) => {
        this.arrayListUsuarios = response || [];
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
    this._usuarioService.eliminarUsuario(id).subscribe({
      next: () => {
        this._utilService.mostrarMensaje(
          "success",
          "Usuario eliminado correctamente"
        );
        this.obtenerDataUsuarios();
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error.message);
      },
    });
  }
}