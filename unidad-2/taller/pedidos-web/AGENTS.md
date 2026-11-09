# AGENTS.md · Pedidos Web

## Proyecto
Portal web de pedidos de una distribuidora; moderniza un sistema de escritorio en Delphi + DBISAM.
API REST en ASP.NET Core (.NET 10, Minimal API) con 45 pedidos de ejemplo en memoria; sin base de datos.

## Comandos
- Compilar: `dotnet build`
- Tests: `dotnet test` (solo pedidos: `dotnet test --filter "FullyQualifiedName~Tests.Pedidos"`)
- Ejecutar la API: `dotnet run --project src/PedidosWeb.Api` → `http://localhost:5080` (salud: `/salud`)

## Estilo de código
- Nombres de dominio, comentarios y mensajes en español de Chile (`Pedido`, `ObtenerPorId`).
- Dinero en CLP: `decimal` sin decimales; nunca `double` ni `float`.
- Fechas de pedido: `DateOnly` (sin hora).
- `src/PedidosWeb.Core/`: lógica de dominio, sin dependencias de infraestructura ni de ASP.NET.
- `src/PedidosWeb.Api/`: solo traduce HTTP ↔ Core (endpoints en `Program.cs`).

## Tests
- xUnit, en `tests/PedidosWeb.Core.Tests/`, una carpeta por área y un archivo por clase (`<Clase>Tests.cs`).
- Nombre: `Metodo_Escenario_Resultado` (ej.: `ObtenerPorId_IdInexistente_DevuelveNull`).
- La lógica se prueba en Core, no a través del endpoint.
- Ejecuta `dotnet test` después de cada cambio y antes de decir que terminaste.

## Seguridad
- Nunca secretos, claves ni cadenas de conexión en el código ni en el repo.

## Reglas de trabajo
1. Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
2. Un test xUnit por criterio de aceptación.
3. No hacer commit sin tests en verde.
4. Puedes editar `src/` y `tests/` sin pedir permiso, solo después de un plan aprobado.
   Cualquier otro archivo (`specs/`, `docs/`, `AGENTS.md`, `.sln`, `.csproj`) requiere mi permiso.
5. Nunca hagas commit, push ni merge: lo hace una persona después de leer el diff.
6. No modifiques los 45 pedidos de ejemplo de `PedidoRepositorioEnMemoria`.
