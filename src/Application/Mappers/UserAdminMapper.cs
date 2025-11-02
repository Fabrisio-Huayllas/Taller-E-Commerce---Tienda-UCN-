using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.UserDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Application.Mappers
{
    /// <summary>
    /// Clase para mapear objetos de tipo DTO a User y viceversa para administración.
    /// </summary>
    public class UserAdminMapper
    {
        public UserAdminMapper() { }

        /// <summary>
        /// Configura todos los mapeos de usuarios para administración.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureUserAdminMappings();
        }

        /// <summary>
        /// Configura el mapeo entre User y sus DTOs para administración.
        /// </summary>
        public void ConfigureUserAdminMappings()
        {
            // Mapeo de User a UserForAdminDTO
            TypeAdapterConfig<User, UserForAdminDTO>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.CreatedAt, src => src.RegisteredAt)
                .Map(dest => dest.LastLogin, src => src.LastLoginTime)
                .Ignore(dest => dest.Role)
                .Ignore(dest => dest.Status);

            // Mapeo de User a UserDetailForAdminDTO
            TypeAdapterConfig<User, UserDetailForAdminDTO>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.FirstName)
                .Map(dest => dest.LastName, src => src.LastName)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.Rut, src => src.Rut)
                .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
                .Map(dest => dest.EmailConfirmed, src => src.EmailConfirmed)
                .Map(dest => dest.CreatedAt, src => src.RegisteredAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Map(dest => dest.LastLogin, src => src.LastLoginTime)
                .Ignore(dest => dest.Role)
                .Ignore(dest => dest.Status);
        }
    }
}