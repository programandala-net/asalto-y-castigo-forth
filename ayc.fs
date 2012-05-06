\ ##############################################################
CR .( Asalto y castigo )  \ {{{

\ Una aventura conversacional en castellano, escrita en Forth.
\ A text adventure in Spanish, written in Forth.

\ Copyright (C) 2011,2012 Marcos Cruz (programandala.net)

only forth definitions
: version$  ( -- a u )  s" A-04-2012050701"  ;
version$ type cr

\ 'Asalto y castigo' is free software; you can redistribute
\ it and/or modify it under the terms of the GNU General
\ Public License as published by the Free Software
\ Foundation; either version 2 of the License, or (at your
\ option) any later version.

\ http://gnu.org/licenses/
\ http://gnu.org/licenses/gpl.html
\ http://www.gnu.org/licenses/gpl-2.0.html

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

(

Información

Juegos conversacionales:
  http://caad.es
Forth:
  http://programandala.net/es.artículo.2009.04.27.libros_forth
  http://forth.org
  http://www.forthfreak.net
  http://groups.google.com/group/comp.lang.forth/
Gforth:
  http://www.jwdt.com/~paysan/gforth.html
  http://lists.gnu.org/mailman/listinfo/gforth

)

\ ##############################################################
\ Documentación

\ El historial de desarrollo está en:
\ http://programandala.net/es.programa.asalto_y_castigo.forth.historial

\ La lista de tareas pendientes está al final de este fichero.

\ Notación de la pila (incompleta!!!):

(

En este programa usamos las siguientes abreviaturas para
describir los elementos de la pila:

+n    = número de 32 bitios positivo
-n    = número de 32 bitios negativo
...   = elipsis: número variable de elementos, o rango
a     = dirección de memoria
a u   = dirección y longitud de zona de memoria, p.e. de un texto
b     = octeto, valor de ocho bitios
c     = carácter de un octeto
f     = indicador lógico: cero significa «falso»; otro valor significa «cierto»
false = 0
ff    = indicador puro de ANS Forth: 0=«falso»; -1=«cierto»
        [-1 es un valor de 32 bitios con todos los bitios a uno]
i*x   = grupo de elementos sin especificar; puede estar vacío
j*x   = grupo de elementos sin especificar; puede estar vacío
n     = número de 32 bitios con signo
true  = -1 [valor de 32 bitios con todos los bitios a uno]
u     = número de 32 bitios sin signo
x     = elemento indeterminado
xt    = «execution token», identificador de ejecución de una palabra;
        notación de ANS Forth análoga a «cfa»
        [«code field addrees»] en Forth clásico

Como es costumbre, los diferentes elementos del mismo tipo
se distinguirán con un sufijo, casi siempre un dígito,
o bien un apóstrofo, según los casos.

)

\ }}} ##########################################################
CR .( Requisitos)  \ {{{

\ ------------------------------------------------
\ De Gforth

require random.fs

\ ------------------------------------------------
\ De la librería «Forth Foundation Library»
\ (versión 0.8.0)
\ http://code.google.com/p/ffl/

(

Esta gran librería, preparada para ser compatible con los
sistemas Forth más utilizados, proporciona palabras
especializadas muy útiles, con el objetivo de ayudar a crear
aplicaciones en Forth.  Las palabras están agrupadas
temáticamente en módulos independientes.  Cada módulo de la
librería tiene por nombre una abreviatura de tres letras y
sus palabras comienzan por esas mismas tres letras.  Por
ejemplo: los nombres de las palabras proporcionadas por el
módulo «str» empiezan por «str», como 'str-create',
'str+columns' o 'str.version'.

)

require ffl/str.fs  \ Cadenas de texto dinámicas
require ffl/trm.fs  \ Manejo de terminal ANSI
require ffl/chr.fs  \ Herramientas para caracteres
require ffl/dtm.fs  \ Tipo de datos para fecha y hora
require ffl/dti.fs  \ Herramientas adicionales para fecha y hora

\ ------------------------------------------------
\ De programandala.net

\ Ghoul (Gforth Homemade Obviously Useful Library)

require ghoul/sb.fs \ Almacén circular de cadenas de texto
' bs+ alias s+
' bs& alias s&
' bs" alias s" immediate
2048 dictionary_sb

require ghoul/mdrop.fs  \ Eliminación de varios elementos de la pila
require ghoul/2mdrop.fs  \ Eliminación de varios elementos dobles de la pila
require ghoul/2choose.fs  \ Selección aleatoria de un elemento doble de la pila
' 2choose alias schoose
require ghoul/choose.fs  \ Selección aleatoria de un elemento de la pila
require ghoul/random_strings.fs  \ Selección aleatoria de cadenas de texto
require ghoul/xy.fs  \ Posición actual del cursor
require ghoul/print.fs  \ Impresión de textos ajustados
require ghoul/sconstant.fs \ Constantes de cadenas de texto
require ghoul/svariable.fs \ Variables de cadenas de texto
require ghoul/randomize.fs
require ghoul/between.fs  \ 'between' (variante habitual de 'within')
require ghoul/home.fs
require ghoul/enum.fs
require ghoul/increment.fs  \ '++'
require ghoul/decrement.fs  \ '--'
require ghoul/question-empty.fs  \ '?empty' y '?keep'

\ Otras herramientas

require halto2.fs \ Puntos de chequeo para depuración
false to halto?

\ }}} ##########################################################
\ Meta \ {{{

: wait  key drop  ;

: \eof ( -- )
  \ Ignora el resto del fichero
  source-id if  begin  refill 0=  until  then
  ;
: [or]  ( x1 x2 -- x3 )  or  ;  immediate

\ Indicadores para depuración

false value [debug] immediate  \ ¿Depuración global?
false value [debug_init] immediate  \ ¿Depurar la inicialización?
false value [debug_parsing] immediate  \ ¿Depurar el analizador?
false value [debug_filing] immediate  \ ¿Depurar operaciones de ficheros? 
false value [debug_do_exits] immediate  \ ¿Depurar la acción 'do_exits'?
false value [debug_catch] immediate  \ ¿Depurar 'catch' y 'throw'?
false value [debug_save] immediate  \ ¿Depurar la grabación de partidas?
true value [debug_info] immediate  \ ¿Mostrar info sobre el presto de comandos? 
false value [debug_pause] immediate  \ ¿Hacer pausa en puntos de depuración?
true value [debug_map] immediate  \ ¿Mostrar el número de escenario del juego original?

\ Indicadores para poder elegir alternativas que aún son experimentales

true constant [old_method]  immediate
[old_method] 0= constant [new_method]  immediate
true constant [dark_cave_in_global_plot?]  immediate
false constant [ambush_in_global_plot?]  immediate

\ Constantes

true constant [true] immediate  \ Para compilación condicional 
false constant [false] immediate  \ Para compilación condicional 

\ Títulos de sección

: depth_warning
  cr ." Aviso: La pila no está vacía. Contenido: " 
  ;
: .s?
  \ Imprime el contenido de la pila si no está vacía.
  depth
  if  depth_warning .s cr  wait 
  then
  ;
: section(  ( "text<bracket>" -- )
  \ Notación para los títulos de sección en el código fuente.
  \ Permite hacer tareas de depuración mientras se compila el programa;
  \ por ejemplo detectar el origen de descuadres en la pila.
  cr postpone .(  \ El nombre de sección terminará con: )
  .s?
  ;
: subsection(  ( "text<bracket>" -- )
  \ Notación para los títulos de subsección en el código fuente.
  [char] * emit space postpone .(  \ El nombre de subsección terminará con: )
  space .s?
  ;

\ }}} ##########################################################
section( Vocabularios de Forth)  \ {{{

\ Vocabulario principal del programa (no de la aventura)

vocabulary game_vocabulary

: restore_vocabularies
  \ Restaura los vocabularios a su orden habitual.
  \ En lina los vocabularios son inmediatos
  \ (aunque por alguna razón los creados con 'vocabulary'
  \ no reciben ese tratamiento de 'postpone'), y además al ejecutarse
  \ no sustituyen al primero en el orden, sino que se añaden al orden.
  \ Por ello lina necesita aquí un tratamiento especial:
  only forth also game_vocabulary definitions
  ;
restore_vocabularies

\ Demás vocabularios

vocabulary menu_vocabulary  \ palabras del menú \ Aún no se usa!!!
\ Vocabulario del jugador 

\ Gforth necesita su propio método
\ para crear un vocabulario sensible a mayúsculas,
\ con la palabra 'table':
table value (player_vocabulary)
\ : player_vocabulary  (player_vocabulary) >order  ;  \ Versión antigua!!!
: player_vocabulary
  \ Reemplaza el vocabulario superior con el del jugador.
  \ Código adaptado de Gforth (compat/vocabulary.fs).
  get-order dup 0= 50 and throw  \ Error 50 («search-order underflow») si la lista está vacía
  nip (player_vocabulary) swap set-order
  ;
vocabulary answer_vocabulary  \ respuestas a preguntas de «sí» o «no»
vocabulary config_vocabulary  \ palabras de configuración del juego
vocabulary restore_vocabulary  \ palabras de restauración de una partida 

\ }}} ##########################################################
section( Palabras genéricas)  \ {{{

true constant [true]  immediate
false constant [false]  immediate

pad 0 2constant ""  \ Simula una cadena vacía.

: (alias)  ( xt a u -- )
  \ Crea un alias de una palabra.
  \ xt = Dirección de ejecución de la palabra de la que hay que crear el alias
  \ a u = Nombre del alias
  \ Para definir esta palabra se ha examinado la definición
  \ de 'create' en Gforth; probablemente hay otra forma
  \ más directa de crear una palabra a partir de 
  \ de un xt y una cadena con su nombre.
  name-too-short? header, reveal dovar: cfa, ,
  does> perform
  ;

: ?++  ( a -- )
  \ Incrementa el contenido de una dirección, si es posible.
  \ En la práctica el límite es inalcanzable
  \ (pues es un número de 32 bitios),
  \ pero así queda mejor hecho. 
  \ Pendiente!!! Confirmar este cálculo, pues depende de si el número se considera con signo o no.
  dup @ 1+ ?dup if  swap !  else  drop  then
  ;
: different?  ( x1 x2 x1 -- ff )
  \ ¿Es x2 distinto de x1, y es x1 distinto de cero?
  \ Este cálculo se usa en dos lugares del programa.
  <> swap 0<> and
  ;

' bootmessage alias .forth

\ }}} ##########################################################
section( Definición de estructuras)  \ {{{

(

Creamos herramientas para definir y gestionar campos de datos
buleanos que usen un solo bitio.

)

variable bit#  \ Contador de máscara de bitio para los campos buleanos.
: bitfields  1 bit# !  ;
: bit#?  ( -- u )  bit# @  dup 0= abort" Too many bits defined in the field"  ;
: bit+  bit# @ 1 lshift bit# !  ;
: bitfield:  ( u1 "name" -- u1 )
  \ Crea un campo de bitio en el próximo campo de celda.
  \ u1 = Desplazamiento (respecto al inicio de la ficha) del próximo campo de celda que se creará
  create  bit#? over 2, bit+
  does>  ( a pfa -- u2 a' )
    \ a = Dirección de la ficha de datos
    \ u2 = Máscara del bitio
    \ a' = Dirección del campo en que está definido el bitio en la ficha de datos.
    2@ rot +
  ;
: bit_on  ( u a -- )
  \ Activa un campo de bitio.
  \ u = Máscara del bitio 
  \ a = Dirección del campo
  dup @ rot or swap !
  ;
: bit_off  ( u a -- )
  \ Desactiva un campo de bitio.
  \ u = Máscara del bitio 
  \ a = Dirección del campo
  dup @ rot invert and swap !
  ;
: bit!  ( f u a -- )
  \ Guarda un indicador en un campo de bitio.
  \ u = Máscara del bitio 
  \ a = Dirección del campo
  rot if  bit_on  else  bit_off  then
  ;
: bit@  ( u a -- ff )
  \ Devuelve el contenido de un campo de bitio.
  \ u = Máscara del bitio 
  \ a = Dirección del campo
  @ and 0<>
  ;

\ }}} ##########################################################
section( Vectores)  \ {{{

(

Algunas palabras se necesitan, como parte de la definición
de otras palabras, antes de haber sido escritas.  Esto
ocurre porque no siempre es posible ordenar el código fuente
de forma que todas las palabras que hay que ejecutar o
compilar hayan sido definidas con anterioridad.

En estos casos es necesario crear como vectores estas
palabras que se necesitan antes de tiempo. Los vectores son
palabras cuya dirección interna de ejecución puede ser
modificada para que apunte a la de otra palabra.  Así,
creamos vectores que serán actualizados más adelante en el
código fuente cuando se defina la palabra con el código que
deben ejecutar.

'defer' crea una palabra que no hace nada, pero cuya dirección
de ejecución podrá ser después cambiada usando la palabra 'is'
como en el siguiente ejemplo:

)

false [if]  \ Ejemplo de código

defer palabrita  \ Crear el vector
: usar_palabrita  \ Palabra que usa 'palabrita' y que por tanto necesita que esté ya en el diccionario
  \ La compilación no da error, porque 'palabrita' existe en el diccionario, pues ha sido creada por 'defer',
  \ pero la ejecución posterior no haría nada porque el vector 'palabrita' no ha sido actualizado.
  palabrita 
  ;
: (palabrita)
  \ Definición de lo que tiene que hacer 'palabrita'.
  ." ¡Hola mundo, soy palabrita!"
  ;
\ Tomar la dirección de ejecución de '(palabrita)' y ponérsela al vector 'palabrita':
' (palabrita) is palabrita
\ Ahora tanto 'palabrita' como 'usar_palabrita'
\ harán lo mismo que '(palabrita)'.

[then]  \ Fin del ejemplo

defer protagonist%  \ Ente protagonista
defer sword%  \ Ente espada
defer stone%  \ Ente piedra
defer torch%  \ Antorcha
defer leader%  \ Ente líder de los refugiados
defer location_01%  \ Primer ente escenario

defer do_exits  \ Acción de listar las salidas
defer list_exits  \ Crea e imprime la lista de salidas
defer exits%  \ Ente "salidas"

\ }}} ##########################################################
section( Códigos de error)  \ {{{

(

En el estándar ANS Forth los códigos de error de -1 a -255
están reservados para el propio estándar; el resto de
números negativos se reservan para que los asigne cada
sistema Forth a sus propios mensajes de error; del 1 en
adelante puede usarlos libremente cada programa.

En este programa usamos como códigos de error las
direcciones de ejecución de las palabras que muestran los
errores.  En Forth, la dirección de ejecución de una palabra
se llama tradicionalmente «code field address», o «cfa» en
notación de la pila. Pero el estándar ANS Forth de 1994, el
más extendido en la actualidad, utiliza el término
«execution token», o «xt» en la notación de la pila, pues en
algunos sistemas Forth no es una dirección de memoria sino
un código interno. En este programa lo llamamos «dirección
de ejecución» y en la notación de pila lo representamos
como «xt».

En cualquier caso se trata de lo mismo: es el valor que
devuelven las palabras ''' y '[']' y que sirve de parámetro a
la palabra 'execute'.

)

false [if]  \ Ejemplo de código:

  : palabrita  ." ¡Hola mundo!"  ;
  variable palabrita_xt
  ' palabrita palabrita_xt !
  palabrita_xt @ execute

[then]  \ Fin del ejemplo

(

Como se ve, usar como códigos de error las direcciones de
ejecución de las palabras de error tiene la ventaja de que no
hace falta ningún mecanismo adicional para encontrar las
palabras de error a partir de sus códigos de error
correspondientes, como podría ser una estructura 'case' o una
tabla: basta poner el código de error en la pila y llamar a
'execute'.

Dado que algunos los códigos de error se necesitan antes de
haber sido creadas las palabras de error, por ejemplo
durante la creación de los entes, los creamos aquí por
adelantado como vectores y los actualizaremos
posteriormente, cuando se definan las palabras de error,
exactamente como se muestra en este ejemplo:

)

false [if]  \ Ejemplo de código:

  defer la_cagaste_error#
  : la_cagaste  ." ¡La cagaste!"  ;
  ' la_cagaste constant (la_cagaste_error#)
  ' (la_cagaste_error#) is la_cagaste_error#

[then]  \ Fin del ejemplo

\ false constant no_error# \ No se usa!!!

defer cannot_see_error#
defer cannot_see_what_error#
defer dangerous_error#
defer impossible_error#
defer is_normal_error#
defer is_not_here_error#
defer is_not_here_what_error#
defer no_main_complement_error# 
defer no_verb_error#
defer nonsense_error#
defer not_allowed_main_complement_error# 
defer useless_tool_error# 
defer useless_what_tool_error# 
defer not_allowed_tool_complement_error# 
defer repeated_preposition_error#
defer too_many_actions_error#
defer too_many_complements_error#
defer unexpected_main_complement_error# 
defer unexpected_secondary_complement_error# 
defer unnecessary_tool_error#
defer unnecessary_tool_for_that_error#
defer unresolved_preposition_error# 
defer what_is_already_closed_error#
defer what_is_already_open_error#
defer you_already_have_it_error#
defer you_already_have_what_error#
defer you_already_wear_what_error#
defer you_do_not_have_it_error#
defer you_do_not_have_what_error#
defer you_do_not_wear_what_error#
defer you_need_what_error#

\ }}} ##########################################################
section( Herramientas de azar)  \ {{{

\ Desordenar al azar varios elementos de la pila
\ Pendiente!!! Extraer a la librería.

0 value unsort#
: unsort  ( x1 ... xu u -- x1' ... xu' )
  \ Desordena un número de elementos de la pila.
  \ x1 ... xu = Elementos a desordenar
  \ u = Número de elementos que hay que desordenar 
  \ x1' ... xu' = Los mismos elementos, desordenados
  dup to unsort# 0 ?do
    unsort# random roll
  loop  
  ;

\ Combinar cadenas de forma aleatoria

: rnd2swap  ( a1 u1 a2 u2 -- a1 u1 a2 u2 | a2 u2 a1 u1 )
  \ Intercambia (con 50% de probabililad) la posición de dos textos.
  2 random if  2swap  then
  ;
: (both)  ( a1 u1 a2 u2 -- a1 u1 a3 u3 a2 u2 | a2 u2 a3 u3 a1 u1 )
  \ Devuelve las dos cadenas recibidas, en cualquier orden,
  \ y separadas en la pila por la cadena «y».
  rnd2swap s" y" 2swap
  ;
: both  ( a1 u1 a2 u2 -- a3 u3 )
  \ Devuelve dos cadenas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los dos resultados posibles serían: «fría y espesa» y «espesa y fría».
  (both) bs& bs&
  ;
: both&  ( a0 u0 a1 u1 a2 u2 -- a3 u3 )
  \ Devuelve dos cadenas unidas en cualquier orden por «y»; y concatenada (con separación) a una tercera.
  both bs&
  ;
: both?  ( a1 u1 a2 u2 -- a3 u3 )
  \ Devuelve al azar una de dos cadenas,
  \ o bien ambas unidas en cualquier orden por «y».
  \ Ejemplo: si los parámetros fueran «espesa» y «fría»,
  \ los cuatro resultados posibles serían:
  \ «espesa», «fría», «fría y espesa» y «espesa y fría».
  (both) s&? bs&
  ;
: both?&  ( a0 u0 a1 u1 a2 u2 -- a3 u3 )
  \ Concatena (con separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.
  both? bs&
  ;
: both?+  ( a0 u0 a1 u1 a2 u2 -- a3 u3 )
  \ Concatena (sin separación) al azar una de dos cadenas
  \ (o bien ambas unidas en cualquier orden por «y») a una tercera cadena.
  both? bs+
  ;

\ }}} ##########################################################
section( Variables)  \ {{{

\ Algunas variables de configuración
\ (el resto se crea en sus propias secciones)

variable woman_player?  \ ¿El jugador es una mujer?
variable castilian_quotes?  \ ¿Usar comillas castellanas en las citas, en lugar de raya?
variable location_page?  \ ¿Borrar la pantalla antes de entrar en un escenario o de describirlo?
variable cr?  \ ¿Separar los párrafos con una línea en blanco?
variable ignore_unknown_words?  \ ¿Ignorar las palabras desconocidas?  \ No se usa todavía!!!
variable scene_page?  \ ¿Borrar la pantalla después de la pausa de los cambios de escena?
variable verbose_language_errors?  \ ¿Errores lingüísticos detallados?
 
\ Variables de la trama

variable ambrosio_follows?  \ ¿Ambrosio sigue al protagonista?
variable battle#  \ Contador de la evolución de la batalla (si aún no ha empezado, es cero)
variable climbed_the_fallen_away?  \ ¿El protagonista ha intentado escalar el derrumbe?
variable hacked_the_log?  \ ¿El protagonista ha afilado el tronco?
\ variable hold#  \ Contador de cosas llevadas por el protagonista (no se usa!!!)
variable stone_forbidden?  \ ¿El protagonista ha intentado pasar con la piedra?
variable sword_forbidden?  \ ¿El protagonista ha intentado pasar con la espada?
variable recent_talks_to_the_leader  \ Contador de intentos de hablar con el líder sin cambiar de escenario

: init_plot
  \ Inicializa las variables de la trama.
  ambrosio_follows? off
  battle# off
  climbed_the_fallen_away? off
  hacked_the_log? off
  stone_forbidden? off
  sword_forbidden? off
  recent_talks_to_the_leader off
  ;

\ }}} ##########################################################
section( Pantalla)  \ {{{

\ ------------------------------------------------
subsection( Variables y constantes)  \ {{{

\ Parámetros SGR (Select Graphic Rendition)
\ que no están incluidos en el módulo trm de Forth Foundation library.
\ Referencia:
\ http://en.wikipedia.org/wiki/ANSI_escape_code#graphics

003 constant trm.italic-on
023 constant trm.italic-off
090 constant trm.foreground-black-high
091 constant trm.foreground-red-high
092 constant trm.foreground-green-high
093 constant trm.foreground-brown-high
094 constant trm.foreground-blue-high
095 constant trm.foreground-magenta-high
096 constant trm.foreground-cyan-high
097 constant trm.foreground-white-high
100 constant trm.background-black-high
101 constant trm.background-red-high
102 constant trm.background-green-high
103 constant trm.background-brown-high
104 constant trm.background-blue-high
105 constant trm.background-magenta-high
106 constant trm.background-cyan-high
107 constant trm.background-white-high

' trm+set-attributes alias sgr

\ }}}---------------------------------------------
subsection( Colores)  \ {{{

(

Notas sobre las pruebas realizadas en Debian
con el módulo trm de Forth Foundation Library:

TRM.HALF-BRIGHT causa subrayado, igual que TRM.UNDERSCORE-ON 
TRM.ITALIC-ON causa vídeo inverso, igual que TRM.REVERSE-ON
TRM.FOREGROUND-WHITE pone un blanco apagado diferente al predeterminado.

Referencia:
http://en.wikipedia.org/wiki/ANSI_escape_code

)

: lighter  ( u1 -- u2 )
  \ Incrementa el brillo de un color (entre los colores normales y los brillantes
  \ hay una diferencia numérica fija).
  [ trm.foreground-black-high trm.foreground-black - ] literal +
  ;

: black  ( -- u )  trm.foreground-black  ;
: blue  ( -- u )  trm.foreground-blue  ;
: light_blue  ( -- u )  blue lighter  ;
: brown  ( -- u )  trm.foreground-brown  ;
: cyan  ( -- u )  trm.foreground-cyan  ;
: light_cyan  ( -- u )  cyan lighter  ;
: green  ( -- u )  trm.foreground-green  ;
: light_green  ( -- u )  green lighter  ;
: gray  ( -- u )  trm.foreground-white  ;
: dark_gray  ( -- u )  black lighter  ;
: magenta  ( -- u )  trm.foreground-magenta  ;
: light_magenta  ( -- u )  magenta lighter  ;
: red  ( -- u )  trm.foreground-red  ;
: light_red  ( -- u )  red lighter  ;
: white  ( -- u )  gray lighter  ;
: yellow  ( -- u )  brown lighter  ;

: ink ( u -- )  1 sgr  ;
: paper  ( u -- )  10 + ink  ;
: colors  ( u1 u2 -- )
  \ Pone los colores de papel y tinta.
  \ u1 = Color de papel
  \ u2 = Color de tinta
  ink paper
  ;
: @colors  ( a1 a2 -- )
  \ Pone los colores de papel y tinta con el contenido de dos variables.
  \ a1 = Dirección del color de papel
  \ a2 = Dirección del color de tinta
  @ swap @ swap colors
  ;
\ }}}---------------------------------------------
subsection( Colores utilizados)  \ {{{

\ Variables para guardar cada color de papel y de tinta

variable background_paper  \ Experimental!!!
variable about_ink
variable about_paper
variable command_prompt_ink
variable command_prompt_paper
variable debug_ink
variable debug_paper
variable description_ink
variable description_paper
variable error_ink
variable error_paper
variable input_ink
variable input_paper
variable location_description_ink
variable location_description_paper
variable location_name_ink
variable location_name_paper
variable narration_ink
variable narration_paper
variable scroll_prompt_ink
variable scroll_prompt_paper
variable question_ink
variable question_paper
variable scene_prompt_ink
variable scene_prompt_paper
variable speech_ink
variable speech_paper
variable narration_prompt_ink
variable narration_prompt_paper

: init_colors
  \ Asigna los colores predeterminados.
  [defined] background_paper [if]
    magenta background_paper !  \ Experimental!!!
  [then] 
  dark_gray about_ink !
  black about_paper !
  cyan command_prompt_ink !
  black command_prompt_paper !
  white debug_ink !
  red debug_paper !
  dark_gray description_ink !
  black description_paper !
  light_red error_ink !
  black error_paper !
  light_cyan input_ink !
  black input_paper !
  green location_description_ink !
  black location_description_paper !
  black location_name_ink !
  green location_name_paper !
  dark_gray narration_ink !
  black narration_paper !
  green scroll_prompt_ink !
  black scroll_prompt_paper !
  white question_ink !
  black question_paper !
  green scene_prompt_ink !
  black scene_prompt_paper !
  brown speech_ink !
  black speech_paper !
  green narration_prompt_ink !
  black narration_prompt_paper !
  ;

: about_color
  \ Pone el color de texto de los créditos.
  about_paper about_ink @colors 
  ;
: command_prompt_color
  \ Pone el color de texto del presto de entrada de comandos.
  command_prompt_paper command_prompt_ink @colors
  ;
: debug_color
  \ Pone el color de texto usado en los mensajes de depuración.
  debug_paper debug_ink @colors
  ;
: system_background_color
  \ Pone el color de papel predeterminado en el sistema.
  trm.background-default 1 sgr
  ;
: system_foreground_color
  \ Pone el color de texto predeterminado en el sistema.
  trm.foreground-default 1 sgr
  ;
: system_color
  \ Pone los colores predeterminados en el sistema.
  system_background_color system_foreground_color
  trm.reset 1 sgr 
  ;
: background_color
  \ Pone el color de fondo.
  [defined] background_paper
  [if]    background_paper @ paper
  [else]  system_background_color
  [then]
  ;
: description_color
  \ Pone el color de texto de las descripciones de los entes que no son escenarios.
  description_paper description_ink @colors
  ;
: error_color
  \ Pone el color de texto de los errores.
  error_paper error_ink @colors
  ;
: input_color
  \ Pone el color de texto para la entrada de comandos.
  input_paper input_ink @colors
  ;
: location_description_color
  \ Pone el color de texto de las descripciones de los entes escenario.
  location_description_paper location_description_ink @colors
  ;
: location_name_color
  \ Pone el color de texto del nombre de los escenarios.
  location_name_paper location_name_ink @colors
  ;
: narration_color
  \ Pone el color de texto de la narración.
  narration_paper narration_ink @colors
  ;
: scroll_prompt_color
  \ Pone el color de texto del presto de pantalla llena.
  scroll_prompt_paper scroll_prompt_ink @colors
  ;
: question_color
  \ Pone el color de texto de las preguntas de tipo «sí o no».
  question_paper question_ink @colors
  ;
: scene_prompt_color
  \ Pone el color de texto del presto de fin de escena.
  scene_prompt_paper scene_prompt_ink @colors
  ;
: speech_color
  \ Pone el color de texto de los diálogos.
  speech_paper speech_ink @colors
  ;
: narration_prompt_color
  \ Pone el color de texto del presto de pausa .
  narration_prompt_paper narration_prompt_ink @colors
  ;

\ }}}---------------------------------------------
subsection( Demo de colores)  \ {{{

\ Dos palabras para probar cómo se ven los colores

: color_bar  ( u -- )
  \ Imprime una barra de 64 espacios con el color indicado.
  paper cr 64 spaces  black paper space
  ;
: color_demo
  \ Prueba los colores.
  cr ." Colores descritos como se ven en Debian"
  black color_bar ." negro"
  dark_gray color_bar ." gris oscuro"
  gray color_bar ." gris"
  blue color_bar ." azul"
  light_blue color_bar ." azul claro"
  cyan color_bar ." cian"
  light_cyan color_bar ." cian claro"
  green color_bar ." verde"
  light_green color_bar ." verde claro"
  magenta color_bar ." magenta"
  light_magenta color_bar ." magenta claro"
  red color_bar ." rojo"
  light_red color_bar ." rojo claro"
  brown color_bar ." marrón"
  yellow color_bar ." amarillo"
  white color_bar ." blanco"
  cr
  red color_bar ." rojo"
  brown color_bar ." marrón"
  red color_bar ." rojo"
  brown color_bar ." marrón"
  system_color cr
  ;

\ }}}---------------------------------------------
subsection( Otros atributos tipográficos)  \ {{{

: bold
  \ Activa la negrita.
  trm.bold 1 sgr
  ;
: underline  ( ff -- )
  \ Activa o desactiva el subrayado.
  if  trm.underscore-on  else  trm.underline-off  then  1 sgr
  ;
' underline alias underscore
: inverse  ( ff -- )
  \ Activa o desactiva la inversión de colores (papel y tinta).
  if  trm.reverse-on  else  trm.reverse-off  then  1 sgr
  ;
true [if]  \ Pendiente!!!
: blink ( ff -- )
  \ Activa o desactiva el parpadeo.
  \ No funciona!!!
  if  trm.blink-on  else  trm.blink-off  then  1 sgr
  ;
[then]
: italic  ( ff -- )
  \ Activa o desactiva la cursiva.
  \ Nota: tiene el mismo efecto que 'inverse'.
  if  trm.italic-on  else  trm.italic-off  then  1 sgr
  ;

\ }}}---------------------------------------------
subsection( Cursor)  \ {{{

: last_col  ( -- u )  cols 1-  ;
: last_row  ( -- u )  rows 1-  ;

\ }}}---------------------------------------------
subsection( Borrado de pantalla)  \ {{{

: reset_scrolling
  \ Desactiva la definición de zona de pantalla como ventana.
  [char] r trm+do-csi0
  ;
[defined] background_paper [if]  \ Experimental!!!
: (color_background)  ( u -- )
  \ Colorea el fondo de la pantalla con el color indicado.
  \ No sirve de mucho colorear la pantalla, porque la edición de textos
  \ utiliza el color de fondo predeterminado del sistema, el negro,
  \ cuando se borra el texto que está siendo escrito.
  \ No se ha comprabado si en Windows ocurre lo mismo.
  \ No sirve de nada usar trm+set-default-attributes
  paper home
  [false] [if]  \ Por líneas, más lento
  rows 0 do
    i if  cr  then  [ cols ] literal spaces
  loop
  [else]  \ Pantalla entera, más rápido
    rows cols * spaces
  [then]
  ;
: color_background
  \ Colorea el fondo de la pantalla, si el color no es negativo.
  background_paper @ dup 0>=
  if  (color_background)  else  drop  then
  ;
[then]
: new_page
  \ Borra la pantalla y sitúa el cursor en su origen.
  [defined] background_paper
  [if]    color_background \ Experimental!!!
  [else]  trm+erase-display
  [then]  home
  ;

: clear_screen_for_location
  \ Restaura el color de tinta y borra la pantalla para cambiar de escenario.
  location_page? @ if  new_page  then
  ;
: init_screen
  \ Prepara la pantalla la primera vez.
  trm+reset system_color home
  init_colors
  ;

\ }}}
\ }}} ##########################################################
section( Depuración)  \ {{{

\ : ~~ postpone ~~ s" key drop" evaluate ; immediate
[false] [if]
: ~~
  \ Muestra la misma información que '~~' pero precedida por
  \ el nombre de la última palabra creada,
  \ que es precisamente en la que se está compilando '~~'.
  latest postpone literal postpone .name postpone ~~
  postpone key postpone drop
  ; immediate
[then]

: fatal_error  ( ff a u -- )
  \ Informa de un error y sale del sistema, si el indicador de error es distinto de cero.
  \ No se usa!!!
  \ ff = Indicador de error
  \ a u = Mensaje de error
  rot if  ." Error fatal: " type cr bye  else  2drop  then
  ;
: .stack
  \ Imprime el estado de la pila.
  [false] [if]  \ versión antigua!!!
    ." Pila" depth
    if  ." :" .s ." ( " depth . ." )"
    else  ."  vacía."
    then
  [else]  \ nueva versión
    depth if  cr ." Pila: " .s cr  then
  [then]
  ;
: .sb
  \ Imprime el estado del almacén circular de cadenas.
  ." Espacio para cadenas:" sb# ?
  ;
: .cursor
  \ Imprime las coordenadas del cursor. Antiguo!!!
  ;
: .system_status
  \ Muestra el estado del sistema.
  ( .sb ) .stack ( .cursor )
  ;
: .debug_message  ( a u -- )
  \ Imprime el mensaje del punto de chequeo, si no está vacío.
  dup if  cr type cr  else  2drop  then
  ;
: debug_pause
  \ Pausa tras mostrar la información de depuración.
  [debug_pause] [if]  depth if  wait  then  [then]
  ;
: debug  ( a u -- )
  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema.
  debug_color .debug_message .system_status debug_pause
  ;

\ }}} ##########################################################
section( Manipulación de textos)  \ {{{

str-create tmp_str  \ Cadena dinámica de texto temporal para usos variados

: str-get-last-char  ( a -- c )
  \ Devuelve el último carácter de una cadena dinámica.
  dup str-length@ 1- swap str-get-char 
  ;
: str-get-last-but-one-char  ( a -- c )
  \ Devuelve el penúltimo carácter de una cadena dinámica.
  dup str-length@ 2 - swap str-get-char 
  ;

' toupper alias ascii-char-uppercase 

: (^uppercase)  ( a u -- )
  \ Convierte en mayúsculas la primera letra de una cadena.
  \ Nota:
  \   Solo funciona con caracteres ASCII.
  \   Por tanto no funciona con caracteres UTF-8 de más de un octeto.
  \   Esto debe tenerse en cuenta, pues el código fuente y los textos están en UTF-8.
  if  dup c@ ascii-char-uppercase swap c!
  else  drop
  then
  ;
: ^uppercase  ( a1 u -- a2 u )
  \ Hace una copia de una cadena en el almacén circular y la devuelve con la primera letra en mayúscula.
  \ Nota: Se necesita para los casos en que no queremos
  \ modificar la cadena original.
  >sb 2dup (^uppercase)
  ;
: ?^uppercase  ( a1 u ff -- a1 u | a2 u )
  \ Hace una copia de una cadena en el almacén circular
  \ y la devuelve con la primera letra en mayúscula,
  \ dependiendo del valor de un indicador.
  \ No se usa!!!
  if  ^uppercase  then
  ;
: -punctuation  ( a u -- a u )
  \ Sustituye por espacios todos los signos de puntuación de una cadena.
  2dup bounds  ?do
    i c@ chr-punct? if  bl i c!  then
  loop
  ;
: tmp_str!  ( a u -- )
  \ Guarda una cadena en la cadena dinámica 'tmp_str'.
  tmp_str str-set
  ;
: tmp_str@  ( -- a u )
  \ Devuelve el contenido de cadena dinámica 'tmp_str'.
  tmp_str str-get
  ;
: sreplace  ( a1 u1 a2 u2 a3 u3 -- a4 u4 )
  \ Sustituye en una cadena todas las apariciones
  \ de una subcadena por otra subcadena.
  \ a1 u1 = Cadena en la que se realizarán los reemplazos
  \ a2 u2 = Subcadena buscada
  \ a3 u3 = Subcadena sustituta
  \ a4 u4 = Resultado
  2rot tmp_str!  tmp_str str-replace  tmp_str@
  ;
: *>verb_ending  ( a u ff -- )
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ a u = Texto
  \ ff = ¿Hay que poner los verbos en plural?
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" n"  else  s" "  then  s" *" sreplace 
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" n" rot and  s" *" sreplace 
  [then]
  ;
: *>plural_ending  ( a u ff -- )
  \ Cambia por «s» (terminación plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular los verbos de una frase.
  \ a u = Expresión
  \ ff = ¿Hay que poner los verbos en plural?
  \ No se usa!!!
  [false] [if]  \ Versión al estilo de BASIC:
    if  s" s"  else  s" "  then  s" *" sreplace 
  [else]  \ Versión sin estructuras condicionales, al estilo de Forth:
    s" s" rot and  s" *" sreplace 
  [then]
  ;
: char>string  ( c u -- a u )
  \ Crea una cadena repitiendo un carácter.
  \ c = Carácter
  \ u = Longitud de la cadena
  \ a = Dirección de la cadena
  dup sb_allocate swap 2dup 2>r  rot fill  2r>
  ;

: space+  ( a1 u1 -- a2 u2 )
  \ Añade un espacio al final de una cadena.
  s"  " s+
  ;
: period+  ( a1 u1 -- a2 u2 )
  \ Añade un punto al final de una cadena.
  s" ." s+
  ;
: comma+  ( a1 u1 -- a2 u2 )
  \ Añade una coma al final de una cadena.
  s" ," s+
  ;
: colon+  ( a1 u1 -- a2 u2 )
  \ Añade dos puntos al final de una cadena.
  s" :" s+
  ;
: hyphen+  ( a1 u1 -- a2 u2 )
  \ Añade un guion a una cadena.
  s" -" s+
  ;
: and&  ( a1 u1 -- a2 u2 )
  \ Añade una conjunción «y» al final de una cadena.
  \ No se usa!!!
  s" y" s&
  ;
: or&  ( a1 u1 -- a2 u2 )
  \ Añade una conjunción «o» al final de una cadena.
  \ No se usa!!!
  s" o" s&
  ;

\ }}} ##########################################################
section( Textos aleatorios)  \ {{{

(

Casi todas las palabras de esta sección devuelven una cadena
calculada al azar. Las restantes palabras son auxiliares.

Por convención, en el programa las palabras que devuelven
una cadena sin recibir parámetros en la pila tienen el signo
«$» al final de su nombre.  También por tanto las constantes
de cadena creadas con 'sconstant'.

)

: old_man$  ( -- a u )
  \ Devuelve una forma de llamar al líder de los refugiados.
  s{ s" hombre" s" viejo" s" anciano" }s
  ;
: with_him$  ( -- a u )
  \ Devuelve una variante de «consigo» o una cadena vacía.
  s{ "" s" consigo" s" encima" }s
  ;
: with_you$  ( -- a u )
  \ Devuelve «contigo» o una cadena vacía.
  s" contigo" s?
  ;
: carries$  ( -- a u )
  s{ s" tiene" s" lleva" }s
  ;
: you_carry$  ( -- a u )
  s{ s" tienes" s" llevas" }s
  ;
: ^you_carry$  ( -- a u )
  \ Devuelve una variante de «Llevas» (con la primera mayúscula).
  you_carry$ ^uppercase
  ;
: now$  ( -- a u )
  \ Devuelve una variante de «ahora» o una cadena vacía.
  s{ "" s" ahora" s" en este momento" s" en estos momentos" }s
  ;
: now_$  ( -- a u )
  \ Devuelve el resultado de 'now$' o una cadena vacía.
  \ Sirve como versión de 'now$' con mayor probabilidad devolver una cadena vacía.
  now$ s?
  ;
: here$  ( -- a u )
  \ Devuelve una variante de «aquí». 
  s{ s" por aquí" s" por este lugar" s" en este lugar" s" aquí" }s
  ;
: here|""$  ( -- a u | a 0 )
  \ Devuelve una variante de «aquí» o una cadena vacía.
  here$ s?
  ;
: now|here|""$  ( -- a u | a 0 )
  s{ now$ here|""$ }s
  ;
: only$  ( -- a u )
  \ Devuelve una variante de «solamente».
  s{ s" tan solo" s" solo" s" solamente" s" únicamente" }s
  ;
: ^only$  ( -- a u )
  \ Devuelve una variante de «Solamente» (con la primera mayúscula).
  \ Nota: no se puede calcular este texto a partir de la versión en minúsculas, porque el cambio entre minúsculas y mayúsculas no funciona con caracteres codificados en UTF-8 de más de un octeto.
  s{ s" Tan solo" s" Solo" s" Solamente" s" Únicamente" }s
  ;
: only_$  ( -- a u )
  \ Devuelve una variante de «solamente»
  \ o una cadena vacía.
  only$ s?
  ;
: ^only_$  ( -- a u )
  \ Devuelve una variante de «Solamente» (con la primera mayúscula)
  \ o u una cadena vacía.
  ^only$ s?
  ;
: again$  ( -- a u )
  s{ s" de nuevo" s" otra vez" s" otra vez más" s" una vez más" }s
  ;
: ^again$  ( -- a u )
  again$ ^uppercase
  ;
: again?$  ( -- a u )
  again$ s" ?" s+
  ;
: still$  ( -- a u )
  s{ s" aún" s" todavía" }s
  ;
: even$  ( -- a u )
  s{ s" aun" s" incluso" }s
  ;
: toward$  ( -- a u )
  s{ s" a" s" hacia" }s
  ;
: toward_the(f)$  ( -- a u )
  toward$ s" la" s&
  ;
: toward_the(m)$  ( -- a u )
  s{ s" al" s" hacia el" }s
  ;
: ^toward_the(m)$  ( -- a u )
  toward_the(m)$ ^uppercase
  ;
: from_the(m)$  ( -- a u )
  s{ s" desde el" s" procedente" s? s" del" s& }s
  ;
: to_go_back$  ( -- a u )
  s{ s" volver" s" regresar" }s
  ;
: remains$  ( -- a u )
  s{ s" resta" s" queda" }s
  ;
: possible1$  ( -- a u )
  \ Devuelve «posible» o una cadena vacía.
  s" posible" s?
  ;
: possible2$  ( -- a u )
  \ Devuelve «posibles» o una cadena vacía.
  s" posibles" s?
  ;
: all_your$  ( -- a u )
  \ Devuelve una variante de «todos tus».
  s{ s" todos tus" s" tus" }s
  ;
: ^all_your$  ( -- a u )
  \ Devuelve una variante de «Todos tus» (con la primera mayúscula).
  all_your$ ^uppercase 
  ;
: soldiers$  ( -- a u )
  \ Devuelve una variante de «soldados».
  s{ s" hombres" s" soldados" }s 
  ;
: your_soldiers$  ( -- a u )
  \ Devuelve una variante de "tus hombres".
  s" tus" soldiers$ s&
  ;
: ^your_soldiers$  ( -- a u )
  \ Devuelve una variante de "Tus hombres".
  your_soldiers$ ^uppercase
  ;
: officers$  ( -- a u )
  \ Devuelve una variante de «oficiales».
  s{ s" oficiales" s" mandos" }s 
  ;
: the_enemies$  ( -- a u )
  \ Devuelve una variante de «los enemigos».
  s{ s" los sajones"
  s{ s" las tropas" s" las huestes" }s
  s{ s" enemigas" s" sajonas" }s& }s
  ;
: the_enemy$  ( -- a u )
  \ Devuelve una variante de «el enemigo».
  s{ s" el enemigo"
  s{ s" la tropa" s" la hueste" }s
  s{ s" enemiga" s" sajona" }s& }s
  ;
: (the_enemy|enemies)  ( -- a u ff )
  \ Devuelve una variante de «el/los enemigo/s», y un indicador del número.
  \ a u = Cadena con el texto
  \ ff = ¿El texto está en plural?
  2 random dup if  the_enemies$  else  the_enemy$  then  rot
  ;
: the_enemy|enemies$  ( -- a u )
  \ Devuelve una variante de «el/los enemigo/s».
  (the_enemy|enemies) drop
  ;
: «de_el»>«del»  ( a1 u1 -- a1 u1 | a2 u2 )
  \ Remplaza las apariciones de «de el» en una cadena por «del».
  s" del " s" de el " sreplace
  ;
: of_the_enemy|enemies$  ( -- a u )
  \ Devuelve una variante de «del/de los enemigo/s».
  (the_enemy|enemies) >r
  s" de" 2swap s&
  r> 0= if  «de_el»>«del»  then
  ;
: ^the_enemy|enemies  ( -- a u ff )
  \ Devuelve una variante de «El/Los enemigo/s», y un indicador del número.
  \ a u = Cadena con el texto
  \ ff = ¿El texto está en plural?
  (the_enemy|enemies) >r  ^uppercase  r>
  ;
: of_your_ex_cloak$  ( -- a u )
  \ Devuelve un texto común a las descripciones de los restos de la capa.
  s{ "" s" que queda" s" que quedó" }s s" de" s&
  s{ s" lo" s" la" }s& s" que" s& s" antes" s?&
  s{ s" era" s" fue" s" fuera" }s&
  s{ s" tu" s" la" }s& s{ s" negra" s" oscura" }s?&
  s" capa" s& s" de lana" s?& period+
  ;
: but$  ( -- a u )
  s{ s" pero" s" mas" }s  
  ;
: ^but$  ( -- a u )
  but$ ^uppercase
  ;
: though$  ( -- a u )
  s{ s" si bien" but$ s" aunque" }s
  ;
: place$  ( -- a u )
  s{ s" sitio" s" lugar" }s
  ;
: cave$  ( -- a u )
  s{ s" cueva" s" caverna" s" gruta" }s
  ;
: home$  ( -- a u )
  s{ s" hogar" s" casa" }s
  ;
: sire,$  ( -- a u )
  s" Sire" s" Ulfius" s?& comma+
  ;
: my_name_is$  ( -- a u )
  s{ s" Me llamo" s" Mi nombre es" }s
  ;
: very$  ( -- a u )
  s{ s" muy" s" harto" }s \ añadir asaz!!!
  ;
: very_$  ( -- a u )
  \ Devuelve el resultado de very$ o una cadena vacía.
  very$ s?
  ;
: the_path$  ( -- a u )
  s{ s" el camino" s" la senda" s" el sendero" }s
  ;
: ^the_path$  ( -- a u )
  the_path$ ^uppercase
  ;
: a_path$  ( -- a u )
  s{ s" un camino" s" una senda" }s
  ;
: ^a_path$  ( -- a u )
  a_path$ ^uppercase
  ;
: pass$  ( -- a u )
  s{ s" paso" s" camino" }s
  ;
: the_pass$  ( -- a u )
  s" el" pass$ s&
  ;
: pass_way$  ( -- a u )
  \ Devuelve una variante de «pasaje».
  s{ s" paso" s" pasaje" }s
  ;
: a_pass_way$  ( -- a u )
  \ Devuelve una variante de «un pasaje».
  s" un" pass_way$ s&
  ;
: ^a_pass_way$  ( -- a u )
  \ Devuelve una variante de «Un pasaje» (con la primera mayúscula).
  a_pass_way$ ^uppercase
  ;
: the_pass_way$  ( -- a u )
  \ Devuelve una variante de «el pasaje».
  s" el" pass_way$ s&
  ;
: ^the_pass_way$  ( -- a u )
  \ Devuelve una variante de «El pasaje» (con la primera mayúscula).
  the_pass_way$ ^uppercase
  ;
: pass_ways$  ( -- a u )
  \ Devuelve una variante de «pasajes».
  pass_way$ s" s" s+
  ;
: ^pass_ways$  ( -- a u )
  \ Devuelve una variante de «Pasajes» (con la primera mayúscula).
  pass_ways$ ^uppercase
  ;
: surrounds$  ( -- a u )
  \ Comprobar traducción!!!
  s{ s" rodea" s" circunvala" s" cerca" s" circuye" s" da un rodeo a" }s
  ;
: leads$  ( -- a u )
  s{ s" lleva" s" conduce" }s
  ;
: (they)_lead$  ( -- a u )
  leads$ s" n" s+
  ;
: can_see$  ( -- a u )
  \ Devuelve una forma de decir «ves».
  s{ s" ves" s" se ve" s" puedes ver" }s
  ;
: ^can_see$  ( -- a u )
  \ Devuelve una forma de decir «ves», con la primera letra mayúscula.
  can_see$ ^uppercase
  ;
: cannot_see$  ( -- a u )
  \ Devuelve una forma de decir «no ves».
  s" no" can_see$ s&
  ;
: ^cannot_see$  ( -- a u )
  \ Devuelve una forma de decir «No ves».
  cannot_see$ ^uppercase
  ;
: can_glimpse$  ( -- a u )
  s{ s" vislumbras" s" se vislumbra" s" puedes vislumbrar"
  s" entrevés" s" se entrevé" s" puedes entrever"
  s" columbras" s" se columbra" s" puedes columbrar" }s
  ;
: ^can_glimpse$  ( -- a u )
  can_glimpse$ ^uppercase
  ;
: in_half-darkness_you_glimpse$  ( -- a u )
  \ Devuelve un texto usado en varias descripciones de las cuevas.
  s" En la" s{ s" semioscuridad," s" penumbra," }s& s? dup
  if  can_glimpse$  else  ^can_glimpse$  then  s&
  ;
: you_glimpse_the_cave$  ( -- a u)
  \ Devuelve un texto usado en varias descripciones de las cuevas.
  \ Pendiente!!! Distinguir la antorcha encendida.
  in_half-darkness_you_glimpse$ s" la continuación de la cueva." s&
  ;
: rimarkable$  ( -- a u )
  \ Devuelve una variante de «destacable».
  s{ s" destacable" s" que destacar"
  s" especial" s" de especial"
  s" de particular"
  s" peculiar" s" de peculiar" 
  s" que llame la atención" }s
  ;
: has_nothing$  ( -- a u )
  s" no tiene nada"
  ;
: is_normal$  ( -- a u )
  \ Devuelve una variante de «no tiene nada especial».
  has_nothing$ rimarkable$ s&
  ;
: ^is_normal$  ( -- a u )
  \ Devuelve una variante de «No tiene nada especial» (con la primera letra en mayúscula).
  is_normal$ ^uppercase
  ;
: over_there$  ( -- a u )
  s{ s" allí" s" allá" }s
  ;
: goes_down_into_the_deep$  ( -- a u )
  \ Devuelve una variante de «desciende a las profundidades».
  s{ s" desciende" toward$ s& s" se adentra en"
  s" conduce" toward$ s& s" baja" toward$ s& }s
  s" las profundidades" s&
  ;
: in_that_direction$  ( -- a u )
  \ Devuelve una variante de «en esa dirección».
  s{ s" en esa dirección" s{ s" por" s" hacia" }s over_there$ s& }s
  ;
: ^in_that_direction$  ( -- a u )
  \ Devuelve una variante de «En esa dirección».
  in_that_direction$ ^uppercase
  ;
: (uninteresting_direction_0)$  ( -- a u )
  \ Devuelve primera variante de «En esa dirección no hay nada especial».
  s{ s" Esa dirección" is_normal$ s&
  ^in_that_direction$ s" no hay nada" s& rimarkable$ s&
  ^in_that_direction$ cannot_see$ s& s" nada" s& rimarkable$ s&
  }s period+
  ;
: (uninteresting_direction_1)$  ( -- a u )
  \ Devuelve segunda variante de «En esa dirección no hay nada especial».
  s{ 
  ^is_normal$ s" esa dirección" s&
  ^cannot_see$ s" nada" s& rimarkable$ s& in_that_direction$ s&
  s" No hay nada" rimarkable$ s& in_that_direction$ s&
  }s period+
  ;
: uninteresting_direction$  ( -- a u )
  \ Devuelve una variante de «En esa dirección no hay nada especial».
  ['] (uninteresting_direction_0)$  
  ['] (uninteresting_direction_1)$  
  2 choose execute
  ;
s" de Westmorland" sconstant of_westmorland$
: the_village$  ( -- a u )
  s{ s" la villa" of_westmorland$ s?&
  s" Westmorland" }s
  ;
: ^the_village$  ( -- a u )
  the_village$ ^uppercase
  ;
: of_the_village$  ( -- a u )
  s" de" the_village$ s&
  ;
: (it)_blocks$  ( -- a u )
  s{ s" impide" s" bloquea" }s
  ;
: (they)_block$  ( -- a u )
  s{ s" impiden" s" bloquean" }s
  ;
: (rocks)_on_the_floor$  ( -- a u )
  \ Devuelve un texto sobre las rocas que ya han sido desmoronadas.
  s" yacen desmoronadas" s" a lo largo del pasaje" s?&
  ;
: (rocks)_clue$  ( -- a u )
  \ Devuelve una descripción de las rocas que sirve de pista.
  s" Son" s{ s" muchas" s" muy" s? s" numerosas" s& }s& comma+
  s" aunque no parecen demasiado pesadas y" s&
  s{ s" pueden verse" s" se ven" s" hay" }s s" algunos huecos" s&
  s" entre ellas" rnd2swap s& s&
  ;
: from_that_way$  ( - u )  \
  s" de" s{ s" esa dirección" s" allí" s" ahí" s" allá" }s&
  ;
: that_way$  ( -- a u )
  \ Devuelve una variante de «en esa dirección».
  s{ s" en esa dirección" s" por" s{ s" ahí" s" allí" s" allá" }s& }s
  ;
: ^that_way$  ( -- a u )
  \ Devuelve una variante de «En esa dirección» (con la primera letra mayúscula).
  that_way$ ^uppercase
  ;
: gets_wider$  ( -- a u )
  \ Devuelve una variante de «se ensancha».
  s{
  s" se" s{ s" ensancha" s" va ensanchando"
  s" va haciendo más ancho" s" hace más ancho"
  s" vuelve más ancho" s" va volviendo más ancho" }s&
  2dup 2dup 2dup \ Aumentar las probabilidades de la primera variante
  s{ s" ensánchase" s" hácese más ancho" s" vuélvese más ancho" }s
  }s
  ;
: (narrow)$  ( -- a u )
  s{ s" estrech" s" angost" }s
  ;
: narrow(f)$  ( -- a u )
  \ Devuelve una variante de «estrecha».
  (narrow)$ s" a" s+
  ;
: narrow(m)$  ( -- a u )
  \ Devuelve una variante de «estrecho».
  (narrow)$ s" o" s+
  ;
: narrow(mp)$  ( -- a u )
  \ Devuelve una variante de «estrechos».
  narrow(m)$ s" s" s+
  ;
: ^narrow(mp)$  ( -- a u )
  \ Devuelve una variante de «Estrechos» (con la primera mayúscula).
  narrow(mp)$  ^uppercase
  ;
: gets_narrower(f)$  ( -- a u )
  \ Devuelve una variante de «se hace más estrecha» (femenino).
  s{
  s" se" s{ s" estrecha" s" va estrechando" }s&
  2dup \ Aumentar las probabilidades de la primera variante
  s" se" s{ s" va haciendo más" s" hace más"
  s" vuelve más" s" va volviendo más" }s& narrow(f)$ s&
  2dup \ Aumentar las probabilidades de la segunda variante
  s{ s" estréchase" s{ s" hácese" s" vuélvese" }s s" más" s& narrow(f)$ s& }s
  }s
  ;
: goes_up$  ( -- a u )
  \ Devuelve una variante de «sube».
  s{ s" sube" s" asciende" }s
  ;
: (they)_go_up$  ( -- a u )
  \ Devuelve una variante de «suben».
  goes_up$ s" n" s+
  ;
: goes_down$  ( -- a u )
  \ Devuelve una variante de «baja».
  s{ s" baja" s" desciende" }s
  ;
: (they)_go_down$  ( -- a u )
  \ Devuelve una variante de «bajan».
  goes_down$ s" n" s+
  ;
: almost_invisible(plural)$  ( -- a u )
  \ Devuelve una variante de «casi imperceptibles».
  s" casi" s{ s" imperceptibles" s" invisibles" s" desapercibidos" }s
  \ Confirmar significados!!!
  ;
: ^a_narrow_pass_way$  ( -- a u )
  s" Un" narrow(m)$ pass_way$ rnd2swap s& s&
  ;
: beautiful(m)$  ( -- a u )
  s{ s" bonito" s" bello" s" hermoso" }s
  ;
: a_snake_blocks_the_way$  ( -- a u )
  s" Una serpiente"
  s{ s" bloquea" s" está bloqueando" }s&
  the_pass$ s& toward_the(m)$ s" Sur" s& s?&
  ;
: the_water_current$  ( -- a u )
  s" la" s{ s" caudalosa" s" furiosa" s" fuerte" s" brava" }s&
  s" corriente" s& s" de agua" s?&
  ;
: ^the_water_current$  ( -- a u )
  the_water_current$ ^uppercase
  ;
: comes_from$  ( -- a u )
  s{ s" viene" s" proviene" s" procede" }s
  ;
: to_keep_going$  ( -- a u )
  s{ s" avanzar" s" proseguir" s" continuar" }s
  ;
: lets_you$  ( -- a u )
  s" te" s? s" permite" s&
  ;
: narrow_cave_pass$  ( -- a u )
  \ Devuelve una variante de «estrecho tramo de cueva».
  s" tramo de cueva" narrow(m)$ rnd2swap s&
  ;
: a_narrow_cave_pass$  ( -- a u )
  \ Devuelve una variante de «un estrecho tramo de cueva».
  s" un" narrow_cave_pass$ s&
  ;
: but|and$  ( -- a u )
  s{ s" y" but$ }s
  ;
' but|and$ alias and|but$
: ^but|and$  ( -- a u )
  but|and$ ^uppercase
  ;
' ^but|and$ alias ^and|but$
: rocks$  ( -- a u )
  s{ s" piedras" s" rocas" }s
  ;
: wanted_peace$  ( -- a u )
  \ Texto «la paz», parte final de los mensajes «Queremos/Quieren la paz».
  s{  s" la" s" que haya"
      s" poder" s? s" vivir en" s&
      s{ s" tener" s" poder tener" s" poder disfrutar de" }s? s" una vida en" s&
      s" que" s{ s" reine" s" llegue" }s& s" la" s&
  }s s" paz." s&
  ;
: they_want_peace$  ( -- a u )
  \ Mensaje «quieren la paz».
  only$ s{ s" buscan" s" quieren" s" desean" s" anhelan" }s&
  wanted_peace$ s&
  ;
: we_want_peace$  ( -- a u )
  \ Mensaje «Queremos la paz».
  ^only$ s{ s" buscamos" s" queremos" s" deseamos" s" anhelamos" }s&
  wanted_peace$ s&
  ;
: to_understand$  ( -- a u )
  s{ s" comprender" s" entender" }s
  ;
: way$  ( -- a u )
  s{ s" manera" s" forma" }s
  ;
: to_realize$  ( -- a u )
  s{ s" ver" s" notar" s" advertir" s" apreciar" }s
  ;
: more_carefully$  ( -- a u )
  s{  s" mejor"
      s" con" s{ s" más" s" un" s? s" mayor" s& s" algo más de" }s&
        s{ s" detenimiento" s" cuidado" s" detalle" }s&
  }s
  ;
: finally$  ( -- a u )
  s{  s{ s" al" s" por" }s s" fin"
      s" finalmente"
  }s
  ;
: ^finally$  ( -- a u )
  finally$ ^uppercase
  ;
: rocky(f)$  ( -- a u )
  s{ s" rocosa" s" de roca" s" s" s?+ }s
  ;

\ }}} ##########################################################
section( Cadena dinámica para impresión)  \ {{{

(

Usamos una cadena dinámica llamada 'print_str' para guardar
los párrafos enteros que hay que mostrar en pantalla. En
esta sección creamos la cadena y palabras útiles para
manipularla.

)

str-create print_str  \ Cadena dinámica para almacenar el texto antes de imprimirlo justificado

: «»-clear
  \ Vacía la cadena dinámica 'print_str'.
  print_str str-clear
  ;
: «»!  ( a u -- )
  \ Guarda una cadena en la cadena dinámica 'print_str'.
  print_str str-set
  ;
: «»@  ( -- a u )
  \ Devuelve el contenido de la cadena dinámica 'print_str'.
  print_str str-get
  ;
: «+  ( a u -- )
  \ Añade una cadena al principio de la cadena dinámica 'print_str'.
  print_str str-prepend-string
  ;
: »+  ( a u -- )
  \ Añade una cadena al final de la cadena dinámica 'print_str'.
  print_str str-append-string
  ;
: «c+  ( c -- )
  \ Añade un carácter al principio de la cadena dinámica 'print_str'.
  print_str str-prepend-char
  ;
: »c+  ( c -- )
  \ Añade un carácter al final de la cadena dinámica 'print_str'.
  print_str str-append-char
  ;
: «»bl+?  ( u -- ff )
  \ ¿Se debe añadir un espacio al concatenar una cadena a la cadena dinámica 'print_str'?
  \ u = Longitud de la cadena que se pretende unir a la cadena dinámica 'print_str'
  0<> print_str str-length@ 0<> and
  ;
: »&  ( a u -- )
  \ Añade una cadena al final de la cadena dinámica 'print_str', con un espacio de separación.
  dup «»bl+? if  bl »c+  then  »+
  ;
: «&  ( a u -- )
  \ Añade una cadena al principio de la cadena dinámica 'print_str', con un espacio de separación.
  dup «»bl+? if  bl «c+  then  «+ 
  ;


\ }}} ##########################################################
section( Herramientas para sonido)  \ {{{

(

Las herramientas para proveer de sonido al juego están
apenas esbozadas aquí y de momento solo para Gforth.

La idea consiste en utilizar un reproductor externo que
acepte comandos y no muestre interfaz, como mocp para
GNU/Linux, que es el que usamos en las pruebas. Los comandos
para la consola del sistema operativo se pasan con la
palabra SYSTEM de Gforth.

)

: clear_sound_track
  \ Limpia la lista de sonidos. 
  s" mocp --clear" system
  ;
: add_sound_track  ( a u -- )
  \ Añade un fichero de sonido a la lista de sonidos. 
  s" mocp --add" 2swap s& system
  ;
: play_sound_track
  \ Inicia la reproducción de la lista de sonidos. 
  s" mocp --play" system
  ;
: stop_sound_track
  \ Detiene la reproducción de la lista de sonidos. 
  s" mocp --stop" system
  ;
: next_sound_track
  \ Salta al siguiente elemento de la lista de sonidos. 
  s" mocp --forward" system
  ;

\ }}} ##########################################################
section( Impresión de textos)  \ {{{

variable #lines  \ Número de línea del texto que se imprimirá
variable scroll  \ Indicador de que la impresión no debe parar

\ ------------------------------------------------
subsection( Presto de pausa en la impresión de párrafos)  \ {{{

svariable scroll_prompt  \ Guardará el presto de pausa
: scroll_prompt$  ( -- a u )
  \ Devuelve el presto de pausa.
  scroll_prompt count
  ;
1 value /scroll_prompt  \ Número de líneas de intervalo para mostrar un presto

: scroll_prompt_key
  \ Espera la pulsación de una tecla y actualiza con ella el estado del desplazamiento.
  key  bl =  scroll !
  ;
: .scroll_prompt
  \ Imprime el presto de pausa, espera una tecla y borra el presto.
  trm+save-cursor  scroll_prompt_color
  scroll_prompt$ type  scroll_prompt_key
  trm+erase-line  trm+restore-cursor
  ;
: (scroll_prompt?)  ( u -- ff )
  \ ¿Se necesita imprimir un presto para la línea actual?
  \ u = Línea actual del párrafo que se está imprimiendo
  \ Se tienen que cumplir dos condiciones:
  dup 1+ #lines @ <>  \ ¿Es distinta de la última?
  swap /scroll_prompt mod 0=  and  \ ¿Y el intervalo es correcto?
  ;
: scroll_prompt?  ( u -- ff )
  \ ¿Se necesita imprimir un presto para la línea actual?
  \ u = Línea actual del párrafo que se está imprimiendo
  \ Si el valor de 'scroll' es «verdadero», se devuelve «falso»;
  \ si no, se comprueban las otras condiciones.
  \ ." L#" dup . ." /" #lines @ . \ Depuración!!!
  scroll @ if  drop false  else  (scroll_prompt?)  then
  ;
: .scroll_prompt?  ( u -- )
  \ Imprime un presto y espera la pulsación de una tecla,
  \ si corresponde a la línea en curso.
  \ u = Línea actual del párrafo que se está imprimiendo
  scroll_prompt? if  .scroll_prompt  then
  ;

\ }}}---------------------------------------------
subsection( Impresión de párrafos ajustados)  \ {{{

\ Indentación de la primera línea de cada párrafo (en caracteres):
2 constant default_indentation  \ Predeterminada 
8 constant max_indentation  \ Máxima
variable /indentation  \ En curso

variable indent_first_line_too?  \ ¿Se indentará también la línea superior de la pantalla, si un párrafo empieza en ella?
: indentation?  ( -- ff )
  \ ¿Indentar la línea actual?
  row 0> indent_first_line_too? @ or
  ;
: (indent)
  \ Indenta.
  /indentation @ print_indentation
  ;
: indent
  \ Indenta si es necesario.
  indentation? if  (indent)  then
  ;
: cr+
  print_cr indent 
  ;
: paragraph  ( a u -- )
  \ Imprime un texto justificado como inicio de un párrafo.
  \ a u = Texto
  cr+ print
  ;
: |paragraph  ( a u xt -- )
  \ Imprime un texto justificado como inicio de un párrafo.
  \ Versión que no contamina el sangrado con el
  \ color de fondo del texto.
  \ a u = Texto
  \ xt = Dirección de ejecución de una palabra que cambia los atributos de impresión.
  background_color cr+ execute print
  ;

: report  ( a u -- )
  \ Imprime una cadena como un informe de error.
  ['] error_color |paragraph system_color
  ;
: narrate  ( a u -- )
  \ Imprime una cadena como una narración.
  ['] narration_color |paragraph system_color
  ;

\ }}}---------------------------------------------
subsection( Pausas y prestos en la narración)  \ {{{

variable indent_pause_prompts?  \ ¿Hay que indentar también los prestos?
: .prompt  ( a u -- )
  \ Imprime un presto.
  indent_pause_prompts? @ if  indent  then  cr type
  ;

dtm-create deadline  \ Variable para guardar el momento final de las pausas
: no_time_left?  ( -- ff )
  \ ¿Se acabó el tiempo?
  0 time&date  \ Fecha y hora actuales (más cero para los milisegundos)
  deadline dtm-compare  \ Comparar con el momento final (el resultado puede ser: -1, 0, 1)
  1 =  \ ¿Nos hemos pasado?
  ;
: no_key?  ( -- ff )
  \ ¿No hay una tecla pulsada?
  key? 0=
  ;
: seconds_wait ( u -- )
  \ Espera los segundos indicados, o hasta que se pulse una tecla.
  deadline dtm-init  \ Guardar la fecha y hora actuales como límite...
  s>d deadline dti-seconds+  \ ...y sumarle los segundos indicados
  begin  no_time_left? no_key? or  until
  begin  no_time_left? key? or  until
  ;
: wait  ( u -- )
  \ Hace una pausa.
  \ u = Segundos (o un número negativo para pausa sin fin hasta la pulsación de una tecla)
  dup 0< if  key 2drop  else  seconds_wait  then
  ;

variable narration_break_seconds  \ Segundos de espera en las pausas de la narración
svariable narration_prompt  \ Guardará el presto usado en las pausas de la narración
: narration_prompt$  ( -- a u )
  \ Devuelve el presto usado en las pausas de la narración.
  narration_prompt count
  ;
: .narration_prompt
  \ Imprime el presto de fin de escena.
  narration_prompt_color narration_prompt$ .prompt
  ;
: (narration_break)  ( n -- )
  \ Alto en la narración: Muestra un presto y hace una pausa .
  \ u = Segundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
  trm+save-cursor
  .narration_prompt wait
  trm+erase-line  trm+restore-cursor
  ;
: narration_break
  \ Alto en la narración, si es preciso.
  narration_break_seconds @ ?dup
  if  (narration_break)  then
  ;

variable scene_break_seconds  \ Segundos de espera en las pausas de final de escena
svariable scene_prompt  \ Guardará el presto de cambio de escena
: scene_prompt$  ( -- a u )
  \ Devuelve el presto de cambio de escena.
  scene_prompt count
  ;
: .scene_prompt
  \ Imprime el presto de fin de escena.
  scene_prompt_color scene_prompt$ .prompt
  ;
: (scene_break)  ( n -- )
  \ Final de escena: Muestra un presto y hace una pausa .
  \ n = Segundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
  trm+save-cursor
  .scene_prompt wait
  trm+erase-line  trm+restore-cursor
  scene_page? @ if  new_page  then
  ;
: scene_break
  \ Final de escena, si es preciso.
  scene_break_seconds @ ?dup
  if  (scene_break)  then
  ;

\ }}}---------------------------------------------
subsection( Impresión de citas de diálogos)  \ {{{

s" —" sconstant dash$  \ Raya (código Unicode 2014 en hexadecimal, 8212 en decimal)
s" «" sconstant lquote$ \ Comilla castellana de apertura
s" »" sconstant rquote$  \ Comilla castellana de cierre

: str-with-rquote-only?  ( a -- ff )
  \ ¿Hay en una cadena dinámica una comilla castellana de cierre pero no una de apertura?
  >r rquote$ 0 r@ str-find -1 >  \ ¿Hay una comilla de cierre en la cita?
  lquote$ 0 r> str-find -1 = and  \ ¿Y además falta la comilla de apertura? 
  ;
: str-with-period?  ( a -- ff )
  \ ¿Termina una cadena dinámica con un punto?
  \ Fallo!!! no se pone punto tras puntos suspensivos. Corregir!!! Pendiente!!!
  dup str-get-last-char [char] . =  \ ¿El último carácter es un punto?
  swap str-get-last-but-one-char [char] . <> and  \ ¿Y además el penúltimo no lo es? (para descartar que se trate de puntos suspensivos)
  ;
: str-prepend-quote  ( a -- )
  \ Añade a una cadena dinámica una comilla castellana de apertura.
  lquote$ rot str-prepend-string
  ;
: str-append-quote  ( a -- )
  \ Añade a una cadena dinámica una comilla castellana de cierre.
  rquote$ rot str-append-string
  ;
: str-add-quotes  ( a -- )
  \ Encierra una cadena dinámica entre comillas castellanas.
  dup str-append-quote str-prepend-quote
  ;
false [if]  \ Obsoleto!!!
: str-add-quotes-period  ( a -- )
  \ Encierra una cadena dinámica (que termina en punto) entre comillas castellanas
  dup str-pop-char drop  \ Eliminar el último carácter, el punto
  dup str-add-quotes  \ Añadir las comillas
  s" ." rot str-append-string  \ Añadir de nuevo el punto 
  ;
[then]
: (quotes+)
  \ Añade comillas castellanas a una cita de un diálogo en la cadena dinámica 'tmp_str'.
  tmp_str dup str-with-period?  \ ¿Termina con un punto?
  if    dup str-pop-char drop  \ Eliminarlo
  then  dup str-add-quotes s" ." rot str-append-string
  ;
: quotes+  ( a1 u1 -- a2 u2 )
  \ Añade comillas castellanas a una cita de un diálogo.
  tmp_str!  tmp_str str-with-rquote-only?
  if  \ Es una cita con aclaración final
    tmp_str str-prepend-quote  \ Añadir la comilla de apertura
  else  \ Es una cita sin aclaración, o con aclaración en medio
    (quotes+)
  then  tmp_str@
  ;
: hyphen+  ( a1 u1 -- a2 u2 )
  \ Añade la raya a una cita de un diálogo.
  dash$ 2swap s+ 
  ;
: quoted  ( a1 u1 -- a2 u2 )
  \ Pone comillas o raya a una cita de un diálogo.
  castilian_quotes? @ if  quotes+  else  hyphen+  then  
  ;
: speak  ( a u -- )
  \ Imprime una cita de un diálogo.
  quoted speech_color paragraph system_color
  ;

\ }}}
\ }}} ##########################################################
section( Definición de la ficha de un ente)  \ {{{

(

Denominamos «ente» a cualquier componente del mundo virtual
del juego que es manipulable por el programa.  «Entes» por
tanto son los objetos, manipulables o no por el jugador; los
personajes, interactivos o no; los lugares; y el propio
personaje protagonista. 

Cada ente tiene una ficha en la base de datos del juego.  La
base de datos es una zona de memoria dividida en partes
iguales, una para cada ficha. El identificador de cada ficha es
una palabra que al ejecutarse deja en la pila la dirección de
memoria donde se encuentra la ficha.

Los campos de la base de datos, como es habitual en Forth en
este tipo de estructuras, son palabras que suman el
desplazamiento adecuado a la dirección base de la ficha, que
reciben en la pila, apuntando así a la dirección de memoria que
contiene el campo correspondiente. 

A pesar de que Gforth dispone de palabras especializadas para
crear estructuras de datos de todo tipo, hemos optado por el
método más sencillo: usar '+field' y 'constant'.

El funcionamiento de '+field' es muy sencillo: Toma de la pila
dos valores: el inferior es el desplazamiento en octetos desde
el inicio del «registro», que en este programa denominamos
«ficha»; el superior es el número de octetos necesarios para
almacenar el campo a crear. Con ellos crea una palabra nueva
[cuyo nombre es tomado del flujo de entrada, es decir, es la
siguiente palabra en la línea] que será el identificador del
campo de datos; esta palabra, al ser creada, guardará en su
propio campo de datos el desplazamiento del campo de datos
desde el inicio de la ficha de datos, y cuando sea ejecutada lo
sumará al número de la parte superior de la pila, que deberá
ser la dirección en memoria de la ficha.

Salvo los campos buleanos, que ocupan un solo bitio gracias a
las palabras creadas para ello, todos los demás campos ocupan
una celda.  La «celda» es un concepto de ANS Forth: es la
unidad en que se mide el tamaño de cada elemento de la pila, y
capaz por tanto de contener una dirección de memoria.  En los
sistemas Forth de 8 o 16 bitios una celda equivale a un valor
de 16 bitios; en los sistemas Forth de 32 bitios, como Gforth,
una celda equivale a un valor de 32 bitios.

El contenido de un campo puede representar un número con o sin
signo, un indicador buleano o una dirección de memoria [de una
cadena de texto, de una palabra de Forth, de la ficha de otro
ente, de otra estructura de datos...].

Para facilitar la legibilidad, los nombres de los campos
empiezan con el signo de tilde, «~»; los que contienen datos
buleanos terminan con una interrogación, «?»;  los que
contienen direcciones de ejecución terminan con «_xt»; los que
contienen códigos de error terminan con «_error#».

)

0 \ Valor inicial de desplazamiento para el primer campo

cell +field ~name_str  \ Dirección de una cadena dinámica que contendrá el nombre del ente
cell +field ~init_xt  \ Dirección de ejecución de la palabra que inicializa las propiedades de un ente (experimental!!!)
cell +field ~description_xt  \ Dirección de ejecución de la palabra que describe el ente
cell +field ~direction  \ Desplazamiento del campo de dirección al que corresponde el ente (solo se usa en los entes que son direcciones)

\ Contadores
cell +field ~familiar  \ Contador de familiaridad (cuánto le es conocido el ente al protagonista)
cell +field ~times_open  \ Contador de veces que ha sido abierto. 
cell +field ~conversations  \ Contador para personajes: número de conversaciones tenidas con el protagonista
cell +field ~visits  \ Contador de visitas del protagonista a cada ente escenario (se incrementa al abandonar el escenario)

\ Errores específicos
cell +field ~break_error#  \ Identificador del error adecuado al intentar romper el ente (cero si no hay error); se usa para casos especiales; los errores apuntados por este campo no reciben parámetros salvo en 'what'
cell +field ~take_error#  \ Identificador del error adecuado al intentar tomar el ente (cero si no hay error); se usa para casos especiales; los errores apuntados por este campo no reciben parámetros salvo en 'what'

\ Entes relacionados
cell +field ~location  \ Identificador del ente en que está localizado (sea escenario, contenedor, personaje o «limbo»)
cell +field ~previous_location  \ Ídem para el ente que fue la localización antes del actual 
cell +field ~owner  \ Identificador del ente al que pertenece «legalmente» o «de hecho», independientemente de su localización.

\ Trama
cell +field ~location_enter_plot_xt  \ Dirección de ejecución de la palabra que se ocupa de la trama de entrada al escenario
cell +field ~location_exit_plot_xt  \ Dirección de ejecución de la palabra que se ocupa de la trama de salida del escenario

\ Salidas
cell +field ~north_exit  \ Ente de destino hacia el Norte
cell +field ~south_exit  \ Ente de destino hacia el Sur
cell +field ~east_exit  \ Ente de destino hacia el Este
cell +field ~west_exit  \ Ente de destino hacia el Oeste
cell +field ~up_exit  \ Ente de destino hacia arriba
cell +field ~down_exit  \ Ente de destino hacia abajo
cell +field ~out_exit  \ Ente de destino hacia fuera
cell +field ~in_exit  \ Ente de destino hacia dentro

\ Indicadores
bitfields
  bitfield: ~has_definite_article?  \ ¿El artículo de su nombre debe ser siempre el artículo definido?
  bitfield: ~has_feminine_name?  \ ¿El género gramatical de su nombre es femenino?
  bitfield: ~has_no_article?  \ ¿Su nombre no debe llevar artículo?
  bitfield: ~has_personal_name?  \ ¿Su nombre es un nombre propio?
  bitfield: ~has_plural_name?  \ ¿Su nombre es plural?
  bitfield: ~is_animal?  \ ¿Es animal? 
  bitfield: ~is_character?  \ ¿Es un personaje?
  bitfield: ~is_cloth?  \ ¿Es una prenda que puede ser puesta y quitada?
  bitfield: ~is_decoration?  \ ¿Forma parte de la decoración de su localización?
  bitfield: ~is_global_indoor?  \ ¿Es global (común) en los escenarios interiores? 
  bitfield: ~is_global_outdoor?  \ ¿Es global (común) en los escenarios al aire libre?
  bitfield: ~is_not_listed?  \ ¿No debe ser listado (entre los entes presentes o en inventario)?
  bitfield: ~is_human?  \ ¿Es humano? 
  bitfield: ~is_light?  \ ¿Es una fuente de luz que puede ser encendida?
  bitfield: ~is_lit?  \ ¿El ente, que es una fuente de luz que puede ser encendida, está encendido?
  bitfield: ~is_location?  \ ¿Es un escenario? 
  bitfield: ~is_indoor_location?  \ ¿Es un escenario interior (no exterior, al aire libre)?
  bitfield: ~is_open?  \ ¿Está abierto?
  bitfield: ~is_vegetal?  \ ¿Es vegetal?
  bitfield: ~is_worn?  \ ¿Siendo una prenda, está puesta? 
cell +field ~flags_0  \ Campo para albergar los indicadores anteriores

[false] [if]  \ Campos que aún no se usan!!!:

cell +field ~times_closed  \ Contador de veces que ha sido cerrado. 
cell +field ~desambiguation_xt  \ Dirección de ejecución de la palabra que desambigua e identifica el ente
cell +field ~stamina  \ Energía de los entes vivos

bitfield: ~is_lock?  \ ¿Está cerrado con llave? 
bitfield: ~is_openable?  \ ¿Es abrible? 
bitfield: ~is_lockable?  \ ¿Es cerrable con llave? 
bitfield: ~is_container?  \ ¿Es un contenedor?

[then]

constant /entity  \ Tamaño de cada ficha

\ }}} ##########################################################
section( Interfaz de campos)  \ {{{

(
  
Las palabras de esta sección facilitan la tarea de
interactuar con los campos de las fichas, evitando repetir
cálculos, escondiendo parte de los entresijos de las fichas
y haciendo el código más conciso, más fácil de modificar y
más legible.

Algunas de las palabras que definimos a continuación actúan
de forma análoga a los campos de las fichas de entes:
reciben en la pila el identificador de ente y devuelven en
ella un resultado. La diferencia es que es un resultado
calculado.

Otras actúan como procedimientos para realizar operaciones
frecuentes con los entes.

)

\ ------------------------------------------------
\ Herramientas para los campos de dirección

' ~north_exit alias ~first_exit  \ Primera salida definida en la ficha
' ~in_exit alias ~last_exit  \ Última salida definida en la ficha

\ Guardar el desplazamiento de cada campo de dirección respecto al primero de ellos:
0 ~first_exit constant first_exit>
0 ~last_exit constant last_exit>
0 ~north_exit constant north_exit>
0 ~south_exit constant south_exit>
0 ~east_exit constant east_exit>
0 ~west_exit constant west_exit>
0 ~up_exit constant up_exit>
0 ~down_exit constant down_exit>
0 ~out_exit constant out_exit>
0 ~in_exit constant in_exit>

\ Espacio en octetos ocupado por los campos de salidas:
last_exit> cell+ first_exit> - constant /exits
\ Número de salidas:
/exits cell / constant #exits 

\ Marcador para direcciones sin salida en un ente dirección:
0 constant no_exit

: exit?  ( a -- ff )
  \ ¿Está abierta una dirección de salida de un ente escenario?
  \ a = Contenido de un campo de salida de un ente (que será el ente de destino, o cero)
  no_exit <>
  ;

\ ------------------------------------------------
\ Interfaz básica para leer y modificar los campos

(

Las palabras que siguen permiten hacer las operaciones
básicas de obtención y modificación del contenido de los
campos. 

)

\ Obtener el contenido de los campos

: break_error#  ( a -- u )  ~break_error# @  ;
: conversations  ( a -- u )  ~conversations @  ;
: description_xt  ( a -- xt )  ~description_xt @  ;
: direction  ( a -- u )  ~direction @  ;
: familiar  ( a -- u )  ~familiar @  ;
: flags_0  ( a -- x )  ~flags_0 @  ;
: has_definite_article?  ( a -- ff )  ~has_definite_article? bit@  ;
: has_feminine_name?  ( a -- ff )  ~has_feminine_name? bit@  ;
: has_masculine_name?  ( a -- ff )  has_feminine_name? 0=  ;
: has_no_article?  ( a -- ff )  ~has_no_article? bit@  ;
: has_personal_name?  ( a -- ff )  ~has_personal_name? bit@  ;
: has_plural_name?  ( a -- ff )  ~has_plural_name? bit@  ;
: has_singular_name?  ( a -- ff )  has_plural_name? 0=  ;
: init_xt  ( a -- xt )  ~init_xt @  ;
: is_animal?  ( a -- ff )  ~is_animal? bit@  ;
: is_character?  ( a -- ff )  ~is_character? bit@  ;
: is_cloth?  ( a -- ff )  ~is_cloth? bit@  ;
: is_decoration?  ( a -- ff )  ~is_decoration? bit@  ;
: is_global_indoor?  ( a -- ff )  ~is_global_indoor? bit@  ;
: is_global_outdoor?  ( a -- ff )  ~is_global_outdoor? bit@  ;
: is_human?  ( a -- ff )  ~is_human? bit@  ;
: is_light?  ( a -- ff )  ~is_light? bit@  ;
: is_not_listed?  ( a -- ff )  ~is_not_listed? bit@  ;
: is_listed?  ( a -- ff )  is_not_listed? 0=  ;
: is_lit?  ( a -- ff )  ~is_lit? bit@  ;
: is_not_lit?  ( a -- ff )  is_lit? 0=  ;
: is_location?  ( a -- ff )  ~is_location? bit@  ;
: is_indoor_location?  ( a -- ff )  is_location? ~is_indoor_location? bit@ and  ;
: is_outdoor_location?  ( a -- ff )  is_indoor_location? 0=  ;
: is_open?  ( a -- ff )  ~is_open? bit@  ;
: is_closed?  ( a -- ff )  is_open? 0=  ;
: name_str  ( a1 -- a2 )  ~name_str @  ;
: times_open  ( a -- u )  ~times_open @  ;
: owner  ( a1 -- a2 )  ~owner @  ;
: is_vegetal?  ( a -- ff )  ~is_vegetal? bit@  ;
: is_worn?  ( a -- ff )  ~is_worn? bit@  ;
: location  ( a1 -- a2 )  ~location @  ;
: location_enter_plot_xt  ( a -- xt )  ~location_enter_plot_xt @  ;
: location_exit_plot_xt  ( a -- xt )  ~location_exit_plot_xt @  ;
: previous_location  ( a1 -- a2 )  ~previous_location @  ;
: take_error#  ( a -- u )  ~take_error# @  ;
: visits  ( a -- u )  ~visits @  ;

: north_exit  ( a1 -- a2 )  ~north_exit @  ;
: south_exit  ( a1 -- a2 )  ~south_exit @  ;
: east_exit  ( a1 -- a2 )  ~east_exit @  ;
: west_exit  ( a1 -- a2 )  ~west_exit @  ;
: up_exit  ( a1 -- a2 )  ~up_exit @  ;
: down_exit  ( a1 -- a2 )  ~down_exit @  ;
: out_exit  ( a1 -- a2 )  ~out_exit @  ;
: in_exit  ( a1 -- a2 )  ~in_exit @  ;

\ Modificar el contenido de los campos más habituales

: conversations++  ( a -- )  ~conversations ?++  ;
: familiar++  ( a -- )  ~familiar ?++  ;
: has_definite_article  ( a -- )  ~has_definite_article? bit_on  ;
: has_feminine_name  ( a -- )  ~has_feminine_name? bit_on  ;
: has_masculine_name  ( a -- )  ~has_feminine_name? bit_off  ;
: has_no_article  ( a -- )  ~has_no_article? bit_on  ;
: has_personal_name  ( a -- )  ~has_personal_name? bit_on  ;
: has_plural_name  ( a -- )  ~has_plural_name? bit_on  ;
: has_singular_name  ( a -- )  ~has_plural_name? bit_off  ;
: is_character  ( a -- )  ~is_character? bit_on  ;
: is_animal  ( a -- )  ~is_animal? bit_on  ;
: is_light  ( a -- )  ~is_light? bit_on  ;
: is_not_listed  ( a -- ff )  ~is_not_listed? bit_on  ;
: is_lit  ( a -- )  ~is_lit? bit_on  ;
: is_not_lit  ( a -- )  ~is_lit? bit_off  ;
: is_cloth  ( a -- )  ~is_cloth? bit_on  ;
: is_decoration  ( a -- )  ~is_decoration? bit_on  ;
: is_global_indoor  ( a -- )  ~is_global_indoor? bit_on  ;
: is_global_outdoor  ( a -- )  ~is_global_outdoor? bit_on  ;
: is_human  ( a -- )  ~is_human? bit_on  ;
: is_location  ( a -- )  ~is_location? bit_on  ;
: is_indoor_location  ( a -- )  dup is_location ~is_indoor_location? bit_on  ;
: is_outdoor_location  ( a -- )  dup is_location ~is_indoor_location? bit_off  ;
: is_open  ( a -- )  ~is_open? bit_on  ;
: is_closed  ( a -- )  ~is_open? bit_off  ;
: times_open++  ( a -- )  ~times_open ?++  ;
: is_worn  ( a -- )  ~is_worn? bit_on  ;
: is_not_worn  ( a -- )  ~is_worn? bit_off  ;
: visits++  ( a -- )  ~visits ?++  ;

\ ------------------------------------------------
\ Campos calculados o seudo-campos

(

Los seudo-campos devuelven un cálculo. Sirven para añadir
una capa adicional de abstracción y simplificar el código.

Por conveniencia, en el caso de algunos de los campos
binarios creamos también palabras para la propiedad
contraria.  Por ejemplo, en las fichas existe el campo
~IS_OPEN? para indicar si un ente está abierto, pero creamos
las palabras necesarias para examinar y modificar tanto la
propiedad de «cerrado» como la de «abierto». Esto ayuda a
escribir posteriormente el código efectivo [pues no hace
falta recordar si la propiedad real y por tanto el campo de
la ficha del ente era «abierto» o «cerrado»] y hace el
código más conciso y legible.

)

: is_direction?  ( a -- ff )  direction 0<>  ;
: is_familiar?  ( a -- ff )  familiar 0>  ;
: is_visited?  ( a -- ff )  visits 0>  ;
: is_not_visited?  ( a -- ff )  visits 0=  ;
: conversations?  ( a -- ff )  conversations 0<>  ;
: no_conversations?  ( a -- ff )  conversations 0=  ;
: has_north_exit?  ( a -- ff )  north_exit exit?  ;
: has_east_exit?  ( a -- ff )  east_exit exit?  ;
: has_south_exit?  ( a -- ff )  south_exit exit?  ;

: owns?  ( a1 a2 -- ff )  owner =  ;
: belongs?  ( a1 a2 -- ff )  swap owns?  ;
: owns  ( a1 a2 -- )  ~owner !  ;
: belongs  ( a1 a2 -- )  swap owns  ;

: belongs_to_protagonist?  ( a -- ff )  owner protagonist% =  ;
: belongs_to_protagonist  ( a -- )  ~owner protagonist% swap !  ;

: is_living_being?  ( a -- ff )
  \ ¿El ente es un ser vivo (aunque esté muerto)?
  dup is_vegetal?  over is_animal? or  swap is_human? or
  ;
: is_there  ( a1 a2 -- )
  \ Hace que un ente sea la localización de otro.
  \ a1 = Ente que será la localización de a2
  \ a2 = Ente cuya localización será a1
  ~location !
  ;
: was_there  ( a1 a2 -- )
  \ Hace que un ente sea la localización previa de otro.
  \ a1 = Ente que será la localización previa de a2
  \ a2 = Ente cuya localización previa será a1
  ~previous_location !
  ;
: is_there?  ( a1 a2 -- ff )
  \ ¿Está un ente localizado en otro?
  \ a1 = Ente que actúa de localización
  \ a2 = Ente cuya localización se comprueba
  location =
  ;
: was_there?  ( a1 a2 -- ff )
  \ ¿Estuvo un ente localizado en otro?
  \ a1 = Ente que actúa de localización
  \ a2 = Ente cuya localización se comprueba
  previous_location =
  ;
: is_global?  ( a -- ff )
  \ ¿Es el ente un ente global?
  dup is_global_outdoor?
  swap is_global_indoor? or
  ;
: my_location  ( -- a )
  \ Devuelve la localización del protagonista.
  protagonist% location
  ;
: my_previous_location  ( -- a )
  \ Devuelve la localización anterior del protagonista.
  protagonist% previous_location
  ;
: my_location!  ( a -- )
  \ Mueve el protagonista al ente indicado.
  protagonist% is_there
  ;
: am_i_there?  ( a -- ff )
  \ ¿Está el protagonista en la localización indicada?
  \ a = Ente que actúa de localización
  my_location =
  ;
: is_outdoor_location?  ( a -- ff )
  \ ¿Es el ente un escenario al aire libre?
  \ Cálculo provisional!!!
  drop 0
  ;
: is_indoor_location?  ( a -- ff )
  \ ¿Es el ente un escenario cerrado, no al aire libre?
  is_outdoor_location? 0=
  ;
: am_i_outdoor?  ( -- ff )
  \ ¿Está el protagonista en un escenario al aire libre?
  my_location is_outdoor_location?
  ;
: am_i_indoor?  ( -- ff )
  \ ¿Está el protagonista en un escenario cerrado, no al aire libre?
  am_i_outdoor? 0=
  ;
: is_hold?  ( a -- ff )
  \ ¿Es el protagonista la localización de un ente?
  location protagonist% =
  ;
: is_hold  ( a -- )
  \ Hace que el protagonista sea la localización de un ente.
  ~location protagonist% swap !
  ;
: is_worn_by_me?  ( a -- )
  \ ¿El protagonista lleva puesto el ente indicado?
  dup is_hold?  swap is_worn?  and
  ;
: is_known?  ( a -- ff )
  \ ¿El protagonista ya conoce el ente?
  dup belongs_to_protagonist?  \ ¿Es propiedad del protagonista?
  over is_visited? or  \ ¿O es un escenario ya visitado? (si no es un escenario, la comprobación no tendrá efecto)
  over conversations? or  \ ¿O ha hablado ya con él? (si no es un personaje, la comprobación no tendrá efecto)
  swap is_familiar?  or  \ ¿O ya le es familiar?
  ;
: is_unknown?  ( a -- ff )
  \ ¿El protagonista aún no conoce el ente?
  is_known? 0=
  ;
: is_here?  ( a -- ff )
  \ ¿Está un ente en la misma localización que el protagonista?
  \ El resultado depende de cualquiera de tres condiciones:
  dup location am_i_there?  \ ¿Está efectivamente en la misma localización?
  over is_global_outdoor? am_i_outdoor? and or \ ¿O es un «global exterior» y estamos en un escenario exterior?
  swap is_global_indoor? am_i_indoor? and or  \ ¿O es un «global interior» y estamos en un escenario interior?
  ;
: is_not_here?  ( a -- ff )
  \ ¿Está un ente en otra localización que la del protagonista?
  \ No se usa!!!
  is_here? 0=
  ;
: is_here_and_unknown?  ( a -- ff )
  \ ¿Está un ente en la misma localización que el protagonista y aún no es conocido por él?
  dup is_here? swap is_unknown? and
  ;
: is_here  ( a -- )
  \ Hace que un ente esté en la misma localización que el protagonista.
  my_location swap is_there
  ;
: is_accessible?  ( a -- ff )
  \ ¿Es un ente accesible para el protagonista?
  dup is_hold?  swap is_here?  or
  ;
: is_not_accessible?  ( a -- ff )
  \ ¿Un ente no es accesible para el protagonista?
  is_accessible? 0=
  ;
: can_be_looked_at?  ( a -- ff )
  \ ¿El ente puede ser mirado?
  [false] [if]  \ Primera versión
    dup my_location =  \ ¿Es la localización del protagonista?
    over is_direction? or  \ ¿O es un ente dirección?
    over exits% = or  \ ¿O es el ente "salidas"?
    swap is_accessible? or  \ ¿O está accesible? 
  [else]  \ Segunda versión, menos elegante pero más rápida y legible
    { entity }  \ Variable local creada con el parámetro de la pila
    true case
      entity my_location = of  true  endof \ ¿Es la localización del protagonista?
      entity is_direction? of  true  endof \ ¿Es un ente dirección?
      entity is_accessible? of  true  endof  \ ¿Está accesible?
      entity exits% = of  true  endof  \ ¿Es el ente "salidas"?
      false swap
    endcase
  [then]
  ;
: can_be_taken?  ( a -- ff )
  \ ¿El ente puede ser tomado?
  \ Se usa como norma general, para aquellos entes
  \ que no tienen un error específico indicado en el campo '~take_error#'
  dup is_decoration?
  over is_human? or
  swap is_character? or 0=
  ;
: may_be_climbed?  ( a -- ff )
  \ ¿El ente podría ser escalado? (Aunque en la práctica no sea posible).
  \ Inacabado!!! Hacerlo mejor con un indicador en la ficha.
  [false] [if]
  fallen_away%
  bridge%
  arch%
  bed%
  flags%
  rocks%
  table%
  [then]
  ;
: talked_to_the_leader?  ( -- ff )
  \ ¿El protagonista ha hablado con el líder?
  leader% conversations 0<>
  ;
: do_you_hold_something_forbidden?  ( -- ff )
  \ ¿Llevas algo prohibido?
  \ Cálculo usado en varios lugares del programa,
  \ en relación a los refugiados.
  sword% is_accessible?
  stone% is_accessible? or
  ;
: no_torch?  ( -- ff )
  \ ¿La antorcha no está accesible y encendida?
  torch% is_not_accessible?  torch% is_not_lit?  or
  ;

\ ------------------------------------------------
\ Herramientas de artículos y pronombres

(

La selección del artículo adecuado para el nombre de un ente
tiene su complicación. Depende por supuesto del número y
género gramatical del nombre, pero también de la relación
con el protagonista [distinción entre artículos definidos e
indefinidos] y de la naturaleza del ente [cosa o personaje].

Por conveniencia, consideramos como artículos ciertas
palabras que son adjetivos [como «esta», «ninguna»...], pues
en la práctica para el programa su manejo es idéntico: se
usan para preceder a los nombres bajo ciertas condiciones.

En este mismo apartado definimos palabras para calcular
los pronombres de objeto indirecto [le/s] y de objeto
directo [la/s, lo/s], así como terminaciones habituales.

)

false [if]  \ Primera versión

(

Esta primera versión utiliza cadenas de longitud fija.
No está totalmente depurada.

Funciona en lina, a pesar de que los cálculos se
hacen para una longitud de nueve octetos por cadena,
cuando en realidad son diez, pues la palabra S, guarda
también la longitud.

En Gforth el cálculo no está bien y los artículos que se
obtienen son incorrectos.

En 2012-01-30 se sustituyó esta versión por otra más
sencilla.

)

create 'articles  \ Tabla de artículos
  \ Indefinidos:
  s" un       " s,
  s" una      " s,
  s" unos     " s,
  s" unas     " s,
  \ Definidos:
  s" el       " s,
  s" la       " s,
  s" los      " s,
  s" las      " s,
  \ Posesivos:
  s" tu       " s,
  s" tu       " s,
  s" tus      " s,
  s" tus      " s,
  \ Adjetivos que se tratan como «artículos negativos»:
  s" ningXn   " s,  \ La «X» evita el problema del número de caracteres en UTF-8 y será sustituida después por «ú»
  s" ninguna  " s,
  s" ningunos " s,
  s" ningunas " s,
  \ Adjetivos que se tratan como «artículos distantes»:
  s" ese      " s,
  s" esa      " s,
  s" esos     " s,
  s" esas     " s,
  \ Adjetivos que se tratan como «artículos cercanos»:
  s" este     " s,
  s" esta     " s,
  s" estos    " s,
  s" estas    " s,
9 constant /article  \ Longitud máxima de un artículo en la tabla, con sus espacios finales
1 /article * constant /article_gender_set  \ Separación entre cada grupo según el género (masculino y femenino)
2 /article * constant /article_number_set  \ Separación entre cada grupo según el número (singular y plural)
4 /article * constant /article_type_set  \ Separación entre cada grupo según el tipo (definidos, indefinidos, posesivos y negativos)

: article_number>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el número gramatical del ente.
  has_plural_name? /article_number_set and
  ;
: article_gender>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical del ente.
  has_feminine_name? /article_gender_set and
  ;
: article_gender+number>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical y el número del ente.
  dup article_gender> 
  swap article_number> +
  ;
: definite_article>  ( a -- 0 | 1 )
  \ Devuelve el desplazamiento (en número de grupos)
  \ para apuntar a los artículos definidos de la tabla,
  \ si el ente indicado necesita uno.
  dup has_definite_article?  \ Si el ente necesita siempre artículo definido
  swap is_known? or abs  \ O bien si el ente es ya conocido por el protagonista
  ;
: possesive_article>  ( a -- 0 | 2 )
  \ Devuelve el desplazamiento (en número de grupos)
  \ para apuntar a los artículos posesivos de la tabla,
  \ si el ente indicado necesita uno.
  belongs_to_protagonist? 2 and
  ;
: negative_articles>  ( -- u )
  \ Devuelve el desplazamiento (en número de caracteres)
  \ para apuntar a los «artículos negativos» de la tabla.
  /article_type_set 3 *
  ;
: undefined_articles>  ( -- u )
  \ Devuelve el desplazamiento (en número de caracteres)
  \ para apuntar a los artículos indefinidos de la tabla.
  0
  ;
: definite_articles>  ( -- u )
  \ Devuelve el desplazamiento (en número de caracteres)
  \ para apuntar a los artículos definidos de la tabla.
  /article_type_set 
  ;
: distant_articles>  ( -- u )
  \ Devuelve el desplazamiento (en número de caracteres)
  \ para apuntar a los «artículos distantes» de la tabla.
  /article_type_set 4 *
  ;
: not_distant_articles>  ( -- u )
  \ Devuelve el desplazamiento (en número de caracteres)
  \ para apuntar a los «artículos cercanos» de la tabla.
  /article_type_set 5 *
  ;
: article_type  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el ente requiera un artículo definido, indefinido o posesivo.
  dup definite_article>  swap possesive_article>  max
  /article_type_set *
  ;
: >article  ( u -- a1 u1 )
  \ Devuelve un artículo de la tabla de artículos
  \ a partir de su índice.
  'articles + /article -trailing
  ;

[else]  \ Segunda versión, mejorada

(

Esta segunda versión utiliza una tabla de cadenas de
longitud variable, apuntada por una segunda tabla con sus
direcciones.  Esto unifica y simplifica los cálculos.

)

: hs,  ( a u -- a1 )
  \ Compila una cadena en el diccionario y devuelve su dirección.
  here rot rot s,
  ;

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

\ Separaciones entre artículos en la tabla índice (por tanto en celdas)
cell constant /article_gender_set  \ De femenino a masculino
2 cells constant /article_number_set  \ De plural a singular
4 cells constant /article_type_set  \ Entre grupos de diferente tipo

: article_number>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el número gramatical del ente.
  has_singular_name? /article_number_set and
  ;
: article_gender>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical del ente.
  has_masculine_name? /article_gender_set and
  ;
: article_gender+number>  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el género gramatical y el número del ente.
  dup article_gender> 
  swap article_number> +
  ;
: definite_article>  ( a -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos
  \ si el ente indicado necesita uno.
  dup has_definite_article?  \ Si el ente necesita siempre artículo definido
  swap is_known? or  \ O bien si el ente es ya conocido por el protagonista
  abs  \ Un grupo (pues los definidos son el segundo)
  /article_type_set *
  ;
: possesive_article>  ( a -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos posesivos 
  \ si el ente indicado necesita uno.
  belongs_to_protagonist? 2 and  \ Dos grupos (pues los posesivos son el tercero)
  /article_type_set *
  ;
: negative_articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos negativos». 
  3 /article_type_set *  \ Tres grupos (pues los negativos son el cuarto)
  ;
: undefined_articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos indefinidos. 
  0  \ Desplazamiento cero, pues los indefinidos son el primer grupo.
  ;
: definite_articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los artículos definidos.
  /article_type_set  \ Un grupo, pues los definidos son el segundo
  ;
: distant_articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos distantes».
  4 /article_type_set *  \ Cuatro grupos, pues los «distantes» son el quinto
  ;
: not_distant_articles>  ( -- u )
  \ Devuelve el desplazamiento en la tabla de artículos
  \ para apuntar a los «artículos cercanos». 
  5 /article_type_set *  \ Cinco grupos, pues los «cercanos» son el sexto
  ;
: article_type  ( a -- u )
  \ Devuelve un desplazamiento en la tabla de artículos
  \ según el ente requiera un artículo definido, indefinido o posesivo.
  dup definite_article>  swap possesive_article>  max
  ;
: >article  ( u -- a1 u1 )
  \ Devuelve un artículo de la tabla de artículos
  \ a partir de su índice.
  'articles + @ count
  ;

[then]

: (article)  ( a -- a1 u1 )
  \ Devuelve el artículo apropiado para un ente.
  dup article_gender>  \ Desplazamiento según el género
  over article_number> +  \ Sumado al desplazamiento según el número
  swap article_type +  \ Sumado al desplazamiento según el tipo
  >article
  ;
: article  ( a -- a1 u1 | a 0 )
  \ Devuelve el artículo apropiado para un ente, si lo necesita;
  \ en caso contrario devuelve una cadena vacía.
  dup has_no_article? if  0  else  (article)  then
  ;
: undefined_article  ( a -- a1 u1 )
  \ Devuelve el artículo indefinido
  \ correspondiente al género y número de un ente.
  article_gender+number> undefined_articles> +
  >article
  ;
: definite_article  ( a -- a1 u1 )
  \ Devuelve el artículo definido
  \ correspondiente al género y número de un ente.
  article_gender+number> definite_articles> +
  >article
  ;
: pronoun  ( a -- a1 u1 )
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente.
  definite_article  s" lo" s" el" sreplace
  ;
: ^pronoun  ( a -- a1 u1 )
  \ Devuelve el pronombre
  \ correspondiente al género y número de un ente,
  \ con la primera letra mayúscula.
  pronoun ^uppercase
  ;
: negative_article  ( a -- a1 u1 )
  \ Devuelve el «artículo negativo»
  \ correspondiente al género y número de un ente.
  article_gender+number> negative_articles> +  >article
  [false] [if]
    \ Obsoleto!!!
    \ Esto era necesario en la versión antigua de la tabla de artículos
    \ (con longitudes fijas) para evitar el carácter de dos octetos en UTF-8:
    s" ú" s" X" sreplace
  [then]
  ;
: distant_article  ( a -- a1 u1 )
  \ Devuelve el «artículo distante»
  \ correspondiente al género y número de un ente.
  article_gender+number> distant_articles> +
  >article
  ;
: not_distant_article  ( a -- a1 u1 )
  \ Devuelve el «artículo cercano»
  \ correspondiente al género y número de un ente.
  article_gender+number> not_distant_articles> +
  >article
  ;
: plural_ending  ( a -- a1 u1 )
  \ Devuelve la terminación adecuada del plural para el nombre de un ente.
  [false] [if]
    \ Método 1, «estilo BASIC»:
    has_plural_name? if  s" s"  else  ""  then
  [else]
    \ Método 2, sin estructuras condicionales, «estilo Forth»:
    s" s" rot has_plural_name? and
  [then]
  ;
: gender_ending  ( a -- a1 u1 )
  \ Devuelve la terminación adecuada del género gramatical
  \ para el nombre de un ente.
  [false] [if]
    \ Método 1, «estilo BASIC»
    has_feminine_name? if  s" a"  else  s" o"  then
  [else]
    [false] [if]
      \ Método 2, sin estructuras condicionales, «estilo Forth»
      s" oa" drop swap has_feminine_name? abs + 1
    [else]
      \ Método 3, similar, más directo
      c" oa" swap has_feminine_name? abs + 1+ 1
    [then]
  [then]
  ;
: gender_ending+  ( a1 u1 a -- a2 u2 )
  \ Añade a una cadena la terminación adecuada para el género gramatical de un ente.
  gender_ending s+
  ;
: noun_ending  ( a -- a1 u1 )
  \ Devuelve la terminación adecuada para el nombre de un ente.
  dup gender_ending rot plural_ending s+
  ;
' noun_ending alias adjective_ending
: noun_ending+  ( a1 u1 a -- a2 u2 )
  \ Añade a una cadena la terminación adecuada para el nombre de un ente.
  noun_ending s+
  ;
' noun_ending+ alias adjective_ending+
: direct_pronoun  ( a -- a1 u1 )
  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»).
  s" l" rot noun_ending s+
  ;
: indirect_pronoun  ( a -- a1 u1 )
  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»).
  s" le" rot plural_ending s+
  ;
: verb_number_ending  ( a -- a1 u1 )
  \ Devuelve la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.
  s" n" rot has_plural_name? and
  ;
: verb_number_ending+  ( a1 u1 a -- a2 u2 )
  \ Añade a una cadena la terminación verbal adecuada
  \ (singular o plural: una cadena vacía o «n» respectivamente)
  \ para el sujeto cuyo ente se indica.
  verb_number_ending s+
  ;
: proper_verb_form  ( a a1 u1 -- a2 u2 )
  \ Cambia por «n» (terminación verbal en plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular un verbo 
  \ cuyo sujeto se indica con el identificador de su entidad.
  \ a u = Expresión
  \ a = Entidad
  \ No se usa!!!
  rot has_plural_name? *>verb_ending
  ;
: proper_grammar_number  ( a a1 u1 -- a2 u2 )
  \ Cambia por «s» (terminación del plural)
  \ los asteriscos de un texto, o los quita.
  \ Se usa para convertir en plural o singular las palabras de un texto,
  \ cuyo número gramatical se indica con el identificador de una entidad.
  \ a u = Expresión
  \ a = Entidad
  \ No se usa!!!
  rot has_plural_name? *>plural_ending
  ;

\ ------------------------------------------------
\ Interfaz para los nombres de los entes

(

Como ya se explicó, el nombre de cada ente se guarda en una
cadena dinámica [que se crea en la memoria con 'allocate', no
en el espacio del diccionario del sistema].  El manejo de
estas cadenas dinámicas se hace con el módulo
correspondiente de Forth Foundation Library.

En la ficha del ente se guarda solo la dirección de la
cadena dinámica, en el campo '~name_str'.  Por ello hacen
falta palabras que hagan de interfaz para gestionar los
nombres de ente de forma análoga a como se hace con el resto
de datos de su ficha.

)

: name!  ( a u a1 -- )
  \ Guarda el nombre de un ente.
  \ a u = Nombre
  \ a1 = Ente
  name_str str-set
  ;
: names!  ( a u a1 -- )
  \ Guarda el nombre de un ente, y lo marca como plural.
  \ a u = Nombre
  \ a1 = Ente
  dup has_plural_name  name!
  ;
: fname!  ( a u a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical femenino.
  \ a u = Nombre
  \ a1 = Ente
  dup has_feminine_name
  dup has_singular_name  name!
  ;
: fnames!  ( a u a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical femenino y plural.
  \ a u = Nombre
  \ a1 = Ente
  dup has_feminine_name  
  dup has_plural_name  name!
  ;
: mname!  ( a u a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical masculino. 
  \ a u = Nombre
  \ a1 = Ente
  dup has_masculine_name
  dup has_singular_name  name!
  ;
: mnames!  ( a u a1 -- )
  \ Guarda el nombre de un ente,
  \ indicando también que es de género gramatical masculino y plural.
  \ a u = Nombre
  \ a1 = Ente
  dup has_masculine_name
  dup has_plural_name  name!
  ;
: name  ( a -- a1 u1 )
  \ Devuelve el nombre de un ente.
  \ a = Ente
  \ a1 u1 = Nombre
  name_str str-get
  ;
: ^name  ( a -- a1 u1 )
  \ Devuelve el nombre de un ente, con la primera letra mayúscula.
  name ^uppercase
  ;
: name&  ( a a1 u1 -- a2 u2 )
  \ Añade a un (supuesto) artículo el nombre de un ente.
  \ a = Ente
  \ a1 u1 = Artículo correspondiente (o cualquier otro texto)
  \ a2 u2 = Nombre completo
  rot name s& 
  ;
: full_name  ( a -- a1 u1 )
  \ Devuelve el nombre completo de un ente, con el artículo que le corresponda.
  dup article name& 
  ;
: ^full_name  ( a -- a1 u1 )
  \ Devuelve el nombre completo de un ente, con el artículo que le corresponda (con la primera letra en mayúscula).
  full_name ^uppercase
  ;
: defined_full_name  ( a -- a1 u1 )
  \ Devuelve el nombre completo de un ente, con un artículo definido.
  dup definite_article name&
  ;
: undefined_full_name  ( a -- a1 u1 )
  \ Devuelve el nombre completo de un ente, con un artículo indefinido.
  dup undefined_article name&
  ;
: negative_full_name  ( a -- a1 u1 )
  \ Devuelve el nombre completo de un ente, con un «artículo negativo».
  dup negative_article name&
  ;
: distant_full_name  ( a -- a1 u1 )
  \ Devuelve el nombre completo de un ente, con un «artículo distante».
  dup distant_article name&
  ;
: nonhuman_subjective_negative_name  ( a -- a1 u1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente (no humano), desde el punto de vista del protagonista.
  \ Nota: En este caso hay que usar 'negative_full_name' antes de 's{' y pasar la cadena
  \ mediante la pila de retorno; de otro modo 's{' y '}s' no pueden calcular bien
  \ el crecimiento de la pila.
  negative_full_name 2>r
  s{
  2r> 2dup 2dup  \ Tres nombres repetidos con «artículo negativo»
  s" eso" s" esa cosa" s" tal cosa"  \ Tres alternativas
  }s
  ;
: human_subjective_negative_name  ( a -- a1 u1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente (humano), desde el punto de vista del protagonista.
  dup is_known?
  if  full_name  else  drop s" nadie"  then
  ;
: subjective_negative_name  ( a -- a1 u1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente, desde el punto de vista del protagonista.
  dup is_human?
  if  human_subjective_negative_name
  else  nonhuman_subjective_negative_name
  then
  ;
: /l$  ( a -- a1 u1 | a1 0 )
  \ Devuelve la terminación «l» del artículo determinado masculino para añadirla a la preposición «a», si un ente humano lo requiere para ser usado como objeto directo; o una cadena vacía.
  \ No se usa!!!
  s" l" rot has_personal_name? 0= and
  ;
: a/$  ( a -- a1 u1 | a1 0 )
  \ Devuelve la preposición «a» si un ente lo requiere para ser usado como objeto directo; o una cadena vacía.
  s" a" rot is_human? and
  ; 
: a/l$  ( a -- a1 u1 )
  \ Devuelve la preposición «a», con posible artículo determinado, si un ente lo requiere para ser usado como objeto directo.
  \ No se usa!!!
  a/$ dup if  /l$ s+  then
  ;
: subjective_negative_name_as_direct_object  ( a -- a1 u1 )
  \ Devuelve el nombre subjetivo (negativo) de un ente, desde el punto de vista del protagonista, para ser usado como objeto directo.
  dup a/$ rot subjective_negative_name s&
  ;
: .full_name  ( a -- )
  \ Imprime el nombre completo de un ente.
  \ No se usa!!!
  full_name paragraph
  ;

\ }}} ##########################################################
section( Algunas cadenas calculadas y operaciones con ellas)  \ {{{

(

Nota!!!: ¿Mover a otra sección?

)

: «open»|«closed»  ( a -- a1 u1 )
  \ Devuelve «abierto/a/s» a «cerrado/a/s» según corresponda a un ente.
  dup is_open? if  s" abiert"  else  s" cerrad"  then
  rot noun_ending s+
  ;
: player_gender_ending$  ( -- a u )
  \ Devuelve la terminación «a» u «o» según el sexo del jugador.
  [false] [if]
    \ Método 1, «estilo BASIC»:
    woman_player? @ if  s" a"  else  s" o"  then
  [else]
    \ Método 2, sin estructuras condicionales, «estilo Forth»:
    c" oa" woman_player? @ abs + 1+ 1
  [then]
  ;
: player_gender_ending$+  ( a1 u1 -- a2 u2 )
  \ Añade a una cadena la terminación «a» u «o» según el sexo del jugador.
  player_gender_ending$ s+
  ;

\ }}} ##########################################################
section( Operaciones elementales con entes)  \ {{{

(

Algunas operaciones sencillas relacionadas con la trama.

Alguna es necesario crearla como vector porque se usa en las
descripciones de los entes o en las acciones, antes de
definir la trama.

)

defer lock_found  \ Encontrar el candado; la definición está en '(lock_found)'

0 constant limbo \ Marcador para usar como localización de entes inexistentes
: vanished?  ( a -- ff )
  \ ¿Está un ente desaparecido?
  location limbo =
  ;
: not_vanished?  ( a -- ff )
  \ ¿No está un ente desaparecido?
  vanished? 0=
  ;
: vanish  ( a -- )
  \ Hace desaparecer un ente llevándolo al «limbo».
  limbo swap is_there
  ;
: vanish_if_hold  ( a -- )
  \ Hace desaparecer un ente si su localización es el protagonista.
  \ No se usa!!!
  dup is_hold? if  vanish  else  drop  then
  ;

\ }}} ##########################################################
section( Herramientas para crear las fichas de la base de datos)  \ {{{

(

No es posible reservar el espacio necesario para las fichas
hasta saber cuántas necesitaremos [a menos que usáramos una
estructura un poco más sofisticada con fichas separadas pero
enlazadas entre sí, muy habitual también y fácil de crear].
Por ello la palabra 'ENTITIES [que devuelve la dirección de
la base de datos] se crea como un vector, para asignarle
posteriormente su dirección de ejecución.  Esto permite
crear un nuevo ente fácilmente, sin necesidad de asignar
previamente el número de fichas a una constante.

)

defer 'entities  \ Dirección de los entes; vector que después será redirigido a la palabra real
0 value #entities  \ Contador de entes, que se actualizará según se vayan creando

: #>entity  ( u -- a )
  \ Devuelve la dirección de la ficha de un ente a partir de su número ordinal
  \ (el número del primer ente es el cero).
  /entity * 'entities +
  ;
: entity>#  ( a -- u )
  \ Devuelve el número ordinal de un ente (el primero es el cero)
  \ a partir de la dirección de su ficha.
  'entities - /entity /
  ;
: entity:  ( "name" -- )
  \ Crea un nuevo identificador de ente,
  \ que devolverá la dirección de su ficha.
  create
    #entities ,  \ Guardar la cuenta en el cuerpo de la palabra recién creada
    #entities 1+ to #entities  \ Actualizar el contador
  does>  ( pfa -- a )
    @ #>entity  \ El identificador devolverá la dirección de su ficha
  ;
: erase_entity  ( a -- )
  \ Rellena con ceros la ficha de un ente.
  /entity erase
  ;
: backup_entity  ( a -- x1 x2 x3 x4 x5 )
  \ Respalda los datos de un ente
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético, para facilitar la edición).
  >r
  r@ description_xt
  r@ init_xt
  r@ location_enter_plot_xt
  r@ location_exit_plot_xt
  r> name_str
  ;
: restore_entity  ( x1 x2 x3 x4 x5 a -- )
  \ Restaura los datos de un ente
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético inverso, para facilitar la edición).
  >r
  r@ ~name_str !
  r@ ~location_exit_plot_xt !
  r@ ~location_enter_plot_xt !
  r@ ~init_xt !
  r> ~description_xt !
  ;
: setup_entity  ( a -- )
  \ Prepara la ficha de un ente para ser completada con sus datos .
  >r r@ backup_entity  r@ erase_entity  r> restore_entity
  ;
0 value self%  \ Ente cuyos atributos, descripción o trama están siendo definidos (usado para aligerar la sintaxis)
: :name_str  ( a -- )
  \ Crea una cadena dinámica nueva para guardar el nombre del ente.
  [debug_init] [if]  s" Inicio de :NAME_STR" debug [then]
  dup name_str ?dup
  [debug_init] [if]  s" A punto para STR-FREE" debug [then]
  if  str-free  then
  str-new swap ~name_str !
  [debug_init] [if]  s" Final de :NAME_STR" debug [then]
  ;
: [:attributes]  ( a -- )
  \ Inicia la definición de propiedades de un ente.
  \ Esta palabra se ejecuta cada vez que hay que restaurar los datos del ente,
  \ y antes de la definición de atributos contenida en la palabra
  \ correspondiente al ente.
  \ El identificador del ente está en la pila porque se compiló con 'literal'
  \ cuando se creó la palabra de atributos.
  dup to self%  \ Actualizar el puntero al ente
  dup :name_str  \ Crear una cadena dinámica para el campo '~name_str'
  setup_entity
  ;
: default_description
  \ Descripción predeterminada de los entes
  \ para los que no se ha creado una palabra propia de descripción.
  ^is_normal$ paragraph
  ;
: (:attributes)  ( a xt -- )
  \ Operaciones preliminares para la definición de atributos de un ente.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define sus atributos.
  \ a = Ente para la definición de cuyos atributos se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~init_xt !  \ Conservar la dirección de ejecución en la ficha del ente
  ['] default_description over ~description_xt !  \ Poner la descripción predeterminada
  postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que '[:description]' lo guarde en 'self%' en tiempo de ejecución
  ;
: :attributes  ( a -- )
  \ Inicia la creación de una palabra sin nombre que definirá las propiedades de un ente.
  :noname  5 roll 5 roll 
  (:attributes)  \ Crear la palabra y hacer las operaciones preliminares
  postpone [:attributes]  \ Compilar la palabra '[:attributes]' en la palabra creada, para que se ejecute cuando sea llamada
  \ lina necesita guardar una copia del puntero de la pila tras crear una palabra
  \ ( lo mismo ocurre después con las definiciones de ':description', ':location_enter_plot' y ':action'):
  ;
: ;attributes  ( sys-col -- )  postpone ;  ;  immediate
: init_entity  ( a -- )
  \ Restaura la ficha de un ente a su estado original.
  [debug_init] [if]  s" Inicio de INIT_ENTITY" debug dup entity># cr ." Entity=" .  [then]
  init_xt 
  [debug_init] [if]  s" Antes de EXECUTE" debug  [then]
  execute 
  [debug_init] [if]  s" Final de INIT_ENTITY" debug  [then]
  ;
: init_entities
  \ Restaura las fichas de los entes a su estado original.
  #entities 0 do
    [debug_init] [if]  i cr ." about to init entity #" .  [then]
    i #>entity init_entity
    \ i #>entity full_name space type .s?  \ Depuración!!!
  loop
  ;

\ }}} ##########################################################
section( Herramientas para crear las descripciones)  \ {{{

(

No almacenamos las descripciones en la base de datos junto
con el resto de atributos de los entes, sino que para cada
ente creamos una palabra que imprime su descripción, lo que
es mucho más flexible: La descripción podrá variar en
función del desarrollo del juego y adaptarse a las
circunstancias, e incluso sustituir en algunos casos al
código que controla la trama del juego.

Así pues, lo que almacenamos en la ficha del ente, en el
campo '~description_xt', es la dirección de ejecución de la
palabra que imprime su descripción.

Por tanto, para describir un ente basta tomar de su ficha el
contenido de '~description_xt', y llamar a 'execute'.

)

false value sight  \ Guarda el ente dirección al que se mira en un escenario (o el propio ente escenario); se usa en las palabras de descripción de escenarios
: [:description]  ( a -- )
  \ Operacionas previas a la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al comienzo de la palabra de descripción.
  \ El identificador del ente está en la pila porque se compiló con 'literal' cuando se creó la palabra de descripción.
  to self%  \ Actualizar el puntero al ente, usado para aligerar la sintaxis
  ;
: (:description)  ( a xt -- )
  \ Operaciones preliminares para la definición de la descripción de un ente.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que crea su descripción. 
  \ a = Ente para cuya descripción se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~description_xt !  \ Conservar la dirección de ejecución en la ficha del ente
  postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que '[:description]' lo guarde en 'self%' en tiempo de ejecución
  ;
: :description  ( a -- )
  \ Inicia la definición de una palabra de descripción para un ente.
  :noname  5 roll 5 roll 
  (:description)  \ Hacer las operaciones preliminares
  postpone [:description]  \ Compilar la palabra '[:description]' en la palabra creada, para que se ejecute cuando sea llamada
  ;
: [;description]
  \ Operaciones finales tras la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al final de la palabra de descripción.
  false to sight  \ Poner a cero el selector de vista, para evitar posibles errores
  ;
: ;description  ( colon-sys -- )
  \ Termina la definición de una palabra de descripción de un ente.
  postpone [;description]  \ Compilar la palabra '[;description]' en la palabra creada, para que se ejecute cuando sea llamada
  postpone ;
  ;  immediate
: (describe)  ( a -- )
  \ Ejecuta la palabra de descripción de un ente.
  ~description_xt perform
  ;
: .location_name  ( a -- )
  \ Imprime el nombre de un ente escenario, como cabecera de su descripción.
  [debug_map] [if]  dup  [then]
  name ^uppercase ['] location_name_color |paragraph 
  [debug_map] [if]
    entity># location_01% entity># - 1+ ."  [ location #" . ." ]"
  [then]
  ;
: (describe_location)  ( a -- )
  \ Describe un ente escenario.
  dup to sight
  location_description_color (describe)
  ;
: describe_location  ( a -- )
  \ Describe un ente escenario, con borrado de pantalla y título.
  [debug] [if]  s" En DESCRIBE_LOCATION" debug  [then]  \ Depuración!!!
  clear_screen_for_location
  dup .location_name  (describe_location)
  [debug] [if]  cr s" Location visited:" my_location visits .  [then]  \ Depuración!!!
  ;
: describe_other  ( a -- )
  \ Describe un ente de otro tipo.
  description_color (describe)
  ;
: describe_direction  ( a -- )
  \ Describe un ente dirección.
  to sight  \ Poner el ente dirección en 'sight'
  my_location describe_other  \ Y describir el escenario actual como un ente normal; ahí se hace la distinción
  ;
: description_type  ( a -- u )
  \ Convierte un ente en el tipo de descripción que requiere.
  \ a = Ente
  \ u = Tipo de descripción (4:salida, 2:dirección, 1:escenario, 0:otros, 3:¡error!)
  \ Nota: Un resultado de 3 significaría que el ente es a la vez dirección y escenario.
  dup is_location? abs
  [true] [if]
  swap is_direction? 2 and +
  [else]  \ Inacabado!!!
  over is_direction? 2 and +
  swap exits% = 4 and +
  [then]
  ;
: describe  ( a -- )
  \ Describe un ente, según su tipo.
  [debug] [if]  s" En DESCRIBE" debug  [then]  \ Depuración!!!
  dup description_type  
  [debug] [if]  s" En DESCRIBE antes de CASE" debug  [then]  \ Depuración!!!
  case
    0 of  describe_other  endof
    1 of  describe_location  endof
    2 of  describe_direction  endof
    abort" Error fatal en DESCRIBE : dato incorrecto"  \ depuración!!!
  endcase
  ;
: uninteresting_direction
  \ Muestra la descripción de la direcciones que no tienen nada especial.
  uninteresting_direction$ paragraph
  ;

\ }}} ##########################################################
section( Identificadores de entes)  \ {{{

(

Cada ente es identificado mediante una palabra. Los
identificadores de entes se crean con la palabra ENTITY: .
Cuando se ejecutan devuelven la dirección en memoria de la
ficha del ente en la base de datos, que después puede ser
modificada con un identificador de campo para convertirla en
la dirección de memoria de un campo concreto de la ficha.

Para reconocer mejor los identificadores de entes usamos el
sufijo «%» en sus nombres.

Los entes escenario usan como nombre de identificador el
número el número que tienen en la versión original del
programa. Esto hace más fácil la adaptación del código
original en BASIC.  Además, para que algunos cálculos
tomados del código original funcionen, es preciso que los
entes escenario se creen ordenados por ese número.

El orden en que se definan los restantes identificadores es
irrelevante.  Si están agrupados por tipos y en orden
alfabético es solo por claridad. 

)

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
entity: cave_entrance%  
entity: cloak%
entity: cuirasse%
entity: door%
entity: emerald%
entity: fallen_away%
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
entity: ravine_wall%
entity: rocks%
entity: snake%
entity: (stone%) ' (stone%) is stone%
entity: (sword%) ' (sword%) is sword%
entity: table%
entity: thread%
entity: (torch%) ' (torch%) is torch%
entity: wall%  \ Inacabado!!!
entity: waterfall%

\ Entes escenario (en orden de número):
entity: (location_01%) ' (location_01%) is location_01%
entity: location_02%
entity: location_03%
entity: location_04%
entity: location_05%
entity: location_06%
entity: location_07%
entity: location_08%
entity: location_09%
entity: location_10%
entity: location_11%
entity: location_12%
entity: location_13%
entity: location_14%
entity: location_15%
entity: location_16%
entity: location_17%
entity: location_18%
entity: location_19%
entity: location_20%
entity: location_21%
entity: location_22%
entity: location_23%
entity: location_24%
entity: location_25%
entity: location_26%
entity: location_27%
entity: location_28%
entity: location_29%
entity: location_30%
entity: location_31%
entity: location_32%
entity: location_33%
entity: location_34%
entity: location_35%
entity: location_36%
entity: location_37%
entity: location_38%
entity: location_39%
entity: location_40%
entity: location_41%
entity: location_42%
entity: location_43%
entity: location_44%
entity: location_45%
entity: location_46%
entity: location_47%
entity: location_48%
entity: location_49%
entity: location_50%
entity: location_51%

\ Entes globales:
entity: sky%
entity: floor%
entity: ceiling%
entity: clouds%
entity: cave%  \ Inacabado!!!

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
\ (pues la palabra 'entity:' actualiza el contador '#entities')
\ y por tanto podemos reservar espacio para la base de datos:

#entities /entity * constant /entities  \ Espacio necesario para guardar todas las fichas, en octetos
create ('entities) /entities allot  \ Reservar el espacio en el diccionario
' ('entities) is 'entities  \ Asignar el vector a la palabra real
'entities /entities erase  \ Llenar la zona con ceros, para mayor seguridad

\ }}} ##########################################################
section( Herramientas para crear conexiones entre escenarios)  \ {{{

\ Nota!!!: Este código quedaría mejor con el resto
\ de herramientas de la base de datos, para no separar
\ la lista de entes de sus datos.
\ Pero se necesita usar los identificadores
\ de los entes dirección.
\ Se podría solucionar con vectores, más adelante.

(

Para crear el mapa hay que hacer dos operaciones con los
entes escenario: marcarlos como tales, para poder
distinguirlos como escenarios; e indicar a qué otros entes
escenario conducen sus salidas.

La primera operación se hace guardando un valor buleano
«cierto» en el campo ~IS_LOCATION? del ente.  Por ejemplo:

  cave% ~is_location? bit_on

O bien mediante la palabra creada para ello en la interfaz
básica de campos:

  cave% is_location

La segunda operación se hace guardando en los campos de
salida del ente los identificadores de los entes a que cada
salida conduzca.  No hace falta ocuparse de las salidas
impracticables porque ya estarán a cero de forma
predeterminada.  Por ejemplo: 

  path% cave% ~south_exit !  \ Hacer que la salida sur de 'cave%' conduzca a 'path%'
  cave% path% ~north_exit !  \ Hacer que la salida norte de 'path%' conduzca a 'cave%'

No obstante, para hacer más fácil este segundo paso, hemos
creado unas palabras que proporcionan una sintaxis específica,
como mostraremos a continuación.

)

0 [if]  \ Inacabado!!!

create opposite_exits
south_exit> ,
north_exit> ,
west_exit> ,
east_exit> ,
down_exit> ,
up_exit> ,
in_exit> ,
out_exit> ,

create opposite_direction_entities
south% ,
north% ,
west% ,
east% ,
down% ,
up% ,
in% ,
out% ,

[then]

(

Necesitamos una tabla que nos permita traducir esto:

ENTRADA: Un puntero correspondiente a un campo de dirección
de salida en la ficha de un ente.

SALIDA: El identificador del ente dirección al que se
refiere esa salida.

)

create exits_table  \ Tabla de traducción de salidas
#exits cells allot  \ Reservar espacio para tantas celdas como salidas
: >exits_table>  ( u -- a )
  \ Apunta a la dirección de un elemento de la tabla de direcciones.
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  \ a = Dirección del ente dirección correspondiente en la tabla
  first_exit> - exits_table +
  ;
: exits_table!  ( a u -- )
  \ Guarda un ente en una posición de la tabla de salidas.
  \ a = Ente dirección
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  >exits_table> !
  ;
: exits_table@  ( u -- a )
  \ Devuelve un ente dirección a partir de un campo de dirección.
  \ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
  \ a = Ente dirección
  >exits_table> @ 
  ;

\ Rellenar cada elemento de la tabla con un ente de salida,
\ usando como puntero el campo análogo de la ficha.
\ Haciéndolo de esta manera no importa el orden en que se rellenen los elementos.
north% north_exit> exits_table!
south% south_exit> exits_table!
east% east_exit> exits_table!
west% west_exit> exits_table!
up% up_exit> exits_table!
down% down_exit> exits_table!
out% out_exit> exits_table!
in% in_exit> exits_table!

0 [if]  \ Inacabado!!!
: opposite_exit  ( a1 -- a2 )
  \ Devuelve la dirección cardinal opuesta a la indicada.
  first_exit> - opposite_exits + @
  ;
: opposite_exit%  ( a1 -- a2 )
  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada.
  \ a1 = entidad de dirección
  \ a2 = entidad de dirección, opuesta a a1
  first_exit> - opposite_direction_entities + @
  ;
[then]

(

A continuación definimos palabras para proporcionar la
siguiente sintaxis [primero origen y después destino en la
pila, como es convención en Forth]:

  \ Hacer que la salida sur de 'cave%' conduzca a 'path%'
  \ pero sin afectar al sentido contrario:
  cave% path% s--> 
 
  \ Hacer que la salida norte de 'path%' conduzca a 'cave%'
  \ pero sin afectar al sentido contrario:
  path% cave% n-->

O en un solo paso:

  \ Hacer que la salida sur de 'cave%' conduzca a 'path%'
  \ y al contrario: la salida norte de 'path%' conducirá a 'cave%':
  cave% path% s<-->

)

: -->  ( a1 a2 u -- )
  \ Conecta el ente a1 con el ente a2 mediante la salida indicada por el desplazamiento u.
  \ a1 = Ente origen de la conexión
  \ a2 = Ente destino de la conexión
  \ u = Desplazamiento del campo de dirección a usar en a1
  rot + !
  ;
: -->|  ( a1 u -- )
  \ Cierra la salida del ente a1 indicada por el desplazamiento u.
  \ a1 = Ente origen de la conexión
  \ u = Desplazamiento del campo de dirección a usar en a1
  + no_exit swap !
  ;

\ Conexiones unidireccionales

: n-->  ( a1 a2 -- )
  \ Comunica la salida norte del ente a1 con el ente a2.
  north_exit> -->
  ;
: s-->  ( a1 a2 -- )
  \ Comunica la salida sur del ente a1 con el ente a2.
  south_exit> -->
  ;
: e-->  ( a1 a2 -- )
  \ Comunica la salida este del ente a1 con el ente a2.
  east_exit> -->
  ;
: w-->  ( a1 a2 -- )
  \ Comunica la salida oeste del ente a1 con el ente a2.
  west_exit> -->
  ;
: u-->  ( a1 a2 -- )
  \ Comunica la salida hacia arriba del ente a1 con el ente a2.
  up_exit> -->
  ;
: d-->  ( a1 a2 -- )
  \ Comunica la salida hacia abajo del ente a1 con el ente a2.
  down_exit> -->
  ;
: o-->  ( a1 a2 -- )
  \ Comunica la salida hacia fuera del ente a1 con el ente a2.
  out_exit> -->
  ;
: i-->  ( a1 a2 -- )
  \ Comunica la salida hacia dentro del ente a1 con el ente a2.
  in_exit> -->
  ;

: n-->|  ( a1 -- )
  \ Desconecta la salida norte del ente a1.
  north_exit> -->|
  ;
: s-->|  ( a1 -- )
  \ Desconecta la salida sur del ente a1.
  south_exit> -->|
  ;
: e-->|  ( a1 -- )
  \ Desconecta la salida este del ente a1.
  east_exit> -->|
  ;
: w-->|  ( a1 -- )
  \ Desconecta la salida oeste del ente a1.
  west_exit> -->|
  ;
: u-->|  ( a1 -- )
  \ Desconecta la salida hacia arriba del ente a1.
  up_exit> -->|
  ;
: d-->|  ( a1 -- )
  \ Desconecta la salida hacia abajo del ente a1.
  down_exit> -->|
  ;
: o-->|  ( a1 -- )
  \ Desconecta la salida hacia fuera del ente a1.
  out_exit> -->|
  ;
: i-->|  ( a1 -- )
  \ Desconecta la salida hacia dentro del ente a1.
  in_exit> -->|
  ;

\ Conexiones bidireccionales

: n<-->  ( a1 a2 -- )
  \ Comunica la salida norte del ente a1 con el ente a2 (y al contrario).
  2dup n-->  swap s-->
  ;
: s<-->  ( a1 a2 -- )
  \ Comunica la salida sur del ente a1 con el ente a2 (y al contrario).
  2dup s-->  swap n-->
  ;
: e<-->  ( a1 a2 -- )
  \ Comunica la salida este del ente a1 con el ente a2 (y al contrario).
  2dup e-->  swap w-->
  ;
: w<-->  ( a1 a2 -- )
  \ Comunica la salida oeste del ente a1 con el ente a2 (y al contrario).
  2dup w-->  swap e-->
  ;
: u<-->  ( a1 a2 -- )
  \ Comunica la salida hacia arriba del ente a1 con el ente a2 (y al contrario).
  2dup u-->  swap d-->
  ;
: d<-->  ( a1 a2 -- )
  \ Comunica la salida hacia abajo del ente a1 con el ente a2 (y al contrario).
  2dup d-->  swap u-->
  ;
: o<-->  ( a1 a2 -- )
  \ Comunica la salida hacia fuera del ente a1 con el ente a2 (y al contrario).
  2dup o-->  swap i-->
  ;
: i<-->  ( a1 a2 -- )
  \ Comunica la salida hacia dentro del ente a1 con el ente a2 (y al contrario).
  2dup i-->  swap o-->
  ;

: n|<-->|  ( a1 a2 -- )
  \ Desconecta la salida norte del ente a1 con el ente a2 (y al contrario).
  s-->|  n-->|
  ;
: s|<-->|  ( a1 a2 -- )
  \ Desconecta la salida sur del ente a1 con el ente a2 (y al contrario).
  n-->|  s-->|
  ;
: e|<-->|  ( a1 a2 -- )
  \ Desconecta la salida este del ente a1 con el ente a2 (y al contrario).
  w-->|  e-->|
  ;
: w|<-->|  ( a1 a2 -- )
  \ Desconecta la salida oeste del ente a1 con el ente a2 (y al contrario).
  e-->|  w-->|
  ;
: u|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia arriba del ente a1 con el ente a2 (y al contrario).
  d-->|  u-->|
  ;
: d|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia abajo del ente a1 con el ente a2 (y al contrario).
  u-->|  d-->|
  ;
: o|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia fuera del ente a1 con el ente a2 (y al contrario).
  i-->|  o-->|
  ;
: i|<-->|  ( a1 a2 -- )
  \ Desconecta la salida hacia dentro del ente a1 con el ente a2 (y al contrario).
  o-->|  i-->|
  ;

(

Por último, definimos dos palabras para hacer
todas las asignaciones de salidas en un solo paso. 

)

: exits!  ( a1 ... a8 a0 -- )
  \ Asigna todas las salidas de un ente escenario.
  \ a1 ... a8 = Entes escenario de salida (o cero) en el orden habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera
  \ a0 = Ente escenario cuyas salidas hay que modificar
  >r
  r@ ~out_exit !
  r@ ~in_exit !
  r@ ~down_exit !
  r@ ~up_exit !
  r@ ~west_exit !
  r@ ~east_exit !
  r@ ~south_exit !
  r> ~north_exit !
  ;

: init_location  ( a1 ... a8 a0 -- )
  \ Marca un ente como escenario y le asigna todas las salidas. .
  \ a1 ... a8 = Entes escenario de salida (o cero) en el orden habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera
  \ a0 = Ente escenario cuyas salidas hay que modificar
  dup is_location exits!
  ;

\ }}} ##########################################################
section( Recursos para las descripciones de entes)  \ {{{

(

Las palabras de esta sección se usan para 
construir las descripciones de los entes.
Cuando su uso se vuelve más genérico, se mueven
a la sección de textos calculados.

)

\ ------------------------------------------------
\ Albergue de los refugiados

: the_refugees$  ( -- a u )
  leader% conversations?
  if  s" los refugiados"  else  s" todos"  then
  ;
: ^the_refugees$  ( -- a u )
  the_refugees$ ^uppercase
  ;
: they_don't_let_you_pass$  ( -- a u )
  \ Mensaje de que los refugiados no te dejan pasar.
  s{
  s" te" s? (they)_block$ s&
  s" te rodean,"
  s{ s" impidiéndote" s" impidiendo"
  s" obstruyendo" s" obstruyéndote"
  s" bloqueando" s" bloqueándote" }s&
  }s the_pass$ s&
  ;
: the_pass_free$  ( -- a u )
  \ Variante de «libre el paso».
  s" libre" the_pass$ s&
  ;
: they_let_you_pass_0$  ( -- a u )
  \ Primera versión del mensaje de que te dejan pasar.
  s{
  s" te" s? s" han dejado" s&
  s" se han" s{ s" apartado" s" echado a un lado" }s& s" para dejar" s& s" te" s?+
  }s the_pass_free$ s&
  ;
: they_let_you_pass_1$  ( -- a u )
  \ Segunda versión del mensaje de que te dejan pasar.
  s" se han" s{ s" apartado" s" retirado" }s&
  s" a" s{ s{ s" los" s" ambos" }s s" lados" s& s" uno y otro lado" }s& s?& comma+
  s{ s" dejándote" s" dejando" s" para dejar" s" te" s?+ }s&
  the_pass_free$ s&
  ;
: they_let_you_pass_2$  ( -- a u )
  \ Tercera versión del mensaje de que te dejan pasar.
  s" ya no" they_don't_let_you_pass$ s& s" como antes" s?&
  ;
: they_let_you_pass$  ( -- a u )
  \ Mensaje de que te dejan pasar.
  ['] they_let_you_pass_0$
  ['] they_let_you_pass_1$
  ['] they_let_you_pass_2$
  3 choose execute
  ;

: the_leader_said_they_want_peace$  ( -- a u )
  \ El líder te dijo qué buscan los refugiados.
  s" que," s" tal y" s?& s" como" s& leader% full_name s&
  s" te" s?& s" ha" s&{ s" referido" s" hecho saber" s" contado" s" explicado" }s& comma+
  they_want_peace$ s&
  ;
: you_don't_know_why_they're_here$  ( -- a u )
  \ No sabes por qué están aquí los refugiados.
  s{  s" Te preguntas"
      s" No" s{  s" terminas de"
                  s" acabas de"
                  s" aciertas a"
                  s" puedes"
                }s& to_understand$ s&
      s" No" s{ s" entiendes" s" comprendes" }s&
  }s{
    s" qué" s{ s" pueden estar" s" están" }s& s" haciendo" s&
    s" qué" s{ s" los ha" s{ s" podría" s" puede" }s s" haberlos" s& }s&
      s{ s" reunido" s" traído" s" congregado" }s&
    s" cuál es" s{ s" el motivo" s" la razón" }s&
      s" de que se" s&{ s" encuentren" s" hallen" }s&
    s" por qué" s{ s" motivo" s" razón" }s?& s" se encuentran" s&
  }s& here$ s& period+
  ;
: some_refugees_look_at_you$  ( -- a u )
  s" Algunos" s" de ellos" s?&
  s" reparan en" s&{ s" ti" s" tu persona" s" tu presencia" }s&
  ;
: in_their_eyes_and_gestures$  ( -- a u )
  \ En sus ojos y gestos.
  s" En sus" s{ s" ojos" s" miradas" }s&
  s" y" s" en sus" s?& s" gestos" s&? s&
  ;
: the_refugees_trust$  ( -- a u )
  \ Los refugiados confían.
  some_refugees_look_at_you$ period+
  in_their_eyes_and_gestures$ s&
  s{ s" ves" s" notas" s" adviertes" s" aprecias" }s&
  s{
    s" amabilidad" s" confianza" s" tranquilidad"
    s" serenidad" s" afabilidad"
  }s&
  ;
: you_feel_they_observe_you$  ( -- a u )
  \ Sientes que te observan.
  s{ s" tienes la sensación de que" s" sientes que" }s?
  s" te observan" s& s" como" s?&
  s{  s" con timidez" s" tímidamente"
      s" de" way$ s& s" subrepticia" s& s" subrepticiamente"
      s" a escondidas"
  }s& period+
  ;
: the_refugees_don't_trust$  ( -- a u )
  \ Los refugiados no confían.
  some_refugees_look_at_you$ s{ s" . Entonces" s"  y" }s+
  you_feel_they_observe_you$ s&
  in_their_eyes_and_gestures$ s& 
  s{
    s{ s" crees" s" te parece" }s to_realize$ s&
    s{ s" ves" s" notas" s" adviertes" s" aprecias" }s
    s" parece" to_realize$ s& s" se" s+
  }s& s{
    s" cierta" s?{
      s" preocupación" s" desconfianza" s" intranquilidad"
      s" indignación" }s&
    s" cierto" s?{ s" nerviosismo" s" temor" }s&
  }s&
  ;
: diverse_people$  ( -- a u )
  s{ s" personas" s" hombres, mujeres y niños" }s
  s" de toda" s& s" edad y" s?& s" condición" s&
  ;
: refugees_description
  \ Descripición de los refugiados.
  talked_to_the_leader?
  if    s" Los refugiados son"
  else  s" Hay"
  then  diverse_people$ s&
  talked_to_the_leader?
  if    the_leader_said_they_want_peace$
  else  period+ you_don't_know_why_they're_here$
  then  s&
  do_you_hold_something_forbidden? 
  if    the_refugees_don't_trust$
  else  the_refugees_trust$ 
  then  s& period+ paragraph
  ;

\ ------------------------------------------------
\ Tramos de cueva (laberinto)

\ Elementos básicos usados en las descripciones

: this_narrow_cave_pass$  ( -- a u )
  \ Devuelve una variante de «estrecho tramo de cueva», con el artículo adecuado.
  my_location dup is_known?
  if  not_distant_article
  else  undefined_article
  then  narrow_cave_pass$ s&
  ;
: ^this_narrow_cave_pass$  ( -- a u )
  \ Devuelve una variante de «estrecho tramo de cueva», con el artículo adecuado y la primera letra mayúscula.
  this_narrow_cave_pass$ ^uppercase
  ;
: toward_the(m/f)  ( a -- a1 u1 )
  \ Devuelve una variante de «hacia el» con el artículo adecuado a un ente.
  has_feminine_name? if  toward_the(f)$  else  toward_the(m)$  then
  ;
: toward_(the)_name  ( a -- a1 u1 )
  \ Devuelve una variante de «hacia el nombre-de-ente» adecuada a un ente.
  dup has_no_article?
  if  s" hacia"
  else  dup toward_the(m/f)
  then  rot name s&  
  ;
: main_cave_exits_are$  ( -- a u )
  \ Devuelve una variante del inicio de la descripción de los tramos de cueva
  ^this_narrow_cave_pass$ lets_you$ s& to_keep_going$ s&
  ;

\ Variantes para la descripción de cada salida

: cave_exit_description_0$  ( -- a u )
  \ Devuelve la primera variante de la descripción de una salida de un tramo de cueva.
  ^this_narrow_cave_pass$  lets_you$ s& to_keep_going$ s&
  in_that_direction$ s&
  ;
: cave_exit_description_1$ ( -- a1 u1 )
  \ Devuelve la segunda variante de la descripción de una salida de un tramo de cueva.
  ^a_pass_way$
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  in_that_direction$ s&
  ;

\ Variantes para la descripción principal

false [if]  \ Código obsoleto!!!

: $two_main_exits_in_cave ( a1 u1 a2 u2 -- a3 u3 )
  \ Devuelve la descripción de un tramo de cueva con dos salidas a dos puntos cardinales.
  \ No se usa!!!
  \ Esta palabra solo sirve para parámetros de puntos cardinales (todos usan artículo determinado masculino)
  \ Se usa en la descripción principal de un escenario
  \ a1 u1 = Nombre de una dirección cardinal (sin artículo)
  \ a2 u2 = Nombre de la otra dirección cardinal (sin artículo)
  2>r 2>r
  this_narrow_cave_pass$ lets_you$ s& to_keep_going$ s&
  toward_the(m)$ 2dup 2r> s&  \ Una dirección
  2swap 2r> s&  \ La otra dirección
  both&
  ;
: $other_exit_in_cave  ( a1 u1 -- a2 u2 )
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ No se usa!!!
  \ Se usa en la descripción principal de un escenario
  \ Esta palabra solo sirve para parámetros de puntos cardinales (todos usan artículo determinado masculino)
  \ a1 u1 = Nombre de la dirección cardinal
  ^a_pass_way$ s&
  s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  toward_the(m)$ s& 2swap s&
  ;

[then]  \ Fin del código obsoleto!!!

: cave_exit_separator+  ( a1 u1 -- a2 u2 )
  \ Concatena (sin separación) a una cadena el separador entre las salidas principales y las secundarias.
  s{ s" ," s" ;" s" ..." }s+
  s{ s" y" 2dup s" aunque" but$ }s& s" también" s?&
  ;
: (paths)_can_be_seen_0$  ( -- a u )
  s{ s" parten" s" surgen" s" nacen" s" salen" }s
  s" de" s{ s" aquí" s" este lugar" }s& s? rnd2swap s&
  ;
: (paths)_can_be_seen_1$  ( -- a u )
  s{ s" se ven" s" pueden verse"
  s" se vislumbran" s" pueden vislumbrarse"
  s" se adivinan" s" pueden adivinarse"
  s" se intuyen" s" pueden intuirse" }s
  ;
: (paths)_can_be_seen$  ( -- a u )
  \ Pendiente!!! Hacer que el texto dependa, por grupos, de si el escenario es conocido.
  ['] (paths)_can_be_seen_0$  
  ['] (paths)_can_be_seen_1$  
  2 choose execute
  ;
: paths_seen  ( a1 u1 -- a2 u2 )
  \ Devuelve la presentación de la lista de salidas secundarias.
  \ a1 u1 = Cadena con el número de pasajes
  \ a2 u2 = Cadena con el resultado
  pass_ways$ s& s" más" s?&
  (paths)_can_be_seen$ rnd2swap s&
  ;

: secondary_exit_in_cave&  ( a1 a2 u2 -- a3 u3 )
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ a1 = Ente dirección cuya descripción hay que añadir
  \ a2 u2 = Descripción en curso
  rot toward_(the)_name s&
  ;
: one_secondary_exit_in_cave  ( a1 -- a2 u2 )
  \ Devuelve la descripción de una salida adicional en un tramo de cueva.
  \ a1 = Ente dirección
  a_pass_way$
  s{ s" parte" s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
  secondary_exit_in_cave&  
  ;
: two_secondary_exits_in_cave  ( a1 a2 -- a3 u3 )
  \ Devuelve la descripción de dos salidas adicionales en un tramo de cueva.
  s" dos" paths_seen s" :" s?+
  secondary_exit_in_cave& s" y" s&
  secondary_exit_in_cave& 
  ;
: three_secondary_exits_in_cave  ( a1 a2 a3 -- a4 u4 )
  \ Devuelve la descripción de tres salidas adicionales en un tramo de cueva.
  s" tres" paths_seen s" :" s?+
  secondary_exit_in_cave& comma+
  secondary_exit_in_cave& s" y" s&
  secondary_exit_in_cave&
  ;
: two_main_exits_in_cave ( a1 a2 -- a3 u3 )
  \ Devuelve la descripción de dos salidas principales en un tramo de cueva.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  toward_(the)_name rot toward_(the)_name both
  ;
: one_main_exit_in_cave  ( a1 -- a2 u2 )
  \ Devuelve la descripción de una salida principal en un tramo de cueva.
  \ a1 = Ente dirección
  toward_(the)_name 
  ;

\ Descripciones de los tramos de cueva según el reparto entre salidas principales y secundarias

: 1+1_cave_exits  ( a1 a2 -- a u )
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y una secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  one_main_exit_in_cave cave_exit_separator+
  rot one_secondary_exit_in_cave s&
  ;
: 1+2_cave_exits  ( a1 a2 a3 -- a u )
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y dos secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  one_main_exit_in_cave cave_exit_separator+
  2swap two_secondary_exits_in_cave s&
  ;
: 1+3_cave_exits  ( a1 a2 a3 a4 -- a u )
  \ Devuelve la descripción de un tramo de cueva
  \ con una salida principal y tres secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección
  one_main_exit_in_cave cave_exit_separator+
  2>r three_secondary_exits_in_cave 2r> 2swap s&
  ;
: 2+0_cave_exits  ( a1 a2 -- a u )
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y ninguna secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  two_main_exits_in_cave
  ;
: 2+1_cave_exits  ( a1 a2 a3 -- a u )
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y ninguna secundaria.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  two_main_exits_in_cave cave_exit_separator+
  rot one_secondary_exit_in_cave s&
  ;
: 2+2_cave_exits  ( a1 a2 a3 a4 -- a u )
  \ Devuelve la descripción de un tramo de cueva
  \ con dos salidas principales y dos secundarias.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección
  two_main_exits_in_cave cave_exit_separator+
  2swap two_secondary_exits_in_cave s&
  ;

\ Descripciones de los tramos de cueva según su número de salidas

: 1-exit_cave_description   ( a1 -- a u )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene una salida.
  \ a1 = Ente dirección
  toward_(the)_name 
  ;
: 2-exit_cave_description   ( a1 a2 -- a u )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene dos salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  ['] 2+0_cave_exits
  ['] 1+1_cave_exits
  2 choose execute
  ;
: 3-exit_cave_description   ( a1 a2 a3 -- a u )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene tres salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  ['] 2+1_cave_exits
  ['] 1+2_cave_exits
  2 choose execute
  ;
: 4-exit_cave_description   ( a1 a2 a3 a4 -- a u )
  \ Devuelve la descripción principal de un tramo de cueva
  \ que tiene cuatro salidas.
  \ a1 = Ente dirección
  \ a2 = Ente dirección
  \ a3 = Ente dirección
  \ a4 = Ente dirección
  ['] 2+2_cave_exits
  ['] 1+3_cave_exits
  2 choose execute
  ;
\ Tabla para contener las direcciones de las palabras de descripción:
create 'cave_descriptions
' 1-exit_cave_description ,
' 2-exit_cave_description ,
' 3-exit_cave_description ,
' 4-exit_cave_description ,

\ Interfaz para usar en las descripciones de los escenarios:
\ 'exits_cave_description' para la descripción principal
\ 'cave_exit_description$' para la descripción de cada salida

: unsort_cave_exits  ( a1 ... an u -- a1'..an' u )
  \ Desordena los entes dirección que son las salidas de la cueva.
  \ u = Número de elementos de la pila que hay que desordenar
  dup >r unsort r>
  ;
: (exits_cave_description)  ( a1 ... an u -- a2 u2 )
  \ Ejecuta (según el número de salidas) la palabra  que devuelve la descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados
  1- cells 'cave_descriptions + perform
  ;
: exits_cave_description  ( a1 ... an u -- a2 u2 )
  \ Devuelve la descripción principal de un tramo de cueva.
  \ a1 ... an = Entes de dirección correspondientes a las salidas
  \ u = Número de entes de dirección suministrados
  unsort_cave_exits  (exits_cave_description) period+
  main_cave_exits_are$ 2swap s&  \ Añadir el encabezado
  ;
: cave_exit_description$  ( -- a1 u1 )
  \ Devuelve la descripción de una dirección de salida de un tramo de cueva.
  ['] cave_exit_description_0$  \ Primera variante posible
  ['] cave_exit_description_1$  \ Segunda variante posible
  2 choose execute period+
  ;

\ ------------------------------------------------
\ La aldea sajona

: poor$  ( -- a u )
  s{ s" desgraciada" s" desdichada" }s 
  ;
: poor_village$  ( -- a u )
  poor$ s" aldea" s&
  ;
: rests_of_the_village$  ( -- a u )
  \ Devuelve parte de otras descripciones de la aldea arrasada.
  s" los restos" still$ s? s" humeantes" s& s?&
  s" de la" s& poor_village$ s&
  ;

\ ------------------------------------------------
\ Pared del desfiladero

: it_looks_impassable$  ( -- a u )
  \ Mensaje «parece infranqueable».
  s{ s" por su aspecto" s" a primera vista" s" en principio" }s?
  s{  s" parece imposible" s" de" s?& s{ s" superar" s" atravesar" s" franquear" s" vencer" }s&
      \ Eliminar!!!:
      \ s" no parece ofrecer" s{ s" salida" s" escapatoria" }s& s" alguna" s?&
      s" parece" s{ s" insuperable" s" invencible" s" infranqueable" }s& 
  }s& 
  ;
: the_cave_entrance_is_visible$  ( -- a u )
  s{  s" se" s{ s" ve" s" abre" s" haya"
                s" encuentra" s" aprecia" s" distingue" }s&
      s" puede" s{ s" verse" s" apreciarse" s" distinguirse" }s&
  }s cave_entrance% full_name s&
  ;

\ ------------------------------------------------
\ Entrada a la cueva

: the_cave_entrance_was_discovered?
  \ ¿La entrada a la cueva ya fue descubierta?
  location_08% has_south_exit?
  ;
: the_cave_entrance_is_accessible?
  \ ¿La entrada a la cueva está accesible (presente y descubierta)?
  location_08% am_i_there? the_cave_entrance_was_discovered? and
  ;
: open_the_cave_entrance
  \ Comunica el escenario 8 con el 10 (de dos formas y en ambos sentidos).
  location_08% dup location_10% s<-->  location_10% i<-->
  ;
: you_discover_the_cave_entrance$  ( -- a u )
  \ Mensaje de que descubres la cueva.
  ^but$ comma+
  s{  s" reconociendo" s" el terreno" more_carefully$ rnd2swap s& s&
      s" fijándote" more_carefully$ s&
  }s& comma+ s" descubres" s& s{ s" lo" s" algo" }s& s" que" s&
  s" sin duda" s?& s{ s" parece ser" s" es" s" debe de ser" }s&
  s{ s" la entrada" s" el acceso" }s& s" a una" s& cave$ s&
  ;
: you_discover_the_cave_entrance
  \ Descubres la cueva.
  you_discover_the_cave_entrance$ period+ paragraph
  open_the_cave_entrance
  cave_entrance% is_here
  ;
: you_maybe_discover_the_cave_entrance  ( a u -- )
  \ Descubres la cueva con un 50% de probabilidad.
  \ a u = Texto introductorio
  s" ..." s+ paragraph
  2 random 0= if  narration_break you_discover_the_cave_entrance  then
  ;
: the_cave_entrance_is_hidden$  ( -- a u )
  s" La entrada" s" a la cueva" s?&
  s{ s" está" s" muy" s?& s" no podría estar más" }s&
  s{ s" oculta" s" camuflada" s" escondida" }s& 
  s" en la pared" s& rocky(f)$ s& period+
  ;
: you_were_lucky_finding_it_out$ ( -- a u )
  s" Has tenido" s& s" muy" s?& s" buena" s&{ s" fortuna" s" suerte" }s&
  s{  s{ s" al" s" en" s" con" }s
        s{ s" hallarla" s" encontrarla" s" dar con ella" s" descubrirla" }s&
      s{  s" hallándola" s" encontrándola"
          s" dando con ella" s" descubriéndola"
      }s
  }s& period+
  ;
: it's_your_last_hope$  ( -- a u )
  s{ s" te das cuenta de que" s" sabes que" }s?
  s{ s" es" s" se trata de" }s& ^uppercase s&
  s{ s" la" s" tu" }s& s{ s" única" s" última" }s&
  s{ s" salida" s" opción" s" esperanza" s" posibilidad" }s&
  s{ s" de" s" para" }s{ s" escapar" s" huir" s" evitar ser capturado" }s&? s&
  period+ 
  ;

\ ------------------------------------------------
\ Otros escenarios

: bridge_that_way$  ( -- a u )
  s" El puente" leads$ s& that_way$ s& period+
  ;
: stairway_that_way$  ( -- a u )
  s" Las escaleras" (they)_lead$ s& that_way$ s& period+
  ;
: comes_from_there$  ( -- a u )
  comes_from$ from_that_way$ s&
  ;
: water_from_there$  ( -- a u )
  the_water_current$ comes_from_there$ s&
  ;
: ^water_from_there$  ( -- a u )
  ^the_water_current$ comes_from_there$ s&
  ;
: water_that_way$  ( -- a u )
  ^the_water_current$ s{ s" corre" s" fluye" s" va" }s&
  in_that_direction$ s& period+
  ;
: stairway_to_river$  ( -- a u )
  s" Las escaleras" (they)_go_down$ s&
  that_way$ s& comma+
  s{ s" casi" s? s" hasta el" s& s" mismo" s?& s" borde del" s&
  s" casi" s? s" hasta la" s& s" misma" s?& s" orilla del" s&
  s" casi" s? s" hasta el" s&
  s" hasta" s? s" cerca del" s& }s& s" agua." s&
  ;
: a_high_narrow_pass_way$  ( -- a u )
  s" un" narrow(m)$ s& pass_way$ s& s" elevado" s&
  ;

\ }}} ##########################################################
section( Atributos y descripciones de entes)  \ {{{

\ Ente protagonista

\ cr .( antes de ulfius) .s \ Depuración!!!
ulfius% :attributes
  s" Ulfius" self% mname!
  self% is_human
  self% has_personal_name
  self% has_no_article
  \ location_01% self% is_there
  ;attributes
ulfius% :description
  \ Provisional!!!
  s" [descripción de Ulfius]"
  paragraph 
  ;description

\ Entes personaje

ambrosio% :attributes
  s" hombre" self% mname!  \ El nombre cambiará a «Ambrosio» durante el juego
  self% is_character
  self% is_human
  location_19% self% is_there
  ;attributes
ambrosio% :description
  self% is_known? if
    s" Ambrosio"
    s" es un hombre de mediana edad, que te mira afable." s&
  else  s" Es de mediana edad y mirada afable."
  then  paragraph
  ;description
leader% :attributes
  s" anciano" self% mname!
  self% is_character
  self% is_human
  self% is_not_listed
  location_28% self% is_there
  ;attributes
leader% :description
  \ Pendiente!!! Elaborar esto según la trama.
  leader% conversations?
  if 
    s" Es el jefe de los refugiados."
  else
    s" Es un anciano."
  then
  paragraph
  ;description
soldiers% :attributes
  s" soldados" self% mnames!
  self% is_human
  self% familiar++
  self% is_decoration
  \ self% has_definite_article  \ Mejor implementar que tenga posesivo!!!...
  self% belongs_to_protagonist  \ ...aunque quizá esto baste!!!
  ;attributes
defer soldiers_description  \ Vector a la futura descripción
soldiers% :description
  \ La descripción de los soldados
  \ necesita usar palabras que aún no están definidas,
  \ y por ello es mejor crearla después.
  soldiers_description
  ;description
officers% :attributes
  s" oficiales" self% mnames!
  self% is_human
  self% familiar++
  self% is_decoration
  \ self% has_definite_article  \ Mejor implementar que tenga posesivo!!!...
  self% belongs_to_protagonist  \ ...aunque quizá esto baste!!!
  ;attributes
defer officers_description  \ Vector a la futura descripción
officers% :description
  \ La descripción de los oficiales
  \ necesita usar palabras que aún no están definidas,
  \ y por ello es mejor crearla después.
  officers_description
  ;description
refugees% :attributes
  s" refugiados" self% mnames!
  self% is_human
  self% is_decoration
  ;attributes
refugees% :description
  my_location case
  location_28% of  refugees_description  endof
  location_29% of
    \ Pendiente!!! Provisional!!!
    s" Todos los refugiados quedaron atrás." paragraph
    endof
  endcase
  ;description

\ Entes objeto

altar% :attributes
  s" altar" self% mname!
  self% is_decoration
  impossible_error# self% ~take_error# !
  location_18% self% is_there
  ;attributes
altar% :description
  s" Está" s{ s" situado" s" colocado" }s&
  s" justo en la mitad del puente." s&
  idol% is_unknown? if
    s" Debe de sostener algo importante." s&
  then
  paragraph
  ;description
arch% :attributes
  s" arco" self% mname!
  self% is_decoration
  location_18% self% is_there
  ;attributes
arch% :description
  \ Provisional!!!
  s" Un sólido arco de piedra, de una sola pieza."
  paragraph
  ;description
bed% :attributes
  s" catre" self% mname!
  location_46% self% is_there
  self% ambrosio% belongs
  ;attributes
bed% :description
  s{ s" Parece poco" s" No tiene el aspecto de ser muy"
  s" No parece especialmente" }s
  s{ s" confortable" s" cómod" self% adjective_ending s+ }s& period+
  paragraph
  ;description
bridge% :attributes
  s" puente" self% mname!
  self% is_decoration
  location_13% self% is_there
  ;attributes
bridge% :description
  \ Provisional!!!
  s" Está semipodrido."
  paragraph
  ;description
candles% :attributes
  s" velas" self% fnames!
  location_46% self% is_there
  self% ambrosio% belongs
  ;attributes
candles% :description
  s" Están muy consumidas."
  paragraph
  ;description
cave_entrance% :attributes
  s" entrada a una cueva" self% fname!
  ;attributes
cave_entrance% :description
  the_cave_entrance_is_hidden$
  you_were_lucky_finding_it_out$ s&
  it's_your_last_hope$ s&
  paragraph
  ;description
cloak% :attributes
  s" capa" self% fname!
  self% is_cloth
  self% belongs_to_protagonist
  self% is_worn
  ulfius% self% is_there
  ;attributes
cloak% :description
  s" Tu capa de general, de fina lana"
  s{ s" tintada de negro." s" negra." }s&
  paragraph
  ;description
cuirasse% :attributes
  s" coraza" self% fname!
  self% is_cloth
  self% belongs_to_protagonist
  self% is_worn
  ulfius% self% is_there
  ;attributes
door% :attributes
  s" puerta" self% fname!
  self% is_closed
  impossible_error# self% ~take_error# !
  location_47% self% is_there
  self% ambrosio% belongs
  ;attributes
door% :description
  self% times_open if  s" Es"  else  s" Parece"  then
  s" muy" s?& s{ s" recia" s" gruesa" s" fuerte" }s&
  location_47% am_i_there? if 
    lock% is_known?
    if    s" . A ella está unido el candado"
    else  s"  y tiene un gran candado"
    then  s+ lock_found
  then  period+
  s" Está" s& door% «open»|«closed» s& period+ paragraph
  ;description
emerald% :attributes
  s" esmeralda" self% fname!
  location_39% self% is_there
  ;attributes
emerald% :description
  s" Es preciosa."
  paragraph
  ;description
fallen_away% :attributes
  s" derrumbe" self% mname!
  self% is_decoration
  nonsense_error# self% ~take_error# !
  location_09% self% is_there
  ;attributes
fallen_away% :description
  s{
    s" Muchas," s" Muchísimas," s" Numerosas,"
    s" Un gran número de" s" Una gran cantidad de"
    \ Pendiente!!! Si se añade lo que sigue, hay que crear los entes "pared" y "muro":
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
: don't_take_the_flags
  \ Pendiente!!!
  s" [Yo no lo haría]." narrate
  ;
flags% :attributes
  s" banderas" self% fnames!
  self% is_decoration
  ['] don't_take_the_flags self% ~take_error# !
  location_28% self% is_there
  ;attributes
flags% :description
  s" Son las banderas britana y sajona."
  s" Dos dragones rampantes, rojo y blanco respectivamente, enfrentados." s&
  paragraph
  ;description
flint% :attributes
  s" pedernal" self% mname!
  ;attributes
flint% :description
  s" Es dura y afilada." 
  paragraph
  ;description
grass% :attributes
  s" hierba" self% fname!
  self% is_decoration
  ;attributes
grass% :description
  door% times_open if
    s" Está" self% verb_number_ending+
    s" aplastad" self% adjective_ending+ s&
    s{ s" en el" s" bajo el" s" a lo largo del" }s&
    s{ s" trazado" s" recorrido" }s&
    s{ s" de la puerta." s" que hizo la puerta al abrirse." }s&
  else
    s" Cubre" self% verb_number_ending+
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
  s" ídolo" self% mname!
  self% is_decoration
  impossible_error# self% ~take_error# !
  location_41% self% is_there
  ;attributes
idol% :description
  s" El ídolo tiene dos agujeros por ojos."
  paragraph
  ;description
key% :attributes
  s" llave" self% fname!
  location_46% self% is_there
  self% ambrosio% belongs
  ;attributes
key% :description
  \ Crear ente!!! hierro, herrumbre y óxido, visibles con la llave en la mano
  s" Grande, de hierro herrumboso."
  paragraph
  ;description
lake% :attributes
  s" lago" self% mname!
  self% is_decoration
  nonsense_error# self% ~take_error# !
  location_44% self% is_there
  ;attributes
lake% :description
  s{ s" La" s" Un rayo de" }s
  s" luz entra por un resquicio, y sus caprichosos reflejos te maravillan." s&
  paragraph
  ;description
lock% :attributes
  s" candado" self% mname!
  self% is_decoration
  self% is_closed
  impossible_error# self% ~take_error# !
  self% ambrosio% belongs
  ;attributes
lock% :description
  s" Es grande y parece" s{ s" fuerte." s" resistente." }s&
  s" Está" s&{ s" fijad" s" unid" }s& self% adjective_ending+
  s" a la puerta y" s&
  lock% «open»|«closed» s& period+
  paragraph
  ;description
log% :attributes
  s" tronco" self% mname!
  location_15% self% is_there
  ;attributes
log% :description
  s" Es un tronco"
  s{ s" recio," s" resistente," s" fuerte," }s&
  but$ s& s{ s" liviano." s" ligero." }s&
  paragraph
  ;description
piece% :attributes
  s" trozo de tela" self% mname!
  \ Nota!!! Ojo con este «de tela»: «Tela» es sinónimo de trozo;
  \ hay que contemplar estos casos en el cálculo de los genitivos.
  ;attributes
piece% :description
  s" Un pequeño" s{ s" retal" s" pedazo" s" trozo" s" resto" }s&
  of_your_ex_cloak$ s&
  paragraph
  ;description
rags% :attributes
  s" harapo" self% mname!
  ;attributes
rags% :description
  s" Un" s{ s" retal" s" pedazo" s" trozo" }s&
  s{ s" un poco" s" algo" }s?& s" grande" s&
  of_your_ex_cloak$ s&
  paragraph
  ;description
ravine_wall% :attributes
  s" pared" rocky(f)$ s& self% fname!
  location_08% self% is_there
  self% is_not_listed  \ Innecesario!!!
  self% is_decoration
  ;attributes
ravine_wall% :description
  s" en" the_cave_entrance_was_discovered? ?keep
  s" la pared" s& rocky(f)$ s& ^uppercase
  the_cave_entrance_was_discovered? if
    s" , que" it_looks_impassable$ s& comma+ s?+
    the_cave_entrance_is_visible$ s&
    period+ paragraph
  else
    it_looks_impassable$ s&
    you_maybe_discover_the_cave_entrance
  then
  ;description
rocks% :attributes
  s" rocas" self% fnames!
  self% is_decoration
  location_31% self% is_there
  ;attributes
rocks% :description
  location_31% has_north_exit?
  if  (rocks)_on_the_floor$ ^uppercase
  else  (rocks)_clue$
  then  period+ paragraph
  ;description
snake% :attributes
  s" serpiente" self% fname!
  self% is_animal
  dangerous_error# self% ~take_error# !
  location_43% self% is_there
  ;attributes
snake% :description
  \ Provisional!!! Distinguir si está muerta
  \ Nota!!! en el programa original no hace falta
  s" Una serpiente muy maja."
  paragraph
  ;description
stone% :attributes
  s" piedra" self% fname!
  location_18% self% is_there
  ;attributes
stone% :description
  s" Recia y pesada, pero no muy grande, de forma piramidal."
  paragraph
  ;description
sword% :attributes
  s" espada" self% fname!
  self% belongs_to_protagonist
  ulfius% self% is_there
  ;attributes
sword% :description
  s{ s" Legado" s" Herencia" }s s" de tu padre," s&
  s{ s" fiel herramienta" s" arma fiel" }s& s" en" s&
  s{ s" mil" s" incontables" s" innumerables" }s&
  s" batallas." s&
  paragraph
  ;description
table% :attributes
  s" mesa" self% fname!
  location_46% self% is_there
  self% ambrosio% belongs
  ;attributes
table% :description
  s" Es pequeña y de" s{ s" basta" s" tosca" }s& s" madera." s&
  paragraph
  ;description
thread% :attributes
  s" hilo" self% mname!
  ;attributes
thread% :description
  \ Mover esto al evento de cortar la capa!!!
  \ s" Un hilo se ha desprendido al cortar la capa con la espada."
  s" Un hilo" of_your_ex_cloak$ s&
  paragraph
  ;description
torch% :attributes
  s" antorcha" self% fname!
  self% is_light
  self% is_not_lit
  ;attributes
torch% :description
  \ Inacabado!!! 
  s" Está apagada."
  paragraph
  ;description
waterfall% :attributes
  s" cascada" self% fname!
  self% is_decoration
  nonsense_error# self% ~take_error# !
  location_38% self% is_there
  ;attributes
waterfall% :description
  s" No ves nada por la cortina de agua."
  s" El lago es muy poco profundo." s&
  paragraph
  ;description

\ Entes escenario

(

Las palabras que describen entes escenario reciben en
'sight' [variable que está creada con 'value' y por tanto
devuelve su valor como si fuera una constante] un
identificador de ente.  Puede ser el mismo ente escenario o
un ente de dirección.  Esto permite describir lo que hay más
allá de cada escenario en cualquier dirección.

)

location_01% :attributes
  s" aldea sajona" self% fname!
  0 location_02% 0 0 0 0 0 0 self% init_location
  ;attributes
location_01% :description
  \ Pendiente!!! Crear colina en los tres escenarios
  sight case
  self% of
    s" No ha quedado nada en pie, ni piedra sobre piedra."
    s{ s" El entorno es desolador." s" Todo alrededor es desolación." }s
    rnd2swap s&
    s{ ^only$ remains$ s&
    s" Lo único que" remains$ s& s" por hacer" s?& s" es" s&
    s" No" remains$ s& s{ s" más" s" otra cosa" }s& s" que" s&
    }s& to_go_back$ s& s" al Sur, a casa." s&
    paragraph
    endof
  south% of
    2 random if \ Versión 0:
      ^toward_the(m)$ s" Sur" s&
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
    (they)_go_up$ s&
    s{ s" lastimosamente" s" penosamente" }s&
    s" hacia" s{ s" el cielo" s" las alturas" }s& s?&
    s{ s" desde" s" de entre" }s& rests_of_the_village$ s&
    s" , como si" s" también" s" ellas" rnd2swap s& s?&
    s{ s" desearan" s" anhelaran" s" soñaran" }s&
    s" poder hacer un último esfuerzo por" s?&
    s" escapar" s& but|and$ s& s" no supieran cómo" s& s?+
    s" ..." s+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_02% :attributes
  s" cima de la colina" self% fname!
  location_01% 0 0 location_03% 0 0 0 0 self% init_location
  ;attributes
location_02% :description
  \ Crear ente!!! aldea, niebla
  sight case
  self% of
    s" Sobre" s" la cima de" s?&
    s" la colina, casi" s& s{ s" sobre" s" por encima de" }s&
    s" la" s&
    s" espesa" s" fría" both?& s" niebla de la aldea sajona arrasada al Norte, a tus pies." s&
    ^the_path$ s& goes_down$ s& toward_the(m)$ s& s" Oeste." s&
    paragraph
    endof
  north% of
    s" La" poor_village$ s& s" sajona" s& s" , arrasada," s?+ s" agoniza bajo la" s&
    s" espesa" s" fría" both?& s" niebla." s&
    paragraph
    endof
  west% of
    ^the_path$ goes_down$ s& s" por la" s& s" ladera de la" s?& s" colina." s&
    paragraph
    endof
  down% of
    \ Bajar la colina
    \ puede equivaler a bajar por el sur o por el oeste; esto
    \ se decide al azar cada vez que se
    \ entra en el escenario, por lo que su descripción
    \ debe tenerlo en cuenta y redirigir a la descripción adecuada:
    self% down_exit self% north_exit =
    if  north%  else  west%  then  describe
    endof
  uninteresting_direction
  endcase
  ;description
location_03% :attributes
  s" camino entre colinas" self% mname!
  0 0 location_02% location_04% 0 0 0 0 self% init_location
  ;attributes
location_03% :description
  sight case
  self% of
    ^the_path$ s" avanza por el valle," s&
    s" desde la parte alta, al Este," s&
    s" a una zona" s& very_$ s& s" boscosa, al Oeste." s&
    paragraph
    endof
  east% of
    ^the_path$ s" se pierde en la parte alta del valle." s&
    paragraph
    endof
  west% of
    s" Una zona" very_$ s& s" boscosa." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_04% :attributes
  s" cruce de caminos" self% mname!
  location_05% 0 location_03% location_09% 0 0 0 0 self% init_location
  ;attributes
location_04% :description
  sight case
  self% of
    s" Una senda parte al Oeste, a la sierra por el paso del Perro,"
    s" y otra hacia el Norte, por un frondoso bosque que la rodea." s&
    paragraph
    endof
  north% of
    ^a_path$ surrounds$ s& s" la sierra a través de un frondoso bosque." s&
    paragraph
    endof
  west% of
    ^a_path$ leads$ s& toward_the(f)$ s& s" sierra por el paso del Perro." s&
    paragraph
    endof
  down% of  endof
  up% of  endof
  uninteresting_direction
  endcase
  ;description
location_05% :attributes
  s" linde del bosque" self% fname!
  0 location_04% 0 location_06% 0 0 0 0 self% init_location
  ;attributes
location_05% :description
  sight case
  self% of
    ^toward_the(m)$ s" Oeste se extiende" s&
    s{ s" frondoso" s" exhuberante" }s& \ pendiente!!! independizar
    s" el bosque que rodea la sierra." s&
    s" La salida se abre" s&
    toward_the(m)$ s& s" Sur." s&
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
  uninteresting_direction
  endcase
  ;description
location_06% :attributes
  s" bosque" self% mname!
  0 0 location_05% location_07% 0 0 0 0 self% init_location
  ;attributes
location_06% :description
  sight case
  self% of
    s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
    ^the_path$ s& s" serpentea entre raíces, de un luminoso Este" s&
    toward_the(m)$ s& s" Oeste." s&
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
  uninteresting_direction
  endcase
  ;description
location_07% :attributes
  s" paso del Perro" self% mname!
  0 location_08% location_06% 0 0 0 0 0 self% init_location
  ;attributes
location_07% :description
  sight case
  self% of
    s" Abruptamente, del bosque se pasa a un estrecho camino entre altas rocas."
    s" El" s& s{ s" inquietante" s" sobrecogedor" }s&
    s" desfiladero" s& s{ s" tuerce" s" gira" }s&
    s" de Este a Sur." s&
    paragraph
    endof
  south% of
    ^the_path$ s" gira" s& in_that_direction$ s& period+
    paragraph
    endof
  east% of
    s" La estrecha senda es" s{ s" engullida" s" tragada" }s&
    s" por las" s&
    s" fauces" s{ s" frondosas" s" exhuberantes" }s rnd2swap s& s&
    s" del bosque." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_08% :attributes
  s" desfiladero" self% mname!
  location_07% 0 0 0 0 0 0 0 self% init_location
  ;attributes
location_08% :description
  \ Pendiente!!! Crear pared y roca y desfiladero
  sight case
  self% of
    ^the_pass_way$ s" entre el desfiladero sigue de Norte a Este" s&
    s" junto a una" s&
    s{  s" pared" rocky(f)$ s& s" rocosa pared" }s& period+
    \ Pendiente!!! Completar con entrada a caverna, tras ser descubierta.
    paragraph
    endof
  north% of
    s" El camino" s{ s" tuerce" s" gira" }s& \ pendiente!!! independizar gira/tuerce
    s" hacia el inquietante paso del Perro." s&
    paragraph
    endof
  south% of
    self% ravine_wall% is_there
    s{ ^in_that_direction$ s" Hacia el sur" }s
    s{ s" se alza" s" se levanta" }s&
    \ s" una pared" s& rocky(f)$ s& \ Antiguo!!!
    ravine_wall% full_name s&
    the_cave_entrance_was_discovered? if
      comma+ s" en la" s&{ s" que" s" cual" }s&
      the_cave_entrance_is_visible$ s&
      period+ paragraph
    else
      ravine_wall% is_known? if
        s" que" it_looks_impassable$ s& s?&
        you_maybe_discover_the_cave_entrance
      else  ravine_wall% familiar++
      then
    then
    endof
  uninteresting_direction
  endcase
  ;description
location_09% :attributes
  s" derrumbe" self% mname!
  0 0 location_04% 0 0 0 0 0 self% init_location
  ;attributes
location_09% :description
  sight case
  self% of
    ^the_path$ goes_down$ s& s" hacia la agreste sierra, al Oeste," s&
    s" desde los" s& s" verdes" s" valles" rnd2swap s& s& s" al Este." s&
    ^but$ s& s" un" s&{ s" gran" s" enorme" }s?& s" derrumbe" s&
    (it)_blocks$ s& s" el paso hacia" s&{ s" el Oeste" s" la sierra." }s&
    paragraph
    endof
  east% of
    ^can_see$ s" la salida del bosque." s&
    paragraph
    endof
  west% of
    s" Un gran derrumbe" (it)_blocks$ s& the_pass$ s&
    toward$ s& s" la sierra." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_10% :attributes
  s" gruta de entrada" self% fname!
  self% is_indoor_location
  location_08% 0 0 location_11% 0 0 0 0 self% init_location
  ;attributes
location_10% :description
  sight case
  self% of
    s" El estrecho paso se adentra hacia el Oeste, desde la boca, al Norte."
    paragraph
    endof
  north% of
    s" La boca de la gruta conduce al exterior."
    paragraph
    endof
  east% of
  endof
  uninteresting_direction
  endcase
  ;description
location_11% :attributes
  s" gran lago" self% mname!
  self% is_indoor_location
  0 0 location_10% 0 0 0 0 0 self% init_location
  ;attributes
location_11% :description
  \ Crear ente!!! estancia y aguas
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
    s" la luz" s&
    s{  s" que se filtra" s{ s" del" s" desde el" }s&
        s{ s" procendente" s" que procede" }s s" del" s&
    }s?& s" exterior." s&
    s" No hay" s&{ s" otra" s" más" }s& s" salida que el Este." s&
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
  uninteresting_direction
  endcase
  ;description
location_12% :attributes
  s" salida del paso secreto" self% fname!
  self% is_indoor_location
  0 0 0 location_13% 0 0 0 0 self% init_location
  ;attributes
location_12% :description
  \ Crear ente!!! agua aquí
  sight case
  self% of
    s" Una" s{ s" gran" s" amplia" }s
    s" estancia se abre hacia el Oeste," s&
    s" y se estrecha hasta" s&
    s{ s" morir" s" terminar" }s
    s" , al Este, en una parte de agua." s+
    paragraph
    endof
  east% of
    s{ s" La estancia" s" El lugar" }s
    s" se estrecha hasta " s&
    s{ s" morir" s" terminar" }s
    s" en una parte de agua." s&
    paragraph
  endof
  west% of
    s" Se vislumbra la continuación de la cueva."
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_13% :attributes
  s" puente semipodrido" self% mname!
  self% is_indoor_location
  0 0 location_12% location_14% 0 0 0 0 self% init_location
  ;attributes
location_13% :description
  \ Crear ente!!! canal, agua, lecho(~catre)
  sight case
  self% of
    s" La sala se abre en"
    s{ s" semioscuridad" s" penumbra" }s
    s" a un puente cubierto de podredumbre" s&
    s" sobre el lecho de un canal, de Este a Oeste." s&
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
  uninteresting_direction
  endcase
  ;description
location_14% :attributes
  s" recodo de la cueva" self% mname!
  self% is_indoor_location
  0 location_15% location_13% 0 0 0 0 0 self% init_location
  ;attributes
location_14% :description
  sight case
  self% of
    s" La iridiscente cueva gira de Este a Sur."
    paragraph
    endof
  south% of
    you_glimpse_the_cave$ paragraph
    endof
  east% of
    you_glimpse_the_cave$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_15% :attributes
  s" pasaje arenoso" self% mname!
  self% is_indoor_location
  location_14% location_17% location_16% 0 0 0 0 0 self% init_location
  ;attributes
location_15% :description
  sight case
  self% of
    s" La gruta" goes_down$ s& s" de Norte a Sur" s&
    s" sobre un lecho arenoso." s&
    s" Al Este, un agujero del que llega" s&
    s{ s" algo de luz." s" claridad." }s&
    paragraph
    endof
  north% of
    you_glimpse_the_cave$
    s" La cueva" goes_up$ s& in_that_direction$ s& period+
    paragraph
    endof
  south% of
    you_glimpse_the_cave$
    s" La cueva" goes_down$ s& in_that_direction$ s& period+
    paragraph
    endof
  east% of
    s{ s" La luz" s" Algo de luz" s" Algo de claridad" }s
    s" procede de esa dirección." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_16% :attributes
  s" pasaje del agua" self% mname!
  self% is_indoor_location
  0 0 0 location_15% 0 0 0 0 self% init_location
  ;attributes
location_16% :description
\ pendiente!!! el examen del agua aquí debe dar más pistas
  sight case
  self% of
    s" Como un acueducto, el agua"
    goes_down$ s& s" con gran fuerza de Norte a Este," s&
    s" aunque la salida practicable es la del Oeste." s&
    paragraph
    endof
  north% of
    s" El agua" goes_down$ s& s" con gran fuerza" s& from_that_way$ s& period+
    paragraph
    endof
  east% of
    s" El agua" goes_down$ s& s" con gran fuerza" s& that_way$ s& period+
    paragraph
    endof
  west% of
    s" Es la única salida." paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_17% :attributes
  s" estalactitas" self% fname!
  self% is_indoor_location
  location_15% location_20% location_18% 0 0 0 0 0 self% init_location
  ;attributes
location_17% :description
  \ Crear ente!!! estalactitas
  sight case
  self% of
    s" Muchas estalactitas se agrupan encima de tu cabeza,"
    s" y se abren cual arco de entrada hacia el Este y Sur." s&
    paragraph
    endof
  north% of
    you_glimpse_the_cave$
    paragraph
    endof
  up% of
    s" Las estalactitas se agrupan encima de tu cabeza."
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_18% :attributes
  s" puente de piedra" self% mname!
  self% is_indoor_location
  0 0 location_19% location_17% 0 0 0 0 self% init_location
  ;attributes
location_18% :description
  \ Crear ente!!! puente, arco
  sight case
  self% of
    s" Un arco de piedra se eleva,"
    s{ s" cual" s" como si fuera un" s" a manera de" }s&
    s" puente" s&
    s" que se" s{ s" elevara" s" alzara" }s& s?&
    s{ s" sobre" s" por encima de" }s&
    s" la oscuridad, de Este a Oeste." s&
    s{ s" Hacia" s" En" }s& s" su mitad" s&
    altar% is_known?
    if    s" está" s&
    else  s{ s" hay" s" es posible ver" s" puede verse" }s&
    then  altar% full_name s& period+ paragraph
    endof
  east% of
    s" El arco de piedra se extiende" that_way$ s& period+
    paragraph
    endof
  west% of
    s" El arco de piedra se extiende" that_way$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_19% :attributes
  s" recodo arenoso del canal" self% mname!
  self% is_indoor_location
  0 0 0 location_18% 0 0 0 0 self% init_location
  ;attributes
location_19% :description
  sight case
  self% of
    \ Pendiente!!! Hacer variaciones
    the_water_current$ comma+
    s" que discurre" s?&
    s" de Norte a Este," s& (it)_blocks$ s&
    s" el paso, excepto al Oeste." s&
    s{ s" Al" s" Del" s" Hacia el" s" Proveniente del" s" Procedente del" }s&
    s" fondo" s& s{ s" se oye" s" se escucha" s" puede oírse" }s&
    s" un gran estruendo." s&
    paragraph
    endof
  north% of
    ^water_from_there$ period+ paragraph
    endof
  east% of
    water_that_way$ paragraph
    endof
  west% of
    s" Se puede" to_go_back$ s& toward_the(m)$ s& s" arco de piedra" s& in_that_direction$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_20% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  location_17% location_22% location_25% 0 0 0 0 0 self% init_location
  ;attributes
location_20% :description
  sight no_torch? 0= abs *  case
  self% of
    north% south% east% 3 exits_cave_description paragraph
    endof
  north% of
    cave_exit_description$ paragraph
    endof
  south% of
    cave_exit_description$ paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_21% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  0 location_27% location_23% location_20% 0 0 0 0 self% init_location
  ;attributes
location_21% :description
  sight case
  self% of
    east% west% south% 3 exits_cave_description paragraph
    endof
  south% of
    cave_exit_description$ paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  west% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_22% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  0 location_24% location_27% location_22% 0 0 0 0 self% init_location
  ;attributes
location_22% :description
  sight case
  self% of
    south% east% west% 3 exits_cave_description paragraph
    endof
  south% of
    cave_exit_description$ paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  west% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_23% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  0 location_25% 0 location_21% 0 0 0 0 self% init_location
  ;attributes
location_23% :description
  sight case
  self% of
    west% south% 2 exits_cave_description paragraph
    endof
  south% of
    cave_exit_description$ paragraph
    endof
  west% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_24% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  location_22% 0 location_26% 0 0 0 0 0 self% init_location
  ;attributes
location_24% :description
  sight case
  self% of
    east% north% 2 exits_cave_description paragraph
    endof
  north% of
    cave_exit_description$ paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_25% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  location_22% location_28% location_23% location_21% 0 0 0 0 self% init_location
  ;attributes
location_25% :description
  sight case
  self% of
    north% south% east% west% 4 exits_cave_description paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  west% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_26% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  location_26% 0 location_20% location_27% 0 0 0 0 self% init_location
  ;attributes
location_26% :description
  \ Crear ente!!! pasaje/camino/senda tramo/cueva (en todos los tramos)
  sight case
  self% of
    north% east% west% 3 exits_cave_description paragraph
    endof
  north% of
    cave_exit_description$ paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  west% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_27% :attributes
  s" tramo de cueva" self% mname!
  self% is_indoor_location
  location_27% 0 0 location_25% 0 0 0 0 self% init_location
  ;attributes
location_27% :description
  sight case
  self% of
    north% east% west% 3 exits_cave_description paragraph
    endof
  north% of
    cave_exit_description$ paragraph
    endof
  east% of
    cave_exit_description$ paragraph
    endof
  west% of
    cave_exit_description$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_28% :attributes
  s" amplia estancia" self% fname!
  self% is_indoor_location
  location_26% 0 0 0 0 0 0 0 self% init_location
  ;attributes
location_28% :description
  sight case
  self% of
    \ Crear ente!!! estancia(para todos),albergue y refugio (tras hablar con anciano)
    self% ^full_name s" se extiende de Norte a Este." s&
    leader% conversations?
    if  s" Hace de albergue para los refugiados."
    else  s" Está llen" self% gender_ending+ s" de gente." s&
    then  s&
    flags% is_known?
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
    self% has_east_exit?
    if    s" Es por donde viniste." 
    else  s" Es la única salida."
    then  paragraph
    endof
  east% of
    ^the_refugees$
    self% has_east_exit?
    if    they_let_you_pass$ s&
    else  they_don't_let_you_pass$ s&
    then  period+ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_29% :attributes
  s" espiral" self% fname!
  self% is_indoor_location
  0 0 0 location_28% 0 location_30% 0 0 self% init_location
  ;attributes
location_29% :description
  \ Crear ente!!! escalera/espiral, refugiados
  sight case
  self% of
    s" Cual escalera de caracol gigante,"
    goes_down_into_the_deep$ comma+ s&
    s" dejando a los refugiados al Oeste." s&
    paragraph
    endof
  west% of
    over_there$ s" están los refugiados." s&
    paragraph
    endof
  down% of
    s" La espiral" goes_down_into_the_deep$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_30% :attributes
  s" inicio de la espiral" self% mname!
  self% is_indoor_location
  0 0 location_31% 0 location_29% 0 0 0 self% init_location
  ;attributes
location_30% :description
  sight case
  self% of
    s" Se eleva en la penumbra."
    s" La" s& cave$ s& gets_narrower(f)$ s&
    s" ahora como para una sola persona, hacia el Este." s&
    paragraph
    endof
  east% of
    s" La" cave$ s& gets_narrower(f)$ s& period+
    paragraph
    endof
  up% of
    s" La" cave$ s& s" se eleva en la penumbra." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_31% :attributes
  s" puerta norte" self% fname!
  self% is_indoor_location
  0 0 0 location_30% 0 0 0 0 self% init_location
  ;attributes
location_31% :description
  \ Crear ente!!! arco, columnas, hueco/s(entre rocas)
  sight case
  self% of
    s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
    paragraph
    endof
  north% of
    s" Las rocas"  self% has_north_exit?
    if  (rocks)_on_the_floor$
    else  (they)_block$ the_pass$ s&
    then  s& period+ paragraph
    endof
  west% of
    ^that_way$ s" se encuentra el inicio de la espiral." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_32% :attributes
  s" precipicio" self% mname!
  self% is_indoor_location
  0 location_33% 0 location_31% 0 0 0 0 self% init_location
  ;attributes
location_32% :description
  \ Crear ente!!! precipicio, abismo, cornisa, camino, roca/s
  sight case
  self% of
    s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
    s" El soporte de roca gira en forma de «U» de Oeste a Sur." s&
    paragraph
    endof
  south% of
    ^the_path$ s" gira" s& that_way$ s& period+
    paragraph
    endof
  west% of
    ^the_path$ s" gira" s& that_way$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_33% :attributes
  s" pasaje de salida" self% mname!
  self% is_indoor_location
  location_32% 0 location_34% 0 0 0 0 0 self% init_location
  ;attributes
location_33% :description
  \ Crear ente!!! camino/paso/sendero
  sight case
  self% of
    s" El paso se va haciendo menos estrecho a medida que se avanza hacia el Sur, para entonces comenzar hacia el Este."
    paragraph
    endof
  north% of
    ^the_path$ s" se estrecha" s& that_way$ s& period+
    paragraph
    endof
  south% of
    ^the_path$ gets_wider$ s& that_way$ s&
    s" y entonces gira hacia el Este." s&
    paragraph
    endof
  east% of
    ^the_path$ gets_wider$ s& that_way$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_34% :attributes
  \ Crear ente!!! gravilla
  s" pasaje de gravilla" self% mname!
  self% is_indoor_location
  location_35% 0 0 location_33% 0 0 0 0 self% init_location
  ;attributes
location_34% :description
  \ Crear ente!!! camino/paso/sendero, guijarros, moho, roca, suelo...
  sight case
  self% of
    s" El paso" gets_wider$ s& s" de Oeste a Norte," s&
    s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
    paragraph
    endof
  north% of
    ^the_path$ gets_wider$ s& that_way$ s& period+
    paragraph
    endof
  west% of
    ^the_path$ s" se estrecha" s& that_way$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_35% :attributes
  s" puente sobre el acueducto" self% mname!
  self% is_indoor_location
  location_40% location_34% 0 location_36% 0 location_36% 0 0 self% init_location
  ;attributes
location_35% :description
  \ Crear ente!!! escaleras, puente, río/curso/agua
  sight case
  self% of
    s" Un puente" s{ s" se tiende" s" cruza" }s& s" de Norte a Sur sobre el curso del agua." s&
    s" Unas resbaladizas escaleras" s& (they)_go_down$ s& s" hacia el Oeste." s&
    paragraph
    endof
  north% of
    bridge_that_way$ paragraph
    endof
  south% of
    bridge_that_way$ paragraph
    endof
  west% of
    stairway_to_river$ paragraph
    endof
  down% of
    stairway_to_river$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_36% :attributes
  s" remanso" self% mname!
  self% is_indoor_location
  0 0 location_35% location_37% location_35% 0 0 0 self% init_location
  ;attributes
location_36% :description
  sight case
  self% of
    s" Una" s{ s" ruidosa" s" estruendosa" s" ensordecedora" }s&
    s" corriente" s& goes_down$ s&
    s{ s" con" s" siguiendo" }s& s" el" s& pass_way$ s&
    s" elevado desde el Oeste, y forma un meandro arenoso." s&
    s" Unas escaleras" s& (they)_go_up$ s& toward_the(m)$ s& s" Este." s&
    paragraph
    endof
  east% of
    stairway_that_way$ paragraph
    endof
  west% of
    ^water_from_there$ period+ paragraph
    endof
  up% of
    stairway_that_way$ paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_37% :attributes
  s" canal de agua" self% mname!
  self% is_indoor_location
  0 0 location_36% location_38% 0 0 0 0 self% init_location
  ;attributes
location_37% :description
  sight case
  self% of
    s" El agua" goes_down$ s& s" por un canal" s?&
    from_the(m)$ s& s" Oeste con" s&
    s{ s" renovadas fuerzas" s" renovada energía" s" renovado ímpetu" }s& comma+
    s" dejando" s& s{
    s" a un lado" a_high_narrow_pass_way$ s&
    a_high_narrow_pass_way$ s{ s" lateral" s" a un lado" }s&
    }s& s" que" s& lets_you$ s& to_keep_going$ s&
    toward_the(m)$ s" Este" s&
    toward_the(m)$ s" Oeste" s& rnd2swap s" o" s& 2swap s& s&
    period+ paragraph
    endof
  east% of
    ^the_pass_way$ s" elevado" s?& lets_you$ s& to_keep_going$ s& that_way$ s& period+
    paragraph
    endof
  west% of
    water_from_there$
    the_pass_way$ s" elevado" s?& lets_you$ s& to_keep_going$ s& that_way$ s&
    both s" también" s& ^uppercase period+
    paragraph 
    endof
  uninteresting_direction
  endcase
  ;description
location_38% :attributes
  s" gran cascada" self% fname!
  self% is_indoor_location
  0 0 location_37% location_39% 0 0 0 0 self% init_location
  ;attributes
location_38% :description
  sight case
  self% of
    s" Cae el agua hacia el Este,"
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
    water_that_way$ paragraph
    endof
  west% of
    \ pendiente!!! el artículo de «cascada» debe depender también de si se ha visitado el escenario 39 o este mismo 38
    ^water_from_there$
    s" , de" s+ waterfall% full_name s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_39% :attributes
  s" interior de la cascada" self% mname!
  self% is_indoor_location
  0 0 location_38% 0 0 0 0 0 self% init_location
  ;attributes
location_39% :description
  sight case
  self% of
    \ Crear ente!!! musgo, cortina, agua, hueco
    s" Musgoso y rocoso, con la cortina de agua"
    s{ s" tras de ti," s" a tu espalda," }s&
    s{ s" el nivel" s" la altura" }s& s" del agua ha" s&
    s{ s" subido" s" crecido" }s&
    s{ s" un poco" s" algo" }s& s" en este" s&
    s{ s" curioso" s" extraño" }s& s" hueco." s&
    paragraph
    endof
  east% of
    \ pendiente!!! variar
    s" Es la única salida." paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_40% :attributes
  s" explanada" self% fname!
  self% is_indoor_location
  0 location_35% location_41% 0 0 0 0 0 self% init_location
  ;attributes
location_40% :description
  \ Crear ente!!! losas y losetas, estalactitas, panorama, escalones
  sight case
  self% of
    s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
    s" Unos casi imperceptibles escalones conducen al Este." s&
    paragraph
    endof
  south% of
    ^that_way$ s" se va" s& toward_the(m)$ s& s" puente." s&
    paragraph
    endof
  east% of
    s" Los escalones" (they)_lead$ s& that_way$ s& period+
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
  uninteresting_direction
  endcase
  ;description
location_41% :attributes
  \ Pendiente!!! Cambiar el nombre. No se puede pasar a mayúscula un carácter pluriocteto en UTF-8.
  self% is_indoor_location
  s" ídolo" self% mname!
  0 0 0 location_40% 0 0 0 0 self% init_location
  ;attributes
location_41% :description
  \ Crear ente!!! roca, centinela
  sight case
  self% of
    s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al Sur."
    s" Se puede" s& to_go_back$ s& toward$ s& s" la explanada hacia el Oeste." s&
    paragraph
    endof
  south% of
    s" Hay una" s" roca" s" enorme" rnd2swap s& s&
    that_way$ s& period+
    paragraph
    endof
  west% of
    s" Se puede volver" toward$ s& s" la explanada" s& that_way$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_42% :attributes
  s" pasaje estrecho" self% mname!
  self% is_indoor_location
  location_41% location_43% 0 0 0 0 0 0 self% init_location
  ;attributes
location_42% :description
  sight case
  self% of
    s" Como un pasillo que corteja el canal de agua, a su lado, baja de Norte a Sur."
    paragraph
    endof
  north% of
    ^the_pass_way$ goes_up$ s& that_way$ s&
    s" , de donde" s{ s" corre" s" procede" s" viene" s" proviene" }s& s" el agua." s& s+
    paragraph
    endof
  south% of
    ^the_pass_way$ goes_down$ s& that_way$ s&
    s" , siguiendo el canal de agua," s+
    s" hacia un lugar en que" s&
    s{ s" se aprecia" s" puede apreciarse" s" se distingue" }s&
    s" un aumento de luz." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_43% :attributes
  s" pasaje de la serpiente" self% mname!
  self% is_indoor_location
  location_42% 0 0 0 0 0 0 0 self% init_location
  ;attributes
location_43% :description
  sight case
  self% of
    ^the_pass_way$ s" sigue de Norte a Sur." s&
    paragraph
    endof
  north% of
    ^the_pass_way$ s" continúa" s& that_way$ s& period+
    paragraph
    endof
  south% of
    snake% is_here?
    if  a_snake_blocks_the_way$
    else  ^the_pass_way$ s" continúa" s& that_way$ s&
    then  period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_44% :attributes
  s" lago interior" self% mname!
  self% is_indoor_location
  location_43% 0 0 location_45% 0 0 0 0 self% init_location
  ;attributes
location_44% :description
  \ Crear ente!!! lago, escaleras, pasaje, lago
  sight case
  self% of
    s" Unas escaleras" s{ s" dan" s" permiten el" }s& s{ s" paso" s" acceso" }s&
    s" a un" s& beautiful(m)$ s& s" lago interior, hacia el Oeste." s&
    s" Al Norte, un oscuro y"
    narrow(m)$ s& pass_way$ s& goes_up$ s& period+ s?&
    paragraph
    endof
  north% of
    s" Un pasaje oscuro y" narrow(m)$ s& goes_up$ s& that_way$ s& period+
    paragraph
    endof
  west% of
    s" Las escaleras" (they)_lead$ s& that_way$ s& s" , hacia el lago" s?+ period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_45% :attributes
  s" cruce de pasajes" self% mname!
  self% is_indoor_location
  0 location_47% location_44% location_46% 0 0 0 0 self% init_location
  ;attributes
location_45% :description
  \ Crear ente!!! pasaje/camino/paso/senda
  sight case
  self% of
    ^narrow(mp)$ pass_ways$ s&
    s" permiten ir al Oeste, al Este y al Sur." s&
    paragraph
    endof
  south% of
    ^a_narrow_pass_way$ s" permite ir" s& that_way$ s&
    s" , de donde" s+ s{ s" proviene" s" procede" }s&
    s{ s" una gran" s" mucha" }s& s" luminosidad." s&
    paragraph
    endof
  west% of
    ^a_narrow_pass_way$ leads$ s& that_way$ s& period+
    paragraph
    endof
  east% of
    ^a_narrow_pass_way$ leads$ s& that_way$ s& period+
    s" , de donde" s{ s" proviene" s" procede" }s&
    s{ s" algo de" s" una poca" s" un poco de" }s&
    s{ s" claridad" s" luz" }s& period+ s+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_46% :attributes
  s" hogar de Ambrosio" self% mname!
  self% is_indoor_location
  0 0 location_45% 0 0 0 0 0 self% init_location
  ;attributes
location_46% :description
  sight case
  self% of
    s" Un catre, algunas velas y una mesa es todo lo que"
    s{ s" tiene" s" posee" }s s" Ambrosio" rnd2swap s& s&
    period+  paragraph
    endof
  east% of
    s" La salida"
    s{ s" de la casa" s" del hogar" }s s" de Ambrosio" s& s?&
    s" está" s& that_way$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_47% :attributes
  s" salida de la cueva" self% fname!
  self% is_indoor_location
  location_45% 0 0 0 0 0 0 0 self% init_location
  ;attributes
location_47% :description
  \ Descripción inacabada!!! 
  sight case
  self% of
    s" Por el Oeste,"
    door% full_name s& door% «open»|«closed» s& comma+
    door% is_open? if  \ La puerta está abierta
      s" por la cual entra la luz que ilumina la estancia," s&
      s" permite salir de la cueva." s&
    else  \ La puerta está cerrada
      s" al otro lado de la cual se adivina la luz diurna," s&
      door% is_known?
      if    s" impide" s&
      else  s" parece ser" s&
      then  s" la salida de la cueva." s&
    then
    paragraph
    endof
  north% of
    \ pendiente!!! variar
    s" Hay salida" that_way$ s& period+ paragraph
    endof
  west% of
    \ pendiente!!! variar
    door% is_open? if
      s" La luz diurna entra por la puerta."
    else  
      s" Se adivina la luz diurna al otro lado de la puerta."
    then
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_48% :attributes
  s" bosque a la entrada" self% mname!
  0 0 location_47% location_49% 0 0 0 0 self% init_location
  ;attributes
: when_the_door$  ( -- a u )
  s" cuando" s{ s" la" s" su" }s& s" puerta" s&
  ;
: like_now$+  ( a1 u1 -- a1 u1 | a2 u2 )
  s" , como ahora" s?+
  ;
location_48% :description
  \ Crear ente!!! cueva
  sight case
  self% of
    s{ s" Apenas" s" Casi no" }s
    s{ s" se puede" s" es posible" }s&
    s" reconocer la entrada de la cueva, al Este." s&
    ^the_path$ s& s{ s" parte" s" sale" }s&
    s" del bosque hacia el Oeste." s&
    paragraph
    endof
  east% of
    s" La entrada de la cueva" s{
    s" está" s" bien" s?& s{ s" camuflada" s" escondida" }s&
    s" apenas se ve" s" casi no se ve" s" pasa casi desapercibida"
    }s& comma+
    door% is_open? if
      even$ s& when_the_door$ s&
      s{ s" está abierta" s" no está cerrada" }s& like_now$+
    else
      s{ s" especialmente" s" sobre todo" }s& when_the_door$ s&
      s{ s" no está abierta" s" está cerrada" }s& like_now$+
    then  period+ paragraph
    endof
  west% of
    ^the_path$ s{ s" parte" s" sale" }s& s" del bosque" s& in_that_direction$ s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_49% :attributes
  s" sendero del bosque" self% mname!
  0 0 location_48% location_50% 0 0 0 0 self% init_location
  ;attributes
location_49% :description
  sight case
  self% of
    ^the_path$ s" recorre" s& s" toda" s?&
    s" esta" s& s{ s" parte" s" zona" }s&
    s" del bosque de Este a Oeste." s&
    paragraph
    endof
  east% of
    ^the_path$ leads$ s&
    s" al bosque a la entrada de la cueva." s&
    paragraph
    endof
  west% of
    ^the_path$ s" continúa" s& in_that_direction$ s& period+
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_50% :attributes
  s" camino norte" self% mname!
  0 location_51% location_49% 0 0 0 0 0 self% init_location
  ;attributes
location_50% :description
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
    ^can_see$ s" el sendero del bosque." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description
location_51% :attributes
  s" Westmorland" self% fname!
  self% has_no_article
  location_50% 0 0 0 0 0 0 0 self% init_location
  ;attributes
location_51% :description
  \ Crear ente!!! mercado, plaza, villa, pueblo, castillo
  sight case
  self% of
    ^the_village$ s" bulle de actividad con el mercado en el centro de la plaza," s&
    s" donde se encuentra el castillo." s&
    paragraph
    endof
  north% of
    s" El camino norte" of_the_village$ s& leads$ s& s" hasta el bosque." s&
    paragraph
    endof
  uninteresting_direction
  endcase
  ;description

\ Entes globales

cave% :attributes
  s" cueva" self% fname!
  \ self% is_global_indoor \ !!!
  ;attributes
cave% :description
  \ Provisional!!!
  s" La cueva es chachi."
  paragraph
  ;description
ceiling% :attributes
  s" techo" self% mname!
  self% is_global_indoor
  ;attributes
ceiling% :description
  \ Provisional!!!
  s" El techo es muy bonito."
  paragraph
  ;description
clouds% :attributes
  s" nubes" self% fnames!
  self% is_global_outdoor
  ;attributes
clouds% :description
  \ Pendiente!!!:
  \ Distinguir no solo interiores, sino escenarios en
  \ que se puede vislumbrar el exterior.
  \ Provisional!!!:
  s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
  s" en la estación del frío se han alejado por el momento. " s&
  2 random if  paragraph  else  2drop sky% describe  then  \ comprobar!!!
  ;description
floor% :attributes
  s" suelo" self% mname!
  self% is_global_indoor
  self% is_global_outdoor
  ;attributes
floor% :description
  \ Provisional!!!
  am_i_outdoor? if
    s" El suelo fuera es muy bonito."
    paragraph
  else
    s" El suelo dentro es muy bonito."
    paragraph
  then
  ;description
sky% :attributes
  s" cielo" self% mname!
  self% is_global_outdoor
  ;attributes
sky% :description
  \ Provisional!!!
  s" [El cielo es mu bonito]"
  paragraph
  ;description
wall% :attributes
  s" pared" self% mname!
  self% is_global_indoor
  ;attributes
wall% :description
  \ Provisional!!!
  s" [La pared es mu bonita]"
  paragraph
  ;description

\ Entes virtuales

exits% :attributes
  s" salida" self% fname!
  self% is_global_outdoor
  self% is_global_indoor
  ;attributes
exits% :description
  list_exits
  ;description
inventory% :attributes
  ;attributes
enemy% :attributes
  \ Inacabado!!!
  s" enemigos" self% mnames!
  self% is_human
  self% is_decoration
  ;attributes
enemy% :description
  \ Inacabado!!!
  battle# @ if
    s" Enemigo en batalla!!!"  \ tmp!!!
  else
    s" Enemigo en paz!!!"  \ tmp!!!
  then  paragraph
  ;description

\ Entes dirección

\ Los entes dirección guardan en su campo '~direction'
\ el desplazamiento correspodiente al campo de 
\ dirección que representan 
\ Esto sirve para reconocerlos como tales entes dirección 
\ (pues todos los valores posibles son diferentes de cero)
\ y para hacer los cálculos en las acciones de movimiento.

north% :attributes
  s" Norte" self% mname!
  self% has_definite_article
  north_exit> self% ~direction !
  ;attributes
south% :attributes
  s" Sur" self% mname!
  self% has_definite_article
  south_exit> self% ~direction !
  ;attributes
east% :attributes
  s" Este" self% mname!
  self% has_definite_article
  east_exit> self% ~direction !
  ;attributes
west% :attributes
  s" Oeste" self% name!
  self% has_definite_article
  west_exit> self% ~direction !
  ;attributes
up% :attributes
  s" arriba" self% name!
  self% has_no_article
  up_exit> self% ~direction !
  ;attributes
up% :description
  am_i_outdoor?
  if  sky% describe
  else  ceiling% describe
  then
  ;description
down% :attributes
  s" abajo" self% name!
  self% has_no_article
  down_exit> self% ~direction !
  ;attributes
down% :description
  \ Provisional!!!
  am_i_outdoor? if  
    s" El suelo exterior es muy bonito." paragraph
  else
    s" El suelo interior es muy bonito." paragraph
  then
  ;description

out% :attributes
  s" afuera" self% name!
  self% has_no_article
  out_exit> self% ~direction !
  ;attributes
in% :attributes
  s" adentro" self% name!
  self% has_no_article
  in_exit> self% ~direction !
  ;attributes
  
\ }}} ##########################################################
section( Errores de las acciones)  \ {{{

variable action  \ Código de la acción del comando

\ Entes complemento:
variable main_complement  \ Principal (complemento directo o destino)
variable secondary_complement  \ Secundario (complemento indirecto, destino u origen)
defer tool_complement  \ Herramienta
false [if]  \ Descartado!!! Pendiente!!!
variable tool_complement  \ Herramienta 
variable to_complement  \ Destino \ No utilizado!!!
variable from_complement  \ Origen \ No utilizado!!!
variable into_complement  \ Destino dentro \ No utilizado!!!
[then]

\ Ente que ha provocado un error
\ y puede ser citado en el mensaje de error correspondiente:
variable what

\ Código de la (seudo)preposición abierta, o cero:
variable current_preposition

: known_entity_is_not_here$  ( a -- a1 u1 )
  \  Devuelve mensaje de que un ente conocido no está presente.
  full_name s" no está" s&
  s{ s" aquí" s" por aquí" }s& 
  ;
: unknown_entity_is_not_here$  ( a -- a1 u1 )
  \  Devuelve mensaje de que un ente desconocido no está presente
  s{ s" Aquí" s" Por aquí" }s
  s" no hay" s&
  rot subjective_negative_name s&
  ;
: is_not_here  ( a -- )
  \  Informa de que un ente no está presente.
  dup familiar
  if  known_entity_is_not_here$
  else  unknown_entity_is_not_here$
  then  period+ narrate
  ;
' is_not_here constant (is_not_here_error#)
' (is_not_here_error#) is is_not_here_error#
: is_not_here_what
  \  Informa de que el ente 'what' no está presente.
  what @ is_not_here
  ;
' is_not_here_what constant (is_not_here_what_error#)
' (is_not_here_what_error#) is is_not_here_what_error#
: cannot_see  ( a -- )
  \ Informa de que un ente no puede ser mirado.
  ^cannot_see$
  rot subjective_negative_name_as_direct_object s&
  period+ narrate
  ;
' cannot_see constant (cannot_see_error#)
' (cannot_see_error#) is cannot_see_error#
: cannot_see_what
  \ Informa de que el ente 'what' no puede ser mirado.
  what @ cannot_see
  ;
' cannot_see_what constant (cannot_see_what_error#)
' (cannot_see_what_error#) is cannot_see_what_error#
: like_that$  ( -- a u )
  \ No se usa!!!
  s{ s" así" s" como eso" }s
  ;
: something_like_that$  ( -- a u )
  \ Devuelve una variante de «hacer eso».
  s" hacer" s?
  s{ s" algo así"
  s" algo semejante"
  s" eso"
  s" semejante cosa"
  s" tal cosa"
  s" una cosa así" }s&
  ;
: is_impossible$  ( -- a u )
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
  }s
  ;
: ^is_impossible$  ( -- a u )
  \ Devuelve una variante de «Es imposible» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción.
  is_impossible$ ^uppercase
  ;
: x_is_impossible$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «X es imposible».
  dup
  if  ^uppercase is_impossible$ s&
  else  2drop ^is_impossible$
  then
  ;
: it_is_impossible_x$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «Es imposible x».
  ^is_impossible$ 2swap s& 
  ;
: is_impossible  ( a u -- )
  \ Informa de que una acción indicada (en infinitivo) es imposible.
  \ a u = Acción imposible, en infinitivo, o una cadena vacía
  ['] x_is_impossible$
  ['] it_is_impossible_x$
  2 choose execute  period+ narrate
  ;
: impossible
  \ Informa de que una acción no especificada es imposible.
  [debug] [if]  s" En IMPOSSIBLE" debug  [then]  \ Depuración!!!
  something_like_that$ is_impossible
  ;
' impossible constant (impossible_error#)
' (impossible_error#) is impossible_error#
: try$  ( -- a u )
  \ Devuelve una variante de «intentar» (o vacía).
  s{ "" "" s" intentar" }s
  ;
: nonsense$  ( -- a u )
  \ Devuelve una variante de «no tiene sentido»,
  \ que formará parte de mensajes personalizados por cada acción.
  \ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos
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
  }s
  ;
: ^nonsense$  ( -- a u )
  \ Devuelve una variante de «No tiene sentido»
  \ (con la primera letra en mayúsculas)
  \ que formará parte de mensajes personalizados por cada acción.
  nonsense$ ^uppercase
  ;
: x_is_nonsense$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «X no tiene sentido».
  dup
  if  try$ 2swap s& ^uppercase nonsense$ s&
  else  2drop ^nonsense$
  then
  ;
: it_is_nonsense_x$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «No tiene sentido x».
  ^nonsense$ try$ s& 2swap s& 
  ;
: is_nonsense  ( a u -- )
  \ Informa de que una acción dada no tiene sentido.
  \ a u = Acción que no tiene sentido;
  \       es un verbo en infinitivo, un sustantivo o una cadena vacía
  ['] x_is_nonsense$
  ['] it_is_nonsense_x$ 
  2 choose execute  period+ narrate
  ;
: nonsense
  \ Informa de que alguna acción no especificada no tiene sentido.
  \ Provisional!!!
  [debug] [if]  s" En NONSENSE" debug  [then]  \ Depuración!!!
  s" eso" is_nonsense 
  ;
' nonsense constant (nonsense_error#)
' (nonsense_error#) is nonsense_error#
: dangerous$  ( -- a u )
  \ Devuelve una variante de «es peligroso», que formará parte de mensajes personalizados por cada acción.
  \ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos y unificar 
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
  }s
  ;
: ^dangerous$  ( -- a u )
  \ Devuelve una variante de «Es peligroso» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción.
  dangerous$ ^uppercase
  ;
: x_is_dangerous$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «X es peligroso».
  dup
  if  try$ 2swap s& ^uppercase dangerous$ s&
  else  2drop ^dangerous$
  then
  ;
: it_is_dangerous_x$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «Es peligroso x».
  ^dangerous$ try$ s& 2swap s& 
  ;
: is_dangerous  ( a u -- )
  \ Informa de que una acción dada (en infinitivo)
  \ no tiene sentido.
  \ a u = Acción que no tiene sentido, en infinitivo, o una cadena vacía
  ['] x_is_dangerous$
  ['] it_is_dangerous_x$ 
  2 choose execute  period+ narrate
  ;
: dangerous
  \ Informa de que alguna acción no especificada no tiene sentido.
  something_like_that$ is_dangerous
  ;
' dangerous constant (dangerous_error#)
' (dangerous_error#) is dangerous_error#
: ?full_name&  ( a1 u1 a2 -- )
  \ Añade a una cadena el nombre de un posible ente.
  \ No se usa!!!
  \ a1 u1 = Cadena
  \ a2 = Ente (o cero)
  ?dup if  full_name s&  then
  ;
: +is_nonsense  ( a u a1 -- )
  \ Informa de que una acción dada (en infinitivo)
  \ ejecutada sobre un ente no tiene sentido.
  \ a u = Acción en infinitivo
  \ a1 = Ente al que se refiere la acción y cuyo objeto directo es (o cero)
  ?dup
  if full_name s& is_nonsense
  else  2drop nonsense
  then
  ;
: main_complement+is_nonsense  ( a u -- )
  \ Informa de que una acción dada (en infinitivo),
  \ que hay que completar con el nombre del complemento principal,
  \ no tiene sentido.
  \ a u = Acción que no tiene sentido, en infinitivo
  main_complement @ +is_nonsense
  ;
: secondary_complement+is_nonsense  ( a u -- )
  \ Informa de que una acción dada (en infinitivo),
  \ que hay que completar con el nombre del complemento secundario,
  \ no tiene sentido.
  \ a u = Acción que no tiene sentido, en infinitivo
  secondary_complement @ +is_nonsense
  ;
: no_reason_for$  ( -- a u )
  \ Devuelve una variante de «no hay motivo para».
  \ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos
  s" No hay" s{
  s" motivo" s" alguno" s?& s" para"
  s" nada que justifique"
  s" necesidad" s" alguna" s?& s" de" s&
  s" ninguna necesidad de"
  s" ninguna razón para"
  s" ningún motivo para"
  s" razón" s" alguna" s?& s" para"
  }s&
  ;
: no_reason_for_that  ( a u -- )
  \ Informa de que no hay motivo para una acción (en infinitivo).
  \ a u = Acción para la que no hay razón, en infinitivo, o una cadena vacía
  \ Pendiente!!!
  no_reason_for$ 2swap s& period+ narrate
  ;
: no_reason
  \ Informa de que no hay motivo para una acción no especificada.
  \ Pendiente!!!
  something_like_that$ no_reason_for_that
  ;
: nonsense|no_reason
  \ Informa de que una acción no especificada no tiene sentido o no tiene motivo.
  \ No se usa todavía!!!
  ['] nonsense
  ['] no_reason
  2 choose execute
  ;
variable silent_well_done?  \ No se usa todavía!!!
: well_done
  \ Informa de que una acción se ha realizado.
  silent_well_done? @ 0=
  if  s" Hecho." narrate  then
  silent_well_done? off
  ;
: (do_not_worry_0)$  ( -- a u)
  \ Primera versión posible del mensaje de 'do_not_worry'.
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
  }s&
  ;
: (do_not_worry_1)$  ( -- a u)
  \ Segunda versión posible del mensaje de 'do_not_worry'.
  \ Pendiente!!!: Consultar «menester»
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
  }s&
  ;
: do_not_worry
  \ Informa de que una acción no tiene importancia.
  \ Provisional!!! No se usa!!!
  ['] (do_not_worry_0)$
  ['] (do_not_worry_1)$ 2 choose execute
  now_$ s&  period+ narrate
  ;

: unnecessary_tool_for_that  ( a1 u1 a2 -- )
  \ Informa de que un ente es innecesario como herramienta
  \ para ejecutar una acción.
  \ a1 u1 = Acción (una frase con verbo en infinitivo)
  \ a2 = Ente innecesario
  \ Inacabado!!!
  full_name s" No necesitas" 2swap s& s" para" s& 2swap s&
  period+ narrate
  ;
' unnecessary_tool_for_that constant (unnecessary_tool_for_that_error#)
' (unnecessary_tool_for_that_error#) is unnecessary_tool_for_that_error#
: unnecessary_tool  ( a -- )
  \ Informa de que un ente es innecesario como herramienta
  \ para ejecutar una acción sin especificar.
  \ a = Ente innecesario
  \ Inacabado!!! Añadir variante "no es/son necesaria/o/s
  \ Inacabado!!! Ojo con entes especiales: personas, animales, virtuales...
  ['] full_name ['] negative_full_name 2 choose execute
  s" No" s{ s" hace falta"
  s" necesitas" s" se necesita"
  s" precisas" s" se precisa"
  s" hay necesidad de" s{ s" usar" s" emplear" s" utilizar" }s?&
  }s&  2swap s&
  s{ s" para nada" s" para eso" }s?&  period+ narrate
  \ Inacabado!!! Añadir coletilla "aunque la/lo/s tuvieras"?
  ;
' unnecessary_tool constant (unnecessary_tool_error#)
' (unnecessary_tool_error#) is unnecessary_tool_error#

0 [if]  \ Error «no tiene nada especial», inacabado!!! 

: it_is_normal_x$  ( a1 u1 -- a2 u2 )
  \ Devuelve una variante de «no tiene nada especial x».
  ^normal$ try$ s& 2swap s& 
  ;
: is_normal  ( a -- )
  \ Informa de que un ente no tiene nada especial.
  \ a u = Acción que no tiene nada especial; es un verbo en infinitivo, un sustantivo o una cadena vacía
  ['] x_is_normal$
  ['] it_is_normal_x$ 
  2 choose execute  period+ narrate
  ;
' is_normal constant (is_normal_error#)
' (is_normal_error#) is is_normal_error#

[then]

: that$  ( a -- a1 u1 )
  \  Devuelve el nombre de un ente, o un pronombre demostrativo.
  2 random
  if  drop s" eso"  else  full_name  then
  ;
: you_do_not_have_it_(0)$  ( a -- )
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 0).
  s" No" you_carry$ s& rot that$ s& with_you$ s&
  ;
: you_do_not_have_it_(1)$  ( a -- )
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 1, solo para entes conocidos).
  s" No" rot direct_pronoun s& you_carry$ s& with_you$ s&
  ;
: you_do_not_have_it_(2)$  ( a -- )
  \ Devuelve mensaje de que el protagonista no tiene un ente
  \ (variante 2, solo para entes no citados en el comando).
  s" No" you_carry$ s& rot full_name s& with_you$ s&
  ;
: you_do_not_have_it  ( a -- )
  \ Informa de que el protagonista no tiene un ente.
  dup is_known? if
    ['] you_do_not_have_it_(0)$
    ['] you_do_not_have_it_(1)$
    2 choose execute
  else  you_do_not_have_it_(0)$
  then  period+ narrate
  ;
' you_do_not_have_it constant (you_do_not_have_it_error#)
' (you_do_not_have_it_error#) is you_do_not_have_it_error#
: you_do_not_have_what
  \ Informa de que el protagonista no tiene el ente 'what'.
  what @ you_do_not_have_it
  ;
' you_do_not_have_what constant (you_do_not_have_what_error#)
' (you_do_not_have_what_error#) is you_do_not_have_what_error#

: it_seems$  ( -- a u )
  s{ "" s" parece que" s" por lo que parece," }s
  ;
: it_is$  ( -- a u )
  s{ s" es" s" sería" s" será" }s
  ;
: to_do_it$  ( -- a u )
  s" hacerlo" s?
  ;
: possible_to_do$  ( -- a u )
  it_is$ s" posible" s& to_do_it$ s&
  ;
: impossible_to_do$  ( -- a u )
  it_is$ s" imposible" s& to_do_it$ s&
  ;
: can_be_done$  ( -- a u )
  s{ s" podrá" s" podría" s" puede" }s
  s" hacerse" s&
  ;
: can_not_be_done$  ( -- a u )
  s" no" can_be_done$ s&
  ;
: only_by_hand$  ( -- a u )
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
  }s
  ;
: not_by_hand_0$  ( -- a u )
  \ Devuelve la primera versión del mensaje de NOT_BY_HAND.
  it_seems$
  s{
    s" no" possible_to_do$ s&
    impossible_to_do$
    can_not_be_done$
  }s&
  only_by_hand$ s& period+ ^uppercase
  ;
: some_tool$  ( -- a u )
  s{
  s{ s" la" s" alguna" s" una" }s s" herramienta" s&
  s{ s" adecuada" s" apropiada" }s&
  s{ s" el" s" algún" s" un" }s s" instrumento" s&
  s{ s" adecuado" s" apropiado" }s&
  }s
  ;
: not_by_hand_1$  ( -- a u )
  \ Devuelve la segunda versión del mensaje de NOT_BY_HAND.
  it_seems$
  s{
    s{ s" hará" s" haría" s" hace" }s s" falta" s&
    s{ 
      s{ s" será" s" sería" s" es" }s s" menester" s&
      s{ s" habrá" s" habría" s" hay" }s s" que" s&
    }s{ s" usar" s" utilizar" s" emplear" }s&
  }s& some_tool$ s& period+ ^uppercase
  ;
: not_by_hand$  ( -- a u )
  \ Devuelve mensaje de NOT_BY_HAND.
  ['] not_by_hand_0$
  ['] not_by_hand_1$
  2 choose execute ^uppercase
  ;
: not_by_hand
  \ Informa de que la acción no puede hacerse sin una herramienta.
  not_by_hand$ narrate
  ;
: you_need  ( a -- )
  \ Informa de que el protagonista no tiene un ente necesario.
  2 random
  if  you_do_not_have_it_(2)$ period+ narrate
  else  drop not_by_hand
  then
  ;
: you_need_what
  \ Informa de que el protagonista no tiene el ente WHAT necesario.
  what @ you_need
  ;
' you_need_what constant (you_need_what_error#)
' (you_need_what_error#) is you_need_what_error#
: you_already_have_it_(0)$  ( a -- )
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 0).
  s" Ya" you_carry$ s& rot that$ s& with_you$ s&
  ;
: you_already_have_it_(1)$  ( a -- )
  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 1, solo para entes conocidos).
  s" Ya" rot direct_pronoun s& you_carry$ s& with_you$ s&
  ;
: you_already_have_it  ( a -- )
  \ Informa de que el protagonista ya tiene un ente.
  dup familiar over belongs_to_protagonist? or   if
    ['] you_already_have_it_(0)$
    ['] you_already_have_it_(1)$
    2 choose execute
  else  you_already_have_it_(0)$
  then  period+ narrate
  ;
' you_already_have_it constant (you_already_have_it_error#)
' (you_already_have_it_error#) is you_already_have_it_error#
: you_already_have_what  ( a -- )
  \ Informa de que el protagonista ya tiene el ente 'what'.
  what @ you_already_have_it
  ;
' you_already_have_what constant (you_already_have_what_error#)
' (you_already_have_what_error#) is you_already_have_what_error#
: (you_do_not_wear_it)  ( a -- )
  \ Informa de que el protagonista no lleva puesto un ente prenda.
  >r s" No llevas puest" r@ noun_ending+
  r> full_name s& period+ narrate
  ;
: you_do_not_wear_it  ( a -- )
  \ Informa de que el protagonista no lleva puesto un ente prenda, según lo lleve o no consigo.
  dup is_hold?
  if  you_do_not_have_it
  else  (you_do_not_wear_it) 
  then
  ;
: you_do_not_wear_what
  \ Informa de que el protagonista no lleva puesto el ente 'what', según lo lleve o no consigo.
  what @ you_do_not_wear_it
  ;
' you_do_not_wear_what constant (you_do_not_wear_what_error#)
' (you_do_not_wear_what_error#) is you_do_not_wear_what_error#
: you_already_wear_it  ( a -- )
  \ Informa de que el protagonista lleva puesto un ente prenda.
  >r s" Ya llevas puest" r@ noun_ending+
  r> full_name s& period+ narrate
  ;
: you_already_wear_what
  \ Informa de que el protagonista lleva puesto el ente 'what'.
  what @ you_already_wear_it
  ;
' you_already_wear_what constant (you_already_wear_what_error#)
' (you_already_wear_what_error#) is you_already_wear_what_error#
: not_with_that$  ( -- a u )
  \ Devuelve mensaje de NOT_WITH_THAT.
  s" Con eso no..." 
  s" No con eso..." 
  2 schoose
  ;
: not_with_that
  \ Informa de que la acción no puede hacerse con la herramienta elegida.
  not_with_that$ narrate
  ;
: it_is_already_open  ( a -- )
  \ Informa de que un ente ya está abierto.
  s" Ya está abiert" rot noun_ending+ period+ narrate
  ;
: what_is_already_open
  \ Informa de que el ente WHAT ya está abierto.
  what @ it_is_already_open
  ;
' what_is_already_open constant (what_is_already_open_error#)
' (what_is_already_open_error#) is what_is_already_open_error#
: it_is_already_closed  ( a -- )
  \ Informa de que un ente ya está cerrado.
  s" Ya está cerrad" r@ noun_ending+ period+ narrate
  ;
: what_is_already_closed
  \ Informa de que el ente WHAT ya está cerrado.
  what @ it_is_already_closed
  ;
' what_is_already_closed constant (what_is_already_closed_error#)
' (what_is_already_closed_error#) is what_is_already_closed_error#

\ }}} ##########################################################
section( Listas)  \ {{{

variable #listed  \ Contador de elementos listados, usado en varias acciones
variable #elements  \ Total de los elementos de una lista

: list_separator$  ( u1 u2 -- a u )
  \ Devuelve el separador adecuado a un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  \ a u = Cadena devuelta, que podrá ser « y » o «, » o «» (vacía)
  ?dup if
    1+ = if  s"  y "  else  s" , "  then
  else  0 
  then
  ;
: (list_separator)  ( u1 u2 -- )
  \ Añade a la cadena dinámica 'print_str' el separador adecuado («y» o «,») para un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  1+ = if  s" y" »&  else  s" ," »+  then
  ;
: list_separator  ( u1 u2 -- )
  \ Añade a la cadena dinámica 'print_str' el separador adecuado (o ninguno) para un elemento de una lista.
  \ u1 = Elementos que tiene la lista
  \ u2 = Elementos listados hasta el momento
  ?dup if  (list_separator)  else  drop  then
  ;
: can_be_listed?  ( a -- ff )
  \ ¿El ente puede ser incluido en las listas?
  \ Inacabado!!!
  dup protagonist% <>  \ ¿No es el protagonista?
  over is_decoration? 0=  and  \ ¿Y no es decorativo?
  over is_listed? and  \ ¿Y puede ser listado?
  swap is_global? 0=  and  \ ¿Y no es global?
  ;
: /list++  ( u1 a1 a2 -- u1 | u2 )
  \ Actualiza un contador si un ente es la localización de otro y puede ser listado.
  \ u1 = Contador
  \ a1 = Ente que actúa como localización
  \ a2 = Ente cuya localización hay que comprobar
  \ u2 = Contador incrementado
  dup can_be_listed?
  if  location = abs +  else  2drop  then
  ;
: /list  ( a -- u )
  \ Cuenta el número de entes cuya localización es el ente indicado y pueden ser listados.
  \ a = Ente que actúa como localización
  \ u = Número de entes localizados en el ente y que pueden ser listados
  0  \ Contador
  #entities 0 do
    over i #>entity /list++
  loop  nip
  ;
: (worn)$  ( a -- a1 u1 )
  \ Devuelve «(puesto/a/s)», según el género y número del ente indicado.
  s" (puest" rot noun_ending s" )" s+ s+
  ;
: (worn)&  ( a1 u1 a2 -- a1 u1 | a3 u3 )
  \ Añade a una cadena, si es necesario, el indicador de que el ente indicado es una prenda puesta.
  \ a1 u1 = Cadena con el nombre del ente
  \ a2 = Ente
  \ a3 u3 = Nombre del ente con, si es necesario, el indicador de que se trata de una prenda puesta
  dup  is_worn? if  (worn)$ s&  else  drop  then
  ;
: (content_list)  ( a -- )
  \ Añade a la lista en la cadena dinámica 'print_str' el separador y el nombre de un ente.
  #elements @ #listed @  list_separator
  dup full_name rot (worn)& »&  #listed ++
  ;
: about_to_list  ( a -- u )
  \ Prepara el inicio de una lista.
  \ a = Ente que es la localización de los entes a incluir en la lista
  \ u = Número de entes que serán listados
  #listed off  /list dup #elements !
  ;
: content_list  ( a -- a1 u1 )
  \ Devuelve una lista de entes localización es el ente indicado.
  \ a = Ente que actúa como localización
  \ a1 u1 = Lista de objetos localizados en dicho ente
  «»-clear
  dup about_to_list if
    #entities 1 do
      dup i #>entity dup can_be_listed? if
        is_there? if  i #>entity (content_list)  then
      else  2drop
      then
    loop  s" ." »+
  then  drop  «»@
  ;
: .present
  \ Lista los entes presentes.
  my_location content_list dup
  if  s" Ves" s" Puedes ver" 2 schoose 2swap s& narrate
  else  2drop
  then
  ;

\ }}} ##########################################################
section( Herramientas para las tramas asociadas a lugares)  \ {{{

: [:location_plot]  ( a -- )
  \ Inicia la definición de trama
  \ de un ente escenario, tanto de entrada como de salida.
  \ Esta palabra se ejecutará al comienzo de la palabra de trama de escenario.
  \ El identificador del ente está en la pila porque se compiló con LITERAL cuando se creó la palabra de trama. 
  to self%  \ Actualizar el puntero al ente, usado para aligerar la sintaxis
  ;
: (:location_enter_plot)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de la trama de entrada a un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define su trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~location_enter_plot_xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que '[:description]' lo guarde en 'self%' en tiempo de ejecución
  ;
: :location_enter_plot  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ la trama de entrada a un ente escenario
  :noname 5 roll 5 roll 
  (:location_enter_plot)  \ Hacer las operaciones preliminares
  postpone [:location_plot]  \ Compilar la palabra [:LOCATION_PLOT] en la palabra creada, para que se ejecute cuando sea llamada
  ;
: (:location_exit_plot)  ( a xt -- )
  \ Operaciones preliminares para la definición
  \ de la trama de salida de un ente escenario.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define su trama.
  \ a = Ente escenario para cuya trama se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada
  over ~location_exit_plot_xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
  postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que '[:description]' lo guarde en 'self%' en tiempo de ejecución
  ;
: :location_exit_plot  ( a -- xt a )
  \ Crea una palabra sin nombre que manejará
  \ la trama de salida de un ente escenario
  :noname 5 roll 5 roll
  (:location_exit_plot)  \ Hacer las operaciones preliminares
  postpone [:location_plot]  \ Compilar la palabra [:LOCATION_PLOT] en la palabra creada, para que se ejecute cuando sea llamada
  ;
true [if]
  : ;location_enter_plot  ( colon-sys -- )  postpone ;  ;  immediate
  : ;location_exit_plot  ( colon-sys -- )  postpone ;  ;  immediate
[else]  \ Así es más simple pero no funciona en Gforth!!!:
  ' ; alias ;location_enter_plot  immediate
  ' ; alias ;location_exit_plot  immediate
[then]
: ?execute  ( xt | 0 -- )
  \ Ejecuta un vector de ejecución, si no es cero.
  ?dup if  execute  then
  ;
: every_location_enter_plot
  \ Trama de entrada común a todos los entes escenario.
  \ No se usa!!!
  ;
: location_enter_plot  ( a -- )
  \ Ejecuta la trama de entrada a un ente escenario.
  every_location_enter_plot
  location_enter_plot_xt ?execute 
  ;
: every_location_exit_plot
  \ Trama de salida común a todos los entes escenario.
  \ No se usa!!!
  ;
: location_exit_plot  ( a -- )
  \ Ejecuta la trama de salida de un ente escenario.
  every_location_exit_plot
  location_enter_plot_xt ?execute 
  ;
: leave_location
  \ Tareas previas a abandonar el escenario actual.
  my_location ?dup if
    dup visits++
    dup location_exit_plot 
    protagonist% was_there
  then
  ;
: enter_location  ( a -- )
  \ Entra en un escenario.
  [debug] [if]  s" En ENTER" debug  [then]  \ Depuración!!!
  leave_location
  \ Pendiente!!! Llamar desde aquí a una trama previa de entrada en escenario, antes de su asignación,
  \ lo que permitiría gestionar aquí el cambio de escenario desde la cueva oscura.
  dup my_location!
  \ Pendiente!!! Llamar desde aquí a una trama previa de entrada en escenario, antes de su descripción.
  dup describe
  dup location_enter_plot 
  familiar++  .present
  \ Pendiente!!! Llamar desde aquí, tras listar los objetos presentes, a una trama final de escenario.
  ;

\ }}} ##########################################################
section( Recursos de las tramas asociadas a lugares)  \ {{{

\ ------------------------------------------------
\ Regreso a casa

: pass_still_open?  ( -- ff )
  \ ¿El paso del desfiladero está abierto por el Norte?
  location_08% has_north_exit?
  ;
: still_in_the_village?  ( -- ff )
  \ ¿Los soldados no se han movido aún de la aldea sajona?
  my_location location_01% =
  location_02% is_not_visited? and
  ;
: back_to_the_village?  ( -- ff )
  \ ¿Los soldados han regresado a la aldea sajona?
  \ No se usa!!!
  my_location location_01% =
  location_02% is_visited? and
  ;
: soldiers_follow_you
  \ De vuelta a casa.
  ^all_your$ soldiers$ s&
  s{ s" siguen tus pasos." s" te siguen." }s&
  narrate
  ;
: going_home
  \ De vuelta a casa, si procede.
  pass_still_open?
  still_in_the_village? 0= and
  if  soldiers_follow_you  then
  ;
: celebrating
  \ Celebrando la victoria.
  \ Inacabado!!!
  ^all_your$ soldiers$ s&
  s{ s" lo están celebrando." s" lo celebran." }s&
  narrate
  ;

[ambush_in_global_plot?] 0= [if]  \ Experimental!!!
 
\ ------------------------------------------------
\ Persecución

: pursued
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
  }s s" ..." s+  narrate
  ;
: pursue_location?  ( -- ff )
  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?
  my_location location_12% <
  ;

\ ------------------------------------------------
\ Batalla

: all_your_men  ( -- a u ff )
  \ Devuelve una variante de «Todos tus hombres», y un indicador de número.
  \ a u = Cadena
  \ ff = ¿El texto está en plural?
  2 random dup
  if  s{ s" Todos" s" Todos y cada uno de" }s
  else  s" Hasta el último de"
  then  your_soldiers$ s&  rot
  ;
: ?plural_verb  ( a1 u1 ff -- a1 u1 | a2 u2 )
  \ Pone un verbo en plural si es preciso.
  if  s" n" s+  then
  ;
: fight/s$  ( ff -- a u )
  \ Devuelve una variante de «lucha/n».
  \ ff = ¿El resultado debe estar en plural?
  \ a u = Resultado
  s{ s" lucha" s" combate" s" pelea" s" se bate" }s
  rot ?plural_verb
  ;
: resist/s$  ( ff -- a u )
  \ Devuelve una variante de «resiste/n».
  \ ff = ¿El resultado debe estar en plural?
  \ a u = Resultado
  s{ s" resiste" s" aguanta" s" contiene" }s
  rot ?plural_verb
  ;
: heroe$  ( -- a u )
  \ Devuelve una variante de «héroe».
  s{ s" héroe" s" valiente" s" jabato" }s
  ;
: heroes$  ( -- a u )
  \ Devuelve una variante de «héroes».
  heroe$ s" s" s+
  ;
: like_a_heroe$ ( -- a u )
  \ Devuelve una variante de «como un héroe».
  s" como un" s" auténtico" s?& heroe$ s&
  ;
: like_heroes$ ( -- a u )
  \ Devuelve una variante de «como héroes».
  s" como" s" auténticos" s?& heroes$ s&
  ;
: (bravery)$  ( -- a u )
  \ Devuelve una variante de «con denuedo».
  s{ s" con denuedo" s" con bravura" s" con coraje"
  s" heroicamente" s" esforzadamente" s" valientemente" }s
  ;
: bravery$  ( ff -- a u )
  \ Devuelve una variante de «con denuedo», en singular o plural.
  \ ff = ¿El resultado debe estar en plural?
  \ a u = Resultado
  (bravery)$  rot
  if  like_heroes$  else  like_a_heroe$  then
  2 schoose 
  ;
: step_by_step$  ( -- a u )
  \ Devuelve una variante de «poco a poco».
  s{ s" por momentos" s" palmo a palmo" s" poco a poco" }s
  ;
: field$  ( -- a u )
  \ Devuelve «terreno» o «posiciones».
  s{ s" terreno" s" posiciones" }s
  ;
: last(fp)$  ( -- a u )
  \ Devuelve una variante de «últimas».
  s{ s" últimas" s" postreras" }s
  ;
: last$  ( -- a u )
  \ Devuelve una variante de «último».
\ Nota!!! Confirmar «postrer»
  s{ s" último" s" postrer" }s
  ;
: last_energy(fp)$  ( -- a u )
  \ Devuelve una variante de «últimas energías».
  last(fp)$ s{ s" energías" s" fuerzas" }s&
  ;
: battle_phase_00$  ( -- a u )
  \ Devuelve la descripción del combate (fase 00)
  s" A pesar de" s{
  s" haber sido" s{ s" atacados por sorpresa" s" sorprendidos" }s&
  s" la sorpresa" s" inicial" s?&
  s" lo" s{ s" inesperado" s" sorpresivo" s" sorprendente" s" imprevisto" }s&
  s" del ataque" s& }s& comma+ your_soldiers$ s&
  s{ s" responden" s" reaccionan" }s&
  s{ s" con prontitud" s" sin perder un instante"
  s" rápidamente" s" como si fueran uno solo"
  }s& s" y" s&{
  s" adoptan una formación defensiva"
  s" organizan la defensa"
  s" se" s{ s" preparan" s" aprestan" }s& s" para" s&
  s{ s" defenderse" s" la defensa" }s&
  }s& period+ 
  ;
: battle_phase_00
  \ Combate (fase 00).
  battle_phase_00$ narrate
  ;
: battle_phase_01$  ( -- a u )
  \ Devuelve la descripción del combate (fase 01)
  all_your_men  dup resist/s$  rot bravery$  s& s&
  s{  s{ s" el ataque" s" el empuje" s" la acometida" }s
      s" inicial" s&
      s" el primer" s{ s" ataque" s" empuje" }s&
      s" la primera acometida"
  }s& of_the_enemy|enemies$ s& period+ 
  ;
: battle_phase_01
  \ Combate (fase 01).
  battle_phase_01$ narrate
  ;
: battle_phase_02$  ( -- a u )
  \ Devuelve la descripción del combate (fase 02)
  all_your_men  dup fight/s$  rot bravery$  s& s&
  s" contra" s&  the_enemy|enemies$ s&  period+
  ;
: battle_phase_02
  \ Combate (fase 02).
  battle_phase_02$ narrate
  ;
: battle_phase_03$  ( -- a u )
  \ Devuelve la descripción del combate (fase 03)
  \ Inacabado!!!
  ^your_soldiers$
  s" empiezan a acusar" s&
  s{ "" s" visiblemente" s" notoriamente" }s&
  s" el" s&{ s" titánico" s" enorme" }s?&
  s" esfuerzo." s&
  ;
: battle_phase_03
  \ Combate (fase 03).
  battle_phase_03$ narrate
  ;
: battle_phase_04$  ( -- a u )
  \ Devuelve la descripción del combate (fase 04)
  ^the_enemy|enemies
  s" parece que empieza* a" rot *>verb_ending s&
  s{ s" dominar" s" controlar" }s&
  s{ s" el campo" s" el combate" s" la situación" s" el terreno" }s&
  period+ 
  ;
: battle_phase_04
  \ Combate (fase 04).
  battle_phase_04$ narrate
  ;
: battle_phase_05$  ( -- a u )
  \ Devuelve la descripción del combate (fase 05)
  \ Inacabado!!!?
  ^the_enemy|enemies s{
  s" está* haciendo retroceder a" your_soldiers$ s&
  s" está* obligando a" your_soldiers$ s& s" a retroceder" s&
  }s rot *>verb_ending s&
  step_by_step$ s& period+
  ;
: battle_phase_05
  \ Combate (fase 05).
  battle_phase_05$ narrate
  ;
: battle_phase_06$  ( -- a u )
  \ Devuelve la descripción del combate (fase 06)
  \ Inacabado!!!
  ^the_enemy|enemies s{
  s" va* ganando" field$ s&
  s" va* adueñándose del terreno"
  s" va* conquistando" field$ s&
  s" se va* abriendo paso"
  }s rot *>verb_ending s&
  step_by_step$ s& period+
  ;
: battle_phase_06
  \ Combate (fase 06).
  battle_phase_06$ narrate
  ;
: battle_phase_07$  ( -- a u )
  \ Devuelve la descripción del combate (fase 07)
  ^your_soldiers$
  s{ s" caen" s" van cayendo," }s&
  s" uno tras otro," s?&
  s{ s" vendiendo cara su vida" s" defendiéndose" }s&
  like_heroes$ s& period+
  ;
: battle_phase_07
  \ Combate (fase 07).
  battle_phase_07$ narrate
  ;
: battle_phase_08$  ( -- a u )
  \ Devuelve la descripción del combate (fase 08)
  ^the_enemy|enemies
  s{ s" aplasta* a" s" acaba* con" }s
  rot *>verb_ending s&
  s" los últimos de" s" entre" s?& s&
  your_soldiers$ s& s" que," s&
  s{ s" heridos" s{ s" extenuados" s" exhaustos" s" agotados" }s both?
  s{ s" apurando" s" con" }s s" sus" s& last_energy(fp)$ s&
  s" con su" last$ s& s" aliento" s&
  s" haciendo un" last$ s& s" esfuerzo" s&
  }s& comma+ still$ s&
  s{ s" combaten" s" resisten"
  s{ s" se mantienen" s" aguantan" s" pueden mantenerse" }s
  s" en pie" s&
  s{ s" ofrecen" s" pueden ofrecer" }s s" alguna" s?&
  s" resistencia" s&
  }s& period+
  ;
: battle_phase_08
  \ Combate (fase 08).
  battle_phase_08$ narrate
  ;
create 'battle_phases  \ Tabla para las fases del combate
here \ Dirección libre actual, para calcular después el número de fases
  \ Compilar la dirección de ejecución de cada fase: 
  ' battle_phase_00 ,
  ' battle_phase_01 ,
  ' battle_phase_02 ,
  ' battle_phase_03 ,
  ' battle_phase_04 ,
  ' battle_phase_05 ,
  ' battle_phase_06 ,
  ' battle_phase_07 ,
  ' battle_phase_08 ,
here swap - cell / constant battle_phases  \ Fases de la batalla
: (battle_phase)  ( u -- )
  \ Ejecuta una fase del combate.
  \ u = Fase del combate (la primera es la cero)
  cells 'battle_phases + perform  
  ;
: battle_phase
  \ Ejecuta la fase en curso del combate.
  battle# @ 1- (battle_phase)
  ;
: battle_location?  ( -- ff )
  \ ¿En el escenario de la batalla?
  my_location location_10% <  \ ¿Está el protagonista en un escenario menor que el 10?
  pass_still_open? 0=  and  \ ¿Y el paso del desfiladero está cerrado?
  ;
: battle_phase++
  \ Incrementar la fase de la batalla (salvo una de cada diez veces, al azar).
  10 random if  battle# ++  then
  ;
: battle
  \ Batalla y persecución.
  battle_location? if  battle_phase  then
  pursue_location? if  pursued  then
  battle_phase++
  ;
: battle?  ( -- ff )
  \ ¿Ha empezado la batalla?
  battle# @ 0>
  ;
: the_battle_ends
  \ Termina la batalla.
  battle# off
  ;
: the_battle_begins
  \ Comienza la batalla.
  1 battle# !
  ;

\ ------------------------------------------------
\ Emboscada de los sajones

: the_pass_is_closed
  \ Cerrar el paso, la salida norte.
  no_exit location_08% ~north_exit !
  ;
: the_ambush_begins
  \ Comienza la emboscada.
  s" Una partida sajona aparece por el Este."
  s" Para cuando" s&
  s{ s" te vuelves" s" intentas volver" }s&
  toward_the(m)$ s& s" Norte," s&
  s" ya no" s& s{ s" te" s? s" queda" s& s" tienes" }s&
  s{ s" duda:" s" duda alguna:" s" ninguna duda:" }s&
  s{  s" es" s" se trata de"
      s{ s" te" s" os" }s s" han tendido" s&
  }s& s" una" s&
  s{ s" emboscada" s" celada" s" encerrona" s" trampa" }s&
  period+  narrate narration_break
  ;

: they_win_0$  ( -- a u )
  \ Devuelve la primera versión de la parte final de las palabras de los oficiales.
  s{ s" su" s{ s" victoria" s" triunfo" }s&
  s" nuestra" s{ s" derrota" s" humillación" }s&
  }s s" será" s&{ s" doble" s" mayor" }s&
  ;
: they_win_1$  ( -- a u )
  \ Devuelve la segunda versión de la parte final de las palabras de los oficiales.
  s{ s" ganan" s" nos ganan" s" vencen"
  s" perdemos" s" nos vencen" s" nos derrotan" }s
  s{ s" doblemente" s" por partida doble" }s&
  ;
: they_win$  ( -- a u )
  \ Devuelve la parte final de las palabras de los oficiales.
  they_win_0$ they_win_1$ 2 schoose period+
  ;
: taking_prisioner$  ( -- a u )
  \ Devuelve una parte de las palabras de los oficiales.
  s" si" s{ s" capturan" s" hacen prisionero" s" toman prisionero" }s&
  ;
: officers_speach
  \ Palabras de los oficiales.
  sire,$ s?  dup taking_prisioner$
  rot 0= if  ^uppercase  then  s&
  s" a un general britano" s& they_win$ s&  speak
  ;
: officers_talk_to_you
  \ Los oficiales hablan con el protagonista.
  s" Tus oficiales te"
  s{ s" conminan a huir"
  s" conminan a ponerte a salvo"
  s" piden que te pongas a salvo"
  s" piden que huyas" }s& colon+ narrate
  officers_speach
  s{ s" Sabes" s" Comprendes" }s s" que" s&
  s{ s" es cierto" s" llevan razón"
  s" están en lo cierto" }s&
  s" , y te duele." s+  narrate
  ;
: the_enemy_is_stronger
  \ El enemigo es superior.
  s" En el" narrow(m)$ s& s" paso es posible" s&
  s{ s" resistir," s" defenderse," }s& but$ s&
  s{ s" por desgracia" s" desgraciadamente" }s&
  s{
  s" los sajones son muy superiores en número"
  s" los sajones son mucho más numerosos"
  s" sus soldados son más numerosos que los tuyos"
  s" sus tropas son más numerosas que las tuyas"
  s" sus hombres son más numerosos que los tuyos"
  s" ellos son muy superiores en número"
  s" ellos son mucho más numerosos"
  }s& period+  narrate scene_break
  ;
: ambush
  \ Emboscada.
  the_pass_is_closed
  the_ambush_begins
  the_battle_begins
  the_enemy_is_stronger
  officers_talk_to_you
  ;

[then]

\ ------------------------------------------------
\ Albergue de los refugiados

: the_old_man_is_angry?  ( -- ff )
  \ ¿El anciano se enfada porque llevas algo prohibido?
  stone% is_accessible?
  sword% is_accessible?  or
  ;
: he_looks_at_you_with_anger$  ( -- a u )
  \ Texto de que el líder de los refugiados te mira.
  s" parece sorprendido y" s?
  s{
  s" te mira" s{ s" con dureza" s" con preocupación" }s&
  s" te dirige una dura mirada"
  s" dirige su mirada hacia ti"
  }s&
  ;
: he_looks_at_you_with_calm$  ( -- a u )
  \ Texto de que el líder de los refugiados te mira.
  s" advierte tu presencia y" s?
  s{ s" por un momento" s" durante unos instantes" }s?&
  s" te" s&{ s" observa" s" contempla" }s&
  s{ s" con serenidad" s" con expresión serena" s" en calma" s" sereno" }s&
  ;
: the_leader_looks_at_you$  ( -- a u )
  \ Texto de que el líder de los refugiados te mira.
  leader% ^full_name  the_old_man_is_angry?
  if  he_looks_at_you_with_anger$ 
  else  he_looks_at_you_with_calm$
  then  s& period+
  ;
: the_refugees_surround_you$  ( -- a u )
  \ Descripción de la actitud de los refugiados.
  ^the_refugees$
  location_28% has_east_exit?
  if  they_let_you_pass$ 
  else  they_don't_let_you_pass$
  then  period+ s&
  ;

\ }}} ##########################################################
section( Tramas asociadas a lugares)  \ {{{

location_01% :location_enter_plot
  soldiers% is_here
  still_in_the_village?
  if  celebrating  else  going_home  then
  ;location_enter_plot
location_02% :location_enter_plot
  \ Decidir hacia dónde conduce la dirección hacia abajo
  [false] [if]  \ Primera versión
    \ Decidir al azar:
    self% location_01% location_03% 2 choose d-->
  [else]  \ Segunda versión mejorada
    \ Decidir según el escenario de procedencia:
    self%
    protagonist% previous_location location_01% =  \ ¿Venimos de la aldea?
    if  location_03%  else  location_01%  then  d-->
  [then]
  soldiers% is_here going_home
  ;location_enter_plot
location_03% :location_enter_plot
  soldiers% is_here going_home
  ;location_enter_plot
location_04% :location_enter_plot
  soldiers% is_here going_home
  ;location_enter_plot
location_05% :location_enter_plot
  soldiers% is_here going_home
  ;location_enter_plot
location_06% :location_enter_plot
  soldiers% is_here going_home
  ;location_enter_plot
location_07% :location_enter_plot
  soldiers% is_here going_home
  ;location_enter_plot
location_08% :location_enter_plot
  soldiers% is_here
  going_home
  [ [ambush_in_global_plot?] 0= ] [if]
    pass_still_open? if  ambush  then
  [then]
  ;location_enter_plot
location_09% :location_enter_plot
  soldiers% is_here
  going_home
  ;location_enter_plot
location_10% :location_enter_plot
  s" entrada a la cueva" cave_entrance% fname!
  cave_entrance% familiar++
  self% visits
  if  ^again$  else  ^finally$ s" ya" s?&  then
  \ Inacabado!!! Ampliar con otros textos alternativos
  s{  s" crees estar"
      s" piensas que te encuentras"
      s" te sientes"
      s" crees sentirte"
      s" tienes la sensación de estar"
  }s&
  s{ s" a salvo" s" seguro" }s&
  period+ narrate
  ;location_enter_plot
location_11% :location_enter_plot
  lake% is_here
  ;location_enter_plot
location_16% :location_enter_plot
  s" En la distancia, por entre los resquicios de las rocas,"
  s" y allende el canal de agua, los sajones" s&
  s{ s" intentan" s" se esfuerzan en" s" tratan de" s" se afanan en" }s&
  s{ s" hallar" s" buscar" s" localizar" }s&
  s" la salida que encontraste por casualidad." s&
  narrate
  ;location_enter_plot
[dark_cave_in_global_plot?] 0= [if]
: dark_cave
  \ En la cueva y sin luz.
  new_page
  s" Ante la reinante"
  s{ s" e intimidante" s" e impenetrable" s" y sobrecogedora" }s&
  s" oscuridad," s&
  s{ s" vuelves atrás" s" retrocedes" }s&
  s{ "" s" unos pasos" s" sobre tus pasos" }s&
  s" hasta donde puedes ver." s&
  narrate  scene_break 
  ;
location_20% :location_enter_plot
  \ Pendiente!!!
  \ Llamar de nuevo desde aquí a 'enter_location' podría
  \ tener consecuencias imprevistas, pues la llamada original
  \ sigue teniendo su dirección en la pila de retorno:
  no_torch? if  dark_cave location_17% my_location!  then
  ;location_enter_plot
[then]
location_28% :location_enter_plot
  self% no_exit e-->  \ Cerrar la salida hacia el Este
  recent_talks_to_the_leader off
  refugees% is_here
  the_refugees_surround_you$ narrate
  the_leader_looks_at_you$ narrate
  ;location_enter_plot
location_29% :location_enter_plot
  refugees% is_here  \ Para que sean visibles en la distancia
  ;location_enter_plot
location_31% :location_enter_plot
  self% has_north_exit? if
    s" Las rocas yacen desmoronadas a lo largo del"
    pass_way$ s& period+
  else
    s" Las rocas" (they)_block$ s& s" el paso." s&
  then  narrate
  ;location_enter_plot
location_38% :location_enter_plot
  lake% is_here
  ;location_enter_plot
location_43% :location_enter_plot
  snake% is_here? if
    a_snake_blocks_the_way$ period+
    narrate
  then
  ;location_enter_plot
location_44% :location_enter_plot
  lake% is_here
  ;location_enter_plot
location_47% :location_enter_plot
  door% is_here
  ;location_enter_plot
location_48% :location_enter_plot
  door% is_here
  ;location_enter_plot

\ }}} ##########################################################
section( Trama global)  \ {{{

\ ------------------------------------------------
\ Varios

: (lock_found)
  \ Encontrar el candado (al mirar la puerta o al intentar abrirla).
  door% location lock% is_there
  lock% familiar++
  ;  ' (lock_found) is lock_found

[ambush_in_global_plot?] [if]
 
\ ------------------------------------------------
\ Persecución

: pursued
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
  }s s" ..." s+  narrate
  ;
: pursue_location?  ( -- ff )
  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?
  my_location location_12% <
  ;

\ ------------------------------------------------
\ Batalla

: all_your_men  ( -- a u ff )
  \ Devuelve una variante de «Todos tus hombres», y un indicador de número.
  \ a u = Cadena
  \ ff = ¿El texto está en plural?
  2 random dup
  if  s{ s" Todos" s" Todos y cada uno de" }s
  else  s" Hasta el último de"
  then  your_soldiers$ s&  rot
  ;
: ?plural_verb  ( a1 u1 ff -- a1 u1 | a2 u2 )
  \ Pone un verbo en plural si es preciso.
  if  s" n" s+  then
  ;
: fight/s$  ( ff -- a u )
  \ Devuelve una variante de «lucha/n».
  \ ff = ¿El resultado debe estar en plural?
  \ a u = Resultado
  s{ s" lucha" s" combate" s" pelea" s" se bate" }s
  rot ?plural_verb
  ;
: resist/s$  ( ff -- a u )
  \ Devuelve una variante de «resiste/n».
  \ ff = ¿El resultado debe estar en plural?
  \ a u = Resultado
  s{ s" resiste" s" aguanta" s" contiene" }s
  rot ?plural_verb
  ;
: heroe$  ( -- a u )
  \ Devuelve una variante de «héroe».
  s{ s" héroe" s" valiente" s" jabato" }s
  ;
: heroes$  ( -- a u )
  \ Devuelve una variante de «héroes».
  heroe$ s" s" s+
  ;
: like_a_heroe$ ( -- a u )
  \ Devuelve una variante de «como un héroe».
  s" como un" s" auténtico" s?& heroe$ s&
  ;
: like_heroes$ ( -- a u )
  \ Devuelve una variante de «como héroes».
  s" como" s" auténticos" s?& heroes$ s&
  ;
: (bravery)$  ( -- a u )
  \ Devuelve una variante de «con denuedo».
  s{ s" con denuedo" s" con bravura" s" con coraje"
  s" heroicamente" s" esforzadamente" s" valientemente" }s
  ;
: bravery$  ( ff -- a u )
  \ Devuelve una variante de «con denuedo», en singular o plural.
  \ ff = ¿El resultado debe estar en plural?
  \ a u = Resultado
  (bravery)$  rot
  if  like_heroes$  else  like_a_heroe$  then
  2 schoose 
  ;
: step_by_step$  ( -- a u )
  \ Devuelve una variante de «poco a poco».
  s{ s" por momentos" s" palmo a palmo" s" poco a poco" }s
  ;
: field$  ( -- a u )
  \ Devuelve «terreno» o «posiciones».
  s{ s" terreno" s" posiciones" }s
  ;
: last(fp)$  ( -- a u )
  \ Devuelve una variante de «últimas».
  s{ s" últimas" s" postreras" }s
  ;
: last$  ( -- a u )
  \ Devuelve una variante de «último».
\ Nota!!! Confirmar «postrer»
  s{ s" último" s" postrer" }s
  ;
: last_energy(fp)$  ( -- a u )
  \ Devuelve una variante de «últimas energías».
  last(fp)$ s{ s" energías" s" fuerzas" }s&
  ;
: battle_phase_00$  ( -- a u )
  \ Devuelve la descripción del combate (fase 00)
  s" A pesar de" s{
  s" haber sido" s{ s" atacados por sorpresa" s" sorprendidos" }s&
  s" la sorpresa" s" inicial" s?&
  s" lo" s{ s" inesperado" s" sorpresivo" s" sorprendente" s" imprevisto" }s&
  s" del ataque" s& }s& comma+ your_soldiers$ s&
  s{ s" responden" s" reaccionan" }s&
  s{ s" con prontitud" s" sin perder un instante"
  s" rápidamente" s" como si fueran uno solo"
  }s& s" y" s&{
  s" adoptan una formación defensiva"
  s" organizan la defensa"
  s" se" s{ s" preparan" s" aprestan" }s& s" para" s&
  s{ s" defenderse" s" la defensa" }s&
  }s& period+ 
  ;
: battle_phase_00
  \ Combate (fase 00).
  battle_phase_00$ narrate
  ;
: battle_phase_01$  ( -- a u )
  \ Devuelve la descripción del combate (fase 01)
  all_your_men  dup resist/s$  rot bravery$  s& s&
  s{  s{ s" el ataque" s" el empuje" s" la acometida" }s
      s" inicial" s&
      s" el primer" s{ s" ataque" s" empuje" }s&
      s" la primera acometida"
  }s& of_the_enemy|enemies$ s& period+ 
  ;
: battle_phase_01
  \ Combate (fase 01).
  battle_phase_01$ narrate
  ;
: battle_phase_02$  ( -- a u )
  \ Devuelve la descripción del combate (fase 02)
  all_your_men  dup fight/s$  rot bravery$  s& s&
  s" contra" s&  the_enemy|enemies$ s&  period+
  ;
: battle_phase_02
  \ Combate (fase 02).
  battle_phase_02$ narrate
  ;
: battle_phase_03$  ( -- a u )
  \ Devuelve la descripción del combate (fase 03)
  \ Inacabado!!!
  ^your_soldiers$
  s" empiezan a acusar" s&
  s{ "" s" visiblemente" s" notoriamente" }s&
  s" el" s&{ s" titánico" s" enorme" }s?&
  s" esfuerzo." s&
  ;
: battle_phase_03
  \ Combate (fase 03).
  battle_phase_03$ narrate
  ;
: battle_phase_04$  ( -- a u )
  \ Devuelve la descripción del combate (fase 04)
  ^the_enemy|enemies
  s" parece que empieza* a" rot *>verb_ending s&
  s{ s" dominar" s" controlar" }s&
  s{ s" el campo" s" el combate" s" la situación" s" el terreno" }s&
  period+ 
  ;
: battle_phase_04
  \ Combate (fase 04).
  battle_phase_04$ narrate
  ;
: battle_phase_05$  ( -- a u )
  \ Devuelve la descripción del combate (fase 05)
  \ Inacabado!!!?
  ^the_enemy|enemies s{
  s" está* haciendo retroceder a" your_soldiers$ s&
  s" está* obligando a" your_soldiers$ s& s" a retroceder" s&
  }s rot *>verb_ending s&
  step_by_step$ s& period+
  ;
: battle_phase_05
  \ Combate (fase 05).
  battle_phase_05$ narrate
  ;
: battle_phase_06$  ( -- a u )
  \ Devuelve la descripción del combate (fase 06)
  \ Inacabado!!!
  ^the_enemy|enemies s{
  s" va* ganando" field$ s&
  s" va* adueñándose del terreno"
  s" va* conquistando" field$ s&
  s" se va* abriendo paso"
  }s rot *>verb_ending s&
  step_by_step$ s& period+
  ;
: battle_phase_06
  \ Combate (fase 06).
  battle_phase_06$ narrate
  ;
: battle_phase_07$  ( -- a u )
  \ Devuelve la descripción del combate (fase 07)
  ^your_soldiers$
  s{ s" caen" s" van cayendo," }s&
  s" uno tras otro," s?&
  s{ s" vendiendo cara su vida" s" defendiéndose" }s&
  like_heroes$ s& period+
  ;
: battle_phase_07
  \ Combate (fase 07).
  battle_phase_07$ narrate
  ;
: battle_phase_08$  ( -- a u )
  \ Devuelve la descripción del combate (fase 08)
  ^the_enemy|enemies
  s{ s" aplasta* a" s" acaba* con" }s
  rot *>verb_ending s&
  s" los últimos de" s" entre" s?& s&
  your_soldiers$ s& s" que," s&
  s{ s" heridos" s{ s" extenuados" s" exhaustos" s" agotados" }s both?
  s{ s" apurando" s" con" }s s" sus" s& last_energy(fp)$ s&
  s" con su" last$ s& s" aliento" s&
  s" haciendo un" last$ s& s" esfuerzo" s&
  }s& comma+ still$ s&
  s{ s" combaten" s" resisten"
  s{ s" se mantienen" s" aguantan" s" pueden mantenerse" }s
  s" en pie" s&
  s{ s" ofrecen" s" pueden ofrecer" }s s" alguna" s?&
  s" resistencia" s&
  }s& period+
  ;
: battle_phase_08
  \ Combate (fase 08).
  battle_phase_08$ narrate
  ;
create 'battle_phases  \ Tabla para las fases del combate
here \ Dirección libre actual, para calcular después el número de fases
  \ Compilar la dirección de ejecución de cada fase: 
  ' battle_phase_00 ,
  ' battle_phase_01 ,
  ' battle_phase_02 ,
  ' battle_phase_03 ,
  ' battle_phase_04 ,
  ' battle_phase_05 ,
  ' battle_phase_06 ,
  ' battle_phase_07 ,
  ' battle_phase_08 ,
here swap - cell / constant battle_phases  \ Fases de la batalla
: (battle_phase)  ( u -- )
  \ Ejecuta una fase del combate.
  \ u = Fase del combate (la primera es la cero)
  cells 'battle_phases + perform  
  ;
: battle_phase
  \ Ejecuta la fase en curso del combate.
  battle# @ 1- (battle_phase)
  ;
: battle_location?  ( -- ff )
  \ ¿En el escenario de la batalla?
  my_location location_10% <  \ ¿Está el protagonista en un escenario menor que el 10?
  pass_still_open? 0=  and  \ ¿Y el paso del desfiladero está cerrado?
  ;
: battle_phase++
  \ Incrementar la fase de la batalla (salvo una de cada diez veces, al azar).
  10 random if  battle# ++  then
  ;
: battle
  \ Batalla y persecución.
  battle_location? if  battle_phase  then
  pursue_location? if  pursued  then
  battle_phase++
  ;
: battle?  ( -- ff )
  \ ¿Ha empezado la batalla?
  battle# @ 0>
  ;
: the_battle_ends
  \ Termina la batalla.
  battle# off
  ;
: the_battle_begins
  \ Comienza la batalla.
  1 battle# !
  ;

\ ------------------------------------------------
\ Emboscada de los sajones

: ambush?  ( -- ff )
  \ ¿Ha caído el protagonista en la emboscada?
  my_location location_08% =  \ ¿Está en el escenario 8?
  pass_still_open?  and  \ ¿Y además el paso está abierto?
  ;
: the_pass_is_closed
  \ Cerrar el paso, la salida norte.
  no_exit location_08% ~north_exit !
  ;
: the_ambush_begins
  \ Comienza la emboscada.
  s" Una partida sajona aparece por el Este."
  s" Para cuando" s&
  s{ s" te vuelves" s" intentas volver" }s&
  toward_the(m)$ s& s" Norte," s&
  s" ya no" s& s{ s" te" s? s" queda" s& s" tienes" }s&
  s{ s" duda:" s" duda alguna:" s" ninguna duda:" }s&
  s{
    s" es" s" se trata de"
    s{ s" te" s" os" }s s" han tendido" s&
  }s& s" una" s&
  s{ s" emboscada" s" celada" s" encerrona" s" trampa" }s&
  period+  narrate narration_break
  ;

: they_win_0$  ( -- a u )
  \ Devuelve la primera versión de la parte final de las palabras de los oficiales.
  s{ s" su" s{ s" victoria" s" triunfo" }s&
  s" nuestra" s{ s" derrota" s" humillación" }s&
  }s s" será" s&{ s" doble" s" mayor" }s&
  ;
: they_win_1$  ( -- a u )
  \ Devuelve la segunda versión de la parte final de las palabras de los oficiales.
  s{ s" ganan" s" nos ganan" s" vencen"
  s" perdemos" s" nos vencen" s" nos derrotan" }s
  s{ s" doblemente" s" por partida doble" }s&
  ;
: they_win$  ( -- a u )
  \ Devuelve la parte final de las palabras de los oficiales.
  they_win_0$ they_win_1$ 2 schoose period+
  ;
: taking_prisioner$  ( -- a u )
  \ Devuelve una parte de las palabras de los oficiales.
  s" si" s{ s" capturan" s" hacen prisionero" s" toman prisionero" }s&
  ;
: officers_speach
  \ Palabras de los oficiales.
  sire,$ s?  dup taking_prisioner$
  rot 0= if  ^uppercase  then  s&
  s" a un general britano" s& they_win$ s&  speak
  ;
: officers_talk_to_you
  \ Los oficiales hablan con el protagonista.
  s" Tus oficiales te"
  s{ s" conminan a huir."
  s" conminan a ponerte a salvo."
  s" piden que te pongas a salvo."
  s" piden que huyas." }s&  narrate
  officers_speach
  s{ s" Sabes" s" Comprendes" }s s" que" s&
  s{ s" es cierto" s" llevan razón"
  s" están en lo cierto" }s&
  s" , y te duele." s+  narrate
  ;
: the_enemy_is_stronger
  \ El enemigo es superior.
  s" En el" narrow(m)$ s& s" paso es posible" s&
  s{ s" resistir," s" defenderse," }s& but$ s&
  s{ s" por desgracia" s" desgraciadamente" }s&
  s{
  s" los sajones son muy superiores en número"
  s" los sajones son mucho más numerosos"
  s" sus soldados son más numerosos que los tuyos"
  s" sus tropas son más numerosas que las tuyas"
  s" sus hombres son más numerosos que los tuyos"
  s" ellos son muy superiores en número"
  s" ellos son mucho más numerosos"
  }s& period+  narrate scene_break
  ;
: ambush
  \ Emboscada.
  the_pass_is_closed
  the_ambush_begins
  the_battle_begins
  the_enemy_is_stronger
  officers_talk_to_you
  ;

[then]

\ ------------------------------------------------
\ Oscuridad en la cueva

[dark_cave_in_global_plot?] [if]

: dark_cave?  ( -- ff )
  \ ¿Entrar en la zona oscura de cueva y sin luz?
  no_torch?
  my_location location_20% =  and
  my_previous_location location_17% =  and
  ;
: dark_cave
  \ En la cueva y sin luz.
  new_page
  s" Ante la reinante"
  s{ s" e intimidante" s" e impenetrable" s" y sobrecogedora" }s&
  s" oscuridad," s&
  s{ s" vuelves atrás" s" retrocedes" }s&
  s{ "" s" unos pasos" s" sobre tus pasos" }s&
  s" hasta donde puedes ver." s&
  narrate  scene_break  location_17% enter_location
  ;
[then]

\ ------------------------------------------------
\ Ambrosio nos sigue

(

Pendiente!!!:

Confirmar la función de la llave aquí. En el código original
solo se distingue que sea manipulable o no, lo que es
diferente a que esté accesible.

)

: ambrosio_must_follow?
  \ ¿Ambrosio tiene que estar siguiéndonos?
  ambrosio% not_vanished?  key% is_accessible? and
  location_46% am_i_there?  ambrosio_follows? @ or  and
  ;
: ambrosio_must_follow
  \ Ambrosio tiene que estar siguiéndonos.
  my_location ambrosio% is_there
  s" Tu benefactor te sigue, esperanzado." narrate  
  ;

\ ------------------------------------------------
\ Gestor de la trama global

: plot
  \ Trama global.
  \ Nota: Las subtramas deben comprobarse en orden cronológico:
  [ambush_in_global_plot?] [if] 
    ambush? if  ambush exit  then
  [then]
  \ Pendiente!!! La trama de la batalla sería adecuada para una trama global de escenario,
  \ invocada desde aquí. Aquí quedarían solo las tramas generales que no dependen de 
  \ ningún escenario.
  battle? if  battle exit  then
  [dark_cave_in_global_plot?] [if]
    dark_cave? if  dark_cave exit  then
  [then]
  ambrosio_must_follow? if  ambrosio_must_follow  then
  ;

\ }}} ##########################################################
section( Descripciones especiales)  \ {{{

(

Esta sección contiene palabras que muestran descripciones
que necesitan un tratamiento especial porque hacen
uso de palabras relacionadas con la trama.

En lugar de crear vectores para las palabras que estas
descripciones utilizan, es más sencillo crearlos para las
descripciones y definirlas aquí, a continuación de la trama.

)

: officers_forbid_to_steal$
  \ Devuelve una variante de «Tus oficiales detienen el saqueo».
  s{ s" los" s" tus" }s s" oficiales" s&
  s{
  s" intentan detener" s" detienen como pueden"
  s" hacen" s{ s" todo" s? s" lo que pueden" s& s" lo imposible" }s&
    s{ s" para" s" por" }s& s" detener" s&
  }s& s{ s" el saqueo" 2dup s" el pillaje" }s&
  ;
: ^officers_forbid_to_steal$
  \ Devuelve una variante de «Tus oficiales detienen el saqueo» (con la primera mayúscula).
  officers_forbid_to_steal$ ^uppercase
  ;
: (they_do_it)_their_way$  ( -- a u )
  s" ," s{
    s" a su" s{ s" manera" s" estilo" }s&
    s" de la única" way$ s&
    s" que" s& s{ s" saben" s" conocen" }s&
  }s& comma+
  ;
: this_sad_victory$  ( -- a u )
  s" esta" s" tan" s{ s" triste" s" fácil" s" poco honrosa" }s&
  s" victoria" rnd2swap s& s&
  ;
: (soldiers_steal$)  ( a1 u1 -- a2 u2 )
  \ Completa una descripción de tus soldados en la aldea arrasada.
  soldiers$ s& s{ s" aún" s" todavía" }s?&
  s{ s" celebran" s{ s" están" s" siguen" s" continúan" }s s" celebrando" s& }s&
  (they_do_it)_their_way$ s?+ 
  this_sad_victory$ s& s{ s" :" s" ..." }s+
  s{ s" saqueando" s" buscando" s" apropiándose de" s" robando" }s&
  s" todo" s?& s" cuanto de valor" s&
  s" aún" s?& s{ s" quede" s" pueda quedar" }s&
  s" entre" s& rests_of_the_village$ s&
  ;
: soldiers_steal$  ( -- a u )
  \ Devuelve una descripción de tus soldados en la aldea arrasada.
  all_your$ (soldiers_steal$)
  ;
: ^soldiers_steal$  ( -- a u )
  \ Devuelve una descripción de tus soldados en la aldea arrasada (con la primera mayúscula).
  ^all_your$ (soldiers_steal$)
  ;
: soldiers_steal_spite_of_officers_0$  ( -- a u )
  \ Devuelve la primera versión de la descripción de los soldados en la aldea.
  ^soldiers_steal$ period+
  ^officers_forbid_to_steal$ s&
  ;
: soldiers_steal_spite_of_officers_1$  ( -- a u )
  \ Devuelve la segunda versión de la descripción de los soldados en la aldea.
  ^soldiers_steal$
  s{ s" , mientras" s" que" s?&
  s{ s" ; mientras" s" . Mientras" }s s" tanto" s?& comma+
  s" . Al mismo tiempo," }s+
  officers_forbid_to_steal$ s&
  ;
: soldiers_steal_spite_of_officers_2$  ( -- a u )
  \ Devuelve la tercera versión de la descripción de los soldados en la aldea.
  \ No se usa!!! La frase queda incoherente en algunos casos.
  ^officers_forbid_to_steal$
  s" , pero" s+ s" a pesar de ello" s?&
  soldiers_steal$ s&
  ;
: soldiers_steal_spite_of_officers$  ( -- a u )
  \ Devuelve una descripción de tus soldados en la aldea arrasada.
  ['] soldiers_steal_spite_of_officers_0$
  ['] soldiers_steal_spite_of_officers_1$
  2 choose execute
  ;
: soldiers_steal_spite_of_officers
  \ Describe a tus soldados en la aldea arrasada.
  soldiers_steal_spite_of_officers$ period+ paragraph
  ;
: will_follow_you_forever$
  \ Describe a tus hombres durante el regreso a casa, sin citarlos.
  s" te seguirían hasta el"
  s{ s{ s" mismo" s" mismísimo" }s s" infierno" s&
  s" último rincón de la Tierra"
  }s& 
  ;
: will_follow_you_forever  ( a u -- )
  \ Completa e imprime la descripción de soldados u oficiales.
  \ a u = Sujeto de la frase
  will_follow_you_forever$ s& period+ paragraph
  ;
: soldiers_go_home
  \ Describe a tus soldados durante el regreso a casa.
  ^all_your$ soldiers$ s& will_follow_you_forever
  ;
: officers_go_home
  \ Describe a tus soldados durante el regreso a casa.
  ^all_your$ officers$ s&
  s" , como"
  s{ s" el resto de tus" all_your$ }s& soldiers$ s& comma+ s?+
  will_follow_you_forever
  ;
: (soldiers_description)
  \ Describe a tus soldados.
  true case
    still_in_the_village? of  soldiers_steal_spite_of_officers  endof
\   back_to_the_village? of  soldiers_go_home  endof  \ No se usa!!!
    pass_still_open? of  soldiers_go_home  endof
\   battle? of  battle_phase  endof  \ No se usa!!! Redundante, porque tras la descripción se mostrará otra vez la situación de la batalla
  endcase
  ;
' (soldiers_description) is soldiers_description
: (officers_description)
  \ Describe a tus soldados.
  true case
    still_in_the_village? of  ^officers_forbid_to_steal$  endof
\   back_to_the_village? of  officers_go_home  endof  \ No se usa!!!
    pass_still_open? of  officers_go_home  endof
\   battle? of  battle_phase  endof  \ No se usa!!! Redundante, porque tras la descripción se mostrará otra vez la situación de la batalla
  endcase
  ;
' (officers_description) is officers_description

\ }}} ##########################################################
section( Errores del intérprete de comandos)  \ {{{

: please$  ( -- a u )
  \ Devuelve «por favor» o vacía.
  s" por favor" s?
  ;
: (please&)  ( a1 u1 a2 u2 -- a3 u3 )
  \ Añade una cadena al inicio o al final de otra, con una coma de separación.
  \ a1 u1 = Cadena principal
  \ a2 u2 = Cadena que se añadirá a la principal
  2 random if  2swap  then
  comma+ 2swap s&
  ;
: please&  ( a1 u1 -- a1 u1 | a2 u2 )
  \ Añade «por favor» al inicio o al final de una cadena, con una coma de separación; o la deja sin tocar.
  please$ dup if  (please&)  else  2drop  then
  ;
: in_the_sentence$  ( -- a u )
  \ Devuelve una variante de «en la frase» (o una cadena vacía).
  s{ "" s" en la frase" s" en el comando" s" en el texto" }s
  ;
: error_comment_0$  ( -- a u )
  \ Devuelve la variante 0 del mensaje de acompañamiento para los errores lingüísticos.
  s" sé más clar" player_gender_ending$+
  ;
: error_comment_1$  ( -- a u )
  \ Devuelve la variante 1 del mensaje de acompañamiento para los errores lingüísticos.
  s{ s" exprésate" s" escribe" }s
  s{
  s" más claramente"
  s" más sencillamente"
  s{ s" con más" s" con mayor" }s
  s{ s" sencillez" s" claridad" }s&
  }s s&
  ;
: error_comment_2$  ( -- a u )
  \ Devuelve la variante 2 del mensaje de acompañamiento para los errores lingüísticos.
  \ Comienzo común:
  s{ s" intenta" s" procura" s" prueba a" }s
  s{ s" reescribir" s" expresar" s" escribir" s" decir" }s&
  \ Pendiente!!! este "lo" crea problema de concordancia con el final de la frase:
  s{ s"  la frase" s" lo" s"  la idea" }s+
  s{
  \ Final 0:
  s{ s" de" s" otra" }s way$ s&? 
  s{ "" s" un poco" s" algo" }s& s" más" s&
  s{ s" simple" s" sencilla" s" clara" }s&
  \ Final 1:
  s{ s" más claramente" s" con más sencillez" }s
  }s&
  ;
: error_comment$  ( -- a u )
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos.
  error_comment_0$ error_comment_1$ error_comment_2$
  3 schoose please&
  ;
: ^error_comment$  ( -- a u )
  \ Devuelve mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula.
  error_comment$ ^uppercase
  ;
: verbose_language_error  ( a u -- )
  \ Muestra un mensaje detallado sobre un error lingüístico,
  \ combinándolo con una frase común.
  \ a u = Mensaje de error detallado
  \ Inacabado!!! Hacer que use coma o punto y coma, al azar
  in_the_sentence$ s&  3 random
  if  ^uppercase period+ ^error_comment$
  else  ^error_comment$ comma+ 2swap
  then  period+ s&  report
  ;
: short_language_error
  \ Muestra un mensaje breve y genérico sobre un error lingüístico.
  \ Inacabado!!!
  s" Frase incorrecta" period+ report
  ;
: language_error  ( a u -- )
  \ Muestra un mensaje sobre un error lingüístico,
  \ detallado o breve según la configuración.
  \ a u = Mensaje de error detallado
  verbose_language_errors? @
  if  verbose_language_error
  else  2drop short_language_error
  then
  ;
: there_are$  ( -- a u )
  \ Devuelve una variante de «hay» para sujeto plural, comienzo de varios errores.
  s{ s" parece haber" s" se identifican" s" se reconocen" }s
  ;
: there_is$  ( -- a u )
  \ Devuelve una variante de «hay» para sujeto singular, comienzo de varios errores.
  s{ s" parece haber" s" se identifica" s" se reconoce" }s
  ;
: there_is_no$  ( -- a u )
  \ Devuelve una variante de «no hay», comienzo de varios errores.
  s" no se" s{ s" identifica" s" encuentra" s" reconoce" }s&
  s{ s" el" s" ningún" }s&
  ;
: too_many_actions
  \ Informa de que se ha producido un error porque hay dos verbos en el comando.
  s{ there_are$ s" dos verbos" s&
  there_is$ s" más de un verbo" s&
  there_are$ s" al menos dos verbos" s&
  }s  language_error
  ;
' too_many_actions constant (too_many_actions_error#)
' (too_many_actions_error#) is too_many_actions_error#
: too_many_complements
  \ Informa de que se ha producido un error
  \ porque hay dos complementos secundarios en el comando.
  \ Provisional!!!
  s{
  there_are$
  s" dos complementos secundarios" s&
  there_is$
  s" más de un complemento secundario" s&
  there_are$
  s" al menos dos complementos secundarios" s&
  }s  language_error
  ;
' too_many_complements constant (too_many_complements_error#)
' (too_many_complements_error#) is too_many_complements_error#
: no_verb
  \ Informa de que se ha producido un error por falta de verbo en el comando.
  there_is_no$ s" verbo" s& language_error
  ;
' no_verb constant (no_verb_error#)
' (no_verb_error#) is no_verb_error#
: no_main_complement
  \ Informa de que se ha producido un error por falta de complemento principal en el comando.
  there_is_no$ s" complemento principal" s& language_error
  ;
' no_main_complement constant (no_main_complement_error#)
' (no_main_complement_error#) is no_main_complement_error# 
: unexpected_main_complement
  \ Informa de que se ha producido un error
  \ por la presencia de complemento principal en el comando.
  there_is$ s" un complemento principal" s&
  s" pero el verbo no puede llevarlo" s&
  language_error
  ;
' unexpected_main_complement constant (unexpected_main_complement_error#)
' (unexpected_main_complement_error#) is unexpected_main_complement_error# 
: unexpected_secondary_complement
  \ Informa de que se ha producido un error
  \ por la presencia de complemento secundario en el comando.
  there_is$ s" un complemento secundario" s&
  s" pero el verbo no puede llevarlo" s&
  language_error
  ;
' unexpected_secondary_complement constant (unexpected_secondary_complement_error#)
' (unexpected_secondary_complement_error#) is unexpected_secondary_complement_error# 
: not_allowed_main_complement
  \ Informa de que se ha producido un error
  \ por la presencia de un complemento principal en el comando
  \ que no está permitido.
  there_is$ s" un complemento principal no permitido con esta acción" s&
  language_error
  ;
' not_allowed_main_complement constant (not_allowed_main_complement_error#)
' (not_allowed_main_complement_error#) is not_allowed_main_complement_error# 
: not_allowed_tool_complement
  \ Informa de que se ha producido un error
  \ por la presencia de un complemento instrumental en el comando
  \ que no está permitido.
  there_is$ s" un complemento principal no permitido con esta acción" s&
  language_error
  ;
' not_allowed_tool_complement constant (not_allowed_tool_complement_error#)
' (not_allowed_tool_complement_error#) is not_allowed_tool_complement_error# 
: useless_tool
  \ Informa de que se ha producido un error
  \ porque una herramienta no especificada no es la adecuada.
  \ Inacabado!!!
  s" [Con eso no puedes]"
  narrate
  ;
' useless_tool constant (useless_tool_error#)
' (useless_tool_error#) is useless_tool_error# 
: useless_what_tool
  \ Informa de que se ha producido un error
  \ porque el ente WHAT no es la herramienta adecuada.
  \ Inacabado!!!
  \ Distinguir si la llevamos, si está presente, si es conocida...!!!
  s" [Con" what @ full_name s& s" no puedes]" s&
  narrate
  ;
' useless_what_tool constant (useless_what_tool_error#)
' (useless_what_tool_error#) is useless_what_tool_error# 
: unresolved_preposition
  \ Informa de que se ha producido un error
  \ porque un complemento (seudo)preposicional quedó incompleto.
  there_is$ s" un complemento (seudo)preposicional sin completar" s&
  language_error
  ;
' unresolved_preposition constant (unresolved_preposition_error#)
' (unresolved_preposition_error#) is unresolved_preposition_error# 
: repeated_preposition
  \ Informa de que se ha producido un error por
  \ la repetición de una (seudo)preposición.
  there_is$ s" una (seudo)preposición repetida" s&
  language_error
  ;
' repeated_preposition constant (repeated_preposition_error#)
' (repeated_preposition_error#) is repeated_preposition_error# 

: ?wrong  ( xt | 0 -- )
  \ Informa, si es preciso, de un error en el comando.
  \ xt = Dirección de ejecución de la palabra de error (que se usa también como código del error)
  [debug_catch] [debug_parsing] [or] ?halto" Al entrar en ?WRONG"  \ Depuración!!!
  ?dup if  execute  then
  [debug_catch] [debug_parsing] [or] ?halto" Al salir de ?WRONG"  \ Depuración!!!
  ;

\ }}} ##########################################################
section( Herramientas para crear las acciones)  \ {{{

\ ------------------------------------------------
subsection( Pronombres)  \ {{{

\ Pendiente!!!:
\ Mover esto a la sección del intérprete.

variable last_action  \ Última acción utilizada por el jugador
create last_complement  \ Tabla para últimos complementos usados
\ Se necesita cinco celdas:
\ una para el último complemento usado
\ y cuatro para cada último complemento usado de cada género y número.
\ El espacio se multiplica por dos
\ para guardar en la segunda mitad los penúltimos complementos.
5 cells 2*  dup constant /last_complements  allot \ Octetos necesarios para toda la tabla

(

La tabla LAST_COMPLEMENT sirve para guardar los
identificadores de entes correspondientes a los últimos
complementos utilizados en los comandos del jugador. De este
modo los pronombres podrán recuperarlos.

La estructura de la tabla LAST_COMPLEMENT es la siguiente,
con desplazamiento indicado en celdas:

Último complemento usado:
  +0 De cualquier género y número.
  +1 Masculino singular.
  +2 Femenino singular.
  +3 Masculino plural.
  +4 Femenino plural.
Penúltimo complemento usado:
  +5 De cualquier género y número.
  +6 Masculino singular.
  +7 Femenino singular.
  +8 Masculino plural.
  +9 Femenino plural.

)

\ Desplazamientos para acceder a los elementos de la tabla:
1 cells constant />masculine_complement  \ Respecto al inicio de tabla
2 cells constant />feminine_complement  \ Respecto al inicio de tabla
0 cells constant />singular_complement  \ Respecto a su género en singular
2 cells constant />plural_complement  \ Respecto a su género en singular
5 cells constant />but_one_complement  \ Respecto a la primera mitad de la tabla
: >masculine  ( a1 -- a2 )  />masculine_complement +  ;
: >feminine  ( a1 -- a2 )  />feminine_complement +  ;
: >singular  ( a -- a )  />singular_complement +  ;
: >plural  ( a -- a )  />plural_complement +  ;
: >but_one  ( a1 -- a2 )  />but_one_complement +  ;

: last_but_one_complement  ( - a )
  \ Devuelve la dirección del penúltimo complemento absoluto,
  \ que es también el inicio de la sección «penúltimos»
  \ de la tabla 'last_complements'.
  last_complement >but_one
  ;
: (>last_complement)  ( a1 a2 -- a3 )
  \ Apunta a la dirección adecuada para un ente
  \ en una sección de la tabla 'last_complement',
  \ bien «últimos» o «penúltimos».
  \ Nota: Hace falta sumar los desplazamientos de ambos géneros
  \ debido a que ambos son respecto al inicio de la tabla.
  \ El desplazamiento para singular no es necesario,
  \ pues sabemos que es cero, a menos que se cambie la estructura.
  \ a1 = Ente para el que se calcula la dirección
  \ a2 = Dirección de una de las secciones de la tabla
  over has_feminine_name? />feminine_complement and +
  over has_masculine_name? />masculine_complement and +
  swap has_plural_name? />plural_complement and +
  ;
: >last_complement  ( a1 -- a2 )
  \ Apunta a la dirección adecuada para un ente
  \ en la sección «últimos» de la tabla 'last_complement'.
  last_complement (>last_complement)
  ;
: >last_but_one_complement  ( a1 -- a2 )
  \ Apunta a la dirección adecuada para un ente
  \ en la sección «penúltimos» de la tabla 'last_complement'.
  last_but_one_complement (>last_complement)
  ;

: erase_last_command_elements
  \ Borra todos los últimos elementos guardados de los comandos.
  last_action off
  last_complement /last_complements erase
  ;

\ }}}---------------------------------------------
subsection( Herramientas para la creación de acciones)  \ {{{

(

Los nombres de las acciones empiezan por el prefijo «do_»
[algunas palabras secundarias de las acciones 
también usan el mismo prefijo].

Pendiente!!! explicación sobre la sintaxis

)

: action:  ( "name" -- )
  \ Crear un identificador de acción.
  \ "name" = nombre del identificador de la acción, en el flujo de entrada
  create  \ Crea una palabra con el nombre indicado...
    ['] noop ,  \ ...y guarda en su campo de datos (pfa) la dirección de ejecución de NOOP
  does>  ( pfa -- )  \ Cuando la palabra sea llamada tendrá su pfa en la pila...
    perform  \ ...y ejecutará la dirección de ejecución que contenga
  ;
: :action  ( "name" -- )
  \ Inicia la definición de una palabra que ejecutará una acción.
  \ "name" = nombre del identificador de la acción, en el flujo de entrada
  :noname  \ Crea una palabra sin nombre para la acción
  4 roll  ( xt )
  \ Guardar la dirección de ejecución
  \ en el campo de datos del identificador de la acción:
  ' >body !
  ;
: ;action
  postpone ;
  ;  immediate 

\ }}}---------------------------------------------
subsection( Comprobación de los requisitos de las acciones)  \ {{{

(

En las siguientes palabras usamos las llaves en sus nombres
como una notación, para hacer más legible y más fácil de
modificar el código.  El texto entre las llaves indica la
condición que se ha de cumplir.

Si la condición no se cumple, se provocará un error con
THROW que devolverá el flujo al último 'catch'.

Este sistema de filtros y errores permite simplificar el
código de las acciones porque ahorra muchas estructuras
condicionales anidadas.

)

: main_complement{forbidden}
  \ Provoca un error si hay complemento principal.
  main_complement @
  0<> unexpected_main_complement_error# and throw
  ;
: secondary_complement{forbidden}
  \ Provoca un error si hay complemento secundario.
  secondary_complement @
  0<> unexpected_secondary_complement_error# and throw
  ;
: main_complement{required}
  \ Provoca un error si no hay complemento principal.
  main_complement @
  [false] ?halto" main_complement{required} 1"
  0= no_main_complement_error# and throw
  [false] ?halto" main_complement{required} 2"
  ;
: main_complement{this_only}  ( a -- )
  \ Provoca un error si hay complemento principal y no es el indicado.
  \ a = Ente que será aceptado como complemento
  main_complement @ swap over different?
  not_allowed_main_complement_error# and throw
  ;
: different_tool?  ( a -- ff )
  \ ¿Es el ente diferente a la herramienta usada, si la hay?
  \ a = Ente
  tool_complement @ swap over different?
  ;
: tool_complement{this_only}  ( a -- )
  \ Provoca un error (lingüístico)
  \ si hay complemento instrumental y no es el indicado.
  \ a = Ente que será aceptado como complemento instrumental
  different_tool? not_allowed_tool_complement_error# and throw
  ;
: tool{not_this}  ( a -- )
  \ Provoca un error (narrativo) si se usa cierta herramienta.
  \ a = Ente que no será aceptado como herramienta
  \ No se usa!!!
  dup what !
  different_tool? 0= useless_what_tool_error# and throw
  ;
: tool{this_only}  ( a -- )
  \ Provoca un error (narrativo) si no se usa cierta herramienta.
  \ a = Ente que será aceptado como herramienta
  tool_complement @ what !
  different_tool? useless_what_tool_error# and throw
  ;
: tool_complement{unnecessary}
  \ Provoca un error si hay un complemento instrumental.
  tool_complement @ ?dup if  unnecessary_tool  then
  ;
: tool_complement{unnecessary_for_that}  ( a u -- )
  \ Provoca un error si hay un complemento instrumental.
  \ a u = Acción para la que sobra el complemento
  \       (una frase con verbo en infinitivo)
  tool_complement @ ?dup
  if  unnecessary_tool_for_that  else  2drop  then
  ;
: {hold}  ( a -- )
  \ Provoca un error si un ente no está en inventario.
  dup what !
  is_hold? 0= you_do_not_have_what_error# and throw
  ;
: ?{hold}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está en inventario.
  ?dup if  {hold}  then
  ;
: main_complement{hold}
  \ Provoca un error si el complemento principal existe y no está en inventario.
  main_complement @ ?{hold}
  ;
: tool_complement{hold}
  \ Provoca un error si el complemento instrumental existe y no está en inventario.
  tool_complement @ ?{hold}
  ;
: {not_hold}  ( a -- )
  \ Provoca un error si un ente está en inventario.
  dup what !
  is_hold? you_already_have_what_error# and throw
  ;
: ?{not_hold}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y está en inventario.
  ?dup if  {not_hold}  then
  ;
: main_complement{not_hold}
  \ Provoca un error si el complemento principal existe y está en inventario.
  main_complement @ ?{not_hold}
  ;
: {worn}  ( a -- )
  \ Provoca un error si un ente no lo llevamos puesto.
  dup what !
  is_worn_by_me? 0= you_do_not_wear_what_error# and throw
  ;
: ?{worn}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no lo llevamos puesto.
  ?dup if  {worn}  then
  ;
: main_complement{worn}
  \ Provoca un error si el complemento principal existe y no lo llevamos puesto.
  main_complement @ ?{worn}
  ;
: {open}  ( a -- )
  \ Provoca un error si un ente no está abierto.
  dup what !
  is_closed? what_is_already_closed_error# and throw
  ;
: {closed}  ( a -- )
  \ Provoca un error si un ente no está cerrado.
  dup what !
  is_open? what_is_already_open_error# and throw
  ;
: {not_worn}  ( a -- )
  \ Provoca un error si un ente lo llevamos puesto.
  dup what !
  is_worn_by_me? you_already_wear_what_error# and throw
  ;
: ?{not_worn}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y lo llevamos puesto.
  ?dup if  {not_worn}  then
  ;
: main_complement{not_worn}
  \ Provoca un error si el complemento principal existe y lo llevamos puesto.
  main_complement @ ?{not_worn}
  ;
: {cloth}  ( a -- )
  \ Provoca un error si un ente no se puede llevar puesto.
  is_cloth? 0= nonsense_error# and throw
  ;
: ?{cloth}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no se puede llevar puesto.
  ?dup if  {cloth}  then
  ;
: main_complement{cloth}
  \ Provoca un error si el complemento principal existe y no se puede llevar puesto.
  main_complement @ ?{cloth}
  ;
: {here}  ( a -- )
  \ Provoca un error si un ente no está presente.
  dup what !
  is_here? 0= is_not_here_what_error# and throw
  ;
: ?{here}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está presente.
  ?dup if  {here}  then
  ;
: main_complement{here}
  \ Provoca un error si el complemento principal existe y no está presente.
  main_complement @ ?{here}
  ;
: {accessible}  ( a -- )
  \ Provoca un error si un ente no está accessible.
  [false] ?halto" {accessible} 1"
  dup what !
  is_not_accessible?
  [false] ?halto" {accessible} 1a"
  cannot_see_what_error# and
  [false] ?halto" {accessible} 1b"
  throw
  [false] ?halto" {accessible} 2"
  ;
: ?{accessible}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está accessible.
  [false] ?halto" ?{accessible} 1"
  ?dup if  {accessible}  then
  [false] ?halto" ?{accessible} 1"
  ;
: main_complement{accessible}
  \ Provoca un error si el complemento principal existe y no está accessible.
  [false] ?halto" main_complement{accessible} 1"
  main_complement @ ?{accessible}
  [false] ?halto" main_complement{accessible} 2"
  ;
: {takeable}  ( a -- )
  \ Provoca un error si un ente no puede ser tomado.
  \ Nota: los errores apuntados por el campo ~TAKE_ERROR# no reciben parámetros salvo en WHAT
  dup what !
  ~take_error# @ throw  \ Error específico del ente
  can_be_taken? 0= nonsense_error# and throw  \ Condición general de error
  ;
: ?{takeable}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no puede ser tomado.
  ?dup if  {takeable}  then
  ;
: main_complement{takeable}
  \ Provoca un error si el complemento principal existe y no puede ser tomado.
  main_complement @ ?{takeable}
  ;
: {broken}  ( a -- )
  \ Provoca un error si un ente no puede ser roto.
  \ Nota: los errores apuntados por el campo ~BREAK_ERROR# no reciben parámetros salvo en WHAT
  dup what ! ~break_error# @ throw
  ;
: ?{broken}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no puede ser roto.
  ?dup if  {broken}  then
  ;
: main_complement{broken}
  \ Provoca un error si el complemento principal existe y no puede ser roto.
  main_complement @ ?{broken}
  ;
: {looked}  ( a -- )
  \ Provoca un error si un ente no puede ser mirado.
  \ Nota: los errores apuntados por el campo ~TAKE_ERROR# no deben necesitar parámetros, o esperarlo en WHAT
  dup what !
  can_be_looked_at? 0= cannot_see_what_error# and throw
  ;
: ?{looked}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no puede ser mirado.
  ?dup if  {looked}  then
  ;
: main_complement{looked}
  \ Provoca un error si el complemento principal existe y no puede ser mirado.
  main_complement @ ?{looked}
  ;
: {living}  ( a -- )
  \ Provoca un error si un ente no es un ser vivo.
  [false] ?halto" {living} 1"
  is_living_being? 0= nonsense_error# and throw
  [false] ?halto" {living} 2"
  ;
: ?{living}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no es un ser vivo.
  [false] ?halto" ?{living} 1"
  ?dup if  {living}  then
  [false] ?halto" ?{living} 2"
  ;
: main_complement{living}
  \ Provoca un error si el complemento principal existe y no es un ser vivo.
  [false] ?halto" main_complement{living} 1"
  main_complement @ ?{living}
  [false] ?halto" main_complement{living} 2"
  ;
: {needed}  ( a -- )
  \ Provoca un error si un ente no está en inventario, pues es necesario.
  dup what !
  is_hold? 0= you_need_what_error# and throw
  ;
: ?{needed}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no está en inventario, pues es necesario.
  ?dup if  {needed}  then
  ;
: main_complement{needed}
  \ Provoca un error si el complemento principal existe y no está en inventario, pues lo necesitamos.
  main_complement @ ?{needed}
  ;
: {direction}  ( a -- )
  \ Provoca un error si un ente no es una dirección.
  dup what !
  is_direction? 0= nonsense_error# and throw
  ;
: ?{direction}  ( a | 0 -- )
  \ Provoca un error si un supuesto ente lo es y no es una dirección.
  ?dup if  {direction}  then
  ;
: main_complement{direction}
  \ Provoca un error si el complemento principal existe y no es una dirección.
  main_complement @ ?{direction}
  ;


\ }}}
\ }}} ##########################################################
section( Acciones)  \ {{{

(

Para crear una acción, primero es necesario crear su
identificador con la palabra 'action:', que funciona de forma
parecida a 'defer'. Después hay que definir la palabra de la
acción con las palabras previstas para ello, que se ocupan
de darle al identificador el valor de ejecución
correspondiente. Ejemplo de la sintaxis:

action: identificador

:action identificador
  \ definición de la acción
  ;action

Todos los identificadores deben ser creados antes de las
definiciones, pues su objetivo es posibilitar que las
acciones se llamen unas a otras sin importar el orden en que
estén definidas en el código fuente.

)

\ ------------------------------------------------
subsection( Identificadores)  \ {{{

action: (do_finish)  \ Esta acción se define en la sección de finales; útil solo durante el desarrollo
action: do_attack
action: do_break
action: do_climb
action: do_close
action: do_do
action: do_drop
action: do_examine
action: (do_exits)  ' (do_exits) is do_exits
action: do_fear  \ Confirmar traducción!!!
action: do_finish  \ Esta acción se define en la sección de finales
action: do_go
action: do_go_ahead
action: do_go_back
action: do_go_down
action: do_go_east
action: do_go_in
action: do_go_north
action: do_go|do_break
action: do_go_out
action: do_go_south
action: do_go_up
action: do_go_west
action: do_hit
action: do_introduce_yourself
action: do_inventory
action: do_kill
action: do_load_the_game
action: do_look
action: do_look_to_direction
action: do_look_yourself
action: do_make
action: do_open
action: do_put_on
action: do_save_the_game
action: do_search
action: do_sharpen
action: do_speak
action: do_swim
action: do_take
action: do_take|do_eat \ !!! Cambiar do_eat por ingerir
action: do_take_off

\ }}}---------------------------------------------
subsection( Herramientas para averiguar complemento omitido)  \ {{{

: whom  ( -- a | 0 )
  \ Devuelve un ente personaje al que probablemente se refiera un comando.
  \ Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica.
  \ Pendiente!!! Ampliar para contemplar los soldados y oficiales, según la trama, el escenario y la fase de la batalla.
  true case
    ambrosio% is_here? of  ambrosio%  endof
    leader% is_here? of  leader%  endof
    false swap
  endcase
  ;
: unknown_whom  ( -- a | 0 )
  \ Devuelve un ente personaje desconocido
  \ al que probablemente se refiera un comando.
  \ Se usa para averiguar el objeto de algunas acciones
  \ cuando el jugador no lo especifica
  true case
    ambrosio% is_here_and_unknown? of  ambrosio%  endof
    leader% is_here_and_unknown? of  leader%  endof
    false swap
  endcase
  ;

\ }}}---------------------------------------------
subsection( Mirar, examinar y registrar)  \ {{{

: (do_look)  ( a -- )
  \ Mira un ente.
  dup describe
  dup is_location? if  .present  then  familiar++ 
  ;
:action do_look
  \  Acción de mirar.
  tool_complement{unnecessary} 
  main_complement @ ?dup 0= if  my_location  then
  dup {looked} (do_look)  
  ;action
:action do_look_yourself
  \  Acción de mirarse.
  tool_complement{unnecessary} 
  main_complement @ ?dup 0= if  protagonist%  then
  (do_look)
  ;action
:action do_look_to_direction
  \  Acción de otear.
  \ Pendiente!!! traducir «otear» en el nombre de la palabra
  tool_complement{unnecessary} 
  main_complement{required}
  main_complement{direction}
  main_complement @ (do_look)
  ;action
:action do_examine
  \ Acción de examinar.
  \ Provisional!!!
  do_look
  ;action
:action do_search
  \ Acción de registrar.
  \ Provisional!!!
  do_look
  ;action

\ }}}---------------------------------------------
subsection( Salidas)  \ {{{

\ Inacabado!!! No se usa!!!
create do_exits_table_index  \ Tabla para desordenar el listado de salidas
#exits cells allot
\ Esta tabla permitirá que las salidas se muestren cada vez en un orden diferente

variable #free_exits  \ Contador de las salidas posibles
: no_exit$  ( -- a u )
  \ Devuelve mensaje usado cuando no hay salidas que listar.
  s" No hay"
  s{ s" salidas" s" salida" s" ninguna salida" }s&
  ;
: go_out$  ( -- a u )
  s{ s" salir" s" seguir" }s
  ;
: go_out_to& ( a u -- a1 u1 )
  go_out$ s& s" hacia" s&
  ;
: one_exit_only$  ( -- a u )
  \ Devuelve mensaje usado cuando solo hay una salidas que listar.
  s{
  s" La única salida" possible1$ s& s" es" s& s" hacia" s?&
  ^only$ s" hay salida" s& possible1$ s& s" hacia" s&
  ^only$ s" es posible" s& go_out_to&
  ^only$ s" se puede" s& go_out_to&
  }s
  ;
: possible_exits$  ( -- a u )
  s" salidas" possible2$ s& 
  ;
: several_exits$  ( -- a u )
  \ Devuelve mensaje usado cuando hay varias salidas que listar.
  s{
  s" Hay" possible_exits$ s& s" hacia" s&
  s" Las" possible_exits$ s& s" son" s&
  }s
  ;
: .exits
  \ Imprime las salidas posibles.
  #listed @ case
    0 of  no_exit$  endof
    1 of  one_exit_only$  endof
    several_exits$ rot
  endcase
  «& «»@ period+ narrate
  ;
: exit_separator$  ( -- a u )
  \ Devuelve el separador adecuado a la salida actual.
  #free_exits @ #listed @ list_separator$
  ;
: exit>list  ( u -- )
  \ Lista una salida.
  \ u = Puntero a un campo de dirección (desplazamiento relativo desde el inicio de la ficha)
  [debug_do_exits] [if]  cr ." exit>list" cr .stack  [then]  \ Depuración!!!
  exit_separator$ »+
  exits_table@ full_name »+
  #listed ++
  [debug_do_exits] [if]  cr .stack  [then]  \ Depuración!!!
  ;

false [if]  \ Primera versión

\ Las salidas se listan siempre en el mismo orden en el que
\ están definidas en las fichas.

: free_exits  ( a -- u )
  \ Devuelve el número de salidas posibles de un ente.
  [debug_do_exits] [if]  cr ." free_exits" cr .stack  [then]  \ Depuración!!!
  0 swap
  ~first_exit /exits bounds do
\   [debug_do_exits] [if]  i i cr . @ .  [then]  \ Depuración!!!
    i @ 0<> abs +
  cell  +loop 
  [debug_do_exits] [if]  cr .stack  [then]  \ Depuración!!!
  ;

:action (do_exits)
  \ Acción de listar las salidas posibles de la localización del protagonista.
  «»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
  #listed off
  my_location dup free_exits #free_exits !
  last_exit> 1+ first_exit> do
\   [debug_do_exits]  ?[false] ?halto" do_exits 1"  \ Depuración!!!
    dup i + @
\   [debug_do_exits]  ?[false] ?halto" do_exits 2"  \ Depuración!!!
    if  i exit>list  then
  cell  +loop  drop
  .exits
  ;action

[else]  \ Segunda versión

\ Las salidas se muestran cada vez en orden aleatorio.

0 value this_location  \ Guardará el ente del que queremos calcular las salidas libres (para simplificar el manejo de la pila en el bucle)
: free_exits  ( a0 -- a1 ... au u )
  \ Devuelve el número de salidas posibles de un ente.
  \ a0 = Ente
  \ a1 ... au = Entes de salida del ente a0
  \ u = número de entes de salida del ente a0
  [debug_do_exits] [if]  cr ." free_exits" cr .stack  [then]  \ Depuración!!!
  to this_location  depth >r
  last_exit> 1+ first_exit> do
    this_location i + @ if  i  then
  cell  +loop 
  depth r> -
  [debug_do_exits] [if]  cr .stack  [then]  \ Depuración!!!
  ;
: (list_exits)
  \ Crea la lista de salidas y la imprime
  «»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
  #listed off
  my_location free_exits
  dup >r unsort r>  dup #free_exits !
  0 ?do  exit>list  loop  .exits
  ;
' (list_exits) is list_exits
:action (do_exits)
  \ Lista las salidas posibles de la localización del protagonista.
  \ Comprobar la conveniencia de posibles complementos:
  tool_complement{unnecessary}
  secondary_complement{forbidden}
  main_complement @ ?dup if
    dup my_location <> swap direction 0= and
    nonsense_error# and throw
  then  list_exits
  ;action

[then]

\ }}}---------------------------------------------
subsection( Ponerse y quitarse prendas)  \ {{{

: (do_put_on)  ( a -- )
  \ Ponerse una prenda.
  is_worn  well_done
  ;
:action do_put_on
  \ Acción de ponerse una prenda.
  \ Pendiente!!! Hacer que tome la prenda si no la tiene
  tool_complement{unnecessary}
  main_complement{required}
  main_complement{cloth}
  main_complement{not_worn}
  main_complement{hold}
  main_complement @ (do_put_on)
  ;action
: (do_take_off)  ( a -- )
  \ Quitarse una prenda.
  is_not_worn  well_done
  ;
:action do_take_off
  \ Acción de quitarse una prenda.
  tool_complement{unnecessary}
  main_complement{required}
  main_complement{worn}
  main_complement @ (do_take_off)
  ;action

\ }}}---------------------------------------------
subsection( Tomar y dejar)  \ {{{

\ Antiguo!!! Puede que aún sirva:
\ : cannot_take_the_altar  \ No se puede tomar el altar
\   s" [el altar no se toca]" narrate  \ tmp!!!
\   impossible
\   ;
\ : cannot_take_the_flags  \ No se puede tomar las banderas
\   s" [las banderas no se tocan]" narrate  \ tmp!!!
\   nonsense
\   ;
\ : cannot_take_the_idol  \ No se puede tomar el ídolo
\   s" [el ídolo no se toca]" narrate  \ tmp!!!
\   impossible
\   ;
\ : cannot_take_the_door  \ No se puede tomar la puerta
\   s" [la puerta no se toca]" narrate  \ tmp!!!
\   impossible
\   ;
\ : cannot_take_the_fallen_away  \ No se puede tomar el derrumbe
\   s" [el derrumbe no se toca]" narrate  \ tmp!!!
\   nonsense
\   ;
\ : cannot_take_the_snake  \ No se puede tomar la serpiente
\   s" [la serpiente no se toca]" narrate  \ tmp!!!
\   dangerous
\   ;
\ : cannot_take_the_lake  \ No se puede tomar el lago
\   s" [el lago no se toca]" narrate  \ tmp!!!
\   nonsense
\   ;
\ : cannot_take_the_lock  \ No se puede tomar el candado
\   s" [el candado no se toca]" narrate  \ tmp!!!
\   impossible
\   ;
\ : cannot_take_the_water_fall  \ No se puede tomar la cascada
\   s" [la cascada no se toca]" narrate  \ tmp!!!
\   nonsense
\   ;
: (do_take)  ( a -- )
  \ Toma un ente.
  dup is_hold familiar++ well_done
  ;
:action do_take
  \ Toma un ente, si es posible.
  main_complement{required}
  main_complement{not_hold}
  main_complement{here}
  main_complement{takeable}
  main_complement @ (do_take)
  ;action
: (do_drop)  ( a -- )
  \ Deja un ente.
  dup is_not_worn is_here  well_done
  ;
:action do_drop
  \ Acción de dejar.
  main_complement{required}
  main_complement{hold}
  main_complement @ (do_drop)
  ;action
:action do_take|do_eat
  \ Acción de desambiguación.
  \ Pendiente!!!
  do_take
  ;action

\ }}}---------------------------------------------
subsection( Cerrar y abrir)  \ {{{

: first_close_the_door
  \ Informa de que la puerta está abierta
  \ y hay que cerrarla antes de poder cerrar el candado.
  s" cierras" s" primero" rnd2swap s& ^uppercase
  door% full_name s& period+ narrate
  door% is_closed
  ;
: .the_key_fits
  \ Pendiente!!! Nuevo texto, quitar «fácilmente».
  s" La llave gira fácilmente dentro del candado."
  narrate
  ;
: close_the_lock
  \ Cerrar el candado, si es posible.
  key% tool{this_only}
  lock% {open}
  key% {hold}
  door% is_open? if  first_close_the_door  then
  lock% is_closed  .the_key_fits
  ;
: .the_door_closes
  \ Muestra el mensaje de cierre de la puerta.
  s" La puerta"
  s{ s" rechina" s" emite un chirrido" }s&
  s{ s" mientras la cierras" s" al cerrarse" }s&
  period+ narrate
  ;
: (close_the_door)
  \ Cerrar la puerta.
  door% is_closed .the_door_closes 
  location_47% location_48% w|<-->|
  location_47% location_48% o|<-->|
  ;
: close_and_lock_the_door
  \ Cerrar la puerta, si está abierta, y el candado.
  door% {open}  key% {hold}
  (close_the_door) close_the_lock 
  ;
: just_close_the_door
  \ Cerrar la puerta, sin candarla, si está abierta.
  door% {open} (close_the_door) 
  ;
: close_the_door
  \ Cerrar la puerta, si es posible.
  key% tool{this_only}
  tool_complement @ ?dup
  if    close_and_lock_the_door  \ Con llave
  else  just_close_the_door  \ Sin llave
  then
  ;
: close_it  ( a -- )
  \ Cerrar un ente, si es posible.
  case
    door% of  close_the_door  endof
    lock% of  close_the_lock  endof
    nonsense
  endcase
  ;
:action do_close
  \ Acción de cerrar.
  main_complement{required}
  main_complement{accessible}
  main_complement @ close_it
  ;action
: the_door_is_locked
  \ Informa de que la puerta está cerrada por el candado.
  \ Pendiente!!! Añadir variantes.
  lock% ^full_name s" bloquea la puerta." s&
  narrate
  lock_found
  ;
: unlock_the_door
  \ Abrir la puerta candada, si es posible.
  \ Pendiente!!! falta mensaje adecuado sobre la llave que gira.
  the_door_is_locked
  key% {needed}
  lock% dup is_open
  ^pronoun s" abres con" s& key% full_name s& period+ narrate
  ;
: open_the_lock
  \ Abrir el candado, si es posible.
  key% tool{this_only}
  lock% {closed}
  key% {needed}
  lock% is_open  well_done
  ;
: the_plants$  ( -- a u )
  \ Devuelve las plantas que la puerta rompe al abrirse.
  s" las hiedras" s" las hierbas" both
  \ Pendiente!!! hacerlas visibles.
  ;
: the_door_breaks_the_plants_0$  ( -- a u )
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (primera variante).
  s{ s" mientras" s" al tiempo que" }s
  the_plants$ s& s" se rompen en su trazado" s&
  ;
: the_door_breaks_the_plants_1$  ( -- a u )
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta
  \ (segunda variante).
  s" rompiendo" the_plants$ s& s" a su paso" s&
  ;
: the_door_breaks_the_plants$  ( -- a u )
  \ Devuelve el mensaje sobre la rotura de las plantas por la puerta.
  ['] the_door_breaks_the_plants_0$
  ['] the_door_breaks_the_plants_1$
  2 choose execute
  ;
: the_door_sounds$  ( -- a u )
  s{ s" rechinando" s" con un chirrido" }s
  ;
: ambrosio_byes
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
  period+ narrate
  ;
: the_door_opens_first_time$  ( -- a u )
  \ Devuelve el mensaje de apertura de la puerta
  \ la primera vez.
  s" La puerta" s{ s" cede" s" se abre" }s&
  s{ s" despacio" s" poco a poco" s" lentamente" }s&
  s" y no sin" s&
  s{ s" dificultad" s" ofrecer resistencia" }s& comma+
  the_door_sounds$ comma+ s&
  the_door_breaks_the_plants$ s& period+
  ;
: the_door_opens_once_more$  ( -- a u )
  \ Devuelve el mensaje de apertura de la puerta
  \ la segunda y siguientes veces.
  s" La puerta se abre" the_door_sounds$ s& period+
  ;
: .the_door_opens
  \ Muestra el mensaje de apertura de la puerta.
  door% times_open 
  if    the_door_opens_once_more$ narrate
  else  the_door_opens_first_time$ narrate ambrosio_byes
  then  
  ;
: (open_the_door)
  \ Abrir la puerta, que está cerrada.
  key% tool{this_only}  \ Pendiente!!! ¿Por qué aquí?
  lock% is_closed? if  unlock_the_door  then
  location_47% location_48% w<-->
  location_47% location_48% o<-->
  .the_door_opens
  door% dup is_open times_open++
  grass% is_here
  ;
: open_the_door
  \ Abrir la puerta, si es posible.
  door% is_open?
  if  door% it_is_already_open tool_complement{unnecessary}
  else  (open_the_door)
  then
  ;
: open_it  ( a -- )
  \ Abrir un ente, si es posible.
  dup familiar++
  case
    door% of  open_the_door  endof
    lock% of  open_the_lock  endof
    nonsense
  endcase
  ;
:action do_open
  \ Acción de abrir.
  s" do_open" halto  \ Depuración!!!
  main_complement{required}
  main_complement{accessible}
  main_complement @ open_it
  ;action

\ }}}---------------------------------------------
subsection( Agredir)  \ {{{

: the_snake_runs_away
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
  narrate
  ;
: attack_the_snake
  \ Atacar la serpiente.
  \ Inacabado!!!
  sword% {needed}
  the_snake_runs_away
  snake% vanish
  ;
: attack_ambrosio
  \ Atacar a Ambrosio.
  no_reason
  ;
: attack_leader
  \ Atacar al jefe.
  no_reason
  ;
: (do_attack)  ( a -- )
  \ Atacar un ser vivo.
  case
    snake% of  attack_the_snake  endof
    ambrosio% of  attack_ambrosio  endof
    leader% of  attack_leader  endof
    do_not_worry
  endcase
  ;
:action do_attack
  \ Acción de atacar.
  main_complement{required}
  main_complement{accessible}
  main_complement{living} \ Pendiente!!! También es posible atacar otras cosas, como la ciudad u otros lugares, o el enemigo!!!
  tool_complement{hold}
  main_complement @ (do_attack)
  ;action
:action do_fear
  \ Acción de asustar.
  \ Pendiente!!! Distinguir de las demás en grado o requisitos
  main_complement{required}
  main_complement{accessible}
  main_complement{living}
  tool_complement{hold}
  main_complement @ (do_attack)
  ;action
: kill_the_snake
  \ Matar la serpiente.
  sword% {needed}
  the_snake_runs_away
  snake% vanish
  ;
: kill_ambrosio
  \ Matar a Ambrosio.
  no_reason
  ;
: kill_leader
  \ Matar al jefe.
  no_reason
  ;
: kill_your_soldiers
  \ Matar a tus hombres.
  no_reason
  ;
: (do_kill)  ( a -- )
  \ Matar un ser vivo.
  case
    snake% of  kill_the_snake  endof
    ambrosio% of  kill_ambrosio  endof
    leader% of  kill_leader  endof
    soldiers% of  kill_your_soldiers  endof
    do_not_worry
  endcase
  ;
:action do_kill
  \ Acción de matar.
  [false] ?halto" do_kill 1"
  main_complement{required}
  [false] ?halto" do_kill 2"
  main_complement{accessible}
  [false] ?halto" do_kill 3"
  main_complement{living}  \ Pendiente!!! También es posible matar otras cosas, como el enemigo!!!
  [false] ?halto" do_kill 4"
  tool_complement{hold}
  [false] ?halto" do_kill 5"
  main_complement @ (do_kill)
  [false] ?halto" do_kill 6"
  ;action
: break_the_cloak
  \ Romper la capa.
  ;
: (do_break)  ( a -- )
  \ Romper un ente.
  case
    snake% of  kill_the_snake  endof  \ Provisional!!!
    cloak% of  break_the_cloak  endof
    do_not_worry
  endcase
  ;
:action do_break
  \ Acción de romper.
  main_complement{required}
  main_complement{accessible}
  main_complement{broken}
  tool_complement{hold}
  main_complement @ (do_break)
  ;action
:action do_hit
  \ Acción de golpear.
  s" golpear"  main_complement+is_nonsense
  ;action
: can_be_sharpened?  ( a -- ff )
  \ ¿Puede un ente ser afilado?
  \ Pendiente!!! Mover esta palabra junto con los demás seudo-campos
  dup log% =  swap sword% =  or  \ ¿Es el tronco o la espada?
  ;
: log_already_sharpened$  ( -- a u )
  \ Devuulve una cadena con una variante de «Ya está afilado»
  s" Ya" s{
  s" lo afilaste antes"
  s" está afilado de antes"
  s" tiene una buena punta"
  s" quedó antes bien afilado"
  }s&
  ;
: no_need_to_do_it_again$  ( -- a u )
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
  }s& again$ s&
  ;
: ^no_need_to_do_it_again$  ( -- a u )
  \ Devuelve una variante de «No hace falta hacerlo otra vez».
  no_need_to_do_it_again$ ^uppercase
  ;
: log_already_sharpened_0$  ( -- a u )
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 0).
  log_already_sharpened$ ^uppercase period+
  ^no_need_to_do_it_again$ period+ s&
  ;
: log_already_sharpened_1$  ( -- a u )
  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 1).
  ^no_need_to_do_it_again$ period+ s&
  log_already_sharpened$ ^uppercase period+ s&
  ;
: log_already_sharpened
  \ Informa de que el tronco ya estaba afilado.
  ['] log_already_sharpened_0$
  ['] log_already_sharpened_1$
  2 choose execute  narrate
  ;
: sharpen_the_log
  \ Afila el tronco.
  \ Inacabado!!! Distinguir herramientas
  hacked_the_log? @
  if  log_already_sharpened
  else  hacked_the_log? on  well_done
  then
  ;
: sharpen_the_sword
  \ Afila la espada.
  \ Inacabado!!!
  ;
: (do_sharpen)  ( a -- )
  \ Afila un ente que puede ser afilado.
  case
    sword% of  sharpen_the_sword  endof
    log% of  sharpen_the_log  endof
  endcase
  ;
:action do_sharpen
  \ Acción de afilar.
  main_complement{required}
  main_complement{accessible}
  main_complement @ can_be_sharpened?
  if  main_complement @ (do_sharpen)
  else  nonsense
  then
  ;action

\ }}}---------------------------------------------
subsection( Movimiento)  \ {{{

: toward_that_direction  ( a -- a2 u2 )
  \ Devuelve «al/hacia la dirección indicada».
  \ a = Ente dirección
  dup >r  has_no_article?
  if  \ no debe llevar artículo
    s" hacia" r> full_name 
  else  \ debe llevar artículo
    toward_the(m)$ r> ^name
  then  s&
  ;
: impossible_move  ( a -- )
  \ El movimiento es imposible.
  \ a = Ente dirección
  \ Inacabado!!! Añadir una tercera variante «ir en esa dirección»; y otras específicas como «no es posible subir»
  ^is_impossible$ s" ir" s&  rot
  3 random 
  if  toward_that_direction
  else  drop that_way$
  then  s& period+ narrate
  ;
: do_go_if_possible  ( a -- )
  \ Comprueba si el movimiento es posible y lo efectúa.
  \ a = Ente supuestamente de tipo dirección
  [debug] [if]  s" Al entrar en DO_GO_IF_POSSIBLE" debug  [then]  \ Depuración!!!
  dup direction ?dup if  \ ¿El ente es una dirección?
    my_location + @ ?dup  \ ¿Tiene mi escenario salida en esa dirección?
    if  nip enter_location  else  impossible_move  then
  else  drop nonsense
  then
  [debug] [if]  s" Al salir de DO_GO_IF_POSSIBLE" debug  [then]  \ Depuración!!!
  ;
: simply_do_go
  \ Ir sin dirección específica.
  \ Inacabado!!!
  s" Ir sin rumbo...?" narrate
  ;
:action do_go
  \ Acción de ir.
  [debug] [if]  s" Al entrar en DO_GO" debug  [then]  \ Depuración!!!
  tool_complement{unnecessary} 
  main_complement @ ?dup
  if  do_go_if_possible
  else  simply_do_go
  then
  [debug] [if]  s" Al salir de DO_GO" debug  [then]  \ Depuración!!!
  ;action
:action do_go_north
  \ Acción de ir al Norte.
  tool_complement{unnecessary} 
  north% main_complement{this_only}
  north% do_go_if_possible
  ;action
:action do_go_south
  \ Acción de ir al Sur.
  [debug_catch] [if]  s" Al entrar en DO_GO_SOUTH" debug  [then]  \ Depuración!!!
  tool_complement{unnecessary} 
  south% main_complement{this_only}
  south% do_go_if_possible
  [debug_catch] [if]  s" Al salir de DO_GO_SOUTH" debug  [then]  \ Depuración!!!
  ;action
:action do_go_east
  \ Acción de ir al Este.
  tool_complement{unnecessary} 
  east% main_complement{this_only}
  east% do_go_if_possible
  ;action
:action do_go_west
  \ Acción de ir al Oeste.
  tool_complement{unnecessary} 
  west% main_complement{this_only}
  west% do_go_if_possible
  ;action
:action do_go_up
  \ Acción de ir hacia arriba.
  tool_complement{unnecessary} 
  up% main_complement{this_only}
  up% do_go_if_possible
  ;action
:action do_go_down
  \ Acción de ir hacia abajo.
  tool_complement{unnecessary} 
  down% main_complement{this_only}
  down% do_go_if_possible
  ;action
:action do_go_out
  \ Acción de ir hacia fuera.
  tool_complement{unnecessary} 
  out% main_complement{this_only}
  out% do_go_if_possible
  ;action
:action do_go_in
  \ Acción de ir hacia dentro.
  tool_complement{unnecessary} 
  in% main_complement{this_only}
  in% do_go_if_possible
  ;action
:action do_go_back
  \ Acción de ir hacia atrás.
  \ Pendiente!!!
  tool_complement{unnecessary} 
  main_complement{forbidden}
  s" [voy atrás, pero es broma]" narrate \ tmp!!!
  ;action
:action do_go_ahead
  \ Acción de ir hacia delante.
  \ Pendiente!!!
  tool_complement{unnecessary} 
  main_complement{forbidden}
  s" [voy alante, pero es broma]" narrate \ tmp!!!
  ;action

\ }}}---------------------------------------------
subsection( Partir [desambiguación])  \ {{{

:action do_go|do_break
  \ Acción de partir (desambiguar: romper o marchar).
  main_complement @ ?dup
  if  ( a )  \ Hay complemento principal
    is_direction?
    if  do_go  else  do_break  then 
  else
    tool_complement @
    if do_break  \ Solo con herramienta, suponemos que es «romper»
    else  simply_do_go  \ Sin complementos, suponemos que es «marchar»
    then
  then
  ;action

\ }}}---------------------------------------------
subsection( Nadar)  \ {{{

: in_a_different_place$  ( -- a u )
  \ Devuelve una variante de «en un lugar diferente».
  s" en un" s& place$
  s{ s" desconocido" s" nuevo" s" diferente" }s&
  s" en otra parte"
  s" en otro lugar"
  3 schoose
  ;
: you_emerge$  ( -- a u )
  \ Devuelve mensaje sobre la salida a la superficie.
  s{ s" Consigues" s" Logras" }s
  s{ s" emerger," s" salir a la superficie," }s&
  though$ s& in_a_different_place$ s&
  s" de la" s& cave$ s& s" ..." s&
  ;
: swiming$  ( -- a u )
  \ Devuelve mensaje sobre el buceo.
  s" Buceas" s{ s" pensando en" s" deseando"
  s" con la esperanza de" s" con la intención de" }s&
  s{ s" avanzar," s" huir," s" escapar,"  s" salir," }s&
  s" aunque" s&{ s" perdido." s" desorientado." }s&
  ;
: drop_the_cuirasse$  ( ff -- a u )
  \ Devuelve mensaje sobre deshacerse de la coraza dentro del agua.
  \ ff = ¿Inicio de frase?
  s{ s" te desprendes de ella" s" te deshaces de ella"
  s" la dejas caer" s" la sueltas" }s
  rot if  \ ¿Inicio de frase?
    s{ s" Rápidamente" s" Sin dilación"
    s" Sin dudarlo" s{ "" s" un momento" s" un instante" }s&
    }s 2swap s&
  then  period+
  ;
: you_leave_the_cuirasse$  ( -- a u )
  \ Devuelve mensaje sobre quitarse y soltar la coraza dentro del agua.
  cuirasse% is_worn_by_me?  \ ¿La llevamos puesta?
  if
    s{ s" Como puedes," s" No sin dificultad," }s
    s{ s" logras quitártela" s" te la quitas" }s&
    s" y" s& false drop_the_cuirasse$ s&
  else
    true drop_the_cuirasse$
  then
  ;
: (you_sink_0)$ ( -- a u )
  \ Devuelve la primera versión del mensaje sobre hundirse con la coraza.
  s{ s" Caes" s" Te hundes"
  s{ s" Empiezas" s" Comienzas" }s{ s" a hundirte" s" a caer" }s&
  }s s" sin remedio" s?& s" hacia" s&
  s{ s" el fondo" s" las profundidades" }s&
  s{ s" por el" s" debido al" s" arrastrado por" s" a causa del" }s&
  s" peso de tu coraza" s&
  ;
: (you_sink_1)$ ( -- a u )
  \ Devuelve la segunda versión del mensaje sobre hundirse con la coraza.
  s" El peso de tu coraza"
  s{ s" te arrastra" s" tira de ti" }s&
  s{ "" s" sin remedio" s" con fuerza" }s&
  s{
  s" hacia el fondo"
  s" hacia las profundidaes" 
  s" hacia abajo"
  }s&
  ;
: you_sink$ ( -- a u )
  \ Devuelve mensaje sobre hundirse con la coraza.
  2 random
  if  (you_sink_0)$
  else  (you_sink_1)$
  then  period+
  ;
: you_swim_with_cuirasse$  ( -- a u )
  \  Devuelve mensaje inicial sobre nadar con coraza.
  you_sink$ you_leave_the_cuirasse$ s&
  ;
: you_swim$  ( -- a u )
  \  Devuelve mensaje sobre nadar.
  cuirasse% is_hold? 
  if  you_swim_with_cuirasse$  cuirasse% vanish
  else  ""
  then  swiming$ s&
  ;
:action do_swim
  \ Acción de nadar.
  my_location location_11% = if
    clear_screen_for_location
    you_swim$ narrate narration_break
    you_emerge$ narrate narration_break
    location_12% enter_location  the_battle_ends
  else
    s" nadar" now|here|""$ s& is_nonsense
  then
  ;action

\ }}}---------------------------------------------
subsection( Escalar)  \ {{{

: you_try_climbing_the_fallen_away
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
  }s&  period+ narrate narration_break
  ;
: you_can_not_climb_the_fallen_away
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
  }s& s" ..." s+ narrate narration_break
  ;
: do_climb_the_fallen_away_first
  \ Imprime el mensaje
  \ previo al primer intento de escalar el derrumbe.
  you_try_climbing_the_fallen_away
  you_can_not_climb_the_fallen_away
  ;
: climbing_the_fallen_away_is_impossible
  \ Imprime el mensaje de error de que
  \ es imposible escalar el derrumbe.
  s{ s" pasar" s" escalar" s" subir por" }s
  s{
    s" el derrumbe" 
    s{ s" el muro" s" la pared" s" el montón" }s s" de" s& rocks$ s&
    s" las" rocks$ s&
  }s& is_impossible
  ;
: do_climb_the_fallen_away
  \ Escalar el derrumbe.
  climbed_the_fallen_away? @ 0=
  if  do_climb_the_fallen_away_first  then
  climbing_the_fallen_away_is_impossible
  climbed_the_fallen_away? on
  ;
: do_climb_this_here_if_possible  ( a -- )
  \ Escalar el ente indicado, que está presente, si es posible.
  \ Inacabado!!!
  ;
: do_climb_if_possible  ( a -- )
  \ Escalar el ente indicado si es posible.
  \ Inacabado!!!
  dup is_here? if  dup s" [escalar eso]" narrate
  else  drop s" [no está aquí eso para escalarlo]" narrate
  then
  ;
: nothing_to_climb
  \ Inacabado!!!
  s" [No hay nada que escalar]" narrate
  ;
: do_climb_something
  \ Escalar algo no especificado.
  \ Inacabado!!!
  location_09% am_i_there?  \ ¿Ante el derrumbe?
  if  do_climb_the_fallen_away exit
  then
  location_08% am_i_there?  \ ¿En el desfiladero?
  if  s" [Escalar en el desfiladero]" narrate exit
  then
  my_location is_indoor_location?
  if  s" [Escalar en un interior]" narrate exit 
  then
  nothing_to_climb  
  ;
:action do_climb
  \ Acción de escalar.
  \ Inacabado!!!
  main_complement @ ?dup
  if  do_climb_if_possible  else  do_climb_something  then
  ;action
 
\ }}}---------------------------------------------
subsection( Inventario)  \ {{{

: anything_with_you$  ( -- a u )
  \ Devuelve una variante de «nada contigo».
  s" nada" with_you$  ?dup if
    2 random if  2swap  then  s&
  else drop
  then
  ;
: you_are_carrying_nothing$  ( -- a u )
  \ Devuelve mensaje para sustituir a un inventario vacío.
  s" No" you_carry$ anything_with_you$ period+ s& s& 
  ;
: ^you_are_carrying$  ( -- a u )
  \ Devuelve mensaje para encabezar la lista de inventario,
  \ con la primera letra mayúscula.
  ^you_carry$ with_you$ s& 
  ;
: you_are_carrying$  ( -- a u )
  \ Devuelve mensaje para encabezar la lista de inventario.
  you_carry$ with_you$ s& 
  ;
: you_are_carrying_only$  ( -- a u )
  \ Devuelve mensaje para encabezar una lista de inventario de un solo elemento.
  2 random
  if  ^you_are_carrying$ only_$ s& 
  else  ^only_$ you_are_carrying$ s& 
  then
  ;
:action do_inventory
  \ Acción de hacer inventario.
  protagonist% content_list  \ Hace la lista en la cadena dinámica 'print_str'
  #listed @ case
    0 of  you_are_carrying_nothing$ 2swap s& endof
    1 of  you_are_carrying_only$ 2swap s& endof
    >r ^you_are_carrying$ 2swap s& r>
  endcase  narrate 
  ;action

\ }}}---------------------------------------------
subsection( Hacer)  \ {{{

:action do_make
  \ Acción de hacer (fabricar).
  main_complement @
  if    nonsense
  else  do_not_worry
  then
  ;action

:action do_do
  \ Acción de hacer (genérica).
  main_complement @ inventory% =
  if    do_inventory
  else  do_make
  then
  ;action

\ }}}---------------------------------------------
subsection( Hablar y presentarse)  \ {{{

\ ------------------------------------------------
\ Conversaciones con el líder de los refugiados

: a_man_takes_the_stone
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
  location_18% stone% is_there
  ;
: gets_angry$  ( -- a u )
  \ Devuelve una variante de «se enfada».
  s" se" s{ s" irrita" s" enfada" s" enoja" s" enfurece" }s& 
  ;
: the_leader_gets_angry$  ( -- a u )
  \ Devuelve una variante de «El líder se enfada».
  \ Yo no se usa!!! 
  s{ s" Al verte" s" Viéndote" s" Tras verte" }s
  s{ s" llegar" s" aparecer" s" venir" }s&
  again$ stone_forbidden? @ ?keep s&
  s" con la piedra," s&
  s" el" s& old_man$ s& gets_angry$ s& 
  ;
: the_leader_gets_angry
  \ Mensaje de que el líder se enfada.
  \ Ya no se usa!!! 
  the_leader_gets_angry$ period+ narrate
  ;
: warned_once$  ( -- a u )
  s{
  s" antes"
  s" en la ocasión anterior"
  s" en la otra ocasión" 
  s" en una ocasión" 
  s" la otra vez"
  s" la vez anterior"
  s" una vez"
  }s
  ;
: warned_twice$  ( -- a u )
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
  }s
  ;
: warned_several_times$  ( -- a u )
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
  }s
  ;
: warned_many_times$  ( -- a u )
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
  }s
  ;
: times_warned  ( u -- a1 u1 )
  { times }  \ Variable local
  true case
    times 0 = of  ""  endof
    times 1 = of  warned_once$  endof
    times 2 = of  warned_twice$  endof
    times 6 < of  warned_several_times$  endof
    warned_many_times$ rot
  endcase
  ;
: already_warned$  ( -- a u )
  \ Mensaje de que el líder ya te advirtió sobre un objeto.
  \ Pendiente!!! Elaborar más.
  s" ya" s?
  s{
    s" fuisteis" s{ s" avisado" s" advertido" }s& s" de ello" s?&
    s" se os" s{ s" avisó" s" advirtió" }s& s" de ello" s?&
    s" os lo" s{ s" hicimos saber" s" advertimos" }s&
    s" os lo" s{ s" hice saber" s" advertí" }s&
    s" se os" s{ s" hizo saber" s" dejó claro" }s&
  }s& 
  ;
: already_warned  ( u -- a1 u1 )
  \ Mensaje de que el líder ya te advirtió sobre un objeto,
  \ con indicación al azar del número de veces.
  times_warned already_warned$ rnd2swap s& period+ ^uppercase
  ;
: you_can_not_take_the_stone$  ( -- a u )
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
  2dup stone% fname!  \ Nuevo nombre para la piedra
  s& s& period+
  ;
: gesture_about_the_stone$  ( -- a u )
  \ Mensaje de que el líder hace un gesto sobre la piedra.
  s" y" s? s{ s" entonces" s" a continuación" s" seguidamente" }s& ^uppercase
  s" hace un" s&
  s" pequeño" s?& s" gesto" s& s" con la mano," s?&
  s" casi imperceptible" s?&
  s" ..." s+ 
  ;
: the_stone_must_be_in_its_place$  ( -- a u )
  \ El líder dice que la piedra debe ser devuelta.
  s" La piedra" s{ s" ha de" s" debe" s" tiene que" }s&
  s{ s" ser devuelta" s" devolverse" to_go_back$ }s&
  s{
    s" a su lugar" s" de encierro" s?&
    s" al lugar al que pertenece"
    s" al lugar del que nunca debió" s{ s" salir" s" ser sacada" s" ser arrancada" }s&
    s" al lugar que nunca debió" s{ s" abandonar" s" haber abandonado" }s&
  }s& 
  ;
: the_leader_warns_about_the_stone
  \ El líder habla acerca de la piedra.
  stone_forbidden? @ already_warned
  you_can_not_take_the_stone$
  the_stone_must_be_in_its_place$ rnd2swap s& s&
  period+ speak
  ;
: the_leader_points_to_the_north$  ( -- a u )
  \ El líder se enfada y apunta al Norte.
  \ Pendiente!!! Crear ente "brazo" aquí, o activarlo como sinómino del anciano.
  leader% ^full_name
  s{ s" alza" s" extiende" s" levanta" }s&
  s{ s" su" s" el" }s& s" brazo" s&
  s{ s" indicando" s" en dirección" s" señalando" }s&
  toward_the(m)$ s& s" Norte." s&
  ;
: the_leader_points_to_the_north
  \ El líder se enfada y apunta al Norte.
  the_leader_points_to_the_north$ narrate
  ;
: nobody_passes_with_arms$  ( -- a u )
  \ El líder dice que nadie pasa con armas.
  s{ s" Nadie" s" Ningún hombre" }s
  s{ s" con" s" llevando" s" portando" s" portador de"
  s" que porte" s" que lleve" }s&
  s{ s" armas" s" un arma" s" una espada" }s&
  with_him$ s&{ s" debe" s" puede" s" podrá" }s& 
  s" pasar." s&
  ;
: the_leader_warns_about_the_sword
  \ El líder habla acerca de la espada.
  the_leader_points_to_the_north
  sword_forbidden? @ already_warned
  nobody_passes_with_arms$ s& speak
  ;
: the_leader_points_to_the_east
  \ El líder apunta al Este.
  s" El" old_man$ s& comma+
  s{ s" confiado" s" calmado" s" sereno" s" tranquilo" }s& comma+
  s{ s" indica" s" señala" }s&
  s{ toward_the(m)$ s" en dirección al" }s& s" Este y" s&
  s{  s" te" s? s" dice" s&
      s" pronuncia las siguientes palabras"
  }s& colon+ narrate
  ;
: something_had_been_forbidden?  ( -- ff )
  \ Se le prohibió alguna vez al protagonista pasar con algo prohibido?
  sword_forbidden? @ stone_forbidden? @ or
  ;
: go_in_peace
  \ El líder dice que puedes ir en paz.
  s{ s" Ya que" s" Puesto que" s" Dado que" s" Pues" }s
  something_had_been_forbidden? if
    s{ s" esta vez" s" ahora" s" en esta ocasión" s" por fin" s" finalmente" }s&
  then
  s{ s" vienes" s" llegas" s" has venido" s" has llegado" }s&
  s" en paz, puedes" s&
  s{ s" ir" s" marchar" s" continuar" s" tu camino" s?& }s&
  s" en paz." s& speak
  ;
: the_refugees_let_you_go
  \ Los refugiados te dejan pasar.
  s" todos" s? s" los refugiados" s& ^uppercase
  s" se apartan y" s& s" te" s?&
  s{  s" permiten" s{ s" el paso" s" pasar" }s&
      s" dejan" s" libre" s" el" s{ s" paso" s" camino" }s& rnd2swap s& 
  }s& toward_the(m)$ s& s" Este." s& narrate
  ;
: the_leader_lets_you_go
  \ El jefe deja marchar al protagonista.
  location_28% location_29% e-->  \ Hacer que la salida al Este de 'location_28%' conduzca a 'location_29%'
  the_leader_points_to_the_east
  go_in_peace the_refugees_let_you_go  
  ;
: talked_to_the_leader
  \ Aumentar el contador de conversaciones con el jefe de los refugiados.
  leader% conversations++
  recent_talks_to_the_leader ?++
  ;
: we_are_refugees$  ( -- a u )
  \ Mensaje «Somos refugiados».
  s" todos" s? s" nosotros" s? rnd2swap s&
  s" somos refugiados de" s& ^uppercase
  s{ s" la gran" s" esta terrible" }s& s" guerra." s&
  s" refugio" location_28% mname!
  ;
: we_are_refugees
  \ Somos refugiados.
  we_are_refugees$ we_want_peace$ s& speak narration_break
  ;
: the_leader_trusts
  \ El líder te corta, confiado.
  s" El" old_man$ s& s" asiente" s&
  s{ s" confiado" s" con confianza" }s& s" y," s&
  s" con un suave gesto" s& s" de su mano" s?& comma+
  s" te interrumpe para" s&
  s{  s" explicar" s{ s" te" s" se" "" }s+
      s" presentarse" s" contarte" s" decir" s" te" s?+
  }s& colon+ narrate
  ;
: untrust$  ( -- a u )
  s{ s" desconfianza" s" nerviosismo" s" impaciencia" }s
  ;
: the_leader_does_not_trust
  \ El líder te corta, desconfiado.
  s" El" old_man$ s& s" asiente" s& s" con la cabeza" s?& comma+
  s{  s" desconfiado" s" nervioso" s" impaciente"
      s" mostrando" s" claros" s?& s" signos de" s& untrust$ s&
      s{ s" dando" s" con" }s s" claras" s?& s" muestras de" s& untrust$ s&
  }s& comma+ s" y te interrumpe:" s& narrate
  ;
: the_leader_introduces_himself
  \ El líder se presenta.
  do_you_hold_something_forbidden?
  if    the_leader_does_not_trust
  else  the_leader_trusts
  then  we_are_refugees
  ;
: first_conversation_with_the_leader
  \ Pendiente!!! Elaborar mejor el texto.
  my_name_is$ s" Ulfius y..." s& speak talked_to_the_leader
  the_leader_introduces_himself
  ;
: the_leader_forbids_the_stone
  \ El jefe te avisa de que no puedes pasar con la piedra y te la quita.
  the_leader_warns_about_the_stone
  stone_forbidden? ?++  \ Recordarlo
  gesture_about_the_stone$ narrate narration_break
  a_man_takes_the_stone
  ;
: the_leader_forbids_the_sword
  \ El jefe te avisa de que no puedes pasar con la espada.
  the_leader_warns_about_the_sword
  sword_forbidden? ?++  \ Recordarlo
  ;
: the_leader_checks_what_you_carry
  \ El jefe controla lo que llevas.
  \ Pendiente!!! Mejorar para que se pueda pasar si dejamos el objeto en el suelo o se lo damos.
  true case
    stone% is_accessible? of  the_leader_forbids_the_stone  endof
    sword% is_accessible? of  the_leader_forbids_the_sword  endof
    the_leader_lets_you_go
  endcase
  ;
: insisted_once$  ( -- a u )
  \ Inacabado!!!
  s{ s" antes" s" una vez" }s
  ;
: insisted_twice$  ( -- a u )
  \ Inacabado!!!
  s{ s" antes" s" dos veces" s" un par de veces" }s
  ;
: insisted_several_times$  ( -- a u )
  \ Inacabado!!!
  s{ s" las otras" s" más de dos" s" más de un par de" s" varias" }s
  s" veces" s&
  ;
: insisted_many_times$  ( -- a u )
  \ Inacabado!!!
  s{  s" demasiadas" s" incontables" s" innumerables"
      s" las otras" s" muchas" s" varias"
  }s  s" veces" s&
  ;
: times_insisted  ( u -- a1 u1 )
  \ Inacabado!!!
  { times }  \ Variable local
  true case
    times 0 = of  ""  endof  \ Innecesario!!!
    times 1 = of  insisted_once$  endof
    times 2 = of  insisted_twice$  endof
    times 6 < of  insisted_several_times$  endof
    insisted_many_times$ rot
  endcase
  ;
: please_don't_insist$  ( -- a u )
  \ Mensaje de que por favor no insistas.
  s{ s" os ruego que" s" os lo ruego," s" he de rogaros que" }s
  s" no insistáis" s&
  ;
: don't_insist$  ( -- a u )
  \ Inacabado!!!
  s" ya" s?
  s{
    s" habéis sido" s{ s" avisado" s" advertido" }s&
    s" os lo he" s" mos" s?+ s{ s" hecho saber" s" advertido" s" dejado claro" }s&
    s" se os ha" s{ s" hecho saber" s" advertido" s" dejado claro" }s&
  }s& 
  ;
: don't_insist
  \ Inacabado!!!
  times_insisted don't_insist$ rnd2swap s& period+ ^uppercase
  ;
: the_leader_ignores_you
  \ El líder te ignora.
  ;
: (talk_to_the_leader)
  \ Hablar con el jefe.
  leader% no_conversations?
  if  first_conversation_with_the_leader  then
  the_leader_checks_what_you_carry
  ;
: talk_to_the_leader
  \ Hablar con el jefe, si se puede.
  recent_talks_to_the_leader @
  if    the_leader_ignores_you  
  else  (talk_to_the_leader)
  then
  ;

\ ------------------------------------------------
\ Conversaciones con Ambrosio

: talked_to_ambrosio
  \ Aumentar el contador de conversaciones con Ambrosio.
  ambrosio% conversations++
  ;
: is_ambrosio's_name  ( a u -- )
  \ Le pone a ambrosio su nombre de pila.
  \ a u = Nombre.
  ambrosio% mname!
  ambrosio% has_no_article
  ambrosio% has_personal_name
  ;
: (conversation_0_with_ambrosio)
  \ Primera conversación con Ambrosio.
  s" Hola, buen hombre." speak
  s" Hola, Ulfius." 
  my_name_is$ s& s" Ambrosio" 2dup is_ambrosio's_name  
  period+ s& speak
  scene_break
  s" Por" s" primera" s" vez" rnd2swap s& s& s" en" s&
  s{ s" mucho" s" largo" }s& s" tiempo, te sientas"
  s" y" s& s" le" s?& s{ s" cuentas" s" narras" }s&
  s" a alguien todo lo que ha" s&{ s" pasado." s" ocurrido." }s&
  s" Y, tras tanto acontecido, lloras" s&
  s{ s" desconsoladamente." s" sin consuelo" }s&
  narrate scene_break
  s" Ambrosio te propone un trato, que aceptas:"
  s" por ayudarle a salir de la cueva," s&
  s" objetos, vitales para la empresa, te son entregados." s&
  narrate narration_break
  torch% is_hold  flint% is_hold
  s{ s" Bien," s" Venga," s" Vamos," }s
  s" Ambrosio," s&
  s{ s" emprendamos la marcha"
  s" pongámonos en" s{ s" marcha" s" camino" }s&
  }s& period+  speak
  location_46% ambrosio% is_there
  s" Te das la vuelta"
  s" para ver si Ambrosio te sigue," s&
  s" pero... ha desaparecido." s&
  narrate narration_break
  s" Piensas entonces en el hecho curioso"
  s" de que supiera tu nombre." s&
  narrate scene_break  talked_to_ambrosio
  ;
: conversation_0_with_ambrosio
  \ Primera conversación con Ambrosio, si se dan las condiciones.
  location_19% am_i_there?
  if  (conversation_0_with_ambrosio)  then
  ;
: (conversation_1_with_ambrosio)
  \ Segunda conversación con Ambrosio.
  s" La llave, Ambrosio, estaba ya en vuestro poder."
  s" Y es obvio que conocéis un camino más corto." s&
  speak
  s" Estoy atrapado en la cueva debido a magia de maligno poder."
  s" En cuanto al camino, vos debéis hacer el vuestro," s&
  s" verlo todo con vuestros ojos." s&
  speak
  s" Sacudes la cabeza." narrate
  s" No lo entiendo, la verdad." speak
  talked_to_ambrosio
  ;
: conversation_1_with_ambrosio
  \ Segunda conversación con Ambrosio, si se dan las condiciones.
  location_46% am_i_there?
  ambrosio_follows? 0=  and
  if  (conversation_1_with_ambrosio)  then
  ;
: (conversation_2_with_ambrosio)
  \ Tercera conversación con Ambrosio.
  s{ s" Por favor," s" Os lo ruego," }s
  s" Ulfius," s&
  s" cumplid vuestra" s{ s" promesa." s" palabra." }s&
  s" Tomad la llave" s&
  s{ "" s" en vuestra mano" s" en vuestras manos" s" con vos" }s&
  s" y abrid la puerta de la cueva." s&  speak
  key% is_hold
  \ aquí en SuperBASIC: do_takeable the_key \ pendiente!!!
  ambrosio_follows? on  talked_to_ambrosio  
  ;
: conversation_2_with_ambrosio
  \ Tercera conversación con Ambrosio, si se dan las condiciones.
  location_45% 1- location_47% 1+ my_location within 
  if  (conversation_2_with_ambrosio)  then
  ;
: (talk_to_ambrosio)
  \ Hablar con Ambrosio.
  \ Método nuevo en pruebas!!!:
  ambrosio% conversations case
    0 of  conversation_0_with_ambrosio  endof
    1 of  conversation_1_with_ambrosio  endof
    2 of  conversation_2_with_ambrosio  endof
    \ Aquí faltaría algo!!!
  endcase
  \ Métodos antiguos!!!:
  [false] [if]
  my_location case
    location_19% of  conversation_0_with_ambrosio  endof
    location_46% of  conversation_1_with_ambrosio  endof
  endcase
  [then]
  [false] [if]
  my_location case
    location_45% of  conversation_2_with_ambrosio  endof
    location_46% of  conversation_2_with_ambrosio  endof
    location_47% of  conversation_2_with_ambrosio  endof
  endcase
  [then]
  [false] [if]  \ Método alternativo poco legible, inacabado!!!
  location_45% 1- location_47% 1+ my_location within if
    conversation_2_with_ambrosio
  then
  [then]
  ;
: talk_to_ambrosio
  \ Hablar con Ambrosio, si se puede.
  \ Provisional!!! Esto debería comprobarse en 'do_speak' o 'do_speak_if_possible'.
  ambrosio% is_here?
  if  (talk_to_ambrosio)
  else  ambrosio% is_not_here
  then
  ;

\ ------------------------------------------------
\ Conversaciones sin éxito

: talk_to_something  ( a -- )
  \ Hablar con un ente que no es un personaje .
  \ Pendiente!!!
  2 random
  if  drop nonsense
  else  full_name s" hablar con" 2swap s& is_nonsense 
  then
  ;
: talk_to_yourself$  ( -- a u )
  \ Devuelve una variante de «hablar solo».
  s{ s" hablar" s{ s" solo" s" con uno mismo" }s&
  s" hablarse" s{ s" a sí" s" a uno" }s& s" mismo" s?&
  }s 
  ;
: talk_to_yourself
  \ Hablar solo.
  talk_to_yourself$ is_nonsense
  ;

\ ------------------------------------------------
\ Acciones

: do_speak_if_possible  ( a -- )
  \ Hablar con un ente si es posible.
  [debug] [if]  s" En DO_SPEAK_IF_POSSIBLE" debug  [then]  \ Depuración!!!
  case
    leader% of  talk_to_the_leader  endof
    ambrosio% of  talk_to_ambrosio  endof
    dup talk_to_something
  endcase
  ;
: (do_speak)  ( a | 0 -- )
  \ Hablar con alguien o solo.
  ?dup if  do_speak_if_possible  else  talk_to_yourself  then
  ;
: (you_speak_to)  ( a -- )
  s" Hablas con" rot full_name s& colon+ narrate
  ;
: you_speak_to  ( a | 0 -- )
  ?dup if  (you_speak_to)  then
  ;
:action do_speak
  \ Acción de hablar.
  [debug] [if]  s" En DO_SPEAK" debug  [then]  \ Depuración!!!
  main_complement @ ?dup 0=  \ Si no hay complemento...
  if    whom dup you_speak_to  \ ...buscar y mostrar el más probable.
  then  (do_speak)
  ;action
:action do_introduce_yourself
  \ Acción de presentarse a alguien.
  main_complement @ ?dup 0=  \ Si no hay complemento...
  if  unknown_whom  \ ...buscar el (desconocido) más probable.
  then  (do_speak)
  ;action

\ }}}---------------------------------------------
subsection( Guardar el juego)  \ {{{

(

Para guardar el estado de la partida usaremos una solución
muy sencilla: ficheros de texto que reproduzcan el código
Forth necesario para restaurarlas. Esto permitirá
restaurar una partida con solo interpretar ese fichero
como cualquier otro código fuente.

)

0 [if] \ Inacabado!!! Pendiente!!!
: n>s  ( u -- a1 u1 )
  \ Convierte un número en una cadena (con dos dígitos como mínimo).
  s>d <# # #s #> >sb
  ;
: n>s+  ( u a1 u1 -- a2 u2 )
  \ Añade un número a una cadena tras convertirlo en cadena.
  \ u = Número
  \ a1 u1 = Cadena original
  rot n>s s+
  ;
: yyyymmddhhmmss$  ( -- a u )
  \ Devuelve la fecha y hora actuales como una cadena en formato «aaaammddhhmmss».
  time&date n>s n>s+ n>s+ n>s+ n>s+ n>s+
  ;
: file_name$  ( -- a u )
  \ Devuelve el nombre con que se grabará el juego.
  s" ayc_" yyyymmddhhmmss$ s+
  s" .exe" windows? and s+  \ Añadir sufijo si estamos en Windows
  ;
defer reenter
svariable filename
: (do_save_the_game)
  \ Graba el juego.
  \ Inacabado!!! No está decidido el sistema que se usará para salvar las partidas
  \ 2011-12-01 No funciona bien. Muestra mensajes de gcc con parámetros sacados de textos del programa!
\ false to spf-init?  \ Desactivar la inicialización del sistema
\ true to console?  \ Activar el modo de consola (no está claro en el manual)
\ false to gui?  \ Desactivar el modo gráfico (no está claro en el manual)
  ['] reenter to <main>  \ Actualizar la palabra que se ejecutará al arrancar
\ file_name$ save  new_page
  file_name$ filename place filename count save  
  ;
:action do_save_the_game
  \ Acción de salvar el juego.
  main_complement{forbidden}
  (do_save_the_game)
  ;action
[then]

svariable game_file_name  \ Nombre del fichero en que se graba la partida
variable game_file_id  \ Identificador del fichero en que se graba la partida
: game_file_name$  ( -- a u )
  \ Devuelve el nombre del fichero en que se graba la partida.
  game_file_name count
  ;
: close_game_file
  \ Cierra el fichero en que se grabó la partida.
  game_file_id @ close-file abort" Close file error."  \ mensaje tmp!!!
  ;
: create_game_file  ( a u -- )
  \ Crea un fichero para grabar una partida
  \ (sobreescribiendo otro que tuviera el mismo nombre).
  \ a u = Nombre del fichero
  r/w create-file abort" Create file error."  \ mensaje tmp!!!
  game_file_id !
  ;
: read_game_file  ( a u -- )
  \ Lee el fichero de configuración.
  \ a u = Nombre del fichero
  \ Pendiente!!! Comprobar la existencia del fichero y atrapar errores al leerlo.
  only restore_vocabulary
  included  restore_vocabularies
  ;
: >file/  ( a u -- )
  \ Escribe una línea en el fichero de la partida.
  game_file_id @ write-line abort" Write file error"  \ mensaje tmp!!!
  ;
: cr>file
  \ Escribe un final de línea en el fichero de la partida.
  s" " >file/
  ;
: >file  ( a u -- )
  \ Escribe una cadena en el fichero de la partida.
  space+
  game_file_id @ write-file abort" Write file error"  \ mensaje tmp!!!
  ;
also restore_vocabulary  definitions
' \ alias \
immediate
' true alias true
' false alias false
' s" alias s"
: load_entity  ( x0 ... xn u -- )
  \ Restaura los datos de un ente.
  \ x0 ... xn = Datos del ente, en orden inverso a como los crea la palabra 'save_entity'.
  \ u = Número ordinal del ente.
  #>entity >r
  \ cr .s  \ Depuración!!!
  r@ ~direction !
  #>entity r@ ~in_exit !
  #>entity r@ ~out_exit !
  #>entity r@ ~down_exit !
  #>entity r@ ~up_exit !
  #>entity r@ ~west_exit !
  #>entity r@ ~east_exit !
  #>entity r@ ~south_exit !
  #>entity r@ ~north_exit !
  r@ ~familiar !
  r@ ~visits !
  #>entity r@ ~previous_location !
  #>entity r@ ~location !
  r@ ~owner !
  r@ ~flags_0 !
  r@ ~times_open !
  r@ ~conversations !
  \ 2dup cr type .s  \ depuración!!!
  r> name!
  ;
: load_plot  ( x0 ... xn -- )
  \ Restaura las variables de la trama.
  \ Debe hacerse en orden alfabético inverso.
  recent_talks_to_the_leader !
  sword_forbidden? !
  stone_forbidden? !
  hacked_the_log? !
  climbed_the_fallen_away? !
  battle# !
  ambrosio_follows? !
  ;
restore_vocabularies
: string>file  ( a u -- )
  \ Escribe una cadena en el fichero de la partida.
  bs| s" | 2swap s+ bs| "| s+ >file
  ;
: f>string  ( ff -- a u )
  \ Convierte un indicador binario en su nombre de constante.
  if  s" true"  else  s" false"  then
  ;
: f>file  ( ff -- )
  \ Escribe un indicador binario en el fichero de la partida.
  f>string >file
  ;
: n>string  ( n -- a u )
  \ Convierte un número con signo en una cadena.
  s>d swap over dabs
  <# #s rot sign #> >sb
  ;
: u>string ( u -- a u )
  \ Convierte un número sin signo en una cadena.
  s>d <# #s #> >sb
  ;
: 00>s  ( u -- a1 u1 )
  \ Convierte un número sin signo en una cadena (de dos dígitos como mínimo).
  s>d <# # #s #> >sb
  ;
: 00>s+  ( u a1 u1 -- a2 u2 )
  \ Añade a una cadena un número tras convertirlo en cadena.
  rot 00>s s+
  ;
: yyyy-mm-dd_hh:mm:ss$  ( -- a u )
  \ Devuelve la fecha y hora actuales como una cadena en formato «aaaa-mm-dd_hh:mm:ss».
  time&date 00>s hyphen+ 00>s+ hyphen+ 00>s+ space+
  00>s+ colon+ 00>s+ colon+ 00>s+
  ;
: n>file  ( n -- )
  \ Escribe un número con signo en el fichero de la partida.
  n>string >file
  ;
: entity>file  ( a -- )
  \ Escribe la referencia a un ente en el fichero de la partida.
  \ a = Ente (dirección de su ficha)
  \ Esta palabra es necesaria porque no es posible guardar y restaurar
  \ las direcciones de ficha de los entes, pues variarán con cada
  \ sesión de juego. Hay que guardar los números ordinales de las
  \ fichas y con ellos calcular sus direcciones durante la restauración.
  entity># n>file
  ;
: save_entity  ( u -- )
  \ Escribe los datos de un ente en el fichero de la partida.
  \ u = Número ordinal del ente.
  dup #>entity >r
  r@ name string>file
  r@ conversations n>file
  r@ times_open n>file
  r@ flags_0 n>file
  r@ owner n>file
  r@ location entity>file
  r@ previous_location entity>file
  r@ visits n>file
  r@ familiar n>file
  r@ north_exit entity>file
  r@ south_exit entity>file
  r@ east_exit entity>file
  r@ west_exit entity>file
  r@ up_exit entity>file
  r@ down_exit entity>file
  r@ out_exit entity>file
  r@ in_exit entity>file
  r> direction n>file
  n>file  \ Número ordinal del ente
  s" load_entity" >file/  \ Palabra que hará la restauración del ente
  ;
: rule>file
  \ Escribe una línea de separación en el fichero de la partida.
  s" \ ----------------------------------------------------" >file/
  ;
: section>file  ( a u -- )
  \ Escribe el título de una sección en el fichero de la partida.
  cr>file rule>file s" \ " >file >file/ rule>file cr>file
  ;
: save_entities
  \ Escribe los datos de los entes en el fichero de la partida.
  s" Entes" section>file
  #entities 0 do  i save_entity  loop
  ;
: save_config
  \ Escribe los valores de configuración en el fichero de la partida.
  \ Pendiente!!!
  s" Configuración" section>file
  ;
: save_plot
  \ Escribe las variables de la trama en el fichero de la partida.
  \ Debe hacerse en orden alfabético. 
  s" Trama" section>file
  ambrosio_follows? @ f>file
  battle# @ n>file
  climbed_the_fallen_away? @ f>file
  hacked_the_log? @ f>file
  stone_forbidden? @ f>file
  sword_forbidden? @ f>file
  recent_talks_to_the_leader @ n>file
  s" load_plot" >file/  \ Palabra que hará la restauración de la trama 
  ;
: file_header
  \ Escribe la cabecera del fichero de la partida.
  s" \ Datos de restauración de una partida de «Asalto y castigo»" >file/
  s" \ (http://pragramandala.net/es.programa.asalto_y_castigo.forth)" >file/
  s" \ Fichero creado en" yyyy-mm-dd_hh:mm:ss$ s& >file/
  ;
: write_game_file
  \ Escribe el contenido del fichero de la partida.
  file_header save_entities save_config save_plot
  ;
: fs+  ( a u -- a' u' )
  \ Añade la extensión .fs a un nombre de fichero.
  s" .fs" s+
  ;
: (do_save_the_game)  ( a u -- )
  \ Salva la partida.
  fs+ create_game_file write_game_file close_game_file
  ;
:action do_save_the_game  ( a u -- )
  \ Acción de salvar la partida.
  \ main_complement{forbidden}
  (do_save_the_game)
  ;action
: continue_the_loaded_game
  \ Continúa el juego en el punto que se acaba de restaurar.
  scene_break new_page
  my_location describe_location 
  ;
:action do_load_the_game  ( a u -- )
  \ Acción de salvar la partida.
  \ Pendiente!!! No funciona bien
  \ main_complement{forbidden}
  only restore_vocabulary
  [debug_filing] ?halto" in do_load_the_game before save-input"
  \ included  \ !!! el sistema estalla
  \ 2drop  \ !!! sin error
  \ cr type  \ !!! sin error  
  2>r save-input 2r>
  [debug_filing] ?halto" in do_load_the_game before fs+"
  fs+
  [debug_filing] ?halto" in do_load_the_game before included"
[false] [if]  \ Depuración!!!
  read_game_file
[else]
  ['] read_game_file
  [debug_filing] ?halto" in do_load_the_game before catch"
  catch  
  [debug_filing] ?halto" in do_load_the_game after catch"
  restore_vocabularies
  [debug_filing] ?halto" in do_load_the_game before if"
  ?dup if
    ( a u u2 ) nip nip
    case  \ tmp!!!
      2 of  s" Fichero no encontrado." narrate  endof
      s" Error al intentar leer el fichero." narrate
    endcase
    [debug_filing] ?halto" in do_load_the_game after endcase"
  then
[then]
  [debug_filing] ?halto" in do_load_the_game after then"
  restore-input
  if
    \ tmp!!!
    s" Error al intentar restaurar la entrada tras leer el fichero." narrate
  then
  [debug_filing] ?halto" in do_load_the_game at the end" 
  continue_the_loaded_game
  ;action

\ }}}
\ }}} ##########################################################
section( Intérprete de comandos)  \ {{{

(

Gracias al uso del propio intérprete de Forth como
intérprete de comandos del juego, más de la mitad del
trabajo ya está hecha por anticipado. Para ello
basta crear las palabras del vocabulario del juego como
palabras propias de Forth y hacer que Forth interprete
directamente la entrada del jugador. Creando las palabras
en un vocabulario de Forth específico para ellas,
y haciendo que sea el único vocabulario activo en
el momento de la interpretación, solo las palabras del
juego serán reconocidas, no las del programa ni las del
sistema Forth.

Sin embargo hay una consideración importante: Al pasarle
directamente al intérprete de Forth el texto del comando
escrito por el jugador, Forth ejecutará las palabras que
reconozca [haremos que las no reconocidas las ignore] en el
orden en que estén escritas en la frase.  Esto quiere decir
que, al contrario de lo que ocurre con otros métodos, no
podremos tener una visión global del comando del jugador: ni
de cuántas palabras consta ni, en principio, qué viene a
continuación de la palabra que está siendo interpretada en
cada momento.

Una solución sería que cada palabra del jugador guardara un
identificador unívoco en la pila o en una tabla, y
posteriormente interpretáramos el resultado de una forma
convencional.

Sin embargo, hemos optado por dejar a Forth hacer su trabajo
hasta el final, pues nos parece más sencillo y eficaz
[también es más propio del espíritu de Forth usar su
intérprete como intérprete de la aplicación en lugar de
programar un intérprete adicional específico].  Las palabras
reconocidas en el comando del jugador se ejecutarán pues en
el orden en que fueron escritas. Cada una actualizará el
elemento del comando que represente, verbo o complemento,
tras comprobar si ya ha habido una palabra previa que
realice la misma función y en su caso deteniendo el proceso
con un error.

)

\ Constantes para los identificadores de (seudo)preposiciones:
1  \ Valor del primer identificador
enum «con»_preposition
false [if]  \ Inacabado!!!
enum «a»_preposition
enum «contra»_preposition
enum «de»_preposition
enum «en»_preposition
enum «hacia»_preposition
enum «para»_preposition
enum «por»_preposition
[then]
( u1 )
  \ Número de (seudo)preposiciones +1
1- dup constant prepositions#  \ Número de (seudo)preposiciones
cells dup constant /prepositions  \ Octetos necesarios para guardarlas
( u2 )

\ Tabla de complementos (seudo)preposicionales:
create prepositional_complements  cells allot

(

Las [seudo]preposiciones permitidas en el juego pueden
tener usos diferentes, y algunos de ellos dependen del
ente al que se refieran, por lo que su análisis hay que
hacerlo en varios niveles.

Decimos «[seudo]preposiciones» porque algunos de los
términos usados como preposiciones no lo son [como por
ejemplo «usando», que es una forma verbal] pero se usan como
si lo fueran.

Los identificadores creados arriba se refieren a
[seudo]preposiciones del vocabulario de juego [por ejemplo,
«a», «con»...] o a sus sinónimos, no a sus posibles usos
finales como complementos [por ejemplo, destino de
movimiento, objeto indirecto, herramienta, compañía...]. Por
ejemplo, el identificador '«a»_preposition' se usa para
indicar [en la tabla] que se ha encontrado la preposición
«a» [o su sinónimo «al»], pero el significado efectivo [por
ejemplo, indicar una dirección o un objeto indirecto o un
objeto directo de persona, en este caso] se calculará en una
etapa posterior.

Cada elemento de la tabla de complementos
[seudo]preposicionales representa una [seudo]preposición
[incluidos evidentemente sus sinónimos]; será apuntado pues
por un identificador de [seudo]preposición y contendrá el
identificador del ente que haya sido usado en el comando con
dicha [seudo]preposición, o bien cero si la
[seudo]preposición no ha sido utilizada hasta el momento.

)

: erase_prepositional_complements
  \ Borra la tabla de complementos (seudo)preposicionales.
  prepositional_complements /prepositions erase
  ;
: prepositional_complement  ( u -- a )
  \ Devuelve la dirección de un elemento de la tabla
  \ de complementos (seudo)preposicionales.
  \ u = Identificador de la preposición
  \ a = Dirección en la tabla de complementos (seudo)preposicionales
  1- prepositional_complements +
  ;
: current_prepositional_complement  ( -- a )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente a la (seudo)preposición abierta.
  \ a = Dirección en la tabla de complementos (seudo)preposicionales
  current_preposition @ prepositional_complement
  ;
: (tool_complement)  ( -- a )
  \ Devuelve la dirección del elemento de la tabla
  \ de complementos (seudo)preposicionales
  \ correspondiente al complemento instrumental
  \ (complemento que puede ser cero si no existe).
  \ Sistema provisional!!!
  «con»_preposition prepositional_complement
  ;
' (tool_complement) is tool_complement
: prepositions_off
  \ Inicializa las preposiciones.
  erase_prepositional_complements
  current_preposition off
  ;
: complements_off
  \ Inicializa los complementos.
  main_complement off
  secondary_complement off
  [false] [if]  \ Descartado!!!
  tool_complement off
  to_complement off
  into_complement off
  from_complement off
  [then]
  prepositions_off
  ;
: init_parsing
  \ Preparativos previos a cada análisis.
  action off
  complements_off
  ;

: (execute_action)  ( xt -- )
  \ Ejecuta la acción del comando.
  [debug_catch] [debug_parsing] [or] ?halto" En (EXECUTE_ACTION) antes de CATCH"  \ Depuración!!!
  catch  \ Ejecutar la acción a través de CATCH para poder regresar directamente con THROW en caso de error
  [debug_catch] [debug_parsing] [or] ?halto" En (EXECUTE_ACTION) después de CATCH"  \ Depuración!!!
  ?wrong
  [debug_catch] [debug_parsing] [or] ?halto" En (EXECUTE_ACTION) después de ?WRONG"  \ Depuración!!!
  ;
: execute_action
  \ Ejecuta la acción del comando, si existe.
  [debug_catch] [debug_parsing] [or] ?halto" En EXECUTE_ACTION"  \ Depuración!!!
  action @ ?dup
  [debug_catch] [debug_parsing] [or] ?halto" En EXECUTE_ACTION tras ACTION @ ?DUP"  \ Depuración!!!
  if  (execute_action)  else  no_verb_error# ?wrong  then
  [debug_catch] [debug_parsing] [or] ?halto" Al final de EXECUTE_ACTION"  \ Depuración!!!
  ;

: (evaluate_command)
  \ Analiza la fuente actual, ejecutando las palabras reconocidas que contenga.
  begin   parse-name ?dup
  while   find-name ?dup if  name>int execute  then
  repeat  drop
  ;
: evaluate_command  ( a u -- )
  \ Analiza el comando, ejecutando las palabras reconocidas que contenga.
  ['] (evaluate_command) execute-parsing
  ;

: a_preposition_is_open?  ( -- ff )
  \ ¿Hay un complemento (seudo)preposicional abierto?
  current_preposition @ 0<>
  ;
: no_parsing_error_left?  ( -- ff )
  \ ¿No quedó algún error pendiente tras el análisis?
  \ Comprueba si quedó un complemento (seudo)preposicional
  \ incompleto, algo que no puede detectarse en el análisis
  \ principal.
  \ ff = ¿No quedó error pendiente tras el análisis?
  \      (true=ningún error pendiente; false=algún error pendiente)
  a_preposition_is_open? dup
  unresolved_preposition_error# and ?wrong  0=
  ;
: valid_parsing?  ( a u -- ff )
  \ Evalúa un comando con el vocabulario del juego.
  \ a u = Comando
  \ ff = ¿El comando se analizó sin error?
  -punctuation 
  [debug_parsing] ?halto" Entrando en VALID_PARSING?"  \ Depuración!!!
  \ Dejar solo el diccionario PLAYER_VOCABULARY activo
  only player_vocabulary
  \ [debug_catch] [if]  s" En VALID_PARSING? antes de preparar CATCH" debug  [then]  \ Depuración!!!
  [debug_parsing] ?halto" en valid_parsing? antes de preparar CATCH"  \ Depuración!!!
  ['] evaluate_command catch
  [debug_parsing] ?halto" en valid_parsing? después de CATCH"  \ Depuración!!!
  [debug_parsing] ?halto" en valid_parsing? antes de NIP NIP"  \ Depuración!!!
  dup if  nip nip  then  \ Arreglar la pila, pues CATCH hace que apunte a su posición previa
  [debug_parsing] ?halto" en valid_parsing? tras NIP NIP"
  dup ?wrong 0=
  [debug_parsing] ?halto" en valid_parsing? tras ?WRONG"
  restore_vocabularies
  no_parsing_error_left? and
  [debug_parsing] ?halto" Saliendo de VALID_PARSING?"  \ Depuración!!!
  ;

: >but_one!  ( a -- )
  \ Copia un complemento de la zona «últimos» a la «penúltimos»
  \ de la tabla 'last_complement'.
  \ a = Dirección en la zona «últimos» de la tabla 'last_complement'.
  dup @ swap >but_one !
  ;
: shift_last_complement  ( a -- )
  \ Copia el último complemento al lugar del penúltimo.
  \ a = Ente que fue encontrado como último complemento.
  >last_complement >but_one!  \ El último del mismo género y número
  last_complement >but_one!  \ El último absoluto
  ;
: new_last_complement  ( a -- )
  \ Guarda un nuevo complemento como el último complemento hallado.
  dup shift_last_complement  \ Copiar último a penúltimo
  dup last_complement !  \ Guardarlo como último absoluto
  dup >last_complement !  \ Guardarlo como último de su género y número
  ;
: save_command_elements
  \ No se usa!!!
  action @ last_action !
  \ Pendiente!!! Falta guardar los complementos
  ;

: obbey  ( a u -- )
  \ Evalúa un comando con el vocabulario del juego.
  [debug_parsing] ?halto" Al entrar en OBBEY"  \ Depuración!!!
  dup if
    init_parsing valid_parsing?
    if  execute_action  then
  else  2drop
  then
  [debug_parsing] ?halto" Al final de OBBEY"  \ Depuración!!!
  ; 

: second?  ( x1 x2 -- x1 ff )
  \ ¿La acción o el complemento son los segundos que se encuentran?
  \ Los dos valores representan una acción (xt) o un ente (a).
  \ x1 = Acción o complemento recién encontrado
  \ x2 = Acción o complemento anterior, o cero
  [debug_parsing] ?halto" second? 1"
  2dup different?  \ ¿Hay ya otro anterior y es diferente?
  [debug_parsing] ?halto" second? 2"
  ;
: action!  ( a -- )
  \ Comprueba y almacena la acción.
  \ a = Dirección de ejecución de la palabra de la acción
  [debug_parsing] ?halto" action! 1"
  action @ second?  \ ¿Había ya una acción?
  [debug_parsing] ?halto" action! 2"
  too_many_actions_error# and
  [debug_parsing] ?halto" action! antes de throw"
  throw  \ Sí, error
  [debug_parsing] ?halto" action! 2a"
  action !  \ No, guardarla
  [debug_parsing] ?halto" action! 3"
  ;
: preposition!  ( u -- )
  \ Almacena una (seudo)preposición recién hallada en la frase.
  \ u = Identificador de la preposición
  a_preposition_is_open?
  unresolved_preposition_error# and throw  \ Si es así, error
  current_preposition !
  ;
: prepositional_complement!  ( a -- )
  \ Almacena un complemento (seudo)preposicional.
  \ a = Identificador de ente 
  [debug_parsing] ?halto" prepositional_complement! 1"
  current_prepositional_complement @ second?  \ ¿Se había usado ya esta preposición con otro complemento?
  repeated_preposition_error# and throw  \ Si es así, error
  dup new_last_complement
  current_prepositional_complement !
  current_preposition off  \ Cerrar la preposición
  [debug_parsing] ?halto" prepositional_complement! 2"
  ;
: secondary_complement!  ( a -- )
  \ Almacena el complemento secundario. 
  \ a = Identificador de ente 
  secondary_complement @ second?  \ ¿Había ya un complemento secundario? 
  too_many_complements_error# and throw  \ Si es así, error
  secondary_complement !
  ;
: main_complement!  ( a -- )
  \ Almacena el complemento principal.
  \ a = Identificador de ente 
  [debug_parsing] ?halto" En (COMPLEMENT!)"  \ Depuración!!!
  main_complement @ second?  \ ¿Había ya un complemento principal?
  too_many_complements_error# and throw  \ Si es así, error
  dup new_last_complement
  main_complement !
  ;
: non_prepositional_complement!  ( a -- )
  \ Almacena un complemento principal o secundario. 
  \ a = Identificador de ente 
  \ Pendiente!!! Esta palabra sobrará cuando las (seudo)preposiciones estén implementadas completamente.
  main_complement @
  if  secondary_complement!
  else  main_complement!
  then
  ;
: (complement!)  ( a -- )
  \ Comprueba y almacena un complemento.
  \ a = Identificador de ente
  [debug_parsing] ?halto" En (COMPLEMENT!)"  \ Depuración!!!
  current_preposition @  \ ¿Hay una (seudo)preposición abierta?
  if  prepositional_complement!  \ Sí: complemento (seudo)preposicional
  else  non_prepositional_complement!  \ No: complemento principal o secundario
  then
  ;
: complement!  ( a | 0 -- )
  \ Comprueba y almacena un complemento.
  \ a = Identificador de ente,
  \     o cero si se trata de un pronombre sin referente.
  [debug_parsing] ?halto" En COMPLEMENT!"  \ Depuración!!!
  ?dup if  (complement!)  then
  ;
: action|complement!  ( a1 a2 -- )
  \ Comprueba y almacena un complemento o una acción,
  \ ambos posibles significados de la misma palabra.
  \ a1 = Dirección de ejecución de la palabra de la acción
  \ a2 = Identificador de ente
  action @  \ ¿Hay ya una acción reconocida...
  current_preposition @ or  \ ...o bien una (seudo)preposición abierta?
  if    nip complement!  \ Sí: lo tomamos como complemento
  else  drop action!  \ No: lo tomamos como acción
  then
  ;

\ }}} ##########################################################
section( Fichero de configuración)  \ {{{

(

El juego tiene un fichero de configuración en que el jugador
puede indicar sus preferencias. Este fichero es código en
Forth y se interpreta directamente, pero en él solo serán
reconocidas las palabras creadas expresamente para la
configuración, así como las palabras habituales para hacer
comentarios de bloques o líneas en Forth. Cualquier otra
palabra dará error.

El fichero de configuración se lee al inicio de cada
partida.

)

: config_file$  ( -- a u )
  \ Fichero de configuración.
  s" ~/forth/ayc/ayc.ini"   \ Provisional!!! Debería encontrarlo en el directorio del programa, ¿pero cómo?
  ;

svariable command_prompt
variable space_after_command_prompt?  \ ¿Separar el presto de comandos con un espacio posterior?
variable cr_after_command_prompt?  \ ¿Hacer un salto de línea tras el presto de comando?

also config_vocabulary  definitions

\ Las palabras cuyas definiciones siguen a continuación
\ se crearán en el vocabulario 'config_vocabulary' y
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
: varón
  \ Indica que el jugador es un varón.
  woman_player? off
  ;
' varón alias hombre
' varón alias masculino
: mujer
  \ Indica que el jugador es una mujer.
  woman_player? on
  ;
' mujer alias femenino
: comillas  ( ff -- )
  \ Indica si se usan las comillas castellanas en las citas.
  castilian_quotes? !
  ;
: espacios_de_indentación  ( u -- )
  \ Fija la indentación de los párrafos.
  max_indentation min /indentation !
  ;
: indentar_primera_línea_de_pantalla  ( ff -- )
  \ Indica si se indentará también la línea superior de la pantalla, si un párrafo empieza en ella.
  indent_first_line_too? !
  ;
: indentar_prestos_de_pausa  ( ff -- )
  \ Indica si se indentarán los prestos.
  indent_pause_prompts? !
  ;
: borrar_pantalla_para_escenarios  ( ff -- )
  \ Indica si se borra la pantalla al entrar en un escenario o describirlo.
  location_page? !
  ;
: borrar_pantalla_para_escenas  ( ff -- )
  \ Indica si se borra la pantalla tras la pausa de fin de escena.
  scene_page? !
  ;
: separar_párrafos  ( ff -- )
  \ Indica si se separan los párrafos con un línea en blanco.
  cr? !
  ;
: segundos_en_pausas_de_narración  ( u -- )
  \ Indica los segundos de las pausas cortas (o, si es valor es cero, que hay que pulsar una tecla).
  narration_break_seconds ! 
  ;
: segundos_en_pausas_de_final_de_escena  ( u -- )
  \ Indica los segundos de las pausas de final de esecena (o, si es valor es cero, que hay que pulsar una tecla).
  scene_break_seconds !
  ;
' black alias negro
' blue alias azul
' light_blue alias azul_claro
' brown alias marrón
' cyan alias cian
' light_cyan alias cian_claro
' green alias verde
' light_green alias verde_claro
' gray alias gris
' dark_gray alias gris_oscuro
' magenta alias magenta
' light_magenta alias magenta_claro
' red alias rojo
' light_red alias rojo_claro
' white alias blanco
' yellow alias amarillo

: papel_de_fondo  ( u -- )
  [defined] background_paper
  [if]  background_paper !  [else]  drop  [then]
  ;
: tinta_de_créditos  ( u -- )  about_ink !  ;
: papel_de_créditos  ( u -- )  about_paper !  ;
: tinta_de_presto_de_comandos  ( u -- )  command_prompt_ink !  ;
: papel_de_presto_de_comandos  ( u -- )  command_prompt_paper !  ;
: tinta_de_depuración  ( u -- )  debug_ink !  ;
: papel_de_depuración  ( u -- )  debug_paper !  ;
: tinta_de_descripción  ( u -- )  description_ink !  ;
: papel_de_descripción  ( u -- )  description_paper !  ;
: tinta_de_error  ( u -- )  error_ink !  ;
: papel_de_error  ( u -- )  error_paper !  ;
: tinta_de_entrada  ( u -- )  input_ink !  ;
: papel_de_entrada  ( u -- )  input_paper !  ;
: tinta_de_descripción_de_escenario  ( u -- )  location_description_ink !  ;
: papel_de_descripción_de_escenario  ( u -- )  location_description_paper !  ;
: tinta_de_nombre_de_escenario  ( u -- )  location_name_ink !  ;
: papel_de_nombre_de_escenario  ( u -- )  location_name_paper !  ;
: tinta_de_narración  ( u -- )  narration_ink !  ;
: papel_de_narración  ( u -- )  narration_paper !  ;
: tinta_de_presto_de_pantalla_llena  ( u -- )  scroll_prompt_ink !  ;
: papel_de_presto_de_pantalla_llena  ( u -- )  scroll_prompt_paper !  ;
: tinta_de_pregunta  ( u -- )  question_ink !  ;
: papel_de_pregunta  ( u -- )  question_paper !  ;
: tinta_de_presto_de_escena  ( u -- )  scene_prompt_ink !  ;
: papel_de_presto_de_escena  ( u -- )  scene_prompt_paper !  ;
: tinta_de_diálogos  ( u -- )  speech_ink !  ;
: papel_de_diálogos  ( u -- )  speech_paper !  ;

\ Prestos
' s" alias s"
: presto_de_pantalla_llena  ( a u -- )  scroll_prompt place  ;
: presto_de_narración  ( a u -- )  narration_prompt place  ;
: presto_de_fin_de_escena  ( a u -- )  scene_prompt place  ;
: presto_de_comando  ( a u -- )  command_prompt place  ;
: espacio_tras_presto_de_comando  ( ff -- )  space_after_command_prompt? !  ;
: nueva_línea_tras_presto_de_comando  ( ff -- )  cr_after_command_prompt? !  ;

: mensajes_de_error_lingüístico_detallados  ( ff -- )
  \ Indica si se mostrarán errores lingüísticos detallados
  \ (o bien breves y genéricos).
  verbose_language_errors? !
  ;

\ Fin de las palabras permitidas en el fichero configuración.

restore_vocabularies

: init_prompts
  \ Inicializa los prestos sus valores predeterminados.
  indent_pause_prompts? on
  s" ..." scroll_prompt place
  s" ..." narration_prompt place
  s" ..." scene_prompt place
  s" >" command_prompt place
  space_after_command_prompt? on
  cr_after_command_prompt? off
  ;

: init_config
  \ Inicializa las variables de configuración con sus valores predeterminados.
  woman_player? off
  castilian_quotes? on
  location_page? on
  cr? off
  ignore_unknown_words? off
  default_indentation /indentation !
  indent_first_line_too? on
  -1 narration_break_seconds !
  -1 scene_break_seconds !
  scene_page? on
  verbose_language_errors? on
  init_prompts  init_colors
  ;
: read_config_error
  s" Se ha producido un error leyendo el fichero de configuración."
  report wait
  ;
: read_config
  \ Lee el fichero de configuración.
  \ Pendiente!!! Atrapar errores al leerlo.
  base @ decimal
  only config_vocabulary
  config_file$ ['] included catch  ( x1 x2 n | 0 )
  if  2drop  read_config_error  then
  restore_vocabularies  base !
  ;
: get_config
  \ Inicializa las variables de configuración
  \ y lee el fichero de configuración.
  init_config read_config
  ;

\ }}} ##########################################################
section( Herramientas para crear el vocabulario del juego)  \ {{{

(

El vocabulario del juego está implementado como un
vocabulario de Forth, creado con el nombre de
'player_vocabulary'.  La idea es muy sencilla: crearemos en
este vocabulario nuevo palabras de Forth cuyos nombres sean
las palabras españolas que han de ser reconocidas en los
comandos del jugador. De este modo bastará interpretar la
frase del jugador con la palabra estándar EVALUATE 
[o, según el sistema Forth de que se trate, con la palabra
escrita a medida EVALUATE_COMMAND ], que ejecutará
cada palabra que contenga el texto.

A continuación definimos palabras que nos proporcionan una sintaxis
cómoda para crear un número variable de sinónimos de cada palabra del
vocabulario del jugador.

)

: parse_synonym  ( -- a u )
  \ Devuelve el siguiente sinónimo de la lista.
  begin   parse-name dup 0=
  while   2drop refill 0=
          abort" Error en el código fuente: lista de sinónimos incompleta"
  repeat
  \ 2dup ." sinónimo: " type space \ Depuración!!!
  ;
: (another_synonym?)  ( a u -- ff )
  \ ¿No se ha terminado la lista de sinónimos?
  s" }synonyms" compare
  ;
: another_synonym?  ( -- a u ff )
  \ Toma la siguiente palabra en el flujo de entrada
  \ y comprueba si es el final de la lista de sinónimos.
  parse_synonym 2dup (another_synonym?)
  ;
: synonyms{  (  xt "name#0" ... "name#n" "}synonyms" -- )
  \ Crea uno o varios sinónimos de una palabra.
  \ xt = Dirección de ejecución de la palabra a clonar
  begin  dup another_synonym? ( xt xt a u ff )
  while  (alias)
  repeat  2drop 2drop
  ;
: immediate_synonyms{  (  xt "name#0" ... "name#n" "}synonyms" -- )
  \ Crea uno o varios sinónimos inmediatos de una palabra.
  \ xt = Dirección de ejecución de la palabra a clonar
  begin  dup another_synonym?  ( xt xt a u ff )
  while  (alias)  immediate
  repeat  2drop 2drop
  ;

\ Resolución de entes ambiguos

(

Algunos términos del vocabulario del jugador pueden
referirse a varios entes. Por ejemplo, «hombre» puede
referirse al jefe de los refugiados o a Ambrosio,
especialmente antes de que Ulfius hable con él por primera
vez y sepa su nombre.  Otra palabra, como «ambrosio», solo
debe ser reconocida cuando Ambrosio ya se ha presentado
y ha dicho su nombre.

Para estos casos creamos palabras que devuelven el ente
adecuado en función de las circunstancias.  Serán llamadas
desde la palabra correspondiente del vocabulario del
jugador.

Si la ambigüedad no puede ser resuelta, o si la palabra
ambigua no debe ser reconocida en las circunstancias de
juego actuales, se devolverá un 'false', que tendrá el mismo
efecto que si la palabra ambigua no existiera en el
comando del jugador. Esto provocará después el error
adecuado.

Las acciones ambiguas, como por ejemplo «partir» [que puede
significar «marchar» o «romper»] no pueden ser resueltas de
esta manera, pues antes es necesario que que todos los
términos de la frase hayan sido evaluados. Por ello se
tratan como si fueran acciones como las demás, pero que al
ejecutarse resolverán la ambigüedad y llamarán a la acción
adecuada.

)

: (man) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «hombre» y sus sinónimos
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ Puede referirse al líder de los refugiados (si está presente),
  \ a Ambrosio (si está presente),
  \ o a los soldados (durante la marcha o la batalla).
  true case 
    leader% is_here? of  leader%  endof
    ambrosio% is_here? of  ambrosio%  endof
    pass_still_open? battle? or of  soldiers%  endof
    false swap
  endcase
  ;
: (men)  ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «hombres» y sus sinónimos
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ Puede referirse a los soldados o a los refugiados.
  [false] [if] \ Primera versión.
    true case
      location_28% am_i_there? location_29% am_i_there? or of  refugees%  endof
      pass_still_open? battle? or of  soldiers%  endof
      false swap
    endcase
  [else]  \ Segunda versión, lo mismo pero sin 'case':
    location_28% am_i_there? location_29% am_i_there? or
    if  refugees% exit  then
    pass_still_open? battle? or
    if  soldiers% exit  then
    false
  [then]
  ;
: (ambrosio) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «ambrosio»
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ La palabra «Ambrosio» es válida únicamente si
  \ el protagonista ha hablado con Ambrosio.
  ambrosio% dup conversations? and
  ;

: (cave) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «cueva»
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  true case
    my_location location_10% location_47% between of  cave%  endof
    the_cave_entrance_is_accessible? of  cave_entrance%  endof
    false swap
  endcase
  ;
: (entrance) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «entrada»
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  true case
    the_cave_entrance_is_accessible? of  cave_entrance%  endof
    \ Pendiente!!! Quizá no se implemente esto porque precisaría asociar a cave_entrance% el vocablo «salida/s», lo que crearía una ambigüedad adicional que resolver:
    \ location_10% am_i_there? of  cave_entrance%  endof
    false swap
  endcase
  ;
: (exits)  ( -- a )
  \ Devuelve el ente adecuado a la palabra «salida/s».
  the_cave_entrance_is_accessible?
  if    cave_entrance% 
  else  exits%
  then
  ;
: (stone) ( -- a )
  \ Devuelve el ente adecuado a la palabra «piedra».
  \ Puede referise, en orden preferente,
  \ a la piedra, a la esmeralda, a la pared de roca del desfiladero o a las rocas.
  true case
    stone% is_accessible? of  stone%  endof
    emerald% is_accessible? of  emerald%  endof
    location_08% am_i_there? of  ravine_wall%  endof
    rocks% swap
  endcase
  ;
: (wall) ( -- a )
  \ Devuelve el ente adecuado a la palabra «pared».
  location_08% am_i_there?
  if    ravine_wall%
  else  wall%
  then
  ;
: (somebody) ( -- a | false )
  \ Devuelve el ente adecuado a la palabra «alguien».
  \ (o FALSE si la ambigüedad no puede ser resuelta).
  \ Puede referirse a los soldados, a los refugiados
  \ o a ambrosio.
  true case
    pass_still_open? battle? or of  soldiers%  endof
    location_28% am_i_there? location_29% am_i_there? or of  refugees%  endof
    ambrosio% is_here? of  ambrosio%  endof
    false swap
  endcase
  ;

\ }}} ##########################################################
section( Vocabulario del juego)  \ {{{

\ Elegir el vocabulario 'player_vocabulary' para crear en él las nuevas palabras:
also player_vocabulary definitions

\ Pronombres

(
De momento no se implementan las formas sin tilde 
porque obligarían a distinguir sus usos como adjetivos
o sustantivos.
)

: éste  last_complement >masculine >singular @ complement!  ;
' éste synonyms{ ése }synonyms
: ésta  last_complement >feminine >singular @ complement!  ;
' ésta synonyms{ ésa }synonyms
: éstos  last_complement >masculine >plural @ complement!  ;
' éstos synonyms{ ésos }synonyms
: éstas  last_complement >feminine >plural @ complement!  ;
' éstas synonyms{ ésas }synonyms
: aquél  last_but_one_complement >masculine >singular @ complement!  ;
: aquélla  last_but_one_complement >feminine >singular @ complement!  ;
: aquéllos  last_but_one_complement >masculine >plural @ complement!  ;
: aquéllas  last_but_one_complement >feminine >plural @ complement!  ;

\ Verbos

: ir ['] do_go action!  ;
' ir synonyms{
  dirigirme diríjame diríjome
  dirigirse dirigíos diríjase
  dirigirte diríjote dirígete
  irme voyme váyame
  irse váyase
  irte vete
  moverme muévame muévome
  moverse muévase moveos
  moverte muévete 
  ve id idos voy vaya
  marchar marcha marchad marcho marche
  }synonyms

: abrir  ['] do_open action!  ;
' abrir synonyms{  abre abrid abro abra  }synonyms
: abrirlo  abrir éste  ;
' abrirlo synonyms{ ábrelo abridlo ábrolo ábralo }synonyms
: abrirla  abrir ésta  ;
' abrirla synonyms{ ábrela abridla ábrola ábrala }synonyms
: abrirlos  abrir éstos  ;
' abrirlos synonyms{ ábrelos abridlos ábrolos ábralos }synonyms
: abrirlas  abrir éstas  ;
' abrirlas synonyms{ ábrelas abridlas ábrolas ábralas }synonyms

: cerrar  ['] do_close action!  ;
' cerrar synonyms{  cierra cerrad cierro }synonyms
: cerrarlo  cerrar éste  ;
' cerrarlo synonyms{  ciérralo cerradlo ciérrolo ciérrelo }synonyms
: cerrarla  cerrar ésta  ;
' cerrarla synonyms{  ciérrala cerradla ciérrola ciérrela }synonyms
: cerrarlos  cerrar éstos  ;
' cerrarlos synonyms{  ciérralos cerradlos ciérrolos ciérrelos }synonyms
: cerrarlas  cerrar éstas  ;
' cerrarlas synonyms{  ciérralas cerradlas ciérrolas ciérrelas }synonyms

: coger  ['] do_take action!  ;
' coger synonyms{
  coge coged cojo coja
  agarrar agarra agarrad agarro agarre
  recoger recoge recoged recojo recoja
  }synonyms
: cogerlo  coger éste  ;
' cogerlo synonyms{
  cógelo cogedlo cójolo cójalo
  agarrarlo agárralo agarradlo agárrolo agárrelo
  recogerlo recógelo recogedlo recójolo recójalo
  }synonyms
: cogerla  coger éste  ;
' cogerla synonyms{
  cógela cogedla cójola cójala
  agarrarla agárrala agarradla agárrola agárrela
  recogerla recógela recogedla recójola recójala
  }synonyms
: cogerlos  coger éstos  ;
' cogerlos synonyms{
  cógelos cogedlos cójolos cójalos
  agarrarlos agárralos agarradlos agárrolos agárrelos
  recogerlos recógelos recogedlos recójolos recójalos
  }synonyms
: cogerlas  coger éstas  ;
' cogerlas synonyms{
  cógelas cogedlas cójolas cójalas
  agarrarlas agárralas agarradlas agárrolas agárrelas
  recogerlas recógelas recogedlas recójolas recójalas
  }synonyms

: tomar  ['] do_take|do_eat action!  ; \ Inacabado!!!
' tomar  synonyms{
  toma tomad tomo tome
  }synonyms
: tomarlo  tomar éste  ;
' tomarlo synonyms{ tómalo tomadlo tómolo tómelo }synonyms

: dejar  ['] do_drop action!  ;
' dejar synonyms{
  deja dejad dejo deje
  soltar suelta soltad suelto suelte
  tirar tira tirad tiro tire
  }synonyms
: dejarlo  dejar éste  ;
' dejarlo synonyms{
  déjalo dejadlo déjolo déjelo
  soltarlo suéltalo soltadlo suéltolo suéltelo
  tirarlo tíralo tiradlo tírolo tírelo
  }synonyms
: dejarlos  dejar éstos  ;
' dejarlos synonyms{
  déjalos dejadlos déjolos déjelos
  soltarlos suéltalos soltadlos suéltolos suéltelos
  tirarlos tíralos tiradlos tírolos tírelos
  }synonyms
: dejarla  dejar ésta  ;
' dejarla synonyms{
  déjala dejadla déjola déjela
  soltarla suéltala soltadla suéltola suéltela
  tirarla tírala tiradla tírola tírela
  }synonyms
: dejarlas  dejar éstas  ;
' dejarlas synonyms{
  déjalas dejadlas déjolas déjelas
  soltarlas suéltalas soltadlas suéltolas suéltelas
  tirarlas tíralas tiradlas tírolas tírelas
  }synonyms

: mirar  ['] do_look action!  ;
' mirar synonyms{
  m mira mirad miro mire
  contemplar contempla contemplad contemplo contemple
  observar observa observad observo observe
  }synonyms
: mirarlo  mirar éste  ;
' mirarlo synonyms{
  míralo miradlo mírolo mírelo
  contemplarlo contémplalo contempladlo contémplolo contémplelo
  observarlo obsérvalo observadlo obsérvolo obsérvelo
  }synonyms
: mirarla  mirar ésta  ;
' mirarla synonyms{
  mírala miradla mírola mírela
  contemplarla contémplala contempladla contémplola contémplela
  observarla obsérvala observadla obsérvola obsérvela
  }synonyms
: mirarlos  mirar éstos  ;
' mirarlos synonyms{
  míralos miradlos mírolos mírelos
  contemplarlos contémplalos contempladlos contémplolos contémplelos
  observarlos obsérvalos observadlos obsérvolos obsérvelos
  }synonyms
: mirarlas  mirar éstas  ;
' mirarlas synonyms{
  míralas miradlas mírolas mírelas
  contemplarlas contémplalas contempladlas contémplolas contémplelas
  observarlas obsérvalas observadlas obsérvolas obsérvelas
  }synonyms


: mirarse  ['] do_look_yourself action!  ;
' mirarse synonyms{
  mírese miraos
  mirarte mírate mírote mírete
  mirarme mírame miradme mírome míreme
  contemplarse contemplaos contémplese
  contemplarte contémplate contémplote contémplete
  contemplarme contémplame contempladme contémplome contémpleme
  observarse obsérvese observaos
  observarte obsérvate obsérvote obsérvete
  observarme obsérvame observadme obsérvome obsérveme
  }synonyms

: otear  ['] do_look_to_direction action!  ;
' otear synonyms{ oteo otea otead otee }synonyms

: x  ['] do_exits action!  ;
: salida  ['] do_exits (exits) action|complement!  ;
' salida synonyms{  salidas  }synonyms

: examinar  ['] do_examine action!  ;
' examinar synonyms{ ex examina examinad examino examine  }synonyms
: examinarlo  examinar éste  ;
' examinarlo synonyms{ examínalo examinadlo examínolo examínelo  }synonyms
: examinarlos  examinar éstos  ;
' examinarlos synonyms{ examínalos examinadlos examínolos examínelos  }synonyms
: examinarla  examinar ésta  ;
' examinarla synonyms{ examínala examinadla examínola examínela  }synonyms
: examinarlas  examinar éstas  ;
' examinarlas synonyms{ examínalas examinadlas examínolas examínelas  }synonyms

: examinarse  ['] do_examine action! protagonist% complement!  ;
' examinarse synonyms{
  examínese examinaos
  examinarte examínate examínete
  examinarme examíname examinadme examínome examíneme
  }synonyms

: registrar  ['] do_search action!  ;
' registrar synonyms{  registra registrad registro registre  }synonyms
: registrarlo  registrar éste  ;
' registrarlo synonyms{ regístralo registradlo regístrolo regístrelo  }synonyms
: registrarla  registrar ésta  ;
' registrarla synonyms{ regístrala registradla regístrola regístrela  }synonyms
: registrarlos  registrar éstos  ;
' registrarlos synonyms{ regístralos registradlos regístrolos regístrelos  }synonyms
: registrarlas  registrar éstas  ;
' registrarlas synonyms{ regístralas registradlas regístrolas regístrelas  }synonyms

: i  ['] do_inventory inventory% action|complement!  ;
' i synonyms{  inventario  }synonyms
: inventariar  ['] do_inventory action!  ;
' inventariar synonyms{
  inventaría inventariad inventarío inventaríe
  registrarse regístrase regístrese
  registrarme regístrame registradme regístrome regístreme
  registrarte regístrate regístrote regístrete
  }synonyms

: hacer  ['] do_do action!  ;
' hacer synonyms{  haz haced hago haga  }synonyms
: hacerlo  hacer éste  ;
' hacerlo synonyms{  hazlo hacedlo hágolo hágalo  }synonyms
: hacerla  hacer ésta  ;
' hacerla synonyms{  hazla hacedla hágola hágala  }synonyms
: hacerlos  hacer éstos  ;
' hacerlos synonyms{  hazlos hacedlos hágolos hágalos  }synonyms
: hacerlas  hacer éstas  ;
' hacerlas synonyms{  hazlas hacedlas hágolas hágalas  }synonyms

: fabricar  ['] do_make action!  ;
' fabricar synonyms{
  fabrica fabricad fabrico fabrique
  construir construid construye construyo construya
  }synonyms
: fabricarlo  fabricar éste  ;
' fabricarlo synonyms{
  fabrícalo fabricadlo fabrícolo fabríquelo
  construirlo constrúyelo construidlo constrúyolo constrúyalo
  }synonyms
: fabricarla  fabricar éste  ;
' fabricarla synonyms{
  fabrícala fabricadla fabrícola fabríquela
  construirla constrúyela construidla constrúyola constrúyala
  }synonyms
: fabricarlos  fabricar éste  ;
' fabricarlos synonyms{
  fabrícalos fabricadlos fabrícolos fabríquelos
  construirlos constrúyelos construidlos constrúyolos constrúyalos
  }synonyms
: fabricarlas  fabricar éste  ;
' fabricarlas synonyms{
  fabrícalas fabricadlas fabrícolas fabríquelas
  construirlas constrúyelas construidlas constrúyolas constrúyalas
  }synonyms

: nadar  ['] do_swim action!  ;
' nadar synonyms{
  nada nado nade
  bucear bucea bucead buceo bucee
  sumergirse sumérgese sumérjase
  sumergirme sumérgeme sumérjome sumérjame
  sumergirte sumérgete sumergíos sumérjote sumérjate
  zambullirse zambullíos zambúllese zambúllase
  zambullirme zambúlleme zambúllome zambúllame
  zambullirte zambúllete zambúllote zambúllate
  bañarse báñase báñese
  bañarme báñame báñome báñeme
  bañarte báñate bañaos báñote báñete
  }synonyms

: quitarse  ['] do_take_off action!  ;
' quitarse synonyms{
  quítase quitaos quítese
  quitarte quítate quítote quítete
  quitarme quítame quítome quíteme
  }synonyms
: quítarselo  quitarse éste  ;
' quítarselo synonyms{
  quitártelo quitáoslo quíteselo
  quitármelo quítamelo quítomelo quítemelo
  }synonyms
: quítarsela  quitarse ésta  ;
' quítarsela synonyms{
  quitártela quitáosla quítesela
  quitármela quítamela quítomela quítemela
  }synonyms
: quítarselos  quitarse éstos  ;
' quítarselos synonyms{
  quitártelos quitáoslos quíteselos
  quitármelos quítamelos quítomelos quítemelos
  }synonyms
: quítarselas  quitarse éstas  ;
' quítarselas synonyms{
  quitártelas quitáoslas quíteselas
  quitármelas quítamelas quítomelas quítemelas
  }synonyms

: ponerse  ['] do_put_on action!  ;
' ponerse synonyms{
  póngase poneos
  ponerme ponme póngome póngame
  ponerte ponte póngote póngate
  colocarse colocaos colóquese
  colocarte colócate colóquete
  colocarme colócame colócome colóqueme
  }synonyms
\ Crear acción!!! vestir [con], parte como sinónimo y parte independiente
: ponérselo  ponerse éste  ;
' ponérselo synonyms{
  póngaselo ponéoslo
  ponérmelo pónmelo póngomelo póngamelo
  ponértelo póntelo póngotelo póngatelo
  colocórselo colocáoslo colóqueselo
  colocártelo colócatelo colóquetelo
  colocármelo colócamelo colócomelo colóquemelo 
  }synonyms
: ponérsela  ponerse ésta  ;
' ponérsela synonyms{
  póngasela ponéosla
  ponérmela pónmela póngomela póngamela
  ponértela póntela póngotela póngatela
  colocórsela colocáosla colóquesela
  colocártela colócatela colóquetela
  colocármela colócamela colócomela colóquemela 
  }synonyms
: ponérselos  ponerse éstos  ;
' ponérselos synonyms{
  póngaselos ponéoslos
  ponérmelos pónmelos póngomelos póngamelos
  ponértelos póntelos póngotelos póngatelos
  colocórselos colocáoslos colóqueselos
  colocártelos colócatelos colóquetelos
  colocármelos colócamelos colócomelos colóquemelos 
  }synonyms
: ponérselas  ponerse éstas  ;
' ponérselas synonyms{
  póngaselas ponéoslas
  ponérmelas pónmelas póngomelas póngamelas
  ponértelas póntelas póngotelas póngatelas
  colocórselas colocáoslas colóqueselas
  colocártelas colócatelas colóquetelas
  colocármelas colócamelas colócomelas colóquemelas 
  }synonyms
 

: matar  ['] do_kill action!  ;
' matar synonyms{
  mata matad mato mate
  asesinar asesina asesinad asesino asesine
  aniquilar aniquila aniquilad aniquilo aniquile
  }synonyms
: matarlo  matar éste  ;
' matarlo synonyms{
  mátalo matadlo mátolo mátelo
  asesinarlo asesínalo asesinadlo asesínolo asesínelo
  aniquilarlo aniquínalo aniquinadlo aniquínolo aniquínelo
  }synonyms
: matarla  matar ésta  ;
' matarla synonyms{
  mátala matadla mátola mátela
  asesinarla asesínala asesinadla asesínola asesínela
  aniquilarla aniquínala aniquinadla aniquínola aniquínela
  }synonyms
: matarlos  matar éstos  ;
' matarlos synonyms{
  mátalos matadlos mátolos mátelos
  asesinarlos asesínalos asesinadlos asesínolos asesínelos
  aniquilarlos aniquínalos aniquinadlos aniquínolos aniquínelos
  }synonyms
: matarlas  matar éstas  ;
' matarlas synonyms{
  mátalas matadlas mátolas mátelas
  asesinarlas asesínalas asesinadlas asesínolas asesínelas
  aniquilarlas aniquínalas aniquinadlas aniquínolas aniquínelas
  }synonyms

: golpear  ['] do_hit action!  ;
' golpear synonyms{
  golpea golpead golpeo golpee
  sacudir sacude sacudid sacudo sacuda
  }synonyms
: golpearla  golpear ésta  ;
' golpearla synonyms{
  golpéala golpeadla golpéola golpéela
  sacudirla sacúdela sacudidla sacúdola sacúdala
  }synonyms
: golpearlos  golpear éstos  ;
' golpearlos synonyms{
  golpéalos golpeadlos golpéolos golpéelos
  sacudirlos sacúdelos sacudidlos sacúdolos sacúdalos
  }synonyms
: golpearlas  golpear éstas  ;
' golpearlas synonyms{
  golpéalas golpeadlas golpéolas golpéelas
  sacudirlas sacúdelas sacudidlas sacúdolas sacúdalas
  }synonyms

: atacar  ['] do_attack action!  ;
' atacar synonyms{  
  ataca atacad ataco ataque
  agredir agrede agredid agredo agreda
  }synonyms
: atacarlo  atacar éste  ;
' atacarlo synonyms{  
  atácalo atacadlo atácolo atáquelo
  agredirlo agrédelo agredidlo agrédolo agrédalo
  }synonyms
: atacarla  atacar ésta  ;
' atacarla synonyms{  
  atácala atacadla atácola atáquela
  agredirla agrédela agredidla agrédola agrédala
  }synonyms
: atacarlos  atacar éstos  ;
' atacarlos synonyms{  
  atácalos atacadlos atácolos atáquelos
  agredirlos agrédelos agredidlos agrédolos agrédalos
  }synonyms
: atacarlas  atacar éstas  ;
' atacarlas synonyms{  
  atácalas atacadlas atácolas atáquelas
  agredirlas agrédelas agredidlas agrédolas agrédalas
  }synonyms

: romper  ['] do_break action!  ;
' romper synonyms{
  rompe romped rompo rompa
  despedazar despedaza despedazad despedazo despedace
  destrozar destroza destrozad destrozo destroce
  dividir divide dividid divido divida
  cortar corta cortad corto corte
  }synonyms
: romperlo  romper éste  ;
' romperlo synonyms{
  rómpelo rompedlo rómpolo rómpalo
  despedazarlo despedazalo despedazadlo despedázolo despedácelo
  destrozarlo destrázalo destrozadlo destrózolo destrócelo
  dividirlo divídelo divididlo divídolo divídalo
  cortarlo cortalo cortadlo córtolo córtelo
  }synonyms
: romperla  romper ésta  ;
' romperla synonyms{
  rómpela rompedla rómpola rómpala
  despedazarla despedazala despedazadla despedázola despedácela
  destrozarla destrázala destrozadla destrózola destrócela
  dividirla divídela divididla divídola divídala
  cortarla córtala cortadla córtola córtela
  }synonyms
: romperlos  romper éstos  ;
' romperlos synonyms{
  rómpelos rompedlos rómpolos rómpalos
  despedazarlos despedazalos despedazadlos despedázolos despedácelos
  destrozarlos destrázalos destrozadlos destrózolos destrócelos
  dividirlos divídelos divididlos divídolos divídalos
  cortarlos córtalos cortadlos córtolos córtelos
  }synonyms
: romperlas  romper éstas  ;
' romperlas synonyms{
  rómpelas rompedlas rómpolas rómpalas
  despedazarlas despedazalas despedazadlas despedázolas despedácelas
  destrozarlas destrázalas destrozadlas destrózolas destrócelas
  dividirlas divídelas divididlas divídolas divídalas
  cortarlas córtalas cortadlas córtolas córtelas
  }synonyms

\ quebrar \ Pendiente!!!
\ desgarrar \ Pendiente!!!

: asustar  ['] do_fear action!  ;
' asustar synonyms{
  asusto asusta asustad asuste
  amedrentar amedrento amedrenta amedrentad amedrente
  acojonar acojono acojona acojonad acojone
  atemorizar atemoriza atemorizad atemorizo atemorice
  }synonyms
: asustarlo  asustar éste  ;
' asustarlo synonyms{
  asústolo asústalo asustadlo asústelo
  amedrentarlo amedréntolo amedréntalo amedrentadlo amedréntelo
  acojonarlo acojónolo acojónalo acojonadlo acojónelo
  atemorizarlo atemorízalo atemorizadlo atemorízolo atemorícelo
  }synonyms
: asustarla  asustar ésta  ;
' asustarla synonyms{
  asústola asústala asustadla asústela
  amedrentarla amedréntola amedréntala amedrentadla amedréntela
  acojonarla acojónola acojónala acojonadla acojónela
  atemorizarla atemorízala atemorizadla atemorízola atemorícela
  }synonyms
: asustarlos  asustar éstos  ;
' asustarlos synonyms{
  asústolos asústalos asustadlos asústelos
  amedrentarlos amedréntolos amedréntalos amedrentadlos amedréntelos
  acojonarlos acojónolos acojónalos acojonadlos acojónelos
  atemorizarlos atemorízalos atemorizadlos atemorízolos atemorícelos
  }synonyms
: asustarlas  asustar éstas  ;
' asustarlas synonyms{
  asústolas asústalas asustadlas asústelas
  amedrentarlas amedréntolas amedréntalas amedrentadlas amedréntelas
  acojonarlas acojónolas acojónalas acojonadlas acojónelas
  atemorizarlas atemorízalas atemorizadlas atemorízolas atemorícelas
  }synonyms

: afilar  ['] do_sharpen action!  ;
' afilar synonyms{  afila afilad afilo afile  }synonyms
: afilarlo  afilar éste  ;
' afilarlo synonyms{  afílalo afiladlo afílolo afílelo  }synonyms
: afilarla  afilar ésta  ;
' afilarla synonyms{  afílala afiladla afílola afílela  }synonyms
: afilarlos  afilar éstos  ;
' afilarlos synonyms{  afílalos afiladlos afílolos afílelos  }synonyms
: afilarlas  afilar éstas  ;
' afilarlas synonyms{  afílalas afiladlas afílolas afílelas  }synonyms

: partir  ['] do_go|do_break action!  ;
' partir synonyms{  parto partid parta  }synonyms
\ «parte» está en la sección final de ambigüedades
: partirlo  partir éste  ;
' partirlo synonyms{  pártelo pártolo partidlo pártalo  }synonyms
: partirla  partir ésta  ;
' partirla synonyms{  pártela pártola partidla pártala  }synonyms
: partirlos  partir éstos  ;
' partirlos synonyms{  pártelos pártolos partidlos pártalos  }synonyms
: partirlas  partir éstas  ;
' partirlas synonyms{  pártelas pártolas partidlas pártalas  }synonyms

: esperar  \ Pendiente!!!
  ;
' esperar synonyms{
  z espera esperad espero espere
  aguardar aguarda aguardad aguardo aguarde
  }synonyms
: esperarlo  esperar éste  ;
' esperarlo synonyms{
  esperadlo espérolo espérelo
  aguardarlo aguárdalo aguardadlo aguárdolo aguárdelo
  }synonyms
: esperarla  esperar ésta  ;
' esperarla synonyms{
  esperadla espérola espérela
  aguardarla aguárdala aguardadla aguárdola aguárdela
  }synonyms
: esperarlos  esperar éstos  ;
' esperarlos synonyms{
  esperadlos espérolos espérelos
  aguardarlos aguárdalos aguardadlos aguárdolos aguárdelos
  }synonyms
: esperarlas  esperar éstas  ;
' esperarlas synonyms{
  esperadlas espérolas espérelas
  aguardarlas aguárdalas aguardadlas aguárdolas aguárdelas
  }synonyms

\ Pendiente!!!:
\ meter introducir insertar colar encerrar

: ulfius  ulfius% complement!  ;
: ambrosio  (ambrosio) complement!  ;
: hombre  (man) complement!  ;
' hombre synonyms{  señor tipo individuo persona  }synonyms
: hombres  (men) complement!  ;
' hombres synonyms{ gente personas }synonyms
\ Ambigüedad!!!:
\ «jefe» podría ser también el jefe de los enemigos durante la batalla:
: jefe  leader% complement!  ;
' jefe synonyms{
  líder viejo anciano abuelo
  }synonyms
: soldados  soldiers% complement!  ;
' soldados synonyms{
  guerreros luchadores combatientes camaradas
  compañeros oficiales suboficiales militares
  guerrero luchador combatiente camarada
  compañero oficial suboficial militar
  }synonyms
: multitud  refugees% complement!  ;
' multitud synonyms{
  niño niños niña niñas
  muchacho muchachos muchacha muchachas
  adolescente adolescentes 
  ancianos anciana ancianas mayores viejos vieja viejas
  joven jóvenes
  abuela abuelos abuelas
  nieto nietos nieta nietas
  padre padres madre madres mamá mamás papás
  bebé bebés beba bebas bebito bebitos bebita bebitas
  pobres desgraciados desafortunados
  desgraciadas desafortunadas
  muchedumbre masa enjambre
  }synonyms  
: refugiados leader% conversations? if  multitud  then  ;
' refugiados synonyms{ refugiada refugiadas }synonyms  
: refugiado leader% conversations? if  viejo  then  ;
 
: altar  altar% complement!  ;
: arco  arch% complement!  ;
: capa  cloak% complement!  ; \ pendiente!!! hijuelo?
' capa synonyms{  lana  }synonyms
\ ' capa synonyms{  abrigo  }synonyms \ diferente género!!!
: coraza  cuirasse% complement!  ;
' coraza synonyms{  armadura  }synonyms
: puerta  door% complement!  ;
: esmeralda  emerald% complement!  ;
' esmeralda synonyms{  joya  }synonyms
\ pendiente!!! piedra_preciosa brillante
: derrumbe fallen_away% complement!  ;
: banderas  flags% complement!  ;
' banderas synonyms{
    bandera pendones enseñas pendón enseña
    mástil mástiles
    estandarte estandartes 
  }synonyms \ pendiente: estandarte, enseña... otro género!!!
: dragones  flags% is_known? if  banderas  then  ;
' dragones synonyms{ dragón }synonyms
: pedernal  flint% complement!  ;
: ídolo  idol% complement!  ;
' ídolo synonyms{  ojo orificio agujero  }synonyms
\ pendiente!!! separar los sinónimos de ídolo
: llave  key% complement!  ;
: lago  lake% complement!  ;
' lago synonyms{  laguna agua estanque  }synonyms  \ diferente género!!!
: candado  lock% complement!  ;
' candado synonyms{  cerrojo  }synonyms
: tronco  log% complement!  ;
' tronco synonyms{  leño madero  }synonyms
\ pendiente!!! madera
: trozo  piece% complement!  ;
' trozo synonyms{  pedazo retal tela  }synonyms
: harapo  rags% complement!  ;
: rocas 
  \ Este término puede referise a las rocas o al derrumbe.
  location_09% am_i_there?
  if  fallen_away%  else  rocks%  then  complement!
  ;
' rocas synonyms{  piedras pedruscos  }synonyms
: piedra  (stone) complement!  ;
' piedra synonyms{  roca pedrusco  }synonyms
: serpiente  snake% complement!  ;
' serpiente synonyms{  reptil ofidio culebra animal bicho  }synonyms
: espada  sword% complement!  ;
' espada synonyms{  tizona arma  }synonyms
\ Nota!!!: "arma" es femenina pero usa artículo "él", contemplar en los cálculos de artículo.
: hilo  thread% complement!  ;
' hilo synonyms{  hebra  }synonyms
: antorcha  torch% complement!  ;
: cascada  waterfall% complement!  ;
' cascada synonyms{  catarata  }synonyms
: catre  s" catre" bed% mname! bed% complement!  ;
' catre synonyms{  camastro  }synonyms
: cama s" cama" bed% fname! bed% complement!  ;
: velas  candles% complement!  ;
' velas synonyms{  vela  }synonyms
: mesa  table% complement!  ;
' mesa synonyms{  mesita pupitre  }synonyms
: puente  bridge% complement!  ;
: alguien  (somebody) complement!  ;
: hierba  s" hierba" grass% fname! grass% complement!  ;
: hierbas  s" hierbas" grass% fnames! grass% complement!  ;
: hierbajo  s" hierbajo" grass% mname! grass% complement!  ;
: hierbajos  s" hierbajos" grass% mnames! grass% complement!  ;
: hiedra  s" hiedra" grass% fname! grass% complement!  ;
: hiedras  s" hiedras" grass% fnames! grass% complement!  ;

: n  ['] do_go_north north% action|complement!  ;
' n synonyms{  norte septentrión  }synonyms

: s  ['] do_go_south south% action|complement!  ;
' s synonyms{  sur meridión  }synonyms

: e  ['] do_go_east east% action|complement!  ;
' e synonyms{  este oriente levante  }synonyms

: o  ['] do_go_west west% action|complement!  ;
' o synonyms{  oeste occidente poniente  }synonyms

: a  ['] do_go_up up% action|complement!  ;
' a synonyms{  arriba  }synonyms
: subir  ['] do_go_up action!  ;
' subir synonyms{  sube subid subo suba  }synonyms
' subir synonyms{  ascender asciende ascended asciendo ascienda  }synonyms
' subir synonyms{  subirse subíos súbese súbase  }synonyms
' subir synonyms{  subirte súbete súbote súbate  }synonyms

: b  ['] do_go_down down% action|complement!  ;
' b synonyms{  abajo  }synonyms
: bajar  ['] do_go_down action!  ;
' bajar synonyms{  baja bajad bajo baje  }synonyms
' bajar synonyms{  bajarse bajaos bájase bájese  }synonyms
' bajar synonyms{  bajarte bájate bájote bájete  }synonyms
' bajar synonyms{  descender desciende descended desciendo descienda  }synonyms

: salir  ['] do_go_out action!  ;
' salir synonyms{  sal salid salgo salga  }synonyms
\ ambigüedad!!! sal
' salir synonyms{  salirse }synonyms
' salir synonyms{  salirme sálgome  }synonyms
' salir synonyms{  salirte }synonyms
\ ambigüedad!!! salte
: fuera  ['] do_go_out out% action|complement!  ;
' fuera synonyms{  afuera }synonyms
: exterior  out% complement!  ;
: entrar ['] do_go_in action!  ;
' entrar synonyms{  entra entrad entro entre  }synonyms
' entrar synonyms{  entrarse entraos éntrese éntrase  }synonyms
' entrar synonyms{  entrarte éntrete éntrate  }synonyms
: dentro  ['] do_go_in in% action|complement!  ;
' dentro synonyms{  adentro  }synonyms
: interior  in% complement!  ;

: escalar  ['] do_climb action!  ;
' escalar synonyms{  escala escalo escale  }synonyms
' escalar synonyms{  trepar trepa trepo trepe  }synonyms

: hablar  ['] do_speak action!  ;
\ Pendiente!!! Crear nuevas palabras según la preposición que necesiten.
\ Pendiente!!! Separar matices.
' hablar synonyms{
  habla hablad hablo hable 
  hablarle háblale háblole háblele
  conversar conversa conversad converso converse
  charlar charla charlad charlo charle
  decir di decid digo diga
  decirle dile decidle dígole dígale
  platicar platica platicad platico platique
  platicarle platícale platicadle platícole platíquele
  }synonyms
  \ contar cuenta cuento cuente  \ !!!
  \ contarle cuéntale cuéntole cuéntele  \ !!!

: presentarse  ['] do_introduce_yourself action!  ;
' presentarse synonyms{
  preséntase preséntese
  presentarte preséntate presentaos preséntete
  }synonyms

\ Términos asociados a entes globales o virtuales

: nubes  clouds% complement!  ;
\ Pendiente!!! ¿cúmulo-nimbos?, ¿nimbos?
' nubes synonyms{  nube estratocúmulo estratocúmulos cirro cirros  }synonyms
: suelo  floor% complement!  ;
' suelo synonyms{  suelos tierra firme  }synonyms
\ Pendiente!!! Añadir «piso», que es ambiguo
: cielo  sky% complement!  ;
' cielo synonyms{  cielos firmamento  }synonyms
: techo  ceiling% complement!  ;
: cueva  (cave) complement!  ;
' cueva synonyms{  caverna gruta  }synonyms
: entrada  (entrance) complement!  ;
\ Pendiente!!! ¿Implementar cambio de nombre y/o género gramatical? (entrada, acceso):
' entrada synonyms{  acceso }synonyms
: enemigo  enemy% complement!  ;
' enemigo synonyms{ enemigos sajón sajones }synonyms
: todo ;  \ Pendiente!!!
\ Pendiente!!! ¿Implementar cambio de nombre y/o género gramatical? (pared/es, muro/s):
: pared  (wall) complement!  ;
' pared  synonyms{ muro }synonyms
: paredes  wall% complement!  ;
' paredes  synonyms{ muros }synonyms

\ Artículos

\ Los artículos no hacen nada pero es necesario crearlos
\ para que no provoquen un error cuando el intérprete de
\ comandos funcione en el modo opcional de no ignorar las
\ palabras desconocidas.

: la  ;
' la synonyms{ las el los una un unas unos }synonyms

\ Adjetivos demostrativos

\ Lo mismo hacemos con los adjetivos demostrativos
\ y pronombres demostrativos sin tilde; salvo «este», que siempre
\ será interpretado como punto cardinal.

: esta ;
' esta synonyms{ estas estos }synonyms

\ (Seudo)preposiciones 

: con
  \ Uso: Herramienta
  «con»_preposition preposition!
  ;
' con synonyms{ usando empleando utilizando mediante }synonyms
false [if]  \ Descartado!!! Pendiente!!!
: a
  \ Uso: Destino de movimiento, objeto indirecto
  «a»_preposition preposition!
  ;
' a synonyms{ al }synonyms
: de
  \ Uso: Origen de movimiento, propiedad
  «de»_preposition preposition!
  ;
: hacia
  \ Uso: Destino de movimiento, destino de lanzamiento
  «hacia»_preposition preposition!
  ;
: contra
  \ Uso: Destino de lanzamiento
  «contra»_preposition preposition!
  ;
: para
  \ Uso: Destino de movimiento, destino de lanzamiento
  «para»_preposition preposition!
  ;
: por
  \ Uso: Destino de movimiento 
  «por»_preposition preposition!
  ;
[then]

\ Meta

\ Antiguo!!!
\ : save  ['] do_save_the_game action!  ;

\ Términos ambiguos

: cierre  action @ if  candado  else  cerrar  then  ;
: parte  action @ if  trozo  else  partir  then  ;

\ Comandos del sistema

: COLOREAR
  \ Restaura los colores predeterminados.
  init_colors  new_page  my_location describe
  ;
' COLOREAR synonyms{
  COLOREA COLOREO
  RECOLOREAR RECOLOREA RECOLOREO
  PINTAR PINTA PINTO
  LIMPIAR LIMPIA LIMPIO
  }synonyms
' get_config alias CONFIGURAR  \ Restaura la configuración predeterminada y después carga el fichero de configuración
' CONFIGURAR synonyms{
  CONFIGURA CONFIGURO
  }synonyms
: GRABAR  ( "name" -- )
  \ Graba el estado de la partida en un fichero.
  [debug_parsing] ?halto" en GRABAR 1"  \ depuración!!!
  parse-name >sb
  [debug_parsing] ?halto" en GRABAR 2"  \ depuración!!!
  ['] do_save_the_game action!
  [debug_parsing] ?halto" en GRABAR 3"  \ depuración!!!
  ;  immediate
' GRABAR immediate_synonyms{
  GRABA GRABO
  EXPORTAR EXPORTA EXPORTO
  SALVAR SALVA SALVO
  GUARDAR GUARDA GUARDO
  }synonyms
: CARGAR  ( "name" -- )
  \ Carga el estado de la partida de un fichero.
  [debug_parsing] ?halto" en CARGAR 1"  \ depuración!!!
  parse-name
  [debug_parsing] ?halto" en CARGAR 1a"  \ depuración!!!
  >sb
  [debug_parsing] ?halto" en CARGAR 2"  \ depuración!!!
  ['] do_load_the_game action!
  [debug_parsing] ?halto" en CARGAR 3"  \ depuración!!!
  ;  immediate
' CARGAR immediate_synonyms{
  CARGA CARGO
  IMPORTAR IMPORTA IMPORTO
  LEER LEE LEO
  RECARGAR RECARGA RECARGO
  RECUPERAR RECUPERA RECUPERO
  RESTAURAR RESTAURA RESTAURO
  }synonyms
: FIN  do_finish  ;  \ Abandonar la partida
' FIN synonyms{
  ACABAR ACABA ACABO 
  ADIÓS
  APAGAR APAGA APAGO
  CERRAR CIERRA CIERRO
  CONCLUIR CONCLUYE CONCLUYO
  FINALIZAR FINALIZA FINALIZO
  SALIR SAL SALGO
  TERMINAR TERMINA TERMINO 
  }synonyms
: AYUDA
  \ Pendiente!!! 
  ;
' AYUDA synonyms{
  AYUDAR AYUDITA AYUDAS
  INSTRUCCIONES MANUAL GUÍA MAPA PLANO MENÚ
  PISTA PISTAS
  SOCORRO AUXILIO FAVOR
  }synonyms

\ Comandos para usar durante el desarrollo!!!:
: forth  (do_finish)  ;
: bye  bye  ;
: quit  quit  ; 

restore_vocabularies

\ }}} ##########################################################
section( Vocabulario para entradas «sí» o «no»)  \ {{{

(

Para los casos en que el programa hace una pregunta que debe
ser respondida con «sí» o «no», usamos un truco análogo al
del vocabulario del juego: creamos un vocabulario específico
con palabras cuyos nombres sean las posibles respuestas:
«sí», «no», «s» y «n».  Estas palabras actualizarán una
variable,  con cuyo valor el programa sabrá si se ha
producido una respuesta válida o no y cuál es.

En principio, si el jugador introdujera varias respuestas
válidas la última sería la que tendría efecto. Por ejemplo,
la respuesta «sí sí sí sí sí no» sería considerada negativa.
Para dotar al método de una chispa de inteligencia, las
respuestas no cambian el valor de la variable sino que lo
incrementan o decrementan. Así el mayor número de respuestas
afirmativas o negativas decide el resultado; y si la
cantidad es la misma, como por ejemplo en «sí sí no no», el
resultado será el mismo que si no se hubiera escrito nada.

)

variable #answer  \ Su valor será 0 si no ha habido respuesta válida; negativo para «no»; y positivo para «sí»
: answer_undefined
  \ Inicializa la variable antes de hacer la pregunta.
  #answer off
  ;
: think_it_again$  ( -- a u )
  \ Devuelve un mensaje complementario para los errores.
  s{ s" Piénsalo mejor"
  s" Decídete" s" Cálmate" s" Concéntrate"
  s" Presta atención"
  s{ s" Prueba" s" Inténtalo" }s again$ s&
  s" No es tan difícil" }s colon+
  ;
: yes_but_no$  ( -- a u )
  \ Devuelve mensaje de error: se dijo «no» tras «sí».
  s" ¿Primero «sí»" but|and$ s&
  s" después «no»?" s& think_it_again$ s&
  ;
: no_but_yes$  ( -- a u )
  \ Devuelve mensaje de error: se dijo «sí» tras «no».
  s" ¿Primero «no»" but|and$ s&
  s" después «sí»?" s& think_it_again$ s&
  ;
: yes_but_no
  \ Muestra error: se dijo «no» tras «sí».
  yes_but_no$ narrate
  ;
' yes_but_no constant yes_but_no_error#
: no_but_yes
  \ Muestra error: se dijo «sí» tras «no».
  no_but_yes$ narrate
  ;
' no_but_yes constant no_but_yes_error#
: two_options_only$  ( -- a u )
  \ Devuelve un mensaje que informa de las opciones disponibles.
  ^only$ s{ s" hay" s" tienes" }s&
  s" dos" s& s" respuestas" s" posibles" rnd2swap s& s& colon+
  s" «sí»" s" «no»" both& s" (o sus iniciales)" s& period+
  ;
: two_options_only
  \ Muestra error: sólo hay dos opciones.
  two_options_only$ narrate
  ;
' two_options_only constant two_options_only_error#
: wrong_yes$  ( -- a u )
  \ Devuelve el mensaje usado para advertir de que se ha escrito mal «sí».
  s{ s" ¿Si qué...?" s" ¿Si...?" s" ¿Cómo «si»?" s" ¿Cómo que «si»?" }s
  s" No" s& s{
  s{ s" hay" s" puedes poner" }s  s" condiciones" s&
  s{ s" hay" s" tienes" }s s" nada que negociar" s& }s&
  s{ s" aquí" s" en esto" s" en esta cuestión" }s& period+
  \ two_options_only$ s?&
  ;
: wrong_yes
  \ Muestra error: se ha usado la forma errónea «si».
  wrong_yes$ narrate
  ;
' wrong_yes constant wrong_yes_error#
: answer_no
  \ Anota una respuesta negativa.
  #answer @ 0> yes_but_no_error# and throw  \ Provocar error si antes había habido síes
  #answer --
  ;
: answer_yes
  \ Anota una respuesta afirmativa.
  #answer @ 0< no_but_yes_error# and throw  \ Provocar error si antes había habido noes
  #answer ++
  ;

also answer_vocabulary definitions  \ Las palabras que siguen se crearán en dicho vocabulario

: sí  answer_yes  ;
: sÍ  answer_yes  ;  \ Necesario, por el problema del cambio de capitalidad y UTF-8
: s  answer_yes  ;
: no  answer_no  ;
: n  answer_no  ;
: si  wrong_yes_error# throw  ;

restore_vocabularies

\ }}} ##########################################################
section( Entrada de comandos)  \ {{{

(

Para la entrada de comandos se usa la palabra de Forth
'accept', que permite limitar el número máximo de caracteres
que serán aceptados. Por desgracia ACCEPT permite escribir
más y después trunca la cadena, por lo que sería mejor
escribir una alternativa.

)

svariable command  \ Zona de almacenamiento del comando

: command_prompt$  ( -- a u )
  \ Devuelve el presto de entrada de comandos.
  command_prompt count
  ;
: /command  ( -- u )
  \ Devuelve la longitud máxima posible para un comando.
  \ Tomar las columnas disponibles y restarles la indentación:
  cols /indentation @ - 
  \ Restar la longitud del presto si no lleva detrás un salto de línea:
  cr_after_command_prompt? @ 0= abs command_prompt$ nip * -
  \ Restar uno si tras el presto va no va salto de línea pero sí un espacio:
  cr_after_command_prompt? @ 0= space_after_command_prompt? @ and abs -
  ;
: wait_for_input  ( -- a u )
  \ Espera y devuelve el comando introducido por el jugador.
  input_color  command /command accept
  command swap  str+strip  \ Eliminar espacios laterales
  ;
: .command_prompt
  \ Imprime un presto para la entrada de comandos.
  command_prompt$ ['] command_prompt_color |paragraph
  cr_after_command_prompt? @ if
    cr+
  else
    space_after_command_prompt?
    if  background_color space  then
  then
  ;
: listen  ( -- a u )
  \ Imprime un presto y devuelve el comando introducido por el jugador.
  [debug_info] [if]  "" debug  [then]  \ Depuración!!!
  .command_prompt wait_for_input
  [debug] [if]  cr cr ." <<<" 2dup type ." >>>" cr cr  [then]  \ Depuración!!!
  ; 

\ }}} ##########################################################
section( Entrada de respuestas de tipo «sí o no»)  \ {{{

: yes|no  ( a u -- n )
  \ Evalúa una respuesta a una pregunta del tipo «sí o no».
  \ a u = Respuesta a evaluar
  \ n = Resultado (un número negativo para «no» y positivo para «sí»; cero si no se ha respondido ni «sí» ni «no», o si se produjo un error)
  answer_undefined
  only answer_vocabulary
  ['] evaluate_command catch
  dup if  nip nip  then  \ Reajustar la pila si ha habido error
  dup ?wrong 0=  \ Ejecutar el posible error y preparar su indicador para usarlo en el resultado
  #answer @ 0= two_options_only_error# and ?wrong  \ Ejecutar error si la respuesta fue irreconocible
  #answer @ dup 0<> and and  \ Calcular el resultado final
  restore_vocabularies
  ;
svariable question
: .question
  \ Imprime la pregunta.
  question_color question count paragraph
  ;
: answer  ( a u -- n )
  \ Devuelve la respuesta a una pregunta del tipo «sí o no».
  \ a u = Pregunta
  \ n = Respuesta: un número negativo para «no» y positivo para «sí»
  question place
  begin
    .question listen  yes|no ?dup
  until
  ;
: yes?  ( a u -- ff )
  \ ¿Es afirmativa la respuesta a una pregunta?
  \ a u = Pregunta
  \ ff = ¿Es la respuesta positiva?
  answer 0>
  ;
: no?  ( a u -- ff )
  \ ¿Es negativa la respuesta a una pregunta?
  \ a u = Pregunta
  \ ff = ¿Es la respuesta negativa?
  answer 0<
  ;

\ }}} ##########################################################
section( Fin)  \ {{{

: success?  ( -- ff )
  \ ¿Ha completado con éxito su misión el protagonista?
  my_location location_51% =
  ;
false [if]  \ No se usa!!!
: battle_phases  ( -- u )
  \ Devuelve el número máximo de fases de la batalla.
  5 random 7 +  \ Número al azar, de 8 a 11
  ;
[then]
: failure?  ( -- ff )
  \ ¿Ha fracasado el protagonista?
  battle# @ battle_phases >
  ;
: .bye
  \ Mensaje final cuando el jugador no quiere jugar otra partida.
  \ Provisional!!!
  s" ¡Adiós!" narrate
  ;
: bye_bye
  \ Abandona el programa.
  new_page .bye bye
  ;
: play_again?$  ( -- a u )
  \ Devuelve la pregunta que se hace al jugador tras haber completado con éxito el juego.
  s{ s" ¿Quieres" s" ¿Te animas a" s" ¿Te apetece" }s
  s{ s" jugar" s" empezar" }s&  again?$ s&
  ;
: retry?1$  ( -- a u )
  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado.
  s" ¿Tienes"
  s{ s" fuerzas" s" arrestos" s" agallas" s" energías" s" ánimos" }s&
  ;
: retry?2$  ( -- a u )
  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado.
  s{ s" ¿Te quedan" s" ¿Guardas" s" ¿Conservas" }s
  s{ s" fuerzas" s" energías" s" ánimos" }s&
  ;
: retry?$  ( -- a u )
  \ Devuelve la pregunta que se hace al jugador tras haber fracasado.
  s{ retry?1$ retry?2$ }s s" para" s&
  s{ s" jugar" s" probar" s" intentarlo" }s& again?$ s&
  ;
: enough?  ( -- ff )
  \ ¿Prefiere el jugador no jugar otra partida?
  success? if  play_again?$  else  retry?$  then  cr no?
  ;
: surrender?  ( -- ff )
  \ ¿Quiere el jugador dejar el juego?
  \ No se usa!!!
  s{
  s" ¿Quieres"
  s" ¿En serio quieres"
  s" ¿De verdad quieres"
  s" ¿Estás segur" player_gender_ending$+ s" de que quieres" s& 
  s" ¿Estás decidid" player_gender_ending$+ s" a" s&
  }s{
  s" dejarlo?"
  s" rendirte?"
  s" abandonar?"
  }s&  yes? 
  ;
: game_over?  ( -- ff )
  \ ¿Se terminó ya el juego?
  success? failure? or
  ;
: the_favorite_says$  ( -- a u )
  s" te" s{ s" indica" s" hace saber" }s&
  s" el valido" s&
  ;
: do_not_disturb$  ( -- a u )
  s" ha" s{
  s" ordenado"
  s" dado órdenes de"
  s" dado" s" la" s?& s" orden de" s&
  }s& s" que" s& s{ s" nadie" s" no se" }s&
  s" lo moleste" s& comma+
  ;
: favorite's_speech$  ( -- a u )
  s" El rey"
  castilian_quotes? @
  if  \ Comillas castellanas
    rquote$ s+ comma+ the_favorite_says$ s& comma+
    lquote$ do_not_disturb$ s+ s&
  else  \ Raya
    dash$ the_favorite_says$ s+ dash$ s+ comma+ s&
    do_not_disturb$ s&
  then
  s" pues sufre una amarga tristeza." s&
  ;
: the_happy_end
  \ Final del juego con éxito.
  s" Agotado, das parte en el castillo de tu llegada"
  s" y de lo que ha pasado." s&
  narrate  narration_break
  s" Pides audiencia al rey, Uther Pendragon."
  narrate  scene_break
  favorite's_speech$ speak  narration_break
  s" No puedes entenderlo. El rey, tu amigo."
  narrate  narration_break
  s" Agotado, decepcionado, apesadumbrado,"
  s" decides ir a dormir a tu casa." s&
  s" Es lo poco que puedes hacer." s&
  narrate  narration_break
  s" Te has ganado un buen descanso."
  narrate
  ;
: enemy_speech
  \ Palabras del general enemigo.
  s{  s" Hoy" s{ s" es" s" parece ser" s" sin duda es" }s&
      s" Sin duda hoy"
      s" No cabe duda de que hoy es"
  }s s{ s" un" s" mi" }s& s" día" s&
  s{ s" de suerte" s" afortunado" }s& s" ..." s+
  s{ s" Bien, bien..." s" Excelente..." }s&
  s{  s" Por el gran Ulfius podremos pedir un" s{ s" buen" s" suculento" }s& s" rescate" s&
      s" Del gran Ulfius podremos" s{ s" sacar" s" obtener" }s& s" una buena ventaja" s&
  }s& period+  speak
  ;
: the_sad_end
  \ Final del juego con fracaso.
  s" Los sajones"
  location_10% am_i_there? if
    \ Inacabado!!!
    comma+
    s" que te han visto entrar," s&
    s" siguen tus pasos y" s&
  then
  \ Inacabado!!! Ampliar, explicar por qué no lo matan
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
  narration_break enemy_speech
  ;
: the_end
  \ Mensaje final del juego.
  success? if  the_happy_end  else  the_sad_end  then
  scene_break 
  ;
:action (do_finish)
  \ Abandonar el juego.
  restore_vocabularies system_color cr .forth quit
  ;action
:action do_finish
  \ Acción de abandonar el juego.
  surrender? if
    \ retry?$  cr no? if  (do_finish)  then
    (do_finish)
  then
  ;action

\ }}} ##########################################################
section( Acerca del programa)  \ {{{

: based_on
  \ Muestra un texto sobre el programa original.
  s" «Asalto y castigo» está basado"
  s" en el programa homónimo escrito en BASIC en 2009 por" s&
  s" Baltasar el Arquero (http://caad.es/baltasarq/)." s&
  paragraph
  ;
: license
  \ Muestra un texto sobre la licencia.
  s" (C) 2011,2012 Marcos Cruz (programandala.net)" paragraph
  s" «Asalto y castigo» es un programa libre;"
  s" puedes distribuirlo y/o modificarlo bajo los términos de" s&
  s" la Licencia Pública General de GNU, tal como está publicada" s&
  s" por la Free Software Foundation (Fundación para los Programas Libres)," s&
  s" bien en su versión 2 o, a tu elección, cualquier versión posterior" s&
  s" (http://gnu.org/licenses/)." s& \ Confirmar!!!
  paragraph
  ;
: program
  \ Muestra el nombre y versión del programa.
  s" «Asalto y castigo» (escrito en Gforth)" paragraph
  s" Versión " version$ s& paragraph
  ;
: about
  \ Muestra información sobre el programa.
  new_page about_color
  program cr license cr based_on
  scene_break
  ;

\ }}} ##########################################################
section( Introducción)  \ {{{

: sun$  ( -- a u )
  s{ s" sol" s" astro rey" }s
  ;
: intro_0
  \ Muestra la introducción al juego (parte 0).
  s{
  s{ s" El " s" La luz del" }s sun$ s&
    s{ s" despunta de entre" s" penetra en" s" atraviesa" s" corta" }s&
  s" Los rayos del" sun$ s&
    s{ s" despuntan de entre" s" penetran en" s" atraviesan" s" cortan" }s&
  }s
  s" la" s& s" densa" s?& s" niebla," s&
  s" haciendo humear los" s& s" pobres" s?& s" tejados de paja." s&
  narrate  narration_break
  ;
: intro_1
  \ Muestra la introducción al juego (parte 1).
  s" Piensas en"
  s{ s" el encargo de"
  s" la" s{ s" tarea" s" misión" }s& s" encomendada por" s&
  s" la orden dada por" s" las órdenes de" }s&
  s{ s" Uther Pendragon" s" , tu rey" s?+ s" tu rey" }s& \ tmp!!!
  s" ..." s+
  narrate  narration_break
  ;
: intro_2
  \ Muestra la introducción al juego (parte 2).
  s{ s" Atacar" s" Arrasar" s" Destruir" }s s" una" s&
  s" aldea" s{ s" tranquila" s" pacífica" }s rnd2swap s& s&
  s" , aunque" s+ s{ s" se trate de una" s" sea una" s" esté" }s&
  s{ s" llena de" s" habitada por" s" repleta de" }s&
  s" sajones, no te" s&{ s" llena" s" colma" }s&
  s" de orgullo." s&
  narrate  narration_break
  ;
: intro_3
  \ Muestra la introducción al juego (parte 3).
  s" Los hombres se" s{ s" ciernen" s" lanzan" s" arrojan" }s&
  s" sobre la aldea, y la" s& s{ s" destruyen." s" arrasan." }s&
  s" No hubo tropas enemigas, ni honor en" s&
  s{ s" la batalla." s" el combate." s" la lucha." }s&
  narrate  scene_break
  ;
: intro_4
  \ Muestra la introducción al juego (parte 4).
  sire,$ s{
  s" el asalto" s" el combate" s" la batalla"
  s" la lucha" s" todo" 
  }s& s" ha" s&{ s" terminado" s" concluido" }s&
  period+ speak
  ;
: needed_orders$  ( -- a u )
  \ Devuelve una variante de «órdenes necesarias».
  s" órdenes" s{ "" s" necesarias" s" pertinentes" }s&
  ;
: intro_5
  \ Muestra la introducción al juego (parte 5).
  s" Lentamente," s{
  s" ordenas"
  s" das la orden de"
  s" das las" needed_orders$ s& s" para" s&
  }s& to_go_back$ s& s" a casa." s&
  narrate  narration_break
  ;
: intro_6
  \ Muestra la introducción al juego (parte 6).
  [false] [if]  \ Primera versión, descartada
    ^officers_forbid_to_steal$ period+
  [else]  \ Segunda versión
    soldiers_steal_spite_of_officers$ period+   
  [then]
  narrate  scene_break
  ;
: intro
  \ Muestra la introducción al juego .
  new_page
  intro_0 intro_1 intro_2 intro_3 intro_4 intro_5 intro_6
  ;

\ }}} ##########################################################
section( Principal)  \ {{{

: init/once
  \ Preparativos que hay que hacer solo una vez, antes de la primera partida.
  restore_vocabularies  init_screen
  ;
: init_parser/game
  \ Preparativos que hay que hacer en el intérprete
  \ de comandos antes de cada partida.
  \ Pendiente!!! Trasladar a su zona.
  erase_last_command_elements
  ;
: init/game
  \ Preparativos que hay que hacer antes de cada partida.
  randomize
  init_parser/game
  init_entities init_plot get_config
  \ Anular esto para depuración!!!:
  \ about cr intro  
  \ location_01% enter_location

  \ Activar esto selectivamente para depuración!!!:
  location_08% enter_location  \ Emboscada 
  \ location_17% enter_location  \ Antes de la cueva oscura
  \ location_28% enter_location  \ Refugiados
  \ location_47% enter_location  \ casa de Ambrosio
  \ snake% is_here
  \ ambrosio% is_here
  \ key% is_hold
  ;
: game
  \ Bucle de la partida.
  begin  plot listen obbey  game_over?  until
  ;
: main
  \ Bucle principal del juego.
  init/once
  begin  init/game game the_end  enough?  until
  bye_bye
  ;

also forth definitions
' main alias go
' main alias run

: i0
  \ Hace toda la inicialización; para depuración!!!.
  init/once init/game
  s" Datos preparados." paragraph
  ;

\ i0 cr  \ Para depuración!!!

\ }}} ##########################################################
section( Pruebas)  \ {{{

(

Esta sección contiene código para probar el programa
sin interactuar con el juego, para detectar mejor posibles
errores.

)

true [if]

: check_stack1
  \ Provoca un error -3 («stack overflow») si la pila no tiene solo un elemento.
  depth 1 <> -3 and throw
  ;
: check_stack
  \ Provoca un error -3 («stack overflow») si la pila no está vacía.
  depth 0<> -3 and throw
  ;
: test_location_description  ( a -- )
  \ Comprueba todas las descripciones de un ente escenario.
  cr ." = Descripción de escenario =======" cr
  dup my_location!
  describe_location check_stack
  cr ." == Mirar al Norte:" cr
  north% describe_direction check_stack
  cr ." == Mirar al Sur:" cr
  south% describe_direction check_stack
  cr ." == Mirar al Este:" cr
  east% describe_direction check_stack
  cr ." == Mirar al Oeste:" cr
  west% describe_direction check_stack
  cr ." == Mirar hacia arriba:" cr
  up% describe_direction check_stack
  cr ." == Mirar hacia abajo:" cr
  down% describe_direction check_stack
\ Aún no implementado:
\ cr ." == Mirar hacia fuera:" cr
\ out% describe_direction check_stack
\ cr ." == Mirar hacia dentro:" cr
\ in% describe_direction check_stack
  ;
0 value tested
: test_description  ( a -- )
  \ Comprueba la descripciones de un ente .
  to tested
  cr ." = Nombre =========================" cr
  tested full_name type
  cr ." = Descripción ====================" cr
  tested describe check_stack
  tested is_location? if  tested test_location_description  then
  ;
: test_descriptions
  \ Comprueba la descripción de todos los entes.
  #entities 0 do
    i #>entity test_description
  loop
  ;
: test_battle_phase  ( u -- )
  \ Comprueba una fase de la batalla
  32 0 do  \ 32 veces cada fase, porque los textos son aleatorios
    dup (battle_phase) check_stack1
  loop  drop
  ;
: test_battle
  \ Comprueba todas las fases de la batalla.
  battle_phases 0 do
    i test_battle_phase
  loop
  ;

[then]

only forth

\eof  \ Ignora el resto del fichero, que se usa para notas

\ }}} ########################################################## 
\ Notas {{{

------------------------------------------------
Términos usados en los comentarios del programa

«ente»
«lugar»/«escenario»
«salida»/«dirección»
«protagonista»
«base de datos»
«ficha»
«campo»/«atributo»

------------------------------------------------
Esbozo de acciones y (seudo)preposiciones

a, al
con, usando...
de


do_attack

atacar
atacar H
atacar O
atacar a H
atacar a H con O

do_break

romper O
romper O1 con O2

do_climb

escalar
escalar O
escalar O1 con O2

do_close:

cerrar
cerrar O
cerrar O1 con O2

do_do:

hacer?

do_drop:

soltar O
soltar O1 con O2

do_examine:

(do_exits):

salidas

do_fear  
do_finish
do_go
do_go_ahead
do_go_back
do_go_down
do_go_east
do_go_in
do_go_north
do_go|do_break
do_go_out
do_go_south
do_go_up
do_go_west
do_hit
do_introduce_yourself
do_inventory
do_kill
do_load_the_game
do_look
do_look_to_direction
do_look_yourself
do_make
do_open
do_put_on
do_save_the_game
do_search
do_sharpen
do_speak
do_swim
do_take
do_take|do_eat 
do_take_off

\ }}} ########################################################## 
\ Tareas pendientes: programación {{{

...........................
2012-03-01:

Error: «No se ve ningunas velas». No es incorrecto, pero queda mejor
poner el verbo en plural en ese caso, con velas como sujeto en lugar de
«se», y «se» como reflexivo.

...........................
2012-02-29:

Ideas para facilitar la depuración de la futura primera versión beta:

* comando GET para apropiarse de cualquier ente, esté donde esté.
* comando GO en el fichero de configuración, para elegir escenario por su número
* mostrar el número de escenario en pantalla

...........................
2012-02-20:

Añadir «hierba» y «hiedra» al escenario
location_47% , pues se citan al abrir la puerta.
Hacer que aparezcan al mencionarlas,
o al examinar la puerta o el suelo.

...........................
2012-02-20:

cambiar "tu benefactor te sigue"
por "tu benefactor te acompaña",
salvo tras movimientos.

...........................
2012-02-20:

Mostrar mensajes completos y variables al final de cada
acción, en lugar de "Hecho".

...........................
2012-02-20:

Hacer una palabra para añadir un texto justificado
a partir de la posición actual del cursor. Esto evita
tener que calcular las cadenas enteras antes de imprimirlas.

Basta una palabra que tome tantos caracteres como espacios libres haya
en la línea actual, los imprima hasta antes del último espacio y para
el resto use la palabra actual.

2012-03-06: Serviría una adaptación de la palabra PRINT provista por la
librería del sistema 4tH.

...........................

2012-02-07:

Hacer x sinónimo de ex cuando no tenga complemento.

...........................
2012-01-03:

Tras el análisis, detectar:

Preposición con artículo (al, del) que no concuerde en
género y número con su ente.

...........................

2011-12:

Hacer que Gforth encuentre ayc.ini en su ruta de búsqueda
de forma trasparente.

...........................

Desambiguar «hombre» para evitar «no se ve a nadie»
al decir «m hombre» en presencia de soldados.

...........................

Implementar tres niveles en mirar:

0 = mirar
1 = examinar
2 = registrar

¿O hacer que sean acciones efectivas separadas?

...........................

2011-12:

Grave: los comandos no vacíos y sin verbo reconocido
hacen saltar el sistema.

2012-01-30: No pasa en Gforth.

...........................

2011-12:

Poner de un color diferente, configurable, el presto y el
texto de las respuestas al sistema (preguntas sí/no).

...........................

2011-12:

Los comandos de configuración no evitan que el análisis dé
error por falta de comandos del juego!

Esto es fácil de arreglar:

¿Hacer que anulen todo lo que siga?
¿O que continúen como si fuera un comando nuevo?
O mejor: simplemente rellenar ACTION con un xt
de una acción que no haga nada!

No! Lo que hay que hacer es ejecutar las acciones de
configuración como el resto de acciones, metiendo su xt en
'action'.  Y si después queremos seguir (dependerá de la
acción de sistema de que se trata) basta poner ACTION a cero
otra vez. O se puede leer el resto del comando, para
anularlo!

...........................

2011-12:

Comprobar si el hecho de no usar el número máximo de líneas
causa problemas con diferentes tamaños de consola.

Los textos son cortos, de modo que no hay riesgo de
que se pierdan antes poder leerlos, antes de que
se pida entrada a un comando.

...........................

2011-12:

Hacer un comando que lea el fichero de
configuración en medio de una partida.

...........................

2011-12:

Evitar mensaje «todos tus hombres siguen tus pasos» en la
aldea, nada más empezar. Usar otra frase mientras dura el
saqueo. Reescribir ese texto bien, entre la intro y la
aldea.

...........................

2011-12:

Implementar transcripción en fichero.

...........................

2011-12:

Anotar que ha habido palabras no reconocidas, para variar el error en
lugar de actuar como si faltaran.  p.e. mirar / mirar xxx.

...........................

2011-12:

Hacer más naturales los mensajes que dicen
que no hay nada de interés en la dirección indicada,
p.e.,
miras hacia...
intentas vislumbrar (en la cueva oscura)...
contemplas el cielo...
miras a tus pies...

...........................

2011-12:

Añadir variante:
«No observas nada digno de mención al mirar hacia el Este».

...........................

2011-12:

Añadir «tocar».

...........................

2011-12:

Implementar que «todo» pueda usarse
con examinar y otros verbos, y se cree una lista
ordenada aleatoriamente de entes que cumplan
los requisitos.

...........................

2011-12:

Hacer que los objetos (y ambrosio) no estén siempre en el
mismo sitio. ¿Altar? ¿Serpiente?

...........................

2011-12:


Hacer algo así en las tramas del laberinto:

(una vez de x se equivoca)

: this_place_seems_familiar
  my_location is_visited?
  if
    s" Este sitio me suena"
  then
  ;

...........................

2011-12:

Respuesta a mirar como en «Pronto»:

Miras, pero no ves eso por aquí. ¿Realmente importa?

...........................

2011-12:

Crear ente «enemigo» con el término ambiguo «sajones» (por
los sajones muertos en la aldea.

...........................

2011-12:

Crear ente (sub)oficiales, con descripción complementaria a
la de los soldados.

...........................

2011-12:

Crear ente «general» para el general enemigo, con
descripción durante la batalla, dependiendo de la fase.

...........................

2011-12:

Implementar «describir», sinónimo de examinar para entes
presentes pero que funciona con entes no presentes ya
conocidos!

...........................

2011-12:

Implementar «esperar» («z»)

...........................

2011-12:

Hacer más robusto el analizador con:

«todo», «algo»

«ahora»:

>coge libro
>ahora la espada
>y ahora la espada
>y la espada
>también la espada
>y también la espada
>y además la espada
>además la espada

nombres sueltos, ¿mirarlos?:

>espada
Es muy bonita.

...........................

2011-12:

Hace que «examinar» sin más examine todo.

¿Y también «coger» y otros?

coger sin objeto buscaría qué hay.
si solo hay una cosa para coger, la coge.
si hay varias, error.


...........................

2011-12:

Error nuevo para no coger las cosas de la casa de Ambrosio:
Es mejor dejar las cosas de Ambrosio donde están.

Añadir a la ficha con su xt.

...........................

2011-12:

Solucionar el eterno problema de los sinónimos que no tienen
el mismo género o número...

La palabra del vocabulario podría ponerse a sí misma como
nombre del ente... Pero esto obligaría a usar el género
y número de la ficha en las descripciones.

Algo relacionado: "arma" es femenina pero usa artículo "el";
contemplar en los cálculos de artículo!!!

Mirar cómo lo solucioné en «La legionela del pisto»: con una
lista de nombres separada de los datos de entes.

...........................

2011-12:

¿Crear un método para dar de alta fácilmente entes
decorativos? Hay muchos en las descripciones de los
escenarios.

...........................

2011-12:

Hacer que no salga el presto de pausa si las pausas son
cero.

...........................

2011-12:

Hacer variantes de CHOOSE y DCHOOSE para elegir un elemento
con un cálculo en lugar de al azar. 

¿En dónde se necesitaba?

...........................

2011-12:

Crear un mensaje de error más elaborado para las acciones
que precisan objeto directo, con el infinitivo como
parámetro: «¿Matar por matar?» «Normalmente hay que matar a
alguien o algo».

...........................

2011-12:

Implementar pronombres. Para empezar, que la forma «mírate»
sea compatible con «mírate la capa». Para esto habría que
distiguir dos variantes de complemento principal, y que al
asignar cualquiera de ellas se compruebe si había ya otro
complemento principal del otro tipo.

...........................

2011-12:

Limitar los usos de 'print_str' a la impresión. Renombrarla.
Crear otra cadena dinámica para los usos genéricos con «+ y
palabras similares.

...........................

2011-12:

Comprobar los usos de 'tmp_str'.

...........................

2011-12:

Poner en fichero de configuración el número de líneas
necesario para mostrar un presto de pausa.

...........................

2011-12:

Implementar opción para tener en cuenta las palabras no
reconocidas y detener el análisis.

...........................

2011-12:

Poner en fichero de configuración si las palabras no
reconocidas deben interrumpir el análisis.

...........................

2011-12:

Poner todos los textos relativos al protagonista en segunda 
persona.

(Creo que ya está hecho).

...........................

2011-12:

Añadir las salidas hacia fuera y dentro. Y atrás. Y
adelante. Y seguir.

...........................

2011-12:

Implementar el recuerdo de la dirección del último
movimiento.

...........................

2011-12:

Hacer que «salir», si no hay dirección de salida en el ente,
calcule la dirección con la del último movimiento.

...........................

2011-12:

Añadir a la configuración si los errores lingüísticos deben
ser detallados (técnicos) o vagos (narrativos) o ambos.

...........................

2011-12:

Hacer que primero se muestre la introducción y después
los créditos y el menú.

...........................

\ }}} ########################################################## 
\ Ideas desestimadas para este proyecto {{{

...........................

Hacer que los nombres de entes imprimidos con los textos
actualicen la lista usada por los pronombres, para que
por ejemplo «mírala» se refiera al último ente citado, tanto
en el comando como en los textos.

Es fácil de hacer. Basta una palabra central que se ocupe de
la impresión de nombres. Para ello, la impresión justificada
de párrafos debe poder hacerse por partes.

...........................
Lista de puzles completados.

...........................

Hacer que el color de fondo y de texto cambie en los
escenarios al aire libre.

...........................

Implementar que en las acciones intermedias automáticas,
como quitarse una prenda antes de dejarla, el mensaje sea
más natural: «Ulfius se quita la corbata y a continuación la
deja».

...........................

Crear tramas de escenario separadas: entrar de él, estar en
él y salir de él.  Hay que distinguir tramas de descripción
de escenario (como la actual, que se activa en la descripción)
de tramas de entrada, salida o permanencia en escenario...
Haría falta un selector similar a SIGHT para seleccionar el
caso adecuado en la palabra LOCATION_ENTER_PLOT

...........................

Hacer que se acepten los movimientos a entes de decoración o
mobiliario, o a otros entes presentes, y que se conserve el
ente junto al que estamos, para ilustrar con ello los
textos en algunos casos.

...........................

Mensajes de error integrados en la narración.

Contraejemplo de La Mansión:
> coge libros
No creo que tenga sentido cargar con eso

Mejor así:
> coge libros
Por un momento tienes la absurda tentación de coger los libros,
pero la descartas.

Y mejor aún si las sucesivas órdenes iguales no tuvieran
respuesta (o diferente respuesta de forma gradual). Para eso
hay que implementar una lista de recuerdo...

\ }}} ########################################################## 
\ Tareas pendientes: trama y puzles {{{

Hace que el líder de los refugiados nos deje pasar si
dejamos el objeto (piedra o espada) allí o se lo damos.

Activar la cueva cuando se examina la pared de roca

Hacer que la espada corte en más pedazos la capa si ha sido
afilada con la piedra. Hacer que su descripción varíe.

Hace que el altar solo aparezca al examinar el puente, y la
piedra al examinar el altar.

Escenario y subtrama bajo el agua.

Distinguir nadar de bucear.

Quitarse la coraza o la capa antes de nadar (ambas son
demasiado pesadas para cruzar el lago con 100% de éxito)

No poder nadar si llevamos algo en las manos aparte de la
espada.

Posibilidad de perder la capa al nadar si no la llevamos
puesta.

\ }}} ########################################################## 
\ Tareas pendientes: código fuente {{{

Unificar los comentarios de palabras que devuelven cadenas de texto:

* Devuelve mensaje de que X...
* Mensaje de que X...
* X...

Terminar de cambiar el formato de nombres de palabras en los textos:

De esto: «La palabra ZX , a veces, se usa como ZX2 .»
A esto: «La palabra 'zx', a veces, se usa como 'zx2'.»

Recortar las líneas para que no sobrepasen los 75 caracteres.

}}}

