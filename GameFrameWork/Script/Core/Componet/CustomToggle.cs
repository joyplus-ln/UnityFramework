using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : Toggle
{
    public UnityAction<string> onselect;


    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (onselect != null)
        {
            onselect.Invoke(gameObject.name);
        }
    }
}