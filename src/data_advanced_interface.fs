\ data_advanced_interface.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201708161140
\ See change log at the end of the file

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-false.fs    \ `[false]`
require galope/bracket-true.fs     \ `[true]`
require galope/s-curly-bracket.fs  \ `s{`
require galope/replaced.fs         \ `replaced`
require galope/txt-plus.fs         \ `txt+`

set-current

\ ==============================================================
\ Interfaz de datos avanzada

\ Esta interfaz de datos depende de algunos identificadores de entes.

: belongs-to-protagonist?  ( a -- f )  protagonist~ is-owner?  ;
: belongs-to-protagonist  ( a -- )  protagonist~ be-owner  ;

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

\ XXX TODO -- write `is-carried` (also hold inside a container);
\ or rename `is-hold` to `is-hold-by-hand`?

: is-not-hold?  ( a -- f )  is-hold? 0=  ;
  \ ¿No es el protagonista la localización de un ente?

: be-hold  ( a -- )  ~location protagonist~ swap !  ;
  \ Hace que el protagonista sea la localización de un ente _a_.

: is-worn-by-me?  ( a -- f )  dup is-hold?  swap is-worn?  and  ;
  \ ¿El protagonista lleva puesto el ente indicado?

: is-not-worn-by-me?  ( a -- f )  is-worn-by-me? 0=  ;

: is-known?  ( a -- f )
  dup belongs-to-protagonist?
  over is-visited? or
  over conversations? or
  swap is-familiar?  or  ;
  \ ¿El protagonista ya conoce el ente?  El resultado depende de
  \ cualquiera de cuatro condiciones: 1) ¿Es propiedad del
  \ protagonista?; 2) ¿Es un escenario ya visitado? (si no es un
  \ escenario, la comprobación no tendrá efecto); 3) ¿Ha hablado ya
  \ con él? (si no es un personaje, la comprobación no tendrá efecto);
  \ 4) ¿O ya le es familiar?.

: is-unknown?  ( a -- f )  is-known? 0=  ;
  \ ¿El protagonista aún no conoce el ente?

: is-here?  ( a -- f )
  dup location am-i-there?
  over is-global-outdoor? am-i-outdoor? and or
  swap is-global-indoor? am-i-indoor? and or  ;
  \ ¿Está un ente en la misma localización que el protagonista?
  \ El resultado depende de cualquiera de tres condiciones:
  \ 1) ¿Está efectivamente en la misma localización?;
  \ 2) ¿Es un «global exterior» y estamos en un escenario exterior?;
  \ 3) ¿Es un «global interior» y estamos en un escenario interior?.

: is-not-here?  ( a -- f )  is-here? 0=  ;
  \ ¿Está un ente en otra localización que la del protagonista?
  \ XXX TODO -- no usado

: is-here-and-unknown?  ( a -- f )  dup is-here? swap is-unknown? and  ;
  \ ¿Está un ente en la misma localización que el protagonista y aún
  \ no es conocido por él?

: be-here  ( a -- )  my-location swap be-there  ;
  \ Hace que un ente esté en la misma localización que el protagonista.

: is-accessible?  ( a -- f )  dup is-hold?  swap is-here?  or  ;
  \ ¿Es un ente accesible para el protagonista?

: is-not-accessible?  ( a -- f )  is-accessible? 0=  ;
  \ ¿Un ente no es accesible para el protagonista?

: must-be-listed?  ( a -- f )
  dup protagonist~ <>  \ ¿No es el protagonista?
  over is-decoration? 0=  and  \ ¿Y no es decorativo?
  over is-listed? and  \ ¿Y puede ser listado?
  swap is-global? 0=  and  ;  \ ¿Y no es global?
  \ ¿El ente debe ser incluido en las listas?
  \ XXX TODO -- inconcluso

: can-be-looked-at?  ( a -- f )
  dup am-i-there?    ?dup if  nip exit  then
  dup is-direction?  ?dup if  nip exit  then
  dup exits~ =       ?dup if  nip exit  then
  is-accessible?  ;
  \ ¿El ente puede ser mirado?

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
  \ ¿Llevas algo prohibido?  Este cálculo se usa en varios lugares del
  \ programa, en relación a los refugiados.

: no-torch?  ( -- f )
  torch~ is-not-accessible?  torch~ is-not-lighted?  or  ;
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
\ Gestión de nombres

get-current forth-wordlist set-current

\ Talanto
\ http://programandala.net/en.program.talanto.html

require talanto/naming.es.fs

set-current

\ ----------------------------------------------
\ Otros campos calculados

: «open»|«closed»  ( a -- ca len )
  dup is-open? if  s" abiert"  else  s" cerrad"  then
  rot noun-ending s+  ;
  \ Devuelve en _ca len_ «abierto/a/s» o «cerrado/a/s»,
  \ según corresponda a un ente _a_.

: was-the-cave-entrance-discovered?  ( -- f )
  location-08~ has-south-exit?  ;
  \ ¿La entrada a la cueva ya fue descubierta?

\ ==============================================================
\ Herramientas para crear conexiones entre escenarios

\ XXX TODO -- move to Flibustre when possible

0 [if]  \ XXX TODO -- inconcluso

create opposite-exits
south-exit> ,
north-exit> ,
west-exit> ,
east-exit> ,
down-exit> ,
up-exit> ,
in-exit> ,
out-exit> ,

create opposite-direction-entities
south~ ,
north~ ,
west~ ,
east~ ,
down~ ,
up~ ,
in~ ,
out~ ,

[then]

\ Necesitamos una tabla que nos permita traducir esto:
\
\ ENTRADA: Un puntero correspondiente a un campo de dirección
\ de salida en la ficha de un ente.
\
\ SALIDA: El identificador del ente dirección al que se
\ refiere esa salida.

create exits-table  #exits cells allot
  \ Tabla de traducción de salidas.

: >exits-table>  ( n -- a )  first-exit> - exits-table +  ;
  \ Convierte el campo de dirección _n_ (por tanto, un desplazamiento
  \ relativo al inicio de la ficha de un ente) en la dirección _a_ del
  \ ente dirección correspondiente en la tabla.

: exits-table!  ( a n -- )  >exits-table> !  ;
  \ Guarda un ente _a_ en una posición _n_ de la tabla de salidas,
  \ siendo _n_ también un campo de dirección (por tanto, un
  \ desplazamiento relativo al inicio de la ficha de un ente).

: exits-table@  ( n -- a )  >exits-table> @  ;
  \ Convierte el campo de dirección _n_ (por tanto, un desplazamiento
  \ relativo al inicio de la ficha de un ente) en la dirección _a_ del
  \ ente dirección correspondiente en la tabla.

\ Rellenar cada elemento de la tabla con un ente de salida, usando
\ como puntero el campo análogo de la ficha.  Haciéndolo de esta
\ manera no importa el orden en que se rellenen los elementos.

north~ north-exit> exits-table!
south~ south-exit> exits-table!
east~ east-exit> exits-table!
west~ west-exit> exits-table!
up~ up-exit> exits-table!
down~ down-exit> exits-table!
out~ out-exit> exits-table!
in~ in-exit> exits-table!

0 [if]  \ XXX TODO -- inconcluso
: opposite-exit  ( a1 -- a2 )
  first-exit> - opposite-exits + @  ;
  \ Devuelve la dirección cardinal opuesta a la indicada.

: opposite-exit~  ( a1 -- a2 )
  first-exit> - opposite-direction-entities + @  ;
  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada.
  \ a1 = entidad de dirección
  \ a2 = entidad de dirección, opuesta a a1

[then]

require talanto/location_connectors.fs

\ Por último, definimos dos palabras para hacer
\ todas las asignaciones de salidas en un solo paso.

: set-exits  ( a1 ... a8 a0 -- )
  >r r@ ~out-exit !
     r@ ~in-exit !
     r@ ~down-exit !
     r@ ~up-exit !
     r@ ~west-exit !
     r@ ~east-exit !
     r@ ~south-exit !
     r> ~north-exit !  ;
  \ Asigna todas las salidas _a1 ... a8_ de un ente escenario _a0_.
  \ Los entes de salida _a1 ... a8_ (o cero) están en el orden
  \ habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera.

: exit-from-here  ( a1 -- a2 | 0 )
  direction my-location + @  ;
  \ Devuelve el ente _a2_ al que conduce el ente dirección _a1_ desde
  \ el escenario del protagonista, o bien devuelve cero si no hay
  \ salida en esa dirección.


\ ==============================================================
\ Operaciones con conexiones entre escenarios

: open-the-cave-entrance  ( -- )
  location-08~ dup location-10~ s<-->  location-10~ i<-->  ;
  \ Comunica el escenario 8 con el 10 (de dos formas y en ambos sentidos).

\ ==============================================================
\ Cálculos para averiguar complemento omitido

: whom  ( -- a | 0 )
  ambrosio~ dup is-here? and ?dup ?exit
  leader~   dup is-here? and ?dup ?exit
  false  ;
  \ Devuelve un ente personaje _a_ al que probablemente se refiera un
  \ comando.  Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica.
  \
  \ XXX TODO -- ampliar para contemplar los soldados y oficiales,
  \ según la trama, el escenario y la fase de la batalla

: unknown-whom  ( -- a | 0 )
  ambrosio~ dup is-here-and-unknown? and ?dup ?exit
  leader~   dup is-here-and-unknown? and ?dup ?exit
  false  ;
  \ Devuelve un ente personaje desconocido al que probablemente se
  \ refiera un comando.  Se usa para averiguar el objeto de algunas
  \ acciones cuando el jugador no lo especifica

\ ==============================================================
\ Change log

\ 2017-08-05: Move the location connectors to Talanto.
\
\ 2017-08-16: Move the naming tools to Talanto.

\ vim:filetype=gforth:fileencoding=utf-8
