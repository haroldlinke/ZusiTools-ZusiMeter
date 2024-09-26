// Decompiled with JetBrains decompiler
// Type: ZusiMeter.App
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using log4net;
using log4net.Config;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

#nullable disable
namespace ZusiMeter
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
      int num = (int) System.Windows.MessageBox.Show(e.Exception.Message, "Zusi•Meter - Der Fehler lässt sich nicht gerade biegen", MessageBoxButton.OK);
      this.MainWindow?.Close();
    }
  
  }
}
