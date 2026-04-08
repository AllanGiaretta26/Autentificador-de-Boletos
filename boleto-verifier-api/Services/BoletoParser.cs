using BoletoVerifier.Models;

namespace BoletoVerifier.Services;

// Essa API tem uma única responsabilidade: receber o código de barras e devolver um BoletoInfo.
public class BoletoParser
{
    public BoletoInfo Extrair(string codigo)
    {
        // Remove espaços e pontos que o usuário possa ter digitado
        var codigoLimpo = codigo.Replace(" ", "").Replace(".", "").Replace("-", "");

        return new BoletoInfo
        {
            CodigoOriginal = codigoLimpo,
            Banco = ExtrairBanco(codigoLimpo),
            Valor = ExtrairValor(codigoLimpo),
            Vencimento = ExtrairVencimento(codigoLimpo)
        };
    }

    private string ExtrairBanco(string codigo)
    {
        // Os 3 primeiros dígitos identificam o banco
        if (codigo.Length < 3) return "Desconhecido";

        return codigo[..3] switch
        {
            "001" => "Banco do Brasil",
            "033" => "Santander",
            "104" => "Caixa Econômica Federal",
            "237" => "Bradesco",
            "341" => "Itaú",
            "756" => "Sicoob",
            _ => $"Banco {codigo[..3]}"
        };
    }

    private decimal ExtrairValor(string codigo)
    {
        // No código de barras, as posições 9 a 19 contêm o valor
        if (codigo.Length < 19) return 0;

        var valorStr = codigo.Substring(9, 10);
        if (decimal.TryParse(valorStr, out var valor))
            return valor / 100; // últimos 2 dígitos são centavos

        return 0;
    }

    private string ExtrairVencimento(string codigo)
    {
        // Posições 5 a 8 contêm fator de vencimento (dias desde 07/10/1997)
        if (codigo.Length < 9) return "Não identificado";

        var fatorStr = codigo.Substring(5, 4);
        if (!int.TryParse(fatorStr, out var fator) || fator == 0)
            return "Sem vencimento";

        var dataBase = new DateTime(1997, 10, 7);
        var vencimento = dataBase.AddDays(fator);
        return vencimento.ToString("dd/MM/yyyy");
    }
}