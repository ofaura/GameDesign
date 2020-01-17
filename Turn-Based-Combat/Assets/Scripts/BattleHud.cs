using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider hpSlider;

    public void SetHUD(Adventurer unit)
    {
        nameText.text = unit.name;
        levelText.text = "Lvl " + unit.LvL;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }

    public void SetHP(float hp)
    {
        hpSlider.value = hp;
    }
}
