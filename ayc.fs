\ #############################################################
CR .( Asalto y castigo )  \ {{{

\ Un juego conversacional en castellano, escrito en Forth.
\ Konversacia hispanlingva ludo, verkita en Forth.
\ A Spanish text adventure, written in Forth.

\ Copyright (C) 2011 Marcos Cruz (programandala.net)

ONLY FORTH DEFINITIONS
: version$  ( -- a u )  S" A-02-201112300031"  ;
version$ TYPE CR

\ 'Asalto y castigo' (written in Forth) is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public License
\ as published by the Free Software Foundation; either version 2
\ of the License, or (at your option) any later version.
\ http://gnu.org/licenses/
\ http://gnu.org/licenses/gpl.html
\ http://www.gnu.org/licenses/gpl-2.0.html

\ «Asalto y castigo» (escrito en Forth) es un programa libre;
\ puedes distribuirlo y/o modificarlo bajo los términos de
\ la Licencia Pública General de GNU, tal como está publicada
\ por la Free Software Foundation ('fundación para los programas libres'),
\ bien en su versión 2 o, a tu elección, cualquier versión posterior.
\ (http://gnu.org/licenses/) !!!

\ «Asalto y castigo» (escrito en Forth)
\ está basado en el programa homónimo
\ escrito por Baltasar el Arquero
\ en Sinclair BASIC para ZX Spectrum.

\ Idea, argumento, textos y programa originales:
\ Copyright (C) 2009 Baltasar el Arquero (http://caad.es/baltasarq/).

(

Información sobre:

Juegos conversacionales: http://caad.es

Forth: http://forth.org
SP-Forth: http://spf.sf.net
Gforth:
lina: 
bigFORTH:

Otros proyectos del autor: http://programanadala.net

)

\ #############################################################
\ Documentación

\ El historial de desarrollo está en:
\ http://programandala.net/es.programa.asalto_y_castigo.forth.historial

\ La lista de tareas pendientes está al final de este fichero.

0  [IF]  \ ......................................

Notación de la pila (incompleta!!!)

a     = dirección de memoria
b     = octeto (valor ocho bitios)
c     = carácter
f     = indicador lógico (cero significa «falso»; otro valor significa «cierto»)
ff    = indicador de Forth (0=«falso»; -1=«cierto»)
u     = número de 32 bitios sin signo
n     = número de 32 bitios con signo
+n    = número de 32 bitios positivo
-n    = número de 32 bitios positivo
a u   = zona de memoria, generalmente una cadena de texto (dirección y longitud)
xt    = dirección de ejecución de una palabra
x     = valor sin determinar 32 bitios
true  = -1
false = 0

[THEN]  \ ......................................

\ }}}###########################################################
CR .( Identificación del sistema)  \ {{{

0  [IF]  \ ......................................

A continuación creamos varias constantes [con versiones
inmediatas de cada una, para ser usadas como indicadores de
compilación condicional] que indican el sistema Forth y el
sistema operativo en el que funciona el juego.

El soporte para Gforth no está completado todavía.

[THEN]  \ ......................................

: check  ( n -- )  \ Para poner puntos de comprobación de la pila
	cr . .s ." ..." key drop
	;

\ Sistema Forth

[defined] SPF-INI
DUP CONSTANT sp-forth?
CONSTANT [sp-forth?] IMMEDIATE

S" gforth" ENVIRONMENT? DUP
[IF]  NIP NIP  [THEN]
DUP CONSTANT gforth?
CONSTANT [gforth?] IMMEDIATE

S" bigforth" ENVIRONMENT? DUP
[IF]  NIP NIP  [THEN]
DUP CONSTANT bigforth?
CONSTANT [bigforth?] IMMEDIATE

S" name" ENVIRONMENT? DUP
[IF]  DROP s" ciforth" COMPARE 0=  [THEN]
DUP CONSTANT lina?
CONSTANT [lina?] IMMEDIATE

sp-forth? gforth? OR bigforth? OR lina? OR 0=
[IF]
CR .( El programa no ha sido adaptado a este sistema Forth.)
CR .( Probablemente se producirán errores fatales durante la compilación.)
\ CR .( Pulsa una tecla para continuar.)  KEY DROP
[THEN]

sp-forth? ABS
gforth? ABS +
bigforth? ABS + 
lina? ABS + 1 >
[IF]
CR .( Más de un sistema Forth ha sido reconocido.)
CR .( La compilación es interrumpida.) ABORT
[THEN]

gforth? bigforth? or lina? or
[IF]
CR .( El programa aún no está plenamente adaptado a este sistema Forth.)
CR .( La compilación podría detenerse con errores.)
\ CR .( Pulsa una tecla para continuar.)  KEY DROP
[THEN]

\ Sistema operativo

gforth?  [IF]
s" os-type" ENVIRONMENT? drop s" linux-gnu" COMPARE 0=
[THEN]
sp-forth?  [IF]  [DEFINED] WINAPI: 0=  [THEN]
bigforth?  [IF]  TRUE  [THEN]  \ Provisional!!!
lina?  [IF]  TRUE  [THEN]

( f )  \ ¿Estamos en GNU/Linux?
DUP CONSTANT gnu/linux?
DUP CONSTANT [gnu/linux?] IMMEDIATE
0= DUP
CONSTANT windows?
CONSTANT [windows?] IMMEDIATE

\ }}}###########################################################
CR .( Requisitos)  \ {{{

(

Nota sobre la sensibilidad a mayúsculas:

La sensibilidad a mayúsculas es una característica de cada
sistema Forth. Tradicionalmente la mayoría de los sistemas
Forth son sensibles a mayúsculas y usan las mayúsculas para
los nombres de las palabras.  Algunos sistemas Forth
modernos no dan opción, pero la mayoría permite elegir ambos
modos.  El estándar ANS Forth solo exige que las palabras
que forman parte de él sean reconocidas en mayúsculas.

El uso entre los programadores varía: Algunos escriben todas las 
palabras en mayúsculas, como se hacía antaño; otros solo las palabras
que forman parte del estándar ANS Forth

El uso entre los programadores varía: Algunos escriben todas
las palabras en mayúsculas, como se hacía antaño; otros solo
las palabras que forman parte del estándar ANS Forth; otros,
por legibilidad, solo las palabras que son estructuras de
control y bucles; y por último están aquellos que, como en
nuestro caso, escriben siempre en minúsculas, pues
consideramos que es más fácil de escribir y más legible [los
editores con coloreado de código, como Vim, hacen
innecesario usar las mayúsculas para resaltar ciertas
palabras, salvo en un texto impreso].

Tras cargar los ficheros adecuados para cada sistema Forth,
quedará asegurado que aquellos que distinguen mayúsculas y
minúsculas [como SP-Forth y lina] ya no lo hagan, lo que
permite que el resto del código de este programa esté
escrito en minúsculas.

Las palabras de compilación condicional [IF] , [ELSE] y
[THEN] son un caso espcial: aunque el sistema no sea
sensible a mayúsculas, y estas palabras sean reconocidas de
cualquier manera, ellas al ejecutarse se buscan a sí mismas
como texto en el código fuente, y según el ejemplo
disponible en el estándar ANS Forth emplean la palabra
COMPARE, que hace una comparación binaria y por tanto
sensible a mayúsculas. La mayoría de los Forth cambian esto
y permiten usar esas tres palabras también en minúsculas. No
es el caso de lina, aunque es fácil reescribirlas [véase
http://programanadala.net/es.programa.lina_toolkit]. No obstante,
para aumentar la legibilidad y la portabilidad, escribiremos
esas tres palabras en mayúsculas.

)

\ ------------------------------------------------
\ Requisitos de SP-Forth 

(

SP-Forth incluye mucho código fuente opcional
tanto de sus propios desarrolladores como
contribuciones de usuarios.

)

sp-forth? [IF]

REQUIRE CASE-INS lib/ext/caseins.f  \ Para que el sistema no distinga mayúsculas de minúsculas en las palabras de Forth
CASE-INS ON  \ Activarlo
require ansi-file lib/include/ansi-file.f  \ Para que el sistema sea lo más compatible posible con el estándar ANS Forth de 1994
\ require random lib/ext/rnd.f  \ Generador de números aleatorios

require with ~ygrek/spf/included.f  \ Directorios de búsqueda para INCLUDED
\ Añadir directorios a la búsqueda de INCLUDED :
s" /home/programandala.net/forth/" with  
S" /home/programandala.net/forth/ayc/" with

gnu/linux?  [IF]
\ require key-termios lib/posix/key.f  \ Palabra KEY 
require key-termios ~programandala.net/sp-forth/key.f  \ Palabra KEY (copia modificada del fichero original debido a un problema con la ruta de una librería)
[THEN]

[THEN]

\ ------------------------------------------------
\ Requisitos de lina

lina? [IF]

(

El fichero lina_extension.fs que cargaremos a continuación
está creado a medida para ampliar lina con todas las
herramientas necesarias. Aunque se supone que ya ha sido
incluido en el arranque del sistema [pues para haber
interpretado este programa hasta aquí lina precisa de las
palabras para compilación condicional y otras, que no están
en su núcleo], hacerlo aquí no supone un inconveniente y
permite cargar este programa desde un lina casi desnudo.

)

S" lina_extension.fs" INCLUDED

(

El siguiente fichero provee definiciones alternativas para
[IF] [ELSE] [THEN] que funcionan sin distinguir mayúsculas
[lo que es una práctica no estándar para estas palabras,
pero muy extendida]. Esto previene contra fallos de
compilación condicional en caso de que alguna librería
utilice esas palabras en minúsculas.

)

S" lina_cicc.fs" INCLUDED

\ Algunas equivalencias:

' (word) alias parse-name  \ Nombre más común
' noop alias refill  \ En lina no existe REFILL porque no es necesario

: \eof ( -- ) \ Ignora el resto del fichero
	\ Nota: esta palabra en SP-Forth es nativa (está definida en ensamblador).
	\ Esta es una definición equivalente para lina:
	SRC 1 CELLS + @ IN !
	;

(

Para más información sobre el arranque
y la configuración de lina véase:
http://programandala.net/es.programa.lina_toolkit

)

[THEN]

\ ------------------------------------------------
\ La librería «Forth Foundation Library»
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
módulo «str» empiezan por «str», como STR-CREATE o
STR+COLUMNS o STR.VERSION .

)

s" ffl/str.fs" included \ Cadenas de texto dinámicas
s" ffl/trm.fs" included \ Manejador de secuencias de escape de la consola (para cambiar sus características, colores, posición del cursor...)
s" ffl/chr.fs" included \ Herramientas para caracteres
\ s" ffl/est.fs" included \ Cadenas de texto con secuencias de escape
s" ffl/dtm.fs" included \ Tipo de datos para fecha (gregoriana) y hora
s" ffl/dti.fs" included \ Palabras adicionales para manipular el tipo de datos provisto por el módulo dtm

\ ------------------------------------------------
\ csb2

(

De las herramientas propias necesitamos csb2, que implementa
un almacén circular de cadenas y diversas herramientas
relacionadas.

Véase: http://programandala.net/es.programa.csb2

Pero antes de que el programa csb2 sustituya la palabra
original S" por su propia versión, hacemos un alias con
nombre P" porque necesitaremos la versión original en un
caso especial.

)

: p"  postpone s"  ;  immediate

\ Almacén circular de cadenas,
\ con definición de cadenas y operadores de concatenación:

lina?
[IF]    s" ayc/csb2.fs" included
[ELSE]  s" csb2.fs" included
[THEN] 

\ Nota:
\ No es posible usar un solo INCLUDED para ambos casos,
\ colacado tras el [THEN] , como sería más elegante,
\ porque en algunos Forth la cadena creada con S" es guardada en
\ el PAD u otra zona de almacenamiento temporal, zona que puede
\ ser machacada por el funcionamiento de [IF] y [ELSE] , que
\ leen cada palabra del código fuente y la guardan en el mismo
\ sitio.

0  [IF]  \ ......................................

La palabra SAVE de SP-Forth, que crea un ejecutable
del sistema, no funciona bien cuando se ha cargado csb2.

Parece que la causa tiene alguna relación con que las
palabras básicas creadas por csb2 para manipular cadenas
usan el almacén circular, que ha sido creado en memoria con
ALLOCATE . Para evitar esto, se crea un nuevo almacén en el
espacio del diccionario de Forth.

Para más detalles, veáse el código fuente del programa csb2.

[THEN]  \ ......................................

false  [IF]  \ Solución descartada

\ Creamos el almacén circular de cadenas en el diccionario.
\ Pero no da resultado: Se produce otro error similar en SAVE .
\ Por eso antes creamos un alias para S" .

free_csb  \ Borrar el almacén predeterminado
2048 here  \ Longitud y dirección del nuevo almacén
over >bytes/csb allot  \ Hacer espacio en el diccionario para el almacén
csb_set  \ Inicializar el almacén

[ELSE]

\ Pero en cualquier caso el kibiocteto predeterminado
\ no es suficiente para hacer tantas operaciones con
\ cadenas y hace falta redimensionar el almacén:

2048 resize_csb

[THEN]

\ }}}###########################################################
\ Meta \ {{{

: [or]  ( x1 x2 -- x3 )  or  ;  immediate

\ Indicadores para depuración

false value [debug] immediate  \ Indicador: ¿Modo de depuración global?
false value [debug_init] immediate  \ Indicador: ¿Modo de depuración para la inicialización?
false value [debug_parsing] immediate  \ Indicador: ¿Modo de depuración para el analizador?
false value [debug_do_exits] immediate  \ Indicador: ¿Depurar la acción DO_EXITS ?
false value [debug_catch] immediate  \ Indicador: ¿Depurar CATCH y THROW ?
false value [debug_save] immediate  \ Indicador: ¿Depurar la grabación de partidas?
true value [debug_info] immediate  \ Indicador: ¿Mostrar info de depuración sobre el presto de comandos? 
false value [debug_pause] immediate  \ Indicador: ¿Hacer una pausa en cada punto de depuración?

[debug]
[debug_init] or
[debug_do_exits] or
[debug_parsing] or
[debug_catch] or
[debug_save] or
true or  \ !!!
sp-forth? and  [IF]  startlog  [THEN]

\ Indicadores para poder elegir alternativas que aún son experimentales

true constant [old_method]  immediate
[old_method] 0= constant [new_method]  immediate

\ Constantes

true constant [true] immediate  \ Para usar en compilación condicional 
false constant [false] immediate  \ Para usar en compilación condicional 
false constant [0] immediate  \ Para usar en comentarios multilínea con [IF] dentro de las definiciones de palabras

s" /COUNTED-STRING" environment? 0=  [IF]  255 chars  [THEN]
constant /counted_string  \ Longitud máxima de una cadena contada (incluyendo la cuenta)

\ Títulos de sección

: .s?  ( -- )  \ Imprime el contenido de la pila si no está vacía
	depth
	[lina?]  [if]  1 >  [then]  \ Al parecer lina mantiene un valor en la pila mientra interpreta ficheros
	if  cr ." Pila: " .s cr  key drop
	then
	;
: section(  ( "text<bracket>" -- )  \ Notación para los títulos de sección en el código fuente
	\ Esta palabra permite hacer tareas de depuración mientras se compila el programa;
	\ por ejemplo detectar el origen de descuadres en la pila.
	cr postpone .(  \ El nombre de sección terminará con: )
	.s?
	;
: subsection(  ( "text<bracket>" -- )  \ Notación para los títulos de subsección en el código fuente
	[char] * emit space postpone .(  \ El nombre de subsección terminará con: )
	space .s?
	;

: program_filename$  ( -- a u )  \ Devuelve el nombre con que se grabará el juego
	\ Nota:
	\ En SP-Forth no se puede usar la versión de S" redefinida por 
	\ el programa csb2 (pues usa el almacén circular de cadenas)
	\ sino la original (que usa el PAD ) y cuyo alias está en P" .
	\ De otro modo SAVE no funcionará bien:
	\ mostrará mensajes de gcc con parámetros sacados de textos del programa,
	\ y creará el fichero objeto pero no el ejecutable.
	\ La causa última del problema no está clara.
	\ Pendiente!!! Añadir número de versión al nombre de fichero
	[ sp-forth? gnu/linux? and ]  [IF]  p" ayc"  [THEN]
	[ sp-forth? windows? and ]  [IF]  p" ayc.exe"  [THEN]
	[lina?]  [IF] s" ayc"  [THEN]
	;
defer main
: create_executable  ( -- )  \ Crea un ejecutable con el programa
	[sp-forth?]  [IF]
		false to spf-init?  \ Desactivar la inicialización del sistema
\		true to console?  \ Activar el modo de consola (no está claro en el manual)
\		false to gui?  \ Desactivar el modo gráfico (no está claro en el manual)
		['] main to <main>  \ Actualizar la palabra que se ejecutará al arrancar
		program_filename$ save  
		\ Pendiente!!! Borrar el fichero objeto
	[THEN]
	[lina?]  [IF]
		program_filename$ save-system
	[THEN]
	;

\ }}}###########################################################
section( Vocabularios de Forth)  \ {{{

lina? false and  [IF]  \ anulado!!!

(

Una de las peculiaridades de lina es que las palabras que
definen un vocabulario, al ser ejecutadas, añaden su
identificador a la pila de vocabularios de búsqueda, en
lugar de sustituir el vocabulario que está en la parte
superior, que es el funcionamiento clásico y estándar.

Esto hace que la palabra ALSO [que en lina tiene su función
estándar de duplicar el vocabulario más reciente en el orden
de búsqueda] sea no solo innecesaria sino problemática para
escribir código común a varios Forth.

Una posible solución sería anular ALSO , por ejemplo así:

	' noop alias also

Pero para no perder la funcionalidad de ALSO y para acercar lina
al funcionamiento estándar, es más conveniente parchear la
definición de la palabra VOCABULARY [en la que se encuentra
el código que ejecutan todos los vocabularios creados con ella,
pues usa CREATE y DOES> ] para sustituir ALSO por NOOP .

La herramienta completa para hacer esto puede consultarse en
http://programandala.net/es.programa.lina_tools con el
nombre fixvoc.fs; lo que sigue es un extracto esencial de
ella, que es lo único que se necesita aquí:

)

: vocabulary-word>  ( n -- a )  \ Devuelve la dirección del CFA de la enésima palabra en la definición de VOCABULARY
  cells ['] vocabulary >dfa @ + 
  ;
: (fixvoc)  ( -- )  \ Modifica la definición de VOCABULARY para evitar que los vocabularios hagan ALSO
	['] noop >cfa  \ Calcular el CFA de la palabra NOOP , que no hace nada
	24 \ Posición de ALSO en la zona de datos de VOCABULARY (averiguado leyendo las fuentes de lina en ensamblador)
	vocabulary-word> !  \ Guardar en ella el CFA de NOOP
  ;
(fixvoc)

(

Otra peculiaridad es que la palabra FORTH , correspondiente
al vocabulario principal del sistema, es inmediata [como
todos los vocabularios en lina] y por tanto se ejecuta en
tiempo de compilación. Esto es un inconveniente para crear
código pluriplataforma, porque no es el comportamiento
estándar en Forth.

Para evitarlo, en lugar de crear condiciones de compilación
en cada caso, creamos una palabra homónima que no sea
inmediata y que llame a la original:

)

: forth  ( -- )  postpone forth  ;

[THEN]

\ Creamos los vocabularios que necesitaremos en el programa

\ Vocabulario principal del programa (no de la aventura)
gforth?  [IF]
	\ Gforth necesita su propio método para crear un vocabulario sensible a mayúsculas
	\ Borrador!!!
	table value (game_vocabulary)
	: game_vocabulary  ( -- )  (game_vocabulary) >order  ;
[ELSE]
	vocabulary game_vocabulary
[THEN]

: restore_vocabularies  ( -- )  \ Restaura los vocabularios a su orden habitual
	\ En lina los vocabularios son inmediatos
	\ (aunque por alguna razón los creados con VOCABULARY no reciben ese tratamiento
	\ de POSTPONE ), y además al ejecutarse
	\ no sustituyen al primero en el orden, sino que se añaden al orden.
	\ Por ello lina necesita aquí un tratamiento especial:
	[lina?]
	[IF]    postpone only postpone forth game_vocabulary
	[ELSE]  only forth also game_vocabulary
	[THEN]  definitions
	;
restore_vocabularies

\ Vocabularios
vocabulary menu_vocabulary  \ palabras del menú \ Aún no se usa!!!
vocabulary player_vocabulary  \ palabras de la aventura en sí (los comandos del jugador)
vocabulary answer_vocabulary  \ respuestas a preguntas de «sí» o «no»
vocabulary config_vocabulary  \ palabras de configuración del juego
vocabulary restore_vocabulary  \ palabras de restauración de una partida del juego

\ }}}###########################################################
section( Palabras genéricas)  \ {{{

: enum  (  n "name" -- n+1 ) \ Crear una constante como parte de una lista
	\ Palabra tomada de la librería de contribuciones de SP-Forth (~nn/lib/enum.f)
	dup constant 1+
	;
: drops  ( x1..xn n -- )  \ Elimina n celdas de la pila
	0  do  drop  loop
	;
: truncate_length  ( u1 -- u1 | u2 )  \ Recorta la longitud de una cadena si es demasiado larga
	/counted_string chars min 
	;
: sconstant  (  a1 u "name" -- )  \ Crea una constante de cadena
	[lina?]  [IF]
		\ Nota!!!:
		\ Lina en principio no necesita almacenar la cadena en la constante,
		\ porque ya la guarda en el espacio del diccionario al crearla con S" .
		\ No obstante la definición siguiente no funcionará
		\ si la cadena recibida está por algún motivo en un lugar temporal:
		create  , ,
		does>  ( pfa -- a1 u )  2@
	[ELSE]
		create  truncate_length dup c, s, align
		does>  ( pfa -- a2 u )  count
	[THEN]
	;
: svariable  ( "name" -- )  \ Crea una variable de cadena
	create  /counted_string chars allot align
	;
lina?  [IF]
' $! alias s!
[ELSE]
: s!  ( a1 u1 a2 -- )  \ Guarda una cadena en una variable de cadena
	swap truncate_length swap
	2dup c!  char+ swap chars cmove
	;
[THEN]

[undefined] perform  [IF]
: perform  ( a -- )  \ Ejecuta la dirección de ejecución contenida en una dirección; palabra tomada de Gforth
	@ execute
	;
[THEN]

sp-forth?  [IF]  \ En SP-Forh redefinimos su versión de alias de forma estándar
: (alias)  ( xt a u -- )  \ Crea un alias de una palabra
	\ xt = Dirección de ejecución de la palabra de la que hay que crear el alias
	\ a u = Nombre del alias
	sheader last-cfa @ !
	;
: alias (  xt "name" -- )  \ Crea un alias de una palabra
	\ xt = Dirección de ejecución de la palabra de la que hay que crear el alias
	\ "name" = Nombre del alias, en el flujo de entrada
	\ Versión modificada (para hacer su sintaxis más estándar)
	\ de la palabra homónima provista por
	\ la librería de contribuciones de SP-Forth:
	\ ~moleg/lib/util/alias.f
	\ Copyright (C) 2007 mOleg 
	nextword (alias)
	;
[THEN]

lina?  [IF]
: (alias)  ( dea a u -- )  \ Crea un alias de una palabra
	\ dea = Dirección de la entrada de diccionario de la palabra de la que hay que crear el alias
	\ a u = Nombre del alias
	(create) latest 3 cells move 
	;
[THEN]

sp-forth?  [IF]

\ En SP-Forh creamos OFFSET: , que servirá para crear los campos
\ de las fichas de datos, como alias de la palabra equivalente,
\ antes de redefinirla para otro uso.

' -- alias offset:

[ELSE]

\ En el resto de sistemas definimos OFFSET: :

: offset:  (  u1 u2 "name" -- u3 )  \ Crea un campo en una ficha de datos
	\ u1 = Desplazamiento del campo a crear respecto al inicio de la ficha
	\ u2 = Espacio (en celdas) necesario para el campo
	\ u3 = Desplazamiento del próximo campo
	create over , +
	does>  ( a pfa -- a' )   \ Cuando la constante sea llamada tendrá su pfa en la pila
		@ +
	;
[THEN]

: ++  ( a -- )  \ Incrementa el contenido de una dirección de memoria
	1 swap +!
	;
: --  ( a -- )  \ Decrementa el contenido de una dirección de memoria
 	-1 swap +!
	;
: (++)  ( a -- )  \ Incrementa el contenido de una dirección, si es posible
	\ Nota:
	\ En la práctica el límite es inalcanzable
	\ (pues es un número de 32 bitios),
	\ pero así queda mejor hecho.
	dup @ 1+ ?dup
	if  swap !  else  drop  then
	;

: .forth  ( -- )  \ Imprime el nombre del sistema Forth
	[sp-forth?]  [IF]  (title)  [THEN]
	[lina?]      [IF]  .signon  [THEN]
	[gforth?]    [IF]    [THEN]  \ Pendiente!!!
	[bigforth?]  [IF]    [THEN]  \ Pendiente!!!
	;

\ }}}###########################################################
section( Vectores)  \ {{{

0  [IF]  \ ......................................

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

La palabra de SP-Forth para crear vectores es VECT y la
palabra para actualizarlos es TO (la misma que actualiza los
valores creados con VALUE ) pero las que usaremos son las
habituales en ANS Forth: DEFER e IS . Hacen exactamente lo
mismo: DEFER crea una palabra que no hace
nada, pero cuya dirección de ejecución podrá ser después
cambiada usando la palabra IS de la siguiente forma:

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

[THEN]  \ ......................................

defer protagonist%  \ Ente protagonista
defer do_exits  \ Acción de listar las salidas

\ }}}###########################################################
section( Códigos de error)  \ {{{

0  [IF]  \ ......................................

En el estándar ANS Forth los códigos de error de -1 a -255
están reservados para el propio estándar; el resto de
números negativos se reservan para que los asigne cada
sistema Forth a sus propios mensajes de error; del 1 en
adelante puede usarlos libremente cada programa.

En este programa usamos como códigos de error las
direcciones de ejecución de las palabras que muestran los
errores.  En Forth, la dirección de ejecución de una palabra
se llama tradicionalmente «code offset: address» («cfa» en
notación de la pila). Pero el estándar ANS Forth de 1994, el
más extendido en la actualidad, utiliza el término
«execution token» («xt» en la notación de la pila), pues en
algunos sistemas Forth no es una dirección de memoria sino
un código interno. En este programa lo llamamos «dirección
de ejecución» pero en la notación de pila lo representamos
como «xt».

En cualquier caso es lo mismo en cualquier sistema Forth: es
el valor que devuelven las palabras ' y ['] y que sirve de
parámetro a EXECUTE .

Ejemplo:

	: palabrita  ." ¡Hola mundo!"  ;
	variable palabrita_xt
	' palabrita palabrita_xt !
	palabrita_xt @ execute

Como se ve, usar como códigos de error las direcciones de
ejecución de las palabras de error tiene la ventaja de que
no hace falta ningún mecanismo adicional para encontrar las
palabras de error a partir de sus códigos de error
correspondientes (como podría ser una estructura CASE o una
tabla): basta poner el código de error en la pila y llamar a
EXECUTE .

Dado que algunos los códigos de error se necesitan antes de
haber sido creadas las palabras de error (por ejemplo
durante la creación de los entes), los creamos aquí por
adelantado como vectores y los actualizaremos
posteriormente, cuando se definan las palabras de error,
exactamente como se muestra en este ejemplo:

	defer la_cagaste_error#
	: la_cagaste  ." ¡La cagaste!"  ;
	' la_cagaste constant (la_cagaste_error#)
	' (la_cagaste_error#) is la_cagaste_error#

[THEN]  \ ......................................

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
defer too_many_actions_error#
defer too_many_complements_error#
defer unexpected_main_complement_error# 
defer what_is_already_closed_error#
defer what_is_already_open_error#
defer you_already_have_it_error#
defer you_already_have_what_error#
defer you_already_wear_what_error#
defer you_do_not_have_it_error#
defer you_do_not_have_what_error#
defer you_do_not_wear_what_error#
defer you_need_what_error#

\ }}}###########################################################
section( Generador de números aleatorios; herramientas de azar)  \ {{{

\ Generador de números aleatorios

lina? false and  [IF]  \ Descartado!!!

(

Descartado porque el generador es prácticamente
idéntico al tomado de Gforth, pero el ensamblador
que necesita para usar TICKS en RANDOMIZE falla
algunas veces incluso con el modo de sensibilidad
a mayúsculas. Es más fácil usar el generador
genérico que encontrar la causa del problema.

)

\ lina tiene su propio generador de números aleatorios.
\ Para compilarlo necesita usar su ensamblador, pero
\ falla a menos que desactivemos la insensibilidad
\ a las minúsculas.

CASE-SENSITIVE  REQUIRE RAND  CASE-INSENSITIVE

\ lina usa CHOOSE para la palabra principal
\ (que devuelve un número menor que el que recibe
\ y mayor o igual que cero)
\ pero en este programa usamos el nombre RANDOM

' choose alias random 

[THEN]

gforth?  [IF]  \ Gforh tiene también su generador de números aleatorios

include random.fs

\ Pediente!!! Comprobar si incluye RANDOMIZE

[ELSE]  \ El resto usa el código de Gforth

\ Tomado del código de Gforth,
\ publicado bajo la licencia GPL versión 2 o superior.
\ (http://www.jwdt.com/~paysan/gforth.html)

\ ...................... Comienzo del código tomado de Gforth
\ This code is part of Gforth
\ generates random numbers                             12jan94py
\ Copyright (C) 1995,2000,2003 Free Software Foundation, Inc.
variable seed
hex 10450405 decimal constant generator
: rnd  ( -- n )  seed @ generator um* drop 1+ dup seed !  ;
: random  ( n -- 0..n-1 )  rnd um* nip  ;
\ ...................... Fin del código tomado de Gforth

: randomize  ( -- )  \ Reinicia la semilla de generación de números aleatorios
	time&date 2drop 2drop * seed !
	;

[THEN]

\ Elegir un elemento al azar de la pila

: choose  ( x1..xn n -- xn' )  \ Devuelve un elemento de la pila elegido al azar entre los n superiores y borra el resto
	dup >r random pick r> swap >r drops r>
	;
: dchoose  ( d1..dn n -- dn' )  \ Devuelve un elemento doble de la pila elegido al azar entre los n superiores y borra el resto
	dup >r random 2*  ( d1..dn n' -- ) ( r: n )
	dup 1+ pick swap 2 + pick swap  ( d1..dn dn' -- ) ( r: n )
	r> rot rot 2>r  2* drops  2r>
	;

\ Elegir una cadena al azar entre varias

0  [IF]  \ ......................................

Para facilitar la selección aleatoria de una cadena entre un
grupo, crearemos las palabras S{ y }S , que proporcionarán
un sintaxis fácil de escribir y crearán un código fácil de
leer. También crearemos variantes que concatenen la
cadena elegida de diversas maneras.

Pero para que las palabras S{ y }S puedan ser anidadas,
necesitan una pila propia en la que guardar la profundidad
actual de la pila en cada anidación.

[THEN]  \ ......................................

' dchoose alias schoose  \ Alias de DCHOOSE para usar con cadenas de texto (solo por estética)

4 constant /dstack  \ Elementos de la pila (y por tanto número máximo de anidaciones)
variable dstack>  \ Puntero al elemento superior de la pila (o cero si está vacía)
/dstack cells allot  \ Hacer espacio para la pila
0 dstack> !  \ Pila vacía para empezar
: 'dstack>  ( -- a )  \ Dirección del elemento superior de la pila
	dstack> dup @ cells + 
	;
: dstack_full?  ( -- ff )  \ ¿Está la pila llena?
	dstack> @ /dstack =
	;
: dstack_empty?  ( -- ff )  \ ¿Está la pila vacía?
	dstack> @ 0=
	;
: dstack!  ( u -- )  \ Guarda un elemento en la pila
	dstack_full? abort" Error de anidación de S{ y }S : su pila está llena."
	dstack> ++ 'dstack> ! 
	;
: dstack@  ( -- u )  \ Devuelve el elemento superior de la pila
	dstack_empty? abort" Error de anidación de S{ y }S : su pila está vacía."
	'dstack> @ dstack> --
	;
: s{  ( -- )  \ Inicia una zona de selección aleatoria de cadenas
	depth dstack!
	;
: }s  ( a1 u1 .. an un -- a' u' )  \ Elige una cadena entre las puestas en la pila desde que se ejecutó por última vez la palabra S{
	depth dstack@ - 2 / schoose
	;
: }s&  ( a0 u0 a1 u1 .. an un -- a' u' )  \ Elige una cadena entre las puestas en la pila desde que se ejecutó S{ y la concatena (con separación) a una cadena anterior
	}s s&
	;
: }s+  ( a0 u0 a1 u1 .. an un -- a' u' )  \ Elige una cadena entre las puestas en la pila desde que se ejecutó S{ y la concatena a una cadena anterior
	}s s+
	;
: s?  ( a u -- a u | a 0 )  \ Vacía una cadena (con el 50% de probabilidad)
	2 random *
	;
: s?&  ( a1 u1 a2 u3 -- a3 u3 )  \ Devuelve una cadena concatenada o no (al azar) a otra, con separación
	s? s&
	;
: s?+  ( a1 u1 a2 u3 -- a3 u3 )  \ Devuelve una cadena concatenada o no (al azar) a otra
	s? s+
	;
: s+?  ( a1 u1 a2 u3 -- a3 u3 | a3 0 )  \ Devuelve dos cadenas concatenadas o (al azar) una cadena vacía
	s+ s?
	;
: s&?  ( a1 u1 a2 u3 -- a3 u3 | a3 0 )  \ Devuelve dos cadenas concatenadas (con separación) o (al azar) una cadena vacía
	s& s?
	;
: }s?  ( a1 u1 .. an un -- a' u' | a' 0 )  \ Elige una cadena entre las puestas en la pila desde que se ejecutó S{ y la vacía con el 50% de probabilidad
	}s s?
	;
: }s?&  ( a0 u0 a1 u1 .. an un -- a' u' )  \ Elige una cadena entre las puestas en la pila desde que se ejecutó S{ y (con un 50% de probabilidad) la concatena (con separación) a una cadena anterior
	}s? s&
	;
: }s?+  ( a0 u0 a1 u1 .. an un -- a' u' )  \ Elige una cadena entre las puestas en la pila desde que se ejecutó S{ y (con un 50% de probabilidad) la concatena (sin separación) a una cadena anterior
	}s? s+
	;
: s&{  ( a1 u1 a2 u2 -- a3 u3 )  \ Concatena dos cadenas (con separación) e inicia una zona de selección aleatoria de cadenas
	s& s{
	;
: s+{  ( a1 u1 a2 u2 -- a3 u3 )  \ Concatena dos cadenas (sin separación) e inicia una zona de selección aleatoria de cadenas
	s+ s{
	;

\ Combinar cadenas de forma aleatoria

: r2swap  ( a1 u1 a2 u2 -- a1 u1 a2 u2 | a2 u2 a1 u1 )  \ Intercambia (con 50% de probabililad) la posición de dos textos
	2 random  if  2swap  then
	;
: (both)  ( a1 u1 a2 u2 -- a1 u1 a3 u3 a2 u2 | a2 u2 a3 u3 a1 u1 )  \ Devuelve las dos cadenas recibidas, en cualquier orden, y separadas en la pila por la cadena «y»
	r2swap s" y" 2swap
	;
: both  ( a1 u1 a2 u2 -- a3 u3 )  \ Devuelve dos cadenas unidas en cualquier orden por «y»
	\ Ejemplo: si los parámetros fueran «espesa» y «fría»,
	\ los dos resultados posibles serían: «fría y espesa» y «espesa y fría».
	(both) s& s&
	;
: both&  ( a0 u0 a1 u1 a2 u2 -- a3 u3 )  \ Devuelve dos cadenas unidas en cualquier orden por «y»; y concatenada (con separación) a una tercera
	both s&
	;
: both?  ( a1 u1 a2 u2 -- a3 u3 )  \ Devuelve al azar una de dos cadenas, o bien ambas unidas en cualquier orden por «y»
	\ Ejemplo: si los parámetros fueran «espesa» y «fría»,
	\ los cuatro resultados posibles serían: «espesa», «fría», «fría y espesa» y «espesa y fría».
	(both) s&? s&
	;
: both?&  ( a0 u0 a1 u1 a2 u2 -- a3 u3 )  \ Concatena (con separación) al azar una de dos cadenas (o bien ambas unidas en cualquier orden por «y») a una tercera cadena
	both? s&
	;
: both?+  ( a0 u0 a1 u1 a2 u2 -- a3 u3 )  \ Concatena (sin separación) al azar una de dos cadenas (o bien ambas unidas en cualquier orden por «y») a una tercera cadena
	both? s+
	;

\ Desordenar al azar varios elementos de la pila

0 value unsort#
: unsort  ( x1..xu u -- x1'..xu' )  \ Desordena un número de elementos de la pila
	\ x1..xu = Elementos a desordenar
	\ u = Número de elementos de la pila que hay que desordenar 
	\ x1'..xu' = Los mismos elementos, desordenados
	dup to unsort# 0  do
		unsort# random roll
	loop	
	;

\ }}}###########################################################
section( Variables)  \ {{{

\ Algunas variables de configuración (el resto se crea en sus propias secciones)

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
\ variable hold#  \ Contador de cosas llevadas por el protagonista (no se usa!!!)

: init_plot  ( -- )  \ Inicializa las variables de la trama
	battle# off
	ambrosio_follows? off
	talked_to_the_leader? off
	hacked_the_log? off
	;

\ }}}###########################################################
section( Pantalla)  \ {{{

\ ------------------------------------------------
subsection( Variables y constantes)  \ {{{

79 constant default_max_x
24 constant default_max_y
default_max_x value max_x  \ Número máximo de columna (80 columnas)
default_max_y value max_y  \ Número máximo de fila (25 filas)

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

\ }}}---------------------------------------------
subsection( Colores)  \ {{{
0  [IF]  \ ......................................

Notas sobre las pruebas realizadas en Debian
con el módulo trm de Forth Foundation Library:

TRM.HALF-BRIGHT causa subrayado, igual que TRM.UNDERSCORE-ON 
TRM.ITALIC-ON causa vídeo inverso, igual que TRM.REVERSE-ON
TRM.FOREGROUND-WHITE pone un blanco apagado diferente al predeterminado.

Referencia:
http://en.wikipedia.org/wiki/ANSI_escape_code

[THEN]  \ ......................................

trm.foreground-black-high trm.foreground-black -
constant +lighter  \ Diferencia entre los dos niveles de brillo

: color  ( u -- )  1 sgr  ;
: paper  ( u -- )  10 +  color  ;
: pen  ( u -- )  color  ;
: colors  ( u1 u2 -- )  \ Pone los colores de papel y pluma
	\ u1 = Color de papel
	\ u2 = Color de pluma
	pen paper
	;
: @colors  ( a1 a2 -- )  \ Pone los colores de papel y pluma con el contenido de dos variables
	\ a1 = Dirección del color de papel
	\ a2 = Dirección del color de pluma
	@ swap @ swap colors
	;
: lighter  ( u1 -- u2 )  +lighter +  ;

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

\ }}}---------------------------------------------
subsection( Colores utilizados)  \ {{{

\ Variables para guardar cada color de papel y de pluma

variable background_paper  \ Experimental!!!
variable about_pen
variable about_paper
variable command_prompt_pen
variable command_prompt_paper
variable debug_pen
variable debug_paper
variable description_pen
variable description_paper
variable error_pen
variable error_paper
variable input_pen
variable input_paper
variable location_description_pen
variable location_description_paper
variable location_name_pen
variable location_name_paper
variable narration_pen
variable narration_paper
variable scroll_prompt_pen
variable scroll_prompt_paper
variable question_pen
variable question_paper
variable scene_prompt_pen
variable scene_prompt_paper
variable speech_pen
variable speech_paper
variable narration_prompt_pen
variable narration_prompt_paper

: init_colors  ( -- )  \ Asigna los colores predeterminados
	[defined] background_paper [IF]
		black background_paper !  \ Experimental!!!
	[THEN] 
	dark_gray about_pen !
	black about_paper !
	cyan command_prompt_pen !
	black command_prompt_paper !
	white debug_pen !
	red debug_paper !
	dark_gray description_pen !
	black description_paper !
	light_red error_pen !
	black error_paper !
	light_cyan input_pen !
	black input_paper !
	green location_description_pen !
	black location_description_paper !
	black location_name_pen !
	green location_name_paper !
	dark_gray narration_pen !
	black narration_paper !
	green scroll_prompt_pen !
	black scroll_prompt_paper !
	white question_pen !
	black question_paper !
	green scene_prompt_pen !
	black scene_prompt_paper !
	brown speech_pen !
	black speech_paper !
	green narration_prompt_pen !
	black narration_prompt_paper !
	;

: about_color  ( -- )  \ Pone el color de texto de los créditos
	about_paper about_pen @colors 
	;
: command_prompt_color  ( -- )  \ Pone el color de texto del presto de entrada de comandos
	command_prompt_paper command_prompt_pen @colors
	;
: debug_color  ( -- )  \ Pone el color de texto usado en los mensajes de depuración
	debug_paper debug_pen @colors
	;
: system_color  ( -- )  \ Pone el color de texto predeterminado en el sistema
	trm.foreground-default
	trm.background-default
	trm.reset
	3 sgr 
	;
: description_color  ( -- )  \ Pone el color de texto de las descripciones de los entes que no son escenarios
	description_paper description_pen @colors
	;
: error_color  ( -- )  \ Pone el color de texto de los errores
	error_paper error_pen @colors
	;
: input_color  ( -- )  \ Pone el color de texto para la entrada de comandos
	input_paper input_pen @colors
	;
: location_description_color  ( -- )  \ Pone el color de texto de las descripciones de los entes escenario
	location_description_paper location_description_pen @colors
	;
: location_name_color  ( -- )  \ Pone el color de texto del nombre de los escenarios
	location_name_paper location_name_pen @colors
	;
: narration_color  ( -- )  \ Pone el color de texto de la narración
	narration_paper narration_pen @colors
	;
: scroll_prompt_color  ( -- )  \ Pone el color de texto del presto de pantalla llena
	scroll_prompt_paper scroll_prompt_pen @colors
	;
: question_color  ( -- )  \ Pone el color de texto de las preguntas de tipo «sí o no»
	question_paper question_pen @colors
	;
: scene_prompt_color  ( -- )  \ Pone el color de texto del presto de fin de escena
	scene_prompt_paper scene_prompt_pen @colors
	;
: speech_color  ( -- )  \ Pone el color de texto de los diálogos
	speech_paper speech_pen @colors
	;
: narration_prompt_color  ( -- )  \ Pone el color de texto del presto de pausa 
	narration_prompt_paper narration_prompt_pen @colors
	;

\ }}}---------------------------------------------
subsection( Demo de colores)  \ {{{

\ Dos palabras para probar cómo se ven los colores

: color_bar  ( u -- )  \ Imprime una barra de 64 espacios con el color indicado
	paper cr 64 spaces  black paper space
	;
: color_demo  ( -- )  \ Prueba los colores
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
subsection( Otros atributos)  \ {{{

: bold  ( -- )  \ Activa la negrita
	trm.bold 1 sgr
	;
: underline  ( ff -- )  \ Activa o desactiva el subrayado
	if  trm.underscore-on  else  trm.underline-off  then  1 sgr
	;
' underline alias underscore
: inverse  ( ff -- )  \ Activa o desactiva la inversión de colores (papel y pluma)
	if  trm.reverse-on  else  trm.reverse-off  then  1 sgr
	;
0  [IF]
: blink ( ff -- )  \ Activa o desactiva el parpadeo
	\ No funciona!!!
	if  trm.blink-on  else  trm.blink-off  then  1 sgr
	;
[THEN]
: italic  ( ff -- )  \ Activa o desactiva la cursiva
	\ Nota: tiene el mismo efecto que INVERSE .
	if  trm.italic-on  else  trm.italic-off  then  1 sgr
	;

\ }}}---------------------------------------------
subsection( Cursor)  \ {{{

: cursor!  ( u1 u2 -- )  \ Actualiza las variables del cursor en columna u1 y fila u2
	cursor_y !  cursor_x !
	;
: init_cursor  ( -- )  \ Pone a cero las variables del cursor
	0 dup cursor!
	;
: at-xy  ( u1 u2 -- )  \ Sitúa el cursor en columna u1 y fila u2
	2dup trm+move-cursor cursor!
	;
: home  ( -- )  \ Sitúa el cursor en la esquina superior izquierda
	0 dup at-xy
	;

\ }}}---------------------------------------------
subsection( Borrado de pantalla)  \ {{{

true  [IF]  \ Experimental!!!
: color_background  ( -- )  \ Colorea el fondo de la pantalla
	\ No se usa!!! No implementado!!!
	\ No sirve de mucho colorear la pantalla, porque la edición de textos
	\ utiliza el color de fondo predeterminado del sistema, el negro,
	\ cuando se borra el texto que está siendo escrito.
	\ No se ha comprabado si en Windows ocurre lo mismo.
	background_paper @ paper
	trm+set-default-attributes  \ Fijar los atributos actuales como predeterminados, lo que no resuelve el problema!!!
	home  max_y 0  do
		i  if  cr  then  max_x 1+  spaces
	loop
	;
[THEN]
: page  ( -- )  \ Borra la pantalla y sitúa el cursor en su origen
	trm+erase-display
	[defined] color_background  [IF]
		color_background \ Experimental!!!
	[THEN]
	home
	;

false  [IF]  \ Antiguo!!!
: new_page  ( -- )  \ Restaura el color de tinta y borra la pantalla
	background_paper @ paper page 
	;
[ELSE]
' page alias new_page
[THEN]

: clear_screen_for_location  ( -- )  \ Restaura el color de tinta y borra la pantalla para cambiar de escenario
	location_page? @  if  new_page  then
	;
: init_screen  ( -- )  \ Prepara la pantalla la primera vez
	trm+reset init_cursor system_color home
	init_colors
	;

\ }}}
\ }}}###########################################################
section( Depuración)  \ {{{

: fatal_error  ( ff a u -- )  \ Informa de un error y sale del sistema, si el indicador de error es distinto de cero
	\ No se usa!!!
	\ ff = Indicador de error
	\ a u = Mensaje de error
	rot if  ." Error fatal: " type cr bye
	else  2drop 
	then
	;
: .stack  ( -- )  \ Imprime el estado de la pila
	[false]  [IF]  \ versión antigua!!!
	." Pila" depth
	if  ." :" .s ." ( " depth . ." )"
	else  ."  vacía."
	then
	[ELSE]  \ nueva versión
	depth  if  cr ." Pila: " .s cr  then
	[THEN]
	;
: .csb  ( -- )  \ Imprime el estado del almacén circular de cadenas
	." Espacio para cadenas:" csb ?
	;
: .cursor  ( -- )  \ Imprime las coordenadas del cursor
	." Cursor:" cursor_x ? cursor_y ?
	;
: .system_status  ( -- )  \ Muestra el estado del sistema
	( .csb ) .stack ( .cursor )
	;
: .debug_message  ( a u -- )  \ Imprime el mensaje del punto de chequeo, si no está vacío
	dup  if  cr type cr  else  2drop  then
	;
: debug_pause  ( -- )  \ Pausa tras mostrar la información de depuración
	[debug_pause]  [IF]  depth  if  key drop  then  [THEN]
	;
: debug  ( a u -- )  \ Punto de chequeo: imprime un mensaje y muestra el estado del sistema
	debug_color .debug_message .system_status debug_pause
	;

\ Cargor halto2,
\ herramienta para poner puntos de comprobación

lina?
[IF]    include ayc/halto2.fs  
[ELSE]  include halto2.fs  
[THEN]  false to halto?

\ }}}###########################################################
section( Manipulación de textos)  \ {{{

str-create tmp_str  \ Cadena dinámica de texto temporal para usos variados

: str-get-last-char  ( a -- c )  \ Devuelve el último carácter de una cadena dinámica
	dup str-length@ 1- swap str-get-char 
	;
: str-get-last-but-one-char  ( a -- c )  \ Devuelve el penúltimo carácter de una cadena dinámica
	dup str-length@ 2 - swap str-get-char 
	;

gforth?  [IF]
\ En Gforth utilizamos su propia palabra nativa, más rápida.
' toupper alias ascii-char-uppercase 
[ELSE]
\ Para el resto de sistemas la definimos, simplificando la 
: ascii-char-uppercase ( c -- c1 )  \ Convierte a mayúsculas una letra ASCII
	\ Versión simplificada de la definición de CHAR-UPPERCASE
	\ en la librería de SP-Forth (lib/string/uppercase.y).
	dup [char] a [char] z 1+ within  if  32 -  then
	;
[THEN]

: (^uppercase)  ( a u -- )  \ Convierte en mayúsculas la primera letra de una cadena
	\ Nota:
	\   Solo funciona con caracteres ASCII.
	\   Por tante no funciona con caracteres UTF-8 de más de un octeto.
	\   Esto debe tenerse en cuenta, pues el código fuente y los textos están en UTF-8.
	if  dup c@ ascii-char-uppercase swap c!
	else  drop
	then
	;
: ^uppercase  ( a1 u -- a2 u )  \ Hace una copia de una cadena en el almacén circular y la devuelve con la primera letra en mayúscula
	\ Nota: Se necesita para los casos en que no queremos
	\ modificar la cadena original.
	>csb 2dup (^uppercase)
	;
: ?^uppercase  ( a1 u ff -- a1 u | a2 u )  \ Hace una copia de una cadena en el almacén circular y la devuelve con la primera letra en mayúscula, dependiendo del valor de una bandera
	\ No se usa!!!
	if  ^uppercase  then
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
: *>verb_ending  ( a u ff -- )  \ Cambia por «n» (terminación verbal en plural) los asteriscos de un texto, o los quita
	\ Se usa para convertir en plural o singular los verbos de una frase.
	\ a u = Expresión
	\ ff = ¿Hay que poner los verbos en plural?
\ Versión antigua!!!:
\	if  s" n"  else  s" "  then  s" *" sreplace 
\ Versión nueva!!!:
	s" n" rot and  s" *" sreplace 
	;
: char>string  ( c u -- a u )  \ Crea una cadena repitiendo un carácter
	\ c = Carácter
	\ u = Longitud de la cadena
	\ a = Dirección de la cadena
	dup 'csb swap 2dup 2>r  rot fill  2r>
	;

: space+  ( a1 u1 -- a2 u2 )  \ Añade un espacio a una cadena
	s"  " s+
	;
: period+  ( a1 u1 -- a2 u2 )  \ Añade un punto final a una cadena
	s" ." s+
	;
: comma+  ( a1 u1 -- a2 u2 )  \ Añade una coma al final de una cadena
	s" ," s+
	;
: colon+  ( a1 u1 -- a2 u2 )  \ Añade dos puntos al final de una cadena
	s" :" s+
	;
: dash2+  ( a1 u1 -- a2 u2 )  \ Añade un guion a una cadena
	\ Pendiente!!! elegir otro nombre
	s" -" s+
	;
: and&  ( a1 u1 -- a2 u2 )  \ Añade una conjunción «y» al final de una cadena
	\ No se usa!!!
	s" y" s&
	;
: or&  ( a1 u1 -- a2 u2 )  \ Añade una conjunción «o» al final de una cadena
	\ No se usa!!!
	s" o" s&
	;

s" " sconstant 0$  \ Cadena de longitud cero

\ }}}###########################################################
section( Textos aleatorios)  \ {{{

0  [IF]  \ ......................................

Casi todas las palabras de esta sección devuelven una cadena
calculada al azar. Las restantes palabras son auxiliares.

Por convención, en todo el programa, las palabras que
devuelven una cadena sin recibir parámetros en la pila
tienen el signo «$» al final de su nombre.  También por
tanto las constantes de cadena creadas con SCONSTANT .

[THEN]  \ ......................................

: old_man$  ( -- a u )  \ Devuelve una forma de llamar al líder de los refugiados
	s{ s" hombre" s" viejo" s" anciano" }s
	;
: with_him$  ( -- a u )  \ Devuelve una variante de «consigo» o una cadena vacía
	s{ 0$ s" consigo" s" encima" }s
	;
: with_you$  ( -- a u )  \ Devuelve «contigo» o una cadena vacía
	s" contigo" s?
	;
: carries$  ( -- a u )  \ Devuelve una variante de «lleva»
	s{ s" tiene" s" lleva" }s
	;
: you_carry$  ( -- a u )  \ Devuelve una variante de «llevas»
	s{ s" tienes" s" llevas" }s
	;
: ^you_carry$  ( -- a u )  \ Devuelve una variante de «Llevas» (con la primera mayúscula)
	you_carry$ ^uppercase
	;
: now$  ( -- a u )  \ Devuelve una variante de «ahora» o una cadena vacía
	s{ 0$ s" ahora" s" en este momento" s" en estos momentos" }s
	;
: now_$  ( -- a u )  \ Devuelve el resultado de NOW$ o una cadena vacía
	\ Sirve como versión de NOW$ con mayor probabilidad devolver una cadena vacía.
	now$ s?
	;
: here$  ( -- a u )  \ Devuelve una variante de «aquí» o una cadena vacía
	s{ 0$ s" en este lugar" s" por aquí" s" aquí" }s
	;
: now|here$  ( -- a u )  \ Devuelve el resultado de NOW$ o el de HERE$ 
	s{ now$ here$ }s
	;
: only$  ( -- a u )  \ Devuelve una variante de «solamente»
	s{ s" tan solo" s" solo" s" solamente" s" únicamente" }s
	;
: ^only$  ( -- a u )  \ Devuelve una variante de «Solamente» (con la primera mayúscula)
	\ Nota: no se puede calcular este texto a partir de la versión en minúsculas, porque el cambio entre minúsculas y mayúsculas no funciona con caracteres codificados en UTF-8 de más de un octeto.
	s{ s" Tan solo" s" Solo" s" Solamente" s" Únicamente" }s
	;
: only_$  ( -- a u )  \ Devuelve una variante de «solamente» o una cadena vacía
	only$ s?
	;
: ^only_$  ( -- a u )  \ Devuelve una variante de «Solamente» (con la primera mayúscula) o u una cadena vacía
	^only$ s?
	;
: again$  ( -- a u )
	s{ s" de nuevo" s" otra vez" s" otra vez más" s" una vez más" }s
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
: possible1$  ( -- a u )  \ Devuelve «posible» o una cadena vacía
	s" posible" s?
	;
: possible2$  ( -- a u )  \ Devuelve «posibles» o una cadena vacía
	s" posibles" s?
	;
: all_your$  ( -- a u )  \ Devuelve una variante de «todos tus»
	s{ s" todos tus" s" tus" }s
	;
: ^all_your$  ( -- a u )  \ Devuelve una variante de «Todos tus» (con la primera mayúscula)
	all_your$ ^uppercase 
	;
: soldiers$  ( -- a u )  \ Devuelve una variante de «soldados»
	s{ s" hombres" s" soldados" }s 
	;
: your_soldiers$  ( -- a u )  \ Devuelve una variante de "tus hombres"
	s" tus" soldiers$ s&
	;
: ^your_soldiers$  ( -- a u )  \ Devuelve una variante de "Tus hombres"
	your_soldiers$ ^uppercase
	;
: the_enemies$  ( -- a u )  \ Devuelve una variante de «los enemigos»
	s{ s" los sajones"
	s{ s" las tropas" s" las huestes" }s
	s{ s" enemigas" s" sajonas" }s& }s
	;
: the_enemy$  ( -- a u )  \ Devuelve una variante de «el enemigo»
	s{ s" el enemigo"
	s{ s" la tropa" s" la hueste" }s
	s{ s" enemiga" s" sajona" }s& }s
	;
: (the_enemy|enemies)  ( -- a u ff )  \ Devuelve una variante de «el/los enemigo/s», y un indicador del número
	\ a u = Cadena con el texto
	\ ff = ¿El texto está en plural?
	2 random dup  if  the_enemies$  else  the_enemy$  then  rot
	;
: the_enemy|enemies$  ( -- a u )  \ Devuelve una variante de «el/los enemigo/s»
	(the_enemy|enemies) drop
	;
: «de_el»>«del»  ( a1 u1 -- a1 u1 | a2 u2 )  \ Remplaza las apariciones de «de el» en una cadena por «del»
	s" del " s" de el " sreplace
	;
: of_the_enemy|enemies$  ( -- a u )  \ Devuelve una variante de «del/de los enemigo/s»
	(the_enemy|enemies) >r
	s" de" 2swap s&
	r> 0=  if  «de_el»>«del»  then
	;
: ^the_enemy|enemies  ( -- a u ff )  \ Devuelve una variante de «El/Los enemigo/s», y un indicador del número
	\ a u = Cadena con el texto
	\ ff = ¿El texto está en plural?
	(the_enemy|enemies) >r  ^uppercase  r>
	;
: of_your_ex_cloak$  ( -- a u )  \ Devuelve un texto común a las descripciones de los restos de la capa
	s{ 0$ s" que queda" s" que quedó" }s s" de" s&
	s{ s" lo" s" la" }s& s{ s" que antes" s" que" }s&
	s{ s" era" s" fue" s" fuera" }s&
	s{ s" tu" s" la" }s& s" oscura" s?&
	s" capa" s& s" de lana" s?& period+
	;
: though$  ( -- a u )
	s{ s" si bien" s" pero" s" aunque" }s
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
: very_$  ( -- a u )  \ Devuelve el resultado de very$ o una cadena vacía
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
: pass_way$  ( -- a u )  \ Devuelve una variante de «pasaje»
	s{ s" paso" s" pasaje" }s
	;
: a_pass_way$  ( -- a u )  \ Devuelve una variante de «un pasaje»
	s" un" pass_way$ s&
	;
: ^a_pass_way$  ( -- a u )  \ Devuelve una variante de «Un pasaje» (con la primera mayúscula)
	a_pass_way$ ^uppercase
	;
: the_pass_way$  ( -- a u )  \ Devuelve una variante de «el pasaje»
	s" el" pass_way$ s&
	;
: ^the_pass_way$  ( -- a u )  \ Devuelve una variante de «El pasaje» (con la primera mayúscula)
	the_pass_way$ ^uppercase
	;
: pass_ways$  ( -- a u )  \ Devuelve una variante de «pasajes»
	pass_way$ s" s" s+
	;
: ^pass_ways$  ( -- a u )  \ Devuelve una variante de «Pasajes» (con la primera mayúscula)
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
: can_see$  ( -- a u )  \ Devuelve una forma de decir «ves»
	s{ s" ves" s" se ve" s" puedes ver" }s
	;
: ^can_see$  ( -- a u )  \ Devuelve una forma de decir «ves», con la primera letra mayúscula
	can_see$ ^uppercase
	;
: cannot_see$  ( -- a u )  \ Devuelve una forma de decir «no ves»
	s" no" can_see$ s&
	;
: ^cannot_see$  ( -- a u )  \ Devuelve una forma de decir «No ves»
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
: in_half-darkness_you_glimpse$  ( -- a u )  \ Devuelve un texto usado en varias descripciones de las cuevas
	s" En la" s{ s" semioscuridad," s" penumbra," }s& s? dup
	if  can_glimpse$  else  ^can_glimpse$  then  s&
	;
: you_glimpse_the_cave$  ( -- a u)  \ Devuelve un texto usado en varias descripciones de las cuevas
	\ Pendiente!!! Distinguir la antorcha encendida.
	in_half-darkness_you_glimpse$ s" la continuación de la cueva." s&
	;
: rimarkable$  ( -- a u )  \ Devuelve una variante de «destacable»
	s{ s" de especial" s" de particular"
	s" de peculiar" s" destacable" 
	s" especial" s" peculiar"
	s" que llame la atención" s" que destacar" }s
	;
: has_nothing$  ( -- a u )
	s" no tiene nada"
	;
: is_normal$  ( -- a u )  \ Devuelve una variante de «no tiene nada especial»
	has_nothing$ rimarkable$ s&
	;
: ^is_normal$  ( -- a u )  \ Devuelve una variante de «No tiene nada especial» (con la primera letra en mayúscula)
	is_normal$ ^uppercase
	;
: over_there$  ( -- a u )
	s{ s" allí" s" allá" }s
	;
: goes_down_into_the_deep$  ( -- a u )  \ Devuelve una variante de «desciende a las profundidades»
	s{ s" desciende" toward$ s& s" se adentra en"
	s" conduce" toward$ s& s" baja" toward$ s& }s
	s" las profundidades" s&
	;
: in_that_direction$  ( -- a u )  \ Devuelve una variante de «en esa dirección»
	s{ s" en esa dirección" s{ s" por" s" hacia" }s over_there$ s& }s
	;
: ^in_that_direction$  ( -- a u )  \ Devuelve una variante de «En esa dirección»
	in_that_direction$ ^uppercase
	;
: (uninteresting_direction_0)$  ( -- a u )  \ Devuelve primera variante de «En esa dirección no hay nada especial»
	s{ s" Esa dirección" is_normal$ s&
	^in_that_direction$ s" no hay nada" s& rimarkable$ s&
	^in_that_direction$ cannot_see$ s& s" nada" s& rimarkable$ s&
	}s period+
	;
: (uninteresting_direction_1)$  ( -- a u )  \ Devuelve segunda variante de «En esa dirección no hay nada especial»
	s{ 
	^is_normal$ s" esa dirección" s&
	^cannot_see$ s" nada" s& rimarkable$ s& in_that_direction$ s&
	s" No hay nada" rimarkable$ s& in_that_direction$ s&
	}s period+
	;
: uninteresting_direction$  ( -- a u )  \ Devuelve una variante de «En esa dirección no hay nada especial»
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
: (rocks)_on_the_floor$  ( -- a u )  \ Devuelve un texto sobre las rocas que ya han sido desmoronadas
	s" yacen desmoronadas" s" a lo largo del pasaje" s?&
	;
: (rocks)_clue$  ( -- a u )  \ Devuelve una descripción de las rocas que sirve de pista
	s" Son" s{ s" muchas" s" muy" s? s" numerosas" s& }s& comma+
	s" aunque no parecen demasiado pesadas y" s&
	s{ s" pueden verse" s" se ven" s" hay" }s s" algunos huecos" s&
	s" entre ellas" r2swap s& s&
	;
: from_that_way$  ( - u )  \
	s" de" s{ s" esa dirección" s" allí" s" ahí" s" allá" }s&
	;
: that_way$  ( -- a u )  \ Devuelve una variante de «en esa dirección»
	s{ s" en esa dirección" s" por" s{ s" ahí" s" allí" s" allá" }s& }s
	;
: ^that_way$  ( -- a u )  \ Devuelve una variante de «En esa dirección» (con la primera letra mayúscula)
	that_way$ ^uppercase
	;
: gets_wider$  ( -- a u )  \ Devuelve una variante de «se ensancha»
	s{
	s" se" s{ s" anchea" s" ensancha" s" va ensanchando"
	s" va haciendo más ancho" s" hace más ancho"
	s" vuelve más ancho" s" va volviendo más ancho" }s&
	2dup 2dup 2dup \ Aumentar las probabilidades de la primera variante
	s{ s" ensánchase" s" hácese más ancho" s" vuélvese más ancho" }s
	}s
	;
: (narrow)$  ( -- a u )
	s{ s" estrech" s" angost" }s
	;
: narrow(f)$  ( -- a u )  \ Devuelve una variante de «estrecha»
	(narrow)$ s" a" s+
	;
: narrow(m)$  ( -- a u )  \ Devuelve una variante de «estrecho»
	(narrow)$ s" o" s+
	;
: narrow(mp)$  ( -- a u )  \ Devuelve una variante de «estrechos»
	narrow(m)$ s" s" s+
	;
: ^narrow(mp)$  ( -- a u )  \ Devuelve una variante de «Estrechos» (con la primera mayúscula)
	narrow(mp)$  ^uppercase
	;
: gets_narrower(f)$  ( -- a u )  \ Devuelve una variante de «se hace más estrecha» (femenino)
	s{
	s" se" s{ s" estrecha" s" va estrechando" }s&
	2dup \ Aumentar las probabilidades de la primera variante
	s" se" s{ s" va haciendo más" s" hace más"
	s" vuelve más" s" va volviendo más" }s& narrow(f)$ s&
	2dup \ Aumentar las probabilidades de la segunda variante
	s{ s" estréchase" s{ s" hácese" s" vuélvese" }s s" más" s& narrow(f)$ s& }s
	}s
	;
: goes_up$  ( -- a u )  \ Devuelve una variante de «sube»
	s{ s" sube" s" asciende" }s
	;
: (they)_go_up$  ( -- a u )  \ Devuelve una variante de «suben»
	goes_up$ s" n" s+
	;
: goes_down$  ( -- a u )  \ Devuelve una variante de «baja»
	s{ s" baja" s" desciende" }s
	;
: (they)_go_down$  ( -- a u )  \ Devuelve una variante de «bajan»
	goes_down$ s" n" s+
	;
: almost_invisible(plural)$  ( -- a u )  \ Devuelve una variante de «casi imperceptibles»
	s" casi" s{ s" imperceptibles" s" invisibles" s" desapercibidos" }s
	\ Confirmar significados!!!
	;
: ^a_narrow_pass_way$  ( -- a u )
	s" Un" narrow(m)$ pass_way$ r2swap s& s&
	;
: beautiful(m)$  ( -- a u )
	s{ s" bonito" s" bello" s" hermoso" }s
	;
: a_snake_blocks_the_way$  ( -- a u )
	s" Una serpiente"
	s{ s" bloquea" s" está bloqueando" }s&
	the_pass$ s& toward_the(m)$ s" Sur" s& s?&
	;
: the_water_flow$  ( -- a u )
	\ Pendiente!!! confirmar nombre en inglés
	s" la" s{ s" caudalosa" s" furiosa" s" fuerte" s" brava" }s&
	s" corriente" s& s" de agua" s?&
	;
: ^the_water_flow$  ( -- a u )
	the_water_flow$ ^uppercase
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
: narrow_cave_pass$  ( -- a u )  \ Devuelve una variante de «estrecho tramo de cueva»
	s" tramo de cueva" narrow(m)$ r2swap s&
	;
: a_narrow_cave_pass$  ( -- a u )  \ Devuelve una variante de «un estrecho tramo de cueva»
	s" un" narrow_cave_pass$ s&
	;
: but|and$  ( -- a u )
	s{ s" y" s" pero" }s
	;
' but|and$ alias and|but$

\ }}}###########################################################
section( Cadena dinámica para impresión)  \ {{{

0  [IF]  \ ......................................

Usamos una cadena dinámica llamada PRINT_STR para guardar
los párrafos enteros que hay que mostrar en pantalla. En
esta sección creamos la cadena y palabras útiles para
manipularla.

[THEN]  \ ......................................

str-create print_str  \ Cadena dinámica para almacenar el texto antes de imprimirlo justificado

: «»-clear  ( -- )  \ Vacía la cadena dinámica PRINT_STR
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
: «c+  ( c -- )  \ Añade un carácter al principio de la cadena dinámica PRINT_STR
	print_str str-prepend-char
	;
: »c+  ( c -- )  \ Añade un carácter al final de la cadena dinámica PRINT_STR
	print_str str-append-char
	;
: «»bl+?  ( u -- ff )  \ ¿Se debe añadir un espacio al concatenar una cadena a la cadena dinámica PRINT_STR ?
	\ u = Longitud de la cadena que se pretende unir a la cadena dinámica PRINT_STR
	0<> print_str str-length@ 0<> and
	;
: »&  ( a u -- )  \ Añade una cadena al final de la cadena dinámica TXT, con un espacio de separación
	dup «»bl+?  if  bl »c+  then  »+
	;
: «&  ( a u -- )  \ Añade una cadena al principio de la cadena dinámica TXT, con un espacio de separación
	dup «»bl+?  if  bl «c+  then  «+ 
	;

\ }}}###########################################################
section( Impresión de textos)  \ {{{

variable #lines  \ Número de línea del texto que se imprimirá
variable scroll  \ Indicador de que la impresión no debe parar

\ ------------------------------------------------
subsection( Presto de pausa en la impresión de párrafos)  \ {{{

svariable scroll_prompt  \ Guardará el presto de pausa
: scroll_prompt$  ( -- a u )  \ Devuelve el presto de pausa
	scroll_prompt count
	;
1 value /scroll_prompt  \ Número de líneas de intervalo para mostrar un presto

: scroll_prompt_key  ( -- )  \ Espera la pulsación de una tecla y actualiza con ella el estado del desplazamiento
	key  bl =  scroll !
	;
: .scroll_prompt  ( -- )  \ Imprime el presto de pausa, espera una tecla y borra el presto
	trm+save-cursor  scroll_prompt_color
	scroll_prompt$ type  scroll_prompt_key
	trm+erase-line  trm+restore-cursor
	;
: (scroll_prompt?)  ( u -- ff )  \ ¿Se necesita imprimir un presto para la línea actual?
	\ u = Línea actual del párrafo que se está imprimiendo
	\ Se tienen que cumplir dos condiciones:
	dup 1+ #lines @ <>  \ ¿Es distinta de la última?
	swap /scroll_prompt mod 0=  and  \ ¿Y el intervalo es correcto?
	;
: scroll_prompt?  ( u -- ff )  \ ¿Se necesita imprimir un presto para la línea actual?
	\ u = Línea actual del párrafo que se está imprimiendo
	\ Si el valor de SCROLL es «verdadero», se devuelve «falso»; si no, se comprueban las otras condiciones.
	\ ." L#" dup . ." /" #lines @ . \ Depuración!!!
	scroll @  if  drop false  else  (scroll_prompt?)  then
	;
: .scroll_prompt?  ( u -- )  \ Imprime un presto y espera la pulsación de una tecla, si corresponde a la línea en curso
	\ u = Línea actual del párrafo que se está imprimiendo
	scroll_prompt?  if  .scroll_prompt  then
	;

\ }}}---------------------------------------------
subsection( Impresión de párrafos ajustados)  \ {{{

variable /indentation  \ Longitud de la indentación de cada párrafo
8 constant max_indentation
: line++  ( -- )  \ Incrementa el número de línea, si se puede
	\ No se usa!!!
	\ Versión para no pasar del máximo:
	\   cursor_y @ 1+ max_y 1- min cursor_y !
	\ Versión circular para pasar del máximo a cero:
	\	cursor_y dup @ 1+ dup max_y < abs * swap !
	\ Versión circular, con puesta a cero de la columna:
	\	cursor_y @ 1+ dup max_y < abs *  0 swap at-xy
	\ 2011-12-01 Cambio!!!:
	cursor_y @ 1+ dup max_y < and  0 swap at-xy
	;
: cr+  ( -- )  \ Hace un salto de línea y actualiza el cursor
	cr cursor_y ++ cursor_x off
	;
: ?cr  ( u -- )  \ Hace un salto de línea si hace falta
	\ u = Longitud en caracteres del párrafo que ha sido imprimido
	0> cr? @ and  if  cr+  then
	;
: not_first_line?  ( -- ff )  \ ¿La línea de pantalla donde se imprimirá es la primera?
	cursor_y @ 0>
	;
variable indent_first_line_too?  \ ¿Se indentará también la línea superior de la pantalla, si un párrafo empieza en ella?
: indentation?  ( -- ff )  \ ¿Indentar la línea actual?
	not_first_line? indent_first_line_too? @ or
	;
: indentation+  ( -- )  \ Añade indentación ficticia (con un carácter distinto del espacio) a la cadena dinámica PRINT_STR, si la línea del cursor no es la primera
	indentation?  if
		[char] X /indentation @ char>string «+
	then
	;
: indentation-  ( a1 u1 -- a2 u2 )  \ Quita a una cadena tantos caracteres por la izquierda como el valor de la indentación
	/indentation @ -  swap /indentation @ +  swap
	;
: indent  ( -- )  \ Mueve el cursor a la posición requerida por la indentación
	/indentation @ ?dup  if  trm+move-cursor-right  then
	;
: indentation>  ( a1 u1 -- a2 u2 ) \ Prepara la indentación de una línea
	[debug]  [IF]  s" Al entrar en INDENTATION>" debug  [THEN]  \ Depuración!!!
	indentation?  if  indentation- indent  then
	[debug]  [IF]  s" Al salir de INDENTATION>" debug  [THEN]  \ Depuración!!!
	;
: .line  ( a u -- )  \ Imprime una línea de texto y un salto de línea
	[debug]  [IF]  s" En .LINE" debug  [THEN]  \ Depuración!!!
	type cr+
	;
: .lines  ( a1 u1 ... an un n -- )  \ Imprime n líneas de texto
	\ a1 u1 = Última línea de texto
	\ an un = Primera línea de texto
	\ n = Número de líneas de texto en la pila
	dup #lines !  scroll on
	0  ?do  .line  i .scroll_prompt?  loop
	;
: (paragraph)  ( -- )  \ Imprime la cadena dinámica PRINT_STR ajustándose al ancho de la pantalla
	indentation+  \ Añade indentación ficticia
	print_str str-get max_x str+columns  \ Divide la cadena dinámica PRINT_STR en tantas líneas como haga falta
	[debug]  [IF]  s" En (PARAGRAPH)" debug  [THEN]  \ Depuración!!!
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
	error_color paragraph system_color
	;
: narrate  ( a u -- )  \ Imprime una cadena como una narración
	narration_color paragraph system_color
	;

\ }}}---------------------------------------------
subsection( Pausas y prestos en la narración)  \ {{{

0  [IF]  \ ......................................

En SP-Forth para Linux, la palabra KEY? de ANS Forth
devuelve siempre cero porque aún no está completamente
implementada, mientras que en la versión para Windows sí
funciona correctamente.  Esto impide, en Linux, crear pausas
de duración máxima que puedan ser interrumpidas con una
pulsación de teclas.

Por ello hemos optado por un sistema alternativo.

[THEN]  \ ......................................

false  [IF]  \ Sistema original descartado, que funciona en SP-Forth para Windows

dtm-create deadline  \ Variable para guardar el momento final de las pausas

: no_time_left?  ( -- ff )  \ ¿Se acabó el tiempo?
	0 time&date  \ Fecha y hora actuales (más cero para los milisegundos)
	deadline dtm-compare  \ Comparar con el momento final (el resultado puede ser: -1, 0, 1)
	1 =  \ ¿Nos hemos pasado?
	;
: no_key?  ( -- ff )  \ ¿No hay una tecla pulsada?
	key? 0=
	;
: seconds_wait ( u -- )  \ Espera los segundos indicados, o hasta que se pulse una tecla
	deadline dtm-init  \ Guardar la fecha y hora actuales como límite...
	s>d deadline dti-seconds+  \ ...y sumarle los segundos indicados
	begin  no_time_left? no_key? or  until
	begin  no_time_left? key? or  until
	;
: narration_break  ( -- )  \ Hace una pausa en la narración; se usa entre ciertos párrafos
	1 seconds_wait
	;
: scene_break  ( -- )  \ Hace una pausa en la narración; se usa entre ciertos párrafos
	3 seconds_wait
	;

[THEN]

\ Sistema nuevo, con pausas fijas en milisegundos o indefinidas hasta la pulsación de una tecla

variable indent_pause_prompts?  \ ¿Hay que indentar también los prestos?
: .prompt  ( a u -- )  \ Imprime un presto
	indent_pause_prompts? @  if  indent  then  type
	;
: wait  ( u -- )  \ Hace una pausa
	\ u = Milisegundos (o un número negativo para pausa sin fin hasta la pulsación de una tecla)
	dup 0<  if  key 2drop  else  ms  then
	;
variable narration_break_milliseconds  \ Milisegundos de espera en las pausas de la narración
svariable narration_prompt  \ Guardará el presto usado en las pausas de la narración
: narration_prompt$  ( -- a u )  \ Devuelve el presto usado en las pausas de la narración
	narration_prompt count
	;
: .narration_prompt  ( -- )  \ Imprime el presto de fin de escena
	narration_prompt_color narration_prompt$ .prompt
	;
: (narration_break)  ( n -- )  \ Alto en la narración: Muestra un presto y hace una pausa 
	\ u = Milisegundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
	trm+save-cursor
	.narration_prompt wait
	trm+erase-line  trm+restore-cursor
	;
: narration_break  ( -- )  \ Alto en la narración, si es preciso
	narration_break_milliseconds @ ?dup
	if  (narration_break)  then
	;

variable scene_break_milliseconds  \ Milisegundos de espera en las pausas de final de escena
svariable scene_prompt  \ Guardará el presto de cambio de escena
: scene_prompt$  ( -- a u )  \ Devuelve el presto de cambio de escena
	scene_prompt count
	;
: .scene_prompt  ( -- )  \ Imprime el presto de fin de escena
	scene_prompt_color scene_prompt$ .prompt
	;
: (scene_break)  ( n -- )  \ Final de escena: Muestra un presto y hace una pausa 
	\ n = Milisegundos (o un número negativo para hacer una pausa indefinida hasta la pulsación de una tecla)
	trm+save-cursor
	.scene_prompt wait
	trm+erase-line  trm+restore-cursor
	scene_page? @  if  new_page  then
	;
: scene_break  ( -- )  \ Final de escena, si es preciso
	scene_break_milliseconds @ ?dup
	if  (scene_break)  then
	;

\ }}}---------------------------------------------
subsection( Impresión de citas de diálogos)  \ {{{

s" —" sconstant dash$  \ Raya (código Unicode 2014 en hexadecimal, 8212 en decimal)
s" «" sconstant lquote$ \ Comilla castellana de apertura
s" »" sconstant rquote$  \ Comilla castellana de cierre
: str-with-rquote-only?  ( a -- ff )  \ ¿Hay en una cadena dinámica una comilla castellana de cierre pero no una de apertura?
	>r rquote$ 0 r@ str-find -1 >  \ ¿Hay una comilla de cierre en la cita?
	lquote$ 0 r> str-find -1 = And  \ ¿Y además falta la comilla de apertura? 
	;
: str-with-period?  ( a -- ff )  \ ¿Termina una cadena dinámica con un punto? 
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
: quoted  ( a1 u1 -- a2 u2 )  \ Pone comillas o raya a una cita de un diálogo
	castilian_quotes? @  if  quotes+  else  dash+  then  
	;
: speak  ( a u -- )  \ Imprime una cita de un diálogo
	quoted speech_color paragraph system_color
	;

\ }}}
\ }}}###########################################################
section( Definición de la ficha de un ente)  \ {{{

0  [IF]  \ ......................................

Denominamos «ente» a cualquier componente del mundo virtual
del juego que es manipulable por el programa.  «Entes» por
tanto son los objetos, manipulables o no por el jugador; los
personajes, interactivos o no; los lugares; y el propio
personaje protagonista. 

Cada ente tiene una ficha en la base de datos del juego.  La
base de datos es una zona de memoria dividida en partes
iguales, una para cada ficha. El identificador de cada ficha
es una palabra que al ejecutarse deja en la pila la
dirección de memoria donde se encuentra la ficha.

Los campos de la base de datos, como es habitual en Forth en
este tipo de estructuras, son palabras que suman el
desplazamiento adecuado a la dirección base de la ficha, que
reciben en la pila, apuntando así a la dirección de memoria
que contiene el campo correspondiente. 

La palabra provista por SP-Forth para crear campos de
estructuras de datos es -- (dos guiones); para usar este
nombre de palabra para otro uso (decrementar variables), en
este programa se ha creado un alias para la palabra -- con
el nombre de OFFSET: (tomado de las utilidades ToolBelt de
Will Baden, de 2003), aunque el nombre habitual en la
mayoría de las implementaciones es FIELD .

El funcionamiento de OFFSET: es muy sencillo. Toma de la
pila dos valores: el inferior es el desplazamiento en
octetos desde el inicio del «registro» (que en este programa
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

En cada ficha hay campos binarios que podrían almacenarse en
un solo bitio, y otros que no precisarían más de un octeto.
Pero cuando la memoria usada por cada campo no es la misma
para todos, las operaciones para leer y modificar los campos
tienen ser diferentes también en cada caso.  Para evitar
este inconveniente se podría escribir fácilmente un conjunto
de palabras que actuara como capa superior de abstracción
para manipular los campos de datos, escondiendo las
interioridades de cómo se guarda efectivamente cada dato en
la ficha (en un bitio, en un octeto, o en varios).

No obstante, dado que la cantidad de datos es pequeña y la
memoria disponible no es un condicionante, y con el objetivo
de simplificar el código, hemos optado por usar el mismo
tamaño en todos los campos: una «celda».

La «celda» es un concepto de ANS Forth: es la unidad en que
se mide el tamaño de cada elemento de la pila, y capaz por
tanto de contener una dirección de memoria.  En los sistemas
Forth de 8 o 16 bitios una celda equivale a un valor de 16
bitios; en los sistemas Forth de 32 bitios, como SP-Forth,
una celda equivale a un valor de 32 bitios.

El contenido de un campo puede representar un número (con o
sin signo), un indicador buleano o una dirección de memoria
(de una cadena de texto, de una palabra de Forth, de la
ficha de otro ente, de otra estructura de datos...).

Para facilitar la legibilidad, los nombres de los campos
empiezan con el signo de tilde («~»); los que contienen
datos buleanos terminan con una interrogación («?»);  los
que contienen direcciones de ejecución terminan con «_xt»;
los que contienen códigos de error terminan «_error#».

[THEN]  \ .......................................

0 \ Valor inicial de desplazamiento para el primer campo
cell offset: ~name_str  \ Dirección de una cadena dinámica que contendrá el nombre del ente
cell offset: ~has_personal_name?  \ Indicador: ¿el nombre del ente es un nombre propio?
cell offset: ~has_feminine_name?  \ Indicador: ¿el género gramatical del nombre es femenino?
cell offset: ~has_plural_name?  \ Indicador: ¿el nombre es plural?
cell offset: ~has_no_article?  \ Indicador: ¿el nombre no debe llevar artículo?
cell offset: ~has_definite_article?  \ Indicador: ¿el artículo debe ser siempre el artículo definido?
cell offset: ~description_xt  \ Dirección de ejecución de la palabra que describe el ente
cell offset: ~init_xt  \ Dirección de ejecución de la palabra que inicializa las propiedades de un ente (experimental!!!)
cell offset: ~is_character?  \ Indicador: ¿el ente es un personaje?
cell offset: ~conversations  \ Contador para personajes: número de conversaciones tenidas con el protagonista
cell offset: ~is_decoration?  \ Indicador: ¿el ente forma parte de la decoración de su localización?
cell offset: ~take_error#  \ Identificador del error adecuado al intentar tomar el ente (cero si no hay error); se usa para casos especiales; los errores apuntados por este campo no reciben parámetros salvo en WHAT
cell offset: ~break_error#  \ Identificador del error adecuado al intentar romper el ente (cero si no hay error); se usa para casos especiales; los errores apuntados por este campo no reciben parámetros salvo en WHAT
cell offset: ~is_global_outdoor?  \ Indicador ¿el ente es global (común) en los escenarios al aire libre?
cell offset: ~is_global_indoor?  \ Indicador ¿el ente es global (común) en los escenarios interiores? 
cell offset: ~is_owned?  \ Indicador: ¿el ente pertenece al protagonista? 
cell offset: ~is_cloth?  \ Indicador: ¿el ente es una prenda que puede ser llevada como puesta?
cell offset: ~is_worn?  \ Indicador: ¿el ente, que es una prenda, está puesto? 
cell offset: ~is_light?  \ Indicador: ¿el ente es una fuente de luz que puede ser encendida?
cell offset: ~is_lit?  \ Indicador: ¿el ente, que es una fuente de luz que puede ser encendida, está encendido?
cell offset: ~is_vegetal?  \ Indicador: ¿es vegetal?
cell offset: ~is_animal?  \ Indicador: ¿es animal? 
cell offset: ~is_human?  \ Indicador: ¿es humano? 
cell offset: ~is_open?  \ Indicador: ¿está abierto? 
cell offset: ~is_location?  \ Indicador: ¿es un escenario? 
cell offset: ~location  \ Identificador del ente en que está localizado (sea escenario, contenedor, personaje o «limbo»)
cell offset: ~previous_location  \ Ídem para el ente que fue la localización antes del actual 
cell offset: ~location_plot_xt  \ Dirección de ejecución de la palabra que se ocupa de la trama del escenario
cell offset: ~visits  \ Contador de visitas del protagonista a cada ente escenario (se incrementa al abandanar el escenario)
cell offset: ~familiar  \ Contador de familiaridad (cuánto le es conocido el ente al protagonista)
cell offset: ~north_exit  \ Ente de destino hacia el Norte
cell offset: ~south_exit  \ Ente de destino hacia el Sur
cell offset: ~east_exit  \ Ente de destino hacia el Este
cell offset: ~west_exit  \ Ente de destino hacia el Oeste
cell offset: ~up_exit  \ Ente de destino hacia arriba
cell offset: ~down_exit  \ Ente de destino hacia abajo
cell offset: ~out_exit  \ Ente de destino hacia fuera
cell offset: ~in_exit  \ Ente de destino hacia dentro
cell offset: ~direction  \ Desplazamiento del campo de dirección al que corresponde el ente (solo se usa en los entes que son direcciones)

[false]  [IF]  \ Campos omitidos porque aún no se usan!!!:
cell offset: ~is_lock?  \ Indicador: ¿está cerrado con llave? 
cell offset: ~is_openable?  \ Indicador: ¿es abrible? 
cell offset: ~is_lockable?  \ Indicador: ¿es cerrable con llave? 
cell offset: ~desambiguation_xt  \ Dirección de ejecución de la palabra que desambigua e identifica el ente
cell offset: ~is_container?  \ Indicador: ¿es un contenedor?
cell offset: ~stamina  \ Energía de los entes vivos
[THEN]

constant /entity  \ Tamaño de cada ficha

\ }}}###########################################################
section( Interfaz de campos)  \ {{{

0  [IF]  \ ......................................
	
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

[THEN]  \ ......................................

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

last_exit> cell+ first_exit> - constant /exits  \ Espacio en octetos ocupado por los campos de salidas
/exits cell / constant #exits  \ Número de salidas

0 constant no_exit  \ Marcador para direcciones sin salida en un ente dirección
: exit?  ( a -- ff )  \ ¿Está abierta una dirección de salida de un ente escenario?
	\ a = Contenido de un campo de salida de un ente (que será el ente de destino, o cero)
	no_exit <>
	;

\ ------------------------------------------------
\ Interfaz básica para leer y modificar los campos

0  [IF]  \ ......................................

Las palabras que siguen permiten hacer las operaciones
básicas de obtención y modificación del contenido de los
campos. 

[THEN]  \ ......................................

\ Obtener el contenido de los campos

: break_error#  ( a -- u )  ~break_error# @  ;
: conversations  ( a -- u )  ~conversations @  ;
: description_xt  ( a -- xt )  ~description_xt @  ;
: direction  ( a -- u )  ~direction @  ;
: familiar  ( a -- u )  ~familiar @  ;
: has_definite_article?  ( a -- ff )  ~has_definite_article? @  ;
: has_feminine_name?  ( a -- ff )  ~has_feminine_name? @  ;
: has_no_article?  ( a -- ff )  ~has_no_article? @  ;
: has_personal_name?  ( a -- ff )  ~has_personal_name? @  ;
: has_plural_name?  ( a -- ff )  ~has_plural_name? @  ;
: init_xt  ( a -- xt )  ~init_xt @  ;
: is_animal?  ( a -- ff )  ~is_animal? @  ;
: is_character?  ( a -- ff )  ~is_character? @  ;
: is_cloth?  ( a -- ff )  ~is_cloth? @  ;
: is_decoration?  ( a -- ff )  ~is_decoration? @  ;
: is_global_indoor?  ( a -- ff )  ~is_global_indoor? @  ;
: is_global_outdoor?  ( a -- ff )  ~is_global_outdoor? @  ;
: is_human?  ( a -- ff )  ~is_human? @  ;
: is_light?  ( a -- ff )  ~is_light? @  ;
: is_lit?  ( a -- ff )  ~is_lit? @  ;
: is_location?  ( a -- ff )  ~is_location? @  ;
: is_open?  ( a -- ff )  ~is_open? @  ;
: is_owned?  ( a -- ff )  ~is_owned? @  ;
: is_vegetal?  ( a -- ff )  ~is_vegetal? @  ;
: is_worn?  ( a -- ff )  ~is_worn? @  ;
: location  ( a1 -- a2 )  ~location @  ;
: location_plot_xt  ( a -- xt )  ~location_plot_xt @  ;
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

: conversations++  ( a -- )  ~conversations (++)  ;
: familiar++  ( a -- )  ~familiar (++)  ;
: has_definite_article  ( a -- )  ~has_definite_article? on  ;
: has_feminine_name  ( a -- )  ~has_feminine_name? on  ;
: has_masculine_name  ( a -- )  ~has_feminine_name? off  ;
: has_no_article  ( a -- )  ~has_no_article? on  ;
: has_personal_name  ( a -- )  ~has_personal_name? on  ;
: has_plural_name  ( a -- )  ~has_plural_name? on  ;
: is_character  ( a -- )  ~is_character? on  ;
: is_cloth  ( a -- )  ~is_cloth? on  ;
: is_decoration  ( a -- )  ~is_decoration? on  ;
: is_global_indoor  ( a -- )  ~is_global_indoor? on  ;
: is_global_outdoor  ( a -- )  ~is_global_outdoor? on  ;
: is_human  ( a -- )  ~is_human? on  ;
: is_location  ( a -- )  ~is_location? on  ;
: is_open  ( a -- )  ~is_open? on  ;
: is_owned  ( a -- ff )  ~is_owned? on  ;
: is_worn  ( a -- )  ~is_worn? on  ;
: visits++  ( a -- )  ~visits (++)  ;

\ ------------------------------------------------
\ Campos calculados o seudo-campos

0  [IF]  \ ......................................

Los seudo-campos devuelven un cálculo. Sirven para añadir
una capa adicional de abstracción y simplificar el código.

Por conveniencia, en el caso de algunos de los campos
binarios creamos también palabras para la propiedad
contraria.  Por ejemplo, en las fichas existe el campo
~IS_OPEN? para indicar si un ente está abierto, pero creamos
las palabras necesarias para examinar y modificar tanto la
propiedad de «cerrado» como la de «abierto». Esto ayuda a
escribir posteriormente el código efectivo (pues no hace
falta recordar si la propiedad real y por tanto el campo de
la ficha del ente era «abierto» o «cerrado») y hace el
código más conciso y legible.

Pendiente!!! Hay que unificar el criterio de los nombres de
estas palabras, y sacar de aquí las que se refieran a campos
básicos. 

[THEN]  \ ......................................

: is_direction?  ( a -- ff )  direction 0<>  ;
: is_familiar?  ( a -- ff )  familiar 0>  ;
: is_visited?  ( a -- ff )  visits 0>  ;
: is_not_visited?  ( a -- ff )  visits 0=  ;
: conversations?  ( a -- ff )  conversations 0<>  ;
: no_conversations?  ( a -- ff )  conversations 0=  ;
: has_north_exit?  ( a -- ff )  north_exit exit?  ;
: has_east_exit?  ( a -- ff )  east_exit exit?  ;

: is_closed?  ( a -- ff )  is_open? 0=  ;
: is_closed  ( a -- )  ~is_open? off  ;
: has_singular_name?  ( a -- ff )  has_plural_name? 0=  ;
: has_masculine_name?  ( a -- ff )  has_feminine_name? 0=  ;

: is_living_being?  ( a -- ff )  \ ¿El ente es un ser vivo (aunque esté muerto)?
	dup is_vegetal?
	over is_animal? or
	swap is_human? or
	;
: is_there  ( a1 a2 -- )  \ Hace que un ente sea la localización de otro
	\ a1 = Ente que será la localización de a2
	\ a2 = Ente cuya localización será a1
	~location !
	;
: was_there  ( a1 a2 -- )  \ Hace que un ente sea la localización previa de otro
	\ a1 = Ente que será la localización previa de a2
	\ a2 = Ente cuya localización previa será a1
	~previous_location !
	;
: is_there?  ( a1 a2 -- ff )  \ ¿Está un ente localizado en otro?
	\ a1 = Ente que actúa de localización
	\ a2 = Ente cuya localización se comprueba
	location =
	;
: was_there?  ( a1 a2 -- ff )  \ ¿Estuvo un ente localizado en otro?
	\ a1 = Ente que actúa de localización
	\ a2 = Ente cuya localización se comprueba
	previous_location =
	;
: is_global?  ( a -- ff )  \ ¿Es el ente un ente global?
	dup is_global_outdoor?
	swap is_global_indoor? or
	;
: my_location  ( -- a )  \ Devuelve la localización del protagonista
	protagonist% location
	;
: my_location!  ( a -- )  \ Mueve el protagonista al ente indicado
	protagonist% is_there
	;
: am_i_there?  ( a -- ff )  \ ¿Está el protagonista en la localización indicada?
	\ a = Ente que actúa de localización
	my_location =
	;
: is_outdoor_location?  ( a -- ff )  \ ¿Es el ente un escenario al aire libre?
	\ Cálculo provisional!!!
	drop 0
	;
: is_indoor_location?  ( a -- ff )  \ ¿Es el ente un escenario cerrado, no al aire libre?
	is_outdoor_location? 0=
	;
: am_i_outdoor?  ( -- ff )  \ ¿Está el protagonista en un escenario al aire libre?
	my_location is_outdoor_location?
	;
: am_i_indoor?  ( -- ff )  \ ¿Está el protagonista en un escenario cerrado, no al aire libre?
	am_i_outdoor? 0=
	;
: is_hold?  ( a -- ff )  \ ¿Es el protagonista la localización de un ente?
	location protagonist% =
	;
: is_hold  ( a -- )  \ Hace que el protagonista sea la localización de un ente
	~location protagonist% swap !
	;
: is_worn_by_me?  ( a -- )  \ ¿El protagonista lleva puesto el ente indicado?
	dup is_hold?  swap is_worn?  and
	;
: is_known?  ( a -- ff )  \ ¿El protagonista ya conoce el ente?
	dup is_owned?  \ ¿Es propiedad del protagonista?
	over is_visited? or  \ ¿O es un escenario ya visitado? (si no es un escenario, la comprobación no tendrá efecto)
	over conversations? or  \ ¿O ha hablado ya con él? (si no es un personaje, la comprobación no tendrá efecto)
	swap is_familiar?  or  \ ¿O ya le es familiar?
	;
: is_unknown?  ( a -- ff ) \ ¿El protagonista aún no conoce el ente?
	is_known? 0=
	;
: is_here?  ( a -- ff )  \ ¿Está un ente en la misma localización que el protagonista?
	\ El resultado depende de cualquiera de tres condiciones:
	dup location am_i_there?  \ ¿Está efectivamente en la misma localización?
	over is_global_outdoor? am_i_outdoor? and or \ ¿O es un «global exterior» y estamos en un escenario exterior?
	swap is_global_indoor? am_i_indoor? and or  \ ¿O es un «global interior» y estamos en un escenario interior?
	;
: is_not_here?  ( a -- ff )  \ ¿Está un ente en otra localización que la del protagonista?
	\ No se usa!!!
	is_here? 0=
	;
: is_here_and_unknown?  ( a -- ff )  \ ¿¿Está un ente en la misma localización que el protagonista y aún no es conocido por él?
	dup is_here? swap is_unknown? and
	;
: is_here  ( a -- )  \ Hace que un ente esté en la misma localización que el protagonista
	my_location swap is_there
	;
: is_accessible?  ( a -- ff )  \ ¿Es un ente accesible para el protagonista?
	dup is_hold?  swap is_here?  or
	;
: is_not_accessible?  ( a -- ff )  \ ¿Un ente no es accesible para el protagonista?
	is_accessible? 0=
	;
: can_be_looked_at?  ( a -- ff )  \ ¿El ente puede ser mirado?
	dup my_location =  \ ¿Es la localización del protagonista?
	over is_direction? or  \ ¿O es un ente dirección?
	swap is_accessible? or  \ ¿O está accesible? 
	;
: can_be_taken?  ( a -- ff )  \ ¿El ente puede ser tomado?
	\ Se usa como norma general, para aquellos entes que no tienen un error específico indicado en el campo ~TAKE_ERROR#
	dup is_decoration?
	over is_human? or
	swap is_character? or 0=
	;

\ ------------------------------------------------
\ Herramientas de artículos y pronombres

0  [IF]  \ ......................................

La selección del artículo adecuado para el nombre de un ente
tiene su complicación. Depende por supuesto del número y
género gramatical del nombre, pero también de la relación
con el protagonista (distinción entre artículos definidos e
indefinidos) y de la naturaleza del ente (cosa o personaje).

Por conveniencia, consideramos como artículos ciertas
palabras que son adjetivos (como «esta», «ninguna»...), pues
en la práctica para el programa su manejo es idéntico: se
usan para preceder a los nombres bajo ciertas condiciones.

En este mismo apartado definimos palabras para calcular
los pronombres de objeto indirecto (le/s) y de objeto
directo (la/s, lo/s), así como terminaciones habituales.

[THEN]  \ ......................................

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

: article_number>  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el número gramatical del ente
	has_plural_name? /article_number_set and
	;
: article_gender>  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el género gramatical del ente
	has_feminine_name? /article_gender_set and
	;
: article_gender+number>  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el género gramatical y el número del ente
	dup article_gender> 
	swap article_number> +
	;
: definite_article>  ( a -- 0 | 1 )  \ Devuelve el desplazamiento (en número de grupos) para apuntar a los artículos definidos de la tabla, si el ente indicado necesita uno
	dup has_definite_article?  \ Si el ente necesita siempre artículo definido
	swap is_known? or abs  \ O bien si el ente es ya conocido por el protagonista
	;
: possesive_article>  ( a -- 0 | 2 )  \ Devuelve el desplazamiento (en número de grupos) para apuntar a los artículos posesivos de la tabla, si el ente indicado necesita uno
	is_owned? 2 and
	;
: negative_articles>  ( -- u )  \ Devuelve el desplazamiento (en número de caracteres) para apuntar a los «artículos negativos» de la tabla
	/article_type_set 3 *
	;
: undefined_articles>  ( -- u )  \ Devuelve el desplazamiento (en número de caracteres) para apuntar a los artículos indefinidos de la tabla
	0
	;
: definite_articles>  ( -- u )  \ Devuelve el desplazamiento (en número de caracteres) para apuntar a los artículos definidos de la tabla
	/article_type_set 
	;
: distant_articles>  ( -- u )  \ Devuelve el desplazamiento (en número de caracteres) para apuntar a los «artículos distantes» de la tabla
	/article_type_set 4 *
	;
: not_distant_articles>  ( -- u )  \ Devuelve el desplazamiento (en número de caracteres) para apuntar a los «artículos cercanos» de la tabla
	/article_type_set 5 *
	;
: article_type  ( a -- u )  \ Devuelve un desplazamiento parcial en la tabla de artículos según el ente requiera un artículo definido, indefinido o posesivo
	dup definite_article>  swap possesive_article>  max
	/article_type_set *
	;
: >article  ( u -- a1 u1 )  \ Devuelve un artículo de la tabla de artículos a partir de su índice
	'articles + /article -trailing
	;
: (article)  ( a -- a1 u1 )  \ Devuelve el artículo apropiado para un ente
	dup article_gender>  \ Desplazamiento según el género
	over article_number> +  \ Sumado al desplazamiento según el número
	swap article_type +  \ Sumado al desplazamiento según el tipo
	>article
	;
: article  ( a -- a1 u1 | a 0 )  \ Devuelve el artículo apropiado para un ente, si lo necesita; en caso contrario devuelve una cadena vacía 
	dup has_no_article?  if  0  else  (article)  then
	;
: undefined_article  ( a -- a1 u1 )  \ Devuelve el artículo indefinido correspondiente al género y número de un ente
	article_gender+number> undefined_articles> +
	>article
	;
: definite_article  ( a -- a1 u1 )  \ Devuelve el artículo definido correspondiente al género y número de un ente
	article_gender+number> definite_articles> +
	>article
	;
: pronoun  ( a -- a1 u1 )  \ Devuelve el pronombre correspondiente al género y número de un ente
	definite_article  s" lo" s" el" sreplace
	;
: negative_article  ( a -- a1 u1 )  \ Devuelve el «artículo negativo» correspondiente al género y número de un ente
	article_gender+number> negative_articles> +
	>article  s" ú" s" X" sreplace
	;
: distant_article  ( a -- a1 u1 )  \ Devuelve el «artículo distante» correspondiente al género y número de un ente
	article_gender+number> distant_articles> +
	>article
	;
: not_distant_article  ( a -- a1 u1 )  \ Devuelve el «artículo cercano» correspondiente al género y número de un ente
	article_gender+number> not_distant_articles> +
	>article
	;
: plural_ending  ( a -- a u )  \ Devuelve la terminación adecuada del plural para el nombre de un ente
	[false]  [IF]
		\ Método 1, «estilo BASIC»:
		has_plural_name?  if  s" s"  else  0$  then
	[ELSE]
		\ Método 2, sin estructuras condicionales, «estilo Forth»:
		s" s" rot has_plural_name? and
	[THEN]
	;
: gender_ending  ( a -- a u )  \ Devuelve la terminación adecuada del género gramatical para el nombre de un ente
	[lina?]  [IF]
		\ Método 1, «estilo BASIC»:
		has_feminine_name?  if  s" a"  else  s" o"  then
	[ELSE]
		\ Método 2, sin estructuras condicionales, «estilo Forth»:
\		s" oa" drop swap has_feminine_name? abs + 1
		\ Método 3, más directo:
		c" oa" swap has_feminine_name? abs + 1+ 1
	[THEN]
	;
: noun_ending  ( a -- a1 u1 )  \ Devuelve la terminación adecuada para el nombre de un ente
	dup gender_ending rot plural_ending s+
	;
: direct_pronoun  ( a -- a1 u1 )  \ Devuelve el pronombre de objeto directo para un ente («la/s» o «lo/s»)
	s" l" rot noun_ending s+
	;
: indirect_pronoun  ( a -- a1 u1 )  \ Devuelve el pronombre de objeto indirecto para un ente («le/s»)
	s" le" rot plural_ending s+
	;
: noun_ending+  ( a1 u1 a -- a2 u2 )  \ Añade a una cadena la terminación adecuada para el nombre de un ente
	noun_ending s+
	;

\ ------------------------------------------------
\ Interfaz para los nombres de los entes

0  [IF]  \ ......................................

Como ya se explicó, el nombre de cada ente se guarda en una
cadena dinámica (que se crea en la memoria con ALLOCATE , no
en el espacio del diccionario del sistema).  El manejo de
estas cadenas dinámicas se hace con el módulo
correspondiente de Forth Foundation Library.

En la ficha del ente se guarda solo la dirección de la
cadena dinámica (en el campo ~NAME_STR ).  Por ello hacen
falta palabras que hagan de interfaz para gestionar los
nombres de ente de forma análoga a como se hace con el resto
de datos de su ficha.

[THEN]  \ ......................................

: name!  ( a u a1 -- )  \ Guarda el nombre de un ente
	\ a u = Nombre
	\ a1 = Ente
	~name_str @ str-set
	;
: names!  ( a u a1 -- )  \ Guarda el nombre de un ente, y lo marca como plural
	\ a u = Nombre
	\ a1 = Ente
	dup has_plural_name  name!
	;
: fname!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino
	\ a u = Nombre
	\ a1 = Ente
	dup has_feminine_name  name!
	;
: fnames!  ( a u a1 -- )  \ Guarda el nombre de un ente, indicando también que es de género gramatical femenino y plural
	\ a u = Nombre
	\ a1 = Ente
	dup has_plural_name  fname!
	;
: name  ( a -- a1 u1 )  \ Devuelve el nombre de un ente
	\ a = Ente
	\ a1 u1 = Nombre
	~name_str @ str-get
	;
: ^name  ( a -- a1 u1 )  \ Devuelve el nombre de un ente, con la primera letra mayúscula
	name ^uppercase
	;
: name&  ( a a1 u1 -- a2 u2 )  \ Añade a un (supuesto) artículo el nombre de un ente
	\ a = Ente
	\ a1 u1 = Artículo correspondiente (o cualquier otro texto)
	\ a2 u2 = Nombre completo
	rot name s& 
	;
: full_name  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con el artículo que le corresponda
	dup article name& 
	;
: ^full_name  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con el artículo que le corresponda (con la primera letra en mayúscula)
	full_name ^uppercase
	;
: defined_full_name  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con un artículo definido
	dup definite_article name&
	;
: undefined_full_name  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con un artículo indefinido
	dup undefined_article name&
	;
: negative_full_name  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con un «artículo negativo»
	dup negative_article name&
	;
: distant_full_name  ( a -- a1 u1 )  \ Devuelve el nombre completo de un ente, con un «artículo distante»
	dup distant_article name&
	;
: nonhuman_subjective_negative_name  ( a -- a1 u1 )  \ Devuelve el nombre subjetivo (negativo) de un ente (no humano), desde el punto de vista del protagonista
	\ Nota: En este caso hay que usar NEGATIVE_FULL_NAME antes de S{ y pasar la cadena
	\ mediante la pila de retorno; de otro modo S{ y }S no pueden calcular bien
	\ el crecimiento de la pila.
	negative_full_name 2>r
	s{
	2r> 2dup 2dup  \ Tres nombres repetidos con «artículo negativo»
	s" eso" s" esa cosa" s" tal cosa"  \ Tres alternativas
	}s
	;
: human_subjective_negative_name  ( a -- a1 u1 )  \ Devuelve el nombre subjetivo (negativo) de un ente (humano), desde el punto de vista del protagonista
	dup is_known?
	if  full_name  else  drop s" nadie"  then
	;
: subjective_negative_name  ( a -- a1 u1 )  \ Devuelve el nombre subjetivo (negativo) de un ente, desde el punto de vista del protagonista
	dup is_human?
	if  human_subjective_negative_name
	else  nonhuman_subjective_negative_name
	then
	;
: /l$  ( a -- a1 u1 | a1 0 )  \ Devuelve la terminación «l» del artículo determinado masculino para añadirla a la preposición «a», si un ente humano lo requiere para ser usado como objeto directo; o una cadena vacía
	\ No se usa!!!
	s" l" rot has_personal_name? 0= and
	;
: a/$  ( a -- a1 u1 | a1 0 )  \ Devuelve la preposición «a» si un ente lo requiere para ser usado como objeto directo; o una cadena vacía
	s" a" rot is_human? and
	; 
: a/l$  ( a -- a1 u1 )  \ Devuelve la preposición «a», con posible artículo determinado, si un ente lo requiere para ser usado como objeto directo
	\ No se usa!!!
	a/$ dup  if  /l$ s+  then
	;
: subjective_negative_name_as_direct_object  ( a -- a1 u1 )  \ Devuelve el nombre subjetivo (negativo) de un ente, desde el punto de vista del protagonista, para ser usado como objeto directo
	dup a/$ rot subjective_negative_name s&
	;
: .full_name  ( a -- )  \ Imprime el nombre completo de un ente
	\ No se usa!!!
	full_name paragraph
	;

\ }}}###########################################################
section( Algunas cadenas calculadas y operaciones con ellas)  \ {{{

0  [IF]  \ ......................................

Nota!!!: ¿Mover a otra sección?

[THEN]  \ ......................................

: «open»|«closed»  ( a -- a1 u1 )  \ Devuelve «abierto/a/s» a «cerrado/a/s» según corresponda a un ente
	dup is_open?  if  s" abiert"  else  s" cerrad"  then
	rot noun_ending s+
	;
: player_gender_ending$  ( -- a u )  \ Devuelve la terminación «a» u «o» según el sexo del jugador
	[lina?]  [IF]
		\ Método 1, «estilo BASIC»:
		woman_player? @  if  s" a"  else  s" o"  then
	[ELSE]
		\ Método 2, sin estructuras condicionales, «estilo Forth»:
		c" oa" woman_player? @ abs + 1+ 1
	[THEN]
	;
: player_gender_ending$+  ( a1 u1 -- a2 u2 )  \ Añade a una cadena la terminación «a» u «o» según el sexo del jugador
	player_gender_ending$ s+
	;

\ }}}###########################################################
section( Operaciones elementales con entes)  \ {{{

0  [IF]  \ ......................................

Algunas operaciones sencillas relacionadas con la trama.

Alguna es necesario crearla como vector porque se usa en las
descripciones de los entes o en las acciones, antes de
definir la trama.

[THEN]  \ ......................................

defer lock_found  \ Encontrar el candado; la definición está en (LOCK_FOUND)

0 constant limbo \ Marcador para usar como localización de entes inexistentes
: vanished?  ( a -- ff )  \ ¿Está un ente desaparecido?
	location limbo =
	;
: not_vanished?  ( a -- ff )  \ ¿No está un ente desaparecido?
	vanished? 0=
	;
: vanish  ( a -- )  \ Hace desaparecer un ente llevándolo al «limbo»
	limbo swap is_there
	;
: vanish_if_hold  ( a -- )  \ Hace desaparecer un ente si su localización es el protagonista
	\ No se usa!!!
	dup is_hold?  if  vanish  else  drop  then
	;

\ }}}###########################################################
section( Herramientas para crear las fichas de la base de datos)  \ {{{

0  [IF]  \ ......................................

No es posible reservar el espacio necesario para las fichas
hasta saber cuántas necesitaremos (a menos que usáramos una
estructura un poco más sofisticada con fichas separadas pero
enlazadas entre sí, muy habitual también y fácil de crear).
Por ello la palabra 'ENTITIES (que devuelve la dirección de
la base de datos) se crea como un vector, para asignarle
posteriormente su dirección de ejecución.  Esto permite
crear un nuevo ente fácilmente, sin necesidad de asignar
previamente el número de fichas a una constante.

[THEN]  \ ......................................

defer 'entities  \ Dirección de los entes; vector que después será redirigido a la palabra real
0 value #entities  \ Contador de entes, que se actualizará según se vayan creando

: #>entity  ( u -- a )  \ Devuelve la dirección de la ficha de un ente a partir de su número ordinal (el número del primer ente es el cero)
	/entity * 'entities +
	;
: entity>#  ( a -- u )  \ Devuelve el número ordinal de un ente (el primero es el cero) a partir de la dirección de su ficha 
	'entities - /entity /
	;
: entity:  ( "name" -- ) \ Crea un nuevo identificador de ente, que devolverá la dirección de su ficha
	create
		#entities ,  \ Guardar la cuenta en el cuerpo de la palabra recién creada
		#entities 1+ to #entities  \ Actualizar el contador
	does>  ( pfa -- a )  \ Cuando la constante sea llamada tendrá su pfa en la pila
		@ #>entity  \ Cuando el identificador se ejecute, devolverá la dirección de su ficha
	;
: erase_entity  ( a -- )  \ Rellena con ceros la ficha de un ente
	/entity erase
	;
: backup_entity  ( a -- x1 x2 x3 x4 )  \ Respalda los datos de un ente que se crearon durante la compilación del código y deben preservarse
	>r
	r@ ~name_str @
	r@ ~init_xt @
	r@ ~location_plot_xt @
	r> ~description_xt @ 
	;
: restore_entity  ( x1 x2 x3 x4 a -- )  \ Restaura los datos de un ente que se crearon durante la compilación del código y deben preservarse
	>r
	r@ ~description_xt !
	r@ ~location_plot_xt !
	r@ ~init_xt !
	r> ~name_str !
	;
: setup_entity  ( a -- )  \ Prepara la ficha de un ente para ser completada con sus datos 
	>r r@ backup_entity  r@ erase_entity  r> restore_entity
	;
0 value self%  \ Ente cuyos atributos, descripción o trama están siendo definidos (usado para aligerar la sintaxis)
: :name_str  ( a -- )  \ Crea una cadena dinámica nueva para guardar el nombre del ente
	[debug_init]  [IF]  s" Inicio de :NAME_STR" debug [THEN]
	dup ~name_str @ ?dup
	[debug_init]  [IF]  s" A punto para STR-FREE" debug [THEN]
	if  str-free  then
	str-new swap ~name_str !
	[debug_init]  [IF]  s" Final de :NAME_STR" debug [THEN]
	;
: [:attributes]  ( a -- )  \ Inicia la definición de propiedades de un ente
	\ Esta palabra se ejecuta cada vez que hay que restaurar los datos del ente,
	\ y antes de la definición de atributos contenida en la palabra correspondiente al ente.
	\ El identificador del ente está en la pila porque se compiló con LITERAL cuando se creó la palabra de atributos.
	dup to self%  \ Actualizar el puntero al ente, usado para aligerar la sintaxis
	dup :name_str  \ Crear una cadena dinámica para el campo ~NAME_STR
	setup_entity
	;
: default_description  ( -- )  \ Descripción predeterminada de los entes para los que no se ha creado una palabra propia de descripción
	^is_normal$ paragraph
	;
: (:attributes)  ( a xt -- )  \ Operaciones preliminares para la definición de atributos de un ente
	\ Esta palabra solo se ejecuta una vez para cada ente,
	\ al inicio de la compilación del código de la palabra
	\ que define sus atributos.
	\ a = Ente para la definición de cuyos atributos se ha creado una palabra
	\ xt = Dirección de ejecución de la palabra recién creada
	over ~init_xt !  \ Conservar la dirección de ejecución en la ficha del ente
	['] default_description over ~description_xt !  \ Poner la descripción predeterminada
	postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que [:DESCRIPTION] lo guarde en SELF% en tiempo de ejecución
	;
: :attributes  ( a -- )  \ Inicia la creación de una palabra sin nombre que definirá las propiedades de un ente
	:noname (:attributes)  \ Crear la palabra y hacer las operaciones preliminares
	postpone [:attributes]  \ Compilar la palabra [:ATTRIBUTES] en la palabra creada, para que se ejecute cuando sea llamada
	\ lina necesita guardar una copia del puntero de la pila tras crear una palabra
	\ ( lo mismo ocurre después con las definiciones de :DESCRIPTION , :LOCATION_PLOT y :ACTION ):
	[lina?]  [IF]  !CSP  [THEN]
	;
gforth?  [IF]
	\ Primera versión de ;attributes
	\ (cuando se ejecuta provoca error «dirección de memoria incorrecta»):
		\ comp' ; alias ;attributes immediate drop
	\ Segunda versión de ;attributes
	\ (cuando se ejecuta provoca error «desestructurado»):
		\ : ;attributes  postpone ;  ;  immediate
	\ Tercera versión de ;attributes
	\ (cuando se ejecuta no vuelve al modo de interpretación):
		\ : ;attributes  [comp'] ; postpone,  ;  immediate
	\ Cuarta versión de ;attributes
	: ;attributes  [comp'] ; postpone, postpone [  ;  immediate
[ELSE]
	' ; alias ;attributes
	\ Al contrario de lo que es habitual en Forth,
	\ lina copia en el alias el bitio de inmediatez de la
	\ palabra original. Y además cada ejecución de IMMEDIATE
	\ no pone el bitio a 1 como es la norma, sino que lo cambia de estado...
	\ Por ello no hay que usar IMMEDIATE en lina en este caso
	\ y en otros dos análogos que tendrán lugar más adelante:
	\ ;LOCATION_PLOT y ;ACTION , así como en los alias
	\ de \ y ( creados en las vocabularios CONFIG_VOCABULARY
	\ y RESTORE_VOCABULARY .
	lina? 0=  [IF]  immediate  [THEN]
[THEN]
: init_entity  ( a -- )  \ Restaura la ficha de un ente a su estado original
	[debug_init]  [IF]  s" Inicio de INIT_ENTITY" debug dup entity># cr ." Entity=" .  [THEN]
	~init_xt @ 
	[debug_init]  [IF]  s" Antes de EXECUTE" debug [THEN]
	execute 
	[debug_init]  [IF]  s" Final de INIT_ENTITY" debug  [THEN]
	;
: init_entities  ( -- )  \ Restaura las fichas de los entes a su estado original
	#entities 0  do
		[debug_init]  [IF]  i cr ." about to init entity #" .  [THEN]
		i #>entity init_entity
	loop
	;

\ }}}###########################################################
section( Herramientas para crear las descripciones)  \ {{{

0  [IF]  \ ......................................

No almacenamos las descripciones en la base de datos junto
con el resto de atributos de los entes, sino que para cada
ente creamos una palabra que imprime su descripción, lo que
es mucho más flexible: La descripción podrá variar en
función del desarrollo del juego y adaptarse a las
circunstancias, e incluso sustituir en algunos casos al
código que controla la trama del juego.

Así pues, lo que almacenamos en la ficha del ente, en el
campo ~DESCRIPTION_XT , es la dirección de ejecución de la
palabra que imprime su descripción.

Por tanto, para describir un ente basta tomar de su ficha el
contenido de ~DESCRIPTION_XT , y llamar a EXECUTE (véase más
abajo la definición de la palabra (DESCRIBE) , que es la que
hace la tarea).

[THEN]  \ ......................................

false value sight  \ Guarda el ente dirección al que se mira en un escenario (o el propio ente escenario); se usa en las palabras de descripción de escenarios
: [:description]  ( a -- )  \ Operacionas previas a la ejecución de la descripción de un ente
	\ Esta palabra se ejecutará al comienzo de la palabra de descripción.
	\ El identificador del ente está en la pila porque se compiló con LITERAL cuando se creó la palabra de descripción.
	to self%  \ Actualizar el puntero al ente, usado para aligerar la sintaxis
	;
: (:description)  ( a xt -- )  \ Operaciones preliminares para la definición de la descripción de un ente
	\ Esta palabra solo se ejecuta una vez para cada ente,
	\ al inicio de la compilación del código de la palabra
	\ que crea su descripción. 
	\ a = Ente para cuya descripción se ha creado una palabra
	\ xt = Dirección de ejecución de la palabra recién creada
	\ s" en (:DESCRIPTION)" debug  \ Depuración!!!
	over ~description_xt !  \ Conservar la dirección de ejecución en la ficha del ente
	\ s" en (:DESCRIPTION)" debug  \ Depuración!!!
	postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que [:DESCRIPTION] lo guarde en SELF% en tiempo de ejecución
	;
: :description  ( a -- )  \ Inicia la definición de una palabra de descripción para un ente
	:noname (:description)  \ Crear la palabra y hacer las operaciones preliminares
	postpone [:description]  \ Compilar la palabra [:DESCRIPTION] en la palabra creada, para que se ejecute cuando sea llamada
	[lina?]  [IF]  !CSP  [THEN]
	;
: [;description]  ( -- )  \ Operaciones finales tras la ejecución de la descripción de un ente
	\ Esta palabra se ejecutará al final de la palabra de descripción.
	false to sight  \ Poner a cero el selector de vista, para evitar posibles errores
	;
gforth?  [IF]
: ;description  ( -- )  \ Termina la definición de una palabra de descripción de un ente
	postpone [;description]  \ Compilar la palabra [;DESCRIPTION] en la palabra creada, para que se ejecute cuando sea llamada
	[comp'] ; postpone, postpone [
	;  immediate
[ELSE]
: ;description  ( -- )  \ Termina la definición de una palabra de descripción de un ente
	postpone [;description]  \ Compilar la palabra [;DESCRIPTION] en la palabra creada, para que se ejecute cuando sea llamada
	postpone ;
	; immediate
[THEN]
: (describe)  ( a -- )  \ Ejecuta la palabra de descripción de un ente
	~description_xt perform
	;
: .location_name  ( a -- )  \ Imprime el nombre de un ente escenario, como cabecera de su descripción
	name ^uppercase location_name_color paragraph system_color
	;
: (describe_location)  ( a -- )  \ Describe un ente escenario
	dup to sight
	location_description_color (describe)
	;
: describe_location  ( a -- )  \ Describe un ente escenario, con borrado de pantalla y título
	[debug]  [IF]  s" En DESCRIBE_LOCATION" debug  [THEN]  \ Depuración!!!
	clear_screen_for_location
	dup .location_name  (describe_location)
	[debug]  [IF]  cr s" Location visited:" my_location ~visits @ .  [THEN]  \ Depuración!!!
	;
: describe_other  ( a -- )  \ Describe un ente de otro tipo
	description_color (describe)
	;
: describe_direction  ( a -- )  \ Describe un ente dirección
	to sight  \ Poner el ente dirección en SIGHT
	my_location describe_other  \ Y describir el escenario actual como un ente normal; ahí se hace la distinción
	;
: description_type  ( a -- u )  \ Convierte un ente en el tipo de descripción que requiere
	\ a = Ente
	\ u = Tipo de descripción (2:dirección, 1:escenario, 0:otros, 3:¡error!)
	\ Nota: Un resultado de 3 significaría que el ente es a la vez dirección y escenario
	dup is_location? abs
	swap is_direction? 2 and +
	;
: describe  ( a -- )  \ Describe un ente, según su tipo
	[debug]  [IF]  s" En DESCRIBE" debug  [THEN]  \ Depuración!!!
	dup description_type
	[debug]  [IF]  s" En DESCRIBE antes de CASE" debug  [THEN]  \ Depuración!!!
	case
		0 of  describe_other  endof
		1 of  describe_location  endof
		2 of  describe_direction  endof
		3 of  true abort" Error fatal en DESCRIBE: dato incorrecto"  endof \ depuración!!!
	endcase
	;
: uninteresting_direction  ( -- )  \ Muestra la descripción de la direcciones que no tienen nada especial
	uninteresting_direction$ paragraph
	;

\ }}}###########################################################
section( Identificadores de entes)  \ {{{

0  [IF]  \ ......................................

Cada ente es identificado mediante una palabra. Los
identificadores de entes se crean con la palabra ENTITY: .
Cuando se ejecutan devuelven la dirección en memoria de la
ficha del ente en la base de datos, que después puede ser
modificada con un identificador de campo para convertirla en
la dirección de memoria de una campo concreto de la ficha.

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

[THEN]  \ ......................................

entity: ulfius%
' ulfius% is protagonist%  \ Actualizar el vector que apunta al ente protagonista

\ Entes que son (seudo)personajes: 
entity: ambrosio%
entity: leader%
entity: soldiers%
entity: refugees%

\ Entes que son objetos:
entity: altar%
entity: arch%
entity: bed%
entity: bridge%
entity: candles%
entity: cloak%
entity: cuirasse%
entity: door%
entity: emerald%
entity: fallen_away%
entity: flags%
entity: flint%
entity: idol%
entity: key%
entity: lake%
entity: lock%
entity: log%
entity: piece%
entity: rags%
entity: rocks%
entity: snake%
entity: stone%
entity: sword%
entity: table%
entity: thread%
entity: torch%
entity: waterfall%

\ Entes escenario (en orden de número)
entity: location_01%
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
entity: exits%
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
\ (pues la palabra ENTITY: actualiza el contador #ENTITIES )
\ y por tanto podemos reservar espacio para la base de datos:

#entities /entity * constant /entities  \ Espacio necesario para guardar todas las fichas, en octetos
create ('entities) /entities allot  \ Reservar el espacio en el diccionario
' ('entities) is 'entities  \ Actualizar el vector que apunta a dicho espacio
'entities /entities erase  \ Llenar la zona con ceros, para mayor seguridad

\ }}}###########################################################
section( Herramientas para crear conexiones entre escenarios)  \ {{{

\ Nota!!!: Este código quedaría mejor con el resto
\ de herramientas de la base de datos, para no separar
\ la lista de entes de sus datos.
\ Pero se necesita usar los identificadores
\ de los entes dirección.
\ Se podría solucionar con vectores, más adelante.

0  [IF]  \ ......................................

Para crear el mapa hay que hacer dos operaciones con los
entes escenario: marcarlos como tales, para poder
distinguirlos como escenarios; e indicar a qué otros entes
escenarios conducen sus salidas.

La primera operación se hace guardando un valor buleano
«cierto» en el campo ~IS_LOCATION? del ente.  Por ejemplo:

	cave% ~is_location? on

O bien mediante la palabra creada para ello en la interfaz
básica de campos:

	cave% is_location

La segunda operación se hace guardando en los campos de
salida del ente los identificadores de los entes a que cada
salida conduzca.  No hace falta ocuparse de las salidas
impracticables porque ya estarán a cero de forma
predeterminada.  Por ejemplo:	

	path% cave% ~south_exit !  \ Hacer que la salida sur de CAVE% conduzca a PATH%
	cave% path% ~north_exit !  \ Hacer que la salida norte de PATH% conduzca a CAVE%

No obstante, para hacer más fácil este segundo paso, hemos
creado unas palabras que proporcionan una sintaxis específica,
como mostraremos a continuación.

[THEN]  \ ......................................

0  [IF]  \ Inacabado!!!

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

[THEN]

0  [IF]  \ ......................................

Necesitamos una tabla que nos permita traducir esto:

ENTRADA: Un puntero correspondiente a un campo de dirección
de salida en la ficha de un ente.

SALIDA: El identificador del ente dirección al que se
refiere esa salida.

[THEN]  \ ......................................

create exits_table  \ Tabla de traducción de salidas
#exits cells allot  \ Reservar espacio para tantas celdas como salidas
: >exits_table>  ( u -- a )  \ Apunta a la dirección de un elemento de la tabla de direcciones
	\ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
	\ a = Dirección del ente dirección correspondiente en la tabla
	first_exit> - exits_table +
	;
: exits_table!  ( a u -- )  \ Guarda un ente en una posición de la tabla de salidas
	\ a = Ente dirección
	\ u = Campo de dirección (por tanto, desplazamiento relativo al inicio de la ficha de un ente)
	>exits_table> !
	;
: exits_table@  ( u -- a )  \ Devuelve un ente dirección a partir de un campo de dirección
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

0  [IF]  \ Inacabado!!!
: opposite_exit  ( a1 -- a2 )  \ Devuelve la dirección cardinal opuesta a la indicada
	first_exit> - opposite_exits + @
	;
: opposite_exit%  ( a1 -- a2 )  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada
	\ a1 = entidad de dirección
	\ a2 = entidad de dirección, opuesta a a1
	first_exit> - opposite_direction_entities + @
	;
[THEN]

0  [IF]  \ ......................................

A continuación definimos palabras para proporcionar la
siguiente sintaxis (primero origen y después destino en la
pila, como es convención en Forth):

	cave% path% s-->  \ Hacer que la salida sur de CAVE% conduzca a PATH% (pero sin afectar al sentido contrario)
	path% cave% n-->  \ Hacer que la salida norte de PATH% conduzca a CAVE% (pero sin afectar al sentido contrario)

O en un solo paso:

	cave% path% s<-->  \ Hacer que la salida sur de CAVE% conduzca a PATH% (y al contrario: la salida norte de PATH% conducirá a CAVE_E)


[THEN]  \ ......................................

: -->  ( a1 a2 u -- )  \ Comunica el ente a1 con el ente a2 mediante la salida indicada por el desplazamiento u
	\ a1 = Ente origen de la conexión
	\ a2 = Ente destino de la conexión
	\ u = Desplazamiento del campo de dirección a usar en a1
	rot + !
	;

\ Conexiones unidireccionales

: n-->  ( a1 a2 -- )  \ Comunica la salida norte del ente a1 con el ente a2
	north_exit> -->
	;
: s-->  ( a1 a2 -- )  \ Comunica la salida sur del ente a1 con el ente a2
	south_exit> -->
	;
: e-->  ( a1 a2 -- )  \ Comunica la salida este del ente a1 con el ente a2
	east_exit> -->
	;
: w-->  ( a1 a2 -- )  \ Comunica la salida oeste del ente a1 con el ente a2
	west_exit> -->
	;
: u-->  ( a1 a2 -- )  \ Comunica la salida hacia arriba del ente a1 con el ente a2
	up_exit> -->
	;
: d-->  ( a1 a2 -- )  \ Comunica la salida hacia abajo del ente a1 con el ente a2
	down_exit> -->
	;
: o-->  ( a1 a2 -- )  \ Comunica la salida hacia fuera del ente a1 con el ente a2
	out_exit> -->
	;
: i-->  ( a1 a2 -- )  \ Comunica la salida hacia dentro del ente a1 con el ente a2
	in_exit> -->
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

0  [IF]  \ ......................................

Por último, definimos dos palabras para hacer
todas las asignaciones de salidas en un solo paso. 

[THEN]  \ ......................................

\ Múltiples conexiones a la vez

: exits!  ( a1..a8 a0 -- )  \ Asigna todas las salidas de un ente escenario
	\ a1..a8 = Entes escenario de salida (o cero) en el orden habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera
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

\ Una palabra final para hacer todas las operaciones en un solo paso

: init_location  ( a1..a8 a0 -- )  \ Marca un ente como escenario y le asigna todas las salidas. 
	\ a1..a8 = Entes escenario de salida (o cero) en el orden habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera
	\ a0 = Ente escenario cuyas salidas hay que modificar
	dup is_location exits!
	;

\ }}}###########################################################
section( Recursos para las descripciones de entes)  \ {{{

0  [IF]  \ ......................................

Las palabras de esta sección se usan para 
construir las descripciones de los entes.
Cuando su uso se vuelve más genérico, se mueven
a la sección de textos calculados.

[THEN]  \ ......................................

\ ------------------------------------------------
\ Albergue de los refugiados

: they_don't_let_you_pass$  ( -- a u )  \ Devuelve el mensaje de que los refugiados no te dejan pasar
	s{
	s" te" s? (they)_block$ s&
	s" te rodean,"
	s{ s" impidiéndote" s" impidiendo"
	s" obstruyendo" s" obstruyéndote"
	s" bloqueando" s" bloqueándote" }s&
	}s the_pass$ s&
	;
: the_pass_free$  ( -- a u )  \ Devuelve una variante de «libre el paso»
	s" libre" the_pass$ s&
	;
: they_let_you_pass_0$  ( -- a u )  \ Devuelve la primera versión del mensaje de que te dejan pasar
	s{
	s" te" s? s" han dejado" s&
	s" se han" s{ s" apartado" s" echado a un lado" }s& s" para dejar" s& s" te" s?+
	}s the_pass_free$ s&
	;
: they_let_you_pass_1$  ( -- a u )  \ Devuelve la segunda versión del mensaje de que te dejan pasar
	s" se han" s{ s" apartado" s" retirado" }s&
	s" a" s{ s{ s" los" s" ambos" }s s" lados" s& s" uno y otro lado" }s& s?& comma+
	s{ s" dejándote" s" dejando" s" para dejar" s" te" s?+ }s&
	the_pass_free$ s&
	;
: they_let_you_pass_2$  ( -- a u )  \ Devuelve la tercera versión del mensaje de que te dejan pasar
	s" ya no" they_don't_let_you_pass$ s& s" como antes" s?&
	;
: they_let_you_pass$  ( -- a u )  \ Devuelve el mensaje de que te dejan pasar
	['] they_let_you_pass_0$
	['] they_let_you_pass_1$
	['] they_let_you_pass_2$
	3 choose execute
	;

\ ------------------------------------------------
\ Tramos de cueva (laberinto)

\ Elementos básicos usados en las descripciones

: this_narrow_cave_pass$  ( -- a u )  \ Devuelve una variante de «estrecho tramo de cueva», con el artículo adecuado
	\ Pendiente!!! Para que un escenario sea conocido, 
	\ hay que incrementar sus visitas al salir de él,
	\ y tenerlas en cuenta en IS_KNOWN?
	my_location dup is_known?
	if  not_distant_article
	else  undefined_article
	then  narrow_cave_pass$ s&
	;
: ^this_narrow_cave_pass$  ( -- a u )  \ Devuelve una variante de «estrecho tramo de cueva», con el artículo adecuado y la primera letra mayúscula
	this_narrow_cave_pass$ ^uppercase
	;
: toward_the(m/f)  ( a -- a1 u1 )  \ Devuelve una variante de «hacia el» con el artículo adecuado a un ente
	has_feminine_name?  if  toward_the(f)$  else  toward_the(m)$  then
	;
: toward_(the)_name  ( a -- a1 u1 )  \ Devuelve una variante de «hacia el nombre-de-ente» adecuada a un ente
	dup has_no_article?
	if  s" hacia"
	else  dup toward_the(m/f)
	then  rot name s&
	;
: main_cave_exits_are$  ( -- a u ) \ Devuelve una variante del inicio de la descripción de los tramos de cueva
	^this_narrow_cave_pass$ lets_you$ s& to_keep_going$ s&
	;

\ Variantes para la descripción de cada salida

: cave_exit_description_0$  ( -- a u )  \ Devuelve la primera variante de la descripción de una salida de un tramo de cueva
	^this_narrow_cave_pass$  lets_you$ s& to_keep_going$ s&
	in_that_direction$ s&
	;
: cave_exit_description_1$ ( -- a1 u1 )  \ Devuelve la segunda variante de la descripción de una salida de un tramo de cueva
	^a_pass_way$
	s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
	in_that_direction$ s&
	;

\ Variantes para la descripción principal

false  [IF]  \ Código obsoleto!!!

: $two_main_exits_in_cave ( a1 u1 a2 u2 -- a3 u3 )  \ Devuelve la descripción de un tramo de cueva con dos salidas a dos puntos cardinales
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
: $other_exit_in_cave  ( a1 u1 -- a2 u2 )  \ Devuelve la descripción de una salida adicional en un tramo de cueva
	\ No se usa!!!
	\ Se usa en la descripción principal de un escenario
	\ Esta palabra solo sirve para parámetros de puntos cardinales (todos usan artículo determinado masculino)
	\ a1 u1 = Nombre de la dirección cardinal
	^a_pass_way$ s&
	s{ s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
	toward_the(m)$ s& 2swap s&
	;

[THEN]  \ Fin del código obsoleto!!!

: cave_exit_separator+  ( a1 u1 -- a2 u2 )  \ Concatena (sin separación) a una cadena el separador entre las salidas principales y las secundarias
	s{ s" ," s" ;" s" ..." }s+
	s{ s" y" 2dup s" aunque" s" pero" }s& s" también" s?&
	;
: (paths)_can_be_seen_0$  ( -- a u )
	s{ s" parten" s" surgen" s" nacen" s" salen" }s
	s" de" s{ s" aquí" s" este lugar" }s& s? r2swap s&
	;
: (paths)_can_be_seen_1$  ( -- a u )
	s{ s" se ven" s" pueden verse"
	s" se vislumbran" s" pueden vislumbrarse"
	s" se adivinan" s" pueden adivinarse"
	s" se intuyen" s" pueden intuirse" }s
	;
: (paths)_can_be_seen$  ( -- a u )
	\ Pendiente!!! Hacer que el texto dependa, por grupos, de si el escenario es conocido
	['] (paths)_can_be_seen_0$  
	['] (paths)_can_be_seen_1$  
	2 choose execute
	;
: paths_seen  ( a1 u1 -- a2 u2 )  \ Devuelve la presentación de la lista de salidas secundarias
	\ a1 u1 = Cadena con el número de pasajes
	\ a2 u2 = Cadena con el resultado
	pass_ways$ s& s" más" s?&
	(paths)_can_be_seen$ r2swap s&
	;

: secondary_exit_in_cave&  ( a1 a2 u2 -- a3 u3 )  \ Devuelve la descripción de una salida adicional en un tramo de cueva
	\ a1 = Ente dirección cuya descripción hay que añadir
	\ a2 u2 = Descripción en curso
	rot toward_(the)_name s&
	;
: one_secondary_exit_in_cave  ( a1 -- a2 u2 )  \ Devuelve la descripción de una salida adicional en un tramo de cueva
	\ a1 = Ente dirección
	a_pass_way$
	s{ s" parte" s" surge" s" se ve" s" nace" s" sale" s" puede verse" }s&
	secondary_exit_in_cave&  
	;
: two_secondary_exits_in_cave  ( a1 a2 -- a3 u3 )  \ Devuelve la descripción de dos salidas adicionales en un tramo de cueva
	s" dos" paths_seen s" :" s?+
	secondary_exit_in_cave& s" y" s&
	secondary_exit_in_cave& 
	;
: three_secondary_exits_in_cave  ( a1 a2 a3 -- a4 u4 )  \ Devuelve la descripción de tres salidas adicionales en un tramo de cueva
	s" tres" paths_seen s" :" s?+
	secondary_exit_in_cave& comma+
	secondary_exit_in_cave& s" y" s&
	secondary_exit_in_cave&
	;
: two_main_exits_in_cave ( a1 a2 -- a3 u3 )  \ Devuelve la descripción de con dos salidas principales en un tramo de cueva
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	toward_(the)_name rot toward_(the)_name both
	;
: one_main_exit_in_cave  ( a1 -- a2 u2 )  \ Devuelve la descripción de una salida principal en un tramo de cueva
	\ a1 = Ente dirección
	toward_(the)_name 
	;

\ Descripciones de los tramos de cueva según el reparto entre salidas principales y secundarias

: 1+1_cave_exits  ( a1 a2 -- a u )  \  Devuelve la descripción de un tramo de cueva con una salida principal y ninguna secundaria
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	one_main_exit_in_cave cave_exit_separator+
	rot one_secondary_exit_in_cave s&
	;
: 1+2_cave_exits  ( a1 a2 a3 -- a u )  \  Devuelve la descripción de un tramo de cueva con una salida principal y dos secundarias
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	\ a3 = Ente dirección
	one_main_exit_in_cave cave_exit_separator+
	2swap two_secondary_exits_in_cave s&
	;
: 1+3_cave_exits  ( a1 a2 a3 a4 -- a u )  \  Devuelve la descripción de un tramo de cueva con una salida principal y tres secundarias
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	\ a3 = Ente dirección
	\ a4 = Ente dirección
	one_main_exit_in_cave cave_exit_separator+
	2>r	three_secondary_exits_in_cave 2r> 2swap s&
	;
: 2+0_cave_exits  ( a1 a2 -- a u )  \  Devuelve la descripción de un tramo de cueva con dos salidas principales y ninguna secundaria
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	two_main_exits_in_cave
	;
: 2+1_cave_exits  ( a1 a2 a3 -- a u )  \  Devuelve la descripción de un tramo de cueva con dos salidas principales y ninguna secundaria
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	\ a3 = Ente dirección
	two_main_exits_in_cave cave_exit_separator+
	rot one_secondary_exit_in_cave s&
	;
: 2+2_cave_exits  ( a1 a2 a3 a4 -- a u )  \  Devuelve la descripción de un tramo de cueva con dos salidas principales y dos secundarias
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	\ a3 = Ente dirección
	\ a4 = Ente dirección
	two_main_exits_in_cave cave_exit_separator+
	2swap two_secondary_exits_in_cave s&
	;

\ Descripciones de los tramos de cueva según su número de salidas

: 1-exit_cave_description   ( a1 -- a u )  \ Devuelve la descripción principal de un tramo de cueva que tiene una salida
	\ a1 = Ente dirección
	toward_(the)_name 
	;
: 2-exit_cave_description   ( a1 a2 -- a u )  \ Devuelve la descripción principal de un tramo de cueva que tiene dos salidas
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	['] 2+0_cave_exits
	['] 1+1_cave_exits
	2 choose execute
	;
: 3-exit_cave_description   ( a1 a2 a3 -- a u )  \ Devuelve la descripción principal de un tramo de cueva que tiene tres salidas
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	\ a3 = Ente dirección
	['] 2+1_cave_exits
	['] 1+2_cave_exits
	2 choose execute
	;
: 4-exit_cave_description   ( a1 a2 a3 a4 -- a u )  \ Devuelve la descripción principal de un tramo de cueva que tiene cuatro salidas
	\ a1 = Ente dirección
	\ a2 = Ente dirección
	\ a3 = Ente dirección
	\ a4 = Ente dirección
	['] 2+2_cave_exits
	['] 1+3_cave_exits
	2 choose execute
	;
create 'cave_descriptions  \ Tabla para contener las direcciones de las palabras de descripción
' 1-exit_cave_description ,
' 2-exit_cave_description ,
' 3-exit_cave_description ,
' 4-exit_cave_description ,

\ Interfaz para usar en las descripciones de los escenarios:
\ EXITS_CAVE_DESCRIPTION para la descripción principal
\ CAVE_EXIT_DESCRIPTION$ para la descripción de cada salida

: unsort_cave_exits  ( a1..an u -- a1'..an' u )  \ Desordena los entes dirección que son las salidas de la cueva
	\ u = Número de elementos de la pila que hay que desordenar
	dup >r unsort r>
	;
: (exits_cave_description)  ( a1..an u -- a2 u2 )  \ Ejecuta (según el número de salidas) la palabra  que devuelve la descripción principal de un tramo de cueva
	\ a1..an = Entes de dirección correspondientes a las salidas
	\ u = Número de entes de dirección suministrados
	1- cells 'cave_descriptions + perform
	;
: exits_cave_description  ( a1..an u -- a2 u2 )  \ Devuelve la descripción principal de un tramo de cueva
	\ a1..an = Entes de dirección correspondientes a las salidas
	\ u = Número de entes de dirección suministrados
	unsort_cave_exits  (exits_cave_description) period+
	main_cave_exits_are$ 2swap s&  \ Añadir el encabezado
	;
: cave_exit_description$  ( -- a1 u1 )  \ Devuelve la descripción de una dirección de salida de un tramo de cueva
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
: rests_of_the_village$  ( -- a u )  \ Devuelve parte de otras descripciones de la aldea arrasada
	s" los restos" still$ s? s" humeantes" s& s?&
	s" de la" s& poor_village$ s&
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
	the_water_flow$ comes_from_there$ s&
	;
: ^water_from_there$  ( -- a u )
	^the_water_flow$ comes_from_there$ s&
	;
: water_that_way$  ( -- a u )
	^the_water_flow$ s{ s" corre" s" fluye" s" va" }s&
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

\ }}}###########################################################
section( Atributos y descripciones de entes)  \ {{{

\ Ente protagonista

ulfius% :attributes
[ subsection( a0) ]
	s" Ulfius" self% name!
	self% is_human
	self% has_personal_name
	self% has_no_article
	location_01% self% is_there
[ subsection( a0a) ]
	;attributes
subsection( a1)
ulfius% :description
	\ Provisional!!!
	s" [descripción de Ulfius]"
	paragraph	
	;description
subsection( d1)

\ Entes personaje

ambrosio% :attributes
	s" hombre" self% name!  \ El nombre cambiará a «Ambrosio» durante el juego
	self% is_character
	self% is_human
	location_19% self% is_there
	;attributes
ambrosio% :description
	self% is_known?  if
		s" Ambrosio"
		s" es un hombre de mediana edad, que te mira afable." s&
	else  s" Es de mediana edad y mirada afable."
	then  paragraph
	;description
leader% :attributes
	s" anciano" self% name!
	self% is_character
	self% is_human
	location_28% self% is_there
	;attributes
leader% :description
	s" Es el jefe de los refugiados."
	paragraph
	;description
soldiers% :attributes
	s" soldados" self% name!
	self% has_plural_name
	self% is_human
	self% familiar++
	self% is_decoration
	\ self% has_definite_article  \ Mejor implementar que tenga posesivo!!!...
	self% is_owned  \ ...aunque quizá esto baste!!!
	;attributes
defer soldiers_description  \ Vector a la futura descripción
soldiers% :description
	\ La descripción de los soldados
	\ necesita usar palabras que aún no están definidas,
	\ y por ello es mejor crearla después.
	soldiers_description
	;description
refugees% :attributes
	s" refugiados" self% name!
	self% has_plural_name
	self% is_human
	self% is_decoration
	;attributes
refugees% :description
	\ Pendiente!!! Descripciones provisionales
	my_location  case
	location_28%  of
		s" Los refugiados bla bla bla..." paragraph
		endof
	location_29%  of
		s" Todos los refugiados quedaron atrás." paragraph
		endof
	endcase
	;description

\ Entes objeto

altar% :attributes
	s" altar" self% name!
	self% is_decoration
	impossible_error# self% ~take_error# !
	location_18% self% is_there
	;attributes
altar% :description
	s" Está colocado justo en la mitad del puente."
	idol% is_known? 0=  if
		s" Debe de sostener algo importante." s&
	then
	paragraph
	;description
arch% :attributes
	s" arco" self% name!
	self% is_decoration
	location_18% self% is_there
	;attributes
arch% :description
	\ Provisional!!!
	s" Un sólido arco de piedra, de una sola pieza."
	paragraph
	;description
bed% :attributes
	s" catre" self% name!
	location_46% self% is_there
	;attributes
bed% :description
	s" Parece poco confortable."
	paragraph
	;description
bridge% :attributes
	s" puente" self% name!
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
	;attributes
candles% :description
	s" Están muy consumidas."
	paragraph
	;description
cloak% :attributes
	s" capa" self% fname!
	self% is_cloth
	self% is_owned
	self% is_worn
	ulfius% self% is_there
	;attributes
cloak% :description
	s" Tu capa de general, de fina lana tintada de negro."
	paragraph
	;description
cuirasse% :attributes
	s" coraza" self% fname!
	self% is_cloth
	self% is_owned
	self% is_worn
	ulfius% self% is_there
	;attributes
door% :attributes
	s" puerta" self% fname!
	impossible_error# self% ~take_error# !
	location_47% self% is_there
	;attributes
door% :description
	s" Es muy recia y tiene un gran candado."
\ Inacabado!!!
\	s" Es muy recia y"
\	door% is_open?
\	if  s" tiene un gran candado."
\	else  s" tiene un gran candado."
\	then  s&
	lock_found
	paragraph
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
	s" derrumbe" self% name!
	self% is_decoration
	self% ~take_error# nonsense_error# swap !
	location_09% self% is_there
	;attributes
fallen_away% :description
	s" Muchas, inalcanzables rocas, apiladas una sobre otra."
	paragraph
	;description
flags% :attributes
	s" banderas" self% fnames!
	self% is_decoration
	self% ~take_error# nonsense_error# swap !
	location_28% self% is_there
	;attributes
flags% :description
	s" Son las banderas britana y sajona."
	s" Dos dragones rampantes, rojo y blanco respectivamente, enfrentados." s&
	paragraph
	;description
flint% :attributes
	s" pedernal" self% name!
	;attributes
flint% :description
	s" Es dura y afilada." 
	paragraph
	;description
idol% :attributes
	s" ídolo" self% name!
	self% is_decoration
	self% ~take_error# impossible_error# swap !
	location_41% self% is_there
	;attributes
idol% :description
	s" El ídolo tiene dos agujeros por ojos."
	paragraph
	;description
key% :attributes
	s" llave" self% fname!
	location_46% self% is_there
	;attributes
key% :description
	\ Crear ente!!! hierro, herrumbre y óxido, visibles con la llave en la mano
	s" Grande, de hierro herrumboso."
	paragraph
	;description
lake% :attributes
	s" lago" self% name!
	self% is_decoration
	self% ~take_error# nonsense_error# swap !
	location_44% self% is_there
	;attributes
lake% :description
	s{ s" La" s" Un rayo de" }s
	s" luz entra por un resquicio, y caprichosos reflejos te maravillan." s&
	paragraph
	;description
lock% :attributes
	s" candado" self% name!
	self% is_decoration
	self% ~take_error# impossible_error# swap !
	;attributes
lock% :description
	s" Está" lock% «open»|«closed» s& period+
	s" Es grande y parece resistente." s&
	paragraph
	;description
piece% :description
	s" Es un pedazo" of_your_ex_cloak$ s&
	paragraph
	;description
log% :attributes
	s" tronco" self% name!
	location_15% self% is_there
	;attributes
log% :description
	s" Es un tronco"
	s{ s" recio," s" resistente," s" fuerte," }s&
	s" pero" s&
	s{ s" de liviano peso." s" ligero." }s&
	paragraph
	;description
piece% :attributes
	s" trozo" self% name!
	;attributes
rags% :attributes
	s" harapo" self% name!
	;attributes
rags% :description
	s" Un trozo un poco grande" of_your_ex_cloak$ s&
	paragraph
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
	self% ~is_animal? on
	self% ~take_error# dangerous_error# swap !
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
	self% is_owned
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
	;attributes
table% :description
	s" Es pequeña y de" s{ s" basta" s" tosca" }s& s" madera." s&
	paragraph
	;description
thread% :attributes
	s" hilo" self% name!
	;attributes
thread% :description
	\ Mover esto al evento de cortar la capa!!!
	\ s" Un hilo se ha desprendido al cortar la capa con la espada."
	s" Un hilo" of_your_ex_cloak$ s&
	paragraph
	;description
torch% :attributes
	s" antorcha" self% fname!
	self% ~is_light? on
	self% ~is_lit? off
	;attributes
torch% :description
	\ Inacabado!!! 
	s" Está apagada."
	paragraph
	;description
waterfall% :attributes
	s" cascada" self% fname!
	self% is_decoration
	self% ~take_error# nonsense_error# swap !
	location_38% self% is_there
	;attributes
waterfall% :description
	s" No ves nada por la cortina de agua."
	s" El lago es muy poco profundo." s&
	paragraph
	;description

\ Entes escenario

0  [IF]  \ ......................................

Las palabras que describen entes escenario reciben en SIGHT
(variable que está creada con VALUE y por tanto devuelve su
valor como si fuera una constante) un identificador de ente.
Puede ser el mismo ente escenario o un ente de dirección.
Esto permite describir lo que hay más allá de cada escenario
en cualquier dirección.

2011-11-30 Inacabado!!!  Este sistema está siendo
implementado poco a poco. Las estructuras CASE ya están
puestas.

[THEN]  \ ......................................

location_01% :attributes
	s" aldea sajona" self% fname!
	0 location_02% 0 0 0 0 0 0 self% init_location
	;attributes
location_01% :description
	\ Crear colina!!! en los tres escenarios
	sight  case
	self%  of
		s" No ha quedado nada en pie, ni piedra sobre piedra."
		s{ s" El entorno es desolador." s" Todo alrededor es desolación." }s&
		s{ only$ remains$ s&
		s" Lo único que" remains$ s& s" por hacer" s?& s" es" s&
		s" No" remains$ s& s{ s" más" s" otra cosa" }s& s" que" s&
		}s& to_go_back$ s& s" al Sur, a casa." s&
		paragraph
		endof
	south%  of
		2 random  if \ Versión 0:
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
	up%  of
		s{ s" pronto" s" sin compasión" s" de inmediato" }s
		s{ s" vencidas" s" derrotadas" s" sojuzgadas" }s r2swap s& ^uppercase
		s" por la fría" s&
		s{ s" e implacable" s" y despiadada" }s?&
		s" niebla," s& s" torpes" s" tristes" both?&
		s" columnas de" s& s" negro" s" humo" r2swap s& s&
		(they)_go_up$ s&
		s{ s" lastimosamente" s" penosamente" }s&
		s" hacia" s{ s" el cielo" s" las alturas" }s& s?&
		s{ s" desde" s" de entre" }s& rests_of_the_village$ s&
		s" , como si" s" también" s" ellas" r2swap s& s?&
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
	sight  case
	self%  of
		s" Sobre" s" la cima de" s?&
		s" la colina, casi" s& s{ s" sobre" s" por encima de" }s&
		s" la" s&
		s" espesa" s" fría" both?& s" niebla de la aldea sajona arrasada al Norte, a tus pies." s&
		^the_path$ s& goes_down$ s& toward_the(m)$ s& s" Oeste." s&
		paragraph
		endof
	north%  of
		s" La" poor_village$ s& s" sajona" s& s" , arrasada," s?+ s" agoniza bajo la" s&
		s" espesa" s" fría" both?& s" niebla." s&
		paragraph
		endof
	west%  of
		^the_path$ goes_down$ s& s" por la" s& s" ladera de la" s?& s" colina." s&
		paragraph
		endof
	down%  of
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
	s" camino entre colinas" self% name!
	0 0 location_02% location_04% 0 0 0 0 self% init_location
	;attributes
location_03% :description
	sight  case
	self%  of
		^the_path$ s" avanza por el valle," s&
		s" desde la parte alta, al Este," s&
		s" a una zona" s& very_$ s& s" boscosa, al Oeste." s&
		paragraph
		endof
	east%  of
		^the_path$ s" se pierde en la parte alta del valle." s&
		paragraph
		endof
	west%  of
		s" Una zona" very_$ s& s" boscosa." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_04% :attributes
	s" cruce de caminos" self% name!
	location_05% 0 location_03% location_09% 0 0 0 0 self% init_location
	;attributes
location_04% :description
	sight  case
	self%  of
		s" Una senda parte al Oeste, a la sierra por el paso del Perro,"
		s" y otra hacia el Norte, por un frondoso bosque que la rodea." s&
		paragraph
		endof
	north%  of
		^a_path$ surrounds$ s& s" la sierra a través de un frondoso bosque." s&
		paragraph
		endof
	west%  of
		^a_path$ leads$ s& toward_the(f)$ s& s" sierra por el paso del Perro." s&
		paragraph
		endof
	down%  of  endof
	up%  of  endof
	uninteresting_direction
	endcase
	;description
location_05% :attributes
	s" linde del bosque" self% name!
	0 location_04% 0 location_06% 0 0 0 0 self% init_location
	;attributes
location_05% :description
	sight  case
	self%  of
		^toward_the(m)$ s" Oeste se extiende" s&
		s{ s" frondoso" s" exhuberante" }s& \ pendiente!!! independizar
		s" el bosque que rodea la sierra." s&
		s" La salida se abre" s&
		toward_the(m)$ s& s" Sur." s&
		paragraph
		endof
	south%  of
		s" Se ve la salida del bosque."
		paragraph
		endof
	west%  of
		s" El bosque se extiende"
		s{ s" exhuberante" s" frondoso" }s&
		s" alrededor de la sierra." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_06% :attributes
	s" bosque" self% name!
	0 0 location_05% location_07% 0 0 0 0 self% init_location
	;attributes
location_06% :description
	sight  case
	self%  of
		s" Jirones de niebla se enzarcen en frondosas ramas y arbustos."
		^the_path$ s& s" serpentea entre raíces, de un luminoso Este" s&
		toward_the(m)$ s& s" Oeste." s&
		paragraph
		endof
	east%  of
		s" De la linde del bosque"
		s{ s" procede" s" llega" s" viene" }s&
		s{ s" una cierta" s" algo de" s" un poco de" }s&
		s{ s" claridad" s" luminosidad" }s&
		s" entre" s&
		s{ s" el follaje" s" la vegetación" }s& period+
		paragraph
		endof
	west%  of
		s" La niebla parece más" s" densa" s" oscura" both?& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_07% :attributes
	s" paso del Perro" self% name!
	0 location_08% location_06% 0 0 0 0 0 self% init_location
	;attributes
location_07% :description
	sight  case
	self%  of
		s" Abruptamente, del bosque se pasa a un estrecho camino entre altas rocas."
		s" El" s& s{ s" inquietante" s" sobrecogedor" }s&
		s" desfiladero" s& s{ s" tuerce" s" gira" }s&
		s" de Este a Sur." s&
		paragraph
		endof
	south%  of
		^the_path$ s" gira en esa dirección." s&
		paragraph
		endof
	east%  of
		s" La estrecha senda es" s{ s" engullida" s" tragada" }s&
		s" por las" s&
		s" fauces" s{ s" frondosas" s" exhuberantes" }s r2swap s& s&
		s" del bosque." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_08% :attributes
	s" desfiladero" self% name!
	location_07% 0 0 0 0 0 0 0 self% init_location
	;attributes
location_08% :description
	\ Pendiente!!! Crear pared y roca y desfiladero
	sight  case
	self%  of
		^the_pass_way$ s" entre el desfiladero sigue de Norte a Este" s&
		s" junto a una" s&
		s" rocosa" s" pared" r2swap s& s& period+
		paragraph
		endof
	north%  of
		s" El camino" s{ s" tuerce" s" gira" }s& \ pendiente!!! independizar gira/tuerce
		s" hacia el inquietante paso del Perro." s&
		paragraph
		endof
	south%  of
		self% location_10% s-->
		s" entrada a la cueva" self% fname!
		\ Pendiente!!! Enriquecer el texto
		s" La entrada a una cueva se abre en la pared de roca."
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_09% :attributes
	s" derrumbe" self% name!
	0 0 location_04% 0 0 0 0 0 self% init_location
	;attributes
location_09% :description
	sight  case
	self%  of
		^the_path$ goes_down$ s& s" hacia la agreste sierra, al Oeste," s&
		s" desde los" s& s" verdes" s" valles" r2swap s& s& s" al Este." s&
		s" Pero un gran derrumbe" s& (it)_blocks$ s& s" el paso hacia la sierra." s&
		paragraph
		endof
	east%  of
		^can_see$ s" la salida del bosque." s&
		paragraph
		endof
	west%  of
		s" Un gran derrumbe" (it)_blocks$ s& the_pass$ s& toward$ s& s" la sierra." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_10% :attributes
	s" gruta de entrada" self% fname!
	location_08% 0 0 location_11% 0 0 0 0 self% init_location
	;attributes
location_10% :description
	sight  case
	self%  of
		s" El estrecho paso se adentra hacia el Oeste, desde la boca, al Norte."
		paragraph
		endof
	north%  of
		s" La boca de la gruta conduce al exterior."
		paragraph
		endof
	east%  of
	endof
	uninteresting_direction
	endcase
	;description
location_11% :attributes
	s" gran lago" self% name!
	0 0 location_10% 0 0 0 0 0 self% init_location
	;attributes
location_11% :description
	\ Crear ente!!! estancia y aguas
	sight  case
	self%  of
		s" Una gran estancia alberga un lago"
		s" de profundas e iridiscentes aguas," s&
		s" debido a la luz exterior." s&
		s" No hay otra salida que el Este." s&
		paragraph
		endof
	east%  of
		s" De la entrada de la gruta procede la luz que hace brillar el agua del lago."
		paragraph
	endof
	uninteresting_direction
	endcase
	;description
location_12% :attributes
	s" salida del paso secreto" self% fname!
	0 0 0 location_13% 0 0 0 0 self% init_location
	;attributes
location_12% :description
	\ Crear ente!!! agua aquí
	sight  case
	self%  of
		s" Una gran estancia se abre hacia el Oeste,"
		s" y se estrecha hasta morir, al Este, en una parte de agua." s&
		paragraph
		endof
	east%  of
		s" La estancia se estrecha hasta morir en una parte de agua."
		paragraph
	endof
	west%  of
		s" Se vislumbra la continuación de la cueva."
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_13% :attributes
	s" puente semipodrido" self% name!
	0 0 location_12% location_14% 0 0 0 0 self% init_location
	;attributes
location_13% :description
	\ Crear ente!!! canal, agua, lecho(~catre)
	sight  case
	self%  of
		s" La sala se abre en semioscuridad"
		s" a un puente cubierto de podredumbre" s&
		s" sobre el lecho de un canal, de Este a Oeste." s&
		paragraph
		endof
	east%  of
		s" Se vislumbra el inicio de la cueva."
		paragraph
	endof
	west%  of
		s" Se vislumbra un recodo de la cueva."
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_14% :attributes
	s" recodo de la cueva" self% name!
	0 location_15% location_13% 0 0 0 0 0 self% init_location
	;attributes
location_14% :description
	sight  case
	self%  of
		s" La iridiscente cueva gira de Este a Sur."
		paragraph
		endof
	south%  of
		you_glimpse_the_cave$ paragraph
		endof
	east%  of
		you_glimpse_the_cave$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_15% :attributes
	s" pasaje arenoso" self% name!
	location_14% location_17% location_16% 0 0 0 0 0 self% init_location
	;attributes
location_15% :description
	sight  case
	self%  of
		s" La gruta desciende de Norte a Sur"
		s" sobre un lecho arenoso." s&
		s" Al Este, un agujero del que llega claridad." s&
		paragraph
		endof
	north%  of
		you_glimpse_the_cave$
		s" La cueva asciende en esa dirección." s&
		paragraph
		endof
	south%  of
		you_glimpse_the_cave$
		s" La cueva desciende en esa dirección." s&
		paragraph
		endof
	east%  of
		s" La luz procede de esa dirección."
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_16% :attributes
	s" pasaje del agua" self% name!
	0 0 0 location_15% 0 0 0 0 self% init_location
	;attributes
location_16% :description
\ pendiente!!! el examen del agua aquí debe dar más pistas
	sight  case
	self%  of
		s" Como un acueducto, el agua"
		goes_down$ s& s" con gran fuerza de Norte a Este," s&
		s" aunque la salida practicable es la del Oeste." s&
		paragraph
		endof
	north%  of
		s" El agua" goes_down$ s& s" con gran fuerza" s& from_that_way$ s& period+
		paragraph
		endof
	east%  of
		s" El agua" goes_down$ s& s" con gran fuerza" s& that_way$ s& period+
		paragraph
		endof
	west%  of
		s" Es la única salida." paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_17% :attributes
	s" estalactitas" self% fname!
	location_15% location_20% location_18% 0 0 0 0 0 self% init_location
	;attributes
location_17% :description
	\ Crear ente!!! estalactitas
	sight  case
	self%  of
		s" Muchas estalactitas se agrupan encima de tu cabeza,"
		s" y se abren cual arco de entrada hacia el Este y Sur." s&
		paragraph
		endof
	north%  of
		you_glimpse_the_cave$
		paragraph
		endof
	up%  of
		s" Las estalactitas se agrupan encima de tu cabeza."
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_18% :attributes
	s" puente de piedra" self% name!
	0 0 location_19% location_17% 0 0 0 0 self% init_location
	;attributes
location_18% :description
\ Crear ente!!! puente, arco
	sight  case
	self%  of
		s" Un arco de piedra se eleva, cual puente sobre la oscuridad, de Este a Oeste."
		s" En su mitad, un altar." s&
		paragraph
		endof
	east%  of
		s" El arco de piedra se extiende" that_way$ s& period+
		paragraph
		endof
	west%  of
		s" El arco de piedra se extiende" that_way$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_19% :attributes
	s" recodo arenoso del canal" self% name!
	0 0 0 location_18% 0 0 0 0 self% init_location
	;attributes
location_19% :description
	sight  case
	self%  of
		\ Pendiente!!! Hacer variaciones
		the_water_flow$ comma+
		s" que discurre" s?&
		s" de Norte a Este, impide el paso, excepto al Oeste." s+
		s" Al fondo" s&
		s{ s" se oye" s" se escucha" s" puede oírse" }s&
		s" un gran estruendo." s&
		paragraph
		endof
	north%  of
		^water_from_there$ period+ paragraph
		endof
	east%  of
		water_that_way$ paragraph
		endof
	west%  of
		s" Se puede" to_go_back$ s& toward_the(m)$ s& s" arco de piedra" s& in_that_direction$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_20% :attributes
	s" tramo de cueva" self% name!
	location_17% location_22% location_25% 0 0 0 0 0 self% init_location
	;attributes
location_20% :description
	sight  case
	self%  of
		north% south% east% 3 exits_cave_description paragraph
		endof
	north%  of
		cave_exit_description$ paragraph
		endof
	south%  of
		cave_exit_description$ paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_21% :attributes
	s" tramo de cueva" self% name!
	0 location_27% location_23% location_20% 0 0 0 0 self% init_location
	;attributes
location_21% :description
	sight  case
	self%  of
		east% west% south% 3 exits_cave_description paragraph
		endof
	south%  of
		cave_exit_description$ paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	west%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_22% :attributes
	s" tramo de cueva" self% name!
	0 location_24% location_27% location_22% 0 0 0 0 self% init_location
	;attributes
location_22% :description
	sight  case
	self%  of
		south% east% west% 3 exits_cave_description paragraph
		endof
	south%  of
		cave_exit_description$ paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	west%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_23% :attributes
	s" tramo de cueva" self% name!
	0 location_25% 0 location_21% 0 0 0 0 self% init_location
	;attributes
location_23% :description
	sight  case
	self%  of
		west% south% 2 exits_cave_description paragraph
		endof
	south%  of
		cave_exit_description$ paragraph
		endof
	west%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_24% :attributes
	s" tramo de cueva" self% name!
	location_22% 0 location_26% 0 0 0 0 0 self% init_location
	;attributes
location_24% :description
	sight  case
	self%  of
		east% north% 2 exits_cave_description paragraph
		endof
	north%  of
		cave_exit_description$ paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_25% :attributes
	s" tramo de cueva" self% name!
	location_22% location_28% location_23% location_21% 0 0 0 0 self% init_location
	;attributes
location_25% :description
	sight  case
	self%  of
		north% south% east% west% 4 exits_cave_description paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	west%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_26% :attributes
	s" tramo de cueva" self% name!
	location_26% 0 location_20% location_27% 0 0 0 0 self% init_location
	;attributes
location_26% :description
\ Crear ente!!! pasaje/camino/senda tramo/cueva (en todos los tramos)
	sight  case
	self%  of
		north% east% west% 3 exits_cave_description paragraph
		endof
	north%  of
		cave_exit_description$ paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	west%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_27% :attributes
	s" tramo de cueva" self% name!
	location_27% 0 0 location_25% 0 0 0 0 self% init_location
	;attributes
location_27% :description
	sight  case
	self%  of
		north% east% west% 3 exits_cave_description paragraph
		endof
	north%  of
		cave_exit_description$ paragraph
		endof
	east%  of
		cave_exit_description$ paragraph
		endof
	west%  of
		cave_exit_description$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_28% :attributes
	s" refugio" self% name!
	location_26% 0 0 0 0 0 0 0 self% init_location
	;attributes
location_28% :description
	sight  case
	self%  of
		\ Crear ente!!! refugiados,estancia(para todos,estancia(para todos),albergue
		self% is_known?  if  s" La"  else  s" Una"  then
		s" amplia estancia, que se extiende de Norte a Este," s&
		leader% conversations?
		if  s" hace de albergue para los refugiados."
		else  s" está llena de gente. Parecen refugiados."
		then  s& 
		s" Hay banderas de ambos bandos." s&
		paragraph
		endof
	east%  of
		s" Los refugiados"
		self% has_east_exit?
		if  they_let_you_pass$ s&
		else  they_don't_let_you_pass$ s&
		then  period+ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_29% :attributes
	s" espiral" self% fname!
	0 0 0 location_28% 0 location_30% 0 0 self% init_location
	;attributes
location_29% :description
\ Crear ente!!! escalera/espiral, refugiados
	sight  case
	self%  of
		s" Cual escalera de caracol gigante,"
		goes_down_into_the_deep$ comma+ s&
		s" dejando a los refugiados al Oeste." s&
		paragraph
		endof
	west%  of
		over_there$ s" están los refugiados." s&
		paragraph
		endof
	down%  of
		s" La espiral" goes_down_into_the_deep$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_30% :attributes
	s" inicio de la espiral" self% name!
	0 0 location_31% 0 location_29% 0 0 0 self% init_location
	;attributes
location_30% :description
	sight  case
	self%  of
		s" Se eleva en la penumbra."
		s" La" s& cave$ s& gets_narrower(f)$ s&
		s" ahora como para una sola persona, hacia el Este." s&
		paragraph
		endof
	east%  of
		s" La" cave$ s& gets_narrower(f)$ s& period+
		paragraph
		endof
	up%  of
		s" La" cave$ s& s" se eleva en la penumbra." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_31% :attributes
	s" puerta norte" self% fname!
	0 0 0 location_30% 0 0 0 0 self% init_location
	;attributes
location_31% :description
\ Crear ente!!! arco, columnas, hueco/s(entre rocas)
	sight  case
	self%  of
		s" En este pasaje grandes rocas se encuentran entre las columnas de un arco de medio punto."
		paragraph
		endof
	north%  of
		s" Las rocas"  self% has_north_exit?
		if  (rocks)_on_the_floor$
		else  (they)_block$ the_pass$ s&
		then  s& period+ paragraph
		endof
	west%  of
		^that_way$ s" se encuentra el inicio de la espiral." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_32% :attributes
	s" precipicio" self% name!
	0 location_33% 0 location_31% 0 0 0 0 self% init_location
	;attributes
location_32% :description
\ Crear ente!!! precipicio, abismo, cornisa, camino, roca/s
	sight  case
	self%  of
		s" El camino ahora no excede de dos palmos de cornisa sobre un abismo insondable."
		s" El soporte de roca gira en forma de «U» de Oeste a Sur." s&
		paragraph
		endof
	south%  of
		^the_path$ s" gira" s& that_way$ s& period+
		paragraph
		endof
	west%  of
		^the_path$ s" gira" s& that_way$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_33% :attributes
	s" pasaje de salida" self% name!
	location_32% 0 location_34% 0 0 0 0 0 self% init_location
	;attributes
location_33% :description
\ Crear ente!!! camino/paso/sendero
	sight  case
	self%  of
		s" El paso se va haciendo menos estrecho a medida que se avanza hacia el Sur, para entonces comenzar hacia el Este."
		paragraph
		endof
	north%  of
		^the_path$ s" se estrecha" s& that_way$ s& period+
		paragraph
		endof
	south%  of
		^the_path$ gets_wider$ s& that_way$ s&
		s" y entonces gira hacia el Este." s&
		paragraph
		endof
	east%  of
		^the_path$ gets_wider$ s& that_way$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_34% :attributes
\ Crear ente!!! gravilla
	s" pasaje de gravilla" self% name!
	location_35% 0 0 location_33% 0 0 0 0 self% init_location
	;attributes
location_34% :description
\ Crear ente!!! camino/paso/sendero, guijarros, moho, roca, suelo...
	sight  case
	self%  of
		\ anchea?!!!
		\ crear guijarros, moho,
		s" El paso" gets_wider$ s& s" de Oeste a Norte," s&
		s" y guijarros mojados y mohosos tachonan el suelo de roca." s&
		paragraph
		endof
	north%  of
		^the_path$ gets_wider$ s& that_way$ s& period+
		paragraph
		endof
	west%  of
		^the_path$ s" se estrecha" s& that_way$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_35% :attributes
	s" puente sobre el acueducto" self% name!
	location_40% location_34% 0 location_36% 0 location_36% 0 0 self% init_location
	;attributes
location_35% :description
\ Crear ente!!! escaleras, puente, río/curso/agua
	sight  case
	self%  of
		s" Un puente" s{ s" se tiende" s" cruza" }s& s" de Norte a Sur sobre el curso del agua." s&
		s" Unas resbaladizas escaleras" s& (they)_go_down$ s& s" hacia el Oeste." s&
		paragraph
		endof
	north%  of
		bridge_that_way$ paragraph
		endof
	south%  of
		bridge_that_way$ paragraph
		endof
	west%  of
		stairway_to_river$ paragraph
		endof
	down%  of
		stairway_to_river$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_36% :attributes
	s" remanso" self% name!
	0 0 location_35% location_37% location_35% 0 0 0 self% init_location
	;attributes
location_36% :description
	sight  case
	self%  of
		s" Una" s{ s" ruidosa" s" estruendosa" s" ensordecedora" }s&
		s" corriente" s& goes_down$ s&
		s{ s" con" s" siguiendo" }s& s" el" s& pass_way$ s&
		s" elevado desde el Oeste, y forma un meandro arenoso." s&
		s" Unas escaleras" s& (they)_go_up$ s& toward_the(m)$ s& s" Este." s&
		paragraph
		endof
	east%  of
		stairway_that_way$ paragraph
		endof
	west%  of
		^water_from_there$ period+ paragraph
		endof
	up%  of
		stairway_that_way$ paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_37% :attributes
	s" canal de agua" self% name!
	0 0 location_36% location_38% 0 0 0 0 self% init_location
	;attributes
location_37% :description
	sight  case
	self%  of
		s" El agua" goes_down$ s& s" por un canal" s?&
		from_the(m)$ s& s" Oeste con" s&
		s{ s" renovadas fuerzas" s" renovada energía" s" renovado ímpetu" }s& comma+
		s" dejando" s& s{
		s" a un lado" a_high_narrow_pass_way$ s&
		a_high_narrow_pass_way$ s{ s" lateral" s" a un lado" }s&
		}s& s" que" s& lets_you$ s& to_keep_going$ s&
		toward_the(m)$ s" Este" s&
		toward_the(m)$ s" Oeste" s& r2swap s" o" s& 2swap s& s&
		period+ paragraph
		endof
	east%  of
		^the_pass_way$ s" elevado" s?& lets_you$ s& to_keep_going$ s& that_way$ s& period+
		paragraph
		endof
	west%  of
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
	0 0 location_37% location_39% 0 0 0 0 self% init_location
	;attributes
location_38% :description
	sight  case
	self%  of
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
	east%  of
		water_that_way$ paragraph
		endof
	west%  of
		\ pendiente!!! el artículo de «cascada» debe depender también de si se ha visitado el escenario 39 o este mismo 38
		^water_from_there$
		s" , de" s+ waterfall% full_name s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_39% :attributes
	s" interior de la cascada" self% name!
	0 0 location_38% 0 0 0 0 0 self% init_location
	;attributes
location_39% :description
	sight  case
	self%  of
		\ Crear ente!!! musgo, cortina, agua, hueco
		s" Musgoso y rocoso, con la cortina de agua"
		s{ s" tras de ti," s" a tu espalda," }s&
		s{ s" el nivel" s" la altura" }s& s" del agua ha" s&
		s{ s" subido" s" crecido" }s&
		s{ s" un poco" s" algo" }s& s" en este" s&
		s{ s" curioso" s" extraño" }s& s" hueco." s&
		paragraph
		endof
	east%  of
		\ pendiente!!! variar
		s" Es la única salida." paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_40% :attributes
	s" explanada" self% fname!
	0 location_35% location_41% 0 0 0 0 0 self% init_location
	;attributes
location_40% :description
\ Crear ente!!! losas y losetas, estalactitas, panorama, escalones
	sight  case
	self%  of
		s" Una gran explanada enlosetada contempla un bello panorama de estalactitas."
		s" Unos casi imperceptibles escalones conducen al Este." s&
		paragraph
		endof
	south%  of
		^that_way$ s" se va" s& toward_the(m)$ s& s" puente." s&
		paragraph
		endof
	east%  of
		s" Los escalones" (they)_lead$ s& that_way$ s& period+
		paragraph
		endof
	up%  of
		s{ s" Sobre" s" Por encima de" }s
		s{ s" ti" s" tu cabeza" }s& s" se" s& 
		s{ s" exhibe" s" extiende" s" disfruta" }s&
		s" un" s& beautiful(m)$ s&
		s{ s" panorama" s" paisaje" }s s& s" de estalactitas." s&
		paragraph
		endof
	down%  of
		s" Es una" s{ s" gran" s" buena" }s s& s" explanada enlosetada." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_41% :attributes
\ Pendiente!!! Cambiar el nombre. No se puede pasar a mayúscula un carácter pluriocteto en UTF-8.
	s" ídolo" self% name!
	0 0 0 location_40% 0 0 0 0 self% init_location
	;attributes
location_41% :description
\ Crear ente!!! roca, centinela
	sight  case
	self%  of
		s" El ídolo parece un centinela siniestro de una gran roca que se encuentra al Sur."
		s" Se puede" s& to_go_back$ s& toward$ s& s" la explanada hacia el Oeste." s&
		paragraph
		endof
	south%  of
		s" Hay una" s" roca" s" enorme" r2swap s& s&
		that_way$ s& period+
		paragraph
		endof
	west%  of
		s" Se puede volver" toward$ s& s" la explanada" s& that_way$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_42% :attributes
	s" pasaje estrecho" self% name!
	location_41% location_43% 0 0 0 0 0 0 self% init_location
	;attributes
location_42% :description
	sight  case
	self%  of
		s" Como un pasillo que corteja el canal de agua, a su lado, baja de Norte a Sur."
		paragraph
		endof
	north%  of
		^the_pass_way$ goes_up$ s& that_way$ s&
		s" , de donde" s{ s" corre" s" procede" s" viene" s" proviene" }s& s" el agua." s& s+
		paragraph
		endof
	south%  of
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
	s" pasaje de la serpiente" self% name!
	location_42% 0 0 0 0 0 0 0 self% init_location
	;attributes
location_43% :description
	sight  case
	self%  of
		^the_pass_way$ s" sigue de Norte a Sur." s&
		paragraph
		endof
	north%  of
		^the_pass_way$ s" continúa" s& that_way$ s& period+
		paragraph
		endof
	south%  of
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
	s" lago interior" self% name!
	location_43% 0 0 location_45% 0 0 0 0 self% init_location
	;attributes
location_44% :description
\ Crear ente!!! lago, escaleras, pasaje, lago
	sight  case
	self%  of
		s" Unas escaleras" s{ s" dan" s" permiten el" }s& s{ s" paso" s" acceso" }s&
		s" a un" s& beautiful(m)$ s& s" lago interior, hacia el Oeste." s&
		s" Al Norte, un oscuro y"
		narrow(m)$ s& pass_way$ s& goes_up$ s& period+ s?&
		paragraph
		endof
	north%  of
		s" Un pasaje oscuro y" narrow(m)$ s& goes_up$ s& that_way$ s& period+
		paragraph
		endof
	west%  of
		s" Las escaleras" (they)_lead$ s& that_way$ s& s" , hacia el lago" s?+ period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_45% :attributes
	s" cruce de pasajes" self% name!
	0 location_47% location_44% location_46% 0 0 0 0 self% init_location
	;attributes
location_45% :description
\ Crear ente!!! pasaje/camino/paso/senda
	sight  case
	self%  of
		^narrow(mp)$ pass_ways$ s&
		s" permiten ir al Oeste, al Este y al Sur." s&
		paragraph
		endof
	south%  of
		^a_narrow_pass_way$ s" permite ir" s& that_way$ s&
		s" , de donde" s+ s{ s" proviene" s" procede" }s&
		s{ s" una gran" s" mucha" }s& s" luminosidad." s&
		paragraph
		endof
	west%  of
		^a_narrow_pass_way$ leads$ s& that_way$ s& period+
		paragraph
		endof
	east%  of
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
	s" hogar de Ambrosio" self% name!
	0 0 location_45% 0 0 0 0 0 self% init_location
	;attributes
location_46% :description
	sight  case
	self%  of
		s" Un catre, algunas velas y una mesa es todo lo que"
		s{ s" tiene" s" posee" }s s" Ambrosio" r2swap s& s&
		period+  paragraph
		endof
	east%  of
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
	location_45% 0 0 0 0 0 0 0 self% init_location
	;attributes
location_47% :description
\ Descripción inacabada!!! 
	sight  case
	self%  of
		s" Por el Oeste,"
		door% full_name s& door% «open»|«closed» s&
		door% is_open?  if
			comma+
			s" por la cual entra la luz que ilumina la estancia," s&
			s" permite salir de la cueva."
		else
			door% is_known?
			if
				comma+
				s" al otro lado de la cual se adivina la luz diurna," s&
				s" impide la salida de la cueva." s&
			else
				comma+
				s" al otro lado de la cual se adivina la luz diurna," s&
				s" parece ser la salida de la cueva." s&
			then
		then
		door% is_open?  if
			s" adivina la luz diurna al otro lado." s&
		else
			s" Se adivina la luz diurna al otro lado." s&
		then
		paragraph
		endof
	north%  of
		\ pendiente!!! variar
		s" Hay salida" that_way$ s& period+ paragraph
		endof
	west%  of
		\ pendiente!!! variar
		door% is_open?  if
			s" La luz diurna que entra por la puerta."
		else	
			s" Se adivina la luz diurna al otro lado de la puerta."
		then
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_48% :attributes
	s" bosque a la entrada" self% name!
	0 0 location_47% location_49% 0 0 0 0 self% init_location
	;attributes
location_48% :description
	\ Crear ente!!! cueva
	sight  case
	self%  of
		s{ s" Apenas" s" Casi no" }s
		s{ s" se puede" s" es posible" }s&
		s" reconocer la entrada de la cueva, al Este." s&
		^the_path$ s& s{ s" parte" s" sale" }s&
		s" del bosque hacia el Oeste." s&
		paragraph
		endof
	east%  of
		s" La entrada de la cueva" s{
		s" está" s" bien" s?& s{ s" camuflada" s" escondida" }s&
		s" apenas se ve" s" casi no se ve" s" pasa casi desapercibida"
		}s& period+ paragraph
		endof
	west%  of
		^the_path$ s{ s" parte" s" sale" }s& s" del bosque" s& in_that_direction$ s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_49% :attributes
	s" sendero del bosque" self% name!
	0 0 location_48% location_50% 0 0 0 0 self% init_location
	;attributes
location_49% :description
	sight  case
	self%  of
		^the_path$ s" recorre" s& s" toda" s?&
		s" esta" s& s{ s" parte" s" zona" }s&
		s" del bosque de Este a Oeste." s&
		paragraph
		endof
	east%  of
		^the_path$ leads$ s&
		s" al bosque a la entrada de la cueva." s&
		paragraph
		endof
	west%  of
		^the_path$ s" continúa" s& in_that_direction$ s& period+
		paragraph
		endof
	uninteresting_direction
	endcase
	;description
location_50% :attributes
	s" camino norte" self% name!
	0 location_51% location_49% 0 0 0 0 0 self% init_location
	;attributes
location_50% :description
	sight  case
	self%  of
		s" El camino norte que sale de Westmorland se interna en el bosque,"
		s" aunque en tu estado no puedes ir." s&
		paragraph
		endof
	south%  of
		s{ s" ¡Westmorland!" s" Westmorland..." }s
		paragraph
		endof
	east%  of
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
	sight  case
	self%  of
		^the_village$ s" bulle de actividad con el mercado en el centro de la plaza," s&
		s" donde se encuentra el castillo." s&
		paragraph
		endof
	north%  of
		s" El camino norte" of_the_village$ s& leads$ s& s" hasta el bosque." s&
		paragraph
		endof
	uninteresting_direction
	endcase
	;description

\ Entes globales

cave% :attributes
	s" cueva" self% fname!
	self% ~is_global_indoor? on
	;attributes
cave% :description
	\ Provisional!!!
	s" La cueva es chachi."
	paragraph
	;description
ceiling% :attributes
	s" techo" self% name!
	self% ~is_global_indoor? on
	;attributes
ceiling% :description
	\ Provisional!!!
	s" El techo es muy bonito."
	paragraph
	;description
clouds% :attributes
	s" nubes" self% fnames!
	self% ~is_global_outdoor? on
	;attributes
clouds% :description
	\ Pendiente!!!:
	\ Distinguir no solo interiores, sino escenarios en
	\ que se puede vislumbrar el exterior.
	\ Provisional!!!:
	s" Los estratocúmulos que traen la nieve y que cuelgan sobre la Tierra"
	s" en la estación del frío se han alejado por el momento. " s&
	2 random  if  paragraph  else  2drop sky% describe  then  \ comprobar!!!
	;description
floor% :attributes
	s" suelo" self% name!
	self% ~is_global_indoor? on
	self% ~is_global_outdoor? on
	;attributes
floor% :description
	\ Provisional!!!
	am_i_outdoor?  if
		s" El suelo fuera es muy bonito."
	paragraph
	else
		s" El suelo dentro es muy bonito."
	paragraph
	then
	;description
sky% :attributes
	s" cielo" self% name!
	self% ~is_global_outdoor? on
	;attributes
sky% :description
	\ Provisional!!!
	s" El cielo es un cuenco de color azul, listado en lo alto por nubes"
	s" del tipo cirros, ligeras y trasparentes." s&
	paragraph
	;description

\ Entes virtuales

exits% :attributes
	s" salida" self% fname!
	self% is_global_outdoor
	self% is_global_indoor
	;attributes
exits% :description
	do_exits
	;description
inventory% :attributes
	;attributes
enemy% :attributes
	\ Inacabado!!!
	s" enemigos" self% names!
	self% is_human
	self% is_decoration
	;attributes
enemy% :description
	\ Inacabado!!!
	battle# @ if
	s" Enemigo en batalla!!!"  \ tmp!!!
	else  s" Enemigo en paz!!!"  \ tmp!!!
	then  paragraph
	;description

\ Entes dirección

\ Los entes dirección guardan en su campo ~DIRECTION
\ el desplazamiento correspodiente al campo de 
\ dirección que representan 
\ Esto sirve para reconocerlos como tales entes dirección 
\ (pues todos los valores posibles son diferentes de cero)
\ y para hacer los cálculos en las acciones de movimiento.

north% :attributes
	s" Norte" self% name!
	self% has_definite_article
	north_exit> self% ~direction !
	;attributes
south% :attributes
	s" Sur" self% name!
	self% has_definite_article
	south_exit> self% ~direction !
	;attributes
east% :attributes
	s" Este" self% name!
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
	am_i_outdoor?  if  
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
	
\ }}}###########################################################
section( Errores de las acciones)  \ {{{

variable action  \ Código de la acción del comando
variable main_complement  \ Código del complemento principal del comando (generalmente es el complemento directo)
variable tool_complement  \ Código del complemento instrumental del comando  \ No utilizado!!!
variable other_complement  \ Código del complemento indirecto o preposicional del comando \ No utilizado!!!
variable what  \ Ente que ha provocado un error y puede ser citado en el mensaje de error correspondiente

: known_entity_is_not_here$  ( a -- a1 u1 )  \  Devuelve mensaje de que un ente conocido no está presente
	full_name s" no está" s&
	s{ s" aquí" s" por aquí" }s& 
	;
: unknown_entity_is_not_here$  ( a -- a1 u1 ) \  Devuelve mensaje de que un ente desconocido no está presente
	s{ s" Aquí" s" Por aquí" }s
	s" no hay" s&
	rot subjective_negative_name s&
	;
: is_not_here  ( a -- )  \  Informa de que un ente no está presente
	dup ~familiar @
	if  known_entity_is_not_here$
	else  unknown_entity_is_not_here$
	then  period+ narrate
	;
' is_not_here constant (is_not_here_error#)
' (is_not_here_error#) is is_not_here_error#
: is_not_here_what  ( -- )  \  Informa de que el ente WHAT no está presente
	what @ is_not_here
	;
' is_not_here_what constant (is_not_here_what_error#)
' (is_not_here_what_error#) is is_not_here_what_error#
: cannot_see  ( a -- )  \ Informa de que un ente no puede ser mirado
	^cannot_see$
	rot subjective_negative_name_as_direct_object s&
	period+ narrate
	;
' cannot_see constant (cannot_see_error#)
' (cannot_see_error#) is cannot_see_error#
: cannot_see_what   \ Informa de que el ente WHAT no puede ser mirado
	what @ cannot_see
	;
' cannot_see_what constant (cannot_see_what_error#)
' (cannot_see_what_error#) is cannot_see_what_error#
: like_that$  ( -- a u )
	\ No se usa!!!
	s{ s" así" s" como eso" }s
	;
: something_like_that$  ( -- a u )  \ Devuelve una variante de «hacer eso»
	s" hacer" s?
	s{ s" algo así"
	s" algo semejante"
	s" eso"
	s" semejante cosa"
	s" tal cosa"
	s" una cosa así" }s&
	;
: is_impossible$  ( -- a u )  \ Devuelve una variante de «es imposible», que formará parte de mensajes personalizados por cada acción
	s{
	s" es imposible"
	s" es inviable"
	s" no es posible"
	s" no es viable" 
	s" no sería posible"
	s" no sería viable"
	s" sería imposible"
	s" sería inviable"
	}s
	;
: ^is_impossible$  ( -- a u )  \ Devuelve una variante de «Es imposible» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción
	is_impossible$ ^uppercase
	;
: x_is_impossible$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «X es imposible»
	dup
	if  ^uppercase is_impossible$ s&
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
: impossible  ( -- )  \ Informa de que una acción no especificada es imposible
	[debug]  [IF]  s" En IMPOSSIBLE" debug  [THEN]  \ Depuración!!!
	something_like_that$ is_impossible
	;
' impossible constant (impossible_error#)
' (impossible_error#) is impossible_error#
: try$  ( -- a u )  \ Devuelve una variante de «intentar» (o vacía)
	s{ 0$ 0$ s" intentar" }s
	;
: nonsense$  ( -- a u )  \ Devuelve una variante de «no tiene sentido», que formará parte de mensajes personalizados por cada acción
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
: ^nonsense$  ( -- a u )  \ Devuelve una variante de «No tiene sentido» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción
	nonsense$ ^uppercase
	;
: x_is_nonsense$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante de «X no tiene sentido»
	dup
	if  try$ 2swap s& ^uppercase nonsense$ s&
	else  2drop ^nonsense$
	then
	;
: it_is_nonsense_x$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante de «No tiene sentido x»
	^nonsense$ try$ s& 2swap s& 
	;
: is_nonsense  ( a u -- ) \ Informa de que una acción dada no tiene sentido
	\ a u = Acción que no tiene sentido; es un verbo en infinitivo, un sustantivo o una cadena vacía
	['] x_is_nonsense$
	['] it_is_nonsense_x$ 
	2 choose execute  period+ narrate
	;
: nonsense  ( -- )  \ Informa de que alguna acción no especificada no tiene sentido
	\ Provisional!!!
	[debug]  [IF]  s" En NONSENSE" debug  [THEN]  \ Depuración!!!
	s" eso" is_nonsense 
	;
' nonsense constant (nonsense_error#)
' (nonsense_error#) is nonsense_error#
: dangerous$  ( -- a u )  \ Devuelve una variante de «es peligroso», que formará parte de mensajes personalizados por cada acción
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
: ^dangerous$  ( -- a u )  \ Devuelve una variante de «Es peligroso» (con la primera letra en mayúsculas) que formará parte de mensajes personalizados por cada acción
	dangerous$ ^uppercase
	;
: x_is_dangerous$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante «X es peligroso»
	dup
	if  try$ 2swap s& ^uppercase dangerous$ s&
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
: dangerous  ( -- )  \ Informa de que alguna acción no especificada no tiene sentido
	something_like_that$ is_dangerous
	;
' dangerous constant (dangerous_error#)
' (dangerous_error#) is dangerous_error#
: ?full_name&  ( a1 u1 a2 -- )  \ Añade a una cadena el nombre de un posible ente
	\ No se usa!!!
	\ a1 u1 = Cadena
	\ a2 = Ente (o cero)
	?dup  if  full_name s&  then
	;
: +is_nonsense  ( a u a1 -- )  \ Informa de una acción dada (en infinitivo) ejecutada sobre un ente no tiene sentido
	\ a u = Acción en infinitivo
	\ a1 = Ente al que se refiere la acción y cuyo objeto directo es (o cero)
	?dup
	if full_name s& is_nonsense
	else  2drop nonsense
	then
	;
: main_complement+is_nonsense  ( a u -- ) \ Informa de una acción dada (en infinitivo), que hay que completar con el nombre del complemento directo, no tiene sentido
	main_complement @ +is_nonsense
	;
: other_complement+is_nonsense  ( a u -- ) \ Informa de una acción dada (en infinitivo), que hay que completar con el nombre del complemento auxiliar, no tiene sentido
	other_complement @ +is_nonsense
	;
: no_reason_for$  ( -- a u )  \ Devuelve una variante de «no hay motivo para»
	\ Pendiente!!! Quitar las variantes que no sean adecuadas a todos los casos
	s" No hay" s{
	s" motivo alguno para"
	s" motivo para"
	s" nada que justifique"
	s" necesidad alguna de"
	s" necesidad de"
	s" ninguna necesidad de"
	s" ninguna razón para"
	s" ningún motivo para"
	s" razón alguna para"
	s" razón para"
	}s&
	;
: no_reason_for_that  ( a u -- )  \ Informa de que no hay motivo para una acción (en infinitivo)
	\ a u = Acción para la que no hay razón, en infinitivo, o una cadena vacía
	\ Pendiente!!!
	no_reason_for$ 2swap s& period+ narrate
	;
: no_reason  ( -- )  \ Informa de que no hay motivo para una acción no especificada
	\ Pendiente!!!
	something_like_that$ no_reason_for_that
	;
: nonsense|no_reason  ( -- )  \ Informa de que una acción no especificada no tiene sentido o no tiene motivo
	\ No se usa todavía!!!
	['] nonsense
	['] no_reason
	2 choose execute
	;
variable silent_well_done?  silent_well_done? off
: well_done  ( -- )  \ Informa de que una acción se ha realizado
	silent_well_done? @ 0=
	if  s" Hecho." narrate  then
	silent_well_done? off
	;
: (do_not_worry_0)$  ( -- a u)  \ Primera versión posible del mensaje de DO_NOT_WORRY
	s{
	s" Como si no hubiera"
	s" Hay"
	s" Se diría que hay"
	s" Seguro que hay" 
	s" Sin duda hay" 
	}s
	s{ s" cosas" s" tareas" s" asuntos" s" cuestiones" }s&
	s" más" s&
	s{ s" importantes" s" necesarias" s" urgentes" s" útiles" }s&
	s{
	0$ s" a que prestar atención" s" de que ocuparse"
	s" para ocuparse" s" para prestarles atención"
	}s&
	;
: (do_not_worry_1)$  ( -- a u)  \ Segunda versión posible del mensaje de DO_NOT_WORRY
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
: do_not_worry  ( -- )  \ Informa de que una acción no tiene importancia
	\ Provisional!!! No se usa!!!
	['] (do_not_worry_0)$
	['] (do_not_worry_1)$ 2 choose execute
	now_$ s&  period+ narrate
	;

0  [IF]  \ Error «no tiene nada especial», aún en desarrollo!!!

: it_is_normal_x$  ( a1 u1 -- a2 u2 )  \ Devuelve una variante de «no tiene nada especial x»
	^normal$ try$ s& 2swap s& 
	;
: is_normal  ( a -- )  \ Informa de que un ente no tiene nada especial
	\ a u = Acción que no tiene nada especial; es un verbo en infinitivo, un sustantivo o una cadena vacía
	['] x_is_normal$
	['] it_is_normal_x$ 
	2 choose execute  period+ narrate
	;
' is_normal constant (is_normal_error#)
' (is_normal_error#) is is_normal_error#

[THEN]

: that$  ( a -- a1 u1 )  \  Devuelve el nombre de un ente, o un pronombre demostrativo
	2 random
	if  drop s" eso"  else  full_name  then
	;
: you_do_not_have_it_(0)$  ( a -- )  \ Devuelve mensaje de que el protagonista no tiene un ente (variante 0)
	s" No" you_carry$ s& rot that$ s& with_you$ s&
	;
: you_do_not_have_it_(1)$  ( a -- )  \ Devuelve mensaje de que el protagonista no tiene un ente (variante 1, solo para entes conocidos)
	s" No" rot direct_pronoun s& you_carry$ s& with_you$ s&
	;
: you_do_not_have_it_(2)$  ( a -- )  \ Devuelve mensaje de que el protagonista no tiene un ente (variante 2, solo para entes no citados en el comando)
	s" No" you_carry$ s& rot full_name s& with_you$ s&
	;
: you_do_not_have_it  ( a -- )  \ Informa de que el protagonista no tiene un ente
	dup is_known?  if
		['] you_do_not_have_it_(0)$
		['] you_do_not_have_it_(1)$
		2 choose execute
	else  you_do_not_have_it_(0)$
	then  period+ narrate
	;
' you_do_not_have_it constant (you_do_not_have_it_error#)
' (you_do_not_have_it_error#) is you_do_not_have_it_error#
: you_do_not_have_what  ( -- )  \ Informa de que el protagonista no tiene el ente WHAT
	what @ you_do_not_have_it
	;
' you_do_not_have_what constant (you_do_not_have_what_error#)
' (you_do_not_have_what_error#) is you_do_not_have_what_error#

: it_seems$  ( -- a u )
	s{ 0$ s" parece que" s" por lo que parece," }s
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
: not_by_hand_0$  ( -- a u )  \ Devuelve la primera versión del mensaje de NOT_BY_HAND
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
: not_by_hand_1$  ( -- a u )  \ Devuelve la segunda versión del mensaje de NOT_BY_HAND
	it_seems$
	s{
		s{ s" hará" s" haría" s" hace" }s s" falta" s&
		s{ 
			s{ s" será" s" sería" s" es" }s s" menester" s&
			s{ s" habrá" s" habría" s" hay" }s s" que" s&
		}s s{ s" usar" s" utilizar" s" emplear" }s&
	}s& some_tool$ s& period+ ^uppercase
	;
: not_by_hand$  ( -- a u )  \ Devuelve mensaje de NOT_BY_HAND
	['] not_by_hand_0$
	['] not_by_hand_1$
	2 choose execute ^uppercase
	;
: not_by_hand  ( -- )  \ Informa de que la acción no puede hacerse sin una herramienta
	not_by_hand$ narrate
	;
: you_need  ( a -- )  \ Informa de que el protagonista no tiene un ente necesario
	2 random
	if  you_do_not_have_it_(2)$ period+ narrate
	else  drop not_by_hand
	then
	;
: you_need_what  ( -- )  \ Informa de que el protagonista no tiene el ente WHAT necesario
	what @ you_need
	;
' you_need_what constant (you_need_what_error#)
' (you_need_what_error#) is you_need_what_error#
: you_already_have_it_(0)$  ( a -- )  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 0)
	s" Ya" you_carry$ s& rot that$ s& with_you$ s&
	;
: you_already_have_it_(1)$  ( a -- )  \ Devuelve mensaje de que el protagonista ya tiene un ente (variante 1, solo para entes conocidos)
	s" Ya" rot direct_pronoun s& you_carry$ s& with_you$ s&
	;
: you_already_have_it  ( a -- )  \ Informa de que el protagonista ya tiene un ente
	dup ~familiar @ over ~is_owned? or   if
		['] you_already_have_it_(0)$
		['] you_already_have_it_(1)$
		2 choose execute
	else  you_already_have_it_(0)$
	then  period+ narrate
	;
' you_already_have_it constant (you_already_have_it_error#)
' (you_already_have_it_error#) is you_already_have_it_error#
: you_already_have_what  ( a -- )  \ Informa de que el protagonista ya tiene el ente WHAT
	what @ you_already_have_it
	;
' you_already_have_what constant (you_already_have_what_error#)
' (you_already_have_what_error#) is you_already_have_what_error#
: (you_do_not_wear_it)  ( a -- )  \ Informa de que el protagonista no lleva puesto un ente prenda
	>r s" No llevas puest" r@ noun_ending+
	r> full_name s& period+ narrate
	;
: you_do_not_wear_it  ( a -- )  \ Informa de que el protagonista no lleva puesto un ente prenda, según lo lleve o no consigo
	dup is_hold?
	if  you_do_not_have_it
	else  (you_do_not_wear_it) 
	then
	;
: you_do_not_wear_what  ( -- )  \ Informa de que el protagonista no lleva puesto el ente WHAT , según lo lleve o no consigo
	what @ you_do_not_wear_it
	;
' you_do_not_wear_what constant (you_do_not_wear_what_error#)
' (you_do_not_wear_what_error#) is you_do_not_wear_what_error#
: you_already_wear_it  ( a -- )  \ Informa de que el protagonista lleva puesto un ente prenda
	>r s" Ya llevas puest" r@ noun_ending+
	r> full_name s& period+ narrate
	;
: you_already_wear_what  ( -- )  \ Informa de que el protagonista lleva puesto el ente WHAT
	what @ you_already_wear_it
	;
' you_already_wear_what constant (you_already_wear_what_error#)
' (you_already_wear_what_error#) is you_already_wear_what_error#
: not_with_that$  ( -- a u )  \ Devuelve mensaje de NOT_WITH_THAT
	s" Con eso no..." 
	s" No con eso..." 
	2 schoose
	;
: not_with_that  ( -- )  \ Informa de que la acción no puede hacerse con la herramienta elegida
	not_with_that$ narrate
	;
: it_is_already_open  ( a -- )  \ Informa de que un ente ya está abierto
	s" Ya está abiert" r@ noun_ending+ period+ narrate
	;
: what_is_already_open  ( -- )  \ Informa de que el ente WHAT ya está abierto
	what @ it_is_already_open
	;
' what_is_already_open constant (what_is_already_open_error#)
' (what_is_already_open_error#) is what_is_already_open_error#
: it_is_already_closed  ( a -- )  \ Informa de que un ente ya está cerrado
	s" Ya está cerrad" r@ noun_ending+ period+ narrate
	;
: what_is_already_closed  ( -- )  \ Informa de que el ente WHAT ya está cerrado
	what @ it_is_already_closed
	;
' what_is_already_closed constant (what_is_already_closed_error#)
' (what_is_already_closed_error#) is what_is_already_closed_error#

\ }}}###########################################################
section( Listas)  \ {{{

variable #listed  \ Contador de elementos listados, usado en varias acciones
variable #elements  \ Total de los elementos de una lista

: list_separator$  ( u1 u2 -- a u )  \ Devuelve el separador adecuado a un elemento de una lista
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	\ a u = Cadena devuelta, que podrá ser « y » o «, » o «» (vacía)
	?dup  if
		1+ =  if  s"  y "  else  s" , "  then
	else  0 
	then
	;
: (list_separator)  ( u1 u2 -- )  \ Añade a la cadena dinámica PRINT_STR el separador adecuado («y» o «,») para un elemento de una lista
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	1+ =  if  s" y" »&  else  s" ," »+  then
	;
: list_separator  ( u1 u2 -- )  \ Añade a la cadena dinámica PRINT_STR el separador adecuado (o ninguno) para un elemento de una lista
	\ u1 = Elementos que tiene la lista
	\ u2 = Elementos listados hasta el momento
	?dup  if  (list_separator)  else  drop  then
	;
: can_be_listed?  ( a -- ff )  \ ¿El ente puede ser incluido en las listas?
	\ Inacabado!!!
	dup protagonist% <>  \ ¿No es el protagonista?
	over is_decoration? 0=  and  \ ¿Y no es decorativo?
	swap is_global? 0=  and  \ ¿Y no es global?
	;
: /list++  ( u1 a1 a2 -- u1 | u2 )  \ Actualiza un contador si un ente es la localización de otro y puede ser listado
	\ u1 = Contador
	\ a1 = Ente que actúa como localización
	\ a2 = Ente cuya localización hay que comprobar
	\ u2 = Contador incrementado
	dup can_be_listed?
	if  location = abs +  else  2drop  then
	;
: /list  ( a -- u )  \ Cuenta el número de entes cuya localización es el ente indicado y pueden ser listados
	\ a = Ente que actúa como localización
	\ u = Número de entes localizados en el ente y que pueden ser listados
	0  \ Contador
	#entities 0  do
		over i #>entity /list++
	loop  nip
	;
: (worn)$  ( a -- a1 u1 )  \ Devuelve «(puesto/a/s)», según el género y número del ente indicado
	s" (puest" rot noun_ending s" )" s+ s+
	;
: (worn)&  ( a1 u1 a2 -- a1 u1 | a3 u3 )  \ Añade a una cadena, si es necesario, el indicador de que el ente indicado es una prenda puesta
	\ a1 u1 = Cadena con el nombre del ente
	\ a2 = Ente
	\ a3 u3 = Nombre del ente con, si es necesario, el indicador de que se trata de una prenda puesta
	dup  is_worn?  if  (worn)$ s&  else  drop  then
	;
: (content_list)  ( a -- )  \ Añade a la lista en la cadena dinámica PRINT_STR el separador y el nombre de un ente
	#elements @ #listed @  list_separator
	dup full_name rot (worn)& »&  #listed ++
	;
: about_to_list  ( a -- u )  \ Prepara el inicio de una lista
	\ a = Ente que es la localización de los entes a incluir en la lista
	\ u = Número de entes que serán listados
	#listed off  /list dup #elements !
	;
: content_list  ( a -- a1 u1 )  \ Devuelve una lista de entes localización es el ente indicado
	\ a = Ente que actúa como localización
	\ a1 u1 = Lista de objetos localizados en dicho ente
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
: .present  ( -- )  \ Lista los entes presentes
	my_location content_list dup
	if  s" Ves" s" Puedes ver" 2 schoose 2swap s& narrate
	else  2drop
	then
	;

\ }}}###########################################################
section( Herramientas para las tramas asociadas a lugares)  \ {{{

: [:location_plot]  ( a -- )  \ Inicia la definición de trama de un ente escenario
	\ Esta palabra se ejecutará al comienzo de la palabra de trama de escenario.
	\ El identificador del ente está en la pila porque se compiló con LITERAL cuando se creó la palabra de trama. 
	to self%  \ Actualizar el puntero al ente, usado para aligerar la sintaxis
	;
: (:location_plot)  ( a xt -- ) \ Operaciones preliminares para la definición de la trama de un ente escenario
	\ Esta palabra solo se ejecuta una vez para cada ente,
	\ al inicio de la compilación del código de la palabra
	\ que define su trama.
	\ a = Ente escenario para cuya trama se ha creado una palabra
	\ xt = Dirección de ejecución de la palabra recién creada
	over ~location_plot_xt !  \ Guardar el xt de la nueva palabra en la ficha del ente
	postpone literal  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que [:DESCRIPTION] lo guarde en SELF% en tiempo de ejecución
	;
: :location_plot  ( a -- xt a ) \ Crea una palabra sin nombre que manejará la trama de un ente escenario
	:noname (:location_plot)  \ Crear la palabra y hacer las operaciones preliminares
	postpone [:location_plot]  \ Compilar la palabra [:LOCATION_PLOT] en la palabra creada, para que se ejecute cuando sea llamada
	[lina?]  [IF]  !CSP  [THEN]
	;
gforth?  [IF]
	: ;location_plot  [comp'] ; postpone, postpone [  ;  immediate
[ELSE]
	' ; alias ;location_plot 
	lina? 0=  [IF]  immediate  [THEN]
[THEN]
: location_plot  ( a -- )  \ Ejecuta la palabra de trama de un ente escenario
	location_plot_xt ?dup  if  execute  then
	;
: leave_location  ( -- )  \ Tareas previas a abandonar el escenario actual
	my_location ?dup  if
		dup visits++
		protagonist% was_there
	then
	;
: enter  ( a -- )  \ Entra en un escenario
	[debug]  [IF]  s" En ENTER" debug  [THEN]  \ Depuración!!!
	leave_location
	dup my_location!
	dup describe
	dup location_plot 
	familiar++  .present
	;

\ }}}###########################################################
section( Recursos de las tramas asociadas a lugares)  \ {{{

\ ------------------------------------------------
\ Regreso a casa

: pass_still_open?  ( -- ff )  \ ¿El paso del desfiladero está abierto por el Norte?
	location_08% has_north_exit?
	;
: still_in_the_village?  ( -- ff )  \ ¿Los soldados no se han movido aún de la aldea sajona?
	my_location location_01% =
	location_02% is_not_visited? and
	;
: back_to_the_village?  ( -- ff )  \ ¿Los soldados han regresado a la aldea sajona?
	\ No se usa!!!
	my_location location_01% =
	location_02% is_visited? and
	;
: soldiers_follow_you  ( -- )  \ De vuelta a casa
	^all_your$ soldiers$ s&
	s{ s" siguen tus pasos." s" te siguen." }s&
	narrate
	;
: going_home  ( -- )  \ De vuelta a casa, si procede
	pass_still_open?
	still_in_the_village? 0= and
	if  soldiers_follow_you  then
	;
: celebrating  ( -- )  \ Celebrando la victoria
	\ Inacabado!!!
	^all_your$ soldiers$ s&
	s{ s" lo están celebrando." s" lo celebran." }s&
	narrate
	;

\ ------------------------------------------------
\ Albegue de los refugiados

: the_old_man_is_angry?  ( -- ff )  \ ¿El anciano se enfada porque llevas algo prohibido?
	stone% is_accessible?
	sword% is_accessible?  or
	;
: he_looks_at_you_with_anger$  ( -- a u )  \ Devuelve una versión del texto de que el líder de los refugiados te mira
	s" parece sorprendido y" s?
	s{
	s" te mira" s{ s" con dureza" s" con preocupación" }s&
	s" te dirige una dura mirada"
	s" dirige su mirada hacia ti"
	}s&
	;
: he_looks_at_you_with_calm$  ( -- a u )  \ Devuelve una versión del texto de que el líder de los refugiados te mira
	s" advierte tu presencia y" s?
	s{ s" por un momento" s" durante unos instantes" }s?&
	s" te" s&{ s" observa" s" contempla" }s&
	s{ s" con serenidad" s" con expresión serena" s" en calma" s" sereno" }s&
	;
: the_leader_looks_at_you$  ( -- a u )  \ Devuelve el texto de que el líder de los refugiados te mira
	leader% ^full_name  the_old_man_is_angry?
	if  he_looks_at_you_with_anger$ 
	else  he_looks_at_you_with_calm$
	then  s& period+
	;
: the_refugees_surround_you$  ( -- a u )  \ Devuelve descripción de la actitud de los refugiados
	s" Los refugiados"
	location_28% has_east_exit?
	if  they_let_you_pass$ 
	else  they_don't_let_you_pass$
	then  period+ s&
	;

\ }}}###########################################################
section( Tramas asociadas a lugares)  \ {{{

location_01% :location_plot
	soldiers% is_here
	still_in_the_village?
	if  celebrating  else  going_home  then
	;location_plot
location_02% :location_plot
	\ Decidir hacia dónde conduce la dirección hacia abajo
	[false]  [IF]  \ Primera versión, al azar
		self% location_01% location_03% 2 choose d-->  \ Decidir al azar la salida hacia abajo
	[ELSE]  \ Segunda versión, según el escenario de procedencia
		self%
		protagonist% previous_location location_01% =  \ ¿Venimos de la aldea?
		if  location_03%  else  location_01%  then  d-->
	[THEN]
	soldiers% is_here going_home
	;location_plot
location_03% :location_plot
	soldiers% is_here going_home
	;location_plot
location_04% :location_plot
	soldiers% is_here going_home
	;location_plot
location_05% :location_plot
	soldiers% is_here going_home
	;location_plot
location_06% :location_plot
	soldiers% is_here going_home
	;location_plot
location_07% :location_plot
	soldiers% is_here going_home
	;location_plot
location_08% :location_plot
	soldiers% is_here
	going_home
	;location_plot
location_09% :location_plot
	soldiers% is_here
	going_home
	;location_plot
location_11% :location_plot
	lake% is_here
	;location_plot
location_16% :location_plot
	s" En la distancia, por entre los resquicios de las rocas,"
	s" y allende el canal de agua, los sajones" s&
	s{ s" intentan" s" se esfuerzan en" s" tratan de" s" se afanan en" }s&
	s{ s" hallar" s" buscar" s" localizar" }s&
	s" la salida que encontraste por casualidad." s&
	narrate
	;location_plot
location_28% :location_plot
	self% no_exit e-->  \ Cerrar la salida hacia el Este
	refugees% is_here
	the_refugees_surround_you$ narrate
	the_leader_looks_at_you$ narrate
	;location_plot
location_28% :location_plot
	refugees% is_here  \ Para que sean visibles en la distancia
	;location_plot
location_31% :location_plot
	self% has_north_exit?  if
		s" Las rocas yacen desmoronadas a lo largo del"
		pass_way$ s& period+
	else
		s" Las rocas" (they)_block$ s& s" el paso." s&
	then  narrate
	;location_plot
location_38% :location_plot
	lake% is_here
	;location_plot
location_43% :location_plot
	snake% is_here?  if
		a_snake_blocks_the_way$ period+
		narrate
	then
	;location_plot
location_44% :location_plot
	lake% is_here
	;location_plot

\ }}}###########################################################
section( Trama global)  \ {{{

\ ------------------------------------------------
\ Miscelánea

: (lock_found)  ( -- )  \ Encontrar el candado (al mirar la puerta o al intentar abrirla)
	door% ~location @ lock% is_there
	lock% familiar++
	;  ' (lock_found) is lock_found

\ ------------------------------------------------
\ Persecución

: pursued  ( -- )  \ Perseguido por los sajones
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
: pursue_location?  ( -- ff )  \ ¿En un escenario en que los sajones pueden perseguir al protagonista?
	my_location location_12% <
	;

\ ------------------------------------------------
\ Batalla

: all_your_men  ( -- a u ff )  \ Devuelve una variante de «Todes tus hombres», y un indicador de número
	\ a u = Cadena
	\ ff = ¿El texto está en plural?
	2 random dup
	if  s{ s" Todos" s" Todos y cada uno de" }s
	else  s" Hasta el último de"
	then  your_soldiers$ s&  rot
	;
: ?plural_verb  ( a1 u1 ff -- a1 u1 | a2 u2 )  \ Pone un verbo en plural si es preciso
	if  s" n" s+  then
	;
: fight/s$  ( ff -- a u )  \ Devuelve una variante de «lucha/n»
	\ ff = ¿El resultado debe estar en plural?
	\ a u = Resultado
	s{ s" lucha" s" combate" s" pelea" s" se bate" }s
	rot ?plural_verb
	;
: resist/s$  ( ff -- a u )  \ Devuelve una variante de «resiste/n»
	\ ff = ¿El resultado debe estar en plural?
	\ a u = Resultado
	s{ s" resiste" s" aguanta" s" contiene" }s
	rot ?plural_verb
	;
: like_a_heroe$ ( -- a u )  \ Devuelve una variante de «como un héroe»
	s" como un" s" auténtico" s?&
	s{ s" héroe" s" valiente" s" jabato" }s&
	;
: like_heroes$ ( -- a u )  \ Devuelve una variante de «como héroes»
	s" como" s" auténticos" s?&
	s{ s" héroes" s" valientes" s" jabatos" }s&
	;
: (bravery)$  ( -- a u )  \ Devuelve una variante de «con denuedo»
	s{ s" con denuedo" s" con bravura" s" con coraje"
	s" heroicamente" s" esforzadamente" s" valientemente" }s
	;
: bravery$  ( ff -- a u )  \ Devuelve una variante de «con denuedo», en singular o plural
	\ ff = ¿El resultado debe estar en plural?
	\ a u = Resultado
	(bravery)$  rot
	if  like_heroes$  else  like_a_heroe$  then
	2 schoose 
	;
: step_by_step$  ( -- a u )  \ Devuelve una variante de «poco a poco»
	s{ s" por momentos" s" palmo a palmo" s" poco a poco" }s
	;
: field$  ( -- a u )  \ Devuelve «terreno» o «posiciones»
	s{ s" terreno" s" posiciones" }s
	;
: last(fp)$  ( -- a u )  \ Devuelve una variante de «últimas»
	s{ s" últimas" s" postreras" }s
	;
: last$  ( -- a u )  \ Devuelve una variante de «último»
\ Nota!!! Confirmar «postrer»
	s{ s" último" s" postrer" }s
	;
: last_energy(fp)$  ( -- a u )  \ Devuelve una variante de «últimas energías»
	last(fp)$ s{ s" energías" s" fuerzas" }s&
	;
: battle_phase_00$  ( -- a u ) \ Devuelve la descripción del combate (fase 00)
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
: battle_phase_00  ( -- )  \ Combate (fase 00)
	battle_phase_00$ narrate
	;
: battle_phase_01$  ( -- a u ) \ Devuelve la descripción del combate (fase 01)
	all_your_men  dup resist/s$  rot bravery$  s& s&
	s{ s{ s" el ataque" s" el empuje" s" la acometida" }s
	s" inicial" s&
	s" el primer" s{ s" ataque" s" empuje" }s&
	s" la primera acometida"
	}s& of_the_enemy|enemies$ s& period+ 
	;
: battle_phase_01  ( -- )  \ Combate (fase 01)
	battle_phase_01$ narrate
	;
: battle_phase_02$  ( -- a u ) \ Devuelve la descripción del combate (fase 02)
	all_your_men  dup fight/s$  rot bravery$  s& s&
	s" contra" s&  the_enemy|enemies$ s&  period+
	;
: battle_phase_02  ( -- )  \ Combate (fase 02)
	battle_phase_02$ narrate
	;
: battle_phase_03$  ( -- a u ) \ Devuelve la descripción del combate (fase 03)
	\ Inacabado!!!
	^your_soldiers$
	s" empiezan a acusar" s&
	s{ 0$ s" visiblemente" s" notoriamente" }s&
	s" el" s&{ s" titánico" s" enorme" }s?&
	s" esfuerzo." s&
	;
: battle_phase_03  ( -- )  \ Combate (fase 03)
	battle_phase_03$ narrate
	;
: battle_phase_04$  ( -- a u ) \ Devuelve la descripción del combate (fase 04)
	^the_enemy|enemies
	s" parece que empieza* a" rot *>verb_ending s&
	s{ s" dominar" s" controlar" }s&
	s{ s" el campo" s" el combate" s" la situación" s" el terreno" }s&
	period+ 
	;
: battle_phase_04  ( -- )  \ Combate (fase 04)
	battle_phase_04$ narrate
	;
: battle_phase_05$  ( -- a u ) \ Devuelve la descripción del combate (fase 05)
	\ Inacabado!!!?
	^the_enemy|enemies s{
	s" está* haciendo retroceder a" your_soldiers$ s&
	s" está* obligando a" your_soldiers$ s& s" a retroceder" s&
	}s rot *>verb_ending s&
	step_by_step$ s& period+
	;
: battle_phase_05  ( -- )  \ Combate (fase 05)
	battle_phase_05$ narrate
	;
: battle_phase_06$  ( -- a u ) \ Devuelve la descripción del combate (fase 06)
	\ Inacabado!!!
	^the_enemy|enemies s{
	s" va* ganando" field$ s&
	s" va* adueñándose del terreno"
	s" va* conquistando" field$ s&
	s" se va* abriendo paso"
	}s rot *>verb_ending s&
	step_by_step$ s& period+
	;
: battle_phase_06  ( -- )  \ Combate (fase 06)
	battle_phase_06$ narrate
	;
: battle_phase_07$  ( -- a u ) \ Devuelve la descripción del combate (fase 07)
	^your_soldiers$
	s{ s" caen" s" van cayendo," }s&
	s" uno tras otro," s?&
	s{ s" vendiendo cara su vida" s" defendiéndose" }s&
	like_heroes$ s& period+
	;
: battle_phase_07  ( -- )  \ Combate (fase 07)
	battle_phase_07$ narrate
	;
: battle_phase_08$  ( -- a u ) \ Devuelve la descripción del combate (fase 08)
	^the_enemy|enemies
	s{ s" aplasta* a" s" acaba* con" }s
	rot *>verb_ending s&
	s" los últimos de" s" entre" s?& s&
	your_soldiers$ s& s" que," s&
	s{ s" heridos y agotados" s" extenuados"
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
: battle_phase_08  ( -- )  \ Combate (fase 08)
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
: (battle_phase)  ( u -- )  \ Ejecuta una fase del combate
	\ u = Fase del combate (la primera es la cero)
	cells 'battle_phases + perform  
	;
: battle_phase  ( -- )  \ Ejecuta la fase en curso del combate
	battle# @ 1- (battle_phase)
	;
: battle_location?  ( -- ff )  \ ¿En el escenario de la batalla?
	my_location location_10% <  \ ¿Está el protagonista en un escenario menor que el 10?
	pass_still_open? 0=  and  \ ¿Y el paso del desfiladero está cerrado?
	;
: battle_phase++  ( -- )  \ Incrementar la fase de la batalla (salvo una de cada diez veces, al azar)
	10 random  if  battle# ++  then
	;
: battle  ( -- )  \ Batalla y persecución
	battle_location?  if  battle_phase  then
	pursue_location?  if  pursued  then
	battle_phase++
	;
: battle?  ( -- f)  \ ¿Ha empezado la batalla?
	battle# @ 0>
	;
: the_battle_ends  ( -- )  \ Termina la batalla
	battle# off
	;
: the_battle_begins  ( -- )  \ Comienza la batalla
	1 battle# !
	;

\ ------------------------------------------------
\ Emboscada de los sajones

: ambush?  ( -- ff )  \ ¿Ha caído el protagonista en la emboscada?
	my_location location_08% =  \ ¿Está en el escenario 8?
	pass_still_open?  and  \ ¿Y además el paso está abierto?
	;
: the_pass_is_closed  ( -- )  \ Cerrar es paso, la salida norte
	no_exit location_08% ~north_exit !
	;
: the_ambush_begins  ( -- )  \ Comienza la emboscada
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

: they_win_0$  ( -- a u )  \ Devuelve la primera versión de la parte final de las palabras de los oficiales
	s{ s" su" s{ s" victoria" s" triunfo" }s&
	s" nuestra" s{ s" derrota" s" humillación" }s&
	}s s" será" s&{ s" doble" s" mayor" }s&
	;
: they_win_1$  ( -- a u )  \ Devuelve la segunda versión de la parte final de las palabras de los oficiales
	s{ s" ganan" s" nos ganan" s" vencen"
	s" perdemos" s" nos vencen" s" nos derrotan" }s
	s{ s" doblemente" s" por partida doble" }s&
	;
: they_win$  ( -- a u )  \ Devuelve la parte final de las palabras de los oficiales
	they_win_0$ they_win_1$ 2 schoose period+
	;
: taking_prisioner$  ( -- a u )  \ Devuelve una parte de las palabras de los oficiales
	s" si" s{ s" capturan" s" hacen prisionero" s" toman prisionero" }s&
	;
: officers_speach  ( -- )  \ Palabras de los oficiales
	sire,$ s?  dup taking_prisioner$
	rot 0=  if  ^uppercase  then  s&
	s" a un general britano" s& they_win$ s&  speak
	;
: officers_talk_to_you  ( -- )  \ Tus oficiales hablan con el protagonista
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
: the_enemy_is_stronger  ( -- )  \ El enemigo es superior
	s" En el" narrow(m)$ s& s" paso es posible" s&
	s{ s" resistir," s" defenderse," }s& s" pero" s&
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
: ambush  ( -- )  \ Emboscada
	the_pass_is_closed
	the_ambush_begins
	the_battle_begins
	the_enemy_is_stronger
	officers_talk_to_you
	;

\ ------------------------------------------------
\ Zona oscura de la cueva

: dark_cave?  ( -- ff )  \ ¿Entrar en la zona oscura de cueva y sin luz?
	torch% is_not_accessible?
	torch% is_lit? 0=  or
	my_location location_20% =  and
	;
: dark_cave  ( -- )  \ En la cueva y sin luz
	new_page
	s" Ante la reinante"
	s{ s" e intimidante" s" e impenetrable" s" y sobrecogedora" }s&
	s" oscuridad," s&
	s{ s" vuelves atrás" s" retrocedes" }s&
	s{ 0$ s" unos pasos" s" sobre tus pasos" }s&
	s" hasta donde puedes ver." s&
	narrate  scene_break
	location_17% enter
	;

\ ------------------------------------------------
\ Ambrosio nos sigue

0  [IF]  \ ......................................

Pondiente!!!:

Confirmar la función de la llave aquí. En el código original
solo se distingue que sea manipulable o no, lo que es
diferente a que esté accesible.

[THEN]  \ ......................................

: ambrosio_must_follow?  ( -- )  \ ¿Ambrosio tiene que estar siguiéndonos?
	ambrosio% not_vanished?  key% is_accessible? and
	location_46% am_i_there?  ambrosio_follows? @ or  and
	;
: ambrosio_must_follow  ( -- )  \ Ambrosio tiene que estar siguiéndonos
	my_location ambrosio% is_there
	s" Tu benefactor te sigue, esperanzado." narrate	
	;

\ ------------------------------------------------
\ Gestor de la trama global

: plot  ( -- )  \ Trama global
	\ Nota: Las subtramas deben comprobarse en orden cronológico:
	ambush?  if  ambush exit  then
	battle?  if  battle exit  then
	dark_cave?  if  dark_cave  then
	ambrosio_must_follow?  if  ambrosio_must_follow  then
	;

\ }}}###########################################################
section( Descripciones especiales)  \ {{{

0  [IF]  \ ......................................

Esta sección contiene palabras que muestran descripciones
que necesitan un tratamiento especial porque hacen
uso de palabras relacionadas con la trama.

En lugar de crear vectores para las palabras que estas
descripciones utilizan, es más sencillo crearlos para las
descripciones y definirlas aquí, a continuación de la trama.

[THEN]  \ ......................................

: officers_forbid_to_steal$  ( -- )  \ Devuelve una variante de «Tus oficiales detienen el saqueo»
	s{ s" los" s" tus" }s s" oficiales" s&
	s{
	s" intentan detener" s" detienen como pueden"
	s" hacen" s{ s" todo" s? s" lo que pueden" s& s" lo imposible" }s&
		s{ s" para" s" por" }s& s" detener" s&
	}s& s{ s" el saqueo" 2dup s" el pillaje" }s&
	;
: ^officers_forbid_to_steal$  ( -- )  \ Devuelve una variante de «Tus oficiales detienen el saqueo» (con la primera mayúscula)
	officers_forbid_to_steal$ ^uppercase
	;
: (they_do_it)_their_way$  ( -- a u )
	s" ," s{
		s" a su" s{ s" manera" s" estilo" }s&
		s" de la única"
		s{ s" manera" s" forma" }s& s" que" s& s{ s" saben" s" conocen" }s&
	}s& comma+
	;
: this_sad_victory$  ( -- a u )
	s" esta" s" tan" s{ s" triste" s" fácil" s" poco honrosa" }s&
	s" victoria" r2swap s& s&
	;
: (soldiers_steal$)  ( a1 u1 -- a2 u2 )  \ Completa una descripción de tus soldados en la aldea arrasada
	soldiers$ s& s{ s" aún" s" todavía" }s?&
	s{ s" celebran" s{ s" están" s" siguen" s" continúan" }s s" celebrando" s& }s&
	(they_do_it)_their_way$ s?+	
	this_sad_victory$ s& s{ s" :" s" ..." }s+
	s{ s" saqueando" s" buscando" s" apropiándose de" s" robando" }s&
	s" todo" s?& s" cuanto de valor" s&
	s" aún" s?& s{ s" quede" s" pueda quedar" }s&
	s" entre" s& rests_of_the_village$ s&
	;
: soldiers_steal$  ( -- a u )  \ Devuelve una descripción de tus soldados en la aldea arrasada
	all_your$ (soldiers_steal$)
	;
: ^soldiers_steal$  ( -- a u )  \ Devuelve una descripción de tus soldados en la aldea arrasada (con la primera mayúscula)
	^all_your$ (soldiers_steal$)
	;
: soldiers_steal_spite_of_officers_0$  ( -- a u )  \ Devuelve la primera versión de la descripción de los soldados en la aldea
	^soldiers_steal$ period+
	^officers_forbid_to_steal$ s&
	;
: soldiers_steal_spite_of_officers_1$  ( -- a u )  \ Devuelve la segunda versión de la descripción de los soldados en la aldea
	^soldiers_steal$
	s{ s" , mientras" s" que" s?&
	s{ s" ; mientras" s" . Mientras" }s s" tanto" s?& comma+
	s" . Al mismo tiempo," }s+
	officers_forbid_to_steal$ s&
	;
: soldiers_steal_spite_of_officers_2$  ( -- a u )  \ Devuelve la tercera versión de la descripción de los soldados en la aldea
	\ No se usa!!! La frase queda incoherente en algunos casos.
	^officers_forbid_to_steal$
	s" , pero" s+ s" a pesar de ello" s?&
	soldiers_steal$ s&
	;
: soldiers_steal_spite_of_officers$  ( -- a u )  \ Devuelve una descripción de tus soldados en la aldea arrasada
	['] soldiers_steal_spite_of_officers_0$
	['] soldiers_steal_spite_of_officers_1$
	2 choose execute
	;
: soldiers_steal_spite_of_officers  ( -- )  \ Describe a tus soldados en la aldea arrasada
	soldiers_steal_spite_of_officers$ period+ paragraph
	;
: soldiers_go_home  ( -- )  \ Describe a tus soldados durante el regreso a casa
	^all_your$ soldiers$ s& s" te seguirían hasta el" s&
	s{ s{ s" mismo" s" mismísimo" }s s" infierno" s&
	s" último rincón de la Tierra"
	}s& period+  paragraph
	;
: (soldiers_description)  ( -- )  \ Describe a tus soldados
	true  case
		still_in_the_village?  of  soldiers_steal_spite_of_officers  endof
\		back_to_the_village?  of  soldiers_go_home  endof  \ No se usa!!!
		pass_still_open?  of  soldiers_go_home  endof
\		battle?  of  battle_phase  endof  \ No se usa!!! Redundante, porque tras la descripción se mostrará otra vez la situación de la batalla
	endcase
	;
' (soldiers_description) is soldiers_description

\ }}}###########################################################
section( Errores del intérprete de comandos)  \ {{{

: please$  ( -- a u )  \ Devuelve «por favor» o vacía
	s" por favor" s?
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
: in_the_sentence$  ( -- a u )  \ Devuelve una variante de «en la frase» (o una cadena vacía)
	s{ 0$ s" en la frase" s" en el comando" s" en el texto" }s
	;
: error_comment_0$  ( -- a u )  \ Devuelve la variante 0 del mensaje de acompañamiento para los errores lingüísticos
	s" sé más clar" player_gender_ending$+
	;
: error_comment_1$  ( -- a u )  \ Devuelve la variante 1 del mensaje de acompañamiento para los errores lingüísticos
	s{ s" exprésate" s" escribe" }s
	s{
	s" más claramente"
	s" más sencillamente"
	s{ s" con más" s" con mayor" }s
	s{ s" sencillez" s" claridad" }s&
	}s s&
	;
: error_comment_2$  ( -- a u )  \ Devuelve la variante 2 del mensaje de acompañamiento para los errores lingüísticos
	\ Comienzo común:
	s{ s" intenta" s" procura" s" prueba a" }s
	s{ s" reescribir" s" expresar" s" escribir" s" decir" }s&
	s{ s"  la frase" s" lo" s"  la idea" }s+
	s{
	\ Final 0:
	s{ 0$ s" de" s" otra" }s
	s{ s" forma" s" manera" }s&
	s{ 0$ s" un poco" s" algo" }s& s" más" s&
	s{ s" simple" s" sencilla" s" clara" }s&
	\ Final 1:
	s{ s" más claramente" s" con más sencillez" }s
	}s&
	;
: error_comment$  ( -- a u )  \ Devuelve mensaje de acompañamiento para los errores lingüísticos
	error_comment_0$ error_comment_1$ error_comment_2$
	3 schoose please&
	;
: ^error_comment$  ( -- a u )  \ Devuelve mensaje de acompañamiento para los errores lingüísticos, con la primera letra mayúscula
	error_comment$ ^uppercase
	;
: language_error  ( a u -- )  \ Combina un mensaje de error con un comentario común e informa de él
	\ Pendiente!!! Hacer que use coma o punto y coma, al azar
	in_the_sentence$ s&  3 random
	if  ^uppercase period+ ^error_comment$
	else  ^error_comment$ comma+ 2swap
	then  period+ s&  report
	;
: there_are$  ( -- a u )  \ Devuelve una variante de «hay» para sujeto plural, comienzo de varios errores
	s{ s" parece haber" s" se identifican" s" se reconocen" }s
	;
: there_is$  ( -- a u )  \ Devuelve una variante de «hay» para sujeto singular, comienzo de varios errores
	s{ s" parece haber" s" se identifica" s" se reconoce" }s
	;
: there_is_no$  ( -- a u )  \ Devuelve una variante de «no hay», comienzo de varios errores
	s" no se" s{ s" identifica" s" encuentra" s" reconoce" }s&
	s{ s" el" s" ningún" }s&
	;
: too_many_actions  ( -- )  \ Informa de que se ha producido un error porque hay dos verbos en el comando
	s{ there_are$ s" dos verbos" s&
	there_is$ s" más de un verbo" s&
	there_are$ s" al menos dos verbos" s&
	}s  language_error
	;
' too_many_actions constant (too_many_actions_error#)
' (too_many_actions_error#) is too_many_actions_error#
: too_many_complements  ( -- )  \ Informa de que se ha producido un error porque hay dos complementos en el comando
	\ Provisional!!!
	s{
	there_are$
	s" dos complementos indirectos o preposicionales" s&
	there_is$
	s" más de un complemento indirecto o preposicional" s&
	there_are$
	s" al menos dos complementos indirectos o preposicionales" s&
	}s  language_error
	;
' too_many_complements constant (too_many_complements_error#)
' (too_many_complements_error#) is too_many_complements_error#
: no_verb  ( -- )  \ Informa de que se ha producido un error por falta de verbo en el comando
	there_is_no$ s" verbo" s& language_error
	;
' no_verb constant (no_verb_error#)
' (no_verb_error#) is no_verb_error#
: no_main_complement  ( -- )  \ Informa de que se ha producido un error por falta de complemento directo en el comando
	there_is_no$ s" complemento directo" s& language_error
	;
' no_main_complement constant (no_main_complement_error#)
' (no_main_complement_error#) is no_main_complement_error# 
: unexpected_main_complement  ( -- )  \ Informa de que se ha producido un error por presencia de complemento directo en el comando
	there_is$ s" un complemento directo" s&
	s" pero el verbo no puede llevarlo" s&
	language_error
	;
' unexpected_main_complement constant (unexpected_main_complement_error#)
' (unexpected_main_complement_error#) is unexpected_main_complement_error# 

: ?wrong  ( xt | 0 -- )  \ Informa, si es preciso, de un error en el comando
	\ xt = Dirección de ejecución de la palabra de error (que se usa también como código del error)
	[debug_catch] [debug_parsing] [or] ?halto" Al entrar en ?WRONG"  \ Depuración!!!
	?dup  if  execute  then
	[debug_catch] [debug_parsing] [or] ?halto" Al salir de ?WRONG"  \ Depuración!!!
	;

\ }}}###########################################################
section( Herramientas para crear las acciones)  \ {{{

\ ------------------------------------------------
subsection( Pronombres)  \ {{{

\ Inacabado!!! No usado!!!

variable last_action
variable last_feminine_singular_complement
variable last_masculine_singular_complement
variable last_feminine_plural_complement
variable last_masculine_plural_complement

: init_command  ( -- )  \ Prepara los valores variables usados en los comandos
	\ Inacabado!!! No usado!!!
	last_action off
	\ last_feminine_singular_complement off
	\ last_masculine_singular_complement off
	\ last_feminine_plural_complement off
	\ last_masculine_plural_complement off
	;

\ }}}---------------------------------------------
subsection( Herramientas para la creación de acciones)  \ {{{

0  [IF]  \ ......................................

Los nombres de las acciones empiezan por el prefijo «do_»
(algunas palabras secundarias de las acciones 
también usan el mismo prefijo).

Pendiente!!! explicación sobre la sintaxis

[THEN]  \ ......................................

: action:  ( "name" -- )  \ Crear un identificador de acción
	\ "name" = nombre del identificador de la acción, en el flujo de entrada
	create  \ Crea una palabra con el nombre indicado...
		['] noop ,  \ ...y guarda en su campo de datos (pfa) la dirección de ejecución de NOOP
	does>  ( pfa -- )  \ Cuando la palabra sea llamada tendrá su pfa en la pila...
		perform  \ ...ejecutará la dirección de ejecución que tenga guardada
	;
: :action  ( "name" -- )  \ Inicia la definición de una palabra que ejecutará una acción
	\ "name" = nombre del identificador de la acción, en el flujo de entrada
	:noname  ( xt )  \ Crea una palabra para la acción
	' >body !  \ Guarda su dirección de ejecución en el campo de datos del identificador de la acción
	[lina?]  [IF]  !CSP  [THEN]
	;
gforth?  [IF]
	: ;action  [comp'] ; postpone, postpone [  ;  immediate
[ELSE]
	' ; alias ;action 
	lina? 0=  [IF]  immediate  [THEN]
[THEN]

\ }}}---------------------------------------------
subsection( Comprobación de los requisitos de las acciones)  \ {{{

0  [IF]  \ ......................................

En las siguientes palabras usamos las llaves en sus nombres
como una notación, para hacer más legible y más fácil de
modificar el código.  El texto entre las llaves indica la
condición que se ha de cumplir.

Si la condición no se cumple, se provocará un error con
THROW que devolverá el flujo al último CATCH .

Este sistema de filtros y errores permite simplificar el
código de las acciones porque ahorra muchas estructuras
condicionales anidadas.

[THEN]  \ ......................................

: main_complement{forbidden} \ Provoca un error si hay complemento directo
	main_complement @
	0<> unexpected_main_complement_error# and throw
	;
: main_complement{required}  ( -- )  \ Provoca un error si no hay complemento directo
	main_complement @
	[false] ?halto" main_complement{required} 1"
	0= no_main_complement_error# and throw
	[false] ?halto" main_complement{required} 2"
	;
: {hold}  ( a -- )  \ Provoca un error si un ente no está en inventario
	dup what !
	is_hold? 0= you_do_not_have_what_error# and throw
	;
: ?{hold}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es no está en inventario
	?dup  if  {hold}  then
	;
: main_complement{hold}  ( -- )  \ Provoca un error si el complemento directo existe y no está en inventario
	main_complement @ ?{hold}
	;
: tool_complement{hold}  ( -- )  \ Provoca un error si el complemento instrumental existe y no está en inventario
	tool_complement @ ?{hold}
	;
: {not_hold}  ( a -- )  \ Provoca un error si un ente está en inventario
	dup what !
	is_hold? you_already_have_what_error# and throw
	;
: ?{not_hold}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y está en inventario
	?dup  if  {not_hold}  then
	;
: main_complement{not_hold}  ( -- )  \ Provoca un error si el complemento directo existe y está en inventario
	main_complement @ ?{not_hold}
	;
: {worn}  ( a -- )  \ Provoca un error si un ente no lo llevamos puesto
	dup what !
	is_worn_by_me? 0= you_do_not_wear_what_error# and throw
	;
: ?{worn}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no lo llevamos puesto
	?dup  if  {worn}  then
	;
: main_complement{worn}  ( -- )  \ Provoca un error si el complemento directo existe y no lo llevamos puesto
	main_complement @ ?{worn}
	;
: {open}  ( a -- )  \ Provoca un error si un ente no está abierto
	dup what !
	is_closed? what_is_already_closed_error# and throw
	;
: {closed}  ( a -- )  \ Provoca un error si un ente no está cerrado
	dup what !
	is_open? what_is_already_open_error# and throw
	;
: {not_worn}  ( a -- )  \ Provoca un error si un ente lo llevamos puesto
	dup what !
	is_worn_by_me? you_already_wear_what_error# and throw
	;
: ?{not_worn}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y lo llevamos puesto
	?dup  if  {not_worn}  then
	;
: main_complement{not_worn}  ( -- )  \ Provoca un error si el complemento directo existe y lo llevamos puesto
	main_complement @ ?{not_worn}
	;
: {cloth}  ( a -- )  \ Provoca un error si un ente no se puede llevar puesto
	is_cloth? 0= nonsense_error# and throw
	;
: ?{cloth}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no se puede llevar puesto
	?dup  if  {cloth}  then
	;
: main_complement{cloth}  ( -- )  \ Provoca un error si el complemento directo existe y no se puede llevar puesto
	main_complement @ ?{cloth}
	;
: {here}  ( a -- )  \ Provoca un error si un ente no está presente
	dup what !
	is_here? 0= is_not_here_what_error# and throw
	;
: ?{here}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no está presente
	?dup  if  {here}  then
	;
: main_complement{here}  ( -- )  \ Provoca un error si el complemento directo existe y no está presente
	main_complement @ ?{here}
	;
: {accessible}  ( a -- )  \ Provoca un error si un ente no está accessible
	[false] ?halto" {accessible} 1"
	dup what !
	is_not_accessible?
	[false] ?halto" {accessible} 1a"
	cannot_see_what_error# and
	[false] ?halto" {accessible} 1b"
	throw
	[false] ?halto" {accessible} 2"
	;
: ?{accessible}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no está accessible
	[false] ?halto" ?{accessible} 1"
	?dup  if  {accessible}  then
	[false] ?halto" ?{accessible} 1"
	;
: main_complement{accessible}  ( -- )  \ Provoca un error si el complemento directo existe y no está accessible
	[false] ?halto" main_complement{accessible} 1"
	main_complement @ ?{accessible}
	[false] ?halto" main_complement{accessible} 2"
	;
: {taken}  ( a -- )  \ Provoca un error si un ente no puede ser tomado
	\ Nota: los errores apuntados por el campo ~TAKE_ERROR# no reciben parámetros salvo en WHAT
	dup what !
	~take_error# @ throw  \ Error específico del ente
	can_be_taken? 0= nonsense_error# and throw  \ Condición general de error
	;
: ?{taken}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no puede ser tomado
	?dup  if  {taken}  then
	;
: main_complement{taken}  ( -- )  \ Provoca un error si el complemento directo existe y no puede ser tomado
	main_complement @ ?{taken}
	;
: {broken}  ( a -- )  \ Provoca un error si un ente no puede ser roto
	\ Nota: los errores apuntados por el campo ~BREAK_ERROR# no reciben parámetros salvo en WHAT
	dup what ! ~break_error# @ throw
	;
: ?{broken}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no puede ser roto
	?dup  if  {broken}  then
	;
: main_complement{broken}  ( -- )  \ Provoca un error si el complemento directo existe y no puede ser roto
	main_complement @ ?{broken}
	;
: {looked}  ( a -- )  \ Provoca un error si un ente no puede ser mirado
	\ Nota: los errores apuntados por el campo ~TAKE_ERROR# no deben necesitar parámetros, o esperarlo en WHAT
	dup what !
	can_be_looked_at? 0= cannot_see_what_error# and throw
	;
: ?{looked}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no puede ser mirado
	?dup  if  {looked}  then
	;
: main_complement{looked}  ( -- )  \ Provoca un error si el complemento directo existe y no puede ser mirado
	main_complement @ ?{looked}
	;
: {living}  ( a -- )  \ Provoca un error si un ente no es un ser vivo
	[false] ?halto" {living} 1"
	is_living_being? 0= nonsense_error# and throw
	[false] ?halto" {living} 2"
	;
: ?{living}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no es un ser vivo
	[false] ?halto" ?{living} 1"
	?dup  if  {living}  then
	[false] ?halto" ?{living} 2"
	;
: main_complement{living}  ( -- )  \ Provoca un error si el complemento directo existe y no es un ser vivo
	[false] ?halto" main_complement{living} 1"
	main_complement @ ?{living}
	[false] ?halto" main_complement{living} 2"
	;
: {needed}  ( a -- )  \ Provoca un error si un ente no está en inventario, pues es necesario
	dup what !
	is_hold? 0= you_need_what_error# and throw
	;
: ?{needed}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no está en inventario, pues es necesario
	?dup  if  {needed}  then
	;
: main_complement{needed}  ( -- )  \ Provoca un error si el complemento directo existe y no está en inventario, pues lo necesitamos
	main_complement @ ?{needed}
	;
: {direction}  ( a -- )  \ Provoca un error si un ente no es una dirección
	dup what !
	is_direction? 0= nonsense_error# and throw
	;
: ?{direction}  ( a | 0 -- )  \ Provoca un error si un supuesto ente lo es y no es una dirección
	?dup  if  {direction}  then
	;
: main_complement{direction}  ( -- )  \ Provoca un error si el complemento directo existe y no es una dirección
	main_complement @ ?{direction}
	;


\ }}}
\ }}}###########################################################
section( Acciones)  \ {{{


0  [IF]  \ ......................................

Para crear una acción, primero es necesario crear su
identificador con la palabra ACTION: , que funciona de forma
parecida a DEFER . Después hay que definir la palabra de la
acción con las palabras previstas para ello, que se ocupan
de darle al identificador el valor de ejecución
correspondiente. Ejemplo de la sintaxis:

ACTION: identificador

:ACTION identificador
	( definición de la acción )
	;ACTION

Todos los identificadores deben ser creados antes de las
definiciones, pues su objetivo es posibilitar que las
acciones se llamen unas a otras sin importar el orden en que
estén definidas en el código fuente.

[THEN]  \ ......................................

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
subsection( Herramientas)  \ {{{

: whom  ( -- a | 0 )  \ Devuelve un ente personaje al que probablemente se refiera un comando
	\ Se usa para averiguar el objeto de algunas acciones cuando el jugador no lo especifica
	true case
		ambrosio% is_here?  of  ambrosio%  endof
		leader% is_here?  of  leader%  endof
		false swap
	endcase
	;
: unknown_whom  ( -- a | 0 )  \ Devuelve un ente personaje desconocido al que probablemente se refiera un comando
	\ Se usa para averiguar el objeto de algunas acciones cuando el jugador no lo especifica
	true case
		ambrosio% is_here_and_unknown?  of  ambrosio%  endof
		leader% is_here_and_unknown?  of  leader%  endof
		false swap
	endcase
	;

\ }}}---------------------------------------------
subsection( Mirar, examinar y registrar)  \ {{{

: (do_look)  ( a -- )  \ Mira un ente
	dup describe
	dup is_location?  if  .present  then
	familiar++ 
	;
:action do_look  ( -- )  \  Acción de mirar
	main_complement @ ?dup 0=  if  my_location  then
	dup {looked}  (do_look)
	;action
:action do_look_yourself  ( -- )  \  Acción de mirarse
	main_complement @ ?dup 0=  if  protagonist%  then
	(do_look)
	;action
\ Pendiente!!! traducir «otear»
:action do_look_to_direction  ( -- )  \  Acción de otear
	main_complement{required}
	main_complement{direction}
	main_complement @ (do_look)
	;action
:action do_examine  ( -- )  \ Acción de examinar
	\ Provisional!!!
	do_look
	;action
:action do_search  ( -- )  \ Acción de registrar
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
: no_exit$  ( -- a u )  \ Devuelve mensaje usado cuando no hay salidas que listar
	s" No hay"
	s{ s" salidas" s" salida" s" ninguna salida" }s&
	;
: go_out$  ( -- a u )
	s{ s" salir" s" seguir" }s
	;
: go_out_to& ( a u -- a1 u1 )
	go_out$ s& s" hacia" s&
	;
: one_exit_only$  ( -- a u )  \ Devuelve mensaje usado cuando solo hay una salidas que listar
	s{
	s" La única salida" possible1$ s& s" es" s& s" hacia" s?&
	^only$ s" hay salida" s& possible1$ s& s" hacia" s&
	^only$ s" es posible" s& go_out_to&
	^only$ s" se puede" s& go_out_to&
	}s
	;
: several_exits$  ( -- a u )  \ Devuelve mensaje usado cuando hay varias salidas que listar
	s{
	s" Hay salidas" possible2$ s& s" hacia" s&
	s" Las salidas" possible2$ s& s" son" s&
	}s
	;
: .exits  ( -- )  \ Imprime las salidas posibles
	#listed @  case
		0  of  no_exit$  endof
		1  of  one_exit_only$  endof
		several_exits$ rot
	endcase
	«& «»@ period+ narrate
	;
: exit_separator$  ( -- a u )  \ Devuelve el separador adecuado a la salida actual
	#free_exits @ #listed @ list_separator$
	;
: (do_exit)  ( u -- )  \ Lista una salida
	\ u = Puntero a un campo de dirección (desplazamiento relativo desde el inicio de la ficha)
	[debug_do_exits]  [IF]  cr ." (do_exit)" cr .stack  [THEN]  \ Depuración!!!
	exit_separator$ »+
	exits_table@ full_name »+
	#listed ++
	[debug_do_exits]  [IF]  cr .stack  [THEN]  \ Depuración!!!
	;

false  [IF]  \ Primera versión: las salidas se listan siempre en el mismo orden (en el que están definidas en las fichas)

: free_exits  ( a -- u )  \ Devuelve el número de salidas posibles de un ente
	[debug_do_exits]  [IF]  cr ." free_exits" cr .stack  [THEN]  \ Depuración!!!
	0 swap
	~first_exit /exits bounds  do
\		[debug_do_exits]  [IF]  i i cr . @ .  [THEN]  \ Depuración!!!
		i @ 0<> abs +
	cell  +loop 
	[debug_do_exits]  [IF]  cr .stack  [THEN]  \ Depuración!!!
	;
:action (do_exits)  ( -- )  \ Lista las salidas posibles de la localización del protagonista
	«»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
	#listed off
	my_location dup free_exits #free_exits !
	last_exit> 1+ first_exit>  do
\		[debug_do_exits]  ?[false] ?halto" do_exits 1"  \ Depuración!!!
		dup i + @
\		[debug_do_exits]  ?[false] ?halto" do_exits 2"  \ Depuración!!!
		if  i (do_exit)  then
	cell  +loop  drop
	.exits
	;action

[ELSE]  \ Segunda versión: las salidas se muestran cada vez en orden aleatorio

0 value this_location  \ Guardará el ente del que queremos calcular las salidas libres (para simplificar el manejo de la pila en el bucle)
: free_exits  ( a0 -- a1..au u )  \ Devuelve el número de salidas posibles de un ente
	\ a0 = Ente
	\ a1..au = Entes de salida del ente a0
	\ u = número de entes de salida del ente a0
	[debug_do_exits]  [IF]  cr ." free_exits" cr .stack  [THEN]  \ Depuración!!!
	to this_location  depth >r
	last_exit> 1+ first_exit>  do
		this_location i + @  if  i  then
	cell  +loop 
	depth r> -
	[debug_do_exits]  [IF]  cr .stack  [THEN]  \ Depuración!!!
	;
:action do_exits  ( -- )  \ Lista las salidas posibles de la localización del protagonista
	«»-clear  \ Borrar la cadena dinámica de impresión, que servirá para guardar la lista de salidas.
	#listed off
	my_location free_exits
	dup >r unsort r>  dup #free_exits !
	0 do  (do_exit)  loop  .exits
	;action

[THEN]

\ }}}---------------------------------------------
subsection( Ponerse y quitarse prendas)  \ {{{

: (do_put_on)  ( a -- )  \ Ponerse una prenda
	is_worn  well_done
	;
:action do_put_on  ( -- )  \ Acción de ponerse una prenda
	\ Pendiente!!! Hacer que tome la prenda si no la tiene
	main_complement{required}
	main_complement{cloth}
	main_complement{not_worn}
	main_complement{hold}
	main_complement @ (do_put_on)
	;action
: (do_take_off)  ( a -- )  \ Quitarse una prenda
	~is_worn? off  well_done
	;
:action do_take_off  ( -- )  \ Acción de quitarse una prenda
	main_complement{required}
	main_complement{worn}
	main_complement @ (do_take_off)
	;action

\ }}}---------------------------------------------
subsection( Tomar y dejar)  \ {{{

\ Antiguo!!! Puede que aún sirva:
\ : cannot_take_the_altar  \ No se puede tomar el altar
\ 	s" [el altar no se toca]" narrate  \ tmp!!!
\ 	impossible
\ 	;
\ : cannot_take_the_flags  \ No se puede tomar las banderas
\ 	s" [las banderas no se tocan]" narrate  \ tmp!!!
\ 	nonsense
\ 	;
\ : cannot_take_the_idol  \ No se puede tomar el ídolo
\ 	s" [el ídolo no se toca]" narrate  \ tmp!!!
\ 	impossible
\ 	;
\ : cannot_take_the_door  \ No se puede tomar la puerta
\ 	s" [la puerta no se toca]" narrate  \ tmp!!!
\ 	impossible
\ 	;
\ : cannot_take_the_fallen_away  \ No se puede tomar el derrumbe
\ 	s" [el derrumbe no se toca]" narrate  \ tmp!!!
\ 	nonsense
\ 	;
\ : cannot_take_the_snake  \ No se puede tomar la serpiente
\ 	s" [la serpiente no se toca]" narrate  \ tmp!!!
\ 	dangerous
\ 	;
\ : cannot_take_the_lake  \ No se puede tomar el lago
\ 	s" [el lago no se toca]" narrate  \ tmp!!!
\ 	nonsense
\ 	;
\ : cannot_take_the_lock  \ No se puede tomar el candado
\ 	s" [el candado no se toca]" narrate  \ tmp!!!
\ 	impossible
\ 	;
\ : cannot_take_the_water_fall  \ No se puede tomar la cascada
\ 	s" [la cascada no se toca]" narrate  \ tmp!!!
\ 	nonsense
\ 	;
: (do_take)  ( a -- )  \ Toma un ente
	dup is_hold familiar++ well_done
	;
:action do_take  ( -- )  \ Acción de coger
	main_complement{required}
	main_complement{not_hold}
	main_complement{here}
	main_complement{taken}
	main_complement @ (do_take)
	;action
: (do_drop)  ( a -- )  \ Deja un ente 
	dup ~is_worn? off  is_here  well_done
	;
:action do_drop  ( -- )  \ Acción de dejar
	main_complement{required}
	main_complement{hold}
	main_complement @ (do_drop)
	;action
:action do_take|do_eat  ( -- )  \ Acción de desambiguación
\ Pendiente!!!
	do_take
	;action

\ }}}---------------------------------------------
subsection( Cerrar y abrir)  \ {{{

\ Inacabado!!!
\ Falta terminar cosas, como la lógica de la combinación
\ de estados de puerta y candado.

: (do_close)  ( a -- )  \ Cerrar un ente
	~is_open? off
	;
: close_the_door  ( -- )  \ Cerrar la puerta, si es posible
	door% {open}
	key% {hold}
	door% (do_close)  well_done
	;
: close_the_lock  ( -- )  \ Cerrar el candado, si es posible
	lock% {open}
	key% {hold}
	lock% (do_close)  well_done
	;
: close_it  ( a -- )  \ Cerrar un ente, si es posible
	case
		door%  of  close_the_door  endof
		lock%  of  close_the_lock  endof
		nonsense
	endcase
	;
:action do_close  ( -- )  \ Acción de cerrar
	main_complement{required}
	main_complement{accessible}
	main_complement @ close_it
	;action
: (do_open)  ( a -- )  \ Abrir un ente
	s" (do_open)" halto  \ Depuración!!!
	is_open
	;
: the_door_is_locked  ( -- )  \ Informa de que la puerta está cerrada por el candado
	lock% ^full_name s" bloquea la puerta." s&
	narrate
	lock_found
	;
: unlock_the_door  ( -- )  \ Abrir la puerta candada, si es posible
	the_door_is_locked
	key% {needed}
	lock% (do_open)
	;
: open_the_door  ( -- )  \ Abrir la puerta, si es posible
	door% {closed}
	lock% is_closed?  if  unlock_the_door  then
	door% (do_open)
	;
: open_the_lock  ( -- )  \ Abrir el candado, si es posible
	lock% {closed}
	key% {needed}
	lock% (do_open)
	;
: open_it  ( a -- )  \ Abrir un ente, si es posible
	case
		door%  of  open_the_door  endof
		lock%  of  open_the_lock  endof
		nonsense
	endcase
	;
:action do_open  ( -- )  \ Acción de abrir
	s" do_open" halto  \ Depuración!!!
	main_complement{required}
	main_complement{accessible}
	main_complement @ open_it
	;action
0 [IF]  \ Pendiente!!!
: open_it  ( a -- )  \ Abrir un ente, si es posible
	case
		door%  of  open_the_door  endof
		lock%  of  open_the_lock  endof
		nonsense
	endcase
	;
:action do_lock  ( -- )  \ Acción de candar
	main_complement{required}
	main_complement{accessible}
	main_complement{unlocked}
	main_complement @ lock_it
	;
[THEN]

\ }}}---------------------------------------------
subsection( Agredir)  \ {{{

: the_snake_runs_away  ( -- )  \ La serpiente huye
	s{ s" Sorprendida por" s" Ante" }s
	s" los amenazadores tajos," s&
	s" la serpiente" s&
	s{
	s" huye" s" se aleja" s" se esconde"
	s" se da a la fuga" s" se quita de enmedio"
	s" se aparta" s" escapa"
	}s&
	s{ 0$ s" asustada" s" atemorizada" }s&
	narrate
	;
: attack_the_snake  ( -- )  \ Atacar la serpiente
	sword% {needed}
	the_snake_runs_away
	snake% vanish
	;
: attack_ambrosio  ( -- )  \ Atacar a Ambrosio
	no_reason
	;
: attack_leader  ( -- )  \ Atacar al jefe
	no_reason
	;
: (do_attack)  ( a -- )  \ Atacar un ser vivo
	case
		snake%  of  attack_the_snake  endof
		ambrosio%  of  attack_ambrosio  endof
		leader%  of  attack_leader  endof
		do_not_worry
	endcase
	;
:action do_attack  ( -- )  \ Acción de atacar
	main_complement{required}
	main_complement{accessible}
	main_complement{living} \ Pendiente!!! También es posible atacar otras cosas, como la ciudad u otros lugares, o el enemigo!!!
	tool_complement{hold}
	main_complement @ (do_attack)
	;action
:action do_fear  ( -- )  \ Acción de asustar
	\ Pendiente!!! Distinguir de las demás en grado o requisitos
	main_complement{required}
	main_complement{accessible}
	main_complement{living}
	tool_complement{hold}
	main_complement @ (do_attack)
	;action
: kill_the_snake  ( -- )  \ Matar la serpiente
	sword% {needed}
	the_snake_runs_away
	snake% vanish
	;
: kill_ambrosio  ( -- )  \ Matar a Ambrosio
	no_reason
	;
: kill_leader  ( -- )  \ Matar al jefe
	no_reason
	;
: kill_your_soldiers  ( -- )  \ Matar a tus hombres
	no_reason
	;
: (do_kill)  ( a -- )  \ Matar un ser vivo
	case
		snake%  of  kill_the_snake  endof
		ambrosio%  of  kill_ambrosio  endof
		leader%  of  kill_leader  endof
		soldiers%  of  kill_your_soldiers  endof
		do_not_worry
	endcase
	;
:action do_kill  ( -- )  \ Acción de matar
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
: break_the_cloak  ( -- )  \ Romper la capa
	;
: (do_break)  ( a -- )  \ Romper un ente
	case
		snake%  of  kill_the_snake  endof  \ Provisional!!!
		cloak%  of  break_the_cloak  endof
		do_not_worry
	endcase
	;
:action do_break  ( -- )  \ Acción de romper
	main_complement{required}
	main_complement{accessible}
	main_complement{broken}
	tool_complement{hold}
	main_complement @ (do_break)
	;action
:action do_hit  ( -- )  \ Acción de golpear
	s" golpear"  main_complement+is_nonsense
	;action
: can_be_sharpened?  ( a -- ff )  \ ¿Puede un ente ser afilado?
	\ Pendiente!!! Mover esta palabra junto con los demás seudo-campos
	dup log% =  swap sword% =  or  \ ¿Es el tronco o la espada?
	;
: log_already_sharpened$  ( -- a u ) \ Devuulve una cadena con una variante de «Ya está afilado»
	s" Ya" s{
	s" lo afilaste antes"
	s" está afilado de antes"
	s" tiene una buena punta"
	s" quedó antes bien afilado"
	}s&
	;
: no_need_to_do_it_again$  ( -- a u )  \ Devuelve una variante de «no hace falta hacerlo otra vez»
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
	}s s{
	s" hacerlo"
	s" volver a hacerlo"
	s" repetirlo"
	}s& again$ s&
	;
: ^no_need_to_do_it_again$  ( -- a u )  \ Devuelve una variante de «No hace falta hacerlo otra vez»
	no_need_to_do_it_again$ ^uppercase
	;
: log_already_sharpened_0$  ( -- a u )  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 0)
	log_already_sharpened$ ^uppercase period+
	^no_need_to_do_it_again$ period+ s&
	;
: log_already_sharpened_1$  ( -- a u )  \ Devuelve mensaje de que el tronco ya estaba afilado (variante 1)
	^no_need_to_do_it_again$ period+ s&
	log_already_sharpened$ ^uppercase period+ s&
	;
: log_already_sharpened  ( -- )  \ Informa de que el tronco ya estaba afilado
	['] log_already_sharpened_0$
	['] log_already_sharpened_1$
	2 choose execute  narrate
	;
: sharpen_the_log  ( -- )  \ Afila el tronco
	\ Inacabado!!! Distinguir herramientas
	hacked_the_log? @
	if  log_alreadY_sharpened
	else  hacked_the_log? on  well_done
	then
	;
: sharpen_the_sword  ( -- )  \ Afila la espada
	\ Inacabado!!!
	;
: (do_sharpen)  ( a -- )  \ Afila un ente que puede ser afilado
	case
		sword%  of  sharpen_the_sword  endof
		log%  of  sharpen_the_log  endof
	endcase
	;
:action do_sharpen  ( -- )  \ Acción de afilar
	main_complement{required}
	main_complement{accessible}
	main_complement @ can_be_sharpened?
	if  main_complement @ (do_sharpen)
	else  nonsense
	then
	;action

\ }}}---------------------------------------------
subsection( Movimiento)  \ {{{

: toward_that_direction  ( a -- a2 u2 )  \ Devuelve «al/hacia la dirección indicada»
	\ a = Ente dirección
	dup >r  has_no_article?
	if  \ no debe llevar artículo
		s" hacia" r> full_name 
	else  \ debe llevar artículo
		toward_the(m)$ r> ^name
	then  s&
	;
: impossible_move  ( a -- )  \ El movimiento es imposible
	\ a = Ente dirección
	\ Inacabado!!! Añadir una tercera variante «ir en esa dirección»; y otras específicas como «no es posible subir»
	^is_impossible$ s" ir" s&  rot
	3 random 
	if  toward_that_direction
	else  drop that_way$
	then  s& period+ narrate
	;
: do_go_if_possible  ( a -- )  \ Comprueba si el movimiento es posible y lo efectúa
	\ a = Ente supuestamente de tipo dirección
	[debug]  [IF]  s" Al entrar en DO_GO_IF_POSSIBLE" debug  [THEN]  \ Depuración!!!
	dup ~direction @ ?dup  if  \ ¿El ente es una dirección?
		my_location + @ ?dup
		if  nip enter  else  impossible_move  then
	else  drop nonsense
	then
	[debug]  [IF]  s" Al salir de DO_GO_IF_POSSIBLE" debug  [THEN]  \ Depuración!!!
	;
: simply_do_go  ( -- )  \ Ir sin dirección específica
	\ Inacabado!!!
	s" Ir sin rumbo...?" narrate
	;
:action do_go  ( -- )  \ Acción de ir
	[debug]  [IF]  s" Al entrar en DO_GO" debug  [THEN]  \ Depuración!!!
	main_complement @ ?dup
	if  do_go_if_possible
	else  simply_do_go
	then
	[debug]  [IF]  s" Al salir de DO_GO" debug  [THEN]  \ Depuración!!!
	;action
:action do_go_north  ( -- )  \ Acción de ir al Norte
	main_complement{forbidden}
	north% do_go_if_possible
	;action
:action do_go_south  ( -- )  \ Acción de ir al Sur
	[debug_catch]  [IF]  s" Al entrar en DO_GO_SOUTH" debug  [THEN]  \ Depuración!!!
	main_complement{forbidden}
	south% do_go_if_possible
	[debug_catch]  [IF]  s" Al salir de DO_GO_SOUTH" debug  [THEN]  \ Depuración!!!
	;action
:action do_go_east  ( -- )  \ Acción de ir al Este
	main_complement{forbidden}
	east% do_go_if_possible
	;action
:action do_go_west  ( -- )  \ Acción de ir al Oeste
	main_complement{forbidden}
	west% do_go_if_possible
	;action
:action do_go_up  ( -- )  \ Acción de ir hacia arriba
	main_complement{forbidden}
	up% do_go_if_possible
	;action
:action do_go_down  ( -- )  \ Acción de ir hacia abajo
	main_complement{forbidden}
	down% do_go_if_possible
	;action
:action do_go_out  ( -- )  \ Acción de ir hacia fuera
	main_complement{forbidden}
	s" voy fuera" narrate \ tmp!!!
	;action
:action do_go_in  ( -- )  \ Acción de ir hacia dentro
	main_complement{forbidden}
	s" voy dentro" narrate \ tmp!!!
	;action
:action do_go_back  ( -- )  \ Acción de ir hacia atrás
	main_complement{forbidden}
	s" voy atrás" narrate \ tmp!!!
	;action
:action do_go_ahead  ( -- )  \ Acción de ir hacia delante
	main_complement{forbidden}
	s" voy alante" narrate \ tmp!!!
	;action

\ }}}---------------------------------------------
subsection( Partir [desambiguación])  \ {{{
:action do_go|do_break  ( -- )  \ Acción de partir (desambiguar: romper o marchar)
	main_complement @ 0=
	if
		tool_complement @
		if do_break  \ Solo con herramienta, suponemos que es «romper»
		else  simply_do_go  \ Sin complementos, suponemos que es «marchar»
		then
	else
		main_complement @ is_direction?
		if  do_go  else  do_break  then 
	then
	;action
\ }}}---------------------------------------------
subsection( Nadar)  \ {{{

: in_a_different_place$  ( -- a u )  \ Devuelve una variante de «en un lugar diferente»
	s" en un" s& place$
	s{ s" desconocido" s" nuevo" s" diferente" }s&
	s" en otra parte"
	s" en otro lugar"
	3 schoose
	;
: you_emerge$  ( -- a u )  \ Devuelve mensaje sobre la salida a la superficie
	s{ s" Consigues" s" Logras" }s
	s{ s" emerger," s" salir a la superficie," }s&
	though$ s& in_a_different_place$ s&
	s" de la" s& cave$ s& s" ..." s&
	;
: swiming$  ( -- a u )  \ Devuelve mensaje sobre el buceo
	s" Buceas" s{ s" pensando en" s" deseando"
	s" con la esperanza de" s" con la intención de" }s&
	s{ s" avanzar," s" huir," s" escapar,"  s" salir," }s&
	s" aunque" s&{ s" perdido." s" desorientado." }s&
	;
: drop_the_cuirasse$  ( ff -- a u )  \ Devuelve mensaje sobre deshacerse de la coraza dentro del agua
	\ ff = ¿Inicio de frase?
	s{ s" te desprendes de ella" s" te deshaces de ella"
	s" la dejas caer" s" la sueltas" }s
	rot  if  \ ¿Inicio de frase?
		s{ s" Rápidamente" s" Sin dilación"
		s" Sin dudarlo" s{ 0$ s" un momento" s" un instante" }s&
		}s 2swap s&
	then  period+
	;
: you_leave_the_cuirasse$  ( -- a u )  \ Devuelve mensaje sobre quitarse y soltar la coraza dentro del agua
	cuirasse% is_worn_by_me?  \ ¿La llevamos puesta?
	if
		s{ s" Como puedes," s" No sin dificultad," }s
		s{ s" logras quitártela" s" te la quitas" }s&
		s" y" s& false drop_the_cuirasse$ s&
	else
		true drop_the_cuirasse$
	then
	;
: (you_sink_1)$ ( -- a u )  \ Devuelve la primera versión del mensaje sobre hundirse con la coraza
	s{ s" Caes" s" Te hundes"
	s" Empiezas a hundirte" s" Empiezas a caer"
	}s s" sin remedio" s?&
	s{ s" hacia el fondo" s" hacia las profundidades" }s&
	s{ s" por el" s" debido al" }s&
	s" peso de tu coraza" s&
	;
: (you_sink_2)$ ( -- a u )  \ Devuelve la segunda versión del mensaje sobre hundirse con la coraza
	s" El peso de tu coraza"
	s{ s" te arrastra" s" tira de ti" }s&
	s{ 0$ s" sin remedio" s" con fuerza" }s&
	s{
	s" hacia el fondo"
	s" hacia las profundidaes" 
	s" hacia abajo"
	}s&
	;
: you_sink$ ( -- a u )  \ Devuelve mensaje sobre hundirse con la coraza
	2 random
	if  (you_sink_1)$
	else  (you_sink_2)$
	then  period+
	;
: you_swim_with_cuirasse$  ( -- a u )  \  Devuelve mensaje inicial sobre nadar con coraza
	you_sink$ you_leave_the_cuirasse$ s&
	;
: you_swim$  ( -- a u )  \  Devuelve mensaje sobre nadar
	cuirasse% is_hold? 
	if  you_swim_with_cuirasse$  cuirasse% vanish
	else  0$
	then  swiming$ s&
	;
:action do_swim  ( -- )  \ Acción de nadar
	my_location location_11% =  if
		clear_screen_for_location
		you_swim$ narrate narration_break
		you_emerge$ narrate narration_break
		location_12% enter  the_battle_ends
	else
		s" nadar" now|here$ s& is_nonsense
	then
	;action

\ }}}---------------------------------------------
subsection( Escalar)  \ {{{

: do_climb_if_possible  ( a -- )  \ Escalar el ente indicado si es posible
	\ Inacabado!!!
	dup is_here?  if  dup s" [escalar]" narrate
	else  drop s" [no está aquí]" narrate
	then
	;
:action do_climb  ( -- )  \ Acción de escalar
	\ Inacabado!!!
	main_complement @ ?dup
	if  do_climb_if_possible
	else  no_main_complement  \ Inacabado!!! Comprobar si es el derrumbe u otra cosa.
	then
	;action
 
\ }}}---------------------------------------------
subsection( Inventario)  \ {{{

: anything_with_you$  ( -- a u )  \ Devuelve una variante de «nada contigo»
	s" nada" with_you$  ?dup  if
		2 random  if  2swap  then  s&
	else drop
	then
	;
: you_are_carrying_nothing$  ( -- a u )  \ Devuelve mensaje para sustituir a un inventario vacío
	s" No" you_carry$ anything_with_you$ period+ s& s& 
	;
: ^you_are_carrying$  ( -- a u )  \ Devuelve mensaje para encabezar la lista de inventario
	^you_carry$ with_you$ s& 
	;
: you_are_carrying$  ( -- a u )  \ Devuelve mensaje para encabezar la lista de inventario
	you_carry$ with_you$ s& 
	;
: you_are_carrying_only$  ( -- a u )  \ Devuelve mensaje para encabezar una lista de inventario de un solo elemento
	2 random
	if  ^you_are_carrying$ only_$ s& 
	else  ^only_$ you_are_carrying$ s& 
	then
	;
:action do_inventory  ( -- )  \ Acción de hacer inventario
	protagonist% content_list  \ Hace la lista en la cadena dinámica PRINT_STR
	#listed @ case
		0 of  you_are_carrying_nothing$ 2swap s& endof
		1 of  you_are_carrying_only$ 2swap s& endof
		>r ^you_are_carrying$ 2swap s& r>
	endcase  narrate 
	;action

\ }}}---------------------------------------------
subsection( Hacer)  \ {{{

:action do_make  ( -- )  \ Acción de hacer (fabricar)
	main_complement @
	if  nonsense
	else  do_not_worry
	then
	;action

:action do_do  ( -- )  \ Acción de hacer (genérica)
	main_complement @ inventory% =
	if  do_inventory
	else do_make
	then
	;action

\ }}}---------------------------------------------
subsection( Hablar y presentarse)  \ {{{

\ ------------------------------------------------
\ Conversaciones con el líder de los refugiados

: a_man_takes_the_stone  ( -- )  \ Mensaje sobre el hombre que devuelve la pieda a su sitio
	s{ s" Un hombre" s" Uno de los" s{ s" hombres" s" refugiados" }s }s
	s" te" s&{ s" arrebata" s" quita" }s&
	s" la piedra" s& s" de las manos" s?& s" y" s&
	s{
		s" se la lleva."
		s{ s" se marcha" s" se va" s" desaparece" }s s" con ella" s?& period+
	}s&
	narrate  location_18% stone% is_there
	;
: gets_angry$  ( -- a u )  \ Devuelve una variante de «se enfada»
	s" se" s{ s" irrita" s" enfada" s" enoja" s" enfurece" }s& 
	;
: the_leader_gets_angry$  ( -- a u )  \ Devuelve una variante de «El líder se enfada»
	s" El" old_man$ s& gets_angry$ s& 
	;
: the_leader_gets_angry  ( -- )  \ Mensaje de que el líder se enfada
	the_leader_gets_angry$ gets_angry$ s& period+
	narrate
	;
: you_can_not_take_the_stone  ( -- )  \ El líder dice que no te puedes llevar la piedra
	s" No podemos" s{ s" permitiros" s" consentiros" }s
	s{ s" huir con" s" escapar con" s" marchar con" s" pasar con"
	s" que os vayáis con" s" que os marchéis con"
	s" que marchéis con" s" que os llevéis" s" que robéis"
	}s& s" la piedra del druida." speak
	s" piedra del druida" stone% name!  \ Nuevo nombre para la piedra
	s" Hace un gesto..."
	narrate narration_break
	;
: the_stone_must_be_in_its_place  ( -- )  \ El líder dice que la piedra debe ser devuelta
	s" La piedra" s{ s" debe" s" tiene que" }s&
	s{ s" ser devuelta" s" devolverse" to_go_back$ }s&
	s" a su lugar" s" de encierro" s?& period+
	speak
	;
: the_leader_talks_about_the_stone  ( -- )  \ El líder habla acerca de la piedra
	the_leader_gets_angry
	you_can_not_take_the_stone  
	the_stone_must_be_in_its_place
	;
: the_leader_points_to_the_north  ( -- )  \ El líder se enfada y apunta al Norte
	the_leader_gets_angry$  
	s" y" s&{ s" alza" s" extiende" s" levanta" }s&
	s{ s" su" s" el" }s& s" brazo" s&
	s{ s" indicando" s" en dirección" s" señalando" }s&
	toward_the(m)$ s& s" Norte." s&
	narrate
	;
: nobody_passes_with_arms  ( -- )  \ El líder dice que nadie pasa con armas
	s{ s" Nadie" s" Ningún hombre" }s
	s{ s" con" s" llevando" s" portando" s" portador de"
	s" que porte" s" que lleve" }s&
	s{ s" armas" s" un arma" s" una espada" }s&
	with_him$ s&{ s" debe" s" puede " s" podrá" }s& 
	s" pasar." s&  speak
	;
: the_leader_talks_about_the_sword  ( -- )  \ El líder habla acerca de la espada
	the_leader_points_to_the_north  
	nobody_passes_with_arms  
	;
: the_leader_points_to_the_east  ( -- )  \ El líder se enfada y apunta al Este
	s" El" old_man$ s& comma+
	s{ s" calmado," s" sereno," s" tranquilo," }s&
	s{ s" indica" s" señala" }s&
	s{ toward_the(m)$ s" en dirección al" }s& s" Este y" s&
	s{ s" habla:" s" dice:" s" pronuncia las siguientes palabras:" }s&
	narrate
	;
: go_in_peace  ( -- )  \ El líder dice que puedes ir en paz
	s" Si vienes en paz, puedes ir en paz." speak
	;
: the_refugees_let_you_go  ( -- )  \ Los refugiados te dejan pasar
	s{ s" Todos" s" los refugiados" s?& s" Los refugiados" }s
	s" se apartan y" s& s" te" s?&
	s{ s" permiten" s{ s" el paso" s" pasar" }s&
	s" dejan"
	s" libre" s" el" s{ s" paso" s" camino" }s& r2swap s& }s&
	toward_the(m)$ s& s" Este." s&
	narrate
	;
: the_leader_lets_you_go  ( -- )  \ El jefe deja marchar al protagonista
	location_28% location_29% e-->  \ Hacer que la salida al Este de LOCATION_28% conduzca a LOCATION_29%
	the_leader_points_to_the_east
	go_in_peace  the_refugees_let_you_go  
	;
: talked_to_the_leader  ( -- )  \ Aumentar el contador de conversaciones con el jefe de los refugiados
	leader% conversations++
	;
: we_are_refugees  ( -- )  \ Somos refugiados
	s" Somos refugiados de la gran guerra."
	s{
	s{ ^only$ s" buscamos" s& s" Buscamos" }s
	s{ ^only$ s" queremos" s& s" Queremos" }s
	}s&  s{ s" la" s" vivir en" }s& s" paz." s&
	speak narration_break
	; 
: first_conversation_with_the_leader
	my_name_is$ s" Ulfius y..." s& speak
	talked_to_the_leader
	s" El" old_man$ s& s" asiente, impaciente." s& narrate
	we_are_refugees
	;
: the_leader_checks_what_you_carry  ( -- )  \ El jefe controla lo que llevas
	true case
		stone% is_accessible?  of
			the_leader_talks_about_the_stone
			a_man_takes_the_stone
		endof
		sword% is_accessible?  of
			the_leader_talks_about_the_sword
		endof
		the_leader_lets_you_go
	endcase
	;
: talk_to_the_leader  ( -- )  \ Hablar con el jefe
	leader% no_conversations?
	if  first_conversation_with_the_leader  then
	the_leader_checks_what_you_carry  
	;

\ ------------------------------------------------
\ Conversaciones con Ambrosio

: talked_to_ambrosio  ( -- )  \ Aumentar el contador de conversaciones con Ambrosio
	ambrosio% conversations++
	;
: (conversation_0_with_ambrosio)  ( -- )  \ Primera conversación con Ambrosio
	s" Hola, buen hombre." speak
	s" Hola, Ulfius." 
	my_name_is$ s& s" Ambrosio" 2dup ambrosio% name!
	period+ s& speak
	scene_break
	s" Por" s" primera" s" vez" r2swap s& s& s" en" s&
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
: conversation_0_with_ambrosio  ( -- )  \ Primera conversación con Ambrosio, si se dan las condiciones
	location_19% am_i_there?
	if  (conversation_0_with_ambrosio)  then
	;
: (conversation_1_with_ambrosio)  ( -- )  \ Segunda conversación con Ambrosio
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
: conversation_1_with_ambrosio  ( -- )  \ Segunda conversación con Ambrosio, si se dan las condiciones
	location_46% am_i_there?
	ambrosio_follows? 0=  and
	if  (conversation_1_with_ambrosio)  then
	;
: (conversation_2_with_ambrosio)  ( -- )  \ Tercera conversación con Ambrosio
	s{ s" Por favor," s" Os lo ruego," }s
	s" Ulfius," s&
	s" cumplid vuestra" s{ s" promesa." s" palabra." }s&
	s" Tomad la llave" s&
	s{ 0$ s" en vuestra mano" s" en vuestras manos" s" con vos" }s&
	s" y abrid la puerta de la cueva." s&  speak
	key% is_hold
	\ aquí en SuperBASIC: do_takeable the_key \ pendiente!!!
	ambrosio_follows? on  talked_to_ambrosio  
	;
: conversation_2_with_ambrosio  ( -- )  \ Tercera conversación con Ambrosio, si se dan las condiciones
	location_45% 1- location_47% 1+ my_location within 
	if  (conversation_2_with_ambrosio)  then
	;
: (talk_to_ambrosio)  ( -- )  \ Hablar con Ambrosio
	\ Método nuevo en pruebas!!!:
	ambrosio% conversations  case
		0  of  conversation_0_with_ambrosio  endof
		1  of  conversation_1_with_ambrosio  endof
		2  of  conversation_2_with_ambrosio  endof
		\ Aquí faltaría algo!!!
	endcase
	\ Métodos antiguos!!!:
	[false]  [IF]
	my_location case
		location_19%  of  conversation_0_with_ambrosio  endof
		location_46%  of  conversation_1_with_ambrosio  endof
	endcase
	[THEN]
	[false]  [IF]
	my_location case
		location_45%  of  conversation_2_with_ambrosio  endof
		location_46%  of  conversation_2_with_ambrosio  endof
		location_47%  of  conversation_2_with_ambrosio  endof
	endcase
	[THEN]
	[false]  [IF]  \ Método alternativo poco legible, inacabado!!!
	location_45% 1- location_47% 1+ my_location within  if
		conversation_2_with_ambrosio
	then
	[THEN]
	;
: talk_to_ambrosio  ( -- )  \ Hablar con Ambrosio, si se puede
	\ Provisional!!! Esto debería comprobarse en DO_SPEAK o DO_SPEAK_IF_POSSIBLE
	ambrosio% is_here?
	if  (talk_to_ambrosio)
	else  ambrosio% is_not_here
	then
	;

\ ------------------------------------------------
\ Conversaciones sin éxito

: talk_to_something  ( a -- )  \ Hablar con un ente que no es un personaje 
	\ Pendiente!!!
	2 random
	if  drop nonsense
	else  full_name s" hablar con" 2swap s& is_nonsense 
	then
	;
: talk_to_yourself$  ( -- a u )  \ Devuelve una variante de «hablar solo»
	s{ s" hablar" s{ s" solo" s" con uno mismo" }s&
	s" hablarse" s{ s" a sí" s" a uno" }s& s" mismo" s?&
	}s 
	;
: talk_to_yourself  ( -- )  \ Hablar solo
	talk_to_yourself$ is_nonsense
	;

\ ------------------------------------------------
\ Acciones

: do_speak_if_possible  ( a -- )  \ Hablar con un ente si es posible
	[debug]  [IF]  s" En DO_SPEAK_IF_POSSIBLE" debug  [THEN]  \ Depuración!!!
	case
		leader%  of  talk_to_the_leader  endof
		ambrosio%  of  talk_to_ambrosio  endof
		dup talk_to_something
	endcase
	;
: (do_speak)  ( a | 0 -- )  \ Hablar con alguien o solo
	if  do_speak_if_possible
	else  talk_to_yourself
	then
	;
:action do_speak  ( -- )  \ Acción de hablar
	[debug]  [IF]  s" En DO_SPEAK" debug  [THEN]  \ Depuración!!!
	main_complement @ ?dup 0=  \ Si no hay complemento...
	if  whom  \ ...buscar el más probable
	then  (do_speak)
	;action
:action do_introduce_yourself  ( -- )  \ Acción de presentarse a alguien
	main_complement @ ?dup 0=  \ Si no hay complemento...
	if  unknown_whom  \ ...buscar el (desconocido) más probable
	then  (do_speak)
	;action

\ }}}---------------------------------------------
subsection( Guardar el juego)  \ {{{

0  [IF]  \ ......................................

Parece que SP-Forth tiene problemas para crear un ejecutable
con el estado del juego. (Los detalles del problema están en
el historial de desarrollo y en la definición de la palabra
que crea el ejecutable principal, en la sección Meta).

Para guardar el estado de la partida usaremos una solución
alternativa: ficheros de texto que reproduzcan el código
Forth necesario para restaurarlas. Esto ocupará menos y es
más transportable entre plataformas.

[THEN]  \ ......................................

0 [IF] \ Inacabado!!! Pendiente!!!
: n>s  ( u -- a1 u1 )  \ Convierte un número en una cadena (con dos dígitos como mínimo)
	s>d <# # #s #> >csb
	;
: n>s+  ( u a1 u1 -- a2 u2 )  \ Añade a una cadena un número tras convertirlo en cadena
	rot n>s s+
	;
: yyyymmddhhmmss$  ( -- a u )  \ Devuelve la fecha y hora actuales como una cadena en formato «aaaammddhhmmss»
	time&date n>s n>s+ n>s+ n>s+ n>s+ n>s+
	;
: file_name$  ( -- a u )  \ Devuelve el nombre con que se grabará el juego
	s" ayc_" yyyymmddhhmmss$ s+
	s" .exe" windows? and s+  \ Añadir sufijo si estamos en Windows
	;
defer reenter
svariable filename
: (do_save_the_game)  ( -- )  \ Graba el juego
	\ Inacabado!!! No está decidido el sistema que se usará para salvar las partidas
	\ 2011-12-01 No funciona bien. Muestra mensajes de gcc con parámetros sacados de textos del programa!
\	false to spf-init?  \ Desactivar la inicialización del sistema
\	true to console?  \ Activar el modo de consola (no está claro en el manual)
\	false to gui?  \ Desactivar el modo gráfico (no está claro en el manual)
	['] reenter to <main>  \ Actualizar la palabra que se ejecutará al arrancar
\	file_name$ save  new_page
	file_name$ filename s! filename count save  
	;
:action do_save_the_game  ( -- )  \ Acción de salvar el juego
	main_complement{forbidden}
	(do_save_the_game)
	;action
[THEN]

svariable game_file_name  \ Nombre del fichero en que se graba la partida
variable game_file_id  \ Identificador del fichero en que se graba la partida
: game_file_name$  ( -- a u )  \ Devuelve el nombre del fichero en que se graba la partida
	game_file_name count
	;
: close_game_file  ( -- )  \ Cierra el fichero en que se grabó la partida
	game_file_id @ close-file abort" Close file error."  \ mensaje tmp!!!
	;
: create_game_file  ( a u -- )  \ Crea un fichero para guardar una partida (sobreescribiendo otro que tuviera el mismo nombre)
	\ a u = Nombre del fichero
	r/w create-file abort" Create file error."  \ mensaje tmp!!!
	game_file_id !
	;
: read_game_file  ( a u -- )  \ Lee el fichero de configuración
	\ Pendiente!!! Comprobar la existencia del fichero y atrapar errores al leerlo.
	[lina?]
	[IF]    postpone only restore_vocabulary
	[ELSE]  only restore_vocabulary
	[THEN]  included  restore_vocabularies
	;
: >file/  ( a u -- )  \ Escribe una línea en el fichero en que se está grabando la partida
	game_file_id @ write-line abort" Write file error"  \ mensaje tmp!!!
	;
: cr>file  ( a u -- )  \ Escribe un final de línea en el fichero en que se está grabando la partida
	s" " >file/
	;
: >file  ( a u -- )  \ Escribe una cadena en el fichero en que se está grabando la partida
	space+
	game_file_id @ write-file abort" Write file error"  \ mensaje tmp!!!
	;
lina? 0=  [IF]  also  [THEN]
restore_vocabulary  definitions
' \ alias \
lina? 0=  [IF]  immediate  [THEN]
' true alias true
' false alias false
: load_entity  ( x0..xn a -- )  \ Restaura los datos de un ente
	\ x0..xn = Datos del ente, en orden inverso a como los crea la palabra SAVE_ENTITY .
	\ a = Ente.
	>r
	r@ ~direction !
	r@ ~in_exit !
	r@ ~out_exit !
	r@ ~down_exit !
	r@ ~up_exit !
	r@ ~west_exit !
	r@ ~east_exit !
	r@ ~south_exit !
	r@ ~north_exit !
	r@ ~familiar !
	r@ ~visits !
	r@ ~location_plot_xt !
	r@ ~previous_location !
	r@ ~location !
	r@ ~is_location? !
	r@ ~is_open? !
	r@ ~is_human? !
	r@ ~is_animal? !
	r@ ~is_vegetal? !
	r@ ~is_lit? !
	r@ ~is_light? !
	r@ ~is_worn? !
	r@ ~is_cloth? !
	r@ ~is_owned? !
	r@ ~is_global_indoor? !
	r@ ~is_global_outdoor? !
	r@ ~break_error# !
	r@ ~take_error# !
	r@ ~is_decoration? !
	r@ ~conversations !
	r@ ~is_character? !
	r@ ~init_xt !
	r@ ~description_xt !
	r@ ~has_definite_article? !
	r@ ~has_no_article? !
	r@ ~has_plural_name? !
	r@ ~has_feminine_name? !
	r@ ~has_personal_name? !
	r> name!
	;
restore_vocabularies
: string>file  ( a u -- )  \ Crea una cadena en el fichero de la partida
	s| s"| 2swap s& s| "| s+ >file
	;
: f>string  ( ff -- a u )  \ Convierte un indicador binario en su nombre de constante
	if  s" true"  else  s" false"  then
	;
: f>file  ( ff -- )  \ Crea un indicador binario en el fichero de la partida
	f>string >file
	;
: n>string  ( n -- a u )  \ Convierte un número con signo en una cadena
	s>d swap over dabs
	<# #s rot sign #> >csb
	;
: u>string ( u -- a u )  \ Convierte un número sin signo en una cadena
	s>d <# #s #> >csb
	;
: 00>s  ( u -- a1 u1 )  \ Convierte un número sin signo en una cadena (de dos dígitos como mínimo)
	s>d <# # #s #> >csb
	;
: 00>s+  ( u a1 u1 -- a2 u2 )  \ Añade a una cadena un número tras convertirlo en cadena
	rot 00>s s+
	;
: yyyy-mm-dd_hh:mm:ss$  ( -- a u )  \ Devuelve la fecha y hora actuales como una cadena en formato «aaaa-mm-dd_hh:mm:ss»
	time&date 00>s dash2+ 00>s+ dash2+ 00>s+ space+
	00>s+ colon+ 00>s+ colon+ 00>s+
	;
: n>file  ( n -- )  \ Crea un número con signo en el fichero de la partida
	n>string >file
	;
: save_entity  ( u -- )  \ Guarda en el fichero de la partida los datos de un ente
	#>entity >r
	r@ name string>file
	r@ has_personal_name? f>file
	r@ has_feminine_name? f>file
	r@ has_plural_name? f>file
	r@ has_no_article? f>file
	r@ has_definite_article? f>file
	r@ description_xt n>file
	r@ init_xt n>file
	r@ is_character? f>file
	r@ conversations n>file
	r@ is_decoration? f>file
	r@ take_error# n>file
	r@ break_error# n>file
	r@ is_global_outdoor? f>file
	r@ is_global_indoor? f>file
	r@ is_owned? f>file
	r@ is_cloth? f>file
	r@ is_worn? f>file
	r@ is_light? f>file
	r@ is_lit? f>file
	r@ is_vegetal? f>file
	r@ is_animal? f>file
	r@ is_human? f>file
	r@ is_open? f>file
	r@ is_location? f>file
	r@ location n>file
	r@ previous_location n>file
	r@ location_plot_xt n>file
	r@ visits n>file
	r@ familiar n>file
	r@ north_exit n>file
	r@ south_exit n>file
	r@ east_exit n>file
	r@ west_exit n>file
	r@ up_exit n>file
	r@ down_exit n>file
	r@ out_exit n>file
	r@ in_exit n>file
	r@ direction n>file
	r> n>file s" restore_entity" >file/  \ Añadir el número de ente y la palabra que hará la restauración
	;
: save_entities  ( -- )  \ Guarda todos los entes en el fichero de la partida
	#entities 0  do  i save_entity  loop
	;
: save_flag  ( a u ff -- )
	;
: save_config
	;
: save_plot
	;
: write_game_file  ( -- )  \ Escribe el fichero en que se graba la partida
	s" \ Datos de restauración de una partida de «Asalto y castigo»" >file/
	s" \ Fichero creado en" yyyy-mm-dd_hh:mm:ss$ s& >file/
	s" \ Entes" >file/
	save_entities
	s" \ Configuración" >file/
	save_config
	s" \ Trama" >file/
	save_plot
	;
: fs+  ( a u -- a' u' )  \ Añade la extensión .fs a un nombre de fichero
	s" .fs" s+
	;
: (do_save_the_game)  ( a u -- )  \ Salva la partida
	fs+ create_game_file write_game_file close_game_file
	;
:action do_save_the_game  ( a u -- )  \ Acción de salvar la partida
	\ main_complement{forbidden}
	(do_save_the_game)
	;action
:action do_load_the_game  ( a u -- )  \ Acción de salvar la partida
	\ Pendiente!!! No funciona
	\ main_complement{forbidden}
	[lina?]
	[IF]    postpone only restore_vocabulary 
	[ELSE]  only restore_vocabulary
	[THEN]
	[debug_parsing] ?halto" in do_load_the_game before save-input"
	\ included  \ !!! el sistema estalla
	\ 2drop  \ !!! sin error
	\ cr type  \ !!! sin error	
	2>r save-input 2r>
	[debug_parsing] ?halto" in do_load_the_game before fs+"
	fs+
	[debug_parsing] ?halto" in do_load_the_game before included"
	['] included 
	[debug_parsing] ?halto" in do_load_the_game before catch"
	catch  
	[debug_parsing] ?halto" in do_load_the_game after catch"
	restore_vocabularies
	[debug_parsing] ?halto" in do_load_the_game before if"
	?dup  if
		( a u u2 ) nip nip
		case  \ tmp!!!
			2  of  s" Fichero no encontrado." narrate  endof
			s" Error al intentar leer el fichero." narrate
		endcase
		[debug_parsing] ?halto" in do_load_the_game after endcase"
	then
	[debug_parsing] ?halto" in do_load_the_game after then"
	restore-input
	[debug_parsing] ?halto" in do_load_the_game at the end" 
	;action

\ }}}
\ }}}###########################################################
section( Intérprete de comandos)  \ {{{

0  [IF]  \ ......................................

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
reconozca (las no reconocidas las ignorará, gracias a una
útil peculiaridad de SP-Forth) en el orden en que estén
escritas en la frase.  Esto quiere decir que, al contrario
de lo que ocurre con otros métodos, no podremos tener una
visión global del comando del jugador: ni de cuántas
palabras consta ni, en principio, qué viene a continuación
de la palabra que está siendo interpretada en cada momento.

Una solución sería que cada palabra del jugador guardara un
identificador unívoco en la pila o en una tabla, y
posteriormente interpretáramos el resultado de una forma
convencional.

Sin embargo, hemos optado por dejar a Forth hacer su trabajo
hasta el final, pues nos parece más sencillo y eficaz: las
palabras reconocidas en el comando del jugador se ejecutarán
pues en el orden en que fueron escritas. Cada una
actualizará el elemento del comando que represente (verbo o
complemento) tras comprobar si ya ha habido una palabra
previa que realice la misma función y en su caso deteniendo
el proceso con un error.

[THEN]  \ ......................................

: init_parsing  ( -- )  \ Preparativos previos al análisis
	action off
	main_complement off
	;
subsection( x1...)
: (execute_action)  ( xt -- )  \ Ejecuta la acción del comando
	[debug_catch] [debug_parsing] [or] ?halto" En (EXECUTE_ACTION) antes de CATCH"  \ Depuración!!!
	catch  \ Ejecutar la acción a través de CATCH para poder regresar directamente con THROW en caso de error
	[debug_catch] [debug_parsing] [or] ?halto" En (EXECUTE_ACTION) después de CATCH"  \ Depuración!!!
	?wrong
	[debug_catch] [debug_parsing] [or] ?halto" En (EXECUTE_ACTION) después de ?WRONG"  \ Depuración!!!
	;
subsection( x1 execute_action:)
: execute_action  ( -- )  \ Ejecuta la acción del comando, si existe
	[debug_catch] [debug_parsing] [or] ?halto" En EXECUTE_ACTION"  \ Depuración!!!
	action @ ?dup
	[debug_catch] [debug_parsing] [or] ?halto" En EXECUTE_ACTION tras ACTION @ ?DUP"  \ Depuración!!!
	if  (execute_action)  else  no_verb_error# ?wrong  then
	[debug_catch] [debug_parsing] [or] ?halto" Al final de EXECUTE_ACTION"  \ Depuración!!!
	;
subsection( x1 valid_parsing?:)
: ignore_case  ( -- )  \ Hace el intérprete insensible a mayúsculas
	[sp-forth?]  [IF]  case-ins on  [THEN]
	[lina?]  [IF]  case-insensitive  [THEN]
	;
: don't_ignore_case  ( -- )  \ Hace el intérprete sensible a mayúsculas
	[sp-forth?]  [IF]  case-ins off  [THEN]
	[lina?]  [IF]  case-sensitive  [THEN]
	;
: valid_parsing?  ( a u -- ff )  \ Evalúa un comando con el vocabulario del juego
	\ a u = Comando
	\ ff = ¿El comando se analizó sin error?
	[debug_parsing] ?halto" Entrando en VALID_PARSING?"  \ Depuración!!!

	don't_ignore_case 
	\ Dejar solo el diccionario PLAYER_VOCABULARY activo
	[lina?]
	[IF]    postpone only player_vocabulary
	[ELSE]  only player_vocabulary
	[THEN]
	\ [debug_catch]  [IF]  s" En VALID_PARSING? antes de CATCH" debug  [THEN]  \ Depuración!!!
	[debug_parsing] ?halto" en valid_parsing? antes de preparar CATCH"
	['] evaluate 
	[debug_parsing] ?halto" en valid_parsing? antes de CATCH"
	catch  \ Llamar a EVALUATE a través de CATCH para poder regresar directamente con THROW en caso de error
	[debug_parsing] ?halto" en valid_parsing? después de CATCH"
	ignore_case
	\ Pendiente!!! problema aún no resuelto
	\ Ahora «abre» sin complemento
	\ nip nip  \ Arreglar la pila, pues CATCH hace que apunte a su posición previa
	\ [debug_catch]  [IF]  s" En VALID_PARSING? después de CATCH" debug  [THEN]  \ Depuración!!!
	dup  if  nip nip  then
	[debug_parsing] ?halto" en valid_parsing? tras NIP NIP"
	dup ?wrong 0=
	[debug_parsing] ?halto" en valid_parsing? tras ?WRONG"
	restore_vocabularies
	[debug_parsing] ?halto" Saliendo de VALID_PARSING?"  \ Depuración!!!
	;
subsection( x1)
: save_command_elements  ( -- )
	\ No se usa!!!
	\ Pendiente!!! El reconocimiento de pronombres aún no está implementado
	action @ last_action !
	main_complement @ 
	last_feminine_singular_complement !
	last_masculine_singular_complement !
	last_feminine_plural_complement !
	last_masculine_plural_complement !
	;
subsection( x1 obbey:)
: obbey  ( a u -- )  \ Evalúa un comando con el vocabulario del juego
	[debug_parsing] ?halto" Al entrar en OBBEY"  \ Depuración!!!
	dup  if
		init_parsing valid_parsing?
		if  execute_action  then
	else  2drop
	then
	[debug_parsing] ?halto" Al final de OBBEY"  \ Depuración!!!
	; 
subsection( x1 second?:)
: second?  ( a1 a2 -- a1 ff )  \ ¿La acción o el complemento son los segundos que se encuentran?
	\ a1 = Acción o complemento recién encontrado
	\ a2 = Acción o complemento anterior, o cero
	[debug_parsing] ?halto" second? 1"
	2dup <> swap 0<> and  \ ¿Hay ya otro anterior y es diferente?
	[debug_parsing] ?halto" second? 2"
	;
subsection( x1)
: action!  ( a -- )  \ Comprueba y almacena la acción (la dirección de ejecución de su palabra) 
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
: preposition!  ( a -- )
	\ Pendiente!!!
	;
subsection( x1)
: (complement!)  ( a -- )  \ Almacena un complemento (la dirección de la ficha de su ente)
	[debug_parsing] ?halto" En (COMPLEMENT!)"  \ Depuración!!!
	other_complement @ second?  \ ¿Había ya un complemento secundario?
	if  too_many_complements_error# throw  \ Sí, error
	else  other_complement !  \ No, guardarlo
	then
	;
subsection( x1)
: complement!  ( a -- )  \ Comprueba y almacena un complemento (la dirección de la ficha de su ente)
	[debug_parsing] ?halto" En COMPLEMENT!"  \ Depuración!!!
	main_complement @ second?  \ ¿Había ya un complemento directo?
	if  (complement!)
	else  main_complement !
	then
	;
subsection( x1)
: action|complement!  ( a1 a2 -- )  \ Comprueba y almacena un posible complemento o una posible acción, significados ambos de la misma palabra
	\ a1 = Identificador de acción
	\ a2 = Identificador de ente
	action @  \ ¿Había ya una acción reconocida?
	if  nip complement!  \ Sí, luego tomamos el uso de complemento
	else  drop action!  \ No, luego tomamos el uso de acción
	then
	;

\ }}}###########################################################
section( Fichero de configuración)  \ {{{

0  [IF]  \ ......................................

El juego tiene un fichero de configuración en que el jugador
puede indicar sus preferencias. Este fichero es código en
Forth y se interpreta directamente, pero en él solo serán
reconocidas las palabras creadas expresamente para la
configuración, así como las palabras habituales para hacer
comentarios de bloques o líneas en Forth. Cualquier otra
palabra dará error.

El fichero de configuración se lee de nuevo al inicio de
cada partida.

[THEN]  \ ......................................

lina? [IF]  s" ayc/ayc.ini"  [THEN]
\ Nota: lina no necesita pasar la cadena al almacén circular,
\ porque la crea en el espacio de diccionario, actualizando después HERE .
\ En los demás Forth hay que hacerlo, porque crean la cadena en PAD
\ y el funcionamiento del siguiente [IF] o [ELSE] la machacaría.
sp-forth? [IF]  s" ~programandala.net/ayc/ayc.ini" >csb  [THEN]
gforth? [IF]  s" ayc.ini" >csb  [THEN]
bigforth? [IF]  s" ayc.ini" >csb  [THEN]
sconstant config_file$  \ Fichero de configuración

svariable command_prompt

lina? 0=  [IF]  also  [THEN]
config_vocabulary  definitions

\ Las palabras cuyas definiciones siguen a continuación
\ se crearán en el vocabulario CONFIG_VOCABULARY y
\ son las únicas que podrán usarse para configurar el juego
\ en el fichero de configuración:

[false]  [IF]  \ En vez de así:
: (  ( "texto<cierre de paréntesis>" -- ) \ Comentario clásico
	postpone ( 
	;  immediate
: \  ( "texto<fin de línea>" -- ) \ Comentario de línea
	postpone \ 
	;  immediate
[ELSE]  \ Es más sencillo así:
' ( alias (  \ Cerramos paréntesis solo para no arruinar el coloreado de sintaxis: ) 
lina? 0=  [IF]  immediate  [THEN]
' \ alias \ 
lina? 0=  [IF]  immediate  [THEN]
[THEN]
' true alias sí
' false alias no
: columnas  ( u -- )  \ Cambia el número de columnas
	1- to max_x
	;
: líneas ( u -- )  \ Cambia el número de líneas
	1- to max_y
	;
: varón  ( -- )  \ Indica que el jugador es un varón
	woman_player? off
	;
' varón alias hombre
' varón alias masculino
: mujer  ( -- )  \ Indica que el jugador es una mujer
	woman_player? on
	;
' mujer alias femenino
: comillas  ( ff -- )  \ Indica si se usan las comillas castellanas en las citas
	castilian_quotes? !
	;
: espacios_de_indentación  ( u -- )  \ Fija la indentación de los párrafos
	max_indentation min /indentation !
	;
: indentar_primera_línea_de_pantalla  ( ff -- )  \ Indica si se indentará también la línea superior de la pantalla, si un párrafo empieza en ella
	indent_first_line_too? !
	;
: indentar_prestos_de_pausa  ( ff -- )  \ Indica si se indentarán los prestos
	indent_pause_prompts? !
	;
: borrar_pantalla_para_escenarios  ( ff -- )  \ Indica si se borra la pantalla al entrar en un escenario o describirlo
	location_page? !
	;
: borrar_pantalla_para_escenas  ( ff -- )  \ Indica si se borra la pantalla tras la pausa de fin de escena
	scene_page? !
	;
: separar_párrafos  ( ff -- )  \ Indica si se separan los párrafos con un línea en blanco
	cr? !
	;
: milisegundos_en_pausas_de_narración  ( u -- )  \ Indica los milisegundos de las pausas cortas (o, si es valor es cero, que hay que pulsar una tecla)
	narration_break_milliseconds ! 
	;
: milisegundos_en_pausas_de_final_de_escena  ( u -- )  \ Indica los milisegundos de las pausas de final de esecena (o, si es valor es cero, que hay que pulsar una tecla)
	scene_break_milliseconds !
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

\ : papel_de_fondo  ( u -- )  background_paper !  ;  \ No implementado!!!
: pluma_de_créditos  ( u -- )  about_pen !  ;
: papel_de_créditos  ( u -- )  about_paper !  ;
: pluma_de_presto_de_comandos  ( u -- )  command_prompt_pen !  ;
: papel_de_presto_de_comandos  ( u -- )  command_prompt_paper !  ;
: pluma_de_depuración  ( u -- )  debug_pen !  ;
: papel_de_depuración  ( u -- )  debug_paper !  ;
: pluma_de_descripción  ( u -- )  description_pen !  ;
: papel_de_descripción  ( u -- )  description_paper !  ;
: pluma_de_error  ( u -- )  error_pen !  ;
: papel_de_error  ( u -- )  error_paper !  ;
: pluma_de_entrada  ( u -- )  input_pen !  ;
: papel_de_entrada  ( u -- )  input_paper !  ;
: pluma_de_descripción_de_escenario  ( u -- )  location_description_pen !  ;
: papel_de_descripción_de_escenario  ( u -- )  location_description_paper !  ;
: pluma_de_nombre_de_escenario  ( u -- )  location_name_pen !  ;
: papel_de_nombre_de_escenario  ( u -- )  location_name_paper !  ;
: pluma_de_narración  ( u -- )  narration_pen !  ;
: papel_de_narración  ( u -- )  narration_paper !  ;
: pluma_de_presto_de_pantalla_llena  ( u -- )  scroll_prompt_pen !  ;
: papel_de_presto_de_pantalla_llena  ( u -- )  scroll_prompt_paper !  ;
: pluma_de_pregunta  ( u -- )  question_pen !  ;
: papel_de_pregunta  ( u -- )  question_paper !  ;
: pluma_de_presto_de_escena  ( u -- )  scene_prompt_pen !  ;
: papel_de_presto_de_escena  ( u -- )  scene_prompt_paper !  ;
: pluma_de_diálogos  ( u -- )  speech_pen !  ;
: papel_de_diálogos  ( u -- )  speech_paper !  ;

\ Nota!!!: no se puede definir NOTFOUND así porque los números no serían reconocidos:
\ : NOTFOUND  ( a u -- )  2drop  ;

\ Fin de las palabras permitidas en el fichero configuración.

restore_vocabularies

: init_config  ( -- )  \ Inicializa las variables de configuración con sus valores predeterminados
	woman_player? off
	castilian_quotes? on
	location_page? on
	cr? off
	ignore_unknown_words? off
	4 /indentation !
	indent_first_line_too? on
	indent_pause_prompts? on
	s" ..." scroll_prompt s!
	s" ..." narration_prompt s!
	s" ..." scene_prompt s!
	s" >" command_prompt s!
	-1 narration_break_milliseconds !
	-1 scene_break_milliseconds !
	scene_page? on
	default_max_x to max_x  \ Número máximo de columna 
	default_max_y to max_y  \ Número máximo de fila 
	init_colors  \ Colores predeterminados
	;
: read_config  ( -- )  \ Lee el fichero de configuración
	\ Pendiente!!! Atrapar errores al leerlo.
	[lina?]
	[IF]    postpone only 
	[ELSE]  only 
	[THEN]  config_vocabulary
	config_file$
	2dup type cr order key drop  \ Depuración!!!
	['] included catch  ( x1 x2 n )
	restore_vocabularies
	?dup  if  throw  then  2drop
	;
: get_config  ( -- )  \ Lee el fichero de configuración tras inicializar las variables de configuración
	init_config read_config
	;

\ }}}###########################################################
section( Herramientas para crear el vocabulario del juego)  \ {{{

0  [IF]  \ ......................................

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
FORTH , y el vocabulario propio del programa.

Para evitar que el sistema se cuelgue en el improbable caso
de que el jugador escriba la palabra NOTFOUND en el comando
(pues se intentaría borrar dos elementos de la pila estando
ésta vacía), basta comprobar la profundidad de la pila antes
de borrar la cadena recibida.

[THEN]  \ ......................................

: parse_synonym  ( -- a u )  \ Devuelve el siguiente sinónimo de la lista
	begin  parse-name dup 0=
	while  2drop refill 0= abort" Error en el código fuente: lista de sinónimos incompleta"
	repeat
	\ 2dup ." sinónimo: " type space \ Depuración!!!
	;
: (another_synonym?)  ( a u -- ff )  \ ¿No se ha terminado la lista de sinónimos?
	s" }synonyms" compare
	;
sp-forth? lina? or  [IF]
: another_synonym?  ( -- a u ff )  \ Toma la siguiente palabra en el flujo de entrada y comprueba si es el final de la lista de sinónimos
	parse_synonym 2dup (another_synonym?)
	;
: synonyms{  (  xt "name#0..name#n synonyms" -- )  \ Crea uno o varios sinónimos de una palabra
	\ xt = Dirección de ejecución de la palabra a clonar
	begin  dup another_synonym? ( xt xt a u ff )
	while  (alias)
	repeat  2drop 2drop
	;
: immediate_synonyms{  (  xt "name#0..name#n }synonyms" -- )  \ Crea uno o varios sinónimos inmediatos de una palabra
	\ xt = Dirección de ejecución de la palabra a clonar
	begin  dup another_synonym?  ( xt xt a u ff )
	while  (alias)  immediate
	repeat  2drop 2drop
	;
[THEN]
gforth?  [IF]  \ Inacabado!!!
: another_synonym?  ( -- ff )  \ Hace las operaciones del interior del bucle de creación de sinónimos
	save-input parse_synonym 2>r restore_input
	2r> (another_synonym?)
	;
: synonyms{  (  xt "name#0..name#n }synonyms" -- )  \ Crea uno o varios sinónimos de una palabra
	\ xt = Dirección de ejecución de la palabra a clonar
	begin  dup another_synonym? ( xt xt ff )
	while  alias
	repeat  2drop 
	;
: immediate_synonyms{  (  xt "name#0..name#n }synonyms" -- )  \ Crea uno o varios sinónimos inmediatos de una palabra
	\ xt = Dirección de ejecución de la palabra a clonar
	begin  dup another_synonym?  ( xt xt ff )
	while  alias  immediate
	repeat  2drop
	;
[THEN]

\ Resolución de ambigüedades

0  [IF]  \ ......................................

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

[THEN]  \ ......................................

: (man) ( -- a | false )  \ Devuelve el ente adecuado a la palabra «hombre» y sus sinónimos (o FALSE si la ambigüedad no puede ser resuelta)
	leader% is_here?  if
		\ El jefe de los refugiados tiene preferencia si está presente
		leader%  
	else
		\ Si Ulfius no ha hablado con Ambrosio (y por tanto aún no sabe su nombre), la palabra se refiere a Ambrosio:
\ antiguo!!!
\		ambrosio% ~conversations @ 0= abs ambrosio% *
		ambrosio% dup no_conversations? and
	then
	;
: (ambrosio) ( -- a | false )  \ Devuelve el ente adecuado a la palabra «ambrosio» (o FALSE si la ambigüedad no puede ser resuelta)
	\ La palabra «Ambrosio» es válida solo si el protagonista ha hablado con Ambrosio:
	ambrosio% dup conversations? and
	;

: (cave) ( -- a | false )  \ Devuelve el ente adecuado a la palabra «cueva» (o FALSE si la ambigüedad no puede ser resuelta)
	\ Inacabado!!!
	cave%
	;

\ }}}###########################################################
section( Vocabulario del juego)  \ {{{

also player_vocabulary definitions  \ Elegir el vocabulario PLAYER_VOCABULARY para crear en él las nuevas palabras

\ Pendiente!!! Añadir formas verbales en primera persona
\ Pendiente!!! Desambiguar formas verbales que son también nombres.
\ Pendiente!!! Añadir verbos en tercera persona (póngase, vaya, suba)

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

: cerrar  ['] do_close action!  ;
' cerrar synonyms{  cierra cerrad cierro }synonyms

: coger  ['] do_take action!  ;
' coger synonyms{
	agarrar agarra agarrad agarro agarre
	coge coged cojo coja
	recoger recoge recoged recojo recoja
	}synonyms

: tomar  ['] do_take|do_eat action!  ; \ Inacabado!!!
' tomar  synonyms{
	toma tomad tomo tome
	}synonyms

: dejar  ['] do_drop action!  ;
' dejar synonyms{
	deja dejad dejo deje
	soltar suelta soltad suelto suelte
	tirar tira tirad tiro tire
	}synonyms

: mirar  ['] do_look action!  ;
' mirar synonyms{
	m mira mirad miro mire
	contemplar contempla contemplad contemplo contemple
	observar observa observad observo observe
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
: salida  ['] do_exits exits% action|complement!  ;
' salida synonyms{  salidas  }synonyms

: examinar  ['] do_examine action!  ;
' examinar synonyms{  ex examina examinad examino examine  }synonyms

: examinarse  ['] do_examine action! protagonist% complement!  ;
' examinarse synonyms{
	examínese examinaos
	examinarte examínate examínete
	examinarme examíname examinadme examínome examíneme
	}synonyms

: registrar  ['] do_search action!  ;
' registrar synonyms{  registra registrad registro registre  }synonyms

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

: fabricar  ['] do_make action!  ;
' fabricar synonyms{
	fabrica fabricad fabrico fabrique
	construir construid construye construyo construya
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

: matar  ['] do_kill action!  ;
' matar synonyms{
	mata matad mato mate
	asesinar asesina asesinad asesino asesine
	aniquilar aniquila aniquilad aniquilo aniquile
	}synonyms
: golpear  ['] do_hit action!  ;
' golpear synonyms{
	golpea golpead golpeo golpee
	sacudir sacude sacudid sacudo sacuda
	}synonyms
: atacar  ['] do_attack action!  ;
' atacar synonyms{  
	ataca atacad ataco ataque
	agredir agrede agredid agredo agreda
	}synonyms
: romper  ['] do_break action!  ;
' romper synonyms{
	rompe romped rompo rompa
	despedazar despedaza despedazad despedazo despedace
	destrozar destroza destrozad destrozo destroce
	dividir divide dividid divido divida
	cortar corta cortad corto corte
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
: afilar  ['] do_sharpen action!  ;
' afilar synonyms{  afila afilad afilo afile  }synonyms
: partir  ['] do_go|do_break action!  ;
' partir synonyms{  parto partid parta  }synonyms
\ «parte» está en la sección final de ambigüedades
\ Pendiente!!!:
\ meter introducir insertar colar encerrar

: ulfius  ulfius% complement!  ;
: ambrosio  (ambrosio) complement!  ;
: hombre  (man) complement!  ;
' hombre synonyms{  señor tipo individuo persona  }synonyms
\ Ambigüedad!!!: jefe de los enemigos durante la batalla
: jefe  leader% complement!  ;
' jefe synonyms{
	líder refugiado viejo anciano abuelo
	}synonyms
: soldados  soldiers% complement!  ;
' soldados synonyms{
	guerreros luchadores combatientes camaradas
	compañeros oficiales suboficiales militares
	}synonyms
: refugiados  refugees% complement!  ;
' refugiados synonyms{
	refugiada refugiadas
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
	muchedumbre multitud
	}synonyms  
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
' banderas synonyms{  pendones enseñas  }synonyms
\ pendiente!!! estandartes, otro género
: pedernal  flint% complement!  ;
: ídolo  idol% complement!  ;
' ídolo synonyms{  ojo agujero  }synonyms
: llave  key% complement!  ;
: lago  lake% complement!  ;
' lago synonyms{  laguna agua  }synonyms  \ diferente género!!!
: candado  lock% complement!  ;
' candado synonyms{  cerrojo  }synonyms
\ Ambigüedad!!!: cierre
: tronco  log% complement!  ;
' tronco synonyms{  leño madero  }synonyms
\ pendiente!!! madera
: trozo  piece% complement!  ;
' trozo synonyms{  pedazo retal  }synonyms
: harapo  rags% complement!  ;
: rocas  rocks% complement!  ;
: serpiente  snake% complement!  ;
' serpiente synonyms{  reptil ofidio culebra  }synonyms
: piedra stone% complement!  ;
' piedra synonyms{  pedrusco  }synonyms
: espada  sword% complement!  ;
' espada synonyms{  tizona arma  }synonyms
\ "arma" es femenina pero usa artículo "él", contemplar en los cálculos de artículo!!!
: hilo  thread% complement!  ;
' hilo synonyms{  hebra  }synonyms
: antorcha  torch% complement!  ;
: cascada  waterfall% complement!  ;
' cascada synonyms{  catarata  }synonyms
: catre  bed% complement!  ;
' catre synonyms{  cama camastro  }synonyms
: velas  candles% complement!  ;
' velas synonyms{  vela  }synonyms
: mesa  table% complement!  ;
' mesa synonyms{  mesita pupitre  }synonyms
: puente  bridge% complement!  ;

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
' nubes synonyms{  nube estratocúmulo estratocúmulos cirro cirros  }synonyms
: suelo  floor% complement!  ;
' suelo synonyms{  suelos tierra firme  }synonyms
: cielo  sky% complement!  ;
' cielo synonyms{  cielos firmamento  }synonyms
: techo  ceiling% complement!  ;
: cueva  (cave) complement!  ;
' cueva synonyms{  caverna gruta  }synonyms
: enemigo  enemy% complement!  ;
' enemigo synonyms{ enemigos sajón sajones }synonyms

\ Meta

\ Antiguo!!!
\ : save  ['] do_save_the_game action!  ;

\ Términos ambiguos

: cierre
	action @  if  candado  else  cerrar  then
	;
: parte 
	action @  if  trozo  else  partir  then
	;
: hombres
	true  case
		pass_still_open?  of  soldados  endof
		battle?  of  soldados  endof
		location_28% am_i_there?  of  refugiados  endof
		location_29% am_i_there?  of  refugiados  endof
	endcase
	;
' hombres synonyms{ gente personas }synonyms

\ Comandos del sistema

: COLOREAR  ( -- )  \ Restaura los colores predeterminados
	init_colors  page  my_location describe
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
: GRABAR  ( "name" -- )  \ Graba el estado de la partida en un fichero
	[debug_parsing] ?halto" en GRABAR 1"  \ depuración!!!
	parse-name >csb
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
: CARGAR  ( "name" -- )  \ Carga el estado de la partida de un fichero
	[debug_parsing] ?halto" en CARGAR 1"  \ depuración!!!
	parse-name
	[debug_parsing] ?halto" en CARGAR 1a"  \ depuración!!!
	>csb
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
: FIN  ( -- )  do_finish  ;  \ Abandonar la partida
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
: AYUDA  ( -- )
	\ Pendiente!!! 
	;
' AYUDA synonyms{
	AYUDAR AYUDITA AYUDAS
	INSTRUCCIONES MANUAL GUÍA MAPA PLANO MENÚ
	PISTA PISTAS
	SOCORRO AUXILIO
	}synonyms

\ Comandos para usar durante el desarrollo!!!:
: forth  (do_finish)  ;
: bye  bye  ;
: quit  quit  ; 

: NOTFOUND  ( a u -- )  \ Tratar una palabra no encontrada
	\ Esta palabra será ejecutada automáticamente por SP-Forth
	\ tras no encontrar en el vocabulario una palabra del comando
	\ del jugador.
	[debug_parsing] ?halto" en notfound"  \ depuración!!!
	depth 2 >=  if  2drop  then   \ Borrarla
	;

restore_vocabularies

\ }}}###########################################################
section( Vocabulario para entradas «sí» o «no»)  \ {{{

0  [IF]  \ ......................................

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

[THEN]  \ ......................................

variable #answer  \ Su valor será 0 si no ha habido respuesta válida; negativo para «no»; y positivo para «sí»
: answer_undefined  ( -- )  \ Inicializa la variable antes de hacer la pregunta
	#answer off
	;
: think_it_again$  ( -- a u )  \ Devuelve un mensaje complementario para los errores
	s{ s" Piénsalo mejor"
	s" Decídete" s" Cálmate" s" Concéntrate"
	s" Presta atención"
	s{ s" Prueba" s" Inténtalo" }s again$ s&
	s" No es tan difícil" }s colon+
	;
: yes_but_no$  ( -- a u )  \ Devuelve mensaje de error: se dijo «no» tras «sí»
	s" ¿Primero «sí»" but|and$ s&
	s" después «no»?" s& think_it_again$ s&
	;
: no_but_yes$  ( -- a u )  \ Devuelve mensaje de error: se dijo «sí» tras «no»
	s" ¿Primero «no»" but|and$ s&
	s" después «sí»?" s& think_it_again$ s&
	;
: yes_but_no  ( -- )  \ Muestra error: se dijo «no» tras «sí»
	yes_but_no$ narrate
	;
' yes_but_no constant yes_but_no_error#
: no_but_yes  ( -- )  \ Muestra error: se dijo «sí» tras «no»
	no_but_yes$ narrate
	;
' no_but_yes constant no_but_yes_error#
: two_options_only$  ( -- a u )  \ Devuelve un mensaje que informa de las opciones disponibles
	^only$ s{ s" hay" s" tienes" }s&
	s" dos" s& s" respuestas" s" posibles" r2swap s& s& colon+
	s" «sí»" s" «no»" both& s" (o sus iniciales)" s& period+
	;
: two_options_only  ( -- )  \ Muestra error: sólo hay dos opciones
	two_options_only$ narrate
	;
' two_options_only constant two_options_only_error#
: wrong_yes$  ( -- a u )  \ Devuelve el mensaje usado para advertir de que se ha escrito mal «sí»
	s{ s" ¿Si qué...?" s" ¿Si...?" s" ¿Cómo «si»?" s" ¿Cómo que «si»?" }s
	s" No" s& s{
	s{ s" hay" s" puedes poner" }s 	s" condiciones" s&
	s{ s" hay" s" tienes" }s s" nada que negociar" s& }s&
	s{ s" aquí" s" en esto" s" en esta cuestión" }s& period+
	two_options_only$ s?&
	;
: wrong_yes  ( -- )  \ Muestra error: se ha usado la forma errónea «si»
	wrong_yes$ narrate
	;
' wrong_yes constant wrong_yes_error#
: answer_no  ( -- )  \ Anota una respuesta negativa
	#answer @ 0> yes_but_no_error# and throw  \ Provocar error si antes había habido síes
	#answer --
	;
: answer_yes  ( -- )  \ Anota una respuesta afirmativa
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
\ Pendiente!!!: ¿hacer que no se acepte ninguna otra palabra:
: NOTFOUND  ( a u -- )  depth 2 >=  if  2drop  then  ;

restore_vocabularies

\ }}}###########################################################
section( Entrada de comandos)  \ {{{

0  [IF]  \ ......................................

Para la entrada de comandos se usa la palabra de Forth
ACCEPT , que permite limitar el número máximo de caracteres
que serán aceptados (aunque por desgracia permite escribir
más y después trunca la cadena).

[THEN]  \ ......................................

\ Pendiente!!! calcular según el ancho de pantalla, la longitud del presto, si hay o no un salto de línea tras el presto, y la indentación...
80 constant /command  \ Longitud máxima de un comando
create command /command chars allot  \ Zona de almacenamiento del comando

: (wait_for_input)  ( -- a u )  \ Devuelve el comando introducido por el jugador
	input_color command /command accept  command swap  
	;
: command_prompt$  ( -- a u )  \ Devuelve el presto de entrada de comandos
	command_prompt count
	;
: .command_prompt  ( -- )  \ Imprime un presto para la entrada de comandos
	command_prompt_color indent command_prompt$ type
	system_color space
	;
: wait_for_input  ( -- a u )  \ Imprime un presto y devuelve el comando introducido por el jugador
	.command_prompt (wait_for_input)
	;
: listen  ( -- a u )  \ Espera y devuelve el comando introducido por el jugador, formateado
	[debug_info]  [IF]  0$ debug  [THEN]  \ Depuración!!!
	wait_for_input  -punctuation
	[debug]  [IF]  cr cr ." <<<" 2dup type ." >>>" cr cr  [THEN]  \ Depuración!!!
	; 

\ }}}###########################################################
section( Entrada de respuestas de tipo «sí o no»)  \ {{{

: yes|no  ( a u -- n )  \ Evalúa una respuesta a una pregunta del tipo «sí o no»
	\ a u = Respuesta a evaluar
	\ n = Resultado (un número negativo para «no» y positivo para «sí»; cero si no se ha respondido ni «sí» ni «no», o si se produjo un error)
	answer_undefined
	[lina?]
	[if]    postpone only answer_vocabulary
	[else]  only answer_vocabulary
	[then]
	['] evaluate catch
	dup  if  nip nip  then  \ Reajustar la pila si ha habido error
	dup ?wrong 0=  \ Ejecutar el posible error y preparar su indicador para usarlo en el resultado
	#answer @ 0= two_options_only_error# and ?wrong  \ Ejecutar error si la respuesta fue irreconocible
	#answer @ dup 0<> and and  \ Calcular el resultado final
	restore_vocabularies
	;
svariable question
: .question  ( -- )  \ Imprime la pregunta
	question_color question count paragraph
	;
: answer  ( a u -- n )  \ Devuelve la respuesta a una pregunta del tipo «sí o no»
	\ a u = Pregunta
	\ n = Respuesta: un número negativo para «no» y positivo para «sí»
	question s!
	begin
		.question wait_for_input  yes|no ?dup
	until
	;
: yes?  ( a u -- ff )  \ ¿Es afirmativa la respuesta a una pregunta?
	\ a u = Pregunta
	\ ff = ¿Es la respuesta positiva?
	answer 0>
	;
: no?  ( a u -- ff )  \ ¿Es negativa la respuesta a una pregunta?
	\ a u = Pregunta
	\ ff = ¿Es la respuesta negativa?
	answer 0<
	;

\ }}}###########################################################
section( Fin)  \ {{{

: success?  ( -- ff )  \ ¿Ha completado con éxito su misión el protagonista?
	my_location location_51% =
	;
false  [IF]
: battle_phases  ( -- u )  \ Devuelve el número máximo de fases de la batalla
	5 random 7 +  \ Número al azar, de 8 a 11
	;
[THEN]
: failure?  ( -- ff )  \ ¿Ha fracasado el protagonista?
	battle# @ battle_phases >
	;
: .bye  ( -- )  \ Mensaje final cuando el jugador no quiere jugar otra partida
	\ Provisional!!!
	s" ¡Adiós!" narrate
	;
: bye_bye  ( -- )  \ Abandona el programa
	new_page .bye bye
	;
: play_again?$  ( -- a u )  \ Devuelve la pregunta que se hace al jugador tras haber completado con éxito el juego
	s{ s" ¿Quieres" s" ¿Te animas a" s" ¿Te apetece" }s
	s{ s" jugar" s" empezar" }s&  again?$ s&
	;
: retry?1$  ( -- a u )  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado
	s" ¿Tienes"
	s{ s" fuerzas" s" arrestos" s" agallas" s" energías" s" ánimos" }s&
	;
: retry?2$  ( -- a u )  \ Devuelve una variante para el comienzo de la pregunta que se hace al jugador tras haber fracasado
	s{ s" ¿Te quedan" s" ¿Guardas" s" ¿Conservas" }s
	s{ s" fuerzas" s" energías" s" ánimos" }s&
	;
: retry?$  ( -- a u )  \ Devuelve la pregunta que se hace al jugador tras haber fracasado
	s{ retry?1$ retry?2$ }s s" para" s&
	s{ s" jugar" s" probar" s" intentarlo" }s& again?$ s&
	;
: enough?  ( -- ff )  \ ¿Prefiere el jugador no jugar otra partida?
	success?  if  play_again?$  else  retry?$  then  cr+ no?
	;
: surrender?  ( -- ff )  \ ¿Quiere el jugador dejar el juego?
	\ No se usa!!!
	s{
	s" ¿Quieres"
	s" ¿En serio quieres"
	s" ¿De verdad quieres"
	s" ¿Estás segur" player_gender_ending$+ s" de que quieres" s& 
	s" ¿Estás decidid" player_gender_ending$+ s" a" s&
	}s s{
	s" dejarlo?"
	s" rendirte?"
	s" abandonar?"
	}s&  yes? 
	;
: game_over?  ( -- ff )  \ ¿Se terminó ya el juego?
	success? failure? or
	;
: the_happy_end  ( -- )  \ Final del juego con éxito
	s" Agotado, das parte en el castillo de tu llegada"
	s" y de lo que ha pasado." s&
	narrate  narration_break
	s" Pides audiencia al rey, Uther Pendragon."
	narrate  scene_break
	castilian_quotes? @
	if  \ Comillas castellanas
		s" El rey" rquote$ s+ s" , te indica el valido," s+
		lquote$ s" ha ordenado que no se lo moleste," s+ s&
	else  \ Raya
		s" El rey" 
		dash$ s" te indica el valido" dash$ comma+ s+ s+ s&
		s" ha ordenado que no se lo moleste," s&
	then
	s" pues sufre una amarga tristeza." s&
	speak  narration_break
	s" No puedes entenderlo. El rey, tu amigo."
	narrate  narration_break
	s" Agotado, decepcionado, apesadumbrado,"
	s" decides ir a dormir a tu casa." s&
	s" Es lo poco que puedes hacer." s&
	narrate  narration_break
	s" Te has ganado un buen descanso."
	narrate
	;
: enemy_speech  ( -- )  \ Palabras del general enemigo
	s" Su general, sonriendo ampliamente, dice:" s&
	narrate  narration_break
	s{
	s" Hoy es"
	s" Hoy parece ser" 
	s" Hoy sin duda es"
	s" No cabe duda de que hoy es"
	}s s" mi día de suerte..." s&
	s{ s" Bien, bien..." s" Excelente..." }s&
	s{
	s" Por el gran Ulfius podremos pedir un buen rescate"
	s" Del gran Ulfius podremos sacar una buena ventaja"
	}s& period+  speak
	;
: the_sad_end  ( -- )  \ Final del juego con fracaso
	s" Los sajones te"
	s{ s" hacen prisionero" s" capturan" s" atrapan" }s& period+
	enemy_speech
	;
: the_end  ( -- )  \ Mensaje final del juego
	success?  if  the_happy_end  else  the_sad_end  then
	scene_break 
	;
:action (do_finish)  ( -- )  \ Abandonar el juego
	restore_vocabularies system_color cr .forth quit
	;action
:action do_finish  ( -- )  \ Acción de abandonar el juego
	surrender?  if
		\ retry?$  cr+ no?  if  (do_finish)  then
		(do_finish)
	then
	;action

\ }}}###########################################################
section( Acerca del programa)  \ {{{

: based_on  ( -- )  \ Muestra un texto sobre el programa original
	s" «Asalto y castigo» está basado"
	s" en el programa homónimo escrito en BASIC en 2009 por" s&
	s" Baltasar el Arquero (http://caad.es/baltasarq/)." s&
	paragraph
	;
: license  ( -- )  \ Muestra un texto sobre la licencia
	s" (C) 2011 Marcos Cruz (programandala.net)" paragraph
	s" «Asalto y castigo» es un programa libre;"
	s" puedes distribuirlo y/o modificarlo bajo los términos de" s&
	s" la Licencia Pública General de GNU, tal como está publicada" s&
	s" por la Free Software Foundation (Fundación para los Programas Libres)," s&
	s" bien en su versión 2 o, a tu elección, cualquier versión posterior" s&
	s" (http://gnu.org/licenses/)." s& \ Confirmar!!!
	paragraph
	;
: program  ( -- )  \ Muestra el nombre y versión del programa
	s" «Asalto y castigo» (escrito en SP-Forth)" paragraph/
	s" Versión " version$ s& paragraph/
	;
: about  ( -- )  \ Muestra información sobre el programa
	new_page about_color
	program	cr license cr based_on
	scene_break
	;

\ }}}###########################################################
section( Introducción)  \ {{{

: intro_0  ( -- )  \ Muestra la introducción al juego (parte 0)
	s" El sol despunta de entre la niebla,"
	s" haciendo humear los tejados de paja." s&
	narrate  narration_break
	;
: intro_1  ( -- )  \ Muestra la introducción al juego (parte 1)
	s" Piensas en"
	s{ s" el encargo de"
	s" la" s{ s" tarea" s" misión" }s& s" encomendada por" s&
	s" la orden dada por" s" las órdenes de" }s&
	s{ s" Uther Pendragon" s" , tu rey" s?+ s" tu rey" }s& \ tmp!!!
	s" ..." s+
	narrate  narration_break
	;
: intro_2  ( -- )  \ Muestra la introducción al juego (parte 2)
	s{ s" Atacar" s" Arrasar" }s s" una" s&
	s" aldea" s{ s" tranquila" s" pacífica" }s r2swap s& s&
	s" , aunque" s+ s{ s" sea una" s" esté" }s&
	s{ s" llena de" s" habitada por" s" repleta de" }s&
	s" sajones, no te" s&{ s" llena" s" colma" }s&
	s" de orgullo." s&
	narrate  narration_break
	;
: intro_3  ( -- )  \ Muestra la introducción al juego (parte 3)
	s" Los hombres se" s{ s" ciernen" s" lanzan" }s&
	s" sobre la aldea, y la destruyen." s&
	s" No hubo tropas enemigas, ni honor en" s&
	s{ s" la batalla." s" el combate." }s&
	narrate  scene_break
	;
: intro_4  ( -- )  \ Muestra la introducción al juego (parte 4)
	sire,$ s{
	s" el asalto" s" el combate" s" la batalla"
	s" la lucha" s" todo" s" la misión"
	}s& s{ s" ha terminado." s" ha concluido." }s&
	speak
	;
: needed_orders$  ( -- a u )  \ Devuelve una variante de «órdenes necesarias»
	s" órdenes" s{ 0$ s" necesarias" s" pertinentes" }s&
	;
: intro_5  ( -- )  \ Muestra la introducción al juego (parte 5)
	s" Lentamente," s{
	s" ordenas"
	s" das la orden de"
	s" das las" needed_orders$ s& s" para" s&
	}s& to_go_back$ s& s" a casa." s&
	narrate  narration_break
	;
: intro_6  ( -- )  \ Muestra la introducción al juego (parte 6)
	[false]  [IF]  \ Primera versión
		^officers_forbid_to_steal$ period+
	[ELSE]  \ Segunda versión
		soldiers_steal_spite_of_officers$ period+ 	
	[THEN]
	narrate  scene_break
	;
: intro  ( -- )  \ Muestra la introducción al juego 
	new_page
	intro_0 intro_1 intro_2 intro_3 intro_4 intro_5 intro_6
	;

\ }}}###########################################################
section( Principal)  \ {{{

: init/once  ( -- )  \ Preparativos que hay que hacer solo una vez, antes de la primera partida
	restore_vocabularies
	init_screen  \ Pantalla
	;
: init/game  ( -- )  \ Preparativos que hay que hacer antes de cada partida
	randomize
	init_entities init_plot get_config
	\ Anular esto para depuración!!!:
	\ about cr intro  
	location_01% enter

	\ Activar esto selectivamente para depuración!!!:
	\ location_08% enter  \ Emboscada 
	\ location_47% enter
	\ location_01% enter
	\ snake% is_here
	\ ambrosio% is_here
	;
: game  ( -- )  \ Bucle de la partida
	begin
		plot  listen obbey
	game_over? until
	;
: (main)  ( -- )  \ Bucle principal del juego
	init/once
	begin
		init/game game the_end
	enough? until
	bye_bye
	;
' (main) is main
' main alias ayc
' main alias go

: i0  ( -- )  \ Hace toda la inicialización; para depuración!!!
	init/once init/game
	." Datos preparados."
	;

\ i0 cr  \ Para depuración!!!
\ main

\ }}}###########################################################
section( Pruebas)  \ {{{

0  [IF]  \ ......................................

Esta sección contiene código para probar el programa
sin interactuar con el juego, para detectar mejor posibles
errores.

[THEN]  \ ......................................

: check_stack1  ( -- )  \ Provoca un error -3 («stack overflow») si la pila no tiene solo un elemento
	depth 1 <> -3 and throw
	;
: check_stack  ( -- )  \ Provoca un error -3 («stack overflow») si la pila no está vacía
	depth 0<> -3 and throw
	;
: test_location_description  ( a -- )  \ Comprueba todas las descripciones de un ente escenario
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
\	cr ." == Mirar hacia fuera:" cr
\	out% describe_direction check_stack
\	cr ." == Mirar hacia dentro:" cr
\	in% describe_direction check_stack
	;
0 value tested
: test_description  ( a -- )  \ Comprueba la descripciones de un ente 
	to tested
	cr ." = Nombre =========================" cr
	tested full_name type
	cr ." = Descripción ====================" cr
	tested describe check_stack
	tested is_location?  if  tested test_location_description  then
	;
: test_descriptions  ( -- )  \ Comprueba la descripción de todos los entes
	#entities 0  do
		i #>entity test_description
	loop
	;
: test_battle_phase  ( u -- ) \ Comprueba una fase de la batalla
	32 0  do  \ 32 veces cada fase, porque los textos son aleatorios
		dup (battle_phase) check_stack1
	loop  drop
	;
: test_battle  ( -- )  \ Comprueba todas las fases de la batalla
	battle_phases 0  do
		i test_battle_phase
	loop
	;

\eof  \ Final del programa; el resto del fichero es ignorado 

\ }}}##########################################################
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
Dudas sobre SP-Forth

¿Cómo saber la dirección de la pila? No hay SP0 .

¿Cómo saber el tamaño de la pantalla? No hay FORM .

¿Cómo borrar la pantalla? No hay PAGE .

¿Cómo saber si estoy en SP-Forth o en otro Forth?

\ }}}##########################################################
\ Tareas pendientes: programación {{{

...........................

Arreglar el problema de lina con los vocabularios:

No se puede hacer ONLY GAME_VOCABULARY porque al pasar a
ONLY ya no está visible GAME_VOCABULARY , que se creó en
FORTH . ANS Forth dice que SET-ORDER (que permitiría una
solución) debe estar disponible tras ONLY pero no está.

Esto impide seleccionar un vocabulario único para
hacer EVALUATE con el comando del jugador.

Solución 1:

Hacer que los vocabularios estén en el vocabulario ONLY
(ahora solo está FORTH ). ¿Cómo? ¿Parcheando las
definiciones?  Es delicado porque son listas enlazadas...

Solución 2:

Usar (FIND) para buscar en un diccionario concreto:

s" palabra_buscada" ' game_vocabulary >wid (find)

Esto obliga a usar un intérprete propio en lugar de
EVALUATE , lo que es una pena.

Solución 3:

Escribir SET-ORDER . Esto requiere investigar cómo
hace lina su lista de vocabularios.

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

Grave: los comandos no vacíos y sin verbo reconocido
hacen saltar el sistema.

...........................

Poner de un color diferente, configurable, el presto y el
texto de las respuestas al sistema (proguntas sí/no).

...........................

Los comandos de configuración no evitan que el análisis dé
error por falta de comandos del juego!

Esto es fácil de arreglar:

¿Hacer que anulen todo lo que siga?
¿O que continúen como si fuera un comando nuevo?
O mejor: simplemente rellenar ACTION con un xt
de una acción que no haga nada!

No! Lo que hay que hacer es ejecutar las acciones de
configuración como el resto de acciones, metiendo su xt en
ACTION .  Y si después queremos seguir (dependerá de la
acción de sistema de que se trata) basta poner ACTION a cero
otra vez. O se puede leer el resto del comando, para
anularlo!

...........................

Comprobar si el hecho de no usar el número máximo de líneas
causa problemas con diferentes tamaños de consola.

Los textos son cortos, de modo que no hay riesgo de
que se pierdan antes poder leerlos, antes de que
se pida entrada a un comando.

...........................

Hacer un comando que lea el fichero de
configuración en medio de una partida.

...........................

Evitar mensaje «todos tus hombres siguen tus pasos» en la
aldea, nada más empezar. Usar otra frase mientras dura el
saqueo. Reescribir ese texto bien, entre la intro y la
aldea.

...........................

Implementar transcripción en fichero

...........................
Anotar que ha habido paalabras no reconocidas, para variar el error en lugar de actuar como si faltaran.
p.e. mirar / mirar xxx.


...........................


Hacer más naturales los mensajes que dicen
que no hay nada de interés en la dirección indicada,
p.e.,
miras hacia
intentas vislumbrar (en la cueva oscura)
contemplas el cielo...
miras a tus pies...

...........................

comando de sistema para restaurar los colores predeterminados

COLORES

...........................

Añadir variante:
«No observas nada digno de mención al mirar hacia el Este».

...........................

Añadir «tocar».

...........................

Implementar que «todo» pueda usarse
con examinar y otros verbos, y se cree una lista
ordenada aleatoriamente de entes que cumplan
los requisitos.

...........................

Hacer que los objetos (y ambrosio) no estén siempre en el
mismo sitio. ¿Altar? ¿Serpiente?

...........................


Hacer algo así en las tramas del laberinto:

(una vez de x se equivoca)

: this_place_seems_familiar
	my_location is_visited?
	if
		s" Este sitio me suena"
	then
	;

...........................

Respuesta a mirar como en «Pronto»:

Miras, pero no ves eso por aquí. ¿Realmente importa?

...........................

Crear ente «enemigo» con el término ambiguo «sajones» (por
los sajones muertos en la aldea.

...........................

Crear ente (sub)oficiales, con descripción complementaria a
la de los soldados.

...........................

Crear ente «general» para el general enemigo, con
descripción durante la batalla, dependiendo de la fase.

...........................

Implementar «describir», sinónimo de examinar para entes
presentes pero que funciona con entes no presentes ya
conocidos!

...........................

Implementar «esperar» («z»)

...........................

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

Hace que «examinar» sin más examine todo.

¿Y también «coger» y otros?

coger sin objeto buscaría qué hay.
si solo hay una cosa para coger, la coge.
si hay varias, error.


...........................

Error nuevo para no coger las cosas de la casa de Ambrosio:
Es mejor dejar las cosas de Ambrosio donde están.

Añadir a la ficha con su xt.

...........................

Solucionar el eterno problema de los sinónimos que no tienen
el mismo género o número...

La palabra del vocabulario podría ponerse a sí misma como
nombre del ente... Pero esto obligaría a usar el género
y número de la ficha en las descripciones.

Algo relacionado: "arma" es femenina pero usa artículo "él";
contemplar en los cálculos de artículo!!!

Mirar cómo lo solucioné en «La legionela del pisto»: con una
lista de nombres separada de los datos de entes.

...........................

¿Crear un método para dar de alta fácilmente entes
decorativos? Hay muchos en las descripciones de los
escenarios.

...........................

Hacer que no salga el presto de pausa si las pausas son
cero.

...........................

Hacer variantes de CHOOSE y DCHOOSE para elegir un elemento
con un cálculo en lugar de al azar. 

¿En dónde se necesitaba?

...........................

Crear un mensaje de error más elaborado para las acciones
que precisan objeto directo, con el infinitivo como
parámetro: «¿Matar por matar?» «Normalmente hay que matar a
alguien o algo».

...........................

Implementar pronombres. Para empezar, que la forma «mírate»
sea compatible con «mírate la capa». Para esto habría que
distiguir dos variantes de complemento directo, y que al
asignar cualquiera de ellas se compruebe si había ya otro
complemento directo del otro tipo.

...........................

Limitar los usos de PRINT_STR a la impresión. Renombrarla.
Crear otra cadena dinámica para los usos genéricos con «+ y
palabras similares.

...........................

Comprobar los usos de TMP_STR .

...........................

Poner en fichero de configuración el número de líneas
necesario para mostrar un presto de pausa.

...........................

Implementar opción para tener en cuenta las palabras no
reconocidas y detener el análisis.

...........................

Poner en fichero de configuración si las palabras no
reconocidas deben interrumpir el análisis.

...........................

Poner todos los textos relativos al protagonista en segunda 
persona.

(Creo que ya está hecho).

...........................

Añadir las salidas hacia fuera y dentro. Y atrás. Y
adelante. Y seguir.

...........................

Implementar el recuerdo de la dirección del último
movimiento.

...........................

Hacer que «salir», si no hay dirección de salida en el ente,
calcule la dirección con la del último movimiento.

...........................

Añadir a la configuración si los errores lingüísticos deben
ser detallados (técnicos) o vagos (narrativos) o ambos.

...........................

Hacer que primero se muestre la introducción y después
los créditos y el menú.

...........................



...........................

\ }}}##########################################################
\ Ideas desestimadas para este proyecto {{{

...........................
Lista de puzles completados (como Transilvania Corruption).

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
de escenario (como actual, que se activa en la descripción)
de tramas de entrada, salida o permanencia en escenario...
Haría falta un selector similar a SIGHT para seleccionar el
caso adecuado en la palabra LOCATION_PLOT

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

\ }}}##########################################################
\ Tareas pendientes: trama y puzles {{{

Activar la cueva cuando se mira al sur o se examina la pared
de roca

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

\ }}}##########################################################
\ Tareas pendientes: código fuente {{{

}}}
0  [IF]  \ ......................................
[THEN]  \ ......................................

