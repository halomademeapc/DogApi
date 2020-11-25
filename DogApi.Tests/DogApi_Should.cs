using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DogApi.Tests
{
    public class DogApi_Should
    {
        private readonly DogApiClient client = new DogApiClient(new HttpClient(), "0ab608ec-154b-4cce-be31-f9dbede04393");

        [Fact]
        public async Task Fetch_Random_Image()
        {
            var random = await client.SearchImagesAsync();
            Assert.Single(random);
        }

        [Fact]
        public async Task Fetch_Single_Image()
        {
            var image = await client.GetImageAsync("MUGiNcu_Z");
            Assert.NotNull(image);
        }

        [Fact]
        public async Task Get_Breeds_For_Image()
        {
            var breeds = await client.GetBreedsAsync("MUGiNcu_Z");
            Assert.NotEmpty(breeds);
        }

        [Fact]
        public async Task Fetch_Breeds()
        {
            var breeds = await client.GetBreedsAsync();
            Assert.NotEmpty(breeds);
        }

        [Fact]
        public async Task Fetch_Breed()
        {
            var breed = await client.GetBreedAsync(6);
            Assert.NotNull(breed);
        }
    }
}
