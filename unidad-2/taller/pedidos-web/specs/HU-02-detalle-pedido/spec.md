# HU-02 · Detalle de un pedido

> Spec breve creada desde `docs/plantilla-spec-breve.md` (taller U3, etapa 1).

**Historia:** Como vendedor, quiero ver el detalle de un pedido con su cliente, sus líneas y su total, para responderle al cliente sin entrar al sistema Delphi.

## Qué devuelve

La ruta `GET /api/pedidos/{id}/detalle` devuelve:

- **Del pedido:** `id`, `folio`, `fecha`, `estado` y `sucursal`.
- **Del cliente:** `clienteId` y `clienteNombre`, tal como vienen en el pedido.
- **De cada línea**, en orden de número: `numero`, `producto`, `cantidad`, `precioUnitario` y `subtotal` (`cantidad × precioUnitario`).
- **Total:** la suma de los subtotales de sus líneas, en CLP **neto, sin IVA** (`decimal` sin decimales).

## Criterios de aceptación

Cada criterio se puede comprobar con un sí o un no.

- **CA-01** · Dado el pedido 8, cuando calculo su total, entonces es 120.100 (3 × 21.500 + 12 × 2.950 + 10 × 2.020).
- **CA-02** · Dado cada uno de los 45 pedidos de ejemplo, cuando calculo su total desde sus líneas, entonces es igual a su `Pedido.Total`.
- **CA-03** · Dado el pedido 8, cuando pido su detalle, entonces recibo:
  - folio `PED-STGO-000008`, fecha 2026-10-20, estado «Entregado» y sucursal `STGO`;
  - cliente `C-001` «Ferretería El Clavo»;
  - 3 líneas numeradas 1, 2 y 3, con subtotales 64.500, 35.400 y 20.200;
  - total 120.100.
- **CA-04** · Dado el pedido 999, que no existe, cuando pido su detalle, entonces no recibo detalle, y la API responde 404 con `{ "codigo": "PEDIDO_NO_ENCONTRADO", "mensaje": "..." }`.

## Si el pedido no existe

La API responde **404** con `{ "codigo": "PEDIDO_NO_ENCONTRADO", "mensaje": "..." }`. Es el mismo formato de error de la HU-01; el `mensaje` va en español y los tests comparan el `codigo`.

## Incrementos (un commit cada uno)

1. **Calcular el total** de un pedido desde sus líneas, en Core → CA-01 y CA-02.
2. **Obtener el detalle** (pedido, cliente, líneas con subtotal y total), en Core → CA-03 y CA-04 (el «no hay detalle»).
3. **Publicarlo en la API** con la ruta nueva `GET /api/pedidos/{id}/detalle` → CA-04 (el 404) y revisión en el navegador del pedido 8.

## Fuera de alcance

- IVA y total con IVA.
- Datos del cliente fuera del pedido (RUT, región, si está activo) y cualquier uso de `ClientesService`, que se refactoriza en la etapa 2.
- Cambiar la ruta existente `GET /api/pedidos/{id}`.
- Tests HTTP del endpoint (requieren un paquete NuGet): el incremento 3 se revisa en el navegador.
