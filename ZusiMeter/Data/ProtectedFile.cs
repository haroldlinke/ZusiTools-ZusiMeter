// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Data.ProtectedFile
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using Sovoma;
using System.IO;

#nullable disable
namespace ZusiMeter.Data
{
  internal class ProtectedFile : Disposable
  {
    private Stream _stream;

    public Stream Stream => this._stream;

    public ProtectedFile(string fileName)
    {
      if (File.Exists(fileName))        
        _stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
    }

    public void Rewind()
    {
      Stream stream = this._stream;
      if (stream == null)
        return;
      StreamEx.Rewind(stream);
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Disposable.Dispose<Stream>(ref this._stream);
    }
  }
}
