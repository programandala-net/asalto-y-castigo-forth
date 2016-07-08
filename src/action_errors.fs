\ action_errors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607082202

\ Note: The comments of the code are in Spanish.

\ XXX TODO -- finish the conversion to the new system: many errors
\ received their parameters on the stack, what is impossible with the
\ new system, because of `throw`. Other errors used the intermediate
\ variable `wrong-entity` because of that (see examples in Flibustre's
\ <action_conditions.fs>. This solution is being improved with
\ the double variable `error-message`.

\ ==============================================================
\ Error genérico

: generic-action-error$  ( -- ca len )
  'generic-action-error$ count  ;
  \ Devuelve el mensaje de error operativo para el nivel 1.

:noname ( -- )
  generic-action-error$ action-error.  ;
is generic-action-error
  \ Muestra el mensaje de error operativo para el nivel 1.

\ ==============================================================
\ Gestión de los errores específicos

' action-error. is specific-action-error

\ ==============================================================
\ Errores específicos

: known-entity>is-not-here$  ( a -- ca len )
  full-name s" no está" s&
  s{ s" aquí" s" por aquí" }s&  ;
  \  Devuelve mensaje de que un ente conocido no está presente.

: unknown-entity>is-not-here$  ( a -- ca len )
  s{ s" Aquí" s" Por aquí" }s
  s" no hay" s&
  rot subjective-negative-name s&  ;
  \  Devuelve mensaje de que un ente desconocido no está presente

:noname  ( -- )
  wrong-entity @ dup is-familiar?
  if    known-entity>is-not-here$
  else  unknown-entity>is-not-here$
  then  period+ action-error  ;
  \  Informa de que un ente no está presente.

to is-not-here-what-error#

:noname  ( -- )
  ^cannot-see$
  wrong-entity @ subjective-negative-name-as-direct-object s&
  period+ action-error  ;
  \ Informa de que el ente `wrong-entity` no puede ser visto.

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

:noname  ( ca len -- )
  error-message 2@
  ['] x-is-impossible$
  ['] it-is-impossible-x$
  2 choose execute  period+ action-error  ;
  \ Informa de que una acción indicada (en infinitivo) _ca len_ es
  \ imposible.

to that-is-impossible-error#

:noname  ( -- )
  something-like-that$ that-is-impossible-error# execute  ;
  \ Informa de que una acción no especificada es imposible.

to impossible-error#

: try$  ( -- ca len )  s{ null$ null$ s" intentar" }s  ;
  \ Devuelve «intentar» o una cadena vacía.

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

:noname  ( ca len -- )
  error-message 2@
  ['] x-is-nonsense$
  ['] it-is-nonsense-x$
  2 choose execute  period+ action-error  ;
  \ Informa de que una acción _ca len_ (verbo en infinitivo,
  \ sustantivo o cadena vacía) no tiene sentido.

to that-is-nonsense-error#

:noname  ( -- )  s" eso" that-is-nonsense-error# execute  ;
  \ Informa de que alguna acción no especificada no tiene sentido.
  \ XXX TMP

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

:noname  ( ca len -- )
  error-message 2@
  ['] x-is-dangerous$
  ['] it-is-dangerous-x$
  2 choose execute  period+ action-error  ;
  \ Informa de que una acción dada _ca len_ (en infinitivo, o vacía)
  \ no tiene sentido.

to that-is-dangerous-error#

:noname  ( -- )
  something-like-that$ that-is-dangerous-error# execute  ;
  \ Informa de que alguna acción no especificada no tiene sentido.

to dangerous-error#

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

:noname  ( ca len | ca 0 -- )
  error-message 2@
  no-reason-for$ 2swap s& period+ action-error  ;
  \ Informa de que no hay motivo para una acción (en infinitivo) _ca
  \ len_.

to no-reason-for-that-error#

:noname  ( -- )
  something-like-that$ no-reason-for-that-error# execute  ;
  \ Informa de que no hay motivo para una acción no especificada.
  \ XXX TODO

to no-reason-error#

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
  \ s" parece importante"
  \ s" parece necesario"
  s" tiene importancia"
  s" tiene utilidad"
  }s&  ;
  \ Segunda versión posible del mensaje de `do-not-worry`.

:noname  ( -- )
  ['] (do-not-worry-0)$
  ['] (do-not-worry-1)$ 2 choose execute
  now-or-null$ s&  period+ action-error  ;
  \ Informa de que una acción no tiene importancia.

is do-not-worry-error#

:noname  ( -- )
  wrong-entity @ full-name s" No necesitas" 2swap s&
  s" para" s& error-message 2@ s&
  period+ action-error  ;
  \ Informa de que el ente `wrong-entity` es innecesario como
  \ herramienta para ejecutar una acción descrita en la cadena
  \ apuntada por `error-message` (frase con verbo en infinitivo).
  \
  \ XXX TODO -- inconcluso

to unnecessary-tool-for-that-error#

:noname  ( -- )
  wrong-entity @
  ['] full-name ['] negative-full-name 2 choose execute
  s" No" s{ s" te" s? s" hace falta" s&
  s" necesitas" s" se necesita"
  s" precisas" s" se precisa"
  s" hay necesidad de" s{ s" usar" s" emplear" s" utilizar" }s?&
  }s&  2swap s&
  s{ s" para nada" s" para eso" }s?&  period+ action-error  ;
  \ Informa de que un ente es innecesario como herramienta
  \ para ejecutar una acción sin especificar.
  \ a = Ente innecesario
  \ XXX TODO -- añadir variante "no es/son necesaria/o/s
  \ XXX TODO -- ojo con entes especiales: personas, animales, virtuales..
  \ XXX TODO -- añadir coletilla "aunque la/lo/s tuvieras"?

to unnecessary-tool-error#

: >that$  ( a -- ca len )
  2 random if  drop s" eso"  else  full-name  then  ;
  \  Devuelve en _ca len_ el nombre de un ente _a_,
  \  o bien, al azar, "eso".

: you-do-not-have-it-(0)$  ( a -- ca len )
  s" No" you-carry$ s& rot >that$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 0).

: you-do-not-have-it-(1)$  ( a -- ca len )
  s" No" rot direct-pronoun s& you-carry$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 1, solo para entes conocidos).

: you-do-not-have-it-(2)$  ( a -- ca len )
  s" No" you-carry$ s& rot full-name s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 2, solo para entes no citados en el comando).

:noname  ( a -- )
  dup is-known?
  if    ['] you-do-not-have-it-(0)$
        ['] you-do-not-have-it-(1)$
        2 choose execute
  else  you-do-not-have-it-(0)$
  then  period+ action-error  ;
  \ Informa de que el protagonista no tiene un ente.

to you-do-not-have-it-error#

:noname  ( -- )
  wrong-entity @ you-do-not-have-it-error# execute  ;
  \ Informa de que el protagonista no tiene el ente `wrong-entity`.

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

:noname  ( -- )  not-by-hand$ action-error  ;
  \ Informa de que la acción no puede hacerse sin una herramienta.

to not-by-hand-error#

:noname  ( a -- )
  2 random if  you-do-not-have-it-(2)$ period+ action-error
           else  drop not-by-hand-error# execute  then  ;
  \ Informa de que el protagonista no tiene un ente necesario.

to you-need-it-error#

:noname ( -- )  wrong-entity @ you-need-it-error# execute  ;
  \ Informa de que el protagonista no tiene el ente `wrong-entity` necesario.

to you-need-what-error#

: >you-already-have-it-(0)$  ( a -- ca len )
  s" Ya" you-carry$ s& rot >that$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 0).

: >you-already-have-it-(1)$  ( a -- ca len )
  s" Ya" rot direct-pronoun s& you-carry$ s& with-you$ s&  ;
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 1, solo para entes conocidos).

:noname  ( a -- )
  dup familiar over belongs-to-protagonist? or   if
    ['] >you-already-have-it-(0)$
    ['] >you-already-have-it-(1)$
    2 choose execute
  else  >you-already-have-it-(0)$
  then  period+ action-error  ;
  \ Informa de que el protagonista ya tiene un ente.

to you-already-have-it-error#

:noname  ( a -- )  wrong-entity @ you-already-have-it-error# execute  ;
  \ Informa de que el protagonista ya tiene el ente `wrong-entity`.

to you-already-have-what-error#

: >you-do-not-wear-it$  ( a -- ca len )
  >r s" No llevas puest" r@ noun-ending+
  r> full-name s& period+  ;
  \ Informa de que el protagonista no lleva puesto un ente prenda.

:noname  ( a -- )
  dup is-hold?
  if    you-do-not-have-it-error# execute
  else  >you-do-not-wear-it$ action-error  then  ;
  \ Informa de que el protagonista no lleva puesto un ente prenda,
  \ según lo lleve o no consigo.

to you-do-not-wear-it-error#

:noname  ( -- )  wrong-entity @ you-do-not-wear-it-error# execute  ;
  \ Informa de que el protagonista no lleva puesto el ente `wrong-entity`,
  \ según lo lleve o no consigo.

to you-do-not-wear-what-error#

:noname  ( a -- )
  >r s" Ya llevas puest" r@ noun-ending+
  r> full-name s& period+ action-error  ;
  \ Informa de que el protagonista lleva puesto un ente prenda.

to you-already-wear-it-error#

:noname  ( -- )  wrong-entity @ you-already-wear-it-error# execute  ;
  \ Informa de que el protagonista lleva puesto el ente `wrong-entity`.

to you-already-wear-what-error#

: >not-with-that$  ( a -- ca len )
  full-name 2>r
  s{ s" Con eso no..."
     s" No con eso..."
     s" Con" 2r@ s& s" no..." s&
     s" No con" 2r> s& s" ..." s&
  }s  ;
  \ Devuelve mensaje de `not-with-that`.

:noname  ( -- )
  wrong-entity @ >not-with-that$ action-error  ;
  \ Informa de que la acción no puede hacerse con la herramienta elegida.

to wrong-tool-error#

:noname  ( a -- )
  s" Ya está abiert" rot noun-ending+ period+ action-error  ;
  \ Informa de que un ente ya está abierto.

to it-is-already-open-error#

:noname  ( -- )  wrong-entity @ it-is-already-open-error# execute  ;
  \ Informa de que el ente `wrong-entity` ya está abierto.

to what-is-already-open-error#

:noname  ( a -- )
  s" Ya está cerrad" r@ noun-ending+ period+ action-error.  ;
  \ Informa de que un ente ya está cerrado.

to it-is-already-closed-error#

:noname  ( -- )  wrong-entity @ it-is-already-closed-error# execute  ;
  \ Informa de que el ente `wrong-entity` ya está cerrado.

to what-is-already-closed-error#

: >toward-that-direction$  ( a -- ca len )
  dup >r  has-no-article?
  if    s" hacia" r> full-name
  else  toward-the(m)$ r> name
  then  s&  ;
  \ Devuelve «al/hacia la dirección indicada», correspondiente al ente
  \ dirección _a_.

:noname  ( -- )
  ^is-impossible$ s" ir" s&  wrong-entity @
  3 random if    >toward-that-direction$
           else  drop that-way$
           then  s& period+ action-error  ;
  \ El movimiento es imposible hacia el ente dirección en
  \ `wrong-entity`.
  \ XXX TODO -- añadir una tercera variante «ir en esa dirección»; y
  \ otras específicas como «no es posible subir»

to impossible-move-to-it-error#

\ ==============================================================
\ Atajos para errores frecuentes

: nonsense  ( -- )
  nonsense-error# throw  ;

: that-is-nonsense  ( ca len -- )
  error-message 2! that-is-nonsense-error# throw  ;

: cannot-see-what  ( a -- )
  wrong-entity ! cannot-see-what-error# throw  ;

: do-not-worry  ( -- )
  do-not-worry-error# throw  ;

: impossible-move-to-it  ( a -- )
  wrong-entity ! impossible-move-to-it-error# throw  ;

: unnecessary-tool  ( a -- )
  wrong-entity ! unnecessary-tool-error# throw  ;

: unnecessary-tool-for-that  ( ca len a -- )
  wrong-entity ! error-message 2!
  unnecessary-tool-for-that-error# throw  ;

: it-is-already-open  ( a -- )
  wrong-entity ! it-is-already-open-error# throw  ;

: no-reason  ( -- )
  no-reason-error# throw  ;

: no-reason-for-that  ( ca len -- )
  error-message 2! no-reason-for-that-error# throw  ;

: that-is-impossible  ( ca len -- )
  error-message 2! that-is-impossible-error# throw  ;

: that-is-dangerous  ( ca len -- )
  error-message 2! that-is-dangerous-error# throw  ;

: wrong-tool  ( a -- )
  wrong-entity ! wrong-tool-error# throw  ;

: nonsense-or-no-reason  ( -- )
  nonsense-error# no-reason-error# 2 choose throw  ;
  \ Informa de que una acción no especificada no tiene sentido o no tiene motivo.
  \ XXX TODO -- aún no usado

\ XXX TODO mover a Flibustre

\ vim:filetype=gforth:fileencoding=utf-8

