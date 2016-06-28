\ printing.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606282005

\ Note: The comments of the code are in Spanish.

\ ==============================================================

variable #lines
  \ Número de línea del texto que se imprimirá.

variable scroll
  \ Indicador de que la impresión no debe parar.

\ ==============================================================
\ Cadena dinámica para impresión

\ Usamos una cadena dinámica llamada `print-str` para guardar
\ los párrafos enteros que hay que mostrar en pantalla. En
\ esta sección creamos la cadena y palabras útiles para
\ manipularla.

str-create print-str
  \ Cadena dinámica para almacenar el texto antes de imprimirlo
  \ justificado.

: «»-clear  ( -- )  print-str str-clear  ;
  \ Vacía la cadena dinámica `print-str`.

: «»!  ( ca len -- )  print-str str-set  ;
  \ Guarda una cadena en la cadena dinámica `print-str`.

: «»@  ( -- ca len )  print-str str-get  ;
  \ Devuelve el contenido de la cadena dinámica `print-str`.

: «+  ( ca len -- )  print-str str-prepend-string  ;
  \ Añade una cadena al principio de la cadena dinámica `print-str`.

: »+  ( ca len -- )  print-str str-append-string  ;
  \ Añade una cadena al final de la cadena dinámica `print-str`.

: «c+  ( c -- )  print-str str-prepend-char  ;
  \ Añade un carácter al principio de la cadena dinámica `print-str`.

: »c+  ( c -- )  print-str str-append-char  ;
  \ Añade un carácter al final de la cadena dinámica `print-str`.

: «»bl+?  ( u -- f )  0<> print-str str-length@ 0<> and  ;
  \ ¿Se debe añadir un espacio al concatenar una cadena a la cadena
  \ dinámica `print-str`?
  \ u = Longitud de la cadena que se pretende
  \     unir a la cadena dinámica `print-str`

: »&  ( ca len -- )  dup «»bl+? if  bl »c+  then  »+  ;
  \ Añade una cadena al final de la cadena dinámica `print-str`,
  \ con un espacio de separación.

: «&  ( ca len -- )  dup «»bl+? if  bl «c+  then  «+  ;
  \ Añade una cadena al principio de la cadena dinámica `print-str`,
  \ con un espacio de separación.

\ ==============================================================
\ Presto de pausa en la impresión de párrafos

svariable scroll-prompt  \ Guardará el presto de pausa

: scroll-prompt$  ( -- ca len )  scroll-prompt count  ;
  \ Devuelve el presto de pausa.

1 value /scroll-prompt
  \ Número de líneas de intervalo para mostrar un presto.

: scroll-prompt-key  ( -- )  key  bl =  scroll !  ;
  \ Espera la pulsación de una tecla
  \ y actualiza con ella el estado del desplazamiento.

: .scroll-prompt  ( -- )
  trm+save-cursor  scroll-prompt-color
  scroll-prompt$ type  scroll-prompt-key
  trm+erase-line  trm+restore-cursor  ;
  \ Imprime el presto de pausa, espera una tecla y borra el presto.

: (scroll-prompt?)  ( u -- f )
  dup 1+ #lines @ <>
  swap /scroll-prompt mod 0=  and  ;
  \ ¿Se necesita imprimir un presto para la línea actual?  Se tienen
  \ que cumplir dos condiciones.  1) ¿Es distinta de la última?; 2) ¿Y
  \ el intervalo es correcto?.  _u_ es la línea actual del párrafo que
  \ se está imprimiendo.
  \
  \ XXX TODO factorizar para no tener que comentar las condiciones.

: scroll-prompt?  ( u -- f )
  scroll @ if  drop false  else  (scroll-prompt?)  then  ;
  \ ¿Se necesita imprimir un presto para la línea actual?
  \ u = Línea actual del párrafo que se está imprimiendo
  \ Si el valor de `scroll` es «verdadero», se devuelve «falso»;
  \ si no, se comprueban las otras condiciones.
  \ ." L#" dup . ." /" #lines @ . \ XXX INFORMER

: .scroll-prompt?  ( u -- )  scroll-prompt? ?? .scroll-prompt  ;
  \ Imprime un presto y espera la pulsación de una tecla, si
  \ corresponde a la línea en curso.  _u_ es la línea actual del
  \ párrafo que se está imprimiendo.
  \
  \ \ XXX TODO -- no se usa

\ ==============================================================
\ Impresión de párrafos justificados

2 constant default-indentation
  \ Indentación predeterminada de la primera línea de cada párrafo
  \ (en caracteres).

8 constant max-indentation
  \ Indentación máxima de la primera línea de cada párrafo
  \ (en caracteres).

variable /indentation
  \ Indentación en curso de la primera línea de cada párrafo
  \ (en caracteres).

variable indent-first-line-too?
  \ ¿Se indentará también la línea superior de la pantalla,
  \ si un párrafo empieza en ella?

: not-first-line?  ( -- f )  row 0>  ;
  \ ¿El cursor no está en la primera línea?

: indentation?  ( -- f )  not-first-line? indent-first-line-too? @ or  ;
  \ ¿Indentar la línea actual?

: (indent)  ( -- )  /indentation @ print_indentation  ;
  \ Indenta.

: indent  ( -- )  indentation? ?? (indent)  ;
  \ Indenta si es necesario.

: background-line  ( -- )  background-color cols spaces  ;
  \ Colorea una línea con el color de fondo.

: ((print_cr))  ( -- )
  cr trm+save-current-state background-line
  trm+restore-current-state  ;
  \ Salto de línea alternativo para los párrafos justificados.
  \ Colorea la nueva línea con el color de fondo, lo que parchea
  \ el problema de que las nuevas líneas volvían a aparecer
  \ con el color predeterminado de la terminal.
  \ XXX TODO -- inacabado, en pruebas
  \ background-color cols #printed @ - spaces  \ final de línea
  \ blue paper cols #printed @ - spaces key drop  \ XXX INFORMER

' ((print_cr)) is (print_cr)
  \ Redefinir `(print_cr)` (de galope/print.fs).

: cr+  ( -- )
  [false] [if]  \ XXX OLD
    print_cr indent
  [else]  \ XXX NEW
    \ XXX TODO -- pruebas para solucionar problema de la línea en blanco
    \ row last-row <> column or ?? print_cr indent
    column 0<> ?? print_cr indent
  [then]  ;
  \ Hace un salto de línea y una indentación, para el sistema
  \ de impresión de textos justificados.

: paragraph  ( ca len -- )  cr+ print  ;
  \ Imprime un texto _ca len_ justificado como inicio de un párrafo.

: (language-error)  ( ca len -- )
  language-error-color paragraph system-colors  ;
  \ Imprime una cadena como un informe de error lingüístico.
  \ XXX TODO -- renombrar

: action-error  ( ca len -- )
  action-error-color paragraph system-colors  ;
  \ Imprime una cadena como un informe de error de un comando.

: system-error  ( ca len -- )
  system-error-color paragraph system-colors  ;
  \ Imprime una cadena como un informe de error del sistema.

: narrate  ( ca len -- )
  narration-color paragraph system-colors  ;
  \ Imprime una cadena como una narración.

\ ==============================================================
\ Pausas y prestos en la narración

variable indent-pause-prompts?
  \ ¿Hay que indentar también los prestos?

: indent-prompt  ( -- )
  indent-pause-prompts? @ ?? indent  ;
  \ Indenta antes de un presto, si es necesario.

: .prompt  ( ca len -- )  print_cr indent-prompt print  ;
  \ Imprime un presto.

: wait  ( +n|-n -- )  dup 0< if  key 2drop  else  seconds  then  ;
  \ Hace una pausa de _+n_ segundos (o _-n_ para una pausa sin fin
  \ hasta la pulsación de una tecla).

variable narration-break-seconds
  \ Segundos de espera en las pausas de la narración.

svariable narration-prompt
  \ Presto usado en las pausas de la narración.

: narration-prompt$  ( -- ca len )  narration-prompt count  ;
  \ Devuelve el presto usado en las pausas de la narración.

: .narration-prompt  ( -- )
  narration-prompt-color narration-prompt$ .prompt  ;
  \ Imprime el presto de fin de escena.

: (break)  ( +n|-n -- )
  [false] [if]
    \ XXX OLD -- antiguo. versión primera, que no coloreaba la línea
    press-key  trm+erase-line print_start_of_line
  [else]
    \ XXX NEW
    press-key  print_start_of_line
    trm+save-current-state background-line trm+restore-current-state
  [then]  ;
  \ Hace una pausa de _+n_
  \ segundos (o _-n_ para hacer una pausa indefinida hasta la
  \ pulsación de una tecla) borra la línea en que se ha mostrado el
  \ presto de pausa y restaura la situación de impresión para no
  \ afectar al siguiente párrafo.

: (narration-break)  ( +n|-n -- )
  .narration-prompt (break)  ;
  \ Alto en la narración: Muestra un presto y hace una pausa de
  \ _+n_ segundos (o _-n_ para hacer una pausa indefinida hasta la
  \ pulsación de una tecla).

: narration-break  ( -- )
  narration-break-seconds @ ?dup ?? (narration-break)  ;
  \ Alto en la narración, si es preciso.

variable scene-break-seconds
  \ Segundos de espera en las pausas de final de escena.

svariable scene-prompt
  \ Presto de cambio de escena.

: scene-prompt$  ( -- ca len )  scene-prompt count  ;
  \ Devuelve el presto de cambio de escena.

: .scene-prompt  ( -- )
  scene-prompt-color scene-prompt$ .prompt  ;
  \ Imprime el presto de fin de escena.

: (scene-break)  ( +n|-n -- )
  .scene-prompt (break)  scene-page? @ ?? new-page  ;
  \ Final de escena: Muestra un presto y hace una pausa de
  \ _+n_ segundos (o _-n_ para hacer una pausa indefinida hasta la
  \ pulsación de una tecla).

: scene-break  ( -- )
  scene-break-seconds @ ?dup ?? (scene-break)  ;
  \ Final de escena, si es preciso.

: about-pause  ( -- )  20 (break)  ;
  \ Pausa tras los créditos.

\ ==============================================================
\ Impresión de citas de diálogos

s" —" sconstant dash$     \ Raya (Unicode $2014, #8212).
s" «" sconstant lquote$   \ Comilla castellana de apertura.
s" »" sconstant rquote$   \ Comilla castellana de cierre.

: str-with-rquote-only?  ( a -- f )
  >r rquote$ 0 r@ str-find -1 >
  lquote$ 0 r> str-find -1 = and  ;
  \ ¿Hay en una cadena dinámica una comilla castellana de cierre pero
  \ no una de apertura?
  \ XXX TODO -- factor out

: str-with-period?  ( a -- f )
  dup str-get-last-char [char] . =
  swap str-get-last-but-one-char [char] . <> and  ;
  \ ¿Termina una cadena dinámica con un punto,
  \ y además el penúltimo no lo es? (para descartar que se trate de
  \ puntos suspensivos).
  \ XXX FIXME -- fallo: no se pone punto tras puntos suspensivos
  \ XXX TODO -- factorizar
  \ XXX TODO -- usar signo de puntos suspensivos UTF-8

: str-prepend-quote  ( a -- )
  lquote$ rot str-prepend-string  ;
  \ Añade a una cadena dinámica una comilla castellana de apertura.

: str-append-quote  ( a -- )
  rquote$ rot str-append-string  ;
  \ Añade a una cadena dinámica una comilla castellana de cierre.

: str-add-quotes  ( a -- )
  dup str-append-quote str-prepend-quote  ;
  \ Encierra una cadena dinámica entre comillas castellanas.

false [if]  \ XXX OLD -- obsoleto
: str-add-quotes-period  ( a -- )
  dup str-pop-char drop  \ Eliminar el último carácter, el punto
  dup str-add-quotes  \ Añadir las comillas
  s" ." rot str-append-string  ; \ Añadir de nuevo el punto
  \ Encierra una cadena dinámica (que termina en punto) entre comillas castellanas

[then]
: (quotes+)  ( -- )
  tmp-str dup str-with-period?  \ ¿Termina con un punto?
  if    dup str-pop-char drop  \ Eliminarlo
  then  dup str-add-quotes s" ." rot str-append-string  ;
  \ Añade comillas castellanas a una cita de un diálogo en la cadena dinámica `tmp-str`.

: quotes+  ( ca1 len1 -- ca2 len2 )
  tmp-str!  tmp-str str-with-rquote-only?
  if  \ Es una cita con aclaración final
    tmp-str str-prepend-quote  \ Añadir la comilla de apertura
  else  \ Es una cita sin aclaración, o con aclaración en medio
    (quotes+)
  then  tmp-str@  ;
  \ Añade comillas castellanas a una cita de un diálogo.

: dash+  ( ca1 len1 -- ca2 len2 )
  dash$ 2swap s+  ;
  \ Añade la raya a una cita de un diálogo.

: quoted  ( ca1 len1 -- ca2 len2 )
  castilian-quotes? @ if  quotes+  else  dash+  then  ;
  \ Pone comillas o raya a una cita de un diálogo.

: speak  ( ca len -- )
  quoted speech-color paragraph system-colors  ;
  \ Imprime una cita de un diálogo.

\ vim:filetype=gforth:fileencoding=utf-8

