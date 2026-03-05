# CandidatoLar API

> **REST API de nível entrevista em big tech** — CRUD de Pessoa e Telefone com  
> Clean Architecture · DDD · MediatR · EF Core 8 · SQL Server · Serilog · FluentValidation · Swagger

---

## Sumário

1. [Visão Geral](#visão-geral)
2. [Decisões Arquiteturais](#decisões-arquiteturais)
3. [Estrutura de Pastas](#estrutura-de-pastas)
4. [Pré-requisitos](#pré-requisitos)
5. [Como Rodar (LocalDB)](#como-rodar-localdb)
6. [Como Rodar via Docker](#como-rodar-via-docker)
7. [Aplicar Migrations](#aplicar-migrations)
8. [Como Rodar os Testes](#como-rodar-os-testes)
9. [Endpoints](#endpoints)
10. [Exemplos de Chamadas (curl)](#exemplos-de-chamadas-curl)
11. [Validação de CPF](#validação-de-cpf)
12. [Evolução Incremental (commits sugeridos)](#evolução-incremental)

---

## Visão Geral

Sistema web API para gerenciamento de **Pessoas** e seus **Telefones**, implementado como exercício de nível sênior/staff engineer. A solução demonstra:

- **Domain-Driven Design** com agregado rico (Pessoa), Value Object (Cpf), exceções de domínio nomeadas e invariantes encapsuladas no domain layer.
- **Clean Architecture** com dependências unidirecionais: `Domain ← Application ← Infrastructure ← Api`.
- **CQRS via MediatR** — Commands e Queries separados, ValidationBehaviour no pipeline.
- **EF Core 8** com Fluent API, owned entities, índice único no CPF, migration inicial incluída.
- **Serilog** com JSON structured logging e correlation ID por request.
- **FluentValidation** integrado ao pipeline do MediatR.
- **Swagger/OpenAPI** com versionamento (v1).
- **Health Check** monitorando o banco de dados.
- **Testes unitários** (xUnit + FluentAssertions + Moq) e **testes de integração** (WebApplicationFactory + EF InMemory).

---

## Decisões Arquiteturais

| Decisão | Motivo |
|---|---|
| **Clean Architecture** | Isola o domínio de frameworks/infra; facilita testes e substituição de tecnologias |
| **DDD rico** | Invariantes ficam no objeto correto; elimina `anemic domain model` e serviços de domínio excessivos |
| **MediatR (CQRS)** | Desacopla controllers de handlers; organização natural em Features; pipeline fácil de extender (logging, validação, etc.) |
| **Manual mapping** | Sem AutoMapper→ menos "magia", stacktraces mais claros e sem problemas de convenção |
| **Owned entity (Cpf)** | CPF como Value Object mapeado com `OwnsOne`→ sem tabela extra, sem FK, mas com comportamento encapsulado |
| **Hard delete** | Documentado explicitamente no README; fácil substituir por soft-delete adicionando `IsDeleted` + query filter |
| **In-memory DB nos testes** | Zero setup, isolamento por nome de banco, feedback rápido; `TestContainers` pode ser adicionado para testes de regressão mais fiéis |

---

## Estrutura de Pastas

```
CandidatoLar/
├── CandidatoLar.sln
├── .editorconfig
├── .gitignore
├── docker-compose.yml
├── src/
│   ├── CandidatoLar.Domain/
│   │   ├── Entities/          Pessoa.cs, Telefone.cs
│   │   ├── ValueObjects/      Cpf.cs
│   │   ├── Enums/             TipoTelefone.cs
│   │   └── Exceptions/        DomainException, EntityNotFoundException, ...
│   ├── CandidatoLar.Application/
│   │   ├── Common/            PagedResult, ValidationBehaviour
│   │   ├── Contracts/         IPessoaRepository, IUnitOfWork
│   │   ├── DTOs/              PessoaResponse, CreatePessoaRequest, TelefoneResponse, ...
│   │   ├── Features/
│   │   │   ├── Pessoas/       Commands + Queries + Handlers
│   │   │   └── Telefones/     Commands + Queries + Handlers
│   │   ├── Mappings/          PessoaMappings (manual)
│   │   └── Validators/        CreatePessoa, UpdatePessoa, AddTelefone, UpdateTelefone
│   ├── CandidatoLar.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/  PessoaConfiguration, TelefoneConfiguration
│   │   │   └── Migrations/      InitialCreate
│   │   ├── Repositories/      PessoaRepository, UnitOfWork
│   │   └── DependencyInjection.cs
│   └── CandidatoLar.Api/
│       ├── Controllers/       PessoasController, TelefonesController
│       ├── Extensions/        ApplicationServiceExtensions, SwaggerExtensions
│       ├── Middlewares/       ExceptionHandlingMiddleware, CorrelationIdMiddleware
│       ├── Properties/        launchSettings.json
│       ├── appsettings.json
│       └── Program.cs
└── tests/
    ├── CandidatoLar.Tests.Unit/
    │   ├── Domain/            CpfTests, PessoaDomainTests
    │   └── Application/       CreatePessoaCommandHandlerTests, AddTelefoneCommandHandlerTests
    └── CandidatoLar.Tests.Integration/
        ├── CustomWebApplicationFactory.cs
        └── Controllers/       PessoasControllerIntegrationTests
```

---

## Pré-requisitos

| Ferramenta | Versão mínima |
|---|---|
| .NET SDK | 8.0 |
| SQL Server LocalDB | Incluído no Visual Studio / instalável separadamente |
| (Opcional) Docker Desktop | Para rodar via container |

> **Instalar LocalDB:** [Download SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) → marcar "LocalDB" na instalação.

---

## Como Rodar (LocalDB)

```powershell
# 1. Clone / extraia o projeto
cd C:\caminho\para\CandidatoLar

# 2. Restaurar pacotes
dotnet restore

# 3. Aplicar migrations (cria banco automaticamente)
dotnet ef database update `
  --project src/CandidatoLar.Infrastructure `
  --startup-project src/CandidatoLar.Api

# 4. Iniciar a API
dotnet run --project src/CandidatoLar.Api

# 5. Abrir Swagger UI
Start-Process https://localhost:7100
```

> A migration também é aplicada automaticamente em Development ao iniciar a API (`db.Database.MigrateAsync()`).

---

## Como Rodar via Docker

```powershell
# 1. Subir SQL Server 2022
docker-compose up -d

# 2. Alterar connection string no appsettings.json:
#    Server=localhost,1433;Database=CandidatoLarDb;User Id=sa;Password=CandidatoLar@2024;TrustServerCertificate=True

# 3. Aplicar migrations
dotnet ef database update `
  --project src/CandidatoLar.Infrastructure `
  --startup-project src/CandidatoLar.Api

# 4. Rodar a API
dotnet run --project src/CandidatoLar.Api
```

---

## Aplicar Migrations

```powershell
# Aplicar migration existente
dotnet ef database update `
  --project src/CandidatoLar.Infrastructure `
  --startup-project src/CandidatoLar.Api

# Criar nova migration (após alterar domain/configs)
dotnet ef migrations add NomeDaMigration `
  --project src/CandidatoLar.Infrastructure `
  --startup-project src/CandidatoLar.Api `
  --output-dir Persistence/Migrations
```

---

## Como Rodar os Testes

```powershell
# Todos os testes
dotnet test

# Somente unitários
dotnet test tests/CandidatoLar.Tests.Unit

# Somente integração
dotnet test tests/CandidatoLar.Tests.Integration

# Com cobertura de código
dotnet test --collect:"XPlat Code Coverage"
```

---

## Endpoints

### Pessoas

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/pessoas` | Criar pessoa |
| `GET` | `/api/v1/pessoas` | Listar com filtros e paginação |
| `GET` | `/api/v1/pessoas/{id}` | Buscar por Id |
| `PUT` | `/api/v1/pessoas/{id}` | Atualizar nome e data de nascimento |
| `PATCH` | `/api/v1/pessoas/{id}/status` | Ativar / Desativar |
| `DELETE` | `/api/v1/pessoas/{id}` | Excluir (hard delete) |

### Telefones (sub-recurso)

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/v1/pessoas/{pessoaId}/telefones` | Adicionar telefone |
| `GET` | `/api/v1/pessoas/{pessoaId}/telefones` | Listar telefones |
| `PUT` | `/api/v1/pessoas/{pessoaId}/telefones/{telefoneId}` | Atualizar telefone |
| `DELETE` | `/api/v1/pessoas/{pessoaId}/telefones/{telefoneId}` | Remover telefone |

### Utilitários

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/health` | Health check (disponibilidade da API + banco) |
| `GET` | `/` | Swagger UI |

---

## Exemplos de Chamadas (curl)

### Criar Pessoa

```bash
curl -X POST https://localhost:7100/api/v1/pessoas \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João da Silva",
    "cpf": "529.982.247-25",
    "dataNascimento": "1990-06-15"
  }'
```

### Listar Pessoas (paginado + filtro)

```bash
curl "https://localhost:7100/api/v1/pessoas?nome=João&ativo=true&page=1&pageSize=10"
```

### Buscar por Id

```bash
curl https://localhost:7100/api/v1/pessoas/{id}
```

### Atualizar Pessoa

```bash
curl -X PUT https://localhost:7100/api/v1/pessoas/{id} \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva Atualizado",
    "dataNascimento": "1990-06-15"
  }'
```

### Ativar / Desativar

```bash
curl -X PATCH https://localhost:7100/api/v1/pessoas/{id}/status \
  -H "Content-Type: application/json" \
  -d '{ "ativo": false }'
```

### Adicionar Telefone

```bash
# Tipo: 1=Celular, 2=Residencial, 3=Comercial
curl -X POST https://localhost:7100/api/v1/pessoas/{pessoaId}/telefones \
  -H "Content-Type: application/json" \
  -d '{
    "tipo": 1,
    "numero": "11987654321"
  }'
```

### Deletar Telefone

```bash
curl -X DELETE https://localhost:7100/api/v1/pessoas/{pessoaId}/telefones/{telefoneId}
```

### Health Check

```bash
curl https://localhost:7100/health
```

---

## Validação de CPF

- É feita validação estrutural completa usando o algoritmo oficial (dois dígitos verificadores).
- O CPF é **normalizado** (somente dígitos) antes de persistir — `"529.982.247-25"` e `"52998224725"` são equivalentes.
- O índice único no banco garante exclusividade mesmo contra race conditions.
- CPFs com todos os dígitos iguais (`111.111.111-11`) são considerados inválidos pela regra de negócio.

**CPFs de teste válidos:**
- `529.982.247-25`
- `011.285.700-81`
- `153.509.460-56`
- `263.946.400-30`
- `568.999.350-60`

---

## Validação de Número de Telefone

- Somente dígitos (qualquer formatação é normalizada automaticamente).
- Mínimo 8 dígitos, máximo 13.
- Não pode existir duplicata com mesmo Tipo + Número na mesma Pessoa.

---

## Evolução Incremental

Commits sugeridos refletindo a ordem de implementação:

```
feat: base da solução, Clean Architecture e projeto Domain
feat: entidade Pessoa com invariantes e value object Cpf
feat: Application layer - MediatR commands/queries para Pessoa
feat: Infrastructure - AppDbContext, PessoaRepository, UoW, EF Migrations
feat: API - PessoasController, middlewares, Swagger, HealthCheck
feat: entidade Telefone e métodos ricos no agregado Pessoa
feat: Application layer - Commands e Queries de Telefone
feat: API - TelefonesController e endpoints de sub-recurso
test: testes unitários de domínio e application handlers
test: testes de integração com WebApplicationFactory + InMemory
docs: README completo com exemplos curl e guia de execução
```

---

## Tratamento de Erros

Todos os erros retornam `application/problem+json` (RFC 7807):

```json
{
  "status": 400,
  "title": "Validation Error",
  "detail": "One or more validation errors occurred.",
  "instance": "/api/v1/pessoas",
  "errors": {
    "Cpf": ["CPF is invalid."],
    "Nome": ["Nome must be between 2 and 120 characters."]
  },
  "correlationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

| Exceção | Status HTTP |
|---|---|
| `ValidationException` | 400 |
| `InvalidCpfException` | 400 |
| `DomainException` | 400 |
| `EntityNotFoundException` | 404 |
| `DuplicateCpfException` | 409 |
| `DuplicatePhoneException` | 409 |
| Qualquer outra | 500 |
