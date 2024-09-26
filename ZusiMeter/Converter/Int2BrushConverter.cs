// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Converter.Int2BrushConverter
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

#nullable disable
namespace ZusiMeter.Converter
{
  [ValueConversion(typeof (int), typeof (Brush))]
  public class Int2BrushConverter : IValueConverter
  {
    private static readonly RadialGradientBrush _red = new RadialGradientBrush(Color.FromArgb(byte.MaxValue, (byte) 253, (byte) 208, (byte) 208), Colors.Red);
    private static readonly RadialGradientBrush _yellow = new RadialGradientBrush(Color.FromArgb(byte.MaxValue, (byte) 253, (byte) 253, (byte) 208), Color.FromArgb(byte.MaxValue, (byte) 210, (byte) 210, (byte) 0));
    private static readonly RadialGradientBrush _green = new RadialGradientBrush(Color.FromArgb(byte.MaxValue, (byte) 202, byte.MaxValue, (byte) 202), Color.FromArgb(byte.MaxValue, (byte) 0, (byte) 210, (byte) 0));

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      RadialGradientBrush radialGradientBrush;
      switch ((int) value)
      {
        case 1:
          radialGradientBrush = Int2BrushConverter._yellow;
          break;
        case 2:
          radialGradientBrush = Int2BrushConverter._green;
          break;
        default:
          radialGradientBrush = Int2BrushConverter._red;
          break;
      }
      return (object) radialGradientBrush;
    }

    public object ConvertBack(
      object value,
      Type targetType,
      object parameter,
      CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
