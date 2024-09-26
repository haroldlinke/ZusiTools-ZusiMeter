// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Properties.Settings
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ZusiMeterGaugesLib.Common;

#nullable disable
namespace ZusiMeterKonfigurator.Properties
{
  [CompilerGenerated]
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default => Settings.defaultInstance;

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool AutoUpdate
    {
      get => (bool) this[nameof (AutoUpdate)];
      set => this[nameof (AutoUpdate)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    public BackgroundSetting DefaultDialBackground
    {
      get => (BackgroundSetting) this[nameof (DefaultDialBackground)];
      set => this[nameof (DefaultDialBackground)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    public BackgroundSetting DefaultTextBackground
    {
      get => (BackgroundSetting) this[nameof (DefaultTextBackground)];
      set => this[nameof (DefaultTextBackground)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool VintageBackground
    {
      get => (bool) this[nameof (VintageBackground)];
      set => this[nameof (VintageBackground)] = (object) value;
    }
  }
}
