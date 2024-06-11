using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Interactable interactable))
        {
            interactable.Interact();
        }

        if (collision.collider.TryGetComponent(out MoneyDetector detector))
        {
            InternalEvents.TuttorialMoneyTriggered?.Invoke();
        }
        
        if (collision.collider.TryGetComponent(out WasteDetector wasteDetector))
        {
            InternalEvents.WasteTriggered?.Invoke();
        }
    }
    
    private void OnCollisionStay(Collision collision)
    {
        // if (collision.collider.TryGetComponent(out Interactable interactable))
        // {
        //     interactable.Interact();
        // }

        if (collision.collider.TryGetComponent(out MoneyDetector detector))
        {
            InternalEvents.TuttorialMoneyTriggered?.Invoke();
        }
        
        if (collision.collider.TryGetComponent(out WasteDetector wasteDetector))
        {
            InternalEvents.WasteTriggered?.Invoke();
        }
    }
    

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Interactable interactable))
        {
            interactable.EndInteraction();
        }
    }

}
