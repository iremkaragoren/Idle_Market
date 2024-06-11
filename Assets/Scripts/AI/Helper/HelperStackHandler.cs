using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class HelperStackHandler : MonoBehaviour
{
    [SerializeField] private PlayerData_SO helperData;
    [SerializeField] private float productSize = 0.2f;
    [SerializeField] private Transform targetPoint;
    [SerializeField] private HelperTargetHandler targetHandler;
  
    private Dictionary<Enums.ProductType,int> productCounts = new Dictionary<Enums.ProductType, int>();
   
    private List<Transform> movedProducts = new List<Transform>();
    private List<Transform> addedItem;

    internal bool canPickAnim;
    internal bool isMiniFarmSelected;
    private void Awake()
    {
        InternalEvents.HelperTriggeredArea += OnProductAreaTriggered;
    }

    private void OnDisable()
    {
        InternalEvents.HelperTriggeredArea -= OnProductAreaTriggered;

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
        
        if (isMiniFarmSelected)
        {
            if (eggPoints.Count > 0)
            {
                TakeSalesProducts(triggeredAreaData);
            }
            isMiniFarmSelected = false;
        }
        else if (movedProducts.Count > 0 && helperData.CurrentProductType == Enums.ProductType.Tomato)
        {
            int droppedItemCount = movedProducts.Count;
            DropProducts(triggeredAreaData);
            InternalEvents.ChickenFeedDropped?.Invoke(droppedItemCount);
        } 
    }
    
    private void CannedProductionTriggered(TriggeredAreaData triggeredAreaData)
    {
        var cannedPoints = triggeredAreaData.salesList;
        
        if (movedProducts.Count > 0 && helperData.CurrentProductType == Enums.ProductType.Tomato)
        {
            int ıtemCount = movedProducts.Count;
            DropProducts(triggeredAreaData);
            InternalEvents.CannedTomatoDropped?.Invoke(ıtemCount);
        }
        
    }
    
   private void TakeSalesProducts(TriggeredAreaData triggeredAreaData)
    {
        int currentProductCount = helperData.ProductCount;
    
      if (currentProductCount < helperData.MaxProductCount)
      { 
          int canTake = Mathf.Min(helperData.MaxProductCount - currentProductCount, 5 - currentProductCount);
        
         if (canTake > 0)
         {
            TakeProducts(canTake, triggeredAreaData);
         }
      }
    }

    
    private void TakeProducts(int canTake, TriggeredAreaData triggeredAreaData)
    {
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

       
        if (!productCounts.ContainsKey(productType))
        {
            productCounts[productType] = 0;
        }
        productCounts[productType] += countToMove;
        helperData.ProductCount += countToMove;
        helperData.CurrentProductType = productType;
        InternalEvents.PlayerTakeProducts?.Invoke(countToMove, productType);
        canPickAnim = true;
    }

    public void DropProductToBin(Transform wasteBin)
    {
        foreach (Transform product in movedProducts)
        {
            product.DOMove(wasteBin.position, .2f)
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
        helperData.ProductCount = 0;
        helperData.CurrentProductType = Enums.ProductType.None;
    }
    
    private void DropProducts(TriggeredAreaData triggeredAreaData)
    {
        
        var productPoints = triggeredAreaData.productionList;
        
        if (productPoints.Count == 0 || movedProducts.Count == 0)
            return;
        
        int productIndex = 0;
        
        addedItem = new List<Transform>();
    

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
        
        
        if (helperData.CurrentProductType == triggeredAreaData.productType)
        {
            InternalEvents.ProductPoints?.Invoke(addedItem, triggeredAreaData.productType);
        }

        canPickAnim = false;
        if (productIndex < movedProducts.Count)
        {
            InternalEvents.AllDropPointFull?.Invoke();
        }

        movedProducts.RemoveRange(0, productIndex);
        helperData.ProductCount -= productIndex;
        if (helperData.ProductCount == 0)
        {
            helperData.CurrentProductType = Enums.ProductType.None;
        }
        
       
    }
}
