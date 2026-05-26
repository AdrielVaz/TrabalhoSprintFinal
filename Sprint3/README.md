# TASKPI

## 1. Descricao do Projeto

O TASKPI e uma aplicacao web para organizacao de projetos, atividades e tarefas, seguindo uma ideia parecida com quadros de produtividade.

No sistema, o usuario pode criar projetos, compartilhar acesso com outros usuarios, controlar niveis de permissao, receber convites, cadastrar atividades, criar tarefas, editar perfil e usar foto de usuario. A aplicacao tambem possui confirmacao de email, recuperacao de senha e autenticacao por cookie.

O objetivo do projeto e facilitar o gerenciamento de tarefas em equipe, separando o projeto em atividades e cada atividade em tarefas.

## 2. Tecnologias Utilizadas

Linguagem: C#

Framework: ASP.NET Core

Banco de Dados: MySQL, usando Entity Framework Core

Seguranca: Autenticacao por cookie, JWT configurado, claims do usuario autenticado, controle de acesso por nivel no projeto e confirmacao de email

Documentacao de API: Swagger

Frontend: HTML, CSS e JavaScript puro

Email: SMTP com Gmail

## 3. Instrucoes de Execucao

Para rodar o projeto localmente, siga os passos abaixo:

1. Entre na pasta do projeto:

```bash
cd Sprint3
```

2. Crie um arquivo chamado `.env` dentro da pasta `Sprint3`, no mesmo local do `Program.cs`, e coloque as variaveis abaixo:

```env
ConnectionStrings__DefaultConnection=Server=zephyr.proxy.rlwy.net;Port=33545;Database=railway;User=root;Password=pDjFXVHtXBWLRyWCQhmaKcsOyWDkArBk;SslMode=Preferred;AllowPublicKeyRetrieval=True;Connection Timeout=30;
Jwt__Audience=Sprint3Client
Jwt__SecretKey=2p2int3-CFaSgG-TCiV-3eR-WADSh-At-L2ast-32-CGaDactZrs
Email__Smtp__Host=smtp.gmail.com
Email__Smtp__Port=587
Email__Smtp__EnableSsl=true
Email__Smtp__Username=taskpipi@gmail.com
Email__Smtp__Password=tccw zavt qhno yyia
Email__Smtp__From=taskpipi@gmail.com
Email__Smtp__FromName=TASKPI
OBS: isso so vai durar ate a apresentação
```

3. Restaure os pacotes do projeto:

```bash
dotnet restore
```

4. Aplique as migrations no banco de dados:

```bash
dotnet ef database update
```

5. Inicie a aplicacao:

```bash
dotnet run
```

Depois de iniciar, acesse no navegador:

```text
http://localhost:5000
```

Rotas de tela:

- Login: `http://localhost:5000`
- Cadastro: `http://localhost:5000/cadastro`
- Esqueci minha senha: `http://localhost:5000/esqueci-senha`
- Quadro principal: `http://localhost:5000/mainscreen`
- Perfil do usuario: `http://localhost:5000/perfil`
- Swagger: `http://localhost:5000/swagger`

## 4. Endpoints da API

Abaixo estao os principais endpoints disponiveis no sistema:

### Autenticacao

| Metodo | Endpoint | Descricao |
| --- | --- | --- |
| POST | `/api/Auth/Login` | Faz login do usuario e cria o cookie de autenticacao |
| POST | `/api/Auth/register` | Cadastra um novo usuario |
| POST | `/api/Auth/logout` | Encerra a sessao do usuario |
| GET | `/api/Auth/confirmar-email` | Confirma o email do usuario pelo token |
| POST | `/api/Auth/reenviar-confirmacao` | Reenvia o email de confirmacao |
| POST | `/api/Auth/esqueci-senha` | Envia link para redefinir senha |
| POST | `/api/Auth/redefinir-senha` | Redefine a senha do usuario |

### Projetos

| Metodo | Endpoint | Descricao |
| --- | --- | --- |
| POST | `/api/Projetos` | Cria um novo projeto |
| POST | `/api/Projetos/projetos/Listar` | Lista os projetos do usuario autenticado |
| PUT | `/api/Projetos/{projetoId}` | Atualiza um projeto |
| DELETE | `/api/Projetos/{projetoId}` | Remove um projeto |
| POST | `/api/Projetos/{projetoId}/acessos` | Cria convite para compartilhar projeto |
| GET | `/api/Projetos/{projetoId}/membros` | Lista os membros do projeto |
| POST | `/api/Projetos/{projetoId}/membros/remover` | Remove um membro do projeto |
| GET | `/api/Projetos/convites/pendentes` | Lista convites pendentes do usuario |
| POST | `/api/Projetos/convites/{conviteId}/aceitar` | Aceita um convite de projeto |
| POST | `/api/Projetos/convites/{conviteId}/recusar` | Recusa um convite de projeto |

### Atividades

| Metodo | Endpoint | Descricao |
| --- | --- | --- |
| POST | `/api/Atividades/projetos/{projetoId}/atividades` | Cria uma atividade dentro do projeto |
| GET | `/api/Atividades/projetos/{projetoId}/atividades` | Lista atividades do projeto |
| PUT | `/api/Atividades/projetos/{projetoId}/atividades/{id}` | Atualiza uma atividade |
| DELETE | `/api/Atividades/projetos/{projetoId}/atividades/{id}` | Remove uma atividade |

### Tarefas

| Metodo | Endpoint | Descricao |
| --- | --- | --- |
| POST | `/api/Tarefas/projetos/{projetoId}/atividades/{atividadeId}/tarefas` | Cria uma tarefa dentro da atividade |
| GET | `/api/Tarefas/projetos/{projetoId}/atividades/{atividadeId}/tarefas` | Lista tarefas de uma atividade |
| PUT | `/api/Tarefas/projetos/{projetoId}/atividades/{atividadeId}/tarefas/{id}` | Atualiza uma tarefa |
| DELETE | `/api/Tarefas/projetos/{projetoId}/atividades/{atividadeId}/tarefas/{id}` | Remove uma tarefa |

### Usuario e Perfil

| Metodo | Endpoint | Descricao |
| --- | --- | --- |
| GET | `/api/Usuarios/me` | Retorna os dados do usuario autenticado |
| PUT | `/api/Usuarios/me` | Atualiza nome e foto do perfil |
| GET | `/api/Usuarios/{id}/foto` | Retorna a foto de perfil de um usuario |

## Observacoes

- O arquivo `.env` nao deve ser enviado para o GitHub.
- As credenciais acima sao temporarias e devem ser trocadas depois da apresentacao.
- Caso o banco do Railway esteja lento, algumas requisicoes podem demorar um pouco mais no primeiro carregamento.
- Para testar os endpoints protegidos no Swagger, primeiro faca login pelo sistema para gerar o cookie de autenticacao.
