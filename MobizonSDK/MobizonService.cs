using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MobizonSDK
{
    public enum MobizonSericeResponseFormat
    {
        JSON, XML
    }

    internal static class MobizonResponseCodes
    {
        public const int AUTHORIZATION_ERROR = 8;
    }

    public class MobizonService
    {
        private readonly Uri _urlBase = new Uri("https://api.mobizon.kz/service");
        private readonly int _apiVersion = 1;
        private readonly string _apiKey;
        public MobizonService(string apiKey)
        {
            _apiKey = apiKey;
        }

        private string GetOutputFormatQuery(MobizonSericeResponseFormat format)
        {
            switch (format)
            {
                case MobizonSericeResponseFormat.JSON:
                    return "json";
                case MobizonSericeResponseFormat.XML:
                    return "xml";
                default:
                    throw new InvalidOperationException(nameof(format));
            }
        }

        public async Task SendSmsMessage(string recipient, string text)
        {
            try
            {
                string requestQuery = "Message/SendSmsMessage";

                var query = BuildRequestQuery(requestQuery, MobizonSericeResponseFormat.JSON);

                query += String.Format("&recipient={0}&text={1}", recipient, text);                

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(query);

                    var content = await response.Content.ReadAsStringAsync();

                    var jObject = JObject.Parse(content);

                    var code = Convert.ToInt32(jObject["code"].ToString());

                    if (code == 1)
                    {
                        throw new InvalidOperationException("Что то заполнено не верно");
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async void GetSmsStatus(string[] ids)
        {
            string requestQuery = "Message/GetSMSStatus";

            var query = BuildRequestQuery(requestQuery, MobizonSericeResponseFormat.JSON);
            for (int i = 0; i < ids.Length; i++)
            {
                query += String.Format("&ids%5B{0}%5d={1}", i.ToString(), ids[i]);
            }
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(query);

                var content = await response.Content.ReadAsStringAsync();

                var jObject = JObject.Parse(content);

                var code = Convert.ToInt32(jObject["code"].ToString());
                Console.WriteLine("Error :"+code);
                if (code == 1)
                {
                    throw new InvalidOperationException("Что то заполнено не верно");
                }

            }
        }

        public async Task<decimal> GetCurrentBalance()
        {
            string requestQuery = "user/getownbalance";
            var query = BuildRequestQuery(requestQuery, MobizonSericeResponseFormat.JSON);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(query);

                var contentAsString = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(contentAsString);

                var code = Convert.ToInt32(jObject["code"].ToString());
                if (code == MobizonResponseCodes.AUTHORIZATION_ERROR)
                {
                    throw new InvalidOperationException("Нет доступа к API, обратитесь к системному администратору!");
                }

                var balance = Convert.ToDecimal(jObject["data"]["balance"].ToString(), CultureInfo.InvariantCulture);
                return balance;
            }
        }
        private string BuildRequestQuery(string requestQuery, MobizonSericeResponseFormat format)
        {
            var output = GetOutputFormatQuery(format);

            var requestQueryFormatted = string.Format("{0}/{1}?output={2}&api=v{3}&apiKey={4}",
                _urlBase.ToString(),
                requestQuery,
                output,
                _apiVersion,
                _apiKey);

            return requestQueryFormatted;
        }

    }
}
