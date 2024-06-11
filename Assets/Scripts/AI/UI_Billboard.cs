using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.rotation=Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, 0f);
    }
}
