using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DogApi
{
    public class ImageSearchResult
    {
        public string Id { get; set; }
        public Uri Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [JsonProperty("mime_type")]
        public string MimeType { get; set; }
        public IEnumerable<Breed> Breeds { get; set; }
    }

}
