using System.Collections.Generic;


public class Jugador
{
    Stack<Carta> pilaCartas;
    Carta cartaActual;


    public Jugador()
    {
        pilaCartas = new Stack<Carta>();
    }

    public void AgregarCarta(Carta carta)
    {
        pilaCartas.Push(carta);
    }

    public void ObtenerCartaActual()
    {
        cartaActual = pilaCartas.Pop();
    }
}
