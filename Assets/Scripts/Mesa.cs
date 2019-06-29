using System.Collections.Generic;
using UnityEngine;

class Mesa
{
    Stack<Carta>[] cartasEnJuego = new Stack<Carta>[4];
    Stack<GameObject>[] gameObjectsEnJuego = new Stack<GameObject>[4];
    int iTopeCartas = 0, iTopeGameObjects = 0;
    private ModoJuego _modo;

    public Mesa()
    {
        _modo = ModoJuego.Normal;
        for (int i = 0; i < 4; i++)
        {
            cartasEnJuego[i] = new Stack<Carta>();
            gameObjectsEnJuego[i] = new Stack<GameObject>();
        }
    }

    public ModoJuego Modo
    {
        get
        {
            return _modo;
        }
    }

    public void NormalizarModo()
    {
        Debug.Log("Modo normalizado exitosamente");
        _modo = ModoJuego.Normal;
    }

    public ModoJuego AgregarCarta(Carta c)
    {
        cartasEnJuego[iTopeCartas].Push(c);
        VerificarFinModoColor(); //Se fija (si existe) si terminó la ronda de color
        VerificarEspecial(c); //Si la carta es especial cambia el modo de juego
        iTopeCartas = (iTopeCartas < 3) ? iTopeCartas + 1 : 0;
        return _modo;
    }
    
    private void VerificarFinModoColor()
    {
        if (_modo == ModoJuego.Colores)
        {
            bool bColorActivado = false;
            int i = 0;
            while (!bColorActivado && i < 4)
            {
                Carta c = cartasEnJuego[i].Peek();
                if (c.color == Carta.Color.Especial && c.forma == -1)
                {
                    bColorActivado = true;
                }
                i++;
            }
            if (!bColorActivado)
            {
                _modo = ModoJuego.Normal;
            }
        }
    }

    private void VerificarEspecial(Carta c)
    {
        if (c.color == Carta.Color.Especial)
        {
            _modo = (ModoJuego)c.forma;
            Debug.Log("Nos hemos encontrado con una carta especial de tipo " + _modo);
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

    //Devuelve los jugadores enemigos en un duelo
    public List<int> VerificarIgualdadConResto(int iIndexJugador)
    {
        List<int> listaCoincidencias = new List<int>();
        switch (_modo)
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

public enum ModoJuego
{
    Normal = 0,
    Colores = -1,
    Dentro = -2,
    Fuera = -3
}