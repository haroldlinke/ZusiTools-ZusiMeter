// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Extensions.TreeViewHelper
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#nullable disable
namespace ZusiMeter.Extensions
{
  public static class TreeViewHelper
  {
    private static TreeViewItem _currentItem = (TreeViewItem) null;
    private static readonly DependencyPropertyKey _keyIsMouseOverItem = DependencyProperty.RegisterAttachedReadOnly("IsMouseOverItem", typeof (bool), typeof (TreeViewHelper), (PropertyMetadata) new FrameworkPropertyMetadata((PropertyChangedCallback) null, new CoerceValueCallback(TreeViewHelper.CalculateIsMouseOverItem)));
    public static readonly DependencyProperty IsMouseOverItemProperty = TreeViewHelper._keyIsMouseOverItem.DependencyProperty;
    private static readonly RoutedEvent UpdateOverItemEvent = EventManager.RegisterRoutedEvent("UpdateOverItem", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (TreeViewHelper));

    public static bool GetIsMouseOverItem(DependencyObject d)
    {
      return (bool) d.GetValue(TreeViewHelper.IsMouseOverItemProperty);
    }

    private static object CalculateIsMouseOverItem(DependencyObject d, object value)
    {
      return (object) (d == TreeViewHelper._currentItem);
    }

    static TreeViewHelper()
    {
      EventManager.RegisterClassHandler(typeof (TreeViewItem), UIElement.MouseEnterEvent, (Delegate) new MouseEventHandler(TreeViewHelper.OnMouseTransition), true);
      EventManager.RegisterClassHandler(typeof (TreeViewItem), UIElement.MouseLeaveEvent, (Delegate) new MouseEventHandler(TreeViewHelper.OnMouseTransition), true);
      EventManager.RegisterClassHandler(typeof (TreeViewItem), TreeViewHelper.UpdateOverItemEvent, (Delegate) new RoutedEventHandler(TreeViewHelper.OnUpdateOverItem), true);
    }

    public static void RegisterHandler()
    {
    }

    public static TreeViewItem GetDataFromTreeView(TreeView source, Point point)
    {
      UIElement reference = source.InputHitTest(point) as UIElement;
      while (reference != null && reference.GetType() != typeof (TreeViewItem))
      {
        reference = VisualTreeHelper.GetParent((DependencyObject) reference) as UIElement;
        if (reference == source)
        {
          reference = (UIElement) null;
          break;
        }
      }
      return (TreeViewItem) reference;
    }

    private static void OnUpdateOverItem(object sender, RoutedEventArgs e)
    {
      TreeViewHelper._currentItem = sender as TreeViewItem;
      TreeViewHelper._currentItem.InvalidateProperty(TreeViewHelper.IsMouseOverItemProperty);
      e.Handled = true;
    }

    private static void OnMouseTransition(object sender, MouseEventArgs e)
    {
      lock (TreeViewHelper.IsMouseOverItemProperty)
      {
        if (TreeViewHelper._currentItem != null)
        {
          TreeViewItem currentItem = TreeViewHelper._currentItem;
          TreeViewHelper._currentItem = (TreeViewItem) null;
          DependencyProperty overItemProperty = TreeViewHelper.IsMouseOverItemProperty;
          currentItem.InvalidateProperty(overItemProperty);
        }
        Mouse.DirectlyOver?.RaiseEvent(new RoutedEventArgs(TreeViewHelper.UpdateOverItemEvent));
      }
    }
  }
}
