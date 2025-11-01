using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Application.DTO.BrandDTO;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Application.Mappers
{
    /// <summary>
    /// Clase para mapear objetos de tipo DTO a Brand y viceversa.
    /// </summary>
    public class BrandMapper
    {
        public BrandMapper() { }

        /// <summary>
        /// Configura todos los mapeos de marcas.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureBrandMappings();
        }

        /// <summary>
        /// Configura el mapeo entre Brand y sus DTOs.
        /// </summary>
        public void ConfigureBrandMappings()
        {
            // Mapeo de Brand a BrandDetailDTO
            TypeAdapterConfig<Brand, BrandDetailDTO>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Slug, src => src.Slug)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.IsActive, src => src.IsActive)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.UpdatedAt, src => src.UpdatedAt)
                .Ignore(dest => dest.ProductCount); // Se asigna manualmente en el servicio

            // Mapeo de BrandCreateDTO a Brand
            TypeAdapterConfig<BrandCreateDTO, Brand>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.IsActive, src => true)
                .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Slug)
                .Ignore(dest => dest.Products);

            // Mapeo de BrandUpdateDTO a Brand (para actualizaciones)
            TypeAdapterConfig<BrandUpdateDTO, Brand>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Slug)
                .Ignore(dest => dest.IsActive)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.Products);
        }
    }
}