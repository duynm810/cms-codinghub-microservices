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

## Connection string (database)

- Category API (MySQL)

```Powershell
jdbc:mysql://localhost:3307/xxx?user=root&password=Passw0rd!
```
- **xxx: database name**
---

## How to migration the project (Development)

- Category API

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

---

## Application URLs - DEVELOPMENT Environment:

- Category API: http://localhost:5002/swagger/index.html

---

## Application URLs - LOCAL Environment (Docker Container):

- Category API: http://localhost:6002/swagger/index.html

---

## Docker Application URLs - LOCAL Environment (Docker Container):

- Portainer: http://localhost:9000 - username: admin ; pass: "Admin123456@"
- Kibana: http://localhost:5601 - username: elastic ; pass: admin
- RabbitMQ: http://localhost:15672 - username: guest ; pass: guest
