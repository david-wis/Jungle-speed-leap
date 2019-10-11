using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoMensajesManager : MonoBehaviour
{

    #region Singleton
    public static DemoMensajesManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public TextMeshProUGUI demoMensajes;

    public void PonerMensaje(string strMensaje)
    {
        demoMensajes.SetText(strMensaje);
    }
}