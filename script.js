document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('boleto-form');
    const codigoBarras = document.getElementById('codigo-barras');
    const email = document.getElementById('email');
    const codigoError = document.getElementById('codigo-error');
    const btnEnviar = document.getElementById('btn-enviar');

    // Permitir apenas números no campo de código de barras
    codigoBarras.addEventListener('input', function (e) {
        e.target.value = e.target.value.replace(/\D/g, '');
        codigoError.textContent = '';
    });

    // Validação e envio do formulário
    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const codigo = codigoBarras.value.trim();
        const emailValue = email.value.trim();
        let isValid = true;
        let errorMessage = '';

        // Validação do código de barras (47 ou 48 caracteres)
        if (codigo.length !== 47 && codigo.length !== 48) {
            isValid = false;
            errorMessage = 'O código de barras deve ter 47 ou 48 dígitos.';
        }

        // Validação do e-mail
        if (!emailValue || !validateEmail(emailValue)) {
            isValid = false;
            if (!errorMessage) errorMessage = 'Por favor, insira um e-mail válido.';
        }

        if (!isValid) {
            codigoError.textContent = errorMessage;
            return;
        }

        // Desabilita o botão enquanto aguarda a resposta
        btnEnviar.disabled = true;
        btnEnviar.textContent = 'Enviando...';
        codigoError.textContent = '';

        try {
                const response = await fetch('https://boleto-verifier-api.azurewebsites.net/api/boleto', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    codigoBarras: codigo,
                    emailDestino: emailValue
                })
            });

            const data = await response.json();

            if (response.ok) {
                exibirSucesso(data.dados);
                form.reset();
            } else {
                codigoError.textContent = data.erro || 'Erro ao processar o boleto.';
            }
        } catch (error) {
            codigoError.textContent = 'Não foi possível conectar à API. Verifique se ela está rodando.';
        } finally {
            // Reabilita o botão independente do resultado
            btnEnviar.disabled = false;
            btnEnviar.textContent = 'Enviar Boleto';
        }
    });

    function exibirSucesso(dados) {
        codigoError.style.color = 'green';
        codigoError.textContent = `✅ E-mail enviado! Banco: ${dados.banco} | Valor: R$ ${dados.valor.toFixed(2)} | Vencimento: ${dados.vencimento}`;
    }

    function validateEmail(email) {
        const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return re.test(email);
    }
});