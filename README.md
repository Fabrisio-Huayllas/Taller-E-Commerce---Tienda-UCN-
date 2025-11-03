# Tienda UCN – Ecommerce Platform

Este proyecto implementa una **API REST** utilizando **ASP.NET Core 9** y **SQLite** para la creación de una plataforma de comercio electrónico llamada **Tienda UCN**.  
El sistema incluye autenticación de usuarios con **JWT**, gestión de productos, carrito de compras, pedidos y subida de imágenes a **Cloudinary**.

El proyecto sigue una **arquitectura limpia (Clean Architecture)** con el **patrón Repository**, asegurando separación de responsabilidades, fácil mantenimiento y escalabilidad.

---

## Instalación

### Requisitos previos

Antes de ejecutar el proyecto, instala lo siguiente:

- [Visual Studio Code 1.89.1+](https://code.visualstudio.com/)
- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
- [Git 2.45.1+](https://git-scm.com/downloads)
- [Postman](https://www.postman.com/downloads/)

### Pasos de instalación

1. **Clona este repositorio:**

   ```bash
   git clone https://github.com/Fabrisio-Huayllas/Taller-E-Commerce---Tienda-UCN-.git
   ```

2. **Navega al directorio del proyecto:**

   ```bash
   cd TiendaProyecto
   ```

3. **Abre el proyecto en Visual Studio Code:**

   ```bash
   code .
   ```

4. **Configura el archivo de settings:**

   Copia el archivo `appsettings.example.json` y crea un nuevo archivo `appsettings.json`:

   ```bash
   cp appsettings.example.json appsettings.json
   ```

5. **Edita `appsettings.json` con tus credenciales:**

   ```json
   {
     "ConnectionStrings": {
       "SqliteDatabase": "Data Source=app.db"
     },
     "Serilog": {
       "MinimumLevel": {
         "Default": "Debug",
         "Override": {
           "Microsoft": "Information",
           "System": "Information"
         }
       },
       "WriteTo": [
         {
           "Name": "Console"
         },
         {
           "Name": "File",
           "Args": {
             "path": "Logs/myapp-.txt",
             "rollingInterval": "Day"
           }
         }
       ]
     },
     "IdentityConfiguration": {
       "AllowedUserNameCharacters": "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"
     },
     "Cloudinary": {
       "CloudName": "your_cloud_name",
       "ApiKey": "your_api_key",
       "ApiSecret": "your_api_secret"
     },
     "JWTSecret": "your_jwt_secret_min_32_characters",
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
     "Jobs": {
       "CronJobDeleteUnconfirmedUsers": "50 20 * * *",
       "TimeZone": "Pacific SA Standard Time",
       "DaysOfDeleteUnconfirmedUsers": 7
     },
     "HangfireDashboard": {
       "StatsPollingInterval": 5000,
       "DashboardTitle": "Tienda UCN - Panel de Control de Hangfire",
       "DashboardPath": "/hangfire",
       "DisplayStorageConnectionString": false
     },
     "VerificationCode": {
       "ExpirationTimeInMinutes": 3
     },
     "EmailConfiguration": {
       "WelcomeSubject": "Bienvenido a la Tienda UCN",
       "From": "Tienda UCN <onboarding@resend.dev>",
       "VerificationSubject": "Codigo de verificacion"
     },
     "User": {
       "AdminUser": {
         "Email": "admin@tiendaucn.com",
         "Password": "Admin123!",
         "FirstName": "Administrador",
         "LastName": "Sistema",
         "Gender": "Masculino",
         "Rut": "12345678-9",
         "BirthDate": "1990-01-01",
         "PhoneNumber": "+569 12345678"
       },
       "RandomUserPassword": "Usuario123!",
       "Genders": [
         "Masculino",
         "Femenino",
         "Otro"
       ]
     }
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

## Características Implementadas

### 1. Sistema de Autenticación Completo

**Registro de Usuarios**
- Validación de datos (nombre, apellido, RUT chileno, email, contraseña, etc.).
- Contraseñas seguras mediante hashing (ASP.NET Core Identity).
- Asignación automática del rol `Cliente`.

**Verificación de Cuenta**
- Generación de código de 6 dígitos enviado por correo electrónico.
- Activación de cuenta tras la verificación.
- Expiración configurable de códigos (3 minutos por defecto).

**Inicio de Sesión**
- Generación de tokens **JWT** con claims (ID, rol, email, expiración).
- Manejo seguro de errores de autenticación.
- Opción "Recordarme" para extender duración del token.

**Recuperación de Contraseña**
- Envío de código de recuperación por correo.
- Validación del código y restablecimiento seguro.

**Cambio de Contraseña**
- Validación de contraseña actual y actualización segura.

**Gestión de Perfil**
- Consulta y actualización de datos del usuario.
- Validación de información personal.

---

###  2. CRUD Completo de Productos

**Para Administradores:**
- Creación de productos con validación completa (título, precio, stock, categoría, marca, imágenes).
- Actualización de productos existentes.
- Eliminación lógica de productos.
- Cambio de estado activo/inactivo.
- Gestión de descuentos por producto.
- Subida y eliminación de múltiples imágenes por producto.
- Vista completa con stock real y estado.

**Para Clientes:**
- Listado de productos activos con filtros y paginación.
- Búsqueda por término, categoría y marca.
- Filtros por rango de precio.
- Vista optimizada sin información administrativa.

---

###  3. Gestión Avanzada de Carrito de Compras

**Funcionalidades del Carrito**
- Creación automática de carrito para usuarios anónimos y autenticados.
- Agregar productos al carrito con validación de stock.
- Actualizar cantidades de productos.
- Eliminar productos individuales o limpiar carrito completo.
- Asociación automática de carrito anónimo con usuario al autenticarse.
- Persistencia mediante cookies HTTP-only.

**Cálculos Automáticos**
- Subtotal y total con descuentos aplicados.
- Visualización de ahorros generados.
- Validación de disponibilidad de productos.
- Recálculo automático de totales.

**Checkout**
- Validación de stock antes de procesar.
- Ajuste de cantidades según disponibilidad.
- Preparación para conversión a orden.
   * Nota Importante: se debe hacer el checkout y despues volver a obtener el carrito (metodo get) para proceder con la orden.

---

###  4. Sistema Completo de Órdenes

**Para Clientes:**
- Conversión de carrito a orden de compra.
- Generación de código único de orden.
- Historial de órdenes con paginación y filtros.
- Detalle completo de cada orden.
- Búsqueda por código de orden.

**Para Administradores:**
- Lista de todas las órdenes del sistema.
- Cambio de estado de órdenes (Creada, Pagada, Enviada, Entregada, Cancelada).
- Filtros avanzados (cliente, fecha, estado, monto).
- Seguimiento de órdenes con notas administrativas.
- Auditoría de cambios de estado.

**Funcionalidades Automáticas**
- Actualización automática de stock al crear orden.
- Limpieza del carrito tras crear la orden.
- Validación de transiciones de estado válidas.

---

###  5. Gestión de Imágenes con Cloudinary

**Subida de Imágenes**
- Validación de tipo de archivo (JPG, JPEG, PNG, WEBP).
- Validación de tamaño máximo (5MB).
- Transformación automática (redimensionado, optimización).
- Almacenamiento seguro en Cloudinary.

**Gestión de Imágenes**
- Múltiples imágenes por producto.
- Eliminación de imágenes individuales.
- URLs optimizadas para diferentes tamaños.
- Imagen por defecto cuando no hay imágenes disponibles.

---

###  6. Gestión de Categorías y Marcas

**Categorías**
- CRUD completo de categorías.
- Generación automática de slugs únicos.
- Validación de nombres únicos.
- Conteo de productos por categoría.
- Eliminación con validación de dependencias.

**Marcas**
- CRUD completo de marcas.
- Generación automática de slugs únicos.
- Validación de nombres únicos.
- Conteo de productos por marca.
- Eliminación con validación de dependencias.

---

###  7. Administración de Usuarios

**Para Administradores:**
- Listado de usuarios con filtros avanzados.
- Búsqueda por email, rol y estado.
- Detalle completo de usuarios.
- Cambio de estado (activo/bloqueado).
- Cambio de roles con validaciones de seguridad.
- Auditoría de cambios realizados.

**Validaciones de Seguridad:**
- Prevención de auto-bloqueo de administradores.
- Validación de que siempre exista al menos un administrador.
- Registro de auditoría para cambios críticos.

---

###  8. Middleware Personalizado

**Manejo de Errores (ErrorHandlerMiddleware)**
- Captura global de excepciones no manejadas.
- Mapeo de excepciones a códigos HTTP apropiados.
- Respuestas JSON estandarizadas con información de error.
- Logging detallado de errores.

**Correlación (CorrelationMiddleware)**
- Asignación de ID único (`X-Correlation-ID`) a cada solicitud.
- Trazabilidad completa de requests.
- Facilitación de debugging y monitoreo.

**Gestión de Carrito (CartMiddleware)**
- Manejo automático de cookies de carrito.
- Generación de IDs únicos para compradores.
- Configuración de cookies seguras (HttpOnly, Secure).

---

###  9. Trabajos en Segundo Plano (Hangfire)

**Tareas Automatizadas**
- Eliminación automática de usuarios no confirmados.
- Configuración mediante cron expressions.
- Panel de control de Hangfire integrado.
- Reintentos automáticos en caso de fallo.

**Configuración**
- Zona horaria configurable.
- Intervalos personalizables.
- Monitoreo en tiempo real.

---

###  10. Logging Estructurado con Serilog

**Sistema de Logs**
- Logging estructurado con Serilog.
- Escritura simultánea en consola y archivos.
- Rotación diaria de archivos.
- Correlación de logs con requests HTTP.
- Niveles de log configurables.

---

###  11. Mapeo de Objetos con Mapster

**Configuraciones de Mapeo**
- Mapeo automático entre entidades y DTOs.
- Configuraciones personalizadas para cada dominio.
- Optimización de rendimiento.
- Transformaciones de datos específicas.

---

###  12. Validaciones Personalizadas

**Validadores Personalizados**
- Validación de RUT chileno con dígito verificador.
- Validación de fecha de nacimiento (mayor de edad).
- Validaciones de archivos de imagen.
- Validaciones de negocio específicas.

---

##  Endpoints Principales

###  Autenticación
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **POST** | `/api/auth/register` | Registro de usuarios | Público |
| **POST** | `/api/auth/login` | Inicio de sesión | Público |
| **POST** | `/api/auth/recover-password` | Solicitud de recuperación | Público |
| **POST** | `/api/auth/reset-password` | Restablecer contraseña | Público |
| **POST** | `/api/auth/verify-email` | Verificación de cuenta | Público |
| **POST** | `/api/auth/resend-email-verification-code` | Reenvío código verificación | Público |
| **PATCH** | `/api/auth/change-password` | Cambio de contraseña | Autenticado |
| **GET** | `/api/auth/profile` | Obtener perfil de usuario | Autenticado |
| **PUT** | `/api/auth/profile` | Actualizar perfil de usuario | Autenticado |

###  Productos (Administrador)
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **POST** | `/api/product` | Crear un producto | Admin |
| **GET** | `/api/product/admin/{id}` | Obtener producto por ID (admin) | Admin |
| **GET** | `/api/product/admin/products` | Listar productos (admin) | Admin |
| **PUT** | `/api/product/admin/products/{id}` | Actualizar producto | Admin |
| **DELETE** | `/api/product/admin/products/{id}` | Eliminar producto | Admin |
| **PATCH** | `/api/product/{id}/toggle-active` | Activar/desactivar producto | Admin |
| **PATCH** | `/api/product/{id}/discount` | Actualizar descuento | Admin |
| **POST** | `/api/product/{id}/images` | Agregar imágenes | Admin |
| **DELETE** | `/api/product/{id}/images/{imageId}` | Eliminar imagen | Admin |

###  Productos (Cliente)
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **GET** | `/api/product/products` | Listar productos (cliente) | Público |
| **GET** | `/api/product/{id}` | Obtener detalle de producto | Público |

###  Carrito de Compras
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **GET** | `/api/cart` | Obtener carrito actual | Autenticado |
| **POST** | `/api/cart/items` | Agregar producto al carrito | Autenticado |
| **DELETE** | `/api/cart/items/{productId}` | Eliminar producto del carrito | Autenticado |
| **PATCH** | `/api/cart/items` | Actualizar cantidad de producto | Autenticado |
| **POST** | `/api/cart/clear` | Limpiar carrito | Autenticado |
| **POST** | `/api/cart/checkout` | Procesar checkout | Customer |

###  Órdenes (Cliente)
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **POST** | `/api/orders/create` | Crear nueva orden | Customer |
| **GET** | `/api/orders/detail/{orderCode}` | Obtener detalle de orden | Customer |
| **GET** | `/api/orders/user-orders` | Listar órdenes del usuario | Customer |

###  Órdenes (Administrador)
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **GET** | `/api/orders/admin/list` | Listar todas las órdenes | Admin |
| **PUT** | `/api/orders/admin/{orderCode}/status` | Cambiar estado de orden | Admin |

###  Administración de Usuarios
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **GET** | `/api/admin/users` | Listar usuarios | Admin |
| **GET** | `/api/admin/users/{id}` | Obtener detalle de usuario | Admin |
| **PATCH** | `/api/admin/users/{id}/status` | Cambiar estado de usuario | Admin |
| **PATCH** | `/api/admin/users/{id}/role` | Cambiar rol de usuario | Admin |

###  Categorías
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **GET** | `/api/admin/category` | Listar categorías | Admin |
| **GET** | `/api/admin/category/{id}` | Obtener categoría por ID | Admin |
| **POST** | `/api/admin/category` | Crear categoría | Admin |
| **PUT** | `/api/admin/category/{id}` | Actualizar categoría | Admin |
| **DELETE** | `/api/admin/category/{id}` | Eliminar categoría | Admin |

###  Marcas
| Método | Endpoint | Descripción | Roles |
|:--------|:-----------|:-------------|:--------|
| **GET** | `/api/admin/brand` | Listar marcas | Admin |
| **GET** | `/api/admin/brand/{id}` | Obtener marca por ID | Admin |
| **POST** | `/api/admin/brand` | Crear marca | Admin |
| **PUT** | `/api/admin/brand/{id}` | Actualizar marca | Admin |
| **DELETE** | `/api/admin/brand/{id}` | Eliminar marca | Admin |

---

##  Tecnologías Utilizadas

### **Backend**
- **ASP.NET Core 9** - Framework web principal
- **Entity Framework Core** - ORM para acceso a datos
- **SQLite** - Base de datos ligera
- **ASP.NET Core Identity** - Sistema de autenticación y autorización

### **Autenticación y Seguridad**
- **JWT (JSON Web Tokens)** - Autenticación stateless
- **BCrypt** - Hashing de contraseñas

### **Validación**
- **FluentValidation** - Validación de DTOs
- **Data Annotations** - Validaciones básicas
- **Validadores personalizados** - RUT chileno, fechas, imágenes

### **Servicios Externos**
- **Cloudinary API** - Almacenamiento y gestión de imágenes
- **Resend** - Envío de correos electrónicos

### **Trabajos en Segundo Plano**
- **Hangfire** - Procesamiento de tareas en background
- **SQLite Storage** - Persistencia de trabajos

### **Logging y Monitoreo**
- **Serilog** - Logging estructurado
- **Console Sink** - Logs en consola
- **File Sink** - Logs en archivos con rotación diaria

### **Documentación API**
- **Swagger / OpenAPI** - Documentación interactiva de la API

### **Mapeo de Objetos**
- **Mapster** - Mapeo entre DTOs y entidades

### **Procesamiento de Imágenes**
- **SkiaSharp** - Validación y procesamiento de imágenes
- **Cloudinary SDK** - Transformación y optimización

---

##  Arquitectura del Proyecto

```
src/
├── API/
│   └── Controllers/              # Controladores de la API
│       ├── AuthController.cs     # Autenticación
│       ├── ProductController.cs  # Gestión de productos
│       ├── CartController.cs     # Carrito de compras
│       ├── OrderController.cs    # Gestión de órdenes
│       ├── UsersController.cs    # Administración de usuarios
│       ├── CategoryController.cs # Gestión de categorías
│       ├── BrandController.cs    # Gestión de marcas
│       └── BaseController.cs     # Controlador base
├── Application/
│   ├── DTO/                      # Data Transfer Objects
│   │   ├── AuthDTO/             # DTOs de autenticación
│   │   ├── ProductDTO/          # DTOs de productos
│   │   ├── CartDTO/             # DTOs de carrito
│   │   ├── OrderDTO/            # DTOs de órdenes
│   │   ├── UserDTO/             # DTOs de usuarios
│   │   ├── CategoryDTO/         # DTOs de categorías
│   │   ├── BrandDTO/            # DTOs de marcas
│   │   └── BaseResponse/        # DTOs de respuesta base
│   ├── Services/                # Servicios de aplicación
│   │   ├── Interfaces/          # Interfaces de servicios
│   │   └── Implements/          # Implementaciones de servicios
│   ├── Mappers/                 # Mapeo entre entidades y DTOs
│   ├── Jobs/                    # Trabajos en segundo plano
│   │   ├── Interfaces/          # Interfaces de jobs
│   │   └── Implements/          # Implementaciones de jobs
│   ├── Validators/              # Validaciones personalizadas
│   └── Templates/               # Plantillas de email
│       └── Email/               # Plantillas HTML para emails
├── Domain/
│   ├── Models/                  # Entidades del dominio
│   │   ├── User.cs              # Usuario
│   │   ├── Product.cs           # Producto
│   │   ├── Cart.cs              # Carrito
│   │   ├── Order.cs             # Orden
│   │   ├── Category.cs          # Categoría
│   │   ├── Brand.cs             # Marca
│   │   └── ...                  # Otras entidades
│   └── Enums/                   # Enumeraciones del dominio
├── Infrastructure/
│   ├── Data/                    # Contexto de base de datos
│   │   ├── DataContext.cs       # Contexto principal
│   │   ├── DataSeeder.cs        # Sembrado de datos
│   │   └── DesignTimeDbContextFactory.cs
│   └── Repositories/            # Implementación de repositorios
│       ├── Interfaces/          # Interfaces de repositorios
│       └── Implements/          # Implementaciones de repositorios
├── Middleware/                  # Middleware personalizado
│   ├── ErrorHandlerMiddleware.cs    # Manejo de errores
│   ├── CorrelationMiddleware.cs     # Correlación de requests
│   └── CartMiddleware.cs            # Gestión de carrito
└── Exceptions/                  # Excepciones personalizadas
    ├── AppException.cs          # Excepción base
    ├── BadRequestAppException.cs # Errores 400
    ├── NotFoundException.cs     # Errores 404
    ├── ConflictException.cs     # Errores 409
    ├── ForbiddenException.cs    # Errores 403
    └── UnauthorizedAppException.cs # Errores 401
```

---

##  Patrones de Diseño Implementados

- **Repository Pattern** - Abstracción del acceso a datos
- **Dependency Injection** - Inversión de control
- **DTO Pattern** - Transferencia de datos entre capas
- **Service Layer Pattern** - Lógica de negocio encapsulada
- **Middleware Pattern** - Procesamiento de requests HTTP
- **Factory Pattern** - Creación de objetos complejos
- **Mapper Pattern** - Transformación de objetos
- **Strategy Pattern** - Algoritmos intercambiables

---

##  Seguridad

- **Autenticación JWT** con expiración configurable
- **Autorización basada en roles** (Admin, Customer)
- **Validación de entrada** en todos los endpoints
- **Hashing seguro de contraseñas** con ASP.NET Core Identity
- **Headers de correlación** para trazabilidad
- **Validación de archivos** antes de subir a Cloudinary
- **Cookies HTTP-only** para gestión de carrito
- **Prevención de ataques** comunes (XSS, CSRF)
- **Validación de transiciones de estado**
- **Auditoría de cambios críticos**

---

##  Configuración Adicional

### **Variables de Entorno Requeridas**
- `Cloudinary:CloudName` - Nombre del cloud de Cloudinary
- `Cloudinary:ApiKey` - API Key de Cloudinary
- `Cloudinary:ApiSecret` - API Secret de Cloudinary
- `JWTSecret` - Clave secreta para JWT (mínimo 32 caracteres)
- `ResendAPIKey` - API Key de Resend para envío de emails

### **Configuración de Hangfire**
- Panel accesible en `/hangfire`
- Trabajos recurrentes configurables via cron expressions
- Zona horaria configurable
- Reintentos automáticos con delays incrementales

### **Configuración de Logging**
- Niveles de log configurables
- Rotación diaria de archivos
- Formato estructurado JSON
- Correlación con requests HTTP

---

##  Funcionalidades Avanzadas

### **Gestión de Stock**
- Validación en tiempo real de disponibilidad
- Actualización automática tras crear órdenes
- Indicadores de stock (Alto, Medio, Bajo, Agotado)

### **Sistema de Descuentos**
- Descuentos por producto
- Cálculo automático de precios finales
- Visualización de ahorros en carrito

### **Filtros y Búsquedas**
- Búsqueda por texto en productos
- Filtros por categoría, marca y precio
- Ordenamiento personalizable
- Paginación optimizada

### **Auditoría y Trazabilidad**
- Logging de todas las operaciones críticas
- Correlación de requests con IDs únicos
- Auditoría de cambios de usuario
- Historial de cambios de estado de órdenes

---

##  Enlaces Útiles

- [Documentación de ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Hangfire Documentation](https://docs.hangfire.io/)
- [Cloudinary Documentation](https://cloudinary.com/documentation)
- [Serilog Documentation](https://serilog.net/)

---

##  Integrantes del grupo

- **Sebastian Hernandez** - 21.701.267-8 - sebastian.hernandez02@alumnos.ucn.cl
- **Fabrisio Huayllas** - 22.108.928-6 - fabrisio.huayllas02@alumnos.ucn.cl
- **Rodrigo Tapia** - 21.382.034-6 - rodrigo04@alumnos.ucn.cl

---

##  Licencia

Este proyecto es desarrollado con fines académicos para la Universidad Católica del Norte.
