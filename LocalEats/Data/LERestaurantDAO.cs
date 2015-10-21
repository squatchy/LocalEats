using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
using LocalEats.Model;

namespace LocalEats.Data
{
	public class LERestaurantDAO : ILERestaurantDAO
    {
        /// <summary>
        /// Dictionary of name to id mapping.
        /// </summary>
        private static readonly Dictionary<string, string> STATE_LONG_NAMES = new Dictionary<string, string>() {
            #region List
            { "AL","Alabama" },
            { "AK","Alaska" },
            { "AZ","Arizona" },
            { "AR","Arkansas" },
            { "CA","California" },
            { "CO","Colorado" },
            { "CT","Connecticut" },
            { "DE","Delaware" },
            { "DC","Dist. of Columbia" },
            { "FL","Florida" },
            { "GA","Georgia" },
            { "HI","Hawaii" },
            { "ID","Idaho" },
            { "IL","Illinois" },
            { "IN","Indiana" },
            { "IA","Iowa" },
            { "KS","Kansas" },
            { "KY","Kentucky" },
            { "LA","Louisiana" },
            { "ME","Maine" },
            { "MD","Maryland" },
            { "MA","Massachusetts" },
            { "MI","Michigan" },
            { "MN","Minnesota" },
            { "MS","Mississippi" },
            { "MO","Missouri" },
            { "MT","Montana" },
            { "NE","Nebraska" },
            { "NV","Nevada" },
            { "NH","New Hampshire" },
            { "NJ","New Jersey" },
            { "NM","New Mexico" },
            { "NY","New York" },
            { "NC","North Carolina" },
            { "ND","North Dakota" },
            { "OH","Ohio" },
            { "OK","Oklahoma" },
            { "OR","Oregon" },
            { "PA","Pennsylvania" },
            { "RI","Rhode Island" },
            { "SC","South Carolina" },
            { "SD","South Dakota" },
            { "TN","Tennessee" },
            { "TX","Texas" },
            { "UT","Utah" },
            { "VT","Vermont" },
            { "VA","Virginia" },
            { "WA","Washington" },
            { "WV","West Virginia" },
            { "WI","Wisconsin" },
            { "WY","Wyoming" }
            #endregion
        };

        /// <summary>
        /// Return the list of states in the gift card program.
        /// </summary>
        /// <returns>List of states</returns>
        public async Task<List<StateInfo>> GetStates()
        {
            var command = new LECommand("GetAllMajorUSCites");

            //skip=0&top=500&format=json
            command.AddParam("format", "json");
            command.AddParam("top", "100");
            command.AddParam("skip", "0");

            var json = await this.DoGet(command);

            var cnt = (int)json["Count"];

            if (cnt > 0)
            {
                var majorCityArray = json["Cities"];

                return majorCityArray.Select<JToken, StateInfo>(jt =>
                {
                    var stateinfo = new StateInfo();

                    stateinfo.Id = (string)jt["State_ID"];

                    if (STATE_LONG_NAMES.ContainsKey((string)jt["StateShort"]) == false)
                        throw new DataException(String.Format("Unknown state short name. {0}", (string)jt["StateShort"]));

                    stateinfo.Name = STATE_LONG_NAMES[(string)jt["StateShort"]];

                    return stateinfo;
                })
                .OrderBy<StateInfo,string>(r => r.Name)
                .ToList<StateInfo>();
            }
            else
            {
                throw new DataException("No state information available.");
            }
        }

        /// <summary>
        /// Given the state info get the cities of that state and return
        /// the state populated with the cities on the gift card program.
        /// </summary>
        /// <param name="stateInfo">The state to get cities for</param>
        /// <returns>The given state with its cities loaded</returns>
        public async Task<StateInfo> GetCities(StateInfo stateInfo)
        {
            // GetCityListByStateID
            var command = new LECommand("GetCityListByStateID");

            //&top = -Indicates how many cities you want to pull back
            //-&format = -Tell it which format you would like the results in. Options are
            //xml and json
            //- &stateId = -Tell it which state to retrieve cities in by ID.
            command.AddParam("top","500");
            command.AddParam("format", "json");
            command.AddParam("stateId", Convert.ToString(stateInfo.Id));

            var json = await this.DoGet(command);

            var cnt = (int)json["Count"];

            if (cnt > 0)
            {
                var citiesArray = (JArray)json["Cities"];

                var cities = citiesArray.Select<JToken, CityInfo>(jt =>{
                    var city = new CityInfo();

                    city.Id = Convert.ToString((int)jt["D_City_ID"]);
                    city.Name = (string)jt["City_Name"];

                    return city;
                }).ToList<CityInfo>();

                stateInfo.Cities = cities;

                return stateInfo;
            }
            else
            {
                stateInfo.Cities = new List<CityInfo>();

                return stateInfo;
            }
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
        public async Task<List<RestaurantInfo>> GetRestaurantListNearBy(
            FilterOptions filterOptions,
            double? lat = null, double? lon = null, 
            string orCityId = null, int givenSize = 25, int skipping = 0)
        {
            System.Diagnostics.Debug.Assert((lat.HasValue && lon.HasValue) || !String.IsNullOrEmpty(orCityId));

            var command = new LECommand("GetRestaurantListNearBy");

            command.AddParam("format", "json");

            if(String.IsNullOrEmpty(orCityId) == false)
                command.AddParam("cityid", orCityId);

            // this returns the 
            command.AddParam("ginfo", "acdhprstm");
            if (lat.HasValue)
                command.AddParam("lat", Convert.ToString(lat.Value));
            if (lon.HasValue)
                command.AddParam("lon", Convert.ToString(lon.Value));

            command.AddParam("top",Convert.ToString(givenSize));

            command.AddParam("skip",Convert.ToString(skipping));

            if (filterOptions != null)
            {
                foreach (var filterOption in filterOptions.AllOptions)
                {
                    command.AddParam(filterOption.Name, filterOption.Value);
                }
            }

            JObject json = await this.DoGet(command);

            if(json == null)
                return new List<RestaurantInfo>();

            var cnt = (int)json["Count"];

            if (cnt > 0)
            {
                JArray restaurantArray = (JArray)json["Restaurants"];

                return restaurantArray.Select<JToken, RestaurantInfo>(jObj => {

                    RestaurantInfo restaurant = new RestaurantInfo();

                    restaurant.LocId = Convert.ToString((int)jObj["AddyID"]);
                    restaurant.Id = Convert.ToString((int)jObj["RestID"]);
                    restaurant.Name = (string)jObj["RestName"];

                    restaurant.StreetAddress = (string)jObj["Address1"];
                    if ((string)jObj["Address2"] != null)
                    {
                        restaurant.StreetAddress += (Environment.NewLine + (string)jObj["Address2"]);
                    }

                    restaurant.CityStateZip = String.Format("{0}, {1} {2}",
                        (string)jObj["City"], (string)jObj["State"], (string)jObj["Zip"]
                    );

                    restaurant.PhoneNumber = (string)jObj["Phone"];

                    restaurant.PriceRange = string.Empty;
                    int avgCost = (int)jObj["AvgCost"];
                    for (int i = 0; i <= avgCost; i++)
                    {
                        restaurant.PriceRange += '$';//star - '\u2605';
                    }

                    //CatInfoList
                    var catList = (JArray)jObj["CatInfoList"];

                    restaurant.CategoryList = 
                        catList.Select<JToken, RestaurantInfo.CategoryInfo>(jCat => {
                            return new RestaurantInfo.CategoryInfo()
                            {
                                Id = (string)jCat["CatID"],
                                Name = (string)jCat["CatName"],
                                IsBestOf = jCat["IsBestOf"] == null ? false : ((bool)jCat["IsBestOf"])
                            };
                        });

                    // Features/Amenities
                    var featuresList = (JArray)jObj["AmendInfoList"];

                    restaurant.AmenityList =
                        featuresList.Select<JToken, RestaurantInfo.AmenityInfo>(jAmen =>
                        {
                            return new RestaurantInfo.AmenityInfo()
                            {
                                Id = (string)jAmen["AmendityID"],
                                Name = (string)jAmen["AmendityName"],
                                LocId = (string)jAmen["AddyID"]
                            };
                        });

                    //MediaInfoList
                    var mediaInfoList = (JArray)jObj["MediaInfoList"];
                    var mediaInfo = mediaInfoList.FirstOrDefault<JToken>();
                    if (mediaInfo != null)
                    {
                        //http://cdn.localeats.com/media/images/23132-1.jpg?dummy=dummy40
                        restaurant.ImageUri = 
                            String.Format(@"http://cdn.localeats.com/media/images/{0}?dummy=dummy40",
                                          (string)mediaInfo["MediaFileName"]);
                    }

                    if (jObj["DistanceAway"] != null)
                        restaurant.SpatialOffset = (decimal)jObj["DistanceAway"];

                    // Awards
                    if (jObj["AwardInfoList"] != null)
                    {
                        var jAwardList = (JArray)jObj["AwardInfoList"];

                        var awardList = new List<RestaurantInfo.AwardInfo>();

                        foreach (var awardObj in jAwardList)
                        {
                            var awardName = (string)awardObj["AwardName"];

                            // as switch later .. maybe
                            if (((int)awardObj["AwardID"]) == ((int)Award.EditorsPick))
                            { 
                                restaurant.IsEditorsPick = true;

                                // LE now refers to Top 100 as editors pick
                                awardName = "Editors Pick";
                            }
                            
                            awardList.Add(new RestaurantInfo.AwardInfo()
                            {
                                Id = (string)awardObj["AwardID"],
                                Name = awardName
                            });
                        }

                        restaurant.AwardList = awardList;
                    }

                    // Neighborhoods
                    if (jObj["HoodInfoList"] != null)
                    {
                        var jHoodList = (JArray)jObj["HoodInfoList"];

                        restaurant.NeighborhoodList =
                            jHoodList.Select<JToken, RestaurantInfo.NeighborhoodInfo>(jHood =>
                            {
                                return new RestaurantInfo.NeighborhoodInfo() {
                                    Id = (string)jHood["HoodID"],
                                    Name = (string)jHood["HoodName"]
                                };
                            });
                    }

                    // Deals
                    if (jObj["DealInfoList"] != null)
                    {
                        var jDealList = (JArray)jObj["DealInfoList"];

                        restaurant.DealList =
                            jDealList.Select<JToken, RestaurantInfo.DealInfo>(jDeal =>
                             {
                                 return new RestaurantInfo.DealInfo()
                                 {
                                     Id = (string)jDeal["Id"],
                                     Description = (string)jDeal["Description"],
                                     StartDate = (DateTime)jDeal["StartDate"],
                                     Title = (string)jDeal["Title"]
                                 };
                             });
                    }

                    return restaurant;
                }).ToList<RestaurantInfo>();
            }
            else
            {
                return new List<RestaurantInfo>();
            }
        }

        /// <summary>
        /// Given the restaurant info get the description for the restaurant and return the same RestaurantInfo
        /// object updated with the description.
        /// </summary>
        /// <param name="restaurantInfo">The restaurant to get description for</param>
        /// <returns>The updated restaurant info</returns>
        public async Task<RestaurantInfo> GetRestaurantDescription(RestaurantInfo restaurantInfo)
        {
            var command = new LECommand("GetRestauranDescByAddyID");

            command.AddParam("format", "json");
            command.AddParam("addyid", restaurantInfo.LocId);

            JObject json = await this.DoGet(command);

            JObject restaurantObj = (JObject)json["Restaurant"];

            restaurantInfo.Description = String.Format("{0} {1}", (string)restaurantObj["RestDesc"], (string)restaurantObj["RestCite"]);
            restaurantInfo.Serves = (string)restaurantObj["RestAddInfo"];

            restaurantInfo.IsFullyLoaded = true;

            return restaurantInfo;
        }

        /// <summary>
        /// Given the restaurant city info and filter options return the list of restaurant category information objects.
        /// </summary>
        /// <param name="city">Restaurant city info</param>
        /// <param name="filterOptions">Filter options</param>
        /// <returns>List of restaurant category info objects</returns>
        public async Task<IEnumerable<RestaurantCategoryInfo>> GetCategoryInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            var command = new LECommand("GetCategoryListCount");

            command.AddParam("format", "json");
            command.AddParam("cityid", city.Id);

            if (filterOptions != null)
            {
                foreach (var filterOption in filterOptions.AllOptions)
                {
                    command.AddParam(filterOption.Name, filterOption.Value);
                }
            }

            JObject json = await this.DoGet(command);

            JArray categoryInfos = (JArray)json["CategoryFilterCounts"];

            return categoryInfos.Select<JToken, RestaurantCategoryInfo>(jsonToken =>
            {
                return new RestaurantCategoryInfo() {
                        Id = (string)jsonToken["CategoryID"],
                        Name = (string)jsonToken["CategoryName"],
                        RestaurantCount = (int)jsonToken["CategoryCount"]
                };
            });
        }

        public async Task<IEnumerable<NeighborhoodInfo>> GetNeighborhoodInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            var command = new LECommand("GetHoodListCount");

            command.AddParam("format", "json");
            command.AddParam("cityid", city.Id);

            if (filterOptions != null)
            {
                foreach (var filterOption in filterOptions.AllOptions)
                {
                    command.AddParam(filterOption.Name, filterOption.Value);
                }
            }

            JObject json = await this.DoGet(command);

            JArray neighborhoodInfos = (JArray)json["HoodFilterCount"];

            return neighborhoodInfos.Select<JToken, NeighborhoodInfo>(jsonToken =>
            {
                return new NeighborhoodInfo()
                {
                    Id = (string)jsonToken["HoodID"],
                    Name = (string)jsonToken["HoodName"],
                    RestaurantCount = (int)jsonToken["HoodCount"]
                };
            });
        }

        public async Task<IEnumerable<RestaurantAmenityInfo>> GetAmenityInfoListFor(CityInfo city, FilterOptions filterOptions)
        {
            var command = new LECommand("GetAmmendityListCount");

            command.AddParam("format", "json");
            command.AddParam("cityid", city.Id);

            if (filterOptions != null)
            {
                foreach (var filterOption in filterOptions.AllOptions)
                {
                    command.AddParam(filterOption.Name, filterOption.Value);
                }
            }

            JObject json = await this.DoGet(command);

            JArray amenityInfos = (JArray)json["AmendityFilterCount"];

            return amenityInfos.Select<JToken, RestaurantAmenityInfo>(jsonToken =>
            {
                return new RestaurantAmenityInfo()
                {
                    Id = (string)jsonToken["AmendityID"],
                    Name = (string)jsonToken["AmendityName"],
                    RestaurantCount = (int)jsonToken["AmendityCount"]
                };
            });
        }

        private async Task<JObject> DoGet(LECommand command)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Executing GET: "+command.ToUri());
                HttpWebRequest request = WebRequest.Create(command.ToUri()) as HttpWebRequest;

                using (HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new DataException(String.Format("Server error (HTTP {0}: {1}).",
                                                response.StatusCode,
                                                response.StatusDescription));
                    }

                    if (response.ContentLength == 0)
                    {
                        return null;                            
                    }

                    var reader = new StreamReader(response.GetResponseStream());
                    
                    return JObject.Parse(await reader.ReadToEndAsync());
                }
            }
            catch (DataException dex)
            {
                throw dex;
            }
            catch (Exception e)
            {
                throw new DataException(e);
            }

        }

        private class LECommand
        {
            private static readonly string BASE = @"http://apiv2.localeats.com/Mobile/{0}?skey={1}&{2}";

            private string _apiKey;
            private string _command;
            private Dictionary<string,string> _params = new Dictionary<string, string>();

            public LECommand(string command)
            {
                System.Diagnostics.Debug.Assert(String.IsNullOrEmpty(command) == false);

                this._command = command;

                var rm = new ResourceManager("LocalEats.Properties.Resources", this.GetType().GetTypeInfo().Assembly);

                this._apiKey = rm.GetString("LEApiKey");
            }

            public void AddParam(string name, string value)
            {
                this._params[name] = value;
            }

            public Uri ToUri()
            {
                var queryString = _params.ToQueryString(true);

                var uriStr = String.Format(BASE, _command, _apiKey, queryString);

                return new Uri(uriStr);
            }

			public override bool Equals (object obj)
			{
				LECommand r = obj as LECommand; 
				if (r == null)
					return false;
				else 
					return this.ToUri ().Equals(r.ToUri ());
			}

			public override int GetHashCode ()
			{
				return this.ToUri ().GetHashCode ();
			}
        }
    }
}
