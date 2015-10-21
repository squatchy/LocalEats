using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LocalEats.Model;

namespace LocalEats.Data
{
	public interface ILERestaurantDAO
	{
		Task<List<StateInfo>> GetStates();
		Task<StateInfo> GetCities (StateInfo stateInfo);
		Task<List<RestaurantInfo>> GetRestaurantListNearBy(FilterOptions filterOptions, double? lat = null, double? lon = null, 
            string orCityId = null, int givenSize = 25, int skipping = 0);
		Task<RestaurantInfo> GetRestaurantDescription (RestaurantInfo restaurantInfo);
        Task<IEnumerable<RestaurantCategoryInfo>> GetCategoryInfoListFor(CityInfo city, FilterOptions filterOptions);
        Task<IEnumerable<NeighborhoodInfo>> GetNeighborhoodInfoListFor(CityInfo city, FilterOptions filterOptions);
        Task<IEnumerable<RestaurantAmenityInfo>> GetAmenityInfoListFor(CityInfo city, FilterOptions filterOptions);
    }
}

