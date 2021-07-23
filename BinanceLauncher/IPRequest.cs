using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using BinanceLauncher.Models;
using Newtonsoft.Json;

namespace BinanceLauncher
{
    internal static class IpRequest
    {
        private const string ApiBaseUrl = "http://ip-api.com/json";

        public static async Task<IPResponse> QueryIPInformation()
        {
            if (await CheckInternetConnected()){
                using (HttpClient client = new HttpClient())
                {
                    using (var resp = await client.GetAsync(ApiBaseUrl, HttpCompletionOption.ResponseHeadersRead))
                    {
                        resp.EnsureSuccessStatusCode();
                        if(resp.Content is object)
                        {
                            try
                            {
                                var body = await resp.Content.ReadAsStringAsync();
                                return JsonConvert.DeserializeObject<IPResponse>(body);
                            }
                            catch { }
                        }
                    }
                }
            }
            return null;
        }

        public static async Task<bool> CheckInternetConnected()
        {
            using (Ping ping = new Ping())
            { 
                var response = await ping.SendPingAsync("1.1.1.1");
                return response.Status == IPStatus.Success;
            }
        }
    }
}
