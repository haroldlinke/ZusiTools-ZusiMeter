// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.About.AboutDlg
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

#nullable disable
namespace ZusiMeterKonfigurator.About
{
  public partial class AboutDlg : Window, IComponentConnector
  {
    public static readonly RoutedUICommand CommandClose = new RoutedUICommand("Schließen", nameof (CommandClose), typeof (AboutDlg));

    public AboutDlg()
    {
      this.InitializeComponent();
      this.CommandBindings.Add(new CommandBinding((ICommand) AboutDlg.CommandClose, (ExecutedRoutedEventHandler) ((s, e) => this.DialogResult = new bool?(true)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = true)));
    }
  
  }
}
