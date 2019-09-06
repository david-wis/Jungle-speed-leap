using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotemManager : MonoBehaviour
{

    //Usando un singleton vamos a poder acceder al totem desde cualquier lado
    #region Singleton
    public static TotemManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject totem;
}
