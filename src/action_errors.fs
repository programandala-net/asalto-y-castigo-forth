\ action_errors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607071430

\ Note: The comments of the code are in Spanish.

\ ==============================================================

: generic-action-error$  ( -- ca len )
  'generic-action-error$ count  ;
  \ Devuelve el mensaje de error operativo para el nivel 1.

:noname ( -- )
  generic-action-error$ action-error.  ;
is generic-action-error
  \ Muestra el mensaje de error operativo para el nivel 1.

: known-entity>is-not-here$  ( a -- ca len )
  full-name s" no está" s&
  s{ s" aquí" s" por aquí" }s&  ;
  \  Devuelve mensaje de que un ente conocido no está presente.

: unknown-entity>is-not-here$  ( a -- ca len )
  s{ s" Aquí" s" Por aquí" }s
  s" no hay" s&
  rot subjective-negative-name s&  ;
  \  Devuelve mensaje de que un ente desconocido no está presente

: (is-not-here)  ( a -- )
  dup is-familiar?
  if    known-entity>is-not-here$
  else  unknown-entity>is-not-here$
  then  period+ action-error.  ;
  \  Informa de que un ente no está presente.

1 ' (is-not-here) action-error be-not-here
to is-not-here-error#

: (is-not-here-what)  ( -- )  what @ (is-not-here)  ;
  \  Informa de que el ente `what` no está presente.

0 ' (is-not-here-what) action-error be-not-here-what
to is-not-here-what-error#

: (cannot-see)  ( a -- )
  ^cannot-see$
  rot subjective-negative-name-as-direct-object s&
  period+ action-error.  ;
  \ Informa de que un ente no puede ser mirado.

1 ' (cannot-see) action-error cannot-see
to cannot-see-error#

: (cannot-see-what)  ( -- )
  what @ (cannot-see)  ;
  \ Informa de que el ente `what` no puede ser mirado.

0 ' (cannot-see-what) action-error cannot-see-what
to cannot-see-what-error#

: like-that$  ( -- ca len )
  s{ s" así" s" como eso" }s  ;
  \ XXX TODO -- no usado

: something-like-that$  ( -- ca len )
  s" hacer" s?
  s{ s" algo así"
  s" algo semejante"
  s" eso"
  s" semejante cosa"
  s" tal cosa"
  s" una cosa así" }s&  ;
  \ Devuelve una variante de «hacer eso».

: is-impossible$  ( -- ca len )
  s{
  s" es imposible"
  \ s" es inviable"
  s" no es posible"
  \ s" no es viable"
  \ s" no sería posible"
  \ s" no sería viable"
  \ s" sería imposible"
  \ s" sería inviable"
  }s  ;
  \ Devuelve una variante de «es imposible», que formará parte de
  \ mensajes personalizados por cada acción.

: ^is-impossible$  ( -- ca len )  is-impossible$ ^uppercase  ;
  \ Devuelve una variante de «Es imposible» (con la primera letra en
  \ mayúsculas) que formará parte de mensajes personalizados por cada
  \ acción.

: x-is-impossible$  ( ca1 len1 -- ca2 len2 )
  dup if    ^uppercase is-impossible$ s&
      else  2drop ^is-impossible$  then  ;
  \ Devuelve una variante de «X es imposible».

: it-is-impossible-x$  ( ca1 len1 -- ca2 len2 )
  ^is-impossible$ 2swap s&  ;
  \ Devuelve una variante de «Es imposible x».

: (is-impossible)  ( ca len -- )
  ['] x-is-impossible$
  ['] it-is-impossible-x$
  2 choose execute  period+ action-error.  ;
  \ Informa de que una acción indicada (en infinitivo) _ca len_ es
  \ imposible.

2 ' (is-impossible) action-error be-impossible drop

: (impossible)  ( -- )
  something-like-that$ (is-impossible)  ;
  \ Informa de que una acción no especificada es imposible.

0 ' (impossible) action-error impossible
to impossible-error#

: try$  ( -- ca len )  s{ null$ null$ s" intentar" }s  ;
  \ Devuelve una variante de «intentar» (o cadena vacía).

: nonsense$  ( -- ca len )
  s{
  s" es ilógico"
  s" no parece lógico"
  s" no parece muy lógico"
  s" no tiene lógica ninguna"
  s" no tiene lógica"
  s" no tiene mucha lógica"
  s" no tiene mucho sentido"
  s" no tiene ninguna lógica"
  s" no tiene ningún sentido"
  s" no tiene sentido"
  }s  ;
  \ Devuelve una variante de «no tiene sentido»,
  \ que formará parte de mensajes personalizados por cada acción.
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos

: ^nonsense$  ( -- ca len )  nonsense$ ^uppercase  ;
  \ Devuelve una variante de «No tiene sentido»
  \ (con la primera letra en mayúsculas)
  \ que formará parte de mensajes personalizados por cada acción.

: x-is-nonsense$  ( ca1 len1 -- ca2 len2 )
  dup if    try$ 2swap s& ^uppercase nonsense$ s&
      else  2drop ^nonsense$  then  ;
  \ Devuelve una variante de «X no tiene sentido».

: it-is-nonsense-x$  ( ca1 len1 -- ca2 len2 )
  ^nonsense$ try$ s& 2swap s&  ;
  \ Devuelve una variante de «No tiene sentido x».

: (is-nonsense)  ( ca len -- )
  ['] x-is-nonsense$
  ['] it-is-nonsense-x$
  2 choose execute  period+ action-error.  ;
  \ Informa de que una acción _ca len_ (verbo en infinitivo,
  \ sustantivo o cadena vacía) no tiene sentido.

2 ' (is-nonsense) action-error be-nonsense drop

: (nonsense)  ( -- )  s" eso" (is-nonsense)  ;
  \ Informa de que alguna acción no especificada no tiene sentido.
  \ XXX TMP

0 ' (nonsense) action-error nonsense
to nonsense-error#

: dangerous$  ( -- ca len )
  s{
  s" es algo descabellado"
  s" es descabellado"
  s" es muy arriesgado"
  s" es peligroso"
  s" es una insensatez"
  s" es una locura"
  s" no es seguro"
  s" no es sensato"
  s" sería algo descabellado"
  s" sería demasiado arriesgado"
  s" sería descabellado"
  s" sería peligroso"
  s" sería una insensatez"
  }s  ;
  \ Devuelve una variante de «es peligroso», que formará parte de
  \ mensajes personalizados por cada acción.
  \
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los
  \ casos y unificar

: ^dangerous$  ( -- ca len )  dangerous$ ^uppercase  ;
  \ Devuelve una variante de «Es peligroso» (con la primera letra en
  \ mayúsculas) que formará parte de mensajes personalizados por cada
  \ acción.

: x-is-dangerous$  ( ca1 len1 -- ca2 len2 )
  dup if    try$ 2swap s& ^uppercase dangerous$ s&
      else  2drop ^dangerous$  then  ;
  \ Devuelve una variante de «X es peligroso».

: it-is-dangerous-x$  ( ca1 len1 -- ca2 len2 )
  ^dangerous$ try$ s& 2swap s&  ;
  \ Devuelve una variante de «Es peligroso x».

: (is-dangerous)  ( ca len -- )
  ['] x-is-dangerous$
  ['] it-is-dangerous-x$
  2 choose execute  period+ action-error.  ;
  \ Informa de que una acción dada (en infinitivo)
  \ no tiene sentido.
  \ ca len = Acción que no tiene sentido, en infinitivo, o una cadena vacía

2 ' (is-dangerous) action-error be-dangerous drop

: (dangerous)  ( -- )
  something-like-that$ (is-dangerous)  ;
  \ Informa de que alguna acción no especificada no tiene sentido.

0 ' (dangerous) action-error dangerous
to dangerous-error#

: ?full-name&  ( ca len a | ca len 0 -- )
  ?dup if  full-name s&  then  ;
  \ Añade a una cadena _ca len_ el nombre de un posible ente _a_.
  \ XXX TODO -- no usado

: (+is-nonsense)  ( ca len a1 -- )
  ?dup if    full-name s& (is-nonsense)
       else  2drop nonsense  then  ;
  \ Informa de que una acción dada (en infinitivo)
  \ ejecutada sobre un ente no tiene sentido.
  \ ca len = Acción en infinitivo
  \ a1 = Ente al que se refiere la acción y cuyo objeto directo es (o cero)

3 ' (+is-nonsense) action-error +is-nonsense drop

: (main-complement+is-nonsense)  ( ca len -- )
  main-complement (+is-nonsense)  ;
  \ Informa de que una acción dada (en infinitivo) _ca len_, que hay
  \ que completar con el nombre del complemento principal, no tiene
  \ sentido.

2 ' (main-complement+is-nonsense) action-error main-complement+is-nonsense drop

: (secondary-complement+is-nonsense)  ( ca len -- )
  secondary-complement (+is-nonsense)  ;
  \ Informa de que una acción dada (en infinitivo) _ca len_, que hay
  \ que completar con el nombre del complemento secundario, no tiene
  \ sentido.

2 ' (secondary-complement+is-nonsense) action-error secondary-complement+is-nonsense drop

: no-reason-for$  ( -- ca len )
  s" No hay" s{
    s" nada que justifique"
    s{  s" necesidad" s" alguna" s?&
        s" ninguna necesidad"
    }s s" de" s&
    s{  s" ninguna razón"
        s" ningún motivo"
        s" motivo" s" alguno" s?&
        s" razón" s" alguna" s?&
    }s s" para" s&
  }s&  ;
  \ Devuelve una variante de «no hay motivo para».
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos

: (no-reason-for-that)  ( ca len | ca 0 -- )
  no-reason-for$ 2swap s& period+ action-error.  ;
  \ Informa de que no hay motivo para una acción (en infinitivo) _ca
  \ len_.

2 ' (no-reason-for-that) action-error no-reason-for-that drop

: (no-reason)  ( -- )
  something-like-that$ (no-reason-for-that)  ;
  \ Informa de que no hay motivo para una acción no especificada.
  \ XXX TODO

0 ' (no-reason) action-error no-reason drop

: (nonsense|no-reason)  ( -- )
  ['] nonsense ['] no-reason 2 choose execute  ;
  \ Informa de que una acción no especificada no tiene sentido o no tiene motivo.
  \ XXX TODO -- aún no usado

0 ' (nonsense|no-reason) action-error nonsense|no-reason drop

: (do-not-worry-0)$  ( -- a u)
  s{
  s" Como si no hubiera"
  s" Hay"
  s" Se diría que hay"
  s" Seguro que hay"
  s" Sin duda hay"
  }s
  s{ s" cosas" s" tareas" s" asuntos" s" cuestiones" }s&
  s" más" s&
  s{ s" importantes" s" urgentes" s" útiles" }s&
  s{
  null$ s" a que prestar atención" s" de que ocuparse"
  s" para ocuparse" s" para prestarles atención"
  }s&  ;
  \ Primera versión posible del mensaje de `do-not-worry`.

: (do-not-worry-1)$  ( -- a u)
  s" Eso no" s{
  s" es importante"
  s" es menester"
  s" es necesario"
  s" hace falta"
  s" importa"
  s" parece importante"
  s" parece necesario"
  s" tiene importancia"
  s" tiene utilidad"
  }s&  ;
  \ Segunda versión posible del mensaje de `do-not-worry`.

: do-not-worry  ( -- )
  ['] (do-not-worry-0)$
  ['] (do-not-worry-1)$ 2 choose execute
  now-or-null$ s&  period+ action-error.  ;
  \ Informa de que una acción no tiene importancia.
  \ XXX TMP, no se usa

: (unnecessary-tool-for-that)  ( ca len a -- )
  full-name s" No necesitas" 2swap s& s" para" s& 2swap s&
  period+ action-error.  ;
  \ Informa de que un ente _a_ es innecesario como herramienta
  \ para ejecutar una acción _ca len_ (frase con verbo en infinitivo).
  \ XXX TODO -- inconcluso

3 ' (unnecessary-tool-for-that) action-error unnecessary-tool-for-that
to unnecessary-tool-for-that-error#

: (unnecessary-tool)  ( a -- )
  ['] full-name ['] negative-full-name 2 choose execute
  s" No" s{ s" te" s? s" hace falta" s&
  s" necesitas" s" se necesita"
  s" precisas" s" se precisa"
  s" hay necesidad de" s{ s" usar" s" emplear" s" utilizar" }s?&
  }s&  2swap s&
  s{ s" para nada" s" para eso" }s?&  period+ action-error.  ;
  \ Informa de que un ente es innecesario como herramienta
  \ para ejecutar una acción sin especificar.
  \ a = Ente innecesario
  \ XXX TODO -- añadir variante "no es/son necesaria/o/s
  \ XXX TODO -- ojo con entes especiales: personas, animales, virtuales..
  \ XXX TODO -- añadir coletilla "aunque la/lo/s tuvieras"?

1 ' (unnecessary-tool) action-error unnecessary-tool
to unnecessary-tool-error#

0 [if]

  \ XXX FIXME -- error «no tiene nada especial»
  \ XXX TODO -- inacabado

: it-is-normal-x$  ( ca1 len1 -- ca2 len2 )
  ^normal$ try$ s& 2swap s&  ;
  \ Devuelve una variante de «no tiene nada especial x».

: (is-normal)  ( a -- )
  ['] x-is-normal$
  ['] it-is-normal-x$
  2 choose execute  period+ action-error.  ;
  \ Informa de que un ente _a_ no tiene nada especial.

1 ' (is-normal) action-error be-normal
to is-normal-error#

[then]

: that$  ( a -- ca len )
  2 random if  drop s" eso"  else  full-name  then  ;
  \  Devuelve en _ca len_ el nombre de un ente _a_,
  \  o bien, al azar, "eso".

: you-do-not-have-it-(0)$  ( a -- )
  s" No" you-carry$ s& rot that$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 0).

: you-do-not-have-it-(1)$  ( a -- )
  s" No" rot direct-pronoun s& you-carry$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 1, solo para entes conocidos).

: you-do-not-have-it-(2)$  ( a -- )
  s" No" you-carry$ s& rot full-name s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 2, solo para entes no citados en el comando).

: (you-do-not-have-it)  ( a -- )
  dup is-known? if
    ['] you-do-not-have-it-(0)$
    ['] you-do-not-have-it-(1)$
    2 choose execute
  else  you-do-not-have-it-(0)$
  then  period+ action-error.  ;
  \ Informa de que el protagonista no tiene un ente.

1 ' (you-do-not-have-it) action-error you-do-not-have-it
to you-do-not-have-it-error#

: (you-do-not-have-what)  ( -- )
  what @ (you-do-not-have-it)  ;
  \ Informa de que el protagonista no tiene el ente `what`.

0 ' (you-do-not-have-what) action-error you-do-not-have-what
to you-do-not-have-what-error#

: it-seems$  ( -- ca len )
  s{ null$ s" parece que" s" por lo que parece," }s  ;

: it-is$  ( -- ca len )
  s{ s" es" s" sería" s" será" }s  ;

: to-do-it$  ( -- ca len )
  s" hacerlo" s?  ;

: possible-to-do$  ( -- ca len )
  it-is$ s" posible" s& to-do-it$ s&  ;

: impossible-to-do$  ( -- ca len )
  it-is$ s" imposible" s& to-do-it$ s&  ;

: can-be-done$  ( -- ca len )
  s{ s" podrá" s" podría" s" puede" }s
  s" hacerse" s&  ;

: can-not-be-done$  ( -- ca len )
  s" no" can-be-done$ s&  ;

: only-by-hand$  ( -- ca len )
  s{
  s" con la sola ayuda de las manos"
  s" con las manos como única herramienta"
  s" con las manos desnudas"
  s" con las manos"
  s" simplemente con las manos"
  s" sin alguna herramienta"
  s" sin herramientas"
  s" sin la herramienta adecuada"
  s" sin una herramienta"
  s" solamente con las manos"
  s" solo con las manos"
  s" tan solo con las manos"
  s" únicamente con las manos"
  }s  ;

: not-by-hand-0$  ( -- ca len )
  it-seems$
  s{
    s" no" possible-to-do$ s&
    impossible-to-do$
    can-not-be-done$
  }s&
  only-by-hand$ s& period+ ^uppercase  ;
  \ Devuelve la primera versión del mensaje de NOT-BY-HAND.

: some-tool$  ( -- ca len )
  s{
  s{ s" la" s" alguna" s" una" }s s" herramienta" s&
  s{ s" adecuada" s" apropiada" }s&
  s{ s" el" s" algún" s" un" }s s" instrumento" s&
  s{ s" adecuado" s" apropiado" }s&
  }s  ;

: not-by-hand-1$  ( -- ca len )
  it-seems$
  s{
    s{ s" hará" s" haría" s" hace" }s s" falta" s&
    s{
      s{ s" será" s" sería" s" es" }s s" menester" s&
      s{ s" habrá" s" habría" s" hay" }s s" que" s&
    }s{ s" usar" s" utilizar" s" emplear" }s&
  }s& some-tool$ s& period+ ^uppercase  ;
  \ Devuelve la segunda versión del mensaje de `not-by-hand`.

: not-by-hand$  ( -- ca len )
  ['] not-by-hand-0$ ['] not-by-hand-1$ 2 choose execute ^uppercase  ;
  \ Devuelve mensaje de `not-by-hand`.

: (not-by-hand)  ( -- )  not-by-hand$ action-error.  ;
  \ Informa de que la acción no puede hacerse sin una herramienta.

0 ' (not-by-hand) action-error not-by-hand drop

: (you-need)  ( a -- )
  2 random if  you-do-not-have-it-(2)$ period+ action-error.
           else  drop (not-by-hand)  then  ;
  \ Informa de que el protagonista no tiene un ente necesario.

1 ' (you-need) action-error you-need drop

: (you-need-what)  ( -- )  what @ (you-need)  ;
  \ Informa de que el protagonista no tiene el ente `what` necesario.

0 ' (you-need-what) action-error  you-need-what
to you-need-what-error#

: you-already-have-it-(0)$  ( a -- )
  s" Ya" you-carry$ s& rot that$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 0).

: you-already-have-it-(1)$  ( a -- )
  s" Ya" rot direct-pronoun s& you-carry$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 1, solo para entes conocidos).

: (you-already-have-it)  ( a -- )
  dup familiar over belongs-to-protagonist? or   if
    ['] you-already-have-it-(0)$
    ['] you-already-have-it-(1)$
    2 choose execute
  else  you-already-have-it-(0)$
  then  period+ action-error.  ;
  \ Informa de que el protagonista ya tiene un ente.

1 ' (you-already-have-it) action-error you-already-have-it
to you-already-have-it-error#

: (you-already-have-what)  ( a -- )  what @ (you-already-have-it)  ;
  \ Informa de que el protagonista ya tiene el ente `what`.

1 ' (you-already-have-what) action-error you-already-have-what
to you-already-have-what-error#

: ((you-do-not-wear-it))  ( a -- )
  >r s" No llevas puest" r@ noun-ending+
  r> full-name s& period+ action-error.  ;
  \ Informa de que el protagonista no lleva puesto un ente prenda.

: (you-do-not-wear-it)  ( a -- )
  dup is-hold?
  if  (you-do-not-have-it)  else  ((you-do-not-wear-it))  then  ;
  \ Informa de que el protagonista no lleva puesto un ente prenda,
  \ según lo lleve o no consigo.

1 ' (you-do-not-wear-it) action-error you-do-not-wear-it drop

: (you-do-not-wear-what)  ( -- )  what @ (you-do-not-wear-it)  ;
  \ Informa de que el protagonista no lleva puesto el ente `what`,
  \ según lo lleve o no consigo.

0 ' (you-do-not-wear-what) action-error you-do-not-wear-what
to you-do-not-wear-what-error#

: (you-already-wear-it)  ( a -- )
  >r s" Ya llevas puest" r@ noun-ending+
  r> full-name s& period+ action-error.  ;
  \ Informa de que el protagonista lleva puesto un ente prenda.

1 ' (you-already-wear-it) action-error you-already-wear-it drop

: (you-already-wear-what)  ( -- )
  what @ (you-already-wear-it)  ;
  \ Informa de que el protagonista lleva puesto el ente `what`.

0 ' (you-already-wear-what) action-error you-already-wear-what
to you-already-wear-what-error#

: not-with-that$  ( -- ca len )
  s" Con eso no..." s" No con eso..." 2 schoose  ;
  \ Devuelve mensaje de `not-with-that`.

: (not-with-that)  ( -- )  not-with-that$ action-error.  ;
  \ Informa de que la acción no puede hacerse con la herramienta elegida.

0 ' (not-with-that) action-error not-with-that drop

: (it-is-already-open)  ( a -- )
  s" Ya está abiert" rot noun-ending+ period+ action-error.  ;
  \ Informa de que un ente ya está abierto.

1 ' (it-is-already-open) action-error it-is-already-open drop

: (what-is-already-open)  ( -- )
  what @ (it-is-already-open)  ;
  \ Informa de que el ente `what` ya está abierto.

0 ' (what-is-already-open) action-error what-is-already-open
to what-is-already-open-error#

: (it-is-already-closed)  ( a -- )
  s" Ya está cerrad" r@ noun-ending+ period+ action-error.  ;
  \ Informa de que un ente ya está cerrado.

1 ' (it-is-already-closed) action-error it-is-already-closed drop

: (what-is-already-closed)  ( -- )  what @ (it-is-already-closed)  ;
  \ Informa de que el ente `what` ya está cerrado.

0 ' (what-is-already-closed) action-error what-is-already-closed
to what-is-already-closed-error#

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
           then  s& period+ action-error.  ;
  \ El movimiento es imposible hacia el ente dirección _a_.
  \ XXX TODO -- añadir una tercera variante «ir en esa dirección»; y
  \ otras específicas como «no es posible subir»

1 ' (impossible-move) action-error impossible-move drop

\ vim:filetype=gforth:fileencoding=utf-8

