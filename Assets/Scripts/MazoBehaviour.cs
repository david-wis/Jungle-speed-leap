using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazoBehaviour : MonoBehaviour {
    private InteractionBehaviour _intObj;

    public int iNroMazo; 

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();

    }

    void Update() {
        if (_intObj.isPrimaryHovered)
        {
            if (MesaManager.instance.mesa.Modo != ModoJuego.Fuera)
            {
                GenerarCarta();
            }
        }
    }

    /// <summary>
    /// Dispara el evento "agarrarcartaX" dependiendo del mazo tocado
    /// </summary>
    public void GenerarCarta()
    {
        EventManager.TriggerEvent("agarrarcarta" + iNroMazo);
    }

    public void PonerMazos()
    {
        switch (iNroMazo)
        {
            case 0:
                _intObj.transform.position = new Vector3();
                break;
            case 1:
                _intObj.transform.position = new Vector3();
                break;
            case 2:
                _intObj.transform.position = new Vector3();
                break;
            case 3:
                _intObj.transform.position = new Vector3();
                break;
            default:
                break;
        }
        
    }
}
