using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LocalEats.Data
{
    public static class DataExtensions
    {
        #region Dictionary Extensions
        public static string ToQueryString(this Dictionary<string, string> source, bool removeEmptyEntries)
        {
            return source != null ?
                   String.Join("&",
                        source.Select<KeyValuePair<string, string>, string>(kv => {
                            return String.Format("{0}={1}",
                                WebUtility.UrlEncode(kv.Key),
                                kv.Value != null ? WebUtility.UrlEncode(kv.Value) : string.Empty
                            );
                        }).ToArray<string>()
                   ) : 
                   string.Empty;
        }
        #endregion
    }
}
