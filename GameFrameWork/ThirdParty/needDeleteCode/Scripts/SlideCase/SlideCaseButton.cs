using System;
using UnityEngine;
using UnityEngine.UI;

public class SlideCaseButton : MonoBehaviour
{
    public enum SlideType
    {
        World,
        Rank,
        Inbox,
        Task
    }

    public SlideType type;
    public Image image;
    public Sprite activeSprite;
    public Sprite inActiveSprite;
    public bool useSprite = false;

    public Color NormalColor;
    public Color ActiveColor;

    public bool toggle = false;
    public Action act_normal;
    public Action act_onfouce;

    public void ChangeButtonStatus()
    {
        toggle = !toggle;
        if (useSprite)
        {
            if (toggle)
            {
                image.sprite = activeSprite;
                // if (act_onfouce != null) act_onfouce.Invoke();
            }
            else
            {
                image.sprite = inActiveSprite;
                // if (act_normal != null) act_normal.Invoke();
            }
        }
        else
        {
            if (toggle)
            {
                image.color = ActiveColor;
            }
            else
            {
                image.color = NormalColor;
            }
        }
        if (!toggle)
        {
            if (act_onfouce != null) act_onfouce.Invoke();
        }
        else
        {
            if (act_normal != null) act_normal.Invoke();
        }
    }
}