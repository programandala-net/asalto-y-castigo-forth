\ #! /usr/bin/spf4

\ #############################################################
cr .( Asalto y castigo )

\ {{{

\ A text adventure in Spanish, written in SP-Forth.
\ Un juego conversacional en castellano, escrito en SP-Forth.

: version$  s" A-20111115"  ;  version$ type cr

\ Copyright (C) 2011 Marcos Cruz (programandala.net)

\ 'Asalto y castigo' (written in SP-Forth) is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.
\ http://gnu.org/licenses/ !!!

\ «Asalto y castigo» (escrito en SP-Forth) es un programa libre;
\ puedes distribuirlo y/o modificarlo bajo los términos de
\ la Licencia Pública General de GNU, tal como está publicada
\ por la Free Software Foundation ('fundación para los programas libres'),
\ bien en su versión 2 o, a tu elección, cualquier versión posterior.
\ (http://gnu.org/licenses/) !!!

\ «Asalto y castigo» (escrito en SP-Forth) está basado
\ en la versión escrita en SuperBASIC por el mismo autor,
\ a su vez basada en el programa original,
\ escrito en Sinclair BASIC, Locomotive BASIC y Blassic,
\ (C) 2009 Baltasar el Arquero (http://caad.es/baltasarq/).

\ Información sobre juegos conversacionales:
\ http://caad.es
\ Información sobre Forth:
\ http://forth.org
\ Desarrollo de SP-Forth:
\ http://spf.sf.net

\ #############################################################
\ Historial de desarrollo

\ Véase:
\ http://programandala.net/es.programa.ayc.forth.historial

\ }}}

\ #############################################################
\ Tareas pendientes

\ {{{

0 [if]  \ ......................................

-------------------------------------------------------------
Programación 
-------------------------------------------------------------

Crear un mensaje de error más laborado para las
acciones que precisan objeto directo, con el infinitivo
como parámetro:
«¿Matar por matar?» «Normalmente hay que matar a alguien o algo».

Usar [ y ] para permitir comandos de configuración dentro
de los comandos del jugador.

Implementar pronombres. Para empezar, que la forma «mírate»
sea compatible con «mírate la capa». Para esto habría que
distiguir dos variantes de complemento directo, y que al
asignar cualquiera de ellas se compruebe si había ya otro
complemento directo del otro tipo.

Limitar los usos de PRINT_STR a la impresión. Renombrarla.
Crear otra cadena dinámica para los usos genéricos con «+ y
palabras similares.

Comprobar los usos de TMP_STR .

Poner en fichero de configuración el número de líneas
necesario para mostrar un presto de pausa.

Poner en fichero de configuración los colores de los textos.

Implementar opción para tener en cuenta las palabras no
reconocidas y detener el análisis.

Poner en fichero de configuración si las palabras no
reconocidas deben interrumpir el análisis.

Poner todos los textos relativos al protagonista en segunda 
persona.

Implementar acciones intermedias automáticas, como quitarse
una prenda antes de dejarla, y que el mensaje sea correcto:
«Ulfius se quita la corbata y a continuación la deja».

Hacer que el color de fondo y de texto cambie en los
escenarios al aire libre.

Añadir las salidas hacia fuera y dentro.

Implementar el recuerdo de la dirección del último
movimiento.

Hacer que «salir», si no hay dirección de salida en el ente,
calcule la dirección con la del último movimiento.

Añadir a la configuración si los errores lingüísticos deben
ser detallados (técnicos) o vagos (narrativos).

-------------------------------------------------------------
Trama y puzles 
-------------------------------------------------------------

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

-------------------------------------------------------------
Código fuente
-------------------------------------------------------------

Homogeneizar el uso de los puntos finales de los comentarios.

Unificar la terminología: localización / lugar /escenario.

[then]  \ ......................................

\ }}}

\ #############################################################
\ Novedades significativas respecto a la versión en SuperBASIC y a las versiones originales

0 [if]  \ ......................................

Todos los objetos citados en las descripciones exiten como
tales (p.e. catre, puente, pared...) y por tanto son
manipulables, pero no se incluyen en la lista de los objetos
presentes.

Objetos globales (p.e. cielo, suelo, techo, pared...)

Las direcciones de salida son objetos virtuales, lo que
permite expresiones como «ve al Norte» o «mira hacia el Sur».

La condición final de la batalla no es un número fijo de
pasos sino aleatorio.

Algunas de las descripciones de los objetos y personajes
varían en función de la trama y la situación. \ Pendiente!!!

[then]  \ ......................................


\ #############################################################
\ Términos usados en los comentarios del programa

0 [if]  \ ......................................

«ente»
«lugar»
«salida»/«dirección»
«protagonista»
«base de datos»
«ficha»
«campo»/«atributo»

[then]  \ ......................................

\ #############################################################
\ Dudas

0 [if]  \ ......................................

¿Cómo saber la dirección de la pila? No hay SP0 .

¿Cómo saber el tamaño de la pantalla? No hay FORM .

¿Cómo borrar la pantalla? No hay PAGE .

¿Cómo saber si estoy en Windows o en Linux?

¿Cómo saber si estoy en SP-Forth o en otro Forth?

[then]  \ ......................................

\ ##############################################################
cr .( Requisitos)

\ {{{

\ -------------------------------------------------------------
\ De la librería de SP-Forth 

\ SP-Forth incluye mucho código fuente opcional
\ tanto de sus propios desarrolladores como
\ contribuciones de usuarios.

REQUIRE CASE-INS lib/ext/caseins.f  \ Para que el sistema no distinga mayúsculas de minúsculas en las palabras de Forth
CASE-INS ON  \ Activarlo
require ansi-file lib/include/ansi-file.f  \ Para que el sistema sea lo más compatible posible con el estándar ANS Forth de 1994
\ require random lib/ext/rnd.f  \ Generador de números aleatorios
\ require key-termios lib/posix/key.f  \ Palabra KEY 
require key-termios key.f  \ Palabra KEY (copia modificada del fichero original debido a un problema con la ruta de una librería)

\ -------------------------------------------------------------
\ De la librería de contribuciones de SP-Forth 

require enum ~nn/lib/enum.f  \ Palabra ENUM para crear cómodamente constantes enumeradas

\ -------------------------------------------------------------
\ De la librería «Forth Foundation Library»
\ http://code.google.com/p/ffl/

\ Esta gran librería, preparada para ser compatible con
\ los sistemas Forth más utilizados,
\ proporciona palabras especializadas muy útiles,
\ con el objetivo de ayudar a crear aplicaciones en Forth.
\ Las palabras están agrupadas temáticamente en módulos independientes.
\ Cada módulo de la librería tiene por nombre una abreviatura de tres letras
\ y sus palabras comienzan por esas mismas tres letras.
\ Por ejemplo: los nombres de las palabras
\ proporcionadas por el módulo «str» empiezan por «str»,
\ como STR-CREATE o STR+COLUMNS o STR.VERSION .

require str.version ffl/str.fs \ Cadenas de texto dinámicas
require trm.version ffl/trm.fs \ Manejador de secuencias de escape de la consola (para cambiar sus características, colores, posición del cursor...)
require chr.version ffl/chr.fs \ Herramientas para caracteres
\ require est.version ffl/est.fs \ Cadenas de texto con secuencias de escape
require dtm.version ffl/dtm.fs \ Tipo de datos para fecha (gregoriana) y hora
require dti.version ffl/dti.fs \ Palabras adicionales para manipular el tipo de datos provisto por el módulo dtm

\ -------------------------------------------------------------
\ De programandala.net

require csb csb2.fs  \ Almacén circular de cadenas, con definición de cadenas y operadores de concatenación
\ Véase: http://programandala.net/es.programa.csb2

\ }}}

\ ##############################################################
\ Meta

\ {{{

false value [debug] immediate  \ Indicador: ¿Modo de depuración global?
false value [debug_do_exits] immediate  \ Indicador: ¿Depurar la acción DO_EXITS ?
false value [debug_catch] immediate  \ Indicador: ¿Depurar CATCH y THROW ?
true value [status] immediate  \ Indicador: ¿Mostrar info de depuración sobre el presto de comandos? 

true constant [true] immediate  \ Para usar en compilación condicional 
false constant [false] immediate  \ Para usar en compilación condicional 
false constant [0] immediate  \ Para usar en comentarios multilínea con [IF] dentro de las definiciones de palabras

S" /COUNTED-STRING" environment?  [if]  [else]  256 chars  [then]
constant /counted_string  \ Longitud máxima de una cadena contada (incluyendo la cuenta)

: .s?  \ Imprime el contenido de la pila si no está vacía
	depth  if  cr ." Pila: " .s key drop  then
	;
: section(  ( "text" -- )  \ Notación para los títulos de sección en el código fuente
	\ Esta palabra permite hacer tareas de depuración mientras se compila el programa;
	\ por ejemplo detectar el origen de descuadres en la pila.
	cr postpone .(  \ El nombre de sección a imprimir terminará por tanto en )
	.s?
	;
: windows?  ( -- f )  \ ¿Está el programa corriendo en Windows?
	[defined] winapi: literal
	;
: gnu/linux?  ( -- f )  \ ¿Está el programa corriendo en GNU/Linux?
	windows? 0=
	;
defer main
: save_ayc  \ Crea un ejecutable con el programa
	\ Aún no se usa!!!
	\ Inacabado!!! Hacer el nombre de fichero semiautemático con la fecha y la hora 
	0 to spf-init?  \ Desactivar la inicialización del sistema
	\ 1 to console?  \ Activar el modo de consola
	['] main to <main>  \ Actualizar la palabra que se ejecutará al arrancar
	s" ayc"  windows?  if  s" .exe" s+  then  \ Nombre del fichero ejecutable
	save  \ Grabar el sistema en el fichero
	;

\ }}}

\ ##############################################################
section( Inicio)

vocabulary ayc_vocabulary  \ Vocabulario en que se crearán las palabras del programa (no se trata del vocabulario del jugador)
: restore_vocabularies  \ Restaura los vocabularios FORTH y AYC_VOCABULARY a su orden habitual
	only also  ayc_vocabulary  definitions
	;
restore_vocabularies

\ ##############################################################
section( Constantes)

\ {{{

\ }}}

\ ##############################################################
section( Palabras genéricas)

\ {{{

: drops  ( x1..xn n -- )  \ Elimina n celdas de la pila
	0  do  drop  loop
	;
: sconstant  ( a1 u "name" -- )  \ Crea una constante de cadena
	create  dup c, s, align
	does>  ( -- a2 u )  count
	;
: svariable  ( "name" -- )  \ Crea una variable de cadena
	create  /counted_string chars allot align
	;
: place  ( a1 u1 a2 -- )  \ Guarda una cadena en una variable de cadena
	2dup c!  char+ swap chars cmove
	; 
: alias ( xt "name" -- )  \ Crea un alias de una palabra
	\ Versión modificada (para hacer su sintaxis más estándar)
	\ de la palabra homónima provista por
	\ la librería de contribuciones de SP-Forth:
	\ ~moleg/lib/util/alias.f
	\ Copyright (C) 2007 mOleg 
	nextword sheader last-cfa @ !
	;
' -- alias field  \ Crear FIELD como alias de -- antes de redefinir -- para otro uso
: ++  ( a -- )  \ Incrementa el contenido de una dirección de memoria
	1 swap +!
	;
: --  ( a -- )  \ Decrementa el contenido de una dirección de memoria
 	-1 swap +!
	;

\ }}}

\ ##############################################################
section( Números aleatorios)

\ {{{

\ Generador de números aleatorios, tomado del código de Gforth,
\ publicado bajo la licencia GPL versión 2 o superior.
\ (http://www.jwdt.com/~paysan/gforth.html)

\ ...................... Comienzo del código tomado de Gforth
\ This code is part of Gforth
\ generates random numbers                             12jan94py
\ Copyright (C) 1995,2000,2003 Free Software Foundation, Inc.
variable seed
0x10450405 constant generator
: rnd  ( -- n )  seed @ generator um* drop 1+ dup seed !  ;
: random  ( n -- 0..n-1 )  rnd um* nip  ;
\ ...................... Fin del código tomado de Gforth

: randomize  \ Reinicia la semilla de generación de números aleatorios
	time&date 2drop 2drop * seed !
	;
: choose  ( x1..xn n -- xn' )  \ Devuelve un elemento de la pila elegido al azar entre los n superiores y borra el resto
	dup >r random pick r> swap >r drops r>
	;
: dchoose  ( d1..dn n -- dn' )  \ Devuelve un elemento doble de la pila elegido al azar entre los n superiores y borra el resto
	dup >r random 2*  ( d1..dn n' -- ) ( r: n )
	dup 1+ pick swap 2+ pick swap  ( d1..dn dn' -- ) ( r: n )
	r> rot rot 2>r  2* drops  2r>
	;
' dchoose alias schoose  \ Alias de DCHOOSE para usar con cadenas de texto (solo por estética)

\ }}}

\ ##############################################################
section( Vectores)

\ {{{

0 [if]  \ ......................................

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

La palabra de SP-Forth para crear vectores es VECT pero la
que usaremos es la estándar en ANS Forth: DEFER . Hacen
exactamente lo mismo y con la misma sintaxis: crean una
palabra que no hace nada, pero cuya dirección de ejecución
podrá ser después cambiada usando la palabra IS de la
siguiente forma:

defer palabrita  \ Crear el vector
: usar_palabrita  \ Palabra que usa PALABRITA y que por tanto necesita que esté ya en el diccionario
	\ La compilación no da error, porque PALABRITA existe en el diccionario, pues ha sido creada por DEFER ,
	\ pero la ejecución posterior no haría nada porque el vector PALABRITA no ha sido actualizado.
	palabrita 
	;
: (palabrita)  \ Definición de lo que tiene que hacer PALABRITA
	." ¡Hola mundo, soy palabrita!"
	;
\ Tomar la dirección de ejecución de (PALABRITA) y ponérsela al vector PALABRITA :
' (palabrita) is palabrita
\ Ahora tanto PALABRITA como USAR_PALABRITA
\ harán lo mismo que (PALABRITA) .

[then]  \ ......................................

defer enter  \ Entrar en un escenario; su código está en la palabra (ENTER)
defer debug_color  \ Color de los mensajes de depuración
defer debug  \ Punto de depuración

\ }}}

\ ##############################################################
section( Variables)

\ {{{

\ Variables de configuración

variable woman_player?  \ Indica si el jugador es una mujer; se usa para cambiar el género gramatical en algunos mensajes.
variable castilian_quotes?  \ Indica si se usan comillas castellanas en las citas, en lugar de raya.
variable location_page?  \ Indica si se borra la pantalla antes de entrar en un escenario o de describirlo.
variable cr?  \ Indica si han de separarse los párrafos con una línea en blanco.
variable ignore_unknown_words?  \ Indica si se han de ignorar las palabras desconocidas (o provocar un error).
variable scene_page?  \ Indica si se borra la pantalla después de la pausa de los cambios de escena.
 
\ Variables de la trama

variable battle#  \ Contador de la evolución de la batalla (si aún no ha empezado, es cero)
variable ambrosio_follows?  \ ¿Ambrosio sigue al protagonista?
variable talked_to_the_leader?  \ ¿El protagonista ha hablado con el líder?
variable hacked_the_log?  \ ¿El protagonista ha afilado el tronco?
variable hold#  \ Contador de cosas llevadas por el protagonista (no usado!!!)

: init_plot_variables  \ Inicializa las variables de la trama
	battle# off
	ambrosio_follows? off
	talked_to_the_leader? off
	hacked_the_log? off
	;

\ }}}

\ ##############################################################
section( Pantalla)

\ {{{

\ -------------------------------------------------------------

79 value max_x  \ Máximo número de columna (80 columnas)
24 value max_y  \ Máximo número de fila (25 filas)

\ No se usa!!!
variable cursor_x  \ Columna actual del cursor
variable cursor_y  \ Fila actual del cursor

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

\ -------------------------------------------------------------
\ Colores

0 [if]  \ ......................................

Notas sobre las pruebas realizadas en Debian
con el módulo trm de Forth Foundation Library:

TRM.HALF-BRIGHT causa subrayado, igual que TRM.UNDERSCORE-ON 
TRM.ITALIC-ON causa vídeo inverso, igual que TRM.REVERSE-ON
TRM.FOREGROUND-WHITE pone un blanco apagado diferente al predeterminado.

Referencia:
http://en.wikipedia.org/wiki/ANSI_escape_code

[then]  \ ......................................

trm.foreground-black-high trm.foreground-black - constant >lighter  \ Diferencia entre los dos niveles de brillo

: color  ( u -- )  1 sgr  ;
: paper  ( u -- )  10 +  color  ;
: pen  ( u -- )  color  ;
: lighter  ( u1 -- u2 )  >lighter +  ;

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

\ -------------------------------------------------------------
\ Colores utilizados

: (debug_color)  \ Pone el color de texto usado en los mensajes de depuración
	red paper  white pen
	;
' (debug_color) is debug_color  \ Asignar el vector utilizado previamente en la sección de depuración
: default_color  \ Pone el color de texto predeterminado en el sistema
	trm.foreground-default
	trm.background-default
	trm.reset
	3 sgr 
	;
: common_paper  \ Pone el color de papel común a todas las modalidades de texto
	black paper
	;
: about_color  \ Pone el color de texto de los créditos
	common_paper  white pen
	;
: error_color  \ Pone el color de texto de los errores
	common_paper  light_red pen  
	;
: command_prompt_color  \ Pone el color de texto del presto de entrada de comandos
	common_paper  cyan pen
	;
: pause_prompt_color  \ Pone el color de texto del presto de pausa de impresión
	common_paper  green pen
	;
: scene_prompt_color  \ Pone el color de texto del presto de fin de escena
	common_paper  green pen
	;
: input_color  \ Pone el color de texto para la entrada de comandos
	common_paper  light_cyan pen
	;
: location_name_color  \ Pone el color de texto del nombre de los escenarios
	green paper  black pen
	;
: location_description_color  \ Pone el color de texto de las descripciones de los entes escenarios
	common_paper  green pen
	;
: description_color  \ Pone el color de texto de las descripciones de los entes que no son escenarios
	common_paper  white pen
	;
: narration_color  \ Pone el color de texto de la narración
	common_paper  white pen
	;
: speak_color  \ Pone el color de texto de los diálogos
	common_paper  brown pen 
	;
: answer_color  \ Pone el color de texto de las preguntas de tipo «sí o no»
	common_paper  white pen
	;

\ -------------------------------------------------------------
\ Demo de colores

: color_bar  ( u -- )  \ Imprime una barra de 64 espacios con el color indicado
	paper cr 64 spaces  black paper space
	;
: color_demo  \ Prueba los colores
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
	default_color cr
	;

\ -------------------------------------------------------------
\ Otros atributos

: bold  \ Activa la negrita
	trm.bold 1 sgr
	;
: underline  ( f -- )  \ Activa o desactiva el subrayado
	if  trm.underscore-on  else  trm.underline-off  then  1 sgr
	;
' underline alias underscore
: inverse  ( f -- )  \ Activa o desactiva la inversión de colores (papel y pluma)
	if  trm.reverse-on  else  trm.reverse-off  then  1 sgr
	;
0 [if]
: blink ( f -- )  \ Activa o desactiva el parpadeo
	\ No funciona!!!
	if  trm.blink-on  else  trm.blink-off  then  1 sgr
	;
[then]
: italic  ( f -- )  \ Activa o desactiva la cursiva
	\ Nota: tiene el mismo efecto que INVERSE .
	if  trm.italic-on  else  trm.italic-off  then  1 sgr
	;

\ -------------------------------------------------------------
\ Cursor 

: cursor!  ( u1 u2 -- )  \ Actualiza las variables del cursor en columna u1 y fila u2
	cursor_y !  cursor_x !
	;
: init_cursor  \ Pone a cero las variables del cursor
	0 dup cursor!
	;
: at-xy  ( u1 u2 -- )  \ Sitúa el cursor en columna u1 y fila u2
	2dup trm+move-cursor cursor!
	;
: home  \ Sitúa el cursor en la esquina superior izquierda
	0 dup at-xy
	;

\ -------------------------------------------------------------
\ Borrado de pantalla

: page  \ Borra la pantalla y sitúa el cursor en su origen
	trm+erase-display home
	;
: clear_screen  \ Restaura el color de tinta y borra la pantalla
	gray pen  page  \ Nota!!!: ¿Por qué esa tinta?
	;
: clear_screen_for_location  \ Restaura el color de tinta y borra la pantalla para cambiar de escenario
	location_page? @  if  clear_screen  then
	;
: init_screen/once  \ Prepara la pantalla la primera vez
	trm+reset init_cursor default_color home
	;

\ }}}

\ ##############################################################
section( Depuración)

\ {{{

: fatal_error  ( f a u -- )  \ Informa de un error y sale del sistema, si el indicador de error es distinto de cero
	\ f = Indicador de error
	\ a u = Mensaje de error
	rot if  ." Error fatal: " type cr bye
	else  2drop 
	then
	;
: wait  \ Hace una pausa
	1000 2 * pause
	;
: .stack  \ Imprime el estado de la pila
	[false] [if]  \ versión antigua!!!
	." Pila" depth
	if  ." :" .s ." ( " depth . ." )"
	else  ."  vacía."
	then
	[else]  \ nueva versión
	depth  if  cr ." Pila descuadrada:" .s cr  then
	[then]
	;
: .csb  \ Imprime el estado del almacén circular de cadenas
	." Espacio para cadenas:" csb ?
	;
: .cursor  \ Imprime las coordenadas del cursor
	." Cursor:" cursor_x ? cursor_y ?
	;
: .system_status  \ Muestra el estado del sistema
	( .csb ) .stack ( .cursor )
	;
: .debug_message  ( a u -- )  \ Imprime el mensaje del punto de chequeo, si no está vacío
	dup  if  cr type cr  else  2drop  then
	;
: debug_pause  \ Pausa tras mostrar la información de depuración
	key drop
	;
: (debug)  ( a u -- )  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema
	debug_color .debug_message .system_status debug_pause
	;
' (debug) is debug


\ }}}
\ ##############################################################
section( Manipulación de textos)

\ {{{

str-create tmp_str  \ Cadena dinámica de texto temporal para usos variados

: str-get-last-char  ( a -- c )  \ Devuelve el último carácter de una cadena dinámica
	dup str-length@ 1- swap str-get-char 
	;
: str-get-last-but-one-char  ( a -- c )  \ Devuelve el penúltimo carácter de una cadena dinámica
	dup str-length@ 2- swap str-get-char 
	;

: ^uppercase  ( a u -- )  \ Convierte en mayúsculas la primera letra de una cadena
	\ No funciona con caracteres UTF-8 de más de un octeto!!!
	if  dup c@ char-uppercase swap c!
	else  drop
	then
	;
: >^uppercase  ( a1 u -- a2 u )  \ Hace una copia de una cadena en el almacén circular y la devuelve con la primera letra en mayúscula
	\ Nota: Se necesita para los casos en que no queremos
	\ modificar la cadena original.
	>csb 2dup ^uppercase
	;
: ?>^uppercase  ( a1 u f -- a1 u | a2 u )  \ Hace una copia de una cadena en el almacén circular y la devuelve con la primera letra en mayúscula, dependiendo del valor de una bandera
	if  >^uppercase  then
	;
: -punctuation  ( a u -- a u )  \ Sustituye por espacios todos los signos de puntuación de una cadena
	2dup bounds  ?do
		i c@ chr-punct?  if  bl i c!  then
	loop
	;
: tmp_str!  ( a u -- )  \ Guarda una cadena en la cadena dinámica TMP_STR
	tmp_str str-set
	;
: tmp_str@  ( -- a u )  \ Devuelve el contenido de cadena dinámica TMP_STR
	tmp_str str-get
	;
: sreplace  ( a1 u1 a2 u2 a3 u3 -- a4 u4 )  \ Sustituye en una cadena todas las apariciones de una subcadena por otra subcadena
	\ a1 u1 = Cadena en la que se realizarán los reemplazos
	\ a2 u2 = Subcadena buscada
	\ a3 u3 = Subcadena sustituta
	\ a4 u4 = Resultado
	2rot tmp_str!  tmp_str str-replace  tmp_str@
	;
: *>verb_ending  ( a u f -- )  \ Cambia por «n» (terminación verbal en plural) los asteriscos de un texto, o los quita.
	\ Se usa para convertir en plural o singular los verbos de una frase.
	\ a u = Expresión.
	\ f = ¿Hay que poner los verbos en plural?
	if  s" n"  else  s" "  then  s" *" sreplace 
	;
: char>string  ( c u -- a u )  \ Crea una cadena repitiendo un carácter
	\ c = Carácter
	\ u = longitud de la cadena
	\ a = dirección de la cadena
	dup 'csb swap 2dup 2>r  rot fill  2r>
	;

: period+  ( a1 u1 -- a2 u2 )  \ Añade un punto final a una cadena
	s" ." s+
	;
: comma+  ( a1 u1 -- a2 u2 )  \ Añade una coma al final de una cadena
	s" ," s+
	;

\ }}}

\ ##############################################################
section( Textos variables) 

\ {{{

: our_hero$  ( -- a u ) \ Devuelve el nombre del protagonista
	\ Antiguo, no se usa!!!
	s" Ulfius"
	s" nuestro héroe"
	s" nuestro hombre" 
	s" nuestro protagonista" 4 schoose
	;
: ^our_hero$  ( -- a u ) \ Devuelve el nombre del protagonista, con la primera letra en mayúscula
	\ Antiguo, no se usa!!!
	our_hero$ >^uppercase
	;
: old_man$  ( -- a u )  \ Devuelve una forma de llamar al personaje del viejo
	s" hombre" s" viejo" s" anciano" 3 schoose
	;
: with_him$  ( -- a u )
	s" "
	s" consigo"
	s" encima" 3 schoose
	;
: with_you$  ( -- a u )
	s" "
	s" contigo"
	s" encima" 3 schoose
	;
: carries$  ( -- a u )
	s" tiene"
	s" lleva"
	s" porta" 3 schoose
	;
: you_carry$  ( -- a u )
	s" tienes"
	s" llevas"
	s" portas" 3 schoose
	;
: ^you_carry$  ( -- a u )
	you_carry$ >^uppercase
	;
: now$  ( -- a u )
	s" "
	s" en este momento"
	s" en estos momentos"
	s" ahora" 4 schoose
	;
: here$  ( -- a u )
	s" "
	s" en este lugar"
	s" aquí" 3 schoose
	;
: now|here$  ( -- a u )
	now$ here$ 2 schoose
	;
: only$  ( -- a u )
	s" solo"
	s" solamente"
	s" únicamente"
	s" " 4 schoose
	;
: ^only$  ( -- a u )
	\ Nota: no se puede calcular este texto a partir de la versión en minúsculas, porque el cambio entre minúsculas y mayúsculas no funciona con caracteres codificados en UTF-8 de más de un octeto.
	s" Solo"
	s" Solamente"
	s" Únicamente"
	s" " 4 schoose
	;
: again$  ( -- a u )
	s" de nuevo"
	s" otra vez"
	s" otra vez más"
	s" una vez más" 4 schoose
	;
: again?$  ( -- a u )
	again$ s" ?" s+
	;
: still$  ( -- a u )
	s" aún" s" todavía" 2 schoose
	;
: even$  ( -- a u )
	\ No se usa!!!
	s" aun" s" incluso" 2 schoose
	;
: toward$  ( -- a u )
	s" hacia" s" " 2 schoose
	;
: to_the$  ( -- a u )
	s" hacia el" s" al" 2 schoose
	;
: possible1$  ( -- a u )  \ Devuelve «posible» o una cadena vacía
	s" posible" s" " 2 schoose
	;
: possible2$  ( -- a u )  \ Devuelve «posibles» o una cadena vacía
	s" posibles" s" " 2 schoose
	;

: player_o/a  ( -- a 1 )  \ Devuelve la terminación «a» u «o» según el sexo del jugador
	[false] [if]
		\ Método 1, «estilo BASIC»:
		woman_player? @  if  s" a"  else  s" o"  then
	[else]
		\ Método 2, sin estructuras condicionales, «estilo Forth»:
		c" oa" woman_player? @ abs + 1+ 1
	[then]
	;
: player_o/a+  ( a1 u1 -- a2 u2 )  \ Añade a una cadena la terminación «a» u «o» según el sexo del jugador
	player_o/a s+
	;

: all_your$  ( -- a u )  \ Devuelve una variante de «Todos tus».
	s" Todos tus" s" Tus" 2 schoose
	;
: soldiers$  ( -- a u )  \ Devuelve una variante de «soldados».
	s" hombres" s" soldados" 2 schoose 
	;
: your_soldiers$  ( -- a u )  \ Devuelve una variante de "tus hombres"
	s" tus" soldiers$ s&
	;
: ^your_soldiers$  ( -- a u )  \ Devuelve una variante de "Tus hombres"
	s" Tus" soldiers$ s&
	;

: the_enemies$  ( -- a u )  \ Devuelve una cadena con una variante de «los enemigos».
	s" los sajones"
	s" las tropas" s" las huestes" 2 schoose
	s" enemigas" s" sajonas" 2 schoose s&
	2 schoose
	;
: the_enemy$  ( -- a u )  \ Devuelve una cadena con una variante de «el enemigo».
	s" el enemigo"
	s" la tropa" s" la hueste" 2 schoose
	s" enemiga" s" sajona" 2 schoose s&
	2 schoose
	;
: (the_enemy/enemies)  ( -- a u f )  \ Devuelve una cadena con una variante de «el/los enemigo/s», y un indicador del número
	\ a u = Cadena con el texto
	\ f = ¿El texto está en plural?
	2 random dup  if  the_enemies$  else  the_enemy$  then  rot
	;
: the_enemy/enemies$  ( -- a u )  \ Devuelve una cadena con una variante de «el/los enemigo/s»
	(the_enemy/enemies) drop
	;
: «de_el»>«del»  ( a1 u1 -- a1 u1 | a2 u2 )  \ Remplaza las apariciones de «de el» en una cadena por «del»
	s" del " s" de el " sreplace
	;
: of_the_enemy/enemies$  ( -- a u )  \ Devuelve una cadena con una variante de «del/de los enemigo/s»
	(the_enemy/enemies) >r
	s" de" 2swap s&
	r> 0=  if  «de_el»>«del»  then
	;
: ^the_enemy/enemies  ( -- a u f )  \ Devuelve una cadena con una variante de «El/Los enemigo/s», y un indicador del número
	\ a u = Cadena con el texto
	\ f = ¿El texto está en plural?
	(the_enemy/enemies) >r  >^uppercase  r>
	;

\ }}}

\ ##############################################################
section( Cadena dinámica multiusos)

\ {{{

str-create print_str  \ Cadena dinámica para almacenar el texto antes de imprimirlo justificado

: «»-clear  \ Vacía la cadena dinámica PRINT_STR
	print_str str-clear
	;
: «»!  ( a u -- )  \ Guarda una cadena en la cadena dinámica PRINT_STR
	print_str str-set
	;
: «»@  ( -- a u )  \ Devuelve el contenido de la cadena dinámica PRINT_STR
	print_str str-get
	;
: «+  ( a u -- )  \ Añade una cadena al principio de la cadena dinámica PRINT_STR
	print_str str-prepend-string
	;
: »+  ( a u -- )  \ Añade una cadena al final de la cadena dinámica PRINT_STR
	print_str str-append-string
	;
: space+?  ( -- f )  \ ¿Se debe añadir un espacio al concatenar una cadena a la cadena dinámico PRINT_STR ?
	\ Inacabada!!!
	print_str str-length@ 0<>
	;
: »&  ( a u -- )  \ Añade una cadena al final de la cadena dinámica TXT, con un espacio de separación
	space+?  if  bl print_str str-append-char  then  »+
	;
: «&  ( a u -- )  \ Añade una cadena al principio de la cadena dinámica TXT, con un espacio de separación
	space+?  if  bl print_str str-prepend-char  then  «+ 
	;

\ }}}

\ ##############################################################
section( Impresión de textos)

\ {{{

variable #lines  \ Número de línea del texto que se imprimirá
variable scroll  \ Indicador de que la impresión no debe parar

\ -------------------------------------------------------------
\ Presto de pausa en la impresión de párrafos

s" ..." sconstant pause_prompt$ 
1 value /pause_prompt  \ Número de líneas de intervalo para mostrar un presto

: pause_prompt_key  \ Espera la pulsación de una tecla y actualiza con ella el estado del desplazamiento
	key  bl =  scroll !
	;
: .pause_prompt$  \ Imprime el presto de pausa
	pause_prompt$ type
	;
: .pause_prompt  \ Imprime el presto de pausa, espera una tecla y borra el presto
	trm+save-cursor
	pause_prompt_color .pause_prompt$ pause_prompt_key
	trm+erase-line  trm+restore-cursor
	;
: (pause_prompt?)  ( u -- f )  \ ¿Se necesita imprimir un presto para la línea actual?
	\ u = Línea actual del párrafo que se está imprimiendo
	\ Se tienen que cumplir dos condiciones:
	dup 1+ #lines @ <>  \ ¿Es distinta de la última?
	swap /pause_prompt mod 0=  and  \ ¿Y el intervalo es correcto?
	;
: pause_prompt?  ( u -- f )  \ ¿Se necesita imprimir un presto para la línea actual?
	\ u = Línea actual del párrafo que se está imprimiendo
	\ Si el valor de SCROLL es «verdadero», se devuelve «falso»; si no, se comprueban las otras condiciones.
	\ ." L#" dup . ." /" #lines @ . \ Depuración!!!
	scroll @  if  drop false  else  (pause_prompt?)  then
	;
: .pause_prompt?  ( u -- )  \ Imprime un presto y espera la pulsación de una tecla, si corresponde a la línea en curso
	\ u = Línea actual del párrafo que se está imprimiendo
	pause_prompt?  if  .pause_prompt  then
	;

\ -------------------------------------------------------------
\ Pausas en la narración

\ Nota!!!
\ KEY? devuelve siempre cero en SP-Forth para Linux
\ porque aún no está desarrollado.
\ Esto impide interrumpir las pausas.

dtm-create deadline  \ Variable para guardar el momento final de las pausas

: no_time_left?  ( -- f )  \ ¿Se acabó el tiempo?
	0 time&date  \ Fecha y hora actuales (más cero para los milisegundos)
	deadline dtm-compare  \ Comparar con el momento final (el resultado puede ser: -1, 0, 1)
	1 =  \ ¿Nos hemos pasado?
	;
: no_key?  ( -- f )  \ ¿No hay una tecla pulsada?
	key? 0=
	;
: wait_for_key_press ( u -- )  \ Espera los segundos indicados, o hasta que se pulse una tecla.
	deadline dtm-init  \ Guardar la fecha y hora actuales como límite...
	s>d deadline dti-seconds+  \ ...y sumarle los segundos indicados
	begin  no_time_left? no_key? or  until
	begin  no_time_left? key? or  until
	;
: short_pause  \ Hace una pausa corta; se usa entre ciertos párrafos.
	\ duración provisional!!!
	1 wait_for_key_press
	;
: long_pause  \ Hace una pausa larga; se usa tras cada escena.
	\ duración provisional!!!
	1 wait_for_key_press
	;
: .scene_prompt$  \ Imprime el presto de fin de escena
	scene_prompt_color .pause_prompt$ 
	;
: end_of_scene  \ Muestra un presto y hace una pausa larga. 
	trm+save-cursor
	.scene_prompt$ long_pause
	trm+erase-line  trm+restore-cursor
	scene_page? @  if  clear_screen  then
	;

\ -------------------------------------------------------------
\ Impresión de párrafos ajustados

variable /indentation  \ Longitud de la indentación de cada párrafo
8 constant max_indentation
: line++  \ Incrementa el número de línea, si se puede
	\ No se usa!!!
	\ Versión para no pasar del máximo:
	\   cursor_y @ 1+ max_y 1- min cursor_y !
	\ Versión circular para pasar del máximo a cero:
	\  cursor_y dup @ 1+ dup max_y < abs * swap !
	\ Versión circular, con puesta a cero de la columna:
	cursor_y @ 1+ dup max_y < abs *  0 swap at-xy
	;
: cr+  \ Hace un salto de línea y actualiza el cursor
	cr cursor_y ++ cursor_x off
	;
: ?cr  ( u -- )  \ Hace un salto de línea si hace falta
	0> cr? @ and  if  cr+  then
	;
: not_at_the_top?  ( -- f )  \ ¿La línea de pantalla donde se imprimirá es la primera?
	cursor_y @ 0>
	;
: indentation+  \ Añade indentación ficticia (con un carácter distinto del espacio) a la cadena dinámica PRINT_STR, si la línea del cursor no es la primera
	not_at_the_top?  if
		[char] X /indentation @ char>string «+
	then
	;
: indentation-  ( a1 u1 -- a2 u2 )  \ Quita a una cadena tantos caracteres por la izquierda como el valor de la indentación
	/indentation @ -  swap /indentation @ +  swap
	;
: indent  \ Mueve el cursor a la posición requerida por la indentación
	/indentation @ ?dup  if  trm+move-cursor-right  then
	;
: indentation>  ( a1 u1 -- a2 u2 ) \ Prepara la indentación de una línea
	[debug] [if] s" Al entrar en INDENTATION>" debug [then]  \ Depuración!!!
	not_at_the_top?  if
		indentation- indent
	then
	[debug] [if] s" Al salir de INDENTATION>" debug [then]  \ Depuración!!!
	;
: .line  ( a u  -- )  \ Imprime una línea de texto y un salto de línea
	[debug] [if] s" En .LINE" debug [then]  \ Depuración!!!
	type cr+
	;
: .lines  ( a1 u1 ... an un n -- )  \ Imprime n líneas de texto
	\ a1 u1 = Última línea de texto
	\ an un = Primera línea de texto
	\ n = Número de líneas de texto en la pila
	dup #lines !  scroll on
	0  ?do  .line  i .pause_prompt?  loop
	;
: (paragraph)  \ Imprime la cadena dinámica PRINT_STR ajustándose al ancho de la pantalla
	indentation+  \ Añade indentación ficticia
	print_str str-get max_x str+columns  \ Divide la cadena dinámica PRINT_STR en tantas líneas como haga falta
	[debug] [if] s" En (PARAGRAPH)" debug [then]  \ Depuración!!!
	>r indentation> r>  \ Prepara la indentación efectiva de la primera línea
	.lines  \ Imprime las líneas
	print_str str-init  \ Vacía la cadena dinámica
	;
: paragraph/ ( a u -- )  \ Imprime una cadena ajustándose al ancho de la pantalla
	print_str str-set (paragraph)
	;
: paragraph  ( a u -- )  \ Imprime una cadena ajustándose al ancho de la pantalla; y una separación posterior si hace falta
	dup >r  paragraph/ r> ?cr
	;
: report  ( a u -- )  \ Imprime una cadena como un informe de error
	error_color paragraph default_color
	;
: narrate  ( a u -- )  \ Imprime una cadena como una narración
	narration_color paragraph default_color
	;

\ -------------------------------------------------------------
\ Impresión de citas de diálogos

s" —" sconstant dash$  \ Raya (código Unicode U+2014)
s" «" sconstant lquote$ \ Comilla castellana de apertura
s" »" sconstant rquote$  \ Comilla castellana de cierre
: str-with-rquote-only?  ( a -- f )  \ ¿Hay en una cadena dinámica una comilla castellana de cierre pero no una de apertura?
	>r rquote$ 0 r@ str-find -1 >  \ ¿Hay una comilla de cierre en la cita?
	lquote$ 0 r> str-find -1 = and  \ ¿Y además falta la comilla de apertura? 
	;
: str-with-period?  ( a -- f )  \ ¿Termina una cadena dinámica con un punto? 
	dup str-get-last-char [char] . =  \ ¿El último carácter es un punto?
	swap str-get-last-but-one-char [char] . <> and  \ ¿Y además el penúltimo no lo es? (para descartar que se trate de puntos suspensivos)
	;
: str-prepend-quote  ( a -- )  \ Añade a una cadena dinámica una comilla castellana de apertura
	lquote$ rot str-prepend-string
	;
: str-append-quote  ( a -- )  \ Añade a una cadena dinámica una comilla castellana de cierre
	rquote$ rot str-append-string
	;
: str-add-quotes  ( a -- )  \ Encierra una cadena dinámica entre comillas castellanas
	dup str-append-quote str-prepend-quote
	;
: str-add-quotes-period  ( a -- ) \ Encierra una cadena dinámica (que termina en punto) entre comillas castellanas
	dup str-pop-char drop  \ Eliminar el último carácter, el punto
	dup str-add-quotes  \ Añadir las comillas
	s" ." rot str-append-string  \ Añadir de nuevo el punto 
	;
: quotes+  ( a1 u1 -- a2 u2 )  \ Añade comillas castellanas a una cita de un diálogo
	tmp_str!  tmp_str str-with-rquote-only?
	if  \ Es una cita con aclaración final
		tmp_str str-prepend-quote  \ Añadir la comilla de apertura
	else  \ Es una cita sin aclaración, o con aclaración en medio
		tmp_str str-with-period?  \ ¿Termina con un punto?
		if  tmp_str str-add-quotes-period  
		else  tmp_str str-add-quotes
		then
	then  tmp_str@
	;
: dash+  ( a1 u1 -- a2 u2 )  \ Añade la raya a una cita de un diálogo
	dash$ 2swap s+ 
	;
: >quote  ( a1 u1 -- a2 u2 )  \ Pone comillas o raya a una cita de un diálogo
	castilian_quotes? @  if  quotes+  else  dash+  then  
	;
: speak  ( a u -- )  \ Imprime una cita de un diálogo
	>quote speak_color paragraph default_color
	;

\ }}}

\ ##############################################################
section( Opciones del juego)

\ Pendiente!!!

\ true value clear_screen_coming?  \ 


\ ##############################################################
section( Entes)

\ {{{

0 [if]  \ ......................................

Denominamos «ente» a cualquier componente del mundo virtual
del juego que está descrito en la base de datos. Un «ente»
puede ser un objeto, manipulable o no; un personaje no
jugador, interactivo o no; un lugar; o el protagonista. 

[then]  \ ......................................

defer protagonist  \ Vector que después se redirigirá a la ficha del protagonista en la base de datos
0 constant no_exit  \ Marcador para salidas sin salida
0 constant limbo \ Marcador para usar como localización de entes inexistentes
defer location_plot  \ Vector que después se redirigirá a la palabra que gestiona la trama de los entes escenarios

: exit?  ( a -- f )  \ ¿Está abierta una dirección de salida de un ente escenario?
	no_exit <>
	;
: no_exit?  ( a -- f )  \ ¿Está cerrada una dirección de salida de un ente escenario?
	no_exit =
	;

\ -------------------------------------------------------------
\ Definición de la ficha (estructura de datos) de un ente

0 [if]  \ ......................................

La palabra provista por SP-Forth para crear campos de
estructuras de datos es -- (dos guiones); para usar este
nombre de palabra para otro uso (decrementar variables), en
este programa se ha creado un alias para la palabra -- con
el nombre de FIELD (que es el que usa Gforth y otros
sistemas).

Le funcionamiento de FIELD es muy sencillo. Toma de la pila
dos valores: el inferior es el desplazamiento en octetos
desde el inicio del «registro» (que en este programa
denominamos «ficha»); el superior es el número de octetos
necesarios para almacenar el campo a crear. Con ellos crea
una palabra nueva (cuyo nombre es tomado del flujo de
entrada, es decir, es la siguiente palabra en la línea) que
será el identificador del campo de datos; esta palabra, al
ser creada, guardará en su propio campo de datos el
desplazamiento del campo de datos desde el inicio de la
ficha de datos, y cuando sea ejecutada lo sumará al número
de la parte superior de la pila, que deberá ser la dirección
en memoria de la ficha.

Aunque hay campos binarios que podrían almacenarse en un
solo bitio, y otros que no precisarían más de un octeto,
optamos por almacenar cada campo en una «celda» (concepto de
ANS Forth, unidad en que se mide el tamaño de cada elemento
de la pila y equivalente a 4 octetos en un sistema Forth de
32 bitios).  El motivo es facilitar el desarrollo.  Cuando
la memoria usada por los campos de datos varía, las
operaciones para leer y modificar los campos varían también
en cada caso.  Para evitar esto se podría escribir muy
fácilmente un conjunto de palabras que actuara como capa
superior para manipular los campos de datos, escondiendo las
interioridades de cómo se guarda efectivamente cada dato en
la ficha (en un bitio, en un octeto, en dos o en cuatro).
No obstante, para simplificar aun más, y dado que no hay
problema de escasez de memoria, hemos optado por usar el
mismo tamaño en todos los campos.  De este modo la
aplicación puede usar con todos los campos las palabras
habituales para leer la memoria y modificarla.

[then] \ .......................................

0 \ Valor inicial de desplazamiento para el primer campo
cell field >desambiguation_xt  \ Dirección de ejecución de la palabra que desambigua e identifica el ente (aún no se usa!!!)
cell field >name_str  \ Dirección de una cadena dinámica que contendrá el nombre del ente
cell field >feminine?  \ Indicador: ¿el género gramatical del nombre es femenino?
cell field >plural?  \ Indicador: ¿el nombre es plural?
cell field >no_article?  \ Indicador: ¿el nombre no debe llevar artículo?
cell field >definite_article?  \ Indicador: ¿el artículo debe ser siempre el artículo definido?
cell field >description_xt  \ Dirección de ejecución de la palabra que describe el ente
cell field >character?  \ Indicador: ¿el ente es un personaje?
cell field >conversations  \ Contador para personajes: número de conversaciones tenidas con el protagonista
cell field >decoration?  \ Indicador: ¿el ente forma parte de la decoración de su lugar?
cell field >global_outside?  \ Indicador ¿el ente es global (común) en los lugares al aire libre?
cell field >global_inside?  \ Indicador ¿el ente es global (común) en los lugares interiores? 
cell field >owned?  \ Indicador: ¿el ente pertenece al protagonista? 
cell field >cloth?  \ Indicador: ¿el ente es una prenda que puede ser llevada como puesta?
cell field >worn?  \ Indicador: ¿el ente, que es una prenda, está puesto? 
cell field >light?  \ Indicador: ¿el ente es una fuente de luz que puede ser encendida?
cell field >lit?  \ Indicador: ¿el ente, que es una fuente de luz que puede ser encendida, está encendido?
cell field >vegetal?  \ Indicador: ¿es vegetal?
cell field >animal?  \ Indicador: ¿es animal? 
cell field >human?  \ Indicador: ¿es humano? 
\ cell field >container?  \ Indicador: ¿es un contenedor? 
cell field >open?  \ Indicador: ¿está abierto? 
cell field >location?  \ Indicador: ¿es un lugar? 
cell field >location  \ Identificador del ente en que está localizado (sea lugar, contenedor, personaje o «limbo»)
cell field >location_plot_xt  \ Dirección de ejecución de la palabra que se ocupa de la trama del lugar 
\ cell field >stamina  \ Energía de los entes vivos
cell field >familiar  \ Contador de familiaridad (cuánto le es conocido el ente al protagonista)
cell field >north_exit  \ Ente de destino hacia el Norte
' >north_exit alias >first_exit
cell field >south_exit  \ Ente de destino hacia el Sur
cell field >east_exit  \ Ente de destino hacia el Este
cell field >west_exit  \ Ente de destino hacia el Oeste
cell field >up_exit  \ Ente de destino hacia arriba
cell field >down_exit  \ Ente de destino hacia abajo
cell field >out_exit  \ Ente de destino hacia fuera
cell field >in_exit  \ Ente de destino hacia dentro
' >in_exit alias >last_exit
cell field >direction  \ Desplazamiento del campo de dirección al que corresponde el ente (solo se usa en los entes que son direcciones)
constant /entity  \ Tamaño de cada ficha

0 >first_exit constant first_exit
0 >last_exit constant last_exit
last_exit cell+ first_exit - constant /exits  \ Espacio en octetos ocupado por los campos de salidas
/exits cell / constant #exits  \ Número de salidas

\ -------------------------------------------------------------
\ Base de datos

0 [if]  \ ......................................

La «base de datos» del juego es tan solo una zona de memoria
dividida en partes iguales, una para cada ficha. El
identificador de ficha es una palabra que al ejecutarse deja
en la pila la dirección de la ficha en la memoria.

No se puede reservar el espacio necesario para las fichas
hasta saber cuántas son (a menos que usáramos una estructura
un poco más sofisticada con fichas separadas pero enlazadas
entre sí, muy habitual también y fácil de crear).  Por ello
la palabra 'ENTITIES (que devuelve la dirección de la base
de datos) se crea como un vector, para asignarle
posteriormente su dirección de ejecución.  Esto permite
crear un nuevo ente fácilmente, sin necesidad de asignar
previamente el número de fichas a una constante.

[then]  \ ......................................

defer 'entities  \ Dirección de los entes; vector que después será redirigido a la palabra real
0 value #entities  \ Contador de entes, que se actualizará según se vayan creando

: #>entity  ( u -- a )  \ Devuelve la dirección de la ficha de un ente a partir de su número ordinal (0...#ENTITIES-1)
	/entity * 'entities +
	;
: entity>#  ( a -- u )  \ Devuelve el número ordinal de un ente (0...#ENTITIES-1) a partir de la dirección de su ficha 
	protagonist - /entity /
	;
: entity:  ( "name" -- ) \ Crea un nuevo identificador de entidad, que devolverá la dirección de su ficha
	create
		#entities ,  \ Guardar la cuenta en el cuerpo de la palabra recién creada
		#entities 1+ to #entities  \ Actualizar el contador
	does>  ( -- a )
		@ #>entity  \ Cuando el identificador se ejecute, devolverá la dirección de su ficha
	;

\ -------------------------------------------------------------
\ Identificadores de entes

0 [if]  \ ......................................

Los identificadores de entes se crean con ENTITY: .  Cuando
se ejecutan devuelven la dirección en memoria de su ficha en
la base de datos, que después puede ser modificada con un
identificador de campo para convertirla en la dirección de
memoria de una campo concreto de la ficha.

Para reconocer mejor los identificadores de entes se usa el
sufijo _e en sus nombres.

[then]  \ ......................................

\ **********************
\ Paso 1 de 6 para crear un nuevo ente:
\ Crear su identificador
\ **********************

\ El ente protagonista debe ser el primero (el orden de los restantes es indiferente):
entity: ulfius_e
' ulfius_e is protagonist  \ Actualizar el vector que apunta al ente protagonista

\ Entes que son personajes: 
entity: ambrosio_e
entity: leader_e

\ Entes que son objetos:
entity: altar_e
entity: arch_e
entity: cloak_e
entity: cuirasse_e
entity: door_e
entity: emerald_e
entity: fallen_away_e
entity: flags_e
entity: flint_e
entity: idol_e
entity: key_e
entity: lake_e
entity: lock_e
entity: log_e
entity: piece_e
entity: rags_e
entity: rocks_e
entity: snake_e
entity: stone_e
entity: sword_e
entity: thread_e
entity: torch_e
entity: waterfall_e
entity: bed_e
entity: candles_e
entity: table_e
entity: bridge_e

\ Entes que son escenarios
\ (en lugar de usar su nombre en el identificador,
\ se conserva el número que tienen en la versión
\ original.
\ Además, para que algunos cálculos tomados del código
\ original en BASIC funcionen, es preciso que los 
\ entes escenarios se creen aquí ordenados por ese número):
entity: location_01_e
entity: location_02_e
entity: location_03_e
entity: location_04_e
entity: location_05_e
entity: location_06_e
entity: location_07_e
entity: location_08_e
entity: location_09_e
entity: location_10_e
entity: location_11_e
entity: location_12_e
entity: location_13_e
entity: location_14_e
entity: location_15_e
entity: location_16_e
entity: location_17_e
entity: location_18_e
entity: location_19_e
entity: location_20_e
entity: location_21_e
entity: location_22_e
entity: location_23_e
entity: location_24_e
entity: location_25_e
entity: location_26_e
entity: location_27_e
entity: location_28_e
entity: location_29_e
entity: location_30_e
entity: location_31_e
entity: location_32_e
entity: location_33_e
entity: location_34_e
entity: location_35_e
entity: location_36_e
entity: location_37_e
entity: location_38_e
entity: location_39_e
entity: location_40_e
entity: location_41_e
entity: location_42_e
entity: location_43_e
entity: location_44_e
entity: location_45_e
entity: location_46_e
entity: location_47_e
entity: location_48_e
entity: location_49_e
entity: location_50_e
entity: location_51_e

\ Entes que son globales:
entity: sky_e
entity: floor_e
entity: ceiling_e
entity: clouds_e
entity: cave_e  \ Inacabado!!!

\ Entes que son virtuales
\ (necesarios para la ejecución de algunos comandos):
entity: inventory_e
entity: exit_e
entity: north_e
entity: south_e
entity: east_e
entity: west_e
entity: up_e
entity: down_e
entity: out_e
entity: in_e

\ Tras crear los identificadores de entes
\ ya conocemos cuántos entes hay
\ (pues la palabra ENTITY: actualiza el contador #ENTITIES )
\ y por tanto podemos reservar espacio para la base de datos:

#entities /entity * constant /entities  \ Espacio necesario para guardar todas las fichas, en octetos
create ('entities) /entities allot  \ Reservar el espacio en el diccionario
' ('entities) is 'entities  \ Actualizar el vector que apunta a dicho espacio

\ -------------------------------------------------------------
\ Interfaz de campos

0 [if]  \ ......................................
	
Las palabras siguientes facilitan la tarea de interactuar
con los campos de las fichas, evitando repetir cálculos,
escondiendo parte de los entresijos de las fichas y haciendo
el código más fácil de modificar y más legible.

Algunas de las palabras que definimos a continuación actúan
de forma análoga a los campos de las fichas de entes:
reciben en la pila el identificador de ente y devuelven en
ella un resultado. La diferencia es que es un resultado
calculado.

Otras actúan como procedimientos para realizar operaciones
frecuentes con los entes.

[then]  \ ......................................

: >living_being?  ( a -- f )  \ ¿El ente es un ser vivo (aunque esté muerto)?
	dup >vegetal? @
	over >animal? @ or
	swap >human? @ or
	;

create 'articles  \ Tabla de artículos
	s" un  " s,
	s" una " s,
	s" unos" s,
	s" unas" s,
	s" el  " s,
	s" la  " s,
	s" los " s,
	s" las " s,
	s" tu  " s,
	s" tu  " s,
	s" tus " s,
	s" tus " s,
4 constant /article  \ Longitud máxima de un artículo en la tabla, con sus espacios finales
1 /article * constant /article_gender_set  \ Separación entre cada grupo según el género (masculino y femenino)
2 /article * constant /article_number_set  \ Separación entre cada grupo según el número (singular y plural)
4 /article * constant /article_type_set  \ Separación entre cada grupo según el tipo (definidos, indefinidos, posesivos)

: on_article_number  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el número gramatical del ente
	>plural? @ abs /article_number_set *
	;
: on_article_gender  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el género gramatical del ente
	>feminine? @ abs /article_gender_set *
	;
: >definite_article  ( a -- 0 | 1 )  \ Calcula el desplazamiento (en número de grupos) para apuntar a los artículos definidos de la tabla, si ente indicado necesita uno
	dup >definite_article? @  \ Si el ente necesita siempre artículo definido
	swap >familiar @ 0<>  or abs  \ O bien si el ente es ya familiar al protagonista
	;
: >possesive_article  ( a -- 0 | 2 )  \ Calcula el desplazamiento (en número de grupos) para apuntar a los artículos posesivos de la tabla, si ente indicado necesita uno
	>owned? @ abs 2 *
	; 
: on_article_type  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el ente requiera un artículo definido, indefinido o posesivo
	dup >definite_article swap >possesive_article	
	max /article_type_set *
	;
: (>article@)  ( a -- a1 u1 )  \ Devuelve el artículo apropiado para un ente
	dup on_article_gender  \ Desplazamiento según el género
	over on_article_number +  \ Sumado al desplazamiento según el número
	swap on_article_type +  \ Sumado al desplazamiento según el tipo
	'articles + /article -trailing
	;
: >article@  ( a -- a1 u1 | a 0 )  \ Devuelve el artículo apropiado para un ente, si lo necesita; en caso contrario devuelve una cadena vacía 
	dup >no_article? @  if  0  else  (>article@)  then
	;
: >negative_article@  ( a -- a1 u1 | a 0 )
	\ Inacabado!!!
	;
: >plural_ending@  ( a -- a u )  \ Devuelve la terminación adecuada del plural para el nombre de un ente
	>plural? @  if  s" s"  else  s" "  then
	;
: >gender_ending@  ( a -- a u )  \ Devuelve la terminación adecuada del género gramatical para el nombre de un ente
	>feminine? @  if  s" a"  else  s" o"  then
	;
: >noun_ending@  ( a -- a1 u1 )  \ Devuelve la terminación adecuada para el nombre de un ente
	dup >gender_ending@ rot >plural_ending@ s+
	;
: >direct_pronoun@  ( a -- a1 u1 )  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»)
	s" l" rot >noun_ending@ s+
	;
: >indirect_pronoun@  ( a -- a1 u1 )  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»)
	s" le" rot >plural_ending@ s+
	;
: noun_ending+  ( a1 u1 a -- a2 u2 )  \ Añade a una cadena la terminación adecuada para el nombre de un ente
	>noun_ending@ s+
	;
: >name!  ( a u a1 -- )  \ Guarda el nombre de un ente
	\ a u = Nombre
	\ a1 = Ente
	>name_str @ str-set
	;
: >names!  ( a u a1 -- )  \ Guarda el nombre de un ente, y lo marca como plural
	\ a u = Nombre
	\ a1 = Ente
	dup >plural? on  >name!
	;
: >location_name!  ( a u a1 -- )  \ Guarda el nombre de un ente y marca el ente como escenario
	\ No se usa!!!
	\ a u = Nombre
	\ a1 = Ente
	dup >location? on  >name!
	;
: >fname!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino
	\ a u = Nombre
	\ a1 = Ente
	dup >feminine? on  >name!
	;
: >fnames!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino y plural
	\ a u = Nombre
	\ a1 = Ente
	dup >plural? on  >fname!
	;
: >location_fname!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino; y marca el ente como escenario
	\ No se usa!!!
	\ a u = Nombre
	\ a1 = Ente
	dup >location? on  >fname!
	;
: >name@  ( a -- a1 u1 )  \ Devuelve el nombre de un ente
	\ a = Ente
	\ a1 u1 = Nombre
	>name_str @ str-get
	;
: >full_name@  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con su artículo
	dup >article@ rot >name@ s& 
	;
: >subjective_name@  ( a -- a1 u1 )  \ Devuelve el nombre subjetivo de un ente, desde el punto de vista del protagonista
	dup >familiar @  if
		>full_name@
	else  drop
		\ Inacabado!!! Falta "ningún/a X", con artículo negativo.
		s" eso"
		s" esa cosa" 2 schoose
	then
	;
: .full_name  ( a -- )  \ Imprime el nombre completo de un ente
	\ No se usa!!!
	>full_name@ paragraph
	;
: where  ( a1 -- a2 )  \ Devuelve el ente que es el lugar de otro
	>location @
	;
: be_there  ( a1 a2 -- )  \ Hace que un ente sea el lugar de otro
	\ a1 = Ente que será la localización de a2
	\ a2 = Ente cuya localización será a1
	>location !
	;
: is_there?  ( a1 a2 -- f )  \ ¿Está un ente localizado en otro?
	\ a1 = Ente que actúa de lugar
	\ a2 = Ente cuya situación se comprueba
	where =
	;
: is_global?  ( a -- f )  \ ¿Es el ente un ente global?
	dup >global_outside? @
	swap >global_inside? @ or
	;
: my_location@  ( -- a )  \ Devuelve el lugar del protagonista
	protagonist where
	;
: my_location!  ( a -- )  \ Mueve el protagonista al ente indicado
	protagonist be_there
	;
: am_i_there?  ( a -- f )  \ ¿Está el protagonista en el lugar indicado?
	\ a = Ente que actúa de lugar
	my_location@ =
	;
: is_outside?  ( a -- f )  \ ¿Es el ente un lugar al aire libre?
	\ Cálculo provisional!!!
	drop 0
	;
: is_inside?  ( a -- f )  \ ¿Es el ente un lugar cerrado, no al aire libre?
	is_outside? 0=
	;
: am_i_outside?  ( -- f )  \ ¿Está el protagonista en un lugar al aire libre?
	my_location@ is_outside?
	;
: am_i_inside?  ( -- f )  \ ¿Está el protagonista en un lugar cerrado, no al aire libre?
	am_i_outside? 0=
	;
: is_hold?  ( a -- f )  \ ¿Es el protagonista la localización de un ente?
	where protagonist =
	;
: be_hold  ( a -- )  \ Hace que el protagonista sea la localización de un ente
	>location protagonist swap !
	;
: is_worn?  ( a -- )  \ ¿El protagonista lleva puesto el ente indicado?
	\ Quizá innecesario!!!
	dup is_hold?  swap >worn? @  and
	;
: is_known?  ( a -- f )  \ ¿El protagonista ya conoce al ente?
	dup >conversations @ 0>  \ ¿Ha hablado ya con él? (si no es un personaje, la comprobación no tendrá efecto)
	swap >familiar @ 0>  or  \ ¿O ya le es familiar?
	;
: is_here?  ( a -- f )  \ ¿Está un ente en el mismo lugar que el protagonista?
	\ El resultado depende de cualquiera de tres condiciones:
	dup where am_i_there?  \ ¿Está efectivamente en el mismo lugar?
	over >global_outside? @ am_i_outside? and or \ ¿O es un «global exterior» y estamos en un lugar exterior?
	swap >global_inside? @ am_i_inside? and or  \ ¿O es un «global interior» y estamos en un lugar interior?
	;
: is_not_here?  ( a -- f )  \ ¿Está un ente en otro lugar que el protagonista?
	is_here? 0=
	; 
: be_here  ( a -- )  \ Hace que un ente esté en el mismo lugar que el protagonista
	>location my_location@ swap !
	;
: is_accessible?  ( a -- f )  \ ¿Es un ente accesible para el protagonista?
	dup is_hold?  swap is_here?  or
	;
: is_not_accessible?  ( a -- f )  \ ¿Un ente no es accesible para el protagonista?
	is_accessible? 0=
	;
: can_be_looked_at?  ( a -- )  \ ¿El ente puede ser mirado?
	dup my_location@ =  \ ¿Es el lugar del protagonista?
	over is_here? or  \ ¿O está en el lugar del protagonista?
	swap is_hold?  or  \ ¿O lo tiene el protagonista?
	;

: more_familiar  ( a -- )  \ Aumenta el grado de familiaridad de un ente con el protagonista
	>familiar ++
	\ Nota: no comprobamos el límite porque en la práctica es inalcanzable (un número de 32 bitios)
	\ Comprobar el límite!!!
	;

: vanish  ( a -- )  \ Hace desaparecer un ente llevándolo al «limbo»
	limbo swap be_there
	;
: vanish_if_hold  ( a -- )  \ Hace desaparecer un ente si su localización es el protagonista
	\ No se usa!!!
	dup is_hold?  if  vanish  else  drop  then
	;
: vanish_all  \ Pone todos los entes en el «limbo»
	#entities 0  do
		i #>entity vanish
	loop
	;

\ -------------------------------------------------------------
\ Nombres

0 [if]  \ ......................................

Los nombres que siguen son los que se usarán en los textos
para nombrar los entes.  

[then]  \ ......................................

\ **********************
\ Paso 2 de 6 para crear un nuevo ente:
\ Crear su nombre (el que se usará al citarlo)
\ **********************

: init_entity_names  \ Guarda en las fichas los nombres de los entes y sus atributos gramaticales

	\ Ente protagonista:
	s" Ulfius" ulfius_e >name!

	\ Entes personajes: 
	s" hombre" ambrosio_e >name!  \ El nombre cambiará a «Ambrosio» durante el juego

	\ Entes objetos:
	s" altar" altar_e >name!
	s" arco" arch_e >name!
	s" puente" bridge_e >name!
	s" capa" cloak_e >fname!
	s" coraza" cuirasse_e >fname!
	s" puerta" door_e >fname!
	s" esmeralda" emerald_e >fname!
	s" derrumbe" fallen_away_e >name!
	s" banderas" flags_e >fnames!
	s" pedernal" flint_e >name!
	s" ídolo" idol_e >name!
	s" llave" key_e >fname!
	s" lago" lake_e >name!
	s" candado" lock_e >name!
	s" tronco" log_e >name!
	s" hombre" leader_e >name!
	s" trozo" piece_e >name!
	s" harapo" rags_e >name!
	s" rocas" rocks_e >fnames!
	s" serpiente" snake_e >fname!
	s" piedra" stone_e >fname!
	s" espada" sword_e >fname!
	s" hilo" thread_e >name!
	s" antorcha" torch_e >fname!
	s" cascada" waterfall_e >fname!
	s" catre" bed_e >name!
	s" velas" candles_e >fnames!
	s" mesa" table_e >fname!

	\ Entes escenarios: 
	s" aldea sajona" location_01_e >fname!
	s" cima de la colina" location_02_e >fname!
	s" camino entre colinas" location_03_e >name!
	s" cruce de caminos" location_04_e >name!
	s" linde del bosque" location_05_e >name!  \ nombre supuesto, confirmar!!!
	s" bosque" location_06_e >name!
	s" paso del Perro" location_07_e >name!
	s" entrada a la cueva" location_08_e >fname!
	s" derrumbe" location_09_e >name!
	s" gruta de entrada" location_10_e >fname!
	s" gran lago" location_11_e >name!
	s" salida del paso secreto" location_12_e >fname!
	s" puente semipodrido" location_13_e >name!
	s" recodo de la cueva" location_14_e >name!
	s" pasaje arenoso" location_15_e >name!
	s" pasaje del agua" location_16_e >name!
	s" estalactitas" location_17_e >fname!
	s" puente de piedra" location_18_e >name!
	s" recodo arenoso del canal" location_19_e >name!
	s" tramo de cueva" 
	2dup location_20_e >name!
	2dup location_21_e >name!
	2dup location_22_e >name!
	2dup location_23_e >name!
	2dup location_24_e >name!
	2dup location_25_e >name!
	2dup location_26_e >name!
	location_27_e >name!
	s" refugio" location_28_e >name!
	s" espiral" location_29_e >fname!
	s" inicio de la espiral" location_30_e >name!
	s" puerta norte" location_31_e >fname!
	s" precipicio" location_32_e >name!
	s" pasaje de salida" location_33_e >name!
	s" pasaje de gravilla" location_34_e >name!
	s" puente sobre el acueducto" location_35_e >name!
	s" remanso" location_36_e >name!
	s" canal de agua" location_37_e >name!
	s" gran cascada" location_38_e >fname!
	s" interior de la cascada" location_39_e >name!
	s" explanada" location_40_e >fname!
	s" ídolo" location_41_e >name!
	s" pasaje estrecho" location_42_e >name!
	s" pasaje de la serpiente" location_43_e >name!
	s" lago interior" location_44_e >name!
	s" cruce de pasajes" location_45_e >name!
	s" hogar de Ambrosio" location_46_e >name!
	s" salida de la cueva" location_47_e >fname!
	s" bosque a la entrada" location_48_e >name!
	s" sendero del bosque" location_49_e >name!
	s" camino norte" location_50_e >name!
	s" Westmorland" location_51_e >fname! location_51_e >no_article? on

	\ Entes globales:
	s" techo" ceiling_e >name!
	s" suelo" floor_e >name!
	s" cielo" sky_e >name!
	s" nubes" clouds_e >fnames!
	s" cueva" cave_e >fname!

	\ Entes virtuales:
	s" Norte" north_e >name!  north_e >definite_article? on
	s" Sur" south_e >name!  south_e >definite_article? on
	s" Este" east_e >name!  east_e >definite_article? on
	s" Oeste" west_e >name!  west_e >definite_article? on
	s" arriba" up_e >name!  up_e >no_article? on
	s" abajo" down_e >name!  down_e >no_article? on
	s" afuera" out_e >name!  out_e >no_article? on
	s" adentro" in_e >name!  in_e >no_article? on

	;

\ -------------------------------------------------------------
\ Atributos

\ **********************
\ Paso 3 de 6 para crear un nuevo ente:
\ Definir sus atributos
\ **********************

: init_entity_attributes  \ Guarda en las fichas los atributos de los entes (salvo los lingüísticos)
	\ Nota: No es necesario poner a cero ningún atributo
	\ (salvo los que puedan haber cambiado durante
	\ una partida y deban estar a cero)
	\ pues todos quedan a cero tras borrar la base de datos,
	\ operación que se hace al inicio del programa.
	ambrosio_e >character? on
	ambrosio_e >human? on
	arch_e >decoration? on
	bridge_e >decoration? on
	cave_e >global_inside? on
	ceiling_e >global_inside? on
	cloak_e >cloth? on
	cloak_e >owned? on
	cloak_e >worn? on
	clouds_e >global_outside? on
	cuirasse_e >cloth? on
	cuirasse_e >owned? on
	cuirasse_e >worn? on
	fallen_away_e >decoration? on
	floor_e >global_inside? on
	floor_e >global_outside? on
	lake_e >decoration? on
	leader_e >character? on
	leader_e >human? on
	rocks_e >decoration? on
	sky_e >global_outside? on
	snake_e >animal? on
	sword_e >owned? on
	torch_e >light? on
	torch_e >lit? off
	;

\ -------------------------------------------------------------
section( Descripciones)

0 [if]  \ ......................................

Para cada ente creamos una una palabra que imprimirá su
descripción.  Esto es mucho más flexible que almacenar un
texto invariable en la ficha del ente: La descripción podrá
variar en función del desarrollo del juego y adaptarse a las
circunstancias, e incluso sustituir en algunos casos al
código que controla la trama del juego.

Así pues, lo que almacenamos en la ficha del ente, en el
campo apuntado por >DESCRIPTION_XT , es la dirección de
ejecución de la palabra que imprime su descripción.

Con este método no son necesarias estructuras de control
para seleccionar la palabra de descripción en cada caso:
Bastará tomar su dirección de ejecución de la ficha del ente
y llamar a EXECUTE (véase más abajo la definición de la
palabra (DESCRIBE) , que es la que hace la tarea).

Las direcciones de ejecución de las palabras que imprimen
las descripciones se guardan también en una tabla. El motivo
es conservarlas para restaurar con ellas las de las fichas
al inicio de cada partida, tras haber puesto a cero la base
de datos.  Esto deja abierta también la posibilidad de
cambiarlas libremente durante el juego, pues recuperarán su
valor predeterminado en la siguiente partida.

[then]  \ ......................................

\ Crear la tabla para guardar las direcciones de ejecución de las descripciones:
create descriptions_xt  \ Tabla
#entities cells dup allot  \ Hacer el espacio necesario
descriptions_xt swap 0 fill  \ Borrar la zona con ceros para reconocer después las descripciones vacantes y sustituirlas por la predeterminada

: :description  ( a -- xt a ) \ Inicia la definición de una descripción de un ente; crea una palabra sin nombre que describirá un ente
	:noname swap
	;
: ;description  ( xt a -- )  \ Termina la definición de una palabra que describe un ente
	\ a = Ente cuya palabra de descripción se ha creado
	\ xt = Dirección de ejecución de la palabra de descripción
	2dup  entity># cell *  descriptions_xt + !  \ Guardar xt en la posición de la tabla DESCRIPTIONS_XT correspondiente al ente
	>description_xt !  \ Guardar xt en la ficha del ente
	postpone ;  \ Terminar la definición de la palabra 
	; immediate
: default_description  \ Descripción predeterminada de los entes para los que no se ha creado una palabra propia de descripción; no hace nada
	;
: (describe)  ( a -- )  \ Imprime la descripción de un ente
	>description_xt @ execute
	;
: .location_name  ( a -- )  \ Imprime el nombre de un ente escenario, como cabecera de su descripción
	>name@ >^uppercase location_name_color paragraph default_color
	;
: (describe_location)  ( a -- )  \ Imprime la descripición de un ente escenario
	location_description_color (describe)
	;
: describe_location  ( a -- )  \ Imprime el nombre y la descripción de un ente escenario, y llama a su trama
	[debug] [if] s" En DESCRIBE_LOCATION" debug [then]  \ Depuración!!!
	clear_screen_for_location
	dup .location_name  dup (describe_location)
	location_plot 
	;
: describe_non-location  ( a -- )  \ Imprime la descripción de un ente que no es un escenario
	description_color (describe)
	;
: describe  ( a -- )  \ Imprime la descripción de un ente
	[debug] [if] s" En DESCRIBE" debug [then]  \ Depuración!!!
	dup >location? @
	if  describe_location  else  describe_non-location  then
	;

\ **********************
\ Paso 4 de 6 para crear un nuevo ente:
\ Crear una palabra de descripción, con la sintaxis específica
\ **********************

\ El ente protagonista:

ulfius_e :description  \ Describe el ente ulfius_e
	\ Provisional!!!
	my_location@ is_outside?
	if   s" De pie al sol,"
	else  s" En las sombras,"
	then  
	s" no pareces muy fuerte, a pesar de tener un metro sesenta y cinco de estatura." s&
	my_location@ is_outside?  if
		s" Después de un invierno benigno, la primavera ha traído tal calor que" s&
		s" te hallas a gusto sin vestidos." s& 
	then  paragraph
	;description

\ Los entes personajes: 

ambrosio_e :description
	ambrosio_e is_known?  if
		s" Ambrosio"
		s" es un hombre de mediana edad, que te mira afable." s&
	else  s" Es de mediana edad y mirada afable."
	then  paragraph
	;description
leader_e :description
	s" Es el jefe de los refugiados."
	paragraph
	;description

\ Los entes objetos:

altar_e :description
	s" Está colocado justo en la mitad del puente."
	idol_e is_known? 0=  if
		s" Debe sostener algo importante." s&
	then
	paragraph
	;description
arch_e :description
	\ Provisional!!!
	s" Un sólido arco de piedra, de una sola pieza."
	paragraph
	;description
bridge_e :description
	\ Provisional!!!
	s" Está semipodrido."
	paragraph
	;description
torch_e :description
	\ Inacabado!!! 
	s" Está apagada."
	paragraph
	;description
flags_e :description
	s" Son las banderas britana y sajona."
	s" Dos dragones rampantes, rojo y blanco respectivamente, enfrentados." s&
	paragraph
	;description
cloak_e :description
	s" Tu capa de general, de fina lana tintada de negro."
	paragraph
	;description
waterfall_e :description
	s" No ves nada por la cortina de agua."
	s" El lago es muy poco profundo." s&
	paragraph
	;description
fallen_away_e :description
	s" Muchas, inalcanzables rocas, apiladas una sobre otra."
	paragraph
	;description
emerald_e :description
	s" Es preciosa."
	paragraph
	;description
sword_e :description
	s" Legado " s" Herencia" 2 schoose
	s" de tu padre," s&
	s" fiel herramienta" s" arma fiel" 2 schoose s&
	s" en" 
	s" mil" s" incontables" s" innumerables" 3 schoose s& s&
	s" batallas." s&
	paragraph
	;description
rags_e :description
	s" Un trozo un poco grande de capa."
	paragraph
	;description
thread_e :description
	\ Mover esto al evento de cortar la capa!!!
	s" Un hilo se ha desprendido al cortar la capa con la espada."
	paragraph
	;description
idol_e :description
	s" El ídolo tiene dos agujeros por ojos."
	paragraph
	;description
lake_e :description
	s" La" s" Un rayo de" 2 schoose
	s" luz entra por un resquicio, y caprichosos reflejos te maravillan." s&
	paragraph
	;description
key_e :description
	s" Grande, de hierro herrumboso."
	paragraph
	;description
flint_e :description
	s" Es dura y afilada." 
	paragraph
	;description
stone_e :description
	s" Recia y pesada, pero no muy grande, de forma piramidal."
	paragraph
	;description
door_e :description
	s" Es muy recia y tiene un gran candado."
	paragraph
	;description
rocks_e :description
	s" Son muchas, aunque parecen ligeras y con huecos entre ellas."
	paragraph
	;description
snake_e :description
	\ Provisional!!! Distinguir si está muerta.
	s" Una serpiente muy maja."
	paragraph
	;description
log_e :description
	s" Es un tronco"
	s" recio," s" resistente," s" fuerte," 3 schoose s&
	s" pero" s&
	s" de liviano peso." s" ligero." 2 schoose s&
	paragraph
	;description
piece_e :description
	s" Es un poco de lo que antes era tu capa."
	paragraph
	;description
lock_e :description
	lock_e >open?
	if  s" Está cerrado."
	else  s" Está abierto."
	then  s" Es muy grande y parece resistente." s&
	paragraph
	;description
bed_e :description
	s" Parece poco confortable."
	;description
candles_e :description
	s" Están muy consumidas."
	;description
table_e :description
	s" Pequeña y de basta madera."
	;description

\ Los entes lugares:

location_01_e :description
	s" No ha quedado nada en pie, ni piedra sobre piedra."
	s" El entorno es desolador." s&
	s" Solo resta volver al Sur, a casa." s&
	paragraph
	;description
location_02_e :description
	s" Sobre la colina, casi sobre la niebla de la aldea sajona arrasada al Norte, a tus pies."
	s" El camino desciende hacia el Oeste." s&
	paragraph
	;description
location_03_e :description
	s" El camino avanza por el valle, desde la parte alta, al Este, a una zona harto boscosa, al Oeste."
	paragraph
	;description
location_04_e :description
	s" Una senda parte al Oeste, a la sierra por el paso del Perro, y otra hacia el Norte, por un frondoso bosque que la rodea."
	paragraph
	;description
location_05_e :description
	s" Desde la linde, al Sur, hacia el Oeste se extiende frondoso el bosque que rodea la sierra. La salida se abre hacia el Sur."
	paragraph
	;description
location_06_e :description
	s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
	s" La senda serpentea entre raíces, de un luminoso Este al Oeste." s&
	paragraph
	;description
location_07_e :description
	s" Abruptamente, del bosque se pasa a un estrecho camino entre altas rocas.
	s" El inquietante desfiladero tuerce de Este a Sur." s&
	paragraph
	;description
location_08_e :description
	s" El paso entre el desfiladero sigue de Norte a Este."
	s" La entrada a una cueva se abre al Sur en la pared de roca." s&
	paragraph
	;description
location_09_e :description
	s" El camino desciende hacia la agreste sierra, al Oeste, desde los verdes valles al Este."
	s" Pero un gran derrumbe bloquea el paso hacia la sierra." s&
	paragraph
	;description
location_10_e :description
	s" El estrecho paso se adentra hacia el Oeste, desde la boca, al Norte. "
	paragraph
	;description
location_11_e :description
	s" Una gran estancia alberga un lago"
	s" de profundas e iridiscentes aguas," s&
	s" debido a la luz exterior." s&
	s" No hay otra salida que el Este." s&
	paragraph
	;description
location_12_e :description
	s" Una gran estancia se abre hacia el Oeste, y se estrecha hasta morir, al Este, en una parte de agua."
	paragraph
	;description
location_13_e :description
	s" La sala se abre en semioscuridad a un puente cubierto de podredumbre sobre el lecho de un canal, de Este a Oeste."
	paragraph
	;description
location_14_e :description
	s" La iridiscente cueva gira de Este a Sur."
	paragraph
	;description
location_15_e :description
	s" La gruta desciende de Norte a Sur sobre un lecho arenoso. Al Este, un agujero del que llega claridad."
	paragraph
	;description
location_16_e :description
	s" Como un acueducto, el agua baja con gran fuerza de Norte a Este, aunque la salida practicable es la del Oeste."
	paragraph
	;description
location_17_e :description
	s" Muchas estalactitas se agrupan encima de tu cabeza, y se abren cual arco de entrada hacia el Este y Sur."
	paragraph
	;description
location_18_e :description
	s" Un arco de piedra se eleva, cual puente sobre la oscuridad, de Este a Oeste."
	s" En su mitad, un altar." s&
	paragraph
	;description
location_19_e :description
	s" La furiosa corriente, de Norte a Este, impide el paso, excepto al Oeste."
	s" Al fondo, se oye un gran estruendo." s&
	paragraph
	;description
location_20_e :description
	s" Un tramo de cueva estrecho"
	s" te permite avanzar hacia el Norte y el Sur;" s&
	s" un pasaje surge al Este." s&
	paragraph
	;description
location_21_e :description
	s" Un tramo de cueva estrecho te permite avanzar de Este a Oeste; un pasaje surge al Sur."
	paragraph
	;description
location_22_e :description
	s" Un tramo de cueva estrecho te permite avanzar de Este a Oeste; un pasaje surge al Sur."
	paragraph
	;description
location_23_e :description
	s" Un tramo de cueva estrecho te permite avanzar de Oeste a Sur."
	paragraph
	;description
location_24_e :description
	s" Un tramo de cueva estrecho te permite avanzar de Este a Norte."
	paragraph
	;description
location_25_e :description
	s" Un tramo de cueva estrecho te permite avanzar de Este a Oeste."
	s" Al Norte y al Sur surgen pasajes." s&
	paragraph
	;description
location_26_e :description
	s" Un tramo de cueva estrecho te permite avanzar de Este a Oeste."
	s" Al Norte surge un pasaje." s&
	paragraph
	;description
location_27_e :description
	s" Un tramo de cueva estrecho te permite avanzar al Oeste."
	s" Al Norte surge un pasaje." s&
	paragraph
	;description
location_28_e :description
	s" Una amplia estancia de Norte a Este, hace de albergue a refugiados:"
	s" hay banderas de ambos bandos." s&
	s" Un hombre anciano te contempla." s&
	s" Los refugiados te rodean." s&
	paragraph
	;description
location_29_e :description
	s" Cual escalera de caracol gigante, desciende a las profundidades,"
	s" dejando a los refugiados al Oeste." s&
	paragraph
	;description
location_30_e :description
	s" Se eleva en la penumbra."
	s" La caverna se estrecha ahora como para una sola persona, hacia el este." s&
	paragraph
	;description
location_31_e :description
	s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
	paragraph
	;description
location_32_e :description
	s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
	s" El soporte de roca gira en forma de «U» de Oeste a Sur." s&
	paragraph
	;description
location_33_e :description
	s" El paso se va haciendo menos estrecho a medida que se avanza hacia el Sur, para entonces comenzar hacia el este."
	paragraph
	;description
location_34_e :description
	\ anchea?!!!
	s" El paso se anchea de Oeste a Norte,"
	s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
	paragraph
	;description
location_35_e :description
	s" Un puente se tiende de Norte a Sur sobre el curso del agua."
	s" Resbaladizas escaleras descienden hacia el Oeste." s&
	paragraph
	;description
location_36_e :description
	s" Una estruendosa corriente baja con el pasaje elevado desde el Oeste, y forma un meandro arenoso."
	s" Unas escaleras suben al Este."
	paragraph
	;description
location_37_e :description
	s" El agua baja del Oeste con renovadas fuerzas,"
	s" dejando un estrecho paso elevado lateral para avanzar a Este o a Oeste." s&
	paragraph
	;description
location_38_e :description
	s" Cae el agua hacia el este, descendiendo con gran fuerza hacia el canal,"
	s" no sin antes embalsarse en un lago poco profundo." s&
	paragraph
	;description
location_39_e :description
	s" Musgoso y rocoso, con la cortina de agua tras de ti,"
	s" el nivel del agua ha crecido un poco en este curioso hueco." s&
	paragraph
	;description
location_40_e :description
	s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
	s" Unos casi imperceptibles escalones conducen al Este." s&
	paragraph
	;description
location_41_e :description
	s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al Sur."
	s" Se puede volver a la explanada al Oeste."
	paragraph
	;description
location_42_e :description
	s" Como un pasillo que corteja el canal de agua, a su lado, baja de Norte a Sur."
	s" Se aprecia un aumento de luz hacia el Sur."
	paragraph
	;description
location_43_e :description
	s" El pasaje sigue de Norte a Sur."
	paragraph
	;description
location_44_e :description
	s" Unas escaleras dan paso a un hermoso lago interior, y siguen hacia el Oeste."
	s" Al Norte, un oscuro y estrecho pasaje sube."
	paragraph
	;description
location_45_e :description
	s" Estrechos pasos permiten ir al Oeste, al Este (menos oscuro), y al Sur, un lugar de gran luminosidad."
	paragraph
	;description
location_46_e :description
	\ Crear estos objetos!!!
	s" Un catre, algunas velas y una mesa es todo lo que tiene Ambrosio."
	paragraph
	;description
location_47_e :description
	s" Por el Oeste, una puerta impide, cuando cerrada, la salida de la cueva."
	s" Se adivina la luz diurna al otro lado." s&
	paragraph
	;description
location_48_e :description
	s" Apenas se puede reconocer la entrada de la cueva, al Este."
	s" El sendero sale del bosque hacia el Oeste." s&
	paragraph
	;description
location_49_e :description
	s" El sendero recorre esta parte del bosque de Este a Oeste."
	paragraph
	;description
location_50_e :description
	s" El camino norte de Westmorland se interna hacia el bosque,"
	s" al Norte (en tu estado no puedes ir), y a Westmorland, al Sur." s&
	paragraph
	;description
location_51_e :description
	s" La villa bulle de actividad con el mercado en el centro de la plaza,"
	s" donde se encuentra el castillo." s&
	paragraph
	;description

\ Los entes globales:

clouds_e :description
	\ Provisional!!!
	s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
	s" en la estación del frío se han alejado por el momento. " s&
	2 random  if  paragraph  else  sky_e describe  then  \ check!!!
	;description

sky_e :description
	\ Provisional!!!
	s" El cielo es un cuenco de color azul, listado en lo alto por nubes"
	s" del tipo cirros, ligeras y trasparentes." s&
	paragraph
	;description

floor_e :description
	\ Provisional!!!
	am_i_outside?  if
		s" El suelo fuera es muy bonito."
	paragraph
	else
		s" El suelo dentro es muy bonito."
	paragraph
	then
	;description

ceiling_e :description
	\ Provisional!!!
	s" El techo es muy bonito."
	paragraph
	;description

cave_e :description
	\ Provisional!!!
	s" La cueva es chachi."
	paragraph
	;description

\ Los entes virtuales:

down_e :description  \ tmp!!!
	am_i_outside?  if  
		s" El suelo exterior es muy bonito." paragraph
	else
		s" El suelo interior es muy bonito." paragraph
	then
	;description

up_e :description
	am_i_outside?  if  sky_e describe
	else  ceiling_e describe
	then
	;description

\ Restauración de las descripciones originales

: init_entity_descriptions  \ Restaura las descripciones originales de los entes
	\ Guardamos en el campo >DESCRIPTION_XT de la ficha
	\ de cada ente la dirección de ejecución de la palabra
	\ que se ocupa de su descripción:
	#entities 0  do
		i cell * descriptions_xt + @  \ Tomar la dirección de ejecución de la descripción
		?dup 0=  if  ['] default_description  then  \ Si es cero, sustituirla por la predeterminada
		i #>entity >description_xt !  \ Guardarla en la ficha
	loop
	;

\ -------------------------------------------------------------
\ Entes de tipo dirección

\ Constantes con el desplazamiento correspondiente a cada campo de dirección:
0 >north_exit constant north_exit
0 >south_exit constant south_exit
0 >east_exit constant east_exit
0 >west_exit constant west_exit
0 >up_exit constant up_exit
0 >down_exit constant down_exit
0 >out_exit constant out_exit
0 >in_exit constant in_exit

: init_direction_entities  \ Prepara los entes de dirección
	\ Los entes de dirección guardan en el campo >DIRECTION
	\ el desplazamiento correspodiente al campo de 
	\ dirección que representan. Esto sirve para
	\ reconocerlos como tales entes de dirección 
	\ (pues todos los valores posibles son diferentes de cero)
	\ y para hacer los cálculos en las acciones de movimiento.
	north_exit north_e >direction !
	south_exit south_e >direction !
	east_exit east_e >direction !
	west_exit west_e >direction !
	up_exit up_e >direction !
	down_exit down_e >direction !
	out_exit out_e >direction !
	in_exit in_e >direction !
	;

\ -------------------------------------------------------------
\ Localización de los entes

\ **********************
\ Paso 5 de 6 para crear un nuevo ente:
\ Fijar su localización inicial
\ **********************

: init_entity_locations  \ Asigna a los entes sus localizaciones

	vanish_all  \ Todos al limbo por defecto

	location_01_e ulfius_e be_there
	location_09_e fallen_away_e be_there
	location_15_e log_e be_there
	location_18_e altar_e be_there
	location_18_e arch_e be_there
	location_13_e bridge_e be_there
	location_18_e stone_e be_there
	location_19_e ambrosio_e be_there
	location_28_e flags_e be_there
	location_28_e leader_e be_there
	location_31_e rocks_e be_there
	location_38_e waterfall_e be_there
	location_39_e emerald_e be_there
	location_41_e idol_e be_there
	location_43_e snake_e be_there
	location_44_e lake_e be_there
	location_46_e bed_e be_there
	location_46_e candles_e be_there
	location_46_e key_e be_there
	location_46_e table_e be_there
	location_47_e door_e be_there
	location_47_e lock_e be_there
	ulfius_e cloak_e be_there
	ulfius_e cuirasse_e be_there
	ulfius_e sword_e be_there

	;

\ -------------------------------------------------------------
\ Preparación de la base de datos

: wipe_entities  \ Borra con ceros todas las fichas
	'entities /entities 0 fill
	;
: init_entity_strings  \ Crea una cadena dinámica para guardar el nombre de cada ente y guarda la dirección de la cadena en la ficha del ente
	#entities 0  do
		str-new i #>entity >name_str !  
	loop
	;

\ }}}

\ ##############################################################
section( Mapa)

\ {{{

0 [if]  \ ......................................

Para crear el mapa hay que hacer dos operaciones con los
entes escenarios: marcarlos como tales, para poder
distinguirlos como escenarios; e indicar a qué otros entes
escenarios conducen sus salidas.

La primera operación se hace guardando un valor buleano
«cierto» en el campo >LOCATION? del ente.  Por ejemplo:

	cave_e >location? on

La segunda operación se hace guardando en los campos de
salida del ente los identificadores de los entes a que cada
salida conduzca.  No hace falta ocuparse de las salidas
impracticables porque ya estarán a cero de forma
predeterminada.  Por ejemplo:	

	path_e cave_e >south_exit !  \ Hacer que la salida sur de CAVE_E conduzca a PATH_E
	cave_e path_e >north_exit !  \ Hacer que la salida norte de PATH_E conduzca a CAVE_E

No obstante, para hacer más fácil este segundo paso, hemos
creado unas palabras que proveen la siguiente sintaxis
(primero origen y después destino en la pila, como es
convención en Forth):

	cave_e path_e s-->  \ Hacer que la salida sur de CAVE_E conduzca a PATH_E (pero sin afectar al sentido contrario)
	path_e cave_e n-->  \ Hacer que la salida norte de PATH_E conduzca a CAVE_E (pero sin afectar al sentido contrario)

O en un solo paso:

	cave_e path_e s<-->  \ Hacer que la salida sur de CAVE_E conduzca a PATH_E (y al contrario: la salida norte de PATH_E conducirá a CAVE_E)

Además, la palabra INIT_LOCATION permite inicializar un
escenario asignando en una sola operación todas sus salidas.

[then]  \ ......................................

\ -------------------------------------------------------------
\ Interfaz para crear conexiones entre los escenarios

: opposite_exit  ( a1 -- a2 )  \ Devuelve la dirección cardinal opuesta a la indicada
	\ Inacabado!!!
	;

: -->  ( a1 a2 u -- )  \ Comunica el ente a1 con el ente a2 mediante la salida indicada por el desplazamiento u
	\ a1 = Ente escenario origen
	\ a2 = Ente escenario destino
	\ u = Desplazamiento del campo de dirección a usar en a1
	rot + !
	;

\ Conexiones unidireccionales

: n-->  ( a1 a2 -- )  \ Comunica la salida norte del ente a1 con el ente a2
	north_exit -->
	;
: s-->  ( a1 a2 -- )  \ Comunica la salida sur del ente a1 con el ente a2
	south_exit -->
	;
: e-->  ( a1 a2 -- )  \ Comunica la salida este del ente a1 con el ente a2
	east_exit -->
	;
: w-->  ( a1 a2 -- )  \ Comunica la salida oeste del ente a1 con el ente a2
	west_exit -->
	;
: u-->  ( a1 a2 -- )  \ Comunica la salida hacia arriba del ente a1 con el ente a2
	up_exit -->
	;
: d-->  ( a1 a2 -- )  \ Comunica la salida hacia abajo del ente a1 con el ente a2
	down_exit -->
	;
: o-->  ( a1 a2 -- )  \ Comunica la salida hacia fuera del ente a1 con el ente a2
	out_exit -->
	;
: i-->  ( a1 a2 -- )  \ Comunica la salida hacia dentro del ente a1 con el ente a2
	in_exit -->
	;

\ Conexiones bidireccionales

: n<-->  ( a1 a2 -- )  \ Comunica la salida norte del ente a1 con el ente a2 (y al contrario)
	2dup n-->  swap s-->
	;
: s<-->  ( a1 a2 -- )  \ Comunica la salida sur del ente a1 con el ente a2 (y al contrario)
	2dup s-->  swap n-->
	;
: e<-->  ( a1 a2 -- )  \ Comunica la salida este del ente a1 con el ente a2 (y al contrario)
	2dup e-->  swap w-->
	;
: w<-->  ( a1 a2 -- )  \ Comunica la salida oeste del ente a1 con el ente a2 (y al contrario)
	2dup w-->  swap e-->
	;
: u<-->  ( a1 a2 -- )  \ Comunica la salida hacia arriba del ente a1 con el ente a2 (y al contrario)
	2dup u-->  swap d-->
	;
: d<-->  ( a1 a2 -- )  \ Comunica la salida hacia abajo del ente a1 con el ente a2 (y al contrario)
	2dup d-->  swap u-->
	;
: o<-->  ( a1 a2 -- )  \ Comunica la salida hacia fuera del ente a1 con el ente a2 (y al contrario)
	2dup o-->  swap i-->
	;
: i<-->  ( a1 a2 -- )  \ Comunica la salida hacia dentro del ente a1 con el ente a2 (y al contrario)
	2dup i-->  swap o-->
	;

\ Múltiples conexiones a la vez

: exits!  ( a1..a6 a0 -- )  \ Asigna todas las salidas de un ente escenario.
	\ a1..a8 = Entes escenario de salida (o cero) en el orden: norte, sur, este, oeste, arriba, abajo, dentro, fuera
	\ a0 = Ente escenario cuyas salidas hay que modificar
	dup >r >out_exit !
	r@ >in_exit !
	r@ >down_exit !
	r@ >up_exit !
	r@ >west_exit !
	r@ >east_exit !
	r@ >south_exit !
	r> >north_exit !
	;
: init_location  ( a1..a8 a0 -- )  \ Marca un ente como escenario y le asigna todas las salidas. 
	\ a1..a8 = Entes escenario de salida (o cero) en el orden: norte, sur, este, oeste, arriba, abajo, dentro, fuera
	\ a0 = Ente escenario cuyas salidas hay que modificar
	dup >location? on  exits!
	;

\ -------------------------------------------------------------
\ Datos

: init_map  \ Prepara el mapa

	0 location_02_e 0 0 0 0 0 0 location_01_e init_location
	location_01_e 0 0 location_03_e 0 0 0 0 location_02_e init_location
	0 0 location_02_e location_04_e 0 0 0 0 location_03_e init_location
	location_05_e 0 location_03_e location_09_e 0 0 0 0 location_04_e init_location
	0 location_04_e 0 location_06_e 0 0 0 0 location_05_e init_location
	0 0 location_05_e location_07_e 0 0 0 0 location_06_e init_location
	0 location_08_e location_06_e 0 0 0 0 0 location_07_e init_location
	location_07_e location_10_e 0 0 0 0 0 0 location_08_e init_location
	0 0 location_04_e 0 0 0 0 0 location_09_e init_location
	location_08_e 0 0 location_11_e 0 0 0 0 location_10_e init_location
	0 0 location_10_e 0 0 0 0 0 location_11_e init_location
	0 0 0 location_13_e 0 0 0 0 location_12_e init_location
	0 0 location_12_e location_14_e 0 0 0 0 location_13_e init_location
	0 location_15_e location_13_e 0 0 0 0 0 location_14_e init_location
	location_14_e location_17_e location_16_e 0 0 0 0 0 location_15_e init_location
	0 0 0 location_15_e 0 0 0 0 location_16_e init_location
	location_15_e location_20_e location_18_e 0 0 0 0 0 location_17_e init_location
	0 0 location_19_e location_17_e 0 0 0 0 location_18_e init_location
	0 0 0 location_18_e 0 0 0 0 location_19_e init_location
	location_17_e location_22_e location_25_e 0 0 0 0 0 location_20_e init_location
	0 location_27_e location_23_e location_20_e 0 0 0 0 location_21_e init_location
	0 location_24_e location_27_e location_22_e 0 0 0 0 location_22_e init_location
	0 location_25_e 0 location_21_e 0 0 0 0 location_23_e init_location
	location_22_e 0 location_26_e 0 0 0 0 0 location_24_e init_location
	location_22_e location_28_e location_23_e location_21_e 0 0 0 0 location_25_e init_location
	location_26_e 0 location_20_e location_27_e 0 0 0 0 location_26_e init_location
	location_27_e 0 0 location_25_e 0 0 0 0 location_27_e init_location
	location_26_e 0 0 0 0 0 0 0 location_28_e init_location
	0 0 0 location_28_e 0 location_30_e 0 0 location_29_e init_location
	0 0 location_31_e 0 location_29_e 0 0 0 location_30_e init_location
	0 0 0 location_30_e 0 0 0 0 location_31_e init_location
	0 location_33_e 0 location_31_e 0 0 0 0 location_32_e init_location
	location_32_e 0 location_34_e 0 0 0 0 0 location_33_e init_location
	location_35_e 0 0 location_33_e 0 0 0 0 location_34_e init_location
	location_40_e location_34_e 0 location_36_e 0 location_36_e 0 0 location_35_e init_location
	0 0 location_35_e location_37_e location_35_e 0 0 0 location_36_e init_location
	0 0 location_36_e location_38_e 0 0 0 0 location_37_e init_location
	0 0 location_37_e location_39_e 0 0 0 0 location_38_e init_location
	0 0 location_38_e 0 0 0 0 0 location_39_e init_location
	0 location_35_e location_41_e 0 0 0 0 0 location_40_e init_location
	0 0 0 location_40_e 0 0 0 0 location_41_e init_location
	location_41_e location_43_e 0 0 0 0 0 0 location_42_e init_location
	location_42_e 0 0 0 0 0 0 0 location_43_e init_location
	location_43_e 0 0 location_45_e 0 0 0 0 location_44_e init_location
	0 location_47_e location_44_e location_46_e 0 0 0 0 location_45_e init_location
	0 0 location_45_e 0 0 0 0 0 location_46_e init_location
	location_45_e 0 0 0 0 0 0 0 location_47_e init_location
	0 0 location_47_e location_49_e 0 0 0 0 location_48_e init_location
	0 0 location_48_e location_50_e 0 0 0 0 location_49_e init_location
	0 location_51_e location_49_e 0 0 0 0 0 location_50_e init_location
	location_50_e 0 0 0 0 0 0 0 location_51_e init_location

	;

\ }}}

\ ##############################################################
section( Listas)

\ {{{

variable #listed  \ Contador de elementos listados, usado en varias acciones
variable #elements  \ Total de los elementos de una lista

: list_separator$  ( u1 u2 -- a u )  \ Devuelve una cadena con el separador adecuado a un elemento de una lista
	\ Versión abandonada!!!
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	\ a u = Cadena devuelta, que podrá ser « y » o «, » o «» (vacía)
	?dup  if
		1+ =  if  s"  y "  else  s" , "  then
	else  0 
	then
	;
: (list_separator)  ( u1 u2 -- )  \ Añade a la cadena dinámica PRINT_STR el separador adecuado («y» o «,») para un elemento de una lista.
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	1+ =  if  s" y" »&  else  s" ," »+  then
	;
: list_separator  ( u1 u2 -- )  \ Añade a la cadena dinámica PRINT_STR el separador adecuado (o ninguno) para un elemento de una lista.
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	?dup  if  (list_separator)  else  drop  then
	;
: can_be_listed?  ( a -- f )  \ ¿El ente puede ser incluido en las listas?
	\ Inacabado!!!
	dup protagonist <>  \ ¿No es el protagonista?
	over >decoration? @ 0=  and  \ ¿Y no es decorativo?
	swap is_global? 0=  and  \ ¿Y no es global?
	;

: /list++  ( u1 a1 a2 -- u1 | u2 )  \ Actualiza un contador si un ente es la localización de otro y puede ser listado
	\ u1 = Contador
	\ a1 = Ente lugar
	\ a2 = Ente cuya localización hay que comprobar
	\ u2 = Contador incrementado
	dup can_be_listed?
	if  where = abs +  else  2drop  then
	;
: /list  ( a -- u )  \ Cuenta el número de entes cuya localización es el ente indicado y pueden ser listados
	\ a = Ente lugar
	\ u = Número de entes localizados en el ente lugar y que pueden ser listados
	0  \ Contador
	#entities 0  do
		over i #>entity /list++
	loop  nip
	;
: (worn)$  ( a -- a1 u1 )  \ Devuelve «(puesto/a/s)», según el género y número del ente indicado
	s" (puest" rot >noun_ending@ s" )" s+ s+
	;
: (worn)&  ( a1 u1 a2 -- a1 u1 | a3 u3 )  \ Añade a una cadena, si es necesario, el indicador de que el ente indicado es una prenda puesta
	\ a1 u1 = Cadena con el nombre del ente
	\ a2 = Ente
	\ a3 u3 = Nombre del ente con, si es necesario, el indicador de que se trata de una prenda puesta
	dup  >worn? @  if  (worn)$ s&  else  drop  then
	;
: (content_list)  ( a -- )  \ Añade a la lista en la cadena dinámica PRINT_STR el separador y el nombre de un ente
	#elements @ #listed @  list_separator
	dup >full_name@ rot (worn)& »&  #listed ++
	;
: about_to_list  ( a -- u )  \ Prepara el inicio de una lista
	\ a = Ente que es el lugar de los entes a incluir en la lista
	\ u = Número de entes que serán listados
	#listed off  /list dup #elements !
	;
: content_list  ( a -- a1 u1 )  \ Devuelve una cadena con una lista de entes cuyo lugar es el ente indicado
	\ a = Ente que actúa como localización.
	\ a1 u1 = Lista de objetos localizados en dicho ente.
	«»-clear
	dup about_to_list if
		#entities 1  do
			dup i #>entity dup can_be_listed?  if
				is_there?  if  i #>entity (content_list)  then
			else  2drop
			then
		loop  s" ." »+
	then  drop  «»@
	;
: .present  \ Lista los entes presentes
	my_location@ content_list dup
	if  s" Ves" s" Puedes ver" 2 schoose 2swap s& narrate
	else  2drop
	then
	;

\ }}}

\ ##############################################################
section( Trama) 

\ {{{

\ -------------------------------------------------------------
\ Herramientas para crear las tramas asociadas a lugares

\ Crear la tabla para guardar las direcciones de ejecución de las tramas de los entes escenarios
create location_plots_xt  \ Tabla
#entities cells dup allot  \ Hacer el espacio necesario
location_plots_xt swap 0 fill  \ Borrar la zona con ceros 

: :location_plot  ( a -- xt a ) \ Crea una palabra sin nombre que manejará la trama de un ente escenario
	:noname swap
	;
: ;location_plot  ( xt a -- )  \ Termina la definición de una palabra que manejará la trama de un ente escenario
	\ a = Ente escenario cuya palabra de trama se ha creado
	\ xt = Dirección de ejecución de la palabra de trama
	2dup  entity># cell *  location_plots_xt + !  \ Guardar xt en la posición de la tabla LOCATION_PLOTS_XT correspondiente al ente
	>location_plot_xt !  \ Guardar xt en la ficha del ente
	postpone ;  \ Terminar la definición de la palabra 
	; immediate
: (location_plot)  ( a -- )  \ Ejecuta la palabra de trama de un ente escenario
	>location_plot_xt @ ?dup  if  execute  then
	;
' (location_plot) is location_plot
: init_location_plots  \ Restaura las tramas originales de los entes escenarios
	#entities 0  do
		i cell * location_plots_xt + @  \ Tomar de la tabla la dirección de ejecución 
		?dup if  i #>entity >location_plot_xt !  then  \ Si no es cero, guardarla en su ficha
	loop
	;

\ -------------------------------------------------------------
\ Herramientas de las tramas asociadas a lugares

: is_the_pass_open?  ( -- f )  \ ¿El paso del desfiladero está abierto por el Norte?
	location_08_e >north_exit @ exit?
	;

: going_home  \ De vuelta a casa
	s" Tus" s" Todos tus" 2 schoose
	soldiers$ s&
	s" siguen tus pasos." s" te siguen." 2 schoose s&
	narrate
	;

\ -------------------------------------------------------------
\ Tramas asociadas a lugares

location_01_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_02_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_03_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_04_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_05_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_06_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_07_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_08_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_09_e :location_plot
	is_the_pass_open?  if  going_home  then
	;location_plot
location_11_e :location_plot
    my_location@ lake_e be_there
	;location_plot
location_16_e :location_plot
	s" En la distancia, por entre los resquicios de las rocas,"
	s" y allende el canal de agua, los sajones tratan de buscar" s&
	s" la salida que encontraste por casualidad." s&
	narrate
	;location_plot
location_28_e :location_plot
	no_exit location_28_e >east_exit !
	;location_plot
location_31_e :location_plot
	location_31_e >north_exit @ exit?  if
		s" Las rocas yacen desmoronadas a lo largo del pasaje."
	else
        s" Las rocas bloquean el camino."
	then  narrate
	;location_plot
location_38_e :location_plot
    my_location@ lake_e be_there
	;location_plot
location_43_e :location_plot
	snake_e is_here?  if
		s" Una serpiente bloquea el paso al Sur."
		narrate
	then
	;location_plot
location_44_e :location_plot
    my_location@ lake_e be_there
	;location_plot

\ -------------------------------------------------------------
\ Trama global

\ Emboscada de los sajones

: ambush?  ( -- f )  \ ¿Ha caído el protagonista en la emboscada?
	my_location@ location_08_e =  \ ¿Está en el escenario 8?
	is_the_pass_open?  and  \ ¿Y además el paso está abierto?
	;
: the_battle_ends  \ Termina la batalla
	battle# off
	;
: the_battle_begins  \ Comienza la batalla
	no_exit location_08_e >north_exit !  \ Cerrar la salida norte
	1 battle# !  \ Comienza la batalla 
	;
: the_ambush_begins  \ Comienza la emboscada
    s" Una partida sajona aparece por el Este."
	s" Para cuando te vuelves al Norte," s&
	s" ya no te queda ninguna duda:" s&
	s" era una"
	s" es una" 2 schoose s&
	s" emboscada."
	s" celada."
	s" encerrona."
	s" trampa." 4 schoose s&
	narrate short_pause
	;
: officers_speach  \ Palabras de los oficiales
	s" Sire, capturando" 
    s" Capturando" 
    s" Si capturan"
    s" Sire, si capturan" 4 schoose
	s" a un general britano" s&
	s" su victoria será" s" doble." s" mayor" 2 schoose s&
	s" ganan doblemente."
	s" ganan por partida doble." 3 schoose s&  speak
	;
: your_officers_talk_to_you  \ Tus oficiales hablan con el protagonista
    s" Tus oficiales te"
	s" conminan a huir."
	s" conminan a ponerte a salvo."
	s" piden que te pongas a salvo."
	s" piden que huyas." 4 schoose s&  narrate
	officers_speach
    s" Sabes que" s" Comprendes que" 2 schoose
	s" es cierto"
	s" llevan razón"
	s" están en lo cierto" 3 schoose s&
	s" , y te duele." s+  narrate
	;
: the_enemy_is_stronger  \ El enemigo es superior
    s" En el"
	s" estrecho" s" angosto" 2 schoose s&  s" paso" s&
	s" es posible" s" no es imposible" 2 schoose s&
	s" resistir," s" defenderse," 2 schoose s&
	s" pero" s&
	s" por desgracia" s" desgraciadamente" 2 schoose s&
	\ s" sus efectivos son muy superiores a los tuyos."  \ Anulado porque «efectivo» en ese sentido es acepción moderna.
	s" los sajones son muy superiores en número"
	s" sus soldados son más numerosos que los tuyos"
	s" sus tropas son más numerosas que las tuyas"
	s" sus hombres son más numerosos que los tuyos"
	s" ellos son muy superiores en número"
	s" ellos son mucho más numerosos" 6 schoose period+ s&
	narrate end_of_scene
	;
: ambush  \ Emboscada
	the_ambush_begins
	the_battle_begins
	the_enemy_is_stronger
	your_officers_talk_to_you
	;

\ Textos usados para la ambientacián de la batalla y la persecución

: pursued  \ Perseguido por los sajones
    s" No sabes cuánto tiempo te queda"
    s" Sabes que no puedes perder tiempo"
    s" Sabes que debes darte prisa"
    s" No hay tiempo que perder"
    s" No tienes tiempo que perder"
    s" No tienes mucho tiempo"
    s" Te queda poco tiempo"
    s" Hay que apresurarse"
    s" Tienes que apresurarte"
    s" El tiempo apremia" 10 schoose s" ..." s+
	narrate
	;
: all_your_men  ( -- a u f )  \ Devuelve una variante de «Todes tus hombres», y un indicador de número
	\ a u = Cadena
	\ f = ¿El texto está en plural?
	2 random dup
	if  all_your$ s" Todos y cada uno de" 2 schoose
	else  s" Hasta el último de"
	then  your_soldiers$ s&  rot
	;
: ?plural_verb  ( a1 u1 f -- a1 u1 | a2 u2 )  \ Pone un verbo en plural si es preciso
	if  s" n" s+  then
	;
: fight/s$  ( f -- a u )  \ Devuelve una cadena con una variante de «lucha/n».
	\ f = ¿El resultado debe estar en plural?
	\ a u = Resultado.
	s" lucha" s" combate" s" pelea" s" se bate" 4 schoose
	rot ?plural_verb
	;
: resist/s$  ( f -- a u )  \ Devuelve una cadena con una variante de «resiste/n».
	\ f = ¿El resultado debe estar en plural?
	\ a u = Resultado.
	s" resiste" s" aguanta" s" contiene" 3 schoose
	rot ?plural_verb
	;
: like_a_heroe$ ( -- a u )  \ Devuelve una cadena con una variante de «como un héroe».
	s" como un"
	s" auténtico" s" " 2 schoose s&
	s" héroe" s" valiente" s" jabato"  3 schoose s&
	;
: like_heroes$ ( -- a u )  \ Devuelve una cadena con una variante de «como héroes».
	s" como"
	s" auténticos" s" " 2 schoose s&
	s" héroes" s" valientes" s" jabatos"  3 schoose s&
	;
: (bravery)$  ( -- a u )  \ Devuelve una cadena con una variante de «con denuedo».
	s" con denuedo"
	s" con bravura"
	s" con coraje"
	s" heroicamente" 
	s" esforzadamente"
	s" valientemente"  6 schoose
	;
: bravery$  ( f -- a u )  \ Devuelve una cadena con una variante de «con denuedo», en singular o plural.
	\ f = ¿El resultado debe estar en plural?
	\ a u = Resultado.
	(bravery)$  rot
	if  like_heroes$  else  like_a_heroe$  then
	2 schoose 
	;
: step_by_step$  ( -- a u )  \ Devuelve una variante de «poco a poco»
	s" por momentos"
	s" palmo a palmo" 
	s" poco a poco" 3 schoose
	;
: field$  ( -- a u )  \ Devuelve «terreno» o «posiciones»
	s" terreno"
	s" posiciones" 2 schoose
	;

\ Fases del combate

: battle_phase_00  \ Combate (fase 00)
	s" A pesar de"
	s" haber sido atacados por sorpresa,"
	s" haber sido sorprendidos,"
	s" la sorpresa inicial,"
	s" la sorpresa,"
	s" lo sorpresivo del ataque,"
	s" lo imprevisto del ataque," 6 schoose s&
	your_soldiers$ s&
	s" responden"
	s" reaccionan" 2 schoose s&
	s" con prontitud"
	s" sin perder un instante"
	s" rápidamente"
	s" como si fueran uno solo" 4 schoose s&
	s" y adoptan una formación defensiva."
	s" y organizan la defensa."
	s" y se preparan para defenderse." 3 schoose s&
	narrate
	;
: battle_phase_01  \ Combate (fase 01)
	all_your_men  dup resist/s$  rot bravery$  s& s&
	s" el ataque inicial"
    s" el empuje inicial"
	s" la acometida inicial"
	s" el primer ataque"
	s" el primer empuje" 
	s" la primera acometida" 6 schoose s&
	of_the_enemy/enemies$ s&  period+ narrate
	;
: battle_phase_02  \ Combate (fase 02)
	all_your_men  dup fight/s$  rot bravery$  s& s&
    s" contra" s&  the_enemy/enemies$ s&  period+
	narrate
	;
: battle_phase_03  \ Combate (fase 03)
	\ Inacabado!!!
	^your_soldiers$
	s" empiezan a acusar" s&
	s" visiblemente" s" notoriamente" s" " 3 schoose s&
	s" el" s&
	s" titánico"
	s" enorme" 2 schoose s&
	s" esfuerzo." s&
	narrate
	;
: battle_phase_04  \ Combate (fase 04)
	^the_enemy/enemies
	s" parece que empieza* a" rot *>verb_ending s&
	s" dominar" 
	s" controlar" 2 schoose s&
	s" el campo"
	s" el combate" 
	s" la situación" 3 schoose s&  period+
	narrate
	;
: battle_phase_05  \ Combate (fase 05)
	\ Inacabado!!!
	^the_enemy/enemies
	s" está* haciendo retroceder a" your_soldiers$ s&
	s" está* obligando a" your_soldiers$ s& s" a retroceder" s&
	2 schoose rot *>verb_ending s&
	step_by_step$ s&  period+
	narrate
	;
: battle_phase_06  \ Combate (fase 06)
	\ Inacabado!!!
	^the_enemy/enemies
	s" va* ganando" field$ s&
	s" va* adueñándose del terreno"
	s" va* conquistando" field$ s&
	s" se va* abriendo paso" 4 schoose  rot *>verb_ending s&
	step_by_step$ s&  period+
	narrate
	;
: battle_phase_07  \ Combate (fase 07)
	^your_soldiers$
	s" caen"
	s" van cayendo," 2 schoose s&
	s" " s" uno tras otro," 2 schoose s&
	s" vendiendo cara su vida"
	s" defendiéndose" 2 schoose s&
	like_heroes$ s& period+
	narrate
	;
: battle_phase_08  \ Combate (fase 08)
	^the_enemy/enemies
	s" aplasta* a"
	s" acaba* con" 2 schoose rot *>verb_ending s&
	s" los últimos de"
	s" entre" s" " 2 schoose s& s&
	your_soldiers$ s& s" que," s&
	s" heridos y agotados,"
	s" apurando sus últimas fuerzas,"
	s" haciendo un último esfuerzo," 3 schoose s&
	still$ s&
	s" se mantienen en pie"
	s" pueden mantenerse en pie"
	s" combaten"
	s" ofrecen resistencia" 
	s" pueden ofrecer resistencia" 5 schoose s& period+
	narrate
	;
create 'battle_phases  \ Tabla para las fases del combate
here \ Dirección libre actual, para calcular después las fases
	\ Guardar la dirección de ejecución de cada fase: 
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
: battle_phase  \ Ejecuta la fase en curso del combate
	battle# @ 1- cells 'battle_phases + @ execute  
	;
: pursue_location?  ( -- f )  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?
	my_location@ location_12_e <
	;
: battle_location?  ( -- f )  \ ¿En el escenario de la batalla?
	my_location@ location_10_e <  \ ¿Está el protagonista en un escenario menor que el 10?
	is_the_pass_open? 0=  and  \ ¿Y el paso del desfiladero está cerrado?
	;
: battle_phase++  \ Incrementar la fase de la batalla (salvo una de cada diez veces, al azar)
	10 random  if  battle# ++  then
	;
: battle  \ Batalla y persecución
	battle_location?  if  battle_phase  then
	pursue_location?  if  pursued  then
	battle_phase++
	;
: battle?  ( -- f)  \ ¿Ha empezado la batalla?
	battle# @ 0>
	;

\ Zona oscura de la cueva

: dark_cave?  ( -- f )  \ ¿Entrar en la zona oscura de cueva y sin luz?
	torch_e is_not_accessible?
	torch_e >lit? @ 0=  or
	my_location@ location_20_e =  and
	;
: dark_cave  \ En la cueva y sin luz
	clear_screen
    s" Ante la reinante"
	s" e intimidante"
	s" e impenetrable" 
	s" y sobrecogedora" 3 schoose s&
	s" oscuridad," s&
	s" retrocedes"
	s" " s" unos pasos" s" sobre tus pasos" 3 schoose s& s&
	s" hasta donde puedes ver." s&
    narrate  end_of_scene
    location_17_e enter
	;

\ Gestor de la trama global

: plot  \ Trama global
	\ Nota: Las subtramas deben comprobarse en orden cronológico:
	ambush?  if  ambush exit  then
	battle?  if  battle exit  then
	dark_cave?  if  dark_cave  then
	;

\ -------------------------------------------------------------
\ Comprobaciones de finalización

: success?  ( -- f )  \ ¿Ha completado con éxito su misión el protagonista?
	my_location@ location_51_e =
	;
false [if]
: battle_phases  ( -- u )  \ Devuelve el número máximo de fases de la batalla
	5 random 7 +  \ Número al azar, de 8 a 11
	;
[then]
: failure?  ( -- f )  \ ¿Ha fracasado el protagonista?
	battle# @ battle_phases >
	;

\ }}}

\ ##############################################################
section( Tratamiento de errores del analizador)

\ {{{

\ -------------------------------------------------------------
\ Códigos de error

0 [if]  \ ......................................

En el estándar ANS Forth los códigos de error de -1 a -255
están reservados para el propio estándar; el resto de
números negativos son para que los asigne cada sistema Forth
a sus propios mensajes de error; del 1 en adelante pueden
usarlos libremente cada programa.

[then]  \ ......................................

[false] [if]
\ Sistema antiguo!!!
0
enum no_error_id  \ Ningún error
enum 2-action_error_id  \ Hay dos verbos diferentes
enum 2-complement_error_id  \ Hay dos complementos diferentes
enum no_verb_error_id  \ No hay verbo
enum no_direct_complement_error_id  \ No hay complemento
drop
[then]

\ -------------------------------------------------------------
\ Manejo de errores del analizador

: please$  ( -- a u )  \ Devuelve una cadena con «por favor» o vacía
	s" " s" por favor" 2 schoose
	;
: (please&)  ( a1 u1 a2 u2 -- a3 u3 )  \ Añade una cadena al inicio o al final de otra, con una coma de separación
	\ a1 u1 = Cadena principal
	\ a2 u2 = Cadena que se añadirá a la principal
	2 random  if  2swap  then
	comma+ 2swap s&
	;
: please&  ( a1 u1 -- a1 u1 | a2 u2 )  \ Añade «por favor» al inicio o al final de una cadena, con una coma de separación; o la deja sin tocar
	please$ dup  if  (please&)  else  2drop  then
	;
: in_the_sentence$  ( -- a u )  \ Devuelve una cadena con una variante de «en la frase» (o una cadena vacía)
	s" " s" en la frase" s" en el comando" s" en el texto"
	4 schoose
	;
: error_comment_0$  ( -- a u )  \ Devuelve la variante 0 del mensaje de acompañamiento para los errores lingüísticos
	s" sé más clar" player_o/a+
	;
: error_comment_1$  ( -- a u )  \ Devuelve la variante 1 del mensaje de acompañamiento para los errores lingüísticos
	s" exprésate" s" escribe" 2 schoose
	s" más claramente"
	s" más sencillamente"
	s" con más" s" con mayor" 2 schoose
	s" sencillez" s" claridad" 2 schoose s&
	3 schoose s&
	;
: error_comment_2$  ( -- a u )  \ Devuelve la variante 2 del mensaje de acompañamiento para los errores lingüísticos
	\ Comienzo común:
	s" intenta" s" procura" s" prueba a" 3 schoose
	s" reescribir"
	s" expresar"
	s" escribir"
	s" decir" 4 schoose s&
	s"  la frase" s" lo" s"  la idea" 3 schoose s+
	\ Final 0:
	s" de" s" otra" s" " 2 schoose s&
	s" forma" s" manera" 2 schoose s&
	s" un poco" s" algo" s" " 3 schoose s& s" más" s&
	s" simple" s" sencilla" s" clara" 3 schoose s&
	\ Final 1:
	s" más claramente" 
	s" con más sencillez" 2 schoose
	\ Elegir un final:
	2 schoose s&
	;
: error_comment$  ( -- a u )  \ Devuelve una cadena con un mensaje de acompañamiento para los errores lingüísticos
	error_comment_0$ error_comment_1$ error_comment_2$
	3 schoose please&
	;
: ^error_comment$  ( -- a u )  \ Devuelve una cadena con un mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula
	error_comment$ >^uppercase
	;
: language_error  ( a u -- )  \ Combina un mensaje de error con un comentario común e informa de él
	\ Pendiente!!! Hacer que use coma o punto y coma, al azar
	in_the_sentence$ s&  3 random
	if  >^uppercase period+ ^error_comment$
	else  ^error_comment$ comma+ 2swap
	then  period+ s&  report
	;
: there_are$  ( -- a u )  \ Devuelve una variante de «hay» para sujeto plural, comienzo de varios errores
	s" parece haber"
	s" se identifican" 
	s" se reconocen" 3 schoose
	;
: there_is$  ( -- a u )  \ Devuelve una variante de «hay» para sujeto singular, comienzo de varios errores
	s" parece haber"
	s" se identifica" 
	s" se reconoce" 3 schoose
	;
: there_is_no$  ( -- a u )  \ Devuelve una variante de «no hay», comienzo de varios errores
	s" no se"
	s" identifica"
	s" encuentra"
	s" reconoce" 3 schoose s& 
	s" el" s" ningún" 2 schoose s&
	;
: 2-action_error  \ Informa de que se ha producido un error porque hay dos verbos en el comando
	there_are$ s" dos verbos"
	there_is$ s" más de un verbo" s&
	there_are$ s" al menos dos verbos"
	3 schoose s&  language_error
	;
: 2-complement_error  \ Informa de que se ha producido un error porque hay dos complementos en el comando
	\ Provisional!!!
	there_are$
	s" dos complementos indirecto o preposicional" s&
	there_is$
	s" más de un complemento indirecto o preposicional" s&
	there_are$
	s" al menos dos complementos indirectos o preposicionales" s&
	3 schoose s&  language_error
	;
: no_verb_error  \ Informa de que se ha producido un error por falta de verbo en el comando
	there_is_no$ s" verbo" s& language_error
	;
: no_direct_complement_error  \ Informa de que se ha producido un error por falta de complemento directo en el comando
	there_is_no$ s" complemento directo" s& language_error
	;
' no_direct_complement_error constant no_direct_complement_error_id 

\ xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
[false] [if]
\ Sistema antiguo!!!
create errors_xt  \ Sistema alternativo a CASE , inacabado!!!
	' noop ,
	' 2-action_error ,
	' 2-complement_error ,
	' no_verb_error ,
	' no_direct_complement_error ,
: misunderstood  ( u -- )  \ Informa de un error en el comando
	\ Inacabado!!!
	\ u = Código de error; si es cero no se hará nada
	case  
		2-action_error_id  of  2-action_error  endof
		2-complement_error_id  of  2-complement_error  endof
		no_verb_error_id  of  no_verb_error  endof
		no_direct_complement_error_id  of  no_direct_complement_error  endof
	endcase
	;
[then]
\ xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

false constant no_error_id
' 2-action_error constant 2-action_error_id
' 2-complement_error constant 2-complement_error_id
' no_verb_error constant no_verb_error_id
' no_direct_complement_error constant no_direct_complement_error_id

: ?misunderstood  ( xt | 0 -- )  \ Informa, si es preciso, de un error en el comando
	\ xt = Dirección de ejecución de la palabra de error (que se usa también como código del error)
	[debug_catch] [if] s" Al entrar en ?MISUNDERSTOOD" debug [then]  \ Depuración!!!
	?dup  if  execute  then
	[debug_catch] [if] s" Al salir de ?MISUNDERSTOOD" debug [then]  \ Depuración!!!
	;

\ }}}

\ ##############################################################
section( Acciones)

\ {{{

variable action  \ Código de la acción del comando
variable direct_complement  \ Código del complemento directo del comando
variable other_complement  \ Código del complemento indirecto o preposicional del comando

\ -------------------------------------------------------------
\ Pronombres

\ Inacabado!!! No usado!!!

variable last_action
variable last_feminine_singular_complement
variable last_masculine_singular_complement
variable last_feminine_plural_complement
variable last_masculine_plural_complement

: init_command  \ Prepara los valores variables usados en los comandos
	\ Inacabado!!! No usado!!!
	last_action off
	\ last_feminine_singular_complement off
	\ last_masculine_singular_complement off
	\ last_feminine_plural_complement off
	\ last_masculine_plural_complement off
	;

\ -------------------------------------------------------------
\ Tratamiento de errores de las acciones

: is_not_here  ( a -- )  \  Informa de que un ente no está presente
	\ Inacabado!!! Falta distinguir si es conocido o desconocido
	>full_name@
	s" no está aquí" 
	s" no está por aquí" 2 schoose s& narrate
	;
: cannot_see$  ( -- a u )  \ Devuelve una forma de decir «no ves»
	s" No"
	s" ves"
	s" puedes ver"
	s" encuentras"
	s" puedes encontrar" 4 schoose s&
	;
: cannot_see  ( a -- )  \ Informa de que un ente no puede ser mirado
	cannot_see$ rot >subjective_name@ s& period+ narrate
	;
: crazy_thing$  ( -- a u )
	s" cosa"
	s" ocurrencia"
	s" disparate" 3 schoose
	;
: so_crazy$  ( -- a u )
	s" así"
	s" tal"
	s" semejante" 3 schoose
	;
: something_so_crazy$  ( -- a u )  \ Devuelve una variante de «hacer algo semejante»
	s" " s" hacer" 2 schoose
	s" algo" so_crazy$ s&
	s" cosa" so_crazy$ s&
	s" eso"
	s" semejante" crazy_thing$ s&
	s" tal" crazy_thing$ s&
	s" tamaña" crazy_thing$ s&
	s" un disparate" so_crazy$ s&
	7 schoose s&
	;
: like_that$  ( -- a u )
	s" así"
	s" como eso"
	2 schoose
	;
: something_like_that$  ( -- a u )  \ Devuelve una variante de «hacer eso»
	s" " s" hacer" 2 schoose
	s" algo así"
	s" algo semejante"
	s" eso"
	s" semejante cosa"
	s" tal cosa"
	s" una cosa así"
	6 schoose s&
	;
: is_impossible$  ( -- a u )  \ Devuelve una cadena con una variante de «es imposible», que formará parte de mensajes personalizados por cada acción
	s" es imposible"
	s" es inviable"
	s" no es posible"
	s" no es viable" 
	s" no sería posible"
	s" no sería viable 
	s" sería imposible"
	s" sería inviable"
	8 schoose
	;
: ^is_impossible$  ( -- a u )  \ Devuelve una cadena con una variante de «Es imposible» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción
	is_impossible$ >^uppercase
	;
: x_is_impossible$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «X es imposible»
	dup
	if  >^uppercase is_impossible$ s&
	else  2drop ^is_impossible$
	then
	;
: it_is_impossible_x$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «Es imposible x»
	^is_impossible$ 2swap s& 
	;
: is_impossible  ( a u -- )  \ Informa de que una acción indicada (en infinitivo) es imposible
	\ a u = Acción imposible, en infinitivo, o una cadena vacía
	['] x_is_impossible$
	['] it_is_impossible_x$
	2 choose execute  period+ narrate
	;
: impossible  \ Informa de que una acción no especificada es imposible
	[debug] [if] s" En IMPOSSIBLE" debug [then]  \ Depuración!!!
	something_like_that$ is_impossible
	;
: try$  ( -- a u )  \ Devuelve una cadena con una variante de «intentar» (o vacía)
	s" intentar" s" pretender" s" " s" " 4 schoose 
	;
: nonsense$  ( -- a u )  \ Devuelve una cadena con una variante de «no tiene sentido», que formará parte de mensajes personalizados por cada acción
	\ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos.
	s" es ilógico"
	s" es una insensatez"
	s" no es nada razonable"
	s" no es nada sensato"
	s" no es razonable"
	s" no es sensato"
	s" no parece lógico"
	s" no parece muy lógico"
	s" no parece muy razonable"
	s" no parece razonable"
	s" no parece sensato"
	s" no sería nada razonable"
	s" no sería nada sensato"
	s" no sería razonable"
	s" no sería sensato"
	s" no tendría mucho sentido"
	s" no tendría ningún sentido"
	s" no tendría sentido alguno"
	s" no tendría sentido"
	s" no tiene lógica ninguna"
	s" no tiene lógica"
	s" no tiene mucha lógica"
	s" no tiene mucho sentido"
	s" no tiene ninguna lógica"
	s" no tiene ningún sentido"
	s" no tiene sentido"
	s" sería algo ilógico"
	s" sería ilógico" 
	s" sería una insensatez"
	29 schoose 
	;
: ^nonsense$  ( -- a u )  \ Devuelve una cadena con una variante de «No tiene sentido» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción
	nonsense$ >^uppercase
	;
: x_is_nonsense$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «X no tiene sentido»
	dup
	if  try$ 2swap s& >^uppercase nonsense$ s&
	else  2drop ^nonsense$
	then
	;
: it_is_nonsense_x$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «No tiene sentido x»
	^nonsense$ try$ s& 2swap s& 
	;
: is_nonsense  ( a u -- ) \ Informa de una acción dada (en infinitivo) no tiene sentido
	\ a u = Acción que no tiene sentido, en infinitivo, o una cadena vacía
	['] x_is_nonsense$
	['] it_is_nonsense_x$ 
	2 choose execute  period+ narrate
	;
: nonsense  \ Informa de que alguna acción no especificada no tiene sentido
	\ Provisional!!!
	[debug] [if] s" En NONSENSE" debug [then]  \ Depuración!!!
	something_so_crazy$ is_nonsense 
	;
' nonsense alias nonsense_error_id
: dangerous$  ( -- a u )  \ Devuelve una cadena con una variante de «es peligroso», que formará parte de mensajes personalizados por cada acción
	\ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos.
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
	13 schoose 
	;
: ^dangerous$  ( -- a u )  \ Devuelve una cadena con una variante de «Es peligroso» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción
	dangerous$ >^uppercase
	;
: x_is_dangerous$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «X es peligroso»
	dup
	if  try$ 2swap s& >^uppercase dangerous$ s&
	else  2drop ^dangerous$
	then
	;
: it_is_dangerous_x$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «Es peligroso x»
	^dangerous$ try$ s& 2swap s& 
	;
: is_dangerous  ( a u -- )  \ Informa de una acción dada (en infinitivo) no tiene sentido
	\ a u = Acción que no tiene sentido, en infinitivo, o una cadena vacía
	['] x_is_dangerous$
	['] it_is_dangerous_x$ 
	2 choose execute  period+ narrate
	;
: dangerous  \ Informa de que alguna acción no especificada no tiene sentido
	something_like_that$ is_dangerous
	;
' dangerous alias dangerous_error_id
: ?full_name&  ( a1 u1 a2 -- )  \ Añade a una cadena el nombre de un posible ente
	\ No se usa!!!
	\ a1 u1 = Cadena
	\ a2 = Ente (o cero)
	?dup  if  >full_name@ s&  then
	;
: +is_nonsense  ( a1 u1 a2 -- )  \ Informa de una acción dada (en infinitivo), que hay que completar con el nombre de un supuesto ente, no tiene sentido
	\ a1 u1 = Acción en infinitivo
	\ a2 = Ente al que se refiere la acción y cuyo objeto directo es (o cero)
	?dup
	if >full_name@ s& is_nonsense
	else  2drop nonsense
	then
	;
: direct_complement+is_nonsense  ( a u -- ) \ Informa de una acción dada (en infinitivo), que hay que completar con el nombre del complemento directo, no tiene sentido
	direct_complement @ +is_nonsense
	;
: other_complement+is_nonsense  ( a u -- ) \ Informa de una acción dada (en infinitivo), que hay que completar con el nombre del complemento auxiliar, no tiene sentido
	other_complement @ +is_nonsense
	;
: no_reason_for$  ( -- a u )  \ Devuelve una cadena con una variante de «no hay motivo para»
	\ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos.
	s" no hay"
	s" razón para"
	s" motivo para"
	s" necesidad de" 3 schoose s&
	;
: no_reason_for_that  ( a u -- )  \ Informa de que no hay motivo para una acción (en infinitivo)
	\ a u = Acción para la que no hay razón, en infinitivo, o una cadena vacía
	\ Pendiente!!!
	no_reason_for$ 2swap s& narrate
	;
: no_reason  \ Informa de que no hay motivo para una acción no especificada
	\ Pendiente!!!
	something_like_that$ no_reason_for_that
	;
: nonsense|no_reason  \ Informa de que una acción no especificada no tiene sentido o no tiene motivo
	\ No se usa todavía!!!
	2 random  if  nonsense  else  no_reason  then
	;
: well_done  \ Informa de que una acción se ha realizado
	\ Provisional!!!
	s" Hecho." narrate
	;
: (do_not_worry_0)$  ( -- a u)  \ Primera versión posible del mensaje de DO_NOT_WORRY
	s" Hay"
	s" cosas" s" tareas" s" asuntos" s" cuestiones" 4 schoose s&
	s" más" s&
	s" importantes" s" necesarias" s" urgentes" 3 schoose s&
	s" "
	s" para prestarles atención"
	s" de que ocuparse" 3 schoose s& 
	;
: (do_not_worry_1)$  ( -- a u)  \ Segunda versión posible del mensaje de DO_NOT_WORRY
	s" Eso no"
	s" tiene importancia"
	s" tiene utilidad"
	s" importa"
	s" hace falta" 
	s" es menester" 
	s" es importante"
	s" es necesario" 7 schoose s&
	;
: do_not_worry  \ Informa de que una acción no tiene importancia
	\ Provisional!!!
	['] (do_not_worry_0)$
	['] (do_not_worry_1)$ 2 choose execute
	now$ s&
	period+ narrate
	;
: that$  ( a -- a1 u1 )  \  Devuelve el nombre de un ente, o un pronombre demostrativo
	2 random
	if  drop s" eso"  else  >full_name@  then
	;
: you_do_not_have_it_(0)$  ( a -- )  \ Devuelve mensaje de que el protagonista no tiene un ente (variante 0)
	s" No" you_carry$ s& rot that$ s& with_you$ s&
	;
: you_do_not_have_it_(1)$  ( a -- )  \ Devuelve mensaje de que el protagonista no tiene un ente (variante 1, solo para entes conocidos)
	s" No" rot >direct_pronoun@ s& you_carry$ s& with_you$ s&
	;
: you_do_not_have_it_error  ( a -- )  \ Informa de que el protagonista no tiene un ente
	dup >familiar @ over >owned? or   if
		['] you_do_not_have_it_(0)$
		['] you_do_not_have_it_(1)$
		2 choose execute
	else  you_do_not_have_it_(0)$
	then  period+ narrate
	;
' you_do_not_have_it_error constant you_do_not_have_it_error_id
: (you_do_not_wear_it)  ( a -- )  \ Informa de que el protagonista no lleva puesto un ente prenda
	>r s" No llevas puest" r@ noun_ending+
	r> >full_name@ s& period+ narrate
	;
: you_do_not_wear_it  ( a -- )  \ Informa de que el protagonista no lleva puesto un ente prenda, según lo lleve o no consigo
	dup is_hold?
	if  you_do_not_have_it_error
	else  (you_do_not_wear_it) 
	then
	;
: you_already_wear_it  ( a -- )  \ Informa de que el protagonista lleva puesto un ente prenda
	>r s" Ya llevas puest" r@ noun_ending+
	r> >full_name@ s& period+ narrate
	;
: not_by_hand$  ( -- a u )  \ Devuelve el mensaje de NOT_BY_HAND
	s" En cualquier caso,"
	s" En todo caso,"
	2 schoose
	s" no con las manos desnudas." s&
	;
: not_by_hand  \ Informa de que la acción no puede hacerse sin una herramienta
	not_by_hand$ narrate
	;
: not_with_that$  ( -- a u )  \ Devuelve el mensaje de NOT_WITH_THAT
	s" Con eso no..." 
	s" No con eso..." 
	2 schoose
	;
: not_with_that  \ Informa de que la acción no puede hacerse con la herramienta elegida
	not_with_that$ narrate
	;

\ -------------------------------------------------------------
\ Mirar, examinar y registrar

: actually_do_look  ( a -- )  \ Mira un ente
	dup describe
	>location? @  if  .present  then
	;
: do_look_if_possible  ( a -- )  \ Mira un ente si es posible
	dup can_be_looked_at?
	if  actually_do_look
	else  cannot_see
	then
	;
: do_look  \  Acción de mirar
	direct_complement @ ?dup 0=  if  my_location@  then  \ Si no hay complemento, usa el lugar actual
	do_look_if_possible
	;
' do_look constant do_look_xt
: do_examine  \ Acción de examinar
	\ Provisional!!!
	do_look
	;
' do_examine constant do_examine_xt
: do_search  \ Acción de registrar
	\ Provisional!!!
	do_look
	;
' do_search constant do_search_xt

\ -------------------------------------------------------------
\ Salidas

create do_exits_table  \ Tabla de traducción de salidas
#exits cells allot  \ Reservar espacio para tantas celdas como salidas

\ Rellenar cada elemento de la tabla con un ente de salida,
\ usando como puntero el campo análogo de la ficha.
\ Haciéndolo de esta manera no importa el orden en que se rellenen los elementos.
north_e do_exits_table >north_exit first_exit - !
south_e do_exits_table >south_exit first_exit - !
east_e do_exits_table >east_exit first_exit - !
west_e do_exits_table >west_exit first_exit - !
up_e do_exits_table >up_exit first_exit - !
down_e do_exits_table >down_exit first_exit - !
out_e do_exits_table >out_exit first_exit - !
in_e do_exits_table >in_exit first_exit - !

\ Inacabado!!! No se usa!!!
create do_exits_table_index  \ Tabla para desordenar el listado de salidas
#exits cells allot
\ Esta tabla permite que las salidas se muestren cada vez en un orden diferente

variable #free_exits  \ Contador de las salidas posibles
: no_exit$  ( -- a u )  \ Devuelve mensaje usado cuando no hay salidas que listar
	s" No hay salidas"
	s" No hay salida"
	s" No hay ninguna salida"
	3 schoose
	;
: go_out$  ( -- a u )
	s" salir" s" seguir" 2 schoose
	;
: go_out_to& ( a u -- a1 u1 )
	go_out$ s& s" hacia" s&
	;
: one_exit_only$  ( -- a u )  \ Devuelve mensaje usado cuando solo hay una salidas que listar
	s" La única salida" possible1$ s& s" es" s& toward$ s&
	s" Solo hay salida" possible1$ s& s" hacia" s&
	s" Solo es posible" go_out_to&
	s" Solo se puede" go_out_to&
	4 schoose
	;
: several_exits$  ( -- a u )  \ Devuelve mensaje usado cuando hay varias salidas que listar
	s" Hay salidas" possible2$ s& s" hacia" s&
	s" Las salidas" possible2$ s& s" son" s&
	2 schoose
	;
: .exits  ( -- )  \ Imprime las salidas posibles
	#listed @  case
		0  of  no_exit$  endof
		1  of  one_exit_only$  endof
		several_exits$ rot
	endcase
	«& «»@ period+ narrate
	;
: exit_separator$  ( -- a u )  \ Devuelve una cadena con el separador adecuado a la salida actual
	#free_exits @ #listed @ list_separator$
	;
: (do_exit)  ( u -- )  \ Lista una salida
	\ u = Desplazamiento del campo de salida
	[debug_do_exits] [if]  cr ." (do_exit)" cr .stack [then]  \ Depuración!!!
	exit_separator$ »+
	first_exit - do_exits_table + @ >full_name@ »+
	#listed ++
	[debug_do_exits] [if]  cr .stack [then]  \ Depuración!!!
	;
: free_exits  ( a -- u )  \ Devuelve el número de salidas posibles de un ente
	[debug_do_exits] [if]  cr ." free_exits" cr .stack [then]  \ Depuración!!!
	0 swap
	>first_exit /exits bounds  do
		[debug_do_exits] [if]  i i cr . @ .  [then]  \ Depuración!!!
		i @ 0<> abs +
	cell  +loop 
	[debug_do_exits] [if] cr .stack [then]  \ Depuración!!!
	;
: do_exits  \ Lista las salidas posibles del lugar del protagonista
	\ No funciona todavía!!!
	«»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
	#listed off
	my_location@ dup free_exits #free_exits !
	last_exit 1+ first_exit  do
		[debug_do_exits] [if]  i cr .  [then]  \ Depuración!!!
		dup i + @
		[debug_do_exits] [if]  dup .  [then]  \ Depuración!!!
		if  i (do_exit)  then
	cell  +loop  drop
	.exits
	;
' do_exits constant do_exits_xt

\ -------------------------------------------------------------
\ Tomar y dejar

: cannot_take_the_altar  \ No se puede tomar el altar
	s" [el altar no se toca]" narrate  \ tmp!!!
	impossible
	;
: cannot_take_the_flags  \ No se puede tomar las banderas
	s" [las banderas no se tocan]" narrate  \ tmp!!!
	nonsense
	;
: cannot_take_the_idol  \ No se puede tomar el ídolo
	s" [el ídolo no se toca]" narrate  \ tmp!!!
	impossible
	;
: cannot_take_the_door  \ No se puede tomar la puerta
	s" [la puerta no se toca]" narrate  \ tmp!!!
	impossible
	;
: cannot_take_the_fallen_away  \ No se puede tomar el derrumbe
	s" [el derrumbe no se toca]" narrate  \ tmp!!!
	nonsense
	;
: cannot_take_the_snake  \ No se puede tomar la serpiente
	s" [la serpiente no se toca]" narrate  \ tmp!!!
	dangerous
	;
: cannot_take_the_lake  \ No se puede tomar el lago
	s" [el lago no se toca]" narrate  \ tmp!!!
	nonsense
	;
: cannot_take_the_lock  \ No se puede tomar el candado
	s" [el candado no se toca]" narrate  \ tmp!!!
	impossible
	;
: cannot_take_the_water_fall  \ No se puede tomar la cascada
	s" [la cascada no se toca]" narrate  \ tmp!!!
	nonsense
	;
: actually_do_take  ( a -- )  \ Toma un objeto
	dup be_hold more_familiar well_done
	;
: do_take_if_no_exception  ( a -- )  \ Toma un objeto presente, si no es una excepción
	\ Nota!!! Sustituir esta estructura por una dirección de ejecución guardada en la ficha.
	case
		altar_e  of  cannot_take_the_altar  endof
		door_e  of  cannot_take_the_door  endof
		flags_e  of  cannot_take_the_flags  endof
		fallen_away_e  of  cannot_take_the_fallen_away  endof
		idol_e  of  cannot_take_the_idol  endof	
		lake_e  of  cannot_take_the_lake  endof
		lock_e  of  cannot_take_the_lock  endof
		snake_e  of  cannot_take_the_snake  endof
		dup actually_do_take
	endcase	
	;
: do_take_if_exception  ( a -- )  \ Toma un objeto no presente, si es una excepción
	dup is_accessible?
	if  drop s" [está accesible]" narrate  \ tmp!!!
	else  cannot_see
	then
	;
: do_take_if_possible  ( a -- )  \ Toma un objeto si es posible
	dup is_here?
	if  do_take_if_no_exception
	else  do_take_if_exception
	then
	;
: do_take  \ Acción de tomar
	direct_complement @
	\ Error si no hay objeto directo:
	dup 0= no_direct_complement_error_id and throw
	\ Acción posible:
	do_take_if_possible
	;
' do_take constant do_take_xt
\ antiguo!!!
\ : do_drop_if_possible  ( a -- )  \ Suelta un objeto si es posible
\ 	dup is_hold?
\ 	if  dup >worn? off  be_here  well_done
\ 	else  you_do_not_have_it_error
\ 	then
\ 	;
: do_drop  \ Acción de soltar
\	\ Antiguo!!!
\ 	direct_complement @ ?dup
\ 	if  do_drop_if_possible
\ 	else  no_direct_complement_error
\ 	then
	direct_complement @
	\ Error si no hay objeto directo:
	dup 0= no_direct_complement_error_id and throw
	\ Error si no lo llevamos: 
	dup is_hold? 0= you_do_not_have_it_error_id and throw
	\ Realizar la acción:
	dup >worn? off  be_here  well_done
	;
' do_drop constant do_drop_xt

\ -------------------------------------------------------------
\ Ponerse y quitarse prendas

: actually_do_put_on  ( a -- )  \ Ponerse una prenda
	>worn? on  well_done
	;
: do_put_on_if_possible  ( a -- )  \ Ponerse una prenda, si está en inventario y es tal
	\ Pendiente!!! Hacer que tome la prenda si no la tiene.
	dup is_hold?  if
		dup >worn? @
		if  you_already_wear_it
		else
			dup >cloth? @
			if  actually_do_put_on  else  drop nonsense  then
		then
	else  you_do_not_have_it_error  \ Provisional!!! Cambiar el mensaje si no es prenda.
	then
	;
: do_put_on  \ Acción de ponerse una prenda
	direct_complement @ ?dup
	if  do_put_on_if_possible
	else  no_direct_complement_error
	then
	;
' do_put_on constant do_put_on_xt
: actually_do_take_off  ( a -- )  \ Quitarse una prenda
	>worn? off  well_done
	;
: do_take_off_if_possible  ( a -- )  \ Quitarse una prenda, si es posible
	dup is_worn?
	if  actually_do_take_off
	else  you_do_not_wear_it
	then
	;
: do_take_off  \ Acción de quitarse una prenda
	direct_complement @ ?dup
	if  do_take_off_if_possible
	else  no_direct_complement_error
	then
	;
' do_take_off constant do_take_off_xt

\ -------------------------------------------------------------
\ Cerrar y abrir

: do_close  \ Acción de cerrar
	s" cerrar" narrate  
	;
' do_close constant do_close_xt
: do_open  \ Acción de abrir
	s" abrir" narrate  
	;
' do_open constant do_open_xt

\ -------------------------------------------------------------
\ Agredir

: do_attack  \ Acción de atacar
	s" atacar" direct_complement+is_nonsense
	;
' do_attack constant do_attack_xt
: actually_do_kill  ( a -- )  \ Mata un ser vivo
	\ Pendiente!!! 
	s" Matas " rot >full_name@ s& narrate
	;
: do_kill_if_living  ( a -- )  \ Acción de matar, si se trata de un ser vivo
	>living_being?
	if  actually_do_kill
	else  nonsense
	then
	;
: do_kill_if_possible  ( a -- )  \ Acción de matar, si es posible
	dup is_here?
	if  do_kill_if_living
	else  do_take_if_exception
	then
	;
: do_kill  \ Acción de matar
	\ s" matar"  direct_complement+is_nonsense
	direct_complement @ ?dup
	if  do_kill_if_possible
	else  no_direct_complement_error
	then
	;
' do_kill constant do_kill_xt
: do_break  \ Acción de romper
	s" romper"  direct_complement+is_nonsense
	;
' do_break constant do_break_xt
: do_hit  \ Acción de golpear
	s" golpear"  direct_complement+is_nonsense
	;
' do_hit constant do_hit_xt
: >can_be_sharpened?@  ( a -- f )  \ ¿Puede un ente ser afilado?
	\ Pendiente!!! Mover esta palabra junto con los demás seudo-campos.
	dup log_e =  swap sword_e =  or  \ ¿Es el tronco o la espada?
	;
: log_already_sharpened$  ( -- a u ) \ Devuulve una cadena con una variante de «Ya está afilado»
	s" Ya lo afilaste antes"
	s" Ya está afilado de antes"
	s" Ya tiene una buena punta"
	3 schoose
	;
: no_need_to_do_it_again$  ( -- a u )  \ Devuelve una cadena con una variante de «no hace falta hacerlo otra vez»
	s" no es necesario"
	s" no hace falta" 
	s" no es menester" 
	s" no serviría de nada"
	s" serviría de poco" 
	s" sería inútil" 
	s" sería en balde" 
	s" sería un esfuerzo inútil"
	s" sería un esfuerzo baldío" 8 schoose
	s" hacerlo"
	s" volver a hacerlo"
	s" repetirlo" 3 schoose s&
	again$ s&
	;
: ^no_need_to_do_it_again$  ( -- a u )  \ Devuelve una cadena con una variante de «No hace falta hacerlo otra vez»
	no_need_to_do_it_again$ >^uppercase
	;
: log_already_sharpened_0$  ( -- a u )  \ Devuelve un mensaje de que el tronco ya estaba afilado (variante 0)
	log_already_sharpened$ >^uppercase period+
	^no_need_to_do_it_again$ period+ s&
	;
: log_already_sharpened_1$  ( -- a u )  \ Devuelve un mensaje de que el tronco ya estaba afilado (variante 1)
	^no_need_to_do_it_again$ period+ s&
	log_already_sharpened$ >^uppercase period+ s&
	;
: log_already_sharpened  \ Informa de que el tronco ya estaba afilado
	['] log_already_sharpened_0$
	['] log_already_sharpened_1$
	2 choose execute  narrate
	;
: do_sharpen_the_log  \ Afila el tronco
	\ Inacabado!!! ?
	hacked_the_log? @
	if
	else  hacked_the_log? on  well_done
	then
	;
: do_sharpen_the_sword  \ Afila la espada
	;
: actually_do_sharpen  ( a -- )  \ Afila un ente que puede ser afilado
	case
		sword_e  of  do_sharpen_the_sword  endof
		log_e  of  do_sharpen_the_log  endof
	endcase
	;
: do_sharpen_if_possible  ( a -- ) \ Afilar un ente accesible si es posible
	dup >can_be_sharpened?@
	if  actually_do_sharpen
	else  s" afilar" rot +is_nonsense
	then
	;
: do_sharpen_if_accessible  ( a -- ) \ Afilar un ente si está accesible
	dup is_accessible?
	if  do_sharpen_if_possible
	else  cannot_see
	then
	;
: do_sharpen  \ Acción de afilar
	direct_complement @ ?dup
	if  do_sharpen_if_accessible
	else  no_direct_complement_error
	then
	;
' do_sharpen constant do_sharpen_xt

\ -------------------------------------------------------------
\ Movimiento

: that_way_0$  ( a -- a2 u2 )  \ Devuelve «al/hacia la dirección indicada»
	\ a = Ente de dirección
	\ >r  2 random  r@ >no_article? @  or  \ old!!!
	>r  r@ >no_article? @
	if  \ no debe llevar artículo
		s" hacia" r> >full_name@ 
	else  \ debe llevar artículo
		s" al" s" hacia el" 2 schoose
		r> >name@ >^uppercase
	then  s&
	;
: that_way_1$  ( -- a u )  \ Devuelve una variante de «en esa dirección»
	s" en esa dirección" s" por ahí" 2 schoose
	;
: impossible_move  ( a -- )  \ El movimiento es imposible
	\ a = Ente de dirección
	\ Inacabado!!! Añadir una tercera variante «ir en esa dirección»; y otras específicas como «no es posible subir».
	^is_impossible$ s" ir" s&  rot
	3 random 
	if  that_way_0$
	else  drop that_way_1$
	then  s& period+ narrate
	;
: (enter)  ( a -- )  \ Entra en un lugar
	[debug] [if] s" En ENTER" debug [then]  \ Depuración!!!
	dup protagonist be_there
	dup describe
	more_familiar  .present
	;
' (enter) is enter
: do_go_if_possible  ( a -- )  \ Comprueba si el movimiento es posible y lo efectúa
	\ a = Ente supuestamente de tipo dirección
	[debug] [if] s" Al entrar en DO_GO_IF_POSSIBLE" debug [then]  \ Depuración!!!
	dup >direction @ ?dup  if  \ ¿El ente es una dirección?
		my_location@ + @ ?dup
		if  nip enter  else  impossible_move  then
	else  drop nonsense
	then
	[debug] [if] s" Al salir de DO_GO_IF_POSSIBLE" debug [then]  \ Depuración!!!
	;
: do_go  \ Acción de ir
	[debug] [if] s" Al entrar en DO_GO" debug [then]  \ Depuración!!!
	direct_complement @ ?dup
	if  do_go_if_possible
	else  s" ir sin más!!!" narrate
	then
	[debug] [if] s" Al salir de DO_GO" debug [then]  \ Depuración!!!
	;
' do_go constant do_go_xt
: do_go_north  \ Acción de ir al Norte
	north_e do_go_if_possible
	;
' do_go_north constant do_go_north_xt
: do_go_south  \ Acción de ir al Sur
	[debug_catch] [if] s" Al entrar en DO_GO_SOUTH" debug [then]  \ Depuración!!!
	south_e do_go_if_possible
	[debug_catch] [if] s" Al salir de DO_GO_SOUTH" debug [then]  \ Depuración!!!
	;
' do_go_south constant do_go_south_xt
: do_go_east  \ Acción de ir al Este
	east_e do_go_if_possible
	;
' do_go_east constant do_go_east_xt
: do_go_west  \ Acción de ir al Oeste
	west_e do_go_if_possible
	;
' do_go_west constant do_go_west_xt
: do_go_up  \ Acción de ir hacia arriba
	up_e do_go_if_possible
	;
' do_go_up constant do_go_up_xt
: do_go_down  \ Acción de ir hacia abajo
	down_e do_go_if_possible
	;
' do_go_down constant do_go_down_xt
: do_go_out  \ Acción de ir hacia fuera
	s" voy fuera" narrate \ tmp!!!
	;
' do_go_out constant do_go_out_xt
: do_go_in  \ Acción de ir hacia dentro
	s" voy dentro" narrate \ tmp!!!
	;
' do_go_in constant do_go_in_xt
: do_go_back  \ Acción de ir hacia atrás
	s" voy atrás" narrate \ tmp!!!
	;
' do_go_back constant do_go_back_xt
: do_go_ahead  \ Acción de ir hacia delante
	s" voy alante" narrate \ tmp!!!
	;
' do_go_ahead constant do_go_ahead_xt

\ -------------------------------------------------------------
\ Nadar

: swiming$  ( -- a u )  \ Devuelve el mensaje sobre el buceo
	s" Buceas"
	s" pensando en"
	s" deseando"
	s" con la esperanza de" 
	s" con la intención de" 4 schoose s&
	s" avanzar,"
	s" huir,"
	s" escapar," 
	s" salir," 4 schoose s&
	s" aunque" s&
	s" perdido."
	s" desorientado." 2 schoose s&
	;
: drop_the_cuirasse$  ( f -- a u )  \ Devuelve el mensaje sobre deshacerse de la coraza dentro del agua
	\ f = ¿Inicio de frase?
	s" te desprendes de ella"
	s" te deshaces de ella"
	s" la dejas caer"
	s" la sueltas"
	4 schoose
	rot  if  \ ¿Inicio de frase?
		s" Rápidamente"
		s" Sin dilación"
		s" Sin dudarlo"
		s" un momento" s" un instante" s" " 3 schoose s&
		3 schoose  2swap s&
	then  period+
	;
: you_leave_the_cuirasse$  ( -- a u )  \ Devuelve el mensaje sobre quitarse y soltar la coraza dentro del agua
	cuirasse_e is_worn?  \ ¿La llevamos puesta?
	if
		s" Como puedes,"
		s" No sin dificultad," 2 schoose
		s" logras quitártela y"
		s" te la quitas y" 2 schoose s&
		false drop_the_cuirasse$ s&
	else
		true drop_the_cuirasse$
	then
	;
: (you_sink_1)$ ( -- a u )  \ Devuelve la primera versión del mensaje sobre hundirse con la coraza
	s" Caes"
	s" Te hundes"
	s" Empiezas a hundirte"
	s" Empiezas a caer" 4 schoose
	s" sin remedio" s" " 2 schoose s&
	s" hacia el fondo"
	s" hacia las profundidades" 2 schoose s&
	s" por el"
	s" debido al" 2 schoose s&
	s" peso de tu coraza" s&
	;
: (you_sink_2)$ ( -- a u )  \ Devuelve la segunda versión del mensaje sobre hundirse con la coraza
	s" El peso de tu coraza"
	s" te arrastra"
	s" tira de ti" 2 schoose s&
	s" sin remedio" s" con fuerza" s" " 3 schoose s&
	s" hacia el fondo"
	s" hacia las profundidaes" 
	s" hacia abajo" 3 schoose s&
	;
: you_sink$ ( -- a u )  \ Devuelve el mensaje sobre hundirse con la coraza
	2 random
	if  (you_sink_1)$
	else  (you_sink_2)$
	then  period+
	;
: you_swim_with_cuirasse$  ( -- a u )  \  Devuelve el mensaje inicial sobre nadar con coraza
	you_sink$ you_leave_the_cuirasse$ s&
	;
: you_swim$  ( -- a u )  \  Devuelve el mensaje sobre nadar
	cuirasse_e is_hold?  \ ¿Llevamos la coraza?
	if  you_swim_with_cuirasse$  cuirasse_e vanish
	else  s" "
	then  swiming$ s&
	;
: do_swim  \ Acción de nadar
	my_location@ location_11_e =  if
		clear_screen_for_location
		you_swim$ narrate short_pause
		location_12_e enter  the_battle_ends
	else
		s" nadar" now|here$ s& is_nonsense
	then
	;
' do_swim constant do_swim_xt

\ -------------------------------------------------------------
\ Escalar

: do_climb_if_possible  ( a -- )  \ Escalar el ente indicado si es posible
	\ Inacabado!!!
	dup is_here?  if  s" [escalar]" narrate
	else  drop s" [no está aquí]" narrate
	then
	;
: do_climb  \ Acción de escalar
	direct_complement @ ?dup
	if  do_climb_if_possible
	else  no_direct_complement_error  \ Inacabado!!! Comprobar si es el derrumbe u otra cosa.
	then
	;
' do_climb constant do_climb_xt
 
\ -------------------------------------------------------------
\ Inventario

: anything_with_you$  ( -- a u )  \ Devuelve una cadena con una variante de «nada contigo»
	s" nada" with_you$  ?dup  if
		2 random  if  2swap  then  s&
	else drop
	then
	;
: he_carries_nothing$  ( -- a u )  \ Devuelve un mensaje para sustituir a un inventario vacío
	\ Antiguo, no se usa!!!
	^our_hero$ s" no" carries$ anything_with_you$ period+ s& s& s& 
	;
: he_carries$  ( -- a u )  \ Devuelve un mensaje para encabezar la lista de inventario
	\ Antiguo, no se usa!!!
	^our_hero$ carries$ with_you$ s& s&
	;
: you_are_carrying_nothing$  ( -- a u )  \ Devuelve un mensaje para sustituir a un inventario vacío
	s" No" you_carry$ anything_with_you$ period+ s& s& 
	;
: ^you_are_carrying$  ( -- a u )  \ Devuelve un mensaje para encabezar la lista de inventario
	^you_carry$ with_you$ s& 
	;
: you_are_carrying$  ( -- a u )  \ Devuelve un mensaje para encabezar la lista de inventario
	you_carry$ with_you$ s& 
	;
: you_are_carrying_only$  ( -- a u )  \ Devuelve un mensaje para encabezar una lista de inventario de un solo elemento
	2 random
	if  ^you_are_carrying$ only$ s& 
	else  ^only$ you_are_carrying$ s& 
	then
	;
: do_inventory  \ Acción de hacer inventario
	protagonist content_list  \ Hace la lista en la cadena dinámica PRINT_STR
	#listed @ case
		0 of  you_are_carrying_nothing$ 2swap s& endof
		1 of  you_are_carrying_only$ 2swap s& endof
		>r ^you_are_carrying$ 2swap s& r>
	endcase  narrate 
	;
' do_inventory constant do_inventory_xt

\ -------------------------------------------------------------
\ Hablar

: talked_to_the_leader  \ Aumentar el contador de conversaciones con el líder
	leader_e >conversations ++
	;
: the_leader_talks_about_the_stone  \ El líder habla acerca de la piedra
	s" El" old_man$ s&
	s" se irrita." s" se enfada." s" se enfurece." 3 schoose s&
	narrate
	s" No podemos permitiros"
	s" huir con"
	s" escapar con"
	s" marchar con"
	s" pasar con"
	s" que os vayáis con"
	s" que os marchéis con"
	s" que marchéis con"
	s" que os llevéis"
	s" que robéis" 9 schoose s&
	s" la piedra del druida." speak
	\ Pendiente!!! cambiar el nombre de la piedra por «piedra del druida»
	s" Hace un gesto..." narrate short_pause
	s" La piedra"
	s" debe" s" tiene que" 2 schoose s&
	s" ser devuelta"
	s" devolverse"
	s" regresar"
	s" volver" 4 schoose s&
	s" a su lugar de encierro." speak
	s" Un hombre"
	s" Uno de los hombres"
	s" Uno de los refugiados" 3 schoose
	s" te arrebata" s" te quita" 2 schoose s&
	s" la piedra" s&
	s" de las manos" s" " 2 schoose s&
	2 random  if
		s" y se la lleva."
	else
		s" y se marcha"
		s" y se va"
		s" y desaparece" 3 schoose 
		s" con ella" s" " 2 schoose s& period+
	then  s&
	narrate
	location_18_e stone_e be_there
	;
: the_leader_talks_about_the_sword  \ El líder habla acerca de la espada
	s" El" old_man$ s& s" se" s& 
	s" irrita"
	s" enfada"
	s" enoja"
	s" enfurece" 4 schoose s&
	s" y alza"
	s" y extiende" 
	s" y levanta" 3 schoose s&
	s" su" s" el" 2 schoose s&
	s" brazo" s&
	s" indicando"
	s" en dirección"
	s" señalando" 3 schoose s&
	to_the$ s& s" Norte." s&
	narrate
	s" Nadie" s" Ningún hombre" 2 schoose
	s" con" 
	s" llevando"
	s" portando"
	s" portador"
	s" que porte"
	s" que lleve" 6 schoose s&
	s" armas" s" un arma" s" una espada" 3 schoose s&
	with_him$ s&
	s" debe" s" puede " 2 schoose s& 
	s" pasar." s&
	speak
	;
: the_leader_lets_you_go  \ El jefe deja marchar al protagonista
	location_28_e location_29_e e-->  \ Hacer que la salida al Este de LOCATION_28_E conduzca a LOCATION_29_E
	s" El" old_man$ s& comma+
	s" calmado," s" sereno," s" tranquilo," 3 schoose s&
	s" indica" s" señala" 2 schoose s&
	s" hacia el" s" en dirección al" 2 schoose s&
	s" Este y" s&
	s" habla:" s" dice:" 2 schoose s&
	narrate
	s" Si vienes en paz, puedes ir en paz." speak
	s" Todos"
	s" Todos los refugiados" 
	s" Los refugiados" 3 schoose 
	s" se apartan y" s&
	s" permiten ahora el paso"
	s" dejan el camino libre"
	s" dejan libre el camino"
	s" dejan libre el paso"
	s" dejan el paso libre" 5 schoose s&
	to_the$ s& s" Este." s&
	narrate
	;
: conversation_0_with_the_leader
    s" Me llamo Ulfius y..." speak
    talked_to_the_leader? on
    s" El" old_man$ s&
	s" asiente, impaciente." s& narrate
    s" Somos refugiados de la gran guerra."
	s" Buscamos la paz." s&
	speak short_pause  talked_to_the_leader
	;
: the_leader_checks_what_you_carry  \ El jefe controla lo que llevas

	[false] [if] \ First old version:
	stone_e is_accessible?  if
		the_leader_talks_about_the_stone
	else
		sword_e is_accessible?
		if  the_leader_talks_about_the_sword
		else  the_leader_lets_you_go
		then
	then
	[then]

	\ Clearer version:
	true case
		stone_e is_accessible? of the_leader_talks_about_the_stone endof
		sword_e is_accessible? of  the_leader_talks_about_the_sword endof
		the_leader_lets_you_go
	endcase

	;
: talk_to_the_leader  \ Hablar con el jefe
	leader_e >conversations @ 0=
	if  conversation_0_with_the_leader  then
	the_leader_checks_what_you_carry  
	;
: talked_to_ambrosio  \ Aumentar el contador de conversaciones con Ambrosio
	ambrosio_e >conversations ++
	;
: (conversation_0_with_ambrosio)  \ Primera conversación con Ambrosio
	s" Hola, buen hombre." speak
	s" Hola, Ulfius." 
	s" Mi nombre es" s" Me llamo" 2 schoose s&
	s" Ambrosio" 2dup ambrosio_e >name!
	period+ s& speak
	end_of_scene
	\ Consultar!!! ¿a qué se refiere «primera vez»?
	s" Por primera vez en mucho tiempo, te sientas"
	s" y cuentas a alguien todo lo que ha pasado." s&
	s" Y tras tanto acontecido, lloras desconsoladamente." s&
	narrate end_of_scene
	s" Ambrosio te propone un trato, que aceptas:"
	s" por ayudarle a salir de la cueva," s&
	s" objetos, vitales para la empresa, te son entregados." s&
	narrate short_pause
	torch_e be_hold  flint_e be_hold
	s" Bien," s" Venga," s" Vamos," 3 schoose
	s" Ambrosio,"
	s" emprendamos la marcha."
	s" pongámonos en camino." 2 schoose s&
	speak
	location_46_e ambrosio_e be_there
	s" Te das la vuelta"
	s" para ver si Ambrosio te sigue," s&
	s" pero... ha desaparecido." s&
	narrate short_pause
	s" Piensas entonces en el hecho curioso"
	s" de que supiera tu nombre." s&
	narrate end_of_scene  talked_to_ambrosio
	;
: conversation_0_with_ambrosio  \ Primera conversación con Ambrosio, si se dan las condiciones
	\ Inacabado!!!
	(conversation_0_with_ambrosio)
	;
: (conversation_1_with_ambrosio)  \ Segunda conversación con Ambrosio
	s" La llave, Ambrosio, estaba ya en tu poder."
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
: conversation_1_with_ambrosio  \ Segunda conversación con Ambrosio, si se dan las condiciones
	location_46_e am_i_there?
	ambrosio_follows? 0=  and
	if  (conversation_1_with_ambrosio)  then
	;
: (conversation_2_with_ambrosio)  \ Tercera conversación con Ambrosio
	s" Por favor, Ulfius, cumple tu promesa."
	s" Toma la llave en tu mano" s&
	s" y abre la puerta de la cueva." s&  speak
	key_e be_hold
	\ do_takeable the_key \ pendiente!!!
	ambrosio_follows? on  talked_to_ambrosio  
	;
: conversation_2_with_ambrosio  \ Tercera conversación con Ambrosio, si se dan las condiciones
	\ Inacabado!!!
	(conversation_2_with_ambrosio)
	;
: (talk_to_ambrosio)  \ Hablar con Ambrosio

	\ Método nuevo en pruebas!!!
	ambrosio_e >conversations @  case
		0  of  conversation_0_with_ambrosio  endof
		1  of  conversation_1_with_ambrosio  endof
		2  of  conversation_2_with_ambrosio  endof
	endcase

	[false] [if]
	my_location@ case
		location_19_e  of  conversation_0_with_ambrosio  endof
		location_46_e  of  conversation_1_with_ambrosio  endof
	endcase
	[then]
	[false] [if]
	my_location@ case
		location_45_e  of  conversation_2_with_ambrosio  endof
		location_46_e  of  conversation_2_with_ambrosio  endof
		location_47_e  of  conversation_2_with_ambrosio  endof
	endcase
	[then]
	[false] [if]  \ Método alternativo poco legible, inacabado!!!
	location_45_e 1- location_47_e 1+ my_location@ within  if
		conversation_2_with_ambrosio
	then
	[then]
	;
: talk_to_ambrosio  \ Hablar con Ambrosio, si se puede
	\ Provisional!!! Esto debería comprobarse en DO_SPEAK o DO_SPEAK_IF_POSSIBLE
	ambrosio_e is_here?
	if  (talk_to_ambrosio)
	else  ambrosio_e is_not_here
	then
	;
: talk_to_something  ( a -- )  \ Hablar con un ente que no es un personaje 
	\ Pendiente!!!
	2 random
	if  drop nonsense
	else  >full_name@ s" hablar con" 2swap s& is_nonsense 
	then
	;
: talk_to_yourself  \ Hablar solo
	s" hablar solo" is_nonsense
	;
: do_speak_if_possible  ( a -- )  \ Hablar con un ente si es posible
	[debug] [if] s" En DO_SPEAK_IF_POSSIBLE" debug [then]  \ Depuración!!!
	direct_complement @  case
		0  of  talk_to_yourself  endof
		leader_e  of  talk_to_the_leader  endof
		ambrosio_e  of  talk_to_ambrosio  endof
		dup talk_to_something
	endcase
	;
: do_speak  \ Acción de hablar
	[debug] [if] s" En DO_SPEAK" debug [then]  \ Depuración!!!
	direct_complement @ ?dup
	if  do_speak_if_possible
	else  talk_to_yourself
	then
	;
' do_speak constant do_speak_xt

\ -------------------------------------------------------------
\ Terminar de jugar

defer actually_do_finish  \ Abandonar el juego
defer do_finish  \ Acción de abandonar el juego

\ }}}

\ ##############################################################
section( Intérprete de comandos)

\ {{{

0 [if]  \ ......................................

Gracias al uso del propio intérprete de Forth como
intérprete de comandos del juego, más de la mitad del
trabajo ya está hecha por anticipado.

Sin embargo hay una consideración importante: El intérprete
de Forth ejecutará las palabras en el orden en que estén
escritas en la frase del jugador. Esto quiere decir que no
podemos tener una visión global del comando del jugador: ni
de cuántas palabras consta ni, en principio, qué viene a
continuación de la palabra que está siendo interpretada en
cada momento.  Esta limitación hay que contrarrestarla con
algo de ingenio y con la extraordinaria flexibilidad de
Forth.

[then]  \ ......................................

\ -------------------------------------------------------------
\ Analizador

vocabulary player_vocabulary  \ Vocabulario para guardar en él las palabras del juego

: init_parsing  \ Preparativos previos al análisis
	action off
	direct_complement off
	;
\ : understood?  ( u -- f )  \ Comprueba si se ha producido un error en el comando; devuelve 0 si hubo
\ 	\ antiguo!!! no se usa!!!
\ 	\ u = Código de error, o cero si no se produjo un error
\ 	\ f = Cero si se produjo un error; -1 si no se produjo un error
\ 	[debug] [if] s" En UNDERSTOOD?" debug [then]  \ Depuración!!!
\ 	\ dup ?misunderstood 0= \ antiguo!!!
\ 	dup 0= swap ?misunderstood
\ 	;
: (call_action)  ( xt -- )  \ Ejecuta la acción del comando
	[debug] [if] s" En (CALL_ACTION)" debug [then]  \ Depuración!!!
	[debug_catch] [if] s" En (CALL_ACTION) antes de CATCH" debug [then]  \ Depuración!!!
	['] execute catch  ?misunderstood
	[debug_catch] [if] s" En (CALL_ACTION) después de CATCH" debug [then]  \ Depuración!!!
	[debug] [if] s" Al final de (CALL_ACTION)" debug [then]  \ Depuración!!!
	;
: call_action  \ Ejecuta la acción del comando, si existe
	[debug] [if] s" En CALL_ACTION" debug [then]  \ Depuración!!!
	[debug_catch] [if] s" En CALL_ACTION" debug [then]  \ Depuración!!!
\ 	\ Sistema antiguo!!!
\ 	action @ ?dup  if  execute
\ 	else  no_verb_error_id ?misunderstood
\ 	then
	action @ ?dup
	[debug_catch] [if] s" En CALL_ACTION tras ACTION @ ?DUP" debug [then]  \ Depuración!!!
	if  (call_action)
	else  no_verb_error_id ?misunderstood
	then
	[debug_catch] [if] s" Al final de CALL_ACTION" debug [then]  \ Depuración!!!
	[debug] [if] s" Al final de CALL_ACTION" debug [then]  \ Depuración!!!
	;
: valid_parsing?  ( a u -- f )  \ Evalúa un comando con el vocabulario del juego
	\ a u = Comando
	\ f = ¿El comando se analizó sin error?
	[debug] [if] s" Entrando en VALID_PARSING?" debug [then]  \ Depuración!!!
	only player_vocabulary  \ Dejar solo el diccionario PLAYER_VOCABULARY activo
	[debug_catch] [if] s" En VALID_PARSING? antes de CATCH" debug [then]  \ Depuración!!!
	['] evaluate catch  \ Llamar a EVALUATE a través de CATCH para poder regresar directamente en caso de error
	[debug_catch] [if] s" En VALID_PARSING? después de CATCH" debug [then]  \ Depuración!!!
	dup 0= swap ?misunderstood
	restore_vocabularies
	[debug] [if] s" Saliendo de VALID_PARSING?" debug [then]  \ Depuración!!!
	[debug_catch] [if] s" Saliendo de VALID_PARSING?" debug [then]  \ Depuración!!!
	;
: obbey  ( a u -- )  \ Evalúa un comando con el vocabulario del juego
	[debug] [if] s" Al entrar en OBBEY" debug [then]  \ Depuración!!!
	dup  if
		init_parsing valid_parsing?
		\ understood?  if  call_action  then \ antiguo!!!
		if  call_action  then
	else  2drop
	then
	[debug] [if] s" Al final de OBBEY" debug [then]  \ Depuración!!!
	; 
: second?  ( a1 a2 -- a1 f )  \ ¿La acción o el complemento son los segundos que se encuentran?
	\ a1 = Acción o complemento recién encontrado
	\ a2 = Acción o complemento almacenado, o cero
	2dup <> swap 0<> and  \ ¿Hay ya otro anterior y es diferente?
	;
: action!  ( a -- )  \ Comprueba y almacena la acción (la dirección de ejecución de su palabra) 
	action @ second?  \ ¿Había ya una acción?
	if  2-action_error_id throw  \ Sí, error
	else  action !  \ No, guardarla
	then
	;
: (complement!)  ( a -- )  \
	[debug] [if] s" En (COMPLEMENT!)" debug [then]  \ Depuración!!!
	other_complement @ second?  \ ¿Había ya un complemento secundario?
	if  2-complement_error_id throw  \ Sí, error
	else  other_complement !  \ No, guardarlo
	then
	;
: complement!  ( a -- )  \ Comprueba y almacena un complemento (la dirección de la ficha de su ente)
	[debug] [if] s" En COMPLEMENT!" debug [then]  \ Depuración!!!
	direct_complement @ second?  \ ¿Había ya un complemento directo?
	if  (complement!)
	else  direct_complement !
	then
	;
: action|complement!  ( a1 a2 -- )  \ Comprueba y almacena un posible complemento o una posible acción, significados ambos de la misma palabra
	\ a1 = Identificador de acción
	\ a2 = Identificador de ente
	action @  \ ¿Había ya una acción reconocida?
	if  nip complement!  \ Sí, luego tomamos el uso de complemento
	else  drop action!  \ No, luego tomamos el uso de acción
	then
	;

\ }}}

\ ##############################################################
section( Vocabulario)

\ {{{

0 [if]  \ ......................................

El vocabulario del juego está implementado como un
vocabulario de Forth, creado con el nombre de
PLAYER_VOCABULARY .  La idea es muy sencilla: crearemos en
este vocabulario nuevo palabras de Forth cuyos nombres sean
las palabras españolas que han de ser reconocidas en los
comandos del jugador. De este modo bastará interpretar la
frase del jugador con la palabra EVALUATE , que ejecutará
cada palabra que contenga el texto.

Pero hace falta algo más. Hay que evitar que el intérprete
de Forth dé error cuando encuentre en la frase del jugador
palabras desconocidas.  En lugar de añadir un sistema de
caza de errores en Forth (con las palabras CATCH y THROW ),
usaremos una opción más fácil: la palabra NOTFOUND .

La palabra NOTFOUND es una característica específica de
SP-Forth que simplifica el trabajo de usar el intérprete de
Forth como intérprete para otros usos, en este caso para
nuestro juego.  El intérprete de SP-Forth, cuando no
encuentra una palabra en los vocabularios activos, y antes
de intentar convertirla en un número en la base actual (que
es el procedimiento habitual en Forth), busca una palabra
llamada NOTFOUND en los mismos vocabularios activos y si la
encuentra la ejecuta, pasándole como parámetro en la pila
una cadena con el nombre de la palabra desconocida.

Por tanto, creando en nuestro vocabulario del juego una
palabra llamada NOTFOUND que no haga nada (salvo borrar de
la pila el parámetro que recibe) lograremos que durante la
interpretación del comando del jugador (con la palabra
EVALUATE ) todas las palabras no reconocidas sean ignoradas.
Para que esto funcione hace falta una cosa más: el único
vocabulario activo en el momento de ejecutar EVALUATE debe
ser el del juego; así las palabras de otros vocabularios
serán invisibles para el intérprete de Forth en el momento
de ejecutar EVALUATE . Al final del proceso hay que
restaurar el vocabulario principal del sistema, que se llama
FORTH , como vocabulario activo para la búsqueda de palabras
(aunque esto no es necesario mientras el programa está
funcionando, si interrumpiéramos el programa y el
vocabulario FORTH no estuviera activo el sistema Forth no
respondería a los comandos).

El uso de NOTFOUND tiene una pequeña pega: el jugador podría
escribir esta palabra, el intérprete la reconocería y, al
ejecutarla, haría que el sistema se colgase o se cerrase,
porque borraría de la pila los dos elementos (la cadena que
espera). Para evitar esto, habría que hacer que nuestra
versión de NOTFOUND distinguiera cuándo ha sido llamada
directamente por EVALUATE y cuándo por el intérprete del
sistema (que es lo mismo, pues solo hay un intérprete, pero
ejecutado a diferentes niveles). Podría hacerse, pero no es
fácil porque habría que adentrarse en el funcionamiento
interno de SP-Forth.  Al fin y al cabo, el jugador no tiene
por qué saber que hay una palabra llamada NOTFOUND .

[then]  \ ......................................

\ -------------------------------------------------------------
\ Palabras para crear sinónimos

: synonym:  ( xt "name" -- )  \ Crea un sinónimo de una palabra
	\ xt = Dirección de ejecución de la palabra a clonar
	nextword sheader  last-cfa @ !
	;
: synonyms:  ( xt u "name 1"..."name u" -- )  \ Crea uno o varios sinónimos de una palabra
	\ xt = Dirección de ejecución de la palabra a clonar
	\ u = Número de sinónimos que siguen en el flujo de entrada
	0  do  dup synonym:  loop  drop
	;

\ -------------------------------------------------------------
\ Resolución de ambigüedades

0 [if]  \ ......................................

Algunos nombres del vocabulario del jugador pueden referirse
a varios entes. Por ejemplo, «hombre» puede referirse al
jefe de los refugiados o a Ambrosio (especialmente antes de
que Ulfius hable con él por primera vez y sepa su nombre).

Otras palabras, como «ambrosio», solo deben ser reconocidas
cuando se cumplen ciertas condiciones.

Para estos casos creamos palabras que devuelven el ente
adecuado en función de las circunstancias.  Serán llamadas
desde la palabra correspondiente del vocabulario del
jugador.

Si la ambigüedad no puede ser resuelta, o si la palabra
ambigua no debe ser reconocida en las circunstancias de
juego actuales, se devolverá un FALSE , que tendrá el mismo
efecto que si la palabra problemática no existiera en el
comando del jugador. Esto provocará después el error
adecuado.

[then]  \ ......................................

: «man»>entity ( -- a | false )  \ Devuelve el ente adecuado a la palabra «hombre» y sus sinónimos (o FALSE si la ambigüedad no puede ser resuelta)
	leader_e is_here?  if
		\ El jefe de los refugiados tiene preferencia si está presente
		leader_e  
	else
		\ Si Ulfius no ha hablado con Ambrosio (y por tanto aún no sabe su nombre), la palabra se refiere a Ambrosio:
		ambrosio_e >conversations @ 0= abs ambrosio_e *
	then
	;
: «ambrosio»>entity ( -- a | false )  \ Devuelve el ente adecuado a la palabra «ambrosio» (o FALSE si la ambigüedad no puede ser resuelta)
	\ La palabra «Ambrosio» es válida solo si el protagonista ha hablado con Ambrosio:
	ambrosio_e >conversations @ 0> abs ambrosio_e *
	;

: «cave»>entity ( -- a | false )  \ Devuelve el ente adecuado a la palabra «cueva» (o FALSE si la ambigüedad no puede ser resuelta)
	\ Inacabado!!!
	cave_e
	;
: «part»>entity ( -- a true | xt false )  \ Devuelve el ente o acción adecuados a la palabra «parte»
	\ Pendiente!!!
	;

\ -------------------------------------------------------------
\ Vocabulario del juego

also player_vocabulary definitions  \ Elegir el vocabulario PLAYER_VOCABULARY para crear en él las nuevas palabras

\ **********************
\ Paso 6 de 6 para crear un nuevo ente:
\ Crear las palabras relacionadas con él en el vocabulario del jugador, y sus sinónimos
\ **********************

: ir do_go_xt action!  ;
' ir 8 synonyms: ve vete irse irte dirigirse dirígete muévete moverse

: abrir do_open_xt action!  ;
' abrir synonym: abre

: cerrar  do_close_xt action!  ;
' cerrar synonym: cierra

: coger  do_take_xt action!  ;
' coger 3 synonyms: coge recoger recoge
' coger 4 synonyms: tomar toma agarrar agarra

: dejar  do_drop_xt action!  ;
' dejar 5 synonyms: deja soltar suelta tirar tira

: mirar  do_look_xt action!  ;
' mirar 2 synonyms: m mira

: mirarte do_look_xt action! protagonist complement!  ;
' mirarte 4 synonyms: mírate mirarse mirarme mírame

: x  do_exits_xt action!  ;
: salida do_exits_xt exit_e action|complement!  ;
' salida synonym: salidas

: examinar  do_examine_xt action!  ;
' examinar 2 synonyms: ex examina

: examinarte  do_examine_xt action! protagonist complement!  ;
' examinarte 3 synonyms: examínate examinarme examinarse

: registrar  do_search_xt action!  ;
' registrar synonym: registra

: forth  actually_do_finish  ;  \ Depuración!!!
: bye  bye  ;  \ Depuración!!!
: quit quit  ;  \ Depuración!!!

: i  do_inventory_xt inventory_e action|complement!  ;
' i synonym: inventario
: inventariar  do_inventory_xt action!  ;
' inventariar 3 synonyms: inventaría registrarme registrarte

: nadar  do_swim_xt action!  ;
' nadar 3 synonyms: nada bucear bucea
' nadar 3 synonyms: sumérgete sumergirme sumergirse
' nadar 4 synonyms: zambullirse zambullirte zambullirme zambúllete
' nadar 4 synonyms: bañarte bañarse báñate bañarme

: quitarse  do_take_off_xt action!  ;
' quitarse 3 synonyms: quitarte quitarme quítate 
: ponerse  do_put_on_xt action!  ;
' ponerse 4 synonyms: ponerte ponte ponerme ponme
' ponerse 4 synonyms: colocarse colócate colocarme colócame

: matar  do_kill_xt action!  ;
' matar 3 synonyms: mata asesinar asesina
: golpear  do_hit_xt action!  ;
' golpear synonym: golpea
' golpear 2 synonyms: sacudir sacude
: atacar  do_attack_xt action!  ;
' atacar synonym: ataca
' atacar 2 synonyms: agredir agrede
: romper  do_break_xt action!  ;
' romper synonym: rompe
' romper 2 synonyms: despedaza despedazar
' romper 2 synonyms: quebrar quiebra
' romper 2 synonyms: dividir divide
' romper 2 synonyms: cortar corta
' romper 2 synonyms: desgarrar desgarra  \ Pendiente!!! Independizar.
' romper synonym: partir \ Pendiente!!! Desambiguar con «ir».
: afilar  do_sharpen_xt action!  ;
' afilar synonym: afila

: ulfius  ulfius_e complement!  ;
: ambrosio  «ambrosio»>entity complement!  ;
: altar  altar_e complement!  ;
: arco  arch_e complement!  ;
' arco synonym: puente
: capa  cloak_e complement!  ;
' capa synonym: lana
: coraza  cuirasse_e complement!  ;
' coraza synonym: armadura
: puerta  door_e complement!  ;
: esmeralda  emerald_e complement!  ;
' esmeralda synonym: joya
: derrumbe fallen_away_e complement!  ;
: banderas  flags_e complement!  ;
' banderas 2 synonyms: pendones enseñas
: pedernal  flint_e complement!  ;
: ídolo  idol_e complement!  ;
' ídolo 2 synonyms: ojo agujero
: llave  key_e complement!  ;
: lago  lake_e complement!  ;
' lago 2 synonyms: laguna agua
: candado  lock_e complement!  ;
' candado 2 synonyms: cierre cerrojo
: tronco  log_e complement!  ;
' tronco 2 synonyms: leño madero
: hombre  «man»>entity complement!  ;
' hombre 4 synonyms: señor tipo individuo persona
: jefe  leader_e complement!  ;
' jefe 3 synonyms: líder viejo anciano
: trozo  piece_e complement!  ;
' trozo 2 synonyms: pedazo retal
: parte  «part»>entity  ;  \ Inacabado!!!
: harapo  rags_e complement!  ;
: rocas  rocks_e complement!  ;
: serpiente  snake_e complement!  ;
' serpiente 3 synonyms: reptil ofidio culebra
: piedra stone_e complement!  ;
' piedra synonym: pedrusco
: espada  sword_e complement!  ;
' espada 2 synonyms: tizona arma
: hilo  thread_e complement!  ;
' hilo synonym: hebra
: antorcha  torch_e complement!  ;
: cascada  waterfall_e complement!  ;
' cascada synonym: catarata
: catre  bed_e complement!  ;
' catre 2 synonyms: cama camastro
: velas  candles_e complement!  ;
' velas synonym: vela
: mesa  table_e complement!  ;
' mesa 2 synonyms: mesita pupitre
: puente  bridge_e complement!  ;

: n  do_go_north_xt north_e action|complement!  ;
' n 2 synonyms: norte septentrión

: s  do_go_south_xt south_e action|complement!  ;
' s 2 synonyms: sur meridión

: e  do_go_east_xt east_e action|complement!  ;
' e 3 synonyms: este oriente levante

: o  do_go_west_xt west_e action|complement!  ;
' o 3 synonyms: oeste occidente poniente

: a  do_go_up_xt up_e action|complement!  ;
' a synonym: arriba
: subir  do_go_up_xt action!  ;
' subir 5 synonyms: sube ascender asciende subirte súbete

: b  do_go_up_xt up_e action|complement!  ;
' b synonym: abajo
: bajar  do_go_up_xt action!  ;
' bajar 5 synonyms: baja descender desciende bajarte bájate

: salir  do_go_out_xt action!  ;
' salir synonym: sal
: fuera  do_go_out_xt out_e action|complement!  ;
' fuera synonym: afuera
: entrar do_go_in_xt action!  ;
' entrar synonym: entra
: dentro  do_go_in_xt in_e action|complement!  ;
' dentro synonym: adentro

: escalar  do_climb_xt action!  ;
' escalar 3 synonyms: escala trepar trepa

: hablar  do_speak_xt action!  ;
\ Pendiente!!! Crear nuevas palabras según la preposición que necesiten:
' hablar synonym: habla 
' hablar 2 synonyms: háblale hablarle
' hablar 4 synonyms: conversar conversa charlar charla
' hablar 2 synonyms: preséntate presentarse
' hablar 4 synonyms: decir di decirle dile
' hablar 2 synonyms: platicar platica
' hablar 2 synonyms: platicarle platícale
' hablar 4 synonyms: contar cuenta contarle cuéntale 

: nubes  clouds_e complement!  ;
' nubes 3 synonyms: nube estratocúmulo estratocúmulos
: suelo  floor_e complement!  ;
' suelo 3 synonyms: suelos tierra firme
: cielo  sky_e complement!  ;
' cielo 4 synonyms: cielos firmamento cirro cirros
: techo  ceiling_e complement!  ;
: cueva  «cave»>entity complement!  ;
' cueva 2 synonyms: caverna gruta

: notfound  ( a u -- )  2drop  ;

restore_vocabularies

\ -------------------------------------------------------------
\ Vocabulario para entradas «sí» o «no»

0 [if]  \ ......................................

Para los casos en que el programa hace una pregunta que debe
ser respondida con «sí» o «no», usamos un truco análogo al
del vocabulario del juego: creamos un vocabulario específico
con palabras cuyos nombres sean las posibles respuestas
(«sí», «no», «s» y «n»).  Estas palabras actualizarán una
variable,  con cuyo valor el programa sabrá si se ha
producido una respuesta válida o no y cuál es.

En principio, si el jugador introdujera varias respuestas
válidas la última sería la que tendría efecto (por ejemplo,
la respuesta «sí sí sí sí sí no» sería considerada negativa.
Para dotar al método de una chispa de inteligencia, las
respuestas no cambian el valor de la variable sino que lo
incrementan o decrementan. Así el mayor número de respuestas
afirmativas o negativas decide el resultado; y si la
cantidad es la misma (por ejemplo, «sí sí no no») el
resultado será el mismo que si no se hubiera escrito nada.

[then]  \ ......................................

variable answer  \ Su valor será 0 si no ha habido respuesta válida; negativo para «no»; y positivo para «sí»
: answer_undefined  \ Inicializa la variable antes de hacer la pregunta
	answer off
	;
: answer_no  \ Indica que la respuesta fue negativa
	answer --
	;
: answer_yes  \ Indica que la respuesta fue afirmativa
	answer ++
	;

vocabulary yes/no_vocabulary  \ Nuevo vocabulario de Forth para analizar la respuesta a preguntas de «sí» o «no»
also yes/no_vocabulary definitions  \ Las palabras que siguen se crearán en dicho vocabulario

: sí  answer_yes  ;
: s  answer_yes  ;
: no  answer_no  ;
: n  answer_no  ;
: notfound  ( a u -- )  2drop  ;

restore_vocabularies

\ }}}

\ ##############################################################
section( Entrada) 

\ {{{

\ -------------------------------------------------------------
\ Entrada de comandos

0 [if]  \ ......................................

Para la entrada de comandos se usa la palabra de Forth
ACCEPT , que permite limitar el número máximo de caracteres
que serán aceptados (aunque por desgracia permite escribir
más y después trunca la cadena).

[then]  \ ......................................

80 constant /command  \ Longitud máxima de un comando
create command /command chars allot  \ Zona de almacenamiento del comando

: (wait_for_input)  ( -- a u )  \ Devuelve el comando introducido por el jugador
	input_color command /command accept  command swap  
	;
: .command_prompt  \ Imprime un presto para la entrada de comandos
	command_prompt_color indent ." >" space
	;
: wait_for_input  ( -- a u )  \ Imprime un presto y devuelve el comando introducido por el jugador
	.command_prompt (wait_for_input)
	;
: listen  ( -- a u )  \ Espera y devuelve el comando introducido por el jugador, formateado
	[status] [if] s" " debug [then]  \ Depuración!!!
	wait_for_input  -punctuation
	; 

\ -------------------------------------------------------------
\ Entrada de respuestas a preguntas de tipo «sí o no»

: evaluate_answer  ( a u -- )  \ Evalúa una respuesta a una pregunta del tipo «sí o no»
	only  yes/no_vocabulary
	evaluate 
	restore_vocabularies
	;
: answer@  ( a u -- u2 )  \ Devuelve el contenido de ANSWER tras formular una pregunta del tipo «sí o no»
	\ a u = Pregunta
	\ u2 = Contenido de la variable ANSWER
	answer_color cr type wait_for_input
	answer_undefined evaluate_answer  answer @
	;
: yes?  ( a u -- f )  \ ¿Es afirmativa la respuesta a una pregunta?
	\ a u = Pregunta
	\ f = ¿Es la respuesta positiva?
	answer@ 0>
	;
: no?  ( a u -- f )  \ ¿Es negativa la respuesta a una pregunta?
	\ a u = Pregunta
	\ f = ¿Es la respuesta negativa?
	answer@ 0<
	;

\ }}}

\ ##############################################################
section( Preparativos)

\ {{{

: init_entities/once  \ Preparación de la base de datos que se hace solo la primera vez
	wipe_entities  \ Poner las fichas a cero
	init_entity_strings  \ Cadenas dinámicas para los nombres
	init_direction_entities  \ Entes de dirección
	;
: init_entities/game  \ Preparación de la base de datos que se hace antes de cada partida
	\ Devolvemos a su estado original los datos
	\ que pudieran haber cambiado durante una partida:
	init_entity_names  \ Nombres
	init_entity_attributes  \ Atributos
	init_entity_descriptions  \ Descripciones
	init_entity_locations  \ Localizaciones
	;
: init_plot  \ Preparativos de las tramas
	init_plot_variables  \ Variables de las tramas
	init_location_plots  \ Tramas de los entes escenarios
	;
: init/once  \ Preparativos que hay que hacer solo una vez, antes de la primera partida
	init_screen/once  \ Pantalla
	init_entities/once  \ Entes
	;
: init/game  \ Preparativos que hay que hacer antes de cada partida
	randomize
	init_entities/game  \ Entes
	init_map  \ Mapa
	init_plot  \ Trama
	;

\ }}}

\ ##############################################################
section( Fin)

\ {{{

: .bye  \ Mensaje final cuando el jugador no quiere jugar otra partida
	\ Provisional!!!
	s" ¡Adiós!" narrate
	;
: do_bye  \ Abandona el programa
	clear_screen .bye bye
	;
: play_again?$  ( -- a u )  \ Devuelve la pregunta que se hace al jugador tras haber completado con éxito el juego
	s" ¿Quieres" s" ¿Te animas a" s" ¿Te apetece" 3 schoose
	s" jugar" s" empezar" 2 schoose s&  again?$ s&
	;
: retry?1$  ( -- a u )  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado
	s" ¿Tienes"
	s" fuerzas"
	s" arrestos"
	s" agallas" 
	s" energías"
	s" ánimos" 5 schoose s&
	;
: retry?2$  ( -- a u )  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado
	s" ¿Te quedan"
	s" ¿Guardas"
	s" ¿Conservas" 3 schoose
	s" fuerzas"
	s" energías"
	s" ánimos" 3 schoose s&
	;
: retry?$  ( -- a u )  \ Devuelve la pregunta que se hace al jugador tras haber fracasado
	retry?1$ retry?2$ 2 schoose
	s" para" s&
	s" jugar" s" probar" s" intentarlo" 3 schoose s&
	again?$ s&
	;
: enough?  ( -- f )  \ ¿Prefiere el jugador no jugar otra partida?
	success?  if  play_again?$  else  retry?$  then  cr+ no?
	;
: surrender?  ( -- f )  \ ¿Quiere el jugador dejar el juego?
	\ No se usa!!!
	s" ¿Quieres"
	s" ¿En serio quieres"
	s" ¿De verdad quieres"
	s" ¿Estás segur" player_o/a+ s" de que quieres" s& 
	s" ¿Estás decidid" player_o/a+ s" a" s& 5 schoose
	s" dejarlo?"
	s" rendirte?"
	s" abandonar?" 3 schoose s&  yes? 
	;
: game_over?  ( -- f )  \ ¿Se terminó ya el juego?
	success? failure? or
	;
: the_happy_end  \ Final del juego con éxito
    s" Agotado, das parte en el castillo de tu llegada"
	s" y de lo que ha pasado." s&
	narrate  short_pause
    s" Pides audiencia al rey, Uther Pendragon."
	narrate  end_of_scene
	castilian_quotes? @
	if  \ Comillas castellanas
		s" El rey" rquote$ s+ s" , te indica el valido," s+
		lquote$ s" ha ordenado que no se le moleste," s+ s&
	else  \ Raya
		s" El rey" 
		dash$ s" te indica el valido" dash$ comma+ s+ s+ s&
		s" ha ordenado que no se le moleste," s&
	then
	s" pues sufre una amarga tristeza." s&
	speak  short_pause
    s" No puedes entenderlo. El rey, tu amigo."
	narrate  short_pause
    s" Agotado, decepcionado, apesadumbrado,"
	s" decides ir a dormir a tu casa." s&
	s" Es lo poco que puedes hacer." s&
	narrate  short_pause
    s" Te has ganado un buen descanso."
	narrate
	;
: the_sad_end  \ Final del juego con fracaso
    s" Los sajones te"
	s" hacen prisionero"
	s" capturan"
	s" atrapan" 3 schoose s& period+
	s" Su general, sonriendo ampliamente, dice:" s&
	narrate  short_pause
	s" Hoy parece ser" 
	s" Hoy sin duda es"
	s" No cabe duda de que hoy es"
	s" Hoy es" 4 schoose s" mi día de suerte..." s&
	s" Bien, bien..."
	s" Excelente..." 2 schoose s&
	s" Por el gran Ulfius podremos pedir un buen rescate."
	s" Del gran Ulfius podremos sacar una buena ventaja."
	2 schoose s&  speak
	;
: the_end  \ Mensaje final del juego
	success?  if  the_happy_end  else  the_sad_end  then
    end_of_scene 
	;

\ -------------------------------------------------------------
\ Acción de terminar de jugar

: (actually_do_finish)  \ Abandonar el juego
	restore_vocabularies default_color cr (title) quit
	;
' (actually_do_finish) is actually_do_finish
: (do_finish)  \ Acción de abandonar el juego
	surrender?  if
		\ retry?$  cr+ no?  if  actually_do_finish  then
		actually_do_finish  
	then
	;
' (do_finish) is do_finish

\ }}}

\ ##############################################################
section( Acerca del programa)

\ {{{

: based_on  \ Muestra un texto sobre el programa original
	s" «Asalto y castigo» (escrito en SP-Forth) está basado"
	s" en el programa homónimo escrito en BASIC en 2009 por"s&
	s" Baltasar el Arquero (http://caad.es/baltasarq/)." s&
	paragraph
	;
: license  \ Muestra un texto sobre la licencia
	s" (C) 2011 Marcos Cruz (programandala.net)" paragraph
	s" «Asalto y castigo» (escrito en SP-Forth) es un programa libre;"
	s" puedes distribuirlo y/o modificarlo bajo los términos de" s&
	s" la Licencia Pública General de GNU, tal como está publicada" s&
	s" por la Free Software Foundation (Fundación para los Programas Libres)," s&
	s" bien en su versión 2 o, a tu elección, cualquier versión posterior" s&
	s" (http://gnu.org/licences/)." s& \ Confirmar!!!
	paragraph
	;
: program  \ Muestra el nombre y versión del programa
	s" «Asalto y castigo» (escrito en SP-Forth)" paragraph/
	s" Versión " version$ s& paragraph/
	;
: about  \ Muestra información sobre el programa
	clear_screen about_color
	program	license based_on
	end_of_scene
	;

\ }}}

\ ##############################################################
section( Introducción)

\ {{{

: intro_0  \ Muestra la introducción al juego (parte 0)
	s" El sol despunta de entre la niebla,"
	s" haciendo humear los tejados de paja." s&
	narrate  short_pause
	;
: intro_1  \ Muestra la introducción al juego (parte 1)
	s" Piensas en el encargo realizado por"
	s" Uther Pendragon." s" tu rey." 2 schoose s& \ tmp!!!
	s" Atacar una aldea tranquila," s&
	s" aunque sea una llena de sajones," s&
	s" no te llena" s" no te colma" 2 schoose s&
	s" de orgullo." s&
	narrate  short_pause
	;
: intro_2  \ Muestra la introducción al juego (parte 2)
	s" Los hombres se ciernen sobre la aldea,"
	s" y la destruyen." s&
	s" No hubo tropas enemigas, ni honor en la batalla." s&
	narrate  end_of_scene
	;
: intro_3  \ Muestra la introducción al juego (parte 3)
	s" Sire Ulfius,"
	s" el asalto"
	s" el combate"
	s" la batalla"
	s" la lucha"
	s" todo" 5 schoose s&
	s" ha terminado."
	s" ha concluido." 2 schoose s&
	speak
	;
: intro_4  \ Muestra la introducción al juego (parte 4)
	s" Lentamente,"
	s" ordenas"
	s" das la orden de"
	s" das las órdenes para"
	s" das las órdenes necesarias para"
	4 schoose s&
	s" volver" s" regresar" 2 schoose s&
	s" a casa." s&
	narrate short_pause
	;
: intro_5  \ Muestra la introducción al juego (parte 5)
	s" Los" s" Tus" 2 schoose
	s" oficiales" s&
	s" intentan detener"
	s" detienen como pueden"
	s" hacen lo que pueden"
	s" hacen todo lo que pueden"
	s" hacen lo imposible" 3 schoose
	s" para detener" s" por detener" 2 schoose s&
	3 schoose s&
	s" el saqueo." s&
	narrate  end_of_scene
	;
: intro  \ Muestra la introducción al juego 
	clear_screen
	intro_0 intro_1 intro_2 intro_3 intro_4 intro_5
	;

\ }}}

\ ##############################################################
section( Configuración)

\ {{{

s" ayc.ini" sconstant config_file$  \ Nombre del fichero de configuración

vocabulary config_vocabulary  \ Vocabulario en que se crearán las palabras de configuración del juego

also config_vocabulary  definitions

\ Las palabras cuyas definiciones siguen a continuación
\ se crearán en el vocabulario CONFIG_VOCABULARY y
\ son las únicas que podrán usarse para configurar el juego
\ en el fichero de configuración:

: (  ( "texto<cierre de paréntesis>" -- ) \ Comentario clásico
	postpone ( 
	; immediate
: \  ( "texto<fin de línea>" -- ) \ Comentario de línea
	postpone \ 
	; immediate

: columnas  ( u -- )  \ Cambia el número de columnas
	1- to max_x
	;
: líneas ( u -- )  \ Cambia el número de líneas
	1- to max_y
	;

: varón  \ Indica que el jugador es un varón
	woman_player? off
	;
' varón alias hombre
' varón alias masculino
' varón alias macho
' varón alias chico
' varón alias señor
: mujer  \ Indica que el jugador es una mujer
	woman_player? on
	;
' mujer alias femenino
' mujer alias hembra
' mujer alias chica
' mujer alias señora

: raya  \ Indica que se use la raya en las citas 
	castilian_quotes? off
	;
: comillas  \ Indica que se usen las comillas castellanas en las citas
	castilian_quotes? on
	;
: indentación  ( u -- )  \ Fija la indentación de los párrafos
	max_indentation min /indentation !
	;
: borrar_pantalla_para_escenarios  \ Indica que se borre la pantalla al entrar en un escenario o describirlo
	location_page? on
	;
: borrar_pantalla_tras_escenas  \ Indica que se borre la pantalla tras la pausa de fin de escena
	scene_page? on
	;
: párrafos_separados  \ Indica que se separen los párrafos con un línea en blanco
	cr? on
	;

\ Nota!!!: no se puede definir NOTFOUND así porque los números no serían reconocidos:
\ : notfound  ( a u -- )  2drop  ;

\ Fin de las palabras permitidas en el fichero configuración.

restore_vocabularies

: init_config  \ Inicializa las variables de configuración
	woman_player? off
	castilian_quotes? on
	location_page? off
	cr? off
	ignore_unknown_words? off
	4 /indentation !
	scene_page? off
	;

: read_config  \ Lee el fichero de configuración
	only config_vocabulary
	[ config_file$ ] sliteral included
	restore_vocabularies
	;

\ }}}

\ ##############################################################
section( Principal)

\ {{{

: game  \ Bucle del juego
	begin
		plot  listen obbey
	game_over? until
	;
: game_preparation  \ Preparación de la partida
	init/game
	init_config read_config
	\ about cr intro  \ Anulado para depuración!!!
	\ location_01_e enter  \ Anulado para depuración!!!
	location_08_e enter  \ Depuración!!!
	;
: (main)  \ Palabra principal que arranca el juego
	init/once
	begin
		game_preparation game the_end
	enough? until
	do_bye
	;
' (main) is main
' main alias ayc
' main alias go
' main alias run

: i0  \ Hace toda la inicialización
	\ Palabra temporal para la depuración del programa!!!
	init/once game_preparation
	." Datos preparados."
	;

i0 cr  \ Para depuración!!!
\ main

\ }}}


\eof  \ Final del programa; el resto del fichero es ignorado por SP-Forth

0 [if]  \ ......................................
[then]  \ ......................................
