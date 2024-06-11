using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class MiniFarm :  InteractableBase
{
    
    
    [Button("Tomato Products")]
    [SerializeField] private List<Transform> _productionList;
    
    
    [Button("Egg Products")]
    [SerializeField] private List<Transform> eggPoints;
    
    
    [Button("EggBox Products")]
    [SerializeField] private List<Transform> eggBoxList;

    [SerializeField] private Chicken_UIController chickenUIController;
    
    private List<Transform> spawnerList = new List<Transform>();
    private List<Transform> _salesList = new List<Transform>();
    
    internal int productCount;
    private Coroutine eggCoroutine;
    

    private void Awake()
    {
        InitilaizeDataToSend();
    }

    protected override void InitilaizeDataToSend()
    {
        dataTosend = new TriggeredAreaData
        {
            areaType = Enums.AreaType.MiniFarm,
            productType = Enums.ProductType.Egg,
            targetPoint = this.gameObject.transform,
            productionList = _productionList,
            salesList = _salesList
        };
    }


    private void OnEnable()
    {
        InternalEvents.ChickenFeedDropped += OnChickenFeedDropped;
        InternalEvents.PlayerTakeProducts += OnPlayerTakeProduct;
    }

    private void OnDisable()
    {
        InternalEvents.ChickenFeedDropped -= OnChickenFeedDropped;
        InternalEvents.PlayerTakeProducts -= OnPlayerTakeProduct;
    }

    private void OnPlayerTakeProduct(int takeCount,Enums.ProductType productType)
    {
        if (productType == Enums.ProductType.Egg)
        {
            RemoveProductsFromSalesList(takeCount);
        }
       
    }
    
    private void RemoveProductsFromSalesList(int takeCount)
    {
        int itemsToRemove = Mathf.Min(takeCount, dataTosend.salesList.Count);
        
        for (int i = 0; i < itemsToRemove; i++)
        {
            dataTosend.salesList.RemoveAt(0);
        }
        
    }

   
    private void OnChickenFeedDropped(int droppedCount)
    {
        productCount += droppedCount;
        
       chickenUIController.CurrentItemCount(productCount);
        
        eggCoroutine = StartCoroutine(SpawnEggs());
    }
    

   IEnumerator SpawnEggs()
    {
        while (productCount >= 4)
        {
            yield return new WaitForSeconds(4f);
            
            if (productCount >= 4)
            {
                int spawnCount = 2;
                
                for (int i = 0; i < spawnCount; i++)
                {
                    Transform eggPoint = eggPoints[i];
                    GameObject productGO = Instantiate(areaData.AreaGo, eggPoint.transform.position, Quaternion.identity);
                    productGO.transform.SetParent(eggPoint.transform, true);
                    spawnerList.Add(productGO.transform);
                    
                }
                
                if (spawnerList.Count >= 2)
                {
                    MoveProductsToEggBox();
                    spawnerList.Clear();
                }
                
                List<Transform> filledTomatoPoints = _productionList.Where(feedPoint => feedPoint.childCount > 0).ToList();
                
                if (filledTomatoPoints.Count >= 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int feedPointIndex = Random.Range(0, filledTomatoPoints.Count);
                        Transform selectedFeedPoint = filledTomatoPoints[feedPointIndex];

                        GameObject childToDelete = selectedFeedPoint.GetChild(0).gameObject;
                        Destroy(childToDelete);

                        filledTomatoPoints.RemoveAt(feedPointIndex);
                    }
                    
                    productCount -= 4;
                }
                
                if (productCount < 4)
                {
                    chickenUIController.CurrentItemCount(productCount);
                    StopEggSpawner();
                    yield break;
                }
            }
        }

       
    }
   

    private void StopEggSpawner()
    {
        if (eggCoroutine != null)
        {
            StopCoroutine(SpawnEggs());
            eggCoroutine = null;
        }
    }
    
    

    
    private void MoveProductsToEggBox()
    {
        int productIndex = 0;
        foreach (Transform eggPoint in eggBoxList)
        {
            if (eggPoint.childCount == 0)
            {
                if (productIndex < spawnerList.Count)
                {
                    Transform product = spawnerList[productIndex++];
                    product.transform.DOMove(eggPoint.position, 1f)
                        .OnComplete(() =>
                        {
                            product.transform.SetParent(eggPoint);
                            _salesList.Add(product);

                         });
                    
                    InternalEvents.TomatoSpawned?.Invoke(_salesList);
                }
            }

            else
            {
                continue;
            }
        }
    }

  
}
