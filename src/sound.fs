\ sound.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last modified XXX

\ Note: The comments of the code are in Spanish.

\ ==============================================================

\ Las herramientas para proveer de sonido al juego están apenas
\ esbozadas aquí.

\ La idea consiste en utilizar un reproductor externo que acepte
\ comandos y no muestre interfaz, como mocp para GNU/Linux, que es el
\ que usamos en las pruebas. Los comandos para la consola del sistema
\ operativo se pasan con la palabra SYSTEM de Gforth.

: clear-sound-track  ( -- )  s" mocp --clear" system  ;
  \ Limpia la lista de sonidos.

: add-sound-track  ( ca len -- )  s" mocp --add" 2swap s& system  ;
  \ Añade un fichero de sonido a la lista de sonidos.

: play-sound-track  ( -- )  s" mocp --play" system  ;
  \ Inicia la reproducción de la lista de sonidos.

: stop-sound-track  ( -- )  s" mocp --stop" system  ;
  \ Detiene la reproducción de la lista de sonidos.

: next-sound-track  ( -- )  s" mocp --forward" system  ;
  \ Salta al siguiente elemento de la lista de sonidos.

\ vim:filetype=gforth:fileencoding=utf-8

