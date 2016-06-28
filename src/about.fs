\ about.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606281955

\ Note: The comments of the code are in Spanish.

\ ==============================================================

: based-on  ( -- )
  s" «Asalto y castigo» está basado"
  s" en el programa homónimo escrito en BASIC en 2009 por" s&
  s" Baltasar el Arquero (http://caad.es/baltasarq/)." s&
  paragraph  ;
  \ Muestra un texto sobre el programa original.

: license  ( -- )
  s" (C) 2011-2016 Marcos Cruz (programandala.net)" paragraph
  s" «Asalto y castigo» es un programa libre;"
  s" puedes distribuirlo y/o modificarlo bajo los términos de" s&
  s" la Licencia Pública General de GNU, tal como está publicada" s&
  s" por la Free Software Foundation (Fundación para los Programas Libres)," s&
  s" bien en su versión 2 o, a tu elección, cualquier versión posterior" s&
  s" (http://gnu.org/licenses/)." s& \ XXX TODO -- confirmar
  paragraph  ;
  \ Muestra un texto sobre la licencia.

: program  ( -- )
  s" «Asalto y castigo»" paragraph
  s" Versión " version s& paragraph  
  s" Escrito en Forth con Gforth." paragraph  ;
  \ Muestra el nombre y versión del programa.

: about  ( -- )
  new-page about-color
  program print_cr license print_cr based-on
  scene-break  ;
  \ Muestra información sobre el programa.

\ vim:filetype=gforth:fileencoding=utf-8

