\ intro.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606281956

\ Note: The comments of the code are in Spanish.

\ ==============================================================

: sun$  ( -- ca len )
  s{ s" sol" s" astro rey" }s  ;

: intro-0  ( -- )
  s{
  s{ s" El " s" La luz del" }s sun$ s&
    s{ s" despunta de entre" s" penetra en" s" atraviesa" s" corta" }s&
  s" Los rayos del" sun$ s&
    s{ s" despuntan de entre" s" penetran en" s" atraviesan" s" cortan" }s&
  }s
  s" la" s& s" densa" s?& s" niebla," s&
  s" haciendo humear los" s& s" pobres" s?& s" tejados de paja." s&
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 0).

: intro-1  ( -- )
  s" Piensas en"
  s{ s" el encargo de"
  s" la" s{ s" tarea" s" misión" }s& s" encomendada por" s&
  s" la orden dada por" s" las órdenes de" }s&
  s{ s" Uther Pendragon" s" , tu rey" s?+ s" tu rey" }s& \ XXX TMP
  s" ..." s+
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 1).

: intro-2  ( -- )
  s{ s" Atacar" s" Arrasar" s" Destruir" }s s" una" s&
  s" aldea" s{ s" tranquila" s" pacífica" }s rnd2swap s& s&
  s" , aunque" s+ s{ s" se trate de una" s" sea una" s" esté" }s&
  s{ s" llena de" s" habitada por" s" repleta de" }s&
  s" sajones, no te" s&{ s" llena" s" colma" }s&
  s" de orgullo." s&
  narrate  narration-break  ;
  \ Muestra la introducción al juego (parte 2).

: intro-3  ( -- )
  ^your-soldiers$ s{
    s" se" s{ s" lanzan" s" arrojan" }s& s" sobre" s&
    s" se" s{ s" amparan" s" apoderan" }s& s" de" s&
    s{ s" rodean" s" cercan" }s
  }s& s" la aldea y la" s&
  s{ s" destruyen." s" arrasan." }s&
  s" No hubo" s&{
    s" tropas enemigas"
    s" ejército enemigo"
    s" guerreros enemigos"
  }s&
  s{
    s"  ni honor" s" alguno" s?&
    s" , como tampoco honor" s" alguno" s?& comma+
  }s+
  s" en" s& s{ s" la batalla" s" el combate" s" la lucha" s" la pelea" }s& period+
  narrate  scene-break  ;
  \ Muestra la introducción al juego (parte 3).

: intro-4  ( -- )
  sire,$ s{
  s" el asalto" s" el combate" s" la batalla"
  s" la lucha" s" todo"
  }s& s" ha" s&{ s" terminado" s" concluido" }s&
  period+ speak  ;
  \ Muestra la introducción al juego (parte 4).

: needed-orders$  ( -- ca len )
  s" órdenes" s{ null$ s" necesarias" s" pertinentes" }s&  ;
  \ Devuelve una variante de «órdenes necesarias».

: intro-5  ( -- )
  s" Lentamente," s{
  s" ordenas"
  s" das la orden de"
  s" das las" needed-orders$ s& s" para" s&
  }s& to-go-back$ s& s" a casa." s&
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

