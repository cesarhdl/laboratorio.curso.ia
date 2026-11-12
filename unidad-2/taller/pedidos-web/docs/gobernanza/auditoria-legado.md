# Auditoría del legado · pedidos-web

> Etapa 1 de la U8 · Auditar sin creer. El subagente `analista-legado` propuso los hallazgos de
> `legado/src/UFrmPedidos.pas` y `legado/src/UDMPrincipal.pas` (2026-11-12). Cada archivo:línea se abrió
> y se comprobó a mano antes de marcarlo. Rutas relativas a `legado/`.

## Hallazgos

| ID | Hallazgo | Evidencia (archivo:línea) | Tipo | Severidad | Estado | Argumento |
|---|---|---|---|---|---|---|
| AU-01 | Inyección SQL por la observación del pedido: se concatena en el INSERT sin escapar. | `src/UFrmPedidos.pas:256` `vObs := memObs.Lines.Text;` · `src/UFrmPedidos.pas:262` `''', ''C'', ''' + vObs + ''', 0)';` | Seguridad | Alta | Validado | Solo se recorta a 200 caracteres (`:257-258`). `EscapaComillas` está declarada (`src/UUtilidades.pas:19,164`) pero no se llama en ninguna parte. |
| AU-02 | Inyección SQL por el código de producto. | `src/UFrmPedidos.pas:152` `'FROM PRODUCTOS WHERE CODIGO = ''' + Trim(edtCodProducto.Text) + '''';` | Seguridad | Alta | Validado | El texto del control va directo a la consulta; `Trim` no escapa comillas. |
| AU-03 | Inyección SQL por el nombre de usuario: viene de un `InputBox` libre y se concatena al grabar el pedido y el movimiento de stock. | `src/UFrmPrincipal.pas:46` `gUsuario := UpperCase(Trim(InputBox('Pedidos', 'Usuario:', '')));` · `src/UFrmPedidos.pas:261` `... + ', ''' + gUsuario +` · `src/UStock.pas:87` | Seguridad | Alta | Validado | `UpperCase(Trim(...))` no quita comillas; el valor llega tal cual a los dos INSERT. |
| AU-04 | Sin autenticación: escribir ADMIN o JEFEVENTAS da nivel 9 sin contraseña. | `src/UFrmPrincipal.pas:49` `if (gUsuario = 'ADMIN') or (gUsuario = 'JEFEVENTAS') then` | Seguridad | Alta | Validado | El nivel se decide solo por el texto escrito en `:46`; no hay contraseña ni consulta a una tabla de usuarios. |
| AU-05 | Clave de la base de datos escrita en el código fuente. | `src/UDMPrincipal.pas:41` `CLAVE_BD = 'Distrib#2008';` | Seguridad | Alta | Validado | La constante está en el fuente; quien lea el repo tiene la clave. |
| AU-06 | `EjecutarSQL` es un ejecutor genérico de SQL en texto, sin parámetros. | `src/UDMPrincipal.pas:95` `qryGeneral.SQL.Text := ASQL;` | Seguridad | Media | Descartado | La línea solo asigna el texto que recibe; no es una vulnerabilidad por sí misma. El riesgo está en quien le pasa texto del usuario, y eso ya lo cubren AU-01, AU-02 y AU-03. Es la causa raíz, no un hallazgo aparte. |
| AU-07 | `LeerParametro` concatena la clave del parámetro en el SQL. | `src/UDMPrincipal.pas:85` `'SELECT VALOR FROM PARAMETROS WHERE CLAVE = ''' + AClave + ''''` | Seguridad | Baja | Descartado | Sus únicos llamadores pasan constantes: `src/UFrmPrincipal.pas:62` `LeerParametro('TASA_IVA')` y `:65` `LeerParametro('IMPRESORA')`. Ningún dato del usuario llega a `AClave`. |
| AU-08 | El bloqueo por deuda vencida se revisa solo al buscar el cliente, no al grabar; y no se revisa si el cliente está activo. | `src/UFrmPedidos.pas:110-112` (revisión de deuda en la búsqueda) · `src/UFrmPedidos.pas:204` `if FIdCliente = 0 then` (única revisión del cliente al grabar) · `src/UFrmPedidos.pas:98-101` (el SELECT no trae `ACTIVO`) | Regla de negocio | Media | Validado | Evidencia corregida: el subagente citó solo `:204`; la regla está en `:110-112`. Entre buscar y grabar no se vuelve a comprobar, y a un cliente inactivo se le puede vender. |
| AU-09 | Si no hay stock, el pedido queda grabado igual. | `src/UStock.pas:69-72` `if not DescontarStock(...) then ShowMessage('Sin stock suficiente ...` · `src/UStock.pas:43` `if vNuevo < -20 then` | Regla de negocio | Alta | Validado | El descuento de stock ocurre después de insertar el pedido y sus líneas; la falla solo muestra un mensaje. Un VIP puede dejar el stock en -20. |
| AU-10 | Las reglas del pedido (mínimo $30.000 fuera de la RM, máximo 40 líneas, línea con precio) viven solo en el botón Grabar. | `src/UFrmPedidos.pas:204-235` · `src/UFrmPedidos.pas:231` `if (vSubtotal < 30000) and (FRegion <> 13) then` | Regla de negocio | Media | Validado | No hay validación en la capa de datos. Quien escriba directo en la base (posible con la clave de AU-05) se salta las tres reglas. |
| AU-11 | La grabación del pedido no usa transacción: cabecera con TOTAL 0, líneas, UPDATE del total y stock en pasos separados. | `src/UFrmPedidos.pas:255` `// TODO transaccion` (ubicación) | Integridad | Alta | Validado | El comentario no es evidencia; lo es que `grep -niE "StartTransaction\|Commit\|Rollback" legado/src/*.pas` da 0 resultados. Un fallo intermedio deja pedidos con total 0 o sin líneas. |
| AU-12 | Folio calculado como MAX+1 sin bloqueo: dos puestos pueden obtener el mismo folio. | `src/UFrmPedidos.pas:251` `vFolio := FieldByName('ULTIMO').AsInteger + 1;` | Integridad | Alta | Validado | Se lee `MAX(FOLIO)` y se suma 1 sin bloqueo ni transacción (ver AU-11). |

**Resultado:** 10 Validados y 2 Descartados.

## Cruce con el diagnóstico de la U4

- **Coinciden:** AU-01, AU-02 → RS-01 · AU-04, AU-05 → RS-08 · AU-08 → RS-14, RS-15 · AU-09 → RS-09 · AU-11 → RS-02 · AU-12 → RS-03.
- **Nuevos en esta auditoría:** AU-03 (inyección por el nombre de usuario), AU-08 (la deuda se revisa al buscar y no al grabar) y AU-10 (las reglas del pedido viven solo en la interfaz).
- **De la U4 que el subagente no vio:** RS-05 (dinero en `Double` y `FloatToStr` en el SQL, `src/UFrmPedidos.pas:199,271`), RS-06 (`>=` frente a `>` en el descuento, `src/UFrmPedidos.pas:298`) y RS-07 (RUT sin normalizar en la búsqueda, `src/UFrmPedidos.pas:102`). Los tres están en los archivos auditados: una sola pasada de un agente no basta.
