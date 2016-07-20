\ config.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607202057

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/colors.fs      \ color constants
require galope/sourcepath.fs  \ `sourcepath`
require galope/stringer.fs    \ Circular string buffer

\ Forth Foundation Library
\ http://irdvo.github.io/ffl/

set-current

\ ==============================================================
\ Fichero de configuración

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

: config-dir$  ( -- ca len )  s" ../config/"  ;
  \ Directorio de los ficheros de configuración.

: default-config-file$  ( -- ca len )  s" predeterminado.fs"  ;
  \ Fichero de configuración predeterminado, sin ruta.

svariable temporary-config-file  temporary-config-file off

: current-config-file$  ( -- ca len )
  temporary-config-file count dup
  if    >stringer temporary-config-file off
  else  2drop default-config-file$  then  ;
  \ Fichero de configuración actual: el temporal especificado en un
  \ comando o, si su nombre está vacío, el predeterminado. Sin ruta
  \ en cualquier caso.

: config-file$  ( -- ca len )
  path$ config-dir$ s+ current-config-file$ s+  ;
  \ Fichero de configuración con su ruta completa.

wordlist dup constant config-wordlist
         dup >order set-current

\ Las palabras cuyas definiciones siguen a continuación
\ se crearán en la lista `config-wordlist` y
\ son las únicas que podrán usarse para configurar el juego
\ en el fichero de configuración:

' ( alias (  immediate  \ )
' \ alias \  immediate
' true alias sí
' false alias no

' include alias incluye

: varón  ( -- )  woman-player? off  ;
  \ Configura que el jugador es un varón.

' varón alias masculino

: mujer  ( -- )  woman-player? on  ;
  \ Configura que el jugador es una mujer.

' mujer alias femenino

: comillas  ( f -- )  castilian-quotes? !  ;
  \ Configura si se usan las comillas castellanas en las citas.

: espacios_de_indentación  ( u -- )
  max-indentation min /indentation !  ;
  \ Fija la indentación de los párrafos.

: indentar_primera_línea_de_pantalla  ( f -- )
  indent-first-line-too? !  ;
  \ Configura si se indentará también la línea superior de la pantalla, si un párrafo empieza en ella.

: indentar_prestos_de_pausa  ( f -- )
  indent-pause-prompts? !  ;
  \ Configura si se indentarán los prestos.

: borrar_pantalla_para_escenarios  ( f -- )
  location-page? !  ;
  \ Configura si se borra la pantalla al entrar en un escenario o describirlo.

: borrar_pantalla_para_escenas  ( f -- )
  scene-page? !  ;
  \ Configura si se borra la pantalla tras la pausa de fin de escena.

: separar_párrafos  ( f -- )
  cr? !  ;
  \ Configura si se separan los párrafos con un línea en blanco.

: segundos_en_pausas_de_narración  ( u -- )
  narration-break-seconds !  ;
  \ Configura los segundos de las pausas cortas (o, si es valor es
  \ cero, que hay que pulsar una tecla).

: segundos_en_pausas_de_final_de_escena  ( u -- )
  scene-break-seconds !  ;
  \ Configura los segundos de las pausas de final de esecena (o, si es
  \ valor es cero, que hay que pulsar una tecla).

' black alias negro
' blue alias azul
' light-blue alias azul_claro
' brown alias marrón
' cyan alias cian
' light-cyan alias cian_claro
' green alias verde
' light-green alias verde_claro
' gray alias gris
' light-gray alias gris_claro
' magenta alias magenta
' light-magenta alias magenta_claro
' red alias rojo
' light-red alias rojo_claro
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
  language-errors-verbosity !  ;
  \ Configura el nivel de detalle de los mensajes de error lingüístico.
  \ El rango límite es ajustado cuando se usa.

: mensaje_genérico_de_error_lingüístico  ( ca len -- )
  'generic-language-error$ place  ;
  \ Configura el mensaje genérico para los mensajes de error lingüístico.

: detalle_de_los_mensajes_de_error_operativo  ( n -- )
  action-errors-verbosity !  ;
  \ Configura el nivel de detalle de los mensajes de error operativo.
  \ El rango límite es ajustado cuando se usa.

: mensaje_genérico_de_error_operativo  ( ca len -- )
  'generic-action-error$ place  ;
  \ Configura el mensaje genérico para los mensajes de error operativo.

restore-wordlists
  \ Terminar de definir palabras permitidas en el fichero
  \ configuración.

: init-prompts  ( -- )
  indent-pause-prompts? on
  s" ..." scroll-prompt place
  s" ..." narration-prompt place
  s" ..." scene-prompt place
  s" >" command-prompt place
  space-after-command-prompt? on
  cr-after-command-prompt? off  ;
  \ Inicializa los prestos con sus valores predeterminados.

: init-config  ( -- )
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
  s" Orden incorrecta." 'generic-language-error$ place
  s" No es posible hacer eso." 'generic-action-error$ place
  init-prompts  init-colors  ;
  \ Inicializa las variables de configuración con sus valores
  \ predeterminados.

false [if]

: read-config-error  ( n -- )
  s" Se ha producido un error #"
  rot n>str s+
  s"  leyendo el fichero de configuración." s+
  system-error.  ;
  \ XXX TODO -- El error no es significativo porque siempre es #-37,
  \ no el que ha causado el fallo de interpretación del fichero.
  \ Por eso de momento esta versión está desactivada.

[else]

: read-config-error  ( -- )
  s" Se ha producido un error leyendo el fichero de configuración."
  system-error.  ;

[then]

: read-config  ( -- )
  config-wordlist 1 set-order
  config-file$ ['] included catch  ( x1 x2 n | 0 )
  restore-wordlists
  \ ?dup if  nip nip ( n ) read-config-error  then  ; \ XXX TODO
  if  2drop read-config-error  then  ;
  \ Lee el fichero de configuración.

: get-config  ( -- )
  init-config read-config  ;
  \ Inicializa las variables de configuración
  \ y lee el fichero de configuración.

\ vim:filetype=gforth:fileencoding=utf-8
