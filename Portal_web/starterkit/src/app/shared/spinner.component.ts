import {
  Component,
  Input,
  OnDestroy,
  Inject,
  ViewEncapsulation,
} from "@angular/core";
import {
  Router,
  NavigationStart,
  NavigationEnd,
  NavigationCancel,
  NavigationError,
} from "@angular/router";
import { DOCUMENT } from "@angular/common";

@Component({
  selector: "app-spinner",
  template: `
     <div
      class="position-fixed top-0 start-0 w-100 h-100 d-flex flex-column justify-content-center align-items-center bg-white"
      *ngIf="isSpinnerVisible"
      style="z-index: 9999"
    >
      <div class="spinner-border text-primary mb-3" style="width: 3rem; height: 3rem;" role="status"></div>
      <div class="fw-medium text-muted"><h3><strong class="text-primary">Cargando, por favor espere...</strong></h3></div>
    </div>
  `,
  encapsulation: ViewEncapsulation.None,
})
export class SpinnerComponent implements OnDestroy {
  public isSpinnerVisible = true;

  @Input() public backgroundColor = "#ffffff"; // Ya no lo usamos, pero lo dejo por si quieres reusarlo

  constructor(
    private router: Router,
    @Inject(DOCUMENT) private document: Document
  ) {
    this.router.events.subscribe(
      (event) => {
        if (event instanceof NavigationStart) {
          this.isSpinnerVisible = true;
        } else if (
          event instanceof NavigationEnd ||
          event instanceof NavigationCancel ||
          event instanceof NavigationError
        ) {
          this.isSpinnerVisible = false;
        }
      },
      () => {
        this.isSpinnerVisible = false;
      }
    );
  }

  ngOnDestroy(): void {
    this.isSpinnerVisible = false;
  }
}
