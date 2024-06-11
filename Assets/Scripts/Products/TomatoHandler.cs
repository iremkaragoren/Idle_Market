using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TomatoHandler : MonoBehaviour
{
    private void OnEnable()
    {
        InternalEvents.TomatoSpawned += OnTomatoSpawned;
    }

    private void OnDisable()
    {
        InternalEvents.TomatoSpawned -= OnTomatoSpawned;
    }

    private void OnTomatoSpawned(List<Transform> arg0)
    {
        transform.DOScale(new Vector3(1, 1, 1), 1f);
    }
}
