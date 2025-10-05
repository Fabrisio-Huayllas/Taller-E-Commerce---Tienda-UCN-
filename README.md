# Ecommerce Platform

Este proyecto implementa una API REST utilizando ASP.NET Core 9 y SQLite para la creación de una plataforma de comercio electrónico llamada Tienda UCN. El sistema incluye autenticación de usuarios con JWT, gestión de productos, carrito de compras, pedidos y subida de imágenes a Cloudinary.

El proyecto sigue una arquitectura limpia utilizando el patrón Repository, lo que asegura una separación de responsabilidades y facilita el mantenimiento y escalabilidad del sistema.

# Installation

For the execution of the project, the following must be installed:
-   [Visual Studio Code 1.89.1+](https://code.visualstudio.com/)
-   [ASP .Net Core 9](https://dotnet.microsoft.com/en-us/download)
-   [git 2.45.1+](https://git-scm.com/downloads)
-   [Postman](https://www.postman.com/downloads/)

Once the above is installed, clone the repository with the command:


# Características Implementadas
1. Sistema de Autenticación Completo
Registro de Usuarios:
        Validación de datos de entrada (nombre, apellido, RUT chileno, email, contraseña, etc.).
        Contraseñas almacenadas de forma segura utilizando hashing con ASP.NET Core Identity.
        Asignación automática del rol "Cliente" al registrarse.
Verificación de Cuenta:
        Generación de un código de 6 dígitos enviado por correo electrónico.
        Activación de la cuenta tras la verificación del código.
Inicio de Sesión:
        Validación de credenciales y generación de tokens JWT con claims (ID, rol, email, expiración).
        Manejo seguro de errores de autenticación.
Recuperación de Contraseña:
        Generación de un código de recuperación enviado por correo.
        Validación del código y restablecimiento de la contraseña con reglas de seguridad.
Cambio de Contraseña:
        Validación de la contraseña actual y actualización segura de la nueva contraseña.
2.CRUD Básico de Productos (Admin)
Creación de Productos:
    Validación de datos con CreateProductDTO (nombre, precio, stock, categoría, marca, etc.).
    Persistencia de productos en la base de datos.
    Protección del endpoint con el rol "Administrador".
Consulta de Productos:
    Endpoint para obtener el detalle de un producto por su ID.
    Endpoint para listar productos con filtros y paginación.
Actualización y Eliminación de Productos:
    Endpoints protegidos para actualizar y eliminar productos.

3. Subida de Imágenes con Cloudinary
   Subida de imágenes asociadas a productos.
    Validación de tipo de archivo, tamaño y formato.
    Almacenamiento de la URL de la imagen en la base de datos.
4. Middleware
Manejo de Errores:
    Middleware para capturar y devolver errores personalizados con códigos HTTP y mensajes claros.
    Correlación:
    Middleware para agregar un identificador único a cada solicitud para facilitar el rastreo.


# Configuración del Proyecto
1.Clona este repositorio:

2.Navega al directorio del proyecto:

3.Abre el proyecto en Visual Studio Code:

4.Copia el archivo de configuración de ejemplo y crea el archivo appsettings.json:

5.Configura las siguientes variables en appsettings.json:


JWTSecret: Llave secreta para firmar los tokens JWT.
ResendAPIKey: Clave de la API de Resend para el envío de correos.
Cloudinary: Credenciales para la integración con Cloudinary.
Admin: Configura el RUT, fecha de nacimiento, número de teléfono y contraseña del administrador.
Otros: Configura las variables relacionadas con imágenes, tiempo de expiración de cookies, cron jobs, etc.
(O usar el appsettings.cs proporcionado por el ayudante).

6.Restaurar las dependencias del proyecto.

7. Aplicar las migraciones a la base de datos.
8. Ejecutar proyecto.

# Endpoints principales
Autenticación:
    POST /api/auth/register: Registro de usuarios.
    POST /api/auth/login: Inicio de sesión.
    POST /api/auth/recover-password: Solicitud de recuperación de contraseña.
    PATCH /api/auth/reset-password: Restablecimiento de contraseña.
    POST /api/auth/verify: Verificación de cuenta.
Productos (Admin)
    POST /api/admin/products: Crear un producto.
    GET /api/admin/products/{id}: Consultar un producto por ID.
    PUT /api/admin/products/{id}: Actualizar un producto.


    dotnet restore
```
6. To execute the proyect use the next command in the VSC terminal:
```bash
    dotnet run
```
