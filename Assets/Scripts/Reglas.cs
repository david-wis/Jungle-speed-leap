using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reglas : MonoBehaviour {
    public GameObject pagina1, pagina2, menu;
    private bool _bMenuActivado = true;

    public void Siguiente()
    {
        pagina2.SetActive(true);
        pagina1.SetActive(false);
        _bMenuActivado = false;
        StartCoroutine(ActivarBtnMenu());
    }

    public void Atras()
    {
        pagina1.SetActive(true);
        pagina2.SetActive(false);
    }

    public void VolverAMenu()
    {
        if (_bMenuActivado)
        {
            menu.SetActive(true);
            pagina1.SetActive(true);
            pagina2.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    IEnumerator ActivarBtnMenu()
    {
        yield return new WaitForSeconds(3f);
        _bMenuActivado = true;
    }
}
