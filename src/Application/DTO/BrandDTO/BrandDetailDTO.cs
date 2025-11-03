namespace TiendaProyecto.src.Application.DTO.BrandDTO
{
    /// <summary>
    /// DTO que representa el detalle completo de una marca.
    /// </summary>
    public class BrandDetailDTO
    {
        /// <summary>
        /// Identificador único de la marca.
        /// </summary>
        public required int Id { get; set; }

        /// <summary>
        /// Nombre de la marca.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Slug normalizado de la marca.
        /// </summary>
        public required string Slug { get; set; }

        /// <summary>
        /// Descripción de la marca.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Cantidad de productos asociados a esta marca.
        /// </summary>
        public required int ProductCount { get; set; }

        /// <summary>
        /// Indica si la marca está activa.
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