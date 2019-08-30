using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MesaManager : MonoBehaviour {

    //Usando un singleton vamos a poder acceder al indice de jugador actual desde cualquier lado
    #region Singleton
    public static MesaManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    //Recien se puede agarrar si ya paso tiempoMinimoAgarrar segundos de la ultima vez que se agarro
    float timerUltimoAgarrado, tiempoMinimoAgarrar = 1f;

    private void Start()
    {
        timerUltimoAgarrado = 0f;
    }

    private void Update()
    {
        timerUltimoAgarrado += Time.deltaTime;
        Debug.Log("Timer: " + timerUltimoAgarrado);
    }

    /// <summary>
    /// Indice del jugador al que le toca actualmente
    /// </summary>
    public int iIndexJugActual;

    /// <summary>
    /// Objeto de tipo Mesa sobre el que se juega la partida 
    /// </summary>
    public Mesa mesa = new Mesa();

    /// <summary>
    /// Vector que contiene los jugadores de la partida
    /// </summary>
    public Jugador[] jugadores;

    /// <summary>
    /// Vector que indica cuales de los jugadores estan moviendose hacia el totem
    /// </summary>
    private bool[] vecAgarrandoTotem = { false, false, false, false };

    /// <summary>
    /// Determina si alguien está yendo a tocar el totem. Si está en true, es conveniente desactivar la acción de tocar el mazo de otros bots.
    /// </summary>
    public bool AlguienToca
    {
        get
        {
            bool bAlguienToca = false;
            int i = 0;
            while (!bAlguienToca && i < 4)
            {
                //Debug.Log("Jugador " + i + " agarrando totem: " + vecAgarrandoTotem[i]);
                if (vecAgarrandoTotem[i])
                {
                    bAlguienToca = true;
                }
                i++;
            }
            return bAlguienToca;
        }
    }

    public float TimerUltimoAgarrado
    {
        get
        {
            return timerUltimoAgarrado;
        }

    }

    /// <summary>
    /// Pone el timerUltimoAgarrado en 0
    /// </summary>
    public void reiniciarTimer()
    {
        timerUltimoAgarrado = 0f;
    }

    /// <summary>
    /// Compara si ya paso tiempoMinimoAgarrar de la ultima vez que se agarro
    /// </summary>
    public bool yaSePuedeSacar()
    {
        return timerUltimoAgarrado >= tiempoMinimoAgarrar;
    }

    /// <summary>
    /// Permite indicar si un jugador está o no moviendose hacia el mazo
    /// </summary>
    /// <param name="iIndexJug">Jugador sobre el que se desea actuar</param>
    /// <param name="bEstado">Está moviendose hacia el totem?</param>
    public void CambiarEstadoToque(int iIndexJug, bool bEstado)
    {
        vecAgarrandoTotem[iIndexJug] = bEstado;
    }
}
