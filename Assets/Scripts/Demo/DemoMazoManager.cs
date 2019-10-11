using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMazoManager : MonoBehaviour {

    #region Singleton
    public static DemoMazoManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public DemoMazoBehaviour demoMazoBehaviour;
    public bool yaToco()
    {
        return demoMazoBehaviour.yaToco;
    }
}
