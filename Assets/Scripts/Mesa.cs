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

    /// <summary>
    /// Pone el modo de juego en normal
    /// </summary>
    public void NormalizarModo()
    {
        //Debug.Log("Modo normalizado exitosamente");
        _modo = ModoJuego.Normal;
    }

    /// <summary>
    /// Agrega la Carta al Stack de cartas en juego del Jugador que le toca. 
    /// Devuelve el modo de juego actual.
    /// </summary>
    /// <param name="c">Carta a agregar al vector de cartas en juego del Jugador actual</param>
    public ModoJuego AgregarCarta(Carta c)
    {
        cartasEnJuego[iTopeCartas].Push(c);
        VerificarFinModo(ModoJuego.Colores); //Se fija (si existe) si terminó la ronda de color
        VerificarFinModo(ModoJuego.Dentro); //Se fija (si existe) si terminó la ronda de flechas para adentro
        VerificarEspecial(c); //Si la carta es especial cambia el modo de juego
        iTopeCartas = (iTopeCartas < 3) ? iTopeCartas + 1 : 0;
        return _modo;
    }
    
    /// <summary>
    /// Se fija si la ronda de determinado modo sigue vigente. Si no lo está, cambia a modo normal
    /// </summary>
    /// <param name="modo">Indica el modo que se quiere verificar</param>
    private void VerificarFinModo(ModoJuego modo)
    {
        if (_modo == modo)
        {
            bool bColorActivado = false;
            int i = 0;
            while (!bColorActivado && i < 4)
            {
                Carta c = cartasEnJuego[i].Peek();
                if (c.color == Carta.Color.Especial && c.forma == (int) modo)
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

    /// <summary>
    /// Cambia el modo de juego si nos encontramos con una carta especial
    /// </summary>
    /// <param name="c"></param>
    private void VerificarEspecial(Carta c)
    {
        if (c.color == Carta.Color.Especial)
        {
            _modo = (ModoJuego)c.forma;
            Debug.Log("Nos hemos encontrado con una carta especial de tipo " + _modo);
        }
    }

    /// <summary>
    /// Agrega el GameObject recibido al Stack de GameObjects en juego del jugador actual
    /// </summary>
    /// <param name="gameObject">El GameObject a agregar al Stack</param>
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

    /// <summary>
    /// Devuelve los jugadores enemigos en un duelo
    /// </summary>
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

    /// <summary>
    /// Obtiene los gameobjects de las cartas de un jugador en forma de array y luego vacía la pila
    /// </summary>
    /// <param name="idJugador">Jugador sobre cuya pila de gameobjects de cartas se desea actuar</param>
    /// <returns>Vector de gameobjects de cartas</returns>
    public GameObject[] obtener_VaciarGameObjectsDelJugador(int idJugador)
    {
        GameObject[] gameObjects = obtenerGameObjectsDelJugador(idJugador);
        vaciarGameObjectsDelJugador(idJugador);
        return gameObjects;
    }

    /// <summary>
    /// Vacia la pila de gameobjects de cartas de un jugador
    /// </summary>
    /// <param name="idJugador">Jugador cuya pila se desea vaciar</param>
    public void vaciarGameObjectsDelJugador(int idJugador)
    {
        gameObjectsEnJuego[idJugador].Clear();
    }

    /// <summary>
    /// Devuelve un vector con los gameobjects de las cartas que corresponden a determinado jugador
    /// </summary>
    /// <param name="idJugador">Jugador cuyas cartas se pretende obtener</param>
    /// <returns>Gameobjects de las cartas del jugador</returns>
    public GameObject[] obtenerGameObjectsDelJugador(int idJugador)
    {
        return gameObjectsEnJuego[idJugador].ToArray();
    }
    
    /// <summary>
    /// Devuelve el ultimo GameObject creado del jugador recibido como parametro
    /// </summary>
    /// <param name="idJugador">El ID del jugador que se quiere conocer su ultimo GameObject </param>
    /// <returns>El ultimo GameObject del jugador</returns>
    public GameObject obtenerUltimoGameObjectDelJugador(int idJugador)
    {
        return gameObjectsEnJuego[idJugador].Peek();
    }

    /// <summary>
    /// Obtiene las cartas de un jugador en forma de array y luego vacía la pila
    /// </summary>
    /// <param name="idJugador">Indice del jugador</param>
    /// <returns>Vector de cartas</returns>
    public Carta[] obtener_VaciarCartasDelJugador(int idJugador)
    {
        Carta[] cartas = obtenerCartasDelJugador(idJugador).ToArray();
        vaciarCartasDelJugador(idJugador);
        return cartas;
    }

    /// <summary>
    /// Vacía la pila de cartas de un jugador
    /// </summary>
    /// <param name="idJugador">Indice del jugador</param>
    public void vaciarCartasDelJugador(int idJugador)
    {
        cartasEnJuego[idJugador].Clear();
    }

    /*public Carta[] obtenerCartasDelJugador_Array(int idJugador)
    {
        return cartasEnJuego[idJugador].ToArray();
    }*/

    /// <summary>
    /// Obtiene la pila de cartas de un jugador determinado
    /// </summary>
    /// <param name="idJugador">Indice del jugador</param>
    /// <returns>Stack de cartas</returns>
    public Stack<Carta> obtenerCartasDelJugador(int idJugador)
    {
        return cartasEnJuego[idJugador];
    }
}

/// <summary>
/// Los 3 modos de juego especiales (flechas colores/dentro/fuera) + el modo normal
/// </summary>
public enum ModoJuego
{
    Normal = 0,
    Colores = -1,
    Dentro = -2,
    Fuera = -3
}