# Proyecto de registro automatizado de solicitudes

Guía del estudiante | Power Automate Desktop | Sesión de 3 horas | Revisión 30/09/2026

## Del primer control al proceso completo

En las prácticas introductorias solicitaste datos, utilizaste una condición, guardaste un archivo y rellenaste el campo Solicitante. Ahora conectarás esas capacidades para registrar solicitudes en una aplicación de escritorio y conservar sus folios.

Una oficina recibe solicitudes en un archivo CSV. Su sistema administrativo se opera mediante formularios. Construirás un bot que lea las entradas, capture los datos, confirme el registro y genere una bitácora verificable.

**Producto mínimo:** una solicitud registrada por el bot, con el folio leído de la interfaz y guardado en un archivo. **Producto completo:** tres solicitudes leídas del CSV y registradas sin intervención durante el recorrido, con una fila de resultados por solicitud.

## Recorrido y tiempo de trabajo

| Actividad | Minutos | Punto de comprobación |
|---|---|---|
| Contexto y preparación | 15 | Aplicación abierta y datos ficticios identificados |
| Etapa 1: completar el formulario | 40 | Cinco campos correctos y confirmación visible |
| Etapa 2: recuperar el folio | 40 | Folio real verificado y guardado |
| Etapa 3: procesar tres filas | 60 | Tres solicitudes y tres resultados |
| Pruebas, entrega y cierre | 25 | Correspondencia entre entradas y folios |

## Antes de iniciar

1. Extrae completamente el ZIP en una carpeta con permiso de escritura. No ejecutes la aplicación desde el archivo comprimido.
2. Abre `aplicacion/iniciar.cmd`. Deja el escenario **Base** y una sola instancia abierta. Si el archivo de inicio no abre, ejecuta `aplicacion/bin/SolicitudesRpa.exe`.
3. Abre PAD, inicia sesión localmente y crea el flujo `Proyecto_RegistroSolicitudes`. Reutiliza los elementos de tu puente si funcionan; también puedes capturarlos de nuevo.
4. Ejecuta PAD y la aplicación con el mismo usuario y nivel de privilegios, preferiblemente sin elevar. Se requiere Windows; no hace falta Excel ni una API para este proyecto.
5. Identifica `datos/solicitudes_base.csv`, `SELECTORES.md` y `PLANTILLA_ENTREGA.md`. La carpeta `evidencias` recibirá tus archivos de resultados.

Las guías describen acciones de PAD; no son un flujo importable. Captura los controles en tu equipo. Los ejemplos usan variables clásicas como `%IdSolicitud%`; insértalas con el selector del diseñador. Las traducciones de las acciones pueden variar.

Trabajaremos en el escenario Base. Las variantes de interfaz son una ampliación opcional. El manejo avanzado de excepciones y los reintentos se abordarán después. Si aparece un resultado inesperado, detén el ensayo y conserva la evidencia antes de repetir.

---

# Etapa 1  Variables y elementos UI

40 minutos para toda la etapa 1 | Instrucciones ampliadas del 30 de septiembre de 2026

## Tres conceptos que debes distinguir

Una **variable** almacena un dato, como el nombre de una persona. Un **elemento UI** identifica un control de la aplicación, como el recuadro Solicitante. Una **acción** utiliza el dato y el control: «rellena este recuadro con esta variable».

Capturar un elemento UI no agrega una acción al flujo ni escribe nada. Establecer una variable tampoco rellena el formulario. Primero prepara los datos y los controles; después construye las acciones que los conectan.

## A01 a A05  Establecer las cinco variables

Recuerda cómo se construye esta acción: busca **Establecer variable** en Acciones, arrástrala al área central y abre su configuración. En Variable escribe el nombre sin porcentajes; en Valor escribe el texto literal sin comillas. Pulsa Guardar. Repite una vez por fila, en este orden:

| Variable | Valor |
|---|---|
| IdSolicitud | SOL-500 |
| Solicitante | Persona ficticia Ana |
| Area | Administración |
| Tipo | Equipo |
| Descripcion | Revisar pantalla del equipo de práctica. |

Estas son **cinco acciones distintas**. Cuando uses una variable en otra acción, insértala con el selector de variables: por ejemplo, `%IdSolicitud%`. A01, A02, etc. son referencias de esta guía; no debes escribirlas como comandos y no tienen que coincidir con los números de línea de PAD.

## Capturar los controles antes de utilizarlos

1. Mantén abierta la aplicación. En PAD abre **Elementos de interfaz de usuario** y selecciona Agregar elemento. También puedes hacerlo desde el campo Elemento UI de una acción.
2. Señala el interior del recuadro Identificador, no la etiqueta a su izquierda. Cuando se resalte, pulsa Ctrl + clic izquierdo, según indique el capturador. Finaliza la captura.
3. Identifica el elemento guardado con un nombre claro, como `Campo_Identificador`. Repite con Solicitante y Descripción, seleccionando sus recuadros editables.
4. Captura los desplegables Área y Tipo como `Lista_Area` y `Lista_Tipo`. Captura el control de la lista, no una opción desplegada aislada.
5. Captura los botones Nueva solicitud y Registrar solicitud como `Boton_Nueva` y `Boton_Registrar`. Captura también la ventana principal como `Ventana_Solicitudes` para enfocarla.

En total tendrás siete controles y una referencia a la ventana. Si ya capturaste Solicitante en el puente y funciona, reutilízalo. Los nombres anteriores son alias sugeridos; utiliza los que realmente aparezcan en tu panel. `SELECTORES.md` ayuda a reconocer los IDs de los controles.

---

# Etapa 1  Construir las acciones de captura

Continuación | Agrega cada acción debajo de la anterior y pulsa Guardar en su configuración

## A06 y A07  Enfocar y limpiar

**A06. Enfocar ventana (Focus window).** Busca la acción, arrástrala después de las cinco variables y elige la ventana mediante el elemento UI `Ventana_Solicitudes`. Esta acción prepara el foco para trabajar con el formulario.

**A07. Pulsar botón en ventana (Press button in window).** Agrega otra acción. En Elemento UI elige `Boton_Nueva`. No escribas una variable de texto: esta acción pulsa el botón Nueva solicitud y limpia el formulario.

## A08 y A09  Rellenar dos campos de texto

**A08. Rellenar campo de texto en ventana (Populate text field in window).** En Elemento UI selecciona `Campo_Identificador`. En Texto para rellenar inserta `%IdSolicitud%`. Configura reemplazo del contenido; si aparece Anexar texto, déjalo desactivado.

**A09. Rellenar campo de texto en ventana.** Agrega una segunda acción del mismo tipo. Selecciona el elemento del campo Solicitante y escribe `%Solicitante%` mediante el selector de variables. Conserva el reemplazo del contenido.

Recuerda: son dos acciones separadas. Si duplicas A08 para crear A09, cambia **tanto el elemento UI como la variable**. Cambiar solo el texto haría que ambos datos se escribieran en Identificador.

## A10 y A11  Seleccionar Área y Tipo

**A10. Establecer valor de lista desplegable en ventana (Set drop-down list value in window).** Agrega la acción. En Elemento UI elige `Lista_Area`. En la modalidad de selección elige por nombre/texto de la opción, no por índice. En el valor de la opción inserta `%Area%`.

**A11. Establecer valor de lista desplegable en ventana.** Agrega una segunda acción. En Elemento UI elige `Lista_Tipo`; selecciona por nombre/texto e inserta `%Tipo%`. Guarda. Los valores Administración y Equipo deben coincidir con las opciones de la aplicación.

## A12  Rellenar Descripción y hacer una primera prueba

Agrega **Rellenar campo de texto en ventana**. Selecciona el campo Descripción y configura Texto para rellenar = `%Descripcion%`, reemplazando el contenido. Guarda el flujo y ejecútalo con la aplicación abierta.

**Detente a comprobar:** el flujo tiene doce acciones y solo debe limpiar y rellenar los cinco campos. Todavía no registra nada. Verifica cada dato en la aplicación. Si una lista no se selecciona o un texto aparece en otro campo, corrige ese elemento antes de continuar.

---

# Etapa 1  Abrir la confirmación

Continuación | Terminar la secuencia sin guardar todavía

## A13  Pulsar Registrar solicitud

Cuando el llenado funcione, agrega **Pulsar botón en ventana** al final. En Elemento UI selecciona `Boton_Registrar`, correspondiente al botón Registrar solicitud. Guarda y ejecuta el flujo desde el principio.

Debe aparecer **Confirmar registro**. Comprueba que corresponde a SOL-500. Durante esta prueba pulsa **Cancelar** manualmente; la etapa 2 incorporará la confirmación automática.

## Revisar el orden completo de la etapa 1

```text
A01 Establecer variable IdSolicitud
A02 Establecer variable Solicitante
A03 Establecer variable Area
A04 Establecer variable Tipo
A05 Establecer variable Descripcion
A06 Enfocar la ventana principal
A07 Pulsar Nueva solicitud
A08 Rellenar Identificador con %IdSolicitud%
A09 Rellenar Solicitante con %Solicitante%
A10 Seleccionar Área por el texto %Area%
A11 Seleccionar Tipo por el texto %Tipo%
A12 Rellenar Descripción con %Descripcion%
A13 Pulsar Registrar solicitud
```

## Capturar los elementos del diálogo

Con el flujo detenido, abre manualmente Confirmar registro utilizando datos válidos. Captura la ventana como `Ventana_Confirmar` y su botón Confirmar como `Boton_Confirmar`. Finaliza la captura y pulsa Cancelar manualmente para cerrar sin guardar.

El botón Confirmar pertenece a una ventana diferente del formulario. No lo confundas con Registrar solicitud: este último abre la confirmación; Confirmar autoriza el guardado.

**Punto de control:** tienes las trece acciones en orden, los cinco campos se rellenan correctamente y aparece la confirmación. El registro completo se construye a continuación.

---

# Etapa 2  Confirmar y leer el resultado

40 minutos para toda la etapa 2 | Preparar controles y añadir acciones al final

## Preparación manual de los controles de resultado

Con el flujo detenido, registra manualmente una solicitud ficticia con ID SOL-900 y los demás datos válidos del ejemplo. Si ya existe, usa otro ID de preparación libre. Reserva SOL-500 para el bot y SOL-001 a SOL-003 para el lote.

En el diálogo **Registro completado**, captura la ventana como `Ventana_Completado`, el recuadro del folio como `Campo_FolioDialogo` y el botón Aceptar como `Boton_AceptarResultado`. Cierra el diálogo y captura el recuadro **Último folio** de la ventana principal como `Campo_UltimoFolio`.

El registro de preparación no forma parte de la entrega y cambia la numeración. Nunca calcules ni escribas de antemano un folio en el flujo.

## Configurar las acciones B01 a B06

**B01. Esperar ventana (Wait for window).** Arrástrala después de A13. Selecciona `Ventana_Confirmar`, configura la espera de apertura y activa el fallo por tiempo de espera con un máximo de 10 segundos, según las opciones de tu versión. No uses una pausa fija como sustituto.

**B02. Pulsar botón en ventana.** Selecciona `Boton_Confirmar`. Aquí el bot autoriza el guardado; no debe hacerlo una persona durante la ejecución final.

**B03. Esperar ventana.** Agrega otra acción de espera. Selecciona `Ventana_Completado`, espera su apertura y limita la espera a 10 segundos.

**B04. Obtener detalles de un elemento de interfaz de usuario en ventana (Get details of a UI element in window).** Selecciona `Campo_FolioDialogo`. En Nombre de atributo elige el que devuelve el contenido del recuadro, por ejemplo Propio texto/Own Text o Value según el motor. En Variables producidas renombra la salida como `FolioActual`.

Recuerda que renombrar una salida es distinto de escribir un valor fijo: la acción debe obtener el dato durante la ejecución. Comprueba en Variables que devuelve un valor como FOL-0002 y no el nombre «Folio generado». Si devuelve el nombre, detén la prueba y revisa el atributo antes de continuar.

**B05. Pulsar botón en ventana.** Selecciona `Boton_AceptarResultado`, dentro de Registro completado. Este botón cierra el diálogo de éxito.

**B06. Obtener detalles de un elemento de interfaz de usuario en ventana.** Agrega otra acción de lectura, selecciona `Campo_UltimoFolio` y el atributo que devuelve su contenido. Renombra la salida como `FolioEnPantalla`.

No ejecutes todavía el recorrido completo repetidamente: después de guardar SOL-500, un nuevo registro del mismo ID será duplicado. Termina de configurar las comprobaciones y la salida de la página siguiente antes del ensayo completo.

---

# Etapa 2  Comprobar y escribir la evidencia

Continuación | Añadir controles después de B06 y preparar la ruta antes de registrar

## B07 a B09  Construir tres condiciones de rechazo

Recuerda cómo construir un If: arrastra **If**, configura sus operandos y guarda. PAD agrega un **End**. Las acciones que pertenecen a la condición deben quedar sangradas entre If y End. Las siguientes van debajo de ese End, fuera del bloque.

**B07. If:** primer operando `%FolioActual%`; operador **Está vacío**. Si tu versión oculta el segundo operando para esta condición, no escribas nada más. Dentro agrega Mostrar mensaje: «No se pudo leer el folio», y después **Detener flujo (Stop flow)**, con finalización normal.

**B08. If:** primer operando `%FolioActual%`; operador **No comienza por**; segundo operando `FOL-`, sin comillas. Dentro agrega Mostrar mensaje: «El valor leído no parece un folio», y Detener flujo. Coloca este If después del End de B07.

**B09. If:** primer operando `%FolioActual%`; operador **No es igual a**; segundo operando `%FolioEnPantalla%`, insertado como variable. Dentro agrega Mostrar mensaje: «Los folios no coinciden», y Detener flujo. Colócalo después del End de B08.

Solo si las tres condiciones son falsas, el flujo debe llegar a la escritura. Estos mensajes aparecen únicamente ante una comprobación fallida.

## Preparar la ruta al principio del flujo

Antes de A01 agrega **Establecer variable**, nombre `RutaIndividual`, valor: la ruta absoluta de un archivo nuevo dentro de evidencias. Ejemplo que debes adaptar: `C:\RPA\proyecto_solicitudes\evidencias\individual_01.txt`. La carpeta debe existir.

Debajo agrega **Si el archivo existe (If file exists)**, opción Existe, ruta `%RutaIndividual%`. Dentro coloca Mostrar mensaje: «Elige un nombre de archivo nuevo», y Detener flujo. A01 debe quedar después del End, fuera de esa condición. Así compruebas la ruta antes de crear un registro.

## B10  Escribir el resultado individual

Después del End de B09 agrega **Escribir texto en archivo (Write text to file)**. Ruta = `%RutaIndividual%`; codificación = UTF-8; si existe = sobrescribir (el control inicial ya exige un nombre libre). En Texto para escribir introduce estas tres líneas, insertando las variables:

```text
Solicitud: %IdSolicitud%
Folio: %FolioActual%
Estado: REGISTRADA
```

Guarda y ejecuta desde el principio. Verifica el archivo y el folio visto en pantalla. **Punto de control:** una solicitud guardada y un archivo que acredita su ID y folio. Si necesitas repetir SOL-500, consulta cómo preparar un ensayo limpio al final de la guía.

---

# Etapa 3  Preparar el lote y su archivo de salida

60 minutos para toda la etapa 3 | Conservar el avance individual y adaptar el flujo

## Preparar una versión de lote

Guarda el flujo individual y conserva una copia con el mecanismo disponible en tu diseñador. Trabaja la versión de lote como `Proyecto_RegistroSolicitudes_Lote`. Si creas otro flujo y copias acciones, comprueba también que dispone de todos los elementos UI.

En la versión de lote elimina la preparación de RutaIndividual y su If, las cinco asignaciones fijas A01 a A05 y la escritura B10. Conserva A06 a A13 y B01 a B09, incluyendo los mensajes, Detener flujo y End de sus condiciones: serán el recorrido que se repetirá.

## C01 y C02  Establecer las rutas

Al principio agrega dos acciones **Establecer variable**. En la primera, Variable = `RutaCsv`; Valor = ruta absoluta a `datos\solicitudes_base.csv`. En la segunda, Variable = `RutaResultados`; Valor = ruta absoluta nueva, por ejemplo `C:\RPA\proyecto_solicitudes\evidencias\resultados_lote_01.csv`. Adapta ambas a tu carpeta real.

## C03  Proteger un archivo de resultados anterior

Agrega **Si el archivo existe**, condición Existe, ruta `%RutaResultados%`. Dentro coloca Mostrar mensaje: «Elige otro archivo de resultados», seguido de Detener flujo. Las acciones siguientes deben quedar después del End.

## C04  Leer las tres filas

Agrega **Leer de archivo CSV (Read from CSV file)**. Archivo = `%RutaCsv%`; codificación = UTF-8; separador personalizado = coma; primera línea contiene nombres de columnas = activado. En Variables producidas renombra la tabla como `Solicitudes`.

Ejecuta solo hasta esta acción y revisa la tabla en Variables. Debe contener tres filas con las columnas `id_solicitud`, `solicitante`, `area`, `tipo` y `descripcion`. No continúes si el encabezado aparece como una fila de datos o todo el registro aparece en una única columna.

## C05  Escribir el encabezado una sola vez

Agrega **Escribir texto en archivo**. Ruta = `%RutaResultados%`; texto = `id_solicitud,folio,estado`; codificación = UTF-8; si existe = sobrescribir; añadir nueva línea = activado. La condición C03 exige que el archivo no exista al comenzar el ensayo completo.

## C06  Crear el bucle

Agrega **Para cada (For each)**. Valor que se va a iterar = `%Solicitudes%`; nombre de la fila actual = `SolicitudActual`. Guarda: aparecerá su End. Coloca dentro, con sangría, las acciones descritas en la página siguiente. No copies tres veces la captura; una sola secuencia debe trabajar con cada fila.

---

# Etapa 3  Configurar el interior del bucle

Continuación | Todas las acciones D van entre For each y su End

## D01 a D05  Asignar las cinco variables de la fila

Recuerda: agrega una acción **Establecer variable** por fila de esta tabla. Conserva los nombres que utilizan las acciones de captura, pero ahora el valor procede de SolicitudActual y no de SOL-500.

| Variable | Valor de la fila |
|---|---|
| IdSolicitud | %SolicitudActual['id_solicitud']% |
| Solicitante | %SolicitudActual['solicitante']% |
| Area | %SolicitudActual['area']% |
| Tipo | %SolicitudActual['tipo']% |
| Descripcion | %SolicitudActual['descripcion']% |

## D06  Ejecutar el recorrido que ya funciona

Después de esas asignaciones, coloca dentro del bucle A06 a A13 y B01 a B09 en su orden original. D06 representa ese grupo, no una acción nueva del catálogo. Incluye las tres condiciones completas con sus End. No incluyas las asignaciones fijas A01 a A05 ni la escritura individual B10.

En cada fila, Nueva solicitud limpia el formulario y las dos lecturas B04 y B06 actualizan las variables de folio. Mantén el comportamiento predeterminado de detener la ejecución ante un error técnico; no configures «continuar con la siguiente acción» si falla una lectura, porque podría conservarse un valor anterior.

## D07  Anexar una fila a la bitácora

Dentro del bucle, después del End de B09, agrega **Escribir texto en archivo**. Ruta = `%RutaResultados%`; texto = `%IdSolicitud%,%FolioActual%,REGISTRADA`; codificación = UTF-8; si existe = **anexar contenido**; añadir nueva línea = activado.

Las columnas de salida contienen IDs y estados controlados sin comas internas. Escribe solo esta fila en cada iteración, no el encabezado ni una tabla completa. El CSV de entrada se lee con la acción específica CSV.

## C07  Informar al finalizar

Fuera del bucle, debajo de su End, agrega Mostrar mensaje: «Lote terminado. Revisa %RutaResultados%». No coloques mensajes que pidan Aceptar dentro del recorrido exitoso de cada fila; los botones de la aplicación deben pulsarlos las acciones del bot.

## Orden final de la versión de lote

```text
C01 Establecer RutaCsv
C02 Establecer RutaResultados
C03 Si archivo de salida existe -> avisar y detener -> End
C04 Leer CSV -> Solicitudes
C05 Escribir encabezado
C06 For each SolicitudActual in Solicitudes
      D01-D05 Asignar variables desde la fila
      D06 Capturar y verificar: A06-A13, B01-B09
      D07 Anexar ID, folio y estado
    End del For each
C07 Mostrar resumen final
```

Guarda y ejecuta con SOL-001 a SOL-003 aún sin registrar. Comprueba tres filas de resultados más el encabezado, tres IDs distintos, folios leídos y un aumento de tres registros. Revisa también los cinco campos de cada solicitud. Los folios exactos dependen del historial; no los inventes.

---

# Comprobaciones y entrega

25 minutos | Demostrar y explicar lo construido

## Ensayos que debes documentar

| Ensayo | Resultado que demuestra |
|---|---|
| Una solicitud con valores fijos | Cinco campos correctos, confirmación y folio persistido en archivo |
| Lote de tres filas | Tres altas y tres resultados relacionados por identificador |
| Ventana en otra posición | Los elementos capturados siguen identificando los controles |

Para la prueba de posición basta repetir la captura y detenerse antes de confirmar; así no creas otro registro. Las variantes Formulario reorganizado y Respuesta lenta son opcionales en esta sesión.

## Repetir sin confundir ensayos

La aplicación conserva los registros en `aplicacion/runtime/solicitudes.xml`. Nueva solicitud no vacía ese historial. Si repites un ID ya guardado, verás Solicitud duplicada, no Registro completado.

Para iniciar un ensayo limpio, cierra la aplicación y renombra manualmente la carpeta `runtime` como `runtime_ensayo01` u otro nombre libre. Vuelve a abrir iniciar.cmd: la aplicación creará un registro nuevo. Preserva las carpetas anteriores y usa también nuevos nombres de archivos de resultados. Este procedimiento es preparación manual del laboratorio, no una estrategia de recuperación del bot.

Si el bot se detiene después de confirmar, no vuelvas a ejecutarlo a ciegas. Anota el ID, conserva la pantalla y pide revisión del registro. Resolver esa incertidumbre automáticamente queda para una sesión posterior.

## Qué entregar

1. Flujos `Proyecto_RegistroSolicitudes` y `Proyecto_RegistroSolicitudes_Lote` guardados en PAD. Al compartirlo, comprueba que se transfieren los elementos UI; incluye capturas y pasos para reconstruirlos.
2. Archivo individual y `resultados_lote_01.csv` o el nombre equivalente de tu ensayo.
3. `PLANTILLA_ENTREGA.md` completada con rutas configurables, pruebas y explicación breve de tres selectores.
4. Evidencia del resultado y una reflexión de 150 a 200 palabras: qué dato cambia en cada iteración, cómo verificas el guardado y por qué el folio no se inventa.

## Criterios de evaluación

| Criterio | Puntos |
|---|---|
| Captura correcta de los cinco campos | 20 |
| Confirmación, espera y lectura del folio real | 25 |
| Evidencia individual coherente con la interfaz | 15 |
| Bucle CSV y bitácora de tres solicitudes | 25 |
| Selectores, pruebas y explicación reproducible | 15 |

Por criterio: completo, puntuación total; parcialmente funcional, mitad; ausente, cero. Entrega el avance real y señala lo pendiente. Un clic no acredita un registro exitoso.

## Referencias

Microsoft Learn, consulta del 29 de septiembre de 2026:

- [Acciones de interfaz de usuario](https://learn.microsoft.com/es-es/power-automate/desktop-flows/actions-reference/uiautomation)
- [Acciones de archivos y CSV](https://learn.microsoft.com/es-es/power-automate/desktop-flows/actions-reference/file)
