\ parser.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607062027

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Variables del intérprete de comandos

0 value action  ( -- xt | 0 )
  \ Código de la acción del comando.

0 value previous-action  ( -- xt | 0 )
  \ Código de la acción del comando anterior.

0 value main-complement  ( -- a )
  \ Ente _a_ complemento principal (complemento directo o destino).

0 value secondary-complement  ( -- a )
  \ Ente _a_ complemento secundario (complemento indirecto, destino u
  \ origen).

defer tool-complement  ( -- a )
  \ Ente _a_ complemento instrumental (indicada con «con» o «usando»).

defer explicit-tool-complement  ( -- a )
  \ Ente _a_ complemento instrumental estricto (indicado con «usando»).

defer company-complement  ( -- a )
  \ Ente _a_ complemento de compañía (indicado con «con»).

false [if]
  \ XXX OLD
  \ XXX TODO -- descartado, pendiente
  0 value to-complement  \ Destino \ XXX OLD -- no utilizado
  0 value from-complement  \ Origen \ XXX OLD -- no utilizado
  0 value into-complement  \ Destino dentro \ XXX OLD -- no utilizado
[then]

variable what
  \ Ente que ha provocado un error y puede ser citado en el mensaje de
  \ error correspondiente.

variable current-preposition
  \ Código de la (seudo)preposición abierta, o cero.

variable used-prepositions
  \ Máscara de bitios de las (seudo)preposiciones usadas en la frase.
  \ XXX REMARK -- solo se usa para depuración

\ ==============================================================
\ Pronombres

0 value last-action
  \ Última acción utilizada por el jugador.
  \ XXX TODO -- no usado

\ La tabla `last-complement` que crearemos a continuación sirve para
\ guardar los identificadores de entes correspondientes a los últimos
\ complementos utilizados en los comandos del jugador. De este modo los
\ pronombres podrán recuperarlos.
\
\ Necesita cinco celdas: una para el último complemento usado y cuatro
\ para cada último complemento usado de cada género y número.  El espacio
\ se multiplica por dos para guardar en la segunda mitad los penúltimos
\ complementos.
\
\ La estructura de la tabla es la siguiente, con desplazamientos
\ indicados en celdas:

\ Último complemento usado:
\   +0 De cualquier género y número.
\   +1 Masculino singular.
\   +2 Femenino singular.
\   +3 Masculino plural.
\   +4 Femenino plural.

\ Penúltimo complemento usado:
\   +5 De cualquier género y número.
\   +6 Masculino singular.
\   +7 Femenino singular.
\   +8 Masculino plural.
\   +9 Femenino plural.

5 cells 2* constant /last-complements
  \ Octetos necesarios para la tabla
  \ de últimos complementos usados.

create last-complement /last-complements allot
  \ Tabla de últimos complementos usados.

\ Desplazamientos para acceder a los elementos de la tabla:
1 cells constant />masculine-complement  \ Respecto al inicio de tabla
2 cells constant />feminine-complement  \ Respecto al inicio de tabla
0 cells constant />singular-complement  \ Respecto a su género en singular
2 cells constant />plural-complement  \ Respecto a su género en singular
5 cells constant />but-one-complement  \ Respecto a la primera mitad de la tabla
  \ XXX TODO -- mejorar comentarios

: >masculine  ( a1 -- a2 )  />masculine-complement +  ;
: >feminine  ( a1 -- a2 )  />feminine-complement +  ;
: >singular  ( a1 -- a2 )  />singular-complement +  ;
: >plural  ( a1 -- a2 )  />plural-complement +  ;
: >but-one  ( a1 -- a2 )  />but-one-complement +  ;

: last-but-one-complement  ( - a )  last-complement >but-one  ;
  \ Devuelve la dirección del penúltimo complemento absoluto,
  \ que es también el inicio de la sección «penúltimos»
  \ de la tabla `last-complements`.

: (>last-complement)  ( a1 a2 -- a3 )
  over has-feminine-name? />feminine-complement and +
  over has-masculine-name? />masculine-complement and +
  swap has-plural-name? />plural-complement and +  ;
  \ Apunta a la dirección adecuada para un ente
  \ en una sección de la tabla `last-complement`,
  \ bien «últimos» o «penúltimos».
  \ Nota: Hace falta sumar los desplazamientos de ambos géneros
  \ debido a que ambos son respecto al inicio de la tabla.
  \ El desplazamiento para singular no es necesario,
  \ pues sabemos que es cero, a menos que se cambie la estructura.
  \ a1 = Ente para el que se calcula la dirección
  \ a2 = Dirección de una de las secciones de la tabla

: >last-complement  ( a1 -- a2 )
  last-complement (>last-complement)  ;
  \ Apunta a la dirección adecuada para un ente
  \ en la sección «últimos» de la tabla `last-complement`.

: >last-but-one-complement  ( a1 -- a2 )
  last-but-one-complement (>last-complement)  ;
  \ Apunta a la dirección adecuada para un ente
  \ en la sección «penúltimos» de la tabla `last-complement`.

: erase-last-command-elements  ( -- )
  false to last-action
  last-complement /last-complements erase  ;
  \ Borra todos los últimos elementos guardados de los comandos.

\ ==============================================================
\ Errores del intérprete de comandos

: please$  ( -- ca len )
  \ Devuelve «por favor» o vacía.
  s" por favor" s?  ;

: (please&)  ( ca1 len1 ca2 len2 -- ca3 len3 )
  2 random ?? 2swap  comma+ 2swap s&  ;
  \ Añade una cadena _ca2 len2_ al inicio o al final de una cadena
  \ _ca1 len1_, con una coma de separación.

: please&  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  please$ dup if  (please&)  else  2drop  then  ;
  \ Añade «por favor» al inicio o al final de una cadena _ca1 len1_,
  \ con una coma de separación; o bien la deja sin tocar.

: in-the-sentence$  ( -- ca len )
  s{ null$ s" en la frase" s" en el comando" s" en el texto" }s  ;
  \ Devuelve una variante de «en la frase» (o una cadena vacía).

: error-comment-0$  ( -- ca len )
  s" sé más clar" player-gender-ending$+  ;
  \ Devuelve la variante 0 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment-1$  ( -- ca len )
  s{ s" exprésate" s" escribe" }s
  s{
  s" más claramente"
  s" más sencillamente"
  s{ s" con más" s" con mayor" }s
  s{ s" sencillez" s" claridad" }s&
  }s s&  ;
  \ Devuelve la variante 1 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment-2-start$  ( -- ca len )
  s{ s" intenta" s" procura" s" prueba a" }s
  s{ s" reescribir" s" expresar" s" escribir" s" decir" }s&
  \ XXX TODO -- este "lo" crea problema de concordancia con el final de la frase:
  s{ s"  la frase" s" lo" s"  la idea" }s+  ;
  \ Devuelve el comienzo de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2-end-0$  ( -- ca len )
  s{ s" de" s" otra" }s way$ s&?
  s{ null$ s" un poco" s" algo" }s& s" más" s&
  s{ s" simple" s" sencilla" s" clara" }s&  ;
  \ Devuelve el final 0 de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2-end-1$  ( -- ca len )
  s{ s" más claramente" s" con más sencillez" }s  ;
  \ Devuelve el final 1 de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2$  ( -- ca len )
  error-comment-2-start$
  s{ error-comment-2-end-0$ error-comment-2-end-1$ }s&  ;
  \ Devuelve la variante 2 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment$  ( -- ca len )
  error-comment-0$ error-comment-1$ error-comment-2$
  3 schoose please&  ;
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos.

: ^error-comment$  ( -- ca len )  error-comment$ ^uppercase  ;
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula.

: language-error-specific-message  ( ca len -- )
  in-the-sentence$ s&  3 random
  if    ^uppercase period+ ^error-comment$
  else  ^error-comment$ comma+ 2swap
  then  period+ s&  (language-error)  ;
  \ Muestra un mensaje detallado _ca len_ sobre un error lingüístico,
  \ combinándolo con una frase común.
  \ XXX TODO -- hacer que use coma o punto y coma, al azar

: language-error-general-message$  ( -- ca len )
  'language-error-general-message$ count  ;
  \ Devuelve el mensaje de error lingüístico para el nivel 1.

: language-error-general-message  ( ca len -- )
  2drop language-error-general-message$ (language-error)  ;
  \ Muestra el mensaje de error lingüístico _ca len_ para el nivel 1.

create 'language-error-verbosity-xt
  ' 2drop ,
  ' language-error-general-message ,
  ' language-error-specific-message ,
  \ Tabla de los tres niveles de detalle de los errores lingüísticos:
  \ ningún mensaje, mensaje genérico y mensaje específico.

: language-error  ( ca len -- )
  'language-error-verbosity-xt
  language-errors-verbosity @ cells + perform  ;
  \ Muestra un mensaje sobre un error lingüístico, detallado o breve
  \ según la configuración.  _ca len_ es el mensaje de error
  \ detallado.

: there-are$  ( -- ca len )
  s{ s" parece haber" s" se identifican" s" se reconocen" }s  ;
  \ Devuelve una variante de «hay» para sujeto plural, comienzo de
  \ varios errores.

: there-is$  ( -- ca len )
  s{ s" parece haber" s" se identifica" s" se reconoce" }s  ;
  \ Devuelve una variante de «hay» para sujeto singular, comienzo de
  \ varios errores.

: there-is-no$  ( -- ca len )
  s" no se" s{ s" identifica" s" encuentra" s" reconoce" }s&
  s{ s" el" s" ningún" }s&  ;
  \ Devuelve una variante de «no hay», comienzo de varios errores.

:noname  ( -- )
  s{ there-are$ s" dos verbos" s&
  there-is$ s" más de un verbo" s&
  there-are$ s" al menos dos verbos" s&
  }s  language-error  ; to too-many-actions-error#
  \ Informa de que se ha producido un error porque hay dos verbos en
  \ el comando.

:noname  ( -- )
  s{
  there-are$
  s" dos complementos secundarios" s&
  there-is$
  s" más de un complemento secundario" s&
  there-are$
  s" al menos dos complementos secundarios" s&
  }s  language-error  ; to too-many-complements-error#
  \ Informa de que se ha producido un error
  \ porque hay dos complementos secundarios en el comando.
  \ XXX TMP

:noname  ( -- )
  there-is-no$ s" verbo" s&
  language-error  ; to no-verb-error#
  \ Informa de que se ha producido un error por falta de verbo en el comando.

:noname  ( -- )
  there-is-no$ s" complemento principal" s&
  language-error  ; to no-main-complement-error#
  \ Informa de que se ha producido un error por falta de complemento
  \ principal en el comando.

:noname  ( -- )
  there-is$ s" un complemento principal" s&
  s" pero el verbo no puede llevarlo" s&
  language-error  ; to unexpected-main-complement-error#
  \ Informa de que se ha producido un error por la presencia de
  \ complemento principal en el comando.

:noname  ( -- )
  there-is$ s" un complemento secundario" s&
  s" pero el verbo no puede llevarlo" s&
  language-error  ; to unexpected-secondary-complement-error#
  \ Informa de que se ha producido un error por la presencia de
  \ complemento secundario en el comando.

:noname  ( -- )
  there-is$ s" un complemento principal no permitido con esta acción" s&
  language-error  ; to not-allowed-main-complement-error#
  \ Informa de que se ha producido un error por la presencia de un
  \ complemento principal en el comando que no está permitido.

:noname  ( -- )
  there-is$ s" un complemento principal no permitido con esta acción" s&
  language-error  ; to not-allowed-tool-complement-error#
  \ Informa de que se ha producido un error por la presencia de un
  \ complemento instrumental en el comando que no está permitido.

:noname  ( -- )
  s" [Con eso no puedes]"
  narrate  ; to useless-tool-error#
  \ Informa de que se ha producido un error
  \ porque una herramienta no especificada no es la adecuada.
  \ XXX TODO -- inconcluso

:noname  ( -- )
  s" [Con" what @ full-name s& s" no puedes]" s&
  narrate  ; to useless-what-tool-error#
  \ Informa de que se ha producido un error
  \ porque el ente `what` no es la herramienta adecuada.
  \ XXX TODO -- inconcluso
  \ XXX TODO -- distinguir si la llevamos, si está presente, si es conocida...

:noname  ( -- )
  there-is$ s" un complemento (seudo)preposicional sin completar" s&
  language-error  ; to unresolved-preposition-error#
  \ Informa de que se ha producido un error
  \ porque un complemento (seudo)preposicional quedó incompleto.

:noname  ( -- )
  there-is$ s" una (seudo)preposición repetida" s&
  language-error  ; to repeated-preposition-error#
  \ Informa de que se ha producido un error por
  \ la repetición de una (seudo)preposición.

' ?execute alias ?wrong  ( xt | 0 -- )
  \ Informa, si es preciso, de un error en el comando.  _xt_ es tanto
  \ la palabra que muestra el error como el código del error.

\ ==============================================================
\ Intérprete de comandos

\ Gracias al uso del propio intérprete de Forth como intérprete de
\ comandos del juego, más de la mitad del trabajo ya está hecha por
\ anticipado. Para ello basta crear las palabras del vocabulario del
\ juego como palabras propias de Forth y hacer que Forth interprete
\ directamente la entrada del jugador. Creando las palabras en una
\ lista de palabras de Forth específica para ellas, y haciendo que sea
\ la única lista activa en el momento de la interpretación, solo las
\ palabras del juego serán reconocidas, no las del programa ni las del
\ sistema Forth.
\
\ Sin embargo hay una consideración importante: Al pasarle directamente
\ al intérprete de Forth el texto del comando escrito por el jugador,
\ Forth ejecutará las palabras que reconozca (haremos que las no
\ reconocidas las ignore) en el orden en que estén escritas en la frase.
\ Esto quiere decir que, al contrario de lo que ocurre con otros
\ métodos, no podremos tener una visión global del comando del jugador:
\ ni de cuántas palabras consta ni, en principio, qué viene a
\ continuación de la palabra que está siendo interpretada en cada
\ momento.
\
\ Una solución sería que cada palabra del jugador guardara un
\ identificador unívoco en la pila o en una tabla, y posteriormente
\ interpretáramos el resultado de una forma convencional.
\
\ Sin embargo, hemos optado por dejar a Forth hacer su trabajo hasta el
\ final, pues nos parece más sencillo y eficaz [también es más propio
\ del espíritu de Forth usar su intérprete como intérprete de la
\ aplicación en lugar de programar un intérprete adicional específico].
\ Las palabras reconocidas en el comando del jugador se ejecutarán pues
\ en el orden en que fueron escritas. Cada una actualizará el elemento
\ del comando que represente, verbo o complemento, tras comprobar si ya
\ ha habido una palabra previa que realice la misma función y en su caso
\ deteniendo el proceso con un error.

variable prepositions  prepositions off
  \ Número de (seudo)preposiciones.

: >bit  ( u1 -- u2 )  1 swap lshift  ;
  \ Devuelve un número _u2_ cuyo único bitio activo es aquel cuyo
  \ orden indica _u1_ (ej. 0->0, 1->1, 2->2, 3->4, 4->8...).

: preposition:  ( "name1" "name2" -- )
  prepositions @ >bit constant
  prepositions ++ prepositions @ constant  ;
  \ Crea los identificadores de una (seudo)preposición (_name1_:
  \ nombre del identificador para usar como máscara de bitios;
  \ _name2_: nombre del identificador para usar como índice de tabla)
  \ y actualiza el contador.
  \ XXX REMARK -- _name1_ solo se usa para depuración

\ Constantes para los identificadores de (seudo)preposiciones:

preposition: «con»-preposition-bit «con»-preposition#
preposition: «usando»-preposition-bit «usando»-preposition#

false [if]  \ XXX TODO -- inconcluso
  preposition: «a»-preposition-bit «a»-preposition#
  preposition: «contra»-preposition-bit «contra»-preposition#
  preposition: «de»-preposition-bit «de»-preposition#
  preposition: «en»-preposition-bit «en»-preposition#
  preposition: «hacia»-preposition-bit «hacia»-preposition#
  preposition: «para»-preposition-bit «para»-preposition#
  preposition: «por»-preposition-bit «por»-preposition#
  preposition: «sin»-preposition-bit «sin»-preposition#
  \ XXX REMARK: «sin» servirá para dejar cosas antes de la acción.
[then]

prepositions @ cells constant /prepositional-complements
  \ Octetos necesarios para guardar las (seudo)preposiciones en la
  \ tabla.

create prepositional-complements /prepositional-complements allot
  \ Tabla de complementos (seudo)preposicionales.

\ Las (seudo)preposiciones permitidas en el juego pueden tener usos
\ diferentes, y algunos de ellos dependen del ente al que se refieran,
\ por lo que su análisis hay que hacerlo en varios niveles.
\
\ Decimos «(seudo)preposiciones» porque algunos de los términos usados
\ como preposiciones no lo son [como por ejemplo «usando», que es un
\ gerundio] pero se usan como si lo fueran.
\
\ Los identificadores creados arriba se refieren a (seudo)preposiciones
\ del vocabulario de juego (por ejemplo, «a», «con»...) o a sus
\ sinónimos, no a sus posibles usos finales como complementos [por
\ ejemplo, destino de movimiento, objeto indirecto, herramienta,
\ compañía...]. Por ejemplo, el identificador `«a»-preposition` se usa
\ para indicar (en la tabla) que se ha encontrado la preposición «a» [o
\ su sinónimo «al»], pero el significado efectivo [por ejemplo, indicar
\ una dirección o un objeto indirecto o un objeto directo de persona, en
\ este caso] se calculará en una etapa posterior.
\
\ Cada elemento de la tabla de complementos (seudo)preposicionales
\ representa una (seudo)preposición [incluidos evidentemente sus
\ sinónimos]; será apuntado pues por un identificador de
\ (seudo)preposición y contendrá el identificador del ente que haya sido
\ usado en el comando con dicha (seudo)preposición, o bien cero si la
\ (seudo)preposición no ha sido utilizada hasta el momento.

: erase-prepositional-complements  ( -- )
  prepositional-complements /prepositional-complements erase  ;
  \ Borra la tabla de complementos (seudo)preposicionales.

: prepositional-complement  ( n -- a )
  1- cells prepositional-complements +  ;
  \ Devuelve la dirección _a_ de un elemento ordinal _n_ de la tabla
  \ de complementos (seudo)preposicionales.

: current-prepositional-complement  ( -- a )
  current-preposition @ prepositional-complement  ;
  \ Devuelve la dirección _a_ del elemento de la tabla de complementos
  \ (seudo)preposicionales correspondiente a la (seudo)preposición
  \ abierta.

: (company-complement)  ( -- a )
  «con»-preposition# prepositional-complement  ;
  \ Devuelve la dirección _a_ del elemento de la tabla de complementos
  \ (seudo)preposicionales correspondiente al complemento de compañía
  \ (complemento que puede ser cero si no existe).

:noname  ( -- a | 0 )  (company-complement) @  ;
is company-complement
  \ Ente _a_ complemento de compañía (indicado con «con»).

: (explicit-tool-complement)  ( -- a )
  «usando»-preposition# prepositional-complement  ;
  \ Devuelve la dirección _a_ del elemento de la tabla de complementos
  \ (seudo)preposicionales correspondiente al complemento instrumental
  \ estricto (complemento que puede ser cero si no existe).

:noname  ( -- a | 0 )  (explicit-tool-complement) @  ;
is explicit-tool-complement
  \ Ente _a_ complemento instrumental estricto (indicado con
  \ «usando»).

:noname  ( -- a | 0 )
  explicit-tool-complement ?dup ?exit  company-complement  ;
is tool-complement
  \ Ente _a_ complemento instrumental (indicado con «con» o «usando»).

: init-prepositions  ( -- )
  erase-prepositional-complements
  current-preposition off
  used-prepositions off  ;
  \ Inicializa las preposiciones.

: init-complements  ( -- )
  0 to main-complement
  0 to secondary-complement
  init-prepositions  ;
  \ Inicializa los complementos.

: init-parser  ( -- )  0 to action  init-complements  ;
  \ Preparativos previos a cada análisis.

: (execute-action)  ( xt -- )
  dup to previous-action catch ?wrong  ;
  \ Ejecuta la acción del comando.

: (execute-previous-action)  ( -- )
  previous-action ?dup if  (execute-action)  then  ;
  \ Ejecuta la acción previa, si es posible
  \ (no es posible la primera vez, cuando su valor aún es cero).
  \ XXX NOTE: otra solución posible: inicializar la variable con una
  \ acción que nada haga.

: execute-previous-action  ( -- )
  repeat-previous-action? @ 0= no-verb-error# and throw
  (execute-previous-action)  ;
  \ Ejecuta la acción previa, si así está configurado.

: execute-action  ( -- )
  [debug-catch] [debug-parsing] [or] [??] ~~
  action ?dup
  [debug-catch] [debug-parsing] [or] [??] ~~
  if    (execute-action)
  else  execute-previous-action
  then
  [debug-catch] [debug-parsing] [or] [??] ~~  
  .system-status  \ XXX INFORMER
  ;
  \ Ejecuta la acción del comando, si es posible.

: (evaluate-command)  ( -- )
  begin   parse-name ?dup
  while   find-name ?dup if  name>int execute  then
  repeat  drop  ;
  \ Analiza la fuente actual, ejecutando las palabras reconocidas que contenga.
  \ XXX TODO -- mover a Flibustre

: evaluate-command  ( ca len -- )
  \ ." comando:" 2dup cr type  \ XXX INFORMER
  ['] (evaluate-command) execute-parsing  ;
  \ Analiza el comando, ejecutando las palabras reconocidas que contenga.
  \ XXX TODO -- mover a Flibustre

: a-preposition-is-open?  ( -- f )
  current-preposition @ 0<>  ;
  \ ¿Hay un complemento (seudo)preposicional abierto?

: no-parsing-error-left?  ( -- f )
  a-preposition-is-open? dup
  unresolved-preposition-error# and ?wrong  0=  ;
  \ Comprueba si quedó un complemento (seudo)preposicional incompleto,
  \ algo que no puede detectarse en el análisis principal, y devuelve
  \ el resultado como un indicador: ¿No quedó algún error pendiente
  \ tras el análisis?  (cierto: ningún error pendiente; falso: algún
  \ error pendiente).

[debug-parsing-result] [if]
  : .complement?  ( ca len a -- )
    ?dup if  name s& paragraph  else  2drop  then  ;
    \ Imprime un nombre de ente complemento _a_,
    \ con un texto previo _ca len_, si existe el complemento.
    \ XXX INFORMER
[then]

: valid-parsing?  ( ca len -- f )
  -punctuation
  [debug-parsing] [??] ~~
  player-wordlist 1 set-order
  \ [debug-catch] [if]  s" En `valid-parsing?` antes de preparar `catch`" debug  [then]  \ xxx informer
  [debug-parsing] [??] ~~
  ['] evaluate-command catch
  [debug-parsing] [??] ~~
  dup if  nip nip  then
    \ Arreglar la pila, pues `catch` hace que apunte a su posición previa
  [debug-parsing] [??] ~~
  dup ?wrong 0=
  [debug-parsing] [??] ~~
  restore-wordlists
  no-parsing-error-left? and
  [debug-parsing] [??] ~~
  [debug-parsing-result] [if]
    s" Main           : " main-complement .complement?
    s" Secondary      : " secondary-complement .complement?
    s" Tool           : " tool-complement .complement?
    s" Explicit tool  : " explicit-tool-complement .complement?
    s" Company        : " company-complement .complement?
   [then]  ;
  \ Evalúa un comando _ca len_ con el vocabulario del juego y devuelve
  \ un indicador _f_: ¿El comando se analizó sin error?

: >but-one!  ( a -- )  dup @ swap >but-one !  ;
  \ Copia un complemento de la zona «últimos» a la «penúltimos» de la
  \ tabla `last-complement`.  _a_ es la dirección en la zona «últimos»
  \ de la tabla `last-complement`.

: shift-last-complement  ( a -- )
  >last-complement >but-one!  \ El último del mismo género y número
  last-complement >but-one!  ;  \ El último absoluto
  \ Copia el último complemento al lugar del penúltimo.  _a_ es el
  \ ente que fue encontrado como último complemento.

: new-last-complement  ( a -- )
  dup shift-last-complement
  dup last-complement !
  dup >last-complement !  ;
  \ Guarda un nuevo complemento como el último complemento hallado, en
  \ tres pasos: 1) Copiar último a penúltimo; 2) Guardarlo como último
  \ absoluto; 3) Guardarlo como último de su género y número.

: save-command-elements  ( -- )
  action to last-action  ;
  \ XXX TODO -- no usado
  \ XXX TODO -- falta guardar los complementos

: (obey)  ( ca len -- )
  init-parser valid-parsing? ?? execute-action  ;
  \ Evalúa un comando con el vocabulario del juego.

: obey  ( ca len -- )
  [debug-parsing] [??] ~~
  dup if  (obey)  else  2drop  then
  [debug-parsing] [??] ~~  ;
  \ Evalúa un comando, si no está vacío, con el vocabulario del juego.

: second?  ( x1 x2 -- x1 f )
  2dup different?  ;
  \ ¿Hay ya otra acción o complemento anterior y es diferente?  Los
  \ parámetros representan una acción (_xt_) o un ente (_a_): _x1_ es
  \ la acción o complemento recién encontrado; _x2_ es la acción o
  \ complemento anterior, o cero.

: set-action  ( xt -- )
  action second?
  too-many-actions-error# and throw
  to action  ;
  \ Comprueba y almacena la acción _xt_.
  \ Provoca un error si ya había una acción.

: set-preposition  ( n -- )
  a-preposition-is-open?
  unresolved-preposition-error# and throw
  current-preposition !  ;
  \ Almacena una (seudo)preposición _n_ recién hallada en la frase.

: set-prepositional-complement  ( a -- )
  current-prepositional-complement @ second?
  repeated-preposition-error# and throw
  dup new-last-complement
  current-prepositional-complement !
  current-preposition @ >bit used-prepositions +!
  current-preposition off  ;
  \ Almacena un ente _a_ como complemento (seudo)preposicional.
  \ Provoca error si la preposición ya había sido usada,

: set-secondary-complement  ( a -- )
  secondary-complement second?
  too-many-complements-error# and throw
  to secondary-complement  ;
  \ Almacena el ente _a_ como complemento secundario.
  \ Provoca un error si ya existía un complemento secundario.

: set-main-complement  ( a -- )
  [debug-parsing] [??] ~~
  main-complement second?
  too-many-complements-error# and throw
  dup new-last-complement
  to main-complement  ;
  \ Almacena el ente _a_ como complemento principal.
  \ Provoca un error si ya existía un complemento principal.

: set-non-prepositional-complement  ( a -- )
  main-complement if    set-secondary-complement
                  else  set-main-complement  then  ;
  \ Almacena un complemento principal o secundario.
  \ a = Identificador de ente
  \ XXX TODO -- esta palabra sobrará cuando las (seudo)preposiciones
  \ estén implementadas completamente

: (set-complement)  ( a -- )
  a-preposition-is-open?
  if    set-prepositional-complement
  else  set-non-prepositional-complement  then  ;
  \ Almacena el ente _a_ como complemento.

: set-complement  ( a | 0 -- )
  ?dup ?? (set-complement)  ;
  \ Comprueba y almacena un complemento.
  \ a = Identificador de ente,
  \     o cero si se trata de un pronombre sin referente.

: set-action-or-complement  ( xt a -- )
  action 0<> a-preposition-is-open? or
  if  nip set-complement  else  drop set-action  then  ;
  \ Comprueba y almacena un complemento _a_ o una acción _xt_,
  \ ambos posibles significados de la misma palabra.

\ vim:filetype=gforth:fileencoding=utf-8

