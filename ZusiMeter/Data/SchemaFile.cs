// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Data.SchemaFile
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System.IO;
using System.Reflection;

#nullable disable
namespace ZusiMeter.Data
{
  internal sealed class SchemaFile : ProtectedFile
  {
    private const string DefaultSchemaFileName = "zusimeter.xsd";

    public SchemaFile()
      : base(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "zusimeter.xsd"))
    {
    }
  }
}
