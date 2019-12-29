using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BetaFramework
{
    public class RecordTable<T>
    {
        public delegate T GetTValue(string key,T defaultValue);

        public delegate void SetTValue(string key, T usefulValue);

        private GetTValue getTValue;
        private SetTValue setTValue;

        public RecordTable(GetTValue getTValue, SetTValue setTValue)
        {
            this.getTValue = getTValue;
            this.setTValue = setTValue;
        }
        

        public T GetValue(string key, T defaultValue)
        {
            T needValue = getTValue(key,defaultValue);
            return needValue;
        }

        public void SetValue(string key, T usefulValue)
        {
            setTValue(key, usefulValue);
        }

    }
}