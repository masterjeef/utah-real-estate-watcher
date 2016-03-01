using System;
using UtahRealEstateWatcher.Readers;
using UtahRealEstateWatcher.Models;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics;

namespace UtahRealEstateWatcher
{
    class Program
    {
        private const string lastRunFileName = "LastRun.json";

        private static UtahRealEstateFileReader UreFileReader = new UtahRealEstateFileReader(lastRunFileName);

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No cities specified");
                return;
            }

            var cities = args[0].Split(';');
            var minPrice = getIntArg(args, 1);
            var maxPrice = getIntArg(args, 2);

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

            Console.WriteLine("{0} total listings found.", listings.Count);

            var listingsFromFile = UreFileReader.GetListings();

            WriteListingsToFile(listings, lastRunFileName);

            var newListings = listings.Except(listingsFromFile).ToList();

            Console.WriteLine("{0} new listings found.", newListings.Count);
            
            var listingHtml = string.Join("\n", newListings.Select(x => x.Html));

            var html = string.Format("<html><head><title>New Listings</title></head><body><h1>{0} New Listings</h1>{1}</body></html>", newListings.Count, listingHtml);

            var htmlPath = string.Format("{0}.html", DateTime.Now.Ticks);

            File.WriteAllText(htmlPath, html);

            Process.Start(htmlPath);
        }

        private static int getIntArg(string [] args, int index)
        {
            int value = 0;

            if(args.Length > index)
            {
                value = int.Parse(args[index]);
            }

            return value;
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
