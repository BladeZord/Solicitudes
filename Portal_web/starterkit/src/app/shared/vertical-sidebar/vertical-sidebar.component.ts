import { Component, EventEmitter, Input, Output } from "@angular/core";
import { Router, RouterModule } from "@angular/router";
import { RouteInfo } from "./vertical-sidebar.metadata";
import { VerticalSidebarService } from "./vertical-sidebar.service";
import { TranslateModule } from "@ngx-translate/core";
import { CommonModule } from "@angular/common";
import { FeatherModule } from "angular-feather";
import { AuthService } from "src/app/modules/auth/Services/auth.service";
import { NgbDropdownModule } from "@ng-bootstrap/ng-bootstrap";
import { AuthResponseType } from "src/app/modules/auth/interfaces/AuthType.interface";

@Component({
  selector: "app-vertical-sidebar",
  standalone: true,
  imports: [
    TranslateModule,
    RouterModule,
    CommonModule,
    FeatherModule,
    NgbDropdownModule,
  ],
  templateUrl: "./vertical-sidebar.component.html",
})
export class VerticalSidebarComponent {
  showMenu = "";
  showSubMenu = "";
  public sidebarnavItems: RouteInfo[] = [];
  path = "";
  user = "";

  @Input() showClass: boolean = false;
  @Output() notify: EventEmitter<boolean> = new EventEmitter<boolean>();

  handleNotify() {
    this.notify.emit(!this.showClass);
  }

  constructor(
    private menuServise: VerticalSidebarService,
    private router: Router,
    private _authService: AuthService
  ) {
    // Deserializacion del usuario desde el localStorage
    const usuario: AuthResponseType = JSON.parse(
      localStorage.getItem("usuario") || "{}"
    );
    this.user = usuario?.nombre || "";
    this.menuServise.items.subscribe((menuItems) => {
      // Filtrar elementos del menú según los roles del usuario
      this.sidebarnavItems = this.filtrarMenuPorRol(menuItems);

      // Active menu
      this.sidebarnavItems.filter((m) =>
        m.submenu.filter((s) => {
          if (s.path === this.router.url) {
            this.path = m.title;
          }
        })
      );
      this.addExpandClass(this.path);
    });
  }

  /**
   * Filtra los elementos del menú según los roles del usuario
   */
  private filtrarMenuPorRol(menuItems: RouteInfo[]): RouteInfo[] {
    const rutasPermitidas = this._authService.getPermittedRoutes();
    
    return menuItems.map(item => {
      // Filtrar submenús según las rutas permitidas
      const submenuFiltrado = item.submenu.filter(subItem => 
        rutasPermitidas.includes(subItem.path)
      );
      
      // Solo incluir el elemento si tiene submenús permitidos
      if (submenuFiltrado.length > 0) {
        return {
          ...item,
          submenu: submenuFiltrado
        };
      }
      return null;
    }).filter(item => item !== null) as RouteInfo[];
  }

  addExpandClass(element: any) {
    if (element === this.showMenu) {
      this.showMenu = "0";
    } else {
      this.showMenu = element;
    }
  }

  logout() {
    this._authService.logout();
  }

  addActiveClass(element: any) {
    if (element === this.showSubMenu) {
      this.showSubMenu = "0";
    } else {
      this.showSubMenu = element;
    }
  }

  isMenuActive(menu: RouteInfo): boolean {
    return this.showMenu === menu.title;
  }

  isSubMenuActive(submenu: RouteInfo): boolean {
    return this.showSubMenu === submenu.title;
  }
}
