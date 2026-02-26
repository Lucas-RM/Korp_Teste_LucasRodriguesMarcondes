# Sistema de Emissão de Notas Fiscais

Sistema backend desenvolvido em .NET 8 com arquitetura de microsserviços e Clean Architecture.

## Estrutura do Projeto

O projeto é composto por dois microsserviços:

### 1. Estoque.API (Porta 5001)
- **Banco de Dados**: MySQL 8.0
- **Responsabilidade**: Cadastro e controle de saldo de produtos
- **Estrutura**: Clean Architecture (Domain, Application, Infrastructure, API)

### 2. Faturamento.API (Porta 5002)
- **Banco de Dados**: PostgreSQL 16
- **Responsabilidade**: Emissão e controle de notas fiscais
- **Estrutura**: Clean Architecture (Domain, Application, Infrastructure, API)

## Tecnologias Utilizadas

- .NET 8.0
- Entity Framework Core 8.0.4
- MySQL (Pomelo.EntityFrameworkCore.MySql)
- PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL)
- MediatR 12.2.0
- AutoMapper 13.0.1
- FluentValidation 11.9.2
- Serilog 4.0.0
- Polly 8.3.1 (Retry Policy)
- Swashbuckle (Swagger)

## Pré-requisitos

- .NET 8 SDK
- Docker e Docker Compose (para bancos de dados)
- MySQL 8.0 (ou via Docker)
- PostgreSQL 16 (ou via Docker)

## Configuração

### 1. Iniciar os bancos de dados via Docker Compose

```bash
docker-compose up -d
```

### 2. Configurar Connection Strings

Edite os arquivos `appsettings.json` de cada microsserviço conforme necessário:

**Estoque.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "EstoqueConnection": "Server=localhost;Port=3306;Database=estoque_db;User=root;Password=senha_root;"
  }
}
```

**Faturamento.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "FaturamentoConnection": "Host=localhost;Port=5432;Database=faturamento_db;Username=postgres;Password=senha_pg;"
  },
  "EstoqueService": {
    "BaseUrl": "http://localhost:5001"
  }
}
```

### 3. Aplicar Migrations

**Estoque:**
```bash
cd EstoqueService
dotnet ef migrations add InitialCreate --project src/Estoque.Infrastructure --startup-project src/Estoque.API
dotnet ef database update --project src/Estoque.Infrastructure --startup-project src/Estoque.API
```

**Faturamento:**
```bash
cd FaturamentoService
dotnet ef migrations add InitialCreate --project src/Faturamento.Infrastructure --startup-project src/Faturamento.API
dotnet ef database update --project src/Faturamento.Infrastructure --startup-project src/Faturamento.API
```

## Executar o Projeto

### Estoque.API
```bash
cd EstoqueService/src/Estoque.API
dotnet run
```
Acesse: http://localhost:5001/swagger

### Faturamento.API
```bash
cd FaturamentoService/src/Faturamento.API
dotnet run
```
Acesse: http://localhost:5002/swagger

## Endpoints Principais

### Estoque.API
- `GET /api/v1/produtos` - Listar produtos
- `GET /api/v1/produtos/{id}` - Obter produto por ID
- `POST /api/v1/produtos` - Criar produto
- `POST /api/v1/produtos/baixa-estoque` - Realizar baixa de estoque
- `PATCH /api/v1/produtos/{id}/simular-falha` - Ativar/desativar simulação de falha

### Faturamento.API
- `GET /api/v1/notas-fiscais` - Listar notas fiscais
- `GET /api/v1/notas-fiscais/{id}` - Obter nota fiscal por ID
- `POST /api/v1/notas-fiscais` - Criar nota fiscal
- `POST /api/v1/notas-fiscais/{id}/imprimir` - Imprimir nota fiscal (fecha a nota e realiza baixa de estoque)

## Fluxo de Impressão de Nota Fiscal

1. Cliente chama `POST /api/v1/notas-fiscais/{id}/imprimir`
2. Faturamento.API busca a nota fiscal e verifica se está Aberta
3. Faturamento.API monta requisição de baixa de estoque
4. Faturamento.API chama Estoque.API via HTTP (com Polly Retry: 3 tentativas, backoff exponencial)
5. Se sucesso: Nota é fechada (Status=Fechada) e commit no PostgreSQL
6. Se falha: Nota permanece Aberta, pode ser reimpressa

## Resiliência

- **Polly Retry Policy**: 3 tentativas com backoff exponencial (2s, 4s, 8s)
- **Transações**: Garantia de consistência - nota só é fechada após confirmação do estoque
- **Logs Estruturados**: Serilog com formato JSON compacto

## Observações

- As migrations são aplicadas automaticamente na inicialização (apenas em Development)
- Logs são salvos em `logs/` com rotação diária
- Swagger disponível em `/swagger` em ambos os microsserviços

