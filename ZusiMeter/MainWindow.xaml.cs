using log4net;
using Microsoft.Win32;
using Sovoma;
using Sovoma.WPF.Converter;
using Sovoma.WPF;
using Sovoma.WPF.Network;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xml.Linq;
using ZusiFahrpultLib;
using ZusiMeter.Miscellaneous;
using ZusiMeter.Properties;
using ZusiMeterGaugesLib.Common;
using ZusiMeterGaugesLib.Components;
using ZusiMeterGaugesLib.Controls;
using ZusiMeterGaugesLib.DigitalGauges;
using ZusiMeterGaugesLib.GaugeTemplates;
using ZusiMeterGaugesLib.Interfaces;
using ZusiMeterGaugesLib.Managers;
using ZusiMeterGaugesLib.TheRailRunner;
using ZusiMeterGaugesLib.Utils;
using Zusisuplib;
using System.Windows.Interop;

namespace ZusiMeter
{
  public partial class MainWindow : Window, IComponentConnector  //**HLI
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof (MainWindow));
    private static readonly string? _defaultTitle = AsmInfo.Product;
    private readonly ObservableCollection<string> _layoutFiles = new ObservableCollection<string>();
    private BeaconReceiver _beaconReceiver = new();
    private readonly List<IGauge> _gauges = new();
    private readonly List<ZFtdID> _gaugeIds = new ();
    private readonly List<ZProgID> _gaugeProgIds = new ();
    private FahrpultClient _fahrpult;
    private readonly LayoutBackground _background;
    private readonly bool _initialized;
    private bool _mustReconnect;
    private double _colWidth;
    private double _rowHeight;
    private readonly System.Timers.Timer _timerZusiMelderConf = new System.Timers.Timer(2000.0);
    private readonly System.Timers.Timer _timerGracePeriod = new System.Timers.Timer(3000.0);
    private bool _willIPBoardSwitchVisible;
    private bool _beaconDetected;
    private readonly ConcurrentQueue<EventArgs> _dataQueue = new ConcurrentQueue<EventArgs>();
    private readonly AutoResetEvent _dataWakeUp = new AutoResetEvent(false);
    private readonly bool _dataCancellation;
    private bool _trackWindow;
    private string? _curentlayoutfile = null;
    private static string? _currentlayoutfolder = null;  
    private static readonly DependencyPropertyKey _keyCanActivate = DependencyProperty.RegisterReadOnly(nameof (CanActivate), typeof (bool), typeof (MainWindow), new PropertyMetadata((object) true));
    public static readonly DependencyProperty CanActivateProperty = MainWindow._keyCanActivate.DependencyProperty;
    public static readonly DependencyProperty HostProperty = DependencyProperty.Register(nameof (Host), typeof (string), typeof (MainWindow), new PropertyMetadata((object) null, new PropertyChangedCallback(MainWindow.OnHostChanged)));
    private static readonly DependencyPropertyKey _keyIsIPBoardSwitchVisible = DependencyProperty.RegisterReadOnly(nameof (IsIPBoardSwitchVisible), typeof (bool), typeof (MainWindow), new PropertyMetadata((object) false));
    public static readonly DependencyProperty IsIPBoardSwitchVisibleProperty = MainWindow._keyIsIPBoardSwitchVisible.DependencyProperty;
    public static readonly DependencyProperty IsZusiMelderConfVisibleProperty = DependencyProperty.Register(nameof (IsZusiMelderConfVisible), typeof (bool), typeof (MainWindow), new PropertyMetadata((object) false));
    private static readonly DependencyPropertyKey _missingConfigurationKey = DependencyProperty.RegisterReadOnly(nameof (MissingConfiguration), typeof (bool), typeof (MainWindow), new PropertyMetadata((object) false));
    public static readonly DependencyProperty MissingConfigurationProperty = MainWindow._missingConfigurationKey.DependencyProperty;
    public static readonly DependencyProperty PortProperty = DependencyProperty.Register(nameof (Port), typeof (int), typeof (MainWindow), new PropertyMetadata((object) 0, new PropertyChangedCallback(MainWindow.OnPortChanged)));
    private static readonly DependencyPropertyKey _selectLayoutKey = DependencyProperty.RegisterReadOnly(nameof (SelectLayout), typeof (bool), typeof (MainWindow), new PropertyMetadata((object) true));
    public static readonly DependencyProperty SelectLayoutProperty = MainWindow._selectLayoutKey.DependencyProperty;
    private static readonly DependencyPropertyKey _zoomKey = DependencyProperty.RegisterReadOnly(nameof (Zoom), typeof (double), typeof (MainWindow), new PropertyMetadata((object) 1.0));
    public static readonly DependencyProperty ZoomProperty = MainWindow._zoomKey.DependencyProperty;
    public static readonly DependencyProperty ZusiConfigurationProperty = DependencyProperty.Register(nameof (ZusiConfiguration), typeof (ZusiConfigurationMode), typeof (MainWindow), new PropertyMetadata((object) (ZusiConfigurationMode) Settings.Default.ZusiConfiguration, new PropertyChangedCallback(MainWindow.OnZusiConfigurationChanged)));
    private static readonly DependencyPropertyKey _keyZusiConnectionState = DependencyProperty.RegisterReadOnly(nameof (ZusiConnectionState), typeof (int), typeof (MainWindow), new PropertyMetadata((object) 0, (PropertyChangedCallback) ((d, e) => CommandManager.InvalidateRequerySuggested())));
    public static readonly DependencyProperty ZusiConnectionStateProperty = MainWindow._keyZusiConnectionState.DependencyProperty;
    private static readonly DependencyPropertyKey _keyZusiConnectionString = DependencyProperty.RegisterReadOnly(nameof (ZusiConnectionString), typeof (string), typeof (MainWindow), new PropertyMetadata((PropertyChangedCallback) null));
    public static readonly DependencyProperty ZusiConnectionStringProperty = MainWindow._keyZusiConnectionString.DependencyProperty;
    public static readonly RoutedUICommand CommandLoadLayout = new RoutedUICommand("_Layout laden", nameof (CommandLoadLayout), typeof (MainWindow));
    public static readonly RoutedUICommand CommandLoadOtherLayout = new RoutedUICommand("_Anderes Layout laden", nameof (CommandLoadOtherLayout), typeof (MainWindow), CommandKey.F(Key.F2));
    public static readonly RoutedUICommand CommandAppHelper = new RoutedUICommand("_", nameof (CommandAppHelper), typeof (MainWindow), new InputGestureCollection((IList) new InputGesture[1]
    {
      (InputGesture) new KeyGesture(Key.Z, ModifierKeys.Control | ModifierKeys.Shift)
    }));
    public static readonly RoutedUICommand CommandPause = new RoutedUICommand("", nameof (CommandPause), typeof (MainWindow));
    public static readonly RoutedUICommand CommandTimejump = new RoutedUICommand("", nameof (CommandTimejump), typeof (MainWindow));
    public static readonly RoutedUICommand CommandTimelapse = new RoutedUICommand("", nameof (CommandTimelapse), typeof (MainWindow));
    public static readonly RoutedUICommand CommandBack = new RoutedUICommand("", nameof (CommandBack), typeof (MainWindow));
    public static readonly RoutedUICommand CommandExitApp = new RoutedUICommand("", nameof (CommandExitApp), typeof (MainWindow), CommandKey.Alt_F(Key.F4));
    public static readonly RoutedUICommand CommandMinimizeApp = new RoutedUICommand("", nameof (CommandMinimizeApp), typeof (MainWindow));
    public static readonly RoutedUICommand CommandSendToAutoStart = new RoutedUICommand("", nameof(CommandSendToAutoStart), typeof(MainWindow));
    public static readonly RoutedUICommand CommandOpenIPConnConf = new RoutedUICommand("", nameof(CommandOpenIPConnConf), typeof(MainWindow));
        public bool CanActivate
    {
      get => (bool) this.GetValue(MainWindow.CanActivateProperty);
      private set => this.SetValue(MainWindow._keyCanActivate, (object) value);
    }

    public string Host
    {
      get => (string) this.GetValue(MainWindow.HostProperty);
      set => this.SetValue(MainWindow.HostProperty, (object) value);
    }

    public bool IsIPBoardSwitchVisible
    {
      get => (bool) this.GetValue(MainWindow.IsIPBoardSwitchVisibleProperty);
      private set => this.SetValue(MainWindow._keyIsIPBoardSwitchVisible, (object) value);
    }

    public bool IsZusiMelderConfVisible
    {
      get => (bool) this.GetValue(MainWindow.IsZusiMelderConfVisibleProperty);
      set => this.SetValue(MainWindow.IsZusiMelderConfVisibleProperty, (object) value);
    }

    public bool MissingConfiguration
    {
      get => (bool) this.GetValue(MainWindow.MissingConfigurationProperty);
      private set => this.SetValue(MainWindow._missingConfigurationKey, (object) value);
    }

    public int Port
    {
      get => (int) this.GetValue(MainWindow.PortProperty);
      set => this.SetValue(MainWindow.PortProperty, (object) value);
    }

    public bool SelectLayout
    {
      get => (bool) this.GetValue(MainWindow.SelectLayoutProperty);
      private set => this.SetValue(MainWindow._selectLayoutKey, (object) value);
    }

    public double Zoom
    {
      get => (double) this.GetValue(MainWindow.ZoomProperty);
      private set => this.SetValue(MainWindow._zoomKey, (object) value);
    }

    public ZusiConfigurationMode ZusiConfiguration
    {
      get => (ZusiConfigurationMode) this.GetValue(MainWindow.ZusiConfigurationProperty);
      set => this.SetValue(MainWindow.ZusiConfigurationProperty, (object) value);
    }

    public int ZusiConnectionState
    {
      get => (int) this.GetValue(MainWindow.ZusiConnectionStateProperty);
      private set => this.SetValue(MainWindow._keyZusiConnectionState, (object) value);
    }

    public string ZusiConnectionString
    {
      get => (string) this.GetValue(MainWindow.ZusiConnectionStringProperty);
      private set => this.SetValue(MainWindow._keyZusiConnectionString, (object) value);
    }

    public ObservableCollection<string> LayoutFiles => this._layoutFiles;

    public LayoutBackground LayoutBackground => this._background;

    private static string GetZusiMeterLayoutFileDir()
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

    static MainWindow()
    {
      Window.LeftProperty.AddOwner(typeof (MainWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(MainWindow.OnLeftChanged)));
      Window.TopProperty.AddOwner(typeof (MainWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) double.NaN, new PropertyChangedCallback(MainWindow.OnTopChanged)));
      Window.TopmostProperty.AddOwner(typeof (MainWindow), (PropertyMetadata) new FrameworkPropertyMetadata((object) false, new PropertyChangedCallback(MainWindow.OnTopMostChanged)));
    }

    public MainWindow()
    {
      this.Host = Settings.Default.HostOrIP;
      this.Port = Settings.Default.Port;
      //if (Settings.Default.VintageBackground) // **HLI
      //  BackgroundSettings.SetVintageBackground(); // **HLI
      this._background = new LayoutBackground();
      this.ObtainLayoutFiles();
      this.InitializeComponent();
      this._initialized = true;
      this._timerZusiMelderConf.AutoReset = false;
      this._timerZusiMelderConf.Elapsed += new ElapsedEventHandler(this.TimerZusiMelderConf_Elapsed);
      this._timerGracePeriod.AutoReset = false;
      this._timerGracePeriod.Elapsed += new ElapsedEventHandler(this.TimerGracePeriod_Elapsed);
      this.Title = AppHelper.CanBringZusiToFront ? MainWindow._defaultTitle : MainWindow._defaultTitle + " (*)";
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandLoadLayout, new ExecutedRoutedEventHandler(this.OnLoadLayout), new CanExecuteRoutedEventHandler(this.OnCanLoadLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandLoadOtherLayout, new ExecutedRoutedEventHandler(this.OnLoadOtherLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandAppHelper, new ExecutedRoutedEventHandler(this.OnAppHelper)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandPause, new ExecutedRoutedEventHandler(this.OnPause), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.ZusiConnectionState == 2)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandTimejump, new ExecutedRoutedEventHandler(this.OnTimejump), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.ZusiConnectionState == 2)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandTimelapse, new ExecutedRoutedEventHandler(this.OnTimelapse), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = this.ZusiConnectionState == 2)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandBack, new ExecutedRoutedEventHandler(this.OnBack), (CanExecuteRoutedEventHandler) ((s, e) => e.CanExecute = !this.SelectLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandExitApp, (ExecutedRoutedEventHandler) ((s, e) => this.Close())));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandMinimizeApp, new ExecutedRoutedEventHandler(this.OnMinimize), new CanExecuteRoutedEventHandler(this.OnCanMinimize)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandSendToAutoStart, new ExecutedRoutedEventHandler(this.OnSendToAutoStart), (CanExecuteRoutedEventHandler)((s, e) => e.CanExecute = !this.SelectLayout)));
      this.CommandBindings.Add(new CommandBinding((ICommand) MainWindow.CommandOpenIPConnConf, new ExecutedRoutedEventHandler(this.OnOpenIPConnConf)));

            this.AddHandler(GaugeEventsManager.RegisterGaugeEvent, (Delegate) new RoutedEventHandler(this.MainWindow_RegisterGauge));
      this.Closing += new CancelEventHandler(this.MainWindow_Closing);
      this.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
      this._beaconReceiver.BeaconSignalReceived += new BeaconSignalReceivedEventHandler(this.BeaconReceiver_BeaconSignalReceived);
      this.IsIPBoardSwitchVisible = this.ZusiConfiguration == ZusiConfigurationMode.Manual;
      this._willIPBoardSwitchVisible = this.IsIPBoardSwitchVisible;
    }

    private void MainWindow_Closing(object sender, CancelEventArgs e)
    {
      Disposable.Dispose<BeaconReceiver>(ref this._beaconReceiver);
      this.DisconnectFahrpult();
      Zusiaccess.CreateZUSIMenuEntry(Bezeichnertext: "ZusiMeter", Vatermenu: "SpTBXSubmenuItemKonfiguration", MenuIndex: 17, Params: _curentlayoutfile);
        }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.DataProcessor();
      string[] commandLineArgs = Environment.GetCommandLineArgs();

      //string message = "";

      //for (int i = 0; i < commandLineArgs.Length; i++)
      //      {
      //          message = message + commandLineArgs[i] + "\n";
      //      }
      //if (commandLineArgs.Length > 0)
      //          MessageBox.Show(message);

      if (commandLineArgs.Length == 2 && System.IO.File.Exists(commandLineArgs[1]))
      {
        this.CheckConfiguration();
        //this.Dispatcher.BeginInvoke((Delegate) (s => this.LoadLayout(s)), DispatcherPriority.Loaded, (object) commandLineArgs[1]); **HLI
        this.Dispatcher.BeginInvoke(new Action<string>(s => this.LoadLayout(s)), DispatcherPriority.Loaded, (object)commandLineArgs[1]);
      }
      else
      {
        switch (this.ZusiConfiguration)
        {
          case ZusiConfigurationMode.AutoDetect:
            this._beaconReceiver.ReceiveAsync();
            this._timerGracePeriod.Start();
            break;
          case ZusiConfigurationMode.Manual:
            this.CheckConfiguration();
            break;
        }
      }
    }

    private void MainWindow_RegisterGauge(object sender, RoutedEventArgs e)
    {
      e.Handled = true;
      if (this.SelectLayout || !(e.OriginalSource is IGauge originalSource))
        return;
      this._gauges.Add(originalSource);
      switch (originalSource)
      {
        case RailRunner railRunner:
          railRunner.VolumeChanged += (EventHandler)((s, a) =>
          {
            Settings.Default.RailRunnerVolume = ((RailRunner) s).Volume;
            Settings.Default.Save();
          });
          if (railRunner.Volume != 0.5)
            break;
          railRunner.Volume = Settings.Default.RailRunnerVolume;
          break;
        case DigitalNextStopGauge digitalNextStopGauge:
          digitalNextStopGauge.VolumeChanged += (EventHandler) ((s, a) =>
          {
            Settings.Default.RailRunnerVolume = ((DigitalNextStopGauge) s).Volume;
            Settings.Default.Save();
          });
          break;
        case ComponentTueren componentTueren:
          componentTueren.DoorButton += new DoorButtonEventHandler(this.ComponentTueren_DoorButton);
          componentTueren.DoorSideSwitchPositionChanged += new DoorSideSwitchPositionChangedEventHandler(this.ComponentTueren_DoorSideSwitchPositionChanged);
          break;
      }
    }

    private void ComponentTueren_DoorButton(object sender, DoorButtonEventArgs e)
    {
      if (e.ButtonType == DoorButtonType.DoorRelease)
      {
        if (e.IsPressed)
          this._fahrpult?.SendKeyboardCommand(new KeyboardCommand(KbdAssignment.Türen, KbdAction.Down, KbdCommand.TuerenTasterDown));
        else
          this._fahrpult?.SendKeyboardCommand(new KeyboardCommand(KbdAssignment.Türen, KbdAction.Up, KbdCommand.TuerenTasterUp));
      }
      else
      {
        if (e.ButtonType != DoorButtonType.ForceClosing)
          return;
        if (e.IsPressed)
          this._fahrpult?.SendKeyboardCommand(new KeyboardCommand(KbdAssignment.Türen, KbdAction.Down, KbdCommand.TuerenZuDown));
        else
          this._fahrpult?.SendKeyboardCommand(new KeyboardCommand(KbdAssignment.Türen, KbdAction.Up, KbdCommand.TuerenZuUp));
      }
    }

    private void ComponentTueren_DoorSideSwitchPositionChanged(
      object sender,
      DoorSideSwitchPositionEventArg e)
    {
      if (this._fahrpult == null)
        return;
      int num = e.DesiredPosition - e.CurrentPosition;
      switch (num)
      {
        case -3:
          num = 1;
          break;
        case 0:
          return;
        case 3:
          num = -1;
          break;
      }
      for (int index = 0; index < Math.Abs(num); ++index)
      {
        this._fahrpult.SendKeyboardCommand(new KeyboardCommand(KbdAssignment.Türen, KbdAction.Down, num > 0 ? KbdCommand.TuerenReDown : KbdCommand.TuerenLiDown));
        this._fahrpult.SendKeyboardCommand(new KeyboardCommand(KbdAssignment.Türen, KbdAction.Up, num > 0 ? KbdCommand.TuerenReUp : KbdCommand.TuerenLiUp));
      }
    }

    private void Fahrpult_ClientConnected(object sender, ClientConnectedEventArgs e)
    {
      //this.Dispatcher.BeginInvoke((Delegate) ((v, i) =>  **HLI
      //{ **HLI
      //  this.ZusiConnectionState = e.ClientAccepted ? (e.NeededDataAccepted ? 2 : 1) : 0; **HLI
      //  this.ZusiConnectionString = "Zusi-Version " + v + "/[" + i + "]"; **HLI
      //}), (object) e.ZusiVersion, (object) e.ZusiConnectionInfo); **HLI

        this.Dispatcher.BeginInvoke(new Action<object, object>((v, i) => //**HLI
        { //**HLI
            this.ZusiConnectionState = e.ClientAccepted ? (e.NeededDataAccepted ? 2 : 1) : 0; //**HLI
            this.ZusiConnectionString = "Zusi-Version " + v + "/[" + i + "]"; //**HLI
        }), DispatcherPriority.Loaded, (object)e.ZusiVersion, (object)e.ZusiConnectionInfo); // **HLI

    }

    private void Fahrpult_FtdDataReceived(object sender, FtdDataReceivedEventArgs e)
    {
      this._dataQueue.Enqueue((EventArgs) e);
      this._dataWakeUp.Set();
    }

    private void Fahrpult_ProgDataReceived(object sender, ProgDataReceivedEventArgs e)
    {
      this._dataQueue.Enqueue((EventArgs) e);
      this._dataWakeUp.Set();
    }

    private void Fahrpult_Disconnected(object sender, EventArgs e)
    {
      MainWindow._log.Debug((object) "Fahrpult getrennt");
      this.Dispatcher.BeginInvoke((() =>
      {
        this.ZusiConnectionState = 0;
        this.ZusiConnectionString = (string) null;
        do
          ;
        while (this._dataQueue.TryDequeue(out EventArgs _));
        this._gauges.ForEach((Action<IGauge>) (g => g.ResetValue()));
      }));
    }

    private void CbxIPBoard_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = ((ToggleButton) sender).IsChecked;
      bool flag = false;
      if (!(isChecked.GetValueOrDefault() == flag & isChecked.HasValue) || !this._mustReconnect)
        return;
      this.ReconnectFahrpult();
    }

    private void StatusBar_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (!this.SelectLayout || (Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
        return;
      this._timerGracePeriod.Stop();
      this.cbxIPBoard.IsChecked = new bool?(false);
      this.IsZusiMelderConfVisible = true;
      this._timerZusiMelderConf.Start();
    }

    private void TimerGracePeriod_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.Dispatcher.BeginInvoke((() => this.CheckConfiguration()));
    }

    private void TimerZusiMelderConf_Elapsed(object sender, ElapsedEventArgs e)
    {
      this.Dispatcher.BeginInvoke((() =>
      {
        this.IsZusiMelderConfVisible = false;
        this.IsIPBoardSwitchVisible = this._willIPBoardSwitchVisible;
      }));
    }

    private void ZusiMelderConf_MouseEnter(object sender, MouseEventArgs e)
    {
      this._timerZusiMelderConf.Stop();
    }

    private void ZusiMelderConf_MouseLeave(object sender, MouseEventArgs e)
    {
          this._timerZusiMelderConf.Start();
        //if (this.ZusiConfiguration == ZusiConfigurationMode.Manual)
        //    this.cbxIPBoard.IsChecked = true;
        //else
        //    this.cbxIPBoard.IsChecked = false;
    }

    private static void OnHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as MainWindow).OnHostChanged((string) e.NewValue);
    }

    private void OnHostChanged(string host)
    {
      if (!this._initialized)
        return;
      this.CheckConfiguration();
      Settings.Default.HostOrIP = host;
      Settings.Default.Save();
      this.DisconnectFahrpult();
      this._mustReconnect = true;
    }

    private static void OnLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MainWindow mainWindow))
        return;
      mainWindow.OnLeftChanged((double) e.NewValue);
    }

    private void OnLeftChanged(double value)
    {
      if (!this._trackWindow)
        return;
      WindowPosition.Left = value;
    }

    private static void OnPortChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as MainWindow).OnPortChanged((int) e.NewValue);
    }

    private void OnPortChanged(int port)
    {
      if (!this._initialized)
        return;
      Settings.Default.Port = port;
      Settings.Default.Save();
      this.DisconnectFahrpult();
      this._mustReconnect = true;
    }

    private static void OnTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MainWindow mainWindow))
        return;
      mainWindow.OnTopChanged((double) e.NewValue);
    }

    private void OnTopChanged(double value)
    {
      if (!this._trackWindow)
        return;
      WindowPosition.Top = value;
    }

    private static void OnTopMostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      (d as MainWindow).OnTopMostChanged((bool) e.NewValue);
    }

    private void OnTopMostChanged(bool value)
    {
      if (!this._trackWindow)
        return;
      WindowPosition.Topmost = value;
    }

    private static void OnZusiConfigurationChanged(
      DependencyObject d,
      DependencyPropertyChangedEventArgs e)
    {
      if (!(d is MainWindow mainWindow))
        return;
      mainWindow.OnZusiConfigurationChanged((ZusiConfigurationMode) e.NewValue);
    }

    private void OnZusiConfigurationChanged(ZusiConfigurationMode value)
    {
      Settings.Default.ZusiConfiguration = (int) value;
      Settings.Default.Save();
      this._willIPBoardSwitchVisible = this.SelectLayout && value == ZusiConfigurationMode.Manual;
      switch (value)
      {
        case ZusiConfigurationMode.LocalHost:
          this.MissingConfiguration = false;
          this._beaconReceiver.ShutDown();
          break;
        case ZusiConfigurationMode.AutoDetect:
          this.MissingConfiguration = false;
          if (!this._beaconReceiver.IsListening)
            this._beaconReceiver.ReceiveAsync();
          this._timerGracePeriod.Start();
          break;
        case ZusiConfigurationMode.Manual:
          this._beaconReceiver.ShutDown();
          this.CheckConfiguration();
          break;
      }
    }

    private void LvLayoutFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count <= 0)
        return;
      this.preview?.ShowPreview((string) e.AddedItems[0]);
    }

    private void OnCanLoadLayout(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = this.lvLayoutFiles.SelectedItem != null && this.ZusiConnectionState == 0;
    }

    private void OnLoadLayout(object sender, ExecutedRoutedEventArgs e)
    {
      this.HideIPBoard();
      //this.Dispatcher.BeginInvoke((Delegate) (s => this.LoadLayout(s)), DispatcherPriority.Loaded, (object) (string) this.lvLayoutFiles.SelectedItem); **HLI
      this.Dispatcher.BeginInvoke(new Action<string>(s => this.LoadLayout(s)), DispatcherPriority.Loaded, (string)this.lvLayoutFiles.SelectedItem);
    }

    private void OnLoadOtherLayout(object sender, ExecutedRoutedEventArgs e)
    {
      this.HideIPBoard();
      //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ZusiMeterLayouts");
      string path = Path.Combine(GetZusiMeterLayoutFileDir());
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      OpenFileDialog openFileDialog1 = new OpenFileDialog();
      openFileDialog1.DefaultExt = "zmlf";
      openFileDialog1.Filter = "ZusiMeter-Layoutdateien|*.zmlf|Alle Dateien|*.*";
      openFileDialog1.InitialDirectory = path;
      openFileDialog1.Multiselect = false;
      openFileDialog1.Title = "Layout laden";
      OpenFileDialog openFileDialog2 = openFileDialog1;
      bool? nullable = openFileDialog2.ShowDialog();
      bool flag = true;
      if (!(nullable.GetValueOrDefault() == flag & nullable.HasValue))
        return;
      this.LoadLayout(openFileDialog2.FileName);
    }

    private void OnAppHelper(object sender, ExecutedRoutedEventArgs e)
    {
      AppHelper.CanBringZusiToFront = !AppHelper.CanBringZusiToFront;
      this.Title = AppHelper.CanBringZusiToFront ? MainWindow._defaultTitle : MainWindow._defaultTitle + " (*)";
    }

    private void OnBack(object sender, ExecutedRoutedEventArgs e)
    {
      this.SelectLayout = true;
      Task.Run((Action) (() =>
      {
        this.DisconnectFahrpult();
        this.Dispatcher.Invoke((Action) (() => this.ClearLayout()));
      }));
      this._willIPBoardSwitchVisible = this.IsIPBoardSwitchVisible = Settings.Default.ZusiConfiguration == 2;
      this._trackWindow = false;
      this.Topmost = false;
    }

    private void BeaconReceiver_BeaconSignalReceived(object sender, BeaconSignalReceivedEventArgs e)
    {
      string[] strArray = e.SignalData.Split('/');
      int result;
      if (strArray.Length != 2 || !(strArray[0] == "ZusiSim") || !int.TryParse(strArray[1], out result))
        return;
      e.RemoteEndPoint = new IPEndPoint(e.Source, result);
      e.IsValid = true;
      this.Dispatcher.Invoke((Action) (() =>
      {
        this._beaconDetected = true;
        if (this.ZusiConfiguration != ZusiConfigurationMode.AutoDetect)
          return;
        this.MissingConfiguration = false;
      }));
    }

    private void OnPause(object sender, ExecutedRoutedEventArgs e)
    {
      this._fahrpult.SendControlCommand(ControlCommand.Pause, (object) ControlCommandValue.Toggle);
      AppHelper.BringZusiToFront();
    }

    private void OnTimejump(object sender, ExecutedRoutedEventArgs e)
    {
      this._fahrpult.SendControlCommand(ControlCommand.TimeJump, (object) ControlCommandValue.Toggle);
      AppHelper.BringZusiToFront();
    }

    private void OnTimelapse(object sender, ExecutedRoutedEventArgs e)
    {
      this._fahrpult.SendControlCommand(ControlCommand.TimeLapse, (object) ControlCommandValue.Toggle);
      AppHelper.BringZusiToFront();
    }

    private void OnSendToAutoStart(object sender, ExecutedRoutedEventArgs e)
    {
            string msg = "Wollen Sie dieses Layout in ZUSI AutoStart eintragen?";
            
            switch (System.Windows.MessageBox.Show(msg, "Nachfrage", MessageBoxButton.YesNo))
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.Yes:
                    string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;
                    Zusiaccess.CreateZUSIAutoStartEntry(executablePath,_curentlayoutfile);

                    return;
                    
                case MessageBoxResult.No:
                    return;
            }
        }

    private void OnOpenIPConnConf(object sender, ExecutedRoutedEventArgs e)
    {
        
            this._timerGracePeriod.Stop();
            this.cbxIPBoard.IsChecked = new bool?(false);
            this.IsZusiMelderConfVisible = true;
            this._timerZusiMelderConf.Start();

            return;

           
    }

        private void OnCanMinimize(object sender, CanExecuteRoutedEventArgs e)
    {
      e.CanExecute = this.WindowState != WindowState.Minimized;
    }

    private void OnMinimize(object sender, ExecutedRoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void LayoutItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
      MainWindow.CommandLoadLayout.Execute((object) null, (IInputElement) null);
    }

    private async void DataProcessor()
    {
      await Task.Run((Action) (() =>
      {
        while (!this._dataCancellation)
        {
          this._dataWakeUp.WaitOne();
          if (this._dataCancellation)
            break;
          this.Dispatcher.Invoke((Action) (() =>
          {
            EventArgs result;
            while (this._dataQueue.TryDequeue(out result))
            {
              FtdDataReceivedEventArgs f = result as FtdDataReceivedEventArgs;
              if (f != null)
              {
                this._gauges.ForEach((Action<IGauge>) (g => g.SetFtdData(f)));
              }
              else
              {
                ProgDataReceivedEventArgs p = result as ProgDataReceivedEventArgs;
                if (p != null)
                  this._gauges.ForEach((Action<IGauge>) (g => g.SetProgData(p)));
              }
            }
          }));
        }
      }));
    }

    private void HideIPBoard() => this.cbxIPBoard.IsChecked = new bool?(false);

    private void LoadLayout(string layoutFileName)
    {
      this._willIPBoardSwitchVisible = this.IsIPBoardSwitchVisible = false;
      this.DisconnectFahrpult();
      this.SelectLayout = false;
      this._curentlayoutfile = layoutFileName;
      if (!this.MissingConfiguration)
        this._beaconReceiver.ShutDown();
      this.ClearLayout();
      ZMLFile zmlFile = new ZMLFile(layoutFileName);
      zmlFile.ParseCompleted += (EventHandler) ((s, e) =>
      {
        this.CreateFahrpult();
        this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action (() =>
        {
          this._gaugeIds.Clear();
          this._gaugeProgIds.Clear();
          foreach (IGauge gauge in this._gauges)
          {
            if (gauge.FtdID != ZFtdID.None)
              this._gaugeIds.Add(gauge.FtdID);
            if (gauge.GetAdditionalIDs().Count<ZFtdID>() > 0)
            {
              foreach (ZFtdID additionalId in gauge.GetAdditionalIDs())
                this._gaugeIds.Add(additionalId);
            }
            if (gauge.GetProgIDs().Count<ZProgID>() > 0)
            {
              foreach (ZProgID progId in gauge.GetProgIDs())
                this._gaugeProgIds.Add(progId);
            }
          }
          this.ConnectFahrpult();
          if (WindowPosition.IsValid)
          {
            this.Left = WindowPosition.Left;
            this.Top = WindowPosition.Top;
            this.Topmost = WindowPosition.Topmost;
            MainWindow._log.Debug((object) string.Format("window pos applied: {0} --> {1} | {2} --> {3} | {4} --> {5}", (object) WindowPosition.Left, (object) this.Left, (object) WindowPosition.Top, (object) this.Top, (object) WindowPosition.Topmost, (object) this.Topmost));
          }
          else
            MainWindow._log.Debug((object) "no window pos applied");
          this._trackWindow = true;
        }));
      });
      zmlFile.ParseDocument += (ZMLReadDocumentEventHandler) ((s, e) =>
      {
        float version = XElementEx.GetAttrValue(e.Layout, (XName) "version", 0.0f);
        XElement x = e.Layout.Element((XName) "Background");
        if (x != null)
          this._background.Initialize(x);
        else
          this._background.BackgroundMode = XElementEx.GetAttrValue(e.Layout, (XName) "displayMode", "").ToLower() == "text" ? BackgroundMode.Text : BackgroundMode.Burlwood;
        if (this._background.BackgroundMode == BackgroundMode.Text)
        {
          this._colWidth = 75.0;
          this._rowHeight = 20.0;
          version = 1.1f;
        }
        else
        {
          this._colWidth = 50.0;
          this._rowHeight = 50.0;
        }
        this.Zoom = (double) XElementEx.GetAttrValue(e.Layout, (XName) "zoom", 1f);
        foreach (XElement element in e.Layout.Elements())
        {
          if (!(element.Name.LocalName == "Background"))
            this.InsertGauge(GaugeTemplateFactory.CreateTemplate(e.Namespace, element, version));
        }
      });
      zmlFile.Parse();
    }

    private void ClearLayout()
    {
      this._gauges.Clear();
      this.layoutGrid.Children.Clear();
      this.layoutGrid.RowDefinitions.Clear();
      this.layoutGrid.ColumnDefinitions.Clear();
    }

    private void InsertGauge(IGaugeTemplate gt)
    {
      if (!(gt is DependencyObject))
        return;
      int column = gt.Column;
      int row = gt.Row;
      int sizeX = gt.SizeX;
      int sizeY = gt.SizeY;
      this.PrepareGrid(column, row, sizeX, sizeY);
      if (!(gt.GetGauge() is Control gauge))
        return;
      Grid.SetColumn((UIElement) gauge, column);
      Grid.SetColumnSpan((UIElement) gauge, sizeX);
      Grid.SetRow((UIElement) gauge, row);
      Grid.SetRowSpan((UIElement) gauge, sizeY);
      this.layoutGrid.Children.Add((UIElement) gauge);
    }

    private void PrepareGrid(int col, int row, int sizeX, int sizeY)
    {
      int num1 = col + sizeX;
      GridLength gridLength1 = new GridLength(this._colWidth);
      while (this.layoutGrid.ColumnDefinitions.Count < num1)
        this.layoutGrid.ColumnDefinitions.Add(new ColumnDefinition()
        {
          Width = gridLength1
        });
      int num2 = row + sizeY;
      GridLength gridLength2 = new GridLength(this._rowHeight);
      while (this.layoutGrid.RowDefinitions.Count < num2)
        this.layoutGrid.RowDefinitions.Add(new RowDefinition()
        {
          Height = gridLength2
        });
    }

    private void ObtainLayoutFiles()
    {
      //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "ZusiMeterLayouts");
      string path = Path.Combine(GetZusiMeterLayoutFileDir());

      if (!Directory.Exists(path))
        return;
      foreach (string enumerateFile in Directory.EnumerateFiles(path, "*.zmlf", SearchOption.TopDirectoryOnly))
      {
        if (!enumerateFile.Contains<char>('~'))
          this._layoutFiles.Add(enumerateFile);
      }
    }

    private void ConnectFahrpult()
    {
      this._fahrpult.SetNeededData(this._gaugeIds.Distinct<ZFtdID>());
      this._fahrpult.SetNeededData(this._gaugeProgIds.Distinct<ZProgID>());
      if (Settings.Default.ZusiConfiguration == 1)
        this._fahrpult.OpenAsync();
      else if (this.Port <= 0)
        this._fahrpult.OpenAsync(this.Host);
      else
        this._fahrpult.OpenAsync(this.Host, this.Port);
    }

    private void CreateFahrpult()
    {
      this._fahrpult = new FahrpultClient(ParsingMode.Internal, AsmInfo.Product, VersionEx.ToString(AsmInfo.Version, "%M.%m.%b"))
      {
        LogSocketExceptions = true,
        WaitLoopTime = 4000
      };
      this._fahrpult.ClientConnected += new ClientConnectedEventHandler(this.Fahrpult_ClientConnected);
      this._fahrpult.Disconnected += new EventHandler(this.Fahrpult_Disconnected);
      this._fahrpult.FtdDataReceived += new FtdDataReceivedEventHandler(this.Fahrpult_FtdDataReceived);
      this._fahrpult.ProgDataReceived += new ProgDataReceivedEventHandler(this.Fahrpult_ProgDataReceived);
    }

    private void DisconnectFahrpult()
    {
      if (this._fahrpult == null)
        return;
      this._fahrpult.ClientConnected -= new ClientConnectedEventHandler(this.Fahrpult_ClientConnected);
      this._fahrpult.Disconnected -= new EventHandler(this.Fahrpult_Disconnected);
      this._fahrpult.FtdDataReceived -= new FtdDataReceivedEventHandler(this.Fahrpult_FtdDataReceived);
      this._fahrpult.ProgDataReceived -= new ProgDataReceivedEventHandler(this.Fahrpult_ProgDataReceived);
      try
      {
        this._fahrpult.Dispose();
      }
      catch
      {
      }
      finally
      {
        this._fahrpult = (FahrpultClient) null;
        this.Fahrpult_Disconnected((object) null, EventArgs.Empty);
      }
    }

    private void ReconnectFahrpult()
    {
      this.DisconnectFahrpult();
      this.CreateFahrpult();
      this.ConnectFahrpult();
      this._mustReconnect = false;
    }

    private void CheckConfiguration()
    {
      switch (this.ZusiConfiguration)
      {
        case ZusiConfigurationMode.LocalHost:
          this.MissingConfiguration = false;
          break;
        case ZusiConfigurationMode.AutoDetect:
          this.MissingConfiguration = this.SelectLayout && !this._beaconDetected;
          break;
        case ZusiConfigurationMode.Manual:
          this.MissingConfiguration = this.SelectLayout && string.IsNullOrEmpty(this.Host);
          break;
      }
  }

   

    
  }
}
