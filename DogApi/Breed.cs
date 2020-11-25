using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DogApi
{
    /// <summary>
    /// A breed of dog
    /// </summary>
    public class Breed
    {
        public Weight Weight { get; set; }
        public Height Height { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("bred_for")]
        public string BredFor { get; set; }
        [JsonProperty("breed_group")]
        public string Group { get; set; }
        [JsonProperty("life_span")]
        public string LifeSpan { get; set; }

        public LifeSpan ExpectedLife => new LifeSpan(LifeSpan);

        /// <summary>
        /// List of temperaments, comma-separated
        /// </summary>
        public string Temperament { get; set; }

        public IEnumerable<string> Temperaments => Temperament?.Split(',').Select(s => s.Trim());

        public string Origin { get; set; }
        [JsonProperty("country_code")]
        public string CountryCode { get; set; }
        public string Description { get; set; }
        public string History { get; set; }
    }
}
