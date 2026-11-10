using PedidosWeb.Core.Lineas;
using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Lineas;

// Criterios de specs/HU-02-detalle-pedido/spec.md.
public class CalculadoraTotalPedidoTests
{
    private readonly CalculadoraTotalPedido _calculadora = new(new LineaPedidoRepositorioEnMemoria());

    [Fact] // CA-01
    public void Calcular_Pedido8_Devuelve120100()
    {
        var total = _calculadora.Calcular(8);

        Assert.Equal(120100m, total);
    }

    [Fact] // CA-02
    public void Calcular_Los45Pedidos_CoincideConPedidoTotal()
    {
        var pedidos = new PedidoRepositorioEnMemoria().ObtenerTodos();

        var distintos = pedidos
            .Where(p => _calculadora.Calcular(p.Id) != p.Total)
            .Select(p => p.Id)
            .ToList();

        Assert.Equal(45, pedidos.Count);
        Assert.Empty(distintos);
    }
}
