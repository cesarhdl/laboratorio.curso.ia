using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Detalle;

/// <summary>
/// Arma el detalle de un pedido (HU-02, specs/HU-02-detalle-pedido/spec.md).
/// </summary>
public sealed class DetallePedidoServicio
{
    private readonly IPedidoRepositorio _pedidos;
    private readonly ILineaPedidoRepositorio _lineas;

    public DetallePedidoServicio(IPedidoRepositorio pedidos, ILineaPedidoRepositorio lineas)
    {
        _pedidos = pedidos;
        _lineas = lineas;
    }

    /// <summary>El detalle del pedido, o null si el pedido no existe.</summary>
    public DetallePedido? Obtener(int pedidoId)
    {
        var pedido = _pedidos.ObtenerPorId(pedidoId);
        if (pedido is null)
        {
            return null;
        }

        var lineas = _lineas.ObtenerPorPedido(pedidoId)
            .Select(l => new LineaDetalle(
                l.Numero,
                l.Producto,
                l.Cantidad,
                l.PrecioUnitario,
                CalculadoraTotalPedido.Subtotal(l)))
            .ToList();

        return new DetallePedido(
            pedido.Id,
            pedido.Folio,
            pedido.Fecha,
            pedido.Estado,
            pedido.Sucursal,
            pedido.ClienteId,
            pedido.ClienteNombre,
            lineas,
            lineas.Sum(l => l.Subtotal));
    }
}
