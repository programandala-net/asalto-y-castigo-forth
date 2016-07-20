\ intro.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607202136

\ Note: The comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-false.fs   \ `[false]`
require galope/random_strings.fs
require galope/txt-plus.fs        \ `txt+`

set-current

\ ==============================================================

: sun$  ( -- ca len )
  s{ s" sol" s" astro rey" }s  ;

: intro-0  ( -- )
  s{
  s{ s" El " s" La luz del" }s sun$ txt+
    s{ s" despunta de entre" s" penetra en" s" atraviesa" s" corta" }s txt+
  s" Los rayos del" sun$ txt+
    s{ s" despuntan de entre" s" penetran en" s" atraviesan" s" cortan" }s txt+
  }s
  s" la" txt+ s" densa" s? txt+ s" niebla," txt+
  s" haciendo humear los" txt+ s" pobres" s? txt+ s" tejados de paja." txt+
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 0).

: intro-1  ( -- )
  s" Piensas en"
  s{ s" el encargo de"
  s" la" s{ s" tarea" s" misión" }s txt+ s" encomendada por" txt+
  s" la orden dada por" s" las órdenes de" }s txt+
  s{ s" Uther Pendragon" s" , tu rey" s? s+ s" tu rey" }s txt+ \ XXX TMP
  s" ..." s+
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 1).

: intro-2  ( -- )
  s{ s" Atacar" s" Arrasar" s" Destruir" }s s" una" txt+
  s" aldea" s{ s" tranquila" s" pacífica" }s rnd2swap txt+ txt+
  s" , aunque" s+ s{ s" se trate de una" s" sea una" s" esté" }s txt+
  s{ s" llena de" s" habitada por" s" repleta de" }s txt+
  s" sajones, no te" txt+ s{ s" llena" s" colma" }s txt+
  s" de orgullo." txt+
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 2).

: intro-3  ( -- )
  ^your-soldiers$ s{
    s" se" s{ s" lanzan" s" arrojan" }s txt+ s" sobre" txt+
    s" se" s{ s" amparan" s" apoderan" }s txt+ s" de" txt+
    s{ s" rodean" s" cercan" }s
  }s txt+ s" la aldea y la" txt+
  s{ s" destruyen." s" arrasan." }s txt+
  s" No hubo" txt+ s{
    s" tropas enemigas"
    s" ejército enemigo"
    s" guerreros enemigos"
  }s txt+
  s{
    s"  ni honor" s" alguno" s? txt+
    s" , como tampoco honor" s" alguno" s? txt+ comma+
  }s s+
  s" en" txt+ s{ s" la batalla" s" el combate" s" la lucha" s" la pelea" }s txt+ period+
  narrate  scene-break  ;
  \ Muestra la introducción al juego (parte 3).

: intro-4  ( -- )
  sire,$ s{
  s" el asalto" s" el combate" s" la batalla"
  s" la lucha" s" todo"
  }s txt+ s" ha" txt+ s{ s" terminado" s" concluido" }s txt+
  period+ speak  ;
  \ Muestra la introducción al juego (parte 4).

: needed-orders$  ( -- ca len )
  s" órdenes" s{ null$ s" necesarias" s" pertinentes" }s txt+  ;
  \ Devuelve una variante de «órdenes necesarias».

: intro-5  ( -- )
  s" Lentamente," s{
  s" ordenas"
  s" das la orden de"
  s" das las" needed-orders$ txt+ s" para" txt+
  }s txt+ to-go-back$ txt+ s" a casa." txt+
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 5).

: intro-6  ( -- )
  [false] [if]
    \ XXX OLD -- Primera versión, descartada
    ^officers-forbid-to-steal$ period+
  [else]
    \ XXX NEW -- Segunda versión
    soldiers-steal-spite-of-officers$ period+
  [then]
  narrate  scene-break  ;
  \ Muestra la introducción al juego (parte 6).

: intro  ( -- )
  new-page
  intro-0 intro-1 intro-2 intro-3 intro-4 intro-5 intro-6  ;
  \ Muestra la introducción al juego .

\ vim:filetype=gforth:fileencoding=utf-8

