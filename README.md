# Sistema de Emissão de Notas Fiscais

Sistema de Emissão de Notas Fiscais é um sistema de gerenciamento de produtos e notas fiscais. Ele é composto por três serviços principais: Estoque, Faturamento e Frontend. O Estoque é responsável por gerenciar o estoque de produtos, incluindo cadastro, baixas e verificação de estoque.

## Índice

- [Sistema de Emissão de Notas Fiscais](#sistema-de-emissão-de-notas-fiscais)
  - [Índice](#índice)
  - [Estrutura do Projeto](#estrutura-do-projeto)
    - [1. Estoque.API (Porta 5001)](#1-estoqueapi-porta-5001)
    - [2. Faturamento.API (Porta 5002)](#2-faturamentoapi-porta-5002)
    - [3. Frontend (Porta 4200)](#3-frontend-porta-4200)
  - [Pré-requisitos](#pré-requisitos)
  - [Configuração](#configuração)
    - [1. Certifique-se de que os bancos de dados estão ativos](#1-certifique-se-de-que-os-bancos-de-dados-estão-ativos)
    - [2. Configurar Connection Strings](#2-configurar-connection-strings)
    - [3. Aplicar Migrations](#3-aplicar-migrations)
  - [Executar o Projeto](#executar-o-projeto)
    - [Estoque.API](#estoqueapi)
    - [Faturamento.API](#faturamentoapi)
    - [Frontend](#frontend)
  - [Observações](#observações)
  - [Arquivos Postman](#arquivos-postman)
  - [Fluxo de funcionamento da aplicação](#fluxo-de-funcionamento-da-aplicação)
  - [Configurando usuários e senhas do banco](#configurando-usuários-e-senhas-do-banco)
    - [MySQL](#mysql)
    - [PostgreSQL](#postgresql)
  - [Executando passo a passo](#executando-passo-a-passo)


## Estrutura do Projeto

O projeto é composto por dois microsserviços:

> 📘 Consulte também o [Detalhamento Técnico do Projeto](./DETALHAMENTO_T%C3%89CNICO_DO_PROJETO.md) para informações mais aprofundadas sobre arquitetura, bibliotecas e práticas adotadas.


### 1. Estoque.API (Porta 5001)
- **Banco de Dados**: MySQL 8.0
- **Responsabilidade**: Cadastro e controle de saldo de produtos
- **Estrutura**: Clean Architecture (Domain, Application, Infrastructure, API)

### 2. Faturamento.API (Porta 5002)
- **Banco de Dados**: PostgreSQL 16
- **Responsabilidade**: Emissão e controle de notas fiscais
- **Estrutura**: Clean Architecture (Domain, Application, Infrastructure, API)

### 3. Frontend (Porta 4200)
- **Tecnologia**: Angular 17+
- **Responsabilidade**: Interface de usuário para gerenciamento de produtos e notas fiscais
- **Estrutura**: core (interceptors, models, services), features, layout e shared

## Pré-requisitos

- .NET 8 SDK
- Angular CLI 17+
- Node.js 18+ (recomendado LTS)
- MySQL 8.0 instalado e em execução
- PostgreSQL 16 instalado e em execução

## Configuração

### 1. Certifique-se de que os bancos de dados estão ativos

Instale e inicie o MySQL e o PostgreSQL manualmente em seu ambiente. Use as ferramentas de sua preferência (serviço do sistema, container ou hospedagem externa).

### 2. Configurar Connection Strings

Edite os arquivos `appsettings.json` de cada microsserviço conforme necessário:

**Estoque.API/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "EstoqueConnection": "Server=localhost;Port=3306;Database=estoque_db;Uid=root;Pwd=senha_root;"
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

As migrations já estão criadas nos projetos; basta restaurar o banco de dados executando o comando de `update`.

**Estoque:**
```bash
cd EstoqueService
# as migrations já existem, apenas aplique-as
dotnet ef database update --project src/Estoque.Infrastructure --startup-project src/Estoque.API
```

**Faturamento:**
```bash
cd FaturamentoService
# as migrations já existem, apenas aplique-as
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

### Frontend
```bash
cd Frontend
npm install
npm start ou ng serve 
```
Acesse: http://localhost:4200

## Observações

- As migrations são aplicadas automaticamente na inicialização (apenas em Development)
- Logs são salvos em `logs/` com rotação diária
- Swagger disponível em `/swagger` em ambos os microsserviços

## Arquivos Postman

A pasta `PostmanCollections` na raiz do repositório contém dois JSONs exportados que podem ser importados diretamente no Postman:

- `EstoqueService.postman_collection.json`
- `FaturamentoService.postman_collection.json`

Eles já trazem exemplos de corpo e variáveis de rota. Defina a variável de ambiente `baseUrl` para o host/porta desejados (por exemplo `http://localhost:5001` ou `http://localhost:5002`).

## Fluxo de funcionamento da aplicação

1. O usuário interage com o **Front‑end Angular** (por padrão servido em `http://localhost:4200`).
2. O front chama os endpoints da API de _estoque_ para listar/criar produtos e realizar baixas.
3. Quando uma nota fiscal é criada/solicitada no front, o _Faturamento.API_ recebe o `POST` correspondente.
4. Ao imprimir uma nota (`POST /api/v1/notas-fiscais/{id}/imprimir`), o serviço de faturamento faz uma chamada HTTP ao **Estoque.API** para debitar quantidades.
5. A etapa de baixa é protegida por políticas de retry (Polly) e, em caso de sucesso, a nota é fechada no Postgres.
6. Em caso de erro de estoque, a nota permanece aberta para futuras tentativas.

Todos os serviços expõem documentação Swagger nos caminhos `/swagger` enquanto estiverem rodando.

## Configurando usuários e senhas do banco

### MySQL

1. Acesse o console do MySQL:
   ```bash
   mysql -u root -p
   ```
2. Para alterar a senha de um usuário existente:
   ```sql
   ALTER USER 'root'@'localhost' IDENTIFIED BY 'nova_senha';
   FLUSH PRIVILEGES;
   ```
3. Para criar um novo usuário e conceder permissões:
   ```sql
   CREATE USER 'appuser'@'localhost' IDENTIFIED BY 'senha';
   GRANT ALL PRIVILEGES ON estoque_db.* TO 'appuser'@'localhost';
   FLUSH PRIVILEGES;
   ```

### PostgreSQL

1. Entre no shell do `psql`:
   ```bash
   psql -U postgres
   ```
2. Mudar a senha de um usuário:
   ```sql
   ALTER USER postgres WITH PASSWORD 'nova_senha';
   ```
3. Criar um novo usuário e banco (se necessário):
   ```sql
   CREATE USER appuser WITH PASSWORD 'senha';
   CREATE DATABASE faturamento_db OWNER appuser;
   ```

Após alterar credenciais, atualize as connection strings nos respectivos arquivos `appsettings.json` ou configure variáveis de ambiente.

## Executando passo a passo

1. **Inicie os bancos de dados** MySQL e PostgreSQL.
2. **Restaure as dependências** dos projetos .NET com `dotnet restore` nos diretórios de cada serviço (por exemplo, `cd EstoqueService/src/Estoque.API && dotnet restore`).
3. **Aplique as migrations** conforme explicado na seção Configuração.
4. **Abra três terminais**:
   - Terminal 1: navegue até `EstoqueService/src/Estoque.API` e execute `dotnet run` (serviço em `http://localhost:5001`).
   - Terminal 2: vá para `FaturamentoService/src/Faturamento.API` e execute `dotnet run` (serviço em `http://localhost:5002`).
   - Terminal 3: no diretório `Frontend`, rode `npm install` (se necessário) e `npm start` ou `ng serve` para iniciar a aplicação Angular em `http://localhost:4200`.
5. **Acesse**:
   - Backend Estoque: `http://localhost:5001/swagger`
   - Backend Faturamento: `http://localhost:5002/swagger`
   - Front‑end: `http://localhost:4200`

Os endpoints estão documentados acima. Ajuste o `baseUrl` no front ou nos Postman collections conforme a porta ou host desejados.
