using System.Collections.Generic;
using UnityEngine;

public class Mesa
{
    Stack<Carta>[] cartasEnJuego = new Stack<Carta>[4];
    Stack<GameObject>[] gameObjectsEnJuego = new Stack<GameObject>[4];
    private ModoJuego _modo;
    private bool _terminada;

    public bool Terminada
    {
        get { return _terminada; }
    }

    public Mesa()
    {
        _terminada = false;
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
    public void normalizarModo()
    {
        //Debug.Log("Modo normalizado exitosamente");
        _modo = ModoJuego.Normal;
    }

    /// <summary>
    /// Agrega la Carta al Stack de cartas en juego del Jugador que le toca. 
    /// Devuelve el modo de juego actual.
    /// </summary>
    /// <param name="c">Carta a agregar al vector de cartas en juego del Jugador actual</param>
    public ModoJuego agregarCarta(Carta c, int idJugador)
    {
        cartasEnJuego[idJugador].Push(c);
        verificarFinModo(ModoJuego.Colores); //Se fija (si existe) si terminó la ronda de color
        verificarFinModo(ModoJuego.Dentro); //Se fija (si existe) si terminó la ronda de flechas para adentro
        verificarEspecial(c); //Si la carta es especial cambia el modo de juego
        return _modo;
    }
    
    /// <summary>
    /// Se fija si la ronda de determinado modo sigue vigente. Si no lo está, cambia a modo normal
    /// </summary>
    /// <param name="modo">Indica el modo que se quiere verificar</param>
    private void verificarFinModo(ModoJuego modo)
    {
        if (_modo == modo)
        {
            bool bColorActivado = false;
            int i = 0;
            while (!bColorActivado && i < 4)
            {
                if (cartasEnJuego[i].Count > 0)
                {
                    Carta c = cartasEnJuego[i].Peek();
                    if (c.color == Carta.Color.Especial && c.forma == (int) modo)
                    {
                        bColorActivado = true;
                    }
                    i++;
                }
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
    private void verificarEspecial(Carta c)
    {
        if (c.color == Carta.Color.Especial)
        {
            _modo = (ModoJuego)c.forma;
            //Debug.Log("Nos hemos encontrado con una carta especial de tipo " + _modo);
        }
    }

    /// <summary>
    /// Agrega el GameObject recibido al Stack de GameObjects en juego del jugador actual
    /// </summary>
    /// <param name="gameObject">El GameObject a agregar al Stack</param>
    public void agregarGameObject(GameObject gameObject, int idJugador)
    {
        gameObjectsEnJuego[idJugador].Push(gameObject);
    }

    /// <summary>
    /// Se fija si el jugador puede agarrar el totem
    /// </summary>
    /// <param name="iIndexJugador">Indice del jugador sobre el que se quiere saber</param>
    /// <returns>Puede agarrar el totem</returns>
    public bool tieneIgualdadConResto(int iIndexJugador)
    {
        bool bEsCorrecto = false;
        int i = 0;
        switch (_modo)
        {
            case ModoJuego.Normal:
                if (cartasEnJuego[iIndexJugador].Count != 0)
                {
                    Carta cartaJugador = cartasEnJuego[iIndexJugador].Peek();
                    while (!bEsCorrecto && i < 4)
                    {
                        if (i != iIndexJugador && cartasEnJuego[i].Count != 0)
                        {
                            if (cartasEnJuego[i].Peek().forma == cartaJugador.forma)
                            {
                                bEsCorrecto = true;
                            }
                        }
                        i++;
                    }
                }
                break;
            case ModoJuego.Colores:
                if (cartasEnJuego[iIndexJugador].Count != 0)
                {
                    Carta cartaJugador = cartasEnJuego[iIndexJugador].Peek();
                    while (!bEsCorrecto && i < 4)
                    {
                        if (i != iIndexJugador && cartasEnJuego[i].Count != 0)
                        {
                            if (cartasEnJuego[i].Peek().color == cartaJugador.color)
                            {
                                bEsCorrecto = true;
                            }
                        }
                        i++;
                    }
                }
                break;
            case ModoJuego.Dentro:
                bEsCorrecto = true;
                break;
            default:
                bEsCorrecto = false;
                break;
        }
        return bEsCorrecto;
    }

    /// <summary>
    /// Devuelve los jugadores enemigos en un duelo
    /// </summary>
    public List<int> verificarIgualdadConResto(int iIndexJugador)
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
                //Debug.Log("Esto no deberia pasar nunca :v");
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
        GameObject gameObjDevolver = null;
        if (gameObjectsEnJuego[idJugador].Count > 0)
        {
            gameObjDevolver = gameObjectsEnJuego[idJugador].Peek();
        }
        return gameObjDevolver;
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

    /// <summary>
    /// Obtiene la pila de cartas de un jugador determinado
    /// </summary>
    /// <param name="idJugador">Indice del jugador</param>
    /// <returns>Stack de cartas</returns>
    public Stack<Carta> obtenerCartasDelJugador(int idJugador)
    {
        return cartasEnJuego[idJugador];
    }

    /// <summary>
    /// Cambia el estado de la partida a terminada
    /// </summary>
    public void terminarPartida()
    {
        _terminada = true;
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