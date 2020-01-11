using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum Flavor {
        SALTY = 0,
        SOUR,
        SPICY,
        BITTER,
        SWEET,
    }

    enum Advantadge {
        DISADVANTAGE = 0,
        NEUTRAL,
        ADVANTAGE,
    }
    public string unitName;
    public int unitLevel;
    public Flavor unitFlavor;  // This type determines Type Modifications according to the pentagram of advantages

    public int attack;
    public int defense;
    public int speed;

    public float maxHP;
    public float currentHP;
    public bool resting = false;
    public uint rounds_resting = 0;
    public uint required_rounds_to_rest = 2;

    float[] advantageMultiplier = new float[] { 0.75f, 1.0f, 1.5f };

    private void Start()
    {
        RefreshUnitStats();
    }

    public float CalculateTakenDamage(Unit enemy)
    {
        //Actual Function we use to calculate damage
        Advantadge modifierIndex = CalculateTypeAdvantage(enemy.unitFlavor);
        float damage = ((2 * enemy.unitLevel / 10 + 2) * (enemy.attack / this.defense) / 50 + 2) * advantageMultiplier[(int)modifierIndex] * Random.Range(0.85f, 1.0f);

        return damage;
    }


    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    public void ChangeLevel(float new_level)
    {
        unitLevel = (int)new_level;
        RefreshUnitStats(); //We need to update our stats since we leveled up
    }


    void RefreshUnitStats()
    {
        int stackBonuses = 0; //How many level based stack bonuses we will apply, in this case a stack is granted each 5 levels
        stackBonuses = (int) (unitLevel / 5);

        int levelMultiplier = (int) unitLevel - 1; // for all levels after 1 multypling this number by the bonus + adding the stack bonuses will be enough
        switch (unitFlavor)
        {
            case Flavor.SALTY:
                attack = 5 + levelMultiplier * 1 + stackBonuses * 2;
                defense = 11 + levelMultiplier * 3;
                speed = 8 + levelMultiplier * 2;
            break;

            case Flavor.SWEET:
                attack = 8 + levelMultiplier * 2 + stackBonuses * 1;
                defense = 8 + levelMultiplier * 2 + stackBonuses * 1;
                speed = 7 + levelMultiplier * 2;
                break;

            case Flavor.SOUR:
                attack = 6 + levelMultiplier * 2 + stackBonuses * 1;
                defense = 8 + levelMultiplier * 2 + stackBonuses * 1;
                speed = 10 + levelMultiplier * 2 + stackBonuses * 1;
                break;

            case Flavor.SPICY:
                attack = 9 + levelMultiplier * 3;
                defense = 7 + levelMultiplier * 1 + stackBonuses * 2;
                speed = 5 + levelMultiplier * 2;
                break;

            case Flavor.BITTER:
                attack = 10 + levelMultiplier * 3 + stackBonuses * 2;
                defense = 6 + levelMultiplier * 1 ;
                speed = 7 + levelMultiplier * 2;
                break;
        }
    }

    Advantadge CalculateTypeAdvantage(Flavor enemy_flavor)
    {
        Advantadge ret = Advantadge.NEUTRAL;

        //Using our flavour, and calculate wheter the enemy has advantage or disadvantage against us
        Flavor advantage1 = this.unitFlavor + 2;
        if ((int)advantage1 > 4)
            advantage1 -= 5;

        Flavor advantage2 = this.unitFlavor + 4;
        if ((int)advantage2 > 4)
            advantage2 -= 5;

        Flavor disadvantage1 = this.unitFlavor + 1;
        if ((int)disadvantage1 > 4)
            disadvantage1 -= 5;

        Flavor disadvantage2 = this.unitFlavor + 3;
        if ((int)disadvantage2 > 4)
            disadvantage2 -= 5;

        //Determine Advantage or disadvantage
        if (this.unitFlavor == enemy_flavor) //Enemy is of our same flavor
            ret = Advantadge.NEUTRAL;
        else if (enemy_flavor == advantage1 || enemy_flavor == advantage2) //The enemy has advantage on us
            ret = Advantadge.ADVANTAGE;
        else if (enemy_flavor == disadvantage1 || enemy_flavor == disadvantage2) //The enemy has disadvantage against us
            ret = Advantadge.DISADVANTAGE;

        return ret;
    }
}
