\ config_variables.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607071351

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Variables de configuración

\ Algunas variables de configuración
\ (el resto se crea en sus propias secciones)

variable woman-player?
  \ ¿El jugador es una mujer?

variable castilian-quotes?
  \ ¿Usar comillas castellanas en las citas, en lugar de raya?

variable location-page?
  \ ¿Borrar la pantalla antes de entrar en un escenario o de describirlo?

variable cr?
  \ ¿Separar los párrafos con una línea en blanco?

variable ignore-unknown-words?
  \ ¿Ignorar las palabras desconocidas?
  \ XXX TODO -- no usado

variable scene-page?
  \ ¿Borrar la pantalla después de la pausa de los cambios de escena?

variable language-errors-verbosity
  \ Nivel de detalle de los mensajes de error lingüístico.

svariable 'generic-language-error$
  \ Mensaje de error lingüístico para el nivel 1.

variable action-errors-verbosity
  \ Nivel de detalle de los mensajes de error operativo.

svariable 'action-error-general-message$
  \ Mensaje de error operativo para el nivel 1.

0 constant min-errors-verbosity
  \ Nivel mínimo para el detalle de errores.

2 constant max-errors-verbosity
  \ Nivel máximo para el detalle de errores.

variable repeat-previous-action?
  \ ¿Repetir la acción anterior cuando no se especifica otra en el
  \ comando?

variable space-after-command-prompt?
  \ ¿Separar el presto de comandos con un espacio posterior?

variable cr-after-command-prompt?
  \ ¿Hacer un salto de línea tras el presto de comando?

\ ==============================================================
\ Textos calculados relacionados

: player-gender-ending$  ( -- ca len )
  [false] [if]
    \ Método 1, «estilo BASIC»:
    woman-player? @ if  s" a"  else  s" o"  then
  [else]
    \ Método 2, sin estructuras condicionales, «estilo Forth»:
    c" oa" woman-player? @ abs + 1+ 1
  [then]  ;
  \ Devuelve en _ca len_ la terminación «a» u «o» según el sexo del jugador.

: player-gender-ending$+  ( ca1 len1 -- ca2 len2 )
  player-gender-ending$ s+  ;
  \ Añade a una cadena _ca1 len1_ la terminación «a» u «o» según el
  \ sexo del jugador, devolviendo el resultado en _ca2 len2_.

\ vim:filetype=gforth:fileencoding=utf-8

