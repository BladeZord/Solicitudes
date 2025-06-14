import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';
import { AuthService } from '../../modules/auth/Services/auth.service';

@Directive({
  selector: '[appRole]'
})
export class RoleDirective implements OnInit {
  private hasView = false;

  @Input() set appRole(roles: string | string[]) {
    const rolesArray = Array.isArray(roles) ? roles : [roles];
    const hasPermission = this.authService.hasAnyRole(rolesArray);

    if (hasPermission && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!hasPermission && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private authService: AuthService
  ) {}

  ngOnInit() {
    // La lógica se maneja en el setter del input
  }
} 