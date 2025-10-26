using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Application.DTO.CategoryDTO
{
    /// <summary>
    /// DTO que representa el detalle completo de una categoría.
    /// </summary>
    public class CategoryDetailDTO
    {
        /// <summary>
        /// Identificador único de la categoría.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Nombre de la categoría.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Slug normalizado de la categoría.
        /// </summary>
        public required string Slug { get; set; }

        /// <summary>
        /// Descripción de la categoría.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Cantidad de productos asociados a esta categoría.
        /// </summary>
        public required int ProductCount { get; set; }

        /// <summary>
        /// Indica si la categoría está activa.
        /// </summary>
        public required bool IsActive { get; set; }

        /// <summary>
        /// Fecha de creación.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización.
        /// </summary>
        public required DateTime UpdatedAt { get; set; }
    }
}