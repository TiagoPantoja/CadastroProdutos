# Sistema de Cadastro de Produtos

Sistema de cadastro de produtos com API REST .NET 9.0 e front Angular 17.

## Estrutura do Projeto

O projeto consiste em:
- **Backend**: API REST desenvolvida em .NET 9.0 com PostgreSQL
- **Frontend**: Aplicação Angular 17 com standalone components
- **Banco de Dados**: PostgreSQL com queries SQL explícitas

## Tecnologias Utilizadas

### Backend
- .NET 9.0
- ASP.NET Core Web API
- PostgreSQL
- Dapper
- Swagger/OpenAPI

### Frontend
- Angular 17
- TypeScript
- CSS
- Standalone Components

## 📋 Pré-requisitos

Certifique-se de ter instalado:

- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)**
- **[Node.js 18+](https://nodejs.org/)**
- **[Angular CLI](https://angular.io/cli)**: `npm install -g @angular/cli`
- **[PostgreSQL 12+](https://www.postgresql.org/download/)**
- **[Git](https://git-scm.com/downloads)**

### Verificar instalações:
```bash
dotnet --version    # Deve mostrar 9.0.x
node --version      # Deve mostrar 18.x ou superior
ng version          # Deve mostrar Angular CLI 17.x ou superior
psql --version      # Deve mostrar PostgreSQL 12.x ou superior
```

### Configuração e Instalação
1. Configurar o Banco de Dados

- Opção a) PostgreSQL Local
````bash
# Conectar ao PostgreSQL
psql -U postgres -h localhost

# Criar banco de dados
CREATE DATABASE productdb_dev;

# Sair
\q
````

- Opção b) PostgreSQL Docker (Recomendado)
```bash
# Executar PostgreSQL em container
docker run --name postgres-productapi \
  -e POSTGRES_PASSWORD=123456 \
  -e POSTGRES_DB=productdb_dev \
  -p 5432:5432 \
  -d postgres:15

# Verificar se está rodando
docker ps
```

2. Configurar o Backend
```bash
# Navegar para a pasta do backend
cd ProductAPI

# Restaurar dependências
dotnet restore

# Configurar string de conexão no appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=productdb_dev;Username=postgres;Password=SUA_SENHA;Port=5432"
  }
}

# Executar a API
dotnet run
```

API disponível:
- HTTPS: https://localhost:7001
- HTTP: http://localhost:5139
- Swagger: http://localhost:7001/swagger

3.Configurar o Frontend
```bash
# Em um novo terminal, navegar para a pasta do frontend
cd product-frontend

# Instalar dependências
npm install

# Verificar se a URL da API está correta em src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7001'
};

# Executar o frontend
ng serve
```

Frontend disponível:
http://localhost:4200