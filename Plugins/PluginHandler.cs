// Decompiled with JetBrains decompiler
// Type: ReplaySeeker.Plugins.PluginHandler
// Assembly: ReplaySeeker, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ReplaySeeker.Plugins
{
  public class PluginHandler
  {
    public static string PluginFolderName = "Plugins";

    public static void PrepareFolder()
    {
      string path = Application.StartupPath + "\\" + PluginHandler.PluginFolderName;
      if (Directory.Exists(path))
        return;
      Directory.CreateDirectory(path);
    }

    public static PluginCollection LoadPlugins()
    {
      string[] files = Directory.GetFiles(Application.StartupPath + "\\" + PluginHandler.PluginFolderName, "*.dll");
      PluginCollection pluginCollection = new PluginCollection();
      foreach (string path in files)
      {
        try
        {
          foreach (System.Type type in Assembly.LoadFile(path).GetTypes())
          {
            if (type.GetInterface("IReplaySeekerPlugin") != null && !type.IsAbstract)
            {
              IReplaySeekerPlugin instance = (IReplaySeekerPlugin) Activator.CreateInstance(type);
              pluginCollection.Add(instance);
            }
          }
        }
        catch (Exception ex)
        {
          if (!(ex is BadImageFormatException))
          {
            int num = (int) MessageBox.Show("Couldn't load plugin: " + path + "\n" + ex.Message);
          }
        }
      }
      return pluginCollection;
    }
  }
}
