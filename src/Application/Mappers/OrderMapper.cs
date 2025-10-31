using Mapster;
using TiendaProyecto.src.Application.DTO.CartDTO;
using TiendaProyecto.src.Application.DTO.OrderDTO;
using TiendaProyecto.src.Application.DTO.OrderItemDTO;
using TiendaProyecto.src.Domain.Models;
using TiendaProyecto.src.Application.DTO.OrderDTO.AdminDTO;

namespace TiendaProyecto.src.Application.Mappers
{
    /// <summary>
    /// Clase para mapeo de órdenes a DTO y viceversa.
    /// </summary>
    public class OrderMapper
    {
        private readonly IConfiguration _configuration;
        private readonly string _defaultImageURL;
        private readonly TimeZoneInfo _timeZone;

        public OrderMapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _defaultImageURL = _configuration["Products:DefaultImageUrl"] ?? throw new InvalidOperationException("La configuración de DefaultImageUrl es necesaria.");
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
        }

        public void ConfigureAllMappings()
        {
            ConfigureOrderItemsMappings();
            ConfigureOrderMappings();
            ConfigureAdminOrderMappings(); // Nuevo mapeo para Administrador
        }

        public void ConfigureOrderMappings()
        {
            // Mapeo para el cliente (OrderDetailDTO)
            // Mapeo a OrderDetailDTO (Para Cliente)
            TypeAdapterConfig<Order, OrderDetailDTO>.NewConfig()
                .Map(dest => dest.Items, src => src.OrderItems)
                .Map(dest => dest.PurchasedAt, src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, _timeZone))
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Total, src => src.Total.ToString("C", new System.Globalization.CultureInfo("es-CL"))) 
                .Map(dest => dest.SubTotal, src => src.SubTotal.ToString("C", new System.Globalization.CultureInfo("es-CL"))); 
        }

        public void ConfigureOrderItemsMappings()
        {
            // Mapeo para el cliente (OrderItemDTO)
            TypeAdapterConfig<OrderItem, OrderItemDTO>.NewConfig()
                .Map(dest => dest.ProductTitle, src => src.TitleAtMoment)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.ProductDescription, src => src.DescriptionAtMoment)
                .Map(dest => dest.MainImageURL, src => src.ImageAtMoment)
                .Map(dest => dest.PriceAtMoment, src => src.PriceAtMoment.ToString("C", new System.Globalization.CultureInfo("es-CL")));
            // Mapeo de CartItem a OrderItem 
            TypeAdapterConfig<CartItem, OrderItem>.NewConfig()
                .Map(dest => dest.TitleAtMoment, src => src.Product.Title)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.DescriptionAtMoment, src => src.Product.Description)
                .Map(dest => dest.ImageAtMoment, src => src.Product.Images != null && src.Product.Images.Any() ? src.Product.Images.First().ImageUrl : _defaultImageURL)
                .Map(dest => dest.PriceAtMoment, src => src.Product.Price)
                .Map(dest => dest.DiscountAtMoment, src => src.Product.Discount)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Order);
        }

        /// <summary>
        /// Configura los mapeos específicos para los DTOs del panel de administrador.
        /// </summary>
        public void ConfigureAdminOrderMappings()
        {
            // Mapeo para el listado de administrador (OrderForAdminDTO)
            TypeAdapterConfig<Order, OrderForAdminDTO>.NewConfig()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.CustomerEmail, src => src.User.Email)
                .Map(dest => dest.UserId, src => src.UserId)
                .Map(dest => dest.Total, src => src.Total.ToString("C", new System.Globalization.CultureInfo("es-CL"))) // APLICAR FORMATO CLP
                .Map(dest => dest.Status, src => src.Status)
                .Map(dest => dest.CreatedAt, src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, _timeZone))
                .Map(dest => dest.UpdatedAt, src => TimeZoneInfo.ConvertTimeFromUtc(src.UpdatedAt, _timeZone))
                .Map(dest => dest.StatusChangedAt, src => src.StatusChangedAt.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(src.StatusChangedAt.Value, _timeZone) : (DateTime?)null)
                .Map(dest => dest.ChangeReason, src => src.ChangeReason)
                // Se mapean los OrderItemForAdminDTO 
                .Map(dest => dest.Items, src => src.OrderItems.Adapt<List<OrderItemForAdminDTO>>());

            // Mapeo para el detalle de administrador (OrderDetailForAdminDTO)
            TypeAdapterConfig<Order, OrderDetailForAdminDTO>.NewConfig()
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.CreatedAt, src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, _timeZone))
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.CustomerFirstName, src => src.User.FirstName)
                .Map(dest => dest.CustomerLastName, src => src.User.LastName)
                .Map(dest => dest.CustomerEmail, src => src.User.Email)
                .Map(dest => dest.Total, src => src.Total.ToString("C", new System.Globalization.CultureInfo("es-CL"))) // APLICAR FORMATO CLP
                .Map(dest => dest.SubTotal, src => src.SubTotal.ToString("C", new System.Globalization.CultureInfo("es-CL"))) // APLICAR FORMATO CLP
                .Map(dest => dest.Items, src => src.OrderItems.Adapt<List<OrderItemForAdminDTO>>());

            // Mapeo para los ítems del detalle de administrador (OrderItemForAdminDTO)
            TypeAdapterConfig<OrderItem, OrderItemForAdminDTO>.NewConfig()
                .Map(dest => dest.ProductName, src => src.TitleAtMoment)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.UnitPrice, src => src.PriceAtMoment.ToString("C", new System.Globalization.CultureInfo("es-CL"))) // APLICAR FORMATO CLP
                .Map(dest => dest.SubTotal, src => (src.PriceAtMoment * src.Quantity).ToString("C", new System.Globalization.CultureInfo("es-CL"))) // CALCULAR Y APLICAR FORMATO CLP
                .Map(dest => dest.ImageUrl, src => src.ImageAtMoment);
        }
    }
}