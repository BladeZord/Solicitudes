import { Component, OnInit } from "@angular/core";
import { CatalogoService } from "../../services/catalogo.service";
import { LogicaComunService } from "../../services/logica-comun.service";
import { CatalogoType } from "../../interfaces/catalogo.interface";
import { HttpErrorResponse } from "@angular/common/http";

@Component({
  selector: "app-catalogo",
  templateUrl: "./catalogo.component.html",
  styleUrls: ["./catalogo.component.scss"],
})
export class CatalogoComponent implements OnInit {
  arrayListCatalogos: CatalogoType[] = [];
  numberPages: number[] = [5, 10, 15, 20, 25, 50, 100];
  page = 1;
  pageSize = 5;

  constructor(
    private _catalogoService: CatalogoService,
    private _utilService: LogicaComunService
  ) {}

  ngOnInit(): void {
    this.obtenerDataCatalogos();
  }

  

  obtenerDataCatalogos() {
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
}
