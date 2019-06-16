class Mesa
{
    Carta[] cartasEnJuego = new Carta[4];
    int iTope = 0;
    public void AgregarCarta(Carta c)
    {
        cartasEnJuego[iTope] = c; 
    }

    //Este metodo se tiene que llamar cada vez que alguien agarre el totem 
    public bool VerificarIgualdadConResto(Carta c)
    {
        bool bEsCorrecto = false;
        for (int i = 0; i < 4; i++)
        {
            if (cartasEnJuego[i] != c)
            {
                if (cartasEnJuego[i].forma == c.forma)
                {
                    bEsCorrecto = true;
                }
            }
        }
        return bEsCorrecto;
    }
}

