using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazoBehaviour : MonoBehaviour {
    private InteractionBehaviour _intObj;

    public int iNroMazo;
    GameObject totem;
    TotemBehaviour totemBehaviour;

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();
        totem = TotemManager.instance.totem;
        totemBehaviour = totem.GetComponent<TotemBehaviour>();
       
        PonerMazos();
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
                _intObj.transform.position = totem.transform.position + new Vector3(0.15f, 0.035f, -0.25f);
                _intObj.transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case 1:
                _intObj.transform.position = totem.transform.position + new Vector3(0.25f, 0.035f, 0.15f);
                _intObj.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case 2:
                _intObj.transform.position = totem.transform.position + new Vector3(-0.15f, 0.035f, 0.25f);
                _intObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case 3:
                _intObj.transform.position = totem.transform.position + new Vector3(-0.25f, 0.035f, -0.15f);
                _intObj.transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
            default:
                break;
        }
        
    }
}
