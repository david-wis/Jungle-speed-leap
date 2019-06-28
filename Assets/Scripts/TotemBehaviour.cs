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
    private static int iIndexApretado = -1;
    /* 0-4 -> id del jugador / -1 -> no lo tiene nadie / -2 -> init 
     * Evitar que la funcion agarrar totem se llame cada frame que el jugador tenga el totem
     */
    private int iIndexViejo = -2;
    private bool bAgarradoCorrecto = false;
    UnityAction eventoListenerAgarrado;

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();
        //bAgarrado = false;
        bAgarrado = true;
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, Main.CompareObNames);
        for (int i = 0; i < 4; i++)
        {
            sManos[i] = manos[i].name;
        }
        eventoListenerAgarrado = new UnityAction(SetAgarradoCorrecto);
        EventManager.StartListening("totemagarrado", eventoListenerAgarrado);
    }


    Vector3 PosAnterior;
	void Update ()
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
                //Debug.Log(transform.position);
                switch (iIndexApretado)
                {
                    case 0: //Caso del jugador, es el unico que probe asi que despues fijense xd
                        if (/*PosAnterior.z - transform.position.z >= 0.15*/ transform.position.z <= -0.3)
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

    void SetAgarradoCorrecto()
    {
        bAgarradoCorrecto = true;
        PosAnterior = transform.position;
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

    public static int ObtenerJugador()
    {
        return iIndexApretado;
    }
}
