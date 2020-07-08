using CMoreClient.DTO;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CMoreClient
{
  
    public class CMoreClient
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static async Task<String> SendTrack(List<TrackRequest> track)
        {

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("https://dmore.csir.co.za/dev/WebAPI/api/");
            client.DefaultRequestHeaders.Accept.Clear();

            string body = JsonConvert.SerializeObject(track);
            Console.WriteLine("Request To CMORE IS : "+ body);
            logger.Log(LogLevel.Info, "Sending Data To CMORE");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", "aRq0MUhU5a5hoi0RbQ0qFkS8LMwiwbiX08d1C3RZ/H8="); 
             HttpResponseMessage response = await client.PostAsJsonAsync("Gateway/Batch", track);
            var contents = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            String orderResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response from CMORE: " + orderResponse);
            logger.Log(LogLevel.Info, "Response from CMORE"+ orderResponse);
            return orderResponse;
        }

    }

}
