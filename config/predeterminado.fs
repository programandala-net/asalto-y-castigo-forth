\ config/predeterminado.fs
\ Fichero de configuración predeterminado

\ Este fichero forma parte de
\ «Asalto y castigo»
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Autor: Marcos Cruz (programandala.net), 2011..2016

\ Última modificación: 201607111055

\ ==============================================================

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

\ Un presto o inductor (en inglés, «prompt») es una marca
\ gráfica convencional que sirve para indicar que el programa
\ está preparado para recibir la entrada del usuario y señalar
\ el lugar de la pantalla en que se mostrará dicha entrada.

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
\ Estilo de colores
\ --------------------------------------------------------------

\ El estilo de colores se lee desde otro fichero.  El estilo
\ predeterminado es el que imita los colores de la versión
\ original del juego:

incluye color.predeterminado.fs

\ vim: textwidth=64
