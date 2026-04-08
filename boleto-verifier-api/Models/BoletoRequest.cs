namespace BoletoVerifier.Models;

// Models são classes simples que representam os dados que trafegam na API.
// O que a API recebe do formulário:
public class BoletoRequest
{
    public string CodigoBarras { get; set; } = string.Empty;
    public string EmailDestino { get; set; } = string.Empty;
}