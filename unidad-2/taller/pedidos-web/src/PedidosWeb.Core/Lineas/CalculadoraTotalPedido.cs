namespace PedidosWeb.Core.Lineas;

/// <summary>
/// Calcula el total de un pedido desde sus líneas (HU-02, specs/HU-02-detalle-pedido/spec.md).
/// Montos en CLP netos, sin IVA.
/// </summary>
public sealed class CalculadoraTotalPedido
{
    private readonly ILineaPedidoRepositorio _lineas;

    public CalculadoraTotalPedido(ILineaPedidoRepositorio lineas)
    {
        _lineas = lineas;
    }

    /// <summary>Cantidad × precio unitario de una línea.</summary>
    public static decimal Subtotal(LineaPedido linea) => linea.Cantidad * linea.PrecioUnitario;

    /// <summary>Suma de los subtotales de las líneas del pedido. Cero si no tiene líneas.</summary>
    public decimal Calcular(int pedidoId) => _lineas.ObtenerPorPedido(pedidoId).Sum(Subtotal);
}
