# 💉 Vacina API

Bem-vindo à Vacina API, uma solução para gerenciamento de registros de vacinação.

## 🏗️ Estrutura do Projeto

A solução é composta por dois projetos principais:

-   **`VacinaApi`**: O projeto principal da API, construído com .NET 10. Ele contém todos os controladores, modelos de dados e a lógica de negócios para gerenciar os registros de vacinação.
-   **`VacinaApi.Tests`**: Um projeto de testes utilizando xUnit para garantir a qualidade e o funcionamento correto da API.

## ⚙️ Instalação

Para instalar as dependências do projeto, navegue até o diretório raiz e execute o seguinte comando:

```sh
dotnet restore
```

## 🚀 Executando a Aplicação

Você pode executar a aplicação de várias maneiras, tanto localmente quanto com Docker.

### Executando Localmente (Sem Docker)

#### Modo de Desenvolvimento

Para executar a API em modo de desenvolvimento, navegue até o diretório `VacinaApi` e execute:

```sh
cd VacinaApi
dotnet run
```

A API estará disponível em `http://localhost:5139/swagger`. Em modo de desenvolvimento, a documentação da API é gerada automaticamente e pode ser acessada via Swagger UI no endereço:

[http://localhost:5139/swagger](http://localhost:5139/swagger)

### Executando com Docker

#### Modo de Produção

Para construir a imagem Docker e iniciar o container em modo de produção, execute o seguinte comando na raiz do projeto:

```sh
docker-compose up --build
```

A API estará disponível em `http://localhost:5139`.

#### Modo de Desenvolvimento (com Hot Reload)

Para executar em modo de desenvolvimento com hot-reloading (a aplicação reinicia automaticamente ao salvar alterações no código), use o seguinte comando:

```sh
docker-compose run --rm --service-ports api-dev
```

## 📖 Documentação da API

A seguir, uma descrição detalhada de cada endpoint disponível na API.

### 📇 Cartões de Vacina

-   **`GET /api/cartoes`**
    -   **Descrição**: Retorna todos os cartões de vacina cadastrados.
    -   **Método**: `GET`

-   **`POST /api/cartoes`**
    -   **Descrição**: Cria um novo cartão de vacina.
    -   **Método**: `POST`
    -   **Corpo da Requisição**: `{"name": "string"}`

-   **`DELETE /api/cartoes/{id}`**
    -   **Descrição**: Remove um cartão de vacina e todos os registros de vacinação associados a ele.
    -   **Método**: `DELETE`
    -   **Parâmetros**: `id` (integer) - O ID do cartão a ser removido.

### 🧑 Pessoas

-   **`GET /api/pessoas`**
    -   **Descrição**: Retorna uma lista de pessoas. Pode ser filtrada por nome e/ou CPF.
    -   **Método**: `GET`
    -   **Query Params**: `nome` (string, opcional), `cpf` (string, opcional).

-   **`POST /api/pessoas`**
    -   **Descrição**: Cadastra uma nova pessoa.
    -   **Método**: `POST`
    -   **Corpo da Requisição**: `{"name": "string", "cpf": "string"}`

-   **`DELETE /api/pessoas/{id}`**
    -   **Descrição**: Remove uma pessoa e todos os seus registros de vacinação.
    -   **Método**: `DELETE`
    -   **Parâmetros**: `id` (integer) - O ID da pessoa a ser removida.

-   **`GET /api/pessoas/{id}/cartoes`**
    -   **Descrição**: Consulta todos os cartões e registros de vacinação de uma pessoa específica.
    -   **Método**: `GET`
    -   **Parâmetros**: `id` (integer) - O ID da pessoa.

### 💉 Vacinas (Metadados)

-   **`GET /api/vacinas`**
    -   **Descrição**: Retorna a lista de todas as vacinas (metadados) disponíveis.
    -   **Método**: `GET`

-   **`POST /api/vacinas`**
    -   **Descrição**: Cadastra uma nova vacina (metadado).
    -   **Método**: `POST`
    -   **Corpo da Requisição**: `{"name": "string", "manufacturer": "string"}`

-   **`DELETE /api/vacinas/{id}`**
    -   **Descrição**: Remove uma vacina (metadado) e todos os registros de vacinação associados a ela.
    -   **Método**: `DELETE`
    -   **Parâmetros**: `id` (integer) - O ID da vacina a ser removida.

### 📝 Registros de Vacinação

-   **`GET /api/vacinacao`**
    -   **Descrição**: Retorna todos os registros de vacinação.
    -   **Método**: `GET`

-   **`POST /api/vacinacao`**
    -   **Descrição**: Cria um novo registro de vacinação, associando uma pessoa, uma vacina e um cartão.
    -   **Método**: `POST`
    -   **Corpo da Requisição**: `{"personId": integer, "vaccineId": integer, "vaccineCardId": integer, "dose": integer, "applicationDate": "YYYY-MM-DD"}`

-   **`DELETE /api/vacinacao/{id}`**
    -   **Descrição**: Remove um registro de vacinação específico.
    -   **Método**: `DELETE`
    -   **Parâmetros**: `id` (integer) - O ID do registro a ser removido.
