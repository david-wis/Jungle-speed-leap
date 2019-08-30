using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {
    public Sprite manoAbierta, manoCerrada;
    public Button btnJugar, btnReglas, btnSalir;
    HandModelManager handManager;
    LeapProvider _provider;
    Controller _controlador;
    SpriteRenderer imgMano;
    GameObject mano;
    Camera c;

    // Use this for initialization
    void Start () {
        handManager = GetComponent<HandModelManager>();
        mano = transform.GetChild(0).gameObject;
        imgMano = mano.GetComponent<SpriteRenderer>();
        _provider = handManager.leapProvider;
        imgMano.sprite = manoAbierta;
	}

    // Update is called once per frame
    void Update() {
        Frame frame = _provider.CurrentFrame;
        if (frame.Hands.Count > 0)
        {
            Hand mano = frame.Hands[0];
            if (mano.Fingers.TrueForAll(NoExtendido))
            {
                imgMano.sprite = manoCerrada;
            } else
            {
                imgMano.sprite = manoAbierta;
            }
        }
        c = Camera.current;
        CheckearContactoBoton(btnJugar);
        CheckearContactoBoton(btnReglas);
        CheckearContactoBoton(btnSalir);
    }

    private void CheckearContactoBoton(Button btn)
    {
        int x = (int)btn.transform.position.x;
        int y = (int)btn.transform.position.y;
        /*if (mano.transform.position.x > x && mano.transform.position.x < (x + btn.GetComponent<RectTransform>().rect.width))
        {
            if (mano.transform.position.y > y && mano.transform.position.y < (y + btn.GetComponent<RectTransform>().rect.width)) {
                Debug.Log("Adentro de " + btn.name);
            }
        }*/

        RaycastHit hit;
        Camera cam = Camera.current;
        Ray ray = cam.ScreenPointToRay(btn.transform.position);

        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;
            Debug.Log("Hay algo en el medio");
            // Do something with the object that was hit by the raycast.
        }
    }

    private bool NoExtendido(Finger f)
    {
        return !f.IsExtended;
    }

    
}
