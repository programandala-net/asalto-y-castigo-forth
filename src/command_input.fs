\ keyboard_input.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607111134

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/xlowercase.fs  \ `xlowercase` for UTF-8

set-current

\ ==============================================================
\ Entrada de comandos

\ Para la entrada de comandos se usa la palabra de Forth `accept`, que
\ permite limitar el número máximo de caracteres que serán aceptados.

svariable command
  \ Zona de almacenamiento del comando.

svariable command-prompt
  \ Presto de entrada de comandos.

: command-prompt$  ( -- ca len )  command-prompt count  ;
  \ Devuelve el presto de entrada de comandos.

: /command  ( -- u )
  cols /indentation @ - 1-
  cr-after-command-prompt? @ 0= abs command-prompt$ nip * -
  cr-after-command-prompt? @ 0= space-after-command-prompt? @ and abs -  ;
  \ Devuelve la longitud máxima posible para un comando.  Hace el
  \ cálculo en tres pasos, correspondientes a las tres líneas de
  \ código de la palabra: 1) Toma las columnas disponibles, les resta
  \ la indentación y uno más para el espacio que ocupará el cursor al
  \ final de la línea; 2) Resta la longitud del presto si no lleva
  \ detrás un salto de línea; 3) Resta uno si tras el presto no va
  \ salto de línea pero sí un espacio.

: .command-prompt  ( -- )
  command-prompt$ command-prompt-color paragraph
  cr-after-command-prompt? @
  if    cr+
  else  space-after-command-prompt?
        if  background-color space  then
  then  ;
  \ Imprime un presto para la entrada de comandos.

: (accept-input)  ( -- ca len )
  input-color command dup /command accept
  str+strip 2dup xlowercase  ;
  \ Espera un comando del jugador y lo devuelve sin espacios laterales
  \ y en minúsculas en la cadena _ca len_.

: accept-input  ( wid -- ca len )
  1 set-order  .command-prompt (accept-input)  restore-wordlists  ;
  \ Espera una entrada del jugador (cuyas palabras aceptadas están en
  \ la lista de palabras _wid_) y lo devuelve sin espacios laterales y
  \ en minúsculas en la cadena _ca len_.

: accept-command  ( -- ca len )  player-wordlist accept-input  ;
  \ Espera un comando del jugador y lo devuelve sin espacios laterales
  \ y en minúsculas en la cadena _ca len_.

\ vim:filetype=gforth:fileencoding=utf-8

