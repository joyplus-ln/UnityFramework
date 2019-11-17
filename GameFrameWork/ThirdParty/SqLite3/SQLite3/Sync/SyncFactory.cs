using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Framework.Reflection.Sync
{
    public class SyncProperty
    {
        public Type ClassType { get; private set; }
        public string ClassName { get; private set; }
        public PropertyInfo[] Infos { get; private set; }
        public int InfosLength { get; private set; }
        public Dictionary<string, PropertyInfo> InfoDict { get; private set; }

        public SyncProperty(Type InClassType, string InClassName, PropertyInfo[] InInfos, Dictionary<string, PropertyInfo> InFoDict, int InInfosLength)
        {
            ClassType = InClassType;
            ClassName = InClassName;
            Infos = InInfos;
            InfosLength = InInfosLength;
            InfoDict = InFoDict;
        }
    }

    public class SyncFactory
    {
        private readonly Type classType;
        private readonly PropertyInfo[] propertyInfos;
        private readonly Dictionary<string, PropertyInfo> infoDict;
        private readonly int propertyInfoLength;
        private readonly SyncProperty syncProperty;

        public SyncFactory(Type InType)
        {
            classType = InType;
            PropertyInfo[] infos = InType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            propertyInfoLength = infos.Length;
            propertyInfos = new PropertyInfo[propertyInfoLength];
            infoDict = new Dictionary<string, PropertyInfo>(propertyInfoLength);
            int correctLen = 0;
            Type syncAttrType = typeof(SyncAttribute);
            for (int i = 0; i < propertyInfoLength; i++)
            {
                object[] attrs = infos[i].GetCustomAttributes(syncAttrType, false);
                if (1 == attrs.Length && attrs[0] is SyncAttribute)
                {
                    int syncId = ((SyncAttribute) attrs[0]).SyncID;
                    if (-1 < syncId && syncId < propertyInfoLength)
                    {
                        propertyInfos[syncId] = infos[i];
                        infoDict.Add(infos[i].Name, infos[i]);
                        ++correctLen;
                    }
                    else throw new IndexOutOfRangeException("Please set SyncAttribute to the property according to the sequence.");
                }
            }

            syncProperty = new SyncProperty(classType, classType.Name, propertyInfos, infoDict, correctLen);
        }

        public void OnSyncOne(object InObj, int InInfoIndex, object InValue)
        {
            if (null == InObj) throw new ArgumentNullException();
            if (0 > InInfoIndex || InInfoIndex >= propertyInfoLength) throw new IndexOutOfRangeException(InInfoIndex + "< 0 || " + InInfoIndex + " >= " + propertyInfoLength);
            if (InObj.GetType() != classType) throw new ArgumentException("The input type not matched.");

            OnSyncOne(InObj, propertyInfos[InInfoIndex], InValue);
        }

        public void OnSyncOne(object InObj, string InInfoName, object InValue)
        {
            if (null == InObj) throw new ArgumentNullException();
            if (string.IsNullOrEmpty(InInfoName) || infoDict.ContainsKey(InInfoName)) throw new Exception("UnKnow property name.");
            if (InObj.GetType() != classType) throw new ArgumentException("The input type not matched.");

        }

        public void OnSyncOne(object InObj, PropertyInfo InInfo, object InValue)
        {
            if (null != InObj && null != InInfo && null != InValue)
            {
                object oldValue = InInfo.GetValue(InObj, null);
                if (!InValue.Equals(oldValue))
                {
                    if (null == oldValue || InValue.GetType() != InInfo.PropertyType)
                        InValue = ConvertTypeUtility.ChangeSyncType(InValue, InInfo.PropertyType);
                    InInfo.SetValue(InObj, InValue, null);
                    SyncBase syncBase = InObj as SyncBase;
                    if (syncBase != null) syncBase.OnPropertyChanged(InInfo.Name, oldValue, InValue);
                }
            }
        }

        private static readonly Dictionary<Type, SyncFactory> FactoriesDict = new Dictionary<Type, SyncFactory>();
        public static SyncFactory GetOrCreateSyncFactory(Type InType)
        {
            SyncFactory factory;
            if (!FactoriesDict.TryGetValue(InType, out factory))
            {
                try
                {
                    factory = new SyncFactory(InType);
                    FactoriesDict.Add(InType, factory);
                }
                catch (Exception ex)
                {
                    Debug.LogError(InType + " Create SyncFactory Error : " + ex.Message);
                }
            }

            return factory;
        }

        public static SyncProperty GetSyncProperty<T>() where T : SyncBase
        {
            return GetOrCreateSyncFactory(typeof(T)).syncProperty;
        }

        public static SyncProperty GetSyncProperty(object InObj)
        {
            return GetOrCreateSyncFactory(InObj.GetType()).syncProperty;
        }

        public static SyncProperty GetSyncProperty(Type InType)
        {
            return GetOrCreateSyncFactory(InType).syncProperty;
        }
    }
}