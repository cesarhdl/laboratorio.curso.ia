#!/usr/bin/env bash
# Guardián (taller U8): hook PreToolUse que frena toda acción de Claude que nombre un .env o un .pem.
# Claude Code le pasa por stdin un JSON con la herramienta y sus argumentos (tool_input).
# Exit 2 = bloquear: Claude Code no ejecuta la acción y le muestra a Claude el mensaje de stderr.

entrada="$(cat)"

# Solo se mira lo que la herramienta va a hacer (tool_input), no las rutas de la sesión.
accion="$(printf '%s' "$entrada" | grep -o '"tool_input".*')"

# .env o .pem como nombre: «.env», «certs/prueba.pem». No calza con «.environment» ni «.envrc».
if printf '%s' "$accion" | grep -qiE '\.(env|pem)([^a-z0-9_]|$)'; then
  echo "Bloqueado por guardian.sh: la acción nombra un .env o un .pem (política de IA de pedidos-web)." >&2
  exit 2
fi

exit 0
