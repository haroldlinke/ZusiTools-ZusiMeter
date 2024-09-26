// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Data.PoolFile
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using System.IO;
using System.Reflection;

#nullable disable
namespace ZusiMeterKonfigurator.Data
{
  internal sealed class PoolFile : ProtectedFile
  {
    private const string DefaultPoolFileName = "GaugePool.xml";

    public PoolFile()
      : base(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "GaugePool.xml"))
    {
    }
  }
}
