using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Predefined", menuName = "Product/IconHolderData", order = 0)]
public class IconHolderData_SO : SerializedScriptableObject
{
    public Dictionary<Enums.ProductType, Sprite> ProductIconDict;


    public Sprite GetIcon(Enums.ProductType productType)
    {
        return ProductIconDict[productType];
    }
    
    //Example Usage: Sprite sprite = iconholderData_SO.GetIcon(Enums.ProductType.Tomato);

}



