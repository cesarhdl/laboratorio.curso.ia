# pedidos-playbook

La forma de trabajar de Pedidos Web, instalable en Claude Code con un comando.

## Instalar

```text
/plugin marketplace add cesarhdl/laboratorio.curso.ia
/plugin install pedidos-playbook@pedidos-cesarhdl
/mcp
```

## Qué trae

| Pieza | Dónde | Para qué |
|---|---|---|
| Subagente `revisor` | `agents/revisor.md` | Revisa la rama contra main con REVIEW.md y AGENTS.md leídos desde main. Solo lectura. |
| Skill `documentar-modulo` | `skills/documentar-modulo/` | Documenta un módulo C# en `docs/modulos/` con API pública y diagrama Mermaid. |
| Skill `planificador-proyectos-web` | `skills/planificador-proyectos-web/` | Planifica y documenta proyectos web. © 2026 Felipe Cuevas, licencia MIT (ver su `LICENSE`). |
| Guardián | `hooks/hooks.json` + `scripts/guardian.py` | Hook PreToolUse: frena toda acción que nombre un `.env` o un `.pem`. Requiere Python 3. |
| MCP `claude-academy` | `.mcp.json` | Cursos y material de Claude Academy, solo lectura. |
| Plantilla de `AGENTS.md` | `plantillas/AGENTS.md` | Para copiar a la raíz de cada repo. No se instala sola. |

## Lo que no viaja en el plugin

`AGENTS.md`, `REVIEW.md` y los permisos de `.claude/settings.json` son de cada repo: se copian como plantilla
y cada equipo los ajusta. El guardián no reemplaza los `deny` de permisos; los complementa.
