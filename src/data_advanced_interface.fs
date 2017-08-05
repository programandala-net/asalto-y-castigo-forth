\ data_advanced_interface.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last modified 201708051150
\ See change log at the end of the file

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-false.fs    \ `[false]`
require galope/bracket-true.fs     \ `[true]`
require galope/s-curly-bracket.fs  \ `s{`
require galope/replaced.fs         \ `replaced`
require galope/txt-plus.fs         \ `txt+`

set-current

\ ==============================================================
\ Interfaz de datos avanzada

\ Esta interfaz de datos depende de algunos identificadores de entes.

: belongs-to-protagonist?  ( a -- f )  protagonist~ is-owner?  ;
: belongs-to-protagonist  ( a -- )  protagonist~ be-owner  ;

: my-location  ( -- a )  protagonist~ location  ;
  \ Devuelve la localización del protagonista.

: my-previous-location  ( -- a )  protagonist~ previous-location  ;
  \ Devuelve la localización anterior del protagonista.

: my-location!  ( a -- )  protagonist~ be-there  ;
  \ Mueve el protagonista al ente indicado.

: am-i-there?  ( a -- f )  my-location =  ;
  \ ¿Está el protagonista en la localización indicada?
  \ a = Ente que actúa de localización

: am-i-outdoor?  ( -- f )  my-location is-outdoor-location?  ;
  \ ¿Está el protagonista en un escenario al aire libre?

: am-i-indoor?  ( -- f )  am-i-outdoor? 0=  ;
  \ ¿Está el protagonista en un escenario cerrado, no al aire libre?

: is-hold?  ( a -- f )  location protagonist~ =  ;
  \ ¿Es el protagonista la localización de un ente?

\ XXX TODO -- write `is-carried` (also hold inside a container);
\ or rename `is-hold` to `is-hold-by-hand`?

: is-not-hold?  ( a -- f )  is-hold? 0=  ;
  \ ¿No es el protagonista la localización de un ente?

: be-hold  ( a -- )  ~location protagonist~ swap !  ;
  \ Hace que el protagonista sea la localización de un ente _a_.

: is-worn-by-me?  ( a -- f )  dup is-hold?  swap is-worn?  and  ;
  \ ¿El protagonista lleva puesto el ente indicado?

: is-not-worn-by-me?  ( a -- f )  is-worn-by-me? 0=  ;

: is-known?  ( a -- f )
  dup belongs-to-protagonist?
  over is-visited? or
  over conversations? or
  swap is-familiar?  or  ;
  \ ¿El protagonista ya conoce el ente?  El resultado depende de
  \ cualquiera de cuatro condiciones: 1) ¿Es propiedad del
  \ protagonista?; 2) ¿Es un escenario ya visitado? (si no es un
  \ escenario, la comprobación no tendrá efecto); 3) ¿Ha hablado ya
  \ con él? (si no es un personaje, la comprobación no tendrá efecto);
  \ 4) ¿O ya le es familiar?.

: is-unknown?  ( a -- f )  is-known? 0=  ;
  \ ¿El protagonista aún no conoce el ente?

: is-here?  ( a -- f )
  dup location am-i-there?
  over is-global-outdoor? am-i-outdoor? and or
  swap is-global-indoor? am-i-indoor? and or  ;
  \ ¿Está un ente en la misma localización que el protagonista?
  \ El resultado depende de cualquiera de tres condiciones:
  \ 1) ¿Está efectivamente en la misma localización?;
  \ 2) ¿Es un «global exterior» y estamos en un escenario exterior?;
  \ 3) ¿Es un «global interior» y estamos en un escenario interior?.

: is-not-here?  ( a -- f )  is-here? 0=  ;
  \ ¿Está un ente en otra localización que la del protagonista?
  \ XXX TODO -- no usado

: is-here-and-unknown?  ( a -- f )  dup is-here? swap is-unknown? and  ;
  \ ¿Está un ente en la misma localización que el protagonista y aún
  \ no es conocido por él?

: be-here  ( a -- )  my-location swap be-there  ;
  \ Hace que un ente esté en la misma localización que el protagonista.

: is-accessible?  ( a -- f )  dup is-hold?  swap is-here?  or  ;
  \ ¿Es un ente accesible para el protagonista?

: is-not-accessible?  ( a -- f )  is-accessible? 0=  ;
  \ ¿Un ente no es accesible para el protagonista?

: must-be-listed?  ( a -- f )
  dup protagonist~ <>  \ ¿No es el protagonista?
  over is-decoration? 0=  and  \ ¿Y no es decorativo?
  over is-listed? and  \ ¿Y puede ser listado?
  swap is-global? 0=  and  ;  \ ¿Y no es global?
  \ ¿El ente debe ser incluido en las listas?
  \ XXX TODO -- inconcluso

: can-be-looked-at?  ( a -- f )
  dup am-i-there?    ?dup if  nip exit  then
  dup is-direction?  ?dup if  nip exit  then
  dup exits~ =       ?dup if  nip exit  then
  is-accessible?  ;
  \ ¿El ente puede ser mirado?

: may-be-climbed?  ( a -- f )
  [false] [if]
  fallen-away~
  bridge~
  arch~
  bed~
  flags~
  rocks~
  table~
  [else]  false
  [then]  ;
  \ ¿El ente podría ser escalado? (Aunque en la práctica no sea posible).
  \ XXX TODO -- hacerlo mejor con un indicador en la ficha

: can-be-sharpened?  ( a -- f )
  dup log~ =  swap sword~ =  or  ;
  \ ¿Puede un ente ser afilado?

: talked-to-the-leader?  ( -- f )  leader~ conversations 0<>  ;
  \ ¿El protagonista ha hablado con el líder?

: do-you-hold-something-forbidden?  ( -- f )
  sword~ is-accessible?  stone~ is-accessible?  or  ;
  \ ¿Llevas algo prohibido?  Este cálculo se usa en varios lugares del
  \ programa, en relación a los refugiados.

: no-torch?  ( -- f )
  torch~ is-not-accessible?  torch~ is-not-lighted?  or  ;
  \ ¿La antorcha no está accesible y encendida?

\ ----------------------------------------------
\ Hacer desaparecer entes

0 constant limbo
  \ Marcador para usar como localización de entes inexistentes.

: vanished?  ( a -- f )  location limbo =  ;
  \ ¿Está un ente desaparecido?

: not-vanished?  ( a -- f )  vanished? 0=  ;
  \ ¿No está un ente desaparecido?

: vanish  ( a -- )  limbo swap be-there  ;
  \ Hace desaparecer un ente llevándolo al «limbo».

: vanish-if-hold  ( a -- )
  dup is-hold? if  vanish  else  drop  then  ;
  \ Hace desaparecer un ente si su localización es el protagonista.
  \ XXX TODO -- no usado

\ ----------------------------------------------
\ Herramientas de artículos y pronombres

\ La selección del artículo adecuado para el nombre de un ente tiene
\ su complicación. Depende por supuesto del número y género gramatical
\ del nombre, pero también de la relación con el protagonista
\ (distinción entre artículos definidos e indefinidos) y de la
\ naturaleza del ente (cosa o personaje).
\
\ Por conveniencia, consideramos como artículos ciertas palabras que
\ son adjetivos (como «esta», «ninguna»...), pues en la práctica para
\ el programa su manejo es idéntico: se usan para preceder a los
\ nombres bajo ciertas condiciones.
\
\ En este mismo apartado definimos palabras para calcular los
\ pronombres de objeto indirecto (le/s) y de objeto directo (la/s,
\ lo/s), así como terminaciones habituales.
\
\ Utilizamos una tabla de cadenas de longitud variable, apuntada por
\ una segunda tabla con sus direcciones.  Esto unifica y simplifica
\ los cálculos.

: hs,  ( ca len -- a1 )  here rot rot s,  ;
  \ Compila una cadena en el diccionario y devuelve su dirección.

s" él" hs, s" ella" hs, s" ellos" hs, s" ellas" hs,
  \ Pronombres personales.
s" este" hs, s" esta" hs, s" estos" hs, s" estas" hs,
  \ Adjetivos que se tratan como «artículos cercanos».
s" ese" hs, s" esa" hs, s" esos" hs, s" esas" hs,
  \ Adjetivos que se tratan como «artículos distantes».
s" ningún" hs, s" ninguna" hs, s" ningunos" hs, s" ningunas" hs,
  \ Adjetivos que se tratan como «artículos negativos».
s" tu" hs, s" tu" hs, s" tus" hs, s" tus" hs,
  \ Artículos posesivos.
s" el" hs, s" la" hs, s" los" hs, s" las" hs,
  \ Artículos definidos.
s" un" hs, s" una" hs, s" unos" hs, s" unas" hs,
  \ Artículos indefinidos.

create 'articles
  \ Tabla índice de los artículos.

  \ Compilar las direcciones donde se han compilado las
  \ cadenas de los artículos:

  , , , ,  \ Indefinidos
  , , , ,  \ Definidos
  , , , ,  \ Posesivos
  , , , ,  \ «Negativos»
  , , , ,  \ «Distantes»
  , , , ,  \ «Cercanos»
  , , , ,  \ Pronombres personales

\ Separaciones entre artículos en la tabla índice (por tanto en
\ celdas):
  cell  constant /article-gender-set  \ De femenino a masculino.
2 cells constant /article-number-set  \ De plural a singular.
4 cells constant /article-type-set    \ Entre grupos de diferente tipo.

: article-number>  ( a -- u )
  has-singular-name? /article-number-set and  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el número gramatical del ente.

: article-gender>  ( a -- u )
  has-masculine-name? /article-gender-set and  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical del ente.

: article-gender+number>  ( a -- u )
  dup article-gender>  swap article-number> +  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical y el número del ente.

: definite-article>  ( a -- u )
  dup has-definite-article?  swap is-known? or
  abs /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos para apuntar a
  \ los artículos definidos si el ente indicado necesita uno.  Las
  \ condiciones son: 1) El ente necesita siempre artículo definido; 2)
  \ El ente es ya conocido por el protagonista.  En cualquiera de
  \ ambos casos _u_ es la longitud de un grupo de artículos (pues los
  \ definidos son el segundo). Si no se cumple alguna de las dos
  \ condiciones, _u_ es cero.

: possesive-article>  ( a -- u )
  belongs-to-protagonist? 2 and  \ Dos grupos (pues los posesivos son el tercero)
  /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos posesivos
  \ si el ente indicado necesita uno.

: negative-articles>  ( -- u )
  3 /article-type-set *  ; \ Tres grupos (pues los negativos son el cuarto)
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos negativos».

: undefined-articles>  ( -- u )  0  ;
  \ Devuelve el desplazamiento en la tabla de artículos para apuntar a
  \ los artículos indefinidos.  El desplazamiento es cero porque los
  \ artículos indefinidos son el primer grupo.

: definite-articles>  ( -- u )  /article-type-set  ;
  \ Devuelve el desplazamiento en la tabla de artículos para apuntar a
  \ los artículos definidos.  Devuelve la longitud de un grupo, pues
  \ los definidos son el segundo grupo.

: distant-articles>  ( -- u )  4 /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos para apuntar a
  \ los «artículos distantes».  Devuelve la longitud de cuatro grupos,
  \ pues los «distantes» son el quinto grupo.

: not-distant-articles>  ( -- u )  5 /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos para apuntar a
  \ los «artículos cercanos».  Devuelve la longitud de cinco grupos,
  \ pues los «cercanos» son el sexto grupo.

: personal-pronouns>  ( -- u )  6 /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos para apuntar a
  \ los pronombres personales.  Devuelve la longitud de seis grupos,
  \ pues los artículos personales son el séptimo grupo.

: article-type  ( a -- u )
  dup definite-article>  swap possesive-article>  max  ;
  \ Devuelve un desplazamiento en la tabla de artículos según el ente
  \ requiera un artículo definido, indefinido o posesivo.

: >article  ( u -- ca len )  'articles + @ count  ;
  \ Devuelve un artículo de la tabla de artículos a partir de su
  \ índice.

: (article)  ( a -- ca len )
  dup article-gender>
  over article-number> +
  swap article-type +  >article  ;
  \ Devuelve el artículo apropiado para un ente _a_,
  \ según el género, el número y el tipo.

: article  ( a -- ca len | a 0 )
  dup has-no-article? if  0  else  (article)  then  ;
  \ Devuelve el artículo apropiado para un ente _a_, si lo necesita;
  \ en caso contrario devuelve una cadena vacía.

: undefined-article  ( a -- ca len )
  article-gender+number> undefined-articles> +
  >article  ;
  \ Devuelve el artículo indefinido correspondiente al género y número
  \ de un ente _a_.

: definite-article  ( a -- ca len )
  article-gender+number> definite-articles> +
  >article  ;
  \ Devuelve el artículo definido
  \ correspondiente al género y número de un ente.

: pronoun  ( a -- ca len )
  definite-article  s" lo" s" el" replaced  ;
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente.

: ^pronoun  ( a -- ca len )  pronoun ^uppercase  ;
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente,
  \ con la primera letra mayúscula.

: negative-article  ( a -- ca len )
  article-gender+number> negative-articles> +  >article  ;
  \ Devuelve el «artículo negativo»
  \ correspondiente al género y número de un ente.

: distant-article  ( a -- ca len )
  article-gender+number> distant-articles> +  >article  ;
  \ Devuelve el «artículo distante»
  \ correspondiente al género y número de un ente.

: not-distant-article  ( a -- ca len )
  article-gender+number> not-distant-articles> +  >article  ;
  \ Devuelve el «artículo cercano»
  \ correspondiente al género y número de un ente.

: personal-pronoun  ( a -- ca len )
  article-gender+number> personal-pronouns> +  >article  ;
  \ Devuelve el pronombre personal
  \ correspondiente al género y número de un ente.

: plural-ending  ( a -- ca len )
  s" s" rot has-plural-name? and  ;
  \ Devuelve la terminación adecuada del plural
  \ para el nombre de un ente.

: plural-ending+  ( ca1 len1 a -- ca2 len2 )  plural-ending s+  ;
  \ Añade a una cadena la terminación adecuada del plural
  \ para el nombre de un ente.

: gender-ending  ( a -- ca len )
  c" oa" swap has-feminine-name? abs 1+ chars + 1  ;
  \ Devuelve la terminación adecuada del género gramatical
  \ para el nombre de un ente.

: gender-ending+  ( ca1 len1 a -- ca2 len2 )  gender-ending s+  ;
  \ Añade a una cadena la terminación adecuada para el género gramatical de un ente.

: noun-ending  ( a -- ca len )
  dup gender-ending rot plural-ending s+  ;
  \ Devuelve la terminación adecuada para el nombre de un ente.

' noun-ending alias adjective-ending

: noun-ending+  ( ca1 len1 a -- ca2 len2 )  noun-ending s+  ;
  \ Añade a una cadena la terminación adecuada para el nombre de un ente.

' noun-ending+ alias adjective-ending+
: direct-pronoun  ( a -- ca len )
  s" l" rot noun-ending s+  ;
  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»).

: ^direct-pronoun  ( a -- ca len )
  direct-pronoun ^uppercase  ;
  \ Devuelve el pronombre de objeto directo para un ente («La/s»
  \ o «Lo/s»), con la primera letra mayúscula.

: indirect-pronoun  ( a -- ca len )
  s" le" rot plural-ending s+  ;
  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»).

: verb-number-ending  ( a -- ca len )
  s" n" rot has-plural-name? and  ;
  \ Devuelve la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.

: verb-number-ending+  ( ca1 len1 a -- ca2 len2 )
  verb-number-ending s+  ;
  \ Añade a una cadena la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.

: proper-verb-form  ( a ca1 len1 -- ca2 len2 )
  rot has-plural-name? *>verb-ending  ;
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular un verbo
  \ cuyo sujeto se indica con el identificador de su entidad.
  \ ca len = Expresión
  \ a = Entidad
  \ XXX TODO -- no usado

: proper-grammar-number  ( a ca1 len1 -- ca2 len2 )
  rot has-plural-name? *>plural-ending  ;
  \ Cambia por «s» (terminación del plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular las palabras de un texto,
  \ cuyo número gramatical se indica con el identificador de una entidad.
  \ ca len = Expresión
  \ a = Entidad
  \ XXX TODO -- no usado

\ ----------------------------------------------
\ Interfaz para los nombres de los entes

: p-name!  ( ca len a -- )  dup have-plural-name name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ y lo marca como plural.

: s-name!  ( ca len a -- )  dup have-singular-name name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ y lo marca como singular.

: fs-name!  ( ca len a -- )  dup have-feminine-name s-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical femenino y singular.

: fp-name!  ( ca len a -- )  dup have-feminine-name p-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical femenino y plural.

: ms-name!  ( ca len a -- )  dup have-masculine-name s-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical masculino y singular.

: mp-name!  ( ca len a -- )  dup have-masculine-name p-name!  ;
  \ Guarda el nombre _ca len_ de un ente _a_,
  \ indicando también que es de género gramatical masculino y plural.

: ^name  ( a -- ca len )  name ^uppercase  ;
  \ Devuelve el nombre de un ente, con la primera letra mayúscula.

: name&  ( a ca1 len1 -- ca2 len2 )  rot name txt+  ;
  \ Añade a un (supuesto) artículo _ca1 len1_ el nombre de un ente _a_,
  \ formando el nombre completo _ca2 len2_.

: full-name  ( a -- ca len )  dup article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con el artículo que le corresponda.

: ^full-name  ( a -- ca len )  full-name ^uppercase  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con el artículo que le corresponda (con la primera letra en mayúscula).

: defined-full-name  ( a -- ca len )  dup definite-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un artículo definido.

: undefined-full-name  ( a -- ca len )  dup undefined-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un artículo indefinido.

: negative-full-name  ( a -- ca len )  dup negative-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un «artículo negativo».

: distant-full-name  ( a -- ca len )  dup distant-article name&  ;
  \ Devuelve el nombre completo _ca len_ de un ente _a_,
  \ con un «artículo distante».

: human-subjective-negative-name  ( a -- ca len )
  dup is-known? if  full-name  else  drop s" nadie"  then  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente
  \ (humano) _a_, desde el punto de vista del protagonista.

: subjective-negative-name  ( a -- ca len )
  dup is-human?
  if    human-subjective-negative-name
  else  negative-full-name  then  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente _a_,
  \ desde el punto de vista del protagonista.

: /l$  ( a -- ca len )  s" l" rot has-personal-name? 0= and  ;
  \ Devuelve en _ca len_ la terminación «l» del artículo determinado
  \ masculino para añadirla a la preposición «a», si un ente humano
  \ _a_ lo requiere para ser usado como objeto directo; o una cadena
  \ vacía.
  \ XXX TODO -- no usado

: a/$  ( a -- ca len )  s" a" rot is-human? and  ;
  \ Devuelve la preposición «a» en _ca len_ si un ente _a_ lo requiere
  \ para ser usado como objeto directo; o una cadena vacía.

: a/l$  ( a -- ca len )  a/$ dup if  /l$ s+  then  ;
  \ Devuelve la preposición «a» en _ca len_, con posible artículo
  \ determinado, si un ente _a_ lo requiere para ser usado como objeto
  \ directo.
  \ XXX TODO -- no usado

: subjective-negative-name-as-direct-object  ( a -- ca len )
  dup a/$ rot subjective-negative-name txt+  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente _a_,
  \ desde el punto de vista del protagonista, para ser usado como
  \ objeto directo.

: full-name-as-direct-complement  ( a -- ca len )
  dup s" a" rot is-human? and
  rot full-name txt+
  s" al" s" a el" replaced  ;
  \ Devuelve el nombre completo de un ente en función de complemento
  \ directo.  Esto es necesario para añadir la preposición «a» a las
  \ personas.

\ ----------------------------------------------
\ Otros campos calculados

: «open»|«closed»  ( a -- ca len )
  dup is-open? if  s" abiert"  else  s" cerrad"  then
  rot noun-ending s+  ;
  \ Devuelve en _ca len_ «abierto/a/s» o «cerrado/a/s»,
  \ según corresponda a un ente _a_.

: was-the-cave-entrance-discovered?  ( -- f )
  location-08~ has-south-exit?  ;
  \ ¿La entrada a la cueva ya fue descubierta?

\ ==============================================================
\ Herramientas para crear conexiones entre escenarios

\ XXX TODO -- move to Flibustre when possible

0 [if]  \ XXX TODO -- inconcluso

create opposite-exits
south-exit> ,
north-exit> ,
west-exit> ,
east-exit> ,
down-exit> ,
up-exit> ,
in-exit> ,
out-exit> ,

create opposite-direction-entities
south~ ,
north~ ,
west~ ,
east~ ,
down~ ,
up~ ,
in~ ,
out~ ,

[then]

\ Necesitamos una tabla que nos permita traducir esto:
\
\ ENTRADA: Un puntero correspondiente a un campo de dirección
\ de salida en la ficha de un ente.
\
\ SALIDA: El identificador del ente dirección al que se
\ refiere esa salida.

create exits-table  #exits cells allot
  \ Tabla de traducción de salidas.

: >exits-table>  ( n -- a )  first-exit> - exits-table +  ;
  \ Convierte el campo de dirección _n_ (por tanto, un desplazamiento
  \ relativo al inicio de la ficha de un ente) en la dirección _a_ del
  \ ente dirección correspondiente en la tabla.

: exits-table!  ( a n -- )  >exits-table> !  ;
  \ Guarda un ente _a_ en una posición _n_ de la tabla de salidas,
  \ siendo _n_ también un campo de dirección (por tanto, un
  \ desplazamiento relativo al inicio de la ficha de un ente).

: exits-table@  ( n -- a )  >exits-table> @  ;
  \ Convierte el campo de dirección _n_ (por tanto, un desplazamiento
  \ relativo al inicio de la ficha de un ente) en la dirección _a_ del
  \ ente dirección correspondiente en la tabla.

\ Rellenar cada elemento de la tabla con un ente de salida, usando
\ como puntero el campo análogo de la ficha.  Haciéndolo de esta
\ manera no importa el orden en que se rellenen los elementos.

north~ north-exit> exits-table!
south~ south-exit> exits-table!
east~ east-exit> exits-table!
west~ west-exit> exits-table!
up~ up-exit> exits-table!
down~ down-exit> exits-table!
out~ out-exit> exits-table!
in~ in-exit> exits-table!

0 [if]  \ XXX TODO -- inconcluso
: opposite-exit  ( a1 -- a2 )
  first-exit> - opposite-exits + @  ;
  \ Devuelve la dirección cardinal opuesta a la indicada.

: opposite-exit~  ( a1 -- a2 )
  first-exit> - opposite-direction-entities + @  ;
  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada.
  \ a1 = entidad de dirección
  \ a2 = entidad de dirección, opuesta a a1

[then]

require talanto/location_connectors.fs

\ Por último, definimos dos palabras para hacer
\ todas las asignaciones de salidas en un solo paso.

: set-exits  ( a1 ... a8 a0 -- )
  >r r@ ~out-exit !
     r@ ~in-exit !
     r@ ~down-exit !
     r@ ~up-exit !
     r@ ~west-exit !
     r@ ~east-exit !
     r@ ~south-exit !
     r> ~north-exit !  ;
  \ Asigna todas las salidas _a1 ... a8_ de un ente escenario _a0_.
  \ Los entes de salida _a1 ... a8_ (o cero) están en el orden
  \ habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera.

: exit-from-here  ( a1 -- a2 | 0 )
  direction my-location + @  ;
  \ Devuelve el ente _a2_ al que conduce el ente dirección _a1_ desde
  \ el escenario del protagonista, o bien devuelve cero si no hay
  \ salida en esa dirección.


\ ==============================================================
\ Operaciones con conexiones entre escenarios

: open-the-cave-entrance  ( -- )
  location-08~ dup location-10~ s<-->  location-10~ i<-->  ;
  \ Comunica el escenario 8 con el 10 (de dos formas y en ambos sentidos).

\ ==============================================================
\ Cálculos para averiguar complemento omitido

: whom  ( -- a | 0 )
  ambrosio~ dup is-here? and ?dup ?exit
  leader~   dup is-here? and ?dup ?exit
  false  ;
  \ Devuelve un ente personaje _a_ al que probablemente se refiera un
  \ comando.  Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica.
  \
  \ XXX TODO -- ampliar para contemplar los soldados y oficiales,
  \ según la trama, el escenario y la fase de la batalla

: unknown-whom  ( -- a | 0 )
  ambrosio~ dup is-here-and-unknown? and ?dup ?exit
  leader~   dup is-here-and-unknown? and ?dup ?exit
  false  ;
  \ Devuelve un ente personaje desconocido al que probablemente se
  \ refiera un comando.  Se usa para averiguar el objeto de algunas
  \ acciones cuando el jugador no lo especifica

\ ==============================================================
\ Change log

\ 2017-08-05: Move the location connectors to Talanto.

\ vim:filetype=gforth:fileencoding=utf-8
