using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Main : MonoBehaviour {
    public const int CANTCARTAS = 80;
    public GameObject[] prefabCartas = new GameObject[CANTCARTAS]; //Recibe los prefabs desde Unity
    public Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    public Jugador[] jugadores = new Jugador[4];
    public GameObject[] cartasEstaticas = new GameObject[4];
    public RuntimeAnimatorController contrAnimCartaDesdeMazo;

    Animator elAnimador;

    public int iCantJugadores;
    public int iIndexJugActual;

    public UnityAction eventoListener;

    // Use this for initialization
    void Start() {
        /* Aca en algun momento vamos a meter un menu 
         * para hostear partidas y elegir cant de jugadores
         */
        LlenarMazo();
        eventoListener = new UnityAction(PonerCarta);
        EventManager.StartListening("agarrarcarta", eventoListener);

        //Solo para Debug
        EventManager.TriggerEvent("agarrarcarta");


        //RepartirMazo();
        iIndexJugActual = 0;

        //En el futuro eligiremos el jugador inicial de manera aleatoria
        //iIndexJugActual = ObtenerRandom(4);

	}

    void PonerCarta()
    {
        //Debug.Log("Poniendo carta");
        Vector3 posicEstatico = new Vector3(-0.2686f, 0.1245f, -0.493f);
        Instantiate(prefabCartas[0], 
            transform.position += posicEstatico, 
            Quaternion.identity);

        Vector3 posicDesdeMazo = new Vector3(0.161f, 1.5f, -0.484f);
        Vector3 rotacDesdeMazo = new Vector3(270f, 185f, 0f);
        GameObject cartaDesdeMazo = Instantiate(
            prefabCartas[0],
            posicDesdeMazo,
            Quaternion.Euler(rotacDesdeMazo), 
            null);
        cartaDesdeMazo.AddComponent<Animator>();
        elAnimador = cartaDesdeMazo.GetComponent<Animator>();
        elAnimador.runtimeAnimatorController = contrAnimCartaDesdeMazo;        
    }

    private void RepartirMazo(int iCantJugadores = 4)
    {
        Mezclar(ref arrayMazoTotal);
        Mezclar(ref arrayMazoTotal);
        for (int i = 0; i < arrayMazoTotal.Length; i++)
        {
            //TODO: dividirlo por iCantJugadores, cuando permitamos elegir la cantidad de jugadores
            int iIndexJug = i % 4;
            jugadores[iIndexJug].AgregarCarta(arrayMazoTotal[i]);
        }
    }

    private void Mezclar(ref Carta[] arrayMazoTotal)
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
        for (int i = 0; i < CANTCARTAS; i++)
        {
            arrayMazoTotal[i] = new Carta(i + 1);
            arrayMazoTotal[i].prefab = prefabCartas[i];
        }
    }

    private int ObtenerRandom(int iMax)
    {
        System.Random r = new System.Random();
        return r.Next(iMax);
    }

    // Update is called once per frame
    void Update () {
        
    }
}
