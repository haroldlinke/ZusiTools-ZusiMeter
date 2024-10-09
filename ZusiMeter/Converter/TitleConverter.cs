// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Converter.TitleConverter
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

#nullable disable
namespace ZusiMeter.Converter
{
  public class TitleConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append((string) values[0]);
      if ((bool) values[1])
        stringBuilder.Append("*");
      stringBuilder.Append(" - ");
      stringBuilder.Append((string) values[2]);
      stringBuilder.Append((string) parameter);
      return (object) stringBuilder.ToString();
    }

    public object[] ConvertBack(
      object value,
      Type[] targetTypes,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
