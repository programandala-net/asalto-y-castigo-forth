\ printing.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201712041706

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-false.fs       \ `[false]`
require galope/column.fs              \ `column`
require galope/l-type.fs              \ `ltype`, `/ltype`, etc.
require galope/question-question.fs   \ `??`
require galope/question-seconds.fs    \ `?seconds`
require galope/row.fs                 \ `row`
require galope/sconstant.fs           \ `sconstant`
require galope/svariable.fs           \ `svariable`
require galope/system-colors.fs       \ `system-colors`

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

require ffl/str.fs

set-current

\ ==============================================================
\ Presto de pausa en la impresión de párrafos

svariable scroll-prompt ( -- ca )
  \ Presto de pausa.

: scroll-prompt$ ( -- ca len ) scroll-prompt count ;
  \ Devuelve el presto de pausa.

' scroll-prompt$ is lprompt$

\ ==============================================================
\ Impresión de párrafos justificados

2 constant default-indentation
  \ Indentación predeterminada de la primera línea de cada párrafo
  \ (en caracteres).

8 constant max-indentation
  \ Indentación máxima de la primera línea de cada párrafo
  \ (en caracteres).

: background-line ( -- ) background-color cols spaces ;
  \ Colorea una línea con el color de fondo.

false [if]

  \ XXX OLD -- 2017-11-17: It seems this in no longer needed on Gforth
  \ 0.7.9.

: (cr) ( -- )
  cr trm+save-current-state background-line
     trm+restore-current-state ;
  \ Salto de línea alternativo para los párrafos justificados.
  \ Colorea la nueva línea con el color de fondo, lo que parchea
  \ el problema de que las nuevas líneas volvían a aparecer
  \ con el color predeterminado de la terminal.
  \ XXX TODO -- inacabado, en pruebas
  \ background-color cols #printed @ - spaces  \ final de línea
  \ blue paper cols #printed @ - spaces key drop  \ XXX INFORMER

' (cr) is ((lcr))

[then]

: language-error. ( ca len -- )
  language-error-color /ltype system-colors ;
  \ Imprime una cadena como un informe de error lingüístico.

: action-error. ( ca len -- )
  action-error-color /ltype system-colors ;
  \ Imprime una cadena como un informe de error de un comando.

: system-error. ( ca len -- )
  system-error-color /ltype system-colors ;
  \ Imprime una cadena como un informe de error del sistema.

: narrate ( ca len -- )
  narration-color /ltype system-colors ;
  \ Imprime una cadena como una narración.

\ ==============================================================
\ Pausas y prestos en la narración

variable indent-pause-prompts?
  \ ¿Hay que indentar también los prestos?

: indent-prompt ( -- )
  indent-pause-prompts? @ ?? indent ;
  \ Indenta antes de un presto, si es necesario.

: wait ( +n|-n -- ) dup 0< if  key 2drop  else  ?seconds  then ;
  \ Hace una pausa de _+n_ segundos (o _-n_ para una pausa sin fin
  \ hasta la pulsación de una tecla).

' wait is lprompt-pause

: .prompt ( +n|-n ca len -- )
  ((lcr)) next-lrow no-ltyped indent-prompt lprompted ;
  \ Imprime un presto _ca len_ con segundos de pausa _+n|-n_.

variable narration-break-seconds
  \ Segundos de espera en las pausas de la narración.

svariable narration-prompt
  \ Presto usado en las pausas de la narración.

: narration-prompt$ ( -- ca len ) narration-prompt count ;
  \ Devuelve el presto usado en las pausas de la narración.

: .narration-prompt ( +n|-n -- )
  narration-prompt-color
  narration-prompt$ .prompt previous-lrow ;
  \ Alto en la narración: Muestra el presto de fin de escena y hace
  \ una pausa de _+n_ segundos (o _-n_ para hacer una pausa indefinida
  \ hasta la pulsación de una tecla).

: narration-break ( -- )
  narration-break-seconds @ ?dup ?? .narration-prompt ;
  \ Alto en la narración, si es preciso.

variable scene-break-seconds
  \ Segundos de espera en las pausas de final de escena.

svariable scene-prompt
  \ Presto de cambio de escena.

: scene-prompt$ ( -- ca len ) scene-prompt count ;
  \ Devuelve el presto de cambio de escena.

: .scene-prompt ( +n|-n -- )
  scene-prompt-color scene-prompt$ .prompt previous-lrow ;
  \ Muestra el presto de escena y hace una pausa de _+n_ segundos (o
  \ _-n_ para hacer una pausa indefinida hasta la pulsación de una
  \ tecla).

: (scene-break) ( +n|-n -- )
  .scene-prompt scene-page? @ ?? new-page ;
  \ Final de escena: Muestra el presto de escena y hace una pausa de
  \ _+n_ segundos (o _-n_ para hacer una pausa indefinida hasta la
  \ pulsación de una tecla).  Si está así configurado, también borra
  \ la pantalla al final.

: scene-break ( -- )
  scene-break-seconds @ ?dup ?? (scene-break) ;
  \ Final de escena, si es preciso.

\ ==============================================================
\ Impresión de citas de diálogos

s" —" sconstant dash$     \ Raya (Unicode $2014, #8212).
s" «" sconstant lquote$   \ Comilla castellana de apertura.
s" »" sconstant rquote$   \ Comilla castellana de cierre.

: str-with-rquote-only? ( a -- f )
  >r rquote$ 0 r@ str-find -1 >
  lquote$ 0 r> str-find -1 = and ;
  \ ¿Hay en una cadena dinámica una comilla castellana de cierre pero
  \ no una de apertura?
  \ XXX TODO -- factor out

: str-with-period? ( a -- f )
  dup str-get-last-char [char] . =
  swap str-get-last-but-one-char [char] . <> and ;
  \ ¿Termina una cadena dinámica con un punto,
  \ y además el penúltimo no lo es? (para descartar que se trate de
  \ puntos suspensivos).
  \ XXX FIXME -- fallo: no se pone punto tras puntos suspensivos
  \ XXX TODO -- factorizar
  \ XXX TODO -- usar signo de puntos suspensivos UTF-8

: str-prepend-quote ( a -- )
  lquote$ rot str-prepend-string ;
  \ Añade a una cadena dinámica una comilla castellana de apertura.

: str-append-quote ( a -- )
  rquote$ rot str-append-string ;
  \ Añade a una cadena dinámica una comilla castellana de cierre.

: str-add-quotes ( a -- )
  dup str-append-quote str-prepend-quote ;
  \ Encierra una cadena dinámica entre comillas castellanas.

false [if]  \ XXX OLD -- obsoleto
: str-add-quotes-period ( a -- )
  dup str-pop-char drop  \ Eliminar el último carácter, el punto
  dup str-add-quotes  \ Añadir las comillas
  s" ." rot str-append-string  ; \ Añadir de nuevo el punto
  \ Encierra una cadena dinámica (que termina en punto) entre comillas castellanas

[then]
: (quotes+) ( -- )
  tmp-str dup str-with-period?  \ ¿Termina con un punto?
  if    dup str-pop-char drop  \ Eliminarlo
  then  dup str-add-quotes s" ." rot str-append-string ;
  \ Añade comillas castellanas a una cita de un diálogo en la cadena dinámica `tmp-str`.

: quotes+ ( ca1 len1 -- ca2 len2 )
  tmp-str!  tmp-str str-with-rquote-only?
  if  \ Es una cita con aclaración final
    tmp-str str-prepend-quote  \ Añadir la comilla de apertura
  else  \ Es una cita sin aclaración, o con aclaración en medio
    (quotes+)
  then  tmp-str@ ;
  \ Añade comillas castellanas a una cita de un diálogo.

: dash+ ( ca1 len1 -- ca2 len2 )
  dash$ 2swap s+ ;
  \ Añade la raya a una cita de un diálogo.

: quoted ( ca1 len1 -- ca2 len2 )
  castilian-quotes? @ if  quotes+  else  dash+  then ;
  \ Pone comillas o raya a una cita de un diálogo.

: speak ( ca len -- )
  quoted speech-color /ltype system-colors ;
  \ Imprime una cita de un diálogo.

\ ==============================================================
\ Change log

\ 2017-11-17: Replace the ad-hoc left-justified printing system with
\ Galope's module <l-type.fs>.
\
\ 2017-11-18: Update to Galope 0.141.0.
\
\ 2017-12-04: Update to Galope 0.157.0: Replace `seconds` with
\ `?seconds`.

\ vim:filetype=gforth:fileencoding=utf-8
