using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextController : MonoBehaviour {

    Text txt;
	// Use this for initialization
	void Start () {
        txt = transform.gameObject.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        txt.text = MesaManager.instance.jugadores[0].obtenerCantCartas().ToString();
	}
}
