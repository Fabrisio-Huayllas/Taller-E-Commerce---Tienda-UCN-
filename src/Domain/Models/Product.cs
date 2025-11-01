using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaProyecto.src.Domain.Models
{
    public enum Status
    {
        New,
        Used
    }

    public class Product
    {
        /// <summary>
        /// Identificador único del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título del producto.
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// Descripción del producto.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Precio del producto.
        /// </summary>
        public required int Price { get; set; }

        /// <summary>
        /// Descuento del producto.
        /// </summary>
        public int Discount { get; set; }

        /// <summary>
        /// Stock del producto.
        /// </summary>
        public required int Stock { get; set; }

        /// <summary>
        /// Estado del producto (Nuevo o usado).
        /// </summary>
        public required Status Status { get; set; }

        /// <summary>
        /// Indica si el producto está disponible para la venta.
        /// </summary>
        public bool IsAvailable { get; set; } = true;

        /// <summary>
        /// Identificador de la categoría del producto.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Categoría del producto.
        /// </summary>
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Identificador de la marca del producto.
        /// </summary>
        public int BrandId { get; set; }

        /// <summary>
        /// Marca del producto.
        /// </summary>
        public Brand Brand { get; set; } = null!;

        /// <summary>
        /// Lista de imágenes asociadas al producto.
        /// </summary>
        public ICollection<Image> Images { get; set; } = new List<Image>();

        /// <summary>
        /// Indica si el producto ha sido eliminado lógicamente.
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Fecha y hora en que el producto fue eliminado lógicamente.
        /// </summary>
        public DateTime? DeletedAt { get; set; }


        /// <summary>
        /// Fecha de creación del producto.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Fecha de actualización del producto.
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
            /// Precio final del producto después de aplicar el descuento.
            /// </summary>
            public int FinalPrice => (int)(Price * (1 - (decimal)Discount / 100));
        
    }
}