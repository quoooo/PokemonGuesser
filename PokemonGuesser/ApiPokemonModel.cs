using Newtonsoft.Json;
using System;


namespace PokemonGuesser
{
    public partial class Pokemon
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public PokemonSpecies PokemonSpecies { get; set; }

        [JsonProperty("types")]
        public TypeElement[] Types { get; set; }

        [JsonProperty("abilities")]
        public AbilityClass[] Abilities { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("sprites")]
        public Sprites Sprites { get; set; }
    }

    public partial class PokemonSpecies
    {
        [JsonProperty("color")]
        public NameUri Color { get; set; }

        [JsonProperty("flavor_text_entries")]
        public FlavorTextEntry[] FlavorTextEntries { get; set; }

        [JsonProperty("genera")]
        public Genus[] Genera { get; set; }

        [JsonProperty("generation")]
        public NameUri Generation { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("is_baby")]
        public bool IsBaby { get; set; }

        [JsonProperty("is_legendary")]
        public bool IsLegendary { get; set; }

        [JsonProperty("is_mythical")]
        public bool IsMythical { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("names")]
        public Class[] Names { get; set; }
    }

    public partial class AbilityClass
    {
        [JsonProperty("ability")]
        public NameUri Ability { get; set; }

        [JsonProperty("is_hidden")]
        public bool IsHidden { get; set; }

        [JsonProperty("slot")]
        public int Slot { get; set; }
    }

    public partial class FlavorTextEntry
    {
        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("version")]
        public NameUri Version { get; set; }

        public Version VersionName { get; set; }
    }

    public partial class Genus
    {
        [JsonProperty("genus")]
        public string GenusGenus { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }
    }

    public partial class Version
    {
        [JsonProperty("names")]
        public Name[] Names { get; set; }
    }

    public partial class Name
    {
        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("name")]
        public string NameName { get; set; }
    }

    public partial class NameUri
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Language
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Sprites
    {
        [JsonProperty("back_default")]
        public string BackDefault { get; set; }

        [JsonProperty("back_female")]
        public string BackFemale { get; set; }

        [JsonProperty("back_shiny")]
        public string BackShiny { get; set; }

        [JsonProperty("back_shiny_female")]
        public string BackShinyFemale { get; set; }

        [JsonProperty("front_default")]
        public string FrontDefault { get; set; }

        [JsonProperty("front_female")]
        public string FrontFemale { get; set; }

        [JsonProperty("front_shiny")]
        public string FrontShiny { get; set; }

        [JsonProperty("front_shiny_female")]
        public string FrontShinyFemale { get; set; }

        [JsonProperty("other")]
        public Other Other { get; set; }
    }

    public partial class Other
    {
        [JsonProperty("dream_world")]
        public DreamWorld DreamWorld { get; set; }

        [JsonProperty("official-artwork")]
        public OfficialArtwork OfficialArtwork { get; set; }
    }

    public partial class DreamWorld
    {
        [JsonProperty("front_default")]
        public Uri FrontDefault { get; set; }

        [JsonProperty("front_female")]
        public object FrontFemale { get; set; }
    }

    public partial class OfficialArtwork
    {
        [JsonProperty("front_default")]
        public Uri FrontDefault { get; set; }
    }

    public partial class TypeElement
    {
        [JsonProperty("slot")]
        public int Slot { get; set; }

        [JsonProperty("type")]
        public NameUri Type { get; set; }
    }

    public partial class Class
    {
        [JsonProperty("language")]
        public NameUri Language { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

}
