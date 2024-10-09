// Decompiled with JetBrains decompiler
// Type: ZusiMeter.Data.ToolBoxViewModel
// Assembly: ZusiMeter, Version=3.13.0.2, Culture=neutral, PublicKeyToken=null
// MVID: A0F120AF-B357-4240-A6C3-9D75D75958B8
// Assembly location: D:\data\Development\ZUSI-Tools\ZusiMeter\ZusiMeter_decomp\ZusiMeter.exe

using Sovoma.WPF;
using Sovoma;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using ZusiMeterGaugesLib.Common;
using ZusiMeterGaugesLib.GaugeTemplates;
using ZusiMeterGaugesLib.Interfaces;

#nullable disable
namespace ZusiMeter.Data
{
  public class ToolBoxViewModel : BaseTreeViewViewModel<ToolBoxViewModel, IGaugeTemplate>
  {
    private readonly List<ToolBoxViewModel> _captions = new List<ToolBoxViewModel>();

    internal static ToolBoxViewModel Create(PoolFile poolFile, bool textMode)
    {
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Root");
      XmlReaderSettings defaultReaderSettings = ZMLFile.DefaultReaderSettings;
      toolBoxViewModel.ParseDocument(XDocument.Load(XmlReader.Create(poolFile.Stream, defaultReaderSettings)), textMode);
      ((BaseTreeViewViewModel<ToolBoxViewModel, IGaugeTemplate>) toolBoxViewModel).Initialize();
      return toolBoxViewModel;
    }

    public bool CanCaption(int index)
    {
      --index;
      return index >= 0 && index < this._captions.Count;
    }

    public void Caption(int index)
    {
      --index;
      for (int index1 = 0; index1 < this._captions.Count; ++index1)
        this._captions[index1].IsExpanded = index1 == index;
    }

    public List<string> GetCaptions()
    {
      return this._captions.Select<ToolBoxViewModel, string>((Func<ToolBoxViewModel, string>) (t => t.DisplayName)).ToList<string>();
    }

    public virtual void Initialize()
    {
      base.Initialize();
      for (int index = 0; index < this._captions.Count; ++index)
        this._captions[index].IsExpanded = index == 0;
    }

    private static void Xrs_ValidationEventHandler(object sender, ValidationEventArgs e)
    {
      throw new Exception(e.Message);
    }

    private ToolBoxViewModel(string caption)
      : base((ToolBoxViewModel) null, true, false)
    {
      this._displayName = caption;
      this.IsBold = true;
    }

    private ToolBoxViewModel(IGaugeTemplate g)
      : base((ToolBoxViewModel) null, true, false)
    {
      this._displayName = g.DisplayName;
      this._object = g;
    }

    private void ParseDocument(XDocument doc, bool textMode)
    {
      XElement root = doc.Root;
      XNamespace ns = root.Name.Namespace;
      float attrValue = XElementEx.GetAttrValue(root, (XName) "version", 0.0f);
      if (textMode)
      {
        XElement xgauges = root.Element(ns + "TextGauges");
        this.ReadTextGauges(ns, xgauges, attrValue);
        XElement xctrlLamps = root.Element(ns + "TextControlLamps");
        this.ReadTextControlLamps(ns, xctrlLamps, attrValue);
        XElement xcomponents = root.Element(ns + "TextComponents");
        this.ReadTextComponents(ns, xcomponents, attrValue);
      }
      else
      {
        XElement xgauges1 = root.Element(ns + "AnalogGauges");
        this.ReadAnalogGauges(ns, xgauges1, attrValue);
        XElement xgauges2 = root.Element(ns + "HorizontalGauges");
        this.ReadHorizontalGauges(ns, xgauges2, attrValue);
        XElement xgauges3 = root.Element(ns + "VerticalGauges");
        this.ReadVerticalGauges(ns, xgauges3);
        XElement xgauges4 = root.Element(ns + "DigitalGauges");
        this.ReadDigitalGauges(ns, xgauges4, attrValue);
        XElement xctrlLamps = root.Element(ns + "ControlLamps");
        this.ReadControlLamps(ns, xctrlLamps, attrValue);
        XElement xcomponents1 = root.Element(ns + "Components");
        this.ReadComponents(ns, xcomponents1, attrValue);
        XElement xcomponents2 = root.Element(ns + "Operators");
        this.ReadOperators(ns, xcomponents2, attrValue);
      }
    }

    private void ReadAnalogGauges(XNamespace ns, XElement xgauges, float version)
    {
      Dictionary<string, List<IGaugeTemplate>> items = new Dictionary<string, List<IGaugeTemplate>>();
      foreach (XElement element in xgauges.Elements())
      {
        RoundGaugeTemplate gtb = new RoundGaugeTemplate(ns, element, version);
        string displayGroup = this.GetDisplayGroup((GaugeTemplateBase) gtb, "Sonstige");
        if (!items.ContainsKey(displayGroup))
          items[displayGroup] = new List<IGaugeTemplate>();
        items[displayGroup].Add((IGaugeTemplate) gtb);
      }
      List<string> list = items.Keys.Where<string>((Func<string, bool>) (k => k != "Sonstige")).ToList<string>();
      list.Sort();
      if (items.ContainsKey("Sonstige"))
        list.Add("Sonstige");
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Rundinstrumente");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      list.ForEach((Action<string>) (k =>
      {
        ToolBoxViewModel g = new ToolBoxViewModel(k);
        List<IGaugeTemplate> igaugeTemplateList = items[k];
        igaugeTemplateList.Sort((Comparison<IGaugeTemplate>) ((x, y) => string.Compare(x.DisplayName, y.DisplayName)));
        igaugeTemplateList.ForEach((Action<IGaugeTemplate>) (t => g.Children.Add(new ToolBoxViewModel(t))));
        tvm.Children.Add(g);
      }));
      this._children.Add(tvm);
    }

    private void ReadDigitalGauges(XNamespace ns, XElement xgauges, float version)
    {
      Dictionary<string, List<IGaugeTemplate>> items = new Dictionary<string, List<IGaugeTemplate>>();
      foreach (XElement element in xgauges.Elements())
      {
        DigitalGaugeTemplate gtb = new DigitalGaugeTemplate(ns, element, version);
        string displayGroup = this.GetDisplayGroup((GaugeTemplateBase) gtb, "Sonstige");
        if (!items.ContainsKey(displayGroup))
          items[displayGroup] = new List<IGaugeTemplate>();
        items[displayGroup].Add((IGaugeTemplate) gtb);
      }
      List<string> list = items.Keys.Where<string>((Func<string, bool>) (k => k != "Sonstige")).ToList<string>();
      list.Sort();
      if (items.ContainsKey("Sonstige"))
        list.Add("Sonstige");
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Digitale Instrumente");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      list.ForEach((Action<string>) (k =>
      {
        ToolBoxViewModel g = new ToolBoxViewModel(k);
        List<IGaugeTemplate> igaugeTemplateList = items[k];
        igaugeTemplateList.Sort((Comparison<IGaugeTemplate>) ((x, y) => string.Compare(x.DisplayName, y.DisplayName)));
        igaugeTemplateList.ForEach((Action<IGaugeTemplate>) (t => g.Children.Add(new ToolBoxViewModel(t))));
        tvm.Children.Add(g);
      }));
      this._children.Add(tvm);
    }

    private void ReadHorizontalGauges(XNamespace ns, XElement xgauges, float version)
    {
      Dictionary<string, List<IGaugeTemplate>> items = new Dictionary<string, List<IGaugeTemplate>>();
      foreach (XElement element in xgauges.Elements())
      {
        HorizontalGaugeTemplate gtb = new HorizontalGaugeTemplate(ns, element, version);
        string displayGroup = this.GetDisplayGroup((GaugeTemplateBase) gtb, "Sonstige");
        if (!items.ContainsKey(displayGroup))
          items[displayGroup] = new List<IGaugeTemplate>();
        items[displayGroup].Add((IGaugeTemplate) gtb);
      }
      List<string> list = items.Keys.Where<string>((Func<string, bool>) (k => k != "Sonstige")).ToList<string>();
      list.Sort();
      if (items.ContainsKey("Sonstige"))
        list.Add("Sonstige");
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Horz. Instrumente");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      list.ForEach((Action<string>) (k =>
      {
        ToolBoxViewModel g = new ToolBoxViewModel(k);
        List<IGaugeTemplate> igaugeTemplateList = items[k];
        igaugeTemplateList.Sort((Comparison<IGaugeTemplate>) ((x, y) => string.Compare(x.DisplayName, y.DisplayName)));
        igaugeTemplateList.ForEach((Action<IGaugeTemplate>) (t => g.Children.Add(new ToolBoxViewModel(t))));
        tvm.Children.Add(g);
      }));
      this._children.Add(tvm);
    }

    private void ReadVerticalGauges(XNamespace ns, XElement xgauges)
    {
      Dictionary<string, List<IGaugeTemplate>> items = new Dictionary<string, List<IGaugeTemplate>>();
      foreach (XElement element in xgauges.Elements())
      {
        VerticalGaugeTemplate gtb = new VerticalGaugeTemplate(ns, element, 1.1f);
        string displayGroup = this.GetDisplayGroup((GaugeTemplateBase) gtb, "Sonstige");
        if (!items.ContainsKey(displayGroup))
          items[displayGroup] = new List<IGaugeTemplate>();
        items[displayGroup].Add((IGaugeTemplate) gtb);
      }
      List<string> list = items.Keys.Where<string>((Func<string, bool>) (k => k != "Sonstige")).ToList<string>();
      list.Sort();
      if (items.ContainsKey("Sonstige"))
        list.Add("Sonstige");
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Vert. Instrumente");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      list.ForEach((Action<string>) (k =>
      {
        ToolBoxViewModel g = new ToolBoxViewModel(k);
        List<IGaugeTemplate> igaugeTemplateList = items[k];
        igaugeTemplateList.Sort((Comparison<IGaugeTemplate>) ((x, y) => string.Compare(x.DisplayName, y.DisplayName)));
        igaugeTemplateList.ForEach((Action<IGaugeTemplate>) (t => g.Children.Add(new ToolBoxViewModel(t))));
        tvm.Children.Add(g);
      }));
      this._children.Add(tvm);
    }

    private void ReadControlLamps(XNamespace ns, XElement xctrlLamps, float version)
    {
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Melder");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      List<ControlLampTemplate> controlLampTemplateList = new List<ControlLampTemplate>();
      foreach (XElement element in xctrlLamps.Elements())
        controlLampTemplateList.Add(new ControlLampTemplate(ns, element, version));
      controlLampTemplateList.Sort((Comparison<ControlLampTemplate>) ((x, y) => string.Compare(((GaugeTemplateBase) x).DisplayName, ((GaugeTemplateBase) y).DisplayName)));
      controlLampTemplateList.ForEach((Action<ControlLampTemplate>) (t => tvm.Children.Add(new ToolBoxViewModel((IGaugeTemplate) t))));
      this._children.Add(tvm);
    }

    private void ReadComponents(XNamespace ns, XElement xcomponents, float version)
    {
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Baugruppen");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      List<ComponentTemplate> componentTemplateList = new List<ComponentTemplate>();
      foreach (XElement element in xcomponents.Elements())
        componentTemplateList.Add(new ComponentTemplate(ns, element, version));
      componentTemplateList.Sort((Comparison<ComponentTemplate>) ((x, y) => string.Compare(((GaugeTemplateBase) x).DisplayName, ((GaugeTemplateBase) y).DisplayName)));
      componentTemplateList.ForEach((Action<ComponentTemplate>) (t => tvm.Children.Add(new ToolBoxViewModel((IGaugeTemplate) t))));
      this._children.Add(tvm);
    }

    private void ReadOperators(XNamespace ns, XElement xcomponents, float version)
    {
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Interaktiv");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      List<OperatorTemplate> operatorTemplateList = new List<OperatorTemplate>();
      foreach (XElement element in xcomponents.Elements())
        operatorTemplateList.Add(new OperatorTemplate(ns, element, version));
      operatorTemplateList.Sort((Comparison<OperatorTemplate>) ((x, y) => string.Compare(x.DisplayName, y.DisplayName)));
      operatorTemplateList.ForEach((Action<OperatorTemplate>) (t => tvm.Children.Add(new ToolBoxViewModel((IGaugeTemplate) t))));
      this._children.Add(tvm);
    }

    private void ReadTextGauges(XNamespace ns, XElement xgauges, float version)
    {
      Dictionary<string, List<IGaugeTemplate>> items = new Dictionary<string, List<IGaugeTemplate>>();
      foreach (XElement element in xgauges.Elements())
      {
        TextGaugeTemplate gtb = new TextGaugeTemplate(ns, element, version);
        string displayGroup = this.GetDisplayGroup((GaugeTemplateBase) gtb, "Sonstige");
        if (!items.ContainsKey(displayGroup))
          items[displayGroup] = new List<IGaugeTemplate>();
        items[displayGroup].Add((IGaugeTemplate) gtb);
      }
      List<string> list = items.Keys.Where<string>((Func<string, bool>) (k => k != "Sonstige")).ToList<string>();
      list.Sort();
      if (items.ContainsKey("Sonstige"))
        list.Add("Sonstige");
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Instrumente");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      list.ForEach((Action<string>) (k =>
      {
        ToolBoxViewModel g = new ToolBoxViewModel(k);
        List<IGaugeTemplate> igaugeTemplateList = items[k];
        igaugeTemplateList.Sort((Comparison<IGaugeTemplate>) ((x, y) => string.Compare(x.DisplayName, y.DisplayName)));
        igaugeTemplateList.ForEach((Action<IGaugeTemplate>) (t => g.Children.Add(new ToolBoxViewModel(t))));
        tvm.Children.Add(g);
      }));
      this._children.Add(tvm);
    }

    private void ReadTextControlLamps(XNamespace ns, XElement xctrlLamps, float version)
    {
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Melder");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      List<ControlLampTemplate> controlLampTemplateList = new List<ControlLampTemplate>();
      foreach (XElement element in xctrlLamps.Elements())
        controlLampTemplateList.Add((ControlLampTemplate) new TextControlLampTemplate(ns, element, version));
      controlLampTemplateList.Sort((Comparison<ControlLampTemplate>) ((x, y) => string.Compare(((GaugeTemplateBase) x).DisplayName, ((GaugeTemplateBase) y).DisplayName)));
      controlLampTemplateList.ForEach((Action<ControlLampTemplate>) (t => tvm.Children.Add(new ToolBoxViewModel((IGaugeTemplate) t))));
      this._children.Add(tvm);
    }

    private void ReadTextComponents(XNamespace ns, XElement xcomponents, float version)
    {
      ToolBoxViewModel toolBoxViewModel = new ToolBoxViewModel("Baugruppen");
      toolBoxViewModel.Anything = true;
      ToolBoxViewModel tvm = toolBoxViewModel;
      this._captions.Add(tvm);
      List<ComponentTemplate> componentTemplateList = new List<ComponentTemplate>();
      foreach (XElement element in xcomponents.Elements())
        componentTemplateList.Add((ComponentTemplate) new TextComponentTemplate(ns, element, version));
      componentTemplateList.Sort((Comparison<ComponentTemplate>) ((x, y) => string.Compare(((GaugeTemplateBase) x).DisplayName, ((GaugeTemplateBase) y).DisplayName)));
      componentTemplateList.ForEach((Action<ComponentTemplate>) (t => tvm.Children.Add(new ToolBoxViewModel((IGaugeTemplate) t))));
      this._children.Add(tvm);
    }

    private string GetDisplayGroup(GaugeTemplateBase gtb, string defaultGroup)
    {
      string displayGroup = gtb.DisplayGroup;
      return !string.IsNullOrEmpty(displayGroup) ? displayGroup : defaultGroup;
    }
  }
}
