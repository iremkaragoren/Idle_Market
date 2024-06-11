using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStackHandler : MonoBehaviour
{
    [SerializeField] private PlayerAudioHandler playerAudioHandler;
    [SerializeField] private PlayerData_SO playerData;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float productSize = 0.2f;
    [SerializeField] private float detectionRadius; 
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform wastePoint;
    

    private Dictionary<Enums.ProductType,int> productCounts = new Dictionary<Enums.ProductType, int>();
    private List<Transform> movedProducts = new List<Transform>();
    private List<Transform> addedItem;
   

    private void Awake()
    {
        InternalEvents.PlayerTriggeredArea += OnProductAreaTriggered;
        InternalEvents.WasteTriggered += OnWasteTriggered;
    }

    private void OnDisable()
    {
        InternalEvents.PlayerTriggeredArea -= OnProductAreaTriggered;
        InternalEvents.WasteTriggered -= OnWasteTriggered;
    }

    private void OnWasteTriggered()
    {
        MoveProductsToWastePoint();
    }

    private void OnProductAreaTriggered(TriggeredAreaData triggeredAreaData)
    {
       
        if (triggeredAreaData.areaType == Enums.AreaType.Manufacture)
        {
            TakeSalesProducts(triggeredAreaData);
        }

        if(triggeredAreaData.areaType==Enums.AreaType.MiniFarm)
        {
            MiniFarmTriggered(triggeredAreaData);
        }

        if (triggeredAreaData.areaType== Enums.AreaType.Aisle)
        {
            if(triggeredAreaData.productType==playerData.CurrentProductType)
                DropProducts(triggeredAreaData);
           
        }

        if (triggeredAreaData.areaType == Enums.AreaType.Canned)
        {
            CannedProductionTriggered(triggeredAreaData);
        }
        
    }

  
    private void MiniFarmTriggered(TriggeredAreaData triggeredAreaData)
    {
        var eggPoints = triggeredAreaData.salesList;
        
        
        if (movedProducts.Count >0 && playerData.CurrentProductType==Enums.ProductType.Tomato)
        {
            int dropeedItemCount = movedProducts.Count;
            DropProducts(triggeredAreaData);
            InternalEvents.ChickenFeedDropped?.Invoke(dropeedItemCount);
        }
        
        else if (eggPoints.Count>0)
        {
           TakeSalesProducts(triggeredAreaData);
        }
        
    }
    

    private void TakeSalesProducts(TriggeredAreaData triggeredAreaData)
    {
        int currentProductCount = playerData.ProductCount;
      
        if (currentProductCount < playerData.MaxProductCount)
        {
            int canTake = Mathf.Min(playerData.MaxProductCount - currentProductCount, 5 - currentProductCount);
        
            if (canTake > 0)
            {
                TakeProducts(canTake, triggeredAreaData);
            }
        }
    }


    private void TakeProducts(int canTake, TriggeredAreaData triggeredAreaData)
    {
        playerAudioHandler.PlayTakingSound();
        var productList = triggeredAreaData.salesList;
        var productType = triggeredAreaData.productType;
        
        int countToMove = Mathf.Min(canTake, productList.Count);
        float currentYOffset = 0;

        for (int i = 0; i < countToMove; i++)
        {
            Transform productToMove = productList[i];
            productToMove.transform.DOMove(targetPoint.position, 1f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    productToMove.transform.SetParent(this.transform);
                    productToMove.transform.localPosition = new Vector3(0, currentYOffset, 0);
                    currentYOffset += productSize;
                });
            movedProducts.Add(productToMove);
        }
        
        productCounts[productType] = 0;
        playerData.ProductCount += countToMove;
        playerData.CurrentProductType = productType;
        InternalEvents.PlayerTakeProducts?.Invoke(countToMove,productType);

      
    }
    
    private void CannedProductionTriggered(TriggeredAreaData triggeredAreaData)
    {
        var cannedPoints = triggeredAreaData.salesList;
        
        if (movedProducts.Count > 0 && playerData.CurrentProductType == Enums.ProductType.Tomato)
        {
            int itemCount = movedProducts.Count;
            DropProducts(triggeredAreaData);
            InternalEvents.CannedTomatoDropped?.Invoke(itemCount);
        }
        
    }


    private void DropProducts(TriggeredAreaData triggeredAreaData)
    {
        var productPoints = triggeredAreaData.productionList;
        
        if (productPoints.Count == 0 || movedProducts.Count == 0)
            return;
        
    
        int productIndex = 0;
        
        addedItem = new List<Transform>();
    
        playerAudioHandler.PlayDroppingSound();
        
        foreach (Transform point in productPoints)
        {
            if (point.childCount == 0) 
            {
                    if (productIndex < movedProducts.Count)
                    {
                        Transform product = movedProducts[productIndex++];
                        product.DOMove(point.position, 1f).OnComplete(() => {
                            product.SetParent(point);
                        });
                        
                        addedItem.Add(product);
                    }
                    else
                    {
                        break; 
                    }
                    
            }
            
        }
        
        
        if (playerData.CurrentProductType == triggeredAreaData.productType)
        {
            InternalEvents.ProductPoints?.Invoke(addedItem, triggeredAreaData.productType);
        }
       
        if (productIndex < movedProducts.Count)
        {
            Debug.Log("Not all products could be placed; some points were already full.");
        }

        movedProducts.RemoveRange(0, productIndex);
        playerData.ProductCount -= productIndex;
        
        if (playerData.ProductCount == 0)
        {
            playerData.CurrentProductType = Enums.ProductType.None;
        }
       
    }
    
    
    private void MoveProductsToWastePoint()
    {
        foreach (Transform product in movedProducts)
        {
            product.DOMove(wastePoint.position, .2f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    product.DOScale(Vector3.zero, .2f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            Destroy(product.gameObject);
                        });
                });
        }

        movedProducts.Clear();
        playerData.ProductCount = 0;
        playerData.CurrentProductType = Enums.ProductType.None;
    }

}
