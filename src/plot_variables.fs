\ plot_variables.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606282056

\ Note: The comments of the code are in Spanish.

\ ==============================================================
\ Variables de la trama

variable ambrosio-follows?
  \ ¿Ambrosio sigue al protagonista?

variable battle#
  \ Contador de la evolución de la batalla (si aún no ha empezado, es cero)

variable climbed-the-fallen-away?
  \ ¿El protagonista ha intentado escalar el derrumbe?

variable hacked-the-log?
  \ ¿El protagonista ha afilado el tronco?

\ variable hold#
  \ Contador de cosas llevadas por el protagonista.
  \ XXX OLD -- no se usa

variable stone-forbidden?
  \ ¿El protagonista ha intentado pasar con la piedra?

variable sword-forbidden?
  \ ¿El protagonista ha intentado pasar con la espada?

variable recent-talks-to-the-leader
  \ Contador de intentos de hablar con el líder sin cambiar de
  \ escenario.

\ vim:filetype=gforth:fileencoding=utf-8

