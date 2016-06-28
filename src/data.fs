\ data.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606282022

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Recursos para las descripciones de entes

\ Las palabras de esta sección se usan para
\ construir las descripciones de los entes.
\ Cuando su uso se vuelve más genérico, se mueven
\ a la sección de textos calculados.

\ ----------------------------------------------
\ Albergue de los refugiados

: the-refugees$  ( -- ca len )
  leader% conversations?
  if  s" los refugiados"  else  s" todos"  then  ;

: ^the-refugees$  ( -- ca len )
  the-refugees$ ^uppercase  ;

: they-don't-let-you-pass$  ( -- ca len )
  s{
  s" te" s? (they)-block$ s&
  s" te rodean,"
  s{ s" impidiéndote" s" impidiendo"
  s" obstruyendo" s" obstruyéndote"
  s" bloqueando" s" bloqueándote" }s&
  }s the-pass$ s&  ;
  \ Mensaje de que los refugiados no te dejan pasar.

: the-pass-free$  ( -- ca len )  s" libre" the-pass$ s&  ;
  \ Variante de «libre el paso».

: they-let-you-pass-0$  ( -- ca len )
  s{
  s" te" s? s" han dejado" s&
  s" se han" s{ s" apartado" s" echado a un lado" }s& s" para dejar" s& s" te" s?+
  }s the-pass-free$ s&  ;
  \ Primera versión del mensaje de que te dejan pasar.

: they-let-you-pass-1$  ( -- ca len )
  s" se han" s{ s" apartado" s" retirado" }s&
  s" a" s{ s{ s" los" s" ambos" }s s" lados" s& s" uno y otro lado" }s& s?& comma+
  s{ s" dejándote" s" dejando" s" para dejar" s" te" s?+ }s&
  the-pass-free$ s&  ;
  \ Segunda versión del mensaje de que te dejan pasar.

: they-let-you-pass-2$  ( -- ca len )
  s" ya no" they-don't-let-you-pass$ s& s" como antes" s?&  ;
  \ Tercera versión del mensaje de que te dejan pasar.

: they-let-you-pass$  ( -- ca len )
  ['] they-let-you-pass-0$
  ['] they-let-you-pass-1$
  ['] they-let-you-pass-2$
  3 choose execute  ;
  \ Mensaje de que te dejan pasar.

: the-leader-said-they-want-peace$  ( -- ca len )
  s" que," s" tal y" s?& s" como" s& leader% full-name s&
  s" te" s?& s" ha" s&{ s" referido" s" hecho saber" s" contado" s" explicado" }s& comma+
  they-want-peace$ s&  ;
  \ El líder te dijo qué buscan los refugiados.

: you-don't-know-why-they're-here$  ( -- ca len )
  s{  s" Te preguntas"
      s" No" s{  s" terminas de"
                  s" acabas de"
                  s" aciertas a"
                  s" puedes"
                }s& to-understand$ s&
      s" No" s{ s" entiendes" s" comprendes" }s&
  }s{
    s" qué" s{ s" pueden estar" s" están" }s& s" haciendo" s&
    s" qué" s{ s" los ha" s{ s" podría" s" puede" }s s" haberlos" s& }s&
      s{ s" reunido" s" traído" s" congregado" }s&
    s" cuál es" s{ s" el motivo" s" la razón" }s&
      s" de que se" s&{ s" encuentren" s" hallen" }s&
    s" por qué" s{ s" motivo" s" razón" }s?& s" se encuentran" s&
  }s& here$ s& period+  ;
  \ No sabes por qué están aquí los refugiados.

: some-refugees-look-at-you$  ( -- ca len )
  s" Algunos" s" de ellos" s?&
  s" reparan en" s&{ s" ti" s" tu persona" s" tu presencia" }s&  ;

: in-their-eyes-and-gestures$  ( -- ca len )
  s" En sus" s{ s" ojos" s" miradas" }s&
  s" y" s" en sus" s?& s" gestos" s&? s&  ;
  \ En sus ojos y gestos.

: the-refugees-trust$  ( -- ca len )
  some-refugees-look-at-you$ period+
  in-their-eyes-and-gestures$ s&
  s{ s" ves" s" notas" s" adviertes" s" aprecias" }s&
  s{
    s" amabilidad" s" confianza" s" tranquilidad"
    s" serenidad" s" afabilidad"
  }s&  ;
  \ Los refugiados confían.

: you-feel-they-observe-you$  ( -- ca len )
  s{ s" tienes la sensación de que" s" sientes que" }s?
  s" te observan" s& s" como" s?&
  s{  s" con timidez" s" tímidamente"
      s" de" way$ s& s" subrepticia" s& s" subrepticiamente"
      s" a escondidas"
  }s& period+  ;
  \ Sientes que te observan.

: the-refugees-don't-trust$  ( -- ca len )
  some-refugees-look-at-you$ s{ s" . Entonces" s"  y" }s+
  you-feel-they-observe-you$ s&
  in-their-eyes-and-gestures$ s&
  s{
    s{ s" crees" s" te parece" }s to-realize$ s&
    s{ s" ves" s" notas" s" adviertes" s" aprecias" }s
    s" parece" to-realize$ s& s" se" s+
  }s& s{
    s" cierta" s?{
      s" preocupación" s" desconfianza" s" intranquilidad"
      s" indignación" }s&
    s" cierto" s?{ s" nerviosismo" s" temor" }s&
  }s&  ;
  \ Los refugiados no confían.

: diverse-people$  ( -- ca len )
  s{ s" personas" s" hombres, mujeres y niños" }s
  s" de toda" s& s" edad y" s?& s" condición" s&  ;

: refugees-description  ( -- )
  talked-to-the-leader?
  if    s" Los refugiados son"
  else  s" Hay"
  then  diverse-people$ s&
  talked-to-the-leader?
  if    the-leader-said-they-want-peace$
  else  period+ you-don't-know-why-they're-here$
  then  s&
  do-you-hold-something-forbidden?
  if    the-refugees-don't-trust$
  else  the-refugees-trust$
  then  s& period+ narrate  ;
  \ Descripición de los refugiados.

\ ----------------------------------------------
\ Tramos de cueva (laberinto)

\ Elementos básicos usados en las descripciones

: this-narrow-cave-pass$  ( -- ca len )
  my-location dup is-known? if    not-distant-article
                            else  undefined-article
                            then  narrow-cave-pass$ s&  ;
  \ Devuelve una variante de «estrecho tramo de cueva», con el
  \ artículo adecuado.

: ^this-narrow-cave-pass$  ( -- ca len )
  this-narrow-cave-pass$ ^uppercase  ;
  \ Devuelve una variante de «estrecho tramo de cueva», con el
  \ artículo adecuado y la primera letra mayúscula.

: toward-the(m/f)  ( a -- ca1 len1 )
  has-feminine-name? if  toward-the(f)$  else  toward-the(m)$  then  ;
  \ Devuelve una variante de «hacia el» con el artículo adecuado a un
  \ ente.

: toward-(the)-name  ( a -- ca1 len1 )
  dup has-no-article?
  if    s" hacia"
  else  dup toward-the(m/f)
  then  rot name s&  ;
  \ Devuelve una variante de «hacia el nombre-de-ente» adecuada a un ente.

: main-cave-exits-are$  ( -- ca len )
  ^this-narrow-cave-pass$ lets-you$ s& to-keep-going$ s&  ;
  \ Devuelve una variante del inicio de la descripción de los tramos
  \ de cueva

\ Variantes para la descripción de cada salida

: cave-exit-description-0$  ( -- ca len )
  ^this-narrow-cave-pass$  lets-you$ s& to-keep-going$ s&
  in-that-direction$ s&  ;
  \ Devuelve la primera variante de la descripción de una salida de un
  \ tramo de cueva.

: cave-exit-description-1$ ( -- ca1 len1 )
  ^a-pass-way$
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  in-that-direction$ s&  ;
  \ Devuelve la segunda variante de la descripción de una salida de un
  \ tramo de cueva.

\ Variantes para la descripción principal

false [if]  \ XXX OLD -- código obsoleto

: $two-main-exits-in-cave ( ca1 len1 ca2 len2 -- ca3 len3 )
  2>r 2>r
  this-narrow-cave-pass$ lets-you$ s& to-keep-going$ s&
  toward-the(m)$ 2dup 2r> s&  \ Una dirección
  2swap 2r> s&  \ La otra dirección
  both&  ;
  \ Devuelve la descripción _ca3 len3_ de un tramo de cueva con dos salidas a dos
  \ puntos cardinales, cuyos nombres (sin artículo) son _ca1 len1_ y
  \ _ca2 len2_.  Esta palabra solo sirve para parámetros de puntos
  \ cardinales (todos usan artículo determinado masculino). Se usa en
  \ la descripción principal de un escenario
  \
  \ XXX TODO -- no usado

: $other-exit-in-cave  ( ca1 len1 -- ca2 len2 )
  ^a-pass-way$ s&
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  toward-the(m)$ s& 2swap s&  ;
  \ Devuelve la descripción _ca2 len2_ de una salida adicional en un
  \ tramo de cueva, siendo _ca1 len1_ el nombre de la dirección
  \ cardinal.  Se usa en la descripción principal de un escenario Esta
  \ palabra solo sirve para parámetros de puntos cardinales (todos
  \ usan artículo determinado masculino)
  \
  \ XXX TODO -- no usado

[then]  \ XXX NOTE: fin del código obsoleto

: cave-exit-separator+  ( ca1 len1 -- ca2 len2 )
  s{ s" ," s" ;" s" ..." }s+
  s{ s" y" 2dup s" aunque" but$ }s& s" también" s?&  ;
  \ Concatena (sin separación) a una cadena el separador entre las
  \ salidas principales y las secundarias.

: (paths)-can-be-seen-0$  ( -- ca len )
  s{ s" parten" s" surgen" s" nacen" s" salen" }s
  s" de" s{ s" aquí" s" este lugar" }s& s? rnd2swap s&  ;

: (paths)-can-be-seen-1$  ( -- ca len )
  s{ s" se ven" s" pueden verse"
  s" se vislumbran" s" pueden vislumbrarse"
  s" se adivinan" s" pueden adivinarse"
  s" se intuyen" s" pueden intuirse" }s  ;

: (paths)-can-be-seen$  ( -- ca len )
  ['] (paths)-can-be-seen-0$
  ['] (paths)-can-be-seen-1$
  2 choose execute  ;
  \ XXX TODO -- hacer que el texto dependa, por grupos, de si el
  \ escenario es conocido

: paths-seen  ( ca1 len1 -- ca2 len2 )
  pass-ways$ s& s" más" s?&
  (paths)-can-be-seen$ rnd2swap s&  ;
  \ Devuelve la presentación de la lista de salidas secundarias.
  \ ca1 len1 = Cadena con el número de pasajes
  \ ca2 len2 = Cadena con el resultado

: secondary-exit-in-cave&  ( a1 ca2 len2 -- ca3 len3 )
  rot toward-(the)-name s&  ;
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ a1 = Ente dirección cuya descripción hay que añadir
  \ ca2 len2 = Descripción en curso

: one-secondary-exit-in-cave  ( a -- ca len )
  a-pass-way$
  s{ s" parte" s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  secondary-exit-in-cave&  ;
  \ Devuelve la descripción _ca len_ de una salida adicional en un
  \ tramo de cueva, del ente dirección _a_.

: two-secondary-exits-in-cave  ( a1 a2 -- ca3 len3 )
  s" dos" paths-seen s" :" s?+
  secondary-exit-in-cave& s" y" s&
  secondary-exit-in-cave&  ;
  \ Devuelve la descripción de dos salidas adicionales en un tramo de
  \ cueva.

: three-secondary-exits-in-cave  ( a1 a2 a3 -- ca4 len4 )
  s" tres" paths-seen s" :" s?+
  secondary-exit-in-cave& comma+
  secondary-exit-in-cave& s" y" s&
  secondary-exit-in-cave&  ;
  \ Devuelve la descripción de tres salidas adicionales en un tramo de
  \ cueva.

: two-main-exits-in-cave ( a1 a2 -- ca len )
  toward-(the)-name rot toward-(the)-name both  ;
  \ Devuelve la descripción _ca len_ de dos salidas principales en un
  \ tramo de cueva, de dos entes dirección _a1_ y _a2_.

: one-main-exit-in-cave  ( a -- ca len )  toward-(the)-name  ;
  \ Devuelve la descripción _ca len_ de una salida principal en un
  \ tramo de cueva, de un nte dirección _a_.

\ Descripciones de los tramos de cueva según el reparto entre salidas
\ principales y secundarias

: 1+1-cave-exits  ( a1 a2 -- ca len )
  one-main-exit-in-cave cave-exit-separator+
  rot one-secondary-exit-in-cave s&  ;
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y una secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección

: 1+2-cave-exits  ( a1 a2 a3 -- ca len )
  one-main-exit-in-cave cave-exit-separator+
  2swap two-secondary-exits-in-cave s&  ;
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y dos secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección

: 1+3-cave-exits  ( a1 a2 a3 a4 -- ca len )
  one-main-exit-in-cave cave-exit-separator+
  2>r three-secondary-exits-in-cave 2r> 2swap s&  ;
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y tres secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección

: 2+0-cave-exits  ( a1 a2 -- ca len )  two-main-exits-in-cave  ;
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y ninguna secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección

: 2+1-cave-exits  ( a1 a2 a3 -- ca len )
  two-main-exits-in-cave cave-exit-separator+
  rot one-secondary-exit-in-cave s&  ;
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y ninguna secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección

: 2+2-cave-exits  ( a1 a2 a3 a4 -- ca len )
  two-main-exits-in-cave cave-exit-separator+
  2swap two-secondary-exits-in-cave s&  ;
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y dos secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección

\ Descripciones de los tramos de cueva según su número de salidas

: 1-exit-cave-description   ( a1 -- ca len )  toward-(the)-name  ;
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene una salida.
  \ a1 = Ente dirección

: 2-exit-cave-description   ( a1 a2 -- ca len )
  ['] 2+0-cave-exits
  ['] 1+1-cave-exits
  2 choose execute  ;
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene dos salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección

: 3-exit-cave-description   ( a1 a2 a3 -- ca len )
  ['] 2+1-cave-exits
  ['] 1+2-cave-exits
  2 choose execute  ;
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene tres salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección

: 4-exit-cave-description   ( a1 a2 a3 a4 -- ca len )
  ['] 2+2-cave-exits
  ['] 1+3-cave-exits
  2 choose execute  ;
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene cuatro salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección

create 'cave-descriptions
  \ Tabla para contener las direcciones de las palabras de
  \ descripción.
  ' 1-exit-cave-description ,
  ' 2-exit-cave-description ,
  ' 3-exit-cave-description ,
  ' 4-exit-cave-description ,

\ Interfaz para usar en las descripciones de los escenarios:
\ `exits-cave-description` para la descripción principal
\ `cave-exit-description$` para la descripción de cada salida

: unsort-cave-exits  ( a1 ... an u -- a1'..an' u )  dup >r unsort r>  ;
  \ Desordena los entes dirección que son las salidas de la cueva.
  \ u = Número de elementos de la pila que hay que desordenar

: (exits-cave-description)  ( a1 ... an u -- ca2 len2 )
  1- cells 'cave-descriptions + perform  ;
  \ Ejecuta (según el número de salidas) la palabra  que devuelve la
  \ descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados

: exits-cave-description  ( a1 ... an u -- ca2 len2 )
  unsort-cave-exits  (exits-cave-description) period+
  main-cave-exits-are$ 2swap s&  ;  \ Añadir el encabezado
  \ Devuelve la descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados

: cave-exit-description$  ( -- ca1 len1 )
  ['] cave-exit-description-0$  \ Primera variante posible
  ['] cave-exit-description-1$  \ Segunda variante posible
  2 choose execute period+  ;
  \ Devuelve la descripción de una dirección de salida de un tramo de cueva.

\ ----------------------------------------------
\ La aldea sajona

: poor$  ( -- ca len )
  s{ s" desgraciada" s" desdichada" }s  ;

: poor-village$  ( -- ca len )
  poor$ s" aldea" s&  ;

: rests-of-the-village$  ( -- ca len )
  s" los restos" still$ s? s" humeantes" s& s?&
  s" de la" s& poor-village$ s&  ;
  \ Devuelve parte de otras descripciones de la aldea arrasada.

\ ----------------------------------------------
\ Pared del desfiladero

: to-pass-(wall)$  ( -- ca len )
  s{ s" superar" s" atravesar" s" franquear" s" vencer" s" escalar" }s  ;
  \ Infinitivo aplicable para atravesar una pared o un muro.

: it-looks-impassable$  ( -- ca len )
  s{ s" por su aspecto" s" a primera vista" }s?
  s{  s" parece"
      s" diríase que es"
      s" bien" s? s" puede decirse que es" s&
  }s&
  s{  s" imposible de" to-pass-(wall)$ s&
      s" imposible" to-pass-(wall)$ s& s" la" s+
      s{ s" insuperable" s" invencible" s" infranqueable" }s
  }s&  ;
  \ Mensaje «parece infranqueable».
  \ XXX TODO -- ojo: género femenino; generalizar factorizando cuando se use en otro contexto

: the-cave-entrance-is-visible$  ( -- ca len )
  s{  s" se" s{ s" ve" s" abre" s" haya"
                s" encuentra" s" aprecia" s" distingue" }s&
      s" puede" s{ s" verse" s" apreciarse" s" distinguirse" }s&
  }s cave-entrance% full-name s&  ;

\ ----------------------------------------------
\ Entrada a la cueva

: the-cave-entrance-was-discovered?  ( -- )
  location-08% has-south-exit?  ;
  \ ¿La entrada a la cueva ya fue descubierta?

: the-cave-entrance-is-accessible?  ( -- )
  location-08% am-i-there? the-cave-entrance-was-discovered? and  ;
  \ ¿La entrada a la cueva está accesible (presente y descubierta)?

: open-the-cave-entrance  ( -- )
  location-08% dup location-10% s<-->  location-10% i<-->  ;
  \ Comunica el escenario 8 con el 10 (de dos formas y en ambos sentidos).

: you-discover-the-cave-entrance$  ( -- ca len )
  ^but$ comma+
  s{  s" reconociendo" s" el terreno" more-carefully$ rnd2swap s& s&
      s" fijándote" s" ahora" s?& more-carefully$ s&
  }s& comma+ s" descubres" s& s{ s" lo" s" algo" }s& s" que" s&
  s" sin duda" s?& s{ s" parece ser" s" es" s" debe de ser" }s&
  s{ s" la entrada" s" el acceso" }s& s" a una" s& cave$ s&  ;
  \ Mensaje de que descubres la cueva.

: you-discover-the-cave-entrance  ( -- )
  you-discover-the-cave-entrance$ period+ narrate
  open-the-cave-entrance  cave-entrance% be-here  ;
  \ Descubres la cueva.

: you-maybe-discover-the-cave-entrance  ( ca len -- )
  s" ..." s+ narrate
  2 random 0= if  narration-break you-discover-the-cave-entrance  then  ;
  \ Descubres la cueva con un 50% de probabilidad.
  \ ca len = Texto introductorio

: the-cave-entrance-is-hidden$  ( -- ca len )
  s" La entrada" s" a la cueva" s?&
  s{ s" está" s" muy" s?& s" no podría estar más" }s&
  s{ s" oculta" s" camuflada" s" escondida" }s&
  s" en la pared" s& rocky(f)$ s& period+  ;

: you-were-lucky-discovering-it$ ( -- ca len )
  s" Has tenido" s" muy" s?& s" buena" s&{ s" fortuna" s" suerte" }s&
  s{  s{ s" al" s" en" s" con" }s
        s{ s" hallarla" s" encontrarla" s" dar con ella" s" descubrirla" }s&
      s{  s" hallándola" s" encontrándola"
          s" dando con ella" s" descubriéndola"
      }s
  }s& period+  ;

: it's-your-last-hope$  ( -- ca len )
  s{ s" te das cuenta de que" s" sabes que" }s?
  s{ s" es" s" se trata de" }s& ^uppercase
  your|the(f)$ s& s{ s" única" s" última" }s&
  s{ s" salida" s" opción" s" esperanza" s" posibilidad" }s&
  s{ s" de" s" para" }s
  s{  s" escapar" s" huir"
      s" evitar" s{ s" ser capturado" s" que te capturen" }s&
  }s&? s& period+  ;

\ ----------------------------------------------
\ Entrada a la cueva

\ XXX TODO

\ ----------------------------------------------
\ Otros escenarios

: bridge-that-way$  ( -- ca len )
  s" El puente" leads$ s& that-way$ s& period+  ;

: stairway-that-way$  ( -- ca len )
  s" Las escaleras" (they)-lead$ s& that-way$ s& period+  ;

: comes-from-there$  ( -- ca len )
  comes-from$ from-that-way$ s&  ;

: water-from-there$  ( -- ca len )
  the-water-current$ comes-from-there$ s&  ;

: ^water-from-there$  ( -- ca len )
  ^the-water-current$ comes-from-there$ s&  ;

: water-that-way$  ( -- ca len )
  ^the-water-current$ s{ s" corre" s" fluye" s" va" }s&
  in-that-direction$ s& period+  ;

: stairway-to-river$  ( -- ca len )
  s" Las escaleras" (they)-go-down$ s&
  that-way$ s& comma+
  s{ s" casi" s? s" hasta el" s& s" mismo" s?& s" borde del" s&
  s" casi" s? s" hasta la" s& s" misma" s?& s" orilla del" s&
  s" casi" s? s" hasta el" s&
  s" hasta" s? s" cerca del" s& }s& s" agua." s&  ;

: a-high-narrow-pass-way$  ( -- ca len )
  s" un" narrow(m)$ s& pass-way$ s& s" elevado" s&  ;

\ ==============================================================
\ Atributos y descripciones de entes

\ ----------------------------------------------
\ Ente protagonista

\ cr .( antes de ulfius) .s \ XXX INFORMER
ulfius% :attributes
  s" Ulfius" self% ms-name!
  self% be-human
  self% has-personal-name
  self% has-no-article
  \ location-01% self% be-there
  ;attributes

ulfius% :description
  \ XXX TMP
  s" [descripción de Ulfius]"
  paragraph
  ;description

\ ----------------------------------------------
\ Entes personaje

ambrosio% :attributes
  s" hombre" self% ms-name!  \ El nombre cambiará a «Ambrosio» durante el juego
  self% be-character
  self% be-human
  location-19% self% be-there
  ;attributes

ambrosio% :description
  self% conversations if
    s" Ambrosio"
    s" es un hombre de mediana edad, que te mira afable." s&
  else  s" Es de mediana edad y mirada afable."
  then  paragraph
  ;description

leader% :attributes
  s" anciano" self% ms-name!
  self% be-character
  self% be-human
  self% be-not-listed
  location-28% self% be-there
  ;attributes

leader% :description
  \ XXX TODO -- elaborar esto según la trama
  leader% conversations?
  if
    s" Es el jefe de los refugiados."
  else
    s" Es un anciano."
  then
  paragraph
  ;description

soldiers% :attributes
  s" soldados" self% mp-name!
  self% be-human
  self% familiar++
  self% be-decoration
  \ self% has-definite-article  \ XXX TODO -- mejor implementar que tenga posesivo...
  self% belongs-to-protagonist  \ XXX TODO -- ...aunque quizá esto baste
  ;attributes

defer soldiers-description  \ Vector a la futura descripción
soldiers% :description
  \ La descripción de los soldados
  \ necesita usar palabras que aún no están definidas,
  \ y por ello es mejor crearla después.
  soldiers-description
  ;description

officers% :attributes
  s" oficiales" self% mp-name!
  self% be-human
  self% familiar++
  self% be-decoration
  \ self% has-definite-article  \ XXX TODO -- mejor implementar que tenga posesivo...
  self% belongs-to-protagonist  \ XXX TODO -- ...aunque quizá esto baste
  ;attributes

defer officers-description  \ Vector a la futura descripción
officers% :description
  \ La descripción de los oficiales
  \ necesita usar palabras que aún no están definidas,
  \ y por ello es mejor crearla después.
  officers-description
  ;description

refugees% :attributes
  s" refugiados" self% mp-name!
  self% be-human
  self% be-decoration
  ;attributes

refugees% :description
  my-location case
  location-28% of  refugees-description  endof
  location-29% of
    \ XXX TODO -- provisional
    s" Todos los refugiados quedaron atrás." paragraph
    endof
  endcase
  ;description

\ ----------------------------------------------
\ Entes objeto

altar% :attributes
  s" altar" self% ms-name!
  self% be-decoration
  impossible-error# self% ~take-error# !
  location-18% self% be-there
  ;attributes

altar% :description
  s" Está" s{ s" situado" s" colocado" }s&
  s" justo en la mitad del puente." s&
  idol% is-unknown? if
    s" Debe de sostener algo importante." s&
  then
  paragraph
  ;description

arch% :attributes
  s" arco" self% ms-name!
  self% be-decoration
  location-18% self% be-there
  ;attributes

arch% :description
  \ XXX TMP
  s" Un sólido arco de piedra, de una sola pieza."
  paragraph
  ;description

bed% :attributes
  s" catre" self% ms-name!
  location-46% self% be-there
  self% ambrosio% belongs
  ;attributes

bed% :description
  s{ s" Parece poco" s" No tiene el aspecto de ser muy"
  s" No parece especialmente" }s
  s{ s" confortable" s" cómod" self% adjective-ending s+ }s& period+
  paragraph
  ;description

bridge% :attributes
  s" puente" self% ms-name!
  self% be-decoration
  location-13% self% be-there
  ;attributes

bridge% :description
  \ XXX TMP
  s" Está semipodrido."
  paragraph
  ;description

candles% :attributes
  s" velas" self% fp-name!
  location-46% self% be-there
  self% ambrosio% belongs
  ;attributes

candles% :description
  s" Están muy consumidas."
  paragraph
  ;description

cave-entrance% :attributes
  s" entrada a una cueva" self% fs-name!
  ;attributes

cave-entrance% :description
  the-cave-entrance-is-hidden$
  you-were-lucky-discovering-it$ s&
  it's-your-last-hope$ s&
  paragraph
  ;description

cloak% :attributes
  s" capa" self% fs-name!
  self% be-cloth
  self% belongs-to-protagonist
  self% be-worn
  self% taken
  ;attributes

cloak% :description
  s" Tu capa de general, de fina lana"
  s{ s" tintada de negro." s" negra." }s&
  paragraph
  ;description

cuirasse% :attributes
  s" coraza" self% fs-name!
  self% be-cloth
  self% belongs-to-protagonist
  self% be-worn
  self% taken
  ;attributes

door% :attributes
  s" puerta" self% fs-name!
  self% be-closed
  impossible-error# self% ~take-error# !
  location-47% self% be-there
  self% ambrosio% belongs
  ;attributes

door% :description
  self% times-open if  s" Es"  else  s" Parece"  then
  s" muy" s?& s{ s" recia" s" gruesa" s" fuerte" }s&
  location-47% am-i-there? if
    lock% is-known?
    if    s" . A ella está unido el candado"
    else  s"  y tiene un gran candado"
    then  s+ lock-found
  then  period+
  s" Está" s& door% «open»|«closed» s& period+ paragraph
  ;description

emerald% :attributes
  s" esmeralda" self% fs-name!
  location-39% self% be-there
  ;attributes

emerald% :description
  s" Es preciosa."
  paragraph
  ;description

fallen-away% :attributes
  s" derrumbe" self% ms-name!
  self% be-decoration
  nonsense-error# self% ~take-error# !
  location-09% self% be-there
  ;attributes

fallen-away% :description
  s{
    s" Muchas," s" Muchísimas," s" Numerosas,"
    s" Un gran número de" s" Una gran cantidad de"
    \ XXX TODO -- si se añade lo que sigue, hay que crear los entes "pared" y "muro":
    \ s" Un muro de" s" Una pared de"
  }s
  s{ s" inalcanzables" s" inaccesibles" }s&
  s{ s" y enormes" s" y pesadas" s" y grandes" }s?&
  s" rocas," s& s{ s" apiladas" s" amontonadas" }s&
  s{
    s" una sobre otra"
    s" unas sobre otras"
    s" una encima de otra"
    s" unas encima de otras"
    s" la una encima de la otra"
    s" las unas encima de las otras"
    s" la una sobre la otra"
    s" las unas sobre las otras"
  }s& period+
  paragraph
  ;description


: don't-take-the-flags  ( -- )
  s" [Yo no lo haría]." narrate  ;
  \ XXX TODO

flags% :attributes
  s" banderas" self% fp-name!
  self% be-decoration
  ['] don't-take-the-flags self% ~take-error# !
  location-28% self% be-there
  ;attributes

flags% :description
  s" Son las banderas britana y sajona."
  s" Dos dragones rampantes, rojo y blanco respectivamente, enfrentados." s&
  paragraph
  ;description

flint% :attributes
  s" pedernal" self% ms-name!
  ;attributes

flint% :description
  s" Es una piedra dura y afilada."
  paragraph
  ;description

grass% :attributes
  s" hierba" self% fs-name!
  self% be-decoration
  ;attributes

grass% :description
  door% times-open if
    s" Está" self% verb-number-ending+
    s" aplastad" self% adjective-ending+ s&
    s{ s" en el" s" bajo el" s" a lo largo del" }s&
    s{ s" trazado" s" recorrido" }s&
    s{ s" de la puerta." s" que hizo la puerta al abrirse." }s&
  else
    s" Cubre" self% verb-number-ending+
    s" el suelo junto a la puerta, lo" s&
    s{ s" que" s" cual" }s&
    s{ s" indica" s" significa" s" delata" }s
    s" que ésta" s&
    s{ s" no ha sido abierta en" s" lleva cerrada"
    s" ha permanecido cerrada" s" durante" s?& }s
    s" mucho tiempo." s&
  then  paragraph
  ;description

idol% :attributes
  s" ídolo" self% ms-name!
  self% be-decoration
  impossible-error# self% ~take-error# !
  location-41% self% be-there
  ;attributes

idol% :description
  s" El ídolo tiene dos agujeros por ojos."
  paragraph
  ;description

key% :attributes
  s" llave" self% fs-name!
  location-46% self% be-there
  self% ambrosio% belongs
  ;attributes

key% :description
  \ XXX TODO -- crear ente. hierro, herrumbre y óxido, visibles con la llave en la mano
  s" Grande, de hierro herrumboso."
  paragraph
  ;description

lake% :attributes
  s" lago" self% ms-name!
  self% be-decoration
  nonsense-error# self% ~take-error# !
  location-44% self% be-there
  ;attributes

lake% :description
  s{ s" La" s" Un rayo de" }s
  s" luz entra por un resquicio, y sus caprichosos reflejos te maravillan." s&
  paragraph
  ;description

lock% :attributes
  s" candado" self% ms-name!
  self% be-decoration
  self% be-closed
  impossible-error# self% ~take-error# !
  self% ambrosio% belongs
  ;attributes

lock% :description
  s" Es grande y parece" s{ s" fuerte." s" resistente." }s&
  s" Está" s&{ s" fijad" s" unid" }s& self% adjective-ending+
  s" a la puerta y" s&
  lock% «open»|«closed» s& period+
  paragraph
  ;description

log% :attributes
  s" tronco" self% ms-name!
  location-15% self% be-there
  ;attributes

log% :description
  s" Es un tronco"
  s{ s" recio," s" resistente," s" fuerte," }s&
  but$ s& s{ s" liviano." s" ligero." }s&
  paragraph
  ;description

piece% :attributes
  s" trozo de tela" self% ms-name!
  \ XXX NOTE: ojo con este «de tela»: «tela» es sinónimo de trozo;
  \ hay que contemplar estos casos en el cálculo de los genitivos.
  ;attributes

piece% :description
  s" Un pequeño" s{ s" retal" s" pedazo" s" trozo" s" resto" }s&
  of-your-ex-cloak$ s&
  paragraph
  ;description

rags% :attributes
  s" harapo" self% ms-name!
  ;attributes

rags% :description
  s" Un" s{ s" retal" s" pedazo" s" trozo" }s&
  s{ s" un poco" s" algo" }s?& s" grande" s&
  of-your-ex-cloak$ s&
  paragraph
  ;description

ravine-wall% :attributes
  s" pared" rocky(f)$ s& self% fs-name!
  location-08% self% be-there
  self% be-not-listed  \ XXX OLD -- innecesario
  self% be-decoration
  ;attributes

ravine-wall% :description
  s" en" the-cave-entrance-was-discovered? ?keep
  s" la pared" s& rocky(f)$ s& ^uppercase
  the-cave-entrance-was-discovered? if
    s" , que" it-looks-impassable$ s& comma+ s?+
    the-cave-entrance-is-visible$ s&
    period+ paragraph
  else
    it-looks-impassable$ s&
    ravine-wall% is-known?
    if    you-maybe-discover-the-cave-entrance
    else  period+ paragraph
    then
  then
  ;description

rocks% :attributes
  s" rocas" self% fp-name!
  self% be-decoration
  location-31% self% be-there
  ;attributes

rocks% :description
  location-31% has-north-exit?
  if  (rocks)-on-the-floor$ ^uppercase
  else  (rocks)-clue$
  then  period+ paragraph
  ;description

snake% :attributes
  s" serpiente" self% fs-name!
  self% be-animal
  dangerous-error# self% ~take-error# !
  location-43% self% be-there
  ;attributes

snake% :description
  \ XXX TODO -- distinguir si está muerta
  \ XXX NOTE: en el programa original no hace falta
  s" Una serpiente muy maja."
  paragraph
  ;description

stone% :attributes
  s" piedra" self% fs-name!
  location-18% self% be-there
  ;attributes

stone% :description
  s" Recia y pesada, pero no muy grande, de forma piramidal."
  paragraph
  ;description

sword% :attributes
  s" espada" self% fs-name!
  self% belongs-to-protagonist
  self% taken
  ;attributes

sword% :description
  s{ s" Legado" s" Herencia" }s s" de tu padre," s&
  s{ s" fiel herramienta" s" arma fiel" }s& s" en" s&
  s{ s" mil" s" incontables" s" innumerables" }s&
  s" batallas." s&
  paragraph
  ;description

table% :attributes
  s" mesa" self% fs-name!
  location-46% self% be-there
  self% ambrosio% belongs
  ;attributes

table% :description
  s" Es pequeña y de" s{ s" basta" s" tosca" }s& s" madera." s&
  paragraph
  ;description

thread% :attributes
  s" hilo" self% ms-name!
  ;attributes

thread% :description
  \ XXX TODO -- mover esto al evento de cortar la capa
  \ s" Un hilo se ha desprendido al cortar la capa con la espada."
  s" Un hilo" of-your-ex-cloak$ s&
  paragraph
  ;description

torch% :attributes
  s" antorcha" self% fs-name!
  self% be-light
  self% be-not-lit
  ;attributes

torch% :description
  s" Está" self% is-lit?
  if  s" encendida."  else  s" apagada."  then  s& paragraph
  ;description

waterfall% :attributes
  s" cascada" self% fs-name!
  self% be-decoration
  nonsense-error# self% ~take-error# !
  location-38% self% be-there
  ;attributes

waterfall% :description
  s" No ves nada por la cortina de agua."
  s" El lago es muy poco profundo." s&
  paragraph
  ;description

\ ----------------------------------------------
\ Entes escenario

\ Las palabras que describen entes escenario reciben en `sight`
\ (variable que está creada con `value` y por tanto devuelve su
\ valor como si fuera una constante) un identificador de ente.
\ Puede ser el mismo ente escenario o un ente de dirección.  Esto
\ permite describir lo que hay más allá de cada escenario en
\ cualquier dirección.

location-01% :attributes
  s" aldea sajona" self% fs-name!
  0 location-02% 0 0 0 0 0 0 self% init-location
  ;attributes

location-01% :description
  \ XXX TODO -- crear colina en los tres escenarios
  sight case
  self% of
    s" No ha quedado nada en pie, ni piedra sobre piedra."
    s{ s" El entorno es desolador." s" Todo alrededor es desolación." }s
    rnd2swap s&
    s{ ^only$ remains$ s&
    s" Lo único que" remains$ s& s" por hacer" s?& s" es" s&
    s" No" remains$ s& s{ s" más" s" otra cosa" }s& s" que" s&
    }s& to-go-back$ s& s" al sur, a casa." s&
    paragraph
    endof
  south% of
    2 random if \ Versión 0:
      ^toward-the(m)$ s" sur" s&
      s{ s" está" s" puedo ver" s" se puede ver" }s&
      s" la colina." s&  \ Descripción principal
      s" Y mucho más allá está tu" home$ s& period+  \ Coletilla...
      2 random * s&  \ ...que aparecerá con un 50% de probabilidad
    else  \ Versión 1:
      s" Muy lejos de aquí está tu" home$ s& comma+
      s" y el camino empieza detrás de aquella colina." s&
    then  paragraph
    endof
  up% of
    s{ s" pronto" s" sin compasión" s" de inmediato" }s
    s{ s" vencidas" s" derrotadas" s" sojuzgadas" }s rnd2swap s& ^uppercase
    s" por la fría" s&
    s{ s" e implacable" s" y despiadada" }s?&
    s" niebla," s& s" torpes" s" tristes" both?&
    s" columnas de" s& s" negro" s" humo" rnd2swap s& s&
    (they)-go-up$ s&
    s{ s" lastimosamente" s" penosamente" }s&
    s" hacia" s{ s" el cielo" s" las alturas" }s& s?&
    s{ s" desde" s" de entre" }s& rests-of-the-village$ s&
    s" , como si" s" también" s" ellas" rnd2swap s& s?&
    s{ s" desearan" s" anhelaran" s" soñaran" }s&
    s" poder hacer un último esfuerzo por" s?&
    s" escapar" s& but|and$ s& s" no supieran cómo" s& s?+
    s" ..." s+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-02% :attributes
  s" cima de la colina" self% fs-name!
  location-01% 0 0 location-03% 0 0 0 0 self% init-location
  ;attributes

location-02% :description
  \ XXX TODO -- crear ente. aldea, niebla
  sight case
  self% of
    s" Sobre" s" la cima de" s?&
    s" la colina, casi" s& s{ s" sobre" s" por encima de" }s&
    s" la" s&
    s" espesa" s" fría" both?& s" niebla de la aldea sajona arrasada al norte, a tus pies." s&
    ^the-path$ s& goes-down$ s& toward-the(m)$ s& s" oeste." s&
    paragraph
    endof
  north% of
    s" La" poor-village$ s& s" sajona" s& s" , arrasada," s?+ s" agoniza bajo la" s&
    s" espesa" s" fría" both?& s" niebla." s&
    paragraph
    endof
  west% of
    ^the-path$ goes-down$ s& s" por la" s& s" ladera de la" s?& s" colina." s&
    paragraph
    endof
  down% of
    \ Bajar la colina
    \ puede equivaler a bajar por el sur o por el oeste; esto
    \ se decide al azar cada vez que se
    \ entra en el escenario, por lo que su descripción
    \ debe tenerlo en cuenta y redirigir a la descripción adecuada:
    self% down-exit self% north-exit =
    if  north%  else  west%  then  describe
    endof
  uninteresting-direction
  endcase
  ;description

location-03% :attributes
  s" camino entre colinas" self% ms-name!
  0 0 location-02% location-04% 0 0 0 0 self% init-location
  ;attributes

location-03% :description
  sight case
  self% of
    ^the-path$ s" avanza por el valle," s&
    s" desde la parte alta, al este," s&
    s" a una zona" s& very-or-null$ s& s" boscosa, al oeste." s&
    paragraph
    endof
  east% of
    ^the-path$ s" se pierde en la parte alta del valle." s&
    paragraph
    endof
  west% of
    s" Una zona" very-or-null$ s& s" boscosa." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-04% :attributes
  s" cruce de caminos" self% ms-name!
  location-05% 0 location-03% location-09% 0 0 0 0 self% init-location
  ;attributes

location-04% :description
  sight case
  self% of
    s" Una senda parte al oeste, a la sierra por el paso del Perro,"
    s" y otra hacia el norte, por un frondoso bosque que la rodea." s&
    paragraph
    endof
  north% of
    ^a-path$ surrounds$ s& s" la sierra a través de un frondoso bosque." s&
    paragraph
    endof
  west% of
    ^a-path$ leads$ s& toward-the(f)$ s& s" sierra por el paso del Perro." s&
    paragraph
    endof
  down% of  endof
  up% of  endof
  uninteresting-direction
  endcase
  ;description

location-05% :attributes
  s" linde del bosque" self% fs-name!
  0 location-04% 0 location-06% 0 0 0 0 self% init-location
  ;attributes

location-05% :description
  sight case
  self% of
    ^toward-the(m)$ s" oeste se extiende" s&
    s{ s" frondoso" s" exhuberante" }s& \ XXX TODO -- independizar
    s" el bosque que rodea la sierra." s&
    s" La salida se abre" s&
    toward-the(m)$ s& s" sur." s&
    paragraph
    endof
  south% of
    s" Se ve la salida del bosque."
    paragraph
    endof
  west% of
    s" El bosque se extiende"
    s{ s" exhuberante" s" frondoso" }s&
    s" alrededor de la sierra." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-06% :attributes
  s" bosque" self% ms-name!
  0 0 location-05% location-07% 0 0 0 0 self% init-location
  ;attributes

location-06% :description
  sight case
  self% of
    s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
    ^the-path$ s& s" serpentea entre raíces, de un luminoso este" s&
    toward-the(m)$ s& s" oeste." s&
    paragraph
    endof
  east% of
    s" De la linde del bosque"
    s{ s" procede" s" llega" s" viene" }s&
    s{ s" una cierta" s" algo de" s" un poco de" }s&
    s{ s" claridad" s" luminosidad" }s&
    s" entre" s&
    s{ s" el follaje" s" la vegetación" }s& period+
    paragraph
    endof
  west% of
    s" La niebla parece más" s" densa" s" oscura" both?& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-07% :attributes
  s" paso del Perro" self% ms-name!
  0 location-08% location-06% 0 0 0 0 0 self% init-location
  ;attributes

location-07% :description
  sight case
  self% of
    s" Abruptamente, el bosque desaparece y deja paso a un estrecho camino entre altas rocas."
    s" El" s& s{ s" inquietante" s" sobrecogedor" }s&
    s" desfiladero" s& s{ s" tuerce" s" gira" }s&
    s" de este a sur." s&
    paragraph
    endof
  south% of
    ^the-path$ s" gira" s& in-that-direction$ s& period+
    paragraph
    endof
  east% of
    s" La estrecha senda es" s{ s" engullida" s" tragada" }s&
    s" por las" s&
    s" fauces" s{ s" frondosas" s" exhuberantes" }s rnd2swap s& s&
    s" del bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-08% :attributes
  s" desfiladero" self% ms-name!
  location-07% 0 0 0 0 0 0 0 self% init-location
  ;attributes

location-08% :description
  \ XXX TODO -- crear pared y roca y desfiladero
  sight case
  self% of
    ^the-pass-way$ s" entre el desfiladero sigue de norte a este" s&
    s" junto a una" s&
    s{  s" pared" rocky(f)$ s& s" rocosa pared" }s& period+
    \ XXX TODO -- completar con entrada a caverna, tras ser descubierta
    paragraph
    endof
  north% of
    s" El camino" s{ s" tuerce" s" gira" }s& \ XXX TODO -- independizar gira/tuerce
    s" hacia el inquietante paso del Perro." s&
    paragraph
    endof
  south% of
    s{ ^in-that-direction$ s" Hacia el sur" }s
    s{ s" se alza" s" se levanta" }s&
    \ s" una pared" s& rocky(f)$ s& \ XXX OLD -- antiguo
    ravine-wall% full-name s&
    the-cave-entrance-was-discovered? if
      comma+ s" en la" s&{ s" que" s" cual" }s&
      the-cave-entrance-is-visible$ s&
      period+ paragraph
    else
      ravine-wall% is-known? if
        s" que" it-looks-impassable$ s& s?&
        you-maybe-discover-the-cave-entrance
      else
        period+ paragraph  ravine-wall% familiar++
      then
    then
    endof
  uninteresting-direction
  endcase
  ;description

location-09% :attributes
  s" derrumbe" self% ms-name!
  0 0 location-04% 0 0 0 0 0 self% init-location
  ;attributes

location-09% :description
  sight case
  self% of
    ^the-path$ goes-down$ s& s" hacia la agreste sierra, al oeste," s&
    s" desde los" s& s" verdes" s" valles" rnd2swap s& s& s" al este." s&
    ^but$ s& s" un" s&{ s" gran" s" enorme" }s?& s" derrumbe" s&
    (it)-blocks$ s& s" el paso hacia" s&{ s" el oeste" s" la sierra." }s&
    paragraph
    endof
  east% of
    ^can-see$ s" la salida del bosque." s&
    paragraph
    endof
  west% of
    s" Un gran derrumbe" (it)-blocks$ s& the-pass$ s&
    toward$ s& s" la sierra." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-10% :attributes
  s" gruta de entrada" self% fs-name!
  self% be-indoor-location
  location-08% 0 0 location-11% 0 0 0 0 self% init-location
  ;attributes

location-10% :description
  sight case
  self% of
    s" El estrecho paso se adentra hacia el oeste, desde la boca, al norte."
    paragraph
    endof
  north% of
    s" La boca de la gruta conduce al exterior."
    paragraph
    endof
  east% of
  endof
  uninteresting-direction
  endcase
  ;description

location-11% :attributes
  s" gran lago" self% ms-name!
  self% be-indoor-location
  0 0 location-10% 0 0 0 0 0 self% init-location
  ;attributes

location-11% :description
  \ XXX TODO -- crear ente. estancia y aguas
  sight case
  self% of
    s" Una" s{
      s{ s" gran" s" amplia" }s s" estancia" s&
      s" estancia" s" muy" s?& s{ s" grande" s" amplia" }s&
    }s& s" alberga un lago de" s&
    s{
      s" profundas" s" aguas" rnd2swap s& comma+ s" e" s?& s" iridiscentes" s&
      s" aguas tan profundas como iridiscentes,"
    }s&
    s{ s" gracias a" s" debido a" s" a causa de" s" por el efecto de" }s&
    s{
      s" la" s{ s" débil" s" tenue" }s?& s" luz" s&
        s{  s" que se filtra" s{ s" del" s" desde el" }s&
            s{ s" procendente" s" que procede" s" que entra" }s s" del" s&
        }s
      s" los" s{ s" débiles" s" tenues" }s?& s" rayos de luz" s&
        s{  s" que se filtran" s{ s" del" s" desde el" }s&
            s{ s" procendentes" s" que proceden" s" que entran" }s s" del" s&
        }s
    }s?& s" exterior." s&
    s" No hay" s&{ s" otra" s" más" }s& s" salida que el este." s&
    paragraph
    endof
  east% of
    s" De la entrada de la gruta"
    s{ s" procede" s" proviene" }s&
    s" la" s& s{ s" luz" s" luminosidad" s" claridad" }s
    s" que hace brillar" s&
    s{ s" el agua" s" las aguas" s" la superficie" }s&
    s" del lago." s&
    paragraph
  endof
  uninteresting-direction
  endcase
  ;description

location-12% :attributes
  s" salida del paso secreto" self% fs-name!
  self% be-indoor-location
  0 0 0 location-13% 0 0 0 0 self% init-location
  ;attributes

location-12% :description
  \ XXX TODO -- crear ente. agua aquí
  sight case
  self% of
    s" Una" s{ s" gran" s" amplia" }s&
    s" estancia se abre hacia el oeste," s&
    s" y se estrecha hasta" s& s{ s" morir" s" terminar" }s&
    s" , al este, en una" s+{ s" parte" s" zona" }s& s" de agua." s&
    paragraph
    endof
  east% of
    s{ s" La estancia" s" El lugar" }s
    s" se estrecha hasta " s&
    s{ s" morir" s" terminar" }s&
    s" en una" s&{ s" parte" s" zona" }s& s" de agua." s&
    paragraph
  endof
  west% of
    s" Se vislumbra la continuación de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-13% :attributes
  s" puente semipodrido" self% ms-name!
  self% be-indoor-location
  0 0 location-12% location-14% 0 0 0 0 self% init-location
  ;attributes

location-13% :description
  \ XXX TODO -- crear ente. canal, agua, lecho(~catre)
  sight case
  self% of
    s" La sala se abre en"
    s{ s" semioscuridad" s" penumbra" }s&
    s" a un puente cubierto de podredumbre" s&
    s" sobre el lecho de un canal, de este a oeste." s&
    paragraph
    endof
  east% of
    s" Se vislumbra el inicio de la cueva."
    paragraph
  endof
  west% of
    s" Se vislumbra un recodo de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-14% :attributes
  s" recodo de la cueva" self% ms-name!
  self% be-indoor-location
  0 location-15% location-13% 0 0 0 0 0 self% init-location
  ;attributes

location-14% :description
  sight case
  self% of
    s" La iridiscente cueva gira de este a sur."
    paragraph
    endof
  south% of
    you-glimpse-the-cave$ paragraph
    endof
  east% of
    you-glimpse-the-cave$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-15% :attributes
  s" pasaje arenoso" self% ms-name!
  self% be-indoor-location
  location-14% location-17% location-16% 0 0 0 0 0 self% init-location
  ;attributes

location-15% :description
  sight case
  self% of
    s" La gruta" goes-down$ s& s" de norte a sur" s&
    s" sobre un lecho arenoso." s&
    s" Al este, un agujero del que llega" s&
    s{ s" algo de luz." s" claridad." }s&
    paragraph
    endof
  north% of
    you-glimpse-the-cave$
    s" La cueva" goes-up$ s& in-that-direction$ s& period+
    paragraph
    endof
  south% of
    you-glimpse-the-cave$
    s" La cueva" goes-down$ s& in-that-direction$ s& period+
    paragraph
    endof
  east% of
    s{ s" La luz" s" Algo de luz" s" Algo de claridad" }s
    s" procede de esa dirección." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-16% :attributes
  s" pasaje del agua" self% ms-name!
  self% be-indoor-location
  0 0 0 location-15% 0 0 0 0 self% init-location
  ;attributes

location-16% :description
\ XXX TODO -- el examen del agua aquí debe dar más pistas
  sight case
  self% of
    s" Como un acueducto, el agua"
    goes-down$ s& s" con gran fuerza de norte a este," s&
    s" aunque la salida practicable es la del oeste." s&
    paragraph
    endof
  north% of
    s" El agua" goes-down$ s& s" con gran fuerza" s& from-that-way$ s& period+
    paragraph
    endof
  east% of
    s" El agua" goes-down$ s& s" con gran fuerza" s& that-way$ s& period+
    paragraph
    endof
  west% of
    s" Es la única salida." paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-17% :attributes
  s" estalactitas" self% fs-name!
  self% be-indoor-location
  location-15% location-20% location-18% 0 0 0 0 0 self% init-location
  ;attributes

location-17% :description
  \ XXX TODO -- crear ente. estalactitas
  sight case
  self% of
    s" Muchas estalactitas se agrupan encima de tu cabeza,"
    s" y se abren cual arco de entrada hacia el este y sur." s&
    paragraph
    endof
  north% of
    you-glimpse-the-cave$
    paragraph
    endof
  up% of
    s" Las estalactitas se agrupan encima de tu cabeza."
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-18% :attributes
  s" puente de piedra" self% ms-name!
  self% be-indoor-location
  0 0 location-19% location-17% 0 0 0 0 self% init-location
  ;attributes

location-18% :description
  \ XXX TODO -- crear ente. puente, arco
  sight case
  self% of
    s" Un arco de piedra se extiende,"
    s{ s" cual" s" como si fuera un" s" a manera de" }s&
    s" puente" s&
    s" que se" s{ s" elevara" s" alzara" }s& s?&
    s{ s" sobre" s" por encima de" }s&
    s" la oscuridad, de este a oeste." s&
    s{ s" Hacia" s" En" }s& s" su mitad" s&
    altar% is-known?
    if    s" está" s&
    else  s{ s" hay" s" es posible ver" s" puede verse" }s&
    then  altar% full-name s& period+ paragraph
    endof
  east% of
    s" El arco de piedra se extiende" that-way$ s& period+
    paragraph
    endof
  west% of
    s" El arco de piedra se extiende" that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-19% :attributes
  s" recodo arenoso del canal" self% ms-name!
  self% be-indoor-location
  0 0 0 location-18% 0 0 0 0 self% init-location
  ;attributes

location-19% :description
  sight case
  self% of
    \ XXX TODO -- hacer variaciones
    ^the-water-current$ comma+
    s" que discurre" s?&
    s" de norte a este," s& (it)-blocks$ s&
    s" el paso, excepto al oeste." s&
    s{ s" Al" s" Del" s" Hacia el" s" Proveniente del" s" Procedente del" }s&
    s" fondo" s& s{ s" se oye" s" se escucha" s" puede oírse" }s&
    s" un gran estruendo." s&
    paragraph
    endof
  north% of
    ^water-from-there$ period+ paragraph
    endof
  east% of
    water-that-way$ paragraph
    endof
  west% of
    s" Se puede" to-go-back$ s& toward-the(m)$ s& s" arco de piedra" s& in-that-direction$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-20% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  location-17% location-22% location-25% 0 0 0 0 0 self% init-location
  ;attributes

location-20% :description
  \ XXX TODO -- aplicar el filtro de la antorcha a todos los escenarios afectados, quizá en una capa superior
  \ sight no-torch? 0= abs *  case
  sight case
  self% of
    north% south% east% 3 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-21% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  0 location-27% location-23% location-20% 0 0 0 0 self% init-location
  ;attributes

location-21% :description
  sight case
  self% of
    east% west% south% 3 exits-cave-description paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-22% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  0 location-24% location-27% location-22% 0 0 0 0 self% init-location
  ;attributes

location-22% :description
  sight case
  self% of
    south% east% west% 3 exits-cave-description paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-23% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  0 location-25% 0 location-21% 0 0 0 0 self% init-location
  ;attributes

location-23% :description
  sight case
  self% of
    west% south% 2 exits-cave-description paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-24% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  location-22% 0 location-26% 0 0 0 0 0 self% init-location
  ;attributes

location-24% :description
  sight case
  self% of
    east% north% 2 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-25% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  location-22% location-28% location-23% location-21% 0 0 0 0 self% init-location
  ;attributes

location-25% :description
  sight case
  self% of
    north% south% east% west% 4 exits-cave-description paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-26% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  location-26% 0 location-20% location-27% 0 0 0 0 self% init-location
  ;attributes

location-26% :description
  \ XXX TODO -- crear ente. pasaje/camino/senda tramo/cueva (en todos los tramos)
  sight case
  self% of
    north% east% west% 3 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-27% :attributes
  s" tramo de cueva" self% ms-name!
  self% be-indoor-location
  location-27% 0 0 location-25% 0 0 0 0 self% init-location
  ;attributes

location-27% :description
  sight case
  self% of
    north% east% west% 3 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-28% :attributes
  s" amplia estancia" self% fs-name!
  self% be-indoor-location
  location-26% 0 0 0 0 0 0 0 self% init-location
  ;attributes

location-28% :description
  sight case
  self% of
    \ XXX TODO -- crear ente. estancia(para todos),albergue y refugio (tras hablar con anciano)
    self% ^full-name s" se extiende de norte a este." s&
    leader% conversations?
    if  s" Hace de albergue para los refugiados."
    else  s" Está llen" self% gender-ending+ s" de gente." s&
    then  s&
    flags% is-known?
    if
      s" Hay" s&
      s{  s" una bandera de cada bando"
          s" banderas de" s{ s" ambos" s" los dos" }s& s" bandos" s&
          s{ s" dos banderas: una" s" una bandera" }s
              s{  s" britana y otra sajona" s" sajona y otra britana" }s&
      }s& period+
    else
      s" Hay" s& s{ s" dos" s" unas" }s& s" banderas." s&
    then
    paragraph
    endof
  north% of
    self% has-east-exit?
    if    s" Es por donde viniste."
    else  s" Es la única salida."
    then  paragraph
    endof
  east% of
    ^the-refugees$
    self% has-east-exit?
    if    they-let-you-pass$ s&
    else  they-don't-let-you-pass$ s&
    then  period+ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-29% :attributes
  s" espiral" self% fs-name!
  self% be-indoor-location
  0 0 0 location-28% 0 location-30% 0 0 self% init-location
  ;attributes

location-29% :description
  \ XXX TODO -- crear ente. escalera/espiral, refugiados
  sight case
  self% of
    s" Cual escalera de caracol gigante,"
    goes-down-into-the-deep$ comma+ s&
    s" dejando a los refugiados al oeste." s&
    paragraph
    endof
  west% of
    over-there$ s" están los refugiados." s&
    paragraph
    endof
  down% of
    s" La espiral" goes-down-into-the-deep$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-30% :attributes
  s" inicio de la espiral" self% ms-name!
  self% be-indoor-location
  0 0 location-31% 0 location-29% 0 0 0 self% init-location
  ;attributes

location-30% :description
  sight case
  self% of
    s" Se eleva en la penumbra."
    s" La" s& cave$ s& gets-narrower(f)$ s&
    s" ahora como para una sola persona, hacia el este." s&
    paragraph
    endof
  east% of
    s" La" cave$ s& gets-narrower(f)$ s& period+
    paragraph
    endof
  up% of
    s" La" cave$ s& s" se eleva en la penumbra." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-31% :attributes
  s" puerta norte" self% fs-name!
  self% be-indoor-location
  0 0 0 location-30% 0 0 0 0 self% init-location
  ;attributes

location-31% :description
  \ XXX TODO -- crear ente. arco, columnas, hueco/s(entre rocas)
  sight case
  self% of
    s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
    paragraph
    endof
  north% of
    s" Las rocas"  self% has-north-exit?
    if  (rocks)-on-the-floor$
    else  (they)-block$ the-pass$ s&
    then  s& period+ paragraph
    endof
  west% of
    ^that-way$ s" se encuentra el inicio de la espiral." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-32% :attributes
  s" precipicio" self% ms-name!
  self% be-indoor-location
  0 location-33% 0 location-31% 0 0 0 0 self% init-location
  ;attributes

location-32% :description
  \ XXX TODO -- crear ente. precipicio, abismo, cornisa, camino, roca/s
  sight case
  self% of
    s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
    s" El soporte de roca gira en forma de «U» de oeste a sur." s&
    paragraph
    endof
  south% of
    ^the-path$ s" gira" s& that-way$ s& period+
    paragraph
    endof
  west% of
    ^the-path$ s" gira" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-33% :attributes
  s" pasaje de salida" self% ms-name!
  self% be-indoor-location
  location-32% 0 location-34% 0 0 0 0 0 self% init-location
  ;attributes

location-33% :description
  \ XXX TODO -- crear ente. camino/paso/sendero
  sight case
  self% of
    s" El paso se va haciendo menos estrecho a medida que se avanza hacia el sur, para entonces comenzar hacia el este."
    paragraph
    endof
  north% of
    ^the-path$ s" se estrecha" s& that-way$ s& period+
    paragraph
    endof
  south% of
    ^the-path$ gets-wider$ s& that-way$ s&
    s" y entonces gira hacia el este." s&
    paragraph
    endof
  east% of
    ^the-path$ gets-wider$ s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-34% :attributes
  \ XXX TODO -- crear ente. gravilla
  s" pasaje de gravilla" self% ms-name!
  self% be-indoor-location
  location-35% 0 0 location-33% 0 0 0 0 self% init-location
  ;attributes

location-34% :description
  \ XXX TODO -- crear ente. camino/paso/sendero, guijarros, moho, roca, suelo..
  sight case
  self% of
    s" El paso" gets-wider$ s& s" de oeste a norte," s&
    s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
    paragraph
    endof
  north% of
    ^the-path$ gets-wider$ s& that-way$ s& period+
    paragraph
    endof
  west% of
    ^the-path$ s" se estrecha" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-35% :attributes
  s" puente sobre el acueducto" self% ms-name!
  self% be-indoor-location
  location-40% location-34% 0 location-36% 0 location-36% 0 0 self% init-location
  ;attributes

location-35% :description
  \ XXX TODO -- crear ente. escaleras, puente, río/curso/agua
  sight case
  self% of
    s" Un puente" s{ s" se tiende" s" cruza" }s& s" de norte a sur sobre el curso del agua." s&
    s" Unas resbaladizas escaleras" s& (they)-go-down$ s& s" hacia el oeste." s&
    paragraph
    endof
  north% of
    bridge-that-way$ paragraph
    endof
  south% of
    bridge-that-way$ paragraph
    endof
  west% of
    stairway-to-river$ paragraph
    endof
  down% of
    stairway-to-river$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-36% :attributes
  s" remanso" self% ms-name!
  self% be-indoor-location
  0 0 location-35% location-37% location-35% 0 0 0 self% init-location
  ;attributes

location-36% :description
  sight case
  self% of
    s" Una" s{ s" ruidosa" s" estruendosa" s" ensordecedora" }s&
    s" corriente" s& goes-down$ s&
    s{ s" con" s" siguiendo" }s& s" el" s& pass-way$ s&
    s" elevado desde el oeste, y forma un meandro arenoso." s&
    s" Unas escaleras" s& (they)-go-up$ s& toward-the(m)$ s& s" este." s&
    paragraph
    endof
  east% of
    stairway-that-way$ paragraph
    endof
  west% of
    ^water-from-there$ period+ paragraph
    endof
  up% of
    stairway-that-way$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-37% :attributes
  s" canal de agua" self% ms-name!
  self% be-indoor-location
  0 0 location-36% location-38% 0 0 0 0 self% init-location
  ;attributes

location-37% :description
  sight case
  self% of
    s" El agua" goes-down$ s& s" por un canal" s?&
    from-the(m)$ s& s" oeste con" s&
    s{ s" renovadas fuerzas" s" renovada energía" s" renovado ímpetu" }s& comma+
    s" dejando" s& s{
    s" a un lado" a-high-narrow-pass-way$ s&
    a-high-narrow-pass-way$ s{ s" lateral" s" a un lado" }s&
    }s& s" que" s& lets-you$ s& to-keep-going$ s&
    toward-the(m)$ s" este" s&
    toward-the(m)$ s" oeste" s& rnd2swap s" o" s& 2swap s& s&
    period+ paragraph
    endof
  east% of
    ^the-pass-way$ s" elevado" s?& lets-you$ s& to-keep-going$ s& that-way$ s& period+
    paragraph
    endof
  west% of
    water-from-there$
    the-pass-way$ s" elevado" s?& lets-you$ s& to-keep-going$ s& that-way$ s&
    both s" también" s& ^uppercase period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-38% :attributes
  s" gran cascada" self% fs-name!
  self% be-indoor-location
  0 0 location-37% location-39% 0 0 0 0 self% init-location
  ;attributes

location-38% :description
  sight case
  self% of
    s" Cae el agua hacia el este,"
    s{ s" descendiendo" s" bajando" }s&
    s{ s" con mucha fuerza" s" con gran fuerza" s" fuertemente" }s&
    s{ s" en dirección al" s" hacia el" }s& s" canal," s&
    s{ s" no sin antes" s" tras" s" después de" }s&
    s{ s" embalsarse" s" haberse embalsado" }s&
    s" en un lago" s&
    s{ s" no muy" s" no demasiado" s" poco" }s& s" profundo." s&
    paragraph
    endof
  east% of
    water-that-way$ paragraph
    endof
  west% of
    \ XXX TODO -- el artículo de «cascada» debe depender también de si se ha visitado el escenario 39 o este mismo 38
    ^water-from-there$
    s" , de" s+ waterfall% full-name s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-39% :attributes
  s" interior de la cascada" self% ms-name!
  self% be-indoor-location
  0 0 location-38% 0 0 0 0 0 self% init-location
  ;attributes

location-39% :description
  sight case
  self% of
    \ XXX TODO -- crear ente. musgo, cortina, agua, hueco
    s" Musgoso y rocoso, con la cortina de agua"
    s{ s" tras de ti," s" a tu espalda," }s&
    s{ s" el nivel" s" la altura" }s& s" del agua ha" s&
    s{ s" subido" s" crecido" }s&
    s{ s" un poco" s" algo" }s& s" en este" s&
    s{ s" curioso" s" extraño" }s& s" hueco." s&
    paragraph
    endof
  east% of
    \ XXX TODO -- variar
    s" Es la única salida." paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-40% :attributes
  s" explanada" self% fs-name!
  self% be-indoor-location
  0 location-35% location-41% 0 0 0 0 0 self% init-location
  ;attributes

location-40% :description
  \ XXX TODO -- crear ente. losas y losetas, estalactitas, panorama, escalones
  sight case
  self% of
    s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
    s" Unos casi imperceptibles escalones conducen al este." s&
    paragraph
    endof
  south% of
    ^that-way$ s" se va" s& toward-the(m)$ s& s" puente." s&
    paragraph
    endof
  east% of
    s" Los escalones" (they)-lead$ s& that-way$ s& period+
    paragraph
    endof
  up% of
    s{ s" Sobre" s" Por encima de" }s
    s{ s" ti" s" tu cabeza" }s& s" se" s&
    s{ s" exhibe" s" extiende" s" disfruta" }s&
    s" un" s& beautiful(m)$ s&
    s{ s" panorama" s" paisaje" }s s& s" de estalactitas." s&
    paragraph
    endof
  down% of
    s" Es una" s{ s" gran" s" buena" }s s& s" explanada enlosetada." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-41% :attributes
  \ XXX TODO -- cambiar el nombre. no se puede pasar a mayúscula un carácter pluriocteto en utf-8
  self% be-indoor-location
  s" ídolo" self% ms-name!
  0 0 0 location-40% 0 0 0 0 self% init-location
  ;attributes

location-41% :description
  \ XXX TODO -- crear ente. roca, centinela
  sight case
  self% of
    s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al sur."
    s" Se puede" s& to-go-back$ s& toward$ s& s" la explanada hacia el oeste." s&
    paragraph
    endof
  south% of
    s" Hay una" s" roca" s" enorme" rnd2swap s& s&
    that-way$ s& period+
    paragraph
    endof
  west% of
    s" Se puede volver" toward$ s& s" la explanada" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-42% :attributes
  s" pasaje estrecho" self% ms-name!
  self% be-indoor-location
  location-41% location-43% 0 0 0 0 0 0 self% init-location
  ;attributes

location-42% :description
  sight case
  self% of
    s" Como un pasillo que corteja el canal de agua, a su lado, baja de norte a sur."
    paragraph
    endof
  north% of
    ^the-pass-way$ goes-up$ s& that-way$ s&
    s" , de donde" s{ s" corre" s" procede" s" viene" s" proviene" }s& s" el agua." s& s+
    paragraph
    endof
  south% of
    ^the-pass-way$ goes-down$ s& that-way$ s&
    s" , siguiendo el canal de agua," s+
    s" hacia un lugar en que" s&
    s{ s" se aprecia" s" puede apreciarse" s" se distingue" }s&
    s" un aumento de luz." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-43% :attributes
  s" pasaje de la serpiente" self% ms-name!
  self% be-indoor-location
  location-42% 0 0 0 0 0 0 0 self% init-location
  ;attributes

location-43% :description
  sight case
  self% of
    ^the-pass-way$ s" sigue de norte a sur." s&
    paragraph
    endof
  north% of
    ^the-pass-way$ s" continúa" s& that-way$ s& period+
    paragraph
    endof
  south% of
    snake% is-here?
    if  a-snake-blocks-the-way$
    else  ^the-pass-way$ s" continúa" s& that-way$ s&
    then  period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-44% :attributes
  s" lago interior" self% ms-name!
  self% be-indoor-location
  location-43% 0 0 location-45% 0 0 0 0 self% init-location
  ;attributes

location-44% :description
  \ XXX TODO -- crear ente. lago, escaleras, pasaje, lago
  sight case
  self% of
    s" Unas escaleras" s{ s" dan" s" permiten el" }s& s{ s" paso" s" acceso" }s&
    s" a un" s& beautiful(m)$ s& s" lago interior, hacia el oeste." s&
    s" Al norte, un oscuro y"
    narrow(m)$ s& pass-way$ s& goes-up$ s& period+ s?&
    paragraph
    endof
  north% of
    s" Un pasaje oscuro y" narrow(m)$ s& goes-up$ s& that-way$ s& period+
    paragraph
    endof
  west% of
    s" Las escaleras" (they)-lead$ s& that-way$ s& s" , hacia el lago" s?+ period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-45% :attributes
  s" cruce de pasajes" self% ms-name!
  self% be-indoor-location
  0 location-47% location-44% location-46% 0 0 0 0 self% init-location
  ;attributes

location-45% :description
  \ XXX TODO -- crear ente. pasaje/camino/paso/senda
  sight case
  self% of
    ^narrow(mp)$ pass-ways$ s&
    s" permiten ir al oeste, al este y al sur." s&
    paragraph
    endof
  south% of
    ^a-narrow-pass-way$ s" permite ir" s& that-way$ s&
    s" , de donde" s+ s{ s" proviene" s" procede" }s&
    s{ s" una gran" s" mucha" }s& s" luminosidad." s&
    paragraph
    endof
  west% of
    ^a-narrow-pass-way$ leads$ s& that-way$ s& period+
    paragraph
    endof
  east% of
    ^a-narrow-pass-way$ leads$ s& that-way$ s& period+
    s" , de donde" s{ s" proviene" s" procede" }s&
    s{ s" algo de" s" una poca" s" un poco de" }s&
    s{ s" claridad" s" luz" }s& period+ s+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-46% :attributes
  s" hogar de Ambrosio" self% ms-name!
  self% be-indoor-location
  0 0 location-45% 0 0 0 0 0 self% init-location
  ;attributes

location-46% :description
  sight case
  self% of
    s" Un catre, algunas velas y una mesa es todo lo que"
    s{ s" tiene" s" posee" }s s" Ambrosio" rnd2swap s& s&
    period+  paragraph
    endof
  east% of
    s" La salida"
    s{ s" de la casa" s" del hogar" }s s" de Ambrosio" s& s?&
    s" está" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-47% :attributes
  s" salida de la cueva" self% fs-name!
  self% be-indoor-location
  location-45% 0 0 0 0 0 0 0 self% init-location
  ;attributes

location-47% :description
  \ XXX TODO -- descripción inacabada.
  sight case
  self% of
    s" Por el oeste,"
    door% full-name s& door% «open»|«closed» s& comma+
    door% is-open? if  \ La puerta está abierta
      s" por la cual entra la luz que ilumina la estancia," s&
      s" permite salir de la cueva." s&
    else  \ La puerta está cerrada
      s" al otro lado de la cual se adivina la luz diurna," s&
      door% is-known?
      if    s" impide" s&
      else  s" parece ser" s&
      then  s" la salida de la cueva." s&
    then
    paragraph
    endof
  north% of
    \ XXX TODO -- variar
    s" Hay salida" that-way$ s& period+ paragraph
    endof
  west% of
    \ XXX TODO -- variar
    door% is-open? if
      s" La luz diurna entra por la puerta."
    else
      s" Se adivina la luz diurna al otro lado de la puerta."
    then
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-48% :attributes
  s" bosque a la entrada" self% ms-name!
  0 0 location-47% location-49% 0 0 0 0 self% init-location
  ;attributes

: when-the-door$  ( -- ca len )
  s" cuando" s{ s" la" s" su" }s& s" puerta" s&  ;

: like-now$+  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" , como ahora" s?+  ;

location-48% :description
  \ XXX TODO -- crear ente. cueva
  sight case
  self% of
    s{ s" Apenas si" s" Casi no" }s
    s{ s" se puede" s" es posible" }s&
    s" reconocer la entrada de la cueva, al este." s&
    ^the-path$ s& s{ s" parte" s" sale" }s&
    s" del bosque hacia el oeste." s&
    paragraph
    endof
  east% of
    s" La entrada de la cueva" s{
    s" está" s" bien" s?& s{ s" camuflada" s" escondida" }s&
    s" apenas se ve" s" casi no se ve" s" pasa casi desapercibida"
    }s& comma+
    door% is-open? if
      even$ s& when-the-door$ s&
      s{ s" está abierta" s" no está cerrada" }s& like-now$+
    else
      s{ s" especialmente" s" sobre todo" }s& when-the-door$ s&
      s{ s" no está abierta" s" está cerrada" }s& like-now$+
    then  period+ paragraph
    endof
  west% of
    ^the-path$ s{ s" parte" s" sale" }s& s" del bosque" s& in-that-direction$ s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-49% :attributes
  s" sendero del bosque" self% ms-name!
  0 0 location-48% location-50% 0 0 0 0 self% init-location
  ;attributes

location-49% :description
  sight case
  self% of
    ^the-path$ s" recorre" s& s" toda" s?&
    s" esta" s& s{ s" parte" s" zona" }s&
    s" del bosque de este a oeste." s&
    paragraph
    endof
  east% of
    ^the-path$ leads$ s&
    s" al bosque a la entrada de la cueva." s&
    paragraph
    endof
  west% of
    ^the-path$ s" continúa" s& in-that-direction$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-50% :attributes
  s" camino norte" self% ms-name!
  0 location-51% location-49% 0 0 0 0 0 self% init-location
  ;attributes

location-50% :description
  sight case
  self% of
    s" El camino norte" s{ s" que sale" s" que parte" s" procedente" }s&
    s" de Westmorland se" s{ s" interna" s" adentra" }s& s" en el bosque," s&
    s" aunque en tu estado no puedes ir." s&
    paragraph
    endof
  south% of
    s{ s" ¡Westmorland!" s" Westmorland..." }s
    paragraph
    endof
  east% of
    ^can-see$ s" el sendero del bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

location-51% :attributes
  s" Westmorland" self% fs-name!
  self% has-no-article
  location-50% 0 0 0 0 0 0 0 self% init-location
  ;attributes

location-51% :description
  \ XXX TODO -- crear ente. mercado, plaza, villa, pueblo, castillo
  sight case
  self% of
    ^the-village$ s" bulle de actividad con el mercado en el centro de la plaza," s&
    s" donde se encuentra el castillo." s&
    paragraph
    endof
  north% of
    s" El camino norte" of-the-village$ s& leads$ s& s" hasta el bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

\ ----------------------------------------------
\ Entes globales

cave% :attributes
  s" cueva" self% fs-name!
  \ self% be-global-indoor \ XXX
  ;attributes

cave% :description
  \ XXX TMP
  s" La cueva es chachi."
  paragraph
  ;description

ceiling% :attributes
  s" techo" self% ms-name!
  self% be-global-indoor
  ;attributes

ceiling% :description
  \ XXX TMP
  s" El techo es muy bonito."
  paragraph
  ;description

clouds% :attributes
  s" nubes" self% fp-name!
  self% be-global-outdoor
  ;attributes

clouds% :description
  \ XXX TODO:
  \ Distinguir no solo interiores, sino escenarios en
  \ que se puede vislumbrar el exterior.
  \ XXX TMP.:
  s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
  s" en la estación del frío se han alejado por el momento. " s&
  2 random if  paragraph  else  2drop sky% describe  then  \ XXX TODO -- comprobar
  ;description

floor% :attributes
  s" suelo" self% ms-name!
  self% be-global-indoor
  self% be-global-outdoor
  ;attributes

floor% :description
  \ XXX TMP
  am-i-outdoor? if
    s" El suelo fuera es muy bonito."
    paragraph
  else
    \ XXX FIXME -- se muestra esto en exteriores
    s" El suelo dentro es muy bonito."
    paragraph
  then
  ;description

sky% :attributes
  s" cielo" self% ms-name!
  self% be-global-outdoor
  ;attributes

sky% :description
  \ XXX TMP
  s" [El cielo es mu bonito]"
  paragraph
  ;description

wall% :attributes
  s" pared" self% ms-name!
  self% be-global-indoor
  ;attributes

wall% :description
  \ XXX TMP
  s" [La pared es mu bonita]"
  paragraph
  ;description

\ ----------------------------------------------
\ Entes virtuales

exits% :attributes
  s" salida" self% fs-name!
  self% be-global-outdoor
  self% be-global-indoor
  ;attributes

exits% :description
  list-exits
  ;description

inventory% :attributes
  ;attributes

enemy% :attributes
  \ XXX TODO -- inconcluso
  s" enemigos" self% mp-name!
  self% be-human
  self% be-decoration
  ;attributes

enemy% :description
  \ XXX TODO -- inconcluso
  battle# @ if
    s" Enemigo en batalla!!!"  \ XXX TMP
  else
    s" Enemigo en paz!!!"  \ XXX TMP
  then  paragraph
  ;description

\ ----------------------------------------------
\ Entes dirección

\ Los entes dirección guardan en su campo `~direction`
\ el desplazamiento correspodiente al campo de
\ dirección que representan
\ Esto sirve para reconocerlos como tales entes dirección
\ (pues todos los valores posibles son diferentes de cero)
\ y para hacer los cálculos en las acciones de movimiento.

north% :attributes
  s" norte" self% ms-name!
  self% has-definite-article
  north-exit> self% ~direction !
  ;attributes

south% :attributes
  s" sur" self% ms-name!
  self% has-definite-article
  south-exit> self% ~direction !
  ;attributes

east% :attributes
  s" este" self% ms-name!
  self% has-definite-article
  east-exit> self% ~direction !
  ;attributes

west% :attributes
  s" oeste" self% name!
  self% has-definite-article
  west-exit> self% ~direction !
  ;attributes

up% :attributes
  s" arriba" self% name!
  self% has-no-article
  up-exit> self% ~direction !
  ;attributes

up% :description
  am-i-outdoor?
  if  sky% describe
  else  ceiling% describe
  then
  ;description

down% :attributes
  s" abajo" self% name!
  self% has-no-article
  down-exit> self% ~direction !
  ;attributes

down% :description
  \ XXX TMP
  am-i-outdoor? if
    s" El suelo exterior es muy bonito." paragraph
  else
    s" El suelo interior es muy bonito." paragraph
  then
  ;description

out% :attributes
  s" afuera" self% name!
  self% has-no-article
  out-exit> self% ~direction !
  ;attributes

in% :attributes
  s" adentro" self% name!
  self% has-no-article
  in-exit> self% ~direction !
  ;attributes

\ vim:filetype=gforth:fileencoding=utf-8

