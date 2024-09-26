// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Controls.LayoutGrid
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

using log4net;
using Sovoma;
using Sovoma.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using ZusiMeterGaugesLib.Common;
using ZusiMeterGaugesLib.Controls;
using ZusiMeterGaugesLib.Gauges;
using ZusiMeterGaugesLib.GaugeTemplates;
using ZusiMeterGaugesLib.Interfaces;
using ZusiMeterKonfigurator.Classes;
using ZusiMeterKonfigurator.Data;
using ZusiMeterKonfigurator.Extensions;

#nullable disable
namespace ZusiMeterKonfigurator.Controls
{
  public class LayoutGrid : Grid, IDisposable
  {
    private const int MaxCountColumns = 60;
    private const int MaxCountRows = 100;
    private static readonly ILog Log = LogManager.GetLogger(typeof (LayoutGrid));
    private readonly LayoutBackground _background = new LayoutBackground();
    private HoverAdorner _adorner;
    private readonly LayoutGrid.FieldStates _fieldStates = new LayoutGrid.FieldStates();
    private int _colWidth = 50;
    private int _rowHeight = 50;
    private int _loadCounter = -1;
    private Point _mousePos;
    private static readonly DependencyPropertyKey _selectedGaugeKey = DependencyProperty.RegisterReadOnly(nameof (SelectedGauge), typeof (IGaugeControl), typeof (LayoutGrid), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty SelectedGaugeProperty = LayoutGrid._selectedGaugeKey.DependencyProperty;
    public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(nameof (Zoom), typeof (double), typeof (LayoutGrid), new PropertyMetadata((object) 1.0, new PropertyChangedCallback(LayoutGrid.OnZoomChanged)));

    public LayoutBackground LayoutBackground => this._background;

    [Browsable(false)]
    public BackgroundMode BackgroundMode => this._background.BackgroundMode;

    public bool HasGauges
    {
      get
      {
        foreach (UIElement child in this.Children)
        {
          if (child is IGaugeControl)
            return true;
        }
        return false;
      }
    }

    public IGaugeControl SelectedGauge
    {
      get => (IGaugeControl) this.GetValue(LayoutGrid.SelectedGaugeProperty);
      private set => this.SetValue(LayoutGrid._selectedGaugeKey, (object) value);
    }

    public double Zoom
    {
      get => (double) this.GetValue(LayoutGrid.ZoomProperty);
      set => this.SetValue(LayoutGrid.ZoomProperty, (object) value);
    }

    public event SelectedGaugeChangedEventHandler SelectedGaugeChanged;

    static LayoutGrid()
    {
      FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutGrid), (PropertyMetadata) new FrameworkPropertyMetadata((object) typeof (LayoutGrid)));
    }

    public LayoutGrid()
    {
      this.DragOver += new DragEventHandler(this.GaugePlaceholder_DragOver);
      this.DragLeave += new DragEventHandler(this.GaugePlaceholder_DragLeave);
      this.Drop += new DragEventHandler(this.GaugePlaceholder_Drop);
      this._background.BackgroundChanged += (EventHandler) ((s, e) => DataManager.Instance.IsLayoutDirty = true);
      this._background.BackgroundModeChanged += new EventHandler(this.OnBackgroundModeChanged);
      this.AddHandler(GaugeHelper.SelectedGaugeChangedEvent, (Delegate) new RoutedEventHandler(this.OnSelectedGaugeChangedEvent));
      this.LayoutUpdated += (EventHandler) ((s, e) =>
      {
        if (this._loadCounter != 0)
          return;
        foreach (object child in this.Children)
        {
          if (child is IGaugeControl)
          {
            ((UIElement) child).Focus();
            break;
          }
        }
        DataManager.Instance.IsLayoutDirty = false;
        this._loadCounter = -1;
      });
      this.Loaded += (RoutedEventHandler) ((s, e) =>
      {
        GaugeHelper.RegisterHandler();
        TreeViewHelper.RegisterHandler();
      });
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this._adorner?.Dispose();
    }

    public void Clear(bool textMode)
    {
      foreach (UIElement child in this.Children)
      {
        if (child is IGaugeControl)
          GaugeHelper.GetAdorner((DependencyObject) child)?.Dispose();
      }
      this.Children.Clear();
      this._fieldStates.Reset();
      this.ColumnDefinitions.Clear();
      this.RowDefinitions.Clear();
      this.RecalculateSize();
      this._background.Initialize(textMode ? BackgroundSettings.DefaultTextBackground : BackgroundSettings.DefaultDialBackground);
      DataManager.Instance.IsLayoutDirty = false;
    }

    public void LoadLayout(string layoutFile)
    {
      try
      {
        this.Clear(false);
        this._loadCounter = 0;
        ZMLFile zmlFile = new ZMLFile(layoutFile);
        zmlFile.ParseDocument += new ZMLReadDocumentEventHandler(this.ZmlFile_ParseDocument);
        zmlFile.Parse();
      }
      catch (Exception ex)
      {
        LayoutGrid.Log.Error((object) ex.ToString());
      }
    }

    public void RemoveGauge(UIElement gauge, bool undoable)
    {
      (gauge as IGaugeControl).Dirty -= new EventHandler(this.Gauge_Dirty);
      if (gauge is IGaugeControl igaugeControl && this.SelectedGauge == igaugeControl.Device)
      {
        this.SelectedGauge = (IGaugeControl) null;
        this.OnSelectedGaugeChanged((IGaugeControl) null);
      }
      this.Children.Remove(gauge);
      GaugeHelper.GetAdorner((DependencyObject) gauge)?.Dispose();
      DataManager.Instance.IsLayoutDirty = true;
      IGaugeTemplate gaugeTemplate = GaugeTemplateBase.GetGaugeTemplate((DependencyObject) gauge);
      Coordinate? insertPosition = GaugeHelper.GetInsertPosition(gaugeTemplate as DependencyObject);
      if (insertPosition.HasValue)
        this._fieldStates.Release(insertPosition.Value.R, insertPosition.Value.C, gaugeTemplate.SizeY, gaugeTemplate.SizeX);
      this.RecalculateSize();
      if (undoable)
        UndoManager<Control>.Push(UndoActionType.Delete, gauge as Control, (object) null);
      this.InvalidateVisual();
    }

    public bool SaveLayout(string layoutFile)
    {
      ZMLFile zmlFile = new ZMLFile(layoutFile);
      zmlFile.SaveDocument += new ZMLSaveDocumentEventHandler(this.ZmlFile_SaveDocument);
      zmlFile.Save();
      DataManager.Instance.IsLayoutDirty = false;
      return true;
    }

    public void Undo()
    {
      UndoItem<Control> undoItem = UndoManager<Control>.Pop();
      switch (undoItem.Action)
      {
        case UndoActionType.Insert:
          this.RemoveGauge((UIElement) undoItem.Object, false);
          break;
        case UndoActionType.Delete:
          this.InsertGauge(GaugeTemplateBase.GetGaugeTemplate((DependencyObject) undoItem.Object), undoItem.Object, false, true);
          break;
        case UndoActionType.Move:
          this.ReverseMove(undoItem.Object, (GaugeMoveState) undoItem.State);
          break;
      }
    }

    protected override void OnRender(DrawingContext dc)
    {
      base.OnRender(dc);
      SolidColorBrush solidColorBrush = new SolidColorBrush(Color.FromArgb((byte) 96, (byte) 200, (byte) 200, (byte) 200));
      for (int r = 0; r < this.RowDefinitions.Count; ++r)
      {
        for (int c = 0; c < this.ColumnDefinitions.Count; ++c)
        {
          Rect rectangle = new Rect((double) (c * this._colWidth), (double) (r * this._rowHeight), (double) this._colWidth, (double) this._rowHeight);
          if (this._fieldStates[r, c] == LayoutGrid.FieldState.Placeholder)
            dc.DrawRectangle((Brush) solidColorBrush, (Pen) null, rectangle);
        }
      }
    }

    private void OnSelectedGaugeChanged(IGaugeControl value)
    {
      SelectedGaugeChangedEventHandler selectedGaugeChanged = this.SelectedGaugeChanged;
      if (selectedGaugeChanged == null)
        return;
      selectedGaugeChanged((object) this, new SelectedGaugeChangedEventArgs(value));
    }

    private void OnSelectedGaugeChangedEvent(object sender, RoutedEventArgs e)
    {
      IGaugeControl selectedGauge = this.SelectedGauge;
      if (e.Source is IGaugeControl igaugeControl)
      {
        IGaugeControl visualAncestor = VisualTreeExtensions.GetVisualAncestor<IGaugeControl>((DependencyObject) igaugeControl);
        if (visualAncestor != null)
          igaugeControl = visualAncestor;
        this.SelectedGauge = igaugeControl;
      }
      this.UpdateSelection(selectedGauge, this.SelectedGauge);
      this.OnSelectedGaugeChanged(this.SelectedGauge);
    }

    private void UpdateSelection(IGaugeControl oldSelection, IGaugeControl newSelection)
    {
      if (oldSelection is Control d1)
      {
        HoverAdorner adorner = GaugeHelper.GetAdorner((DependencyObject) d1);
        if (adorner != null)
          adorner.HoverRect = Rect.Empty;
        GaugeHelper.SetIsSelected((DependencyObject) d1, false);
      }
      if (!(newSelection is Control d2))
        return;
      GaugeHelper.SetIsSelected((DependencyObject) d2, true);
    }

    private void GaugePlaceholder_Drop(object sender, DragEventArgs e)
    {
      if (this._adorner != null)
      {
        this._adorner.Dispose();
        this._adorner = (HoverAdorner) null;
      }
      IGaugeTemplate gt = e.Data.GetData(typeof (IGaugeTemplate)) as IGaugeTemplate;

      IGaugeControl ctrl = (IGaugeControl) null;
      if (gt == null && e.Data.GetData(typeof (IGaugeControl)) is IGaugeControl tempctrl)
      {
          gt = GaugeTemplateBase.GetGaugeTemplate((DependencyObject)tempctrl);
          ctrl = tempctrl;
      }
      else
      {
          ctrl = null;
      }
        
      if (gt != null)
      {
        this._loadCounter = -2;
        this.InsertGauge(gt, (Control) ctrl, true, true);
      }
      e.Handled = true;
    }

    private void GaugePlaceholder_DragLeave(object sender, DragEventArgs e)
    {
      if (this._adorner == null)
        return;
      this._adorner.Dispose();
      this._adorner = (HoverAdorner) null;
    }

    private void GaugePlaceholder_DragOver(object sender, DragEventArgs e)
    {
      e.Effects = DragDropEffects.None;
      e.Handled = true;
      if (this._adorner == null)
        this._adorner = new HoverAdorner((UIElement) this);

      IGaugeTemplate d = null; //**HLI

            if (!(e.Data.GetData(typeof(IGaugeTemplate)) is IGaugeTemplate tempd))
            {

                if (e.Data.GetData(typeof(IGaugeControl)) is IGaugeControl data)
                {
                        d = GaugeTemplateBase.GetGaugeTemplate((DependencyObject)data);
                    }
                }

            else
            {
                d = tempd;
            }
      
      if (d == null)
      {
        this._adorner.HoverRect = Rect.Empty;
      }
      else
      {
        Point position = e.GetPosition((IInputElement) this);
        int col1 = (int) (position.X / (double) this._colWidth);
        int row1 = (int) (position.Y / (double) this._rowHeight);
        if (col1 < 0 || col1 + d.SizeX >= 60 || row1 < 0 || row1 + d.SizeY >= 100)
        {
          this._adorner.HoverRect = Rect.Empty;
        }
        else
        {
          int row2 = row1;
          int col2 = col1;
          if (this._fieldStates.CanInsert(ref row2, ref col2, d.SizeY, d.SizeX))
          {
            GaugeHelper.SetMoveDirection((DependencyObject) d, MoveDirectionType.None);
            GaugeHelper.SetInsertPosition((DependencyObject) d, new Coordinate?(new Coordinate(row2, col2)));
            this._adorner.Shape = ShapeType.Rect;
            this._adorner.HoverRect = new Rect((double) (col2 * this._colWidth), (double) (row2 * this._rowHeight), (double) (d.SizeX * this._colWidth), (double) (d.SizeY * this._rowHeight));
            e.Effects = DragDropEffects.Copy;
          }
          else
          {
            double height = (double) this._rowHeight * 0.5;
            Rect rect = new Rect((double) (col1 * this._colWidth), (double) (row1 * this._rowHeight), (double) (d.SizeX * this._colWidth), height);
            if (rect.Contains(position))
            {
              Dictionary<int, IGaugeControl> gaugesIn = this.FindGaugesIn(col1, row1, d.SizeX, d.SizeY);
              bool flag = gaugesIn.Count > 0;
              foreach (IGaugeControl element in gaugesIn.Values)
              {
                if (Grid.GetRow(element as UIElement) < row1)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {
                gaugesIn.Clear();
                if (!this.FetchAllGaugesToMove(gaugesIn, col1, row1, d.SizeX, d.SizeY, 0, d.SizeY))
                  return;
                GaugeHelper.SetMoveDirection((DependencyObject) d, MoveDirectionType.Down);
                GaugeHelper.SetInsertPosition((DependencyObject) d, new Coordinate?(new Coordinate(row1, col1)));
                this._adorner.Shape = ShapeType.ArrowDown;
                this._adorner.HoverRect = rect;
                e.Effects = DragDropEffects.Copy;
                return;
              }
            }
            double width = (double) this._colWidth * 0.5;
            rect = new Rect((double) (col1 * this._colWidth), (double) (row1 * this._rowHeight), width, (double) (d.SizeY * this._rowHeight));
            if (rect.Contains(position))
            {
              Dictionary<int, IGaugeControl> gaugesIn = this.FindGaugesIn(col1, row1, d.SizeX, d.SizeY);
              bool flag = gaugesIn.Count > 0;
              foreach (IGaugeControl element in gaugesIn.Values)
              {
                if (Grid.GetColumn(element as UIElement) < col1)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
              {
                gaugesIn.Clear();
                if (!this.FetchAllGaugesToMove(gaugesIn, col1, row1, d.SizeX, d.SizeY, d.SizeX, 0))
                  return;
                GaugeHelper.SetMoveDirection((DependencyObject) d, MoveDirectionType.Right);
                GaugeHelper.SetInsertPosition((DependencyObject) d, new Coordinate?(new Coordinate(row1, col1)));
                this._adorner.Shape = ShapeType.ArrowRight;
                this._adorner.HoverRect = rect;
                e.Effects = DragDropEffects.Copy;
                return;
              }
            }
            this._adorner.HoverRect = Rect.Empty;
          }
        }
      }
    }

    private Coordinate? FindGaugeAt(int row, int col, out IGaugeControl gc)
    {
      gc = (IGaugeControl) null;
      foreach (UIElement child in this.Children)
      {
        if (child is IGaugeControl igaugeControl)
        {
          IGaugeTemplate gaugeTemplate = GaugeTemplateBase.GetGaugeTemplate((DependencyObject) child);
          int row1 = Grid.GetRow(child);
          int column = Grid.GetColumn(child);
          if (row >= row1 && row < row1 + gaugeTemplate.SizeY && col >= column && col < column + gaugeTemplate.SizeX)
          {
            gc = igaugeControl;
            return new Coordinate?(new Coordinate(row1, column));
          }
        }
      }
      return new Coordinate?();
    }

    private Dictionary<int, IGaugeControl> FindGaugesIn(int col, int row, int nCols, int nRows)
    {
      Dictionary<int, IGaugeControl> gaugesIn = new Dictionary<int, IGaugeControl>();
      int num1 = 0;
      int col1 = col;
      while (num1 < nCols)
      {
        int num2 = 0;
        int row1 = row;
        while (num2 < nRows)
        {
          IGaugeControl gc;
          Coordinate? gaugeAt = this.FindGaugeAt(row1, col1, out gc);
          if (gaugeAt.HasValue)
          {
            int key = gaugeAt.Value.R * 100 + gaugeAt.Value.C;
            gaugesIn[key] = gc;
          }
          ++num2;
          ++row1;
        }
        ++num1;
        ++col1;
      }
      return gaugesIn;
    }

    private bool FetchAllGaugesToMove(
      Dictionary<int, IGaugeControl> gauges,
      int col,
      int row,
      int nCols,
      int nRows,
      int offsetX,
      int offsetY)
    {
      if (offsetX == 0 && offsetY == 0)
        return false;
      int num1 = col + offsetX;
      int num2 = row + offsetY;
      foreach (IGaugeControl igaugeControl in this.FindGaugesIn(col, row, nCols, nRows).Values)
      {
        int column = Grid.GetColumn((UIElement) igaugeControl);
        int row1 = Grid.GetRow((UIElement) igaugeControl);
        int key = row1 * 100 + column;
        if (!gauges.ContainsKey(key))
        {
          IGaugeTemplate gaugeTemplate = GaugeTemplateBase.GetGaugeTemplate((DependencyObject) igaugeControl);
          int offsetY1 = num2 - row1;
          int row2 = row1 + offsetY1;
          if (row2 + gaugeTemplate.SizeY >= 100)
            return false;
          int offsetX1 = num1 - column;
          int col1 = column + offsetX1;
          if (col1 + gaugeTemplate.SizeX >= 60)
            return false;
          gauges[key] = igaugeControl;
          GaugeHelper.SetOffsetX((DependencyObject) igaugeControl, offsetX1);
          GaugeHelper.SetOffsetY((DependencyObject) igaugeControl, offsetY1);
          if (!this.FetchAllGaugesToMove(gauges, col1, row2, gaugeTemplate.SizeX, gaugeTemplate.SizeY, offsetX1, offsetY1))
            return false;
        }
      }
      return true;
    }

    private Control InsertGauge(
      IGaugeTemplate gt,
      Control ctrl,
      bool undoable,
      bool selectOnLoaded)
    {
      Control control1 = (Control) null;
      if (gt is DependencyObject d)
      {
        Coordinate? insertPosition = GaugeHelper.GetInsertPosition(d);
        if (insertPosition.HasValue)
        {
          MoveDirectionType moveDirection = GaugeHelper.GetMoveDirection(d);
          if (moveDirection != MoveDirectionType.None)
            this.MoveGauges(insertPosition.Value.C, insertPosition.Value.R, gt.SizeX, gt.SizeY, moveDirection);
          this._fieldStates.Occupy(insertPosition.Value.R, insertPosition.Value.C, gt.SizeY, gt.SizeX);
          this.RecalculateSize();
          control1 = ctrl ?? gt.GetGauge() as Control;
          if (control1 != null)
          {
            if (selectOnLoaded)
              control1.Loaded += (RoutedEventHandler) ((s, a) =>
              {
                if (s is Control control3)
                  control3.Focus();
                if (!(s is IGaugeControl igaugeControl2) || !(igaugeControl2.Device is Gauge device2))
                  return;
                device2.Dirty += new EventHandler(this.Gauge_Dirty);
              });
            else
              control1.Loaded += (RoutedEventHandler) ((s, a) =>
              {
                --this._loadCounter;
                if (!(s is IGaugeControl igaugeControl4) || !(igaugeControl4.Device is Gauge device4))
                  return;
                device4.Dirty += new EventHandler(this.Gauge_Dirty);
              });
            control1.MouseLeftButtonDown += new MouseButtonEventHandler(this.Gauge_MouseLeftButtonDown);
            control1.MouseMove += new MouseEventHandler(this.Gauge_MouseMove);
            Grid.SetColumn((UIElement) control1, insertPosition.Value.C);
            Grid.SetColumnSpan((UIElement) control1, gt.SizeX);
            Grid.SetRow((UIElement) control1, insertPosition.Value.R);
            Grid.SetRowSpan((UIElement) control1, gt.SizeY);
            this.Children.Add((UIElement) control1);
            ++this._loadCounter;
            control1.IsTabStop = true;
            control1.Focusable = true;
            DataManager.Instance.IsLayoutDirty = true;
            GaugeHelper.SetAdorner((DependencyObject) control1, new HoverAdorner((UIElement) control1)
            {
              Stroke = (Brush) Brushes.BurlyWood
            });
            (control1 as IGaugeControl).Dirty += new EventHandler(this.Gauge_Dirty);
            if (undoable)
              UndoManager<Control>.Push(UndoActionType.Insert, control1, (object) null);
            this.InvalidateVisual();
          }
        }
      }
      return control1;
    }

    private void Gauge_MouseLeftButtonDown(object sender, MouseEventArgs e)
    {
      this._mousePos = e.GetPosition((IInputElement) this);
      if (sender is IGaugeControl && sender is UIElement uiElement)
      {
        // ISSUE: explicit non-virtual call
        uiElement.Focus();
      }
      e.Handled = true;
    }

    private void Gauge_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton != MouseButtonState.Pressed)
        return;
      Point position = e.GetPosition((IInputElement) this);
      double num1 = position.X - this._mousePos.X;
      double num2 = position.Y - this._mousePos.Y;
      if (Math.Sqrt(num1 * num1 + num2 * num2) < 4.0 || !(sender is IGaugeControl data))
        return;
      this.RemoveGauge((UIElement) sender, true);
      int num3 = (int) DragDrop.DoDragDrop((DependencyObject) this, (object) new DataObject(typeof (IGaugeControl), (object) data), DragDropEffects.Copy);
    }

    private void MoveGauges(
      int startCol,
      int startRow,
      int countCols,
      int countRows,
      MoveDirectionType dir)
    {
      if (dir == MoveDirectionType.None)
        return;
      Dictionary<int, IGaugeControl> gauges = new Dictionary<int, IGaugeControl>();
      int offsetX1 = dir == MoveDirectionType.Down ? 0 : countCols;
      int offsetY1 = dir == MoveDirectionType.Down ? countRows : 0;
      this.FetchAllGaugesToMove(gauges, startCol, startRow, countCols, countRows, offsetX1, offsetY1);
      foreach (IGaugeControl igaugeControl in gauges.Values)
      {
        UIElement uiElement = (UIElement) igaugeControl;
        int column = Grid.GetColumn(uiElement);
        int row = Grid.GetRow(uiElement);
        int columnSpan = Grid.GetColumnSpan(uiElement);
        int rowSpan = Grid.GetRowSpan(uiElement);
        GaugeMoveState state = new GaugeMoveState()
        {
          FromCol = column,
          FromRow = row,
          ToCol = column,
          ToRow = row
        };
        this._fieldStates.Release(row, column, rowSpan, columnSpan);
        int offsetX2 = GaugeHelper.GetOffsetX((DependencyObject) uiElement);
        int offsetY2 = GaugeHelper.GetOffsetY((DependencyObject) uiElement);
        if (dir == MoveDirectionType.Down)
        {
          row += offsetY2;
          Grid.SetRow(uiElement, row);
          state.ToRow = row;
        }
        else
        {
          column += offsetX2;
          Grid.SetColumn(uiElement, column);
          state.ToCol = column;
        }
        this._fieldStates.Occupy(row, column, rowSpan, columnSpan);
        GaugeHelper.SetInsertPosition(GaugeTemplateBase.GetGaugeTemplate((DependencyObject) uiElement) as DependencyObject, new Coordinate?(new Coordinate(row, column)));
        UndoManager<Control>.Push(UndoActionType.Move, (Control) igaugeControl, (object) state);
      }
    }

    private void ReverseMove(Control ctrl, GaugeMoveState state)
    {
      Control element = ctrl;
      int columnSpan = Grid.GetColumnSpan((UIElement) element);
      int rowSpan = Grid.GetRowSpan((UIElement) element);
      this._fieldStates.Release(state.ToRow, state.ToCol, rowSpan, columnSpan);
      Grid.SetRow((UIElement) element, state.FromRow);
      Grid.SetColumn((UIElement) element, state.FromCol);
      this._fieldStates.Occupy(state.FromRow, state.FromCol, rowSpan, columnSpan);
      this.InvalidateVisual();
    }

    private void Gauge_Dirty(object sender, EventArgs e)
    {
      DataManager.Instance.IsLayoutDirty = true;
    }

    private void OnBackgroundModeChanged(object sender, EventArgs a)
    {
      if (this._background.BackgroundMode == (BackgroundMode)3)
      {
        this._colWidth = 75;
        this._rowHeight = 20;
      }
      else
      {
        this._colWidth = 50;
        this._rowHeight = 50;
      }
      this.RecalculateSize();
    }

    private void RecalculateSize()
    {
      int num1 = -1;
      int num2 = -1;
      int num3 = -1;
      int num4 = -1;
      for (int r = 99; r >= 0; --r)
      {
        for (int c = 59; c >= 0; --c)
        {
          if (this._fieldStates[r, c] != LayoutGrid.FieldState.Locked)
          {
            if (this._fieldStates[r, c] == LayoutGrid.FieldState.Occupied)
            {
              if (c > num3)
                num3 = c;
              if (r > num4)
                num4 = r;
            }
            if (c > num1)
              num1 = c;
            if (r > num2)
              num2 = r;
          }
        }
      }
      int num5;
      DataManager.Instance.PackedWidth = (double) ((num5 = num3 + 1) * this._colWidth);
      int num6;
      DataManager.Instance.PackedHeight = (double) ((num6 = num4 + 1) * this._rowHeight);
      int val2 = num1 + 2;
      this.Width = (double) (Math.Max(4, val2) * this._colWidth);
      int num7 = num2 + (this._background.BackgroundMode == (BackgroundMode)3 ? 5 : 4);
      this.Height = (double) (num7 * this._rowHeight);
      while (this.RowDefinitions.Count > num7)
        this.RowDefinitions.RemoveAt(this.RowDefinitions.Count - 1);
      GridLength gridLength1 = new GridLength((double) this._rowHeight);
      for (int index = 0; index < this.RowDefinitions.Count; ++index)
        this.RowDefinitions[index].Height = gridLength1;
      while (this.RowDefinitions.Count < num7)
        this.RowDefinitions.Add(new RowDefinition()
        {
          Height = gridLength1
        });
      while (this.ColumnDefinitions.Count > val2)
        this.ColumnDefinitions.RemoveAt(this.ColumnDefinitions.Count - 1);
      GridLength gridLength2 = new GridLength((double) this._colWidth);
      for (int index = 0; index < this.ColumnDefinitions.Count; ++index)
        this.ColumnDefinitions[index].Width = gridLength2;
      while (this.ColumnDefinitions.Count < val2)
        this.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = gridLength2
        });
    }

    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is LayoutGrid layoutGrid))
        return;
      layoutGrid.OnZoomChanged();
    }

    private void OnZoomChanged() => DataManager.Instance.IsLayoutDirty = true;

    private void ZmlFile_ParseDocument(object sender, ZMLReadDocumentEventArgs e)
    {
      float num = XElementEx.GetAttrValue(e.Layout, (XName) "version", 0.0f);
      XElement xelement = e.Layout.Element((XName) "Background");
      if (xelement != null)
        this._background.Initialize(xelement);
      else
        this._background.BackgroundMode = XElementEx.GetAttrValue(e.Layout, (XName) "displayMode", "").ToLower() == "text" ? (BackgroundMode) 3 : (BackgroundMode) 0;
      if (this._background.BackgroundMode == (BackgroundMode)3)
      {
        this._colWidth = 75;
        this._rowHeight = 20;
        num = 1.1f;
      }
      else
      {
        this._colWidth = 50;
        this._rowHeight = 50;
      }
      this.Zoom = (double) XElementEx.GetAttrValue(e.Layout, (XName) "zoom", 1f);
      foreach (XElement element in e.Layout.Elements())
      {
        if (!(element.Name.LocalName == "Background"))
        {
          IGaugeTemplate template = GaugeTemplateFactory.CreateTemplate(e.Namespace, element, num);
          if (template != null)
          {
            //Coordinate coordinate; **HLI
            // ISSUE: explicit constructor call
            //((Coordinate) ref coordinate).\u002Ector(template.Row, template.Column); **HLI
            Coordinate coordinate = new Coordinate(template.Row, template.Column);

                        GaugeHelper.SetInsertPosition((DependencyObject) template, new Coordinate?(coordinate));
            this.InsertGauge(template, (Control) null, false, false);
          }
        }
      }
    }

    private void ZmlFile_SaveDocument(object sender, ZMLSaveDocumentEventArgs e)
    {
      XmlWriterEx.WriteAttributeFloat(e.Writer, "zoom", (float) this.Zoom, 3);
      this._background.Save(e.Writer);
      foreach (UIElement child in this.Children)
      {
        if (child is IGaugeControl igaugeControl)
          GaugeTemplateBase.GetGaugeTemplate((DependencyObject) child).Save(e.Writer, igaugeControl, Grid.GetColumn(child), Grid.GetRow(child));
      }
    }

    private enum FieldState
    {
      Locked,
      Placeholder,
      Occupied,
    }

    private class FieldStates
    {
      private readonly LayoutGrid.FieldState[,] _fieldStates = new LayoutGrid.FieldState[100, 60];

      public LayoutGrid.FieldState this[int r, int c] => this._fieldStates[r, c];

      public FieldStates() => this.Reset();

      public bool CanInsert(ref int row, ref int col, int nRows, int nCols)
      {
        if (row < 0 || row >= 100 || col < 0 || col >= 60 || nRows <= 0 || nCols <= 0)
          return false;
        int index1 = row;
        int index2 = col;
        int num1 = nRows;
        int num2 = nCols;
        while (this._fieldStates[index1, index2] != LayoutGrid.FieldState.Placeholder)
        {
          --num2;
          --index2;
          if (index2 < 0 || num2 == 0)
          {
            index2 = col;
            num2 = nCols;
            --index1;
            --num1;
            if (index1 < 0 || num1 == 0)
              return false;
          }
        }
        int num3 = 0;
        int index3 = index1;
        while (num3 < nRows)
        {
          int num4 = 0;
          int index4 = index2;
          while (num4 < nCols)
          {
            if (this._fieldStates[index3, index4] == LayoutGrid.FieldState.Occupied)
              return false;
            ++num4;
            ++index4;
          }
          ++num3;
          ++index3;
        }
        row = index1;
        col = index2;
        return true;
      }

      public void Occupy(int rStart, int cStart, int rows, int cols)
      {
        this.SetState(rStart, cStart, rows, cols, LayoutGrid.FieldState.Occupied);
        this.UpdatePlaceholder();
      }

      public void Release(int rStart, int cStart, int rows, int cols)
      {
        this.SetState(rStart, cStart, rows, cols, LayoutGrid.FieldState.Locked);
        this.UpdatePlaceholder();
      }

      public void Reset()
      {
        for (int index1 = 0; index1 < 100; ++index1)
        {
          for (int index2 = 0; index2 < 60; ++index2)
            this._fieldStates[index1, index2] = LayoutGrid.FieldState.Locked;
        }
        this._fieldStates[0, 0] = LayoutGrid.FieldState.Placeholder;
      }

      private void SetState(
        int rStart,
        int cStart,
        int rows,
        int cols,
        LayoutGrid.FieldState state)
      {
        int num1 = 0;
        int index1 = rStart;
        while (num1 < rows && num1 < 100)
        {
          int num2 = 0;
          int index2 = cStart;
          while (num2 < cols && num2 < 60)
          {
            this._fieldStates[index1, index2] = state;
            ++num2;
            ++index2;
          }
          ++num1;
          ++index1;
        }
      }

      private void SetState(
        int rStart,
        int cStart,
        int rows,
        int cols,
        LayoutGrid.FieldState state,
        Func<int, int, bool> condition)
      {
        int num1 = 0;
        int index1 = rStart;
        while (num1 < rows && num1 < 100)
        {
          int num2 = 0;
          int index2 = cStart;
          while (num2 < cols && num2 < 60)
          {
            if (condition(index1, index2))
              this._fieldStates[index1, index2] = state;
            ++num2;
            ++index2;
          }
          ++num1;
          ++index1;
        }
      }

      private void UpdatePlaceholder()
      {
        this.SetState(0, 0, 100, 60, LayoutGrid.FieldState.Locked, (Func<int, int, bool>) ((r, c) => this._fieldStates[r, c] != LayoutGrid.FieldState.Occupied));
        int num1 = -1;
        int num2 = -1;
        for (int index1 = 99; index1 >= 0; --index1)
        {
          for (int index2 = 59; index2 >= 0; --index2)
          {
            if (this._fieldStates[index1, index2] == LayoutGrid.FieldState.Occupied)
            {
              if (index2 > num1)
                num1 = index2;
              if (index1 > num2)
                num2 = index1;
            }
          }
        }
        int num3 = num2 + 1;
        int num4 = num1 + 1;
        if (num3 == 0 && num4 == 0)
        {
          this._fieldStates[0, 0] = LayoutGrid.FieldState.Placeholder;
        }
        else
        {
          int num5;
          int num6;
          this.SetState(0, 0, num5 = num3 + 1, num6 = num4 + 1, LayoutGrid.FieldState.Placeholder, (Func<int, int, bool>) ((r, c) => this._fieldStates[r, c] != LayoutGrid.FieldState.Occupied));
        }
      }
    }
  }
}
