using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Manager<PlayerData>
{
    public int Money { get; private set; }
    public int Blood { get; private set; }

    public System.Action OnUpdate;

    private void Start()
    {
        Money = 250;
        Blood = 0;
    }

    public bool CanAfford(int amount)
    {
        return amount <= Money;
    }

    public void SpendMoney(int amount)
    {
        Money -= amount;
        OnUpdate?.Invoke();
    }

    public bool CanPayBlood(int amount)
    {
        return amount <= Blood;
    }

    public void SpendBlood(int amount)
    {
        Blood -= amount;
        OnUpdate?.Invoke();
    }

    public void AddBlood(int amount)
    {
        Blood += amount;
        OnUpdate?.Invoke();
    }
}
