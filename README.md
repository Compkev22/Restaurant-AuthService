# 🍽️ Restaurant Auth Service - Servicio de Autenticación

Servicio de autenticación y gestión de usuarios para la plataforma **Restaurant Management System**.

---

⚙️ Variables de Entorno
Crear un archivo appsettings.json en la raíz del proyecto con la siguiente configuración:

JSON
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RestaurantAuthDb;Integrated Security=true;TrustServerCertificate=true;"
  },
  "JwtSettings": {
    "SecretKey": "tu_llave_secreta_super_segura_minimo_32_caracteres",
    "Issuer": "RestaurantAuthService",
    "Audience": "RestaurantApp",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
📂 Estructura del Proyecto
Code
RestaurantAuthService/
├── Controllers/        # Controladores API
│   ├── AuthController.cs
│   └── UserController.cs
├── Models/            # Modelos de datos
│   ├── User.cs
│   ├── Restaurant.cs
│   └── DTOs/
├── Services/          # Servicios de negocio
│   ├── AuthService.cs
│   ├── UserService.cs
│   └── TokenService.cs
├── Data/              # Contexto y migración EF
│   ├── ApplicationDbContext.cs
│   └── Migrations/
├── Middleware/        # Middleware personalizado
│   └── ErrorHandling.cs
├── Configuration/     # Configuración de la app
└── Program.cs         # Punto de entrada
🔌 Endpoints Principales
🔐 Autenticación
Método	Endpoint	Descripción
POST	/api/v1/auth/register	Registrar nuevo usuario (Admin/Employee)
POST	/api/v1/auth/login	Iniciar sesión con JWT
POST	/api/v1/auth/refresh-token	Renovar token expirado
POST	/api/v1/auth/logout	Cerrar sesión
👤 Usuarios
Método	Endpoint	Descripción
GET	/api/v1/users	Listar todos los usuarios
GET	/api/v1/users/:id	Obtener detalle de un usuario
PUT	/api/v1/users/:id	Actualizar datos de usuario
PUT	/api/v1/users/:id/change-password	Cambiar contraseña
PUT	/api/v1/users/:id/status	Activar o Desactivar usuario
DELETE	/api/v1/users/:id	Eliminar usuario
🏪 Restaurante
Método	Endpoint	Descripción
GET	/api/v1/restaurants	Listar restaurantes
GET	/api/v1/restaurants/:id	Obtener detalle de restaurante
POST	/api/v1/restaurants	Crear nuevo restaurante
PUT	/api/v1/restaurants/:id	Actualizar datos de restaurante
👥 Roles y Permisos
Método	Endpoint	Descripción
GET	/api/v1/roles	Listar roles disponibles
GET	/api/v1/roles/:id/permissions	Obtener permisos de un rol
POST	/api/v1/roles	Crear nuevo rol
PUT	/api/v1/roles/:id	Actualizar rol
📊 Ejemplos de Petición (JSON)
Aquí encontrarás los cuerpos JSON (Body) necesarios para probar cada entidad en Postman.

🔐 1. Auth (Registro y Login)
Registrar Nuevo Usuario
POST
http://localhost:5000/api/v1/auth/register

JSON
{
  "firstName": "Kevin",
  "lastName": "Velasquez",
  "email": "kevin@restaurant.com",
  "password": "SecurePassword123!",
  "phoneNumber": "+50212345678",
  "role": "ADMIN",
  "restaurantId": "550e8400-e29b-41d4-a716-446655440000"
}
Iniciar Sesión
POST
http://localhost:5000/api/v1/auth/login

JSON
{
  "email": "kevin@restaurant.com",
  "password": "SecurePassword123!"
}
Respuesta:

JSON
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "firstName": "Kevin",
    "email": "kevin@restaurant.com",
    "role": "ADMIN",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  },
  "message": "Login exitoso"
}
Renovar Token
POST
http://localhost:5000/api/v1/auth/refresh-token

JSON
{
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
👤 2. Usuario (Gestión)
Actualizar Perfil
PUT
http://localhost:5000/api/v1/users/550e8400-e29b-41d4-a716-446655440000

JSON
{
  "firstName": "Kevin Alejandro",
  "lastName": "Velasquez",
  "phoneNumber": "+50212345678",
  "email": "nuevo_correo@restaurant.com"
}
Cambiar Contraseña
PUT
http://localhost:5000/api/v1/users/550e8400-e29b-41d4-a716-446655440000/change-password

JSON
{
  "currentPassword": "SecurePassword123!",
  "newPassword": "NewSecurePassword456!"
}
🏪 3. Restaurante
Crear Restaurante
POST
http://localhost:5000/api/v1/restaurants

JSON
{
  "name": "El Buen Sabor",
  "address": "Calle Principal 123",
  "phoneNumber": "+50287654321",
  "email": "info@elbuen sabor.com",
  "cuisineType": "Comida Típica",
  "openingTime": "11:00",
  "closingTime": "22:00"
}
Actualizar Restaurante
PUT
http://localhost:5000/api/v1/restaurants/550e8400-e29b-41d4-a716-446655440000

JSON
{
  "name": "El Buen Sabor Premium",
  "phoneNumber": "+50287654321",
  "openingTime": "10:00",
  "closingTime": "23:00"
}
👥 4. Roles y Permisos
Crear Nuevo Rol
POST
http://localhost:5000/api/v1/roles

JSON
{
  "name": "MANAGER",
  "description": "Gerente de restaurante con permisos limitados",
  "permissions": [
    "VIEW_DASHBOARD",
    "MANAGE_EMPLOYEES",
    "VIEW_ORDERS",
    "MANAGE_MENUS"
  ]
}
🗄️ Modelos de Base de Datos (Esquemas)
👤 Usuario (User)
Representa a los empleados y administradores del restaurante.

Campo	Tipo	Requerido	Descripción
Id	GUID	✅	Identificador único (PK)
FirstName	String	✅	Nombre del usuario (Máx 100 caracteres)
LastName	String	✅	Apellido del usuario (Máx 100 caracteres)
Email	String	✅	Correo electrónico (Único)
PasswordHash	String	✅	Contraseña hasheada
PhoneNumber	String	❌	Número telefónico
Role	String	✅	ROL: ['ADMIN', 'MANAGER', 'EMPLOYEE']
Status	Boolean	❌	true activo / false inactivo (Default: true)
RestaurantId	GUID	✅	FK - Referencia a Restaurante
CreatedAt	DateTime	✅	Fecha de creación
UpdatedAt	DateTime	❌	Fecha de última actualización
🏪 Restaurante (Restaurant)
Información de cada sucursal o restaurante.

Campo	Tipo	Requerido	Descripción
Id	GUID	✅	Identificador único (PK)
Name	String	✅	Nombre del restaurante (Máx 150 caracteres)
Address	String	✅	Dirección física
PhoneNumber	String	✅	Teléfono de contacto
Email	String	✅	Correo electrónico (Único)
CuisineType	String	✅	Tipo de cocina
OpeningTime	TimeSpan	✅	Hora de apertura
ClosingTime	TimeSpan	✅	Hora de cierre
Status	Boolean	❌	true activo / false inactivo (Default: true)
CreatedAt	DateTime	✅	Fecha de creación
UpdatedAt	DateTime	❌	Fecha de última actualización
👥 Rol (Role)
Definición de roles y permisos.

Campo	Tipo	Requerido	Descripción
Id	GUID	✅	Identificador único (PK)
Name	String	✅	Nombre del rol (Único)
Description	String	❌	Descripción del rol
Permissions	List<String>	✅	Lista de permisos asignados
IsActive	Boolean	❌	true activo / false inactivo (Default: true)
CreatedAt	DateTime	✅	Fecha de creación
🔑 Token de Sesión (RefreshToken)
Gestión de tokens de renovación.

Campo	Tipo	Requerido	Descripción
Id	GUID	✅	Identificador único (PK)
UserId	GUID	✅	FK - Referencia al Usuario
Token	String	✅	Token de renovación
ExpiryDate	DateTime	✅	Fecha de expiración
IsRevoked	Boolean	❌	true revocado / false activo (Default: false)
CreatedAt	DateTime	✅	Fecha de creación
🛠️ Scripts Disponibles
bash
# Compilar el proyecto
dotnet build

# Ejecutar el servidor en modo desarrollo
dotnet run

# Ver aplicación en
https://localhost:5001/swagger

# Ejecutar migraciones de base de datos
dotnet ef database update

# Crear nueva migración
dotnet ef migrations add NombreMigracion

# Ejecutar tests
dotnet test
🔒 Seguridad
✅ Autenticación con JWT
✅ Hashing de contraseñas con bcrypt
✅ CORS configurado
✅ Rate Limiting implementado
✅ Validación de entrada con FluentValidation
✅ HTTPS obligatorio en producción
📋 Licencia
Distribuido bajo la Licencia MIT. Ver LICENSE para más detalles.

👨‍💻 Autor
Kevin Velásquez
GitHub: @Compkev22
