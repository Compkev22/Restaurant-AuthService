# 🍔 Restaurant System - AuthService

API RESTful para la gestión de autenticación, usuarios y roles del ecosistema **Restaurant System (Kinal Fried Chicken)**.

---

## 📝 Descripción

Este microservicio se encarga de la seguridad centralizada de la plataforma. Permite:

- Registro de nuevos usuarios  
- Verificación de identidad mediante **Argon2id**  
- Control de acceso basado en roles (**RBAC**)  
- Emisión de tokens **JWT**  

---

## 🛠️ Tech Stack

- **Runtime:** .NET 8.0 SDK  
- **Framework:** ASP.NET Core Web API  
- **Base de Datos:** PostgreSQL  
- **ORM:** Entity Framework Core (Code First)  

### 🔐 Seguridad
- Argon2id  
- JWT Bearer Tokens  

### 📄 Documentación
- Swagger / OpenAPI  

### 📦 Librerías
- Konscious.Security.Cryptography.Argon2  
- CloudinaryDotNet  
- MailKit / MimeKit  

---

## 🚀 Instalación

    # Clonar repositorio
    git clone [URL_DEL_REPOSITORIO]

    # Restaurar dependencias
    dotnet restore

    # Certificados HTTPS
    dotnet dev-certs https --trust

    # Compilar
    dotnet build

---

## ⚙️ Configuración de Base de Datos

Editar archivo:

    src/AuthService.Api/appsettings.json

Contenido:

    {
      "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Port=5432;Database=db_restaurante_auth;Username=TU_USUARIO;Password=TU_CONTRASEÑA"
      }
    }

Aplicar migraciones:

    cd src/AuthService.Api
    dotnet ef database update

---

## 📂 Estructura del Proyecto

    Restaurant-AuthService/
    ├── src/
    │   ├── AuthService.Api/
    │   ├── AuthService.Application/
    │   ├── AuthService.Domain/
    │   └── AuthService.Persistence/
    └── AuthService.sln

---

# 🔌 Endpoints Principales

---

## 🔐 Autenticación

| Método | Endpoint | Descripción |
|--------|----------|------------|
| POST | /api/v1/Auth/register | Registrar usuario |
| POST | /api/v1/Auth/login | Iniciar sesión |
| POST | /api/v1/Auth/verify-email | Verificar correo |
| POST | /api/v1/Auth/forgot-password | Recuperar contraseña |
| GET | /api/v1/Auth/profile | Obtener perfil |

---

# 📊 Ejemplos de Petición

---

## 🔐 Registro

POST /api/v1/Auth/register

    firstName: Juan
    lastName: Perez
    systemUsername: jperez
    email: juan@kfc.local
    password: Password123!
    profilePicture: [archivo]

---

## 🔑 Login

POST /api/v1/Auth/login

    emailOrUsername: sysadmin
    password: 12345678

---

## ✉️ Verificar Email

POST /api/v1/Auth/verify-email

    token: TOKEN_DE_LA_DB

---

## 👤 Perfil

GET /api/v1/Auth/profile

    Authorization: Bearer TU_TOKEN_JWT

---

# 🎭 Roles del Sistema

| Rol | Descripción |
|-----|------------|
| PLATFORM_ADMIN_ROLE | Admin global |
| BRANCH_ADMIN_ROLE | Admin sucursal |
| EMPLOYEE_ROLE | Empleado |
| CLIENT_ROLE | Cliente |

---

## 🛠️ Comandos Útiles

    dotnet run --project src/AuthService.Api
    dotnet clean

    dotnet ef migrations add NombreMigracion \
    --project src/AuthService.Persistence \
    --startup-project src/AuthService.Api
