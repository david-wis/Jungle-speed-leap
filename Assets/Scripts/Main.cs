using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;

public class Main : MonoBehaviour {
    public const int CANTCARTAS = 8;
    public const int CANTJUGADORES = 4;

    Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    Jugador[] jugadores = new Jugador[CANTJUGADORES];
    public GameObject[] cartasEstaticas = new GameObject[4];

    int iIndexJugActual;
    UnityAction eventolistener;

    // Use this for initialization
    void Start() {
        /* Aca en algun momento vamos a meter un menu 
         * para hostear partidas y elegir cant de jugadores
         */

        //Llenado de mazo
        LlenarMazo();

        //Referenciar cartas estaticas
        cartasEstaticas = GameObject.FindGameObjectsWithTag("CartasEstaticas");

        //Creacion de jugadores
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            jugadores[i] = new Jugador();
            jugadores[i].Manos = manos[i];
            Debug.Log(i + ": " + manos[i].name);
        }

        //Se reparten las cartas entre los jugadores
        RepartirMazo();

        //Creacion de eventos
        eventolistener = new UnityAction(PonerCarta);
        EventManager.StartListening("agarrarcarta", eventolistener); //Evento que se produce cuando un jugador toca un mazo

        

        //En el futuro eligiremos el jugador inicial de manera aleatoria
        //iIndexJugActual = ObtenerRandom(4);
        iIndexJugActual = 0;



    }

    float fTimer = 0.0f; //Bereishit
    // Update is called once per frame
    void Update()
    {
        fTimer += Time.deltaTime;
        if (iIndexJugActual != 0)
        {

        }
    }

    float fLastTime = 0.0f; //Ultima vez que se tocó el mazo
    const float fCoolDown = 2.0f; //2 segundos
    void PonerCarta()
    {
        /*Vector3 pos = new Vector3(0.25f, 0.2f, 0.4f);
        Instantiate(jugadores[iIndexJugActual].ObtenerCartaActual().img3D,
            jugadores[iIndexJugActual].Manos.transform.position += pos,
            Quaternion.Euler(new Vector3(180f, 0f, 0f)));*/
        if (fTimer - fLastTime >= fCoolDown) { //Asi evitamos que mantener la mano apretada cause que haga todo al instante
            Carta cartaActual = jugadores[iIndexJugActual].ObtenerCartaActual();
            Image imagen = cartasEstaticas[iIndexJugActual].GetComponent<Image>();
            imagen.sprite = cartaActual.img2D;
            fLastTime = fTimer;
        }

        /*
         * 
         * TODO: Animacion echi carta se da vuelta
         * 
         */
        iIndexJugActual = (iIndexJugActual < 3)? iIndexJugActual+1 : 0;
    }


    private void RepartirMazo(int iCantJugadores = 4)
    {
        Mezclar();
        for (int i = 0; i < arrayMazoTotal.Length; i++)
        {
            //TODO: dividirlo por iCantJugadores, cuando permitamos elegir la cantidad de jugadores
            int iIndexJug = i % 4;
            jugadores[iIndexJug].AgregarCarta(arrayMazoTotal[i]);
        }
    }

    private void Mezclar()
    {
        for (int i = 0; i < arrayMazoTotal.Length; i++)
        {
            Carta cAux = arrayMazoTotal[i];
            int iRand = ObtenerRandom(arrayMazoTotal.Length);
            arrayMazoTotal[i] = arrayMazoTotal[iRand];
            arrayMazoTotal[iRand] = cAux;
        }
    }

    private void LlenarMazo()
    {
        string[] rutaCartitas = AssetDatabase.FindAssets("b:carta", new[] { "Assets/Cartas" });
        for (int i = 0; i < rutaCartitas.Length; i++)
        {
            string ruta = AssetDatabase.GUIDToAssetPath(rutaCartitas[i]);
            arrayMazoTotal[i] = (Carta)AssetDatabase.LoadAssetAtPath(ruta, typeof(Carta));
        }
    }

    private int ObtenerRandom(int iMax)
    {
        return Random.Range(0, iMax);
    }


    void CargarCartasEstaticas()
    {

    }

}
