using System.Collections.Generic;
using UnityEngine;

class Mesa
{
    Stack<Carta>[] cartasEnJuego = new Stack<Carta>[4];
    Stack<GameObject>[] gameObjectsEnJuego = new Stack<GameObject>[4];
    int iTopeCartas = 0, iTopeGameObjects = 0;

    public Mesa()
    {
        for (int i = 0; i < 4; i++)
        {
            cartasEnJuego[i] = new Stack<Carta>();
            gameObjectsEnJuego[i] = new Stack<GameObject>();
        }
    }

    public void AgregarCarta(Carta c)
    {
        cartasEnJuego[iTopeCartas].Push(c);
        iTopeCartas = (iTopeCartas < 3) ? iTopeCartas + 1 : 0;
    }

    public void AgregarGameObject(GameObject gameObject)
    {
        gameObjectsEnJuego[iTopeGameObjects].Push(gameObject);
        iTopeGameObjects = (iTopeGameObjects < 3) ? iTopeGameObjects + 1 : 0;
    }

    //Metodo deprecado para verificar igualdad entre cartas
    /*public bool VerificarIgualdadConResto(int iIndexJugador)
    {
        bool bEsCorrecto = false;
        int i = 0;
        Carta cartaJugador = cartasEnJuego[iIndexJugador].Peek();
        while (!bEsCorrecto && i < 4)
        {
            //No quiero comparar la carta que busco con ella misma
            //Tampoco me conviene comparar con una carta que no existe :v
            if (i != iIndexJugador && cartasEnJuego[i].Count != 0) 
            {
                if (cartasEnJuego[i].Peek().forma == cartaJugador.forma)
                {
                    bEsCorrecto = true;
                }
            }
            i++;
        }
        return bEsCorrecto;
    }*/

    //Este metodo se tiene que llamar cada vez que alguien agarre el totem 
    public List<int> VerificarIgualdadConResto(int iIndexJugador)
    {
        List<int> listaCoincidencias = new List<int>();
        if (cartasEnJuego[iIndexJugador].Count != 0) { 
            Carta cartaJugador = cartasEnJuego[iIndexJugador].Peek();
            for (int i = 0; i < 4; i++)
            {
                if (i != iIndexJugador && cartasEnJuego[i].Count != 0)
                {
                    if (cartasEnJuego[i].Peek().forma == cartaJugador.forma)
                    {
                        listaCoincidencias.Add(i);
                    }
                }
            }
        }
        return listaCoincidencias;
    }

    public GameObject[] obtenerGameObjectsDelJugador(int idJugador)
    {
        GameObject[] gameObjects = gameObjectsEnJuego[idJugador].ToArray();
        return gameObjects;
                
    }

    public Stack<Carta> obtenerCartasDelJugador(int idJugador)
    {
        return cartasEnJuego[idJugador];
    }
}

