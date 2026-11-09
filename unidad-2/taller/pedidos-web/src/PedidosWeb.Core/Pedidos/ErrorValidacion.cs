namespace PedidosWeb.Core.Pedidos;

/// <summary>Error de validación del listado (RN-07). La API lo responde con HTTP 400.</summary>
/// <param name="Codigo">Código estable que comparan los tests y el frontend.</param>
/// <param name="Mensaje">Explicación en español para personas.</param>
public sealed record ErrorValidacion(string Codigo, string Mensaje)
{
    public const string FechaInvalida = "FECHA_INVALIDA";
    public const string FechasInvertidas = "FECHAS_INVERTIDAS";
    public const string EstadoInvalido = "ESTADO_INVALIDO";
    public const string PaginacionInvalida = "PAGINACION_INVALIDA";
}
