// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.MainWindow
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using Microsoft.Win32;
using Sovoma.WPF;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using ZusiKlassenLib;
using ZusiMeterGaugesLib.Common;
using ZusiMeterGaugesLib.Editors;
using ZusiMeterKonfigurator.About;
using ZusiMeterKonfigurator.Data;
using ZusiMeterKonfigurator.Pages;
using ZusiMeterKonfigurator.Properties;
using Zusisuplib;

#nullable disable
namespace ZusiMeterKonfigurator
{
  public partial class MainWindow : Window, IComponentConnector
  {
    public static readonly RoutedUICommand CommandWillNewLayout = new RoutedUICommand("_Neues Layout", nameof (CommandWillNewLayout), typeof (MainWindow), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.N, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandNewGraphicLayout = new RoutedUICommand("_Grafiklayout anlegen", nameof (CommandNewGraphicLayout), typeof (MainWindow));
    public static readonly RoutedUICommand CommandNewTextLayout = new RoutedUICommand("_Textlayout anlegen", nameof (CommandNewTextLayout), typeof (MainWindow));
    public static readonly RoutedUICommand CommandLoadLayout = new RoutedUICommand("Layout la_den", nameof (CommandLoadLayout), typeof (MainWindow));
    public static readonly RoutedUICommand CommandOpenLayout = new RoutedUICommand("Layout ö_ffnen...", nameof (CommandOpenLayout), typeof (MainWindow), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.O, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandBackToLayout = new RoutedUICommand("Layout _weiter bearbeiten »", nameof (CommandBackToLayout), typeof (MainWindow));
    public static readonly RoutedUICommand CommandQuit = new RoutedUICommand("_Beenden", nameof (CommandQuit), typeof (MainWindow), CommandKey.Alt_F(Key.F4));
    public static readonly RoutedUICommand CommandAbout = new RoutedUICommand("Über _ZusiMeter", nameof (CommandAbout), typeof (MainWindow));
    public static readonly RoutedUICommand CommandCheckUpdates = new RoutedUICommand("_Update suchen", nameof (CommandCheckUpdates), typeof (MainWindow));
    public static readonly RoutedUICommand CommandInstallUpdates = new RoutedUICommand("Update _installieren", nameof (CommandInstallUpdates), typeof (MainWindow));
    public static readonly RoutedUICommand CommandHideUpdatesBorder = new RoutedUICommand("Später", nameof (CommandHideUpdatesBorder), typeof (MainWindow));
    private static readonly string _defaultSavePrompt = "Soll das aktuelle Layout schnell noch gespeichert werden?";
    private static readonly string _defaultSavePrompt2 = "Das aktuelle Layout ist leer. Soll dieses Layout gelöscht werden?";
    private static string? _currentlayoutfolder = null;

        public event RoutedEventHandler DefaultDialBackgroundSettingsChanged
    {
      add
      {
        this.AddHandler(BrushEditor.DefaultDialBackgroundSettingsChangedEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(BrushEditor.DefaultDialBackgroundSettingsChangedEvent, (Delegate) value);
      }
    }

    public event RoutedEventHandler DefaultTextBackgroundSettingsChanged
    {
      add
      {
        this.AddHandler(BrushEditor.DefaultTextBackgroundSettingsChangedEvent, (Delegate) value);
      }
      remove
      {
        this.RemoveHandler(BrushEditor.DefaultTextBackgroundSettingsChangedEvent, (Delegate) value);
      }
    }
        

    public MainWindow()
    {
      // if (Settings.Default.VintageBackground)  **HLI
      //  BackgroundSettings.DefaultDialBackground.SetVintageBackground(); **HLI
      this.InitializeComponent();
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandWillNewLayout, new ExecutedRoutedEventHandler(this.OnWillNewLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandNewGraphicLayout, new ExecutedRoutedEventHandler(this.OnNewGraphicLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandNewTextLayout, new ExecutedRoutedEventHandler(this.OnNewTextLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandLoadLayout, new ExecutedRoutedEventHandler(this.OnLoadLayout), new CanExecuteRoutedEventHandler(this.OnCanLoadLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandOpenLayout, new ExecutedRoutedEventHandler(this.OnOpenLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandBackToLayout, new ExecutedRoutedEventHandler(this.OnBackToLayout), new CanExecuteRoutedEventHandler(this.OnCanBackToLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandQuit, (ExecutedRoutedEventHandler) ((s, e) => this.Close())));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandAbout, new ExecutedRoutedEventHandler(this.OnAbout)));
      this.Closing += new CancelEventHandler(this.MainWindow_Closing);
      this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
      this.DefaultDialBackgroundSettingsChanged += new RoutedEventHandler(this.MainWindow_DefaultDialBackgroundSettingsChanged);
      this.DefaultTextBackgroundSettingsChanged += new RoutedEventHandler(this.MainWindow_DefaultTextBackgroundSettingsChanged);
    }
    
    public static string GetZusiMeterLayoutFileDir()
        {
            if (_currentlayoutfolder == null)
            {
                string folderpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ZusiMeterLayouts");

                if (!Directory.Exists(folderpath))
                {
                    string zusifolderpath = Zusi.DataPath[2]; // public data zusi folder
                    folderpath = Path.Combine(zusifolderpath, "_Tools\\ZusiMeter\\ZusiMeterLayouts");
                    //folderpath = Path.Combine(Zusiaccess.GetZUSIUserfiledir(folderpath), "ZusiMeterLayouts");
                    if (!Directory.Exists(folderpath))
                    {
                        Directory.CreateDirectory(folderpath);

                    }
                }
                _currentlayoutfolder = folderpath;
            }

            return _currentlayoutfolder;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      e.Cancel = !this.SavePropmt((string) null);
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      BackgroundSettings.ApplyUserSettings(Settings.Default.DefaultDialBackground, Settings.Default.DefaultTextBackground);
    }

    private void MainWindow_DefaultDialBackgroundSettingsChanged(object sender, RoutedEventArgs e)
    {
      Settings.Default["DefaultDialBackground"] = (object) BackgroundSettings.DefaultDialBackground;
      Settings.Default.Save();
    }

    private void MainWindow_DefaultTextBackgroundSettingsChanged(object sender, RoutedEventArgs e)
    {
      Settings.Default["DefaultTextBackground"] = (object) BackgroundSettings.DefaultTextBackground;
      Settings.Default.Save();
    }

    private void OnAbout(object sender, ExecutedRoutedEventArgs e)
    {
      AboutDlg aboutDlg = new AboutDlg();
      aboutDlg.Owner = (Window) this;
      aboutDlg.ShowDialog();
    }

    private void OnWillNewLayout(object sender, ExecutedRoutedEventArgs e)
    {
      if (!this.SavePropmt((string) null))
        return;
      this.editorPage.Visibility = Visibility.Collapsed;
      this.startPage.Visibility = Visibility.Visible;
    }

    private void OnNewGraphicLayout(object sender, ExecutedRoutedEventArgs e)
    {
      this.startPage.Visibility = Visibility.Collapsed;
      this.editorPage.Visibility = Visibility.Visible;
      this.editorPage.Dispatcher.BeginInvoke(new Action(() => this.editorPage.NewLayout(false)), DispatcherPriority.Loaded);
    }

    private void OnNewTextLayout(object sender, ExecutedRoutedEventArgs e)
    {
      this.startPage.Visibility = Visibility.Collapsed;
      this.editorPage.Visibility = Visibility.Visible;
      this.editorPage.Dispatcher.BeginInvoke(new Action(() => this.editorPage.NewLayout(true)), DispatcherPriority.Loaded);
    }

    private void OnCanLoadLayout(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = this.startPage.lvLayoutFiles.SelectedItem != null;
    }

    private void OnLoadLayout(object sender, ExecutedRoutedEventArgs e)
    {
      if (!this.SavePropmt("Soll das aktuelle Layout schnell noch gespeichert werden?"))
        return;
      this.startPage.Visibility = Visibility.Collapsed;
      this.editorPage.Visibility = Visibility.Visible;
      //this.editorPage.Dispatcher.BeginInvoke((Delegate) (s => this.editorPage.LoadLayout(s)), DispatcherPriority.Loaded, (object) (string) this.startPage.lvLayoutFiles.SelectedItem);
        this.editorPage.Dispatcher.BeginInvoke(new Action(() =>
        {
            this.editorPage.LoadLayout((string)this.startPage.lvLayoutFiles.SelectedItem);
        }), DispatcherPriority.Loaded);

    }

        private void OnOpenLayout(object sender, ExecutedRoutedEventArgs e)
    {
      if (!this.SavePropmt((string) null))
        return;
      //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ZusiMeterLayouts");
      string path = GetZusiMeterLayoutFileDir();    
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.DefaultExt = "zmlf";
      openFileDialog1.Filter = "ZusiMeter-Layoutdateien|*.zmlf|Alle Dateien|*.*";
      openFileDialog1.InitialDirectory = path;
      openFileDialog1.Multiselect = false;
      openFileDialog1.Title = "Layout öffnen";
      OpenFileDialog openFileDialog2 = openFileDialog1;
      bool? nullable = openFileDialog2.ShowDialog();
      bool flag = true;
      if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
        return;
      this.startPage.Visibility = Visibility.Collapsed;
      this.editorPage.Visibility = Visibility.Visible;
      //this.editorPage.Dispatcher.BeginInvoke((Delegate) (s => this.editorPage.LoadLayout(s)), DispatcherPriority.Loaded, (object) openFileDialog2.FileName);
            this.editorPage.Dispatcher.BeginInvoke(new Action<string>(s =>
            {
                this.editorPage.LoadLayout(s);
            }), DispatcherPriority.Loaded, openFileDialog2.FileName);

        }

        private void OnCanBackToLayout(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = DataManager.Instance.HasLayout;
    }

    private void OnBackToLayout(object sender, ExecutedRoutedEventArgs e)
    {
      this.startPage.Visibility = Visibility.Collapsed;
      this.editorPage.Visibility = Visibility.Visible;
    }

    private bool SavePropmt(string msg)
    {
      if (DataManager.Instance.IsLayoutDirty)
      {
        if (this.editorPage.placeholder.HasGauges)
        {
          if (string.IsNullOrEmpty(msg))
            msg = MainWindow._defaultSavePrompt;
          switch (System.Windows.MessageBox.Show(msg, "Nachfrage", MessageBoxButton.YesNoCancel))
          {
            case MessageBoxResult.Cancel:
              return false;
            case MessageBoxResult.Yes:
              if (!this.editorPage.SaveLayout())
                return false;
              break;
            case MessageBoxResult.No:
              DataManager.Instance.IsLayoutDirty = false;
              break;
          }
        }
        else
        {
          string layoutFileName = DataManager.Instance.LayoutFileName;
          if (!string.IsNullOrEmpty(layoutFileName) && File.Exists(layoutFileName))
          {
            msg = MainWindow._defaultSavePrompt2;
            switch (System.Windows.MessageBox.Show(msg, "Nachfrage", MessageBoxButton.YesNoCancel))
            {
              case MessageBoxResult.Cancel:
                return false;
              case MessageBoxResult.Yes:
                try
                {
                  File.Delete(layoutFileName);
                  break;
                }
                catch
                {
                  break;
                }
              case MessageBoxResult.No:
                DataManager.Instance.IsLayoutDirty = false;
                break;
            }
          }
        }
      }
      return true;
    }
  }
}
