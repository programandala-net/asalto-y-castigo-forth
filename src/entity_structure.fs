\ entity_structure.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

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

require galope/bit-field-colon.fs \ `bitfield:`

set-current

require ./entity.fs

\ ==============================================================
\ Entity structure

0

\ ----------------------------------------------
\ Identification

field: ~initializer
  \ Execution token of the word that initializes the properties of the
  \ entity.

preserve-to-here

2field: ~name
  \ Address and length of a string that contains the name of the
  \ entity.

field: ~describer
  \ An execution token that describes the entity.

field: ~direction
  \ Offset of the direction field. Used only by entities that are
  \ directions.

\ ----------------------------------------------
\ Counters

field: ~familiar
  \ How much the entity is familiar to the protagonist.

field: ~times-open
  \ Number of times the entity has been open.

field: ~conversations
  \ Number of conversations the (human) entity has have with the
  \ protagonist.

field: ~visits
  \ Number of visits of the protagonist. Used only by location
  \ entities. By convention, the counter should be updated before
  \ leaving the location.

field: ~stamina
  \ Stamina of the entity. Used by living entities.

\ ----------------------------------------------
\ Related entities

field: ~holder
  \ Other entity the entity is at/in (location, container,
  \ character or "limbo").

field: ~previous-holder
  \ Other entity the entity was previously at/in (location,
  \ container, character or "limbo").

field: ~owner
  \ Other entity the entity belongs to (either "in law" or "in fact"),
  \ no matter its current localization.

\ ----------------------------------------------
\ Location plot

\ These fields contain execution tokens.

field: ~enter-checker
  \ Can a location be entered? It contains a flag.

field: ~before-description-plotter
  \ Executed before describing the current location.

field: ~after-description-plotter
  \ Executed after describing the current location.

field: ~before-prompt-plotter
  \ Executed after listing the things present in the location, and
  \ before showing the command prompt.

field: ~before-exit-plotter
  \ Executed before leaving the current location.

\ ----------------------------------------------
\ Exits

\ These fields contain the entity the corresponding exit leads to.

field: ~north-exit
field: ~north-east-exit
field: ~north-west-exit
field: ~south-exit
field: ~south-east-exit
field: ~south-west-exit
field: ~east-exit
field: ~west-exit
field: ~up-exit
field: ~down-exit
field: ~out-exit
field: ~in-exit

\ ----------------------------------------------
\ Flags

\ Flags are implemented as bit fields, and a layer of words is
\ provided apart in order to set and fetch them.

begin-bitfields ~bitfields

  bitfield: ~is-human
    \ Is the entity a human?

  bitfield: ~is-animal
    \ Is the entity an animal (other than a human)?

  bitfield: ~is-character
    \ Is the entity a non-playing character?

  bitfield: ~is-vegetal
    \ Is the entity a vegetal?

  bitfield: ~has-no-article
    \ Does the entity's name must have no article?

  bitfield: ~has-definite-article
    \ Does the article of the entity's name must be definite?

  bitfield: ~has-feminine-name
    \ Is the gender of the entity's name feminine?

  bitfield: ~has-personal-name
    \ Is the entity's name personal?

  bitfield: ~has-plural-name
    \ Is the entity's name plural?

  bitfield: ~is-wearable
    \ Is the entity a cloth or something that can be worn?

  bitfield: ~is-worn
    \ Being wearable, is it worn?

  bitfield: ~is-location
    \ Is the entity a location?

  bitfield: ~is-indoor-location
    \ Being a location, is the entity an indoor location?

  bitfield: ~is-global-indoor
    \ Is the entity a global one, common to all indoor locations?
    \ (e.g. floor, walls...)

  bitfield: ~is-global-outdoor
    \ Is the entity a global one, common to all outdoor locations?
    \ (e.g. ground, sky...)

  bitfield: ~is-decoration
    \ Is the entity part of the decoration of its location, and cannot
    \ be manipulated?

  bitfield: ~is-not-listed
    \ Is the entity (for any reason) not listed among the present
    \ things or in the inventory?

  bitfield: ~is-light-source
    \ Is the entity a source of light?

  bitfield: ~is-lighted
    \ Being a light source, is the entity lighted?

  bitfield: ~is-lightable
    \ Being a light source, can the entity be lighted (or turned on
    \ and off)?

  bitfield: ~is-container
    \ Is the entity a container?

  bitfield: ~is-openable
    \ Being a container, can the entity be open?

  bitfield: ~is-open
    \ Being a container, is the entity open?

  bitfield: ~is-lockable
    \ Can the entity be lock?

  bitfield: ~is-lock
    \ Is the entity lock?

end-bitfields constant /bitfields

to /entity
  \ Update size of the record.

\ ==============================================================
\ Change log

\ 2017-08-05: Add diagonal cardinal points fields.
\
\ 2017-11-10: Rename "location" "holder" to mean a pointer to other
\ entity (location, container, character or "limbo"). This makes the
\ code clearer.
\
\ 2018-05-04: Update heading.
\
\ 2018-05-05: Move this module from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ vim: filetype=gforth
