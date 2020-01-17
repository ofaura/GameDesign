using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrepareCombat : MonoBehaviour
{
    public Canvas canvasHUD;
    public GameObject battleSystem;
    public GameObject enemyBattleStation;
    public GameObject playerBattleStation;

    public Transform combatPosition;

    public GameObject[] initialPositions;
    public GameObject[] prefabs;

    public int index;

    BattleSystem b;

    Adventurer playerUnit;

    GameObject playerGO;

    public void StartPrepareCombat()
    {
        GameObject go = GameObject.Find("Battle System");
        b = go.GetComponent<BattleSystem>();
        playerUnit = b.playerUnit;
    }

    public void SaltyButtonPressed()
    {
        if (b.state != BattleState.CHOOSEMINION)
            return;

        index = 0;
        playerUnit.selected_minion = playerUnit.salty_minion;
        SelectCard("SaltyButton");
    }

    public void SweetButtonPressed()
    {
        if (b.state != BattleState.CHOOSEMINION)
            return;

        index = 1;
        playerUnit.selected_minion = playerUnit.sweet_minion;
        SelectCard("SweetButton");
    }

    public void SourButtonPressed()
    {
        if (b.state != BattleState.CHOOSEMINION)
            return;

        index = 2;
        playerUnit.selected_minion = playerUnit.sour_minion;
        SelectCard("SourButton");
    }

    public void SpicyButtonPressed()
    {
        if (b.state != BattleState.CHOOSEMINION)
            return;

        index = 3;
        playerUnit.selected_minion = playerUnit.spicy_minion;
        SelectCard("SpicyButton");
    }

    public void BitterButtonPressed()
    {
        if (b.state != BattleState.CHOOSEMINION)
            return;

        index = 4;
        playerUnit.selected_minion = playerUnit.bitter_minion;
        SelectCard("BitterButton");
    }

    void ReadyForCombat()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        canvasHUD.GetComponent<Canvas>().enabled = true;
        battleSystem.SetActive(true);
    }

    void SelectCard(string taste)
    {
        GameObject tmp = GameObject.Find(taste);

        GameObject enemyGO = GameObject.Find("Enemy");
        Adventurer enemyUnit = enemyGO.GetComponent<Adventurer>();

        //We reveal the minion chosen by the enemy
        enemyUnit.ChooseMinion();

        prefabs[index].transform.GetChild(1).gameObject.SetActive(true);
        tmp.GetComponent<Button>().interactable = false;

        playerUnit.selected_minion.transform.position = playerBattleStation.transform.position;

        b.state = BattleState.PLAYERTURN;
    }
}
