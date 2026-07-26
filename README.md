# AutoFlow — Gestão de Oficina Mecânica

Projeto da Pós-Graduação em Arquitetura de Software (FIAP) — Tech Challenge.

Sistema de gestão para oficina mecânica, construído em .NET 10 seguindo uma
arquitetura em camadas (Domain, Application, Infrastructure e Api).

## Estrutura do projeto

```
src/backend/
├── AutoFlow.slnx                  # Solução (.NET)
├── docker-compose.yml             # Orquestração dos containers (API + SQL Server)
├── docker-compose.override.yml    # Configuração local de desenvolvimento
├── src/
│   ├── AutoFlow.Api/              # Camada de apresentação (Web API, endpoints)
│   ├── AutoFlow.Application/      # Casos de uso, regras de aplicação
│   ├── AutoFlow.Domain/           # Entidades e regras de negócio
│   └── AutoFlow.Infrastructure/   # Persistência, integrações externas
└── tests/
    └── AutoFlow.UnitTests/        # Testes unitários (xUnit)
```

## Pré-requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) e Docker Compose (recomendado para subir a API junto com o SQL Server)
- Uma IDE de sua preferência (Visual Studio, VS Code, Rider)

## Como executar

### Opção 1 — Docker Compose (recomendado)

Sobe a API e o SQL Server em containers.

```bash
cd src/backend
docker compose up --build
```

A API ficará disponível em:
- HTTP: `http://localhost:8080`
- HTTPS: `https://localhost:8081`

O SQL Server ficará disponível em `localhost:1433` (usuário `sa`, senha
`YourStrong!Passw0rd`, definidos em [docker-compose.yml](src/backend/docker-compose.yml)).

Para parar e remover os containers:

```bash
docker compose down
```

### Opção 2 — Executar localmente com o .NET CLI

Requer uma instância de SQL Server acessível (pode ser a do próprio Docker Compose,
subindo apenas o serviço `sqlserver`: `docker compose up sqlserver`).

```bash
cd src/backend
dotnet restore
dotnet run --project src/AutoFlow.Api
```

Por padrão a API sobe conforme o `launchSettings.json`/perfil escolhido na IDE.

## Documentação da API

Em ambiente de desenvolvimento, a documentação interativa (Scalar) fica disponível em:

```
{url-base}/scalar/v1
```

O documento OpenAPI cru pode ser obtido em `{url-base}/openapi/v1.json`.

## Executando os testes

```bash
cd src/backend
dotnet test
```

## Stack

- .NET 10 / ASP.NET Core
- SQL Server (containerizado via Docker Compose)
- Scalar (documentação OpenAPI)
- xUnit (testes)
- Docker / Docker Compose
