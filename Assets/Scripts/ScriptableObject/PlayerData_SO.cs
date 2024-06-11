using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Predefined", menuName = "Player/PlayerData", order = 1)]
public class PlayerData_SO : ScriptableObject
{
   [SerializeField] private int currentProductCount; 
   [SerializeField] private int maxStackCount;
   [SerializeField] private Enums.ProductType currentProductType;
   [SerializeField] private int currentMoney;

   public int CurrentMoney
   {
      get => currentMoney;
      set => currentMoney = value;
   }

   public int MaxProductCount => maxStackCount;
   
   public int ProductCount
   {
      get => currentProductCount;
      set => currentProductCount = value;
   }
   
   public Enums.ProductType CurrentProductType
   {
      get => currentProductType;
      set => currentProductType = value;
   }
   public void ResetData()
   {
      currentProductType = Enums.ProductType.None; 
      currentProductCount = 0;
      currentMoney = 0;
   }
}
