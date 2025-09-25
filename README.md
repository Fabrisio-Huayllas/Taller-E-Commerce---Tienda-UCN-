# Ecommerce Platform

The objective of this project is to implement a REST API using ASP.NET Core 9 and SQLite to create a ecommerce platform called Tienda UCN. It includes user authentication with JWT, profile, products and cart management.

The Repository Pattern is implemented to ensure a clean architecture, separation used for of concerns, and easier maintainability.

Cloudinary is external media storage, allowing efficient handling of images and other assets.

The system is designed for scalability, security, and high performance.

# Installation

For the execution of the project, the following must be installed:
-   [Visual Studio Code 1.89.1+](https://code.visualstudio.com/)
-   [ASP .Net Core 9](https://dotnet.microsoft.com/en-us/download)
-   [git 2.45.1+](https://git-scm.com/downloads)
-   [Postman](https://www.postman.com/downloads/)

Once the above is installed, clone the repository with the command:


# Quick Start
1. Clone this repository to your local machine using CMD:
```bash
    git clone https://github.com/Fabrisio-Huayllas/Taller-E-Commerce---Tienda-UCN-.git
```
2. Navigate to the project folder:
```bash
    cd TiendaProyecto
```
3. Open the proyect with Visual Studio Code:
```bash
    code .
```

4. Copy the content of the file appsettings.example.json and create an appsettings.json file with the next command:
```bash
    cp appsettings.example.json appsettings.json
```
  Then replace the variables with your credentials:

**Required configuration:**
- Replace `JWTSecret` with a strong secret key with at least 32 characters long.
- Replace `ResendAPIKey` with your resend API Key, you can get your API key in the following link: [Resend - Getting Started](https://resend.com/docs/send-with-dotnet)
- Replace Cloudinary credentials with your actual values
- Replace admin `Rut` with the following format XXXXXXXX-X
- Replace admin `BirthDate` with the following format YYYY-MM-DD
- Replace admin `PhoneNumber` with the following format +569 XXXXXXXX
- Replace admin `Password` and de `RandomUserPassword` with an alphanumeric password with at least one capital letter and at least one special character.
- Replace the `WelcomeSubject`, `From` and `VerificationSubject` with your own email variables, but, i recommend use the `<onboarding@resend.dev>` email domain to use the free plan of resend API.
- Replace the `TimeZone` with your local time zone, the `CronJobDeleteUnconfirmedUsers` with your own cronjob and the `DaysOfDeleteUnconfirmedUsers` with your own interval on days to delete the unconfirmed users.
- Replace the `DefaultImageUrl` with your own default image URL.
- Replace the `CookieExpirationDays` with your cookie expiration time of your preference.
- Keep the `HangfireDashboard` section if you want a default configuration of the dashboard.
- Keep the `AllowedUserNameCharacters`, the `ExpirationTimeInMinutes`, `TransformationWidth`, `TransformationCrop`, `TransformationQuality`, `TransformationFetchFormat`, `DefaultPageSize` and the `ImageMaxSizeInBytes` configuration of the appsettings.example.json file.

5. Restore the project dependencies in the terminal:
```bash
    dotnet restore
```
6. To execute the proyect use the next command in the VSC terminal:
```bash
    dotnet run
```