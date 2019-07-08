using UnityEngine;

public class ManosController : MonoBehaviour {

    public int iIndexJug;
    public float fSpeed = 1.0f;
    GameObject totem;
    GameObject manoIzq, manoDer;
    TotemBehaviour totemBehaviour;
    Transform[] posicionInicial = new Transform[2]; //Posicion inicial de las manitos
    

    // Use this for initialization
    void Start()
    {
        manoIzq = transform.GetChild(0).gameObject;
        manoDer = transform.GetChild(1).gameObject;
        totem = TotemManager.instance.totem;
        totemBehaviour = totem.GetComponent<TotemBehaviour>();
        posicionInicial[0] = manoIzq.transform;
        posicionInicial[1] = manoDer.transform;
    }

    // Update is called once per frame

    void Update()
    {
        if (MesaManager.instance.iIndexJugActual == iIndexJug)
        {
            //Debug.Log("Soy un bot consciente! Y mi indice es " + iIndexJug);
            //TODO: Hacer que el bot acerque bien las manos al totem
            /*float fStep = fSpeed * Time.deltaTime;
            if (!totemBehaviour.estaAgarrado()) { //Si nadie lo esta agarrando
                manoDer.transform.position = Vector3.MoveTowards(manoDer.transform.position, totem.transform.position, fStep);
            } else //Si ya lo tengo o si otro jugador lo agarró vuelvo
            {
                manoDer.transform.position = Vector3.MoveTowards(manoDer.transform.position, posicionInicial[1].position, fStep);
            }*/
        }

    }
}
