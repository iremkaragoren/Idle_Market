using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using UnityEngine;

public class HelperDetector : MonoBehaviour
{
  
    private void OnTriggerEnter(Collider collider)
    {
       
        if (collider.TryGetComponent(out Interactable interactable))
        {
           
            interactable.HelperInteract();
        }

        
        if (collider.TryGetComponent(out WasteDetector wasteDetector))
        {
          
            InternalEvents.WasteTriggered?.Invoke();
        }
    }
    
   
    private void OnTriggerStay(Collider collider)
    {
        
        if (collider.TryGetComponent(out WasteDetector wasteDetector))
        {
            InternalEvents.WasteTriggered?.Invoke();
        }
    }
    
  
    private void OnTriggerExit(Collider collider)
    {
        
        if (collider.TryGetComponent(out Interactable interactable))
        {
            interactable.EndInteraction();
        }
    }
}
