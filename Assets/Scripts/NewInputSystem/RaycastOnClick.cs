using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastOnClick : MonoBehaviour
{
    
    private Vector2 touchPoint;

    public void GetTouchPos(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        touchPoint = context.ReadValue<Vector2>();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPoint);
                 
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                IClickable clickable = hitInfo.collider.GetComponent<IClickable>(); 
                if (clickable != null)
                {
                    clickable.OnClick();
                         
                }
            }
        }
    }
}

