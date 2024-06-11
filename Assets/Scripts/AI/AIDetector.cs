using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class AIDetector : MonoBehaviour
{
    [SerializeField] private AIController aiController;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            TriggeredAreaData triggeredAreaData = interactable.AIInteract(aiController);
             aiController.CollectAisleItem(triggeredAreaData);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            interactable.AIExit(aiController);
        }
        
    }
    
}