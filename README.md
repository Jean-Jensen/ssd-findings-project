# Findings app lab setup

Requires Git, Docker Desktop with Compose, the .NET 10 SDK, and Node.js 22 or newer with npm. Start Docker Desktop before running the commands. Use Terminal on macOS or PowerShell on Windows. On Windows, Docker Desktop must be in Linux containers mode.

## 1. Clone and configure

```sh
git clone https://github.com/Jean-Jensen/ssd-findings-project.git
cd ssd-findings-project
```

**macOS**

```sh
cp .env.example .env
```

**Windows PowerShell**

```powershell
Copy-Item .env.example .env
```

On either OS, run this once per variable and paste each result after its `=` in `.env`, with no quotes. Keep `.env` local. Compose supplies the client secrets and test user password to Keycloak during realm import.

```sh
node -p "require('node:crypto').randomBytes(32).toString('hex')"
```

## 2. Start Keycloak and Postgres

```sh
docker compose up -d --wait
```

Wait for the [Keycloak console](http://localhost:8080/admin/) to open. Its admin login is `lab-admin` with `LAB_KEYCLOAK_ADMIN_PASSWORD`. Keycloak imports the [lab realm](keycloak/findings-lab-realm.json), including its two app clients and test users, on first startup. Postgres creates the tables from `proposal.sql` when its volume is empty. Ports `8080` and `5432` must be available locally.

## 3. Start the API

Run the commands for your OS from the repository root.

**macOS**

```sh
source .env
dotnet user-secrets set --project server/Startup 'OpenIDConnectSettings:ClientSecret' "$LAB_OIDC_SECRET"
dotnet user-secrets set --project server/Startup 'KeycloakAdmin:ClientSecret' "$LAB_ROLE_MANAGER_SECRET"
export OpenIDConnectSettings__Authority=http://localhost:8080/realms/findings-lab
export AppOptions__Seed=false
cd server/Startup
dotnet run --launch-profile http
```

**Windows PowerShell**

```powershell
$lab = ConvertFrom-StringData (Get-Content .env -Raw)
dotnet user-secrets set --project server/Startup 'OpenIDConnectSettings:ClientSecret' $lab.LAB_OIDC_SECRET
dotnet user-secrets set --project server/Startup 'KeycloakAdmin:ClientSecret' $lab.LAB_ROLE_MANAGER_SECRET
$env:OpenIDConnectSettings__Authority = 'http://localhost:8080/realms/findings-lab'
$env:AppOptions__Seed = 'false'
Set-Location server/Startup
dotnet run --launch-profile http
```

The API listens on `http://localhost:5001`. The imported `vuln-app` client already registers `http://localhost:5001/api/auth/Callback`.

## 4. Start React

Open another terminal in the repository root.

**macOS**

```sh
cd client
npm ci
npm run dev -- --strictPort
```

**Windows PowerShell**

```powershell
Set-Location client
npm.cmd ci
npm.cmd run dev -- --strictPort
```

Open [the app](http://localhost:5173). Use `LAB_TEST_USER_PASSWORD` with any of these accounts:

| Username | App role |
| --- | --- |
| `analyst` | Analyst |
| `developer` | Developer |
| `project_manager` | PM |
| `auditor` | Auditor |
| `roleless` | None |

## Stop or reset

Run `docker compose down` from the repository root to stop the services and keep their data. To start over, `docker compose down -v` deletes both **local lab volumes**. The next startup reruns the database setup and imports the realm with the current `.env` values.
