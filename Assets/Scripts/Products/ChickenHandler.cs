using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ChickenHandler : MonoBehaviour
{
    [SerializeField] private Transform eggPoint;
    private int productCount;
    private Tween feedBoxTween;
    private Quaternion startRotation;

  
    private void OnEnable()
    {
        startRotation = transform.rotation;
        InternalEvents.ChickenFeedDropped += OnChickenFeedDropped;
       
    }

    private void OnDisable()
    {
        InternalEvents.ChickenFeedDropped = OnChickenFeedDropped;
    }

    
    private void OnChickenFeedDropped(int droppedCount)
    {
        productCount += droppedCount;
        
        ChickenMovement();
    }

    private void ChickenMovement()
    {
        if (productCount >= 4)
        {
            Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x + 12, startRotation.eulerAngles.y, startRotation.eulerAngles.z);
            transform.DORotateQuaternion(targetRotation, .5f)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
    
    private void OnFeedBoxEmpty()
    {
        StopFeedBoxAnimation();
    }
    
    private void StopFeedBoxAnimation()
    {
        if (feedBoxTween != null && feedBoxTween.IsActive())
        {
            feedBoxTween.Kill(); 
        }
    }
}
