// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.HabPropertiesExComparer
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.Collections;
using System.Collections.Generic;

namespace ReplaySeeker.Core
{
  public class HabPropertiesExComparer : IComparer
  {
    private KeyValuePair<string, Type>[] NameTypePairs = new KeyValuePair<string, Type>[0];
    private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

    public HabPropertiesExComparer(params KeyValuePair<string, Type>[] NameTypePairs)
    {
      this.NameTypePairs = NameTypePairs;
    }

    int IComparer.Compare(object x, object y)
    {
      return this.Compare(x as HabProperties, y as HabProperties, this.NameTypePairs, 0);
    }

    public int Compare(HabProperties hps1, HabProperties hps2)
    {
      return this.Compare(hps1, hps2, this.NameTypePairs, 0);
    }

    private int Compare(HabProperties hps1, HabProperties hps2, KeyValuePair<string, Type>[] NameTypePairs, int index)
    {
      if (index >= NameTypePairs.Length)
        return 0;
      string key = NameTypePairs[index].Key;
      Type conversionType = NameTypePairs[index].Value;
      object a = hps1.GetValue(key);
      object b = hps2.GetValue(key);
      try
      {
        a = Convert.ChangeType(a, conversionType);
        b = Convert.ChangeType(b, conversionType);
      }
      catch
      {
      }
      int num = this.cic.Compare(a, b);
      if (num == 0)
        return this.Compare(hps1, hps2, NameTypePairs, index + 1);
      return num;
    }
  }
}
