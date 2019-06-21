using Leap.Unity.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemBehaviour : MonoBehaviour {
    InteractionBehaviour _intObj;
    private bool bForceGrasp;
    //private GameObject[] manos;
    private string[] sManos = new string[4];
    public static int iIndexApretado = -1;

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();
        bForceGrasp = false;
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, Main.CompareObNames);
        for (int i = 0; i < 4; i++)
        {
            sManos[i] = manos[i].name;
        }      
    }
	
	// Update is called once per frame
	void Update () {
        if (_intObj.isGrasped /*|| bForceGrasp*/)
        {
            /*
             * Si armamos el multiplayer aca tendriamos que revisar quien agarro el totem
             * Para eso deberiamos usar los atributos de graspingcontroller/hands
             */
            EventManager.TriggerEvent("agarrartotem");
            //bForceGrasp = false;            
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
    }
}
