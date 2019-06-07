using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carta : MonoBehaviour {
    private int iDCarta = -1;
    public GameObject prefab;
    private string Color;
    private string Forma;

    public Carta()
    {
    }

    public Carta(int id)
    {
        iDCarta = id;
    }

    public int IDCarta
    {
        get
        {
            return IDCarta;
        }

        set
        {
            IDCarta = value;
        }
    }

    /*public Carta(float x, float y, float z)
    {
        //Instantiate(objCarta, new Vector3(x, y, z), Quaternion.identity);
        objCarta = GameObject.CreatePrimitive(PrimitiveType.Cube);
        objCarta.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        objCarta.transform.position = new Vector3(x, y, z);
        objCarta.AddComponent<Rigidbody>();
        objCarta.GetComponentInChildren<Renderer>().material.color = Color.red; //Cambiarlo por textura cuando Abril termine
    }*/
}
