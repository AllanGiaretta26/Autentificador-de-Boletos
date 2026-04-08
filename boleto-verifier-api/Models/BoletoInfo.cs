namespace BoletoVerifier.Models;

// o que a API extrai do código de barras:
public class BoletoInfo
{
    public string Banco { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Vencimento { get; set; } = string.Empty;
    public string CodigoOriginal { get; set; } = string.Empty;
}