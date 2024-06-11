using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip spawnClip;
    [SerializeField] private List<GameObject> productDeskList = new();
    [SerializeField] private GameObject helperGO;

    private int currentActiveIndex;
    private readonly List<Transform> activateDeskList = new();
    private GameObject deskToActivate;

    private void Awake()
    {
        InternalEvents.NeededMoneyZero += OnNeededMoneyZero;
        InternalEvents.HelperNeededMoneyZero += OnHelperNeededMoneyZero;
        InternalEvents.TuttorialMoneyTriggered += OnTutorialTriggered;
    }

    private void OnDisable()
    {
        InternalEvents.NeededMoneyZero -= OnNeededMoneyZero;
        InternalEvents.HelperNeededMoneyZero -= OnHelperNeededMoneyZero;
        InternalEvents.TuttorialMoneyTriggered -= OnTutorialTriggered;
    }

    private void OnHelperNeededMoneyZero()
    {
       helperGO.SetActive(true);
    }

    private void OnTutorialTriggered()
    {
        deskToActivate = productDeskList[0];
        deskToActivate.SetActive(true);
        PlaySpawnSound();
        activateDeskList.Add(deskToActivate.transform);

        currentActiveIndex++;

        InternalEvents.ProductDeskActive?.Invoke(activateDeskList);
    }

    private void OnNeededMoneyZero()
    {
        if (currentActiveIndex < productDeskList.Count)
        {
            deskToActivate = productDeskList[currentActiveIndex];

            if (!deskToActivate.activeInHierarchy)
            {
                deskToActivate.SetActive(true);
                PlaySpawnSound();

                activateDeskList.Add(deskToActivate.transform);
            }

            currentActiveIndex++;

            InternalEvents.ProductDeskActive?.Invoke(activateDeskList);
        }
    }

    private void PlaySpawnSound()
    {
        if (audioSource != null && spawnClip != null) audioSource.PlayOneShot(spawnClip);
    }
}