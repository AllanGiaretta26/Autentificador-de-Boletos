# boleto-verifier

**🌐 Site online:** [Acessar o site](https://red-mushroom-0446f0b0f.2.azurestaticapps.net)

Sistema web que lê o código de barras de um boleto bancário, extrai as informações e envia para um e-mail específico.

## Funcionalidades

- Leitura de código de barras de boletos bancários (47 ou 48 dígitos)
- Extração automática de banco, valor e vencimento
- Envio das informações por e-mail via SMTP

## Tecnologias

- **Frontend:** HTML5, CSS3, JavaScript
- **Backend:** .NET 10, ASP.NET Core Web API, C#
- **E-mail:** MailKit (SMTP Gmail)
- **Containerização:** Docker
- **Hospedagem:** Azure App Service + Azure Static Web Apps

## Estrutura do projeto

```
boleto-verifier/
├── index.html
├── styles.css
├── script.js
└── boleto-verifier-api/
    ├── Controllers/
    │   └── BoletoController.cs
    ├── Models/
    │   ├── BoletoRequest.cs
    │   └── BoletoInfo.cs
    ├── Services/
    │   ├── BoletoParser.cs
    │   └── EmailService.cs
    ├── Program.cs
    └── Dockerfile
```

## Como rodar localmente

**Pré-requisitos:** .NET 10 SDK, Docker Desktop

### Sem Docker

```bash
cd boleto-verifier-api
dotnet user-secrets set "Email:Remetente" "seuemail@gmail.com"
dotnet user-secrets set "Email:Senha" "sua-senha-de-app"
dotnet run --launch-profile http
```

Acesse `http://localhost:5041/swagger` para testar a API.

### Com Docker

```bash
cd boleto-verifier-api
docker build -t boleto-api .
docker run -p 5041:8080 \
  -e "Email__Remetente=seuemail@gmail.com" \
  -e "Email__Senha=sua-senha-de-app" \
  boleto-api
```

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

Conectado ao repositório GitHub via Azure Portal. Deploy automático a cada push na branch `main`.

## Variáveis de ambiente

| Variável | Descrição |
|---|---|
| `Email__Remetente` | E-mail Gmail usado para envio |
| `Email__Senha` | Senha de app gerada no Google Account |

## Aprendizados

Projeto desenvolvido para aprender na prática: arquitetura REST com .NET, containerização com Docker e deploy na nuvem com Azure.

## Autor
Desenvolvido por Allan Giaretta.
