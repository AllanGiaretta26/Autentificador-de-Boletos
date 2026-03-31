# Atualizacao README + .gitignore

## Visao geral
Foram realizadas duas atualizacoes finais no projeto:
- Inclusao dos assets no `README.md`.
- Reestruturacao do `.gitignore` com comentarios e entradas para arquivos/pastas nao essenciais.

## 1) README.md atualizado com assets
Foi adicionada a secao `# Assets` ao final do `README.md` com as capturas de tela existentes na pasta `assets/`.

Arquivos referenciados:
- `assets/Captura de tela 2026-03-31 104240.png`
- `assets/Captura de tela 2026-03-31 130205.png`
- `assets/Captura de tela 2026-03-31 130234.png`

Observacao tecnica:
- Os caminhos no Markdown usam `%20` para espacos no nome dos arquivos, evitando problemas de renderizacao.

## 2) .gitignore revisado e comentado
O `.gitignore` foi corrigido (antes estava com conteudo de comando) e organizado por categorias:

- Python cache e bytecode
  - `__pycache__/`, `*.py[cod]`, `*$py.class`
- Ambientes virtuais locais
  - `.venv/`, `venv/`, `env/`, `ENV/`
- Arquivos de ambiente/segredos locais
  - `.env`, `.env.*`, excecao `!.env.example`
- Logs e artefatos de execucao
  - `*.log`, `*.pid`
- Cobertura/testes/cache
  - `.coverage*`, `htmlcov/`, `.pytest_cache/`, `.mypy_cache/`, `.ruff_cache/`
- Build/distribuicao
  - `build/`, `dist/`, `*.egg-info/`, `.eggs/`
- Configuracoes locais de IDE
  - `.vscode/`, `.idea/`

## Estrutura e tecnologia (contexto)
- Linguagem: Python
- API: FastAPI
- Servidor: Uvicorn
- Containerizacao: Docker
- Deploy: Azure (scripts em PowerShell)

## Comandos de terminal utilizados nesta etapa
```powershell
ls
ls "assets"
```

## Resultado final
- `README.md` com secao visual de assets.
- `.gitignore` limpo, comentado e pronto para evitar versionamento de arquivos locais/temporarios.