using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : MonoBehaviour
{
    //Minions
    public Unit salty_minion;
    public Unit sour_minion;
    public Unit spicy_minion;
    public Unit bitter_minion;
    public Unit sweet_minion;

    public Unit selected_minion = null;

    public string name;
    public int maxHP = 30;
    public float currentHP;
    public uint LvL = 1;
    // Start is called before the first frame update
    void Start()
    {
        ReStartAdventurer();
    }

    void ReStartAdventurer()
    {
        currentHP = maxHP;

        //Restart minion levels
        salty_minion.ChangeLevel(LvL);
        sour_minion.ChangeLevel(LvL);
        spicy_minion.ChangeLevel(LvL);
        bitter_minion.ChangeLevel(LvL);
        sweet_minion.ChangeLevel(LvL);
    }

    public void ChooseMinion()
    {
        int minion = Random.Range(0, 4);

        switch ((Unit.Flavor)minion)
        {
            case Unit.Flavor.SALTY:
                selected_minion = salty_minion;
            break;

            case Unit.Flavor.SOUR:
                selected_minion = sour_minion;
            break;

            case Unit.Flavor.SPICY:
                selected_minion = spicy_minion;
            break;

            case Unit.Flavor.BITTER:
                selected_minion = bitter_minion;
            break;

            case Unit.Flavor.SWEET:
                selected_minion = sweet_minion;
            break;
        }
    }


}
