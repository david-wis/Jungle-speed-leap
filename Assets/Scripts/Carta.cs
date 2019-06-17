using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CartaX", menuName ="Carta")]
public class Carta : ScriptableObject {
    //public new string name;
    public GameObject img3D;
    public Sprite img2D;
    public Color color;
    public int forma;

    public enum Color
    {
        Verde, 
        Amarillo,
        Rojo,
        Violeta
    }

    override public string ToString()
    {
        string txt = color.ToString() + "-" + forma;
        return txt;
    }
}
