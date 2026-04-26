# Boleto Verifier

![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12-239120?logo=csharp)
![Docker](https://img.shields.io/badge/Docker-disponível-2496ED?logo=docker)
![Azure](https://img.shields.io/badge/Azure-hospedado-0078D4?logo=microsoftazure)
![Status](https://img.shields.io/badge/status-concluído-brightgreen)
![Licença](https://img.shields.io/badge/licença-MIT-blue)

> Aplicação web que lê o código de barras de um boleto bancário, extrai banco, valor e vencimento, e envia as informações para um e-mail informado pelo usuário.

**API pausada:** [red-mushroom-0446f0b0f.2.azurestaticapps.net](https://red-mushroom-0446f0b0f.2.azurestaticapps.net)

---

## Descrição

O Boleto Verifier recebe um código de barras de 47 ou 48 dígitos, interpreta os campos de acordo com o padrão FEBRABAN e retorna banco emissor, valor e data de vencimento. As informações extraídas são enviadas automaticamente por e-mail para o endereço indicado pelo usuário.

## Status do Projeto

![Status](https://img.shields.io/badge/status-concluído-brightgreen)

Projeto concluído e hospedado na nuvem.

## Tecnologias

![HTML5](https://img.shields.io/badge/HTML5-E34F26?logo=html5&logoColor=white)
![CSS3](https://img.shields.io/badge/CSS3-1572B6?logo=css3&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?logo=javascript&logoColor=black)
![.NET](https://img.shields.io/badge/.NET_10-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23_12-239120?logo=csharp&logoColor=white)
![MailKit](https://img.shields.io/badge/MailKit-SMTP-lightgrey)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)
![Azure](https://img.shields.io/badge/Azure_Static_Web_Apps-0078D4?logo=microsoftazure&logoColor=white)

| Camada | Tecnologia |
|---|---|
| Frontend | HTML5, CSS3, JavaScript |
| Backend | .NET 10, ASP.NET Core Web API, C# |
| E-mail | MailKit + MimeKit via SMTP Gmail |
| Containerização | Docker (multi-stage build) |
| Hospedagem | Azure App Service + Azure Static Web Apps |

## Como Instalar e Rodar

**Pré-requisitos:** [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Sem Docker

```bash
git clone https://github.com/AllanGiaretta26/boleto-verifier.git
cd boleto-verifier/boleto-verifier-api

dotnet user-secrets set "Email:Remetente" "seuemail@gmail.com"
dotnet user-secrets set "Email:Senha" "sua-senha-de-app"

dotnet run --launch-profile http
```

Acesse `http://localhost:5041/swagger` para explorar a API.

### Com Docker

```bash
cd boleto-verifier-api

docker build -t boleto-api .

docker run -p 5041:8080 \
  -e "Email__Remetente=seuemail@gmail.com" \
  -e "Email__Senha=sua-senha-de-app" \
  boleto-api
```

> A senha deve ser uma **senha de app** gerada em [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords) — não a senha da conta Google.

## Endpoints da API

### `POST /api/boleto`

Processa o código de barras e envia as informações por e-mail.

**Request body:**
```json
{
  "codigoBarras": "00190000090114971860168895702175185530000010000",
  "emailDestino": "destinatario@email.com"
}
```

**Response `200 OK`:**
```json
{
  "mensagem": "Boleto processado e e-mail enviado com sucesso!",
  "dados": {
    "banco": "Banco do Brasil",
    "valor": 100.00,
    "vencimento": "15/05/2025",
    "codigoOriginal": "00190000090114971860168895702175185530000010000"
  }
}
```

**Response `400 Bad Request`** — código de barras ou e-mail ausente:
```json
{ "erro": "Código de barras é obrigatório." }
```

**Response `500 Internal Server Error`** — boleto processado, mas falha no envio do e-mail:
```json
{
  "erro": "Boleto processado, mas falha ao enviar e-mail.",
  "detalhe": "mensagem de erro"
}
```

**Bancos identificados:** Banco do Brasil (`001`), Santander (`033`), Caixa Econômica Federal (`104`), Bradesco (`237`), Itaú (`341`), Sicoob (`756`). Demais bancos são exibidos pelo código numérico.

## Variáveis de Ambiente

| Variável | Descrição |
|---|---|
| `Email__Remetente` | Endereço Gmail usado para envio |
| `Email__Senha` | Senha de app gerada no Google Account |

Em desenvolvimento, use `dotnet user-secrets` para não expor credenciais no código. Em produção (Docker / Azure), passe as variáveis via `-e` ou pelas configurações do App Service.

## Deploy

### API — Azure App Service

```bash
docker tag boleto-api usuario/boleto-api:v1
docker push usuario/boleto-api:v1

az webapp config appsettings set \
  --resource-group rg-boleto \
  --name boleto-verifier-api \
  --settings Email__Remetente="seuemail@gmail.com" Email__Senha="sua-senha-de-app"
```

### Frontend — Azure Static Web Apps

Deploy automático via GitHub Actions a cada push na branch `main`, configurado pelo Azure Portal.

## Estrutura do Projeto

```
boleto-verifier/
├── index.html
├── styles.css
├── script.js
└── boleto-verifier-api/
    ├── Controllers/
    │   └── BoletoController.cs   # POST /api/boleto
    ├── Models/
    │   ├── BoletoRequest.cs      # Entrada: código + e-mail
    │   └── BoletoInfo.cs         # Saída: banco, valor, vencimento
    ├── Services/
    │   ├── BoletoParser.cs       # Extração dos dados do código
    │   └── EmailService.cs       # Envio via SMTP Gmail
    ├── Program.cs
    └── Dockerfile
```

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

Desenvolvido por [Allan Giaretta](https://github.com/AllanGiaretta26).
