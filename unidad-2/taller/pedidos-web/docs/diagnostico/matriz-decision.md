# Matriz de decisión de Pedidos Legado

> Etapa 3 · Decidir. Claude propuso pesos y puntajes con su justificación; los aprobó el responsable del diagnóstico.
> Evidencia en `inventario.md`, `dependencias.md`, `reglas-negocio.md` y `riesgos.md`.

## Pesos

| Criterio | Qué mide | Peso |
|---|---|---|
| Costo | Cuánto cuesta en horas y dinero | 20 % |
| Complejidad | Qué tan difícil es técnicamente | 15 % |
| Riesgo | Qué puede salir mal en el camino | 30 % |
| Impacto | Cuánto valor entrega al negocio | 25 % |
| Personas | Conocimiento disponible en el equipo | 10 % |
| **Total** |  | **100 %** |

**Por qué:** el sistema maneja dinero y el diagnóstico encontró 9 riesgos de prioridad Alta, por eso Riesgo pesa más. Impacto responde a «por dónde empezar». Complejidad pesa menos porque ya está en parte dentro de Costo y Riesgo. Personas pesa poco porque el equipo ya trabaja en .NET.

## Estrategias

- **Mantener:** dejarlo como está en Delphi.
- **Refactorizar:** ordenar el código Delphi sin cambiar lo que hace.
- **Modernizar:** migrarlo a .NET (`pedidos-web`) conservando sus reglas, con tests.
- **Reemplazar:** sustituirlo por algo nuevo (desarrollo desde cero o producto comprado).

## Matriz

Puntaje ponderado = Σ (puntaje 1–5 × peso). 5 = la mejor opción para ese criterio (en Costo, lo más barato; en Riesgo, lo menos riesgoso).

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar | Recomendación |
|---|---|---|---|---|---|
| 1 · Cálculo de totales (`UCalculoTotales` + recálculo en `UFrmPedidos`) | 2,50 | 2,65 | **4,35** | 2,70 | **Modernizar** · empezar aquí |
| 2 · Toma de pedidos (`UFrmPedidos`) | 2,50 | 2,20 | **3,35** | 2,60 | Modernizar |
| 3 · Clientes y RUT (`UFrmClientes`, RUT de `UUtilidades`) | 2,80 | 3,00 | **3,75** | 2,90 | Modernizar |
| 4 · Stock (`UStock`) | 2,50 | 3,25 | **3,45** | 2,90 | Modernizar |
| 5 · Informes (`UFrmInformes`) | 3,35 | 3,30 | 3,65 | **3,70** | Reemplazar (empate técnico con Modernizar: 0,05) |
| 6 · Acceso a datos y globales (`UDMPrincipal`, `UGlobales`) | 2,50 | 2,20 | 3,00 | **3,25** | Reemplazar (DBISAM por una base moderna) |

**Orden sugerido por puntaje:** Cálculo de totales (4,35) → Clientes y RUT (3,75) → Informes (3,70) → Stock (3,45) → Toma de pedidos (3,35) → Acceso a datos (3,25).

### Detalle de puntajes

Orden de cada celda: Costo · Complejidad · Riesgo · Impacto · Personas.

| Módulo | Mantener | Refactorizar | Modernizar | Reemplazar |
|---|---|---|---|---|
| 1 · Cálculo de totales | 5·5·1·1·2 | 3·3·3·2·2 | 4·4·4·5·5 | 2·2·2·4·4 |
| 2 · Toma de pedidos | 5·5·1·1·2 | 3·2·2·2·2 | 2·2·3·5·5 | 1·1·2·5·4 |
| 3 · Clientes y RUT | 5·5·2·1·2 | 4·4·3·2·2 | 3·3·4·4·5 | 2·3·3·3·4 |
| 4 · Stock | 5·5·1·1·2 | 4·4·3·3·2 | 3·3·3·4·5 | 2·2·3·4·3 |
| 5 · Informes | 5·5·3·2·2 | 4·4·4·2·2 | 3·4·4·3·5 | 3·4·4·4·3 |
| 6 · Acceso a datos y globales | 5·5·1·1·2 | 3·2·2·2·2 | 2·2·3·4·4 | 2·2·3·5·4 |

### Justificación por módulo

1. **Cálculo de totales:** lógica pura, sin formularios ni base (`dependencias.md`), así que migrarla es barato y se puede probar con tests. Mantenerla deja vivo que el total mostrado no sea el grabado (regla 2, RS-06). Es el dinero de cada pedido: el mayor impacto.
2. **Toma de pedidos:** 420 líneas que mezclan pantalla, SQL y grabación; caro y riesgoso de tocar sin tests. Mantenerlo deja la inyección SQL, el folio duplicado y los pedidos a medias (RS-01 a RS-03).
3. **Clientes y RUT:** ya existe un `ClientesService` ordenado en .NET (U3); la validación de RUT duplicada e invertida (regla 16, RS-07) se corrige mejor con tests nuevos.
4. **Stock:** cerca de Refactorizar (3,25 vs 3,45). Lo grave (`MOV_STOCK` inexistente, sin bloqueo, venta sin stock: RS-04, RS-09) depende de la base y se resuelve de fondo junto con el módulo 6.
5. **Informes:** bajo impacto y hoy funciona. Reemplazar gana a Modernizar por 0,05: es un empate técnico y la decisión puede ir para cualquiera de los dos lados.
6. **Acceso a datos y globales:** DBISAM, la clave en el código, las rutas fijas y las globales que usan 7 de 9 unidades (RS-08, RS-16). Reemplazar la base es lo que más valor entrega y prepara el resto de la migración.

## Resumen ejecutivo

1. **Qué es y su estado:** Pedidos Legado es el sistema de escritorio que toma los pedidos hace 18 años; funciona, pero usa una base de datos antigua y su documentación no coincide con lo que realmente hace.
2. **Hallazgo principal (con evidencia):** el total que ve el vendedor puede no ser el que queda grabado, porque el descuento se calcula dos veces con reglas distintas (`UCalculoTotales.pas:79` y `UFrmPedidos.pas:298`) y el aviso que lo detectaría está apagado.
3. **Riesgo más urgente:** un pedido puede quedar grabado a medias, dos vendedores pueden recibir el mismo número de pedido y un texto escrito en pantalla puede alterar la base de datos.
4. **Estrategia y por dónde empezar:** modernizar por partes, empezando por el cálculo de totales (mejor puntaje de la matriz: 4,35 de 5), llevándolo al sistema nuevo con pruebas automáticas que fijen cada regla.
5. **Próximo paso que se pide aprobar:** que el equipo migre el cálculo de totales y que el área comercial defina cuál de los dos cálculos de descuento es el correcto.
