\ entity_structure_interface.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Description: The basic interface to fetch and set fields of the
\ entity structure. This layer makes sure possible changes in the
\ structure will not affect the way the application access it.

\ XXX UNDER DEVELOPMENT -- Being adapted from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ Author: Marcos Cruz (programandala.net), 2011..2018.

\ Last modified 201805052247
\ See change log at the end of the file

\ ==============================================================
\ Requirements

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/question-one-plus-store.fs  \ `?1+!`

set-current

require ./entity_structure.fs

\ ==============================================================
\ Entity structure interface

\ ----------------------------------------------
\ Direction fields tools

synonym ~first-exit ~north-exit
  \ First exit defined in the data structure.

synonym ~last-exit ~in-exit
  \ Last exit defined in the data structure.

\ Calculate the offset of every exit field from the first of them:

0 ~first-exit      constant first-exit>
0 ~last-exit       constant last-exit>
0 ~north-exit      constant north-exit>
0 ~north-east-exit constant north-east-exit>
0 ~north-west-exit constant north-west-exit>
0 ~south-exit      constant south-exit>
0 ~south-east-exit constant south-east-exit>
0 ~south-west-exit constant south-west-exit>
0 ~east-exit       constant east-exit>
0 ~west-exit       constant west-exit>
0 ~up-exit         constant up-exit>
0 ~down-exit       constant down-exit>
0 ~out-exit        constant out-exit>
0 ~in-exit         constant in-exit>

last-exit> cell+ first-exit> - constant /exits
  \ Size of all exit fields, in address units.

/exits cell / constant #exits
  \ Number of exit fields.

0 constant no-exit

: exit? ( a -- f ) no-exit <> ;

\ ----------------------------------------------
\ Field fetchers

: name ( a -- ca len ) ~name 2@ ;

: conversations ( a -- u ) ~conversations @ ;
: describer ( a -- xt ) ~describer @ ;
: direction ( a -- u ) ~direction @ ;
: familiar ( a -- u ) ~familiar @ ;

: has-definite-article? ( a -- f ) ~has-definite-article bit@ ;
: has-feminine-name? ( a -- f ) ~has-feminine-name bit@ ;
: has-masculine-name? ( a -- f ) has-feminine-name? 0= ;
: has-no-article? ( a -- f ) ~has-no-article bit@ ;
: has-personal-name? ( a -- f ) ~has-personal-name bit@ ;
: has-plural-name? ( a -- f ) ~has-plural-name bit@ ;
: has-singular-name? ( a -- f ) has-plural-name? 0= ;

: initializer ( a -- xt ) ~initializer @ ;

: is-human? ( a -- f ) ~is-human bit@ ;
: is-animal? ( a -- f ) ~is-animal bit@ ;
: is-character? ( a -- f ) ~is-character bit@ ;
: is-vegetal? ( a -- f ) ~is-vegetal bit@ ;

: is-wearable? ( a -- f ) ~is-wearable bit@ ;
: is-not-wearable? ( a -- f ) is-wearable? 0= ;
: is-worn? ( a -- f ) ~is-worn bit@ ;
: is-not-worn? ( a -- f ) is-worn? 0= ;

: is-not-listed? ( a -- f ) ~is-not-listed bit@ ;
: is-decoration? ( a -- f ) ~is-decoration bit@ ;
: is-global-indoor? ( a -- f ) ~is-global-indoor bit@ ;
: is-global-outdoor? ( a -- f ) ~is-global-outdoor bit@ ;

: is-light-source? ( a -- f ) ~is-light-source bit@ ;
: is-listed? ( a -- f ) is-not-listed? 0= ;
: is-lighted? ( a -- f ) ~is-lighted bit@ ;
: is-not-lighted? ( a -- f ) is-lighted? 0= ;

: is-location? ( a -- f ) ~is-location bit@ ;
: is-indoor-location? ( a -- f )
  dup is-location? swap ~is-indoor-location bit@ and ;
: is-outdoor-location? ( a -- f ) is-indoor-location? 0= ;
: visits ( a -- u ) ~visits @ ;
: is-visited? ( a -- f ) visits 0> ;
: is-not-visited? ( a -- f ) visits 0= ;

: holder ( a1 -- a2 ) ~holder @ ;
: owner ( a1 -- a2 ) ~owner @ ;

: enter-checker ( a -- xt ) ~enter-checker @ ;
: before-description-plotter ( a -- xt ) ~before-description-plotter @ ;
: after-description-plotter ( a -- xt ) ~after-description-plotter @ ;
: before-prompt-plotter ( a -- xt ) ~before-prompt-plotter @ ;
: before-exit-plotter ( a -- xt ) ~before-exit-plotter @ ;
: previous-holder ( a1 -- a2 ) ~previous-holder @ ;

: is-direction? ( a -- f ) direction 0<> ;
: north-exit ( a1 -- a2 ) ~north-exit @ ;
: south-exit ( a1 -- a2 ) ~south-exit @ ;
: east-exit ( a1 -- a2 ) ~east-exit @ ;
: west-exit ( a1 -- a2 ) ~west-exit @ ;
: up-exit ( a1 -- a2 ) ~up-exit @ ;
: down-exit ( a1 -- a2 ) ~down-exit @ ;
: out-exit ( a1 -- a2 ) ~out-exit @ ;
: in-exit ( a1 -- a2 ) ~in-exit @ ;

: is-familiar? ( a -- f ) familiar 0> ;

: conversations? ( a -- f ) conversations 0<> ;
: no-conversations? ( a -- f ) conversations 0= ;

: has-north-exit? ( a -- f ) north-exit exit? ;
: has-south-exit? ( a -- f ) south-exit exit? ;
: has-east-exit? ( a -- f ) east-exit exit? ;
: has-west-exit? ( a -- f ) west-exit exit? ;
: has-up-exit? ( a -- f ) up-exit exit? ;
: has-down-exit? ( a -- f ) down-exit exit? ;
: has-in-exit? ( a -- f ) in-exit exit? ;
: has-out-exit? ( a -- f ) out-exit exit? ;

: is-container? ( a -- f ) ~is-container bit@ ;
: is-openable? ( a -- f ) ~is-openable bit@ ;
: is-open? ( a -- f ) ~is-open bit@ ;
: is-closed? ( a -- f ) is-open? 0= ;
: is-lockable? ( a -- f ) ~is-lockable bit@ ;
: is-lock? ( a -- f ) ~is-lock bit@ ;
: times-open ( a -- u ) ~times-open @ ;

: is-owner? ( a1 a2 -- f ) swap owner = ;
  \ Is entity _a2_ the owner of entity _a1_?

: is-there? ( a1 a2 -- f ) holder = ;
  \ Is entity _a2_ at/in entity _a1_?

: was-there? ( a1 a2 -- f ) previous-holder = ;
  \ Was entity _a2_ at/in entity _a1_?

: is-global? ( a -- f )
  dup is-global-outdoor? swap is-global-indoor? or ;

: is-beast? ( a -- f )
  dup is-animal? swap is-human? or ;
  \ Is entity _a_ a beast (animal or human)?

: is-living-being? ( a -- f )
  dup is-vegetal? swap is-beast? or ;
  \ Is entity _a_ a living being (no matter dead or alive)?

: is-takeable? ( a -- f )
  dup is-decoration? swap is-global? or 0= ;
  \ Can entity _a_ be taken?

\ ----------------------------------------------
\ Field modifiers

: name! ( ca len a -- ) ~name 2! ;

: be-before-description-plotter ( xt a -- ) ~before-description-plotter ! ;
: be-describer ( xt a -- ) ~describer ! ;
: be-after-description-plotter ( xt a -- ) ~after-description-plotter ! ;
: be-before-prompt-plotter ( xt a -- ) ~before-prompt-plotter ! ;
: be-before-exit-plotter ( xt a -- ) ~before-exit-plotter ! ;

: have-definite-article ( a -- ) ~has-definite-article bit-on ;
: have-feminine-name ( a -- ) ~has-feminine-name bit-on ;
: have-masculine-name ( a -- ) ~has-feminine-name bit-off ;
: have-no-article ( a -- ) ~has-no-article bit-on ;
: have-personal-name ( a -- ) ~has-personal-name bit-on ;
: have-plural-name ( a -- ) ~has-plural-name bit-on ;
: have-singular-name ( a -- ) ~has-plural-name bit-off ;

: be-human ( a -- ) ~is-human bit-on ;
: be-character ( a -- ) ~is-character bit-on ;
: be-animal ( a -- ) ~is-animal bit-on ;

: be-light-source ( a -- ) ~is-light-source bit-on ;
: be-lightable ( a -- ) ~is-lightable bit-on ;
: be-lighted ( a -- ) ~is-lighted bit-on ;
: be-not-lighted ( a -- ) ~is-lighted bit-off ;

: be-wearable ( a -- ) ~is-wearable bit-on ;
: be-worn ( a -- ) ~is-worn bit-on ;
: be-not-worn ( a -- ) ~is-worn bit-off ;

: be-decoration ( a -- ) ~is-decoration bit-on ;
: be-not-listed ( a -- f ) ~is-not-listed bit-on ;
: be-global-indoor ( a -- ) ~is-global-indoor bit-on ;
: be-global-outdoor ( a -- ) ~is-global-outdoor bit-on ;

: be-location ( a -- ) ~is-location bit-on ;
: be-indoor-location ( a -- ) dup be-location ~is-indoor-location bit-on ;
: be-outdoor-location ( a -- ) dup be-location ~is-indoor-location bit-off ;

: be-container ( a -- ) ~is-container bit-on ;
: be-openable ( a -- ) ~is-openable bit-on ;
: be-open ( a -- ) ~is-open bit-on ;
: be-closed ( a -- ) ~is-open bit-off ;
: be-lockable ( a -- ) ~is-lockable bit-on ;
: be-lock ( a -- ) ~is-lock bit-on ;
: be-unlock ( a -- ) ~is-lock bit-off ;
: times-open++ ( a -- ) ~times-open ?1+! ;

: visits++ ( a -- ) ~visits ?1+! ;
: conversations++ ( a -- ) ~conversations ?1+! ;
: familiar++ ( a -- ) ~familiar ?1+! ;

: be-owner ( a1 a2 -- ) swap ~owner ! ;

: be-there ( a1 a2 -- ) ~holder ! ;
: was-there ( a1 a2 -- ) ~previous-holder ! ;

\ ==============================================================
\ Change log

\ 2017-07-07: Add path to local requirements.
\
\ 2017-08-05: Add field offsets of diagonal cardinal points, to
\ support the corresponding connectors.
\
\ 2017-11-10: Rename "location" "holder" to mean a pointer to other
\ entity (location, container, character or "limbo"). This makes the
\ code clearer.
\
\ 2017-11-12: Replace deprecated `++` with `1+!`; rename `/list++`
\ `/list+`.
\
\ 2018-05-04: Update heading and header.
\
\ 2018-05-05: Replace `alias` with `synonym`.  Move this module from
\ Talanto (http://programandala.net/en.program.talanto.html).

\ vim: filetype=gforth
