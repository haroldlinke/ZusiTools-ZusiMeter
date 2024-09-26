// Decompiled with JetBrains decompiler
// Type: ZusiMeterKonfigurator.Classes.UndoItem`1
// Assembly: ZusiMeterKonfigurator, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeterKonfigurator_decomp\ZusiMeterKonfigurator.exe

#nullable disable
namespace ZusiMeterKonfigurator.Classes
{
  public class UndoItem<T>
  {
    private readonly T _object;
    private readonly UndoActionType _action;
    private readonly object _state;

    public T Object => this._object;

    public UndoActionType Action => this._action;

    public object State => this._state;

    public UndoItem(UndoActionType action, T obj, object state)
    {
      this._action = action;
      this._object = obj;
      this._state = state;
    }
  }
}
