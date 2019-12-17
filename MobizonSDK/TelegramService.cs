using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MobizonSDK
{
    public class SendMessageRequest
    {
        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class TelegramService
    {
        public async Task<string> ExecuteRequest(SendMessageRequest request)
        {
            using(var httpClient = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage();
                httpRequest.RequestUri = new Uri("https://api.telegram.org/bot123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11/getMe");
                httpRequest.Method = HttpMethod.Post;
                httpRequest.Headers.Add("Distributed-Transaction-Id", Guid.NewGuid().ToString());

                var json = JsonConvert.SerializeObject(request);
                string xml = "<tag>x</tag>";
                var stringContent = new StringContent(xml, Encoding.UTF8, "application/xml");

                httpRequest.Content = stringContent;

                var result = await httpClient.SendAsync(httpRequest);

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}
