using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MensajeInicial : MonoBehaviour
{

	public GameObject mensajeInicial;
	private InteractionBehaviour _intObj;

	// Use this for initialization
	void Start()
	{
		_intObj = GetComponent<InteractionBehaviour>();
	}

	// Update is called once per frame
	void Update()
	{
		if (_intObj.isPrimaryHovered)
		{
			Destroy(mensajeInicial);//Eventually, the mensajito stopped thinking
			Destroy(this);
		}
	}
}
