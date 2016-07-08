\ language_errors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607081914

\ Note: The comments of the code are in Spanish.

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
  s" por favor" s?  ;
  \ Devuelve «por favor» o vacía.

: (please&)  ( ca1 len1 ca2 len2 -- ca3 len3 )
  2 random ?? 2swap  comma+ 2swap s&  ;
  \ Añade una cadena _ca2 len2_ al inicio o al final de una cadena
  \ _ca1 len1_, con una coma de separación.

: please&  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  please$ dup if  (please&)  else  2drop  then  ;
  \ Añade «por favor» al inicio o al final de una cadena _ca1 len1_,
  \ con una coma de separación; o bien la deja sin tocar.

: in-the-sentence$  ( -- ca len )
  s{ null$ s" en la frase" s" en el comando" s" en el texto" }s  ;
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
  s{ s" sencillez" s" claridad" }s&
  }s s&  ;
  \ Devuelve la variante 1 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment-2-start$  ( -- ca len )
  s{ s" intenta" s" procura" s" prueba a" }s
  s{ s" reescribir" s" expresar" s" escribir" s" decir" }s&
  \ XXX TODO -- este "lo" crea problema de concordancia con el final de la frase:
  s{ s"  la frase" s" lo" s"  la idea" }s+  ;
  \ Devuelve el comienzo de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2-end-0$  ( -- ca len )
  s{ s" de" s" otra" }s way$ s&?
  s{ null$ s" un poco" s" algo" }s& s" más" s&
  s{ s" simple" s" sencilla" s" clara" }s&  ;
  \ Devuelve el final 0 de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2-end-1$  ( -- ca len )
  s{ s" más claramente" s" con más sencillez" }s  ;
  \ Devuelve el final 1 de la variante 2 del mensaje de
  \ acompañamiento para los errores lingüísticos.

: error-comment-2$  ( -- ca len )
  error-comment-2-start$
  s{ error-comment-2-end-0$ error-comment-2-end-1$ }s&  ;
  \ Devuelve la variante 2 del mensaje de acompañamiento para los
  \ errores lingüísticos.

: error-comment$  ( -- ca len )
  error-comment-0$ error-comment-1$ error-comment-2$
  3 schoose please&  ;
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos.

: ^error-comment$  ( -- ca len )  error-comment$ ^uppercase  ;
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula.

:noname  ( ca len -- )
  in-the-sentence$ s&  3 random
  if    ^uppercase period+ ^error-comment$
  else  ^error-comment$ comma+ 2swap
  then  period+ s&  language-error.  ;
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
  s" no se" s{ s" identifica" s" encuentra" s" reconoce" }s&
  s{ s" el" s" ningún" }s&  ;
  \ Devuelve una variante de «no hay», comienzo de varios errores.

:noname  ( -- )
  s{ there-are$ s" dos verbos" s&
  there-is$ s" más de un verbo" s&
  there-are$ s" al menos dos verbos" s&
  }s  language-error  ; to too-many-actions-error#
  \ Informa de que se ha producido un error porque hay dos verbos en
  \ el comando.

:noname  ( -- )
  s{
  there-are$
  s" dos complementos secundarios" s&
  there-is$
  s" más de un complemento secundario" s&
  there-are$
  s" al menos dos complementos secundarios" s&
  }s  language-error  ; to too-many-complements-error#
  \ Informa de que se ha producido un error
  \ porque hay dos complementos secundarios en el comando.
  \ XXX TMP

:noname  ( -- )
  there-is-no$ s" verbo" s&
  language-error  ; to no-verb-error#
  \ Informa de que se ha producido un error por falta de verbo en el comando.

:noname  ( -- )
  there-is-no$ s" complemento principal" s&
  language-error  ; to no-main-complement-error#
  \ Informa de que se ha producido un error por falta de complemento
  \ principal en el comando.

:noname  ( -- )
  there-is$ s" un complemento principal" s&
  s" pero el verbo no puede llevarlo" s&
  language-error  ; to unexpected-main-complement-error#
  \ Informa de que se ha producido un error por la presencia de
  \ complemento principal en el comando.

:noname  ( -- )
  there-is$ s" un complemento secundario" s&
  s" pero el verbo no puede llevarlo" s&
  language-error  ; to unexpected-secondary-complement-error#
  \ Informa de que se ha producido un error por la presencia de
  \ complemento secundario en el comando.

:noname  ( -- )
  there-is$ s" un complemento principal no permitido con esta acción" s&
  language-error  ; to not-allowed-main-complement-error#
  \ Informa de que se ha producido un error por la presencia de un
  \ complemento principal en el comando que no está permitido.

:noname  ( -- )
  there-is$ s" un complemento principal no permitido con esta acción" s&
  language-error  ; to not-allowed-tool-complement-error#
  \ Informa de que se ha producido un error por la presencia de un
  \ complemento instrumental en el comando que no está permitido.

:noname  ( -- )
  s" [Con eso no puedes]"
  narrate  ; to useless-tool-error#
  \ Informa de que se ha producido un error
  \ porque una herramienta no especificada no es la adecuada.
  \ XXX TODO -- inconcluso

:noname  ( -- )
  s" [Con" wrong-entity @ full-name s& s" no puedes]" s&
  narrate  ; to useless-what-tool-error#
  \ Informa de que se ha producido un error
  \ porque el ente `what` no es la herramienta adecuada.
  \ XXX TODO -- inconcluso
  \ XXX TODO -- distinguir si la llevamos, si está presente, si es conocida...

:noname  ( -- )
  there-is$ s" un complemento (seudo)preposicional sin completar" s&
  language-error  ; to unresolved-preposition-error#
  \ Informa de que se ha producido un error
  \ porque un complemento (seudo)preposicional quedó incompleto.

:noname  ( -- )
  there-is$ s" una (seudo)preposición repetida" s&
  language-error  ; to repeated-preposition-error#
  \ Informa de que se ha producido un error por
  \ la repetición de una (seudo)preposición.

\ vim:filetype=gforth:fileencoding=utf-8
