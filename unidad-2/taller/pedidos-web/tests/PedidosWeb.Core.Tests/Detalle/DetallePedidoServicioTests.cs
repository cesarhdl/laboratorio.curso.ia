using PedidosWeb.Core.Detalle;
using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Detalle;

// Criterios de specs/HU-02-detalle-pedido/spec.md.
public class DetallePedidoServicioTests
{
    private readonly DetallePedidoServicio _servicio =
        new(new PedidoRepositorioEnMemoria(), new LineaPedidoRepositorioEnMemoria());

    [Fact] // CA-03
    public void Obtener_Pedido8_DevuelvePedidoClienteLineasYTotal()
    {
        var detalle = _servicio.Obtener(8);

        Assert.NotNull(detalle);
        Assert.Equal(8, detalle.Id);
        Assert.Equal("PED-STGO-000008", detalle.Folio);
        Assert.Equal(new DateOnly(2026, 10, 20), detalle.Fecha);
        Assert.Equal("Entregado", detalle.Estado);
        Assert.Equal("STGO", detalle.Sucursal);
        Assert.Equal("C-001", detalle.ClienteId);
        Assert.Equal("Ferretería El Clavo", detalle.ClienteNombre);
        Assert.Equal(new[] { 1, 2, 3 }, detalle.Lineas.Select(l => l.Numero));
        Assert.Equal(new[] { 64500m, 35400m, 20200m }, detalle.Lineas.Select(l => l.Subtotal));
        Assert.Equal(120100m, detalle.Total);
    }

    [Fact] // CA-04 (Core: no hay detalle; el 404 lo prueba el incremento 3)
    public void Obtener_PedidoInexistente_DevuelveNull()
    {
        var detalle = _servicio.Obtener(999);

        Assert.Null(detalle);
    }
}
