using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemBehaviour : MonoBehaviour {
    InteractionBehaviour _intObj;
    private bool bForceGrasp;
    //private GameObject[] manos;
    private Collider colisionador;
    private Collider[] colManos = new Collider[4];

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();
        bForceGrasp = false;
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, Main.CompareObNames);
        for (int i = 0; i < 4; i++)
        {
            colManos[i] = manos[i].GetComponent<Collider>(); 
        }
        colisionador = GetComponent<Collider>();        
    }
	
	// Update is called once per frame
	void Update () {
        if (_intObj.isGrasped || bForceGrasp)
        {
            /*
             * Si armamos el multiplayer aca tendriamos que revisar quien agarro el totem
             * Para eso deberiamos usar los atributos de graspingcontroller/hands
             */
            EventManager.TriggerEvent("agarrartotem");
            bForceGrasp = false;            
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
    }
}
