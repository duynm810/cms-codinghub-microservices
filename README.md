## Prepare environment

* Install dotnet core version in file `global.json`
* IDE: Visual Studio 2022+, Rider, Visual Studio Code
* Docker Desktop
* EF Core tools reference (.NET CLI):

```Powershell
dotnet tool install --global dotnet-ef
```

```Powershell
dotnet tool update --global dotnet-ef
```

---

## How to run the project

- Run command for build project

```Powershell
dotnet build
```

Go to folder contain file `docker-compose`

1. Using docker-compose

- **RUN**

```Powershell (Only run)
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

- **BUILD and RE-RUN**

```Powershell (Build and run)
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build --remove-orphans
```

- **BUILD and RE-RUN IMAGE ONLY**

```Powershell (Build and run)
docker-compose up -d --build [service-name]
```

---

## How to migration the project (Development)

- Category Api

```Powershell
dotnet ef migrations "Initial"
```

```Powershell
dotnet ef database update
```

- Post Api

```Powershell
dotnet ef migrations add "Initial" --project Post.Infrastructure --startup-project Post.Api
```

```Powershell
dotnet ef database update --project Post.Infrastructure --startup-project Post.Api
```

- Identity Api

```Powershell

```


```Powershell
- Move to Identity.Api folder

dotnet ef migrations add Initial_Persisted_Grant_Migration -c PersistedGrantDbContext -c Migrations/IdentityServer/PersistedGrant
dotnet ef migrations add Initial_Configuration_Migration -c ConfigurationDbContext -o Migrations/IdentityServer/Configuration

dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext
```
```Powershell
- Move out Identity.Api folder (in root Identity folder)

dotnet ef migrations add Initial_AspNet_Identity -c IdentityContext -o Persistence/Migrations --project Identity.Infrastructure --startup-project Identity.Api
dotnet ef database update -c IdentityContext --project Identity.Infrastructure --startup-project Identity.Api
```
---

## Application URLs - DEVELOPMENT Environment:



---

## Application URLs - LOCAL Environment (Docker Container):



---

## Docker Application URLs - LOCAL Environment (Docker Container):

- Portainer: http://localhost:9000 - userName: admin ; pass: "Admin123456@"
- Kibana: http://localhost:5601 - userName: elastic ; pass: admin
- RabbitMQ: http://localhost:15672 - username: guest ; pass: guest
- HangfireUI: http://localhost:6016/jobs (docker)
- WebUI: http://localhost:6300 (docker)