# Desafio Sprint 3 - Desenvolvimento de APIs .NET

## Sobre o Projeto:
Este é um projeto base para o desenvolvimento de uma API REST funcional. 

## Tecnologias Utilizadas:
- ASP.NET Core Web API
- Entity Framework Core
- JWT (Autenticação)
- Swagger (Documentação)
- SQL Server / MySQL

## Como rodar o projeto:
1. Clone o repositório.
2. Configure a string de conexão no `appsettings.json`.
3. No Console do Gerenciador de Pacotes, execute:
   `Update-Database`
4. Execute o projeto (F5) para abrir o Swagger.

## Arquitetura:
O projeto segue o padrão de camadas (Controllers -> Services -> Repositories) para garantir a separação de responsabilidades.

## Instalação dos Pacotes Essenciais (NuGet):
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Swashbuckle.AspNetCore