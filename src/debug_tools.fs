\ debug_tools.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606291210

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Herramientas para depuración

: press-key  ( -- ) key drop  ;

\ ----------------------------------------------
\ Indicadores genéricos

false value [debug] immediate
  \ ¿Depuración global?
false value [debug-init] immediate
  \ ¿Depurar la inicialización?
false value [debug-parsing] immediate
  \ ¿Depurar el analizador?
false value [debug-parsing-result] immediate
  \ ¿Mostrar el resultado del analizador?
false value [debug-filing] immediate
  \ ¿Depurar operaciones de ficheros?
false value [debug-do-exits] immediate
  \ ¿Depurar la acción `do-exits`?
false value [debug-catch] immediate
  \ ¿Depurar `catch` y `throw`?
false value [debug-save] immediate
  \ ¿Depurar la grabación de partidas?
true value [debug-info] immediate
  \ ¿Mostrar info sobre el presto de comandos?
false value [debug-pause] immediate
  \ ¿Hacer pausa en puntos de depuración?
false value [debug-map] immediate
  \ ¿Mostrar el número de escenario del juego original?

\ ----------------------------------------------
\ Indicadores para alternativas experimentales

true dup constant [old-method] immediate
      0= constant [new-method] immediate

\ ----------------------------------------------
\ Comprobación de la pila

: depth-warning  ( -- )
  cr ." Aviso: La pila no está vacía. Contenido: "  ;

: ?.s  ( -- )
  depth if  depth-warning .s cr  press-key  then  ;
  \ Imprime el contenido de la pila si no está vacía.

\ ----------------------------------------------
\ Error fatal

\ XXX OLD -- no se usa

: fatal-error  ( f ca len -- )
  rot if  ." Error fatal: " type cr bye  else  2drop  then  ;
  \ Si el indicador _f_ es distinto de cero,
  \ informa de un error _ca len_ y sale del sistema.

\ ----------------------------------------------
\ Punto de chequeo

\ XXX OLD

: .stack  ( -- )
  [false] [if]  \ XXX OLD
    ." Pila" depth
    if    ." :" .s ." ( " depth . ." )"
    else  ."  vacía."  then
  [else]  \ XXX NEW
    depth if  cr ." Pila: " .s cr  then
  [then]  ;
  \ Imprime el estado de la pila.

: .sb  ( -- )
  ." Espacio para cadenas:" sb# ?  ;
  \ Imprime el estado del almacén circular de cadenas.

: .system-status  ( -- )
  ( .sb ) .stack  ;
  \ Muestra el estado del sistema.

: .debug-message  ( ca len -- )
  dup if  cr type cr  else  2drop  then  ;
  \ Imprime el mensaje del punto de chequeo, si no está vacío.

: debug-pause  ( -- )
  [debug-pause] [if]  depth ?? press-key [then]  ;
  \ Pausa tras mostrar la información de depuración.

defer debug-color

: debug  ( ca len -- )
  debug-color .debug-message .system-status debug-pause  ;
  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema.

\ vim:filetype=gforth:fileencoding=utf-8
