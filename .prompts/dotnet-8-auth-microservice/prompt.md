# Prompts para usar com Codex no VS Code

## Prompt 1 — Criar estrutura inicial

```txt
Use as instruções do AGENTS.md e a skill .skills/dotnet-8-auth-microservice/SKILL.md.

Crie apenas a estrutura inicial da solução .NET 8, projetos, referências, pastas e pacotes essenciais.

Não implemente autenticação ainda.

Execute dotnet build no final e informe o resultado.
```

---

## Prompt 2 — Criar domínio

```txt
Use as instruções do AGENTS.md e a skill .skills/dotnet-8-auth-microservice/SKILL.md.

Agora implemente apenas o domínio inicial:

- User
- Role
- Permission
- RefreshToken

Não use EF Core no Domain.
Crie regras básicas de domínio e testes unitários para User e RefreshToken.
Execute dotnet build e dotnet test no final.
```

---

## Prompt 3 — Criar CQRS na Application

```txt
Use as instruções do AGENTS.md e a skill .skills/dotnet-8-auth-microservice/SKILL.md.

Agora implemente os casos de uso da camada Application usando CQRS:

- RegisterUserCommand
- LoginCommand
- RefreshTokenCommand
- RevokeRefreshTokenCommand
- GetCurrentUserQuery

Crie handlers, validators, responses e interfaces necessárias.
Não implemente Infrastructure ainda.
Execute dotnet build no final.
```

---

## Prompt 4 — Criar Infrastructure

```txt
Use as instruções do AGENTS.md e a skill .skills/dotnet-8-auth-microservice/SKILL.md.

Agora implemente a camada Infrastructure:

- AuthDbContext
- Mapeamentos EF Core
- Repositórios
- PasswordHasher
- JwtTokenService
- RefreshToken persistence

Use Options Pattern para JWT.
Não coloque segredos hardcoded.
Execute dotnet build no final.
```

---

## Prompt 5 — Criar API

```txt
Use as instruções do AGENTS.md e a skill .skills/dotnet-8-auth-microservice/SKILL.md.

Agora implemente a camada API:

- AuthController
- endpoint register
- endpoint login
- endpoint refresh-token
- endpoint revoke-token
- endpoint me

Configure Swagger e JWT Bearer Authentication.
Execute dotnet build no final.
```

---

## Prompt 6 — Revisão arquitetural

```txt
Use as instruções do AGENTS.md e a skill .skills/dotnet-8-auth-microservice/SKILL.md.

Revise a arquitetura atual do projeto e verifique:

- Se Domain está isolado.
- Se Controllers não possuem regra de negócio.
- Se Application está usando CQRS corretamente.
- Se Infrastructure não está vazando para a API.
- Se JWT está configurado com Options Pattern.
- Se PasswordHash não aparece em responses.
- Se existem abstrações desnecessárias.
- Se o projeto compila.

Não faça alterações ainda.
Primeiro apresente os problemas encontrados e uma ordem de correção.
```
