using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TotemBehaviour : MonoBehaviour {
    InteractionBehaviour _intObj; //Permite obtener el isgrasped (puede servir para multiplayer)
    private bool bAgarrado; //Si algun jugador lo tiene en la mano
    private string[] sManos = new string[4];
    private int iIndexApretado = -2;
    /* 0-4 -> id del jugador / -1 -> no lo tiene nadie / -2 -> init 
     * Evitar que la funcion agarrar totem se llame cada frame que el jugador tenga el totem
     */
    private int iIndexViejo = -1;
    private bool bAgarradoCorrecto = false;
    public Vector3 posicionInicial;
    public Quaternion rotacionInicial;

    private List<GameObject> _gameObjsEnTotem = new List<GameObject>();
    private List<Carta> _cartasEnTotem = new List<Carta>();

    UnityAction forzarAgarradoListener, forzarAgarradoCorrectoListener, terminarForzarAgarradoListener;

    // Use this for initialization
    void Start() {
        _intObj = GetComponent<InteractionBehaviour>();
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        bAgarrado = false;
        //bAgarrado = true;
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, Main.CompareObNames);
        for (int i = 0; i < 4; i++)
        {
            sManos[i] = manos[i].name;
        }
        forzarAgarradoListener = new UnityAction(forzarAgarrado);
        EventManager.StartListening("forzarAgarrado", forzarAgarradoListener);
        terminarForzarAgarradoListener = new UnityAction(terminarForzarAgarrado);
        EventManager.StartListening("terminarForzarAgarrado", terminarForzarAgarradoListener);
        forzarAgarradoCorrectoListener = new UnityAction(forzarAgarradoCorrecto);
        EventManager.StartListening("forzarAgarradoCorrecto", forzarAgarradoCorrectoListener);
    }


    Vector3 PosAnterior;

    void Update()
    {
        if (bAgarrado)
        {
            //Debug.Log(iIndexViejo + " - " + iIndexApretado);
            if (iIndexViejo != iIndexApretado)
            {
                Debug.Log("evento producido agarrartotem");
                EventManager.TriggerEvent("agarrartotem");
                iIndexViejo = iIndexApretado;
            }

            if (bAgarradoCorrecto)
            {
                switch (iIndexApretado)
                {
                    case 0: //Caso del jugador, es el unico que probe asi que despues fijense xd
                        if (posicionInicial.z - transform.position.z >= 0.2)
                        {
                            Debug.Log("Totem robado por el jugador 1");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    case 1:
                        if (PosAnterior.x - transform.position.x <= -0.2)
                        {
                            Debug.Log("Totem robado por el jugador 2");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    case 2:
                        if (transform.position.z - PosAnterior.z >= 0.2)
                        {
                            Debug.Log("Totem robado por el jugador 3");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    case 3:
                        if (transform.position.x - PosAnterior.x <= -0.2)
                        {
                            Debug.Log("Totem robado por el jugador 4");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Establece que el totem fue agarrado con exito
    /// </summary>
    public void SetAgarradoCorrecto()
    {
        bAgarradoCorrecto = true;
        PosAnterior = transform.position;
    }

    /// <summary>
    /// Pone el totem en su posicion original
    /// </summary>
    public void ReiniciarPosicion()
    {
        _intObj.enabled = false;
        transform.parent = null;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = rotacionInicial;
        transform.position = posicionInicial + Vector3.up*0.1f;
        _intObj.enabled = true;
        bAgarrado = false;
        bAgarradoCorrecto = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "floor") //dato: tuve que meterle un box collider xq no funciona con el mesh
        {
            ReiniciarPosicion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string sNomMano = other.gameObject.transform.parent.transform.parent.name;
        Debug.Log(sNomMano);
        int i = 0;
        iIndexApretado = -1;
        while (i < 4 && iIndexApretado == -1) {
            if (sNomMano == sManos[i])
            {
                iIndexApretado = i;
            }
            i++;
        }

        if (iIndexApretado != -1)
        {
            bAgarrado = true;
            Debug.Log("agarrado");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (bAgarrado)
        {
            bAgarrado = false;
            iIndexApretado = -1; //No lo agarra nadie
            iIndexViejo = -1; //Se reinicia el registro de agarradas
        }
    }

    /// <summary>
    /// Retorna el indice de jugador actual
    /// </summary>
    /// <returns>Indice del jugador actual</returns>
    public int ObtenerJugador()
    {
        return iIndexApretado;
    }

    /// <summary>
    /// Permite agarrar el totem a través de un evento
    /// </summary>
    private void forzarAgarrado()
    {
        iIndexApretado = 0;
        bAgarrado = true;
        if (iIndexViejo != iIndexApretado)
        {
            Debug.Log("Evento de batalla disparado, recorda apretar 6 para un agarrado correcto y 7 para soltar");
            EventManager.TriggerEvent("agarrartotem");
            iIndexViejo = iIndexApretado;
        }
    }

    /// <summary>
    /// Permite traer el totem de manera correcta a través de un evento
    /// </summary>
    private void forzarAgarradoCorrecto()
    {
        SetAgarradoCorrecto();
        EventManager.TriggerEvent("totemtraido");
        bAgarradoCorrecto = false;
        Debug.Log("Evento de agarrado correcto disparado, recorda apretar 7");
    }

    /// <summary>
    /// Permite soltar el totem a través de un evento
    /// </summary>
    private void terminarForzarAgarrado()
    {
        if (bAgarrado)
        {
            bAgarrado = false;
            iIndexApretado = -1; //No lo agarra nadie
            iIndexViejo = -1; //Se reinicia el registro de agarradas
        }
    }

    /// <summary>
    /// Determina si el totem esta siendo agarrado
    /// </summary>
    /// <returns>Si esta siendo agarrado</returns>
    public bool estaAgarrado()
    {
        return bAgarrado;
    }

    /// <summary>
    /// Agrega una Carta a la lista de Cartas en el Totem
    /// </summary>
    /// <param name="carta">La Carta a agregar a la lista</param>
    public void agregarCartaALista(Carta carta)
    {
        _cartasEnTotem.Add(carta);
    }

    /// <summary>
    /// Agrega un GameObject a la lista de GameObjects en el Totem
    /// </summary>
    /// <param name="gameObject">El GameObject a agregar a la lista</param>
    public void agregarGameObjALista(GameObject gameObject)
    {
        _gameObjsEnTotem.Add(gameObject);
    }

    /// <summary>
    /// Vacia la lista gameObjsEnTotem y la devuelve
    /// </summary>
    /// <returns></returns>
    public List<GameObject> obtener_VaciarGameObjectsEnTotem()
    {
        List<GameObject> gameObjects = _gameObjsEnTotem;
        vaciarGameObjectsEnTotem();
        return gameObjects;
    }

    /// <summary>
    /// Getter de la lista gameObjsEnTotem
    /// </summary>
    public List<GameObject> GameObjsEnTotem
    {
        get
        {
            return _gameObjsEnTotem;
        }
    }

    /// <summary>
    /// Vacia la lista gameObjsEnTotem
    /// </summary>
    public void vaciarGameObjectsEnTotem()
    {
        _gameObjsEnTotem.Clear();
    }

    /// <summary>
    /// Vacia la lista cartasEnTotem y la devuelve
    /// </summary>
    /// <returns></returns>
    public List<Carta> obtener_VaciarCartasEnTotem()
    {
        List<Carta> cartas = _cartasEnTotem;
        vaciarCartasEnTotem();
        return cartas;
    }

    /// <summary>
    /// Getter de la lista cartasEnTotem
    /// </summary>
    public List<Carta> CartasEnTotem
    {
        get
        {
            return _cartasEnTotem;
        }
    }

    /// <summary>
    /// Vacia la lista cartasEnTotem
    /// </summary>
    public void vaciarCartasEnTotem()
    {
        _cartasEnTotem.Clear();
    }

    Transform mano = null;
    public void fijarTotemEnMano(Transform mano)
    {
        this.mano = mano;
        transform.parent = mano;
        GetComponent<Rigidbody>().useGravity = false;
        //transform.SetParent(mano.transform);
    }
}
