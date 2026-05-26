# TASKPI

Aplicacao ASP.NET Core com autenticação, confirmação por email, projetos, atividades, tarefas, convites e perfil de usuário.

## Requisitos

- .NET SDK 10
- MySQL local ou MySQL no Railway
- Ferramenta do Entity Framework instalada:

```bash
dotnet tool install --global dotnet-ef
```

Se já tiver instalada:

```bash
dotnet tool update --global dotnet-ef
```

## Criar o arquivo `.env`

Crie um arquivo chamado `.env` dentro da pasta `Sprint3`, ao lado do `Program.cs`.

Exemplo:

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


Observações rápidas:

- No Railway, use o host público TCP, parecido com `xxxx.proxy.rlwy.net`, e a porta pública TCP do MySQL.
- Não use `mysql.railway.internal` se estiver rodando a aplicação fora do Railway.
- Para Gmail, a senha precisa ser uma senha de app, não a senha normal da conta.
- O `.env` já está no `.gitignore`, então ele não deve ir para o GitHub.

## Configurar o banco

Entre na pasta do projeto:

```bash
cd Sprint3
```

Restaure os pacotes:

```bash
dotnet restore
```

Aplique as migrations no banco:

```bash
dotnet ef database update
```

Se precisar criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration
dotnet ef database update
```

## Iniciar a aplicação

Dentro da pasta `Sprint3`, rode:

```bash
dotnet run
```

Depois abra no navegador:

```text
http://localhost:5000
```

Se o projeto subir em outra porta, o terminal vai mostrar o endereço correto.

Rotas úteis:

- Login: `http://localhost:5000`
- Cadastro: `http://localhost:5000/cadastro`
- Quadro principal: `http://localhost:5000/mainscreen`
- Perfil: `http://localhost:5000/perfil`
- Swagger: `http://localhost:5000/swagger`

## Problemas comuns

### Erro de conexão com Railway

Confira se a string usa o host público e a porta pública:

```env
ConnectionStrings__DefaultConnection=Server=xxxx.proxy.rlwy.net;Port=12345;Database=railway;Uid=root;Pwd=SUA_SENHA;SslMode=Required;AllowPublicKeyRetrieval=True;
```

### Email não envia

Confira:

- `Email__Smtp__EnableSsl=true`
- `Email__Smtp__Port=587`
- `Email__Smtp__Username` igual ao email remetente
- `Email__Smtp__Password` usando senha de app do Gmail

### Após login, `/mainscreen` volta para login

Limpe os cookies do navegador para `localhost` e faça login novamente.
