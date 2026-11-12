using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

// Un test por criterio de aceptación de specs/HU-03-pedidos-por-fecha/spec.md.
public class FiltroPedidosTests
{
    private static readonly DateOnly Desde = new(2026, 3, 10);
    private static readonly DateOnly Hasta = new(2026, 3, 20);

    [Fact] // CA-01
    public void EntreFechas_PedidoEnFechaDesde_Aparece()
    {
        var pedidos = new[] { PedidoDel(1, Desde) };

        var resultado = FiltroPedidos.EntreFechas(pedidos, Desde, Hasta);

        Assert.Equal(new[] { 1 }, resultado.Select(p => p.Id));
    }

    [Fact] // CA-02
    public void EntreFechas_PedidoEnFechaHasta_Aparece()
    {
        var pedidos = new[] { PedidoDel(2, Hasta) };

        var resultado = FiltroPedidos.EntreFechas(pedidos, Desde, Hasta);

        Assert.Equal(new[] { 2 }, resultado.Select(p => p.Id));
    }

    [Fact] // CA-03
    public void EntreFechas_PedidoFueraDelRango_NoAparece()
    {
        var pedidos = new[]
        {
            PedidoDel(3, new DateOnly(2026, 3, 9)),  // un día antes de desde
            PedidoDel(4, new DateOnly(2026, 3, 15)), // dentro del rango
            PedidoDel(5, new DateOnly(2026, 3, 21))  // un día después de hasta
        };

        var resultado = FiltroPedidos.EntreFechas(pedidos, Desde, Hasta);

        Assert.Equal(new[] { 4 }, resultado.Select(p => p.Id));
    }

    [Fact] // CA-04
    public void EntreFechas_DesdeMayorQueHasta_LanzaArgumentException()
    {
        var pedidos = new[] { PedidoDel(6, new DateOnly(2026, 3, 15)) };

        var error = Assert.Throws<ArgumentException>(
            () => FiltroPedidos.EntreFechas(pedidos, desde: Hasta, hasta: Desde));

        Assert.Contains("2026-03-20", error.Message);
        Assert.Contains("2026-03-10", error.Message);
        Assert.Contains("no puede ser mayor", error.Message);
    }

    [Fact] // Borde pedido por REVIEW.md («Revisar siempre» 3): lista vacía.
    public void EntreFechas_ListaVacia_DevuelveListaVacia()
    {
        var resultado = FiltroPedidos.EntreFechas(Array.Empty<Pedido>(), Desde, Hasta);

        Assert.Empty(resultado);
    }

    private static Pedido PedidoDel(int id, DateOnly fecha) =>
        new(id, $"PED-STGO-{id:000000}", "C-001", "Cliente de prueba", fecha, "Pendiente", 10_000m, "STGO");
}
