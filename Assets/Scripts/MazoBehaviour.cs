using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazoBehaviour : MonoBehaviour {
    private InteractionBehaviour _intObj;
    public int iNroMazo; 

    //[Tooltip("If enabled, the object will use its primaryHoverColor when the primary hover of an InteractionHand.")]
    //public bool usePrimaryHover = false;

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();
    }

    void Update() {
        //Debug.Log("isPrimaryHovered: " + _intObj.isPrimaryHovered);
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
}
