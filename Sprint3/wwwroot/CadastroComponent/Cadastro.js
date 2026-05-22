function togglePassword() {
            const passwordInput = document.getElementById('password');
            const toggleIcon = document.querySelector('.toggle-password');

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                toggleIcon.textContent = 'Ocultar';
            } else {
                passwordInput.type = 'password';
                toggleIcon.textContent = 'Mostrar';
            }
        }

        function showMessage(type, message) {
            const errorDiv = document.getElementById('errorMessage');
            const successDiv = document.getElementById('successMessage');

            if (type === 'error') {
                errorDiv.textContent = message;
                errorDiv.style.display = 'block';
                successDiv.style.display = 'none';

                setTimeout(() => {
                    errorDiv.style.display = 'none';
                }, 3000);
            } else if (type === 'success') {
                successDiv.textContent = message;
                successDiv.style.display = 'block';
                errorDiv.style.display = 'none';
            }
        }

        function checkPasswordStrength(password) {
            let strength = 0;
            let message = '';

            if (password.length === 0) {
                return { strength: 0, message: 'Digite uma senha' };
            }

            if (password.length >= 6) strength++;
            if (password.length >= 8) strength++;
            if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
            if (/[0-9]/.test(password)) strength++;
            if (/[^a-zA-Z0-9]/.test(password)) strength++;

            switch (strength) {
                case 0:
                case 1:
                    message = 'Senha fraca';
                    break;
                case 2:
                case 3:
                    message = 'Senha média';
                    break;
                case 4:
                case 5:
                    message = 'Senha forte';
                    break;
            }

            return { strength: Math.min(strength, 5), message };
        }

        function updatePasswordStrength() {
            const password = document.getElementById('password').value;
            const { strength, message } = checkPasswordStrength(password);
            const strengthBar = document.getElementById('strengthBar');
            const strengthText = document.getElementById('strengthText');

            let width = (strength / 5) * 100;
            let color = '';

            if (strength <= 2) {
                color = '#ff4444';
            } else if (strength <= 3) {
                color = '#ffaa44';
            } else {
                color = '#44ff44';
            }

            strengthBar.style.width = width + '%';
            strengthBar.style.backgroundColor = color;
            strengthText.textContent = message;
            strengthText.style.color = color;
        }

        function validateEmail(email) {
            const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        }

        function validateName(name) {
            return name.trim().length >= 3;
        }

        function validatePassword(password) {
            return password.length >= 6;
        }

        function validateForm() {
            const name = document.getElementById('name').value;
            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;
            const confirmPassword = document.getElementById('confirmPassword').value;
            const terms = document.getElementById('terms').checked;

            if (!validateName(name)) {
                showMessage('error', 'Nome deve ter pelo menos 3 caracteres!');
                return false;
            }

            if (!validateEmail(email)) {
                showMessage('error', 'Por favor, insira um email válido!');
                return false;
            }

            if (!validatePassword(password)) {
                showMessage('error', 'A senha deve ter pelo menos 6 caracteres!');
                return false;
            }

            if (password !== confirmPassword) {
                showMessage('error', 'As senhas não coincidem!');
                return false;
            }

            if (!terms) {
                showMessage('error', 'Você precisa aceitar os termos para continuar!');
                return false;
            }

            return true;
        }

        async function registerUser(userData) {
            const response = await fetch('/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(userData)
            });

            let payload = {};
            try {
                payload = await response.json();
            } catch {
                payload = {};
            }

            if (!response.ok) {
                throw new Error(payload.message || 'Erro ao cadastrar usuario.');
            }

            return payload;
        }

        document.getElementById('registerForm').addEventListener('submit', async function (e) {
            e.preventDefault();

            if (!validateForm()) {
                return;
            }

            const name = document.getElementById('name').value;
            const email = document.getElementById('email').value;
            const password = document.getElementById('password').value;

            const submitBtn = document.getElementById('submitBtn');
            submitBtn.disabled = true;
            submitBtn.textContent = 'Cadastrando...';

            const userData = {
                nome: name,
                email: email,
                senha: password
            };

            try {
                const payload = await registerUser(userData);
                showMessage('success', payload.message || ('Cadastro realizado. Confirme sua conta na caixa de entrada de ' + email + '.'));

                document.getElementById('registerForm').reset();
                updatePasswordStrength();

                setTimeout(() => {
                    window.location.href = '/';
                }, 4500);
            } catch (error) {
                showMessage('error', error.message);
            } finally {
                submitBtn.disabled = false;
                submitBtn.textContent = 'Cadastrar';
            }
        });

        document.getElementById('password').addEventListener('input', updatePasswordStrength);

        document.getElementById('confirmPassword').addEventListener('input', function () {
            const password = document.getElementById('password').value;
            const confirmPassword = this.value;

            if (password !== confirmPassword && confirmPassword.length > 0) {
                this.style.borderColor = '#ff4444';
            } else {
                this.style.borderColor = '#e1e1e1';
            }
        });

        const inputs = document.querySelectorAll('input');
        inputs.forEach(input => {
            input.addEventListener('focus', function () {
                this.parentElement.style.transform = 'scale(1.02)';
            });

            input.addEventListener('blur', function () {
                this.parentElement.style.transform = 'scale(1)';
            });
        });
