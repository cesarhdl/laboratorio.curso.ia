# Inventario de Pedidos Legado

> Etapa 1 · Entender. Líneas contadas con `wc -l`; responsabilidades deducidas de los nombres de procedimientos, el título de cada formulario y su SQL (no de los comentarios).

## Archivos de `legado/src/`

| Archivo | Tipo | Responsabilidad | Líneas |
|---|---|---|---|
| `UFrmPrincipal.pas` | Formulario | Menú principal: conecta la base, carga parámetros (`TASA_IVA`, `IMPRESORA`) y abre los demás formularios. | 104 |
| `UFrmPrincipal.dfm` | Formulario (diseño) | Ventana «Pedidos - Distribuidora» con el menú. | 62 |
| `UFrmPedidos.pas` | Formulario | Toma de pedidos: busca cliente, agrega y quita líneas, calcula totales, graba el pedido y sus líneas, y descuenta stock. | 420 |
| `UFrmPedidos.dfm` | Formulario (diseño) | Ventana «Toma de pedidos». | 226 |
| `UFrmClientes.pas` | Formulario | Mantención de clientes: busca, crea, graba y elimina; valida el RUT (`RutOk`) y si el cliente tiene pedidos (`TienePedidos`). | 215 |
| `UFrmClientes.dfm` | Formulario (diseño) | Ventana «Clientes», con su propia tabla `tblCli` sobre `CLIENTES`. | 213 |
| `UFrmProductos.pas` | Formulario | Mantención de productos: filtra y valida antes de grabar (`tblProdBeforePost`). | 105 |
| `UFrmProductos.dfm` | Formulario (diseño) | Ventana «Productos», con su propia tabla `tblProd` sobre `PRODUCTOS`. | 98 |
| `UFrmInformes.pas` | Formulario | Informe de ventas: genera la consulta de pedidos por cliente y la exporta. | 137 |
| `UFrmInformes.dfm` | Formulario (diseño) | Ventana «Informe de ventas», con su propia consulta `qryInforme`. | 160 |
| `UDMPrincipal.pas` | Data module | Conecta y desconecta la base, lee parámetros (`LeerParametro`) y ejecuta SQL (`EjecutarSQL`) para el resto del sistema. | 100 |
| `UDMPrincipal.dfm` | Data module (diseño) | Base `dbPedidos` (DBISAM), 5 tablas y 2 consultas compartidas (`qryGeneral`, `qryAux`). | 65 |
| `UCalculoTotales.pas` | Unidad de lógica | Cálculo del pedido: subtotal, descuentos por volumen, flete, IVA y totales; agregar y quitar líneas. | 147 |
| `UStock.pas` | Unidad de lógica | Stock: consulta el disponible, lo descuenta por producto y por pedido, y registra movimientos. | 93 |
| `UUtilidades.pas` | Utilidades | Funciones de apoyo: redondeo y formato de pesos, RUT (limpiar, validar, formatear), fechas, días hábiles y textos. | 189 |
| `UGlobales.pas` | Globales | Variables globales del sistema (usuario, nivel, tasa de IVA, flete base, último folio, ruta de datos) y su inicialización. | 41 |
| `PedidosLegado.dpr` | Proyecto | Programa principal: declara las 10 unidades y crea el data module y el formulario principal. | 24 |

**Total `legado/src/`:** 16 archivos (10 `.pas` y 6 `.dfm`), 2.375 líneas.

## Tablas de la base (`legado/db/esquema-dbisam.sql`)

| Tabla | Para qué sirve | Filas de ejemplo (CSV) | La usan |
|---|---|---|---|
| `REGIONES` | Regiones con su flete base. | 16 | `UFrmPedidos` (lee, `UFrmPedidos.pas:101`) |
| `CLIENTES` | Clientes de la distribuidora. | 30 | `UDMPrincipal`, `UFrmClientes`, `UFrmPedidos`, `UFrmInformes` |
| `PRODUCTOS` | Productos con precio y stock. | 40 | `UDMPrincipal`, `UFrmProductos`, `UFrmPedidos`, `UStock` (lee y actualiza, `UStock.pas:51`) |
| `PEDIDOS` | Encabezado de cada pedido, con su total. | 60 | `UDMPrincipal`, `UFrmPedidos` (inserta y actualiza, `:259`, `:320`), `UFrmClientes`, `UFrmInformes` |
| `PEDIDO_LINEAS` | Líneas de cada pedido. | 169 | `UDMPrincipal`, `UFrmPedidos` (inserta, `:268`), `UStock` |
| `PARAMETROS` | Parámetros del sistema (clave y valor). | 6 | `UDMPrincipal` (`UDMPrincipal.pas:85`) |

## Lo que me llamó la atención

- **El README miente sobre la base.** `legado/README.md:6` dice «Paradox via BDE», pero el código usa DBISAM: `UDMPrincipal.pas:6` (`dbisamtb`) y `UDMPrincipal.dfm:9` (`TDBISAMDatabase`).
- **Una tabla que no existe.** `UStock.pas:85` hace `INSERT INTO MOV_STOCK`, pero `MOV_STOCK` no está en `esquema-dbisam.sql`. Por verificar en la etapa 2 si ese código se ejecuta y qué pasa cuando falla.
- **Acceso a la base repartido.** Además del data module, tres formularios tienen sus propios componentes de base: `UFrmClientes.dfm:200` (`tblCli`), `UFrmProductos.dfm:85` (`tblProd`) y `UFrmInformes.dfm:149` (`qryInforme`). `UFrmInformes` ni siquiera declara `UDMPrincipal` en su `uses`, pero depende de que la base `PEDIDOS` ya esté abierta.
