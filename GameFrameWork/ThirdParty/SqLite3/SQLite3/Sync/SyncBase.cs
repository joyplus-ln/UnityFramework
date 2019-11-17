using System.Collections.Generic;

namespace Framework.Reflection.Sync
{
    public abstract class SyncBase
    {
        public delegate void DlgtPropertyChanged(object InOldValue, object InCurrentValue);

        private Dictionary<string, DlgtPropertyChanged> propertyChangedDict;

        protected SyncFactory factory;

        protected SyncBase()
        {
            factory = SyncFactory.GetOrCreateSyncFactory(GetType());

            propertyChangedDict = new Dictionary<string, DlgtPropertyChanged>();
        }

        public virtual void OnSyncOne(int InIndex, object InValue)
        {
            factory.OnSyncOne(this, InIndex, InValue);
        }

        public virtual void OnSyncAll(object[] InValues)
        {
            int length = InValues.Length;

            for (int i = 0; i < length; i++)
            {
                factory.OnSyncOne(this, i, InValues[i]);
            }
        }

        public void RegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
                propertyChangedDict[InPropertyName] += InPropertyChangedFuc;
            else
                propertyChangedDict.Add(InPropertyName, InPropertyChangedFuc);
        }

        public void UnRegisterPropertyChanged(string InPropertyName, DlgtPropertyChanged InPropertyChangedFuc)
        {
            if (propertyChangedDict.ContainsKey(InPropertyName))
                propertyChangedDict[InPropertyName] -= InPropertyChangedFuc;
        }

        public void OnPropertyChanged(string InPropertyName, object InOldValue, object InPropertyValue)
        {
            DlgtPropertyChanged del;
            if (propertyChangedDict.TryGetValue(InPropertyName, out del) && null != del)
                del(InOldValue, InPropertyValue);
        }
    }
}
