namespace PedidosWeb.Core.Folios;

/// <summary>
/// Genera el folio de un pedido con el mismo formato del sistema legado.
/// Las reglas completas y los ejemplos están en el README, sección «La función del taller».
/// </summary>
public static class FolioFormatter
{
    private const int NumeroMinimo = 1;
    private const int NumeroMaximo = 999999;
    private const int LargoMinimoSucursal = 2;
    private const int LargoMaximoSucursal = 4;

    /// <summary>
    /// Devuelve el folio con el formato PED-{SUCURSAL}-{NUMERO}, por ejemplo "PED-STGO-000123".
    /// </summary>
    /// <param name="numero">Número correlativo del pedido, entre 1 y 999999.</param>
    /// <param name="sucursal">Código de la sucursal: de 2 a 4 letras de la A a la Z.</param>
    /// <exception cref="ArgumentOutOfRangeException">Si <paramref name="numero"/> está fuera de rango (R1).</exception>
    /// <exception cref="ArgumentException">Si <paramref name="sucursal"/> es nula, vacía o no cumple R3.</exception>
    public static string FormatearFolio(int numero, string sucursal)
    {
        // R1 y R5: el número se valida antes que la sucursal.
        if (numero < NumeroMinimo || numero > NumeroMaximo)
        {
            throw new ArgumentOutOfRangeException(
                nameof(numero), numero,
                $"El número del pedido debe estar entre {NumeroMinimo} y {NumeroMaximo}.");
        }

        // R2: sin espacios en los extremos y en mayúsculas (invariante, para no depender de la cultura).
        ArgumentException.ThrowIfNullOrWhiteSpace(sucursal);
        var codigo = sucursal.Trim().ToUpperInvariant();

        // R3: el legado guarda el código en un campo ASCII de 4 caracteres.
        if (!EsCodigoSucursalValido(codigo))
        {
            throw new ArgumentException(
                $"La sucursal debe tener entre {LargoMinimoSucursal} y {LargoMaximoSucursal} letras de la A a la Z.",
                nameof(sucursal));
        }

        // R4: número con ceros a la izquierda hasta 6 dígitos.
        return $"PED-{codigo}-{numero:D6}";
    }

    private static bool EsCodigoSucursalValido(string codigo)
    {
        if (codigo.Length < LargoMinimoSucursal || codigo.Length > LargoMaximoSucursal)
        {
            return false;
        }

        // Solo A–Z ASCII: char.IsLetter aceptaría Ñ y letras con tilde.
        foreach (var letra in codigo)
        {
            if (letra < 'A' || letra > 'Z')
            {
                return false;
            }
        }

        return true;
    }
}
