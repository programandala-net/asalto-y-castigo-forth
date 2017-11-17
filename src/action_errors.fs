\ action_errors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711072218

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/choose.fs                  \ `choose`
require galope/s-curly-bracket.fs         \ `s{`
require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/txt-plus.fs                \ `txt+`

set-current

\ ==============================================================
\ Error genérico

: generic-action-error$ ( -- ca len )
  'generic-action-error$ count ;
  \ Devuelve el mensaje de error operativo para el nivel 1.

:noname ( ca len -- )
  2drop generic-action-error$ action-error. ;
is generic-action-error
  \ Muestra el mensaje de error operativo para el nivel 1.

\ ==============================================================
\ Gestión de los errores específicos

' action-error. is specific-action-error

\ ==============================================================
\ Errores específicos

: >known-is-not-here$ ( a -- ca len )
  >r s{ r@ ^full-name s" no está aquí" txt+
        s" Aquí no está" r> full-name txt+ }s ;
  \  Devuelve mensaje de que un ente conocido no está presente.

: >unknown-is-not-here$ ( a -- ca len )
  s{ s" Aquí" s" Por aquí" }s s" no hay" txt+
  rot subjective-negative-name txt+ ;
  \  Devuelve mensaje de que un ente desconocido no está presente

:noname ( a -- )
  dup is-known?
  if    >known-is-not-here$
  else  >unknown-is-not-here$
  then  period+ action-error ;
  \  Informa de que un ente _a_ no está presente.

is is-not-here.error

:noname ( a -- )
  ^cannot-see$
  rot subjective-negative-name-as-direct-object txt+
  period+ action-error ;
  \ Informa de que el ente _a_ no puede ser visto.

is cannot-be-seen.error

: like-that$ ( -- ca len )
  s{ s" así" s" como eso" }s ;
  \ XXX TODO -- no usado

: something-like-that$ ( -- ca len )
  s" hacer" 50%nullify
  s{ s" algo así"
  s" algo semejante"
  s" eso"
  s" semejante cosa"
  s" tal cosa"
  s" una cosa así" }s txt+ ;
  \ Devuelve una variante de «hacer eso».

: is-impossible$ ( -- ca len )
  s{
  s" es imposible"
  \ s" es inviable"
  s" no es posible"
  \ s" no es viable"
  \ s" no sería posible"
  \ s" no sería viable"
  \ s" sería imposible"
  \ s" sería inviable"
  }s ;
  \ Devuelve una variante de «es imposible», que formará parte de
  \ mensajes personalizados por cada acción.

: ^is-impossible$ ( -- ca len ) is-impossible$ xcapitalized ;
  \ Devuelve una variante de «Es imposible» (con la primera letra en
  \ mayúsculas) que formará parte de mensajes personalizados por cada
  \ acción.

: x-is-impossible$ ( ca1 len1 -- ca2 len2 )
  dup if    xcapitalized is-impossible$ txt+
      else  2drop ^is-impossible$  then ;
  \ Devuelve una variante de «X es imposible».

: it-is-impossible-x$ ( ca1 len1 -- ca2 len2 )
  ^is-impossible$ 2swap txt+ ;
  \ Devuelve una variante de «Es imposible x».

:noname ( ca len -- )
  ['] x-is-impossible$
  ['] it-is-impossible-x$
  2 choose execute  period+ action-error ;
  \ Informa de que una acción indicada (en infinitivo) _ca len_ es
  \ imposible.

is that-is-impossible.error

:noname ( -- )
  something-like-that$ that-is-impossible.error ;
  \ Informa de que una acción no especificada es imposible.

is impossible.error

: try$ ( -- ca len ) s{ null$ null$ s" intentar" }s ;
  \ Devuelve «intentar» o una cadena vacía.

: nonsense$ ( -- ca len )
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
  }s ;
  \ Devuelve una variante de «no tiene sentido»,
  \ que formará parte de mensajes personalizados por cada acción.
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos

: ^nonsense$ ( -- ca len ) nonsense$ xcapitalized ;
  \ Devuelve una variante de «No tiene sentido»
  \ (con la primera letra en mayúsculas)
  \ que formará parte de mensajes personalizados por cada acción.

: x-is-nonsense$ ( ca1 len1 -- ca2 len2 )
  dup if    try$ 2swap txt+ xcapitalized nonsense$ txt+
      else  2drop ^nonsense$  then ;
  \ Devuelve una variante de «X no tiene sentido».

: it-is-nonsense-x$ ( ca1 len1 -- ca2 len2 )
  ^nonsense$ try$ txt+ 2swap txt+ ;
  \ Devuelve una variante de «No tiene sentido x».

:noname ( ca len -- )
  ['] x-is-nonsense$
  ['] it-is-nonsense-x$
  2 choose execute  period+ action-error ;
  \ Informa de que una acción _ca len_ (verbo en infinitivo,
  \ sustantivo o cadena vacía) no tiene sentido.

is that-is-nonsense.error

:noname ( -- ) s" eso" that-is-nonsense.error ;
  \ Informa de que alguna acción no especificada no tiene sentido.
  \ XXX TMP

is nonsense.error

: dangerous$ ( -- ca len )
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
  }s ;
  \ Devuelve una variante de «es peligroso», que formará parte de
  \ mensajes personalizados por cada acción.
  \
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los
  \ casos y unificar

: ^dangerous$ ( -- ca len ) dangerous$ xcapitalized ;
  \ Devuelve una variante de «Es peligroso» (con la primera letra en
  \ mayúsculas) que formará parte de mensajes personalizados por cada
  \ acción.

: x-is-dangerous$ ( ca1 len1 -- ca2 len2 )
  dup if    try$ 2swap txt+ xcapitalized dangerous$ txt+
      else  2drop ^dangerous$  then ;
  \ Devuelve una variante de «X es peligroso».

: it-is-dangerous-x$ ( ca1 len1 -- ca2 len2 )
  ^dangerous$ try$ txt+ 2swap txt+ ;
  \ Devuelve una variante de «Es peligroso x».

:noname ( ca len -- )
  ['] x-is-dangerous$
  ['] it-is-dangerous-x$
  2 choose execute  period+ action-error ;
  \ Informa de que una acción dada _ca len_ (en infinitivo, o vacía)
  \ no tiene sentido.

is that-is-dangerous.error

:noname ( -- )
  something-like-that$ that-is-dangerous.error ;
  \ Informa de que alguna acción no especificada no tiene sentido.

is dangerous.error

: no-reason-for$ ( -- ca len )
  s" No hay" s{
    s" nada que justifique"
    s{  s" necesidad" s" alguna" 50%nullify txt+
        s" ninguna necesidad"
    }s s" de" txt+
    s{  s" ninguna razón"
        s" ningún motivo"
        s" motivo" s" alguno" 50%nullify txt+
        s" razón" s" alguna" 50%nullify txt+
    }s s" para" txt+
  }s txt+ ;
  \ Devuelve una variante de «no hay motivo para».
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos
  \ XXX TODO -- añadir «no es menester/necesario»
  \ XXX TODO -- añadir «no hay lugar»

:noname ( ca len -- )
  no-reason-for$ 2swap txt+ period+ action-error ;
  \ Informa de que no hay motivo para una acción (en infinitivo) _ca
  \ len_.

is no-reason-for-that.error

:noname ( -- )
  something-like-that$ no-reason-for-that.error ;
  \ Informa de que no hay motivo para una acción no especificada.

is no-reason.error

: (do-not-worry-0)$ ( -- a u)
  s{
  s" Como si no hubiera"
  s" Hay"
  s" Se diría que hay"
  s" Seguro que hay"
  s" Sin duda hay"
  }s
  s{ s" cosas" s" tareas" s" asuntos" s" cuestiones" }s txt+
  s" más" txt+
  s{ s" importantes" s" urgentes" s" útiles" }s txt+
  s{
  null$ s" a que prestar atención" s" de que ocuparse"
  s" para ocuparse" s" para prestarles atención"
  }s txt+ ;
  \ Primera versión posible del mensaje de `do-not-worry`.

: (do-not-worry-1)$ ( -- a u)
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
  }s txt+ ;
  \ Segunda versión posible del mensaje de `do-not-worry`.

:noname ( -- )
  ['] (do-not-worry-0)$
  ['] (do-not-worry-1)$ 2 choose execute
  now-or-null$ txt+  period+ action-error ;
  \ Informa de que una acción no tiene importancia.

is do-not-worry.error

:noname ( ca len a -- )
  full-name s" No necesitas" 2swap txt+ s" para" txt+ 2swap txt+
  period+ action-error ;
  \ Informa de que el ente _a_ es innecesario como
  \ herramienta para ejecutar una acción descrita en la cadena
  \ _ca len_ (frase con verbo en infinitivo).

is unnecessary-tool-for-that.error

:noname ( a -- )
  ['] full-name ['] negative-full-name 2 choose execute
  s" No" s{ s" te" 50%nullify s" hace falta" txt+
  s" necesitas" s" se necesita"
  s" precisas" s" se precisa"
  s" hay necesidad de" s{ s" usar" s" emplear" s" utilizar" }s 50%nullify txt+
  }s txt+  2swap txt+
  s{ s" para nada" s" para eso" }s 50%nullify txt+  period+ action-error ;
  \ Informa de que un ente _a_ es innecesario como herramienta
  \ para ejecutar una acción sin especificar.
  \
  \ XXX TODO -- añadir variante "no es/son necesaria/o/s
  \ XXX TODO -- ojo con entes especiales: personas, animales, virtuales..
  \ XXX TODO -- añadir coletilla "aunque la/lo/s tuvieras"?

is unnecessary-tool.error

: >that$ ( a -- ca len )
  2 random if  drop s" eso"  else  full-name  then ;
  \  Devuelve en _ca len_ el nombre de un ente _a_,
  \  o bien, al azar, "eso".

: you-do-not-have-it-(0)$ ( a -- ca len )
  s" No" you-carry$ txt+ rot >that$ txt+ with-you$ txt+ ;
  \ Devuelve mensaje _ca len_ de que el protagonista no tiene un ente
  \ _a_ (variante 0).

: you-do-not-have-it-(1)$ ( a -- ca len )
  s" No" rot direct-pronoun txt+ you-carry$ txt+ with-you$ txt+ ;
  \ Devuelve mensaje _ca len_ de que el protagonista no tiene un ente
  \ _a_ (variante 1, solo para entes conocidos).

: you-do-not-have-it-(2)$ ( a -- ca len )
  s" No" you-carry$ txt+ rot full-name txt+ with-you$ txt+ ;
  \ Devuelve mensaje _ca len_ de que el protagonista no tiene un ente
  \ _a_ (variante 2, solo para entes no citados en el comando).

:noname ( a -- )
  dup is-known?
  if    ['] you-do-not-have-it-(0)$
        ['] you-do-not-have-it-(1)$
        2 choose execute
  else  you-do-not-have-it-(0)$
  then  period+ action-error ;
  \ Informa de que el protagonista no tiene un ente _a_.

is is-not-hold.error

: it-seems$ ( -- ca len )
  s{ null$ s" parece que" s" por lo que parece," }s ;

: it-is$ ( -- ca len )
  s{ s" es" s" sería" s" será" }s ;

: to-do-it$ ( -- ca len )
  s" hacerlo" 50%nullify ;

: possible-to-do$ ( -- ca len )
  it-is$ s" posible" txt+ to-do-it$ txt+ ;

: impossible-to-do$ ( -- ca len )
  it-is$ s" imposible" txt+ to-do-it$ txt+ ;

: can-be-done$ ( -- ca len )
  s{ s" podrá" s" podría" s" puede" }s
  s" hacerse" txt+ ;

: can-not-be-done$ ( -- ca len )
  s" no" can-be-done$ txt+ ;

: only-by-hand$ ( -- ca len )
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
  }s ;

: not-by-hand-0$ ( -- ca len )
  it-seems$
  s{
    s" no" possible-to-do$ txt+
    impossible-to-do$
    can-not-be-done$
  }s txt+
  only-by-hand$ txt+ period+ xcapitalized ;
  \ Devuelve la primera versión del mensaje de `not-by-hand.error`.

: some-tool$ ( -- ca len )
  s{
  s{ s" la" s" alguna" s" una" }s s" herramienta" txt+
  s{ s" adecuada" s" apropiada" }s txt+
  s{ s" el" s" algún" s" un" }s s" instrumento" txt+
  s{ s" adecuado" s" apropiado" }s txt+
  }s ;

: not-by-hand-1$ ( -- ca len )
  it-seems$
  s{
    s{ s" hará" s" haría" s" hace" }s s" falta" txt+
    s{
      s{ s" será" s" sería" s" es" }s s" menester" txt+
      s{ s" habrá" s" habría" s" hay" }s s" que" txt+
    }s s{ s" usar" s" utilizar" s" emplear" }s txt+
  }s txt+ some-tool$ txt+ period+ xcapitalized ;
  \ Devuelve la segunda versión del mensaje de `not-by-hand.error`.

: not-by-hand$ ( -- ca len )
  ['] not-by-hand-0$ ['] not-by-hand-1$ 2 choose execute xcapitalized ;
  \ Devuelve mensaje de `not-by-hand.error`.

:noname ( -- ) not-by-hand$ action-error ;
  \ Informa de que la acción no puede hacerse sin una herramienta.

is not-by-hand.error

:noname ( a -- )
  2 random if  you-do-not-have-it-(2)$ period+ action-error
           else  drop not-by-hand.error  then ;
  \ Informa de que el protagonista no tiene un ente _a_ necesario.

is is-needed.error

: >you-already-have-it-(0)$ ( a -- ca len )
  s" Ya" you-carry$ txt+ rot >that$ txt+ with-you$ txt+ ;
  \ Devuelve mensaje _ca len_ de que el protagonista ya tiene un ente
  \ _a_ (variante 0).

: >you-already-have-it-(1)$ ( a -- ca len )
  s" Ya" rot direct-pronoun txt+ you-carry$ txt+ with-you$ txt+ ;
  \ Devuelve mensaje _ca len_ de que el protagonista ya tiene un ente
  \ _a_ (variante 1, solo para entes conocidos).

:noname ( a -- )
  dup familiar over belongs-to-protagonist? or   if
    ['] >you-already-have-it-(0)$
    ['] >you-already-have-it-(1)$
    2 choose execute
  else  >you-already-have-it-(0)$
  then  period+ action-error ;
  \ Informa de que el protagonista ya tiene un ente _a_.

is is-hold.error

: >you-do-not-wear-it$ ( a -- ca len )
  >r s" No llevas puest" r@ noun-ending+
  r> full-name txt+ period+ ;
  \ Devuelve un mensaje _ca len_ de que el protagonista no lleva
  \ puesto un ente prenda _a_.

:noname ( a -- )
  dup is-hold?
  if    is-not-hold.error
  else  >you-do-not-wear-it$ action-error  then ;
  \ Informa de que el protagonista no lleva puesto un ente prenda _a_,
  \ según lo lleve o no consigo.

is is-not-worn-by-me.error

:noname ( a -- )
  >r s" Ya llevas puest" r@ noun-ending+
  r> full-name txt+ period+ action-error ;
  \ Informa de que el protagonista lleva puesto un ente prenda _a_.

is is-worn-by-me.error

: >not-with-that$ ( a -- ca len )
  full-name 2>r
  s{ s" Con eso no..."
     s" No con eso..."
     s" Con" 2r@ txt+ s" no..." txt+
     s" No con" 2r> txt+ s" ..." txt+
  }s ;
  \ Devuelve una variante _ca len_ del mensaje de «Con eso no», para
  \ el ente _a_.

:noname ( a -- )
  >not-with-that$ action-error ;
  \ Informa de que la acción no puede hacerse con el ente herramienta
  \ _a_.

is wrong-tool.error

:noname ( a -- )
  s" Lo intentas con" rot full-name txt+
  s" , pero no sirve de nada." s+ action-error ;
  \ Informa de que el ente herramienta _a_ no es adecuado.
  \ XXX TODO -- variar el mensaje

is useless-tool.error

:noname ( a -- )
  s" Ya está abiert" rot noun-ending+ period+
  action-error ;
  \ Informa de que un ente _a_ ya está abierto.

is is-open.error

:noname ( a -- )
  s" Ya está cerrad" rot noun-ending+ period+
  action-error ;
  \ Informa de que un ente _a_ ya está cerrado.

is is-closed.error

: >toward-that-direction$ ( a -- ca len )
  dup >r  has-no-article?
  if    s" hacia" r> full-name
  else  toward-the(m)$ r> name
  then  txt+ ;
  \ Devuelve en _ca len_ variante de mensaje «al/hacia la dirección
  \ indicada», correspondiente al ente dirección _a_.

:noname ( a -- )
  ^is-impossible$ s" ir" txt+  rot
  3 random if    >toward-that-direction$
           else  drop that-way$
           then  txt+ period+ action-error ;
  \ Informa de que el movimiento es imposible hacia el ente dirección
  \ _a_.
  \
  \ XXX TODO -- añadir una tercera variante «ir en esa dirección»; y
  \ otras específicas como «no es posible subir»

is impossible-move-to-it.error

: nonsense-or-no-reason.error ( -- )
  ['] nonsense.error ['] no-reason.error 2 choose execute ;

\ vim:filetype=gforth:fileencoding=utf-8

