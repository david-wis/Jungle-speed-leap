using UnityEngine;

/// <summary>
/// <para>A pesar de tener un nombre confuso, no temais, puesto que la clase Lerpeador lo unico que hace es mover o rotar un objeto de un estado A a uno B.</para>
/// <para>Para usarla se debe pasar como parametro al constructor el tiempo deseado; llamar al metodo start con la posicion final e inicial; y llamar a update() una vez por frame</para>
/// </summary>
public class Lerpeador
{
    public bool bTermino; //Determina si el lerpeador no se esta moviendo
    public bool bActivado; //Determina si el lerpeador ya empezo
    float lerpTime;
    float currentLerpTime;

    Vector3 startPos;
    Vector3 endPos;
    /*Quaternion startRot;
    Quaternion endRot;*/
    GameObject gameObject;
    bool bMovimiento; //Determina si es un lerpeador de movimiento o rotacion
    

    public Lerpeador(float fTiempo)
    {
        lerpTime = fTiempo;
        bTermino = true;
        bActivado = false;
    }

    public void Start(GameObject gameObject, Vector3 posFinal)
    {
        this.gameObject = gameObject;
        startPos = gameObject.transform.position;
        endPos = posFinal;
        bMovimiento = true;
        bActivado = true;
        bTermino = false;
    }

    //Por ahora la funcionalidad de rotacion no se implementa
    /*public void Start(GameObject gameObject, Quaternion rotFinal)
    {
        this.gameObject = gameObject;
        startRot = gameObject.transform.rotation;
        endRot = rotFinal;
        bMovimiento = false;
    }*/

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
        /*else
        {
            gameObject.transform.rotation = Quaternion.Lerp(startRot, endRot, perc);
            Debug.Log(gameObject.transform.rotation);
        }*/
        return bTermino;
    }
}
