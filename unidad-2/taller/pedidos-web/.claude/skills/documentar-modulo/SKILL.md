---
name: documentar-modulo
description: "Documenta un módulo o servicio de C# en docs/modulos/<Modulo>.md: propósito, API pública, ejemplo de uso y diagrama Mermaid. Úsala cuando pidan documentar, explicar o dejar escrito un módulo, servicio o clase para el equipo, aunque no nombren la Skill."
---

# Documentar un módulo

## Pasos
1. Identifica la clase principal del módulo (por ejemplo `ClientesService` en `src/PedidosWeb.Core/Clientes/`) y lee su código completo, los tipos que usa y sus tests en `tests/PedidosWeb.Core.Tests/`.
2. Lista sus miembros públicos (constructores y métodos) con lo que reciben, lo que devuelven y qué pasa en los casos borde que muestra el código.
3. Escribe `docs/modulos/<Modulo>.md`, donde `<Modulo>` es el nombre de la clase (por ejemplo `docs/modulos/ClientesService.md`), con las secciones de abajo.
4. Al terminar, muestra la lista de métodos públicos que documentaste y en qué archivo y línea está cada uno, para que una persona compruebe que la documentación coincide con el código.

## Qué debe tener docs/modulos/<Modulo>.md
- Propósito
- API pública
- Ejemplo de uso
- Diagrama Mermaid: un `classDiagram` con la clase, sus miembros públicos y los tipos de los que depende.

## Reglas
- Documenta solo lo que está en el código y sus tests. Si algo no está (un caso, una regla, un motivo), no lo inventes: omítelo o escríbelo como pregunta abierta.
