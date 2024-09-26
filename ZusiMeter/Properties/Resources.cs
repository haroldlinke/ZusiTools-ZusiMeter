// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Properties.Resources
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

#nullable disable
namespace ZusiMeter.Properties
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ZusiMeter.Properties.Resources.resourceMan == null)
          ZusiMeter.Properties.Resources.resourceMan = new ResourceManager("ZusiMeter.Properties.Resources", typeof (ZusiMeter.Properties.Resources).Assembly);
        return ZusiMeter.Properties.Resources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ZusiMeter.Properties.Resources.resourceCulture;
      set => ZusiMeter.Properties.Resources.resourceCulture = value;
    }
  }
}
