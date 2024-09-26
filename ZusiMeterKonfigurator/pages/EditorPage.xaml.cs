// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Pages.EditorPage
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Xceed.Wpf.Toolkit.PropertyGrid;
using ZusiMeterGaugesLib.Common;
using ZusiMeterGaugesLib.Components;
using ZusiMeterGaugesLib.CompoundGauges;
using ZusiMeterGaugesLib.DigitalGauges;
using ZusiMeterGaugesLib.Enumerations;
using ZusiMeterGaugesLib.Gauges;
using ZusiMeterGaugesLib.GaugeTemplates;
using ZusiMeterGaugesLib.Interfaces;
using ZusiMeterGaugesLib.TheRailRunner;
using ZusiMeterKonfigurator.Controls;
using ZusiMeterKonfigurator.Data;
using ZusiMeterKonfigurator.Extensions;
using PropertyDefinition = Xceed.Wpf.Toolkit.PropertyGrid.PropertyDefinition;

#nullable disable
namespace ZusiMeterKonfigurator.Pages
{
  public partial class EditorPage : UserControl, IComponentConnector
  {
    private static readonly DependencyPropertyKey _keyIsDirty = DependencyProperty.RegisterReadOnly(nameof (IsDirty), typeof (bool), typeof (EditorPage), new PropertyMetadata((object) false));
    public static readonly DependencyProperty IsDirtyProperty = EditorPage._keyIsDirty.DependencyProperty;
    private static readonly DependencyPropertyKey _keyLayoutName = DependencyProperty.RegisterReadOnly(nameof (LayoutName), typeof (string), typeof (EditorPage), new PropertyMetadata((object) "Neues Layout"));
    public static readonly DependencyProperty LayoutNameProperty = EditorPage._keyLayoutName.DependencyProperty;
    public static readonly RoutedUICommand CommandCaption1 = new RoutedUICommand("Caption", nameof (CommandCaption1), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D1, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandCaption2 = new RoutedUICommand("Caption", nameof (CommandCaption2), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D2, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandCaption3 = new RoutedUICommand("Caption", nameof (CommandCaption3), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D3, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandCaption4 = new RoutedUICommand("Caption", nameof (CommandCaption4), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D4, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandCaption5 = new RoutedUICommand("Caption", nameof (CommandCaption5), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D5, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandCaption6 = new RoutedUICommand("Caption", nameof (CommandCaption6), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D6, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandCaption7 = new RoutedUICommand("Caption", nameof (CommandCaption7), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.D7, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandSaveLayout = new RoutedUICommand("Layout _speichern", nameof (CommandSaveLayout), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.S, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandSaveAsLayout = new RoutedUICommand("Layout speichern _unter...", nameof (CommandSaveAsLayout), typeof (EditorPage));
    public static readonly RoutedUICommand CommandUndo = new RoutedUICommand("_Rückgängig", nameof (CommandUndo), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.Z, ModifierKeys.Control)
    }));
    public static readonly RoutedUICommand CommandRemoveGauge = new RoutedUICommand("_Löschen", nameof (CommandRemoveGauge), typeof (EditorPage), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.Delete)
    }));
    
    public bool IsDirty
    {
      get => (bool) this.GetValue(EditorPage.IsDirtyProperty);
      private set => this.SetValue(EditorPage._keyIsDirty, (object) value);
    }

    public string LayoutName
    {
      get => (string) this.GetValue(EditorPage.LayoutNameProperty);
      private set => this.SetValue(EditorPage._keyLayoutName, (object) value);
    }

    public EditorPage()
    {
      this.InitializeComponent();
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption1, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(1)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(1))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption2, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(2)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(2))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption3, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(3)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(3))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption4, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(4)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(4))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption5, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(5)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(5))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption6, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(6)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(6))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandCaption7, (ExecutedRoutedEventHandler) ((s, e) => DataManager.Instance.Caption(7)), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.OnCanCaption(7))));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandSaveLayout, (ExecutedRoutedEventHandler) ((s, e) => this.SaveLayout()), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = DataManager.Instance.IsLayoutDirty && this.placeholder.HasGauges)));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandSaveAsLayout, (ExecutedRoutedEventHandler) ((s, e) => this.SaveAsLayout()), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.placeholder.HasGauges)));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandUndo, new ExecutedRoutedEventHandler(this.OnUndo), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = false)));
      this.CommandBindings.Add(new CommandBinding((ICommand) EditorPage.CommandRemoveGauge, new ExecutedRoutedEventHandler(this.OnRemoveGauge), new CanExecuteRoutedEventHandler(this.OnCanRemoveGauge)));
      this.tvPalette.MouseMove += new MouseEventHandler(this.TvPalette_MouseMove);
      this.placeholder.SelectedGaugeChanged += new SelectedGaugeChangedEventHandler(this.Placeholder_SelectedGaugeChanged);
    }

    public void LoadLayout(string layoutFileName)
    {
      DataManager.Instance.LayoutFileName = layoutFileName;
      DataManager.Instance.HasLayout = true;
      this.placeholder.LoadLayout(layoutFileName);
      DataManager.Instance.UpdatePalette(this.placeholder.BackgroundMode == (BackgroundMode)3);
      this.UpdateCaptions();
    }

    public void NewLayout(bool textMode)
    {
      this.placeholder.Clear(textMode);
      DataManager.Instance.UpdatePalette(textMode);
      DataManager.Instance.LayoutFileName = (string) null;
      DataManager.Instance.HasLayout = true;
      this.UpdateCaptions();
    }

    public bool SaveLayout()
    {
      string layoutFileName = DataManager.Instance.LayoutFileName;
      bool flag = !string.IsNullOrEmpty(layoutFileName) ? this.placeholder.SaveLayout(layoutFileName) : this.SaveAsLayout();
      DataManager.Instance.RefreshLayouts();
      return flag;
    }

    private void Placeholder_SelectedGaugeChanged(object sender, SelectedGaugeChangedEventArgs e)
    {
      int num1 = 0;
      ((Collection<PropertyDefinition>) this.pgGauge.PropertyDefinitions).Clear();
      this.pgGauge.SelectedObject = (object) e.Gauge;
      if (e.SelectedControl != null)
      {
        IGaugeControl selectedControl = e.SelectedControl;
        int num2;
        switch (selectedControl)
        {
          case ControlLamp controlLamp:
            ControlLampColor color = controlLamp.Color;
            PropertyDefinitionCollection propertyDefinitions1 = this.pgGauge.PropertyDefinitions;
            PropertyDefinition propertyDefinition1 = new PropertyDefinition();
            propertyDefinition1.Category = "Melder";
            propertyDefinition1.DisplayName = "Farbe";
            PropertyDefinition propertyDefinition2 = propertyDefinition1;
            int num3 = num1;
            num2 = num3 + 1;
            int? nullable1 = new int?(num3);
            propertyDefinition2.DisplayOrder = nullable1;
            ((PropertyDefinitionBase) propertyDefinition1).TargetProperties = (IList) new string[1]
            {
              "Color"
            };
            PropertyDefinition propertyDefinition3 = propertyDefinition1;
            ((Collection<PropertyDefinition>) propertyDefinitions1).Add(propertyDefinition3);
            controlLamp.Color = color;
            break;
          case RailRunner _:
          case ComponentControl _:
          //case MultiColorControlLamp _:
          case DigitalChronometer _:
          case ZugInfoGauge _:
            ((UIElement) this.pgGauge).Visibility = Visibility.Collapsed;
            return;
          case DigitalGaugeControlBase _:
          case DigitalGauge _:
            PropertyDefinitionCollection propertyDefinitions2 = this.pgGauge.PropertyDefinitions;
            PropertyDefinition propertyDefinition4 = new PropertyDefinition();
            ((PropertyDefinitionBase) propertyDefinition4).TargetProperties = (IList) new string[1]
            {
              "NumDecimals"
            };
            PropertyDefinition propertyDefinition5 = propertyDefinition4;
            ((Collection<PropertyDefinition>) propertyDefinitions2).Add(propertyDefinition5);
            break;
          default:
            IGaugeTemplate gaugeTemplate = GaugeTemplateBase.GetGaugeTemplate(selectedControl as DependencyObject);
            PropertyDefinitionCollection propertyDefinitions3 = this.pgGauge.PropertyDefinitions;
            PropertyDefinition propertyDefinition6 = new PropertyDefinition();
            propertyDefinition6.Category = "Farben";
            propertyDefinition6.DisplayName = "Hintergrundfarbe";
            PropertyDefinition propertyDefinition7 = propertyDefinition6;
            int num4 = num1;
            int num5 = num4 + 1;
            int? nullable2 = new int?(num4);
            propertyDefinition7.DisplayOrder = nullable2;
            ((PropertyDefinitionBase) propertyDefinition6).TargetProperties = (IList) new string[1]
            {
              "Background"
            };
            PropertyDefinition propertyDefinition8 = propertyDefinition6;
            ((Collection<PropertyDefinition>) propertyDefinitions3).Add(propertyDefinition8);
            PropertyDefinitionCollection propertyDefinitions4 = this.pgGauge.PropertyDefinitions;
            PropertyDefinition propertyDefinition9 = new PropertyDefinition();
            propertyDefinition9.Category = "Farben";
            propertyDefinition9.DisplayName = "Schriftfarbe";
            PropertyDefinition propertyDefinition10 = propertyDefinition9;
            int num6 = num5;
            int num7 = num6 + 1;
            int? nullable3 = new int?(num6);
            propertyDefinition10.DisplayOrder = nullable3;
            ((PropertyDefinitionBase) propertyDefinition9).TargetProperties = (IList) new string[1]
            {
              "Foreground"
            };
            PropertyDefinition propertyDefinition11 = propertyDefinition9;
            ((Collection<PropertyDefinition>) propertyDefinitions4).Add(propertyDefinition11);
            if (gaugeTemplate is AnalogTemplateBase && !(selectedControl is BrakeManometer))
            {
              PropertyDefinitionCollection propertyDefinitions5 = this.pgGauge.PropertyDefinitions;
              PropertyDefinition propertyDefinition12 = new PropertyDefinition();
              propertyDefinition12.DisplayOrder = new int?(num7++);
              ((PropertyDefinitionBase) propertyDefinition12).TargetProperties = (IList) new string[1]
              {
                "NeedleColor"
              };
              PropertyDefinition propertyDefinition13 = propertyDefinition12;
              ((Collection<PropertyDefinition>) propertyDefinitions5).Add(propertyDefinition13);
            }
            if (gaugeTemplate is RoundGaugeTemplate && !(selectedControl is BrakeManometer))
            {
              PropertyDefinitionCollection propertyDefinitions6 = this.pgGauge.PropertyDefinitions;
              PropertyDefinition propertyDefinition14 = new PropertyDefinition();
              PropertyDefinition propertyDefinition15 = propertyDefinition14;
              int num8 = num7;
              int num9 = num8 + 1;
              int? nullable4 = new int?(num8);
              propertyDefinition15.DisplayOrder = nullable4;
              ((PropertyDefinitionBase) propertyDefinition14).TargetProperties = (IList) new string[1]
              {
                "ClassicStyle"
              };
              PropertyDefinition propertyDefinition16 = propertyDefinition14;
              ((Collection<PropertyDefinition>) propertyDefinitions6).Add(propertyDefinition16);
              PropertyDefinitionCollection propertyDefinitions7 = this.pgGauge.PropertyDefinitions;
              PropertyDefinition propertyDefinition17 = new PropertyDefinition();
              PropertyDefinition propertyDefinition18 = propertyDefinition17;
              int num10 = num9;
              num7 = num10 + 1;
              int? nullable5 = new int?(num10);
              propertyDefinition18.DisplayOrder = nullable5;
              ((PropertyDefinitionBase) propertyDefinition17).TargetProperties = (IList) new string[1]
              {
                "NeedleFace"
              };
              PropertyDefinition propertyDefinition19 = propertyDefinition17;
              ((Collection<PropertyDefinition>) propertyDefinitions7).Add(propertyDefinition19);
            }
            if (e.Gauge != null && !(e.Gauge is LinearGaugeBase) && !(e.Gauge is TachometerGauge) && !(selectedControl is BrakeManometer))
            {
              PropertyDefinitionCollection propertyDefinitions8 = this.pgGauge.PropertyDefinitions;
              PropertyDefinition propertyDefinition20 = new PropertyDefinition();
              propertyDefinition20.DisplayOrder = new int?(num7++);
              ((PropertyDefinitionBase) propertyDefinition20).TargetProperties = (IList) new string[1]
              {
                "HasDigitalDisplay"
              };
              PropertyDefinition propertyDefinition21 = propertyDefinition20;
              ((Collection<PropertyDefinition>) propertyDefinitions8).Add(propertyDefinition21);
              if (gaugeTemplate is RoundGaugeTemplate)
              {
                PropertyDefinitionCollection propertyDefinitions9 = this.pgGauge.PropertyDefinitions;
                PropertyDefinition propertyDefinition22 = new PropertyDefinition();
                propertyDefinition22.DisplayOrder = new int?(num7++);
                ((PropertyDefinitionBase) propertyDefinition22).TargetProperties = (IList) new string[1]
                {
                  "NumDecimals"
                };
                PropertyDefinition propertyDefinition23 = propertyDefinition22;
                ((Collection<PropertyDefinition>) propertyDefinitions9).Add(propertyDefinition23);
              }
            }
            PropertyDefinitionCollection propertyDefinitions10 = this.pgGauge.PropertyDefinitions;
            PropertyDefinition propertyDefinition24 = new PropertyDefinition();
            PropertyDefinition propertyDefinition25 = propertyDefinition24;
            int num11 = num7;
            num2 = num11 + 1;
            int? nullable6 = new int?(num11);
            propertyDefinition25.DisplayOrder = nullable6;
            ((PropertyDefinitionBase) propertyDefinition24).TargetProperties = (IList) new string[1]
            {
              "HasOverflowLED"
            };
            PropertyDefinition propertyDefinition26 = propertyDefinition24;
            ((Collection<PropertyDefinition>) propertyDefinitions10).Add(propertyDefinition26);
            break;
        }
        ((UIElement) this.pgGauge).Visibility = Visibility.Visible;
      }
      else
        ((UIElement) this.pgGauge).Visibility = Visibility.Collapsed;
    }

    private void TvPalette_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton != MouseButtonState.Pressed)
        return;
      TreeView treeView = (TreeView) sender;
      TreeViewItem dataFromTreeView = TreeViewHelper.GetDataFromTreeView(treeView, Mouse.GetPosition((IInputElement) treeView));
      if (dataFromTreeView == null || !(dataFromTreeView.Header is ToolBoxViewModel header) || header.Object == null)
        return;
      DataObject data = new DataObject(typeof (IGaugeTemplate), (object) header.Object);
      int num = (int) DragDrop.DoDragDrop((DependencyObject) treeView, (object) data, DragDropEffects.Copy);
    }

    private bool SaveAsLayout()
    {
      //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ZusiMeterLayouts");
      string path = MainWindow.GetZusiMeterLayoutFileDir(); 
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
      saveFileDialog1.DefaultExt = "zmlf";
      saveFileDialog1.Filter = "ZusiMeter-Layoutdateien|*.zmlf|Alle Dateien|*.*";
      saveFileDialog1.InitialDirectory = path;
      saveFileDialog1.OverwritePrompt = true;
      saveFileDialog1.Title = "Layout speichern unter";
      SaveFileDialog saveFileDialog2 = saveFileDialog1;
      bool? nullable = saveFileDialog2.ShowDialog();
      bool flag = true;
      if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
        return false;
      DataManager.Instance.LayoutFileName = saveFileDialog2.FileName;
      return this.placeholder.SaveLayout(saveFileDialog2.FileName);
    }

    private void OnUndo(object sender, ExecutedRoutedEventArgs e) => this.placeholder.Undo();

    private bool OnCanCaption(int index) => DataManager.Instance.CanCaption(index);

    private void OnCanRemoveGauge(object sender, CanExecuteRoutedEventArgs e)
    {
      IInputElement focusedElement = FocusManager.GetFocusedElement((DependencyObject) Application.Current.MainWindow);
      if (focusedElement != null && focusedElement is IGaugeControl)
        e.CanExecute = true;
      e.Handled = true;
    }

    private void OnRemoveGauge(object sender, ExecutedRoutedEventArgs e)
    {
      IInputElement focusedElement = FocusManager.GetFocusedElement((DependencyObject) Application.Current.MainWindow);
      if (focusedElement != null && focusedElement is IGaugeControl)
        this.placeholder.RemoveGauge(focusedElement as UIElement, true);
      e.Handled = true;
    }

    private void UpdateCaptions()
    {
      List<string> captions = DataManager.Instance.PaletteRoot.GetCaptions();
      int num = 1;
      foreach (string str in captions)
      {
        switch (num)
        {
          case 1:
            this.btnCaption1.Content = (object) str;
            break;
          case 2:
            this.btnCaption2.Content = (object) str;
            break;
          case 3:
            this.btnCaption3.Content = (object) str;
            break;
          case 4:
            this.btnCaption4.Content = (object) str;
            break;
          case 5:
            this.btnCaption5.Content = (object) str;
            break;
          case 6:
            this.btnCaption6.Content = (object) str;
            break;
          case 7:
            this.btnCaption7.Content = (object) str;
            break;
        }
        ++num;
      }
      for (; num < 8; ++num)
      {
        switch (num - 1)
        {
          case 0:
            this.btnCaption1.Content = (object) null;
            break;
          case 1:
            this.btnCaption2.Content = (object) null;
            break;
          case 2:
            this.btnCaption3.Content = (object) null;
            break;
          case 3:
            this.btnCaption4.Content = (object) null;
            break;
          case 4:
            this.btnCaption5.Content = (object) null;
            break;
          case 5:
            this.btnCaption6.Content = (object) null;
            break;
          case 6:
            this.btnCaption7.Content = (object) null;
            break;
        }
      }
    }

    
  }
}
