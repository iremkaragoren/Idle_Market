using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayButton : MonoBehaviour,IClickable
{
    public void OnClick()
    {
        ExternalEvents.PlayButtonClicked?.Invoke();
    }
}
