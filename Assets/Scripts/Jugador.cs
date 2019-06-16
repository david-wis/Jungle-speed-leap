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

    public int ObtenerCantCartas()
    {
        return _pilaCartas.Count;
    }

    public void AgregarCarta(Carta carta)
    {
        _pilaCartas.Push(carta);
    }

    public Carta ObtenerCartaActual()
    {
        if (_pilaCartas.Count > 0) { 
            _cartaActual = _pilaCartas.Pop();
        } else
        {
            _cartaActual = null;
        }
        return CartaActual;
    }
}
