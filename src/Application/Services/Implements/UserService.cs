using Mapster;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.AuthDTO;
using TiendaProyecto.src.Application.Services.Interfaces;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using TiendaProyecto.src.Exceptions;


namespace TiendaProyecto.src.Application.Services.Implements
{
    /// <summary>
    /// Servicio de usuarios mínimo para login con JWT.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IVerificationCodeRepository _verificationCodeRepository;
        private readonly int _verificationCodeExpirationTimeInMinutes;

         private readonly UserManager<User> _userManager;
        public UserService(ITokenService tokenService,
        IUserRepository userRepository,
        IEmailService emailService,
        IVerificationCodeRepository verificationCodeRepository,
        IConfiguration configuration,
        UserManager<User> userManager)
        {

            _tokenService = tokenService;
            _userRepository = userRepository;
            _emailService = emailService;
            _verificationCodeRepository = verificationCodeRepository;
            _configuration = configuration;
            _userManager = userManager;
            _verificationCodeExpirationTimeInMinutes = _configuration.GetValue<int>("VerificationCode:ExpirationTimeInMinutes");
        }

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO con email, contraseña y rememberMe.</param>
        /// <param name="httpContext">Contexto HTTP actual.</param>
        /// <returns>Token JWT y userId.</returns>
        public async Task<(string token, int userId)> LoginAsync(LoginDTO loginDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";

            var user = await _userRepository.GetByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                Log.Warning($"Login fallido. Usuario no encontrado: {loginDTO.Email} desde IP: {ipAddress}");
                throw new UnauthorizedAppException("Credenciales inválidas.");
            }

            var passwordValid = await _userRepository.CheckPasswordAsync(user, loginDTO.Password);
            if (!passwordValid)
            {
                Log.Warning($"Login fallido. Contraseña incorrecta para {loginDTO.Email} desde IP: {ipAddress}");
                throw new UnauthorizedAppException("Credenciales inválidas.");
            }

            string roleName = await _userRepository.GetUserRoleAsync(user)
                ?? throw new InvalidOperationException("El usuario no tiene rol asignado.");

            var token = _tokenService.GenerateToken(user, roleName, loginDTO.RememberMe);

            Log.Information($"Login exitoso para {loginDTO.Email} desde IP: {ipAddress}");
            return (token, user.Id);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            var passwordValid = await _userRepository.CheckPasswordAsync(user, dto.CurrentPassword);
            if (!passwordValid)
                throw new UnauthorizedAppException("La contraseña actual es incorrecta.");

            var result = await _userRepository.UpdatePasswordAsync(user, dto.NewPassword);
            if (!result)
                throw new Exception("No se pudo actualizar la contraseña.");
        }

        /// <summary>
        /// (Opcional) Elimina usuarios no confirmados.
        /// </summary>
        public async Task<int> DeleteUnconfirmedAsync()
        {
            return await _userRepository.DeleteUnconfirmedAsync();
        }
        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el mensaje de éxito del registro.</returns>
        public async Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext)
        {
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "IP desconocida";
            Log.Information($"Intento de registro de nuevo usuario: {registerDTO.Email} desde la IP: {ipAddress}");

            bool isRegistered = await _userRepository.ExistsByEmailAsync(registerDTO.Email);
            if (isRegistered)
            {
                Log.Warning($"El usuario con el correo {registerDTO.Email} ya está registrado.");
                throw new InvalidOperationException("El usuario ya está registrado.");
            }
            isRegistered = await _userRepository.ExistsByRutAsync(registerDTO.Rut);
            if (isRegistered)
            {
                Log.Warning($"El usuario con el RUT {registerDTO.Rut} ya está registrado.");
                throw new InvalidOperationException("El RUT ya está registrado.");
            }
            var user = registerDTO.Adapt<User>();
            var result = await _userRepository.CreateAsync(user, registerDTO.Password);
            if (!result)
            {
                Log.Warning($"Error al registrar el usuario: {registerDTO.Email}");
                throw new Exception("Error al registrar el usuario.");
            }
            Log.Information($"Registro exitoso para el usuario: {registerDTO.Email} desde la IP: {ipAddress}");
            string code = new Random().Next(100000, 999999).ToString();
            var verificationCode = new VerificationCode
            {
                UserId = user.Id,
                Code = code,
                CodeType = CodeType.EmailVerification,
                ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes)
            };
            var createdVerificationCode = await _verificationCodeRepository.CreateAsync(verificationCode);
            Log.Information($"Código de verificación generado para el usuario: {registerDTO.Email} - Código: {createdVerificationCode.Code}");

            await _emailService.SendVerificationCodeEmailAsync(registerDTO.Email, createdVerificationCode.Code);
            Log.Information($"Se ha enviado un código de verificación al correo electrónico: {registerDTO.Email}");
            return "Se ha enviado un código de verificación a su correo electrónico.";
        }

        /// <summary>
        /// Reenvía el código de verificación al correo electrónico del usuario.
        /// </summary>
        /// <param name="resendEmailVerificationCodeDTO">DTO que contiene el correo electrónico del usuario.</param>
        /// <returns>Un string que representa el mensaje de éxito del reenvío.</returns>
        public async Task<string> ResendEmailVerificationCodeAsync(ResendEmailVerificationCodeDTO resendEmailVerificationCodeDTO)
        {
            var currentTime = DateTime.UtcNow;
            User? user = await _userRepository.GetByEmailAsync(resendEmailVerificationCodeDTO.Email);
            if (user == null)
            {
                Log.Warning($"El usuario con el correo {resendEmailVerificationCodeDTO.Email} no existe.");
                throw new KeyNotFoundException("El usuario no existe.");
            }
            if (user.EmailConfirmed)
            {
                Log.Warning($"El usuario con el correo {resendEmailVerificationCodeDTO.Email} ya ha verificado su correo electrónico.");
                throw new InvalidOperationException("El correo electrónico ya ha sido verificado.");
            }
            VerificationCode? verificationCode = await _verificationCodeRepository.GetLatestByUserIdAsync(user.Id, CodeType.EmailVerification);
            var expirationTime = verificationCode!.CreatedAt.AddMinutes(_verificationCodeExpirationTimeInMinutes);
            if (expirationTime > currentTime)
            {
                int remainingSeconds = (int)(expirationTime - currentTime).TotalSeconds;
                Log.Warning($"El usuario {resendEmailVerificationCodeDTO.Email} ha solicitado un reenvío del código de verificación antes de los {_verificationCodeExpirationTimeInMinutes} minutos.");
                throw new TimeoutException($"Debe esperar {remainingSeconds} segundos para solicitar un nuevo código de verificación.");
            }
            string newCode = new Random().Next(100000, 999999).ToString();
            verificationCode.Code = newCode;
            verificationCode.ExpiryDate = DateTime.UtcNow.AddMinutes(_verificationCodeExpirationTimeInMinutes);
            await _verificationCodeRepository.UpdateAsync(verificationCode);
            Log.Information($"Nuevo código de verificación generado para el usuario: {resendEmailVerificationCodeDTO.Email} - Código: {newCode}");
            await _emailService.SendVerificationCodeEmailAsync(user.Email!, newCode);
            Log.Information($"Se ha reenviado un nuevo código de verificación al correo electrónico: {resendEmailVerificationCodeDTO.Email}");
            return "Se ha reenviado un nuevo código de verificación a su correo electrónico.";
        }

        /// <summary>
        /// Verifica el correo electrónico del usuario.
        /// </summary>
        /// <param name="verifyEmailDTO">DTO que contiene el correo electrónico y el código de verificación.</param>
        /// <returns>Un string que representa el mensaje de éxito de la verificación.</returns>
        public async Task<string> VerifyEmailAsync(VerifyEmailDTO verifyEmailDTO)
        {
            User? user = await _userRepository.GetByEmailAsync(verifyEmailDTO.Email);
            if (user == null)
            {
                Log.Warning($"El usuario con el correo {verifyEmailDTO.Email} no existe.");
                throw new KeyNotFoundException("El usuario no existe.");
            }
            if (user.EmailConfirmed)
            {
                Log.Warning($"El usuario con el correo {verifyEmailDTO.Email} ya ha verificado su correo electrónico.");
                throw new InvalidOperationException("El correo electrónico ya ha sido verificado.");
            }
            CodeType codeType = CodeType.EmailVerification;

            VerificationCode? verificationCode = await _verificationCodeRepository.GetLatestByUserIdAsync(user.Id, codeType);
            if (verificationCode == null)
            {
                Log.Warning($"No se encontró un código de verificación para el usuario: {verifyEmailDTO.Email}");
                throw new KeyNotFoundException("El código de verificación no existe.");
            }
            if (verificationCode.Code != verifyEmailDTO.VerificationCode || DateTime.UtcNow >= verificationCode.ExpiryDate)
            {
                int attempsCountUpdated = await _verificationCodeRepository.IncreaseAttemptsAsync(user.Id, codeType);
                Log.Warning($"Código de verificación incorrecto o expirado para el usuario: {verifyEmailDTO.Email}. Intentos actuales: {attempsCountUpdated}");
                if (attempsCountUpdated >= 5)
                {
                    Log.Warning($"Se ha alcanzado el límite de intentos para el usuario: {verifyEmailDTO.Email}");
                    bool codeDeleteResult = await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, codeType);
                    if (codeDeleteResult)
                    {
                        Log.Warning($"Se ha eliminado el código de verificación para el usuario: {verifyEmailDTO.Email}");
                        bool userDeleteResult = await _userRepository.DeleteAsync(user.Id);
                        if (userDeleteResult)
                        {
                            Log.Warning($"Se ha eliminado el usuario: {verifyEmailDTO.Email}");
                            throw new ArgumentException("Se ha alcanzado el límite de intentos. El usuario ha sido eliminado.");
                        }
                    }
                }
                if (DateTime.UtcNow >= verificationCode.ExpiryDate)
                {
                    Log.Warning($"El código de verificación ha expirado para el usuario: {verifyEmailDTO.Email}");
                    throw new ArgumentException("El código de verificación ha expirado.");
                }
                else
                {
                    Log.Warning($"El código de verificación es incorrecto para el usuario: {verifyEmailDTO.Email}");
                    throw new ArgumentException($"El código de verificación es incorrecto, quedan {5 - attempsCountUpdated} intentos.");
                }
            }
            bool emailConfirmed = await _userRepository.ConfirmEmailAsync(user.Email!);
            if (emailConfirmed)
            {
                bool codeDeleteResult = await _verificationCodeRepository.DeleteByUserIdAsync(user.Id, codeType);
                if (codeDeleteResult)
                {
                    Log.Warning($"Se ha eliminado el código de verificación para el usuario: {verifyEmailDTO.Email}");
                    await _emailService.SendWelcomeEmailAsync(user.Email!);
                    Log.Information($"El correo electrónico del usuario {verifyEmailDTO.Email} ha sido confirmado exitosamente.");
                    return "!Ya puedes iniciar sesión y disfrutar de todos los beneficios de Tienda UCN!";
                }
                throw new Exception("Error al confirmar el correo electrónico.");
            }
            throw new Exception("Error al verificar el correo electrónico.");
        }

        public async Task<UserProfileDTO> GetProfileAsync(int userId)
{
    var user = await _userManager.FindByIdAsync(userId.ToString())
               ?? throw new NotFoundException("Usuario no encontrado.");

    return new UserProfileDTO
    {
        Id = user.Id,
        Email = user.Email!,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PhoneNumber = user.PhoneNumber
    };
}

public async Task UpdateProfileAsync(int userId, UpdateProfileDTO updateProfileDTO)
{
    var user = await _userManager.FindByIdAsync(userId.ToString())
               ?? throw new NotFoundException("Usuario no encontrado.");

    user.FirstName = updateProfileDTO.FirstName ?? user.FirstName;
    user.LastName = updateProfileDTO.LastName ?? user.LastName;
    user.PhoneNumber = updateProfileDTO.PhoneNumber ?? user.PhoneNumber;

    var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded)
        throw new AppException("Error al actualizar el perfil: " +
            string.Join(", ", result.Errors.Select(e => e.Description)));
}

    }
}