#! /usr/bin/env gforth

\ ==============================================================
CR .( Asalto y castigo )  \ {{{

\ A text adventure in Spanish,
\ written in Forth with Gforth.

\ Project under development.

\ Copyright (C) 2011..2016 Marcos Cruz (programandala.net)

only forth definitions
s" 0.8.0+201606252049" 2constant version
version type cr

\ 'Asalto y castigo' is free software; you can redistribute
\ it and/or modify it under the terms of the GNU General
\ Public License as published by the Free Software
\ Foundation; either version 2 of the License, or (at your
\ option) any later version. See:

\ http://gnu.org/licenses/
\ http://gnu.org/licenses/gpl-2.0.html

\ «Asalto y castigo» es un programa libre; puedes
\ distribuirlo y/o modificarlo bajo los términos de la
\ Licencia Pública General de GNU, tal como está publicada
\ por la Free Software Foundation ('fundación para los
\ programas libres'), bien en su versión 2 o, a tu elección,
\ cualquier versión posterior.

\ «Asalto y castigo» está basado en el programa homónimo
\ escrito por Baltasar el Arquero en Sinclair BASIC para ZX
\ Spectrum.

\ Idea, argumento, textos y programa originales:
\ Copyright (C) 2009 Baltasar el Arquero
\ http://caad.es/baltasarq/
\ http://baltasarq.info

\ Información:
\

\ ##############################################################
\ Documentación

\ El historial de desarrollo está en:
\ http://programandala.net/es.programa.asalto_y_castigo.forth.historia.html

\ La lista de tareas pendientes está en el fichero <TO-DO.adoc>.

\ Notación de la pila

0 [if]

Abreviaturas usadas en este programa para describir los
elementos de la pila:

+n        = número de 32 bitios positivo
-n        = número de 32 bitios negativo
...       = elipsis: número variable de elementos, o rango
a         = dirección de memoria
ca        = dirección de memoria alineada a un carácter [octeto]
ca len    = dirección y longitud de una zona de memoria alineada
            a un carácter (octeto); casi siempre, una cadena de texto
b         = octeto, valor de ocho bitios
c         = carácter de un octeto
colon-sys = valores internos de control durante la compilación de una palabra
            (en el caso de Gforth se trata de tres elementos)
f         = indicador lógico: 0 es «falso»; -1 es «cierto»
            (-1 es un valor de 32 bitios con todos los bitios a uno)
false     = 0
i*x       = grupo de elementos sin especificar; puede estar vacío
j*x       = grupo de elementos sin especificar; puede estar vacío
n         = número de 32 bitios con signo
true      = -1 (valor de 32 bitios con todos los bitios a uno)
u         = número de 32 bitios sin signo
x         = elemento indeterminado
xt        = «execution token», identificador de ejecución de una palabra;
            notación de ANS Forth análoga a «cfa»
            («code field addrees») en Forth clásico

Como es costumbre, los diferentes elementos del mismo tipo se
distinguirán con un sufijo: un dígito para indicar que se trata
de elementos distintos; un apóstrofo para indicar que se trata
del mismo elemento, modificado.

[then]

\ }}} ==========================================================
CR .( Requisitos)  \ {{{

\ ----------------------------------------------
\ De Gforth

require random.fs

\ ----------------------------------------------
\ De la librería «Forth Foundation Library»
\ http://code.google.com/p/ffl/

require ffl/str.fs  \ Cadenas de texto dinámicas
require ffl/trm.fs  \ Manejo de terminal ANSI
require ffl/chr.fs  \ Herramientas para caracteres
require ffl/dtm.fs  \ Tipo de datos para fecha y hora
require ffl/dti.fs  \ Herramientas adicionales para fecha y hora

\ ----------------------------------------------
\ De Galope
\ http://programandala.net/en.program.galope.html

require galope/sb.fs \ Almacén circular de cadenas de texto
' bs+ alias s+
' bs& alias s&
' bs" alias s" immediate
2048 dictionary_sb

require galope/aliases-colon.fs  \ `aliases:`
require galope/at-x.fs  \ `at-x`  \ XXX TMP -- for debugging
require galope/between.fs  \ `between` (variante habitual de `within`)
require galope/bit-field-colon.fs  \ `bitfield:`
require galope/bracket-false.fs  \ `[false]`
require galope/bracket-or.fs  \ `[or]`
require galope/bracket-question-question.fs  \ `[??]`
require galope/bracket-true.fs  \ `[true]`
require galope/choose.fs  \ `choose`, selección aleatoria de un elemento de la pila
require galope/colon-alias.fs  \ `:alias`
require galope/colors.fs
require galope/column.fs  \ `column`
require galope/drops.fs  \ `drops`, eliminación de varios elementos de la pila
require galope/enum.fs  \ `enum`
require galope/home.fs  \ `home`
require galope/immediate-aliases-colon.fs  \ `immediate-aliases:`
require galope/ink.fs
require galope/minus-minus.fs  \ `--`
require galope/paper.fs
require galope/plus-plus.fs  \ `++`
require galope/print.fs  \ Impresión de textos ajustados
require galope/question-empty.fs  \ `?empty`
require galope/question-keep.fs  \ `?keep`
require galope/question-question.fs  \ `??`
require galope/random_strings.fs  \ Selección aleatoria de cadenas de texto
require galope/randomize.fs  \ `randomize`
require galope/row.fs  \ `row`
require galope/sconstant.fs \ Constantes de cadenas de texto
require galope/seconds.fs \ `seconds`, pausa en segundos acortable con una pulsación de tecla
require galope/sourcepath.fs
require galope/svariable.fs \ Variables de cadenas de texto
require galope/system_colors.fs
require galope/tilde-tilde.fs  \ `~~` mejorado
require galope/to-yyyymmddhhmmss.fs  \ `>yyyymmddhhss`
require galope/two-choose.fs  \ `2schoose`, selección aleatoria de un elemento doble de la pila
' 2choose alias schoose
require galope/x-c-store.fs  \ `xc!`
require galope/xcase_es.fs  \ Cambio de caja para caracteres propios del castellano en UTF-8
require galope/xy.fs  \ Posición actual del cursor

\ ----------------------------------------------
\ De Flibustre
\ http://programandala.net

require flibustre/different-question.fs  \ `different?`

\ ----------------------------------------------
\ Otras herramientas
\ http://programandala.net

require halto2.fs \ XXX TMP -- check points for debugging
false to halto?

\ }}} ==========================================================
\ Meta \ {{{

: wait  key drop  ;

\ Indicadores para depuración

false value [debug] immediate  \ ¿Depuración global?
false value [debug-init] immediate  \ ¿Depurar la inicialización?
false value [debug-parsing] immediate  \ ¿Depurar el analizador?
false value [debug-parsing-result] immediate  \ ¿Mostrar el resultado del analizador?
false value [debug-filing] immediate  \ ¿Depurar operaciones de ficheros?
false value [debug-do-exits] immediate  \ ¿Depurar la acción `do-exits`?
false value [debug-catch] immediate  \ ¿Depurar `catch` y `throw`?
false value [debug-save] immediate  \ ¿Depurar la grabación de partidas?
true value [debug-info] immediate  \ ¿Mostrar info sobre el presto de comandos?
false value [debug-pause] immediate  \ ¿Hacer pausa en puntos de depuración?
false value [debug-map] immediate  \ ¿Mostrar el número de escenario del juego original?

\ Indicadores para poder elegir alternativas que aún son experimentales

true constant [old-method]  immediate
[old-method] 0= constant [new-method]  immediate

\ Títulos de sección

: depth-warning  ( -- )
  cr ." Aviso: La pila no está vacía. Contenido: "  ;

: .s?  ( -- )
  \ Imprime el contenido de la pila si no está vacía.
  depth if  depth-warning .s cr  wait  then  ;

: section(  ( "text<bracket>" -- )
  \ Notación para los títulos de sección en el código fuente.
  \ Permite hacer tareas de depuración mientras se compila el programa;
  \ por ejemplo detectar el origen de descuadres en la pila.
  cr postpone .(  \ El nombre de sección terminará con: )
  .s?  ;

: subsection(  ( "text<bracket>" -- )
  \ Notación para los títulos de subsección en el código fuente.
  cr 2 spaces [char] - emit postpone .(  \ El nombre de subsección terminará con: )
  .s?  ;

\ }}} ==========================================================
section( Vocabularios de Forth)  \ {{{

\ Vocabulario principal del programa (no de la aventura)

vocabulary game-vocabulary

: restore-vocabularies  ( -- )
  \ Restaura los vocabularios a su orden habitual.
  only forth also game-vocabulary definitions  ;

restore-vocabularies

\ Demás vocabularios

vocabulary menu-vocabulary  \ palabras del menú \ XXX TODO -- not used yet
vocabulary player-vocabulary  \ palabras del jugador
vocabulary answer-vocabulary  \ respuestas a preguntas de «sí» o «no»
vocabulary config-vocabulary  \ palabras de configuración del juego
vocabulary restore-vocabulary  \ palabras de restauración de una partida

\ }}} ==========================================================
section( Palabras genéricas)  \ {{{

true constant [true]  immediate
false constant [false]  immediate

pad 0 2constant ""  \ Simula una cadena vacía.

: ?++  ( a -- )
  \ Incrementa el contenido de una dirección, si es posible.
  \ En la práctica el límite es inalcanzable
  \ (pues es un número de 32 bitios),
  \ pero así queda mejor hecho.
  \ XXX TODO -- confirmar este cálculo, pues depende de si el número se considera con signo o no
  dup @ 1+ ?dup if  swap !  else  drop  then  ;

' bootmessage alias .forth

\ }}} ==========================================================
section( Vectores)  \ {{{

\ XXX TODO -- efecto de pila

defer protagonist%  \ Ente protagonista
defer sword%  \ Ente espada
defer stone%  \ Ente piedra
defer torch%  \ Antorcha
defer leader%  \ Ente líder de los refugiados
defer location-01%  \ Primer ente escenario

defer list-exits  \ Crea e imprime la lista de salidas
defer exits%  \ Ente "salidas"

\ }}} ==========================================================
section( Códigos de error)  \ {{{

\ The execution token of the word that manages the error
\ is used as `throw` code:

\ 0 constant no-error# \ XXX TODO -- not used

0 value cannot-see-error#
0 value cannot-see-what-error#
0 value dangerous-error#
0 value impossible-error#
0 value is-normal-error#
0 value is-not-here-error#
0 value is-not-here-what-error#
0 value no-main-complement-error#
0 value no-verb-error#
0 value nonsense-error#
0 value not-allowed-main-complement-error#
0 value useless-tool-error#
0 value useless-what-tool-error#
0 value not-allowed-tool-complement-error#
0 value repeated-preposition-error#
0 value too-many-actions-error#
0 value too-many-complements-error#
0 value unexpected-main-complement-error#
0 value unexpected-secondary-complement-error#
0 value unnecessary-tool-error#
0 value unnecessary-tool-for-that-error#
0 value unresolved-preposition-error#
0 value what-is-already-closed-error#
0 value what-is-already-open-error#
0 value you-already-have-it-error#
0 value you-already-have-what-error#
0 value you-already-wear-what-error#
0 value you-do-not-have-it-error#
0 value you-do-not-have-what-error#
0 value you-do-not-wear-what-error#
0 value you-need-what-error#

\ }}} ==========================================================
section( Herramientas de azar)  \ {{{

\ Desordenar al azar varios elementos de la pila
\ XXX TODO -- mover a la librería Galope.

0 value unsort#
: unsort  ( x1 ... xu u -- x1' ... xu' )
  \ Desordena un número de elementos de la pila.
  \ x1 ... xu = Elementos a desordenar
  \ u = Número de elementos que hay que desordenar
  \ x1' ... xu' = Los mismos elementos, desordenados
  dup to unsort# 0 ?do
    unsort# random roll
  loop  ;

\ Combinar cadenas de forma aleatoria

: rnd2swap  ( ca1 len1 ca2 len2 -- ca1 len1 ca2 len2 | ca2 len2 ca1 len1 )
  \ Intercambia (con 50% de probabilidad) la posición de dos textos.
  2 random ?? 2swap  ;

: (both)  ( ca1 len1 ca2 len2 -- ca1 len1 ca3 len3 ca2 len2 | ca2 len2 ca3 len3 ca1 len1 )
  \ Devuelve las dos cadenas recibidas, en cualquier orden,
  \ y separadas en la pila por la cadena «y».
  rnd2swap s" y" 2swap  ;

: both  ( ca1 len1 ca2 len2 -- ca3 len3 )
  \ Devuelve dos cadenas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los dos resultados posibles serían: «fría y espesa» y «espesa y fría».
  (both) bs& bs&  ;

: both&  ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  \ Devuelve dos cadenas unidas en cualquier orden por «y»; y concatenada (con separación) a una tercera.
  both bs&  ;

: both?  ( ca1 len1 ca2 len2 -- ca3 len3 )
  \ Devuelve al azar una de dos cadenas,
  \ o bien ambas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los cuatro resultados posibles serían:
  \ «espesa», «fría», «fría y espesa» y «espesa y fría».
  (both) s&? bs&  ;

: both?&  ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  \ Concatena (con separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.
  both? bs&  ;

: both?+  ( ca0 len0 ca1 len1 ca2 len2 -- ca3 len3 )
  \ Concatena (sin separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.
  both? bs+  ;

\ }}} ==========================================================
section( Variables y constantes)  \ {{{

\ Algunas variables de configuración
\ (el resto se crea en sus propias secciones)

variable woman-player?  \ ¿El jugador es una mujer?
variable castilian-quotes?  \ ¿Usar comillas castellanas en las citas, en lugar de raya?
variable location-page?  \ ¿Borrar la pantalla antes de entrar en un escenario o de describirlo?
variable cr?  \ ¿Separar los párrafos con una línea en blanco?
variable ignore-unknown-words?  \ ¿Ignorar las palabras desconocidas?  \ XXX TODO -- not used yet
variable scene-page?  \ ¿Borrar la pantalla después de la pausa de los cambios de escena?
variable language-errors-verbosity  \ Nivel de detalle de los mensajes de error lingüístico
svariable 'language-error-general-message$  \ Mensaje de error lingüístico para el nivel 1
variable action-errors-verbosity  \ Nivel de detalle de los mensajes de error operativo
svariable 'action-error-general-message$  \ Mensaje de error operativo para el nivel 1
0 constant min-errors-verbosity  \ Nivel mínimo para el detalle de errores
2 constant max-errors-verbosity  \ Nivel máximo para el detalle de errores
variable repeat-previous-action?  \ ¿Repetir la acción anterior cuando no se especifica otra en el comando?

\ Variables de la trama

variable ambrosio-follows?  \ ¿Ambrosio sigue al protagonista?
variable battle#  \ Contador de la evolución de la batalla (si aún no ha empezado, es cero)
variable climbed-the-fallen-away?  \ ¿El protagonista ha intentado escalar el derrumbe?
variable hacked-the-log?  \ ¿El protagonista ha afilado el tronco?
\ variable hold#  \ XXX OLD -- contador de cosas llevadas por el protagonista (no se usa.)
variable stone-forbidden?  \ ¿El protagonista ha intentado pasar con la piedra?
variable sword-forbidden?  \ ¿El protagonista ha intentado pasar con la espada?
variable recent-talks-to-the-leader  \ Contador de intentos de hablar con el líder sin cambiar de escenario

: init-plot  ( -- )
  \ Inicializa las variables de la trama.
  ambrosio-follows? off
  battle# off
  climbed-the-fallen-away? off
  hacked-the-log? off
  stone-forbidden? off
  sword-forbidden? off
  recent-talks-to-the-leader off  ;

\ }}} ==========================================================
section( Pantalla)  \ {{{

\ }}}---------------------------------------------
subsection( Colores)  \ {{{

: colors  ( u1 u2 -- )
  \ Pone los colores de papel y tinta.
  \ u1 = Color de papel
  \ u2 = Color de tinta
  ink paper  ;

: @colors  ( a1 a2 -- )
  \ Pone los colores de papel y tinta con el contenido de dos variables.
  \ a1 = Dirección del color de papel
  \ a2 = Dirección del color de tinta
  @ swap @ swap colors  ;

\ }}}---------------------------------------------
subsection( Colores utilizados)  \ {{{

\ Variables para guardar cada color de papel y de tinta

variable about-ink
variable about-paper
variable background-paper  \ XXX TMP -- experimental
variable command-prompt-ink
variable command-prompt-paper
variable debug-ink
variable debug-paper
variable description-ink
variable description-paper
variable input-ink
variable input-paper
variable language-error-ink
variable language-error-paper
variable location-description-ink
variable location-description-paper
variable location-name-ink
variable location-name-paper
variable narration-ink
variable narration-paper
variable narration-prompt-ink
variable narration-prompt-paper
variable question-ink
variable question-paper
variable scene-prompt-ink
variable scene-prompt-paper
variable scroll-prompt-ink
variable scroll-prompt-paper
variable speech-ink
variable speech-paper
variable system-error-ink
variable system-error-paper
variable action-error-ink
variable action-error-paper

: init-colors  ( -- )
  \ Asigna los colores predeterminados.
  [defined] background-paper [if]
    black background-paper !  \ XXX TMP -- experimental
  [then]
  gray about-ink !
  black about-paper !
  cyan command-prompt-ink !
  black command-prompt-paper !
  white debug-ink !
  red debug-paper !
  gray description-ink !
  black description-paper !
  light_red language-error-ink !
  black language-error-paper !
  light_cyan input-ink !
  black input-paper !
  green location-description-ink !
  black location-description-paper !
  black location-name-ink !
  green location-name-paper !
  gray narration-ink !
  black narration-paper !
  green scroll-prompt-ink !
  black scroll-prompt-paper !
  white question-ink !
  black question-paper !
  green scene-prompt-ink !
  black scene-prompt-paper !
  light_gray speech-ink !
  black speech-paper !
  light_red system-error-ink !
  black system-error-paper !
  green narration-prompt-ink !
  black narration-prompt-paper !  ;

: about-color  ( -- )
  \ Pone el color de texto de los créditos.
  about-paper about-ink @colors  ;

: command-prompt-color  ( -- )
  \ Pone el color de texto del presto de entrada de comandos.
  command-prompt-paper command-prompt-ink @colors  ;

: debug-color  ( -- )
  \ Pone el color de texto usado en los mensajes de depuración.
  debug-paper debug-ink @colors  ;

: background-color  ( -- )
  \ Pone el color de fondo.
  [defined] background-paper
  [if]    background-paper @ paper
  [else]  system-background-color
  [then]  ;

: description-color  ( -- )
  \ Pone el color de texto de las descripciones de los entes que no son escenarios.
  description-paper description-ink @colors  ;

: system-error-color  ( -- )
  \ Pone el color de texto de los errores del sistema.
  system-error-paper language-error-ink @colors  ;

: action-error-color  ( -- )
  \ Pone el color de texto de los errores operativos.
  action-error-paper language-error-ink @colors  ;

: language-error-color  ( -- )
  \ Pone el color de texto de los errores lingüísticos.
  language-error-paper language-error-ink @colors  ;

: input-color  ( -- )
  \ Pone el color de texto para la entrada de comandos.
  input-paper input-ink @colors  ;

: location-description-color  ( -- )
  \ Pone el color de texto de las descripciones de los entes escenario.
  location-description-paper location-description-ink @colors  ;

: location-name-color  ( -- )
  \ Pone el color de texto del nombre de los escenarios.
  location-name-paper location-name-ink @colors  ;

: narration-color  ( -- )
  \ Pone el color de texto de la narración.
  narration-paper narration-ink @colors  ;

: scroll-prompt-color  ( -- )
  \ Pone el color de texto del presto de pantalla llena.
  scroll-prompt-paper scroll-prompt-ink @colors  ;

: question-color  ( -- )
  \ Pone el color de texto de las preguntas de tipo «sí o no».
  question-paper question-ink @colors  ;

: scene-prompt-color  ( -- )
  \ Pone el color de texto del presto de fin de escena.
  scene-prompt-paper scene-prompt-ink @colors  ;

: speech-color  ( -- )
  \ Pone el color de texto de los diálogos.
  speech-paper speech-ink @colors  ;

: narration-prompt-color  ( -- )
  \ Pone el color de texto del presto de pausa.
  narration-prompt-paper narration-prompt-ink @colors  ;

\ }}}---------------------------------------------
subsection( Demo de colores)  \ {{{

\ Dos palabras para probar cómo se ven los colores

: color-bar  ( u -- )
  \ Imprime una barra de 64 espacios con el color indicado.
  paper cr 32 spaces  black paper space  ;

: color-demo  ( -- )
  \ Prueba los colores.
  cr ." Colores descritos como se ven en Debian"
  black color-bar ." negro"
  gray color-bar ." gris"
  light_gray color-bar ." gris claro"
  blue color-bar ." azul"
  light_blue color-bar ." azul claro"
  cyan color-bar ." cian"
  light_cyan color-bar ." cian claro"
  green color-bar ." verde"
  light_green color-bar ." verde claro"
  magenta color-bar ." magenta"
  light_magenta color-bar ." magenta claro"
  red color-bar ." rojo"
  light_red color-bar ." rojo claro"
  brown color-bar ." marrón"
  yellow color-bar ." amarillo"
  white color-bar ." blanco"
  \ red color-bar ." rojo"
  \ brown color-bar ." marrón"
  \ red color-bar ." rojo"
  \ brown color-bar ." marrón"
  system_colors cr  ;

\ }}}---------------------------------------------
subsection( Otros atributos tipográficos)  \ {{{

: bold  ( -- )
  \ Activa la negrita.
  trm.bold 1 sgr  ;

: underline  ( f -- )
  \ Activa o desactiva el subrayado.
  if  trm.underscore-on  else  trm.underline-off  then  1 sgr  ;

' underline alias underscore
: inverse  ( f -- )
  \ Activa o desactiva la inversión de colores (papel y tinta).
  if  trm.reverse-on  else  trm.reverse-off  then  1 sgr  ;

true [if]  \ XXX TODO
: blink ( f -- )
  \ Activa o desactiva el parpadeo.
  \ XXX FIXME -- no funciona
  if  trm.blink-on  else  trm.blink-off  then  1 sgr  ;

[then]
: italic  ( f -- )
  \ Activa o desactiva la cursiva.
  \ Nota: tiene el mismo efecto que `inverse`.
  if  trm.italic-on  else  trm.italic-off  then  1 sgr  ;

\ }}}---------------------------------------------
subsection( Borrado de pantalla)  \ {{{

: reset-scrolling  ( -- )
  \ Desactiva la definición de zona de pantalla como ventana.
  [char] r trm+do-csi0  ;

[defined] background-paper [if]  \ XXX TMP -- experimental
: (color-background)  ( u -- )
  \ Colorea el fondo de la pantalla con el color indicado.
  \ No sirve de mucho colorear la pantalla, porque la edición de textos
  \ utiliza el color de fondo predeterminado del sistema, el negro,
  \ cuando se borra el texto que está siendo escrito.
  \ No se ha comprobado si en Windows ocurre lo mismo.
  \ No sirve usar `trm+set-default-attributes`.
  paper print_home
  [false] [if]  \ Por líneas, más lento
    rows 0 do  i ?? cr [ cols ] literal spaces  loop
  [else]  \ Pantalla entera, más rápido
    form * spaces
  [then]  ;

: color-background  ( -- )
  \ Colorea el fondo de la pantalla, si el color no es negativo.
  background-paper @ dup 0>=
  if  (color-background)  else  drop  then  ;

[else]
' trm+erase-display alias color-background
[then]
: new-page  ( -- )
  \ Borra la pantalla y sitúa el cursor en su origen.
  color-background print_home  ;

: clear-screen-for-location  ( -- )
  \ Restaura el color de tinta y borra la pantalla para cambiar de escenario.
  location-page? @ ?? new-page  ;

: init-screen  ( -- )
  \ Prepara la pantalla la primera vez.
  trm+reset init-colors  ;

\ }}} ==========================================================
section( Depuración)  \ {{{

: fatal-error  ( f ca len -- )
  \ Informa de un error y sale del sistema, si el indicador de error es distinto de cero.
  \ XXX TODO -- not used
  \ f = Indicador de error
  \ ca len = Mensaje de error
  rot if  ." Error fatal: " type cr bye  else  2drop  then  ;

: .stack  ( -- )
  \ Imprime el estado de la pila.
  [false] [if]  \ XXX OLD -- versión antigua
    ." Pila" depth
    if  ." :" .s ." ( " depth . ." )"
    else  ."  vacía."
    then
  [else]  \ nueva versión
    depth if  cr ." Pila: " .s cr  then
  [then]  ;

: .sb  ( -- )
  \ Imprime el estado del almacén circular de cadenas.
  ." Espacio para cadenas:" sb# ?  ;

: .system-status  ( -- )
  \ Muestra el estado del sistema.
  ( .sb ) .stack  ;

: .debug-message  ( ca len -- )
  \ Imprime el mensaje del punto de chequeo, si no está vacío.
  dup if  cr type cr  else  2drop  then  ;

: debug-pause  ( -- )
  \ Pausa tras mostrar la información de depuración.
  [debug-pause] [if]  depth ?? wait [then]  ;

: debug  ( ca len -- )
  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema.
  debug-color .debug-message .system-status debug-pause  ;

\ }}} ==========================================================
section( Manipulación de textos)  \ {{{

str-create tmp-str  \ Cadena dinámica de texto temporal para usos variados

: str-get-last-char  ( a -- c )
  \ Devuelve el último carácter de una cadena dinámica.
  \ XXX TODO -- soporte para UTF-8
  dup str-length@ 1- swap str-get-char  ;

: str-get-last-but-one-char  ( a -- c )
  \ Devuelve el penúltimo carácter de una cadena dinámica.
  \ XXX TODO -- soporte para UTF-8
  dup str-length@ 2 - swap str-get-char  ;

: (^uppercase)  ( ca len -- )
  \ Convierte en mayúsculas la primera letra de una cadena.
  [false] [if]  \ XXX OLD -- antiguo, ascii
    if  dup c@ toupper swap c!  else  drop  then
  [else]  \ UTF-8
    if  dup xc@ xtoupper swap xc!  else  drop  then
  [then]  ;

: ^uppercase  ( a1 u -- a2 u )
  \ Hace una copia de una cadena en el almacén circular
  \ y la devuelve con la primera letra en mayúscula.
  \ Nota: Se necesita para los casos en que no queremos
  \ modificar la cadena original.
  >sb 2dup (^uppercase)  ;

: ?^uppercase  ( a1 u f -- a1 u | a2 u )
  \ Hace una copia de una cadena en el almacén circular
  \ y la devuelve con la primera letra en mayúscula,
  \ dependiendo del valor de un indicador.
  \ XXX TODO -- not used
  ?? ^uppercase  ;

: -punctuation  ( ca len -- ca len )
  \ Sustituye por espacios todos los signos de puntuación ASCII de una cadena.
  \ XXX TODO -- recorrer la cadena por caracteres UTF-8
  \ XXX TODO -- sustituir también signos de puntuación UTF-8
  exit \ XXX TMP
  \ XXX FIXME -- esto elimina las marcas "#" de los comandos del sistema!
  2dup bounds  ?do
    i c@ chr-punct? if  bl i c!  then
  loop  ;

: tmp-str!  ( ca len -- )
  \ Guarda una cadena en la cadena dinámica `tmp-str`.
  tmp-str str-set  ;

: tmp-str@  ( -- ca len )
  \ Devuelve el contenido de cadena dinámica `tmp-str`.
  tmp-str str-get  ;

: sreplace  ( ca1 len1 ca2 len2 ca3 len3 -- ca4 len4 )
  \ Sustituye en una cadena todas las apariciones
  \ de una subcadena por otra subcadena.
  \ ca1 len1 = Cadena en la que se realizarán los reemplazos
  \ ca2 len2 = Subcadena buscada
  \ ca3 len3 = Subcadena sustituta
  \ ca4 len4 = Resultado
  \ XXX TODO -- use Galope's `replaced` instead
  2rot tmp-str!  tmp-str str-replace  tmp-str@  ;

: *>verb-ending  ( ca len f -- )
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ ca len = Texto
  \ f = ¿Hay que poner los verbos en plural?
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" n"  else  s" "  then  s" *" sreplace
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" n" rot and  s" *" sreplace
  [then]  ;

: *>plural-ending  ( ca len f -- )
  \ Cambia por «s» (terminación plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ ca len = Expresión
  \ f = ¿Hay que poner los verbos en plural?
  \ XXX TODO -- not used
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" s"  else  s" "  then  s" *" sreplace
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" s" rot and  s" *" sreplace
  [then]  ;

: char>string  ( c u -- ca len )
  \ Crea una cadena repitiendo un carácter.
  \ c = Carácter
  \ u = Longitud de la cadena
  \ a = Dirección de la cadena
  \ XXX TODO -- mover a la librería galope
  dup sb_allocate swap 2dup 2>r  rot fill  2r>  ;

: space+  ( ca1 len1 -- ca2 len2 )
  \ Añade un espacio al final de una cadena.
  s"  " s+  ;

: period+  ( ca1 len1 -- ca2 len2 )
  \ Añade un punto al final de una cadena.
  s" ." s+  ;

: comma+  ( ca1 len1 -- ca2 len2 )
  \ Añade una coma al final de una cadena.
  s" ," s+  ;

: colon+  ( ca1 len1 -- ca2 len2 )
  \ Añade dos puntos al final de una cadena.
  s" :" s+  ;

: hyphen+  ( ca1 len1 -- ca2 len2 )
  \ Añade un guion a una cadena.
  s" -" s+  ;

: and&  ( ca1 len1 -- ca2 len2 )
  \ Añade una conjunción «y» al final de una cadena.
  \ XXX TODO -- not used
  s" y" s&  ;

: or&  ( ca1 len1 -- ca2 len2 )
  \ Añade una conjunción «o» al final de una cadena.
  \ XXX TODO -- not used
  s" o" s&  ;

\ }}} ==========================================================
section( Textos aleatorios)  \ {{{

\ Casi todas las palabras de esta sección devuelven una cadena
\ calculada al azar. Las restantes palabras son auxiliares.

\ Por convención, en el programa las palabras que devuelven
\ una cadena sin recibir parámetros en la pila tienen el signo
\ «$» al final de su nombre.  También por tanto las constantes
\ de cadena creadas con `sconstant`.

: at-least$  ( ca len -- )
  s{ s" al" s" por lo" }s s" menos" s&  ;

: that-(at-least)$  ( ca len -- )
  s" que" at-least$ s?&  ;

: that(m)$  ( -- ca len )
  s{ s" que" s" cual" }s  ;

: the-that(m)$  ( -- ca len )
  s{ s" que" s" el cual" }s  ;

: this|the(f)$  ( -- ca len )
  s{ s" esta" s" la" }s  ;

: this|the(m)$  ( -- ca len )
  \ XXX TODO -- not used
  s{ s" este" s" el" }s  ;

: your|the(f)$  ( -- ca len )
  s{ s" tu" s" la" }s  ;

: old-man$  ( -- ca len )
  \ Devuelve una forma de llamar al líder de los refugiados.
  s{ s" hombre" s" viejo" s" anciano" }s  ;

: with-him$  ( -- ca len )
  \ Devuelve una variante de «consigo» o una cadena vacía.
  s{ "" s" consigo" s" encima" }s  ;

: with-you$  ( -- ca len )
  \ Devuelve «contigo» o una cadena vacía.
  s" contigo" s?  ;

: carries$  ( -- ca len )
  s{ s" tiene" s" lleva" }s  ;

: you-carry$  ( -- ca len )
  s{ s" tienes" s" llevas" }s  ;

: ^you-carry$  ( -- ca len )
  \ Devuelve una variante de «Llevas» (con la primera mayúscula).
  you-carry$ ^uppercase  ;

: now$  ( -- ca len )
  \ Devuelve una variante de «ahora» o una cadena vacía.
  s{ "" s" ahora" s" en este momento" s" en estos momentos" }s  ;

: now-$  ( -- ca len )
  \ Devuelve el resultado de `now$` o una cadena vacía.
  \ Sirve como versión de `now$` con mayor probabilidad devolver una cadena vacía.
  now$ s?  ;

: here$  ( -- ca len )
  \ Devuelve una variante de «aquí».
  s{ s" por aquí" s" por este lugar" s" en este lugar" s" aquí" }s  ;

: here|""$  ( -- ca len | a 0 )
  \ Devuelve una variante de «aquí» o una cadena vacía.
  here$ s?  ;

: now|here|""$  ( -- ca len | a 0 )
  s{ now$ here|""$ }s  ;

: only$  ( -- ca len )
  \ Devuelve una variante de «solamente».
  s{ s" tan solo" s" solo" s" solamente" s" únicamente" }s  ;

: ^only$  ( -- ca len )
  \ Devuelve una variante de «Solamente» (con la primera mayúscula).
  \ Nota: no se puede calcular este texto a partir de la versión en minúsculas, porque el cambio entre minúsculas y mayúsculas no funciona con caracteres codificados en UTF-8 de más de un octeto.
  s{ s" Tan solo" s" Solo" s" Solamente" s" Únicamente" }s  ;

: only-$  ( -- ca len )
  \ Devuelve una variante de «solamente»
  \ o una cadena vacía.
  only$ s?  ;

: ^only-$  ( -- ca len )
  \ Devuelve una variante de «Solamente» (con la primera mayúscula)
  \ o u una cadena vacía.
  ^only$ s?  ;

: again$  ( -- ca len )
  s{ s" de nuevo" s" otra vez" s" otra vez más" s" una vez más" }s  ;

: ^again$  ( -- ca len )
  again$ ^uppercase  ;

: again?$  ( -- ca len )
  again$ s" ?" s+  ;

: still$  ( -- ca len )
  s{ s" aún" s" todavía" }s  ;

: even$  ( -- ca len )
  s{ s" aun" s" incluso" }s  ;

: toward$  ( -- ca len )
  s{ s" a" s" hacia" }s  ;

: toward-the(f)$  ( -- ca len )
  toward$ s" la" s&  ;

: toward-the(m)$  ( -- ca len )
  s{ s" al" s" hacia el" }s  ;

: ^toward-the(m)$  ( -- ca len )
  toward-the(m)$ ^uppercase  ;

: from-the(m)$  ( -- ca len )
  s{ s" desde el" s" procedente" s? s" del" s& }s  ;

: to-go-back$  ( -- ca len )
  s{ s" volver" s" regresar" }s  ;

: remains$  ( -- ca len )
  s{ s" resta" s" queda" }s  ;

: possible1$  ( -- ca len )
  \ Devuelve «posible» o una cadena vacía.
  s" posible" s?  ;

: possible2$  ( -- ca len )
  \ Devuelve «posibles» o una cadena vacía.
  s" posibles" s?  ;

: all-your$  ( -- ca len )
  \ Devuelve una variante de «todos tus».
  s{ s" todos tus" s" tus" }s  ;

: ^all-your$  ( -- ca len )
  \ Devuelve una variante de «Todos tus» (con la primera mayúscula).
  all-your$ ^uppercase  ;

: soldiers$  ( -- ca len )
  \ Devuelve una variante de «soldados».
  s{ s" hombres" s" soldados" }s  ;

: your-soldiers$  ( -- ca len )
  \ Devuelve una variante de "tus hombres".
  s" tus" soldiers$ s&  ;

: ^your-soldiers$  ( -- ca len )
  \ Devuelve una variante de "Tus hombres".
  your-soldiers$ ^uppercase  ;

: officers$  ( -- ca len )
  \ Devuelve una variante de «oficiales».
  s{ s" oficiales" s" mandos" }s  ;

: the-enemies$  ( -- ca len )
  \ Devuelve una variante de «los enemigos».
  s{ s" los sajones"
  s{ s" las tropas" s" las huestes" }s
  s{ s" enemigas" s" sajonas" }s& }s  ;

: the-enemy$  ( -- ca len )
  \ Devuelve una variante de «el enemigo».
  s{ s" el enemigo"
  s{ s" la tropa" s" la hueste" }s
  s{ s" enemiga" s" sajona" }s& }s  ;

: (the-enemy|enemies)  ( -- ca len f )
  \ Devuelve una variante de «el/los enemigo/s», y un indicador del número.
  \ ca len = Cadena con el texto
  \ f = ¿El texto está en plural?
  2 random dup if  the-enemies$  else  the-enemy$  then  rot  ;

: the-enemy|enemies$  ( -- ca len )
  \ Devuelve una variante de «el/los enemigo/s».
  (the-enemy|enemies) drop  ;

: «de-el»>«del»  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  \ Remplaza las apariciones de «de el» en una cadena por «del».
  s" del " s" de el " sreplace  ;

: of-the-enemy|enemies$  ( -- ca len )
  \ Devuelve una variante de «del/de los enemigo/s».
  (the-enemy|enemies) >r
  s" de" 2swap s&
  r> 0= ?? «de-el»>«del»  ;

: ^the-enemy|enemies  ( -- ca len f )
  \ Devuelve una variante de «El/Los enemigo/s», y un indicador del número.
  \ ca len = Cadena con el texto
  \ f = ¿El texto está en plural?
  (the-enemy|enemies) >r  ^uppercase  r>  ;

: of-your-ex-cloak$  ( -- ca len )
  \ Devuelve un texto común a las descripciones de los restos de la capa.
  s{ "" s" que queda" s" que quedó" }s s" de" s&
  s{ s" lo" s" la" }s& s" que" s& s" antes" s?&
  s{ s" era" s" fue" s" fuera" }s&
  your|the(f)$ s& s{ s" negra" s" oscura" }s?&
  s" capa" s& s" de lana" s?& period+  ;

: but$  ( -- ca len )
  s{ s" pero" s" mas" }s  ;

: ^but$  ( -- ca len )
  but$ ^uppercase  ;

: though$  ( -- ca len )
  s{ s" si bien" but$ s" aunque" }s  ;

: place$  ( -- ca len )
  s{ s" sitio" s" lugar" }s  ;

: cave$  ( -- ca len )
  s{ s" cueva" s" caverna" s" gruta" }s  ;

: home$  ( -- ca len )
  s{ s" hogar" s" casa" }s  ;

: sire,$  ( -- ca len )
  s" Sire" s" Ulfius" s?& comma+  ;

: my-name-is$  ( -- ca len )
  s{ s" Me llamo" s" Mi nombre es" }s  ;

: very$  ( -- ca len )
  s{ s" muy" s" harto" }s  ; \ XXX TODO -- añadir asaz

: very-$  ( -- ca len )
  \ Devuelve el resultado de very$ o una cadena vacía.
  very$ s?  ;

: the-path$  ( -- ca len )
  s{ s" el camino" s" la senda" s" el sendero" }s  ;

: ^the-path$  ( -- ca len )
  the-path$ ^uppercase  ;

: a-path$  ( -- ca len )
  s{ s" un camino" s" una senda" }s  ;

: ^a-path$  ( -- ca len )
  a-path$ ^uppercase  ;

: pass$  ( -- ca len )
  s{ s" paso" s" camino" }s  ;

: the-pass$  ( -- ca len )
  s" el" pass$ s&  ;

: pass-way$  ( -- ca len )
  \ Devuelve una variante de «pasaje».
  s{ s" paso" s" pasaje" }s  ;

: a-pass-way$  ( -- ca len )
  \ Devuelve una variante de «un pasaje».
  s" un" pass-way$ s&  ;

: ^a-pass-way$  ( -- ca len )
  \ Devuelve una variante de «Un pasaje» (con la primera mayúscula).
  a-pass-way$ ^uppercase  ;

: the-pass-way$  ( -- ca len )
  \ Devuelve una variante de «el pasaje».
  s" el" pass-way$ s&  ;

: ^the-pass-way$  ( -- ca len )
  \ Devuelve una variante de «El pasaje» (con la primera mayúscula).
  the-pass-way$ ^uppercase  ;

: pass-ways$  ( -- ca len )
  \ Devuelve una variante de «pasajes».
  pass-way$ s" s" s+  ;

: ^pass-ways$  ( -- ca len )
  \ Devuelve una variante de «Pasajes» (con la primera mayúscula).
  pass-ways$ ^uppercase  ;

: surrounds$  ( -- ca len )
  \ XXX TODO -- comprobar traducción
  s{ s" rodea" s" circunvala" s" cerca" s" circuye" s" da un rodeo a" }s  ;

: leads$  ( -- ca len )
  s{ s" lleva" s" conduce" }s  ;

: (they)-lead$  ( -- ca len )
  leads$ s" n" s+  ;

: can-see$  ( -- ca len )
  \ Devuelve una forma de decir «ves».
  s{ s" ves" s" se ve" s" puedes ver" }s  ;

: ^can-see$  ( -- ca len )
  \ Devuelve una forma de decir «ves», con la primera letra mayúscula.
  can-see$ ^uppercase  ;

: cannot-see$  ( -- ca len )
  \ Devuelve una forma de decir «no ves».
  s" no" can-see$ s&  ;

: ^cannot-see$  ( -- ca len )
  \ Devuelve una forma de decir «No ves».
  cannot-see$ ^uppercase  ;

: can-glimpse$  ( -- ca len )
  s{ s" vislumbras" s" se vislumbra" s" puedes vislumbrar"
  s" entrevés" s" se entrevé" s" puedes entrever"
  s" columbras" s" se columbra" s" puedes columbrar" }s  ;

: ^can-glimpse$  ( -- ca len )
  can-glimpse$ ^uppercase  ;

: in-half-darkness-you-glimpse$  ( -- ca len )
  \ Devuelve un texto usado en varias descripciones de las cuevas.
  s" En la" s{ s" semioscuridad," s" penumbra," }s& s? dup
  if  can-glimpse$  else  ^can-glimpse$  then  s&  ;

: you-glimpse-the-cave$  ( -- a u)
  \ Devuelve un texto usado en varias descripciones de las cuevas.
  \ XXX TODO -- distinguir la antorcha encendida
  in-half-darkness-you-glimpse$ s" la continuación de la cueva." s&  ;

: rimarkable$  ( -- ca len )
  \ Devuelve una variante de «destacable».
  s{ s" destacable" s" que destacar"
  s" especial" s" de especial"
  s" de particular"
  s" peculiar" s" de peculiar"
  s" que llame la atención" }s  ;

: has-nothing$  ( -- ca len )
  s" no tiene nada"  ;

: is-normal$  ( -- ca len )
  \ Devuelve una variante de «no tiene nada especial».
  has-nothing$ rimarkable$ s&  ;

: ^is-normal$  ( -- ca len )
  \ Devuelve una variante de «No tiene nada especial» (con la primera letra en mayúscula).
  is-normal$ ^uppercase  ;

: over-there$  ( -- ca len )
  s{ s" allí" s" allá" }s  ;

: goes-down-into-the-deep$  ( -- ca len )
  \ Devuelve una variante de «desciende a las profundidades».
  s{ s" desciende" toward$ s& s" se adentra en"
  s" conduce" toward$ s& s" baja" toward$ s& }s
  s" las profundidades" s&  ;

: in-that-direction$  ( -- ca len )
  \ Devuelve una variante de «en esa dirección».
  s{ s" en esa dirección" s{ s" por" s" hacia" }s over-there$ s& }s  ;

: ^in-that-direction$  ( -- ca len )
  \ Devuelve una variante de «En esa dirección».
  in-that-direction$ ^uppercase  ;

: (uninteresting-direction-0)$  ( -- ca len )
  \ Devuelve primera variante de «En esa dirección no hay nada especial».
  s{ s" Esa dirección" is-normal$ s&
  ^in-that-direction$ s" no hay nada" s& rimarkable$ s&
  ^in-that-direction$ cannot-see$ s& s" nada" s& rimarkable$ s&
  }s period+  ;

: (uninteresting-direction-1)$  ( -- ca len )
  \ Devuelve segunda variante de «En esa dirección no hay nada especial».
  s{
  ^is-normal$ s" esa dirección" s&
  ^cannot-see$ s" nada" s& rimarkable$ s& in-that-direction$ s&
  s" No hay nada" rimarkable$ s& in-that-direction$ s&
  }s period+  ;

: uninteresting-direction$  ( -- ca len )
  \ Devuelve una variante de «En esa dirección no hay nada especial».
  ['] (uninteresting-direction-0)$
  ['] (uninteresting-direction-1)$
  2 choose execute  ;

s" de Westmorland" sconstant of-westmorland$
: the-village$  ( -- ca len )
  s{ s" la villa" of-westmorland$ s?&
  s" Westmorland" }s  ;

: ^the-village$  ( -- ca len )
  the-village$ ^uppercase  ;

: of-the-village$  ( -- ca len )
  s" de" the-village$ s&  ;

: (it)-blocks$  ( -- ca len )
  s{ s" impide" s" bloquea" }s  ;

: (they)-block$  ( -- ca len )
  s{ s" impiden" s" bloquean" }s  ;

: (rocks)-on-the-floor$  ( -- ca len )
  \ Devuelve un texto sobre las rocas que ya han sido desmoronadas.
  s" yacen desmoronadas" s" a lo largo del pasaje" s?&  ;

: (rocks)-clue$  ( -- ca len )
  \ Devuelve una descripción de las rocas que sirve de pista.
  s" Son" s{ s" muchas" s" muy" s? s" numerosas" s& }s& comma+
  s" aunque no parecen demasiado pesadas y" s&
  s{ s" pueden verse" s" se ven" s" hay" }s s" algunos huecos" s&
  s" entre ellas" rnd2swap s& s&  ;

: from-that-way$  ( -- u )
  s" de" s{ s" esa dirección" s" allí" s" ahí" s" allá" }s&  ;

: that-way$  ( -- ca len )
  \ Devuelve una variante de «en esa dirección».
  s{ s" en esa dirección" s" por" s{ s" ahí" s" allí" s" allá" }s& }s  ;

: ^that-way$  ( -- ca len )
  \ Devuelve una variante de «En esa dirección» (con la primera letra mayúscula).
  that-way$ ^uppercase  ;

: gets-wider$  ( -- ca len )
  \ Devuelve una variante de «se ensancha».
  s{
  s" se" s{ s" ensancha" s" va ensanchando"
  s" va haciendo más ancho" s" hace más ancho"
  s" vuelve más ancho" s" va volviendo más ancho" }s&
  2dup 2dup 2dup \ Aumentar las probabilidades de la primera variante
  s{ s" ensánchase" s" hácese más ancho" s" vuélvese más ancho" }s
  }s  ;

: (narrow)$  ( -- ca len )
  s{ s" estrech" s" angost" }s  ;

: narrow(f)$  ( -- ca len )
  \ Devuelve una variante de «estrecha».
  (narrow)$ s" a" s+  ;

: narrow(m)$  ( -- ca len )
  \ Devuelve una variante de «estrecho».
  (narrow)$ s" o" s+  ;

: narrow(mp)$  ( -- ca len )
  \ Devuelve una variante de «estrechos».
  narrow(m)$ s" s" s+  ;

: ^narrow(mp)$  ( -- ca len )
  \ Devuelve una variante de «Estrechos» (con la primera mayúscula).
  narrow(mp)$  ^uppercase  ;

: gets-narrower(f)$  ( -- ca len )
  \ Devuelve una variante de «se hace más estrecha» (femenino).
  s{
  s" se" s{ s" estrecha" s" va estrechando" }s&
  2dup \ Aumentar las probabilidades de la primera variante
  s" se" s{ s" va haciendo más" s" hace más"
  s" vuelve más" s" va volviendo más" }s& narrow(f)$ s&
  2dup \ Aumentar las probabilidades de la segunda variante
  s{ s" estréchase" s{ s" hácese" s" vuélvese" }s s" más" s& narrow(f)$ s& }s
  }s  ;

: goes-up$  ( -- ca len )
  \ Devuelve una variante de «sube».
  s{ s" sube" s" asciende" }s  ;

: (they)-go-up$  ( -- ca len )
  \ Devuelve una variante de «suben».
  goes-up$ s" n" s+  ;

: goes-down$  ( -- ca len )
  \ Devuelve una variante de «baja».
  s{ s" baja" s" desciende" }s  ;

: (they)-go-down$  ( -- ca len )
  \ Devuelve una variante de «bajan».
  goes-down$ s" n" s+  ;

: almost-invisible(plural)$  ( -- ca len )
  \ Devuelve una variante de «casi imperceptibles».
  s" casi" s{ s" imperceptibles" s" invisibles" s" desapercibidos" }s  ;
  \ XXX TODO -- confirmar significados

: ^a-narrow-pass-way$  ( -- ca len )
  s" Un" narrow(m)$ pass-way$ rnd2swap s& s&  ;

: beautiful(m)$  ( -- ca len )
  s{ s" bonito" s" bello" s" hermoso" }s  ;

: a-snake-blocks-the-way$  ( -- ca len )
  s" Una serpiente"
  s{ s" bloquea" s" está bloqueando" }s&
  the-pass$ s& toward-the(m)$ s" sur" s& s?&  ;

: the-water-current$  ( -- ca len )
  s" la" s{ s" caudalosa" s" furiosa" s" fuerte" s" brava" }s&
  s" corriente" s& s" de agua" s?&  ;

: ^the-water-current$  ( -- ca len )
  the-water-current$ ^uppercase  ;

: comes-from$  ( -- ca len )
  s{ s" viene" s" proviene" s" procede" }s  ;

: to-keep-going$  ( -- ca len )
  s{ s" avanzar" s" proseguir" s" continuar" }s  ;

: lets-you$  ( -- ca len )
  s" te" s? s" permite" s&  ;

: narrow-cave-pass$  ( -- ca len )
  \ Devuelve una variante de «estrecho tramo de cueva».
  s" tramo de cueva" narrow(m)$ rnd2swap s&  ;

: a-narrow-cave-pass$  ( -- ca len )
  \ Devuelve una variante de «un estrecho tramo de cueva».
  s" un" narrow-cave-pass$ s&  ;

: but|and$  ( -- ca len )
  s{ s" y" but$ }s  ;

' but|and$ alias and|but$
: ^but|and$  ( -- ca len )
  but|and$ ^uppercase  ;

' ^but|and$ alias ^and|but$
: rocks$  ( -- ca len )
  s{ s" piedras" s" rocas" }s  ;

: wanted-peace$  ( -- ca len )
  \ Texto «la paz», parte final de los mensajes «Queremos/Quieren la paz».
  s{  s" la" s" que haya"
      s" poder" s? s" vivir en" s&
      s{ s" tener" s" poder tener" s" poder disfrutar de" }s? s" una vida en" s&
      s" que" s{ s" reine" s" llegue" }s& s" la" s&
  }s s" paz." s&  ;

: they-want-peace$  ( -- ca len )
  \ Mensaje «quieren la paz».
  only$ s{ s" buscan" s" quieren" s" desean" s" anhelan" }s&
  wanted-peace$ s&  ;

: we-want-peace$  ( -- ca len )
  \ Mensaje «Queremos la paz».
  ^only$ s{ s" buscamos" s" queremos" s" deseamos" s" anhelamos" }s&
  wanted-peace$ s&  ;

: to-understand$  ( -- ca len )
  s{ s" comprender" s" entender" }s  ;

: way$  ( -- ca len )
  s{ s" manera" s" forma" }s  ;

: to-realize$  ( -- ca len )
  s{ s" ver" s" notar" s" advertir" s" apreciar" }s  ;

: more-carefully$  ( -- ca len )
  s{  s" mejor"
      s" con" s{ s" más" s" un" s? s" mayor" s& s" algo más de" }s&
        s{ s" detenimiento" s" cuidado" s" detalle" }s&
  }s  ;

: finally$  ( -- ca len )
  s{  s{ s" al" s" por" }s s" fin" s&
      s" finalmente"
  }s  ;

: ^finally$  ( -- ca len )
  finally$ ^uppercase  ;

: rocky(f)$  ( -- ca len )
  s{ s" rocosa" s" de roca" s" s" s?+ }s  ;

\ }}} ==========================================================
section( Cadena dinámica para impresión)  \ {{{

\ Usamos una cadena dinámica llamada `print-str` para guardar
\ los párrafos enteros que hay que mostrar en pantalla. En
\ esta sección creamos la cadena y palabras útiles para
\ manipularla.

str-create print-str  \ Cadena dinámica para almacenar el texto antes de imprimirlo justificado

: «»-clear  ( -- )
  \ Vacía la cadena dinámica `print-str`.
  print-str str-clear  ;

: «»!  ( ca len -- )
  \ Guarda una cadena en la cadena dinámica `print-str`.
  print-str str-set  ;

: «»@  ( -- ca len )
  \ Devuelve el contenido de la cadena dinámica `print-str`.
  print-str str-get  ;

: «+  ( ca len -- )
  \ Añade una cadena al principio de la cadena dinámica `print-str`.
  print-str str-prepend-string  ;

: »+  ( ca len -- )
  \ Añade una cadena al final de la cadena dinámica `print-str`.
  print-str str-append-string  ;

: «c+  ( c -- )
  \ Añade un carácter al principio de la cadena dinámica `print-str`.
  print-str str-prepend-char  ;

: »c+  ( c -- )
  \ Añade un carácter al final de la cadena dinámica `print-str`.
  print-str str-append-char  ;

: «»bl+?  ( u -- f )
  \ ¿Se debe añadir un espacio al concatenar una cadena a la cadena dinámica `print-str`?
  \ u = Longitud de la cadena que se pretende unir a la cadena dinámica `print-str`
  0<> print-str str-length@ 0<> and  ;

: »&  ( ca len -- )
  \ Añade una cadena al final de la cadena dinámica `print-str`, con un espacio de separación.
  dup «»bl+? if  bl »c+  then  »+  ;

: «&  ( ca len -- )
  \ Añade una cadena al principio de la cadena dinámica `print-str`, con un espacio de separación.
  dup «»bl+? if  bl «c+  then  «+  ;

\ }}} ==========================================================
section( Herramientas para sonido)  \ {{{

\ Las herramientas para proveer de sonido al juego están apenas
\ esbozadas aquí.

\ La idea consiste en utilizar un reproductor externo que acepte
\ comandos y no muestre interfaz, como mocp para GNU/Linux, que es el
\ que usamos en las pruebas. Los comandos para la consola del sistema
\ operativo se pasan con la palabra SYSTEM de Gforth.

: clear-sound-track  ( -- )
  \ Limpia la lista de sonidos.
  s" mocp --clear" system  ;

: add-sound-track  ( ca len -- )
  \ Añade un fichero de sonido a la lista de sonidos.
  s" mocp --add" 2swap s& system  ;

: play-sound-track  ( -- )
  \ Inicia la reproducción de la lista de sonidos.
  s" mocp --play" system  ;

: stop-sound-track  ( -- )
  \ Detiene la reproducción de la lista de sonidos.
  s" mocp --stop" system  ;

: next-sound-track  ( -- )
  \ Salta al siguiente elemento de la lista de sonidos.
  s" mocp --forward" system  ;

\ }}} ==========================================================
section( Impresión de textos)  \ {{{

variable #lines  \ Número de línea del texto que se imprimirá
variable scroll  \ Indicador de que la impresión no debe parar

\ ----------------------------------------------
subsection( Presto de pausa en la impresión de párrafos)  \ {{{

svariable scroll-prompt  \ Guardará el presto de pausa
: scroll-prompt$  ( -- ca len )
  \ Devuelve el presto de pausa.
  scroll-prompt count  ;

1 value /scroll-prompt  \ Número de líneas de intervalo para mostrar un presto

: scroll-prompt-key  ( -- )
  \ Espera la pulsación de una tecla y actualiza con ella el estado del desplazamiento.
  key  bl =  scroll !  ;

: .scroll-prompt  ( -- )
  \ Imprime el presto de pausa, espera una tecla y borra el presto.
  trm+save-cursor  scroll-prompt-color
  scroll-prompt$ type  scroll-prompt-key
  trm+erase-line  trm+restore-cursor  ;

: (scroll-prompt?)  ( u -- f )
  \ ¿Se necesita imprimir un presto para la línea actual?
  \ u = Línea actual del párrafo que se está imprimiendo
  \ Se tienen que cumplir dos condiciones:
  dup 1+ #lines @ <>  \ ¿Es distinta de la última?
  swap /scroll-prompt mod 0=  and  ;  \ ¿Y el intervalo es correcto?
  \ XXX TODO factor to save the comments

: scroll-prompt?  ( u -- f )
  \ ¿Se necesita imprimir un presto para la línea actual?
  \ u = Línea actual del párrafo que se está imprimiendo
  \ Si el valor de `scroll` es «verdadero», se devuelve «falso»;
  \ si no, se comprueban las otras condiciones.
  \ ." L#" dup . ." /" #lines @ . \ XXX INFORMER
  scroll @ if  drop false  else  (scroll-prompt?)  then  ;

: .scroll-prompt?  ( u -- )
  \ Imprime un presto y espera la pulsación de una tecla,
  \ si corresponde a la línea en curso.
  \ u = Línea actual del párrafo que se está imprimiendo
  scroll-prompt? ?? .scroll-prompt  ;

\ }}}---------------------------------------------
subsection( Impresión de párrafos ajustados)  \ {{{

\ Indentación de la primera línea de cada párrafo (en caracteres):
2 constant default-indentation  \ Predeterminada
8 constant max-indentation  \ Máxima
variable /indentation  \ En curso

variable indent-first-line-too?  \ ¿Se indentará también la línea superior de la pantalla, si un párrafo empieza en ella?
: not-first-line?  ( -- f )
  \ ¿El cursor no está en la primera línea?
  row 0>  ;

: indentation?  ( -- f )
  \ ¿Indentar la línea actual?
  not-first-line? indent-first-line-too? @ or  ;

: (indent)  ( -- )
  \ Indenta.
  /indentation @ print_indentation  ;

: indent  ( -- )
  \ Indenta si es necesario.
  indentation? ?? (indent)  ;

: background-line  ( -- )
  \ Colorea una línea con el color de fondo.
  background-color cols spaces  ;

: ((print_cr))  ( -- )
  \ Salto de línea alternativo para los párrafos justificados.
  \ Colorea la nueva línea con el color de fondo, lo que parchea
  \ el problema de que las nuevas líneas volvían a aparecer
  \ con el color predeterminado de la terminal.
  \ XXX TODO -- inacabado, en pruebas
  \ background-color cols #printed @ - spaces  \ final de línea
  \ blue paper cols #printed @ - spaces key drop  \ XXX INFORMER
  cr trm+save-current-state background-line
  trm+restore-current-state  ;

' ((print_cr)) is (print_cr)  \ Redefinir `(print_cr)` (de galope/print.fs)
: cr+  ( -- )
  \ Hace un salto de línea y una indentación, para el sistema
  \ de impresión de textos justificados.
  [false] [if]  \ Primera versión
    print_cr indent
  [else]  \ XXX TODO -- pruebas para solucionar problema de la línea en blanco
    \ row last-row <> column or ?? print_cr indent
    column 0<> ?? print_cr indent
  [then]  ;

: paragraph  ( ca len -- )
  \ Imprime un texto justificado como inicio de un párrafo.
  \ ca len = Texto
  cr+ print  ;

: (language-error)  ( ca len -- )
  \ Imprime una cadena como un informe de error lingüístico.
  \ XXX TODO -- renombrar
  language-error-color paragraph system_colors  ;

: action-error  ( ca len -- )
  \ Imprime una cadena como un informe de error de un comando.
  action-error-color paragraph system_colors  ;

: system-error  ( ca len -- )
  \ Imprime una cadena como un informe de error del sistema.
  system-error-color paragraph system_colors  ;

: narrate  ( ca len -- )
  \ Imprime una cadena como una narración.
  narration-color paragraph system_colors  ;

\ }}}---------------------------------------------
subsection( Pausas y prestos en la narración)  \ {{{

variable indent-pause-prompts?  \ ¿Hay que indentar también los prestos?
: indent-prompt  ( -- )
  \ Indenta antes de un presto, si es necesario.
  indent-pause-prompts? @ ?? indent  ;

: .prompt  ( ca len -- )
  \ Imprime un presto.
  print_cr indent-prompt print  ;

: wait  ( u -- )
  \ Hace una pausa.
  \ u = Segundos (o un número negativo para pausa sin fin hasta la pulsación de una tecla)
  dup 0< if  key 2drop  else  seconds  then  ;

variable narration-break-seconds  \ Segundos de espera en las pausas de la narración
svariable narration-prompt  \ Guardará el presto usado en las pausas de la narración
: narration-prompt$  ( -- ca len )
  \ Devuelve el presto usado en las pausas de la narración.
  narration-prompt count  ;

: .narration-prompt  ( -- )
  \ Imprime el presto de fin de escena.
  narration-prompt-color narration-prompt$ .prompt  ;

: (break)  ( n -- )
  \ n = Segundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
  \ Hace una pausa,
  \ borra la línea en que se ha mostrado el presto de pausa
  \ y restaura la situación de impresión para no afectar
  \ al siguiente párrafo.
  [false] [if]  \ XXX OLD -- antiguo. versión primera, que no coloreaba la línea
    wait  trm+erase-line print_start_of_line
  [else]
    wait  print_start_of_line
    trm+save-current-state background-line trm+restore-current-state
  [then]  ;

: (narration-break)  ( n -- )
  \ Alto en la narración: Muestra un presto y hace una pausa.
  \ n = Segundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
  .narration-prompt (break)  ;

: narration-break  ( -- )
  \ Alto en la narración, si es preciso.
  narration-break-seconds @ ?dup ?? (narration-break)  ;

variable scene-break-seconds  \ Segundos de espera en las pausas de final de escena
svariable scene-prompt  \ Guardará el presto de cambio de escena
: scene-prompt$  ( -- ca len )
  \ Devuelve el presto de cambio de escena.
  scene-prompt count  ;

: .scene-prompt  ( -- )
  \ Imprime el presto de fin de escena.
  scene-prompt-color scene-prompt$ .prompt  ;

: (scene-break)  ( n -- )
  \ Final de escena: Muestra un presto y hace una pausa .
  \ n = Segundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
  .scene-prompt (break)  scene-page? @ ?? new-page  ;

: scene-break  ( -- )
  \ Final de escena, si es preciso.
  scene-break-seconds @ ?dup ?? (scene-break)  ;

: about-pause  ( -- )
  \ Pausa tras los créditos.
  20 (break)  ;

\ }}}---------------------------------------------
subsection( Impresión de citas de diálogos)  \ {{{

s" —" sconstant dash$  \ Raya (código Unicode 2014 en hexadecimal, 8212 en decimal)
s" «" sconstant lquote$ \ Comilla castellana de apertura
s" »" sconstant rquote$  \ Comilla castellana de cierre

: str-with-rquote-only?  ( a -- f )
  \ ¿Hay en una cadena dinámica una comilla castellana de cierre pero no una de apertura?
  >r rquote$ 0 r@ str-find -1 >  \ ¿Hay una comilla de cierre en la cita?
  lquote$ 0 r> str-find -1 = and  ;  \ ¿Y además falta la comilla de apertura?
  \ XXX TODO -- factor out to save comments

: str-with-period?  ( a -- f )
  \ ¿Termina una cadena dinámica con un punto?
  \ XXX FIXME -- fallo: no se pone punto tras puntos suspensivos
  dup str-get-last-char [char] . =  \ ¿El último carácter es un punto?
  swap str-get-last-but-one-char [char] . <> and  ;  \ ¿Y además el penúltimo no lo es? (para descartar que se trate de puntos suspensivos)
  \ XXX TODO -- factor out to save comments

: str-prepend-quote  ( a -- )
  \ Añade a una cadena dinámica una comilla castellana de apertura.
  lquote$ rot str-prepend-string  ;

: str-append-quote  ( a -- )
  \ Añade a una cadena dinámica una comilla castellana de cierre.
  rquote$ rot str-append-string  ;

: str-add-quotes  ( a -- )
  \ Encierra una cadena dinámica entre comillas castellanas.
  dup str-append-quote str-prepend-quote  ;

false [if]  \ XXX OLD -- obsoleto
: str-add-quotes-period  ( a -- )
  \ Encierra una cadena dinámica (que termina en punto) entre comillas castellanas
  dup str-pop-char drop  \ Eliminar el último carácter, el punto
  dup str-add-quotes  \ Añadir las comillas
  s" ." rot str-append-string  ; \ Añadir de nuevo el punto

[then]
: (quotes+)  ( -- )
  \ Añade comillas castellanas a una cita de un diálogo en la cadena dinámica `tmp-str`.
  tmp-str dup str-with-period?  \ ¿Termina con un punto?
  if    dup str-pop-char drop  \ Eliminarlo
  then  dup str-add-quotes s" ." rot str-append-string  ;

: quotes+  ( ca1 len1 -- ca2 len2 )
  \ Añade comillas castellanas a una cita de un diálogo.
  tmp-str!  tmp-str str-with-rquote-only?
  if  \ Es una cita con aclaración final
    tmp-str str-prepend-quote  \ Añadir la comilla de apertura
  else  \ Es una cita sin aclaración, o con aclaración en medio
    (quotes+)
  then  tmp-str@  ;

: hyphen+  ( ca1 len1 -- ca2 len2 )
  \ Añade la raya a una cita de un diálogo.
  dash$ 2swap s+  ;

: quoted  ( ca1 len1 -- ca2 len2 )
  \ Pone comillas o raya a una cita de un diálogo.
  castilian-quotes? @ if  quotes+  else  hyphen+  then  ;

: speak  ( ca len -- )
  \ Imprime una cita de un diálogo.
  quoted speech-color paragraph system_colors  ;

\ }}}
\ }}} ==========================================================
section( Definición de la ficha de un ente)  \ {{{

\ Denominamos «ente» a cualquier componente del mundo virtual
\ del juego que es manipulable por el programa.  «Entes» por
\ tanto son los objetos, manipulables o no por el jugador; los
\ personajes, interactivos o no; los lugares; y el propio
\ personaje protagonista.
\
\ Cada ente tiene una ficha en la base de datos del juego.  La
\ base de datos es una zona de memoria dividida en partes
\ iguales, una para cada ficha. El identificador de cada ficha es
\ una palabra que al ejecutarse deja en la pila la dirección de
\ memoria donde se encuentra la ficha.
\
\ Los campos de la base de datos, como es habitual en Forth en
\ este tipo de estructuras, son palabras que suman el
\ desplazamiento adecuado a la dirección base de la ficha, que
\ reciben en la pila, apuntando así a la dirección de memoria que
\ contiene el campo correspondiente.
\
\ A pesar de que Gforth dispone de palabras especializadas para
\ crear estructuras de datos de todo tipo, hemos optado por el
\ método más sencillo: usar `+field` y `constant`.
\
\ El funcionamiento de `+field` es muy sencillo: Toma de la pila dos
\ valores: el inferior es el desplazamiento en octetos desde el inicio
\ del «registro», que en este programa denominamos «ficha»; el superior
\ es el número de octetos necesarios para almacenar el campo a crear. Con
\ ellos crea una palabra nueva [cuyo nombre es tomado del flujo de
\ entrada, es decir, es la siguiente palabra en el código fuente] que
\ será el identificador del campo de datos; esta palabra, al ser creada,
\ guardará en su propio campo de datos el desplazamiento del campo de
\ datos desde el inicio de la ficha de datos, y cuando sea ejecutada lo
\ sumará al número de la parte superior de la pila, que deberá ser la
\ dirección en memoria de la ficha.
\
\ Salvo los campos buleanos, que ocupan un solo bitio gracias a las
\ palabras creadas para ello, todos los demás campos ocupan una celda.
\ La «celda» es un concepto de ANS Forth: es la unidad en que se mide el
\ tamaño de cada elemento de la pila, y capaz por tanto de contener una
\ dirección de memoria.  En los sistemas Forth de 8 o 16 bitios una celda
\ equivale a un valor de 16 bitios; en los sistemas Forth de 32 bitios,
\ como Gforth, una celda equivale a un valor de 32 bitios.
\
\ El contenido de un campo puede representar cualquier cosa: un número
\ con o sin signo, o una dirección de memoria [de una cadena de texto, de
\ una palabra de Forth, de la ficha de otro ente, de otra estructura de
\ datos...].
\
\ Para facilitar la legibilidad, los nombres de los campos empiezan con
\ el signo de tilde, «~»; los que contienen datos buleanos terminan con
\ una interrogación, «?»;  los que contienen direcciones de ejecución
\ terminan con «-xt»; los que contienen códigos de error terminan con
\ «-error#».

0 \ Valor inicial de desplazamiento para el primer campo

\ Identificación
cell +field ~name-str  \ Dirección de una cadena dinámica que contendrá el nombre del ente
cell +field ~init-xt  \ Dirección de ejecución de la palabra que inicializa las propiedades de un ente
cell +field ~description-xt  \ Dirección de ejecución de la palabra que describe el ente
cell +field ~direction  \ Desplazamiento del campo de dirección al que corresponde el ente (solo se usa en los entes que son direcciones)

\ Contadores
cell +field ~familiar  \ Contador de familiaridad (cuánto le es conocido el ente al protagonista)
cell +field ~times-open  \ Contador de veces que ha sido abierto.
cell +field ~conversations  \ Contador para personajes: número de conversaciones tenidas con el protagonista
cell +field ~visits  \ Contador para escenarios: visitas del protagonista (se incrementa al abandonar el escenario)

\ Errores específicos (cero si no hay error); se usan para casos especiales
\ (los errores apuntados por estos campos no reciben parámetros salvo en `what`)
cell +field ~break-error#  \ Error al intentar romper el ente
cell +field ~take-error#  \ Error al intentar tomar el ente

\ Entes relacionados
cell +field ~location  \ Identificador del ente en que está localizado (sea escenario, contenedor, personaje o «limbo»)
cell +field ~previous-location  \ Ídem para el ente que fue la localización antes del actual
cell +field ~owner  \ Identificador del ente al que pertenece «legalmente» o «de hecho», independientemente de su localización.

\ Direcciones de ejecución de las tramas de escenario
cell +field ~can-i-enter-location?-xt  \ Trama previa a la entrada al escenario
cell +field ~before-describing-location-xt  \ Trama de entrada antes de describir el escenario
cell +field ~after-describing-location-xt  \ Trama de entrada tras describir el escenario
cell +field ~after-listing-entities-xt  \ Trama de entrada tras listar los entes presentes
cell +field ~before-leaving-location-xt  \ Trama antes de abandonar el escenario

\ Salidas
cell +field ~north-exit  \ Ente de destino hacia el norte
cell +field ~south-exit  \ Ente de destino hacia el sur
cell +field ~east-exit  \ Ente de destino hacia el este
cell +field ~west-exit  \ Ente de destino hacia el oeste
cell +field ~up-exit  \ Ente de destino hacia arriba
cell +field ~down-exit  \ Ente de destino hacia abajo
cell +field ~out-exit  \ Ente de destino hacia fuera
cell +field ~in-exit  \ Ente de destino hacia dentro

\ Indicadores
bitfields
  bitfield: ~has-definite-article?  \ ¿El artículo de su nombre debe ser siempre el artículo definido?
  bitfield: ~has-feminine-name?  \ ¿El género gramatical de su nombre es femenino?
  bitfield: ~has-no-article?  \ ¿Su nombre no debe llevar artículo?
  bitfield: ~has-personal-name?  \ ¿Su nombre es un nombre propio?
  bitfield: ~has-plural-name?  \ ¿Su nombre es plural?
  bitfield: ~is-animal?  \ ¿Es animal?
  bitfield: ~is-character?  \ ¿Es un personaje?
  bitfield: ~is-cloth?  \ ¿Es una prenda que puede ser puesta y quitada?
  bitfield: ~is-decoration?  \ ¿Forma parte de la decoración de su localización?
  bitfield: ~is-global-indoor?  \ ¿Es global (común) en los escenarios interiores?
  bitfield: ~is-global-outdoor?  \ ¿Es global (común) en los escenarios al aire libre?
  bitfield: ~is-not-listed?  \ ¿No debe ser listado (entre los entes presentes o en inventario)?
  bitfield: ~is-human?  \ ¿Es humano?
  bitfield: ~is-light?  \ ¿Es una fuente de luz que puede ser encendida?
  bitfield: ~is-lit?  \ ¿El ente, que es una fuente de luz que puede ser encendida, está encendido?
  bitfield: ~is-location?  \ ¿Es un escenario?
  bitfield: ~is-indoor-location?  \ ¿Es un escenario interior (no exterior, al aire libre)?
  bitfield: ~is-open?  \ ¿Está abierto?
  bitfield: ~is-vegetal?  \ ¿Es vegetal?
  bitfield: ~is-worn?  \ ¿Siendo una prenda, está puesta?
cell +field ~flags-0  \ Campo para albergar los indicadores anteriores

[false] [if]  \ XXX OLD -- campos que aún no se usan.:

cell +field ~times-closed  \ Contador de veces que ha sido cerrado.
cell +field ~desambiguation-xt  \ Dirección de ejecución de la palabra que desambigua e identifica el ente
cell +field ~stamina  \ Energía de los entes vivos

bitfield: ~is-lock?  \ ¿Está cerrado con llave?
bitfield: ~is-openable?  \ ¿Es abrible?
bitfield: ~is-lockable?  \ ¿Es cerrable con llave?
bitfield: ~is-container?  \ ¿Es un contenedor?

[then]

constant /entity  \ Tamaño de cada ficha

\ }}} ==========================================================
section( Interfaz de campos)  \ {{{

\ Las palabras de esta sección facilitan la tarea de
\ interactuar con los campos de las fichas, evitando repetir
\ cálculos, escondiendo parte de los entresijos de las fichas
\ y haciendo el código más conciso, más fácil de modificar y
\ más legible.
\
\ Algunas de las palabras que definimos a continuación actúan
\ de forma análoga a los campos de las fichas de entes:
\ reciben en la pila el identificador de ente y devuelven en
\ ella un resultado. La diferencia es que es un resultado
\ calculado.
\
\ Otras actúan como procedimientos para realizar operaciones
\ frecuentes con los entes.

\ ----------------------------------------------
\ Herramientas para los campos de dirección

' ~north-exit alias ~first-exit  \ Primera salida definida en la ficha
' ~in-exit alias ~last-exit  \ Última salida definida en la ficha

\ Guardar el desplazamiento de cada campo de dirección respecto al primero de ellos:
0 ~first-exit constant first-exit>
0 ~last-exit constant last-exit>
0 ~north-exit constant north-exit>
0 ~south-exit constant south-exit>
0 ~east-exit constant east-exit>
0 ~west-exit constant west-exit>
0 ~up-exit constant up-exit>
0 ~down-exit constant down-exit>
0 ~out-exit constant out-exit>
0 ~in-exit constant in-exit>

\ Espacio en octetos ocupado por los campos de salidas:
last-exit> cell+ first-exit> - constant /exits
\ Número de salidas:
/exits cell / constant #exits

\ Marcador para direcciones sin salida en un ente dirección:
0 constant no-exit

: exit?  ( a -- f )
  \ ¿Está abierta una dirección de salida de un ente escenario?
  \ a = Contenido de un campo de salida de un ente (que será el ente de destino, o cero)
  no-exit <>  ;

\ ----------------------------------------------
\ Interfaz básica para leer y modificar los campos

\ Las palabras que siguen permiten hacer las operaciones
\ básicas de obtención y modificación del contenido de los
\ campos.

\ Obtener el contenido de un campo a partir de un identificador de ente

: break-error#  ( a -- u )  ~break-error# @  ;
: conversations  ( a -- u )  ~conversations @  ;
: description-xt  ( a -- xt )  ~description-xt @  ;
: direction  ( a -- u )  ~direction @  ;
: familiar  ( a -- u )  ~familiar @  ;
: flags-0  ( a -- x )  ~flags-0 @  ;
: has-definite-article?  ( a -- f )  ~has-definite-article? bit@  ;
: has-feminine-name?  ( a -- f )  ~has-feminine-name? bit@  ;
: has-masculine-name?  ( a -- f )  has-feminine-name? 0=  ;
: has-no-article?  ( a -- f )  ~has-no-article? bit@  ;
: has-personal-name?  ( a -- f )  ~has-personal-name? bit@  ;
: has-plural-name?  ( a -- f )  ~has-plural-name? bit@  ;
: has-singular-name?  ( a -- f )  has-plural-name? 0=  ;
: init-xt  ( a -- xt )  ~init-xt @  ;
: is-animal?  ( a -- f )  ~is-animal? bit@  ;
: is-character?  ( a -- f )  ~is-character? bit@  ;
: is-cloth?  ( a -- f )  ~is-cloth? bit@  ;
: is-decoration?  ( a -- f )  ~is-decoration? bit@  ;
: is-global-indoor?  ( a -- f )  ~is-global-indoor? bit@  ;
: is-global-outdoor?  ( a -- f )  ~is-global-outdoor? bit@  ;
: is-human?  ( a -- f )  ~is-human? bit@  ;
: is-light?  ( a -- f )  ~is-light? bit@  ;
: is-not-listed?  ( a -- f )  ~is-not-listed? bit@  ;
: is-listed?  ( a -- f )  is-not-listed? 0=  ;
: is-lit?  ( a -- f )  ~is-lit? bit@  ;
: is-not-lit?  ( a -- f )  is-lit? 0=  ;
: is-location?  ( a -- f )  ~is-location? bit@  ;
: is-indoor-location?  ( a -- f )  is-location? ~is-indoor-location? bit@ and  ;
: is-outdoor-location?  ( a -- f )  is-indoor-location? 0=  ;
: is-open?  ( a -- f )  ~is-open? bit@  ;
: is-closed?  ( a -- f )  is-open? 0=  ;
: name-str  ( a1 -- a2 )  ~name-str @  ;
: times-open  ( a -- u )  ~times-open @  ;
: owner  ( a1 -- a2 )  ~owner @  ;
: is-vegetal?  ( a -- f )  ~is-vegetal? bit@  ;
: is-worn?  ( a -- f )  ~is-worn? bit@  ;
: location  ( a1 -- a2 )  ~location @  ;
: can-i-enter-location?-xt  ( a -- xt )  ~can-i-enter-location?-xt @  ;
: before-describing-location-xt  ( a -- xt )  ~before-describing-location-xt @  ;
: after-describing-location-xt  ( a -- xt )  ~after-describing-location-xt @  ;
: after-listing-entities-xt  ( a -- xt )  ~after-listing-entities-xt @  ;
: before-leaving-location-xt  ( a -- xt )  ~before-leaving-location-xt @  ;
: previous-location  ( a1 -- a2 )  ~previous-location @  ;
: take-error#  ( a -- u )  ~take-error# @  ;
: visits  ( a -- u )  ~visits @  ;

: north-exit  ( a1 -- a2 )  ~north-exit @  ;
: south-exit  ( a1 -- a2 )  ~south-exit @  ;
: east-exit  ( a1 -- a2 )  ~east-exit @  ;
: west-exit  ( a1 -- a2 )  ~west-exit @  ;
: up-exit  ( a1 -- a2 )  ~up-exit @  ;
: down-exit  ( a1 -- a2 )  ~down-exit @  ;
: out-exit  ( a1 -- a2 )  ~out-exit @  ;
: in-exit  ( a1 -- a2 )  ~in-exit @  ;

\ Modificar el contenido de un campo a partir de un identificador de ente

\ XXX TODO -- rename is-... to be-...
: conversations++  ( a -- )  ~conversations ?++  ;
: familiar++  ( a -- )  ~familiar ?++  ;
: has-definite-article  ( a -- )  ~has-definite-article? bit-on  ;
: has-feminine-name  ( a -- )  ~has-feminine-name? bit-on  ;
: has-masculine-name  ( a -- )  ~has-feminine-name? bit-off  ;
: has-no-article  ( a -- )  ~has-no-article? bit-on  ;
: has-personal-name  ( a -- )  ~has-personal-name? bit-on  ;
: has-plural-name  ( a -- )  ~has-plural-name? bit-on  ;
: has-singular-name  ( a -- )  ~has-plural-name? bit-off  ;
: is-character  ( a -- )  ~is-character? bit-on  ;
: is-animal  ( a -- )  ~is-animal? bit-on  ;
: is-light  ( a -- )  ~is-light? bit-on  ;
: is-not-listed  ( a -- f )  ~is-not-listed? bit-on  ;
: is-lit  ( a -- )  ~is-lit? bit-on  ;
: is-not-lit  ( a -- )  ~is-lit? bit-off  ;
: is-cloth  ( a -- )  ~is-cloth? bit-on  ;
: is-decoration  ( a -- )  ~is-decoration? bit-on  ;
: is-global-indoor  ( a -- )  ~is-global-indoor? bit-on  ;
: is-global-outdoor  ( a -- )  ~is-global-outdoor? bit-on  ;
: is-human  ( a -- )  ~is-human? bit-on  ;
: is-location  ( a -- )  ~is-location? bit-on  ;
: is-indoor-location  ( a -- )  dup is-location ~is-indoor-location? bit-on  ;
: is-outdoor-location  ( a -- )  dup is-location ~is-indoor-location? bit-off  ;
: is-open  ( a -- )  ~is-open? bit-on  ;
: is-closed  ( a -- )  ~is-open? bit-off  ;
: times-open++  ( a -- )  ~times-open ?++  ;
: is-worn  ( a -- )  ~is-worn? bit-on  ;
: is-not-worn  ( a -- )  ~is-worn? bit-off  ;
: visits++  ( a -- )  ~visits ?++  ;

\ ----------------------------------------------
\ Campos calculados o seudo-campos

\ Los seudo-campos devuelven un cálculo. Sirven para añadir
\ una capa adicional de abstracción y simplificar el código.

\ Por conveniencia, en el caso de algunos de los campos binarios
\ creamos también palabras para la propiedad contraria.  Por
\ ejemplo, en las fichas existe el campo `~is-open?` para indicar
\ si un ente está abierto, pero creamos las palabras necesarias
\ para examinar y modificar tanto la propiedad de «cerrado» como
\ la de «abierto». Esto ayuda a escribir posteriormente el código
\ efectivo (pues no hace falta recordar si la propiedad real y por
\ tanto el campo de la ficha del ente era «abierto» o «cerrado») y
\ hace el código más conciso y legible.

: is-direction?  ( a -- f )  direction 0<>  ;
: is-familiar?  ( a -- f )  familiar 0>  ;
: is-visited?  ( a -- f )  visits 0>  ;
: is-not-visited?  ( a -- f )  visits 0=  ;
: conversations?  ( a -- f )  conversations 0<>  ;
: no-conversations?  ( a -- f )  conversations 0=  ;
: has-north-exit?  ( a -- f )  north-exit exit?  ;
: has-east-exit?  ( a -- f )  east-exit exit?  ;
: has-south-exit?  ( a -- f )  south-exit exit?  ;

: owns?  ( a1 a2 -- f )  owner =  ;
: belongs?  ( a1 a2 -- f )  swap owns?  ;
: owns  ( a1 a2 -- )  ~owner !  ;
: belongs  ( a1 a2 -- )  swap owns  ;

: belongs-to-protagonist?  ( a -- f )  owner protagonist% =  ;
: belongs-to-protagonist  ( a -- )  ~owner protagonist% swap !  ;

: is-living-being?  ( a -- f )
  \ ¿El ente es un ser vivo (aunque esté muerto)?
  dup is-vegetal?  over is-animal? or  swap is-human? or  ;

: is-there  ( a1 a2 -- )
  \ Hace que un ente sea la localización de otro.
  \ a1 = Ente que será la localización de a2
  \ a2 = Ente cuya localización será a1
  ~location !  ;

: taken  ( a -- )
  \ Hace que el protagonista sea la localización de un ente.
  protagonist% swap is-there  ;

: was-there  ( a1 a2 -- )
  \ Hace que un ente sea la localización previa de otro.
  \ a1 = Ente que será la localización previa de a2
  \ a2 = Ente cuya localización previa será a1
  ~previous-location !  ;

: is-there?  ( a1 a2 -- f )
  \ ¿Está un ente localizado en otro?
  \ a1 = Ente que actúa de localización
  \ a2 = Ente cuya localización se comprueba
  location =  ;

: was-there?  ( a1 a2 -- f )
  \ ¿Estuvo un ente localizado en otro?
  \ a1 = Ente que actúa de localización
  \ a2 = Ente cuya localización se comprueba
  previous-location =  ;

: is-global?  ( a -- f )
  \ ¿Es el ente un ente global?
  dup is-global-outdoor?
  swap is-global-indoor? or  ;

: my-location  ( -- a )
  \ Devuelve la localización del protagonista.
  protagonist% location  ;

: my-previous-location  ( -- a )
  \ Devuelve la localización anterior del protagonista.
  protagonist% previous-location  ;

: my-location!  ( a -- )
  \ Mueve el protagonista al ente indicado.
  protagonist% is-there  ;

: am-i-there?  ( a -- f )
  \ ¿Está el protagonista en la localización indicada?
  \ a = Ente que actúa de localización
  my-location =  ;

: is-outdoor-location?  ( a -- f )
  \ ¿Es el ente un escenario al aire libre?
  \ XXX TMP -- cálculo provisional
  drop 0  ;

: is-indoor-location?  ( a -- f )
  \ ¿Es el ente un escenario cerrado, no al aire libre?
  is-outdoor-location? 0=  ;

: am-i-outdoor?  ( -- f )
  \ ¿Está el protagonista en un escenario al aire libre?
  my-location is-outdoor-location?  ;

: am-i-indoor?  ( -- f )
  \ ¿Está el protagonista en un escenario cerrado, no al aire libre?
  am-i-outdoor? 0=  ;

: is-hold?  ( a -- f )
  \ ¿Es el protagonista la localización de un ente?
  location protagonist% =  ;

: is-not-hold?  ( a -- f )
  \ ¿No es el protagonista la localización de un ente?
  is-hold? 0=  ;

: is-hold  ( a -- )
  \ Hace que el protagonista sea la localización de un ente.
  ~location protagonist% swap !  ;

: is-worn-by-me?  ( a -- )
  \ ¿El protagonista lleva puesto el ente indicado?
  dup is-hold?  swap is-worn?  and  ;

: is-known?  ( a -- f )
  \ ¿El protagonista ya conoce el ente?
  dup belongs-to-protagonist?  \ ¿Es propiedad del protagonista?
  over is-visited? or  \ ¿O es un escenario ya visitado? (si no es un escenario, la comprobación no tendrá efecto)
  over conversations? or  \ ¿O ha hablado ya con él? (si no es un personaje, la comprobación no tendrá efecto)
  swap is-familiar?  or  ;  \ ¿O ya le es familiar?

: is-unknown?  ( a -- f )
  \ ¿El protagonista aún no conoce el ente?
  is-known? 0=  ;

: is-here?  ( a -- f )
  \ ¿Está un ente en la misma localización que el protagonista?
  \ El resultado depende de cualquiera de tres condiciones:
  dup location am-i-there?  \ ¿Está efectivamente en la misma localización?
  over is-global-outdoor? am-i-outdoor? and or \ ¿O es un «global exterior» y estamos en un escenario exterior?
  swap is-global-indoor? am-i-indoor? and or  ; \ ¿O es un «global interior» y estamos en un escenario interior?

: is-not-here?  ( a -- f )
  \ ¿Está un ente en otra localización que la del protagonista?
  \ XXX TODO -- not used
  is-here? 0=  ;

: is-here-and-unknown?  ( a -- f )
  \ ¿Está un ente en la misma localización que el protagonista y aún no es conocido por él?
  dup is-here? swap is-unknown? and  ;

: is-here  ( a -- )
  \ Hace que un ente esté en la misma localización que el protagonista.
  my-location swap is-there  ;

: is-accessible?  ( a -- f )
  \ ¿Es un ente accesible para el protagonista?
  dup is-hold?  swap is-here?  or  ;

: is-not-accessible?  ( a -- f )
  \ ¿Un ente no es accesible para el protagonista?
  is-accessible? 0=  ;

: can-be-looked-at?  ( a -- f )
  \ ¿El ente puede ser mirado?
  [false] [if]  \ Primera versión
    dup my-location =  \ ¿Es la localización del protagonista?
    over is-direction? or  \ ¿O es un ente dirección?
    over exits% = or  \ ¿O es el ente "salidas"?
    swap is-accessible? or  \ ¿O está accesible?
  [else]  \ Segunda versión, menos elegante pero más rápida y legible
    { entity }  \ Variable local creada con el parámetro de la pila
    true case
      entity am-i-there? of  true  endof \ ¿Es la localización del protagonista?
      entity is-direction? of  true  endof \ ¿Es un ente dirección?
      entity is-accessible? of  true  endof  \ ¿Está accesible?
      entity exits% = of  true  endof  \ ¿Es el ente "salidas"?
      false swap
    endcase
  [then]  ;

: can-be-taken?  ( a -- f )
  \ ¿El ente puede ser tomado?
  \ Se usa como norma general, para aquellos entes
  \ que no tienen un error específico indicado en el campo `~take-error#`
  dup is-decoration?
  over is-human? or
  swap is-character? or 0=  ;

: may-be-climbed?  ( a -- f )
  \ ¿El ente podría ser escalado? (Aunque en la práctica no sea posible).
  \ XXX TODO -- hacerlo mejor con un indicador en la ficha
  [false] [if]
  fallen-away%
  bridge%
  arch%
  bed%
  flags%
  rocks%
  table%
  [else]  false
  [then]  ;

: talked-to-the-leader?  ( -- f )
  \ ¿El protagonista ha hablado con el líder?
  leader% conversations 0<>  ;

: do-you-hold-something-forbidden?  ( -- f )
  \ ¿Llevas algo prohibido?
  \ Cálculo usado en varios lugares del programa,
  \ en relación a los refugiados.
  sword% is-accessible?  stone% is-accessible?  or  ;

: no-torch?  ( -- f )
  \ ¿La antorcha no está accesible y encendida?
  torch% is-not-accessible?  torch% is-not-lit?  or  ;

\ ----------------------------------------------
\ Herramientas de artículos y pronombres

\ La selección del artículo adecuado para el nombre de un ente tiene
\ su complicación. Depende por supuesto del número y género gramatical
\ del nombre, pero también de la relación con el protagonista
\ [distinción entre artículos definidos e indefinidos] y de la
\ naturaleza del ente [cosa o personaje].
\
\ Por conveniencia, consideramos como artículos ciertas palabras que
\ son adjetivos [como «esta», «ninguna»...], pues en la práctica para
\ el programa su manejo es idéntico: se usan para preceder a los
\ nombres bajo ciertas condiciones.
\
\ En este mismo apartado definimos palabras para calcular los
\ pronombres de objeto indirecto [le/s] y de objeto directo [la/s,
\ lo/s], así como terminaciones habituales.
\
\ Utilizamos una tabla de cadenas de longitud variable, apuntada por
\ una segunda tabla con sus direcciones.  Esto unifica y simplifica
\ los cálculos.

: hs,  ( ca len -- a1 )
  \ Compila una cadena en el diccionario y devuelve su dirección.
  here rot rot s,  ;

  \ Pronombres personales:
  s" él" hs, s" ella" hs, s" ellos" hs, s" ellas" hs,
  \ Adjetivos que se tratan como «artículos cercanos»:
  s" este" hs, s" esta" hs, s" estos" hs, s" estas" hs,
  \ Adjetivos que se tratan como «artículos distantes»:
  s" ese" hs, s" esa" hs, s" esos" hs, s" esas" hs,
  \ Adjetivos que se tratan como «artículos negativos»:
  s" ningún" hs, s" ninguna" hs, s" ningunos" hs, s" ningunas" hs,
  \ Artículos posesivos:
  s" tu" hs, s" tu" hs, s" tus" hs, s" tus" hs,
  \ Artículos definidos:
  s" el" hs, s" la" hs, s" los" hs, s" las" hs,
  \ Artículos indefinidos:
  s" un" hs, s" una" hs, s" unos" hs, s" unas" hs,

create 'articles  \ Tabla índice de los artículos
  \ Compilar las direcciones de los artículos:
  , , , ,  \ Indefinidos
  , , , ,  \ Definidos
  , , , ,  \ Posesivos
  , , , ,  \ «Negativos»
  , , , ,  \ «Distantes»
  , , , ,  \ «Cercanos»
  , , , ,  \ Pronombres personales

\ Separaciones entre artículos en la tabla índice (por tanto en celdas)
cell constant /article-gender-set  \ De femenino a masculino
2 cells constant /article-number-set  \ De plural a singular
4 cells constant /article-type-set  \ Entre grupos de diferente tipo

: article-number>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el número gramatical del ente.
  has-singular-name? /article-number-set and  ;

: article-gender>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical del ente.
  has-masculine-name? /article-gender-set and  ;

: article-gender+number>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical y el número del ente.
  dup article-gender>
  swap article-number> +  ;

: definite-article>  ( a -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos
  \ si el ente indicado necesita uno.
  dup has-definite-article?  \ Si el ente necesita siempre artículo definido
  swap is-known? or  \ O bien si el ente es ya conocido por el protagonista
  abs  \ Un grupo (pues los definidos son el segundo)
  /article-type-set *  ;

: possesive-article>  ( a -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos posesivos
  \ si el ente indicado necesita uno.
  belongs-to-protagonist? 2 and  \ Dos grupos (pues los posesivos son el tercero)
  /article-type-set *  ;

: negative-articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos negativos».
  3 /article-type-set *  ; \ Tres grupos (pues los negativos son el cuarto)

: undefined-articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos indefinidos.
  0  ; \ Desplazamiento cero, pues los indefinidos son el primer grupo.

: definite-articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos.
  /article-type-set  ;  \ Un grupo, pues los definidos son el segundo

: distant-articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos distantes».
  4 /article-type-set *  ;  \ Cuatro grupos, pues los «distantes» son el quinto

: not-distant-articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos cercanos».
  5 /article-type-set *  ;  \ Cinco grupos, pues los «cercanos» son el sexto

: personal-pronouns>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los pronombres personales.
  6 /article-type-set *  ;

: article-type  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el ente requiera un artículo definido, indefinido o posesivo.
  dup definite-article>  swap possesive-article>  max  ;

: >article  ( u -- ca1 len1 )
  \ Devuelve un artículo de la tabla de artículos
  \ a partir de su índice.
  'articles + @ count  ;

: (article)  ( a -- ca1 len1 )
  \ Devuelve el artículo apropiado para un ente.
  dup article-gender>  \ Desplazamiento según el género
  over article-number> +  \ Sumado al desplazamiento según el número
  swap article-type +  \ Sumado al desplazamiento según el tipo
  >article  ;

: article  ( a -- ca1 len1 | a 0 )
  \ Devuelve el artículo apropiado para un ente, si lo necesita;
  \ en caso contrario devuelve una cadena vacía.
  dup has-no-article? if  0  else  (article)  then  ;

: undefined-article  ( a -- ca1 len1 )
  \ Devuelve el artículo indefinido
  \ correspondiente al género y número de un ente.
  article-gender+number> undefined-articles> +
  >article  ;

: definite-article  ( a -- ca1 len1 )
  \ Devuelve el artículo definido
  \ correspondiente al género y número de un ente.
  article-gender+number> definite-articles> +
  >article  ;

: pronoun  ( a -- ca1 len1 )
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente.
  definite-article  s" lo" s" el" sreplace  ;

: ^pronoun  ( a -- ca1 len1 )
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente,
  \ con la primera letra mayúscula.
  pronoun ^uppercase  ;

: negative-article  ( a -- ca1 len1 )
  \ Devuelve el «artículo negativo»
  \ correspondiente al género y número de un ente.
  article-gender+number> negative-articles> +  >article  ;

: distant-article  ( a -- ca1 len1 )
  \ Devuelve el «artículo distante»
  \ correspondiente al género y número de un ente.
  article-gender+number> distant-articles> +
  >article  ;

: not-distant-article  ( a -- ca1 len1 )
  \ Devuelve el «artículo cercano»
  \ correspondiente al género y número de un ente.
  article-gender+number> not-distant-articles> +
  >article  ;

: personal-pronoun  ( a -- ca1 len1 )
  \ Devuelve el pronombre personal
  \ correspondiente al género y número de un ente.
  article-gender+number> personal-pronouns> +
  >article  ;

: plural-ending  ( a -- ca1 len1 )
  \ Devuelve la terminación adecuada del plural
  \ para el nombre de un ente.
  [false] [if]
    \ Método 1, «estilo BASIC»:
    has-plural-name? if  s" s"  else  ""  then
  [else]
    \ Método 2, sin estructuras condicionales, «estilo Forth»:
    s" s" rot has-plural-name? and
  [then]  ;

: plural-ending+  ( ca1 len1 a -- ca2 len2 )
  \ Añade a una cadena la terminación adecuada del plural
  \ para el nombre de un ente.
  plural-ending s+  ;

: gender-ending  ( a -- ca1 len1 )
  \ Devuelve la terminación adecuada del género gramatical
  \ para el nombre de un ente.
  [false] [if]
    \ Método 1, «estilo BASIC»
    has-feminine-name? if  s" a"  else  s" o"  then
  [else]
    [false] [if]
      \ Método 2, sin estructuras condicionales, «estilo Forth»
      s" oa" drop swap has-feminine-name? abs + 1
    [else]
      \ Método 3, similar, más directo
      c" oa" swap has-feminine-name? abs + 1+ 1
    [then]
  [then]  ;

: gender-ending+  ( ca1 len1 a -- ca2 len2 )
  \ Añade a una cadena la terminación adecuada para el género gramatical de un ente.
  gender-ending s+  ;

: noun-ending  ( a -- ca1 len1 )
  \ Devuelve la terminación adecuada para el nombre de un ente.
  dup gender-ending rot plural-ending s+  ;

' noun-ending alias adjective-ending
: noun-ending+  ( ca1 len1 a -- ca2 len2 )
  \ Añade a una cadena la terminación adecuada para el nombre de un ente.
  noun-ending s+  ;

' noun-ending+ alias adjective-ending+
: direct-pronoun  ( a -- ca1 len1 )
  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»).
  s" l" rot noun-ending s+  ;

: ^direct-pronoun  ( a -- ca1 len1 )
  \ Devuelve el pronombre de objeto directo para un ente («La/s»
  \ o «Lo/s»), con la primera letra mayúscula.
  direct-pronoun ^uppercase  ;

: indirect-pronoun  ( a -- ca1 len1 )
  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»).
  s" le" rot plural-ending s+  ;

: verb-number-ending  ( a -- ca1 len1 )
  \ Devuelve la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.
  s" n" rot has-plural-name? and  ;

: verb-number-ending+  ( ca1 len1 a -- ca2 len2 )
  \ Añade a una cadena la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.
  verb-number-ending s+  ;

: proper-verb-form  ( a ca1 len1 -- ca2 len2 )
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular un verbo
  \ cuyo sujeto se indica con el identificador de su entidad.
  \ ca len = Expresión
  \ a = Entidad
  \ XXX TODO -- not used
  rot has-plural-name? *>verb-ending  ;

: proper-grammar-number  ( a ca1 len1 -- ca2 len2 )
  \ Cambia por «s» (terminación del plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular las palabras de un texto,
  \ cuyo número gramatical se indica con el identificador de una entidad.
  \ ca len = Expresión
  \ a = Entidad
  \ XXX TODO -- not used
  rot has-plural-name? *>plural-ending  ;

\ ----------------------------------------------
\ Interfaz para los nombres de los entes

\ Como ya se explicó, el nombre de cada ente se guarda en una
\ cadena dinámica [que se crea en la memoria con `allocate`, no
\ en el espacio del diccionario del sistema].  El manejo de
\ estas cadenas dinámicas se hace con el módulo
\ correspondiente de Forth Foundation Library.
\
\ En la ficha del ente se guarda solo la dirección de la
\ cadena dinámica, en el campo `~name-str`.  Por ello hacen
\ falta palabras que hagan de interfaz para gestionar los
\ nombres de ente de forma análoga a como se hace con el resto
\ de datos de su ficha.

: name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente.
  \ ca len = Nombre
  \ a1 = Ente
  name-str str-set  ;

: p-name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente, y lo marca como plural.
  \ ca len = Nombre
  \ a1 = Ente
  dup has-plural-name name!  ;

: s-name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente, y lo marca como singular.
  \ ca len = Nombre
  \ a1 = Ente
  dup has-singular-name name!  ;

: fs-name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical femenino y singular.
  \ ca len = Nombre
  \ a1 = Ente
  dup has-feminine-name s-name!  ;

: fp-name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical femenino y plural.
  \ ca len = Nombre
  \ a1 = Ente
  dup has-feminine-name p-name!  ;

: ms-name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical masculino y singular.
  \ ca len = Nombre
  \ a1 = Ente
  dup has-masculine-name s-name!  ;

: mp-name!  ( ca len a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical masculino y plural.
  \ ca len = Nombre
  \ a1 = Ente
  dup has-masculine-name p-name!  ;

: name  ( a -- ca1 len1 )
  \ Devuelve el nombre de un ente.
  \ a = Ente
  \ ca1 len1 = Nombre
  name-str str-get  ;

: ?name  ( a|0 -- a1 u1|0 )
  \ Devuelve el nombre de un ente, si es tal; una cadena vacía si es cero.
  \ XXX TMP -- solo se usa para depuración
  ?dup if  name  else  ""  then  ;

: ?.name  ( a|0 -- )
  \ Imprime el nombre de un ente, si es tal; una cadena vacía si es cero.
  \ XXX TMP -- solo se usa para depuración
  ?name type  ;

: ^name  ( a -- ca1 len1 )
  \ Devuelve el nombre de un ente, con la primera letra mayúscula.
  name ^uppercase  ;

: name&  ( a ca1 len1 -- ca2 len2 )
  \ Añade a un (supuesto) artículo el nombre de un ente.
  \ a = Ente
  \ ca1 len1 = Artículo correspondiente (o cualquier otro texto)
  \ ca2 len2 = Nombre completo
  rot name s&  ;

: full-name  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente, con el artículo que le corresponda.
  dup article name&  ;

: ^full-name  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente, con el artículo que le corresponda (con la primera letra en mayúscula).
  full-name ^uppercase  ;

: defined-full-name  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente, con un artículo definido.
  dup definite-article name&  ;

: undefined-full-name  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente, con un artículo indefinido.
  dup undefined-article name&  ;

: negative-full-name  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente, con un «artículo negativo».
  dup negative-article name&  ;

: distant-full-name  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente, con un «artículo distante».
  dup distant-article name&  ;

: nonhuman-subjective-negative-name  ( a -- ca1 len1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente (no humano), desde el punto de vista del protagonista.
  \ Nota: En este caso hay que usar `negative-full-name` antes de `s{` y pasar la cadena
  \ mediante la pila de retorno; de otro modo `s{` y `}s` no pueden calcular bien
  \ el crecimiento de la pila.
  negative-full-name 2>r
  s{
  2r> 2dup 2dup  \ Tres nombres repetidos con «artículo negativo»
  s" eso" s" esa cosa" s" tal cosa"  \ Tres alternativas
  }s  ;

: human-subjective-negative-name  ( a -- ca1 len1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente (humano), desde el punto de vista del protagonista.
  dup is-known?
  if  full-name  else  drop s" nadie"  then  ;

: subjective-negative-name  ( a -- ca1 len1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente, desde el punto de vista del protagonista.
  dup is-human?
  if  human-subjective-negative-name
  else  nonhuman-subjective-negative-name
  then  ;

: /l$  ( a -- ca1 len1 | a1 0 )
  \ Devuelve la terminación «l» del artículo determinado masculino para añadirla a la preposición «a», si un ente humano lo requiere para ser usado como objeto directo; o una cadena vacía.
  \ XXX TODO -- not used
  s" l" rot has-personal-name? 0= and  ;

: a/$  ( a -- ca1 len1 | a1 0 )
  \ Devuelve la preposición «a» si un ente lo requiere para ser usado como objeto directo; o una cadena vacía.
  s" a" rot is-human? and  ;

: a/l$  ( a -- ca1 len1 )
  \ Devuelve la preposición «a», con posible artículo determinado, si un ente lo requiere para ser usado como objeto directo.
  \ XXX TODO -- not used
  a/$ dup if  /l$ s+  then  ;

: subjective-negative-name-as-direct-object  ( a -- ca1 len1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente, desde el punto de vista del protagonista, para ser usado como objeto directo.
  dup a/$ rot subjective-negative-name s&  ;

: .full-name  ( a -- )
  \ Imprime el nombre completo de un ente.
  \ XXX TODO -- not used
  full-name paragraph  ;

\ }}} ==========================================================
section( Algunas cadenas calculadas y operaciones con ellas)  \ {{{

\ XXX TODO -- ¿Mover a otra sección?

: «open»|«closed»  ( a -- ca1 len1 )
  \ Devuelve «abierto/a/s» a «cerrado/a/s» según corresponda a un ente.
  dup is-open? if  s" abiert"  else  s" cerrad"  then
  rot noun-ending s+  ;

: player-gender-ending$  ( -- ca len )
  \ Devuelve la terminación «a» u «o» según el sexo del jugador.
  [false] [if]
    \ Método 1, «estilo BASIC»:
    woman-player? @ if  s" a"  else  s" o"  then
  [else]
    \ Método 2, sin estructuras condicionales, «estilo Forth»:
    c" oa" woman-player? @ abs + 1+ 1
  [then]  ;

: player-gender-ending$+  ( ca1 len1 -- ca2 len2 )
  \ Añade a una cadena la terminación «a» u «o» según el sexo del jugador.
  player-gender-ending$ s+  ;

\ }}} ==========================================================
section( Operaciones elementales con entes)  \ {{{

\ Algunas operaciones sencillas relacionadas con la trama.
\
\ Alguna es necesario crearla como vector porque se usa en las
\ descripciones de los entes o en las acciones, antes de
\ definir la trama.

defer lock-found  \ Encontrar el candado; la definición está en `(lock-found)`

0 constant limbo \ Marcador para usar como localización de entes inexistentes
: vanished?  ( a -- f )
  \ ¿Está un ente desaparecido?
  location limbo =  ;

: not-vanished?  ( a -- f )
  \ ¿No está un ente desaparecido?
  vanished? 0=  ;

: vanish  ( a -- )
  \ Hace desaparecer un ente llevándolo al «limbo».
  limbo swap is-there  ;

: vanish-if-hold  ( a -- )
  \ Hace desaparecer un ente si su localización es el protagonista.
  \ XXX TODO -- not used
  dup is-hold? if  vanish  else  drop  then  ;

\ }}} ==========================================================
section( Herramientas para crear las fichas de la base de datos)  \ {{{

\ No es posible reservar el espacio necesario para las fichas
\ hasta saber cuántas necesitaremos (a menos que usáramos una
\ estructura un poco más sofisticada con fichas separadas pero
\ enlazadas entre sí, muy habitual también y fácil de crear).  Por
\ ello la palabra "'entities" (que devuelve la dirección de la
\ base de datos) se crea como un vector, para asignarle
\ posteriormente su dirección de ejecución.  Esto permite crear un
\ nuevo ente fácilmente, sin necesidad de asignar previamente el
\ número de fichas a una constante.

defer 'entities  \ Dirección de los entes; vector que después será redirigido a la palabra real
0 value #entities  \ Contador de entes, que se actualizará según se vayan creando

: #>entity  ( u -- a )
  \ Devuelve la dirección de la ficha de un ente a partir de su número ordinal
  \ (el número del primer ente es el cero).
  /entity * 'entities +  ;

: entity>#  ( a -- u )
  \ Devuelve el número ordinal de un ente (el primero es el cero)
  \ a partir de la dirección de su ficha.
  'entities - /entity /  ;

: entity:  ( "name" -- )
  \ Crea un nuevo identificador de ente,
  \ que devolverá la dirección de su ficha.
  create
    #entities ,  \ Guardar la cuenta en el cuerpo de la palabra recién creada
    #entities 1+ to #entities  \ Actualizar el contador
  does>  ( pfa -- a )
    @ #>entity  ;  \ El identificador devolverá la dirección de su ficha

: erase-entity  ( a -- )
  \ Rellena con ceros la ficha de un ente.
  /entity erase  ;

: backup-entity  ( a -- x0 x1 x2 x3 x4 x5 x6 )
  \ Respalda los datos de un ente
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético, para facilitar la edición).
  >r
  r@ description-xt
  r@ init-xt
  r@ can-i-enter-location?-xt
  r@ after-describing-location-xt
  r@ after-listing-entities-xt
  r@ before-describing-location-xt
  r@ before-leaving-location-xt
  r> name-str  ;

: restore-entity  ( x0 x1 x2 x3 x4 x5 x6 a -- )
  \ Restaura los datos de un ente
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético inverso, para facilitar la edición).
  >r
  r@ ~name-str !
  r@ ~before-leaving-location-xt !
  r@ ~before-describing-location-xt !
  r@ ~after-listing-entities-xt !
  r@ ~after-describing-location-xt !
  r@ ~can-i-enter-location?-xt !
  r@ ~init-xt !
  r> ~description-xt !  ;

: setup-entity  ( a -- )
  \ Prepara la ficha de un ente para ser completada con sus datos .
  >r r@ backup-entity  r@ erase-entity  r> restore-entity  ;

0 value self%  \ Ente cuyos atributos, descripción o trama están siendo definidos (usado para aligerar la sintaxis)
: :name-str  ( a -- )
  \ Crea una cadena dinámica nueva para guardar el nombre del ente.
  [debug-init] [if]  s" Inicio de :NAME-STR" debug [then]
  dup name-str ?dup
  [debug-init] [if]  s" A punto para STR-FREE" debug [then]
  ?? str-free
  str-new swap ~name-str !
  [debug-init] [if]  s" Final de :NAME-STR" debug [then]  ;

: [:attributes]  ( a -- )
  \ Inicia la definición de propiedades de un ente.
  \ Esta palabra se ejecuta cada vez que hay que restaurar los datos del ente,
  \ y antes de la definición de atributos contenida en la palabra
  \ correspondiente al ente.
  \ El identificador del ente está en la pila porque se compiló con `literal`
  \ cuando se creó la palabra de atributos.
  dup to self%  \ Actualizar el puntero al ente
  dup :name-str  \ Crear una cadena dinámica para el campo `~name-str`
  setup-entity  ;

: default-description  ( -- )
  \ Descripción predeterminada de los entes
  \ para los que no se ha creado una palabra propia de descripción.
  ^is-normal$ paragraph  ;

: (:attributes)  ( a xt -- )
  \ Operaciones preliminares para la definición de atributos de un ente.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define sus atributos.
  \ a = Ente para la definición de cuyos atributos se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~init-xt !  \ Conservar la dirección de ejecución en la ficha del ente
  ['] default-description over ~description-xt !  \ Poner la descripción predeterminada
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: noname-roll  ( a xt colon-sys -- colon-sys a xt )
  \ Mueve los parámetros que nos interesan a la parte alta de la pila;
  \ se usa tras crear con `:noname` una palabra relativa a un ente.
  \ Esta palabra es necesaria porque `:noname` deja en la pila
  \ sus valores de control (colon-sys), que en el caso de Gforth
  \ son tres elementos, para ser consumidos al finalizar la compilación de la
  \ palabra creada.
  \ a = Ente para el que se creó la palabra
  \ xt = Dirección de ejecución de la palabra sin nombre creada por `:noname`
  \ colon-sys = Valores de control dejados por `:noname`
  5 roll 5 roll  ;

: :attributes  ( a -- )
  \ Inicia la creación de una palabra sin nombre que definirá las propiedades de un ente.
  :noname noname-roll
  (:attributes)  \ Crear la palabra y hacer las operaciones preliminares
  postpone [:attributes]  ;  \ Compilar la palabra `[:attributes]` en la palabra creada, para que se ejecute cuando sea llamada

: ;attributes  ( sys-col -- )  postpone ;  ;  immediate
: init-entity  ( a -- )
  \ Restaura la ficha de un ente a su estado original.
  [debug-init] [if]  s" Inicio de INIT-ENTITY" debug dup entity># cr ." Entity=" .  [then]
  init-xt
  [debug-init] [if]  s" Antes de EXECUTE" debug  [then]
  execute
  [debug-init] [if]  s" Final de INIT-ENTITY" debug  [then]  ;

: init-entities  ( -- )
  \ Restaura las fichas de los entes a su estado original.
  #entities 0 do
    [debug-init] [if]  i cr ." about to init entity #" .  [then]
    i #>entity init-entity
    \ i #>entity full-name space type .s?  \ XXX INFORMER
  loop  ;

\ }}} ==========================================================
section( Herramientas para crear las descripciones)  \ {{{

\ No almacenamos las descripciones en la base de datos junto
\ con el resto de atributos de los entes, sino que para cada
\ ente creamos una palabra que imprime su descripción, lo que
\ es mucho más flexible: La descripción podrá variar en
\ función del desarrollo del juego y adaptarse a las
\ circunstancias, e incluso sustituir en algunos casos al
\ código que controla la trama del juego.
\
\ Así pues, lo que almacenamos en la ficha del ente, en el
\ campo `~description-xt`, es la dirección de ejecución de la
\ palabra que imprime su descripción.
\
\ Por tanto, para describir un ente basta tomar de su ficha el
\ contenido de `~description-xt`, y llamar a `execute`.

false value sight  \ Guarda el ente dirección al que se mira en un escenario (o el propio ente escenario); se usa en las palabras de descripción de escenarios
: [:description]  ( a -- )
  \ Operacionas previas a la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al comienzo de la palabra de descripción.
  \ El identificador del ente está en la pila porque se compiló con `literal` cuando se creó la palabra de descripción.
  to self%  ;  \ Actualizar el puntero al ente, usado para aligerar la sintaxis

: (:description)  ( a xt -- )
  \ Operaciones preliminares para la definición de la descripción de un ente.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que crea su descripción.
  \ a = Ente para cuya descripción se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~description-xt !  \ Conservar la dirección de ejecución en la ficha del ente
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: :description  ( a -- )
  \ Inicia la definición de una palabra de descripción para un ente.
  :noname noname-roll
  (:description)  \ Hacer las operaciones preliminares
  postpone [:description]  ;  \ Compilar la palabra `[:description]` en la palabra creada, para que se ejecute cuando sea llamada

: [;description]  ( -- )
  \ Operaciones finales tras la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al final de la palabra de descripción.
  false to sight  ;  \ Poner a cero el selector de vista, para evitar posibles errores

: ;description  ( colon-sys -- )
  \ Termina la definición de una palabra de descripción de un ente.
  postpone [;description]  \ Compilar la palabra `[;description]` en la palabra creada, para que se ejecute cuando sea llamada
  postpone ;
  ;  immediate
: (describe)  ( a -- )
  \ Ejecuta la palabra de descripción de un ente.
  ~description-xt perform  ;

: .location-name  ( a -- )
  \ Imprime el nombre de un ente escenario, como cabecera de su descripción.
  \ XXX TODO -- añadir el artículo correspondiente o no, dependiendo de un indicador de la ficha:
  \     pasaje de la serpiente -- el pasaje de la serpiente
  \     el paso del Perro -- el paso del Perro
  \     un tramo de cueva -- un tramo de cueva
  \     un lago interior -- el lago interior
  \     hogar de Ambrosio -- el hogar de Ambrosio
  [debug-map] [if]  dup  [then]
  name ^uppercase location-name-color paragraph
  [debug-map] [if]
    entity># location-01% entity># - 1+ ."  [ location #" . ." ]"
  [then]  ;

: (describe-location)  ( a -- )
  \ Describe un ente escenario.
  dup to sight
  location-description-color (describe)  ;

: describe-location  ( a -- )
  \ Describe un ente escenario, con borrado de pantalla y título.
  clear-screen-for-location dup .location-name  (describe-location)  ;

: describe-other  ( a -- )
  \ Describe un ente de otro tipo.
  description-color (describe)  ;

: describe-direction  ( a -- )
  \ Describe un ente dirección.
  to sight  \ Poner el ente dirección en `sight`
  my-location describe-other  ;  \ Y describir el escenario actual como un ente normal; ahí se hace la distinción

: description-type  ( a -- u )
  \ Convierte un ente en el tipo de descripción que requiere.
  \ a = Ente
  \ u = Tipo de descripción (4:salida, 2:dirección, 1:escenario, 0:otros, 3:¡error!)
  \ Nota: Un resultado de 3 significaría que el ente es a la vez dirección y escenario.
  dup is-location? abs
  [true] [if]
  swap is-direction? 2 and +
  [else]
  \ XXX TODO -- unfinished
  \ XXX TODO -- not used yet
  over is-direction? 2 and +
  swap exits% = 4 and +
  [then]  ;

: describe  ( a -- )
  \ Describe un ente, según su tipo.
  dup description-type
  case
    0 of  describe-other  endof
    1 of  describe-location  endof
    2 of  describe-direction  endof
    abort" Error fatal en DESCRIBE : dato incorrecto"  \ XXX INFORMER
  endcase  ;

: uninteresting-direction  ( -- )
  \ Muestra la descripción de la direcciones que no tienen nada especial.
  uninteresting-direction$ paragraph  ;

\ }}} ==========================================================
section( Identificadores de entes)  \ {{{

\ Cada ente es identificado mediante una palabra. Los
\ identificadores de entes se crean con la palabra `entity:`.
\ Cuando se ejecutan devuelven la dirección en memoria de la
\ ficha del ente en la base de datos, que después puede ser
\ modificada con un identificador de campo para convertirla en
\ la dirección de memoria de un campo concreto de la ficha.
\
\ Para reconocer mejor los identificadores de entes usamos el
\ sufijo «%» en sus nombres.
\
\ Los entes escenario usan como nombre de identificador el número
\ que tienen en la versión original del programa. Esto hace más
\ fácil la adaptación del código original en BASIC.  Además, para
\ que algunos cálculos tomados del código original funcionen, es
\ preciso que los entes escenario se creen ordenados por ese
\ número.
\
\ El orden en que se definan los restantes identificadores es
\ irrelevante.  Si están agrupados por tipos y en orden
\ alfabético es solo por claridad.

entity: ulfius%
' ulfius% is protagonist%  \ Actualizar el vector que apunta al ente protagonista

\ Entes que son (seudo)personajes:
entity: ambrosio%
entity: (leader%) ' (leader%) is leader%
entity: soldiers%
entity: refugees%
entity: officers%

\ Entes que son objetos:
entity: altar%
entity: arch%
entity: bed%
entity: bridge%
entity: candles%
entity: cave-entrance%
entity: cloak%
entity: cuirasse%
entity: door%
entity: emerald%
entity: fallen-away%
entity: flags%
entity: flint%
entity: grass%
entity: idol%
entity: key%
entity: lake%
entity: lock%
entity: log%
entity: piece%
entity: rags%
entity: ravine-wall%
entity: rocks%
entity: snake%
entity: (stone%) ' (stone%) is stone%
entity: (sword%) ' (sword%) is sword%
entity: table%
entity: thread%
entity: (torch%) ' (torch%) is torch%
entity: wall%  \ XXX TODO -- unfinished
entity: waterfall%

\ Entes escenario (en orden de número):
entity: (location-01%) ' (location-01%) is location-01%
entity: location-02%
entity: location-03%
entity: location-04%
entity: location-05%
entity: location-06%
entity: location-07%
entity: location-08%
entity: location-09%
entity: location-10%
entity: location-11%
entity: location-12%
entity: location-13%
entity: location-14%
entity: location-15%
entity: location-16%
entity: location-17%
entity: location-18%
entity: location-19%
entity: location-20%
entity: location-21%
entity: location-22%
entity: location-23%
entity: location-24%
entity: location-25%
entity: location-26%
entity: location-27%
entity: location-28%
entity: location-29%
entity: location-30%
entity: location-31%
entity: location-32%
entity: location-33%
entity: location-34%
entity: location-35%
entity: location-36%
entity: location-37%
entity: location-38%
entity: location-39%
entity: location-40%
entity: location-41%
entity: location-42%
entity: location-43%
entity: location-44%
entity: location-45%
entity: location-46%
entity: location-47%
entity: location-48%
entity: location-49%
entity: location-50%
entity: location-51%

\ Entes globales:
entity: sky%
entity: floor%
entity: ceiling%
entity: clouds%
entity: cave%  \ XXX TODO -- unfinished

\ Entes virtuales
\ (necesarios para la ejecución de algunos comandos):
entity: inventory%
entity: (exits%)  ' (exits%) is exits%
entity: north%
entity: south%
entity: east%
entity: west%
entity: up%
entity: down%
entity: out%
entity: in%
entity: enemy%

\ Tras crear los identificadores de entes
\ ya conocemos cuántos entes hay
\ (pues la palabra `entity:` actualiza el contador `#entities`)
\ y por tanto podemos reservar espacio para la base de datos:

#entities /entity * constant /entities  \ Espacio necesario para guardar todas las fichas, en octetos
create ('entities) /entities allot  \ Reservar el espacio en el diccionario
' ('entities) is 'entities  \ Asignar el vector a la palabra real
'entities /entities erase  \ Llenar la zona con ceros, para mayor seguridad

\ }}} ==========================================================
section( Herramientas para crear conexiones entre escenarios)  \ {{{

\ XXX Nota.: Este código quedaría mejor con el resto de herramientas
\ de la base de datos, para no separar la lista de entes de sus datos.
\ Pero se necesita usar los identificadores de los entes dirección.
\ Se podría solucionar con vectores, más adelante.

\ Para crear el mapa hay que hacer dos operaciones con los
\ entes escenario: marcarlos como tales, para poder
\ distinguirlos como escenarios; e indicar a qué otros entes
\ escenario conducen sus salidas.
\
\ La primera operación se hace guardando un valor buleano «cierto»
\ en el campo `~is-location?` del ente.  Por ejemplo:

\   cave% ~is-location? bit-on

\ O bien mediante la palabra creada para ello en la interfaz
\ básica de campos:

\   cave% is-location

\ La segunda operación se hace guardando en los campos de
\ salida del ente los identificadores de los entes a que cada
\ salida conduzca.  No hace falta ocuparse de las salidas
\ impracticables porque ya estarán a cero de forma
\ predeterminada.  Por ejemplo:

\   path% cave% ~south-exit !  \ Hacer que la salida sur de `cave%` conduzca a `path%`
\   cave% path% ~north-exit !  \ Hacer que la salida norte de `path%` conduzca a `cave%`

\ No obstante, para hacer más fácil este segundo paso, hemos
\ creado unas palabras que proporcionan una sintaxis específica,
\ como mostraremos a continuación.

0 [if]  \ XXX TODO -- unfinished

create opposite-exits
south-exit> ,
north-exit> ,
west-exit> ,
east-exit> ,
down-exit> ,
up-exit> ,
in-exit> ,
out-exit> ,

create opposite-direction-entities
south% ,
north% ,
west% ,
east% ,
down% ,
up% ,
in% ,
out% ,

[then]

\ Necesitamos una tabla que nos permita traducir esto:
\
\ ENTRADA: Un puntero correspondiente a un campo de dirección
\ de salida en la ficha de un ente.
\
\ SALIDA: El identificador del ente dirección al que se
\ refiere esa salida.

create exits-table  \ Tabla de traducción de salidas
#exits cells allot  \ Reservar espacio para tantas celdas como salidas

: >exits-table>  ( u -- a )  first-exit> - exits-table +  ;
  \ Apunta a la dirección de un elemento de la tabla de direcciones.
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  \ a = Dirección del ente dirección correspondiente en la tabla

: exits-table!  ( a u -- )  >exits-table> !  ;
  \ Guarda un ente _a_ en una posición de la tabla de salidas.
  \ a = Ente dirección
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)

: exits-table@  ( u -- a )  >exits-table> @  ;
  \ Devuelve un ente dirección a partir de un campo de dirección.
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  \ a = Ente dirección

\ Rellenar cada elemento de la tabla con un ente de salida,
\ usando como puntero el campo análogo de la ficha.
\ Haciéndolo de esta manera no importa el orden en que se rellenen los elementos.
north% north-exit> exits-table!
south% south-exit> exits-table!
east% east-exit> exits-table!
west% west-exit> exits-table!
up% up-exit> exits-table!
down% down-exit> exits-table!
out% out-exit> exits-table!
in% in-exit> exits-table!

0 [if]  \ XXX TODO -- unfinished
: opposite-exit  ( a1 -- a2 )
  \ Devuelve la dirección cardinal opuesta a la indicada.
  first-exit> - opposite-exits + @  ;

: opposite-exit%  ( a1 -- a2 )
  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada.
  \ a1 = entidad de dirección
  \ a2 = entidad de dirección, opuesta a a1
  first-exit> - opposite-direction-entities + @  ;

[then]

\ A continuación definimos palabras para proporcionar la
\ siguiente sintaxis (primero origen y después destino en la
\ pila, como es convención en Forth):

\   \ Hacer que la salida sur de `cave%` conduzca a `path%`
\   \ pero sin afectar al sentido contrario:
\   cave% path% s-->

\   \ Hacer que la salida norte de `path%` conduzca a `cave%`
\   \ pero sin afectar al sentido contrario:
\   path% cave% n-->

\ O en un solo paso:

\   \ Hacer que la salida sur de `cave%` conduzca a `path%`
\   \ y al contrario: la salida norte de `path%` conducirá a `cave%`:
\   cave% path% s<-->

: -->  ( a1 a2 u -- )
  \ Conecta el ente a1 con el ente a2 mediante la salida indicada por el desplazamiento u.
  \ a1 = Ente origen de la conexión
  \ a2 = Ente destino de la conexión
  \ u = Desplazamiento del campo de dirección a usar en a1
  rot + !  ;

: -->|  ( a1 u -- )
  \ Cierra la salida del ente a1 indicada por el desplazamiento u.
  \ a1 = Ente origen de la conexión
  \ u = Desplazamiento del campo de dirección a usar en a1
  + no-exit swap !  ;

\ Conexiones unidireccionales

: n-->  ( a1 a2 -- )
  \ Comunica la salida norte del ente a1 con el ente a2.
  north-exit> -->  ;

: s-->  ( a1 a2 -- )
  \ Comunica la salida sur del ente a1 con el ente a2.
  south-exit> -->  ;

: e-->  ( a1 a2 -- )
  \ Comunica la salida este del ente a1 con el ente a2.
  east-exit> -->  ;

: w-->  ( a1 a2 -- )
  \ Comunica la salida oeste del ente a1 con el ente a2.
  west-exit> -->  ;

: u-->  ( a1 a2 -- )
  \ Comunica la salida hacia arriba del ente a1 con el ente a2.
  up-exit> -->  ;

: d-->  ( a1 a2 -- )
  \ Comunica la salida hacia abajo del ente a1 con el ente a2.
  down-exit> -->  ;

: o-->  ( a1 a2 -- )
  \ Comunica la salida hacia fuera del ente a1 con el ente a2.
  out-exit> -->  ;

: i-->  ( a1 a2 -- )
  \ Comunica la salida hacia dentro del ente a1 con el ente a2.
  in-exit> -->  ;

: n-->|  ( a1 -- )
  \ Desconecta la salida norte del ente a1.
  north-exit> -->|  ;

: s-->|  ( a1 -- )
  \ Desconecta la salida sur del ente a1.
  south-exit> -->|  ;

: e-->|  ( a1 -- )
  \ Desconecta la salida este del ente a1.
  east-exit> -->|  ;

: w-->|  ( a1 -- )
  \ Desconecta la salida oeste del ente a1.
  west-exit> -->|  ;

: u-->|  ( a1 -- )
  \ Desconecta la salida hacia arriba del ente a1.
  up-exit> -->|  ;

: d-->|  ( a1 -- )
  \ Desconecta la salida hacia abajo del ente a1.
  down-exit> -->|  ;

: o-->|  ( a1 -- )
  \ Desconecta la salida hacia fuera del ente a1.
  out-exit> -->|  ;

: i-->|  ( a1 -- )
  \ Desconecta la salida hacia dentro del ente a1.
  in-exit> -->|  ;

\ Conexiones bidireccionales

: n<-->  ( a1 a2 -- )
  \ Comunica la salida norte del ente a1 con el ente a2 (y al contrario).
  2dup n-->  swap s-->  ;

: s<-->  ( a1 a2 -- )
  \ Comunica la salida sur del ente a1 con el ente a2 (y al contrario).
  2dup s-->  swap n-->  ;

: e<-->  ( a1 a2 -- )
  \ Comunica la salida este del ente a1 con el ente a2 (y al contrario).
  2dup e-->  swap w-->  ;

: w<-->  ( a1 a2 -- )
  \ Comunica la salida oeste del ente a1 con el ente a2 (y al contrario).
  2dup w-->  swap e-->  ;

: u<-->  ( a1 a2 -- )
  \ Comunica la salida hacia arriba del ente a1 con el ente a2 (y al contrario).
  2dup u-->  swap d-->  ;

: d<-->  ( a1 a2 -- )
  \ Comunica la salida hacia abajo del ente a1 con el ente a2 (y al contrario).
  2dup d-->  swap u-->  ;

: o<-->  ( a1 a2 -- )
  \ Comunica la salida hacia fuera del ente a1 con el ente a2 (y al contrario).
  2dup o-->  swap i-->  ;

: i<-->  ( a1 a2 -- )
  \ Comunica la salida hacia dentro del ente a1 con el ente a2 (y al contrario).
  2dup i-->  swap o-->  ;

: n|<-->|  ( a1 a2 -- )
  \ Desconecta la salida norte del ente a1 con el ente a2 (y al contrario).
  s-->|  n-->|  ;

: s|<-->|  ( a1 a2 -- )
  \ Desconecta la salida sur del ente a1 con el ente a2 (y al contrario).
  n-->|  s-->|  ;

: e|<-->|  ( a1 a2 -- )
  \ Desconecta la salida este del ente a1 con el ente a2 (y al contrario).
  w-->|  e-->|  ;

: w|<-->|  ( a1 a2 -- )
  \ Desconecta la salida oeste del ente a1 con el ente a2 (y al contrario).
  e-->|  w-->|  ;

: u|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia arriba del ente a1 con el ente a2 (y al contrario).
  d-->|  u-->|  ;

: d|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia abajo del ente a1 con el ente a2 (y al contrario).
  u-->|  d-->|  ;

: o|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia fuera del ente a1 con el ente a2 (y al contrario).
  i-->|  o-->|  ;

: i|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia dentro del ente a1 con el ente a2 (y al contrario).
  o-->|  i-->|  ;

\ Por último, definimos dos palabras para hacer
\ todas las asignaciones de salidas en un solo paso.

: exits!  ( a1 ... a8 a0 -- )
  \ Asigna todas las salidas de un ente escenario.
  \ a1 ... a8 = Entes escenario de salida (o cero) en el orden habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera
  \ a0 = Ente escenario cuyas salidas hay que modificar
  >r
  r@ ~out-exit !
  r@ ~in-exit !
  r@ ~down-exit !
  r@ ~up-exit !
  r@ ~west-exit !
  r@ ~east-exit !
  r@ ~south-exit !
  r> ~north-exit !  ;

: init-location  ( a1 ... a8 a0 -- )
  \ Marca un ente como escenario y le asigna todas las salidas. .
  \ a1 ... a8 = Entes escenario de salida (o cero) en el orden habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera
  \ a0 = Ente escenario cuyas salidas hay que modificar
  dup is-location exits!  ;

\ }}} ==========================================================
section( Recursos para las descripciones de entes)  \ {{{

\ Las palabras de esta sección se usan para
\ construir las descripciones de los entes.
\ Cuando su uso se vuelve más genérico, se mueven
\ a la sección de textos calculados.

\ ----------------------------------------------
\ Albergue de los refugiados

: the-refugees$  ( -- ca len )
  leader% conversations?
  if  s" los refugiados"  else  s" todos"  then  ;

: ^the-refugees$  ( -- ca len )
  the-refugees$ ^uppercase  ;

: they-don't-let-you-pass$  ( -- ca len )
  \ Mensaje de que los refugiados no te dejan pasar.
  s{
  s" te" s? (they)-block$ s&
  s" te rodean,"
  s{ s" impidiéndote" s" impidiendo"
  s" obstruyendo" s" obstruyéndote"
  s" bloqueando" s" bloqueándote" }s&
  }s the-pass$ s&  ;

: the-pass-free$  ( -- ca len )
  \ Variante de «libre el paso».
  s" libre" the-pass$ s&  ;

: they-let-you-pass-0$  ( -- ca len )
  \ Primera versión del mensaje de que te dejan pasar.
  s{
  s" te" s? s" han dejado" s&
  s" se han" s{ s" apartado" s" echado a un lado" }s& s" para dejar" s& s" te" s?+
  }s the-pass-free$ s&  ;

: they-let-you-pass-1$  ( -- ca len )
  \ Segunda versión del mensaje de que te dejan pasar.
  s" se han" s{ s" apartado" s" retirado" }s&
  s" a" s{ s{ s" los" s" ambos" }s s" lados" s& s" uno y otro lado" }s& s?& comma+
  s{ s" dejándote" s" dejando" s" para dejar" s" te" s?+ }s&
  the-pass-free$ s&  ;

: they-let-you-pass-2$  ( -- ca len )
  \ Tercera versión del mensaje de que te dejan pasar.
  s" ya no" they-don't-let-you-pass$ s& s" como antes" s?&  ;

: they-let-you-pass$  ( -- ca len )
  \ Mensaje de que te dejan pasar.
  ['] they-let-you-pass-0$
  ['] they-let-you-pass-1$
  ['] they-let-you-pass-2$
  3 choose execute  ;

: the-leader-said-they-want-peace$  ( -- ca len )
  \ El líder te dijo qué buscan los refugiados.
  s" que," s" tal y" s?& s" como" s& leader% full-name s&
  s" te" s?& s" ha" s&{ s" referido" s" hecho saber" s" contado" s" explicado" }s& comma+
  they-want-peace$ s&  ;

: you-don't-know-why-they're-here$  ( -- ca len )
  \ No sabes por qué están aquí los refugiados.
  s{  s" Te preguntas"
      s" No" s{  s" terminas de"
                  s" acabas de"
                  s" aciertas a"
                  s" puedes"
                }s& to-understand$ s&
      s" No" s{ s" entiendes" s" comprendes" }s&
  }s{
    s" qué" s{ s" pueden estar" s" están" }s& s" haciendo" s&
    s" qué" s{ s" los ha" s{ s" podría" s" puede" }s s" haberlos" s& }s&
      s{ s" reunido" s" traído" s" congregado" }s&
    s" cuál es" s{ s" el motivo" s" la razón" }s&
      s" de que se" s&{ s" encuentren" s" hallen" }s&
    s" por qué" s{ s" motivo" s" razón" }s?& s" se encuentran" s&
  }s& here$ s& period+  ;

: some-refugees-look-at-you$  ( -- ca len )
  s" Algunos" s" de ellos" s?&
  s" reparan en" s&{ s" ti" s" tu persona" s" tu presencia" }s&  ;

: in-their-eyes-and-gestures$  ( -- ca len )
  \ En sus ojos y gestos.
  s" En sus" s{ s" ojos" s" miradas" }s&
  s" y" s" en sus" s?& s" gestos" s&? s&  ;

: the-refugees-trust$  ( -- ca len )
  \ Los refugiados confían.
  some-refugees-look-at-you$ period+
  in-their-eyes-and-gestures$ s&
  s{ s" ves" s" notas" s" adviertes" s" aprecias" }s&
  s{
    s" amabilidad" s" confianza" s" tranquilidad"
    s" serenidad" s" afabilidad"
  }s&  ;

: you-feel-they-observe-you$  ( -- ca len )
  \ Sientes que te observan.
  s{ s" tienes la sensación de que" s" sientes que" }s?
  s" te observan" s& s" como" s?&
  s{  s" con timidez" s" tímidamente"
      s" de" way$ s& s" subrepticia" s& s" subrepticiamente"
      s" a escondidas"
  }s& period+  ;

: the-refugees-don't-trust$  ( -- ca len )
  \ Los refugiados no confían.
  some-refugees-look-at-you$ s{ s" . Entonces" s"  y" }s+
  you-feel-they-observe-you$ s&
  in-their-eyes-and-gestures$ s&
  s{
    s{ s" crees" s" te parece" }s to-realize$ s&
    s{ s" ves" s" notas" s" adviertes" s" aprecias" }s
    s" parece" to-realize$ s& s" se" s+
  }s& s{
    s" cierta" s?{
      s" preocupación" s" desconfianza" s" intranquilidad"
      s" indignación" }s&
    s" cierto" s?{ s" nerviosismo" s" temor" }s&
  }s&  ;

: diverse-people$  ( -- ca len )
  s{ s" personas" s" hombres, mujeres y niños" }s
  s" de toda" s& s" edad y" s?& s" condición" s&  ;

: refugees-description  ( -- )
  \ Descripición de los refugiados.
  talked-to-the-leader?
  if    s" Los refugiados son"
  else  s" Hay"
  then  diverse-people$ s&
  talked-to-the-leader?
  if    the-leader-said-they-want-peace$
  else  period+ you-don't-know-why-they're-here$
  then  s&
  do-you-hold-something-forbidden?
  if    the-refugees-don't-trust$
  else  the-refugees-trust$
  then  s& period+ narrate  ;

\ ----------------------------------------------
\ Tramos de cueva (laberinto)

\ Elementos básicos usados en las descripciones

: this-narrow-cave-pass$  ( -- ca len )
  \ Devuelve una variante de «estrecho tramo de cueva», con el artículo adecuado.
  my-location dup is-known?
  if  not-distant-article
  else  undefined-article
  then  narrow-cave-pass$ s&  ;

: ^this-narrow-cave-pass$  ( -- ca len )
  \ Devuelve una variante de «estrecho tramo de cueva», con el artículo adecuado y la primera letra mayúscula.
  this-narrow-cave-pass$ ^uppercase  ;

: toward-the(m/f)  ( a -- ca1 len1 )
  \ Devuelve una variante de «hacia el» con el artículo adecuado a un ente.
  has-feminine-name? if  toward-the(f)$  else  toward-the(m)$  then  ;

: toward-(the)-name  ( a -- ca1 len1 )
  \ Devuelve una variante de «hacia el nombre-de-ente» adecuada a un ente.
  dup has-no-article?
  if  s" hacia"
  else  dup toward-the(m/f)
  then  rot name s&  ;

: main-cave-exits-are$  ( -- ca len )
  \ Devuelve una variante del inicio de la descripción de los tramos de cueva
  ^this-narrow-cave-pass$ lets-you$ s& to-keep-going$ s&  ;

\ Variantes para la descripción de cada salida

: cave-exit-description-0$  ( -- ca len )
  \ Devuelve la primera variante de la descripción de una salida de un tramo de cueva.
  ^this-narrow-cave-pass$  lets-you$ s& to-keep-going$ s&
  in-that-direction$ s&  ;

: cave-exit-description-1$ ( -- ca1 len1 )
  \ Devuelve la segunda variante de la descripción de una salida de un tramo de cueva.
  ^a-pass-way$
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  in-that-direction$ s&  ;

\ Variantes para la descripción principal

false [if]  \ XXX OLD -- código obsoleto

: $two-main-exits-in-cave ( ca1 len1 ca2 len2 -- ca3 len3 )
  \ Devuelve la descripción de un tramo de cueva con dos salidas a dos puntos cardinales.
  \ XXX TODO -- not used
  \ Esta palabra solo sirve para parámetros de puntos cardinales (todos usan artículo determinado masculino)
  \ Se usa en la descripción principal de un escenario
  \ ca1 len1 = Nombre de una dirección cardinal (sin artículo)
  \ ca2 len2 = Nombre de la otra dirección cardinal (sin artículo)
  2>r 2>r
  this-narrow-cave-pass$ lets-you$ s& to-keep-going$ s&
  toward-the(m)$ 2dup 2r> s&  \ Una dirección
  2swap 2r> s&  \ La otra dirección
  both&  ;

: $other-exit-in-cave  ( ca1 len1 -- ca2 len2 )
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ XXX TODO -- not used
  \ Se usa en la descripción principal de un escenario
  \ Esta palabra solo sirve para parámetros de puntos cardinales (todos usan artículo determinado masculino)
  \ ca1 len1 = Nombre de la dirección cardinal
  ^a-pass-way$ s&
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  toward-the(m)$ s& 2swap s&  ;

[then]  \ XXX NOTE: fin del código obsoleto

: cave-exit-separator+  ( ca1 len1 -- ca2 len2 )
  \ Concatena (sin separación) a una cadena el separador entre las salidas principales y las secundarias.
  s{ s" ," s" ;" s" ..." }s+
  s{ s" y" 2dup s" aunque" but$ }s& s" también" s?&  ;

: (paths)-can-be-seen-0$  ( -- ca len )
  s{ s" parten" s" surgen" s" nacen" s" salen" }s
  s" de" s{ s" aquí" s" este lugar" }s& s? rnd2swap s&  ;

: (paths)-can-be-seen-1$  ( -- ca len )
  s{ s" se ven" s" pueden verse"
  s" se vislumbran" s" pueden vislumbrarse"
  s" se adivinan" s" pueden adivinarse"
  s" se intuyen" s" pueden intuirse" }s  ;

: (paths)-can-be-seen$  ( -- ca len )
  \ XXX TODO -- hacer que el texto dependa, por grupos, de si el escenario es conocido
  ['] (paths)-can-be-seen-0$
  ['] (paths)-can-be-seen-1$
  2 choose execute  ;

: paths-seen  ( ca1 len1 -- ca2 len2 )
  \ Devuelve la presentación de la lista de salidas secundarias.
  \ ca1 len1 = Cadena con el número de pasajes
  \ ca2 len2 = Cadena con el resultado
  pass-ways$ s& s" más" s?&
  (paths)-can-be-seen$ rnd2swap s&  ;

: secondary-exit-in-cave&  ( a1 ca2 len2 -- ca3 len3 )
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ a1 = Ente dirección cuya descripción hay que añadir
  \ ca2 len2 = Descripción en curso
  rot toward-(the)-name s&  ;

: one-secondary-exit-in-cave  ( a1 -- ca2 len2 )
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ a1 = Ente dirección
  a-pass-way$
  s{ s" parte" s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  secondary-exit-in-cave&  ;

: two-secondary-exits-in-cave  ( a1 a2 -- ca3 len3 )
  \ Devuelve la descripción de dos salidas adicionales en un tramo de cueva.
  s" dos" paths-seen s" :" s?+
  secondary-exit-in-cave& s" y" s&
  secondary-exit-in-cave&  ;

: three-secondary-exits-in-cave  ( a1 a2 a3 -- ca4 len4 )
  \ Devuelve la descripción de tres salidas adicionales en un tramo de cueva.
  s" tres" paths-seen s" :" s?+
  secondary-exit-in-cave& comma+
  secondary-exit-in-cave& s" y" s&
  secondary-exit-in-cave&  ;

: two-main-exits-in-cave ( a1 a2 -- ca3 len3 )
  \ Devuelve la descripción de dos salidas principales en un tramo de cueva.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  toward-(the)-name rot toward-(the)-name both  ;

: one-main-exit-in-cave  ( a1 -- ca2 len2 )
  \ Devuelve la descripción de una salida principal en un tramo de cueva.
  \ a1 = Ente dirección
  toward-(the)-name  ;

\ Descripciones de los tramos de cueva según el reparto entre salidas principales y secundarias

: 1+1-cave-exits  ( a1 a2 -- ca len )
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y una secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  one-main-exit-in-cave cave-exit-separator+
  rot one-secondary-exit-in-cave s&  ;

: 1+2-cave-exits  ( a1 a2 a3 -- ca len )
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y dos secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  one-main-exit-in-cave cave-exit-separator+
  2swap two-secondary-exits-in-cave s&  ;

: 1+3-cave-exits  ( a1 a2 a3 a4 -- ca len )
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y tres secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección
  one-main-exit-in-cave cave-exit-separator+
  2>r three-secondary-exits-in-cave 2r> 2swap s&  ;

: 2+0-cave-exits  ( a1 a2 -- ca len )
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y ninguna secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  two-main-exits-in-cave  ;

: 2+1-cave-exits  ( a1 a2 a3 -- ca len )
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y ninguna secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  two-main-exits-in-cave cave-exit-separator+
  rot one-secondary-exit-in-cave s&  ;

: 2+2-cave-exits  ( a1 a2 a3 a4 -- ca len )
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y dos secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección
  two-main-exits-in-cave cave-exit-separator+
  2swap two-secondary-exits-in-cave s&  ;

\ Descripciones de los tramos de cueva según su número de salidas

: 1-exit-cave-description   ( a1 -- ca len )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene una salida.
  \ a1 = Ente dirección
  toward-(the)-name  ;

: 2-exit-cave-description   ( a1 a2 -- ca len )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene dos salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  ['] 2+0-cave-exits
  ['] 1+1-cave-exits
  2 choose execute  ;

: 3-exit-cave-description   ( a1 a2 a3 -- ca len )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene tres salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  ['] 2+1-cave-exits
  ['] 1+2-cave-exits
  2 choose execute  ;

: 4-exit-cave-description   ( a1 a2 a3 a4 -- ca len )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene cuatro salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección
  ['] 2+2-cave-exits
  ['] 1+3-cave-exits
  2 choose execute  ;

\ Tabla para contener las direcciones de las palabras de descripción:
create 'cave-descriptions
' 1-exit-cave-description ,
' 2-exit-cave-description ,
' 3-exit-cave-description ,
' 4-exit-cave-description ,

\ Interfaz para usar en las descripciones de los escenarios:
\ `exits-cave-description` para la descripción principal
\ `cave-exit-description$` para la descripción de cada salida

: unsort-cave-exits  ( a1 ... an u -- a1'..an' u )
  \ Desordena los entes dirección que son las salidas de la cueva.
  \ u = Número de elementos de la pila que hay que desordenar
  dup >r unsort r>  ;

: (exits-cave-description)  ( a1 ... an u -- ca2 len2 )
  \ Ejecuta (según el número de salidas) la palabra  que devuelve la descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados
  1- cells 'cave-descriptions + perform  ;

: exits-cave-description  ( a1 ... an u -- ca2 len2 )
  \ Devuelve la descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados
  unsort-cave-exits  (exits-cave-description) period+
  main-cave-exits-are$ 2swap s&  ;  \ Añadir el encabezado

: cave-exit-description$  ( -- ca1 len1 )
  \ Devuelve la descripción de una dirección de salida de un tramo de cueva.
  ['] cave-exit-description-0$  \ Primera variante posible
  ['] cave-exit-description-1$  \ Segunda variante posible
  2 choose execute period+  ;

\ ----------------------------------------------
\ La aldea sajona

: poor$  ( -- ca len )
  s{ s" desgraciada" s" desdichada" }s  ;

: poor-village$  ( -- ca len )
  poor$ s" aldea" s&  ;

: rests-of-the-village$  ( -- ca len )
  \ Devuelve parte de otras descripciones de la aldea arrasada.
  s" los restos" still$ s? s" humeantes" s& s?&
  s" de la" s& poor-village$ s&  ;

\ ----------------------------------------------
\ Pared del desfiladero

: to-pass-(wall)$  ( -- ca len )
  \ Infinitivo aplicable para atravesar una pared o un muro.
  s{ s" superar" s" atravesar" s" franquear" s" vencer" s" escalar" }s  ;

: it-looks-impassable$  ( -- ca len )
  \ Mensaje «parece infranqueable».
  \ XXX TODO -- ojo: género femenino; generalizar factorizando cuando se use en otro contexto
  s{ s" por su aspecto" s" a primera vista" }s?
  s{  s" parece"
      s" diríase que es"
      s" bien" s? s" puede decirse que es" s&
  }s&
  s{  s" imposible de" to-pass-(wall)$ s&
      s" imposible" to-pass-(wall)$ s& s" la" s+
      s{ s" insuperable" s" invencible" s" infranqueable" }s
  }s&  ;

: the-cave-entrance-is-visible$  ( -- ca len )
  s{  s" se" s{ s" ve" s" abre" s" haya"
                s" encuentra" s" aprecia" s" distingue" }s&
      s" puede" s{ s" verse" s" apreciarse" s" distinguirse" }s&
  }s cave-entrance% full-name s&  ;

\ ----------------------------------------------
\ Entrada a la cueva

: the-cave-entrance-was-discovered?  ( -- )
  \ ¿La entrada a la cueva ya fue descubierta?
  location-08% has-south-exit?  ;

: the-cave-entrance-is-accessible?  ( -- )
  \ ¿La entrada a la cueva está accesible (presente y descubierta)?
  location-08% am-i-there? the-cave-entrance-was-discovered? and  ;

: open-the-cave-entrance  ( -- )
  \ Comunica el escenario 8 con el 10 (de dos formas y en ambos sentidos).
  location-08% dup location-10% s<-->  location-10% i<-->  ;

: you-discover-the-cave-entrance$  ( -- ca len )
  \ Mensaje de que descubres la cueva.
  ^but$ comma+
  s{  s" reconociendo" s" el terreno" more-carefully$ rnd2swap s& s&
      s" fijándote" s" ahora" s?& more-carefully$ s&
  }s& comma+ s" descubres" s& s{ s" lo" s" algo" }s& s" que" s&
  s" sin duda" s?& s{ s" parece ser" s" es" s" debe de ser" }s&
  s{ s" la entrada" s" el acceso" }s& s" a una" s& cave$ s&  ;

: you-discover-the-cave-entrance  ( -- )
  \ Descubres la cueva.
  you-discover-the-cave-entrance$ period+ narrate
  open-the-cave-entrance
  cave-entrance% is-here  ;

: you-maybe-discover-the-cave-entrance  ( ca len -- )
  \ Descubres la cueva con un 50% de probabilidad.
  \ ca len = Texto introductorio
  s" ..." s+ narrate
  2 random 0= if  narration-break you-discover-the-cave-entrance  then  ;

: the-cave-entrance-is-hidden$  ( -- ca len )
  s" La entrada" s" a la cueva" s?&
  s{ s" está" s" muy" s?& s" no podría estar más" }s&
  s{ s" oculta" s" camuflada" s" escondida" }s&
  s" en la pared" s& rocky(f)$ s& period+  ;

: you-were-lucky-discovering-it$ ( -- ca len )
  s" Has tenido" s" muy" s?& s" buena" s&{ s" fortuna" s" suerte" }s&
  s{  s{ s" al" s" en" s" con" }s
        s{ s" hallarla" s" encontrarla" s" dar con ella" s" descubrirla" }s&
      s{  s" hallándola" s" encontrándola"
          s" dando con ella" s" descubriéndola"
      }s
  }s& period+  ;

: it's-your-last-hope$  ( -- ca len )
  s{ s" te das cuenta de que" s" sabes que" }s?
  s{ s" es" s" se trata de" }s& ^uppercase
  your|the(f)$ s& s{ s" única" s" última" }s&
  s{ s" salida" s" opción" s" esperanza" s" posibilidad" }s&
  s{ s" de" s" para" }s
  s{  s" escapar" s" huir"
      s" evitar" s{ s" ser capturado" s" que te capturen" }s&
  }s&? s& period+  ;

\ ----------------------------------------------
\ Entrada a la cueva

\ ----------------------------------------------
\ Otros escenarios

: bridge-that-way$  ( -- ca len )
  s" El puente" leads$ s& that-way$ s& period+  ;

: stairway-that-way$  ( -- ca len )
  s" Las escaleras" (they)-lead$ s& that-way$ s& period+  ;

: comes-from-there$  ( -- ca len )
  comes-from$ from-that-way$ s&  ;

: water-from-there$  ( -- ca len )
  the-water-current$ comes-from-there$ s&  ;

: ^water-from-there$  ( -- ca len )
  ^the-water-current$ comes-from-there$ s&  ;

: water-that-way$  ( -- ca len )
  ^the-water-current$ s{ s" corre" s" fluye" s" va" }s&
  in-that-direction$ s& period+  ;

: stairway-to-river$  ( -- ca len )
  s" Las escaleras" (they)-go-down$ s&
  that-way$ s& comma+
  s{ s" casi" s? s" hasta el" s& s" mismo" s?& s" borde del" s&
  s" casi" s? s" hasta la" s& s" misma" s?& s" orilla del" s&
  s" casi" s? s" hasta el" s&
  s" hasta" s? s" cerca del" s& }s& s" agua." s&  ;

: a-high-narrow-pass-way$  ( -- ca len )
  s" un" narrow(m)$ s& pass-way$ s& s" elevado" s&  ;

\ }}} ==========================================================
section( Atributos y descripciones de entes)  \ {{{

\ Ente protagonista

\ cr .( antes de ulfius) .s \ XXX INFORMER
ulfius% :attributes
  s" Ulfius" self% ms-name!
  self% is-human
  self% has-personal-name
  self% has-no-article
  \ location-01% self% is-there
  ;attributes
ulfius% :description
  \ XXX TMP
  s" [descripción de Ulfius]"
  paragraph
  ;description

\ Entes personaje

ambrosio% :attributes
  s" hombre" self% ms-name!  \ El nombre cambiará a «Ambrosio» durante el juego
  self% is-character
  self% is-human
  location-19% self% is-there
  ;attributes
ambrosio% :description
  self% conversations if
    s" Ambrosio"
    s" es un hombre de mediana edad, que te mira afable." s&
  else  s" Es de mediana edad y mirada afable."
  then  paragraph
  ;description
leader% :attributes
  s" anciano" self% ms-name!
  self% is-character
  self% is-human
  self% is-not-listed
  location-28% self% is-there
  ;attributes
leader% :description
  \ XXX TODO -- elaborar esto según la trama
  leader% conversations?
  if
    s" Es el jefe de los refugiados."
  else
    s" Es un anciano."
  then
  paragraph
  ;description
soldiers% :attributes
  s" soldados" self% mp-name!
  self% is-human
  self% familiar++
  self% is-decoration
  \ self% has-definite-article  \ XXX TODO -- mejor implementar que tenga posesivo...
  self% belongs-to-protagonist  \ XXX TODO -- ...aunque quizá esto baste
  ;attributes
defer soldiers-description  \ Vector a la futura descripción
soldiers% :description
  \ La descripción de los soldados
  \ necesita usar palabras que aún no están definidas,
  \ y por ello es mejor crearla después.
  soldiers-description
  ;description
officers% :attributes
  s" oficiales" self% mp-name!
  self% is-human
  self% familiar++
  self% is-decoration
  \ self% has-definite-article  \ XXX TODO -- mejor implementar que tenga posesivo...
  self% belongs-to-protagonist  \ XXX TODO -- ...aunque quizá esto baste
  ;attributes
defer officers-description  \ Vector a la futura descripción
officers% :description
  \ La descripción de los oficiales
  \ necesita usar palabras que aún no están definidas,
  \ y por ello es mejor crearla después.
  officers-description
  ;description
refugees% :attributes
  s" refugiados" self% mp-name!
  self% is-human
  self% is-decoration
  ;attributes
refugees% :description
  my-location case
  location-28% of  refugees-description  endof
  location-29% of
    \ XXX TODO -- provisional
    s" Todos los refugiados quedaron atrás." paragraph
    endof
  endcase
  ;description

\ Entes objeto

altar% :attributes
  s" altar" self% ms-name!
  self% is-decoration
  impossible-error# self% ~take-error# !
  location-18% self% is-there
  ;attributes
altar% :description
  s" Está" s{ s" situado" s" colocado" }s&
  s" justo en la mitad del puente." s&
  idol% is-unknown? if
    s" Debe de sostener algo importante." s&
  then
  paragraph
  ;description
arch% :attributes
  s" arco" self% ms-name!
  self% is-decoration
  location-18% self% is-there
  ;attributes
arch% :description
  \ XXX TMP
  s" Un sólido arco de piedra, de una sola pieza."
  paragraph
  ;description
bed% :attributes
  s" catre" self% ms-name!
  location-46% self% is-there
  self% ambrosio% belongs
  ;attributes
bed% :description
  s{ s" Parece poco" s" No tiene el aspecto de ser muy"
  s" No parece especialmente" }s
  s{ s" confortable" s" cómod" self% adjective-ending s+ }s& period+
  paragraph
  ;description
bridge% :attributes
  s" puente" self% ms-name!
  self% is-decoration
  location-13% self% is-there
  ;attributes
bridge% :description
  \ XXX TMP
  s" Está semipodrido."
  paragraph
  ;description
candles% :attributes
  s" velas" self% fp-name!
  location-46% self% is-there
  self% ambrosio% belongs
  ;attributes
candles% :description
  s" Están muy consumidas."
  paragraph
  ;description
cave-entrance% :attributes
  s" entrada a una cueva" self% fs-name!
  ;attributes
cave-entrance% :description
  the-cave-entrance-is-hidden$
  you-were-lucky-discovering-it$ s&
  it's-your-last-hope$ s&
  paragraph
  ;description
cloak% :attributes
  s" capa" self% fs-name!
  self% is-cloth
  self% belongs-to-protagonist
  self% is-worn
  self% taken
  ;attributes
cloak% :description
  s" Tu capa de general, de fina lana"
  s{ s" tintada de negro." s" negra." }s&
  paragraph
  ;description
cuirasse% :attributes
  s" coraza" self% fs-name!
  self% is-cloth
  self% belongs-to-protagonist
  self% is-worn
  self% taken
  ;attributes
door% :attributes
  s" puerta" self% fs-name!
  self% is-closed
  impossible-error# self% ~take-error# !
  location-47% self% is-there
  self% ambrosio% belongs
  ;attributes
door% :description
  self% times-open if  s" Es"  else  s" Parece"  then
  s" muy" s?& s{ s" recia" s" gruesa" s" fuerte" }s&
  location-47% am-i-there? if
    lock% is-known?
    if    s" . A ella está unido el candado"
    else  s"  y tiene un gran candado"
    then  s+ lock-found
  then  period+
  s" Está" s& door% «open»|«closed» s& period+ paragraph
  ;description
emerald% :attributes
  s" esmeralda" self% fs-name!
  location-39% self% is-there
  ;attributes
emerald% :description
  s" Es preciosa."
  paragraph
  ;description
fallen-away% :attributes
  s" derrumbe" self% ms-name!
  self% is-decoration
  nonsense-error# self% ~take-error# !
  location-09% self% is-there
  ;attributes
fallen-away% :description
  s{
    s" Muchas," s" Muchísimas," s" Numerosas,"
    s" Un gran número de" s" Una gran cantidad de"
    \ XXX TODO -- si se añade lo que sigue, hay que crear los entes "pared" y "muro":
    \ s" Un muro de" s" Una pared de"
  }s
  s{ s" inalcanzables" s" inaccesibles" }s&
  s{ s" y enormes" s" y pesadas" s" y grandes" }s?&
  s" rocas," s& s{ s" apiladas" s" amontonadas" }s&
  s{
    s" una sobre otra"
    s" unas sobre otras"
    s" una encima de otra"
    s" unas encima de otras"
    s" la una encima de la otra"
    s" las unas encima de las otras"
    s" la una sobre la otra"
    s" las unas sobre las otras"
  }s& period+
  paragraph
  ;description
: don't-take-the-flags  ( -- )
  \ XXX TODO
  s" [Yo no lo haría]." narrate  ;

flags% :attributes
  s" banderas" self% fp-name!
  self% is-decoration
  ['] don't-take-the-flags self% ~take-error# !
  location-28% self% is-there
  ;attributes
flags% :description
  s" Son las banderas britana y sajona."
  s" Dos dragones rampantes, rojo y blanco respectivamente, enfrentados." s&
  paragraph
  ;description
flint% :attributes
  s" pedernal" self% ms-name!
  ;attributes
flint% :description
  s" Es dura y afilada." \ XXX FIXME
  paragraph
  ;description
grass% :attributes
  s" hierba" self% fs-name!
  self% is-decoration
  ;attributes
grass% :description
  door% times-open if
    s" Está" self% verb-number-ending+
    s" aplastad" self% adjective-ending+ s&
    s{ s" en el" s" bajo el" s" a lo largo del" }s&
    s{ s" trazado" s" recorrido" }s&
    s{ s" de la puerta." s" que hizo la puerta al abrirse." }s&
  else
    s" Cubre" self% verb-number-ending+
    s" el suelo junto a la puerta, lo" s&
    s{ s" que" s" cual" }s&
    s{ s" indica" s" significa" s" delata" }s
    s" que ésta" s&
    s{ s" no ha sido abierta en" s" lleva cerrada"
    s" ha permanecido cerrada" s" durante" s?& }s
    s" mucho tiempo." s&
  then  paragraph
  ;description
idol% :attributes
  s" ídolo" self% ms-name!
  self% is-decoration
  impossible-error# self% ~take-error# !
  location-41% self% is-there
  ;attributes
idol% :description
  s" El ídolo tiene dos agujeros por ojos."
  paragraph
  ;description
key% :attributes
  s" llave" self% fs-name!
  location-46% self% is-there
  self% ambrosio% belongs
  ;attributes
key% :description
  \ XXX TODO -- crear ente. hierro, herrumbre y óxido, visibles con la llave en la mano
  s" Grande, de hierro herrumboso."
  paragraph
  ;description
lake% :attributes
  s" lago" self% ms-name!
  self% is-decoration
  nonsense-error# self% ~take-error# !
  location-44% self% is-there
  ;attributes
lake% :description
  s{ s" La" s" Un rayo de" }s
  s" luz entra por un resquicio, y sus caprichosos reflejos te maravillan." s&
  paragraph
  ;description
lock% :attributes
  s" candado" self% ms-name!
  self% is-decoration
  self% is-closed
  impossible-error# self% ~take-error# !
  self% ambrosio% belongs
  ;attributes
lock% :description
  s" Es grande y parece" s{ s" fuerte." s" resistente." }s&
  s" Está" s&{ s" fijad" s" unid" }s& self% adjective-ending+
  s" a la puerta y" s&
  lock% «open»|«closed» s& period+
  paragraph
  ;description
log% :attributes
  s" tronco" self% ms-name!
  location-15% self% is-there
  ;attributes
log% :description
  s" Es un tronco"
  s{ s" recio," s" resistente," s" fuerte," }s&
  but$ s& s{ s" liviano." s" ligero." }s&
  paragraph
  ;description
piece% :attributes
  s" trozo de tela" self% ms-name!
  \ XXX NOTE: ojo con este «de tela»: «tela» es sinónimo de trozo;
  \ hay que contemplar estos casos en el cálculo de los genitivos.
  ;attributes
piece% :description
  s" Un pequeño" s{ s" retal" s" pedazo" s" trozo" s" resto" }s&
  of-your-ex-cloak$ s&
  paragraph
  ;description
rags% :attributes
  s" harapo" self% ms-name!
  ;attributes
rags% :description
  s" Un" s{ s" retal" s" pedazo" s" trozo" }s&
  s{ s" un poco" s" algo" }s?& s" grande" s&
  of-your-ex-cloak$ s&
  paragraph
  ;description
ravine-wall% :attributes
  s" pared" rocky(f)$ s& self% fs-name!
  location-08% self% is-there
  self% is-not-listed  \ XXX OLD -- innecesario
  self% is-decoration
  ;attributes
ravine-wall% :description
  s" en" the-cave-entrance-was-discovered? ?keep
  s" la pared" s& rocky(f)$ s& ^uppercase
  the-cave-entrance-was-discovered? if
    s" , que" it-looks-impassable$ s& comma+ s?+
    the-cave-entrance-is-visible$ s&
    period+ paragraph
  else
    it-looks-impassable$ s&
    ravine-wall% is-known?
    if    you-maybe-discover-the-cave-entrance
    else  period+ paragraph
    then
  then
  ;description
rocks% :attributes
  s" rocas" self% fp-name!
  self% is-decoration
  location-31% self% is-there
  ;attributes
rocks% :description
  location-31% has-north-exit?
  if  (rocks)-on-the-floor$ ^uppercase
  else  (rocks)-clue$
  then  period+ paragraph
  ;description
snake% :attributes
  s" serpiente" self% fs-name!
  self% is-animal
  dangerous-error# self% ~take-error# !
  location-43% self% is-there
  ;attributes
snake% :description
  \ XXX TODO -- distinguir si está muerta
  \ XXX NOTE: en el programa original no hace falta
  s" Una serpiente muy maja."
  paragraph
  ;description
stone% :attributes
  s" piedra" self% fs-name!
  location-18% self% is-there
  ;attributes
stone% :description
  s" Recia y pesada, pero no muy grande, de forma piramidal."
  paragraph
  ;description
sword% :attributes
  s" espada" self% fs-name!
  self% belongs-to-protagonist
  self% taken
  ;attributes
sword% :description
  s{ s" Legado" s" Herencia" }s s" de tu padre," s&
  s{ s" fiel herramienta" s" arma fiel" }s& s" en" s&
  s{ s" mil" s" incontables" s" innumerables" }s&
  s" batallas." s&
  paragraph
  ;description
table% :attributes
  s" mesa" self% fs-name!
  location-46% self% is-there
  self% ambrosio% belongs
  ;attributes
table% :description
  s" Es pequeña y de" s{ s" basta" s" tosca" }s& s" madera." s&
  paragraph
  ;description
thread% :attributes
  s" hilo" self% ms-name!
  ;attributes
thread% :description
  \ XXX TODO -- mover esto al evento de cortar la capa
  \ s" Un hilo se ha desprendido al cortar la capa con la espada."
  s" Un hilo" of-your-ex-cloak$ s&
  paragraph
  ;description
torch% :attributes
  s" antorcha" self% fs-name!
  self% is-light
  self% is-not-lit
  ;attributes
torch% :description
  \ XXX TODO -- unfinished
  s" Está apagada."
  paragraph
  ;description
waterfall% :attributes
  s" cascada" self% fs-name!
  self% is-decoration
  nonsense-error# self% ~take-error# !
  location-38% self% is-there
  ;attributes
waterfall% :description
  s" No ves nada por la cortina de agua."
  s" El lago es muy poco profundo." s&
  paragraph
  ;description

\ Entes escenario

\ Las palabras que describen entes escenario reciben en `sight`
\ (variable que está creada con `value` y por tanto devuelve su
\ valor como si fuera una constante) un identificador de ente.
\ Puede ser el mismo ente escenario o un ente de dirección.  Esto
\ permite describir lo que hay más allá de cada escenario en
\ cualquier dirección.

location-01% :attributes
  s" aldea sajona" self% fs-name!
  0 location-02% 0 0 0 0 0 0 self% init-location
  ;attributes
location-01% :description
  \ XXX TODO -- crear colina en los tres escenarios
  sight case
  self% of
    s" No ha quedado nada en pie, ni piedra sobre piedra."
    s{ s" El entorno es desolador." s" Todo alrededor es desolación." }s
    rnd2swap s&
    s{ ^only$ remains$ s&
    s" Lo único que" remains$ s& s" por hacer" s?& s" es" s&
    s" No" remains$ s& s{ s" más" s" otra cosa" }s& s" que" s&
    }s& to-go-back$ s& s" al sur, a casa." s&
    paragraph
    endof
  south% of
    2 random if \ Versión 0:
      ^toward-the(m)$ s" sur" s&
      s{ s" está" s" puedo ver" s" se puede ver" }s&
      s" la colina." s&  \ Descripción principal
      s" Y mucho más allá está tu" home$ s& period+  \ Coletilla...
      2 random * s&  \ ...que aparecerá con un 50% de probabilidad
    else  \ Versión 1:
      s" Muy lejos de aquí está tu" home$ s& comma+
      s" y el camino empieza detrás de aquella colina." s&
    then  paragraph
    endof
  up% of
    s{ s" pronto" s" sin compasión" s" de inmediato" }s
    s{ s" vencidas" s" derrotadas" s" sojuzgadas" }s rnd2swap s& ^uppercase
    s" por la fría" s&
    s{ s" e implacable" s" y despiadada" }s?&
    s" niebla," s& s" torpes" s" tristes" both?&
    s" columnas de" s& s" negro" s" humo" rnd2swap s& s&
    (they)-go-up$ s&
    s{ s" lastimosamente" s" penosamente" }s&
    s" hacia" s{ s" el cielo" s" las alturas" }s& s?&
    s{ s" desde" s" de entre" }s& rests-of-the-village$ s&
    s" , como si" s" también" s" ellas" rnd2swap s& s?&
    s{ s" desearan" s" anhelaran" s" soñaran" }s&
    s" poder hacer un último esfuerzo por" s?&
    s" escapar" s& but|and$ s& s" no supieran cómo" s& s?+
    s" ..." s+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-02% :attributes
  s" cima de la colina" self% fs-name!
  location-01% 0 0 location-03% 0 0 0 0 self% init-location
  ;attributes
location-02% :description
  \ XXX TODO -- crear ente. aldea, niebla
  sight case
  self% of
    s" Sobre" s" la cima de" s?&
    s" la colina, casi" s& s{ s" sobre" s" por encima de" }s&
    s" la" s&
    s" espesa" s" fría" both?& s" niebla de la aldea sajona arrasada al norte, a tus pies." s&
    ^the-path$ s& goes-down$ s& toward-the(m)$ s& s" oeste." s&
    paragraph
    endof
  north% of
    s" La" poor-village$ s& s" sajona" s& s" , arrasada," s?+ s" agoniza bajo la" s&
    s" espesa" s" fría" both?& s" niebla." s&
    paragraph
    endof
  west% of
    ^the-path$ goes-down$ s& s" por la" s& s" ladera de la" s?& s" colina." s&
    paragraph
    endof
  down% of
    \ Bajar la colina
    \ puede equivaler a bajar por el sur o por el oeste; esto
    \ se decide al azar cada vez que se
    \ entra en el escenario, por lo que su descripción
    \ debe tenerlo en cuenta y redirigir a la descripción adecuada:
    self% down-exit self% north-exit =
    if  north%  else  west%  then  describe
    endof
  uninteresting-direction
  endcase
  ;description
location-03% :attributes
  s" camino entre colinas" self% ms-name!
  0 0 location-02% location-04% 0 0 0 0 self% init-location
  ;attributes
location-03% :description
  sight case
  self% of
    ^the-path$ s" avanza por el valle," s&
    s" desde la parte alta, al este," s&
    s" a una zona" s& very-$ s& s" boscosa, al oeste." s&
    paragraph
    endof
  east% of
    ^the-path$ s" se pierde en la parte alta del valle." s&
    paragraph
    endof
  west% of
    s" Una zona" very-$ s& s" boscosa." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-04% :attributes
  s" cruce de caminos" self% ms-name!
  location-05% 0 location-03% location-09% 0 0 0 0 self% init-location
  ;attributes
location-04% :description
  sight case
  self% of
    s" Una senda parte al oeste, a la sierra por el paso del Perro,"
    s" y otra hacia el norte, por un frondoso bosque que la rodea." s&
    paragraph
    endof
  north% of
    ^a-path$ surrounds$ s& s" la sierra a través de un frondoso bosque." s&
    paragraph
    endof
  west% of
    ^a-path$ leads$ s& toward-the(f)$ s& s" sierra por el paso del Perro." s&
    paragraph
    endof
  down% of  endof
  up% of  endof
  uninteresting-direction
  endcase
  ;description
location-05% :attributes
  s" linde del bosque" self% fs-name!
  0 location-04% 0 location-06% 0 0 0 0 self% init-location
  ;attributes
location-05% :description
  sight case
  self% of
    ^toward-the(m)$ s" oeste se extiende" s&
    s{ s" frondoso" s" exhuberante" }s& \ XXX TODO -- independizar
    s" el bosque que rodea la sierra." s&
    s" La salida se abre" s&
    toward-the(m)$ s& s" sur." s&
    paragraph
    endof
  south% of
    s" Se ve la salida del bosque."
    paragraph
    endof
  west% of
    s" El bosque se extiende"
    s{ s" exhuberante" s" frondoso" }s&
    s" alrededor de la sierra." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-06% :attributes
  s" bosque" self% ms-name!
  0 0 location-05% location-07% 0 0 0 0 self% init-location
  ;attributes
location-06% :description
  sight case
  self% of
    s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
    ^the-path$ s& s" serpentea entre raíces, de un luminoso este" s&
    toward-the(m)$ s& s" oeste." s&
    paragraph
    endof
  east% of
    s" De la linde del bosque"
    s{ s" procede" s" llega" s" viene" }s&
    s{ s" una cierta" s" algo de" s" un poco de" }s&
    s{ s" claridad" s" luminosidad" }s&
    s" entre" s&
    s{ s" el follaje" s" la vegetación" }s& period+
    paragraph
    endof
  west% of
    s" La niebla parece más" s" densa" s" oscura" both?& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-07% :attributes
  s" paso del Perro" self% ms-name!
  0 location-08% location-06% 0 0 0 0 0 self% init-location
  ;attributes
location-07% :description
  sight case
  self% of
    s" Abruptamente, el bosque desaparece y deja paso a un estrecho camino entre altas rocas."
    s" El" s& s{ s" inquietante" s" sobrecogedor" }s&
    s" desfiladero" s& s{ s" tuerce" s" gira" }s&
    s" de este a sur." s&
    paragraph
    endof
  south% of
    ^the-path$ s" gira" s& in-that-direction$ s& period+
    paragraph
    endof
  east% of
    s" La estrecha senda es" s{ s" engullida" s" tragada" }s&
    s" por las" s&
    s" fauces" s{ s" frondosas" s" exhuberantes" }s rnd2swap s& s&
    s" del bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-08% :attributes
  s" desfiladero" self% ms-name!
  location-07% 0 0 0 0 0 0 0 self% init-location
  ;attributes
location-08% :description
  \ XXX TODO -- crear pared y roca y desfiladero
  sight case
  self% of
    ^the-pass-way$ s" entre el desfiladero sigue de norte a este" s&
    s" junto a una" s&
    s{  s" pared" rocky(f)$ s& s" rocosa pared" }s& period+
    \ XXX TODO -- completar con entrada a caverna, tras ser descubierta
    paragraph
    endof
  north% of
    s" El camino" s{ s" tuerce" s" gira" }s& \ XXX TODO -- independizar gira/tuerce
    s" hacia el inquietante paso del Perro." s&
    paragraph
    endof
  south% of
    s{ ^in-that-direction$ s" Hacia el sur" }s
    s{ s" se alza" s" se levanta" }s&
    \ s" una pared" s& rocky(f)$ s& \ XXX OLD -- antiguo
    ravine-wall% full-name s&
    the-cave-entrance-was-discovered? if
      comma+ s" en la" s&{ s" que" s" cual" }s&
      the-cave-entrance-is-visible$ s&
      period+ paragraph
    else
      ravine-wall% is-known? if
        s" que" it-looks-impassable$ s& s?&
        you-maybe-discover-the-cave-entrance
      else
        period+ paragraph  ravine-wall% familiar++
      then
    then
    endof
  uninteresting-direction
  endcase
  ;description
location-09% :attributes
  s" derrumbe" self% ms-name!
  0 0 location-04% 0 0 0 0 0 self% init-location
  ;attributes
location-09% :description
  sight case
  self% of
    ^the-path$ goes-down$ s& s" hacia la agreste sierra, al oeste," s&
    s" desde los" s& s" verdes" s" valles" rnd2swap s& s& s" al este." s&
    ^but$ s& s" un" s&{ s" gran" s" enorme" }s?& s" derrumbe" s&
    (it)-blocks$ s& s" el paso hacia" s&{ s" el oeste" s" la sierra." }s&
    paragraph
    endof
  east% of
    ^can-see$ s" la salida del bosque." s&
    paragraph
    endof
  west% of
    s" Un gran derrumbe" (it)-blocks$ s& the-pass$ s&
    toward$ s& s" la sierra." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-10% :attributes
  s" gruta de entrada" self% fs-name!
  self% is-indoor-location
  location-08% 0 0 location-11% 0 0 0 0 self% init-location
  ;attributes
location-10% :description
  sight case
  self% of
    s" El estrecho paso se adentra hacia el oeste, desde la boca, al norte."
    paragraph
    endof
  north% of
    s" La boca de la gruta conduce al exterior."
    paragraph
    endof
  east% of
  endof
  uninteresting-direction
  endcase
  ;description
location-11% :attributes
  s" gran lago" self% ms-name!
  self% is-indoor-location
  0 0 location-10% 0 0 0 0 0 self% init-location
  ;attributes
location-11% :description
  \ XXX TODO -- crear ente. estancia y aguas
  sight case
  self% of
    s" Una" s{
      s{ s" gran" s" amplia" }s s" estancia" s&
      s" estancia" s" muy" s?& s{ s" grande" s" amplia" }s&
    }s& s" alberga un lago de" s&
    s{
      s" profundas" s" aguas" rnd2swap s& comma+ s" e" s?& s" iridiscentes" s&
      s" aguas tan profundas como iridiscentes,"
    }s&
    s{ s" gracias a" s" debido a" s" a causa de" s" por el efecto de" }s&
    s{
      s" la" s{ s" débil" s" tenue" }s?& s" luz" s&
        s{  s" que se filtra" s{ s" del" s" desde el" }s&
            s{ s" procendente" s" que procede" s" que entra" }s s" del" s&
        }s
      s" los" s{ s" débiles" s" tenues" }s?& s" rayos de luz" s&
        s{  s" que se filtran" s{ s" del" s" desde el" }s&
            s{ s" procendentes" s" que proceden" s" que entran" }s s" del" s&
        }s
    }s?& s" exterior." s&
    s" No hay" s&{ s" otra" s" más" }s& s" salida que el este." s&
    paragraph
    endof
  east% of
    s" De la entrada de la gruta"
    s{ s" procede" s" proviene" }s&
    s" la" s& s{ s" luz" s" luminosidad" s" claridad" }s
    s" que hace brillar" s&
    s{ s" el agua" s" las aguas" s" la superficie" }s&
    s" del lago." s&
    paragraph
  endof
  uninteresting-direction
  endcase
  ;description
location-12% :attributes
  s" salida del paso secreto" self% fs-name!
  self% is-indoor-location
  0 0 0 location-13% 0 0 0 0 self% init-location
  ;attributes
location-12% :description
  \ XXX TODO -- crear ente. agua aquí
  sight case
  self% of
    s" Una" s{ s" gran" s" amplia" }s&
    s" estancia se abre hacia el oeste," s&
    s" y se estrecha hasta" s& s{ s" morir" s" terminar" }s&
    s" , al este, en una" s+{ s" parte" s" zona" }s& s" de agua." s&
    paragraph
    endof
  east% of
    s{ s" La estancia" s" El lugar" }s
    s" se estrecha hasta " s&
    s{ s" morir" s" terminar" }s&
    s" en una" s&{ s" parte" s" zona" }s& s" de agua." s&
    paragraph
  endof
  west% of
    s" Se vislumbra la continuación de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-13% :attributes
  s" puente semipodrido" self% ms-name!
  self% is-indoor-location
  0 0 location-12% location-14% 0 0 0 0 self% init-location
  ;attributes
location-13% :description
  \ XXX TODO -- crear ente. canal, agua, lecho(~catre)
  sight case
  self% of
    s" La sala se abre en"
    s{ s" semioscuridad" s" penumbra" }s&
    s" a un puente cubierto de podredumbre" s&
    s" sobre el lecho de un canal, de este a oeste." s&
    paragraph
    endof
  east% of
    s" Se vislumbra el inicio de la cueva."
    paragraph
  endof
  west% of
    s" Se vislumbra un recodo de la cueva."
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-14% :attributes
  s" recodo de la cueva" self% ms-name!
  self% is-indoor-location
  0 location-15% location-13% 0 0 0 0 0 self% init-location
  ;attributes
location-14% :description
  sight case
  self% of
    s" La iridiscente cueva gira de este a sur."
    paragraph
    endof
  south% of
    you-glimpse-the-cave$ paragraph
    endof
  east% of
    you-glimpse-the-cave$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-15% :attributes
  s" pasaje arenoso" self% ms-name!
  self% is-indoor-location
  location-14% location-17% location-16% 0 0 0 0 0 self% init-location
  ;attributes
location-15% :description
  sight case
  self% of
    s" La gruta" goes-down$ s& s" de norte a sur" s&
    s" sobre un lecho arenoso." s&
    s" Al este, un agujero del que llega" s&
    s{ s" algo de luz." s" claridad." }s&
    paragraph
    endof
  north% of
    you-glimpse-the-cave$
    s" La cueva" goes-up$ s& in-that-direction$ s& period+
    paragraph
    endof
  south% of
    you-glimpse-the-cave$
    s" La cueva" goes-down$ s& in-that-direction$ s& period+
    paragraph
    endof
  east% of
    s{ s" La luz" s" Algo de luz" s" Algo de claridad" }s
    s" procede de esa dirección." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-16% :attributes
  s" pasaje del agua" self% ms-name!
  self% is-indoor-location
  0 0 0 location-15% 0 0 0 0 self% init-location
  ;attributes
location-16% :description
\ XXX TODO -- el examen del agua aquí debe dar más pistas
  sight case
  self% of
    s" Como un acueducto, el agua"
    goes-down$ s& s" con gran fuerza de norte a este," s&
    s" aunque la salida practicable es la del oeste." s&
    paragraph
    endof
  north% of
    s" El agua" goes-down$ s& s" con gran fuerza" s& from-that-way$ s& period+
    paragraph
    endof
  east% of
    s" El agua" goes-down$ s& s" con gran fuerza" s& that-way$ s& period+
    paragraph
    endof
  west% of
    s" Es la única salida." paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-17% :attributes
  s" estalactitas" self% fs-name!
  self% is-indoor-location
  location-15% location-20% location-18% 0 0 0 0 0 self% init-location
  ;attributes
location-17% :description
  \ XXX TODO -- crear ente. estalactitas
  sight case
  self% of
    s" Muchas estalactitas se agrupan encima de tu cabeza,"
    s" y se abren cual arco de entrada hacia el este y sur." s&
    paragraph
    endof
  north% of
    you-glimpse-the-cave$
    paragraph
    endof
  up% of
    s" Las estalactitas se agrupan encima de tu cabeza."
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-18% :attributes
  s" puente de piedra" self% ms-name!
  self% is-indoor-location
  0 0 location-19% location-17% 0 0 0 0 self% init-location
  ;attributes
location-18% :description
  \ XXX TODO -- crear ente. puente, arco
  sight case
  self% of
    s" Un arco de piedra se extiende,"
    s{ s" cual" s" como si fuera un" s" a manera de" }s&
    s" puente" s&
    s" que se" s{ s" elevara" s" alzara" }s& s?&
    s{ s" sobre" s" por encima de" }s&
    s" la oscuridad, de este a oeste." s&
    s{ s" Hacia" s" En" }s& s" su mitad" s&
    altar% is-known?
    if    s" está" s&
    else  s{ s" hay" s" es posible ver" s" puede verse" }s&
    then  altar% full-name s& period+ paragraph
    endof
  east% of
    s" El arco de piedra se extiende" that-way$ s& period+
    paragraph
    endof
  west% of
    s" El arco de piedra se extiende" that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-19% :attributes
  s" recodo arenoso del canal" self% ms-name!
  self% is-indoor-location
  0 0 0 location-18% 0 0 0 0 self% init-location
  ;attributes
location-19% :description
  sight case
  self% of
    \ XXX TODO -- hacer variaciones
    ^the-water-current$ comma+
    s" que discurre" s?&
    s" de norte a este," s& (it)-blocks$ s&
    s" el paso, excepto al oeste." s&
    s{ s" Al" s" Del" s" Hacia el" s" Proveniente del" s" Procedente del" }s&
    s" fondo" s& s{ s" se oye" s" se escucha" s" puede oírse" }s&
    s" un gran estruendo." s&
    paragraph
    endof
  north% of
    ^water-from-there$ period+ paragraph
    endof
  east% of
    water-that-way$ paragraph
    endof
  west% of
    s" Se puede" to-go-back$ s& toward-the(m)$ s& s" arco de piedra" s& in-that-direction$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-20% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  location-17% location-22% location-25% 0 0 0 0 0 self% init-location
  ;attributes
location-20% :description
  \ XXX TODO -- aplicar el filtro de la antorcha a todos los escenarios afectados, quizá en una capa superior
  \ sight no-torch? 0= abs *  case
  sight case
  self% of
    north% south% east% 3 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-21% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  0 location-27% location-23% location-20% 0 0 0 0 self% init-location
  ;attributes
location-21% :description
  sight case
  self% of
    east% west% south% 3 exits-cave-description paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-22% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  0 location-24% location-27% location-22% 0 0 0 0 self% init-location
  ;attributes
location-22% :description
  sight case
  self% of
    south% east% west% 3 exits-cave-description paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-23% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  0 location-25% 0 location-21% 0 0 0 0 self% init-location
  ;attributes
location-23% :description
  sight case
  self% of
    west% south% 2 exits-cave-description paragraph
    endof
  south% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-24% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  location-22% 0 location-26% 0 0 0 0 0 self% init-location
  ;attributes
location-24% :description
  sight case
  self% of
    east% north% 2 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-25% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  location-22% location-28% location-23% location-21% 0 0 0 0 self% init-location
  ;attributes
location-25% :description
  sight case
  self% of
    north% south% east% west% 4 exits-cave-description paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-26% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  location-26% 0 location-20% location-27% 0 0 0 0 self% init-location
  ;attributes
location-26% :description
  \ XXX TODO -- crear ente. pasaje/camino/senda tramo/cueva (en todos los tramos)
  sight case
  self% of
    north% east% west% 3 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-27% :attributes
  s" tramo de cueva" self% ms-name!
  self% is-indoor-location
  location-27% 0 0 location-25% 0 0 0 0 self% init-location
  ;attributes
location-27% :description
  sight case
  self% of
    north% east% west% 3 exits-cave-description paragraph
    endof
  north% of
    cave-exit-description$ paragraph
    endof
  east% of
    cave-exit-description$ paragraph
    endof
  west% of
    cave-exit-description$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-28% :attributes
  s" amplia estancia" self% fs-name!
  self% is-indoor-location
  location-26% 0 0 0 0 0 0 0 self% init-location
  ;attributes
location-28% :description
  sight case
  self% of
    \ XXX TODO -- crear ente. estancia(para todos),albergue y refugio (tras hablar con anciano)
    self% ^full-name s" se extiende de norte a este." s&
    leader% conversations?
    if  s" Hace de albergue para los refugiados."
    else  s" Está llen" self% gender-ending+ s" de gente." s&
    then  s&
    flags% is-known?
    if
      s" Hay" s&
      s{  s" una bandera de cada bando"
          s" banderas de" s{ s" ambos" s" los dos" }s& s" bandos" s&
          s{ s" dos banderas: una" s" una bandera" }s
              s{  s" britana y otra sajona" s" sajona y otra britana" }s&
      }s& period+
    else
      s" Hay" s& s{ s" dos" s" unas" }s& s" banderas." s&
    then
    paragraph
    endof
  north% of
    self% has-east-exit?
    if    s" Es por donde viniste."
    else  s" Es la única salida."
    then  paragraph
    endof
  east% of
    ^the-refugees$
    self% has-east-exit?
    if    they-let-you-pass$ s&
    else  they-don't-let-you-pass$ s&
    then  period+ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-29% :attributes
  s" espiral" self% fs-name!
  self% is-indoor-location
  0 0 0 location-28% 0 location-30% 0 0 self% init-location
  ;attributes
location-29% :description
  \ XXX TODO -- crear ente. escalera/espiral, refugiados
  sight case
  self% of
    s" Cual escalera de caracol gigante,"
    goes-down-into-the-deep$ comma+ s&
    s" dejando a los refugiados al oeste." s&
    paragraph
    endof
  west% of
    over-there$ s" están los refugiados." s&
    paragraph
    endof
  down% of
    s" La espiral" goes-down-into-the-deep$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-30% :attributes
  s" inicio de la espiral" self% ms-name!
  self% is-indoor-location
  0 0 location-31% 0 location-29% 0 0 0 self% init-location
  ;attributes
location-30% :description
  sight case
  self% of
    s" Se eleva en la penumbra."
    s" La" s& cave$ s& gets-narrower(f)$ s&
    s" ahora como para una sola persona, hacia el este." s&
    paragraph
    endof
  east% of
    s" La" cave$ s& gets-narrower(f)$ s& period+
    paragraph
    endof
  up% of
    s" La" cave$ s& s" se eleva en la penumbra." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-31% :attributes
  s" puerta norte" self% fs-name!
  self% is-indoor-location
  0 0 0 location-30% 0 0 0 0 self% init-location
  ;attributes
location-31% :description
  \ XXX TODO -- crear ente. arco, columnas, hueco/s(entre rocas)
  sight case
  self% of
    s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
    paragraph
    endof
  north% of
    s" Las rocas"  self% has-north-exit?
    if  (rocks)-on-the-floor$
    else  (they)-block$ the-pass$ s&
    then  s& period+ paragraph
    endof
  west% of
    ^that-way$ s" se encuentra el inicio de la espiral." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-32% :attributes
  s" precipicio" self% ms-name!
  self% is-indoor-location
  0 location-33% 0 location-31% 0 0 0 0 self% init-location
  ;attributes
location-32% :description
  \ XXX TODO -- crear ente. precipicio, abismo, cornisa, camino, roca/s
  sight case
  self% of
    s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
    s" El soporte de roca gira en forma de «U» de oeste a sur." s&
    paragraph
    endof
  south% of
    ^the-path$ s" gira" s& that-way$ s& period+
    paragraph
    endof
  west% of
    ^the-path$ s" gira" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-33% :attributes
  s" pasaje de salida" self% ms-name!
  self% is-indoor-location
  location-32% 0 location-34% 0 0 0 0 0 self% init-location
  ;attributes
location-33% :description
  \ XXX TODO -- crear ente. camino/paso/sendero
  sight case
  self% of
    s" El paso se va haciendo menos estrecho a medida que se avanza hacia el sur, para entonces comenzar hacia el este."
    paragraph
    endof
  north% of
    ^the-path$ s" se estrecha" s& that-way$ s& period+
    paragraph
    endof
  south% of
    ^the-path$ gets-wider$ s& that-way$ s&
    s" y entonces gira hacia el este." s&
    paragraph
    endof
  east% of
    ^the-path$ gets-wider$ s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-34% :attributes
  \ XXX TODO -- crear ente. gravilla
  s" pasaje de gravilla" self% ms-name!
  self% is-indoor-location
  location-35% 0 0 location-33% 0 0 0 0 self% init-location
  ;attributes
location-34% :description
  \ XXX TODO -- crear ente. camino/paso/sendero, guijarros, moho, roca, suelo..
  sight case
  self% of
    s" El paso" gets-wider$ s& s" de oeste a norte," s&
    s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
    paragraph
    endof
  north% of
    ^the-path$ gets-wider$ s& that-way$ s& period+
    paragraph
    endof
  west% of
    ^the-path$ s" se estrecha" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-35% :attributes
  s" puente sobre el acueducto" self% ms-name!
  self% is-indoor-location
  location-40% location-34% 0 location-36% 0 location-36% 0 0 self% init-location
  ;attributes
location-35% :description
  \ XXX TODO -- crear ente. escaleras, puente, río/curso/agua
  sight case
  self% of
    s" Un puente" s{ s" se tiende" s" cruza" }s& s" de norte a sur sobre el curso del agua." s&
    s" Unas resbaladizas escaleras" s& (they)-go-down$ s& s" hacia el oeste." s&
    paragraph
    endof
  north% of
    bridge-that-way$ paragraph
    endof
  south% of
    bridge-that-way$ paragraph
    endof
  west% of
    stairway-to-river$ paragraph
    endof
  down% of
    stairway-to-river$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-36% :attributes
  s" remanso" self% ms-name!
  self% is-indoor-location
  0 0 location-35% location-37% location-35% 0 0 0 self% init-location
  ;attributes
location-36% :description
  sight case
  self% of
    s" Una" s{ s" ruidosa" s" estruendosa" s" ensordecedora" }s&
    s" corriente" s& goes-down$ s&
    s{ s" con" s" siguiendo" }s& s" el" s& pass-way$ s&
    s" elevado desde el oeste, y forma un meandro arenoso." s&
    s" Unas escaleras" s& (they)-go-up$ s& toward-the(m)$ s& s" este." s&
    paragraph
    endof
  east% of
    stairway-that-way$ paragraph
    endof
  west% of
    ^water-from-there$ period+ paragraph
    endof
  up% of
    stairway-that-way$ paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-37% :attributes
  s" canal de agua" self% ms-name!
  self% is-indoor-location
  0 0 location-36% location-38% 0 0 0 0 self% init-location
  ;attributes
location-37% :description
  sight case
  self% of
    s" El agua" goes-down$ s& s" por un canal" s?&
    from-the(m)$ s& s" oeste con" s&
    s{ s" renovadas fuerzas" s" renovada energía" s" renovado ímpetu" }s& comma+
    s" dejando" s& s{
    s" a un lado" a-high-narrow-pass-way$ s&
    a-high-narrow-pass-way$ s{ s" lateral" s" a un lado" }s&
    }s& s" que" s& lets-you$ s& to-keep-going$ s&
    toward-the(m)$ s" este" s&
    toward-the(m)$ s" oeste" s& rnd2swap s" o" s& 2swap s& s&
    period+ paragraph
    endof
  east% of
    ^the-pass-way$ s" elevado" s?& lets-you$ s& to-keep-going$ s& that-way$ s& period+
    paragraph
    endof
  west% of
    water-from-there$
    the-pass-way$ s" elevado" s?& lets-you$ s& to-keep-going$ s& that-way$ s&
    both s" también" s& ^uppercase period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-38% :attributes
  s" gran cascada" self% fs-name!
  self% is-indoor-location
  0 0 location-37% location-39% 0 0 0 0 self% init-location
  ;attributes
location-38% :description
  sight case
  self% of
    s" Cae el agua hacia el este,"
    s{ s" descendiendo" s" bajando" }s&
    s{ s" con mucha fuerza" s" con gran fuerza" s" fuertemente" }s&
    s{ s" en dirección al" s" hacia el" }s& s" canal," s&
    s{ s" no sin antes" s" tras" s" después de" }s&
    s{ s" embalsarse" s" haberse embalsado" }s&
    s" en un lago" s&
    s{ s" no muy" s" no demasiado" s" poco" }s& s" profundo." s&
    paragraph
    endof
  east% of
    water-that-way$ paragraph
    endof
  west% of
    \ XXX TODO -- el artículo de «cascada» debe depender también de si se ha visitado el escenario 39 o este mismo 38
    ^water-from-there$
    s" , de" s+ waterfall% full-name s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-39% :attributes
  s" interior de la cascada" self% ms-name!
  self% is-indoor-location
  0 0 location-38% 0 0 0 0 0 self% init-location
  ;attributes
location-39% :description
  sight case
  self% of
    \ XXX TODO -- crear ente. musgo, cortina, agua, hueco
    s" Musgoso y rocoso, con la cortina de agua"
    s{ s" tras de ti," s" a tu espalda," }s&
    s{ s" el nivel" s" la altura" }s& s" del agua ha" s&
    s{ s" subido" s" crecido" }s&
    s{ s" un poco" s" algo" }s& s" en este" s&
    s{ s" curioso" s" extraño" }s& s" hueco." s&
    paragraph
    endof
  east% of
    \ XXX TODO -- variar
    s" Es la única salida." paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-40% :attributes
  s" explanada" self% fs-name!
  self% is-indoor-location
  0 location-35% location-41% 0 0 0 0 0 self% init-location
  ;attributes
location-40% :description
  \ XXX TODO -- crear ente. losas y losetas, estalactitas, panorama, escalones
  sight case
  self% of
    s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
    s" Unos casi imperceptibles escalones conducen al este." s&
    paragraph
    endof
  south% of
    ^that-way$ s" se va" s& toward-the(m)$ s& s" puente." s&
    paragraph
    endof
  east% of
    s" Los escalones" (they)-lead$ s& that-way$ s& period+
    paragraph
    endof
  up% of
    s{ s" Sobre" s" Por encima de" }s
    s{ s" ti" s" tu cabeza" }s& s" se" s&
    s{ s" exhibe" s" extiende" s" disfruta" }s&
    s" un" s& beautiful(m)$ s&
    s{ s" panorama" s" paisaje" }s s& s" de estalactitas." s&
    paragraph
    endof
  down% of
    s" Es una" s{ s" gran" s" buena" }s s& s" explanada enlosetada." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-41% :attributes
  \ XXX TODO -- cambiar el nombre. no se puede pasar a mayúscula un carácter pluriocteto en utf-8
  self% is-indoor-location
  s" ídolo" self% ms-name!
  0 0 0 location-40% 0 0 0 0 self% init-location
  ;attributes
location-41% :description
  \ XXX TODO -- crear ente. roca, centinela
  sight case
  self% of
    s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al sur."
    s" Se puede" s& to-go-back$ s& toward$ s& s" la explanada hacia el oeste." s&
    paragraph
    endof
  south% of
    s" Hay una" s" roca" s" enorme" rnd2swap s& s&
    that-way$ s& period+
    paragraph
    endof
  west% of
    s" Se puede volver" toward$ s& s" la explanada" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-42% :attributes
  s" pasaje estrecho" self% ms-name!
  self% is-indoor-location
  location-41% location-43% 0 0 0 0 0 0 self% init-location
  ;attributes
location-42% :description
  sight case
  self% of
    s" Como un pasillo que corteja el canal de agua, a su lado, baja de norte a sur."
    paragraph
    endof
  north% of
    ^the-pass-way$ goes-up$ s& that-way$ s&
    s" , de donde" s{ s" corre" s" procede" s" viene" s" proviene" }s& s" el agua." s& s+
    paragraph
    endof
  south% of
    ^the-pass-way$ goes-down$ s& that-way$ s&
    s" , siguiendo el canal de agua," s+
    s" hacia un lugar en que" s&
    s{ s" se aprecia" s" puede apreciarse" s" se distingue" }s&
    s" un aumento de luz." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-43% :attributes
  s" pasaje de la serpiente" self% ms-name!
  self% is-indoor-location
  location-42% 0 0 0 0 0 0 0 self% init-location
  ;attributes
location-43% :description
  sight case
  self% of
    ^the-pass-way$ s" sigue de norte a sur." s&
    paragraph
    endof
  north% of
    ^the-pass-way$ s" continúa" s& that-way$ s& period+
    paragraph
    endof
  south% of
    snake% is-here?
    if  a-snake-blocks-the-way$
    else  ^the-pass-way$ s" continúa" s& that-way$ s&
    then  period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-44% :attributes
  s" lago interior" self% ms-name!
  self% is-indoor-location
  location-43% 0 0 location-45% 0 0 0 0 self% init-location
  ;attributes
location-44% :description
  \ XXX TODO -- crear ente. lago, escaleras, pasaje, lago
  sight case
  self% of
    s" Unas escaleras" s{ s" dan" s" permiten el" }s& s{ s" paso" s" acceso" }s&
    s" a un" s& beautiful(m)$ s& s" lago interior, hacia el oeste." s&
    s" Al norte, un oscuro y"
    narrow(m)$ s& pass-way$ s& goes-up$ s& period+ s?&
    paragraph
    endof
  north% of
    s" Un pasaje oscuro y" narrow(m)$ s& goes-up$ s& that-way$ s& period+
    paragraph
    endof
  west% of
    s" Las escaleras" (they)-lead$ s& that-way$ s& s" , hacia el lago" s?+ period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-45% :attributes
  s" cruce de pasajes" self% ms-name!
  self% is-indoor-location
  0 location-47% location-44% location-46% 0 0 0 0 self% init-location
  ;attributes
location-45% :description
  \ XXX TODO -- crear ente. pasaje/camino/paso/senda
  sight case
  self% of
    ^narrow(mp)$ pass-ways$ s&
    s" permiten ir al oeste, al este y al sur." s&
    paragraph
    endof
  south% of
    ^a-narrow-pass-way$ s" permite ir" s& that-way$ s&
    s" , de donde" s+ s{ s" proviene" s" procede" }s&
    s{ s" una gran" s" mucha" }s& s" luminosidad." s&
    paragraph
    endof
  west% of
    ^a-narrow-pass-way$ leads$ s& that-way$ s& period+
    paragraph
    endof
  east% of
    ^a-narrow-pass-way$ leads$ s& that-way$ s& period+
    s" , de donde" s{ s" proviene" s" procede" }s&
    s{ s" algo de" s" una poca" s" un poco de" }s&
    s{ s" claridad" s" luz" }s& period+ s+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-46% :attributes
  s" hogar de Ambrosio" self% ms-name!
  self% is-indoor-location
  0 0 location-45% 0 0 0 0 0 self% init-location
  ;attributes
location-46% :description
  sight case
  self% of
    s" Un catre, algunas velas y una mesa es todo lo que"
    s{ s" tiene" s" posee" }s s" Ambrosio" rnd2swap s& s&
    period+  paragraph
    endof
  east% of
    s" La salida"
    s{ s" de la casa" s" del hogar" }s s" de Ambrosio" s& s?&
    s" está" s& that-way$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-47% :attributes
  s" salida de la cueva" self% fs-name!
  self% is-indoor-location
  location-45% 0 0 0 0 0 0 0 self% init-location
  ;attributes
location-47% :description
  \ XXX TODO -- descripción inacabada.
  sight case
  self% of
    s" Por el oeste,"
    door% full-name s& door% «open»|«closed» s& comma+
    door% is-open? if  \ La puerta está abierta
      s" por la cual entra la luz que ilumina la estancia," s&
      s" permite salir de la cueva." s&
    else  \ La puerta está cerrada
      s" al otro lado de la cual se adivina la luz diurna," s&
      door% is-known?
      if    s" impide" s&
      else  s" parece ser" s&
      then  s" la salida de la cueva." s&
    then
    paragraph
    endof
  north% of
    \ XXX TODO -- variar
    s" Hay salida" that-way$ s& period+ paragraph
    endof
  west% of
    \ XXX TODO -- variar
    door% is-open? if
      s" La luz diurna entra por la puerta."
    else
      s" Se adivina la luz diurna al otro lado de la puerta."
    then
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-48% :attributes
  s" bosque a la entrada" self% ms-name!
  0 0 location-47% location-49% 0 0 0 0 self% init-location
  ;attributes
: when-the-door$  ( -- ca len )
  s" cuando" s{ s" la" s" su" }s& s" puerta" s&  ;

: like-now$+  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  s" , como ahora" s?+  ;

location-48% :description
  \ XXX TODO -- crear ente. cueva
  sight case
  self% of
    s{ s" Apenas si" s" Casi no" }s
    s{ s" se puede" s" es posible" }s&
    s" reconocer la entrada de la cueva, al este." s&
    ^the-path$ s& s{ s" parte" s" sale" }s&
    s" del bosque hacia el oeste." s&
    paragraph
    endof
  east% of
    s" La entrada de la cueva" s{
    s" está" s" bien" s?& s{ s" camuflada" s" escondida" }s&
    s" apenas se ve" s" casi no se ve" s" pasa casi desapercibida"
    }s& comma+
    door% is-open? if
      even$ s& when-the-door$ s&
      s{ s" está abierta" s" no está cerrada" }s& like-now$+
    else
      s{ s" especialmente" s" sobre todo" }s& when-the-door$ s&
      s{ s" no está abierta" s" está cerrada" }s& like-now$+
    then  period+ paragraph
    endof
  west% of
    ^the-path$ s{ s" parte" s" sale" }s& s" del bosque" s& in-that-direction$ s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-49% :attributes
  s" sendero del bosque" self% ms-name!
  0 0 location-48% location-50% 0 0 0 0 self% init-location
  ;attributes
location-49% :description
  sight case
  self% of
    ^the-path$ s" recorre" s& s" toda" s?&
    s" esta" s& s{ s" parte" s" zona" }s&
    s" del bosque de este a oeste." s&
    paragraph
    endof
  east% of
    ^the-path$ leads$ s&
    s" al bosque a la entrada de la cueva." s&
    paragraph
    endof
  west% of
    ^the-path$ s" continúa" s& in-that-direction$ s& period+
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-50% :attributes
  s" camino norte" self% ms-name!
  0 location-51% location-49% 0 0 0 0 0 self% init-location
  ;attributes
location-50% :description
  sight case
  self% of
    s" El camino norte" s{ s" que sale" s" que parte" s" procedente" }s&
    s" de Westmorland se" s{ s" interna" s" adentra" }s& s" en el bosque," s&
    s" aunque en tu estado no puedes ir." s&
    paragraph
    endof
  south% of
    s{ s" ¡Westmorland!" s" Westmorland..." }s
    paragraph
    endof
  east% of
    ^can-see$ s" el sendero del bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description
location-51% :attributes
  s" Westmorland" self% fs-name!
  self% has-no-article
  location-50% 0 0 0 0 0 0 0 self% init-location
  ;attributes
location-51% :description
  \ XXX TODO -- crear ente. mercado, plaza, villa, pueblo, castillo
  sight case
  self% of
    ^the-village$ s" bulle de actividad con el mercado en el centro de la plaza," s&
    s" donde se encuentra el castillo." s&
    paragraph
    endof
  north% of
    s" El camino norte" of-the-village$ s& leads$ s& s" hasta el bosque." s&
    paragraph
    endof
  uninteresting-direction
  endcase
  ;description

\ Entes globales

cave% :attributes
  s" cueva" self% fs-name!
  \ self% is-global-indoor \ XXX
  ;attributes
cave% :description
  \ XXX TMP
  s" La cueva es chachi."
  paragraph
  ;description
ceiling% :attributes
  s" techo" self% ms-name!
  self% is-global-indoor
  ;attributes
ceiling% :description
  \ XXX TMP
  s" El techo es muy bonito."
  paragraph
  ;description
clouds% :attributes
  s" nubes" self% fp-name!
  self% is-global-outdoor
  ;attributes
clouds% :description
  \ XXX TODO:
  \ Distinguir no solo interiores, sino escenarios en
  \ que se puede vislumbrar el exterior.
  \ XXX TMP.:
  s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
  s" en la estación del frío se han alejado por el momento. " s&
  2 random if  paragraph  else  2drop sky% describe  then  \ XXX TODO -- comprobar
  ;description
floor% :attributes
  s" suelo" self% ms-name!
  self% is-global-indoor
  self% is-global-outdoor
  ;attributes
floor% :description
  \ XXX TMP
  am-i-outdoor? if
    s" El suelo fuera es muy bonito."
    paragraph
  else
    s" El suelo dentro es muy bonito."
    paragraph
  then
  ;description
sky% :attributes
  s" cielo" self% ms-name!
  self% is-global-outdoor
  ;attributes
sky% :description
  \ XXX TMP
  s" [El cielo es mu bonito]"
  paragraph
  ;description
wall% :attributes
  s" pared" self% ms-name!
  self% is-global-indoor
  ;attributes
wall% :description
  \ XXX TMP
  s" [La pared es mu bonita]"
  paragraph
  ;description

\ Entes virtuales

exits% :attributes
  s" salida" self% fs-name!
  self% is-global-outdoor
  self% is-global-indoor
  ;attributes
exits% :description
  list-exits
  ;description
inventory% :attributes
  ;attributes
enemy% :attributes
  \ XXX TODO -- unfinished
  s" enemigos" self% mp-name!
  self% is-human
  self% is-decoration
  ;attributes
enemy% :description
  \ XXX TODO -- unfinished
  battle# @ if
    s" Enemigo en batalla!!!"  \ XXX TMP
  else
    s" Enemigo en paz!!!"  \ XXX TMP
  then  paragraph
  ;description

\ Entes dirección

\ Los entes dirección guardan en su campo `~direction`
\ el desplazamiento correspodiente al campo de
\ dirección que representan
\ Esto sirve para reconocerlos como tales entes dirección
\ (pues todos los valores posibles son diferentes de cero)
\ y para hacer los cálculos en las acciones de movimiento.

north% :attributes
  s" norte" self% ms-name!
  self% has-definite-article
  north-exit> self% ~direction !
  ;attributes
south% :attributes
  s" sur" self% ms-name!
  self% has-definite-article
  south-exit> self% ~direction !
  ;attributes
east% :attributes
  s" este" self% ms-name!
  self% has-definite-article
  east-exit> self% ~direction !
  ;attributes
west% :attributes
  s" oeste" self% name!
  self% has-definite-article
  west-exit> self% ~direction !
  ;attributes
up% :attributes
  s" arriba" self% name!
  self% has-no-article
  up-exit> self% ~direction !
  ;attributes
up% :description
  am-i-outdoor?
  if  sky% describe
  else  ceiling% describe
  then
  ;description
down% :attributes
  s" abajo" self% name!
  self% has-no-article
  down-exit> self% ~direction !
  ;attributes
down% :description
  \ XXX TMP
  am-i-outdoor? if
    s" El suelo exterior es muy bonito." paragraph
  else
    s" El suelo interior es muy bonito." paragraph
  then
  ;description

out% :attributes
  s" afuera" self% name!
  self% has-no-article
  out-exit> self% ~direction !
  ;attributes
in% :attributes
  s" adentro" self% name!
  self% has-no-article
  in-exit> self% ~direction !
  ;attributes

\ }}} ==========================================================
section( Mensaje de acción completada)  \ {{{

variable silent-well-done?  \ XXX TODO -- not used yet

: (well-done)  ( -- )
  \ Informa, con un mensaje genérico, de que una acción se ha realizado.
  s{ s" Hecho." s" Bien." }s narrate  ;

: well-done  ( -- )
  \ Informa, con un mensaje genérico, de que una acción se ha realizado,
  \ si es preciso.
  silent-well-done? @ 0= ?? (well-done)
  silent-well-done? off  ;

: well-done-this  ( ca len -- )
  \ Informa, con un mensaje específico, de que una acción se ha realizado,
  \ si es preciso.
  silent-well-done? @ 0= if  narrate  else  2drop  then
  silent-well-done? off  ;

\ }}} ==========================================================
section( Errores de las acciones)  \ {{{

variable action  \ Código de la acción del comando
variable previous-action  \ Código de la acción del comando anterior

\ Entes complemento:
variable main-complement  \ Principal (complemento directo o destino)
variable secondary-complement  \ Secundario (complemento indirecto, destino u origen)
defer tool-complement  \ Herramienta (indicada con «con» o «usando»)
defer actual-tool-complement  \ Herramienta estricta (indicada con «usando»)
defer company-complement  \ Compañía (indicado con «con»)
defer actual-company-complement  \ Compañía estricta (indicada con «con» en presencia de «usando»)
false [if]
  \ XXX OLD
  \ XXX TODO -- descartado, pendiente
  variable to-complement  \ Destino \ XXX OLD -- no utilizado
  variable from-complement  \ Origen \ XXX OLD -- no utilizado
  variable into-complement  \ Destino dentro \ XXX OLD -- no utilizado
[then]

\ Ente que ha provocado un error
\ y puede ser citado en el mensaje de error correspondiente:
variable what

\ Código de la (seudo)preposición abierta, o cero:
variable current-preposition
\ Máscara de bitios de las (seudo)preposiciones usadas en la frase:
variable used-prepositions

: action-error-general-message$  ( -- ca len )
  \ Devuelve el mensaje de error operativo para el nivel 1.
  'action-error-general-message$ count  ;

: action-error-general-message  ( -- )
  \ Muestra el mensaje de error operativo para el nivel 1.
  action-error-general-message$ action-error  ;

: unerror  ( i*j pfa -- )
  \ Borra de la pila los parámetros de un error operativo.
  cell+ @ drops  ;

: action-error:  ( n xt "name1" -- xt2 )
  \ Crea un error operativo.
  \ n = número de parámetros del error operativo efectivo
  \ xt = dirección de ejecución del error operativo efectivo
  \ "name1" = nombre de la palabra de error a crear
  \ xt2 = dirección de ejecución de la palabra de error creada
  create , , latestxt
  does>  ( pfa )
    action-errors-verbosity @ case
      0 of  unerror  endof
      1 of  unerror action-error-general-message  endof
      2 of  perform  endof
    endcase  ;

: known-entity-is-not-here$  ( a -- ca1 len1 )
  \  Devuelve mensaje de que un ente conocido no está presente.
  full-name s" no está" s&
  s{ s" aquí" s" por aquí" }s&  ;

: unknown-entity-is-not-here$  ( a -- ca1 len1 )
  \  Devuelve mensaje de que un ente desconocido no está presente
  s{ s" Aquí" s" Por aquí" }s
  s" no hay" s&
  rot subjective-negative-name s&  ;

: (is-not-here)  ( a -- )
  \  Informa de que un ente no está presente.
  dup is-familiar?
  if    known-entity-is-not-here$
  else  unknown-entity-is-not-here$
  then  period+ action-error  ;

1 ' (is-not-here) action-error: is-not-here
to is-not-here-error#
: (is-not-here-what)  ( -- )
  \  Informa de que el ente `what` no está presente.
  what @ (is-not-here)  ;

0 ' (is-not-here-what) action-error: is-not-here-what
to is-not-here-what-error#
: (cannot-see)  ( a -- )
  \ Informa de que un ente no puede ser mirado.
  ^cannot-see$
  rot subjective-negative-name-as-direct-object s&
  period+ action-error  ;

1 ' (cannot-see) action-error: cannot-see
to cannot-see-error#
: (cannot-see-what)  ( -- )
  \ Informa de que el ente `what` no puede ser mirado.
  what @ (cannot-see)  ;

0 ' (cannot-see-what) action-error: cannot-see-what
to cannot-see-what-error#
: like-that$  ( -- ca len )
  \ XXX TODO -- not used
  s{ s" así" s" como eso" }s  ;

: something-like-that$  ( -- ca len )
  \ Devuelve una variante de «hacer eso».
  s" hacer" s?
  s{ s" algo así"
  s" algo semejante"
  s" eso"
  s" semejante cosa"
  s" tal cosa"
  s" una cosa así" }s&  ;

: is-impossible$  ( -- ca len )
  \ Devuelve una variante de «es imposible», que formará parte de mensajes personalizados por cada acción.
  s{
  s" es imposible"
  \ s" es inviable"
  s" no es posible"
  \ s" no es viable"
  \ s" no sería posible"
  \ s" no sería viable"
  \ s" sería imposible"
  \ s" sería inviable"
  }s  ;

: ^is-impossible$  ( -- ca len )
  \ Devuelve una variante de «Es imposible» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción.
  is-impossible$ ^uppercase  ;

: x-is-impossible$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «X es imposible».
  dup
  if  ^uppercase is-impossible$ s&
  else  2drop ^is-impossible$
  then  ;

: it-is-impossible-x$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «Es imposible x».
  ^is-impossible$ 2swap s&  ;

: (is-impossible)  ( ca len -- )
  \ Informa de que una acción indicada (en infinitivo) es imposible.
  \ ca len = Acción imposible, en infinitivo, o una cadena vacía
  ['] x-is-impossible$
  ['] it-is-impossible-x$
  2 choose execute  period+ action-error  ;

2 ' (is-impossible) action-error: is-impossible drop
: (impossible)  ( -- )
  \ Informa de que una acción no especificada es imposible.
  something-like-that$ (is-impossible)  ;

0 ' (impossible) action-error: impossible
to impossible-error#
: try$  ( -- ca len )
  \ Devuelve una variante de «intentar» (o vacía).
  s{ "" "" s" intentar" }s  ;

: nonsense$  ( -- ca len )
  \ Devuelve una variante de «no tiene sentido»,
  \ que formará parte de mensajes personalizados por cada acción.
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos
  s{
  s" es ilógico"
  s" no parece lógico"
  s" no parece muy lógico"
  s" no tiene lógica ninguna"
  s" no tiene lógica"
  s" no tiene mucha lógica"
  s" no tiene mucho sentido"
  s" no tiene ninguna lógica"
  s" no tiene ningún sentido"
  s" no tiene sentido"
  }s  ;

: ^nonsense$  ( -- ca len )
  \ Devuelve una variante de «No tiene sentido»
  \ (con la primera letra en mayúsculas)
  \ que formará parte de mensajes personalizados por cada acción.
  nonsense$ ^uppercase  ;

: x-is-nonsense$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «X no tiene sentido».
  dup if    try$ 2swap s& ^uppercase nonsense$ s&
      else  2drop ^nonsense$  then  ;

: it-is-nonsense-x$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «No tiene sentido x».
  ^nonsense$ try$ s& 2swap s&  ;

: (is-nonsense)  ( ca len -- )
  \ Informa de que una acción dada no tiene sentido.
  \ ca len = Acción que no tiene sentido;
  \       es un verbo en infinitivo, un sustantivo o una cadena vacía
  ['] x-is-nonsense$
  ['] it-is-nonsense-x$
  2 choose execute  period+ action-error  ;

2 ' (is-nonsense) action-error: is-nonsense drop
: (nonsense)  ( -- )
  \ Informa de que alguna acción no especificada no tiene sentido.
  \ XXX TMP
  s" eso" (is-nonsense)  ;

0 ' (nonsense) action-error: nonsense
to nonsense-error#
: dangerous$  ( -- ca len )
  \ Devuelve una variante de «es peligroso», que formará parte de mensajes personalizados por cada acción.
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos y unificar
  s{
  s" es algo descabellado"
  s" es descabellado"
  s" es muy arriesgado"
  s" es peligroso"
  s" es una insensatez"
  s" es una locura"
  s" no es seguro"
  s" no es sensato"
  s" sería algo descabellado"
  s" sería demasiado arriesgado"
  s" sería descabellado"
  s" sería peligroso"
  s" sería una insensatez"
  }s  ;

: ^dangerous$  ( -- ca len )
  \ Devuelve una variante de «Es peligroso» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción.
  dangerous$ ^uppercase  ;

: x-is-dangerous$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «X es peligroso».
  dup if    try$ 2swap s& ^uppercase dangerous$ s&
      else  2drop ^dangerous$  then  ;

: it-is-dangerous-x$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «Es peligroso x».
  ^dangerous$ try$ s& 2swap s&  ;

: (is-dangerous)  ( ca len -- )
  \ Informa de que una acción dada (en infinitivo)
  \ no tiene sentido.
  \ ca len = Acción que no tiene sentido, en infinitivo, o una cadena vacía
  ['] x-is-dangerous$
  ['] it-is-dangerous-x$
  2 choose execute  period+ action-error  ;

2 ' (is-dangerous) action-error: is-dangerous drop
: (dangerous)  ( -- )
  \ Informa de que alguna acción no especificada no tiene sentido.
  something-like-that$ (is-dangerous)  ;

0 ' (dangerous) action-error: dangerous
to dangerous-error#
: ?full-name&  ( ca1 len1 a2 -- )
  \ Añade a una cadena el nombre de un posible ente.
  \ XXX TODO -- not used
  \ ca1 len1 = Cadena
  \ a2 = Ente (o cero)
  ?dup if  full-name s&  then  ;

: (+is-nonsense)  ( ca len a1 -- )
  \ Informa de que una acción dada (en infinitivo)
  \ ejecutada sobre un ente no tiene sentido.
  \ ca len = Acción en infinitivo
  \ a1 = Ente al que se refiere la acción y cuyo objeto directo es (o cero)
  ?dup if    full-name s& (is-nonsense)
       else  2drop nonsense  then  ;

3 ' (+is-nonsense) action-error: +is-nonsense drop
: (main-complement+is-nonsense)  ( ca len -- )
  \ Informa de que una acción dada (en infinitivo),
  \ que hay que completar con el nombre del complemento principal,
  \ no tiene sentido.
  \ ca len = Acción que no tiene sentido, en infinitivo
  main-complement @ (+is-nonsense)  ;

2 ' (main-complement+is-nonsense) action-error: main-complement+is-nonsense drop
: (secondary-complement+is-nonsense)  ( ca len -- )
  \ Informa de que una acción dada (en infinitivo),
  \ que hay que completar con el nombre del complemento secundario,
  \ no tiene sentido.
  \ ca len = Acción que no tiene sentido, en infinitivo
  secondary-complement @ (+is-nonsense)  ;

2 ' (secondary-complement+is-nonsense) action-error: secondary-complement+is-nonsense drop
: no-reason-for$  ( -- ca len )
  \ Devuelve una variante de «no hay motivo para».
  \ XXX TODO -- quitar las variantes que no sean adecuadas a todos los casos
  s" No hay" s{
    s" nada que justifique"
    s{  s" necesidad" s" alguna" s?&
        s" ninguna necesidad"
    }s s" de" s&
    s{  s" ninguna razón"
        s" ningún motivo"
        s" motivo" s" alguno" s?&
        s" razón" s" alguna" s?&
    }s s" para" s&
  }s&  ;

: (no-reason-for-that)  ( ca len -- )
  \ Informa de que no hay motivo para una acción (en infinitivo).
  \ ca len = Acción para la que no hay razón, en infinitivo, o una cadena vacía
  \ XXX TODO
  no-reason-for$ 2swap s& period+ action-error  ;

2 ' (no-reason-for-that) action-error: no-reason-for-that drop
: (no-reason)  ( -- )
  \ Informa de que no hay motivo para una acción no especificada.
  \ XXX TODO
  something-like-that$ (no-reason-for-that)  ;

0 ' (no-reason) action-error: no-reason drop
: (nonsense|no-reason)  ( -- )
  \ Informa de que una acción no especificada no tiene sentido o no tiene motivo.
  \ XXX TODO -- not used yet
  ['] nonsense
  ['] no-reason
  2 choose execute  ;

0 ' (nonsense|no-reason) action-error: nonsense|no-reason drop
: (do-not-worry-0)$  ( -- a u)
  \ Primera versión posible del mensaje de `do-not-worry`.
  s{
  s" Como si no hubiera"
  s" Hay"
  s" Se diría que hay"
  s" Seguro que hay"
  s" Sin duda hay"
  }s
  s{ s" cosas" s" tareas" s" asuntos" s" cuestiones" }s&
  s" más" s&
  s{ s" importantes" s" urgentes" s" útiles" }s&
  s{
  "" s" a que prestar atención" s" de que ocuparse"
  s" para ocuparse" s" para prestarles atención"
  }s&  ;

: (do-not-worry-1)$  ( -- a u)
  \ Segunda versión posible del mensaje de `do-not-worry`.
  \ XXX TODO: consultar «menester»
  s" Eso no" s{
  s" es importante"
  s" es menester"
  s" es necesario"
  s" hace falta"
  s" importa"
  s" parece importante"
  s" parece necesario"
  s" tiene importancia"
  s" tiene utilidad"
  }s&  ;

: do-not-worry  ( -- )
  \ Informa de que una acción no tiene importancia.
  \ XXX TMP, no se usa
  ['] (do-not-worry-0)$
  ['] (do-not-worry-1)$ 2 choose execute
  now-$ s&  period+ action-error  ;

: (unnecessary-tool-for-that)  ( ca1 len1 a2 -- )
  \ Informa de que un ente es innecesario como herramienta
  \ para ejecutar una acción.
  \ ca1 len1 = Acción (una frase con verbo en infinitivo)
  \ a2 = Ente innecesario
  \ XXX TODO -- unfinished
  full-name s" No necesitas" 2swap s& s" para" s& 2swap s&
  period+ action-error  ;

3 ' (unnecessary-tool-for-that) action-error: unnecessary-tool-for-that
to unnecessary-tool-for-that-error#
: (unnecessary-tool)  ( a -- )
  \ Informa de que un ente es innecesario como herramienta
  \ para ejecutar una acción sin especificar.
  \ a = Ente innecesario
  \ XXX TODO -- añadir variante "no es/son necesaria/o/s
  \ XXX TODO -- ojo con entes especiales: personas, animales, virtuales..
  ['] full-name ['] negative-full-name 2 choose execute
  s" No" s{ s" te" s? s" hace falta" s&
  s" necesitas" s" se necesita"
  s" precisas" s" se precisa"
  s" hay necesidad de" s{ s" usar" s" emplear" s" utilizar" }s?&
  }s&  2swap s&
  s{ s" para nada" s" para eso" }s?&  period+ action-error  ;
  \ XXX TODO -- añadir coletilla "aunque la/lo/s tuvieras"?

1 ' (unnecessary-tool) action-error: unnecessary-tool
to unnecessary-tool-error#

0 [if]

  \ XXX FIXME -- error «no tiene nada especial»
  \ XXX TODO -- inacabado

: it-is-normal-x$  ( ca1 len1 -- ca2 len2 )
  \ Devuelve una variante de «no tiene nada especial x».
  ^normal$ try$ s& 2swap s&  ;

: (is-normal)  ( a -- )
  \ Informa de que un ente no tiene nada especial.
  \ ca len = Acción que no tiene nada especial; es un verbo en infinitivo, un sustantivo o una cadena vacía
  ['] x-is-normal$
  ['] it-is-normal-x$
  2 choose execute  period+ action-error  ;

1 ' (is-normal) action-error: is-normal
to is-normal-error#

[then]

: that$  ( a -- ca1 len1 )
  \  Devuelve el nombre de un ente, o un pronombre demostrativo.
  2 random
  if  drop s" eso"  else  full-name  then  ;

: you-do-not-have-it-(0)$  ( a -- )
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 0).
  s" No" you-carry$ s& rot that$ s& with-you$ s&  ;

: you-do-not-have-it-(1)$  ( a -- )
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 1, solo para entes conocidos).
  s" No" rot direct-pronoun s& you-carry$ s& with-you$ s&  ;

: you-do-not-have-it-(2)$  ( a -- )
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 2, solo para entes no citados en el comando).
  s" No" you-carry$ s& rot full-name s& with-you$ s&  ;

: (you-do-not-have-it)  ( a -- )
  \ Informa de que el protagonista no tiene un ente.
  dup is-known? if
    ['] you-do-not-have-it-(0)$
    ['] you-do-not-have-it-(1)$
    2 choose execute
  else  you-do-not-have-it-(0)$
  then  period+ action-error  ;

1 ' (you-do-not-have-it) action-error: you-do-not-have-it
to you-do-not-have-it-error#
: (you-do-not-have-what)  ( -- )
  \ Informa de que el protagonista no tiene el ente `what`.
  what @ (you-do-not-have-it)  ;

0 ' (you-do-not-have-what) action-error: you-do-not-have-what
to you-do-not-have-what-error#

: it-seems$  ( -- ca len )
  s{ "" s" parece que" s" por lo que parece," }s  ;

: it-is$  ( -- ca len )
  s{ s" es" s" sería" s" será" }s  ;

: to-do-it$  ( -- ca len )
  s" hacerlo" s?  ;

: possible-to-do$  ( -- ca len )
  it-is$ s" posible" s& to-do-it$ s&  ;

: impossible-to-do$  ( -- ca len )
  it-is$ s" imposible" s& to-do-it$ s&  ;

: can-be-done$  ( -- ca len )
  s{ s" podrá" s" podría" s" puede" }s
  s" hacerse" s&  ;

: can-not-be-done$  ( -- ca len )
  s" no" can-be-done$ s&  ;

: only-by-hand$  ( -- ca len )
  s{
  s" con la sola ayuda de las manos"
  s" con las manos como única herramienta"
  s" con las manos desnudas"
  s" con las manos"
  s" simplemente con las manos"
  s" sin alguna herramienta"
  s" sin herramientas"
  s" sin la herramienta adecuada"
  s" sin una herramienta"
  s" solamente con las manos"
  s" solo con las manos"
  s" tan solo con las manos"
  s" únicamente con las manos"
  }s  ;

: not-by-hand-0$  ( -- ca len )
  \ Devuelve la primera versión del mensaje de NOT-BY-HAND.
  it-seems$
  s{
    s" no" possible-to-do$ s&
    impossible-to-do$
    can-not-be-done$
  }s&
  only-by-hand$ s& period+ ^uppercase  ;

: some-tool$  ( -- ca len )
  s{
  s{ s" la" s" alguna" s" una" }s s" herramienta" s&
  s{ s" adecuada" s" apropiada" }s&
  s{ s" el" s" algún" s" un" }s s" instrumento" s&
  s{ s" adecuado" s" apropiado" }s&
  }s  ;

: not-by-hand-1$  ( -- ca len )
  \ Devuelve la segunda versión del mensaje de `not-by-hand`.
  it-seems$
  s{
    s{ s" hará" s" haría" s" hace" }s s" falta" s&
    s{
      s{ s" será" s" sería" s" es" }s s" menester" s&
      s{ s" habrá" s" habría" s" hay" }s s" que" s&
    }s{ s" usar" s" utilizar" s" emplear" }s&
  }s& some-tool$ s& period+ ^uppercase  ;

: not-by-hand$  ( -- ca len )
  \ Devuelve mensaje de `not-by-hand`.
  ['] not-by-hand-0$
  ['] not-by-hand-1$
  2 choose execute ^uppercase  ;

: (not-by-hand)  ( -- )
  \ Informa de que la acción no puede hacerse sin una herramienta.
  not-by-hand$ action-error  ;

0 ' (not-by-hand) action-error: not-by-hand drop
: (you-need)  ( a -- )
  \ Informa de que el protagonista no tiene un ente necesario.
  2 random
  if  you-do-not-have-it-(2)$ period+ action-error
  else  drop (not-by-hand)
  then  ;

1 ' (you-need) action-error: you-need drop
: (you-need-what)  ( -- )
  \ Informa de que el protagonista no tiene el ente `what` necesario.
  what @ (you-need)  ;

0 ' (you-need-what) action-error:  you-need-what
to you-need-what-error#
: you-already-have-it-(0)$  ( a -- )
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 0).
  s" Ya" you-carry$ s& rot that$ s& with-you$ s&  ;

: you-already-have-it-(1)$  ( a -- )
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 1, solo para entes conocidos).
  s" Ya" rot direct-pronoun s& you-carry$ s& with-you$ s&  ;

: (you-already-have-it)  ( a -- )
  \ Informa de que el protagonista ya tiene un ente.
  dup familiar over belongs-to-protagonist? or   if
    ['] you-already-have-it-(0)$
    ['] you-already-have-it-(1)$
    2 choose execute
  else  you-already-have-it-(0)$
  then  period+ action-error  ;

1 ' (you-already-have-it) action-error: you-already-have-it
to you-already-have-it-error#
: (you-already-have-what)  ( a -- )
  \ Informa de que el protagonista ya tiene el ente `what`.
  what @ (you-already-have-it)  ;

1 ' (you-already-have-what) action-error: you-already-have-what
to you-already-have-what-error#
: ((you-do-not-wear-it))  ( a -- )
  \ Informa de que el protagonista no lleva puesto un ente prenda.
  >r s" No llevas puest" r@ noun-ending+
  r> full-name s& period+ action-error  ;

: (you-do-not-wear-it)  ( a -- )
  \ Informa de que el protagonista no lleva puesto un ente prenda, según lo lleve o no consigo.
  dup is-hold?
  if  (you-do-not-have-it)  else  ((you-do-not-wear-it))  then  ;

1 ' (you-do-not-wear-it) action-error: you-do-not-wear-it drop
: (you-do-not-wear-what)  ( -- )
  \ Informa de que el protagonista no lleva puesto el ente `what`, según lo lleve o no consigo.
  what @ (you-do-not-wear-it)  ;

0 ' (you-do-not-wear-what) action-error: you-do-not-wear-what
to you-do-not-wear-what-error#
: (you-already-wear-it)  ( a -- )
  \ Informa de que el protagonista lleva puesto un ente prenda.
  >r s" Ya llevas puest" r@ noun-ending+
  r> full-name s& period+ action-error  ;

1 ' (you-already-wear-it) action-error: you-already-wear-it drop
: (you-already-wear-what)  ( -- )
  \ Informa de que el protagonista lleva puesto el ente `what`.
  what @ (you-already-wear-it)  ;

0 ' (you-already-wear-what) action-error: you-already-wear-what
to you-already-wear-what-error#
: not-with-that$  ( -- ca len )
  \ Devuelve mensaje de NOT-WITH-THAT.
  s" Con eso no..."
  s" No con eso..."
  2 schoose  ;

: (not-with-that)  ( -- )
  \ Informa de que la acción no puede hacerse con la herramienta elegida.
  not-with-that$ action-error  ;

0 ' (not-with-that) action-error: not-with-that drop
: (it-is-already-open)  ( a -- )
  \ Informa de que un ente ya está abierto.
  s" Ya está abiert" rot noun-ending+ period+ action-error  ;

1 ' (it-is-already-open) action-error: it-is-already-open drop
: (what-is-already-open)  ( -- )
  \ Informa de que el ente `what` ya está abierto.
  what @ (it-is-already-open)  ;

0 ' (what-is-already-open) action-error: what-is-already-open
to what-is-already-open-error#
: (it-is-already-closed)  ( a -- )
  \ Informa de que un ente ya está cerrado.
  s" Ya está cerrad" r@ noun-ending+ period+ action-error  ;

1 ' (it-is-already-closed) action-error: it-is-already-closed drop
: (what-is-already-closed)  ( -- )
  \ Informa de que el ente `what` ya está cerrado.
  what @ (it-is-already-closed)  ;

0 ' (what-is-already-closed) action-error: what-is-already-closed
to what-is-already-closed-error#

\ }}} ==========================================================
section( Listas)  \ {{{

variable #listed  \ Contador de elementos listados, usado en varias acciones
variable #elements  \ Total de los elementos de una lista

: list-separator$  ( u1 u2 -- ca len )
  \ Devuelve el separador adecuado a un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  \ ca len = Cadena devuelta, que podrá ser « y » o «, » o «» (vacía)
  ?dup if
    1+ = if  s"  y "  else  s" , "  then
  else  0
  then  ;

: (list-separator)  ( u1 u2 -- )
  \ Añade a la cadena dinámica `print-str` el separador adecuado («y» o «,») para un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  1+ = if  s" y" »&  else  s" ," »+  then  ;

: list-separator  ( u1 u2 -- )
  \ Añade a la cadena dinámica `print-str` el separador adecuado (o ninguno) para un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  ?dup if  (list-separator)  else  drop  then  ;

: can-be-listed?  ( a -- f )
  \ ¿El ente puede ser incluido en las listas?
  \ XXX TODO -- unfinished
  dup protagonist% <>  \ ¿No es el protagonista?
  over is-decoration? 0=  and  \ ¿Y no es decorativo?
  over is-listed? and  \ ¿Y puede ser listado?
  swap is-global? 0=  and  ;  \ ¿Y no es global?

: /list++  ( u1 a1 a2 -- u1 | u2 )
  \ Actualiza un contador si un ente es la localización de otro y puede ser listado.
  \ u1 = Contador
  \ a1 = Ente que actúa como localización
  \ a2 = Ente cuya localización hay que comprobar
  \ u2 = Contador incrementado
  dup can-be-listed?
  if  location = abs +  else  2drop  then  ;

: /list  ( a -- u )
  \ Cuenta el número de entes cuya localización es el ente indicado y pueden ser listados.
  \ a = Ente que actúa como localización
  \ u = Número de entes localizados en el ente y que pueden ser listados
  0  \ Contador
  #entities 0 do
    over i #>entity /list++
  loop  nip  ;

: (worn)$  ( a -- ca1 len1 )
  \ Devuelve «(puesto/a/s)», según el género y número del ente indicado.
  s" (puest" rot noun-ending s" )" s+ s+  ;

: (worn)&  ( ca1 len1 a2 -- ca1 len1 | ca3 len3 )
  \ Añade a una cadena, si es necesario, el indicador de que el ente indicado es una prenda puesta.
  \ ca1 len1 = Cadena con el nombre del ente
  \ a2 = Ente
  \ ca3 len3 = Nombre del ente con, si es necesario, el indicador de que se trata de una prenda puesta
  dup  is-worn? if  (worn)$ s&  else  drop  then  ;

: full-name-as-direct-complement  ( a -- ca1 len1 )
  \ Devuelve el nombre completo de un ente en función de complemento directo.
  \ Esto es necesario para añadir la preposición «a» a las personas.
  dup s" a" rot is-human? and
  rot full-name s&
  s" al" s" a el" sreplace  ;

: (content-list)  ( a -- )
  \ Añade a la lista en la cadena dinámica `print-str` el separador y el nombre de un ente.
  #elements @ #listed @  list-separator
  dup full-name-as-direct-complement rot (worn)& »&  #listed ++  ;

: about-to-list  ( a -- u )
  \ Prepara el inicio de una lista.
  \ a = Ente que es la localización de los entes a incluir en la lista
  \ u = Número de entes que serán listados
  #listed off  /list dup #elements !  ;

: content-list  ( a -- ca1 len1 )
  \ Devuelve una lista de entes localización es el ente indicado.
  \ a = Ente que actúa como localización
  \ ca1 len1 = Lista de objetos localizados en dicho ente
  «»-clear
  dup about-to-list if
    #entities 1 do
      dup i #>entity dup can-be-listed? if
        is-there? if  i #>entity (content-list)  then
      else  2drop
      then
    loop  s" ." »+
  then  drop  «»@  ;

: .present  ( -- )
  \ Lista los entes presentes.
  my-location content-list dup
  if  s" Ves" s" Puedes ver" 2 schoose 2swap s& narrate
  else  2drop
  then  ;

\ }}} ==========================================================
section( Tramas comunes a todos los escenarios)  \ {{{

\ ----------------------------------------------
\ Ambrosio nos sigue

\ XXX TODO --
\ Confirmar la función de la llave aquí. En el código original
\ solo se distingue que sea manipulable o no, lo que es
\ diferente a que esté accesible.

: ambrosio-must-follow?  ( -- )
  \ ¿Ambrosio tiene que estar siguiéndonos?
  ambrosio% not-vanished?  key% is-accessible? and
  location-46% am-i-there?  ambrosio-follows? @ or  and  ;

: ambrosio-must-follow  ( -- )
  \ Ambrosio tiene que estar siguiéndonos.
  my-location ambrosio% is-there
  s{ s" tu benefactor" ambrosio% full-name }s ^uppercase
  s" te sigue, esperanzado." s& narrate  ;

\ ----------------------------------------------
\ Lanzadores de las tramas comunes a todos los escenarios

: before-describing-any-location  ( -- )
  \ Trama de entrada común a todos los entes escenario.
  ;
  \ XXX TODO -- not used

: after-describing-any-location  ( -- )
  \ Trama de entrada común a todos los entes escenario.
  ;
  \ XXX TODO -- not used

: after-listing-entities-of-any-location  ( a -- )
  \ Trama final de entrada común a todos los entes escenario.
  ambrosio-must-follow? ?? ambrosio-must-follow  ;

\ }}} ==========================================================
section( Herramientas para las tramas asociadas a escenarios)  \ {{{

: [:location-plot]  ( a -- )
  \ Inicia la definición de trama de un ente escenario
  \ (cualquier tipo de trama de escenario).
  \ Esta palabra se ejecutará al comienzo de la palabra de trama de escenario.
  \ El identificador del ente está en la pila
  \ porque se compiló con `literal` cuando se creó la palabra de trama.
  to self%  ;  \ Actualizar el puntero al ente, usado para aligerar la sintaxis

: (:can-i-enter-location?)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de la trama previa de entrada a un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define la trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~can-i-enter-location?-xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: :can-i-enter-location?  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ la trama previa de entrada a un ente escenario
  :noname noname-roll
  (:can-i-enter-location?)  \ Hacer las operaciones preliminares
  postpone [:location-plot]  ;  \ Compilar la palabra `[:location-plot]` en la palabra creada, para que se ejecute cuando sea llamada

: (:before-describing-location)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de una trama de entrada a un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define la trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~before-describing-location-xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: :before-describing-location  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ una trama de entrada a un ente escenario
  :noname noname-roll
  (:before-describing-location)  \ Hacer las operaciones preliminares
  postpone [:location-plot]  ;  \ Compilar la palabra `[:location-plot]` en la palabra creada, para que se ejecute cuando sea llamada

: (:after-describing-location)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de una trama de entrada a un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define la trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~after-describing-location-xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: :after-describing-location  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ una trama de entrada a un ente escenario
  :noname noname-roll
  (:after-describing-location)  \ Hacer las operaciones preliminares
  postpone [:location-plot]  ;  \ Compilar la palabra `[:location-plot]` en la palabra creada, para que se ejecute cuando sea llamada

: (:after-listing-entities)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de la trama final de entrada a un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define la trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~after-listing-entities-xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: :after-listing-entities  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ la trama de entrada a un ente escenario
  :noname noname-roll
  (:after-listing-entities)  \ Hacer las operaciones preliminares
  postpone [:location-plot]  ;  \ Compilar la palabra [:LOCATION-PLOT] en la palabra creada, para que se ejecute cuando sea llamada

: (:before-leaving-location)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de la trama de salida de un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define su trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~before-leaving-location-xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self%` en tiempo de ejecución

: :before-leaving-location  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ la trama de salida de un ente escenario
  :noname noname-roll
  (:before-leaving-location)  \ Hacer las operaciones preliminares
  postpone [:location-plot]  ;  \ Compilar la palabra `[:location-plot]` en la palabra creada, para que se ejecute cuando sea llamada

true [if]
  : ;can-i-enter-location?  ( colon-sys -- )  postpone ;  ;  immediate
  : ;after-describing-location  ( colon-sys -- )  postpone ;  ;  immediate
  : ;after-listing-entities  ( colon-sys -- )  postpone ;  ;  immediate
  : ;before-leaving-location  ( colon-sys -- )  postpone ;  ;  immediate
[else]  \ XXX OLD -- así es más simple pero no funciona en gforth.:
  ' ; alias ;can-i-enter-location?  immediate
  ' ; alias ;after-describing-location  immediate
  ' ; alias ;after-listing-entities  immediate
  ' ; alias ;before-leaving-location  immediate
[then]
: ?execute  ( xt | 0 -- )
  \ Ejecuta un vector de ejecución, si no es cero.
  ?dup ?? execute  ;

: before-describing-location  ( a -- )
  \ Trama de entrada a un ente escenario.
  before-describing-any-location
  before-describing-location-xt ?execute  ;

: after-describing-location  ( a -- )
  \ Trama de entrada a un ente escenario.
  after-describing-any-location
  after-describing-location-xt ?execute  ;

: after-listing-entities  ( a -- )
  \ Trama final de entrada a un ente escenario.
  after-listing-entities-of-any-location
  after-listing-entities-xt ?execute  ;

: before-leaving-any-location  ( -- )
  \ Trama de salida común a todos los entes escenario.
  ;
  \ XXX TODO -- not used

: before-leaving-location  ( a -- )
  \ Ejecuta la trama de salida de un ente escenario.
  before-leaving-any-location
  before-leaving-location-xt ?execute  ;

: (leave-location)  ( a -- )
  \ Tareas previas a abandonar un escenario.
  dup visits++
  dup before-leaving-location
  protagonist% was-there  ;

: leave-location  ( -- )
  \ Tareas previas a abandonar el escenario actual.
  my-location ?dup if  (leave-location)  then  ;

: actually-enter-location  ( a -- )
  \ Entra en un escenario.
  leave-location
  dup my-location!
  dup before-describing-location
  dup describe
  dup after-describing-location
  dup familiar++  .present
  after-listing-entities  ;

: enter-location?  ( a -- f )
  \ Ejecuta la trama previa a la entrada de un escenario,
  \ que devolverá un indicador de que puede entrarse en el escenario;
  \ si esta trama no está definida para el ente, el indicador será `true`.
  \ a = Ente escenario
  \ f = ¿Hay que entrar en él?
  can-i-enter-location?-xt ?dup if  execute  else  true  then  ;

: enter-location  ( a -- )
  \ Entra en un escenario, si es posible.
  dup enter-location? and ?dup ?? actually-enter-location  ;

\ }}} ==========================================================
section( Recursos de las tramas asociadas a lugares)  \ {{{

\ ----------------------------------------------
\ Regreso a casa

: pass-still-open?  ( -- f )
  \ ¿El paso del desfiladero está abierto por el norte?
  location-08% has-north-exit?  ;

: still-in-the-village?  ( -- f )
  \ ¿Los soldados no se han movido aún de la aldea sajona?
  my-location location-01% =
  location-02% is-not-visited? and  ;

: back-to-the-village?  ( -- f )
  \ ¿Los soldados han regresado a la aldea sajona?
  \ XXX TODO -- not used
  my-location location-01% =
  location-02% is-visited? and  ;

: soldiers-follow-you  ( -- )
  \ De vuelta a casa.
  ^all-your$ soldiers$ s&
  s{ s" siguen tus pasos." s" te siguen." }s&
  narrate  ;

: going-home  ( -- )
  \ De vuelta a casa, si procede.
  pass-still-open?
  still-in-the-village? 0= and
  ?? soldiers-follow-you  ;

: celebrating  ( -- )
  \ Celebrando la victoria.
  \ XXX TODO -- unfinished
  ^all-your$ soldiers$ s&
  s{ s" lo están celebrando." s" lo celebran." }s&
  narrate  ;

\ ----------------------------------------------
\ Persecución

: pursued  ( -- )
  \ Perseguido por los sajones.
  s{
  s" El tiempo apremia"
  s" Hay que apresurarse"
  s" No hay mucho tiempo"
  s" No hay tiempo que perder"
  s" No sabes cuánto tiempo te queda"
  s" No te queda mucho tiempo"
  s" No tienes mucho tiempo"
  s" No tienes tiempo que perder"
  s" Sabes que debes darte prisa"
  s" Sabes que no puedes perder tiempo"
  s" Te queda poco tiempo"
  s" Tienes que apresurarte"
  }s s" ..." s+  narrate  ;

: pursue-location?  ( -- f )
  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?
  my-location location-12% <  ;

: you-think-you're-safe$  ( -- ca len )
  s{  s" crees estar"
      s" te sientes"
      s" crees sentirte"
      s" tienes la sensación de estar"
  }s{ s" a salvo" s" seguro" }s&  ;

: but-it's-an-impression$  ( -- ca len )
  s" ," but$ s&
  s{ s" dentro de ti" s" en tu interior" s" en el fondo" }s?&
  s{
    s{  s" sabes" s" bien" s?& s" eres consciente de" }s s" que" s&
    s{  s" tu instinto" s{ s" militar" s" guerrero" s" de soldado" }s?&
        s" una voz"
        s" algo"
    }s s{ s" no te engaña"
          s" te dice que" s{  s" no debes confiarte"
                              still$ s" no lo has logrado" s& }s&
    }s& s{ s" ;" s" :" s" ..." }s+
  }s& s{
    only$ s" es una falsa impresión" s&
    still$  s{  s" no lo has" s{ s" logrado" s" conseguido" }s&
                s" podrían" s{ s" encontrarte" s" atraparte" }s& s" aquí" s?&
            }s&
    s" puede que te hayan" s{ s" visto" s" entrar" s?&
                              s" seguido" }s&
    s{ s" probablemente" s" seguramente" }s
      s" te" s& s{  s" estarán buscando"
                    s" habrán seguido"
                    s" hayan visto" s" entrar" s?& }s&
  }s&  ;

\ ----------------------------------------------
\ Batalla

: all-your-men  ( -- ca len f )
  \ Devuelve una variante de «Todos tus hombres», y un indicador de número.
  \ ca len = Cadena
  \ f = ¿El texto está en plural?
  2 random dup
  if  s{ s" Todos" s" Todos y cada uno de" }s
  else  s" Hasta el último de"
  then  your-soldiers$ s&  rot  ;

: ?plural-verb  ( ca1 len1 f -- ca1 len1 | ca2 len2 )
  \ Pone un verbo en plural si es preciso.
  if  s" n" s+  then  ;

: fight/s$  ( f -- ca len )
  \ Devuelve una variante de «lucha/n».
  \ f = ¿El resultado debe estar en plural?
  \ ca len = Resultado
  s{ s" lucha" s" combate" s" pelea" s" se bate" }s
  rot ?plural-verb  ;

: resist/s$  ( f -- ca len )
  \ Devuelve una variante de «resiste/n».
  \ f = ¿El resultado debe estar en plural?
  \ ca len = Resultado
  s{ s" resiste" s" aguanta" s" contiene" }s
  rot ?plural-verb  ;

: heroe$  ( -- ca len )
  \ Devuelve una variante de «héroe».
  s{ s" héroe" s" valiente" s" jabato" }s  ;

: heroes$  ( -- ca len )
  \ Devuelve una variante de «héroes».
  heroe$ s" s" s+  ;

: like-a-heroe$ ( -- ca len )
  \ Devuelve una variante de «como un héroe».
  s" como un" s" auténtico" s?& heroe$ s&  ;

: like-heroes$ ( -- ca len )
  \ Devuelve una variante de «como héroes».
  s" como" s" auténticos" s?& heroes$ s&  ;

: (bravery)$  ( -- ca len )
  \ Devuelve una variante de «con denuedo».
  s{ s" con denuedo" s" con bravura" s" con coraje"
  s" heroicamente" s" esforzadamente" s" valientemente" }s  ;

: bravery$  ( f -- ca len )
  \ Devuelve una variante de «con denuedo», en singular o plural.
  \ f = ¿El resultado debe estar en plural?
  \ ca len = Resultado
  (bravery)$  rot
  if  like-heroes$  else  like-a-heroe$  then
  2 schoose  ;

: step-by-step$  ( -- ca len )
  \ Devuelve una variante de «poco a poco».
  s{ s" por momentos" s" palmo a palmo" s" poco a poco" }s  ;

: field$  ( -- ca len )
  \ Devuelve «terreno» o «posiciones».
  s{ s" terreno" s" posiciones" }s  ;

: last(fp)$  ( -- ca len )
  \ Devuelve una variante de «últimas».
  s{ s" últimas" s" postreras" }s  ;

: last$  ( -- ca len )
  \ Devuelve una variante de «último».
  s{ s" último" s" postrer" }s  ;

: last-energy(fp)$  ( -- ca len )
  \ Devuelve una variante de «últimas energías».
  last(fp)$ s{ s" energías" s" fuerzas" }s&  ;

: battle-phase-00$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 00)
  s" A pesar de" s{
  s" haber sido" s{ s" atacados por sorpresa" s" sorprendidos" }s&
  s" la sorpresa" s" inicial" s?&
  s" lo" s{ s" inesperado" s" sorpresivo" s" sorprendente" s" imprevisto" }s&
  s" del ataque" s& }s& comma+ your-soldiers$ s&
  s{ s" responden" s" reaccionan" }s&
  s{ s" con prontitud" s" sin perder un instante"
  s" rápidamente" s" como si fueran uno solo"
  }s& s" y" s&{
  s" adoptan una formación defensiva"
  s" organizan la defensa"
  s" se" s{ s" preparan" s" aprestan" }s& s" para" s&
  s{ s" defenderse" s" la defensa" }s&
  }s& period+  ;

: battle-phase-00  ( -- )
  \ Combate (fase 00).
  battle-phase-00$ narrate  ;

: battle-phase-01$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 01)
  all-your-men  dup resist/s$  rot bravery$  s& s&
  s{  s{ s" el ataque" s" el empuje" s" la acometida" }s
      s" inicial" s&
      s" el primer" s{ s" ataque" s" empuje" }s&
      s" la primera acometida"
  }s& of-the-enemy|enemies$ s& period+  ;

: battle-phase-01  ( -- )
  \ Combate (fase 01).
  battle-phase-01$ narrate  ;

: battle-phase-02$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 02)
  all-your-men  dup fight/s$  rot bravery$  s& s&
  s" contra" s&  the-enemy|enemies$ s&  period+  ;

: battle-phase-02  ( -- )
  \ Combate (fase 02).
  battle-phase-02$ narrate  ;

: battle-phase-03$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 03)
  \ XXX TODO -- unfinished
  ^your-soldiers$
  s" empiezan a acusar" s&
  s{ "" s" visiblemente" s" notoriamente" }s&
  s" el" s&{ s" titánico" s" enorme" }s?&
  s" esfuerzo." s&  ;

: battle-phase-03  ( -- )
  \ Combate (fase 03).
  battle-phase-03$ narrate  ;

: battle-phase-04$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 04)
  ^the-enemy|enemies
  s" parece que empieza* a" rot *>verb-ending s&
  s{ s" dominar" s" controlar" }s&
  s{ s" el campo" s" el combate" s" la situación" s" el terreno" }s&
  period+  ;

: battle-phase-04  ( -- )
  \ Combate (fase 04).
  battle-phase-04$ narrate  ;

: battle-phase-05$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 05)
  \ XXX TODO -- unfinished?
  ^the-enemy|enemies s{
  s" está* haciendo retroceder a" your-soldiers$ s&
  s" está* obligando a" your-soldiers$ s& s" a retroceder" s&
  }s rot *>verb-ending s&
  step-by-step$ s& period+  ;

: battle-phase-05  ( -- )
  \ Combate (fase 05).
  battle-phase-05$ narrate  ;

: battle-phase-06$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 06)
  \ XXX TODO -- unfinished
  ^the-enemy|enemies s{
  s" va* ganando" field$ s&
  s" va* adueñándose del terreno"
  s" va* conquistando" field$ s&
  s" se va* abriendo paso"
  }s rot *>verb-ending s&
  step-by-step$ s& period+  ;

: battle-phase-06  ( -- )
  \ Combate (fase 06).
  battle-phase-06$ narrate  ;

: battle-phase-07$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 07)
  ^your-soldiers$
  s{ s" caen" s" van cayendo," }s&
  s" uno tras otro," s?&
  s{ s" vendiendo cara su vida" s" defendiéndose" }s&
  like-heroes$ s& period+  ;

: battle-phase-07  ( -- )
  \ Combate (fase 07).
  battle-phase-07$ narrate  ;

: battle-phase-08$  ( -- ca len )
  \ Devuelve la descripción del combate (fase 08)
  ^the-enemy|enemies
  s{ s" aplasta* a" s" acaba* con" }s
  rot *>verb-ending s&
  s" los últimos de" s" entre" s?& s&
  your-soldiers$ s& s" que," s&
  s{  s" heridos" s{ s" extenuados" s" exhaustos" s" agotados" }s both?
      s{ s" apurando" s" con" }s s" sus" s& last-energy(fp)$ s&
      s" con su" last$ s& s" aliento" s&
      s" haciendo un" last$ s& s" esfuerzo" s&
  }s& comma+ still$ s&
  s{  s" combaten" s" resisten"
      s{ s" se mantienen" s" aguantan" s" pueden mantenerse" }s
      s" en pie" s&
      s{ s" ofrecen" s" pueden ofrecer" }s s" alguna" s?&
      s" resistencia" s&
  }s& period+  ;

: battle-phase-08  ( -- )
  \ Combate (fase 08).
  battle-phase-08$ narrate  ;

create 'battle-phases  \ Tabla para las fases del combate
here \ Dirección libre actual, para calcular después el número de fases
  \ Compilar la dirección de ejecución de cada fase:
  ' battle-phase-00 ,
  ' battle-phase-01 ,
  ' battle-phase-02 ,
  ' battle-phase-03 ,
  ' battle-phase-04 ,
  ' battle-phase-05 ,
  ' battle-phase-06 ,
  ' battle-phase-07 ,
  ' battle-phase-08 ,
here swap - cell / constant battle-phases  \ Fases de la batalla
: (battle-phase)  ( u -- )
  \ Ejecuta una fase del combate.
  \ u = Fase del combate (la primera es la cero)
  cells 'battle-phases + perform  ;

: battle-phase  ( -- )
  \ Ejecuta la fase en curso del combate.
  battle# @ 1- (battle-phase)  ;

: battle-location?  ( -- f )
  \ ¿En el escenario de la batalla?
  my-location location-10% <  \ ¿Está el protagonista en un escenario menor que el 10?
  pass-still-open? 0=  and  ;  \ ¿Y el paso del desfiladero está cerrado?

: battle-phase++  ( -- )
  \ Incrementar la fase de la batalla (salvo una de cada diez veces, al azar).
  10 random if  battle# ++  then  ;

: battle  ( -- )
  \ Batalla y persecución.
  battle-location? ?? battle-phase
  pursue-location? ?? pursued
  battle-phase++  ;

: battle?  ( -- f )
  \ ¿Ha empezado la batalla?
  battle# @ 0>  ;

: the-battle-ends  ( -- )
  \ Termina la batalla.
  battle# off  ;

: the-battle-begins  ( -- )
  \ Comienza la batalla.
  1 battle# !  ;

\ ----------------------------------------------
\ Emboscada de los sajones

: the-pass-is-closed  ( -- )
  \ Cerrar el paso, la salida norte.
  no-exit location-08% ~north-exit !  ;

: a-group-of-saxons$  ( -- ca len )
  s" una partida" s{ s" de sajones" s" sajona" }s&  ;

: suddenly$  ( -- a u)
  s" de" s{ s" repente" s" pronto" }s&  ;

: suddenly|then$  ( -- ca len )
  s{ suddenly$ s" entonces" }s  ;

: the-ambush-begins  ( -- )
  \ Comienza la emboscada.
  s{  suddenly$ s" ," s?+ a-group-of-saxons$ s& s" aparece" s&
      a-group-of-saxons$  s" aparece" s& suddenly$ s&
  }s ^uppercase s" por el este." s&
  s" Para cuando" s&
  s{ s" te vuelves" s" intentas volver" }s&
  toward-the(m)$ s& s" norte," s&
  s" ya no" s& s{ s" te" s? s" queda" s& s" tienes" }s&
  s{ s" duda:" s" duda alguna:" s" ninguna duda:" }s&
  s{  s" es" s" se trata de"
      s{ s" te" s" os" }s s" han tendido" s&
  }s& s" una" s&
  s{ s" emboscada" s" celada" s" encerrona" s" trampa" }s&
  period+  narrate narration-break  ;

: they-win-0$  ( -- ca len )
  \ Devuelve la primera versión de la parte final de las palabras de los oficiales.
  s{  s" su" s{ s" victoria" s" triunfo" }s&
      s{ s" la" s" nuestra" }s s{ s" derrota" s" humillación" }s&
  }s s" será" s&{ s" doble" s" mayor" }s&  ;

: they-win-1$  ( -- ca len )
  \ Devuelve la segunda versión de la parte final de las palabras de los oficiales.
  s{  s" ganan" s" nos ganan" s" vencen" s" nos vencen"
      s" perdemos" s" nos derrotan" }s
  s{ s" doblemente" s" por partida doble" }s&  ;

: they-win$  ( -- ca len )
  \ Devuelve la parte final de las palabras de los oficiales.
  they-win-0$ they-win-1$ 2 schoose period+  ;

: taking-prisioner$  ( -- ca len )
  \ Devuelve una parte de las palabras de los oficiales.
  s" si" s{ s" capturan" s" hacen prisionero" s" toman prisionero" }s&  ;

: officers-speach  ( -- )
  \ Palabras de los oficiales.
  sire,$ s?  dup taking-prisioner$
  rot 0= ?? ^uppercase s&
  s" a un general britano" s& they-win$ s&  speak  ;

: officers-talk-to-you  ( -- )
  \ Los oficiales hablan con el protagonista.
  s" Tus oficiales te"
  s{ s" conminan a huir"
  s" conminan a ponerte a salvo"
  s" piden que te pongas a salvo"
  s" piden que huyas" }s& colon+ narrate
  officers-speach
  s{ s" Sabes" s" Comprendes" }s s" que" s&
  s{  s" es cierto" s{ s" tienen" s" llevan" }s s" razón" s&
      s" están en lo cierto" }s& comma+
  s{  but$ s{ s" a pesar de ello" s" aun así" }s?&
      s" y"
  }s& s" te duele" s&

  period+ narrate  ;

: the-enemy-is-stronger$  ( -- ca len )
  \ Mensaje de que el enemigo es superior.
  s" En el" narrow(m)$ s& s" paso es posible" s&
  s{ s" resistir," s" defenderse," }s& but$ s&
  s{ s" por desgracia" s" desgraciadamente" }s&
  s{
    s{ s" los sajones" s" ellos" }s s" son" s&
      s{  s" muy" s? s" superiores en número" s&
          s" mucho" s? s" más numerosos" s&
      }s&
    s" sus tropas son" s" mucho" s?& s" más numerosas que las tuyas" s&
    s" sus" s{ s" hombres" s" soldados" }s&
      s" son" s& s" mucho" s?& s" más numerosos que los tuyos" s&
  }s&  ;

: the-enemy-is-stronger  ( -- )
  \ El enemigo es superior.
  the-enemy-is-stronger$ period+ narrate scene-break  ;

: ambush  ( -- )
  \ Emboscada.
  the-pass-is-closed
  the-ambush-begins
  the-battle-begins
  the-enemy-is-stronger
  officers-talk-to-you  ;

\ ----------------------------------------------
\ Oscuridad en la cueva

: considering-the-darkness$  ( -- ca len )
  s" Ante" s" el muro de" s?& s" la reinante" s&
  s{ s" e intimidante" s" e impenetrable" s" e infranqueable" s" y sobrecogedora" }s&
  s" oscuridad," s&  ;

: you-go-back$  ( -- ca len )
  s{  s{  s" prefieres" s" decides" s" eliges" s" optas por"
          s{ s" no te queda" s" no tienes" }s
            s" otra" s&{ s" opción" s" alternativa" }s& s" que" s&
          s" no puedes hacer"
            s{ s" sino" s" otra cosa que" s" más que" }s&
          s{ s" no te queda" s" no tienes" }s s{ s" otro" s" más" }s&
            s{ s" remedio" s" camino" }s& s" que" s&
      }s{ s" volver atrás" s" retroceder" }s&
      s{ s" vuelves atrás" s" retrocedes" }s s" sin remedio" s?&
  }s{ "" s" unos pasos" s" sobre tus pasos" }s&  ;

: to-the-place-where$  ( -- a u)
  s" hasta" s" el lugar" s? dup if  s" de la cueva" s?&  then s&
  s" donde" s&  ;

: to-see-something$  ( -- ca len )
  s" ver" s" algo" s?&  ;

: there-is-some-light$  ( -- ca len )
  still$ s? s{ s" hay" s" llega" s" entra" s" penetra" s" se filtra" }s&
  s{  s" un mínimo de" s" una mínima" s" cierta"
      s" algo de" s" suficiente" s" bastante"
  }s& s{ s" luz" s" claridad" s" luminosidad" }s&
  that-(at-least)$ s" permite" s& to-see-something$ s& s?&  ;

: sun-adjectives$  ( -- ca len )
  \ XXX TODO -- hacer que seleccione uno o dos adjetivos
  \ s" tímido" s" débil" s" triste" s" lejano"
  ""  ;

: there-are-some-sun-rays$  ( -- ca len )
  still$ s? s{ s" llegan" s" entran" s" penetran" s" se filtran" }s&
  s" alg" s? s" unos" s+ s" pocos" s?& s&
  s" rayos de" s&{ s" luz" sun-adjectives$ s" sol" s& }s&
  that-(at-least)$ s" permiten" s& to-see-something$ s& s?&  ;

: it's-possible-to-see$  ( -- ca len )
  s{ s" se puede" s" puedes" s" es posible" }s s" ver" s& s" algo" s?&  ;

: dark-cave  ( -- )
  \ En la cueva y sin luz.
  new-page
  considering-the-darkness$ you-go-back$ s& to-the-place-where$ s&
  s{  there-is-some-light$
      there-are-some-sun-rays$
      it's-possible-to-see$
  }s& period+ narrate  ;

\ ----------------------------------------------
\ Albergue de los refugiados

: the-old-man-is-angry?  ( -- f )
  \ ¿El anciano se enfada porque llevas algo prohibido?
  stone% is-accessible?
  sword% is-accessible?  or  ;

: he-looks-at-you-with-anger$  ( -- ca len )
  \ Texto de que el líder de los refugiados te mira.
  s" parece sorprendido y" s?
  s{
  s" te mira" s{ s" con dureza" s" con preocupación" }s&
  s" te dirige una dura mirada"
  s" dirige su mirada hacia ti"
  }s&  ;

: he-looks-at-you-with-calm$  ( -- ca len )
  \ Texto de que el líder de los refugiados te mira.
  s" advierte tu presencia y" s?
  s{ s" por un momento" s" durante unos instantes" }s?&
  s" te" s&{ s" observa" s" contempla" }s&
  s{ s" con serenidad" s" con expresión serena" s" en calma" s" sereno" }s&  ;

: the-leader-looks-at-you$  ( -- ca len )
  \ Texto de que el líder de los refugiados te mira.
  leader% ^full-name  the-old-man-is-angry?
  if  he-looks-at-you-with-anger$
  else  he-looks-at-you-with-calm$
  then  s& period+  ;

: the-refugees-surround-you$  ( -- ca len )
  \ Descripción de la actitud de los refugiados.
  ^the-refugees$
  location-28% has-east-exit?
  if  they-let-you-pass$
  else  they-don't-let-you-pass$
  then  period+ s&  ;

\ }}} ==========================================================
section( Tramas asociadas a lugares)  \ {{{

\ XXX TODO -- convertir las tramas que corresponda
\ de :after-describing-location
\ en :before-describing-location

location-01% :after-describing-location
  soldiers% is-here
  still-in-the-village?
  if  celebrating  else  going-home  then
  ;after-describing-location
location-02% :after-describing-location
  \ Decidir hacia dónde conduce la dirección hacia abajo
  [false] [if]  \ Primera versión
    \ Decidir al azar:
    self% location-01% location-03% 2 choose d-->
  [else]  \ Segunda versión mejorada
    \ Decidir según el escenario de procedencia:
    self%
    protagonist% previous-location location-01% =  \ ¿Venimos de la aldea?
    if  location-03%  else  location-01%  then  d-->
  [then]
  soldiers% is-here going-home
  ;after-describing-location
location-03% :after-describing-location
  soldiers% is-here going-home
  ;after-describing-location
location-04% :after-describing-location
  soldiers% is-here going-home
  ;after-describing-location
location-05% :after-describing-location
  soldiers% is-here going-home
  ;after-describing-location
location-06% :after-describing-location
  soldiers% is-here going-home
  ;after-describing-location
location-07% :after-describing-location
  soldiers% is-here going-home
  ;after-describing-location
location-08% :after-describing-location
  soldiers% is-here
  going-home
  pass-still-open? ?? ambush
  ;after-describing-location
location-09% :after-describing-location
  soldiers% is-here
  going-home
  ;after-describing-location
location-10% :after-describing-location
  s" entrada a la cueva" cave-entrance% fs-name!
  cave-entrance% familiar++
  location-08% my-previous-location = if  \ Venimos del exterior
    self% visits
    if  ^again$  else  ^finally$ s" ya" s?&  then
    \ XXX TODO -- ampliar con otros textos alternativos
    you-think-you're-safe$ s&
    but-it's-an-impression$ s?+
    period+ narrate
  \ XXX TODO -- si venimos del interior, mostrar otros textos
  then
  ;after-describing-location
location-11% :after-describing-location
  lake% is-here
  ;after-describing-location
location-16% :after-describing-location
  s" En la distancia, por entre los resquicios de las rocas,"
  s" y allende el canal de agua, los sajones" s&
  s{ s" intentan" s" se esfuerzan en" s" tratan de" s" se afanan en" }s&
  s{ s" hallar" s" buscar" s" localizar" }s&
  s" la salida que encontraste por casualidad." s&
  narrate
  ;after-describing-location
location-20% :can-i-enter-location?  ( -- f )
  location-17% am-i-there? no-torch? and
  dup 0= swap ?? dark-cave
  ;can-i-enter-location?
location-28% :after-describing-location
  self% no-exit e-->  \ Cerrar la salida hacia el este
  recent-talks-to-the-leader off
  refugees% is-here
  the-refugees-surround-you$ narrate
  the-leader-looks-at-you$ narrate
  ;after-describing-location
location-29% :after-describing-location
  refugees% is-here  \ Para que sean visibles en la distancia
  ;after-describing-location
location-31% :after-describing-location
  \ XXX TODO -- mover a la descripción?
  self% has-north-exit? if
    s" Las rocas yacen desmoronadas a lo largo del"
    pass-way$ s& period+
  else
    s" Las rocas" (they)-block$ s& s" el paso." s&
  then  narrate
  ;after-describing-location
location-38% :after-describing-location
  lake% is-here
  ;after-describing-location
location-43% :after-describing-location
  snake% is-here? if
    a-snake-blocks-the-way$ period+
    narrate
  then
  ;after-describing-location
location-44% :after-describing-location
  lake% is-here
  ;after-describing-location
location-47% :after-describing-location
  door% is-here
  ;after-describing-location
location-48% :after-describing-location
  door% is-here
  ;after-describing-location

\ }}} ==========================================================
section( Trama global)  \ {{{

\ ----------------------------------------------
\ Varios

: (lock-found)  ( -- )
  \ Encontrar el candado (al mirar la puerta o al intentar abrirla).
  door% location lock% is-there
  lock% familiar++
  ;  ' (lock-found) is lock-found

\ ----------------------------------------------
\ Gestor de la trama global

: plot  ( -- )
  \ Trama global.
  \ Nota: Las subtramas deben comprobarse en orden cronológico:
  \ XXX TODO -- la trama de la batalla sería adecuada para una trama global de escenario,
  \ invocada desde aquí. Aquí quedarían solo las tramas generales que no dependen de
  \ ningún escenario.
  battle? if  battle exit  then  ;

\ }}} ==========================================================
section( Descripciones especiales)  \ {{{

\ Esta sección contiene palabras que muestran descripciones
\ que necesitan un tratamiento especial porque hacen
\ uso de palabras relacionadas con la trama.
\
\ En lugar de crear vectores para las palabras que estas
\ descripciones utilizan, es más sencillo crearlos para las
\ descripciones y definirlas aquí, a continuación de la trama.

: officers-forbid-to-steal$  ( -- )
  \ Devuelve una variante de «Tus oficiales detienen el saqueo».
  s{ s" los" s" tus" }s s" oficiales" s&
  s{
  s" intentan detener" s" detienen como pueden"
  s" hacen" s{ s" todo" s? s" lo que pueden" s& s" lo imposible" }s&
    s{ s" para" s" por" }s& s" detener" s&
  }s& s{ s" el saqueo" 2dup s" el pillaje" }s&  ;

: ^officers-forbid-to-steal$  ( -- )
  \ Devuelve una variante de «Tus oficiales detienen el saqueo» (con la primera mayúscula).
  officers-forbid-to-steal$ ^uppercase  ;

: (they-do-it)-their-way$  ( -- ca len )
  s" ," s{
    s" a su" s{ s" manera" s" estilo" }s&
    s" de la única" way$ s&
    s" que" s& s{ s" saben" s" conocen" }s&
  }s& comma+  ;

: this-sad-victory$  ( -- ca len )
  s" esta" s" tan" s{ s" triste" s" fácil" s" poco honrosa" }s&
  s" victoria" rnd2swap s& s&  ;

: (soldiers-steal$)  ( ca1 len1 -- ca2 len2 )
  \ Completa una descripción de tus soldados en la aldea arrasada.
  soldiers$ s& s{ s" aún" s" todavía" }s?&
  s{ s" celebran" s{ s" están" s" siguen" s" continúan" }s s" celebrando" s& }s&
  (they-do-it)-their-way$ s?+
  this-sad-victory$ s& s{ s" :" s" ..." }s+
  s{ s" saqueando" s" buscando" s" apropiándose de" s" robando" }s&
  s" todo" s?& s" cuanto de valor" s&
  s" aún" s?& s{ s" quede" s" pueda quedar" }s&
  s" entre" s& rests-of-the-village$ s&  ;

: soldiers-steal$  ( -- ca len )
  \ Devuelve una descripción de tus soldados en la aldea arrasada.
  all-your$ (soldiers-steal$)  ;

: ^soldiers-steal$  ( -- ca len )
  \ Devuelve una descripción de tus soldados en la aldea arrasada (con la primera mayúscula).
  ^all-your$ (soldiers-steal$)  ;

: soldiers-steal-spite-of-officers-0$  ( -- ca len )
  \ Devuelve la primera versión de la descripción de los soldados en la aldea.
  ^soldiers-steal$ period+
  ^officers-forbid-to-steal$ s&  ;

: soldiers-steal-spite-of-officers-1$  ( -- ca len )
  \ Devuelve la segunda versión de la descripción de los soldados en la aldea.
  ^soldiers-steal$
  s{ s" , mientras" s" que" s?&
  s{ s" ; mientras" s" . Mientras" }s s" tanto" s?& comma+
  s" . Al mismo tiempo," }s+
  officers-forbid-to-steal$ s&  ;

: soldiers-steal-spite-of-officers-2$  ( -- ca len )
  \ Devuelve la tercera versión de la descripción de los soldados en la aldea.
  \ XXX TODO -- not used. la frase queda incoherente en algunos casos
  ^officers-forbid-to-steal$
  s" , pero" s+ s" a pesar de ello" s?&
  soldiers-steal$ s&  ;

: soldiers-steal-spite-of-officers$  ( -- ca len )
  \ Devuelve una descripción de tus soldados en la aldea arrasada.
  ['] soldiers-steal-spite-of-officers-0$
  ['] soldiers-steal-spite-of-officers-1$
  2 choose execute  ;

: soldiers-steal-spite-of-officers  ( -- )
  \ Describe a tus soldados en la aldea arrasada.
  soldiers-steal-spite-of-officers$ period+ paragraph  ;

: will-follow-you-forever$  ( -- )
  \ Describe a tus hombres durante el regreso a casa, sin citarlos.
  s" te seguirían hasta el"
  s{ s{ s" mismo" s" mismísimo" }s s" infierno" s&
  s" último rincón de la Tierra"
  }s&  ;

: will-follow-you-forever  ( ca len -- )
  \ Completa e imprime la descripción de soldados u oficiales.
  \ ca len = Sujeto de la frase
  will-follow-you-forever$ s& period+ paragraph  ;

: soldiers-go-home  ( -- )
  \ Describe a tus soldados durante el regreso a casa.
  ^all-your$ soldiers$ s& will-follow-you-forever  ;

: officers-go-home  ( -- )
  \ Describe a tus soldados durante el regreso a casa.
  ^all-your$ officers$ s&
  s" , como"
  s{ s" el resto de tus" all-your$ }s& soldiers$ s& comma+ s?+
  will-follow-you-forever  ;

: (soldiers-description)  ( -- )
  \ Describe a tus soldados.
  true case
    still-in-the-village? of  soldiers-steal-spite-of-officers  endof
\   back-to-the-village? of  soldiers-go-home  endof  \ XXX TODO -- not used
    pass-still-open? of  soldiers-go-home  endof
\   battle? of  battle-phase  endof  \ XXX TODO -- not used. redundante, porque tras la descripción se mostrará otra vez la situación de la batalla
  endcase  ;

' (soldiers-description) is soldiers-description
: (officers-description)  ( -- )
  \ Describe a tus soldados.
  true case
    still-in-the-village? of  ^officers-forbid-to-steal$  endof
\   back-to-the-village? of  officers-go-home  endof  \ XXX TODO -- not used
    pass-still-open? of  officers-go-home  endof
\   battle? of  battle-phase  endof  \ XXX TODO -- not used. redundante, porque tras la descripción se mostrará otra vez la situación de la batalla
  endcase  ;

' (officers-description) is officers-description

\ }}} ==========================================================
section( Errores del intérprete de comandos)  \ {{{

: please$  ( -- ca len )
  \ Devuelve «por favor» o vacía.
  s" por favor" s?  ;

: (please&)  ( ca1 len1 ca2 len2 -- ca3 len3 )
  \ Añade una cadena al inicio o al final de otra, con una coma de separación.
  \ ca1 len1 = Cadena principal
  \ ca2 len2 = Cadena que se añadirá a la principal
  2 random ?? 2swap
  comma+ 2swap s&  ;

: please&  ( ca1 len1 -- ca1 len1 | ca2 len2 )
  \ Añade «por favor» al inicio o al final de una cadena, con una coma de separación; o la deja sin tocar.
  please$ dup if  (please&)  else  2drop  then  ;

: in-the-sentence$  ( -- ca len )
  \ Devuelve una variante de «en la frase» (o una cadena vacía).
  s{ "" s" en la frase" s" en el comando" s" en el texto" }s  ;

: error-comment-0$  ( -- ca len )
  \ Devuelve la variante 0 del mensaje de acompañamiento para los errores lingüísticos.
  s" sé más clar" player-gender-ending$+  ;

: error-comment-1$  ( -- ca len )
  \ Devuelve la variante 1 del mensaje de acompañamiento para los errores lingüísticos.
  s{ s" exprésate" s" escribe" }s
  s{
  s" más claramente"
  s" más sencillamente"
  s{ s" con más" s" con mayor" }s
  s{ s" sencillez" s" claridad" }s&
  }s s&  ;

: error-comment-2$  ( -- ca len )
  \ Devuelve la variante 2 del mensaje de acompañamiento para los errores lingüísticos.
  \ Comienzo común:
  s{ s" intenta" s" procura" s" prueba a" }s
  s{ s" reescribir" s" expresar" s" escribir" s" decir" }s&
  \ XXX TODO -- este "lo" crea problema de concordancia con el final de la frase:
  s{ s"  la frase" s" lo" s"  la idea" }s+
  s{
  \ Final 0:
  s{ s" de" s" otra" }s way$ s&?
  s{ "" s" un poco" s" algo" }s& s" más" s&
  s{ s" simple" s" sencilla" s" clara" }s&
  \ Final 1:
  s{ s" más claramente" s" con más sencillez" }s
  }s&  ;

: error-comment$  ( -- ca len )
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos.
  error-comment-0$ error-comment-1$ error-comment-2$
  3 schoose please&  ;

: ^error-comment$  ( -- ca len )
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula.
  error-comment$ ^uppercase  ;

: language-error-specific-message  ( ca len -- )
  \ Muestra un mensaje detallado sobre un error lingüístico,
  \ combinándolo con una frase común.
  \ ca len = Mensaje de error detallado
  \ XXX TODO -- hacer que use coma o punto y coma, al azar
  in-the-sentence$ s&  3 random
  if    ^uppercase period+ ^error-comment$
  else  ^error-comment$ comma+ 2swap
  then  period+ s&  (language-error)  ;

: language-error-general-message$  ( -- ca len )
  \ Devuelve el mensaje de error lingüístico para el nivel 1.
  'language-error-general-message$ count  ;

: language-error-general-message  ( ca len -- )
  \ Muestra el mensaje de error lingüístico para el nivel 1.
  \ ca len = Mensaje de error detallado
  2drop language-error-general-message$ (language-error)  ;

create 'language-error-verbosity-xt
  ' 2drop ,  \ Verbosity 0
  ' language-error-general-message ,  \ Verbosity 1
  ' language-error-specific-message ,  \ Verbosity 2
: language-error  ( ca len -- )
  \ Muestra un mensaje sobre un error lingüístico,
  \ detallado o breve según la configuración.
  \ ca len = Mensaje de error detallado
  'language-error-verbosity-xt
  language-errors-verbosity @ cells + perform  ;

: there-are$  ( -- ca len )
  \ Devuelve una variante de «hay» para sujeto plural, comienzo de varios errores.
  s{ s" parece haber" s" se identifican" s" se reconocen" }s  ;

: there-is$  ( -- ca len )
  \ Devuelve una variante de «hay» para sujeto singular, comienzo de varios errores.
  s{ s" parece haber" s" se identifica" s" se reconoce" }s  ;

: there-is-no$  ( -- ca len )
  \ Devuelve una variante de «no hay», comienzo de varios errores.
  s" no se" s{ s" identifica" s" encuentra" s" reconoce" }s&
  s{ s" el" s" ningún" }s&  ;

: too-many-actions  ( -- )
  \ Informa de que se ha producido un error porque hay dos verbos en el comando.
  s{ there-are$ s" dos verbos" s&
  there-is$ s" más de un verbo" s&
  there-are$ s" al menos dos verbos" s&
  }s  language-error  ;

' too-many-actions constant (too-many-actions-error#)
' (too-many-actions-error#) is too-many-actions-error#
: too-many-complements  ( -- )
  \ Informa de que se ha producido un error
  \ porque hay dos complementos secundarios en el comando.
  \ XXX TMP
  s{
  there-are$
  s" dos complementos secundarios" s&
  there-is$
  s" más de un complemento secundario" s&
  there-are$
  s" al menos dos complementos secundarios" s&
  }s  language-error  ;

' too-many-complements constant (too-many-complements-error#)
' (too-many-complements-error#) is too-many-complements-error#
: no-verb  ( -- )
  \ Informa de que se ha producido un error por falta de verbo en el comando.
  there-is-no$ s" verbo" s& language-error  ;

' no-verb constant (no-verb-error#)
' (no-verb-error#) is no-verb-error#
: no-main-complement  ( -- )
  \ Informa de que se ha producido un error por falta de complemento principal en el comando.
  there-is-no$ s" complemento principal" s& language-error  ;

' no-main-complement constant (no-main-complement-error#)
' (no-main-complement-error#) is no-main-complement-error#
: unexpected-main-complement  ( -- )
  \ Informa de que se ha producido un error
  \ por la presencia de complemento principal en el comando.
  there-is$ s" un complemento principal" s&
  s" pero el verbo no puede llevarlo" s&
  language-error  ;

' unexpected-main-complement constant (unexpected-main-complement-error#)
' (unexpected-main-complement-error#) is unexpected-main-complement-error#
: unexpected-secondary-complement  ( -- )
  \ Informa de que se ha producido un error
  \ por la presencia de complemento secundario en el comando.
  there-is$ s" un complemento secundario" s&
  s" pero el verbo no puede llevarlo" s&
  language-error  ;

' unexpected-secondary-complement constant (unexpected-secondary-complement-error#)
' (unexpected-secondary-complement-error#) is unexpected-secondary-complement-error#
: not-allowed-main-complement  ( -- )
  \ Informa de que se ha producido un error
  \ por la presencia de un complemento principal en el comando
  \ que no está permitido.
  there-is$ s" un complemento principal no permitido con esta acción" s&
  language-error  ;

' not-allowed-main-complement constant (not-allowed-main-complement-error#)
' (not-allowed-main-complement-error#) is not-allowed-main-complement-error#
: not-allowed-tool-complement  ( -- )
  \ Informa de que se ha producido un error
  \ por la presencia de un complemento instrumental en el comando
  \ que no está permitido.
  there-is$ s" un complemento principal no permitido con esta acción" s&
  language-error  ;

' not-allowed-tool-complement constant (not-allowed-tool-complement-error#)
' (not-allowed-tool-complement-error#) is not-allowed-tool-complement-error#
: useless-tool  ( -- )
  \ Informa de que se ha producido un error
  \ porque una herramienta no especificada no es la adecuada.
  \ XXX TODO -- unfinished
  s" [Con eso no puedes]"
  narrate  ;

' useless-tool constant (useless-tool-error#)
' (useless-tool-error#) is useless-tool-error#
: useless-what-tool  ( -- )
  \ Informa de que se ha producido un error
  \ porque el ente `what` no es la herramienta adecuada.
  \ XXX TODO -- unfinished
  \ XXX TODO -- distinguir si la llevamos, si está presente, si es conocida...
  s" [Con" what @ full-name s& s" no puedes]" s&
  narrate  ;

' useless-what-tool constant (useless-what-tool-error#)
' (useless-what-tool-error#) is useless-what-tool-error#
: unresolved-preposition  ( -- )
  \ Informa de que se ha producido un error
  \ porque un complemento (seudo)preposicional quedó incompleto.
  there-is$ s" un complemento (seudo)preposicional sin completar" s&
  language-error  ;

' unresolved-preposition constant (unresolved-preposition-error#)
' (unresolved-preposition-error#) is unresolved-preposition-error#
: repeated-preposition  ( -- )
  \ Informa de que se ha producido un error por
  \ la repetición de una (seudo)preposición.
  there-is$ s" una (seudo)preposición repetida" s&
  language-error  ;

' repeated-preposition constant (repeated-preposition-error#)
' (repeated-preposition-error#) is repeated-preposition-error#

: ?wrong  ( xt | 0 -- )
  \ Informa, si es preciso, de un error en el comando.
  \ xt = Dirección de ejecución de la palabra de error (que se usa también como código del error)
  \ [debug-catch] [debug-parsing] [or] [??] ~~
  ?dup ?? execute
  \ [debug-catch] [debug-parsing] [or] [??] ~~
  ;

\ }}} ==========================================================
section( Herramientas para crear las acciones)  \ {{{

\ ----------------------------------------------
subsection( Pronombres)  \ {{{

\ XXX TODO:
\ Mover esto a la sección del intérprete.

variable last-action  \ Última acción utilizada por el jugador

\ La tabla `last-complement` que crearemos a continuación sirve para
\ guardar los identificadores de entes correspondientes a los últimos
\ complementos utilizados en los comandos del jugador. De este modo los
\ pronombres podrán recuperarlos.
\
\ Necesita cinco celdas: una para el último complemento usado y cuatro
\ para cada último complemento usado de cada género y número.  El espacio
\ se multiplica por dos para guardar en la segunda mitad los penúltimos
\ complementos.
\
\ La estructura de la tabla es la siguiente, con desplazamientos
\ indicados en celdas:

\ Último complemento usado:
\   +0 De cualquier género y número.
\   +1 Masculino singular.
\   +2 Femenino singular.
\   +3 Masculino plural.
\   +4 Femenino plural.

\ Penúltimo complemento usado:
\   +5 De cualquier género y número.
\   +6 Masculino singular.
\   +7 Femenino singular.
\   +8 Masculino plural.
\   +9 Femenino plural.

create last-complement  \ Tabla para últimos complementos usados
5 cells 2*  \ Octetos necesarios para toda la tabla
dup constant /last-complements  allot

\ Desplazamientos para acceder a los elementos de la tabla:
1 cells constant />masculine-complement  \ Respecto al inicio de tabla
2 cells constant />feminine-complement  \ Respecto al inicio de tabla
0 cells constant />singular-complement  \ Respecto a su género en singular
2 cells constant />plural-complement  \ Respecto a su género en singular
5 cells constant />but-one-complement  \ Respecto a la primera mitad de la tabla
: >masculine  ( a1 -- a2 )  />masculine-complement +  ;
: >feminine  ( a1 -- a2 )  />feminine-complement +  ;
: >singular  ( a1 -- a2 )  />singular-complement +  ;
: >plural  ( a1 -- a2 )  />plural-complement +  ;
: >but-one  ( a1 -- a2 )  />but-one-complement +  ;

: last-but-one-complement  ( - a )
  \ Devuelve la dirección del penúltimo complemento absoluto,
  \ que es también el inicio de la sección «penúltimos»
  \ de la tabla `last-complements`.
  last-complement >but-one  ;

: (>last-complement)  ( a1 a2 -- a3 )
  \ Apunta a la dirección adecuada para un ente
  \ en una sección de la tabla `last-complement`,
  \ bien «últimos» o «penúltimos».
  \ Nota: Hace falta sumar los desplazamientos de ambos géneros
  \ debido a que ambos son respecto al inicio de la tabla.
  \ El desplazamiento para singular no es necesario,
  \ pues sabemos que es cero, a menos que se cambie la estructura.
  \ a1 = Ente para el que se calcula la dirección
  \ a2 = Dirección de una de las secciones de la tabla
  over has-feminine-name? />feminine-complement and +
  over has-masculine-name? />masculine-complement and +
  swap has-plural-name? />plural-complement and +  ;

: >last-complement  ( a1 -- a2 )
  \ Apunta a la dirección adecuada para un ente
  \ en la sección «últimos» de la tabla `last-complement`.
  last-complement (>last-complement)  ;

: >last-but-one-complement  ( a1 -- a2 )
  \ Apunta a la dirección adecuada para un ente
  \ en la sección «penúltimos» de la tabla `last-complement`.
  last-but-one-complement (>last-complement)  ;

: erase-last-command-elements  ( -- )
  \ Borra todos los últimos elementos guardados de los comandos.
  last-action off
  last-complement /last-complements erase  ;

\ }}}---------------------------------------------
subsection( Herramientas para la creación de acciones)  \ {{{

\ Los nombres de las acciones empiezan por el prefijo «do-»
\ (algunas palabras secundarias de las acciones
\ también usan el mismo prefijo).

\ XXX TODO -- explicación sobre la sintaxis

: action:  ( "name" -- )
  \ Crear un identificador de acción.
  \ "name" = nombre del identificador de la acción, en el flujo de entrada
  create  ['] noop ,
  does>  ( pfa -- )  perform  ;

: :action  ( "name" -- )
  \ Inicia la definición de una palabra que ejecutará una acción.
  \ "name" = nombre del identificador de la acción, en el flujo de entrada
  :noname  \ Crea una palabra sin nombre para la acción
  4 roll  ( xt )
  \ Guardar la dirección de ejecución
  \ en el campo de datos del identificador de la acción:
  ' >body !  ;

: ;action  ( -- )
  postpone ;
  ;  immediate

\ }}}---------------------------------------------
subsection( Comprobación de los requisitos de las acciones)  \ {{{

\ En las siguientes palabras usamos las llaves en sus nombres
\ como una notación, para hacer más legible y más fácil de
\ modificar el código.  El texto entre las llaves indica la
\ condición que se ha de cumplir.
\
\ Si la condición no se cumple, se provocará un error con
\ `throw` que devolverá el flujo al último `catch`.
\
\ Este sistema de filtros y errores permite simplificar el
\ código de las acciones porque ahorra muchas estructuras
\ condicionales anidadas.

: main-complement{forbidden}  ( -- )
  \ Provoca un error si hay complemento principal.
  main-complement @
  0<> unexpected-main-complement-error# and throw  ;

: secondary-complement{forbidden}  ( -- )
  \ Provoca un error si hay complemento secundario.
  secondary-complement @
  0<> unexpected-secondary-complement-error# and throw  ;

: main-complement{required}  ( -- )
  \ Provoca un error si no hay complemento principal.
  main-complement @
  0= no-main-complement-error# and throw  ;

: main-complement{this-only}  ( a -- )
  \ Provoca un error si hay complemento principal y no es el indicado.
  \ a = Ente que será aceptado como complemento
  main-complement @ swap over different?
  not-allowed-main-complement-error# and throw  ;

: different-tool?  ( a -- f )
  \ ¿Es el ente diferente a la herramienta usada, si la hay?
  \ a = Ente
  tool-complement @ swap over different?  ;

: different-actual-tool?  ( a -- f )
  \ ¿Es el ente diferente a la herramienta estricta usada, si la hay?
  \ a = Ente
  actual-tool-complement @ swap over different?  ;

: tool-complement{this-only}  ( a -- )
  \ Provoca un error (lingüístico)
  \ si hay complemento instrumental y no es el indicado.
  \ a = Ente que será aceptado como complemento instrumental
  different-tool? not-allowed-tool-complement-error# and throw  ;

: actual-tool-complement{this-only}  ( a -- )
  \ Provoca un error (lingüístico)
  \ si hay complemento instrumental estricto y no es el indicado.
  \ a = Ente que será aceptado como complemento instrumental
  different-actual-tool? not-allowed-tool-complement-error# and throw  ;

: tool{not-this}  ( a -- )
  \ Provoca un error (narrativo) si se usa cierta herramienta.
  \ a = Ente que no será aceptado como herramienta
  \ XXX TODO -- not used
  dup what !
  different-tool? 0= useless-what-tool-error# and throw  ;

: actual-tool{not-this}  ( a -- )
  \ Provoca un error (narrativo) si se usa cierta herramienta estricta.
  \ a = Ente que no será aceptado como herramienta estricta
  \ XXX TODO -- not used
  dup what !
  different-actual-tool? 0= useless-what-tool-error# and throw  ;

: tool{this-only}  ( a -- )
  \ Provoca un error (narrativo) si no se usa cierta herramienta.
  \ a = Ente que será aceptado como herramienta
  tool-complement @ what !
  different-tool? useless-what-tool-error# and throw  ;

: actual-tool{this-only}  ( a -- )
  \ Provoca un error (narrativo) si no se usa cierta herramienta estricta.
  \ a = Ente que será aceptado como herramienta estricta
  actual-tool-complement @ what !
  different-actual-tool? useless-what-tool-error# and throw  ;

: tool-complement{unnecessary}  ( -- )
  \ Provoca un error si hay un complemento instrumental.
  tool-complement @ ?dup ?? unnecessary-tool  ;

: actual-tool-complement{unnecessary}  ( -- )
  \ Provoca un error si hay un complemento instrumental estricto.
  actual-tool-complement @ ?dup ?? unnecessary-tool  ;

: tool-complement{unnecessary-for-that}  ( ca len -- )
  \ Provoca un error si hay un complemento instrumental.
  \ ca len = Acción para la que sobra el complemento
  \       (una frase con verbo en infinitivo)
  tool-complement @ ?dup
  if  unnecessary-tool-for-that  else  2drop  then  ;

: actual-tool-complement{unnecessary-for-that}  ( ca len -- )
  \ Provoca un error si hay un complemento instrumental estricto.
  \ ca len = Acción para la que sobra el complemento
  \       (una frase con verbo en infinitivo)
  actual-tool-complement @ ?dup
  if  unnecessary-tool-for-that  else  2drop  then  ;

: {hold}  ( a -- )
  \ Provoca un error si un ente no está en inventario.
  dup what !
  is-hold? 0= you-do-not-have-what-error# and throw  ;

: ?{hold}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está en inventario.
  ?dup ?? {hold}  ;

: main-complement{hold}  ( -- )
  \ Provoca un error si el complemento principal existe y no está en inventario.
  main-complement @ ?{hold}  ;

: tool-complement{hold}  ( -- )
  \ Provoca un error si el complemento instrumental existe y no está en inventario.
  tool-complement @ ?{hold}  ;

: {not-hold}  ( a -- )
  \ Provoca un error si un ente está en inventario.
  dup what !
  is-hold? you-already-have-what-error# and throw  ;

: ?{not-hold}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y está en inventario.
  ?dup ?? {not-hold}  ;

: main-complement{not-hold}  ( -- )
  \ Provoca un error si el complemento principal existe y está en inventario.
  main-complement @ ?{not-hold}  ;

: {worn}  ( a -- )
  \ Provoca un error si un ente no lo llevamos puesto.
  dup what !
  is-worn-by-me? 0= you-do-not-wear-what-error# and throw  ;

: ?{worn}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no lo llevamos puesto.
  ?dup ?? {worn}  ;

: main-complement{worn}  ( -- )
  \ Provoca un error si el complemento principal existe y no lo llevamos puesto.
  main-complement @ ?{worn}  ;

: {open}  ( a -- )
  \ Provoca un error si un ente no está abierto.
  dup what !
  is-closed? what-is-already-closed-error# and throw  ;

: {closed}  ( a -- )
  \ Provoca un error si un ente no está cerrado.
  dup what !
  is-open? what-is-already-open-error# and throw  ;

: {not-worn}  ( a -- )
  \ Provoca un error si un ente lo llevamos puesto.
  dup what !
  is-worn-by-me? you-already-wear-what-error# and throw  ;

: ?{not-worn}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y lo llevamos puesto.
  ?dup ?? {not-worn}  ;

: main-complement{not-worn}  ( -- )
  \ Provoca un error si el complemento principal existe y lo llevamos puesto.
  main-complement @ ?{not-worn}  ;

: {cloth}  ( a -- )
  \ Provoca un error si un ente no se puede llevar puesto.
  is-cloth? 0= nonsense-error# and throw  ;

: ?{cloth}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no se puede llevar puesto.
  ?dup ?? {cloth}  ;

: main-complement{cloth}  ( -- )
  \ Provoca un error si el complemento principal existe y no se puede llevar puesto.
  main-complement @ ?{cloth}  ;

: {here}  ( a -- )
  \ Provoca un error si un ente no está presente.
  dup what !
  is-here? 0= is-not-here-what-error# and throw  ;

: ?{here}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está presente.
  ?dup ?? {here}  ;

: main-complement{here}  ( -- )
  \ Provoca un error si el complemento principal existe y no está presente.
  main-complement @ ?{here}  ;

: {accessible}  ( a -- )
  \ Provoca un error si un ente no está accessible.
  dup what !  is-not-accessible?  cannot-see-what-error# and throw  ;

: ?{accessible}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está accessible.
  ?dup ?? {accessible}  ;

: main-complement{accessible}  ( -- )
  \ Provoca un error si el complemento principal existe y no está accessible.
  main-complement @ ?{accessible}  ;

: {takeable}  ( a -- )
  \ Provoca un error si un ente no puede ser tomado.
  \ Nota: los errores apuntados por el campo ~TAKE-ERROR# no reciben parámetros salvo en WHAT
  dup what !
  dup take-error# throw  \ Error específico del ente
  can-be-taken? 0= nonsense-error# and throw  ;  \ Condición general de error

: ?{takeable}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no puede ser tomado.
  ?dup ?? {takeable}  ;

: main-complement{takeable}  ( -- )
  \ Provoca un error si el complemento principal existe y no puede ser tomado.
  main-complement @ ?{takeable}  ;

: {broken}  ( a -- )
  \ Provoca un error si un ente no puede ser roto.
  \ Nota: los errores apuntados por el campo ~BREAK-ERROR# no reciben parámetros salvo en WHAT
  dup what ! ~break-error# @ throw  ;

: ?{broken}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no puede ser roto.
  ?dup ?? {broken}  ;

: main-complement{broken}  ( -- )
  \ Provoca un error si el complemento principal existe y no puede ser roto.
  main-complement @ ?{broken}  ;

: {looked}  ( a -- )
  \ Provoca un error si un ente no puede ser mirado.
  \ Nota: los errores apuntados por el campo ~TAKE-ERROR# no deben necesitar parámetros, o esperarlo en WHAT
  dup what !
  can-be-looked-at? 0= cannot-see-what-error# and throw  ;

: ?{looked}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no puede ser mirado.
  ?dup ?? {looked}  ;

: main-complement{looked}  ( -- )
  \ Provoca un error si el complemento principal existe y no puede ser mirado.
  main-complement @ ?{looked}  ;

: {living}  ( a -- )
  \ Provoca un error si un ente no es un ser vivo.
  is-living-being? 0= nonsense-error# and throw  ;

: ?{living}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no es un ser vivo.
  ?dup ?? {living}  ;

: main-complement{living}  ( -- )
  \ Provoca un error si el complemento principal existe y no es un ser vivo.
  main-complement @ ?{living}  ;

: {needed}  ( a -- )
  \ Provoca un error si un ente no está en inventario, pues es necesario.
  dup what !
  is-hold? 0= you-need-what-error# and throw  ;

: ?{needed}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está en inventario, pues es necesario.
  ?dup ?? {needed}  ;

: main-complement{needed}  ( -- )
  \ Provoca un error si el complemento principal existe y no está en inventario, pues lo necesitamos.
  main-complement @ ?{needed}  ;

: {direction}  ( a -- )
  \ Provoca un error si un ente no es una dirección.
  dup what !
  is-direction? 0= nonsense-error# and throw  ;

: ?{direction}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no es una dirección.
  ?dup ?? {direction}  ;

: main-complement{direction}  ( -- )
  \ Provoca un error si el complemento principal existe y no es una dirección.
  main-complement @ ?{direction}  ;

\ }}}
\ }}} ==========================================================
section( Acciones)  \ {{{

\ XXX TODO -- usar `defer` en lugar de este sistema

\ Para crear una acción, primero es necesario crear su
\ identificador con la palabra `action:`, que funciona de forma
\ parecida a `defer`. Después hay que definir la palabra de la
\ acción con las palabras previstas para ello, que se ocupan
\ de darle al identificador el valor de ejecución
\ correspondiente. Ejemplo de la sintaxis:

\ action: identificador

\ :action identificador
\   \ definición de la acción
\   ;action

\ Todos los identificadores deben ser creados antes de las
\ definiciones, pues su objetivo es posibilitar que las
\ acciones se llamen unas a otras sin importar el orden en que
\ estén definidas en el código fuente.

\ ----------------------------------------------
subsection( Identificadores)  \ {{{

\ Acciones del juego

action: do-attack
action: do-break
action: do-climb
action: do-close
action: do-do
action: do-drop
action: do-examine
action: do-exits
action: do-frighten
action: do-go
action: do-go-ahead
action: do-go-back
action: do-go-down
action: do-go-east
action: do-go-in
action: do-go-north
action: do-go|do-break
action: do-go-out
action: do-go-south
action: do-go-up
action: do-go-west
action: do-hit
action: do-introduce-yourself
action: do-inventory
action: do-kill
action: do-look
action: do-look-to-direction
action: do-look-yourself
action: do-make
action: do-open
action: do-put-on
action: do-search
action: do-sharpen
action: do-speak
action: do-swim
action: do-take
action: do-take|do-eat \ XXX TODO -- cambiar do-eat por ingerir
action: do-take-off

\ }}}---------------------------------------------
subsection( Herramientas para averiguar complemento omitido)  \ {{{

: whom  ( -- a | 0 )
  \ Devuelve un ente personaje al que probablemente se refiera un comando.
  \ Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica.
  \ XXX TODO -- ampliar para contemplar los soldados y oficiales, según la trama, el escenario y la fase de la batalla
  true case
    ambrosio% is-here? of  ambrosio%  endof
    leader% is-here? of  leader%  endof
    false swap
  endcase  ;

: unknown-whom  ( -- a | 0 )
  \ Devuelve un ente personaje desconocido
  \ al que probablemente se refiera un comando.
  \ Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica
  true case
    ambrosio% is-here-and-unknown? of  ambrosio%  endof
    leader% is-here-and-unknown? of  leader%  endof
    false swap
  endcase  ;

\ }}}---------------------------------------------
subsection( Mirar, examinar y registrar)  \ {{{

: (do-look)  ( a -- )
  \ Mira un ente.
  dup describe
  dup is-location? ?? .present familiar++  ;

:action do-look
  \  Acción de mirar.
  tool-complement{unnecessary}
  main-complement @ ?dup 0= ?? my-location  \ Si falta el complemento principal, usar el escenario
  dup {looked} (do-look)
  ;action
:action do-look-yourself
  \  Acción de mirarse.
  tool-complement{unnecessary}
  main-complement @ ?dup 0= ?? protagonist%
  (do-look)
  ;action
:action do-look-to-direction
  \  Acción de otear.
  \ XXX TODO -- traducir «otear» en el nombre de la palabra
  tool-complement{unnecessary}
  main-complement{required}
  main-complement{direction}
  main-complement @ (do-look)
  ;action
:action do-examine
  \ Acción de examinar.
  \ XXX TMP
  \ XXX TODO -- implementar `x salida`
  do-look
  ;action
:action do-search
  \ Acción de registrar.
  \ XXX TMP
  do-look
  ;action

\ }}}---------------------------------------------
subsection( Salidas)  \ {{{

\ XXX Inacabado, no se usa
create do-exits-table-index  \ Tabla para desordenar el listado de salidas
#exits cells allot
\ Esta tabla permitirá que las salidas se muestren cada vez en un orden diferente

variable #free-exits  \ Contador de las salidas posibles
: no-exit$  ( -- ca len )
  \ Devuelve mensaje usado cuando no hay salidas que listar.
  s" No hay"
  s{ s" salidas" s" salida" s" ninguna salida" }s&  ;

: go-out$  ( -- ca len )
  s{ s" salir" s" seguir" }s  ;

: go-out-to& ( ca len -- ca1 len1 )
  go-out$ s& s" hacia" s&  ;

: one-exit-only$  ( -- ca len )
  \ Devuelve mensaje usado cuando solo hay una salidas que listar.
  s{
  s" La única salida" possible1$ s& s" es" s& s" hacia" s?&
  ^only$ s" hay salida" s& possible1$ s& s" hacia" s&
  ^only$ s" es posible" s& go-out-to&
  ^only$ s" se puede" s& go-out-to&
  }s  ;

: possible-exits$  ( -- ca len )
  s" salidas" possible2$ s&  ;

: several-exits$  ( -- ca len )
  \ Devuelve mensaje usado cuando hay varias salidas que listar.
  s{
  s" Hay" possible-exits$ s& s" hacia" s&
  s" Las" possible-exits$ s& s" son" s&
  }s  ;

: .exits  ( -- )
  \ Imprime las salidas posibles.
  #listed @ case
    0 of  no-exit$  endof
    1 of  one-exit-only$  endof
    several-exits$ rot
  endcase
  «& «»@ period+ narrate  ;

: exit-separator$  ( -- ca len )
  \ Devuelve el separador adecuado a la salida actual.
  #free-exits @ #listed @ list-separator$  ;

: exit>list  ( u -- )
  \ Lista una salida.
  \ u = Puntero a un campo de dirección (desplazamiento relativo desde el inicio de la ficha)
  [debug-do-exits] [if]  cr ." exit>list" cr .stack  [then]  \ XXX INFORMER
  exit-separator$ »+
  exits-table@ full-name »+
  #listed ++
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER

false [if]  \ Primera versión

\ Las salidas se listan siempre en el mismo orden en el que
\ están definidas en las fichas.

: free-exits  ( a -- u )
  \ Devuelve el número de salidas posibles de un ente.
  [debug-do-exits] [if]  cr ." free-exits" cr .stack  [then]  \ XXX INFORMER
  0 swap
  ~first-exit /exits bounds do
\   [debug-do-exits] [if]  i i cr . @ .  [then]  \ XXX INFORMER
    i @ 0<> abs +
  cell  +loop
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER

:action do-exits
  \ Acción de listar las salidas posibles de la localización del protagonista.
  «»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
  #listed off
  my-location dup free-exits #free-exits !
  last-exit> 1+ first-exit> do
    [debug-do-exits] [??] ~~
    dup i + @
    [debug-do-exits] [??] ~~
    if  i exit>list  then
  cell  +loop  drop
  .exits
  ;action

[else]  \ Segunda versión

\ Las salidas se muestran cada vez en orden aleatorio.

0 value this-location  \ Guardará el ente del que queremos calcular las salidas libres (para simplificar el manejo de la pila en el bucle)
: free-exits  ( a0 -- a1 ... au u )
  \ Devuelve el número de salidas posibles de un ente.
  \ a0 = Ente
  \ a1 ... au = Entes de salida del ente a0
  \ u = número de entes de salida del ente a0
  [debug-do-exits] [if]  cr ." free-exits" cr .stack  [then]  \ XXX INFORMER
  to this-location  depth >r
  last-exit> 1+ first-exit> do
    this-location i + @ ?? i
  cell  +loop
  depth r> -
  [debug-do-exits] [if]  cr .stack  [then]  ;  \ XXX INFORMER

: (list-exits)  ( -- )
  \ Crea la lista de salidas y la imprime
  «»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
  #listed off
  my-location free-exits
  dup >r unsort r>  dup #free-exits !
  0 ?do  exit>list  loop  .exits  ;

' (list-exits) is list-exits
:action do-exits
  \ Lista las salidas posibles de la localización del protagonista.
  \ Comprobar la conveniencia de posibles complementos:
  tool-complement{unnecessary}
  secondary-complement{forbidden}
  main-complement @ ?dup if
    dup my-location <> swap direction 0= and
    nonsense-error# and throw
  then  list-exits
  ;action

[then]

\ }}}---------------------------------------------
subsection( Ponerse y quitarse prendas)  \ {{{

: (do-put-on)  ( a -- )
  \ Ponerse una prenda.
  is-worn  well-done  ;

:action do-put-on
  \ Acción de ponerse una prenda.
  tool-complement{unnecessary}
  main-complement{required}
  main-complement{cloth}
  \ XXX TODO -- terminar, hacer que tome la prenda si no la tiene:
  main-complement{not-worn}
  main-complement @ is-not-hold? if  do-take  then
  main-complement{hold}
  main-complement @ (do-put-on)
  ;action
: do-take-off-done-v1$  ( -- ca len )
  \ Devuelve una variante del mensaje que informa de que el
  \ protagonista se ha quitado el complemento principal, una
  \ prenda.
  \ XXX TODO -- esta variante no queda natural
  main-complement @ direct-pronoun s" quitas" s&  ;

: do-take-off-done-v2$  ( -- ca len )
  \ Devuelve una variante del mensaje que informa de que el
  \ protagonista se ha quitado el complemento principal, una
  \ prenda.
  s" quitas" main-complement @ full-name s&  ;

: do-take-off-done$  ( -- ca len )
  \ Devuelve el mensaje que informa de que el protagonista se ha
  \ quitado el complemento principal, una prenda.
  s" Te" s{ do-take-off-done-v1$ do-take-off-done-v2$ }s& period+  ;

: (do-take-off)  ( a -- )
  \ Quitarse una prenda.
  is-not-worn  do-take-off-done$ well-done-this  ;

:action do-take-off
  \ Acción de quitarse una prenda.
  tool-complement{unnecessary}
  main-complement{required}
  main-complement{worn}
  main-complement @ (do-take-off)
  ;action

\ }}}---------------------------------------------
subsection( Tomar y dejar)  \ {{{

\ XXX OLD -- Puede que aún sirva:
\ : cannot-take-the-altar  \ No se puede tomar el altar
\   s" [el altar no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-flags  \ No se puede tomar las banderas
\   s" [las banderas no se tocan]" narrate  \ XXX TMP
\   nonsense
\   ;
\ : cannot-take-the-idol  \ No se puede tomar el ídolo
\   s" [el ídolo no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-door  \ No se puede tomar la puerta
\   s" [la puerta no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-fallen-away  \ No se puede tomar el derrumbe
\   s" [el derrumbe no se toca]" narrate  \ XXX TMP
\   nonsense
\   ;
\ : cannot-take-the-snake  \ No se puede tomar la serpiente
\   s" [la serpiente no se toca]" narrate  \ XXX TMP
\   dangerous
\   ;
\ : cannot-take-the-lake  \ No se puede tomar el lago
\   s" [el lago no se toca]" narrate  \ XXX TMP
\   nonsense
\   ;
\ : cannot-take-the-lock  \ No se puede tomar el candado
\   s" [el candado no se toca]" narrate  \ XXX TMP
\   impossible
\   ;
\ : cannot-take-the-water-fall  \ No se puede tomar la cascada
\   s" [la cascada no se toca]" narrate  \ XXX TMP
\   nonsense
\   ;
: (do-take)  ( a -- )
  \ Toma un ente.
  dup is-hold familiar++ well-done  ;

:action do-take
  \ Toma un ente, si es posible.
  main-complement{required}
  main-complement{not-hold}
  main-complement{here}
  main-complement{takeable}
  main-complement @ (do-take)
  ;action
: >do-drop-done-v1$  ( a -- ca1 len1 ) { object }
  s" Te desprendes de" s{
    object full-name
    object personal-pronoun
  }s& period+  ;

: >do-drop-done-v2$  ( a -- ca1 len1 )
  ^direct-pronoun s" dejas." s&  ;

: >do-drop-done$  ( a -- ca1 len1 ) { object }
  s{ object >do-drop-done-v1$  object >do-drop-done-v2$ }s  ;

: (do-drop)  ( a -- ) { object }
  \ Deja un ente.
  object is-worn? if

    [false] [if]  \ XXX TODO -- mensaje combinado:
    object is-not-worn
    s" te" s{
      direct-pronoun s& s" quita" object plural-ending+
      s" quita" object plural-ending+ object full-name s&
    }s& s" y" s&

  else  ""

    [then]

    \ XXX NOTE: método más sencillo:

    do-take-off

  then
  object is-here
  object >do-drop-done$ well-done-this  ;

:action do-drop
  \ Acción de dejar.
  main-complement{required}
  main-complement{hold}
  main-complement @ (do-drop)
  ;action
:action do-take|do-eat
  \ Acción de desambiguación.
  \ XXX TODO
  do-take
  ;action

\ }}}---------------------------------------------
subsection( Cerrar y abrir)  \ {{{

: first-close-the-door  ( -- )
  \ Informa de que la puerta está abierta
  \ y hay que cerrarla antes de poder cerrar el candado.
  s" cierras" s" primero" rnd2swap s& ^uppercase
  door% full-name s& period+ narrate
  door% is-closed  ;

: .the-key-fits  ( -- )
  \ XXX TODO -- nuevo texto, quitar «fácilmente»
  s" La llave gira fácilmente dentro del candado."
  narrate  ;

: close-the-lock  ( -- )
  \ Cerrar el candado, si es posible.
  key% tool{this-only}
  lock% {open}
  key% {hold}
  door% is-open? ?? first-close-the-door
  lock% is-closed  .the-key-fits  ;

: .the-door-closes  ( -- )
  \ Muestra el mensaje de cierre de la puerta.
  s" La puerta"
  s{ s" rechina" s" emite un chirrido" }s&
  s{ s" mientras la cierras" s" al cerrarse" }s&
  period+ narrate  ;

: (close-the-door)  ( -- )
  \ Cerrar la puerta.
  door% is-closed .the-door-closes
  location-47% location-48% w|<-->|
  location-47% location-48% o|<-->|  ;

: close-and-lock-the-door  ( -- )
  \ Cerrar la puerta, si está abierta, y el candado.
  door% {open}  key% {hold}
  (close-the-door) close-the-lock  ;

: just-close-the-door  ( -- )
  \ Cerrar la puerta, sin candarla, si está abierta.
  door% {open} (close-the-door)  ;

: close-the-door  ( -- )
  \ Cerrar la puerta, si es posible.
  key% tool{this-only}
  tool-complement @ ?dup
  if    close-and-lock-the-door  \ Con llave
  else  just-close-the-door  \ Sin llave
  then  ;

: close-it  ( a -- )
  \ Cerrar un ente, si es posible.
  case
    door% of  close-the-door  endof
    lock% of  close-the-lock  endof
    nonsense
  endcase  ;

:action do-close
  \ Acción de cerrar.
  main-complement{required}
  main-complement{accessible}
  main-complement @ close-it
  ;action
: the-door-is-locked  ( -- )
  \ Informa de que la puerta está cerrada por el candado.
  \ XXX TODO -- añadir variantes
  lock% ^full-name s" bloquea la puerta." s&
  narrate
  lock-found  ;

: unlock-the-door  ( -- )
  \ Abrir la puerta candada, si es posible.
  \ XXX TODO -- falta mensaje adecuado sobre la llave que gira
  the-door-is-locked
  key% {needed}
  lock% dup is-open
  ^pronoun s" abres con" s& key% full-name s& period+ narrate  ;

: open-the-lock  ( -- )
  \ Abrir el candado, si es posible.
  key% tool{this-only}
  lock% {closed}
  key% {needed}
  lock% is-open  well-done  ;

: the-plants$  ( -- ca len )
  \ Devuelve las plantas que la puerta rompe al abrirse.
  s" las hiedras" s" las hierbas" both  ;
  \ XXX TODO -- hacerlas visibles

: the-door-breaks-the-plants-0$  ( -- ca len )
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (primera variante).
  s{ s" mientras" s" al tiempo que" }s
  the-plants$ s& s" se rompen en su trazado" s&  ;

: the-door-breaks-the-plants-1$  ( -- ca len )
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (segunda variante).
  s" rompiendo" the-plants$ s& s" a su paso" s&  ;

: the-door-breaks-the-plants$  ( -- ca len )
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta.
  ['] the-door-breaks-the-plants-0$
  ['] the-door-breaks-the-plants-1$
  2 choose execute  ;

: the-door-sounds$  ( -- ca len )
  s{ s" rechinando" s" con un chirrido" }s  ;

: ambrosio-byes  ( -- )
  \ Ambrosio se despide cuando se abre la puerta por primera vez.
  s" Ambrosio, alegre, se despide de ti:" narrate
  s{
  s{ s" Tengo" s" Ten" }s s" por seguro" s&
  s{ s" Estoy" s" Estate" }s s" seguro de" s&
  s{ s" Tengo" s" Ten" }s s" la" s& s{ s" seguridad" s" certeza" }s& s" de" s&
  s" No" s{ s" me cabe" s" te quepa" }s& s" duda de" s&
  s" No" s{ s" dudo" s" dudes" }s&
  }s s" que" s&
  s{
  s" nos volveremos a" s{ s" encontrar" s" ver" }s& again$ s?&
  s" volveremos a" s{ s" encontrarnos" s" vernos" }s& again$ s?&
  s" nos" s{ s" encontraremos" s" veremos" }s& again$ s&
  s" nuestros caminos" s{ s" volverán a cruzarse" s" se cruzarán" }s& again$ s&
  }s& period+ speak
  ^and|but$ s" , antes de que puedas" s+
  s{ s" decir algo" s" corresponderle" s" responderle" s" despedirte" s" de él" s?& }s&
  s" , te das cuenta de que" s+ s" Ambrosio" s?&
  s" , misteriosamente," s?+
  s{ s" ha desaparecido" s" se ha marchado" s" se ha ido" s" ya no está" }s&
  period+ narrate  ;

: the-door-opens-first-time$  ( -- ca len )
  \ Devuelve el mensaje de apertura de la puerta
  \ la primera vez.
  s" La puerta" s{ s" cede" s" se abre" }s&
  s{ s" despacio" s" poco a poco" s" lentamente" }s&
  s" y no sin" s&
  s{ s" dificultad" s" ofrecer resistencia" }s& comma+
  the-door-sounds$ comma+ s&
  the-door-breaks-the-plants$ s& period+  ;

: the-door-opens-once-more$  ( -- ca len )
  \ Devuelve el mensaje de apertura de la puerta
  \ la segunda y siguientes veces.
  s" La puerta se abre" the-door-sounds$ s& period+  ;

: .the-door-opens  ( -- )
  \ Muestra el mensaje de apertura de la puerta.
  door% times-open
  if    the-door-opens-once-more$ narrate
  else  the-door-opens-first-time$ narrate ambrosio-byes
  then  ;

: (open-the-door)  ( -- )
  \ Abrir la puerta, que está cerrada.
  key% tool{this-only}  \ XXX TODO ¿por qué aquí?
  lock% is-closed? ?? unlock-the-door
  location-47% location-48% w<-->
  location-47% location-48% o<-->
  .the-door-opens
  door% dup is-open times-open++
  grass% is-here  ;

: open-the-door  ( -- )
  \ Abrir la puerta, si es posible.
  door% is-open?
  if    door% it-is-already-open tool-complement{unnecessary}
  else  (open-the-door)
  then  ;

: open-it  ( a -- )
  \ Abrir un ente, si es posible.
  dup familiar++
  case
    door% of  open-the-door  endof
    lock% of  open-the-lock  endof
    nonsense
  endcase  ;

:action do-open
  \ Acción de abrir.
  s" do-open" halto  \ XXX INFORMER
  main-complement{required}
  main-complement{accessible}
  main-complement @ open-it
  ;action

\ }}}---------------------------------------------
subsection( Agredir)  \ {{{

: the-snake-runs-away  ( -- )
  \ La serpiente huye.
  s{ s" Sorprendida por" s" Ante" }s
  s" los amenazadores tajos," s&
  s" la serpiente" s&
  s{
  s" huye" s" se aleja" s" se esconde"
  s" se da a la fuga" s" se quita de enmedio"
  s" se aparta" s" escapa"
  }s&
  s{ "" s" asustada" s" atemorizada" }s&
  narrate  ;

: attack-the-snake  ( -- )
  \ Atacar la serpiente.
  \ XXX TODO -- unfinished
  sword% {needed}
  the-snake-runs-away
  snake% vanish  ;

: attack-ambrosio  ( -- )
  \ Atacar a Ambrosio.
  no-reason  ;

: attack-leader  ( -- )
  \ Atacar al jefe.
  no-reason  ;

: (do-attack)  ( a -- )
  \ Atacar un ser vivo.
  case
    snake% of  attack-the-snake  endof
    ambrosio% of  attack-ambrosio  endof
    leader% of  attack-leader  endof
    do-not-worry
  endcase  ;

:action do-attack
  \ Acción de atacar.
  main-complement{required}
  main-complement{accessible}
  main-complement{living} \ XXX TODO -- también es posible atacar otras cosas, como la ciudad u otros lugares, o el enemigo
  tool-complement{hold}
  main-complement @ (do-attack)
  ;action
:action do-frighten
  \ Acción de asustar.
  \ XXX TODO -- distinguir de las demás en grado o requisitos
  main-complement{required}
  main-complement{accessible}
  main-complement{living}
  tool-complement{hold}
  main-complement @ (do-attack)
  ;action
: kill-the-snake  ( -- )
  \ Matar la serpiente.
  sword% {needed}
  the-snake-runs-away
  snake% vanish  ;

: kill-ambrosio  ( -- )
  \ Matar a Ambrosio.
  no-reason  ;

: kill-leader  ( -- )
  \ Matar al jefe.
  no-reason  ;

: kill-your-soldiers  ( -- )
  \ Matar a tus hombres.
  no-reason  ;

: (do-kill)  ( a -- )
  \ Matar un ser vivo.
  case
    snake% of  kill-the-snake  endof
    ambrosio% of  kill-ambrosio  endof
    leader% of  kill-leader  endof
    soldiers% of  kill-your-soldiers  endof
    do-not-worry
  endcase  ;

:action do-kill
  \ Acción de matar.
  main-complement{required}
  main-complement{accessible}
  main-complement{living}  \ XXX TODO -- también es posible matar otras cosas, como el enemigo
  tool-complement{hold}
  main-complement @ (do-kill)
  ;action
: cloak-piece  ( a -- )
  \ Hace aparecer un resto de la capa rota de forma aleatoria:
  \ en el escenario o en el inventario.
  2 random if  is-here  else  taken  then  ;

: cloak-pieces  ( -- )
  \ Hace aparecer los restos de la capa rota de forma aleatoria:
  \ en el escenario o en el inventario.
  rags% cloak-piece  thread% cloak-piece  piece% cloak-piece  ;

: break-the-cloak  ( -- )
  \ Romper la capa.
  \ Inacabado
  sword% {accessible}
  sword% taken
  s{ s" Con la ayuda de" s" Sirviéndote de" s" Usando" s" Empleando" }s
  sword% full-name s& comma+
  s" rasgas" s& cloak% full-name s& period+ narrate
  cloak-pieces  ;

: (do-break)  ( a -- )
  \ Romper un ente.
  case
    snake% of  kill-the-snake  endof  \ XXX TMP
    cloak% of  break-the-cloak  endof
    do-not-worry
  endcase  ;

:action do-break
  \ Acción de romper.
  main-complement{required}
  main-complement{accessible}
  main-complement{broken}
  tool-complement{hold}
  main-complement @ (do-break)
  ;action
: hit-the-flint  ( -- )
  ;
  \ XXX TODO -- unfinished

: (do-hit)  ( a -- )
  \ Golpear un ente.
  case
    snake% of  kill-the-snake  endof
    cloak% of  break-the-cloak  endof
    flint% of  hit-the-flint  endof
    do-not-worry
  endcase  ;

:action do-hit
  \ Acción de golpear.
  main-complement{required}
  main-complement{accessible}
  main-complement @ (do-hit)
  \ s" golpear"  main-complement+is-nonsense \ XXX TMP
  ;action
: can-be-sharpened?  ( a -- f )
  \ ¿Puede un ente ser afilado?
  \ XXX TODO -- mover esta palabra junto con los demás seudo-campos
  dup log% =  swap sword% =  or  ;  \ ¿Es el tronco o la espada?

: log-already-sharpened$  ( -- ca len )
  \ Devuulve una cadena con una variante de «Ya está afilado»
  s" Ya" s{
  s" lo afilaste antes"
  s" está afilado de antes"
  s" tiene una buena punta"
  s" quedó antes bien afilado"
  }s&  ;

: no-need-to-do-it-again$  ( -- ca len )
  \ Devuelve una variante de «no hace falta hacerlo otra vez».
  s{
  s" no es necesario"
  s" no hace" s" ninguna" s?& s" falta" s&
  s" no es menester"
  s" no serviría de nada"
  s" no serviría de mucho"
  s" serviría de poco"
  s" sería inútil"
  s" sería en balde"
  s" sería un esfuerzo" s{ s" inútil" s" baldío" }s&
  s" sería un esfuerzo baldío"
  }s{
  s" hacerlo"
  s" volver a hacerlo"
  s" repetirlo"
  }s& again$ s&  ;

: ^no-need-to-do-it-again$  ( -- ca len )
  \ Devuelve una variante de «No hace falta hacerlo otra vez».
  no-need-to-do-it-again$ ^uppercase  ;

: log-already-sharpened-0$  ( -- ca len )
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 0).
  log-already-sharpened$ ^uppercase period+
  ^no-need-to-do-it-again$ period+ s&  ;

: log-already-sharpened-1$  ( -- ca len )
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 1).
  ^no-need-to-do-it-again$ period+ s&
  log-already-sharpened$ ^uppercase period+ s&  ;

: log-already-sharpened  ( -- )
  \ Informa de que el tronco ya estaba afilado.
  ['] log-already-sharpened-0$
  ['] log-already-sharpened-1$
  2 choose execute  narrate  ;

: sharpen-the-log  ( -- )
  \ Afila el tronco.
  \ XXX TODO -- distinguir herramientas
  hacked-the-log? @
  if    log-already-sharpened
  else  hacked-the-log? on  well-done
  then  ;

: sharpen-the-sword  ( -- )
  \ Afila la espada.
  ;
  \ XXX TODO -- unfinished

: (do-sharpen)  ( a -- )
  \ Afila un ente que puede ser afilado.
  case
    sword% of  sharpen-the-sword  endof
    log% of  sharpen-the-log  endof
  endcase  ;

:action do-sharpen
  \ Acción de afilar.
  main-complement{required}
  main-complement{accessible}
  main-complement @ can-be-sharpened?
  if    main-complement @ (do-sharpen)
  else  nonsense
  then
  ;action

\ }}}---------------------------------------------
subsection( Movimiento)  \ {{{

\ XXX TODO -- mover a la sección de errores
: toward-that-direction  ( a -- ca2 len2 )
  \ Devuelve «al/hacia la dirección indicada».
  \ a = Ente dirección
  dup >r  has-no-article?
  if  \ no debe llevar artículo
    s" hacia" r> full-name
  else  \ debe llevar artículo
    toward-the(m)$ r> name
  then  s&  ;

: (impossible-move)  ( a -- )
  \ El movimiento es imposible.
  \ a = Ente dirección
  \ XXX TODO -- añadir una tercera variante «ir en esa dirección»; y otras específicas como «no es posible subir»
  ^is-impossible$ s" ir" s&  rot
  3 random
  if    toward-that-direction
  else  drop that-way$
  then  s& period+ action-error  ;

1 ' (impossible-move) action-error: impossible-move drop
: do-go-if-possible  ( a -- )
  \ Comprueba si el movimiento es posible y lo efectúa.
  \ a = Ente supuestamente de tipo dirección
  [debug] [if]  s" Al entrar en DO-GO-IF-POSSIBLE" debug  [then]  \ XXX INFORMER
  dup direction ?dup if  \ ¿El ente es una dirección?
    my-location + @ ?dup  \ ¿Tiene mi escenario salida en esa dirección?
    if  nip enter-location  else  impossible-move  then
  else  drop nonsense
  then
  [debug] [if]  s" Al salir de DO-GO-IF-POSSIBLE" debug  [then]  ;  \ XXX INFORMER

: simply-do-go  ( -- )
  \ Ir sin dirección específica.
  \ XXX TODO -- unfinished
  s" Ir sin rumbo...?" narrate  ;

:action do-go
  \ Acción de ir.
  [debug] [if]  s" Al entrar en DO-GO" debug  [then]  \ XXX INFORMER
  tool-complement{unnecessary}
  main-complement @ ?dup
  if  do-go-if-possible
  else  simply-do-go
  then
  [debug] [if]  s" Al salir de DO-GO" debug  [then]  \ XXX INFORMER
  ;action
:action do-go-north
  \ Acción de ir al norte.
  tool-complement{unnecessary}
  north% main-complement{this-only}
  north% do-go-if-possible
  ;action
:action do-go-south
  \ Acción de ir al sur.
  [debug-catch] [if]  s" Al entrar en DO-GO-SOUTH" debug  [then]  \ XXX INFORMER
  tool-complement{unnecessary}
  south% main-complement{this-only}
  south% do-go-if-possible
  [debug-catch] [if]  s" Al salir de DO-GO-SOUTH" debug  [then]  \ XXX INFORMER
  ;action
:action do-go-east
  \ Acción de ir al este.
  tool-complement{unnecessary}
  east% main-complement{this-only}
  east% do-go-if-possible
  ;action
:action do-go-west
  \ Acción de ir al oeste.
  tool-complement{unnecessary}
  west% main-complement{this-only}
  west% do-go-if-possible
  ;action
:action do-go-up
  \ Acción de ir hacia arriba.
  tool-complement{unnecessary}
  up% main-complement{this-only}
  up% do-go-if-possible
  ;action
:action do-go-down
  \ Acción de ir hacia abajo.
  tool-complement{unnecessary}
  down% main-complement{this-only}
  down% do-go-if-possible
  ;action
:action do-go-out
  \ Acción de ir hacia fuera.
  tool-complement{unnecessary}
  out% main-complement{this-only}
  out% do-go-if-possible
  ;action
:action do-go-in
  \ Acción de ir hacia dentro.
  tool-complement{unnecessary}
  in% main-complement{this-only}
  in% do-go-if-possible
  ;action
:action do-go-back
  \ Acción de ir hacia atrás.
  \ XXX TODO
  tool-complement{unnecessary}
  main-complement{forbidden}
  s" [voy atrás, pero es broma]" narrate \ XXX TMP
  ;action
:action do-go-ahead
  \ Acción de ir hacia delante.
  \ XXX TODO
  tool-complement{unnecessary}
  main-complement{forbidden}
  s" [voy alante, pero es broma]" narrate \ XXX TMP
  ;action

\ }}}---------------------------------------------
subsection( Partir [desambiguación])  \ {{{

:action do-go|do-break
  \ Acción de partir (desambiguar: romper o marchar).
  main-complement @ ?dup
  if  ( a )  \ Hay complemento principal
    is-direction?
    if  do-go  else  do-break  then
  else
    tool-complement @
    if do-break  \ Solo con herramienta, suponemos que es «romper»
    else  simply-do-go  \ Sin complementos, suponemos que es «marchar»
    then
  then
  ;action

\ }}}---------------------------------------------
subsection( Nadar)  \ {{{

: in-a-different-place$  ( -- ca len )
  \ Devuelve una variante de «en un lugar diferente».
  s" en un" s& place$
  s{ s" desconocido" s" nuevo" s" diferente" }s&
  s" en otra parte"
  s" en otro lugar"
  3 schoose  ;

: you-emerge$  ( -- ca len )
  \ Devuelve mensaje sobre la salida a la superficie.
  s{ s" Consigues" s" Logras" }s
  s{ s" emerger," s" salir a la superficie," }s&
  though$ s& in-a-different-place$ s&
  s" de la" s& cave$ s& s" ..." s+  ;

: swiming$  ( -- ca len )
  \ Devuelve mensaje sobre el buceo.
  s" Buceas" s{ s" pensando en" s" deseando"
  s" con la esperanza de" s" con la intención de" }s&
  s{ s" avanzar," s" huir," s" escapar,"  s" salir," }s&
  s" aunque" s&{ s" perdido." s" desorientado." }s&  ;

: drop-the-cuirasse$  ( f -- ca len )
  \ Devuelve mensaje sobre deshacerse de la coraza dentro del agua.
  \ f = ¿Inicio de frase?
  s{ s" te desprendes de ella" s" te deshaces de ella"
  s" la dejas caer" s" la sueltas" }s
  rot if  \ ¿Inicio de frase?
    s{ s" Rápidamente" s" Sin dilación"
    s" Sin dudarlo" s{ "" s" un momento" s" un instante" }s&
    }s 2swap s&
  then  period+  ;

: you-leave-the-cuirasse$  ( -- ca len )
  \ Devuelve mensaje sobre quitarse y soltar la coraza dentro del agua.
  cuirasse% is-worn-by-me?  \ ¿La llevamos puesta?
  if
    s{ s" Como puedes," s" No sin dificultad," }s
    s{ s" logras quitártela" s" te la quitas" }s&
    s" y" s& false drop-the-cuirasse$ s&
  else
    true drop-the-cuirasse$
  then  ;

: (you-sink-0)$ ( -- ca len )
  \ Devuelve la primera versión del mensaje sobre hundirse con la coraza.
  s{ s" Caes" s" Te hundes"
  s{ s" Empiezas" s" Comienzas" }s{ s" a hundirte" s" a caer" }s&
  }s s" sin remedio" s?& s" hacia" s&
  s{ s" el fondo" s" las profundidades" }s&
  s{ s" por el" s" debido al" s" arrastrado por" s" a causa del" }s&
  s" peso de tu coraza" s&  ;

: (you-sink-1)$ ( -- ca len )
  \ Devuelve la segunda versión del mensaje sobre hundirse con la coraza.
  s" El peso de tu coraza"
  s{ s" te arrastra" s" tira de ti" }s&
  s{ "" s" sin remedio" s" con fuerza" }s&
  s{
  s" hacia el fondo"
  s" hacia las profundidades"
  s" hacia abajo"
  }s&  ;

: you-sink$ ( -- ca len )
  \ Devuelve mensaje sobre hundirse con la coraza.
  ['] (you-sink-0)$
  ['] (you-sink-1)$
  2 choose execute period+  ;

: you-swim-with-cuirasse$  ( -- ca len )
  \  Devuelve mensaje inicial sobre nadar con coraza.
  you-sink$ you-leave-the-cuirasse$ s&  ;

: you-swim$  ( -- ca len )
  \  Devuelve mensaje sobre nadar.
  cuirasse% is-hold?
  if  you-swim-with-cuirasse$  cuirasse% vanish
  else  ""
  then  swiming$ s&  ;

:action do-swim
  \ Acción de nadar.
  my-location location-11% = if
    clear-screen-for-location
    you-swim$ narrate narration-break
    you-emerge$ narrate narration-break
    location-12% enter-location  the-battle-ends
  else
    s" nadar" now|here|""$ s& is-nonsense
  then
  ;action

\ }}}---------------------------------------------
subsection( Escalar)  \ {{{

: you-try-climbing-the-fallen-away  ( -- )
  \ Imprime la primera parte del mensaje
  \ previo al primer intento de escalar el derrumbe.
  s{ s" Aunque" s" A pesar de que" }s
  s{  s" parece no haber salida"
      s" el obstáculo parece insuperable"
      s" la situación parece desesperada"
      s" el regreso parece inevitable"
      s" continuar parece imposible"
  }s& comma+
  s{ s" optas por" s" decides" s" tomas la decisión de" }s&
  s{ s" explorar" s" examinar" }s& s" el" s&
  s{ s" derrumbe" s" muro de" rocks$ s& }s&
  s{  s" en compañía de" s" junto con"
      s" ayudado por" s" acompañado por"
  }s&
  s{ s" algunos" s" varios" }s& s" de tus" s&
  s{ s" oficiales" soldiers$ }s& s" , con la" s+
  s{ s" vana" s" pobre" s" débil" }s& s" esperanza" s&
  s" de" s&
  s{ s" encontrar" s" hallar" s" descubrir" }s&
  s{ s" la" s" alguna" }s& way$ s& s" de" s&
  s{  s" escalarlo" s" vencerlo" s" atravesarlo"
      s" superarlo" s" pasarlo"
      s{ s" pasar" s" cruzar" }s s" al otro lado" s&
  }s&  period+ narrate narration-break  ;

: you-can-not-climb-the-fallen-away  ( -- )
  \ Imprime la segunda parte del mensaje
  \ previo al primer intento de escalar el derrumbe.
  ^but$
  s{  s{ s" pronto" s" enseguida" }s s" has de" s&
      s" no tardas mucho en"
  }s&
  s{  s" rendirte ante" s" aceptar" s" resignarte ante" }s&
  s{
    s{ s" los hechos" s" la realidad" s" la situación" s" tu suerte" }s
    s{  s" la voluntad"
        s{ s" el" s" este" }s s" giro" s&
        s{ s" el" s" este" }s s" capricho" s&
        s{ s" la" s" esta" }s s" mudanza" s&
    }s s" de" s& s" la diosa" s?& s" Fortuna" s&
  }s& s" ..." s+ narrate narration-break  ;

: do-climb-the-fallen-away-first  ( -- )
  \ Imprime el mensaje
  \ previo al primer intento de escalar el derrumbe.
  you-try-climbing-the-fallen-away
  you-can-not-climb-the-fallen-away  ;

: climbing-the-fallen-away-is-impossible  ( -- )
  \ Imprime el mensaje de error de que
  \ es imposible escalar el derrumbe.
  s{ s" pasar" s" escalar" s" subir por" }s
  s{
    s" el derrumbe"
    s{ s" el muro" s" la pared" s" el montón" }s s" de" s& rocks$ s&
    s" las" rocks$ s&
  }s& is-impossible  ;

: do-climb-the-fallen-away  ( -- )
  \ Escalar el derrumbe.
  climbed-the-fallen-away? @ 0=
  ?? do-climb-the-fallen-away-first
  climbing-the-fallen-away-is-impossible
  climbed-the-fallen-away? on  ;

: do-climb-this-here-if-possible  ( a -- )
  \ Escalar el ente indicado, que está presente, si es posible.
  ;
  \ XXX TODO -- unfinished

: do-climb-if-possible  ( a -- )
  \ Escalar el ente indicado si es posible.
  \ XXX TODO -- unfinished
  dup is-here? if  drop s" [escalar eso]" narrate
  else  drop s" [no está aquí eso para escalarlo]" narrate
  then  ;

: nothing-to-climb  ( -- )
  \ XXX TODO -- unfinished
  s" [No hay nada que escalar]" narrate  ;

: do-climb-something  ( -- )
  \ Escalar algo no especificado.
  \ XXX TODO -- unfinished
  location-09% am-i-there?  \ ¿Ante el derrumbe?
  if  do-climb-the-fallen-away exit
  then
  location-08% am-i-there?  \ ¿En el desfiladero?
  if  s" [Escalar en el desfiladero]" narrate exit
  then
  my-location is-indoor-location?
  if  s" [Escalar en un interior]" narrate exit
  then
  nothing-to-climb  ;

:action do-climb
  \ Acción de escalar.
  \ XXX TODO -- unfinished
  main-complement @ ?dup
  if  do-climb-if-possible  else  do-climb-something  then
  ;action

\ }}}---------------------------------------------
subsection( Inventario)  \ {{{

: anything-with-you$  ( -- ca len )
  \ Devuelve una variante de «nada contigo».
  s" nada" with-you$  ?dup if
    2 random ?? 2swap s&
  else drop
  then  ;

: you-are-carrying-nothing$  ( -- ca len )
  \ Devuelve mensaje para sustituir a un inventario vacío.
  s" No" you-carry$ anything-with-you$ period+ s& s&  ;

: ^you-are-carrying$  ( -- ca len )
  \ Devuelve mensaje para encabezar la lista de inventario,
  \ con la primera letra mayúscula.
  ^you-carry$ with-you$ s&  ;

: you-are-carrying$  ( -- ca len )
  \ Devuelve mensaje para encabezar la lista de inventario.
  you-carry$ with-you$ s&  ;

: you-are-carrying-only$  ( -- ca len )
  \ Devuelve mensaje para encabezar una lista de inventario de un solo elemento.
  2 random
  if  ^you-are-carrying$ only-$ s&
  else  ^only-$ you-are-carrying$ s&
  then  ;

:action do-inventory
  \ Acción de hacer inventario.
  protagonist% content-list  \ Hace la lista en la cadena dinámica `print-str`
  #listed @ case
    0 of  you-are-carrying-nothing$ 2swap s& endof
    1 of  you-are-carrying-only$ 2swap s& endof
    >r ^you-are-carrying$ 2swap s& r>
  endcase  narrate
  ;action

\ }}}---------------------------------------------
subsection( Hacer)  \ {{{

:action do-make
  \ Acción de hacer (fabricar).
  main-complement @
  if    nonsense
  else  do-not-worry
  then
  ;action

:action do-do
  \ Acción de hacer (genérica).
  main-complement @ inventory% =
  if    do-inventory
  else  do-make
  then
  ;action

\ }}}---------------------------------------------
subsection( Hablar y presentarse)  \ {{{

\ ----------------------------------------------
\ Conversaciones con el líder de los refugiados

: a-man-takes-the-stone  ( -- )
  \ Un hombre te quita la piedra.
  s{  s" Un hombre" s" Un refugiado"
      s" Uno de los" s{ s" hombres" s" refugiados" }s&
  }s{ s" se adelanta,"
      s" sale de entre"
        s{  s" sus compañeros" s" los otros"
            s" la multitud" s" la gente"
        }s& comma+
  }s?&
  s{  s" se te acerca," s" se acerca a ti,"
      s" se te aproxima," s" se aproxima a ti,"
  }s?&
  s" te" s&{ s" arrebata" s" quita" }s&
  s" la piedra" s& s" de las manos" s?& s" y" s&
  s{
    s" se la lleva"
    s{ s" se marcha" s" se va" s" desaparece" }s s" con ella" s?&
  }s& period+ narrate
  location-18% stone% is-there  ;

: gets-angry$  ( -- ca len )
  \ Devuelve una variante de «se enfada».
  s" se" s{ s" irrita" s" enfada" s" enoja" s" enfurece" }s&  ;

: the-leader-gets-angry$  ( -- ca len )
  \ Devuelve una variante de «El líder se enfada».
  \ XXX OLD -- yo no se usa.
  s{ s" Al verte" s" Viéndote" s" Tras verte" }s
  s{ s" llegar" s" aparecer" s" venir" }s&
  again$ stone-forbidden? @ ?keep s&
  s" con la piedra," s&
  s" el" s& old-man$ s& gets-angry$ s&  ;

: the-leader-gets-angry  ( -- )
  \ Mensaje de que el líder se enfada.
  \ XXX OLD -- ya no se usa.
  the-leader-gets-angry$ period+ narrate  ;

: warned-once$  ( -- ca len )
  s{
  s" antes"
  s" en la ocasión anterior"
  s" en la otra ocasión"
  s" en una ocasión"
  s" la otra vez"
  s" la vez anterior"
  s" una vez"
  }s  ;

: warned-twice$  ( -- ca len )
  s{
  s" antes"
  s" dos veces"
  s" en dos ocasiones"
  s" en las otras ocasiones"
  s" en mas de una ocasión"
  s" en un par de ocasiones"
  s" las otras veces"
  s" más de una vez"
  s" un par de veces"
  }s  ;

: warned-several-times$  ( -- ca len )
  s{
  s" en las otras ocasiones"
  s" en más de dos ocasiones"
  s" en más de un par de ocasiones"
  s" en otras ocasiones"
  s" en varias ocasiones"
  s" las otras veces"
  s" más de dos veces"
  s" más de un par de veces"
  s" otras veces"
  s" varias veces"
  }s  ;

: warned-many-times$  ( -- ca len )
  s{
  s" demasiadas veces"
  s" en demasiadas ocasiones"
  s" en incontables ocasiones"
  s" en innumerables ocasiones"
  s" en las otras ocasiones"
  s" en muchas ocasiones"
  s" en varias ocasiones"
  s" incontables veces"
  s" innumerables veces"
  s" las otras veces"
  s" muchas veces"
  s" otras veces"
  s" varias veces"
  }s  ;

: times-warned  ( u -- ca1 len1 )
  { times }  \ Variable local
  true case
    times 0 = of  ""  endof
    times 1 = of  warned-once$  endof
    times 2 = of  warned-twice$  endof
    times 6 < of  warned-several-times$  endof
    warned-many-times$ rot
  endcase  ;

: already-warned$  ( -- ca len )
  \ Mensaje de que el líder ya te advirtió sobre un objeto.
  \ XXX TODO -- elaborar más
  s" ya" s?
  s{
    s" fuisteis" s{ s" avisado" s" advertido" }s& s" de ello" s?&
    s" se os" s{ s" avisó" s" advirtió" }s& s" de ello" s?&
    s" os lo" s{ s" hicimos saber" s" advertimos" }s&
    s" os lo" s{ s" hice saber" s" advertí" }s&
    s" se os" s{ s" hizo saber" s" dejó claro" }s&
  }s&  ;

: already-warned  ( u -- ca1 len1 )
  \ Mensaje de que el líder ya te advirtió sobre un objeto,
  \ con indicación al azar del número de veces.
  times-warned already-warned$ rnd2swap s& period+ ^uppercase  ;

: you-can-not-take-the-stone$  ( -- ca len )
  \ Mensaje de que no te puedes llevar la piedra.
  s{ s" No" s" En modo alguno" s" De ninguna" way$ s& s" De ningún modo" }s
  s" podemos" s&
  s{
    s{ s" permitiros" s" consentiros" }s
      s{ s" huir" s" escapar" s" marchar" s" pasar" }s& s" con" s&
    s{ s" permitir" s" consentir" s" aceptar" }s s" que" s&
      s{  s{ s" huyáis" s" escapéis" s" marchéis" s" paséis" }s s" con" s&
          s" os vayáis con"
          s" os" s? s" marchéis con" s&
          s" os llevéis"
          s" nos" s? s" robéis" s&
          s" os" s{ s" apropiés" s" apoderéis" s" adueñéis" }s& s" de" s&
      }s&
  }s& s" la" s" piedra del druida"
  2dup stone% fs-name!  \ Nuevo nombre para la piedra
  s& s& period+  ;

: gesture-about-the-stone$  ( -- ca len )
  \ Mensaje de que el líder hace un gesto sobre la piedra.
  s" y" s? s{ s" entonces" s" a continuación" s" seguidamente" }s& ^uppercase
  s" hace un" s&
  s" pequeño" s?& s" gesto" s& s" con la mano," s?&
  s" casi imperceptible" s?&
  s" ..." s+  ;

: the-stone-must-be-in-its-place$  ( -- ca len )
  \ El líder dice que la piedra debe ser devuelta.
  s" La piedra" s{ s" ha de" s" debe" s" tiene que" }s&
  s{ s" ser devuelta" s" devolverse" to-go-back$ }s&
  s{
    s" a su lugar" s" de encierro" s?&
    s" al lugar al que pertenece"
    s" al lugar del que nunca debió" s{ s" salir" s" ser sacada" s" ser arrancada" }s&
    s" al lugar que nunca debió" s{ s" abandonar" s" haber abandonado" }s&
  }s&  ;

: the-leader-warns-about-the-stone  ( -- )
  \ El líder habla acerca de la piedra.
  stone-forbidden? @ already-warned
  you-can-not-take-the-stone$
  the-stone-must-be-in-its-place$ rnd2swap s& s&
  period+ speak  ;

: the-leader-points-to-the-north$  ( -- ca len )
  \ El líder se enfada y apunta al norte.
  \ XXX TODO -- crear ente "brazo" aquí, o activarlo como sinómino del anciano
  leader% ^full-name
  s{ s" alza" s" extiende" s" levanta" }s&
  s{ s" su" s" el" }s& s" brazo" s&
  s{ s" indicando" s" en dirección" s" señalando" }s&
  toward-the(m)$ s& s" norte." s&  ;

: the-leader-points-to-the-north  ( -- )
  \ El líder se enfada y apunta al norte.
  the-leader-points-to-the-north$ narrate  ;

: nobody-passes-with-arms$  ( -- ca len )
  \ El líder dice que nadie pasa con armas.
  s{ s" Nadie" s" Ningún hombre" }s
  s{ s" con" s" llevando" s" portando" s" portador de"
  s" que porte" s" que lleve" }s&
  s{ s" armas" s" un arma" s" una espada" }s&
  with-him$ s&{ s" debe" s" puede" s" podrá" }s&
  s" pasar." s&  ;

: the-leader-warns-about-the-sword  ( -- )
  \ El líder habla acerca de la espada.
  the-leader-points-to-the-north
  sword-forbidden? @ already-warned
  nobody-passes-with-arms$ s& speak  ;

: the-leader-points-to-the-east  ( -- )
  \ El líder apunta al este.
  s" El" old-man$ s& comma+
  s{ s" confiado" s" calmado" s" sereno" s" tranquilo" }s& comma+
  s{ s" indica" s" señala" }s&
  s{ toward-the(m)$ s" en dirección al" }s& s" este y" s&
  s{  s" te" s? s" dice" s&
      s" pronuncia las siguientes palabras"
  }s& colon+ narrate  ;

: something-had-been-forbidden?  ( -- f )
  \ Se le prohibió alguna vez al protagonista pasar con algo prohibido?
  sword-forbidden? @ stone-forbidden? @ or  ;

: go-in-peace  ( -- )
  \ El líder dice que puedes ir en paz.
  s{ s" Ya que" s" Puesto que" s" Dado que" s" Pues" }s
  something-had-been-forbidden? if
    s{ s" esta vez" s" ahora" s" en esta ocasión" s" por fin" s" finalmente" }s&
  then
  s{ s" vienes" s" llegas" s" has venido" s" has llegado" }s&
  s" en paz, puedes" s&
  s{ s" ir" s" marchar" s" continuar" s" tu camino" s?& }s&
  s" en paz." s& speak  ;

: the-refugees-let-you-go  ( -- )
  \ Los refugiados te dejan pasar.
  s" todos" s? s" los refugiados" s& ^uppercase
  s" se apartan y" s& s" te" s?&
  s{  s" permiten" s{ s" el paso" s" pasar" }s&
      s" dejan" s" libre" s" el" s{ s" paso" s" camino" }s& rnd2swap s&
  }s& toward-the(m)$ s& s" este." s& narrate  ;

: the-leader-lets-you-go  ( -- )
  \ El jefe deja marchar al protagonista.
  location-28% location-29% e-->  \ Hacer que la salida al este de `location-28%` conduzca a `location-29%`
  the-leader-points-to-the-east
  go-in-peace the-refugees-let-you-go  ;

: talked-to-the-leader  ( -- )
  \ Aumentar el contador de conversaciones con el jefe de los refugiados.
  leader% conversations++
  recent-talks-to-the-leader ?++  ;

: we-are-refugees$  ( -- ca len )
  \ Mensaje «Somos refugiados».
  s" todos" s? s" nosotros" s? rnd2swap s&
  s" somos refugiados de" s& ^uppercase
  s{ s" la gran" s" esta terrible" }s& s" guerra." s&
  s" refugio" location-28% ms-name!  ;

: we-are-refugees  ( -- )
  \ Somos refugiados.
  we-are-refugees$ we-want-peace$ s& speak narration-break  ;

: the-leader-trusts  ( -- )
  \ El líder te corta, confiado.
  s" El" old-man$ s& s" asiente" s&
  s{ s" confiado" s" con confianza" }s& s" y," s&
  s" con un suave gesto" s& s" de su mano" s?& comma+
  s" te interrumpe para" s&
  s{  s" explicar" s{ s" te" s" se" "" }s+
      s" presentarse" s" contarte" s" decir" s" te" s?+
  }s& colon+ narrate  ;

: untrust$  ( -- ca len )
  s{ s" desconfianza" s" nerviosismo" s" impaciencia" }s  ;

: the-leader-does-not-trust  ( -- )
  \ El líder te corta, desconfiado.
  s" El" old-man$ s& s" asiente" s& s" con la cabeza" s?& comma+
  s{  s" desconfiado" s" nervioso" s" impaciente"
      s" mostrando" s" claros" s?& s" signos de" s& untrust$ s&
      s{ s" dando" s" con" }s s" claras" s?& s" muestras de" s& untrust$ s&
  }s& comma+ s" y te interrumpe:" s& narrate  ;

: the-leader-introduces-himself  ( -- )
  \ El líder se presenta.
  do-you-hold-something-forbidden?
  if    the-leader-does-not-trust
  else  the-leader-trusts
  then  we-are-refugees  ;

: first-conversation-with-the-leader  ( -- )
  \ XXX TODO -- elaborar mejor el texto
  my-name-is$ s" Ulfius y..." s& speak talked-to-the-leader
  the-leader-introduces-himself  ;

: the-leader-forbids-the-stone  ( -- )
  \ El jefe te avisa de que no puedes pasar con la piedra y te la quita.
  the-leader-warns-about-the-stone
  stone-forbidden? ?++  \ Recordarlo
  gesture-about-the-stone$ narrate narration-break
  a-man-takes-the-stone  ;

: the-leader-forbids-the-sword  ( -- )
  \ El jefe te avisa de que no puedes pasar con la espada.
  the-leader-warns-about-the-sword
  sword-forbidden? ?++  ;  \ Recordarlo

: the-leader-checks-what-you-carry  ( -- )
  \ El jefe controla lo que llevas.
  \ XXX TODO -- mejorar para que se pueda pasar si dejamos el objeto en el suelo o se lo damos
  true case
    stone% is-accessible? of  the-leader-forbids-the-stone  endof
    sword% is-accessible? of  the-leader-forbids-the-sword  endof
    the-leader-lets-you-go
  endcase  ;

: insisted-once$  ( -- ca len )
  \ XXX TODO -- unfinished
  s{ s" antes" s" una vez" }s  ;

: insisted-twice$  ( -- ca len )
  \ XXX TODO -- unfinished
  s{ s" antes" s" dos veces" s" un par de veces" }s  ;

: insisted-several-times$  ( -- ca len )
  \ XXX TODO -- unfinished
  s{ s" las otras" s" más de dos" s" más de un par de" s" varias" }s
  s" veces" s&  ;

: insisted-many-times$  ( -- ca len )
  \ XXX TODO -- unfinished
  s{  s" demasiadas" s" incontables" s" innumerables"
      s" las otras" s" muchas" s" varias"
  }s  s" veces" s&  ;

: times-insisted  ( u -- ca1 len1 )
  \ XXX TODO -- unfinished
  { times }  \ Variable local
  true case
    times 0 = of  ""  endof  \ XXX OLD -- innecesario
    times 1 = of  insisted-once$  endof
    times 2 = of  insisted-twice$  endof
    times 6 < of  insisted-several-times$  endof
    insisted-many-times$ rot
  endcase  ;

: please-don't-insist$  ( -- ca len )
  \ Mensaje de que por favor no insistas.
  s{ s" os ruego que" s" os lo ruego," s" he de rogaros que" }s
  s" no insistáis" s&  ;

: don't-insist$  ( -- ca len )
  \ XXX TODO -- unfinished
  s" ya" s?
  s{
    s" habéis sido" s{ s" avisado" s" advertido" }s&
    s" os lo he" s" mos" s?+ s{ s" hecho saber" s" advertido" s" dejado claro" }s&
    s" se os ha" s{ s" hecho saber" s" advertido" s" dejado claro" }s&
  }s&  ;

: don't-insist  ( -- )
  \ XXX TODO -- unfinished
  times-insisted don't-insist$ rnd2swap s& period+ ^uppercase  ;

: the-leader-ignores-you  ( -- )
  ;
  \ El líder te ignora.

: (talk-to-the-leader)  ( -- )
  \ Hablar con el jefe.
  leader% no-conversations?
  ?? first-conversation-with-the-leader
  the-leader-checks-what-you-carry  ;

: talk-to-the-leader  ( -- )
  \ Hablar con el jefe, si se puede.
  recent-talks-to-the-leader @
  if    the-leader-ignores-you
  else  (talk-to-the-leader)
  then  ;

\ ----------------------------------------------
\ Conversaciones con Ambrosio

: talked-to-ambrosio  ( -- )
  \ Aumentar el contador de conversaciones con Ambrosio.
  ambrosio% conversations++  ;

: is-ambrosio's-name  ( ca len -- )
  \ Le pone a ambrosio su nombre de pila.
  \ ca len = Nombre.
  ambrosio% ms-name!
  ambrosio% has-no-article
  ambrosio% has-personal-name  ;

: ambrosio-introduces-himself  ( -- )
  s" Hola, Ulfius."
  my-name-is$ s& s" Ambrosio" 2dup is-ambrosio's-name
  period+ s& speak  ;

: you-cry  ( -- )
  s" Por" s" primera" s" vez" rnd2swap s& s& s" en" s&
  s{ s" mucho" s" largo" }s& s" tiempo, te sientas y" s&
  s" le" s?& s{ s" cuentas" s" narras" s" relatas" }s&
  s" a alguien todo lo que ha" s&
  s{ s" sucedido" s" pasado" s" ocurrido" }s& period+
  s" Y, tras tanto" s& s" pesar" s?& s{ s" acontecido" s" vivido" }s&
  s" , lloras" s+{ s" desconsoladamente" s" sin consuelo" }s&
  period+ narrate  ;

: ambrosio-proposes-a-deal  ( -- )
  s" Ambrosio te propone un" s{ s" trato" s" acuerdo" }s& comma+
  s{  the-that(m)$ s" aceptas" s&
      s" con el" that(m)$ s&{ s" consientes" s" estás de acuerdo" }s&
      the-that(m)$ s" te parece justo" s&
  }s& colon+
  s" por ayudarlo a salir de" s&{ s" la" s" esta" }s& s" cueva," s&
  s{ s" objetos" s" útiles" }s& comma+
  s{ s" vitales" s" imprescindibles" s" necesarios" }s&
  s" para" s& s" el éxito de" s?&
  s{ s" la" s" tal" s" dicha" }s& s{ s" misión" s" empresa" }s&
  s" , te son entregados." s+ narrate
  torch% is-hold  flint% is-hold  ;

: ambrosio-let's-go  ( -- )
  s{  s" Bien"
      s{ s" Venga" s" Vamos" }s s" pues" s?&
  }s comma+ s" Ambrosio," s&
  s{  s{ s" iniciemos" s" emprendamos" }s{ s" la marcha" s" el camino" }s&
      s" pongámonos en" s{ s" marcha" s" camino" }s&
  }s& period+  speak
  location-46% ambrosio% is-there
  s" Te" s{ s" giras" s" das la vuelta" }s& s" para" s&
  s{  s{ s" comprobar" s" ver" }s s" si" s&
      s{ s" cerciorarte" s" asegurarte" }s s" de que" s&
  }s& s" Ambrosio te sigue," s& but$ s& s" ..." s+
  s{  s" ha desaparecido"
      s" se ha esfumado"
      s" no hay" s" ni" s?& s" rastro de él" s&
      s" ya" s? s" no está" s&
      s" ya" s? s" no hay nadie" s&
      s" ya" s? s" no ves a nadie" s&
      s" es como si se lo hubiera tragado la tierra"
  }s& period+ narrate  ;

: ambrosio-is-gone  ( -- )
  s{  suddenly|then$ s" piensas" rnd2swap s& s" en el" s&
      suddenly|then$ s" caes en la cuenta" rnd2swap s& s" del" s&
  }s ^uppercase s" hecho" s" curioso" rnd2swap s& s& s" de que" s&
  s{  s{ s" supiera" s" conociera" }s{ s" cómo te llamas" s" tu nombre" }s&
      s" te llamara por tu nombre"
  }s& s" ..." s+ narrate  ;

: (conversation-0-with-ambrosio)  ( -- )
  \ Primera conversación con Ambrosio.
  s" Hola, buen hombre." speak
  ambrosio-introduces-himself scene-break
  you-cry scene-break
  ambrosio-proposes-a-deal narration-break
  ambrosio-let's-go narration-break
  ambrosio-is-gone
  talked-to-ambrosio  ;

: conversation-0-with-ambrosio  ( -- )
  \ Primera conversación con Ambrosio, si se dan las condiciones.
  location-19% am-i-there?
  ?? (conversation-0-with-ambrosio)  ;

: i-am-stuck-in-the-cave$  ( -- ca len )
  s{  s" por desgracia" s" desgraciadamente" s" desafortunadamente"
      s" tristemente" s" lamentablemente"
  }s? s{ s" estoy" s" me encuentro" s" me hallo" }s& ^uppercase
  s{ s" atrapado" s" encerrado" }s&
  s" en" s&{ s" la" s" esta" }s& s" cueva" s&
  s{ s" debido a" s" por causa de" s" por influjo de" }s&
  s{ s" una" s" cierta" }s& s" magia de" s&
  s{ s" maligno" s" maléfico" s" malvado" s" terrible" }s&
  s" poder." s&  ;

: you-must-follow-your-way$  ( -- ca len )
  s{ s" En cuanto" s" Por lo que respecta" }s&
  s" al camino, vos" s&
  s{ s" habéis de" s" debéis" s" habréis de" }s&
  s{ s" recorrer" s" seguir" s" hacer " }s& s" el vuestro," s&
  s{ s" ver" s" mirar" s" contemplar" }s s" lo" s?+
  s" todo con vuestros" s& s" propios" s?& s" ojos." s&  ;

: ambrosio-explains  ( -- )
  s" Ambrosio"
  s{  s" parece meditar un instante"
      s" asiente ligeramente con la cabeza"
  }s& s" y" s&
  s{  s" te" s{ s" dice" s" explica" }s&
      s" se explica"
  }s&
  colon+ narrate
  i-am-stuck-in-the-cave$ you-must-follow-your-way$ s& speak  ;

: i-can-not-understand-it$  ( -- ca len )
  s" no"
  s{  s" lo" s? s{ s" entiendo" s" comprendo" }s&
      s{ s" alcanzo" s" acierto" }s s" a" s&
         s{ s" entender" s" comprender" }s& s" lo" s?+
  }s&  ;

: you-shake-your-head  ( -- )
  s{ s" Sacudes" s" Mueves" s" Haces un gesto con" }s s" la cabeza" s&
  s{  s{ s" poniendo"  s" dejando" }s
        s{ s" clara" s" de manifiesto" s" patente" s" manifiesta" }s&
      s{ s" manifestando" s" delatando" s" mostrando" }s s" claramente" s?&
  }s s" tu" s&
  s{ s" sorpresa" s" perplejidad" s" resignación" s" incredulidad" }s&? s&
  colon+ narrate  ;

: you-don't-understand  ( -- )
  s{  i-can-not-understand-it$ s" , la verdad" s?+
      s{ s" la verdad" s" lo cierto" }s s" es que" s&
        i-can-not-understand-it$ s&
      s{ s" en verdad" s" realmente" s" verdaderamente" }s
        i-can-not-understand-it$ s&
  }s ^uppercase speak  ;

: you-already-had-the-key$  ( -- ca len )
  \ XXX TODO -- ampliar y variar
  s{
    s" La llave, Ambrosio, estaba ya en vuestro poder."
    s" Vos, Ambrosio, estabais ya en posesión de la llave."
    s" Vos, Ambrosio, ya teníais la llave en vuestro poder."
  }s  ;

: you-know-other-way$  ( -- ca len )
  s" Y" s{ s" por lo demás" s" por otra parte" }s?&
  s{ s" es" s" parece" }s&
  s{ s" obvio" s" evidente" s" claro" s" indudable" }s&
  s" que" s&{ s" conocéis" s" sabéis" s" no desconocéis" }s&
  s{ s" un" s" algún" s" otro" }s& s" camino" s&
  s{  s" más" s{ s" corto" s" directo" s" fácil" s" llevadero" }s&
      s" menos" s{ s" largo" s" luengo" s" difícil" s" pesado" }s&
  }s& period+  ;

: you-reproach-ambrosio  ( -- )
  \ Reprochas a Ambrosio acerca de la llave y el camino.
  you-already-had-the-key$ you-know-other-way$ s& speak  ;

: (conversation-1-with-ambrosio)  ( -- )
  \ Segunda conversación con Ambrosio.
  you-reproach-ambrosio ambrosio-explains
  you-shake-your-head you-don't-understand
  talked-to-ambrosio  ;

: conversation-1-with-ambrosio  ( -- )
  \ Segunda conversación con Ambrosio, si se dan las condiciones.
  location-46% am-i-there?
  ambrosio-follows? 0=  and
  ?? (conversation-1-with-ambrosio)  ;

: ambrosio-gives-you-the-key  ( -- )
  s{ s" Por favor," s" Os lo ruego," }s
  s" Ulfius," s&
  s" cumplid vuestra" s{ s" promesa." s" palabra." }s&
  s" Tomad" this|the(f)$ s& s" llave" s&
  s{ "" s" en vuestra mano" s" en vuestras manos" s" con vos" }s&
  s" y abrid" s& s" la puerta de" s?& this|the(f)$ s& s" cueva." s&
  speak
  key% is-hold  ;

: (conversation-2-with-ambrosio)  ( -- )
  \ Tercera conversación con Ambrosio.
  \ aquí en SuperBASIC: do-takeable the-key \ XXX TODO
  ambrosio-gives-you-the-key
  ambrosio-follows? on  talked-to-ambrosio  ;

: conversation-2-with-ambrosio  ( -- )
  \ Tercera conversación con Ambrosio, si se dan las condiciones.
  \ XXX TODO -- simplificar la condición
  location-45% 1- location-47% 1+ my-location within
  ?? (conversation-2-with-ambrosio)  ;

false [if]  \ Primera versión, con una estructura `case`
: (talk-to-ambrosio)  ( -- )
  \ Hablar con Ambrosio.
  ambrosio% conversations case
    0 of  conversation-0-with-ambrosio  endof
    1 of  conversation-1-with-ambrosio  endof
    2 of  conversation-2-with-ambrosio  endof
    \ XXX TODO -- implementar qué hacer cuando ya no hay más conversaciones
  endcase  ;

[else]  \ Segunda versión, más «estilo Forth»
create conversations-with-ambrosio
' (conversation-0-with-ambrosio) ,
' (conversation-1-with-ambrosio) ,
' (conversation-2-with-ambrosio) ,
' noop , \ XXX TODO -- implementar qué hacer cuando ya no hay más conversaciones
: (talk-to-ambrosio)  ( -- )
  \ Hablar con Ambrosio.
  ambrosio% conversations cells conversations-with-ambrosio + perform  ;

[then]
: talk-to-ambrosio  ( -- )
  \ Hablar con Ambrosio, si se puede.
  \ XXX TMP. esto debería comprobarse en `do-speak` o `do-speak-if-possible`
  ambrosio% is-here?
  if  (talk-to-ambrosio)  else  ambrosio% is-not-here  then  ;

\ ----------------------------------------------
\ Conversaciones sin éxito

: talk-to-something  ( a -- )
  \ Hablar con un ente que no es un personaje.
  \ XXX TODO
  2 random
  if    drop nonsense
  else  full-name s" hablar con" 2swap s& is-nonsense
  then  ;

: talk-to-yourself$  ( -- ca len )
  \ Devuelve una variante de «hablar solo».
  s{  s" hablar" s{ s" solo" s" con uno mismo" }s&
      s" hablarse" s{ s" a sí" s" a uno" }s& s" mismo" s?&
  }s  ;

: talk-to-yourself  ( -- )
  \ Hablar solo.
  talk-to-yourself$ is-nonsense  ;

\ ----------------------------------------------
\ Acciones

: do-speak-if-possible  ( a -- )
  \ Hablar con un ente si es posible.
  [debug] [if]  s" En DO-SPEAK-IF-POSSIBLE" debug  [then]  \ XXX INFORMER
  case
    leader% of  talk-to-the-leader  endof
    ambrosio% of  talk-to-ambrosio  endof
    dup talk-to-something
  endcase  ;

: (do-speak)  ( a | 0 -- )
  \ Hablar con alguien o solo.
  ?dup if  do-speak-if-possible  else  talk-to-yourself  then  ;

: (you-speak-to)  ( a -- )
  dup familiar++
  s" Hablas con" rot full-name s& colon+ narrate  ;

: you-speak-to  ( a | 0 -- )
  ?dup ?? (you-speak-to)  ;

:action do-speak
  \ Acción de hablar.
  [debug] [??] debug  \ XXX INFORMER
  main-complement{forbidden}
  actual-tool-complement{unnecessary}
  company-complement @ ?dup 0=  \ Si no hay complemento...
  ?? whom dup you-speak-to  \ ...buscar y mostrar el más probable.
  (do-speak)
  ;action
:action do-introduce-yourself
  \ Acción de presentarse a alguien.
  main-complement @ ?dup 0=  \ Si no hay complemento...
  ?? unknown-whom  \ ...buscar el (desconocido) más probable.
  (do-speak)
  ;action

\ }}}---------------------------------------------
subsection( Guardar el juego)  \ {{{

\ Para guardar el estado de la partida usaremos una solución
\ muy sencilla: ficheros de texto que reproduzcan el código
\ Forth necesario para restaurarlas. Esto permitirá
\ restaurar una partida con solo interpretar ese fichero
\ como cualquier otro código fuente.

0 [if] \ XXX TODO -- pendiente
: yyyymmddhhmmss$  ( -- ca len )
  \ Devuelve la fecha y hora actuales como una cadena en formato «aaaammddhhmmss».
  time&date >yyyymmddhhmmss  ;

: file-name$  ( -- ca len )
  \ Devuelve el nombre con que se grabará el juego.
  s" ayc-" yyyymmddhhmmss$ s+
  s" .exe" windows? and s+  ;  \ Añadir sufijo si estamos en Windows

defer reenter
svariable filename
: (save-the-game)  ( -- )
  \ Graba el juego.

  \ XXX TODO -- no está decidido el sistema que se usará para salvar las partidas

  \ 2011-12-01 No funciona bien. Muestra mensajes de gcc con
  \ parámetros sacados de textos del programa!

\ false to spf-init?  \ Desactivar la inicialización del sistema
\ true to console?  \ Activar el modo de consola (no está claro en el manual)
\ false to gui?  \ Desactivar el modo gráfico (no está claro en el manual)
  ['] reenter to <main>  \ Actualizar la palabra que se ejecutará al arrancar
\ file-name$ save  new-page
  file-name$ filename place filename count save  ;

: save-the-game
  \ Acción de salvar el juego.
  main-complement{forbidden}
  action ? key drop  \ XXX INFORMER
  (save-the-game)  ;

[then]

svariable game-file-name  \ Nombre del fichero en que se graba la partida
variable game-file-id  \ Identificador del fichero en que se graba la partida
: game-file-name$  ( -- ca len )
  \ Devuelve el nombre del fichero en que se graba la partida.
  game-file-name count  ;

: close-game-file  ( -- )
  \ Cierra el fichero en que se grabó la partida.
  game-file-id @ close-file abort" Close file error."  ;  \ XXX TMP -- mensaje tmp

: create-game-file  ( ca len -- )
  \ Crea un fichero para grabar una partida
  \ (sobreescribiendo otro que tuviera el mismo nombre).
  \ ca len = Nombre del fichero
  r/w create-file abort" Create file error."  \ XXX TMP -- mensaje tmp
  game-file-id !  ;

: read-game-file  ( ca len -- )
  \ Lee el fichero de configuración.
  \ ca len = Nombre del fichero
  \ XXX TODO -- comprobar la existencia del fichero y atrapar errores al leerlo
  only restore-vocabulary
  included  restore-vocabularies  ;

: >file/  ( ca len -- )
  \ Escribe una línea en el fichero de la partida.
  game-file-id @ write-line abort" Write file error"  ;  \ XXX TMP -- mensaje tmp

: cr>file  ( -- )
  \ Escribe un final de línea en el fichero de la partida.
  s" " >file/  ;

: >file  ( ca len -- )
  \ Escribe una cadena en el fichero de la partida.
  space+
  game-file-id @ write-file abort" Write file error"  ;  \ XXX TMP -- mensaje tmp

also restore-vocabulary  definitions
' \ alias \
immediate
' true alias true
' false alias false
' s" alias s"
: load-entity  ( x0 ... xn u -- )
  \ Restaura los datos de un ente.
  \ x0 ... xn = Datos del ente, en orden inverso a como los crea la palabra `save-entity`.
  \ u = Número ordinal del ente.
  #>entity >r
  \ cr .s  \ XXX INFORMER
  r@ ~direction !
  #>entity r@ ~in-exit !
  #>entity r@ ~out-exit !
  #>entity r@ ~down-exit !
  #>entity r@ ~up-exit !
  #>entity r@ ~west-exit !
  #>entity r@ ~east-exit !
  #>entity r@ ~south-exit !
  #>entity r@ ~north-exit !
  r@ ~familiar !
  r@ ~visits !
  #>entity r@ ~previous-location !
  #>entity r@ ~location !
  r@ ~owner !
  r@ ~flags-0 !
  r@ ~times-open !
  r@ ~conversations !
  \ 2dup cr type .s  \ XXX INFORMER
  r> name!  ;

: load-plot  ( x0 ... xn -- )
  \ Restaura las variables de la trama.
  \ Debe hacerse en orden alfabético inverso.
  recent-talks-to-the-leader !
  sword-forbidden? !
  stone-forbidden? !
  hacked-the-log? !
  climbed-the-fallen-away? !
  battle# !
  ambrosio-follows? !  ;

restore-vocabularies
: string>file  ( ca len -- )
  \ Escribe una cadena en el fichero de la partida.
  bs| s" | 2swap s+ bs| "| s+ >file  ;

: f>string  ( f -- ca len )
  \ Convierte un indicador binario en su nombre de constante.
  if  s" true"  else  s" false"  then  ;

: f>file  ( f -- )
  \ Escribe un indicador binario en el fichero de la partida.
  f>string >file  ;

: n>string  ( n -- ca len )
  \ Convierte un número con signo en una cadena.
  s>d swap over dabs
  <# #s rot sign #> >sb  ;

: u>string ( u -- ca len )
  \ Convierte un número sin signo en una cadena.
  s>d <# #s #> >sb  ;

: 00>s  ( u -- ca1 len1 )
  \ Convierte un número sin signo en una cadena (de dos dígitos como mínimo).
  s>d <# # #s #> >sb  ;

: 00>s+  ( u ca1 len1 -- ca2 len2 )
  \ Añade a una cadena un número tras convertirlo en cadena.
  rot 00>s s+  ;

: yyyy-mm-dd-hh:mm:ss$  ( -- ca len )
  \ Devuelve la fecha y hora actuales como una cadena en formato «aaaa-mm-dd-hh:mm:ss».
  time&date 00>s hyphen+ 00>s+ hyphen+ 00>s+ space+
  00>s+ colon+ 00>s+ colon+ 00>s+  ;

: n>file  ( n -- )
  \ Escribe un número con signo en el fichero de la partida.
  n>string >file  ;

: entity>file  ( a -- )
  \ Escribe la referencia a un ente en el fichero de la partida.
  \ a = Ente (dirección de su ficha)
  \ Esta palabra es necesaria porque no es posible guardar y restaurar
  \ las direcciones de ficha de los entes, pues variarán con cada
  \ sesión de juego. Hay que guardar los números ordinales de las
  \ fichas y con ellos calcular sus direcciones durante la restauración.
  entity># n>file  ;

: save-entity  ( u -- )
  \ Escribe los datos de un ente en el fichero de la partida.
  \ u = Número ordinal del ente.
  dup #>entity >r
  r@ name string>file
  r@ conversations n>file
  r@ times-open n>file
  r@ flags-0 n>file
  r@ owner n>file
  r@ location entity>file
  r@ previous-location entity>file
  r@ visits n>file
  r@ familiar n>file
  r@ north-exit entity>file
  r@ south-exit entity>file
  r@ east-exit entity>file
  r@ west-exit entity>file
  r@ up-exit entity>file
  r@ down-exit entity>file
  r@ out-exit entity>file
  r@ in-exit entity>file
  r> direction n>file
  n>file  \ Número ordinal del ente
  s" load-entity" >file/  ;  \ Palabra que hará la restauración del ente

: rule>file  ( -- )
  \ Escribe una línea de separación en el fichero de la partida.
  s" \ ----------------------------------------------------" >file/  ;

: section>file  ( ca len -- )
  \ Escribe el título de una sección en el fichero de la partida.
  cr>file rule>file s" \ " >file >file/ rule>file cr>file  ;

: save-entities  ( -- )
  \ Escribe los datos de los entes en el fichero de la partida.
  s" Entes" section>file
  #entities 0 do  i save-entity  loop  ;

: save-config  ( -- )
  \ Escribe los valores de configuración en el fichero de la partida.
  \ XXX TODO
  s" Configuración" section>file  ;

: save-plot  ( -- )
  \ Escribe las variables de la trama en el fichero de la partida.
  \ Debe hacerse en orden alfabético.
  s" Trama" section>file
  ambrosio-follows? @ f>file
  battle# @ n>file
  climbed-the-fallen-away? @ f>file
  hacked-the-log? @ f>file
  stone-forbidden? @ f>file
  sword-forbidden? @ f>file
  recent-talks-to-the-leader @ n>file
  s" load-plot" >file/  ;  \ Palabra que hará la restauración de la trama

: file-header  ( -- )
  \ Escribe la cabecera del fichero de la partida.
  s" \ Datos de restauración de una partida de «Asalto y castigo»" >file/
  s" \ (http://pragramandala.net/es.programa.asalto-y-castigo.forth)" >file/
  s" \ Fichero creado en" yyyy-mm-dd-hh:mm:ss$ s& >file/  ;

: write-game-file  ( -- )
  \ Escribe el contenido del fichero de la partida.
  file-header save-entities save-config save-plot  ;

: fs+  ( ca len -- a' u' )
  \ Añade la extensión .fs a un nombre de fichero.
  s" .fs" s+  ;

: (save-the-game)  ( ca len -- )
  \ Salva la partida.
  fs+ create-game-file write-game-file close-game-file  ;

: save-the-game  ( ca len -- )
  \ Acción de salvar la partida.
  \ main-complement{forbidden}
  (save-the-game)  ;

: continue-the-loaded-game  ( -- )
  \ Continúa el juego en el punto que se acaba de restaurar.
  scene-break new-page
  my-location describe-location  ;

: load-the-game  ( ca len -- )
  \ Acción de salvar la partida.
  \ XXX TODO -- no funciona bien
  \ main-complement{forbidden}
  only restore-vocabulary
  [debug-filing] [??] ~~
  \ included  \ XXX FIXME -- el sistema estalla
  \ 2drop  \ XXX NOTE: sin error
  \ cr type  \ XXX NOTE: sin error
  2>r save-input 2r>
  [debug-filing] [??] ~~
  fs+
  [debug-filing] [??] ~~
[false] [if]  \ XXX INFORMER
  read-game-file
[else]
  ['] read-game-file
  [debug-filing] [??] ~~
  catch
  [debug-filing] [??] ~~
  restore-vocabularies
  [debug-filing] [??] ~~
  ?dup if
    ( ca len u2 ) nip nip
    case  \ XXX TMP
      2 of  s" Fichero no encontrado." narrate  endof
      s" Error al intentar leer el fichero." narrate
    endcase
    [debug-filing] [??] ~~
  then
[then]
  [debug-filing] [??] ~~
  restore-input
  if
    \ XXX TMP
    s" Error al intentar restaurar la entrada tras leer el fichero." narrate
  then
  [debug-filing] [??] ~~
  continue-the-loaded-game  ;

\ }}} ==========================================================
section( Acciones de configuración)  \ {{{

\ XXX TMP -- esto no soluciona el problema

\ :action do-color  ( -- )
\   init-colors  new-page  my-location describe
\   ;action

: recolor  ( -- )
  init-colors  new-page  my-location describe  ;

defer finish

\ }}}
\ }}} ==========================================================
section( Intérprete de comandos)  \ {{{

\ Gracias al uso del propio intérprete de Forth como intérprete de
\ comandos del juego, más de la mitad del trabajo ya está hecha por
\ anticipado. Para ello basta crear las palabras del vocabulario del
\ juego como palabras propias de Forth y hacer que Forth interprete
\ directamente la entrada del jugador. Creando las palabras en un
\ vocabulario de Forth específico para ellas, y haciendo que sea el
\ único vocabulario activo en el momento de la interpretación, solo las
\ palabras del juego serán reconocidas, no las del programa ni las del
\ sistema Forth.
\
\ Sin embargo hay una consideración importante: Al pasarle directamente
\ al intérprete de Forth el texto del comando escrito por el jugador,
\ Forth ejecutará las palabras que reconozca (haremos que las no
\ reconocidas las ignore) en el orden en que estén escritas en la frase.
\ Esto quiere decir que, al contrario de lo que ocurre con otros
\ métodos, no podremos tener una visión global del comando del jugador:
\ ni de cuántas palabras consta ni, en principio, qué viene a
\ continuación de la palabra que está siendo interpretada en cada
\ momento.
\
\ Una solución sería que cada palabra del jugador guardara un
\ identificador unívoco en la pila o en una tabla, y posteriormente
\ interpretáramos el resultado de una forma convencional.
\
\ Sin embargo, hemos optado por dejar a Forth hacer su trabajo hasta el
\ final, pues nos parece más sencillo y eficaz [también es más propio
\ del espíritu de Forth usar su intérprete como intérprete de la
\ aplicación en lugar de programar un intérprete adicional específico].
\ Las palabras reconocidas en el comando del jugador se ejecutarán pues
\ en el orden en que fueron escritas. Cada una actualizará el elemento
\ del comando que represente, verbo o complemento, tras comprobar si ya
\ ha habido una palabra previa que realice la misma función y en su caso
\ deteniendo el proceso con un error.

variable 'prepositions#  \ Número de (seudo)preposiciones
: prepositions#  ( -- n )
  \ Número de (seudo)preposiciones.
  'prepositions# @  ;

: >bit  ( u1 -- u2 )
  \ u2 = número cuyo único bitio activo es aquel cuyo orden
  \ indica u1 (ej. 1->1, 2->2, 3->4, 4->8...)
  1 swap lshift  ;

: preposition:  ( "name1" "name2" -- )
  \ Crea los identificadores de una (seudo)preposición
  \ y actualiza el contador.
  \ "name1" = nombre del identificador para usar como máscara de bitios
  \ "name2" = nombre del identificador para usar como índice de tabla
  prepositions# >bit constant
  'prepositions# ++ prepositions# constant  ;

\ Constantes para los identificadores de (seudo)preposiciones:
preposition: «con»-preposition-bit «con»-preposition#
preposition: «usando»-preposition-bit «usando»-preposition#
false [if]  \ XXX TODO -- unfinished
preposition: «a»-preposition-bit «a»-preposition#
preposition: «contra»-preposition-bit «contra»-preposition#
preposition: «de»-preposition-bit «de»-preposition#
preposition: «en»-preposition-bit «en»-preposition#
preposition: «hacia»-preposition-bit «hacia»-preposition#
preposition: «para»-preposition-bit «para»-preposition#
preposition: «por»-preposition-bit «por»-preposition#
preposition: «sin»-preposition-bit «sin»-preposition#  \ XXX NOTE: para dejar cosas antes de la acción
[then]
( n )
\ Octetos necesarios para guardar las (seudo)preposiciones en la
\ tabla:
prepositions# cells constant /prepositional-complements
\ Tabla de complementos (seudo)preposicionales:
create prepositional-complements /prepositional-complements allot

\ Las (seudo)preposiciones permitidas en el juego pueden tener usos
\ diferentes, y algunos de ellos dependen del ente al que se refieran,
\ por lo que su análisis hay que hacerlo en varios niveles.
\
\ Decimos «(seudo)preposiciones» porque algunos de los términos usados
\ como preposiciones no lo son [como por ejemplo «usando», que es un
\ gerundio] pero se usan como si lo fueran.
\
\ Los identificadores creados arriba se refieren a (seudo)preposiciones
\ del vocabulario de juego (por ejemplo, «a», «con»...) o a sus
\ sinónimos, no a sus posibles usos finales como complementos [por
\ ejemplo, destino de movimiento, objeto indirecto, herramienta,
\ compañía...]. Por ejemplo, el identificador `«a»-preposition` se usa
\ para indicar (en la tabla) que se ha encontrado la preposición «a» [o
\ su sinónimo «al»], pero el significado efectivo [por ejemplo, indicar
\ una dirección o un objeto indirecto o un objeto directo de persona, en
\ este caso] se calculará en una etapa posterior.
\
\ Cada elemento de la tabla de complementos (seudo)preposicionales
\ representa una (seudo)preposición [incluidos evidentemente sus
\ sinónimos]; será apuntado pues por un identificador de
\ (seudo)preposición y contendrá el identificador del ente que haya sido
\ usado en el comando con dicha (seudo)preposición, o bien cero si la
\ (seudo)preposición no ha sido utilizada hasta el momento.

: erase-prepositional-complements  ( -- )
  \ Borra la tabla de complementos (seudo)preposicionales.
  prepositional-complements /prepositional-complements erase  ;

: prepositional-complement  ( u -- a )
  \ Devuelve la dirección de un elemento de la tabla
  \ de complementos (seudo)preposicionales.
  \ u = Número de la preposición
  \ a = Dirección en la tabla de complementos (seudo)preposicionales
  1- cells prepositional-complements +  ;

: current-prepositional-complement  ( -- a )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente a la (seudo)preposición abierta.
  \ a = Dirección en la tabla de complementos (seudo)preposicionales
  current-preposition @ prepositional-complement  ;

: (company-complement)  ( -- a )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento de compañía
  \ (complemento que puede ser cero si no existe).
  «con»-preposition# prepositional-complement  ;

' (company-complement) is company-complement
: (actual-company-complement)  ( -- a|0 )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento de compañía estricto
  \ (complemento que puede ser cero si no existe).
  \ XXX TODO -- experimental, ojo: puede devolver cero
  «usando»-preposition# prepositional-complement @ dup 0<>
  if  drop company-complement  then  ;

' (actual-company-complement) is actual-company-complement
: (actual-tool-complement)  ( -- a )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento instrumental estricto
  \ (complemento que puede ser cero si no existe).
  «usando»-preposition# prepositional-complement  ;

' (actual-tool-complement) is actual-tool-complement
: (tool-complement)  ( -- a )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento instrumental
  \ (complemento que puede ser cero si no existe).
  actual-tool-complement dup @ 0=
  if  drop company-complement  then  ;

' (tool-complement) is tool-complement
: prepositions-off  ( -- )
  \ Inicializa las preposiciones.
  erase-prepositional-complements
  current-preposition off
  used-prepositions off  ;

: complements-off  ( -- )
  \ Inicializa los complementos.
  main-complement off
  secondary-complement off
  prepositions-off  ;

: init-parsing  ( -- )
  \ Preparativos previos a cada análisis.
  action off  complements-off  ;

: (execute-action)  ( xt -- )
  \ Ejecuta la acción del comando.
  dup previous-action !
  [debug-catch] [debug-parsing] [or] [??] ~~
  catch  \ Ejecutar la acción a través de CATCH para poder regresar directamente con THROW en caso de error
  [debug-catch] [debug-parsing] [or] [??] ~~
  ?wrong
  [debug-catch] [debug-parsing] [or] [??] ~~  ;

: (execute-previous-action)  ( -- )
  \ Ejecuta la acción previa, si es posible
  \ (no es posible la primera vez, cuando su valor aún es cero).
  \ XXX NOTE: otra solución posible: inicializar la variable con una acción que nada haga.
  previous-action @ ?dup if  (execute-action)  then  ;

: execute-previous-action  ( -- )
  \ Ejecuta la acción previa, si así está configurado.
  repeat-previous-action? @
  if  (execute-previous-action)  else  no-verb-error# ?wrong  then  ;

: execute-action  ( -- )
  \ Ejecuta la acción del comando, si es posible.
  [debug-catch] [debug-parsing] [or] [??] ~~
  action @ ?dup
  [debug-catch] [debug-parsing] [or] [??] ~~
  if    (execute-action)
  else  execute-previous-action
  then
  [debug-catch] [debug-parsing] [or] [??] ~~  ;

: (evaluate-command)  ( -- )
  \ Analiza la fuente actual, ejecutando las palabras reconocidas que contenga.
  begin   parse-name ?dup
  while   find-name ?dup if  name>int execute  then
  repeat  drop  ;

: evaluate-command  ( ca len -- )
  \ Analiza el comando, ejecutando las palabras reconocidas que contenga.
  \ ." comando:" 2dup cr type  \ XXX INFORMER
  ['] (evaluate-command) execute-parsing  ;

: a-preposition-is-open?  ( -- f )
  \ ¿Hay un complemento (seudo)preposicional abierto?
  current-preposition @ 0<>  ;

: no-parsing-error-left?  ( -- f )
  \ ¿No quedó algún error pendiente tras el análisis?
  \ Comprueba si quedó un complemento (seudo)preposicional
  \ incompleto, algo que no puede detectarse en el análisis
  \ principal.
  \ f = ¿No quedó error pendiente tras el análisis?
  \      (true=ningún error pendiente; false=algún error pendiente)
  a-preposition-is-open? dup
  unresolved-preposition-error# and ?wrong  0=  ;

[debug-parsing-result] [if]
  : .complement?  ( ca1 len1 a2 -- )  \ XXX INFORMER
    \ Imprime un nombre de complemento, con un texto previo, si existe.
    @ ?dup
    if    name s& paragraph
    else  2drop
    then
    ;
[then]
: valid-parsing?  ( ca len -- f )
  \ Evalúa un comando con el vocabulario del juego.
  \ ca len = Comando
  \ f = ¿El comando se analizó sin error?
  -punctuation
  [debug-parsing] [??] ~~
  \ Dejar solo el diccionario PLAYER-VOCABULARY activo
  only player-vocabulary
  \ [debug-catch] [if]  s" En VALID-PARSING? antes de preparar CATCH" debug  [then]  \ XXX INFORMER
  [debug-parsing] [??] ~~
  ['] evaluate-command catch
  [debug-parsing] [??] ~~
  dup if  nip nip  then  \ Arreglar la pila, pues CATCH hace que apunte a su posición previa
  [debug-parsing] [??] ~~
  dup ?wrong 0=
  [debug-parsing] [??] ~~
  restore-vocabularies
  no-parsing-error-left? and
  [debug-parsing] [??] ~~
  [debug-parsing-result] [if]
    s" Main           : " main-complement .complement?
    s" Secondary      : " secondary-complement .complement?
    s" Tool           : " tool-complement .complement?
    s" Actual tool    : " actual-tool-complement .complement?
    s" Company        : " company-complement .complement?
    \ s" Actual company : " actual-company-complement .complement? \ XXX TMP -- experimental
   [then]  ;

: >but-one!  ( a -- )
  \ Copia un complemento de la zona «últimos» a la «penúltimos»
  \ de la tabla `last-complement`.
  \ a = Dirección en la zona «últimos» de la tabla `last-complement`.
  dup @ swap >but-one !  ;

: shift-last-complement  ( a -- )
  \ Copia el último complemento al lugar del penúltimo.
  \ a = Ente que fue encontrado como último complemento.
  >last-complement >but-one!  \ El último del mismo género y número
  last-complement >but-one!  ;  \ El último absoluto

: new-last-complement  ( a -- )
  \ Guarda un nuevo complemento como el último complemento hallado.
  dup shift-last-complement  \ Copiar último a penúltimo
  dup last-complement !  \ Guardarlo como último absoluto
  dup >last-complement !  ;  \ Guardarlo como último de su género y número

: save-command-elements  ( -- )
  action @ last-action !  ;
  \ XXX TODO -- not used
  \ XXX TODO -- falta guardar los complementos

: (obey)  ( ca len -- )
  \ Evalúa un comando con el vocabulario del juego.
  init-parsing valid-parsing? ?? execute-action  ;

: obey  ( ca len -- )
  \ Evalúa un comando con el vocabulario del juego, si no está vacío.
  [debug-parsing] [??] ~~
  dup if  (obey)  else  2drop  then
  [debug-parsing] [??] ~~  ;

: second?  ( x1 x2 -- x1 f )
  \ ¿La acción o el complemento son los segundos que se encuentran?
  \ Los dos valores representan una acción (xt) o un ente (a).
  \ x1 = Acción o complemento recién encontrado
  \ x2 = Acción o complemento anterior, o cero
  [debug-parsing] [??] ~~
  2dup different?  \ ¿Hay ya otro anterior y es diferente?
  [debug-parsing] [??] ~~  ;

: action!  ( xt -- )
  \ Comprueba y almacena la acción.
  \ xt = Identificador de ejecución de la acción
  [debug-parsing] [??] ~~
  action @ second?  \ ¿Había ya una acción?
  [debug-parsing] [??] ~~
  too-many-actions-error# and
  [debug-parsing] [??] ~~
  throw  \ Sí, error
  [debug-parsing] [??] ~~
  action !  \ No, guardarla
  [debug-parsing] [??] ~~  ;

: preposition!  ( u -- )
  \ Almacena una (seudo)preposición recién hallada en la frase.
  \ u = Identificador de la preposición
  a-preposition-is-open?  \ ¿Hay ya una preposición abierta?
  unresolved-preposition-error# and throw  \ Si es así, error
  current-preposition !  ;

: prepositional-complement!  ( a -- )
  \ Almacena un complemento (seudo)preposicional.
  \ a = Identificador de ente
  [debug-parsing] [??] ~~
  current-prepositional-complement @ second?  \ ¿Se había usado ya esta preposición con otro complemento?
  repeated-preposition-error# and throw  \ Si es así, error
  dup new-last-complement
  current-prepositional-complement !
  current-preposition @ >bit used-prepositions +!  \ Añadir la preposición a las usadas
  current-preposition off  \ Cerrar la preposición en curso
  [debug-parsing] [??] ~~  ;

: secondary-complement!  ( a -- )
  \ Almacena el complemento secundario.
  \ a = Identificador de ente
  secondary-complement @ second?  \ ¿Había ya un complemento secundario?
  too-many-complements-error# and throw  \ Si es así, error
  secondary-complement !  ;

: main-complement!  ( a -- )
  \ Almacena el complemento principal.
  \ a = Identificador de ente
  [debug-parsing] [??] ~~
  main-complement @ second?  \ ¿Había ya un complemento principal?
  too-many-complements-error# and throw  \ Si es así, error
  dup new-last-complement
  main-complement !  ;

: non-prepositional-complement!  ( a -- )
  \ Almacena un complemento principal o secundario.
  \ a = Identificador de ente
  \ XXX TODO -- esta palabra sobrará cuando las (seudo)preposiciones estén implementadas completamente
  main-complement @
  if    secondary-complement!
  else  main-complement!
  then  ;

: (complement!)  ( a -- )
  a-preposition-is-open?
  if    prepositional-complement!
  else  non-prepositional-complement!  then  ;
  \ Almacena el ente _a_ como complemento.

: complement!  ( a | 0 -- )
  ?dup ?? (complement!)  ;
  \ Comprueba y almacena un complemento.
  \ a = Identificador de ente,
  \     o cero si se trata de un pronombre sin referente.

: action|complement!  ( xt a -- )
  action @
  a-preposition-is-open? or
  if  nip complement!  else  drop action!  then  ;
  \ Comprueba y almacena un complemento _a_ o una acción _xt_,
  \ ambos posibles significados de la misma palabra.

\ }}} ==========================================================
section( Fichero de configuración)  \ {{{

\ El juego tiene un fichero de configuración en que el jugador
\ puede indicar sus preferencias. Este fichero es código en
\ Forth y se interpreta directamente, pero en él solo serán
\ reconocidas las palabras creadas expresamente para la
\ configuración, así como las palabras habituales para hacer
\ comentarios de bloques o líneas en Forth. Cualquier otra
\ palabra dará error.
\
\ El fichero de configuración se lee al inicio de cada
\ partida.

sourcepath 2constant path$

: config-file$  ( -- ca len )
  \ Fichero de configuración.
  \ XXX FIXME
  \ `.` está en `fpath` en mi gforth
  \ s" ./ayc.ini.fs" \ lo encuentra al arrancar desde el dir del programa; no desde otro
  \ s" ayc.ini.fs" \ ídem
  \ Tras hacer un enlace /usr/share/gforth/site-forth/ayc al dir del proyecto:
  \ s" ayc/ayc.ini.fs" \ lo encuentra desde cualquier dir
  path$ s" ayc.ini.fs" s+  ;

svariable command-prompt
variable space-after-command-prompt?  \ ¿Separar el presto de comandos con un espacio posterior?
variable cr-after-command-prompt?  \ ¿Hacer un salto de línea tras el presto de comando?
: verbosity-range  ( n -- n' )
  \ Asegura que un nivel de detalle de error está entre los
  \ límites.
  min-errors-verbosity max max-errors-verbosity min  ;

also config-vocabulary  definitions

\ Las palabras cuyas definiciones siguen a continuación
\ se crearán en el vocabulario `config-vocabulary` y
\ son las únicas que podrán usarse para configurar el juego
\ en el fichero de configuración:

[false] [if]  \ En vez de así:
: (  ( "texto<cierre de paréntesis>" -- )
  \ Comentario clásico
  postpone (
  ;  immediate
: \  ( "texto<fin de línea>" -- )
  \ Comentario de línea
  postpone \
  ;  immediate
[else]  \ Es más sencillo así:
' ( alias (  \ Cerramos paréntesis solo para no arruinar el coloreado de sintaxis: )
immediate
' \ alias \
immediate
[then]
' true alias sí
' false alias no
: varón  ( -- )
  \ Configura que el jugador es un varón.
  woman-player? off  ;

' varón alias masculino
: mujer  ( -- )
  \ Configura que el jugador es una mujer.
  woman-player? on  ;

' mujer alias femenino
: comillas  ( f -- )
  \ Configura si se usan las comillas castellanas en las citas.
  castilian-quotes? !  ;

: espacios_de_indentación  ( u -- )
  \ Fija la indentación de los párrafos.
  max-indentation min /indentation !  ;

: indentar_primera_línea_de_pantalla  ( f -- )
  \ Configura si se indentará también la línea superior de la pantalla, si un párrafo empieza en ella.
  indent-first-line-too? !  ;

: indentar_prestos_de_pausa  ( f -- )
  \ Configura si se indentarán los prestos.
  indent-pause-prompts? !  ;

: borrar_pantalla_para_escenarios  ( f -- )
  \ Configura si se borra la pantalla al entrar en un escenario o describirlo.
  location-page? !  ;

: borrar_pantalla_para_escenas  ( f -- )
  \ Configura si se borra la pantalla tras la pausa de fin de escena.
  scene-page? !  ;

: separar_párrafos  ( f -- )
  \ Configura si se separan los párrafos con un línea en blanco.
  cr? !  ;

: segundos_en_pausas_de_narración  ( u -- )
  \ Configura los segundos de las pausas cortas (o, si es valor es cero, que hay que pulsar una tecla).
  narration-break-seconds !  ;

: segundos_en_pausas_de_final_de_escena  ( u -- )
  \ Configura los segundos de las pausas de final de esecena (o, si es valor es cero, que hay que pulsar una tecla).
  scene-break-seconds !  ;

' black alias negro
' blue alias azul
' light_blue alias azul_claro
' brown alias marrón
' cyan alias cian
' light_cyan alias cian_claro
' green alias verde
' light_green alias verde_claro
' gray alias gris
' light_gray alias gris_claro
' magenta alias magenta
' light_magenta alias magenta_claro
' red alias rojo
' light_red alias rojo_claro
' white alias blanco
' yellow alias amarillo

: papel_de_fondo  ( u -- )
  [defined] background-paper
  [if]  background-paper !  [else]  drop  [then]  ;

: tinta_de_créditos  ( u -- )  about-ink !  ;
: papel_de_créditos  ( u -- )  about-paper !  ;
: tinta_de_presto_de_comandos  ( u -- )  command-prompt-ink !  ;
: papel_de_presto_de_comandos  ( u -- )  command-prompt-paper !  ;
: tinta_de_depuración  ( u -- )  debug-ink !  ;
: papel_de_depuración  ( u -- )  debug-paper !  ;
: tinta_de_descripción  ( u -- )  description-ink !  ;
: papel_de_descripción  ( u -- )  description-paper !  ;
: tinta_de_error_lingüístico  ( u -- )  language-error-ink !  ;
: papel_de_error_lingüístico  ( u -- )  language-error-paper !  ;
: tinta_de_error_operativo  ( u -- )  action-error-ink !  ;
: papel_de_error_operativo  ( u -- )  action-error-paper !  ;
: tinta_de_error_del_sistema  ( u -- )  system-error-ink !  ;
: papel_de_error_del_sistema  ( u -- )  system-error-paper !  ;
: tinta_de_entrada  ( u -- )  input-ink !  ;
: papel_de_entrada  ( u -- )  input-paper !  ;
: tinta_de_descripción_de_escenario  ( u -- )  location-description-ink !  ;
: papel_de_descripción_de_escenario  ( u -- )  location-description-paper !  ;
: tinta_de_nombre_de_escenario  ( u -- )  location-name-ink !  ;
: papel_de_nombre_de_escenario  ( u -- )  location-name-paper !  ;
: tinta_de_narración  ( u -- )  narration-ink !  ;
: papel_de_narración  ( u -- )  narration-paper !  ;
: tinta_de_presto_de_pantalla_llena  ( u -- )  scroll-prompt-ink !  ;
: papel_de_presto_de_pantalla_llena  ( u -- )  scroll-prompt-paper !  ;
: tinta_de_pregunta  ( u -- )  question-ink !  ;
: papel_de_pregunta  ( u -- )  question-paper !  ;
: tinta_de_presto_de_escena  ( u -- )  scene-prompt-ink !  ;
: papel_de_presto_de_escena  ( u -- )  scene-prompt-paper !  ;
: tinta_de_presto_de_pausa_de_narración  ( u -- )  narration-prompt-ink !  ;
: papel_de_presto_de_pausa_de_narración  ( u -- )  narration-prompt-paper !  ;
: tinta_de_diálogos  ( u -- )  speech-ink !  ;
: papel_de_diálogos  ( u -- )  speech-paper !  ;

\ Prestos
' s" alias s"
: presto_de_pantalla_llena  ( ca len -- )  scroll-prompt place  ;
: presto_de_pausa_de_narración  ( ca len -- )  narration-prompt place  ;
: presto_de_fin_de_escena  ( ca len -- )  scene-prompt place  ;
: presto_de_comando  ( ca len -- )  command-prompt place  ;
: espacio_tras_presto_de_comando  ( f -- )  space-after-command-prompt? !  ;
: nueva_línea_tras_presto_de_comando  ( f -- )  cr-after-command-prompt? !  ;

: detalle_de_los_mensajes_de_error_lingüístico  ( n -- )
  \ Configura el nivel de detalle de los mensajes de error lingüístico.
  verbosity-range language-errors-verbosity !  ;

: mensaje_genérico_de_error_lingüístico  ( ca len -- )
  \ Configura el mensaje genérico para los mensajes de error lingüístico.
  'language-error-general-message$ place  ;

: detalle_de_los_mensajes_de_error_operativo  ( n -- )
  \ Configura el nivel de detalle de los mensajes de error operativo.
  verbosity-range action-errors-verbosity !  ;

: mensaje_genérico_de_error_operativo  ( ca len -- )
  \ Configura el mensaje genérico para los mensajes de error operativo.
  'action-error-general-message$ place  ;

: repetir_la_última_acción  ( f -- )
  \ Configura si hay que usar acción anterior cuando no se
  \ especifica otra en el comando.
  repeat-previous-action? !  ;

\ Fin de las palabras permitidas en el fichero configuración.

restore-vocabularies

: init-prompts  ( -- )
  \ Inicializa los prestos con sus valores predeterminados.
  indent-pause-prompts? on
  s" ..." scroll-prompt place
  s" ..." narration-prompt place
  s" ..." scene-prompt place
  s" >" command-prompt place
  space-after-command-prompt? on
  cr-after-command-prompt? off  ;

: init-config  ( -- )
  \ Inicializa las variables de configuración con sus valores predeterminados.
  woman-player? off
  castilian-quotes? on
  location-page? on
  cr? off
  ignore-unknown-words? off
  default-indentation /indentation !
  indent-first-line-too? on
  -1 narration-break-seconds !
  -1 scene-break-seconds !
  scene-page? on
  max-errors-verbosity language-errors-verbosity !
  max-errors-verbosity action-errors-verbosity !
  s" Orden incorrecta." 'language-error-general-message$ place
  s" No es posible hacer eso." 'action-error-general-message$ place
  repeat-previous-action? on
  init-prompts  init-colors  ;

: read-config-error  ( -- )
  s" Se ha producido un error leyendo el fichero de configuración."
  system-error wait  ;

: read-config  ( -- )
  \ Lee el fichero de configuración.
  \ XXX TODO -- atrapar errores al leerlo
  base @ decimal
  only config-vocabulary
  config-file$ 2dup type ['] included catch  ( x1 x2 n | 0 )
  if  2drop  read-config-error  then
  restore-vocabularies  base !  ;

: get-config  ( -- )
  \ Inicializa las variables de configuración
  \ y lee el fichero de configuración.
  init-config read-config  ;

\ }}} ==========================================================
section( Herramientas para crear el vocabulario del juego)  \ {{{

\ El vocabulario del juego está implementado como un
\ vocabulario de Forth, creado con el nombre de
\ `player-vocabulary`.  La idea es muy sencilla: crearemos en
\ este vocabulario nuevo palabras de Forth cuyos nombres sean
\ las palabras españolas que han de ser reconocidas en los
\ comandos del jugador. De este modo bastará interpretar la
\ frase del jugador con la palabra estándar EVALUATE
\ [o, según el sistema Forth de que se trate, con la palabra
\ escrita a medida EVALUATE-COMMAND ], que ejecutará
\ cada palabra que contenga el texto.

\ ---------------------------------------------
\ Resolución de entes ambiguos

\ Algunos términos del vocabulario del jugador pueden
\ referirse a varios entes. Por ejemplo, «hombre» puede
\ referirse al jefe de los refugiados o a Ambrosio,
\ especialmente antes de que Ulfius hable con él por primera
\ vez y sepa su nombre.  Otra palabra, como «ambrosio», solo
\ debe ser reconocida cuando Ambrosio ya se ha presentado
\ y ha dicho su nombre.
\
\ Para estos casos creamos palabras que devuelven el ente
\ adecuado en función de las circunstancias.  Serán llamadas
\ desde la palabra correspondiente del vocabulario del
\ jugador.
\
\ Si la ambigüedad no puede ser resuelta, o si la palabra ambigua
\ no debe ser reconocida en las circunstancias de juego actuales,
\ se devolverá un `false`, que tendrá el mismo efecto que si la
\ palabra ambigua no existiera en el comando del jugador. Esto
\ provocará después el error adecuado.
\
\ Las acciones ambiguas, como por ejemplo «partir» [que puede
\ significar «marchar» o «romper»] no pueden ser resueltas de
\ esta manera, pues antes es necesario que que todos los
\ términos de la frase hayan sido evaluados. Por ello se
\ tratan como si fueran acciones como las demás, pero que al
\ ejecutarse resolverán la ambigüedad y llamarán a la acción
\ adecuada.

: (man) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «hombre» y sus sinónimos
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ Puede referirse al líder de los refugiados (si está presente),
  \ a Ambrosio (si está presente),
  \ o a los soldados (durante la marcha o la batalla).
  true case
    leader% is-here? of  leader%  endof
    ambrosio% is-here? of  ambrosio%  endof
    pass-still-open? battle? or of  soldiers%  endof
    false swap
  endcase  ;

: (men)  ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «hombres» y sus sinónimos
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ Puede referirse a los soldados o a los refugiados.
  [false] [if] \ Primera versión.
    true case
      location-28% am-i-there? location-29% am-i-there? or of  refugees%  endof
      pass-still-open? battle? or of  soldiers%  endof
      false swap
    endcase
  [else]  \ Segunda versión, lo mismo pero sin `case`:
    location-28% am-i-there? location-29% am-i-there? or
    if  refugees% exit  then
    pass-still-open? battle? or
    if  soldiers% exit  then
    false
  [then]  ;

: (ambrosio) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «ambrosio»
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ La palabra «Ambrosio» es válida únicamente si
  \ el protagonista ha hablado con Ambrosio.
  ambrosio% dup conversations? and  ;

: (cave) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «cueva»
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  true case
    my-location location-10% location-47% between of  cave%  endof
    the-cave-entrance-is-accessible? of  cave-entrance%  endof
    false swap
  endcase  ;

: (entrance) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «entrada»
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  true case
    the-cave-entrance-is-accessible? of  cave-entrance%  endof
    \ XXX TODO -- quizá no se implemente esto porque precisaría asociar a cave-entrance% el vocablo «salida/s», lo que crearía una ambigüedad adicional que resolver:
    \ location-10% am-i-there? of  cave-entrance%  endof
    false swap
  endcase  ;

: (exits)  ( -- a )
  \ Devuelve el ente adecuado a la palabra «salida/s».
  the-cave-entrance-is-accessible?
  if    cave-entrance%
  else  exits%
  then  ;

: (stone) ( -- a )
  \ Devuelve el ente adecuado a la palabra «piedra».
  \ Puede referise, en orden preferente,
  \ a la piedra, a la esmeralda, a la pared de roca del desfiladero o a las rocas.
  true case
    stone% is-accessible? of  stone%  endof
    emerald% is-accessible? of  emerald%  endof
    location-08% am-i-there? of  ravine-wall%  endof
    rocks% swap
  endcase  ;

: (wall) ( -- a )
  \ Devuelve el ente adecuado a la palabra «pared».
  \ XXX TODO -- probablemente habrá que añadir más casos
  location-08% am-i-there?
  if    ravine-wall%
  else  wall%
  then  ;

: (somebody) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «alguien».
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ Puede referirse a los soldados, a los refugiados
  \ o a ambrosio.
  true case
    pass-still-open? battle? or of  soldiers%  endof
    location-28% am-i-there? location-29% am-i-there? or of  refugees%  endof
    ambrosio% is-here? of  ambrosio%  endof
    false swap
  endcase  ;

: (bridge)  ( -- a )
  \ Devuelve el ente adecuado a la palabra «puente».
  true case
    location-13% am-i-there? of  bridge%  endof
    location-18% am-i-there? of  arch%  endof
    bridge% is-known? of bridge%  endof
    arch% is-known? of arch%  endof
    false swap
  endcase  ;

\ }}} ==========================================================
section( Vocabulario del juego)  \ {{{

\ Elegir el vocabulario `player-vocabulary` para crear en él las nuevas palabras:
also player-vocabulary definitions

\ ----------------------------------------------
\ Pronombres

\ De momento no se implementan las formas sin tilde
\ porque obligarían a distinguir sus usos como adjetivos
\ o sustantivos.

\ XXX TODO
\ esto/s eso/s y aquello/s podrían implementarse como sinónimos
\ de las formas masculinas o bien referirse al último y penúltimo
\ complemento usado, de cualquier género, pero para ello la estructura
\ de la tabla `last-complement` debería ser modificada.
\ : esto  last-complement @ complement!  ;
\ : aquello  last-but-one-complement @ complement!  ;

: éste  last-complement >masculine >singular @ complement!  ;
' éste aliases: Éste ése Ése ;aliases
: ésta  last-complement >feminine >singular @ complement!  ;
' ésta aliases: Ésta ésa Ésa ;aliases
: éstos  last-complement >masculine >plural @ complement!  ;
' éstos aliases: Éstos ésos Ésos ;aliases
: éstas  last-complement >feminine >plural @ complement!  ;
' éstas aliases: Éstas ésas Ésas ;aliases
: aquél  last-but-one-complement >masculine >singular @ complement!  ;
' aquél aliases: aquÉl ;aliases
: aquélla  last-but-one-complement >feminine >singular @ complement!  ;
' aquélla aliases: aquÉlla ;aliases
: aquéllos  last-but-one-complement >masculine >plural @ complement!  ;
' aquéllos aliases: aquÉllos ;aliases
: aquéllas  last-but-one-complement >feminine >plural @ complement!  ;
' aquéllas aliases: aquÉllas ;aliases

\ ----------------------------------------------
\ Verbos

: ir ['] do-go action!  ;
' ir aliases:
  dirigirme diríjame dirÍjame diríjome dirÍjome
  dirigirse dirigíos dirigÍos diríjase dirÍjase
  dirigirte diríjote dirÍjote dirígete dirÍgete
  irme voyme váyame vÁyame
  irse váyase vÁyase
  irte vete
  moverme muévame muÉvame muévome muÉvome
  moverse muévase muÉvase moveos
  moverte muévete muÉvete
  ve id idos voy vaya
  marchar marcha marchad marcho marche
  ;aliases

: abrir  ['] do-open action!  ;
' abrir aliases:  abre abrid abro abra  ;aliases
: abrirlo  abrir éste  ;
' abrirlo aliases: ábrelo Ábrelo abridlo ábrolo Ábrolo ábralo Ábralo ;aliases
: abrirla  abrir ésta  ;
' abrirla aliases: ábrela Ábrela abridla ábrola Ábrola ábrala Ábrala ;aliases
: abrirlos  abrir éstos  ;
' abrirlos aliases: ábrelos Ábrelos abridlos ábrolos Ábrolos ábralos Ábralos ;aliases
: abrirlas  abrir éstas  ;
' abrirlas aliases: ábrelas Ábrelas abridlas ábrolas Ábrolas ábralas Ábralas ;aliases

: cerrar  ['] do-close action!  ;
' cerrar aliases:  cierra cerrad cierro ;aliases
: cerrarlo  cerrar éste  ;
' cerrarlo aliases:  ciérralo ciÉrralo cerradlo ciérrolo ciÉrrolo ciérrelo ciÉrrelo ;aliases
: cerrarla  cerrar ésta  ;
' cerrarla aliases:  ciérrala ciÉrrala cerradla ciérrola ciÉrrola ciérrela ciÉrrela ;aliases
: cerrarlos  cerrar éstos  ;
' cerrarlos aliases:  ciérralos ciÉrralos cerradlos ciérrolos ciÉrrolos ciérrelos ciÉrrelos ;aliases
: cerrarlas  cerrar éstas  ;
' cerrarlas aliases:  ciérralas ciÉrralas cerradlas ciérrolas ciÉrrolas ciérrelas ciÉrrelas ;aliases

: coger  ['] do-take action!  ;
' coger aliases:
  coge coged cojo coja
  agarrar agarra agarrad agarro agarre
  recoger recoge recoged recojo recoja
  ;aliases
: cogerlo  coger éste  ;
' cogerlo aliases:
  cógelo cÓgelo cogedlo cójolo cÓjolo cójalo cÓjalo
  agarrarlo agárralo agÁrralo agarradlo agárrolo agÁrrolo agárrelo agÁrrelo
  recogerlo recógelo recÓgelo recogedlo recójolo recÓjolo recójalo recÓjalo
  ;aliases
: cogerla  coger éste  ;
' cogerla aliases:
  cógela cÓgela cogedla cójola cÓjola cójala cÓjala
  agarrarla agárrala agÁrrala agarradla agárrola agÁrrola agárrela agÁrrela
  recogerla recógela recÓgela recogedla recójola recÓjola recójala recÓjala
  ;aliases
: cogerlos  coger éstos  ;
' cogerlos aliases:
  cógelos cÓgelos cogedlos cójolos cÓjolos cójalos cÓjalos
  agarrarlos agárralos agÁrralos agarradlos agárrolos agÁrrolos agárrelos agÁrrelos
  recogerlos recógelos recÓgelos recogedlos recójolos recÓjolos recójalos recÓjalos
  ;aliases
: cogerlas  coger éstas  ;
' cogerlas aliases:
  cógelas cÓgelas cogedlas cójolas cÓjolas cójalas cÓjalas
  agarrarlas agárralas agÁrralas agarradlas agárrolas agÁrrolas agárrelas agÁrrelas
  recogerlas recógelas recÓgelas recogedlas recójolas recÓjolas recójalas recÓjalas
  ;aliases

: tomar  ['] do-take|do-eat action!  ; \ XXX TODO -- unfinished
' tomar  aliases:
  toma tomad tomo tome
  ;aliases
: tomarlo  tomar éste  ;
' tomarlo aliases: tómalo tÓmalo tomadlo tómolo tÓmolo tómelo tÓmelo ;aliases

: dejar  ['] do-drop action!  ;
' dejar aliases:
  deja dejad dejo deje
  soltar suelta soltad suelto suelte
  tirar tira tirad tiro tire
  ;aliases
: dejarlo  dejar éste  ;
' dejarlo aliases:
  déjalo dÉjalo dejadlo déjolo dÉjolo déjelo dÉjelo
  soltarlo suéltalo suÉltalo soltadlo suéltolo suÉltolo suéltelo suÉltelo
  tirarlo tíralo tÍralo tiradlo tírolo tÍrolo tírelo tÍrelo
  ;aliases
: dejarlos  dejar éstos  ;
' dejarlos aliases:
  déjalos dÉjalos dejadlos déjolos dÉjolos déjelos dÉjelos
  soltarlos suéltalos suÉltalos soltadlos suéltolos suÉltolos suéltelos suÉltelos
  tirarlos tíralos tÍralos tiradlos tírolos tÍrolos tírelos tÍrelos
  ;aliases
: dejarla  dejar ésta  ;
' dejarla aliases:
  déjala dÉjala dejadla déjola dÉjola déjela dÉjela
  soltarla suéltala suÉltala soltadla suéltola suÉltola suéltela suÉltela
  tirarla tírala tÍrala tiradla tírola tÍrola tírela tÍrela
  ;aliases
: dejarlas  dejar éstas  ;
' dejarlas aliases:
  déjalas dÉjalas dejadlas déjolas dÉjolas déjelas dÉjelas
  soltarlas suéltalas suÉltalas soltadlas suéltolas suÉltolas suéltelas suÉltelas
  tirarlas tíralas tÍralas tiradlas tírolas tÍrolas tírelas tÍrelas
  ;aliases

: mirar  ['] do-look action!  ;
' mirar aliases:
  m mira mirad miro mire
  contemplar contempla contemplad contemplo contemple
  observar observa observad observo observe
  ;aliases
: mirarlo  mirar éste  ;
' mirarlo aliases:
  míralo mÍralo miradlo mírolo mÍrolo mírelo mÍrelo
  contemplarlo contémplalo contÉmplalo contempladlo contémplolo contÉmplolo contémplelo contÉmplelo
  observarlo obsérvalo obsÉrvalo observadlo obsérvolo obsÉrvolo obsérvelo obsÉrvelo
  ;aliases
: mirarla  mirar ésta  ;
' mirarla aliases:
  mírala mÍrala miradla mírola mÍrola mírela mÍrela
  contemplarla contémplala contÉmplala contempladla contémplola contÉmplola contémplela contÉmplela
  observarla obsérvala obsÉrvala observadla obsérvola obsÉrvola obsérvela obsÉrvela
  ;aliases
: mirarlos  mirar éstos  ;
' mirarlos aliases:
  míralos mÍralos miradlos mírolos mÍrolos mírelos mÍrelos
  contemplarlos contémplalos contÉmplalos contempladlos contémplolos contÉmplolos contémplelos contÉmplelos
  observarlos obsérvalos obsÉrvalos observadlos obsérvolos obsÉrvolos obsérvelos obsÉrvelos
  ;aliases
: mirarlas  mirar éstas  ;
' mirarlas aliases:
  míralas mÍralas miradlas mírolas mÍrolas mírelas mÍrelas
  contemplarlas contémplalas contÉmplalas contempladlas contémplolas contÉmplolas contémplelas contÉmplelas
  observarlas obsérvalas obsÉrvalas observadlas obsérvolas obsÉrvolas obsérvelas obsÉrvelas
  ;aliases

: mirarse  ['] do-look-yourself action!  ;
' mirarse aliases:
  mírese mÍrese miraos
  mirarte mírate mÍrate mírote mÍrote mírete mÍrete
  mirarme mírame mÍrame miradme mírome mÍrome míreme mÍreme
  contemplarse contemplaos contémplese contÉmplese
  contemplarte contémplate contÉmplate contémplote contÉmplote contémplete contÉmplete
  contemplarme contémplame contÉmplame contempladme contémplome contÉmplome contémpleme contÉmpleme
  observarse obsérvese obsÉrvese observaos
  observarte obsérvate obsÉrvate obsérvote obsÉrvote obsérvete obsÉrvete
  observarme obsérvame obsÉrvame observadme obsérvome obsÉrvome obsérveme obsÉrveme
  ;aliases

: otear  ['] do-look-to-direction action!  ;
' otear aliases: oteo otea otead otee ;aliases

: x  ['] do-exits action!  ;
: salida  ['] do-exits (exits) action|complement!  ;
' salida aliases:  salidas  ;aliases

: examinar  ['] do-examine action!  ;
' examinar aliases: ex examina examinad examino examine  ;aliases
: examinarlo  examinar éste  ;
' examinarlo aliases: examínalo examÍnalo examinadlo examínolo examÍnolo examínelo examÍnelo  ;aliases
: examinarlos  examinar éstos  ;
' examinarlos aliases: examínalos examÍnalos examinadlos examínolos examÍnolos examínelos examÍnelos  ;aliases
: examinarla  examinar ésta  ;
' examinarla aliases: examínala examÍnala examinadla examínola examÍnola examínela examÍnela  ;aliases
: examinarlas  examinar éstas  ;
' examinarlas aliases: examínalas examÍnalas examinadlas examínolas examÍnolas examínelas examÍnelas  ;aliases

: examinarse  ['] do-examine action! protagonist% complement!  ;
' examinarse aliases:
  examínese examÍnese examinaos
  examinarte examínate examÍnate examínete examÍnete
  examinarme examíname examÍname examinadme examínome examÍnome examíneme examÍneme
  ;aliases

: registrar  ['] do-search action!  ;
' registrar aliases:  registra registrad registro registre  ;aliases
: registrarlo  registrar éste  ;
' registrarlo aliases: regístralo regÍstralo registradlo regístrolo regÍstrolo regístrelo regÍstrelo  ;aliases
: registrarla  registrar ésta  ;
' registrarla aliases: regístrala regÍstrala registradla regístrola regÍstrola regístrela regÍstrela  ;aliases
: registrarlos  registrar éstos  ;
' registrarlos aliases: regístralos regÍstralos registradlos regístrolos regÍstrolos regístrelos regÍstrelos  ;aliases
: registrarlas  registrar éstas  ;
' registrarlas aliases: regístralas regÍstralas registradlas regístrolas regÍstrolas regístrelas regÍstrelas  ;aliases

: i  ['] do-inventory inventory% action|complement!  ;
' i aliases:  inventario  ;aliases
: inventariar  ['] do-inventory action!  ;
' inventariar aliases:
  inventaría inventarÍa inventariad inventarío inventarÍo inventaríe inventarÍe
  registrarse regístrase regÍstrase regístrese regÍstrese
  registrarme regístrame regÍstrame registradme regístrome regÍstrome regístreme regÍstreme
  registrarte regístrate regÍstrate regístrote regÍstrote regístrete regÍstrete
  ;aliases

: hacer  ['] do-do action!  ;
' hacer aliases:  haz haced hago haga  ;aliases
: hacerlo  hacer éste  ;
' hacerlo aliases:  hazlo hacedlo hágolo hÁgolo hágalo hÁgalo  ;aliases
: hacerla  hacer ésta  ;
' hacerla aliases:  hazla hacedla hágola hÁgola hágala hÁgala  ;aliases
: hacerlos  hacer éstos  ;
' hacerlos aliases:  hazlos hacedlos hágolos hÁgolos hágalos hÁgalos  ;aliases
: hacerlas  hacer éstas  ;
' hacerlas aliases:  hazlas hacedlas hágolas hÁgolas hágalas hÁgalas  ;aliases

: fabricar  ['] do-make action!  ;
' fabricar aliases:
  fabrica fabricad fabrico fabrique
  construir construid construye construyo construya
  ;aliases
: fabricarlo  fabricar éste  ;
' fabricarlo aliases:
  fabrícalo fabrÍcalo fabricadlo fabrícolo fabrÍcolo fabríquelo fabrÍquelo
  construirlo constrúyelo constrÚyelo construidlo constrúyolo constrÚyolo constrúyalo constrÚyalo
  ;aliases
: fabricarla  fabricar éste  ;
' fabricarla aliases:
  fabrícala fabrÍcala fabricadla fabrícola fabrÍcola fabríquela fabrÍquela
  construirla constrúyela constrÚyela construidla constrúyola constrÚyola constrúyala constrÚyala
  ;aliases
: fabricarlos  fabricar éste  ;
' fabricarlos aliases:
  fabrícalos fabrÍcalos fabricadlos fabrícolos fabrÍcolos fabríquelos fabrÍquelos
  construirlos constrúyelos constrÚyelos construidlos constrúyolos constrÚyolos constrúyalos constrÚyalos
  ;aliases
: fabricarlas  fabricar éste  ;
' fabricarlas aliases:
  fabrícalas fabrÍcalas fabricadlas fabrícolas fabrÍcolas fabríquelas fabrÍquelas
  construirlas constrúyelas constrÚyelas construidlas constrúyolas constrÚyolas constrúyalas constrÚyalas
  ;aliases

: nadar  ['] do-swim action!  ;
' nadar aliases:
  nada nado nade
  bucear bucea bucead buceo bucee
  sumergirse sumérgese sumÉrgese sumérjase sumÉrjase
  sumergirme sumérgeme sumÉrgeme sumérjome sumÉrjome sumérjame sumÉrjame
  sumergirte sumérgete sumÉrgete sumergíos sumergÍos sumérjote sumÉrjote sumérjate sumÉrjate
  zambullirse zambullíos zambullÍos zambúllese zambÚllese zambúllase zambÚllase
  zambullirme zambúlleme zambÚlleme zambúllome zambÚllome zambúllame zambÚllame
  zambullirte zambúllete zambÚllete zambúllote zambÚllote zambúllate zambÚllate
  bañarse baÑarse báñase bÁñase báÑase bÁÑase báñese bÁñese báÑese bÁÑese
  bañarme baÑarme báñame bÁñame báÑame bÁÑame báñome bÁñome báÑome bÁÑome báñeme bÁñeme báÑeme bÁÑeme
  bañarte báñate bÁñate báÑate bÁÑate bañaos baÑaos báñote bÁñote báÑote bÁÑote báñete bÁñete báÑete bÁÑete
  ;aliases

: quitarse  ['] do-take-off action!  ;
' quitarse aliases:
  quítase quÍtase quitaos quítese quÍtese
  quitarte quítate quÍtate quítote quÍtote quítete quÍtete
  quitarme quítame quÍtame quítome quÍtome quíteme quÍteme
  ;aliases
: quitárselo  quitarse éste  ;
' quitárselo aliases:
  quitÁrselo
  quitártelo quitÁrtelo quitáoslo quitÁoslo quíteselo quÍteselo
  quitármelo quitÁrmelo quítamelo quÍtamelo quítomelo quÍtomelo quítemelo quÍtemelo
  ;aliases
: quitársela  quitarse ésta  ;
' quitársela aliases:
  quitÁrsela
  quitártela quitÁrtela quitáosla quitÁosla quítesela quÍtesela
  quitármela quitÁrmela quítamela quÍtamela quítomela quÍtomela quítemela quÍtemela
  ;aliases
: quitárselos  quitarse éstos  ;
' quitárselos aliases:
  quitÁrselos
  quitártelos quitÁrtelos quitáoslos quitÁoslos quíteselos quÍteselos
  quitármelos quitÁrmelos quítamelos quÍtamelos quítomelos quÍtomelos quítemelos quÍtemelos
  ;aliases
: quitárselas  quitarse éstas  ;
' quitárselas aliases:
  quitÁrselas
  quitártelas quitÁrtelas quitáoslas quitÁoslas quíteselas quÍteselas
  quitármelas quitÁrmelas quítamelas quÍtamelas quítomelas quÍtomelas quítemelas quÍtemelas
  ;aliases

: ponerse  ['] do-put-on action!  ;
' ponerse aliases:
  póngase pÓngase poneos
  ponerme ponme póngome pÓngome póngame pÓngame
  ponerte ponte póngote pÓngote póngate pÓngate
  colocarse colocaos colóquese colÓquese
  colocarte colócate colÓcate colóquete colÓquete
  colocarme colócame colÓcame colócome colÓcome colóqueme colÓqueme
  ;aliases
\ XXX TODO -- crear acción. vestir [con], parte como sinónimo y parte independiente
: ponérselo  ponerse éste  ;
' ponérselo aliases:
  ponÉrselo
  póngaselo pÓngaselo ponéoslo ponÉoslo
  ponérmelo ponÉrmelo pónmelo pÓnmelo póngomelo pÓngomelo póngamelo pÓngamelo
  ponértelo ponÉrtelo póntelo pÓntelo póngotelo pÓngotelo póngatelo pÓngatelo
  colocórselo colocÓrselo colocáoslo colocÁoslo colóqueselo colÓqueselo
  colocártelo colocÁrtelo colócatelo colÓcatelo colóquetelo colÓquetelo
  colocármelo colocÁrmelo colócamelo colÓcamelo colócomelo colÓcomelo colóquemelo colÓquemelo
  ;aliases
: ponérsela  ponerse ésta  ;
' ponérsela aliases:
  ponÉrsela
  póngasela pÓngasela ponéosla ponÉosla
  ponérmela ponÉrmela pónmela pÓnmela póngomela pÓngomela póngamela pÓngamela
  ponértela ponÉrtela póntela pÓntela póngotela pÓngotela póngatela pÓngatela
  colocórsela colocÓrsela colocáosla colocÁosla colóquesela colÓquesela
  colocártela colocÁrtela colócatela colÓcatela colóquetela colÓquetela
  colocármela colocÁrmela colócamela colÓcamela colócomela colÓcomela colóquemela colÓquemela
  ;aliases
: ponérselos  ponerse éstos  ;
' ponérselos aliases:
  ponÉrselos
  póngaselos pÓngaselos ponéoslos ponÉoslos
  ponérmelos ponÉrmelos pónmelos pÓnmelos póngomelos pÓngomelos póngamelos pÓngamelos
  ponértelos ponÉrtelos póntelos pÓntelos póngotelos pÓngotelos póngatelos pÓngatelos
  colocórselos colocÓrselos colocáoslos colocÁoslos colóqueselos colÓqueselos
  colocártelos colocÁrtelos colócatelos colÓcatelos colóquetelos colÓquetelos
  colocármelos colocÁrmelos colócamelos colÓcamelos colócomelos colÓcomelos colóquemelos colÓquemelos
  ;aliases
: ponérselas  ponerse éstas  ;
' ponérselas aliases:
  ponÉrselas
  póngaselas pÓngaselas ponéoslas ponÉoslas
  ponérmelas ponÉrmelas pónmelas pÓnmelas póngomelas pÓngomelas póngamelas pÓngamelas
  ponértelas ponÉrtelas póntelas pÓntelas póngotelas pÓngotelas póngatelas pÓngatelas
  colocórselas colocÓrselas colocáoslas colocÁoslas colóqueselas colÓqueselas
  colocártelas colocÁrtelas colócatelas colÓcatelas colóquetelas colÓquetelas
  colocármelas colocÁrmelas colócamelas colÓcamelas colócomelas colÓcomelas colóquemelas colÓquemelas
  ;aliases

: matar  ['] do-kill action!  ;
' matar aliases:
  mata matad mato mate
  asesinar asesina asesinad asesino asesine
  aniquilar aniquila aniquilad aniquilo aniquile
  ;aliases
: matarlo  matar éste  ;
' matarlo aliases:
  mátalo mÁtalo matadlo mátolo mÁtolo mátelo mÁtelo
  asesinarlo asesínalo asesÍnalo asesinadlo asesínolo asesÍnolo asesínelo asesÍnelo
  aniquilarlo aniquínalo aniquÍnalo aniquinadlo aniquínolo aniquÍnolo aniquínelo aniquÍnelo
  ;aliases
: matarla  matar ésta  ;
' matarla aliases:
  mátala mÁtala matadla mátola mÁtola mátela mÁtela
  asesinarla asesínala asesÍnala asesinadla asesínola asesÍnola asesínela asesÍnela
  aniquilarla aniquínala aniquÍnala aniquinadla aniquínola aniquÍnola aniquínela aniquÍnela
  ;aliases
: matarlos  matar éstos  ;
' matarlos aliases:
  mátalos mÁtalos matadlos mátolos mÁtolos mátelos mÁtelos
  asesinarlos asesínalos asesÍnalos asesinadlos asesínolos asesÍnolos asesínelos asesÍnelos
  aniquilarlos aniquínalos aniquÍnalos aniquinadlos aniquínolos aniquÍnolos aniquínelos aniquÍnelos
  ;aliases
: matarlas  matar éstas  ;
' matarlas aliases:
  mátalas mÁtalas matadlas mátolas mÁtolas mátelas mÁtelas
  asesinarlas asesínalas asesÍnalas asesinadlas asesínolas asesÍnolas asesínelas asesÍnelas
  aniquilarlas aniquínalas aniquÍnalas aniquinadlas aniquínolas aniquÍnolas aniquínelas aniquÍnelas
  ;aliases

: golpear  ['] do-hit action!  ;
' golpear aliases:
  golpea golpead golpeo golpee
  sacudir sacude sacudid sacudo sacuda
  ;aliases
: golpearla  golpear ésta  ;
' golpearla aliases:
  golpéala golpÉala golpeadla golpéola golpÉola golpéela golpÉela
  sacudirla sacúdela sacÚdela sacudidla sacúdola sacÚdola sacúdala sacÚdala
  ;aliases
: golpearlos  golpear éstos  ;
' golpearlos aliases:
  golpéalos golpÉalos golpeadlos golpéolos golpÉolos golpéelos golpÉelos
  sacudirlos sacúdelos sacÚdelos sacudidlos sacúdolos sacÚdolos sacúdalos sacÚdalos
  ;aliases
: golpearlas  golpear éstas  ;
' golpearlas aliases:
  golpéalas golpÉalas golpeadlas golpéolas golpÉolas golpéelas golpÉelas
  sacudirlas sacúdelas sacÚdelas sacudidlas sacúdolas sacÚdolas sacúdalas sacÚdalas
  ;aliases

: atacar  ['] do-attack action!  ;
' atacar aliases:
  ataca atacad ataco ataque
  agredir agrede agredid agredo agreda
  ;aliases
: atacarlo  atacar éste  ;
' atacarlo aliases:
  atácalo atÁcalo atacadlo atácolo atÁcolo atáquelo atÁquelo
  agredirlo agrédelo agrÉdelo agredidlo agrédolo agrÉdolo agrédalo agrÉdalo
  ;aliases
: atacarla  atacar ésta  ;
' atacarla aliases:
  atácala atÁcala atacadla atácola atÁcola atáquela atÁquela
  agredirla agrédela agrÉdela agredidla agrédola agrÉdola agrédala agrÉdala
  ;aliases
: atacarlos  atacar éstos  ;
' atacarlos aliases:
  atácalos atÁcalos atacadlos atácolos atÁcolos atáquelos atÁquelos
  agredirlos agrédelos agrÉdelos agredidlos agrédolos agrÉdolos agrédalos agrÉdalos
  ;aliases
: atacarlas  atacar éstas  ;
' atacarlas aliases:
  atácalas atÁcalas atacadlas atácolas atÁcolas atáquelas atÁquelas
  agredirlas agrédelas agrÉdelas agredidlas agrédolas agrÉdolas agrédalas agrÉdalas
  ;aliases

: romper  ['] do-break action!  ;
' romper aliases:
  rompe romped rompo rompa
  despedazar despedaza despedazad despedazo despedace
  destrozar destroza destrozad destrozo destroce
  dividir divide dividid divido divida
  cortar corta cortad corto corte
  ;aliases
: romperlo  romper éste  ;
' romperlo aliases:
  rómpelo rÓmpelo rompedlo rómpolo rÓmpolo rómpalo rÓmpalo
  despedazarlo despedazalo despedazadlo despedázolo despedÁzolo despedácelo despedÁcelo
  destrozarlo destrázalo destrÁzalo destrozadlo destrózolo destrÓzolo destrócelo destrÓcelo
  dividirlo divídelo divÍdelo divididlo divídolo divÍdolo divídalo divÍdalo
  cortarlo cortalo cortadlo córtolo cÓrtolo córtelo cÓrtelo
  ;aliases
: romperla  romper ésta  ;
' romperla aliases:
  rómpela rÓmpela rompedla rómpola rÓmpola rómpala rÓmpala
  despedazarla despedazala despedazadla despedázola despedÁzola despedácela despedÁcela
  destrozarla destrázala destrÁzala destrozadla destrózola destrÓzola destrócela destrÓcela
  dividirla divídela divÍdela divididla divídola divÍdola divídala divÍdala
  cortarla córtala cÓrtala cortadla córtola cÓrtola córtela cÓrtela
  ;aliases
: romperlos  romper éstos  ;
' romperlos aliases:
  rómpelos rÓmpelos rompedlos rómpolos rÓmpolos rómpalos rÓmpalos
  despedazarlos despedazalos despedazadlos despedázolos despedÁzolos despedácelos despedÁcelos
  destrozarlos destrázalos destrÁzalos destrozadlos destrózolos destrÓzolos destrócelos destrÓcelos
  dividirlos divídelos divÍdelos divididlos divídolos divÍdolos divídalos divÍdalos
  cortarlos córtalos cÓrtalos cortadlos córtolos cÓrtolos córtelos cÓrtelos
  ;aliases
: romperlas  romper éstas  ;
' romperlas aliases:
  rómpelas rÓmpelas rompedlas rómpolas rÓmpolas rómpalas rÓmpalas
  despedazarlas despedazalas despedazadlas despedázolas despedÁzolas despedácelas despedÁcelas
  destrozarlas destrázalas destrÁzalas destrozadlas destrózolas destrÓzolas destrócelas destrÓcelas
  dividirlas divídelas divÍdelas divididlas divídolas divÍdolas divídalas divÍdalas
  cortarlas córtalas cÓrtalas cortadlas córtolas cÓrtolas córtelas cÓrtelas
  ;aliases

\ quebrar \ XXX TODO
\ desgarrar \ XXX TODO

: asustar  ['] do-frighten action!  ;
' asustar aliases:
  asusto asusta asustad asuste
  amedrentar amedrento amedrenta amedrentad amedrente
  acojonar acojono acojona acojonad acojone
  atemorizar atemoriza atemorizad atemorizo atemorice
  ;aliases
: asustarlo  asustar éste  ;
' asustarlo aliases:
  asústolo asÚstolo asústalo asÚstalo asustadlo asústelo asÚstelo
  amedrentarlo amedréntolo amedrÉntolo amedréntalo amedrÉntalo amedrentadlo amedréntelo amedrÉntelo
  acojonarlo acojónolo acojÓnolo acojónalo acojÓnalo acojonadlo acojónelo acojÓnelo
  atemorizarlo atemorízalo atemorÍzalo atemorizadlo atemorízolo atemorÍzolo atemorícelo atemorÍcelo
  ;aliases
: asustarla  asustar ésta  ;
' asustarla aliases:
  asústola asÚstola asústala asÚstala asustadla asústela asÚstela
  amedrentarla amedréntola amedrÉntola amedréntala amedrÉntala amedrentadla amedréntela amedrÉntela
  acojonarla acojónola acojÓnola acojónala acojÓnala acojonadla acojónela acojÓnela
  atemorizarla atemorízala atemorÍzala atemorizadla atemorízola atemorÍzola atemorícela atemorÍcela
  ;aliases
: asustarlos  asustar éstos  ;
' asustarlos aliases:
  asústolos asÚstolos asústalos asÚstalos asustadlos asústelos asÚstelos
  amedrentarlos amedréntolos amedrÉntolos amedréntalos amedrÉntalos amedrentadlos amedréntelos amedrÉntelos
  acojonarlos acojónolos acojÓnolos acojónalos acojÓnalos acojonadlos acojónelos acojÓnelos
  atemorizarlos atemorízalos atemorÍzalos atemorizadlos atemorízolos atemorÍzolos atemorícelos atemorÍcelos
  ;aliases
: asustarlas  asustar éstas  ;
' asustarlas aliases:
  asústolas asÚstolas asústalas asÚstalas asustadlas asústelas asÚstelas
  amedrentarlas amedréntolas amedrÉntolas amedréntalas amedrÉntalas amedrentadlas amedréntelas amedrÉntelas
  acojonarlas acojónolas acojÓnolas acojónalas acojÓnalas acojonadlas acojónelas acojÓnelas
  atemorizarlas atemorízalas atemorÍzalas atemorizadlas atemorízolas atemorÍzolas atemorícelas atemorÍcelas
  ;aliases

: afilar  ['] do-sharpen action!  ;
' afilar aliases:  afila afilad afilo afile  ;aliases
: afilarlo  afilar éste  ;
' afilarlo aliases:  afílalo afÍlalo afiladlo afílolo afÍlolo afílelo afÍlelo  ;aliases
: afilarla  afilar ésta  ;
' afilarla aliases:  afílala afÍlala afiladla afílola afÍlola afílela afÍlela  ;aliases
: afilarlos  afilar éstos  ;
' afilarlos aliases:  afílalos afÍlalos afiladlos afílolos afÍlolos afílelos afÍlelos  ;aliases
: afilarlas  afilar éstas  ;
' afilarlas aliases:  afílalas afÍlalas afiladlas afílolas afÍlolas afílelas afÍlelas  ;aliases

: partir  ['] do-go|do-break action!  ;
' partir aliases:  parto partid parta  ;aliases
\ «parte» está en la sección final de ambigüedades
: partirlo  partir éste  ;
' partirlo aliases:  pártelo pÁrtelo pártolo pÁrtolo partidlo pártalo pÁrtalo  ;aliases
: partirla  partir ésta  ;
' partirla aliases:  pártela pÁrtela pártola pÁrtola partidla pártala pÁrtala  ;aliases
: partirlos  partir éstos  ;
' partirlos aliases:  pártelos pÁrtelos pártolos pÁrtolos partidlos pártalos pÁrtalos  ;aliases
: partirlas  partir éstas  ;
' partirlas aliases:  pártelas pÁrtelas pártolas pÁrtolas partidlas pártalas pÁrtalas  ;aliases

: esperar  ;  \ XXX TODO

' esperar aliases:
  z espera esperad espero espere
  aguardar aguarda aguardad aguardo aguarde
  ;aliases
: esperarlo  esperar éste  ;
' esperarlo aliases:
  esperadlo espérolo espÉrolo espérelo espÉrelo
  aguardarlo aguárdalo aguÁrdalo aguardadlo aguárdolo aguÁrdolo aguárdelo aguÁrdelo
  ;aliases
: esperarla  esperar ésta  ;
' esperarla aliases:
  esperadla espérola espÉrola espérela espÉrela
  aguardarla aguárdala aguÁrdala aguardadla aguárdola aguÁrdola aguárdela aguÁrdela
  ;aliases
: esperarlos  esperar éstos  ;
' esperarlos aliases:
  esperadlos espérolos espÉrolos espérelos espÉrelos
  aguardarlos aguárdalos aguÁrdalos aguardadlos aguárdolos aguÁrdolos aguárdelos aguÁrdelos
  ;aliases
: esperarlas  esperar éstas  ;
' esperarlas aliases:
  esperadlas espérolas espÉrolas espérelas espÉrelas
  aguardarlas aguárdalas aguÁrdalas aguardadlas aguárdolas aguÁrdolas aguárdelas aguÁrdelas
  ;aliases

\ XXX TODO:
\ meter introducir insertar colar encerrar

\ ----------------------------------------------

: ulfius  ulfius% complement!  ;
: ambrosio  (ambrosio) complement!  ;
: hombre  (man) complement!  ;
' hombre aliases:  señor seÑor tipo individuo persona  ;aliases
: hombres  (men) complement!  ;
' hombres aliases: gente personas ;aliases
\ XXX Ambigüedad.:
\ «jefe» podría ser también el jefe de los enemigos durante la batalla:
: jefe  leader% complement!  ;
' jefe aliases:
  líder lÍder viejo anciano abuelo
  ;aliases
: soldados  soldiers% complement!  ;
' soldados aliases:
  guerreros luchadores combatientes camaradas
  compañeros compaÑeros oficiales suboficiales militares
  guerrero luchador combatiente camarada
  compañero compaÑero oficial suboficial militar
  ;aliases
: multitud  refugees% complement!  ;
' multitud aliases:
  niño niÑo niños niÑos niña niÑa niñas niÑas
  muchacho muchachos muchacha muchachas
  adolescente adolescentes
  ancianos anciana ancianas mayores viejos vieja viejas
  joven jóvenes jÓvenes
  abuela abuelos abuelas
  nieto nietos nieta nietas
  padre padres madre madres mamá mamÁ mamás mamÁs papás papÁs
  bebé bebÉ bebés bebÉs beba bebas bebito bebitos bebita bebitas
  pobres desgraciados desafortunados
  desgraciadas desafortunadas
  muchedumbre masa enjambre
  ;aliases
: refugiados leader% conversations? ?? multitud ;
' refugiados aliases: refugiada refugiadas ;aliases
: refugiado leader% conversations? ?? viejo ;

: altar  altar% complement!  ;
: arco  arch% complement!  ;
: capa  cloak% complement!  ; \ XXX TODO -- hijuelo?
' capa aliases:  lana  ;aliases
\ ' capa aliases:  abrigo  ;aliases \ XXX TODO -- diferente género
: coraza  cuirasse% complement!  ;
' coraza aliases:  armadura  ;aliases
: puerta  door% complement!  ;
: esmeralda  emerald% complement!  ;
' esmeralda aliases:  joya  ;aliases
\ XXX TODO -- piedra-preciosa brillante
: derrumbe fallen-away% complement!  ;
: banderas  flags% complement!  ;
' banderas aliases:
    bandera pendones enseñas enseÑas pendón pendÓn enseña enseÑa
    mástil mÁstil mástiles mÁstiles
    estandarte estandartes
  ;aliases \ XXX TODO -- estandarte, enseña... otro género
: dragones  flags% is-known? ?? banderas ;
' dragones aliases: dragón dragÓn ;aliases
: pedernal  flint% complement!  ;
: ídolo  idol% complement!  ;
' ídolo aliases:  Ídolo ojo orificio agujero  ;aliases
\ XXX TODO -- separar los sinónimos de ídolo
: llave  key% complement!  ;
: lago  lake% complement!  ;
' lago aliases:  laguna agua estanque  ;aliases  \ XXX TODO -- diferente género
: candado  lock% complement!  ;
' candado aliases:  cerrojo  ;aliases
: tronco  log% complement!  ;
' tronco aliases:  leño leÑo madero  ;aliases
\ XXX TODO -- madera
: trozo  piece% complement!  ;
' trozo aliases:  pedazo retal tela  ;aliases
: harapo  rags% complement!  ;
: rocas  ( -- )
  \ Este término puede referise a las rocas o al derrumbe.
  location-09% am-i-there?
  if  fallen-away%  else  rocks%  then  complement!  ;

' rocas aliases:  piedras pedruscos  ;aliases
: piedra  (stone) complement!  ;
' piedra aliases:  roca pedrusco  ;aliases
: serpiente  snake% complement!  ;
' serpiente aliases:  reptil ofidio culebra animal bicho  ;aliases
: espada  sword% complement!  ;
' espada aliases:  tizona arma  ;aliases
\ XXX Nota.: "arma" es femenina pero usa artículo "él", contemplar en los cálculos de artículo.
: hilo  thread% complement!  ;
' hilo aliases:  hebra  ;aliases
: antorcha  torch% complement!  ;
: cascada  waterfall% complement!  ;
' cascada aliases:  catarata  ;aliases
: catre  s" catre" bed% ms-name! bed% complement!  ;
' catre aliases:  camastro  ;aliases
: cama s" cama" bed% fs-name! bed% complement!  ;
: velas  candles% complement!  ;
' velas aliases:  vela  ;aliases
: mesa  table% complement!  ;
' mesa aliases:  mesita pupitre  ;aliases
: puente  (bridge) complement!  ;
: alguien  (somebody) complement!  ;
: hierba  s" hierba" grass% fs-name! grass% complement!  ;
: hierbas  s" hierbas" grass% fp-name! grass% complement!  ;
: hierbajo  s" hierbajo" grass% ms-name! grass% complement!  ;
: hierbajos  s" hierbajos" grass% mp-name! grass% complement!  ;
: hiedra  s" hiedra" grass% fs-name! grass% complement!  ;
: hiedras  s" hiedras" grass% fp-name! grass% complement!  ;

\ ----------------------------------------------

: n  ['] do-go-north north% action|complement!  ;
' n aliases:  norte septentrión septentriÓn  ;aliases

: s  ['] do-go-south south% action|complement!  ;
' s aliases:  sur meridión meridiÓn  ;aliases

: e  ['] do-go-east east% action|complement!  ;
' e aliases:  este oriente levante  ;aliases

: o  ['] do-go-west west% action|complement!  ;
' o aliases:  oeste occidente poniente  ;aliases

: ar  ['] do-go-up up% action|complement!  ;
' ar aliases:  arriba  ;aliases
: subir  ['] do-go-up action!  ;
' subir aliases:  sube subid subo suba  ;aliases
' subir aliases:  ascender asciende ascended asciendo ascienda  ;aliases
' subir aliases:  subirse subíos subÍos súbese sÚbese súbase sÚbase  ;aliases
' subir aliases:  subirte súbete sÚbete súbote sÚbote súbate sÚbate  ;aliases

: ab  ['] do-go-down down% action|complement!  ;
' ab aliases:  abajo  ;aliases
: bajar  ['] do-go-down action!  ;
' bajar aliases:  baja bajad bajo baje  ;aliases
' bajar aliases:  bajarse bajaos bájase bÁjase bájese bÁjese  ;aliases
' bajar aliases:  bajarte bájate bÁjate bájote bÁjote bájete bÁjete  ;aliases
' bajar aliases:  descender desciende descended desciendo descienda  ;aliases

: salir  ['] do-go-out action!  ;
' salir aliases:  sal salid salgo salga  ;aliases
\ XXX TODO -- ambigüedad. sal
' salir aliases:  salirse ;aliases
' salir aliases:  salirme sálgome sÁlgome  ;aliases
' salir aliases:  salirte ;aliases
\ XXX TODO -- ambigüedad. salte
: fuera  ['] do-go-out out% action|complement!  ;
' fuera aliases:  afuera ;aliases
: exterior  out% complement!  ;
: entrar ['] do-go-in action!  ;
' entrar aliases:  entra entrad entro entre  ;aliases
' entrar aliases:  entrarse entraos éntrese Éntrese éntrase Éntrase  ;aliases
' entrar aliases:  entrarte éntrete Éntrete éntrate Éntrate  ;aliases
: dentro  ['] do-go-in in% action|complement!  ;
' dentro aliases:  adentro  ;aliases
: interior  in% complement!  ;

: escalar  ['] do-climb action!  ;
' escalar aliases:  escala escalo escale  ;aliases
' escalar aliases:  trepar trepa trepo trepe  ;aliases

: hablar  ['] do-speak action!  ;
\ XXX TODO -- Crear nuevas palabras según la preposición que necesiten.
\ XXX TODO -- Separar matices.
' hablar aliases:
  habla hablad hablo hable
  hablarle háblale hÁblale háblole hÁblole háblele hÁblele
  conversar conversa conversad converso converse
  charlar charla charlad charlo charle
  decir di decid digo diga
  decirle dile decidle dígole dÍgole dígale dÍgale
  platicar platica platicad platico platique
  platicarle platícale platÍcale platicadle platícole platÍcole platíquele platÍquele
  ;aliases
  \ contar cuenta cuento cuente  \ XXX
  \ contarle cuéntale cuÉntale cuéntole cuÉntole cuéntele cuÉntele  \ XXX

: presentarse  ['] do-introduce-yourself action!  ;
' presentarse aliases:
  preséntase presÉntase preséntese presÉntese
  presentarte preséntate presÉntate presentaos preséntete presÉntete
  ;aliases

\ Términos asociados a entes globales o virtuales

: nubes  clouds% complement!  ;
\ XXX TODO ¿cúmulo-nimbos?, ¿nimbos?
' nubes aliases:  nube estratocúmulo estratocÚmulo estratocúmulos estratocÚmulos cirro cirros  ;aliases
: suelo  floor% complement!  ;
' suelo aliases:  suelos tierra firme  ;aliases
\ XXX TODO -- Añadir «piso», que es ambiguo
: cielo  sky% complement!  ;
' cielo aliases:  cielos firmamento  ;aliases
: techo  ceiling% complement!  ;
: cueva  (cave) complement!  ;
' cueva aliases:  caverna gruta  ;aliases
: entrada  (entrance) complement!  ;
\ XXX TODO ¿Implementar cambio de nombre y/o género gramatical? (entrada, acceso):
' entrada aliases:  acceso ;aliases
: enemigo  enemy% complement!  ;
' enemigo aliases: enemigos sajón sajÓn sajones ;aliases
: todo ;  \ XXX TODO
\ XXX TODO ¿Implementar cambio de nombre y/o género gramatical? (pared/es, muro/s):
: pared  (wall) complement!  ;
' pared  aliases: muro ;aliases
: paredes  wall% complement!  ;
' paredes  aliases: muros ;aliases

\ ----------------------------------------------
\ Artículos

\ Los artículos no hacen nada pero es necesario crearlos
\ para que no provoquen un error cuando el intérprete de
\ comandos funcione en el modo opcional de no ignorar las
\ palabras desconocidas.

: la  ;
' la aliases: las el los una un unas unos ;aliases

\ ----------------------------------------------
\ Adjetivos demostrativos

\ Lo mismo hacemos con los adjetivos demostrativos
\ y pronombres demostrativos sin tilde; salvo «este», que siempre
\ será interpretado como punto cardinal.

: esta  ;
' esta aliases: estas estos ;aliases

\ ----------------------------------------------
\ (Seudo)preposiciones

: con  ( -- )
  \ Uso: Herramienta o compañía
  «con»-preposition# preposition!  ;

: usando  ( -- )
  \ Uso: Herramienta
  «usando»-preposition# preposition!  ;

' usando aliases: utilizando empleando mediante ;aliases
false [if]
  \ XXX OLD
  \ XXX TODO -- descartado, pendiente
: a  ( -- )
  \ Uso: Destino de movimiento, objeto indirecto
  «a»-preposition# preposition!  ;

' a aliases: al ;aliases
: de  ( -- )
  \ Uso: Origen de movimiento, propiedad
  «de»-preposition# preposition!  ;

: hacia  ( -- )
  \ Uso: Destino de movimiento, destino de lanzamiento
  «hacia»-preposition# preposition!  ;

: contra  ( -- )
  \ Uso: Destino de lanzamiento
  «contra»-preposition# preposition!  ;

: para  ( -- )
  \ Uso: Destino de movimiento, destino de lanzamiento
  «para»-preposition# preposition!  ;

: por  ( -- )
  \ Uso: Destino de movimiento
  «por»-preposition# preposition!  ;

[then]

\ ----------------------------------------------
\ Meta

\ Términos ambiguos

: cierre  action @ if  candado  else  cerrar  then  ;
: parte  action @ if  trozo  else  partir  then  ;

\ Comandos del sistema

: #colorear  ( -- )
  \ Restaura los colores predeterminados.
  ['] recolor action!  ;

' #colorear aliases:
  #colorea #coloreo
  #recolorear #recolorea #recoloreo
  #pintar #pinta #pinto
  #limpiar #limpia #limpio
  ;aliases

: #configurar  ( -- )
  \ Restaura la configuración predeterminada
  \ y después carga el fichero de configuración
  ['] get-config action!  ;

' #configurar aliases:
  #reconfigurar
  #configura #configuro
  #reconfigura #reconfiguro
  ;aliases

: #grabar  ( "name" -- )
  \ Graba el estado de la partida en un fichero.
  [debug-parsing] [??] ~~
  parse-name >sb
  [debug-parsing] [??] ~~
  ['] save-the-game action!
  [debug-parsing] [??] ~~
  ;  immediate
' #grabar immediate-aliases:
  #graba #grabo
  #exportar #exporta #exporto
  #salvar #salva #salvo
  #guardar #guarda #guardo
  ;aliases

: #cargar  ( "name" -- )
  \ Carga el estado de la partida de un fichero.
  [debug-parsing] [??] ~~
  parse-name
  [debug-parsing] [??] ~~
  >sb
  [debug-parsing] [??] ~~
  ['] load-the-game action!
  [debug-parsing] [??] ~~
  ;  immediate
' #cargar immediate-aliases:
  #carga #cargo
  #importar #importa #importo
  #leer #lee #leo
  #recargar #recarga #recargo
  #recuperar #recupera #recupero
  #restaurar #restaura #restauro
  ;aliases

: #fin  ( -- )
  \ Abandonar la partida
  ['] finish action!  ;

' #fin aliases:
  #acabar #acaba #acabo
  #adiós #adiÓs
  #apagar #apaga #apago
  #cerrar #cierra #cierro
  #concluir #concluye #concluyo
  #finalizar #finaliza #finalizo
  #salir #sal #salgo
  #terminar #termina #termino
  ;aliases

: #ayuda  ( -- )
  \ ['] do-help action!
  ;
  \ XXX TODO

' #ayuda aliases:
  #ayudar #ayudita #ayudas
  #instrucciones #manual #guía #guÍa #mapa #plano #menú #menÚ
  #pista #pistas
  #socorro #auxilio #favor
  ;aliases

\ XXX TMP -- Comandos para usar durante el desarrollo:
\ : forth  (finish)  ;
: bye  bye  ;
: quit  quit  ;

restore-vocabularies

\ }}} ==========================================================
section( Vocabulario para entradas «sí» o «no»)  \ {{{

\ Para los casos en que el programa hace una pregunta que debe
\ ser respondida con «sí» o «no», usamos un truco análogo al
\ del vocabulario del juego: creamos un vocabulario específico
\ con palabras cuyos nombres sean las posibles respuestas:
\ «sí», «no», «s» y «n».  Estas palabras actualizarán una
\ variable,  con cuyo valor el programa sabrá si se ha
\ producido una respuesta válida o no y cuál es.
\
\ En principio, si el jugador introdujera varias respuestas
\ válidas la última sería la que tendría efecto. Por ejemplo,
\ la respuesta «sí sí sí sí sí no» sería considerada negativa.
\ Para dotar al método de una chispa de inteligencia, las
\ respuestas no cambian el valor de la variable sino que lo
\ incrementan o decrementan. Así el mayor número de respuestas
\ afirmativas o negativas decide el resultado; y si la
\ cantidad es la misma, como por ejemplo en «sí sí no no», el
\ resultado será el mismo que si no se hubiera escrito nada.

\ XXX TODO -- 2016-06-24: This module has been adapted to _La pistola
\ de agua_, and simplified a lot. The code could be reused by both
\ projects.

variable #answer  \ Su valor será 0 si no ha habido respuesta válida; negativo para «no»; y positivo para «sí»
: answer-undefined  ( -- )
  \ Inicializa la variable antes de hacer la pregunta.
  #answer off  ;

: think-it-again$  ( -- ca len )
  \ Devuelve un mensaje complementario para los errores.
  s{ s" Piénsalo mejor"
  s" Decídete" s" Cálmate" s" Concéntrate"
  s" Presta atención"
  s{ s" Prueba" s" Inténtalo" }s again$ s&
  s" No es tan difícil" }s colon+  ;

: yes-but-no$  ( -- ca len )
  \ Devuelve mensaje de error: se dijo «no» tras «sí».
  s" ¿Primero «sí»" but|and$ s&
  s" después «no»?" s& think-it-again$ s&  ;

: no-but-yes$  ( -- ca len )
  \ Devuelve mensaje de error: se dijo «sí» tras «no».
  s" ¿Primero «no»" but|and$ s&
  s" después «sí»?" s& think-it-again$ s&  ;

: yes-but-no  ( -- )
  \ Muestra error: se dijo «no» tras «sí».
  yes-but-no$ narrate  ;

' yes-but-no constant yes-but-no-error#
: no-but-yes  ( -- )
  \ Muestra error: se dijo «sí» tras «no».
  no-but-yes$ narrate  ;

' no-but-yes constant no-but-yes-error#
: two-options-only$  ( -- ca len )
  \ Devuelve un mensaje que informa de las opciones disponibles.
  ^only$ s{ s" hay" s" tienes" }s&
  s" dos" s& s" respuestas" s" posibles" rnd2swap s& s& colon+
  s" «sí»" s" «no»" both& s" (o sus iniciales)" s& period+  ;

: two-options-only  ( -- )
  \ Muestra error: sólo hay dos opciones.
  two-options-only$ narrate  ;

' two-options-only constant two-options-only-error#
: wrong-yes$  ( -- ca len )
  \ Devuelve el mensaje usado para advertir de que se ha escrito mal «sí».
  s{ s" ¿Si qué...?" s" ¿Si...?" s" ¿Cómo «si»?" s" ¿Cómo que «si»?" }s
  s" No" s& s{
  s{ s" hay" s" puedes poner" }s{ s" condiciones" s" condición alguna" }s&
  s{ s" hay" s" tienes" }s s" nada que negociar" s& }s&
  s{ s" aquí" s" en esto" s" en esta cuestión" }s& period+
  \ two-options-only$ s?&
  ;

: wrong-yes  ( -- )
  \ Muestra error: se ha usado la forma errónea «si».
  wrong-yes$ narrate  ;

' wrong-yes constant wrong-yes-error#
: answer-no  ( -- )
  \ Anota una respuesta negativa.
  #answer @ 0> yes-but-no-error# and throw  \ Provocar error si antes había habido síes
  #answer --  ;

: answer-yes  ( -- )
  \ Anota una respuesta afirmativa.
  #answer @ 0< no-but-yes-error# and throw  \ Provocar error si antes había habido noes
  #answer ++  ;

also answer-vocabulary definitions  \ Las palabras que siguen se crearán en dicho vocabulario

: sí  answer-yes  ;
: sÍ  answer-yes  ;  \ Necesario, por el problema del cambio de capitalidad y UTF-8
: s  answer-yes  ;
: no  answer-no  ;
: n  answer-no  ;
: si  wrong-yes-error# throw  ;

restore-vocabularies

\ }}} ==========================================================
section( Entrada de comandos)  \ {{{

\ Para la entrada de comandos se usa la palabra de Forth `accept`, que
\ permite limitar el número máximo de caracteres que serán aceptados.

svariable command  \ Zona de almacenamiento del comando

: command-prompt$  ( -- ca len )
  \ Devuelve el presto de entrada de comandos.
  command-prompt count  ;

: /command  ( -- u )
  \ Devuelve la longitud máxima posible para un comando.
  \ Tomar las columnas disponibles, restarles la indentación
  \ y uno más para el espacio que ocupará el cursor al final de la línea:
  cols /indentation @ - 1-
  \ Restar la longitud del presto si no lleva detrás un salto de línea:
  cr-after-command-prompt? @ 0= abs command-prompt$ nip * -
  \ Restar uno si tras el presto no va salto de línea pero sí un espacio:
  cr-after-command-prompt? @ 0= space-after-command-prompt? @ and abs -  ;

: wait-for-input  ( -- ca len )
  \ Espera y devuelve el comando introducido por el jugador.
  only player-vocabulary
  input-color
  command dup /command accept
  str+strip  \ Eliminar espacios laterales
  restore-vocabularies  ;

: .command-prompt  ( -- )
  \ Imprime un presto para la entrada de comandos.
  command-prompt$ command-prompt-color paragraph
  cr-after-command-prompt? @ if
    cr+
  else
    space-after-command-prompt?
    if  background-color space  then
  then  ;

: listen  ( -- ca len )
  \ Imprime un presto y devuelve el comando introducido por el jugador.
  [debug-info] [if]  "" debug  [then]  \ XXX INFORMER
  .command-prompt wait-for-input
  [debug] [if]  cr cr ." <<<" 2dup type ." >>>" cr cr  [then]  ;  \ XXX INFORMER

\ }}} ==========================================================
section( Entrada de respuestas de tipo «sí o no»)  \ {{{

\ XXX TODO -- 2016-06-25: This module has been adapted to _La pistola
\ de agua_. The code could be reused by both projects.

: yes|no  ( ca len -- n )
  \ Evalúa una respuesta a una pregunta del tipo «sí o no».
  \ ca len = Respuesta a evaluar
  \ n = Resultado (un número negativo para «no» y positivo para «sí»; cero si no se ha respondido ni «sí» ni «no», o si se produjo un error)
  answer-undefined
  only answer-vocabulary
  ['] evaluate-command catch
  dup if  nip nip  then  \ Reajustar la pila si ha habido error
  dup ?wrong 0=  \ Ejecutar el posible error y preparar su indicador para usarlo en el resultado
  #answer @ 0= two-options-only-error# and ?wrong  \ Ejecutar error si la respuesta fue irreconocible
  #answer @ dup 0<> and and  \ Calcular el resultado final
  restore-vocabularies  ;

: .question  ( xt -- )
  \ Imprime la pregunta.
  \ xt = Dirección de ejecución que devuelve una cadena con la pregunta
  question-color execute paragraph  ;

: answer  ( xt -- n )
  \ Devuelve la respuesta a una pregunta del tipo «sí o no».
  \ xt = Dirección de ejecución que devuelve una cadena con la pregunta
  \ n = Respuesta: un número negativo para «no» y positivo para «sí»
  begin
    dup .question listen  yes|no ?dup
  until  nip  ;

: yes?  ( xt -- f )
  \ ¿Es afirmativa la respuesta a una pregunta?
  \ xt = Dirección de ejecución que devuelve una cadena con la pregunta
  \ f = ¿Es la respuesta positiva?
  answer 0>  ;

: no?  ( xt -- f )
  \ ¿Es negativa la respuesta a una pregunta?
  \ xt = Dirección de ejecución que devuelve una cadena con la pregunta
  \ f = ¿Es la respuesta negativa?
  answer 0<  ;

\ }}} ==========================================================
section( Fin)  \ {{{

: success?  ( -- f )
  \ ¿Ha completado con éxito su misión el protagonista?
  my-location location-51% =  ;

false [if]  \ XXX TODO -- not used
: battle-phases  ( -- u )
  \ Devuelve el número máximo de fases de la batalla.
  5 random 7 +  ;  \ Número al azar, de 8 a 11

[then]
: failure?  ( -- f )
  \ ¿Ha fracasado el protagonista?
  battle# @ battle-phases >  ;

: .bye  ( -- )
  \ Mensaje final cuando el jugador no quiere jugar otra partida.
  \ XXX TMP
  s" ¡Adiós!" narrate  ;

: farewell  ( -- )
  \ Abandona el programa.
  new-page .bye bye  ;

: play-again?$  ( -- ca len )
  \ Devuelve la pregunta que se hace al jugador tras haber completado con éxito el juego.
  s{ s" ¿Quieres" s" ¿Te" s{ s" animas a" s" apetece" }s& }s
  s{ s" jugar" s" empezar" }s&  again?$ s&  ;

: retry?0$  ( -- ca len )
  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado.
  s" ¿Tienes"
  s{ s" fuerzas" s" arrestos" s" agallas" s" energías" s" ánimos" }s&  ;

: retry?1$  ( -- ca len )
  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado.
  s{ s" ¿Te quedan" s" ¿Guardas" s" ¿Conservas" }s
  s{ s" fuerzas" s" energías" s" ánimos" }s&  ;

: retry?$  ( -- ca len )
  \ Devuelve la pregunta que se hace al jugador tras haber fracasado.
  s{ retry?0$ retry?1$ }s s" para" s&
  s{ s" jugar" s" probar" s" intentarlo" }s& again?$ s&  ;

: enough?  ( -- f )
  \ ¿Prefiere el jugador no jugar otra partida?
  \ XXX TODO -- hacer que la pregunta varíe si la respuesta es incorrecta
  \ Para ello, pasar como parámetro el xt que crea la cadena.
  success? if  ['] play-again?$  else  ['] retry?$  then  cr no?  ;

: surrender?$  ( -- ca len )
  \ Devuelve la pregunta de si el jugador quiere dejar el juego.
  \ XXX TODO -- not used
  s{
  s" ¿Quieres"
  s" ¿En serio quieres"
  s" ¿De verdad quieres"
  s" ¿Estás segur" player-gender-ending$+ s" de que quieres" s&
  s" ¿Estás decidid" player-gender-ending$+ s" a" s&
  }s{
  s" dejarlo?"
  s" rendirte?"
  s" abandonar?"
  }s&  ;

: surrender?  ( -- f )
  \ ¿Quiere el jugador dejar el juego?
  ['] surrender?$
  yes?
  ~~  ;

: game-over?  ( -- f )
  \ ¿Se terminó ya el juego?
  success? failure? or  ;

: the-favorite-says$  ( -- ca len )
  s" te" s{ s" indica" s" hace saber" }s&
  s" el valido" s&  ;

: do-not-disturb$  ( -- ca len )
  s" ha" s{
  s" ordenado"
  s" dado órdenes de"
  s" dado" s" la" s?& s" orden de" s&
  }s& s" que" s& s{ s" nadie" s" no se" }s&
  s" lo moleste" s& comma+  ;

: favorite's-speech$  ( -- ca len )
  s" El rey"
  castilian-quotes? @
  if  \ Comillas castellanas
    rquote$ s+ comma+ the-favorite-says$ s& comma+
    lquote$ do-not-disturb$ s+ s&
  else  \ Raya
    dash$ the-favorite-says$ s+ dash$ s+ comma+ s&
    do-not-disturb$ s&
  then
  s" pues sufre una amarga tristeza." s&  ;

: the-happy-end  ( -- )
  \ Final del juego con éxito.
  s" Agotado, das parte en el castillo de tu llegada"
  s" y de lo que ha pasado." s&
  narrate  narration-break
  s" Pides audiencia al rey, Uther Pendragon."
  narrate  scene-break
  favorite's-speech$ speak  narration-break
  s" No puedes entenderlo. El rey, tu amigo."
  narrate  narration-break
  s" Agotado, decepcionado, apesadumbrado,"
  s" decides ir a dormir a tu casa." s&
  s" Es lo poco que puedes hacer." s&
  narrate  narration-break
  s" Te has ganado un buen descanso."
  narrate  ;

: ransom$  ( -- ca len )
  s" un" s{ s" buen" s" suculento" }s& s" rescate" s&  ;

: my-lucky-day$  ( -- ca len )
  s{  s" Hoy" s{ s" es" s" parece ser" s" sin duda es" s" al parecer es" }s&
      s{  s" Sin duda" s" No cabe duda de que"
          s" Parece que" s" Por lo que parece"
      }s s" hoy es" s&
  }s s{ s" un" s" mi" }s& s" día" s&
  s{ s" de suerte" s" afortunado" }s& s" ..." s+  ;

: enemy-speech$  ( -- ca len )
  \ Texto de las palabras del general enemigo.
  my-lucky-day$
  s{ s" Bien, bien..." s" Excelente..." }s&
  s{  s" Por el gran Ulfius"
        s{  s" podremos" s{ s" pedir" s" negociar" s" exigir" }s&
            s" pediremos" s" exigiremos" s" nos darán" s" negociaremos"
        }s& ransom$ s&
      s" Del gran Ulfius" s{ s" podremos" s" lograremos" }s&
        s{ s" sacar" s" obtener" }s&
        s{ ransom$ s{ s" alguna" s" una" }s s" buena ventaja" s& }s&
  }s&  ;

: enemy-speech  ( -- )
  \ Palabras del general enemigo.
  enemy-speech$ period+  speak  ;

: the-sad-end  ( -- )
  \ Final del juego con fracaso.
  s" Los sajones"
  \ XXX TODO -- añadir también el siguiente escenario, el lago
  location-10% am-i-there? if
    \ XXX TODO -- ampliar y variar
    comma+
    s" que te han visto entrar," s&
    s" siguen tus pasos y" s&
  then
  \ XXX TODO -- ampliar, explicar por qué no lo matan
  s" te" s&
  s{ s" hacen prisionero" s" capturan" s" atrapan" }s& period+
  s" Su general" s&
  s" , que no tarda en" s{ s" llegar" s" aparecer" }s& comma+ s?+
  s" te" s&{
    s" reconoce" s{ s" enseguida" s" de immediato" s" al instante" }s&
    s{ s" contempla" s" observa" s" mira" }s
      s" durante" s?& s{ s" un momento" s" un instante" }s&
  }s& s" y" s& comma+
  s{  s" sonriendo" s{ s" ampliamente" s" despectivamente" s" cruelmente" }s&
      s" con una" s{ s" amplia" s" cruel" s" despectiva" }s& s" sonrisa" s&
  }s& comma+
  s{  s" exclama" s" dice" }s& colon+ ^uppercase narrate
  narration-break enemy-speech  ;

: the-end  ( -- )
  \ Mensaje final del juego.
  success? if  the-happy-end  else  the-sad-end  then
  scene-break  ;

: (finish)  ( -- )
  \ Abandonar el juego.
  restore-vocabularies system_colors cr .forth quit  ;

:noname  ( -- )
  \ Acción de abandonar el juego.
  surrender?  if
    \ retry?$  cr no? ?? (finish)
    (finish)
  then
  ; is finish

\ }}} ==========================================================
section( Acerca del programa)  \ {{{

: based-on  ( -- )
  \ Muestra un texto sobre el programa original.
  s" «Asalto y castigo» está basado"
  s" en el programa homónimo escrito en BASIC en 2009 por" s&
  s" Baltasar el Arquero (http://caad.es/baltasarq/)." s&
  paragraph  ;

: license  ( -- )
  \ Muestra un texto sobre la licencia.
  s" (C) 2011-2016 Marcos Cruz (programandala.net)" paragraph
  s" «Asalto y castigo» es un programa libre;"
  s" puedes distribuirlo y/o modificarlo bajo los términos de" s&
  s" la Licencia Pública General de GNU, tal como está publicada" s&
  s" por la Free Software Foundation (Fundación para los Programas Libres)," s&
  s" bien en su versión 2 o, a tu elección, cualquier versión posterior" s&
  s" (http://gnu.org/licenses/)." s& \ XXX TODO -- confirmar
  paragraph  ;

: program  ( -- )
  \ Muestra el nombre y versión del programa.
  s" «Asalto y castigo» (escrito en Gforth)" paragraph
  s" Versión " version s& paragraph  ;

: about  ( -- )
  \ Muestra información sobre el programa.
  new-page about-color
  program cr license cr based-on
  scene-break  ;

\ }}} ==========================================================
section( Introducción)  \ {{{

: sun$  ( -- ca len )
  s{ s" sol" s" astro rey" }s  ;

: intro-0  ( -- )
  \ Muestra la introducción al juego (parte 0).
  s{
  s{ s" El " s" La luz del" }s sun$ s&
    s{ s" despunta de entre" s" penetra en" s" atraviesa" s" corta" }s&
  s" Los rayos del" sun$ s&
    s{ s" despuntan de entre" s" penetran en" s" atraviesan" s" cortan" }s&
  }s
  s" la" s& s" densa" s?& s" niebla," s&
  s" haciendo humear los" s& s" pobres" s?& s" tejados de paja." s&
  narrate  narration-break  ;

: intro-1  ( -- )
  \ Muestra la introducción al juego (parte 1).
  s" Piensas en"
  s{ s" el encargo de"
  s" la" s{ s" tarea" s" misión" }s& s" encomendada por" s&
  s" la orden dada por" s" las órdenes de" }s&
  s{ s" Uther Pendragon" s" , tu rey" s?+ s" tu rey" }s& \ XXX TMP
  s" ..." s+
  narrate  narration-break  ;

: intro-2  ( -- )
  \ Muestra la introducción al juego (parte 2).
  s{ s" Atacar" s" Arrasar" s" Destruir" }s s" una" s&
  s" aldea" s{ s" tranquila" s" pacífica" }s rnd2swap s& s&
  s" , aunque" s+ s{ s" se trate de una" s" sea una" s" esté" }s&
  s{ s" llena de" s" habitada por" s" repleta de" }s&
  s" sajones, no te" s&{ s" llena" s" colma" }s&
  s" de orgullo." s&
  narrate  narration-break  ;

: intro-3  ( -- )
  \ Muestra la introducción al juego (parte 3).
  \ XXX FIXME -- comprobar «cernirse»; añadir otras opciones
  ^your-soldiers$ s{
    s" se" s{ s" ciernen" s" lanzan" s" arrojan" }s& s" sobre" s&
    s" se" s{ s" amparan" s" apoderan" }s& s" de" s&
    s{ s" rodean" s" cercan" }s
  }s& s" la aldea y la" s&
  s{ s" destruyen." s" arrasan." }s&
  s" No hubo" s&{
    s" tropas enemigas"
    s" ejército enemigo"
    s" guerreros enemigos"
  }s&
  s{
    s"  ni honor" s" alguno" s?&
    s" , como tampoco honor" s" alguno" s?& comma+
  }s+
  s" en" s& s{ s" la batalla" s" el combate" s" la lucha" s" la pelea" }s& period+
  narrate  scene-break  ;

: intro-4  ( -- )
  \ Muestra la introducción al juego (parte 4).
  sire,$ s{
  s" el asalto" s" el combate" s" la batalla"
  s" la lucha" s" todo"
  }s& s" ha" s&{ s" terminado" s" concluido" }s&
  period+ speak  ;

: needed-orders$  ( -- ca len )
  \ Devuelve una variante de «órdenes necesarias».
  s" órdenes" s{ "" s" necesarias" s" pertinentes" }s&  ;

: intro-5  ( -- )
  \ Muestra la introducción al juego (parte 5).
  s" Lentamente," s{
  s" ordenas"
  s" das la orden de"
  s" das las" needed-orders$ s& s" para" s&
  }s& to-go-back$ s& s" a casa." s&
  narrate  narration-break  ;

: intro-6  ( -- )
  \ Muestra la introducción al juego (parte 6).
  [false] [if]  \ Primera versión, descartada
    ^officers-forbid-to-steal$ period+
  [else]  \ Segunda versión
    soldiers-steal-spite-of-officers$ period+
  [then]
  narrate  scene-break  ;

: intro  ( -- )
  \ Muestra la introducción al juego .
  new-page
  intro-0 intro-1 intro-2 intro-3 intro-4 intro-5 intro-6  ;

\ }}} ==========================================================
section( Principal)  \ {{{

: init-once  ( -- )
  \ Preparativos que hay que hacer solo una vez, antes de la primera partida.
  restore-vocabularies  init-screen  ;

: init-parser/game  ( -- )
  \ Preparativos que hay que hacer en el intérprete
  \ de comandos antes de cada partida.
  \ XXX TODO -- trasladar a su zona
  erase-last-command-elements  ;

: init-game  ( -- )
  \ Preparativos que hay que hacer antes de cada partida.
  randomize
  init-parser/game
  init-entities init-plot
  get-config new-page
  [true] [if]
    \ about cr intro
    location-01% enter-location
  [else]  \ XXX INFORMER
    \ XXX TODO -- activar selectivamente para depuración:
    \ location-08% enter-location  \ Emboscada
    \ location-11% enter-location  \ Lago
    \ location-17% enter-location  \ Antes de la cueva oscura
    \ location-19% enter-location  \ Encuentro con Ambrosio
    \ location-28% enter-location  \ Refugiados
    \ location-47% enter-location  \ casa de Ambrosio
    \ snake% is-here
    \ ambrosio% is-here
    \ key% is-hold
  [then]  ;

: game  ( -- )
  begin  plot listen obey  game-over?  until  ;
  \ Bucle de la partida.

: (main)  ( -- )
  begin  init-game game the-end  enough?  until  ;

: main  ( -- )
  init-once (main) farewell  ;
  \ Bucle principal del juego.

also forth definitions
' main alias go
' main alias run

: i0  ( -- )
  \ XXX TMP -- hace toda la inicialización; para depuración.
  init-once init-game
  s" Datos preparados." paragraph  ;

\ i0 cr  \ XXX TMP -- para depuración

\ }}} ==========================================================
section( Pruebas)  \ {{{

\ Esta sección contiene código para probar el programa
\ sin interactuar con el juego, para detectar mejor posibles
\ errores.

true [if]

: pp  ( -- )
  \ this does not work fine!
  page ." first para. press any key or wait 3 seconds."
  3 (break) ." second para"  ;

: ww  ( -- )
  \ this works fine
  page ." first para. press any key or wait 3 seconds."
  3 wait ." second para"  ;

: tt  ( -- )
  page ." first para. press any key or wait 3 seconds."
  trm+erase-line print_start_of_line
  ." second para"  ;

: -pp  ( -- )
  \ this works fine
  page ." first para. press any key or wait."
  -3 (break) ." second para"  ;

: -ww  ( -- )
  \ this works fine
  page ." first para. press any key or wait"
  -3 wait ." second para"  ;

: check-stack1  ( -- )
  \ Provoca un error -3 («stack overflow») si la pila no tiene solo un elemento.
  depth 1 <> -3 and throw  ;

: check-stack  ( -- )
  \ Provoca un error -3 («stack overflow») si la pila no está vacía.
  depth 0<> -3 and throw  ;

: test-location-description  ( a -- )
  \ Comprueba todas las descripciones de un ente escenario.
  cr ." = Descripción de escenario =======" cr
  dup my-location!
  describe-location check-stack
  cr ." == Mirar al norte:" cr
  north% describe-direction check-stack
  cr ." == Mirar al sur:" cr
  south% describe-direction check-stack
  cr ." == Mirar al este:" cr
  east% describe-direction check-stack
  cr ." == Mirar al oeste:" cr
  west% describe-direction check-stack
  cr ." == Mirar hacia arriba:" cr
  up% describe-direction check-stack
  cr ." == Mirar hacia abajo:" cr
  down% describe-direction check-stack
\ Aún no implementado:
\ cr ." == Mirar hacia fuera:" cr
\ out% describe-direction check-stack
\ cr ." == Mirar hacia dentro:" cr
\ in% describe-direction check-stack
  ;

0 value tested
: test-description  ( a -- )
  \ Comprueba la descripciones de un ente.
  to tested
  cr ." = Nombre =========================" cr
  tested full-name type
  cr ." = Descripción ====================" cr
  tested describe check-stack
  tested is-location? if  tested test-location-description  then  ;

: test-descriptions  ( -- )
  \ Comprueba la descripción de todos los entes.
  #entities 0 do
    i #>entity test-description
  loop  ;

: test-battle-phase  ( u -- )
  \ Comprueba una fase de la batalla.
  32 0 do  \ 32 veces cada fase, porque los textos son aleatorios
    dup (battle-phase) check-stack1
  loop  drop  ;

: test-battle  ( -- )
  \ Comprueba todas las fases de la batalla.
  battle-phases 0 do
    i test-battle-phase
  loop  ;

: check-prepos
  s" mira espada usando capa"
  ~~
  init-parsing
  ~~
  valid-parsing?
  ~~
  0= abort" parsing failed"
  ~~
  used-prepositions ?
  ~~  ;

[then]

only forth

