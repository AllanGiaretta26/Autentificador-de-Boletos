from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from app.validator import validar_boleto

# Metadados básicos exibidos na documentação automática (Swagger / OpenAPI).
app = FastAPI(
    title="Boleto Verifier Service",
    description="Serviço de verificação e autenticação de boletos bancários",
    version="1.0.0"
)

# Modelos de entrada e saída para validar o contrato da API.
class BoletoRequest(BaseModel):
    linha_digitavel: str

class BoletoResponse(BaseModel):
    valido: bool
    dados: dict | None = None
    mensagem: str | None = None

@app.post("/validar-boleto", response_model=BoletoResponse)
async def validar(request: BoletoRequest):
    # Normaliza e valida a linha digitável via regras do boleto.
    valido, resultado = validar_boleto(request.linha_digitavel)
    if valido:
        return {"valido": True, "dados": resultado, "mensagem": "Boleto autenticado com sucesso!"}
    else:
        # Erro de validação volta como HTTP 400 com mensagem direta.
        raise HTTPException(status_code=400, detail=resultado["erro"])

@app.get("/health")
async def health():
    # Endpoint simples para checagem de liveness/readiness.
    return {"status": "ok"}
