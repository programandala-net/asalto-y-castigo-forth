\ display.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607111056

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Colores

: colors  ( u1 u2 -- )  ink paper  ;
  \ Pone el color de papel _u1_ y tinta _u2_.

: @colors  ( a1 a2 -- )  @ swap @ swap colors  ;
  \ Pone el color de papel contenido en _a1_ y el de tinta contenido
  \ en _a2_.

\ ==============================================================
\ Colores utilizados

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
  light-red language-error-ink !
  black language-error-paper !
  gray action-error-ink !
  black action-error-paper !
  light-cyan input-ink !
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
  light-gray speech-ink !
  black speech-paper !
  light-red system-error-ink !
  black system-error-paper !
  green narration-prompt-ink !
  black narration-prompt-paper !  ;
  \ Asigna los colores predeterminados.

: about-color  ( -- )
  about-paper about-ink @colors  ;
  \ Pone el color de texto de los créditos.

: command-prompt-color  ( -- )
  command-prompt-paper command-prompt-ink @colors  ;
  \ Pone el color de texto del presto de entrada de comandos.

:noname  ( -- )
  debug-paper debug-ink @colors  ; is debug-color
  \ Pone el color de texto usado en los mensajes de depuración.

: background-color  ( -- )
  [defined] background-paper
  [if]    background-paper @ paper
  [else]  system-background-color
  [then]  ;
  \ Pone el color de fondo.

: description-color  ( -- )
  description-paper description-ink @colors  ;
  \ Pone el color de texto de las descripciones de los entes que no
  \ son escenarios.

: system-error-color  ( -- )
  system-error-paper language-error-ink @colors  ;
  \ Pone el color de texto de los errores del sistema.

: action-error-color  ( -- )
  action-error-paper language-error-ink @colors  ;
  \ Pone el color de texto de los errores operativos.

: language-error-color  ( -- )
  language-error-paper language-error-ink @colors  ;
  \ Pone el color de texto de los errores lingüísticos.

: input-color  ( -- )
  input-paper input-ink @colors  ;
  \ Pone el color de texto para la entrada de comandos.

: location-description-color  ( -- )
  location-description-paper location-description-ink @colors  ;
  \ Pone el color de texto de las descripciones de los entes escenario.

: location-name-color  ( -- )
  location-name-paper location-name-ink @colors  ;
  \ Pone el color de texto del nombre de los escenarios.

: narration-color  ( -- )
  narration-paper narration-ink @colors  ;
  \ Pone el color de texto de la narración.

: scroll-prompt-color  ( -- )
  scroll-prompt-paper scroll-prompt-ink @colors  ;
  \ Pone el color de texto del presto de pantalla llena.

: question-color  ( -- )
  question-paper question-ink @colors  ;
  \ Pone el color de texto de las preguntas de tipo «sí o no».

: scene-prompt-color  ( -- )
  scene-prompt-paper scene-prompt-ink @colors  ;
  \ Pone el color de texto del presto de fin de escena.

: speech-color  ( -- )
  speech-paper speech-ink @colors  ;
  \ Pone el color de texto de los diálogos.

: narration-prompt-color  ( -- )
  narration-prompt-paper narration-prompt-ink @colors  ;
  \ Pone el color de texto del presto de pausa.

\ ==============================================================
\ Demo de colores

false [if]

\ Dos palabras para probar cómo se ven los colores

: color-bar  ( u -- )
  paper cr 32 spaces  black paper space  ;
  \ Imprime una barra de 64 espacios con el color indicado.

: color-demo  ( -- )
  cr ." Colores descritos como se ven en Debian"
  black color-bar ." negro"
  gray color-bar ." gris"
  light-gray color-bar ." gris claro"
  blue color-bar ." azul"
  light-blue color-bar ." azul claro"
  cyan color-bar ." cian"
  light-cyan color-bar ." cian claro"
  green color-bar ." verde"
  light-green color-bar ." verde claro"
  magenta color-bar ." magenta"
  light-magenta color-bar ." magenta claro"
  red color-bar ." rojo"
  light-red color-bar ." rojo claro"
  brown color-bar ." marrón"
  yellow color-bar ." amarillo"
  white color-bar ." blanco"
  \ red color-bar ." rojo"
  \ brown color-bar ." marrón"
  \ red color-bar ." rojo"
  \ brown color-bar ." marrón"
  system-colors cr  ;
  \ Prueba los colores.

[then]

\ ==============================================================
\ Otros atributos tipográficos

: bold  ( -- )
  trm.bold 1 sgr  ;
  \ Activa la negrita.

: underline  ( f -- )
  if  trm.underscore-on  else  trm.underline-off  then  1 sgr  ;
  \ Activa o desactiva el subrayado.

' underline alias underscore
: inverse  ( f -- )
  if  trm.reverse-on  else  trm.reverse-off  then  1 sgr  ;
  \ Activa o desactiva la inversión de colores (papel y tinta).

false [if]  \ XXX TODO
: blink ( f -- )
  if  trm.blink-on  else  trm.blink-off  then  1 sgr  ;
  \ Activa o desactiva el parpadeo.
  \ XXX FIXME -- no funciona
[then]

: italic  ( f -- )
  \ Activa o desactiva la cursiva.
  \ Nota: tiene el mismo efecto que `inverse`.
  if  trm.italic-on  else  trm.italic-off  then  1 sgr  ;

\ ==============================================================
\ Borrado de pantalla

: reset-scrolling  ( -- )
  [char] r trm+do-csi0  ;
  \ Desactiva la definición de zona de pantalla como ventana.

[defined] background-paper [if]  \ XXX TMP -- experimental

: (color-background)  ( u -- )
  paper print_home
  [false] [if]  \ Por líneas, más lento
    rows 0 do  i ?? cr [ cols ] literal spaces  loop
  [else]  \ Pantalla entera, más rápido
    form * spaces
  [then]  ;
  \ Colorea el fondo de la pantalla con el color indicado.
  \ No sirve de mucho colorear la pantalla, porque la edición de textos
  \ utiliza el color de fondo predeterminado del sistema, el negro,
  \ cuando se borra el texto que está siendo escrito.
  \ No se ha comprobado si en Windows ocurre lo mismo.
  \ No sirve usar `trm+set-default-attributes`.

: color-background  ( -- )
  background-paper @ dup 0>=
  if  (color-background)  else  drop  then  ;
  \ Colorea el fondo de la pantalla, si el color no es negativo.

[else]
' trm+erase-display alias color-background
[then]

: new-page  ( -- )
  color-background print_home  ;
  \ Borra la pantalla y sitúa el cursor en su origen.

: clear-screen-for-location  ( -- )
  location-page? @ ?? new-page  ;
  \ Restaura el color de tinta y borra la pantalla para cambiar de escenario.

: init-screen  ( -- )
  trm+reset init-colors  ;
  \ Prepara la pantalla la primera vez.

\ vim:filetype=gforth:fileencoding=utf-8
