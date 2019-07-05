using UnityEngine;

public class SimuladorAnimaciones : MonoBehaviour
{
    float fTimer;
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
                EventManager.TriggerEvent("terminarForzarAgarrado");
                fTimer = 0f;
            }

            if (Input.GetKey(KeyCode.R))
            {
                EventManager.TriggerEvent("restablecertotem");
                fTimer = 0f;
            }
        }
        
    }
}
