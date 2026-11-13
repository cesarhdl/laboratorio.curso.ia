# Plan de evolución de la IA para una Business Unit

> Plantilla de la Unidad 9. Complétala con Claude Code en la etapa 1 y revísala tú. En la etapa 3 la conviertes en PDF con tu Skill `pdf-con-estilo`.
> Fuente de datos: `docs/adopcion/analytics-ficticio.csv` (4 equipos × 8 semanas, datos ficticios). No uses cifras que no estén en el CSV.

**BU:** Desarrollo de la distribuidora · **Autor:** Cesar Herrera · **Fecha:** 13-11-2026

## 1. Resumen para la gerencia

<3 a 5 líneas: dónde está hoy la BU, qué propones y qué se instala.>

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

| Equipo | Objetivo medible | Indicador que lo muestra |
|---|---|---|
| | | |

## 4. El playbook

<Qué trae `pedidos-playbook` y para qué sirve cada pieza. Cómo lo instala otro equipo (los comandos están en el README del taller).>

## 5. Hoja de ruta a 6 y 12 meses

| Hito | Mes | Equipo | Champion | Cómo sabemos que se cumplió |
|---|---|---|---|---|
| | | | | |

<El ciclo: estandarizar → medir → ajustar → escalar.>

## 6. Indicadores que vamos a seguir

<Cuáles, cada cuánto y quién los revisa.>

## 7. Riesgos y próximos pasos

- <riesgo> → <cómo lo mitigamos>
