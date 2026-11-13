# Plan de evolución de la IA para una Business Unit

> Fuente de datos: `docs/adopcion/analytics-ficticio.csv` (4 equipos × 8 semanas, datos ficticios). Todas las cifras del diagnóstico y de las líneas base salen de ese archivo.

**BU:** Desarrollo de la distribuidora · **Autor:** Cesar Herrera · **Fecha:** 13-11-2026

## 1. Resumen para la gerencia

La BU está en tres niveles distintos: Pedidos Web trabaja como equipo AI-native (nivel 4), Bodega y Reportes
usan la IA de forma individual (nivel 2) y Facturación casi no la usa (nivel 1). El problema no es la cantidad
de uso: Bodega es el equipo que más la usa y el que menos código acepta. Proponemos llevar a los tres equipos
al nivel 3 en 6 meses instalando `pedidos-playbook`, la forma de trabajar de Pedidos Web empaquetada como plugin
de Claude Code, y medir cada mes los mismos indicadores para todos.

## 2. Diagnóstico por equipo

Modelo de madurez del curso: 1 Uso a escondidas · 2 Uso individual · 3 Equipo con reglas · 4 Proceso AI-native · 5 Organización que mide y mejora.

| Equipo | Nivel | Evidencia 1 (indicador del CSV) | Evidencia 2 (indicador del CSV) |
|---|---|---|---|
| Pedidos Web | **4 · Proceso AI-native** | **Claude Code está en el flujo de PR:** 86 de 107 PR (80 %) se hicieron con Claude Code, entre 9 y 12 por semana, las 8 semanas. | **Uso alto y de calidad, estable:** 5 a 6 usuarios activos y tasa de aceptación entre 0,74 y 0,78 todas las semanas (17.570 líneas aceptadas, 26,1 por sesión). |
| Facturacion | **1 · Uso a escondidas** | **Uso casi nulo y aislado:** 0 o 1 usuario activo por semana; 15 sesiones en 8 semanas, y en las semanas 2 y 5 no hubo ninguna. | **No llega al trabajo del equipo:** 1 de 71 PR (1 %) con Claude Code; un solo PR en 8 semanas (semana 4). La tasa de aceptación (0,55 a 0,65) es de una sola persona. |
| Bodega | **2 · Uso individual** | **Mucho uso:** 7 usuarios activos las 8 semanas y 921 sesiones, la cifra más alta de la BU. | **Muy poca aceptación:** la tasa de aceptación está entre 0,11 y 0,17 todas las semanas (5,5 líneas aceptadas por sesión, frente a 26,1 en Pedidos Web), y solo 30 de 109 PR (28 %) usan Claude Code. Cada persona prueba por su cuenta, sin reglas que mejoren el resultado. |
| Reportes | **2 · Uso individual** | **Uso intermitente:** en las semanas impares hay 3 o 4 usuarios y entre 38 y 47 sesiones; en las pares, 0 o 1 usuario y entre 0 y 6 sesiones (semana 4 sin sesiones). | **Calidad aceptable cuando se usa, pero no sostenida:** tasa de aceptación entre 0,50 y 0,58; 18 de 49 PR (37 %) con Claude Code, todos en semanas impares (0 PR con Claude Code en las semanas 2, 4, 6 y 8). |

> Ningún indicador solo: mira el uso, la tasa de aceptación y los PR juntos. Una celda vacía en `tasa_aceptacion` significa que esa semana no hubo sesiones.

## 3. Objetivo por equipo para el próximo trimestre

| Equipo | Objetivo medible (nivel meta) | Indicador que lo muestra (línea base del CSV → meta) |
|---|---|---|
| Pedidos Web | Pasar de usar bien a medir y mejorar (nivel 5): publicar cada mes los indicadores de la BU y mantener su calidad. | Tasa de aceptación 0,74–0,78 → ≥ 0,74 todas las semanas; PR con Claude Code 80 % → ≥ 80 %; informe mensual publicado los 3 meses. |
| Facturacion | Que el equipo use la IA a la vista y con reglas (nivel 2 → 3 en el año). | Usuarios activos 0–1 → ≥ 3 por semana; semanas sin sesiones 2 de 8 → 0; PR con Claude Code 1 % → ≥ 20 %. |
| Bodega | Convertir el uso alto en código aceptado (nivel 3). | Tasa de aceptación 0,11–0,17 → ≥ 0,40; PR con Claude Code 28 % → ≥ 50 %, sin bajar de 7 usuarios activos. |
| Reportes | Uso sostenido y no solo algunas semanas (nivel 3). | Semanas sin PR con Claude Code 4 de 8 → 0; PR con Claude Code 37 % → ≥ 60 %; tasa de aceptación ≥ 0,50 todas las semanas. |

## 4. El playbook

`pedidos-playbook` (versión 1.0.0) empaqueta en un plugin de Claude Code lo que Pedidos Web aprendió en 8 unidades:

| Pieza | Para qué sirve |
|---|---|
| Subagente `revisor` | Revisa la rama contra `main` con las reglas de `REVIEW.md` y `AGENTS.md` leídas desde `main`. Solo lectura. |
| Skill `documentar-modulo` | Documenta un módulo con su API pública, un ejemplo y un diagrama, solo con lo que está en el código. |
| Skill `planificador-proyectos-web` | Planifica y documenta un proyecto: specs, épicas, decisiones y reglas para agentes. |
| Guardián | Hook que frena toda acción de Claude que nombre un `.env` o un `.pem`. |
| MCP `claude-academy` | Cursos y material de Claude Academy, de solo lectura. |
| Plantilla de `AGENTS.md` | Las reglas de trabajo, para que cada equipo las copie y ajuste en su repo. |

Lo que **no** viaja en el plugin se entrega como plantilla: `AGENTS.md`, `REVIEW.md` y los permisos de
`.claude/settings.json`, porque cada equipo decide sus reglas. Otro equipo lo instala con:

```text
claude plugin marketplace add cesarhdl/laboratorio.curso.ia
claude plugin install pedidos-playbook@pedidos-cesarhdl
```

## 5. Hoja de ruta a 6 y 12 meses

| Hito | Mes | Equipo | Champion | Cómo sabemos que se cumplió |
|---|---|---|---|---|
| Playbook instalado y `AGENTS.md` + `REVIEW.md` propios | 1 | Bodega | Líder técnico de Bodega, con apoyo de Pedidos Web | El plugin aparece instalado en los 7 usuarios y su repo tiene `AGENTS.md` y `REVIEW.md` en `main`. |
| Playbook instalado y revisor en cada PR | 2 | Reportes | Líder técnico de Reportes | Hay PR con Claude Code todas las semanas del mes. |
| Piloto con 2 personas y política de IA leída | 3 | Facturacion | Líder técnico de Facturacion | ≥ 2 usuarios activos cada semana del mes y primer PR revisado con el revisor. |
| Primer informe mensual de indicadores de la BU | 3 | Pedidos Web | Cesar Herrera (Pedidos Web) | Informe publicado con los 6 indicadores de la sección 6 para los 4 equipos. |
| Revisión de mitad de año | 6 | Toda la BU | Gerencia de Desarrollo | Bodega y Reportes cumplen sus metas de la sección 3; el playbook va en una versión nueva con lo aprendido. |
| BU en nivel 3 o más | 12 | Toda la BU | Gerencia de Desarrollo | Los 4 equipos cumplen su meta y Facturacion llega a nivel 3. |

El ciclo: **estandarizar** (instalar el playbook) → **medir** (indicadores mensuales) → **ajustar** (nueva
versión del playbook) → **escalar** (siguiente equipo).

## 6. Indicadores que vamos a seguir

Los mismos 6 del CSV, para los 4 equipos, **semanales** y revisados **una vez al mes** por los champions con la
gerencia: usuarios activos, sesiones, líneas aceptadas, tasa de aceptación, PR con Claude Code y PR totales.
De ellos se calculan tres señales:

- **% de PR con Claude Code** (PR con Claude Code ÷ PR totales): si la IA llega al trabajo del equipo.
- **Líneas aceptadas por sesión**: si el uso produce código útil (hoy, de 5,5 en Bodega a 26,1 en Pedidos Web).
- **Semanas sin sesiones o sin PR con Claude Code**: si el uso es sostenido.

Ningún indicador se lee solo: mucho uso con poca aceptación es una señal de alerta, no un éxito.

## 7. Riesgos y próximos pasos

- **Medir solo el uso premia lo equivocado** (Bodega tiene el mayor uso y la menor aceptación) → las metas
  combinan uso, aceptación y PR, y se revisan juntas.
- **Uso a escondidas en Facturacion**, sin reglas ni controles sobre los datos → piloto pequeño con la política
  de IA de Pedidos Web (`docs/gobernanza/politica-ia.md`) y el guardián instalado desde el primer día.
- **El plugin falla en el equipo de otro** (nos pasó: rutas de más de 260 caracteres en Windows y un hook que
  bloqueaba todo en PowerShell) → cada versión se prueba instalándola desde GitHub en una carpeta limpia
  antes de anunciarla, y se publica con un número de versión nuevo.
- **El uso depende de pocas personas** (Reportes alterna semanas con y sin uso) → un champion por equipo y
  metas de uso sostenido, no solo de promedio.
- **Los datos son de 8 semanas y ficticios** → antes del mes 1, repetir el diagnóstico con el análisis real de
  uso del equipo (Claude Code analytics) y ajustar las líneas base.

**Próximo paso:** aprobar el plan y elegir los champions de Bodega, Reportes y Facturacion.
