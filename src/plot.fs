\ plot.fs
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

require galope/choose.fs                  \ `choose`
require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/plus-plus.fs               \ `++`
require galope/s-curly-bracket.fs         \ `s{`
require galope/question-execute.fs        \ `?execute`
require galope/question-question.fs       \ `??`
require galope/two-choose.fs              \ `2choose`
require galope/txt-plus.fs                \ `txt+`

set-current

\ ==============================================================
\ Inicialización

: init-plot  ( -- )
  ambrosio-follows? off
  battle# off
  climbed-the-fallen-away? off
  hacked-the-log? off
  stone-forbidden? off
  sword-forbidden? off
  recent-talks-to-the-leader off  ;
  \ Inicializa las variables de la trama.

\ ==============================================================
\ Tramas comunes a todos los escenarios

\ ----------------------------------------------
\ Ambrosio nos sigue

: does-ambrosio-follow?  ( -- f )
  ambrosio~ not-vanished?  key~ is-accessible? and
  location-46~ am-i-there?  ambrosio-follows? @ or  and  ;
  \ ¿Ambrosio nos sigue?
  \
  \ XXX TODO -- Confirmar la función de la llave aquí. En el código
  \ original solo se distingue que sea manipulable o no, lo que es
  \ diferente a que esté accesible.

: ambrosio-follows  ( -- )
  my-location ambrosio~ be-there
  s{ s" tu benefactor" ambrosio~ full-name }s ^uppercase
  s" te sigue, esperanzado." txt+ narrate  ;
  \ Ambrosio nos sigue.

: maybe-ambrosio-follows  ( -- )
  does-ambrosio-follow? ?? ambrosio-follows  ;
  \ Quizá Ambrosio nos sigue.

\ ----------------------------------------------
\ Lanzadores de las tramas comunes a todos los escenarios

\ XXX TODO -- convertir en diferidas y mover a Flibustre

: before-describing-any-location  ( -- )  ;
  \ Trama de entrada común a todos los entes escenario.
  \ XXX TODO -- no usado

: after-describing-any-location  ( -- )  ;
  \ Trama de entrada común a todos los entes escenario.
  \ XXX TODO -- no usado

: after-listing-entities-of-any-location  ( a -- )
  maybe-ambrosio-follows  ;
  \ Trama final de entrada común a todos los entes escenario.

: before-leaving-any-location  ( -- )  ;
  \ Trama de salida común a todos los entes escenario.
  \ XXX TODO -- no usado

\ ==============================================================
\ Herramientas para las tramas asociadas a escenarios

\ XXX TODO -- mover a Flibustre

: before-describing-location  ( a -- )
  before-describing-any-location
  before-description-plotter ?execute  ;
  \ Trama de entrada a un ente escenario.

: after-describing-location  ( a -- )
  after-describing-any-location
  after-description-plotter ?execute  ;
  \ Trama de entrada a un ente escenario.

: after-listing-entities  ( a -- )
  after-listing-entities-of-any-location
  before-prompt-plotter ?execute  ;
  \ Trama final de entrada a un ente escenario.

: before-leaving-location  ( a -- )
  before-leaving-any-location
  before-exit-plotter ?execute  ;
  \ Ejecuta la trama de salida de un ente escenario.

: leave-location  ( a -- )
  dup visits++
  dup before-leaving-location
  protagonist~ was-there  ;
  \ Tareas previas a abandonar un escenario.

: leave-my-location  ( -- )
  my-location ?dup if  leave-location  then  ;
  \ Tareas previas a abandonar el escenario actual.

: actually-enter-location  ( a -- )
  leave-my-location
  dup my-location!
  dup before-describing-location
  dup describe
  dup after-describing-location
  dup familiar++  .present
      after-listing-entities  ;
  \ Entra en un escenario.

: can-be-entered?  ( a -- f )
  enter-checker ?dup if  execute  else  true  then  ;
  \ Ejecuta la trama previa a la entrada a un ente escenario _a_, que
  \ devolverá un indicador _f_ de que puede entrarse en el escenario;
  \ si esta trama no está definida para el ente, el indicador será
  \ `true`.

: enter-location  ( a -- )
  dup can-be-entered? and ?dup ?? actually-enter-location  ;
  \ Entra en un escenario _a_, si es posible.

\ ==============================================================
\ Recursos de las tramas asociadas a lugares

\ ----------------------------------------------
\ Regreso a casa

: pass-still-open?  ( -- f )  location-08~ has-north-exit?  ;
  \ ¿El paso del desfiladero está abierto por el norte?

: still-in-the-village?  ( -- f )
  location-01~ am-i-there?
  location-02~ is-not-visited? and  ;
  \ ¿Los soldados no se han movido aún de la aldea sajona?

: back-to-the-village?  ( -- f )
  location-01~ am-i-there?  location-02~ is-visited? and  ;
  \ ¿Los soldados han regresado a la aldea sajona?
  \ XXX TODO -- no usado

: soldiers-follow-you  ( -- )
  ^all-your$ soldiers$ txt+
  s{ s" siguen tus pasos." s" te siguen." }s txt+ narrate  ;
  \ De vuelta a casa.

: going-home  ( -- )
  pass-still-open?  still-in-the-village? 0=  and
  ?? soldiers-follow-you  ;
  \ De vuelta a casa, si procede.

: celebrating  ( -- )
  ^all-your$ soldiers$ txt+
  s{ s" lo están celebrando." s" lo celebran." }s txt+ narrate  ;
  \ Celebrando la victoria.
  \ XXX TODO -- inconcluso

\ ----------------------------------------------
\ Persecución

: pursued  ( -- )
  s{
  s" El tiempo apremia"
  s" Hay que apresurarse"
  s" No hay mucho tiempo"
  s" No hay tiempo que perder"
  s" No sabes cuánto tiempo te queda"
  s" No te queda mucho tiempo"
  s" No tienes mucho tiempo"
  s" No tienes tiempo que perder"
  s" Sabes que debes darte prisa"
  s" Sabes que no puedes perder tiempo"
  s" Te queda poco tiempo"
  s" Tienes que apresurarte"
  }s s" ..." s+  narrate  ;
  \ Perseguido por los sajones.

: pursue-location?  ( -- f )
  my-location location-12~ <  ;
  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?

: you-think-you're-safe$  ( -- ca len )
  s{  s" crees estar"
      s" te sientes"
      s" crees sentirte"
      s" tienes la sensación de estar"
  }s s{ s" a salvo" s" seguro" }s txt+  ;

: but-it's-an-impression$  ( -- ca len )
  s" ," but$ txt+
  s{ s" dentro de ti" s" en tu interior" s" en el fondo" }s 50%nullify txt+
  s{
    s{  s" sabes" s" bien" 50%nullify txt+ s" eres consciente de" }s s" que" txt+
    s{  s" tu instinto" s{ s" militar" s" guerrero" s" de soldado" }s 50%nullify txt+
        s" una voz"
        s" algo"
    }s s{ s" no te engaña"
          s" te dice que" s{  s" no debes confiarte"
                              still$ s" no lo has logrado" txt+ }s txt+
    }s txt+ s{ s" ;" s" :" s" ..." }s s+
  }s txt+ s{
    only$ s" es una falsa impresión" txt+
    still$  s{  s" no lo has" s{ s" logrado" s" conseguido" }s txt+
                s" podrían" s{ s" encontrarte" s" atraparte" }s txt+ s" aquí" 50%nullify txt+
            }s txt+
    s" puede que te hayan" s{ s" visto" s" entrar" 50%nullify txt+
                              s" seguido" }s txt+
    s{ s" probablemente" s" seguramente" }s
      s" te" txt+ s{  s" estarán buscando"
                    s" habrán seguido"
                    s" hayan visto" s" entrar" 50%nullify txt+ }s txt+
  }s txt+  ;

\ ----------------------------------------------
\ Batalla

: all-your-men  ( -- ca len f )
  2 random dup
  if  s{ s" Todos" s" Todos y cada uno de" }s
  else  s" Hasta el último de"
  then  your-soldiers$ txt+  rot  ;
  \ Devuelve en una cadena _ca len_ una variante de «Todos tus
  \ hombres», y un indicador _f_ de número (cierto= el texto está en
  \ plural; falso=el texto está en singular).

: ?plural-verb  ( ca1 len1 f -- ca1 len1 | ca2 len2 )
  if  s" n" s+  then  ;
  \ Pone un verbo en plural si es preciso.

: fight/s$  ( f -- ca len )
  s{ s" lucha" s" combate" s" pelea" s" se bate" }s
  rot ?plural-verb  ;
  \ Devuelve una variante _ca len_ de «lucha/n», y un indicador _f_ de
  \ número (cierto: el texto está en plural; falso: el texto está en
  \ singular).

: resist/s$  ( f -- ca len )
  s{ s" resiste" s" aguanta" s" contiene" }s
  rot ?plural-verb  ;
  \ Devuelve una variante _ca len_ de «resiste/n», y un indicador _f_
  \ de número (cierto: el texto está en plural; falso: el texto está
  \ en singular).

: heroe$  ( -- ca len )
  s{ s" héroe" s" valiente" s" jabato" }s  ;
  \ Devuelve una variante de «héroe».

: heroes$  ( -- ca len )  heroe$ s" s" s+  ;
  \ Devuelve una variante de «héroes».

: like-a-heroe$ ( -- ca len )
  s" como un" s" auténtico" 50%nullify txt+ heroe$ txt+  ;
  \ Devuelve una variante de «como un héroe».

: like-heroes$ ( -- ca len )
  s" como" s" auténticos" 50%nullify txt+ heroes$ txt+  ;
  \ Devuelve una variante de «como héroes».

: (bravery)$  ( -- ca len )
  s{ s" con denuedo" s" con bravura" s" con coraje"
  s" heroicamente" s" esforzadamente" s" valientemente" }s  ;
  \ Devuelve una variante de «con denuedo».

: bravery$  ( f -- ca len )
  (bravery)$  rot
  if  like-heroes$  else  like-a-heroe$  then  2 2choose  ;
  \ Devuelve una variante _ca len_ de «con denuedo», en singular o
  \ plural, dependiendo del indicador _f_ (cierto: el resultado debe
  \ estar en plural; falso: el resultado debe estar en singular).

: step-by-step$  ( -- ca len )
  s{ s" por momentos" s" palmo a palmo" s" poco a poco" }s  ;
  \ Devuelve una variante de «poco a poco».

: field$  ( -- ca len )
  s{ s" terreno" s" posiciones" }s  ;
  \ Devuelve «terreno» o «posiciones».

: last(fp)$  ( -- ca len )
  s{ s" últimas" s" postreras" }s  ;
  \ Devuelve una variante de «últimas».

: last$  ( -- ca len )
  s{ s" último" s" postrer" }s  ;
  \ Devuelve una variante de «último».

: last-energy(fp)$  ( -- ca len )
  last(fp)$ s{ s" energías" s" fuerzas" }s txt+  ;
  \ Devuelve una variante de «últimas energías».

: battle-phase-00$  ( -- ca len )
  s" A pesar de" s{
  s" haber sido" s{ s" atacados por sorpresa" s" sorprendidos" }s txt+
  s" la sorpresa" s" inicial" 50%nullify txt+
  s" lo" s{ s" inesperado" s" sorpresivo" s" sorprendente" s" imprevisto" }s txt+
  s" del ataque" txt+ }s txt+ comma+ your-soldiers$ txt+
  s{ s" responden" s" reaccionan" }s txt+
  s{ s" con prontitud" s" sin perder un instante"
  s" rápidamente" s" como si fueran uno solo"
  }s txt+ s" y" txt+ s{
  s" adoptan una formación defensiva"
  s" organizan la defensa"
  s" se" s{ s" preparan" s" aprestan" }s txt+ s" para" txt+
  s{ s" defenderse" s" la defensa" }s txt+
  }s txt+ period+  ;
  \ Devuelve la descripción del combate (fase 00).

: battle-phase-00  ( -- )
  \ Combate (fase 00).
  battle-phase-00$ narrate  ;

: battle-phase-01$  ( -- ca len )
  all-your-men  dup resist/s$  rot bravery$  txt+ txt+
  s{  s{ s" el ataque" s" el empuje" s" la acometida" }s
      s" inicial" txt+
      s" el primer" s{ s" ataque" s" empuje" }s txt+
      s" la primera acometida"
  }s txt+ of-the-enemy|enemies$ txt+ period+  ;
  \ Devuelve la descripción del combate (fase 01).

: battle-phase-01  ( -- )  battle-phase-01$ narrate  ;
  \ Combate (fase 01).

: battle-phase-02$  ( -- ca len )
  all-your-men  dup fight/s$  rot bravery$  txt+ txt+
  s" contra" txt+  the-enemy|enemies$ txt+  period+  ;
  \ Devuelve la descripción del combate (fase 02).

: battle-phase-02  ( -- )  battle-phase-02$ narrate  ;
  \ Combate (fase 02).

: battle-phase-03$  ( -- ca len )
  ^your-soldiers$
  s" empiezan a acusar" txt+
  s{ null$ s" visiblemente" s" notoriamente" }s txt+
  s" el" txt+ s{ s" titánico" s" enorme" }s 50%nullify txt+
  s" esfuerzo." txt+  ;
  \ Devuelve la descripción del combate (fase 03).
  \ XXX TODO -- inconcluso

: battle-phase-03  ( -- )  battle-phase-03$ narrate  ;
  \ Combate (fase 03).

: battle-phase-04$  ( -- ca len )
  ^the-enemy|enemies
  s" parece que empieza* a" rot *>verb-ending txt+
  s{ s" dominar" s" controlar" }s txt+
  s{ s" el campo" s" el combate" s" la situación" s" el terreno" }s txt+
  period+  ;
  \ Devuelve la descripción del combate (fase 04).

: battle-phase-04  ( -- )  battle-phase-04$ narrate  ;
  \ Combate (fase 04).

: battle-phase-05$  ( -- ca len )
  ^the-enemy|enemies s{
  s" está* haciendo retroceder a" your-soldiers$ txt+
  s" está* obligando a" your-soldiers$ txt+ s" a retroceder" txt+
  }s rot *>verb-ending txt+
  step-by-step$ txt+ period+  ;
  \ Devuelve la descripción del combate (fase 05).
  \ XXX TODO -- inconcluso?

: battle-phase-05  ( -- )  battle-phase-05$ narrate  ;
  \ Combate (fase 05).

: battle-phase-06$  ( -- ca len )
  ^the-enemy|enemies s{
  s" va* ganando" field$ txt+
  s" va* adueñándose del terreno"
  s" va* conquistando" field$ txt+
  s" se va* abriendo paso"
  }s rot *>verb-ending txt+
  step-by-step$ txt+ period+  ;
  \ Devuelve la descripción del combate (fase 06).
  \ XXX TODO -- inconcluso

: battle-phase-06  ( -- )  battle-phase-06$ narrate  ;
  \ Combate (fase 06).

: battle-phase-07$  ( -- ca len )
  ^your-soldiers$
  s{ s" caen" s" van cayendo," }s txt+
  s" uno tras otro," 50%nullify txt+
  s{ s" vendiendo cara su vida" s" defendiéndose" }s txt+
  like-heroes$ txt+ period+  ;
  \ Devuelve la descripción del combate (fase 07).

: battle-phase-07  ( -- )  battle-phase-07$ narrate  ;
  \ Combate (fase 07).

: battle-phase-08$  ( -- ca len )
  ^the-enemy|enemies
  s{ s" aplasta* a" s" acaba* con" }s
  rot *>verb-ending txt+
  s" los últimos de" s" entre" 50%nullify txt+ txt+
  your-soldiers$ txt+ s" que," txt+
  s{  s" heridos" s{ s" extenuados" s" exhaustos" s" agotados" }s both?
      s{ s" apurando" s" con" }s s" sus" txt+ last-energy(fp)$ txt+
      s" con su" last$ txt+ s" aliento" txt+
      s" haciendo un" last$ txt+ s" esfuerzo" txt+
  }s txt+ comma+ still$ txt+
  s{  s" combaten" s" resisten"
      s{ s" se mantienen" s" aguantan" s" pueden mantenerse" }s
      s" en pie" txt+
      s{ s" ofrecen" s" pueden ofrecer" }s s" alguna" 50%nullify txt+
      s" resistencia" txt+
  }s txt+ period+  ;
  \ Devuelve la descripción del combate (fase 08).

: battle-phase-08  ( -- )  battle-phase-08$ narrate  ;
  \ Combate (fase 08).

create 'battle-phases  here
  \ Tabla para las fases del combate.

  ' battle-phase-00 ,
  ' battle-phase-01 ,
  ' battle-phase-02 ,
  ' battle-phase-03 ,
  ' battle-phase-04 ,
  ' battle-phase-05 ,
  ' battle-phase-06 ,
  ' battle-phase-07 ,
  ' battle-phase-08 ,

here swap - cell / constant battle-phases
  \ Fases de la batalla.

: (battle-phase)  ( n -- )
  cells 'battle-phases + perform  ;
  \ Ejecuta la fase _n_ del combate.

: battle-phase  ( -- )  battle# @ 1- (battle-phase)  ;
  \ Ejecuta la fase en curso del combate.

: battle-location?  ( -- f )
  my-location location-10~ <
  pass-still-open? 0=  and  ;
  \ ¿En el escenario de la batalla?

: battle-phase++  ( -- )  10 random if  battle# ++  then  ;
  \ Incrementar la fase de la batalla (salvo una de cada diez veces,
  \ al azar).

: battle  ( -- )
  battle-location? ?? battle-phase
  pursue-location? ?? pursued
  battle-phase++  ;
  \ Batalla y persecución.

: battle?  ( -- f )  battle# @ 0>  ;
  \ ¿Ha empezado la batalla?

: the-battle-ends  ( -- )  battle# off  ;
  \ Termina la batalla.

: the-battle-begins  ( -- )  1 battle# !  ;
  \ Comienza la batalla.

\ ----------------------------------------------
\ Emboscada de los sajones

: the-pass-is-closed  ( -- )  no-exit location-08~ ~north-exit !  ;
  \ Cerrar el paso, la salida norte.

: a-group-of-saxons$  ( -- ca len )
  s" una partida" s{ s" de sajones" s" sajona" }s txt+  ;

: suddenly$  ( -- ca len )  s" de" s{ s" repente" s" pronto" }s txt+  ;

: suddenly|then$  ( -- ca len )  s{ suddenly$ s" entonces" }s  ;

: the-ambush-begins  ( -- )
  s{  suddenly$ s" ," 50%nullify s+ a-group-of-saxons$ txt+ s" aparece" txt+
      a-group-of-saxons$  s" aparece" txt+ suddenly$ txt+
  }s ^uppercase s" por el este." txt+
  s" Para cuando" txt+
  s{ s" te vuelves" s" intentas volver" }s txt+
  toward-the(m)$ txt+ s" norte," txt+
  s" ya no" txt+ s{ s" te" 50%nullify s" queda" txt+ s" tienes" }s txt+
  s{ s" duda:" s" duda alguna:" s" ninguna duda:" }s txt+
  s{  s" es" s" se trata de"
      s{ s" te" s" os" }s s" han tendido" txt+
  }s txt+ s" una" txt+
  s{ s" emboscada" s" celada" s" encerrona" s" trampa" }s txt+
  period+  narrate narration-break  ;
  \ Comienza la emboscada.

: they-win-0$  ( -- ca len )
  s{  s" su" s{ s" victoria" s" triunfo" }s txt+
      s{ s" la" s" nuestra" }s s{ s" derrota" s" humillación" }s txt+
  }s s" será" txt+ s{ s" doble" s" mayor" }s txt+  ;
  \ Devuelve la primera versión de la parte final de las palabras de
  \ los oficiales.

: they-win-1$  ( -- ca len )
  s{  s" ganan" s" nos ganan" s" vencen" s" nos vencen"
      s" perdemos" s" nos derrotan" }s
  s{ s" doblemente" s" por partida doble" }s txt+  ;
  \ Devuelve la segunda versión de la parte final de las palabras de
  \ los oficiales.

: they-win$  ( -- ca len )
  they-win-0$ they-win-1$ 2 2choose period+  ;
  \ Devuelve la parte final de las palabras de los oficiales.

: taking-prisioner$  ( -- ca len )
  s" si" s{ s" capturan" s" hacen prisionero" s" toman prisionero" }s txt+  ;
  \ Devuelve una parte de las palabras de los oficiales.

: officers-speach  ( -- )
  sire,$ 50%nullify  dup taking-prisioner$
  rot 0= ?? ^uppercase txt+
  s" a un general britano" txt+ they-win$ txt+  speak  ;
  \ Palabras de los oficiales.

: officers-talk-to-you  ( -- )
  s" Tus oficiales te"
  s{ s" conminan a huir"
  s" conminan a ponerte a salvo"
  s" piden que te pongas a salvo"
  s" piden que huyas" }s txt+ colon+ narrate
  officers-speach
  s{ s" Sabes" s" Comprendes" }s s" que" txt+
  s{  s" es cierto" s{ s" tienen" s" llevan" }s s" razón" txt+
      s" están en lo cierto" }s txt+ comma+
  s{  but$ s{ s" a pesar de ello" s" aun así" }s 50%nullify txt+
      s" y"
  }s txt+ s" te duele" txt+ period+ narrate  ;
  \ Los oficiales hablan con el protagonista.

: the-enemy-is-stronger$  ( -- ca len )
  s" En el" narrow(m)$ txt+ s" paso es posible" txt+
  s{ s" resistir," s" defenderse," }s txt+ but$ txt+
  s{ s" por desgracia" s" desgraciadamente" }s txt+
  s{
    s{ s" los sajones" s" ellos" }s s" son" txt+
      s{  s" muy" 50%nullify s" superiores en número" txt+
          s" mucho" 50%nullify s" más numerosos" txt+
      }s txt+
    s" sus tropas son" s" mucho" 50%nullify txt+ s" más numerosas que las tuyas" txt+
    s" sus" s{ s" hombres" s" soldados" }s txt+
      s" son" txt+ s" mucho" 50%nullify txt+ s" más numerosos que los tuyos" txt+
  }s txt+  ;
  \ Mensaje de que el enemigo es superior.

: the-enemy-is-stronger  ( -- )
  the-enemy-is-stronger$ period+ narrate scene-break  ;
  \ El enemigo es superior.

: ambush  ( -- )
  the-pass-is-closed
  the-ambush-begins
  the-battle-begins
  the-enemy-is-stronger
  officers-talk-to-you  ;
  \ Emboscada.

\ ----------------------------------------------
\ Oscuridad en la cueva

: considering-the-darkness$  ( -- ca len )
  s" Ante" s" el muro de" 50%nullify txt+ s" la reinante" txt+
  s{ s" e intimidante" s" e impenetrable" s" e infranqueable" s" y sobrecogedora" }s txt+
  s" oscuridad," txt+  ;

: you-go-back$  ( -- ca len )
  s{  s{  s" prefieres" s" decides" s" eliges" s" optas por"
          s{ s" no te queda" s" no tienes" }s
            s" otra" txt+ s{ s" opción" s" alternativa" }s txt+ s" que" txt+
          s" no puedes hacer"
            s{ s" sino" s" otra cosa que" s" más que" }s txt+
          s{ s" no te queda" s" no tienes" }s s{ s" otro" s" más" }s txt+
            s{ s" remedio" s" camino" }s txt+ s" que" txt+
      }s s{ s" volver atrás" s" retroceder" }s txt+
      s{ s" vuelves atrás" s" retrocedes" }s s" sin remedio" 50%nullify txt+
  }s s{ null$ s" unos pasos" s" sobre tus pasos" }s txt+  ;

: to-the-place-where$  ( -- a u)
  s" hasta" s" el lugar" 50%nullify dup if  s" de la cueva" 50%nullify txt+  then txt+
  s" donde" txt+  ;

: to-see-something$  ( -- ca len )
  s" ver" s" algo" 50%nullify txt+  ;

: there-is-some-light$  ( -- ca len )
  still$ 50%nullify s{ s" hay" s" llega" s" entra" s" penetra" s" se filtra" }s txt+
  s{  s" un mínimo de" s" una mínima" s" cierta"
      s" algo de" s" suficiente" s" bastante"
  }s txt+ s{ s" luz" s" claridad" s" luminosidad" }s txt+
  that-(at-least)$ s" permite" txt+ to-see-something$ txt+ 50%nullify txt+  ;

: sun-adjectives$  ( -- ca len )
  \ s" tímido" s" débil" s" triste" s" lejano"
  null$  ;
  \ XXX TODO -- hacer que seleccione uno o dos adjetivos

: there-are-some-sun-rays$  ( -- ca len )
  still$ 50%nullify s{ s" llegan" s" entran" s" penetran" s" se filtran" }s txt+
  s" alg" 50%nullify s" unos" s+ s" pocos" 50%nullify txt+ txt+
  s" rayos de" txt+ s{ s" luz" sun-adjectives$ s" sol" txt+ }s txt+
  that-(at-least)$ s" permiten" txt+ to-see-something$ txt+ 50%nullify txt+  ;

: it's-possible-to-see$  ( -- ca len )
  s{ s" se puede" s" puedes" s" es posible" }s s" ver" txt+ s" algo" 50%nullify txt+  ;

: dark-cave  ( -- )
  new-page
  considering-the-darkness$ you-go-back$ txt+ to-the-place-where$ txt+
  s{  there-is-some-light$
      there-are-some-sun-rays$
      it's-possible-to-see$
  }s txt+ period+ narrate  ;
  \ En la cueva y sin luz.

\ ----------------------------------------------
\ Albergue de los refugiados

: the-old-man-is-angry?  ( -- f )
  stone~ is-accessible?
  sword~ is-accessible?  or  ;
  \ ¿El anciano se enfada porque llevas algo prohibido?

: he-looks-at-you-with-anger$  ( -- ca len )
  s" parece sorprendido y" 50%nullify
  s{
  s" te mira" s{ s" con dureza" s" con preocupación" }s txt+
  s" te dirige una dura mirada"
  s" dirige su mirada hacia ti"
  }s txt+  ;
  \ Texto de que el líder de los refugiados te mira.

: he-looks-at-you-with-calm$  ( -- ca len )
  s" advierte tu presencia y" 50%nullify
  s{ s" por un momento" s" durante unos instantes" }s 50%nullify txt+
  s" te" txt+ s{ s" observa" s" contempla" }s txt+
  s{ s" con serenidad" s" con expresión serena" s" en calma" s" sereno" }s txt+  ;
  \ Texto de que el líder de los refugiados te mira.

: the-leader-looks-at-you$  ( -- ca len )
  leader~ ^full-name  the-old-man-is-angry?
  if    he-looks-at-you-with-anger$
  else  he-looks-at-you-with-calm$
  then  txt+ period+  ;
  \ Texto de que el líder de los refugiados te mira.

: the-refugees-surround-you$  ( -- ca len )
  ^the-refugees$
  location-28~ has-east-exit?
  if  they-let-you-pass$
  else  they-don't-let-you-pass$
  then  period+ txt+  ;
  \ Descripción de la actitud de los refugiados.

\ ==============================================================
\ Tramas asociadas a lugares

: soldiers-are-here  ( -- )  soldiers~ be-here going-home  ;

: lake-is-here  ( -- )  lake~ be-here  ;

: door-is-here  ( -- )  door~ be-here  ;

\ ==============================================================
\ Trama global

\ ----------------------------------------------
\ Varios

: lock-found  ( -- )
  door~ location lock~ be-there
  lock~ familiar++  ;
  \ ;  ' (lock-found) is lock-found  \ XXX OLD
  \ Encontrar el candado (al mirar la puerta o al intentar abrirla).

\ ----------------------------------------------
\ Gestor de la trama global

: plot  ( -- )  battle? if  battle exit  then  ;
  \ Trama global.  Nota: Las subtramas deben comprobarse en orden
  \ cronológico.

\ ==============================================================
\ Descripciones especiales

\ Esta sección contiene palabras que calculan o muestran descripciones
\ que necesitan un tratamiento especial porque hacen uso de palabras
\ relacionadas con la trama.

: officers-forbid-to-steal$  ( -- )
  s{ s" los" s" tus" }s s" oficiales" txt+
  s{
  s" intentan detener" s" detienen como pueden"
  s" hacen" s{ s" todo" 50%nullify s" lo que pueden" txt+ s" lo imposible" }s txt+
    s{ s" para" s" por" }s txt+ s" detener" txt+
  }s txt+ s{ s" el saqueo" 2dup s" el pillaje" }s txt+  ;
  \ Devuelve una variante de «Tus oficiales detienen el saqueo».

: ^officers-forbid-to-steal$  ( -- ca len )
  officers-forbid-to-steal$ ^uppercase  ;
  \ Devuelve una variante de «Tus oficiales detienen el saqueo» (con
  \ la primera mayúscula).

: (they-do-it)-their-way$  ( -- ca len )
  s" ," s{
    s" a su" s{ s" manera" s" estilo" }s txt+
    s" de la única" way$ txt+
    s" que" txt+ s{ s" saben" s" conocen" }s txt+
  }s txt+ comma+  ;

: this-sad-victory$  ( -- ca len )
  s" esta" s" tan" s{ s" triste" s" fácil" s" poco honrosa" }s txt+
  s" victoria" rnd2swap txt+ txt+  ;

: (soldiers-steal$)  ( ca1 len1 -- ca2 len2 )
  soldiers$ txt+ s{ s" aún" s" todavía" }s 50%nullify txt+
  s{ s" celebran" s{ s" están" s" siguen" s" continúan" }s s" celebrando" txt+ }s txt+
  (they-do-it)-their-way$ 50%nullify s+
  this-sad-victory$ txt+ s{ s" :" s" ..." }s s+
  s{ s" saqueando" s" buscando" s" apropiándose de" s" robando" }s txt+
  s" todo" 50%nullify txt+ s" cuanto de valor" txt+
  s" aún" 50%nullify txt+ s{ s" quede" s" pueda quedar" }s txt+
  s" entre" txt+ rests-of-the-village$ txt+  ;
  \ Completa una descripción de tus soldados en la aldea arrasada.

: soldiers-steal$  ( -- ca len )  all-your$ (soldiers-steal$)  ;
  \ Devuelve una descripción de tus soldados en la aldea arrasada.

: ^soldiers-steal$  ( -- ca len )  ^all-your$ (soldiers-steal$)  ;
  \ Devuelve una descripción de tus soldados en la aldea arrasada (con
  \ la primera mayúscula).

: soldiers-steal-spite-of-officers-0$  ( -- ca len )
  ^soldiers-steal$ period+
  ^officers-forbid-to-steal$ txt+  ;
  \ Devuelve la primera versión de la descripción de los soldados en
  \ la aldea.

: soldiers-steal-spite-of-officers-1$  ( -- ca len )
  ^soldiers-steal$
  s{ s" , mientras" s" que" 50%nullify txt+
  s{ s" ; mientras" s" . Mientras" }s s" tanto" 50%nullify txt+ comma+
  s" . Al mismo tiempo," }s s+
  officers-forbid-to-steal$ txt+  ;
  \ Devuelve la segunda versión de la descripción de los soldados en
  \ la aldea.

: soldiers-steal-spite-of-officers-2$  ( -- ca len )
  ^officers-forbid-to-steal$
  s" , pero" s+ s" a pesar de ello" 50%nullify txt+
  soldiers-steal$ txt+  ;
  \ Devuelve la tercera versión de la descripción de los soldados en
  \ la aldea.
  \ XXX TODO -- no se usa: la frase queda incoherente en algunos casos

: soldiers-steal-spite-of-officers$  ( -- ca len )
  ['] soldiers-steal-spite-of-officers-0$
  ['] soldiers-steal-spite-of-officers-1$  2 choose execute  ;
  \ Devuelve una descripción de tus soldados en la aldea arrasada.

: soldiers-steal-spite-of-officers  ( -- )
  soldiers-steal-spite-of-officers$ period+ paragraph  ;
  \ Describe a tus soldados en la aldea arrasada.

: will-follow-you-forever$  ( -- ca len )
  s" te seguirían hasta el"
  s{ s{ s" mismo" s" mismísimo" }s s" infierno" txt+
  s" último rincón de la Tierra"
  }s txt+  ;
  \ Devuelve la descripción de tus hombres durante el regreso a casa,
  \ sin citarlos.

: will-follow-you-forever  ( ca len -- )
  will-follow-you-forever$ txt+ period+ paragraph  ;
  \ Completa e imprime la descripción de soldados u oficiales, cuyo
  \ sujeto es _ca len_.

: soldiers-go-home  ( -- )
  ^all-your$ soldiers$ txt+ will-follow-you-forever  ;
  \ Describe a tus soldados durante el regreso a casa.

: officers-go-home  ( -- )
  ^all-your$ officers$ txt+
  s" , como"
  s{ s" el resto de tus" all-your$ }s txt+ soldiers$ txt+ comma+ 50%nullify s+
  will-follow-you-forever  ;
  \ Describe a tus soldados durante el regreso a casa.

\ vim:filetype=gforth:fileencoding=utf-8

