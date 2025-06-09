import { Component, OnInit } from "@angular/core";
import { AuthService } from "../Services/auth.service";
import { CatalogoService } from "../../mantenimiento/services/catalogo.service";
import { AuthRequestType } from "../interfaces/AuthType.interface";
import { HttpErrorResponse } from "@angular/common/http";
import { CatalogoType } from "../../mantenimiento/interfaces/catalogo.interface";
import { Router } from "@angular/router";
import { UsuarioType } from "../../mantenimiento/interfaces/usuario.inteface";
import { UsuarioService } from "../../mantenimiento/services/usuario.service";
import { LogicaComunService } from "../../mantenimiento/services/logica-comun.service";

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"],
})
export class LoginComponent implements OnInit {
  formulario: AuthRequestType;
  listTipoPersona: CatalogoType[];
  bandera: boolean = false;
  contrasenia2: string = "";
  mostrarPasswordLogin: boolean = false;
  mostrarPasswordRegistro: boolean = false;
  mostrarPasswordRepetir: boolean = false;
  formularioRegistro: UsuarioType;

  constructor(
    private _loginService: AuthService,
    private _catalogoService: CatalogoService,
    private _router: Router,
    private _usuarioService: UsuarioService,
    private _utilService: LogicaComunService
  ) {}

  ngOnInit(): void {
    this.bandera = false; // En caso que su valor este en true
    this.ObtenerTipoPersona();
    this.reiniciarFormulario();
    this.reiniciarFormularioRegistro();
  }

  reiniciarFormulario() {
    this.formulario = {
      contrasenia: "",
      correo: "",
      perfil: "",
    };
  }

  reiniciarFormularioRegistro() {
    this.formularioRegistro = {
      id: 0,
      nombre: "",
      apellidos: "",
      correo: "",
      contrasenia: "",
      domicilio: "",
      telefono: "",
      roles: [
        {
          id: 0,
          descripcion: "",
        },
      ],
    };
  }

  iniciarSesion() {
    if (
      !this.formulario.correo ||
      this.formulario.contrasenia.trim() === "" ||
      !this.formulario.contrasenia ||
      this.formulario.contrasenia.trim() === ""
    ) {
      this._utilService.mostrarMensaje("warning", "Complete los campos");
      return;
    }

    this._loginService.login(this.formulario).subscribe({
      next: (response) => {
        if (!response) {
          this._utilService.mostrarMensaje(
            "warning",
            "Inicio de sesión fallido"
          );

          return;
        }
        this._utilService.mostrarMensaje("success", "Inicio de sesión exitoso");

        localStorage.setItem("usuario", JSON.stringify(response));
        this._router.navigate(["/starter"]);
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error);
      },
    });
  }
  
  ObtenerTipoPersona() {
    this._catalogoService.obtenerCatalogosPorTipo("TIPO_PERSONA").subscribe({
      next: (response) => {
        this.listTipoPersona = response;
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error);
      },
    });
  }

  mostarFormularioRegistro() {
    this.bandera = true;
    this.reiniciarFormularioRegistro();
  }

  guardarRegistro() {
    const camposRequeridos = [
      this.formularioRegistro.nombre,
      this.formularioRegistro.apellidos,
      this.formularioRegistro.domicilio,
      this.formularioRegistro.telefono,
      this.formularioRegistro.correo,
      this.formularioRegistro.contrasenia,
    ];

    const camposValidos = camposRequeridos.every((campo) =>
      this._utilService.isInputValido(campo)
    );

    if (!camposValidos) {
      this._utilService.mostrarMensaje("warning", "Complete los campos");
      return;
    }

    if (this.formularioRegistro.contrasenia !== this.contrasenia2) {
      this._utilService.mostrarMensaje(
        "warning",
        "Las contraseñas no coinciden"
      );
      return;
    }

    this._usuarioService.crearUsuario(this.formularioRegistro).subscribe({
      next: () => {
        this._utilService.mostrarMensaje(
          "success",
          "Usuario registrado exitosamente"
        );
        this.bandera = false;
    this.reiniciarFormularioRegistro();
      },
      error: (err: HttpErrorResponse) => {
        console.error(err);
        this._utilService.mostrarMensaje("error", err.error);
      },
    });
  }

  cancelarRegistro() {
    this.bandera = false;
    this.reiniciarFormularioRegistro();
  }
}
