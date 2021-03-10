using System.Net.Http;
using System.Net.Http.Headers;

namespace PokemonGuesser
{
    public static class ApiHelper
    {
        public static HttpClient ApiWebClient { get; set; }

        public static void InitializeClient()
        {
            ApiWebClient = new HttpClient();

            ApiWebClient.DefaultRequestHeaders.Accept.Clear();
            ApiWebClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}

