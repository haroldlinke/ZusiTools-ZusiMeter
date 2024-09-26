// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Miscellaneous.WindowPosition
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: 7FD5A0AE-3235-40D4-8590-30227303A956
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter (3)\ZusiMeter.exe

using log4net;
using Sovoma;
using System;
using System.IO;

#nullable disable
namespace ZusiMeter.Miscellaneous
{
  internal class WindowPosition : Singleton<WindowPosition>, ISingletonBase
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof (WindowPosition));
    private const string Section = "ZusiMeter";
    private const string KeyLeft = "Left";
    private const string KeyTop = "Top";
    private const string KeyTopmost = "Topmost";
    private readonly string _iniFilePath;
    private readonly IniFile _iniFile;
    private double _left = (double) int.MinValue;
    private double _top = (double) int.MinValue;
    private bool _topmost;

    private double Left_impl
    {
      get => this._left;
      set
      {
        if (this._left == value)
          return;
        this._left = value;
        this._iniFile.Write("Left", "ZusiMeter", string.Format("{0}", (object) this._left));
      }
    }

    private double Top_impl
    {
      get => this._top;
      set
      {
        if (this._top == value)
          return;
        this._top = value;
        this._iniFile.Write("Top", "ZusiMeter", string.Format("{0}", (object) this._top));
      }
    }

    private bool Topmost_impl
    {
      get => this._topmost;
      set
      {
        if (this._topmost == value)
          return;
        this._topmost = value;
        this._iniFile.Write("Topmost", "ZusiMeter", string.Format("{0}", (object) this._topmost));
      }
    }

    public WindowPosition()
    {
      this._iniFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ZusiMeter\\window.ini");
      WindowPosition._log.Debug((object) string.Format("ini file path: {0} ({1})", (object) this._iniFilePath, (object) File.Exists(this._iniFilePath)));
      this._iniFile = new IniFile(this._iniFilePath);
    }

    public void Initialize()
    {
      try
      {
        if (File.Exists(this._iniFilePath))
        {
          double.TryParse(this._iniFile.Read("Left", "ZusiMeter"), out this._left);
          double.TryParse(this._iniFile.Read("Top", "ZusiMeter"), out this._top);
          bool.TryParse(this._iniFile.Read("Topmost", "ZusiMeter"), out this._topmost);
          WindowPosition._log.Debug((object) string.Format("got data from ini file: left {0}, top: {1}, topmost: {2}", (object) this._left, (object) this._top, (object) this._topmost));
        }
        else
          WindowPosition._log.Debug((object) "ini file doesn't exists");
      }
      catch (Exception ex)
      {
        WindowPosition._log.Error((object) ex.ToString());
        this._left = this._top = (double) int.MinValue;
        this._topmost = false;
      }
    }

    private bool IsValid_impl()
    {
      return this._left > (double) int.MinValue && this._top > (double) int.MinValue;
    }

    public static bool IsValid => Singleton<WindowPosition>.Instance.IsValid_impl();

    public static double Left
    {
      get => Singleton<WindowPosition>.Instance.Left_impl;
      set => Singleton<WindowPosition>.Instance.Left_impl = value;
    }

    public static double Top
    {
      get => Singleton<WindowPosition>.Instance.Top_impl;
      set => Singleton<WindowPosition>.Instance.Top_impl = value;
    }

    public static bool Topmost
    {
      get => Singleton<WindowPosition>.Instance.Topmost_impl;
      set => Singleton<WindowPosition>.Instance.Topmost_impl = value;
    }
  }
}
