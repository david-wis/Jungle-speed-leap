class Mesa
{
    Carta[] cartasEnJuego = new Carta[4];
    int iTope = 0;

    public Mesa()
    {
        for (int i = 0; i < 4; i++)
        {
            cartasEnJuego[i] = null;
        }
    }

    public void AgregarCarta(Carta c)
    {
        cartasEnJuego[iTope] = c;
        iTope = (iTope < 3) ? iTope + 1 : 0;
    }

    //Este metodo se tiene que llamar cada vez que alguien agarre el totem 
    public bool VerificarIgualdadConResto(int iIndexJugador)
    {
        bool bEsCorrecto = false;
        int i = 0;
        while (!bEsCorrecto && i < 4)
        {
            //No quiero comparar la carta que busco con ella misma
            //Tampoco me conviene comparar con una carta que no existe :v
            if (i != iIndexJugador && cartasEnJuego[i] != null) 
            {
                if (cartasEnJuego[i].forma == cartasEnJuego[iIndexJugador].forma)
                {
                    bEsCorrecto = true;
                }
            }
            i++;
        }
        return bEsCorrecto;
    }
}

