const form = document.getElementById('profileForm');
        const nomeInput = document.getElementById('nome');
        const emailBox = document.getElementById('email');
        const fotoInput = document.getElementById('foto');
        const avatarPreview = document.getElementById('avatarPreview');
        const messageBox = document.getElementById('profileMessage');
        const saveBtn = document.getElementById('saveBtn');

        function showMessage(type, text) {
            messageBox.className = 'alert alert-' + type;
            messageBox.textContent = text;
            messageBox.style.display = 'block';
        }

        function renderAvatar(nome, fotoUrl) {
            avatarPreview.innerHTML = '';
            if (fotoUrl) {
                const img = document.createElement('img');
                img.src = fotoUrl;
                img.alt = nome || 'Foto do usuario';
                avatarPreview.appendChild(img);
                return;
            }

            avatarPreview.textContent = (nome || 'U').trim().charAt(0).toUpperCase() || 'U';
        }

        async function carregarPerfil() {
            const response = await fetch('/api/Usuarios/me', { credentials: 'include' });
            if (response.status === 401) {
                window.location.href = '/index.html';
                return;
            }

            const perfil = await response.json();
            nomeInput.value = perfil.nome || '';
            emailBox.textContent = perfil.email || '';
            localStorage.setItem('nomeUsuario', perfil.nome || '');
            localStorage.setItem('fotoPerfilUrl', perfil.fotoPerfilUrl || '');
            renderAvatar(perfil.nome, perfil.fotoPerfilUrl);
        }

        fotoInput.addEventListener('change', () => {
            const file = fotoInput.files && fotoInput.files[0];
            if (!file) {
                renderAvatar(nomeInput.value, localStorage.getItem('fotoPerfilUrl'));
                return;
            }

            renderAvatar(nomeInput.value, URL.createObjectURL(file));
        });

        nomeInput.addEventListener('input', () => {
            if (!fotoInput.files.length) {
                renderAvatar(nomeInput.value, localStorage.getItem('fotoPerfilUrl'));
            }
        });

        form.addEventListener('submit', async (event) => {
            event.preventDefault();
            messageBox.style.display = 'none';

            const formData = new FormData();
            formData.append('nome', nomeInput.value.trim());
            if (fotoInput.files && fotoInput.files[0]) {
                formData.append('foto', fotoInput.files[0]);
            }

            saveBtn.disabled = true;
            saveBtn.textContent = 'Salvando...';

            try {
                const response = await fetch('/api/Usuarios/me', {
                    method: 'PUT',
                    credentials: 'include',
                    body: formData
                });

                const payload = await response.json().catch(() => ({}));
                if (!response.ok) {
                    throw new Error(payload.message || 'Nao foi possivel salvar o perfil.');
                }

                localStorage.setItem('nomeUsuario', payload.nome || '');
                localStorage.setItem('fotoPerfilUrl', payload.fotoPerfilUrl || '');
                renderAvatar(payload.nome, payload.fotoPerfilUrl);
                fotoInput.value = '';
                showMessage('success', 'Perfil atualizado com sucesso.');
            } catch (error) {
                showMessage('danger', error.message);
            } finally {
                saveBtn.disabled = false;
                saveBtn.textContent = 'Salvar alteraÃ§Ãµes';
            }
        });

        carregarPerfil().catch(() => showMessage('danger', 'Nao foi possivel carregar o perfil.'));
