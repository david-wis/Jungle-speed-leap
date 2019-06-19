using UnityEngine;

public class SimuladorAnimaciones : MonoBehaviour
{    
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            Debug.Log("Apretaste 1");
            EventManager.TriggerEvent("agarrarcarta0");
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            Debug.Log("Apretaste 2");
            EventManager.TriggerEvent("agarrarcarta1");
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            Debug.Log("Apretaste 3");
            EventManager.TriggerEvent("agarrarcarta2");
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            Debug.Log("Apretaste 4");
            EventManager.TriggerEvent("agarrarcarta3");
        }        
    }
}
