using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LocalEats;
using LocalEats.Data;
using LocalEats.Model;
using System.IO;

namespace ExampleLEApp
{
    class ExampleProgram
    {
        // The LE client is a thread safe client object.
        private static LEClient _leClient = new LEClient();

        static void Main(string[] args)
        {
            // The LE API is very location centric.  Meaning, you first have to establish
            // a "current location".  That can be a city or a latitude longitude (geo location).
            
            // First lets get the states list filter it to Tennessee.
            var tn = _leClient.GetStatesList().Result.First(si => si.Name == "Tennessee");

            // This just shows that by default states don't have their cities loaded.
            // So check to see if they are loaded (note this purely academic this check is not
            // necessary since we just loaded them).
            if (tn.AreCitiesLoaded == false)
            {
                // The call returns the city list but ALSO fills the state with it's 
                // cities.
                _leClient.GetCityList(tn).Wait();
            }

            // Filter to nashville.
            var nashville = tn.Cities.First(ci => ci.Name == "Nashville");

            // Now save all the best of hot chicken restaurants in Nashville... mmmm... hot chicken.
            ExampleSaveBestHotChickenRestaurantsIn(nashville).Wait();
        }

        static async Task ExampleSaveBestHotChickenRestaurantsIn(CityInfo city)
        {
            // Get just categories with best of winners
            var categories = _leClient.GetCategoryInfoListFor(city, FilterOptions.NewWith(FilterOptions.BEST_OF_WINNER)).Result;

            // Get hot chicken category
            var hotChicken = categories.First(ci => ci.Name == "Hot Chicken");

            // create filtering option
            // NOTE: filtering in the LE api is not very fine grained.  For each option you add you are saying, for example,
            // "Return restaurants that has food of the given category and return restaurants that are a 'best of' winner in A category".
            // 
            // So to translate to something SQL-iSH...
            // Ideally you would want something like SELECT * FROM Restaurant r WHERE r.BestOfCategory = 'Hot Chicken'
            // but what LE API does is SELECT * FROM Restaurant r WHERE 'Hot Chicken' IN (r.Categorys) OR r.HasABestOfWinnerAward = TRUE


            // This is going to return ALL restaurants that server hot chicken
            var filterOptions = new FilterOptions(FilterOptions.CATEGORIES.AddValue(hotChicken.Id));

            // Because the results includes more than just restaurants that are "BEST OF IN HOT CHICKEN" we have to filter it
            // after we get the results
            var hotChickenPlaces = await _leClient.GetRestaurantListNearBy(filterOptions, orCity: city);

            // Filter out the restaurants where the have a category they are best of in and the category is hot chicken
            var bestHotChickenPlaces = hotChickenPlaces.Where(ri => ri.CategoryList.Any(ci => ci.IsBestOf && ci.Name == "Hot Chicken"));

            await SaveToDatabase(bestHotChickenPlaces);
        }

        static async Task SaveToDatabase(IEnumerable<RestaurantInfo> restaurants)
        {
            foreach(var restaurant in restaurants)
            {
                var restaurantToSave = restaurant;

                // Does the restaurant have it's description loaded?
                if (restaurantToSave.IsFullyLoaded == false)
                {
                    // By default the restaurant does not have a description loaded.
                    // Reason is the description the restaurants can be large depending
                    // on popularity of the restaurant.

                    // This call loads the description and returns the update restaurant with
                    // the description loaded.
                    restaurantToSave = await _leClient.GetRestaurantDescription(restaurantToSave);
                }

                // First save the restaurants image to file system.
                // Once on the filesystem you can store it as blob to db or
                // move to file server or media storage. 
                var restaurantFileName = String.Format("{0}_{1}.jpg", restaurantToSave.Name, restaurantToSave.LocId);
                var pathToRestaurantImage = String.Format("C:\\Temp\\{0}", CleanFileName(restaurantFileName));

                restaurantToSave.SaveRestaurantImageToFile(pathToRestaurantImage);

                // Here you would save the data in the restaurant object to your database.
                
            }
        }

        public static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
