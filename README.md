[TiendaUCN.postman_collection.json](https://github.com/user-attachments/files/23294711/TiendaUCN.postman_collection.json)# Tienda UCN – Ecommerce Platform

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
   Nota: Asegurese que los datos de la appsettigns concuerden con lo que se ponga en el postman.

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

## Postman
[Upl{
	"info": {
		"_postman_id": "a01c58de-b022-4e79-b2c0-134da615fe8f",
		"name": "TiendaUCN",
		"description": "Colección completa para probar la API de TiendaUCN. Incluye flujos de autenticación, compra y administración, con validación automática y captura de variables directamente en la colección para fácil portabilidad.",
		"schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json",
		"_exporter_id": "47835012",
		"_collection_link": "https://rorrox-s-team.postman.co/workspace/Team-Workspace~63220b38-a76c-468e-8993-d31b8d288aa1/collection/47835012-a01c58de-b022-4e79-b2c0-134da615fe8f?action=share&source=collection_link&creator=47835012"
	},
	"item": [
		{
			"name": "Authentication",
			"item": [
				{
					"name": "[Auth] Login Admin",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"Token de Admin y UserId recibidos\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.token).to.be.a('string');",
									"    pm.expect(jsonData.userId).to.be.a('number');",
									"    // Guardar token y ID para flujos de admin",
									"    pm.collectionVariables.set(\"adminToken\", jsonData.token);",
									"    pm.collectionVariables.set(\"adminUserId\", jsonData.userId);",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n    \"Email\":\"{{testAdminEmail}}\",\r\n    \"Password\":\"{{testAdminPassword}}\"\r\n\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/login",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"login"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Login Cliente",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"Token y UserId recibidos\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.token).to.be.a('string');",
									"    pm.expect(jsonData.userId).to.be.a('number');",
									"    // Guardar token y ID para flujos de cliente",
									"    pm.collectionVariables.set(\"authToken\", jsonData.token);",
									"    pm.collectionVariables.set(\"currentUserId\", jsonData.userId);",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n    \"email\":\"{{testUserEmail}}\",\r\n    \"password\":\"{{testUserPassword}}\"\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/login",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"login"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Registrar Nuevo Usuario (Dinámico)",
					"event": [
						{
							"listen": "prerequest",
							"script": {
								"exec": [
									"// Genera un email único usando un timestamp",
									"pm.collectionVariables.set(\"testUserEmail\", \"testuser_\" + Date.now() + \"@tiendaucn.com\");",
									"pm.collectionVariables.set(\"testUserPassword\", \"TestUser123!\");",
									"pm.collectionVariables.set(\"testUserRut\", \"11\" + Math.floor(100000 + Math.random() * 900000) + \"-K\");"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						},
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200 (OK)\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"Respuesta tiene mensaje de éxito\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.message).to.eql(\"Registro exitoso\");",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\"Email\":\"{{testUserEmail}}\",\r\n\"Password\":\"{{testUserPassword}}\",\r\n\"confirmPassword\":\"{{testUserPassword}}\",\r\n\"rut\": \"21382034-6\",\r\n\"firstName\": \"Rodrigo\",\r\n\"LastName\":\"Tapia\",\r\n\"BirthDate\":\"2003-01-01T19:16:50.085Z\",\r\n\"PhoneNumber\":\"123456789\",\r\n\"Gender\":\"Masculino\"\r\n}\r\n",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/register",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"register"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Registrar Mismo Usuario (Espera Error 400/409)",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 400 or 409 (Conflicto/Bad Request)\", function () {",
									"    pm.expect(pm.response.code).to.be.oneOf([400, 409]);",
									"});",
									"pm.test(\"Respuesta indica que el usuario ya existe\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.message).to.contain(\"ya está registrado\");",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\"Email\":\"{{testUserEmail}}\",\r\n\"Password\":\"{{testUserPassword}}\",\r\n\"confirmPassword\":\"{{testUserPassword}}\",\r\n\"rut\": \"{{testUserRut}}\",\r\n\"firstName\": \"Rodrigo (Test)\",\r\n\"LastName\":\"Tapia (Test)\",\r\n\"BirthDate\":\"2003-01-01T19:16:50.085Z\",\r\n\"PhoneNumber\":\"123456789\",\r\n\"Gender\":\"Masculino\"\r\n}\r\n",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/register",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"register"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Verificar Email (Usar código de email)",
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n    \"email\":\"{{testUserEmail}}\",\r\n    \"verificationCode\":\"860355\"\r\n\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/verify-email",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"verify-email"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Reenviar Código Verificación",
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n    \"email\":\"{{testUserEmail}}\"\r\n\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/resend-email-verification-code",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"resend-email-verification-code"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Solicitar Recuperar Contraseña",
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n  \"email\": \"{{testUserEmail}}\"\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/recover-password",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"recover-password"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Auth] Resetear Contraseña (Usar código de email)",
					"request": {
						"method": "POST",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n  \"email\": \"{{testUserEmail}}\",\r\n  \"code\": \"PASTE_CODE_FROM_EMAIL\",\r\n  \"newPassword\": \"{{testUserPassword}}\"\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/reset-password",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"reset-password"
							]
						}
					},
					"response": []
				}
			],
			"description": "Flujo 1: Sistema de Autenticación Completo. Incluye registro, verificación, login y recuperación."
		},
		{
			"name": "Users (Profile)",
			"item": [
				{
					"name": "[Profile] Obtener Perfil",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"Perfil coincide con el usuario logueado\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.id).to.eql(parseInt(pm.collectionVariables.get(\"currentUserId\")));",
									"    pm.expect(jsonData.email).to.eql(pm.collectionVariables.get(\"testUserEmail\"));",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "GET",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/auth/profile",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"profile"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Profile] Actualizar Perfil",
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "PUT",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\r\n   \"firstName\": \"Fabrisio (Editado)\",\r\n    \"lastName\": \"Huayllas (Editado)\",\r\n    \"phoneNumber\": \"987654322\"\r\n}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/profile",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"profile"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Profile] Cambiar Contraseña",
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "PATCH",
						"header": [],
						"body": {
							"mode": "raw",
							"raw": "{\"currentPassword\": \"{{testUserPassword}}\",\r\n  \"newPassword\": \"NewPassword123!\",\r\n  \"confirmNewPassword\": \"NewPassword123!\"}",
							"options": {
								"raw": {
									"language": "json"
								}
							}
						},
						"url": {
							"raw": "{{baseUrl}}/auth/change-password",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"change-password"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Profile] Acceso Protegido con Token Inválido",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 401 (Unauthorized)\", function () {",
									"    pm.response.to.have.status(401);",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "untokeninvalido123",
									"type": "string"
								}
							]
						},
						"method": "GET",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/auth/profile",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"auth",
								"profile"
							]
						}
					},
					"response": []
				}
			],
			"description": "Flujo 2: Gestión de Cuenta de Usuario (Rutas de /api/auth que manejan perfiles)"
		},
		{
			"name": "Products (Customer)",
			"item": [
				{
					"name": "[Products] Listar Productos (Cliente)",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"La respuesta contiene lista de productos\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.data.products).to.be.an('array');",
									"    // Guardar el ID del primer producto para el flujo de compra",
									"    if (jsonData.data.products.length > 0) {",
									"        pm.collectionVariables.set(\"productId\", jsonData.data.products[0].id); // Asumiendo que el DTO tiene 'id'",
									"    }",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"method": "GET",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/product/products?PageNumber=1",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"product",
								"products"
							],
							"query": [
								{
									"key": "PageNumber",
									"value": "1"
								},
								{
									"key": "PageSize",
									"value": "2",
									"disabled": true
								},
								{
									"key": "SearchTerm",
									"value": "gorg\n",
									"disabled": true
								},
								{
									"key": "CategoryId",
									"value": "1",
									"disabled": true
								},
								{
									"key": "BrandId",
									"value": "3",
									"disabled": true
								}
							]
						}
					},
					"response": []
				},
				{
					"name": "[Products] Ver Detalle de Producto (Cliente)",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"El ID del producto coincide con el solicitado\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.data.id).to.eql(parseInt(pm.collectionVariables.get(\"productId\")));",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"method": "GET",
						"header": [],
						"url": {
							"raw": "http://localhost:5043/api/product/{{productId}}",
							"protocol": "http",
							"host": [
								"localhost"
							],
							"port": "5043",
							"path": [
								"api",
								"product",
								"{{productId}}"
							]
						}
					},
					"response": []
				}
			],
			"description": "Flujo 5: Catálogo de Productos (Público y Cliente)"
		},
		{
			"name": "Cart",
			"item": [
				{
					"name": "[Cart] Obtener Carrito",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"Respuesta de carrito obtenida\", function () {",
									"    var jsonData = pm.response.json();",
									"    pm.expect(jsonData.data.items).to.be.an('array');",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "GET",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/cart",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"cart"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Cart] Agregar Item al Carrito",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});",
									"pm.test(\"El item está en el carrito\", function () {",
									"    var jsonData = pm.response.json();",
									"    var addedItem = jsonData.data.items.find(item => item.productId == pm.collectionVariables.get(\"productId\"));",
									"    pm.expect(addedItem).to.not.be.undefined;",
									"    pm.expect(addedItem.quantity).to.eql(2);",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "POST",
						"header": [],
						"body": {
							"mode": "formdata",
							"formdata": [
								{
									"key": "productId",
									"value": "{{productId}}",
									"type": "text"
								},
								{
									"key": "quantity",
									"value": "3",
									"type": "text"
								}
							]
						},
						"url": {
							"raw": "{{baseUrl}}/cart/items",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"cart",
								"items"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Cart] Actualizar Cantidad de Item",
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "PATCH",
						"header": [],
						"body": {
							"mode": "formdata",
							"formdata": [
								{
									"key": "productId",
									"value": "{{productId}}",
									"type": "text"
								},
								{
									"key": "quantity",
									"value": "4",
									"type": "text"
								}
							]
						},
						"url": {
							"raw": "{{baseUrl}}/cart/items",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"cart",
								"items"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Cart] Checkout",
					"event": [
						{
							"listen": "test",
							"script": {
								"exec": [
									"pm.test(\"Status code is 200\", function () {",
									"    pm.response.to.have.status(200);",
									"});"
								],
								"type": "text/javascript",
								"packages": {},
								"requests": {}
							}
						}
					],
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "POST",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/cart/checkout",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"cart",
								"checkout"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Cart] Eliminar Item del Carrito",
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "DELETE",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/cart/items/{{productId}}",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"cart",
								"items",
								"{{productId}}"
							]
						}
					},
					"response": []
				},
				{
					"name": "[Cart] Limpiar Carrito",
					"request": {
						"auth": {
							"type": "bearer",
							"bearer": [
								{
									"key": "token",
									"value": "{{authToken}}",
									"type": "string"
								}
							]
						},
						"method": "POST",
						"header": [],
						"url": {
							"raw": "{{baseUrl}}/cart/clear",
							"host": [
								"{{baseUrl}}"
							],
							"path": [
								"cart",
								"clear"
							]
						}
					},
					"response": []
				}
			],
			"description": "Flujo 3: Sistema de Carrito de Compras (Requiere Auth de Cliente)"
		},
		{
			"name": "Orders (Customer & Admin)",
			"item": [
				{
					"name": "Flujo Cliente",
					"item": [
						{
							"name": "[Orders] Crear Orden (requiere items en carrito)",
							"event": [
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 201 (Created)\", function () {",
											"    pm.response.to.have.status(201);",
											"});",
											"pm.test(\"Respuesta contiene el código de la orden\", function () {",
											"    var jsonData = pm.response.json();",
											"    pm.expect(jsonData.data).to.be.a('string');",
											"    // Guardar el código de la orden para verificarla",
											"    pm.collectionVariables.set(\"orderCode\", jsonData.data);",
											"});"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{authToken}}",
											"type": "string"
										}
									]
								},
								"method": "POST",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/orders/create",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"orders",
										"create"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Orders] Verificar que el Carrito está vacío (Post-Orden)",
							"event": [
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 200\", function () {",
											"    pm.response.to.have.status(200);",
											"});",
											"pm.test(\"El carrito ahora está vacío\", function () {",
											"    var jsonData = pm.response.json();",
											"    pm.expect(jsonData.data.items).to.be.an('array').and.be.empty;",
											"});"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{authToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/cart",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"cart"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Orders] Consultar Historial de Órdenes (Cliente)",
							"event": [
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 200\", function () {",
											"    pm.response.to.have.status(200);",
											"});",
											"pm.test(\"La nueva orden aparece en el historial\", function () {",
											"    var jsonData = pm.response.json();",
											"    var newOrder = jsonData.data.orders.find(order => order.code == pm.collectionVariables.get(\"orderCode\"));",
											"    pm.expect(newOrder).to.not.be.undefined;",
											"});"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{authToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/orders/user-orders?PageNumber=1&PageSize=5",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"orders",
										"user-orders"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "5"
										},
										{
											"key": "status",
											"value": null,
											"disabled": true
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Orders] Consultar Detalle de Orden (Cliente)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{authToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/orders/detail/{{orderCode}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"orders",
										"detail",
										"{{orderCode}}"
									]
								}
							},
							"response": []
						}
					]
				},
				{
					"name": "Flujo Admin",
					"item": [
						{
							"name": "[Orders Admin] Listar Órdenes (Admin)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/orders/admin/list?PageNumber=1&PageSize=5",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"orders",
										"admin",
										"list"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "5"
										},
										{
											"key": "status",
											"value": null,
											"disabled": true
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Orders Admin] Cambiar Estado Orden (Admin)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PUT",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"status\": \"Shipped\",\r\n  \"note\": \"Enviado por DHL\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/api/orders/admin/{{orderCode}}/status",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"orders",
										"admin",
										"{{orderCode}}",
										"status"
									]
								}
							},
							"response": []
						}
					]
				}
			],
			"description": "Flujo 4 y 8: Gestión de pedidos del cliente y admin (rutas en /api/orders/)"
		},
		{
			"name": "Admin",
			"item": [
				{
					"name": "Admin Products",
					"item": [
						{
							"name": "[Admin Prod] Crear Producto",
							"event": [
								{
									"listen": "prerequest",
									"script": {
										"exec": [
											"// Variables para crear producto",
											"pm.collectionVariables.set(\"testProductName\", \"Laptop Test \" + Date.now());",
											"pm.collectionVariables.set(\"testProductPrice\", Math.floor(100000 + Math.random() * 900000));"
										],
										"type": "text/javascript"
									}
								},
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 201 (Created)\", function () {",
											"    pm.response.to.have.status(201);",
											"});",
											"pm.test(\"Respuesta contiene el ID del nuevo producto\", function () {",
											"    var jsonData = pm.response.json();",
											"    pm.expect(jsonData.data).to.be.a('string');",
											"    // Guardar el ID del producto para usarlo en actualizar/eliminar",
											"    pm.collectionVariables.set(\"newProductId\", jsonData.data);",
											"});"
										],
										"type": "text/javascript"
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "POST",
								"header": [],
								"body": {
									"mode": "formdata",
									"formdata": [
										{
											"key": "Title",
											"value": "{{testProductName}}",
											"type": "text"
										},
										{
											"key": "Description",
											"value": "Salsa preparda en lata",
											"type": "text"
										},
										{
											"key": "Price",
											"value": "{{testProductPrice}}",
											"type": "text"
										},
										{
											"key": "Stock",
											"value": "10",
											"type": "text"
										},
										{
											"key": "Status",
											"value": "New",
											"type": "text"
										},
										{
											"key": "Discount",
											"value": "0",
											"type": "text"
										},
										{
											"key": "CategoryName",
											"value": "Test Category",
											"type": "text"
										},
										{
											"key": "BrandName",
											"value": "Test Brand",
											"type": "text"
										},
										{
											"key": "Images",
											"type": "file",
											"src": "/C:/Users/rorro/Downloads/bde2db98b2b73987278cfb8f3f12f557.jpg"
										}
									]
								},
								"url": {
									"raw": "{{baseUrl}}/api/product",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"product"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Listar Productos (Admin)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/product/admin/products?PageNumber=1&PageSize=50",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"product",
										"admin",
										"products"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "50"
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Ver Detalle Producto Creado (Admin)",
							"event": [
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 200\", function () {",
											"    pm.response.to.have.status(200);",
											"});",
											"pm.test(\"El ID del producto coincide con el solicitado\", function () {",
											"    var jsonData = pm.response.json();",
											"    pm.expect(jsonData.data.id).to.eql(parseInt(pm.collectionVariables.get(\"newProductId\")));",
											"});"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/product/admin/{{newProductId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"product",
										"admin",
										"{{newProductId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Actualizar Producto",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PUT",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n    \"name\": \"{{testProductName}} (Actualizado)\",\r\n    \"description\": \"Ps5 y dos juegos.\",\r\n    \"price\": 15000,\r\n    \"stock\": 20,\r\n    \"categoryId\": 1,\r\n    \"brandId\": 2\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/product/admin/products/{{newProductId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"product",
										"admin",
										"products",
										"{{newProductId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Actualizar Descuento",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PATCH",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"discountPercent\": 20\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/product/admin/products/{{newProductId}}/discount",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"product",
										"admin",
										"products",
										"{{newProductId}}",
										"discount"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Activar/Desactivar Producto",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PATCH",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/product/{{newProductId}}/toggle-active",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"product",
										"{{newProductId}}",
										"toggle-active"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Subir Imágenes (extra)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "POST",
								"header": [],
								"body": {
									"mode": "formdata",
									"formdata": [
										{
											"key": "files",
											"type": "file",
											"src": "/C:/Users/rorro/Downloads/w=800,h=800,fit=pad.webp"
										}
									]
								},
								"url": {
									"raw": "{{baseUrl}}/api/product/{{newProductId}}/images",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"product",
										"{{newProductId}}",
										"images"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Eliminar Imagen (requiere imageId)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "DELETE",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/product/{{newProductId}}/images/1",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"product",
										"{{newProductId}}",
										"images",
										"1"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Eliminar Producto",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "DELETE",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/product/admin/products/{{newProductId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"product",
										"admin",
										"products",
										"{{newProductId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Prod] Acceso Denegado (Cliente a Admin)",
							"event": [
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 403 (Forbidden)\", function () {",
											"    pm.response.to.have.status(403);",
											"});"
										],
										"type": "text/javascript"
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{authToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/product/admin/products?PageNumber=1&PageSize=5",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"product",
										"admin",
										"products"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "5"
										}
									]
								}
							},
							"response": []
						}
					]
				},
				{
					"name": "Admin Categories",
					"item": [
						{
							"name": "[Admin Cat] Crear Categoría",
							"event": [
								{
									"listen": "prerequest",
									"script": {
										"exec": [
											"pm.collectionVariables.set(\"newCategoryName\", \"Test Category \" + Date.now());"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								},
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 201 (Created)\", function () {",
											"    pm.response.to.have.status(201);",
											"});",
											"var jsonData = pm.response.json();",
											"pm.collectionVariables.set(\"newCategoryId\", jsonData.data.id);"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "POST",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"name\": \"{{newCategoryName}}\",\r\n  \"description\": \"Dispositivos tecnológicos y gadgets\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/category",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"category"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Cat] Listar Categorías",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/category?PageNumber=1&PageSize=10&SearchTerm=",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"category"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "10"
										},
										{
											"key": "SearchTerm",
											"value": ""
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Cat] Obtener Categoría por ID",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/admin/category/{{newCategoryId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"admin",
										"category",
										"{{newCategoryId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Cat] Actualizar Categoría",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PUT",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"name\": \"{{newCategoryName}} (Actualizada)\",\r\n  \"description\": \"helado\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/category/{{newCategoryId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"category",
										"{{newCategoryId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Cat] Eliminar Categoría",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "DELETE",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": ""
								},
								"url": {
									"raw": "{{baseUrl}}/admin/category/{{newCategoryId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"category",
										"{{newCategoryId}}"
									]
								}
							},
							"response": []
						}
					]
				},
				{
					"name": "Admin Brands",
					"item": [
						{
							"name": "[Admin Brand] Crear Marca",
							"event": [
								{
									"listen": "prerequest",
									"script": {
										"exec": [
											"pm.collectionVariables.set(\"newBrandName\", \"Test Brand \" + Date.now());"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								},
								{
									"listen": "test",
									"script": {
										"exec": [
											"pm.test(\"Status code is 201 (Created)\", function () {",
											"    pm.response.to.have.status(201);",
											"});",
											"var jsonData = pm.response.json();",
											"pm.collectionVariables.set(\"newBrandId\", jsonData.data.id);"
										],
										"type": "text/javascript",
										"packages": {},
										"requests": {}
									}
								}
							],
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "POST",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"name\": \"{{newBrandName}}\",\r\n  \"description\": \"Marca china de tecnología innovadora\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/brand",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"brand"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Brand] Listar Marcas",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/brand?PageNumber=1&PageSize=10",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"brand"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "10"
										},
										{
											"key": "SearchTerm",
											"value": null,
											"disabled": true
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Brand] Obtener Marca por ID",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/api/admin/brand/{{newBrandId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"api",
										"admin",
										"brand",
										"{{newBrandId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Brand] Actualizar Marca",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PUT",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"name\": \"{{newBrandName}} (Actualizada)\",\r\n  \"description\": \"Empresa multinacional china de electrónicos y tecnología\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/brand/{{newBrandId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"brand",
										"{{newBrandId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Brand] Eliminar Marca",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "DELETE",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/brand/{{newBrandId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"brand",
										"{{newBrandId}}"
									]
								}
							},
							"response": []
						}
					]
				},
				{
					"name": "Admin Users",
					"item": [
						{
							"name": "[Admin Users] Listar Usuarios",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/users?PageNumber=1&PageSize=5&Role=&Status=&Email=&CreatedFrom&OrderBy=&OrderDir=",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"users"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "5"
										},
										{
											"key": "Role",
											"value": ""
										},
										{
											"key": "Status",
											"value": ""
										},
										{
											"key": "Email",
											"value": ""
										},
										{
											"key": "CreatedFrom",
											"value": null
										},
										{
											"key": "CreatedTo",
											"value": "2024-12-31",
											"disabled": true
										},
										{
											"key": "OrderBy",
											"value": ""
										},
										{
											"key": "OrderDir",
											"value": ""
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Users] Ver Detalle Usuario",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/users/{{currentUserId}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"users",
										"{{currentUserId}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Users] Actualizar Rol Usuario",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PATCH",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"role\": \"Admin\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/users/{{currentUserId}}/role",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"users",
										"{{currentUserId}}",
										"role"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Users] Actualizar Estado Usuario (Bloquear)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PATCH",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n    \"status\": \"Blocked\",\r\n    \"reason\": \"Prueba de bloqueo\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/users/{{currentUserId}}/status",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"users",
										"{{currentUserId}}",
										"status"
									]
								}
							},
							"response": []
						}
					]
				},
				{
					"name": "Admin Orders (Rutas /api/admin/orders)",
					"item": [
						{
							"name": "[Admin Orders] Listar Órdenes (api/admin/orders)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/orders?PageNumber=1&PageSize=5",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"orders"
									],
									"query": [
										{
											"key": "PageNumber",
											"value": "1"
										},
										{
											"key": "PageSize",
											"value": "5"
										}
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Orders] Ver Detalle Orden (api/admin/orders)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "GET",
								"header": [],
								"url": {
									"raw": "{{baseUrl}}/admin/orders/{{orderCode}}",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"orders",
										"{{orderCode}}"
									]
								}
							},
							"response": []
						},
						{
							"name": "[Admin Orders] Actualizar Estado Orden (api/admin/orders)",
							"request": {
								"auth": {
									"type": "bearer",
									"bearer": [
										{
											"key": "token",
											"value": "{{adminToken}}",
											"type": "string"
										}
									]
								},
								"method": "PATCH",
								"header": [],
								"body": {
									"mode": "raw",
									"raw": "{\r\n  \"status\": \"Shipped\",\r\n  \"note\": \"Enviado por DHL\"\r\n}",
									"options": {
										"raw": {
											"language": "json"
										}
									}
								},
								"url": {
									"raw": "{{baseUrl}}/admin/orders/{{orderCode}}/status",
									"host": [
										"{{baseUrl}}"
									],
									"path": [
										"admin",
										"orders",
										"{{orderCode}}",
										"status"
									]
								}
							},
							"response": []
						}
					]
				}
			],
			"description": "Flujos 6, 7, 8 y 9: Administración de Productos, Categorías, Marcas, Órdenes y Usuarios."
		}
	],
	"event": [
		{
			"listen": "prerequest",
			"script": {
				"type": "text/javascript",
				"packages": {},
				"requests": {},
				"exec": [
					""
				]
			}
		},
		{
			"listen": "test",
			"script": {
				"type": "text/javascript",
				"packages": {},
				"requests": {},
				"exec": [
					""
				]
			}
		}
	],
	"variable": [
		{
			"key": "baseUrl",
			"value": "http://localhost:5000/api"
		},
		{
			"key": "authToken",
			"value": ""
		},
		{
			"key": "adminToken",
			"value": ""
		},
		{
			"key": "currentUserId",
			"value": ""
		},
		{
			"key": "productId",
			"value": "1"
		},
		{
			"key": "orderId",
			"value": "1"
		},
		{
			"key": "testUserEmail",
			"value": "customer-{{$timestamp}}@test.com"
		},
		{
			"key": "testUserPassword",
			"value": "TestUser123!"
		},
		{
			"key": "testAdminEmail",
			"value": "admin@tiendaucn.cl"
		},
		{
			"key": "testAdminPassword",
			"value": "Admin123!"
		},
		{
			"key": "testProductName",
			"value": "Postman Test Product {{$timestamp}}"
		},
		{
			"key": "testProductPrice",
			"value": "19990"
		},
		{
			"key": "verificationCode",
			"value": "123456"
		},
		{
			"key": "verifiedUserEmail",
			"value": "cliente@test.com"
		},
		{
			"key": "verifiedUserPassword",
			"value": "Cliente123!"
		},
		{
			"key": "newProductId",
			"value": ""
		},
		{
			"key": "newCategoryId",
			"value": "1"
		},
		{
			"key": "newBrandId",
			"value": "1"
		},
		{
			"key": "newImageId",
			"value": "1"
		},
		{
			"key": "targetUserId",
			"value": "2"
		},
		{
			"key": "tempCategoryId",
			"value": ""
		},
		{
			"key": "tempBrandId",
			"value": ""
		},
		{
			"key": "newAuthToken",
			"value": ""
		},
		{
			"key": "adminCategoryId",
			"value": "1"
		},
		{
			"key": "adminBrandId",
			"value": "1"
		},
		{
			"key": "adminProductId",
			"value": ""
		},
		{
			"key": "adminUserId",
			"value": ""
		},
		{
			"key": "testUserRut",
			"value": ""
		},
		{
			"key": "newBrandName",
			"value": ""
		},
		{
			"key": "newCategoryName",
			"value": ""
		},
		{
			"key": "orderCode",
			"value": ""
		}
	]
}oading TiendaUCN.postman_collection.json…]()


