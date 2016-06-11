\ halto2.fs

\ Halto2
\ (C) 2011 Marcos Cruz (programandala.net)
\ Licencia/Permesilo/License:
\ http://programandala.net/license

\ Simple debugger written in Forth.

\ Tested in Gforth and SP-Forth.

\ 2011-12-02 First version, based on the old "halto" (written by the same author in 2005). Factorized and improved.
\ 2011-12-03 New user interface words.
\ 2011-12-17 Bug fixed in ?HALTO" .
\ 2011-12-17 New words ?HALTO[ and (?HALTO") .
\ 2011-12-17 New version doesn't work fine. Meanwhile, [FIRST_VERSION] is used to compile the first version instead.
\ 2011-12-22 Fixed: the default exit flag in HALTO-FINISHED? troubled some commands. Now every command leaves its own flag.
\ 2012-01-01 The stack notation for stream input now is standard.
\ 2012-01-01 Draft for command "Off", to turn off the global switch (it makes no effect yet, because the break points are already compiled).

cr .( Halto2)

true constant [first_version]

true value halto?  \ Global switch; value to turn on and off the break points

\ Stack pointer:

variable halto-stack>
: halto-pick  ( -- u )
	halto-stack> @ pick
	;

\ Screen output:

: .halto-stack  ( -- )
	cr ." ( " .s ." -- ) "
	." < " depth  if  halto-pick .  then  ." >"
	;
: .halto-commands  ( -- )
	cr ." < ? > Emit Type dumP Binary Decimal Hex Leave Quit bYe "
	;
: .halto  ( -- )
	cr ." HALTO " 
	;

\ Commands:

: halto-stack++  ( -- )
	depth 1- halto-stack> @ swap < abs halto-stack> +!
	;
: halto-stack--  ( -- )
	halto-stack> @ 0> halto-stack> +!
	;
: halto-fetch  ( -- )
	halto-pick cr ? 
	;
: halto-dump  ( -- )
	halto-pick cr 256 dump 
	;
: halto-off  ( -- )  \ Not used yet!!!
  false to halto?
  ;
: halto-type  ( -- )
	halto-pick halto-pick cr type 
	;
: halto-emit  ( -- )
	halto-pick cr emit
	;

\ Command dispatcher:

: halto-finished?  ( c -- f )
	case
		[char] <  of  halto-stack--  false  endof
		[char] >  of  halto-stack++  false  endof
		[char] ?  of  halto-fetch  false  endof
		[char] b  of  2 base !  false  endof
		[char] d  of  10 base !  false  endof
		[char] e  of  halto-emit  false  endof
		[char] h  of  16 base !  false  endof
		[char] l  of  true  endof  
		\ [char] o  of  halto-off  true  endof  
		[char] p  of  halto-dump  false  endof
		[char] q  of  cr quit  endof
		[char] t  of  halto-type  false  endof
		[char] y  of  bye  endof
	endcase
	;

\ Main:

: halto-init  ( -- )
	0 halto-stack> !
	;
: halto-menu  ( -- )
	halto-init
	begin
		.halto-stack .halto-commands
		key halto-finished?
	until  cr
;

\ Core of the user interface:

: (halto#)  ( n f -- )
	if  .halto [char] # emit . halto-menu  else  drop  then
	;
: ((halto))  ( a u -- )
	.halto type halto-menu
	;
: (halto)  ( a u f -- )
	if  ((halto))  else  2drop  then
	;
: (halto")  ( a u f -- )
	if  postpone sliteral postpone ((halto))  else  2drop  then
	;

[first_version] 0=  [if]

: (?halto")  ( f c "text<double quote>" -- )
	parse rot halto? and (halto")
	; 

[then]

: halto-compile-only  ( -- )
	state @ 0= abort" Halto2 error: compile only"
	; 

\ User interface (words to create break points):

: halto#  ( n -- )
	halto? (halto#)
	;
: ?halto#  ( n f -- )
	halto? and (halto#)
	;
: halto  ( a u -- )
	halto? (halto)
	;
: ?halto  ( a u f -- )
	halto? and (halto)
	;
: halto"  ( "text<double quote>" -- )
	halto-compile-only  [char] " parse halto? (halto")
	; immediate

[first_version]  [if]  

: ?halto"  ( f "text<double quote>" -- )
	halto-compile-only  [char] " parse rot halto? and (halto")
	; immediate
: ?halto[  ( f "text<]>" -- )
	halto-compile-only  [char] ] parse rot halto? and (halto")
	; immediate

[else]
 
: ?halto"  ( f "text<double quote>" -- )
	halto-compile-only  [char] " (?halto")
	; immediate
: ?halto[  ( f "text<]>" -- )
	halto-compile-only  [char] ] (?halto")
	; immediate

[then]
