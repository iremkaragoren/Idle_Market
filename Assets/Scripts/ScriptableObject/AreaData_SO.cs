using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Variable", menuName = "ProductArea/AreaData", order = 0)]
public class AreaData_SO : ScriptableObject
{
    [SerializeField] private GameObject areaGO;
    [SerializeField] private Enums.AreaType area;
    [SerializeField] private Enums.ProductType productType;

    public GameObject AreaGo => areaGO;
    public Enums.ProductType ProductType => productType;
    public Enums.AreaType Area => area;

    
}





