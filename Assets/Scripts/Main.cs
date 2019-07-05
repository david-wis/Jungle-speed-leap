using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System;

public class Main : MonoBehaviour
{
    public const int CANTCARTAS = 80; //Flechas para adentro y afuera por ahora no van a ser cartas
    //public const int CANTCARTAS = 32;

    public const int CANTJUGADORES = 4;
    public RuntimeAnimatorController[] contrAnimacDelMazo = new RuntimeAnimatorController[4];
    public RuntimeAnimatorController[] contrAnimac0HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimac1HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimac2HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimac3HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[,] matrizContrAnimacHaciaMazos = new RuntimeAnimatorController[4, 3];

    Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    Jugador[] jugadores = new Jugador[CANTJUGADORES];
    public GameObject[] cartasEstaticas = new GameObject[4];
    GameObject[] mazos = new GameObject[4];

    public GameObject totem;
    public GameObject turnos;

    Mesa mesa = new Mesa();

    Vector3[] posicCartasDelMazo = { new Vector3(0.13f, 1.5f, -0.43f),
                                     new Vector3(0.2f, 1.5f, -0.09f),
                                     new Vector3(-0.235f, 1.5f, 0.15f),
                                     new Vector3(-0.38f, 1.5f, -0.25f)};
    Vector3[] rotacCartasDelMazo = { new Vector3(-90f, 0f, -175f),
                                     new Vector3(-90f, -90f, -175f),
                                     new Vector3(-90f, -90f, 95f),
                                     new Vector3(-90f, -90f, 5f)};

    List<GameObject> gameObjectsAnimandoseHaciaMazo = new List<GameObject>();
    const float fDuracAnimacionDesdeMazo = 1.0f, fDuracAnimacionHaciaMazo = 1.3f;
    float fTimerAnimacHaciaMazo = 0f;
    bool bSeEstaAnimandoHaciaMazo = false;
    float[] vecTimersAnimacDesdeMazo = { 0f, 0f, 0f, 0f };
    bool[] vecSeEstaAnimandoDesdeMazo = { false, false, false, false };    

    int iIndexJugActual;
    UnityAction eventoListenerMazo0, eventoListenerMazo1, eventoListenerMazo2, eventoListenerMazo3;
    UnityAction eventoListenerTotem;
    UnityAction eventoListenerRestablecerTotem;

    // Use this for initialization
    void Start()
    {
        /* Aca en algun momento vamos a meter un menu 
         * para hostear partidas y elegir cant de jugadores
         */
        totem.transform.position += new Vector3(0, 3, 0);
        LlenarMazo();
        ReferenciarCartasEstaticas();
        ReferenciarMazos();
        CrearJugadores();
        LlenarMatrizAnimaciones();

        //Se reparten las cartas entre los jugadores
        RepartirMazo();

        //Creacion de eventos 
        /*
         * OPTIMIZAR (si es posible :v) EN VERSION FUTURA
         */
        eventoListenerMazo0 = new UnityAction(delegate () { PonerCarta(0); });
        eventoListenerMazo1 = new UnityAction(delegate () { PonerCarta(1); });
        eventoListenerMazo2 = new UnityAction(delegate () { PonerCarta(2); });
        eventoListenerMazo3 = new UnityAction(delegate () { PonerCarta(3); });

        eventoListenerTotem = new UnityAction(AgarrarTotem);

        eventoListenerRestablecerTotem = new UnityAction(ReiniciarTotem);

        EventManager.StartListening("agarrarcarta0", eventoListenerMazo0); //Evento que se produce cuando un jugador toca un mazo
        EventManager.StartListening("agarrarcarta1", eventoListenerMazo1);
        EventManager.StartListening("agarrarcarta2", eventoListenerMazo2);
        EventManager.StartListening("agarrarcarta3", eventoListenerMazo3);

        EventManager.StartListening("agarrartotem", eventoListenerTotem); //Evento que se produce cuando un jugador agarra el totem

        EventManager.StartListening("restablecertotem", eventoListenerRestablecerTotem); //Evento de debug para restablecer la posicion del totem

        //En el futuro eligiremos el jugador inicial de manera aleatoria
        //iIndexJugActual = ObtenerRandom(4); 
        iIndexJugActual = 0;
        cambiarTurno(0);
    }
    float fTimer = 0.0f; //Bereishit
    bool bCartaEsperando = false;
    float fLastTime = -2.0f; //Ultima vez que se tocó el mazo
    bool bPause = false; //Permite pausar el mundo
    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;
        if (iIndexJugActual != 0 && !bPause)
        {
            if (!bCartaEsperando)
            {
                StartCoroutine(PonerCartaBot(iIndexJugActual));
                bCartaEsperando = true;
            }
        }
        verificarAnimacionesDesdeMazo(); //Si alguna carta se esta animando desde el mazo, cuenta para despues sacarle la animacion
        verificarAnimacionesHaciaMazo(); //Si alguna carta se esta animando hacia el mazo, cuenta para despues destruirlo
    }

    /// <summary>
    /// Llena el mazo con las cartas de Assets (tienen "carta" como tag)
    /// </summary>
    private void LlenarMazo()
    {
        string[] rutaCartitas = AssetDatabase.FindAssets("b:carta", new[] { "Assets/Cartas" });
        for (int i = 0; i < CANTCARTAS; i++)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(rutaCartitas[i]);
            arrayMazoTotal[i] = (Carta)AssetDatabase.LoadAssetAtPath(ruta, typeof(Carta));
        }
    }

    /// <summary>
    /// Carga de manera ordenada el vector de cartas estáticas y las hace invisibles 
    /// </summary>
    void ReferenciarCartasEstaticas()
    {
        cartasEstaticas = GameObject.FindGameObjectsWithTag("CartasEstaticas");
        for (int i = 0; i < 4; i++)
        {
            cartasEstaticas[i].transform.parent.gameObject.SetActive(false);
        }
        Array.Sort(cartasEstaticas, CompareObNames);
    }

    /// <summary>
    /// Carga de manera ordenada los mazos en el vector de mazos
    /// </summary>
    void ReferenciarMazos()
    {
        mazos = GameObject.FindGameObjectsWithTag("Mazo");
        Array.Sort(mazos, CompareObNames);
    }

    /// <summary>
    /// Carga de manera ordenada los jugadores en el vector de jugadores y referencia los gameobjects de sus manos
    /// </summary>
    public void CrearJugadores()
    {
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, CompareObNames); //Supuestamente ya estaban en orden pero x las dudas
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            jugadores[i] = new Jugador();
            jugadores[i].Manos = manos[i];
        }
    }

    /// <summary>
    /// Carga la matriz de animaciones de las cartas hacia los diferentes mazos
    /// </summary>
    public void LlenarMatrizAnimaciones()
    {
        RuntimeAnimatorController[] contrAnimac;
        for (int iJugador = 0; iJugador < 4; iJugador++)
        {
            contrAnimac = GetType().GetField("contrAnimac" + iJugador + "HaciaMazos").GetValue(this) as RuntimeAnimatorController[];
            for (int iAnimacion = 0; iAnimacion < 3; iAnimacion++)
            {
                matrizContrAnimacHaciaMazos[iJugador, iAnimacion] = contrAnimac[iAnimacion];                
            }
        }
    }

    /// <summary>
    /// Reparte todas las cartas entre la cantidad de jugadores
    /// </summary>
    /// <param name="iCantJugadores">La cantidad de jugadores en la partida</param>
    private void RepartirMazo(int iCantJugadores = 4)
    {
        Mezclar();
        for (int i = 0; i < arrayMazoTotal.Length; i++)
        {
            int iIndexJug = i % 4;
            jugadores[iIndexJug].AgregarCarta(arrayMazoTotal[i]);
        }
    }

    /// <summary>
    /// Mezcla el mazo de todas las cartas
    /// </summary>
    private void Mezclar()
    {
        for (int i = 0; i < arrayMazoTotal.Length; i++)
        {
            Carta cAux = arrayMazoTotal[i];
            int iRand = ObtenerRandom(arrayMazoTotal.Length);
            arrayMazoTotal[i] = arrayMazoTotal[iRand];
            arrayMazoTotal[iRand] = cAux;
        }
    }

    /// <summary>
    /// Pone el totem en su posicion inicial
    /// </summary>
    private void ReiniciarTotem()
    {
        TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
        totemBehaviour.ReiniciarPosicion();
    }

    /// <summary>
    /// Define el tiempo que tardan los bots en tocar el mazo
    /// </summary>
    const float fCoolDown = 1.0f; //1 segundo

    /// <summary>
    /// Llama a la funcion PonerCarta luego de "fCoolDown" segundos
    /// </summary>
    /// <param name="iIndex"></param>
    /// <returns></returns>
    IEnumerator PonerCartaBot(int iIndex)
    {
        yield return new WaitForSeconds(fCoolDown);
        PonerCarta(iIndex);
    }

    /// <summary>
    /// Si es su turno, hace la operacion de levantar la carta de un jugador
    /// </summary>
    /// <param name="iIndexMazo">Indice del jugador que intenta levantar su carta</param>
    /// <param name="bFlechaAfuera">Si se quieren levantar todas las cartas al mismo tiempo, no hay intervalo de tiempo</param>
    void PonerCarta(int iIndexMazo, bool bFlechaAfuera = false)
    {
        if (bFlechaAfuera == bPause)
        {
            if (iIndexJugActual == iIndexMazo)
            {
                if ((fTimer - fLastTime >= fCoolDown) || bFlechaAfuera)
                { //Asi evitamos que mantener la mano apretada cause que haga todo al instante
                    Carta cartaActual = jugadores[iIndexJugActual].ObtenerSiguienteCarta();
                    if (cartaActual != null)
                    {
                        ModoJuego modo = mesa.AgregarCarta(cartaActual); //Agregamos la carta al vector de cartas de la mesa
                        Crear_AnimarCarta(cartaActual); //Crea la carta y la anima
                        cartasEstaticas[iIndexJugActual].transform.parent.gameObject.SetActive(true);
                        Image imagen = cartasEstaticas[iIndexJugActual].GetComponent<Image>();
                        var color = imagen.color;
                        color.a = 1;
                        imagen.sprite = cartaActual.img2D;
                        imagen.color = color;
                        fLastTime = fTimer;
                        //Debug.Log("Jugador " + iIndexJugActual + " - " + cartaActual.img2D.name + " - " + imagen.name + " - " + mazos[iIndexJugActual].name);
                        verificarMazoVacioJugador(iIndexJugActual);
                        iIndexJugActual = (iIndexJugActual < 3) ? iIndexJugActual + 1 : 0;
                        bCartaEsperando = false;
                        if (modo == ModoJuego.Fuera)
                        {
                            bPause = true;
                            StartCoroutine(LevantarCartasModoFlechaFuera());
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Cambia el sprite del menu de turnos dependiendo del jugador actual y del modo
    /// </summary>
    /// <param name="iOpcion">Indice del jugador actual. 4 representa todos y 5 nadie </param>
    void cambiarTurno(int iOpcion)
    {
        TurnosBehaviour turnosBehaviour = turnos.GetComponent<TurnosBehaviour>();
        if ((bPause && iOpcion == 4) || (!bPause && iOpcion != 4))
        {
            turnosBehaviour.cambiarImagen(iOpcion);
        }
    }

    /// <summary>
    /// Si el mazo del jugador recibido ya no tiene cartas, lo desactiva. 
    /// Si tiene cartas, lo activa
    /// </summary>
    /// <param name="idJugador">El jugador para fijarse su mazo</param>
    public void verificarMazoVacioJugador(int idJugador)
    {
        if (jugadores[idJugador].ObtenerCantCartas() == 0)
        {
            mazos[idJugador].SetActive(false);
        }
        else
        {
            mazos[idJugador].SetActive(true);
        }
    }

    /// <summary>
    /// Levanta todas las cartas cuando sale una carta de flecha para afuera luego de 3 segundos
    /// </summary>
    IEnumerator LevantarCartasModoFlechaFuera()
    {
        cambiarTurno(4);
        yield return new WaitForSeconds(3f);
        mesa.NormalizarModo();
        Debug.Log("Flechas para afuera");
        int iIndexAux = iIndexJugActual;
        iIndexJugActual = 0;
        for (int i = 0; i < 4; i++)
        {
            PonerCarta(i, true);
        }
        iIndexJugActual = iIndexAux;
        bPause = false; //El flujo del tiempo retoma su curso :v
    }

    int iJugadorTotem = 0; //Jugador que agarro el totem

    UnityAction eventoListenerTotemTraido;
    /// <summary>
    ///Funcion que se ejecuta cuando se produce el evento de agarrartotem.
    ///<para>Si el modo de juego de la mesa es dentro, todos pueden agarrar el totem. </para>
    ///<para>Si el modo de juego es color o normal, se verifican las coincidencias. Si se encuentran coincidencias se empieza a escuchar el evento de totem traido,
    ///sino se reinicia el totem y se le meten todas las cartas al perdedor.</para>
    ///<para>El modo de juego nunca va a ser flechas para fuera porque ese estado solo dura 3 segundos.</para>
    /// </summary>
    void AgarrarTotem()
    {
        iJugadorTotem = ObtenerJugadorAgarroTotem();
        if (mesa.Modo == ModoJuego.Dentro) //Todos se tiran a por el totem
        {
            TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
            totemBehaviour.SetAgarradoCorrecto();
            eventoListenerTotemTraido = new UnityAction(delegate () { DarCartas(true); });
            EventManager.StartListening("totemtraido", eventoListenerTotemTraido);
        }
        else
        {
            List<int> listaJugadoresEnemigos = mesa.VerificarIgualdadConResto(iJugadorTotem);
            for (int i = 0; i < listaJugadoresEnemigos.Count; i++)
            {
                Debug.Log("Enemigo en AgarrarTotem: " + listaJugadoresEnemigos[i]);
            }
            //mostrarFormasJugadores(); //SOLO PARA DEBUG
            if (listaJugadoresEnemigos.Count > 0) //Si hay algun jugador con el mismo simbolo
            {
                Debug.Log("Totem agarrado correctamente, llevatelo");
                TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
                totemBehaviour.SetAgarradoCorrecto();
                eventoListenerTotemTraido = new UnityAction(delegate () { DarCartas(false, listaJugadoresEnemigos); });
                EventManager.StartListening("totemtraido", eventoListenerTotemTraido);                
            }
            else //Agarro mal el totem
            {
                totemMalAgarrado();
                Debug.Log("Totem mal agarrado");
                ReiniciarTotem();
                //TODO: meterle las cartas de los otros y del totem al jugador
            }
        }
    }

    /// <summary>
    /// Le mete al jugador que agarro mal el totem las cartas de todos los demas
    /// </summary>
    void totemMalAgarrado()
    {
        /* El jugador que agarro mal el totem es el enemigo, y los demas son ganadores */
        List<int> listaEnemigos = new List<int>();
        listaEnemigos.Add(iJugadorTotem);
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            if (i != iJugadorTotem)
            {
                for (int j = 0; j < listaEnemigos.Count; j++)
                {
                    Debug.Log("Ganador en totemMalAgarrado: " + i);
                }
                StartCoroutine(llevarCartasAOtroMazo(i, listaEnemigos));
            }
        }
    }

    int ObtenerJugadorAgarroTotem()
    {
        TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
        return totemBehaviour.ObtenerJugador();
    }

    /*void mostrarFormasJugadores()
    {
        for (int i = 0; i < 4; i++)
        {
            Debug.Log("Forma del Jugador " + i + ": " + mesa.obtenerCartasDelJugador(i).Peek().forma);
        }        
    }*/

    /// <summary>
    /// Se le meten cartas a los perdedores o al centro de mesa.
    /// </summary>
    /// <param name="bAlCentro">Define si las cartas van para el centro</param>
    /// <param name="jugadoresEnemigos">Indica a que jugadores dar las cartas</param>
    void DarCartas(bool bAlCentro, List<int> jugadoresEnemigos = null)
    {
        if (!bAlCentro)
        { //Cartas para todos
            //string ids = "";
            //for (int i = 0; i < jugadoresEnemigos.Count; i++)
            //{
            //    ids += ", Jugador " + (jugadoresEnemigos[i]);
            //}
            //Debug.Log("Perdedores: " + ids.Substring(2)); //Le saca el primer ", "
            StartCoroutine(llevarCartasAOtroMazo(iJugadorTotem, jugadoresEnemigos));
        }
        else
        {
            //TODO: cartas de iIndexJugador para el mazo
        }
        ReiniciarTotem();
        mesa.NormalizarModo(); //Sea lo que sea siempre que se le den cartas a alguien el modo queda en normal
        EventManager.StopListening("totemtraido", eventoListenerTotemTraido);
    }

    /// <summary>
    /// Devuelve un Random entre 0 (incluyendolo) y el numero especificado (excluyendolo)
    /// </summary>
    /// <param name="iMax">El numero maximo del Random (excluyendolo)</param>
    private int ObtenerRandom(int iMax)
    {
        return UnityEngine.Random.Range(0, iMax);
    }

    /// <summary>
    /// Crea un GameObject (basandose en una carta) y lo anima saliendo del mazo
    /// </summary>
    /// <param name="cartaActual">La carta que se quiere animar</param>
    public void Crear_AnimarCarta(Carta cartaActual)
    {
        GameObject cartaCreada = crearCarta(cartaActual);
        animarCarta(cartaCreada, contrAnimacDelMazo[iIndexJugActual]);
        vecSeEstaAnimandoDesdeMazo[iIndexJugActual] = true;
        mesa.AgregarGameObject(cartaCreada);
    }

    /// <summary>
    /// Crea un GameObject a partir de una carta, y lo devuelve
    /// </summary>
    /// <param name="carta">La carta para crear el GameObject</param>
    public GameObject crearCarta(Carta carta)
    {
        GameObject gameObject = Instantiate(carta.img3D,
                                posicCartasDelMazo[iIndexJugActual],
                                Quaternion.Euler(rotacCartasDelMazo[iIndexJugActual]));
        return gameObject;
    }

    /// <summary>
    /// Anima el GameObject recibido con el AnimatorController recibido
    /// </summary>
    /// <param name="gameObject">El GameObject a animar</param>
    /// <param name="controller">El AnimatorController con el que animar al GameObject</param>
    public void animarCarta(GameObject gameObject, RuntimeAnimatorController controller)
    {
        //Debug.Log("AnimandoDesdeMazo: " + gameObject.name);
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        animator.runtimeAnimatorController = controller;
        animator.enabled = true;
        //Debug.Log("AnimeDesdeMazo: " + gameObject.name);
    }

    /// <summary>
    /// Se fija si hay alguna animacion de cualquier mazo que se este animando. 
    /// Si es asi, se fija si llego a 1seg de animacion, para detenerla
    /// </summary>
    public void verificarAnimacionesDesdeMazo()
    {
        for (int i = 0; i < 4; i++)
        {
            if (vecSeEstaAnimandoDesdeMazo[i])
            {
                vecTimersAnimacDesdeMazo[i] += Time.deltaTime;
                if (vecTimersAnimacDesdeMazo[i] >= fDuracAnimacionDesdeMazo)
                {
                    finAnimacionDesdeMazo(i);
                }
            }
        }
    }

    /// <summary>
    /// Detiene la animacion de la carta saliendo del mazo del jugador recibido
    /// </summary>
    /// <param name="idJugador">El ID del jugador desde donde sale la carta</param>
    public void finAnimacionDesdeMazo(int idJugador)
    {
        GameObject gameObjFinalizar = mesa.obtenerUltimoGameObjectDelJugador(idJugador);
        //Debug.Log("FinalizandoAnimacion: " + gameObjFinalizar.name);
        vecSeEstaAnimandoDesdeMazo[idJugador] = false;
        vecTimersAnimacDesdeMazo[idJugador] = 0f;
        gameObjFinalizar.GetComponent<Animator>().enabled = false; //Desactivo la animacion
        BoxCollider boxCollider = gameObjFinalizar.AddComponent<BoxCollider>(); //Creo un BoxCollider para que choque con el piso y con las cartas que caen despues
        boxCollider.center = new Vector3(0, 0, 0.065f);
        boxCollider.size = new Vector3(0.115f, 0.13f, 0.005f);
        Rigidbody rigidbody = gameObjFinalizar.AddComponent<Rigidbody>(); //Creo un RigidBody para que caiga con gravedad
        rigidbody.drag = 10f; //Para que la caida sea mas lenta
        //Debug.Log("FinalizoAnimacion: " + gameObjFinalizar.name);
        cambiarTurno(iIndexJugActual);
    }

    /// <summary>
    /// Si se estan animando cartas hacia algun mazo, cuento el tiempo. 
    /// Si ya paso el tiempo establecido, llamo a finAnimacionHaciaMazo() 
    /// </summary>
    public void verificarAnimacionesHaciaMazo()
    {
        if (bSeEstaAnimandoHaciaMazo)
        {
            fTimerAnimacHaciaMazo += Time.deltaTime;
            if (fTimerAnimacHaciaMazo >= fDuracAnimacionHaciaMazo)
            {
                finAnimacionHaciaMazo();
            }
        }
    }

    /// <summary>
    /// Destruyo los GameObjects que iban a sus mazos de destino, y me fijo si 
    /// algun mazo que se habia quedado sin cartas ahora tiene (y lo hago aparecer)
    /// </summary>
    public void finAnimacionHaciaMazo()
    {
        foreach (GameObject gameObject in gameObjectsAnimandoseHaciaMazo)
        {
            Destroy(gameObject);
        }
        gameObjectsAnimandoseHaciaMazo.Clear();
        bSeEstaAnimandoHaciaMazo = false;
        fTimerAnimacHaciaMazo = 0f;
        for (int i = 0; i < 4; i++)
        {
            verificarMazoVacioJugador(i);
        }
    }

    /// <summary>
    /// Reparte las cartas tiradas del jugador ganador entre todos los jugadores perdedores
    /// </summary>
    /// <param name="idJugadorGanador">El ID del jugador ganador</param>
    /// <param name="jugadoresEnemigos">Lista de los IDs de los jugadores perdedores</param>
    public IEnumerator llevarCartasAOtroMazo(int idJugadorGanador, List<int> jugadoresEnemigos)
    {
        /* Como le voy a dar todas las cartas tiradas del ganador a los perdedores, 
         * vacío el Stack de Cartas y de GameObjects del ganador, y se lo meto a cada perdedor
         */
        for (int i = 0; i < jugadoresEnemigos.Count; i++)
        {
            Debug.Log("Enemigo en llevarCartasAMazo: " + jugadoresEnemigos[i]);
        }        

        GameObject[] gameObjectsEnMesaDelJugador = mesa.obtener_VaciarGameObjectsDelJugador(idJugadorGanador);
        Carta[] cartasEnMesaDelJugador = mesa.obtener_VaciarCartasDelJugador(idJugadorGanador);
        List<RuntimeAnimatorController> contrParaUsar = obtenerContrAnimacHaciaMazos(idJugadorGanador, jugadoresEnemigos);

        int iCantEnemigos = jugadoresEnemigos.Count,
            iCantCartas = gameObjectsEnMesaDelJugador.Length,
            iPosiEnemigos = 0; //Reparto las cartas entre los enemigos de la lista jugadoresEnemigos
        GameObject gameObject;

        bSeEstaAnimandoHaciaMazo = true;
        for (int i = 0; i < iCantCartas; i++)
        {
            yield return new WaitForSeconds(0.05f);
            gameObject = gameObjectsEnMesaDelJugador[i];
            gameObjectsAnimandoseHaciaMazo.Add(gameObject);
            sacarCuerpo(gameObject);
            animarCarta(gameObject, contrParaUsar[iPosiEnemigos]);
            jugadores[jugadoresEnemigos[iPosiEnemigos]].AgregarCarta(cartasEnMesaDelJugador[i]);
            iPosiEnemigos = (iPosiEnemigos == iCantEnemigos - 1) ? 0 : iPosiEnemigos + 1;
        }
    }    

    /// <summary>
    /// Elimina el RigidBody y el BoxCollider del GameObject recibido, para que no se choque con nada ni lo afecte la gravedad
    /// </summary>
    /// <param name="gameObject">Al GameObject al cual sacarle el "cuerpo"</param>
    public void sacarCuerpo(GameObject gameObject)
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
    }

    /// <summary>
    /// Devuelve una lista de las animaciones hacia mazos a usar dependiendo del jugador ganador y de los perdedores
    /// </summary>
    /// <param name="idJugadorGanador">El ID del jugador ganador</param>
    /// <param name="jugadoresEnemigos">Lista de los IDs de los jugadores perdedores</param>
    public List<RuntimeAnimatorController> obtenerContrAnimacHaciaMazos(int idJugadorGanador, List<int> jugadoresEnemigos)
    {
        List<RuntimeAnimatorController> contrsParaUsar = new List<RuntimeAnimatorController>();
        int iCantEnemigos = jugadoresEnemigos.Count;
        for (int i = 0; i < iCantEnemigos; i++)
        {
            if (jugadoresEnemigos[i] < idJugadorGanador)
            {
                contrsParaUsar.Add(matrizContrAnimacHaciaMazos[idJugadorGanador, jugadoresEnemigos[i]]);
            }
            else
            {
                contrsParaUsar.Add(matrizContrAnimacHaciaMazos[idJugadorGanador, jugadoresEnemigos[i] - 1]);
            }
        }
        return contrsParaUsar;
    }

    /// <summary>
    /// Permite ordenar un vector
    /// </summary>
    public static int CompareObNames(GameObject x, GameObject y) //Ordenar por nombre
    {
        return x.name.CompareTo(y.name);
    }
}
