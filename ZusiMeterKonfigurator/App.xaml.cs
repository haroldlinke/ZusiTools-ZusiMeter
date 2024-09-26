// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.App
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using log4net;
using log4net.Config;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

#nullable disable
namespace ZusiMeterKonfigurator
{
  public partial class App : Application
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof (App));
    
    public App()
    {
      this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(this.App_DispatcherUnhandledException);
      GlobalContext.Properties["LogPath"] = (object) Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      XmlConfigurator.Configure();
    }

    private void App_DispatcherUnhandledException(
      object sender,
      DispatcherUnhandledExceptionEventArgs e)
    {
      App._log.Fatal((object) e.Exception.ToString());
      int num = (int) System.Windows.MessageBox.Show(e.Exception.Message, "Zusi•Meter Konfigurator - Der Fehler lässt sich nicht gerade biegen", MessageBoxButton.OK);
      this.MainWindow.Close();
    }
    
  }
}
