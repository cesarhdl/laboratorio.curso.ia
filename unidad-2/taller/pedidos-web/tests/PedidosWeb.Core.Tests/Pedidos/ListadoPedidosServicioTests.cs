using PedidosWeb.Core.Pedidos;

namespace PedidosWeb.Core.Tests.Pedidos;

// Un test por criterio de aceptación de specs/HU-01-listar-pedidos/spec.md.
public class ListadoPedidosServicioTests
{
    private readonly ListadoPedidosServicio _servicio = new(new PedidoRepositorioEnMemoria());

    [Fact] // CA-01
    public void Listar_SinFiltros_DevuelvePrimeraPaginaOrdenada()
    {
        var pagina = ListarConExito(new ConsultaPedidos());

        Assert.Equal(45, pagina.Total);
        Assert.Equal(1, pagina.Pagina);
        Assert.Equal(20, pagina.Tamano);
        Assert.Equal(
            new[] { 21, 42, 2, 23, 44, 4, 25, 6, 27, 8, 29, 10, 31, 12, 33, 14, 35, 16, 37, 18 },
            Ids(pagina));
    }

    [Fact] // CA-02
    public void Listar_ClienteParcial_DevuelvePedidosDelCliente()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "esquina"));

        Assert.Equal(5, pagina.Total);
        Assert.Equal(new[] { 23, 31, 39, 7, 15 }, Ids(pagina));
    }

    [Fact] // CA-03
    public void Listar_ClienteEnMayusculas_IgnoraMayusculas()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "ESQUINA"));

        Assert.Equal(5, pagina.Total);
        Assert.Equal(new[] { 23, 31, 39, 7, 15 }, Ids(pagina));
    }

    [Fact] // CA-04
    public void Listar_ClienteSinTildes_IgnoraTildes()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "panaderia"));

        Assert.Equal(new[] { 44, 4, 12, 20, 28, 36 }, Ids(pagina));
    }

    [Fact] // CA-05
    public void Listar_ClienteConNEnVezDeEnie_LaTrataComoN()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "dona rosa"));

        Assert.Equal(new[] { 21, 29, 37, 45, 5, 13 }, Ids(pagina));
    }

    [Fact] // CA-06
    public void Listar_EstadoEnMinusculas_DevuelveSoloEseEstado()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Estado: "entregado", Tamano: "100"));

        Assert.Equal(15, pagina.Total);
        Assert.Equal(15, pagina.Pedidos.Count);
        Assert.All(pagina.Pedidos, p => Assert.Equal("Entregado", p.Estado));
    }

    [Fact] // CA-07
    public void Listar_EstadoInexistente_DevuelveEstadoInvalido()
    {
        var error = ListarConError(new ConsultaPedidos(Estado: "Cancelado"));

        Assert.Equal("ESTADO_INVALIDO", error.Codigo);
    }

    [Fact] // CA-08
    public void Listar_ClienteYFechas_CombinaConY()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "esquina", Desde: "2026-10-01", Hasta: "2026-10-31"));

        Assert.Equal(new[] { 23, 31, 39 }, Ids(pagina));
    }

    [Fact] // CA-09
    public void Listar_ClienteYEstado_CombinaConY()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "esquina", Estado: "Entregado"));

        Assert.Equal(new[] { 31, 15 }, Ids(pagina));
    }

    [Theory] // CA-10
    [InlineData("2026-10-31", 21)]
    [InlineData("2026-09-02", 40)]
    public void Listar_DesdeIgualHasta_IncluyeLosLimites(string fecha, int idEsperado)
    {
        var pagina = ListarConExito(new ConsultaPedidos(Desde: fecha, Hasta: fecha));

        Assert.Equal(new[] { idEsperado }, Ids(pagina));
    }

    [Fact] // CA-11
    public void Listar_FechasInvertidas_DevuelveError()
    {
        var error = ListarConError(new ConsultaPedidos(Desde: "2026-10-31", Hasta: "2026-10-01"));

        Assert.Equal("FECHAS_INVERTIDAS", error.Codigo);
    }

    [Fact] // CA-12
    public void Listar_FechaInexistente_DevuelveFechaInvalida()
    {
        var error = ListarConError(new ConsultaPedidos(Desde: "2026-13-01"));

        Assert.Equal("FECHA_INVALIDA", error.Codigo);
    }

    [Theory] // CA-13
    [InlineData("3", new[] { 36, 17, 38, 19, 40 })]
    [InlineData("4", new int[0])]
    public void Listar_PaginaYTamano_DevuelveLaPaginaPedida(string numeroPagina, int[] idsEsperados)
    {
        var pagina = ListarConExito(new ConsultaPedidos(Pagina: numeroPagina, Tamano: "20"));

        Assert.Equal(45, pagina.Total);
        Assert.Equal(idsEsperados, Ids(pagina));
    }

    [Theory] // CA-14
    [InlineData(null, "101")]
    [InlineData(null, "0")]
    [InlineData("0", null)]
    public void Listar_PaginacionFueraDeRango_DevuelvePaginacionInvalida(string? numeroPagina, string? tamano)
    {
        var error = ListarConError(new ConsultaPedidos(Pagina: numeroPagina, Tamano: tamano));

        Assert.Equal("PAGINACION_INVALIDA", error.Codigo);
    }

    [Fact] // CA-15
    public void Listar_FiltrosVacios_SeIgnoran()
    {
        var sinFiltros = ListarConExito(new ConsultaPedidos());
        var conVacios = ListarConExito(new ConsultaPedidos(Cliente: "  ", Estado: "", Pagina: "", Tamano: ""));

        Assert.Equal(sinFiltros.Total, conVacios.Total);
        Assert.Equal(sinFiltros.Pagina, conVacios.Pagina);
        Assert.Equal(sinFiltros.Tamano, conVacios.Tamano);
        Assert.Equal(Ids(sinFiltros), Ids(conVacios));
    }

    [Fact] // CA-16
    public void Listar_ClienteInexistente_DevuelveListaVacia()
    {
        var pagina = ListarConExito(new ConsultaPedidos(Cliente: "inexistente"));

        Assert.Equal(0, pagina.Total);
        Assert.Empty(pagina.Pedidos);
    }

    [Fact] // CA-17
    public void Listar_MismaFecha_DesempataPorIdDescendente()
    {
        var fecha = new DateOnly(2026, 10, 1);
        var repositorio = new RepositorioDePrueba(
            new Pedido(1, "PED-STGO-000001", "C-001", "Cliente Uno", fecha, "Pendiente", 1000, "STGO"),
            new Pedido(2, "PED-STGO-000002", "C-001", "Cliente Uno", fecha, "Pendiente", 2000, "STGO"),
            new Pedido(3, "PED-STGO-000003", "C-001", "Cliente Uno", fecha, "Pendiente", 3000, "STGO"));
        var servicio = new ListadoPedidosServicio(repositorio);

        var resultado = servicio.Listar(new ConsultaPedidos());

        Assert.True(resultado.EsValido);
        Assert.Equal(new[] { 3, 2, 1 }, Ids(resultado.Pagina!));
    }

    [Fact] // CA-18
    public void Listar_VariosErrores_InformaElPrimeroSegunPrioridad()
    {
        var error = ListarConError(new ConsultaPedidos(Estado: "Cancelado", Desde: "2026-10-31", Hasta: "2026-10-01"));

        Assert.Equal("FECHAS_INVERTIDAS", error.Codigo);
    }

    private PaginaPedidos ListarConExito(ConsultaPedidos consulta)
    {
        var resultado = _servicio.Listar(consulta);

        Assert.True(resultado.EsValido, $"Se esperaba éxito y llegó el error {resultado.Error?.Codigo}.");
        return resultado.Pagina!;
    }

    private ErrorValidacion ListarConError(ConsultaPedidos consulta)
    {
        var resultado = _servicio.Listar(consulta);

        Assert.False(resultado.EsValido, "Se esperaba un error de validación.");
        Assert.Null(resultado.Pagina);
        return resultado.Error!;
    }

    private static int[] Ids(PaginaPedidos pagina) => pagina.Pedidos.Select(p => p.Id).ToArray();

    private sealed class RepositorioDePrueba(params Pedido[] pedidos) : IPedidoRepositorio
    {
        public IReadOnlyList<Pedido> ObtenerTodos() => pedidos;

        public Pedido? ObtenerPorId(int id) => pedidos.FirstOrDefault(p => p.Id == id);
    }
}
