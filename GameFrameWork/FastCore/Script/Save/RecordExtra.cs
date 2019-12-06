using System;
using System.Collections;
using System.Collections.Generic;
using BetaFramework;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;

public class RecordExtra {

	
    public class PrefData<T>
    {
        private string key;
        protected T data;
        protected T lastData;
        public UnityEvent ValueChanged = new UnityEvent();

        public PrefData(string key, T defVal)
        {
            this.key = key;
            this.data = Get(key, defVal);
            lastData = data;
        }

        public T Value
        {
            get { return data; }
            set
            {
                Set(key, value);
                data = value;
                if (ValueChanged != null)
                {
                    ValueChanged.Invoke();
                }
            }
        }

        public T LastValue
        {
            get { return lastData; }
            set
            {
                lastData = value;
            }
        }

        public void ResetLastValue()
        {
            lastData = data;
        }

        public bool IsChanged { get { return !lastData.Equals(data); } }
        
        public void Save()
        {
            Set(key, data);
        }

        protected virtual T Get(string key, T defVal)
        {
            return defVal;
        }

        protected virtual void Set(string key, T val)
        {
            Record.SetString(key,JsonConvert.SerializeObject(val));
        }
    }

public class IntPrefData : PrefData<int>
{
    public IntPrefData(string key, int defVal) : base(key, defVal)
    {
    }

    protected override int Get(string key, int defVal)
    {
        return Record.GetInt(key, defVal);
    }

    protected override void Set(string key, int val)
    {
        if (data == val)
            return;
        Record.SetInt(key, val);
    }
}

public class FloatPrefData : PrefData<float>
{
    public FloatPrefData(string key, float defVal) : base(key, defVal)
    {
    }

    protected override float Get(string key, float defVal)
    {
        return Record.GetFloat(key, defVal);
    }

    protected override void Set(string key, float val)
    {
        Record.SetFloat(key, val);
    }
}

public class DoublePrefData : PrefData<double>
{
    public DoublePrefData(string key, double defVal) : base(key, defVal)
    {
    }

    protected override double Get(string key, double defVal)
    {
        return Record.GetDouble(key, defVal);
    }

    protected override void Set(string key, double val)
    {
        Record.SetDouble(key, val);
    }
}

public class BoolPrefData : PrefData<bool>
{
    public BoolPrefData(string key, bool defVal) : base(key, defVal)
    {
    }

    protected override bool Get(string key, bool defVal)
    {
        return Record.GetBool(key, defVal);
    }

    protected override void Set(string key, bool val)
    {
        if (data == val)
            return;
        Record.SetBool(key, val);
    }
}

public class StringPrefData : PrefData<string>
{
    public StringPrefData(string key, string defVal) : base(key, defVal)
    {
    }

    protected override string Get(string key, string defVal)
    {
        return Record.GetString(key, defVal);
    }

    protected override void Set(string key, string val)
    {
        if (val == null)
            Record.DeleteKey(key);
        else
            Record.SetString(key, val);
    }
}

public class ObjectPrefData<T> : PrefData<T> where T : new()
{
    public ObjectPrefData(string key, T defVal) : base(key, defVal)
    {
    }

    protected override T Get(string key, T defVal)
    {
        return Record.GetObject(key, defVal);
    }

    protected override void Set(string key, T val)
    {
        if (val == null)
            Record.DeleteKey(key);
        else
            Record.SetObject(key, val);
    }
}

}
