### **Desafio Técnico - Desenvolvedor C# Pleno - Scraping com Integração API**

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![HtmlAgilityPack](https://img.shields.io/badge/HtmlAgilityPack-blue?style=for-the-badge)

#### **1. Contexto do Desafio**

Este projeto foi desenvolvido como parte de um desafio técnico para a vaga de Desenvolvedor C# Pleno. A aplicação simula um processo comum na UneCont, onde dados estruturados são extraídos de fontes diversas, como portais de notas fiscais, e integrados a sistemas via API.

O objetivo é extrair dados de livros do site `https://books.toscrape.com/`, aplicar filtros configuráveis e, em seguida, exportar os dados em formatos JSON e XML. Por fim, os dados são enviados para um endpoint de API simulado.

#### **2. Arquitetura da Solução**

A solução segue os princípios da **Arquitetura Limpa (Clean Architecture)** e **SOLID**, garantindo que o código seja modular, desacoplado, testável e de fácil manutenção. O projeto está organizado em quatro camadas:

* **`Domain`**: O núcleo da aplicação. Contém as entidades de negócio (`Book`, `ScrapingConfig`) e as interfaces que definem as operações (`IScrapingService`, `IExportService`, `IApiService`).
* **`Application`**: Orquestra os casos de uso. A classe `ScrapingApplication` coordena o fluxo de trabalho, chamando os serviços para extrair, filtrar, exportar e enviar os dados.
* **`Infrastructure`**: Contém as implementações concretas das interfaces do `Domain`, lidando com os detalhes técnicos. Aqui estão as classes que usam `HttpClient` para requisições web, `HtmlAgilityPack` para parsing de HTML e as bibliotecas de serialização para JSON e XML.
* **`Presentation`**: O ponto de entrada da aplicação (`Program.cs`). Responsável por configurar a injeção de dependência e inicializar o fluxo de execução.

#### **3. Requisitos Técnicos**

* **Linguagem**: C#
* **Framework**: .NET 9.0 SDK
* **Parsing HTML**: `HtmlAgilityPack` 
* **Serialização**: JSON e XML
* **Configuração**: `appsettings.json` para filtros
* **Integração**: `HttpClient` para comunicação com API REST
* **Boas Práticas**: Tratamento de erros e logging mínimo, documentação clara no `README.md`

#### **4. Passo a Passo para Configuração e Execução**

1.  **Pré-requisitos**:
    * Garanta que o `.NET 9.0 SDK` esteja instalado.
    * Um editor de código como Visual Studio 2022 ou VS Code.
    * Crie uma pasta para o projeto em um local de fácil acesso, como `C:\`.

2.  **Clonar o Repositório**:
    ```bash
    git clone [https://github.com/alefsilvaf/desafio-unecont.git](https://github.com/alefsilvaf/desafio-unecont.git)
    cd desafio-unecont
    ```

3.  **Restaurar Pacotes NuGet**:
    Este comando baixa todas as dependências do projeto.
    ```bash
    dotnet restore
    ```

4.  **Configuração dos Filtros**:
    A aplicação utiliza valores padrão do arquivo `appsettings.json`, mas permite que o usuário sobrescreva esses valores via terminal. Para um filtro ser ignorado, basta deixar a entrada em branco e pressionar `Enter` quando o prompt aparecer.
    * **Categorias**: A aplicação requer que pelo menos uma categoria seja informada. O valor padrão do `appsettings.json` será utilizado se a entrada for vazia.
    * **Preço e Estrelas**: A entrada do usuário é opcional. Se a entrada for vazia, o filtro correspondente não será aplicado, trazendo todos os livros da categoria.
    ```json
    {
      "ScrapingConfig": {
        "Categories": [ "travel", "mystery", "historical-fiction" ],
        "PriceFilter": {
          "MinPrice": 15.00,
          "MaxPrice": 40.00
        },
        "RatingFilter": 4
      }
    }
    ```

5.  **Execução da Aplicação**:
    No terminal, a partir da raiz do projeto, execute o seguinte comando:
    ```bash
    dotnet run
    ```
    A aplicação irá:
    * Prompt para o usuário inserir os valores dos filtros.
    * Fazer o scraping das categorias configuradas.
    * Aplicar os filtros.
    * Gerar os arquivos `books.json` (formato JSON indentado) e `books.xml` (estrutura equivalente) no diretório de saída.
    * Enviar o conteúdo de `books.json` via POST para `https://httpbin.org/post` ou mock equivalente.
    * Exibir no console o status da requisição e um resumo do que foi enviado.