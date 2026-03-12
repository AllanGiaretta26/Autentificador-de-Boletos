# Build da imagem
docker build -t boleto-verifier .

# Executar container
docker run -p 8000:8000 boleto-verifier

# Testar no navegador ou curl (exemplo acima)

# 1. Criar Resource Group
az group create --name rg-boleto-verifier --location brazilsouth

# 2. Criar Azure Container Registry (ACR)
az acr create --resource-group rg-boleto-verifier \
  --name acrboletoverifier \
  --sku Basic --admin-enabled true

# 3. Build e push da imagem direto no ACR
az acr build --registry acrboletoverifier \
  --image boleto-verifier:v1 .

# 4. Deploy no Azure Container Instances (rápido e barato)
az container create \
  --resource-group rg-boleto-verifier \
  --name boleto-verifier-service \
  --image acrboletoverifier.azurecr.io/boleto-verifier:v1 \
  --registry-login-server acrboletoverifier.azurecr.io \
  --registry-username $(az acr credential show -n acrboletoverifier --query username -o tsv) \
  --registry-password $(az acr credential show -n acrboletoverifier --query passwords[0].value -o tsv) \
  --dns-name-label boleto-verifier-$(date +%s) \
  --ports 80 \
  --cpu 1 --memory 1 \
  --environment-variables ASPNETCORE_ENVIRONMENT=Production

# 5. Obter a URL pública
az container show \
  --resource-group rg-boleto-verifier \
  --name boleto-verifier-service \
  --query "{FQDN:ipAddress.fqdn, IP:ipAddress.ip}" -o table