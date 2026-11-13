# Modelo de ROI · piloto de Pedidos Web

> Plantilla de la Unidad 10. Complétala con Claude Code en las etapas 1 y 2 y revísala tú.
> Fuente de datos: `docs/roi/datos-piloto.csv` (equipo Pedidos Web de 6 devs; semanas 1-6 antes, 7-12 con IA; datos ficticios) y tus entregables de las unidades 5 a 9.
> Regla: ningún número sin su fuente o su supuesto. Si un dato no está, escribe FALTA.

**Autor:** Cesar Herrera · **Fecha:** 13-11-2026 · **Horizonte:** 12 meses

## 1. Datos (etapa 1)

«Antes» = promedio de las semanas 1-6. «Con IA» = promedio de las semanas 9-12 (régimen estable; las semanas 7-8
son la curva de adopción y van aparte). Todo sale de `docs/roi/datos-piloto.csv`.

| Dato | Antes (sem. 1-6) | Con IA (sem. 9-12) | Sem. 7-8 | Fuente (archivo y semanas) |
|---|---|---|---|---|
| Despliegues por semana | 2,0 | 3,5 | 2,0 | `datos-piloto.csv`, columna `despliegues`: 12 en sem. 1-6; 14 en sem. 9-12; 4 en sem. 7-8 |
| Lead time (días) | 6,58 | 4,38 | 6,75 | `datos-piloto.csv`, `lead_time_dias`: promedio simple de cada tramo |
| Change fail rate | 41,7 % (5 de 12) | 21,4 % (3 de 14) | 75 % (3 de 4) | `datos-piloto.csv`, `cambios_con_falla` ÷ `despliegues` de cada tramo |
| Tiempo de recuperación (horas por falla) | 6,0 (30 h ÷ 5) | 3,3 (10 h ÷ 3) | 4,7 (14 h ÷ 3) | `datos-piloto.csv`, `horas_recuperacion` ÷ `cambios_con_falla` de cada tramo |
| Defectos escapados por semana | 2,0 | 1,0 | 2,5 | `datos-piloto.csv`, `defectos_escapados`: 12 en sem. 1-6; 4 en sem. 9-12; 5 en sem. 7-8 |
| Horas de retrabajo por semana | 12,0 | 8,75 | 14,5 | `datos-piloto.csv`, `horas_retrabajo`: 72 h en sem. 1-6; 35 h en sem. 9-12; 29 h en sem. 7-8 |
| Horas ahorradas por semana (equipo) | 0 | 11,0 | 3,5 | `datos-piloto.csv`, `horas_ahorradas_equipo`: 44 h en sem. 9-12; 7 h en sem. 7-8. Es lo que **reporta** el equipo, no una medición. |
| Deployment rework rate (DORA) | FALTA | FALTA | FALTA | El CSV no distingue despliegues no planificados por un incidente. |
| Costo de un defecto en producción | FALTA | FALTA | FALTA | Ni el CSV ni los entregables U5-U9 lo miden: va como supuesto estimado en la etapa 2. |

**Evidencia propia** (una línea por archivo: qué dato aporta):

- `docs/migracion/defectos-corregidos.md` (U5): la migración con golden master sacó a la luz 1 defecto corregido (D1, redondeo R05: 13 de los 60 casos cambian en 1 o 2 pesos) y 1 hallazgo de negocio sin corregir (H1: 13 de 60 pedidos grabados con 428.400 pesos menos que lo mostrado en pantalla). Aporta ejemplos de defectos detectados antes de producción; no aporta horas ni costos.
- `docs/calidad/registro-u6.md` (U6): 5 tests de borde para `CalculoTotales`; 4 de ellos detectan errores que los 60 casos del golden master no ven (cada uno falló con su cambio a mano). CI en verde en cada push y PR. Aporta evidencia de calidad de la red de tests; no aporta tiempos.
- `docs/checklist-revision.md` (U7): en el PR #1 (HU-03), el revisor local encontró 3 detalles (2 aceptados y corregidos antes de abrir el PR, 1 descartado); la revisión automática en GitHub no encontró nada más, y la revisión humana aportó 1 hallazgo de negocio que la IA no vio. Aporta evidencia cualitativa de defectos evitados antes del merge.
- `docs/gobernanza/politica-ia.md` (U8): 8 controles de seguridad probados el 2026-11-12 y 1 hueco probado y documentado (`grep -r` lee el `.env`). Aporta el control de riesgos; no aporta beneficio monetario.
- `docs/adopcion/plan-evolucion.md` (U9): Pedidos Web hizo 86 de 107 PR (80 %) con Claude Code, con tasa de aceptación de 0,74 a 0,78 y 5 a 6 usuarios activos por semana (`analytics-ficticio.csv`, sem. 1-8). Confirma que el equipo del piloto usó la herramienta de forma sostenida.

**Lo que veo en las semanas 7 y 8:** empeoran antes de mejorar. Comparadas con «antes», hay más fallas (75 % frente a 41,7 %),
más defectos (2,5 frente a 2,0 por semana) y más retrabajo (14,5 frente a 12 horas), con solo 3,5 horas ahorradas por semana.
Es la curva de adopción: promediar las semanas 7-12 juntas esconde ese costo y diluye el régimen estable.

## 2. Supuestos (etapa 2)

Tipos: **oficial** (publicado, con fecha) · **medido** (dato del piloto) · **estimado** (juicio del equipo, el más débil).

| Supuesto | Valor | Tipo | Fuente |
|---|---|---|---|
| Asiento | USD 25 por dev al mes (pago mensual, conservador) · USD 20 (pago anual, optimista) | Oficial | Plan Team, asiento Standard con Claude Code incluido: claude.com/pricing y support.claude.com «What is the Team plan?», consultados el 13-11-2026 |
| Capacitación | 24 horas por dev | Estimado | Juicio del autor: lo que toma un curso de 10 unidades con talleres y práctica |
| Curva de adopción | 17 horas de equipo: 5 h de retrabajo extra + 4 h de recuperación extra + 1 defecto extra × 8 h | Medido | `datos-piloto.csv`, sem. 7-8 frente al promedio de sem. 1-6: retrabajo 29 h vs 24 h; recuperación 14 h vs 10 h; defectos 5 vs 4 |
| Valor hora | USD 25 por hora de dev (costo para la empresa) | Estimado | Juicio del autor; no hay dato de costo laboral en el repo |
| Horas ahorradas | 8,5 h/semana de equipo (conservador) · 11,0 h/semana (optimista) | Medido (reportado) | `datos-piloto.csv`, `horas_ahorradas_equipo`: promedio de sem. 7-12 (51 h ÷ 6) y de sem. 9-12 (44 h ÷ 4). Lo reporta el equipo; no es una medición de tiempos |
| Defectos evitados | 0,5 por semana (conservador) · 1,0 por semana (optimista), a 8 h cada uno | Medido (cantidad) · Estimado (8 h por defecto) | Cantidad: `datos-piloto.csv`, `defectos_escapados`, 2,0/sem. en sem. 1-6 frente a 1,5 en sem. 7-12 y 1,0 en sem. 9-12. Horas por defecto: juicio del autor (en etapa 1 el costo quedó como FALTA) |
| Horizonte y semanas con beneficio | 12 meses · 42 semanas con beneficio | Estimado | 44 semanas laborales al año (juicio: 52 menos vacaciones y feriados) menos las 2 semanas de curva de adopción |
| Tamaño del equipo | 6 devs | Medido | `datos-piloto.csv` (encabezado de la plantilla: equipo Pedidos Web de 6 devs) |

**Qué no se suma, a propósito:** la baja del retrabajo (12,0 → 8,75 h/semana) y de la recuperación (6,0 → 3,3 h por falla).
Probablemente ya están contadas dentro de las horas que el equipo reporta como ahorradas; sumarlas sería contar dos veces.

## 3. Costos

| Costo | Cálculo | USD |
|---|---|---|
| Asientos | Conservador: 25 × 6 devs × 12 meses · Optimista: 20 × 6 × 12 | 1.800 · 1.440 |
| Capacitación | 24 h × 6 devs × USD 25 | 3.600 |
| Curva de adopción | 17 h × USD 25 | 425 |
| **Total** | Conservador: 1.800 + 3.600 + 425 · Optimista: 1.440 + 3.600 + 425 | **5.825 · 5.465** |

Los asientos son solo el 31 % del costo conservador (1.800 ÷ 5.825): lo que más cuesta es aprender.

## 4. Beneficios y escenarios

ROI = (beneficio − costo) / costo

| | Conservador | Optimista |
|---|---|---|
| Horas ahorradas × valor hora | 8,5 h × 42 sem. × USD 25 = 8.925 | 11,0 h × 42 sem. × USD 25 = 11.550 |
| Defectos evitados × costo en producción | 0,5 × 42 sem. × 8 h × USD 25 = 4.200 | 1,0 × 42 sem. × 8 h × USD 25 = 8.400 |
| **Beneficio total** | **13.125** | **19.950** |
| **ROI** | (13.125 − 5.825) ÷ 5.825 = **125 %** | (19.950 − 5.465) ÷ 5.465 = **265 %** |

**ROI entre 125 % y 265 % a 12 meses.** El conservador es lo mínimo que defendería: incluye las semanas de
la curva en el promedio de horas ahorradas y en el de defectos. El optimista supone que el régimen de las
semanas 9-12 se mantiene todo el año.

## 5. Sensibilidad

**Supuesto que más pesa:** las horas ahorradas. Son el 68 % del beneficio conservador (8.925 ÷ 13.125) y son
el dato más débil: el equipo las reporta, nadie las midió. Le sigue el costo de un defecto (8 h, estimado).

| Si… | Cálculo | ROI |
|---|---|---|
| El ahorro es la mitad del conservador | (4.462,5 + 4.200 − 5.825) ÷ 5.825 | **49 %** |
| Además, no se evita ningún defecto | (4.462,5 − 5.825) ÷ 5.825 | **−23 %** |
| Un defecto cuesta 4 h en vez de 8 (resto conservador) | (8.925 + 2.100 − 5.825) ÷ 5.825 | **89 %** |

Con la mitad del ahorro el ROI sigue positivo; solo se vuelve negativo si además la calidad no mejora.

## 6. Punto de equilibrio (Tu reto)

<Escríbelo tú: qué despejas, el resultado y una línea para la gerencia.>

## 7. Limitaciones

- Los datos son ficticios y la muestra es pequeña: este ROI es un ejercicio de método.
- Las horas ahorradas las reporta el equipo; no hay medición de tiempos. Es el supuesto que más mueve el ROI.
- Solo 4 semanas de régimen estable (9-12): no sabemos si el ahorro se mantiene, crece o cae en el año.
- Un solo equipo, el más maduro de la BU (nivel 4 en la U9). Los otros equipos parten de niveles 1 y 2:
  su curva de adopción probablemente será más larga y su ROI, menor.
- No incluye el tiempo de los champions ni el de mantener el playbook (U9), ni el uso por encima del asiento.
- Faltan dos datos: el *deployment rework rate* de DORA y el costo real de un defecto en producción.
