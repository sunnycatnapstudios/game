using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public float health = 10;
    public float attack = 1;
    public float speed = 1;

    public int nextActTurn;
    private bool dead = false;

    public void calculateNextActTurn(int currentTurn)
    {
        nextActTurn = currentTurn + (int)Math.Ceiling(100.0f / speed);
    }
    
    public int CompareTo (object otherStats)
    {
        return nextActTurn.CompareTo(((UnitStats)otherStats).nextActTurn);
    }

    public bool isDead()
    {
        return dead;
    }
}
