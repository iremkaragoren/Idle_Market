using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelperAreaDetector : InteractableBase
{
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private PlayerData_SO playerData;
    [SerializeField] private int neededMoney;
    [SerializeField] private float delaySeconds;

    private Coroutine coroutine;

    protected override void SendInteractableType()
    {
        base.SendInteractableType();
        AddNeededMoney();
    }

    protected override void InitilaizeDataToSend()
    {
        dataTosend = new TriggeredAreaData
        {
            areaType = Enums.AreaType.Money,
            productType = areaData.ProductType,
            productionList = new List<Transform>(),
            salesList = new List<Transform>()
        };
    }

    protected override void EndInteractableType()
    {
        base.EndInteractableType();
        StopMoneyCoroutine();
    }

    private void AddNeededMoney()
    {
        if (playerData.CurrentMoney > 0 && coroutine == null) 
        {
            coroutine = StartCoroutine(DecreaseNeededMoney());
        }
    }

    IEnumerator DecreaseNeededMoney()
    {
        while (neededMoney > 0)
        {
            neededMoney--;
            moneyText.text = neededMoney.ToString();
            InternalEvents.MoneyDecrease?.Invoke(1); 
            yield return new WaitForSeconds(delaySeconds);
        }

        if (neededMoney == 0)
        {
            InternalEvents.HelperNeededMoneyZero?.Invoke();
        }
       
    }
    private void StopMoneyCoroutine()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}
