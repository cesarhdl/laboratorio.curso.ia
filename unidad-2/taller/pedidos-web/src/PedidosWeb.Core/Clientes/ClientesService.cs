namespace PedidosWeb.Core.Clientes;

// Servicio heredado: lo escribió un ex integrante del equipo, con apuro.
// Funciona y tiene 4 tests. Nadie se ha atrevido a tocarlo.
public class ClientesService
{
    private readonly List<Cliente> _l;

    public ClientesService() : this(ClientesDeEjemplo.Crear()) { }

    public ClientesService(List<Cliente> l)
    {
        _l = l;
    }

    // ficha del cliente para la pantalla
    public string ObtenerFicha(string id)
    {
        Cliente? c = BuscarPorId(id);
        if (c == null) return "";
        string r = NormalizarRut(c.Rut);
        string t = ZonaDe(c.Region);
        string e = "";
        if (c.Activo == true) e = "Activo"; else e = "Inactivo";
        return c.Nombre + " | " + r + " | " + t + " | " + e;
    }

    // si existe y está activo
    public bool EstaActivo(string id)
    {
        Cliente? c = BuscarPorId(id);
        if (c == null) return false;
        if (c.Activo == true) return true; else return false;
    }

    // El primer cliente con ese id, o null si no existe.
    private Cliente? BuscarPorId(string id)
    {
        Cliente? c = null;
        for (int i = 0; i < _l.Count; i++)
        {
            if (_l[i].Id == id) { c = _l[i]; break; }
        }
        return c;
    }

    // RUT sin puntos ni espacios y con la «k» en mayúscula, como lo muestra la ficha.
    private static string NormalizarRut(string rut)
    {
        return rut.Replace(".", "").Replace(" ", "").ToUpper();
    }

    // Zona comercial según la región del cliente.
    private static string ZonaDe(string region)
    {
        string t = "";
        if (region == "RM") t = "Zona centro";
        else if (region == "V") t = "Zona centro";
        else if (region == "VIII") t = "Zona sur";
        else if (region == "II") t = "Zona norte";
        else t = "Sin zona";
        return t;
    }
}
