using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    
    [SerializeField] private int health;
    [SerializeField] private int healthMax;
    [SerializeField] private int attack;
    [SerializeField] private int speed;

    private bool dead = false;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            dead = true;
        }
    }

    public void HealHealth(int heal)
    {
        health += heal;
        if (health > healthMax)
        {
            health = healthMax;
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetHealthPercentage()
    {
        return (float) health / healthMax;
    }
    public int GetAttack()
    {
        // Vary attack amount by 75% to 125%
        // TODO removing randomess
        int randomAttack = attack; //+ UnityEngine.Random.Range(-attack/4, attack/4);
        return randomAttack;
    }
    
    public int GetSpeed()
    {
        return speed;
    }

    public void SetHealth(int health)
    {
        this.health = health;
    }

    public void SetAttack(int attack)
    {
        this.attack = attack;
    }

    public void SetSpeed(int speed)
    {
        this.speed = speed;
    }

    public bool IsDead()
    {
        return dead;
    }
}
