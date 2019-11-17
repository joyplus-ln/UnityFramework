using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomButton : Button
{
    
    [FormerlySerializedAs("DoubleClick")]
    [SerializeField]
    private Button.ButtonClickedEvent m_DoubleClick = new Button.ButtonClickedEvent();
    
    [FormerlySerializedAs("LongtouchClick")]
    [SerializeField]
    private Button.ButtonClickedEvent m_LongtouchClick = new Button.ButtonClickedEvent();
    

    private void OnMouseDown()
    {
    }

    private void OnMouseEnter()
    {
    }

    private void OnMouseExit()
    {
    }

    private void OnMouseOver()
    {
    }

    private void OnMouseUp()
    {
    }

    private void OnMouseUpAsButton()
    {
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (!this.gameObject.activeInHierarchy)
            return;
        switch (state)
        {
            case Selectable.SelectionState.Normal:
                this.image.transform.localScale = Vector3.one;
                break;
            case Selectable.SelectionState.Highlighted:
                this.image.transform.localScale = Vector3.one;
                break;
            case Selectable.SelectionState.Pressed:
                this.image.transform.localScale = Vector3.one * 1.1f;
                break;
            case Selectable.SelectionState.Selected:
                this.image.transform.localScale = Vector3.one;
                break;
            case Selectable.SelectionState.Disabled:
                this.image.transform.localScale = Vector3.one;
                break;
            default:
                this.image.transform.localScale = Vector3.one;
                break;
        }

    }
}
