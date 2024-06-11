using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CannedProductionSite : InteractableBase
{
    [Button("Tomato Products")]
    [SerializeField] private List<Transform> tomatoPointList;
    
    [Button("EggBox Products")]
    [SerializeField] private List<Transform> cannedBoxPoints;
    [SerializeField] private Transform tomatoBlendStartPoint;
    [SerializeField] private Cooker_UIController cookerUIController;
    [SerializeField] private Transform tomatoBlendFinishPoint;
    [SerializeField] private GameObject cookerGO;
    
    private Coroutine tomatoCoroutine;
    private List<Transform> cannedList = new List<Transform>();
    private int productCount;
    
    private void Awake()
    {
        InitilaizeDataToSend();
    }

    protected override void InitilaizeDataToSend()
    {
        dataTosend = new TriggeredAreaData
        {
            areaType = Enums.AreaType.Canned,
            productType = Enums.ProductType.Canned,
            targetPoint = this.gameObject.transform,
            productionList = tomatoPointList,
            salesList = cannedList
        };
    }

    private void OnEnable()
    {
        InternalEvents.CannedTomatoDropped += OnCannedTomatoDropped;
        InternalEvents.HelperTargetDeskActivated?.Invoke(dataTosend);
    }

    private void OnDisable()
    {
        InternalEvents.CannedTomatoDropped -= OnCannedTomatoDropped;
    }

    private void OnCannedTomatoDropped(int droppedCount)
    {
        productCount += droppedCount;
        cookerUIController.CurrentItemCount(productCount);
        
        tomatoCoroutine = StartCoroutine(TomatoMover());
    }

    IEnumerator TomatoMover()
    {
        while (productCount >= 4)
        {
            yield return new WaitForSeconds(2.5f);

            List<Transform> filledTomatoPoints = tomatoPointList.Where(tomatoPoint => tomatoPoint != null && tomatoPoint.childCount > 0).ToList();

            if (filledTomatoPoints.Count >= 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    int tomatoPointIndex = Random.Range(0, filledTomatoPoints.Count);
                    Transform selectedTomatoPoint = filledTomatoPoints[tomatoPointIndex];

                    if (selectedTomatoPoint != null)
                    {
                        Transform tomatoChild = selectedTomatoPoint.GetChild(0);
                        tomatoChild .transform.DOJump(tomatoBlendStartPoint.transform.position, 1, 1, .5f)
                            .SetEase(Ease.OutQuad)
                            .OnComplete(() =>
                            {
                                if (selectedTomatoPoint != null)
                                {
                                    tomatoChild .transform.DOMoveY(transform.position.y - tomatoBlendFinishPoint.position.y, 0.5f)
                                        .SetEase(Ease.InQuad);
                                    tomatoChild .transform.DOScale(Vector3.zero, 0.5f)
                                        .SetEase(Ease.InBack)
                                        .OnComplete(() =>
                                        {
                                            if (tomatoChild  != null)
                                            {
                                                if (cookerGO != null)
                                                {
                                                    cookerGO.transform.DOLocalRotate(new Vector3(0, 0, 2), 0.1f, RotateMode.LocalAxisAdd)
                                                        .SetLoops(4, LoopType.Yoyo)
                                                        .OnComplete(() =>
                                                        {
                                                            Destroy(tomatoChild.gameObject);
                                                        });
                                                }
                                            }
                                        });
                                }
                            });
                        
                    }
                }
                StartCoroutine(CannedSpawners());
                
                productCount -= 4;
                cookerUIController.CurrentItemCount(productCount); 
            }

            if (productCount < 4)
            {
                StopCannedCoroutine();
            }
        }
    }

    IEnumerator CannedSpawners()
    {
        GameObject productGO = Instantiate(areaData.AreaGo, tomatoBlendStartPoint.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1.5f);

        foreach (Transform cannedPoint in cannedBoxPoints)
        {
            if (cannedPoint.childCount == 0)
            {
                productGO.transform.DOMove(cannedPoint.position, 0.5f)
                    .OnComplete(() =>
                    {
                        productGO.transform.SetParent(cannedPoint);
                        cannedList.Add(productGO.transform);
                        ShareProductWithAIList();
                    });
                
                break; 
            }
        }
        
    }

    private void StopCannedCoroutine()
    {
        if (tomatoCoroutine != null)
        {
            StopCoroutine(tomatoCoroutine);
            tomatoCoroutine = null;
        }
        
        
    }
}
