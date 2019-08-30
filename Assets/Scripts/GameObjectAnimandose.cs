using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectAnimandose : ScriptableObject{
    private GameObject _gameObject;
    private float _timerAnimacion;
    private float _duracAnimacion;
    private TipoAnimacion _tipoAnimacion;

    //TODO: pasar todos los "new GameObjectAnimandose" a ScriptableObject.CreateInstance

    public GameObjectAnimandose(GameObject gameObject, float timerAnimacion, float duracAnimacion, TipoAnimacion tipoAnimacion)
    {
        _gameObject = gameObject;
        _timerAnimacion = timerAnimacion;
        _duracAnimacion = duracAnimacion;
        _tipoAnimacion = tipoAnimacion;
    }

    public GameObject GameObject
    {
        get
        {
            return _gameObject;
        }

        set
        {
            _gameObject = value;
        }
    }

    public float TimerAnimacion
    {
        get
        {
            return _timerAnimacion;
        }

        set
        {
            _timerAnimacion = value;
        }
    }

    public TipoAnimacion TipoAnimacion
    {
        get
        {
            return _tipoAnimacion;
        }

        set
        {
            _tipoAnimacion = value;
        }
    }

    public float DuracAnimacion
    {
        get
        {
            return _duracAnimacion;
        }

        set
        {
            _duracAnimacion = value;
        }
    }
}

public enum TipoAnimacion
{
    DesdeMazo,
    HaciaMazo, 
    DesdeTotem, 
    HaciaTotem
}