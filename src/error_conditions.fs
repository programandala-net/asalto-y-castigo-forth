\ error_conditions.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

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

require galope/question-question.fs  \ `??`

\ Talanto
\ http://programandala.net/en.program.talanto.html

require talanto/different-question.fs \ `different?`

set-current

require ./errors.fs

\ ==============================================================

\ ----------------------------------------------
\ Error checks

: different-tool? ( a -- f )
  tool-complement swap over different? ;
  \ Is entity _a_ different from the used tool, if any?

\ ----------------------------------------------
\ Complement combinations

: complements= ( x -- f ) used-complements @ = ;
  \ Is _x_ the bitmask of all used complements?

: main+secondary-complements% ( -- x )
  main-complement% secondary-complement% or ;
  \ Bitmask of main and secondary complements.

: main+secondary+tool-complements% ( -- x )
  main+secondary-complements% tool-complement% or ;
  \ Bitmask of main, secondary and tool complements.

: main+tool-complements% ( -- x )
  main-complement% tool-complement% or ;
  \ Bitmask of main and tool complements.

: secondary+topic-complements% ( -- x )
  secondary-complement% topic-complement% or ;
  \ Bitmask of secondary and topic complements.

: secondary+topic+tool-complements% ( -- x )
  secondary+topic-complements% tool-complement% or ;
  \ Bitmask of secondary, topic and tool complements.

: main+destination-complements% ( -- x )
  main-complement% destination-complement% or ;
  \ Bitmask of main and destination complements.

: main+destination+tool-complements% ( -- x )
  main+destination-complements% tool-complement% or ;
  \ Bitmask of main, destination and tool complements.

: main+origin-complements% ( -- x )
  main-complement% origin-complement% or ;
  \ Bitmask of main and origin complements.

: main+origin+tool-complements% ( -- x )
  main+origin-complements% tool-complement% or ;
  \ Bitmask of main, destination and tool complements.

\ ----------------------------------------------
\ Language error conditions

: ?no-main-complement ( -- )
  main-complement 0<> ?? unexpected-main-complement.error ;
  \ Cause an error if the main complement is present.

: ?no-secondary-complement ( -- )
  secondary-complement 0<> ?? unexpected-secondary-complement.error ;
  \ Cause an error if the secondary complement is present.

: ?main-complement ( -- )
  main-complement 0= ?? no-main-complement.error ;
  \ Cause an error if the main complement is missing.

: ?this-main-complement ( a -- )
  main-complement swap over different?
  ?? not-allowed-main-complement.error ;
  \ Cause an error if the main complement is present and it's not
  \ entity _a_.

: ?main+secondary-complements ( -- x )
  main+secondary-complements% complements= 0=
  ?? main+secondary-complements-required.error ;
  \ Cause an error if the complements are other than
  \ the main and the secondary.

: ?main+destination-complements ( -- x )
  main+destination-complements% complements= 0=
  ?? main+destination-complements-required.error ;
  \ Cause an error if the complements are other than
  \ the main and the destination.

: ?main+origin-complements ( -- x )
  main+origin-complements% complements= 0=
  ?? main+origin-complements-required.error ;
  \ Cause an error if the complements are other than
  \ the main and the origin.

: ?main+tool-complements ( -- x )
  main+tool-complements% complements= 0=
  ?? main+tool-complements-required.error ;
  \ Cause an error if the complements are other than
  \ the main and the tool.

: ?secondary+topic-complements ( -- x )
  secondary+topic-complements% complements= 0=
  ?? secondary+topic-complements-required.error ;
  \ Cause an error if the complements are other than
  \ the secondary and the topic.

\ ----------------------------------------------
\ Action error conditions

: ?no-tool-complement ( -- )
  tool-complement ?? not-allowed-tool-complement.error ;
  \ Cause an error if there's a tool complement.

: ?no-tool-complement-for-that ( ca len -- )
  tool-complement ?dup
  if unnecessary-tool-for-that.error else 2drop then ;
  \ Cause an error if there's a tool complement. The string _ca len_
  \ contains the action the tool is unnecessary for (a sentence with
  \ the verb in infinitive).

: ?this-tool ( a -- )
  different-tool?
  if tool-complement useless-tool.error then ;
  \ Cause an error if there's a tool complement, and it's not
  \ entity _a_.

: ?not-this-tool ( a -- )
  dup different-tool?
  if drop else useless-tool.error then ;
  \ Cause an error if entity _a_ is used as tool complement.

: ?hold ( a -- )
  dup is-hold? if drop else is-not-hold.error then ;
  \ Cause an error if entity _a_ is not hold by the protagonist.

: ?not-hold ( a -- )
  dup is-not-hold? if drop else is-hold.error then ;
  \ Cause an error if entity _a_ is hold by the protagonist.

: ?worn-by-me ( a -- )
  dup is-worn-by-me? if drop else is-not-worn-by-me.error then ;
  \ Cause an error if entity _a_ is not worn by the protagonist.

: ?not-worn-by-me ( a -- )
  dup is-not-worn-by-me? if drop else is-worn-by-me.error then ;
  \ Cause an error if entity _a_ is worn by the protagonist.

: ?wearable ( a -- )
  is-not-wearable? if nonsense.error then ;
  \ Cause an error if entity _a_ is not wearable.
  \ XXX TODO -- specific error

: ?open ( a -- )
  dup is-open? if drop else is-closed.error then ;
  \ Cause an error if entity _a_ is not open.

: ?closed ( a -- )
  dup is-closed? if drop else is-open.error then ;
  \ Cause an error if entity _a_ is not closed.

: ?here ( a -- )
  dup is-here? if drop else is-not-here.error then ;
  \ Cause an error if entity _a_ is not here.

: ?accessible ( a -- )
  dup is-accessible? if drop else cannot-be-seen.error then ;
  \ Cause an error if entity _a_ is not accessible.
  \ XXX TODO -- specific error

: ?lookable ( a -- )
  dup can-be-looked-at? if drop else cannot-be-seen.error then ;
  \ Cause an error if entity _a_ is not lookable.

: ?living ( a -- )
  is-living-being? 0= if nonsense.error then ;
  \ Cause an error if entity _a_ is not a living being (no matter if
  \ it's dead).
  \ XXX TODO -- specific error

: ?beast ( a -- )
  is-beast? 0= if nonsense.error then ;
  \ Cause an error if entity _a_ is not a beast (animal or human).
  \ XXX TODO -- specific error

: ?needed ( a -- )
  dup is-hold? if drop else is-needed.error then ;
  \ Cause an error if entity _a_ is not hold by the protagonist,
  \ because it's needed.

: ?direction ( a -- )
  is-direction? 0= if nonsense.error then ;
  \ Cause an error if entity _a_ is not a direction.
  \ XXX TODO -- specific error

\ ==============================================================
\ Change log

\ 2017-07-07: Add path to local requirements.
\
\ 2018-05-04: Require `??`. Update heading.
\
\ 2018-05-05: Move this module from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ vim: filetype=gforth
