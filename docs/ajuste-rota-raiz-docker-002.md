# Ajuste da rota raiz (404 em GET /) ao rodar via Docker

## Visao geral
Ao executar o comando:

```powershell
docker run -p 8000:8000 boleto-verifier
```

o container subia corretamente, mas os logs mostravam:

```text
INFO:     Uvicorn running on http://0.0.0.0:8000 (Press CTRL+C to quit)
INFO:     172.17.0.1:46852 - "GET / HTTP/1.1" 404 Not Found
INFO:     172.17.0.1:46852 - "GET /favicon.ico HTTP/1.1" 404 Not Found
```

Ou seja, o servidor estava saudavel, mas nao existia rota para `/` (raiz) nem para `/favicon.ico`, o que gerava 404 ao abrir `http://localhost:8000/` no navegador.

## Causa
- A aplicacao FastAPI definia apenas:
  - `POST /validar-boleto`
  - `GET /health`
- Nao havia um handler para `GET /`, portanto o comportamento esperado pelo FastAPI/Uvicorn e responder com HTTP 404.
- O acesso via navegador tenta automaticamente `/` e `/favicon.ico`, o que explica as entradas de log.

Importante: isso nao era uma falha de inicializacao ou de dependencia da API, apenas ausencia de rota raiz.

## Ajuste implementado
Arquivo modificado:
- `app/main.py`

Foi adicionada uma rota raiz simples:

```python
@app.get("/")
async def root():
    # Endpoint raiz simples para evitar 404 e indicar como usar o serviço.
    return {
        "mensagem": "Boleto Verifier Service está em execução.",
        "docs_url": "/docs",
        "health_url": "/health",
        "validar_boleto_url": "/validar-boleto"
    }
```

E os endpoints ja existentes foram mantidos:
- `POST /validar-boleto` (principal para validacao de boleto)
- `GET /health` (checagem de saude)

Com isso, acessar `http://localhost:8000/` retorna um JSON descritivo em vez de 404, e o log fica mais limpo para uso normal.

## Passos executados com Docker

1. Parar o container antigo:

```powershell
docker ps
docker stop <nome-ou-id-do-container>
```

No caso observado:

```powershell
docker stop silly_haslett
```

2. Rebuild da imagem com o novo codigo:

```powershell
docker build -t boleto-verifier .
```

3. Subir o novo container:

```powershell
docker run -d -p 8000:8000 boleto-verifier
```

4. Testar os endpoints (exemplos):

No navegador ou via ferramenta HTTP (ex.: curl, Postman, Thunder Client):
- `GET http://localhost:8000/` -> JSON com mensagem e links (`docs_url`, `health_url`, `validar_boleto_url`)
- `GET http://localhost:8000/health` -> `{"status": "ok"}`
- `GET http://localhost:8000/docs` -> interface Swagger da API

## Sobre o uso de .venv
O erro relatado estava relacionado apenas ao comportamento dentro do container Docker (rota inexistente), nao a falta de bibliotecas no ambiente virtual local.

Mesmo assim, se for necessario garantir as dependencias no `.venv`, o fluxo recomendado e:

```powershell
python -m venv .venv
.\.venv\Scripts\activate
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
```

Isso instala as mesmas dependencias usadas no Docker (`fastapi`, `uvicorn[standard]`, `pydantic`, `python-dateutil`) dentro do ambiente virtual, mantendo o desenvolvimento local alinhado com o container.

