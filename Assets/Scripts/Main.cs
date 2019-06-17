using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using System;

public class Main : MonoBehaviour {
    public const int CANTCARTAS = 8;
    public const int CANTJUGADORES = 4;

    Carta[] arrayMazoTotal = new Carta[CANTCARTAS];
    Jugador[] jugadores = new Jugador[CANTJUGADORES];
    public GameObject[] cartasEstaticas = new GameObject[4];
    GameObject[] mazos = new GameObject[4];

    GameObject totem;

    Mesa mesa = new Mesa();

    int iIndexJugActual;
    UnityAction eventoListenerMazo0;
    UnityAction eventoListenerMazo1;
    UnityAction eventoListenerMazo2;
    UnityAction eventoListenerMazo3;
    UnityAction eventoListenerTotem;

    // Use this for initialization
    void Start() {
        /* Aca en algun momento vamos a meter un menu 
         * para hostear partidas y elegir cant de jugadores
         */

        LlenarMazo();
        ReferenciarCartasEstaticas();
        ReferenciarMazos();
        CrearJugadores();

        //Se reparten las cartas entre los jugadores
        RepartirMazo();

        //Creacion de eventos 
        /*
         * OPTIMIZAR (si es posible :v) EN VERSION FUTURA
         */
        eventoListenerMazo0 = new UnityAction(delegate () { PonerCarta(0); });
        eventoListenerMazo1 = new UnityAction(delegate () { PonerCarta(1); });
        eventoListenerMazo2 = new UnityAction(delegate () { PonerCarta(2); });
        eventoListenerMazo3 = new UnityAction(delegate () { PonerCarta(3); });

        eventoListenerTotem = new UnityAction(AgarrarTotem);

        EventManager.StartListening("agarrarcarta0", eventoListenerMazo0); //Evento que se produce cuando un jugador toca un mazo
        EventManager.StartListening("agarrarcarta1", eventoListenerMazo1); 
        EventManager.StartListening("agarrarcarta2", eventoListenerMazo2); 
        EventManager.StartListening("agarrarcarta3", eventoListenerMazo3); 

        EventManager.StartListening("agarrartotem", eventoListenerTotem); //Evento que se produce cuando un jugador agarra el totem

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
            /*
             * TODO: bots :v
             */
        }
    }

    float fLastTime = 0.0f; //Ultima vez que se tocó el mazo
    const float fCoolDown = 2.0f; //2 segundos
    void PonerCarta(int iIndexMazo)
    {
        /*Vector3 pos = new Vector3(0.25f, 0.2f, 0.4f);
        Instantiate(jugadores[iIndexJugActual].ObtenerCartaActual().img3D,
            jugadores[iIndexJugActual].Manos.transform.position += pos,
            Quaternion.Euler(new Vector3(180f, 0f, 0f)));*/
        if (iIndexJugActual == iIndexMazo) { 
            if (fTimer - fLastTime >= fCoolDown) { //Asi evitamos que mantener la mano apretada cause que haga todo al instante
                Carta cartaActual = jugadores[iIndexJugActual].ObtenerCartaActual();
                if (cartaActual != null)
                {
                    mesa.AgregarCarta(cartaActual); //Agregamos la carta al vector de cartas de la mesa
                    /*
                        * 
                        * TODO: Animacion echi carta se da vuelta
                        * 
                    */
                    Image imagen = cartasEstaticas[iIndexJugActual].GetComponent<Image>();
                    var color = imagen.color;
                    color.a = 1;
                    imagen.sprite = cartaActual.img2D;
                    imagen.color = color;
                    fLastTime = fTimer;
                    //Debug.Log("Jugador " + iIndexJugActual + " - " + cartaActual.img2D.name + " - " + imagen.name + " - " + mazos[iIndexJugActual].name);
                    if (jugadores[iIndexJugActual].ObtenerCantCartas() == 0)
                    {
                        mazos[iIndexJugActual].SetActive(false);
                    }
                    iIndexJugActual = (iIndexJugActual < 3) ? iIndexJugActual + 1 : 0;
                }
            }
        }
    }

    /*
     *  Hasta que hagamos el multiplayer, iJugadorTotem va a ser una variable
     *  que se va a modificar siempre por alguna funcion del mismo main.
     *  Si armamos el multiplayer, vamos a tener que pasarle un parametro a AgarrarTotem
     *  que indique que jugador realizo el grasp, ya que no va a ser tan facil 
     *  predecirlo como en este caso, en el que o bien decidimos nosotros que bot 
     *  agarra el totem, o por defecto lo hace el único jugador.
     */
    int iJugadorTotem = 0; //Jugador que agarro el totem
    void AgarrarTotem()
    {
        bool bHizoBien = mesa.VerificarIgualdadConResto(iJugadorTotem);
        if (bHizoBien)
        {
            Debug.Log("Well done!");
        } else
        {
            Debug.Log("wtf, sacame la manito bro");
        }
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
        return UnityEngine.Random.Range(0, iMax);
    }

    void ReferenciarCartasEstaticas()
    {
        cartasEstaticas = GameObject.FindGameObjectsWithTag("CartasEstaticas");
        Array.Sort(cartasEstaticas, CompareObNames);
    }

    void ReferenciarMazos()
    {
        mazos = GameObject.FindGameObjectsWithTag("Mazo");
        Array.Sort(mazos, CompareObNames); 
    }

    public void CrearJugadores()
    {
        GameObject[] manos = GameObject.FindGameObjectsWithTag("Jugador");
        Array.Sort(manos, CompareObNames); //Supuestamente ya estaban en orden pero x las dudas
        for (int i = 0; i < CANTJUGADORES; i++)
        {
            jugadores[i] = new Jugador();
            jugadores[i].Manos = manos[i];
        }
    }

    int CompareObNames(GameObject x, GameObject y) //Ordenar por nombre
    {
        return x.name.CompareTo(y.name);
    }
}
