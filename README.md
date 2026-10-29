# Onion Architecture .NET Backend + React & Typescript Frontend

### **Verify backend**
```sh
dotnet build
```

### **PostgreSQL Database Setup with Docker**
```sh
# Start PostgreSQL container (from where docker-compose.yml file is)
docker-compose up -d

# Verify container is running
docker ps
```

### **Verify frontend**
```sh
cd client
pnpm install
pnpm run dev
```

## 🚀 ** Running the Projects**
```sh
dotnet run --project Startup/Startup.csproj
```


