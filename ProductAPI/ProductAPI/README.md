# Product API - Sistema de Gerenciamento de Produtos

API REST desenvolvida para gerenciar produtos de e-commerce, construída com C#, .NET 9.0 e PostgreSQL.

## Sobre o projeto

API desenvolvida para um cenário real de e-commerce onde são gerenciados produtos de forma eficiente. A API foi codificada para ser simples, mas robusta o suficiente para uso em produção.

**Diferenciais do projeto:**
- Queries SQL explícitas
- Implementa exclusão lógica
- Validações dos campos
- Documentação usando Swagger

## Funcionalidades da API

### Gerenciamento de Produtos
Você pode criar, listar, atualizar e excluir produtos. Cada produto tem:
- Código único
- Descrição detalhada
- Departamento
- Preço
- Status ativo/inativo

### Sistema de Departamentos
A API possui 4 departamentos principais:
- **010** - BEBIDAS
- **020** - CONGELADOS
- **030** - LATICÍNIOS
- **040** - VEGETAIS

## Tecnologias utilizadas
- **[.NET 9.0](https://dotnet.microsoft.com/)** - Framework para desenvolvimento da API
- **[PostgreSQL](https://www.postgresql.org/)** - Banco de dados relacional gratuito
- **[Dapper](https://github.com/DapperLib/Dapper)** - Controle sobre as queries
- **[Swagger](https://swagger.io/)** - Para testar os endpoints da API
- **[XUnit](https://xunit.net/)** - Testes unitários da API

## Como Utilizar a API

Você vai precisar ter instalado:

- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**
- **[PostgreSQL](https://www.postgresql.org/download/)** (versão 12 ou superior)
- **[Git](https://git-scm.com/downloads)**

## Como instalar

### 1. Instalação com Docker (recomendado):
```bash
# Clone o projeto
git clone https://github.com/seu-usuario/product-api.git
cd product-api

# Suba tudo com Docker
docker-compose up -d

# Acesse a API
http://localhost:8080
```

### 2. Instalação Local:

### Pré-Requisitos:
- .NET 9.0 SDK
- PostgreSQL

```bash
# Clone o projeto
git clone https://github.com/seu-usuario/product-api.git
cd product-api

# Configure o banco
psql -U postgres -c "CREATE DATABASE productdb_dev;"

# Configure a conexão no appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productdb_dev;Username=postgres;Password=SUA_SENHA"
  }
}

# Execute
dotnet restore
dotnet run

# Acesse a API
https://localhost:7001
```

## Como Usar a API

### Swagger UI
Acesse http://localhost:8080 (Docker) ou https://localhost:7001 (Local) para testar a API.

### Endpoints Principais

**Departamentos:**
- `GET /api/departments` - Lista departamentos
- `GET /api/departments/010` - Busca departamento

**Produtos:**
- `GET /api/products` - Lista produtos
- `POST /api/products` - Cria produto
- `PUT /api/products/{id}` - Atualiza produto
- `DELETE /api/products/{id}` - Remove produto

### Exemplo: Criar Produto

**Body (JSON):**
```json
{
  "codigo": "PROD001",
  "descricao": "Produto Teste",
  "departamento": "010",
  "preco": 15.90,
  "status": true
}
```

### Departamentos Disponíveis
- **010** - BEBIDAS
- **020** - CONGELADOS
- **030** - LATICÍNIOS
- **040** - VEGETAIS

### Testes Unitários
Para rodar os testes unitários, execute:
```bash
dotnet test
```

