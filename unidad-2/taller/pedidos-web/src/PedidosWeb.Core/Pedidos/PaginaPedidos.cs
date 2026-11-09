namespace PedidosWeb.Core.Pedidos;

/// <summary>Una página del listado de pedidos (RN-06).</summary>
/// <param name="Total">Pedidos que cumplen los filtros, antes de paginar.</param>
/// <param name="Pagina">Número de página devuelta, desde 1.</param>
/// <param name="Tamano">Tamaño de página usado.</param>
/// <param name="Pedidos">Pedidos de esta página, ordenados por fecha y id descendentes.</param>
public sealed record PaginaPedidos(int Total, int Pagina, int Tamano, IReadOnlyList<Pedido> Pedidos);
