using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, CHOOSEMINION, ENEMYTURN, ENDTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    public GameObject[] playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public bool bot_vs_bot;
    public bool extracting_simulation_data; // If we are doing 1000 simulations to quickly get data, we don't want to wait between attacks & attack automatically
    public int nBattles;

    public Canvas endMenu;

    public Text battlesWonText;
    public Text battlesLostText;

    bool playerAttack = false;

    [HideInInspector]
    public int battlesWon = 0;
    [HideInInspector]
    public int battlesLost = 0;

    //Changed to adventurers since they have the health and manage the minions + we pack AI & reinitialization in 1 class
    [HideInInspector]
    public Adventurer playerUnit;
    Adventurer enemyUnit;

    public Text dialogueText;

    public BattleHud playerHUD;
    public BattleHud enemyHUD;

    public BattleState state;

    int index;

    public void StartBattle()
    {
        GameObject tmp = GameObject.Find("CanvasMinion");
        PrepareCombat minion = tmp.GetComponent<PrepareCombat>();
        index = minion.index;

        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        if (!extracting_simulation_data)
        {
            battlesWon = 0;
            battlesLost = 0;
        }

        GameObject playerGO = GameObject.Find("Player");
        playerUnit = playerGO.GetComponent<Adventurer>();
        playerUnit.currentHP = playerUnit.maxHP;

        GameObject enemyGO = GameObject.Find("Enemy");
        enemyUnit = enemyGO.GetComponent<Adventurer>();
        enemyUnit.currentHP = enemyUnit.maxHP;

        dialogueText.text = "A wild " + enemyUnit.name + " approaches...";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        if (extracting_simulation_data)
            yield return new WaitForSeconds(0f);

        else
            yield return new WaitForSeconds(2f);

        PlayerTurn();
    }

    void PlayerTurn()
    {
        state = BattleState.CHOOSEMINION;
        if (!bot_vs_bot)
            dialogueText.text = "Choose an action:"; 
        else
        {
            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerAttack());
        }
    }

    IEnumerator EndTurn()
    {
        playerUnit.RestMinion();
        enemyUnit.RestMinion();

        //Add resting rounds for minions
        ManageMinionRest(enemyUnit);
        ManageMinionRest(playerUnit);

        if (extracting_simulation_data)
            yield return new WaitForSeconds(0f);

        else
            yield return new WaitForSeconds(2f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    IEnumerator PlayerAttack()
    {
        if (bot_vs_bot && !playerAttack) //If the combat is simulated or spected by the player
        {
            enemyUnit.ChooseMinion();
            playerUnit.ChooseMinion(true);
        }

        if (playerUnit.selected_minion.speed >= enemyUnit.selected_minion.speed || playerAttack)
        {
            float damage_taken = enemyUnit.selected_minion.CalculateTakenDamage(playerUnit.selected_minion);
            enemyUnit.currentHP -= damage_taken;

            bool isDead = false;
            if (enemyUnit.currentHP <= 0)
                isDead = true;
            else
                isDead = false;

            enemyHUD.SetHP(enemyUnit.currentHP);
            dialogueText.text = playerUnit.selected_minion.name + ", attack!";


            if (extracting_simulation_data)
                yield return new WaitForSeconds(0f);

            else
                yield return new WaitForSeconds(2f);

            if (isDead)
            {
                state = BattleState.WON;
                enemyUnit.RestMinion();            

                EndBattle();
                yield break;
            }

            if (playerAttack)
            {
                state = BattleState.ENDTURN;
                StartCoroutine(EndTurn());
            }

            else
            {
                state = BattleState.ENEMYTURN;
                StartCoroutine(EnemyTurn());
            }

            playerAttack = false;

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

        if (extracting_simulation_data)
            yield return new WaitForSeconds(0f);

        else
            yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.name + " attacks!";

        if (extracting_simulation_data)
            yield return new WaitForSeconds(0f);

        else
            yield return new WaitForSeconds(1f);

        float damage_taken = playerUnit.selected_minion.CalculateTakenDamage(enemyUnit.selected_minion);
        playerUnit.currentHP -= damage_taken;

        bool isDead = false;
        if (playerUnit.currentHP <= 0)
            isDead = true;
        else
            isDead = false;

        playerHUD.SetHP(playerUnit.currentHP);

        if (extracting_simulation_data)
            yield return new WaitForSeconds(0f);

        else
            yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
            yield break;
        }

        if (enemyUnit.selected_minion.speed > playerUnit.selected_minion.speed)
        {
            playerAttack = true;
            state = BattleState.PLAYERTURN;
            StartCoroutine(PlayerAttack());
        }

        else
        {
            state = BattleState.ENDTURN;
            StartCoroutine(EndTurn());
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
                GameObject salty = GameObject.Find("SaltyButton");
                salty.GetComponent<Button>().interactable = true;
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
                GameObject salty = GameObject.Find("SourButton");
                salty.GetComponent<Button>().interactable = true;
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
                GameObject salty = GameObject.Find("SpicyButton");
                salty.GetComponent<Button>().interactable = true;
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
                GameObject salty = GameObject.Find("BitterButton");
                salty.GetComponent<Button>().interactable = true;
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
                GameObject salty = GameObject.Find("SweetButton");
                salty.GetComponent<Button>().interactable = true;
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
            battlesWon++;
        }
        else if (state == BattleState.LOST)
        {
            dialogueText.text = "You were defeated.";
            battlesLost++;
        }

        if (extracting_simulation_data && ((battlesLost + battlesWon) < nBattles))
        {
            playerUnit.RestMinion();
            enemyUnit.RestMinion();

            playerUnit.salty_minion.resting = false;
            playerUnit.salty_minion.rounds_resting = 0;
            playerUnit.salty_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.sweet_minion.resting = false;
            playerUnit.sweet_minion.rounds_resting = 0;
            playerUnit.sweet_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.sour_minion.resting = false;
            playerUnit.sour_minion.rounds_resting = 0;
            playerUnit.sour_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.spicy_minion.resting = false;
            playerUnit.spicy_minion.rounds_resting = 0;
            playerUnit.spicy_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.bitter_minion.resting = false;
            playerUnit.bitter_minion.rounds_resting = 0;
            playerUnit.bitter_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.salty_minion.resting = false;
            enemyUnit.salty_minion.rounds_resting = 0;
            enemyUnit.salty_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.sweet_minion.resting = false;
            enemyUnit.sweet_minion.rounds_resting = 0;
            enemyUnit.sweet_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.sour_minion.resting = false;
            enemyUnit.sour_minion.rounds_resting = 0;
            enemyUnit.sour_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.spicy_minion.resting = false;
            enemyUnit.spicy_minion.rounds_resting = 0;
            enemyUnit.spicy_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.bitter_minion.resting = false;
            enemyUnit.bitter_minion.rounds_resting = 0;
            enemyUnit.bitter_minion.transform.GetChild(1).gameObject.SetActive(false);

            StartCoroutine(SetupBattle());
        }
        else
        {
            playerUnit.RestMinion();
            enemyUnit.RestMinion();

            playerUnit.salty_minion.resting = false;
            playerUnit.salty_minion.rounds_resting = 0;
            playerUnit.salty_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.sweet_minion.resting = false;
            playerUnit.sweet_minion.rounds_resting = 0;
            playerUnit.sweet_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.sour_minion.resting = false;
            playerUnit.sour_minion.rounds_resting = 0;
            playerUnit.sour_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.spicy_minion.resting = false;
            playerUnit.spicy_minion.rounds_resting = 0;
            playerUnit.spicy_minion.transform.GetChild(1).gameObject.SetActive(false);

            playerUnit.bitter_minion.resting = false;
            playerUnit.bitter_minion.rounds_resting = 0;
            playerUnit.bitter_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.salty_minion.resting = false;
            enemyUnit.salty_minion.rounds_resting = 0;
            enemyUnit.salty_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.sweet_minion.resting = false;
            enemyUnit.sweet_minion.rounds_resting = 0;
            enemyUnit.sweet_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.sour_minion.resting = false;
            enemyUnit.sour_minion.rounds_resting = 0;
            enemyUnit.sour_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.spicy_minion.resting = false;
            enemyUnit.spicy_minion.rounds_resting = 0;
            enemyUnit.spicy_minion.transform.GetChild(1).gameObject.SetActive(false);

            enemyUnit.bitter_minion.resting = false;
            enemyUnit.bitter_minion.rounds_resting = 0;
            enemyUnit.bitter_minion.transform.GetChild(1).gameObject.SetActive(false);

            battlesWonText.text = "Battles Won: " + battlesWon.ToString();
            battlesLostText.text = "Battles Lost: " + battlesLost.ToString();

            endMenu.enabled = true;
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
