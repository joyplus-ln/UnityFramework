using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStatus
{
    protected ApplicationStart startApp;
    public BaseStatus(ApplicationStart startApp)
    {
        this.startApp = startApp;
    }

    public virtual void OnUpdate()
    {
        
    }
    public virtual void Leave()
    {
        
    }

    public virtual void Enter()
    {
        
    }
}
