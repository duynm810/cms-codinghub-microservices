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

```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --remove-orphans
```

- **BUILD and RE-RUN**

```Powershell
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d --build --remove-orphans
```

- **BUILD and RE-RUN IMAGE ONLY**

```Powershell
docker-compose up -d --build [service-name]
```

- **BUILD with Profile**

```Powershell
docker-compose --profile [profileName] -f docker-compose.yml -f docker-compose.override.yml up -d --build --remove-orphans
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

```Powershell (Migration)
dotnet ef migrations add "Initial" --project Post.Infrastructure --startup-project Post.Api
```

```Powershell (Update database)
dotnet ef database update --project Post.Infrastructure --startup-project Post.Api
```

- Identity Api

```Powershell (Migration)
- Move to Identity.Api folder

dotnet ef migrations add Initial_Persisted_Grant_Migration -c PersistedGrantDbContext -c Migrations/IdentityServer/PersistedGrant
dotnet ef migrations add Initial_Configuration_Migration -c ConfigurationDbContext -o Migrations/IdentityServer/Configuration

dotnet ef database update -c PersistedGrantDbContext
dotnet ef database update -c ConfigurationDbContext
```
```Powershell (Update database)
- Move out Identity.Api folder (in root Identity folder)

dotnet ef migrations add ConfigureUserProperties -c IdentityContext -o Persistence/Migrations --project Identity.Infrastructure --startup-project Identity.Api
dotnet ef database update -c IdentityContext --project Identity.Infrastructure --startup-project Identity.Api
```
---

## Application URLs - DEVELOPMENT Environment:

# Api services

- Ocelot GW: http://localhost:5001/swagger/index.html
- Identity Api: http://localhost:5002/swagger/index.html
- Category Api: http://localhost:5003/swagger/index.html
- Post Api: http://localhost:5004/swagger/index.html
- Series Api: http://localhost:5005/swagger/index.html
- Post In Series Api: http://localhost:5006/swagger/index.html
- Tag Api: http://localhost:5007/swagger/index.html
- Post In Tag Api: http://localhost:5008/swagger/index.html
- Comment Api: http://localhost:5009/swagger/index.html
- Hangfire Api: http://localhost:5010/swagger/index.html
- Media Api: http://localhost:5011/swagger/index.html

# Grpc services

- Identity Grpc: http://localhost:5101
- Category Grpc: http://localhost:5102
- Post Grpc: http://localhost:5103
- Series Grpc: http://localhost:5104
- Post In Series Grpc: http://localhost:5105
- Tag Grpc: http://localhost:5106
- Post In Tag Grpc: http://localhost:5107

# Other services

- Hangfire UI: http://localhost:5010/jobs
- Healthcheck UI: http://localhost:5200
- WebUI: http://localhost:5300

---

## Application URLs - LOCAL Environment (Docker Container):

# Api services

- Ocelot GW: http://localhost:6001/swagger/index.html
- Identity Api: http://localhost:6002/swagger/index.html
- Category Api: http://localhost:6003/swagger/index.html
- Post Api: http://localhost:6004/swagger/index.html
- Series Api: http://localhost:6005/swagger/index.html
- Post In Series Api: http://localhost:65006/swagger/index.html
- Tag Api: http://localhost:6007/swagger/index.html
- Post In Tag Api: http://localhost:6008/swagger/index.html
- Comment Api: http://localhost:6009/swagger/index.html
- Hangfire Api: http://localhost:6010/swagger/index.html
- Media Api: http://localhost:6011/swagger/index.html

# Grpc services

- Identity Grpc: http://localhost:6101
- Category Grpc: http://localhost:6102
- Post Grpc: http://localhost:6103
- Series Grpc: http://localhost:6104
- Post In Series Grpc: http://localhost:6105
- Tag Grpc: http://localhost:6106
- Post In Tag Grpc: http://localhost:6107

# Other services

- Hangfire UI: http://localhost:6010/jobs
- Healthcheck UI: http://localhost:6200
- WebUI: http://localhost:6300

---

## Docker Application URLs - LOCAL Environment (Docker Container):

- Portainer: http://localhost:9000 - userName: admin ; pass: "Admin123456@"
- Kibana: http://localhost:5601 - userName: elastic ; pass: admin
- RabbitMQ: http://localhost:15672 - username: guest ; pass: guest