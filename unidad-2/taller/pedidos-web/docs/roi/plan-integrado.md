# Plan integrado de adopción de IA · Pedidos Web

> **Recomendación:** extender la forma de trabajar de Pedidos Web a Bodega y Reportes por 6 meses, con Facturacion como piloto de 2 personas desde el mes 3.

> **Decisión antes del 15-12-2026.** ROI estimado entre 125 % y 265 % a 12 meses; 49 % si el ahorro es la mitad.

**Para:** Gerencia general · **Autor:** Cesar Herrera · **Fecha:** 13-11-2026

## Informe ejecutivo

### 1. Problema
Terminó el piloto de 12 semanas en Pedidos Web y hay que decidir si la misma forma de trabajar con IA se extiende a
los otros tres equipos de la BU. Conviene decidir ahora: Bodega ya usa mucho la IA pero acepta poco de lo que produce,
y Facturacion casi no la usa.

### 2. Qué se hizo
El equipo de Pedidos Web (6 personas) trabajó 6 semanas como siempre y 6 semanas con Claude Code, siguiendo reglas
escritas, revisión de cada cambio y controles de seguridad. Comparamos entregas, fallas, defectos y horas.

### 3. Resultados
Entre las semanas 1-6 y las semanas 9-12: el tiempo de entrega bajó de 6,58 a 4,38 días, las entregas con falla de
41,7 % a 21,4 % y los defectos en producción de 2,0 a 1,0 por semana. Las semanas 7 y 8 empeoraron antes de mejorar:
es el costo de aprender. **ROI a 12 meses: entre 125 % y 265 %.** Lo que más lo mueve son las horas ahorradas,
que reporta el equipo y no se midieron: con la mitad del ahorro el ROI baja a 49 %, y sería negativo (−23 %)
solo si además no mejorara la calidad. **Punto de equilibrio:** la inversión se paga si cada persona ahorra
3,24 horas al mes, aun sin contar la mejora de calidad; el piloto reporta 4,96. Datos ficticios y muestra pequeña.

### 4. Recomendación
Extender la forma de trabajar de Pedidos Web a Bodega y Reportes por 6 meses, con Facturacion como piloto de
2 personas desde el mes 3.

### 5. Próximo paso
La gerencia decide antes del 15-12-2026. Si aprueba, Cesar Herrera (Pedidos Web) instala el playbook en Bodega
en el mes 1 junto a su líder técnico, y en el mes 3 se presenta el primer informe de indicadores de los 4 equipos.

*Preparado con apoyo de IA; cifras verificadas por Cesar Herrera.*

## Anexo

No repite los entregables: los cita.

### Roadmap
Mes 1: playbook en Bodega · mes 2: Reportes · mes 3: piloto en Facturacion y primer informe mensual ·
mes 6: revisión de mitad de año · mes 12: los 4 equipos en nivel 3 o más.
Respaldo: `docs/adopcion/plan-evolucion.md` (U9), sección 5.

### Indicadores
Los del piloto (despliegues, tiempo de entrega, entregas con falla, recuperación, defectos y retrabajo) y los de uso
de la U9 (usuarios activos, tasa de aceptación y % de PR con Claude Code), revisados una vez al mes. Cada cambio
pasa por CI con tests de borde y por revisión con reglas escritas antes del merge.
Respaldo: `docs/calidad/registro-u6.md` (U6) · `docs/checklist-revision.md` (U7) · `docs/roi/modelo-roi.md`, sección 1.

### Gobernanza
Se llevan a los otros equipos los controles probados el 12-11-2026: no leer secretos (`.env`, `*.pem`), pedir
aprobación antes de conectarse a internet o de tocar el golden master, el legado en solo lectura y los tests después
de cada cambio. También lo que no cubren (por ejemplo, una búsqueda con `grep -r` puede mostrar un secreto).
Respaldo: `docs/gobernanza/politica-ia.md` (U8).

### ROI
Costo a 12 meses: USD 5.465 a 5.825 (asientos 1.440 a 1.800, capacitación 3.600, curva de adopción 425).
Beneficio: USD 13.125 a 19.950. ROI entre 125 % y 265 %; con el ahorro a la mitad, 49 %; con la mitad del ahorro
y sin mejora de calidad, −23 %. Supuestos clave: valor hora USD 25 y 8 h por defecto (estimados), asiento
USD 20 a 25 (oficial), horas ahorradas reportadas por el equipo (medido).
Respaldo: `docs/roi/modelo-roi.md` · `docs/migracion/defectos-corregidos.md` (U5), defectos detectados antes de producción.

### Punto de equilibrio
ROI = 0 cuando el beneficio iguala el costo conservador (USD 5.825). Despejando las horas ahorradas por dev al mes, con
valor hora USD 25, 6 devs y 12 meses: 3,24 h sin contar defectos evitados y 0,90 h contándolos. El piloto reporta
4,96 h (conservador) a 6,42 h (optimista): el ahorro puede caer un 35 % antes de perder plata.
Respaldo: `docs/roi/modelo-roi.md`, sección 6.

### Evolución
Hoy: Pedidos Web en nivel 4, Bodega y Reportes en nivel 2, Facturacion en nivel 1. Meta: los tres en nivel 3 en
6 a 12 meses y Pedidos Web en nivel 5 (mide y mejora).
Respaldo: `docs/adopcion/plan-evolucion.md` (U9), secciones 2 y 3.

### Checklist final
| Punto | Estado | Cómo se verificó |
|---|---|---|
| `docs/roi/modelo-roi.md` con supuestos, escenarios y punto de equilibrio | ✔ Cumplido | Secciones 2 (Supuestos), 4 (Beneficios y escenarios) y 6 (Punto de equilibrio) del modelo |
| `docs/roi/plan-integrado.md` con informe y anexo | ✔ Cumplido | Informe ejecutivo en 5 franjas (página 2) y Anexo con 6 partes |
| `docs/roi/plan-integrado.pdf` generado con tu Skill | ✔ Cumplido | Generado con `.claude/skills/pdf-con-estilo/` (U9), sin tocar su CSS |
| Nada fuera de `docs/roi/` cambió | ✔ Cumplido | `git diff` desde la base de la Unidad 10: 0 archivos cambiados fuera de `docs/roi/` |
