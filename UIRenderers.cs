// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.UIRenderers
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using ReplaySeeker.Core.Resources;

namespace ReplaySeeker
{
  public class UIRenderers
  {
    private static NoBorderRenderer noBorderRenderer;

    public static NoBorderRenderer NoBorderRenderer
    {
      get
      {
        if (UIRenderers.noBorderRenderer == null)
          UIRenderers.noBorderRenderer = new NoBorderRenderer();
        return UIRenderers.noBorderRenderer;
      }
    }
  }
}
