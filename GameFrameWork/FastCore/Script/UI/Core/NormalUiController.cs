using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUiController
{
    private Transform parent;
    private BaseUI currentUI;
    private List<BaseUI> uiwindowQueue = new List<BaseUI>();
    private bool busing = false;
    public NormalUiController(Transform parent)
    {
        this.parent = parent;
    }

    public IEnumerator ShowWindow(BaseUI baseui)
    {
        if (busing)
        {
            switch (@baseui.openType)
            {
                case OpenUiType.First:
                    uiwindowQueue.Insert(0,baseui);
                    break;
                case OpenUiType.Next:
                    uiwindowQueue.Insert(0,baseui);
                    break;
                case OpenUiType.Queue:
                    uiwindowQueue.Add(baseui);
                    break;
                case OpenUiType.Replace:
                    uiwindowQueue.Insert(0,baseui);
                    break;
            }
            
        }
        else
        {
            busing = true;
            //如果面板是被隐藏的
            if (baseui.states == UiState.Hidden)
            {
                ReopenUiWindow(baseui);
            }
            else
            {
                bool open = true;
                if (currentUI != null)
                {
                    switch (baseui.openType)
                    {
                        case OpenUiType.First:
                            HiddenUiWindow(currentUI);
                            break;
                        case OpenUiType.Next:
                            open = false;
                            uiwindowQueue.Insert(0,baseui);
                            break;
                        case OpenUiType.Queue:
                            uiwindowQueue.Add(baseui);
                            break;
                        case OpenUiType.Replace:
                            yield return CloseWindow(currentUI);
                            break;
                    }
                }

                if (open)
                {
                    baseui.SetController(this);
                    //设置随机id
                    baseui.UIID = System.Guid.NewGuid().ToString();
                    baseui.transform.SetParent(parent,false);
                    currentUI = baseui;
                    baseui.OnOpen();
                    yield return baseui.OpenAnim();
                    baseui.OpenCompleted();
                    busing = false;
                    yield return CheckUIOpen();
                }

            }

        }
    }


    /// <summary>
    /// 检查当前第一个是否是需要立即打开的
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckUIOpen()
    {
        if (uiwindowQueue.Count > 0)
        {
            BaseUI ui = uiwindowQueue[0];
            if (ui.openType == OpenUiType.First || ui.openType == OpenUiType.Replace)
            {
                uiwindowQueue.RemoveAt(0);
                yield return ShowWindow(ui);
            }
        }
    }

    /// <summary>
    /// 打开被隐藏的面板
    /// </summary>
    /// <param name="baseui"></param>
    private void ReopenUiWindow(BaseUI baseui)
    {
        currentUI = baseui;
        baseui.OnReOpen();
    }

    private void HiddenUiWindow(BaseUI baseui)
    {
        baseui.states = UiState.Hidden;
        baseui.OnHidden();
    }

    public IEnumerator CloseWindow(BaseUI baseui)
    {
        if (currentUI.UIID == baseui.UIID)
        {
            yield return baseui.ClosingAnim();
            baseui.CloseCompleted();
            baseui.OnClose();
            if (uiwindowQueue.Count > 0)
            {
                BaseUI ui = uiwindowQueue[0];
                uiwindowQueue.RemoveAt(0);
                yield return ShowWindow(ui);
            }
        }
    }
}
