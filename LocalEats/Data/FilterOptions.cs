using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalEats.Data
{
    public class FilterOptions
    {
//- &tfil=1 – Will bring back only restaurants that are Top10 award winners.
//- &bfil= 1 – Will bring back only restaurants that are Best Of category award
//winners.
//- &pfil= 1 – Will bring back only restaurants that are 1 $ prices.You can can
//comma delimitate more than one. For example &pfil= 1,2
//- &cfil= 52 – Will bring back only restaurants that contain the specified
//categories by their category IDs. You can can comma delimitate more than
//one.
//- &hfil= 2 – Will bring back only restaurants that are in the specified
//neighborhoods by their hood IDs.You can can comma delimitate more
//than one.
//- &afil= 4 – Will bring back only restaurants that contain the specified
//features by their feature IDs. You can can comma delimitate more than
//one.
//- &sfil= 1 – Will bring back only restaurants that serve the specified meal
//times.You can can comma delimitate more than one.
//- &SortExpr= rest_name – Will sort the results by alpha in regards to the
//name. Not having this parameter will order the results by distance.
        public static readonly FilterOption TOP_10 = new FilterOption("tfil","1");
        public static readonly FilterOption BEST_OF_WINNER = new FilterOption("bfil", "1");
        public static readonly FilterOption PRICE_1 = new FilterOption("pfil", "1");
        public static readonly FilterOption PRICE_2 = new FilterOption("pfil", "2");
        public static readonly FilterOption PRICE_3 = new FilterOption("pfil", "3");
        public static readonly FilterOption PRICE_4 = new FilterOption("pfil", "4");

        // There default to empty.  You add to them once you pinpoint the option you
        // want.
        public static readonly FilterOption CATEGORIES = new FilterOption("cfil", "");
        public static readonly FilterOption NEIGHBORHOODS = new FilterOption("hfil", "");
        public static readonly FilterOption AMENITY_FEATURES = new FilterOption("afil", "");
        public static readonly FilterOption MEAL_TIMES = new FilterOption("sfil", "");

        // Default sort is distance from location but you can override that using this filter option.
        public static readonly FilterOption SORT_BY_NAME = new FilterOption("SortExpr", "rest_name");

        public static readonly FilterOptions EMPTY_OPTIONS = new FilterOptions();

        public IEnumerable<FilterOption> AllOptions {get; set;}

        public FilterOptions()
        {
            this.AllOptions = new List<FilterOption>();
        }

        public FilterOptions(FilterOption firstOption)
        {
            this.AllOptions = new List<FilterOption>();

            this.Add(firstOption);
        }

        public FilterOptions Add(params FilterOption[] otherFilterOptions)
        {
            this.AllOptions = this.AllOptions.Concat(otherFilterOptions);

            return this;
        }

        public static FilterOptions NewWith(params FilterOption[] otherFilterOptions)
        {
            var options = new FilterOptions();

            return options.Add(otherFilterOptions);
        }
    }

    public class FilterOption
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public FilterOption(string name, string value)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(name) && value  != null);

            this.Name = name;
            this.Value = value;    
        }

        public FilterOption AddValue(string otherValue)
        {
            System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(otherValue));

            if (String.IsNullOrEmpty(this.Value))
            {
                this.Value = otherValue;
            }
            else
            {
                this.Value = String.Join(",", this.Value, otherValue);
            }
            
            return this;
        }
    }
}
