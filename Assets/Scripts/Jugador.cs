using System.Collections.Generic;
using UnityEngine;

public class Jugador
{
    private Stack<Carta> _pilaCartas;
    private Carta _cartaActual;
    public GameObject Manos;

    public Carta CartaActual
    {
        get
        {
            return _cartaActual;
        }
    }

    public Jugador()
    {
        _pilaCartas = new Stack<Carta>();
    }

    /// <summary>
    /// Devuelve la cantidad de cartas restantes en el mazo del jugador
    /// </summary>
    public int ObtenerCantCartas()
    {
        return _pilaCartas.Count;
    }

    /// <summary>
    /// Agrega una carta a la pila (mazo) de cartas del jugador
    /// </summary>
    /// <param name="carta">Carta para agregar al mazo del jugador</param>
    public void AgregarCarta(Carta carta)
    {
        _pilaCartas.Push(carta);
    }

    /// <summary>
    /// Devuelve la siguiente carta de la pila (mazo) de cartas del jugador, y la saca de la pila
    /// </summary>
    /// <returns>La siguiente carta si hay, o null si no tiene mas cartas</returns>
    public Carta ObtenerSiguienteCarta()
    {
        if (_pilaCartas.Count > 0) { 
            _cartaActual = _pilaCartas.Pop();
        } else
        {
            _cartaActual = null;
        }
        return _cartaActual;
    }
}
