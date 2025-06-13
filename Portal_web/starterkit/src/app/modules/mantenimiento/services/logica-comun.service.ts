import { Injectable } from "@angular/core";
import { ToastrService } from "ngx-toastr";
import * as XLSX from "xlsx";
import * as FileSaver from "file-saver";
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
type ToastType = "success" | "error" | "info" | "warning";

@Injectable({
  providedIn: "root",
})
export class LogicaComunService {
  constructor(private toastr: ToastrService) {}

  mostrarMensaje(
    tipo: ToastType,
    mensaje: string,
    titulo: string = ""
  ): void {
    switch (tipo) {
      case "success":
        this.toastr.success(mensaje, titulo);
        break;
      case "error":
        this.toastr.error(mensaje, titulo);
        break;
      case "info":
        this.toastr.info(mensaje, titulo);
        break;
      case "warning":
        this.toastr.warning(mensaje, titulo);
        break;
      default:
        this.toastr.info(mensaje, titulo);
        break;
    }
  }

  isInputValido(valor: string | null | undefined): boolean {
    return typeof valor === "string" && valor.trim().length > 0;
  }

  exportarExcel(entidad: string, encabezados: string[], data: any[]) {
    const worksheetData = [
      encabezados,
      ...data.map((obj) => encabezados.map((col) => obj[col])),
    ];

    const worksheet: XLSX.WorkSheet = XLSX.utils.aoa_to_sheet(worksheetData);

    // Aplica estilo a los encabezados (primer fila)
    const range = XLSX.utils.decode_range(worksheet["!ref"] || "");
    for (let C = range.s.c; C <= range.e.c; ++C) {
      const cellAddress = XLSX.utils.encode_cell({ r: 0, c: C });
      if (!worksheet[cellAddress]) continue;
      worksheet[cellAddress].s = {
        fill: {
          fgColor: { rgb: "1F4E78" }, // Azul oscuro
        },
        font: {
          bold: true,
          color: { rgb: "FFFFFF" }, // Letras blancas
        },
        alignment: {
          horizontal: "center",
          vertical: "center",
        },
      };
    }

    const workbook: XLSX.WorkBook = {
      Sheets: { data: worksheet },
      SheetNames: ["data"],
    };

    const excelBuffer: any = XLSX.write(workbook, {
      bookType: "xlsx",
      type: "array",
      cellStyles: true, // Activar estilos
    });

    const fileName = `${entidad}-${new Date().toISOString().slice(0, 10)}.xlsx`;
    const dataBlob: Blob = new Blob([excelBuffer], {
      type: "application/octet-stream",
    });
    FileSaver.saveAs(dataBlob, fileName);
  }

  /**
   * Exporta datos a un archivo PDF con un formato personalizado.
   * Permite agregar un encabezado de texto antes de la tabla y manejar formatos de tabla o formulario.
   * @param filename Nombre del archivo PDF.
   * @param headerText Texto que se mostrará en el encabezado del documento (opcional).
   * @param columns Columnas de la tabla (solo para formato de tabla).
   * @param data Datos a exportar. Puede ser un array de objetos para tabla o un objeto para formulario.
   * @param isFormatoTabla Booleano que indica si se debe exportar como tabla (true) o como formulario (false).
   */
  exportarPDFPersonalizado(
    filename: string,
    headerText: string[] = [],
    columns: any[] = [],
    data: any, // data puede ser un array de objetos para tabla o un objeto para formulario
    isFormatoTabla: boolean = true
  ): void {
    const doc = new jsPDF();
    let yOffset = 15; // Posición inicial Y para el contenido

    // Agregar texto de encabezado
    headerText.forEach(text => {
      doc.text(text, 14, yOffset);
      yOffset += 7; // Espacio entre líneas de texto
    });

    // Espacio antes de la tabla/formulario
    yOffset += 10;

    if (isFormatoTabla) {
      // Formato de tabla
      autoTable(doc, {
        head: [columns.map((col: any) => col.header)], // Solo los headers para el head
        body: data.map((row: any) => columns.map((col: any) => row[col.dataKey])),
        startY: yOffset,
        didDrawPage: (data) => {
          // Pie de página con número de página
          doc.text(
            `Página ${data.pageNumber}`,
            doc.internal.pageSize.width - 20,
            doc.internal.pageSize.height - 10,
            { align: "right" }
          );
        },
      });
    } else {
      // Formato de formulario (key-value pairs)
      for (const key in data) {
        if (Object.prototype.hasOwnProperty.call(data, key)) {
          doc.text(`${key}: ${data[key]}`, 14, yOffset);
          yOffset += 7;
          // Si el contenido excede el tamaño de la página, añadir nueva página
          if (yOffset > doc.internal.pageSize.height - 20) {
            doc.addPage();
            yOffset = 15; // Resetear Y para la nueva página
          }
        }
      }
    }

    doc.save(`${filename}.pdf`);
  }

  /**
   * Exporta datos a un archivo PDF (método original, ahora llama a exportarPDFPersonalizado para compatibilidad).
   * @param filename Nombre del archivo PDF.
   * @param titulos Títulos de las columnas.
   * @param data Datos a exportar.
   */
  exportarPDF(filename: string, titulos: string[], data: any[]): void {
    const columns = titulos.map(title => ({ header: title, dataKey: title }));
    this.exportarPDFPersonalizado(filename, [], columns, data, true);
  }

  imprimirTabla(nombre: string, encabezadoTabla: string[], data: any[]): void {
    const doc = new jsPDF();
    const pageWidth = doc.internal.pageSize.getWidth();
    const margin = 20;
    let startY = margin;

    // Configuración de estilos
    doc.setFont("helvetica", "bold");
    doc.setFontSize(16);
    doc.setTextColor(31, 78, 120); // Color azul oscuro

    // Título del reporte
    const titulo = nombre.toUpperCase();
    const tituloWidth = doc.getTextWidth(titulo);
    doc.text(titulo, (pageWidth - tituloWidth) / 2, startY);
    startY += 10;

    // Información del usuario y fecha
    const userData = localStorage.getItem("usuario");
    const usuario = userData ? JSON.parse(userData).nombre : "Usuario";
    const fecha = new Date().toLocaleDateString();
    
    doc.setFontSize(10);
    doc.setTextColor(100);
    doc.text(`Usuario: ${usuario}`, margin, startY);
    doc.text(`Fecha: ${fecha}`, pageWidth - margin - 40, startY);
    startY += 10;

    // Tabla de datos
    autoTable(doc, {
      startY: startY,
      head: [encabezadoTabla],
      body: data,
      theme: 'grid',
      headStyles: {
        fillColor: [31, 78, 120],
        textColor: 255,
        fontSize: 10,
        fontStyle: 'bold'
      },
      styles: {
        fontSize: 9,
        cellPadding: 3
      },
      margin: { top: startY }
    });

    // Guardar el PDF
    doc.save(`${nombre}-${new Date().toISOString().slice(0, 10)}.pdf`);
  }

  /**
   * Sanitiza un input para que solo acepte números
   * @param event Evento del input
   * @param allowDecimals Si permite decimales
   * @param maxLength Longitud máxima permitida
   */
  soloNumeros(event: KeyboardEvent, allowDecimals: boolean = false, maxLength?: number): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (maxLength && value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es número o punto decimal
    const pattern = allowDecimals ? /^[0-9.]*$/ : /^[0-9]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }

    // Si permite decimales, validar que no haya más de un punto
    if (allowDecimals && event.key === '.' && value.includes('.')) {
      event.preventDefault();
      return;
    }
  }

  /**
   * Formatea un código permitiendo solo letras, números y guiones bajos
   * @param event Evento del input
   * @param maxLength Longitud máxima permitida
   */
  formatearCodigoEspecial(event: KeyboardEvent, maxLength: number = 15): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es letra, número o guión bajo
    const pattern = /^[a-zA-Z0-9_]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }
  }

  /**
   * Formatea un campo de descripción permitiendo letras, números, espacios y caracteres especiales limitados
   * @param event Evento del input
   * @param maxLength Longitud máxima permitida
   */
  formatearDescripcion(event: KeyboardEvent, maxLength: number = 50): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es letra, número, espacio o caracteres especiales permitidos
    const pattern = /^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s,.\(\)]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }

    // Validar que no haya más de 2 espacios consecutivos
    if (event.key === ' ' && value.endsWith('  ')) {
      event.preventDefault();
      return;
    }
  }

  /**
   * Formatea un input para que solo acepte letras y espacios
   * @param event Evento del input
   * @param maxLength Longitud máxima permitida
   */
  soloLetras(event: KeyboardEvent, maxLength?: number): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (maxLength && value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es letra o espacio
    const pattern = /^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }
  }

  /**
   * Formatea un input para que solo acepte letras, números y espacios
   * @param event Evento del input
   * @param maxLength Longitud máxima permitida
   */
  alfanumerico(event: KeyboardEvent, maxLength?: number): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (maxLength && value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es letra, número o espacio
    const pattern = /^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑ\s]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }
  }

  /**
   * Formatea un input para que solo acepte correos electrónicos
   * @param event Evento del input
   * @param maxLength Longitud máxima permitida
   */
  formatearEmail(event: KeyboardEvent, maxLength?: number): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (maxLength && value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es caracter válido para email
    const pattern = /^[a-zA-Z0-9@._-]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }
  }

  /**
   * Formatea un input para que solo acepte números y el símbolo de moneda
   * @param event Evento del input
   * @param maxLength Longitud máxima permitida
   */
  formatearMoneda(event: KeyboardEvent, maxLength?: number): void {
    const input = event.target as HTMLInputElement;
    let value = input.value;

    // Si se presiona backspace o delete, permitir
    if (event.key === 'Backspace' || event.key === 'Delete') {
      return;
    }

    // Si se presiona tab, permitir
    if (event.key === 'Tab') {
      return;
    }

    // Validar longitud máxima
    if (maxLength && value.length >= maxLength) {
      event.preventDefault();
      return;
    }

    // Validar si es número o punto decimal
    const pattern = /^[0-9.]*$/;
    if (!pattern.test(event.key)) {
      event.preventDefault();
      return;
    }

    // Validar que no haya más de un punto decimal
    if (event.key === '.' && value.includes('.')) {
      event.preventDefault();
      return;
    }

    // Validar que no haya más de dos decimales
    if (value.includes('.')) {
      const decimales = value.split('.')[1];
      if (decimales && decimales.length >= 2) {
        event.preventDefault();
        return;
      }
    }
  }
}
