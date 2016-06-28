\ error_codes.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.program.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201606281811

\ ==============================================================

\ The execution token of the word that manages the error is used also
\ as `throw` code. So empty values are defined first.

0 value cannot-see-error#
0 value cannot-see-what-error#
0 value dangerous-error#
0 value impossible-error#
0 value is-normal-error#
0 value is-not-here-error#
0 value is-not-here-what-error#
0 value no-main-complement-error#
0 value no-verb-error#
0 value nonsense-error#
0 value not-allowed-main-complement-error#
0 value useless-tool-error#
0 value useless-what-tool-error#
0 value not-allowed-tool-complement-error#
0 value repeated-preposition-error#
0 value too-many-actions-error#
0 value too-many-complements-error#
0 value unexpected-main-complement-error#
0 value unexpected-secondary-complement-error#
0 value unnecessary-tool-error#
0 value unnecessary-tool-for-that-error#
0 value unresolved-preposition-error#
0 value what-is-already-closed-error#
0 value what-is-already-open-error#
0 value you-already-have-it-error#
0 value you-already-have-what-error#
0 value you-already-wear-what-error#
0 value you-do-not-have-it-error#
0 value you-do-not-have-what-error#
0 value you-do-not-wear-what-error#
0 value you-need-what-error#

\ vim:filetype=gforth
