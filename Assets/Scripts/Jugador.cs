using System.Collections.Generic;


public class Jugador
{
    private Stack<Carta> _pilaCartas;
    public Carta cartaActual;

    public Jugador()
    {
        _pilaCartas = new Stack<Carta>();
    }

    public void AgregarCarta(Carta carta)
    {
        _pilaCartas.Push(carta);
    }

    public Carta ObtenerCartaActual()
    {
        cartaActual = _pilaCartas.Pop();
        return cartaActual;
    }
}
