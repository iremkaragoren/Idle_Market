using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CheckoutInteractable : InteractableBase
{
    [SerializeField] private float spaceBetweenAIs;
    
    internal List<Transform> salesList = new List<Transform>();
    private List<AIController> aiQueue = new List<AIController>();
    public Transform cashierPoint;
    

    private void OnEnable()
    {
        InternalEvents.PlayerTriggeredArea += OnProductAreaTriggered;
    }
    
    private void OnDisable()
    {
        InternalEvents.PlayerTriggeredArea -= OnProductAreaTriggered;
    }


    private void OnProductAreaTriggered(TriggeredAreaData triggeredAreaData)
    {
        
        if (triggeredAreaData.areaType == Enums.AreaType.Checkout)
        {
            
            if (aiQueue.Count >0 &&aiQueue[0].isProductInBox)
            {
              
                aiQueue[0].AllProductMoved();
                RemoveAI();
                
            }

            if (salesList.Count > 0)
            {
                foreach (Transform money in salesList)
                {
                    if (money != null && money.gameObject != null)
                    {
                        Sequence seq = DOTween.Sequence();
                        
                        seq.Append(money.DOMoveY(money.position.y + 10, 1.0f).SetEase(Ease.OutQuad));
                        seq.Append(money.DOScale(Vector3.zero, 1.0f).SetEase(Ease.InOutQuad));
                        seq.OnComplete(() =>
                        {
                            if (money != null) Destroy(money.gameObject);
                        });
                        
                        seq.Play();
                    }

                    InternalEvents.MoneyListCount?.Invoke(salesList.Count);

                }
            }
        }
    }

    

    public void AddedAI(AIController aiController)
    {
        aiQueue.Add(aiController);
        
        UpdateQueuePositions(); 
    }
    
    private void UpdateQueuePositions()
    {
        for (int i = 0; i < aiQueue.Count; i++)
        {
            Vector3 queuePosition = new Vector3(
                cashierPoint.position.x,
                cashierPoint.position.y,
                cashierPoint.position.z + i * spaceBetweenAIs);

            aiQueue[i].linePosition = queuePosition;
        }
        
    }
    
    public void RemoveAI()
    {
        if (aiQueue.Count > 0)
        {
            aiQueue.RemoveAt(0);
            UpdateQueuePositions(); 
        }
    }
    
    
    public override TriggeredAreaData AIInteract(AIController newAI)
    {
        if (aiQueue.Contains(newAI) && aiQueue.IndexOf(newAI)==0)
        {
            aiQueue[0].ProcessItem();
        }
        
        return dataTosend;
    }
    
    protected override void InitilaizeDataToSend()
    {
        dataTosend = new TriggeredAreaData
        {
            areaType = Enums.AreaType.Checkout,
            productType = areaData.ProductType,
            productionList = null,
            salesList =salesList
        };
        

    }
}
