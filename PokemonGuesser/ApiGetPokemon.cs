using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokeGuesser
{
    class ApiGetPokemon
    {
        public static async Task<Pokemon> GetPokemon(string mon)
        {
            string urlMon = "https://pokeapi.co/api/v2/pokemon/" + mon;

            using (HttpResponseMessage response = await ApiHelper.ApiWebClient.GetAsync(urlMon))
            {
                if (response.IsSuccessStatusCode)
                {
                    Pokemon pokemon = await response.Content.ReadAsAsync<Pokemon>();

                    pokemon.PokemonSpecies = await GetPokemonSpecies(pokemon);

                    return pokemon;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public static async Task<PokemonSpecies> GetPokemonSpecies(Pokemon mon)
        {
            string urlMon = "https://pokeapi.co/api/v2/pokemon-species/" + mon.Id;

            using (HttpResponseMessage response = await ApiHelper.ApiWebClient.GetAsync(urlMon))
            {
                if (response.IsSuccessStatusCode)
                {
                    PokemonSpecies result = await response.Content.ReadAsAsync<PokemonSpecies>();

                    return result;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        //public static async Task<Version> GetPokemonVersion(Uri uri)
        //{
        //    using (HttpResponseMessage response = await ApiHelper.ApiWebClient.GetAsync(uri))
        //    {
        //        if (response.IsSuccessStatusCode)
        //        {
        //            Version result = await response.Content.ReadAsAsync<Version>();

        //            return result;
        //        }
        //        else
        //        {
        //            throw new Exception(response.ReasonPhrase);
        //        }
        //    }
        //}


    }
}
