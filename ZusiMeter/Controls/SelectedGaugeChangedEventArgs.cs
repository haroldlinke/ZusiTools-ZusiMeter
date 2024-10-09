// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Controls.SelectedGaugeChangedEventArgs
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System;
using ZusiMeterGaugesLib.Interfaces;

#nullable disable
namespace ZusiMeter.Controls
{
  public class SelectedGaugeChangedEventArgs : EventArgs
  {
    public IGauge Gauge { get; private set; }

    public IGaugeControl SelectedControl { get; private set; }

    public SelectedGaugeChangedEventArgs(IGaugeControl control)
    {
      this.SelectedControl = control;
      this.Gauge = control is IGauge igauge ? igauge : control?.Device;
    }
  }
}
