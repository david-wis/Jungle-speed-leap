using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseController : MonoBehaviour {
    public const int CANT_BOTONES = 6;
    public Sprite manoAbierta, manoCerrada;
    public Button[] btns = new Button[CANT_BOTONES];
    public Camera c;
    public EventSystem wEvents;
    private bool _bManoCerrada;
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
        _bManoCerrada = false;
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
                _bManoCerrada = true;
            } else
            {
                imgMano.sprite = manoAbierta;
                _bManoCerrada = false;
            }
        }
        //Debug.Log("Mano: " + c.WorldToScreenPoint(mano.transform.position));
        bool bSeleccionado = false;
        for (int i = 0; i < CANT_BOTONES; i++)
        {
            bSeleccionado = bSeleccionado || CheckearContactoBoton(btns[i]);
        }
        if (!bSeleccionado)
        {
            wEvents.SetSelectedGameObject(null);
        }
    }

    private bool CheckearContactoBoton(Button btn)
    {
        bool bSeleccionado = false;
        if (btn.IsActive())
        {
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
                if (_bManoCerrada)
                {
                    btn.onClick.Invoke();
                }
            }
        }
        return bSeleccionado;
    }

    private bool NoExtendido(Finger f)
    {
        return !f.IsExtended;
    }

    
}
