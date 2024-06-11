using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersDataHandler : MonoBehaviour
{
    [SerializeField] private PlayerData_SO playerData;
    
    
    private void OnEnable()
    {
        ExternalEvents.PlayButtonClicked += OnPlayButtonClicked;
    }

    private void OnDisable()
    {
        ExternalEvents.PlayButtonClicked += OnPlayButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        playerData.ResetData();
    }
}
