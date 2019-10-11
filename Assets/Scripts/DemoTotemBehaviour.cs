using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DemoTotemBehaviour : MonoBehaviour
{
    InteractionBehaviour _intObj; //Permite obtener el isgrasped (puede servir para multiplayer)
    private bool bAgarrado; //Si algun jugador lo tiene en la mano
    private string sManos;
    private int iIndexApretado = -2;
    /* 0 -> id del jugador / -1 -> no lo tiene nadie / -2 -> init 
     * Evitar que la funcion agarrar totem se llame cada frame que el jugador tenga el totem
     */
    private int iIndexViejo = -1;
    public Vector3 posicionInicial;
    public Quaternion rotacionInicial;
    
    private float diferenciaPosiciones = 0;

    // Use this for initialization
    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
        _intObj.ignoreGrasping = true;
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        bAgarrado = false;
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        sManos = manos[0].name;
    }

    Vector3 PosAnterior;

    void Update()
    {
        //Debug.Log(bAgarrado);
        if (bAgarrado)
        {
            if (iIndexViejo != iIndexApretado)
            {
                if (DemoMazoManager.instance.yaToco())
                {
                    Debug.Log("------Evento producido agarrartotemdemo");
                    EventManager.TriggerEvent("agarrartotemdemo");
                    iIndexViejo = iIndexApretado;
                }
                else
                {
                    DemoMensajesManager.instance.PonerMensaje("Primero tenes que tocar el mazo");
                }
            }

            diferenciaPosiciones = posicionInicial.z - transform.position.z;
            Debug.Log(diferenciaPosiciones);
            if (diferenciaPosiciones >= 0.2)
            {
                //Debug.Log("Totem robado por el jugador 1");
                EventManager.TriggerEvent("totemtraidodemo");
                ReiniciarPosicion();
            }
        }
    }

    /// <summary>
    /// Establece que el totem fue agarrado con exito
    /// </summary>
    public void SetAgarrado()
    {
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
        transform.position = posicionInicial + Vector3.up * 0.1f;
        _intObj.enabled = true;
        bAgarrado = false;
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
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(sNomMano + " (index: " + iIndexApretadoTemp + ") agarro el totem");
        bAgarrado = true;
    }

    private void OnTriggerExit(Collider other)
    {
        bAgarrado = false;
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
    /// Determina si el totem esta siendo agarrado
    /// </summary>
    /// <returns>Si esta siendo agarrado</returns>
    public bool estaAgarrado()
    {
        return bAgarrado;
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

    public void PermitirGrasp()
    {
        _intObj.ignoreGrasping = false;
    }

    public void Destruirme()
    {
        Destroy(this.gameObject);
    }
}
