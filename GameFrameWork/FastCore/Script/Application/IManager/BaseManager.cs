using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager
{
    private bool InitAsyncFinished = false;
    public Action InitFinshed = null;
    /// <summary>
    /// 同步初始化
    /// </summary>
    public virtual void Init()
    {
        
    }

    /// <summary>
    /// 异步初始化
    /// </summary>
    public virtual void InitAsync()
    {
        InitAsyncSuccessful();
    }

    /// <summary>
    /// 异步初始化成功
    /// </summary>
    protected void InitAsyncSuccessful()
    {
        InitAsyncFinished = true;
        InitFinshed?.Invoke();
    }

    /// <summary>
    /// 是否初始化完成了
    /// </summary>
    /// <returns></returns>
    public bool Finished()
    {
        return InitAsyncFinished;
    }

    public virtual void Update()
    {
        
    }

    public virtual void GamePause(bool pause)
    {
        
    }

    public virtual void ExitGame()
    {
        
    }
}
