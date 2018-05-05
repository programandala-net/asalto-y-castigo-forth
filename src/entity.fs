\ entity.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Description: This file provides `entity`, the entity definer, and
\ `:init`, an entity-initializer definer.  The entity data is stored
\ in the heap.

\ XXX UNDER DEVELOPMENT -- Being adapted from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ Author: Marcos Cruz (programandala.net), 2016, 2017, 2018.

\ Last modified 201805052247
\ See change log at the end of the file

\ ==============================================================
\ Requirements

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/question-execute.fs \ `?execute`
require galope/slash-colon-sys.fs  \ `/colon-sys`

set-current

\ ==============================================================

0 value /entity
  \ Size of the entity data structure; it must be updated by the
  \ application before creating entities with `entity`.

cell value /preserved-entity
  \ Size of the region, at the start of the entity structure, that
  \ must be preserved from erasing during the initialization of the
  \ entity, because it contains data calculated during compilation
  \ (e.g., by default, the xt of the initialization code itself).

: preserve-to-here ( n -- n ) dup to /preserved-entity ;
  \ Set _n_ as the size of the region, at the start of the entity
  \ structure, that must be preserved from erasing. This word must be
  \ used by the application in the definition of its own entity
  \ structure, right after the fields that must be preserved.

defer init-new-entity ( a -- ) ' drop is init-new-entity
  \ Initialize entity _a_ with its default data; this word is a hook
  \ for the the application.

0 value #entities
  \ Number of defined entities, automatically updated.

: /entities ( -- len ) #entities /entity * ;
  \ Size of the entities data table.

/entities allocate throw value entities

: #>entity ( n -- a ) /entity * entities + ;
  \ Convert entity ordinal number _n_ to its data address _a_.

: entity># ( a -- n ) entities - /entity / ;
  \ Convert entity data address _a_ to its ordinal number _n_.

: resize-entities ( -- )
  entities /entities resize throw  to entities ;
  \ Resize the entities data table after the current number of
  \ entities.

: erase-whole-entity ( a -- ) /entity erase ;
  \ Erase the whole data of entity _a_.

: erase-entity ( a -- )
  /entity /preserved-entity /string erase ;
  \ Erase the data of entity _a_, but not the preserved fields
  \ at the start of the structure.

: new-entity ( -- )
  #entities  dup 1+ to #entities  resize-entities
             #>entity dup erase-whole-entity init-new-entity ;
  \ Add a new entity to the entities table
  \ and init it with default values.

: entity ( "name" -- )
  create  #entities , new-entity
  does> ( -- a ) ( pfa ) @ #>entity ;
  \ Create an entity identifier _name_.
  \ When _name_ is executed, it will return the entity data address.

0 value self~
  \ Entity that is being initialized in a word defined by `:init`.

defer initializer-entity-field ( a1 -- a2 )
  \ Field of the data structure that holds the xt of the
  \ initialization word. It must be set by the application.

: init-entities ( -- )
  #entities 0 do
    i #>entity initializer-entity-field @ ?execute
  loop ;
  \ Init all entities to their default status.

defer init-entity ( a -- ) ' drop is init-entity
  \ Initialize entity _a_ with its default data; this word is a hook
  \ for the the application.

: [:init] ( a -- )
  dup to self~  dup init-entity  erase-entity ;
  \ Start initialization of entity _a_. This word is executed every
  \ time the original data of the entity must be restored, before the
  \ initialization code programmed by the application.  The entity
  \ identifier _a_ is on the stack because it was compiled by
  \ `(:init)`.

: (:init) ( a xt -- )
  over initializer-entity-field !  postpone literal ;
  \ Preparation tasks before initializing entity _a_.  This word is
  \ executed only once, at the start of the compilation of the word
  \ (_xt_) that initializes the entity.

: (apart-colon-sys) ( x1 x2 colon-sys -- x1 colon-sys x2 )
                    ( x1 colon-sys x2 -- colon-sys x2 x1 )
  [ /colon-sys 1+ ] literal roll ;

: apart-colon-sys ( a xt colon-sys -- colon-sys a xt )
  (apart-colon-sys) (apart-colon-sys) ;

: :init ( a -- )
  :noname apart-colon-sys (:init)  postpone [:init] ;
  \ Start a definition that will initialize entity _a_ to its default
  \ data.  The definition is finished by `;`.
  \
  \ XXX TODO -- Rewrite, simplify: don't depend on _colon-sys_.

\ ==============================================================
\ Change log

\ 2016-06-29: Move from _La pistola de agua_
\ (http://programandala.net/es.programa.la_pistola_de_agua.html).
\
\ 2016-07-04: Add support to preserve a region at the start of the
\ data structure. Move `:init`, `init-entities` and related words from
\ _Asalto y castigo_
\ (http://programandala.net/es.programa.asalto_y_castigo.forth.html).
\
\ 2017-08-01: Adapt `:init` to Gforth 0.7.9, which uses 4 cells for a
\ _colon-sys_, one more than Gforth 0.7.3: Calculate the size of
\ _colon-sys_ to make the code work in both versions.
\
\ 2017-08-04: Fix calculation of `/colon-sys`.
\
\ 2017-08-19: Move `/colon-sys` to Galope.
\
\ 2018-05-04: Update heading.
\
\ 2018-05-05: Move this module from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ vim: filetype=gforth
