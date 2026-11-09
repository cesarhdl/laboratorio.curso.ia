namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Parámetros del listado de pedidos (HU-01), tal como llegan de la petición: como texto, sin convertir.
/// Core los valida para responder errores con el formato de la spec (RN-07).
/// Un valor nulo, vacío o con solo espacios cuenta como no enviado.
/// </summary>
/// <param name="Cliente">Parte del nombre del cliente (RN-02).</param>
/// <param name="Estado">Uno de los 5 estados válidos, sin distinguir mayúsculas (RN-03).</param>
/// <param name="Desde">Fecha inicial inclusiva, formato yyyy-MM-dd (RN-04).</param>
/// <param name="Hasta">Fecha final inclusiva, formato yyyy-MM-dd (RN-04).</param>
/// <param name="Pagina">Número de página, desde 1; por defecto 1 (RN-06).</param>
/// <param name="Tamano">Pedidos por página, de 1 a 100; por defecto 20 (RN-06).</param>
public sealed record ConsultaPedidos(
    string? Cliente = null,
    string? Estado = null,
    string? Desde = null,
    string? Hasta = null,
    string? Pagina = null,
    string? Tamano = null);
