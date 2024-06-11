using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AreaCursorHandler : MonoBehaviour
{
   [SerializeField] private List<GameObject> areaCursorsList = new List<GameObject>();
   [SerializeField] private GameObject helperCursor;
   private int currentActiveIndex = 0;
   private Coroutine activeObjectCoroutine;
   private Tween yMovementTween;

   private void OnEnable()
   {
      ExternalEvents.PlayButtonClicked += OnPlayButtonClicked;
      InternalEvents.TuttorialMoneyTriggered += OnTutorialTriggered;
      InternalEvents.ProductDeskActive += OnProductDeskActive;
      InternalEvents.HelperNeededMoneyZero += OnHelperNeededMoneyZero;
   }

   private void OnDisable()
   {
      ExternalEvents.PlayButtonClicked -= OnPlayButtonClicked;
      InternalEvents.TuttorialMoneyTriggered -= OnTutorialTriggered;
       InternalEvents.ProductDeskActive -= OnProductDeskActive;
       InternalEvents.HelperNeededMoneyZero -= OnHelperNeededMoneyZero;
   }

   private void OnHelperNeededMoneyZero()
   {
      helperCursor.SetActive(false);
   }

   private void OnPlayButtonClicked()
   {
      if (areaCursorsList.Count > 0)
      {
         areaCursorsList[currentActiveIndex].SetActive(true);
         
         StartYMovement(areaCursorsList[currentActiveIndex]);
         
      }
   }
   
   
   private void OnTutorialTriggered()
   {
      if (yMovementTween != null)
      {
         yMovementTween.Kill();
      }
      
   }

   
   private void StartYMovement(GameObject targetObject)
   {
      if (targetObject != null)
      {
         targetObject.transform.DOMoveY(targetObject.transform.position.y - 0.5f, .5f).SetLoops(-1, LoopType.Yoyo);
      }
   }

   private void OnProductDeskActive(List<Transform> pointGO)
   {
      
      if (currentActiveIndex >= 0 && currentActiveIndex < areaCursorsList.Count)
      {
         areaCursorsList[currentActiveIndex].SetActive(false);
         if (activeObjectCoroutine != null)
         {
            StopCoroutine(activeObjectCoroutine);
         }
      }

      currentActiveIndex++;
      if (currentActiveIndex < areaCursorsList.Count)
      {
         areaCursorsList[currentActiveIndex].SetActive(true);
         InternalEvents.ActiveCursorChanged?.Invoke(areaCursorsList[currentActiveIndex].transform);
         
         activeObjectCoroutine = StartCoroutine(AnimateActiveObject(areaCursorsList[currentActiveIndex]));
      }
      
      if (currentActiveIndex == 9)
      {
        helperCursor.SetActive(true);
      }
   }
   
   private IEnumerator AnimateActiveObject(GameObject activeObject)
   {
      Transform currentTransform = activeObject.transform;
      Image currentImage = activeObject.GetComponent<Image>();
      Vector3 originalScale = currentTransform.localScale;
      Color originalColor = currentImage.color;
      Color targetColor = new Color(255/255f, 216/255f, 216/255f);


      while (activeObject.activeSelf)
      {
         currentTransform.DOScale(originalScale * 1.2f, .5f);
         currentImage.DOColor(targetColor, .5f);

         yield return new WaitForSeconds(1f);
         
         currentTransform.DOScale(originalScale, .5f);
         currentImage.DOColor(originalColor, .5f);

         yield return new WaitForSeconds(1f);
      }
   }


   

  
   
}
