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

    GameObject playerGO;

    int currentCards = 0;

    public void SaltyButtonPressed()
    {
        index = 0;
        SelectCard("SaltyButton");
        ///ReadyForCombat();
    }

    public void SweetButtonPressed()
    {
        index = 1;
        SelectCard("SweetButton");
       // ReadyForCombat();
    }

    public void SourButtonPressed()
    {
        index = 2;
        SelectCard("SourButton");
    }

    public void SpicyButtonPressed()
    {
        index = 3;
        SelectCard("SpicyButton");
    }

    public void BitterButtonPressed()
    {
        index = 4;
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

        GameObject station = GameObject.Find("PlayerBattleStation");
        Transform[] children = station.GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.name != "PlayerBattleStation")
                Destroy(child.gameObject);
        }

        GameObject salty = GameObject.Find("SaltyButton");
        salty.GetComponentInChildren<Image>().enabled = true;
        salty.GetComponent<Button>().interactable = true;
        GameObject sweet = GameObject.Find("SweetButton");
        sweet.GetComponentInChildren<Image>().enabled = true;
        sweet.GetComponent<Button>().interactable = true;
        GameObject sour = GameObject.Find("SourButton");
        sour.GetComponentInChildren<Image>().enabled = true;
        sour.GetComponent<Button>().interactable = true;
        GameObject spicy = GameObject.Find("SpicyButton");
        spicy.GetComponentInChildren<Image>().enabled = true;
        spicy.GetComponent<Button>().interactable = true;
        GameObject bitter = GameObject.Find("BitterButton");
        bitter.GetComponentInChildren<Image>().enabled = true;
        bitter.GetComponent<Button>().interactable = true;

        playerGO = Instantiate(prefabs[index], combatPosition);
        tmp.GetComponentInChildren<Image>().enabled = false;
        tmp.GetComponent<Button>().interactable = false;
    }
}
