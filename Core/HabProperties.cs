// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.HabProperties
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections.Generic;
using System.Globalization;

namespace ReplaySeeker.Core
{
  public class HabProperties : Dictionary<string, object>, IConfigProperties
  {
    public string name;
    public object priority;

    public HabProperties()
      : base((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
    }

    public HabProperties(string name, params KeyValuePair<string, object>[] kvpArray)
      : base((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
      this.name = name;
      this.AddRange((ICollection<KeyValuePair<string, object>>) kvpArray);
    }

    public HabProperties(string FileName)
      : base((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
      this.Clear();
    }

    public HabProperties(IDictionary<string, object> dictionary)
      : base(dictionary, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
    }

    public HabProperties(int capacity)
      : base(capacity, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
    }

    public void Add(KeyValuePair<string, object> kvp)
    {
      this.Add(kvp.Key, kvp.Value);
    }

    public void AddEx(string name, object value)
    {
      if (this.ContainsKey(name))
        return;
      this.Add(name, value);
    }

    public void AddRange(ICollection<KeyValuePair<string, object>> kvps)
    {
      foreach (KeyValuePair<string, object> kvp in (IEnumerable<KeyValuePair<string, object>>) kvps)
        this.Add(kvp.Key, kvp.Value);
    }

    public void AddRangeEx(HabProperties hps, bool update)
    {
      if (update)
      {
        foreach (KeyValuePair<string, object> hp in (Dictionary<string, object>) hps)
          this[hp.Key] = hp.Value;
      }
      else
      {
        foreach (KeyValuePair<string, object> hp in (Dictionary<string, object>) hps)
          this.AddEx(hp.Key, hp.Value);
      }
    }

    public void SetRange(HabProperties hps)
    {
      foreach (KeyValuePair<string, object> hp in (Dictionary<string, object>) hps)
        this[hp.Key] = hp.Value;
    }

    public void Merge(HabProperties hps)
    {
      if (hps.priority != null)
        this.priority = hps.priority;
      this.SetRange(hps);
    }

    public void Merge(HabProperties hps, bool update)
    {
      if (hps.priority != null)
        this.priority = hps.priority;
      this.AddRangeEx(hps, update);
    }

    public object GetValue(string Name)
    {
      object obj;
      this.TryGetValue(Name, out obj);
      return obj;
    }

    public object GetValue(string Name, bool strictMatch)
    {
      if (strictMatch)
      {
        object obj;
        this.TryGetValue(Name, out obj);
        return obj;
      }
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) this)
      {
        if (keyValuePair.Key.Contains(Name))
          return keyValuePair.Value;
      }
      return (object) null;
    }

    public HabProperties AddHeader(string value)
    {
      HabProperties habProperties = new HabProperties();
      habProperties.name = this.name;
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) this)
        habProperties.Add(HabProperty.GetNameWithHeader(keyValuePair.Key, value), keyValuePair.Value);
      return habProperties;
    }

    public HabProperties GetCopy()
    {
      return new HabProperties((IDictionary<string, object>) this)
      {
        name = this.name,
        priority = this.priority
      };
    }

    public HabProperties GetCopyEx()
    {
      HabProperties habProperties = new HabProperties(this.Count);
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) this)
      {
        object obj = keyValuePair.Value;
        if (obj is List<string>)
          obj = (object) new List<string>((IEnumerable<string>) (obj as List<string>));
        else if (obj is HabPropertiesCollection)
          obj = (object) (obj as HabPropertiesCollection).GetCopy();
        habProperties.Add(keyValuePair.Key, obj);
      }
      habProperties.name = this.name;
      habProperties.priority = this.priority;
      return habProperties;
    }

    public void SetValueToFirstMatchedKey(string name, object value)
    {
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) this)
      {
        if (keyValuePair.Key.Contains(name))
        {
          this[keyValuePair.Key] = value;
          break;
        }
      }
    }

    public string GetStringValue(string key)
    {
      object obj;
      this.TryGetValue(key, out obj);
      if (obj is List<string>)
        return (obj as List<string>)[0];
      return obj.ToString() + "";
    }

    public string GetStringValue(string key, string retOnFail)
    {
      object obj;
      if (!this.TryGetValue(key, out obj))
        return retOnFail;
      if (obj is List<string>)
        return (obj as List<string>)[0];
      return obj.ToString() + "";
    }

    public void SetStringValue(string key, string value)
    {
      this[key] = (object) value;
    }

    public List<string> GetStringListValue(string key)
    {
      object obj;
      this.TryGetValue(key, out obj);
      if (obj is List<string>)
        return obj as List<string>;
      return HabProperty.SplitValue(obj.ToString() + "");
    }

    public void SetStringListValue(string key, List<string> value)
    {
      this[key] = (object) value;
    }

    public int GetIntValue(string key)
    {
      object obj;
      if (this.TryGetValue(key, out obj))
      {
        try
        {
          return Convert.ToInt32(obj);
        }
        catch
        {
        }
      }
      return 0;
    }

    public int GetIntValue(string key, int retOnFail)
    {
      object obj;
      if (this.TryGetValue(key, out obj))
      {
        try
        {
          return Convert.ToInt32(obj);
        }
        catch
        {
        }
      }
      return retOnFail;
    }

    public void SetIntValue(string key, int value)
    {
      this[key] = (object) value;
    }

    public double GetDoubleValue(string key)
    {
      object obj;
      if (this.TryGetValue(key, out obj))
      {
        try
        {
          return Convert.ToDouble(obj, (IFormatProvider) NumberFormatInfo.InvariantInfo);
        }
        catch
        {
        }
      }
      return 0.0;
    }

    public double GetDoubleValue(string key, double retOnFail)
    {
      object obj;
      if (this.TryGetValue(key, out obj))
      {
        try
        {
          return Convert.ToDouble(obj, (IFormatProvider) NumberFormatInfo.InvariantInfo);
        }
        catch
        {
        }
      }
      return retOnFail;
    }

    public void SetDoubleValue(string key, double value)
    {
      this[key] = (object) value;
    }

    public HabPropertiesCollection GetHpcValue(string key)
    {
      object obj;
      this.TryGetValue(key, out obj);
      if (obj is HabPropertiesCollection)
        return obj as HabPropertiesCollection;
      if (obj is string)
        return HabPropertiesCollection.ParseFrom(obj.ToString() + "");
      return new HabPropertiesCollection();
    }

    public HabProperties GetHpsValue(string key)
    {
      object obj;
      this.TryGetValue(key, out obj);
      if (obj is HabProperties)
        return obj as HabProperties;
      if (obj is string)
        return HabProperties.ParseFrom(obj.ToString() + "");
      return new HabProperties();
    }

    public HabProperties GetHpsValue(string key, bool associated)
    {
      object obj;
      this.TryGetValue(key, out obj);
      if (obj is HabProperties)
        return obj as HabProperties;
      HabProperties habProperties = !(obj is string) ? new HabProperties() : HabProperties.ParseFrom(obj.ToString() + "");
      if (associated)
        this[key] = (object) habProperties;
      return habProperties;
    }

    public static unsafe HabProperties ParseFrom(string hpsString)
    {
      HabProperties habProperties = new HabProperties();
      fixed (char* chPtr1 = hpsString)
      {
        char* chPtr2 = chPtr1;
        while ((int) *chPtr2 != 0)
        {
          while ((int) *chPtr2 != 91)
            ++chPtr2;
          char* chPtr3;
          char* chPtr4 = chPtr3 = chPtr2 + 1;
          while ((int) *chPtr3 != 93)
            ++chPtr3;
          string key = new string(chPtr4, 0, (int) (chPtr3 - chPtr4));
          char* chPtr5 = chPtr3 + 1;
          while ((int) *chPtr5 != 61)
            ++chPtr5;
          char* chPtr6 = chPtr2 = chPtr5 + 1;
          while ((int) *chPtr2 != 0 && (int) *chPtr2 != 91)
            ++chPtr2;
          string str = new string(chPtr6, 0, (int) (chPtr2 - chPtr6));
          if (key == "hpsName")
            habProperties.name = str;
          else
            habProperties.Add(key, (object) str.Trim('\''));
        }
      }
      return habProperties;
    }

    public override string ToString()
    {
      string str = "";
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) this)
        str = str + "[" + keyValuePair.Key + "]='" + HabProperty.ToStringList(keyValuePair.Value) + "'";
      return str;
    }
  }
}
