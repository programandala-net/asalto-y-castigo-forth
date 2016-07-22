\ out-str.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607212213

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/str-append-txt.fs      \ `str-append-txt`
require galope/str-prepend-txt.fs     \ `str-prepend-txt`

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

require ffl/str.fs

set-current

\ ==============================================================

str-create out-str
  \ Dynamic string used by some algorithms in order to build the
  \ text that will be printed at the end.

