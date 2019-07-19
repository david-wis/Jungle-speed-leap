using System;
using System.Collections.Generic;
using UnityEngine;

public class ManosController : MonoBehaviour {

    public int iIndexJug;
    GameObject totem;
    GameObject manoIzq, manoDer;
    TotemBehaviour totemBehaviour;
    Vector3[] posicionInicial = new Vector3[2]; //Posicion inicial de las manitos
    Lerpeador lerpMov;
    //Lerpeador lerpRot;
    int iEstado; //0 empieza a moverse - 1 en movimiento - 2 movido
    Vector3 corrimiento;
    

    // Use this for initialization
    void Start()
    {
        lerpMov = new Lerpeador(0.5f);
        //lerpRot = new Lerpeador(0.5f);

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
            //TODO: Animacion tocar mazo
        }

        bool bAgarrarPosible = MesaManager.instance.mesa.TieneIgualdadConResto(iIndexJug);
        Debug.Log("Jugador " + iIndexJug + " " + bAgarrarPosible);
        if (bAgarrarPosible)
        {
            switch (iEstado)
            {
                case 0:
                    lerpMov.Start(manoDer, totem.transform.position + corrimiento);
                    iEstado++;
                    Debug.Log("Estado 2 terminado");
                    break;
                case 1:
                    if (lerpMov.Update())
                    {
                        iEstado++;
                        Debug.Log("Estado 3 terminado");
                    }
                    break;
                case 2:
                    //Animacion
                    iEstado = 0;
                    Debug.Log("Estado 4 terminado");
                    break;
            }
        }
    }

    /*Quaternion ObtenerRotacion()
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
    }*/

    void PonerManos()
    {
        float fDistancia = 0.08f; //Espacio contra el totem
        float fCorrimientoMano = 0.05f; //Evitar atravesar el totem
        float fDistY = 0.08f;
        switch (iIndexJug)
        {
            case 1:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, 0.09f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 0, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, 180, -90);
                corrimiento = new Vector3(fDistancia, fDistY, fCorrimientoMano);
                break;
            case 2:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.09f, 0.1f, 0.35f);
                manoDer.transform.position = totem.transform.position + new Vector3(-0.09f, 0.1f, 0.35f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 270, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, -270, -90);
                corrimiento = new Vector3(-fCorrimientoMano, fDistY, fDistancia);
                break;
            case 3:
                manoIzq.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, 0.09f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 180, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, 0, -90);
                corrimiento = new Vector3(-fDistancia, fDistY, -fCorrimientoMano);
                break;
            default:
                Debug.Log("Esto no deberia pasar xdxd");
                break;
        }
    }
}
