using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AI_UIController : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI wantedText;
  [SerializeField] private TextMeshProUGUI addedText;
  [SerializeField] private TextMeshProUGUI slashText;
  [SerializeField] private Image productSprite;
  [SerializeField] private Image processSprite;
  [SerializeField] private IconHolderData_SO iconHolderData;

  
  public void MaxItemCount(int maxProductCount)
  {
    wantedText.text = maxProductCount.ToString();
  }
  
  public void CurrentItemCount(int addedItem)
  {
    addedText.text = addedItem.ToString();
  }

  public void ProductSprite(Enums.ProductType icon )
  {
    switch (icon)
    {
      case Enums.ProductType.Tomato:
        productSprite.sprite = iconHolderData.GetIcon(Enums.ProductType.Tomato);
        break;
      case Enums.ProductType.Egg:
        productSprite.sprite  = iconHolderData.GetIcon(Enums.ProductType.Egg);
        break;
      case Enums.ProductType.Canned:
        productSprite.sprite  = iconHolderData.GetIcon(Enums.ProductType.Canned);
        break;
      case Enums.ProductType.Cashier :
        processSprite.sprite = iconHolderData.GetIcon(Enums.ProductType.Cashier);
        CloseUIElement();
        break;
      case Enums.ProductType.HappyEmoji:
        processSprite.sprite = iconHolderData.GetIcon(Enums.ProductType.HappyEmoji);
        break;
    
    }
  }

  private void CloseUIElement()
  {
     processSprite.enabled = true;
    wantedText.enabled = false;
    productSprite.enabled = false;
    addedText.enabled = false;
    slashText.enabled = false;

  }
  
}
