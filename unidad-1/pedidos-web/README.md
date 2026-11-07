# Pedidos Web

Portal web de pedidos de la distribuidora. Es la modernización de **Pedidos Legado**, un sistema de escritorio hecho en Delphi con DBISAM que la empresa usa hace 18 años.

Este repositorio es el **caso hilo** del curso *Desarrollando con IA* (Vela LATAM). Crece unidad a unidad: hoy (Unidad 1) tiene una biblioteca con la primera función del dominio; en las próximas unidades se suman la API en ASP.NET Core, el módulo de totales migrado, PostgreSQL y el pipeline de CI.

> Todo el contenido es ficticio y tiene fines educativos.

## Stack

| Pieza | Versión | Para qué |
|---|---|---|
| .NET SDK | 10 | Compilar y ejecutar |
| C# | el que trae el SDK | Lenguaje del proyecto |
| xUnit | 2.9 | Tests |
| JetBrains Rider | la versión vigente del All Products Pack | IDE del curso |
| Claude Code | 2.1.277 o superior | Agente de código (lee `AGENTS.md`) |

## Estructura

```
pedidos-web/
├── AGENTS.md                         ← reglas del proyecto para agentes de IA
├── PedidosWeb.sln
├── src/
│   └── PedidosWeb.Core/              ← lógica de dominio
│       └── Folios/FolioFormatter.cs  ← aquí implementas FormatearFolio()
├── tests/
│   └── PedidosWeb.Core.Tests/        ← tests xUnit
│       └── Folios/FolioFormatterTests.cs
└── docs/                             ← aquí va tu mapa-sdlc.md (paso 4)
```

## Cómo empezar

1. Clona el repo y ábrelo en Rider: **File → Open** y elige `PedidosWeb.sln`.
2. Compila y ejecuta los tests desde la terminal integrada de Rider:

   ```bash
   dotnet build
   dotnet test
   ```

3. **Resultado esperado hoy:** compila y los 3 tests de `FolioFormatterTests` fallan con `NotImplementedException`. Es normal: la función todavía no existe. La implementas en el paso 3 del taller.

---

## La función del taller: `FormatearFolio()`

### Para qué sirve

Cada pedido tiene un **folio**: el código que aparece en la factura, en el correo al cliente y en las búsquedas del portal. El sistema legado lo arma con el código de la sucursal y el número correlativo del pedido. El portal nuevo tiene que generarlo **exactamente igual**, o los clientes no van a encontrar sus pedidos antiguos.

### Firma

Archivo: `src/PedidosWeb.Core/Folios/FolioFormatter.cs`

```csharp
public static string FormatearFolio(int numero, string sucursal)
```

### Formato de salida

```
PED-{SUCURSAL}-{NUMERO}
```

- `PED` es un prefijo fijo.
- `{SUCURSAL}` es el código de la sucursal en mayúsculas.
- `{NUMERO}` es el número del pedido con ceros a la izquierda hasta completar 6 dígitos.

### Reglas

| # | Regla |
|---|---|
| R1 | `numero` debe estar entre **1 y 999999** (ambos incluidos). Si no, lanza `ArgumentOutOfRangeException`. |
| R2 | A `sucursal` se le quitan los espacios del inicio y del final, y se pasa a **mayúsculas**. |
| R3 | Después de R2, `sucursal` debe tener **entre 2 y 4 letras de la A a la Z**. Sin `Ñ`, sin tildes, sin números ni espacios internos: el legado guarda el código en un campo ASCII de 4 caracteres. Si no se cumple, o si `sucursal` es `null` o vacía, lanza `ArgumentException` (vale una derivada, como `ArgumentNullException`). |
| R4 | `numero` se completa con ceros a la izquierda hasta 6 dígitos. |
| R5 | Si `numero` y `sucursal` son inválidos a la vez, se valida primero `numero` (R1). |

### Ejemplos

| `numero` | `sucursal` | Resultado |
|---|---|---|
| `123` | `"STGO"` | `"PED-STGO-000123"` |
| `1` | `"VAL"` | `"PED-VAL-000001"` |
| `999999` | `"CO"` | `"PED-CO-999999"` |
| `45` | `" stgo "` | `"PED-STGO-000045"` |
| `0` | `"STGO"` | `ArgumentOutOfRangeException` |
| `1000000` | `"STGO"` | `ArgumentOutOfRangeException` |
| `-5` | `"VAL"` | `ArgumentOutOfRangeException` |
| `7` | `""`, `"   "` o `null` | `ArgumentException` |
| `7` | `"S"` | `ArgumentException` (1 letra) |
| `7` | `"SANTI"` | `ArgumentException` (5 letras) |
| `7` | `"ÑUÑO"` | `ArgumentException` (la `Ñ` no es A–Z) |
| `7` | `"ST1"` | `ArgumentException` (tiene un número) |
| `0` | `"S"` | `ArgumentOutOfRangeException` (R5) |

### Lo que ya está y lo que falta

- `FolioFormatterTests.cs` trae **3 tests** con los casos base (las tres primeras filas de la tabla).
- **Los demás casos no tienen test.** Si tu implementación, o la de Claude, olvida una regla, los tests no te van a avisar. Escribe tú los tests que faltan.

---

## Taller de la Unidad 1: lo que haces en este repo

| Paso | Qué haces aquí |
|---|---|
| 1 | Abres `claude` en la terminal integrada de Rider, dentro de esta carpeta, y le preguntas: *«¿Cuáles son las reglas de este proyecto?»*. Debe citar `AGENTS.md`. |
| 3 | Implementas `FormatearFolio()` dos veces: **ronda A sin IA** y **ronda B con Claude Code**. Anotas tiempo, errores y qué corregiste. Entre una ronda y otra puedes volver al estado inicial con `git stash` o `git checkout -- src`. |
| 4 | Creas `docs/mapa-sdlc.md` con la plantilla de la guía. |
| 5 | Haces commit: `git add docs/mapa-sdlc.md` y `git commit -m "docs: mapa de uso de IA en el SDLC (U1)"`. |

## Licencias

- Claude Team Standard (Claude Code se autentica con la cuenta Team).
- JetBrains All Products Pack (Rider).
