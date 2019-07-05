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

    UnityAction forzarAgarradoListener, forzarAgarradoCorrectoListener, terminarForzarAgarradoListener;

    // Use this for initialization
    void Start () {
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
        //Debug.Log(transform.position);
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
                //Debug.Log(transform.position);
                switch (iIndexApretado)
                {
                    case 0: //Caso del jugador, es el unico que probe asi que despues fijense xd
                        if (transform.position.z <= -0.35)
                        {
                            Debug.Log("Totem robado por el jugador 1");
                            bAgarradoCorrecto = false;
                            EventManager.TriggerEvent("totemtraido");       
                        }
                        break;
                    case 1:
                        if (PosAnterior.x - transform.position.x >= 0.2)
                        {
                            Debug.Log("Totem robado por el jugador 2");
                            bAgarradoCorrecto = false;
                        }
                        break;
                    case 2:
                        if (transform.position.z - PosAnterior.z  >= 0.2)
                        {
                            Debug.Log("Totem robado por el jugador 3");
                            bAgarradoCorrecto = false;
                        }
                        break;
                    case 3:
                        if (transform.position.x - PosAnterior.x >= 0.2)
                        {
                            Debug.Log("Totem robado por el jugador 4");
                            bAgarradoCorrecto = false;
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
        transform.rotation = rotacionInicial;
        transform.position = posicionInicial;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        _intObj.enabled = true;
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

    private void forzarAgarradoCorrecto()
    {
        SetAgarradoCorrecto();
        EventManager.TriggerEvent("totemtraido");
        bAgarradoCorrecto = false;
        Debug.Log("Evento de agarrado correcto disparado, recorda apretar 7");
    }

    private void terminarForzarAgarrado()
    {
        if (bAgarrado)
        {
            bAgarrado = false;
            iIndexApretado = -1; //No lo agarra nadie
            iIndexViejo = -1; //Se reinicia el registro de agarradas
        }
    }
}
