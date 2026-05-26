# Documento do Sistema TASKPI

## 1. Objetivo do Sistema

O objetivo do TASKPI e permitir que usuarios organizem projetos, atividades e tarefas em um ambiente colaborativo.

O sistema ajuda no controle de demandas, separando cada projeto em atividades e cada atividade em tarefas. Alem disso, permite compartilhar projetos com outros usuarios, controlar permissoes de acesso, acompanhar participantes, confirmar email, recuperar senha e editar o perfil do usuario.

## 2. Regras de Negocio

RF001 - O usuario deve possuir uma conta cadastrada e email confirmado para conseguir acessar o sistema.

RF002 - O login deve autenticar o usuario e manter sua sessao ativa por meio de cookie de autenticacao.

RF003 - Um projeto deve possuir um usuario administrador, responsavel por gerenciar o projeto e seus participantes.

RF004 - O administrador pode criar, editar, excluir e compartilhar projetos com outros usuarios.

RF005 - O compartilhamento de projeto deve ser feito por convite enviado ao email do usuario, sem adicionar automaticamente o participante ao projeto.

RF006 - O usuario convidado deve visualizar seus convites pendentes e escolher se deseja aceitar ou recusar o acesso ao projeto.

RF007 - O administrador do projeto nao pode convidar a si mesmo para o proprio projeto.

RF008 - O administrador pode remover participantes do projeto, desde que o participante nao seja o dono do projeto.

RF009 - Cada participante deve possuir um nivel de acesso no projeto: administrador, ajudante ou observador.

RF010 - Usuarios observadores nao devem visualizar funcionalidades de criacao, edicao, exclusao ou compartilhamento no frontend.

RF011 - Usuarios ajudantes podem colaborar no quadro, mas nao devem ter permissoes administrativas do projeto.

RF012 - Cada projeto pode conter varias atividades.

RF013 - Cada atividade pode conter varias tarefas.

RF014 - As tarefas devem possuir informacoes como descricao, prioridade e status de conclusao.

RF015 - Ao editar, criar ou excluir dados do quadro, o sistema deve atualizar as informacoes exibidas ao usuario.

RF016 - O usuario pode editar seu perfil e adicionar uma foto, que tambem deve aparecer na area de participantes do projeto.

RF017 - O sistema deve enviar email para confirmacao de conta e recuperacao de senha.

RF018 - As APIs protegidas devem permitir acesso apenas para usuarios autenticados.

## 3. Funcionalidades Principais

- Cadastro de usuario.
- Confirmacao de email.
- Login e logout.
- Recuperacao de senha por email.
- Edicao de perfil do usuario.
- Upload e exibicao de foto de perfil.
- Criacao de projetos.
- Listagem de projetos do usuario.
- Edicao e exclusao de projetos.
- Compartilhamento de projeto por convite.
- Listagem de convites pendentes.
- Aceitar ou recusar convite de projeto.
- Controle de nivel de acesso por projeto.
- Exibicao dos participantes do projeto com foto, nome, email e permissao.
- Remocao de participantes do projeto.
- Criacao de atividades dentro de um projeto.
- Edicao e exclusao de atividades.
- Criacao de tarefas dentro de uma atividade.
- Edicao, exclusao e movimentacao de tarefas.
- Exibicao de prioridade das tarefas.
- Cache em memoria no frontend para melhorar o carregamento dos projetos, atividades e tarefas.
- Documentacao dos endpoints pelo Swagger.
