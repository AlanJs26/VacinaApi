# 📄 Documentação Completa da Vacina API

## 1. 🎯 Introdução e Objetivo do Projeto

Bem-vindo à documentação técnica da **Vacina API**. Esta solução foi desenvolvida para fornecer um sistema de back-end robusto e eficiente para o gerenciamento de registros de vacinação. O objetivo principal é oferecer uma API RESTful simples, intuitiva e bem documentada que permita a criação, consulta, atualização e remoção de dados relacionados a pessoas, vacinas, cartões de vacina e os registros de vacinação em si.

A aplicação foi construída com foco na simplicidade e manutenibilidade, utilizando tecnologias modernas do ecossistema .NET.

---

## 2. 🏛️ Arquitetura e Decisões de Design

### 2.1. Stack de Tecnologias

-   **.NET 10**: A versão mais recente do framework .NET, garantindo performance, segurança e acesso aos recursos mais modernos da plataforma.
-   **ASP.NET Core**: Para a construção da Web API, oferecendo um framework leve e de alto desempenho.
-   **Entity Framework Core**: Utilizado como ORM (Object-Relational Mapper) para a interação com o banco de dados, abstraindo a complexidade do acesso a dados.
-   **SQLite**: Um banco de dados leve e baseado em arquivo, escolhido pela sua simplicidade de configuração e por ser ideal para desenvolvimento e pequenas aplicações.
-   **xUnit**: Framework de testes para garantir a qualidade e a corretude do código.
-   **Docker**: Para a containerização da aplicação, facilitando o deploy e a criação de ambientes de desenvolvimento e produção consistentes.

### 2.2. Estilo Arquitetural

A API segue uma abordagem de **API Minimalista (Minimal API-style)**, embora implementada com controladores. A lógica de negócios está contida diretamente nos actions dos controladores. Esta decisão foi tomada para manter a simplicidade e a clareza do código, o que é ideal para um projeto deste escopo.

-   **Controlador Único (`SistemaController`)**: Toda a lógica da API foi centralizada em um único controlador. Isso simplifica o roteamento e a localização de código, tornando a manutenção mais direta para uma aplicação com um número limitado de entidades.

### 2.3. Persistência de Dados

-   **Code-First com Entity Framework Core**: O banco de dados é modelado a partir das classes C# (POCOs) definidas em `Models/Entities.cs`.
-   **Criação Automática do Banco de Dados**: Em vez de usar migrations, a aplicação utiliza o método `db.Database.EnsureCreated()`. Na primeira vez que a aplicação é executada, o EF Core verifica se o banco de dados (`vacinas.db`) existe e, caso não exista, o cria com base no modelo de dados.
    -   **Decisão**: Esta abordagem foi escolhida para simplificar o setup inicial. A desvantagem é que, ao modificar os modelos, é necessário apagar o arquivo `vacinas.db` manualmente para que o schema seja recriado. Para um ambiente de produção mais complexo, o uso de migrations seria mais apropriado.

---

## 3. 📂 Estrutura de Arquivos e Projetos

A solução está organizada em dois projetos principais:

```
/
├── Vacina.slnx
├── VacinaApi/
│   ├── Controllers/
│   │   └── VacinaController.cs  # O único controlador da API
│   ├── Data/
│   │   └── AppDbContext.cs      # Contexto do Entity Framework
│   ├── Models/
│   │   └── Entities.cs          # As entidades (POCOs)
│   ├── Utils/
│   │   └── Utils.cs             # Funções utilitárias (ex: validação de CPF)
│   ├── Properties/
│   │   └── launchSettings.json  # Configurações de execução local
│   ├── appsettings.json         # Configurações da aplicação
│   ├── Dockerfile               # Instruções para build da imagem Docker
│   └── Program.cs               # Ponto de entrada e configuração da aplicação
│
├── VacinaApi.Tests/
│   ├── PersonTests.cs           # Testes para a entidade Person
│   ├── VaccineCardTests.cs      # Testes para a entidade VaccineCard
│   ├── VaccineRecordTests.cs    # Testes para os registros de vacinação
│   ├── VaccineTests.cs          # Testes para a entidade Vaccine
│   └── UtilsTests.cs            # Testes para as funções utilitárias
│
├── docker-compose.yaml          # Orquestração dos containers
└── README.md                    # Documentação resumida
```

---

## 4. 📦 Dependências e Pacotes (NuGet)

### VacinaApi

-   **`Microsoft.AspNetCore.OpenApi`**: Para integração com o Swagger e geração da documentação da API.
-   **`Microsoft.EntityFrameworkCore.Design`**: Contém as ferramentas de design-time do EF Core.
-   **`Microsoft.EntityFrameworkCore.Sqlite`**: O provedor do EF Core para o banco de dados SQLite.
-   **`Swashbuckle.AspNetCore`**: A biblioteca principal para a geração da interface do Swagger UI.

### VacinaApi.Tests

-   **`Microsoft.NET.Test.Sdk`**: O SDK de testes da Microsoft.
-   **`xunit`** e **`xunit.runner.visualstudio`**: O framework de testes xUnit e seu executor para o Visual Studio.
-   **`coverlet.collector`**: Para a coleta de dados de cobertura de testes.
-   **`Microsoft.EntityFrameworkCore.InMemory`**: Provedor de banco de dados em memória do EF Core, usado para isolar os testes.

---

## 5. 📖 Documentação dos Endpoints da API

Todos os endpoints estão sob o prefixo `/api`.

### 📇 Cartões de Vacina

-   **`GET /cartoes`**: Retorna todos os cartões de vacina.
-   **`POST /cartoes`**: Cria um novo cartão.
    -   **Corpo**: `{ "name": "string" }`
-   **`DELETE /cartoes/{id}`**: Remove um cartão e seus registros associados.

### 🧑 Pessoas

-   **`GET /pessoas`**: Lista pessoas, com filtros opcionais.
    -   **Query Params**: `nome` (string), `cpf` (string).
-   **`POST /pessoas`**: Cadastra uma nova pessoa.
    -   **Corpo**: `{ "name": "string", "cpf": "string" }`
-   **`DELETE /pessoas/{id}`**: Remove uma pessoa e seus registros.
-   **`GET /pessoas/{id}/cartoes`**: Consulta os cartões e registros de uma pessoa.

### 💉 Vacinas (Metadados)

-   **`GET /vacinas`**: Retorna todas as vacinas.
-   **`POST /vacinas`**: Cadastra uma nova vacina.
    -   **Corpo**: `{ "name": "string", "manufacturer": "string" }`
-   **`DELETE /vacinas/{id}`**: Remove uma vacina e seus registros.

### 📝 Registros de Vacinação

-   **`GET /vacinacao`**: Retorna todos os registros de vacinação.
-   **`POST /vacinacao`**: Cria um novo registro.
    -   **Corpo**: `{ "personId": int, "vaccineId": int, "vaccineCardId": int, "dose": int, "applicationDate": "YYYY-MM-DD" }`
-   **`DELETE /vacinacao/{id}`**: Remove um registro de vacinação.

---

## 6. 🧪 Estratégia de Testes

A solução possui uma suíte de testes robusta para garantir a qualidade e a estabilidade do código.

-   **Framework**: Os testes são escritos utilizando **xUnit**.
-   **Isolamento**: Para os testes de integração que envolvem o banco de dados, é utilizado o **provedor em memória do Entity Framework Core**. Isso garante que cada teste execute em um banco de dados limpo e isolado, sem depender de um arquivo físico e sem interferir em outros testes.
-   **Estrutura**: Os testes são categorizados em arquivos separados por funcionalidade (`PersonTests.cs`, `VaccineTests.cs`, etc.), o que torna a suíte de testes organizada e fácil de manter.
-   **Execução**: Para rodar todos os testes, utilize o comando na raiz do projeto:
    ```sh
    dotnet test
    ```

---

## 7. 💻 Desenvolvimento e Execução Local

### Pré-requisitos

-   [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Passos

1.  **Clone o repositório.**
2.  **Restaure as dependências**:
    ```sh
    dotnet restore
    ```
3.  **Execute a aplicação**:
    ```sh
    cd VacinaApi
    dotnet run
    ```
A API estará rodando em `http://localhost:5139`.

### Documentação Interativa (Swagger)

Em modo de desenvolvimento, a documentação da API é gerada automaticamente e pode ser acessada em:
[http://localhost:5139/swagger](http://localhost:5139/swagger)

A interface do Swagger UI permite visualizar todos os endpoints, seus parâmetros e testá-los diretamente do navegador.

---

## 8. 🐳 Deploy com Docker

A aplicação está configurada para ser executada em containers Docker, facilitando o deploy e a consistência entre ambientes.

### 8.1. Dockerfile

O `Dockerfile` é multi-stage, otimizado para segurança e performance:

1.  **`base`**: Imagem base do ASP.NET Core, leve e otimizada para produção.
2.  **`build`**: Imagem do .NET SDK, usada para compilar a aplicação e restaurar dependências.
3.  **`test`**: Uma etapa intermediária que executa os testes. Se os testes falharem, o build da imagem é interrompido.
4.  **`publish`**: Publica a aplicação, gerando os artefatos otimizados para deploy.
5.  **`final`**: A imagem final, que copia apenas os artefatos publicados da etapa `publish` para a imagem `base`, resultando em uma imagem pequena e segura.

### 8.2. Docker Compose

O arquivo `docker-compose.yaml` orquestra os serviços:

-   **`api` (Produção)**: Constrói a imagem a partir do `Dockerfile` e a executa.
    ```sh
    docker-compose up --build
    ```
-   **`api-dev` (Desenvolvimento)**: Monta o código-fonte local como um volume dentro do container e utiliza `dotnet watch` para habilitar o **hot-reloading**.
    ```sh
    docker-compose run --rm --service-ports api-dev
    ```
Isso permite que as alterações no código sejam refletidas instantaneamente no container, agilizando o desenvolvimento.
