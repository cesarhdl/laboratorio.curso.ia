# HU-01 · Listar pedidos con filtros

> Estado: **aprobada** por el PO el 2026-11-09. Cualquier cambio de regla pasa primero por esta spec.

## Historia de usuario
Como vendedor, quiero listar pedidos filtrando por cliente, estado y rango de fechas.

## Objetivo
Que el vendedor consulte los pedidos desde el navegador, sin entrar al sistema Delphi, con `GET /api/pedidos`.
Puede combinar filtros opcionales por parte del nombre del cliente, por estado y por rango de fechas. Recibe, paginados, los pedidos que cumplen todos los filtros indicados, del más reciente al más antiguo. Si un filtro es inválido, recibe un error claro en lugar de un resultado sorpresa.

## Reglas de negocio
- **RN-01 · Filtros opcionales y combinados con Y.** Los parámetros `cliente`, `estado`, `desde` y `hasta` son opcionales. Si vienen varios, un pedido aparece solo si cumple todos. Sin filtros, se listan todos los pedidos, incluidos los anulados.
  - Un filtro vacío o con solo espacios cuenta como no enviado.
- **RN-02 · Cliente por parte del nombre.** `cliente` se recorta (trim) y se busca como texto dentro de `ClienteNombre`.
  - No distingue mayúsculas ni tildes: «panaderia» encuentra «Panadería».
  - La `ñ` se trata como `n`: «dona rosa» encuentra «Doña Rosa».
- **RN-03 · Estado.** Los estados válidos son exactamente `Pendiente`, `Confirmado`, `Despachado`, `Entregado` y `Anulado`.
  - No distingue mayúsculas: «entregado» equivale a «Entregado».
  - Cualquier otro valor es error `ESTADO_INVALIDO`.
- **RN-04 · Rango de fechas.** `desde` y `hasta` usan el formato `yyyy-MM-dd`, se comparan con `Fecha` y son **inclusivos**.
  - Se puede enviar solo uno de los dos.
  - Una fecha que no cumple el formato o que no existe (por ejemplo `2026-13-01`) es error `FECHA_INVALIDA`.
  - Si `desde` es posterior a `hasta`, es error `FECHAS_INVERTIDAS`.
- **RN-05 · Orden.** Por `Fecha` descendente. Si dos pedidos tienen la misma fecha, por `Id` descendente. El orden es fijo: el usuario no lo elige.
- **RN-06 · Paginación.** Se usan los parámetros `pagina` (desde 1, por defecto 1) y `tamano` (por defecto 20, máximo 100).
  - La respuesta es `{ "total": n, "pagina": p, "tamano": t, "pedidos": [...] }`, donde `total` es la cantidad de pedidos que cumplen los filtros antes de paginar.
  - `pagina` o `tamano` vacíos o con solo espacios cuentan como no enviados y usan su valor por defecto.
  - `pagina` menor que 1, o `tamano` fuera de 1..100, es error `PAGINACION_INVALIDA`. Lo mismo si alguno no es un número entero.
  - Una página posterior a la última no es error: responde 200 con `pedidos` vacío.
- **RN-07 · Formato de error.** Todo error de validación responde **HTTP 400** con `{ "codigo": "...", "mensaje": "..." }`. El `mensaje` va en español y es para personas; los tests comparan el `codigo`.
  - Si hay varios errores, se informa solo el primero, en este orden: `FECHA_INVALIDA`, `FECHAS_INVERTIDAS`, `ESTADO_INVALIDO`, `PAGINACION_INVALIDA`.

## Criterios de aceptación
Se usan los 45 pedidos de ejemplo de `PedidoRepositorioEnMemoria`, salvo en CA-17. Si un criterio no indica paginación, se usan los valores por defecto (`pagina` 1, `tamano` 20).

- [ ] **CA-01 · Sin filtros.** Dado los 45 pedidos, cuando listo sin parámetros, entonces recibo `total` 45, `pagina` 1 y `tamano` 20, y los ids 21, 42, 2, 23, 44, 4, 25, 6, 27, 8, 29, 10, 31, 12, 33, 14, 35, 16, 37 y 18, en ese orden.
- [ ] **CA-02 · Cliente por parte del nombre.** Dado los 45 pedidos, cuando filtro con `cliente=esquina`, entonces recibo `total` 5 y los ids 23, 31, 39, 7 y 15, en ese orden.
- [ ] **CA-03 · Sin distinguir mayúsculas.** Dado los 45 pedidos, cuando filtro con `cliente=ESQUINA`, entonces recibo el mismo resultado que en CA-02.
- [ ] **CA-04 · Sin distinguir tildes.** Dado los 45 pedidos, cuando filtro con `cliente=panaderia`, entonces recibo los ids 44, 4, 12, 20, 28 y 36.
- [ ] **CA-05 · La ñ como n.** Dado los 45 pedidos, cuando filtro con `cliente=dona rosa`, entonces recibo los ids 21, 29, 37, 45, 5 y 13.
- [ ] **CA-06 · Estado.** Dado los 45 pedidos, cuando filtro con `estado=entregado`, entonces recibo `total` 15 y todos los pedidos tienen `Estado` «Entregado».
- [ ] **CA-07 · Estado inválido.** Dado los 45 pedidos, cuando filtro con `estado=Cancelado`, entonces recibo 400 con `codigo` `ESTADO_INVALIDO`.
- [ ] **CA-08 · Cliente y fechas combinados.** Dado los 45 pedidos, cuando filtro con `cliente=esquina`, `desde=2026-10-01` y `hasta=2026-10-31`, entonces recibo exactamente los ids 23, 31 y 39.
- [ ] **CA-09 · Cliente y estado combinados.** Dado los 45 pedidos, cuando filtro con `cliente=esquina` y `estado=Entregado`, entonces recibo exactamente los ids 31 y 15.
- [ ] **CA-10 · Fechas inclusivas.** Dado los 45 pedidos:
  - cuando filtro con `desde=2026-10-31` y `hasta=2026-10-31`, entonces recibo solo el id 21;
  - cuando filtro con `desde=2026-09-02` y `hasta=2026-09-02`, entonces recibo solo el id 40.
- [ ] **CA-11 · Fechas invertidas.** Dado `desde=2026-10-31` y `hasta=2026-10-01`, cuando listo pedidos, entonces recibo 400 con `codigo` `FECHAS_INVERTIDAS`.
- [ ] **CA-12 · Fecha inválida.** Dado `desde=2026-13-01`, cuando listo pedidos, entonces recibo 400 con `codigo` `FECHA_INVALIDA`.
- [ ] **CA-13 · Paginación.** Dado los 45 pedidos:
  - cuando pido `pagina=3` y `tamano=20`, entonces recibo `total` 45 y 5 pedidos: los ids 36, 17, 38, 19 y 40;
  - cuando pido `pagina=4` y `tamano=20`, entonces recibo 200, `total` 45 y `pedidos` vacío.
- [ ] **CA-14 · Paginación inválida.** Dado `tamano=101`, o `tamano=0`, o `pagina=0`, cuando listo pedidos, entonces recibo 400 con `codigo` `PAGINACION_INVALIDA`.
- [ ] **CA-15 · Filtros vacíos.** Dado los 45 pedidos, cuando filtro con `cliente=` de dos espacios, `estado=` vacío, `pagina=` vacío y `tamano=` vacío, entonces recibo el mismo resultado que en CA-01.
- [ ] **CA-16 · Sin resultados.** Dado los 45 pedidos, cuando filtro con `cliente=inexistente`, entonces recibo 200 con `total` 0 y `pedidos` vacío.
- [ ] **CA-17 · Desempate por id.** Dado un repositorio de prueba con los pedidos 1, 2 y 3, todos con fecha 2026-10-01, cuando listo sin filtros, entonces recibo los ids 3, 2 y 1, en ese orden. Los 45 pedidos de ejemplo no tienen fechas repetidas, por eso este criterio usa otro repositorio.
- [ ] **CA-18 · Prioridad de errores.** Dado `estado=Cancelado`, `desde=2026-10-31` y `hasta=2026-10-01`, cuando listo pedidos, entonces recibo 400 con `codigo` `FECHAS_INVERTIDAS`.

## Ejemplos de entrada y salida

| Entrada | Salida esperada |
|---|---|
| `GET /api/pedidos?cliente=esquina&estado=entregado` | 200 · `{ "total": 2, "pagina": 1, "tamano": 20, "pedidos": [ {id 31…}, {id 15…} ] }` |
| `GET /api/pedidos?cliente=esquina&desde=2026-10-01&hasta=2026-10-31` | 200 · `total` 3 · ids 23, 31, 39 |
| `GET /api/pedidos?pagina=3&tamano=20` | 200 · `total` 45 · ids 36, 17, 38, 19, 40 |
| `GET /api/pedidos?desde=2026-10-31&hasta=2026-10-01` | 400 · `{ "codigo": "FECHAS_INVERTIDAS", "mensaje": "La fecha 'desde' no puede ser posterior a 'hasta'." }` |
| `GET /api/pedidos?estado=Cancelado` | 400 · `{ "codigo": "ESTADO_INVALIDO", "mensaje": "..." }` |

Cada pedido de `pedidos` se devuelve con los campos del modelo `Pedido`: `id`, `folio`, `clienteId`, `clienteNombre`, `fecha`, `estado`, `total` y `sucursal`.

## Errores esperados

| Código | Cuándo | HTTP |
|---|---|---|
| `FECHA_INVALIDA` | `desde` o `hasta` no son una fecha `yyyy-MM-dd` válida | 400 |
| `FECHAS_INVERTIDAS` | `desde` es posterior a `hasta` | 400 |
| `ESTADO_INVALIDO` | `estado` no es uno de los 5 estados válidos | 400 |
| `PAGINACION_INVALIDA` | `pagina` es menor que 1, `tamano` está fuera de 1..100 o alguno no es entero | 400 |

## Fuera de alcance
- Filtro por sucursal y filtro exacto por código de cliente (`ClienteId`).
- Modificar los datos de ejemplo o agregar base de datos (llega en la Unidad 5).

## Preguntas abiertas
Ninguna.
