using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InternalEvents : MonoBehaviour
{
    public static UnityAction<List<Transform>, Enums.ProductType> ProductPoints;
    public static UnityAction<List<Transform>> TomatoSpawned;
    public static UnityAction<int, Enums.ProductType> PlayerTakeProducts;
    
    public static UnityAction<TriggeredAreaData> PlayerTriggeredArea;
    public static UnityAction<TriggeredAreaData> HelperTriggeredArea;
    public static UnityAction<Enums.AreaType, Enums.ProductType> PlayerOutArea;


    public static UnityAction<TriggeredAreaData> HelperTargetDeskActivated;
   
    public static UnityAction<int> MoneyListCount;
    public static UnityAction TuttorialMoneyTriggered;
    public static UnityAction<int> MoneyDecrease;
    public static UnityAction NeededMoneyZero;
    public static UnityAction HelperNeededMoneyZero;

    public static UnityAction MiniFarmSelected;
    
    
    public static UnityAction<List<Transform>> ProductDeskActive;
    public static UnityAction<int> ChickenFeedDropped;
    public static UnityAction<int> CannedTomatoDropped;
    public static UnityAction WasteTriggered;
    
    public static UnityAction<Transform> FinishedAIProcessing;
    
    public static UnityAction AllDropPointFull;
    public static UnityAction<Transform> ActiveCursorChanged;








}
