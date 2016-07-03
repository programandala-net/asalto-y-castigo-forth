\ data_tools.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607040015

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Herramientas para crear las fichas de la base de datos

: backup-entity  ( a -- x0 x1 )
  >r
  r@ init-xt
  r> name-str  ;
  \ Respalda los datos de un ente _a_ que se crearon durante la
  \ compilación del código y deben preservarse.  (En orden alfabético,
  \ para facilitar la edición).

: restore-entity  ( x0 x1 a -- )
  tuck ~name-str !
       ~init-xt !  ;
  \ Restaura los datos de un ente _a_ que se crearon durante la
  \ compilación del código y deben preservarse.  (En orden alfabético
  \ inverso, para facilitar la edición).

: setup-entity  ( a -- )
  [debug-init] [if]  s" En `setup-entity`" debug [then]
  dup >r backup-entity
  [debug-init] [if]  s" Después de `backup-entity`" debug [then]
  r@ erase-entity
  [debug-init] [if]  s" Después de `erase-entity`" debug [then]
  r> restore-entity
  [debug-init] [if]
  s" Después de `restore-entity`" debug  ." hola!"
  [then]
  ;
  \ Prepara la ficha de un ente para ser completada con sus datos .

0 value self~
  \ Ente cuyos atributos, descripción o trama están siendo definidos
  \ (usado para aligerar la sintaxis).

: :name-str  ( a -- )
  [debug-init] [if]  s" Inicio de `:name-str`" debug [then]
  dup name-str ?dup
  [debug-init] [if]  s" A punto para `str-free`" debug [then]
  ?? str-free
  str-new swap ~name-str !
  [debug-init] [if]  s" Final de `:name-str`" debug [then]  ;
  \ Crea una cadena dinámica nueva para guardar el nombre del ente
  \ _a_.

: [:init]  ( a -- )
  [debug-init] [if]  s" Inicio de `[:init]`" debug [then]
  dup to self~
  [debug-init] [if]  s" Antes de `:name-str`" debug [then]
  dup :name-str
  [debug-init] [if]  s" Antes de `setup-entity`" debug [then]
  setup-entity
  [debug-init] [if]  s" Después de `setup-entity`" debug [then]
  ;
  \ Inicia la definición de propiedades de un ente.  Esta palabra se
  \ ejecuta cada vez que hay que restaurar los datos del ente, y antes
  \ de la definición de atributos contenida en la palabra
  \ correspondiente al ente.  El identificador del ente está en la
  \ pila porque se compiló con `literal` cuando se creó la palabra de
  \ atributos.

: (:init)  ( a xt -- )
  over ~init-xt !  postpone literal  ;
  \ Operaciones preliminares para la definición de atributos de un
  \ ente _a_.  Esta palabra solo se ejecuta una vez para cada ente, al
  \ inicio de la compilación del código de la palabra que define sus
  \ atributos.  _xt_ es la dirección de ejecución de la palabra recién
  \ creada. Operaciones: 1) Conserva la dirección de ejecución en la
  \ ficha del ente; 2) Compila el identificador de ente en la palabra
  \ de descripción recién creada, para que `[:description]` lo guarde
  \ en `self~` en tiempo de ejecución.

: noname-roll  ( a xt colon-sys -- colon-sys a xt )  5 roll 5 roll  ;
  \ Mueve los parámetros que nos interesan a la parte alta de la pila;
  \ se usa tras crear con `:noname` una palabra _xt_ relativa a un
  \ ente _a_.  Esta palabra es necesaria porque `:noname` deja en la
  \ pila sus valores de control, _colon-sys_, que en el caso de Gforth
  \ son tres elementos, para ser consumidos al finalizar la
  \ compilación de la palabra creada.

: :init  ( a -- )
  :noname noname-roll  (:init)  postpone [:init]  ;
  \ Inicia la creación de una palabra sin nombre que definirá las
  \ propiedades de un ente _a_. Hace tres operaciones: 1) Crea la palabra
  \ con `:noname`;
  \ 2) Hace las operaciones preliminares, mediante `(:init)`; 3)
  \ Compila la palabra `[:init]` en la palabra creada, para que
  \ se ejecute cuando sea llamada.

  \ XXX TODO -- mejorar el sistema de inicialización de entes en
  \ Flibustre, para hacerlo tan potente como el de _Asalto y castigo_.

: custom-init-entity ( a -- )
  [debug-init] [if]
    s" Inicio de `custom-init-entity`" debug
    dup entity># cr ." Entity=" .
  [then]
  init-xt
  [debug-init]
    [if]  s" Antes de `execute`" debug
  [then]
  execute
  [debug-init] [if]
    s" Final de `custom-init-entity`" debug
  [then]  ;
  \ Restaura la ficha de un ente a su estado original.

: init-entities  ( -- )
  #entities 0 do
    [debug-init] [if]  i cr ." About to init entity #" .  [then]
    i #>entity custom-init-entity
    \ i #>entity full-name space type ?.s  \ XXX INFORMER
  loop  ;
  \ Restaura las fichas de los entes a su estado original.

\ ==============================================================
\ Herramientas para mostrar las descripciones

\ En el campo `~description-xt` de cada ente se almacena la dirección
\ de ejecución de una palabra que imprime su descripción.

false value sight
  \ Ente dirección al que se mira en un escenario
  \ (o el propio ente escenario); se usa en las palabras de
  \ descripción de escenarios

defer default-description  ( -- )
  \ Descripción predeterminada de los entes
  \ para los que no se ha creado una palabra propia de descripción.

: (default-description)  ( -- )  ^is-normal$ paragraph  ;
  \ Comportamiento predeterminado de la descripción predeterminada de
  \ los entes para los que no se ha creado una palabra propia de
  \ descripción.

' (default-description) is default-description

: (describe)  ( a -- )
  description-xt ?dup if    execute
                      else  default-description  then  ;
  \ Ejecuta la palabra de descripción de un ente _a_, si está
  \ inicializada; en caso contrario ejecuta la descripción
  \ predeterminada.

: .location-name  ( a -- )
  [debug-map] [if]  dup  [then]
  name ^uppercase location-name-color paragraph
  [debug-map] [if]
    entity># location-01~ entity># - 1+ ."  [ location #" . ." ]"
  [then]  ;
  \ Imprime el nombre de un ente escenario, como cabecera de su descripción.
  \ XXX TODO -- añadir el artículo correspondiente o no, dependiendo
  \ de un indicador de la ficha:
  \     pasaje de la serpiente -- el pasaje de la serpiente
  \     el paso del Perro -- el paso del Perro
  \     un tramo de cueva -- un tramo de cueva
  \     un lago interior -- el lago interior
  \     hogar de Ambrosio -- el hogar de Ambrosio

: (describe-location)  ( a -- )
  dup to sight  location-description-color (describe)  ;
  \ Describe un ente escenario _a_.

: describe-location  ( a -- )
  clear-screen-for-location
  dup .location-name  (describe-location)  ;
  \ Describe un ente escenario _a_,
  \ con borrado de pantalla y título.

: describe-other  ( a -- )  description-color (describe)  ;
  \ Describe un ente de otro tipo.

: describe-direction  ( a -- )
  to sight  my-location describe-other  ;
  \ Describe un ente dirección _a_.

: description-type  ( a -- u )
  dup is-location? abs
  [true] [if]
    swap is-direction? 2 and +
  [else]
    \ XXX TODO -- terminar; no usado
    over is-direction? 2 and +
    swap exits~ = 4 and +
  [then]  ;
  \ Convierte un ente _a_ en el tipo de descripción _u_ que requiere:
  \ 4=salida, 2=dirección, 1=escenario, 0=otros, 3=¡error!.  Un
  \ resultado de 3 significaría que el ente es a la vez dirección y
  \ escenario.

: describe  ( a -- )
  dup description-type
  case
    0 of  describe-other  endof
    1 of  describe-location  endof
    2 of  describe-direction  endof
    abort" Error fatal en `describe`: dato incorrecto"  \ XXX INFORMER
  endcase  ;
  \ Describe un ente _a_, según su tipo.

: uninteresting-direction  ( -- )
  uninteresting-direction$ paragraph  ;
  \ Muestra la descripción de la direcciones que no tienen nada especial.

\ ==============================================================
\ Herramientas para crear conexiones entre escenarios

\ Para crear el mapa hay que hacer dos operaciones con los
\ entes escenario: marcarlos como tales, para poder
\ distinguirlos como escenarios; e indicar a qué otros entes
\ escenario conducen sus salidas.
\
\ La primera operación se hace guardando un valor buleano «cierto»
\ en el campo `~is-location?` del ente.  Por ejemplo:

\   cave~ ~is-location? bit-on

\ O bien mediante la palabra creada para ello en la interfaz
\ básica de campos:

\   cave~ be-location

\ La segunda operación se hace guardando en los campos de
\ salida del ente los identificadores de los entes a que cada
\ salida conduzca.  No hace falta ocuparse de las salidas
\ impracticables porque ya estarán a cero de forma
\ predeterminada.  Por ejemplo:

\   path~ cave~ ~south-exit !  \ Hacer que la salida sur de `cave~` conduzca a `path~`
\   cave~ path~ ~north-exit !  \ Hacer que la salida norte de `path~` conduzca a `cave~`

\ No obstante, para hacer más fácil este segundo paso, hemos
\ creado unas palabras que proporcionan una sintaxis específica,
\ como mostraremos a continuación.

0 [if]  \ XXX TODO -- inconcluso

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
south~ ,
north~ ,
west~ ,
east~ ,
down~ ,
up~ ,
in~ ,
out~ ,

[then]

\ Necesitamos una tabla que nos permita traducir esto:
\
\ ENTRADA: Un puntero correspondiente a un campo de dirección
\ de salida en la ficha de un ente.
\
\ SALIDA: El identificador del ente dirección al que se
\ refiere esa salida.

create exits-table  #exits cells allot
  \ Tabla de traducción de salidas.

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

\ Rellenar cada elemento de la tabla con un ente de salida, usando
\ como puntero el campo análogo de la ficha.  Haciéndolo de esta
\ manera no importa el orden en que se rellenen los elementos.

north~ north-exit> exits-table!
south~ south-exit> exits-table!
east~ east-exit> exits-table!
west~ west-exit> exits-table!
up~ up-exit> exits-table!
down~ down-exit> exits-table!
out~ out-exit> exits-table!
in~ in-exit> exits-table!

0 [if]  \ XXX TODO -- inconcluso
: opposite-exit  ( a1 -- a2 )
  first-exit> - opposite-exits + @  ;
  \ Devuelve la dirección cardinal opuesta a la indicada.

: opposite-exit~  ( a1 -- a2 )
  first-exit> - opposite-direction-entities + @  ;
  \ Devuelve el ente dirección cuya direccién es opuesta a la indicada.
  \ a1 = entidad de dirección
  \ a2 = entidad de dirección, opuesta a a1

[then]

\ A continuación definimos palabras para proporcionar la
\ siguiente sintaxis (primero origen y después destino en la
\ pila, como es convención en Forth):

\   \ Hacer que la salida sur de `cave~` conduzca a `path~`
\   \ pero sin afectar al sentido contrario:
\   cave~ path~ s-->

\   \ Hacer que la salida norte de `path~` conduzca a `cave~`
\   \ pero sin afectar al sentido contrario:
\   path~ cave~ n-->

\ O en un solo paso:

\   \ Hacer que la salida sur de `cave~` conduzca a `path~`
\   \ y al contrario: la salida norte de `path~` conducirá a `cave~`:
\   cave~ path~ s<-->

: -->  ( a1 a2 u -- )  rot + !  ;
  \ Conecta el ente _a1_ (origen) con el ente _a2_ (destino) mediante
  \ la salida de _a1_ indicada por el desplazamiento de campo _u_.

: -->|  ( a1 u -- )  + no-exit swap !  ;
  \ Cierra la salida del ente _a1_ (origen) hacia el ente _a2_
  \ (destino), indicada por el desplazamiento de campo _u_.

\ Conexiones unidireccionales

: n-->  ( a1 a2 -- )  north-exit> -->  ;
  \ Comunica la salida norte del ente _a1_ con el ente _a2_.

: s-->  ( a1 a2 -- )  south-exit> -->  ;
  \ Comunica la salida sur del ente _a1_ con el ente _a2_.

: e-->  ( a1 a2 -- )  east-exit> -->  ;
  \ Comunica la salida este del ente _a1_ con el ente _a2_.

: w-->  ( a1 a2 -- )  west-exit> -->  ;
  \ Comunica la salida oeste del ente _a1_ con el ente _a2_.

: u-->  ( a1 a2 -- )  up-exit> -->  ;
  \ Comunica la salida hacia arriba del ente _a1_ con el ente _a2_.

: d-->  ( a1 a2 -- )  down-exit> -->  ;
  \ Comunica la salida hacia abajo del ente _a1_ con el ente _a2_.

: o-->  ( a1 a2 -- )  out-exit> -->  ;
  \ Comunica la salida hacia fuera del ente _a1_ con el ente _a2_.

: i-->  ( a1 a2 -- )  in-exit> -->  ;
  \ Comunica la salida hacia dentro del ente _a1_ con el ente _a2_.

: n-->|  ( a1 -- )  north-exit> -->|  ;
  \ Desconecta la salida norte del ente _a1_.

: s-->|  ( a1 -- )  south-exit> -->|  ;
  \ Desconecta la salida sur del ente _a1_.

: e-->|  ( a1 -- )  east-exit> -->|  ;
  \ Desconecta la salida este del ente _a1_.

: w-->|  ( a1 -- )  west-exit> -->|  ;
  \ Desconecta la salida oeste del ente _a1_.

: u-->|  ( a1 -- )  up-exit> -->|  ;
  \ Desconecta la salida hacia arriba del ente _a1_.

: d-->|  ( a1 -- )  down-exit> -->|  ;
  \ Desconecta la salida hacia abajo del ente _a1_.

: o-->|  ( a1 -- )  out-exit> -->|  ;
  \ Desconecta la salida hacia fuera del ente _a1_.

: i-->|  ( a1 -- )  in-exit> -->|  ;
  \ Desconecta la salida hacia dentro del ente _a1_.

\ Conexiones bidireccionales

: n<-->  ( a1 a2 -- )  2dup n-->  swap s-->  ;
  \ Comunica la salida norte del ente _a1_ con el ente _a2_ (y al
  \ contrario).

: s<-->  ( a1 a2 -- )  2dup s-->  swap n-->  ;
  \ Comunica la salida sur del ente _ente a1_ con el ente _ente a2_ (y
  \ al contrario).

: e<-->  ( a1 a2 -- )  2dup e-->  swap w-->  ;
  \ Comunica la salida este del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: w<-->  ( a1 a2 -- )  2dup w-->  swap e-->  ;
  \ Comunica la salida oeste del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: u<-->  ( a1 a2 -- )  2dup u-->  swap d-->  ;
  \ Comunica la salida hacia arriba del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: d<-->  ( a1 a2 -- )  2dup d-->  swap u-->  ;
  \ Comunica la salida hacia abajo del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: o<-->  ( a1 a2 -- )  2dup o-->  swap i-->  ;
  \ Comunica la salida hacia fuera del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: i<-->  ( a1 a2 -- )  2dup i-->  swap o-->  ;
  \ Comunica la salida hacia dentro del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: n|<-->|  ( a1 a2 -- )  s-->|  n-->|  ;
  \ Desconecta la salida norte del ente _ente a1_ con el ente _ente
  \ a2_ (y al contrario).

: s|<-->|  ( a1 a2 -- )  n-->|  s-->|  ;
  \ Desconecta la salida sur del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: e|<-->|  ( a1 a2 -- )  w-->|  e-->|  ;
  \ Desconecta la salida este del ente _ente a1_ con el ente _ente a2_
  \ (y al contrario).

: w|<-->|  ( a1 a2 -- )  e-->|  w-->|  ;
  \ Desconecta la salida oeste del ente _ente a1_ con el ente _ente
  \ a2_ (y al contrario).

: u|<-->|  ( a1 a2 -- )  d-->|  u-->|  ;
  \ Desconecta la salida hacia arriba del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: d|<-->|  ( a1 a2 -- )  u-->|  d-->|  ;
  \ Desconecta la salida hacia abajo del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: o|<-->|  ( a1 a2 -- )  i-->|  o-->|  ;
  \ Desconecta la salida hacia fuera del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

: i|<-->|  ( a1 a2 -- )  o-->|  i-->|  ;
  \ Desconecta la salida hacia dentro del ente _ente a1_ con el ente
  \ _ente a2_ (y al contrario).

\ Por último, definimos dos palabras para hacer
\ todas las asignaciones de salidas en un solo paso.

: exits!  ( a1 ... a8 a0 -- )
  >r r@ ~out-exit !
     r@ ~in-exit !
     r@ ~down-exit !
     r@ ~up-exit !
     r@ ~west-exit !
     r@ ~east-exit !
     r@ ~south-exit !
     r> ~north-exit !  ;
  \ Asigna todas las salidas _a1 ... a8_ de un ente escenario _a0_.
  \ Los entes de salida _a1 ... a8_ (o cero) están en el orden
  \ habitual: norte, sur, este, oeste, arriba, abajo, dentro, fuera.

: init-location  ( a1 ... a8 a0 -- )  dup be-location exits!  ;
  \ Marca un ente _a0_ como escenario y le asigna todas las salidas.
  \ _a1 ... a8_, que sen entes escenario de salida (o cero) en el
  \ orden habitual: norte, sur, este, oeste, arriba, abajo, dentro,
  \ fuera.

\ vim:filetype=gforth:fileencoding=utf-8
