# AutoFlow — Gestão de Oficina Mecânica

Projeto da Pós-Graduação em Arquitetura de Software (FIAP) — Tech Challenge.

Sistema de gestão para oficina mecânica, construído em .NET 10 seguindo uma
arquitetura em camadas (Domain, Application, Infrastructure e Api), com
autenticação e autorização via JWT.

## Estrutura do projeto

```
src/backend/
├── AutoFlow.slnx                  # Solução (.NET)
├── docker-compose.yml             # Orquestração dos containers (API + SQL Server)
├── docker-compose.override.yml    # Configuração local de desenvolvimento
├── src/
│   ├── AutoFlow.Api/              # Camada de apresentação (Minimal APIs, endpoints, autenticação JWT)
│   ├── AutoFlow.Application/      # Casos de uso, DTOs, validadores, regras de aplicação
│   ├── AutoFlow.Domain/           # Entidades, value objects e regras de negócio
│   └── AutoFlow.Infrastructure/   # Persistência (EF Core), migrations, repositórios, geração de token
└── tests/
    └── AutoFlow.UnitTests/        # Testes unitários e de integração (xUnit)
```

### Domínio

- **Cliente** — cadastro de clientes da oficina (documento CPF/CNPJ, e-mail, telefone).
- **Veículo** — veículos vinculados a um cliente (placa, chassi, quilometragem, combustível, tipo).
- **Usuario** — conta de acesso com perfil `Colaborador` (equipe da oficina) ou `Cliente`
  (vinculado a um `Cliente` existente), usada para autenticação.

## Pré-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) e Docker Compose (recomendado para subir a API junto com o SQL Server)
- Uma IDE de sua preferência (Visual Studio, VS Code, Rider)

## Configuração

A API exige um segredo JWT (`Jwt:Secret`) para assinar os tokens — sem ele a
aplicação não inicia. As demais chaves (`Jwt:Issuer`, `Jwt:Audience`,
`Jwt:ExpiresInMinutes`) já têm valores padrão em
[appsettings.json](src/backend/src/AutoFlow.Api/appsettings.json).

Ao subir pela primeira vez (ambiente de desenvolvimento), a API aplica as
migrations pendentes automaticamente e, se não houver nenhum usuário
cadastrado, faz o *seed* de um colaborador inicial a partir de
`Bootstrap:Colaborador:Nome/Email/Senha`.

### Opção 1 — Docker Compose

Já vem configurado em [docker-compose.override.yml](src/backend/docker-compose.override.yml)
com um `Jwt__Secret` de desenvolvimento e um colaborador inicial padrão
(`colaborador@autoflow.local` / `DevOnly@123456`), sobrescrevíveis pelas
variáveis de ambiente `AUTOFLOW_BOOTSTRAP_NOME`, `AUTOFLOW_BOOTSTRAP_EMAIL` e
`AUTOFLOW_BOOTSTRAP_SENHA`. Nenhuma ação extra é necessária.

### Opção 2 — Execução local com o .NET CLI

Configure o segredo com `dotnet user-secrets` (não versione segredos em
`appsettings.json`):

```bash
cd src/backend/src/AutoFlow.Api
dotnet user-secrets set "Jwt:Secret" "uma-chave-bem-grande-e-secreta-de-desenvolvimento"
dotnet user-secrets set "Bootstrap:Colaborador:Email" "colaborador@autoflow.local"
dotnet user-secrets set "Bootstrap:Colaborador:Senha" "DevOnly@123456"
```

## Como executar

### Opção 1 — Docker Compose (recomendado)

Sobe a API e o SQL Server em containers.

```bash
cd src/backend
docker compose up --build
```

As portas HTTP (8080) e HTTPS (8081) do container são publicadas em portas
efêmeras do host (não fixas), pois
[docker-compose.override.yml](src/backend/docker-compose.override.yml) não
mapeia uma porta fixa. Para descobrir a porta local atribuída:

```bash
docker compose port autoflow.api 8080
docker compose port autoflow.api 8081
```

O SQL Server ficará disponível em `localhost:1433` (usuário `sa`, senha
`YourStrong!Passw0rd`, definidos em [docker-compose.yml](src/backend/docker-compose.yml)).

Para parar e remover os containers:

```bash
docker compose down
```

### Opção 2 — Executar localmente com o .NET CLI

Requer uma instância de SQL Server acessível (pode ser a do próprio Docker Compose,
subindo apenas o serviço `sqlserver`: `docker compose up sqlserver`) e o
segredo JWT configurado (ver [Configuração](#configuração)).

```bash
cd src/backend
dotnet restore
dotnet run --project src/AutoFlow.Api
```

Por padrão a API sobe conforme o `launchSettings.json`/perfil escolhido na IDE.

## Autenticação

A API usa autenticação via **JWT Bearer**. O fluxo típico é:

1. `POST /api/auth/login` com e-mail e senha retorna um token, sua expiração
   e os dados do usuário (nome, e-mail, perfil e, se `Cliente`, o `ClienteId`
   vinculado).
2. O token é enviado no header `Authorization: Bearer {token}` nas chamadas
   seguintes.
3. Endpoints de `Clientes`, `Veiculos` e criação de colaboradores exigem o
   perfil `Colaborador`; a ativação de acesso do cliente
   (`POST /api/usuarios/clientes`) é pública, usada pelo próprio cliente para
   criar sua conta a partir de um documento já cadastrado.

Na documentação interativa (Scalar) é possível autenticar informando o
Bearer token diretamente na UI.

## Documentação da API

Em ambiente de desenvolvimento, a documentação interativa (Scalar) fica disponível em:

```
{url-base}/scalar/v1
```

O documento OpenAPI cru pode ser obtido em `{url-base}/openapi/v1.json`.

### Principais endpoints

| Método | Rota                        | Autorização           | Descrição                                   |
|--------|-----------------------------|------------------------|----------------------------------------------|
| POST   | `/api/auth/login`           | Anônimo                 | Autentica e retorna o token JWT              |
| POST   | `/api/usuarios`              | `Colaborador`           | Cria um novo usuário colaborador             |
| POST   | `/api/usuarios/clientes`     | Anônimo                 | Ativa o acesso de um cliente já cadastrado   |
| GET    | `/api/clientes`              | `Colaborador`           | Lista clientes                               |
| GET    | `/api/clientes/{id}`         | `Colaborador`           | Obtém um cliente por id                      |
| POST   | `/api/clientes`              | `Colaborador`           | Cadastra um cliente                          |
| PUT    | `/api/clientes/{id}`         | `Colaborador`           | Atualiza um cliente                          |
| DELETE | `/api/clientes/{id}`         | `Colaborador`           | Exclui um cliente                            |
| GET    | `/api/veiculos`              | `Colaborador`           | Lista veículos                               |
| GET    | `/api/veiculos/{id}`         | `Colaborador`           | Obtém um veículo por id                      |
| POST   | `/api/veiculos`              | `Colaborador`           | Cadastra um veículo                          |
| PUT    | `/api/veiculos/{id}`         | `Colaborador`           | Atualiza um veículo                          |
| DELETE | `/api/veiculos/{id}`         | `Colaborador`           | Exclui um veículo                            |

## Executando os testes

```bash
cd src/backend
dotnet test
```

O projeto [AutoFlow.UnitTests](src/backend/tests/AutoFlow.UnitTests) reúne
testes unitários (domínio, validadores, serviços) e testes de integração dos
endpoints, executados contra um `WebApplicationFactory` com SQLite em memória.

## Stack

- .NET 10 / ASP.NET Core (Minimal APIs)
- Entity Framework Core + SQL Server (containerizado via Docker Compose)
- Autenticação/autorização via JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Scalar (documentação OpenAPI)
- xUnit + SQLite in-memory (testes unitários e de integração)
- Docker / Docker Compose
