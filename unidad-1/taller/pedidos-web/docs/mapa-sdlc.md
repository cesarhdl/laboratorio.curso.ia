# Mapa de uso de IA en el SDLC · Pedidos Web

> Paso 4 del taller de la Unidad 1.
> Para cada etapa: qué tarea concreta se delega a Claude, qué artefacto queda versionado,
> quién decide y qué revisa, qué puede salir mal y en qué evento de Scrum ocurre.
> Modelo: el caso de Valentina y TurnoVet (`caso-scrum-y-sdd.pdf`), traducido a Pedidos Web.

**Autor:** Cesar Herrera
**Fecha:** 2026-11-09

**Historia que se sigue en todo el mapa:** *Como cliente de la distribuidora, quiero que el portal muestre el folio de mi pedido con el mismo formato del sistema legado (`PED-STGO-000123`), para encontrar también mis pedidos antiguos.* → función `FormatearFolio()`.

| Etapa | Se delega a Claude | Artefacto versionado | Revisa una persona | Riesgo | Evento Scrum |
|---|---|---|---|---|---|
| Requisitos | Leer las reglas R1–R5 y los ejemplos del README y listar lo ambiguo **como preguntas**, sin resolverlo por su cuenta (regla 8 de `AGENTS.md`). Proponer un borrador de criterios de aceptación. | `README.md` (tabla de reglas R1–R5 y ejemplos) como spec de la historia; la historia priorizada en el Product Backlog. | El PO (yo) valida con negocio que el formato es igual al del legado (campo ASCII de 4 caracteres) y que cada criterio se puede probar: «2 a 4 letras A–Z», no «sucursal válida». Decide qué entra al sprint. | Con un criterio flojo, Claude supone que `Ñ`, tildes o 5 letras son válidas y genera folios que no calzan con el legado: los clientes no encuentran sus pedidos antiguos. | Sprint Planning (sale del Product Backlog). |
| Diseño | En plan mode: «Lee el README y `FolioFormatter.cs` y propón un plan de máximo 5 viñetas. No edites nada». Después: «Guárdalo como `docs/specs/folio/plan.md`». | `docs/specs/folio/plan.md`, junto a la spec. | Yo corrijo y apruebo el plan antes de que edite. Reviso: orden de validación (R1 antes de R3, por R5), que la validación sea solo A–Z ASCII y no `char.IsLetter`, que use `ToUpperInvariant`, que no agregue paquetes NuGet y que solo toque `src/` y `tests/`. | Sin plan aprobado, Claude elige `char.IsLetter` (acepta `Ñ` y tildes) o valida la sucursal antes que el número, y el error recién aparece en producción. | Inicio del sprint, justo después del Planning. |
| Código | Implementar `FormatearFolio()` tarea por tarea: 1) validar `numero` (R1), 2) normalizar `sucursal` (R2), 3) validar `sucursal` (R3), 4) armar el folio con ceros a la izquierda (R4). | `src/PedidosWeb.Core/Folios/FolioFormatter.cs` | Leo el código de cada tarea antes de pasar a la siguiente: tipo de excepción correcto (`ArgumentOutOfRangeException` frente a `ArgumentException`), manejo de `null` y vacío, y que no invente reglas que no están en el README. | Los 3 tests base pasan aunque falten R2, R3 o R5: el código «funciona en mi PC» pero rompe con `" stgo "`, `"ÑUÑO"` o `(0, "S")`. | Durante el sprint; el avance y los bloqueos se anotan en la Daily. |
| Pruebas | Escribir un test xUnit por cada fila de ejemplos del README que no tiene test (las 10 restantes, al menos uno por regla R1–R5) y ejecutar `dotnet test`. | `tests/PedidosWeb.Core.Tests/Folios/FolioFormatterTests.cs` | Compruebo que cada fila del README tiene su test y que los valores esperados salen **del README, no del código**. Ejecuto `dotnet test` yo mismo. Aplico la Definition of Done de `AGENTS.md`. | Claude ajusta el test al código en vez del código al README (test circular), y el verde no prueba nada. Si falta un test, una regla se puede romper sin que nadie se entere. | Durante el sprint, antes de dar la historia por terminada (DoD). |
| Revisión y despliegue | Resumir el diff y confirmar `dotnet test` en verde. **No** hace commit, push ni merge (regla 3 de `AGENTS.md`). | Commit `feat: FormatearFolio con reglas R1–R5` en `main`. Despliegue: hoy no hay; desde las próximas unidades, PR + pipeline de CI que corre `dotnet test` antes del merge. | Leo y entiendo el diff completo y hago commit y push yo. En la Review muestro al negocio folios generados por el portal al lado de los del legado. El despliegue lo aprueba una persona, después del CI en verde. | Commit sin leer el diff: se cuela un cambio fuera de alcance (`.idea/`, un paquete NuGet, `AGENTS.md`). Desplegar un folio distinto al del legado rompe las búsquedas de los clientes. | Sprint Review (el negocio ve el incremento). |
| Mejora continua | Proponer la redacción de la regla aprendida. No edita `AGENTS.md` (regla 6): la propone en el chat. | `AGENTS.md`, nueva sección «Aprendido en la retro del sprint 1». Por ejemplo: «Las letras válidas son solo A–Z ASCII; no uses `char.IsLetter`» y «Los valores esperados de los tests salen de los ejemplos del README». | Yo decido qué regla entra, la escribo en `AGENTS.md` y la verifico en la próxima historia (por ejemplo, `CalcularTotal`). | Sin retro, el mismo error se repite en la próxima historia y hay que corregirlo a mano en cada prompt. | Retrospectiva. |

## Evidencia del paso 3

> Datos reales de las rondas A (sin IA) y B (con Claude Code). Los errores de la columna «Riesgo» se deben contrastar con lo que pasó de verdad.

| | Sin IA | Con Claude Code |
|---|---|---|
| Tiempo (min) | _por completar_ | _por completar_ |
| Errores encontrados | _por completar_ | _por completar_ |
| Qué tuve que corregir | _por completar_ | _por completar_ |
