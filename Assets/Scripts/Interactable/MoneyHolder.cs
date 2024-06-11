using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class MoneyDetector : MonoBehaviour
{

    public List<Transform> moneyList;
    
    [Button]
    public void GetAllChildren()
    {
        for (int i = 0; i <transform.childCount; i++)
        {
            moneyList.Add(transform.GetChild(i));
        }
    }

    private void OnEnable()
    {
        ExternalEvents.PlayButtonClicked += OnPlayButtonClicked;
        InternalEvents.TuttorialMoneyTriggered += OnTutorialMoneyTriggerd;
    }

    private void OnDisable()
    {
        ExternalEvents.PlayButtonClicked -= OnPlayButtonClicked;
        InternalEvents.TuttorialMoneyTriggered -= OnTutorialMoneyTriggerd;
    }

    private void OnPlayButtonClicked()
    {
        foreach (var  money in moneyList)
        {
          money.gameObject.SetActive(true);
        }
    }

    private void OnTutorialMoneyTriggerd()
    {
        foreach (Transform money in moneyList)
        {
           
            money.DOMoveY(money.position.y + 10, 1.0f) 
                .OnComplete(() => Destroy(money.gameObject)); 

            money.DOScale(Vector3.zero, 1.0f); 
        }
        InternalEvents.MoneyListCount?.Invoke(moneyList.Count*3);
        Destroy(gameObject);
    }
    
    
}
