import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
type ToastType = 'success' | 'error' | 'info' | 'warning';

@Injectable({
  providedIn: 'root'
})
export class LogicaComunService {

  constructor(private toastr: ToastrService) { }

  mostrarMensaje(tipo: ToastType, mensaje: string, titulo?: string): void {
    switch (tipo) {
      case 'success':
        this.toastr.success(mensaje, titulo || 'Éxito');
        break;
      case 'error':
        this.toastr.error(mensaje, titulo || 'Error');
        break;
      case 'info':
        this.toastr.info(mensaje, titulo || 'Info');
        break;
      case 'warning':
        this.toastr.warning(mensaje, titulo || 'Atención');
        break;
      default:
        this.toastr.show(mensaje, titulo || '');
        break;
    }
  }

  isInputValido(valor: string | null | undefined): boolean {
  return typeof valor === 'string' && valor.trim().length > 0;
}

}
