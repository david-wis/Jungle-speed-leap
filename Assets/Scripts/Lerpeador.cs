using UnityEngine;

public class Lerpeador
{
    public bool bTermino;
    float lerpTime;
    float currentLerpTime;

    Vector3 startPos;
    Vector3 endPos;
    Quaternion startRot;
    Quaternion endRot;
    GameObject gameObject;
    bool bMovimiento;

    public Lerpeador(float fTiempo)
    {
        lerpTime = fTiempo;
        bTermino = true;
    }

    public void Start(GameObject gameObject, Vector3 posFinal)
    {
        this.gameObject = gameObject;
        startPos = gameObject.transform.position;
        endPos = posFinal;
        bMovimiento = true;
        bTermino = false;
    }

    public void Start(GameObject gameObject, Quaternion rotFinal)
    {
        this.gameObject = gameObject;
        startRot = gameObject.transform.rotation;
        endRot = rotFinal;
        bMovimiento = false;
    }

    public bool Update()
    {
        //increment timer once per frame
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
            bTermino = true;
        }
        //lerp!
        float perc = currentLerpTime / lerpTime;
        if (bMovimiento)
        {
            gameObject.transform.position = Vector3.Lerp(startPos, endPos, perc);
        }
        else
        {
            gameObject.transform.rotation = Quaternion.Lerp(startRot, endRot, perc);
            Debug.Log(gameObject.transform.rotation);
        }
        return bTermino;
    }
}
