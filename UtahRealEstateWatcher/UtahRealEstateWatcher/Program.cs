using System;
using UtahRealEstateWatcher.Readers;
using UtahRealEstateWatcher.Models;

namespace UtahRealEstateWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var listingReader = new UtahRealEstateReader
            {
                Criteria = new SearchCriteria
                {
                    City = "Lehi",
                    MinPrice = 200000,
                    MaxPrice = 280000
                }
            };

            var listings = listingReader.GetListings();
            
            foreach (var listing in listings)
            {
                Console.WriteLine("MLS : {0}", listing.Mls);
            }

            Console.WriteLine("{0} total listings", listings.Count);

            Console.ReadLine();
        }
    }
}
