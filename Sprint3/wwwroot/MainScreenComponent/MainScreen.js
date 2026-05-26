const nome = localStorage.getItem("nomeUsuario");
const fotoPerfilUrl = localStorage.getItem("fotoPerfilUrl");

if (nome) {
    document.getElementById("NameUser").innerText = nome;
}

renderHeaderProfile(nome, fotoPerfilUrl);

let loadingRequestCount = 0;
let loadingMessages = [];

function atualizarLoading() {
    const loading = document.getElementById('requestLoading');
    if (!loading) return;

    const ativo = loadingRequestCount > 0;
    const texto = document.getElementById('requestLoadingText');
    if (texto) {
        texto.textContent = loadingMessages[loadingMessages.length - 1] || 'Carregando...';
    }

    loading.classList.toggle('active', ativo);
    loading.setAttribute('aria-hidden', ativo ? 'false' : 'true');
}

async function apiFetch(url, options, config = {}) {
    const silent = Boolean(config.silent);
    const loadingText = config.loadingText || 'Carregando...';
    if (!silent) {
        loadingRequestCount++;
        loadingMessages.push(loadingText);
        atualizarLoading();
    }

    try {
        return await fetch(url, options);
    } finally {
        if (!silent) {
            loadingRequestCount = Math.max(0, loadingRequestCount - 1);
            const messageIndex = loadingMessages.lastIndexOf(loadingText);
            if (messageIndex >= 0) {
                loadingMessages.splice(messageIndex, 1);
            }
            atualizarLoading();
        }
    }
}

const API = {
    listProjetos: () => apiFetch('/api/Projetos/projetos/Listar', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: '{}'
    }, { loadingText: 'Carregando projetos...' }),
    criarProjeto: (descricao) => apiFetch('/api/Projetos', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ descricao })
    }, { loadingText: 'Salvando projeto...' }),
    atualizarProjeto: (id, descricao) => apiFetch(`/api/Projetos/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ descricao })
    }, { loadingText: 'Atualizando projeto...' }),
    deletarProjeto: (id) => apiFetch(`/api/Projetos/${id}`, {
        method: 'DELETE',
        credentials: 'include'
    }, { loadingText: 'Excluindo projeto...' }),
    removerMembroProjeto: (projetoId, email) => apiFetch(`/api/Projetos/${projetoId}/membros/remover`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({ email })
    }, { loadingText: 'Removendo membro...' }),
    listarAtividades: (projetoId, silent = false) => apiFetch(`/api/Atividades/projetos/${projetoId}/atividades`, {
        credentials: 'include'
    }, { silent, loadingText: 'Carregando atividades...' }),
    criarAtividade: (projetoId, body) => apiFetch(`/api/Atividades/projetos/${projetoId}/atividades`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(body)
    }, { loadingText: 'Salvando atividade...' }),
    atualizarAtividade: (projetoId, atividadeId, body) => apiFetch(`/api/Atividades/projetos/${projetoId}/atividades/${atividadeId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(body)
    }, { loadingText: 'Atualizando atividade...' }),
    deletarAtividade: (projetoId, atividadeId) => apiFetch(`/api/Atividades/projetos/${projetoId}/atividades/${atividadeId}`, {
        method: 'DELETE',
        credentials: 'include'
    }, { loadingText: 'Excluindo atividade...' }),
    criarTarefa: (projetoId, atividadeId, body) => apiFetch(`/api/Tarefas/projetos/${projetoId}/atividades/${atividadeId}/tarefas`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(body)
    }, { loadingText: 'Salvando tarefa...' }),
    atualizarTarefa: (projetoId, atividadeId, tarefaId, body) => apiFetch(`/api/Tarefas/projetos/${projetoId}/atividades/${atividadeId}/tarefas/${tarefaId}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(body)
    }, { loadingText: 'Atualizando tarefa...' }),
    deletarTarefa: (projetoId, atividadeId, tarefaId) => apiFetch(`/api/Tarefas/projetos/${projetoId}/atividades/${atividadeId}/tarefas/${tarefaId}`, {
        method: 'DELETE',
        credentials: 'include'
    }, { loadingText: 'Excluindo tarefa...' }),
    listarMembros: (projetoId, silent = false) => apiFetch(`/api/Projetos/${projetoId}/membros`, {
        credentials: 'include'
    }, { silent, loadingText: 'Carregando membros...' }),
    compartilharProjeto: (projetoId, body) => apiFetch(`/api/Projetos/${projetoId}/acessos`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify(body)
    }, { loadingText: 'Enviando convite...' }),
    listarConvites: (silent = false) => apiFetch('/api/Projetos/convites/pendentes', {
        credentials: 'include'
    }, { silent, loadingText: 'Carregando convites...' }),
    obterPerfil: (silent = false) => apiFetch('/api/Usuarios/me', {
        credentials: 'include'
    }, { silent, loadingText: 'Carregando perfil...' }),
    aceitarConvite: (conviteId) => apiFetch(`/api/Projetos/convites/${conviteId}/aceitar`, {
        method: 'POST',
        credentials: 'include'
    }, { loadingText: 'Aceitando convite...' }),
    recusarConvite: (conviteId) => apiFetch(`/api/Projetos/convites/${conviteId}/recusar`, {
        method: 'POST',
        credentials: 'include'
    }, { loadingText: 'Recusando convite...' }),
    logout: () => apiFetch('/api/Auth/logout', {
        method: 'POST',
        credentials: 'include'
    }, { loadingText: 'Saindo...' })

};

async function logout(event) {
    event?.preventDefault();
    console.log('Logout solicitado');

    const btnSair = document.getElementById('btnSair');
    if (btnSair) {
        btnSair.disabled = true;
        btnSair.textContent = 'Saindo...';
    }

    try {
        const response = await API.logout();
        if (!response.ok) {
            throw new Error('Nao foi possivel sair agora.');
        }
    } catch (error) {
        console.error('Erro ao fazer logout:', error);
    } finally {
        localStorage.removeItem('nomeUsuario');
        localStorage.removeItem('fotoPerfilUrl');
        localStorage.removeItem('convitesPendentes');
        window.location.replace('/index.html');
    }
}

document.addEventListener('click', (event) => {
    const btnSair = event.target.closest('#btnSair');
    if (!btnSair) return;
    logout(event);
});

const Prioridade = { Alta: 0, Media: 1, Baixa: 2 };

function normPrio(p) {
    if (p === 'Alta' || p === 0) return Prioridade.Alta;
    if (p === 'Media' || p === 'Média' || p === 1) return Prioridade.Media;
    if (p === 'Baixa' || p === 2) return Prioridade.Baixa;
    return Prioridade.Media;
}

let projetos = [];
let projetoAtual = null;
let atividades = [];
let membros = [];
let convitesPendentes = [];
let draggedTarefaId = null;
let draggedAtividadeId = null;
let modalTarefaBootstrap = null;
let modalProjetoBootstrap = null;
let atividadeAtualParaNovaTarefa = null;
let projetoEditandoId = null;
let atividadeEditandoId = null;
let projetoContextoId = null;
let membroRemocaoEmail = null;
let projetosCacheCarregado = false;
const boardCache = new Map();
const boardCachePromises = new Map();

function invalidarCacheProjetos() {
    projetosCacheCarregado = false;
}

function invalidarCacheBoard(projetoId = projetoAtual?.id) {
    if (!projetoId) return;
    const key = Number(projetoId);
    boardCache.delete(key);
    boardCachePromises.delete(key);
    renderListaProjetosSePronto();
}

function salvarCacheBoard(projetoId, atividadesCache, membrosCache) {
    boardCache.set(Number(projetoId), {
        atividades: structuredCloneSafe(atividadesCache),
        membros: structuredCloneSafe(membrosCache)
    });
}

function obterCacheBoard(projetoId) {
    const cached = boardCache.get(Number(projetoId));
    if (!cached) return null;

    return {
        atividades: structuredCloneSafe(cached.atividades),
        membros: structuredCloneSafe(cached.membros)
    };
}

function renderListaProjetosSePronto() {
    if (document.getElementById('projetoList')) {
        renderListaProjetos();
    }
}

function estadoCacheProjeto(projetoId) {
    const key = Number(projetoId);

    if (boardCache.has(key)) {
        return { classe: 'ready', texto: 'cache pronto' };
    }

    if (boardCachePromises.has(key)) {
        return { classe: 'loading', texto: 'cacheando' };
    }

    return { classe: 'cold', texto: 'sem cache' };
}

async function buscarBoardProjeto(projetoId, options = {}) {
    const key = Number(projetoId);
    const cached = obterCacheBoard(key);
    if (cached) return cached;

    if (boardCachePromises.has(key)) {
        return boardCachePromises.get(key);
    }

    const silent = Boolean(options.silent);
    const promise = (async () => {
        const [res, membrosRes] = await Promise.all([
            API.listarAtividades(key, silent),
            API.listarMembros(key, silent)
        ]);

        if (res.status === 401) {
            throw new Error('UNAUTHORIZED');
        }

        const atividadesData = await handleRes(res);
        let membrosData = [];
        try {
            membrosData = await handleRes(membrosRes);
        } catch {
            membrosData = [];
        }

        salvarCacheBoard(key, atividadesData, membrosData);
        return obterCacheBoard(key);
    })();

    boardCachePromises.set(key, promise);
    renderListaProjetosSePronto();

    try {
        return await promise;
    } finally {
        boardCachePromises.delete(key);
        renderListaProjetosSePronto();
    }
}

function preCarregarBoardsNoCache() {
    const ids = projetos
        .map(p => Number(p.id))
        .filter(id => id && !boardCache.has(id) && !boardCachePromises.has(id));

    if (!ids.length) return;

    mostrarFonteBoard('refresh', 'Atualizando cache em segundo plano...');

    const promessas = ids.map(projetoId =>
        buscarBoardProjeto(projetoId, { silent: true }).catch(() => null)
    );

    Promise.allSettled(promessas).then(() => {
        renderListaProjetosSePronto();
        if (projetoAtual && boardCache.has(Number(projetoAtual.id))) {
            mostrarFonteBoard('cache', 'Cache pronto');
        }
    });
}

function structuredCloneSafe(value) {
    if (typeof structuredClone === 'function') {
        return structuredClone(value);
    }

    return JSON.parse(JSON.stringify(value));
}

function mostrarFonteBoard(tipo, texto) {
    const el = document.getElementById('boardSource');
    if (!el) return;

    if (!texto) {
        el.className = 'board-source';
        el.textContent = '';
        return;
    }

    el.className = 'board-source show ' + tipo;
    el.textContent = texto;
}

function nivelProjeto(projeto = projetoAtual) {
    return (projeto && projeto.nivelAcesso ? projeto.nivelAcesso : '').toString();
}

function podeAdministrarProjeto(projeto = projetoAtual) {
    return nivelProjeto(projeto) === 'Adm';
}

function podeEditarQuadro(projeto = projetoAtual) {
    const nivel = nivelProjeto(projeto);
    return nivel === 'Adm' || nivel === 'Ajudante';
}

function toast(msg, isError) {
    const el = document.createElement('div');
    el.className = 'alert ' + (isError ? 'alert-danger' : 'alert-success') + ' py-2 px-3 mb-2';
    el.textContent = msg;
    document.getElementById('toastArea').appendChild(el);
    setTimeout(() => el.remove(), 4000);
}

async function handleRes(res, okMsg) {
    let data = null;
    try { data = await res.json(); } catch { }
    if (!res.ok) {
        const msg = (data && data.message) || res.statusText || 'Erro na requisição';
        throw new Error(msg);
    }
    if (okMsg) toast(okMsg);
    return data;
}

async function carregarProjetos() {
    if (!projetosCacheCarregado) {
        const res = await API.listProjetos();

        if (res.status === 401) {
            window.location.href = '/index.html';
            return;
        }

        projetos = await handleRes(res);
        projetosCacheCarregado = true;
    }

    // pega o projeto salvo
    const projetoSalvoId = parseInt(localStorage.getItem('projetoAtualId'));

    // tenta encontrar ele na lista
    const projetoSalvo = projetos.find(p => p.id === projetoSalvoId);

    renderListaProjetos();

    preCarregarBoardsNoCache();

    if (projetoAtual && !projetos.find(p => p.id === projetoAtual.id)) {
        projetoAtual = null;
        atividades = [];
        membros = [];
    }

    // se existir projeto salvo, abre ele
    if (projetoSalvo) {
        await selecionarProjeto(projetoSalvo.id);
    }
    // senão abre o primeiro
    else if (!projetoAtual && projetos.length) {
        await selecionarProjeto(projetos[0].id);
    }
    else if (projetoAtual) {
        await carregarBoardProjeto(projetoAtual.id);
    }
    else {
        mostrarBoardVazio();
    }

}

function renderListaProjetos() {
    const box = document.getElementById('projetoList');
    box.innerHTML = '';
    projetos.forEach(p => {
        const b = document.createElement('button');
        b.type = 'button';
        const compartilhado = Boolean(p.compartilhado) || (p.nivelAcesso && p.nivelAcesso !== 'Adm');
        const estadoCache = estadoCacheProjeto(p.id);
        b.className = 'projeto-item' + (compartilhado ? ' shared' : '') + (projetoAtual && projetoAtual.id === p.id ? ' active' : '');
        b.title = (p.descricao || ('Projeto #' + p.id)) + ' - ' + (compartilhado ? 'Compartilhado' : 'Meu projeto');
        b.innerHTML =
            '<span class="project-icon" aria-hidden="true">' + projectIconSvg(compartilhado) + '</span>' +
            '<span class="project-main">' +
            '<span class="project-name">' + escapeHtml(p.descricao || ('Projeto #' + p.id)) + '</span>' +
            '<span class="project-meta"><span class="project-person-icon" aria-hidden="true">' + personIconSvg(compartilhado) + '</span>' +
            '<span class="project-badge">' + (compartilhado ? 'Compartilhado' : 'Meu projeto') + (p.nivelAcesso ? ' · ' + escapeHtml(p.nivelAcesso) : '') + '</span></span>' +
            '<span class="project-cache-state ' + estadoCache.classe + '">' + estadoCache.texto + '</span>' +
            '</span>';
        b.onclick = () => selecionarProjeto(p.id);
        b.oncontextmenu = async (e) => {
            e.preventDefault();
            if (podeAdministrarProjeto(p)) {
                await selecionarProjeto(p.id);
                abrirMenuProjeto(e.clientX, e.clientY, p.id);
            }
        };
        box.appendChild(b);
    });
    if (!projetos.length) {
        const hint = document.createElement('p');
        hint.className = 'sidebar-hint';
        hint.textContent = 'Nenhum projeto ainda. Clique em « Novo projeto ». Botão direito em um projeto para excluir.';
        box.appendChild(hint);
    }
}

async function selecionarProjeto(id) {
    projetoAtual = projetos.find(p => p.id === id) || null;
    localStorage.setItem('projetoAtualId', id);
    renderListaProjetos();
    if (!projetoAtual) {
        mostrarBoardVazio();
        return;
    }
    await carregarBoardProjeto(id);
}

async function carregarBoardProjeto(projetoId) {
    const cached = obterCacheBoard(projetoId);
    if (cached) {
        atividades = cached.atividades;
        membros = cached.membros;
        renderBoard();
        mostrarFonteBoard('cache', 'Dados em cache');
        return;
    }

    try {
        const aguardandoCache = boardCachePromises.has(Number(projetoId));
        if (!aguardandoCache) {
            buscarBoardProjeto(projetoId, { silent: true }).catch(() => null);
        }

        mostrarFonteBoard('refresh', aguardandoCache ? 'Aguardando cache em andamento...' : 'Atualizando cache em segundo plano...');

        const data = await buscarBoardProjeto(projetoId);
        atividades = data.atividades;
        membros = data.membros;
        renderBoard();
        mostrarFonteBoard('cache', 'Dados em cache');
    } catch (error) {
        if (error.message === 'UNAUTHORIZED') {
            window.location.href = '/index.html';
            return;
        }

        toast(error.message || 'Erro ao carregar projeto.', true);
    }
}

function mostrarBoardVazio() {
    document.getElementById('emptyState').style.display = 'flex';
    document.getElementById('boardContent').style.display = 'none';
    mostrarFonteBoard('', '');
}

function renderBoard() {
    if (!projetoAtual) {
        mostrarBoardVazio();
        return;
    }
    document.getElementById('emptyState').style.display = 'none';
    document.getElementById('boardContent').style.display = 'flex';
    const title = projetoAtual.descricao || ('Projeto #' + projetoAtual.id);
    document.getElementById('boardTitle').textContent = title;
    const tarefasDoProjeto = atividades.flatMap(a => (a.tarefas || []).map(t => ({ ...t, atividadeId: a.id })));
    const total = tarefasDoProjeto.length;
    const done = tarefasDoProjeto.filter(t => t.concluida).length;
    document.getElementById('boardMeta').textContent = total + ' tarefa' + (total !== 1 ? 's' : '') +
        (total ? ' · ' + done + ' concluída' + (done !== 1 ? 's' : '') : '') +
        (projetoAtual.nivelAcesso ? ' · ' + projetoAtual.nivelAcesso : '');
    renderMembros();
    document.getElementById('btnNovaAtividade').style.display = podeEditarQuadro() ? 'inline-flex' : 'none';
    document.getElementById('btnCompartilhar').style.display = podeAdministrarProjeto() ? 'inline-flex' : 'none';

    const cols = document.getElementById('boardColumns');
    cols.innerHTML = '';

    atividades.forEach(atividade => {
        const listTasks = atividade.tarefas || [];
        const colEl = document.createElement('div');
        colEl.className = 'column col-open';
        colEl.dataset.atividadeId = atividade.id;

        const head = document.createElement('div');
        head.className = 'column-header';
        const descricaoAtividade = (atividade.descricao || '').trim();
        head.innerHTML =
            '<div class="column-title-block">' +
            '<div class="column-title-row"><span class="col-dot"></span><span class="column-title-text">' +
            escapeHtml(atividade.titulo || 'Atividade') + '</span></div>' +
            (descricaoAtividade ? '<p class="column-desc">' + escapeHtml(descricaoAtividade) + '</p>' : '') +
            '</div><span class="count">' + listTasks.length + '</span>';
        head.oncontextmenu = (e) => {
            e.preventDefault();
            if (podeEditarQuadro()) {
                abrirMenuAtividade(e.clientX, e.clientY, atividade.id);
            }
        };
        colEl.appendChild(head);

        const body = document.createElement('div');
        body.className = 'column-body';
        body.dataset.atividadeId = atividade.id;

        listTasks.forEach(t => body.appendChild(criarCard(t, atividade.id)));

        if (podeEditarQuadro()) {
            body.addEventListener('dragover', (e) => {
                e.preventDefault();
                body.classList.add('drag-over');
            });
            body.addEventListener('dragleave', () => body.classList.remove('drag-over'));
            body.addEventListener('drop', (e) => {
                e.preventDefault();
                body.classList.remove('drag-over');
                onDropTarefa(atividade.id);
            });
        }

        let addBtn = null;
        if (podeEditarQuadro()) {
            addBtn = document.createElement('button');
            addBtn.type = 'button';
            addBtn.className = 'add-card-btn';
            addBtn.innerHTML = '+ Adicionar cartão';
            addBtn.onclick = () => abrirModalNovaTarefa(atividade.id);
        }

        colEl.appendChild(body);
        if (addBtn) colEl.appendChild(addBtn);
        cols.appendChild(colEl);
    });

    if (!atividades.length) {
        if (podeEditarQuadro()) {
            const emptyButton = document.createElement('button');
            emptyButton.type = 'button';
            emptyButton.className = 'empty-activity-launcher';
            emptyButton.innerHTML = '<span class="window-icon" aria-hidden="true"></span><span>Criar atividade</span>';
            emptyButton.onclick = criarAtividade;
            cols.appendChild(emptyButton);
        }
    }
}

function renderMembros() {
    const box = document.getElementById('projectMembers');
    box.innerHTML = '';
    if (!membros.length) {
        const empty = document.createElement('span');
        empty.className = 'member-empty';
        empty.textContent = 'Sem participantes';
        box.appendChild(empty);
        return;
    }

    membros.forEach(m => {
        const avatar = document.createElement('span');
        const nomeMembro = m.nome || m.email || 'Usuario';
        avatar.className = 'member-avatar';
        avatar.title = nomeMembro + ' - ' + m.nivelAcesso;
        avatar.setAttribute('aria-label', avatar.title);

        if (m.fotoPerfilUrl) {
            const img = document.createElement('img');
            img.src = m.fotoPerfilUrl;
            img.alt = nomeMembro;
            avatar.appendChild(img);
        } else {
            avatar.textContent = inicialNome(nomeMembro);
        }

        box.appendChild(avatar);
    });
}

function renderHeaderProfile(nomeUsuario, fotoUrl) {
    const avatar = document.getElementById('headerProfileAvatar');
    const label = document.getElementById('headerProfileName');
    const nomePerfil = nomeUsuario || 'Perfil';

    if (label) {
        label.textContent = nomePerfil;
    }

    if (!avatar) return;

    avatar.innerHTML = '';
    if (fotoUrl) {
        const img = document.createElement('img');
        img.src = fotoUrl;
        img.alt = nomePerfil;
        avatar.appendChild(img);
    } else {
        avatar.textContent = inicialNome(nomePerfil);
    }
}

function inicialNome(valor) {
    return (valor || 'U').trim().charAt(0).toUpperCase() || 'U';
}

function projectIconSvg(compartilhado) {
    if (compartilhado) {
        return '<svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 7.5A2.5 2.5 0 0 1 5.5 5H9l2 2h7.5A2.5 2.5 0 0 1 21 9.5v7A2.5 2.5 0 0 1 18.5 19h-13A2.5 2.5 0 0 1 3 16.5z"/><path d="M8 15a3 3 0 0 1 6 0"/><circle cx="11" cy="11" r="1.7"/></svg>';
    }

    return '<svg width="17" height="17" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><path d="M3 7.5A2.5 2.5 0 0 1 5.5 5H9l2 2h7.5A2.5 2.5 0 0 1 21 9.5v7A2.5 2.5 0 0 1 18.5 19h-13A2.5 2.5 0 0 1 3 16.5z"/></svg>';
}

function personIconSvg(compartilhado) {
    if (compartilhado) {
        return '<svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round"><path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/></svg>';
    }

    return '<svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round"><path d="M20 21a8 8 0 0 0-16 0"/><circle cx="12" cy="7" r="4"/></svg>';
}

function prioLabel(p) {
    const n = normPrio(p);
    if (n === Prioridade.Alta) return { text: 'Alta', cls: 'prio-alta', badge: 'prio-alta badge-fill' };
    if (n === Prioridade.Media) return { text: 'Média', cls: 'prio-media', badge: 'prio-media badge-fill' };
    return { text: 'Baixa', cls: 'prio-baixa', badge: 'prio-baixa badge-fill' };
}

function criarCard(t, atividadeId) {
    const div = document.createElement('div');
    const pl = prioLabel(t.prioridade);
    div.className = 'card-task ' + pl.cls + (podeEditarQuadro() ? '' : ' readonly');
    div.draggable = podeEditarQuadro();
    div.dataset.tarefaId = t.id;
    div.dataset.atividadeId = atividadeId;
    const descricaoTarefa = (t.descricao || '').trim();
    const descPrev = descricaoTarefa.slice(0, 220);
    div.innerHTML =
        '<div class="card-topline"><div class="card-badges">' +
        '<span class="prio-badge ' + pl.badge + '">' + pl.text + '</span>' +
        (t.concluida ? '<span class="prio-badge prio-baixa badge-fill">Concluída</span>' : '') +
        '</div>' + (podeEditarQuadro() ? '<div class="card-actions">' +
            '<button type="button" class="btn-icon" data-del="' + t.id + '" title="Excluir">✕</button></div>' : '') + '</div>' +
        '<div class="title">' + escapeHtml(t.titulo || '') + '</div>' +
        (descPrev ? '<div class="desc-preview"><span class="desc-label">Descrição</span><p>' + escapeHtml(descPrev) + (descricaoTarefa.length > 220 ? '…' : '') + '</p></div>' : '');

    div.addEventListener('dragstart', () => {
        if (!podeEditarQuadro()) return;
        draggedTarefaId = t.id;
        draggedAtividadeId = atividadeId;
        div.style.opacity = '0.55';
        div.style.transform = 'rotate(2deg)';
    });
    div.addEventListener('dragend', () => {
        div.style.opacity = '1';
        div.style.transform = '';
        draggedTarefaId = null;
        draggedAtividadeId = null;
        document.querySelectorAll('.column-body.drag-over').forEach(el => el.classList.remove('drag-over'));
    });
    div.addEventListener('click', (e) => {
        if (e.target.closest('.btn-icon')) return;
        if (!podeEditarQuadro()) return;
        abrirModalEditarTarefa(atividadeId, t.id);
    });
    const btnDel = div.querySelector('[data-del]');
    if (btnDel) {
        btnDel.addEventListener('click', (e) => {
            e.stopPropagation();
            if (confirm('Excluir esta tarefa?')) excluirTarefa(atividadeId, t.id);
        });
    }
    return div;
}

function escapeHtml(s) {
    const d = document.createElement('div');
    d.textContent = s;
    return d.innerHTML;
}

function tarefaPayload(base) {
    const titulo = base.titulo;
    const descricao = base.descricao;
    const dataCriacao = base.dataCriacao || new Date().toISOString();
    return {
        titulo,
        descricao,
        concluida: !!base.concluida,
        dataCriacao,
        dataConclusao: base.concluida ? (base.dataConclusao || new Date().toISOString()) : null,
        prioridade: normPrio(base.prioridade)
    };
}

async function onDropTarefa(targetAtividadeId) {
    if (!draggedTarefaId || !projetoAtual) return;
    const origem = atividades.find(a => a.id === draggedAtividadeId);
    const t = origem && (origem.tarefas || []).find(x => x.id === draggedTarefaId);
    if (!t) return;
    const payload = tarefaPayload(t);
    try {
        await handleRes(
            await API.atualizarTarefa(projetoAtual.id, targetAtividadeId, draggedTarefaId, payload)
        );
        invalidarCacheBoard(projetoAtual.id);
        await carregarBoardProjeto(projetoAtual.id);
    } catch (e) {
        toast(e.message, true);
    }
}

function abrirModalNovaTarefa(atividadeId) {
    atividadeAtualParaNovaTarefa = atividadeId;
    document.getElementById('tarefaId').value = '';
    document.getElementById('modalTarefaTitle').textContent = 'Nova tarefa';
    document.getElementById('tarefaTitulo').value = '';
    document.getElementById('tarefaDescricao').value = '';
    document.getElementById('tarefaConcluida').checked = false;
    const sel = document.getElementById('tarefaPrioridade');
    sel.value = '1';
    document.getElementById('btnExcluirTarefa').style.display = 'none';
    modalTarefaBootstrap.show();
}

function abrirModalEditarTarefa(atividadeId, id) {
    atividadeAtualParaNovaTarefa = atividadeId;
    const atividade = atividades.find(a => a.id === atividadeId);
    const t = atividade && (atividade.tarefas || []).find(x => x.id === id);
    if (!t) return;
    document.getElementById('tarefaId').value = t.id;
    document.getElementById('modalTarefaTitle').textContent = 'Editar tarefa';
    document.getElementById('tarefaTitulo').value = t.titulo || '';
    document.getElementById('tarefaDescricao').value = t.descricao || '';
    document.getElementById('tarefaPrioridade').value = String(normPrio(t.prioridade));
    document.getElementById('tarefaConcluida').checked = !!t.concluida;
    document.getElementById('btnExcluirTarefa').style.display = 'inline-block';
    modalTarefaBootstrap.show();
}

async function salvarTarefaModal() {
    const idField = document.getElementById('tarefaId').value;
    const titulo = document.getElementById('tarefaTitulo').value.trim();
    if (!titulo) {
        toast('Informe o título da tarefa.', true);
        return;
    }
    const descricao = document.getElementById('tarefaDescricao').value.trim() || null;
    const prioridade = parseInt(document.getElementById('tarefaPrioridade').value, 10);
    const concluida = document.getElementById('tarefaConcluida').checked;
    const baseDate = new Date().toISOString();

    const body = {
        titulo,
        descricao,
        concluida,
        dataCriacao: baseDate,
        dataConclusao: concluida ? (new Date().toISOString()) : null,
        prioridade
    };

    try {
        if (idField) {
            const atividade = atividades.find(a => a.id === atividadeAtualParaNovaTarefa);
            const existente = atividade && (atividade.tarefas || []).find(x => x.id === parseInt(idField, 10));
            body.dataCriacao = existente && existente.dataCriacao ? existente.dataCriacao : baseDate;
            if (concluida && existente && existente.dataConclusao) {
                body.dataConclusao = existente.dataConclusao;
            }
            await handleRes(await API.atualizarTarefa(projetoAtual.id, atividadeAtualParaNovaTarefa, parseInt(idField, 10), body));
        } else {
            await handleRes(await API.criarTarefa(projetoAtual.id, atividadeAtualParaNovaTarefa, body), 'Tarefa criada.');
        }
        modalTarefaBootstrap.hide();
        invalidarCacheBoard(projetoAtual.id);
        await carregarBoardProjeto(projetoAtual.id);
    } catch (e) {
        toast(e.message, true);
    }
}

async function excluirTarefa(atividadeId, id) {
    try {
        await handleRes(await API.deletarTarefa(projetoAtual.id, atividadeId, id));
        modalTarefaBootstrap.hide();
        invalidarCacheBoard(projetoAtual.id);
        await carregarBoardProjeto(projetoAtual.id);
    } catch (e) {
        toast(e.message, true);
    }
}

async function excluirProjeto(id) {
    try {
        await handleRes(await API.deletarProjeto(id));
        if (projetoAtual && projetoAtual.id === id) {
            projetoAtual = null;
            atividades = [];
            membros = [];
        }
        invalidarCacheProjetos();
        invalidarCacheBoard(id);
        await carregarProjetos();
        toast('Projeto removido.');
    } catch (e) {
        toast(e.message, true);
    }
}

function abrirMenuProjeto(x, y, projetoId) {
    if (!podeAdministrarProjeto(projetos.find(p => p.id === projetoId))) return;
    const menu = document.getElementById('contextMenu');
    menu.innerHTML =
        '<button type="button" data-action="editar-projeto">Editar projeto</button>' +
        '<button type="button" data-action="remover-membro">Remover participante</button>' +
        '<div class="menu-separator"></div>' +
        '<button type="button" class="danger" data-action="deletar-projeto">Deletar projeto</button>';

    menu.querySelector('[data-action="editar-projeto"]').onclick = () => {
        fecharContextMenu();
        abrirEditarProjeto(projetoId);
    };
    menu.querySelector('[data-action="remover-membro"]').onclick = () => {
        fecharContextMenu();
        abrirRemoverMembro(projetoId);
    };
    menu.querySelector('[data-action="deletar-projeto"]').onclick = () => {
        fecharContextMenu();
        excluirProjeto(projetoId);
    };
    posicionarContextMenu(menu, x, y);
}

function abrirMenuAtividade(x, y, atividadeId) {
    if (!podeEditarQuadro()) return;
    const menu = document.getElementById('contextMenu');
    menu.innerHTML =
        '<button type="button" data-action="editar-atividade">Editar atividade</button>' +
        '<button type="button" class="danger" data-action="deletar-atividade">Deletar atividade</button>';

    menu.querySelector('[data-action="editar-atividade"]').onclick = () => {
        fecharContextMenu();
        abrirEditarAtividade(atividadeId);
    };
    menu.querySelector('[data-action="deletar-atividade"]').onclick = () => {
        fecharContextMenu();
        excluirAtividade(atividadeId);
    };
    posicionarContextMenu(menu, x, y);
}

function posicionarContextMenu(menu, x, y) {
    menu.classList.add('open');
    const rect = menu.getBoundingClientRect();
    const left = Math.min(x, window.innerWidth - rect.width - 10);
    const top = Math.min(y, window.innerHeight - rect.height - 10);
    menu.style.left = Math.max(10, left) + 'px';
    menu.style.top = Math.max(10, top) + 'px';
}

function fecharContextMenu() {
    const menu = document.getElementById('contextMenu');
    menu.classList.remove('open');
    menu.innerHTML = '';
}

function abrirEditarProjeto(projetoId) {
    const projeto = projetos.find(p => p.id === projetoId);
    if (!projeto) return;

    projetoEditandoId = projetoId;
    document.getElementById('modalProjetoTitle').textContent = 'Editar projeto';
    document.getElementById('btnSalvarProjeto').textContent = 'Salvar projeto';
    document.getElementById('inputProjetoDesc').value = projeto.descricao || '';
    modalProjetoBootstrap.show();
}

function abrirRemoverMembro(projetoId) {
    projetoContextoId = projetoId;
    membroRemocaoEmail = null;
    renderListaRemoverMembros();
    modalRemoverMembroBootstrap.show();
}

function renderListaRemoverMembros() {
    const list = document.getElementById('listaRemoverMembros');
    list.innerHTML = '';

    const removiveis = membros.filter(m => m.nivelAcesso !== 'Adm');
    if (!removiveis.length) {
        list.innerHTML = '<p class="invite-meta mb-0">Nenhum participante removível neste projeto.</p>';
        return;
    }

    removiveis.forEach(m => {
        const nomeMembro = m.nome || m.email || 'Usuario';
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.className = 'member-remove-option';
        btn.dataset.email = m.email || '';
        btn.innerHTML =
            '<span class="member-remove-photo">' + avatarHtml(nomeMembro, m.fotoPerfilUrl) + '</span>' +
            '<span class="member-remove-info">' +
            '<p class="member-remove-name">' + escapeHtml(nomeMembro) + '</p>' +
            '<p class="member-remove-email">' + escapeHtml(m.email || '') + ' · ' + escapeHtml(m.nivelAcesso || '') + '</p>' +
            '</span>';
        btn.onclick = () => {
            membroRemocaoEmail = m.email || '';
            document.querySelectorAll('.member-remove-option').forEach(el => el.classList.toggle('active', el === btn));
        };
        list.appendChild(btn);
    });
}

function avatarHtml(nomeMembro, fotoUrl) {
    if (fotoUrl) {
        return '<img src="' + escapeHtml(fotoUrl) + '" alt="' + escapeHtml(nomeMembro) + '">';
    }

    return escapeHtml(inicialNome(nomeMembro));
}

function abrirEditarAtividade(atividadeId) {
    const atividade = atividades.find(a => a.id === atividadeId);
    if (!atividade) return;

    atividadeEditandoId = atividadeId;
    document.getElementById('modalAtividadeTitle').textContent = 'Editar atividade';
    document.getElementById('btnSalvarAtividade').textContent = 'Salvar atividade';
    document.getElementById('inputAtividadeNome').value = atividade.titulo || '';
    document.getElementById('inputAtividadeDesc').value = atividade.descricao || '';
    modalAtividadeBootstrap.show();
}

async function criarAtividade() {
    if (!projetoAtual) return;
    if (!podeEditarQuadro()) return;
    atividadeEditandoId = null;
    document.getElementById('modalAtividadeTitle').textContent = 'Nova atividade';
    document.getElementById('btnSalvarAtividade').textContent = 'Criar atividade';
    document.getElementById('inputAtividadeNome').value = '';
    document.getElementById('inputAtividadeDesc').value = '';
    modalAtividadeBootstrap.show();
}

async function salvarAtividadeModal() {
    const titulo = document.getElementById('inputAtividadeNome').value.trim();
    if (!titulo) { toast('Informe o nome da atividade.', true); return; }
    const descricao = document.getElementById('inputAtividadeDesc').value.trim() || null;
    try {
        if (atividadeEditandoId) {
            await handleRes(await API.atualizarAtividade(projetoAtual.id, atividadeEditandoId, { titulo, descricao }), 'Atividade atualizada.');
        } else {
            await handleRes(await API.criarAtividade(projetoAtual.id, { titulo, descricao }), 'Atividade criada.');
        }
        modalAtividadeBootstrap.hide();
        atividadeEditandoId = null;
        invalidarCacheBoard(projetoAtual.id);
        await carregarBoardProjeto(projetoAtual.id);
    } catch (e) { toast(e.message, true); }
}

async function excluirAtividade(id) {
    if (!projetoAtual) return;
    if (!confirm('Excluir esta atividade e todas as tarefas dela?')) return;

    try {
        await handleRes(await API.deletarAtividade(projetoAtual.id, id));
        invalidarCacheBoard(projetoAtual.id);
        await carregarBoardProjeto(projetoAtual.id);
        toast('Atividade removida.');
    } catch (e) {
        toast(e.message, true);
    }
}

async function compartilharProjeto() {
    if (!projetoAtual) return;
    if (!podeAdministrarProjeto()) return;
    document.getElementById('inputCompartilharEmail').value = '';
    document.querySelectorAll('[name="nivelAcesso"]').forEach(r => {
        r.checked = r.value === '1';
        r.nextElementSibling.classList.toggle('active', r.value === '1');
    });
    modalCompartilharBootstrap.show();
}

async function confirmarCompartilhar() {
    const email = document.getElementById('inputCompartilharEmail').value.trim();
    if (!email) { toast('Informe o e-mail do usuário.', true); return; }
    const nivelAcesso = parseInt(document.querySelector('[name="nivelAcesso"]:checked').value, 10);
    try {
        await handleRes(await API.compartilharProjeto(projetoAtual.id, { email, nivelAcesso }), 'Convite enviado.');
        modalCompartilharBootstrap.hide();
    } catch (e) { toast(e.message, true); }
}

async function carregarConvitesPendentes() {
    try {
        const salvos = JSON.parse(localStorage.getItem('convitesPendentes') || '[]');
                convitesPendentes = salvos.length ? salvos : await handleRes(await API.listarConvites(true));
        localStorage.setItem('convitesPendentes', JSON.stringify(convitesPendentes));
    } catch {
        convitesPendentes = [];
    }

    renderConvites();
}

function renderConvites() {
    const count = document.getElementById('conviteCount');
    const list = document.getElementById('conviteList');
    count.textContent = convitesPendentes.length;
    list.innerHTML = '';

    if (!convitesPendentes.length) {
        list.innerHTML = '<p class="invite-meta mb-0">Nenhum convite pendente.</p>';
        return;
    }

    convitesPendentes.forEach(convite => {
        const projeto = convite.projetoDescricao || ('Projeto #' + convite.projetoId);
        const item = document.createElement('div');
        item.className = 'invite-item';
        item.innerHTML =
            '<div><p class="invite-title">' + escapeHtml(projeto) + '</p>' +
            '<p class="invite-meta">Acesso: ' + escapeHtml(convite.nivelAcesso || '') + '</p></div>' +
            '<div class="invite-actions">' +
            '<button type="button" class="btn btn-sm btn-primary" data-accept="' + convite.id + '">Aceitar</button>' +
            '<button type="button" class="btn btn-sm btn-outline-secondary" data-decline="' + convite.id + '">Recusar</button>' +
            '</div>';
        list.appendChild(item);
    });

    list.querySelectorAll('[data-accept]').forEach(btn => {
        btn.addEventListener('click', () => responderConvite(parseInt(btn.dataset.accept, 10), true));
    });
    list.querySelectorAll('[data-decline]').forEach(btn => {
        btn.addEventListener('click', () => responderConvite(parseInt(btn.dataset.decline, 10), false));
    });
}

async function responderConvite(conviteId, aceitou) {
    try {
        if (aceitou) {
            await handleRes(await API.aceitarConvite(conviteId), 'Convite aceito.');
        } else {
            await handleRes(await API.recusarConvite(conviteId), 'Convite recusado.');
        }

        convitesPendentes = convitesPendentes.filter(c => c.id !== conviteId);
        localStorage.setItem('convitesPendentes', JSON.stringify(convitesPendentes));
        renderConvites();
        invalidarCacheProjetos();
        boardCache.clear();
        await carregarProjetos();
    } catch (e) {
        toast(e.message, true);
    }
}

document.getElementById('btnNovoProjeto').addEventListener('click', () => {
    projetoEditandoId = null;
    document.getElementById('modalProjetoTitle').textContent = 'Novo projeto';
    document.getElementById('btnSalvarProjeto').textContent = 'Criar projeto';
    document.getElementById('inputProjetoDesc').value = '';
    modalProjetoBootstrap.show();
});

document.getElementById('btnNovaAtividade').addEventListener('click', criarAtividade);
document.getElementById('btnCompartilhar').addEventListener('click', compartilharProjeto);
document.getElementById('btnConvites').addEventListener('click', () => {
    renderConvites();
    modalConvitesBootstrap.show();
});
document.getElementById('btnSalvarAtividade').addEventListener('click', salvarAtividadeModal);
document.getElementById('btnConfirmarCompartilhar').addEventListener('click', confirmarCompartilhar);
document.getElementById('btnConfirmarRemoverMembro').addEventListener('click', async () => {
    const email = membroRemocaoEmail;
    if (!email) {
        toast('Selecione um participante.', true);
        return;
    }

    try {
        await handleRes(await API.removerMembroProjeto(projetoContextoId || projetoAtual.id, email), 'Participante removido.');
        modalRemoverMembroBootstrap.hide();
        invalidarCacheBoard(projetoContextoId || projetoAtual.id);
        await carregarBoardProjeto(projetoAtual.id);
    } catch (e) {
        toast(e.message, true);
    }
});

document.querySelectorAll('[name="nivelAcesso"]').forEach(radio => {
    radio.addEventListener('change', () => {
        document.querySelectorAll('[name="nivelAcesso"]').forEach(r => {
            r.nextElementSibling.classList.toggle('active', r.checked);
        });
    });
});

document.getElementById('btnSalvarProjeto').addEventListener('click', async () => {
    const desc = document.getElementById('inputProjetoDesc').value.trim();
    if (!desc) {
        toast('Digite um nome para o projeto.', true);
        return;
    }
    try {
        if (projetoEditandoId) {
            const atualizado = await handleRes(await API.atualizarProjeto(projetoEditandoId, desc), 'Projeto atualizado.');
            projetos = projetos.map(p => p.id === atualizado.id ? atualizado : p);
            if (projetoAtual && projetoAtual.id === atualizado.id) {
                projetoAtual = atualizado;
            }
            modalProjetoBootstrap.hide();
            projetoEditandoId = null;
            invalidarCacheProjetos();
            renderListaProjetos();
            await carregarBoardProjeto(atualizado.id);
        } else {
            const created = await handleRes(await API.criarProjeto(desc), 'Projeto criado.');
            modalProjetoBootstrap.hide();
            projetos.push(created);
            projetoAtual = created;
            invalidarCacheProjetos();
            salvarCacheBoard(created.id, [], []);
            renderListaProjetos();
            await carregarBoardProjeto(created.id);
        }
    } catch (e) {
        toast(e.message, true);
    }
});

document.getElementById('btnSalvarTarefa').addEventListener('click', salvarTarefaModal);
document.getElementById('btnExcluirTarefa').addEventListener('click', () => {
    const id = parseInt(document.getElementById('tarefaId').value, 10);
    if (id && atividadeAtualParaNovaTarefa && confirm('Excluir esta tarefa?')) {
        excluirTarefa(atividadeAtualParaNovaTarefa, id);
    }
});

let modalAtividadeBootstrap = null;
let modalCompartilharBootstrap = null;
let modalConvitesBootstrap = null;
let modalRemoverMembroBootstrap = null;

function inicializarMainScreen() {
    modalTarefaBootstrap = new bootstrap.Modal(document.getElementById('modalTarefa'));
    modalProjetoBootstrap = new bootstrap.Modal(document.getElementById('modalProjeto'));
    modalAtividadeBootstrap = new bootstrap.Modal(document.getElementById('modalAtividade'));
    modalCompartilharBootstrap = new bootstrap.Modal(document.getElementById('modalCompartilhar'));
    modalConvitesBootstrap = new bootstrap.Modal(document.getElementById('modalConvites'));
    modalRemoverMembroBootstrap = new bootstrap.Modal(document.getElementById('modalRemoverMembro'));
    carregarProjetos()
        .then(carregarConvitesPendentes)
        .catch(e => toast(e.message, true));
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', inicializarMainScreen, { once: true });
} else {
    inicializarMainScreen();
}

document.addEventListener('click', (event) => {
    if (!event.target.closest('#contextMenu')) {
        fecharContextMenu();
    }
});

document.addEventListener('keydown', (event) => {
    if (event.key === 'Escape') {
        fecharContextMenu();
    }
});

window.addEventListener('scroll', fecharContextMenu, true);
async function carregarUsuario() {
    try {
        const response = await API.obterPerfil(true);
        if (response.status === 401) {
            window.location.href = '/index.html';
            return;
        }

        const perfil = await response.json();
        localStorage.setItem('nomeUsuario', perfil.nome || '');
        localStorage.setItem('fotoPerfilUrl', perfil.fotoPerfilUrl || '');
        document.getElementById('NameUser').innerText = perfil.nome || '';
        renderHeaderProfile(perfil.nome, perfil.fotoPerfilUrl);
    } catch (error) {
        console.error('Erro ao carregar usuario:', error);
    }
}

carregarUsuario();
