using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyCounter : MonoBehaviour
{
    [SerializeField] private PlayerData_SO playerDataSO;
    [SerializeField] private TextMeshProUGUI moneyText;
    private int counter;

    private void OnEnable()
    {
        InternalEvents.MoneyListCount += OnMoneyListCount;
        InternalEvents.MoneyDecrease += OnMoneyDecrease;
    }

    private void OnDisable()
    {
        InternalEvents.MoneyListCount -= OnMoneyListCount;
        InternalEvents.MoneyDecrease -= OnMoneyDecrease;
    }

    private void OnMoneyDecrease(int newMoneyCount)
    {
        if (playerDataSO.CurrentMoney <= 0)
        {
            return;
        }

        playerDataSO.CurrentMoney -= newMoneyCount;
        
        if (playerDataSO.CurrentMoney < 0)
        {
            playerDataSO.CurrentMoney = 0;
        }

        moneyText.text = playerDataSO.CurrentMoney.ToString();
    }

    private void OnMoneyListCount(int moneyCount)
    {
        counter += moneyCount;
        moneyText.text = counter.ToString();
        playerDataSO.CurrentMoney = counter;
        counter = 0;
    }
}
