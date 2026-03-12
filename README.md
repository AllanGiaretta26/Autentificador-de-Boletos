# Boleto Verifier Service

Servico Dockerizado para validar e autenticar boletos bancarios brasileiros via API.

**Resumo**
- API FastAPI com endpoint de validacao de linha digitavel.
- Validacao de blocos (modulo 10), digito geral (modulo 11) e extracao de dados.
- Script pronto para build, execucao local e deploy no Azure.

**Como Funciona**
O nucleo de validacao esta em `app/validator.py`. A funcao `validar_boleto`:
- Normaliza a linha digitavel (mantem apenas digitos).
- Valida os 3 blocos com modulo 10.
- Reconstitui o codigo de barras e valida o digito geral com modulo 11.
- Extrai banco, valor e vencimento.

Exemplo do fluxo no codigo:
```python
def validar_boleto(linha_digitavel: str):
    linha = ''.join(c for c in linha_digitavel if c.isdigit())
    # valida blocos com modulo 10...
    # monta codigo de barras (44 digitos)...
    # valida digito geral com modulo 11...
    # extrai dados (banco, valor, vencimento)...
```

O endpoint esta em `app/main.py`:
```python
@app.post("/validar-boleto")
async def validar(request: BoletoRequest):
    valido, resultado = validar_boleto(request.linha_digitavel)
    if valido:
        return {"valido": True, "dados": resultado, "mensagem": "Boleto autenticado com sucesso!"}
    raise HTTPException(status_code=400, detail=resultado["erro"])
```

**Endpoints**
- `POST /validar-boleto` - valida a linha digitavel.
- `GET /health` - health check.

**Exemplo de Requisicao**
```bash
curl -X POST http://localhost:8000/validar-boleto \
  -H "Content-Type: application/json" \
  -d '{"linha_digitavel": "23793381286000782713695000063305975520000370000"}'
```

**Exemplo de Resposta (sucesso)**
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

**Exemplo de Resposta (erro)**
```json
{
  "detail": "Digito verificador do bloco 1 invalido"
}
```

**Rodar Localmente (Docker)**
```bash
docker build -t boleto-verifier .
docker run -p 8000:8000 boleto-verifier
```

**Rodar Localmente (Python)**
```bash
python -m venv .venv
.\.venv\Scripts\activate
pip install -r requirements.txt
uvicorn app.main:app --host 0.0.0.0 --port 8000 --reload
```

**Scripts Automatizados**
O arquivo `scripts.ps1` reune os comandos principais:
- Build da imagem Docker.
- Execucao local do container.
- Deploy no Azure (Resource Group, ACR, build, ACI e obtencao de URL).

Para executar passo a passo:
```powershell
.\scripts.ps1
```

**Deploy no Azure (via scripts.ps1)**
Pre-requisitos:
- Azure CLI instalado e autenticado (`az login`).
- Permissao para criar recursos no subscription.

Etapas descritas no `scripts.ps1`:
1. Criar Resource Group.
2. Criar Azure Container Registry (ACR).
3. Build e push da imagem no ACR.
4. Criar Azure Container Instance (ACI).
5. Obter a URL publica do servico.

**Estrutura do Projeto**
- `app/main.py` - API FastAPI e rotas.
- `app/validator.py` - regras de validacao e extracao dos dados.
- `scripts.ps1` - automacao de build e deploy.
- `Dockerfile` - container da aplicacao.
