using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Aisle : InteractableBase
{
    [Button("Spawn Products")]
    [SerializeField] private List<Transform> productPoints;

    private readonly List<Transform> _secondList = new List<Transform>();
    
   
    private void Awake()
    {
        InitilaizeDataToSend();
    }

    protected override void InitilaizeDataToSend()
    {
        dataTosend = new TriggeredAreaData
        {
            areaType = Enums.AreaType.Aisle,
            productType = areaData.ProductType,
            productionList = productPoints,
            salesList =_secondList
        };
    }

    private void OnEnable()
    {
        InternalEvents.ProductPoints += OnProductAdded;
    }

    private void OnDisable()
    {
        InternalEvents.ProductPoints -= OnProductAdded;
    }
    
    private void OnProductAdded(List<Transform> list,Enums.ProductType productType)
    {
        if (dataTosend.productType == productType)
        {
            dataTosend.salesList.AddRange(list);
       
            ShareProductWithAIList();
        }
           
    }
    
}

