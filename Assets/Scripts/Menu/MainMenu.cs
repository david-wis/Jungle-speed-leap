using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject reglas;
    public GameObject mano;

    public void Start()
    {
        Debug.Log("holis");
        string strMenuActivado = Ini.IniReadValue("config", "menu-activado");
        if (strMenuActivado == "false")
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadSceneAsync(buildIndex + 1, LoadSceneMode.Single);
        }
    }

    public void PlayGame()
    {
        string strTutorialActivado = Ini.IniReadValue("config", "tutorial-activado");
        Destroy(mano);
        int intSiguiente = (strTutorialActivado == "true") ? 1 : 2;
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(buildIndex + intSiguiente, LoadSceneMode.Single);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex+1));
    }	

    public void OpenReglas()
    {
        reglas.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
