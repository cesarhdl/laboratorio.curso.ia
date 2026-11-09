using PedidosWeb.Core.Pedidos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPedidoRepositorio, PedidoRepositorioEnMemoria>();
builder.Services.AddSingleton<ListadoPedidosServicio>();

var app = builder.Build();

// Para comprobar que la API está arriba.
app.MapGet("/salud", () => Results.Ok(new { estado = "ok" }));

// Detalle de un pedido por id.
app.MapGet("/api/pedidos/{id:int}", (int id, IPedidoRepositorio repositorio) =>
    repositorio.ObtenerPorId(id) is { } pedido
        ? Results.Ok(pedido)
        : Results.NotFound());

// HU-01 · Listar pedidos con filtros (specs/HU-01-listar-pedidos/spec.md).
// Los parámetros llegan como texto: Core los valida y responde el error con el formato de la spec.
app.MapGet("/api/pedidos", (
    string? cliente,
    string? estado,
    string? desde,
    string? hasta,
    string? pagina,
    string? tamano,
    ListadoPedidosServicio servicio) =>
{
    var resultado = servicio.Listar(new ConsultaPedidos(cliente, estado, desde, hasta, pagina, tamano));

    return resultado.EsValido
        ? Results.Ok(resultado.Pagina)
        : Results.BadRequest(resultado.Error);
});

app.Run();

// Permite crear tests de integración con WebApplicationFactory<Program> más adelante.
public partial class Program { }
