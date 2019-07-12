using UnityEngine;

public class Lerpeador
{
    float lerpTime = 1f;
    float currentLerpTime;

    float moveDistance = 10f;

    Vector3 startPos;
    Vector3 endPos;
    GameObject gameObject;

    public void Start(GameObject gameObject, Vector3 posFinal)
    {
        this.gameObject = gameObject;
        startPos = gameObject.transform.position;
        endPos = posFinal;
    }

    public void Update()
    {
        //reset when we press spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentLerpTime = 0f;
        }

        //increment timer once per frame
        currentLerpTime += Time.deltaTime;
        if (currentLerpTime > lerpTime)
        {
            currentLerpTime = lerpTime;
        }

        //lerp!
        float perc = currentLerpTime / lerpTime;
        gameObject.transform.position = Vector3.Lerp(startPos, endPos, perc);
    }
}
