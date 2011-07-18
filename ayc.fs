\ #! /usr/bin/spf4

\ #############################################################
cr .( Asalto y castigo )

\ A text adventure in Spanish, written in SP-Forth.
\ Un juego conversacional en castellano, escrito en SP-Forth.

: version$  s" A-20110717"  ;  version$ type cr

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

\ #############################################################
\ Tareas de programación pendientes

0 [if]  \ ......................................

Limitar los usos de PRINT_STR a la impresión. Renombrarla.
Crear otra cadena dinámica para los usos genéricos con «+ y
palabras similares.

Comprobar los usos de TMP_STR .

Implementar indentación opcional y configurable en los
párrafos, con caracteres comodín que serán traducidos a
espacios antes de imprimir la primera línea del párrafo.

Poner en fichero de configuración el número de líneas
necesario para mostrar un presto de pausa.

Poner en fichero de configuración los colores de los textos.

Unificar la terminología: localización / lugar /escenario.

[then]  \ ......................................

\ #############################################################
\ Puzles pendientes

0 [if]  \ ......................................

Quitarse la coraza o la capa antes de nadar (ambas son
demasiado pesadas para cruzar el lago con 100% de éxito)

No poder nadar si llevamos algo en las manos aparte de la
espada.

Perder la capa al nadar si no la llevamos puesta.

[then]  \ ......................................

\ #############################################################
\ Novedades significativas respecto a la versión en SuperBASIC y a las versiones originales

0 [if]  \ ......................................

Todos los objetos citados en las descripciones exiten como
tales (p.e. catre, puente, pared...) y por tanto son
manipulables, pero no se incluyen en la lista de los objetos
presentes.

Objetos globales (p.e. cielo, suelo, techo, pared...)

Las direcciones de salida son objetos virtuales, lo que
permite expresiones como «ve al norte» o «mira hacia el sur».

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
\ Véase:
\ http://programandala.net/es.programa.csb2

\ ##############################################################
\ Inicio

vocabulary ayc_vocabulary  \ Vocabulario en que se crearán las palabras del programa (no se trata del vocabulario del jugador)
: restore_vocabularies  \ Restaura los vocabularios FORTH y AYC_VOCABULARY a su orden habitual
	only also  ayc_vocabulary  definitions
	;
restore_vocabularies

\ ##############################################################
cr .( Depuración)

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
	." [Pila:]" .s ." ( " depth . ." )"
	;
: .csb  \ Imprime el estado del almacén circular de cadenas
	." [Espacio para cadenas:]" csb @ .
	;
false value [debug] immediate
defer debug_color
: .system_status  \ Muestra el estado del sistema
	.csb .stack
	;
: .debug_message  ( a u -- )  \ Imprime el mensaje del punto de chequeo, si no está vacío
	dup  if  cr type cr  else  2drop  then
	;
: debug_pause  \ Pausa tras mostrar la información de depuración
	\ key drop
	;
: debug  ( a u -- )  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema
	debug_color .debug_message .system_status debug_pause
	;

\ ##############################################################
cr .( Palabras genéricas)

: drops  ( x1..xn n -- )  \ Elimina n celdas de la pila
	0  do  drop  loop
	;
: sconstant  ( a1 u "name" -- )  \ Crea una constante de cadena
	create  dup c, s, align
	does>  ( -- a2 u )  count
	;
: alias ( xt "name" -- )  \ Crea un alias de una palabra
	\ Versión modificada de la palabra homónima provista por
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

\ ##############################################################
cr .( Constantes)

false constant [0] immediate  \ Para usar en comentarios multilínea con [IF] dentro de las definiciones de palabras

\ ##############################################################
cr .( Números aleatorios)

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

\ ##############################################################
cr .( Variables)

\ Variables de configuración

variable woman_player?  \ Indica si el jugador es una mujer; se usa para cambiar el género gramatical en algunos mensajes.
woman_player? off
variable castilian_quotes?  \ Indica si se usan comillas castellanas en las citas, en lugar de raya.
castilian_quotes? on
variable location_page?  \ Indica si se borra la pantalla antes de entrar en un escenario o de describirlo.
location_page? on
 
\ Variables de la trama

variable battle#  \ Contador de la evolución de la batalla (si aún no ha empezado, es cero)
variable ambrosio_follows?  \ ¿Ambrosio sigue al protagonista?
variable talked_to_the_man?  \ ¿El protagonista ha hablado con el hombre?
variable hacked_the_log?  \ ¿El protagonista ha afilado el tronco?
variable hold#  \ Contador de cosas llevadas por el protagonista (no usado!!!)

: init_plot_variables  \ Inicializa las variables de la trama
	battle# off
	ambrosio_follows? off
	talked_to_the_man? off
	hacked_the_log? off
	;

\ ##############################################################
cr .( Pantalla)

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
: credits_color  \ Pone el color de texto de los créditos
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
: init_screen/once  \ Prepara la pantalla la primera vez
	trm+reset init_cursor default_color home
	;

\ ##############################################################
cr .( Manipulación de textos)

str-create tmp_str  \ Cadena dinámica de texto temporal para usos variados

: str-get-last-char  ( a -- c )  \ Devuelve el último carácter de una cadena dinámica
	dup str-length@ 1- swap str-get-char 
	;
: str-get-last-but-one-char  ( a -- c )  \ Devuelve el penúltimo carácter de una cadena dinámica
	dup str-length@ 2- swap str-get-char 
	;

: ^uppercase  ( a u -- )  \ Convierte en mayúsculas la primera letra de una cadena
	\ No funciona con UTF8!!!
	if  dup c@ char-uppercase swap c!
	else  drop
	then
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
: *>verb_ending  ( a u f -- )  \ Cambia por «n» los asteriscos de un texto, o los quita.
	\ Se usa para convertir en plural o singular los verbos de una frase.
	\ a u = Expresión.
	\ f = ¿Hay que poner los verbos en plural?
	>r  tmp_str!
	r>  if  s" n"  else  s" "  then
	s" *" tmp_str str-replace  tmp_str@
	;

\ ##############################################################
cr .( Textos variables) 

: our_hero$  ( -- a u ) \ Devuelve el nombre del protagonista
	\ Pendiente!!! Hacer que varíe según la condiciones del juego
	s" Ulfius"
	s" nuestro héroe"
	s" nuestro hombre" 
	s" nuestro protagonista" 4 schoose
	;
: ^our_hero$  ( -- a u ) \ Devuelve el nombre del protagonista, con la primera letra en mayúscula
	our_hero$ 2dup ^uppercase
	;
: old_man$  ( -- a u )  \ Devuelve una forma de llamar al personaje del viejo
	s" hombre" s" viejo" s" anciano" 3 schoose
	;
: with_him$  ( -- a u )
	s" "
	s" consigo"
	s" encima" 3 schoose
	;
: carries$  ( -- a u )
	s" tiene"
	s" lleva"
	s" porta" 3 schoose
	;
: now$  ( -- a u )
	s" "
	s" en este momento"
	s" en estos momentos"
	s" ahora" 4 schoose
	;
: again?$  ( -- a u )
	s" de nuevo?"
	s" otra vez?"
	s" una vez más?" 3 schoose
	;

: player_o/a  ( -- a 1 )  \ Devuelve la terminación «a» u «o» según el sexo del jugador
	[ 0 ] [if]
		\ Método 1, «estilo BASIC»:
		woman_player? @  if  s" a"  else  s" o"  then
	[else]
		\ Método 2, sin estructuras condicionales, «estilo Forth»:
		c" oa" woman_player? @ abs + 1+ 1
	[then]
	;

: all_your$  ( -- a u )  \ Devuelve una variante de «Todos tus».
	s" Todos tus" s" Tus" 2 schoose
	;
: soldiers$  ( -- a u )  \ Devuelve una variante de «soldados».
	s" hombres" s" soldados" 2 schoose 
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
: ^the_enemy/enemies  ( -- a u f )  \ Devuelve una cadena con una variante de «El/Los enemigo/s», y un indicador del número
	\ a u = Cadena con el texto
	\ f = ¿El texto está en plural?
	(the_enemy/enemies) >r  2dup ^uppercase  r>
	;
\ ##############################################################
cr .( Textos concatenados)

: period+  ( a1 u1 -- a2 u2 )  \ Añade un punto final a una cadena
	s" ." s+
	;
: player_o/a+  ( a1 u1 -- a2 u2 )  \ Añade la terminación «a» u «o» según el sexo del jugador
	player_o/a s+
	;

\ ##############################################################
cr .( Cadena dinámica multiusos)

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

\ ##############################################################
cr .( Impresión de textos)

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
	;

\ -------------------------------------------------------------
\ Impresión de párrafos ajustados

: .line  ( a u -- )  \ Imprime una línea de texto y un salto de línea
	s" " s+ type cr
	;
: .lines  ( a1 u1 ... an un n -- )  \ Imprime n líneas de texto
	\ a1 u1 = Última línea de texto
	\ an un = Primera línea de texto
	\ n = Número de líneas de texto en la pila
	dup #lines !  scroll on
	0  ?do  .line  i .pause_prompt?  loop
	;
: (tell)  \ Imprime la cadena dinámica PRINT_STR ajustándose al ancho de la pantalla
	print_str str-get max_x str+columns  \ Divide la cadena dinámica PRINT_STR en tantas líneas como haga falta
	.lines  \ Las imprime
	print_str str-init  \ Vacía la cadena dinámica
	\ default_color
	;
: paragraph/ ( a u -- )  \ Imprime una cadena ajustándose al ancho de la pantalla
	print_str str-set (tell)
	;
: paragraph  ( a u -- )  \ Imprime una cadena ajustándose al ancho de la pantalla; y una separación posterior si hace falta
	dup >r  paragraph/  r>  if  cr  then
	;
: report  ( a u -- )  \ Imprime una cadena como un informe de error
	error_color paragraph
	;
: narrate  ( a u -- )  \ Imprime una cadena como narración
	narration_color paragraph
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
: quotes+  ( a1 u1 -- a2 u2 )  \ Añade comillas castellanas a una cita
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
: dash+  ( a1 u1 -- a2 u2 )  \ Añade la raya a una cita
	dash$ 2swap s+ 
	;
: speak  ( a u -- )  \ Imprime el texto de una cita
	castilian_quotes? @
	if  quotes+
	else  dash+
	then  speak_color paragraph
	;

\ ##############################################################
cr .( Opciones del juego)

\ Pendiente!!!

\ true value clear_screen_coming?  \ 


\ ##############################################################
cr .( Entes)

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

: exit?  ( a -- )  \ ¿Está abierta una dirección de salida de un ente escenario?
	no_exit <>
	;
: no_exit?  ( a -- )  \ ¿Está cerrada una dirección de salida de un ente escenario?
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

Su funcionamiento es muy sencillo. Toma de la pila dos
valores: el inferior es el desplazamiento en octetos desde
el inicio del «registro» (que en este programa denominamos
«ficha»); el superior es el número de octetos necesarios
para almacenar el campo a crear. Con ellos crea una palabra
nueva (cuyo nombre es tomado del flujo de entrada, es decir,
es la siguiente palabra en la línea) que será el
identificador del campo de datos; esta palabra guardará en
su interior el desplazamiento del campo de datos desde el
inicio de la ficha de datos, y cuando sea ejecutada lo
sumará al número de la parte superior de la pila, que deberá
ser la dirección en memoria de la ficha.

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
estándar para leer y escribir la memoria.

[then] \ .......................................

0 \ Valor inicial de desplazamiento para el primer campo
\ cell field >desambiguation_xt  \ Dirección de ejecución de la palabra que desambigua e identifica el ente (aún no se usa!!!)
cell field >name_str  \ Dirección de una cadena dinámica que contendrá el nombre del ente
cell field >description_xt  \ Dirección de ejecución de la palabra que describe el ente
cell field >feminine?  \ Indicador: ¿el género gramatical del nombre es femenino?
cell field >plural?  \ Indicador: ¿el nombre es plural?
cell field >no_article?  \ Indicador: ¿el nombre no debe llevar artículo?
cell field >definite_article?  \ Indicador: ¿el artículo debe ser siempre el artículo definido?
cell field >character?  \ Indicador: ¿el ente es un personaje?
cell field >decoration?  \ Indicador: ¿el ente forma parte de la decoración de su lugar?
cell field >global_outside?  \ Indicador ¿el ente es global (común) en los lugares al aire libre?
cell field >global_inside?  \ Indicador ¿el ente es global (común) en los lugares interiores? 
cell field >owned?  \ Indicador: ¿el ente pertenece al protagonista? 
cell field >cloth?  \ Indicador: ¿el ente es una prenda que puede ser llevada como puesta?
cell field >worn?  \ Indicador: ¿el ente, que es una prenda, está puesto? 
\ cell field >vegetal?  \ Indicador: ¿es un vegetal?
\ cell field >animal?  \ Indicador: ¿es un animal? 
\ cell field >container?  \ Indicador: ¿es un contenedor? 
\ cell field >open?  \ Indicador: ¿está abierto? 
cell field >location?  \ Indicador: ¿es un lugar? 
cell field >location  \ Identificador del ente en que está localizado (sea lugar, contenedor, personaje o «limbo»)
cell field >location_plot_xt  \ Dirección de ejecución de la palabra que se ocupa de la trama del lugar 
\ cell field >stamina  \ Energía de los entes vivos
cell field >familiar  \ Contador de familiaridad (cuánto le es conocido el ente al protagonista)
cell field >north_exit  \ Ente de destino hacia el norte
' >north_exit alias >first_exit
cell field >south_exit  \ Ente de destino hacia el sur
cell field >east_exit  \ Ente de destino hacia el este
cell field >west_exit  \ Ente de destino hacia el oeste
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

Los identificadores de entes se crean con la palabra ENTITY:
.  Cuando se ejecutan devuelven la dirección en memoria de
su ficha en la base de datos, que después puede ser
modificada con un identificador de campo para convertirla en
la dirección de memoria de una campo concreto de la ficha.

Para reconocer mejor los identificadores de entes se usa el
sufijo _e en sus nombres.

[then]  \ ......................................

\ **********************
\ Paso 1/6 para crear un nuevo ente:
\ Crear su identificador
\ **********************

\ El ente protagonista debe ser el primero (el orden de los restantes es indiferente):
entity: ulfius_e
' ulfius_e is protagonist  \ Actualizar el vector que apunta al ente protagonista

\ Entes que son personajes: 
entity: ambrosio_e
entity: man_e

\ Entes que son objetos:
entity: altar_e
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

\ Entes que son escenarios
\ (en lugar de usar su nombre en el identificador,
\ se conserva el número que tienen en la versión
\ original; para que algunos cálculos tomados del código
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

Otras por el contrario actúan como procedimientos para
realizar operaciones frecuentes con los entes.

[then]  \ ......................................

create 'articles  \ Tabla de artículos
	s" un  " s,
	s" una " s,
	s" unos" s,
	s" unas" s,
	s" el  " s,
	s" la  " s,
	s" los " s,
	s" las " s,
	s" mi" s,  \ Inacabado!!!
	s" mi" s,
	s" mis" s,
	s" mis" s,
4 constant /article  \ Longitud máxima de un artículo en la tabla, con sus espacios finales
1 /article * constant /article_gender_set  \ Separación entre cada grupo según el género (masculino y femenino)
2 /article * constant /article_number_set  \ Separación entre cada grupo según el número (singular y plural)
4 /article * constant /article_type_set  \ Separación entre cada grupo según el tipo (definidos e indefinidos)

: on_article_number  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el número gramatical del ente
	>plural? @ abs /article_number_set *
	;
: on_article_gender  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el género gramatical del ente
	>feminine? @ abs /article_gender_set *
	;
: on_article_type  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el ente requiera un artículo definido o indefinido
	dup >definite_article? @  \ Si el ente necesita siempre artículo definido
	swap >familiar @ 0<>  or  \ O bien si el ente es ya familiar al protagonista
	abs /article_type_set *
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
: >noun_ending@  ( a -- a1 u1 )  \ Devuelve la terminación adecuada para el nombre de un ente
	dup >feminine?  if  s" a"  else  s" o"  then
	rot >plural?  if  s" s"  else  s" "  then  s+
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
	dup >r >name!  r> >plural? on
	;
: >location_name!  ( a u a1 -- )  \ Guarda el nombre de un ente y marca el ente como escenario
	\ No se usa!!!
	\ a u = Nombre
	\ a1 = Ente
	dup >r >name!  r> >location? on
	;
: >fname!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino
	\ a u = Nombre
	\ a1 = Ente
	dup >r >name!  r> >feminine? on
	;
: >fnames!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino y plural
	\ a u = Nombre
	\ a1 = Ente
	dup >r >fname!  r> >plural? on
	;
: >location_fname!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino; y marca el ente como escenario
	\ No se usa!!!
	\ a u = Nombre
	\ a1 = Ente
	dup >r >fname!  r> >location? on
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
: is_here?  ( a -- f )  \ ¿Está ente en el mismo lugar que el protagonista?
	\ El resultado depende de cualquiera de tres condiciones:
	dup where my_location@ =  \ ¿Está efectivamente en el mismo lugar?
	over >global_outside? @ am_i_outside? and or \ ¿O es un «global exterior» y estamos en un lugar exterior?
	swap >global_inside? @ am_i_inside? and or  \ ¿O es un «global interior» y estamos en un lugar interior?
	;
: be_here  ( a -- )  \ Hace que un ente esté en el mismo lugar que el protagonista
	>location my_location@ swap !
	;
: is_accessible?  ( a -- f )  \ ¿Es un ente accesible para el protagonista?
	dup is_hold?  swap is_here?  or
	;
: can_be_looked_at?  ( a -- )  \ ¿El ente puede ser mirado?
	dup my_location@ =  \ ¿Es el lugar del protagonista?
	over is_here? or  \ ¿O está en el lugar del protagonista?
	swap is_hold?  or  \ ¿O lo tiene el protagonista?
	;

: more_familiar  ( a -- )  \ Aumenta el grado de familiaridad de un ente con el protagonista
	swap >familiar ++
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
\ Paso 2/6 para crear un nuevo ente:
\ Crear su nombre (el que se usará al citarlo)
\ **********************

: init_entity_names  \ Guarda en las fichas los nombres predeterminados y sus atributos gramaticales

	\ Ente protagonista:
	s" Ulfius" ulfius_e >name!

	\ Entes personajes: 
	s" Ambrosio" ambrosio_e >name!

	\ Entes objetos:
	s" altar" altar_e >name!
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
	s" hombre" man_e >name!
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

	\ Entes virtuales:
	s" norte" north_e >name!  north_e >definite_article? on
	s" sur" south_e >name!  south_e >definite_article? on
	s" este" east_e >name!  east_e >definite_article? on
	s" oeste" west_e >name!  west_e >definite_article? on
	s" arriba" up_e >name!  up_e >no_article? on
	s" abajo" down_e >name!  down_e >no_article? on
	s" afuera" out_e >name!  out_e >no_article? on
	s" adentro" in_e >name!  in_e >no_article? on

	;

\ -------------------------------------------------------------
\ Atributos

\ **********************
\ Paso 3/6 para crear un nuevo ente:
\ Definir sus atributos
\ **********************

: init_entity_attributes  \ Guarda en las fichas los atributos de los entes (salvo los lingüísticos)
	ambrosio_e >character? on
	cloak_e >cloth? on
	cloak_e >owned? on
	cloak_e >worn? on
	cuirasse_e >cloth? on
	cuirasse_e >owned? on
	cuirasse_e >worn? on
	fallen_away_e >decoration? on
	lake_e >decoration? on
	man_e >character? on
	rocks_e >decoration? on
	sword_e >owned? on
	;

\ -------------------------------------------------------------
cr .( Descripciones)

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

: :description  ( a -- xt a ) \ Crea una palabra sin nombre que describirá un ente
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
	>name@ 2dup ^uppercase location_name_color paragraph
	;
: describe_location  ( a -- )  \ Imprime la descripción de un ente escenario
	[debug] [if] s" En DESCRIBE_LOCATION" debug [then]  \ Depuración!!!
	location_page? @  if  clear_screen  then
	dup .location_name location_description_color dup (describe)
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
\ Paso 4/6 para crear un nuevo ente:
\ Crear una palabra de descripción, con la sintaxis específica
\ **********************

\ El ente protagonista:

ulfius_e :description  \ Describe el ente ulfius_e
	\ tmp!!!
	my_location@ is_outside?
	if   s" De pie al sol,"
	else  s" En las sombras,"
	then  our_hero$ s&
	s" no parece muy fuerte, a pesar de tener un metro sesenta y cinco de estatura."
	my_location@ is_outside?  if
		s" Después de un invierno benigno, la primavera ha traído tal calor que" s&
		ulfius_e >name@ s&
		s" se halla a gusto sin vestidos." s& 
	then  paragraph
	;description

\ Los entes personajes: 

ambrosio_e :description
	s" Ambrosio es un hombre de mediana edad, que te mira afable."
	paragraph
	;description
man_e :description
	s" Es el jefe de los refugiados."
	paragraph
	;description

\ Los entes objetos:

altar_e :description
	s" Justo en la mitad del puente, debe sostener algo importante."
	paragraph
	;description
torch_e :description
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
	s" Legado de tu padre, fiel herramienta en mil batallas."
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
	s" La luz entra por un resquicio, y caprichosos reflejos te maravillan."
	paragraph
	;description
key_e :description
	s" Una llave grande, de hierro herrumboso."
	paragraph
	;description
flint_e :description
	s" Se trata de una dura y afilada piedra."
	paragraph
	;description
stone_e :description
	s" Recia y pesada, pero no muy grande, de forma piramidal."
	paragraph
	;description
door_e :description
	s" Muy recia y con un gran candado."
	paragraph
	;description
rocks_e :description
	s" Son muchas, aunque parecen ligeras y con huecos entre ellas."
	paragraph
	;description
snake_e :description
	s" Una serpiente bloquea el paso al sur, corriendo a su lado el agua."
	paragraph
	;description
log_e :description
	s" Es un tronco recio, pero de liviano peso."
	paragraph
	;description
piece_e :description
	s" Es un poco de lo que antes era tu capa."
	paragraph
	;description
lock_e :description
	s" Está cerrado. Es muy grande y parece resistente."
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
	s" Solo resta volver al sur, a casa." s&
	paragraph
	;description
location_02_e :description
	s" Sobre la colina, casi sobre la niebla de la aldea sajona arrasada al norte, a tus pies."
	s" El camino desciende hacia el oeste." s&
	paragraph
	;description
location_03_e :description
	s" El camino avanza por el valle, desde la parte alta, al este, a una zona harto boscosa, al oeste."
	paragraph
	;description
location_04_e :description
	s" Una senda parte al oeste, a la sierra por el paso del Perro, y otra hacia el norte, por un frondoso bosque que la rodea."
	paragraph
	;description
location_05_e :description
	s" Desde la linde, al sur, hacia el oeste se extiende frondoso el bosque que rodea la sierra. La salida se abre hacia el sur."
	paragraph
	;description
location_06_e :description
	s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
	s" La senda serpentea entre raíces, de un luminoso este al oeste." s&
	paragraph
	;description
location_07_e :description
	s" Abruptamente, del bosque se pasa a un estrecho camino entre altas rocas.
	s" El inquietante desfiladero tuerce de este a sur." s&
	paragraph
	;description
location_08_e :description
	s" El paso entre el desfiladero sigue de norte a este."
	s" La entrada a una cueva se abre al sur en la pared de roca." s&
	paragraph
	;description
location_09_e :description
	s" El camino desciende hacia la agreste sierra, al oeste, desde los verdes valles al este."
	s" Pero un gran derrumbe bloquea el paso hacia la sierra." s&
	paragraph
	;description
location_10_e :description
	s" El estrecho paso se adentra hacia el oeste, desde la boca, al norte. "
	paragraph
	;description
location_11_e :description
	s" Una gran estancia alberga un lago"
	s" de profundas e iridiscentes aguas," s&
	s" debido a la luz exterior." s&
	s" No hay otra salida que el este." s&
	paragraph
	;description
location_12_e :description
	s" Una gran estancia se abre hacia el oeste, y se estrecha hasta morir, al este, en una parte de agua."
	paragraph
	;description
location_13_e :description
	s" La sala se abre en semioscuridad a un puente cubierto de podredumbre sobre el lecho de un canal, de este a oeste."
	paragraph
	;description
location_14_e :description
	s" La iridiscente cueva gira de este a sur."
	paragraph
	;description
location_15_e :description
	s" La gruta desciende de norte a sur sobre un lecho arenoso. Al este, un agujero del que llega claridad."
	paragraph
	;description
location_16_e :description
	s" Como un acueducto, el agua baja con gran fuerza de norte a este, aunque la salida practicable es la del oeste."
	paragraph
	;description
location_17_e :description
	s" Muchas estalactitas se agrupan encima de tu cabeza, y se abren cual arco de entrada hacia el este y sur."
	paragraph
	;description
location_18_e :description
	s" Un arco de piedra se eleva, cual puente sobre la oscuridad, de este a oeste."
	s" En su mitad, un altar." s&
	paragraph
	;description
location_19_e :description
	s" La furiosa corriente, de norte a este, impide el paso, excepto al oeste."
	s" Al fondo, se oye un gran estruendo."
	paragraph
	;description
location_20_e :description
	s" Un tramo de cueva estrecho te permite avanzar hacia el norte y el sur; un pasaje surge al este."
	paragraph
	;description
location_21_e :description
	s" Un tramo de cueva estrecho te permite avanzar de este a oeste; un pasaje surge al sur."
	paragraph
	;description
location_22_e :description
	s" Un tramo de cueva estrecho te permite avanzar de este a oeste; un pasaje surge al sur."
	paragraph
	;description
location_23_e :description
	s" Un tramo de cueva estrecho te permite avanzar de oeste a sur."
	paragraph
	;description
location_24_e :description
	s" Un tramo de cueva estrecho te permite avanzar de este a norte."
	paragraph
	;description
location_25_e :description
	s" Un tramo de cueva estrecho te permite avanzar de este a oeste."
	s" Al norte y al sur surgen pasajes." s&
	paragraph
	;description
location_26_e :description
	s" Un tramo de cueva estrecho te permite avanzar de este a oeste."
	s" Al norte surge un pasaje." s&
	paragraph
	;description
location_27_e :description
	s" Un tramo de cueva estrecho te permite avanzar al oeste."
	s" Al norte surge un pasaje." s&
	paragraph
	;description
location_28_e :description
	s" Una amplia estancia de norte a este, hace de albergue a refugiados:"
	s" hay banderas de ambos bandos." s&
	s" Un hombre anciano te contempla." s&
	s" Los refugiados te rodean." s&
	paragraph
	;description
location_29_e :description
	s" Cual escalera de caracol gigante, desciende a las profundidades,"
	s" dejando a los refugiados al oeste." s&
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
	s" El soporte de roca gira en 'U' de oeste a sur." s&
	paragraph
	;description
location_33_e :description
	s" El paso se va haciendo menos estrecho a medida que se avanza hacia el sur, para entonces comenzar hacia el este."
	paragraph
	;description
location_34_e :description
	s" El paso se anchea de oeste a norte,"
	s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
	paragraph
	;description
location_35_e :description
	s" Un puente se tiende de norte a sur sobre el curso del agua."
	s" Resbaladizas escaleras descienden hacia el oeste." s&
	paragraph
	;description
location_36_e :description
	s" Estruendosa corriente baja con el pasaje elevado desde el oeste, y forma un meandro arenoso."
	s" Unas escaleras suben al este."
	paragraph
	;description
location_37_e :description
	s" El agua baja del oeste con renovadas fuerzas,"
	s" dejando un estrecho paso elevado lateral para avanzar a este o a oeste." s&
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
	s" Unos casi imperceptibles escalones conducen al este." s&
	paragraph
	;description
location_41_e :description
	s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al sur."
	s" Se puede volver a la explanada al oeste."
	paragraph
	;description
location_42_e :description
	s" Como un pasillo que corteja el canal de agua, a su lado, baja de norte a sur."
	s" Se aprecia un aumento de luz hacia el sur."
	paragraph
	;description
location_43_e :description
	s" El pasaje sigue de norte a sur."
	paragraph
	;description
location_44_e :description
	s" Unas escaleras dan paso a un hermoso lago interior, y siguen hacia el oeste."
	s" Al norte, un oscuro y estrecho pasaje sube."
	paragraph
	;description
location_45_e :description
	s" Estrechos pasos permiten ir al oeste, al este (menos oscuro), y al sur, un lugar de gran luminosidad."
	paragraph
	;description
location_46_e :description
	\ Crear estos objetos!!!
	s" Un catre, algunas velas y una mesa es todo lo que tiene Ambrosio."
	paragraph
	;description
location_47_e :description
	s" Por el oeste, una puerta impide, cuando cerrada, la salida de la cueva."
	s" Se adivina la luz diurna al otro lado." s&
	paragraph
	;description
location_48_e :description
	s" Apenas se puede reconocer la entrada de la cueva, al este."
	s" El sendero sale del bosque hacia el oeste." s&
	paragraph
	;description
location_49_e :description
	s" El sendero recorre esta parte del bosque de este a oeste."
	paragraph
	;description
location_50_e :description
	s" El camino norte de Westmorland se interna hacia el bosque, al norte (en tu estado no puedes ir), y a Westmorland, al sur."
	paragraph
	;description
location_51_e :description
	s" La villa bulle de actividad con el mercado en el centro de la plaza, donde se encuentra el castillo."
	paragraph
	;description

\ Los entes globales:

clouds_e :description  \ tmp!!!
	s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
	s" en la estación del frío se han alejado por el momento. " s&
	2 random  if  paragraph  else  sky_e describe  then
	;description

sky_e :description  \ tmp!!!
	s" El cielo es un cuenco de color azul, listado en lo alto por nubes"
	s" del tipo cirros, ligeras y trasparentes." s&
	paragraph
	;description

floor_e :description  \ tmp!!!
	am_i_outside?  if
		s" El suelo fuera es muy bonito."
	paragraph
	else
		s" El suelo dentro es muy bonito."
	paragraph
	then
	;description

ceiling_e :description
	s" El techo es muy bonito."
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
\ Paso 5/6 para crear un nuevo ente:
\ Fijar su localización inicial
\ **********************

: init_entity_locations  \ Asigna a los entes sus localizaciones

	vanish_all  \ Todos al limbo por defecto

	location_01_e ulfius_e be_there
	location_09_e fallen_away_e be_there
	location_15_e log_e be_there
	location_18_e altar_e be_there
	location_18_e stone_e be_there
	location_19_e ambrosio_e be_there
	location_28_e flags_e be_there
	location_28_e man_e be_there
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

	\ Los entes globales no están en un lugar concreto,
	\ pero deben ser marcados como tales:

	ceiling_e >global_inside? on
	floor_e >global_inside? on
	floor_e >global_outside? on
	sky_e >global_outside? on
	clouds_e >global_outside? on

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

\ ##############################################################
cr .( Mapa)

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

: -->  ( a1 a2 u -- )  \ Comunica la salida el ente a1 con el ente a2 mediante la salida indicada por el desplazamiento u
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
	\ a1..a6 = Entes escenario de salida (o cero) en el orden: norte, sur, este, oeste, arriba y abajo
	\ a0 = Ente escenario cuyas salidas hay que modificar
	\ Nota!!! No se usan aún las direcciones dentro y fuera.
	dup >r >down_exit !
	r@ >up_exit !
	r@ >west_exit !
	r@ >east_exit !
	r@ >south_exit !
	r> >north_exit !
	;
: init_location  ( a1..a6 a0 -- )  \ Marca un ente como escenario y le asigna todas las salidas. 
	\ a1..a6 = Entes escenario de salida (o cero) en el orden: norte, sur, este, oeste, arriba, abajo
	\ a0 = Ente escenario cuyas salidas hay que modificar
	dup >location? on  exits!
	;

\ -------------------------------------------------------------
\ Datos

: init_map  \ Prepara el mapa

	0 location_02_e 0 0 0 0 location_01_e init_location
	location_01_e 0 0 location_03_e 0 0 location_02_e init_location
	0 0 location_02_e location_04_e 0 0 location_03_e init_location
	location_05_e 0 location_03_e location_09_e 0 0 location_04_e init_location
	0 location_04_e 0 location_06_e 0 0 location_05_e init_location
	0 0 location_05_e location_07_e 0 0 location_06_e init_location
	0 location_08_e location_06_e 0 0 0 location_07_e init_location
	location_07_e location_10_e 0 0 0 0 location_08_e init_location
	0 0 location_04_e 0 0 0 location_09_e init_location
	location_08_e 0 0 location_11_e 0 0 location_10_e init_location
	0 0 location_10_e 0 0 0 location_11_e init_location
	0 0 0 location_13_e 0 0 location_12_e init_location
	0 0 location_12_e location_14_e 0 0 location_13_e init_location
	0 location_15_e location_13_e 0 0 0 location_14_e init_location
	location_14_e location_17_e location_16_e 0 0 0 location_15_e init_location
	0 0 0 location_15_e 0 0 location_16_e init_location
	location_15_e location_20_e location_18_e 0 0 0 location_17_e init_location
	0 0 location_19_e location_17_e 0 0 location_18_e init_location
	0 0 0 location_18_e 0 0 location_19_e init_location
	location_17_e location_22_e location_25_e 0 0 0 location_20_e init_location
	0 location_27_e location_23_e location_20_e 0 0 location_21_e init_location
	0 location_24_e location_27_e location_22_e 0 0 location_22_e init_location
	0 location_25_e 0 location_21_e 0 0 location_23_e init_location
	location_22_e 0 location_26_e 0 0 0 location_24_e init_location
	location_22_e location_28_e location_23_e location_21_e 0 0 location_25_e init_location
	location_26_e 0 location_20_e location_27_e 0 0 location_26_e init_location
	location_27_e 0 0 location_25_e 0 0 location_27_e init_location
	location_26_e 0 0 0 0 0 location_28_e init_location
	0 0 0 location_28_e 0 location_30_e location_29_e init_location
	0 0 location_31_e 0 location_29_e 0 location_30_e init_location
	0 0 0 location_30_e 0 0 location_31_e init_location
	0 location_33_e 0 location_31_e 0 0 location_32_e init_location
	location_32_e 0 location_34_e 0 0 0 location_33_e init_location
	location_35_e 0 0 location_33_e 0 0 location_34_e init_location
	location_40_e location_34_e 0 location_36_e 0 location_36_e location_35_e init_location
	0 0 location_35_e location_37_e location_35_e 0 location_36_e init_location
	0 0 location_36_e location_38_e 0 0 location_37_e init_location
	0 0 location_37_e location_39_e 0 0 location_38_e init_location
	0 0 location_38_e 0 0 0 location_39_e init_location
	0 location_35_e location_41_e 0 0 0 location_40_e init_location
	0 0 0 location_40_e 0 0 location_41_e init_location
	location_41_e location_43_e 0 0 0 0 location_42_e init_location
	location_42_e 0 0 0 0 0 location_43_e init_location
	location_43_e 0 0 location_45_e 0 0 location_44_e init_location
	0 location_47_e location_44_e location_46_e 0 0 location_45_e init_location
	0 0 location_45_e 0 0 0 location_46_e init_location
	location_45_e 0 0 0 0 0 location_47_e init_location
	0 0 location_47_e location_49_e 0 0 location_48_e init_location
	0 0 location_48_e location_50_e 0 0 location_49_e init_location
	0 location_51_e location_49_e 0 0 0 location_50_e init_location
	location_50_e 0 0 0 0 0 location_51_e init_location

	;

\ ##############################################################
\ Listas

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
: list_separator  ( u1 u2 -- )  \ Añade a la cadena dinámica PRINT_STR el separador adecuado a un elemento de una lista.
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	\ a u = Cadena devuelta, que podrá ser « y » o «, » o «» (vacía)
	?dup  if
		1+ =  if  s" y" »&  else  s" ," »+  then
	else  drop
	then
	;
: can_be_listed?  ( a -- f )  \ ¿El ente puede ser incluido en las listas?
	\ Inacabado!!!
	dup protagonist <>  \ ¿No es el protagonista?
	over >decoration? @ 0=  and  \ ¿Y no es decorativo?
	swap is_global? 0=  and  \ ¿Y no es global?
	;

: /list  ( a -- u )  \ Número de entidades cuyo lugar es cierta entidad 
	0  \ Contador
	#entities 0  do
		over i #>entity dup can_be_listed?
		if  where = abs +
		else  2drop
		then
	loop  nip
	;

: (content_list)  ( a -- )  \ Añade a la lista en la cadena dinámica PRINT_STR el separador y el nombre de un ente
	#elements @ #listed @  list_separator
	>full_name@ »&  #listed ++
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
	my_location@ content_list
	dup  if
		s" Veo" s" Puedo ver" 2 schoose 2swap s&
		narrate
	else  2drop
	then
	[ false ] [if] \ Versión antigua!!!
	#listed @  case
		0  of  s" " endof
		1  of  s" Solo veo"  endof
		s" Veo" rot
	endcase
	2swap s& narrate
	[then]
	;

\ ##############################################################
cr .( Trama) 

\ -------------------------------------------------------------
\ Comprobaciones de finalización

: success?  ( -- f )  \ ¿Ha completado con éxito su misión el protagonista?
	my_location@ location_51_e =
	;
: battle_max  ( -- u )  \ Devuelve el máximo de fases de la batalla
	5 random 7 +  \ Número al azar, de 8 a 11
	;
: failure?  ( -- f )  \ ¿Ha fracasado el protagonista?
	battle# @ battle_max >
	;

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
\ Tramas asociadas a lugares

location_11_e :location_plot
    my_location@ lake_e be_there
	;location_plot
location_16_e :location_plot
	s" En la distancia, por entre los resquicios de las rocas,"
	s" y allende el canal de agua, los sajones tratan de buscar" s&
	s" la salida que encontraste por casualidad."
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
location_44_e :location_plot
    my_location@ lake_e be_there
	;location_plot

\ -------------------------------------------------------------
\ Trama global

: is_the_pass_open?  ( -- f )  \ ¿El paso del desfiladero está abierto por el norte?
	location_08_e >north_exit @ exit?
	;
: going_home?  ( -- f )  \ ¿De vuelta a casa?
	my_location@ location_10_e <  \ ¿Está el protagonista en un escenario menor que el 10?
	is_the_pass_open?  and  \ ¿Y además el paso del desfiladero está abierto por el norte?
	;
: going_home  \ De vuelta a casa
	s" Tus" s" Todos tus" 2 schoose
	soldiers$ s&
	s" siguen tus pasos." s" te siguen." 2 schoose s&
	narrate
	;
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
    s" Una partida sajona aparece por el este."
	s" Para cuando te vuelves al norte," s&
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
	s" su victoria será doble."
	s" su victoria será mayor." 
	s" ganan doblemente."
	s" ganan por partida doble." 4 schoose s&  speak
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
	the_ambush_begins the_battle_begins
	the_enemy_is_stronger your_officers_talk_to_you
	;
: pursued  \ Perseguido por los sajones
    s" No sabes cuánto tiempo te queda"
    s" Sabes que no puedes perder tiempo"
    s" No hay tiempo que perder"
    s" No tienes tiempo que perder"
    s" Te queda poco tiempo"
    s" El tiempo apremia" 6 schoose s" ..." s+
	narrate
	;
: your_men  ( -- a u f )  \ Devuelve una variante de «Tus hombres», y un indicador de número.
	\ a u = Cadena
	\ f = ¿El texto está en plural?
	2 random dup
	if  all_your$ s" Todos y cada uno de tus" 2 schoose
	else  s" Hasta el último de tus"
	then  soldiers$ s&  rot
	;
: fight/s$  ( f -- a u )  \ Devuelve una cadena con una variante de «lucha/n».
	\ f = ¿El resultado debe estar en plural?
	\ a u = Resultado.
	s" lucha" s" combate" s" pelea" s" se bate" 4 schoose
	rot  if
		s" n" s+  \ Poner el verbo en plural si es preciso
	then
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
: fighting_0  \ Ambientación de la batalla (variante 0)
	your_men  dup fight/s$  rot bravery$  s& s&
    s" contra" s&  the_enemy/enemies$ s&  period+ narrate
	;
: fighting_1  \ Ambientación de la batalla (variante 1)
	^the_enemy/enemies
	s" está* haciendo retroceder a tus hombres"
	s" está* obligando a tus hombres a retroceder"
	s" va* ganando terreno"
	s" se va* abriendo paso" 4 schoose rot *>verb_ending s&
	s" por momentos"
	s" palmo a palmo" 
	s" poco a poco" 3 schoose s&
	period+ narrate
	;
: fighting  \ Las tropas combaten.
	\ Inacabado!!! Hacer que las variantes sean consecutivas, según el valor de BATTLE#
	2 random  case 
		0  of  fighting_0  endof
		1  of  fighting_1  endof
	endcase
	;
: pursue_location?  ( -- f )  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?
	my_location@ location_12_e <
	;
: battle_location?  ( -- f )  \ ¿En el escenario de la batalla?
	my_location@ location_10_e <  \ ¿Está el protagonista en un escenario menor que el 10?
	is_the_pass_open? 0=  and  \ ¿Y el paso del desfiladero está cerrado?
	;
: battle  \ Combate y persecución.
	battle_location?  if  fighting  then
	pursue_location?  if  pursued  then
	battle# ++
	;
: battle?  ( -- f)  \ ¿Ha empezado la batalla?
	battle# @ 0>
	;

: dark_cave?  ( -- f )  \ ¿En la cueva y sin luz?
	\ Inacabado!!!
	\ if current_location=20 and (not is_it_accessible(the_torch) or not lit_the_torch)
    \ rem por qué >19?!!! Pongo =20, que es la salida desde la 17
	false
	;
: dark_cave  \ En la cueva y sin luz.
    s" Ante la reinante e intimidante oscuridad,"
	s" retrocedes hasta donde puedes ver." s&
    narrate short_pause
    location_17_e 
	;

: plot  \ Trama global
	\ Nota: Las subtramas deben comprobarse en orden cronológico:
	going_home?  if  going_home  then
	ambush?  if  ambush exit  then
	battle?  if  battle exit  then
	dark_cave?  if  dark_cave  then
	;

\ ##############################################################
cr .( Acciones)

0 [if]  \ ......................................


[then]  \ ......................................

variable action  \ Código de la acción del comando
variable complement  \ Código del complemento del comando

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
\ Tratamiento de errores

: impossible  \ Informa de que una acción es imposible
	\ Provisional!!!
	[debug] [if] s" En IMPOSSIBLE" debug [then]  \ Depuración!!!
	s" No es posible"
	s" Eso no es posible"
	s" No es posible hacer eso"
	s" Es imposible"
	s" Eso es imposible" 
	s" Es imposible hacer eso" 	6 schoose period+ narrate
	;
: nonsense  \ Informa de que alguna acción no tiene sentido
	\ Provisional!!!
	[debug] [if] s" En NONSENSE" debug [then]  \ Depuración!!!
	s" Eso no tiene sentido." narrate
	;
: does_not_make_sense  ( a u -- ) \ Informa de que la acción indicada no tiene sentido
	\ a u = Cadena con el nombre de la acción, generalmente el infitivo
	s" No tiene sentido" 2swap s&
	s" nada" s&
	now$ s&
	period+
	;
: well_done  \ Informa de que una acción se ha realizado
	\ Provisional!!!
	s" Hecho." narrate
	;
: (do_not_worry_0)  ( -- a u)  \ Primera versión posible del mensaje de DO_NOT_WORRY
	s" Hay"
	s" cosas" s" tareas" s" asuntos" s" cuestiones" 4 schoose s&
	s" más" s&
	s" importantes" s" necesarias" s" urgentes" 3 schoose s&
	s" "
	s" para prestarles atención"
	s" de que ocuparse" 3 schoose s& 
	;
: (do_not_worry_1)  ( -- a u)  \ Segunda versión posible del mensaje de DO_NOT_WORRY
	s" Eso no"
	s" tiene importancia"
	s" tiene utilidad"
	s" importa"
	s" hace falta" 
	s" es importante"
	s" es necesario" 6 schoose s&
	;
: do_not_worry  \ Informa de que una acción no tiene importancia
	\ Provisional!!!
	['] (do_not_worry_0)
	['] (do_not_worry_1) 2 choose execute
	now$ s&
	period+ narrate
	;
: that$  ( a -- a1 u1 )  \  Devuelve el nombre de un ente, o un pronombre demostrativo
	2 random  if  s" eso"
	else  >full_name@
	then
	;
: i_do_not_have_it  ( a -- )  \ Informa de que el protagonista no tiene un ente
	^our_hero$ s" no" s& carries$ s&
	that$ s& with_him$ s& period+ narrate
	;
: (i_do_not_wear_it)  ( a -- )  \ Informa de que el protagonista no lleva puesto un ente prenda
	>r ^our_hero$ s" no" s&
	s" lleva puest" r@ noun_ending+
	r> >full_name@ s& period+ narrate
	;
: i_do_not_wear_it  ( a -- )  \ Informa de que el protagonista no lleva puesto un ente prenda, según lo lleve o no consigo
	dup is_hold? @
	if  i_do_not_have_it
	else  (i_do_not_wear_it) 
	then
	;
: i_already_wear_it  ( a -- )  \ Informa de que el protagonista lleva puesto un ente prenda
	>r ^our_hero$ s" ya" s&
	s" lleva puest" r@ noun_ending+
	r> >full_name@ s& period+ narrate
	;

\ -------------------------------------------------------------
\ Mirar, examinar y registrar

: cannot_look$  ( -- a u )  \ Devuelve una forma de decir «no ve»
	s" no ve"
	s" no encuentra" 2 schoose
	;
: cannot_look  ( a -- )  \ Informa de que un ente no puede ser mirado
	^our_hero$  cannot_look$ s&
	rot >subjective_name@ s& period+ report
	;
: actually_do_look  ( a -- )  \ Mira un ente
	dup describe
	>location? @  if  .present  then
	;
: do_look_if_possible  ( a -- )  \ Mira un ente si es posible
	dup can_be_looked_at?  if  actually_do_look
	else  cannot_look
	then
	;
: do_look  \  Acción de mirar
	complement @ ?dup 0=  if  my_location@  then  \ Si no hay complemento, usa el lugar actual
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

\ Inacabado!!!
create do_exits_table_index  \ Tabla para desordenar el listado de salidas
#exits cells allot
\ Esta tabla permite que las salidas se muestren cada vez en un orden diferente

variable #free_exits  \ Contador de las salidas posibles
: .exits  \ Imprime las salidas posibles
	#listed @  case
		0  of  s" No hay salidas"  endof
		1  of  s" Solo hay una salida hacia "  endof
		s" Hay salidas hacia " rot
	endcase
	2swap s& period+ narrate
	;
: exit_separator$  ( -- a u )  \ Devuelve una cadena con el separador adecuado a la salida actual
	#free_exits @ #listed @ list_separator$
	;
: (do_exit)  ( u -- )  \ Lista una salida
	\ u = Desplazamiento del campo de salida
	exit_separator$ s&
	first_exit - do_exits_table + @ >full_name@ s&
	#listed ++
	;
: free_exits  ( a -- u )  \ Devuelve el número de salidas posibles de un ente
	0 swap
	>first_exit /exits bounds  do
		[debug] [if]  i i cr . @ .  [then]  \ Depuración!!!
		i @ 0<> abs +
	cell  +loop 
	;
: do_exits  \ Lista las salidas posibles del lugar del protagonista
	\ No funciona todavía!!!
	#listed off
	my_location@ dup free_exits #free_exits !
	last_exit 1+ first_exit  do
		[debug] [if]  i cr .  [then]  \ Depuración!!!
		dup i + @
		[debug] [if]  dup .  [then]  \ Depuración!!!
		if  i (do_exit)  then
	cell  +loop  drop
	.exits
	;
' do_exits constant do_exits_xt

\ -------------------------------------------------------------
\ Tomar y dejar

: cannot_take_the_altar  \ No se puede tomar el altar
	s" [el altar no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_flags  \ No se puede tomar las banderas
	s" [las banderas no se tocan]" narrate  \ tmp!!!
	;
: cannot_take_the_idol  \ No se puede tomar el ídolo
	s" [el ídolo no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_door  \ No se puede tomar la puerta
	s" [la puerta no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_fallen_away  \ No se puede tomar el derrumbe
	s" [el derrumbe no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_snake  \ No se puede tomar la serpiente
	s" [la serpiente no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_lake  \ No se puede tomar el lago
	s" [el lago no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_lock  \ No se puede tomar el candado
	s" [el candado no se toca]" narrate  \ tmp!!!
	;
: cannot_take_the_water_fall  \ No se puede tomar la cascada
	s" [la cascada no se toca]" narrate  \ tmp!!!
	;
: actually_do_take  ( a -- )  \ Toma un objeto presente, si es posible
	case
		altar_e  of  cannot_take_the_altar  endof
		door_e  of  cannot_take_the_door  endof
		flags_e  of  cannot_take_the_flags  endof
		fallen_away_e  of  cannot_take_the_fallen_away  endof
		idol_e  of  cannot_take_the_idol  endof	
		lake_e  of  cannot_take_the_lake  endof
		lock_e  of  cannot_take_the_lock  endof
		snake_e  of  cannot_take_the_snake  endof
		be_hold well_done
	endcase	
	;
: do_take_if_possible  ( a -- )  \ Toma un objeto, si está presente
	dup is_here?  if  actually_do_take
	else  drop s" [no está aquí]" narrate
	then
	;
: do_take  \ Acción de tomar
	complement @ ?dup  if
		do_take_if_possible
	else
		do_not_worry
	then
	;
' do_take constant do_take_xt
: (do_drop)  ( a -- )
	dup is_hold?  if  be_here well_done
	else  i_do_not_have_it
	then
	;
: do_drop  \ Acción de soltar
	complement @ ?dup  if
		(do_drop)
	else
		do_not_worry
	then
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
		if

		dup >cloth? @
		if  actually_do_put_on  else  drop nonsense  then
	else  i_do_not_have_it  \ Provisional!!! Cambiar el mensaje si no es prenda.
	then
	;
: do_put_on  \ Acción de ponerse una prenda
	complement @ ?dup
	if  do_put_on_if_possible
	else  do_not_worry
	then
	;
' do_put_on constant do_put_on_xt
: actually_do_take_off  ( a -- )  \ Quitarse una prenda
	>worn? off
	;
: do_take_off_if_possible  ( a -- )  \ Quitarse una prenda, si es posible
	dup >worn? @
	if  actually_do_take_off
	else  i_do_not_wear_it
	then
	;
: do_take_off  \ Acción de quitarse una prenda
	complement @ ?dup
	if  do_take_off_if_possible
	else  do_not_worry
	then
	;
' do_take_off constant do_take_off_xt

\ -------------------------------------------------------------
\ Cerrar y abrir

: do_close  \ Acción de cerrar
	." cierro"  
	;
' do_close constant do_close_xt
: do_open  \ Acción de abrir
	." abre"  
	;
' do_open constant do_open_xt

\ -------------------------------------------------------------
\ Agredir

: do_kill  \ Acción de matar
	." mata"  
	;
' do_kill constant do_kill_xt
: do_break  \ Acción de romper
	s" romper" does_not_make_sense
	;
' do_break constant do_break_xt

\ -------------------------------------------------------------
\ Movimiento

: impossible_move  ( a -- )  \ El movimiento es imposible
	\ a = Ente de dirección
	\ Inacabado!!! Añadir una tercera variante «ir en esa dirección»; y otras específicas como «no es posible subir».
	>r
	s" Es imposible" s" No es posible" 2 schoose
	2 random  r@ >no_article? @  or
	if  s" ir hacia" r> >full_name@ 
	else  s" ir al" r> >name@
	then  s& s& period+ narrate
	;
: enter  ( a -- )  \ Entra en un lugar
	[debug] [if] s" En ENTER" debug [then]  \ Depuración!!!
	dup protagonist be_there
	dup describe
	more_familiar  .present
	;
: (do_go)  ( a -- )  \ Comprueba si el movimiento es posible y lo efectúa
	\ a = Ente supuestamente de tipo dirección
	[debug] [if] s" Al entrar en (DO_GO)" debug [then]  \ Depuración!!!
	dup >direction @ ?dup  if  \ ¿El ente es una dirección?
		my_location@ + @ ?dup
		if  nip enter  else  impossible_move  then
	else  drop nonsense
	then
	[debug] [if] s" Al salir de (DO_GO)" debug [then]  \ Depuración!!!
	;
: do_go  \ Acción de ir
	[debug] [if] s" Al entrar en DO_GO" debug [then]  \ Depuración!!!
	complement @ ?dup
	if  (do_go)
	else  ." ir sin más!!!"
	then
	[debug] [if] s" Al salir de DO_GO" debug [then]  \ Depuración!!!
	;
' do_go constant do_go_xt
: do_go_north  \ Acción de ir al norte
	north_e (do_go)
	;
' do_go_north constant do_go_north_xt
: do_go_south  \ Acción de ir al sur
	south_e (do_go)
	;
' do_go_south constant do_go_south_xt
: do_go_east  \ Acción de ir al este
	east_e (do_go)
	;
' do_go_east constant do_go_east_xt
: do_go_west  \ Acción de ir al oeste
	west_e (do_go)
	;
' do_go_west constant do_go_west_xt
: do_go_up  \ Acción de ir hacia arriba
	up_e (do_go)
	;
' do_go_up constant do_go_up_xt
: do_go_down  \ Acción de ir hacia abajo
	down_e (do_go)
	;
' do_go_down constant do_go_down_xt
: do_go_out  \ Acción de ir hacia fuera
	." voy fuera"
	;
' do_go_out constant do_go_out_xt
: do_go_in  \ Acción de ir hacia dentro
	." voy dentro"
	;
' do_go_in constant do_go_in_xt
: do_go_back  \ Acción de ir hacia atrás
	." voy atrás"
	;
' do_go_back constant do_go_back_xt
: do_go_ahead  \ Acción de ir hacia delante
	." voy alante"
	;
' do_go_ahead constant do_go_ahead_xt
: do_swim  \ Acción de nadar
	my_location@ location_11_e =  if
		clear_screen
		s" Caes hacia el fondo por el peso de tu coraza."
		s" Como puedes, te desprendes de ella y buceas," s&
		s" pensando en avanzar, aunque perdido." s&
		narrate short_pause
		location_12_e enter  the_battle_ends
	else
		\ Provisional!!!
		s" No tiene sentido nadar ahora." narrate
	then
	;
' do_swim constant do_swim_xt

 
\ -------------------------------------------------------------
\ Inventario

: anything_with_him$  ( -- a u )  \ Devuelve una cadena con una variante de «nada consigo»
	s" nada" with_him$  ?dup  if
		2 random  if  2swap  then  s&
	else drop
	then
	;
: he_carries_nothing$  ( -- a u )  \ Devuelve un mensaje para sustituir a un inventario vacío
	^our_hero$ s" no" carries$ anything_with_him$ period+ s& s& s& 
	;
: he_carries$  ( -- a u )  \ Devuelve un mensaje para encabezar la lista de inventario
	^our_hero$ carries$ with_him$ s& s&
	;
: do_inventory  \ Acción de hacer inventario
	protagonist content_list  \ Hace la lista en la cadena dinámica PRINT_STR
	#listed @ case
		0 of  he_carries_nothing$ 2swap s& endof
		1 of  he_carries$ 2swap s& endof
		>r he_carries$ 2swap s& r>
	endcase  narrate 
	;
' do_inventory constant do_inventory_xt

\ -------------------------------------------------------------
\ Hablar

: the_man_talks_about_the_stone  \ El hombre habla acerca de la piedra
	s" El" old_man$ s&
	s" se irrita." s" se enfada." s" se enfurece." 3 schoose s&
	narrate
	s" No podemos permitiros"
	s" huir con"
	s" escapar con"
	s" que os vayáis con"
	s" que os llevéis"
	s" que robéis" 5 schoose s&
	s" la piedra del druida." speak
	s" Hace un gesto..." narrate short_pause
	s" La piedra"
	s" debe" s" tiene que" 2 schoose s&
	s" devolverse" s" regresar" 2 schoose s&
	s" a su lugar de encierro." speak
	s" Un hombre te arrebata la piedra y se la lleva." narrate
	location_18_e stone_e be_there
	;
: the_man_talks_about_the_sword  \ El hombre habla acerca de la espada
	s" El" old_man$ s& 
	s" se irrita," s" se enfada," s" se enfurece," 3 schoose s&
	s" y alza su mano indicando al norte." s&
	narrate
	s" Nadie" s" Ningún hombre" 2 schoose
	s" portando" s" que porte" s" llevando" s" que lleve" 4 schoose s&
	s" armas" s" una arma" s" una espada" 3 schoose s&
	with_him$ s&
	s" puede pasar." s& 
	speak
	;
: the_man_lets_you_go  \ El hombre deja marchar al protagonista
	location_28_e location_29_e e-->  \ Hacer que la salida al Este de LOCATION_28_E conduzca a LOCATION_29_E
	s" El" old_man$ s& s" ," s+
	s" calmado," s" sereno," s" tranquilo," 3 schoose s&
	s" indica" s" señala" 2 schoose s&
	s" hacia el" s" en dirección al" 2 schoose s&
	s" este y" s&
	s" habla:" s" dice:" 2 schoose s&
	narrate
	s" Si vienes en paz, puedes ir en paz." speak
	s" Todos" s" Los refugiados" 2 schoose 
	s" se apartan y" s&
	s" permiten ahora el paso al" 
	s" dejan el paso libre hacia el" 2 schoose s&
	s" este." s&
	narrate
	;
: conversation_0_with_the_man
    s" Me llamo Ulfius y..." speak
    talked_to_the_man? on
    s" El" old_man$ s&
	s" asiente, impaciente." narrate
    s" Somos refugiados de la gran guerra."
	s" Buscamos la paz." s&
	speak short_pause
	;
: talk_to_the_man  \ Hablar con el hombre
	talked_to_the_man? 0=  if  conversation_0_with_the_man  then
	stone_e is_accessible?  if
		the_man_talks_about_the_stone
	else
		sword_e is_accessible?
		if  the_man_talks_about_the_sword
		else  the_man_lets_you_go
		then
	then
	;
: conversation_0_with_ambrosio  \ Primera conversación con Ambrosio
	s" Hola, buen hombre." speak
	s" Hola, Ulfius."
	s" Mi nombre es" s" Me llamo" 2 schoose s&
	s" Ambrosio." s& speak
	end_of_scene
	s" Por primera vez, Ulfius se sienta"
	s" y cuenta a Ambrosio todo lo que ha pasado."
	s" Y tras tanto acontecido, llora desconsoladamente." s&
	narrate end_of_scene
	s" Ambrosio le propone un trato, que acepta:"
	s" por ayudarle a salir de la cueva," s&
	s" objetos, vitales para la empresa, le son entregados." s&
	narrate short_pause
	torch_e be_hold  flint_e be_hold
	s" Bien, Ambrosio, emprendamos la marcha." speak
	location_46_e ambrosio_e be_there
	s" Ulfius se da la vuelta"
	s" para ver si Ambrosio le sigue," s&
	s" pero... ha desaparecido." s&
	narrate short_pause
	s" Ulfius piensa entonces en el hecho curioso"
	s" de que supiera su nombre." s&
	narrate end_of_scene
	;
: (conversation_1_with_ambrosio)  \ Segunda conversación con Ambrosio
	s" La llave, Ambrosio, estaba ya en tu poder."
	s" Y es obvio que conocéis un camino más corto." s&
	speak
	s" Estoy atrapado en la cueva debido a magia de maligno poder."
	s" En cuanto al camino, vos debéis hacer el vuestro," s&
	s" verlo todo con vuestros ojos." s&
	speak
	s" Ulfius sacude la cabeza." narrate
	s" No lo entiendo, la verdad." speak
	;
: conversation_1_with_ambrosio  \ Segunda conversación con Ambrosio, si Ambrosio no sigue al protagonista
	ambrosio_follows? 0=  if
		(conversation_1_with_ambrosio)  
	then
	;
: conversation_2_with_ambrosio  \ Tercera conversación con Ambrosio
	s" Por favor, Ulfius, cumple tu promesa."
	s" Toma la llave en tu mano" s&
	s" y abre la puerta de la cueva." s&
	speak
	key_e be_hold
	\ do_takeable the_key \ pendiente!!!
	ambrosio_follows? on
	;
: talk_to_ambrosio  \ Hablar con Ambrosio
	my_location@ case
		location_19_e  of  conversation_0_with_ambrosio  endof
		location_46_e  of  conversation_1_with_ambrosio  endof
	endcase
	[ true ] [if]
	my_location@ case
		location_45_e  of  conversation_2_with_ambrosio  endof
		location_46_e  of  conversation_2_with_ambrosio  endof
		location_47_e  of  conversation_2_with_ambrosio  endof
	endcase
	[else]  \ Método alternativo, inacabado!!!
	location_45_e 1- location_47_e 1+ my_location@ within  if
		conversation_2_with_ambrosio
	then
	[then]
	;
: talk_to_something  \ Hablar con un ente que no es un personaje 
	\ Pendiente!!!
	nonsense
	;
: just_talk  \ Hablar sin más
	\ Pendiente!!!
	s" Bla bla bla." speak
	;
: do_speak  \ Acción de hablar
	complement @  case
		0  of  just_talk  endof
		man_e  of  talk_to_the_man  endof
		ambrosio_e  of  talk_to_ambrosio  endof
		talk_to_something
	endcase
	;
' do_speak constant do_speak_xt

\ ##############################################################
cr .( Intérprete de comandos)

0 [if]  \ ......................................

Gracias al uso del propio intérprete de Forth como
intérprete de comandos del juego, más de la mitad del
trabajo ya está hecha por anticipado.

Sin embargo hay una consideración importante: El intérprete
de Forth ejecutará las palabras en el orden en que el orden
en que estén escritas en la frase del jugador. Esto quiere
decir que no podemos tener una visión global del comando del
jugador: ni de cuántas palabras consta ni qué viene a
continuación de la palabra que está siendo interpretada en
cada momento.  Esta limitación hay que contrarrestarla con
algo de ingenio y con la extraordinaria flexibilidad de
Forth.

[then]  \ ......................................

\ -------------------------------------------------------------
\ Códigos de error

0 [if]  \ ......................................

En en el estándar ANS Forth los códigos de error de -1 a
-255 están reservados para el propio estándar; el resto de
números negativos son para que los asigne cada sistema Forth
a sus propios mensajes de error; del 1 en adelante puede
usarlos libremente cada programa.

[then]  \ ......................................

0
enum no_error  \ Ningún error
enum 2-action_error  \ Hay dos verbos diferentes
enum 2-complement_error  \ Hay dos complementos diferentes
enum no_verb_error  \ No hay verbo
enum no_complement_error  \ No hay complemento
drop

\ -------------------------------------------------------------
\ Tratamiento de errores

: misunderstood  ( u -- )  \ Informa de un error en el comando
	\ Inacabado!!!
	\ u = Código de error; si es cero no se hará nada
	case  
		2-action_error  of  s" 2 verbos!" report  endof
		2-complement_error  of s" 2 complementos!" report  endof
		no_verb_error  of  s" falta verbo" report  endof
		no_complement_error  of  s" falta complemento" report  endof
	endcase
	;

\ -------------------------------------------------------------
\ Analizador

vocabulary player_vocabulary  \ Vocabulario para guardar en él las palabras del juego

: init_parsing  \ Preparativos previos al análisis
	action off
	complement off
	;
: understood?  ( u -- f2 )  \ Comprueba si se ha producido un error en el comando; devuelve 0 si hubo
	\ u = Código de error, o cero si no se produjo un error
	\ f = Cero si se produjo un error; -1 si no se produjo un error
	[debug] [if] s" En UNDERSTOOD?" debug [then]  \ Depuración!!!
	dup misunderstood 0=
	;
: (obbey)  ( a u -- u2 )  \ Evalúa un comando con el vocabulario del juego
	\ a u = Comando
	\ u2 = Código de error; o cero si no se produjo un error
	[debug] [if] s" En (OBBEY)" debug [then]  \ Depuración!!!
	only player_vocabulary  \ Dejar solo el diccionario PLAYER_VOCABULARY activo
	['] evaluate catch  \ Llamar a EVALUATE a través de CATCH para poder regresar directamente en caso de error
	restore_vocabularies
	;
: call_action  \ Ejecuta la acción del comando, si existe
	[debug] [if] s" En CALL_ACTION" debug [then]  \ Depuración!!!
	action @ ?dup  if  execute
	else  no_verb_error misunderstood
	then
	;
: obbey  ( a u -- )  \ Evalúa un comando con el vocabulario del juego
	[debug] [if] s" Al entrar en OBBEY" debug [then]  \ Depuración!!!
	init_parsing (obbey)
	understood?  if  call_action  then
	[debug] [if] s" Al final de OBBEY" debug [then]  \ Depuración!!!
	; 
: second?  ( a1 a2 -- a1 f )  \ ¿La acción o el complemento son los segundos que se encuentran?
	\ a1 = Acción o complemento recién encontrado
	\ a2 = Acción o complemento almacenado, o cero
	2dup <> swap 0<> and  \ ¿Hay ya otro anterior y es diferente?
	;
: action!  ( a -- )  \ Comprueba y almacena la acción (la dirección de ejecución de su palabra) 
	action @ second?  \ ¿Había ya una acción?
	if  2-action_error throw  \ Sí, error
	else  action !  \ No, guardarla
	then
	;
: complement!  ( a -- )  \ Comprueba y almacena un complemento (la dirección de la ficha de su ente)
	complement @ second?  \ ¿Había ya un complemento?
	if  2-complement_error throw  \ Sí, error
	else  complement !  \ No, guardarlo
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

\ ##############################################################
cr .( Vocabulario)

0 [if]  \ ......................................

El vocabulario del juego es realmente un vocabulario de
Forth, creado con el nombre de PLAYER_VOCABULARY .  La idea
es muy sencilla: crearemos en este vocabulario nuevo
palabras de Forth cuyos nombres sean las palabras españolas
que han de ser reconocidas en los comandos del jugador.  

De este modo bastará interpretar la frase del jugador con la
palabra EVALUATE , que ejecutará cada palabra que contenga.

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
\ Vocabulario del juego

also player_vocabulary definitions  \ Elegir el vocabulario PLAYER_VOCABULARY para crear en él las nuevas palabras

\ **********************
\ Paso 6/6 para crear un nuevo ente:
\ Crear las palabras relacionadas con él en el vocabulario del jugador, y sus sinónimos
\ **********************

: ir do_go_xt action!  ;
' ir 8 synonyms: ve vete irse irte dirigirse dirígete muévete moverse

: abrir do_open_xt action!  ;
' abrir synonym: abre

: cerrar  do_close_xt action!  ;
' cerrar synonym: cierra

: coger  do_take_xt action!  ;
' coger 7 synonyms: coge recoger recoge tomar toma agarrar agarra

: dejar  do_drop_xt action!  ;
' dejar 5 synonyms: deja soltar suelta tirar tira

: mirar  do_look_xt action!  ;
' mirar 2 synonyms: m mira

: mirarte do_look_xt action! protagonist complement!  ;
' mirarte synonym: mírate

: x  do_exits_xt action!  ;
: salida do_exits_xt exit_e action|complement!  ;
' salida synonym: salidas

: examinar  do_examine_xt action!  ;
' examinar 2 synonyms: ex examina

: examinarte  do_examine_xt action! protagonist complement!  ;
' examinarte synonym: examínate

: registrar  do_search_xt action!  ;
' registrar synonym: registra

: fin  restore_vocabularies default_color cr (title) quit  ;  \ Provisional!!!

: i  do_inventory_xt inventory_e action|complement!  ;
' i synonym: inventario
: inventariar  do_inventory_xt action!  ;
' inventariar synonym: inventaría

: nadar  do_swim_xt action!  ;
' nadar 10 synonyms: nada bucear bucea sumérgete zambullirse zambullirte zambúllete bañarte bañarse báñate

: ulfius  ulfius_e complement!  ;
: ambrosio  ambrosio_e complement!  ;
: altar  altar_e complement!  ;
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
: hombre  man_e complement!  ;
' hombre 3 synonyms: viejo jefe anciano
: trozo  piece_e complement!  ;
' trozo synonym: pedazo
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
: subir  do_go_up_xt up_e action!  ;
' subir 5 synonyms: sube ascender asciende subirte súbete

: b  do_go_up_xt up_e action|complement!  ;
' b synonym: abajo
: bajar  do_go_up_xt up_e action!  ;
' bajar 5 synonyms: baja descender desciende bajarte bájate

: nubes  clouds_e complement!  ;
' nubes 3 synonyms: nube estratocúmulo estratocúmulos
: suelo  floor_e complement!  ;
' suelo 3 synonyms: suelos tierra firme
: cielo  sky_e complement!  ;
' cielo 4 synonyms: cielos firmamento cirro cirros
: techo  ceiling_e complement!  ;

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

\ ##############################################################
cr .( Entrada de comandos)

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
	cr command_prompt_color ." >" space
	;
: wait_for_input  ( -- a u )  \ Imprime un presto y devuelve el comando introducido por el jugador
	.command_prompt (wait_for_input) cr
	;
: listen  ( -- a u )  \ Espera y devuelve el comando introducido por el jugador, formateado
	[ true ] [if] s" " debug [then]  \ Depuración!!!
	wait_for_input  -punctuation
	; 

\ ##############################################################
cr .( Entrada de respuestas a preguntas de tipo «sí o no»)

: evaluate_answer  ( a u -- )  \ Evalúa una respuesta a una pregunta del tipo «sí o no»
	only  yes/no_vocabulary  evaluate  restore_vocabularies
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

\ ##############################################################
cr .( Preparativos)

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

\ ##############################################################
cr .( Fin)

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
: retry?$  ( -- a u )  \ Devuelve la pregunta que se hace al jugador tras haber fracasado
	s" ¿Tienes"
	s" ¿Te quedan"
	s" ¿Guardas"
	s" ¿Conservas" 4 schoose
	s" fuerzas"
	s" arrestos"
	s" agallas" 
	s" energías"
	s" ánimos" 5 schoose s&
	s" para" s&
	s" jugar" s" probar" s" intentarlo" 3 schoose s&
	again?$ s&
	;
: enough?  ( -- f )  \ ¿Prefiere el jugador no jugar otra partida?
	success?  if  play_again?$  else  retry?$  then  no?
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
	s" y de lo que ha pasado." s& narrate
    short_pause
    s" Pides audiencia al rey, Uther Pendragon." narrate
    end_of_scene
	castilian_quotes? @
	if  \ Comillas castellanas
		s" El rey" rquote$ s+ s" , te indica el valido," s+
		lquote$ s" ha ordenado que no se le moleste," s+ s&
	else  \ Raya
		s" El rey" 
		dash$ s" te indica el valido" dash$ s" ," s+ s+ s+ s&
		s" ha ordenado que no se le moleste," s&
	then
	s" pues sufre una amarga tristeza." s& speak
    short_pause
    s" No puedes entenderlo. El rey, tu amigo." narrate
    short_pause
    s" Agotado, decepcionado, apesadumbrado,"
	s" decides ir a dormir a tu casa." s&
	s" Es lo poco que puedes hacer." s& narrate
    short_pause
    s" Te has ganado un buen descanso." narrate
	;
: the_sad_end  \ Final del juego con fracaso
    s" Los sajones te capturan."
	s" Su general, sonriendo ampliamente, dice:" s&
	narrate short_pause
	s" Bien, bien..."
	s" Hoy parece ser" 
	s" Hoy sin duda es"
	s" No cabe duda de que hoy es"
	s" Hoy es" 4 schoose s" mi día de suerte..." s&
	s" Excelente..." 3 schoose
	s" Por el gran Ulfius podremos pedir un buen rescate."
	s" Del gran Ulfius podremos sacar una buena ventaja."
	2 schoose s&  speak
	;
: the_end  \ Mensaje final del juego
	success?  if  the_happy_end  else  the_sad_end  then
    end_of_scene 
	;

\ ##############################################################
cr .( Introducción)

: credits  \ Créditos del programa
	credits_color
	s" «Asalto y castigo» (escrito en SP-Forth)" paragraph/
	s" Versión " version$ s& paragraph/
	s" (C) 2011 Marcos Cruz (programandala.net)" paragraph
	s" «Asalto y castigo» (escrito en SP-Forth) es un programa libre;"
	s" puedes distribuirlo y/o modificarlo bajo los términos de" s&
	s" la Licencia Pública General de GNU, tal como está publicada" s&
	s" por la Free Software Foundation ('fundación para los programas libres')," s&
	s" bien en su versión 2 o, a tu elección, cualquier versión posterior" S&
	s" (http://gnu.org/licences/)." s&
	paragraph
	s" «Asalto y castigo» (escrito en SP-Forth) está basado"
	s" en la versión escrita en SuperBASIC por el mismo autor," s&
	s" a su vez basada en el programa original," s&
	s" escrito en Sinclair BASIC, Locomotive BASIC y Blassic," s&
	s" (C) 2009 Baltasar el Arquero (http://caad.es/baltasarq/)." s& paragraph
	;

: intro  \ Texto de introducción al juego 
	clear_screen
	s" El sol despunta de entre la niebla,"
	s" haciendo humear los tejados de paja." s&
	narrate  short_pause
	s" Piensas en el encargo realizado por Uther Pendragon."
	s" Atacar una aldea tranquila," s&
	s" aunque sea una llena de sajones," s&
	s" no te llena de orgullo." s&
	narrate  short_pause
	s" Los hombres se ciernen sobre la aldea,"
	s" y la destruyen." s&
	s" No hubo tropas enemigas, ni honor en la batalla." s&
	narrate  end_of_scene
	s" Sire Ulfius, la batalla ha terminado." speak
	s" Lentamente, das la orden de volver a casa."
	s" Los oficiales detienen como pueden el saqueo." s&
	narrate  end_of_scene
	;

\ ##############################################################
cr .( Configuración)

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
: mujer  \ Indica que el jugador es una mujer
	woman_player? on
	;
' mujer alias femenino

: raya  \ Indica que se use la raya en las citas 
	castilian_quotes? off
	;
: comillas  \ Indica que se usen las comillas castellanas en las citas
	castilian_quotes? on
	;

\ Fin de las palabras que pueden usarse
\ en el fichero configuración.

restore_vocabularies

: read_config  ( -- )  \ Lee el fichero de configuración
	only config_vocabulary
	[ config_file$ ] sliteral included
	restore_vocabularies
	;

\ ##############################################################
cr .( Principal)

: game  \ Bucle del juego
	begin
		plot  listen obbey
	game_over? until
	;
: game_preparation  \ Preparación de la partida
	init/game read_config
	credits cr intro
	location_08_e enter
	;
: main  \ Palabra principal que arranca el juego
	init/once
	begin
		game_preparation game the_end
	enough? until
	do_bye
	;
' main alias ayc
' main alias go
' main alias run

: i0  \ Hace toda la inicialización
	\ Palabra temporal para la depuración del programa!!!
	init/once init/game read_config
	;

cr
\ i0
\ ayc

\eof

\ ##############################################################
cr .( Grabación del sistema)

0 [if]
: save_ayc
	0 to spf-init?  \ Desactivar la inicialización del sistema
	1 to console?  \ Activar el modo de consola
	['] ug to <main>  \ Actualizar la palabra que se ejecutará al arrancar
	s" ayc" save  \ Grabar el sistema en un fichero
	;
[then]


\eof  \ Marca del final efectivo del programa; el resto del fichero contiene comentarios

0 [if]  \ ......................................
[then]  \ ......................................
