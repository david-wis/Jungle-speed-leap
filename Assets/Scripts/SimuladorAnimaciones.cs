using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Con esta clase se puede jugar sin tener el leap motion, reemplazando su funcionalidad por comandos
/// </summary>
public class SimuladorAnimaciones : MonoBehaviour
{
    float fTimer;
	public GameObject mensajeInicial;
    // Use this for initialization
    void Start()
    {
        fTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;
        if (fTimer >= 1f) //Para que cada apretada sea con 1s de diferencia minimo
        {
            if (Input.GetKey(KeyCode.Alpha1))
            {
                EventManager.TriggerEvent("agarrarcarta0");
                if (mensajeInicial != null)
                {
                    Destroy(mensajeInicial);
                    mensajeInicial = null;
                }
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Alpha2))
            {
                EventManager.TriggerEvent("agarrarcarta1");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Alpha3))
            {
                EventManager.TriggerEvent("agarrarcarta2");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Alpha4))
            {
                EventManager.TriggerEvent("agarrarcarta3");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Alpha5))
            {
                EventManager.TriggerEvent("forzarAgarrado");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Alpha6))
            {
                EventManager.TriggerEvent("forzarAgarradoCorrecto");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Alpha7))
            {
                EventManager.TriggerEvent("terminarForzarAgarrado");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.R))
            {
                EventManager.TriggerEvent("restablecertotem");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (Input.GetKey(KeyCode.F5))
            {
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadSceneAsync(buildIndex - 1, LoadSceneMode.Single);
            }
            if (Input.GetKey(KeyCode.F6))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        
    }
}
