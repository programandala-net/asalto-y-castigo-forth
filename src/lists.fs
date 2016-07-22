\ lists.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607212220

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/plus-plus.fs       \ `++`
require galope/replaced.fs        \ `replaced`
require galope/str-append-txt.fs  \ `str-append-txt`
require galope/two-choose.fs      \ `2choose`
require galope/txt-plus.fs        \ `txt+`

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

require ffl/str.fs

set-current

require out-str.fs

\ ==============================================================

variable #listed
  \ Contador de elementos listados, usado en varias acciones.

variable #elements
  \ Total de los elementos de una lista.

: list-separator$  ( u1 u2 -- ca len )
  ?dup if
    1+ = if  s"  y "  else  s" , "  then
  else  0  then  ;
  \ Devuelve el separador adecuado a un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  \ ca len = Cadena devuelta, que podrá ser « y » o «, » o «» (vacía)

: (list-separator)  ( u1 u2 -- )
  1+ = if  s" y" out-str str-append-txt  else  s" ," out-str str-append-string  then  ;
  \ Añade a la cadena dinámica `out-str` el separador adecuado («y»
  \ o «,») para un elemento de una lista, siendo _u1_ el número de
  \ elementos que tiene la lista y _u2_ el número de elementos ya listados.

: list-separator  ( u1 u2 -- )
  ?dup if  (list-separator)  else  drop  then  ;
  \ Añade a la cadena dinámica `out-str` el separador adecuado (o
  \ ninguno) para un elemento de una lista, siendo _u1_ el número de
  \ elementos que tiene la lista y _u2_ el número de elementos ya
  \ listados.

: /list++  ( u a1 a2 -- u | u+1 )
  dup must-be-listed?
  if  location = abs +  else  2drop  then  ;
  \ Incrementa el contador _u_ si un ente _a1_ es la localización de
  \ otro ente _a2_ y puede ser listado.

: /list  ( a -- u )
  0  \ Contador
  #entities 0 do  over i #>entity /list++  loop  nip  ;
  \ Cuenta el número de entes _u_ cuya localización es el ente _a_ y
  \ pueden ser listados.

: (worn)$  ( a -- ca1 len1 )
  s" (puest" rot noun-ending s" )" s+ s+  ;
  \ Devuelve «(puesto/a/s)», según el género y número del ente indicado.

: (worn)&  ( ca1 len1 a -- ca1 len1 | ca2 len2 )
  dup  is-worn? if  (worn)$ txt+  else  drop  then  ;
  \ Añade a una cadena _ca1 len1_, si es necesario, el indicador de
  \ que el ente _a_ es una prenda puesta, devolviendo una nueva cadena
  \ _ca2 len2_ con el resultado.

: (content-list)  ( a -- )
  #elements @ #listed @  list-separator
  dup full-name-as-direct-complement rot (worn)& out-str str-append-txt  #listed ++  ;
  \ Añade a la lista en la cadena dinámica `out-str` el separador y
  \ el nombre de un ente.

: about-to-list  ( a -- n )  #listed off  /list dup #elements !  ;
  \ Prepara el inicio de una lista, siendo _a_ el ente que es la
  \ localización de los entes a incluir en la lista; y _n_ el número
  \ de entes que serán listados.

: content-list  ( a -- ca len )
  out-str str-clear
  dup about-to-list if
    #entities 1 do
      dup i #>entity dup must-be-listed? if
        is-there? if  i #>entity (content-list)  then
      else  2drop
      then
    loop  s" ." out-str str-append-string
  then  drop  out-str str-get  ;
  \ Devuelve una lista _ca len_ de entes cuya localización es el
  \ ente _a_.

: .present  ( -- )
  my-location content-list dup
  if  s" Ves" s" Puedes ver" 2 2choose 2swap txt+ narrate
  else  2drop  then  ;
  \ Lista los entes presentes.

\ vim:filetype=gforth:fileencoding=utf-8

