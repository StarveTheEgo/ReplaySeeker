// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Properties.Resources
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 49D4BBC3-DA84-4680-9F28-5BFBA7523C67
// Assembly location: C:\Users\Mr.&Mrs.Deer\Downloads\DotA Replay Seeker - Dota WorkShop\ReplaySeeker.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;

namespace ReplaySeeker.Properties
{
  [CompilerGenerated]
  [DebuggerNonUserCode]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) ReplaySeeker.Properties.Resources.resourceMan, (object) null))
          ReplaySeeker.Properties.Resources.resourceMan = new ResourceManager("ReplaySeeker.Properties.Resources", typeof (ReplaySeeker.Properties.Resources).Assembly);
        return ReplaySeeker.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get
      {
        return ReplaySeeker.Properties.Resources.resourceCulture;
      }
      set
      {
        ReplaySeeker.Properties.Resources.resourceCulture = value;
      }
    }

    internal static Bitmap avatar
    {
      get
      {
        return (Bitmap) ReplaySeeker.Properties.Resources.ResourceManager.GetObject("avatar", ReplaySeeker.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap BTNThirst
    {
      get
      {
        return (Bitmap) ReplaySeeker.Properties.Resources.ResourceManager.GetObject("BTNThirst", ReplaySeeker.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap Control_Sound
    {
      get
      {
        return (Bitmap) ReplaySeeker.Properties.Resources.ResourceManager.GetObject("Control_Sound", ReplaySeeker.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap DISBTNThirst
    {
      get
      {
        return (Bitmap) ReplaySeeker.Properties.Resources.ResourceManager.GetObject("DISBTNThirst", ReplaySeeker.Properties.Resources.resourceCulture);
      }
    }

    internal static Bitmap DonTomaso
    {
      get
      {
        return (Bitmap) ReplaySeeker.Properties.Resources.ResourceManager.GetObject("DonTomaso", ReplaySeeker.Properties.Resources.resourceCulture);
      }
    }

    internal static Icon Icon
    {
      get
      {
        return (Icon) ReplaySeeker.Properties.Resources.ResourceManager.GetObject("Icon", ReplaySeeker.Properties.Resources.resourceCulture);
      }
    }

    internal Resources()
    {
    }
  }
}
