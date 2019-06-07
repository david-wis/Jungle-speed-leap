using UnityEngine;

public class Mazo : MonoBehaviour {
    public GameObject objMazo;
    const float scaleX = 0.18f, scaleY = 0.05f, scaleZ = 0.15f;

    public Mazo(float x, float y, float z)
    {
        Instantiate(objMazo, new Vector3(x, y, z), Quaternion.identity);
        //objMazo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        objMazo.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        //objMazo.transform.position = new Vector3(x, y, z);
        objMazo.AddComponent<Rigidbody>();
        objMazo.GetComponentInChildren<Renderer>().material.color = Color.green; //Cambiarlo por textura cuando Abril termine
    }
}

