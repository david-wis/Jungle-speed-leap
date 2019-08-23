using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap.Unity.Interaction;

public class RestartBehaviour : MonoBehaviour {

    private InteractionBehaviour _intObj;

    private void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
    }

    private void Update()
    {
        if (_intObj.isPrimaryHovered)
        {
            RestartGame();
        }
    }


    /// <summary>
    /// Reinicia la GameScene
    /// </summary>
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }    
}