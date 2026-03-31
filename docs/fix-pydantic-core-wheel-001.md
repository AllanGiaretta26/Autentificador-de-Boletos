# Fix pip: pydantic-core metadata-generation-failed (Windows Python 3.14)

## Visao geral
Foi identificado um erro durante o `pip install -r requirements.txt` relacionado ao `pydantic-core`, que estava tentando compilar extensoes em vez de instalar via wheel (binario).

A correcao aplicada foi ajustar a versao do `pydantic` no `requirements.txt`, garantindo que o `pydantic-core` resolvido tenha wheel compativel com `cp314` (Python 3.14).

## Erro observado
Durante a instalacao, o pip falhou em `pydantic-core` com:

```text
error: metadata-generation-failed
...
Preparing metadata (pyproject.toml): finished with status 'error'
...
Cargo, the Rust package manager, is not installed or is not on PATH.
This package requires Rust and Cargo to compile extensions.
```

## Ambiente (onde ocorreu)
- Sistema: Windows (win32 10.0.26200)
- Python: `3.14.2`
- pip: `26.0.1`

Com essa combinacao, a versao fixada anteriormente em `requirements.txt` forjava a instalacao de:
- `pydantic==2.9.2` -> `pydantic-core==2.23.4`

Para esse conjunto, nao havia wheel para `cp314`, entao o pip tentou construir a partir do fonte, exigindo `Rust/Cargo`.

## Analise e causa raiz
1. O `pydantic` fixado em `2.9.2` depende de um `pydantic-core` especifico (na instalacao, `2.23.4`).
2. No Python `3.14` (tag `cp314`), nao existia wheel para `pydantic-core==2.23.4`.
3. Sem wheel disponivel, o pip tenta compilar via `pyproject.toml`.
4. Como `cargo` nao estava disponivel no `PATH`, a compilacao falhou.

## Feedback / opcoes de resolucao
Existem 2 caminhos praticos:
1. Usar uma versao de Python compativel com wheels (ex.: `3.12`), ou
2. Garantir que o conjunto de versoes resolvido para `pydantic-core` tenha wheel para `cp314`, evitando compilacao.

Neste caso, a opcao (2) foi aplicada via ajuste no `requirements.txt`.

## Alteracao aplicada (corrigindo dependencias)
Arquivo alterado:
- `requirements.txt`

Mudanca:
- De: `pydantic==2.9.2`
- Para: `pydantic==2.12.5`

Resultado esperado:
- O `pip` passa a resolver `pydantic-core` com versao que possui wheel para `cp314` (na instalacao, foi resolvido para `pydantic-core==2.41.5`).
- Assim, `pip install -r requirements.txt` instala via binario e nao tenta usar Rust/Cargo.

## Como validar (comandos)
1. Verificar versao do Python e pip:

```powershell
python -V
python -m pip -V
```

2. Instalar dependencias:

```powershell
python -m pip install -r "requirements.txt"
```

3. (Opcional) Testar resolucao sem instalar de fato:

```powershell
python -m pip install --dry-run "fastapi==0.115.0" "uvicorn[standard]==0.30.6" "python-dateutil==2.9.0"
```

## Tecnologias e estrutura do projeto
Tecnologias utilizadas no projeto:
- Python
- FastAPI (API REST)
- Uvicorn (servidor ASGI)
- Pydantic (validacao de schema)
- Docker (containerizacao)
- Azure (deploy via script)

Estrutura:
- `app/main.py`: endpoints do FastAPI (`/validar-boleto` e `/health`)
- `app/validator.py`: logica de validacao de boletos
- `Dockerfile`: imagem usando `python:3.12-slim`
- `scripts.ps1`: comandos para build e deploy no Azure

## Dependencias (referencia do requirements.txt)
- `fastapi==0.115.0`
- `uvicorn[standard]==0.30.6`
- `pydantic==2.12.5`
- `python-dateutil==2.9.0`

## Alternativa: rodar via Docker (evita problema local de Python)
Como o `Dockerfile` ja usa `python:3.12-slim`, o build do container nao depende do Python local.

Comandos:
```powershell
docker build -t boleto-verifier .
docker run -p 8000:8000 boleto-verifier
```

Endpoints:
- API: `http://localhost:8000`
- Docs Swagger: `http://localhost:8000/docs`

