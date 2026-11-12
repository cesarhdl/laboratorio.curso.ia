# Registro de calidad · Unidad 6

<!--
Lo completas tú durante el taller. Frases cortas: lo que hiciste y por qué.
Borra estos comentarios al terminar.
-->

## Preparación

- **Tests en `Skip` (pendientes de unidades anteriores):** ninguno. Al cerrar la U5 los 118 tests están en verde (incluidos los 60 del golden master).

## Etapa 1 · Tests que sí prueban

- **Módulo elegido:** `CalculoTotales` (`tests/PedidosWeb.Core.Tests/Totales/CalculoTotalesBordesTests.cs`).
- **Tests que propuso Claude:** 5, con valores esperados calculados a mano: IVA distinto de 0,19; pedido sin líneas; `" ofe "` en minúsculas con espacios; VIP con solo líneas OFE; pedido sobre `int.MaxValue`.
- **Cambio que hice a mano para probarlos:** uno por test en `src/PedidosWeb.Core/Totales/CalculoTotales.cs`, restaurando después:
  - IVA fijo `* 0.19m` → falla solo el borde 1.
  - Flete aunque no haya líneas → falla solo el borde 2.
  - `linea.Categoria != "OFE"` sin `Trim`/`ToUpperInvariant` → falla solo el borde 3.
  - Descuento sobre el subtotal → falla el borde 4 y 13 casos del golden master.
  - Importe con `int` → falla solo el borde 5.
- **Tests que me quedé:** los 5. Los bordes 1, 2, 3 y 5 detectan errores que el golden master no ve; el 4 repite una detección del golden master, pero documenta el caso VIP sin base de descuento.
- **Tests que borré y por qué:** ninguno; los 5 fallaron con su cambio.

## Etapa 2 · Pipeline seguro

- **`ci.yml`:** <enlace al primer run en verde>
- **`claude.yml` en `main`:** <sí / no>
- **Revisé:** permisos mínimos ☐ · timeout ☐ · versiones fijas ☐ · token solo como secreto ☐

## Etapa 3 · La falla

- **PR:** <enlace>
- **Qué rompí (a mano):** <archivo y cambio>
- **Diagnóstico de `@claude` (en una línea):** <resumen>
- **Mi decisión:** Aceptar ☐ · Ajustar ☐ · Rechazar ☐
- **Por qué:** <tu razón>
- **Corrección y merge:** <commit de la corrección; CI en verde ☐; merge ☐>
