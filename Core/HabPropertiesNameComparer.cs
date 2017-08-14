// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.HabPropertiesNameComparer
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Collections;

namespace ReplaySeeker.Core
{
  public class HabPropertiesNameComparer : IComparer
  {
    private CaseInsensitiveComparer cic = new CaseInsensitiveComparer();

    int IComparer.Compare(object x, object y)
    {
      return this.cic.Compare((object) (x as HabProperties).name, (object) (y as HabProperties).name);
    }
  }
}
