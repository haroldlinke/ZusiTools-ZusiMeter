// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Pages.StartPage
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using ZusiMeterGaugesLib.Controls;

#nullable disable
namespace ZusiMeter.Pages
{
  public partial class StartPage : UserControl, IComponentConnector
  {
   public StartPage() => this.InitializeComponent();

    private void LayoutItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
      MainWindow.CommandLoadLayout.Execute((object) null, (IInputElement) null);
    }

    private void LvLayoutFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count <= 0)
        return;
      this.preview.ShowPreview((string) e.AddedItems[0]);
    }
   
  }
}
