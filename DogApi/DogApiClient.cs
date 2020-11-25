using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DogApi
{
    /// <summary>
    /// API for interacting with the Dog API
    /// </summary>
    public class DogApiClient
    {
        private readonly HttpClient client;
        private readonly string apiKey;

        public DogApiClient(HttpClient client, string apiKey, string baseUrl = "https://api.thedogapi.com/v1/")
        {
            this.client = client;
            client.BaseAddress = new Uri(baseUrl);
            this.apiKey = apiKey;
        }

        /// <summary>
        /// Search for public images
        /// </summary>
        /// <param name="size">Size to filter to</param>
        /// <param name="breedsOnly">Filter only to images with tagged breeds</param>
        /// <param name="page">Page to skip to</param>
        /// <param name="limit">Size of page</param>
        /// <param name="sortMode">Mode to sort by</param>
        public Task<IEnumerable<ImageSearchResult>> SearchImagesAsync(ImageSize? size = default, bool breedsOnly = true, int page = 0, int limit = 1, SortMode sortMode = SortMode.Random)
        {
            var url = "images/search";
            var query = HttpUtility.ParseQueryString(url);
            if (size.HasValue)
                query["size"] = size.Value.ToString();
            if (breedsOnly)
                query["has_breeds"] = true.ToString();
            if (sortMode != SortMode.Random)
                query["order"] = sortMode == SortMode.Ascending ? "ASC" : "DESC";
            query["page"] = page.ToString();
            query["limit"] = limit.ToString();

            var builder = new UriBuilder(client.BaseAddress + url);
            builder.Query = query.ToString();

            return GetAsync<IEnumerable<ImageSearchResult>>(builder.Uri);
        }

        /// <summary>
        /// Get a specific image
        /// </summary>
        /// <param name="id">ID of the image</param>
        public Task<ImageSearchResult> GetImageAsync(string id) => GetAsync<ImageSearchResult>(FormatUrl($"images/{id}"));

        /// <summary>
        /// Get images that you have uploaded
        /// </summary>
        /// <param name="page">Page to skip to</param>
        /// <param name="limit">Images per page</param>
        /// <param name="sortMode">Sorting method for images</param>
        public Task<IEnumerable<ImageSearchResult>> GetUploadedImagesAsync(int page = 0, int limit = 10, SortMode sortMode = SortMode.Ascending)
        {
            var url = "images";
            var query = HttpUtility.ParseQueryString(url);
            if (sortMode != SortMode.Random)
                query["order"] = sortMode == SortMode.Ascending ? "ASC" : "DESC";
            query["page"] = page.ToString();
            query["limit"] = limit.ToString();

            var builder = new UriBuilder(client.BaseAddress + url);
            builder.Query = query.ToString();

            return GetAsync<IEnumerable<ImageSearchResult>>(builder.Uri);
        }

        /// <summary>
        /// Upload an image to your account
        /// </summary>
        /// <param name="imageStream">Stream containing info for image</param>
        /// <param name="fileName">Filename to assign to image</param>
        /// <param name="subId">Unique ID for your reference</param>
        /// <param name="breedIds">List of breed IDs to tag in image</param>
        public async Task UploadImageAsync(Stream imageStream, string fileName, string subId = default, IEnumerable<int> breedIds = default)
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StreamContent(imageStream), "file", fileName);
                if (!string.IsNullOrEmpty(subId))
                    content.Add(new StringContent(subId), "sub_id");
                if (breedIds != default && breedIds.Any())
                    content.Add(new StringContent(string.Join(",", breedIds.Select(i => i.ToString()))), "breed_ids");

                using (var message = await client.PostAsync("images/upload", content))
                {
                    if (!message.IsSuccessStatusCode)
                        throw new DogException(message.StatusCode, await message.Content.ReadAsStringAsync());
                }
            }
        }

        /// <summary>
        /// Delete an image from your account
        /// </summary>
        /// <param name="id">ID of the image</param>
        public Task DeleteImageAsync(string id) => DeleteAsync(FormatUrl($"images/{id}"));

        /// <summary>
        /// Get the breeds that have been tagged in an image
        /// </summary>
        /// <param name="imageId">ID of the image</param>
        public Task<IEnumerable<Breed>> GetBreedsAsync(string imageId) => GetAsync<IEnumerable<Breed>>(FormatUrl($"images/{imageId}/breeds"));

        /// <summary>
        /// Get the breeds that have been tagged in an image
        /// </summary>
        /// <param name="image">Image to get breeds for</param>
        public Task<IEnumerable<Breed>> GetBreedsAsync(ImageSearchResult image) => GetBreedsAsync(image.Id);

        /// <summary>
        /// Tag a breed on an image
        /// </summary>
        /// <param name="imageId">ID of image to tag</param>
        /// <param name="breedId">ID of breed to tag</param>
        public Task AddBreedAsync(string imageId, int breedId) => PostAsync(FormatUrl($"image/{imageId}/breeds"), new ImageTagDto(breedId));

        /// <summary>
        /// Tag a breed on an image
        /// </summary>
        /// <param name="image">Image to tag</param>
        /// <param name="breedId">ID of breed to tag</param>
        public Task AddBreedAsync(ImageSearchResult image, int breedId) => AddBreedAsync(image.Id, breedId);

        /// <summary>
        /// Tag a breed on an image
        /// </summary>
        /// <param name="imageId">ID of image to tag</param>
        /// <param name="breed">Breed to tag</param>
        public Task AddBreedAsync(string imageId, Breed breed) => AddBreedAsync(imageId, breed.Id);

        /// <summary>
        /// Tag a breed on an image
        /// </summary>
        /// <param name="image">Image to tag</param>
        /// <param name="breed">Breed to tag</param>
        public Task AddBreedAsync(ImageSearchResult image, Breed breed) => AddBreedAsync(image.Id, breed.Id);

        /// <summary>
        /// Remove a tagged breed from an image
        /// </summary>
        /// <param name="imageId">ID of the image</param>
        /// <param name="breedId">ID of the breed</param>
        public Task RemoveBreedAsync(string imageId, int breedId) => DeleteAsync(FormatUrl($"image/{imageId}/breeds/{breedId}"));

        /// <summary>
        /// Remove a tagged breed from an image
        /// </summary>
        public Task RemoveBreedAsync(ImageSearchResult image, int breedId) => RemoveBreedAsync(image.Id, breedId);

        /// <summary>
        /// Remove a tagged breed from an image
        /// </summary>
        public Task RemoveBreedAsync(string imageId, Breed breed) => RemoveBreedAsync(imageId, breed.Id);

        /// <summary>
        /// Remove a tagged breed from an image
        /// </summary>
        public Task RemoveBreedAsync(ImageSearchResult image, Breed breed) => RemoveBreedAsync(image.Id, breed.Id);

        /// <summary>
        /// Get a list of breeds
        /// </summary>
        /// <param name="page">Page to skip to</param>
        /// <param name="limit">Size of page</param>
        /// <returns></returns>
        public Task<IEnumerable<Breed>> GetBreedsAsync(int page = 0, int limit = 10)
        {
            var url = "breeds";
            var query = HttpUtility.ParseQueryString(url);
            query["page"] = page.ToString();
            query["limit"] = limit.ToString();

            var builder = new UriBuilder(client.BaseAddress + url);
            builder.Query = query.ToString();

            return GetAsync<IEnumerable<Breed>>(builder.Uri);
        }

        /// <summary>
        /// Get a breed
        /// </summary>
        /// <param name="breedId">ID of the breed</param>
        public Task<Breed> GetBreedAsync(int breedId) => GetAsync<Breed>(FormatUrl($"breeds/{breedId}"));

        private async Task DeleteAsync(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, url))
            {
                request.Headers.Add("x-api-key", apiKey);
                using (var res = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        throw new DogException(res.StatusCode, content);
                    }
                }
            }
        }

        private static string FormatUrl(FormattableString url) => string.Format(url.Format, url.GetArguments().Select(arg => HttpUtility.UrlEncode(arg.ToString())).ToArray());

        private async Task PostAsync<TContent>(string url, TContent payload)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Add("x-api-key", apiKey);
                request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                using (var res = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        throw new DogException(res.StatusCode, content);
                    }
                }
            }
        }

        private Task<TResult> GetAsync<TResult>(Uri url) => GetAsync<TResult>(url.ToString());

        private async Task<TResult> GetAsync<TResult>(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("x-api-key", apiKey);
                using (var res = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (!res.IsSuccessStatusCode)
                    {
                        var content = await res.Content.ReadAsStringAsync();
                        throw new DogException(res.StatusCode, content);
                    }
                    return Deserialize<TResult>(await res.Content.ReadAsStreamAsync());
                }
            }
        }

        private static TResult Deserialize<TResult>(Stream stream)
        {
            if (stream == default || !stream.CanRead)
                return default;

            using (var sr = new StreamReader(stream))
            using (var jtr = new JsonTextReader(sr))
            {
                var js = new JsonSerializer();
                return js.Deserialize<TResult>(jtr);
            }
        }

        public enum SortMode
        {
            Ascending,
            Descending,
            Random
        }
    }
}
