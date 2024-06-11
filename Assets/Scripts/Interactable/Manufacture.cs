using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Manufacture : InteractableBase
{
    [Button("Spawn Products")]
    [SerializeField] private List<Transform> _productionList;
    private List<Transform> _salesList = new List<Transform>();
    private int productCount;
    

    private void OnEnable()
    {
        productCount = 3;
        ProductSpawner();  
         InternalEvents.PlayerTakeProducts += OnTakebleCount;
    }

    private void OnDisable()
    {
        InternalEvents.PlayerTakeProducts -= OnTakebleCount;
    }


    private void Awake()
    {
        InitilaizeDataToSend();
    }

    protected override void InitilaizeDataToSend()
    {
        dataTosend = new TriggeredAreaData
        {
            areaType = Enums.AreaType.Manufacture,
            targetPoint = this.gameObject.transform,
            productType = Enums.ProductType.Tomato,
            productionList = _productionList,
            salesList = _salesList
        };
    }

    private void OnTakebleCount(int count, Enums.ProductType productType)
    {
        productCount = count;
        StartCoroutine(DelayedSpawn(1f));  
    }
    
    

    private IEnumerator DelayedSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);  
        ProductSpawner();  
    }
    
    private void ProductSpawner()
    {
        _salesList.Clear();
        
        int spawnCount = Mathf.Min(productCount, _productionList.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform seedPoint = _productionList[i];
            GameObject productGO = Instantiate(areaData.AreaGo, seedPoint.transform.position, Quaternion.identity);
            productGO.transform.SetParent(seedPoint.transform, true);
            _salesList.Add(productGO.transform);
        }
        
    }
}