#! /usr/bin/env gforth

\ main.fs

\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ A text adventure in Spanish,
\ written in Forth with Gforth.

\ Project under development.

\ Version: see file <version.fs>.

\ Last update: 201606291159

\ Copyright (C) 2011..2016 Marcos Cruz (programandala.net)

\ 'Asalto y castigo' is free software; you can redistribute
\ it and/or modify it under the terms of the GNU General
\ Public License as published by the Free Software
\ Foundation; either version 2 of the License, or (at your
\ option) any later version. See:

\ http://gnu.org/licenses/
\ http://gnu.org/licenses/gpl-2.0.html

\ ==============================================================
\ Reconocimiento

\ «Asalto y castigo» está basado en el programa homónimo escrito por
\ Baltasar el Arquero en Sinclair BASIC para ZX Spectrum.

\ Idea, argumento, textos y programa originales:
\
\ Copyright (C) 2009 Baltasar el Arquero
\ http://caad.es/baltasarq/
\ http://baltasarq.info

\ ==============================================================
\ Requisitos

include requirements.fs

\ ==============================================================
\ Listas de palabras

wordlist constant game-wordlist
  \ Palabras del programa (no de la aventura)

: restore-wordlists  ( -- )
  only forth game-wordlist dup >order set-current  ;
  \ Restaura las listas a su estado habitual.

restore-wordlists

\ ==============================================================
\ Meta

include version.fs

: press-key  ( -- ) key drop  ;

\ Indicadores para depuración

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

\ Indicadores para poder elegir alternativas que aún son experimentales

true dup constant [old-method] immediate
      0= constant [new-method] immediate

\ Títulos de sección

: depth-warning  ( -- )
  cr ." Aviso: La pila no está vacía. Contenido: "  ;

: ?.s  ( -- )
  depth if  depth-warning .s cr  press-key  then  ;
  \ Imprime el contenido de la pila si no está vacía.

: fatal-error  ( f ca len -- )
  rot if  ." Error fatal: " type cr bye  else  2drop  then  ;
  \ Si el indicador _f_ es distinto de cero,
  \ informa de un error _ca len_ y sale del sistema.
  \ XXX TODO -- no usado

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

\ ==============================================================
\ Vectores

defer protagonist%  ( -- a )  \ Ente protagonista.
defer sword%        ( -- a )  \ Ente espada.
defer stone%        ( -- a )  \ Ente piedra.
defer torch%        ( -- a )  \ Ente antorcha.
defer leader%       ( -- a )  \ Ente líder de los refugiados.
defer location-01%  ( -- a )  \ Primer ente escenario.
defer exits%        ( -- a )  \ Ente salidas.
defer log%          ( -- a )  \ Ente tronco.

defer list-exits  ( -- )
  \ Crea e imprime la lista de salidas.

defer lock-found  ( -- )
  \ Encontrar el candado.

\ ==============================================================

include error_codes.fs
include config_variables.fs
include plot_variables.fs
include display.fs
include strings.fs
include random_texts.fs
\ include sound.fs  \ XXX TODO -- not used yet
include printing.fs
include data_structure.fs
include entity_identifiers.fs
include location_connectors.fs
include data.fs
include action_errors.fs
include lists.fs
include config.fs
include plot.fs
include actions.fs
include parser.fs
include player_vocabulary.fs
include input.fs
include answers.fs
include the_end.fs
include about.fs
include intro.fs

\ ==============================================================
\ Principal

: init-once  ( -- )  restore-wordlists  init-screen  ;
  \ Preparativos que hay que hacer solo una vez, antes de la primera partida.

: init-parser/game  ( -- )  erase-last-command-elements  ;
  \ Preparativos que hay que hacer en el intérprete
  \ de comandos antes de cada partida.
  \ XXX TODO -- trasladar a su zona

: init-game-for-debugging  ( -- )
  location-01% enter-location
  \ location-08% enter-location  \ Emboscada
  \ location-11% enter-location  \ Lago
  \ location-17% enter-location  \ Antes de la cueva oscura
  \ location-19% enter-location  \ Encuentro con Ambrosio
  \ location-28% enter-location  \ Refugiados
  \ location-47% enter-location  \ casa de Ambrosio
  \ snake% be-here
  \ ambrosio% be-here
  \ key% be-hold
  flint% be-hold
  torch% be-hold
  ;
  \ Condiciones especiales de inicio, para forzar situaciones
  \ concretas de la trama en el arranque y así probar el código.

: init-game  ( -- )
  randomize
  init-parser/game init-entities init-plot
  get-config new-page
  [true] [if]    about cr intro  location-01% enter-location
         [else]  init-game-for-debugging
         [then]  ;
  \ Preparativos que hay que hacer antes de cada partida.

: game  ( -- )  begin  plot accept-command obey  game-over?  until  ;
  \ Bucle de la partida.

: (adventure)  ( -- )  begin  init-game game the-end  enough?  until  ;
' (adventure) is adventure
  \ Bucle del juego.

: run  ( -- )  init-once adventure farewell  ;
  \ Arranque del juego.

forth-wordlist set-current

\ ==============================================================

cr .( Escribe RUN para jugar)  \ XXX TMP

: i0  ( -- )
  \ XXX TMP -- hace toda la inicialización; para depuración.
  init-once init-game
  s" Datos preparados." paragraph  ;

\ i0 cr  \ XXX TMP -- para depuración

\ include ../debug/checks.fs

