using System.Globalization;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Filtros sobre una lista de pedidos ya cargada (HU-03).
/// </summary>
public static class FiltroPedidos
{
    /// <summary>
    /// Devuelve los pedidos cuya <see cref="Pedido.Fecha"/> está entre <paramref name="desde"/> y
    /// <paramref name="hasta"/>, ambas fechas inclusive, en el mismo orden en que llegaron.
    /// </summary>
    /// <exception cref="ArgumentException">Si <paramref name="desde"/> es mayor que <paramref name="hasta"/>.</exception>
    public static IReadOnlyList<Pedido> EntreFechas(IEnumerable<Pedido> pedidos, DateOnly desde, DateOnly hasta)
    {
        ArgumentNullException.ThrowIfNull(pedidos);

        if (desde > hasta)
        {
            // InvariantCulture: el año sale igual en cualquier cultura (calendario gregoriano).
            throw new ArgumentException(
                string.Create(
                    CultureInfo.InvariantCulture,
                    $"La fecha 'desde' ({desde:yyyy-MM-dd}) no puede ser mayor que la fecha 'hasta' ({hasta:yyyy-MM-dd})."),
                nameof(desde));
        }

        return pedidos
            .Where(pedido => pedido.Fecha >= desde && pedido.Fecha <= hasta)
            .ToList();
    }
}
