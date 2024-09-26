//#define WITH_WORKINGDIRS

using log4net;
using Microsoft.Win32;
using Sovoma;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ZusiMeter
{
    public enum DataPathType
    {
        Unknown,
        Official,
        OfficialProf,
        DataDir,
#if WITH_WORKINGDIRS
        DataDirProf,
        WorkerDirA,
        WorkerDirB,
        WorkerDirC,
        WorkerDirD
#else
        DataDirProf
#endif
    };

    public class ZusiDataPath : IEnumerable<string>
    {
#if WITH_WORKINGDIRS
        private readonly string[] _dataPath = new string[] { null, null, null, null, null, null, null, null };
#else
        private readonly string[] _dataPath = new string[] { null, null, null, null };
#endif

        public int Length => _dataPath.Length;

        public string this[DataPathType index]
        {
            get => index == DataPathType.Unknown ? null : _dataPath[(int)index - 1];
            internal set
            {
                _dataPath[(int)index - 1] = value?.EnsureTrailingBackslash();
            }
        }

        public string this[int index]
        {
            get => _dataPath[index];
            internal set
            {
                _dataPath[index] = value?.EnsureTrailingBackslash();
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            for (int i = 0; i < _dataPath.Length; i++)
            {
                yield return _dataPath[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class Zusi
    {
        private const string _dataOfficialPathKey = "DatenVerzeichnisOffiziell";
        private const string _dataPathKey = "DatenVerzeichnis";
        private const string _profSuffix = "Prof";
        private const string _steamSuffix = "Steam";
#if WITH_WORKINGDIRS
        private const string _workDirKey = "Arbeitsverzeichnis";
#endif
        private const string _zusiPathKey = "ZusiVerzeichnis";

        private static Zusi _instance = null;
        private static readonly ILog _log = LogManager.GetLogger(typeof(Zusi));

        private readonly FileVersionInfo _fileVersionInfo;
        private string _executable;
        private readonly bool _installed;
        private bool _profVersion;
        private bool _steamVersion;
        private string _zusiPath;
        private string _infrastructureDataPath;

        private readonly ZusiDataPath _dataPath = new();

        public static ZusiDataPath DataPath => Instance._dataPath;
        public static string Executable => Instance._executable;
        public static string InfrastructureDataPath => Instance._infrastructureDataPath;
        public static bool IsInstalled => Instance._installed;
        public static bool IsProfVersion => Instance._profVersion;
        public static bool IsSteamVersion => Instance._steamVersion;
        public static ulong SimVersion => Instance.GetSimVersion();
        public static string ZusiPath => Instance._zusiPath;

        //---------------------------------------------------------------------
        internal static Zusi Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new();
                }
                return _instance;
            }
        }

        //---------------------------------------------------------------------
        private Zusi()
        {
            _installed = ReadRegistry();
            if (_installed)
            {
                try
                {
                    _fileVersionInfo = FileVersionInfo.GetVersionInfo(_executable);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                }
            }
            else
            {
                _log.Fatal("Couldn't fetch Zusi directories.");
            }
        }

        //---------------------------------------------------------------------
        private string GetAbsolutePathOf_impl(string fileName, ref DataPathType dataPath)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (PathHelper.IsPathFullyQualified(fileName))
                {
                    return fileName;
                }
                else
                {
                    if (dataPath == DataPathType.Unknown)
                    {
                        for (int i = _dataPath.Length - 1; i >= 0; i--)
                        {
                            string p = _dataPath[i];
                            if (string.IsNullOrEmpty(p))
                                continue;

                            string path = PathHelper.BuildPath(p, fileName);
                            if (File.Exists(path))
                            {
                                dataPath = (DataPathType)(i + 1);
                                return path;
                            }
                        }
                    }
                    else
                    {
                        return PathHelper.BuildPath(_dataPath[dataPath], fileName);
                    }
                }
            }

            return null;
        }

        //---------------------------------------------------------------------
        private string GetRelativePathOf_impl(string fullPath, ref DataPathType dataPath)
        {
            if (!string.IsNullOrEmpty(fullPath))
            {
                if (dataPath == DataPathType.Unknown)
                {
                    for (int i = _dataPath.Length - 1; i >= 0; i--)
                    {
                        string p = _dataPath[i];
                        if (string.IsNullOrEmpty(p))
                        {
                            continue;
                        }

                        string res = fullPath.StripPrefixPath(p);
                        if (res.Length != fullPath.Length)
                        {
                            dataPath = (DataPathType)(i + 1);
                            return res;
                        }
                    }
                }
                else
                {
                    string res = fullPath.StripPrefixPath(_dataPath[dataPath]);
                    if (res.Length != fullPath.Length)
                    {
                        return res;
                    }
                }
            }

            return null;
        }

        //---------------------------------------------------------------------
        private ulong GetSimVersion()
        {
            return _fileVersionInfo != null ? (ulong)(((long)_fileVersionInfo.ProductMajorPart << 48) | ((long)_fileVersionInfo.ProductMinorPart << 32) | ((long)_fileVersionInfo.ProductBuildPart << 16) | (long)_fileVersionInfo.ProductPrivatePart) : 0;
        }

        //---------------------------------------------------------------------
        private bool ReadRegistry()
        {
            try
            {
                using RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                using RegistryKey k1 = hklm.OpenSubKey(@"Software\Zusi3", false);

#if WITH_WORKINGDIRS
                using RegistryKey hkcu = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
                using RegistryKey k2 = hkcu.OpenSubKey(@"Software\Zusi3\Einstellungen", false);

                Log.Debug($"HKCU\\Software\\Zusi3\\Einstellungen: {k2 != null}");

                if (k1 != null)
                {
                    ReadRegistryValues(k1, k2);
                }
#else
                if (k1 != null)
                {
                    _log.Info("ZUSI Entry found");
                    ReadRegistryValues(k1);
                }
                else {
                    _log.Error("ZUSI Entry not found");
                }  
#endif
            }
            catch (Exception ex)
            {
                _log.Fatal("ReadRegistry" + ex.ToString());
                return false;
            }

            bool res = false;
            if (!string.IsNullOrEmpty(_zusiPath))
            {
                _executable = $"{_zusiPath}ZusiSim.64.exe";
                if (!File.Exists(_executable))
                {
                    _executable = $"{_zusiPath}ZusiSim.exe";
                    _log.Info("ZusiSim.64.exe not found. Switch to ZusiSim.exe");
                }
                res = true;
            }
            else
            {
                _log.Error("ZusiPath not found or ZusiPath is empty");
            }

            return res;
        }

        //    0 DatenVerzeichnisOffiziell || DatenVerzeichnisOffiziellSteam     Installationsverzeichnis
        //    1 DatenVerzeichnisOffiziellProf
        //    2 DatenVerzeichnis || DatenVerzeichnisSteam                       Öffentliche Dokumente
        //    3 DatenVerzeichnisProf
        // 4..7 ArbeitsverzeichnisA..D

        //---------------------------------------------------------------------
#if WITH_WORKINGDIRS
        private void ReadRegistryValues(RegistryKey hklm, RegistryKey hkcu)
#else
        private void ReadRegistryValues(RegistryKey hklm)
#endif
        {
            _steamVersion = false;
            string suffix = string.Empty;
            string name = _zusiPathKey;
            string s = hklm.GetValue(name) as string;
            if (string.IsNullOrEmpty(s))
            {
                _log.Debug("usual ZusiVerzeichnis not found, trying Steam-Version ...");
                // Steam-Version versuchen
                suffix = _steamSuffix;
                name += suffix;
                s = hklm.GetValue(name) as string;
                if (string.IsNullOrEmpty(s))
                {
                    // Zusi nicht gefunden, Abbruch
                    _log.Debug("even ZusiVerzeichnisSteam is not there. Giving up :(");
                    return;
                }
                _steamVersion = true;
            }
            _zusiPath = s.EnsureTrailingBackslash();

            name = _dataOfficialPathKey + suffix;
            _dataPath[0] = hklm.GetValue(name) as string;
            _log.Debug($"{name}: '{_dataPath[0]}'");

            if (!_steamVersion)
            {
                name = _dataOfficialPathKey + _profSuffix;
                _dataPath[1] = hklm.GetValue(name) as string;
                _profVersion = !string.IsNullOrEmpty(_dataPath[1]);
            }

            _infrastructureDataPath = $"{(_profVersion ? _dataPath[1] : _dataPath[0])}Timetables\\Deutschland\\Infrastrukturdaten\\";

            name = _dataPathKey + suffix;
            _dataPath[2] = hklm.GetValue(name) as string;
            _log.Debug($"{name}: '{_dataPath[2]}'");

            if (!_steamVersion)
            {
                name = _dataPathKey + _profSuffix;
                _dataPath[3] = hklm.GetValue(name) as string;
            }

            // Achtung: NUR FÜR PROF AUSGEBEN
            if (_profVersion)
            {
                _log.Debug($"{name}: '{_dataPath[3]}'");
            }

#if WITH_WORKINGDIRS
            if (hkcu != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    name = _workDirKey + (char)('A' + i);
                    _dataPath[i + 4] = hkcu.GetValue(name) as string;
                }
            }
#endif
        }

        //---------------------------------------------------------------------
        public static string GetAbsolutePathOf(string fileName, ref DataPathType dataPath)
        {
            return Instance.GetAbsolutePathOf_impl(fileName, ref dataPath);
        }

        //---------------------------------------------------------------------
        public static string GetRelativePathOf(string fullPath, ref DataPathType dataPath)
        {
            return Instance.GetRelativePathOf_impl(fullPath, ref dataPath);
        }
    }
}
