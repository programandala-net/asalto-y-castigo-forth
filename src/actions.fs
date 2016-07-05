\ actions.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607051141

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Mensaje de acción completada

\ XXX TODO -- mover a Flibustre

variable silent-well-done
  \ XXX TODO -- no usado

: well-done-this  ( ca len -- )
  silent-well-done @ 0= if  narrate  else  2drop  then
  silent-well-done off  ;
  \ Informa, con un mensaje específico, de que una acción se ha realizado,
  \ si es preciso.

defer well-done$  ( -- ca len )
  \ Mensaje genérico, de que una acción se ha realizado.

:noname  ( -- ca len )
  s{ s" Hecho." s" Bien." }s  ; is well-done$
  \ Mensaje genérico, de que una acción se ha realizado.

: well-done  ( -- )  well-done$ well-done-this  ;
  \ Informa, con un mensaje genérico, de que una acción se ha realizado,
  \ si es preciso.

\ ==============================================================
\ Comprobación de los requisitos de las acciones

\ XXX TODO -- mover a Flibustre

\ En las siguientes palabras usamos las llaves en sus nombres
\ como una notación, para hacer más legible y más fácil de
\ modificar el código.  El texto entre las llaves indica la
\ condición que se ha de cumplir.
\
\ Si la condición no se cumple, se provocará un error con
\ `throw` que devolverá el flujo al último `catch`.
\
\ Este sistema de filtros y errores permite simplificar el
\ código de las acciones porque ahorra muchas estructuras
\ condicionales anidadas.

: main-complement{forbidden}  ( -- )
  main-complement @
  0<> unexpected-main-complement-error# and throw  ;
  \ Provoca un error si hay complemento principal.

: secondary-complement{forbidden}  ( -- )
  secondary-complement @
  0<> unexpected-secondary-complement-error# and throw  ;
  \ Provoca un error si hay complemento secundario.

: main-complement{required}  ( -- )
  main-complement @
  0= no-main-complement-error# and throw  ;
  \ Provoca un error si no hay complemento principal.

: main-complement{this-only}  ( a -- )
  main-complement @ swap over different?
  not-allowed-main-complement-error# and throw
  ;
  \ Provoca un error si hay complemento principal y no es el indicado.
  \ a = Ente que será aceptado como complemento

: different-tool?  ( a -- f )
  tool-complement @ swap over different?  ;
  \ ¿Es el ente _a_ diferente a la herramienta usada, si la hay?

: different-actual-tool?  ( a -- f )
  actual-tool-complement @ swap over different?  ;
  \ ¿Es el ente _a_ diferente a la herramienta estricta usada, si la hay?

: tool-complement{this-only}  ( a -- )
  different-tool? not-allowed-tool-complement-error# and throw  ;
  \ Provoca un error (lingüístico)
  \ si hay complemento instrumental y no es el ente _a_.

: actual-tool-complement{this-only}  ( a -- )
  different-actual-tool? not-allowed-tool-complement-error# and throw  ;
  \ Provoca un error (lingüístico)
  \ si hay complemento instrumental estricto y no es el ente _a_.

: tool{not-this}  ( a -- )
  dup what !
  different-tool? 0= useless-what-tool-error# and throw  ;
  \ Provoca un error (narrativo) si se usa cierta herramienta.
  \ a = Ente que no será aceptado como herramienta
  \ XXX TODO -- no usado

: actual-tool{not-this}  ( a -- )
  dup what !
  different-actual-tool? 0= useless-what-tool-error# and throw  ;
  \ Provoca un error (narrativo) si se usa cierta herramienta estricta.
  \ a = Ente que no será aceptado como herramienta estricta
  \ XXX TODO -- no usado

: tool{this-only}  ( a -- )
  tool-complement @ what !
  different-tool? useless-what-tool-error# and throw  ;
  \ Provoca un error (narrativo) si no se usa cierta herramienta.
  \ a = Ente que será aceptado como herramienta

: actual-tool{this-only}  ( a -- )
  actual-tool-complement @ what !
  different-actual-tool? useless-what-tool-error# and throw  ;
  \ Provoca un error (narrativo) si no se usa cierta herramienta estricta.
  \ a = Ente que será aceptado como herramienta estricta

: tool-complement{unnecessary}  ( -- )
  tool-complement @ ?dup ?? unnecessary-tool  ;
  \ Provoca un error si hay un complemento instrumental.

: actual-tool-complement{unnecessary}  ( -- )
  actual-tool-complement @ ?dup ?? unnecessary-tool  ;
  \ Provoca un error si hay un complemento instrumental estricto.

: tool-complement{unnecessary-for-that}  ( ca len -- )
  tool-complement @ ?dup
  if  unnecessary-tool-for-that  else  2drop  then  ;
  \ Provoca un error si hay un complemento instrumental.
  \ ca len = Acción para la que sobra el complemento
  \       (una frase con verbo en infinitivo)

: actual-tool-complement{unnecessary-for-that}  ( ca len -- )
  actual-tool-complement @ ?dup
  if  unnecessary-tool-for-that  else  2drop  then  ;
  \ Provoca un error si hay un complemento instrumental estricto.
  \ ca len = Acción para la que sobra el complemento
  \       (una frase con verbo en infinitivo)

: {hold}  ( a -- )
  dup what !
  is-hold? 0= you-do-not-have-what-error# and throw  ;
  \ Provoca un error si un ente no está en inventario.

: ?{hold}  ( a | 0 -- )  ?dup ?? {hold}  ;
  \ Provoca un error si un supuesto ente lo es y no está en inventario.

: main-complement{hold}  ( -- )  main-complement @ ?{hold}  ;
  \ Provoca un error si el complemento principal existe y no está en inventario.

: tool-complement{hold}  ( -- )  tool-complement @ ?{hold}  ;
  \ Provoca un error si el complemento instrumental existe y no está en inventario.

: {not-hold}  ( a -- )
  dup what !
  is-hold? you-already-have-what-error# and throw  ;
  \ Provoca un error si un ente está en inventario.

: ?{not-hold}  ( a | 0 -- )  ?dup ?? {not-hold}  ;
  \ Provoca un error si un supuesto ente lo es y está en inventario.

: main-complement{not-hold}  ( -- )
  main-complement @ ?{not-hold}  ;
  \ Provoca un error si el complemento principal existe y está en inventario.

: {worn}  ( a -- )
  dup what !
  is-worn-by-me? 0= you-do-not-wear-what-error# and throw  ;
  \ Provoca un error si un ente no lo llevamos puesto.

: ?{worn}  ( a | 0 -- )  ?dup ?? {worn}  ;
  \ Provoca un error si un supuesto ente lo es y no lo llevamos puesto.

: main-complement{worn}  ( -- )
  main-complement @ ?{worn}  ;
  \ Provoca un error si el complemento principal existe y no lo llevamos puesto.

: {open}  ( a -- )
  \ Provoca un error si un ente no está abierto.
  dup what !
  is-closed? what-is-already-closed-error# and throw  ;

: {closed}  ( a -- )
  dup what !
  is-open? what-is-already-open-error# and throw  ;
  \ Provoca un error si un ente no está cerrado.

: {not-worn}  ( a -- )
  dup what !
  is-worn-by-me? you-already-wear-what-error# and throw  ;
  \ Provoca un error si un ente lo llevamos puesto.

: ?{not-worn}  ( a | 0 -- )  ?dup ?? {not-worn}  ;
  \ Provoca un error si un supuesto ente lo es y lo llevamos puesto.

: main-complement{not-worn}  ( -- )
  main-complement @ ?{not-worn}  ;
  \ Provoca un error si el complemento principal existe y lo llevamos
  \ puesto.

: {cloth}  ( a -- )
  \ Provoca un error si un ente no se puede llevar puesto.
  is-cloth? 0= nonsense-error# and throw  ;

: ?{cloth}  ( a | 0 -- )  ?dup ?? {cloth}  ;
  \ Provoca un error si un supuesto ente lo es y no se puede llevar
  \ puesto.

: main-complement{cloth}  ( -- )  main-complement @ ?{cloth}  ;
  \ Provoca un error si el complemento principal existe y no se puede
  \ llevar puesto.

: {here}  ( a -- )
  \ Provoca un error si un ente no está presente.
  dup what !
  is-here? 0= is-not-here-what-error# and throw  ;

: ?{here}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está presente.
  ?dup ?? {here}  ;

: main-complement{here}  ( -- )  main-complement @ ?{here}  ;
  \ Provoca un error si el complemento principal existe y no está
  \ presente.

: {accessible}  ( a -- )
  dup what !  is-not-accessible?  cannot-see-what-error# and throw  ;
  \ Provoca un error si un ente no está accessible.

: ?{accessible}  ( a | 0 -- )  ?dup ?? {accessible}  ;
  \ Provoca un error si un supuesto ente lo es y no está accessible.

: main-complement{accessible}  ( -- )
  main-complement @ ?{accessible}  ;
  \ Provoca un error si el complemento principal existe y no está accessible.

: {takeable}  ( a -- )
  dup what !
  dup take-error# throw
  can-be-taken? 0= nonsense-error# and throw  ;
  \ Provoca un error si un ente no puede ser tomado.
  \ Nota: los errores apuntados por el campo `~take-error#` no reciben
  \ parámetros salvo en `what`.

: ?{takeable}  ( a | 0 -- )  ?dup ?? {takeable}  ;
  \ Provoca un error si un supuesto ente lo es y no puede ser tomado.

: main-complement{takeable}  ( -- )
  main-complement @ ?{takeable}  ;
  \ Provoca un error si el complemento principal existe y no puede ser
  \ tomado.

: {breakable}  ( a -- )  dup what ! ~break-error# @ throw  ;
  \ Provoca un error si un ente no puede ser roto.
  \ Nota: los errores apuntados por el campo `~break-error#` no
  \ reciben parámetros salvo en `what`.

: ?{breakable}  ( a | 0 -- )  ?dup ?? {breakable}  ;
  \ Provoca un error si un supuesto ente lo es y no puede ser roto.

: main-complement{breakable}  ( -- )
  main-complement @ ?{breakable}  ;
  \ Provoca un error si el complemento principal existe y no puede ser roto.

: {lookable}  ( a -- )
  dup what !
  can-be-looked-at? 0= cannot-see-what-error# and throw  ;
  \ Provoca un error si un ente no puede ser mirado.  Nota: los
  \ errores apuntados por el campo `~take-error#` no deben necesitar
  \ parámetros, o esperarlo en `what`.

: ?{lookable}  ( a | 0 -- )  ?dup ?? {lookable}  ;
  \ Provoca un error si un supuesto ente lo es y no puede ser mirado.

: main-complement{lookable}  ( -- )
  main-complement @ ?{lookable}  ;
  \ Provoca un error si el complemento principal existe y no puede ser
  \ mirado.

: {living}  ( a -- )
  is-living-being? 0= nonsense-error# and throw  ;
  \ Provoca un error si un ente no es un ser vivo.

: ?{living}  ( a | 0 -- )  ?dup ?? {living}  ;
  \ Provoca un error si un supuesto ente lo es y no es un ser vivo.

: main-complement{living}  ( -- )
  main-complement @ ?{living}  ;
  \ Provoca un error si el complemento principal existe y no es un ser vivo.

: {needed}  ( a -- )
  dup what !
  is-hold? 0= you-need-what-error# and throw  ;
  \ Provoca un error si un ente no está en inventario, pues es necesario.

: ?{needed}  ( a | 0 -- )
  ?dup ?? {needed}  ;
  \ Provoca un error si un supuesto ente lo es y no está en inventario, pues es necesario.

: main-complement{needed}  ( -- )
  main-complement @ ?{needed}  ;
  \ Provoca un error si el complemento principal existe y no está en inventario, pues lo necesitamos.

: {direction}  ( a -- )
  dup what !
  is-direction? 0= nonsense-error# and throw  ;
  \ Provoca un error si un ente no es una dirección.

: ?{direction}  ( a | 0 -- )
  ?dup ?? {direction}  ;
  \ Provoca un error si un supuesto ente lo es y no es una dirección.

: main-complement{direction}  ( -- )
  main-complement @ ?{direction}  ;
  \ Provoca un error si el complemento principal existe y no es una dirección.

\ ==============================================================
\ Herramientas para averiguar complemento omitido

: whom  ( -- a | 0 )
  true case
    ambrosio~ is-here? of  ambrosio~  endof
    leader~ is-here? of  leader~  endof
    false swap
  endcase  ;
  \ Devuelve un ente personaje al que probablemente se refiera un
  \ comando.  Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica.
  \
  \ XXX TODO -- ampliar para contemplar los soldados y oficiales,
  \ según la trama, el escenario y la fase de la batalla

: unknown-whom  ( -- a | 0 )
  true case
    ambrosio~ is-here-and-unknown? of  ambrosio~  endof
    leader~ is-here-and-unknown? of  leader~  endof
    false swap
  endcase  ;
  \ Devuelve un ente personaje desconocido al que probablemente se
  \ refiera un comando.  Se usa para averiguar el objeto de algunas
  \ acciones cuando el jugador no lo especifica

\ ==============================================================
\ Acciones

defer do-attack  ( -- )
defer do-break  ( -- )
defer do-climb  ( -- )
defer do-close  ( -- )
defer do-do  ( -- )
defer do-drop  ( -- )
defer do-examine  ( -- )
defer do-exits  ( -- )
defer do-frighten  ( -- )
defer do-go  ( -- )
defer do-go-ahead  ( -- )
defer do-go-back  ( -- )
defer do-go-down  ( -- )
defer do-go-east  ( -- )
defer do-go-in  ( -- )
defer do-go-north  ( -- )
defer do-go|do-break  ( -- )
defer do-go-out  ( -- )
defer do-go-south  ( -- )
defer do-go-up  ( -- )
defer do-go-west  ( -- )
defer do-hit  ( -- )
defer do-introduce-yourself  ( -- )
defer do-inventory  ( -- )
defer do-kill  ( -- )
defer do-look  ( -- )
defer do-look-to-direction  ( -- )
defer do-look-yourself  ( -- )
defer do-make  ( -- )
defer do-open  ( -- )
defer do-put-on  ( -- )
defer do-search  ( -- )
defer do-sharpen  ( -- )
defer do-speak  ( -- )
defer do-swim  ( -- )
defer do-take  ( -- )
defer do-take|do-eat  ( -- )  \ XXX TODO -- cambiar do-eat por ingerir
defer do-take-off  ( -- )

\ ----------------------------------------------
\ Mirar, examinar y registrar

: (do-look)  ( a -- )
  dup describe
  dup is-location? ?? .present familiar++  ;
  \ Mira un ente.

: do-look-by-default  ( -- a )
  main-complement @ ?dup 0= ?? my-location  ;
  \ Devuelve qué mirar.  Si falta el complemento principal, usar el
  \ escenario.

:noname  ( -- )
  tool-complement{unnecessary}
  do-look-by-default dup {lookable} (do-look)
  ; is do-look
  \  Acción de mirar.

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement @ ?dup 0= ?? protagonist~
  (do-look)
  ; is do-look-yourself
  \  Acción de mirarse.

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement{required}
  main-complement{direction}
  main-complement @ (do-look)
  ; is do-look-to-direction
  \  Acción de otear.
  \ XXX TODO -- traducir «otear» en el nombre de la palabra

:noname  ( -- )
  do-look
  ; is do-examine
  \ Acción de examinar.
  \ XXX TMP
  \ XXX TODO -- implementar `x salida`

:noname  ( -- )
  do-look
  ; is do-search
  \ Acción de registrar.
  \ XXX TMP

\ ----------------------------------------------
\ Salidas

\ XXX TODO -- Inacabado, no se usa

create do-exits-table-index  #exits cells allot
  \ Tabla para desordenar el listado de salidas.  Esta tabla permite
  \ que las salidas se muestren cada vez en un orden diferente.

variable #free-exits
  \ Contador de las salidas posibles.

: no-exit$  ( -- ca len )
  s" No hay"
  s{ s" salidas" s" salida" s" ninguna salida" }s&  ;
  \ Devuelve mensaje usado cuando no hay salidas que listar.

: go-out$  ( -- ca len )
  s{ s" salir" s" seguir" }s  ;

: go-out-to& ( ca len -- ca1 len1 )
  go-out$ s& s" hacia" s&  ;

: one-exit-only$  ( -- ca len )
  s{
  s" La única salida" possible1$ s& s" es" s& s" hacia" s?&
  ^only$ s" hay salida" s& possible1$ s& s" hacia" s&
  ^only$ s" es posible" s& go-out-to&
  ^only$ s" se puede" s& go-out-to&
  }s  ;
  \ Devuelve mensaje usado cuando solo hay una salidas que listar.

: possible-exits$  ( -- ca len )
  s" salidas" possible2$ s&  ;

: several-exits$  ( -- ca len )
  s{
  s" Hay" possible-exits$ s& s" hacia" s&
  s" Las" possible-exits$ s& s" son" s&
  }s  ;
  \ Devuelve mensaje usado cuando hay varias salidas que listar.

: .exits  ( -- )
  #listed @ case
    0 of  no-exit$  endof
    1 of  one-exit-only$  endof
    several-exits$ rot
  endcase
  «& «»@ period+ narrate  ;
  \ Imprime las salidas posibles.

: exit-separator$  ( -- ca len )
  #free-exits @ #listed @ list-separator$  ;
  \ Devuelve el separador adecuado a la salida actual.

: exit>list  ( u -- )
  [debug-do-exits] [if]  cr ." exit>list" cr .stack  [then]  \ XXX INFORMER
  exit-separator$ »+
  exits-table@ full-name »+
  #listed ++
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER
  \ Lista una salida.
  \ u = Puntero a un campo de dirección (desplazamiento relativo desde
  \ el inicio de la ficha).

false [if]

  \ XXX OLD -- Primera versión: Las salidas se listan siempre en el
  \ mismo orden en el que están definidas en las fichas.

: free-exits  ( a -- u )
  [debug-do-exits] [if]  cr ." free-exits" cr .stack  [then]  \ XXX INFORMER
  0 swap
  ~first-exit /exits bounds do
\   [debug-do-exits] [if]  i i cr . @ .  [then]  \ XXX INFORMER
    i @ 0<> abs +
  cell  +loop
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER
  \ Devuelve el número de salidas posibles de un ente.

:noname  ( -- )
  «»-clear
  #listed off
  my-location dup free-exits #free-exits !
  last-exit> 1+ first-exit> do
    [debug-do-exits] [??] ~~
    dup i + @
    [debug-do-exits] [??] ~~
    if  i exit>list  then
  cell  +loop  drop
  .exits
  ; is do-exits
  \ Acción de listar las salidas posibles de la localización del protagonista.

[else]

  \ XXX NEW -- Segunda versión: Las salidas se muestran cada vez en
  \ orden aleatorio.

0 value this-location
  \ Ente del que queremos calcular las salidas libres (para
  \ simplificar el manejo de la pila en el bucle).

: free-exits  ( a0 -- a1 ... au u )
  [debug-do-exits] [if]  cr ." free-exits" cr .stack  [then]  \ XXX INFORMER
  to this-location  depth >r
  last-exit> 1+ first-exit> do
    this-location i + @ ?? i
  cell  +loop
  depth r> -
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER
  \ Devuelve el número de salidas posibles de un ente.
  \ a0 = Ente
  \ a1 ... au = Entes de salida del ente a0
  \ u = número de entes de salida del ente a0

: (list-exits)  ( -- )
  «»-clear
  #listed off
  my-location free-exits
  dup >r unsort r>  dup #free-exits !
  0 ?do  exit>list  loop  .exits  ;
  \ Crea la lista de salidas y la imprime

' (list-exits) is describe-exits

:noname  ( -- )
  tool-complement{unnecessary}
  secondary-complement{forbidden}
  main-complement @ ?dup if
    dup my-location <> swap direction 0= and
    nonsense-error# and throw
  then  describe-exits
  ; is do-exits
  \ Lista las salidas posibles de la localización del protagonista.

[then]

\ ----------------------------------------------
\ Ponerse y quitarse prendas

: (do-put-on)  ( a -- )  be-worn  well-done  ;
  \ Ponerse una prenda.

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement{required}
  main-complement{cloth}
  \ XXX TODO -- terminar, hacer que tome la prenda si no la tiene:
  main-complement{not-worn}
  main-complement @ is-not-hold? if  do-take  then
  main-complement{hold}
  main-complement @ (do-put-on)
  ; is do-put-on
  \ Acción de ponerse una prenda.

: do-take-off-done-v1$  ( -- ca len )
  main-complement @ direct-pronoun s" quitas" s&  ;
  \ Devuelve una variante del mensaje que informa de que el
  \ protagonista se ha quitado el complemento principal, una
  \ prenda.
  \ XXX TODO -- esta variante no queda natural

: do-take-off-done-v2$  ( -- ca len )
  s" quitas" main-complement @ full-name s&  ;
  \ Devuelve una variante del mensaje que informa de que el
  \ protagonista se ha quitado el complemento principal, una
  \ prenda.

: do-take-off-done$  ( -- ca len )
  s" Te" s{ do-take-off-done-v1$ do-take-off-done-v2$ }s& period+  ;
  \ Devuelve el mensaje que informa de que el protagonista se ha
  \ quitado el complemento principal, una prenda.

: (do-take-off)  ( a -- )
  be-not-worn  do-take-off-done$ well-done-this  ;
  \ Quitarse una prenda.

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement{required}
  main-complement{worn}
  main-complement @ (do-take-off)
  ; is do-take-off
  \ Acción de quitarse una prenda.

\ ----------------------------------------------
\ Tomar y dejar

\ XXX OLD -- Puede que aún sirva:
\ : cannot-take-the-altar  \ No se puede tomar el altar
\   s" [el altar no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-flags  \ No se puede tomar las banderas
\   s" [las banderas no se tocan]" narrate  \ XXX TMP
\   nonsense
\   ;
\ : cannot-take-the-idol  \ No se puede tomar el ídolo
\   s" [el ídolo no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-door  \ No se puede tomar la puerta
\   s" [la puerta no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-fallen-away  \ No se puede tomar el derrumbe
\   s" [el derrumbe no se toca]" narrate  \ XXX TMP
\   nonsense
\   ;
\ : cannot-take-the-snake  \ No se puede tomar la serpiente
\   s" [la serpiente no se toca]" narrate  \ XXX TMP
\   dangerous
\   ;
\ : cannot-take-the-lake  \ No se puede tomar el lago
\   s" [el lago no se toca]" narrate  \ XXX TMP
\   nonsense
\   ;
\ : cannot-take-the-lock  \ No se puede tomar el candado
\   s" [el candado no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-water-fall  \ No se puede tomar la cascada
\   s" [la cascada no se toca]" narrate  \ XXX TMP
\   nonsense
\   ;

: (do-take)  ( a -- )  dup be-hold familiar++ well-done  ;
  \ Toma un ente.

:noname  ( -- )
  main-complement{required}
  main-complement{not-hold}
  main-complement{here}
  main-complement{takeable}
  main-complement @ (do-take)
  ; is do-take
  \ Toma un ente, si es posible.

: >do-drop-done-v1$  ( a -- ca1 len1 ) { object }
  s" Te desprendes de" s{
    object full-name
    object personal-pronoun
  }s& period+  ;

: >do-drop-done-v2$  ( a -- ca1 len1 )
  ^direct-pronoun s" dejas." s&  ;

: >do-drop-done$  ( a -- ca1 len1 ) { object }
  s{ object >do-drop-done-v1$  object >do-drop-done-v2$ }s  ;

: (do-drop)  ( a -- ) { object }
  object is-worn? if

    [false] [if]  \ XXX TODO -- mensaje combinado:
    object be-not-worn
    s" te" s{
      direct-pronoun s& s" quita" object plural-ending+
      s" quita" object plural-ending+ object full-name s&
    }s& s" y" s&

  else  null$

    [then]

    \ XXX NOTE: método más sencillo:

    do-take-off

  then
  object be-here
  object >do-drop-done$ well-done-this  ;
  \ Deja un ente.

:noname  ( -- )
  \ Acción de dejar.
  main-complement{required}
  main-complement{hold}
  main-complement @ (do-drop)
  ; is do-drop

:noname  ( -- )
  \ Acción de desambiguación.
  \ XXX TODO
  do-take
  ; is do-take|do-eat

\ ----------------------------------------------
\ Cerrar y abrir

: first-close-the-door  ( -- )
  s" cierras" s" primero" rnd2swap s& ^uppercase
  door~ full-name s& period+ narrate
  door~ be-closed  ;
  \ Informa de que la puerta está abierta
  \ y hay que cerrarla antes de poder cerrar el candado.

: .the-key-fits  ( -- )
  \ XXX TODO -- nuevo texto, quitar «fácilmente»
  s" La llave gira fácilmente dentro del candado."
  narrate  ;

: close-the-lock  ( -- )
  key~ tool{this-only}
  lock~ {open}
  key~ {hold}
  door~ is-open? ?? first-close-the-door
  lock~ be-closed  .the-key-fits  ;
  \ Cerrar el candado, si es posible.

: .the-door-closes  ( -- )
  s" La puerta"
  s{ s" rechina" s" emite un chirrido" }s&
  s{ s" mientras la cierras" s" al cerrarse" }s&
  period+ narrate  ;
  \ Muestra el mensaje de cierre de la puerta.

: (close-the-door)  ( -- )
  door~ be-closed .the-door-closes
  location-47~ location-48~ w|<-->|
  location-47~ location-48~ o|<-->|  ;
  \ Cerrar la puerta.

: close-and-lock-the-door  ( -- )
  door~ {open}  key~ {hold}
  (close-the-door) close-the-lock  ;
  \ Cerrar la puerta, si está abierta, y el candado.

: just-close-the-door  ( -- )
  door~ {open} (close-the-door)  ;
  \ Cerrar la puerta, sin candarla, si está abierta.

: close-the-door  ( -- )
  key~ tool{this-only}
  tool-complement @ ?dup
  if    close-and-lock-the-door
  else  just-close-the-door  then  ;
  \ Cerrar la puerta, si es posible.

: close-it  ( a -- )
  case
    door~ of  close-the-door  endof
    lock~ of  close-the-lock  endof
    nonsense
  endcase  ;
  \ Cerrar un ente, si es posible.

:noname  ( -- )
  main-complement{required}
  main-complement{accessible}
  main-complement @ close-it
  ; is do-close

: the-door-is-locked  ( -- )
  \ Informa de que la puerta está cerrada por el candado.
  \ XXX TODO -- añadir variantes
  lock~ ^full-name s" bloquea la puerta." s&
  narrate
  lock-found  ;
  \ Acción de cerrar.

: unlock-the-door  ( -- )
  the-door-is-locked
  key~ {needed}
  lock~ dup be-open
  ^pronoun s" abres con" s& key~ full-name s& period+ narrate  ;
  \ Abrir la puerta candada, si es posible.
  \ XXX TODO -- falta mensaje adecuado sobre la llave que gira

: open-the-lock  ( -- )
  key~ tool{this-only}
  lock~ {closed}
  key~ {needed}
  lock~ be-open  well-done  ;
  \ Abrir el candado, si es posible.

: the-plants$  ( -- ca len )
  s" las hiedras" s" las hierbas" both  ;
  \ Devuelve las plantas que la puerta rompe al abrirse.
  \ XXX TODO -- hacerlas visibles

: the-door-breaks-the-plants-0$  ( -- ca len )
  s{ s" mientras" s" al tiempo que" }s
  the-plants$ s& s" se rompen en su trazado" s&  ;
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (primera variante).

: the-door-breaks-the-plants-1$  ( -- ca len )
  s" rompiendo" the-plants$ s& s" a su paso" s&  ;
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (segunda variante).

: the-door-breaks-the-plants$  ( -- ca len )
  ['] the-door-breaks-the-plants-0$
  ['] the-door-breaks-the-plants-1$ 2 choose execute  ;
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta.

: the-door-sounds$  ( -- ca len )
  s{ s" rechinando" s" con un chirrido" }s  ;

: ambrosio-byes  ( -- )
  s" Ambrosio, alegre, se despide de ti:" narrate
  s{
  s{ s" Tengo" s" Ten" }s s" por seguro" s&
  s{ s" Estoy" s" Estate" }s s" seguro de" s&
  s{ s" Tengo" s" Ten" }s s" la" s& s{ s" seguridad" s" certeza" }s& s" de" s&
  s" No" s{ s" me cabe" s" te quepa" }s& s" duda de" s&
  s" No" s{ s" dudo" s" dudes" }s&
  }s s" que" s&
  s{
  s" nos volveremos a" s{ s" encontrar" s" ver" }s& again$ s?&
  s" volveremos a" s{ s" encontrarnos" s" vernos" }s& again$ s?&
  s" nos" s{ s" encontraremos" s" veremos" }s& again$ s&
  s" nuestros caminos" s{ s" volverán a cruzarse" s" se cruzarán" }s& again$ s&
  }s& period+ speak
  ^and|but$ s" , antes de que puedas" s+
  s{ s" decir algo" s" corresponderle" s" responderle" s" despedirte" s" de él" s?& }s&
  s" , te das cuenta de que" s+ s" Ambrosio" s?&
  s" , misteriosamente," s?+
  s{ s" ha desaparecido" s" se ha marchado" s" se ha ido" s" ya no está" }s&
  period+ narrate  ;
  \ Ambrosio se despide cuando se abre la puerta por primera vez.

: the-door-opens-first-time$  ( -- ca len )
  s" La puerta" s{ s" cede" s" se abre" }s&
  s{ s" despacio" s" poco a poco" s" lentamente" }s&
  s" y no sin" s&
  s{ s" dificultad" s" ofrecer resistencia" }s& comma+
  the-door-sounds$ comma+ s&
  the-door-breaks-the-plants$ s& period+  ;
  \ Devuelve el mensaje de apertura de la puerta
  \ la primera vez.

: the-door-opens-once-more$  ( -- ca len )
  s" La puerta se abre" the-door-sounds$ s& period+  ;
  \ Devuelve el mensaje de apertura de la puerta
  \ la segunda y siguientes veces.

: .the-door-opens  ( -- )
  door~ times-open
  if    the-door-opens-once-more$ narrate
  else  the-door-opens-first-time$ narrate ambrosio-byes  then  ;
  \ Muestra el mensaje de apertura de la puerta.

: (open-the-door)  ( -- )
  key~ tool{this-only}  \ XXX TODO ¿por qué aquí?
  lock~ is-closed? ?? unlock-the-door
  location-47~ location-48~ w<-->
  location-47~ location-48~ o<-->
  .the-door-opens
  door~ dup be-open times-open++
  grass~ be-here  ;
  \ Abrir la puerta, que está cerrada.

: open-the-door  ( -- )
  door~ is-open?
  if    door~ it-is-already-open tool-complement{unnecessary}
  else  (open-the-door)  then  ;
  \ Abrir la puerta, si es posible.

: open-it  ( a -- )
  dup familiar++
  case
    door~ of  open-the-door  endof
    lock~ of  open-the-lock  endof
    nonsense
  endcase  ;
  \ Abrir un ente, si es posible.

:noname  ( -- )
  s" do-open" halto  \ XXX INFORMER
  main-complement{required}
  main-complement{accessible}
  main-complement @ open-it
  ; is do-open
  \ Acción de abrir.

\ ----------------------------------------------
\ Agredir

: the-snake-runs-away  ( -- )
  s{ s" Sorprendida por" s" Ante" }s
  s" los amenazadores tajos," s&
  s" la serpiente" s&
  s{
  s" huye" s" se aleja" s" se esconde"
  s" se da a la fuga" s" se quita de enmedio"
  s" se aparta" s" escapa"
  }s&
  s{ null$ s" asustada" s" atemorizada" }s&
  narrate  ;
  \ La serpiente huye.

: attack-the-snake  ( -- )
  sword~ {needed}
  the-snake-runs-away
  snake~ vanish  ;
  \ Atacar la serpiente.
  \ XXX TODO -- inconcluso

: attack-ambrosio  ( -- )  no-reason  ;
  \ Atacar a Ambrosio.

: attack-leader  ( -- )  no-reason  ;
  \ Atacar al jefe.

: (do-attack)  ( a -- )
  case
    snake~ of  attack-the-snake  endof
    ambrosio~ of  attack-ambrosio  endof
    leader~ of  attack-leader  endof
    do-not-worry
  endcase  ;
  \ Atacar un ser vivo.

:noname  ( -- )
  main-complement{required}
  main-complement{accessible}
  main-complement{living} \ XXX TODO -- también es posible atacar otras cosas, como la ciudad u otros lugares, o el enemigo
  tool-complement{hold}
  main-complement @ (do-attack)
  ; is do-attack
  \ Acción de atacar.

:noname  ( -- )
  main-complement{required}
  main-complement{accessible}
  main-complement{living}
  tool-complement{hold}
  main-complement @ (do-attack)
  ; is do-frighten
  \ Acción de asustar.
  \ XXX TODO -- distinguir de las demás en grado o requisitos

: kill-the-snake  ( -- )
  sword~ {needed}
  the-snake-runs-away
  snake~ vanish  ;
  \ Matar la serpiente.

: kill-ambrosio  ( -- )  no-reason  ;
  \ Matar a Ambrosio.

: kill-leader  ( -- )  no-reason  ;
  \ Matar al jefe.

: kill-your-soldiers  ( -- )  no-reason  ;
  \ Matar a tus hombres.

: (do-kill)  ( a -- )
  case
    snake~ of  kill-the-snake  endof
    ambrosio~ of  kill-ambrosio  endof
    leader~ of  kill-leader  endof
    soldiers~ of  kill-your-soldiers  endof
    do-not-worry
  endcase  ;
  \ Matar un ser vivo.

:noname  ( -- )
  main-complement{required}
  main-complement{accessible}
  main-complement{living}  \ XXX TODO -- también es posible matar otras cosas, como el enemigo
  tool-complement{hold}
  main-complement @ (do-kill)
  ; is do-kill
  \ Acción de matar.

: cloak-piece  ( a -- )
  2 random if  be-here  else  taken  then  ;
  \ Hace aparecer un resto de la capa rota de forma aleatoria:
  \ en el escenario o en el inventario.
  \ XXX TODO -- mejorar con mensajes, ejemplo:
  \ s" Un hilo se ha desprendido al cortar la capa con la espada."

: cloak-pieces  ( -- )
  rags~ cloak-piece  thread~ cloak-piece  piece~ cloak-piece  ;
  \ Hace aparecer los restos de la capa rota de forma aleatoria:
  \ en el escenario o en el inventario.

: shatter-the-cloak  ( -- )
  sword~ {accessible}
  sword~ taken
  using$ sword~ full-name s& comma+
  s" rasgas" s& cloak~ full-name s& period+ narrate
  cloak-pieces  cloak~ vanish  ;
  \ Romper la capa.

: (do-break)  ( a -- )
  case
    snake~ of  kill-the-snake  endof  \ XXX TMP
    cloak~ of  shatter-the-cloak  endof
    do-not-worry
  endcase  ;
  \ Romper un ente.

:noname  ( -- )
  main-complement{required}
  main-complement{accessible}
  main-complement{breakable}
  tool-complement{hold}
  main-complement @ (do-break)
  ; is do-break
  \ Acción de romper.

: lit-the-torch  ( -- )
  s" Poderosas chispas salen del choque entre espada y pedernal,"
  s" encendiendo la antorcha." s& narrate
  torch~ be-lit  ;

: hit-the-flint  ( -- )
  flint~ {accessible}
  sword~ taken
  using$ sword~ full-name s& comma+
  s" golpeas" s& flint~ full-name s& period+ narrate
  lit-the-torch  ;

: (do-hit)  ( a -- )
  case
    snake~ of  kill-the-snake     endof
    cloak~ of  shatter-the-cloak  endof
    flint~ of  hit-the-flint      endof
    do-not-worry
  endcase  ;
  \ Golpear un ente.

:noname  ( -- )
  main-complement{required}
  main-complement{accessible}
  main-complement @ (do-hit)
  \ s" golpear"  main-complement+is-nonsense \ XXX TMP
  ; is do-hit
  \ Acción de golpear.

: log-already-sharpened$  ( -- ca len )
  s" Ya" s{ s" lo afilaste antes"
            s" está afilado de antes"
            s" tiene una buena punta"
            s" quedó antes bien afilado"
         }s&  ;
  \ Devuelve una variante de «Ya está afilado».

: no-need-to-do-it-again$  ( -- ca len )
  s{
  s" no es necesario"
  s" no hace" s" ninguna" s?& s" falta" s&
  s" no es menester"
  s" no serviría de nada"
  s" no serviría de mucho"
  s" serviría de poco"
  s" sería inútil"
  s" sería en balde"
  s" sería un esfuerzo" s{ s" inútil" s" baldío" }s&
  s" sería un esfuerzo baldío"
  }s{
  s" hacerlo"
  s" volver a hacerlo"
  s" repetirlo"
  }s& again$ s&  ;
  \ Devuelve una variante de «no hace falta hacerlo otra vez».

: ^no-need-to-do-it-again$  ( -- ca len )
  no-need-to-do-it-again$ ^uppercase  ;
  \ Devuelve una variante de «No hace falta hacerlo otra vez».

: log-already-sharpened-0$  ( -- ca len )
  log-already-sharpened$ ^uppercase period+
  ^no-need-to-do-it-again$ period+ s&  ;
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 0).

: log-already-sharpened-1$  ( -- ca len )
  ^no-need-to-do-it-again$ period+ s&
  log-already-sharpened$ ^uppercase period+ s&  ;
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 1).

: log-already-sharpened  ( -- )
  ['] log-already-sharpened-0$
  ['] log-already-sharpened-1$ 2 choose execute  narrate  ;
  \ Informa de que el tronco ya estaba afilado.

: sharpen-the-log  ( -- )
  hacked-the-log? @
  if    log-already-sharpened
  else  hacked-the-log? on  well-done  then  ;
  \ Afila el tronco.
  \ XXX TODO -- distinguir herramientas

: sharpen-the-sword  ( -- )  ;
  \ Afila la espada.
  \ XXX TODO -- inconcluso

: (do-sharpen)  ( a -- )
  case
    sword~ of  sharpen-the-sword  endof
    log~ of  sharpen-the-log  endof
  endcase  ;
  \ Afila un ente que puede ser afilado.

:noname  ( -- )
  \ Acción de afilar.
  main-complement{required}
  main-complement{accessible}
  main-complement @ can-be-sharpened?
  if    main-complement @ (do-sharpen)
  else  nonsense
  then
  ; is do-sharpen

\ ----------------------------------------------
\ Movimiento

\ XXX TODO -- mover a la sección de errores

: toward-that-direction  ( a -- ca len )
  dup >r  has-no-article?
  if    s" hacia" r> full-name
  else  toward-the(m)$ r> name
  then  s&  ;
  \ Devuelve «al/hacia la dirección indicada», correspondiente al ente
  \ dirección _a_.

: (impossible-move)  ( a -- )
  ^is-impossible$ s" ir" s&  rot
  3 random if    toward-that-direction
           else  drop that-way$
           then  s& period+ action-error  ;
  \ El movimiento es imposible hacia el ente dirección _a_.
  \ XXX TODO -- añadir una tercera variante «ir en esa dirección»; y
  \ otras específicas como «no es posible subir»

1 ' (impossible-move) action-error: impossible-move drop

: do-go-if-possible  ( a -- )
  [debug] [if]  s" Al entrar en DO-GO-IF-POSSIBLE" debug  [then]  \ XXX INFORMER
  dup direction ?dup if  \ ¿El ente es una dirección?
    my-location + @ ?dup  \ ¿Tiene mi escenario salida en esa dirección?
    if  nip enter-location  else  impossible-move  then
  else  drop nonsense  then
  [debug] [if]  s" Al salir de DO-GO-IF-POSSIBLE" debug  [then]  ;  \ XXX INFORMER
  \ Comprueba si el movimiento hacio un supuesto ente de dirección _a_
  \ es posible y si es así lo efectúa.

: simply-do-go  ( -- )  s" Ir sin rumbo...?" narrate  ;
  \ Ir sin dirección específica.
  \ XXX TODO -- inconcluso

:noname  ( -- )
  [debug] [if]  s" Al entrar en DO-GO" debug  [then]  \ XXX INFORMER
  tool-complement{unnecessary}
  main-complement @ ?dup
  if  do-go-if-possible  else  simply-do-go  then
  [debug] [if]  s" Al salir de DO-GO" debug  [then]  \ XXX INFORMER
  ; is do-go
  \ Acción de ir.

:noname  ( -- )
  tool-complement{unnecessary}
  north~ main-complement{this-only}
  north~ do-go-if-possible
  ; is do-go-north
  \ Acción de ir al norte.

:noname  ( -- )
  [debug-catch] [if]  s" Al entrar en DO-GO-SOUTH" debug  [then]  \ XXX INFORMER
  tool-complement{unnecessary}
  south~ main-complement{this-only}
  south~ do-go-if-possible
  [debug-catch] [if]  s" Al salir de DO-GO-SOUTH" debug  [then]  \ XXX INFORMER
  ; is do-go-south
  \ Acción de ir al sur.

:noname  ( -- )
  tool-complement{unnecessary}
  east~ main-complement{this-only}
  east~ do-go-if-possible
  ; is do-go-east
  \ Acción de ir al este.

:noname  ( -- )
  tool-complement{unnecessary}
  west~ main-complement{this-only}
  west~ do-go-if-possible
  ; is do-go-west
  \ Acción de ir al oeste.

:noname  ( -- )
  tool-complement{unnecessary}
  up~ main-complement{this-only}
  up~ do-go-if-possible
  ; is do-go-up
  \ Acción de ir hacia arriba.

:noname  ( -- )
  tool-complement{unnecessary}
  down~ main-complement{this-only}
  down~ do-go-if-possible
  ; is do-go-down
  \ Acción de ir hacia abajo.

:noname  ( -- )
  tool-complement{unnecessary}
  out~ main-complement{this-only}
  out~ do-go-if-possible
  ; is do-go-out
  \ Acción de ir hacia fuera.

: enter-the-cave-entrance  ( -- )
  is-the-cave-entrance-accessible? 0=
  cannot-see-what-error# and throw
  in~ do-go-if-possible  ;

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement @ cave-entrance~ =
  if  enter-the-cave-entrance exit  then
  in~ main-complement{this-only}
  in~ do-go-if-possible
  ; is do-go-in
  \ Acción de ir hacia dentro.

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement{forbidden}
  s" [voy hacia atrás, pero es broma]" narrate \ XXX TMP
  ; is do-go-back
  \ Acción de ir hacia atrás.
  \ XXX TODO

:noname  ( -- )
  tool-complement{unnecessary}
  main-complement{forbidden}
  s" [voy hacia delante, pero es broma]" narrate \ XXX TMP
  ; is do-go-ahead
  \ Acción de ir hacia delante.
  \ XXX TODO

\ ----------------------------------------------
\ Partir (desambiguación)

:noname  ( -- )
  main-complement @ ?dup
  if  ( a )
    is-direction? if  do-go  else  do-break  then
  else
    tool-complement @ if do-break  else  simply-do-go  then
  then
  ; is do-go|do-break
  \ Acción de partir (desambiguar: romper o marchar).

\ ----------------------------------------------
\ Nadar

: in-a-different-place$  ( -- ca len )
  s" en un" s& place$
  s{ s" desconocido" s" nuevo" s" diferente" }s&
  s" en otra parte"
  s" en otro lugar"
  3 schoose  ;
  \ Devuelve una variante de «en un lugar diferente».

: you-emerge$  ( -- ca len )
  s{ s" Consigues" s" Logras" }s
  s{ s" emerger," s" salir a la superficie," }s&
  though$ s& in-a-different-place$ s&
  s" de la" s& cave$ s& s" ..." s+  ;
  \ Devuelve mensaje sobre la salida a la superficie.

: swiming$  ( -- ca len )
  s" Buceas" s{ s" pensando en" s" deseando"
  s" con la esperanza de" s" con la intención de" }s&
  s{ s" avanzar," s" huir," s" escapar,"  s" salir," }s&
  s" aunque" s&{ s" perdido." s" desorientado." }s&  ;
  \ Devuelve mensaje sobre el buceo.

: drop-the-cuirasse$  ( f -- ca len )
  s{ s" te desprendes de ella" s" te deshaces de ella"
  s" la dejas caer" s" la sueltas" }s
  rot if
    s{ s" Rápidamente" s" Sin dilación"
    s" Sin dudarlo" s{ null$ s" un momento" s" un instante" }s&
    }s 2swap s&
  then  period+  ;
  \ Devuelve mensaje sobre deshacerse de la coraza dentro del agua.
  \ El indicador _f_ es cierto si el resultado debe ser el inicio de
  \ una frase.

: you-leave-the-cuirasse$  ( -- ca len )
  cuirasse~ is-worn-by-me?
  if  s{ s" Como puedes," s" No sin dificultad," }s
      s{ s" logras quitártela" s" te la quitas" }s&
      s" y" s& false drop-the-cuirasse$ s&
  else  true drop-the-cuirasse$  then  ;
  \ Devuelve mensaje sobre quitarse y soltar la coraza dentro del agua.

: (you-sink-0)$ ( -- ca len )
  s{ s" Caes" s" Te hundes"
  s{ s" Empiezas" s" Comienzas" }s{ s" a hundirte" s" a caer" }s&
  }s s" sin remedio" s?& s" hacia" s&
  s{ s" el fondo" s" las profundidades" }s&
  s{ s" por el" s" debido al" s" arrastrado por" s" a causa del" }s&
  s" peso de tu coraza" s&  ;
  \ Devuelve la primera versión del mensaje sobre hundirse con la coraza.

: (you-sink-1)$ ( -- ca len )
  s" El peso de tu coraza"
  s{ s" te arrastra" s" tira de ti" }s&
  s{ null$ s" sin remedio" s" con fuerza" }s&
  s{
  s" hacia el fondo"
  s" hacia las profundidades"
  s" hacia abajo"
  }s&  ;
  \ Devuelve la segunda versión del mensaje sobre hundirse con la coraza.

: you-sink$ ( -- ca len )
  ['] (you-sink-0)$
  ['] (you-sink-1)$ 2 choose execute period+  ;
  \ Devuelve mensaje sobre hundirse con la coraza.

: you-swim-with-cuirasse$  ( -- ca len )
  you-sink$ you-leave-the-cuirasse$ s&  ;
  \  Devuelve mensaje inicial sobre nadar con coraza.

: you-swim$  ( -- ca len )
  cuirasse~ is-hold?
  if  you-swim-with-cuirasse$  cuirasse~ vanish
  else  null$
  then  swiming$ s&  ;
  \  Devuelve mensaje sobre nadar.

:noname  ( -- )
  location-11~ am-i-there? if
    clear-screen-for-location
    you-swim$ narrate narration-break
    you-emerge$ narrate narration-break
    location-12~ enter-location  the-battle-ends
  else  s" nadar" now-or-here-or-null$ s& be-nonsense  then
  ; is do-swim
  \ Acción de nadar.

\ ----------------------------------------------
\ Escalar

: you-try-climbing-the-fallen-away  ( -- )
  s{ s" Aunque" s" A pesar de que" }s
  s{  s" parece no haber salida"
      s" el obstáculo parece insuperable"
      s" la situación parece desesperada"
      s" el regreso parece inevitable"
      s" continuar parece imposible"
  }s& comma+
  s{ s" optas por" s" decides" s" tomas la decisión de" }s&
  s{ s" explorar" s" examinar" }s& s" el" s&
  s{ s" derrumbe" s" muro de" rocks$ s& }s&
  s{  s" en compañía de" s" junto con"
      s" ayudado por" s" acompañado por"
  }s&
  s{ s" algunos" s" varios" }s& s" de tus" s&
  s{ s" oficiales" soldiers$ }s& s" , con la" s+
  s{ s" vana" s" pobre" s" débil" }s& s" esperanza" s&
  s" de" s&
  s{ s" encontrar" s" hallar" s" descubrir" }s&
  s{ s" la" s" alguna" }s& way$ s& s" de" s&
  s{  s" escalarlo" s" vencerlo" s" atravesarlo"
      s" superarlo" s" pasarlo"
      s{ s" pasar" s" cruzar" }s s" al otro lado" s&
  }s&  period+ narrate narration-break  ;
  \ Imprime la primera parte del mensaje
  \ previo al primer intento de escalar el derrumbe.

: you-can-not-climb-the-fallen-away  ( -- )
  ^but$
  s{  s{ s" pronto" s" enseguida" }s s" has de" s&
      s" no tardas mucho en"
  }s&
  s{  s" rendirte ante" s" aceptar" s" resignarte ante" }s&
  s{
    s{ s" los hechos" s" la realidad" s" la situación" s" tu suerte" }s
    s{  s" la voluntad"
        s{ s" el" s" este" }s s" giro" s&
        s{ s" el" s" este" }s s" capricho" s&
        s{ s" la" s" esta" }s s" mudanza" s&
    }s s" de" s& s" la diosa" s?& s" Fortuna" s&
  }s& s" ..." s+ narrate narration-break  ;
  \ Imprime la segunda parte del mensaje
  \ previo al primer intento de escalar el derrumbe.

: do-climb-the-fallen-away-first  ( -- )
  you-try-climbing-the-fallen-away
  you-can-not-climb-the-fallen-away  ;
  \ Imprime el mensaje
  \ previo al primer intento de escalar el derrumbe.

: climbing-the-fallen-away-is-impossible  ( -- )
  s{ s" pasar" s" escalar" s" subir por" }s
  s{
    s" el derrumbe"
    s{ s" el muro" s" la pared" s" el montón" }s s" de" s& rocks$ s&
    s" las" rocks$ s&
  }s& be-impossible  ;
  \ Imprime el mensaje de error de que
  \ es imposible escalar el derrumbe.

: do-climb-the-fallen-away  ( -- )
  \ Escalar el derrumbe.
  climbed-the-fallen-away? @ 0=
  ?? do-climb-the-fallen-away-first
  climbing-the-fallen-away-is-impossible
  climbed-the-fallen-away? on  ;

: do-climb-this-here-if-possible  ( a -- )  ;
  \ Escalar el ente indicado, que está presente, si es posible.
  \ XXX TODO -- inconcluso

: do-climb-if-possible  ( a -- )
  dup is-here?
  if    drop s" [escalar eso]" narrate
  else  drop s" [no está aquí eso para escalarlo]" narrate
  then  ;
  \ Escalar el ente indicado si es posible.
  \ XXX TODO -- inconcluso

: nothing-to-climb  ( -- )
  s" [No hay nada que escalar]" narrate  ;
  \ XXX TODO -- inconcluso

: do-climb-something  ( -- )
  location-09~ am-i-there?  \ ¿Ante el derrumbe?
  if  do-climb-the-fallen-away exit
  then
  location-08~ am-i-there?  \ ¿En el desfiladero?
  if  s" [Escalar en el desfiladero]" narrate exit
  then
  my-location is-indoor-location?
  if  s" [Escalar en un interior]" narrate exit
  then
  nothing-to-climb  ;
  \ Escalar algo no especificado.
  \ XXX TODO -- inconcluso

:noname  ( -- )
  main-complement @ ?dup
  if  do-climb-if-possible  else  do-climb-something  then
  ; is do-climb
  \ Acción de escalar.
  \ XXX TODO -- inconcluso

\ ----------------------------------------------
\ Inventario

: anything-with-you$  ( -- ca len )
  s" nada" with-you$ ?dup if   2 random ?? 2swap s&
                          else  drop  then  ;
  \ Devuelve una variante de «nada contigo».

: you-are-carrying-nothing$  ( -- ca len )
  s" No" you-carry$ anything-with-you$ period+ s& s&  ;
  \ Devuelve mensaje para sustituir a un inventario vacío.

: ^you-are-carrying$  ( -- ca len )
  ^you-carry$ with-you$ s&  ;
  \ Devuelve mensaje para encabezar la lista de inventario,
  \ con la primera letra mayúscula.

: you-are-carrying$  ( -- ca len )  you-carry$ with-you$ s&  ;
  \ Devuelve mensaje para encabezar la lista de inventario.

: you-are-carrying-only$  ( -- ca len )
  2 random if    ^you-are-carrying$ only-or-null$ s&
           else  ^only-or-null$ you-are-carrying$ s&  then  ;
  \ Devuelve mensaje para encabezar una lista de inventario de un solo elemento.

:noname  ( -- )
  protagonist~ content-list
  #listed @ case
    0 of  you-are-carrying-nothing$ 2swap s& endof
    1 of  you-are-carrying-only$ 2swap s& endof
    >r ^you-are-carrying$ 2swap s& r>
  endcase  narrate
  ; is do-inventory
  \ Acción de hacer inventario.

' do-inventory is describe-inventory

\ ----------------------------------------------
\ Hacer

:noname  ( -- )
  main-complement @ if  nonsense  else  do-not-worry  then
  ; is do-make
  \ Acción de hacer (fabricar).

:noname  ( -- )
  main-complement @ inventory~ =
  if  do-inventory  else  do-make  then
  ; is do-do
  \ Acción de hacer (genérica).

\ ----------------------------------------------
\ Hablar y presentarse

\ ----------------------------------------------
\ Conversaciones con el líder de los refugiados

: a-man-takes-the-stone  ( -- )
  s{  s" Un hombre" s" Un refugiado"
      s" Uno de los" s{ s" hombres" s" refugiados" }s&
  }s{ s" se adelanta,"
      s" sale de entre"
        s{  s" sus compañeros" s" los otros"
            s" la multitud" s" la gente"
        }s& comma+
  }s?&
  s{  s" se te acerca," s" se acerca a ti,"
      s" se te aproxima," s" se aproxima a ti,"
  }s?&
  s" te" s&{ s" arrebata" s" quita" }s&
  s" la piedra" s& s" de las manos" s?& s" y" s&
  s{
    s" se la lleva"
    s{ s" se marcha" s" se va" s" desaparece" }s s" con ella" s?&
  }s& period+ narrate
  location-18~ stone~ be-there  ;
  \ Un hombre te quita la piedra.

: gets-angry$  ( -- ca len )
  s" se" s{ s" irrita" s" enfada" s" enoja" s" enfurece" }s&  ;
  \ Devuelve una variante de «se enfada».

: the-leader-gets-angry$  ( -- ca len )
  s{ s" Al verte" s" Viéndote" s" Tras verte" }s
  s{ s" llegar" s" aparecer" s" venir" }s&
  again$ stone-forbidden? @ ?keep s&
  s" con la piedra," s&
  s" el" s& old-man$ s& gets-angry$ s&  ;
  \ Devuelve una variante de «El líder se enfada».
  \ XXX OLD -- yo no se usa.

: the-leader-gets-angry  ( -- )
  the-leader-gets-angry$ period+ narrate  ;
  \ Mensaje de que el líder se enfada.
  \ XXX OLD -- ya no se usa.

: warned-once$  ( -- ca len )
  s{
  s" antes"
  s" en la ocasión anterior"
  s" en la otra ocasión"
  s" en una ocasión"
  s" la otra vez"
  s" la vez anterior"
  s" una vez"
  }s  ;

: warned-twice$  ( -- ca len )
  s{
  s" antes"
  s" dos veces"
  s" en dos ocasiones"
  s" en las otras ocasiones"
  s" en mas de una ocasión"
  s" en un par de ocasiones"
  s" las otras veces"
  s" más de una vez"
  s" un par de veces"
  }s  ;

: warned-several-times$  ( -- ca len )
  s{
  s" en las otras ocasiones"
  s" en más de dos ocasiones"
  s" en más de un par de ocasiones"
  s" en otras ocasiones"
  s" en varias ocasiones"
  s" las otras veces"
  s" más de dos veces"
  s" más de un par de veces"
  s" otras veces"
  s" varias veces"
  }s  ;

: warned-many-times$  ( -- ca len )
  s{
  s" demasiadas veces"
  s" en demasiadas ocasiones"
  s" en incontables ocasiones"
  s" en innumerables ocasiones"
  s" en las otras ocasiones"
  s" en muchas ocasiones"
  s" en varias ocasiones"
  s" incontables veces"
  s" innumerables veces"
  s" las otras veces"
  s" muchas veces"
  s" otras veces"
  s" varias veces"
  }s  ;

: times-warned  ( u -- ca1 len1 )
  { times }  \ Variable local
  true case
    times 0 = of  null$  endof
    times 1 = of  warned-once$  endof
    times 2 = of  warned-twice$  endof
    times 6 < of  warned-several-times$  endof
    warned-many-times$ rot
  endcase  ;

: already-warned$  ( -- ca len )
  s" ya" s?
  s{
    s" fuisteis" s{ s" avisado" s" advertido" }s& s" de ello" s?&
    s" se os" s{ s" avisó" s" advirtió" }s& s" de ello" s?&
    s" os lo" s{ s" hicimos saber" s" advertimos" }s&
    s" os lo" s{ s" hice saber" s" advertí" }s&
    s" se os" s{ s" hizo saber" s" dejó claro" }s&
  }s&  ;
  \ Mensaje de que el líder ya te advirtió sobre un objeto.
  \ XXX TODO -- elaborar más

: already-warned  ( u -- ca1 len1 )
  times-warned already-warned$ rnd2swap s& period+ ^uppercase  ;
  \ Mensaje de que el líder ya te advirtió sobre un objeto,
  \ con indicación al azar del número de veces.

: you-can-not-take-the-stone$  ( -- ca len )
  s{ s" No" s" En modo alguno" s" De ninguna" way$ s& s" De ningún modo" }s
  s" podemos" s&
  s{
    s{ s" permitiros" s" consentiros" }s
      s{ s" huir" s" escapar" s" marchar" s" pasar" }s& s" con" s&
    s{ s" permitir" s" consentir" s" aceptar" }s s" que" s&
      s{  s{ s" huyáis" s" escapéis" s" marchéis" s" paséis" }s s" con" s&
          s" os vayáis con"
          s" os" s? s" marchéis con" s&
          s" os llevéis"
          s" nos" s? s" robéis" s&
          s" os" s{ s" apropiés" s" apoderéis" s" adueñéis" }s& s" de" s&
      }s&
  }s& s" la" s" piedra del druida"
  2dup stone~ fs-name!
  s& s& period+  ;
  \ Devuelve el mensaje de que no te puedes llevar la piedra.
  \ También cambia el nombre de la piedra.

: gesture-about-the-stone$  ( -- ca len )
  s" y" s? s{ s" entonces" s" a continuación" s" seguidamente" }s& ^uppercase
  s" hace un" s&
  s" pequeño" s?& s" gesto" s& s" con la mano," s?&
  s" casi imperceptible" s?&
  s" ..." s+  ;
  \ Mensaje de que el líder hace un gesto sobre la piedra.

: the-stone-must-be-in-its-place$  ( -- ca len )
  s" La piedra" s{ s" ha de" s" debe" s" tiene que" }s&
  s{ s" ser devuelta" s" devolverse" to-go-back$ }s&
  s{
    s" a su lugar" s" de encierro" s?&
    s" al lugar al que pertenece"
    s" al lugar del que nunca debió" s{ s" salir" s" ser sacada" s" ser arrancada" }s&
    s" al lugar que nunca debió" s{ s" abandonar" s" haber abandonado" }s&
  }s&  ;
  \ El líder dice que la piedra debe ser devuelta.

: the-leader-warns-about-the-stone  ( -- )
  stone-forbidden? @ already-warned
  you-can-not-take-the-stone$
  the-stone-must-be-in-its-place$ rnd2swap s& s&
  period+ speak  ;
  \ El líder habla acerca de la piedra.

: the-leader-points-to-the-north$  ( -- ca len )
  leader~ ^full-name
  s{ s" alza" s" extiende" s" levanta" }s&
  s{ s" su" s" el" }s& s" brazo" s&
  s{ s" indicando" s" en dirección" s" señalando" }s&
  toward-the(m)$ s& s" norte." s&  ;
  \ El líder se enfada y apunta al norte.
  \ XXX TODO -- crear ente "brazo" aquí, o activarlo como sinómino del anciano

: the-leader-points-to-the-north  ( -- )
  the-leader-points-to-the-north$ narrate  ;
  \ El líder se enfada y apunta al norte.

: nobody-passes-with-arms$  ( -- ca len )
  s{ s" Nadie" s" Ningún hombre" }s
  s{ s" con" s" llevando" s" portando" s" portador de"
  s" que porte" s" que lleve" }s&
  s{ s" armas" s" un arma" s" una espada" }s&
  with-him$ s&{ s" debe" s" puede" s" podrá" }s&
  s" pasar." s&  ;
  \ El líder dice que nadie pasa con armas.

: the-leader-warns-about-the-sword  ( -- )
  the-leader-points-to-the-north
  sword-forbidden? @ already-warned
  nobody-passes-with-arms$ s& speak  ;
  \ El líder habla acerca de la espada.

: the-leader-points-to-the-east  ( -- )
  s" El" old-man$ s& comma+
  s{ s" confiado" s" calmado" s" sereno" s" tranquilo" }s& comma+
  s{ s" indica" s" señala" }s&
  s{ toward-the(m)$ s" en dirección al" }s& s" este y" s&
  s{  s" te" s? s" dice" s&
      s" pronuncia las siguientes palabras"
  }s& colon+ narrate  ;
  \ El líder apunta al este.

: something-had-been-forbidden?  ( -- f )
  sword-forbidden? @ stone-forbidden? @ or  ;
  \ Se le prohibió alguna vez al protagonista pasar con algo prohibido?

: go-in-peace  ( -- )
  s{ s" Ya que" s" Puesto que" s" Dado que" s" Pues" }s
  something-had-been-forbidden? if
    s{ s" esta vez" s" ahora" s" en esta ocasión" s" por fin" s" finalmente" }s&
  then
  s{ s" vienes" s" llegas" s" has venido" s" has llegado" }s&
  s" en paz, puedes" s&
  s{ s" ir" s" marchar" s" continuar" s" tu camino" s?& }s&
  s" en paz." s& speak  ;
  \ El líder dice que puedes ir en paz.

: the-refugees-let-you-go  ( -- )
  s" todos" s? s" los refugiados" s& ^uppercase
  s" se apartan y" s& s" te" s?&
  s{  s" permiten" s{ s" el paso" s" pasar" }s&
      s" dejan" s" libre" s" el" s{ s" paso" s" camino" }s& rnd2swap s&
  }s& toward-the(m)$ s& s" este." s& narrate  ;
  \ Los refugiados te dejan pasar.

: the-leader-lets-you-go  ( -- )
  location-28~ location-29~ e-->
  the-leader-points-to-the-east
  go-in-peace the-refugees-let-you-go  ;
  \ El jefe deja marchar al protagonista.

: talked-to-the-leader  ( -- )
  leader~ conversations++
  recent-talks-to-the-leader ?++  ;
  \ Aumentar el contador de conversaciones con el jefe de los refugiados.

: we-are-refugees$  ( -- ca len )
  s" todos" s? s" nosotros" s? rnd2swap s&
  s" somos refugiados de" s& ^uppercase
  s{ s" la gran" s" esta terrible" }s& s" guerra." s&
  s" refugio" location-28~ ms-name!  ;
  \ Mensaje «Somos refugiados».

: we-are-refugees  ( -- )
  we-are-refugees$ we-want-peace$ s& speak narration-break  ;
  \ Somos refugiados.

: the-leader-trusts  ( -- )
  s" El" old-man$ s& s" asiente" s&
  s{ s" confiado" s" con confianza" }s& s" y," s&
  s" con un suave gesto" s& s" de su mano" s?& comma+
  s" te interrumpe para" s&
  s{  s" explicar" s{ s" te" s" se" null$ }s+
      s" presentarse" s" contarte" s" decir" s" te" s?+
  }s& colon+ narrate  ;
  \ El líder te corta, confiado.

: untrust$  ( -- ca len )
  s{ s" desconfianza" s" nerviosismo" s" impaciencia" }s  ;

: the-leader-does-not-trust  ( -- )
  s" El" old-man$ s& s" asiente" s& s" con la cabeza" s?& comma+
  s{  s" desconfiado" s" nervioso" s" impaciente"
      s" mostrando" s" claros" s?& s" signos de" s& untrust$ s&
      s{ s" dando" s" con" }s s" claras" s?& s" muestras de" s& untrust$ s&
  }s& comma+ s" y te interrumpe:" s& narrate  ;
  \ El líder te corta, desconfiado.

: the-leader-introduces-himself  ( -- )
  do-you-hold-something-forbidden?
  if    the-leader-does-not-trust
  else  the-leader-trusts
  then  we-are-refugees  ;
  \ El líder se presenta.

: first-conversation-with-the-leader  ( -- )
  my-name-is$ s" Ulfius y..." s& speak talked-to-the-leader
  the-leader-introduces-himself  ;
  \ XXX TODO -- elaborar mejor el texto

: the-leader-forbids-the-stone  ( -- )
  the-leader-warns-about-the-stone
  stone-forbidden? ?++  \ Recordarlo
  gesture-about-the-stone$ narrate narration-break
  a-man-takes-the-stone  ;
  \ El jefe te avisa de que no puedes pasar con la piedra y te la quita.

: the-leader-forbids-the-sword  ( -- )
  the-leader-warns-about-the-sword  sword-forbidden? ?++  ;
  \ El jefe te avisa de que no puedes pasar con la espada.
  \ El programa recuerda este hecho incrementando un contador.

: the-leader-checks-what-you-carry  ( -- )
  true case
    stone~ is-accessible? of  the-leader-forbids-the-stone  endof
    sword~ is-accessible? of  the-leader-forbids-the-sword  endof
    the-leader-lets-you-go
  endcase  ;
  \ El jefe controla lo que llevas.
  \ XXX TODO -- mejorar para que se pueda pasar si dejamos el objeto
  \ en el suelo o se lo damos

: insisted-once$  ( -- ca len )
  s{ s" antes" s" una vez" }s  ;
  \ XXX TODO -- inconcluso

: insisted-twice$  ( -- ca len )
  s{ s" antes" s" dos veces" s" un par de veces" }s  ;
  \ XXX TODO -- inconcluso

: insisted-several-times$  ( -- ca len )
  s{ s" las otras" s" más de dos" s" más de un par de" s" varias" }s
  s" veces" s&  ;
  \ XXX TODO -- inconcluso

: insisted-many-times$  ( -- ca len )
  s{  s" demasiadas" s" incontables" s" innumerables"
      s" las otras" s" muchas" s" varias"
  }s  s" veces" s&  ;
  \ XXX TODO -- inconcluso

: times-insisted  ( u -- ca1 len1 )
  { times }
  true case
    times 0 = of  null$  endof  \ XXX OLD -- innecesario
    times 1 = of  insisted-once$  endof
    times 2 = of  insisted-twice$  endof
    times 6 < of  insisted-several-times$  endof
    insisted-many-times$ rot
  endcase  ;
  \ XXX TODO -- inconcluso

: please-don't-insist$  ( -- ca len )
  s{ s" os ruego que" s" os lo ruego," s" he de rogaros que" }s
  s" no insistáis" s&  ;
  \ Mensaje de que por favor no insistas.

: don't-insist$  ( -- ca len )
  s" ya" s?
  s{
    s" habéis sido" s{ s" avisado" s" advertido" }s&
    s" os lo he" s" mos" s?+ s{ s" hecho saber" s" advertido" s" dejado claro" }s&
    s" se os ha" s{ s" hecho saber" s" advertido" s" dejado claro" }s&
  }s&  ;
  \ XXX TODO -- inconcluso

: don't-insist  ( -- )
  times-insisted don't-insist$ rnd2swap s& period+ ^uppercase  ;
  \ XXX TODO -- inconcluso

: the-leader-ignores-you  ( -- )  ;
  \ El líder te ignora.
  \ XXX TODO

: (talk-to-the-leader)  ( -- )
  leader~ no-conversations?
  ?? first-conversation-with-the-leader
  the-leader-checks-what-you-carry  ;
  \ Hablar con el jefe.

: talk-to-the-leader  ( -- )
  recent-talks-to-the-leader @
  if    the-leader-ignores-you
  else  (talk-to-the-leader)  then  ;
  \ Hablar con el jefe, si se puede.

\ ----------------------------------------------
\ Conversaciones con Ambrosio

: talked-to-ambrosio  ( -- )  ambrosio~ conversations++  ;
  \ Aumentar el contador de conversaciones con Ambrosio.

: be-ambrosio's-name  ( ca len -- )
  ambrosio~ ms-name!
  ambrosio~ have-no-article
  ambrosio~ have-personal-name  ;
  \ Le pone a ambrosio su nombre de pila _ca len_.

: ambrosio-introduces-himself  ( -- )
  s" Hola, Ulfius."
  my-name-is$ s& s" Ambrosio" 2dup be-ambrosio's-name
  period+ s& speak  ;

: you-cry  ( -- )
  s" Por" s" primera" s" vez" rnd2swap s& s& s" en" s&
  s{ s" mucho" s" largo" }s& s" tiempo, te sientas y" s&
  s" le" s?& s{ s" cuentas" s" narras" s" relatas" }s&
  s" a alguien todo lo que ha" s&
  s{ s" sucedido" s" pasado" s" ocurrido" }s& period+
  s" Y, tras tanto" s& s" pesar" s?& s{ s" acontecido" s" vivido" }s&
  s" , lloras" s+{ s" desconsoladamente" s" sin consuelo" }s&
  period+ narrate  ;

: ambrosio-proposes-a-deal  ( -- )
  s" Ambrosio te propone un" s{ s" trato" s" acuerdo" }s& comma+
  s{  the-that(m)$ s" aceptas" s&
      s" con el" that(m)$ s&{ s" consientes" s" estás de acuerdo" }s&
      the-that(m)$ s" te parece justo" s&
  }s& colon+
  s" por ayudarlo a salir de" s&{ s" la" s" esta" }s& s" cueva," s&
  s{ s" objetos" s" útiles" }s& comma+
  s{ s" vitales" s" imprescindibles" s" necesarios" }s&
  s" para" s& s" el éxito de" s?&
  s{ s" la" s" tal" s" dicha" }s& s{ s" misión" s" empresa" }s&
  s" , te son entregados." s+ narrate
  torch~ be-hold  flint~ be-hold  ;

: ambrosio-let's-go  ( -- )
  s{  s" Bien"
      s{ s" Venga" s" Vamos" }s s" pues" s?&
  }s comma+ s" Ambrosio," s&
  s{  s{ s" iniciemos" s" emprendamos" }s{ s" la marcha" s" el camino" }s&
      s" pongámonos en" s{ s" marcha" s" camino" }s&
  }s& period+  speak
  location-46~ ambrosio~ be-there
  s" Te" s{ s" giras" s" das la vuelta" }s& s" para" s&
  s{  s{ s" comprobar" s" ver" }s s" si" s&
      s{ s" cerciorarte" s" asegurarte" }s s" de que" s&
  }s& s" Ambrosio te sigue," s& but$ s& s" ..." s+
  s{  s" ha desaparecido"
      s" se ha esfumado"
      s" no hay" s" ni" s?& s" rastro de él" s&
      s" ya" s? s" no está" s&
      s" ya" s? s" no hay nadie" s&
      s" ya" s? s" no ves a nadie" s&
      s" es como si se lo hubiera tragado la tierra"
  }s& period+ narrate  ;

: ambrosio-is-gone  ( -- )
  s{  suddenly|then$ s" piensas" rnd2swap s& s" en el" s&
      suddenly|then$ s" caes en la cuenta" rnd2swap s& s" del" s&
  }s ^uppercase s" hecho" s" curioso" rnd2swap s& s& s" de que" s&
  s{  s{ s" supiera" s" conociera" }s{ s" cómo te llamas" s" tu nombre" }s&
      s" te llamara por tu nombre"
  }s& s" ..." s+ narrate  ;

: (conversation-0-with-ambrosio)  ( -- )
  s" Hola, buen hombre." speak
  ambrosio-introduces-himself scene-break
  you-cry scene-break
  ambrosio-proposes-a-deal narration-break
  ambrosio-let's-go narration-break
  ambrosio-is-gone
  talked-to-ambrosio  ;
  \ Primera conversación con Ambrosio.

: conversation-0-with-ambrosio  ( -- )
  location-19~ am-i-there?
  ?? (conversation-0-with-ambrosio)  ;
  \ Primera conversación con Ambrosio, si se dan las condiciones.

: i-am-stuck-in-the-cave$  ( -- ca len )
  s{  s" por desgracia" s" desgraciadamente" s" desafortunadamente"
      s" tristemente" s" lamentablemente"
  }s? s{ s" estoy" s" me encuentro" s" me hallo" }s& ^uppercase
  s{ s" atrapado" s" encerrado" }s&
  s" en" s&{ s" la" s" esta" }s& s" cueva" s&
  s{ s" debido a" s" por causa de" s" por influjo de" }s&
  s{ s" una" s" cierta" }s& s" magia de" s&
  s{ s" maligno" s" maléfico" s" malvado" s" terrible" }s&
  s" poder." s&  ;

: you-must-follow-your-way$  ( -- ca len )
  s{ s" En cuanto" s" Por lo que respecta" }s&
  s" al camino, vos" s&
  s{ s" habéis de" s" debéis" s" habréis de" }s&
  s{ s" recorrer" s" seguir" s" hacer " }s& s" el vuestro," s&
  s{ s" ver" s" mirar" s" contemplar" }s s" lo" s?+
  s" todo con vuestros" s& s" propios" s?& s" ojos." s&  ;

: ambrosio-explains  ( -- )
  s" Ambrosio"
  s{  s" parece meditar un instante"
      s" asiente ligeramente con la cabeza"
  }s& s" y" s&
  s{  s" te" s{ s" dice" s" explica" }s&
      s" se explica"
  }s&
  colon+ narrate
  i-am-stuck-in-the-cave$ you-must-follow-your-way$ s& speak  ;

: i-can-not-understand-it$  ( -- ca len )
  s" no"
  s{  s" lo" s? s{ s" entiendo" s" comprendo" }s&
      s{ s" alcanzo" s" acierto" }s s" a" s&
         s{ s" entender" s" comprender" }s& s" lo" s?+
  }s&  ;

: you-shake-your-head  ( -- )
  s{ s" Sacudes" s" Mueves" s" Haces un gesto con" }s s" la cabeza" s&
  s{  s{ s" poniendo"  s" dejando" }s
        s{ s" clara" s" de manifiesto" s" patente" s" manifiesta" }s&
      s{ s" manifestando" s" delatando" s" mostrando" }s s" claramente" s?&
  }s s" tu" s&
  s{ s" sorpresa" s" perplejidad" s" resignación" s" incredulidad" }s&? s&
  colon+ narrate  ;

: you-don't-understand  ( -- )
  s{  i-can-not-understand-it$ s" , la verdad" s?+
      s{ s" la verdad" s" lo cierto" }s s" es que" s&
        i-can-not-understand-it$ s&
      s{ s" en verdad" s" realmente" s" verdaderamente" }s
        i-can-not-understand-it$ s&
  }s ^uppercase speak  ;

: you-already-had-the-key$  ( -- ca len )
  s{
    s" La llave, Ambrosio, estaba ya en vuestro poder."
    s" Vos, Ambrosio, estabais ya en posesión de la llave."
    s" Vos, Ambrosio, ya teníais la llave en vuestro poder."
  }s  ;
  \ XXX TODO -- ampliar y variar

: you-know-other-way$  ( -- ca len )
  s" Y" s{ s" por lo demás" s" por otra parte" }s?&
  s{ s" es" s" parece" }s&
  s{ s" obvio" s" evidente" s" claro" s" indudable" }s&
  s" que" s&{ s" conocéis" s" sabéis" s" no desconocéis" }s&
  s{ s" un" s" algún" s" otro" }s& s" camino" s&
  s{  s" más" s{ s" corto" s" directo" s" fácil" s" llevadero" }s&
      s" menos" s{ s" largo" s" luengo" s" difícil" s" pesado" }s&
  }s& period+  ;

: you-reproach-ambrosio  ( -- )
  you-already-had-the-key$ you-know-other-way$ s& speak  ;
  \ Reprochas a Ambrosio acerca de la llave y el camino.

: (conversation-1-with-ambrosio)  ( -- )
  you-reproach-ambrosio ambrosio-explains
  you-shake-your-head you-don't-understand
  talked-to-ambrosio  ;
  \ Segunda conversación con Ambrosio.

: conversation-1-with-ambrosio  ( -- )
  location-46~ am-i-there?
  ambrosio-follows? 0=  and
  ?? (conversation-1-with-ambrosio)  ;
  \ Segunda conversación con Ambrosio, si se dan las condiciones.

: ambrosio-gives-you-the-key  ( -- )
  s{ s" Por favor," s" Os lo ruego," }s
  s" Ulfius," s&
  s" cumplid vuestra" s{ s" promesa." s" palabra." }s&
  s" Tomad" this|the(f)$ s& s" llave" s&
  s{ null$ s" en vuestra mano" s" en vuestras manos" s" con vos" }s&
  s" y abrid" s& s" la puerta de" s?& this|the(f)$ s& s" cueva." s&
  speak
  key~ be-hold  ;

: (conversation-2-with-ambrosio)  ( -- )
  ambrosio-gives-you-the-key
  ambrosio-follows? on  talked-to-ambrosio  ;
  \ Tercera conversación con Ambrosio.
  \ XXX TODO -- hacer que la llave se pueda transportar

: conversation-2-with-ambrosio  ( -- )
  location-45~ 1- location-47~ 1+ my-location within
  ?? (conversation-2-with-ambrosio)  ;
  \ Tercera conversación con Ambrosio, si se dan las condiciones.
  \ XXX TODO -- simplificar la condición

false [if]

  \ XXX OLD -- Primera versión, con una estructura `case`

: (talk-to-ambrosio)  ( -- )
  ambrosio~ conversations case
    0 of  conversation-0-with-ambrosio  endof
    1 of  conversation-1-with-ambrosio  endof
    2 of  conversation-2-with-ambrosio  endof
  endcase  ;
  \ Hablar con Ambrosio.
  \ XXX TODO -- Implementar qué hacer cuando ya no hay más
  \ conversaciones.

[else]

  \ XXX NEW -- Segunda versión, más «estilo Forth», con una tabla de
  \ ejecución.

create conversations-with-ambrosio
  ' (conversation-0-with-ambrosio) ,
  ' (conversation-1-with-ambrosio) ,
  ' (conversation-2-with-ambrosio) ,
  ' noop ,
  \ XXX TODO -- Implementar qué hacer cuando ya no hay más
  \ conversaciones.

: (talk-to-ambrosio)  ( -- )
  ambrosio~ conversations cells conversations-with-ambrosio + perform  ;
  \ Hablar con Ambrosio.

[then]

: talk-to-ambrosio  ( -- )
  ambrosio~ is-here?
  if  (talk-to-ambrosio)  else  ambrosio~ be-not-here  then  ;
  \ Hablar con Ambrosio, si se puede.
  \ XXX TODO -- esto debería comprobarse en `do-speak` o
  \ `do-speak-if-possible`.

\ ----------------------------------------------
\ Conversaciones sin éxito

: talk-to-something  ( a -- )
  2 random
  if    drop nonsense
  else  full-name s" hablar con" 2swap s& be-nonsense  then  ;
  \ Hablar con un ente que no es un personaje.
  \ XXX TODO

: talk-to-yourself$  ( -- ca len )
  s{  s" hablar" s{ s" solo" s" con uno mismo" }s&
      s" hablarse" s{ s" a sí" s" a uno" }s& s" mismo" s?&
  }s  ;
  \ Devuelve una variante de «hablar solo».

: talk-to-yourself  ( -- )  talk-to-yourself$ be-nonsense  ;
  \ Hablar solo.

\ ----------------------------------------------
\ Acciones

: do-speak-if-possible  ( a -- )
  [debug] [if]  s" En DO-SPEAK-IF-POSSIBLE" debug  [then]  \ XXX INFORMER
  case
    leader~ of  talk-to-the-leader  endof
    ambrosio~ of  talk-to-ambrosio  endof
    dup talk-to-something
  endcase  ;
  \ Hablar con un ente si es posible.

: (do-speak)  ( a | 0 -- )
  ?dup if  do-speak-if-possible  else  talk-to-yourself  then  ;
  \ Hablar con alguien o solo.

: (you-speak-to)  ( a -- )
  dup familiar++
  s" Hablas con" rot full-name s& colon+ narrate  ;

: you-speak-to  ( a | 0 -- )  ?dup ?? (you-speak-to)  ;

:noname  ( -- )
  [debug] [??] debug  \ XXX INFORMER
  main-complement{forbidden}
  actual-tool-complement{unnecessary}
  company-complement @ ?dup 0=  \ Si no hay complemento...
  ?? whom dup you-speak-to  \ ...buscar y mostrar el más probable.
  (do-speak)
  ; is do-speak
  \ Acción de hablar.

:noname  ( -- )
  main-complement @ ?dup 0=  \ Si no hay complemento...
  ?? unknown-whom  \ ...buscar el (desconocido) más probable.
  (do-speak)
  ; is do-introduce-yourself
  \ Acción de presentarse a alguien.

\ ----------------------------------------------
\ Guardar el juego

\ Para guardar el estado de la partida usaremos una solución muy
\ sencilla: ficheros de texto que reproduzcan el código Forth
\ necesario para restaurarlas. Esto permitirá restaurar una partida
\ con solo interpretar ese fichero como cualquier otro código fuente.

false [if]

  \ XXX TODO -- pendiente
  \ XXX OLD

: yyyymmddhhmmss$  ( -- ca len )
  time&date >yyyymmddhhmmss  ;
  \ Devuelve la fecha y hora actuales como una cadena en formato
  \ «aaaammddhhmmss».

: file-name$  ( -- ca len )
  \ Devuelve el nombre con que se grabará el juego.
  s" ayc-" yyyymmddhhmmss$ s+ s" .exe" windows? and s+  ;  \ Añadir
  sufijo si estamos en Windows

defer reenter

svariable filename

: (save-the-game)  ( -- )
\ false to spf-init?  \ Desactivar la inicialización del sistema
\ true to console?  \ Activar el modo de consola (no está claro en el manual)
\ false to gui?  \ Desactivar el modo gráfico (no está claro en el manual)
  ['] reenter to <main>  \ Actualizar la palabra que se ejecutará al arrancar
\ file-name$ save  new-page
  file-name$ filename place filename count save  ;
  \ Graba el juego.
  \ XXX TODO -- no está decidido el sistema que se usará para salvar
  \ las partidas
  \ XXX FIXME -- 2011-12-01 No funciona bien. Muestra mensajes de gcc con
  \ parámetros sacados de textos del programa!

: save-the-game
  main-complement{forbidden}
  action ? key drop  \ XXX INFORMER
  (save-the-game)  ;
  \ Acción de salvar el juego.

[then]

svariable game-file-name
  \ Nombre del fichero en que se graba la partida.

variable game-file-id
  \ Identificador del fichero en que se graba la partida.

: game-file-name$  ( -- ca len )  game-file-name count  ;
  \ Devuelve el nombre del fichero en que se graba la partida.

: close-game-file  ( -- )
  game-file-id @ close-file abort" Close file error."  ;
  \ Cierra el fichero en que se grabó la partida.
  \ XXX TODO -- mensaje de error definitivo

: create-game-file  ( ca len -- )
  r/w create-file abort" Create file error."
  game-file-id !  ;
  \ Crea un fichero de nambre _ca len_ para grabar una partida
  \ (sobreescribiendo otro que tuviera el mismo nombre).
  \ XXX TODO -- mensaje de error definitivo

wordlist constant restore-wordlist
  \ Palabras de restauración de una partida.

: read-game-file  ( ca len -- )
  restore-wordlist 1 set-order  included  restore-wordlists  ;
  \ Lee el fichero de configuración de nombre _ca len_.
  \ XXX TODO -- comprobar la existencia del fichero y atrapar errores
  \ al leerlo

: >file/  ( ca len -- )
  game-file-id @ write-line abort" Write file error"  ;
  \ Escribe una línea en el fichero de la partida.
  \ XXX TODO -- mensaje de error definitivo

: cr>file  ( -- )  s" " >file/  ;
  \ Escribe un final de línea en el fichero de la partida.

: >file  ( ca len -- )
  space+
  game-file-id @ write-file abort" Write file error"  ;
  \ Escribe una cadena en el fichero de la partida.
  \ XXX TODO -- mensaje de error definitivo

restore-wordlist set-current

' \ alias \
immediate

' true alias true

' false alias false

' s" alias s"

: load-entity  ( x0 ... xn u -- )
  #>entity >r
  \ cr .s  \ XXX INFORMER
  r@ ~direction !
  #>entity r@ ~in-exit !
  #>entity r@ ~out-exit !
  #>entity r@ ~down-exit !
  #>entity r@ ~up-exit !
  #>entity r@ ~west-exit !
  #>entity r@ ~east-exit !
  #>entity r@ ~south-exit !
  #>entity r@ ~north-exit !
  r@ ~familiar !
  r@ ~visits !
  #>entity r@ ~previous-location !
  #>entity r@ ~location !
  r@ ~owner !
  r@ ~bitfields /bitfields 1- 0 do  tuck i + c!  -1 +loop drop
  r@ ~times-open !
  r@ ~conversations !
  \ 2dup cr type .s  \ XXX INFORMER
  r> name!  ;
  \ Restaura los datos de un ente cuyo número ordinal es _u_.  _x0 ...
  \ xn_ son los datos del ente, en orden inverso a como los crea la
  \ palabra `save-entity`.
  \ XXX TODO -- reescribir, recuperando en bruto todas las celdas del
  \ registro, sin distinguir campos

: load-plot  ( x0 ... xn -- )
  recent-talks-to-the-leader !
  sword-forbidden? !
  stone-forbidden? !
  hacked-the-log? !
  climbed-the-fallen-away? !
  battle# !
  ambrosio-follows? !  ;
  \ Restaura las variables de la trama.
  \ Debe hacerse en orden alfabético inverso.

restore-wordlists

: string>file  ( ca len -- )
  bs| s" | 2swap s+ bs| "| s+ >file  ;
  \ Escribe una cadena en el fichero de la partida.

: f>string  ( f -- ca len )
  if  s" true"  else  s" false"  then  ;
  \ Convierte un indicador binario en su nombre de constante.

: f>file  ( f -- )  f>string >file  ;
  \ Escribe un indicador binario en el fichero de la partida.

: n>string  ( n -- ca len )
  s>d swap over dabs <# #s rot sign #> >sb  ;
  \ Convierte un número con signo en una cadena.

: u>string ( u -- ca len )  s>d <# #s #> >sb  ;
  \ Convierte un número sin signo en una cadena.

: 00>s  ( u -- ca1 len1 )  s>d <# # #s #> >sb  ;
  \ Convierte un número sin signo en una cadena (de dos dígitos como mínimo).

: 00>s+  ( u ca1 len1 -- ca2 len2 )  rot 00>s s+  ;
  \ Añade a una cadena un número tras convertirlo en cadena.

: yyyy-mm-dd-hh:mm:ss$  ( -- ca len )
  time&date 00>s hyphen+ 00>s+ hyphen+ 00>s+ space+
  00>s+ colon+ 00>s+ colon+ 00>s+  ;
  \ Devuelve la fecha y hora actuales como una cadena en formato
  \ «aaaa-mm-dd-hh:mm:ss».

: n>file  ( n -- )  n>string >file  ;
  \ Escribe un número con signo en el fichero de la partida.

: entity>file  ( a -- )  entity># n>file  ;
  \ Escribe la referencia a un ente _a_ en el fichero de la partida.
  \ Esta palabra es necesaria porque no es posible guardar y restaurar
  \ las direcciones de ficha de los entes, pues variarán con cada
  \ sesión de juego. Hay que guardar los números ordinales de las
  \ fichas y con ellos calcular sus direcciones durante la restauración.

: save-entity  ( u -- )
  dup #>entity >r
  r@ name string>file
  r@ conversations n>file
  r@ times-open n>file
  r@ ~bitfields /bitfields bounds do  i c@ n>file  loop
  r@ owner n>file
  r@ location entity>file
  r@ previous-location entity>file
  r@ visits n>file
  r@ familiar n>file
  r@ north-exit entity>file
  r@ south-exit entity>file
  r@ east-exit entity>file
  r@ west-exit entity>file
  r@ up-exit entity>file
  r@ down-exit entity>file
  r@ out-exit entity>file
  r@ in-exit entity>file
  r> direction n>file
  n>file  \ Número ordinal del ente
  s" load-entity" >file/  ;  \ Palabra que hará la restauración del ente
  \ Escribe los datos de un ente (cuyo número ordinal es _u_) en el
  \ fichero de la partida.
  \ XXX TODO -- reescribir, grabando en bruto todas las celdas del
  \ registro, sin distinguir campos

: rule>file  ( -- )
  s" \ ----------------------------------------------------" >file/  ;
  \ Escribe una línea de separación en el fichero de la partida.

: section>file  ( ca len -- )
  cr>file rule>file s" \ " >file >file/ rule>file cr>file  ;
  \ Escribe el título de una sección en el fichero de la partida.

: save-entities  ( -- )
  s" Entes" section>file #entities 0 do  i save-entity  loop  ;
  \ Escribe los datos de los entes en el fichero de la partida.

: save-config  ( -- )  s" Configuración" section>file  ;
  \ Escribe los valores de configuración en el fichero de la partida.
  \ XXX TODO

: save-plot  ( -- )
  s" Trama" section>file
  ambrosio-follows? @ f>file
  battle# @ n>file
  climbed-the-fallen-away? @ f>file
  hacked-the-log? @ f>file
  stone-forbidden? @ f>file
  sword-forbidden? @ f>file
  recent-talks-to-the-leader @ n>file
  s" load-plot" >file/  ;
  \ Escribe las variables de la trama en el fichero de la partida.
  \ Debe hacerse en orden alfabético.  Escribe también la palabra que
  \ hará la restauración de la trama

: file-header  ( -- )
  s" \ Datos de restauración de una partida de «Asalto y castigo»"
  >file/
  s" \ (http://pragramandala.net/es.programa.asalto_y_castigo.forth.html)"
  >file/
  s" \ Fichero creado en" yyyy-mm-dd-hh:mm:ss$ s& >file/  ;
  \ Escribe la cabecera del fichero de la partida.

: write-game-file  ( -- )
  file-header save-entities save-config save-plot  ;
  \ Escribe el contenido del fichero de la partida.

: fs+  ( ca1 len1 -- ca2 len2 )  s" .fs" s+  ;
  \ Añade la extensión «.fs» a un nombre de fichero _ca1 len1_.

: ?fs+  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" .fs" ends? ?exit  fs+  ;
  \ Añade la extensión «.fs» a un nombre de fichero _ca1 len1_,
  \ si no la tiene ya.

: (save-the-game)  ( ca len -- )
  ?fs+ create-game-file write-game-file close-game-file  ;
  \ Salva la partida.

: save-the-game  ( ca len -- )
  \ main-complement{forbidden} \ XXX TODO
  (save-the-game)  ;
  \ Acción de salvar la partida.

: continue-the-loaded-game  ( -- )
  scene-break new-page
  my-location describe-location  ;
  \ Continúa el juego en el punto que se acaba de restaurar.

: load-the-game  ( ca len -- )
  \ main-complement{forbidden}  \ XXX TODO
  restore-wordlist 1 set-order
  [debug-filing] [??] ~~
  \ included  \ XXX FIXME -- el sistema estalla
  \ 2drop  \ XXX NOTE: sin error
  \ cr type  \ XXX NOTE: sin error
  2>r save-input 2r>
  [debug-filing] [??] ~~
  ?fs+
  [debug-filing] [??] ~~
[false] [if]  \ XXX INFORMER
  read-game-file
[else]
  ['] read-game-file
  [debug-filing] [??] ~~
  catch
  [debug-filing] [??] ~~
  restore-wordlists
  [debug-filing] [??] ~~
  ?dup if
    ( ca len u2 ) nip nip
    case  \ XXX TMP
      2 of  s" Fichero no encontrado." narrate  endof
      s" Error al intentar leer el fichero." narrate
    endcase
    [debug-filing] [??] ~~
  then
[then]
  [debug-filing] [??] ~~
  restore-input
  if
    \ XXX TMP
    s" Error al intentar restaurar la entrada tras leer el fichero." narrate
  then
  [debug-filing] [??] ~~
  continue-the-loaded-game  ;
  \ Acción de salvar la partida.
  \ XXX FIXME -- no funciona bien?
  \ XXX TODO -- probarlo

\ ----------------------------------------------
\ Acciones de configuración

: recolor  ( -- )
  init-colors  new-page  my-location describe  ;
  \ XXX TODO rename

defer finish  ( -- )
  \ XXX TODO rename

\ vim:filetype=gforth:fileencoding=utf-8

