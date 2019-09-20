using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject reglas;
    public GameObject mano;

    public void PlayGame()
    {
        Destroy(mano);
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(buildIndex + 1, LoadSceneMode.Single);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(buildIndex+1));
    }	

    public void OpenReglas()
    {
        reglas.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Exit()
    {
        Debug.Log("aaaa");
        Application.Quit();
    }
}
