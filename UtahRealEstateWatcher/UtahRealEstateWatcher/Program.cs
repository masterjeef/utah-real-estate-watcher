using System;
using UtahRealEstateWatcher.Readers;
using UtahRealEstateWatcher.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;
using EasyArgs;

namespace UtahRealEstateWatcher
{
    class Program
    {
        private const string lastRunFileName = "LastRun.json";

        private const string stylesPath = "Assets/css/styles.css";

        private static UtahRealEstateFileReader UreFileReader = new UtahRealEstateFileReader(lastRunFileName);

        static void Main(string[] args)
        {
            var easyArgs = new Args(args);

            var citiesArg = easyArgs["Cities"].Value;

            if (citiesArg == null)
            {
                Console.WriteLine("No cities specified. Press any key to close.");
                Console.ReadLine();
                return;
            }

            var cities = citiesArg.Split(';');
            var minPrice = easyArgs["MinPrice"].AsInt();
            var maxPrice = easyArgs["MaxPrice"].AsInt();

            var listings = new List<UreListing>();
            var reader = new UtahRealEstateReader();

            foreach(var city in cities)
            {
                Console.WriteLine("Pulling listings for {0}", city);

                var criteria = new SearchCriteria
                {
                    City = city,
                    MinPrice = minPrice,
                    MaxPrice = maxPrice
                };

                reader.Criteria = criteria;

                listings.AddRange(reader.GetListings());
            }

            var listingsFromFile = UreFileReader.GetListings();

            WriteListingsToFile(listings, lastRunFileName);

            if(easyArgs.HasFlag("d"))
            {
                File.Delete(lastRunFileName);
            }

            var newListings = listings.Except(listingsFromFile).ToList();

            var newListingsHtml = string.Join("\n", newListings.Select(x => x.Html));
            var allListingsHtml = string.Join("\n", listings.Select(x => x.Html));

            var styles = string.Format("<link type=\"text/css\" rel=\"stylesheet\" href=\"{0}\">", stylesPath);
            var html = string.Format("<html><head><title>New Listings</title>{2}</head><body><div class=\"main\"><h1>{0} New Listings</h1>{1}<hr><h1>All {4} Listings</h1>{3}</div></body></html>", newListings.Count, newListingsHtml, styles, allListingsHtml, listings.Count);

            var htmlPath = string.Format("{0}.html", DateTime.Now.Ticks);

            File.WriteAllText(htmlPath, html);

            Process.Start(htmlPath);
        }


        private static void WriteListingsToFile(List<UreListing> listings, string path)
        {
            var json = JsonConvert.SerializeObject(listings, Formatting.Indented);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, json);
        }
    }
}
