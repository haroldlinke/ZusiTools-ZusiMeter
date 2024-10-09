// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Data.DataManager
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using ZusiMeter.Miscellaneous;
using ZusiMeter.Properties;
using ZusiMeter;
using System.ComponentModel;
using System.Diagnostics;

#nullable disable
namespace ZusiMeter.Data
{
  public class DataManager : DependencyObject
  {
    private static DataManager __instance = (DataManager) null;
    private readonly ObservableCollection<string> _layoutFiles = new ObservableCollection<string>();
    private readonly ObservableCollection<ToolBoxViewModel> _toolBox = new ObservableCollection<ToolBoxViewModel>();
    private string _selectedLayoutFile;
    private string _layoutFileName;
    private ToolBoxViewModel _tvmRoot;
    public static readonly DependencyProperty AutoUpdateProperty = DependencyProperty.Register(nameof (AutoUpdate), typeof (bool), typeof (DataManager), new PropertyMetadata((object) Settings.Default.AutoUpdate, new PropertyChangedCallback(DataManager.OnAutoUpdateChanged)));
    public static readonly DependencyPropertyKey _canShowConsoleKey = DependencyProperty.RegisterReadOnly(nameof (CanShowConsole), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    public static readonly DependencyProperty CanShowConsoleProperty = DataManager._canShowConsoleKey.DependencyProperty;
    public static readonly DependencyProperty HasLayoutProperty = DependencyProperty.Register(nameof (HasLayout), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    public static readonly DependencyProperty HasUpdatesProperty = DependencyProperty.Register(nameof (HasUpdates), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    private static readonly DependencyPropertyKey _isEvilOSKey = DependencyProperty.RegisterReadOnly(nameof (IsEvilOS), typeof (bool), typeof (DataManager), new PropertyMetadata((object) true));
    public static readonly DependencyProperty IsEvilOSProperty = DataManager._isEvilOSKey.DependencyProperty;
    public static readonly DependencyProperty IsLayoutDirtyProperty = DependencyProperty.Register(nameof (IsLayoutDirty), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    public static readonly DependencyProperty IsUpdateSearchRunningProperty = DependencyProperty.Register(nameof (IsUpdateSearchRunning), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    public static readonly DependencyProperty LayoutNameProperty = DependencyProperty.Register(nameof (LayoutName), typeof (string), typeof (DataManager), new PropertyMetadata((object) "Neues Layout"));
    public static readonly DependencyProperty PackedHeightProperty = DependencyProperty.Register(nameof (PackedHeight), typeof (double), typeof (DataManager), new PropertyMetadata((object) 0.0));
    public static readonly DependencyProperty PackedWidthProperty = DependencyProperty.Register(nameof (PackedWidth), typeof (double), typeof (DataManager), new PropertyMetadata((object) 0.0));
    public static readonly DependencyProperty ShouldLoggingProperty = DependencyProperty.Register(nameof (ShouldLogging), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    public static readonly DependencyProperty ShowConsoleProperty = DependencyProperty.Register(nameof (ShowConsole), typeof (bool), typeof (DataManager), new PropertyMetadata((object) false));
    public static readonly DependencyProperty UpdateBorderOffsetProperty = DependencyProperty.Register(nameof (UpdateBorderOffset), typeof (double), typeof (DataManager), new PropertyMetadata((object) 300.0));
    
    public bool AutoUpdate
    {
      get => (bool) this.GetValue(DataManager.AutoUpdateProperty);
      set => this.SetValue(DataManager.AutoUpdateProperty, (object) value);
    }

    public bool CanShowConsole
    {
      get => (bool) this.GetValue(DataManager.CanShowConsoleProperty);
      private set => this.SetValue(DataManager._canShowConsoleKey, (object) value);
    }

    public bool HasLayout
    {
      get => (bool) this.GetValue(DataManager.HasLayoutProperty);
      set => this.SetValue(DataManager.HasLayoutProperty, (object) value);
    }

    public bool HasUpdates
    {
      get => (bool) this.GetValue(DataManager.HasUpdatesProperty);
      set => this.SetValue(DataManager.HasUpdatesProperty, (object) value);
    }

    public bool IsEvilOS
    {
      get => (bool) this.GetValue(DataManager.IsEvilOSProperty);
      private set => this.SetValue(DataManager._isEvilOSKey, (object) value);
    }

    public bool IsLayoutDirty
    {
      get => (bool) this.GetValue(DataManager.IsLayoutDirtyProperty);
      set => this.SetValue(DataManager.IsLayoutDirtyProperty, (object) value);
    }

    public bool IsUpdateSearchRunning
    {
      get => (bool) this.GetValue(DataManager.IsUpdateSearchRunningProperty);
      set => this.SetValue(DataManager.IsUpdateSearchRunningProperty, (object) value);
    }

    public string LayoutName
    {
      get => (string) this.GetValue(DataManager.LayoutNameProperty);
      set => this.SetValue(DataManager.LayoutNameProperty, (object) value);
    }

    public double PackedHeight
    {
      get => (double) this.GetValue(DataManager.PackedHeightProperty);
      set => this.SetValue(DataManager.PackedHeightProperty, (object) value);
    }

    public double PackedWidth
    {
      get => (double) this.GetValue(DataManager.PackedWidthProperty);
      set => this.SetValue(DataManager.PackedWidthProperty, (object) value);
    }

    public string SelectedLayoutFile
    {
      get 
      {
        Debug.WriteLine($"Get SelectedLayoutFile: {_selectedLayoutFile}");
        return _selectedLayoutFile; 
      }
      set
      {
        if (_selectedLayoutFile != value)
        {
          _selectedLayoutFile = value;
          this.OnPropertyChanged(nameof(SelectedLayoutFile));
          Debug.WriteLine($"SelectedLayoutFile changed to: {_selectedLayoutFile}");
        }
        else
        {
          Debug.WriteLine($"SelectedLayoutFile NOT changed: {_selectedLayoutFile}");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool ShouldLogging
    {
      get => (bool) this.GetValue(DataManager.ShouldLoggingProperty);
      set => this.SetValue(DataManager.ShouldLoggingProperty, (object) value);
    }

    public bool ShowConsole
    {
      get => (bool) this.GetValue(DataManager.ShowConsoleProperty);
      set => this.SetValue(DataManager.ShowConsoleProperty, (object) value);
    }

    public double UpdateBorderOffset
    {
      get => (double) this.GetValue(DataManager.UpdateBorderOffsetProperty);
      set => this.SetValue(DataManager.UpdateBorderOffsetProperty, (object) value);
    }
   

    public static DataManager Instance
    {
      get
      {
        if (DataManager.__instance == null)
          DataManager.__instance = new DataManager();
        return DataManager.__instance;
      }
    }

    public string LayoutFileName
    {
      get => this._layoutFileName;
      set
      {
        this._layoutFileName = value;
        this.LayoutName = string.IsNullOrEmpty(value) ? "Neues Layout" : Path.GetFileNameWithoutExtension(value);
      }
    }

    public ObservableCollection<string> LayoutFiles => this._layoutFiles;

    public ObservableCollection<ToolBoxViewModel> ToolBox => this._toolBox;

    public ToolBoxViewModel PaletteRoot => this._tvmRoot;

    private DataManager()
    {
      this.ObtainLayoutFiles();
      Version version = Environment.OSVersion.Version;
      this.IsEvilOS = version.Major < 6 || version.Major == 6 && version.Minor < 2;
    }

    public bool CanCaption(int index) => this._tvmRoot != null && this._tvmRoot.CanCaption(index);

    public void Caption(int index) => this._tvmRoot?.Caption(index);

    public void RefreshLayouts()
    {
      this._layoutFiles.Clear();
      this.ObtainLayoutFiles();
    }

    public void SelectLayoutFile(string layoutFile)
    {
      if (_layoutFiles.Contains(layoutFile))
      {
        SelectedLayoutFile = layoutFile;
      }
    }

    public void UpdatePalette(bool textMode)
    {
      this._toolBox.Clear();
      using (PoolFile poolFile = new PoolFile())
      {
        this._tvmRoot = ToolBoxViewModel.Create(poolFile, textMode);
        foreach (ToolBoxViewModel child in (Collection<ToolBoxViewModel>) this._tvmRoot.Children)
          this._toolBox.Add(child);
      }
    }

    private void ObtainLayoutFiles()
    {
      //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ZusiMeterLayouts");
      string path = MainWindow.GetZusiMeterLayoutFileDir();
      if (!Directory.Exists(path))
        return;
      foreach (string enumerateFile in Directory.EnumerateFiles(path, "*.zmlf", SearchOption.TopDirectoryOnly))
      {
        if (!enumerateFile.Contains<char>('~'))
          this._layoutFiles.Add(enumerateFile);
      }
      if (!string.IsNullOrEmpty(this.LayoutFileName))
      {
        SelectLayoutFile(this.LayoutFileName);
      }
    }

    private static void OnAutoUpdateChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as DataManager).OnAutoUpdateChanged((bool) e.NewValue);
    }

    private void OnAutoUpdateChanged(bool value)
    {
      Settings.Default.AutoUpdate = value;
      Settings.Default.Save();
    }
  }
}
