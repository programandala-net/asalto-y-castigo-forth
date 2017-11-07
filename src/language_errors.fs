\ language_errors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711072219

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/fifty-percent-nullify.fs   \ `50%nullify`
require galope/question-question.fs       \ `??`
require galope/s-curly-bracket.fs         \ `s{`
require galope/two-choose.fs              \ `2choose`
require galope/txt-plus.fs                \ `txt+`

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

set-current

\ ==============================================================
\ Error genérico

: generic-language-error$  ( -- ca len )
  'generic-language-error$ count  ;
  \ Devuelve el mensaje de error lingüístico para el nivel 1.

:noname  ( ca len -- )
  2drop generic-language-error$ language-error.  ;
  is generic-language-error
  \ Muestra el mensaje de error lingüístico _ca len_ para el nivel 1.

\ ==============================================================
\ Gestión de los errores específicos

: please$  ( -- ca len )
  s" por favor" 50%nullify  ;
  \ Devuelve «por favor» o vacía.

: (please&)  ( ca1 len1 ca2 len2 -- ca3 len3 )
  2 random ?? 2swap  comma+ 2swap txt+  ;
  \ Añade una cadena _ca2 len2_ al inicio o al final de una cadena
  \ _ca1 len1_, con una coma de separación.

: please&  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  please$ dup if  (please&)  else  2drop  then  ;
  \ Añade «por favor» al inicio o al final de una cadena _ca1 len1_,
  \ con una coma de separación; o bien la deja sin tocar.

: in-the-sentence$  ( -- ca len )
  s{ null$ s" en la frase" s" en el comando" }s  ;
  \ Devuelve una variante de «en la frase» (o una cadena vacía).

: error-comment-0$  ( -- ca len )
  s" sé más clar" player-gender-ending$+  ;
  \ Devuelve la variante 0 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment-1$  ( -- ca len )
  s{ s" exprésate" s" escribe" }s
  s{
  s" más claramente"
  s" más sencillamente"
  s{ s" con más" s" con mayor" }s
  s{ s" sencillez" s" claridad" }s txt+
  }s txt+  ;
  \ Devuelve la variante 1 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment-2-start$  ( -- ca len )
  s{ s" intenta" s" procura" s" prueba a" }s
  s{ s" reescribir" s" expresar" s" escribir" s" decir" }s txt+
  \ XXX TODO -- este "lo" crea problema de concordancia con el final de la frase:
  s{ s"  la frase" s" lo" s"  la idea" }s s+  ;
  \ Devuelve el comienzo de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2-end-0$  ( -- ca len )
  s" de" s{ s" una" s" otra" }s txt+ way$ txt+
  s{ null$ s" un poco" s" algo" }s txt+ s" más" txt+
  s{ s" simple" s" sencilla" s" clara" }s txt+  ;
  \ Devuelve el final 0 de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2-end-1$  ( -- ca len )
  s{ s" más claramente" s" con más sencillez" }s  ;
  \ Devuelve el final 1 de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2$  ( -- ca len )
  error-comment-2-start$
  s{ error-comment-2-end-0$ error-comment-2-end-1$ }s txt+  ;
  \ Devuelve la variante 2 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment$  ( -- ca len )
  error-comment-0$ error-comment-1$ error-comment-2$
  3 2choose please&  ;
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos.

: ^error-comment$  ( -- ca len )  error-comment$ xcapitalized  ;
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula.

:noname  ( ca len -- )
  in-the-sentence$ txt+  3 random
  if    xcapitalized period+ ^error-comment$
  else  ^error-comment$ comma+ 2swap
  then  period+ txt+  language-error.  ;
  is specific-language-error
  \ Muestra un mensaje detallado _ca len_ sobre un error lingüístico,
  \ combinándolo con una frase común.
  \ XXX TODO -- hacer que use coma o punto y coma, al azar

\ ==============================================================
\ Errores específicos

: there-are$  ( -- ca len )
  s{ s" parece haber" s" se identifican" s" se reconocen" }s  ;
  \ Devuelve una variante de «hay» para sujeto plural, comienzo de
  \ varios errores.

: there-is$  ( -- ca len )
  s{ s" parece haber" s" se identifica" s" se reconoce" }s  ;
  \ Devuelve una variante de «hay» para sujeto singular, comienzo de
  \ varios errores.

: there-is-no$  ( -- ca len )
  s" no se" s{ s" identifica" s" encuentra" s" reconoce" }s txt+
  s{ s" el" s" ningún" }s txt+  ;
  \ Devuelve una variante de «no hay», comienzo de varios errores.

:noname  ( -- )
  s{ there-are$ s" dos verbos" txt+
  there-is$ s" más de un verbo" txt+
  there-are$ s" al menos dos verbos" txt+
  }s  language-error  ; is too-many-actions.error
  \ Error de que se ha producido un error porque hay dos verbos en
  \ el comando.

:noname  ( -- )
  s{
  there-are$
  s" dos complementos principales" txt+
  there-is$
  s" más de un complemento principal" txt+
  there-are$
  s" al menos dos complementos principales" txt+
  }s  language-error  ; is too-many-complements.error
  \ Error de que se ha producido un error
  \ porque hay dos complementos principales en el comando.

:noname  ( -- )
  there-is-no$ s" verbo" txt+
  language-error  ; is no-verb.error
  \ Error de que se ha producido un error por falta de verbo en el comando.

:noname  ( -- )
  there-is-no$ s" complemento principal" txt+
  language-error  ; is no-main-complement.error
  \ Error de que se ha producido un error por falta de complemento
  \ principal en el comando.

:noname  ( -- )
  there-is$ s" un complemento principal" txt+
  s" pero el verbo no puede llevarlo" txt+
  language-error  ; is unexpected-main-complement.error
  \ Error de que se ha producido un error por la presencia de
  \ complemento principal en el comando.

:noname  ( -- )
  there-is$ s" un complemento principal no permitido con esta acción" txt+
  language-error  ; is not-allowed-main-complement.error
  \ Error de que se ha producido un error por la presencia de un
  \ complemento principal en el comando que no está permitido.

:noname  ( -- )
  there-is$ s" un complemento instrumental no permitido con esta acción" txt+
  language-error  ; is not-allowed-tool-complement.error
  \ Error de que se ha producido un error por la presencia de un
  \ complemento instrumental en el comando que no está permitido.

:noname  ( -- )
  there-is$ s" un complemento (seudo)preposicional sin completar" txt+
  language-error  ; is unresolved-preposition.error
  \ Error de que se ha producido un error
  \ porque un complemento (seudo)preposicional quedó incompleto.

:noname  ( -- )
  there-is$ s" una (seudo)preposición repetida" txt+
  language-error  ; is repeated-preposition.error
  \ Error de que se ha producido un error por
  \ la repetición de una (seudo)preposición.

:noname  ( -- )
  there-is$ s" una combinación de complementos no permitada" txt+
  language-error  ; is not-allowed-complements.error
  \ Error de que se ha producido un error por
  \ una combinación no permitida de complementos.

\ vim:filetype=gforth:fileencoding=utf-8
