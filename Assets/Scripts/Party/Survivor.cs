using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]

public class Survivor : ScriptableObject
{

    public string Name;
    public int Damage;
    public int Health;
    public bool isCombatant;

    public string GetName()
    {
        return Name;
    }
    public int GetHealth()
    {
        return Health;
    }

    

    private int curHealth { get; set; }
    public Sprite Sprite;

    public Sprite GetSprite() { return Sprite; }
    



}
