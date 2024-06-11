using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class VirtualCameraHandler : MonoBehaviour
{
     [SerializeField] CinemachineVirtualCamera virtualCamera;
     private Transform originalLookAtTarget;
     private Transform newLookAtTarget;
     
     void Awake()
     {
          if (virtualCamera != null)
          {
               originalLookAtTarget = virtualCamera.LookAt;
          }
     }

     private void OnEnable()
     {
          InternalEvents.ActiveCursorChanged += OnActivateCursorChanged;
     }

     private void OnDisable()
     {
          InternalEvents.ActiveCursorChanged -= OnActivateCursorChanged;
     }

     private void OnActivateCursorChanged(Transform activeCursorTransform)
     {
          newLookAtTarget = activeCursorTransform.transform;
          
          TriggerCameraLookAtChange();
     }

     private void TriggerCameraLookAtChange()
     {
          if (virtualCamera != null && newLookAtTarget != null)
          {
               StartCoroutine(ChangeLookAtTemporarily(1f));
          }
     }
     
     private IEnumerator ChangeLookAtTemporarily(float duration)
     {
          
          virtualCamera.LookAt = newLookAtTarget;
          
          yield return new WaitForSeconds(duration);
          
          virtualCamera.LookAt = originalLookAtTarget;
     }
}
