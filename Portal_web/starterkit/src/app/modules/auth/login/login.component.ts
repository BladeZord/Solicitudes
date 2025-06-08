import { Component, OnInit } from "@angular/core";
import { AuthService } from "../Services/auth.service";
import { CatalogoService } from "../../mantenimiento/services/catalogo.service";
import { AuthRequestType } from "../interfaces/AuthType.interface";
import { ToastrService } from "ngx-toastr";
import { HttpErrorResponse } from "@angular/common/http";
import { CatalogoType } from "../../mantenimiento/interfaces/catalogo.interface";
import { Router } from "@angular/router";
/**
 *{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJrZXZpbi5xdWl0b0B1bmlkYWRuZWdvY2lvLmNvbS5lYyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFuYWxpc3RhIGRlIHJldmlzaW9uIiwiZXhwIjoxNzQ5NDEzODQ2LCJpc3MiOiJlcy11c3VhcmlvIiwiYXVkIjoiZXMtdXN1YXJpby1jbGllbnQifQ._58qiihOWINPpuyDs-PbRfw0WQIfeVDzWixm-zoxd3o",
  "correo": "kevin.quito@unidadnegocio.com.ec",
  "nombre": "Kevin Quito",
  "rol": "Analista de revision"
}
 * 
 */

@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"],
})
export class LoginComponent implements OnInit {
  formulario: AuthRequestType;
  listTipoPersona: CatalogoType[];
  constructor(
    private _loginService: AuthService,
    private _catalogoService: CatalogoService,
    private _toastr: ToastrService,
    private _router: Router
  ) {}

  ngOnInit(): void {
    this.ObtenerTipoPersona();
    this.formulario = {
      contrasenia: "",
      correo: "",
      perfil: "",
    };
  }

  iniciarSesion() {
    if (
      !this.formulario.correo ||
      this.formulario.contrasenia.trim() === "" ||
      !this.formulario.contrasenia ||
      this.formulario.contrasenia.trim() === ""
    ) {
      this._toastr.warning("Complete los campos", "Aviso");
      return;
    }

    this._loginService.login(this.formulario).subscribe({
      next: (response) => {
        if (!response) {
          this._toastr.warning("Inicio de sesión fallido", "Aviso");
          return;
        }
        this._toastr.success("Inicio de sesión exitoso", "Aviso");
        localStorage.setItem("usuario", JSON.stringify(response));
        this._router.navigate(["/starter"]);
      }, error: (err: HttpErrorResponse) => {
        console.error(err);
        this._toastr.error(err.error.message, "Error");
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
        this._toastr.error(err.error.message, "Error");
      },
    });
  }
}
