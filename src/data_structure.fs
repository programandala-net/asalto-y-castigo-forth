\ data_structure.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606291717

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

constant /entity  \ Tamaño de cada ficha

\ ==============================================================
\ Interfaz de campos

\ Las palabras de esta sección facilitan la tarea de
\ interactuar con los campos de las fichas, evitando repetir
\ cálculos, escondiendo parte de los entresijos de las fichas
\ y haciendo el código más conciso, más fácil de modificar y
\ más legible.
\
\ Algunas de las palabras que definimos a continuación actúan
\ de forma análoga a los campos de las fichas de entes:
\ reciben en la pila el identificador de ente y devuelven en
\ ella un resultado. La diferencia es que es un resultado
\ calculado.
\
\ Otras actúan como procedimientos para realizar operaciones
\ frecuentes con los entes.

\ ----------------------------------------------
\ Herramientas para los campos de dirección

' ~north-exit alias ~first-exit
  \ Primera salida definida en la ficha.

' ~in-exit alias ~last-exit
  \ Última salida definida en la ficha.

\ Guardar el desplazamiento de cada campo de dirección respecto al
\ primero de ellos:
0 ~first-exit constant first-exit>
0 ~last-exit constant last-exit>
0 ~north-exit constant north-exit>
0 ~south-exit constant south-exit>
0 ~east-exit constant east-exit>
0 ~west-exit constant west-exit>
0 ~up-exit constant up-exit>
0 ~down-exit constant down-exit>
0 ~out-exit constant out-exit>
0 ~in-exit constant in-exit>

last-exit> cell+ first-exit> - constant /exits
  \ Espacio en octetos ocupado por los campos de salidas.

/exits cell / constant #exits
  \ Número de salidas.

0 constant no-exit
  \ Marcador para direcciones sin salida en un ente dirección.

: exit?  ( a -- f )  no-exit <>  ;
  \ ¿Está abierta una dirección de salida de un ente escenario?
  \ a = Contenido de un campo de salida de un ente (que será el ente de destino, o cero)

\ ----------------------------------------------
\ Interfaz básica para leer y modificar los campos

\ Las palabras que siguen permiten hacer las operaciones
\ básicas de obtención y modificación del contenido de los
\ campos.

\ Obtener el contenido de un campo a partir de un identificador de ente

: break-error#  ( a -- u )  ~break-error# @  ;
: conversations  ( a -- u )  ~conversations @  ;
: description-xt  ( a -- xt )  ~description-xt @  ;
: direction  ( a -- u )  ~direction @  ;
: familiar  ( a -- u )  ~familiar @  ;
: flags-0  ( a -- x )  ~flags-0 @  ;
: has-definite-article?  ( a -- f )  ~has-definite-article bit@  ;
: has-feminine-name?  ( a -- f )  ~has-feminine-name bit@  ;
: has-masculine-name?  ( a -- f )  has-feminine-name? 0=  ;
: has-no-article?  ( a -- f )  ~has-no-article bit@  ;
: has-personal-name?  ( a -- f )  ~has-personal-name bit@  ;
: has-plural-name?  ( a -- f )  ~has-plural-name bit@  ;
: has-singular-name?  ( a -- f )  has-plural-name? 0=  ;
: init-xt  ( a -- xt )  ~init-xt @  ;
: is-animal?  ( a -- f )  ~is-animal bit@  ;
: is-character?  ( a -- f )  ~is-character bit@  ;
: is-cloth?  ( a -- f )  ~is-cloth bit@  ;
: is-decoration?  ( a -- f )  ~is-decoration bit@  ;
: is-global-indoor?  ( a -- f )  ~is-global-indoor bit@  ;
: is-global-outdoor?  ( a -- f )  ~is-global-outdoor bit@  ;
: is-human?  ( a -- f )  ~is-human bit@  ;
: is-light?  ( a -- f )  ~is-light bit@  ;
: is-not-listed?  ( a -- f )  ~is-not-listed bit@  ;
: is-listed?  ( a -- f )  is-not-listed? 0=  ;
: is-lit?  ( a -- f )  ~is-lit bit@  ;
: is-not-lit?  ( a -- f )  is-lit? 0=  ;
: is-location?  ( a -- f )  ~is-location bit@  ;
: is-indoor-location?  ( a -- f )  dup is-location? swap ~is-indoor-location bit@ and  ;
: is-outdoor-location?  ( a -- f )  is-indoor-location? 0=  ;
: is-open?  ( a -- f )  ~is-open bit@  ;
: is-closed?  ( a -- f )  is-open? 0=  ;
: name-str  ( a1 -- a2 )  ~name-str @  ;
: times-open  ( a -- u )  ~times-open @  ;
: owner  ( a1 -- a2 )  ~owner @  ;
: is-vegetal?  ( a -- f )  ~is-vegetal bit@  ;
: is-worn?  ( a -- f )  ~is-worn bit@  ;
: location  ( a1 -- a2 )  ~location @  ;
: can-i-enter-location-xt  ( a -- xt )  ~can-i-enter-location-xt @  ;
: before-describing-location-xt  ( a -- xt )  ~before-describing-location-xt @  ;
: after-describing-location-xt  ( a -- xt )  ~after-describing-location-xt @  ;
: after-listing-entities-xt  ( a -- xt )  ~after-listing-entities-xt @  ;
: before-leaving-location-xt  ( a -- xt )  ~before-leaving-location-xt @  ;
: previous-location  ( a1 -- a2 )  ~previous-location @  ;
: take-error#  ( a -- u )  ~take-error# @  ;
: visits  ( a -- u )  ~visits @  ;

: north-exit  ( a1 -- a2 )  ~north-exit @  ;
: south-exit  ( a1 -- a2 )  ~south-exit @  ;
: east-exit  ( a1 -- a2 )  ~east-exit @  ;
: west-exit  ( a1 -- a2 )  ~west-exit @  ;
: up-exit  ( a1 -- a2 )  ~up-exit @  ;
: down-exit  ( a1 -- a2 )  ~down-exit @  ;
: out-exit  ( a1 -- a2 )  ~out-exit @  ;
: in-exit  ( a1 -- a2 )  ~in-exit @  ;

\ Modificar el contenido de un campo a partir de un identificador de ente

: conversations++  ( a -- )  ~conversations ?++  ;
: familiar++  ( a -- )  ~familiar ?++  ;
: has-definite-article  ( a -- )  ~has-definite-article bit-on  ;
: has-feminine-name  ( a -- )  ~has-feminine-name bit-on  ;
: has-masculine-name  ( a -- )  ~has-feminine-name bit-off  ;
: has-no-article  ( a -- )  ~has-no-article bit-on  ;
: has-personal-name  ( a -- )  ~has-personal-name bit-on  ;
: has-plural-name  ( a -- )  ~has-plural-name bit-on  ;
: has-singular-name  ( a -- )  ~has-plural-name bit-off  ;
: be-character  ( a -- )  ~is-character bit-on  ;
: be-animal  ( a -- )  ~is-animal bit-on  ;
: be-light  ( a -- )  ~is-light bit-on  ;
: be-not-listed  ( a -- f )  ~is-not-listed bit-on  ;
: be-lit  ( a -- )  ~is-lit bit-on  ;
: be-not-lit  ( a -- )  ~is-lit bit-off  ;
: be-cloth  ( a -- )  ~is-cloth bit-on  ;
: be-decoration  ( a -- )  ~is-decoration bit-on  ;
: be-global-indoor  ( a -- )  ~is-global-indoor bit-on  ;
: be-global-outdoor  ( a -- )  ~is-global-outdoor bit-on  ;
: be-human  ( a -- )  ~is-human bit-on  ;
: be-location  ( a -- )  ~is-location bit-on  ;
: be-indoor-location  ( a -- )  dup be-location ~is-indoor-location bit-on  ;
: be-outdoor-location  ( a -- )  dup be-location ~is-indoor-location bit-off  ;
: be-open  ( a -- )  ~is-open bit-on  ;
: be-closed  ( a -- )  ~is-open bit-off  ;
: times-open++  ( a -- )  ~times-open ?++  ;
: be-worn  ( a -- )  ~is-worn bit-on  ;
: be-not-worn  ( a -- )  ~is-worn bit-off  ;
: visits++  ( a -- )  ~visits ?++  ;

\ ----------------------------------------------
\ Campos calculados o seudo-campos

\ Los seudo-campos devuelven un cálculo. Sirven para añadir
\ una capa adicional de abstracción y simplificar el código.

\ Por conveniencia, en el caso de algunos de los campos binarios
\ creamos también palabras para la propiedad contraria.  Por
\ ejemplo, en las fichas existe el campo `~is-open?` para indicar
\ si un ente está abierto, pero creamos las palabras necesarias
\ para examinar y modificar tanto la propiedad de «cerrado» como
\ la de «abierto». Esto ayuda a escribir posteriormente el código
\ efectivo (pues no hace falta recordar si la propiedad real y por
\ tanto el campo de la ficha del ente era «abierto» o «cerrado») y
\ hace el código más conciso y legible.

: is-direction?  ( a -- f )  direction 0<>  ;
: is-familiar?  ( a -- f )  familiar 0>  ;
: is-visited?  ( a -- f )  visits 0>  ;
: is-not-visited?  ( a -- f )  visits 0=  ;
: conversations?  ( a -- f )  conversations 0<>  ;
: no-conversations?  ( a -- f )  conversations 0=  ;
: has-north-exit?  ( a -- f )  north-exit exit?  ;
: has-east-exit?  ( a -- f )  east-exit exit?  ;
: has-south-exit?  ( a -- f )  south-exit exit?  ;

: owns?  ( a1 a2 -- f )  owner =  ;
: belongs?  ( a1 a2 -- f )  swap owns?  ;
: owns  ( a1 a2 -- )  ~owner !  ;
: belongs  ( a1 a2 -- )  swap owns  ;

: belongs-to-protagonist?  ( a -- f )  owner protagonist~ =  ;
: belongs-to-protagonist  ( a -- )  ~owner protagonist~ swap !  ;

: is-living-being?  ( a -- f )
  dup is-vegetal?  over is-animal? or  swap is-human? or  ;
  \ ¿El ente es un ser vivo (aunque esté muerto)?

: be-there  ( a1 a2 -- )  ~location !  ;
  \ Hace que un ente sea la localización de otro.
  \ a1 = Ente que será la localización de a2
  \ a2 = Ente cuya localización será a1

: taken  ( a -- )  protagonist~ swap be-there  ;
  \ Hace que el protagonista sea la localización de un ente.

: was-there  ( a1 a2 -- )  ~previous-location !  ;
  \ Hace que un ente sea la localización previa de otro.
  \ a1 = Ente que será la localización previa de a2
  \ a2 = Ente cuya localización previa será a1

: is-there?  ( a1 a2 -- f )  location =  ;
  \ ¿Está un ente localizado en otro?
  \ a1 = Ente que actúa de localización
  \ a2 = Ente cuya localización se comprueba

: was-there?  ( a1 a2 -- f )  previous-location =  ;
  \ ¿Estuvo un ente localizado en otro?
  \ a1 = Ente que actúa de localización
  \ a2 = Ente cuya localización se comprueba

: is-global?  ( a -- f )
  dup is-global-outdoor?
  swap is-global-indoor? or  ;
  \ ¿Es el ente un ente global?

: my-location  ( -- a )  protagonist~ location  ;
  \ Devuelve la localización del protagonista.

: my-previous-location  ( -- a )  protagonist~ previous-location  ;
  \ Devuelve la localización anterior del protagonista.

: my-location!  ( a -- )  protagonist~ be-there  ;
  \ Mueve el protagonista al ente indicado.

: am-i-there?  ( a -- f )  my-location =  ;
  \ ¿Está el protagonista en la localización indicada?
  \ a = Ente que actúa de localización

: am-i-outdoor?  ( -- f )  my-location is-outdoor-location?  ;
  \ ¿Está el protagonista en un escenario al aire libre?

: am-i-indoor?  ( -- f )  am-i-outdoor? 0=  ;
  \ ¿Está el protagonista en un escenario cerrado, no al aire libre?

: is-hold?  ( a -- f )  location protagonist~ =  ;
  \ ¿Es el protagonista la localización de un ente?

: is-not-hold?  ( a -- f )  is-hold? 0=  ;
  \ ¿No es el protagonista la localización de un ente?

: be-hold  ( a -- )  ~location protagonist~ swap !  ;
  \ Hace que el protagonista sea la localización de un ente.

: is-worn-by-me?  ( a -- f )  dup is-hold?  swap is-worn?  and  ;
  \ ¿El protagonista lleva puesto el ente indicado?

: is-known?  ( a -- f )
  dup belongs-to-protagonist?  \ ¿Es propiedad del protagonista?
  over is-visited? or  \ ¿O es un escenario ya visitado? (si no es un escenario, la comprobación no tendrá efecto)
  over conversations? or  \ ¿O ha hablado ya con él? (si no es un personaje, la comprobación no tendrá efecto)
  swap is-familiar?  or  ;  \ ¿O ya le es familiar?
  \ ¿El protagonista ya conoce el ente?

: is-unknown?  ( a -- f )  is-known? 0=  ;
  \ ¿El protagonista aún no conoce el ente?

: is-here?  ( a -- f )
  dup location am-i-there?  \ ¿Está efectivamente en la misma localización?
  over is-global-outdoor? am-i-outdoor? and or \ ¿O es un «global exterior» y estamos en un escenario exterior?
  swap is-global-indoor? am-i-indoor? and or  ; \ ¿O es un «global interior» y estamos en un escenario interior?
  \ ¿Está un ente en la misma localización que el protagonista?
  \ El resultado depende de cualquiera de tres condiciones:

: is-not-here?  ( a -- f )  is-here? 0=  ;
  \ ¿Está un ente en otra localización que la del protagonista?
  \ XXX TODO -- no usado

: is-here-and-unknown?  ( a -- f )  dup is-here? swap is-unknown? and  ;
  \ ¿Está un ente en la misma localización que el protagonista y aún no es conocido por él?

: be-here  ( a -- )  my-location swap be-there  ;
  \ Hace que un ente esté en la misma localización que el protagonista.

: is-accessible?  ( a -- f )  dup is-hold?  swap is-here?  or  ;
  \ ¿Es un ente accesible para el protagonista?

: is-not-accessible?  ( a -- f )  is-accessible? 0=  ;
  \ ¿Un ente no es accesible para el protagonista?

: can-be-looked-at?  ( a -- f )
  [false] [if]
    \ XXX OLD -- Primera versión
    dup am-i-there?         \ ¿Es la localización del protagonista?
    over is-direction? or   \ ¿O es un ente dirección?
    over exits~ = or        \ ¿O es el ente "salidas"?
    swap is-accessible? or  \ ¿O está accesible?
  [then]
  [false] [if]
    \ XXX OLD -- Segunda versión, menos elegante pero más rápida y legible
    { entity }
    true case
      entity am-i-there?    of  true  endof  \ ¿Es la localización del protagonista?
      entity is-direction?  of  true  endof  \ ¿Es un ente dirección?
      entity is-accessible? of  true  endof  \ ¿Está accesible?
      entity exits~ =       of  true  endof  \ ¿Es el ente "salidas"?
      false swap
    endcase
  [then]
  [true] [if]
    \ XXX NEW -- Tercera versión, más rápida y compacta
    dup am-i-there?    ?dup if  nip exit  then
      \ ¿Es la localización del protagonista?
    dup is-direction?  ?dup if  nip exit  then
      \ ¿Es un ente dirección?
    dup exits~ =       ?dup if  nip exit  then
      \ ¿Es el ente "salidas"?
    is-accessible?
      \ ¿Está accesible?
  [then]  ;
  \ ¿El ente puede ser mirado?

: can-be-taken?  ( a -- f )
  dup is-decoration?
  over is-human? or
  swap is-character? or 0=  ;
  \ ¿El ente puede ser tomado?
  \ Se usa como norma general, para aquellos entes
  \ que no tienen un error específico indicado en el campo `~take-error#`

: may-be-climbed?  ( a -- f )
  [false] [if]
  fallen-away~
  bridge~
  arch~
  bed~
  flags~
  rocks~
  table~
  [else]  false
  [then]  ;
  \ ¿El ente podría ser escalado? (Aunque en la práctica no sea posible).
  \ XXX TODO -- hacerlo mejor con un indicador en la ficha

: can-be-sharpened?  ( a -- f )
  dup log~ =  swap sword~ =  or  ;
  \ ¿Puede un ente ser afilado?

: talked-to-the-leader?  ( -- f )  leader~ conversations 0<>  ;
  \ ¿El protagonista ha hablado con el líder?

: do-you-hold-something-forbidden?  ( -- f )
  sword~ is-accessible?  stone~ is-accessible?  or  ;
  \ ¿Llevas algo prohibido?
  \ Cálculo usado en varios lugares del programa,
  \ en relación a los refugiados.

: no-torch?  ( -- f )
  torch~ is-not-accessible?  torch~ is-not-lit?  or  ;
  \ ¿La antorcha no está accesible y encendida?

\ ----------------------------------------------
\ Hacer desaparecer entes

0 constant limbo
  \ Marcador para usar como localización de entes inexistentes.

: vanished?  ( a -- f )  location limbo =  ;
  \ ¿Está un ente desaparecido?

: not-vanished?  ( a -- f )  vanished? 0=  ;
  \ ¿No está un ente desaparecido?

: vanish  ( a -- )  limbo swap be-there  ;
  \ Hace desaparecer un ente llevándolo al «limbo».

: vanish-if-hold  ( a -- )
  dup is-hold? if  vanish  else  drop  then  ;
  \ Hace desaparecer un ente si su localización es el protagonista.
  \ XXX TODO -- no usado

\ ----------------------------------------------
\ Herramientas de artículos y pronombres

\ La selección del artículo adecuado para el nombre de un ente tiene
\ su complicación. Depende por supuesto del número y género gramatical
\ del nombre, pero también de la relación con el protagonista
\ [distinción entre artículos definidos e indefinidos] y de la
\ naturaleza del ente [cosa o personaje].
\
\ Por conveniencia, consideramos como artículos ciertas palabras que
\ son adjetivos [como «esta», «ninguna»...], pues en la práctica para
\ el programa su manejo es idéntico: se usan para preceder a los
\ nombres bajo ciertas condiciones.
\
\ En este mismo apartado definimos palabras para calcular los
\ pronombres de objeto indirecto [le/s] y de objeto directo [la/s,
\ lo/s], así como terminaciones habituales.
\
\ Utilizamos una tabla de cadenas de longitud variable, apuntada por
\ una segunda tabla con sus direcciones.  Esto unifica y simplifica
\ los cálculos.

: hs,  ( ca len -- a1 )  here rot rot s,  ;
  \ Compila una cadena en el diccionario y devuelve su dirección.

  \ Pronombres personales:
  s" él" hs, s" ella" hs, s" ellos" hs, s" ellas" hs,
  \ Adjetivos que se tratan como «artículos cercanos»:
  s" este" hs, s" esta" hs, s" estos" hs, s" estas" hs,
  \ Adjetivos que se tratan como «artículos distantes»:
  s" ese" hs, s" esa" hs, s" esos" hs, s" esas" hs,
  \ Adjetivos que se tratan como «artículos negativos»:
  s" ningún" hs, s" ninguna" hs, s" ningunos" hs, s" ningunas" hs,
  \ Artículos posesivos:
  s" tu" hs, s" tu" hs, s" tus" hs, s" tus" hs,
  \ Artículos definidos:
  s" el" hs, s" la" hs, s" los" hs, s" las" hs,
  \ Artículos indefinidos:
  s" un" hs, s" una" hs, s" unos" hs, s" unas" hs,

create 'articles  \ Tabla índice de los artículos
  \ Compilar las direcciones de los artículos:
  , , , ,  \ Indefinidos
  , , , ,  \ Definidos
  , , , ,  \ Posesivos
  , , , ,  \ «Negativos»
  , , , ,  \ «Distantes»
  , , , ,  \ «Cercanos»
  , , , ,  \ Pronombres personales

\ Separaciones entre artículos en la tabla índice (por tanto en
\ celdas):
  cell  constant /article-gender-set  \ De femenino a masculino.
2 cells constant /article-number-set  \ De plural a singular.
4 cells constant /article-type-set    \ Entre grupos de diferente tipo.

: article-number>  ( a -- u )
  has-singular-name? /article-number-set and  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el número gramatical del ente.

: article-gender>  ( a -- u )
  has-masculine-name? /article-gender-set and  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical del ente.

: article-gender+number>  ( a -- u )
  dup article-gender>  swap article-number> +  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical y el número del ente.

: definite-article>  ( a -- u )
  dup has-definite-article?  \ Si el ente necesita siempre artículo definido
  swap is-known? or  \ O bien si el ente es ya conocido por el protagonista
  abs  \ Un grupo (pues los definidos son el segundo)
  /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos
  \ si el ente indicado necesita uno.

: possesive-article>  ( a -- u )
  belongs-to-protagonist? 2 and  \ Dos grupos (pues los posesivos son el tercero)
  /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos posesivos
  \ si el ente indicado necesita uno.

: negative-articles>  ( -- u )
  3 /article-type-set *  ; \ Tres grupos (pues los negativos son el cuarto)
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos negativos».

: undefined-articles>  ( -- u )
  0  ; \ Desplazamiento cero, pues los indefinidos son el primer grupo.
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos indefinidos.

: definite-articles>  ( -- u )
  /article-type-set  ;  \ Un grupo, pues los definidos son el segundo
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos.

: distant-articles>  ( -- u )
  4 /article-type-set *  ;  \ Cuatro grupos, pues los «distantes» son el quinto
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos distantes».

: not-distant-articles>  ( -- u )
  5 /article-type-set *  ;  \ Cinco grupos, pues los «cercanos» son el sexto
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos cercanos».

: personal-pronouns>  ( -- u )
  6 /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los pronombres personales.

: article-type  ( a -- u )
  dup definite-article>  swap possesive-article>  max  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el ente requiera un artículo definido, indefinido o posesivo.

: >article  ( u -- ca1 len1 )  'articles + @ count  ;
  \ Devuelve un artículo de la tabla de artículos
  \ a partir de su índice.

: (article)  ( a -- ca1 len1 )
  dup article-gender>  \ Desplazamiento según el género
  over article-number> +  \ Sumado al desplazamiento según el número
  swap article-type +  \ Sumado al desplazamiento según el tipo
  >article  ;
  \ Devuelve el artículo apropiado para un ente.

: article  ( a -- ca1 len1 | a 0 )
  dup has-no-article? if  0  else  (article)  then  ;
  \ Devuelve el artículo apropiado para un ente, si lo necesita;
  \ en caso contrario devuelve una cadena vacía.

: undefined-article  ( a -- ca1 len1 )
  article-gender+number> undefined-articles> +
  >article  ;
  \ Devuelve el artículo indefinido
  \ correspondiente al género y número de un ente.

: definite-article  ( a -- ca1 len1 )
  article-gender+number> definite-articles> +
  >article  ;
  \ Devuelve el artículo definido
  \ correspondiente al género y número de un ente.

: pronoun  ( a -- ca1 len1 )
  definite-article  s" lo" s" el" replaced  ;
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente.

: ^pronoun  ( a -- ca1 len1 )  pronoun ^uppercase  ;
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente,
  \ con la primera letra mayúscula.

: negative-article  ( a -- ca1 len1 )
  article-gender+number> negative-articles> +  >article  ;
  \ Devuelve el «artículo negativo»
  \ correspondiente al género y número de un ente.

: distant-article  ( a -- ca1 len1 )
  article-gender+number> distant-articles> +  >article  ;
  \ Devuelve el «artículo distante»
  \ correspondiente al género y número de un ente.

: not-distant-article  ( a -- ca1 len1 )
  article-gender+number> not-distant-articles> +  >article  ;
  \ Devuelve el «artículo cercano»
  \ correspondiente al género y número de un ente.

: personal-pronoun  ( a -- ca1 len1 )
  article-gender+number> personal-pronouns> +  >article  ;
  \ Devuelve el pronombre personal
  \ correspondiente al género y número de un ente.

: plural-ending  ( a -- ca1 len1 )
  [false] [if]
    \ XXX OLD -- Método 1, «estilo BASIC»:
    has-plural-name? if  s" s"  else  null$  then
  [else]
    \ XXX NEW -- Método 2, sin estructuras condicionales, «estilo Forth»:
    s" s" rot has-plural-name? and
  [then]  ;
  \ Devuelve la terminación adecuada del plural
  \ para el nombre de un ente.

: plural-ending+  ( ca1 len1 a -- ca2 len2 )  plural-ending s+  ;
  \ Añade a una cadena la terminación adecuada del plural
  \ para el nombre de un ente.

: gender-ending  ( a -- ca1 len1 )
  [false] [if]
    \ Método 1, «estilo BASIC»
    has-feminine-name? if  s" a"  else  s" o"  then
  [else]
    [false] [if]
      \ Método 2, sin estructuras condicionales, «estilo Forth»
      s" oa" drop swap has-feminine-name? abs + 1
    [else]
      \ Método 3, similar, más directo
      c" oa" swap has-feminine-name? abs + 1+ 1
    [then]
  [then]  ;
  \ Devuelve la terminación adecuada del género gramatical
  \ para el nombre de un ente.

: gender-ending+  ( ca1 len1 a -- ca2 len2 )  gender-ending s+  ;
  \ Añade a una cadena la terminación adecuada para el género gramatical de un ente.

: noun-ending  ( a -- ca1 len1 )
  dup gender-ending rot plural-ending s+  ;
  \ Devuelve la terminación adecuada para el nombre de un ente.

' noun-ending alias adjective-ending

: noun-ending+  ( ca1 len1 a -- ca2 len2 )  noun-ending s+  ;
  \ Añade a una cadena la terminación adecuada para el nombre de un ente.

' noun-ending+ alias adjective-ending+
: direct-pronoun  ( a -- ca1 len1 )
  s" l" rot noun-ending s+  ;
  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»).

: ^direct-pronoun  ( a -- ca1 len1 )
  direct-pronoun ^uppercase  ;
  \ Devuelve el pronombre de objeto directo para un ente («La/s»
  \ o «Lo/s»), con la primera letra mayúscula.

: indirect-pronoun  ( a -- ca1 len1 )
  s" le" rot plural-ending s+  ;
  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»).

: verb-number-ending  ( a -- ca1 len1 )
  s" n" rot has-plural-name? and  ;
  \ Devuelve la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.

: verb-number-ending+  ( ca1 len1 a -- ca2 len2 )
  verb-number-ending s+  ;
  \ Añade a una cadena la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.

: proper-verb-form  ( a ca1 len1 -- ca2 len2 )
  rot has-plural-name? *>verb-ending  ;
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular un verbo
  \ cuyo sujeto se indica con el identificador de su entidad.
  \ ca len = Expresión
  \ a = Entidad
  \ XXX TODO -- no usado

: proper-grammar-number  ( a ca1 len1 -- ca2 len2 )
  rot has-plural-name? *>plural-ending  ;
  \ Cambia por «s» (terminación del plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular las palabras de un texto,
  \ cuyo número gramatical se indica con el identificador de una entidad.
  \ ca len = Expresión
  \ a = Entidad
  \ XXX TODO -- no usado

\ ----------------------------------------------
\ Interfaz para los nombres de los entes

\ Como ya se explicó, el nombre de cada ente se guarda en una
\ cadena dinámica [que se crea en la memoria con `allocate`, no
\ en el espacio del diccionario del sistema].  El manejo de
\ estas cadenas dinámicas se hace con el módulo
\ correspondiente de Forth Foundation Library.
\
\ En la ficha del ente se guarda solo la dirección de la
\ cadena dinámica, en el campo `~name-str`.  Por ello hacen
\ falta palabras que hagan de interfaz para gestionar los
\ nombres de ente de forma análoga a como se hace con el resto
\ de datos de su ficha.

: name!  ( ca len a -- )  name-str str-set  ;
  \ Guarda el nombre _ca len_ de un ente _a_.

: p-name!  ( ca len a -- )  dup has-plural-name name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ y lo marca como plural.

: s-name!  ( ca len a -- )  dup has-singular-name name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ y lo marca como singular.

: fs-name!  ( ca len a -- )  dup has-feminine-name s-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical femenino y singular.

: fp-name!  ( ca len a -- )  dup has-feminine-name p-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical femenino y plural.

: ms-name!  ( ca len a -- )  dup has-masculine-name s-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical masculino y singular.

: mp-name!  ( ca len a -- )  dup has-masculine-name p-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical masculino y plural.

: name  ( a -- ca len )  name-str str-get  ;
  \ Devuelve el nombre _ca len_ de un ente _a_.

: ?name  ( a -- ca len )  ?dup if  name  else  null$  then  ;
  \ Devuelve el nombre _ca len_ de un ente _a_, si es tal;
  \ devuelve una cadena vacía si _a_ es cero.
  \ XXX TMP -- solo se usa para depuración

: ?.name  ( a|0 -- )  ?name type  ;
  \ Imprime el nombre de un ente _a_, si es tal;
  \ imprime una cadena vacía si _a_ es cero.
  \ XXX TMP -- solo se usa para depuración

: ^name  ( a -- ca1 len1 )  name ^uppercase  ;
  \ Devuelve el nombre de un ente, con la primera letra mayúscula.

: name&  ( a ca1 len1 -- ca2 len2 )  rot name s&  ;
  \ Añade a un (supuesto) artículo _ca1 len1_ el nombre de un ente _a_,
  \ formando el nombre completo _ca2 len2_.

: full-name  ( a -- ca len )  dup article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con el artículo que le corresponda.

: ^full-name  ( a -- ca len )  full-name ^uppercase  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con el artículo que le corresponda (con la primera letra en mayúscula).

: defined-full-name  ( a -- ca len )  dup definite-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un artículo definido.

: undefined-full-name  ( a -- ca len )  dup undefined-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un artículo indefinido.

: negative-full-name  ( a -- ca len )  dup negative-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un «artículo negativo».

: distant-full-name  ( a -- ca len )  dup distant-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un «artículo distante».

: nonhuman-subjective-negative-name  ( a -- ca len )
  negative-full-name 2>r
  s{  2r> 2dup 2dup  \ Tres nombres repetidos con «artículo negativo»
      s" eso" s" esa cosa" s" tal cosa"  \ Tres alternativas
  }s  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente (no
  \ humano) _a_, desde el punto de vista del protagonista.  Nota: En
  \ este caso hay que usar `negative-full-name` antes de `s{` y pasar
  \ la cadena mediante la pila de retorno; de otro modo `s{` y `}s` no
  \ pueden calcular bien el crecimiento de la pila.

: human-subjective-negative-name  ( a -- ca len )
  dup is-known? if  full-name  else  drop s" nadie"  then  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente
  \ (humano) _a_, desde el punto de vista del protagonista.

: subjective-negative-name  ( a -- ca len )
  dup is-human?
  if  human-subjective-negative-name
  else  nonhuman-subjective-negative-name  then  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente _a_,
  \ desde el punto de vista del protagonista.

: /l$  ( a -- ca len )  s" l" rot has-personal-name? 0= and  ;
  \ Devuelve en _ca len_ la terminación «l» del artículo determinado
  \ masculino para añadirla a la preposición «a», si un ente humano
  \ _a_ lo requiere para ser usado como objeto directo; o una cadena
  \ vacía.
  \ XXX TODO -- no usado

: a/$  ( a -- ca len )  s" a" rot is-human? and  ;
  \ Devuelve la preposición «a» en _ca len_ si un ente _a_ lo requiere
  \ para ser usado como objeto directo; o una cadena vacía.

: a/l$  ( a -- ca len )  a/$ dup if  /l$ s+  then  ;
  \ Devuelve la preposición «a» en _ca len_, con posible artículo
  \ determinado, si un ente _a_ lo requiere para ser usado como objeto
  \ directo.
  \ XXX TODO -- no usado

: subjective-negative-name-as-direct-object  ( a -- ca len )
  dup a/$ rot subjective-negative-name s&  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente _a_,
  \ desde el punto de vista del protagonista, para ser usado como
  \ objeto directo.

: .full-name  ( a -- )  full-name paragraph  ;
  \ Imprime el nombre completo de un ente.
  \ XXX TODO -- no usado

\ ----------------------------------------------
\ Otros campos calculados

: «open»|«closed»  ( a -- ca len )
  dup is-open? if  s" abiert"  else  s" cerrad"  then
  rot noun-ending s+  ;
  \ Devuelve en _ca len_ «abierto/a/s» o «cerrado/a/s»,
  \ según corresponda a un ente _a_.

\ ==============================================================
\ Herramientas para crear las fichas de la base de datos

\ No es posible reservar el espacio necesario para las fichas
\ hasta saber cuántas necesitaremos (a menos que usáramos una
\ estructura un poco más sofisticada con fichas separadas pero
\ enlazadas entre sí, muy habitual también y fácil de crear).  Por
\ ello la palabra "'entities" (que devuelve la dirección de la
\ base de datos) se crea como un vector, para asignarle
\ posteriormente su dirección de ejecución.  Esto permite crear un
\ nuevo ente fácilmente, sin necesidad de asignar previamente el
\ número de fichas a una constante.

defer 'entities  ( -- a )
  \ Dirección de los entes.

0 value #entities
  \ Contador de entes.

: #>entity  ( u -- a )  /entity * 'entities +  ;
  \ Devuelve la dirección de la ficha de un ente a partir de su número ordinal
  \ (el número del primer ente es el cero).

: entity>#  ( a -- u )  'entities - /entity /  ;
  \ Devuelve el número ordinal de un ente (el primero es el cero)
  \ a partir de la dirección de su ficha.

: entity:  ( "name" -- )
  create
    #entities ,  \ Guardar la cuenta en el cuerpo de la palabra recién creada
    #entities 1+ to #entities  \ Actualizar el contador
  does>  ( pfa -- a )
    @ #>entity  ;  \ El identificador devolverá la dirección de su ficha
  \ Crea un nuevo identificador de ente,
  \ que devolverá la dirección de su ficha.

: erase-entity  ( a -- )  /entity erase  ;
  \ Rellena con ceros la ficha de un ente.

: backup-entity  ( a -- x0 x1 x2 x3 x4 x5 x6 )
  >r
  r@ description-xt
  r@ init-xt
  r@ can-i-enter-location-xt
  r@ after-describing-location-xt
  r@ after-listing-entities-xt
  r@ before-describing-location-xt
  r@ before-leaving-location-xt
  r> name-str  ;
  \ Respalda los datos de un ente _a_
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético, para facilitar la edición).

: restore-entity  ( x0 x1 x2 x3 x4 x5 x6 a -- )
  >r
  r@ ~name-str !
  r@ ~before-leaving-location-xt !
  r@ ~before-describing-location-xt !
  r@ ~after-listing-entities-xt !
  r@ ~after-describing-location-xt !
  r@ ~can-i-enter-location-xt !
  r@ ~init-xt !
  r> ~description-xt !  ;
  \ Restaura los datos de un ente _a_
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético inverso, para facilitar la edición).

: setup-entity  ( a -- )
  >r r@ backup-entity  r@ erase-entity  r> restore-entity  ;
  \ Prepara la ficha de un ente para ser completada con sus datos .

0 value self~
  \ Ente cuyos atributos, descripción o trama están siendo definidos
  \ (usado para aligerar la sintaxis).

: :name-str  ( a -- )
  [debug-init] [if]  s" Inicio de :NAME-STR" debug [then]
  dup name-str ?dup
  [debug-init] [if]  s" A punto para STR-FREE" debug [then]
  ?? str-free
  str-new swap ~name-str !
  [debug-init] [if]  s" Final de :NAME-STR" debug [then]  ;
  \ Crea una cadena dinámica nueva para guardar el nombre del ente.

: [:attributes]  ( a -- )
  dup to self~  \ Actualizar el puntero al ente
  dup :name-str  \ Crear una cadena dinámica para el campo `~name-str`
  setup-entity  ;
  \ Inicia la definición de propiedades de un ente.  Esta palabra se
  \ ejecuta cada vez que hay que restaurar los datos del ente, y antes
  \ de la definición de atributos contenida en la palabra
  \ correspondiente al ente.  El identificador del ente está en la
  \ pila porque se compiló con `literal` cuando se creó la palabra de
  \ atributos.

: default-description  ( -- )  ^is-normal$ paragraph  ;
  \ Descripción predeterminada de los entes
  \ para los que no se ha creado una palabra propia de descripción.

: (:attributes)  ( a xt -- )
  over ~init-xt !  \ Conservar la dirección de ejecución en la ficha del ente
  ['] default-description over ~description-xt !  \ Poner la descripción predeterminada
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self~` en tiempo de ejecución
  \ Operaciones preliminares para la definición de atributos de un ente.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define sus atributos.
  \ a = Ente para la definición de cuyos atributos se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada

: noname-roll  ( a xt colon-sys -- colon-sys a xt )  5 roll 5 roll  ;
  \ Mueve los parámetros que nos interesan a la parte alta de la pila;
  \ se usa tras crear con `:noname` una palabra relativa a un ente.
  \ Esta palabra es necesaria porque `:noname` deja en la pila
  \ sus valores de control (colon-sys), que en el caso de Gforth
  \ son tres elementos, para ser consumidos al finalizar la compilación de la
  \ palabra creada.
  \ a = Ente para el que se creó la palabra
  \ xt = Dirección de ejecución de la palabra sin nombre creada por `:noname`
  \ colon-sys = Valores de control dejados por `:noname`

: :attributes  ( a -- )
  :noname noname-roll
  (:attributes)  \ Crear la palabra y hacer las operaciones preliminares
  postpone [:attributes]  ;  \ Compilar la palabra `[:attributes]` en la palabra creada, para que se ejecute cuando sea llamada
  \ Inicia la creación de una palabra sin nombre que definirá las
  \ propiedades de un ente.

: ;attributes  ( sys-col -- )  postpone ;  ;  immediate

: init-entity  ( a -- )
  [debug-init] [if]  s" Inicio de INIT-ENTITY" debug dup entity># cr ." Entity=" .  [then]
  init-xt
  [debug-init] [if]  s" Antes de EXECUTE" debug  [then]
  execute
  [debug-init] [if]  s" Final de INIT-ENTITY" debug  [then]  ;
  \ Restaura la ficha de un ente a su estado original.

: init-entities  ( -- )
  #entities 0 do
    [debug-init] [if]  i cr ." about to init entity #" .  [then]
    i #>entity init-entity
    \ i #>entity full-name space type ?.s  \ XXX INFORMER
  loop  ;
  \ Restaura las fichas de los entes a su estado original.

\ ==============================================================
\ Herramientas para crear las descripciones

\ No almacenamos las descripciones en la base de datos junto
\ con el resto de atributos de los entes, sino que para cada
\ ente creamos una palabra que imprime su descripción, lo que
\ es mucho más flexible: La descripción podrá variar en
\ función del desarrollo del juego y adaptarse a las
\ circunstancias, e incluso sustituir en algunos casos al
\ código que controla la trama del juego.
\
\ Así pues, lo que almacenamos en la ficha del ente, en el
\ campo `~description-xt`, es la dirección de ejecución de la
\ palabra que imprime su descripción.
\
\ Por tanto, para describir un ente basta tomar de su ficha el
\ contenido de `~description-xt`, y llamar a `execute`.

false value sight
  \ Ente dirección al que se mira en un escenario
  \ (o el propio ente escenario); se usa en las palabras de
  \ descripción de escenarios

: [:description]  ( a -- )  to self~  ;
  \ Operaciones previas a la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al comienzo de la palabra de descripción.
  \ El identificador del ente está en la pila porque se compiló con
  \ `literal` cuando se creó la palabra de descripción.  Actualmente
  \ lo único que hace es actualizar el puntero al ente, usado para
  \ aligerar la sintaxis.

: (:description)  ( a xt -- )
  over ~description-xt !
  postpone literal  ;
  \ Operaciones preliminares para la definición de la descripción
  \ _xt_ de
  \ un ente _a_,
  \ Esta palabra solo se ejecuta una vez para cada ente, al inicio de
  \ la compilación del código de la palabra que crea su descripción.
  \ Hace dos operaciones: 1) Conservar la dirección de ejecución en la
  \ ficha del ente; 2) Compilar el identificador de ente en la palabra
  \ de descripción recién creada, para que `[:description]` lo guarde
  \ en `self~` en tiempo de ejecución.

: :description  ( a -- )
  :noname noname-roll  (:description)  postpone [:description]  ;
  \ Inicia la definición de una palabra de descripción para un ente
  \ _a_.

: [;description]  ( -- )  false to sight  ;
  \ Operaciones finales tras la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al final de la palabra de descripción.
  \ Pone a cero el selector de vista, para evitar posibles errores.

: ;description  ( colon-sys -- )
  postpone [;description]  postpone ;  ; immediate
  \ Termina la definición de una palabra de descripción de un ente.

: (describe)  ( a -- )  ~description-xt perform  ;
  \ Ejecuta la palabra de descripción de un ente _a_.

: .location-name  ( a -- )
  [debug-map] [if]  dup  [then]
  name ^uppercase location-name-color paragraph
  [debug-map] [if]
    entity># location-01~ entity># - 1+ ."  [ location #" . ." ]"
  [then]  ;
  \ Imprime el nombre de un ente escenario, como cabecera de su descripción.
  \ XXX TODO -- añadir el artículo correspondiente o no, dependiendo
  \ de un indicador de la ficha:
  \     pasaje de la serpiente -- el pasaje de la serpiente
  \     el paso del Perro -- el paso del Perro
  \     un tramo de cueva -- un tramo de cueva
  \     un lago interior -- el lago interior
  \     hogar de Ambrosio -- el hogar de Ambrosio

: (describe-location)  ( a -- )
  dup to sight  location-description-color (describe)  ;
  \ Describe un ente escenario _a_.

: describe-location  ( a -- )
  clear-screen-for-location dup .location-name  (describe-location)  ;
  \ Describe un ente escenario _a_,
  \ con borrado de pantalla y título.

: describe-other  ( a -- )  description-color (describe)  ;
  \ Describe un ente de otro tipo.

: describe-direction  ( a -- )
  to sight  my-location describe-other  ;
  \ Describe un ente dirección _a_.

: description-type  ( a -- u )
  dup is-location? abs
  [true] [if]
    swap is-direction? 2 and +
  [else]
    \ XXX TODO -- terminar; no usado
    over is-direction? 2 and +
    swap exits~ = 4 and +
  [then]  ;
  \ Convierte un ente _a_ en el tipo de descripción _u_ que requiere:
  \ 4=salida, 2=dirección, 1=escenario, 0=otros, 3=¡error!.  Un
  \ resultado de 3 significaría que el ente es a la vez dirección y
  \ escenario.

: describe  ( a -- )
  dup description-type
  case
    0 of  describe-other  endof
    1 of  describe-location  endof
    2 of  describe-direction  endof
    abort" Error fatal en `describe`: dato incorrecto"  \ XXX INFORMER
  endcase  ;
  \ Describe un ente _a_, según su tipo.

: uninteresting-direction  ( -- )  uninteresting-direction$ paragraph  ;
  \ Muestra la descripción de la direcciones que no tienen nada especial.

\ vim:filetype=gforth:fileencoding=utf-8
