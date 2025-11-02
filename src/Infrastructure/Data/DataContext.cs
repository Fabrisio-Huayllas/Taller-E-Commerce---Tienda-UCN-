using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaProyecto.src.Domain.Models;

namespace TiendaProyecto.src.Infrastructure.Data
{
    /// <summary>
    /// Contexto de datos de la aplicación.
    /// </summary>
    /// <remarks>
    /// Hereda de IdentityDbContext para manejar la identidad de usuarios y define los DbSet para las entidades principales.
    /// </remarks>
    public class DataContext : IdentityDbContext<User, Role, int>
    {
        /// <summary>
        /// Constructor que recibe opciones de configuración para el contexto.
        /// </summary>
        /// <param name="options">Opciones de configuración del DbContext.</param>
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        /// <summary>
        /// Constructor por defecto.
        /// </summary>
        public DataContext() : base() { }
        /// <summary>
        /// Productos disponibles en la tienda.
        /// </summary>
        public DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// Imágenes de los productos.
        /// </summary>
        public DbSet<Image> Images { get; set; } = null!;
        /// <summary>
        /// Categorías de productos.
        /// </summary>
        public DbSet<Category> Categories { get; set; } = null!;
        /// <summary>
        /// Marcas de productos.
        /// </summary>
        public DbSet<Brand> Brands { get; set; } = null!;
        /// <summary>
        /// Órdenes realizadas por los usuarios.
        /// </summary>
        public DbSet<Order> Orders { get; set; } = null!;
        /// <summary>
        /// Ítems contenidos en las órdenes.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        /// <summary>
        /// Carritos de compras de los usuarios.
        /// </summary>
        public DbSet<Cart> Carts { get; set; } = null!;
        /// <summary>
        /// Ítems contenidos en los carritos de compras.
        /// </summary>
        public DbSet<CartItem> CartItems { get; set; } = null!;
        /// <summary>
        /// Códigos de verificación asociados a los usuarios.
        /// </summary>
        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;
        /// <summary>
        /// Registros de auditoría de cambios de estado de usuario.
        /// </summary>
        public DbSet<UserStatusAudit> UserStatusAudits { get; set; } = null!;
    }
}