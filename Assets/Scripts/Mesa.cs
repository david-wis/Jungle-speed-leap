using System.Collections.Generic;
using UnityEngine;

class Mesa
{
    Stack<Carta>[] cartasEnJuego = new Stack<Carta>[4];
    Stack<GameObject>[] gameObjectsEnJuego = new Stack<GameObject>[4];
    int iTopeCartas = 0, iTopeGameObjects = 0;
    ModoJuego modo;

    public Mesa()
    {
        modo = ModoJuego.Normal;
        for (int i = 0; i < 4; i++)
        {
            cartasEnJuego[i] = new Stack<Carta>();
            gameObjectsEnJuego[i] = new Stack<GameObject>();
        }
    }

    
    
    public void AgregarCarta(Carta c)
    {
        cartasEnJuego[iTopeCartas].Push(c);
        VerificarModoColor(c);
        iTopeCartas = (iTopeCartas < 3) ? iTopeCartas + 1 : 0;
    }

    int iIndexJugadorColor = -1;
    private void VerificarModoColor(Carta c)
    {
        if (modo == ModoJuego.Colores)
        {
            if (iTopeCartas == iIndexJugadorColor)
            {
                //Volvemos al modo normal y ya no hay ningun jugador que haya empezado el tema del color
                modo = ModoJuego.Normal;
                iIndexJugadorColor = -1;
            }
        }

        if (c.color == Carta.Color.Especial)
        {
            modo = (ModoJuego) c.forma;
            if (modo == ModoJuego.Colores)
            {
                iIndexJugadorColor = iTopeCartas; //Si se cambia a modo color es necesario saber que jugador lo comenzo
            }
        }
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
        switch (modo)
        {
            case ModoJuego.Normal: //Modo normal
                if (cartasEnJuego[iIndexJugador].Count != 0)
                {
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
                break;
            case ModoJuego.Colores: //Se verifica si tienen el mismo color
                if (cartasEnJuego[iIndexJugador].Count != 0)
                {
                    Carta cartaJugador = cartasEnJuego[iIndexJugador].Peek();
                    for (int i = 0; i < 4; i++)
                    {
                        if (i != iIndexJugador && cartasEnJuego[i].Count != 0)
                        {
                            if (cartasEnJuego[i].Peek().color == cartaJugador.color)
                            {
                                listaCoincidencias.Add(i);
                            }
                        }
                    }
                }
                modo = ModoJuego.Normal; //Se termino el efecto de color
                break;
            default:
                Debug.Log("Esto no deberia pasar nunca :v");
                break;
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

enum ModoJuego
{
    Normal = 0,
    Colores = -1,
    Dentro = -2,
    Fuera = -3
}