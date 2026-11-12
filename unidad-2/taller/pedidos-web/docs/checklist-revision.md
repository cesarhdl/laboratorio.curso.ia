# Checklist de revisión · pedidos-web

## Antes de pedir revisión
- [ ] `dotnet test` en verde y un test por cada criterio de aceptación de la spec.
- [ ] Rompí a mano cada límite (`>=` → `>`, `<=` → `<`, sin validación) y falló su test.
- [ ] Corrí el subagente `revisor` sobre la rama y respondí sus hallazgos antes de abrir el PR.
- [ ] La descripción tiene Qué cambia, Por qué (escrito por mí) y Cómo probarlo, sin `[COMPLETAR]`.
- [ ] El PR es chico: una historia y nada fuera de su alcance.

## Al revisar (persona)
- [ ] ¿El cambio cumple el «por qué» del negocio, no solo la spec? (la IA revisa contra la spec, no contra la intención).
- [ ] ¿Qué decisión de negocio quedó implícita y nadie la tomó? (ej.: qué estados cuentan como venta).
- [ ] ¿Cómo se relaciona con lo que ya existe (HU-01, HU-02) y hay contradicciones de criterio?

## Con los hallazgos de la IA
- [ ] Cada hallazgo tiene respuesta: acepto, discutir o descarto, con argumento.
- [ ] Antes de aceptar, compruebo en el código que el hallazgo es real (archivo:línea).
- [ ] «0 hallazgos» no significa «sin problemas»: igual hago mi revisión humana.

## Merge
- [ ] CI en verde en el último commit del PR.
- [ ] Cada hilo respondido y las preguntas abiertas registradas (no se pierden en el merge).
- [ ] El merge lo hace una persona después de leer el diff.

## Mi PR de HU-03
- **PR:** https://github.com/cesarhdl/laboratorio.curso.ia/pull/1
- **Hallazgo de la IA que más discutimos:** el revisor local propuso unificar la validación de fechas con
  `ListadoPedidosServicio`. Lo descarté: la spec pide una clase `FiltroPedidos` separada, el listado usa
  códigos de error HTTP y no excepciones, y unificarlos agrandaba el PR. En GitHub, Claude no dejó hallazgos
  (los 2 reales ya se habían corregido con el revisor local).
- **Mi hallazgo humano:** el filtro incluye pedidos anulados, pero el porqué es «revisar lo vendido de un
  período». La spec no habla del estado. Decisión: Discutir → queda como pregunta abierta para el dueño del
  producto y se hace merge igual (no se cambia código sin spec).
