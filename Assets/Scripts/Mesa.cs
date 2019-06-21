using System.Collections.Generic;
using UnityEngine;

class Mesa
{
    Stack<Carta>[] cartasEnJuego = new Stack<Carta>[4];
    int iTope = 0;

    public Mesa()
    {
        for (int i = 0; i < 4; i++)
        {
            cartasEnJuego[i] = new Stack<Carta>();
        }
    }

    public void AgregarCarta(Carta c)
    {
        cartasEnJuego[iTope].Push(c);
        iTope = (iTope < 3) ? iTope + 1 : 0;
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
}

