# HU-01 · Plan de implementación

> Estado: **aprobado** por el PO el 2026-11-09. Implementa `spec.md` de esta carpeta; si el plan y la spec no coinciden, manda la spec.

## Enfoque
- Toda la lógica va en `PedidosWeb.Core`: validar, filtrar, ordenar y paginar. El endpoint solo traduce entre HTTP y Core.
- Los parámetros llegan a Core **como texto**, sin convertir (`string?`). Si el endpoint recibiera `DateOnly?` o `int?`, ASP.NET rechazaría una fecha o un número mal escritos con su propio 400, sin `{ codigo, mensaje }`. Así no se cumplirían `FECHA_INVALIDA` ni `PAGINACION_INVALIDA`.

## Archivos

| Acción | Archivo | Qué tiene |
|---|---|---|
| Nuevo | `src/PedidosWeb.Core/Pedidos/ConsultaPedidos.cs` | record con `Cliente`, `Estado`, `Desde`, `Hasta`, `Pagina` y `Tamano`, todos `string?` |
| Nuevo | `src/PedidosWeb.Core/Pedidos/PaginaPedidos.cs` | record `(int Total, int Pagina, int Tamano, IReadOnlyList<Pedido> Pedidos)` |
| Nuevo | `src/PedidosWeb.Core/Pedidos/ErrorValidacion.cs` | record `(string Codigo, string Mensaje)` y las constantes `FECHA_INVALIDA`, `FECHAS_INVERTIDAS`, `ESTADO_INVALIDO` y `PAGINACION_INVALIDA` |
| Nuevo | `src/PedidosWeb.Core/Pedidos/ResultadoListado.cs` | trae o una `PaginaPedidos` o un `ErrorValidacion`, nunca los dos |
| Nuevo | `src/PedidosWeb.Core/Pedidos/ListadoPedidosServicio.cs` | recibe `IPedidoRepositorio` y expone `Listar(ConsultaPedidos)` |
| Modifica | `src/PedidosWeb.Api/Program.cs` | registra el servicio (singleton) y agrega `GET /api/pedidos` |
| Nuevo | `tests/PedidosWeb.Core.Tests/Pedidos/ListadoPedidosServicioTests.cs` | un test por criterio, de CA-01 a CA-18 |
| Copia | `src/PedidosWeb.Core/Folios/FolioFormatter.cs` y `tests/.../Folios/FolioFormatterTests.cs` | la solución de la U1 (rama `solucion/u1-formatear-folio`), como en el camino A del README, para que `dotnet test` quede en verde |

No se tocan los datos de ejemplo, `.sln`, `.csproj` ni `AGENTS.md`, y no se agregan paquetes NuGet.

## Lógica de `Listar`
1. **Normalizar.** Recorto los espacios. Un parámetro vacío o con solo espacios cuenta como no enviado, incluidos `pagina` y `tamano` (RN-01, RN-06).
2. **Validar en el orden de RN-07.** Responde solo el primer error:
   1. `FECHA_INVALIDA`: compruebo `desde` y luego `hasta` con `DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)`.
   2. `FECHAS_INVERTIDAS`: `desde` es posterior a `hasta`.
   3. `ESTADO_INVALIDO`: el estado no está en la lista fija `Pendiente, Confirmado, Despachado, Entregado, Anulado`, comparada con `StringComparer.OrdinalIgnoreCase`.
   4. `PAGINACION_INVALIDA`: `pagina` o `tamano` no son enteros (`int.TryParse` con `InvariantCulture`), `pagina` es menor que 1 o `tamano` está fuera de 1..100.
3. **Filtrar con Y.**
   - Cliente: normalizo el nombre y el texto buscado (trim, `Normalize(FormD)`, quito `UnicodeCategory.NonSpacingMark` y paso a `ToUpperInvariant`), y uso `Contains` con `StringComparison.Ordinal`. Así «ñ» queda como «N» y «í» como «I».
   - Estado: `string.Equals(..., OrdinalIgnoreCase)`.
   - Fechas: `>= desde` y `<= hasta`, inclusivas.
4. **Ordenar** con `OrderByDescending(Fecha).ThenByDescending(Id)`.
5. **Paginar.** `total` se cuenta después de filtrar y antes de paginar. El salto es `(long)(pagina - 1) * tamano`: si supera `total`, la lista queda vacía sin error.

## Endpoint
```csharp
app.MapGet("/api/pedidos", (string? cliente, string? estado, string? desde, string? hasta,
                            string? pagina, string? tamano, ListadoPedidosServicio servicio) => ...)
```
- Responde `Results.Ok(pagina)` o `Results.BadRequest(error)`. La serialización por defecto ya produce `total`, `pagina`, `tamano`, `pedidos`, `codigo` y `mensaje`.
- Convive con `/api/pedidos/{id:int}`, que ya existe.
- Se elimina el comentario «HU-01 · PENDIENTE».

## Tests (trazabilidad)
- Prueban `ListadoPedidosServicio` con `PedidoRepositorioEnMemoria`, no el endpoint.
- Cada test lleva `// CA-xx` y compara de forma exacta los ids (en orden), `Total` y `Codigo`.
- Nombres con el formato `Listar_Escenario_Resultado`.

| CA | Test |
|---|---|
| CA-01 | `Listar_SinFiltros_DevuelvePrimeraPaginaOrdenada` |
| CA-02 | `Listar_ClienteParcial_DevuelvePedidosDelCliente` |
| CA-03 | `Listar_ClienteEnMayusculas_IgnoraMayusculas` |
| CA-04 | `Listar_ClienteSinTildes_IgnoraTildes` |
| CA-05 | `Listar_ClienteConNEnVezDeEnie_LaTrataComoN` |
| CA-06 | `Listar_EstadoEnMinusculas_DevuelveSoloEseEstado` |
| CA-07 | `Listar_EstadoInexistente_DevuelveEstadoInvalido` |
| CA-08 | `Listar_ClienteYFechas_CombinaConY` |
| CA-09 | `Listar_ClienteYEstado_CombinaConY` |
| CA-10 | `Listar_DesdeIgualHasta_IncluyeLosLimites` (Theory) |
| CA-11 | `Listar_FechasInvertidas_DevuelveError` |
| CA-12 | `Listar_FechaInexistente_DevuelveFechaInvalida` |
| CA-13 | `Listar_PaginaYTamano_DevuelveLaPaginaPedida` (Theory) |
| CA-14 | `Listar_PaginacionFueraDeRango_DevuelvePaginacionInvalida` (Theory) |
| CA-15 | `Listar_FiltrosVacios_SeIgnoran` |
| CA-16 | `Listar_ClienteInexistente_DevuelveListaVacia` |
| CA-17 | `Listar_MismaFecha_DesempataPorIdDescendente` (con un `RepositorioDePrueba` privado) |
| CA-18 | `Listar_VariosErrores_InformaElPrimeroSegunPrioridad` |

## Pasos (paso 4), con `dotnet test` al final de cada uno
1. Traer `FormatearFolio()` y sus tests desde la U1 → todo en verde.
2. Crear los records y el esqueleto del servicio.
3. Escribir la validación (CA-07, CA-11, CA-12, CA-14 y CA-18).
4. Escribir los filtros (CA-02 a CA-06, CA-08, CA-09, CA-10, CA-15 y CA-16).
5. Escribir el orden y la paginación (CA-01, CA-13 y CA-17).
6. Agregar el endpoint y probarlo a mano con `dotnet run --project src/PedidosWeb.Api`.

## Riesgos
- Que la normalización `FormD` falle con algún carácter. Lo cubren CA-04 y CA-05.
- Validar en otro orden. Lo cubre CA-18.
- Que ASP.NET convierta los parámetros antes que Core. Por eso llegan como `string?`.
- Desborde con una `pagina` gigante. Se calcula con `long`.
