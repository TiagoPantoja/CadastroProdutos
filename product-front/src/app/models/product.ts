export interface Product {
  id?: string;
  codigo: string;
  descricao: string;
  departamento: string;
  preco: number;
  status: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface Department {
  codigo: string;
  descricao: string;
}

export interface CreateProductRequest {
  codigo: string;
  descricao: string;
  departamento: string;
  preco: number;
  status: boolean;
}

export interface UpdateProductRequest {
  codigo: string;
  descricao: string;
  departamento: string;
  preco: number;
  status: boolean;
}
