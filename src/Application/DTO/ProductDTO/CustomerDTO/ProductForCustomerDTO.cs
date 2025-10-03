using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Application.DTO.ProductDTO.CustomerDTO
{
    public class ProductForCustomerDTO
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string MainImageURL { get; set; }
        public required string Price { get; set; }
        public required int Discount { get; set; }
    }
}