# Defectos corregidos · migración del cálculo de totales

> Regla: si un valor esperado de `migracion/golden-master.csv` cambia, ese caso aparece aquí.
> Referencias a reglas y riesgos: `docs/diagnostico/reglas-negocio.md` y `riesgos.md` (U4).

## D1 · R05 · Redondeo al peso (CORREGIDO)

- **Qué hace el legado:** redondea con `Trunc(x + 0,49)`, así que un ,50 exacto baja en vez de subir (`legado/src/UCalculoTotales.pas:50`; duplicado en `legado/src/UUtilidades.pas:30`). Se aplica al descuento, al flete y al IVA. Es la regla 9 del diagnóstico.
- **Corrección:** el C# redondea con `Math.Round(x, 0, MidpointRounding.AwayFromZero)`: ,50 sube (`src/PedidosWeb.Core/Totales/CalculoTotales.cs`, `RedondearAlPeso`).
- **Por qué:** con el redondeo del legado el cliente paga un peso de diferencia en el descuento, el flete o el IVA cada vez que un cálculo termina justo en ,50; no es el redondeo habitual y no hay regla del negocio que lo justifique.
- **Casos del golden master que cambian:** 13 — casos 2, 6, 8, 18, 21, 23, 25, 27, 36, 37, 42, 46 y 54.
  - En 11 casos el total sube entre 1 y 2 pesos: el flete de las regiones 11 y 12 (9 casos: 18, 21, 23, 27, 36, 37, 42, 46 y 54; en el 21 también el IVA) o solo el IVA (2 y 8) dejan de bajar el ,50.
  - En 2 casos el total **baja**: 6 (−1) y 25 (−2), porque sube un peso el descuento y con él baja el IVA.
  - Valores recalculados con aritmética entera; la réplica con el redondeo del legado reproduce el CSV anterior sin diferencias.
- **Suma de la columna total:** 58.202.135 → 58.202.144 (+9).
- **Commit:** `fix: corrige R05, redondeo al peso con ,50 hacia arriba (U5, etapa 3)`, separado de `feat: cálculo de totales en C#, igual al legado (U5, etapa 2)`.
- **Convivencia:** mientras conviven ambos sistemas, el mismo pedido puede diferir en 1 o 2 pesos entre el legado y el portal. Contabilidad debe aceptar esa diferencia como esperada (no es un error) y conciliarla por pedido, no por suma.

## H1 · Umbrales del descuento por volumen: pantalla `>` y grabación `>=` (HALLAZGO, NO CORREGIDO)

- **Qué encontraste:** el descuento por volumen está programado dos veces con condiciones distintas.
  - Pantalla (lo que ve el vendedor): «mayor que» 500.000 y 1.000.000 (`legado/src/UCalculoTotales.pas:77` y `:79`).
  - Grabación (lo que queda en la base): «mayor o igual que» (`legado/src/UFrmPedidos.pas:296` y `:298`).
  - El aviso que detectaría la diferencia está apagado (`legado/src/UFrmPedidos.pas:330`, `gModoPrueba` siempre `False`). Reglas 2 y 22, riesgo RS-06 del diagnóstico.
- **Efecto:** afecta a los pedidos cuya base de descuento es exactamente 500.000 o 1.000.000. En los datos de ejemplo son 13 de 60 (folios 1008, 1011, 1019, 1023, 1026, 1032, 1035, 1039, 1042, 1048, 1050, 1055 y 1060): el total grabado en `PEDIDOS.csv` es menor que el mostrado en **29.750** (base 500.000) o **35.700** (base 1.000.000) pesos. Suman **428.400** pesos. En los otros 47 pedidos, pantalla y grabación coinciden.
- **Quién decide:** el área comercial, con contabilidad: qué vale en el umbral, ¿«mayor que» (lo que vio el vendedor) o «mayor o igual» (lo que se cobró)? El C# replica **la pantalla** (`UCalculoTotales.pas`, «mayor que»), porque es el contrato del golden master. Si el negocio decide la otra versión, se corrige como D2, en su propio commit, y cambian estos 13 casos.
