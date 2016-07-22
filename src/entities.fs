\ entities.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607221417

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/question-keep.fs           \ `?keep`
require galope/question-question.fs       \ `??`
require galope/s-curly-bracket.fs         \ `s{`
require galope/txt-plus.fs                \ `txt+`

set-current

\ ==============================================================
\ Atributos y descripciones de entes

\ ----------------------------------------------
\ Ente protagonista

: describe-ulfius  ( -- )
  s" Sientes sobre ti la carga de tanto"
  s{ s" sucedido" s" acontecido" s" acontecido" s" vivido" }s txt+
  period+ paragraph  ;

ulfius~ :init  ( -- )
  ['] describe-ulfius self~ be-describer
  s" Ulfius" self~ ms-name!
  self~ be-human
  self~ have-personal-name
  self~ have-no-article
  location-01~ self~ be-there  ;

\ ----------------------------------------------
\ Entes personaje

: describe-ambrosio  ( -- )
  ambrosio~ conversations
  if    s" Ambrosio"
        s" es un hombre de mediana edad, que te mira afable." txt+
  else  s" Es de mediana edad y mirada afable."
  then  paragraph  ;

ambrosio~ :init  ( -- )
  ['] describe-ambrosio self~ be-describer
  s" hombre" self~ ms-name!
  self~ be-character
  self~ be-human
  location-19~ self~ be-there  ;
  \ N.B. El nombre cambiará a «Ambrosio» durante el juego.

: describe-leader  ( -- )
  leader~ conversations?
  if    s" Es el jefe de los refugiados."
  else  s" Es un anciano."
  then  paragraph  ;
  \ XXX TODO -- elaborar esto según la trama

leader~ :init  ( -- )
  ['] describe-leader self~ be-describer
  s" anciano" self~ ms-name!
  self~ be-character
  self~ be-human
  self~ be-not-listed
  location-28~ self~ be-there  ;

: describe-soldiers  ( -- )
  true case
    still-in-the-village? of  soldiers-steal-spite-of-officers  endof
\   back-to-the-village? of  soldiers-go-home  endof
      \ XXX TODO -- no usado
    pass-still-open? of  soldiers-go-home  endof
\   battle? of  battle-phase  endof
    \ XXX TODO -- no usado. redundante, porque tras la descripción se
    \ mostrará otra vez la situación de la batalla
  endcase  ;
  \ Describe a tus soldados.

soldiers~ :init  ( -- )
  ['] describe-soldiers self~ be-describer
  s" soldados" self~ mp-name!
  self~ be-human
  self~ familiar++
  self~ be-decoration
  self~ belongs-to-protagonist  ;

: describe-officers  ( -- )
  true case
    still-in-the-village? of  ^officers-forbid-to-steal$  endof
\   back-to-the-village? of  officers-go-home  endof
    \ XXX TODO -- no usado
    pass-still-open? of  officers-go-home  endof
\   battle? of  battle-phase  endof
    \ XXX TODO -- no usado. redundante, porque tras la descripción se
    \ mostrará otra vez la situación de la batalla
  endcase  ;
  \ Describe a tus soldados.

officers~ :init  ( -- )
  ['] describe-officers self~ be-describer
  s" oficiales" self~ mp-name!
  self~ be-human
  self~ familiar++
  self~ be-decoration
  self~ belongs-to-protagonist  ;

: describe-present-refugees  ( -- )
  talked-to-the-leader?
  if    s" Los refugiados son"
  else  s" Hay"
  then  diverse-people$ txt+
  talked-to-the-leader?
  if    the-leader-said-they-want-peace$
  else  period+ you-don't-know-why-they're-here$
  then  txt+
  do-you-hold-something-forbidden?
  if    the-refugees-don't-trust$
  else  the-refugees-trust$
  then  txt+ period+ narrate  ;

: describe-refugees  ( -- )
  my-location case
  location-28~ of  describe-present-refugees  endof
  location-29~ of  s" Todos los refugiados quedaron atrás."
                   paragraph  endof
  endcase  ;
  \ XXX TODO -- ampliar el texto

refugees~ :init  ( -- )
  ['] describe-refugees self~ be-describer
  s" refugiados" self~ mp-name!
  self~ be-human
  self~ be-decoration  ;

\ ----------------------------------------------
\ Entes objeto

: describe-altar  ( -- )
  s" Está" s{ s" situado" s" colocado" }s txt+
  s" justo en la mitad del puente." txt+
  idol~ is-unknown?
  if  s" Debe de sostener algo importante." txt+   then
  paragraph  ;

altar~ :init  ( -- )
  ['] describe-altar self~ be-describer
  s" altar" self~ ms-name!
  self~ be-decoration
  location-18~ self~ be-there  ;

: describe-arch  ( -- )
  s" Un sólido arco de piedra, de una sola pieza."
  paragraph  ;
  \ XXX TODO -- mejorar texto

arch~ :init  ( -- )
  ['] describe-arch self~ be-describer
  s" arco" self~ ms-name!
  self~ be-decoration
  location-18~ self~ be-there  ;

: describe-bed  ( -- )
  s{ s" Parece poco" s" No tiene el aspecto de ser muy"
  s" No parece especialmente" }s
  s{ s" confortable" s" cómod" bed~ adjective-ending s+ }s txt+ period+
  paragraph  ;

bed~ :init  ( -- )
  ['] describe-bed self~ be-describer
  s" catre" self~ ms-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-bridge  ( -- )
  s" Está semipodrido."
  paragraph  ;
  \ XXX TODO -- mejorar texto

bridge~ :init  ( -- )
  ['] describe-bridge self~ be-describer
  s" puente" self~ ms-name!
  self~ be-decoration
  location-13~ self~ be-there  ;

: describe-candles  ( -- )
  s" Están muy consumidas."
  paragraph  ;

candles~ :init  ( -- )
  ['] describe-candles self~ be-describer
  s" velas" self~ fp-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-cave-entrance  ( -- )
  the-cave-entrance-is-hidden$
  you-were-lucky-discovering-it$ txt+
  it's-your-last-hope$ txt+
  paragraph  ;

cave-entrance~ :init  ( -- )
  ['] describe-cave-entrance self~ be-describer
  s" entrada a una cueva" self~ fs-name!  ;

: describe-cloak  ( -- )
  s" Tu capa de general, de fina lana"
  s{ s" tintada de negro." s" negra." }s txt+
  paragraph  ;

cloak~ :init  ( -- )
  ['] describe-cloak self~ be-describer
  s" capa" self~ fs-name!
  self~ be-wearable
  self~ belongs-to-protagonist
  self~ be-worn
  self~ taken  ;

cuirasse~ :init  ( -- )
  \ ['] describe-cuirasse self~ be-describer
  s" coraza" self~ fs-name!
  self~ be-wearable
  self~ belongs-to-protagonist
  self~ taken
  self~ be-worn  ;

: describe-door  ( -- )
  door~ times-open if  s" Es"  else  s" Parece"  then
  s" muy" 50%nullify txt+ s{ s" recia" s" gruesa" s" fuerte" }s txt+
  location-47~ am-i-there? if
    lock~ is-known?
    if    s" . A ella está unido el candado"
    else  s"  y tiene un gran candado"
    then  s+ lock-found
  then  period+
  s" Está" txt+ door~ «open»|«closed» txt+ period+ paragraph  ;

door~ :init  ( -- )
  ['] describe-door self~ be-describer
  s" puerta" self~ fs-name!
  self~ be-closed
  location-47~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-emerald  ( -- )
  s" Es preciosa."
  paragraph  ;

emerald~ :init  ( -- )
  ['] describe-emerald self~ be-describer
  s" esmeralda" self~ fs-name!
  location-39~ self~ be-there  ;

: describe-fallen-away  ( -- )
  s{
    s" Muchas," s" Muchísimas," s" Numerosas,"
    s" Un gran número de" s" Una gran cantidad de"
    \ XXX TODO -- si se añade lo que sigue,
    \ hay que crear los entes "pared" y "muro":
    \ s" Un muro de" s" Una pared de"
  }s
  s{ s" inalcanzables" s" inaccesibles" }s txt+
  s{ s" y enormes" s" y pesadas" s" y grandes" }s 50%nullify txt+
  s" rocas," txt+ s{ s" apiladas" s" amontonadas" }s txt+
  s{
    s" una sobre otra"
    s" unas sobre otras"
    s" una encima de otra"
    s" unas encima de otras"
    s" la una encima de la otra"
    s" las unas encima de las otras"
    s" la una sobre la otra"
    s" las unas sobre las otras"
  }s txt+ period+
  paragraph  ;

fallen-away~ :init  ( -- )
  ['] describe-fallen-away self~ be-describer
  s" derrumbe" self~ ms-name!
  self~ be-decoration
  location-09~ self~ be-there  ;

: describe-flags  ( -- )
  s" Son las banderas britana y sajona:"
  s" Dos dragones rampantes," txt+
  s" rojo y blanco respectivamente, enfrentados." txt+
  paragraph  ;

flags~ :init  ( -- )
  ['] describe-flags self~ be-describer
  s" banderas" self~ fp-name!
  self~ be-decoration
  location-28~ self~ be-there  ;

: describe-flint  ( -- )
  s" Es una piedra dura y afilada."
  paragraph  ;

flint~ :init  ( -- )
  ['] describe-flint self~ be-describer
  s" pedernal" self~ ms-name!  ;

: describe-grass  ( -- )
  door~ times-open if
    s" Está" grass~ verb-number-ending+
    s" aplastad" grass~ adjective-ending+ txt+
    s{ s" en el" s" bajo el" s" a lo largo del" }s txt+
    s{ s" trazado" s" recorrido" }s txt+
    s{ s" de la puerta." s" que hizo la puerta al abrirse." }s txt+
  else
    s" Cubre" grass~ verb-number-ending+
    s" el suelo junto a la puerta, lo" txt+
    s{ s" que" s" cual" }s txt+
    s{ s" indica" s" significa" s" delata" }s
    s" que ésta" txt+
    s{ s" no ha sido abierta en" s" lleva cerrada"
    s" ha permanecido cerrada" s" durante" 50%nullify txt+ }s
    s" mucho tiempo." txt+
  then  paragraph  ;

grass~ :init  ( -- )
  ['] describe-grass self~ be-describer
  s" hierba" self~ fs-name!
  self~ be-decoration  ;

: describe-idol  ( -- )
  s" El ídolo tiene dos agujeros por ojos."
  paragraph  ;

idol~ :init  ( -- )
  ['] describe-idol self~ be-describer
  s" ídolo" self~ ms-name!
  self~ be-decoration
  location-41~ self~ be-there  ;

: describe-key  ( -- )
  s" Es una llave grande, de hierro herrumboso."
  paragraph  ;

key~ :init  ( -- )
  ['] describe-key self~ be-describer
  s" llave" self~ fs-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-lake  ( -- )
  s{ s" La" s" Un rayo de" }s
  s" luz entra por un resquicio," txt+
  s" y sus caprichosos reflejos te maravillan." txt+
  paragraph  ;

lake~ :init  ( -- )
  ['] describe-lake self~ be-describer
  s" lago" self~ ms-name!
  self~ be-decoration
  location-44~ self~ be-there  ;

: describe-lock  ( -- )
  s" Es grande y parece" s{ s" fuerte." s" resistente." }s txt+
  s" Está" txt+ s{ s" fijad" s" unid" }s txt+ lock~ adjective-ending+
  s" a la puerta y" txt+
  lock~ «open»|«closed» txt+ period+
  paragraph  ;

lock~ :init  ( -- )
  ['] describe-lock self~ be-describer
  s" candado" self~ ms-name!
  self~ be-decoration
  self~ be-closed
  self~ ambrosio~ be-owner  ;

: describe-log  ( -- )
  s" Es un tronco"
  s{ s" recio," s" resistente," s" fuerte," }s txt+
  but$ txt+ s{ s" liviano." s" ligero." }s txt+
  paragraph  ;

log~ :init  ( -- )
  ['] describe-log self~ be-describer
  s" tronco" self~ ms-name!
  location-15~ self~ be-there  ;

: describe-piece  ( -- )
  s" Un pequeño" s{ s" retal" s" pedazo" s" trozo" s" resto" }s txt+
  of-your-ex-cloak$ txt+
  paragraph  ;

piece~ :init  ( -- )
  ['] describe-piece self~ be-describer
  s" trozo de tela" self~ ms-name!  ;
  \ XXX TODO -- ojo con este «de tela»: «tela» es sinónimo de trozo;
  \ hay que contemplar estos casos en el cálculo de los genitivos.

: describe-rags  ( -- )
  s" Un" s{ s" retal" s" pedazo" s" trozo" }s txt+
  s{ s" un poco" s" algo" }s 50%nullify txt+ s" grande" txt+
  of-your-ex-cloak$ txt+
  paragraph  ;

rags~ :init  ( -- )
  ['] describe-rags self~ be-describer
  s" harapo" self~ ms-name!  ;

: you-discover-the-cave-entrance  ( -- )
  you-discover-the-cave-entrance$ period+ narrate
  open-the-cave-entrance  cave-entrance~ be-here  ;
  \ Descubres la cueva.

: you-maybe-discover-the-cave-entrance  ( ca len -- )
  s" ..." s+ narrate
  3 random if  narration-break you-discover-the-cave-entrance  then  ;
  \ Descubres la cueva con un 66% de probabilidad.
  \ ca len = Texto introductorio

: describe-ravine-wall  ( -- )
  s" en" was-the-cave-entrance-discovered? ?keep
  s" la pared" txt+ rocky(f)$ txt+ ^uppercase
  was-the-cave-entrance-discovered? if
    s" , que" it-looks-impassable$ txt+ comma+ 50%nullify s+
    the-cave-entrance-is-visible$ txt+
    period+ paragraph
  else
    it-looks-impassable$ txt+
    ravine-wall~ is-known?
    if    you-maybe-discover-the-cave-entrance
    else  period+ paragraph
    then
  then  ;

ravine-wall~ :init  ( -- )
  ['] describe-ravine-wall self~ be-describer
  s" pared" rocky(f)$ txt+ self~ fs-name!
  location-08~ self~ be-there
  self~ be-not-listed  \ XXX OLD -- innecesario
  self~ be-decoration  ;

: describe-rocks  ( -- )
  location-31~ has-north-exit?
  if  (rocks)-on-the-floor$ ^uppercase
  else  (rocks)-clue$
  then  period+ paragraph  ;

rocks~ :init  ( -- )
  ['] describe-rocks self~ be-describer
  s" rocas" self~ fp-name!
  self~ be-decoration
  location-31~ self~ be-there  ;

: describe-snake  ( -- )
  s" Una serpiente grande, muy atenta a tu más mínimo movimiento."
  paragraph  ;

snake~ :init  ( -- )
  ['] describe-snake self~ be-describer
  s" serpiente" self~ fs-name!
  self~ be-animal
  location-43~ self~ be-there  ;
  \ XXX TODO -- distinguir si está muerta; en el programa original no
  \ hace falta

: describe-stone  ( -- )
  s" Recia y pesada, pero no muy grande, de forma piramidal."
  paragraph  ;

stone~ :init  ( -- )
  ['] describe-stone self~ be-describer
  s" piedra" self~ fs-name!
  location-18~ self~ be-there  ;

: describe-sword  ( -- )
  s{ s" Legado" s" Herencia" }s s" de tu padre," txt+
  s{ s" fiel herramienta" s" arma fiel" }s txt+ s" en" txt+
  s{ s" mil" s" incontables" s" innumerables" }s txt+
  s" batallas." txt+
  paragraph  ;

sword~ :init  ( -- )
  ['] describe-sword self~ be-describer
  s" espada" self~ fs-name!
  self~ belongs-to-protagonist
  self~ taken  ;

: describe-table  ( -- )
  s" Es pequeña y de" s{ s" basta" s" tosca" }s txt+ s" madera." txt+
  paragraph  ;

table~ :init  ( -- )
  ['] describe-table self~ be-describer
  s" mesa" self~ fs-name!
  location-46~ self~ be-there
  self~ ambrosio~ be-owner  ;

: describe-thread  ( -- )
  s" Un hilo" of-your-ex-cloak$ txt+
  paragraph  ;

thread~ :init  ( -- )
  ['] describe-thread self~ be-describer
  s" hilo" self~ ms-name!  ;

: describe-torch  ( -- )
  s" Está" torch~ is-lighted?
  if  s" encendida."  else  s" apagada."  then  txt+ paragraph  ;

torch~ :init  ( -- )
  ['] describe-torch self~ be-describer
  s" antorcha" self~ fs-name!
  self~ be-light-source
  self~ be-not-lighted  ;

: describe-waterfall  ( -- )
  s" No ves nada por la cortina de agua."
  s" El lago es muy poco profundo." txt+
  paragraph  ;

waterfall~ :init  ( -- )
  ['] describe-waterfall self~ be-describer
  s" cascada" self~ fs-name!
  self~ be-decoration
  location-38~ self~ be-there  ;

\ ----------------------------------------------
\ Entes escenario

\ Las palabras que describen entes escenario reciben en `sight`
\ (variable que está creada con `value` y por tanto devuelve su
\ valor como si fuera una constante) un identificador de ente.
\ Puede ser el mismo ente escenario o un ente de dirección.  Esto
\ permite describir lo que hay más allá de cada escenario en
\ cualquier dirección.

: describe-location-01  ( -- )
  sight case
  my-location of
    s" No ha quedado nada en pie, ni piedra sobre piedra."
    s{ s" El entorno es desolador." s" Todo alrededor es desolación." }s
    rnd2swap txt+
    s{ ^only$ remains$ txt+
    s" Lo único que" remains$ txt+ s" por hacer" 50%nullify txt+ s" es" txt+
    s" No" remains$ txt+ s{ s" más" s" otra cosa" }s txt+ s" que" txt+
    }s txt+ to-go-back$ txt+ s" al sur, a casa." txt+
    paragraph
    endof
  south~ of
    2 random if \ Versión 0:
      ^toward-the(m)$ s" sur" txt+
      s{ s" está" s" puedo ver" s" se puede ver" }s txt+
      s" la colina." txt+  \ Descripción principal
      s" Y mucho más allá está tu" home$ txt+ period+  \ Coletilla...
      2 random * txt+  \ ...que aparecerá con un 50% de probabilidad
    else  \ Versión 1:
      s" Muy lejos de aquí está tu" home$ txt+ comma+
      s" y el camino empieza detrás de aquella colina." txt+
    then  paragraph
    endof
  up~ of
    s{ s" pronto" s" sin compasión" s" de inmediato" }s
    s{ s" vencidas" s" derrotadas" s" sojuzgadas" }s rnd2swap txt+ ^uppercase
    s" por la fría" txt+
    s{ s" e implacable" s" y despiadada" }s 50%nullify txt+
    s" niebla," txt+ s" torpes" s" tristes" both?&
    s" columnas de" txt+ s" negro" s" humo" rnd2swap txt+ txt+
    (they)-go-up$ txt+
    s{ s" lastimosamente" s" penosamente" }s txt+
    s" hacia" s{ s" el cielo" s" las alturas" }s txt+ 50%nullify txt+
    s{ s" desde" s" de entre" }s txt+ rests-of-the-village$ txt+
    s" , como si" s" también" s" ellas" rnd2swap txt+ 50%nullify txt+
    s{ s" desearan" s" anhelaran" s" soñaran" }s txt+
    s" poder hacer un último esfuerzo por" 50%nullify txt+
    s" escapar" txt+ but|and$ txt+ s" no supieran cómo" txt+ 50%nullify s+
    s" ..." s+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-01  ( -- )
  soldiers~ be-here
  still-in-the-village?
  if  celebrating  else  going-home  then  ;

location-01~ :init  ( -- )
  self~ be-location
  ['] describe-location-01 self~ be-describer
  ['] after-describing-location-01 self~ be-after-description-plotter
  s" aldea sajona" self~ fs-name!
  0 location-02~ 0 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear colina en los tres primeros escenarios

: describe-location-02  ( -- )
  sight case
  my-location of
    s" Sobre" s" la cima de" 50%nullify txt+
    s" la colina, casi" txt+ s{ s" sobre" s" por encima de" }s txt+
    s" la" txt+
    s" espesa" s" fría" both?& s" niebla de la aldea sajona arrasada al norte, a tus pies." txt+
    ^the-path$ txt+ goes-down$ txt+ toward-the(m)$ txt+ s" oeste." txt+
    paragraph
    endof
  north~ of
    s" La" poor-village$ txt+ s" sajona" txt+ s" , arrasada," 50%nullify s+ s" agoniza bajo la" txt+
    s" espesa" s" fría" both?& s" niebla." txt+
    paragraph
    endof
  west~ of
    ^the-path$ goes-down$ txt+ s" por la" txt+ s" ladera de la" 50%nullify txt+ s" colina." txt+
    paragraph
    endof
  down~ of
    location-02~ down-exit location-02~ north-exit =
    if  north~  else  west~  then  describe
    endof
  uninteresting-direction
  endcase  ;

: from-the-village?  ( -- f )
  location-01~ protagonist~ was-there?  ;
  \ ¿Venimos de la aldea?

: after-describing-location-02  ( -- )
  location-02~  from-the-village?
  if  location-03~  else  location-01~  then  d-->
  soldiers~ be-here going-home  ;
  \ Decide hacia dónde conduce la dirección "hacia abajo",
  \ según el escenario de procedencia.

location-02~ :init  ( -- )
  self~ be-location
  ['] describe-location-02 self~ be-describer
  ['] after-describing-location-02 self~ be-after-description-plotter
  s" cima de la colina" self~ fs-name!
  location-01~ 0 0 location-03~ 0 0 0 0 self~ set-exits  ;

  \ N.B. Desde el escenario 02, uno puede bajar por el sur o por el
  \ oeste; esto se decide al azar cada vez que se entra en el
  \ escenario. Por ello descripción tiene esto en cuenta y
  \ redirige a la descripción adecuada.


  \ XXX TODO -- crear entes en escenario 2: aldea, niebla

: describe-location-03  ( -- )
  sight case
  my-location of
    ^the-path$ s" avanza por el valle," txt+
    s" desde la parte alta, al este," txt+
    s" a una zona" txt+ very-or-null$ txt+ s" boscosa, al oeste." txt+
    paragraph
    endof
  east~ of
    ^the-path$ s" se pierde en la parte alta del valle." txt+
    paragraph
    endof
  west~ of
    s" Una zona" very-or-null$ txt+ s" boscosa." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-03~ :init  ( -- )
  self~ be-location
  ['] describe-location-03 self~ be-describer
  ['] soldiers-are-here self~ be-after-description-plotter
  s" camino entre colinas" self~ ms-name!
  0 0 location-02~ location-04~ 0 0 0 0 self~ set-exits  ;

: describe-location-04  ( -- )
  sight case
  my-location of
    s" Una senda parte al oeste, a la sierra por el paso del Perro,"
    s" y otra hacia el norte, por un frondoso bosque que la rodea." txt+
    paragraph
    endof
  north~ of
    ^a-path$ surrounds$ txt+ s" la sierra a través de un frondoso bosque." txt+
    paragraph
    endof
  west~ of
    ^a-path$ leads$ txt+ toward-the(f)$ txt+ s" sierra por el paso del Perro." txt+
    paragraph
    endof
  down~ of  endof
  up~ of  endof
  uninteresting-direction
  endcase  ;

location-04~ :init  ( -- )
  self~ be-location
  ['] describe-location-04 self~ be-describer
  ['] soldiers-are-here self~ be-after-description-plotter
  s" cruce de caminos" self~ ms-name!
  location-05~ 0 location-03~ location-09~ 0 0 0 0 self~ set-exits  ;

: describe-location-05  ( -- )
  sight case
  my-location of
    ^toward-the(m)$ s" oeste se extiende" txt+
    s{ s" frondoso" s" exhuberante" }s txt+ \ XXX TODO -- independizar
    s" el bosque que rodea la sierra." txt+
    s" La salida se abre" txt+
    toward-the(m)$ txt+ s" sur." txt+
    paragraph
    endof
  south~ of
    s" Se ve la salida del bosque."
    paragraph
    endof
  west~ of
    s" El bosque se extiende"
    s{ s" exhuberante" s" frondoso" }s txt+
    s" alrededor de la sierra." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-05~ :init  ( -- )
  self~ be-location
  ['] describe-location-05 self~ be-describer
  ['] soldiers-are-here self~ be-after-description-plotter
  s" linde del bosque" self~ fs-name!
  0 location-04~ 0 location-06~ 0 0 0 0 self~ set-exits  ;

: describe-location-06  ( -- )
  sight case
  my-location of
    s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
    ^the-path$ txt+ s" serpentea entre raíces, de un luminoso este" txt+
    toward-the(m)$ txt+ s" oeste." txt+
    paragraph
    endof
  east~ of
    s" De la linde del bosque"
    s{ s" procede" s" llega" s" viene" }s txt+
    s{ s" una cierta" s" algo de" s" un poco de" }s txt+
    s{ s" claridad" s" luminosidad" }s txt+
    s" entre" txt+
    s{ s" el follaje" s" la vegetación" }s txt+ period+
    paragraph
    endof
  west~ of
    s" La niebla parece más" s" densa" s" oscura" both?& period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-06~ :init  ( -- )
  self~ be-location
  ['] describe-location-06 self~ be-describer
  ['] soldiers-are-here self~ be-after-description-plotter
  s" bosque" self~ ms-name!
  0 0 location-05~ location-07~ 0 0 0 0 self~ set-exits  ;

: describe-location-07  ( -- )
  sight case
  my-location of
    s" Abruptamente, el bosque desaparece y deja paso a un estrecho camino entre altas rocas."
    s" El" txt+ s{ s" inquietante" s" sobrecogedor" }s txt+
    s" desfiladero" txt+ s{ s" tuerce" s" gira" }s txt+
    s" de este a sur." txt+
    paragraph
    endof
  south~ of
    ^the-path$ s" gira" txt+ in-that-direction$ txt+ period+
    paragraph
    endof
  east~ of
    s" La estrecha senda es" s{ s" engullida" s" tragada" }s txt+
    s" por las" txt+
    s" fauces" s{ s" frondosas" s" exhuberantes" }s rnd2swap txt+ txt+
    s" del bosque." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-07~ :init  ( -- )
  self~ be-location
  ['] describe-location-07 self~ be-describer
  ['] soldiers-are-here self~ be-after-description-plotter
  s" paso del Perro" self~ ms-name!
  0 location-08~ location-06~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-08  ( -- )
  sight case
  my-location of
    ^the-pass-way$ s" entre el desfiladero sigue de norte a este" txt+
    s" junto a una" txt+
    s{  s" pared" rocky(f)$ txt+ s" rocosa pared" }s txt+ period+
      \ XXX TODO -- completar con entrada a caverna, tras ser descubierta
    paragraph
    endof
  north~ of
    s" El camino" s{ s" tuerce" s" gira" }s txt+
      \ XXX TODO -- independizar gira/tuerce
    s" hacia el inquietante paso del Perro." txt+
    paragraph
    endof
  south~ of
    s{ ^in-that-direction$ s" Hacia el sur" }s
    s{ s" se alza" s" se levanta" }s txt+
    \ s" una pared" txt+ rocky(f)$ txt+ \ XXX OLD
    ravine-wall~ full-name txt+
    was-the-cave-entrance-discovered? if
      comma+ s" en la" txt+ s{ s" que" s" cual" }s txt+
      the-cave-entrance-is-visible$ txt+
      period+ paragraph
    else
      ravine-wall~ is-known? if
        s" que" it-looks-impassable$ txt+ 50%nullify txt+
        you-maybe-discover-the-cave-entrance
      else
        period+ paragraph  ravine-wall~ familiar++
      then
    then
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-08  ( -- )
  soldiers-are-here pass-still-open? ?? ambush  ;

location-08~ :init  ( -- )
  self~ be-location
  ['] describe-location-08 self~ be-describer
  ['] after-describing-location-08 self~ be-after-description-plotter
  s" desfiladero" self~ ms-name!
  location-07~ 0 0 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear pared y roca y desfiladero en escenario 08

: describe-location-09  ( -- )
  sight case
  my-location of
    ^the-path$ goes-down$ txt+ s" hacia la agreste sierra, al oeste," txt+
    s" desde los" txt+ s" verdes" s" valles" rnd2swap txt+ txt+ s" al este." txt+
    ^but$ txt+ s" un" txt+ s{ s" gran" s" enorme" }s 50%nullify txt+ s" derrumbe" txt+
    (it)-blocks$ txt+ s" el paso hacia" txt+ s{ s" el oeste." s" la sierra." }s txt+
    paragraph
    endof
  east~ of
    ^can-see$ s" la salida del bosque." txt+
    paragraph
    endof
  west~ of
    s" Un gran derrumbe" (it)-blocks$ txt+ the-pass$ txt+
    toward$ txt+ s" la sierra." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-09~ :init  ( -- )
  self~ be-location
  ['] describe-location-09 self~ be-describer
  ['] soldiers-are-here self~ be-after-description-plotter
  s" derrumbe" self~ ms-name!
  0 0 location-04~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-10  ( -- )
  sight case
  my-location of
    s" El estrecho paso se adentra hacia el oeste, desde la boca, al norte."
    paragraph
    endof
  north~ of
    s" La boca de la gruta conduce al exterior."
    paragraph
    endof
  east~ of
  endof
  uninteresting-direction
  endcase  ;

: after-describing-location-10  ( -- )
  s" entrada a la cueva" cave-entrance~ fs-name!
  cave-entrance~ familiar++
  location-08~ my-previous-location = if  \ Venimos del exterior
    location-10~ visits
    if  ^again$  else  ^finally$ s" ya" 50%nullify txt+  then
    \ XXX TODO -- ampliar con otros textos alternativos
    you-think-you're-safe$ txt+
    but-it's-an-impression$ 50%nullify s+
    period+ narrate
  then  ;
  \ XXX TODO -- si venimos del interior, mostrar otros textos

location-10~ :init  ( -- )
  self~ be-location
  ['] describe-location-10 self~ be-describer
  ['] after-describing-location-10 self~ be-after-description-plotter
  s" gruta de entrada" self~ fs-name!
  self~ be-indoor-location
  location-08~ 0 0 location-11~ 0 0 0 0 self~ set-exits  ;

: describe-location-11  ( -- )
  sight case
  my-location of
    s" Una" s{
      s{ s" gran" s" amplia" }s s" estancia" txt+
      s" estancia" s" muy" 50%nullify txt+ s{ s" grande" s" amplia" }s txt+
    }s txt+ s" alberga un lago de" txt+
    s{
      s" profundas" s" aguas" rnd2swap txt+ comma+ s" e" 50%nullify txt+ s" iridiscentes" txt+
      s" aguas tan profundas como iridiscentes,"
    }s txt+
    s{ s" gracias a" s" debido a" s" a causa de" s" por el efecto de" }s txt+
    s{
      s" la" s{ s" débil" s" tenue" }s 50%nullify txt+ s" luz" txt+
        s{  s" que se filtra" s{ s" del" s" desde el" }s txt+
            s{ s" procendente" s" que procede" s" que entra" }s s" del" txt+
        }s
      s" los" s{ s" débiles" s" tenues" }s 50%nullify txt+ s" rayos de luz" txt+
        s{  s" que se filtran" s{ s" del" s" desde el" }s txt+
            s{ s" procendentes" s" que proceden" s" que entran" }s s" del" txt+
        }s
    }s 50%nullify txt+ s" exterior." txt+
    s" No hay" txt+ s{ s" otra" s" más" }s txt+ s" salida que el este." txt+
    paragraph
    endof
  east~ of
    s" De la entrada de la gruta"
    s{ s" procede" s" proviene" }s txt+
    s" la" txt+ s{ s" luz" s" luminosidad" s" claridad" }s
    s" que hace brillar" txt+
    s{ s" el agua" s" las aguas" s" la superficie" }s txt+
    s" del lago." txt+
    paragraph
  endof
  uninteresting-direction
  endcase  ;

location-11~ :init  ( -- )
  self~ be-location
  ['] describe-location-11 self~ be-describer
  ['] lake-is-here self~ be-after-description-plotter
  s" gran lago" self~ ms-name!
  self~ be-indoor-location
  0 0 location-10~ 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. estancia y aguas en escenario 11

: describe-location-12  ( -- )
  sight case
  my-location of
    s" Una" s{ s" gran" s" amplia" }s txt+
    s" estancia se abre hacia el oeste," txt+
    s" y se estrecha hasta" txt+ s{ s" morir" s" terminar" }s txt+
    s" , al este, en una" s+ s{ s" parte" s" zona" }s txt+ s" de agua." txt+
    paragraph
    endof
  east~ of
    s{ s" La estancia" s" El lugar" }s
    s" se estrecha hasta " txt+
    s{ s" morir" s" terminar" }s txt+
    s" en una" txt+ s{ s" parte" s" zona" }s txt+ s" de agua." txt+
    paragraph
  endof
  west~ of
    s" Se vislumbra la continuación de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-12~ :init  ( -- )
  self~ be-location
  ['] describe-location-12 self~ be-describer
  s" salida del paso secreto" self~ fs-name!
  self~ be-indoor-location
  0 0 0 location-13~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente agua en escentario 12

: describe-location-13  ( -- )
  sight case
  my-location of
    s" La sala se abre en"
    s{ s" semioscuridad" s" penumbra" }s txt+
    s" a un puente cubierto de podredumbre" txt+
    s" sobre el lecho de un canal, de este a oeste." txt+
    paragraph
    endof
  east~ of
    s" Se vislumbra el inicio de la cueva."
    paragraph
  endof
  west~ of
    s" Se vislumbra un recodo de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-13~ :init  ( -- )
  self~ be-location
  ['] describe-location-13 self~ be-describer
  s" puente semipodrido" self~ ms-name!
  self~ be-indoor-location
  0 0 location-12~ location-14~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. canal, agua, lecho(~catre) en escenario 14

: describe-location-14  ( -- )
  sight case
  my-location of
    s" La iridiscente cueva gira de este a sur."
    paragraph
    endof
  south~ of
    you-glimpse-the-cave$ paragraph
    endof
  east~ of
    you-glimpse-the-cave$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-14~ :init  ( -- )
  self~ be-location
  ['] describe-location-14 self~ be-describer
  s" recodo de la cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-15~ location-13~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-15  ( -- )
  sight case
  my-location of
    s" La gruta" goes-down$ txt+ s" de norte a sur" txt+
    s" sobre un lecho arenoso." txt+
    s" Al este, un agujero del que llega" txt+
    s{ s" algo de luz." s" claridad." }s txt+
    paragraph
    endof
  north~ of
    you-glimpse-the-cave$
    s" La cueva" goes-up$ txt+ in-that-direction$ txt+ period+
    paragraph
    endof
  south~ of
    you-glimpse-the-cave$
    s" La cueva" goes-down$ txt+ in-that-direction$ txt+ period+
    paragraph
    endof
  east~ of
    s{ s" La luz" s" Algo de luz" s" Algo de claridad" }s
    s" procede de esa dirección." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-15~ :init  ( -- )
  self~ be-location
  ['] describe-location-15 self~ be-describer
  s" pasaje arenoso" self~ ms-name!
  self~ be-indoor-location
  location-14~ location-17~ location-16~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-16  ( -- )
  sight case
  my-location of
    s" Como un acueducto, el agua"
    goes-down$ txt+ s" con gran fuerza de norte a este," txt+
    s" aunque la salida practicable es la del oeste." txt+
    paragraph
    endof
  north~ of
    s" El agua" goes-down$ txt+ s" con gran fuerza" txt+ from-that-way$ txt+ period+
    paragraph
    endof
  east~ of
    s" El agua" goes-down$ txt+ s" con gran fuerza" txt+ that-way$ txt+ period+
    paragraph
    endof
  west~ of
    s" Es la única salida." paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-16  ( -- )
  s" En la distancia, por entre los resquicios de las rocas,"
  s" y allende el canal de agua, los sajones" txt+
  s{ s" intentan" s" se esfuerzan en" s" tratan de" s" se afanan en" }s txt+
  s{ s" hallar" s" buscar" s" localizar" }s txt+
  s" la salida que encontraste por casualidad." txt+
  narrate  ;

location-16~ :init  ( -- )
  self~ be-location
  ['] describe-location-16 self~ be-describer
  ['] after-describing-location-16 self~ be-after-description-plotter
  s" pasaje del agua" self~ ms-name!
  self~ be-indoor-location
  0 0 0 location-15~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- el examen del agua aquí debe dar más pistas en
  \ escenario 16

: describe-location-17  ( -- )
  sight case
  my-location of
    s" Muchas estalactitas se agrupan encima de tu cabeza,"
    s" y se abren cual arco de entrada hacia el este y sur." txt+
    paragraph
    endof
  north~ of
    you-glimpse-the-cave$
    paragraph
    endof
  up~ of
    s" Las estalactitas se agrupan encima de tu cabeza."
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-17~ :init  ( -- )
  self~ be-location
  ['] describe-location-17 self~ be-describer
  s" estalactitas" self~ fs-name!
  self~ be-indoor-location
  location-15~ location-20~ location-18~ 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. estalactitas en escenario 17

: describe-location-18  ( -- )
  sight case
  my-location of
    s" Un arco de piedra se extiende,"
    s{ s" cual" s" como si fuera un" s" a manera de" }s txt+
    s" puente" txt+
    s" que se" s{ s" elevara" s" alzara" }s txt+ 50%nullify txt+
    s{ s" sobre" s" por encima de" }s txt+
    s" la oscuridad, de este a oeste." txt+
    s{ s" Hacia" s" En" }s txt+ s" su mitad" txt+
    altar~ is-known?
    if    s" está" txt+
    else  s{ s" hay" s" es posible ver" s" puede verse" }s txt+
    then  altar~ full-name txt+ period+ paragraph
    endof
  east~ of
    s" El arco de piedra se extiende" that-way$ txt+ period+
    paragraph
    endof
  west~ of
    s" El arco de piedra se extiende" that-way$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-18~ :init  ( -- )
  self~ be-location
  ['] describe-location-18 self~ be-describer
  s" puente de piedra" self~ ms-name!
  self~ be-indoor-location
  0 0 location-19~ location-17~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. puente, arco en escenario 18

: describe-location-19  ( -- )
  sight case
  my-location of
    ^the-water-current$ comma+
    s" que discurre" 50%nullify txt+
    s" de norte a este," txt+ (it)-blocks$ txt+
    s" el paso, excepto al oeste." txt+
    s{ s" Al" s" Del" s" Hacia el" s" Proveniente del" s" Procedente del" }s txt+
    s" fondo" txt+ s{ s" se oye" s" se escucha" s" puede oírse" }s txt+
    s" un gran estruendo." txt+
      \ XXX TODO -- hacer variaciones de todo este texto
    paragraph
    endof
  north~ of
    ^water-from-there$ period+ paragraph
    endof
  east~ of
    water-that-way$ paragraph
    endof
  west~ of
    s" Se puede" to-go-back$ txt+
    toward-the(m)$ txt+ s" arco de piedra" txt+
    in-that-direction$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-19~ :init  ( -- )
  self~ be-location
  ['] describe-location-19 self~ be-describer
  s" recodo arenoso del canal" self~ ms-name!
  self~ be-indoor-location
  0 0 0 location-18~ 0 0 0 0 self~ set-exits  ;

: describe-location-20  ( -- )
  sight case
  my-location of
    north~ south~ east~ 3 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

: can-be-entered-location-20?  ( -- f )
  location-17~ am-i-there? no-torch? and
  dup 0= swap ?? dark-cave  ;

location-20~ :init  ( -- )
  self~ be-location
  ['] describe-location-20 self~ be-describer
  ['] can-be-entered-location-20? self~ ~enter-checker !
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-17~ location-22~ location-25~ 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- aplicar el filtro de la antorcha a todos los
  \ escenarios afectados, quizá en una capa superior
  \ sight no-torch? 0= abs *  case

: describe-location-21  ( -- )
  sight case
  my-location of
    east~ west~ south~ 3 exits-cave-description paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-21~ :init  ( -- )
  self~ be-location
  ['] describe-location-21 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-27~ location-23~ location-20~ 0 0 0 0 self~ set-exits  ;

: describe-location-22  ( -- )
  sight case
  my-location of
    south~ east~ west~ 3 exits-cave-description paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-22~ :init  ( -- )
  self~ be-location
  ['] describe-location-22 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-24~ location-27~ location-22~ 0 0 0 0 self~ set-exits  ;

: describe-location-23  ( -- )
  sight case
  my-location of
    west~ south~ 2 exits-cave-description paragraph
    endof
  south~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-23~ :init  ( -- )
  self~ be-location
  ['] describe-location-23 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  0 location-25~ 0 location-21~ 0 0 0 0 self~ set-exits  ;

: describe-location-24  ( -- )
  sight case
  my-location of
    east~ north~ 2 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-24~ :init  ( -- )
  self~ be-location
  ['] describe-location-24 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-22~ 0 location-26~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-25  ( -- )
  sight case
  my-location of
    north~ south~ east~ west~ 4 exits-cave-description paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-25~ :init  ( -- )
  self~ be-location
  ['] describe-location-25 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-22~ location-28~ location-23~ location-21~ 0 0 0 0 self~ set-exits  ;

: describe-location-26  ( -- )
  sight case
  my-location of
    north~ east~ west~ 3 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-26~ :init  ( -- )
  self~ be-location
  ['] describe-location-26 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-26~ 0 location-20~ location-27~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. pasaje/camino/senda tramo/cueva (en todos
  \ los tramos)

: describe-location-27  ( -- )
  sight case
  my-location of
    north~ east~ west~ 3 exits-cave-description paragraph
    endof
  north~ of
    cave-exit-description$ paragraph
    endof
  east~ of
    cave-exit-description$ paragraph
    endof
  west~ of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-27~ :init  ( -- )
  self~ be-location
  ['] describe-location-27 self~ be-describer
  s" tramo de cueva" self~ ms-name!
  self~ be-indoor-location
  location-27~ 0 0 location-25~ 0 0 0 0 self~ set-exits  ;

: describe-location-28  ( -- )
  sight case
  my-location of
    location-28~ ^full-name s" se extiende de norte a este." txt+
    leader~ conversations?
    if  s" Hace de albergue para los refugiados."
    else  s" Está llen" location-28~ gender-ending+ s" de gente." txt+
    then  txt+
    flags~ is-known?
    if
      s" Hay" txt+
      s{  s" una bandera de cada bando"
          s" banderas de" s{ s" ambos" s" los dos" }s txt+ s" bandos" txt+
          s{ s" dos banderas: una" s" una bandera" }s
              s{  s" britana y otra sajona" s" sajona y otra britana" }s txt+
      }s txt+ period+
    else
      s" Hay" txt+ s{ s" dos" s" unas" }s txt+ s" banderas." txt+
    then
    paragraph
    endof
  north~ of
    location-28~ has-east-exit?
    if    s" Es por donde viniste."
    else  s" Es la única salida."
    then  paragraph
    endof
  east~ of
    ^the-refugees$
    location-28~ has-east-exit?
    if    they-let-you-pass$ txt+
    else  they-don't-let-you-pass$ txt+
    then  period+ paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-28  ( -- )
  location-28~ no-exit e-->  \ Cerrar la salida hacia el este
  recent-talks-to-the-leader off
  refugees~ be-here
  the-refugees-surround-you$ narrate
  the-leader-looks-at-you$ narrate  ;

location-28~ :init  ( -- )
  self~ be-location
  ['] describe-location-28 self~ be-describer
  ['] after-describing-location-28 self~ be-after-description-plotter
  s" amplia estancia" self~ fs-name!
  self~ be-indoor-location
  location-26~ 0 0 0 0 0 0 0 self~ set-exits  ;
  \ XXX TODO -- crear ente. estancia(para todos),albergue y refugio
  \ (tras hablar con anciano) en escenario 28

: describe-location-29  ( -- )
  sight case
  my-location of
    s" Cual escalera de caracol gigante,"
    goes-down-into-the-deep$ comma+ txt+
    s" dejando a los refugiados al oeste." txt+
    paragraph
    endof
  west~ of
    over-there$ s" están los refugiados." txt+
    paragraph
    endof
  down~ of
    s" La espiral" goes-down-into-the-deep$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-29  ( -- )
  refugees~ be-here  ;
  \ Para que sean visibles en la distancia

location-29~ :init  ( -- )
  self~ be-location
  ['] describe-location-29 self~ be-describer
  ['] after-describing-location-29 self~ be-after-description-plotter
  s" espiral" self~ fs-name!
  self~ be-indoor-location
  0 0 0 location-28~ 0 location-30~ 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. escalera/espiral, refugiados en escenario
  \ 29

: describe-location-30  ( -- )
  sight case
  my-location of
    s" Se eleva en la penumbra."
    s" La" txt+ cave$ txt+ gets-narrower(f)$ txt+
    s" ahora como para una sola persona, hacia el este." txt+
    paragraph
    endof
  east~ of
    s" La" cave$ txt+ gets-narrower(f)$ txt+ period+
    paragraph
    endof
  up~ of
    s" La" cave$ txt+ s" se eleva en la penumbra." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-30~ :init  ( -- )
  self~ be-location
  ['] describe-location-30 self~ be-describer
  s" inicio de la espiral" self~ ms-name!
  self~ be-indoor-location
  0 0 location-31~ 0 location-29~ 0 0 0 self~ set-exits  ;

: describe-location-31  ( -- )
  sight case
  my-location of
    s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
    paragraph
    endof
  north~ of
    s" Las rocas" location-31~ has-north-exit?
    if  (rocks)-on-the-floor$
    else  (they)-block$ the-pass$ txt+
    then  txt+ period+ paragraph
    endof
  west~ of
    ^that-way$ s" se encuentra el inicio de la espiral." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-31  ( -- )
  \ XXX TODO -- mover a la descripción
  location-31~ has-north-exit? if
    s" Las rocas yacen desmoronadas a lo largo del"
    pass-way$ txt+ period+
  else
    s" Las rocas" (they)-block$ txt+ s" el paso." txt+
  then  narrate  ;

location-31~ :init  ( -- )
  self~ be-location
  ['] describe-location-31 self~ be-describer
  ['] after-describing-location-31 self~ be-after-description-plotter
  s" puerta norte" self~ fs-name!
  self~ be-indoor-location
  0 0 0 location-30~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. arco, columnas, hueco/s(entre rocas) en
  \ escenario 31

: describe-location-32  ( -- )
  sight case
  my-location of
    s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
    s" El soporte de roca gira en forma de «U» de oeste a sur." txt+
    paragraph
    endof
  south~ of
    ^the-path$ s" gira" txt+ that-way$ txt+ period+
    paragraph
    endof
  west~ of
    ^the-path$ s" gira" txt+ that-way$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-32~ :init  ( -- )
  self~ be-location
  ['] describe-location-32 self~ be-describer
  s" precipicio" self~ ms-name!
  self~ be-indoor-location
  0 location-33~ 0 location-31~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. precipicio, abismo, cornisa, camino,
  \ roca/s en escenario 32

: describe-location-33  ( -- )
  sight case
  my-location of
    s" El paso se va haciendo menos estrecho a medida que se avanza hacia el sur, para entonces comenzar hacia el este."
    paragraph
    endof
  north~ of
    ^the-path$ s" se estrecha" txt+ that-way$ txt+ period+
    paragraph
    endof
  south~ of
    ^the-path$ gets-wider$ txt+ that-way$ txt+
    s" y entonces gira hacia el este." txt+
    paragraph
    endof
  east~ of
    ^the-path$ gets-wider$ txt+ that-way$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-33~ :init  ( -- )
  self~ be-location
  ['] describe-location-33 self~ be-describer
  s" pasaje de salida" self~ ms-name!
  self~ be-indoor-location
  location-32~ 0 location-34~ 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. camino/paso/sendero en escenario 33

: describe-location-34  ( -- )
  sight case
  my-location of
    s" El paso" gets-wider$ txt+ s" de oeste a norte," txt+
    s" y guijarros mojados y mohosos tachonan el suelo de roca." txt+
    paragraph
    endof
  north~ of
    ^the-path$ gets-wider$ txt+ that-way$ txt+ period+
    paragraph
    endof
  west~ of
    ^the-path$ s" se estrecha" txt+ that-way$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-34~ :init  ( -- )
  self~ be-location
  ['] describe-location-34 self~ be-describer
  s" pasaje de gravilla" self~ ms-name!
  self~ be-indoor-location
  location-35~ 0 0 location-33~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. camino/paso/sendero, guijarros, moho,
  \ roca, suelo, gravillla en escenario 34


: describe-location-35  ( -- )
  sight case
  my-location of
    s" Un puente" s{ s" se tiende" s" cruza" }s txt+ s" de norte a sur sobre el curso del agua." txt+
    s" Unas resbaladizas escaleras" txt+ (they)-go-down$ txt+ s" hacia el oeste." txt+
    paragraph
    endof
  north~ of
    bridge-that-way$ paragraph
    endof
  south~ of
    bridge-that-way$ paragraph
    endof
  west~ of
    stairway-to-river$ paragraph
    endof
  down~ of
    stairway-to-river$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-35~ :init  ( -- )
  self~ be-location
  ['] describe-location-35 self~ be-describer
  s" puente sobre el acueducto" self~ ms-name!
  self~ be-indoor-location
  location-40~ location-34~ 0 location-36~ 0 location-36~ 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. escaleras, puente, río/curso/agua en
  \ escenario 35

: describe-location-36  ( -- )
  sight case
  my-location of
    s" Una" s{ s" ruidosa" s" estruendosa" s" ensordecedora" }s txt+
    s" corriente" txt+ goes-down$ txt+
    s{ s" con" s" siguiendo" }s txt+ s" el" txt+ pass-way$ txt+
    s" elevado desde el oeste, y forma un meandro arenoso." txt+
    s" Unas escaleras" txt+ (they)-go-up$ txt+ toward-the(m)$ txt+ s" este." txt+
    paragraph
    endof
  east~ of
    stairway-that-way$ paragraph
    endof
  west~ of
    ^water-from-there$ period+ paragraph
    endof
  up~ of
    stairway-that-way$ paragraph
    endof
  uninteresting-direction
  endcase  ;

location-36~ :init  ( -- )
  self~ be-location
  ['] describe-location-36 self~ be-describer
  s" remanso" self~ ms-name!
  self~ be-indoor-location
  0 0 location-35~ location-37~ location-35~ 0 0 0 self~ set-exits  ;

: describe-location-37  ( -- )
  sight case
  my-location of
    s" El agua" goes-down$ txt+ s" por un canal" 50%nullify txt+
    from-the(m)$ txt+ s" oeste con" txt+
    s{ s" renovadas fuerzas" s" renovada energía" s" renovado ímpetu" }s txt+ comma+
    s" dejando" txt+ s{
    s" a un lado" a-high-narrow-pass-way$ txt+
    a-high-narrow-pass-way$ s{ s" lateral" s" a un lado" }s txt+
    }s txt+ s" que" txt+ lets-you$ txt+ to-keep-going$ txt+
    toward-the(m)$ s" este" txt+
    toward-the(m)$ s" oeste" txt+ rnd2swap s" o" txt+ 2swap txt+ txt+
    period+ paragraph
    endof
  east~ of
    ^the-pass-way$ s" elevado" 50%nullify txt+ lets-you$ txt+ to-keep-going$ txt+ that-way$ txt+ period+
    paragraph
    endof
  west~ of
    water-from-there$
    the-pass-way$ s" elevado" 50%nullify txt+ lets-you$ txt+ to-keep-going$ txt+ that-way$ txt+
    both s" también" txt+ ^uppercase period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-37~ :init  ( -- )
  self~ be-location
  ['] describe-location-37 self~ be-describer
  s" canal de agua" self~ ms-name!
  self~ be-indoor-location
  0 0 location-36~ location-38~ 0 0 0 0 self~ set-exits  ;

: describe-location-38  ( -- )
  sight case
  my-location of
    s" Cae el agua hacia el este,"
    s{ s" descendiendo" s" bajando" }s txt+
    s{ s" con mucha fuerza" s" con gran fuerza" s" fuertemente" }s txt+
    s{ s" en dirección al" s" hacia el" }s txt+ s" canal," txt+
    s{ s" no sin antes" s" tras" s" después de" }s txt+
    s{ s" embalsarse" s" haberse embalsado" }s txt+
    s" en un lago" txt+
    s{ s" no muy" s" no demasiado" s" poco" }s txt+ s" profundo." txt+
    paragraph
    endof
  east~ of
    water-that-way$ paragraph
    endof
  west~ of
    ^water-from-there$
    s" , de" s+ waterfall~ full-name txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-38~ :init  ( -- )
  self~ be-location
  ['] describe-location-38 self~ be-describer
  ['] lake-is-here self~ be-after-description-plotter
  s" gran cascada" self~ fs-name!
  self~ be-indoor-location
  0 0 location-37~ location-39~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- el artículo de «cascada» debe depender también de si
  \ se ha visitado el escenario 39 o este mismo 38

: describe-location-39  ( -- )
  sight case
  my-location of
    s" Musgoso y rocoso, con la cortina de agua"
    s{ s" tras de ti," s" a tu espalda," }s txt+
    s{ s" el nivel" s" la altura" }s txt+ s" del agua ha" txt+
    s{ s" subido" s" crecido" }s txt+
    s{ s" un poco" s" algo" }s txt+ s" en este" txt+
    s{ s" curioso" s" extraño" }s txt+ s" hueco." txt+
    paragraph
    endof
  east~ of
    s" Es la única salida." paragraph
      \ XXX TODO -- variar
    endof
  uninteresting-direction
  endcase  ;

location-39~ :init  ( -- )
  self~ be-location
  ['] describe-location-39 self~ be-describer
  s" interior de la cascada" self~ ms-name!
  self~ be-indoor-location
  0 0 location-38~ 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. musgo, cortina, agua, hueco en escenario
  \ 39

: describe-location-40  ( -- )
  sight case
  my-location of
    s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
    s" Unos casi imperceptibles escalones conducen al este." txt+
    paragraph
    endof
  south~ of
    ^that-way$ s" se va" txt+ toward-the(m)$ txt+ s" puente." txt+
    paragraph
    endof
  east~ of
    s" Los escalones" (they)-lead$ txt+ that-way$ txt+ period+
    paragraph
    endof
  up~ of
    s{ s" Sobre" s" Por encima de" }s
    s{ s" ti" s" tu cabeza" }s txt+ s" se" txt+
    s{ s" exhibe" s" extiende" s" disfruta" }s txt+
    s" un" txt+ beautiful(m)$ txt+
    s{ s" panorama" s" paisaje" }s txt+ s" de estalactitas." txt+
    paragraph
    endof
  down~ of
    s" Es una" s{ s" gran" s" buena" }s txt+ s" explanada enlosetada." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-40~ :init  ( -- )
  self~ be-location
  ['] describe-location-40 self~ be-describer
  s" explanada" self~ fs-name!
  self~ be-indoor-location
  0 location-35~ location-41~ 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. losas y losetas, estalactitas, panorama,
  \ escalones en escenario 40

: describe-location-41  ( -- )
  sight case
  my-location of
    s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al sur."
    s" Se puede" txt+ to-go-back$ txt+ toward$ txt+ s" la explanada hacia el oeste." txt+
    paragraph
    endof
  south~ of
    s" Hay una" s" roca" s" enorme" rnd2swap txt+ txt+
    that-way$ txt+ period+
    paragraph
    endof
  west~ of
    s" Se puede volver" toward$ txt+ s" la explanada" txt+ that-way$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-41~ :init  ( -- )
  self~ be-location
  ['] describe-location-41 self~ be-describer
  self~ be-indoor-location
  s" ídolo" self~ ms-name!
  0 0 0 location-40~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. roca, centinela en escenario 41

: describe-location-42  ( -- )
  sight case
  my-location of
    s" Como un pasillo que corteja el canal de agua, a su lado, baja de norte a sur."
    paragraph
    endof
  north~ of
    ^the-pass-way$ goes-up$ txt+ that-way$ txt+
    s" , de donde" s{ s" corre" s" procede" s" viene" s" proviene" }s txt+ s" el agua." txt+ s+
    paragraph
    endof
  south~ of
    ^the-pass-way$ goes-down$ txt+ that-way$ txt+
    s" , siguiendo el canal de agua," s+
    s" hacia un lugar en que" txt+
    s{ s" se aprecia" s" puede apreciarse" s" se distingue" }s txt+
    s" un aumento de luz." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-42~ :init  ( -- )
  self~ be-location
  ['] describe-location-42 self~ be-describer
  s" pasaje estrecho" self~ ms-name!
  self~ be-indoor-location
  location-41~ location-43~ 0 0 0 0 0 0 self~ set-exits  ;

: describe-location-43  ( -- )
  sight case
  my-location of
    ^the-pass-way$ s" sigue de norte a sur." txt+
    paragraph
    endof
  north~ of
    ^the-pass-way$ s" continúa" txt+ that-way$ txt+ period+
    paragraph
    endof
  south~ of
    snake~ is-here?
    if  a-snake-blocks-the-way$
    else  ^the-pass-way$ s" continúa" txt+ that-way$ txt+
    then  period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

: after-describing-location-43  ( -- )
  snake~ is-here? if
    a-snake-blocks-the-way$ period+
    narrate
  then  ;

location-43~ :init  ( -- )
  self~ be-location
  ['] describe-location-43 self~ be-describer
  ['] after-describing-location-43 self~ be-after-description-plotter
  s" pasaje de la serpiente" self~ ms-name!
  self~ be-indoor-location
  location-42~ 0 0 0 0 0 0 0 self~ set-exits  ;

: describe-location-44  ( -- )
  sight case
  my-location of
    s" Unas escaleras" s{ s" dan" s" permiten el" }s txt+ s{ s" paso" s" acceso" }s txt+
    s" a un" txt+ beautiful(m)$ txt+ s" lago interior, hacia el oeste." txt+
    s" Al norte, un oscuro y"
    narrow(m)$ txt+ pass-way$ txt+ goes-up$ txt+ period+ 50%nullify txt+
    paragraph
    endof
  north~ of
    s" Un pasaje oscuro y" narrow(m)$ txt+ goes-up$ txt+ that-way$ txt+ period+
    paragraph
    endof
  west~ of
    s" Las escaleras" (they)-lead$ txt+ that-way$ txt+ s" , hacia el lago" 50%nullify s+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-44~ :init  ( -- )
  self~ be-location
  ['] describe-location-44 self~ be-describer
  ['] lake-is-here self~ be-after-description-plotter
  s" lago interior" self~ ms-name!
  self~ be-indoor-location
  location-43~ 0 0 location-45~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. lago, escaleras, pasaje, lago en escenario
  \ 44

: describe-location-45  ( -- )
  sight case
  my-location of
    ^narrow(mp)$ pass-ways$ txt+
    s" permiten ir al oeste, al este y al sur." txt+
    paragraph
    endof
  south~ of
    ^a-narrow-pass-way$ s" permite ir" txt+ that-way$ txt+
    s" , de donde" s+ s{ s" proviene" s" procede" }s txt+
    s{ s" una gran" s" mucha" }s txt+ s" luminosidad." txt+
    paragraph
    endof
  west~ of
    ^a-narrow-pass-way$ leads$ txt+ that-way$ txt+ period+
    paragraph
    endof
  east~ of
    ^a-narrow-pass-way$ leads$ txt+ that-way$ txt+ period+
    s" , de donde" s{ s" proviene" s" procede" }s txt+
    s{ s" algo de" s" una poca" s" un poco de" }s txt+
    s{ s" claridad" s" luz" }s txt+ period+ s+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-45~ :init  ( -- )
  self~ be-location
  ['] describe-location-45 self~ be-describer
  s" cruce de pasajes" self~ ms-name!
  self~ be-indoor-location
  0 location-47~ location-44~ location-46~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. pasaje/camino/paso/senda en escenario 45

: describe-location-46  ( -- )
  sight case
  my-location of
    s" Un catre, algunas velas y una mesa es todo lo que"
    s{ s" tiene" s" posee" }s s" Ambrosio" rnd2swap txt+ txt+
    period+  paragraph
    endof
  east~ of
    s" La salida"
    s{ s" de la casa" s" del hogar" }s s" de Ambrosio" txt+ 50%nullify txt+
    s" está" txt+ that-way$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-46~ :init  ( -- )
  self~ be-location
  ['] describe-location-46 self~ be-describer
  s" hogar de Ambrosio" self~ ms-name!
  self~ be-indoor-location
  0 0 location-45~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-47  ( -- )
  sight case
  my-location of
    s" Por el oeste,"
    door~ full-name txt+ door~ «open»|«closed» txt+ comma+
    door~ is-open? if  \ La puerta está abierta
      s" por la cual entra la luz que ilumina la estancia," txt+
      s" permite salir de la cueva." txt+
    else  \ La puerta está cerrada
      s" al otro lado de la cual se adivina la luz diurna," txt+
      door~ is-known?
      if    s" impide" txt+
      else  s" parece ser" txt+
      then  s" la salida de la cueva." txt+
    then
    paragraph
    endof
  north~ of
    s" Hay salida" that-way$ txt+ period+ paragraph
      \ XXX TODO -- variar el texto
    endof
  west~ of
    door~ is-open?
    if    s" La luz diurna entra por la puerta."
    else  s" Se adivina la luz diurna al otro lado de la puerta."
    then  paragraph
      \ XXX TODO -- variar el texto
    endof
  uninteresting-direction
  endcase  ;

location-47~ :init  ( -- )
  self~ be-location
  ['] describe-location-47 self~ be-describer
  ['] door-is-here self~ be-after-description-plotter
  s" salida de la cueva" self~ fs-name!
  self~ be-indoor-location
  location-45~ 0 0 0 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- descripción inacabada de escenario 47

: when-the-door$  ( -- ca len )
  s" cuando" s{ s" la" s" su" }s txt+ s" puerta" txt+  ;

: like-now$+  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" , como ahora" 50%nullify s+  ;

: describe-location-48  ( -- )
  sight case
  my-location of
    s{ s" Apenas si" s" Casi no" }s
    s{ s" se puede" s" es posible" }s txt+
    s" reconocer la entrada de la cueva, al este." txt+
    ^the-path$ txt+ s{ s" parte" s" sale" }s txt+
    s" del bosque hacia el oeste." txt+
    paragraph
    endof
  east~ of
    s" La entrada de la cueva" s{
    s" está" s" bien" 50%nullify txt+ s{ s" camuflada" s" escondida" }s txt+
    s" apenas se ve" s" casi no se ve" s" pasa casi desapercibida"
    }s txt+ comma+
    door~ is-open? if
      even$ txt+ when-the-door$ txt+
      s{ s" está abierta" s" no está cerrada" }s txt+ like-now$+
    else
      s{ s" especialmente" s" sobre todo" }s txt+ when-the-door$ txt+
      s{ s" no está abierta" s" está cerrada" }s txt+ like-now$+
    then  period+ paragraph
    endof
  west~ of
    ^the-path$ s{ s" parte" s" sale" }s txt+ s" del bosque" txt+ in-that-direction$ txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-48~ :init  ( -- )
  self~ be-location
  ['] describe-location-48 self~ be-describer
  ['] door-is-here self~ be-after-description-plotter
  s" bosque a la entrada" self~ ms-name!
  0 0 location-47~ location-49~ 0 0 0 0 self~ set-exits  ;

  \ XXX TODO -- crear ente. cueva en escenario 48

: describe-location-49  ( -- )
  sight case
  my-location of
    ^the-path$ s" recorre" txt+ s" toda" 50%nullify txt+
    s" esta" txt+ s{ s" parte" s" zona" }s txt+
    s" del bosque de este a oeste." txt+
    paragraph
    endof
  east~ of
    ^the-path$ leads$ txt+
    s" al bosque a la entrada de la cueva." txt+
    paragraph
    endof
  west~ of
    ^the-path$ s" continúa" txt+ in-that-direction$ txt+ period+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-49~ :init  ( -- )
  self~ be-location
  ['] describe-location-49 self~ be-describer
  s" sendero del bosque" self~ ms-name!
  0 0 location-48~ location-50~ 0 0 0 0 self~ set-exits  ;

: describe-location-50  ( -- )
  sight case
  my-location of
    s" El camino norte" s{ s" que sale" s" que parte" s" procedente" }s txt+
    s" de Westmorland se" s{ s" interna" s" adentra" }s txt+ s" en el bosque," txt+
    s" aunque en tu estado no puedes ir." txt+
    paragraph
    endof
  south~ of
    s{ s" ¡Westmorland!" s" Westmorland..." }s
    paragraph
    endof
  east~ of
    ^can-see$ s" el sendero del bosque." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-50~ :init  ( -- )
  self~ be-location
  ['] describe-location-50 self~ be-describer
  s" camino norte" self~ ms-name!
  0 location-51~ location-49~ 0 0 0 0 0 self~ set-exits  ;

: describe-location-51  ( -- )
  sight case
  my-location of
    ^the-village$ s" bulle de actividad con el mercado en el centro de la plaza," txt+
    s" donde se encuentra el castillo." txt+
    paragraph
    endof
  north~ of
    s" El camino norte" of-the-village$ txt+ leads$ txt+ s" hasta el bosque." txt+
    paragraph
    endof
  uninteresting-direction
  endcase  ;

location-51~ :init  ( -- )
  self~ be-location
  ['] describe-location-51 self~ be-describer
  s" Westmorland" self~ fs-name!
  self~ have-no-article
  location-50~ 0 0 0 0 0 0 0 self~ set-exits  ;
  \ XXX TODO -- crear ente. mercado, plaza, villa, pueblo, castillo

\ ----------------------------------------------
\ Entes globales

: describe-cave  ( -- )
  s" La cueva es húmeda y sombría."
  paragraph  ;
  \ XXX TODO -- mejorar

cave~ :init  ( -- )
  ['] describe-cave self~ be-describer
  s" cueva" self~ fs-name!  ;
  \ self~ be-global-indoor \ XXX OLD

: describe-ceiling  ( -- )
  s" El techo es muy bonito."
  paragraph  ;
  \ XXX TODO

ceiling~ :init  ( -- )
  ['] describe-ceiling self~ be-describer
  s" techo" self~ ms-name!
  self~ be-global-indoor  ;

: describe-clouds  ( -- )
  s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
  s" en la estación del frío se han alejado por el momento. " txt+
    \ XXX TMP
  2 random if  paragraph  else  2drop sky~ describe  then  ;
    \ XXX TODO -- comprobar

clouds~ :init  ( -- )
  ['] describe-clouds self~ be-describer
  s" nubes" self~ fp-name!
  self~ be-global-outdoor  ;
  \ XXX TODO:
  \ Distinguir no solo interiores, sino escenarios en
  \ que se puede vislumbrar el exterior.

: describe-floor  ( -- )
  am-i-outdoor?
  if    s" [El suelo fuera es muy bonito.]" paragraph
  else  s" [El suelo dentro es muy bonito.]" paragraph  then  ;
  \ XXX TMP
  \ XXX TODO

floor~ :init  ( -- )
  ['] describe-floor self~ be-describer
  s" suelo" self~ ms-name!
  self~ be-global-indoor
  self~ be-global-outdoor  ;

: describe-sky  ( -- )
  s" [El cielo es muy bonito.]"
  paragraph  ;
  \ XXX TODO
  \ XXX TMP

sky~ :init  ( -- )
  ['] describe-sky self~ be-describer
  s" cielo" self~ ms-name!
  self~ be-global-outdoor  ;
  \ XXX TODO
  \ XXX TMP

: describe-wall  ( -- )
  s" [La pared es muy bonita.]"
  paragraph  ;
  \ XXX TODO
  \ XXX TMP

wall~ :init  ( -- )
  ['] describe-wall self~ be-describer
  s" pared" self~ ms-name!
  self~ be-global-indoor  ;

\ ----------------------------------------------
\ Entes virtuales

defer describe-exits  ( -- )

exits~ :init  ( -- )
  ['] describe-exits self~ be-describer
  s" salida" self~ fs-name!
  self~ be-global-outdoor
  self~ be-global-indoor  ;

defer describe-inventory  ( -- )

inventory~ :init  ( -- )
  ['] describe-inventory self~ be-describer
  self~ be-global-outdoor
  self~ be-global-indoor  ;

: describe-enemy  ( -- )
  battle# @
  if    s" [Enemigo en batalla.]"
  else  s" [Enemigo en paz.]"
  then  paragraph  ;
  \ XXX TMP
  \ XXX TODO -- inconcluso

enemy~ :init  ( -- )
  ['] describe-enemy self~ be-describer
  s" enemigos" self~ mp-name!
  self~ be-human
  self~ be-decoration  ;
  \ XXX TODO -- inconcluso

\ ----------------------------------------------
\ Entes dirección

\ Los entes dirección guardan en su campo `~direction`
\ el desplazamiento correspodiente al campo de
\ dirección que representan
\ Esto sirve para reconocerlos como tales entes dirección
\ (pues todos los valores posibles son diferentes de cero)
\ y para hacer los cálculos en las acciones de movimiento.

north~ :init  ( -- )
  \ ['] describe-north self~ be-describer
  s" norte" self~ ms-name!
  self~ have-definite-article
  north-exit> self~ ~direction !  ;

south~ :init  ( -- )
  \ ['] describe-south self~ be-describer
  s" sur" self~ ms-name!
  self~ have-definite-article
  south-exit> self~ ~direction !  ;

east~ :init  ( -- )
  \ ['] describe-east self~ be-describer
  s" este" self~ ms-name!
  self~ have-definite-article
  east-exit> self~ ~direction !  ;

west~ :init  ( -- )
  \ ['] describe-west self~ be-describer
  s" oeste" self~ name!
  self~ have-definite-article
  west-exit> self~ ~direction !  ;

: describe-up  ( -- )
  am-i-outdoor?
  if  sky~ describe
  else  ceiling~ describe
  then  ;

up~ :init  ( -- )
  ['] describe-up self~ be-describer
  s" arriba" self~ name!
  self~ have-no-article
  up-exit> self~ ~direction !  ;

: describe-down  ( -- )
  am-i-outdoor?
  if    s" [El suelo exterior es muy bonito.]" paragraph
  else  s" [El suelo interior es muy bonito.]" paragraph
  then  ;
  \ XXX TMP
  \ XXX TODO

down~ :init  ( -- )
  ['] describe-down self~ be-describer
  s" abajo" self~ name!
  self~ have-no-article
  down-exit> self~ ~direction !  ;

out~ :init  ( -- )
  \ ['] describe-out self~ be-describer
  s" afuera" self~ name!
  self~ have-no-article
  out-exit> self~ ~direction !  ;

in~ :init  ( -- )
  \ ['] describe-in self~ be-describer
  s" adentro" self~ name!
  self~ have-no-article
  in-exit> self~ ~direction !  ;

\ vim:filetype=gforth:fileencoding=utf-8

