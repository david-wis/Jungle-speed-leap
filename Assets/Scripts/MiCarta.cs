using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiCarta : MonoBehaviour {

    public GameObject prefab = null;
	// Use this for initialization
	void Start () {
        Instantiate(prefab,
            transform.position,
            Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
