# Teste Técnico — Paginação de Todo List

## Descrição

Este repositório contém a implementação de melhorias em um sistema de lista de tarefas (*Todo List*), desenvolvido como parte de um teste técnico.

O objetivo foi implementar um sistema de paginação integrado entre o front-end e o back-end, permitindo consultar, ordenar e navegar pelos registros de maneira organizada, além de melhorar a separação de responsabilidades e a confiabilidade das requisições.

O projeto utiliza **AngularJS no front-end** e **C# com ASP.NET Web API 2 no back-end**, utilizando o .NET Framework 4.6.

## Funcionalidades implementadas

### Front-end — AngularJS

- **Paginação:** navegação entre páginas com controle da quantidade de registros exibidos.
- **Controle de quantidade:** seleção do número de itens por página.
- **Navegação:** botões para acessar a primeira página, a anterior, a próxima e a última página.
- **Ordenação:** organização dos registros por propriedades permitidas, em ordem crescente ou decrescente.
- **Estados da interface:** tratamento de carregamento e ausência de registros.
- **Controle de requisições:** utilização de um contador para desconsiderar respostas antigas quando uma requisição mais recente já foi realizada.

### Back-end — C# e ASP.NET Web API 2

- **API paginada:** recebimento dos parâmetros de página, quantidade de registros e ordenação.
- **Separação de responsabilidades:** divisão entre Controller, Service e Repository.
- **DTOs:** utilização de `PageRequest` e `PagedResult` para organizar os dados de entrada e saída.
- **Contagem de registros:** utilização de `Count()` para obter a quantidade total de itens.
- **Paginação:** aplicação de `Skip()` e `Take()` para selecionar os registros da página solicitada.
- **Ordenação segura:** validação das propriedades permitidas antes de ordenar os registros.
- **Ordenação determinística:** utilização de `ThenBy(Id)` como critério secundário para manter uma ordem consistente em caso de valores iguais.
- **Validação da página:** tratamento de números de página inválidos ou superiores ao total disponível.
- **Tratamento de requisições:** retorno de respostas HTTP apropriadas para solicitações válidas ou inválidas.
---

## Decisões técnicas

### Por que utilizar `PageRequest` e `PagedResult<T>`?
O `PageRequest` representa os parâmetros enviados para a consulta, como Página solicitada, quantidade de registros por página, propriedade utilizada para ordenação, direção da ordenação.
O `PagedResult` representa o resultado da consulta, por exemplo:  página atual, o tamanho da página, a quantidade total de registros e o total de páginas.

### Por que separar `Count()` e `List()`?
- **`Count()`:** obtém a quantidade total de registros, permitindo calcular quantas páginas existem.
- **`List()`:** retorna os registros solicitados, aplicando os critérios de ordenação e paginação.

### Por que utilizar `Skip()` e `Take()`?
Esses métodos permitem selecionar um intervalo de registros sem precisar retornar a lista inteira, por exemplo para acessar a segunda página de uma lista com 10 itens por página:

```
Skip(10).Take(10)
```
O sistema ignora os primeiros 10 registros e retorna os próximos 10.

## Bugs identificados e corrigidos

Durante a análise do projeto, foram identificados e corrigidos problemas que afetavam o funcionamento da aplicação:

- **Conflito de rotas na API:** ajuste do roteamento para garantir que as requisições chegassem ao endpoint correto.
- **Compatibilidade com JavaScript legado:** substituição de uma arrow function por uma função compatível com o ambiente utilizado.
- **Identificação dos elementos na lista:** substituição de `track by $index` por `track by todo.id`, permitindo que o AngularJS acompanhe os registros por seus identificadores.
- **Arquivo de teste não incluído no projeto:** correção da configuração do `.csproj` para permitir que o arquivo de testes fosse compilado e descoberto pelo ambiente de testes.

## Tempo dedicado ao projeto

- **Quinta-feira (22/10): 20:00 às 22:00** — Análise inicial do projeto e início das pesquisas sobre as tecnologias utilizadas.
- **Sexta-feira (23/10): 18:30 às 23:00** — Estudo dos principais conceitos de C# e ASP.NET Web API, análise da arquitetura existente e início da implementação das funcionalidades de paginação.
- **Sábado (24/10): 15:00 às 23:00** — Implementação e ajustes da paginação no back-end, incluindo a separação de responsabilidades entre Controller, Service e Repository, além do cálculo do total de páginas e da seleção dos registros.
- **Domingo (25/10): 09:00 ás 15:00** — Desenvolvimento e ajustes da integração entre o front-end AngularJS e a API, tratamento da ordenação e revisão do comportamento da navegação entre páginas.
- **Domingo (25/10): 19:00 às 22:30** — Investigação e correção de bugs, revisão da implementação, análise dos testes automatizados e elaboração da documentação do projeto.

## Estrutura do projeto

```text
InterviewTestPagination/
├── App_Start/
│   └── WebApiConfig.cs
├── Controllers/
│   └── TodoController.cs
├── Models/
│   ├── IModelRepository.cs
│   ├── IModelService.cs
│   ├── PageRequest.cs
│   ├── PagedResult.cs
│   └── Todo/
│       ├── Todo.cs
│       ├── TodoRepository.cs
│       └── TodoService.cs
├── app/
│   ├── main.js
│   ├── app.module.js
│   ├── styles/
│   │   └── styles.css
│   └── templates/
│       ├── pagination.html
│       └── todo.list.paginated.html
├── InterviewTestPagination.Tests/
│   └── Models/
│       └── TodoServiceTest.cs
├── index.html
└── README.md
```

*A estrutura acima representa os principais arquivos envolvidos na implementação. A organização final pode variar conforme os arquivos existentes na solução.*

---
