using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Kpakam.ModClasses.UP
{
    public class UP_API_Call
    {
        private string apiUrl = "https://196.46.20.36:5443/execjson"; // Test URL
       // private string apiUrl = "https://196.46.20.36:5443/execjson"; // Live URL
        public async void CallAPI(string TransactionID, decimal Trans_Amt)
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.SendAsync(CreateRequestMessage(CreateJSONContent(TransactionID, Trans_Amt)));

            if (response.IsSuccessStatusCode)
            {
                // The request was successful. You can process the response content here.
                string jsonResponse = await response.Content.ReadAsStringAsync();
                // Parse the JSON response if needed.
                Console.WriteLine("Response: " + jsonResponse);
            }
            else
            {
                // The request failed. Handle the error here.
                Console.WriteLine("Request failed with status code: " + response.StatusCode);
            }
            httpClient.Dispose();
        }

        private StringContent CreateJSONContent(string TransactionID, decimal Trans_Amt)
        { 
            var requestData = new
            {
                TransactionID = TransactionID,
                Trans_Amt = Trans_Amt
            }; 
            string jsonRequestData = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
            return content; 
        }

        private HttpRequestMessage CreateRequestMessage(StringContent content)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post, // Change the HTTP method as needed (e.g., GET, POST, PUT, etc.)
                RequestUri = new Uri(apiUrl),
                Content = content
            };
            return request;
        }
    }
     
}