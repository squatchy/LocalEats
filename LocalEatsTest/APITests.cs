using System;
using System.Linq;

using NUnit.Framework;
using LocalEats;
using LocalEats.Data;
using LocalEats.Model;

namespace LocalEatsTest
{
    [TestFixture]
    public class APITests
    {
        [Test]
        public async void TestGetStateList()
        {
            var leClient = new LEClient();

            var states = await leClient.GetStatesList();

            Assert.IsNotNull(states);
            Assert.That(states.Count(), Is.GreaterThan(0));
        }

        [Test]
        public async void TestGetCityList()
        {
            var leClient = new LEClient();

            var states = await leClient.GetStatesList();

            var tn = states.First(si => si.Name == "Tennessee");

            var cities = await leClient.GetCityList(tn);

            Assert.IsNotNull(cities);
            Assert.That(cities.Count(), Is.GreaterThan(0));
            Assert.That(cities.FirstOrDefault(city => city.Name == "Nashville"), Is.Not.Null);
        }

        [Test]
        public async void TestGetCategoryInfoList()
        {
            var leClient = new LEClient();

            var states = await leClient.GetStatesList();

            var tn = states.First(si => si.Name == "Tennessee");

            var cities = await leClient.GetCityList(tn);

            Assert.IsNotNull(cities);
            Assert.That(cities.Count(), Is.GreaterThan(0));

            var nashville = cities.FirstOrDefault(city => city.Name == "Nashville");
            Assert.That(nashville, Is.Not.Null);

            var filterOptions = new FilterOptions(FilterOptions.TOP_10);

            var categories = await leClient.GetCategoryInfoListFor(nashville, filterOptions);

            Assert.IsNotNull(categories);
            Assert.That(categories.Count(), Is.GreaterThan(0));
            var first = categories.First();

            Assert.NotNull(first.Id);
            Assert.NotNull(first.Name);
            Assert.NotNull(first.RestaurantCount);
        }

        [Test]
        public async void TestGetNeighborhoodInfoList()
        {
            var leClient = new LEClient();

            var states = await leClient.GetStatesList();

            var tn = states.First(si => si.Name == "Tennessee");

            var cities = await leClient.GetCityList(tn);

            Assert.IsNotNull(cities);
            Assert.That(cities.Count(), Is.GreaterThan(0));

            var nashville = cities.FirstOrDefault(city => city.Name == "Nashville");
            Assert.That(nashville, Is.Not.Null);

            var filterOptions = new FilterOptions(FilterOptions.TOP_10);

            var neighborhoods = await leClient.GetNeighborhoodInfoListFor(nashville, filterOptions);

            Assert.IsNotNull(neighborhoods);
            Assert.That(neighborhoods.Count(), Is.GreaterThan(0));
            var first = neighborhoods.First();

            Assert.NotNull(first.Id);
            Assert.NotNull(first.Name);
            Assert.NotNull(first.RestaurantCount);
        }

        [Test]
        public async void TestGetAmendityInfoList()
        {
            var leClient = new LEClient();

            var states = await leClient.GetStatesList();

            var tn = states.First(si => si.Name == "Tennessee");

            var cities = await leClient.GetCityList(tn);

            Assert.IsNotNull(cities);
            Assert.That(cities.Count(), Is.GreaterThan(0));

            var nashville = cities.FirstOrDefault(city => city.Name == "Nashville");
            Assert.That(nashville, Is.Not.Null);

            var filterOptions = new FilterOptions(FilterOptions.TOP_10);

            var amendities = await leClient.GetAmenityInfoListFor(nashville, filterOptions);

            Assert.IsNotNull(amendities);
            Assert.That(amendities.Count(), Is.GreaterThan(0));
            var first = amendities.First();

            Assert.NotNull(first.Id);
            Assert.NotNull(first.Name);
            Assert.NotNull(first.RestaurantCount);
        }

        [Test]
        public async void TestGetRestaurantList()
        {
            var leClient = new LEClient();

            var states = await leClient.GetStatesList();

            var tn = states.First(si => si.Name == "Tennessee");

            var cities = await leClient.GetCityList(tn);

            Assert.IsNotNull(cities);
            Assert.That(cities.Count(), Is.GreaterThan(0));

            var nashville = cities.FirstOrDefault(city => city.Name == "Nashville");
            Assert.That(nashville, Is.Not.Null);

            var filterOptions = FilterOptions.NewWith(
                                    FilterOptions.TOP_10, 
                                    FilterOptions.PRICE_3, 
                                    FilterOptions.BEST_OF_WINNER);

            var restaurants = await leClient.GetRestaurantListNearBy(filterOptions, orCity: nashville);

            Assert.IsNotNull(restaurants);
            Assert.That(restaurants.Count(), Is.GreaterThan(0));
            
            Assert.That(restaurants.Any(ri => ri.CategoryList.Count() > 0), Is.True);
            Assert.That(restaurants.Any(ri => ri.AmenityList.Count() > 0), Is.True);
            Assert.That(restaurants.Any(ri => ri.AwardList.Count() > 0), Is.True);
            Assert.That(restaurants.Any(ri => ri.DealList.Count() > 0), Is.True);
            Assert.That(restaurants.Any(ri => ri.NeighborhoodList.Count() > 0), Is.True);

            // check that restaurant description loads
            var aRestaurant = restaurants.First();
            aRestaurant = await leClient.GetRestaurantDescription(aRestaurant);

            Assert.That(aRestaurant.Description,Is.Not.Null);


            // check category
            aRestaurant = restaurants.First(ri => ri.CategoryList.Count() > 0);
            var category = aRestaurant.CategoryList.First();
            Assert.That(category.Id, Is.Not.Null);
            Assert.That(category.Name, Is.Not.Null);
            Assert.That(category.IsBestOf, Is.Not.Null);

            // check amenity
            aRestaurant = restaurants.First(ri => ri.AmenityList.Count() > 0);
            var amenity = aRestaurant.AmenityList.First();
            Assert.That(amenity.Id, Is.Not.Null);
            Assert.That(amenity.Name, Is.Not.Null);
            Assert.That(amenity.LocId, Is.Not.Null);

            // check award
            aRestaurant = restaurants.First(ri => ri.AwardList.Count() > 0);
            var award = aRestaurant.AwardList.First();
            Assert.That(award.Id, Is.Not.Null);
            Assert.That(award.Name, Is.Not.Null);

            // check deal
            aRestaurant = restaurants.First(ri => ri.DealList.Count() > 0);
            var deal = aRestaurant.DealList.First();
            Assert.That(deal.Id, Is.Not.Null);
            Assert.That(deal.Title, Is.Not.Null);
            Assert.That(deal.Description, Is.Not.Null);
            Assert.That(deal.StartDate, Is.Not.Null);

            // check neighborhood
            aRestaurant = restaurants.First(ri => ri.NeighborhoodList.Count() > 0);
            var hood = aRestaurant.NeighborhoodList.First();
            Assert.That(hood.Id, Is.Not.Null);
            Assert.That(hood.Name, Is.Not.Null);
        }


    }
}
