using System.Globalization;
using System.Text;

namespace PedidosWeb.Core.Pedidos;

/// <summary>
/// Lista pedidos con filtros, orden y paginación según specs/HU-01-listar-pedidos/spec.md.
/// </summary>
public sealed class ListadoPedidosServicio
{
    private const int PaginaPorDefecto = 1;
    private const int TamanoPorDefecto = 20;
    private const int TamanoMaximo = 100;
    private const string FormatoFecha = "yyyy-MM-dd";

    // RN-03: los únicos estados válidos.
    private static readonly HashSet<string> EstadosValidos = new(StringComparer.OrdinalIgnoreCase)
    {
        "Pendiente", "Confirmado", "Despachado", "Entregado", "Anulado"
    };

    private readonly IPedidoRepositorio _repositorio;

    public ListadoPedidosServicio(IPedidoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public ResultadoListado Listar(ConsultaPedidos consulta)
    {
        // RN-01 y RN-06: vacío o solo espacios cuenta como no enviado.
        var cliente = Limpiar(consulta.Cliente);
        var estado = Limpiar(consulta.Estado);
        var desdeTexto = Limpiar(consulta.Desde);
        var hastaTexto = Limpiar(consulta.Hasta);
        var paginaTexto = Limpiar(consulta.Pagina);
        var tamanoTexto = Limpiar(consulta.Tamano);

        // RN-07: se valida en orden fijo y se informa solo el primer error.
        DateOnly? desde = null;
        DateOnly? hasta = null;

        if (desdeTexto is not null)
        {
            if (!IntentarLeerFecha(desdeTexto, out var fecha))
            {
                return ResultadoListado.Fallo(new ErrorValidacion(
                    ErrorValidacion.FechaInvalida,
                    $"La fecha 'desde' debe tener el formato {FormatoFecha} y existir en el calendario."));
            }

            desde = fecha;
        }

        if (hastaTexto is not null)
        {
            if (!IntentarLeerFecha(hastaTexto, out var fecha))
            {
                return ResultadoListado.Fallo(new ErrorValidacion(
                    ErrorValidacion.FechaInvalida,
                    $"La fecha 'hasta' debe tener el formato {FormatoFecha} y existir en el calendario."));
            }

            hasta = fecha;
        }

        if (desde is not null && hasta is not null && desde > hasta)
        {
            return ResultadoListado.Fallo(new ErrorValidacion(
                ErrorValidacion.FechasInvertidas,
                "La fecha 'desde' no puede ser posterior a 'hasta'."));
        }

        if (estado is not null && !EstadosValidos.Contains(estado))
        {
            return ResultadoListado.Fallo(new ErrorValidacion(
                ErrorValidacion.EstadoInvalido,
                "El estado debe ser Pendiente, Confirmado, Despachado, Entregado o Anulado."));
        }

        var pagina = PaginaPorDefecto;
        var tamano = TamanoPorDefecto;

        if ((paginaTexto is not null && !int.TryParse(paginaTexto, NumberStyles.Integer, CultureInfo.InvariantCulture, out pagina))
            || (tamanoTexto is not null && !int.TryParse(tamanoTexto, NumberStyles.Integer, CultureInfo.InvariantCulture, out tamano))
            || pagina < 1
            || tamano < 1
            || tamano > TamanoMaximo)
        {
            return ResultadoListado.Fallo(new ErrorValidacion(
                ErrorValidacion.PaginacionInvalida,
                $"'pagina' debe ser un entero desde 1 y 'tamano' un entero entre 1 y {TamanoMaximo}."));
        }

        // RN-01: los filtros se combinan con Y.
        IEnumerable<Pedido> pedidos = _repositorio.ObtenerTodos();

        if (cliente is not null)
        {
            var buscado = NormalizarTexto(cliente);
            pedidos = pedidos.Where(p => NormalizarTexto(p.ClienteNombre).Contains(buscado, StringComparison.Ordinal));
        }

        if (estado is not null)
        {
            pedidos = pedidos.Where(p => string.Equals(p.Estado, estado, StringComparison.OrdinalIgnoreCase));
        }

        // RN-04: límites inclusivos.
        if (desde is not null)
        {
            pedidos = pedidos.Where(p => p.Fecha >= desde.Value);
        }

        if (hasta is not null)
        {
            pedidos = pedidos.Where(p => p.Fecha <= hasta.Value);
        }

        // RN-05: fecha descendente; con la misma fecha, id descendente.
        var filtrados = pedidos
            .OrderByDescending(p => p.Fecha)
            .ThenByDescending(p => p.Id)
            .ToList();

        // RN-06: total antes de paginar; una página posterior a la última viene vacía.
        var salto = (long)(pagina - 1) * tamano;
        var paginaDePedidos = salto >= filtrados.Count
            ? new List<Pedido>()
            : filtrados.Skip((int)salto).Take(tamano).ToList();

        return ResultadoListado.Exito(new PaginaPedidos(filtrados.Count, pagina, tamano, paginaDePedidos));
    }

    private static string? Limpiar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static bool IntentarLeerFecha(string texto, out DateOnly fecha) =>
        DateOnly.TryParseExact(texto, FormatoFecha, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);

    /// <summary>
    /// RN-02: sin tildes y en mayúsculas. FormD separa la letra de su marca (í → i + ´, ñ → n + ˜)
    /// y luego se descartan las marcas.
    /// </summary>
    private static string NormalizarTexto(string texto)
    {
        var descompuesto = texto.Trim().Normalize(NormalizationForm.FormD);
        var resultado = new StringBuilder(descompuesto.Length);

        foreach (var caracter in descompuesto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
            {
                resultado.Append(caracter);
            }
        }

        return resultado.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
    }
}
