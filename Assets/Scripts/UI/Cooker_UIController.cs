using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cooker_UIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wantedText;
    [SerializeField] private TextMeshProUGUI addedText;
    [SerializeField] private TextMeshProUGUI slashText;
    [SerializeField] private Image productSprite;
    
    public void CurrentItemCount(int addedItem)
    {
        addedText.text = addedItem.ToString();
        
        if (addedItem >= 4)
        {
            DisableUIElements();
        }
        if (addedItem < 4)
        {
            ActivateUIElements();
        }
    }
    
    private void DisableUIElements()
    {
        wantedText.gameObject.SetActive(false);
        addedText.gameObject.SetActive(false);
        slashText.gameObject.SetActive(false);
        productSprite.gameObject.SetActive(false);
        
        ResetAddedCount();
    }
    
    private void ActivateUIElements()
    {
        wantedText.gameObject.SetActive(true);
        addedText.gameObject.SetActive(true);
        slashText.gameObject.SetActive(true);
        productSprite.gameObject.SetActive(true);
    }

    
    private void ResetAddedCount()
    {
        addedText.text = "0";
    }

}

