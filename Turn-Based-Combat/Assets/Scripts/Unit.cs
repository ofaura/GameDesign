using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;
    public int defense;
    public int speed;

    public float maxHP;
    public float currentHP;

    public bool TakeDamage(int dmg)
    {
        float dmgReduction = 100f / (100f + (float)(defense * defense));

        currentHP -= (dmg * dmgReduction);

        if (currentHP <= 0)
            return true;
        else
            return false;
    }
    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }
}
