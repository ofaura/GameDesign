using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject[] playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public bool bot_vs_bot;
    public bool extracting_simulation_data; // If we are doing 1000 simulations to quickly get data, we don't want to wait between attacks & attack automatically

    //Unit playerUnit;
    //Unit enemyUnit;

    //Changed to adventurers since they have the health and manage the minions + we pack AI & reinitialization in 1 class
    Adventurer playerUnit;
    Adventurer enemyUnit;

    public Text dialogueText;

    public BattleHud playerHUD;
    public BattleHud enemyHUD;

    public BattleState state;

    int index;
    // Start is called before the first frame update
    void Start()
    {
        GameObject tmp = GameObject.Find("CanvasMinion");
        PrepareCombat minion = tmp.GetComponent<PrepareCombat>();
        index = minion.index;

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        //GameObject playerGO = Instantiate(playerPrefab[index], playerBattleStation);
        GameObject playerGO = GameObject.Find("Player");
        playerUnit = playerGO.GetComponent<Adventurer>();

        //GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
        GameObject enemyGO = GameObject.Find("Enemy");
        enemyUnit = enemyGO.GetComponent<Adventurer>();

        dialogueText.text = "A wild " + enemyUnit.name + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";

        //Add resting rounds for minions
        ManageMinionRest(enemyUnit);
        ManageMinionRest(playerUnit);
    }

    IEnumerator PlayerAttack()
    {
        //We reveal the minion chosen by the enemy
        enemyUnit.ChooseMinion();
        if (bot_vs_bot) //If the combat is simulated or spected by the player
        {
            playerUnit.ChooseMinion();

        }

        float damage_taken = enemyUnit.selected_minion.CalculateTakenDamage(playerUnit.selected_minion);
        enemyUnit.currentHP -= damage_taken;

        bool isDead = false;
        if (enemyUnit.currentHP <= 0)
            isDead = true;
        else
            isDead = false;

        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogueText.text = "The attack is successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerHeal()
    {
        playerUnit.selected_minion.Heal(5);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogueText.text = "You feel renewed strength!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.name + " attacks!";

        yield return new WaitForSeconds(1f);

        float damage_taken = playerUnit.selected_minion.CalculateTakenDamage(enemyUnit.selected_minion);
        playerUnit.currentHP -= damage_taken;

        bool isDead = false;
        if (enemyUnit.currentHP <= 0)
            isDead = true;
        else
            isDead = false;

        playerHUD.SetHP(playerUnit.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }

        
    }

    public void ManageMinionRest(Adventurer unit)
    {
        //Add resting rounds for minions
        if (unit.salty_minion.resting)
        {
            if (unit.salty_minion.rounds_resting >= unit.salty_minion.required_rounds_to_rest)
            {
                unit.salty_minion.resting = false;
                unit.salty_minion.rounds_resting = 0;
                unit.salty_minion.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
                unit.salty_minion.rounds_resting++;
        }
        if (unit.sour_minion.resting)
        {
            if (unit.sour_minion.rounds_resting >= unit.sour_minion.required_rounds_to_rest)
            {
                unit.sour_minion.resting = false;
                unit.sour_minion.rounds_resting = 0;
                unit.sour_minion.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
                unit.sour_minion.rounds_resting++;
        }
        if (unit.spicy_minion.resting)
        {
            if (unit.spicy_minion.rounds_resting >= unit.spicy_minion.required_rounds_to_rest)
            {
                unit.spicy_minion.resting = false;
                unit.spicy_minion.rounds_resting = 0;
                unit.spicy_minion.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
                unit.spicy_minion.rounds_resting++;
        }
        if (unit.bitter_minion.resting)
        {
            if (unit.bitter_minion.rounds_resting >= unit.bitter_minion.required_rounds_to_rest)
            {
                unit.bitter_minion.resting = false;
                unit.bitter_minion.rounds_resting = 0;
                unit.bitter_minion.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
                unit.bitter_minion.rounds_resting++;
        }
        if (unit.sweet_minion.resting)
        {
            if (unit.sweet_minion.rounds_resting >= unit.sweet_minion.required_rounds_to_rest)
            {
                unit.sweet_minion.resting = false;
                unit.sweet_minion.rounds_resting = 0;
                unit.sweet_minion.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
                unit.sweet_minion.rounds_resting++;
        }
    }

    void EndBattle()
    {
        if (state == BattleState.WON)
        {
            dialogueText.text = "You won the battle!";
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
        }
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerHeal());
    }

}
