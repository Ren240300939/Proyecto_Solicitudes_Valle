# Catálogo de elementos de escritorio

La aplicación asigna los siguientes nombres e identificadores. Windows UI Automation los expone en el equipo de desarrollo; confirma su representación en el motor de captura de PAD de tu equipo. Los selectores completos se capturan localmente, no se proporcionan como cadenas importables inventadas.

| Nombre accesible | Identificador del control | Tipo | Ámbito |
|---|---|---|---|
| Registro de solicitudes | frmSolicitudes | Ventana | Principal |
| Escenario de prueba | cmbEscenario | Combo | Principal |
| Identificador de solicitud | txtIdSolicitud | Texto editable | Principal |
| Solicitante | txtSolicitante | Texto editable | Principal |
| Área | cmbArea | Combo | Principal |
| Tipo de solicitud | cmbTipo | Combo | Principal |
| Descripción | txtDescripcion | Texto editable | Principal |
| Registrar solicitud | btnRegistrar | Botón | Principal |
| Nueva solicitud | btnNueva | Botón | Principal |
| Último folio | txtFolio | Texto de solo lectura | Principal |
| Estado del registro | lblEstado | Etiqueta | Principal |
| Total de solicitudes | lblTotal | Etiqueta | Principal |
| Confirmar registro | dlgConfirmacion | Ventana modal | Confirmación |
| Confirmar | btnAceptarDialogo | Botón | Confirmación |
| Cancelar | btnCancelarDialogo | Botón | Confirmación |
| Registro completado | dlgResultado | Ventana modal | Éxito |
| Folio generado | txtFolioDialogo | Texto de solo lectura | Éxito o duplicado |
| Aceptar | btnAceptarDialogo | Botón | Resultado |

Las ventanas de error también utilizan `dlgResultado`: diferencia **Registro completado**, **Datos inválidos**, **Solicitud duplicada** y **Error de almacenamiento** por el título o nombre accesible capturado. El ID del botón Aceptar por sí solo no identifica cuál diálogo estás cerrando.

## Ejercicio de selección

Compara tres estrategias para el botón Registrar: ruta completa con contenedores; ruta simplificada con atributos estables dentro de la ventana; selección acotada por ventana principal y nombre/tipo del control. Anota qué atributos realmente aparecen en PAD, prueba unicidad y justifica cuáles conservas. No supongas que la ruta más corta es siempre la correcta.

En la reorganización se intercambian identificador/solicitante y área/tipo. Los nombres e IDs se conservan. Los paneles sin nombre pueden presentar IDs numéricos que cambian entre ejecuciones: evítalos cuando no sean necesarios para distinguir el elemento. El resultado esperado es encontrar el mismo control por su identidad aunque cambie su posición.

Para cada selector entrega: nombre del elemento, atributos conservados, atributos descartados, razón, resultado antes/después del reinicio y resultado en las tres variantes. Un identificador estable no demuestra por sí solo que el flujo completo funcione.
