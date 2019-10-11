using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMazoBehaviour : MonoBehaviour
{
    private InteractionBehaviour _intObj;
    private Material _material;
    [Header("InteractionBehaviour Colors")]
    public Color defaultColor = Color.Lerp(Color.black, Color.white, 0.1F);
    public Color primaryHoverColor = Color.Lerp(Color.black, Color.white, 0.8F);

    public Mesa mesa
    {
        set { DemoMesaManager.instance.mesa = value; }
        get { return DemoMesaManager.instance.mesa; }
    }

    GameObject totem;
    bool yaToco = false;

    // Use this for initialization
    void Start()
    {
        Debug.Log("En el Start del Mazo");
        _intObj = GetComponent<InteractionBehaviour>();
        totem = DemoTotemManager.instance.totem;
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            renderer = GetComponentInChildren<Renderer>();
        }
        if (renderer != null)
        {
            _material = renderer.material;
        }
        PonerMazo();
    }

    void Update()
    {
        if (mesa.Terminada)
            Destroy(this); //Eventually, the mazo stopped thinking
        if (_intObj.isPrimaryHovered)
        {
            if (!yaToco)
            {
                GenerarCarta();
                yaToco = true;
            }
        }
    }

    /// <summary>
    /// Dispara el evento "agarrarcartaX" dependiendo del mazo tocado
    /// </summary>
    public void GenerarCarta()
    {
        EventManager.TriggerEvent("agarrarcarta0demo");
    }

    public void PonerMazo()
    {
        Debug.Log("Poniendo el mazo");
        _intObj.transform.position = totem.transform.position + new Vector3(0.1f, 0.023f, -0.25f);
        _intObj.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}
