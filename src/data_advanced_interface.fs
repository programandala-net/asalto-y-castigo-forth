\ data_advanced_interface.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607041144

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Interfaz de datos avanzada

\ Esta interfaz de datos depende de algunos identificadores de entes.

: belongs-to-protagonist?  ( a -- f )  protagonist~ is-owner?  ;
: belongs-to-protagonist  ( a -- )  protagonist~ be-owner  ;

: taken  ( a -- )  protagonist~ swap be-there  ;
  \ Hace que el protagonista sea la localización de un ente _a_.

: was-there  ( a1 a2 -- )  ~previous-location !  ;
  \ Hace que el ente _a1_ sea la localización previa del ente _a2_.

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

: is-not-hold?  ( a -- f )  is-hold? 0=  ;
  \ ¿No es el protagonista la localización de un ente?

: be-hold  ( a -- )  ~location protagonist~ swap !  ;
  \ Hace que el protagonista sea la localización de un ente.

: is-worn-by-me?  ( a -- f )  dup is-hold?  swap is-worn?  and  ;
  \ ¿El protagonista lleva puesto el ente indicado?

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

: can-be-looked-at?  ( a -- f )
  [false] [if]
    \ XXX OLD -- Primera versión
    dup am-i-there?         \ ¿Es la localización del protagonista?
    over is-direction? or   \ ¿O es un ente dirección?
    over exits~ = or        \ ¿O es el ente "salidas"?
    swap is-accessible? or  \ ¿O está accesible?
  [then]
  [false] [if]
    \ XXX OLD -- Segunda versión, menos elegante pero más rápida y legible
    { entity }
    true case
      entity am-i-there?    of  true  endof  \ ¿Es la localización del protagonista?
      entity is-direction?  of  true  endof  \ ¿Es un ente dirección?
      entity is-accessible? of  true  endof  \ ¿Está accesible?
      entity exits~ =       of  true  endof  \ ¿Es el ente "salidas"?
      false swap
    endcase
  [then]
  [true] [if]
    \ XXX NEW -- Tercera versión, más rápida y compacta
    dup am-i-there?    ?dup if  nip exit  then
      \ ¿Es la localización del protagonista?
    dup is-direction?  ?dup if  nip exit  then
      \ ¿Es un ente dirección?
    dup exits~ =       ?dup if  nip exit  then
      \ ¿Es el ente "salidas"?
    is-accessible?
      \ ¿Está accesible?
  [then]  ;
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
  \ ¿Llevas algo prohibido?
  \ Cálculo usado en varios lugares del programa,
  \ en relación a los refugiados.

: no-torch?  ( -- f )
  torch~ is-not-accessible?  torch~ is-not-lit?  or  ;
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
  dup has-definite-article?  \ Si el ente necesita siempre artículo definido
  swap is-known? or  \ O bien si el ente es ya conocido por el protagonista
  abs  \ Un grupo (pues los definidos son el segundo)
  /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos
  \ si el ente indicado necesita uno.

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

: undefined-articles>  ( -- u )
  0  ; \ Desplazamiento cero, pues los indefinidos son el primer grupo.
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos indefinidos.

: definite-articles>  ( -- u )
  /article-type-set  ;  \ Un grupo, pues los definidos son el segundo
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos.

: distant-articles>  ( -- u )
  4 /article-type-set *  ;  \ Cuatro grupos, pues los «distantes» son el quinto
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos distantes».

: not-distant-articles>  ( -- u )
  5 /article-type-set *  ;  \ Cinco grupos, pues los «cercanos» son el sexto
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos cercanos».

: personal-pronouns>  ( -- u )
  6 /article-type-set *  ;
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los pronombres personales.

: article-type  ( a -- u )
  dup definite-article>  swap possesive-article>  max  ;
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el ente requiera un artículo definido, indefinido o posesivo.

: >article  ( u -- ca1 len1 )  'articles + @ count  ;
  \ Devuelve un artículo de la tabla de artículos
  \ a partir de su índice.

: (article)  ( a -- ca1 len1 )
  dup article-gender>  \ Desplazamiento según el género
  over article-number> +  \ Sumado al desplazamiento según el número
  swap article-type +  \ Sumado al desplazamiento según el tipo
  >article  ;
  \ Devuelve el artículo apropiado para un ente.

: article  ( a -- ca1 len1 | a 0 )
  dup has-no-article? if  0  else  (article)  then  ;
  \ Devuelve el artículo apropiado para un ente, si lo necesita;
  \ en caso contrario devuelve una cadena vacía.

: undefined-article  ( a -- ca1 len1 )
  article-gender+number> undefined-articles> +
  >article  ;
  \ Devuelve el artículo indefinido
  \ correspondiente al género y número de un ente.

: definite-article  ( a -- ca1 len1 )
  article-gender+number> definite-articles> +
  >article  ;
  \ Devuelve el artículo definido
  \ correspondiente al género y número de un ente.

: pronoun  ( a -- ca1 len1 )
  definite-article  s" lo" s" el" replaced  ;
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente.

: ^pronoun  ( a -- ca1 len1 )  pronoun ^uppercase  ;
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente,
  \ con la primera letra mayúscula.

: negative-article  ( a -- ca1 len1 )
  article-gender+number> negative-articles> +  >article  ;
  \ Devuelve el «artículo negativo»
  \ correspondiente al género y número de un ente.

: distant-article  ( a -- ca1 len1 )
  article-gender+number> distant-articles> +  >article  ;
  \ Devuelve el «artículo distante»
  \ correspondiente al género y número de un ente.

: not-distant-article  ( a -- ca1 len1 )
  article-gender+number> not-distant-articles> +  >article  ;
  \ Devuelve el «artículo cercano»
  \ correspondiente al género y número de un ente.

: personal-pronoun  ( a -- ca1 len1 )
  article-gender+number> personal-pronouns> +  >article  ;
  \ Devuelve el pronombre personal
  \ correspondiente al género y número de un ente.

: plural-ending  ( a -- ca1 len1 )
  [false] [if]
    \ XXX OLD -- Método 1, «estilo BASIC»:
    has-plural-name? if  s" s"  else  null$  then
  [else]
    \ XXX NEW -- Método 2, sin estructuras condicionales, «estilo Forth»:
    s" s" rot has-plural-name? and
  [then]  ;
  \ Devuelve la terminación adecuada del plural
  \ para el nombre de un ente.

: plural-ending+  ( ca1 len1 a -- ca2 len2 )  plural-ending s+  ;
  \ Añade a una cadena la terminación adecuada del plural
  \ para el nombre de un ente.

: gender-ending  ( a -- ca1 len1 )
  [false] [if]
    \ Método 1, «estilo BASIC»
    has-feminine-name? if  s" a"  else  s" o"  then
  [else]
    [false] [if]
      \ Método 2, sin estructuras condicionales, «estilo Forth»
      s" oa" drop swap has-feminine-name? abs + 1
    [else]
      \ Método 3, similar, más directo
      c" oa" swap has-feminine-name? abs + 1+ 1
    [then]
  [then]  ;
  \ Devuelve la terminación adecuada del género gramatical
  \ para el nombre de un ente.

: gender-ending+  ( ca1 len1 a -- ca2 len2 )  gender-ending s+  ;
  \ Añade a una cadena la terminación adecuada para el género gramatical de un ente.

: noun-ending  ( a -- ca1 len1 )
  dup gender-ending rot plural-ending s+  ;
  \ Devuelve la terminación adecuada para el nombre de un ente.

' noun-ending alias adjective-ending

: noun-ending+  ( ca1 len1 a -- ca2 len2 )  noun-ending s+  ;
  \ Añade a una cadena la terminación adecuada para el nombre de un ente.

' noun-ending+ alias adjective-ending+
: direct-pronoun  ( a -- ca1 len1 )
  s" l" rot noun-ending s+  ;
  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»).

: ^direct-pronoun  ( a -- ca1 len1 )
  direct-pronoun ^uppercase  ;
  \ Devuelve el pronombre de objeto directo para un ente («La/s»
  \ o «Lo/s»), con la primera letra mayúscula.

: indirect-pronoun  ( a -- ca1 len1 )
  s" le" rot plural-ending s+  ;
  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»).

: verb-number-ending  ( a -- ca1 len1 )
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

\ Como ya se explicó, el nombre de cada ente se guarda en una
\ cadena dinámica [que se crea en la memoria con `allocate`, no
\ en el espacio del diccionario del sistema].  El manejo de
\ estas cadenas dinámicas se hace con el módulo
\ correspondiente de Forth Foundation Library.
\
\ En la ficha del ente se guarda solo la dirección de la
\ cadena dinámica, en el campo `~name-str`.  Por ello hacen
\ falta palabras que hagan de interfaz para gestionar los
\ nombres de ente de forma análoga a como se hace con el resto
\ de datos de su ficha.

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

: ?name  ( a -- ca len )  ?dup if  name  else  null$  then  ;
  \ Devuelve el nombre _ca len_ de un ente _a_, si es tal;
  \ devuelve una cadena vacía si _a_ es cero.
  \ XXX TMP -- solo se usa para depuración

: ?.name  ( a|0 -- )  ?name type  ;
  \ Imprime el nombre de un ente _a_, si es tal;
  \ imprime una cadena vacía si _a_ es cero.
  \ XXX TMP -- solo se usa para depuración

: ^name  ( a -- ca1 len1 )  name ^uppercase  ;
  \ Devuelve el nombre de un ente, con la primera letra mayúscula.

: name&  ( a ca1 len1 -- ca2 len2 )  rot name s&  ;
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

: nonhuman-subjective-negative-name  ( a -- ca len )
  negative-full-name 2>r
  s{  2r> 2dup 2dup  \ Tres nombres repetidos con «artículo negativo»
      s" eso" s" esa cosa" s" tal cosa"  \ Tres alternativas
  }s  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente (no
  \ humano) _a_, desde el punto de vista del protagonista.  Nota: En
  \ este caso hay que usar `negative-full-name` antes de `s{` y pasar
  \ la cadena mediante la pila de retorno; de otro modo `s{` y `}s` no
  \ pueden calcular bien el crecimiento de la pila.

: human-subjective-negative-name  ( a -- ca len )
  dup is-known? if  full-name  else  drop s" nadie"  then  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente
  \ (humano) _a_, desde el punto de vista del protagonista.

: subjective-negative-name  ( a -- ca len )
  dup is-human?
  if  human-subjective-negative-name
  else  nonhuman-subjective-negative-name  then  ;
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
  dup a/$ rot subjective-negative-name s&  ;
  \ Devuelve el nombre subjetivo (negativo) _ca len_ de un ente _a_,
  \ desde el punto de vista del protagonista, para ser usado como
  \ objeto directo.

\ ----------------------------------------------
\ Otros campos calculados

: «open»|«closed»  ( a -- ca len )
  dup is-open? if  s" abiert"  else  s" cerrad"  then
  rot noun-ending s+  ;
  \ Devuelve en _ca len_ «abierto/a/s» o «cerrado/a/s»,
  \ según corresponda a un ente _a_.

\ vim:filetype=gforth:fileencoding=utf-8
