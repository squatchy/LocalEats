using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalEats.Model
{
    public enum Award
    {
        EditorsPick = 1,
        Unknown = -1
    }

    public class RestaurantInfo : BaseModel
    {
        public String Data
        {
            get;
            set;
        }

        public bool IsFullyLoaded
        {
            get;
            set;
        }

        public String LocId
        {
            get;
            set;
        }

        public String Name
        {
            get;
            set;
        }

        public String StreetAddress
        {
            get;
            set;
        }

        public String CityStateZip
        {
            get;
            set;
        }

        public String Description
        {
            get;
            set;
        }

        public String PhoneNumber
        {
            get;
            set;
        }

        public String PriceRange
        {
            get;
            set;
        }

        public String Serves
        {
            get;
            set;
        }

        public Double Latitude
        {
            get;
            set;
        }

        public Double Longitude
        {
            get;
            set;
        }

        public String ImageUri
        {
            get;
            set;
        }

        public Decimal? SpatialOffset
        {
            get;
            set;
        }

        public Boolean IsEditorsPick
        {
            get;
            set;
        }

        public IEnumerable<AwardInfo> AwardList
        {
            get;
            set;
        }

        public IEnumerable<CategoryInfo> CategoryList
        {
            get;
            set;
        }

        public IEnumerable<NeighborhoodInfo> NeighborhoodList
        {
            get;
            set;
        }

        public IEnumerable<AmenityInfo> AmenityList
        {
            get;
            set;
        }

        public IEnumerable<DealInfo> DealList
        {
            get;
            set;
        }

        public RestaurantInfo()
        {
            this.AwardList = new List<AwardInfo>();
            this.CategoryList = new List<CategoryInfo>();
            this.NeighborhoodList = new List<NeighborhoodInfo>();
            this.AmenityList = new List<AmenityInfo>();
            this.DealList = new List<DealInfo>();
        }

        public void SaveRestaurantImageToFile(string fileName)
        {
            // save the restaurants image to a file
            using (var wc = new WebClient())
            {
                wc.DownloadFile(new Uri(ImageUri), fileName);
            }
        }

        #region Inner Helper Classes
        public class AwardInfo : BaseModel
        {
            public string Name { get; set; }
        }

        public class CategoryInfo : BaseModel
        {
            public string Name { get; set; }
            public bool IsBestOf { get; set; }                                    
        }

        public class NeighborhoodInfo : BaseModel
        {
            public string Name { get; set; }
        }

        public class AmenityInfo : BaseModel
        {
            public string LocId { get; set; }
            public string Name { get; set; }
        }

        public class DealInfo : BaseModel
        {
            public string Title { get; set; }
            public DateTime StartDate { get; set; }
            public string Description { get; set; }
        }
        #endregion
    }
}
