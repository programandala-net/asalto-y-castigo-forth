\ data_basic_interface.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607080928

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Interfaz de datos básica

\ ----------------------------------------------
\ Herramientas para los campos de dirección

' ~north-exit alias ~first-exit
  \ Primera salida definida en la ficha.

' ~in-exit alias ~last-exit
  \ Última salida definida en la ficha.

\ Guardar el desplazamiento de cada campo de dirección respecto al
\ primero de ellos:
0 ~first-exit constant first-exit>
0 ~last-exit  constant last-exit>
0 ~north-exit constant north-exit>
0 ~south-exit constant south-exit>
0 ~east-exit  constant east-exit>
0 ~west-exit  constant west-exit>
0 ~up-exit    constant up-exit>
0 ~down-exit  constant down-exit>
0 ~out-exit   constant out-exit>
0 ~in-exit    constant in-exit>

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
\ Lectores de campos

: name  ( a -- ca len )  ~name 2@  ;

: break-error#  ( a -- u )  ~break-error# @  ;
: conversations  ( a -- u )  ~conversations @  ;
: description-xt  ( a -- xt )  ~description-xt @  ;
: direction  ( a -- u )  ~direction @  ;
: familiar  ( a -- u )  ~familiar @  ;
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
: is-wearable?  ( a -- f )  ~is-wearable bit@  ;
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
: times-open  ( a -- u )  ~times-open @  ;
: owner  ( a1 -- a2 )  ~owner @  ;
: is-vegetal?  ( a -- f )  ~is-vegetal bit@  ;
: is-worn?  ( a -- f )  ~is-worn bit@  ;
: location  ( a1 -- a2 )  ~location @  ;
: can-be-entered-xt  ( a -- xt )  ~can-be-entered-xt @  ;
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

: is-direction?  ( a -- f )  direction 0<>  ;
: is-familiar?  ( a -- f )  familiar 0>  ;
: is-visited?  ( a -- f )  visits 0>  ;
: is-not-visited?  ( a -- f )  visits 0=  ;
: conversations?  ( a -- f )  conversations 0<>  ;
: no-conversations?  ( a -- f )  conversations 0=  ;
: has-north-exit?  ( a -- f )  north-exit exit?  ;
: has-south-exit?  ( a -- f )  south-exit exit?  ;
: has-east-exit?  ( a -- f )  east-exit exit?  ;
: has-west-exit?  ( a -- f )  west-exit exit?  ;

: is-owner?  ( a1 a2 -- f )  swap owner =  ;
  \ ¿Es el ente _a1_ propiedad del ente _a2_?

: is-there?  ( a1 a2 -- f )  location =  ;
  \ ¿Está el ente _a1_ localizado en el ente _a2_?

: was-there?  ( a1 a2 -- f )  previous-location =  ;
  \ ¿Estuvo el ente _a1_ localizado en el ente _a2_?

: is-global?  ( a -- f )
  dup is-global-outdoor? swap is-global-indoor? or  ;
  \ ¿Es el ente un ente global?

: is-beast?  ( a -- f )
  dup is-animal?  swap is-human? or  ;
  \ ¿El ente es un animal (incluyendo humano)?

: is-living-being?  ( a -- f )
  dup is-vegetal?  swap is-beast? or  ;
  \ ¿El ente es un ser vivo (aunque esté muerto)?

: can-be-taken?  ( a -- f )
  dup is-decoration?
  over is-human? or
  swap is-character? or 0=  ;
  \ ¿El ente puede ser tomado?  Se usa como norma general, para
  \ aquellos entes que no tienen un error específico indicado en el
  \ campo `~take-error#`.

\ ----------------------------------------------
\ Modificadores de campos

: name!  ( ca len a -- )  ~name 2! ;

: be-before-describing-location-xt  ( xt a -- )  ~before-describing-location-xt !  ;
: be-description-xt  ( xt a -- )  ~description-xt !  ;
: be-after-describing-location-xt  ( xt a -- )  ~after-describing-location-xt !  ;
: be-after-listing-entities-xt  ( xt a -- )  ~after-listing-entities-xt !  ;
: be-before-leaving-location-xt  ( xt a -- )  ~before-leaving-location-xt !  ;

: have-definite-article  ( a -- )  ~has-definite-article bit-on  ;
: have-feminine-name  ( a -- )  ~has-feminine-name bit-on  ;
: have-masculine-name  ( a -- )  ~has-feminine-name bit-off  ;
: have-no-article  ( a -- )  ~has-no-article bit-on  ;
: have-personal-name  ( a -- )  ~has-personal-name bit-on  ;
: have-plural-name  ( a -- )  ~has-plural-name bit-on  ;
: have-singular-name  ( a -- )  ~has-plural-name bit-off  ;

: be-character  ( a -- )  ~is-character bit-on  ;
: be-animal  ( a -- )  ~is-animal bit-on  ;

: be-light  ( a -- )  ~is-light bit-on  ;
: be-lit  ( a -- )  ~is-lit bit-on  ;
: be-not-lit  ( a -- )  ~is-lit bit-off  ;

: be-wearable  ( a -- )  ~is-wearable bit-on  ;

: be-not-listed  ( a -- f )  ~is-not-listed bit-on  ;
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
: conversations++  ( a -- )  ~conversations ?++  ;
: familiar++  ( a -- )  ~familiar ?++  ;

: be-owner  ( a1 a2 -- )  swap ~owner !  ;

: be-there  ( a1 a2 -- )  ~location !  ;

\ vim:filetype=gforth:fileencoding=utf-8
