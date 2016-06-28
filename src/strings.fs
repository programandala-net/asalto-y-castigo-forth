\ strings.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606282010

\ Note: The comments of the code are in Spanish.

\ ==============================================================

str-create tmp-str
  \ Cadena dinámica de texto temporal para usos variados.

: str-get-last-char  ( a -- c )
  dup str-length@ 1- swap str-get-char  ;
  \ Devuelve el último carácter de una cadena dinámica.
  \ XXX TODO -- soporte para UTF-8

: str-get-last-but-one-char  ( a -- c )
  dup str-length@ 2 - swap str-get-char  ;
  \ Devuelve el penúltimo carácter de una cadena dinámica.
  \ XXX TODO -- soporte para UTF-8

: (^uppercase)  ( ca len -- )
  [false] [if]  \ XXX OLD -- antiguo, ascii
    if  dup c@ toupper swap c!  else  drop  then
  [else]  \ UTF-8
    if  dup xc@ xtoupper swap xc!  else  drop  then
  [then]  ;
  \ Convierte en mayúsculas la primera letra de una cadena.

: ^uppercase  ( a1 u -- a2 u )
  >sb 2dup (^uppercase)  ;
  \ Hace una copia de una cadena en el almacén circular
  \ y la devuelve con la primera letra en mayúscula.
  \ Nota: Se necesita para los casos en que no queremos
  \ modificar la cadena original.

: ?^uppercase  ( ca1 len1 f -- ca1 len1 | ca2 len2 )
  ?? ^uppercase  ;
  \ Si _f_ es distinto de cero,
  \ Devuelve una copia _ca2 len2_ de una cadena _ca1 len1_,
  \ poniendo la primera letra en mayúscula.
  \ XXX TODO -- no usado

: -punctuation  ( ca len -- ca len )
  exit \ XXX TMP
  2dup bounds  ?do
    i c@ chr-punct? if  bl i c!  then
  loop  ;
  \ Sustituye por espacios todos los signos de puntuación ASCII de una cadena.
  \ XXX TODO -- recorrer la cadena por caracteres UTF-8
  \ XXX TODO -- sustituir también signos de puntuación UTF-8
  \ XXX FIXME -- no eliminar las marcas "#" de los comandos del sistema

: tmp-str!  ( ca len -- )  tmp-str str-set  ;
  \ Guarda una cadena en la cadena dinámica `tmp-str`.

: tmp-str@  ( -- ca len )  tmp-str str-get  ;
  \ Devuelve el contenido de cadena dinámica `tmp-str`.

: *>verb-ending  ( ca len f -- )
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" n"  else  s" "  then  s" *" replaced
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" n" rot and  s" *" replaced
  [then]  ;
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ ca len = Texto
  \ f = ¿Hay que poner los verbos en plural?

: *>plural-ending  ( ca len f -- )
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" s"  else  s" "  then  s" *" replaced
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" s" rot and  s" *" replaced
  [then]  ;
  \ Cambia por «s» (terminación plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ ca len = Expresión
  \ f = ¿Hay que poner los verbos en plural?
  \ XXX TODO -- no usado

: space+  ( ca1 len1 -- ca2 len2 )  s"  " s+  ;
  \ Añade un espacio al final de una cadena.

: period+  ( ca1 len1 -- ca2 len2 )  s" ." s+  ;
  \ Añade un punto al final de una cadena.

: comma+  ( ca1 len1 -- ca2 len2 )  s" ," s+  ;
  \ Añade una coma al final de una cadena.

: colon+  ( ca1 len1 -- ca2 len2 )  s" :" s+  ;
  \ Añade dos puntos al final de una cadena.

: hyphen+  ( ca1 len1 -- ca2 len2 )  s" -" s+  ;
  \ Añade un guion a una cadena.

: and&  ( ca1 len1 -- ca2 len2 )  s" y" s&  ;
  \ Añade una conjunción «y» al final de una cadena.
  \ XXX TODO -- no usado

: or&  ( ca1 len1 -- ca2 len2 )  s" o" s&  ;
  \ Añade una conjunción «o» al final de una cadena.
  \ XXX TODO -- no usado

: rnd2swap  ( ca1 len1 ca2 len2 -- ca1 len1 ca2 len2 | ca2 len2 ca1 len1 )
  2 random ?? 2swap  ;
  \ Intercambia (con 50% de probabilidad) la posición de dos textos.

: (both)  ( ca1 len1 ca2 len2 -- ca1 len1 ca3 len3 ca2 len2 | ca2 len2 ca3 len3 ca1 len1 )
  rnd2swap s" y" 2swap  ;
  \ Devuelve las dos cadenas recibidas, en cualquier orden,
  \ y separadas en la pila por la cadena «y».

: both  ( ca1 len1 ca2 len2 -- ca3 len3 )
  (both) bs& bs&  ;
  \ Devuelve dos cadenas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los dos resultados posibles serían: «fría y espesa» y «espesa y fría».

: both&  ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  both bs&  ;
  \ Devuelve dos cadenas unidas en cualquier orden por «y»; y concatenada (con separación) a una tercera.

: both?  ( ca1 len1 ca2 len2 -- ca3 len3 )
  (both) s&? bs&  ;
  \ Devuelve al azar una de dos cadenas,
  \ o bien ambas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los cuatro resultados posibles serían:
  \ «espesa», «fría», «fría y espesa» y «espesa y fría».

: both?&  ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  both? bs&  ;
  \ Concatena (con separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.

: both?+  ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  both? bs+  ;
  \ Concatena (sin separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.

\ vim:filetype=gforth:fileencoding=utf-8

