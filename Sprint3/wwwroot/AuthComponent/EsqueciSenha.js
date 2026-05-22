const form = document.getElementById('forgotForm');
        const message = document.getElementById('message');
        const submitBtn = document.getElementById('submitBtn');

        function showMessage(type, text) {
            message.className = 'alert ' + (type === 'error' ? 'alert-danger' : 'alert-success');
            message.textContent = text;
            message.style.display = 'block';
        }

        form.addEventListener('submit', async (e) => {
            e.preventDefault();
            submitBtn.disabled = true;
            submitBtn.textContent = 'Enviando...';

            try {
                const response = await fetch('/api/Auth/esqueci-senha', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email: document.getElementById('email').value.trim() })
                });
                const data = await response.json().catch(() => ({}));
                if (!response.ok) throw new Error(data.message || 'NÃ£o foi possÃ­vel enviar o link.');
                showMessage('success', data.message || 'Verifique seu e-mail.');
            } catch (error) {
                showMessage('error', error.message);
            } finally {
                submitBtn.disabled = false;
                submitBtn.textContent = 'Enviar link';
            }
        });
