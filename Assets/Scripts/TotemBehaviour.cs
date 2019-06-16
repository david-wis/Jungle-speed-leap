using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemBehaviour : MonoBehaviour {
    InteractionBehaviour _intObj;
    private bool bForceGrasp;

    // Use this for initialization
    void Start () {
        _intObj = GetComponent<InteractionBehaviour>();
        bForceGrasp = false;
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
}
