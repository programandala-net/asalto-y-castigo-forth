\ debug_tests.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201711171338
\ See change log at the end of the file

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Pruebas para depurar el programa

\ Esta sección contiene código para probar el programa sin interactuar
\ con el juego, para detectar mejor posibles errores.

: check-stack1 ( -- )
  \ Provoca un error -3 («stack overflow») si la pila no tiene solo un
  \ elemento.
  depth 1 <> -3 and throw ;

: check-stack ( -- )
  \ Provoca un error -3 («stack overflow») si la pila no está vacía.
  depth 0<> -3 and throw ;

: test-location-description ( a -- )
  \ Comprueba todas las descripciones de un ente escenario.
  cr ." = Descripción de escenario =======" cr
  dup my-holder!
  describe-location check-stack
  cr ." == Mirar al norte:" cr
  north~ describe-direction check-stack
  cr ." == Mirar al sur:" cr
  south~ describe-direction check-stack
  cr ." == Mirar al este:" cr
  east~ describe-direction check-stack
  cr ." == Mirar al oeste:" cr
  west~ describe-direction check-stack
  cr ." == Mirar hacia arriba:" cr
  up~ describe-direction check-stack
  cr ." == Mirar hacia abajo:" cr
  down~ describe-direction check-stack
\ Aún no implementado:
\ cr ." == Mirar hacia fuera:" cr
\ out~ describe-direction check-stack
\ cr ." == Mirar hacia dentro:" cr
\ in~ describe-direction check-stack
 ;

0 value tested

: test-description ( a -- )
  to tested
  cr ." = Nombre =========================" cr
  tested full-name type
  cr ." = Descripción ====================" cr
  tested describe check-stack
  tested is-location? if  tested test-location-description  then ;
  \ Comprueba la descripciones de un ente.

: test-descriptions ( -- )
  #entities 0 do
    i #>entity test-description
  loop ;
  \ Comprueba la descripción de todos los entes.

: test-battle-phase ( u -- )
  32 0 do  \ 32 veces cada fase, porque los textos son aleatorios
    dup (battle-phase) check-stack1
  loop  drop ;
  \ Comprueba una fase de la batalla.

: test-battle ( -- )
  battle-phases 0 do
    i test-battle-phase
  loop ;
  \ Comprueba todas las fases de la batalla.

: check-prepos
  s" mira espada usando capa"
  ~~
  init-parsing
  ~~
  valid-parsing?
  ~~
  0= abort" parsing failed"
  ~~
  used-complements ?
  ~~ ;

: bla$ ( -- ca len )
  s" bla bla bla bla bla bla bla bla bla bla bla bla bla bla" ;

: blabla ( -- )
  bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s&
  bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s&
  bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s& bla$ s&
  narrate ;

\ ==============================================================
\ Change log

\ 2017-11-10: Update to Talanto 0.62.0: replace field notation
\ "location" with "holder".
\
\ 2017-11-17: Remove words of Galope's deprecated module <print.fs>,
\ and words removed from <printing.fs>.

\ vim:filetype=gforth:fileencoding=utf-8
