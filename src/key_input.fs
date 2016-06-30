\ key_input.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606301013

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Pulsación de una tecla

: ?key-pause  ( n -- )
  dup 0< if  drop key drop  else  seconds  then  ;
  \ Si _n_ es menor de cero, haz una pausa indefinida hasta la
  \ pulsación de una tecla; de otro modo haz una pausa de _n_ segundos
  \ o hasta la pulsación de una tecla.

\ vim:filetype=gforth:fileencoding=utf-8


