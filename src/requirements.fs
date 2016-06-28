\ requirements.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606282001

\ Note: Some comments of the code are in Spanish.

\ ==============================================================

only forth definitions  decimal

\ ----------------------------------------------
\ Gforth

require random.fs

\ ----------------------------------------------
\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

require ffl/str.fs  \ Cadenas de texto dinámicas
require ffl/trm.fs  \ Manejo de terminal ANSI
require ffl/chr.fs  \ Herramientas para caracteres
require ffl/dtm.fs  \ Tipo de datos para fecha y hora
require ffl/dti.fs  \ Herramientas adicionales para fecha y hora

\ ----------------------------------------------
\ Galope
\ http://programandala.net/en.program.galope.html

require galope/sb.fs \ Almacén circular de cadenas de texto
' bs+ alias s+
' bs& alias s&
' bs" alias s" immediate
2048 dictionary_sb

require galope/aliases-colon.fs               \ `aliases:`
require galope/at-x.fs                        \ `at-x`                                \ XXX TMP -- for debugging
require galope/between.fs                     \ `between`
require galope/bit-field-colon.fs             \ `bitfield:`
require galope/bracket-false.fs               \ `[false]`
require galope/bracket-or.fs                  \ `[or]`
require galope/bracket-question-question.fs   \ `[??]`
require galope/bracket-true.fs                \ `[true]`
require galope/choose.fs                      \ `choose`
require galope/colon-alias.fs                 \ `:alias`
require galope/colors.fs
require galope/column.fs                      \ `column`
require galope/drops.fs                       \ `drops`
require galope/enum.fs                        \ `enum`
require galope/home.fs                        \ `home`
require galope/immediate-aliases-colon.fs     \ `immediate-aliases:`
require galope/ink.fs
require galope/minus-minus.fs                 \ `--`
require galope/paper.fs
require galope/plus-plus.fs                   \ `++`
require galope/print.fs                       \ justified printing
require galope/question-empty.fs              \ `?empty`
require galope/question-execute.fs            \ `?execute`
require galope/question-keep.fs               \ `?keep`
require galope/question-question.fs           \ `??`
require galope/random_strings.fs
require galope/randomize.fs                   \ `randomize`
require galope/replaced.fs                    \ `replaced`
require galope/row.fs                         \ `row`
require galope/sconstant.fs                   \ string constants
require galope/seconds.fs                     \ `seconds`
require galope/sourcepath.fs
require galope/svariable.fs                   \ string variables
require galope/system_colors.fs
require galope/tilde-tilde.fs                 \ improved `~~`
require galope/to-yyyymmddhhmmss.fs           \ `>yyyymmddhhss`
require galope/two-choose.fs                  \ `2schoose`
' 2choose alias schoose
require galope/x-c-store.fs                   \ `xc!`
require galope/xcase_es.fs                    \ Spanish UTF-8 case table
require galope/xlowercase.fs                  \ `xlowercase` for UTF-8
require galope/xy.fs                          \ current cursor position

\ ----------------------------------------------
\ Flibustre
\ http://programandala.net

require flibustre/different-question.fs  \ `different?`

\ ----------------------------------------------
\ Other

require ../debug/halto2.fs \ XXX TMP -- check points for debugging
false to halto?

pad 0 2constant null$
  \ Null string.

: ?++  ( a -- )
  dup @ 1+ ?dup if  swap !  else  drop  then  ;
  \ Incrementa el contenido de una dirección _a_, si es posible.  En
  \ la práctica el límite es inalcanzable (pues es un número de 32
  \ bitios), pero así queda mejor hecho.
  \
  \ XXX TODO -- confirmar este cálculo, pues depende de si el número
  \ se considera con signo o no

\ vim:filetype=gforth:fileencoding=utf-8

