using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LocalEats.Model;
using LocalEats.Data;

namespace LocalEats
{
    /// <summary>
    /// Simple client object that wraps the complexities of access the LE JSON API.
    /// 
    /// For it to work you just have to make sure you have your API key saved in the 
    /// LocalEats.Properties.Resources.resx file.
    /// </summary>
    public class LEClient
    {
        // Repository object that handles caching of calls to 
        // LE api.
        private readonly ILERepository _leRepository;

        // Flag to indicate if a await should be configured to 
        // attempt to marshal the continuation back to the original context 
        // captured.  This can be problematic for server application with thread pools.
        private readonly bool _configAwait;

        /// <summary>
        /// Create a new LocalEats API Client Object.
        /// </summary>
        /// <param name="isServerApp">True if this client is being used in a server application.</param>
        public LEClient(bool isServerApp = false)
        {
            this._leRepository = new LERepository();

            // If you using the LE API in a ASP.NET application you want to pass true in.
            this._configAwait = !isServerApp;
        }

        /// <summary>
        /// Return the list of states that LocalEats supports.
        /// </summary>
        /// <returns>States list</returns>
        public async Task<IEnumerable<StateInfo>> GetStatesList()
        {
            return await this._leRepository.GetStates().ConfigureAwait(_configAwait);
        }

        /// <summary>
        /// Return the list of cities that LocalEats supports in the given state.
        /// </summary>
        /// <param name="forState">The state to get cities for.</param>
        /// <returns>Cities list</returns>
        public async Task<IEnumerable<CityInfo>> GetCityList(StateInfo forState)
        {
            var state = await this._leRepository.GetCities(forState).ConfigureAwait(_configAwait);

            return state.Cities;
        }

        /// <summary>
        /// Return the list of categories in a city along with the count of restaurants in those categories.
        /// Use the filter options to filter restaurants that are in those categories.
        /// </summary>
        /// <param name="city">The city to get categories for.</param>
        /// <param name="filterOptions">Restaurant filter options</param>
        /// <returns>Category list</returns>
        public async Task<IEnumerable<RestaurantCategoryInfo>> GetCategoryInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            return await this._leRepository.GetCategoryInfoListFor(city, filterOptions).ConfigureAwait(_configAwait);
        }

        /// <summary>
        /// Return the list of neighborhoods in a city along with the count of restaurants in those neighborhoods.
        /// Use the filter options to filter restaurants that are in those neighborhoods.
        /// </summary>
        /// <param name="city">The city to get categories for.</param>
        /// <param name="filterOptions">Restaurant filter options</param>
        /// <returns>Neighborhood list</returns>
        public async Task<IEnumerable<NeighborhoodInfo>> GetNeighborhoodInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            return await this._leRepository.GetNeighborhoodInfoListFor(city, filterOptions).ConfigureAwait(_configAwait);
        }

        /// <summary>
        /// Return the list of amenities in a city along with the count of restaurants in those amenities.
        /// Use the filter options to filter restaurants that are in those amenities.
        /// </summary>
        /// <param name="city">The city to get categories for.</param>
        /// <param name="filterOptions">Restaurant filter options</param>
        /// <returns>Amenity list</returns>
        public async Task<IEnumerable<RestaurantAmenityInfo>> GetAmenityInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            return await this._leRepository.GetAmenityInfoListFor(city, filterOptions).ConfigureAwait(_configAwait);
        }

        /// <summary>
        /// Return a list of restaurants "near by" in a city or geo location with the given
        /// size and skipping the given number of restaurants before returning any restaurants.
        /// Note:  If lat/lon is given then city is ignored and vice versa.
        /// </summary>
        /// <param name="givenSize">The size of the restaurant list expected</param>
        /// <param name="skipping">The restaurants to skip before adding restaurant to list</param>
        /// <param name="lat">Near by latitude</param>
        /// <param name="lon">Near by longitude</param>
        /// <param name="orCity">Near by city</param>
        /// <returns>Restaurant list</returns>
        public async Task<List<RestaurantInfo>> GetRestaurantListNearBy(FilterOptions filterOptions, double? lat = null, double? lon = null, 
                                                                        CityInfo orCity = null, int givenSize = 25, int skipping = 0)
        {
            var orCityId = orCity == null ? null : orCity.Id;
            return await this._leRepository.GetRestaurantListNearBy(filterOptions, lat, lon, orCityId, givenSize, skipping);
        }

        /// <summary>
        /// Given the restaurant load it's description and return the updated version with the description loaded.
        /// </summary>
        /// <param name="restaurant">The restaurant to load the description for</param>
        /// <returns>The updated version of the given restaurant with the description loaded</returns>
        public async Task<RestaurantInfo> GetRestaurantDescription(RestaurantInfo restaurant)
        {
            return await this._leRepository.GetRestaurantDescription(restaurant);
        }
    }
}
