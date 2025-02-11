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

    private bool fed;

    public string GetName()
    {
        return Name;
    }
    public int GetHealth()
    {
        return Health;
    }

    public void SetFed()
    {
        fed = true;
    }
    public bool GetFed()
    {
        return fed;
    }
    

    private int curHealth { get; set; }
    public Sprite Sprite;

    public Sprite GetSprite() { return Sprite; }
    



}
