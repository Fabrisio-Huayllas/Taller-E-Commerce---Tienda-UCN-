using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace TiendaProyecto.src.Domain.Enums
{
    /// <summary>
    /// Define los distintos estados posibles de un pedido dentro del sistema.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Pedido creado pero aún no pagado.
        /// </summary>
        Created = 0,

        /// <summary>
        /// Pedido pagado y confirmado.
        /// </summary>
        Paid = 1,

        /// <summary>
        /// Pedido despachado al cliente.
        /// </summary>
        Shipped = 2,

        /// <summary>
        /// Pedido entregado exitosamente.
        /// </summary>
        Delivered = 3,

        /// <summary>
        /// Pedido cancelado por el cliente o el administrador.
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Pedido devuelto o rechazado tras la entrega.
        /// </summary>
        Returned = 5,

        /// <summary>
        /// Pedido reembolsado.
        /// </summary>
        Refunded = 6 // Estado agregado según el requisito del dominio
    }
}