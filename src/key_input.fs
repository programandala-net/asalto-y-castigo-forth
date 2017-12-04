\ key_input.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last modified 201712041706

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/question-seconds.fs \ `?seconds`

set-current

\ ==============================================================
\ Pulsación de una tecla

: ?key-pause ( n -- )
  dup 0< if  drop key drop  else  ?seconds  then ;
  \ Si _n_ es menor de cero, haz una pausa indefinida hasta la
  \ pulsación de una tecla; de otro modo haz una pausa de _n_ segundos
  \ o hasta la pulsación de una tecla.


\ ==============================================================
\ Change log

\ 2017-12-04: Update to Galope 0.157.0: Replace `seconds` with
\ `?seconds`.

\ vim:filetype=gforth:fileencoding=utf-8
