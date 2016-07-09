#! /usr/bin/env gforth

\ main.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html
\
\ Author: Marcos Cruz (programandala.net), 2011..2016
\
\ Last update: 201607100034

\ ==============================================================
\ Credits

\ «Asalto y castigo» is an improved remake of the original version,
\ written by Baltasar 'el Arquero' in BASIC for ZX Spectrum and other
\ 8-bit computers.

\ Original idea, plot, and texts:
\
\ Copyright (C) 2009 Baltasar el Arquero
\ http://caad.es/baltasarq/
\ http://baltasarq.info

\ ==============================================================

include requirements.fs

include flibustre/wordlists.fs

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
require flibustre/entity.fs
s" data_structure.fs" including
s" data_basic_interface.fs" including
s" entity_identifiers.fs" including
s" data_advanced_interface.fs" including
s" data_tools.fs" including
s" calculated_texts.fs" including
s" lists.fs" including
require flibustre/errors.fs
require flibustre/error_conditions.fs
s" parser.fs" including
s" language_errors.fs" including
s" config.fs" including
s" plot.fs" including
s" entities.fs" including
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
  \ Initialization required only once, before the first game.

: init-parser/game  ( -- )  erase-last-command-elements  ;
  \ Initialize the parser before every game.
  \ XXX TODO -- move to other file

: init-game-for-debugging  ( -- )
  \ location-01~ enter-location
  \ location-08~ enter-location  \ ambush
  \ location-43~ enter-location    \ snake
  \ location-11~ enter-location  \ lake
  \ location-17~ enter-location  \ before the dark cave
  \ location-19~ enter-location  \ Ambrosio
  location-28~ enter-location  \ refugees
  \ location-47~ enter-location  \ Ambrosio's home
  \ snake~ be-here
  \ ambrosio~ be-here
  \ key~ be-hold
  flint~ be-hold
  torch~ be-hold
  ;
  \ Special situations.
  \ XXX TMP -- for debugging.

: init-game  ( -- )
  randomize
  init-parser/game init-entities init-plot
  get-config new-page
  \ init-game-for-debugging exit  \ XXX TMP -- for debugging
  about cr intro  \ XXX TMP -- commented out for debugging
  location-01~ enter-location  ;
  \ Initialization needed before every game.

: game  ( -- )  begin  plot accept-command obey  game-over?  until  ;
  \ Game loop.

: (adventure)  ( -- )  begin  init-game game the-end  enough?  until  ;
' (adventure) is adventure

\ ==============================================================
\ Boot

forth-wordlist set-current

: run  ( -- )  init-once adventure farewell  ;

\ run  \ XXX TMP
\ cr .( Escribe RUN para jugar) cr  \ XXX TMP

\ ==============================================================
\ Debug

\ cr
\ : zx idol~ cannot-see-what ;
\ ' zx (execute-action)
\ cr .( test done) cr cr

: i0  ( -- )
  init-once init-game  s" Data are ready." paragraph  ;
  \ XXX TMP -- for debugging

\ i0 cr  \ XXX TMP -- for debugging

\ s" debug_tests.fs" including

