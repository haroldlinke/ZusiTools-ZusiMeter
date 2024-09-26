// Decompiled with JetBrains decompiler
// Type: ZusiMeter.ValidationRules.ValidatePort
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using System.Globalization;
using System.Windows.Controls;

#nullable disable
namespace ZusiMeter.ValidationRules
{
  public class ValidatePort : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if (value == null)
        return new ValidationResult(true, (object) null);
      string s = value as string;
      int result;
      return !string.IsNullOrEmpty(s) && int.TryParse(s, out result) && result >= 1 && result < (int) ushort.MaxValue ? new ValidationResult(true, (object) null) : new ValidationResult(false, (object) string.Format("'{0}' ist kein gültiger Port", (object) value.ToString()));
    }
  }
}
