using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMesaManager : MonoBehaviour
{

    //Usando un singleton vamos a poder acceder al indice de jugador actual desde cualquier lado
    #region Singleton
    public static DemoMesaManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    /// <summary>
    /// Objeto de tipo Mesa sobre el que se juega la partida 
    /// </summary>
    public Mesa mesa = new Mesa();
}
