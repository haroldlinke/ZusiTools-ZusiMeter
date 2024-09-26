// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Classes.GaugeHelper
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZusiMeterGaugesLib.Components;
using ZusiMeterGaugesLib.CompoundGauges;
using ZusiMeterGaugesLib.Controls;
using ZusiMeterGaugesLib.Gauges;
using ZusiMeterKonfigurator.Controls;

#nullable disable
namespace ZusiMeterKonfigurator.Classes
{
  public static class GaugeHelper
  {
    private static UIElement _currentItem = (UIElement) null;
    public static readonly DependencyProperty AdornerProperty = DependencyProperty.RegisterAttached("Adorner", typeof (HoverAdorner), typeof (GaugeHelper), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty InsertPositionProperty = DependencyProperty.RegisterAttached("InsertPosition", typeof (Coordinate?), typeof (GaugeHelper), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof (bool), typeof (GaugeHelper), new PropertyMetadata((object) false));
    public static readonly DependencyProperty MoveDirectionProperty = DependencyProperty.RegisterAttached("MoveDirection", typeof (MoveDirectionType), typeof (GaugeHelper), new PropertyMetadata((object) MoveDirectionType.None));
    public static readonly DependencyProperty OffsetXProperty = DependencyProperty.RegisterAttached("OffsetX", typeof (int), typeof (GaugeHelper), new PropertyMetadata((object) 0));
    public static readonly DependencyProperty OffsetYProperty = DependencyProperty.RegisterAttached("OffsetY", typeof (int), typeof (GaugeHelper), new PropertyMetadata((object) 0));
    public static readonly RoutedEvent SelectedGaugeChangedEvent = EventManager.RegisterRoutedEvent("SelectedGaugeChanged", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (GaugeHelper));
    private static readonly DependencyPropertyKey _keyIsMouseOverItem = DependencyProperty.RegisterAttachedReadOnly("IsMouseOverItem", typeof (bool), typeof (GaugeHelper), new PropertyMetadata((object) false, new PropertyChangedCallback(GaugeHelper.IsMouseOverItemChanged), new CoerceValueCallback(GaugeHelper.CalculateIsMouseOverItem)));
    public static readonly DependencyProperty IsMouseOverItemProperty = GaugeHelper._keyIsMouseOverItem.DependencyProperty;
    private static readonly RoutedEvent UpdateOverItemEvent = EventManager.RegisterRoutedEvent("UpdateOverItem", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (GaugeHelper));
    private static readonly Type[] _types = new Type[7]
    {
      typeof (Gauge),
      typeof (Tachometer),
      typeof (ControlLamp),
      typeof (MultiColorControlLamp),
      typeof (GaugeControlBase),
      typeof (ComponentControl),
      typeof (OperatorControl)
    };

    public static HoverAdorner GetAdorner(DependencyObject d)
    {
      return (HoverAdorner) d.GetValue(GaugeHelper.AdornerProperty);
    }

    public static void SetAdorner(DependencyObject d, HoverAdorner value)
    {
      d.SetValue(GaugeHelper.AdornerProperty, (object) value);
    }

    public static Coordinate? GetInsertPosition(DependencyObject d)
    {
      return (Coordinate?) d.GetValue(GaugeHelper.InsertPositionProperty);
    }

    public static void SetInsertPosition(DependencyObject d, Coordinate? value)
    {
      d.SetValue(GaugeHelper.InsertPositionProperty, (object) value);
    }

    public static bool GetIsSelected(DependencyObject d)
    {
      return (bool) d.GetValue(GaugeHelper.IsSelectedProperty);
    }

    public static void SetIsSelected(DependencyObject d, bool value)
    {
      d.SetValue(GaugeHelper.IsSelectedProperty, (object) value);
    }

    public static MoveDirectionType GetMoveDirection(DependencyObject d)
    {
      return (MoveDirectionType) d.GetValue(GaugeHelper.MoveDirectionProperty);
    }

    public static void SetMoveDirection(DependencyObject d, MoveDirectionType value)
    {
      d.SetValue(GaugeHelper.MoveDirectionProperty, (object) value);
    }

    public static int GetOffsetX(DependencyObject d)
    {
      return (int) d.GetValue(GaugeHelper.OffsetXProperty);
    }

    public static void SetOffsetX(DependencyObject d, int value)
    {
      d.SetValue(GaugeHelper.OffsetXProperty, (object) value);
    }

    public static int GetOffsetY(DependencyObject d)
    {
      return (int) d.GetValue(GaugeHelper.OffsetYProperty);
    }

    public static void SetOffsetY(DependencyObject d, int value)
    {
      d.SetValue(GaugeHelper.OffsetYProperty, (object) value);
    }

    public static bool GetIsMouseOverItem(DependencyObject d)
    {
      return (bool) d.GetValue(GaugeHelper.IsMouseOverItemProperty);
    }

    private static object CalculateIsMouseOverItem(DependencyObject d, object value)
    {
      return (object) (d == GaugeHelper._currentItem);
    }

    static GaugeHelper()
    {
      foreach (Type type in GaugeHelper._types)
      {
        EventManager.RegisterClassHandler(type, UIElement.MouseEnterEvent, (Delegate) new MouseEventHandler(GaugeHelper.OnMouseTransition), true);
        EventManager.RegisterClassHandler(type, UIElement.MouseLeaveEvent, (Delegate) new MouseEventHandler(GaugeHelper.OnMouseTransition), true);
        EventManager.RegisterClassHandler(type, GaugeHelper.UpdateOverItemEvent, (Delegate) new RoutedEventHandler(GaugeHelper.OnUpdateOverItem), true);
        EventManager.RegisterClassHandler(type, UIElement.GotFocusEvent, (Delegate) new RoutedEventHandler(GaugeHelper.OnGotFocus), true);
      }
    }

    public static void RegisterHandler()
    {
    }

    private static void OnGotFocus(object sender, RoutedEventArgs e)
    {
      GaugeHelper.UpdateAdorner(sender as DependencyObject, false);
      RoutedEventArgs e1 = new RoutedEventArgs(GaugeHelper.SelectedGaugeChangedEvent);
      ((UIElement) sender)?.RaiseEvent(e1);
    }

    private static void OnUpdateOverItem(object sender, RoutedEventArgs e)
    {
      GaugeHelper._currentItem = sender as UIElement;
      GaugeHelper._currentItem.InvalidateProperty(GaugeHelper.IsMouseOverItemProperty);
      e.Handled = true;
    }

    private static void OnMouseTransition(object sender, MouseEventArgs e)
    {
      lock (GaugeHelper.IsMouseOverItemProperty)
      {
        if (GaugeHelper._currentItem != null)
        {
          UIElement currentItem = GaugeHelper._currentItem;
          GaugeHelper._currentItem = (UIElement) null;
          DependencyProperty overItemProperty = GaugeHelper.IsMouseOverItemProperty;
          currentItem.InvalidateProperty(overItemProperty);
        }
        Mouse.DirectlyOver?.RaiseEvent(new RoutedEventArgs(GaugeHelper.UpdateOverItemEvent));
      }
    }

    private static void IsMouseOverItemChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      GaugeHelper.UpdateAdorner(d, (bool) e.NewValue);
    }

    private static void UpdateAdorner(DependencyObject d, bool mouseOver)
    {
      HoverAdorner adorner = GaugeHelper.GetAdorner(d);
      if (adorner == null || !(d is Control))
        return;
      Control d1 = d as Control;
      if (mouseOver || GaugeHelper.GetIsSelected((DependencyObject) d1) || d1.IsFocused)
        adorner.HoverRect = new Rect(new Size(d1.ActualWidth, d1.ActualHeight));
      else
        adorner.HoverRect = Rect.Empty;
    }
  }
}
