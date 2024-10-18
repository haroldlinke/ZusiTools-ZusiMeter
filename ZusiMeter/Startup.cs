using Sovoma;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Zusisuplib;
using System.Drawing.Text;
using Microsoft.Win32;

namespace ZusiMeter
{
  class Startup
  {
    private static readonly string _appGuid = "5DED5276-60FF-419F-B64D-864637E44C6C";

    private static void create_Registry_entry_HKCU()
    {
      // Installation only: add ZusiMeter to ZUSI Menu
      Zusiaccess.CreateZUSIMenuEntry(Bezeichnertext: "ZusiMeter (Layoutauswahl)", Vatermenu: "SpTBXSubmenuItemKonfiguration", MenuIndex: 17);
    }

    private static void create_Registry_entry_HKUS()
    {
      // Installation only: add ZusiMeter to ZUSI Menu
      // add menu entry for all users - needs admin rights
      foreach (var userSid in Registry.Users.GetSubKeyNames())
      {
        CreateZUSIMenuEntryHKUsers(userid: userSid, Bezeichnertext: "ZusiMeter (Layoutauswahl1)", Vatermenu: "SpTBXSubmenuItemKonfiguration", MenuIndex: 17);
      }
    }


    // Registry HKEY_Users for all Users, unfortunately not supported by ZUSI yet
    static public void ZUSI_write_ext_menuval_to_Regkey(string keyVal, int EntryIdx = 0, string BezeichnerSprache = "Deutsch", string Bezeichnertext = "", string Vatermenu = "", int MenuIndex = 5, string Datei = "", string? Parameter = "")
    {
      RegistryKey? key;

      try
      {
        key = Registry.Users.OpenSubKey(keyVal, true);
        if (key != null)
        {
          Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} found");
        }
        else
          Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} NOT found");
        try
        {
          key = Registry.Users.CreateSubKey(keyVal);
          Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} created");
        }
        catch (Exception e)
        {
          Debug.WriteLine($"Error in create_ZUSI_menu_entry {e}");
          return;
        }
      }
      catch
      {
        Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} NOT found");
        try
        {
          key = Registry.Users.CreateSubKey(keyVal);
          Debug.WriteLine($"create_ZUSI_menu_entry key {keyVal} created");
        }
        catch (Exception e)
        {
          Debug.WriteLine($"Error in create_ZUSI_menu_entry {e}");
          return;
        }
      }

      try
      {
        key.SetValue("BezeichnerSprache" + EntryIdx.ToString(), BezeichnerSprache, RegistryValueKind.String);
        key.SetValue("BezeichnerText" + EntryIdx.ToString(), Bezeichnertext, RegistryValueKind.String);
        key.SetValue("Vatermenu", Vatermenu, RegistryValueKind.String);
        key.SetValue("MenuIndex", MenuIndex, RegistryValueKind.DWord);
        key.SetValue("Datei", Datei, RegistryValueKind.String);
        if (!string.IsNullOrEmpty(Parameter))
          key.SetValue("Parameter", Parameter, RegistryValueKind.String);
        Debug.WriteLine($"create_ZUSI_menu_entry added key data for Fahrplanerstellung {keyVal}");
      }
      catch (Exception e)
      {
        Debug.WriteLine($"Error in create_ZUSI_menu_entry_2 {e}");
      }
      finally
      {
        key?.Close();
      }
    }

    public static void CreateZUSIMenuEntryHKUsers(string userid = "", string BezeichnerSprache = "Deutsch", string Bezeichnertext = "", string Vatermenu = "", int MenuIndex = 5, string? Params = "")
    {
      bool nozusifound = false;
      bool zusisteamfound = false;
      bool zusi3found = false;
      string keyval = userid + "\\Software\\Zusi3\\Fahrsim\\Einstellungen";
      string text2 = Process.GetCurrentProcess().MainModule?.FileName;
      if (text2 == null)
      {
        return;
      }
      zusi3found = false;
      zusisteamfound = false;
      nozusifound =  false;

      try  // check for Zusi3 entry
      {
        using (Registry.Users.OpenSubKey(keyval, writable: true))
        {
          Debug.WriteLine("create_ZUSI_menu_entry key " + keyval + " found");
          zusi3found = true;
          nozusifound = true;
        }
      }
      catch
      {
        zusi3found = false;
        nozusifound = false;
      }

      if (!zusi3found)
      {
        try
        {
          keyval = userid + "\\Software\\Zusi3\\Fahrsimsteam\\Einstellungen";
          using (Registry.Users.OpenSubKey(keyval, writable: true))
          {
            Debug.WriteLine("create_ZUSI_menu_entry key " + keyval + " found");
            zusisteamfound = true;
            nozusifound = false;
          }
        }
        catch
        {
          zusisteamfound = false;
          nozusifound = true;
        }
      }

      if (nozusifound)
      {
        Debug.WriteLine("create_ZUSI_menu_entry no ZUSI entry found");
        return;
      }

      
      string menu_keyVal = ((!zusisteamfound) ? (userid+"\\SOFTWARE\\Zusi3\\Fahrsim\\Einstellungen\\Menu" + Bezeichnertext) : (userid + "\\SOFTWARE\\Zusi3\\Fahrsimsteam\\Einstellungen\\Menu" + Bezeichnertext));
      
      ZUSI_write_ext_menuval_to_Regkey(menu_keyVal, 0, "Deutsch", Bezeichnertext, Vatermenu, MenuIndex, text2, Params);
      
    }

    //---------------------------------------------------------------------
    [STAThread]
    static void Main()
    {
      string[] commandLineArgs = Environment.GetCommandLineArgs();
      bool testflag = false;

      Debug.WriteLine("Start ZusiMeter");
      if ((commandLineArgs.Length == 2 && commandLineArgs[1] == "*Installation*") || testflag)
      {
        // Installation only: add ZusiMeter to ZUSI Menu
        create_Registry_entry_HKCU();
        create_Registry_entry_HKUS(); // if program runs as administrator menu has to be added to all users
      }
      else
      { 
        string? executablePath = Process.GetCurrentProcess().MainModule?.FileName;

        if (executablePath != null)
        {
          Directory.SetCurrentDirectory(Path.GetDirectoryName(executablePath));
        }
        App app = new();
        app.InitializeComponent();
        _ = app.Run();
      }
    }
  }
}
