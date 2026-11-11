# Plan de migración · Pedidos Legado → Pedidos Web

> Parte del diagnóstico de la U4 (`docs/diagnostico/matriz-decision.md`). Un módulo a la vez (*strangler fig*): el portal absorbe una pieza del legado, conviven, se compara y recién entonces se apaga esa pieza en el legado.

## 1. Etapas

Orden de la matriz de decisión, ajustado por dependencias (`docs/diagnostico/dependencias.md`): Stock y Toma de pedidos escriben en la base, así que van después de reemplazar el acceso a datos.

1. **Cálculo de totales** en C# (`src/PedidosWeb.Core/Totales/`): **hecho en la U5**. Golden master de 60 casos en verde, R05 corregido (D1), umbrales registrados (H1). Matriz: 4,35.
2. **Clientes y RUT** (matriz: 3,75): una sola validación de RUT en C#, decidiendo cuál de las dos actuales es la correcta (regla 16). Golden master: RUT de `CLIENTES.csv` con el resultado de cada validación.
3. **Informes** (3,70, empate Reemplazar/Modernizar): decidir herramienta de reportes o C#; el IVA sale del parámetro, no de 0,19 fijo (RS-10).
4. **Acceso a datos** (3,25, Reemplazar): DBISAM → PostgreSQL, sin clave en el código ni rutas fijas (RS-08, RS-16). Prerrequisito de las etapas 5 y 6.
5. **Stock** (3,45): con bloqueo y transacción; crear o descartar `MOV_STOCK` (RS-04, RS-09).
6. **Toma de pedidos** (3,35): grabación en una transacción, folio sin duplicados y SQL con parámetros (RS-01, RS-02, RS-03). Usa el cálculo de la etapa 1.

## 2. Convivencia

- El legado sigue tomando y grabando pedidos. El portal **calcula en paralelo** cada pedido nuevo con `CalculoTotales` y compara su total con el `TOTAL` grabado en el legado.
- Una diferencia es **esperada** solo si se explica por un registro de `defectos-corregidos.md`: D1 (1 o 2 pesos por redondeo) o H1 (pedidos con base exactamente 500.000 o 1.000.000). Cualquier otra diferencia detiene la etapa y se investiga.
- Cada pedido real con una diferencia no explicada se agrega como caso nuevo al golden master antes de corregir nada.

## 3. Riesgos

| Riesgo | Mitigación |
|---|---|
| El total mostrado y el grabado ya difieren en el legado (H1, RS-06); al migrar se puede elegir la versión equivocada. | El C# replica la pantalla y lo deja documentado; el área comercial decide antes del corte. Si cambia, es D2 en su propio commit. |
| El golden master no cubre algún caso real: no tiene pedidos sin líneas y solo 2 casos llegan al tope de 10 %. | Convivencia en paralelo y cada diferencia no explicada pasa a ser un caso nuevo del golden master. |
| Pedidos grabados a medias y folios duplicados en el legado (RS-02, RS-03) contaminan la comparación. | Comparar solo pedidos completos (cabecera con total y sus líneas); los demás se informan aparte. |
| Dinero en `Double` y SQL armado con `FloatToStr` en el legado (RS-05): un total puede quedar mal grabado según la configuración regional del PC. | El portal usa `decimal`; las diferencias de formato se revisan en la convivencia y se corrigen de raíz en la etapa 6. |
| Contabilidad no acepta diferencias de 1 o 2 pesos (D1). | Acordar la regla de redondeo con contabilidad antes del corte y conciliar por pedido, no por suma. |

## 4. Criterio de corte

- **Cuándo se apaga la pieza del legado:** golden master 100 % en verde, al menos un cierre contable mensual completo en convivencia sin diferencias no explicadas, y aprobación de contabilidad y del área comercial (decisión de H1 incluida).
- **Cómo se vuelve atrás:** el legado no se borra ni se modifica (`legado/` es solo lectura). Si algo falla después del corte, se vuelve a usar el cálculo del legado y el pedido con problema entra al golden master antes de reintentar.
