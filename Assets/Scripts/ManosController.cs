using System.Collections;
using UnityEngine;

public class ManosController : MonoBehaviour {

    public int iIndexJug;
    public RuntimeAnimatorController contAnimacManos;
    public float fVelocidad;

    public Mesa mesa
    {
        set { MesaManager.instance.mesa = value; }
        get { return MesaManager.instance.mesa; }
    }

    GameObject totem;
    GameObject manoIzq, manoDer;
    TotemBehaviour totemBehaviour;
    Vector3[] posicionInicial = new Vector3[2]; //Posicion inicial de las manitos
    Lerpeador lerpMov, lerpMovBack, lerpMovUp;
    //Lerpeador lerpRot;
    int iEstado;
    Vector3 corrimiento;
    //Boolean bLoAgarre;
    bool bAnimandose;

    // Use this for initialization
    void Start()
    {
        fVelocidad = Random.Range(0.8f, 1.8f);
        initLerpeadores();
        //lerpRot = new Lerpeador(0.5f);

        manoIzq = transform.GetChild(0).gameObject;
        manoDer = transform.GetChild(1).gameObject;

        totem = TotemManager.instance.totem;
        totemBehaviour = totem.GetComponent<TotemBehaviour>();
        PonerManos();
        iEstado = 0;
        posicionInicial[0] = manoIzq.transform.position;
        posicionInicial[1] = manoDer.transform.position;
        //bLoAgarre = false;
        bAnimandose = false;        
    }

    // Update is called once per frame
    void Update()
    {
        if (mesa.Terminada)
            Destroy(this); //Eventually, the bot stopped thinking
        verificarTocarMazo();
        verificarAgarrarTotem();
        
    }

    private void verificarTocarMazo()
    {
        //Debug.Log(iIndexJug + ": " + iEstado);
        //Debug.Log("Jugador " + (iIndexJug + 1) + ". Es su turno? " + (MesaManager.instance.iIndexJugActual == iIndexJug) + ". Estado: " + iEstado + ". No puede tocar: " + (!MesaManager.instance.AlguienToca));
        if ((MesaManager.instance.iIndexJugActual == iIndexJug) && (iEstado == 0) && !(MesaManager.instance.AlguienToca) && (mesa.Modo != ModoJuego.Fuera))
        {
            //Debug.Log("Jugador " + (iIndexJug + 1) + ". Animandose? " + bAnimandose + ". Mano derecha en posicion? " + ObtenerPosCorrecta(manoDer, 1));
            if ((MesaManager.instance.yaSePuedeSacar()) && (!bAnimandose) && (ObtenerPosCorrecta(manoDer, 1)) /*&& ObtenerPosCorrecta(manoIzq, 0)*/)
            {
                AnimarMano("tocarMazo");
                StartCoroutine(detenerAnimacion());
                bAnimandose = true;
            }
        }
    }

    private void verificarAgarrarTotem()
    {
        bool bAgarrarPosible = mesa.TieneIgualdadConResto(iIndexJug);
        bool cartasAnimandoseEnMesa = MesaManager.instance.CartaAnimandoseEnMesa;
        //Debug.Log("Jugador " + (iIndexJug+1) + ", posibilidad de agarrar el totem: " + bAgarrarPosible);
        if (bAgarrarPosible && !bAnimandose && !cartasAnimandoseEnMesa) {
            //Se anima solo si no esta yendo al mazo, ni hay cartas animandose
            if (!totemBehaviour.estaAgarrado() || totemBehaviour.ObtenerJugador() == iIndexJug)
            {
                IntentarAgarrar();
            }
            else
            {
                Retroceder();
                iEstado = 0;
            }
        }
        else
        {
            Retroceder();
            iEstado = 0;
        }
    }

    private bool ObtenerPosCorrecta(GameObject mano, int iIndex)
    {
        bool bPosCorrecta = (mano.transform.position == posicionInicial[iIndex]);
        //Debug.Log(mano.transform.parent.name + " en posicion correcta: " + bPosCorrecta);
        return bPosCorrecta;
    }

    //OPTIMIZAR: esto podria fusionarse con la funcion de IntentarAgarrar
    int iEstadoRetroceso = 0;
    /// <summary>
    /// Si la mano se queda en el medio del recorrido en un momento que no corresponde o otro agarra el totem antes, vuelve para atras
    /// </summary>
    private void Retroceder()
    {
        if (!ObtenerPosCorrecta(manoDer, 1))
        {
            MesaManager.instance.CambiarEstadoToque(iIndexJug, false); //Ya no esta buscando el totem
            //Debug.Log("Yo, el Jugador " + (iIndexJug + 1) + " he decidido retroceder. Estado previo: " + iEstado);
            if (iEstadoRetroceso == 0)
            {
                lerpMovBack.Start(manoDer, posicionInicial[1]);
                iEstadoRetroceso++;
            }
            else if (iEstadoRetroceso == 1)
            {
                if (lerpMovBack.Update())
                {
                    iEstadoRetroceso++;
                }
            }
            else
            {
                iEstadoRetroceso = 0;
                initLerpeadores();
                
            }
        }
    }

    /// <summary>
    /// Realiza los movimientos para robar el totem
    /// </summary>
    private void IntentarAgarrar()
    {
        //Debug.Log("Jugador " + (iIndexJug + 1) + " intentando agarrar el totem");
        switch (iEstado)
        {
            case 0:
                //Debug.Log("Yo, el Jugador " + (iIndexJug + 1) + " comienzo a moverme hacia el totem");
                initLerpeadores(); //Por las dudas
                MesaManager.instance.CambiarEstadoToque(iIndexJug, true); //Avisamos que estamos buscando el totem
                lerpMov.Start(manoDer, totem.transform.position + corrimiento);
                iEstado++;
                break;
            case 1:
                if (lerpMov.Update())
                {
                    iEstado++;
                }
                break;
            case 2:
                //Animacion de cerrar mano
                //AnimarMano("agarrarTotem");
                lerpMovUp.Start(manoDer, manoDer.transform.position + Vector3.up * 0.2f);
                totemBehaviour.fijarTotemEnMano(manoDer.transform);
                iEstado++;
                break;
            case 3:
                if (lerpMovUp.Update())
                {
                    iEstado++;
                }
                break;
            case 4:
                lerpMovBack.Start(manoDer, posicionInicial[1]);
                iEstado++;
                break;
            case 5:
                if (lerpMovBack.Update())
                {
                    iEstado++;
                }
                break;
            default:
                //Se reincian todos los lerpeadores y vuelve al estado inicial
                iEstado = 0;
                initLerpeadores();
                MesaManager.instance.CambiarEstadoToque(iIndexJug, false); //Ya no esta buscando el totem
                break;
        }
    }

    /// <summary>
    /// Carga nuevos lerpeadores
    /// </summary>
    private void initLerpeadores()
    {
        lerpMov = new Lerpeador(fVelocidad);
        lerpMovBack = new Lerpeador(fVelocidad);
        lerpMovUp = new Lerpeador(fVelocidad / 2);
    }

    /*Quaternion ObtenerRotacion()
    {
        Quaternion rotacion = Quaternion.Euler(0, 0, 0);
        if (iIndexJug == 1)
        {
            rotacion = Quaternion.Euler(0, 0, 0);
        } else
        {
            rotacion = Quaternion.Euler(0, 90, 0);
        } 
        return rotacion;
    }*/

    void PonerManos()
    {
        float fDistancia = 0.08f; //Espacio contra el totem
        float fDistY = 0.08f;
        float fCorrimientoMano = 0.05f; //Evitar atravesar el totem
        switch (iIndexJug)
        {
            case 1:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position + new Vector3(0.35f, 0.1f, 0.09f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 0, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, 180, -90);
                corrimiento = new Vector3(fDistancia, fDistY, fCorrimientoMano);
                break;
            case 2:
                manoIzq.transform.position = totem.transform.position + new Vector3(0.09f, 0.1f, 0.35f);
                manoDer.transform.position = totem.transform.position + new Vector3(-0.09f, 0.1f, 0.35f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 270, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, -270, -90);
                corrimiento = new Vector3(-fCorrimientoMano, fDistY, fDistancia);
                break;
            case 3:
                manoIzq.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, -0.09f);
                manoDer.transform.position = totem.transform.position - new Vector3(0.35f, -0.1f, 0.09f);
                manoIzq.transform.rotation = Quaternion.Euler(0, 180, 90);
                manoDer.transform.rotation = Quaternion.Euler(0, 0, -90);
                corrimiento = new Vector3(-fDistancia, fDistY, -fCorrimientoMano);
                break;
            default:
                //Debug.Log("Esto no deberia pasar xdxd");
                break;
        }
    }

    /// <summary>
    /// Detiene la animacion luego de 3 segundos
    /// </summary>
    IEnumerator detenerAnimacion()
    {
        yield return new WaitForSeconds(3f);
        bAnimandose = false;
        Animator animator = manoDer.gameObject.GetComponent<Animator>();
        animator.enabled = false;
        animator.StopPlayback();
        animator.runtimeAnimatorController = null;
    }

    /// <summary>
    /// Permite animar la mano derecha
    /// </summary>
    /// <param name="sNomAnimacion">Nombre de la animacion que se desea ejecutar</param>
    void AnimarMano(string sNomAnimacion)
    {
        Animator animator = manoDer.gameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = contAnimacManos;
        animator.enabled = true;
        animator.Play(sNomAnimacion);
    }
}
