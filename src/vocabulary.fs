\ vocabulary.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2017

\ Last modified 201712041247
\ See change log at the end of the file

\ Note: Most comments of the code are in Spanish.

\ ==============================================================

get-current forth-wordlist set-current

\ Galope
\ http://programandala.net/en.program.galope.html

require galope/bracket-false.fs       \ `[false]`
require galope/aliases.fs             \ `aliases`
require galope/between.fs             \ `between`
require galope/question-question.fs   \ `??`
require galope/stringer.fs            \ Circular string buffer
require galope/system-colors.fs       \ `system-colors`

set-current

require talanto/last-complements.es.fs

\ ==============================================================
\ Resolución de entes ambiguos

\ Algunos términos del vocabulario del jugador pueden
\ referirse a varios entes. Por ejemplo, «hombre» puede
\ referirse al jefe de los refugiados o a Ambrosio,
\ especialmente antes de que Ulfius hable con él por primera
\ vez y sepa su nombre.  Otra palabra, como «ambrosio», solo
\ debe ser reconocida cuando Ambrosio ya se ha presentado
\ y ha dicho su nombre.
\
\ Para estos casos creamos palabras que devuelven el ente
\ adecuado en función de las circunstancias.  Serán llamadas
\ desde la palabra correspondiente del vocabulario del
\ jugador.
\
\ Si la ambigüedad no puede ser resuelta, o si la palabra ambigua
\ no debe ser reconocida en las circunstancias de juego actuales,
\ se devolverá un cero, que tendrá el mismo efecto que si la
\ palabra ambigua no existiera en el comando del jugador. Esto
\ provocará después el error adecuado.
\
\ Las acciones ambiguas, como por ejemplo «partir» (que puede
\ significar «marchar» o «romper») no pueden ser resueltas de
\ esta manera, pues antes es necesario que todos los
\ términos de la frase hayan sido evaluados. Por ello se
\ tratan como si fueran acciones como las demás, pero que al
\ ejecutarse resolverán la ambigüedad y llamarán a la acción
\ adecuada.

: unambiguous-man ( -- a | 0 )
  leader~ is-here?            if  leader~    exit  then
  ambrosio~ is-here?          if  ambrosio~  exit  then
  pass-still-open? battle? or if  soldiers~  exit  then
  0 ;
  \ Devuelve el ente _a_ adecuado a la palabra «hombre» y sus
  \ sinónimos (o cero si la ambigüedad no puede ser resuelta).  Puede
  \ referirse al líder de los refugiados (si está presente), a
  \ Ambrosio (si está presente), o a los soldados (durante la marcha o
  \ la batalla).

: unambiguous-men ( -- a | 0 )
  location-28~ am-i-there? location-29~ am-i-there? or
  if  refugees~ exit  then
  pass-still-open? battle? or if  soldiers~ exit  then  0 ;
  \ Devuelve el ente _a_ adecuado a la palabra «hombres» y sus sinónimos
  \ (o cero si la ambigüedad no puede ser resuelta).
  \ Puede referirse a los soldados o a los refugiados.

: unambiguous-ambrosio ( -- a | 0 )
  ambrosio~ dup conversations? and ;
  \ Devuelve el ente _a_ adecuado a la palabra «ambrosio», o cero si
  \ el protagonista aún no conoce a Ambrosio.

: unambiguous-cave ( -- a | 0 )
  cave~ my-holder location-10~ location-47~ between and ?dup ?exit
  cave-entrance~ dup is-accessible? and ?dup ?exit
  0 ;
  \ Devuelve el ente _a_ adecuado a la palabra «cueva»
  \ (o cero si la ambigüedad no puede ser resuelta).

: unambiguous-entrance ( -- a | 0 )
  cave-entrance~ is-accessible? if  cave-entrance~ exit  then
  0 ;
  \ Devuelve el ente _a_ adecuado a la palabra «entrada»
  \ (o cero si la ambigüedad no puede ser resuelta).
  \
  \ XXX TODO -- quizá no se implemente esto porque precisaría asociar
  \ a cave-entrance~ el vocablo «salida/s», lo que crearía una
  \ ambigüedad adicional que resolver: `location-10~ am-i-there? if
  \ cave-entrance~ exit  then`

: unambiguous-exits ( -- a )
  cave-entrance~ is-accessible?
  if  cave-entrance~  else  exits~  then ;
  \ Devuelve el ente _a_ adecuado a la palabra «salida/s».

: unambiguous-stone ( -- a )
  stone~ is-accessible?    if  stone~        exit  then
  emerald~ is-accessible?  if  emerald~      exit  then
  location-08~ am-i-there? if  ravine-wall~  exit  then
  rocks~ ;
  \ Devuelve el ente adecuado a la palabra «piedra».  Puede referise,
  \ en orden preferente, a la piedra, a la esmeralda, a la pared de
  \ roca del desfiladero o a las rocas.
  \ XXX TODO simplificar, sin `case`.

: unambiguous-wall ( -- a )
  location-08~ am-i-there?
  if  ravine-wall~  else  wall~  then ;
  \ Devuelve el ente adecuado a la palabra «pared».
  \ XXX TODO -- probablemente habrá que añadir más casos

: unambiguous-somebody ( -- a | 0 )
  pass-still-open? battle? or
  if  soldiers~  exit  then
  location-28~ am-i-there? location-29~ am-i-there? or
  if  refugees~  exit  then
  ambrosio~ is-here? if  ambrosio~  exit  then  0 ;
  \ Devuelve el ente adecuado a la palabra «alguien».  (o _0_ si
  \ la ambigüedad no puede ser resuelta).  Puede referirse a los
  \ soldados, a los refugiados o a ambrosio.

: unambiguous-bridge ( -- a | 0 )
  location-13~ am-i-there? if  bridge~  exit  then
  location-18~ am-i-there? if  arch~    exit  then
  bridge~ is-known?        if  bridge~  exit  then
  arch~ is-known?          if  arch~    exit  then  0 ;
  \ Devuelve el ente adecuado a la palabra «puente».

\ ==============================================================
\ Vocabulario

: known?? ( a "name" -- a true | false )
  dup is-known? dup 0= if  nip  then  postpone ?? ;
  \ If entity _a_ is known, return _a true_, else return _false_;
  \ execute `??` to compile an `if` for _name_.
  \
  \ XXX REMARK -- Maybe used in the definition of some words of the
  \ player vocabulary. Not used yet.

player-wordlist dup >order set-current
  \ El vocabulario del juego está implementado como una lista de
  \ palabras de Forth, creado con el nombre de `player-wordlist`.

\ ----------------------------------------------
\ Pronombres

\ De momento no se implementan las formas sin tilde porque obligarían
\ a distinguir sus usos como adjetivos o sustantivos.

\ XXX TODO
\ esto/s eso/s y aquello/s podrían implementarse como sinónimos
\ de las formas masculinas o bien referirse al último y penúltimo
\ complemento usado, de cualquier género, pero para ello la estructura
\ de la tabla `last-complement` debería ser modificada.
\ : esto  last-complement @ set-complement ;
\ : aquello  last-but-one-complement @ set-complement ;

: éste ( -- ) last-complement >masculine-singular @ set-complement ;

' éste aliases ése end-aliases

: ésta ( -- ) last-complement >feminine-singular @ set-complement ;

' ésta aliases ésa end-aliases

: éstos ( -- ) last-complement >masculine-plural @ set-complement ;

' éstos aliases ésos end-aliases

: éstas ( -- ) last-complement >feminine-plural @ set-complement ;

' éstas aliases ésas end-aliases

: aquél ( -- ) last-but-one-complement >masculine-singular @ set-complement ;

: aquélla ( -- ) last-but-one-complement >feminine-singular @ set-complement ;

: aquéllos ( -- ) last-but-one-complement >masculine-plural @ set-complement ;

: aquéllas ( -- ) last-but-one-complement >feminine-plural @ set-complement ;

\ ----------------------------------------------
\ Verbos

: ir ['] do-go set-action ;

' ir aliases
  dirigirme diríjame diríjome
  dirigirse dirigíos  diríjase
  dirigirte diríjote dirígete
  irme voyme váyame
  irse váyase
  irte vete
  moverme muévame muévome
  moverse muévase moveos
  moverte muévete
  ve id idos voy vaya
  marchar marcha marchad marcho marche
  end-aliases

: abrir  ['] do-open set-action ;

' abrir aliases abre abrid abro abra end-aliases

: abrirlo ( -- ) abrir éste ;

' abrirlo aliases ábrelo abridlo ábrolo ábralo end-aliases

: abrirla ( -- ) abrir ésta ;

' abrirla aliases ábrela abridla ábrola ábrala end-aliases

: abrirlos ( -- ) abrir éstos ;

' abrirlos aliases ábrelos abridlos ábrolos ábralos end-aliases

: abrirlas ( -- ) abrir éstas ;

' abrirlas aliases ábrelas abridlas ábrolas ábralas end-aliases

: cerrar  ['] do-close set-action ;

' cerrar aliases cierra cerrad cierro end-aliases

: cerrarlo ( -- ) cerrar éste ;

' cerrarlo aliases ciérralo cerradlo ciérrolo ciérrelo end-aliases

: cerrarla ( -- ) cerrar ésta ;

' cerrarla aliases ciérrala cerradla ciérrola ciérrela end-aliases

: cerrarlos ( -- ) cerrar éstos ;

' cerrarlos aliases ciérralos cerradlos ciérrolos ciérrelos end-aliases

: cerrarlas ( -- ) cerrar éstas ;

' cerrarlas aliases ciérralas cerradlas ciérrolas ciérrelas end-aliases

: coger  ['] do-take set-action ;

' coger aliases
  coge coged cojo coja
  agarrar agarra agarrad agarro agarre
  recoger recoge recoged recojo recoja
  end-aliases

: cogerlo ( -- ) coger éste ;

' cogerlo aliases
  cógelo cogedlo cójolo cójalo
  agarrarlo agárralo agarradlo agárrolo agárrelo
  recogerlo recógelo recogedlo recójolo recójalo
  end-aliases

: cogerla ( -- ) coger ésta ;

' cogerla aliases
  cógela cogedla cójola cójala
  agarrarla agárrala agarradla agárrola agárrela
  recogerla recógela recogedla recójola recójala
  end-aliases

: cogerlos ( -- ) coger éstos ;

' cogerlos aliases
  cógelos cogedlos cójolos cójalos
  agarrarlos agárralos agarradlos agárrolos agárrelos
  recogerlos recógelos recogedlos recójolos recójalos
  end-aliases

: cogerlas ( -- ) coger éstas ;

' cogerlas aliases
  cógelas cogedlas cójolas cójalas
  agarrarlas agárralas agarradlas agárrolas agárrelas
  recogerlas recógelas recogedlas recójolas recójalas
  end-aliases

: tomar  ['] do-take|do-eat set-action ;
  \ XXX TODO -- inconcluso
' tomar  aliases
  toma tomad tomo tome
  end-aliases

: tomarlo ( -- ) tomar éste ;

' tomarlo aliases tómalo tomadlo tómolo tómelo end-aliases

: dejar  ['] do-drop set-action ;

' dejar aliases
  deja dejad dejo deje
  soltar suelta soltad suelto suelte
  tirar tira tirad tiro tire
  end-aliases

: dejarlo ( -- ) dejar éste ;

' dejarlo aliases
  déjalo dejadlo déjolo déjelo
  soltarlo suéltalo soltadlo suéltolo suéltelo
  tirarlo tíralo tiradlo tírolo tírelo
  end-aliases

: dejarlos ( -- ) dejar éstos ;

' dejarlos aliases
  déjalos dejadlos déjolos déjelos
  soltarlos suéltalos soltadlos suéltolos suéltelos
  tirarlos tíralos tiradlos tírolos tírelos
  end-aliases

: dejarla ( -- ) dejar ésta ;

' dejarla aliases
  déjala dejadla déjola déjela
  soltarla suéltala soltadla suéltola suéltela
  tirarla tírala tiradla tírola tírela
  end-aliases

: dejarlas ( -- ) dejar éstas ;

' dejarlas aliases
  déjalas dejadlas déjolas déjelas
  soltarlas suéltalas soltadlas suéltolas suéltelas
  tirarlas tíralas tiradlas tírolas tírelas
  end-aliases

: mirar  ['] do-look set-action ;

' mirar aliases
  m mira mirad miro mire
  contemplar contempla contemplad contemplo contemple
  observar observa observad observo observe
  end-aliases

: mirarlo ( -- ) mirar éste ;

' mirarlo aliases
  míralo miradlo mírolo mírelo
  contemplarlo contémplalo contempladlo contémplolo contémplelo
  observarlo obsérvalo observadlo obsérvolo obsérvelo
  end-aliases

: mirarla ( -- ) mirar ésta ;

' mirarla aliases
  mírala miradla mírola mírela
  contemplarla contémplala contempladla contémplola contémplela
  observarla obsérvala observadla obsérvola obsérvela
  end-aliases

: mirarlos ( -- ) mirar éstos ;

' mirarlos aliases
  míralos miradlos mírolos mírelos
  contemplarlos contémplalos contempladlos contémplolos contémplelos
  observarlos obsérvalos observadlos obsérvolos obsérvelos
  end-aliases

: mirarlas ( -- ) mirar éstas ;

' mirarlas aliases
  míralas miradlas mírolas mírelas
  contemplarlas contémplalas contempladlas contémplolas contémplelas
  observarlas obsérvalas observadlas obsérvolas obsérvelas
  end-aliases

: mirarse  ['] do-look-yourself set-action ;

' mirarse aliases
  mírese miraos
  mirarte mírate mírote mírete
  mirarme mírame miradme mírome míreme
  contemplarse contemplaos contémplese
  contemplarte contémplate contémplote contémplete
  contemplarme contémplame contempladme contémplome contémpleme
  observarse obsérvese observaos
  observarte obsérvate obsérvote obsérvete
  observarme obsérvame observadme obsérvome obsérveme
  end-aliases

: otear  ['] do-look-to-direction set-action ;

' otear aliases oteo otea otead otee end-aliases

: x  ['] do-exits set-action ;

: salida  ['] do-exits unambiguous-exits set-action-or-complement ;

' salida aliases salidas end-aliases

: examinar  ['] do-examine set-action ;

' examinar aliases ex examina examinad examino examine end-aliases

: examinarlo ( -- ) examinar éste ;

' examinarlo aliases examínalo examinadlo examínolo examínelo end-aliases

: examinarlos ( -- ) examinar éstos ;

' examinarlos aliases examínalos examinadlos examínolos examínelos end-aliases

: examinarla ( -- ) examinar ésta ;

' examinarla aliases examínala examinadla examínola examínela end-aliases

: examinarlas ( -- ) examinar éstas ;

' examinarlas aliases examínalas examinadlas examínolas examínelas end-aliases

: examinarse  ['] do-examine set-action protagonist~ set-complement ;

' examinarse aliases
  examínese examinaos
  examinarte examínate examínete
  examinarme examíname examinadme examínome examíneme
  end-aliases

: registrar  ['] do-search set-action ;

' registrar aliases registra registrad registro registre end-aliases

: registrarlo ( -- ) registrar éste ;

' registrarlo aliases regístralo registradlo regístrolo regístrelo end-aliases

: registrarla ( -- ) registrar ésta ;

' registrarla aliases regístrala registradla regístrola regístrela end-aliases

: registrarlos ( -- ) registrar éstos ;

' registrarlos aliases regístralos registradlos regístrolos regístrelos end-aliases

: registrarlas ( -- ) registrar éstas ;

' registrarlas aliases regístralas registradlas regístrolas regístrelas end-aliases

: i  ['] do-inventory inventory~ set-action-or-complement ;

' i aliases inventario end-aliases

: inventariar  ['] do-inventory set-action ;

' inventariar aliases
  inventaría inventariad inventarío inventaríe
  registrarse regístrase regístrese
  registrarme regístrame registradme regístrome regístreme
  registrarte regístrate regístrote regístrete
  end-aliases

: hacer  ['] do-do set-action ;

' hacer aliases haz haced hago haga end-aliases

: hacerlo ( -- ) hacer éste ;

' hacerlo aliases hazlo hacedlo hágolo hágalo end-aliases

: hacerla ( -- ) hacer ésta ;

' hacerla aliases hazla hacedla hágola hágala end-aliases

: hacerlos ( -- ) hacer éstos ;

' hacerlos aliases hazlos hacedlos hágolos hágalos end-aliases

: hacerlas ( -- ) hacer éstas ;

' hacerlas aliases hazlas hacedlas hágolas hágalas end-aliases

: fabricar  ['] do-make set-action ;

' fabricar aliases
  fabrica fabricad fabrico fabrique
  construir construid construye construyo construya
  end-aliases

: fabricarlo ( -- ) fabricar éste ;

' fabricarlo aliases
  fabrícalo fabricadlo fabrícolo fabríquelo
  construirlo constrúyelo construidlo constrúyolo constrúyalo
  end-aliases

: fabricarla ( -- ) fabricar ésta ;

' fabricarla aliases
  fabrícala fabricadla fabrícola fabríquela
  construirla constrúyela construidla constrúyola constrúyala
  end-aliases

: fabricarlos ( -- ) fabricar éstos ;

' fabricarlos aliases
  fabrícalos fabricadlos fabrícolos fabríquelos
  construirlos constrúyelos construidlos constrúyolos constrúyalos
  end-aliases

: fabricarlas ( -- ) fabricar éstas ;

' fabricarlas aliases
  fabrícalas fabricadlas fabrícolas fabríquelas
  construirlas constrúyelas construidlas constrúyolas constrúyalas
  end-aliases

: nadar  ['] do-swim set-action ;

' nadar aliases
  nada nado nade
  bucear bucea bucead buceo bucee
  sumergirse sumérgese sumérjase
  sumergirme sumérgeme sumérjome sumérjame
  sumergirte sumérgete sumergíos sumérjote sumérjate
  zambullirse zambullíos zambúllese zambúllase
  zambullirme zambúlleme zambúllome zambúllame
  zambullirte zambúllete zambúllote zambúllate
  bañarse báñase báñese
  bañarme báñame báñome báñeme
  bañarte báñate bañaos báñote báñete
  end-aliases

: quitarse  ['] do-take-off set-action ;

' quitarse aliases
  quítase quitaos quítese
  quitarte quítate quítote quítete
  quitarme quítame quítome quíteme
  end-aliases

: quitárselo ( -- ) quitarse éste ;

' quitárselo aliases
  quítatelo
  quitártelo quitáoslo quíteselo
  quitármelo quítamelo quítomelo quítemelo
  end-aliases

: quitársela ( -- ) quitarse ésta ;

' quitársela aliases
  quítatela
  quitártela quitáosla quítesela
  quitármela quítamela quítomela quítemela
  end-aliases

: quitárselos ( -- ) quitarse éstos ;

' quitárselos aliases
  quítatelos
  quitártelos quitáoslos quíteselos
  quitármelos quítamelos quítomelos quítemelos
  end-aliases

: quitárselas ( -- ) quitarse éstas ;

' quitárselas aliases
  quítatelas
  quitártelas quitáoslas quíteselas
  quitármelas quítamelas quítomelas quítemelas
  end-aliases

: ponerse  ['] do-put-on set-action ;

' ponerse aliases
  póngase poneos
  ponerme ponme póngome póngame
  ponerte ponte póngote póngate
  colocarse colocaos colóquese
  colocarte colócate colóquete
  colocarme colócame colócome colóqueme
  end-aliases
  \ XXX TODO -- crear acción. vestir [con], parte como sinónimo y
  \ parte independiente.

: ponérselo ( -- ) ponerse éste ;

' ponérselo aliases
  póngaselo ponéoslo
  ponérmelo pónmelo póngomelo póngamelo
  ponértelo póntelo póngotelo póngatelo
  colocórselo colocáoslo colóqueselo
  colocártelo colócatelo colóquetelo
  colocármelo colócamelo colócomelo colóquemelo
  end-aliases

: ponérsela ( -- ) ponerse ésta ;

' ponérsela aliases
  póngasela ponéosla
  ponérmela pónmela póngomela póngamela
  ponértela póntela póngotela póngatela
  colocórsela colocáosla colóquesela
  colocártela colócatela colóquetela
  colocármela colócamela colócomela colóquemela
  end-aliases

: ponérselos ( -- ) ponerse éstos ;

' ponérselos aliases
  póngaselos ponéoslos
  ponérmelos pónmelos póngomelos póngamelos
  ponértelos póntelos póngotelos póngatelos
  colocórselos colocáoslos colóqueselos
  colocártelos colócatelos colóquetelos
  colocármelos colócamelos colócomelos colóquemelos
  end-aliases

: ponérselas ( -- ) ponerse éstas ;

' ponérselas aliases
  póngaselas ponéoslas
  ponérmelas pónmelas póngomelas póngamelas
  ponértelas póntelas póngotelas póngatelas
  colocórselas colocáoslas colóqueselas
  colocártelas colócatelas colóquetelas
  colocármelas colócamelas colócomelas colóquemelas
  end-aliases

: matar  ['] do-kill set-action ;

' matar aliases
  mata matad mato mate
  asesinar asesina asesinad asesino asesine
  aniquilar aniquila aniquilad aniquilo aniquile
  end-aliases

: matarlo ( -- ) matar éste ;

' matarlo aliases
  mátalo matadlo mátolo mátelo
  asesinarlo asesínalo asesinadlo asesínolo asesínelo
  aniquilarlo aniquínalo aniquinadlo aniquínolo aniquínelo
  end-aliases

: matarla ( -- ) matar ésta ;

' matarla aliases
  mátala matadla mátola mátela
  asesinarla asesínala asesinadla asesínola asesínela
  aniquilarla aniquínala aniquinadla aniquínola aniquínela
  end-aliases

: matarlos ( -- ) matar éstos ;

' matarlos aliases
  mátalos matadlos mátolos mátelos
  asesinarlos asesínalos asesinadlos asesínolos asesínelos
  aniquilarlos aniquínalos aniquinadlos aniquínolos aniquínelos
  end-aliases

: matarlas ( -- ) matar éstas ;

' matarlas aliases
  mátalas matadlas mátolas mátelas
  asesinarlas asesínalas asesinadlas asesínolas asesínelas
  aniquilarlas aniquínalas aniquinadlas aniquínolas aniquínelas
  end-aliases

: golpear  ['] do-hit set-action ;

' golpear aliases
  golpea golpead golpeo golpee
  sacudir sacude sacudid sacudo sacuda
  end-aliases

: golpearla ( -- ) golpear ésta ;

' golpearla aliases
  golpéala golpeadla golpéola golpéela
  sacudirla sacúdela sacudidla sacúdola sacúdala
  end-aliases

: golpearlos ( -- ) golpear éstos ;

' golpearlos aliases
  golpéalos golpeadlos golpéolos golpéelos
  sacudirlos sacúdelos sacudidlos sacúdolos sacúdalos
  end-aliases

: golpearlas ( -- ) golpear éstas ;

' golpearlas aliases
  golpéalas golpeadlas golpéolas golpéelas
  sacudirlas sacúdelas sacudidlas sacúdolas sacúdalas
  end-aliases

: atacar  ['] do-attack set-action ;

' atacar aliases
  ataca atacad ataco ataque
  agredir agrede agredid agredo agreda
  end-aliases

: atacarlo ( -- ) atacar éste ;

' atacarlo aliases
  atácalo atacadlo atácolo atáquelo
  agredirlo agrédelo agredidlo agrédolo agrédalo
  end-aliases

: atacarla ( -- ) atacar ésta ;

' atacarla aliases
  atácala atacadla atácola atáquela
  agredirla agrédela agredidla agrédola agrédala
  end-aliases

: atacarlos ( -- ) atacar éstos ;

' atacarlos aliases
  atácalos atacadlos atácolos atáquelos
  agredirlos agrédelos agredidlos agrédolos agrédalos
  end-aliases

: atacarlas ( -- ) atacar éstas ;

' atacarlas aliases
  atácalas atacadlas atácolas atáquelas
  agredirlas agrédelas agredidlas agrédolas agrédalas
  end-aliases

: romper  ['] do-break set-action ;

' romper aliases
  rompe romped rompo rompa
  despedazar despedaza despedazad despedazo despedace
  destrozar destroza destrozad destrozo destroce
  dividir divide dividid divido divida
  cortar corta cortad corto corte
  end-aliases

: romperlo ( -- ) romper éste ;

' romperlo aliases
  rómpelo rompedlo rómpolo rómpalo
  despedazarlo despedazalo despedazadlo despedázolo despedácelo
  destrozarlo destrózalo destrozadlo destrózolo destrócelo
  dividirlo divídelo divididlo divídolo divídalo
  cortarlo cortalo cortadlo córtolo córtelo
  end-aliases

: romperla ( -- ) romper ésta ;

' romperla aliases
  rómpela rompedla rómpola rómpala
  despedazarla despedazala despedazadla despedázola despedácela
  destrozarla destrózala destrozadla destrózola destrócela
  dividirla divídela divididla divídola divídala
  cortarla córtala cortadla córtola córtela
  end-aliases

: romperlos ( -- ) romper éstos ;

' romperlos aliases
  rómpelos rompedlos rómpolos rómpalos
  despedazarlos despedazalos despedazadlos despedázolos despedácelos
  destrozarlos destrózalos destrozadlos destrózolos destrócelos
  dividirlos divídelos divididlos divídolos divídalos
  cortarlos córtalos cortadlos córtolos córtelos
  end-aliases

: romperlas ( -- ) romper éstas ;

' romperlas aliases
  rómpelas rompedlas rómpolas rómpalas
  despedazarlas despedázalas despedazadlas despedázolas despedácelas
  destrozarlas destrózalas destrozadlas destrózolas destrócelas
  dividirlas divídelas divididlas divídolas divídalas
  cortarlas córtalas cortadlas córtolas córtelas
  end-aliases

\ quebrar \ XXX TODO
\ desgarrar \ XXX TODO

: asustar  ['] do-frighten set-action ;

' asustar aliases
  asusto asusta asustad asuste
  amedrentar amedrento amedrenta amedrentad amedrente
  acojonar acojono acojona acojonad acojone
  atemorizar atemoriza atemorizad atemorizo atemorice
  espanto espanta espantad espante
  aterrorizo aterroriza aterrorizad aterrorice
  ahuyento ahuyenta ahuyentad ahuyente
  end-aliases

: asustarlo ( -- ) asustar éste ;

' asustarlo aliases
  asústolo asústalo asustadlo asústelo
  amedrentarlo amedréntolo amedréntalo amedrentadlo amedréntelo
  acojonarlo acojónolo acojónalo acojonadlo acojónelo
  atemorizarlo atemorízalo atemorizadlo atemorízolo atemorícelo
  espántolo espántalo espantadlo espántelo
  aterrorízolo aterrorízalo aterrorizadlo aterrorícelo
  ahuyéntolo ahuyéntalo ahuyentadlo ahuyéntelo
  end-aliases

: asustarla ( -- ) asustar ésta ;

' asustarla aliases
  asústola asústala asustadla asústela
  amedrentarla amedréntola amedréntala amedrentadla amedréntela
  acojonarla acojónola acojónala acojonadla acojónela
  atemorizarla atemorízala atemorizadla atemorízola atemorícela
  espántola espántala espantadla espántela
  aterrorízola aterrorízala aterrorizadla aterrorícela
  ahuyéntola ahuyéntala ahuyentadla ahuyéntela
  end-aliases

: asustarlos ( -- ) asustar éstos ;

' asustarlos aliases
  asústolos asústalos asustadlos asústelos
  amedrentarlos amedréntolos amedréntalos amedrentadlos amedréntelos
  acojonarlos acojónolos acojónalos acojonadlos acojónelos
  atemorizarlos atemorízalos atemorizadlos atemorízolos atemorícelos
  espántolos espántalos espantadlos espántelos
  aterrorízolos aterrorízalos aterrorizadlos aterrorícelos
  ahuyéntolos ahuyéntalos ahuyentadlos ahuyéntelos
  end-aliases

: asustarlas ( -- ) asustar éstas ;

' asustarlas aliases
  asústolas asústalas asustadlas asústelas
  amedrentarlas amedréntolas amedréntalas amedrentadlas amedréntelas
  acojonarlas acojónolas acojónalas acojonadlas acojónelas
  atemorizarlas atemorízalas atemorizadlas atemorízolas atemorícelas
  espántolas espántalas espantadlas espántelas
  aterrorízolas aterrorízalas aterrorizadlas aterrorícelas
  ahuyéntolas ahuyéntalas ahuyentadlas ahuyéntelas
  end-aliases

: afilar  ['] do-sharpen set-action ;

' afilar aliases afila afilad afilo afile end-aliases

: afilarlo ( -- ) afilar éste ;

' afilarlo aliases afílalo afiladlo afílolo afílelo end-aliases

: afilarla ( -- ) afilar ésta ;

' afilarla aliases afílala afiladla afílola afílela end-aliases

: afilarlos ( -- ) afilar éstos ;

' afilarlos aliases afílalos afiladlos afílolos afílelos end-aliases

: afilarlas ( -- ) afilar éstas ;

' afilarlas aliases afílalas afiladlas afílolas afílelas end-aliases

: partir  ['] do-go|do-break set-action ;

' partir aliases parto partid parta end-aliases
\ «parte» está en la sección final de ambigüedades

: partirlo ( -- ) partir éste ;

' partirlo aliases pártelo pártolo partidlo pártalo end-aliases

: partirla ( -- ) partir ésta ;

' partirla aliases pártela pártola partidla pártala end-aliases

: partirlos ( -- ) partir éstos ;

' partirlos aliases pártelos pártolos partidlos pártalos end-aliases

: partirlas ( -- ) partir éstas ;

' partirlas aliases pártelas pártolas partidlas pártalas end-aliases

: esperar ( -- ) ;
  \ XXX TODO

' esperar aliases
  z espera esperad espero espere
  aguardar aguarda aguardad aguardo aguarde
  end-aliases

: esperarlo ( -- ) esperar éste ;

' esperarlo aliases
  esperadlo espérolo espérelo
  aguardarlo aguárdalo aguardadlo aguárdolo aguárdelo
  end-aliases

: esperarla ( -- ) esperar ésta ;

' esperarla aliases
  esperadla espérola espérela
  aguardarla aguárdala aguardadla aguárdola aguárdela
  end-aliases

: esperarlos ( -- ) esperar éstos ;

' esperarlos aliases
  esperadlos espérolos espérelos
  aguardarlos aguárdalos aguardadlos aguárdolos aguárdelos
  end-aliases

: esperarlas ( -- ) esperar éstas ;

' esperarlas aliases
  esperadlas espérolas espérelas
  aguardarlas aguárdalas aguardadlas aguárdolas aguárdelas
  end-aliases

: escalar  ['] do-climb set-action ;

' escalar aliases escala escalo escale end-aliases
  \ XXX TODO

' escalar aliases trepar trepa trepo trepe end-aliases
  \ XXX TODO

: hablar  ['] do-speak set-action ;
  \ XXX TODO -- Crear nuevas palabras según la preposición que necesiten.
  \ XXX TODO -- Separar matices.

' hablar aliases
  habla hablad hablo hable
  hablarle háblale háblole háblele
  conversar conversa conversad converso converse
  charlar charla charlad charlo charle
  decir di decid digo diga
  decirle dile decidle dígole dígale
  platicar platica platicad platico platique
  platicarle platícale platicadle platícole platíquele
  end-aliases
  \ contar cuenta cuento cuente  \ XXX
  \ contarle cuéntale cuéntole cuéntele  \ XXX

: presentarse  ['] do-introduce-yourself set-action ;

' presentarse aliases
  preséntase preséntese
  presentarte preséntate presentaos preséntete
  end-aliases

  \ XXX TODO:
  \ meter introducir insertar colar encerrar

\ ----------------------------------------------
\ Acciones especiales

: y ( -- ) do-and ;

\ ----------------------------------------------
\ Nombres de objetos o personas

: ulfius ( -- ) ulfius~ set-complement ;

: ambrosio ( -- ) unambiguous-ambrosio set-complement ;

: hombre ( -- ) unambiguous-man set-complement ;

' hombre aliases señor tipo individuo persona end-aliases

: hombres ( -- ) unambiguous-men set-complement ;

' hombres aliases gente personas end-aliases

: jefe ( -- ) leader~ set-complement ;

' jefe aliases
  líder viejo anciano abuelo
  end-aliases
  \ XXX FIXME -- Ambigüedad.: «jefe» podría ser también el jefe de los
  \ enemigos durante la batalla. Pero solo aparece cuando ya no se
  \ puede dar más comandos.

: soldados ( -- ) soldiers~ set-complement ;

' soldados aliases
  soldado
  guerreros luchadores combatientes camaradas
  compañeros oficiales suboficiales militares
  guerrero luchador combatiente camarada
  compañero oficial suboficial militar
  end-aliases

: multitud ( -- ) refugees~ set-complement ;

' multitud aliases
  niño niños niña niñas
  muchacho muchachos muchacha muchachas
  adolescente adolescentes
  ancianos anciana ancianas mayores viejos vieja viejas
  joven jóvenes
  abuela abuelos abuelas
  nieto nietos nieta nietas
  padre padres madre madres mamá mamás papás
  bebé bebés beba bebas bebito bebitos bebita bebitas
  pobres desgraciados desafortunados
  desgraciadas desafortunadas
  muchedumbre masa
  end-aliases

: refugiados ( -- ) leader~ conversations? ?? multitud ;

' refugiados aliases refugiada refugiadas end-aliases

: refugiado ( -- ) leader~ conversations? ?? viejo ;

: altar ( -- ) altar~ set-complement ;

: arco ( -- ) arch~ set-complement ;

: capa ( -- ) cloak~ set-complement ;
  \ XXX TODO -- hijuelo?

' capa aliases lana end-aliases

\ ' capa aliases abrigo end-aliases
  \ XXX TODO -- diferente género

: coraza ( -- ) cuirasse~ set-complement ;

' coraza aliases armadura end-aliases

: puerta ( -- ) door~ set-complement ;

: esmeralda ( -- ) emerald~ set-complement ;

' esmeralda aliases joya end-aliases
  \ XXX TODO -- piedra-preciosa brillante

: derrumbe ( -- ) fallen-away~ set-complement ;

: banderas ( -- ) flags~ set-complement ;

' banderas aliases
    bandera pendones enseñas pendón enseña
    mástil mástiles
    estandarte estandartes
  end-aliases
  \ XXX TODO -- estandarte, enseña... otro género

: dragones ( -- ) flags~ is-known? ?? banderas ;

' dragones aliases dragón end-aliases

: pedernal ( -- ) flint~ set-complement ;

: ídolo ( -- ) idol~ set-complement ;

' ídolo aliases ojo orificio agujero end-aliases
  \ XXX TODO -- separar los sinónimos de ídolo

: llave ( -- ) key~ set-complement ;

' llave aliases hierro herrumbe óxido end-aliases

: lago ( -- ) lake~ set-complement ;

' lago aliases laguna agua estanque end-aliases
  \ XXX TODO -- diferente género

: candado ( -- ) lock~ set-complement ;

' candado aliases cerrojo end-aliases

: tronco ( -- ) log~ set-complement ;

' tronco aliases leño madero end-aliases
  \ XXX TODO -- madera

: trozo ( -- ) piece~ set-complement ;

' trozo aliases pedazo retal tela end-aliases

: harapo ( -- ) rags~ set-complement ;

: rocas ( -- )
  location-09~ am-i-there?
  if  fallen-away~  else  rocks~  then  set-complement ;

' rocas aliases piedras pedruscos end-aliases

: piedra ( -- ) unambiguous-stone set-complement ;

' piedra aliases roca pedrusco end-aliases

: serpiente ( -- ) snake~ set-complement ;

' serpiente aliases reptil ofidio culebra animal bicho end-aliases

: espada ( -- ) sword~ set-complement ;

' espada aliases tizona arma end-aliases
\ XXX Nota.: "arma" es femenina pero usa artículo "él", contemplar en los cálculos de artículo.

: hilo ( -- ) thread~ set-complement ;

' hilo aliases hebra end-aliases

: antorcha ( -- ) torch~ set-complement ;

: cascada ( -- ) waterfall~ set-complement ;

' cascada aliases catarata end-aliases

: catre ( -- ) s" catre" bed~ ms-name! bed~ set-complement ;

' catre aliases camastro end-aliases

: cama ( -- ) s" cama" bed~ fs-name! bed~ set-complement ;

: velas ( -- ) candles~ set-complement ;

' velas aliases vela end-aliases

: mesa ( -- ) table~ set-complement ;

' mesa aliases mesita pupitre end-aliases

: puente ( -- ) unambiguous-bridge set-complement ;

: alguien ( -- ) unambiguous-somebody set-complement ;

: hierba ( -- ) s" hierba" grass~ fs-name! grass~ set-complement ;

: hierbas ( -- ) s" hierbas" grass~ fp-name! grass~ set-complement ;

: hierbajo ( -- ) s" hierbajo" grass~ ms-name! grass~ set-complement ;

: hierbajos ( -- ) s" hierbajos" grass~ mp-name! grass~ set-complement ;

: hiedra ( -- ) s" hiedra" grass~ fs-name! grass~ set-complement ;

: hiedras ( -- ) s" hiedras" grass~ fp-name! grass~ set-complement ;

\ ----------------------------------------------
\ Direcciones y sus acciones asociadas

: n  ['] do-go-north north~ set-action-or-complement ;

' n aliases norte septentrión end-aliases

: s  ['] do-go-south south~ set-action-or-complement ;

' s aliases sur meridión end-aliases

: e  ['] do-go-east east~ set-action-or-complement ;

' e aliases este oriente levante end-aliases

: o  ['] do-go-west west~ set-action-or-complement ;

' o aliases oeste occidente poniente end-aliases

: ar  ['] do-go-up up~ set-action-or-complement ;

' ar aliases arriba end-aliases

: subir  ['] do-go-up set-action ;

' subir aliases sube subid subo suba end-aliases

' subir aliases ascender asciende ascended asciendo ascienda end-aliases

' subir aliases subirse subíos súbese súbase end-aliases

' subir aliases subirte súbete súbote súbate end-aliases

: ab  ['] do-go-down down~ set-action-or-complement ;

' ab aliases abajo end-aliases

: bajar  ['] do-go-down set-action ;

' bajar aliases baja bajad bajo baje end-aliases

' bajar aliases bajarse bajaos bájase bájese end-aliases

' bajar aliases bajarte bájate bájote bájete end-aliases

' bajar aliases descender desciende descended desciendo descienda end-aliases

: salir  ['] do-go-out set-action ;

' salir aliases sal salid salgo salga end-aliases
  \ XXX TODO -- ambigüedad. sal

' salir aliases salirse end-aliases

' salir aliases salirme sálgome end-aliases

' salir aliases salirte end-aliases
  \ XXX TODO -- ambigüedad. salte

: fuera  ['] do-go-out out~ set-action-or-complement ;

' fuera aliases afuera end-aliases

: exterior ( -- ) out~ set-complement ;

: entrar ['] do-go-in set-action ;

' entrar aliases entra entrad entro entre end-aliases

' entrar aliases entrarse entraos éntrese éntrase end-aliases

' entrar aliases entrarte éntrete éntrate end-aliases

: dentro  ['] do-go-in in~ set-action-or-complement ;

' dentro aliases adentro end-aliases

: interior ( -- ) in~ set-complement ;

\ ----------------------------------------------
\ Términos asociados a entes globales o virtuales

: nubes ( -- ) clouds~ set-complement ;
  \ XXX TODO ¿cúmulo-nimbos?, ¿nimbos?

' nubes aliases nube estratocúmulo estratocúmulos cirro cirros end-aliases

: suelo ( -- ) floor~ set-complement ;

' suelo aliases suelos tierra firme end-aliases
  \ XXX TODO -- Añadir «piso», que es ambiguo

: cielo ( -- ) sky~ set-complement ;

' cielo aliases cielos firmamento end-aliases

: techo ( -- ) ceiling~ set-complement ;

: cueva ( -- ) unambiguous-cave set-complement ;

' cueva aliases caverna gruta end-aliases

: entrada ( -- ) unambiguous-entrance set-complement ;

' entrada aliases acceso end-aliases
  \ XXX TODO ¿Implementar cambio de nombre y/o género gramatical?
\ (entrada, acceso).

: enemigo ( -- ) enemy~ set-complement ;

' enemigo aliases enemigos sajón sajones end-aliases

: todo ( -- ) ;
  \ XXX TODO

: pared ( -- ) unambiguous-wall set-complement ;
' pared  aliases muro end-aliases
  \ XXX TODO ¿Implementar cambio de nombre y/o género gramatical?
  \ (pared/es, muro/s).

: paredes ( -- ) wall~ set-complement ;
' paredes  aliases muros end-aliases

\ ----------------------------------------------
\ Artículos

\ Los artículos no hacen nada pero es necesario crearlos
\ para que no provoquen un error cuando el intérprete de
\ comandos funcione en el modo opcional de no ignorar las
\ palabras desconocidas.

: la ( -- ) ;

' la aliases las el los una un unas unos end-aliases

\ ----------------------------------------------
\ Adjetivos demostrativos

\ Lo mismo hacemos con los adjetivos demostrativos
\ y pronombres demostrativos sin tilde; salvo «este», que siempre
\ será interpretado como punto cardinal.

: esta ( -- ) ;

' esta aliases estas estos end-aliases

\ ----------------------------------------------
\ (Seudo)preposiciones

: a ( -- ) secondary-complement# set-preposition ;
  \ Uso: Complemento secundario

' a aliases al end-aliases

: con ( -- ) tool-complement# set-preposition ;
  \ Uso: Herramienta o compañía

' con aliases usando utilizando empleando mediante end-aliases

\ ----------------------------------------------
\ Términos ambiguos

: cierre ( -- ) action if  candado  else  cerrar  then ;

: parte ( -- ) action if  trozo  else  partir  then ;

\ ----------------------------------------------
\ Comandos del sistema

: #recolorea ( -- ) ['] recolor set-action ;
  \ Restaura los colores predeterminados.

: #configura ( "name" | -- )
  parse-name temporary-config-file place
  ['] read-config set-action  ; immediate
  \ Carga el fichero de configuración _name_.  Si no se indica _name_,
  \ se cargará el fichero de configuración predeterminado.

: #reconfigura ( "name" | -- )
  parse-name temporary-config-file place
  ['] get-config set-action  ; immediate
  \ Restaura la configuración predeterminada y después carga el
  \ fichero de configuración _name_.  Si no se indica _name_, se
  \ cargará el fichero de configuración predeterminado.

: #graba ( "name" -- )
  [debug-parsing] [??] ~~
  parse-name >stringer
  [debug-parsing] [??] ~~
  ['] save-the-game set-action
  [debug-parsing] [??] ~~
  ;  immediate
  \ Graba el estado de la partida en un fichero.

: #carga ( "name" -- )
  [debug-parsing] [??] ~~
  parse-name
  [debug-parsing] [??] ~~
  >stringer
  [debug-parsing] [??] ~~
  ['] load-the-game set-action
  [debug-parsing] [??] ~~
  ;  immediate
  \ Carga el estado de la partida de un fichero.

: #fin ( -- ) ['] finish set-action ;
  \ Abandonar la partida

\ : #ayuda ( -- )
\   \ ['] do-help set-action
\  ;
  \ XXX TODO

: #forth ( -- )
  restore-wordlists system-colors cr bootmessage cr quit ;
  \ XXX TMP -- Para usar durante el desarrollo.

: #bye ( -- ) bye ;
  \ XXX TMP -- Para usar durante el desarrollo.

: #.s ( -- ) .s ;

restore-wordlists

\ ==============================================================
\ Change log

\ 2017-11-10: Update to Talanto 0.62.0: replace field notation
\ "location" with "holder".
\
\ 2017-11-17: Add `#.s`.
\
\ 2017-12-04: Update to Galope 0.156.0: Change `aliases: ;aliases` to
\ `aliases end-aliases`.

\ vim:filetype=gforth:fileencoding=utf-8
