using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System;

public class Main : MonoBehaviour {
    public const int CANTCARTAS = 72; //Por ahora las cartas especiales no estan metidas
    public const int CANTJUGADORES = 4;
    public RuntimeAnimatorController[] contrAnimacDelMazo = new RuntimeAnimatorController[4];
    public RuntimeAnimatorController[] contrAnimac0HaciaMazos = new RuntimeAnimatorController[3];

    Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    Jugador[] jugadores = new Jugador[CANTJUGADORES];
    public GameObject[] cartasEstaticas = new GameObject[4];
    GameObject[] mazos = new GameObject[4];

    GameObject totem;

    Mesa mesa = new Mesa();

    Vector3[] posicCartasDelMazo = { new Vector3(0.13f, 1.5f, -0.43f),
                                     new Vector3(0.2f, 1.5f, -0.09f),
                                     new Vector3(-0.235f, 1.5f, 0.15f),
                                     new Vector3(-0.38f, 1.5f, -0.25f)};
    Vector3[] rotacCartasDelMazo = { new Vector3(-90f, 0f, -175f),
                                     new Vector3(-90f, -90f, -175f),
                                     new Vector3(-90f, -90f, 95f),
                                     new Vector3(-90f, -90f, 5f)};
    GameObject cartaCreada = null; //El gameObject que se crea con la carta del mazo
    float fTimerAnimacion = 0f; //Cuando llega a 1s (la animacion termino), le pongo gravedad a la carta
    bool seEstaAnimandoDesdeMazo = false;

    int iIndexJugActual;
    UnityAction eventoListenerMazo0, eventoListenerMazo1, eventoListenerMazo2, eventoListenerMazo3;
    UnityAction eventoListenerTotem;

    // Use this for initialization
    void Start() {
        /* Aca en algun momento vamos a meter un menu 
         * para hostear partidas y elegir cant de jugadores
         */

        LlenarMazo();
        ReferenciarCartasEstaticas();
        ReferenciarMazos();
        CrearJugadores();

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

        EventManager.StartListening("agarrarcarta0", eventoListenerMazo0); //Evento que se produce cuando un jugador toca un mazo
        EventManager.StartListening("agarrarcarta1", eventoListenerMazo1); 
        EventManager.StartListening("agarrarcarta2", eventoListenerMazo2); 
        EventManager.StartListening("agarrarcarta3", eventoListenerMazo3);

        EventManager.StartListening("agarrartotem", eventoListenerTotem); //Evento que se produce cuando un jugador agarra el totem

        //En el futuro eligiremos el jugador inicial de manera aleatoria
        //iIndexJugActual = ObtenerRandom(4); 
        iIndexJugActual = 0;
    }

    float fTimer = 0.0f; //Bereishit
    bool bCartaEsperando = false;
    float fLastTime = -2.0f; //Ultima vez que se tocó el mazo
    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;
        if (iIndexJugActual != 0)
        {
            if (!bCartaEsperando)
            {
                StartCoroutine(PonerCartaBot(iIndexJugActual));
                bCartaEsperando = true;
            }
        }
        if (seEstaAnimandoDesdeMazo) //Si esta la animacion, cuento cuanto tiempo va pasando
        {
            fTimerAnimacion += Time.deltaTime;
            if (fTimerAnimacion >= 1.0f) //Si ya paso 1s de animacion (terminó), hago que la carta caiga
            {
                finAnimacion();
            }
        }
    }

    private void LlenarMazo()
    {
        string[] rutaCartitas = AssetDatabase.FindAssets("b:carta", new[] { "Assets/Cartas" });
        for (int i = 0; i < CANTCARTAS; i++)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(rutaCartitas[i]);
            arrayMazoTotal[i] = (Carta)AssetDatabase.LoadAssetAtPath(ruta, typeof(Carta));
        }
    }

    void ReferenciarCartasEstaticas()
    {
        cartasEstaticas = GameObject.FindGameObjectsWithTag("CartasEstaticas");
        Array.Sort(cartasEstaticas, CompareObNames);
    }

    void ReferenciarMazos()
    {
        mazos = GameObject.FindGameObjectsWithTag("Mazo");
        Array.Sort(mazos, CompareObNames);
    }

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

    private void RepartirMazo(int iCantJugadores = 4)
    {
        Mezclar();
        for (int i = 0; i < arrayMazoTotal.Length; i++)
        {
            //TODO: dividirlo por iCantJugadores, cuando permitamos elegir la cantidad de jugadores
            int iIndexJug = i % 4;
            jugadores[iIndexJug].AgregarCarta(arrayMazoTotal[i]);
        }
    }

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

    const float fCoolDown = 1.0f; //1 segundo
    IEnumerator PonerCartaBot(int iIndex)
    {
        yield return new WaitForSeconds(fCoolDown);
        PonerCarta(iIndex);
    }

    void PonerCarta(int iIndexMazo)
    {
        if (iIndexJugActual == iIndexMazo) {
            if (fTimer - fLastTime >= fCoolDown) { //Asi evitamos que mantener la mano apretada cause que haga todo al instante
                Carta cartaActual = jugadores[iIndexJugActual].ObtenerSiguienteCarta();
                //Carta cartaActual = agarrarCartaVioleta0();
                if (cartaActual != null)
                {
                    mesa.AgregarCarta(cartaActual); //Agregamos la carta al vector de cartas de la mesa
                    AnimarCarta(cartaActual); //Crea la carta y la anima
                    Image imagen = cartasEstaticas[iIndexJugActual].GetComponent<Image>();
                    var color = imagen.color;
                    color.a = 1;
                    imagen.sprite = cartaActual.img2D;
                    imagen.color = color;
                    fLastTime = fTimer;
                    //Debug.Log("Jugador " + iIndexJugActual + " - " + cartaActual.img2D.name + " - " + imagen.name + " - " + mazos[iIndexJugActual].name);
                    if (jugadores[iIndexJugActual].ObtenerCantCartas() == 0)
                    {
                        mazos[iIndexJugActual].SetActive(false);
                    }
                    iIndexJugActual = (iIndexJugActual < 3) ? iIndexJugActual + 1 : 0;
                    bCartaEsperando = false;
                }
            }
        }
    }

    /*
     *  Hasta que hagamos el multiplayer, iJugadorTotem va a ser una variable
     *  que se va a modificar siempre por alguna funcion del mismo main.
     *  Si armamos el multiplayer, vamos a tener que pasarle un parametro a AgarrarTotem
     *  que indique que jugador realizo el grasp, ya que no va a ser tan facil 
     *  predecirlo como en este caso, en el que o bien decidimos nosotros que bot 
     *  agarra el totem, o por defecto lo hace el único jugador.
     */
    int iJugadorTotem = 0; //Jugador que agarro el totem
    UnityAction eventoListenerTotemTraido;
    void AgarrarTotem()
    {
        List<int> listaJugadoresEnemigos = mesa.VerificarIgualdadConResto(iJugadorTotem);
        listaJugadoresEnemigos.Add(1); //SOLO PARA DEBUG
        if (listaJugadoresEnemigos.Count > 0) //Si hay algun jugador con el mismo simbolo
        {
            Debug.Log("Totem agarrado, ahora es momento de llevarlo a su lugar");
            EventManager.TriggerEvent("totemagarrado");
            eventoListenerTotemTraido = new UnityAction(delegate () { DarCartas(listaJugadoresEnemigos); });
            EventManager.StartListening("totemtraido", eventoListenerTotemTraido);
            //EventManager.TriggerEvent("totemtraido"); //SOLO PARA DEBUG
        } else
        {
            Debug.Log("wtf, sacame la manito bro");
        }
    }

    void DarCartas(List<int> jugadoresEnemigos)
    {
        //TODO: se les meten las cartas a los demas
        string ids = "";
        for (int i = 0; i < jugadoresEnemigos.Count; i++)
        {
            ids += ", Jugador " + (jugadoresEnemigos[i] + 1);
        }
        Debug.Log("Chupate esta! " + ids.Substring(2));
        StartCoroutine(llevarCartasAOtroMazo(iJugadorTotem, jugadoresEnemigos));
    }

    private Carta agarrarCartaVioleta0()
    {
        string[] rutaCartitas = AssetDatabase.FindAssets("b:carta", new[] { "Assets/Cartas" });
        bool bEncontrado = false;
        int iCantCartas = rutaCartitas.Length, iPosi = 0;
        Carta cartaViol0 = null;
        while (!bEncontrado && iPosi < iCantCartas)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(rutaCartitas[iPosi]);
            Carta cartaAux = (Carta)AssetDatabase.LoadAssetAtPath(ruta, typeof(Carta));
            if (cartaAux.ToString().Equals("Violeta-0"))
            {
                bEncontrado = true;
                cartaViol0 = cartaAux;
            }
            else
            {
                iPosi++;
            }
        }
        return cartaViol0;
    }

    private int ObtenerRandom(int iMax)
    {
        return UnityEngine.Random.Range(0, iMax);
    }

    public void AnimarCarta(Carta cartaActual)
    {
        cartaCreada = Instantiate(cartaActual.img3D, 
                            posicCartasDelMazo[iIndexJugActual], 
                            Quaternion.Euler(rotacCartasDelMazo[iIndexJugActual]));
        Animator elAnimador = cartaCreada.AddComponent<Animator>();
        elAnimador.runtimeAnimatorController = contrAnimacDelMazo[iIndexJugActual];
        seEstaAnimandoDesdeMazo = true;
        //mesa.AgregarGameObject(cartaCreada);
    }

    public void finAnimacion()
    {
        seEstaAnimandoDesdeMazo = false;
        fTimerAnimacion = 0f;
        cartaCreada.GetComponent<Animator>().enabled = false; //Desactivo la animacion
        BoxCollider boxCollider = cartaCreada.AddComponent<BoxCollider>(); //Creo un BoxCollider para que choque con el piso y con las cartas que caen despues
        boxCollider.center = new Vector3(0, 0, 0.065f);
        boxCollider.size = new Vector3(0.115f, 0.13f, 0.005f);
        Rigidbody rigidbody = cartaCreada.AddComponent<Rigidbody>(); //Creo un RigidBody para que caiga con gravedad
        rigidbody.drag = 10f; //Para que la caida sea mas lenta
    }

    public IEnumerator llevarCartasAOtroMazo(int idJugadorGanador, List<int> jugadoresEnemigos)
    {
        /* Recibe el ID del ganador y los IDs de los perdedores, y manda las cartas del ganador 
         * a los mazos de los perdedores */
        Debug.Log("En llevarCartasAOtroMazo()");
        //idJugadorGanador = 0; //SOLO PARA DEBUG

        //Stack<GameObject> gameObjectsEnMesaDelJugador = mesa.obtenerGameObjectsDelJugador(idJugadorGanador);
        Stack<Carta> cartasEnMesaDelJugador = mesa.obtenerCartasDelJugador(idJugadorGanador);
        RuntimeAnimatorController[] contrAnimacionesGanador = obtenerContrAnimacionesDelGanador(idJugadorGanador);
        foreach (Carta carta in cartasEnMesaDelJugador)
        {
            GameObject gameObject = carta.img3D;
            //Debug.Log("GameObj en mesa: " + gameObject.name);
            Animator animator = gameObject.GetComponent<Animator>();
            //Hacer que la animacion dependa de los enemigos
            animator.runtimeAnimatorController = contrAnimacionesGanador[0];
            animator.enabled = true;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public RuntimeAnimatorController[] obtenerContrAnimacionesDelGanador(int idJugadorGanador)
    {
        switch (idJugadorGanador)
        {
            case 0:
                return contrAnimac0HaciaMazos;
            /*case 1:
                return contrAnimac1HaciaMazos;
            case 2:
                return contrAnimac2HaciaMazos;
            case 3:
                return contrAnimac3HaciaMazos;*/
            default:
                return null;
        }
    }

    public static int CompareObNames(GameObject x, GameObject y) //Ordenar por nombre
    {
        return x.name.CompareTo(y.name);
    }
}
