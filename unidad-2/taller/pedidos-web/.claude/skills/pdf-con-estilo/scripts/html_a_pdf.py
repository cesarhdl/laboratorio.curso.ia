#!/usr/bin/env python3
"""Convierte un Markdown en PDF con la plantilla de pdf-con-estilo (Playwright + Chromium).

Uso:
    python3 html_a_pdf.py <entrada.md> <salida.pdf> [--etiqueta "TEXTO DE PORTADA"]

La portada se arma con:
- el primer título «# ...» → título;
- las citas «> ...» que vienen justo después → subtítulo;
- las líneas en negrita «**Campo:** valor» que vienen justo después → datos del documento.
El resto del Markdown es el contenido. La plantilla está en ../assets/plantilla.html.
"""
import argparse
import html
import re
import sys
from pathlib import Path

import markdown
from playwright.sync_api import sync_playwright

RAIZ_SKILL = Path(__file__).resolve().parent.parent
PLANTILLA = RAIZ_SKILL / "assets" / "plantilla.html"
EXTENSIONES = ["tables", "fenced_code", "sane_lists"]

PIE = (
    '<div style="width:100%; font-size:7.5pt; color:#5a6878; padding:0 18mm; '
    'font-family:Segoe UI, Arial, sans-serif; display:flex; justify-content:space-between;">'
    '<span>{titulo}</span><span><span class="pageNumber"></span> / <span class="totalPages"></span></span>'
    "</div>"
)


def separar_portada(texto: str) -> tuple[str, str, str, str]:
    """Devuelve (titulo, subtitulo_md, meta_md, cuerpo_md)."""
    lineas = texto.splitlines()
    i = 0
    while i < len(lineas) and not lineas[i].strip():
        i += 1
    if i >= len(lineas) or not lineas[i].startswith("# "):
        sys.exit("El Markdown debe empezar con un título «# ...»: es el título de la portada.")
    titulo = lineas[i][2:].strip()
    i += 1

    subtitulo, meta = [], []
    while i < len(lineas):
        linea = lineas[i].strip()
        if not linea:
            i += 1
        elif linea.startswith(">"):
            subtitulo.append(linea.lstrip("> ").rstrip())
            i += 1
        elif linea.startswith("**"):
            meta.append(linea)
            i += 1
        else:
            break
    return titulo, "\n\n".join(subtitulo), "\n\n".join(meta), "\n".join(lineas[i:])


def a_html(md_texto: str) -> str:
    return markdown.markdown(md_texto, extensions=EXTENSIONES) if md_texto.strip() else ""


def main() -> int:
    sys.stdout.reconfigure(encoding="utf-8")  # en Windows la consola usa cp1252 por defecto
    parser = argparse.ArgumentParser(description="Markdown → PDF con la plantilla de pdf-con-estilo.")
    parser.add_argument("entrada", type=Path)
    parser.add_argument("salida", type=Path)
    parser.add_argument("--etiqueta", default="Pedidos Web · Desarrollando con IA")
    args = parser.parse_args()

    if not args.entrada.is_file():
        sys.exit(f"No existe el archivo de entrada: {args.entrada}")

    titulo, subtitulo_md, meta_md, cuerpo_md = separar_portada(args.entrada.read_text(encoding="utf-8"))
    pagina = (
        PLANTILLA.read_text(encoding="utf-8")
        .replace("{{etiqueta}}", html.escape(args.etiqueta))
        .replace("{{titulo}}", html.escape(titulo))
        .replace("{{subtitulo}}", a_html(subtitulo_md))
        .replace("{{meta}}", a_html(meta_md))
        .replace("{{contenido}}", a_html(cuerpo_md))
    )

    args.salida.parent.mkdir(parents=True, exist_ok=True)
    with sync_playwright() as p:
        navegador = p.chromium.launch()
        hoja = navegador.new_page()
        hoja.set_content(pagina, wait_until="load")
        hoja.pdf(
            path=str(args.salida),
            format="A4",
            print_background=True,  # sin esto, la portada y los encabezados de tabla salen sin color
            prefer_css_page_size=True,
            display_header_footer=True,
            header_template="<span></span>",
            footer_template=PIE.format(titulo=html.escape(titulo)),
        )
        navegador.close()

    paginas = len(re.findall(rb"/Type\s*/Page[^s]", args.salida.read_bytes()))
    print(f"PDF generado: {args.salida} ({paginas} páginas)")
    return 0


if __name__ == "__main__":
    sys.exit(main())
