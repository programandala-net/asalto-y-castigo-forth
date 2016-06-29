\ data_tools.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606291923

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Herramientas para crear las fichas de la base de datos

: backup-entity  ( a -- x0 x1 x2 x3 x4 x5 x6 )
  >r
  r@ description-xt
  r@ init-xt
  r@ can-i-enter-location-xt
  r@ after-describing-location-xt
  r@ after-listing-entities-xt
  r@ before-describing-location-xt
  r@ before-leaving-location-xt
  r> name-str  ;
  \ Respalda los datos de un ente _a_
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético, para facilitar la edición).

: restore-entity  ( x0 x1 x2 x3 x4 x5 x6 a -- )
  >r
  r@ ~name-str !
  r@ ~before-leaving-location-xt !
  r@ ~before-describing-location-xt !
  r@ ~after-listing-entities-xt !
  r@ ~after-describing-location-xt !
  r@ ~can-i-enter-location-xt !
  r@ ~init-xt !
  r> ~description-xt !  ;
  \ Restaura los datos de un ente _a_
  \ que se crearon durante la compilación del código y deben preservarse.
  \ (En orden alfabético inverso, para facilitar la edición).

: setup-entity  ( a -- )
  >r r@ backup-entity  r@ erase-entity  r> restore-entity  ;
  \ Prepara la ficha de un ente para ser completada con sus datos .

0 value self~
  \ Ente cuyos atributos, descripción o trama están siendo definidos
  \ (usado para aligerar la sintaxis).

: :name-str  ( a -- )
  [debug-init] [if]  s" Inicio de :NAME-STR" debug [then]
  dup name-str ?dup
  [debug-init] [if]  s" A punto para STR-FREE" debug [then]
  ?? str-free
  str-new swap ~name-str !
  [debug-init] [if]  s" Final de :NAME-STR" debug [then]  ;
  \ Crea una cadena dinámica nueva para guardar el nombre del ente.

: [:attributes]  ( a -- )
  dup to self~  \ Actualizar el puntero al ente
  dup :name-str  \ Crear una cadena dinámica para el campo `~name-str`
  setup-entity  ;
  \ Inicia la definición de propiedades de un ente.  Esta palabra se
  \ ejecuta cada vez que hay que restaurar los datos del ente, y antes
  \ de la definición de atributos contenida en la palabra
  \ correspondiente al ente.  El identificador del ente está en la
  \ pila porque se compiló con `literal` cuando se creó la palabra de
  \ atributos.

: default-description  ( -- )  ^is-normal$ paragraph  ;
  \ Descripción predeterminada de los entes
  \ para los que no se ha creado una palabra propia de descripción.

: (:attributes)  ( a xt -- )
  over ~init-xt !  \ Conservar la dirección de ejecución en la ficha del ente
  ['] default-description over ~description-xt !  \ Poner la descripción predeterminada
  postpone literal  ;  \ Compilar el identificador de ente en la palabra de descripción recién creada, para que `[:description]` lo guarde en `self~` en tiempo de ejecución
  \ Operaciones preliminares para la definición de atributos de un ente.
  \ Esta palabra solo se ejecuta una vez para cada ente,
  \ al inicio de la compilación del código de la palabra
  \ que define sus atributos.
  \ a = Ente para la definición de cuyos atributos se ha creado una palabra
  \ xt = Dirección de ejecución de la palabra recién creada

: noname-roll  ( a xt colon-sys -- colon-sys a xt )  5 roll 5 roll  ;
  \ Mueve los parámetros que nos interesan a la parte alta de la pila;
  \ se usa tras crear con `:noname` una palabra relativa a un ente.
  \ Esta palabra es necesaria porque `:noname` deja en la pila
  \ sus valores de control (colon-sys), que en el caso de Gforth
  \ son tres elementos, para ser consumidos al finalizar la compilación de la
  \ palabra creada.
  \ a = Ente para el que se creó la palabra
  \ xt = Dirección de ejecución de la palabra sin nombre creada por `:noname`
  \ colon-sys = Valores de control dejados por `:noname`

: :attributes  ( a -- )
  :noname noname-roll
  (:attributes)  \ Crear la palabra y hacer las operaciones preliminares
  postpone [:attributes]  ;  \ Compilar la palabra `[:attributes]` en la palabra creada, para que se ejecute cuando sea llamada
  \ Inicia la creación de una palabra sin nombre que definirá las
  \ propiedades de un ente.

: ;attributes  ( sys-col -- )  postpone ;  ;  immediate

  \ XXX TODO -- mejorar el sistema de inicialización de entes en
  \ Flibustre, para hacerlo tan potente como el de _Asalto y castigo_.

: custom-init-entity ( a -- )
  [debug-init] [if]
    s" Inicio de `custom-init-entity`" debug dup entity># cr ." Entity=" .
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
    [debug-init] [if]  i cr ." about to init entity #" .  [then]
    i #>entity custom-init-entity
    \ i #>entity full-name space type ?.s  \ XXX INFORMER
  loop  ;
  \ Restaura las fichas de los entes a su estado original.

\ ==============================================================
\ Herramientas para crear las descripciones

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

false value sight
  \ Ente dirección al que se mira en un escenario
  \ (o el propio ente escenario); se usa en las palabras de
  \ descripción de escenarios

: [:description]  ( a -- )  to self~  ;
  \ Operaciones previas a la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al comienzo de la palabra de descripción.
  \ El identificador del ente está en la pila porque se compiló con
  \ `literal` cuando se creó la palabra de descripción.  Actualmente
  \ lo único que hace es actualizar el puntero al ente, usado para
  \ aligerar la sintaxis.

: (:description)  ( a xt -- )
  over ~description-xt !
  postpone literal  ;
  \ Operaciones preliminares para la definición de la descripción
  \ _xt_ de
  \ un ente _a_,
  \ Esta palabra solo se ejecuta una vez para cada ente, al inicio de
  \ la compilación del código de la palabra que crea su descripción.
  \ Hace dos operaciones: 1) Conservar la dirección de ejecución en la
  \ ficha del ente; 2) Compilar el identificador de ente en la palabra
  \ de descripción recién creada, para que `[:description]` lo guarde
  \ en `self~` en tiempo de ejecución.

: :description  ( a -- )
  :noname noname-roll  (:description)  postpone [:description]  ;
  \ Inicia la definición de una palabra de descripción para un ente
  \ _a_.

: [;description]  ( -- )  false to sight  ;
  \ Operaciones finales tras la ejecución de la descripción de un ente.
  \ Esta palabra se ejecutará al final de la palabra de descripción.
  \ Pone a cero el selector de vista, para evitar posibles errores.

: ;description  ( colon-sys -- )
  postpone [;description]  postpone ;  ; immediate
  \ Termina la definición de una palabra de descripción de un ente.

: (describe)  ( a -- )  ~description-xt perform  ;
  \ Ejecuta la palabra de descripción de un ente _a_.

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
  clear-screen-for-location dup .location-name  (describe-location)  ;
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

: uninteresting-direction  ( -- )  uninteresting-direction$ paragraph  ;
  \ Muestra la descripción de la direcciones que no tienen nada especial.

\ vim:filetype=gforth:fileencoding=utf-8
