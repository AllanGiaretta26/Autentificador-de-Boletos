from datetime import datetime, timedelta
from typing import Dict, Tuple

# Calcula o dígito verificador pelo algoritmo Módulo 10 (pesos 2 e 1).
def calcular_modulo10(campo: str) -> int:
    soma = 0
    peso = 2
    for digito in reversed(campo):
        temp = int(digito) * peso
        soma += temp // 10 + temp % 10
        peso = 1 if peso == 2 else 2
    resto = soma % 10
    return 0 if resto == 0 else 10 - resto

def validar_modulo10(campo: str, dv: str) -> bool:
    return calcular_modulo10(campo) == int(dv)

# Calcula o dígito verificador pelo algoritmo Módulo 11 (pesos 2..9).
def calcular_modulo11(codigo: str) -> int:
    soma = 0
    peso = 2
    for digito in reversed(codigo):
        soma += int(digito) * peso
        peso += 1
        if peso > 9:
            peso = 2
    resto = soma % 11
    return 0 if resto in (0, 1) else 11 - resto

def validar_boleto(linha_digitavel: str) -> Tuple[bool, Dict]:
    """Valida a linha digitável e retorna (válido, dados ou erro)."""
    linha = ''.join(c for c in linha_digitavel if c.isdigit())
    if len(linha) != 47:
        return False, {"erro": "Linha digitável deve ter exatamente 47 dígitos"}

    # Valida os 3 blocos (Módulo 10) conforme padrão do boleto.
    if not validar_modulo10(linha[0:9], linha[9]):
        return False, {"erro": "Dígito verificador do bloco 1 inválido"}
    if not validar_modulo10(linha[10:20], linha[20]):
        return False, {"erro": "Dígito verificador do bloco 2 inválido"}
    if not validar_modulo10(linha[21:31], linha[31]):
        return False, {"erro": "Dígito verificador do bloco 3 inválido"}

    # Monta o código de barras (44 dígitos) a partir da linha digitável.
    codigo_barras = (linha[0:4] + linha[32] +
                     linha[33:47] + linha[4:9] +
                     linha[10:20] + linha[21:31])

    # Valida o Módulo 11 sobre os 43 dígitos sem o DV geral.
    codigo_para_calculo = codigo_barras[:4] + codigo_barras[5:]
    dv_geral = int(linha[32])
    if calcular_modulo11(codigo_para_calculo) != dv_geral:
        return False, {"erro": "Dígito verificador geral (módulo 11) inválido"}

    # Extrai dados principais do código de barras.
    banco = codigo_barras[0:3]
    fator_vencimento = int(codigo_barras[5:9])
    valor = float(codigo_barras[9:19]) / 100
    # Data base definida pelo padrão (07/10/1997).
    data_base = datetime(1997, 10, 7)
    vencimento = (data_base + timedelta(days=fator_vencimento)).strftime("%d/%m/%Y") if fator_vencimento > 0 else None

    dados = {
        "banco": banco,
        "valor": round(valor, 2),
        "data_vencimento": vencimento,
        "codigo_barras": codigo_barras,
        "linha_digitavel": linha
    }
    return True, dados
