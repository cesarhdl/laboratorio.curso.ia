using System.Globalization;
using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

// Golden master: congela lo que calcula hoy legado/src/UCalculoTotales.pas (migracion/golden-master.csv).
// El CSV es el contrato: no se edita para que un test pase.
public class GoldenMasterTests
{
    private const string RutaRelativaCsv = "migracion/golden-master.csv";

    private static readonly Lazy<IReadOnlyDictionary<int, CasoGoldenMaster>> Casos = new(CargarCasos);

    public static IEnumerable<object[]> NumerosDeCaso() =>
        Casos.Value.Keys.OrderBy(caso => caso).Select(caso => new object[] { caso });

    [Fact]
    public void GoldenMaster_Csv_Tiene60Casos()
    {
        Assert.Equal(60, Casos.Value.Count);
    }

    [Theory]
    [MemberData(nameof(NumerosDeCaso))]
    public void Calcular_CasoDelGoldenMaster_IgualAlLegado(int caso)
    {
        var esperado = Casos.Value[caso];

        var resultado = CalculoTotales.Calcular(
            esperado.Lineas, esperado.EsVip, esperado.Region, esperado.FleteBase, esperado.TasaIva);

        Assert.Equal(esperado.Descuento, resultado.Descuento);
        Assert.Equal(esperado.Flete, resultado.Flete);
        Assert.Equal(esperado.Iva, resultado.Iva);
        Assert.Equal(esperado.Total, resultado.Total);
    }

    private static IReadOnlyDictionary<int, CasoGoldenMaster> CargarCasos()
    {
        var lineas = File.ReadAllLines(BuscarCsv());
        var encabezado = lineas[0].TrimEnd('\r').Split(',');
        int Columna(string nombre) => Array.IndexOf(encabezado, nombre);

        var casos = new Dictionary<int, CasoGoldenMaster>();
        foreach (var linea in lineas.Skip(1).Select(l => l.TrimEnd('\r')).Where(l => l.Length > 0))
        {
            var campos = linea.Split(',');
            decimal Monto(string nombre) => decimal.Parse(campos[Columna(nombre)], CultureInfo.InvariantCulture);

            var caso = new CasoGoldenMaster(
                Caso: int.Parse(campos[Columna("caso")], CultureInfo.InvariantCulture),
                EsVip: campos[Columna("tipo_cliente")] == "V",
                Region: int.Parse(campos[Columna("region")], CultureInfo.InvariantCulture),
                FleteBase: Monto("flete_base"),
                TasaIva: Monto("tasa_iva"),
                Lineas: LeerLineas(campos[Columna("lineas")]),
                Descuento: Monto("descuento"),
                Flete: Monto("flete"),
                Iva: Monto("iva"),
                Total: Monto("total"));

            casos.Add(caso.Caso, caso);
        }

        return casos;
    }

    // Formato: cantidad x precio x categoría, separadas por '|'. Ejemplo: 5x8210xBEB|30x5000xCON
    private static IReadOnlyList<LineaCalculo> LeerLineas(string texto) =>
        texto.Split('|')
            .Select(parte => parte.Split('x', 3))
            .Select(p => new LineaCalculo(
                int.Parse(p[0], CultureInfo.InvariantCulture),
                decimal.Parse(p[1], CultureInfo.InvariantCulture),
                p[2]))
            .ToList();

    // Sube desde la carpeta de ejecución de los tests hasta encontrar migracion/golden-master.csv.
    private static string BuscarCsv()
    {
        for (var carpeta = new DirectoryInfo(AppContext.BaseDirectory); carpeta is not null; carpeta = carpeta.Parent)
        {
            var ruta = Path.Combine(carpeta.FullName, RutaRelativaCsv);
            if (File.Exists(ruta))
            {
                return ruta;
            }
        }

        throw new FileNotFoundException($"No se encontró {RutaRelativaCsv} subiendo desde {AppContext.BaseDirectory}.");
    }

    private sealed record CasoGoldenMaster(
        int Caso,
        bool EsVip,
        int Region,
        decimal FleteBase,
        decimal TasaIva,
        IReadOnlyList<LineaCalculo> Lineas,
        decimal Descuento,
        decimal Flete,
        decimal Iva,
        decimal Total);
}
