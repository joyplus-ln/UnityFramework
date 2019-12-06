using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddressablesConfig
{
   public Dictionary<string,List<string>> lableDict = new Dictionary<string, List<string>>();
   public Dictionary<string,List<string>> groupDict = new Dictionary<string, List<string>>();

   public List<string> GetGroup(string groupName)
   {
      return groupDict[groupName];
   }

   public List<string> GetLabel(string label)
   {
      return lableDict[label];
   }
   
   public void AddLable(string label,string assetName)
   {
      if (lableDict.ContainsKey(label))
      {
         lableDict[label].Add(assetName);
      }
      else
      {
         lableDict.Add(label,new List<string>(){assetName});
      }
   }
   
   public void AddGroup(string group,string assetName)
   {
      if (groupDict.ContainsKey(group))
      {
         groupDict[group].Add(assetName);
      }
      else
      {
         groupDict.Add(group,new List<string>(){assetName});
      }
   }
}
