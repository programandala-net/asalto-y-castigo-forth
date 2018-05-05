#! /usr/bin/env gforth

\ main.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html
\
\ Author: Marcos Cruz (programandala.net), 2011..2016
\
\ Last modified 201805052247

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

only forth definitions

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/randomize.fs  \ `randomize`

require galope/stringer.fs  \ Circular string buffer

2048 allocate-stringer

require galope/s-s-plus.fs  \ `ss+`

' ss+ alias s+
  \ Replace Gforth's `s+`. This must be done before requiring `txt+`,
  \ to make sure `txt+` uses `ss+`.

require galope/txt-plus.fs  \ `txt+`

require galope/tilde-tilde.fs    \ improved `~~`

\ ==============================================================

require talanto/wordlists.fs

$1C5C constant parser-id
  \ Identifier of the parser that will be used from Talanto.

include version.fs
include debug_tools.fs

: including ( ca len -- ) 2dup cr type included ?.s ;
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

require talanto/entity.fs \ XXX TODO -- Move from Talanto.
\ s" entity.fs" including

require talanto/entity_structure.fs \ XXX TODO -- Move from Talanto.
\ s" entity_structure.fs" including

require talanto/entity_structure_interface.fs \ XXX TODO -- Move from Talanto.
\ s" entity_structure_interface.fs" including

s" entity_identifiers.fs" including
s" data_advanced_interface.fs" including
s" data_tools.fs" including
s" calculated_texts.fs" including
s" lists.fs" including

require talanto/errors.fs \ XXX TODO -- Move from Talanto.
\ s" errors.fs" including

require talanto/error_conditions.fs \ XXX TODO -- Move from Talanto.
\ s" error_conditions.fs" including

require talanto/parser.1c5c.fs
require talanto/last-complements.es.fs
s" language_errors.fs" including
s" config.fs" including
s" plot.fs" including
s" entities.fs" including
s" action_errors.fs" including
s" actions.fs" including
s" vocabulary.fs" including
s" answers.fs" including
s" the_end.fs" including
s" about.fs" including
s" intro.fs" including

\ ==============================================================
\ Principal

: init-session ( -- ) restore-wordlists  init-screen ;
  \ Initialize the session.

: init-parser/game ( -- ) erase-last-complements ;
  \ Initialize the parser before every game.
  \ XXX TODO -- move to other file

: init-game-for-debugging ( -- )
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
  log~ be-hold
  idol~ be-hold
  flint~ be-hold
  torch~ be-hold
 ;
  \ Special situations.
  \ XXX TMP -- for debugging.

: init-game ( -- )
  randomize
  init-parser/game init-entities init-plot
  get-config new-page
  \ init-game-for-debugging exit  \ XXX TMP -- for debugging
  about cr intro  \ XXX TMP -- commented out for debugging
  location-01~ enter-location ;
  \ Initialization needed before every game.

: cycle ( -- ) accept-command obey ;

: game ( -- ) begin  cycle plot game-over?  until ;

: session ( -- ) begin  init-game game the-end  enough?  until ;

\ ==============================================================
\ Boot

forth-wordlist set-current

: run ( -- ) init-session session farewell ;

cr .( Escribe RUN para jugar) cr  \ XXX TMP

run  \ XXX TMP

\ ==============================================================
\ Debug

\ cr
\ : zx idol~ cannot-see-what ;
\ ' zx (execute-action)
\ cr .( test done) cr cr

: i0 ( -- )
  init-session init-game  s" Data are ready." /ltype ;
  \ XXX TMP -- for debugging

\ i0 cr  \ XXX TMP -- for debugging

\ s" debug_tests.fs" including

\ ==============================================================
\ Change log

\ 2017-11-16: Update parser's name and id after changes in Talanto.
\
\ 2017-11-17: Update from Galope's deprecated module <print.fs> to
\ <l-type.fs>.

\ vim:filetype=gforth
