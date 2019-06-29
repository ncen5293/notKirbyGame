using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{

    public Vector3 Position { get; set; }
    public string Name { get; set; }
    public int Hp { get; set; }
    public int Power { get; set; }
    public int Weapon { get; set; }
    public int Durability { get; set; }
    public int Throwable { get; set; }
    
}
