# Instrucciones de revisión · pedidos-web

Estas reglas valen para toda revisión, de la IA o de una persona. Se leen desde `main`.

## Qué es Importante aquí
- Lo que cambia un monto que ve o paga el cliente (subtotal, descuento, flete, IVA, total)
  o se aleja del golden master sin una decisión escrita en docs/migracion/defectos-corregidos.md.
- Un resultado distinto de lo que dice la spec: un criterio de aceptación sin cumplir,
  un filtro que deja fuera (o deja entrar) un pedido, un error con otro código o mensaje.
- Lógica de negocio en PedidosWeb.Api en vez de PedidosWeb.Core.
- Secretos, claves o tokens escritos en el repo.

Estilo, nombres y patrones de diseño son Detalle como máximo.

## Tope de detalles
Máximo 3 detalles por revisión. Los Importantes no tienen tope.

## No reportar
- Lo que ya valida el CI: compilación y tests en verde.
- legado/ (solo lectura), migracion/golden-master.csv y los 45 pedidos de ejemplo.
- Redacción de docs/ y specs/, salvo que contradiga el código.
- Lo que está en «Fuera de alcance» de la spec.

## Revisar siempre
1. Dinero en `decimal` y fechas de pedido en `DateOnly`; nunca `double`, `float` ni `DateTime`.
2. Cada criterio de aceptación de la spec tiene su test, con nombre `Metodo_Escenario_Resultado`
   y un valor esperado concreto (no calculado con el mismo código que prueba).
3. Los límites se tratan como dice la spec: extremos inclusivos o exclusivos, rango invertido,
   lista vacía.

## Evidencia
Cada hallazgo cita archivo:línea y, si aplica, el criterio de la spec (ej.: HU-03 CA-02).
Si no hay nada Importante, se dice en una línea: no se inventan hallazgos.
