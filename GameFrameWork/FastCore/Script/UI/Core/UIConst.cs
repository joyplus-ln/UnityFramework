using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConst 
{

}

/// <summary>
/// ui展示类型
/// </summary>
public enum OpenUiType
{
    //代替这一个
    Replace,
    //打开这一个，把当前面板压栈
    First,
    //追加在最后面
    Queue,
    //在当前面板之后
    Next
    
}

public enum UILayer
{
    Normal,
    Middle,
    High
}

public enum UiState
{
    Waiting,
    Opening,
    Idle,
    Hidden,
    Closing,
    Closed
}
