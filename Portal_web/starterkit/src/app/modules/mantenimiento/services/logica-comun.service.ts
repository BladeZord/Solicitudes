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

  mostrarMensaje(tipo: ToastType, mensaje: string, titulo?: string): void {
    switch (tipo) {
      case "success":
        this.toastr.success(mensaje, titulo || "Éxito");
        break;
      case "error":
        this.toastr.error(mensaje, titulo || "Error");
        break;
      case "info":
        this.toastr.info(mensaje, titulo || "Info");
        break;
      case "warning":
        this.toastr.warning(mensaje, titulo || "Atención");
        break;
      default:
        this.toastr.show(mensaje, titulo || "");
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

  exportarPDF(nombre: string, titulos: string[], data: any[]): void {
    const doc = new jsPDF();
    const pageWidth = doc.internal.pageSize.getWidth();
    const margin = 20;
    const startX = margin;
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
    doc.text(`Usuario: ${usuario}`, startX, startY);
    doc.text(`Fecha: ${fecha}`, pageWidth - margin - 40, startY);
    startY += 10;

    // Tabla de datos
    autoTable(doc, {
      startY: startY,
      head: [titulos],
      body: data.map(item => titulos.map(titulo => item[titulo])),
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
}
