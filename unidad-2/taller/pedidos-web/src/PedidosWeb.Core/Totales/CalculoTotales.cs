namespace PedidosWeb.Core.Totales;

/// <summary>Una línea del pedido para el cálculo (equivale a <c>TLineaCalculo</c> del legado).</summary>
/// <param name="Cantidad">Unidades pedidas.</param>
/// <param name="PrecioUnitario">Precio de una unidad en CLP.</param>
/// <param name="Categoria">Categoría del producto, por ejemplo "OFE".</param>
public sealed record LineaCalculo(int Cantidad, decimal PrecioUnitario, string Categoria);

/// <summary>Resultado del cálculo (equivale a <c>TResultadoTotales</c> del legado). Montos en CLP.</summary>
public sealed record ResultadoTotales(
    decimal Subtotal,
    decimal BaseDescuento,
    decimal PorcDescuento,
    decimal Descuento,
    decimal Neto,
    decimal Flete,
    decimal Iva,
    decimal Total);

/// <summary>
/// Cálculo de totales del pedido, migrado desde <c>legado/src/UCalculoTotales.pas</c>.
/// Contrato: <c>migracion/golden-master.csv</c>.
/// </summary>
public static class CalculoTotales
{
    /// <summary>Equivale a <c>CalcularTotales</c> del legado.</summary>
    public static ResultadoTotales Calcular(
        IReadOnlyList<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva)
    {
        throw new NotImplementedException(
            "Se migra en la etapa 2 del taller U5, a partir de legado/src/UCalculoTotales.pas.");
    }
}
