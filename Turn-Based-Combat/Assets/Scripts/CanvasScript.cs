using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    public Image chart;
    public InputField numberOfBattles;

    GameObject go;
    BattleSystem battle;

    public Canvas mainMenu;
    public Canvas endMenu;

    private void Awake()
    {
        go = GameObject.Find("Battle System");
        battle = go.GetComponent<BattleSystem>();
    }

    public void FlavorDominanceChart()
    {
        if (chart.gameObject.activeSelf)
            chart.gameObject.SetActive(false);

        else
            chart.gameObject.SetActive(true);
    }

    public void PlayerVsAI()
    {
        battle.extracting_simulation_data = false;
        battle.bot_vs_bot = false;
        StartCombat();
    }

    public void AIvsAI()
    {
        battle.extracting_simulation_data = false;
        battle.bot_vs_bot = true;
        StartCombat();
    }

    public void BattleSimulator()
    {
        battle.extracting_simulation_data = true;
        battle.bot_vs_bot = true;
        battle.nBattles = int.Parse(numberOfBattles.text);
        battle.battlesWon = 0;
        battle.battlesLost = 0;
        StartCombat();
    }

    public void MainMenu()
    {
        endMenu.enabled = false;
        mainMenu.enabled = true;
    }

    void StartCombat()
    {
        battle.StartBattle();
        GameObject tmp = GameObject.Find("CanvasMinion");
        PrepareCombat combat = tmp.GetComponent<PrepareCombat>();
        combat.StartPrepareCombat();

        endMenu.enabled = false;
        mainMenu.enabled = false;
    }
}
