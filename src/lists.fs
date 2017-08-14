\ lists.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last modified 201708141855
\ See change log at the end of the file

\ ==============================================================
\ Requirements

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
\ Main

variable #listed
  \ Count of elements already listed.

variable #elements
  \ Total count of list elements.

: list-separator$ ( u1 u2 -- ca len )
  ?dup if
    1+ = if s"  y " else s" , " then
  else 0 then ;
  \ Return the proper separator _ca len_, "y" ("and" in Spanish), ","
  \ or nothing, to the dynamic string `out-str`, being _u1_ the total
  \ number of items in the list and _u2_ the number of items already
  \ listed.

: (list-separator) ( u1 u2 -- )
  1+ = if   s" y" out-str str-append-txt
       else s" ," out-str str-append-string then ;
  \ Add the proper separator, "y" ("and" in Spanish) or ",", to the
  \ dynamic string `out-str`, being _u1_ the total number of items in
  \ the list and _u2_ the number of items already listed.

: list-separator ( u1 u2 -- )
  ?dup if (list-separator) else drop then ;
  \ Add the proper separator (or nothing) to the dynamic string
  \ `out-str`, being _u1_ the total number of items in the list and
  \ _u2_ the number of items already listed.

: /list++ ( u entity1 entity2 -- u | u+1 )
  dup must-be-listed?
  if location = abs + else 2drop then ;
  \ If _entity1_ can be listed and it's the location of _entity2_,
  \ increment counter _u_.

: /list ( entity -- u )
  0 \ Contador
  #entities 0 do over i #>entity /list++ loop nip ;
  \ Return number _u_ of entities whose location is _entity_ and
  \ can be listed.

: (worn)$ ( entity -- ca len )
  s" (puest" rot noun-ending s" )" s+ s+ ;
  \ Return string _ca len_ containing "(puesto/a/s)" ("worn" in
  \ Spanish), depending on the gender and number of _entity_.

: (worn)& ( ca1 len1 a -- ca1 len1 | ca2 len2 )
  dup is-worn? if (worn)$ txt+ else drop then ;
  \ If needed, add a note to string _ca1 len1_, to indicate
  \ _entity_ is a worn cloth, returning the result string _ca2 len2_.

: (content-list) ( entity -- )
  #elements @ #listed @ list-separator
  dup full-name-as-direct-complement
  rot (worn)& out-str str-append-txt #listed ++ ;
  \ Add name of _entity_ (with a proper separator), to the list
  \ hold in dynamic srting `out-str`.

: about-to-list ( entity -- n ) #listed off /list dup #elements ! ;
  \ Prepare a list of entities whose location is _entity_, and
  \ return the number _n_ of entities that will be listed.

: content-list ( entity -- ca len )
  out-str str-clear
  dup about-to-list if
    #entities 1 do
      dup i #>entity dup must-be-listed? if
        is-there? if i #>entity (content-list) then
      else 2drop then
    loop s" ." out-str str-append-string
  then drop out-str str-get ;
  \ Return a comma-separated list _ca len_ of entities whose location
  \ is _entity_.

: .present ( -- )
  my-location content-list dup
  if   s" Ves" s" Puedes ver" 2 2choose 2swap txt+ narrate
  else 2drop then ;
  \ List the present entities.

\ ==============================================================
\ Change log

\ 2017-08-14: Translate comments from Spanish to English. Update
\ source style and stack notation.

\ vim:filetype=gforth:fileencoding=utf-8

