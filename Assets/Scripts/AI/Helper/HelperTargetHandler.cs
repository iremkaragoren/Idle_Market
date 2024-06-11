using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelperTargetHandler : MonoBehaviour
{
    [SerializeField] private PlayerData_SO playerData;
    [SerializeField] private MiniFarm miniFarm;
    [SerializeField] private Manufacture manufacture;
    
    [SerializeField] private Transform eggAisle;
    [SerializeField] private Transform tomatoAisle;
    [SerializeField] private Transform cannedAisle;
    [SerializeField] private HelperStackHandler _helperStackHandler;

    private readonly Dictionary<Transform, List<Transform>> salesDict = new();

    private Transform nextTargetArea;
    private Transform nextTargetAisle;

    private void Awake()
    {
        salesDict.Add(miniFarm.transform, new List<Transform> { eggAisle });
        salesDict.Add(manufacture.transform, new List<Transform> { miniFarm.transform, tomatoAisle, cannedAisle });
    }

    public (Transform, Transform) GetNextTarget()
    {
        if (nextTargetArea != null && nextTargetAisle != null)
        {
            var manualTarget = (nextTargetArea, nextTargetAisle);
            nextTargetArea = null;
            nextTargetAisle = null;
            return manualTarget;
        }
        
        
        List<Transform> availableAreas = new List<Transform>();

        if (IsMiniFarmSalesListNotEmpty())
        {
            availableAreas.Add(miniFarm.transform);
        }

        availableAreas.Add(manufacture.transform);

        if (availableAreas.Count == 0)
        {
            return (null, null);
        }
        
        Transform selectedArea = availableAreas[Random.Range(0, availableAreas.Count)];
        
        
        List<Transform> possibleTargets = salesDict[selectedArea];
        Transform selectedAisle = possibleTargets[Random.Range(0, possibleTargets.Count)];

        return (selectedArea, selectedAisle);
    }
    
    private bool IsMiniFarmSalesListNotEmpty()
    {
        return miniFarm.dataTosend.salesList.Any(item => item.childCount > 0);
    }
}
