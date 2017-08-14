// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.HabPropertiesComparer
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;

namespace ReplaySeeker.Core
{
  public class HabPropertiesComparer : IComparer
  {
    private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

    int IComparer.Compare(object x, object y)
    {
      return this.cic.Compare((x as HabProperties).priority, (y as HabProperties).priority);
    }
  }
}
