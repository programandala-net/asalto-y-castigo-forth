\ parser.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606281951

\ Note: The comments of the code are in Spanish.

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

variable 'prepositions#
  \ Número de (seudo)preposiciones.

: prepositions#  ( -- n )  'prepositions# @  ;
  \ Número de (seudo)preposiciones.

: >bit  ( u1 -- u2 )  1 swap lshift  ;
  \ u2 = número cuyo único bitio activo es aquel cuyo orden
  \ indica u1 (ej. 1->1, 2->2, 3->4, 4->8...)

: preposition:  ( "name1" "name2" -- )
  prepositions# >bit constant
  'prepositions# ++ prepositions# constant  ;
  \ Crea los identificadores de una (seudo)preposición (_name1_:
  \ nombre del identificador para usar como máscara de bitios;
  \ _name2_: nombre del identificador para usar como índice de tabla)
  \ y actualiza el contador:

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

prepositions# cells constant /prepositional-complements
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

: prepositional-complement  ( u -- a )
  1- cells prepositional-complements +  ;
  \ Devuelve la dirección _a_ de un elemento ordinal _u_ de la tabla
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

' (company-complement) is company-complement

: (actual-company-complement)  ( -- a|0 )
  «usando»-preposition# prepositional-complement @ dup 0<>
  if  drop company-complement  then  ;
  \ Devuelve la dirección _a_ del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento de compañía estricto
  \ (complemento que puede ser cero si no existe).
  \ XXX TODO -- experimental, ojo: puede devolver cero

' (actual-company-complement) is actual-company-complement

: (actual-tool-complement)  ( -- a )
  «usando»-preposition# prepositional-complement  ;
  \ Devuelve la dirección _a_ del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento instrumental estricto
  \ (complemento que puede ser cero si no existe).

' (actual-tool-complement) is actual-tool-complement

: (tool-complement)  ( -- a )
  actual-tool-complement dup @ 0=
  if  drop company-complement  then  ;
  \ Devuelve la dirección _a_ del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento instrumental
  \ (complemento que puede ser cero si no existe).

' (tool-complement) is tool-complement

: prepositions-off  ( -- )
  erase-prepositional-complements
  current-preposition off
  used-prepositions off  ;
  \ Inicializa las preposiciones.

: complements-off  ( -- )
  main-complement off
  secondary-complement off
  prepositions-off  ;
  \ Inicializa los complementos.

: init-parsing  ( -- )  action off  complements-off  ;
  \ Preparativos previos a cada análisis.

: (execute-action)  ( xt -- )
  dup previous-action ! catch ?wrong  ;
  \ Ejecuta la acción del comando.

: (execute-previous-action)  ( -- )
  previous-action @ ?dup if  (execute-action)  then  ;
  \ Ejecuta la acción previa, si es posible
  \ (no es posible la primera vez, cuando su valor aún es cero).
  \ XXX NOTE: otra solución posible: inicializar la variable con una
  \ acción que nada haga.

: execute-previous-action  ( -- )
  repeat-previous-action? @
  if  (execute-previous-action)  else  no-verb-error# ?wrong  then  ;
  \ Ejecuta la acción previa, si así está configurado.

: execute-action  ( -- )
  [debug-catch] [debug-parsing] [or] [??] ~~
  action @ ?dup
  [debug-catch] [debug-parsing] [or] [??] ~~
  if    (execute-action)
  else  execute-previous-action
  then
  [debug-catch] [debug-parsing] [or] [??] ~~  ;
  \ Ejecuta la acción del comando, si es posible.

: (evaluate-command)  ( -- )
  begin   parse-name ?dup
  while   find-name ?dup if  name>int execute  then
  repeat  drop  ;
  \ Analiza la fuente actual, ejecutando las palabras reconocidas que contenga.

: evaluate-command  ( ca len -- )
  \ ." comando:" 2dup cr type  \ XXX INFORMER
  ['] (evaluate-command) execute-parsing  ;
  \ Analiza el comando, ejecutando las palabras reconocidas que contenga.

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
  : .complement?  ( ca1 len1 a2 -- )  \ XXX INFORMER
    @ ?dup if  name s& paragraph  else  2drop  then  ;
    \ Imprime un nombre de complemento, con un texto previo, si existe.
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
    s" Actual tool    : " actual-tool-complement .complement?
    s" Company        : " company-complement .complement?
    \ s" Actual company : " actual-company-complement .complement? \ XXX TMP -- experimental
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
  action @ last-action !  ;
  \ XXX TODO -- no usado
  \ XXX TODO -- falta guardar los complementos

: (obey)  ( ca len -- )
  init-parsing valid-parsing? ?? execute-action  ;
  \ Evalúa un comando con el vocabulario del juego.

: obey  ( ca len -- )
  [debug-parsing] [??] ~~
  dup if  (obey)  else  2drop  then
  [debug-parsing] [??] ~~  ;
  \ Evalúa un comando, si no está vacío, con el vocabulario del juego.

: second?  ( x1 x2 -- x1 f )
  [debug-parsing] [??] ~~
  2dup different?  \ ¿Hay ya otro anterior y es diferente?
  [debug-parsing] [??] ~~  ;
  \ ¿La acción o el complemento son los segundos que se encuentran?
  \ Los dos valores representan una acción (xt) o un ente (a).
  \ x1 = Acción o complemento recién encontrado
  \ x2 = Acción o complemento anterior, o cero

: action!  ( xt -- )
  [debug-parsing] [??] ~~
  action @ second?  \ ¿Había ya una acción?
  [debug-parsing] [??] ~~
  too-many-actions-error# and
  [debug-parsing] [??] ~~
  throw  \ Sí, error
  [debug-parsing] [??] ~~
  action !  \ No, guardarla
  [debug-parsing] [??] ~~  ;
  \ Comprueba y almacena la acción.
  \ xt = Identificador de ejecución de la acción

: preposition!  ( u -- )
  a-preposition-is-open?
  unresolved-preposition-error# and throw
  current-preposition !  ;
  \ Almacena una (seudo)preposición recién hallada en la frase.
  \ u = Identificador de la preposición

: prepositional-complement!  ( a -- )
  [debug-parsing] [??] ~~
  current-prepositional-complement @ second?  \ ¿Se había usado ya esta preposición con otro complemento?
  repeated-preposition-error# and throw  \ Si es así, error
  dup new-last-complement
  current-prepositional-complement !
  current-preposition @ >bit used-prepositions +!  \ Añadir la preposición a las usadas
  current-preposition off  \ Cerrar la preposición en curso
  [debug-parsing] [??] ~~  ;
  \ Almacena un complemento (seudo)preposicional.
  \ a = Identificador de ente

: secondary-complement!  ( a -- )
  secondary-complement @ second?  \ ¿Había ya un complemento secundario?
  too-many-complements-error# and throw  \ Si es así, error
  secondary-complement !  ;
  \ Almacena el complemento secundario.
  \ a = Identificador de ente

: main-complement!  ( a -- )
  [debug-parsing] [??] ~~
  main-complement @ second?  \ ¿Había ya un complemento principal?
  too-many-complements-error# and throw  \ Si es así, error
  dup new-last-complement
  main-complement !  ;
  \ Almacena el complemento principal.
  \ a = Identificador de ente

: non-prepositional-complement!  ( a -- )
  main-complement @
  if  secondary-complement!  else  main-complement!  then  ;
  \ Almacena un complemento principal o secundario.
  \ a = Identificador de ente
  \ XXX TODO -- esta palabra sobrará cuando las (seudo)preposiciones
  \ estén implementadas completamente

: (complement!)  ( a -- )
  a-preposition-is-open?
  if    prepositional-complement!
  else  non-prepositional-complement!  then  ;
  \ Almacena el ente _a_ como complemento.

: complement!  ( a | 0 -- )
  ?dup ?? (complement!)  ;
  \ Comprueba y almacena un complemento.
  \ a = Identificador de ente,
  \     o cero si se trata de un pronombre sin referente.

: action|complement!  ( xt a -- )
  action @
  a-preposition-is-open? or
  if  nip complement!  else  drop action!  then  ;
  \ Comprueba y almacena un complemento _a_ o una acción _xt_,
  \ ambos posibles significados de la misma palabra.

\ vim:filetype=gforth:fileencoding=utf-8

