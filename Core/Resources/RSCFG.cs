// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Core.Resources.RSCFG
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace ReplaySeeker.Core.Resources
{
  public class RSCFG
  {
    private static string CfgFileName = "repseek.cfg";
    private static HabPropertiesCollection hpcConfig = new HabPropertiesCollection();
    public static RSCFG Items = new RSCFG();

    public static string FileName
    {
      get
      {
        return RSCFG.CfgFileName;
      }
    }

    public HabProperties this[string name]
    {
      get
      {
        HabProperties habProperties;
        if (RSCFG.hpcConfig.TryGetValue(name, out habProperties))
          return habProperties;
        HabProperties hps = new HabProperties();
        RSCFG.hpcConfig.Add(name, hps);
        return hps;
      }
      set
      {
        RSCFG.hpcConfig[name] = value;
      }
    }

    static RSCFG()
    {
      RSCFG.CfgFileName = Application.StartupPath + "\\" + RSCFG.CfgFileName;
      RSCFG.hpcConfig.ReadFromFile(RSCFG.CfgFileName);
    }

    ~RSCFG()
    {
      RSCFG.Save();
    }

    public static void WakeUp()
    {
    }

    public static void Save()
    {
      CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture.Clone() as CultureInfo;
      cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
      Thread.CurrentThread.CurrentCulture = cultureInfo;
      RSCFG.hpcConfig.SaveToFile(RSCFG.CfgFileName);
    }
  }
}
