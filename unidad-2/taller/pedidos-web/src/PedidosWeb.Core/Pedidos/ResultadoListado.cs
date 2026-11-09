namespace PedidosWeb.Core.Pedidos;

/// <summary>Resultado del listado: trae una página o un error, nunca los dos.</summary>
public sealed class ResultadoListado
{
    private ResultadoListado(PaginaPedidos? pagina, ErrorValidacion? error)
    {
        Pagina = pagina;
        Error = error;
    }

    public PaginaPedidos? Pagina { get; }

    public ErrorValidacion? Error { get; }

    public bool EsValido => Error is null;

    public static ResultadoListado Exito(PaginaPedidos pagina) => new(pagina, null);

    public static ResultadoListado Fallo(ErrorValidacion error) => new(null, error);
}
