# Detalhamento Técnico do Projeto

Este documento fornece uma visão aprofundada dos aspectos técnicos do sistema de emissão de notas fiscais.

## Estrutura do Projeto

O repositório está organizado em três pastas principais:

- `EstoqueService`: aplicação de estoque com arquitetura limpa (Clean Architecture) dividida em camadas: Domain, Application, Infrastructure e API.
- `FaturamentoService`: serviço de faturamento com estrutura similar à do estoque, também em arquitetura limpa.
- `Frontend`: aplicação Angular responsável pela interface do usuário.

Cada microsserviço .NET possui projetos separados para domínio, lógica de aplicação e infraestrutura, facilitando a separação de responsabilidades e testes.

## Ciclos de Vida do Angular Utilizados

A aplicação Angular utiliza o conjunto completo de ciclos de vida de componentes, incluindo:

- `ngOnInit` para inicialização de dados e chamadas a serviços;
- `ngOnChanges` para reagir a alterações de `@Input`;
- `ngOnDestroy` para limpeza de subscriptions e recursos;
- `ngAfterViewInit` e outros conforme necessário para operações após renderização.

Esses hooks são aplicados em componentes dentro das pastas `features`, `layout` e `shared` para gerenciar comportamento e estado.

## Uso da Biblioteca RxJS

RxJS é empregada extensivamente para lidar com operações assíncronas e streams de dados. Exemplos:

- `Observables` retornados pelos serviços `EstoqueService` e `FaturamentoService` para chamadas HTTP usando `HttpClient`.
- Uso de operadores como `map`, `switchMap`, `catchError`, `retry` e `takeUntil` para tratamento de fluxos, encadeamento de solicitações e cancelamento de subscriptions.
- Em componentes, as assinaturas são canceladas no `ngOnDestroy` para evitar vazamentos.

## Bibliotecas Adicionais Utilizadas e Suas Finalidades

- **Angular Material**: conjunto de componentes visuais (cards, botões, tabelas, dialogs) para padronização da UI.
- **Polly (no backend)**: implementação de políticas de retry para chamadas HTTP entre serviços (.NET).
- **AutoMapper**: mapeamento entre DTOs/entidades no backend.
- **Serilog**: logging estruturado com rotação de arquivos.
- **Entity Framework Core**: ORM para acesso a bancos MySQL e PostgreSQL.

## Bibliotecas de Componentes Visuais Utilizadas

A camada frontend utiliza principalmente **Angular Material** para:

- Layout responsivo
- Controles de formulário e validação
- Tabelas paginadas e ordenáveis
- Pop-ups e notificações

Também podem existir componentes personalizados na pasta `shared/components`.

## Frameworks Utilizados em C#

No backend .NET 8, são utilizados:

- **ASP.NET Core Web API** como base de projeto.
- **Entity Framework Core** para persistência

A arquitetura segue os princípios de Clean Architecture, separando camadas de domínio, aplicação e infraestrutura.

## Tratamento de Erros e Exceções no Backend

O tratamento de exceções é centralizado por meio de middleware (`ExceptionHandlingMiddleware`) que captura exceções não tratadas, registra no log e retorna respostas padronizadas ao cliente. Exceções de domínio específicas (`DomainException`, `SaldoInsuficienteException`, etc.) são lançadas pela camada de domínio e traduzidas em códigos HTTP adequados.

Além disso:

- Validações são feitas com validadores (provavelmente FluentValidation) na camada de application.
- Erros de comunicação entre serviços usam bibliotecas de retry (Polly) e captura de exceções HTTP.

## Detalhar o Uso de LINQ

LINQ é utilizado na camada de infraestrutura para consultar dados através do Entity Framework Core. Exemplos comuns:

- Filtragem de coleções com `.Where(...)`;
- Projeção de resultados usando `.Select(...)` para converter entidades em DTOs;
- Ordenação com `.OrderBy(...)`/`.OrderByDescending(...)`;
- Operações assíncronas com `ToListAsync()`, `FirstOrDefaultAsync()`, etc.

A sintaxe integrada de LINQ permite escrever consultas expressivas que são traduzidas em SQL pelo EF Core.
