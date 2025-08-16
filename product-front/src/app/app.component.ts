import {ProductManagerComponent} from './components/product-manager/product-manager.component';
import {Component} from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [ProductManagerComponent],
  template: `
    <div class="container">
      <header style="text-align: center; margin-bottom: 30px;">
        <h1 style="color: #495057;">Sistema de Gerenciamento de Produtos</h1>
        <p style="color: #6c757d;">Cadastro e controle de produtos do e-commerce</p>
      </header>

      <app-product-manager></app-product-manager>
    </div>
  `,
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'product-frontend';
}
