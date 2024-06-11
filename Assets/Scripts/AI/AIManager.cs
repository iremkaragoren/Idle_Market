using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class AIManager : SerializedMonoBehaviour
{
    public List<Dictionary<GameObject, int>> allAIDictionaries = new List<Dictionary<GameObject, int>>();
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CheckoutInteractable ai_queHandler;
    [SerializeField] private Transform cashierPoint;
    [SerializeField] private List<Transform> productMovelist = new List<Transform>();

    private List<Transform> activeAIList = new List<Transform>();
    private List<Transform> activePointsList = new List<Transform>();
    [SerializeField] private List<Transform> aiTargetPoints = new List<Transform>();

    private bool isAIStarterRunning = false;
    private int maxAICount =4;

    private void Awake()
    {
         InternalEvents.ProductDeskActive += OnProductDeskActivated;
        InternalEvents.FinishedAIProcessing += OnAIFinishedProcessing;
       
    }

    private void OnDisable()
    { 
        InternalEvents.ProductDeskActive -= OnProductDeskActivated;
        InternalEvents.FinishedAIProcessing -= OnAIFinishedProcessing;
       
    }
    
    private void OnAIFinishedProcessing(Transform finishedAI)
    {
        if (activeAIList.Contains(finishedAI))
        {
            activeAIList.Remove(finishedAI);
        }

        if ( !isAIStarterRunning)
        {
            StartCoroutine(AIStarter());
        }
    }

    private void OnProductDeskActivated(List<Transform> activeGO)
    {
        foreach (Transform activeDesk in activeGO)
        {
            if (aiTargetPoints.Contains(activeDesk))
            {
                activePointsList.Add(activeDesk);
            }
        }

        if (activePointsList.Count > 0 && !isAIStarterRunning)
        {
            StartCoroutine(AIStarter());
        }
    }

    IEnumerator AIStarter()
    {
        isAIStarterRunning = true;

        while (activeAIList.Count < maxAICount)
        {
            AISpawner();
            
            yield return new WaitForSeconds(5f);
            
            if (activeAIList.Count >= maxAICount)
            {
                break;
            }
            
        }

        isAIStarterRunning = false;
    }

    private void AISpawner()
    {
        Dictionary<GameObject, int> selectedDict = SelectedDictionary();

        if (selectedDict != null && selectedDict.Count > 0)
        {
            foreach (KeyValuePair<GameObject, int> entry in selectedDict)
            {
                GameObject aiGO = Instantiate(entry.Key, spawnPoint.position, Quaternion.identity);
                AIController aiController = aiGO.GetComponent<AIController>();
                activeAIList.Add(aiGO.transform);
                aiController.InitializeTargets(activePointsList, entry.Value, cashierPoint, productMovelist, spawnPoint, ai_queHandler);
            }
        }
    }

    Dictionary<GameObject, int> SelectedDictionary()
    {
        int randomIndex = Random.Range(0, allAIDictionaries.Count);
        return allAIDictionaries[randomIndex];
    }
}
