// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Converter.PackedSizeConverter
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

#nullable disable
namespace ZusiMeterKonfigurator.Converter
{
  public class PackedSizeConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      for (int index = 0; index < values.Length; ++index)
      {
        if (!(values[index] is double))
          return (object) null;
      }
      double horizontalBorderHeight = SystemParameters.FixedFrameHorizontalBorderHeight;
      double windowCaptionHeight = SystemParameters.WindowCaptionHeight;
      double verticalBorderWidth = SystemParameters.FixedFrameVerticalBorderWidth;
      double num1 = verticalBorderWidth + Math.Max(75.0, (double) values[0]) + verticalBorderWidth;
      double val2 = 0.0;
      for (int index = 1; index < values.Length; ++index)
        val2 += (double) values[index];
      double num2 = Math.Max(20.0, val2) + (horizontalBorderHeight + windowCaptionHeight + horizontalBorderHeight);
      return (object) string.Format("Resultierende Größe des ZusiMeter-Fensters: {0} x {1}", (object) num1, (object) num2);
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
