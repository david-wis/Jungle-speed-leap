using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {
    public Sprite manoAbierta, manoCerrada;
    public Button btnJugar, btnReglas, btnSalir;
    public Camera c;
    public EventSystem wEvents;
    HandModelManager handManager;
    LeapProvider _provider;
    Controller _controlador;
    SpriteRenderer imgMano;
    GameObject mano;
    Vector3 posMano
    {
        get
        {
            return c.WorldToScreenPoint(mano.transform.position);
        }
    }

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
        //Debug.Log("Mano: " + c.WorldToScreenPoint(mano.transform.position));
        bool bSeleccionado = false;
        bSeleccionado = bSeleccionado || CheckearContactoBoton(btnJugar);
        bSeleccionado = bSeleccionado || CheckearContactoBoton(btnReglas);
        bSeleccionado = bSeleccionado || CheckearContactoBoton(btnSalir);
        if (!bSeleccionado)
        {
            wEvents.SetSelectedGameObject(null);
        }
    }

    private bool CheckearContactoBoton(Button btn)
    {
        bool bSeleccionado = false;
        RectTransform rect = btn.GetComponent<RectTransform>();
        Vector3 pos = c.WorldToScreenPoint(rect.position);
        float fAncho = rect.rect.width;
        float fAlto = rect.rect.height;
        Vector3 fVeci = (pos - new Vector3(fAncho / 2, fAlto / 2, 0));
        Vector3 fVecf = (pos + new Vector3(fAncho / 2, fAlto / 2, 0));
        Debug.Log(btn.name + "- comienzo: " + fVeci + " - fin: " + fVecf);
        Debug.Log(posMano);

        if (posMano.x >= fVeci.x && posMano.y >= fVeci.y && posMano.x <= fVecf.x && posMano.y <= fVecf.y)
        {
            Debug.Log("Adentro de " + btn.name);
            btn.Select();
            bSeleccionado = true;
        }
        return bSeleccionado;
    }

    private bool NoExtendido(Finger f)
    {
        return !f.IsExtended;
    }

    
}
