using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Domain.Models
{
    public class Brand
    {
        /// <summary>
        /// Identificador único de la marca.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre de la marca.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Slug normalizado para URLs amigables.
        /// </summary>
        public required string Slug { get; set; }

        /// <summary>
        /// Descripción opcional de la marca.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indica si la marca está activa.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de creación de la marca.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de última actualización.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Productos asociados a esta marca.
        /// </summary>
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}