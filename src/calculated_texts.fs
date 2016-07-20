\ calculated_texts.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607202041

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/choose.fs          \ `choose`
require galope/random_strings.fs
require galope/shuffle.fs         \ `shuffle`

set-current

\ ==============================================================
\ Textos calculados

\ Las palabras de esta sección se usan para construir las
\ descripciones de los entes.

\ ----------------------------------------------
\ Albergue de los refugiados

: the-refugees$  ( -- ca len )
  leader~ conversations?
  if  s" los refugiados"  else  s" todos"  then  ;

: ^the-refugees$  ( -- ca len )
  the-refugees$ ^uppercase  ;

: they-don't-let-you-pass$  ( -- ca len )
  s{
  s" te" s? (they)-block$ s&
  s" te rodean,"
  s{ s" impidiéndote" s" impidiendo"
  s" obstruyendo" s" obstruyéndote"
  s" bloqueando" s" bloqueándote" }s bs&
  }s the-pass$ s&  ;
  \ Mensaje de que los refugiados no te dejan pasar.

: the-pass-free$  ( -- ca len )  s" libre" the-pass$ s&  ;
  \ Variante de «libre el paso».

: they-let-you-pass-0$  ( -- ca len )
  s{
  s" te" s? s" han dejado" s&
  s" se han" s{ s" apartado" s" echado a un lado" }s bs& s" para dejar" s& s" te" s? bs+
  }s the-pass-free$ s&  ;
  \ Primera versión del mensaje de que te dejan pasar.

: they-let-you-pass-1$  ( -- ca len )
  s" se han" s{ s" apartado" s" retirado" }s bs&
  s" a" s{ s{ s" los" s" ambos" }s s" lados" s& s" uno y otro lado" }s bs& s? bs& comma+
  s{ s" dejándote" s" dejando" s" para dejar" s" te" s? bs+ }s bs&
  the-pass-free$ s&  ;
  \ Segunda versión del mensaje de que te dejan pasar.

: they-let-you-pass-2$  ( -- ca len )
  s" ya no" they-don't-let-you-pass$ s& s" como antes" s? bs&  ;
  \ Tercera versión del mensaje de que te dejan pasar.

: they-let-you-pass$  ( -- ca len )
  ['] they-let-you-pass-0$
  ['] they-let-you-pass-1$
  ['] they-let-you-pass-2$
  3 choose execute  ;
  \ Mensaje de que te dejan pasar.

: the-leader-said-they-want-peace$  ( -- ca len )
  s" que," s" tal y" s? bs& s" como" s& leader~ full-name s&
  s" te" s? bs& s" ha" bs& s{ s" referido" s" hecho saber" s" contado" s" explicado" }s bs& comma+
  they-want-peace$ s&  ;
  \ El líder te dijo qué buscan los refugiados.

: you-don't-know-why-they're-here$  ( -- ca len )
  s{  s" Te preguntas"
      s" No" s{  s" terminas de"
                  s" acabas de"
                  s" aciertas a"
                  s" puedes"
                }s bs& to-understand$ s&
      s" No" s{ s" entiendes" s" comprendes" }s bs&
  }s s{
    s" qué" s{ s" pueden estar" s" están" }s bs& s" haciendo" s&
    s" qué" s{ s" los ha" s{ s" podría" s" puede" }s s" haberlos" s& }s bs&
      s{ s" reunido" s" traído" s" congregado" }s bs&
    s" cuál es" s{ s" el motivo" s" la razón" }s bs&
      s" de que se" bs& s{ s" encuentren" s" hallen" }s bs&
    s" por qué" s{ s" motivo" s" razón" }s s? bs& s" se encuentran" s&
  }s bs& here$ s& period+  ;
  \ No sabes por qué están aquí los refugiados.

: some-refugees-look-at-you$  ( -- ca len )
  s" Algunos" s" de ellos" s? bs&
  s" reparan en" bs& s{ s" ti" s" tu persona" s" tu presencia" }s bs&  ;

: in-their-eyes-and-gestures$  ( -- ca len )
  s" En sus" s{ s" ojos" s" miradas" }s bs&
  s" y" s" en sus" s? bs& s" gestos" bs& s? s&  ;
  \ En sus ojos y gestos.

: the-refugees-trust$  ( -- ca len )
  some-refugees-look-at-you$ period+
  in-their-eyes-and-gestures$ s&
  s{ s" ves" s" notas" s" adviertes" s" aprecias" }s bs&
  s{
    s" amabilidad" s" confianza" s" tranquilidad"
    s" serenidad" s" afabilidad"
  }s bs&  ;
  \ Los refugiados confían.

: you-feel-they-observe-you$  ( -- ca len )
  s{ s" tienes la sensación de que" s" sientes que" }s s?
  s" te observan" s& s" como" s? bs&
  s{  s" con timidez" s" tímidamente"
      s" de" way$ s& s" subrepticia" s& s" subrepticiamente"
      s" a escondidas"
  }s bs& period+  ;
  \ Sientes que te observan.

: the-refugees-don't-trust$  ( -- ca len )
  some-refugees-look-at-you$ s{ s" . Entonces" s"  y" }s bs+
  you-feel-they-observe-you$ s&
  in-their-eyes-and-gestures$ s&
  s{
    s{ s" crees" s" te parece" }s to-realize$ s&
    s{ s" ves" s" notas" s" adviertes" s" aprecias" }s
    s" parece" to-realize$ s& s" se" s+
  }s bs& s{
    s" cierta" s? s{
      s" preocupación" s" desconfianza" s" intranquilidad"
      s" indignación" }s bs&
    s" cierto" s? s{ s" nerviosismo" s" temor" }s bs&
  }s bs&  ;
  \ Los refugiados no confían.

: diverse-people$  ( -- ca len )
  s{ s" personas" s" hombres, mujeres y niños" }s
  s" de toda" s& s" edad y" s? bs& s" condición" s&  ;

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

: toward-the(m/f)  ( a -- ca len )
  has-feminine-name? if  toward-the(f)$  else  toward-the(m)$  then  ;
  \ Devuelve una variante de «hacia el» con el artículo adecuado a un
  \ ente _a_.

: toward-(the)-name  ( a -- ca len )
  dup has-no-article?
  if    s" hacia"
  else  dup toward-the(m/f)
  then  rot name s&  ;
  \ Devuelve una variante de «hacia el nombre-de-ente» adecuada a un
  \ ente _a_.

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

: cave-exit-description-1$ ( -- ca len )
  ^a-pass-way$
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s bs&
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
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s bs&
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
  s{ s" ," s" ;" s" ..." }s bs+
  s{ s" y" 2dup s" aunque" but$ }s bs& s" también" s? bs&  ;
  \ Concatena (sin separación) a una cadena el separador entre las
  \ salidas principales y las secundarias.

: (paths)-can-be-seen-0$  ( -- ca len )
  s{ s" parten" s" surgen" s" nacen" s" salen" }s
  s" de" s{ s" aquí" s" este lugar" }s bs& s? rnd2swap s&  ;

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
  pass-ways$ s& s" más" s? bs&
  (paths)-can-be-seen$ rnd2swap s&  ;
  \ Devuelve la presentación de la lista de salidas secundarias.
  \ ca1 len1 = Cadena con el número de pasajes
  \ ca2 len2 = Cadena con el resultado

: secondary-exit-in-cave&  ( a ca2 len2 -- ca3 len3 )
  rot toward-(the)-name s&  ;
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ a1 = Ente dirección cuya descripción hay que añadir
  \ ca2 len2 = Descripción en curso

: one-secondary-exit-in-cave  ( a -- ca len )
  a-pass-way$
  s{ s" parte" s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s bs&
  secondary-exit-in-cave&  ;
  \ Devuelve la descripción _ca len_ de una salida adicional en un
  \ tramo de cueva, del ente dirección _a_.

: two-secondary-exits-in-cave  ( a1 a2 -- ca3 len3 )
  s" dos" paths-seen s" :" s? bs+
  secondary-exit-in-cave& s" y" s&
  secondary-exit-in-cave&  ;
  \ Devuelve la descripción de dos salidas adicionales en un tramo de
  \ cueva.

: three-secondary-exits-in-cave  ( a1 a2 a3 -- ca4 len4 )
  s" tres" paths-seen s" :" s? bs+
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

: shuffle-cave-exits  ( a1 ... an u -- a1'..an' u )  dup >r shuffle r>  ;
  \ Desordena los entes dirección que son las salidas de la cueva.
  \ u = Número de elementos de la pila que hay que desordenar

: (exits-cave-description)  ( a1 ... an u -- ca2 len2 )
  1- cells 'cave-descriptions + perform  ;
  \ Ejecuta (según el número de salidas) la palabra  que devuelve la
  \ descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados

: exits-cave-description  ( a1 ... an u -- ca2 len2 )
  shuffle-cave-exits  (exits-cave-description) period+
  main-cave-exits-are$ 2swap s&  ;  \ Añadir el encabezado
  \ Devuelve la descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados

: cave-exit-description$  ( -- ca len )
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
  s" los restos" still$ s? s" humeantes" s& s? bs&
  s" de la" s& poor-village$ s&  ;
  \ Devuelve parte de otras descripciones de la aldea arrasada.

\ ----------------------------------------------
\ Pared del desfiladero

: to-pass-(wall)$  ( -- ca len )
  s{ s" superar" s" atravesar" s" franquear" s" vencer" s" escalar" }s  ;
  \ Infinitivo aplicable para atravesar una pared o un muro.

: it-looks-impassable$  ( -- ca len )
  s{ s" por su aspecto" s" a primera vista" }s s?
  s{  s" parece"
      s" diríase que es"
      s" bien" s? s" puede decirse que es" s&
  }s bs&
  s{  s" imposible de" to-pass-(wall)$ s&
      s" imposible" to-pass-(wall)$ s& s" la" s+
      s{ s" insuperable" s" invencible" s" infranqueable" }s
  }s bs&  ;
  \ Mensaje «parece infranqueable».
  \ XXX TODO -- ojo: género femenino; generalizar factorizando cuando se use en otro contexto

: the-cave-entrance-is-visible$  ( -- ca len )
  s{  s" se" s{ s" ve" s" abre" s" haya"
                s" encuentra" s" aprecia" s" distingue" }s bs&
      s" puede" s{ s" verse" s" apreciarse" s" distinguirse" }s bs&
  }s cave-entrance~ full-name s&  ;

\ ----------------------------------------------
\ Entrada a la cueva

: you-discover-the-cave-entrance$  ( -- ca len )
  ^but$ comma+
  s{  s" reconociendo" s" el terreno" more-carefully$ rnd2swap s& s&
      s" fijándote" s" ahora" s? bs& more-carefully$ s&
  }s bs& comma+ s" descubres" s& s{ s" lo" s" algo" }s bs& s" que" s&
  s" sin duda" s? bs& s{ s" parece ser" s" es" s" debe de ser" }s bs&
  s{ s" la entrada" s" el acceso" }s bs& s" a una" s& cave$ s&  ;
  \ Mensaje de que descubres la cueva.

: the-cave-entrance-is-hidden$  ( -- ca len )
  s" La entrada" s" a la cueva" s? bs&
  s{ s" está" s" muy" s? bs& s" no podría estar más" }s bs&
  s{ s" oculta" s" camuflada" s" escondida" }s bs&
  s" en la pared" s& rocky(f)$ s& period+  ;

: you-were-lucky-discovering-it$ ( -- ca len )
  s" Has tenido" s" muy" s? bs& s" buena" bs& s{ s" fortuna" s" suerte" }s bs&
  s{  s{ s" al" s" en" s" con" }s
        s{ s" hallarla" s" encontrarla" s" dar con ella" s" descubrirla" }s bs&
      s{  s" hallándola" s" encontrándola"
          s" dando con ella" s" descubriéndola"
      }s
  }s bs& period+  ;

: it's-your-last-hope$  ( -- ca len )
  s{ s" te das cuenta de que" s" sabes que" }s s?
  s{ s" es" s" se trata de" }s bs& ^uppercase
  your|the(f)$ s& s{ s" única" s" última" }s bs&
  s{ s" salida" s" opción" s" esperanza" s" posibilidad" }s bs&
  s{ s" de" s" para" }s
  s{  s" escapar" s" huir"
      s" evitar" s{ s" ser capturado" s" que te capturen" }s bs&
  }s bs& s? s& period+  ;

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
  ^the-water-current$ s{ s" corre" s" fluye" s" va" }s bs&
  in-that-direction$ s& period+  ;

: stairway-to-river$  ( -- ca len )
  s" Las escaleras" (they)-go-down$ s&
  that-way$ s& comma+
  s{ s" casi" s? s" hasta el" s& s" mismo" s? bs& s" borde del" s&
  s" casi" s? s" hasta la" s& s" misma" s? bs& s" orilla del" s&
  s" casi" s? s" hasta el" s&
  s" hasta" s? s" cerca del" s& }s bs& s" agua." s&  ;

: a-high-narrow-pass-way$  ( -- ca len )
  s" un" narrow(m)$ s& pass-way$ s& s" elevado" s&  ;

\ vim:filetype=gforth:fileencoding=utf-8
