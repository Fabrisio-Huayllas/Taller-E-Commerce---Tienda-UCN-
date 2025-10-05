#  Tienda UCN – Ecommerce Platform

Este proyecto implementa una **API REST** utilizando **ASP.NET Core 9** y **SQLite** para la creación de una plataforma de comercio electrónico llamada **Tienda UCN**.  
El sistema incluye autenticación de usuarios con **JWT**, gestión de productos, carrito de compras, pedidos y subida de imágenes a **Cloudinary**.

El proyecto sigue una **arquitectura limpia (Clean Architecture)** con el **patrón Repository**, asegurando separación de responsabilidades, fácil mantenimiento y escalabilidad.

---

##  Instalación

###  Requisitos previos

Antes de ejecutar el proyecto, instala lo siguiente:

- [Visual Studio Code 1.89.1+](https://code.visualstudio.com/)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Git 2.45.1+](https://git-scm.com/downloads)
- [Postman](https://www.postman.com/downloads/)

###  Pasos de instalación

1. **Clona este repositorio:**

   ```bash
   git clone https://github.com/usuario/tienda-ucn.git
   ```

2. **Navega al directorio del proyecto:**

   ```bash
   cd tienda-ucn
   ```

3. **Abre el proyecto en Visual Studio Code:**

   ```bash
   code .
   ```

4. **Configura el archivo de settings:**

   Copia el archivo `appsettings.example.json` y crea un nuevo archivo `appsettings.json`.

5. **Edita `appsettings.json` con tus credenciales:**

   ```json
   "IdentityConfiguration":
          {
            "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
          },
          "Cloudinary": {
            "CloudName": "your_cloud_name",
            "ApiKey": "your_api_key",
            "ApiSecret": "your_api"
          },
          "JWTSecret": "your_jwt_secret",
          "ResendAPIKey": "your_resend_api_key",
          "Products": {
            "FewUnitsAvailable": 15,
            "DefaultImageUrl": "https://shop.songprinting.com/global/images/PublicShop/ProductSearch/prodgr_default_300.png",
            "DefaultPageSize": 10,
            "ImageAllowedExtensions": [".jpg", ".jpeg", ".png", ".webp"],
            "ImageMaxSizeInBytes": 5242880,
            "TransformationWidth": 1000,
            "TransformationCrop": "scale",
            "TransformationQuality": "auto:best",
            "TransformationFetchFormat": "auto"
          },
          "CookieExpirationDays": 30,
          "Jobs":{
            "CronJobDeleteUnconfirmedUsers": "50 20 * * *",
            "TimeZone": "Pacific SA Standard Time",
            "DaysOfDeleteUnconfirmedUsers": 7
          },
          "HangfireDashboard":{
            "StatsPollingInterval": 5000,
            "DashboardTitle": "Panel de Control de Hangfire",
            "DashboardPath": "/hangfire",
            "DisplayStorageConnectionString": false
          },
          "VerificationCode": {
            "ExpirationTimeInMinutes": 3
          },
          "EmailConfiguration": {
            "WelcomeSubject": "your-welcome-subject",
            "From": "your-email@example.com",
            "VerificationSubject": "your-verification-subject"
          },
          "User": {
            "AdminUser":
            {
              "Email": "your-admin-email",
              "Password": "your-admin-Password",
              "FirstName": "your-admin-FirstName",
              "LastName": "your-admin-LastName",
              "Gender": "your-admin-Gender",
              "Rut": "your-admin-Rut",
              "BirthDate": "your-admin-BirthDate",
              "PhoneNumber": "your-admin-PhoneNumber"
            },
            "RandomUserPassword": "your-User-Random-Password"
          }
   ```

6. **Restaura las dependencias:**

   ```bash
   dotnet restore
   ```

7. **Aplica las migraciones a la base de datos:**

   ```bash
   dotnet ef database update
   ```

8. **Ejecuta el proyecto:**

   ```bash
   dotnet run
   ```

---

##  Características Implementadas

###  1. Sistema de Autenticación Completo

**Registro de Usuarios**
- Validación de datos (nombre, apellido, RUT chileno, email, contraseña, etc.).
- Contraseñas seguras mediante hashing (ASP.NET Core Identity).
- Asignación automática del rol `Cliente`.

**Verificación de Cuenta**
- Generación de código de 6 dígitos enviado por correo electrónico.
- Activación de cuenta tras la verificación.

**Inicio de Sesión**
- Generación de tokens **JWT** con claims (ID, rol, email, expiración).
- Manejo seguro de errores de autenticación.

**Recuperación de Contraseña**
- Envío de código de recuperación por correo.
- Validación del código y restablecimiento seguro.

**Cambio de Contraseña**
- Validación de contraseña actual y actualización segura.

---

###  2. CRUD Básico de Productos (Administrador)

**Creación**
- Validación mediante `CreateProductDTO` (nombre, precio, stock, categoría, marca, etc.).
- Persistencia de productos en base de datos.
- Endpoint protegido para rol **Administrador**.

**Consulta**
- Obtener detalle de un producto por ID.
- Listado con filtros y paginación.

**Actualización y Eliminación**
- Endpoints protegidos para actualizar o eliminar productos.

---

###  3. Subida de Imágenes con Cloudinary

- Subida de imágenes asociadas a productos.
- Validación de tipo, tamaño y formato.
- Almacenamiento de la URL en base de datos.

---

###  4. Middleware

**Manejo de Errores**
- Middleware para capturar excepciones y devolver respuestas JSON con mensajes claros.

**Correlación**
- Middleware que agrega un identificador único (`X-Correlation-ID`) a cada solicitud para facilitar el rastreo.

---

##  Endpoints Principales

###  Autenticación
| Método | Endpoint | Descripción |
|:--------|:-----------|:-------------|
| **POST** | `/api/auth/register` | Registro de usuarios |
| **POST** | `/api/auth/login` | Inicio de sesión |
| **POST** | `/api/auth/recover-password` | Solicitud de recuperación |
| **PATCH** | `/api/auth/reset-password` | Restablecer contraseña |
| **POST** | `/api/auth/verify-email` | Verificación de cuenta |
| **POST** | `/api/auth/resend-email-verification-code` | Verificación de cuenta |
| **PUT** | `/api/auth/change-password` | Cambio de contraseña |

###  Productos (Administrador)
| Método | Endpoint | Descripción |
|:--------|:-----------|:-------------|
| **POST** | `/api/admin/product` | Crear un producto |
| **GET** | `/api/admin/product/{id}` | Obtener un producto por ID |
| **GET** | `/api/product/admin/products` | Filtrar por paginas  |
| **PATCH** | `/api/admin/products/{id}/toggie-active` | Desactiva el producto |

###  Productos (Usuario)
| Método | Endpoint | Descripción |
|:--------|:-----------|:-------------|
| **GET** | `/api/product/customer/product` | Filtrar por paginas  |


---

##  Tecnologías Utilizadas

- **ASP.NET Core 9**
- **Entity Framework Core**
- **SQLite**
- **Hangfire**
- **Cloudinary API**
- **Serilog**
- **JWT Authentication**
- **FluentValidation**
- **Swagger / OpenAPI**

---


 
