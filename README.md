# FIAP Vehicle Sales — Software Principal

API principal da Fase 4 do Tech Challenge SOAT. Este repositório é responsável pelo catálogo administrativo e pelo webhook público de pagamentos. Listagens, reserva e compra ficam no repositório separado do serviço de vendas.

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

O serviço de vendas e o Keycloak devem estar disponíveis nas portas `5000` e `8080`. Depois execute:

```bash
docker compose up -d --build
```

A API principal ficará disponível em `http://localhost:5001` e seu PostgreSQL exclusivo usará a porta `5434`.

## Testes e cobertura

```powershell
dotnet test --collect:"XPlat Code Coverage" --settings coverage.runsettings --results-directory TestResults
./scripts/check-coverage.ps1 -ResultsPath TestResults -Minimum 80
```

A cobertura consolidada atual é 96,67%. O GitHub Actions bloqueia o pipeline abaixo de 80% e publica imagem no GHCR somente após push/merge em `master`.

## Kubernetes

Os manifests em `k8s/` incluem Deployment e Service da API, StatefulSet e Service do banco próprio, ConfigMap, Secret de exemplo e volume persistente.

```bash
kubectl kustomize k8s
kubectl apply -k k8s
```

Substitua todos os valores `CHANGE_ME`, ajuste o endereço do Keycloak e fixe a tag da imagem antes do deploy.
