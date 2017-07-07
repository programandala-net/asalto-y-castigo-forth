\ the_end.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last modified 201607221417

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/question-question.fs  \ `??`
require galope/s-curly-bracket.fs    \ `s{`
require galope/txt-plus.fs           \ `txt+`

set-current

\ ==============================================================

: success?  ( -- f )  location-51~ am-i-there?  ;
  \ ¿Ha completado con éxito su misión el protagonista?

false [if]

: battle-phases  ( -- u )  5 random 7 +  ;
  \ Devuelve el número máximo de fases de la batalla
  \ (número al azar, de 8 a 11).
  \ XXX TODO -- no usado

[then]

: failure?  ( -- f )  battle# @ battle-phases >  ;
  \ ¿Ha fracasado el protagonista?

: .bye  ( -- )  s" ¡Adiós!" narrate  ;
  \ Mensaje final cuando el jugador no quiere jugar otra partida.
  \ XXX TMP

: farewell  ( -- )  new-page .bye bye  ;
  \ Abandona el programa.

: play-again?$  ( -- ca len )
  s{ s" ¿Quieres" s" ¿Te" s{ s" animas a" s" apetece" }s txt+ }s
  s{ s" jugar" s" empezar" }s txt+  again?$ txt+  ;
  \ Devuelve la pregunta que se hace al jugador tras haber completado
  \ con éxito el juego.

: retry?0$  ( -- ca len )
  s" ¿Tienes"
  s{ s" fuerzas" s" arrestos" s" agallas" s" energías" s" ánimos" }s txt+  ;
  \ Devuelve una variante para el comienzo de la pregunta que se hace
  \ al jugador tras haber fracasado.

: retry?1$  ( -- ca len )
  s{ s" ¿Te quedan" s" ¿Guardas" s" ¿Conservas" }s
  s{ s" fuerzas" s" energías" s" ánimos" }s txt+  ;
  \ Devuelve una variante para el comienzo de la pregunta que se hace
  \ al jugador tras haber fracasado.

: retry?$  ( -- ca len )
  s{ retry?0$ retry?1$ }s s" para" txt+
  s{ s" jugar" s" probar" s" intentarlo" }s txt+ again?$ txt+  ;
  \ Devuelve la pregunta que se hace al jugador tras haber fracasado.

: enough?  ( -- f )
  success? if  ['] play-again?$  else  ['] retry?$  then  cr no?  ;
  \ ¿Prefiere el jugador no jugar otra partida?
  \ XXX TODO -- hacer que la pregunta varíe si la respuesta es incorrecta
  \ Para ello, pasar como parámetro el _xt_ que crea la cadena.

: surrender?$  ( -- ca len )
  s{
  s" ¿Quieres"
  s" ¿En serio quieres"
  s" ¿De verdad quieres"
  s" ¿Estás segur" player-gender-ending$+ s" de que quieres" txt+
  s" ¿Estás decidid" player-gender-ending$+ s" a" txt+
  }s s{
  s" dejarlo?"
  s" rendirte?"
  s" abandonar?"
  }s txt+  ;
  \ Devuelve la pregunta de si el jugador quiere dejar el juego.

: surrender?  ( -- f )
  ['] surrender?$ yes?  ;
  \ ¿Quiere el jugador dejar el juego?

: game-over?  ( -- f )  success? failure? or  ;
  \ ¿Se terminó ya el juego?

: the-favorite-says$  ( -- ca len )
  s" te" s{ s" indica" s" hace saber" }s txt+
  s" el valido" txt+  ;

: do-not-disturb$  ( -- ca len )
  s" ha" s{
  s" ordenado"
  s" dado órdenes de"
  s" dado" s" la" 50%nullify txt+ s" orden de" txt+
  }s txt+ s" que" txt+ s{ s" nadie" s" no se" }s txt+
  s" lo moleste" txt+ comma+  ;

: favorite's-speech$  ( -- ca len )
  s" El rey"  castilian-quotes? @
  if    rquote$ s+ comma+ the-favorite-says$ txt+ comma+
        lquote$ do-not-disturb$ s+ txt+
  else  dash$ the-favorite-says$ s+ dash$ s+ comma+ txt+
        do-not-disturb$ txt+
  then  s" pues sufre una amarga tristeza." txt+  ;

: the-happy-end  ( -- )
  s" Agotado, das parte en el castillo de tu llegada"
  s" y de lo que ha pasado." txt+
  narrate  narration-break
  s" Pides audiencia al rey, Uther Pendragon."
  narrate  scene-break
  favorite's-speech$ speak  narration-break
  s" No puedes entenderlo. El rey, tu amigo."
  narrate  narration-break
  s" Agotado, decepcionado, apesadumbrado,"
  s" decides ir a dormir a tu casa." txt+
  s" Es lo poco que puedes hacer." txt+
  narrate  narration-break
  s" Te has ganado un buen descanso."
  narrate  ;
  \ Final del juego con éxito.

: ransom$  ( -- ca len )
  s" un" s{ s" buen" s" suculento" }s txt+ s" rescate" txt+  ;

: my-lucky-day$  ( -- ca len )
  s{  s" Hoy" s{ s" es" s" parece ser" s" sin duda es" s" al parecer es" }s txt+
      s{  s" Sin duda" s" No cabe duda de que"
          s" Parece que" s" Por lo que parece"
      }s s" hoy es" txt+
  }s s{ s" un" s" mi" }s txt+ s" día" txt+
  s{ s" de suerte" s" afortunado" }s txt+ s" ..." s+  ;
  \ Texto de las palabras del general enemigo.

: enemy-speech$  ( -- ca len )
  my-lucky-day$
  s{ s" Bien, bien..." s" Excelente..." }s txt+
  s{  s" Por el gran Ulfius"
        s{  s" podremos" s{ s" pedir" s" negociar" s" exigir" }s txt+
            s" pediremos" s" exigiremos" s" nos darán" s" negociaremos"
        }s txt+ ransom$ txt+
      s" Del gran Ulfius" s{ s" podremos" s" lograremos" }s txt+
        s{ s" sacar" s" obtener" }s txt+
        s{ ransom$ s{ s" alguna" s" una" }s s" buena ventaja" txt+ }s txt+
  }s txt+  ;

: enemy-speech  ( -- )  enemy-speech$ period+  speak  ;
  \ Palabras del general enemigo.

: the-sad-end  ( -- )
  s" Los sajones"
  \ XXX TODO -- añadir también el siguiente escenario, el lago
  location-10~ am-i-there? if
    \ XXX TODO -- ampliar y variar
    comma+
    s" que te han visto entrar," txt+
    s" siguen tus pasos y" txt+
  then
  \ XXX TODO -- ampliar, explicar por qué no lo matan
  s" te" txt+
  s{ s" hacen prisionero" s" capturan" s" atrapan" }s txt+ period+
  s" Su general" txt+
  s" , que no tarda en" s{ s" llegar" s" aparecer" }s txt+ comma+ 50%nullify s+
  s" te" txt+ s{
    s" reconoce" s{ s" enseguida" s" de immediato" s" al instante" }s txt+
    s{ s" contempla" s" observa" s" mira" }s
      s" durante" 50%nullify txt+ s{ s" un momento" s" un instante" }s txt+
  }s txt+ s" y" txt+ comma+
  s{  s" sonriendo" s{ s" ampliamente" s" despectivamente" s" cruelmente" }s txt+
      s" con una" s{ s" amplia" s" cruel" s" despectiva" }s txt+ s" sonrisa" txt+
  }s txt+ comma+
  s{  s" exclama" s" dice" }s txt+ colon+ ^uppercase narrate
  narration-break enemy-speech  ;
  \ Final del juego con fracaso.

: the-end  ( -- )
  success? if  the-happy-end  else  the-sad-end  then  scene-break  ;
  \ Mensaje final del juego.


: retry?  ( -- f )  ['] retry?$ yes?  ;
  \ Pregunta al jugador si quiere volver a intentarlo y devuelve la
  \ respuesta en el indicador _f_.

defer adventure  ( -- )

: (finish)  ( -- )  retry? if  adventure  else  farewell  then  ;
  \ Acción de abandonar el juego, ya confirmada por el jugador.
  \ Pregunta al jugador si quiere volver a intentarlo; si es así,
  \ inicia una nueva partida; de otro modo sale del programa.

:noname  ( -- )  surrender? ?? (finish)  ; is finish
  \ Acción de abandonar el juego. Pide confirmación.

\ vim:filetype=gforth:fileencoding=utf-8

