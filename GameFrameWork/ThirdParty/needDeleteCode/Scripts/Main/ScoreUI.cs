using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    private static ScoreUI _instance = null;

    public static ScoreUI instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<ScoreUI>();
            }
            return _instance;
        }
    }

    public Sprite[] scoreImage;

#if UNITY_EDITOR

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ShowComboScore(0);
        }
    }

#endif

    /// <summary>
    /// 显示combo的分数ui
    /// </summary>
    /// <param name="score"></param>
    public void ShowComboScore(int score)
    {
        StartCoroutine(ShowImage(Vector3.one, TransScore(score)));
    }

    public void ShowExtraWordScore(int score)
    {
        StartCoroutine(ShowImage(Vector3.one, TransScore(score)));
    }

    /// <summary>
    /// 出现的位置，图片的位置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="picpos"></param>
    /// <returns></returns>
    private IEnumerator ShowImage(Vector3 pos, int picpos)
    {
        Image imageobj;
        yield return imageobj = new GameObject().AddComponent<Image>();
        yield return imageobj.sprite = scoreImage[picpos];
        imageobj.transform.SetParent(transform, false);
        imageobj.transform.position = pos;
        imageobj.transform.localScale = Vector3.zero;
        imageobj.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.InOutBack);
        yield return new WaitForSeconds(1f);
        Destroy(imageobj);
    }

    private int TransScore(int score)
    {
        if (score <= 6)
        {
            return score - 1;
        }
        else
        {
            if (score == 9)
            {
                return 6;
            }
            else if (score == 10)
            {
                return 7;
            }
            else if (score == 12)
            {
                return 8;
            }
            else if (score == 15)
            {
                return 9;
            }
        }
        return score;
    }
}