#! /usr/bin/env gforth

\ main.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html
\
\ Author: Marcos Cruz (programandala.net), 2011..2016
\
\ Last update: 201607032349
\
\ 'Asalto y castigo' is free software; you can redistribute
\ it and/or modify it under the terms of the GNU General
\ Public License as published by the Free Software
\ Foundation; either version 2 of the License, or (at your
\ option) any later version. See:
\
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

include requirements.fs

wordlist constant game-wordlist
  \ Palabras del programa (no del vocabulario del jugador).

: restore-wordlists  ( -- )
  only forth game-wordlist dup >order set-current  ;
  \ Restaura las listas de palabras a su estado habitual.

restore-wordlists

wordlist constant player-wordlist
  \ Palabras del vocabulario del jugador.

include version.fs
include debug_tools.fs

: including  ( ca len -- )  2dup cr type included ?.s  ;
  \ Include file _ca len_ and check the stack.
  \ XXX TMP -- for debugging

s" config_variables.fs" including
s" plot_variables.fs" including
s" display.fs" including
s" strings.fs" including
s" random_texts.fs" including
\ s" sound.fs" including  \ XXX TODO -- not used yet
s" key_input.fs" including
s" printing.fs" including
s" command_input.fs" including
s" data_structure.fs" including
s" data_basic_interface.fs" including
s" entity_identifiers.fs" including
s" data_advanced_interface.fs" including
s" data_tools.fs" including
s" calculated_texts.fs" including
s" lists.fs" including
s" config.fs" including
s" parser.fs" including
s" plot.fs" including
s" data.fs" including
s" action_errors.fs" including
s" actions.fs" including
s" player_vocabulary.fs" including
s" answers.fs" including
s" the_end.fs" including
s" about.fs" including
s" intro.fs" including

\ ==============================================================
\ Principal

: init-once  ( -- )  restore-wordlists  init-screen  ;
  \ Preparativos que hay que hacer solo una vez, antes de la primera partida.

: init-parser/game  ( -- )  erase-last-command-elements  ;
  \ Preparativos que hay que hacer en el intérprete
  \ de comandos antes de cada partida.
  \ XXX TODO -- trasladar a su zona

: init-game-for-debugging  ( -- )
  location-01~ enter-location
  \ location-08~ enter-location  \ Emboscada
  \ location-11~ enter-location  \ Lago
  \ location-17~ enter-location  \ Antes de la cueva oscura
  \ location-19~ enter-location  \ Encuentro con Ambrosio
  \ location-28~ enter-location  \ Refugiados
  \ location-47~ enter-location  \ casa de Ambrosio
  \ snake~ be-here
  \ ambrosio~ be-here
  \ key~ be-hold
  flint~ be-hold
  torch~ be-hold
  ;
  \ Condiciones especiales de inicio, para forzar situaciones
  \ concretas de la trama en el arranque y así probar el código.
  \ XXX TMP -- para depuración

: init-game  ( -- )
  randomize
  init-parser/game init-entities init-plot
  get-config new-page
  [true] [if]    about cr intro  location-01~ enter-location
         [else]  init-game-for-debugging
         [then]  ;
  \ Preparativos que hay que hacer antes de cada partida.

: game  ( -- )  begin  plot accept-command obey  game-over?  until  ;
  \ Bucle de la partida.

: (adventure)  ( -- )  begin  init-game game the-end  enough?  until  ;
' (adventure) is adventure
  \ Bucle del juego.

\ ==============================================================
\ Arranque

forth-wordlist set-current

: run  ( -- )  init-once adventure farewell  ;
  \ Arranque del juego.

\ run  \ XXX TMP
cr .( Escribe RUN para jugar) cr  \ XXX TMP

\ ==============================================================
\ Depuración

: i0  ( -- )
  init-once init-game
  s" Datos preparados." paragraph  ;
  \ XXX TMP -- hace toda la inicialización; para depuración

\ i0 cr  \ XXX TMP -- para depuración

\ s" debug_tests.fs" including

