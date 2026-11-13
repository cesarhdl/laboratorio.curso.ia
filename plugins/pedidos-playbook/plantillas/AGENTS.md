# AGENTS.md · <Nombre del proyecto>

<!--
Plantilla de pedidos-playbook (forma de trabajar de Pedidos Web). Cópiala a la raíz de tu repo
y completa cada <…>. Borra estos comentarios al terminar.
Va como plantilla, no viaja en el plugin: cada equipo decide sus reglas y las versiona en su repo.
Junto a este archivo conviene tener REVIEW.md (reglas de revisión) y .claude/settings.json (permisos).
-->

## Proyecto
<Qué es, para quién y qué sistema reemplaza o complementa. Dos líneas.>

## Comandos
- Compilar: `<comando>`
- Tests: `<comando>` (filtro por área: `<comando>`)
- Ejecutar: `<comando>` → `<url local>`

## Estilo de código
- Nombres de dominio, comentarios y mensajes en <idioma>.
- Dinero: `<tipo exacto, por ejemplo decimal>`; nunca `double` ni `float`.
- Fechas: `<tipo, por ejemplo DateOnly>`.
- <Capa de dominio>: sin dependencias de infraestructura.

## Tests
- <Framework>, en `<carpeta>`, un archivo por clase.
- Nombre: `Metodo_Escenario_Resultado`.
- Ejecuta los tests después de cada cambio y antes de decir que terminaste.

## Seguridad
- Nunca secretos, claves ni cadenas de conexión en el código ni en el repo.
- No leas `.env` ni `*.pem` (lo refuerza el guardián de pedidos-playbook).

## Reglas de trabajo
1. Toda funcionalidad parte de una spec aprobada en `specs/<historia>/spec.md`.
2. Un test por criterio de aceptación.
3. No hacer commit sin tests en verde.
4. Puedes editar `<carpetas de código y tests>` solo después de un plan aprobado.
   Cualquier otro archivo requiere permiso.
5. Nunca hagas commit, push ni merge: lo hace una persona después de leer el diff.
6. <Datos o carpetas que no se tocan, por ejemplo el sistema legado.>
