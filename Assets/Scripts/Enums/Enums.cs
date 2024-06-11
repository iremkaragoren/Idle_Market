using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class Enums 
{
    public enum ProductType
  {
      None,
      Tomato,
      Egg,
      Canned,
      Cashier,
      HappyEmoji,
      ClockEmoji,
  }
    
    public enum AreaType
    {
        Manufacture,
        Aisle,
        MiniFarm,
        Canned,
        Money,
        Checkout
    }
}
