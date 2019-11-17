using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationStart
{
    private Dictionary<string,BaseStatus> _status = new Dictionary<string, BaseStatus>();
    public BaseStatus currentBaseStatus;

    public void SetStatus<T>(T t) where T : BaseStatus
    {
        string key = typeof(T).FullName;
        _status.Add(key,t);
    }
    
    public void TransTo<T>() where T : BaseStatus
    {
        BaseStatus _BaseStatus = GetApplicationStatus<T>();
        if (currentBaseStatus != null)
        {
            currentBaseStatus.Leave();
        }

        currentBaseStatus = _BaseStatus;
        currentBaseStatus.Enter();
    }

    private BaseStatus GetApplicationStatus<T>() where T : BaseStatus
    {
        string key = typeof(T).FullName;
        if (_status.ContainsKey(key))
        {
            return _status[key];
        }

        return null;
    }

    public virtual void OnUpdate()
    {
        if (currentBaseStatus != null)
        {
            currentBaseStatus.OnUpdate();
        }
    }
}
