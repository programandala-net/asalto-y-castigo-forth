\ data_structure.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606291902

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Estructura de datos de los entes

\ Denominamos «ente» a cualquier componente del mundo virtual del
\ juego que es manipulable por el programa.  «Entes» por tanto son los
\ objetos, manipulables o no por el jugador; los personajes,
\ interactivos o no; los lugares; y el propio personaje protagonista.
\
\ Cada ente tiene una ficha en la base de datos del juego.  La base de
\ datos es una zona de memoria dividida en partes iguales, una para
\ cada ficha. El identificador de cada ficha es una palabra que al
\ ejecutarse deja en la pila la dirección de memoria donde se
\ encuentra la ficha.
\
\ Los campos de la base de datos, como es habitual en Forth en este
\ tipo de estructuras, son palabras que suman el desplazamiento
\ adecuado a la dirección base de la ficha, que reciben en la pila,
\ apuntando así a la dirección de memoria que contiene el campo
\ correspondiente.
\
\ Salvo los campos buleanos, que ocupan un solo bitio gracias a las
\ palabras creadas para ello, todos los demás campos ocupan una celda.
\ La «celda» es un concepto de ANS Forth: es la unidad en que se mide
\ el tamaño de cada elemento de la pila, y capaz por tanto de contener
\ una dirección de memoria.  En los sistemas Forth de 8 o 16 bitios
\ una celda equivale a un valor de 16 bitios; en los sistemas Forth de
\ 32 bitios, como Gforth, una celda equivale a un valor de 32 bitios.
\
\ Para facilitar la legibilidad, los nombres de los campos empiezan
\ con el signo de tilde, «~»; los que contienen direcciones de
\ ejecución terminan con «-xt»; los que contienen códigos de error
\ terminan con «-error#».

0 \ Valor inicial de desplazamiento para el primer campo

\ Identificación
field: ~name-str  \ Dirección de una cadena dinámica que contendrá el nombre del ente
field: ~init-xt  \ Dirección de ejecución de la palabra que inicializa las propiedades de un ente
field: ~description-xt  \ Dirección de ejecución de la palabra que describe el ente
field: ~direction  \ Desplazamiento del campo de dirección al que corresponde el ente (solo se usa en los entes que son direcciones)

\ Contadores
field: ~familiar  \ Contador de familiaridad (cuánto le es conocido el ente al protagonista)
field: ~times-open  \ Contador de veces que ha sido abierto.
field: ~conversations  \ Contador para personajes: número de conversaciones tenidas con el protagonista
field: ~visits  \ Contador para escenarios: visitas del protagonista (se incrementa al abandonar el escenario)

\ Errores específicos (cero si no hay error); se usan para casos especiales
\ (los errores apuntados por estos campos no reciben parámetros salvo en `what`)
field: ~break-error#  \ Error al intentar romper el ente
field: ~take-error#  \ Error al intentar tomar el ente

\ Entes relacionados
field: ~location  \ Identificador del ente en que está localizado (sea escenario, contenedor, personaje o «limbo»)
field: ~previous-location  \ Ídem para el ente que fue la localización antes del actual
field: ~owner  \ Identificador del ente al que pertenece «legalmente» o «de hecho», independientemente de su localización.

\ Direcciones de ejecución de las tramas de escenario
field: ~can-i-enter-location-xt  \ Trama previa a la entrada al escenario
field: ~before-describing-location-xt  \ Trama de entrada antes de describir el escenario
field: ~after-describing-location-xt  \ Trama de entrada tras describir el escenario
field: ~after-listing-entities-xt  \ Trama de entrada tras listar los entes presentes
field: ~before-leaving-location-xt  \ Trama antes de abandonar el escenario

\ Salidas
field: ~north-exit  \ Ente de destino hacia el norte
field: ~south-exit  \ Ente de destino hacia el sur
field: ~east-exit  \ Ente de destino hacia el este
field: ~west-exit  \ Ente de destino hacia el oeste
field: ~up-exit  \ Ente de destino hacia arriba
field: ~down-exit  \ Ente de destino hacia abajo
field: ~out-exit  \ Ente de destino hacia fuera
field: ~in-exit  \ Ente de destino hacia dentro

\ Indicadores
bitfields
  bitfield: ~has-definite-article  \ ¿El artículo de su nombre debe ser siempre el artículo definido?
  bitfield: ~has-feminine-name  \ ¿El género gramatical de su nombre es femenino?
  bitfield: ~has-no-article  \ ¿Su nombre no debe llevar artículo?
  bitfield: ~has-personal-name  \ ¿Su nombre es un nombre propio?
  bitfield: ~has-plural-name  \ ¿Su nombre es plural?
  bitfield: ~is-animal  \ ¿Es animal?
  bitfield: ~is-character  \ ¿Es un personaje?
  bitfield: ~is-cloth  \ ¿Es una prenda que puede ser puesta y quitada?
  bitfield: ~is-decoration  \ ¿Forma parte de la decoración de su localización?
  bitfield: ~is-global-indoor  \ ¿Es global (común) en los escenarios interiores?
  bitfield: ~is-global-outdoor  \ ¿Es global (común) en los escenarios al aire libre?
  bitfield: ~is-not-listed  \ ¿No debe ser listado (entre los entes presentes o en inventario)?
  bitfield: ~is-human  \ ¿Es humano?
  bitfield: ~is-light  \ ¿Es una fuente de luz que puede ser encendida?
  bitfield: ~is-lit  \ ¿El ente, que es una fuente de luz que puede ser encendida, está encendido?
  bitfield: ~is-location  \ ¿Es un escenario?
  bitfield: ~is-indoor-location  \ ¿Es un escenario interior (no exterior, al aire libre)?
  bitfield: ~is-open  \ ¿Está abierto?
  bitfield: ~is-vegetal  \ ¿Es vegetal?
  bitfield: ~is-worn  \ ¿Siendo una prenda, está puesta?
field: ~flags-0  \ Campo para albergar los indicadores anteriores

[false] [if]  \ XXX OLD -- campos que aún no se usan.:

field: ~times-closed  \ Contador de veces que ha sido cerrado.
field: ~desambiguation-xt  \ Dirección de ejecución de la palabra que desambigua e identifica el ente
field: ~stamina  \ Energía de los entes vivos

bitfield: ~is-lock  \ ¿Está cerrado con llave?
bitfield: ~is-openable  \ ¿Es abrible?
bitfield: ~is-lockable  \ ¿Es cerrable con llave?
bitfield: ~is-container  \ ¿Es un contenedor?

[then]

to /entity  \ Tamaño de cada ficha

' noop is init-entity
  \ XXX TMP

\ vim:filetype=gforth:fileencoding=utf-8