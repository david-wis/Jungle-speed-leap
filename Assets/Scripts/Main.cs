using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Reflection;
using Leap.Unity.Interaction;

public class Main : MonoBehaviour
{
    public const int CANTCARTAS = 80;
    //public const int CANTCARTAS = 16;

    public const int CANTJUGADORES = 4;
    public RuntimeAnimatorController[] contrAnimacDesdeMazo = new RuntimeAnimatorController[4];
    public RuntimeAnimatorController[] contrAnimac0HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimac1HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimac2HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimac3HaciaMazos = new RuntimeAnimatorController[3];
    public RuntimeAnimatorController[] contrAnimacDesdeTotem = new RuntimeAnimatorController[4];
    public RuntimeAnimatorController[] contrAnimacHaciaTotem = new RuntimeAnimatorController[4];
    private RuntimeAnimatorController[,] matrizContrAnimacHaciaMazos = new RuntimeAnimatorController[4, 3];

    Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    public GameObject[] cartasEstaticas = new GameObject[4];
    GameObject[] mazos = new GameObject[4];

    //GameObject totem;
    public GameObject turnos;
    public GameObject btnReinciar;

    public GameObject mensaje;
    public Jugador[] jugadores
    {
        set { MesaManager.instance.jugadores = value; }
        get { return MesaManager.instance.jugadores; }
    }

    public Mesa mesa {
       set { MesaManager.instance.mesa = value; }
       get { return MesaManager.instance.mesa; }
    }

    public GameObject totem
    {
        set { TotemManager.instance.totem = value; }
        get { return TotemManager.instance.totem; }
    }

    Vector3[] posicCartasDelMazo = { new Vector3(0.13f, 1.5f, -0.43f),
                                     new Vector3(0.2f, 1.5f, -0.09f),
                                     new Vector3(-0.235f, 1.5f, 0.15f),
                                     new Vector3(-0.38f, 1.5f, -0.25f)};
    Vector3[] rotacCartasDelMazo = { new Vector3(-90f, 0f, -175f),
                                     new Vector3(-90f, -90f, -175f),
                                     new Vector3(-90f, -90f, 95f),
                                     new Vector3(-90f, -90f, 5f)};

    List<GameObjectAnimandose> listaGameObjectsAnimandose = new List<GameObjectAnimandose>();
    
    public const float fDuracAnimaciones = 1.1f;
        
    int iIndexJugActual {
        set { MesaManager.instance.iIndexJugActual = value; }
        get { return MesaManager.instance.iIndexJugActual; }
    }

    UnityAction eventoListenerMazo0, eventoListenerMazo1, eventoListenerMazo2, eventoListenerMazo3;
    UnityAction eventoListenerTotem;
    UnityAction eventoListenerRestablecerTotem;

    // Use this for initialization
    void Start()
    {
        jugadores = new Jugador[CANTJUGADORES];
        //totem.transform.position += new Vector3(0, 3, 0);
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
        CambiarTurno(0);
    }

    float fTimer = 0.0f; //Bereishit
    //bool bCartaEsperando = false;
    float fLastTime = -2.0f; //Ultima vez que se tocó el mazo
    bool bPause = false; //Permite pausar el mundo
    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;
        VerificarAnimaciones();
        VerificarFinPartida(); //TODO: Por ahi conviene meterlo en otro lado para que no se repita con cada update
    }

    private void VerificarFinPartida()
    {
        int i = 0;
        int iJugadorGanador = -1;
        while (i < 4 && iJugadorGanador == -1)
        {
            if (jugadores[i].obtenerCantCartas() == 0 && mesa.obtenerCartasDelJugador(i).Count == 0)
            {
                iJugadorGanador = i;
            } else
            {
                i++;
            }
        }

        if (iJugadorGanador != -1)
        {
            Debug.Log("Partida finalizada!");
            mesa.terminarPartida();
            TerminarPartida(iJugadorGanador == 0);
            totem.SetActive(false);
            btnReinciar.transform.position = new Vector3(-0.42f, 1.8f, -0.4f); //Para que quede cerquita del jugador
        }
    }

    /// <summary>
    /// Muestra el mensaje final que indica el resultado de la partida
    /// </summary>
    /// <param name="bUsuarioGanador">Gano el usuario</param>
    private void TerminarPartida(bool bUsuarioGanador)
    {
        TextMesh txt = mensaje.GetComponent<TextMesh>();
        if (bUsuarioGanador)
        {
            txt.text = "Ganaste!!!";
        }
        else
        {
            txt.text = "Perdiste :(";
        }
        //Abril, meté acá el codigo de los mensajes
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
            jugadores[iIndexJug].agregarCarta(arrayMazoTotal[i]);
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

    /*IEnumerator PonerCartaBot(int iIndex)
    {
        yield return new WaitForSeconds(fCoolDown);
        PonerCarta(iIndex);
    }*/

    /// <summary>
    /// Si es su turno, hace la operacion de levantar la carta de un jugador
    /// </summary>
    /// <param name="iIndexMazo">Indice del jugador que intenta levantar su carta</param>
    /// <param name="bFlechaAfuera">Si se quieren levantar todas las cartas al mismo tiempo, no hay intervalo de tiempo</param>
    void PonerCarta(int iIndexMazo, bool bFlechaAfuera = false)
    {
        if ((bFlechaAfuera == bPause) && !(MesaManager.instance.AlguienToca))
        {
            if (iIndexJugActual == iIndexMazo)
            {
                if ((fTimer - fLastTime >= fCoolDown) || bFlechaAfuera)
                { //Asi evitamos que mantener la mano apretada cause que haga todo al instante
                    MesaManager.instance.reiniciarTimer();
                    Carta cartaActual = jugadores[iIndexJugActual].obtenerSiguienteCarta();
                    if (cartaActual != null)
                    {
                        ModoJuego modo = mesa.agregarCarta(cartaActual, iIndexJugActual); //Agrega la carta al vector de cartas de la mesa
                        Crear_AnimarCarta(cartaActual); //Crea la carta y la anima
                        cartasEstaticas[iIndexJugActual].transform.parent.gameObject.SetActive(true);
                        Image imagen = cartasEstaticas[iIndexJugActual].GetComponent<Image>();
                        var color = imagen.color;
                        color.a = 1;
                        imagen.sprite = cartaActual.img2D;
                        imagen.color = color;
                        fLastTime = fTimer;
                        VerificarMazoVacioJugador(iIndexJugActual);
                        iIndexJugActual = (iIndexJugActual < 3) ? iIndexJugActual + 1 : 0;
                        //bCartaEsperando = false;
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
    void CambiarTurno(int iOpcion)
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
    public void VerificarMazoVacioJugador(int idJugador)
    {
        if (jugadores[idJugador].obtenerCantCartas() == 0)
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
        CambiarTurno(4);
        yield return new WaitForSeconds(3f);
        mesa.normalizarModo();
        //Debug.Log("Flechas para afuera");
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
        desactivarCuerposGameObjects(); //Para que el Totem no se choque con las cartas en mesa
        if (mesa.Modo == ModoJuego.Dentro) //Todos se tiran a por el totem
        {
            EventManager.StopListening("totemtraido", eventoListenerTotemTraido);
            TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
            totemBehaviour.SetAgarradoCorrecto();
            eventoListenerTotemTraido = new UnityAction(delegate () { DarCartas(true); });
            EventManager.StartListening("totemtraido", eventoListenerTotemTraido);
        }
        else
        {
            List<int> listaJugadoresEnemigos = mesa.verificarIgualdadConResto(iJugadorTotem);
            MostrarEnemigosPorDebug(listaJugadoresEnemigos, "AgarrarTotem");
            if (listaJugadoresEnemigos.Count > 0) //Si hay algun jugador con el mismo simbolo
            {
                EventManager.StopListening("totemtraido", eventoListenerTotemTraido);
                //Debug.Log("Totem agarrado correctamente, llevatelo");
                TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
                totemBehaviour.SetAgarradoCorrecto();
                eventoListenerTotemTraido = new UnityAction(delegate () { DarCartas(false, listaJugadoresEnemigos); });
                EventManager.StartListening("totemtraido", eventoListenerTotemTraido);
            }
            else //Agarro mal el totem
            {
                TotemMalAgarrado();
                //Debug.Log("Totem mal agarrado");
                ReiniciarTotem();
            }
        }
        //mostrarDiccEventos(); //DEBUG
    }

    void MostrarEnemigosPorDebug(List<int> listaJugadoresEnemigos, String dondeEstas)
    {
        for (int i = 0; i < listaJugadoresEnemigos.Count; i++)
        {
            //Debug.Log("--Enemigo en " + dondeEstas + ": " + listaJugadoresEnemigos[i]);
        }
    }

    /// <summary>
    /// Desactiva el BoxCollider y la Gravity de todos los GameObjects en Mesa
    /// </summary>
    public void desactivarCuerposGameObjects()
    {
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            GameObject[] gameObjects = mesa.obtenerGameObjectsDelJugador(i);
            foreach (GameObject gameObject in gameObjects)
            {
                SacarCuerpo(gameObject);
            }
        }
    }

    /// <summary>
    /// Le mete al jugador que agarro mal el totem las cartas de todos los demas
    /// </summary>
    void TotemMalAgarrado()
    {
        /* El jugador que agarro mal el totem es el enemigo, y los demas son ganadores */
        List<int> listaEnemigos = new List<int>();
        listaEnemigos.Add(iJugadorTotem);
        StartCoroutine(LlevarCartasDesdeTotem(listaEnemigos)); //Cartas del Totem al que agarro mal
        for (int i = 0; i < CANTJUGADORES; i++) //Cartas de todos (menos del que agarro mal) al que agarro mal
        {
            if (i != iJugadorTotem)
            {
                StartCoroutine(LlevarCartasAOtroMazo(i, listaEnemigos));
            }
        }
        ReactivarCuerposGameObjects();
        NingunoBuscaTotem();
    }

    /// <summary>
    /// Obtiene el indice del jugador que agarró el totem
    /// </summary>
    /// <returns>Indice del jugador que agarró el totem</returns>
    int ObtenerJugadorAgarroTotem()
    {
        TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
        return totemBehaviour.ObtenerJugador();
    }

    /// <summary>
    /// Se le meten cartas a los perdedores o al centro de mesa.
    /// </summary>
    /// <param name="bAlCentro">Define si las cartas van para el centro</param>
    /// <param name="jugadoresEnemigos">Indica a que jugadores dar las cartas</param>
    void DarCartas(bool bAlCentro, List<int> jugadoresEnemigos = null)
    {
        if (!bAlCentro) //Batalla
        { 
            //Les da a los perdedores las cartas del ganador, y las que estaban en el Totem
            StartCoroutine(LlevarCartasAOtroMazo(iJugadorTotem, jugadoresEnemigos));
            StartCoroutine(LlevarCartasDesdeTotem(jugadoresEnemigos));
        }
        else
        {
            //Las cartas del que lo agarro van al Totem
            StartCoroutine(LlevarCartasAlTotem(iJugadorTotem));
        }
        ReactivarCuerposGameObjects();
        ReiniciarTotem();
        NingunoBuscaTotem();
        mesa.normalizarModo(); //Sea lo que sea siempre que se le den cartas a alguien el modo queda en normal

        //Debug.Log("---Fin dando cartas---");
    }

    /// <summary>
    /// Pone todo el vecAgarrandoTotem del MesaManager en False
    /// </summary>
    private void NingunoBuscaTotem()
    {
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            MesaManager.instance.CambiarEstadoToque(i, false);
        }
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
        //float fDuracAnimac = buscarDuracionTipoAnimacion("DesdeMazo");
        listaGameObjectsAnimandose.Add(new GameObjectAnimandose(cartaCreada, 0f, fDuracAnimaciones, TipoAnimacion.DesdeMazo));
        AnimarCarta(cartaCreada, contrAnimacDesdeMazo[iIndexJugActual]);
        mesa.agregarGameObject(cartaCreada, iIndexJugActual);
        MesaManager.instance.CartaAnimandoseEnMesa = true;
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
        //Debug.Log("Creando " + gameObject.name + " del jugador " + iIndexJugActual);
        return gameObject;
    }

    /// <summary>
    /// Anima el GameObject recibido con el AnimatorController recibido
    /// </summary>
    /// <param name="gameObject">El GameObject a animar</param>
    /// <param name="controller">El AnimatorController con el que animar al GameObject</param>
    public void AnimarCarta(GameObject gameObject, RuntimeAnimatorController controller)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        animator.runtimeAnimatorController = controller;
        animator.enabled = true;
    }

    public void VerificarAnimaciones()
    {
        bool hayCartasAnimandose = false;
        for (int i = 0; i < listaGameObjectsAnimandose.Count; i++)
        {
            GameObjectAnimandose gameObjectAnimandose = listaGameObjectsAnimandose[i];
            if (gameObjectAnimandose != null)
            {
                gameObjectAnimandose.TimerAnimacion += Time.deltaTime;

                String strTipoAnimacion = gameObjectAnimandose.TipoAnimacion.ToString();
                if (gameObjectAnimandose.TimerAnimacion >= gameObjectAnimandose.DuracAnimacion)
                {
                    this.GetType().GetMethod("FinAnimacion" + strTipoAnimacion).Invoke(this, new GameObject[] { gameObjectAnimandose.GameObject }); //Llamo a la funcion que corresponda
                    listaGameObjectsAnimandose.RemoveAt(i);
                }
                else
                {
                    hayCartasAnimandose = true;
                }
            }
            else
            {
                Debug.Log("CartaVerificando: NULL");
            }            
        }
        if (MesaManager.instance.CartaAnimandoseEnMesa != hayCartasAnimandose) //Si cambió el valor, le cambio a MesaManager
        {
            MesaManager.instance.CartaAnimandoseEnMesa = hayCartasAnimandose;
        }
    }

    /*public float buscarDuracionTipoAnimacion(String strTipoAnimacion)
    {
        return (float) this.GetType().GetField("fDuracAnimacion" + strTipoAnimacion).GetValue(this);
    }*/

    public void FinAnimacionDesdeMazo(GameObject gameObjFinalizar)
    {
        gameObjFinalizar.GetComponent<Animator>().enabled = false; //Desactivo la animacion
        BoxCollider boxCollider = gameObjFinalizar.AddComponent<BoxCollider>(); //Creo un BoxCollider para que choque con el piso y con las cartas que caen despues
        boxCollider.center = new Vector3(0, 0, 0.065f);
        boxCollider.size = new Vector3(0.115f, 0.13f, 0.005f);
        Rigidbody rigidbody = gameObjFinalizar.AddComponent<Rigidbody>(); //Creo un RigidBody para que caiga con gravedad
        rigidbody.drag = 1f; //Para que la caida sea mas lenta
        CambiarTurno(iIndexJugActual);
    }

    public void FinAnimacionHaciaMazo(GameObject gameObjFinalizar)
    {
        Destroy(gameObjFinalizar);
        for (int i = 0; i < 4; i++)
        {
            VerificarMazoVacioJugador(i);            
        }
        ReactivarCuerposGameObjects();
    }

    public void FinAnimacionHaciaTotem(GameObject gameObjFinalizar)
    {
        gameObjFinalizar.GetComponent<Animator>().enabled = false;
        PonerCuerpo(gameObjFinalizar);
    }
    
    public void FinAnimacionDesdeTotem(GameObject gameObjFinalizar)
    {
        Destroy(gameObjFinalizar);
    }

    /// <summary>
    /// Activa el BoxCollider y la Gravity de todos los GameObjects en Mesa
    /// </summary>
    public void ReactivarCuerposGameObjects()
    {
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            GameObject[] gameObjects = mesa.obtenerGameObjectsDelJugador(i);
            foreach (GameObject gameObject in gameObjects)
            {
                PonerCuerpo(gameObject);
            }
        }
    }

    /// <summary>
    /// Activa el BoxCollider y la Gravity del GameObject recibido como parametro
    /// </summary>
    /// <param name="gameObject">El GameObject al que se quiere activar el BoxCollider y la Gravity</param>
    public void PonerCuerpo(GameObject gameObject)
    {
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    /// <summary>
    /// Reparte las cartas tiradas del jugador ganador entre todos los jugadores perdedores
    /// </summary>
    /// <param name="idJugadorGanador">El ID del jugador ganador</param>
    /// <param name="jugadoresEnemigos">Lista de los IDs de los jugadores perdedores</param>
    public IEnumerator LlevarCartasAOtroMazo(int idJugadorGanador, List<int> jugadoresEnemigos)
    {
        /* Como le voy a dar todas las cartas tiradas del ganador a los perdedores, 
         * vacío el Stack de Cartas y de GameObjects del ganador, y se lo meto a cada perdedor
         */
        GameObject[] gameObjectsEnMesaDelJugador = mesa.obtener_VaciarGameObjectsDelJugador(idJugadorGanador);
        Carta[] cartasEnMesaDelJugador = mesa.obtener_VaciarCartasDelJugador(idJugadorGanador);
        List<RuntimeAnimatorController> contrParaUsar = ObtenerContrAnimacHaciaMazos(idJugadorGanador, jugadoresEnemigos);

        int iCantEnemigos = jugadoresEnemigos.Count,
            iCantCartas = gameObjectsEnMesaDelJugador.Length,
            iPosiEnemigos = 0; //Reparto las cartas entre los enemigos de la lista jugadoresEnemigos
        GameObject gameObject;

        for (int i = 0; i < iCantCartas; i++)
        {
            yield return new WaitForSeconds(0.05f);
            gameObject = gameObjectsEnMesaDelJugador[i];
            //float fDuracAnimac = buscarDuracionTipoAnimacion("HaciaMazo");
            listaGameObjectsAnimandose.Add(new GameObjectAnimandose(gameObject, 0f, fDuracAnimaciones, TipoAnimacion.HaciaMazo));
            SacarCuerpo(gameObject);
            AnimarCarta(gameObject, contrParaUsar[iPosiEnemigos]);
            jugadores[jugadoresEnemigos[iPosiEnemigos]].agregarCarta(cartasEnMesaDelJugador[i]);
            iPosiEnemigos = (iPosiEnemigos == iCantEnemigos - 1) ? 0 : iPosiEnemigos + 1;
        }
    }

    /// <summary>
    /// Lleva las cartas en mesa del jugador ganador hacia el Totem
    /// </summary>
    /// <param name="idJugadorGanador">El ID del jugador ganador</param>
    public IEnumerator LlevarCartasAlTotem(int idJugadorGanador)
    {
        GameObject[] gameObjectsEnMesaDelJugador = mesa.obtener_VaciarGameObjectsDelJugador(idJugadorGanador);
        Carta[] cartasEnMesaDelJugador = mesa.obtener_VaciarCartasDelJugador(idJugadorGanador);
        RuntimeAnimatorController contrParaUsar = ObtenerContrAnimacHaciaTotem(idJugadorGanador);

        int iCantCartas = gameObjectsEnMesaDelJugador.Length;
        GameObject gameObject;
        TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();

        for (int i = 0; i < iCantCartas; i++)
        {
            yield return new WaitForSeconds(0.1f);
            gameObject = gameObjectsEnMesaDelJugador[i];
            //float fDuracAnimac = buscarDuracionTipoAnimacion("HaciaTotem");
            listaGameObjectsAnimandose.Add(new GameObjectAnimandose(gameObject, 0f, fDuracAnimaciones, TipoAnimacion.HaciaTotem));
            SacarCuerpo(gameObject);
            totemBehaviour.agregarCartaALista(cartasEnMesaDelJugador[i]);
            totemBehaviour.agregarGameObjALista(gameObject);
            AnimarCarta(gameObject, contrParaUsar);
        }
    }

    /// <summary>
    /// Lleva las cartas del Totem al mazo del Jugador perdedor
    /// </summary>
    /// <param name="idPerdedor">El ID del Jugador perdedor</param>
    public IEnumerator LlevarCartasDesdeTotem(List<int> jugadoresEnemigos)
    {
        TotemBehaviour totemBehaviour = totem.GetComponent<TotemBehaviour>();
        List<Carta> cartasEnTotem = totemBehaviour.obtener_VaciarCartasEnTotem();
        List<GameObject> gameObjectsEnTotem = totemBehaviour.obtener_VaciarGameObjectsEnTotem();
        List<RuntimeAnimatorController> contrParaUsar = ObtenerContrAnimacDesdeTotem(jugadoresEnemigos);

        MostrarEnemigosPorDebug(jugadoresEnemigos, "llevarCartasDesdeTotem");
        //Debug.Log("--Cantidad de Cartas en llevarCartasDesdeTotem: " + cartasEnTotem.Count);
        MostrarNombresGameObjsPorDebug(gameObjectsEnTotem, "llevarCartasDesdeTotem");

        int iCantEnemigos = jugadoresEnemigos.Count, 
            iCantCartas = cartasEnTotem.Count, 
            iPosiEnemigos = 0;
        GameObject gameObject;

        for (int i = 0; i < iCantCartas; i++)
        {
            yield return new WaitForSeconds(0.1f);
            gameObject = gameObjectsEnTotem[i];
            //float fDuracAnimac = buscarDuracionTipoAnimacion("DesdeTotem");
            listaGameObjectsAnimandose.Add(new GameObjectAnimandose(gameObject, 0f, fDuracAnimaciones, TipoAnimacion.DesdeTotem));
            SacarCuerpo(gameObject);
            AnimarCarta(gameObject, contrParaUsar[iPosiEnemigos]);
            jugadores[jugadoresEnemigos[iPosiEnemigos]].agregarCarta(cartasEnTotem[i]);
            iPosiEnemigos = (iPosiEnemigos == iCantEnemigos - 1) ? 0 : iPosiEnemigos + 1;
        }
    }

    void MostrarNombresGameObjsPorDebug(List<GameObject> gameObjs, String dondeEstas)
    {
        //Debug.Log("Cantidad de gameObjects en " + dondeEstas + ": " + gameObjs.Count);
        foreach (GameObject gameObj in gameObjs) {
            //Debug.Log("--gameObject en " + dondeEstas + ": " + gameObj.name);
        }
    }

    /// <summary>
    /// Elimina el RigidBody y el BoxCollider del GameObject recibido, para que no se choque con nada ni lo afecte la gravedad
    /// </summary>
    /// <param name="gameObject">Al GameObject al cual sacarle el "cuerpo"</param>
    public void SacarCuerpo(GameObject gameObject)
    {
        if (gameObject.GetComponent<Rigidbody>() != null)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        if (gameObject.GetComponent<BoxCollider>() != null)
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }       
        
    }

    /// <summary>
    /// Devuelve una lista de las animaciones hacia mazos a usar dependiendo del jugador ganador y de los perdedores
    /// </summary>
    /// <param name="idJugadorGanador">El ID del jugador ganador</param>
    /// <param name="jugadoresEnemigos">Lista de los IDs de los jugadores perdedores</param>
    public List<RuntimeAnimatorController> ObtenerContrAnimacHaciaMazos(int idJugadorGanador, List<int> jugadoresEnemigos)
    {
        List<RuntimeAnimatorController> contrsParaUsar = new List<RuntimeAnimatorController>();
        int iCantEnemigos = jugadoresEnemigos.Count;
        for (int i = 0; i < iCantEnemigos; i++)
        {
            if (idJugadorGanador > jugadoresEnemigos[i])
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
    /// Devuelve el controlador de la animacion hacia el Totem del jugador que gano
    /// </summary>
    /// <param name="idJugadorGanador">El ID del jugador que gano</param>
    /// <returns></returns>
    public RuntimeAnimatorController ObtenerContrAnimacHaciaTotem(int idJugadorGanador)
    {
        return contrAnimacHaciaTotem[idJugadorGanador];
    }

    /// <summary>
    /// Devuelve la lista de controllers de animaciones a usar desde el Totem
    /// </summary>
    /// <param name="jugadoresEnemigos">Lista de jugadores que deben recibir las cartas del Totem</param>
    /// <returns></returns>
    public List<RuntimeAnimatorController> ObtenerContrAnimacDesdeTotem(List<int> jugadoresEnemigos)
    {
        List<RuntimeAnimatorController> controllers = new List<RuntimeAnimatorController>();
        foreach(int iEnemigo in jugadoresEnemigos)
        {
            controllers.Add(contrAnimacDesdeTotem[iEnemigo]);
        }
        return controllers;
    }

    /// <summary>
    /// Permite ordenar un vector
    /// </summary>
    public static int CompareObNames(GameObject x, GameObject y) //Ordenar por nombre
    {
        return x.name.CompareTo(y.name);
    }
}
