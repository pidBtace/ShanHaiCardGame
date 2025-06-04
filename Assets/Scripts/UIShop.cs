using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public List<UICard> allCards;
    public Text money;
    public Text blood;

    private EntitiesDatabaseSO cachedDb;
    private int refreshCost = 1;

    private void Start()
    {
        cachedDb = GameManager.Instance.entitiesDatabase;
        GenerateCard();
        PlayerData.Instance.OnUpdate += Refresh;
        Refresh();
    }

    public void GenerateCard()
    {
        for(int i = 0; i < allCards.Count; i++)
        {
            if (!allCards[i].gameObject.activeSelf)
                allCards[i].gameObject.SetActive(true);

            allCards[i].Setup(cachedDb.allEntities[Random.Range(0, cachedDb.allEntities.Count)], this);
        }
    }

    public void OnCardClick(UICard card, EntitiesDatabaseSO.EntityData cardData)
    {
        if (!PlayerData.Instance.CanAfford(cardData.cost))
            return;

        if (!PlayerData.Instance.CanPayBlood(cardData.bloodCost))
        {
            int need = cardData.bloodCost - PlayerData.Instance.Blood;
            var units = GameManager.Instance.GetTeamEntities(Team.Team1);
            while (need > 0 && units.Count > 0)
            {
                GameManager.Instance.SacrificeUnit(units[0]);
                need--;
                units = GameManager.Instance.GetTeamEntities(Team.Team1);
            }

            if (!PlayerData.Instance.CanPayBlood(cardData.bloodCost))
                return;
        }

        PlayerData.Instance.SpendMoney(cardData.cost);
        PlayerData.Instance.SpendBlood(cardData.bloodCost);
        card.gameObject.SetActive(false);
        GameManager.Instance.OnEntityBought(cardData, 0);
    }

    public void OnRefreshClick()
    {
        //Decrease money 
        if(PlayerData.Instance.CanAfford(refreshCost))
        {
            PlayerData.Instance.SpendMoney(refreshCost);
            GenerateCard();
        }
    }

    void Refresh()
    {
        money.text = PlayerData.Instance.Money.ToString();
        if (blood != null)
            blood.text = PlayerData.Instance.Blood.ToString();
    }
}
