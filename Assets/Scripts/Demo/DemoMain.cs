using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System;
using System.Reflection;
using Leap.Unity.Interaction;
using TMPro;
using UnityEngine.SceneManagement;

public class DemoMain : MonoBehaviour
{
    public RuntimeAnimatorController contrAnimacDesdeMazo0;

    Carta cartaJuego;
    GameObject mazoJuego;

    public Mesa mesa
    {
        set { DemoMesaManager.instance.mesa = value; }
        get { return DemoMesaManager.instance.mesa; }
    }

    public GameObject totem
    {
        set { DemoTotemManager.instance.totem = value; }
        get { return DemoTotemManager.instance.totem; }
    }

    public Jugador jugador;

    bool hayAnimandose = false;
    
    Vector3 posicCartasDelMazo = new Vector3(0.13f, 1.5f, -0.43f);
    Vector3 rotacCartasDelMazo = new Vector3(-90f, 0f, -175f);

    GameObjectAnimandose gameObjectAnimandose;

    public const float fDuracAnimaciones = 1.1f;
    float cuentoTres = 0f;

    UnityAction eventoListenerMazo0;
    UnityAction eventoListenerTotem;
    UnityAction eventoListenerRestablecerTotem;


    // Use this for initialization
    void Start()
    {
        string strTutorialActivado = Ini.IniReadValue("config", "tutorial-activado");
        if (strTutorialActivado == "false")
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(buildIndex + 1, LoadSceneMode.Single);
        }

        LlenarMazo();
        ReferenciarMazo();
        CrearJugador();

        eventoListenerMazo0 = new UnityAction(delegate () { PonerCarta(0); });
        EventManager.StartListening("agarrarcarta0demo", eventoListenerMazo0); //Evento que se produce cuando un jugador toca un mazo

        eventoListenerTotem = new UnityAction(TotemAgarrado);
        EventManager.StartListening("agarrartotemdemo", eventoListenerTotem); //Evento que se produce cuando un jugador agarra el totem

        eventoListenerRestablecerTotem = new UnityAction(ReiniciarTotem);
        EventManager.StartListening("restablecertotemdemo", eventoListenerRestablecerTotem); //Evento de debug para restablecer la posicion del totem
    }

    // Update is called once per frame
    void Update()
    {
        if (hayAnimandose)
        {
            verificarAnimaciones();
        }
    }

    /// <summary>
    /// Pone una Carta en cartaJuego
    /// </summary>
    private void LlenarMazo()
    {
        Carta[] mazoTotal = Resources.LoadAll<Carta>("Cartas");
        int iPosiAgarrar = ObtenerRandom(mazoTotal.Length);
        cartaJuego = mazoTotal[iPosiAgarrar];
    }

    /// <summary>
    /// Referencia el Mazo 0 en mazoJuego
    /// </summary>
    void ReferenciarMazo()
    {
        GameObject[] mazos = GameObject.FindGameObjectsWithTag("Mazo");
        Array.Sort(mazos, CompareObNames);
        mazoJuego = mazos[0];
    }

    /// <summary>
    /// Carga el jugador y le pone sus manos
    /// </summary>
    public void CrearJugador()
    {
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, CompareObNames); //Supuestamente ya estaban en orden pero x las dudas
        jugador = new Jugador();
        jugador.Manos = manos[0];
    }

    /// <summary>
    /// Pone el totem en su posicion inicial
    /// </summary>
    private void ReiniciarTotem()
    {
        DemoTotemBehaviour totemBehaviour = totem.GetComponent<DemoTotemBehaviour>();
        totemBehaviour.ReiniciarPosicion();
    }

    void PonerCarta(int iIndexMazo, bool bFlechaAfuera = false)
    {
        DemoMensajesManager.instance.PonerMensaje("Bien hecho!");
        Debug.Log("Toco el mazo");
        if (cartaJuego != null)
        {
            Crear_AnimarCarta(cartaJuego); //Crea la carta y la anima
        }
        DemoMensajesManager.instance.PonerMensaje("Ahora proba agarrar el totem!");
    }

    UnityAction eventoListenerTotemTraido;

    void TotemAgarrado()
    {
        DemoMensajesManager.instance.PonerMensaje("Traete el totem!");
        EventManager.StopListening("totemtraidodemo", eventoListenerTotemTraido);
        //DemoTotemBehaviour totemBehaviour = totem.GetComponent<DemoTotemBehaviour>();
        eventoListenerTotemTraido = new UnityAction(delegate () { TotemTraido(); });
        EventManager.StartListening("totemtraidodemo", eventoListenerTotemTraido);
    }   

    void TotemTraido()
    {
        DemoTotemManager.instance.DestruirTotem();
        DemoMensajesManager.instance.PonerMensaje("Perfecto! Ya estas listo para jugar.\nCargando...");
        StartCoroutine(contarHastaTres());
    }

    private IEnumerator contarHastaTres()
    {
        yield return new WaitForSeconds(3f);
        Destroy(jugador.Manos);
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(buildIndex + 1, LoadSceneMode.Single);
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
    public void Crear_AnimarCarta(Carta cartaCrear)
    {
        GameObject cartaCreada = crearCarta(cartaCrear);
        gameObjectAnimandose = new GameObjectAnimandose(cartaCreada, 0f, fDuracAnimaciones, TipoAnimacion.DesdeMazo);
        animarCarta(cartaCreada, contrAnimacDesdeMazo0);
    }

    /// <summary>
    /// Crea un GameObject a partir de una carta, y lo devuelve
    /// </summary>
    /// <param name="carta">La carta para crear el GameObject</param>
    public GameObject crearCarta(Carta carta)
    {
        GameObject gameObject = Instantiate(carta.img3D,
                                posicCartasDelMazo,
                                Quaternion.Euler(rotacCartasDelMazo));
        return gameObject;
    }

    /// <summary>
    /// Anima el GameObject recibido con el AnimatorController recibido
    /// </summary>
    /// <param name="gameObject">El GameObject a animar</param>
    /// <param name="controller">El AnimatorController con el que animar al GameObject</param>
    public void animarCarta(GameObject gameObject, RuntimeAnimatorController controller)
    {
        Animator animator = gameObject.GetComponent<Animator>();
        if (animator == null)
        {
            animator = gameObject.AddComponent<Animator>();
        }
        animator.runtimeAnimatorController = controller;
        animator.enabled = true;
        hayAnimandose = true;
    }

    public void verificarAnimaciones()
    {
        if (gameObjectAnimandose != null)
        {
            gameObjectAnimandose.TimerAnimacion += Time.deltaTime;
            if (gameObjectAnimandose.TimerAnimacion >= gameObjectAnimandose.DuracAnimacion)
            {
                finAnimacionDesdeMazo(gameObjectAnimandose.GameObject);
            }
        }
    }

    public void finAnimacionDesdeMazo(GameObject gameObjFinalizar)
    {
        hayAnimandose = false;
        gameObjFinalizar.GetComponent<Animator>().enabled = false; //Desactivo la animacion
        BoxCollider boxCollider = gameObjFinalizar.AddComponent<BoxCollider>(); //Creo un BoxCollider para que choque con el piso y con las cartas que caen despues
        boxCollider.center = new Vector3(0, 0, 0.065f);
        boxCollider.size = new Vector3(0.115f, 0.13f, 0.005f);
        Rigidbody rigidbody = gameObjFinalizar.AddComponent<Rigidbody>(); //Creo un RigidBody para que caiga con gravedad
        rigidbody.drag = 1f; //Para que la caida sea mas lenta
    }    

    /// <summary>
    /// Permite ordenar un vector
    /// </summary>
    public static int CompareObNames(GameObject x, GameObject y) //Ordenar por nombre
    {
        return x.name.CompareTo(y.name);
    }
}
