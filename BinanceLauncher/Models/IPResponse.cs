using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BinanceLauncher.Models
{
    class IPResponse
    {
        [JsonProperty("status")]
        public string _status { private get; set; }

        public bool Success => _status == "success";

        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        
        [JsonProperty("query")]
        public string IPAddress { get; set; }
    }
}
