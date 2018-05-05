\ errors.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ XXX UNDER DEVELOPMENT -- Being adapted from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ Author: Marcos Cruz (programandala.net), 2016, 2017, 2018.

\ Last modified 201805052248
\ See change log at the end of the file

\ ==============================================================
\ Requirements

\ require ./parser.data.fs

\ ==============================================================
\ Error launchers

0 constant min-errors-verbosity

2 constant max-errors-verbosity

: verbosity-in-range ( n -- n' )
  min-errors-verbosity max max-errors-verbosity min ;

: perform-error ( a1 a2 -- ) dup >r @ verbosity-in-range cells +
perform  r> throw ;
  \ Execute the execution token which is in the verbosity table _a1_
  \ and pointed by the content of _a2_.
  \
  \ XXX TODO -- _a2_ is used as throw code, but synonyms of the
  \ variables will be helpful in order to let the application manage
  \ the language and action errors apart after `catch`, if needed.

\ ----------------------------------------------
\ Language error launcher

: default-generic-language-error ( ca len -- )
  2drop s" Language error." /ltype ;
  \ Discard the specific language error _ca len_ and show a generic
  \ one instead.

defer generic-language-error ( ca len -- )
' default-generic-language-error is generic-language-error
  \ Discard the specific language error _ca len_ and show a generic
  \ one instead.
  \ Configurable by the application.

defer specific-language-error ( ca len -- )
' /ltype is specific-language-error
  \ Show a specific language error _ca len_.
  \ Configurable by the application.

variable language-errors-verbosity
1 language-errors-verbosity !
  \ 0 = show no error message
  \ 1 = show a configurable generic error message
  \ 2 = show a specific error message

create language-errors-verbosity-table
  ' 2drop ,
  ' generic-language-error ,
  ' specific-language-error ,
  \ Execution table of the language error verbosity (0..2).

: language-error ( ca len -- )
  language-errors-verbosity-table
  language-errors-verbosity perform-error ;
  \ Show a language error, depending on the contents of
  \ `language-errors-verbosity`. _ca len_ is a specific message, used
  \ when `language-errors-verbosity` is 2, else discarded.

\ ----------------------------------------------
\ Action error launcher

: default-generic-action-error ( ca len -- )
  2drop s" Action error." /ltype ;
  \ Discard the specific action error _ca len_ and show a generic one
  \ instead.

defer generic-action-error ( ca len -- )
' default-generic-action-error is generic-action-error
  \ Discard the specific action error _ca len_ and show a generic
  \ one instead.
  \ Configurable by the application.

defer specific-action-error ( ca len -- )
' /ltype is specific-action-error
  \ Show a specific action error _ca len_.
  \ Configurable by the application.

variable action-errors-verbosity
1 action-errors-verbosity !
  \ 0 = show no error message
  \ 1 = show a configurable generic error message
  \ 2 = show a specific error message

create action-errors-verbosity-table
  ' 2drop ,
  ' generic-action-error ,
  ' specific-action-error ,
  \ Execution table of the action error verbosity (0..2).

: action-error ( ca len -- )
  action-errors-verbosity-table
  action-errors-verbosity perform-error ;
  \ Show an action error, depending on the contents of
  \ `action-errors-verbosity`. _ca len_ is a specific message, used
  \ when `action-errors-verbosity` is 2, else discarded.

\ ==============================================================
\ Errors

\ ----------------------------------------------
\ Language errors

defer main+destination-complements-required.error ( -- )
defer main+origin-complements-required.error ( -- )
defer main+secondary-complements-required.error ( -- )
defer main+tool-complements-required.error ( -- )
defer no-main-complement.error ( -- )
defer no-verb.error ( -- )
defer not-allowed-complements.error ( -- )
defer not-allowed-main-complement.error ( -- )
defer not-allowed-tool-complement.error ( -- )
defer repeated-preposition.error ( -- )
defer secondary+topic-complements-required.error ( -- )
defer too-many-actions.error ( -- )
defer too-many-complements.error ( -- )
defer unexpected-main-complement.error ( -- )
defer unexpected-secondary-complement.error ( -- )
defer unresolved-preposition.error ( -- )

\ ----------------------------------------------
\ Action errors

defer cannot-be-seen.error ( a -- ) \ XXX TODO -- rename to `is-not-lookable`
defer dangerous.error ( -- )
defer do-not-worry.error ( -- )
defer impossible-move-to-it.error ( a -- ) \ XXX TODO -- rename to `is-impossible-to-go-to`
defer impossible.error ( -- )
defer is-closed.error ( a -- )
defer is-hold.error ( a -- ) \ XXX TODO -- rename to `is-hold-by-me`
defer is-needed.error ( a -- )
defer is-not-here.error ( a -- )
defer is-not-hold.error ( a -- ) \ XXX TODO -- rename to `is-not-hold-by-me`
defer is-not-worn-by-me.error ( a -- )
defer is-open.error ( a -- )
defer is-worn-by-me.error ( a -- )
defer no-reason-for-that.error ( ca len -- )
defer no-reason.error ( -- )
defer nonsense.error ( -- )
defer not-by-hand.error ( -- )
defer that-is-dangerous.error ( ca len -- )
defer that-is-impossible.error ( ca len -- )
defer that-is-nonsense.error ( ca len -- )
defer unnecessary-tool-for-that.error ( ca len a -- ) \ XXX TODO -- rename to `is-...`
defer unnecessary-tool.error ( a -- ) \ XXX TODO -- rename to `is-...`
defer useless-tool.error ( a -- )
defer wrong-tool.error ( a -- )

\ ==============================================================
\ Change log

\ 2017-07-07: Update code after the renaming of the library.  Add path
\ to local requirements.
\
\ 2018-05-04: Remove requiring `??`.
\
\ 2018-05-05: Update comment. Move this module from Talanto
\ (http://programandala.net/en.program.talanto.html).

\ vim: filetype=gforth
