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
                    MinPrice = 225000,
                    MaxPrice = 275000
                }
            };

            var listings = listingReader.GetListings();
            
            foreach (var mls in listings)
            {
                Console.WriteLine("MLS : {0}", mls);
            }

            Console.WriteLine("{0} total listings", listings.Count);

            Console.ReadLine();
        }
    }
}
