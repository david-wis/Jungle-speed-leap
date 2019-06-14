using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;


public class Main : MonoBehaviour {
    const int CANTCARTAS = 8;
    //public GameObject[] prefabCartas = new GameObject[CANTCARTAS];
    Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    Jugador[] jugadores = {new Jugador(), new Jugador(), new Jugador(), new Jugador()};
    public GameObject[] cartasEstaticas = new GameObject[4];

    int iCantJugadores;
    int iIndexJugActual;

    UnityAction eventolistener;

    //Carta[] todasCartas = new Carta[CANTCARTAS];

    // Use this for initialization
    void Start() {
        /* Aca en algun momento vamos a meter un menu 
         * para hostear partidas y elegir cant de jugadores
         */
        LlenarMazo();
        eventolistener = new UnityAction(PonerCarta);
        EventManager.StartListening("agarrarcarta", eventolistener);
        
        

        RepartirMazo();
        //En el futuro eligiremos el jugador inicial de manera aleatoria
        //iIndexJugActual = ObtenerRandom(4);
        iIndexJugActual = 0;



    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(arrayMazoTotal[0].img.name);
    }

    void PonerCarta()
    {
        Vector3 pos = new Vector3(-0.2686f, 0.1245f, -0.493f);
        Instantiate(jugadores[iIndexJugActual].ObtenerCartaActual().img,
            transform.position += pos,
            Quaternion.Euler(new Vector3(180f, 0f, 0f)));
        /*
         * 
         * TODO: Animacion echi carta se da vuelta
         * 
         */
        //iIndexJugActual++;
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
        System.Random r = new System.Random();
        return r.Next(iMax);
    }


}
