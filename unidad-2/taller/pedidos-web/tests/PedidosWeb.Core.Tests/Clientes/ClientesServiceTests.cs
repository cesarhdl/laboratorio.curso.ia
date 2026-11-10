using PedidosWeb.Core.Clientes;

namespace PedidosWeb.Core.Tests.Clientes;

public class ClientesServiceTests
{
    private readonly ClientesService _servicio = new();

    [Fact]
    public void Proc2_ClienteInexistente_DevuelveTextoVacio()
    {
        var ficha = _servicio.proc2("C-999");

        Assert.Equal("", ficha);
    }

    [Fact]
    public void Proc2_ClienteActivo_DevuelveLaFicha()
    {
        var ficha = _servicio.proc2("C-001");

        Assert.Equal("Ferretería El Clavo | 76123456-0 | Zona centro | Activo", ficha);
    }

    [Fact]
    public void Proc2_RutConPuntosYKMinuscula_LoNormaliza()
    {
        var ficha = _servicio.proc2("C-004");

        Assert.Equal("Almacén La Esquina | 76990011-K | Zona centro | Activo", ficha);
    }

    // Tests de caracterización (etapa 2, paso 0): fijan lo que el código hace hoy,
    // para que el refactor no cambie el comportamiento sin que nos enteremos.
    [Theory]
    [InlineData("C-002", "Minimarket Doña Rosa | 77654321-7 | Zona norte | Activo")]
    [InlineData("C-003", "Botillería Los Andes | 78345612-5 | Zona sur | Activo")]
    [InlineData("C-007", "Kiosco Central | 77321654-1 | Zona sur | Inactivo")]
    [InlineData("C-008", "Distribuidora Norte | 76432198-7 | Zona centro | Activo")]
    public void Proc2_OtrasZonasInactivoYRutConEspacios_DevuelveLaFicha(string id, string fichaEsperada)
    {
        var ficha = _servicio.proc2(id);

        Assert.Equal(fichaEsperada, ficha);
    }

    [Fact]
    public void Proc2_RegionDesconocida_DevuelveSinZona()
    {
        var servicio = new ClientesService(new List<Cliente>
        {
            new() { Id = "C-100", Nombre = "Cliente Austral", Rut = "11.111.111-1", Region = "XII", Activo = true }
        });

        var ficha = servicio.proc2("C-100");

        Assert.Equal("Cliente Austral | 11111111-1 | Sin zona | Activo", ficha);
    }

    [Fact]
    public void Proc3_ClienteActivoEInactivo_DevuelveSiEstaActivo()
    {
        Assert.True(_servicio.proc3("C-001"));
        Assert.False(_servicio.proc3("C-007"));
        Assert.False(_servicio.proc3("C-999"));
    }
}
