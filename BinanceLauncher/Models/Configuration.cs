using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BinanceLauncher.Models
{
    public class Configuration
    {

        private static readonly Lazy<Configuration> _lazy =
            new Lazy<Configuration>(() => new Configuration());

        public static Configuration Instance => _lazy.Value;

        public static void RewriteConfiguration(string path = "config.json")
        {
            var json = JsonConvert.SerializeObject(_lazy.Value);
            File.WriteAllText(path, json);
        }

        public static void LoadConfiguration(string path = "config.json")
        {
            if (!File.Exists(path))
            {
                Initialize(path);
                return;
            }
            var json = File.ReadAllText(path);
            JsonConvert.PopulateObject(json, _lazy.Value);
        }

        private static void Initialize(string path)
        {
            _lazy.Value.BinancePath = BinanceHelper.GetBinancePath() ?? "";
            _lazy.Value.AutoLaunch = false;
            _lazy.Value.Country = "Netherlands";
            var json = JsonConvert.SerializeObject(_lazy.Value);
            File.WriteAllText(path, json);
        }

        public string BinancePath { get; set; }
        public string Country { get; set; }
        public bool AutoLaunch { get; set; }


        private Configuration() { }

    }
}
