using UnityEngine;


/// <summary>
/// Con esta clase se puede jugar sin tener el leap motion, reemplazando su funcionalidad por comandos
/// </summary>
public class DemoSimuladorAnimaciones : MonoBehaviour
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
                EventManager.TriggerEvent("agarrarcarta0demo");
                if (mensajeInicial != null)
                {
                    //Destroy(mensajeInicial);
                    //mensajeInicial = null;
                }
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.R))
            {
                EventManager.TriggerEvent("restablecertotemdemo");
                fTimer = 0f;
            }
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

    }
}
