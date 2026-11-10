namespace PedidosWeb.Core.Detalle;

/// <summary>
/// Detalle de un pedido con su cliente, sus líneas y su total (HU-02, specs/HU-02-detalle-pedido/spec.md).
/// </summary>
/// <param name="Id">Id del pedido.</param>
/// <param name="Folio">Folio del pedido, por ejemplo "PED-STGO-000008".</param>
/// <param name="Fecha">Fecha del pedido.</param>
/// <param name="Estado">Estado del pedido.</param>
/// <param name="Sucursal">Código de la sucursal.</param>
/// <param name="ClienteId">Código del cliente, tal como viene en el pedido.</param>
/// <param name="ClienteNombre">Nombre del cliente, tal como viene en el pedido.</param>
/// <param name="Lineas">Líneas en orden de número.</param>
/// <param name="Total">Suma de los subtotales, en CLP neto (sin IVA).</param>
public sealed record DetallePedido(
    int Id,
    string Folio,
    DateOnly Fecha,
    string Estado,
    string Sucursal,
    string ClienteId,
    string ClienteNombre,
    IReadOnlyList<LineaDetalle> Lineas,
    decimal Total);

/// <summary>Una línea del detalle, con su subtotal (cantidad × precio unitario, CLP neto).</summary>
public sealed record LineaDetalle(
    int Numero,
    string Producto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);
