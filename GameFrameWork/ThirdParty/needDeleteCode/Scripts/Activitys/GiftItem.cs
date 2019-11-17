using System;
using UnityEngine;
using UnityEngine.UI;

public class GiftItem : MonoBehaviour
{
    public Text giftNum;
    public string gifttittle;

    public void SetText(string number, bool fitSize = false)
    {
        giftNum.text = String.Format("{0}{1}", gifttittle, number);
        if (fitSize)
        {
            giftNum.transform.localScale = new Vector3(1f, 1f, 1f);
            giftNum.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            giftNum.fontSize = 50;
        }
    }
}