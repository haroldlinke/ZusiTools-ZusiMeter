// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Properties.Settings
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ZusiMeterGaugesLib.Common;

#nullable disable
namespace ZusiMeter.Properties
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
    [DefaultSettingValue("")]
    public string HostOrIP
    {
      get => (string) this[nameof (HostOrIP)];
      set => this[nameof (HostOrIP)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("1436")]
    public int Port
    {
      get => (int) this[nameof (Port)];
      set => this[nameof (Port)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string PrivateLayoutFolder
    {
      get => (string)this[nameof(PrivateLayoutFolder)];
      set => this[nameof(PrivateLayoutFolder)] = (object)value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string ExampleLayoutFolder
    {
      get => (string)this[nameof(ExampleLayoutFolder)];
      set => this[nameof(ExampleLayoutFolder)] = (object)value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("0.5")]
    public double RailRunnerVolume
    {
      get => (double) this[nameof (RailRunnerVolume)];
      set => this[nameof (RailRunnerVolume)] = (object) value;
    }
	
	

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool VintageBackground
    {
      get => (bool) this[nameof (VintageBackground)];
      set => this[nameof (VintageBackground)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("0")]
    public int ZusiConfiguration
    {
      get => (int) this[nameof (ZusiConfiguration)];
      set => this[nameof (ZusiConfiguration)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("1,2")]
    public string OptionListSelectedItems
    {
      get => (string)this[nameof(OptionListSelectedItems)];
      set => this[nameof(OptionListSelectedItems)] = (object)value;
    }
  }
}
