# Skill: dotnet-efcore-migrations

## Objetivo

Usar esta skill para substituir `EnsureCreatedAsync()` por **EF Core Migrations** em projetos .NET 8 organizados com Clean Architecture, DDD, CQRS e separação em camadas.

Esta skill orienta o Codex no VS Code a configurar migrations de forma segura, previsível e alinhada com boas práticas.

---

## Quando usar

Use esta skill quando o usuário pedir para:

- Substituir `EnsureCreatedAsync()` por EF Core Migrations.
- Criar migration inicial.
- Configurar migrations em solution .NET 8 com múltiplos projetos.
- Ajustar persistência na camada Infrastructure.
- Atualizar README com comandos reais de banco.
- Revisar se o projeto usa migrations corretamente.
- Preparar a aplicação para evolução versionada do schema.

---

## Contexto esperado

A solução normalmente segue esta estrutura:

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

Camada responsável por EF Core:

```txt
src/AuthService.Infrastructure/
```

Projeto inicializável:

```txt
src/AuthService.Api/
```

---

## Regra principal

Não usar `EnsureCreatedAsync()` em projetos que utilizam migrations.

O schema deve ser criado/evoluído por comandos explícitos:

```bash
dotnet ef migrations add InitialCreate \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj \
  --output-dir Persistence/Migrations
```

```bash
dotnet ef database update \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj
```

---

## Princípios obrigatórios

- Manter o `DbContext` na Infrastructure.
- Manter entidades no Domain.
- Manter mappings EF Core na Infrastructure.
- Não colocar migrations em Api, Domain ou Application.
- Não colocar regra de negócio no `DbContext`.
- Não fazer Domain depender de EF Core.
- Não usar `EnsureCreatedAsync()` junto com migrations.
- Não apagar dados ou migrations existentes sem confirmação.
- Não recriar banco automaticamente em runtime.
- Não colocar connection string com senha real no repositório.
- Usar comandos explícitos para criar e aplicar migrations.

---

## Pacotes necessários

### PostgreSQL

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

### SQL Server

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```

O projeto inicializável normalmente não precisa de `Microsoft.EntityFrameworkCore.Design`, desde que o comando use `--project` e `--startup-project`.

---

## Ferramenta dotnet-ef

Antes de criar migrations, verificar:

```bash
dotnet ef --version
```

Se não estiver instalada:

```bash
dotnet tool install --global dotnet-ef
```

Se estiver desatualizada:

```bash
dotnet tool update --global dotnet-ef
```

A versão principal deve ser compatível com EF Core 8.

---

## Arquivos a verificar

Antes de alterar, verificar:

```txt
src/AuthService.Api/Program.cs
src/AuthService.Api/appsettings.json
src/AuthService.Infrastructure/AuthService.Infrastructure.csproj
src/AuthService.Infrastructure/Persistence/AuthDbContext.cs
src/AuthService.Infrastructure/DependencyInjection/InfrastructureServiceCollectionExtensions.cs
src/AuthService.Infrastructure/Persistence/Configurations/
README.md
```

Se algum arquivo não existir, localizar o equivalente.

---

## Fluxo obrigatório

1. Ler estrutura da solution.
2. Identificar projeto do `DbContext`.
3. Identificar projeto inicializável.
4. Verificar uso de `EnsureCreatedAsync()`.
5. Verificar se já existem migrations.
6. Verificar pacotes EF Core necessários.
7. Verificar connection string.
8. Remover `EnsureCreatedAsync()`.
9. Criar migration inicial, se não existir.
10. Revisar migration gerada.
11. Atualizar README.
12. Executar `dotnet build`.
13. Executar `dotnet test`, se houver testes.
14. Informar arquivos alterados e comandos executados.

---

## Remoção de EnsureCreatedAsync

Remover código semelhante a:

```csharp
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
await dbContext.Database.EnsureCreatedAsync();
```

Não substituir automaticamente por `MigrateAsync()` sem decisão explícita.

---

## Política sobre MigrateAsync no startup

Por padrão, **não adicionar**:

```csharp
await dbContext.Database.MigrateAsync();
```

Motivos:

- Pode executar alterações de schema sem controle.
- Pode gerar risco em ambientes compartilhados.
- Em produção, migration deve fazer parte do processo de deploy.

Exceção aceitável:

- Projeto de estudo.
- Ambiente local.
- Usuário pediu explicitamente.
- Código protegido por `Development`.

Exemplo aceitável apenas com decisão explícita:

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.MigrateAsync();
}
```

Se usar isso, documentar no README.

---

## Diretório padrão

Criar migrations em:

```txt
src/AuthService.Infrastructure/Persistence/Migrations/
```

---

## Comandos recomendados

### Criar migration

Linux/macOS:

```bash
dotnet ef migrations add InitialCreate \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj \
  --output-dir Persistence/Migrations
```

Windows PowerShell:

```powershell
dotnet ef migrations add InitialCreate `
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj `
  --startup-project src/AuthService.Api/AuthService.Api.csproj `
  --output-dir Persistence/Migrations
```

### Aplicar migration

Linux/macOS:

```bash
dotnet ef database update \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj
```

Windows PowerShell:

```powershell
dotnet ef database update `
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj `
  --startup-project src/AuthService.Api/AuthService.Api.csproj
```

### Listar migrations

```bash
dotnet ef migrations list \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj
```

### Remover última migration ainda não aplicada

```bash
dotnet ef migrations remove \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj
```

Atenção: não remover migration já aplicada em ambiente compartilhado sem avaliação.

---

## Nomes de migrations

Usar nomes claros:

```txt
InitialCreate
AddUserRoles
AddRefreshTokenHash
AddPasswordResetToken
AddAuditColumns
```

Evitar:

```txt
Migration1
UpdateDb
Fix
Alteracao
NovaMigration
```

---

## Migration inicial

Ao criar `InitialCreate`:

- Confirmar que não existem migrations anteriores.
- Não apagar migrations existentes.
- Validar se tabelas representam entidades atuais.
- Verificar índices únicos.
- Verificar relacionamentos.
- Verificar `nullable: false` em campos obrigatórios.
- Verificar many-to-many, se existir.
- Verificar delete behavior.

---

## Revisão obrigatória da migration

Depois de gerar, revisar:

- Nome das tabelas.
- Nome das colunas.
- Tipos de colunas.
- Tamanho máximo de strings.
- Nullability.
- Índices.
- Foreign keys.
- Delete behavior.
- Operações destrutivas.

Operações perigosas:

```csharp
migrationBuilder.DropTable(...)
migrationBuilder.DropColumn(...)
migrationBuilder.AlterColumn(...)
migrationBuilder.Sql(...)
```

Se houver operação destrutiva, parar e avisar o usuário antes de prosseguir.

---

## Connection string

Aceitável para ambiente local:

```json
{
  "ConnectionStrings": {
    "AuthDb": "Host=localhost;Port=5432;Database=auth_service;Username=postgres"
  }
}
```

Se houver senha, preferir:

- user-secrets
- variável de ambiente
- vault
- secret manager

Não salvar senha real em repositório público.

---

## Ajustes no Program.cs

Permitido:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
```

Remover:

```csharp
await dbContext.Database.EnsureCreatedAsync();
```

Evitar:

- lógica customizada de criação de tabelas;
- acesso direto a migration em controller;
- lógica pesada de banco no startup.

---

## Ajustes na Infrastructure

A configuração do provider deve ficar na Infrastructure.

PostgreSQL:

```csharp
services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
```

SQL Server:

```csharp
services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
```

---

## Design-time DbContext Factory

Só criar se o `dotnet ef` não conseguir criar o `DbContext`.

Arquivo sugerido:

```txt
src/AuthService.Infrastructure/Persistence/AuthDbContextFactory.cs
```

Exemplo PostgreSQL:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthService.Infrastructure.Persistence;

public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();

        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=auth_service;Username=postgres");

        return new AuthDbContext(optionsBuilder.Options);
    }
}
```

Regras:

- Usar apenas se necessário.
- Não colocar senha real.
- Preferir `--startup-project` antes de criar factory.
- Documentar no README se criada.

---

## README obrigatório

Atualizar README com:

### Pré-requisitos

```txt
.NET SDK 8
dotnet-ef
PostgreSQL ou SQL Server
```

### Instalar/atualizar dotnet-ef

```bash
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet ef --version
```

### Criar migration

```bash
dotnet ef migrations add InitialCreate \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj \
  --output-dir Persistence/Migrations
```

### Aplicar migration

```bash
dotnet ef database update \
  --project src/AuthService.Infrastructure/AuthService.Infrastructure.csproj \
  --startup-project src/AuthService.Api/AuthService.Api.csproj
```

### Rodar aplicação

```bash
dotnet run --project src/AuthService.Api/AuthService.Api.csproj
```

Remover qualquer frase dizendo que o schema é criado automaticamente via `EnsureCreatedAsync()`, caso isso não seja mais verdade.

---

## Critérios de aceite

A tarefa só está concluída quando:

- `EnsureCreatedAsync()` foi removido, se existia.
- Projeto está configurado para EF Core Migrations.
- Migration inicial foi criada, se ainda não existia.
- Migrations estão na Infrastructure.
- README foi atualizado.
- `dotnet build` foi executado.
- `dotnet test` foi executado, se houver testes.
- Nenhum segredo real foi adicionado.
- Nenhuma migration destrutiva foi criada sem alerta.
- Domain continua sem EF Core.
- Controllers continuam sem acesso direto a `DbContext`.

---

## Resposta final esperada do Codex

Ao finalizar, responder neste formato:

```txt
Arquivos alterados:
- ...

Comandos executados:
- ...

Resultado:
- dotnet build: sucesso/falha
- dotnet test: sucesso/falha
- dotnet ef migrations add: sucesso/falha
- dotnet ef database update: sucesso/falha, se executado

Observações:
- ...

Próximos passos:
- ...
```

---

## Prompt recomendado para Codex

```txt
Use as instruções do AGENTS.md, da skill .skills/dotnet-8-auth-microservice/SKILL.md e da skill .skills/dotnet-efcore-migrations/SKILL.md.

Objetivo:
Substituir EnsureCreatedAsync por EF Core Migrations neste projeto .NET 8.

Tarefas:
1. Verifique onde o AuthDbContext está configurado.
2. Verifique se Program.cs usa EnsureCreatedAsync.
3. Remova EnsureCreatedAsync.
4. Verifique se os pacotes de EF Core Design estão corretos.
5. Crie a migration inicial em src/AuthService.Infrastructure/Persistence/Migrations.
6. Não use MigrateAsync no startup, a menos que já exista decisão explícita.
7. Atualize o README com comandos para criar e aplicar migrations.
8. Execute dotnet build.
9. Execute dotnet test.

Regras:
- Não alterar regras de domínio.
- Não alterar endpoints.
- Não recriar arquitetura.
- Não apagar migrations existentes.
- Não adicionar segredo real ao appsettings.
- Não finalizar sem informar arquivos alterados e comandos executados.
```

---

## Prompt para diagnóstico sem alteração

```txt
Use as instruções do AGENTS.md e da skill .skills/dotnet-efcore-migrations/SKILL.md.

Revise a configuração atual de EF Core Migrations.

Verifique:
- Se o projeto usa EnsureCreatedAsync.
- Se existem migrations.
- Se as migrations estão na Infrastructure.
- Se existe migration destrutiva.
- Se o README está coerente com o fluxo real.
- Se a connection string está segura para repositório público.
- Se o Domain continua sem dependência de EF Core.

Não faça alterações ainda.
Primeiro apresente o diagnóstico e uma ordem de correção.
```

---

## Prompt para migration futura

```txt
Use as instruções do AGENTS.md e da skill .skills/dotnet-efcore-migrations/SKILL.md.

Crie uma nova migration para refletir as alterações recentes no modelo de persistência.

Antes de criar:
- Verifique alterações nas entidades e mappings.
- Verifique se não existe migration pendente.
- Defina um nome claro para a migration.

Depois de criar:
- Revise o código gerado.
- Alerte se houver DropTable, DropColumn ou alteração destrutiva.
- Execute dotnet build e dotnet test.
- Atualize o README se necessário.
```

---

## Anti-patterns

Evitar:

- `EnsureCreatedAsync()` em projeto com migrations.
- Aplicar migrations automaticamente em produção no startup.
- Criar migration sem revisar o conteúdo.
- Gerar migrations no projeto Api.
- Colocar migrations em Domain ou Application.
- Colocar connection string com senha real no repositório.
- Apagar pasta de migrations para resolver conflito.
- Misturar mudança de schema com mudança grande de regra.
- Criar migration com nome genérico.
- Finalizar sem executar build.
