// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.HabPropertiesCollection
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReplaySeeker.Core
{
  public class HabPropertiesCollection : Dictionary<string, HabProperties>, IEnumerable<HabProperties>, IEnumerable
  {
    public new HabProperties this[string name]
    {
      get
      {
        HabProperties habProperties;
        if (this.TryGetValue(name, out habProperties))
          return habProperties;
        return (HabProperties) null;
      }
      set
      {
        base[name] = value;
      }
    }

    public object this[string hpsname, string pname]
    {
      get
      {
        HabProperties habProperties;
        if (this.TryGetValue(hpsname, out habProperties))
          return habProperties.GetValue(pname);
        return (object) null;
      }
      set
      {
        HabProperties habProperties1;
        if (this.TryGetValue(hpsname, out habProperties1))
        {
          habProperties1[pname] = value;
        }
        else
        {
          HabProperties habProperties2 = new HabProperties(1);
          habProperties2[pname] = value;
          base[hpsname] = habProperties2;
        }
      }
    }

    public HabPropertiesCollection()
      : base((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
    }

    public HabPropertiesCollection(int capacity)
      : base(capacity, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
    }

    public HabPropertiesCollection(MemoryStream ms)
      : base((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase)
    {
      HabPropertiesCollection.LoadFromMemoryFast(ms, this);
    }

    public new void Add(string name, HabProperties hps)
    {
      hps.name = name;
      base.Add(this.GetNewKeyName(hps.name), hps);
    }

    public void AddUnchecked(string name, HabProperties hps)
    {
      hps.name = name;
      base.Add(name, hps);
    }

    public void Add(HabProperties hps)
    {
      base.Add(this.GetNewKeyName(hps.name), hps);
    }

    public void AddUnchecked(HabProperties hps)
    {
      base.Add(hps.name, hps);
    }

    public void Add(KeyValuePair<string, HabProperties> kvp)
    {
      base.Add(kvp.Key, kvp.Value);
    }

    public void Add(HabProperties hps, bool update)
    {
      string name = hps.name;
      if (update)
      {
        HabProperties habProperties;
        if (this.TryGetValue(name, out habProperties))
          habProperties.AddRange((ICollection<KeyValuePair<string, object>>) hps);
        else
          base.Add(name, hps);
      }
      else
        base.Add(this.GetNewKeyName(name), hps);
    }

    public void AddEx(HabProperties hps)
    {
      string name = hps.name;
      if (this.ContainsKey(name))
        return;
      base.Add(name, hps);
    }

    public void AddRange(HabPropertiesCollection hpc)
    {
      foreach (KeyValuePair<string, HabProperties> keyValuePair in (Dictionary<string, HabProperties>) hpc)
        base.Add(keyValuePair.Key, keyValuePair.Value);
    }

    public string GetNewKeyName(string name)
    {
      if (string.IsNullOrEmpty(name))
        return Guid.NewGuid().ToString();
      if (this.ContainsKey(name))
        return name + Guid.NewGuid().ToString();
      return name;
    }

    public void Merge(HabPropertiesCollection hpc)
    {
      foreach (KeyValuePair<string, HabProperties> keyValuePair in (Dictionary<string, HabProperties>) hpc)
      {
        HabProperties habProperties;
        if (this.TryGetValue(keyValuePair.Key, out habProperties))
          habProperties.Merge(keyValuePair.Value);
        else
          base.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public void Merge(HabPropertiesCollection hpc, bool onlyExisting)
    {
      foreach (KeyValuePair<string, HabProperties> keyValuePair in (Dictionary<string, HabProperties>) hpc)
      {
        HabProperties habProperties;
        if (this.TryGetValue(keyValuePair.Key, out habProperties))
          habProperties.Merge(keyValuePair.Value);
        else if (!onlyExisting)
          base.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public void Merge(HabPropertiesCollection hpc, bool onlyExisting, bool update)
    {
      foreach (KeyValuePair<string, HabProperties> keyValuePair in (Dictionary<string, HabProperties>) hpc)
      {
        HabProperties habProperties;
        if (this.TryGetValue(keyValuePair.Key, out habProperties))
          habProperties.Merge(keyValuePair.Value, update);
        else if (!onlyExisting)
          base.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public HabProperties GetValue(string Name)
    {
      HabProperties habProperties;
      if (this.TryGetValue(Name, out habProperties))
        return habProperties;
      return (HabProperties) null;
    }

    public string GetStringValue(string hpsname, string pname)
    {
      HabProperties habProperties;
      if (this.TryGetValue(hpsname, out habProperties))
        return habProperties.GetStringValue(pname);
      return "";
    }

    public HabProperties GetByOrder(int index)
    {
      foreach (HabProperties habProperties in this)
      {
        if (--index < 0)
          return habProperties;
      }
      return (HabProperties) null;
    }

    public void ReadFromFile(string filename)
    {
      this.Clear();
      if (!File.Exists(filename))
        return;
      FileStream fileStream = new FileStream(filename, FileMode.Open);
      byte[] buffer = new byte[fileStream.Length];
      fileStream.Read(buffer, 0, (int) fileStream.Length);
      MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length, false, true);
      HabPropertiesCollection.LoadFromMemoryFast(ms, this);
      ms.Close();
      fileStream.Close();
    }

    public void SaveToFile(string filename)
    {
      using (StreamWriter streamWriter = new StreamWriter((Stream) new FileStream(filename, FileMode.Create)))
      {
        foreach (HabProperties habProperties in this)
        {
          streamWriter.WriteLine("[" + habProperties.name + "]");
          foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) habProperties)
            streamWriter.WriteLine(keyValuePair.Key + "=" + HabProperty.ToStringList(keyValuePair.Value));
        }
      }
    }

    public static HabPropertiesCollection ParseFrom(string hpcString)
    {
      HabPropertiesCollection propertiesCollection = new HabPropertiesCollection();
      string str = hpcString;
      char[] chArray = new char[1]{ ';' };
      foreach (string hpsString in str.Split(chArray))
        propertiesCollection.Add(HabProperties.ParseFrom(hpsString));
      return propertiesCollection;
    }

    public static unsafe void LoadFromMemoryFast(MemoryStream ms, HabPropertiesCollection hpc)
    {
      byte[] buffer = ms.GetBuffer();
      if (buffer.Length == 0)
        return;
      fixed (byte* numPtr1 = buffer)
      {
        if ((int) *(uint*) numPtr1 << 8 == -1078202624)
        {
          char[] chars = Encoding.UTF8.GetChars(buffer, 3, buffer.Length - 3);
          fixed (char* chPtr1 = chars)
          {
            char* ptr = chPtr1;
            char* chPtr2 = ptr + chars.Length;
            HabProperties hps = (HabProperties) null;
            string name;
            while (ptr < chPtr2)
            {
              switch (*ptr)
              {
                case '\r':
                  ++ptr;
                  while ((int) *ptr++ != 10)
                    ;
                  continue;
                case '/':
                  ++ptr;
                  while ((int) *ptr++ != 10)
                    ;
                  continue;
                case '[':
                  name = HabPropertiesCollection.checkNewHPC(ref ptr);
                  do
                    ;
                  while ((int) *ptr++ != 10);
                  if (!hpc.TryGetValue(name, out hps))
                  {
                    hps = new HabProperties();
                    hpc.AddUnchecked(name, hps);
                    continue;
                  }
                  continue;
                default:
                  string str;
                  HabProperty.FromStringFastUnsafe(ref ptr, out name, out str);
                  hps[name] = (object) str;
                  continue;
              }
            }
          }
        }
        else
        {
          sbyte* ptr = (sbyte*) numPtr1;
          sbyte* numPtr2 = ptr + buffer.Length;
          HabProperties hps = (HabProperties) null;
          string name;
          while (ptr < numPtr2)
          {
            switch ((char) *ptr)
            {
              case '\r':
                ++ptr;
                while ((int) *ptr++ != 10)
                  ;
                continue;
              case '/':
                ++ptr;
                while ((int) *ptr++ != 10)
                  ;
                continue;
              case '[':
                name = HabPropertiesCollection.checkNewHPC(ref ptr);
                do
                  ;
                while ((int) *ptr++ != 10);
                if (!hpc.TryGetValue(name, out hps))
                {
                  hps = new HabProperties();
                  hpc.AddUnchecked(name, hps);
                  continue;
                }
                continue;
              default:
                string str;
                HabProperty.FromStringFastUnsafe(ref ptr, out name, out str);
                hps[name] = (object) str;
                continue;
            }
          }
        }
      }
    }

    private static unsafe string checkNewHPC(ref char* ptr)
    {
      char* chPtr = ++ptr;
      while ((int) *ptr != 93)
        ++ptr;
      return new string(chPtr, 0, (int) (ptr++ - chPtr));
    }

    private static unsafe string checkNewHPC(ref sbyte* ptr)
    {
      sbyte* numPtr = ++ptr;
      while ((int) *ptr != 93)
        ++ptr;
      return new string(numPtr, 0, (int) (ptr++ - numPtr));
    }

    public override string ToString()
    {
      string str = "";
      foreach (HabProperties habProperties in this.Values)
        str = str + habProperties.ToString() + ";";
      return str.TrimEnd(';');
    }

    public string ToString(bool named)
    {
      string str = "";
      foreach (HabProperties habProperties in this.Values)
      {
        if (named)
          str = str + "[hpsName]=" + habProperties.name;
        str = str + habProperties.ToString() + ";";
      }
      return str.TrimEnd(';');
    }

    public HabPropertiesCollection GetCopy()
    {
      HabPropertiesCollection propertiesCollection = new HabPropertiesCollection(this.Count);
      foreach (HabProperties habProperties in this)
        propertiesCollection.Add(habProperties.GetCopy());
      return propertiesCollection;
    }

    public HabPropertiesCollection GetCopyEx()
    {
      HabPropertiesCollection propertiesCollection = new HabPropertiesCollection(this.Count);
      foreach (HabProperties habProperties in this)
        propertiesCollection.Add(habProperties.GetCopyEx());
      return propertiesCollection;
    }

    public IEnumerator<HabProperties> GetEnumerator()
    {
      return (IEnumerator<HabProperties>) this.Values.GetEnumerator();
    }

    public void RenameUnchecked(string oldName, string newName)
    {
      HabProperties hps;
      if (!this.TryGetValue(oldName, out hps))
        return;
      this.Remove(oldName);
      this.AddUnchecked(newName, hps);
    }

    public void MakeCopyUnchecked(string orgKey, string copyKey)
    {
      HabProperties habProperties;
      if (!this.TryGetValue(orgKey, out habProperties))
        return;
      this.AddUnchecked(copyKey, habProperties.GetCopy());
    }

    public void MakeCopyUncheckedEx(string orgKey, string copyKey)
    {
      HabProperties habProperties;
      if (!this.TryGetValue(orgKey, out habProperties))
        return;
      this.AddUnchecked(copyKey, habProperties.GetCopyEx());
    }
  }
}
