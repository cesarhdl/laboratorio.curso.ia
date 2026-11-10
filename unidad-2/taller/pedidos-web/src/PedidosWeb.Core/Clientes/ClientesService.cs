namespace PedidosWeb.Core.Clientes;

/// <summary>
/// Consultas sobre los clientes para la pantalla: su ficha y si está activo.
/// Módulo heredado, ordenado en la Unidad 3 sin cambiar su comportamiento.
/// </summary>
public class ClientesService
{
    private readonly List<Cliente> _clientes;

    public ClientesService() : this(ClientesDeEjemplo.Crear()) { }

    public ClientesService(List<Cliente> clientes)
    {
        _clientes = clientes;
    }

    /// <summary>
    /// Ficha del cliente para la pantalla: «Nombre | RUT | Zona | Activo/Inactivo».
    /// Texto vacío si el cliente no existe.
    /// </summary>
    public string ObtenerFicha(string id)
    {
        var cliente = BuscarPorId(id);
        if (cliente is null)
        {
            return "";
        }

        var rut = NormalizarRut(cliente.Rut);
        var zona = ZonaDe(cliente.Region);
        var estado = cliente.Activo ? "Activo" : "Inactivo";

        return $"{cliente.Nombre} | {rut} | {zona} | {estado}";
    }

    /// <summary>True si el cliente existe y está activo; false si está inactivo o no existe.</summary>
    public bool EstaActivo(string id)
    {
        var cliente = BuscarPorId(id);
        return cliente is not null && cliente.Activo;
    }

    /// <summary>El primer cliente con ese id, o null si no existe.</summary>
    private Cliente? BuscarPorId(string id) =>
        _clientes.FirstOrDefault(cliente => cliente.Id == id);

    /// <summary>RUT sin puntos ni espacios y con la «k» en mayúscula, como lo muestra la ficha.</summary>
    private static string NormalizarRut(string rut) =>
        rut.Replace(".", "").Replace(" ", "").ToUpper();

    /// <summary>Zona comercial según la región del cliente.</summary>
    private static string ZonaDe(string region) => region switch
    {
        "RM" or "V" => "Zona centro",
        "VIII" => "Zona sur",
        "II" => "Zona norte",
        _ => "Sin zona"
    };
}
