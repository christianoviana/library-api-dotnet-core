# library-api-dotnet-core
Exemplo de API utilizando o dotnet core '2.2.0'

## Este projeto aborda os seguintes recursos:

  - Authentication (Somente nos Verbos POST, DELETE, PUT)
  - Limitação de Requisição de IP (Ip Rate Limiting)
  - Entity Framework SQL Server e Sqlite
  - Repositório Sync/Async
  - API Simulando um STS (JWT)
  - Handler Customizado para Validação de Token
  - Paginação 
  - Swagger
  - Logs (Nlog)
  - Versionamento por Url (ApiVersion)
  
## O projeto disponiliza as seguintes rotas:

  ### Author
  - GET  api/v1/authors
  - GET  api/v1/authors/{id}
  - GET  api/v1/authors/{id}/books
  - POST api/v1/authors
  - PUT  api/v1/authors/{id}
  - DEL  api/v1/authors/{id}
  
  ### Book
  - GET  api/v1/books
  - GET  api/v1/books/{id}
  - POST api/v1/books
  - PUT  api/v1/books/{id}  
  - PUT  api/v1/books/{id}/authors
  - DEL  api/v1/books/{id}
  - DEL  api/v1/books/{id}/authors/{id}
  
