using Bot46.API.Infrastructure.API;
using Bot46.API.Infrastructure.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot46.API.Infrastructure.Services
{

    public class CatalogService : ICatalogService
    {
        private readonly BotSettings _settings;
        private readonly IHttpClient _apiClient;

        private readonly string _remoteServiceBaseUrl;

        public CatalogService(BotSettings settings, IHttpClient httpClient)
        {
            _settings = settings;
            _apiClient = httpClient;

            _remoteServiceBaseUrl = $"{_settings.CatalogUrl}/api/v1/catalog/";
        }

        public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
        {
            var allcatalogItemsUri = CatalogAPI.GetAllCatalogItems(_remoteServiceBaseUrl, page, take, brand, type);

            var dataString = await _apiClient.GetStringAsync(allcatalogItemsUri);

            var response = JsonConvert.DeserializeObject<Catalog>(dataString);

            return response;
        }

        public async Task<IEnumerable<Brand>> GetBrands()
        {
            var getBrandsUri = CatalogAPI.GetAllBrands(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getBrandsUri);

            var items = new List<Brand>();
            items.Add(new Brand() { Id = "-1", Text = "All", IsSelected= true});

            var brands = JArray.Parse(dataString);

            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new Brand()
                {
                    Id = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }

            return items;
        }

        public async Task<IEnumerable<CatalogType>> GetTypes()
        {
            var getTypesUri = CatalogAPI.GetAllTypes(_remoteServiceBaseUrl);

            var dataString = await _apiClient.GetStringAsync(getTypesUri);

            var items = new List<CatalogType>();
            items.Add(new CatalogType() { Id = "-1", Text = "All", IsSelected= true});

            var brands = JArray.Parse(dataString);
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new CatalogType()
                {
                    Id = brand.Value<string>("id"),
                    Text = brand.Value<string>("type")
                });
            }
            return items;
        }
    }
}
