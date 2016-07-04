\ answers.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606281902

\ Note: The comments of the code are in Spanish.

\ ==============================================================

\ Para los casos en que el programa hace una pregunta que debe
\ ser respondida con «sí» o «no», usamos un truco análogo al
\ del vocabulario del juego: creamos una lista de palabras
\ con palabras cuyos nombres sean las posibles respuestas:
\ «sí», «no», «s» y «n».  Estas palabras actualizarán una
\ variable,  con cuyo valor el programa sabrá si se ha
\ producido una respuesta válida o no y cuál es.
\
\ En principio, si el jugador introdujera varias respuestas
\ válidas la última sería la que tendría efecto. Por ejemplo,
\ la respuesta «sí sí sí sí sí no» sería considerada negativa.
\ Para dotar al método de una chispa de inteligencia, las
\ respuestas no cambian el valor de la variable sino que lo
\ incrementan o decrementan. Así el mayor número de respuestas
\ afirmativas o negativas decide el resultado; y si la
\ cantidad es la misma, como por ejemplo en «sí sí no no», el
\ resultado será el mismo que si no se hubiera escrito nada.

variable #answer
  \ Su valor será 0 si no ha habido respuesta válida; negativo para
  \ «no»; y positivo para «sí»

: answer-undefined  ( -- )  #answer off  ;
  \ Inicializa la variable antes de hacer la pregunta.

: think-it-again$  ( -- ca len )
  s{ s" Piénsalo mejor"
  s" Decídete" s" Cálmate" s" Concéntrate"
  s" Presta atención"
  s{ s" Prueba" s" Inténtalo" }s again$ s&
  s" No es tan difícil" }s colon+  ;
  \ Devuelve un mensaje complementario para los errores.

: yes-but-no$  ( -- ca len )
  s" ¿Primero «sí»" but|and$ s&
  s" después «no»?" s& think-it-again$ s&  ;
  \ Devuelve mensaje de error: se dijo «no» tras «sí».

: no-but-yes$  ( -- ca len )
  s" ¿Primero «no»" but|and$ s&
  s" después «sí»?" s& think-it-again$ s&  ;
  \ Devuelve mensaje de error: se dijo «sí» tras «no».

: yes-but-no  ( -- )  yes-but-no$ narrate  ;
  \ Muestra error: se dijo «no» tras «sí».

' yes-but-no constant yes-but-no-error#

: no-but-yes  ( -- )  no-but-yes$ narrate  ;
  \ Muestra error: se dijo «sí» tras «no».

' no-but-yes constant no-but-yes-error#

: two-options-only$  ( -- ca len )
  ^only$ s{ s" hay" s" tienes" }s&
  s" dos" s& s" respuestas" s" posibles" rnd2swap s& s& colon+
  s" «sí»" s" «no»" both& s" (o sus iniciales)" s& period+  ;
  \ Devuelve un mensaje que informa de las opciones disponibles.

: two-options-only  ( -- )  two-options-only$ narrate  ;
  \ Muestra error: sólo hay dos opciones.

' two-options-only constant two-options-only-error#

: wrong-yes$  ( -- ca len )
  s{ s" ¿Si qué...?" s" ¿Si...?" s" ¿Cómo «si»?" s" ¿Cómo que «si»?" }s
  s" No" s& s{
  s{ s" hay" s" puedes poner" }s{ s" condiciones" s" condición alguna" }s&
  s{ s" hay" s" tienes" }s s" nada que negociar" s& }s&
  s{ s" aquí" s" en esto" s" en esta cuestión" }s& period+
  \ two-options-only$ s?&  \ XXX TODO
  ;
  \ Devuelve el mensaje usado para advertir de que se ha escrito mal «sí».

: wrong-yes  ( -- )  wrong-yes$ narrate  ;
  \ Muestra error: se ha usado la forma errónea «si».

' wrong-yes constant wrong-yes-error#

: error-if-previous-yes  ( -- )
  #answer @ 0> yes-but-no-error# and throw  ;
  \ Provoca error si antes había habido síes.

: answer-no  ( -- )  error-if-previous-yes  #answer --  ;
  \ Anota una respuesta negativa.

: error-if-previous-not  ( -- )
  #answer @ 0< no-but-yes-error# and throw  ;
  \ Provoca error si antes había habido noes.

: answer-yes  ( -- )  error-if-previous-not  #answer ++  ;
  \ Anota una respuesta afirmativa.

wordlist  dup constant answer-wordlist  set-current

: sí  ( -- )  answer-yes  ;
: s   ( -- )  answer-yes  ;
: no  ( -- )  answer-no  ;
: n   ( -- )  answer-no  ;
: si  ( -- )  wrong-yes-error# throw  ;

restore-wordlists

: yes|no  ( ca len -- n )
  answer-undefined
  answer-wordlist 1 set-order
  ['] evaluate-command catch
  dup if  nip nip  then  \ Reajustar la pila si ha habido error
  dup ?wrong 0=  \ Ejecutar el posible error y preparar su indicador para usarlo en el resultado
  #answer @ 0= two-options-only-error# and ?wrong  \ Ejecutar error si la respuesta fue irreconocible
  #answer @ dup 0<> and and  \ Calcular el resultado final
  restore-wordlists  ;
  \ Evalúa una respuesta a una pregunta del tipo «sí o no».
  \ ca len = Respuesta a evaluar
  \ n = Resultado (un número negativo para «no» y positivo para «sí»; cero si no se ha respondido ni «sí» ni «no», o si se produjo un error)

: .question  ( xt -- )
  question-color execute paragraph  ;
  \ Imprime la pregunta.
  \ xt = Dirección de ejecución que devuelve una cadena con la pregunta

: accept-answer  ( -- ca len )  answer-wordlist accept-input  ;
  \ Espera una respuesta sí/no del jugador y la devuelve sin espacios
  \ laterales y en minúsculas en la cadena _ca len_.

: answer  ( xt -- n )
  begin  dup .question accept-answer  yes|no ?dup
  until  nip  ;
  \ Devuelve la respuesta a una pregunta del tipo «sí o no».
  \ xt = Dirección de ejecución que devuelve una cadena con la pregunta
  \ n = Respuesta: un número negativo para «no» y positivo para «sí»
  \ XXX FIXME -- usar una variante de `listen` que permita indicar
  \ la lista de palabras a usar.

: yes?  ( xt -- f )  answer 0>  ;
  \ ¿Es afirmativa la respuesta a una pregunta binaria cuyo texto
  \ es imprimido por _xt_?

: no?  ( xt -- f )  answer 0<  ;
  \ ¿Es negativa la respuesta a una pregunta binaria cuyo texto
  \ es imprimido por _xt_?

\ vim:filetype=gforth:fileencoding=utf-8
