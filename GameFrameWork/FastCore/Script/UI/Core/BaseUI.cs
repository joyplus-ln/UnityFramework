using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private NormalUiController m_controller;
    public string UIID;
    public object[] obj;
    public OpenUiType openType;
    public UiState states = UiState.Waiting;
    public bool CanReuse = true;

    public void SetController(NormalUiController m_controller)
    {
        this.m_controller = m_controller;
    }
    /// <summary>
    /// 被打开的时候调用
    /// </summary>
    public virtual void OnOpen()
    {
        states = UiState.Waiting;
    }

    /// <summary>
    /// 打开动画
    /// </summary>
    public virtual IEnumerator OpenAnim()
    {
        states = UiState.Opening;
        yield return 0;
    }

    /// <summary>
    /// 完成了open动画
    /// </summary>
    public virtual void OpenCompleted()
    {
        states = UiState.Idle;
    }
    
    //在栈中再次弹出的时候调用
    public virtual void OnReOpen()
    {
        
    }
    
    //被隐藏入栈的时候调用
    public virtual void OnHidden()
    {
        
    }

    /// <summary>
    /// 关闭动画
    /// </summary>
    public virtual IEnumerator ClosingAnim()
    {
        states = UiState.Closing;
        yield return 0;
    }

    /// <summary>
    /// 关闭成功
    /// </summary>
    public virtual void CloseCompleted()
    {
        states = UiState.Closed;
    }
    
    //被关闭
    public virtual void OnClose()
    {
        
    }
    
    //进入对象池被回收的时候调用
    public virtual void OnBeRecy()
    {
        
    }
}
