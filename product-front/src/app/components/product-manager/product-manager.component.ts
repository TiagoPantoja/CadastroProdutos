import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../services/product.service';
import { Product, Department, CreateProductRequest, UpdateProductRequest } from '../../models/product';

@Component({
  selector: 'app-product-manager',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-manager.component.html',
  styleUrls: ['./product-manager.component.css']
})
export class ProductManagerComponent implements OnInit {
  products: Product[] = [];
  departments: Department[] = [];

  loading = false;
  error = '';

  showModal = false;
  modalTitle = '';
  isEditMode = false;

  currentProduct: Product | null = null;
  formData = {
    codigo: '',
    descricao: '',
    departamento: '',
    preco: 0,
    status: true
  };
  formErrors: any = {};

  showDeleteConfirm = false;
  productToDelete: Product | null = null;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
    this.loadDepartments();
  }

  loadProducts(): void {
    this.loading = true;
    this.error = '';

    this.productService.getProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.loading = false;
      },
      error: (error) => {
        this.error = error;
        this.loading = false;
      }
    });
  }

  loadDepartments(): void {
    this.productService.getDepartments().subscribe({
      next: (departments) => {
        this.departments = departments;
      },
      error: (error) => {
        console.error('Erro ao carregar departamentos:', error);
      }
    });
  }

  getDepartmentName(codigo: string): string {
    return this.productService.getDepartmentName(codigo);
  }

  formatPrice(price: number): string {
    return this.productService.formatPrice(price);
  }

  openCreateModal(): void {
    this.modalTitle = 'Novo Produto';
    this.isEditMode = false;
    this.currentProduct = null;
    this.resetForm();
    this.showModal = true;
  }

  openEditModal(product: Product): void {
    this.modalTitle = 'Editar Produto';
    this.isEditMode = true;
    this.currentProduct = product;
    this.formData = {
      codigo: product.codigo,
      descricao: product.descricao,
      departamento: product.departamento,
      preco: product.preco,
      status: product.status
    };
    this.formErrors = {};
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.resetForm();
  }

  resetForm(): void {
    this.formData = {
      codigo: '',
      descricao: '',
      departamento: '',
      preco: 0,
      status: true
    };
    this.formErrors = {};
  }

  validateForm(): boolean {
    this.formErrors = {};
    let isValid = true;

    if (!this.formData.codigo.trim()) {
      this.formErrors.codigo = 'Código é obrigatório';
      isValid = false;
    }

    if (!this.formData.descricao.trim()) {
      this.formErrors.descricao = 'Descrição é obrigatória';
      isValid = false;
    }

    if (!this.formData.departamento) {
      this.formErrors.departamento = 'Departamento é obrigatório';
      isValid = false;
    }

    if (this.formData.preco <= 0) {
      this.formErrors.preco = 'Preço deve ser maior que zero';
      isValid = false;
    }

    return isValid;
  }

  saveProduct(): void {
    if (!this.validateForm()) {
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.currentProduct) {
      const updateData: UpdateProductRequest = { ...this.formData };

      this.productService.updateProduct(this.currentProduct.id!, updateData).subscribe({
        next: () => {
          this.loading = false;
          this.closeModal();
          this.loadProducts();
          alert('Produto atualizado com sucesso!');
        },
        error: (error) => {
          this.loading = false;
          alert(`Erro ao atualizar produto: ${error}`);
        }
      });
    } else {
      // Criar novo produto
      const createData: CreateProductRequest = { ...this.formData };

      this.productService.createProduct(createData).subscribe({
        next: () => {
          this.loading = false;
          this.closeModal();
          this.loadProducts();
          alert('Produto criado com sucesso!');
        },
        error: (error) => {
          this.loading = false;
          alert(`Erro ao criar produto: ${error}`);
        }
      });
    }
  }

  openDeleteConfirm(product: Product): void {
    this.productToDelete = product;
    this.showDeleteConfirm = true;
  }

  closeDeleteConfirm(): void {
    this.showDeleteConfirm = false;
    this.productToDelete = null;
  }

  confirmDelete(): void {
    if (!this.productToDelete?.id) return;

    this.loading = true;

    this.productService.deleteProduct(this.productToDelete.id).subscribe({
      next: () => {
        this.loading = false;
        this.closeDeleteConfirm();
        this.loadProducts();
        alert('Produto excluído com sucesso!');
      },
      error: (error) => {
        this.loading = false;
        alert(`Erro ao excluir produto: ${error}`);
      }
    });
  }

  refreshProducts(): void {
    this.loadProducts();
  }
}
