﻿using Leap.Unity.Interaction;
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
        //gameObject.layer = 8; //Le asigna el layer "Totem"
    }

    Vector3 PosAnterior;

    void Update()
    {
        if (bAgarrado)
        {
            //Debug.Log("IndexViejo: " + iIndexViejo + " -- IndexNuevo:" + iIndexApretado);
            if (iIndexViejo != iIndexApretado)
            {
                Debug.Log("------Evento producido agarrartotem");
                EventManager.TriggerEvent("agarrartotem");
                iIndexViejo = iIndexApretado;
            }

            if (bAgarradoCorrecto)
            {
                //Debug.Log("El totem se encuentra agarrado por el Jugador " + (iIndexApretado + 1));
                switch (iIndexApretado)
                {
                    case 0:
                        //Debug.Log("Diferencia en Z: " + (posicionInicial.z - transform.position.z));
                        if (posicionInicial.z - transform.position.z >= 0.18)
                        {
                            //Debug.Log("Totem robado por el jugador 1");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    case 1:
                        if (PosAnterior.x - transform.position.x <= -0.18)
                        {
                            //Debug.Log("Totem robado por el jugador 2");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    case 2:
                        if (transform.position.z - PosAnterior.z >= 0.18)
                        {
                            //Debug.Log("Totem robado por el jugador 3");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");
                        }
                        break;
                    case 3:
                        if (transform.position.x - PosAnterior.x <= -0.18)
                        {
                            //Debug.Log("Totem robado por el jugador 4");
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
        _intObj.enabled = true;
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
        //Experimental, no se si cambian algo estas dos ultimas lineas
        iIndexApretado = -1;
        iIndexViejo = -1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Totem colisiono con " + collision.gameObject.name);

        if (collision.collider.name == "floor") //dato: tuve que meterle un box collider xq no funciona con el mesh
        {
            ReiniciarPosicion();
        }
        else if (collision.gameObject.CompareTag("GameObjCarta"))
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>()); //No funciona
        } else if (collision.gameObject.name == "Platte")
        {
            if (bAgarradoCorrecto)
            {
                Debug.Log("Se cayó el totem!!!!");
                EventManager.TriggerEvent("totemfallido");
                //bAgarradoCorrecto = false;
                ReiniciarPosicion();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string sNomMano = other.gameObject.transform.parent.transform.parent.name;
        int i = 0;
        int iIndexApretadoTemp; //Armo una variable auxiliar asi si no es una mano lo que triggereo el totem no se pierde el registro
        iIndexApretadoTemp = -1;
        while (i < 4 && iIndexApretadoTemp == -1) {
            if (sNomMano == sManos[i])
            {
                iIndexApretadoTemp = i;
            }
            i++;
        }

        //Debug.Log(sNomMano + " (index: " + iIndexApretadoTemp + ") agarro el totem");

        if (iIndexApretadoTemp != -1)
        {
            if (iIndexApretadoTemp != 0) { //Si intenta agarrar un bot, impedimos que el jugador pueda agarrar el totem 
                _intObj.enabled = false;
                bAgarrado = true;
                iIndexApretado = iIndexApretadoTemp;
            } else
            {
                if (_intObj.enabled) //El jugador uno solo va a cambiar el index si puede hacerlo
                {
                    bAgarrado = true;
                    iIndexApretado = iIndexApretadoTemp;
                }
            }
            //Debug.Log("agarrado");
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
            //Debug.Log("Evento de batalla disparado, recorda apretar 6 para un agarrado correcto y 7 para soltar");
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
        //Debug.Log("Evento de agarrado correcto disparado, recorda apretar 7");
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
        List<GameObject> gameObjects = new List<GameObject>(_gameObjsEnTotem);
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
        List<Carta> cartas = new List<Carta>(_cartasEnTotem);
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
    /// <summary>
    /// Setea como gameobject padre a la mano que agarró el totem
    /// </summary>
    /// <param name="mano">Mano que agarró el totem</param>
    public void fijarTotemEnMano(Transform mano)
    {
        this.mano = mano;
        transform.parent = mano;
        GetComponent<Rigidbody>().useGravity = false;
        //transform.SetParent(mano.transform);
    }
}
