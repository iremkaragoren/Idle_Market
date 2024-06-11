using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
  [SerializeField] private GameObject startScene;
  [SerializeField] private GameObject inGame;
  
  private void Awake()
  {
    startScene.SetActive(true);
  }

  private void OnEnable()
  {
    ExternalEvents.PlayButtonClicked += OnPlayButtonClicked;
  }

  private void OnDisable()
  {
    ExternalEvents.PlayButtonClicked -= OnPlayButtonClicked;
  }

  private void OnPlayButtonClicked()
  {
    startScene.SetActive(false);
    inGame.SetActive(true);
  }
}
