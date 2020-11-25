using Newtonsoft.Json;

namespace DogApi
{
    class ImageTagDto
    {
        public ImageTagDto(int breedId)
        {
            BreedId = breedId;
        }

        [JsonProperty("breed_id")]
        public int BreedId { get; set; }
    }
}
