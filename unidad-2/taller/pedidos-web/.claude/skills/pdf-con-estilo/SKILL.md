---
name: pdf-con-estilo
description: "Convierte un documento Markdown (plan, política, informe, spec, auditoría) en un PDF con portada, títulos y tablas con diseño, usando la plantilla HTML de la Skill y Playwright. Úsala cuando pidan un PDF, exportar, imprimir o entregar un documento .md a la gerencia, a otro equipo o a un cliente, aunque no nombren la Skill."
---

# PDF con estilo

Separa el **contenido** (el Markdown) del **diseño** (`assets/plantilla.html`). Para otro documento solo
cambia el Markdown: la plantilla no se toca.

## Pasos
1. Confirma que el Markdown existe y empieza con un título `# ...`. Ese título va en la portada.
   Si justo después del título hay una cita (`> ...`) o una línea en negrita (`**BU:** ...`), también van
   a la portada como subtítulo y datos del documento.
2. Ejecuta el script desde la raíz del proyecto (requiere Python 3 con `playwright` y `markdown`, y Chromium
   instalado con `python -m playwright install chromium`):

   ```bash
   python3 .claude/skills/pdf-con-estilo/scripts/html_a_pdf.py <entrada.md> <salida.pdf>
   ```

   En Windows, si `python3` no existe, usa `python`.
3. Abre el PDF o revisa la salida del script: debe decir cuántas páginas generó.
4. Informa la ruta del PDF y su número de páginas.

## Reglas
- No edites `assets/plantilla.html` para que un documento se vea bien: si hace falta, el problema es del
  Markdown o de la plantilla para todos los documentos, y se corrige como una mejora de la Skill.
- No cambies el contenido del Markdown al convertirlo.
- El PDF es un entregable generado: el Markdown sigue siendo la fuente.
