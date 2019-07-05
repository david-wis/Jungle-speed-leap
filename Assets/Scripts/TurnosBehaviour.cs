using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnosBehaviour : MonoBehaviour {

    public Sprite[] vecEstados = new Sprite[6];
    Image imagen;

	// Use this for initialization
	void Start () {
        imagen = GetComponent<Image>();
	}

    public void cambiarImagen(int iOpcion)
    {
        imagen.sprite = vecEstados[iOpcion];
    }
}
