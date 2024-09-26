// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Classes.HoverAdorner
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

#nullable disable
namespace ZusiMeterKonfigurator.Classes
{
  public class HoverAdorner : Adorner, IDisposable
  {
    private readonly AdornerLayer _layer;
    private Pen _pen;
    public static readonly DependencyProperty FillProperty = DependencyProperty.Register(nameof (Fill), typeof (Brush), typeof (HoverAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty HoverRectProperty = DependencyProperty.Register(nameof (HoverRect), typeof (Rect), typeof (HoverAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) Rect.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register(nameof (Shape), typeof (ShapeType), typeof (HoverAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) ShapeType.Rect));
    public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(nameof (Stroke), typeof (Brush), typeof (HoverAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) Brushes.OrangeRed, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(HoverAdorner.OnStrokeChanged)));
    public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(nameof (StrokeThickness), typeof (double), typeof (HoverAdorner), (PropertyMetadata) new FrameworkPropertyMetadata((object) 2.0, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(HoverAdorner.OnStrokeThicknessChanged)));

    public Brush Fill
    {
      get => (Brush) this.GetValue(HoverAdorner.FillProperty);
      set => this.SetValue(HoverAdorner.FillProperty, (object) value);
    }

    public Rect HoverRect
    {
      get => (Rect) this.GetValue(HoverAdorner.HoverRectProperty);
      set => this.SetValue(HoverAdorner.HoverRectProperty, (object) value);
    }

    public ShapeType Shape
    {
      get => (ShapeType) this.GetValue(HoverAdorner.ShapeProperty);
      set => this.SetValue(HoverAdorner.ShapeProperty, (object) value);
    }

    public Brush Stroke
    {
      get => (Brush) this.GetValue(HoverAdorner.StrokeProperty);
      set => this.SetValue(HoverAdorner.StrokeProperty, (object) value);
    }

    public double StrokeThickness
    {
      get => (double) this.GetValue(HoverAdorner.StrokeThicknessProperty);
      set => this.SetValue(HoverAdorner.StrokeThicknessProperty, (object) value);
    }

    public HoverAdorner(UIElement element)
      : base(element)
    {
      this._layer = AdornerLayer.GetAdornerLayer((Visual) this.AdornedElement);
      this._layer.Add((Adorner) this);
      this.UpdatePen(this.Stroke, this.StrokeThickness);
      this.IsHitTestVisible = false;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected override void OnRender(DrawingContext dc)
    {
      if (this.HoverRect.IsEmpty)
        return;
      Rect hoverRect = this.HoverRect;
      hoverRect.Inflate(new Size(1.0, 1.0));
      if (this.Shape == ShapeType.Rect)
      {
        dc.DrawRectangle((Brush) null, this._pen, hoverRect);
      }
      else
      {
        StreamGeometry streamGeometry = new StreamGeometry();
        PointCollection points = new PointCollection();
        using (StreamGeometryContext streamGeometryContext = streamGeometry.Open())
        {
          streamGeometryContext.BeginFigure(hoverRect.TopLeft, false, true);
          if (this.Shape == ShapeType.ArrowRight)
          {
            points.Add(new Point(hoverRect.Left + hoverRect.Width, hoverRect.Top));
            points.Add(new Point(hoverRect.Left + hoverRect.Width * 2.0, hoverRect.Top + hoverRect.Height * 0.5));
            points.Add(new Point(hoverRect.Left + hoverRect.Width, hoverRect.Top + hoverRect.Height));
            points.Add(new Point(hoverRect.Left, hoverRect.Top + hoverRect.Height));
          }
          else
          {
            if (this.Shape != ShapeType.ArrowDown)
              return;
            points.Add(new Point(hoverRect.Left + hoverRect.Width, hoverRect.Top));
            points.Add(new Point(hoverRect.Left + hoverRect.Width, hoverRect.Top + hoverRect.Height));
            points.Add(new Point(hoverRect.Left + hoverRect.Width * 0.5, hoverRect.Top + hoverRect.Height * 2.0));
            points.Add(new Point(hoverRect.Left, hoverRect.Top + hoverRect.Height));
          }
          streamGeometryContext.PolyLineTo((IList<Point>) points, true, true);
          dc.DrawGeometry((Brush) null, this._pen, (Geometry) streamGeometry);
        }
      }
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this._layer?.Remove((Adorner) this);
    }

    private static void OnStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as HoverAdorner).OnStrokeChanged((Brush) e.NewValue);
    }

    private void OnStrokeChanged(Brush value) => this.UpdatePen(value, this.StrokeThickness);

    private static void OnStrokeThicknessChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      (d as HoverAdorner).OnStrokeThicknessChanged((double) e.NewValue);
    }

    private void OnStrokeThicknessChanged(double value) => this.UpdatePen(this.Stroke, value);

    private void UpdatePen(Brush brush, double thickness) => this._pen = new Pen(brush, thickness);
  }
}
