\ keyboard_input.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last modified 201607141651

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

require ../lib/stringstack.fs
  \ Speuler's string stack.

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/slash-csv.fs   \ `/csv`
require galope/svariable.fs   \ `svariable`
require galope/xlowercase.fs  \ `xlowercase` for UTF-8

set-current

require talanto/parser.data.fs

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

: ((accept-input))  ( -- ca len )
  input-color command dup /command accept
  str+strip 2dup xlowercase  ;
  \ Espera un comando del jugador y lo devuelve sin espacios laterales
  \ y en minúsculas en la cadena _ca len_.
  \ XXX TODO -- renombrar

: split-input  ( ca len -- ca' len' )
  /csv 1- 0 ?do  push$  loop  ;
  \ Divide _ca len_ en valores separados por comas, y los guarda en la
  \ pila de cadenas, salvo el primer valor, que es devuelto como _ca'
  \ len'_. Si no hay comas en _ca len_, _ca' len'_ es una copia de _ca
  \ len_.

: (accept-input)  ( wid -- ca len )
  1 set-order  .command-prompt ((accept-input))  restore-wordlists
  split-input  ;
  \ Espera una entrada del jugador (cuyas palabras aceptadas están en
  \ la lista de palabras _wid_) y lo devuelve sin espacios laterales y
  \ en minúsculas en la cadena _ca len_.

: remaining-input?  ( -- f )  depth$ 0<>  ;

: get-remaining-input  ( -- ca len )  pop$  ;

: accept-input  ( wid -- ca len )
  remaining-input?  dup reuse-previous-action !
  if    drop get-remaining-input narration-break
  else  (accept-input)  then  ;
  \ XXX TODO -- mueve `narration-break` al final de la entrada de
  \ escenarios. Mejor aún: hacerlo configurable y usar una palabra
  \ específica: `command-break`.

: accept-command  ( -- ca len )  player-wordlist accept-input  ;
  \ Espera un comando del jugador y lo devuelve sin espacios laterales
  \ y en minúsculas en la cadena _ca len_.

\ vim:filetype=gforth:fileencoding=utf-8

