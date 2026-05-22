const params = new URLSearchParams(window.location.search);
        const email = params.get('email') || '';
        const token = params.get('token') || '';
        const form = document.getElementById('resetForm');
        const message = document.getElementById('message');
        const submitBtn = document.getElementById('submitBtn');

        function showMessage(type, text) {
            message.className = 'alert ' + (type === 'error' ? 'alert-danger' : 'alert-success');
            message.textContent = text;
            message.style.display = 'block';
        }

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            const password = document.getElementById('password').value;
            const confirmPassword = document.getElementById('confirmPassword').value;

            if (password !== confirmPassword) {
                showMessage('error', 'As senhas nÃ£o coincidem.');
                return;
            }

            submitBtn.disabled = true;
            submitBtn.textContent = 'Alterando...';

            try {
                const response = await fetch('/api/Auth/redefinir-senha', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, token, novaSenha: password })
                });
                const data = await response.json().catch(() => ({}));
                if (!response.ok) throw new Error(data.message || 'NÃ£o foi possÃ­vel alterar a senha.');
                showMessage('success', data.message || 'Senha alterada.');
                form.reset();
            } catch (error) {
                showMessage('error', error.message);
            } finally {
                submitBtn.disabled = false;
                submitBtn.textContent = 'Alterar senha';
            }
        });
