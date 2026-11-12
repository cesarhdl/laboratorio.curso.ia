using PedidosWeb.Core.Totales;

namespace PedidosWeb.Core.Tests.Totales;

// Taller U6, etapa 1: bordes que migracion/golden-master.csv no cubre.
// Los valores esperados se calcularon a mano desde las reglas, no ejecutando el código.
public class CalculoTotalesBordesTests
{
    private const int RegionSantiago = 13;
    private const decimal FleteBaseSantiago = 3500m;
    private const decimal Iva19 = 0.19m;

    [Fact] // Borde 1: los 60 casos del golden master usan IVA 0,19.
    public void Calcular_TasaIvaDistintaDe19_UsaLaTasaRecibida()
    {
        var lineas = new[] { new LineaCalculo(10, 10_000m, "BEB") };

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, RegionSantiago, FleteBaseSantiago, tasaIva: 0.20m);

        // (100.000 + 3.500) × 0,20 = 20.700
        Assert.Equal(20_700m, resultado.Iva);
        Assert.Equal(124_200m, resultado.Total);
    }

    [Fact] // Borde 2: el golden master no tiene pedidos sin líneas.
    public void Calcular_PedidoSinLineas_NoCobraFleteNiNada()
    {
        var resultado = CalculoTotales.Calcular(
            Array.Empty<LineaCalculo>(), esVip: true, region: 12, fleteBase: 16_490m, Iva19);

        Assert.Equal(0m, resultado.Subtotal);
        Assert.Equal(0m, resultado.Descuento);
        Assert.Equal(0m, resultado.Flete);
        Assert.Equal(0m, resultado.Iva);
        Assert.Equal(0m, resultado.Total);
    }

    [Fact] // Borde 3: en el golden master OFE siempre viene en mayúsculas y sin espacios.
    public void Calcular_CategoriaOfeEnMinusculasConEspacios_NoEntraALaBaseDeDescuento()
    {
        var lineas = new[]
        {
            new LineaCalculo(10, 60_000m, " ofe "),
            new LineaCalculo(10, 10_000m, "BEB")
        };

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, RegionSantiago, FleteBaseSantiago, Iva19);

        // Base 100.000 (sin la línea OFE): no llega a 500.000, sin descuento.
        Assert.Equal(100_000m, resultado.BaseDescuento);
        Assert.Equal(0m, resultado.Descuento);
        // (700.000 + 3.500) × 0,19 = 133.665
        Assert.Equal(837_165m, resultado.Total);
    }

    [Fact] // Borde 4: VIP cuya base de descuento es 0 porque todas sus líneas son OFE.
    public void Calcular_VipConSoloLineasOfe_NoAplicaDescuento()
    {
        var lineas = new[] { new LineaCalculo(10, 60_000m, "OFE") };

        var resultado = CalculoTotales.Calcular(lineas, esVip: true, RegionSantiago, FleteBaseSantiago, Iva19);

        Assert.Equal(0.03m, resultado.PorcDescuento);
        Assert.Equal(0m, resultado.Descuento);
        // (600.000 + 3.500) × 0,19 = 114.665
        Assert.Equal(718_165m, resultado.Total);
    }

    [Fact] // Borde 5: subtotal mayor que int.MaxValue (2.147.483.647).
    public void Calcular_PedidoGiganteSobreIntMaxValue_NoSeDesborda()
    {
        var lineas = new[] { new LineaCalculo(50_000, 50_000m, "BEB") };

        var resultado = CalculoTotales.Calcular(lineas, esVip: false, RegionSantiago, FleteBaseSantiago, Iva19);

        Assert.Equal(2_500_000_000m, resultado.Subtotal);
        Assert.Equal(200_000_000m, resultado.Descuento);
        // (2.300.000.000 + 3.500) × 0,19 = 437.000.665
        Assert.Equal(2_737_004_165m, resultado.Total);
    }
}
