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
    private const string CategoriaSinDescuento = "OFE";
    private const decimal UmbralVolumenAlto = 1_000_000m;
    private const decimal UmbralVolumenMedio = 500_000m;
    private const decimal DescuentoVolumenAlto = 0.08m;
    private const decimal DescuentoVolumenMedio = 0.05m;
    private const decimal DescuentoVip = 0.03m;
    private const decimal TopeDescuento = 0.10m;
    private const decimal RecargoFleteExtremo = 1.15m;

    /// <summary>Equivale a <c>CalcularTotales</c> del legado (UCalculoTotales.pas:108-122).</summary>
    public static ResultadoTotales Calcular(
        IReadOnlyList<LineaCalculo> lineas,
        bool esVip,
        int region,
        decimal fleteBase,
        decimal tasaIva)
    {
        var subtotal = lineas.Sum(Importe);
        var baseDescuento = lineas
            .Where(linea => linea.Categoria.Trim().ToUpperInvariant() != CategoriaSinDescuento)
            .Sum(Importe);
        var porcDescuento = PorcentajeDescuento(baseDescuento, esVip);
        var descuento = RedondearAlPeso(baseDescuento * porcDescuento);
        var neto = subtotal - descuento;
        var flete = lineas.Count == 0 ? 0m : CalcularFlete(fleteBase, region);
        var iva = RedondearAlPeso((neto + flete) * tasaIva);
        var total = neto + flete + iva;

        return new ResultadoTotales(subtotal, baseDescuento, porcDescuento, descuento, neto, flete, iva, total);
    }

    private static decimal Importe(LineaCalculo linea) => linea.Cantidad * linea.PrecioUnitario;

    // UCalculoTotales.pas:75-92. Ojo: «mayor que» (>), igual que el legado; la grabación en
    // UFrmPedidos.pas:296-298 usa >= (registrado como hallazgo, no se corrige aquí).
    private static decimal PorcentajeDescuento(decimal baseDescuento, bool esVip)
    {
        var porcentaje = baseDescuento > UmbralVolumenAlto ? DescuentoVolumenAlto
            : baseDescuento > UmbralVolumenMedio ? DescuentoVolumenMedio
            : 0m;

        if (esVip)
        {
            porcentaje += DescuentoVip;
        }

        return Math.Min(porcentaje, TopeDescuento);
    }

    // UCalculoTotales.pas:95-101: Aysén (11) y Magallanes (12) pagan 15 % más.
    private static decimal CalcularFlete(decimal fleteBase, int region) =>
        region is 11 or 12
            ? RedondearAlPeso(fleteBase * RecargoFleteExtremo)
            : RedondearAlPeso(fleteBase);

    // Redondeo al peso: ,50 hacia arriba. Corrige el defecto R05 del legado
    // (UCalculoTotales.pas:48-51, Calc3 = Trunc(x + 0,49), donde un ,50 exacto bajaba).
    // Ver docs/migracion/defectos-corregidos.md (D1).
    private static decimal RedondearAlPeso(decimal valor) =>
        Math.Round(valor, 0, MidpointRounding.AwayFromZero);
}
