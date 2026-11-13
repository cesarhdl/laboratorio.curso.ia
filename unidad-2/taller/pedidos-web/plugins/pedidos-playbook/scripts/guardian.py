#!/usr/bin/env python3
"""Guardián de pedidos-playbook: hook PreToolUse que frena toda acción de Claude que nombre un .env o un .pem.

Versión en Python del guardian.sh de la Unidad 8, para que funcione igual en Windows, macOS y Linux.
Claude Code le pasa por stdin un JSON con la herramienta y sus argumentos (tool_input).
Exit 2 = bloquear: Claude Code no ejecuta la acción y le muestra a Claude el mensaje de stderr.
"""
import json
import re
import sys

# .env o .pem como nombre: «.env», «certs/prueba.pem». No calza con «.environment» ni «.envrc».
PATRON = re.compile(r"\.(env|pem)([^a-z0-9_]|$)", re.IGNORECASE)


def main() -> int:
    # En Windows stderr usa cp1252 por defecto; Claude Code lee UTF-8.
    sys.stderr.reconfigure(encoding="utf-8")
    try:
        entrada = json.load(sys.stdin)
    except (json.JSONDecodeError, ValueError):
        return 0  # Sin JSON válido no hay nada que revisar: no se bloquea.

    # Solo se mira lo que la herramienta va a hacer (tool_input), no las rutas de la sesión.
    accion = json.dumps(entrada.get("tool_input", {}), ensure_ascii=False)
    if PATRON.search(accion):
        print(
            "Bloqueado por guardian.py (pedidos-playbook): la acción nombra un .env o un .pem.",
            file=sys.stderr,
        )
        return 2
    return 0


if __name__ == "__main__":
    sys.exit(main())
