# FIAP Vehicle Sales — Software Principal

API principal da Fase 4 do Tech Challenge SOAT. Este repositório é responsável pelo catálogo administrativo e pelo webhook público de pagamentos. Listagens, reserva e compra ficam no repositório separado do [serviço de venda de veículos](https://github.com/AdamMachado/fiap-vehicle-sales-Fase-4).

## Responsabilidades

- cadastrar e editar veículos no banco principal;
- sincronizar o catálogo com o serviço de vendas por HTTP;
- autenticar a comunicação interna por OAuth 2.0 client credentials;
- receber webhook de pagamento protegido por secret;
- encaminhar confirmação ou cancelamento ao serviço de vendas;
- armazenar notificações processadas para garantir idempotência;
- utilizar PostgreSQL diferente do banco transacional de vendas.

## Endpoints

| Método | Endpoint | Autorização |
| --- | --- | --- |
| POST | `/api/vehicles` | JWT com role `admin` |
| PUT | `/api/vehicles/{id}` | JWT com role `admin` |
| POST | `/api/payments/webhook` | header `X-Webhook-Secret` |
| GET | `/health/live` | público |

Webhook:

```json
{
  "paymentCode": "codigo-retornado-na-compra",
  "status": "Completed"
}
```

Também aceita `Canceled`. Chamadas repetidas com o mesmo resultado são idempotentes; resultados contraditórios retornam conflito.

## Execução local

Primeiro, clone e inicie o repositório do serviço de vendas, que também sobe o Keycloak:

```bash
git clone https://github.com/AdamMachado/fiap-vehicle-sales-Fase-4.git
cd fiap-vehicle-sales-Fase-4
docker compose up -d --build
```

O serviço de vendas e o Keycloak ficarão disponíveis nas portas `5000` e `8080`. Em outro diretório, clone e inicie este repositório:

```bash
git clone https://github.com/AdamMachado/fiap-vehicle-sales-software-principal-fase4.git
cd fiap-vehicle-sales-software-principal-fase4
docker compose up -d --build
```

A API principal ficará disponível em `http://localhost:5001`, o Swagger em `http://localhost:5001/swagger` e seu PostgreSQL exclusivo usará a porta `5434`.

## Autenticação administrativa e Swagger

O usuário de demonstração é importado pelo Keycloak do serviço de vendas:

```text
Usuário: admin@test.com
Senha: 123456
Role: admin
```

Obtenha o token no PowerShell:

```powershell
$adminTokenResponse = Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:8080/realms/fiap-vehicle-sales/protocol/openid-connect/token" `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{
    client_id = "vehicle-sales-api"
    username = "admin@test.com"
    password = "123456"
    grant_type = "password"
  }

$adminTokenResponse.access_token
```

Acesse `http://localhost:5001/swagger`, clique em **Authorize** e informe somente o token. O Swagger acrescenta o prefixo `Bearer` automaticamente.

As credenciais e secrets descritos aqui são exclusivamente para demonstração local e devem ser substituídos em qualquer ambiente publicado.

## Testes e cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory TestResults
./scripts/check-coverage.ps1 -ResultsPath TestResults -Minimum 80
```

A cobertura consolidada atual é 96,67%. O GitHub Actions bloqueia o pipeline abaixo de 80% e publica imagem no GHCR somente após push/merge em `master`.

## CI/CD e publicação

O workflow valida restore, build, testes e cobertura em Pull Requests para `master`. Depois do merge/push em `master`, publica a imagem versionada no GitHub Container Registry. Os manifests em `k8s/` descrevem a implantação; a aplicação em um cluster público exige que o ambiente de destino e suas credenciais sejam configurados.

## Kubernetes

Os manifests em `k8s/` incluem Deployment e Service da API, StatefulSet e Service do banco próprio, ConfigMap, Secret de exemplo e volume persistente.

```bash
kubectl kustomize k8s
kubectl apply -k k8s
```

Substitua todos os valores `CHANGE_ME`, ajuste o endereço do Keycloak e fixe a tag da imagem antes do deploy.
