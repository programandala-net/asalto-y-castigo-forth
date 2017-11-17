\ data_tools.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711171347
\ See change log at the end of the file

\ Note: Most comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-true.fs  \ `[true]`

set-current

\ ==============================================================
\ Herramientas para crear las fichas de la base de datos

' ~initializer is initializer-entity-field  ( a1 -- a2 )
  \ Configure the deferred word defined in the `entity` module of
  \ Flibustre.
  \ XXX OLD -- remove after moving the data structure to Flibustre

\ ==============================================================
\ Herramientas para mostrar las descripciones

\ En el campo `~describer` de cada ente se almacena la dirección
\ de ejecución de una palabra que imprime su descripción.

false value sight
  \ Ente dirección al que se mira en un escenario
  \ (o el propio ente escenario); se usa en las palabras de
  \ descripción de escenarios

defer default-description  ( -- )
  \ Descripción predeterminada de los entes
  \ para los que no se ha creado una palabra propia de descripción.

: (default-description)  ( -- )  ^is-normal$ /ltype  ;
  \ Comportamiento predeterminado de la descripción predeterminada de
  \ los entes para los que no se ha creado una palabra propia de
  \ descripción.

' (default-description) is default-description

: (describe)  ( a -- )
  describer ?dup if  execute
                     else  default-description  then  ;
  \ Ejecuta la palabra de descripción de un ente _a_, si está
  \ inicializada; en caso contrario ejecuta la descripción
  \ predeterminada.

: .location-name  ( a -- )
  [debug-map] [if]  dup  [then]
  name xcapitalized location-name-color /ltype
  \ [debug-map] [ true or ] [if]
  [true] [if]
    dup entity># location-01~ entity># - 1+ ."  [#" 0 .r ." ]"
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
  clear-screen-for-location
  dup .location-name  (describe-location)  ;
  \ Describe un ente escenario _a_,
  \ con borrado de pantalla y título.

: describe-other  ( a -- )  description-color (describe)  ;
  \ Describe un ente de otro tipo.

: describe-direction  ( a -- )
  to sight  my-holder describe-other  ;
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
  \ XXX TODO -- rewrite

: uninteresting-direction  ( -- )
  uninteresting-direction$ /ltype  ;
  \ Muestra la descripción de la direcciones que no tienen nada especial.

\ ==============================================================
\ Change log

\ 2017-11-10: Update to Talanto 0.62.0: replace field notation
\ "location" with "holder".
\
\ 2017-11-17: Update from Galope's deprecated module <print.fs> to
\ <l-type.fs>.

\ vim:filetype=gforth:fileencoding=utf-8
