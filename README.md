# Onion Architecture .NET Backend + React & Typescript Frontend - Step By Step Guide

This project follows the Onion Architecture pattern with the following structure:

```mathematica
ProjectRoot/
│── server/
│   │── YourSolutionName.sln
│   │── Core.Domain/
│   │   ├── Core.Domain.csproj
│   │── Application/
│   │   ├── Application.csproj
│   │── Infrastructure.Postgres/
│   │   ├── Infrastructure.Postgres.csproj
│   │── Api.Rest/
│   │   ├── Api.Rest.csproj
│   │── Api.Websocket/
│   │   ├── Api.Websocket.csproj
│   │── Infrastructure.Postgres.Scaffolding/
│   │   ├── Infrastructure.Postgres.Scaffolding.csproj
│   │── Infrastructure.Websocket/
│   │   ├── Infrastructure.Websocket.csproj
│   │── Startup/
│   │   ├── Startup.csproj
│── client/
│   │── package.json
│   │── pnpm-lock.yaml
│   │── tsconfig.json
│   │── vite.config.ts
│   │── src/
│   │── public/
```

## 🛠️ **Setup Instructions**
Follow these steps to set up the project from scratch:

### **1. Create the Solution**
```sh
dotnet new sln -n YourSolutionName
```

### **2. Create the Projects, Add Projects to Solutions and Set up references**
```sh
dotnet new classlib -n Core.Domain
dotnet new classlib -n Application
dotnet new classlib -n Infrastructure.Postgres
dotnet new classlib -n Infrastructure.Postgres.Scaffolding
dotnet new classlib -n Infrastructure.Websocket
dotnet new webapi -n Api.Rest
dotnet new web -n WebSockets
dotnet new console -n Startup

dotnet sln add Core.Domain/Core.Domain.csproj
dotnet sln add Application/Application.csproj
dotnet sln add Infrastructure.Postgres/Infrastructure.Postgres.csproj
dotnet sln add Infrastructure.Postgres.Scaffolding/Infrastructure.Postgres.Scaffolding.csproj
dotnet sln add Infrastructure.Websocket/Infrastructure.Websocket.csproj
dotnet sln add Api.Rest/Api.Rest.csproj
dotnet sln add Api.WebSocket/Api.WebSocket.csproj
dotnet sln add Startup/Startup.csproj

dotnet add Application/Application.csproj reference Core.Domain/Core.Domain.csproj
dotnet add Infrastructure.Postgres/Infrastructure.Postgres.csproj reference Application/Application.csproj
dotnet add Infrastructure.Postgres/Infrastructure.Postgres.csproj reference Infrastructure.Postgres.Scaffolding/Infrastructure.Postgres.Scaffolding.csproj
dotnet add Infrastructure.Postgres.Scaffolding/Infrastructure.Postgres.Scaffolding.csproj reference Core.Domain/Core.Domain.csproj
dotnet add Infrastructure.Websocket/Infrastructure.Websocket.csproj reference Application/Application.csproj
dotnet add Api.Rest/Api.Rest.csproj reference Application/Application.csproj
dotnet add Api.WebSocket/Api.WebSocket.csproj reference Application/Application.csproj
dotnet add Startup/Startup.csproj reference Application/Application.csproj
dotnet add Startup/Startup.csproj reference Infrastructure.Postgres/Infrastructure.Postgres.csproj
dotnet add Startup/Startup.csproj reference Api.Rest/Api.Rest.csproj
dotnet add Startup/Startup.csproj reference Api.WebSocket/Api.WebSocket.csproj
dotnet add Startup/Startup.csproj reference Infrastructure.WebSocket/Infrastructure.WebSocket.csproj
```

### **3. Adding Nuget Packages**
Inside Infrastructure.Postgres directory, run:
```sh
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package PgCtxSetup
```

Inside Infrastructure.Postgres.Scaffolding directory, run:
```sh
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package Microsoft.EntityFrameworkCore.Relational
```

Inside Infrastructure.Websocket directory, run:
```sh
dotnet add package uldahlalex.websocket.boilerplate
```

Inside Api.Rest directory, run:
```sh
dotnet add package NSwag.AspNetCore
```

Inside Api.WebSocket directory, run:
```sh
dotnet add package uldahlalex.websocket.boilerplate
```

Inside Application directory, run:
```sh
dotnet add package JWT
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions
dotnet add package Microsoft.Extensions.Options
dotnet add package Microsoft.Extensions.Options.ConfigurationExtensions
dotnet add package Microsoft.Extensions.Configuration.Abstractions
```

Inside Startup directory, run:
```sh
dotnet add package Microsoft.Extensions.Logging.Abstractions
dotnet add package Microsoft.Extensions.Logging.Console
dotnet add package NSwag.CodeGeneration.TypeScript
dotnet add package patrikvalentiny-WebSocketProxy
dotnet add package NSwag.AspNetCore
```

### **4. Verify the Structure**
```sh
dotnet build
```

### **5. PostgreSQL Database Setup with Docker**
```sh
# Start PostgreSQL container (from where docker-compose.yml file is)
docker-compose up -d

# Verify container is running
docker ps
```

### **6. Create client template**
```sh
pnpm create vite client --template react-ts
cd client
pnpm install
pnpm run dev
```

## 🛠️ **Steps for adding to Git / GitHub**
```sh
git init
dotnet new gitignore  # Generates a .gitignore file with all you need
git add -A
git commit -m "Your message"
git remote add origin YOUR_GITHUB_REPO_URL
git branch -M main
git push -u origin main
```

## ** Small notes**
In Infrastructure.Postgres.csproj, Api.Rest.csproj, Api.WebSocket.csproj<br>
Add the following statement inside `<PropertyGroup>`:<br>
`<OutputType>library</OutputType>`

## 🚀 ** Running the Projects**
```sh
dotnet run --project Startup/Startup.csproj
```


