using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BBM.Business.Infractstructure
{
    public static class RequestWrapper
    {
        static HttpClient client = new HttpClient() { Timeout = TimeSpan.FromSeconds(10) };

        private static async Task<string> _sendRequest(string url, HttpMethod method, string data = null)
        {
            var rq = new HttpRequestMessage(method, url);
            //rq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            rq.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (data != null)
                rq.Content = new StringContent(data, Encoding.UTF8, "application/json");

            using (var rsp = await client.SendAsync(rq))
            {
                if (rsp.IsSuccessStatusCode)
                    return await rsp.Content.ReadAsStringAsync();
                else
                    return null;
            }
        }

        public static async Task<T> SendRequest<T>(string url, HttpMethod method, string data = null)
        {
            var content = await _sendRequest(url, method, data);
            if (content == null)
                return default(T);

            var format = "dd/MM/yyyy"; // your datetime format
            var dateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = format };

            return JsonConvert.DeserializeObject<T>(content, dateTimeConverter);
        }

        public static async Task<bool> SendRequest(string url, HttpMethod method, string data = null)
        {
            var content = await _sendRequest(url, method, data);
            if (content != null)
                return true;
            return false;
        }
    }
}
