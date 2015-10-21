using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Reflection;
using System.Threading.Tasks;

using LocalEats.Model;
using System.Runtime.Caching;

namespace LocalEats.Data
{
	public class LERepository : ILERepository
	{
		private ILERestaurantDAO _dataAccess;

		private ObjectCache _inMemoryCache = MemoryCache.Default;

		public LERepository ()
		{
			// get a better IoC container and we can inject this... to lazy...
			this._dataAccess = new LERestaurantDAO();
		}

		public async Task<List<StateInfo>> GetStates()
		{
			if (this._inMemoryCache.Contains("GetStates")) {
				return await Task.FromResult<List<StateInfo>> ((List<StateInfo>)this._inMemoryCache["GetStates"]);
			} else {
				var result = await this._dataAccess.GetStates ();

				this._inMemoryCache["GetStates"] = result;

				return result;
			}
		}

		public async Task<StateInfo> GetCities (StateInfo stateInfo)
		{
			if (this._inMemoryCache.Contains(stateInfo.ToString())) {
				return await Task.FromResult<StateInfo> ((StateInfo)this._inMemoryCache[stateInfo.ToString()]);
			} else {
				var result = await this._dataAccess.GetCities (stateInfo);

				this._inMemoryCache[stateInfo.ToString()] = result;

				return result;
			}
		}

		public async Task<List<RestaurantInfo>> GetRestaurantListNearBy(FilterOptions filterOptions, double? lat = null, double? lon = null, string orCityId = null, int givenSize = 25, int skipping = 0)
		{
			var parameters = new Object[]{givenSize, skipping, lat, lon, orCityId, filterOptions };
            var cacheKey = Newtonsoft.Json.JsonConvert.SerializeObject(parameters);

			if (this._inMemoryCache.Contains(cacheKey)) {
				return await Task.FromResult<List<RestaurantInfo>> ((List<RestaurantInfo>)this._inMemoryCache[cacheKey]);
			} else {
				var result = await this._dataAccess.GetRestaurantListNearBy (filterOptions, lat, lon, orCityId, givenSize, skipping);

				this._inMemoryCache[cacheKey] = result;

				return result;
			}
		}

		public async Task<RestaurantInfo> GetRestaurantDescription (RestaurantInfo restaurantInfo)
		{
			if (this._inMemoryCache.Contains(restaurantInfo.ToString())) {
				return await Task.FromResult<RestaurantInfo> ((RestaurantInfo)this._inMemoryCache[restaurantInfo.ToString()]);
			} else {
				var result = await this._dataAccess.GetRestaurantDescription (restaurantInfo);

				this._inMemoryCache[restaurantInfo.ToString()] = result;

				return result;
			}
		}

        public async Task<IEnumerable<RestaurantCategoryInfo>> GetCategoryInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            var cacheKey = (city.ToString() + "GetCategoryInfoListFor");
            if (this._inMemoryCache.Contains(cacheKey))
            {
                return await Task.FromResult<IEnumerable<RestaurantCategoryInfo>>(
                    (IEnumerable<RestaurantCategoryInfo>)this._inMemoryCache[cacheKey]
                );
            }
            else
            {
                var result = await this._dataAccess.GetCategoryInfoListFor(city, filterOptions);

                this._inMemoryCache[cacheKey] = result;

                return result;
            }
        }

        public async Task<IEnumerable<NeighborhoodInfo>> GetNeighborhoodInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            var cacheKey = (city.ToString() + "GetNeighborhoodInfoListFor");
            if (this._inMemoryCache.Contains(cacheKey))
            {
                return await Task.FromResult<IEnumerable<NeighborhoodInfo>>(
                    (IEnumerable<NeighborhoodInfo>)this._inMemoryCache[cacheKey]
                );
            }
            else
            {
                var result = await this._dataAccess.GetNeighborhoodInfoListFor(city, filterOptions);

                this._inMemoryCache[cacheKey] = result;

                return result;
            }
        }

        public async Task<IEnumerable<RestaurantAmenityInfo>> GetAmenityInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            var cacheKey = (city.ToString() + "GetAmenityInfoListFor");
            if (this._inMemoryCache.Contains(cacheKey))
            {
                return await Task.FromResult<IEnumerable<RestaurantAmenityInfo>>(
                    (IEnumerable<RestaurantAmenityInfo>)this._inMemoryCache[cacheKey]
                );
            }
            else
            {
                var result = await this._dataAccess.GetAmenityInfoListFor(city, filterOptions);

                this._inMemoryCache[cacheKey] = result;

                return result;
            }
        }
    }
}

