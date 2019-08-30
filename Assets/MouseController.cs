using Leap;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {
    public Sprite manoAbierta, manoCerrada;
    HandModelManager handManager;
    LeapProvider _provider;
    Controller _controlador;
    SpriteRenderer imgMano;
    
	// Use this for initialization
	void Start () {
        handManager = GetComponent<HandModelManager>();
        imgMano = transform.GetChild(0).GetComponent<SpriteRenderer>();
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
	}

    private bool NoExtendido(Finger f)
    {
        return !f.IsExtended;
    }

    
}
