using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace BinanceLauncher.Models
{
    public static class BinanceHelper
    {
        public static string GetBinancePath()
        {
            string result = null;
            RegistryKey baseKey;
            if (Environment.Is64BitOperatingSystem)
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            }
            else
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            }

            using (var key = baseKey.OpenSubKey(@"SOFTWARE\Binance\"))
            {
                if (key != null)
                {
                    result = key.GetValue("InstallLocation") as string;
                }
            }

            return Path.Combine(result, "Binance.exe");
        }
    }
}
