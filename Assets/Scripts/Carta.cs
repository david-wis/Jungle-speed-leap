using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CartaX", menuName ="Carta")]
public class Carta : ScriptableObject {
    //public new string name;
    public GameObject img;
    public Color color;
    public int Forma;

    public enum Color
    {
        Verde, 
        Amarillo,
        Rojo,
        Violeta
    }
}
