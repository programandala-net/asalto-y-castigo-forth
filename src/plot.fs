\ plot.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607111405

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/choose.fs              \ `choose`
require galope/plus-plus.fs           \ `++`
require galope/random_strings.fs
require galope/question-execute.fs    \ `?execute`
require galope/question-question.fs   \ `??`
require galope/two-choose.fs          \ `2choose`

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
  s" te sigue, esperanzado." s& narrate  ;
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
  ^all-your$ soldiers$ s&
  s{ s" siguen tus pasos." s" te siguen." }s& narrate  ;
  \ De vuelta a casa.

: going-home  ( -- )
  pass-still-open?  still-in-the-village? 0=  and
  ?? soldiers-follow-you  ;
  \ De vuelta a casa, si procede.

: celebrating  ( -- )
  ^all-your$ soldiers$ s&
  s{ s" lo están celebrando." s" lo celebran." }s& narrate  ;
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
  }s{ s" a salvo" s" seguro" }s&  ;

: but-it's-an-impression$  ( -- ca len )
  s" ," but$ s&
  s{ s" dentro de ti" s" en tu interior" s" en el fondo" }s?&
  s{
    s{  s" sabes" s" bien" s?& s" eres consciente de" }s s" que" s&
    s{  s" tu instinto" s{ s" militar" s" guerrero" s" de soldado" }s?&
        s" una voz"
        s" algo"
    }s s{ s" no te engaña"
          s" te dice que" s{  s" no debes confiarte"
                              still$ s" no lo has logrado" s& }s&
    }s& s{ s" ;" s" :" s" ..." }s+
  }s& s{
    only$ s" es una falsa impresión" s&
    still$  s{  s" no lo has" s{ s" logrado" s" conseguido" }s&
                s" podrían" s{ s" encontrarte" s" atraparte" }s& s" aquí" s?&
            }s&
    s" puede que te hayan" s{ s" visto" s" entrar" s?&
                              s" seguido" }s&
    s{ s" probablemente" s" seguramente" }s
      s" te" s& s{  s" estarán buscando"
                    s" habrán seguido"
                    s" hayan visto" s" entrar" s?& }s&
  }s&  ;

\ ----------------------------------------------
\ Batalla

: all-your-men  ( -- ca len f )
  2 random dup
  if  s{ s" Todos" s" Todos y cada uno de" }s
  else  s" Hasta el último de"
  then  your-soldiers$ s&  rot  ;
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
  s" como un" s" auténtico" s?& heroe$ s&  ;
  \ Devuelve una variante de «como un héroe».

: like-heroes$ ( -- ca len )
  s" como" s" auténticos" s?& heroes$ s&  ;
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
  last(fp)$ s{ s" energías" s" fuerzas" }s&  ;
  \ Devuelve una variante de «últimas energías».

: battle-phase-00$  ( -- ca len )
  s" A pesar de" s{
  s" haber sido" s{ s" atacados por sorpresa" s" sorprendidos" }s&
  s" la sorpresa" s" inicial" s?&
  s" lo" s{ s" inesperado" s" sorpresivo" s" sorprendente" s" imprevisto" }s&
  s" del ataque" s& }s& comma+ your-soldiers$ s&
  s{ s" responden" s" reaccionan" }s&
  s{ s" con prontitud" s" sin perder un instante"
  s" rápidamente" s" como si fueran uno solo"
  }s& s" y" s&{
  s" adoptan una formación defensiva"
  s" organizan la defensa"
  s" se" s{ s" preparan" s" aprestan" }s& s" para" s&
  s{ s" defenderse" s" la defensa" }s&
  }s& period+  ;
  \ Devuelve la descripción del combate (fase 00).

: battle-phase-00  ( -- )
  \ Combate (fase 00).
  battle-phase-00$ narrate  ;

: battle-phase-01$  ( -- ca len )
  all-your-men  dup resist/s$  rot bravery$  s& s&
  s{  s{ s" el ataque" s" el empuje" s" la acometida" }s
      s" inicial" s&
      s" el primer" s{ s" ataque" s" empuje" }s&
      s" la primera acometida"
  }s& of-the-enemy|enemies$ s& period+  ;
  \ Devuelve la descripción del combate (fase 01).

: battle-phase-01  ( -- )  battle-phase-01$ narrate  ;
  \ Combate (fase 01).

: battle-phase-02$  ( -- ca len )
  all-your-men  dup fight/s$  rot bravery$  s& s&
  s" contra" s&  the-enemy|enemies$ s&  period+  ;
  \ Devuelve la descripción del combate (fase 02).

: battle-phase-02  ( -- )  battle-phase-02$ narrate  ;
  \ Combate (fase 02).

: battle-phase-03$  ( -- ca len )
  ^your-soldiers$
  s" empiezan a acusar" s&
  s{ null$ s" visiblemente" s" notoriamente" }s&
  s" el" s&{ s" titánico" s" enorme" }s?&
  s" esfuerzo." s&  ;
  \ Devuelve la descripción del combate (fase 03).
  \ XXX TODO -- inconcluso

: battle-phase-03  ( -- )  battle-phase-03$ narrate  ;
  \ Combate (fase 03).

: battle-phase-04$  ( -- ca len )
  ^the-enemy|enemies
  s" parece que empieza* a" rot *>verb-ending s&
  s{ s" dominar" s" controlar" }s&
  s{ s" el campo" s" el combate" s" la situación" s" el terreno" }s&
  period+  ;
  \ Devuelve la descripción del combate (fase 04).

: battle-phase-04  ( -- )  battle-phase-04$ narrate  ;
  \ Combate (fase 04).

: battle-phase-05$  ( -- ca len )
  ^the-enemy|enemies s{
  s" está* haciendo retroceder a" your-soldiers$ s&
  s" está* obligando a" your-soldiers$ s& s" a retroceder" s&
  }s rot *>verb-ending s&
  step-by-step$ s& period+  ;
  \ Devuelve la descripción del combate (fase 05).
  \ XXX TODO -- inconcluso?

: battle-phase-05  ( -- )  battle-phase-05$ narrate  ;
  \ Combate (fase 05).

: battle-phase-06$  ( -- ca len )
  ^the-enemy|enemies s{
  s" va* ganando" field$ s&
  s" va* adueñándose del terreno"
  s" va* conquistando" field$ s&
  s" se va* abriendo paso"
  }s rot *>verb-ending s&
  step-by-step$ s& period+  ;
  \ Devuelve la descripción del combate (fase 06).
  \ XXX TODO -- inconcluso

: battle-phase-06  ( -- )  battle-phase-06$ narrate  ;
  \ Combate (fase 06).

: battle-phase-07$  ( -- ca len )
  ^your-soldiers$
  s{ s" caen" s" van cayendo," }s&
  s" uno tras otro," s?&
  s{ s" vendiendo cara su vida" s" defendiéndose" }s&
  like-heroes$ s& period+  ;
  \ Devuelve la descripción del combate (fase 07).

: battle-phase-07  ( -- )  battle-phase-07$ narrate  ;
  \ Combate (fase 07).

: battle-phase-08$  ( -- ca len )
  ^the-enemy|enemies
  s{ s" aplasta* a" s" acaba* con" }s
  rot *>verb-ending s&
  s" los últimos de" s" entre" s?& s&
  your-soldiers$ s& s" que," s&
  s{  s" heridos" s{ s" extenuados" s" exhaustos" s" agotados" }s both?
      s{ s" apurando" s" con" }s s" sus" s& last-energy(fp)$ s&
      s" con su" last$ s& s" aliento" s&
      s" haciendo un" last$ s& s" esfuerzo" s&
  }s& comma+ still$ s&
  s{  s" combaten" s" resisten"
      s{ s" se mantienen" s" aguantan" s" pueden mantenerse" }s
      s" en pie" s&
      s{ s" ofrecen" s" pueden ofrecer" }s s" alguna" s?&
      s" resistencia" s&
  }s& period+  ;
  \ Devuelve la descripción del combate (fase 08).

: battle-phase-08  ( -- )  battle-phase-08$ narrate  ;
  \ Combate (fase 08).

create 'battle-phases  here
  \ Tabla para las fases del combate.
  \ Preservamos la dirección para calcular después el número de fases.

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

: (battle-phase)  ( u -- )
  cells 'battle-phases + perform  ;
  \ Ejecuta una fase _u_ del combate.

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
  s" una partida" s{ s" de sajones" s" sajona" }s&  ;

: suddenly$  ( -- ca len )  s" de" s{ s" repente" s" pronto" }s&  ;

: suddenly|then$  ( -- ca len )  s{ suddenly$ s" entonces" }s  ;

: the-ambush-begins  ( -- )
  s{  suddenly$ s" ," s?+ a-group-of-saxons$ s& s" aparece" s&
      a-group-of-saxons$  s" aparece" s& suddenly$ s&
  }s ^uppercase s" por el este." s&
  s" Para cuando" s&
  s{ s" te vuelves" s" intentas volver" }s&
  toward-the(m)$ s& s" norte," s&
  s" ya no" s& s{ s" te" s? s" queda" s& s" tienes" }s&
  s{ s" duda:" s" duda alguna:" s" ninguna duda:" }s&
  s{  s" es" s" se trata de"
      s{ s" te" s" os" }s s" han tendido" s&
  }s& s" una" s&
  s{ s" emboscada" s" celada" s" encerrona" s" trampa" }s&
  period+  narrate narration-break  ;
  \ Comienza la emboscada.

: they-win-0$  ( -- ca len )
  s{  s" su" s{ s" victoria" s" triunfo" }s&
      s{ s" la" s" nuestra" }s s{ s" derrota" s" humillación" }s&
  }s s" será" s&{ s" doble" s" mayor" }s&  ;
  \ Devuelve la primera versión de la parte final de las palabras de
  \ los oficiales.

: they-win-1$  ( -- ca len )
  s{  s" ganan" s" nos ganan" s" vencen" s" nos vencen"
      s" perdemos" s" nos derrotan" }s
  s{ s" doblemente" s" por partida doble" }s&  ;
  \ Devuelve la segunda versión de la parte final de las palabras de
  \ los oficiales.

: they-win$  ( -- ca len )
  they-win-0$ they-win-1$ 2 2choose period+  ;
  \ Devuelve la parte final de las palabras de los oficiales.

: taking-prisioner$  ( -- ca len )
  s" si" s{ s" capturan" s" hacen prisionero" s" toman prisionero" }s&  ;
  \ Devuelve una parte de las palabras de los oficiales.

: officers-speach  ( -- )
  sire,$ s?  dup taking-prisioner$
  rot 0= ?? ^uppercase s&
  s" a un general britano" s& they-win$ s&  speak  ;
  \ Palabras de los oficiales.

: officers-talk-to-you  ( -- )
  s" Tus oficiales te"
  s{ s" conminan a huir"
  s" conminan a ponerte a salvo"
  s" piden que te pongas a salvo"
  s" piden que huyas" }s& colon+ narrate
  officers-speach
  s{ s" Sabes" s" Comprendes" }s s" que" s&
  s{  s" es cierto" s{ s" tienen" s" llevan" }s s" razón" s&
      s" están en lo cierto" }s& comma+
  s{  but$ s{ s" a pesar de ello" s" aun así" }s?&
      s" y"
  }s& s" te duele" s& period+ narrate  ;
  \ Los oficiales hablan con el protagonista.

: the-enemy-is-stronger$  ( -- ca len )
  s" En el" narrow(m)$ s& s" paso es posible" s&
  s{ s" resistir," s" defenderse," }s& but$ s&
  s{ s" por desgracia" s" desgraciadamente" }s&
  s{
    s{ s" los sajones" s" ellos" }s s" son" s&
      s{  s" muy" s? s" superiores en número" s&
          s" mucho" s? s" más numerosos" s&
      }s&
    s" sus tropas son" s" mucho" s?& s" más numerosas que las tuyas" s&
    s" sus" s{ s" hombres" s" soldados" }s&
      s" son" s& s" mucho" s?& s" más numerosos que los tuyos" s&
  }s&  ;
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
  s" Ante" s" el muro de" s?& s" la reinante" s&
  s{ s" e intimidante" s" e impenetrable" s" e infranqueable" s" y sobrecogedora" }s&
  s" oscuridad," s&  ;

: you-go-back$  ( -- ca len )
  s{  s{  s" prefieres" s" decides" s" eliges" s" optas por"
          s{ s" no te queda" s" no tienes" }s
            s" otra" s&{ s" opción" s" alternativa" }s& s" que" s&
          s" no puedes hacer"
            s{ s" sino" s" otra cosa que" s" más que" }s&
          s{ s" no te queda" s" no tienes" }s s{ s" otro" s" más" }s&
            s{ s" remedio" s" camino" }s& s" que" s&
      }s{ s" volver atrás" s" retroceder" }s&
      s{ s" vuelves atrás" s" retrocedes" }s s" sin remedio" s?&
  }s{ null$ s" unos pasos" s" sobre tus pasos" }s&  ;

: to-the-place-where$  ( -- a u)
  s" hasta" s" el lugar" s? dup if  s" de la cueva" s?&  then s&
  s" donde" s&  ;

: to-see-something$  ( -- ca len )
  s" ver" s" algo" s?&  ;

: there-is-some-light$  ( -- ca len )
  still$ s? s{ s" hay" s" llega" s" entra" s" penetra" s" se filtra" }s&
  s{  s" un mínimo de" s" una mínima" s" cierta"
      s" algo de" s" suficiente" s" bastante"
  }s& s{ s" luz" s" claridad" s" luminosidad" }s&
  that-(at-least)$ s" permite" s& to-see-something$ s& s?&  ;

: sun-adjectives$  ( -- ca len )
  \ s" tímido" s" débil" s" triste" s" lejano"
  null$  ;
  \ XXX TODO -- hacer que seleccione uno o dos adjetivos

: there-are-some-sun-rays$  ( -- ca len )
  still$ s? s{ s" llegan" s" entran" s" penetran" s" se filtran" }s&
  s" alg" s? s" unos" s+ s" pocos" s?& s&
  s" rayos de" s&{ s" luz" sun-adjectives$ s" sol" s& }s&
  that-(at-least)$ s" permiten" s& to-see-something$ s& s?&  ;

: it's-possible-to-see$  ( -- ca len )
  s{ s" se puede" s" puedes" s" es posible" }s s" ver" s& s" algo" s?&  ;

: dark-cave  ( -- )
  new-page
  considering-the-darkness$ you-go-back$ s& to-the-place-where$ s&
  s{  there-is-some-light$
      there-are-some-sun-rays$
      it's-possible-to-see$
  }s& period+ narrate  ;
  \ En la cueva y sin luz.

\ ----------------------------------------------
\ Albergue de los refugiados

: the-old-man-is-angry?  ( -- f )
  stone~ is-accessible?
  sword~ is-accessible?  or  ;
  \ ¿El anciano se enfada porque llevas algo prohibido?

: he-looks-at-you-with-anger$  ( -- ca len )
  s" parece sorprendido y" s?
  s{
  s" te mira" s{ s" con dureza" s" con preocupación" }s&
  s" te dirige una dura mirada"
  s" dirige su mirada hacia ti"
  }s&  ;
  \ Texto de que el líder de los refugiados te mira.

: he-looks-at-you-with-calm$  ( -- ca len )
  s" advierte tu presencia y" s?
  s{ s" por un momento" s" durante unos instantes" }s?&
  s" te" s&{ s" observa" s" contempla" }s&
  s{ s" con serenidad" s" con expresión serena" s" en calma" s" sereno" }s&  ;
  \ Texto de que el líder de los refugiados te mira.

: the-leader-looks-at-you$  ( -- ca len )
  leader~ ^full-name  the-old-man-is-angry?
  if    he-looks-at-you-with-anger$
  else  he-looks-at-you-with-calm$
  then  s& period+  ;
  \ Texto de que el líder de los refugiados te mira.

: the-refugees-surround-you$  ( -- ca len )
  ^the-refugees$
  location-28~ has-east-exit?
  if  they-let-you-pass$
  else  they-don't-let-you-pass$
  then  period+ s&  ;
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

  \ XXX TODO -- la trama de la batalla sería adecuada para una trama
  \ global de escenario, invocada desde aquí. Aquí quedarían solo las
  \ tramas generales que no dependen de ningún escenario.

\ ==============================================================
\ Descripciones especiales

\ Esta sección contiene palabras que calculan o muestran descripciones
\ que necesitan un tratamiento especial porque hacen uso de palabras
\ relacionadas con la trama.

: officers-forbid-to-steal$  ( -- )
  s{ s" los" s" tus" }s s" oficiales" s&
  s{
  s" intentan detener" s" detienen como pueden"
  s" hacen" s{ s" todo" s? s" lo que pueden" s& s" lo imposible" }s&
    s{ s" para" s" por" }s& s" detener" s&
  }s& s{ s" el saqueo" 2dup s" el pillaje" }s&  ;
  \ Devuelve una variante de «Tus oficiales detienen el saqueo».

: ^officers-forbid-to-steal$  ( -- ca len )
  officers-forbid-to-steal$ ^uppercase  ;
  \ Devuelve una variante de «Tus oficiales detienen el saqueo» (con
  \ la primera mayúscula).

: (they-do-it)-their-way$  ( -- ca len )
  s" ," s{
    s" a su" s{ s" manera" s" estilo" }s&
    s" de la única" way$ s&
    s" que" s& s{ s" saben" s" conocen" }s&
  }s& comma+  ;

: this-sad-victory$  ( -- ca len )
  s" esta" s" tan" s{ s" triste" s" fácil" s" poco honrosa" }s&
  s" victoria" rnd2swap s& s&  ;

: (soldiers-steal$)  ( ca1 len1 -- ca2 len2 )
  soldiers$ s& s{ s" aún" s" todavía" }s?&
  s{ s" celebran" s{ s" están" s" siguen" s" continúan" }s s" celebrando" s& }s&
  (they-do-it)-their-way$ s?+
  this-sad-victory$ s& s{ s" :" s" ..." }s+
  s{ s" saqueando" s" buscando" s" apropiándose de" s" robando" }s&
  s" todo" s?& s" cuanto de valor" s&
  s" aún" s?& s{ s" quede" s" pueda quedar" }s&
  s" entre" s& rests-of-the-village$ s&  ;
  \ Completa una descripción de tus soldados en la aldea arrasada.

: soldiers-steal$  ( -- ca len )  all-your$ (soldiers-steal$)  ;
  \ Devuelve una descripción de tus soldados en la aldea arrasada.

: ^soldiers-steal$  ( -- ca len )  ^all-your$ (soldiers-steal$)  ;
  \ Devuelve una descripción de tus soldados en la aldea arrasada (con
  \ la primera mayúscula).

: soldiers-steal-spite-of-officers-0$  ( -- ca len )
  ^soldiers-steal$ period+
  ^officers-forbid-to-steal$ s&  ;
  \ Devuelve la primera versión de la descripción de los soldados en
  \ la aldea.

: soldiers-steal-spite-of-officers-1$  ( -- ca len )
  ^soldiers-steal$
  s{ s" , mientras" s" que" s?&
  s{ s" ; mientras" s" . Mientras" }s s" tanto" s?& comma+
  s" . Al mismo tiempo," }s+
  officers-forbid-to-steal$ s&  ;
  \ Devuelve la segunda versión de la descripción de los soldados en
  \ la aldea.

: soldiers-steal-spite-of-officers-2$  ( -- ca len )
  ^officers-forbid-to-steal$
  s" , pero" s+ s" a pesar de ello" s?&
  soldiers-steal$ s&  ;
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
  s{ s{ s" mismo" s" mismísimo" }s s" infierno" s&
  s" último rincón de la Tierra"
  }s&  ;
  \ Devuelve la descripción de tus hombres durante el regreso a casa,
  \ sin citarlos.

: will-follow-you-forever  ( ca len -- )
  will-follow-you-forever$ s& period+ paragraph  ;
  \ Completa e imprime la descripción de soldados u oficiales, cuyo
  \ sujeto es _ca len_.

: soldiers-go-home  ( -- )
  ^all-your$ soldiers$ s& will-follow-you-forever  ;
  \ Describe a tus soldados durante el regreso a casa.

: officers-go-home  ( -- )
  ^all-your$ officers$ s&
  s" , como"
  s{ s" el resto de tus" all-your$ }s& soldiers$ s& comma+ s?+
  will-follow-you-forever  ;
  \ Describe a tus soldados durante el regreso a casa.

\ vim:filetype=gforth:fileencoding=utf-8

