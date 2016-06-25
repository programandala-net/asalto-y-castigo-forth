\ ayc.ini.fs

\ Fichero de configuración de:
\ «Asalto y castigo»
\ Copyright (C) 2011,2012,2013,2016 Marcos Cruz (programandala.net)

\ Página del programa:
\ http://programandala.net/es.programa.asalto_y_castigo.forth

\ Última modificación: 201606242015

\ --------------------------------------------------------------
\ Observaciones

(

== Formato de este fichero ==

Este fichero contiene código en Forth y será interpretado como tal
directamente por el intérprete de Forth cada vez que se inicie una
nueva partida del juego.

Durante la interpretación de este fichero sólo estará activo el
vocabulario de configuración del juego, que consta de las
palabras en español creadas para la tarea, así como la palabra
para crear cadenas de texto y las dos palabras clásicas de Forth
para hacer comentarios: el paréntesis y la barra invertida.

Cualquier palabra no reconocida y que no pueda ser convertida en
un número decimal provocará un error.

== Cadenas de texto ==

Para indicar una cadena de texto se usa la palabra estándar de
Forth `s"`, que como siempre debe ir separada con espacios. La
propia cadena empieza tras el primer espacio de separación
posterior, y por ello incluirá otros espacios iniciales si los
hubiera; termina con las primeras comillas dobles, por lo que no
puede incluir este signo.

== Comentarios en este fichero ==

Téngase en cuenta que tanto la apertura de paréntesis como la
barra invertida son palabras de Forth como las demás, es decir,
deben estar separadas del texto circundante  por espacio, un
tabulador o un salto de línea.

La barra invertida desecha el código fuente que la suceda, hasta
el próximo final de línea; la apertura de paréntesis hace lo
mismo hasta el siguiente cierre de paréntesis, lo que permite
crear comentarios de bloque, como se hace para este mismo texto
de ayuda.

== Codificación de este fichero ==

La codificación de este fichero es UTF-8 y no debe cambiarse,
pues de otro modo las palabras de configuración que llevan
tildes no serían reconocidas por el intérprete de Forth.

== Configuración incluida en este fichero ==

Este fichero contiene un ejemplo de configuración que coincide
con la configuración predeterminada en el juego, la cual se
activa antes de leer este fichero.  Por tanto, las
configuraciones que no se incluyan en este fichero conservarán
el valor predeterminado que les asigna el programa.

== Cómo mantener varias configuraciones ==

Para mantener varias configuraciones basta que la que queramos
usar quede la última en este fichero, y de ese modo sustituirá a
las anteriores.

También es posible anular las configuraciones no deseadas
encerrándolas entre paréntesis, como se hace en los ejemplos de
combinaciones de colores alternativas que se ofrecen al final de
este fichero.

)  \ Fin de los comentarios

\ --------------------------------------------------------------
\ Sexo del jugador (no del personaje protagonista)
\ --------------------------------------------------------------

\ Solo se usa para mostrar adecuadamente algunos textos que van
\ dirigidos al jugador, como «¿Estás segura de que quieres
\ terminar?» y similares.

\ Opciones: mujer o femenino, varón o masculino.

varón

\ --------------------------------------------------------------
\ Intérprete de comandos
\ --------------------------------------------------------------

\ Si no se especifica (o no se reconoce) una acción en el
\ comando, ¿repetir la del comando anterior?
\
\ Opciones: sí y no.

sí repetir_la_última_acción

\ --------------------------------------------------------------
\ Mensajes de error
\ --------------------------------------------------------------

\ Detalle de los mensajes de error lingüístico
\ (Lo errores lingüísticos son los que se producen durante el
\ análisis del comando del jugador).
\ Opciones:
\   0 = no se mostrará ningún mensaje
\   1 = se mostrará un mensaje genérico configurable
\   2 = se mostrará un mensaje específico detallado

2 detalle_de_los_mensajes_de_error_lingüístico

\ Mensaje genérico de error lingüístico, usado cuando el detalle
\ de los mensajes de de error lingüístico es 1.

s" Orden incorrecta." mensaje_genérico_de_error_lingüístico

\ Detalle de los mensajes de error operativo.
\ (Los errores operativos son los que se producen
\ intentando ejecutar la acción especificada
\ por el comando del jugador).
\ Opciones:
\   0 = no se mostrará ningún mensaje
\   1 = se mostrará un mensaje genérico configurable
\   2 = se mostrará un mensaje específico detallado

2 detalle_de_los_mensajes_de_error_operativo

\ Mensaje genérico de error operativo, usado cuando el detalle
\ de los mensajes de de error operativo es 1.

s" No es posible hacer eso." mensaje_genérico_de_error_operativo

\ --------------------------------------------------------------
\ Formato de las citas de los diálogos
\ --------------------------------------------------------------

\ ¿Usar comillas castellanas en lugar de raya?
\ Opciones: sí y no.

sí comillas

\ --------------------------------------------------------------
\ Párrafos
\ --------------------------------------------------------------

\ Espacios en blanco que tendrá la indentación
\ de la primera línea de cada párrafo.
\ Opciones: de 0 a 8.

2 espacios_de_indentación

\ ¿Añadir una línea en blanco
\ para separar cada párrafo del siguiente?
\ Opciones: sí y no.

no separar_párrafos

\ ¿Indentar la primera línea de un párrafo también
\ cuando coincide con la primera línea de la pantalla?
\ Opciones: sí y no.

sí indentar_primera_línea_de_pantalla

\ --------------------------------------------------------------
\ Prestos
\ --------------------------------------------------------------

\ Un presto o inductor (en inglés «prompt») es una marca gráfica
\ convencional que sirve para indicar que el programa está
\ preparado para recibir la entrada del usuario y señalar el
\ lugar de la pantalla en que se mostrará.

\ ¿Indentar los prestos de pausa como si de la primera
\ línea de un párrafo se tratara o mostrarlos en el margen?
\ Los prestos de pausa son todos menos el de comando.
\ Opciones: sí y no.

sí indentar_prestos_de_pausa

\ Contenido de los prestos.

s" ..." presto_de_pantalla_llena
s" ..." presto_de_pausa_de_narración
s" ..." presto_de_fin_de_escena
s" >" presto_de_comando

\ ¿Separar el presto de comando con un espacio posterior?
\ Opciones: sí y no.

sí espacio_tras_presto_de_comando

\ ¿Hacer un salto de línea tras el presto de comando
\ (lo que haría indiferente el espacio de separación)?
\ Opciones: sí y no.

no nueva_línea_tras_presto_de_comando

\ --------------------------------------------------------------
\ Pausas
\ --------------------------------------------------------------

\ Las pausas del juego pueden interrumpirse pulsando una tecla.
\ La configuración siguiente en segundos indica por tanto la
\ duración máxima de la pausa si no se pulsa una tecla,
\ salvo un valor negativo, que significa pausa indefinida.

\ Segundos que durarán las pausas de narración
\ usadas al final de ciertos párrafos
\ (si es valor es negativo, se mostrará el presto
\ y habrá que pulsar una tecla para continuar):

4 segundos_en_pausas_de_narración

\ Segundos que durarán las pausas de final de escena
\ (si es valor es negativo, se mostrará el presto
\ y habrá que pulsar una tecla para continuar):

8 segundos_en_pausas_de_final_de_escena

\ --------------------------------------------------------------
\ Borrado de la pantalla
\ --------------------------------------------------------------

\ ¿Borrar la pantalla
\ tras entrar en un escenario o antes de describirlo?
\ Opciones: sí y no.

sí borrar_pantalla_para_escenarios

\ ¿Borrar la pantalla
\ tras el final de una escena?
\ Opciones: sí y no.

no borrar_pantalla_para_escenas

\ --------------------------------------------------------------
\ Combinaciones de colores
\ --------------------------------------------------------------

\ Se puede elegir colores de pluma y de papel para cada tipo de
\ texto del juego.

\ No es posible cambiar el color del cursor, que será el
\ configurado en la terminal del sistema operativo. Dependerá de
\ la configuración de la terminal.

\ xxx todo Comprobar esto:
\ El color de fondo de la pantalla es configurable pero con una
\ limitación: cuando la pantalla se enrolla el color del sistema
\ se utilizará en las nuevas líneas que aparezcan por debajo.

\ Los colores disponibles son los siguientes:
\     amarillo
\     azul
\     azul_claro
\     blanco
\     cian
\     cian_claro
\     gris
\     gris_claro
\     magenta
\     magenta_claro
\     marrón
\     negro
\     rojo
\     rojo_claro
\     verde
\     verde_claro
\ (No se admiten las formas «cyan» y «cyan_claro»).

\ Como ya se ha dicho respecto a la configuración, también los
\ colores elegidos en este fichero coinciden con los
\ predeterminados en el juego, que son similares a los de la
\ versión original para ZX Spectrum.  Pero más abajo se ofrecen
\ configuraciones de color alternativas.

\ --------------------------------------------------------------

\ **** Estilo predeterminado ****

negro papel_de_fondo

\ Colores de los títulos de crédito y la licencia:

gris tinta_de_créditos
negro papel_de_créditos

\ Colores de la narración:

gris tinta_de_narración
negro papel_de_narración

\ Colores del presto de comandos:

cian tinta_de_presto_de_comandos
negro papel_de_presto_de_comandos

\ Colores del texto de entrada:

cian_claro tinta_de_entrada
negro papel_de_entrada

\ Colores del nombre de escenario, antes de su descripción:

negro tinta_de_nombre_de_escenario
verde papel_de_nombre_de_escenario

\ Colores de la descripción de un escenario:

verde tinta_de_descripción_de_escenario
negro papel_de_descripción_de_escenario

\ Colores de las restantes descripciones:

gris tinta_de_descripción
negro papel_de_descripción

\ Colores del presto de pantalla llena:

verde tinta_de_presto_de_pantalla_llena
negro papel_de_presto_de_pantalla_llena

\ Colores del presto de pausa de narración:

verde tinta_de_presto_de_pausa_de_narración
negro papel_de_presto_de_pausa_de_narración

\ Colores del presto de final de escena:

verde tinta_de_presto_de_escena
negro papel_de_presto_de_escena

\ Colores de las citas de diálogos:

gris_claro tinta_de_diálogos
negro papel_de_diálogos

\ Colores de las preguntas de tipo sí o no:

blanco tinta_de_pregunta
negro papel_de_pregunta

\ Colores de los mensajes de error del analizador:

rojo_claro tinta_de_error_lingüístico
negro papel_de_error_lingüístico

\ Colores de los mensajes de error de los comandos:

rojo tinta_de_error_operativo
negro papel_de_error_operativo

\ Colores de los mensajes de error del sistema:

rojo_claro tinta_de_error_del_sistema
negro papel_de_error_del_sistema

\ --------------------------------------------------------------
\ Ejemplos de combinaciones de colores

\ **** Estilo «ceniza» ****

(
negro papel_de_fondo
gris tinta_de_créditos
negro papel_de_créditos
gris tinta_de_narración
negro papel_de_narración
negro tinta_de_presto_de_comandos
gris papel_de_presto_de_comandos
gris tinta_de_entrada
negro papel_de_entrada
negro tinta_de_nombre_de_escenario
gris papel_de_nombre_de_escenario
gris tinta_de_descripción_de_escenario
negro papel_de_descripción_de_escenario
gris tinta_de_descripción
negro papel_de_descripción
gris tinta_de_presto_de_pantalla_llena
negro papel_de_presto_de_pantalla_llena
gris tinta_de_presto_de_pausa_de_narración
negro papel_de_presto_de_pausa_de_narración
gris tinta_de_presto_de_escena
negro papel_de_presto_de_escena
gris tinta_de_diálogos
negro papel_de_diálogos
gris tinta_de_pregunta
negro papel_de_pregunta
gris tinta_de_error_lingüístico
negro papel_de_error_lingüístico
gris tinta_de_error_operativo
negro papel_de_error_operativo
gris tinta_de_error_del_sistema
negro papel_de_error_del_sistema
)

\ **** Estilo «monitor de fósforo verde» ****

(
negro papel_de_fondo
verde tinta_de_créditos
negro papel_de_créditos
verde tinta_de_narración
negro papel_de_narración
negro tinta_de_presto_de_comandos
verde papel_de_presto_de_comandos
verde tinta_de_entrada
negro papel_de_entrada
negro tinta_de_nombre_de_escenario
verde papel_de_nombre_de_escenario
verde tinta_de_descripción_de_escenario
negro papel_de_descripción_de_escenario
verde tinta_de_descripción
negro papel_de_descripción
verde tinta_de_presto_de_pantalla_llena
negro papel_de_presto_de_pantalla_llena
verde tinta_de_presto_de_pausa_de_narración
negro papel_de_presto_de_pausa_de_narración
verde tinta_de_presto_de_escena
negro papel_de_presto_de_escena
verde tinta_de_diálogos
negro papel_de_diálogos
verde tinta_de_pregunta
negro papel_de_pregunta
verde tinta_de_error_lingüístico
negro papel_de_error_lingüístico
negro tinta_de_error_operativo
verde papel_de_error_operativo
verde tinta_de_error_del_sistema
negro papel_de_error_del_sistema
)

\ **** Estilo «monitor de fósforo ámbar» ****

(
negro papel_de_fondo
marrón tinta_de_créditos
negro papel_de_créditos
marrón tinta_de_narración
negro papel_de_narración
negro tinta_de_presto_de_comandos
marrón papel_de_presto_de_comandos
marrón tinta_de_entrada
negro papel_de_entrada
negro tinta_de_nombre_de_escenario
marrón papel_de_nombre_de_escenario
marrón tinta_de_descripción_de_escenario
negro papel_de_descripción_de_escenario
marrón tinta_de_descripción
negro papel_de_descripción
marrón tinta_de_presto_de_pantalla_llena
negro papel_de_presto_de_pantalla_llena
marrón tinta_de_presto_de_pausa_de_narración
negro papel_de_presto_de_pausa_de_narración
marrón tinta_de_presto_de_escena
negro papel_de_presto_de_escena
marrón tinta_de_diálogos
negro papel_de_diálogos
marrón tinta_de_pregunta
negro papel_de_pregunta
marrón tinta_de_error_lingüístico
negro papel_de_error_lingüístico
marrón tinta_de_error_operativo
negro papel_de_error_operativo
marrón tinta_de_error_del_sistema
negro papel_de_error_del_sistema
)

\ **** Estilo «desierto» ****

(
amarillo papel_de_fondo
marrón tinta_de_créditos
amarillo papel_de_créditos
marrón tinta_de_narración
amarillo papel_de_narración
amarillo tinta_de_presto_de_comandos
marrón papel_de_presto_de_comandos
marrón tinta_de_entrada
amarillo papel_de_entrada
amarillo tinta_de_nombre_de_escenario
marrón papel_de_nombre_de_escenario
marrón tinta_de_descripción_de_escenario
amarillo papel_de_descripción_de_escenario
marrón tinta_de_descripción
amarillo papel_de_descripción
marrón tinta_de_presto_de_pantalla_llena
amarillo papel_de_presto_de_pantalla_llena
marrón tinta_de_presto_de_pausa_de_narración
amarillo papel_de_presto_de_pausa_de_narración
marrón tinta_de_presto_de_escena
amarillo papel_de_presto_de_escena
marrón tinta_de_diálogos
amarillo papel_de_diálogos
marrón tinta_de_pregunta
amarillo papel_de_pregunta
marrón tinta_de_error_lingüístico
amarillo papel_de_error_lingüístico
marrón tinta_de_error_operativo
amarillo papel_de_error_operativo
marrón tinta_de_error_del_sistema
amarillo papel_de_error_del_sistema
)

\ vim: textwidth=64
