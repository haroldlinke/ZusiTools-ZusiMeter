// Decompiled with JetBrains decompiler
// Type: ZusiMeter.ValidationRules.ValidateHostOrIP
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using System.Globalization;
using System.Net;
using System.Windows.Controls;

#nullable disable
namespace ZusiMeter.ValidationRules
{
  public class ValidateHostOrIP : ValidationRule
  {
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
      if (string.IsNullOrEmpty(value as string))
        return new ValidationResult(true, (object) null);
      try
      {
        IPAddress[] hostAddresses = Dns.GetHostAddresses((string) value);
        if (hostAddresses != null)
        {
          if (hostAddresses.Length != 0)
            return new ValidationResult(true, (object) null);
        }
      }
      catch
      {
      }
      return new ValidationResult(false, (object) string.Format("'{0}' ist kein gültiger Hostname oder keine gültige IP", (object) value.ToString()));
    }
  }
}
