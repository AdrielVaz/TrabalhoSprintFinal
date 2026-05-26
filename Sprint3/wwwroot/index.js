function togglePassword() {
            const passwordInput = document.getElementById('password');
            const toggleIcon = document.querySelector('.toggle-password');

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.innerHTML = '<i class="bi bi-eye-slash"></i>';
            } else {
                passwordInput.type = 'password';
                toggleIcon.innerHTML = '<i class="bi bi-eye"></i>';
            }
        }

        function showMessage(type, message) {
            const errorDiv = document.getElementById('errorMessage');
            const successDiv = document.getElementById('successMessage');
            const resendDiv = document.getElementById('resendConfirmation');

            if (type === 'error') {
                errorDiv.textContent = message;
                errorDiv.style.display = 'block';
                successDiv.style.display = 'none';
                resendDiv.style.display = message.toLowerCase().includes('confirme seu email') ? 'block' : 'none';

                setTimeout(() => {
                    errorDiv.style.display = 'none';
                }, 3000);
            } else if (type === 'success') {
                successDiv.textContent = message;
                successDiv.style.display = 'block';
                errorDiv.style.display = 'none';
                resendDiv.style.display = 'none';
            }
        }

        function validateEmail(email) {
            const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        }

        function validatePassword(password) {
            return password.length >= 6;
        }

        document.getElementById('loginForm').addEventListener('submit', async function (e) {
            e.preventDefault();

            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;
            const remember = document.getElementById('remember').checked;
            const submitBtn = document.querySelector('#loginForm button[type="submit"]');

            if (!email || !password) {
                showMessage('error', 'Por favor, preencha todos os campos!');
                return;
            }

            if (!validateEmail(email)) {
                showMessage('error', 'Por favor, insira um email valido!');
                return;
            }

            if (!validatePassword(password)) {
                showMessage('error', 'A senha deve ter pelo menos 6 caracteres!');
                return;
            }

            submitBtn.disabled = true;
            submitBtn.textContent = 'Entrando...';

            try {
                const response = await fetch('/api/Auth/Login', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    credentials: 'include',
                    body: JSON.stringify({
                        email: email,
                        senha: password
                    })
                });

                let payload = {};
                try {
                    payload = await response.json();

                } catch {
                    payload = {};
                }

                if (!response.ok) {
                    throw new Error(payload.message || 'Falha no login.');
                }

                showMessage('success', 'Login realizado com sucesso!');   
                localStorage.setItem('nomeUsuario', payload.nome || '');
                localStorage.setItem('fotoPerfilUrl', payload.fotoPerfilUrl || '');
                localStorage.setItem('convitesPendentes', JSON.stringify(payload.convitesPendentes || []));

                if (remember) {
                    localStorage.setItem('rememberedEmail', email);
                    localStorage.setItem('rememberMe', 'true');
                } else {
                    localStorage.removeItem('rememberedEmail');
                    localStorage.removeItem('rememberMe');
                }

                setTimeout(() => {
                    window.location.href = '/mainscreen';
                    console.log('Usuario logado:', payload);
                }, 1000);
            } catch (error) {
                showMessage('error', error.message);
            } finally {
                submitBtn.disabled = false;
                submitBtn.textContent = 'Entrar';
            }
        });

        async function reenviarConfirmacao(email) {
            const response = await fetch('/api/Auth/reenviar-confirmacao', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email })
            });
            const payload = await response.json().catch(() => ({}));
            if (!response.ok) throw new Error(payload.message || 'Não foi possível reenviar a confirmação.');
            showMessage('success', payload.message || 'Verifique seu email.');
        }

        document.getElementById('btnResendConfirmation').addEventListener('click', async () => {
            const email = document.getElementById('email').value.trim();
            if (!email) {
                showMessage('error', 'Informe o email para reenviar a confirmação.');
                return;
            }

            try {
                await reenviarConfirmacao(email);
            } catch (error) {
                showMessage('error', error.message);
            }
        });

        window.addEventListener('load', function () {
            const rememberMe = localStorage.getItem('rememberMe');
            const rememberedEmail = localStorage.getItem('rememberedEmail');

            if (rememberMe === 'true' && rememberedEmail) {
                document.getElementById('email').value = rememberedEmail;
                document.getElementById('remember').checked = true;
            }
        });

        const inputs = document.querySelectorAll('input');
        inputs.forEach(input => {features
            input.addEventListener('focus', function () {
                this.parentElement.style.transform = 'scale(1.02)';
            });

            input.addEventListener('blur', function () {
                this.parentElement.style.transform = 'scale(1)';
            });
        });
