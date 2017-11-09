\ keyboard_input.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711091725
\ See change log at the end of the file

\ ==============================================================

get-current forth-wordlist set-current

require ../lib/stringstack.fs
  \ Speuler's string stack.

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/slash-csv.fs    \ `/csv`
require galope/s-variable.fs   \ `svariable`
require galope/x-lowercase.fs  \ `xlowercase`

set-current

require talanto/parser.data.fs

\ ==============================================================
\ Command input

svariable command ( -- a )
  \ Command buffer.

svariable command-prompt ( -- a )

: command-prompt$  ( -- ca len )  command-prompt count  ;

: /command  ( -- u )
  cols /indentation @ - 1-
  cr-after-command-prompt? @ 0= abs command-prompt$ nip * -
  cr-after-command-prompt? @ 0= space-after-command-prompt? @ and abs -  ;
  \ Return the maximun length of a command.  The calculation is done
  \ in three steps (each line of the code is one step): 1) available
  \ columns, minus the current indentation, plus one for the space
  \ used by the cursor at the end of the line; 2) substract the lenght
  \ of the prompt, unless it's followed by a carriage return; 3)
  \ substract 1 if the prompt is followed by a space and not by a
  \ carriage return.

: .command-prompt  ( -- )
  command-prompt$ command-prompt-color paragraph
  cr-after-command-prompt? @
  if    cr+
  else  space-after-command-prompt?
        if  background-color space  then
  then  ;
  \ Display the command prompt.

: ((accept-input))  ( -- ca len )
  input-color command dup /command accept
  str+strip 2dup xlowercase  ;
  \ Accept a player's command and return it as string _ca len_
  \ in lower case and without surrounding spaces.
  \ XXX TODO -- Rename.

: split-input  ( ca1 len1 -- ca2 len2 )
  /csv 1- 0 ?do  push$  loop  ;
  \ Divide string _ca1 len1_ into its substrings that are are separated
  \ by a comma. Store the substrings on the string stack except the
  \ first one, which is returned as _ca2 len2_. If _ca1 len1_ has no
  \ comma, _ca2 len2_ is a copy of of _ca1 len1_.

: (accept-input)  ( wid -- ca len )
  1 set-order  .command-prompt ((accept-input))  restore-wordlists
  split-input  ;
  \ Accept a player's command using words from word list _wid_,
  \ and return the command as _ca len_, in lower case and without
  \ surrounding spaces.

: remaining-input?  ( -- f )  depth$ 0<>  ;

: get-remaining-input  ( -- ca len )  pop$  ;

: accept-input  ( wid -- ca len )
  remaining-input?  dup reuse-previous-action !
  if    drop get-remaining-input narration-break
  else  (accept-input)  then  ;
  \ XXX TODO -- Move `narration-break` at the end of the location
  \ entry.  Better yet, make it configurable and use a specific word,
  \ `command-break`.

: accept-command  ( -- ca len )  player-wordlist accept-input  ;
  \ Accept a player's command and return it as string _ca len_
  \ in lower case and without surrounding spaces.

\ ==============================================================
\ Change log

\ 2017-11-07: Update name of Galope module.  Translate comments into
\ English and improve documentation.
\
\ 2017-11-09: Update requirements to Galope 0.120.0.

\ vim:filetype=gforth:fileencoding=utf-8
