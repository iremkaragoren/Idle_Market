using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class InteractableBase : MonoBehaviour, Interactable
{
    private List<AIController> aiControllerList = new List<AIController>();
  
    public AreaData_SO areaData;
    
    public TriggeredAreaData dataTosend;
   
    protected virtual void SendInteractableType()
    {
        InitilaizeDataToSend(); 
        InternalEvents.PlayerTriggeredArea?.Invoke(dataTosend);
    }
    
    protected abstract void InitilaizeDataToSend();
    
    protected virtual void EndInteractableType()
    {
        InternalEvents.PlayerOutArea?.Invoke(areaData.Area,areaData.ProductType);
    }

    protected virtual void SendHelperInteractableType()
    {
        InitilaizeDataToSend();
        InternalEvents.HelperTriggeredArea?.Invoke(dataTosend);
        
    }
    
    public void HelperInteract()
    {
        SendHelperInteractableType();
    }
    
    public void Interact()
    {
        SendInteractableType();
        
    }

    protected void ShareProductWithAIList()
    {
        DOVirtual.DelayedCall(1.5f, DistributeProducts);
    }

    private void DistributeProducts()
    {
        if (dataTosend.salesList == null || dataTosend.salesList.Count == 0)
        {
            return;
        }

        List<AIController> completedControllers = new List<AIController>();

       
        foreach (var aiController in aiControllerList)
        {
            Debug.Log($"AI {aiController.name} needs {aiController.CurrentNeededCount} products.");
        }

        for (int i = 0; i < aiControllerList.Count; i++)
        {
            AIController aiController = aiControllerList[i];

            int neededCount = aiController.CurrentNeededCount;
            int availableCount = dataTosend.salesList.Count;

            int productCountToGive = Mathf.Min(neededCount, availableCount);
            int productsGiven = 0;

            
            List<Transform> tempProductList = new List<Transform>(dataTosend.salesList);

            for (int j = 0; j < productCountToGive; j++)
            {
                Transform productToShare = tempProductList[0];
                aiController.CollectNeededCount(productToShare, dataTosend.productType);
                dataTosend.salesList.Remove(productToShare);
                tempProductList.Remove(productToShare);
                productsGiven++;
            }
            
            Debug.Log($"AI {aiController.name} was given {productsGiven} products.");

            if (aiController.CurrentNeededCount == 0)
            {
                completedControllers.Add(aiController);
            }
        }
        
        foreach (var completedController in completedControllers)
        {
            aiControllerList.Remove(completedController);
            Debug.Log("AI OUT");
        }
    }


    public virtual TriggeredAreaData AIInteract(AIController newAI)
    {
        
        if(!aiControllerList.Contains(newAI))
            aiControllerList.Add(newAI);
        
        return dataTosend;
    }

    public void AIExit(AIController aiController)
    {
         aiControllerList.Remove(aiController);
        
    }
    
    public void EndInteraction()
    {
         EndInteractableType();
    }
    
}


public struct TriggeredAreaData
{ 
    public Enums.AreaType areaType;
    public Enums.ProductType productType;
    public Transform targetPoint;
    public List<Transform> productionList;
    public List<Transform> salesList;
    
    public TriggeredAreaData(Enums.AreaType _areaType, Enums.ProductType _productType, Transform _target ,List<Transform> _productionList, List<Transform> _salesList)
    {
        this.areaType = _areaType;
        this.productType = _productType;
        this. targetPoint = _target;
        this.productionList = new List<Transform>(_productionList);
        this.salesList = new List<Transform>(_salesList);
    }
}