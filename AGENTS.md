# AGENTS.md

## Contexto do projeto

Este projeto é um microserviço de autenticação em .NET 8.

O objetivo é construir uma API de autenticação usando boas práticas de engenharia de software, DDD, Clean Architecture, Clean Code, SOLID e CQRS.

Este arquivo deve orientar o Codex no VS Code sobre as regras gerais do projeto.

---

## Stack principal

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- CQRS
- MediatR
- FluentValidation
- JWT Bearer Authentication
- Swagger/OpenAPI
- xUnit
- SQL Server ou PostgreSQL

---

## Arquitetura esperada

A solução deve seguir esta estrutura:

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

## Responsabilidade das camadas

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

### AuthService.Domain

Responsável por:

- Entidades
- Value Objects
- Enums
- Regras de domínio
- Eventos de domínio

Não deve depender de Entity Framework, HTTP, JWT ou Infrastructure.

### AuthService.Infrastructure

Responsável por:

- DbContext
- Mapeamentos EF Core
- Repositórios
- Serviços externos
- Implementação de JWT
- Hash de senha
- Persistência de refresh token

### AuthService.Contracts

Responsável por:

- Requests públicos
- Responses públicos
- Contratos compartilháveis
- Eventos externos, se existirem

---

## Regras de dependência

- Domain não depende de nenhum outro projeto.
- Application pode depender de Domain e Contracts.
- Infrastructure pode depender de Application e Domain.
- Api pode depender de Application, Infrastructure e Contracts.
- Contracts não deve depender de Infrastructure.
- Controllers não devem conter regra de negócio.
- DbContext não deve ser usado diretamente na Api.
- Regras de negócio devem ficar no Domain ou Application, conforme o caso.

---

## Padrões obrigatórios

Aplicar:

- DDD
- Clean Architecture
- SOLID
- Clean Code
- CQRS
- Dependency Injection
- Options Pattern
- Result Pattern quando fizer sentido
- FluentValidation para validações
- Testes unitários para regras importantes

---

## Segurança

- Nunca salvar senha em texto puro.
- Nunca retornar PasswordHash em responses.
- JWT deve ser configurado via Options Pattern.
- Segredos não devem ser hardcoded.
- Refresh token deve ser persistido e revogável.
- Usuário inativo não pode autenticar.
- Access token deve ter tempo curto de expiração.
- Refresh token deve ter tempo maior e controle de revogação.
- Dados sensíveis não devem aparecer em logs.

---

## Convenções de código

- Usar nomes claros e objetivos.
- Evitar métodos grandes.
- Evitar classes com múltiplas responsabilidades.
- Evitar abstrações sem uso real.
- Preferir código simples, legível e testável.
- Não usar entidades de domínio diretamente como response da API.
- Não misturar Command com Query.
- Não usar `DateTime.Now`; preferir abstração de relógio quando necessário.
- Usar `CancellationToken` em operações assíncronas.

---

## Como trabalhar com Codex

Antes de alterar código:

1. Leia a estrutura existente.
2. Identifique a camada correta para a alteração.
3. Faça alterações pequenas e coesas.
4. Evite criar código demais em uma única etapa.
5. Preserve decisões arquiteturais existentes.
6. Execute validações quando possível.

Ao final de cada tarefa, informe:

- Arquivos criados ou alterados.
- Decisões técnicas importantes.
- Comandos executados.
- Resultado do build/testes.
- Próximos passos sugeridos.

---

## Comandos úteis

```bash
dotnet restore
dotnet build
dotnet test
```

---

## Critérios gerais de qualidade

Antes de considerar uma tarefa concluída, verificar:

- A solução compila.
- O Domain continua isolado.
- Controllers não possuem regra de negócio.
- Não existe acesso direto ao DbContext na Api.
- Commands e Queries estão separados.
- Validações estão em Validators.
- Senha não é retornada em responses.
- Configurações sensíveis não estão hardcoded.
- README.md foi atualizado quando necessário.
