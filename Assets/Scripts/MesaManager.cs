using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesaManager : MonoBehaviour {

    //Usando un singleton vamos a poder acceder al indice de jugador actual desde cualquier lado
    #region Singleton
    public static MesaManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public int iIndexJugActual;
    public Mesa mesa = new Mesa();
}
