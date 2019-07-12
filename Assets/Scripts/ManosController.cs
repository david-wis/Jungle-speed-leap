using System;
using UnityEngine;

public class ManosController : MonoBehaviour {

    public int iIndexJug;
    GameObject totem;
    GameObject manoIzq, manoDer;
    TotemBehaviour totemBehaviour;
    Vector3[] posicionInicial = new Vector3[2]; //Posicion inicial de las manitos
    Lerpeador lerpMov, lerpRot;
    int iEstado; // 0 empieza a rotar -  1 en rotacion - 2 empieza a moverse - 3 en movimiento - 4 movido
    

    // Use this for initialization
    void Start()
    {
        lerpMov = new Lerpeador(0.5f);
        lerpRot = new Lerpeador(0.5f);

        manoIzq = transform.GetChild(0).gameObject;
        manoDer = transform.GetChild(1).gameObject;
        totem = TotemManager.instance.totem;
        totemBehaviour = totem.GetComponent<TotemBehaviour>();
        PonerManos();
        iEstado = 0;
        //posicionInicial[0] = manoIzq.transform;
        //posicionInicial[1] = manoDer.transform;
    }

    // Update is called once per frame

    void Update()
    {
        if (MesaManager.instance.iIndexJugActual == iIndexJug)
        {
            switch (iEstado)
            {
                case 0:
                    lerpRot.Start(manoDer, ObtenerRotacion());
                    iEstado++;
                    Debug.Log("Estado 0 terminado");
                    break;
                case 1:
                    if (lerpRot.Update())
                    {
                        iEstado++;
                        Debug.Log("Estado 1 terminado");
                    }
                    break;
                case 2:
                    lerpMov.Start(manoDer, totem.transform.position);
                    iEstado++;
                    Debug.Log("Estado 2 terminado");
                    break;
                case 3:
                    if (lerpMov.Update())
                    {
                        iEstado++;
                        Debug.Log("Estado 3 terminado");
                    }
                    break;
                case 4:
                    //Animacion
                    iEstado = 0;
                    Debug.Log("Estado 4 terminado");
                    break;
            }
        }
    }

    Quaternion ObtenerRotacion()
    {
        Quaternion rotacion = Quaternion.Euler(0, 0, 0);
        if (iIndexJug == 1)
        {
            rotacion = Quaternion.Euler(0, 0, 0);
        } else
        {
            rotacion = Quaternion.Euler(0, 90, 0);
        } 
        return rotacion;
    }

    void PonerManos()
    {
        switch (iIndexJug)
        {
            case 1:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, 0.09f);
                break;
            case 2:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.09f, 0.1f, 0.35f);
                manoDer.transform.position = totem.transform.position + new Vector3(-0.09f, 0.1f, 0.35f);
                break;
            case 3:
                manoIzq.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, 0.09f);
                break;
            default:
                Debug.Log("Esto no deberia pasar xdxd");
                break;
        }
    }
}
