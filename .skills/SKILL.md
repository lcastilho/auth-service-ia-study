# Skill: dotnet-8-auth-microservice

## Objetivo

Criar ou configurar um microserviço de autenticação em .NET 8 usando DDD, Clean Architecture, Clean Code, SOLID e CQRS.

Esta skill deve ser usada quando a tarefa envolver criação, estruturação, revisão ou evolução de um serviço de autenticação.

---

## Quando usar

Use esta skill quando o usuário pedir para:

- Criar a estrutura inicial de um microserviço de autenticação.
- Configurar projeto .NET 8 com Clean Architecture.
- Criar autenticação com JWT.
- Criar login, cadastro, refresh token ou revogação de token.
- Criar features usando CQRS.
- Revisar arquitetura de autenticação.
- Criar testes para autenticação.
- Organizar um microserviço backend usando boas práticas.

---

## Stack obrigatória

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- CQRS
- MediatR
- FluentValidation
- JWT Bearer Authentication
- Swagger/OpenAPI
- SQL Server ou PostgreSQL
- xUnit
- Docker opcional

---

## Estrutura esperada

Criar ou manter a estrutura:

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

---

## Responsabilidades das camadas

### AuthService.Api

Responsável por:

- Controllers
- Middlewares
- Configuração da aplicação
- Swagger
- Autenticação JWT
- Dependency Injection da API

Não deve conter regra de negócio.

### AuthService.Application

Responsável por:

- Commands
- Queries
- Handlers
- Validators
- DTOs internos
- Interfaces
- Casos de uso
- Contratos internos de serviços

### AuthService.Domain

Responsável por:

- Entidades
- Value Objects
- Enums
- Regras de domínio
- Eventos de domínio

Não deve depender de:

- Entity Framework Core
- ASP.NET Core
- JWT
- Banco de dados
- Infrastructure
- HTTP

### AuthService.Infrastructure

Responsável por:

- DbContext
- Mapeamentos EF Core
- Repositórios
- Serviços externos
- Implementação de JWT
- Hash de senha
- Persistência de refresh token
- Implementações das interfaces da Application

### AuthService.Contracts

Responsável por:

- Requests públicos
- Responses públicos
- Contratos compartilháveis
- Eventos externos, se existirem

---

## Regras de dependência

- Api depende de Application, Infrastructure e Contracts.
- Application depende de Domain e Contracts.
- Infrastructure depende de Application e Domain.
- Domain não depende de nenhum outro projeto.
- Contracts deve evitar dependências desnecessárias.
- Domain não deve conhecer banco de dados, HTTP, JWT ou bibliotecas de infraestrutura.

---

## Padrões obrigatórios

Aplicar:

- DDD para modelagem do domínio.
- Clean Architecture para separação das camadas.
- SOLID para organização das responsabilidades.
- Clean Code para legibilidade.
- CQRS separando Commands e Queries.
- Dependency Injection para dependências.
- Options Pattern para configurações.
- Result Pattern quando fizer sentido.
- FluentValidation para validações de entrada.
- Testes unitários para regras críticas.

---

## Entidades iniciais

Criar inicialmente:

```txt
User
Role
Permission
RefreshToken
```

### User

Deve conter:

- Id
- Name
- Email
- PasswordHash
- IsActive
- CreatedAt
- UpdatedAt
- Roles
- RefreshTokens

Regras:

- Usuário inativo não pode autenticar.
- Senha nunca deve ser salva em texto puro.
- PasswordHash nunca deve sair em responses.
- E-mail deve ser único.
- E-mail deve estar em formato válido.
- Usuário deve ter data de criação.

### Role

Deve conter:

- Id
- Name
- Description
- Permissions

### Permission

Deve conter:

- Id
- Name
- Description

### RefreshToken

Deve conter:

- Id
- UserId
- Token
- ExpiresAt
- CreatedAt
- RevokedAt
- IsRevoked
- IsExpired

Regras:

- Token expirado não pode ser usado.
- Token revogado não pode ser usado.
- Token usado em renovação pode ser revogado conforme estratégia adotada.
- Deve ser possível revogar refresh token.

---

## Casos de uso iniciais

Implementar usando CQRS:

```txt
RegisterUserCommand
LoginCommand
RefreshTokenCommand
RevokeRefreshTokenCommand
GetCurrentUserQuery
```

Cada caso de uso deve conter:

```txt
Command ou Query
Handler
Validator
Response
Testes unitários quando aplicável
```

---

## Organização CQRS

Usar esta estrutura:

```txt
Application/
  Features/
    Auth/
      Commands/
        RegisterUser/
          RegisterUserCommand.cs
          RegisterUserCommandHandler.cs
          RegisterUserCommandValidator.cs
          RegisterUserResponse.cs

        Login/
          LoginCommand.cs
          LoginCommandHandler.cs
          LoginCommandValidator.cs
          LoginResponse.cs

        RefreshToken/
          RefreshTokenCommand.cs
          RefreshTokenCommandHandler.cs
          RefreshTokenCommandValidator.cs
          RefreshTokenResponse.cs

        RevokeRefreshToken/
          RevokeRefreshTokenCommand.cs
          RevokeRefreshTokenCommandHandler.cs
          RevokeRefreshTokenCommandValidator.cs

      Queries/
        GetCurrentUser/
          GetCurrentUserQuery.cs
          GetCurrentUserQueryHandler.cs
          CurrentUserResponse.cs
```

---

## Interfaces recomendadas na Application

Criar interfaces como:

```txt
IUserRepository
IRefreshTokenRepository
IPasswordHasher
IJwtTokenService
ICurrentUserService
IUnitOfWork
IDateTimeProvider
```

Evitar criar interfaces sem uso real.

---

## Pacotes recomendados

### AuthService.Api

```bash
dotnet add src/AuthService.Api/AuthService.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/AuthService.Api/AuthService.Api.csproj package Swashbuckle.AspNetCore
```

### AuthService.Application

```bash
dotnet add src/AuthService.Application/AuthService.Application.csproj package MediatR
dotnet add src/AuthService.Application/AuthService.Application.csproj package FluentValidation
dotnet add src/AuthService.Application/AuthService.Application.csproj package FluentValidation.DependencyInjectionExtensions
```

### AuthService.Infrastructure com SQL Server

```bash
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package BCrypt.Net-Next
```

### AuthService.Infrastructure com PostgreSQL

```bash
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj package BCrypt.Net-Next
```

### Testes

```bash
dotnet add tests/AuthService.UnitTests/AuthService.UnitTests.csproj package xunit
dotnet add tests/AuthService.UnitTests/AuthService.UnitTests.csproj package Moq
dotnet add tests/AuthService.UnitTests/AuthService.UnitTests.csproj package FluentAssertions
dotnet add tests/AuthService.UnitTests/AuthService.UnitTests.csproj package Microsoft.NET.Test.Sdk
```

---

## Comandos para criar a solution

Quando iniciar do zero, usar como referência:

```bash
mkdir AuthService
cd AuthService

dotnet new sln -n AuthService

mkdir src
mkdir tests

dotnet new webapi -n AuthService.Api -o src/AuthService.Api
dotnet new classlib -n AuthService.Application -o src/AuthService.Application
dotnet new classlib -n AuthService.Domain -o src/AuthService.Domain
dotnet new classlib -n AuthService.Infrastructure -o src/AuthService.Infrastructure
dotnet new classlib -n AuthService.Contracts -o src/AuthService.Contracts
dotnet new xunit -n AuthService.UnitTests -o tests/AuthService.UnitTests

dotnet sln add src/AuthService.Api/AuthService.Api.csproj
dotnet sln add src/AuthService.Application/AuthService.Application.csproj
dotnet sln add src/AuthService.Domain/AuthService.Domain.csproj
dotnet sln add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj
dotnet sln add src/AuthService.Contracts/AuthService.Contracts.csproj
dotnet sln add tests/AuthService.UnitTests/AuthService.UnitTests.csproj
```

---

## Referências entre projetos

```bash
dotnet add src/AuthService.Api/AuthService.Api.csproj reference src/AuthService.Application/AuthService.Application.csproj
dotnet add src/AuthService.Api/AuthService.Api.csproj reference src/AuthService.Infrastructure/AuthService.Infrastructure.csproj
dotnet add src/AuthService.Api/AuthService.Api.csproj reference src/AuthService.Contracts/AuthService.Contracts.csproj

dotnet add src/AuthService.Application/AuthService.Application.csproj reference src/AuthService.Domain/AuthService.Domain.csproj
dotnet add src/AuthService.Application/AuthService.Application.csproj reference src/AuthService.Contracts/AuthService.Contracts.csproj

dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj reference src/AuthService.Application/AuthService.Application.csproj
dotnet add src/AuthService.Infrastructure/AuthService.Infrastructure.csproj reference src/AuthService.Domain/AuthService.Domain.csproj

dotnet add tests/AuthService.UnitTests/AuthService.UnitTests.csproj reference src/AuthService.Application/AuthService.Application.csproj
dotnet add tests/AuthService.UnitTests/AuthService.UnitTests.csproj reference src/AuthService.Domain/AuthService.Domain.csproj
```

---

## Fluxo de execução da skill

Ao usar esta skill, execute nesta ordem:

1. Verificar se já existe solution `.sln`.
2. Verificar estrutura atual do projeto.
3. Criar projetos ausentes.
4. Configurar referências entre projetos.
5. Criar estrutura de pastas.
6. Instalar pacotes necessários.
7. Criar entidades de domínio.
8. Criar contratos.
9. Criar interfaces da Application.
10. Criar handlers CQRS.
11. Criar validators.
12. Criar infraestrutura.
13. Configurar JWT.
14. Criar controllers.
15. Criar testes iniciais.
16. Executar `dotnet build`.
17. Executar `dotnet test`, se houver testes.
18. Informar arquivos criados ou alterados.

---

## Regras de autenticação

- Usar JWT para access token.
- Usar refresh token persistido no banco.
- Nunca retornar PasswordHash.
- Nunca salvar senha em texto puro.
- Usar serviço próprio para hash de senha.
- Access token deve ter tempo curto de expiração.
- Refresh token deve poder ser revogado.
- Usuário inativo não pode fazer login.
- Login deve validar e-mail e senha.
- Segredos devem vir de configuração externa.
- Não deixar segredo JWT hardcoded no repositório.

---

## Controllers

Controllers devem:

- Receber requests.
- Chamar MediatR.
- Retornar responses HTTP.
- Não conter regra de negócio.
- Não acessar DbContext diretamente.
- Não manipular senha diretamente.
- Não construir JWT diretamente.

---

## Critérios de aceite

A tarefa só deve ser considerada concluída quando:

- A solução compilar.
- As referências entre projetos estiverem corretas.
- Domain estiver isolado.
- Application possuir os casos de uso.
- Infrastructure possuir implementação de persistência.
- API possuir endpoints básicos.
- Swagger estiver funcionando.
- Senha estiver protegida por hash.
- JWT estiver configurado por Options Pattern.
- README tiver instruções básicas de execução.
- Testes principais estiverem passando, quando existirem.

---

## Restrições

Não fazer:

- Não colocar regra de negócio no Controller.
- Não expor entidade de domínio diretamente na API.
- Não retornar PasswordHash.
- Não salvar senha em texto puro.
- Não colocar segredo JWT fixo no código.
- Não fazer Domain depender de Infrastructure.
- Não misturar Command com Query.
- Não criar abstrações sem uso real.
- Não criar código gigante em uma única etapa.
- Não ignorar erros de build.
- Não finalizar tarefa sem explicar o que foi feito.

---

## Comportamento esperado da IA

Quando esta skill for usada, a IA deve:

1. Confirmar o objetivo técnico da tarefa.
2. Ler a estrutura atual do projeto.
3. Fazer alterações pequenas e coesas.
4. Explicar decisões importantes.
5. Evitar overengineering.
6. Priorizar código simples, limpo e testável.
7. Executar build/testes quando possível.
8. Informar próximos passos ao final.
