# Checkpoint 2 — ExpenseHub

Checkpoint de C# em grupos de até 3 pessoas para construção de uma Application Programming Interface (API) corporativa de reembolsos.

O prazo de entrega é **13 de outubro de 2026**. O grupo deverá implementar autenticação, autorização, fluxo de aprovação e reprovação, pagamento simulado, histórico e testes unitários.

## Integrantes

| Nome | RM |
|------|----|
| Ian Monteiro | 558652 |
| Mauricio Alves | 556214 |

## Criar seu repositório

1. Clique em **Use this template**.
2. Selecione **Create a new repository**.
3. Crie um repositório **público** em uma das contas do grupo.
4. Adicione os demais integrantes como colaboradores.
5. Clone o repositório.

Não use fork. As issues permanecem neste repositório original como especificação comum da turma.

Os commits serão utilizados para avaliar a participação. Todos os membros do grupo
devem possuir mais de um commit no repositório.

## Fluxo de trabalho

Para cada issue:

1. leia os critérios no repositório original;
2. crie uma branch com o identificador, por exemplo `i06-ownership`;
3. implemente e valide a feature;
4. abra uma pull request no seu próprio repositório;
5. use um título como `I06 — Ownership e matriz de acesso`;
6. adicione na descrição uma referência completa, como `Racass/checkpoint-csharpracass-expensehub#6`;
7. não use `Closes`, `Fixes` ou `Resolves`, pois a issue original deve permanecer aberta;
8. conclua a auto-revisão e faça o merge.

## Estrutura inicial

```text
sources/
├── ExpenseHub.slnx
├── ExpenseHub.Api/
└── ExpenseHub.UnitTests/
```

A solução começa sem Identity, banco, domínio ou testes funcionais. Toda implementação avaliada deve ser criada por você.

## Comandos

```shell
dotnet restore ./sources/ExpenseHub.slnx
dotnet build ./sources/ExpenseHub.slnx
dotnet test ./sources/ExpenseHub.slnx
dotnet run --project ./sources/ExpenseHub.Api/ExpenseHub.Api.csproj
```

O endpoint inicial `GET /health` existe apenas para confirmar que a aplicação inicia.

## Documentação

- [Enunciado](docs/ENUNCIADO.md)
- [Requisitos e contratos](docs/REQUISITOS.md)
- [Rubrica](docs/RUBRICA.md)
- [Matriz de autorização](docs/MATRIZ-AUTORIZACAO.md)
- [Processo no GitHub](docs/PROCESSO-GITHUB.md)
- [Uso de Inteligência Artificial](docs/USO-DE-IA.md)
- [Regras do pipeline de qualidade](docs/code-quality-rules.md)

## Banco de dados

Você pode utilizar Microsoft SQL Server LocalDB, Oracle Database, SQLite ou outro provider relacional compatível com Entity Framework Core.

A escolha não gera pontos. Documente no README do seu repositório:

- provider e pacote utilizado;
- configuração necessária;
- criação ou atualização do banco;
- como iniciar a aplicação.

Não versione senhas, tokens ou connection strings sensíveis.

---

## Configuração e execução (implementação do grupo)

### Provider escolhido

**SQLite** via `Microsoft.EntityFrameworkCore.Sqlite 10.0.12`.

O banco é criado automaticamente em `expensehub.db` na pasta de saída da aplicação na primeira execução.

### Segredos (não versionados)

Crie o arquivo `sources/ExpenseHub.Api/appsettings.Development.json` (já está no `.gitignore`) com o seguinte conteúdo:

```json
{
  "Jwt": {
    "Key": "<string-de-pelo-menos-32-caracteres>"
  },
  "AdminSeed": {
    "Password": "<senha-com-maiuscula-numero-e-simbolo>"
  }
}
```

A chave JWT deve ter no mínimo 32 caracteres. A senha do admin deve atender às regras de complexidade do ASP.NET Identity (maiúscula, minúscula, dígito e símbolo, mínimo 6 caracteres).

### Executar as migrações e iniciar

```shell
dotnet restore ./sources/ExpenseHub.slnx
dotnet build ./sources/ExpenseHub.slnx
dotnet run --project ./sources/ExpenseHub.Api/ExpenseHub.Api.csproj
```

Na primeira execução, a aplicação aplica as migrações do EF Core automaticamente e cria o usuário administrador.

### Usuário administrador padrão (desenvolvimento)

| Campo  | Valor                              |
|--------|------------------------------------|
| E-mail | `admin@expensehub.local`           |
| Senha  | valor de `AdminSeed:Password`      |
| Papel  | `Admin`                            |

### Autenticação

Todas as rotas (exceto `POST /register` e `POST /login`) exigem o cabeçalho:

```
Authorization: Bearer <token>
```

O token é obtido em `POST /login` e tem validade de 24 horas.

### Roles disponíveis

| Role       | Permissões                                          |
|------------|-----------------------------------------------------|
| Employee   | Criar, editar, submeter e visualizar próprios gastos |
| Approver   | Aprovar ou reprovar despesas submetidas             |
| Finance    | Registrar pagamento de despesas aprovadas           |
| Auditor    | Visualizar todas as despesas e histórico            |
| Admin      | Gerenciar usuários e atribuir roles                 |

### Endpoints principais

| Método | Rota                           | Role(s)              |
|--------|--------------------------------|----------------------|
| POST   | /register                      | Anônimo              |
| POST   | /login                         | Anônimo              |
| GET    | /api/admin/users               | Admin                |
| PUT    | /api/admin/users/{id}/roles    | Admin                |
| POST   | /api/expenses                  | Employee             |
| PUT    | /api/expenses/{id}             | Employee             |
| GET    | /api/expenses                  | Employee/Approver/Finance/Auditor |
| GET    | /api/expenses/{id}             | Employee/Approver/Finance/Auditor |
| POST   | /api/expenses/{id}/submit      | Employee             |
| POST   | /api/expenses/{id}/approve     | Approver             |
| POST   | /api/expenses/{id}/reject      | Approver             |
| POST   | /api/expenses/{id}/pay         | Finance              |
| GET    | /api/expenses/{id}/history     | Employee/Approver/Finance/Auditor |
| GET    | /health                        | Anônimo              |

## Testes

Somente testes unitários escritos por você entram na nota. Testes de integração, end-to-end ou de interface são permitidos, mas opcionais e sem pontuação.

Os testes unitários devem executar sem banco, rede ou serviço externo.

## Entrega

Entregue:

- URL do repositório público;
- commit Secure Hash Algorithm (SHA) final;
- integração contínua executada;
- documentação atualizada.

O projeto deve compilar sem erros e ser entregue sem warnings para receber a pontuação integral de Qualidade de Código.
