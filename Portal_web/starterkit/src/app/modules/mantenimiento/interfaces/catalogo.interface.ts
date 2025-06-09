export interface CatalogoType {
  id: number;
  codigo: string;
  descripcion: string;
  padre_Id?:number
  tipo?: string;
  selected?: boolean;
}
