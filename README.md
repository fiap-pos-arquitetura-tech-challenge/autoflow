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
    ├── AutoFlow.UnitTests/         # Testes unitários (domínio, validadores, serviços)
    └── AutoFlow.IntegrationTests/  # Testes de integração dos endpoints (WebApplicationFactory + SQLite em memória)
```

### Domínio

- **Cliente** — cadastro de clientes da oficina (documento CPF/CNPJ, e-mail, telefone).
- **Veículo** — veículos vinculados a um cliente (placa, chassi, quilometragem, combustível, tipo).
- **Serviço** — serviços oferecidos pela oficina (nome único, preço, tempo médio de execução).
- **Peça/Insumo** — peças e insumos utilizados nos serviços (nome, valor); ao ser cadastrada, gera
  automaticamente um **Estoque** associado com quantidade zero.
- **Estoque** — controla a quantidade de uma peça/insumo, com movimentações de entrada, saída
  (validando disponibilidade) e ajuste (definição direta da quantidade).
- **Usuario** — conta de acesso com perfil `Colaborador` (equipe da oficina) ou `Cliente`
  (vinculado a um `Cliente` existente), usada para autenticação.
- **Ordem de Serviço** — controla o fluxo completo da recepção à entrega do veículo, com diagnóstico,
  serviços e peças em snapshot, orçamento, aprovação/reprovação pelo cliente, execução e histórico de datas.

## Arquitetura e decisões técnicas

### Por que arquitetura em camadas

A solução segue a regra de dependência da Clean Architecture: as setas de
referência de projeto sempre apontam para dentro, em direção ao domínio.

```text
AutoFlow.Api  →  AutoFlow.Application  →  AutoFlow.Domain
                          ↑
              AutoFlow.Infrastructure
```

- **Domain** não referencia nenhum outro projeto — concentra entidades, value
  objects e regras de negócio puras, sem dependência de EF Core, ASP.NET ou
  qualquer detalhe de infraestrutura.
- **Application** referencia apenas o Domain — define casos de uso, DTOs e
  interfaces (`IXxxRepositorio`, `IXxxService`) que a Infrastructure implementa,
  seguindo o Dependency Inversion Principle.
- **Infrastructure** implementa as interfaces da Application (EF Core,
  repositórios, geração de token) e pode ser trocada (ex.: outro banco, outro
  provedor de token) sem alterar regra de negócio.
- **Api** é a camada mais externa, responsável por expor os casos de uso via
  Minimal APIs.

O objetivo é manter o domínio testável e isolado de detalhes de framework, e
permitir que decisões de infraestrutura (banco, autenticação) mudem com o
menor impacto possível no restante do código. Optou-se por **Minimal APIs**
em vez de Controllers por serem mais enxutas e alinhadas à direção atual do
ASP.NET Core para APIs simples.

### Por que JWT customizado em vez de ASP.NET Core Identity

A autenticação foi implementada com entidades próprias (`Usuario`/`Perfil`) e
`Microsoft.AspNetCore.Authentication.JwtBearer`, em vez do ASP.NET Core
Identity. O Identity adiciona ~7 tabelas próprias (`AspNetUsers`,
`AspNetRoles`, etc.), exige que o `DbContext` herde de `IdentityDbContext` e,
mesmo assim, não emite JWT sozinho — ainda seria necessário o JwtBearer por
cima para uma API. Como o projeto começou do zero em autenticação e a
necessidade era apenas login + autorização por perfil (sem reset de senha,
lockout ou 2FA), optou-se pelo caminho mais enxuto, consistente com o padrão
de entidades (`BaseModel`) já usado no domínio.

### Por que o padrão Result para erros de negócio

Os serviços de Application retornam `Result`/`Result<T>` (com `ErrorType`:
`Validation`, `NotFound`, `Conflict`, `Unauthorized`) em vez de lançar exceção
para falhas esperadas (regra de negócio violada, recurso não encontrado,
duplicidade). A camada Api traduz esse resultado em HTTP via
[ResultExtensions](src/backend/src/AutoFlow.Api/Extensions/ResultExtensions.cs),
mantendo o fluxo de erro previsível como parte da assinatura do método, sem o
custo de exceções para controle de fluxo. Já invariantes do próprio domínio
(ex.: CPF inválido, placa inválida) continuam sendo `DomainException`,
tratadas globalmente pelo
[GlobalExceptionHandler](src/backend/src/AutoFlow.Api/Middlewares/GlobalExceptionHandler.cs).

### Por que SQLite em memória nos testes de integração

Os testes de integração sobem a API completa via `WebApplicationFactory`,
mas substituem o SQL Server por **SQLite em memória**. Isso mantém os testes
rápidos, isolados e sem exigir um SQL Server disponível para rodar `dotnet
test` (inclusive em CI), enquanto a aplicação em execução real continua usando
SQL Server — a troca de provider é possível justamente porque a Infrastructure
está isolada atrás de interfaces da Application.

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
migrations pendentes automaticamente e, se não houver nenhum colaborador
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
3. Todos os endpoints administrativos (`Clientes`, `Veiculos`, `Servicos`,
   `PecasInsumos`, `Estoques` e a criação de colaboradores) exigem o perfil
   `Colaborador`; a ativação de acesso do cliente
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
| GET    | `/api/servicos`              | `Colaborador`           | Lista serviços                               |
| GET    | `/api/servicos/{id}`         | `Colaborador`           | Obtém um serviço por id                      |
| GET    | `/api/servicos/nome/{nome}`  | `Colaborador`           | Obtém um serviço pelo nome                   |
| POST   | `/api/servicos`              | `Colaborador`           | Cadastra um serviço (nome único)             |
| PUT    | `/api/servicos/{id}`         | `Colaborador`           | Atualiza um serviço                          |
| DELETE | `/api/servicos/{id}`         | `Colaborador`           | Exclui um serviço                            |
| GET    | `/api/pecasInsumos`          | `Colaborador`           | Lista peças/insumos                          |
| GET    | `/api/pecasInsumos/{id}`     | `Colaborador`           | Obtém uma peça/insumo por id                 |
| POST   | `/api/pecasInsumos`          | `Colaborador`           | Cadastra uma peça/insumo (cria estoque zerado) |
| PUT    | `/api/pecasInsumos/{id}`     | `Colaborador`           | Atualiza uma peça/insumo                     |
| DELETE | `/api/pecasInsumos/{id}`     | `Colaborador`           | Exclui uma peça/insumo                       |
| GET    | `/api/estoques/{id}`         | `Colaborador`           | Obtém um estoque por id                      |
| GET    | `/api/estoques/peca/{pecaInsumoId}` | `Colaborador`    | Obtém o estoque de uma peça/insumo           |
| POST   | `/api/estoques/{id}/entrada` | `Colaborador`           | Registra entrada de quantidade no estoque    |
| POST   | `/api/estoques/{id}/saida`   | `Colaborador`           | Registra saída de quantidade do estoque      |
| PUT    | `/api/estoques/{id}/ajustar` | `Colaborador`           | Ajusta a quantidade do estoque diretamente   |
| POST   | `/api/ordens-servico` | `Colaborador` | Cria uma ordem de serviço |
| GET    | `/api/ordens-servico` | `Colaborador` | Lista ordens de serviço |
| GET    | `/api/ordens-servico/{id}` | `Colaborador` | Obtém os detalhes completos da OS |
| PUT    | `/api/ordens-servico/{id}/avarias` | `Colaborador` | Registra/atualiza avarias observadas |
| POST   | `/api/ordens-servico/{id}/diagnostico/iniciar` | `Colaborador` | Inicia o diagnóstico |
| PUT    | `/api/ordens-servico/{id}/diagnostico` | `Colaborador` | Registra o diagnóstico |
| POST   | `/api/ordens-servico/{id}/servicos` | `Colaborador` | Adiciona serviço à OS |
| DELETE | `/api/ordens-servico/{id}/servicos/{servicoId}` | `Colaborador` | Remove serviço da OS |
| POST   | `/api/ordens-servico/{id}/pecas` | `Colaborador` | Adiciona peça/insumo à OS, validando estoque |
| DELETE | `/api/ordens-servico/{id}/pecas/{pecaId}` | `Colaborador` | Remove peça/insumo da OS |
| POST   | `/api/ordens-servico/{id}/orcamento` | `Colaborador` | Gera o orçamento da OS |
| GET    | `/api/ordens-servico/{id}/orcamento` | `Cliente` (dono da OS) | Consulta itens e valores do orçamento |
| POST   | `/api/ordens-servico/{id}/orcamento/aprovar` | `Cliente` (dono da OS) | Aprova o orçamento e inicia a execução |
| POST   | `/api/ordens-servico/{id}/orcamento/reprovar` | `Cliente` (dono da OS) | Reprova o orçamento com justificativa |
| POST   | `/api/ordens-servico/{id}/servicos/{itemServicoId}/execucao/iniciar` | `Colaborador` | Inicia a execução de um serviço |
| POST   | `/api/ordens-servico/{id}/servicos/{itemServicoId}/execucao/finalizar` | `Colaborador` | Finaliza a execução de um serviço |
| POST   | `/api/ordens-servico/{id}/finalizar` | `Colaborador` | Finaliza a OS após concluir todos os serviços |
| POST   | `/api/ordens-servico/{id}/entregar` | `Colaborador` | Registra a entrega do veículo |
| GET    | `/api/ordens-servico/{id}/andamento` | `Cliente` dono / `Colaborador` | Consulta o andamento resumido da OS |

### Fluxo da Ordem de Serviço

O fluxo principal de status é:

```text
Recebida → EmDiagnostico → AguardandoAprovacao → EmExecucao → Finalizada → Entregue
```

O colaborador prepara diagnóstico, serviços, peças e orçamento. A aprovação ou reprovação do orçamento
é feita pelo cliente autenticado e somente para uma OS vinculada ao seu `ClienteId` no token JWT. Na aprovação,
as peças utilizadas são baixadas do estoque. A OS só pode ser finalizada quando todos os seus serviços estiverem
com a execução concluída.

## Executando os testes

```bash
cd src/backend
dotnet test
```

O projeto [AutoFlow.UnitTests](src/backend/tests/AutoFlow.UnitTests) reúne os
testes unitários (domínio, validadores, serviços). O projeto
[AutoFlow.IntegrationTests](src/backend/tests/AutoFlow.IntegrationTests) cobre
os endpoints da API (incluindo autenticação e autorização por perfil),
executados contra um `WebApplicationFactory` com SQLite em memória.

Para rodar apenas um dos projetos:

```bash
dotnet test tests/AutoFlow.UnitTests/AutoFlow.UnitTests.csproj
dotnet test tests/AutoFlow.IntegrationTests/AutoFlow.IntegrationTests.csproj
```

## Análise de Qualidade com SonarQube

O projeto utiliza o SonarQube para análise estática de código, identificação de vulnerabilidades, code smells, bugs e acompanhamento da cobertura dos testes automatizados.

### Pré-requisitos

Instale as ferramentas abaixo:

#### SonarScanner para .NET

```bash
dotnet tool install --global dotnet-sonarscanner
```

#### Dotnet Coverage

```bash
dotnet tool install --global dotnet-coverage
```

Verifique a instalação:

```bash
dotnet sonarscanner --version
dotnet-coverage --version
```

---

### Executando o SonarQube

O SonarQube está disponível através do Docker Compose do projeto.

Subir apenas o serviço do SonarQube:

```bash
docker compose up -d sonarqube
```

Verificar o status:

```bash
docker compose ps
```

Visualizar logs:

```bash
docker compose logs -f sonarqube
```

Acesse:

```text
http://localhost:9000
```

No primeiro acesso:

- Usuário: `admin`
- Senha: `admin`

Após o login:

1. Crie um projeto no SonarQube.
2. Defina a chave do projeto (Project Key).
3. Gere um token em:

```text
My Account → Security → Generate Tokens
```

---

## Executando uma análise completa

Na raiz da solução:

```bash
cd src/backend
```

### 1. Iniciar o scanner

```powershell
dotnet sonarscanner begin `
  /k:"AutoFlow" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.token="SEU_TOKEN" `
  /d:sonar.cs.vscoveragexml.reportsPaths="coverage.xml"
```

### 2. Compilar a aplicação

```bash
dotnet build AutoFlow.slnx --no-incremental
```

### 3. Executar os testes e gerar cobertura

```bash
dotnet-coverage collect "dotnet test --solution AutoFlow.slnx" -f xml -o coverage.xml
```

Esse comando executa os testes e gera o arquivo:

```text
coverage.xml
```

### 4. Finalizar a análise

```powershell
dotnet sonarscanner end `
  /d:sonar.token="SEU_TOKEN"
```

---

## Fluxo da análise

```text
SonarScanner Begin
        ↓
dotnet build
        ↓
dotnet test + dotnet-coverage
        ↓
coverage.xml
        ↓
SonarScanner End
        ↓
SonarQube
```

---

## Métricas analisadas

O SonarQube avalia automaticamente:

- Bugs
- Vulnerabilidades
- Security Hotspots
- Code Smells
- Cobertura de testes
- Duplicação de código
- Confiabilidade (Reliability)
- Manutenibilidade (Maintainability)
- Segurança (Security)
- Quality Gate

---

## Cobertura de testes

A cobertura é gerada através do `dotnet-coverage` e importada automaticamente pelo SonarQube.

O relatório é salvo em:

```text
coverage.xml
```

Após a análise, a cobertura pode ser consultada diretamente no dashboard do SonarQube.

---

## Exemplo de resultado

```text
Quality Gate: PASSED

Security: B
Reliability: C
Maintainability: A

Coverage: 82,8%
Duplications: 0,7%
```


## Gerando um relatório HTML do SonarQube

Além do dashboard do SonarQube, o script
[gerar_relatorio_sonarqube.py](src/backend/gerar_relatorio_sonarqube.py) gera um
relatório HTML autocontido (resumo de métricas, ratings e lista de issues
abertos) a partir da Web API do SonarQube, útil para anexar em entregas ou
compartilhar sem precisar de acesso ao SonarQube.

### Pré-requisitos

- Python 3
- Dependências:

```bash
pip install requests python-dotenv
```

### Configuração

O script lê `SONAR_URL` e `SONAR_TOKEN` de variáveis de ambiente ou de um
arquivo `.env` na pasta `src/backend`. Copie o exemplo e preencha o token:

```bash
cd src/backend
cp .env.example .env
```

```text
SONAR_URL=http://localhost:9000
SONAR_TOKEN=SEU_TOKEN
```

> **Importante:** o token precisa ser do tipo **User Token** (gerado em
> `My Account → Security → Generate Tokens`, opção *User Token*). Tokens do
> tipo *Global Analysis Token* ou *Project Analysis Token* servem apenas para
> o scanner enviar análises e retornam `403 Insufficient privileges` ao
> consultar a API.

Se `SONAR_URL` não for definida, o padrão é `http://localhost:9000`.

### Uso

Com o SonarQube em execução e o projeto já analisado (ver
[Executando uma análise completa](#executando-uma-análise-completa)):

```bash
cd src/backend
python gerar_relatorio_sonarqube.py <project_key>
```

Por exemplo, para o projeto configurado como `AutoFlow`:

```bash
python gerar_relatorio_sonarqube.py AutoFlow
```

O relatório é salvo na pasta atual como `relatorio-sonarqube-<project_key>.html`.

## Stack

- .NET 10 / ASP.NET Core (Minimal APIs)
- Entity Framework Core + SQL Server (containerizado via Docker Compose)
- Autenticação/autorização via JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- Scalar (documentação OpenAPI)
- xUnit + SQLite in-memory (testes unitários e de integração)
- Docker / Docker Compose
