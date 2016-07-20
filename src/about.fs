\ about.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607202135

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/txt-plus.fs            \ `txt+`

set-current

\ ==============================================================

: based-on  ( -- )
  s" «Asalto y castigo» está basado"
  s" en el programa homónimo escrito en BASIC en 2009 por" txt+
  s" Baltasar el Arquero (http://caad.es/baltasarq/)." txt+
  paragraph  ;
  \ Muestra un texto sobre el programa original.

: license  ( -- )
  s" (C) 2011-2016 Marcos Cruz (programandala.net)" paragraph
  s" «Asalto y castigo» es un programa libre;"
  s" puedes distribuirlo y/o modificarlo bajo los términos de" txt+
  s" la Licencia Pública General de GNU, tal como está publicada" txt+
  s" por la Free Software Foundation (Fundación para los Programas Libres)," txt+
  s" bien en su versión 2 o, a tu elección, cualquier versión posterior" txt+
  s" (http://gnu.org/licenses/)." txt+ \ XXX TODO -- confirmar
  paragraph  ;
  \ Muestra un texto sobre la licencia.

: program  ( -- )
  s" «Asalto y castigo»" paragraph
  s" Versión " version txt+ paragraph
  s" Escrito en Forth con Gforth." paragraph  ;
  \ Muestra el nombre y versión del programa.

: about  ( -- )
  new-page about-color
  program print_cr license print_cr based-on
  scene-break  ;
  \ Muestra información sobre el programa.

\ vim:filetype=gforth:fileencoding=utf-8

