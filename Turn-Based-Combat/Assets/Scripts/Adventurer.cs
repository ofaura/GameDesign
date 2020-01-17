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

    //We save the minions initial transform positions here
    Vector3[] minion_positions;

    public Unit selected_minion = null;

    public string name;
    public int maxHP = 30;
    public float currentHP;
    public uint LvL = 1;

    //Battle Station to deploy selected minion, for now only used by enemy
    public Transform BattleStation;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        //Save minions positions
        minion_positions = new Vector3[]{ salty_minion.transform.position, sour_minion.transform.position,
            spicy_minion.transform.position, bitter_minion.transform.position, sweet_minion.transform.position};

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

    public void RestMinion()
    {
        if (selected_minion != null)
        {
            // Make the selected minion go back to its position & apply resting condition
            int index = 0;
            if (selected_minion.unitName == "Salty")
                index = 0;
            else if (selected_minion.unitName == "Sweet")
                index = 4;
            else if (selected_minion.unitName == "Sour")
                index = 1;
            else if (selected_minion.unitName == "Spicy")
                index = 2;
            else if (selected_minion.unitName == "Bitter")
                index = 3;

            selected_minion.transform.position = minion_positions[index];
            selected_minion.resting = true;
            selected_minion.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void ChooseMinion(bool player = false,  bool force_flavor = false, Unit.Flavor flavor = Unit.Flavor.SALTY)
    {
        int minion = Random.Range(0, 5);

        if (force_flavor)
            minion = (int)flavor;

        if (selected_minion != null)
        {
            // Make the selected minion go back to its position & apply resting condition
            if (!player)
                selected_minion.transform.position = minion_positions[(int)selected_minion.unitFlavor];
            selected_minion.resting = true;
            selected_minion.transform.GetChild(1).gameObject.SetActive(true);
        }

        bool minion_selectable = false;
        while (minion_selectable == false)
        {
            switch ((Unit.Flavor)minion)
            {
                case Unit.Flavor.SALTY:
                    if (salty_minion.resting == false)
                    {
                        selected_minion = salty_minion;
                        minion_selectable = true;
                    }
                    break;

                case Unit.Flavor.SOUR:
                    if (sour_minion.resting == false)
                    { 
                        selected_minion = sour_minion;
                        minion_selectable = true;
                    }
                    break;

                case Unit.Flavor.SPICY:
                    if (spicy_minion.resting == false)
                    {
                        selected_minion = spicy_minion;
                        minion_selectable = true;
                    }
                        break;

                case Unit.Flavor.BITTER:
                    if (bitter_minion.resting == false)
                    {
                        selected_minion = bitter_minion;
                        minion_selectable = true;
                    }
                        break;

                case Unit.Flavor.SWEET:
                    if (sweet_minion.resting == false)
                    {
                        selected_minion = sweet_minion;
                        minion_selectable = true;
                    }
                        break;
            }

            //We rerandomize the number, if the while were to continue we will test with that number,
            // if we have already selected a minion changing the random number here is trivial and won't affect the outcome
            minion = Random.Range(0,5);
        }

        selected_minion.transform.position = BattleStation.transform.position + offset;
    }
}
