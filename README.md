# AuthService

Microservico de autenticacao em .NET 8, organizado com Clean Architecture, DDD, CQRS, MediatR, FluentValidation, Entity Framework Core, PostgreSQL, JWT Bearer Authentication e Swagger/OpenAPI.

## Estrutura

```txt
src/
  AuthService.Api/
  AuthService.Application/
  AuthService.Domain/
  AuthService.Infrastructure/
  AuthService.Contracts/

tests/
  AuthService.UnitTests/
  AuthService.IntegrationTests/
```

## Responsabilidades

- `AuthService.Api`: controllers, Swagger, autenticacao JWT e composicao de DI.
- `AuthService.Application`: commands, queries, handlers, validators, interfaces e casos de uso.
- `AuthService.Domain`: entidades e regras de dominio, sem dependencia de EF Core, HTTP, JWT ou Infrastructure.
- `AuthService.Infrastructure`: DbContext, mappings EF Core, repositorios, hash de senha, JWT e persistencia.
- `AuthService.Contracts`: requests e responses publicos da API.

## Funcionalidades

- Cadastro de usuario.
- Login com access token JWT e refresh token.
- Renovacao de token.
- Revogacao de refresh token.
- Consulta do usuario autenticado.

## Pre-requisitos

- .NET SDK 8
- PostgreSQL local na porta `5432`

No macOS com Homebrew:

```bash
brew install postgresql@16
brew services start postgresql@16
/opt/homebrew/opt/postgresql@16/bin/createdb auth_service
/opt/homebrew/opt/postgresql@16/bin/createuser --superuser postgres
```

Se o role `postgres` ja existir, o ultimo comando pode retornar erro e pode ser ignorado.

## Configuracao Local

A connection string de desenvolvimento esta em:

```txt
src/AuthService.Api/appsettings.json
```

Valor padrao:

```json
"ConnectionStrings": {
  "AuthDb": "Host=localhost;Port=5432;Database=auth_service;Username=postgres"
}
```

Configure o segredo JWT com user-secrets. Nao coloque segredo JWT no `appsettings.json`.

```bash
dotnet user-secrets set "Jwt:SecretKey" "local-development-secret-key-authservice-32chars-minimum" --project src/AuthService.Api/AuthService.Api.csproj
```

Opcionalmente, confie no certificado HTTPS de desenvolvimento:

```bash
dotnet dev-certs https --trust
```

## Executar

```bash
dotnet run --project src/AuthService.Api/AuthService.Api.csproj --launch-profile AuthService.Api
```

URLs locais:

```txt
https://localhost:7088
http://localhost:5088
```

Swagger:

```txt
https://localhost:7088/swagger
http://localhost:5088/swagger
```

Em ambiente `Development`, a API cria o schema do banco automaticamente com `EnsureCreatedAsync()`.

## Endpoints

```txt
POST /api/auth/register
POST /api/auth/login
POST /api/auth/refresh-token
POST /api/auth/revoke-token
GET  /api/auth/me
```

Exemplo de cadastro:

```bash
curl -k -X POST https://localhost:7088/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test.user@example.com","password":"StrongPass123"}'
```

## Build e Testes

```bash
dotnet restore AuthService.sln
dotnet build AuthService.sln
dotnet test AuthService.sln
```

## Observacoes de Seguranca

- Senhas sao persistidas usando hash com BCrypt.
- `PasswordHash` nao e retornado nas responses publicas.
- Refresh tokens sao persistidos e podem ser revogados.
- Segredos devem ser configurados por user-secrets, variaveis de ambiente ou outro provedor seguro.
- O access token possui expiracao curta e o refresh token possui expiracao maior.

## Proximos Passos

- Substituir `EnsureCreatedAsync()` por migrations EF Core.
- Adicionar testes de Application e Integration.
- Estruturar codigos de erro no `Result`.
- Adicionar Docker Compose para PostgreSQL.
