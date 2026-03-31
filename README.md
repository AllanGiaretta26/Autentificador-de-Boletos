# Boleto Verifier Service

---

# Assets

Abaixo estao algumas capturas de tela que mostram o fluxo do projeto em ambiente real (container + testes de API):

1. **API em execucao no container:**  
   Mostra o container ativo com o Uvicorn inicializado e a aplicacao pronta para receber requisicoes HTTP.

   ![API em execucao no container](assets/Captura%20de%20tela%202026-03-31%20130205.png)

2. **Teste de endpoints da API:**  
   Exibe o teste de chamada HTTP para validacao, comprovando o retorno da API durante a execucao.

   ![Teste de endpoints da API](assets/Captura%20de%20tela%202026-03-31%20130234.png)

3. **Execucao inicial da API:**  
   Registra a etapa de inicializacao do servico, com o servidor em estado pronto para atendimento.

   ![Execucao inicial da API](assets/Captura%20de%20tela%202026-03-31%20104240.png)

API para **validação e autenticação de boletos bancários brasileiros**, construída com **FastAPI** e preparada para execução em **Docker** e deploy no **Azure**.

O serviço valida a linha digitável de boletos, verifica os dígitos verificadores e extrai informações relevantes como banco, valor e data de vencimento.

---

# Funcionalidades

* Validação da **linha digitável de boletos bancários**
* Verificação dos **3 blocos pelo algoritmo Módulo 10**
* Validação do **dígito geral pelo Módulo 11**
* Reconstrução do **código de barras (44 dígitos)**
* Extração de:

  * banco
  * valor
  * data de vencimento
* API REST construída com **FastAPI**
* Execução containerizada com **Docker**
* Script para **deploy automatizado no Azure**

---

# Como Funciona

O núcleo da lógica está em:

```
app/validator.py
```

A função principal é:

```python
def validar_boleto(linha_digitavel: str):
    linha = ''.join(c for c in linha_digitavel if c.isdigit())

    # valida os 3 blocos com módulo 10
    # reconstrói o código de barras
    # valida o dígito geral com módulo 11
    # extrai banco, valor e data de vencimento
```

A API expõe o endpoint definido em:

```
app/main.py
```

```python
@app.post("/validar-boleto")
async def validar(request: BoletoRequest):
    valido, resultado = validar_boleto(request.linha_digitavel)

    if valido:
        return {
            "valido": True,
            "dados": resultado,
            "mensagem": "Boleto autenticado com sucesso!"
        }

    raise HTTPException(status_code=400, detail=resultado["erro"])
```

---

# Endpoints

### Validar boleto

```
POST /validar-boleto
```

Valida uma linha digitável de boleto.

### Health check

```
GET /health
```

Usado para verificar se a API está funcionando.

---

# Exemplo de Requisição

```bash
curl -X POST http://localhost:8000/validar-boleto \
  -H "Content-Type: application/json" \
  -d '{"linha_digitavel":"23793381286000782713695000063305975520000370000"}'
```

---

# Exemplo de Resposta (sucesso)

```json
{
  "valido": true,
  "dados": {
    "banco": "237",
    "valor": 3700.0,
    "data_vencimento": "11/06/2018",
    "codigo_barras": "23799755200003700003381260007827139500006330",
    "linha_digitavel": "23793381286000782713695000063305975520000370000"
  },
  "mensagem": "Boleto autenticado com sucesso!"
}
```

---

# Exemplo de Resposta (erro)

```json
{
  "detail": "Dígito verificador do bloco 1 inválido"
}
```

---

# Rodando o projeto localmente

### 1. Build da imagem Docker

```bash
docker build -t boleto-verifier .
```

### 2. Executar o container

```bash
docker run -p 8000:8000 boleto-verifier
```

A API ficará disponível em:

```
http://localhost:8000
```

Documentação automática da API:

```
http://localhost:8000/docs
```

---

# Scripts de Automação

O arquivo `scripts.ps1` reúne comandos para:

* build da imagem Docker
* execução local
* deploy no Azure

Para executar:

```powershell
.\scripts.ps1
```

---

# Deploy no Azure

## Pré-requisitos

* Azure CLI instalado
* Login realizado com:

```bash
az login
```

O script automatiza:

1. Criação do **Resource Group**
2. Criação do **Azure Container Registry (ACR)**
3. Build e push da imagem Docker
4. Criação do **Azure Container Instance (ACI)**
5. Obtenção da **URL pública da API**

---

# Estrutura do Projeto

```
app/
 ├─ main.py        # API FastAPI
 └─ validator.py   # Lógica de validação do boleto

assets/            # Capturas de tela usadas no README
docs/              # Documentacao das analises e ajustes

scripts.ps1        # Automação de build e deploy
Dockerfile         # Container da aplicação
requirements.txt   # Dependencias Python do projeto
.gitignore         # Regras de arquivos/pastas nao essenciais
README.md
```

---

# Tecnologias Utilizadas

* Python
* FastAPI
* Uvicorn
* Pydantic
* python-dateutil
* Docker
* PowerShell
* Azure CLI
* Azure Container Instances
* Azure Container Registry
