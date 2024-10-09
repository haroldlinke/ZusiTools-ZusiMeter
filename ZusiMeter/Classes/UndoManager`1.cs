// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Classes.UndoManager`1
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using System.Collections.Generic;

#nullable disable
namespace ZusiMeter.Classes
{
  public class UndoManager<T>
  {
    private readonly Stack<UndoItem<T>> _stack = new Stack<UndoItem<T>>();
    private static UndoManager<T> __instance;

    protected UndoManager()
    {
    }

    private static UndoManager<T> GetInstance()
    {
      if (UndoManager<T>.__instance == null)
        UndoManager<T>.__instance = new UndoManager<T>();
      return UndoManager<T>.__instance;
    }

    private void Clear_impl() => this._stack.Clear();

    private UndoItem<T> Get_impl()
    {
      UndoItem<T> impl = this._stack.Pop();
      this._stack.Push(impl);
      return impl;
    }

    private bool IsEmpty_impl() => this._stack.Count == 0;

    private void Push_impl(UndoActionType action, T obj, object state)
    {
      this._stack.Push(new UndoItem<T>(action, obj, state));
    }

    private UndoItem<T> Pop_impl() => this._stack.Pop();

    public static void Clear() => UndoManager<T>.GetInstance().Clear_impl();

    public static UndoItem<T> Get() => UndoManager<T>.GetInstance().Get_impl();

    public static bool IsEmpty() => UndoManager<T>.GetInstance().IsEmpty_impl();

    public static void Push(UndoActionType action, T obj, object state)
    {
      UndoManager<T>.GetInstance().Push_impl(action, obj, state);
    }

    public static UndoItem<T> Pop() => UndoManager<T>.GetInstance().Pop_impl();
  }
}
