\ player_vocabulary.fs
\
\ This file is part of _Asalto y castigo_
\ http://programandala.net/es.programa.asalto_y_castigo.forth.html

\ Author: Marcos Cruz (programandala.net), 2011..2016

\ Last update: 201607071316

\ Note: The comments of the code are in Spanish.

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
\ se devolverá un `false`, que tendrá el mismo efecto que si la
\ palabra ambigua no existiera en el comando del jugador. Esto
\ provocará después el error adecuado.
\
\ Las acciones ambiguas, como por ejemplo «partir» [que puede
\ significar «marchar» o «romper»] no pueden ser resueltas de
\ esta manera, pues antes es necesario que que todos los
\ términos de la frase hayan sido evaluados. Por ello se
\ tratan como si fueran acciones como las demás, pero que al
\ ejecutarse resolverán la ambigüedad y llamarán a la acción
\ adecuada.

: (man) ( -- a | false )
  true case
    leader~ is-here? of  leader~  endof
    ambrosio~ is-here? of  ambrosio~  endof
    pass-still-open? battle? or of  soldiers~  endof
    false swap
  endcase  ;
  \ Devuelve el ente adecuado a la palabra «hombre» y sus sinónimos
  \ (o _false_ si la ambigüedad no puede ser resuelta).
  \ Puede referirse al líder de los refugiados (si está presente),
  \ a Ambrosio (si está presente),
  \ o a los soldados (durante la marcha o la batalla).

: (men)  ( -- a | false )
  [false] [if] \ Primera versión.
    true case
      location-28~ am-i-there? location-29~ am-i-there? or of  refugees~  endof
      pass-still-open? battle? or of  soldiers~  endof
      false swap
    endcase
  [else]  \ Segunda versión, lo mismo pero sin `case`:
    location-28~ am-i-there? location-29~ am-i-there? or
    if  refugees~ exit  then
    pass-still-open? battle? or
    if  soldiers~ exit  then
    false
  [then]  ;
  \ Devuelve el ente adecuado a la palabra «hombres» y sus sinónimos
  \ (o `false` si la ambigüedad no puede ser resuelta).
  \ Puede referirse a los soldados o a los refugiados.

: (ambrosio) ( -- a | false )
  ambrosio~ dup conversations? and  ;
  \ Devuelve el ente adecuado a la palabra «ambrosio»
  \ (o _false_ si la ambigüedad no puede ser resuelta).
  \ La palabra «Ambrosio» es válida únicamente si
  \ el protagonista ha hablado con Ambrosio.

: (cave) ( -- a | false )
  true case
    my-location location-10~ location-47~ between of  cave~  endof
    is-the-cave-entrance-accessible? of  cave-entrance~  endof
    false swap
  endcase  ;
  \ Devuelve el ente adecuado a la palabra «cueva»
  \ (o _false_ si la ambigüedad no puede ser resuelta).

: (entrance) ( -- a | false )
  true case
    is-the-cave-entrance-accessible? of  cave-entrance~  endof

    \ XXX TODO -- quizá no se implemente esto porque precisaría
    \ asociar a cave-entrance~ el vocablo «salida/s», lo que crearía
    \ una ambigüedad adicional que resolver:

    \ location-10~ am-i-there? of  cave-entrance~  endof

    false swap
  endcase  ;
  \ Devuelve el ente adecuado a la palabra «entrada»
  \ (o _false_ si la ambigüedad no puede ser resuelta).

: (exits)  ( -- a )
  is-the-cave-entrance-accessible?
  if  cave-entrance~  else  exits~  then  ;
  \ Devuelve el ente adecuado a la palabra «salida/s».

: (stone) ( -- a )
  true case
    stone~ is-accessible? of  stone~  endof
    emerald~ is-accessible? of  emerald~  endof
    location-08~ am-i-there? of  ravine-wall~  endof
    rocks~ swap
  endcase  ;
  \ Devuelve el ente adecuado a la palabra «piedra».  Puede referise,
  \ en orden preferente, a la piedra, a la esmeralda, a la pared de
  \ roca del desfiladero o a las rocas.

: (wall) ( -- a )
  location-08~ am-i-there?
  if  ravine-wall~  else  wall~  then  ;
  \ Devuelve el ente adecuado a la palabra «pared».
  \ XXX TODO -- probablemente habrá que añadir más casos

: (somebody) ( -- a | false )
  true case
    pass-still-open? battle? or of  soldiers~  endof
    location-28~ am-i-there? location-29~ am-i-there? or of  refugees~  endof
    ambrosio~ is-here? of  ambrosio~  endof
    false swap
  endcase  ;
  \ Devuelve el ente adecuado a la palabra «alguien».  (o _false_ si
  \ la ambigüedad no puede ser resuelta).  Puede referirse a los
  \ soldados, a los refugiados o a ambrosio.

: (bridge)  ( -- a )
  true case
    location-13~ am-i-there? of  bridge~  endof
    location-18~ am-i-there? of  arch~  endof
    bridge~ is-known? of bridge~  endof
    arch~ is-known? of arch~  endof
    false swap
  endcase  ;
  \ Devuelve el ente adecuado a la palabra «puente».

\ ==============================================================
\ Vocabulario

\ El vocabulario del juego está implementado como una lista de
\ palabras de Forth, creado con el nombre de `player-wordlist`.

player-wordlist dup >order set-current

\ ----------------------------------------------
\ Pronombres

\ De momento no se implementan las formas sin tilde porque obligarían
\ a distinguir sus usos como adjetivos o sustantivos.

\ XXX TODO
\ esto/s eso/s y aquello/s podrían implementarse como sinónimos
\ de las formas masculinas o bien referirse al último y penúltimo
\ complemento usado, de cualquier género, pero para ello la estructura
\ de la tabla `last-complement` debería ser modificada.
\ : esto  last-complement @ set-complement  ;
\ : aquello  last-but-one-complement @ set-complement  ;

: éste  last-complement >masculine >singular @ set-complement  ;
' éste aliases: ése  ;aliases

: ésta  last-complement >feminine >singular @ set-complement  ;
' ésta aliases: ésa  ;aliases

: éstos  last-complement >masculine >plural @ set-complement  ;
' éstos aliases: ésos  ;aliases

: éstas  last-complement >feminine >plural @ set-complement  ;
' éstas aliases: ésas  ;aliases

: aquél  last-but-one-complement >masculine >singular @ set-complement  ;

: aquélla  last-but-one-complement >feminine >singular @ set-complement  ;

: aquéllos  last-but-one-complement >masculine >plural @ set-complement  ;

: aquéllas  last-but-one-complement >feminine >plural @ set-complement  ;

\ ----------------------------------------------
\ Verbos

: ir ['] do-go set-action  ;
' ir aliases:
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
  ;aliases

: abrir  ['] do-open set-action  ;
' abrir aliases:  abre abrid abro abra  ;aliases
: abrirlo  abrir éste  ;
' abrirlo aliases: ábrelo abridlo ábrolo ábralo  ;aliases
: abrirla  abrir ésta  ;
' abrirla aliases: ábrela abridla ábrola ábrala  ;aliases
: abrirlos  abrir éstos  ;
' abrirlos aliases: ábrelos abridlos ábrolos ábralos  ;aliases
: abrirlas  abrir éstas  ;
' abrirlas aliases: ábrelas abridlas ábrolas ábralas  ;aliases

: cerrar  ['] do-close set-action  ;
' cerrar aliases:  cierra cerrad cierro  ;aliases
: cerrarlo  cerrar éste  ;
' cerrarlo aliases:  ciérralo cerradlo ciérrolo ciérrelo  ;aliases
: cerrarla  cerrar ésta  ;
' cerrarla aliases:  ciérrala cerradla ciérrola ciérrela  ;aliases
: cerrarlos  cerrar éstos  ;
' cerrarlos aliases:  ciérralos cerradlos ciérrolos ciérrelos  ;aliases
: cerrarlas  cerrar éstas  ;
' cerrarlas aliases:  ciérralas cerradlas ciérrolas ciérrelas  ;aliases

: coger  ['] do-take set-action  ;
' coger aliases:
  coge coged cojo coja
  agarrar agarra agarrad agarro agarre
  recoger recoge recoged recojo recoja
  ;aliases
: cogerlo  coger éste  ;
' cogerlo aliases:
  cógelo cogedlo cójolo cójalo
  agarrarlo agárralo agarradlo agárrolo agárrelo
  recogerlo recógelo recogedlo recójolo recójalo
  ;aliases
: cogerla  coger éste  ;
' cogerla aliases:
  cógela cogedla cójola cójala
  agarrarla agárrala agarradla agárrola agárrela
  recogerla recógela recogedla recójola recójala
  ;aliases
: cogerlos  coger éstos  ;
' cogerlos aliases:
  cógelos cogedlos cójolos cójalos
  agarrarlos agárralos agarradlos agárrolos agárrelos
  recogerlos recógelos recogedlos recójolos recójalos
  ;aliases
: cogerlas  coger éstas  ;
' cogerlas aliases:
  cógelas cogedlas cójolas cójalas
  agarrarlas agárralas agarradlas agárrolas agárrelas
  recogerlas recógelas recogedlas recójolas recójalas
  ;aliases

: tomar  ['] do-take|do-eat set-action  ;
  \ XXX TODO -- inconcluso
' tomar  aliases:
  toma tomad tomo tome
  ;aliases
: tomarlo  tomar éste  ;
' tomarlo aliases: tómalo tomadlo tómolo tómelo  ;aliases

: dejar  ['] do-drop set-action  ;
' dejar aliases:
  deja dejad dejo deje
  soltar suelta soltad suelto suelte
  tirar tira tirad tiro tire
  ;aliases
: dejarlo  dejar éste  ;
' dejarlo aliases:
  déjalo dejadlo déjolo déjelo
  soltarlo suéltalo soltadlo suéltolo suéltelo
  tirarlo tíralo tiradlo tírolo tírelo
  ;aliases
: dejarlos  dejar éstos  ;
' dejarlos aliases:
  déjalos dejadlos déjolos déjelos
  soltarlos suéltalos soltadlos suéltolos suéltelos
  tirarlos tíralos tiradlos tírolos tírelos
  ;aliases
: dejarla  dejar ésta  ;
' dejarla aliases:
  déjala dejadla déjola déjela
  soltarla suéltala soltadla suéltola suéltela
  tirarla tírala tiradla tírola tírela
  ;aliases
: dejarlas  dejar éstas  ;
' dejarlas aliases:
  déjalas dejadlas déjolas déjelas
  soltarlas suéltalas soltadlas suéltolas suéltelas
  tirarlas tíralas tiradlas tírolas tírelas
  ;aliases

: mirar  ['] do-look set-action  ;
' mirar aliases:
  m mira mirad miro mire
  contemplar contempla contemplad contemplo contemple
  observar observa observad observo observe
  ;aliases
: mirarlo  mirar éste  ;
' mirarlo aliases:
  míralo miradlo mírolo mírelo
  contemplarlo contémplalo contempladlo contémplolo contémplelo
  observarlo obsérvalo observadlo obsérvolo obsérvelo
  ;aliases
: mirarla  mirar ésta  ;
' mirarla aliases:
  mírala miradla mírola mírela
  contemplarla contémplala contempladla contémplola contémplela
  observarla obsérvala observadla obsérvola obsérvela
  ;aliases
: mirarlos  mirar éstos  ;
' mirarlos aliases:
  míralos miradlos mírolos mírelos
  contemplarlos contémplalos contempladlos contémplolos contémplelos
  observarlos obsérvalos observadlos obsérvolos obsérvelos
  ;aliases
: mirarlas  mirar éstas  ;
' mirarlas aliases:
  míralas miradlas mírolas mírelas
  contemplarlas contémplalas contempladlas contémplolas contémplelas
  observarlas obsérvalas observadlas obsérvolas obsérvelas
  ;aliases

: mirarse  ['] do-look-yourself set-action  ;
' mirarse aliases:
  mírese miraos
  mirarte mírate mírote mírete
  mirarme mírame miradme mírome míreme
  contemplarse contemplaos contémplese
  contemplarte contémplate contémplote contémplete
  contemplarme contémplame contempladme contémplome contémpleme
  observarse obsérvese observaos
  observarte obsérvate obsérvote obsérvete
  observarme obsérvame observadme obsérvome obsérveme
  ;aliases

: otear  ['] do-look-to-direction set-action  ;
' otear aliases: oteo otea otead otee  ;aliases

: x  ['] do-exits set-action  ;
: salida  ['] do-exits (exits) set-action-or-complement  ;
' salida aliases:  salidas  ;aliases

: examinar  ['] do-examine set-action  ;
' examinar aliases: ex examina examinad examino examine  ;aliases
: examinarlo  examinar éste  ;
' examinarlo aliases: examínalo examinadlo examínolo examínelo  ;aliases
: examinarlos  examinar éstos  ;
' examinarlos aliases: examínalos examinadlos examínolos examínelos  ;aliases
: examinarla  examinar ésta  ;
' examinarla aliases: examínala examinadla examínola examínela  ;aliases
: examinarlas  examinar éstas  ;
' examinarlas aliases: examínalas examinadlas examínolas examínelas  ;aliases

: examinarse  ['] do-examine set-action protagonist~ set-complement  ;
' examinarse aliases:
  examínese examinaos
  examinarte examínate examínete
  examinarme examíname examinadme examínome examíneme
  ;aliases

: registrar  ['] do-search set-action  ;
' registrar aliases:  registra registrad registro registre  ;aliases
: registrarlo  registrar éste  ;
' registrarlo aliases: regístralo registradlo regístrolo regístrelo  ;aliases
: registrarla  registrar ésta  ;
' registrarla aliases: regístrala registradla regístrola regístrela  ;aliases
: registrarlos  registrar éstos  ;
' registrarlos aliases: regístralos registradlos regístrolos regístrelos  ;aliases
: registrarlas  registrar éstas  ;
' registrarlas aliases: regístralas registradlas regístrolas regístrelas  ;aliases

: i  ['] do-inventory inventory~ set-action-or-complement  ;
' i aliases:  inventario  ;aliases

: inventariar  ['] do-inventory set-action  ;
' inventariar aliases:
  inventaría inventariad inventarío inventaríe
  registrarse regístrase regístrese
  registrarme regístrame registradme regístrome regístreme
  registrarte regístrate regístrote regístrete
  ;aliases

: hacer  ['] do-do set-action  ;
' hacer aliases:  haz haced hago haga  ;aliases
: hacerlo  hacer éste  ;
' hacerlo aliases:  hazlo hacedlo hágolo hágalo  ;aliases
: hacerla  hacer ésta  ;
' hacerla aliases:  hazla hacedla hágola hágala  ;aliases
: hacerlos  hacer éstos  ;
' hacerlos aliases:  hazlos hacedlos hágolos hágalos  ;aliases
: hacerlas  hacer éstas  ;
' hacerlas aliases:  hazlas hacedlas hágolas hágalas  ;aliases

: fabricar  ['] do-make set-action  ;
' fabricar aliases:
  fabrica fabricad fabrico fabrique
  construir construid construye construyo construya
  ;aliases
: fabricarlo  fabricar éste  ;
' fabricarlo aliases:
  fabrícalo fabricadlo fabrícolo fabríquelo
  construirlo constrúyelo construidlo constrúyolo constrúyalo
  ;aliases
: fabricarla  fabricar éste  ;
' fabricarla aliases:
  fabrícala fabricadla fabrícola fabríquela
  construirla constrúyela construidla constrúyola constrúyala
  ;aliases
: fabricarlos  fabricar éste  ;
' fabricarlos aliases:
  fabrícalos fabricadlos fabrícolos fabríquelos
  construirlos constrúyelos construidlos constrúyolos constrúyalos
  ;aliases
: fabricarlas  fabricar éste  ;
' fabricarlas aliases:
  fabrícalas fabricadlas fabrícolas fabríquelas
  construirlas constrúyelas construidlas constrúyolas constrúyalas
  ;aliases

: nadar  ['] do-swim set-action  ;
' nadar aliases:
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
  ;aliases

: quitarse  ['] do-take-off set-action  ;
' quitarse aliases:
  quítase quitaos quítese
  quitarte quítate quítote quítete
  quitarme quítame quítome quíteme
  ;aliases
: quitárselo  quitarse éste  ;
' quitárselo aliases:
  quítatelo
  quitártelo quitáoslo quíteselo
  quitármelo quítamelo quítomelo quítemelo
  ;aliases
: quitársela  quitarse ésta  ;
' quitársela aliases:
  quítatela
  quitártela quitáosla quítesela
  quitármela quítamela quítomela quítemela
  ;aliases
: quitárselos  quitarse éstos  ;
' quitárselos aliases:
  quítatelos
  quitártelos quitáoslos quíteselos
  quitármelos quítamelos quítomelos quítemelos
  ;aliases
: quitárselas  quitarse éstas  ;
' quitárselas aliases:
  quítatelas
  quitártelas quitáoslas quíteselas
  quitármelas quítamelas quítomelas quítemelas
  ;aliases

: ponerse  ['] do-put-on set-action  ;
' ponerse aliases:
  póngase poneos
  ponerme ponme póngome póngame
  ponerte ponte póngote póngate
  colocarse colocaos colóquese
  colocarte colócate colóquete
  colocarme colócame colócome colóqueme
  ;aliases
  \ XXX TODO -- crear acción. vestir [con], parte como sinónimo y
  \ parte independiente.

: ponérselo  ponerse éste  ;
' ponérselo aliases:
  póngaselo ponéoslo
  ponérmelo pónmelo póngomelo póngamelo
  ponértelo póntelo póngotelo póngatelo
  colocórselo colocáoslo colóqueselo
  colocártelo colócatelo colóquetelo
  colocármelo colócamelo colócomelo colóquemelo
  ;aliases
: ponérsela  ponerse ésta  ;
' ponérsela aliases:
  póngasela ponéosla
  ponérmela pónmela póngomela póngamela
  ponértela póntela póngotela póngatela
  colocórsela colocáosla colóquesela
  colocártela colócatela colóquetela
  colocármela colócamela colócomela colóquemela
  ;aliases
: ponérselos  ponerse éstos  ;
' ponérselos aliases:
  póngaselos ponéoslos
  ponérmelos pónmelos póngomelos póngamelos
  ponértelos póntelos póngotelos póngatelos
  colocórselos colocáoslos colóqueselos
  colocártelos colócatelos colóquetelos
  colocármelos colócamelos colócomelos colóquemelos
  ;aliases
: ponérselas  ponerse éstas  ;
' ponérselas aliases:
  póngaselas ponéoslas
  ponérmelas pónmelas póngomelas póngamelas
  ponértelas póntelas póngotelas póngatelas
  colocórselas colocáoslas colóqueselas
  colocártelas colócatelas colóquetelas
  colocármelas colócamelas colócomelas colóquemelas
  ;aliases

: matar  ['] do-kill set-action  ;
' matar aliases:
  mata matad mato mate
  asesinar asesina asesinad asesino asesine
  aniquilar aniquila aniquilad aniquilo aniquile
  ;aliases
: matarlo  matar éste  ;
' matarlo aliases:
  mátalo matadlo mátolo mátelo
  asesinarlo asesínalo asesinadlo asesínolo asesínelo
  aniquilarlo aniquínalo aniquinadlo aniquínolo aniquínelo
  ;aliases
: matarla  matar ésta  ;
' matarla aliases:
  mátala matadla mátola mátela
  asesinarla asesínala asesinadla asesínola asesínela
  aniquilarla aniquínala aniquinadla aniquínola aniquínela
  ;aliases
: matarlos  matar éstos  ;
' matarlos aliases:
  mátalos matadlos mátolos mátelos
  asesinarlos asesínalos asesinadlos asesínolos asesínelos
  aniquilarlos aniquínalos aniquinadlos aniquínolos aniquínelos
  ;aliases
: matarlas  matar éstas  ;
' matarlas aliases:
  mátalas matadlas mátolas mátelas
  asesinarlas asesínalas asesinadlas asesínolas asesínelas
  aniquilarlas aniquínalas aniquinadlas aniquínolas aniquínelas
  ;aliases

: golpear  ['] do-hit set-action  ;
' golpear aliases:
  golpea golpead golpeo golpee
  sacudir sacude sacudid sacudo sacuda
  ;aliases
: golpearla  golpear ésta  ;
' golpearla aliases:
  golpéala golpeadla golpéola golpéela
  sacudirla sacúdela sacudidla sacúdola sacúdala
  ;aliases
: golpearlos  golpear éstos  ;
' golpearlos aliases:
  golpéalos golpeadlos golpéolos golpéelos
  sacudirlos sacúdelos sacudidlos sacúdolos sacúdalos
  ;aliases
: golpearlas  golpear éstas  ;
' golpearlas aliases:
  golpéalas golpeadlas golpéolas golpéelas
  sacudirlas sacúdelas sacudidlas sacúdolas sacúdalas
  ;aliases

: atacar  ['] do-attack set-action  ;
' atacar aliases:
  ataca atacad ataco ataque
  agredir agrede agredid agredo agreda
  ;aliases
: atacarlo  atacar éste  ;
' atacarlo aliases:
  atácalo atacadlo atácolo atáquelo
  agredirlo agrédelo agredidlo agrédolo agrédalo
  ;aliases
: atacarla  atacar ésta  ;
' atacarla aliases:
  atácala atacadla atácola atáquela
  agredirla agrédela agredidla agrédola agrédala
  ;aliases
: atacarlos  atacar éstos  ;
' atacarlos aliases:
  atácalos atacadlos atácolos atáquelos
  agredirlos agrédelos agredidlos agrédolos agrédalos
  ;aliases
: atacarlas  atacar éstas  ;
' atacarlas aliases:
  atácalas atacadlas atácolas atáquelas
  agredirlas agrédelas agredidlas agrédolas agrédalas
  ;aliases

: romper  ['] do-break set-action  ;
' romper aliases:
  rompe romped rompo rompa
  despedazar despedaza despedazad despedazo despedace
  destrozar destroza destrozad destrozo destroce
  dividir divide dividid divido divida
  cortar corta cortad corto corte
  ;aliases
: romperlo  romper éste  ;
' romperlo aliases:
  rómpelo rompedlo rómpolo rómpalo
  despedazarlo despedazalo despedazadlo despedázolo despedácelo
  destrozarlo destrózalo destrozadlo destrózolo destrócelo
  dividirlo divídelo divididlo divídolo divídalo
  cortarlo cortalo cortadlo córtolo córtelo
  ;aliases
: romperla  romper ésta  ;
' romperla aliases:
  rómpela rompedla rómpola rómpala
  despedazarla despedazala despedazadla despedázola despedácela
  destrozarla destrózala destrozadla destrózola destrócela
  dividirla divídela divididla divídola divídala
  cortarla córtala cortadla córtola córtela
  ;aliases
: romperlos  romper éstos  ;
' romperlos aliases:
  rómpelos rompedlos rómpolos rómpalos
  despedazarlos despedazalos despedazadlos despedázolos despedácelos
  destrozarlos destrózalos destrozadlos destrózolos destrócelos
  dividirlos divídelos divididlos divídolos divídalos
  cortarlos córtalos cortadlos córtolos córtelos
  ;aliases
: romperlas  romper éstas  ;
' romperlas aliases:
  rómpelas rompedlas rómpolas rómpalas
  despedazarlas despedazalas despedazadlas despedázolas despedácelas
  destrozarlas destrózalas destrozadlas destrózolas destrócelas
  dividirlas divídelas divididlas divídolas divídalas
  cortarlas córtalas cortadlas córtolas córtelas
  ;aliases

\ quebrar \ XXX TODO
\ desgarrar \ XXX TODO

: asustar  ['] do-frighten set-action  ;
' asustar aliases:
  asusto asusta asustad asuste
  amedrentar amedrento amedrenta amedrentad amedrente
  acojonar acojono acojona acojonad acojone
  atemorizar atemoriza atemorizad atemorizo atemorice
  espanto espanta espantad espante
  aterrorizo aterroriza aterrorizad aterrorice
  ahuyento ahuyenta ahuyentad ahuyente
  ;aliases
: asustarlo  asustar éste  ;
' asustarlo aliases:
  asústolo asústalo asustadlo asústelo
  amedrentarlo amedréntolo amedréntalo amedrentadlo amedréntelo
  acojonarlo acojónolo acojónalo acojonadlo acojónelo
  atemorizarlo atemorízalo atemorizadlo atemorízolo atemorícelo
  espántolo espántalo espantadlo espántelo
  aterrorízolo aterrorízalo aterrorizadlo aterrorícelo
  ahuyéntolo ahuyéntalo ahuyentadlo ahuyéntelo
  ;aliases
: asustarla  asustar ésta  ;
' asustarla aliases:
  asústola asústala asustadla asústela
  amedrentarla amedréntola amedréntala amedrentadla amedréntela
  acojonarla acojónola acojónala acojonadla acojónela
  atemorizarla atemorízala atemorizadla atemorízola atemorícela
  espántola espántala espantadla espántela
  aterrorízola aterrorízala aterrorizadla aterrorícela
  ahuyéntola ahuyéntala ahuyentadla ahuyéntela
  ;aliases
: asustarlos  asustar éstos  ;
' asustarlos aliases:
  asústolos asústalos asustadlos asústelos
  amedrentarlos amedréntolos amedréntalos amedrentadlos amedréntelos
  acojonarlos acojónolos acojónalos acojonadlos acojónelos
  atemorizarlos atemorízalos atemorizadlos atemorízolos atemorícelos
  espántolos espántalos espantadlos espántelos
  aterrorízolos aterrorízalos aterrorizadlos aterrorícelos
  ahuyéntolos ahuyéntalos ahuyentadlos ahuyéntelos
  ;aliases
: asustarlas  asustar éstas  ;
' asustarlas aliases:
  asústolas asústalas asustadlas asústelas
  amedrentarlas amedréntolas amedréntalas amedrentadlas amedréntelas
  acojonarlas acojónolas acojónalas acojonadlas acojónelas
  atemorizarlas atemorízalas atemorizadlas atemorízolas atemorícelas
  espántolas espántalas espantadlas espántelas
  aterrorízolas aterrorízalas aterrorizadlas aterrorícelas
  ahuyéntolas ahuyéntalas ahuyentadlas ahuyéntelas
  ;aliases

: afilar  ['] do-sharpen set-action  ;
' afilar aliases:  afila afilad afilo afile  ;aliases
: afilarlo  afilar éste  ;
' afilarlo aliases:  afílalo afiladlo afílolo afílelo  ;aliases
: afilarla  afilar ésta  ;
' afilarla aliases:  afílala afiladla afílola afílela  ;aliases
: afilarlos  afilar éstos  ;
' afilarlos aliases:  afílalos afiladlos afílolos afílelos  ;aliases
: afilarlas  afilar éstas  ;
' afilarlas aliases:  afílalas afiladlas afílolas afílelas  ;aliases

: partir  ['] do-go|do-break set-action  ;
' partir aliases:  parto partid parta  ;aliases
\ «parte» está en la sección final de ambigüedades
: partirlo  partir éste  ;
' partirlo aliases:  pártelo pártolo partidlo pártalo  ;aliases
: partirla  partir ésta  ;
' partirla aliases:  pártela pártola partidla pártala  ;aliases
: partirlos  partir éstos  ;
' partirlos aliases:  pártelos pártolos partidlos pártalos  ;aliases
: partirlas  partir éstas  ;
' partirlas aliases:  pártelas pártolas partidlas pártalas  ;aliases

: esperar  ;
  \ XXX TODO

' esperar aliases:
  z espera esperad espero espere
  aguardar aguarda aguardad aguardo aguarde
  ;aliases
: esperarlo  esperar éste  ;
' esperarlo aliases:
  esperadlo espérolo espérelo
  aguardarlo aguárdalo aguardadlo aguárdolo aguárdelo
  ;aliases
: esperarla  esperar ésta  ;
' esperarla aliases:
  esperadla espérola espérela
  aguardarla aguárdala aguardadla aguárdola aguárdela
  ;aliases
: esperarlos  esperar éstos  ;
' esperarlos aliases:
  esperadlos espérolos espérelos
  aguardarlos aguárdalos aguardadlos aguárdolos aguárdelos
  ;aliases
: esperarlas  esperar éstas  ;
' esperarlas aliases:
  esperadlas espérolas espérelas
  aguardarlas aguárdalas aguardadlas aguárdolas aguárdelas
  ;aliases

: escalar  ['] do-climb set-action  ;
' escalar aliases:  escala escalo escale  ;aliases
' escalar aliases:  trepar trepa trepo trepe  ;aliases

: hablar  ['] do-speak set-action  ;
  \ XXX TODO -- Crear nuevas palabras según la preposición que necesiten.
  \ XXX TODO -- Separar matices.

' hablar aliases:
  habla hablad hablo hable
  hablarle háblale háblole háblele
  conversar conversa conversad converso converse
  charlar charla charlad charlo charle
  decir di decid digo diga
  decirle dile decidle dígole dígale
  platicar platica platicad platico platique
  platicarle platícale platicadle platícole platíquele
  ;aliases
  \ contar cuenta cuento cuente  \ XXX
  \ contarle cuéntale cuéntole cuéntele  \ XXX

: presentarse  ['] do-introduce-yourself set-action  ;
' presentarse aliases:
  preséntase preséntese
  presentarte preséntate presentaos preséntete
  ;aliases

  \ XXX TODO:
  \ meter introducir insertar colar encerrar

\ ----------------------------------------------
\ Nombres de objetos o personas

: ulfius  ulfius~ set-complement  ;

: ambrosio  (ambrosio) set-complement  ;

: hombre  (man) set-complement  ;
' hombre aliases:  señor tipo individuo persona  ;aliases

: hombres  (men) set-complement  ;
' hombres aliases: gente personas  ;aliases
\ XXX Ambigüedad.:
\ «jefe» podría ser también el jefe de los enemigos durante la batalla:

: jefe  leader~ set-complement  ;
' jefe aliases:
  líder viejo anciano abuelo
  ;aliases

: soldados  soldiers~ set-complement  ;
' soldados aliases:
  guerreros luchadores combatientes camaradas
  compañeros oficiales suboficiales militares
  guerrero luchador combatiente camarada
  compañero oficial suboficial militar
  ;aliases

: multitud  refugees~ set-complement  ;
' multitud aliases:
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
  muchedumbre masa enjambre
  ;aliases

: refugiados leader~ conversations? ?? multitud ;
' refugiados aliases: refugiada refugiadas  ;aliases

: refugiado leader~ conversations? ?? viejo ;


: altar  altar~ set-complement  ;

: arco  arch~ set-complement  ;

: capa  cloak~ set-complement  ;
  \ XXX TODO -- hijuelo?
' capa aliases:  lana  ;aliases

\ ' capa aliases:  abrigo  ;aliases
  \ XXX TODO -- diferente género

: coraza  cuirasse~ set-complement  ;
' coraza aliases:  armadura  ;aliases

: puerta  door~ set-complement  ;

: esmeralda  emerald~ set-complement  ;
' esmeralda aliases:  joya  ;aliases
  \ XXX TODO -- piedra-preciosa brillante

: derrumbe fallen-away~ set-complement  ;

: banderas  flags~ set-complement  ;
' banderas aliases:
    bandera pendones enseñas pendón enseña
    mástil mástiles
    estandarte estandartes
  ;aliases
  \ XXX TODO -- estandarte, enseña... otro género

: dragones  flags~ is-known? ?? banderas ;
' dragones aliases: dragón  ;aliases

: pedernal  flint~ set-complement  ;

: ídolo  idol~ set-complement  ;
' ídolo aliases:  ojo orificio agujero  ;aliases
  \ XXX TODO -- separar los sinónimos de ídolo

: llave  key~ set-complement  ;
' llave aliases:  hierro herrumbe óxido  ;aliases

: lago  lake~ set-complement  ;
' lago aliases:  laguna agua estanque  ;aliases
  \ XXX TODO -- diferente género

: candado  lock~ set-complement  ;
' candado aliases:  cerrojo  ;aliases

: tronco  log~ set-complement  ;
' tronco aliases:  leño madero  ;aliases
  \ XXX TODO -- madera

: trozo  piece~ set-complement  ;
' trozo aliases:  pedazo retal tela  ;aliases

: harapo  rags~ set-complement  ;

: rocas  ( -- )
  location-09~ am-i-there?
  if  fallen-away~  else  rocks~  then  set-complement  ;
' rocas aliases:  piedras pedruscos  ;aliases

: piedra  (stone) set-complement  ;
' piedra aliases:  roca pedrusco  ;aliases

: serpiente  snake~ set-complement  ;
' serpiente aliases:  reptil ofidio culebra animal bicho  ;aliases

: espada  sword~ set-complement  ;
' espada aliases:  tizona arma  ;aliases
\ XXX Nota.: "arma" es femenina pero usa artículo "él", contemplar en los cálculos de artículo.

: hilo  thread~ set-complement  ;
' hilo aliases:  hebra  ;aliases

: antorcha  torch~ set-complement  ;

: cascada  waterfall~ set-complement  ;
' cascada aliases:  catarata  ;aliases

: catre  s" catre" bed~ ms-name! bed~ set-complement  ;
' catre aliases:  camastro  ;aliases

: cama s" cama" bed~ fs-name! bed~ set-complement  ;

: velas  candles~ set-complement  ;
' velas aliases:  vela  ;aliases

: mesa  table~ set-complement  ;
' mesa aliases:  mesita pupitre  ;aliases

: puente  (bridge) set-complement  ;

: alguien  (somebody) set-complement  ;

: hierba  s" hierba" grass~ fs-name! grass~ set-complement  ;

: hierbas  s" hierbas" grass~ fp-name! grass~ set-complement  ;

: hierbajo  s" hierbajo" grass~ ms-name! grass~ set-complement  ;

: hierbajos  s" hierbajos" grass~ mp-name! grass~ set-complement  ;

: hiedra  s" hiedra" grass~ fs-name! grass~ set-complement  ;

: hiedras  s" hiedras" grass~ fp-name! grass~ set-complement  ;

\ ----------------------------------------------
\ Direcciones y sus acciones asociadas

: n  ['] do-go-north north~ set-action-or-complement  ;
' n aliases:  norte septentrión  ;aliases

: s  ['] do-go-south south~ set-action-or-complement  ;
' s aliases:  sur meridión  ;aliases

: e  ['] do-go-east east~ set-action-or-complement  ;
' e aliases:  este oriente levante  ;aliases

: o  ['] do-go-west west~ set-action-or-complement  ;
' o aliases:  oeste occidente poniente  ;aliases

: ar  ['] do-go-up up~ set-action-or-complement  ;
' ar aliases:  arriba  ;aliases

: subir  ['] do-go-up set-action  ;
' subir aliases:  sube subid subo suba  ;aliases
' subir aliases:  ascender asciende ascended asciendo ascienda  ;aliases
' subir aliases:  subirse subíos súbese súbase  ;aliases
' subir aliases:  subirte súbete súbote súbate  ;aliases

: ab  ['] do-go-down down~ set-action-or-complement  ;
' ab aliases:  abajo  ;aliases

: bajar  ['] do-go-down set-action  ;
' bajar aliases:  baja bajad bajo baje  ;aliases
' bajar aliases:  bajarse bajaos bájase bájese  ;aliases
' bajar aliases:  bajarte bájate bájote bájete  ;aliases
' bajar aliases:  descender desciende descended desciendo descienda  ;aliases

: salir  ['] do-go-out set-action  ;
' salir aliases:  sal salid salgo salga  ;aliases
  \ XXX TODO -- ambigüedad. sal
' salir aliases:  salirse  ;aliases
' salir aliases:  salirme sálgome  ;aliases
' salir aliases:  salirte  ;aliases
  \ XXX TODO -- ambigüedad. salte

: fuera  ['] do-go-out out~ set-action-or-complement  ;
' fuera aliases:  afuera  ;aliases

: exterior  out~ set-complement  ;

: entrar ['] do-go-in set-action  ;
' entrar aliases:  entra entrad entro entre  ;aliases
' entrar aliases:  entrarse entraos éntrese éntrase  ;aliases
' entrar aliases:  entrarte éntrete éntrate  ;aliases

: dentro  ['] do-go-in in~ set-action-or-complement  ;
' dentro aliases:  adentro  ;aliases

: interior  in~ set-complement  ;

\ ----------------------------------------------
\ Términos asociados a entes globales o virtuales

: nubes  clouds~ set-complement  ;
  \ XXX TODO ¿cúmulo-nimbos?, ¿nimbos?
' nubes aliases:  nube estratocúmulo estratocúmulos cirro cirros  ;aliases

: suelo  floor~ set-complement  ;
' suelo aliases:  suelos tierra firme  ;aliases
  \ XXX TODO -- Añadir «piso», que es ambiguo

: cielo  sky~ set-complement  ;
' cielo aliases:  cielos firmamento  ;aliases

: techo  ceiling~ set-complement  ;

: cueva  (cave) set-complement  ;
' cueva aliases:  caverna gruta  ;aliases

: entrada  (entrance) set-complement  ;
' entrada aliases:  acceso  ;aliases
  \ XXX TODO ¿Implementar cambio de nombre y/o género gramatical?
\ (entrada, acceso).

: enemigo  enemy~ set-complement  ;
' enemigo aliases: enemigos sajón sajones  ;aliases

: todo ;
  \ XXX TODO

: pared  (wall) set-complement  ;
' pared  aliases: muro  ;aliases
  \ XXX TODO ¿Implementar cambio de nombre y/o género gramatical?
  \ (pared/es, muro/s).

: paredes  wall~ set-complement  ;
' paredes  aliases: muros  ;aliases

\ ----------------------------------------------
\ Artículos

\ Los artículos no hacen nada pero es necesario crearlos
\ para que no provoquen un error cuando el intérprete de
\ comandos funcione en el modo opcional de no ignorar las
\ palabras desconocidas.

: la  ;
' la aliases: las el los una un unas unos  ;aliases

\ ----------------------------------------------
\ Adjetivos demostrativos

\ Lo mismo hacemos con los adjetivos demostrativos
\ y pronombres demostrativos sin tilde; salvo «este», que siempre
\ será interpretado como punto cardinal.

: esta  ;
' esta aliases: estas estos  ;aliases

\ ----------------------------------------------
\ (Seudo)preposiciones

: con  ( -- )  «con»-preposition# set-preposition  ;
  \ Uso: Herramienta o compañía

: usando  ( -- )  «usando»-preposition# set-preposition  ;
  \ Uso: Herramienta

' usando aliases: utilizando empleando mediante  ;aliases

false [if]

  \ XXX OLD
  \ XXX TODO -- descartado, pendiente

: a  ( -- )  «a»-preposition# set-preposition  ;
  \ Uso: Destino de movimiento, objeto indirecto

' a aliases: al  ;aliases

: de  ( -- )  «de»-preposition# set-preposition  ;
  \ Uso: Origen de movimiento, propiedad

: hacia  ( -- )  «hacia»-preposition# set-preposition  ;
  \ Uso: Destino de movimiento, destino de lanzamiento

: contra  ( -- )  «contra»-preposition# set-preposition  ;
  \ Uso: Destino de lanzamiento

: para  ( -- )  «para»-preposition# set-preposition  ;
  \ Uso: Destino de movimiento, destino de lanzamiento

: por  ( -- )  «por»-preposition# set-preposition  ;
  \ Uso: Destino de movimiento

[then]

\ ----------------------------------------------
\ Términos ambiguos

: cierre  ( -- )  action if  candado  else  cerrar  then  ;

: parte  ( -- )  action if  trozo  else  partir  then  ;

\ ----------------------------------------------
\ Comandos del sistema

: #recolorea  ( -- )  ['] recolor set-action  ;
  \ Restaura los colores predeterminados.

: #configura  ( "name" | -- )
  parse-name temporary-config-file place
  ['] read-config set-action  ; immediate
  \ Carga el fichero de configuración _name_.  Si no se indica _name_,
  \ se cargará el fichero de configuración predeterminado.

: #reconfigura  ( "name" | -- )
  parse-name temporary-config-file place
  ['] get-config set-action  ; immediate
  \ Restaura la configuración predeterminada y después carga el
  \ fichero de configuración _name_.  Si no se indica _name_, se
  \ cargará el fichero de configuración predeterminado.

: #graba  ( "name" -- )
  [debug-parsing] [??] ~~
  parse-name >sb
  [debug-parsing] [??] ~~
  ['] save-the-game set-action
  [debug-parsing] [??] ~~
  ;  immediate
  \ Graba el estado de la partida en un fichero.

: #carga  ( "name" -- )
  [debug-parsing] [??] ~~
  parse-name
  [debug-parsing] [??] ~~
  >sb
  [debug-parsing] [??] ~~
  ['] load-the-game set-action
  [debug-parsing] [??] ~~
  ;  immediate
  \ Carga el estado de la partida de un fichero.

: #fin  ( -- )  ['] finish set-action  ;
  \ Abandonar la partida

\ : #ayuda  ( -- )
\   \ ['] do-help set-action
\   ;
  \ XXX TODO

: #forth  ( -- )
  restore-wordlists system-colors cr bootmessage cr quit  ;
  \ XXX TMP -- Para usar durante el desarrollo.

: #bye  ( -- )  bye  ;
  \ XXX TMP -- Para usar durante el desarrollo.

restore-wordlists

\ vim:filetype=gforth:fileencoding=utf-8
